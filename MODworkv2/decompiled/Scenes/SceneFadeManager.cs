using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Inputs.Cursors;
using UnityEngine;

namespace Scenes;

public class SceneFadeManager : SingletonMonoGlobal<SceneFadeManager>
{
	[HideInInspector]
	public CanvasGroup fadeCanvasGroup;

	[Header("Config")]
	public float fadeInDuration = 0.6f;

	[Header("Config")]
	public float fadeOutDuration = 0.3f;

	private FadeState fadeState;

	private bool isInited;

	public FadeState CurrentState => fadeState;

	protected override void Awake()
	{
		base.Awake();
		Init();
	}

	public void Init()
	{
		if (!isInited)
		{
			BuildFadeUI();
			isInited = true;
		}
	}

	public async UniTask EnsureFadeIn()
	{
		if (!fadeCanvasGroup)
		{
			CursorManager.SetGlobalForceHidden(hidden: false);
			return;
		}
		if (fadeState == FadeState.Transparent)
		{
			CursorManager.SetGlobalForceHidden(hidden: false);
			return;
		}
		if (fadeState == FadeState.Fading)
		{
			while (fadeState == FadeState.Fading)
			{
				await UniTask.Yield(PlayerLoopTiming.Update);
			}
			if (fadeState == FadeState.Transparent)
			{
				CursorManager.SetGlobalForceHidden(hidden: false);
			}
			return;
		}
		fadeState = FadeState.Fading;
		float t = 0f;
		while (t < fadeInDuration)
		{
			t += Time.deltaTime;
			fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeInDuration);
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
		fadeCanvasGroup.alpha = 0f;
		fadeCanvasGroup.blocksRaycasts = false;
		fadeState = FadeState.Transparent;
		CursorManager.SetGlobalForceHidden(hidden: false);
	}

	public async UniTask FadeInAndWait()
	{
		if (!fadeCanvasGroup)
		{
			CursorManager.SetGlobalForceHidden(hidden: false);
			return;
		}
		fadeState = FadeState.Fading;
		float t = 0f;
		while (t < fadeInDuration)
		{
			t += Time.deltaTime;
			fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeInDuration);
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
		fadeCanvasGroup.alpha = 0f;
		fadeCanvasGroup.blocksRaycasts = false;
		fadeState = FadeState.Transparent;
		CursorManager.SetGlobalForceHidden(hidden: false);
	}

	public async UniTask FadeOutAndWait()
	{
		CursorManager.SetGlobalForceHidden(hidden: true);
		if (!fadeCanvasGroup || fadeState == FadeState.Black)
		{
			return;
		}
		fadeState = FadeState.Fading;
		float startAlpha = fadeCanvasGroup.alpha;
		float duration = fadeOutDuration * Mathf.Clamp01(1f - startAlpha);
		if (duration <= 0f)
		{
			fadeCanvasGroup.alpha = 1f;
			fadeCanvasGroup.blocksRaycasts = true;
			fadeState = FadeState.Black;
			return;
		}
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, t / duration);
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
		fadeCanvasGroup.alpha = 1f;
		fadeCanvasGroup.blocksRaycasts = true;
		fadeState = FadeState.Black;
	}

	private void BuildFadeUI()
	{
		Canvas canvas;
		if ((bool)Singleton<UIManager>.Instance.mainCanvas)
		{
			canvas = Singleton<UIManager>.Instance.mainCanvas;
		}
		else
		{
			GameObject obj = Object.Instantiate(Singleton<ResManager>.Instance.Load<GameObject>("UI/Base/MainCanvas"), base.transform);
			obj.name = "FadeCanvas";
			canvas = obj.GetComponent<Canvas>();
			canvas.sortingOrder = 9999;
		}
		Transform transform = canvas.transform.Find("System");
		GameObject original = Singleton<ResManager>.Instance.Load<GameObject>("UI/Panels/FadePanel");
		if ((bool)transform)
		{
			fadeCanvasGroup = Object.Instantiate(original, transform.transform).GetComponent<CanvasGroup>();
		}
		else
		{
			fadeCanvasGroup = Object.Instantiate(original, canvas.transform).GetComponent<CanvasGroup>();
		}
		fadeCanvasGroup.alpha = 0f;
		fadeCanvasGroup.blocksRaycasts = false;
		fadeState = ((fadeCanvasGroup.alpha >= 0.99f) ? FadeState.Black : FadeState.Transparent);
	}
}
