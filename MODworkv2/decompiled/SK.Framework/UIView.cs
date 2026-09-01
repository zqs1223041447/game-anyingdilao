using UnityEngine;

namespace SK.Framework;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class UIView : MonoBehaviour, IUIView
{
	private CanvasGroup canvasGroup;

	private RectTransform rectTransform;

	[HideInInspector]
	[SerializeField]
	private ViewAnimationEvent onVisible;

	[HideInInspector]
	[SerializeField]
	private ViewAnimationEvent onInvisible;

	protected IActionChain animationChain;

	public CanvasGroup CanvasGroup
	{
		get
		{
			if (canvasGroup == null)
			{
				canvasGroup = GetComponent<CanvasGroup>();
			}
			return canvasGroup;
		}
	}

	public RectTransform RectTransform
	{
		get
		{
			if (rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform>();
			}
			return rectTransform;
		}
	}

	public string Name { get; set; }

	public void Show(IViewData data = null, bool instant = false)
	{
		base.gameObject.SetActive(value: true);
		base.transform.SetAsLastSibling();
		OnShow(data);
		onVisible.onBegan?.Invoke();
		onVisible.beginSound.Play();
		CanvasGroup.interactable = false;
		if (animationChain != null)
		{
			animationChain.Stop();
		}
		animationChain = onVisible.animation.Play(this, instant, delegate
		{
			onVisible.onEnd?.Invoke();
			CanvasGroup.interactable = true;
			animationChain = null;
		});
	}

	public void Hide(bool instant = false)
	{
		OnHide();
		onInvisible.onBegan?.Invoke();
		onInvisible.beginSound.Play();
		CanvasGroup.interactable = false;
		if (animationChain != null)
		{
			animationChain.Stop();
		}
		animationChain = onInvisible.animation.Play(this, instant, delegate
		{
			onVisible.onEnd?.Invoke();
			animationChain = null;
			base.gameObject.SetActive(value: false);
		});
	}

	public void Init(IViewData data = null, bool instant = false)
	{
		OnInit(data);
		onVisible.onBegan?.Invoke();
		onVisible.beginSound.Play();
		CanvasGroup.interactable = false;
		if (animationChain != null)
		{
			animationChain.Stop();
		}
		animationChain = onVisible.animation.Play(this, instant, delegate
		{
			onVisible.onEnd?.Invoke();
			CanvasGroup.interactable = true;
			animationChain = null;
		});
	}

	public void Unload(bool instant = false)
	{
		UI.Instance.Remove(Name);
		OnUnload();
		onInvisible.onBegan?.Invoke();
		onInvisible.beginSound.Play();
		CanvasGroup.interactable = false;
		if (animationChain != null)
		{
			animationChain.Stop();
		}
		animationChain = onInvisible.animation.Play(this, instant, delegate
		{
			onVisible.onEnd?.Invoke();
			Object.Destroy(base.gameObject);
		});
	}

	protected virtual void OnInit(IViewData data)
	{
	}

	protected virtual void OnShow(IViewData data)
	{
	}

	protected virtual void OnHide()
	{
	}

	protected virtual void OnUnload()
	{
	}

	public static T Load<T>(string viewName, string viewResourcePath, ViewLevel level = ViewLevel.COMMON, IViewData data = null, bool instant = false) where T : UIView
	{
		if (UI.Instance.LoadView(viewName, viewResourcePath, level, out var view, data, instant))
		{
			return view as T;
		}
		Debug.LogError("加载UI视图 [" + viewName + "] 失败.");
		return null;
	}

	public static T Load<T>(string viewName, ViewLevel level = ViewLevel.COMMON, IViewData data = null, bool instant = false) where T : UIView
	{
		if (UI.Instance.LoadView(viewName, viewName, level, out var view, data, instant))
		{
			return view as T;
		}
		Debug.LogError("加载UI视图 [" + viewName + "] 失败.");
		return null;
	}

	public static T Load<T>(ViewLevel level, IViewData data = null, bool instant = false) where T : UIView
	{
		if (UI.Instance.LoadView(typeof(T).Name, typeof(T).Name, level, out var view, data, instant))
		{
			return view as T;
		}
		Debug.LogError("加载UI视图 [" + typeof(T).Name + "] 失败.");
		return null;
	}

	public static T Load<T>(IViewData data = null, bool instant = false) where T : UIView
	{
		if (UI.Instance.LoadView(typeof(T).Name, typeof(T).Name, ViewLevel.COMMON, out var view, data, instant))
		{
			return view as T;
		}
		Debug.LogError("加载UI视图 [" + typeof(T).Name + "] 失败.");
		return null;
	}

	public static T Show<T>(IViewData data = null, bool instant = false) where T : UIView
	{
		return UI.Instance.ShowView(typeof(T).Name, data, instant) as T;
	}

	public static T Show<T>(string viewName, IViewData data = null, bool instant = false) where T : UIView
	{
		return UI.Instance.ShowView(viewName, data, instant) as T;
	}

	public static T Hide<T>(bool instant = false) where T : UIView
	{
		return UI.Instance.HideView(typeof(T).Name, instant) as T;
	}

	public static T Hide<T>(string viewName, bool instant = false) where T : UIView
	{
		return UI.Instance.HideView(viewName, instant) as T;
	}

	public static T Get<T>() where T : UIView
	{
		return UI.Instance.GetView(typeof(T).Name) as T;
	}

	public static T Get<T>(string viewName) where T : UIView
	{
		return UI.Instance.GetView(viewName) as T;
	}

	public static bool Unload<T>(bool instant = false) where T : UIView
	{
		return UI.Instance.UnloadView(typeof(T).Name, instant);
	}

	public static bool Unload<T>(string viewName, bool instant = false) where T : UIView
	{
		return UI.Instance.UnloadView(viewName, instant);
	}

	public static void UnloadAll()
	{
		UI.Instance.UnloadAll();
	}
}
