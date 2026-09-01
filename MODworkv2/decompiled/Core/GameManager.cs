using Core.Settings;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using Data.SaveData;
using Dialog;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Inputs;
using Inputs.Cursors;
using Level.LevelStates;
using Scenes;
using UI.Panels;
using UnityEngine;

namespace Core;

public class GameManager : Singleton<GameManager>
{
	public static bool gameInited;

	private static bool isInited;

	public static Vector3 HomePlayerPos;

	public static Vector3 HomePortalPos;

	public static Vector3 MijingPos;

	public static void Init()
	{
		if (!isInited)
		{
			RuntimeManager.ToggleLogWarning(SettingsLoader.Instance.fmodToggle);
			InitInput();
			LogUtil.Info("游戏初始化完成");
			isInited = true;
		}
	}

	public static void NotifyHomeSceneReady()
	{
		if (!gameInited)
		{
			InitGame();
			gameInited = true;
		}
	}

	public static async UniTask StartGame(int slotId)
	{
		await UniTask.Yield(PlayerLoopTiming.Update);
		if (SingletonMonoGlobal<SceneFadeManager>.HasInstance)
		{
			await SingletonMonoGlobal<SceneFadeManager>.Instance.FadeOutAndWait();
		}
		if (!SaveManager.EnterGameWithSlot(slotId))
		{
			if (SingletonMonoGlobal<SceneFadeManager>.HasInstance)
			{
				await SingletonMonoGlobal<SceneFadeManager>.Instance.EnsureFadeIn();
			}
			LogUtil.Error($"加载存档失败: {slotId}");
			ShowTipLocalStartKey("save_load_fail", TipType.Fail);
		}
		else
		{
			LogUtil.Success($"成功加载存档: {slotId}");
			SaveManager.SaveLastSlot(slotId);
			InitPlayerTime();
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
			{
				Reason = HomePlayerSpawnReason.HomeDefault,
				PlayHomeStartFromFirst = true
			});
			SingletonMonoGlobal<StateDataManager>.Instance.Init(force: true);
			await SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade);
		}
	}

	public static async UniTask BackToMenu()
	{
		PlayTimeManager.StopCount();
		gameInited = false;
		await SceneLoadManager.LoadStartScene(SceneTransitionMode.Fade);
		while (SaveManager.IsSaving)
		{
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
		SaveManager.ResetSaveData();
		PlayTimeManager.Clear();
	}

	public static void InitInput()
	{
		GamepadDetectManager.OnGamepadConnectionChanged += HandleGamepadConnectionChanged;
		SingletonMonoGlobal<GamepadDetectManager>.Instance.Init();
		SingletonMonoGlobal<CurrentInputManager>.Instance.Init();
		SingletonMonoGlobal<GamepadUINavigationManager>.Instance.Init();
		SingletonMonoGlobal<VirtualCursorManager>.Instance.Init();
		SingletonMonoGlobal<CursorUIManager>.Instance.Init();
	}

	private static void HandleGamepadConnectionChanged(bool connected)
	{
		if (connected)
		{
			ShowTipLocalStartKey("gamepad_con", TipType.Success);
		}
		else
		{
			ShowTipLocalStartKey("gamepad_dis", TipType.Fail);
		}
	}

	public static void SetPlayerStartPos(Vector3 pos)
	{
		HomePlayerPos = pos;
	}

	public static Vector3 GetStartPos()
	{
		return HomePlayerPos;
	}

	public static void SetPortalStartPos(Vector3 pos)
	{
		HomePortalPos = pos;
	}

	public static void SetMijingPos(Vector3 pos)
	{
		MijingPos = pos;
	}

	public static Vector3 GetPortalPos()
	{
		return HomePortalPos;
	}

	public static Vector3 GetMijingPos()
	{
		return MijingPos;
	}

	public static void InitGame()
	{
		LoadSaveData();
	}

	public static void LoadSaveData()
	{
		if (SaveManager.HasRuntime)
		{
			SaveDataEquipmentSanitizer.PostLoadFix(SaveManager.RuntimeData);
			if (SaveManager.RuntimeGlobalData != null && SaveManager.RuntimeGlobalData.GlobalChestData != null)
			{
				SaveDataEquipmentSanitizer.SanitizeGlobalChestItems(SaveManager.RuntimeGlobalData.GlobalChestData.ChestItems);
			}
		}
		if (SaveManager.HasRuntime && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			SingletonMonoScope<PlayerManager>.Instance.InitFromSaveData(SaveManager.RuntimeData.PlayerData);
		}
		if (SaveManager.HasRuntime && SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.InitFromSaveData(SaveManager.RuntimeData.TalentData);
		}
		if (SaveManager.HasRuntime && SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.BeginRestoreFromSaveData(SaveManager.RuntimeData.ActbarData);
		}
		if (SaveManager.HasRuntime && SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<InventoryManager>.Instance.InitFromSaveData(SaveManager.RuntimeData.InventoryData);
		}
		if (SaveManager.HasRuntime && SingletonMonoScope<DialogManager>.HasInstance)
		{
			SingletonMonoScope<DialogManager>.Instance.InitFromSaveData(SaveManager.RuntimeData.DialogData);
		}
		if (SaveManager.HasRuntime && SingletonMonoScope<WarehouseManager>.HasInstance)
		{
			SingletonMonoScope<WarehouseManager>.Instance.InitFromSaveData(SaveManager.RuntimeGlobalData.GlobalChestData);
		}
	}

	private static void InitPlayerTime()
	{
		PlayTimeManager.ResetAndRun(SaveManager.RuntimeData.PlayTimeSeconds);
	}

	public static void ShowTip(string msg, TipType tipType = TipType.Normal, float stayTime = -1f, bool useCustomTextColor = false, Color textColor = default(Color))
	{
		Singleton<UIManager>.Instance.ShowPanel<TipPanel, TipPanelParam>(new TipPanelParam(msg, tipType, stayTime, useCustomTextColor, textColor), null, E_MainLayer.Top);
	}

	public static void ShowTipLocalStartKey(string msg, TipType tipType)
	{
		Singleton<UIManager>.Instance.ShowPanel<TipPanel, TipPanelParam>(new TipPanelParam(LOC.MM.GetStart(msg), tipType), null, E_MainLayer.Top);
	}
}
