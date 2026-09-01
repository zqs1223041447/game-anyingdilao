using System;
using System.Collections.Generic;
using Core;
using Core.Settings;
using Core.Teleport;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using Dialog;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Level.LevelStates;
using Scenes;
using UI.Panels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mijing;

public class MijingManager : SingletonMonoScope<MijingManager>
{
	private bool mijingEnabled;

	[Header("Debug测试 存档是否直接解锁秘境")]
	public bool MijingUnlockedDebug;

	[Header("Config")]
	public MijingSettings mijingSettings;

	private static int currentScore;

	private int currentFloor = 1;

	private static int currentNeedScore;

	public static List<string> mijingIds = new List<string>();

	private bool portalSpawned;

	private bool nextFloorPortalUnlocked;

	private Vector3 nextFloorPortalPosition;

	private int nextFloorPortalTargetFloor;

	private MijingPortal nextFloorPortalInstance;

	private bool isEnteringMijing;

	public DifficultType CurrentDifficulty { get; private set; }

	public int CurrentFloor
	{
		get
		{
			return Mathf.Max(1, currentFloor);
		}
		set
		{
			currentFloor = value;
		}
	}

	public static int CurrentScore
	{
		get
		{
			return Mathf.Max(0, currentScore);
		}
		set
		{
			currentScore = value;
		}
	}

	public static int CurrentNeedScore
	{
		get
		{
			return Mathf.Max(0, currentNeedScore);
		}
		set
		{
			currentNeedScore = value;
		}
	}

