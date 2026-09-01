using System;
using Core.Teleport;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs;
using Interact;
using Level.LevelStates;
using Mijing;
using PostProcess;
using UI.DebugUI;
using UnityEngine;

namespace Scenes;

public class LevelSceneManager : SingletonMonoScene<LevelSceneManager>
{
	private const int DefaultSpecialBgmIndex = 0;

	public static event Action OnLevelSceneRestored;

	private async UniTask InitAsync()
	{
		SingletonMonoScope<PlayerManager>.Instance.lockMove = true;
		InteractionManager.AllInteractToggle = false;
		InputManager.AllActionToggle = false;
		await InitLevelAsync();
		try
		{
			await SingletonMonoScope<LevelManager>.Instance.WaitAllLevelPointsReadyAsync().Timeout(TimeSpan.FromSeconds(10.0));
		}
		catch (TimeoutException)
		{
			LogUtil.Error("关卡点位初始化超时，强制继续");
			SceneLoading.CloseLoadingUIOnce();
		}
		ApplyLevelSettings();
		InitMusic();
		FinalizeLevelReady();
		SingletonMonoGlobal<StateDataManager>.Instance.Init();
		RestoreState();
		if (SingletonMonoScope<PortalManager>.HasInstance)
		{
			SingletonMonoScope<PortalManager>.Instance.RestorePortals();
		}
		if (SingletonMonoScope<MijingManager>.HasInstance && SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetIsMijing())
		{
			SingletonMonoScope<MijingManager>.Instance.RestoreNextFloorPortalForCurrentScene();
		}
		LevelSceneManager.OnLevelSceneRestored?.Invoke();
		if (SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.RestoreAllDropItems();
		}
		if (SingletonMonoScene<LevelInteractablesManager>.HasInstance)
		{
			SingletonMonoScene<LevelInteractablesManager>.Instance.RestoreAll();
		}
		if (SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetIsMijing())
		{
			SingletonMonoScope<GameUIManager>.Instance.PosDisplay.Show(LOC.MM.GetLevelFormat("mijing_portal_display", SingletonMonoScope<MijingManager>.Instance.GetCurrentFloor()));
		}
		else
		{
			SingletonMonoScope<GameUIManager>.Instance.HideMijing();
			SingletonMonoScope<GameUIManager>.Instance.PosDisplay.Show(LOC.MM.GetLevel(LevelManager.GetCurLevelLocalKey()));
		}
		SingletonMonoGlobal<EnemyRuntimeUI>.Instance.RefreshUI();
		FinalizeLevelReady();
		await SingletonMonoGlobal<SceneFadeManager>.Instance.EnsureFadeIn();
	}

