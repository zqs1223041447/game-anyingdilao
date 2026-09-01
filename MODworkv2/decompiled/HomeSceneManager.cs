using System;
using System.Collections.Generic;
using Core;
using Core.Teleport;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using Data.RuntimeData;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs;
using Interact;
using Level.LevelStates;
using Mijing;
using Scenes;
using UI.DebugUI;
using UI.Panels;
using UnityEngine;

public class HomeSceneManager : SingletonMonoScene<HomeSceneManager>
{
	[Header("无存档时加载的默认玩家职业")]
	public int playType = 3;

	[Header("测试用 是否自动生成地图")]
	public bool worldSpawn = true;

	private UniTaskCompletionSource<bool> worldReadyTcs;

	private int expectedStationCount;

	private int readyStationCount;

	public List<TeleportStation> stations = new List<TeleportStation>();

	public static event Action OnHomeSceneRestored;

	public void RegisterStation(TeleportStation station)
	{
		if (!stations.Contains(station))
		{
			stations.Add(station);
			expectedStationCount++;
		}
	}

	public void NotifyStationReady()
	{
		readyStationCount++;
		if (readyStationCount >= expectedStationCount)
		{
			worldReadyTcs.TrySetResult(result: true);
		}
	}

	public void Unregister(TeleportStation s)
	{
		if (stations.Remove(s))
		{
			expectedStationCount = Mathf.Max(0, expectedStationCount - 1);
			if (readyStationCount >= expectedStationCount)
			{
				worldReadyTcs.TrySetResult(result: true);
			}
		}
	}

	public Vector3 FindStationByChapter(int chapterId)
	{
		foreach (TeleportStation station in stations)
		{
			if ((bool)station && station.ChapterId == chapterId)
			{
				return station.transform.position;
			}
		}
		LogUtil.Warn("HomeSceneManager", $"未找到 ChapterId={chapterId} 的章节传送站");
		return Vector3.zero;
	}

	public async UniTask InitAsync()
	{
		SingletonMonoScope<PlayerManager>.Instance.lockMove = true;
		InteractionManager.AllInteractToggle = false;
		InputManager.AllActionToggle = false;
		if (!SaveManager.HasRuntime)
		{
			throw new Exception("未加载存档却进入 HomeScene");
		}
		GameManager.NotifyHomeSceneReady();
		HomePlayerSpawnRequest homeRequest = SingletonMonoGlobal<PlayerSpawnManager>.Instance.PeekHomeRequest();
		if (worldSpawn)
		{
			await InitWorldAsync();
			await SetupPlayerPosAsync();
		}
		ApplyLevelSettings();
		InitMusic(homeRequest);
		await FinalizeLevelReady();
		SingletonMonoGlobal<StateDataManager>.Instance.Init();
		RestoreState();
		if (SingletonMonoScope<PortalManager>.HasInstance)
		{
			SingletonMonoScope<PortalManager>.Instance.RestorePortals();
		}
		HomeSceneManager.OnHomeSceneRestored?.Invoke();
		if (SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.RestoreAllDropItems();
		}
		if (SingletonMonoScene<LevelInteractablesManager>.HasInstance)
		{
			SingletonMonoScene<LevelInteractablesManager>.Instance.RestoreAll();
		}
		SingletonMonoScope<GameUIManager>.Instance.PosDisplay.ClearImmediate();
		SingletonMonoScope<GameUIManager>.Instance.HideMijing();
		SingletonMonoGlobal<EnemyRuntimeUI>.Instance.RefreshUI();
		await FinalizeLevelReady();
		await SingletonMonoGlobal<SceneFadeManager>.Instance.EnsureFadeIn();
	}

	private async UniTask InitWorldAsync()
	{
		await UniTask.Yield();
		if (SingletonMonoScope<LevelManager>.HasInstance && !SingletonMonoScope<LevelManager>.Instance.levelPrefab)
		{
			LogUtil.Error("LevelPrefab SO文件引用丢失");
			return;
		}
		if (SingletonMonoScope<LevelManager>.HasInstance && !SingletonMonoScope<LevelManager>.Instance.levelPrefab.Home)
		{
			LogUtil.Error("LevelPrefab.Home 未配置");
			return;
		}
		UnityEngine.Object.Instantiate(SingletonMonoScope<LevelManager>.Instance.levelPrefab.Home);
		if (expectedStationCount == 0)
		{
			worldReadyTcs.TrySetResult(result: true);
		}
	}

