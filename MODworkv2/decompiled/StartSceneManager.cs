using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Inputs;
using Scenes;

public class StartSceneManager : SingletonMonoScene<StartSceneManager>
{
	private async UniTask InitAsync()
	{
		SingletonMonoGlobal<SceneFadeManager>.Instance.Init();
		GamepadInputManager.ResetStickChangeState();
		Singleton<UIManager>.Instance.ShowExclusivePanel<MainPanel>();
		SingletonMonoGlobal<AudioManager>.Instance.PlayIntroThenLoop(SingletonMonoGlobal<AudioManager>.Instance.musicData.StartUI, SingletonMonoGlobal<AudioManager>.Instance.musicData.StartLoop);
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.MainMenu);
		await SingletonMonoGlobal<SceneFadeManager>.Instance.EnsureFadeIn();
	}

	protected override void Awake()
	{
		base.Awake();
		InitAsync().Forget();
	}

	private void Start()
	{
		if (SingletonMonoGlobal<AudioManager>.HasInstance)
		{
			SingletonMonoGlobal<AudioManager>.Instance.StopAtmos();
		}
	}

	private void OnDestroy()
	{
		if (SingletonMonoGlobal<AudioManager>.HasInstance)
		{
			SingletonMonoGlobal<AudioManager>.Instance.ExitIntroLoopMode();
		}
	}
}