	private static async UniTask InitLevelAsync()
	{
		await UniTask.Yield();
		if (!SingletonMonoScope<LevelManager>.Instance.levelPrefab)
		{
			LogUtil.Error("LevelPrefab SO文件引用丢失");
			return;
		}
		string curLevel = LevelManager.GetCurLevel();
		SingletonMonoScope<LevelManager>.Instance.BeginLevel();
		if (!SingletonMonoScope<LevelManager>.HasInstance)
		{
			return;
		}
		GameObject gameObject;
		switch (LevelManager.GetLevelData(curLevel).Type)
		{
		case LevelType.Normal:
			if (SingletonMonoScope<LevelManager>.Instance.levelPrefab.Normal.Length == 0)
			{
				LogUtil.Error("LevelPrefab.Normal 未配置");
				return;
			}
			gameObject = SingletonMonoScope<LevelManager>.Instance.levelPrefab.Normal[LevelManager.GetLevelData(curLevel).prefabIndex];
			break;
		case LevelType.Boss:
			if (SingletonMonoScope<LevelManager>.Instance.levelPrefab.Boss.Length == 0)
			{
				LogUtil.Error("LevelPrefab.Boss 未配置");
				return;
			}
			gameObject = SingletonMonoScope<LevelManager>.Instance.levelPrefab.Boss[LevelManager.GetLevelData(curLevel).prefabIndex];
			break;
		case LevelType.Optional:
			if (SingletonMonoScope<LevelManager>.Instance.levelPrefab.Optional.Length == 0)
			{
				LogUtil.Error("LevelPrefab.Optional 未配置");
				return;
			}
			gameObject = SingletonMonoScope<LevelManager>.Instance.levelPrefab.Optional[LevelManager.GetLevelData(curLevel).prefabIndex];
			break;
		case LevelType.Challenge:
			if (SingletonMonoScope<LevelManager>.Instance.levelPrefab.Challenge.Length == 0)
			{
				LogUtil.Error("LevelPrefab.Challenge 未配置");
				return;
			}
			gameObject = SingletonMonoScope<LevelManager>.Instance.levelPrefab.Challenge[LevelManager.GetLevelData(curLevel).prefabIndex];
			break;
		case LevelType.Mijing:
			if (SingletonMonoScope<LevelManager>.Instance.levelPrefab.Mijing.Length == 0)
			{
				LogUtil.Error("LevelPrefab.Mijing 未配置");
				return;
			}
			gameObject = SingletonMonoScope<LevelManager>.Instance.levelPrefab.Mijing[LevelManager.GetLevelData(curLevel).prefabIndex];
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if ((bool)gameObject)
		{
			UnityEngine.Object.Instantiate(gameObject);
			if (SaveManager.HasRuntime && LevelManager.IsMainlineType(LevelManager.GetLevelData(curLevel).Type))
			{
				if (SaveManager.RuntimeData.UnlockedChapterIds.Add(LevelManager.GetChapterId(curLevel)))
				{
					LogUtil.Success($"成功解锁章节 ID : {LevelManager.GetChapterId(curLevel)}");
				}
				if (SaveManager.RuntimeData.UnlockedLevelIds.Add(curLevel))
				{
					LogUtil.Success("成功解锁关卡 ID : " + curLevel);
				}
			}
		}
		else
		{
			LogUtil.Error($"预制体不存在。levelType:{LevelManager.GetLevelData(curLevel).Type},levelId:{curLevel},prefabIndex:{LevelManager.GetLevelData(curLevel).prefabIndex}");
		}
	}

	private static void SetupPlayerPos()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			Vector3 position = SingletonMonoGlobal<PlayerSpawnManager>.Instance.ResolveForLevel();
			SingletonMonoScope<PlayerManager>.Instance.SetPosition(position);
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.ClearAllRequest();
			if (SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<PlayerManager>.Instance.HealStat.Cur == 0f)
			{
				SingletonMonoScope<PlayerManager>.Instance.SetPlayerReborn();
			}
		}
	}

	private static void ApplyLevelSettings()
	{
		string curLevel = LevelManager.GetCurLevel();
		SettingDataManager.level_light = LevelManager.GetLevelData(curLevel).Globle_Intensity;
		Singleton<SettingDataManager>.Instance.ApplyVideoSettings();
		SingletonMonoGlobal<PostProcessManager>.Instance.SetGlobalLightColor(LevelManager.GetLevelData(curLevel).Globle_Color);
	}

	private static void InitMusic()
	{
		if (!SingletonMonoScope<LevelManager>.HasInstance)
		{
			return;
		}
		string curLevel = LevelManager.GetCurLevel();
		string[] sD;
		if (LevelManager.GetIsChallenge())
		{
			sD = SingletonMonoGlobal<AudioManager>.Instance.musicData.challengeGroups.Level[0].SD;
			if (!SingletonMonoGlobal<AudioManager>.Instance.IsCurrentBGMPlaylist(sD) && sD.Length != 0)
			{
				SingletonMonoGlobal<AudioManager>.Instance.PlayBGM(sD, random: true);
			}
			return;
		}
		if (LevelManager.GetIsMijing())
		{
			sD = SingletonMonoGlobal<AudioManager>.Instance.musicData.mijingGroups.Level[0].SD;
			if (!SingletonMonoGlobal<AudioManager>.Instance.IsCurrentBGMPlaylist(sD) && sD.Length != 0)
			{
				SingletonMonoGlobal<AudioManager>.Instance.PlayBGM(sD, random: true);
			}
			return;
		}
		sD = SingletonMonoGlobal<AudioManager>.Instance.musicData.Level_BGM.Chapter[LevelManager.GetChapterId(curLevel) - 1].Level[LevelManager.GetLevelData(curLevel).BGM_Index].SD;
		if (SingletonMonoGlobal<AudioManager>.Instance.IsCurrentBGMPlaylist(sD))
		{
			return;
		}
		if (sD.Length != 0)
		{
			SingletonMonoGlobal<AudioManager>.Instance.PlayBGM(sD, random: false);
		}
		if (sD.Length != 0)
		{
			SingletonMonoGlobal<AudioManager>.Instance.PlayBGM(sD, random: false);
		}
		if (LevelManager.GetLevelData(curLevel).ATOM_Index != 0)
		{
			string[] sD2 = SingletonMonoGlobal<AudioManager>.Instance.musicData.ATOM.Chapter[LevelManager.GetChapterId(curLevel) - 1].Level[LevelManager.GetLevelData(curLevel).ATOM_Index].SD;
			if (!SingletonMonoGlobal<AudioManager>.Instance.IsCurrentAtomPlaylist(sD2) && sD2.Length != 0)
			{
				SingletonMonoGlobal<AudioManager>.Instance.PlayAtmos(sD2, random: false);
			}
		}
		else
		{
			SingletonMonoGlobal<AudioManager>.Instance.StopAtmos();
		}
	}

	public void FinalNotifyLevelReady()
	{
		SetupPlayerPos();
		SingletonMonoScope<LevelManager>.Instance.NotifyAllLevelPointsReady();
	}

	private static void FinalizeLevelReady()
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			LogUtil.Error("玩家还未准备好");
			return;
		}
		if (SingletonMonoScope<MijingManager>.HasInstance && LevelManager.GetIsMijing())
		{
			SingletonMonoScope<PlayerManager>.Instance.ItemDrop_Rate_mijing_Tmp = SingletonMonoScope<MijingManager>.Instance.GetPlayerDropRateMultiplier() * 100f;
		}
		else
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
		SceneLoading.CloseLoadingUIOnce();
	}

	protected override void Awake()
	{
		base.Awake();
		InitAsync().Forget();
	}

	private void Start()
	{
		if (SaveManager.HasRuntime)
		{
			this.wait(0.001f, delegate
			{
				SaveManager.RequestSave();
			});
		}
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.CloseMainPanels();
		}
	}

	public void RestoreState()
	{
		if (SingletonMonoGlobal<StateDataManager>.HasInstance)
		{
			SingletonMonoGlobal<StateDataManager>.Instance.RestoreCompState();
		}
	}
}