	public bool IsEnteringMijing => isEnteringMijing;

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		mijingEnabled = SettingsLoader.Instance.MijingToggle;
		SetCurrentFloor(0);
	}

	private void Start()
	{
		if (SaveManager.HasRuntime && !SaveManager.RuntimeData.UnlockedMijing && mijingEnabled && MijingUnlockedDebug)
		{
			SaveManager.RuntimeData.UnlockedMijing = true;
			if (SingletonMonoScope<DialogManager>.HasInstance)
			{
				SingletonMonoScope<DialogManager>.Instance.MarkTriggered("mijing_unlocked");
			}
		}
		if (SingletonMonoScope<LevelManager>.HasInstance)
		{
			mijingIds = LevelManager.GetAllMijingLevelIds();
		}
	}

	public int GetUnlockedFloorByCurrentDifficulty()
	{
		if (!SaveManager.HasRuntime)
		{
			return 1;
		}
		return CurrentDifficulty switch
		{
			DifficultType.Easy => Mathf.Max(1, SaveManager.RuntimeData.mijingFloor_easy), 
			DifficultType.Medium => Mathf.Max(1, SaveManager.RuntimeData.mijingFloor_medium), 
			DifficultType.Hard => Mathf.Max(1, SaveManager.RuntimeData.mijingFloor_hard), 
			DifficultType.Master => Mathf.Max(1, SaveManager.RuntimeData.mijingFloor_master), 
			_ => 1, 
		};
	}

	public void SetUnlockedFloorByCurrentDifficulty(int floor)
	{
		if (SaveManager.HasRuntime)
		{
			floor = Mathf.Max(1, floor);
			switch (CurrentDifficulty)
			{
			case DifficultType.Easy:
				SaveManager.RuntimeData.mijingFloor_easy = floor;
				break;
			case DifficultType.Medium:
				SaveManager.RuntimeData.mijingFloor_medium = floor;
				break;
			case DifficultType.Hard:
				SaveManager.RuntimeData.mijingFloor_hard = floor;
				break;
			case DifficultType.Master:
				SaveManager.RuntimeData.mijingFloor_master = floor;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public void SetUnlockedFloorByCurrentDifficultyMax(int floor)
	{
		int unlockedFloorByCurrentDifficulty = GetUnlockedFloorByCurrentDifficulty();
		if (floor > unlockedFloorByCurrentDifficulty)
		{
			SetUnlockedFloorByCurrentDifficulty(floor);
		}
	}

	public void EnterMijing(int floor)
	{
		if (isEnteringMijing)
		{
			LogUtil.Warn("当前正在进入秘境，忽略重复调用 EnterMijing");
		}
		else
		{
			EnterMijingInternal(floor).Forget();
		}
	}

	private async UniTaskVoid EnterMijingInternal(int floor)
	{
		isEnteringMijing = true;
		try
		{
			ClearNextFloorPortalState();
			currentScore = 0;
			SetUnlockedFloorByCurrentDifficultyMax(floor);
			if (SingletonMonoGlobal<StateDataManager>.HasInstance)
			{
				SingletonMonoGlobal<StateDataManager>.Instance.ClearAllPortalStatesInMijing();
			}
			if (SceneManager.GetActiveScene().name != "HomeScene" && !LevelManager.GetIsMijing())
			{
				LogUtil.Error("禁止在非主城场景且非秘境进入秘境！");
				return;
			}
			if (mijingIds == null || mijingIds.Count == 0)
			{
				LogUtil.Error("mijingIds 为空");
				return;
			}
			string levelId = mijingIds[UnityEngine.Random.Range(0, mijingIds.Count)];
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromMijing
			});
			SetCurrentFloor(floor);
			if (SingletonMonoScope<GameUIManager>.HasInstance)
			{
				SingletonMonoScope<GameUIManager>.Instance.RefreshMijing(floor, CurrentScore, mijingSettings.needScore);
			}
			await SceneLoadManager.LoadLevelScene(levelId, SceneTransitionMode.Fade);
		}
		catch (Exception arg)
		{
			LogUtil.Error($"进入秘境失败: {arg}");
		}
		finally
		{
			isEnteringMijing = false;
		}
	}

	public void RegisterNeedScore(int score)
	{
		CurrentNeedScore = score;
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.RefreshMijing(currentFloor, currentScore, currentNeedScore);
		}
		RestoreNextFloorPortal();
	}

	public void SpawnPortal()
	{
		if (portalSpawned || CurrentScore < CurrentNeedScore)
		{
			return;
		}
		if (!nextFloorPortalUnlocked)
		{
			Vector3 vector = Vector3.zero;
			if (SingletonMonoScope<PortalManager>.HasInstance && SingletonMonoScope<PlayerManager>.HasInstance)
			{
				vector = PortalManager.GetPortalSpawnPos(SingletonMonoScope<PlayerManager>.Instance.transform.position);
			}
			nextFloorPortalUnlocked = true;
			nextFloorPortalPosition = vector;
			nextFloorPortalTargetFloor = GetNextFloor(1);
			GameManager.ShowTip(LOC.MM.GetMain("mijing_next_floor"));
		}
		RestoreNextFloorPortal();
	}

	public void RestoreNextFloorPortal()
	{
		if (!nextFloorPortalUnlocked || (portalSpawned && (bool)nextFloorPortalInstance) || !SingletonMonoScope<LevelManager>.HasInstance || !LevelManager.GetIsMijing())
		{
			return;
		}
		GameObject gameObject = Singleton<ResManager>.Instance.Load<GameObject>("World/Build/MijingPortal");
		if (!gameObject)
		{
			LogUtil.Error("未找到秘境传送门 prefab");
			return;
		}
		MijingPortal component = UnityEngine.Object.Instantiate(gameObject, nextFloorPortalPosition, Quaternion.identity).GetComponent<MijingPortal>();
		if ((bool)component)
		{
			component.Init((nextFloorPortalTargetFloor > 0) ? nextFloorPortalTargetFloor : GetNextFloor(1));
		}
		nextFloorPortalInstance = component;
		portalSpawned = true;
	}

	public void RestoreNextFloorPortalForCurrentScene()
	{
		if (nextFloorPortalUnlocked && !nextFloorPortalInstance)
		{
			portalSpawned = false;
		}
		RestoreNextFloorPortal();
	}

	private void ClearNextFloorPortalState()
	{
		portalSpawned = false;
		nextFloorPortalUnlocked = false;
		nextFloorPortalPosition = Vector3.zero;
		nextFloorPortalTargetFloor = 0;
		nextFloorPortalInstance = null;
	}

	public void AddCurrentScore(int score)
	{
		CurrentScore += score;
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.RefreshMijing(CurrentFloor, CurrentScore, CurrentNeedScore);
		}
		SpawnPortal();
	}

	public void SetCurrentScore(int score)
	{
		CurrentScore = score;
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.RefreshMijing(CurrentFloor, CurrentScore, CurrentNeedScore);
		}
		SpawnPortal();
	}

	public void SetCurrentFloor(int floor)
	{
		CurrentFloor = Mathf.Max(1, floor);
	}

	public int GetCurrentFloor()
	{
		return CurrentFloor;
	}

	public int GetNextFloor(int A)
	{
		return CurrentFloor + A;
	}

	public float GetEnemyHealthMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().EnemyHealth.Evaluate(floor);
	}

	public float GetEnemyDamageMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().EnemyDamage.Evaluate(floor);
	}

	public float GetEnemyDamageReductionMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().EnemyDamageReduction.Evaluate(floor);
	}

	public float GetEnemyPenetrationMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().EnemyPenetration.Evaluate(floor);
	}

	public float GetPlayerDropRateMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().PlayerDropRate.Evaluate(floor);
	}

	public float GetEnterPriceMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().EnterPrice.Evaluate(floor);
	}

	public float GetEnemyXpMultiplier(int floor)
	{
		return GetEnemyXpMultiplier(floor, 1f);
	}

	public float GetEnemyXpMultiplier(int floor, float defaultBaseMultiplier)
	{
		return GetCurrentDifficultyConfig().EnemyXp.EvaluateFromFirstFloorWithFallback(floor, defaultBaseMultiplier, 1f);
	}

	public float GetRareItemDropRateMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().RareItemDropRate.EvaluateFromFirstFloorWithFallback(floor, 1f, 1f);
	}

	public float GetWPDamageMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().WP_DMG.EvaluateFromFirstFloorWithFallback(floor, 1f, 1f);
	}

	public float GetWPPRCMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().WP_PRC.EvaluateFromFirstFloorWithFallback(floor, 1f, 1f);
	}

	public float GetSPCDamageMultiplier(int floor)
	{
		return GetCurrentDifficultyConfig().SPC_DMG.EvaluateFromFirstFloorWithFallback(floor, 1f, 1f);
	}

	public float GetEnemyHealthMultiplier()
	{
		return GetCurrentDifficultyConfig().EnemyHealth.Evaluate(CurrentFloor);
	}

	public float GetEnemyDamageMultiplier()
	{
		return GetCurrentDifficultyConfig().EnemyDamage.Evaluate(CurrentFloor);
	}

	public float GetEnemyDamageReductionMultiplier()
	{
		return GetCurrentDifficultyConfig().EnemyDamageReduction.Evaluate(CurrentFloor);
	}

	public float GetEnemyPenetrationMultiplier()
	{
		return GetCurrentDifficultyConfig().EnemyPenetration.Evaluate(CurrentFloor);
	}

	public float GetPlayerDropRateMultiplier()
	{
		return GetCurrentDifficultyConfig().PlayerDropRate.Evaluate(CurrentFloor);
	}

	public float GetEnemyXpMultiplier()
	{
		return GetEnemyXpMultiplier(CurrentFloor);
	}

	public float GetEnemyXpMultiplier(float defaultBaseMultiplier)
	{
		return GetEnemyXpMultiplier(CurrentFloor, defaultBaseMultiplier);
	}

	public float GetRareItemDropRateMultiplier()
	{
		return GetRareItemDropRateMultiplier(CurrentFloor);
	}

	public float GetWPDamageMultiplier()
	{
		return GetWPDamageMultiplier(CurrentFloor);
	}

	public float GetWPPRCMultiplier()
	{
		return GetWPPRCMultiplier(CurrentFloor);
	}

	public float GetSPCDamageMultiplier()
	{
		return GetSPCDamageMultiplier(CurrentFloor);
	}

	public void ApplyDifficulty(DifficultType difficulty)
	{
		if (mijingEnabled)
		{
			CurrentDifficulty = difficulty;
		}
	}

	public int GetCurrentSceneQulity()
	{
		return CurrentDifficulty switch
		{
			DifficultType.Easy => 1, 
			DifficultType.Medium => 2, 
			DifficultType.Hard => 3, 
			DifficultType.Master => 4, 
			_ => 1, 
		};
	}

	private MijingDifficultyFormulaConfig GetCurrentDifficultyConfig()
	{
		if (!mijingSettings || !mijingEnabled)
		{
			LogUtil.Warn("MijingManager missing MijingDataConfig reference.");
			MijingDifficultyFormulaConfig result = default(MijingDifficultyFormulaConfig);
			result.EnemyHealth = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.EnemyDamage = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.PlayerDropRate = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f,
				MaxMultiplier = 1f
			};
			result.EnemyDamageReduction = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.EnemyPenetration = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.EnterPrice = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.EnemyXp = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.RareItemDropRate = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.WP_DMG = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.WP_PRC = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			result.SPC_DMG = new MijingFormulaParam
			{
				BaseMultiplier = 1f,
				FloorGrowthMultiplier = 1f
			};
			return result;
		}
		return mijingSettings.GetDifficultyConfig(CurrentDifficulty);
	}
}