	private static void ApplyLevelSettings()
	{
		SettingDataManager.level_light = 0f;
		Singleton<SettingDataManager>.Instance.ApplyVideoSettings();
	}

	private static void InitMusic(HomePlayerSpawnRequest request)
	{
		if (request != null && request.PlayHomeVictoryMusic)
		{
			SingletonMonoGlobal<AudioManager>.Instance.PlayHomeVictoryBGM();
		}
		else
		{
			SingletonMonoGlobal<AudioManager>.Instance.PlayHomeStartBGM(request?.PlayHomeStartFromFirst ?? false);
		}
	}

	private async UniTask SetupPlayerPosAsync()
	{
		await worldReadyTcs.Task;
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			Vector3 position = SingletonMonoGlobal<PlayerSpawnManager>.Instance.ResolveForHome();
			SingletonMonoScope<PlayerManager>.Instance.SetPosition(position);
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.ClearAllRequest();
			if (SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<PlayerManager>.Instance.HealStat.Cur == 0f)
			{
				SingletonMonoScope<PlayerManager>.Instance.SetPlayerReborn();
			}
		}
	}

	private static async UniTask FinalizeLevelReady()
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			LogUtil.Error("玩家还未准备好");
			return;
		}
		if (SingletonMonoScope<MijingManager>.HasInstance)
		{
			SingletonMonoScope<PlayerManager>.Instance.ItemDrop_Rate_mijing_Tmp = 0f;
		}
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			SingletonMonoScope<InputManager>.Instance.PrepareGameplayInputUnlock();
		}
		SingletonMonoScope<PlayerManager>.Instance.ResetActionStateForGameplayUnlock();
		SingletonMonoScope<PlayerManager>.Instance.lockMove = false;
		InteractionManager.AllInteractToggle = true;
		InputManager.AllActionToggle = true;
		SingletonMonoScope<PlayerManager>.Instance.CanMove = true;
		SingletonMonoScope<PlayerManager>.Instance.lockMove = false;
		await SceneLoading.CloseLoadingUIOnce();
		ShowDeadMoneyTip();
	}

	private static void InitTalent()
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
		}
	}

	private static void ShowDeadMoneyTip()
	{
		if (GlobalRuntimeData.HasPendingDeathLostMoney)
		{
			long pendingDeathLostMoney = GlobalRuntimeData.PendingDeathLostMoney;
			GlobalRuntimeData.ClearDeathLostMoney();
			GameManager.ShowTip((pendingDeathLostMoney <= 0) ? LOC.MM.GetMain("dead_money_lost_zero") : LOC.MM.GetMainFormat("dead_money_lost", pendingDeathLostMoney), TipType.Fail, 4f);
		}
	}

	public void RestoreState()
	{
		if (SingletonMonoGlobal<StateDataManager>.HasInstance)
		{
			SingletonMonoGlobal<StateDataManager>.Instance.RestoreCompState();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		expectedStationCount = 0;
		readyStationCount = 0;
		worldReadyTcs = new UniTaskCompletionSource<bool>();
		if (!worldSpawn)
		{
			worldReadyTcs.TrySetResult(result: true);
		}
		InitAsync().Forget();
	}

	private void Start()
	{
		InitTalent();
		if (SaveManager.HasRuntime)
		{
			this.wait(0.001f, delegate
			{
				SaveManager.RequestSave();
			});
		}
		if (SingletonMonoGlobal<AudioManager>.HasInstance)
		{
			SingletonMonoGlobal<AudioManager>.Instance.StopAtmos();
		}
		if (SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetCurLevel() != "Home")
		{
			LevelManager.SetCurLevel("Home");
		}
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.CloseMainPanels();
		}
	}
}
