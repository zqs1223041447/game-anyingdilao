using Cysharp.Threading.Tasks;
using Entity.Comp.CompanionAI;
using FinkFramework.Runtime.Scene;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Inputs;
using Level.LevelStates;
using UI.Panels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes;

public class SceneLoadManager : SingletonMonoGlobal<SceneLoadManager>
{
	public static async UniTask LoadStartScene(SceneTransitionMode mode)
	{
		ExecuteAllSceneLoad();
		if (SingletonMonoGlobal<StateDataManager>.HasInstance)
		{
			SingletonMonoGlobal<StateDataManager>.Instance.ClearAllState();
		}
		SingletonMonoGlobal<SessionManager>.Instance.DestroyScope(ProcessScope.Game);
		SingletonMonoGlobal<SessionManager>.Instance.CreateScope(ProcessScope.MainMenu);
		await RunSceneLoad("StartScene", mode);
	}

	public static async UniTask LoadHomeScene(SceneTransitionMode mode)
	{
		ExecuteAllSceneLoad();
		SingletonMonoGlobal<AudioManager>.Instance.ExitIntroLoopMode();
		SingletonMonoGlobal<SessionManager>.Instance.DestroyScope(ProcessScope.MainMenu);
		SingletonMonoGlobal<SessionManager>.Instance.CreateScope(ProcessScope.Game);
		string levelId = (SingletonMonoScope<LevelManager>.HasInstance ? LevelManager.GetCurLevel() : "Home");
		if (SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.FlushToState(levelId);
			SingletonMonoScope<ItemManager>.Instance.ClearAlive();
		}
		if (SingletonMonoScene<EnemyPointManager>.HasInstance)
		{
			SingletonMonoScene<EnemyPointManager>.Instance.FlushAll();
		}
		if (SingletonMonoScene<LevelInteractablesManager>.HasInstance)
		{
			SingletonMonoScene<LevelInteractablesManager>.Instance.FlushAll();
		}
		if (SingletonMonoGlobal<StateDataManager>.HasInstance && SingletonMonoScope<LevelManager>.HasInstance && SingletonMonoScope<CompanionManager>.HasInstance)
		{
			SingletonMonoGlobal<StateDataManager>.Instance.SaveCompsState();
		}
		if (SingletonMonoScope<LevelManager>.HasInstance)
		{
			LevelManager.SetCurLevel("Home");
		}
		await RunSceneLoad("HomeScene", mode);
	}

	public static async UniTask LoadLevelScene(string levelId, SceneTransitionMode mode, int sceneQulity = -1)
	{
		if (levelId == "Home")
		{
			await LoadHomeScene(mode);
			return;
		}
		string text = SceneManager.GetActiveScene().name;
		if (text != "HomeScene" && text != "LevelScene")
		{
			LogUtil.Error("Cannot load LevelScene outside HomeScene or LevelScene.");
			return;
		}
		ExecuteAllSceneLoad();
		SingletonMonoGlobal<SessionManager>.Instance.DestroyScope(ProcessScope.MainMenu);
		SingletonMonoGlobal<SessionManager>.Instance.CreateScope(ProcessScope.Game);
		string levelId2 = (SingletonMonoScope<LevelManager>.HasInstance ? LevelManager.GetCurLevel() : levelId);
		if (SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.FlushToState(levelId2);
			SingletonMonoScope<ItemManager>.Instance.ClearAlive();
		}
		if (SingletonMonoScene<EnemyPointManager>.HasInstance)
		{
			SingletonMonoScene<EnemyPointManager>.Instance.FlushAll();
		}
		if (SingletonMonoScene<LevelInteractablesManager>.HasInstance)
		{
			SingletonMonoScene<LevelInteractablesManager>.Instance.FlushAll();
		}
		if (SingletonMonoGlobal<StateDataManager>.HasInstance)
		{
			SingletonMonoGlobal<StateDataManager>.Instance.EnterChapter(LevelManager.GetChapterId(levelId), levelId);
		}
		if (SingletonMonoGlobal<StateDataManager>.HasInstance && SingletonMonoScope<LevelManager>.HasInstance && SingletonMonoScope<CompanionManager>.HasInstance)
		{
			SingletonMonoGlobal<StateDataManager>.Instance.SaveCompsState();
		}
		if (SingletonMonoScope<LevelManager>.HasInstance)
		{
			LevelManager.SetCurLevel(levelId, sceneQulity);
		}
		await RunSceneLoad("LevelScene", mode);
	}

	private static async UniTask RunSceneLoad(string sceneName, SceneTransitionMode mode)
	{
		switch (mode)
		{
		case SceneTransitionMode.LoadingScreen:
		{
			AsyncOperation asyncOperation = Singleton<ScenesManager>.Instance.LoadSceneAsync(sceneName);
			Singleton<UIManager>.Instance.ShowExclusivePanel<LoadPanel, AsyncOperation>(asyncOperation);
			await asyncOperation.ToUniTask();
			break;
		}
		case SceneTransitionMode.Fade:
			await SingletonMonoGlobal<SceneFadeManager>.Instance.FadeOutAndWait();
			await Singleton<ScenesManager>.Instance.LoadSceneAsync(sceneName);
			break;
		default:
			await SingletonMonoGlobal<SceneFadeManager>.Instance.FadeOutAndWait();
			await Singleton<ScenesManager>.Instance.LoadSceneAsync(sceneName);
			break;
		}
	}

	private static void ExecuteAllSceneLoad()
	{
		GamepadInputManager.ResetStickChangeState();
		Singleton<UIManager>.Instance.HideAllPanels();
		SingletonMonoGlobal<AudioManager>.Instance.SoftResetBGM();
		SceneLoading.Reset();
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			SingletonMonoScope<InputManager>.Instance.ForceClearInteractionLocks();
		}
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			SingletonMonoScope<PlayerManager>.Instance?.ClearAllTargets();
		}
		if (UI_BossTip.HasInstance)
		{
			UI_BossTip.Instance?.ClearBoss();
		}
		Time.timeScale = 1f;
	}
}
