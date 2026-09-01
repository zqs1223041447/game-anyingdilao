using System;
using System.Collections;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class TipPanel : BasePanel, IPanelParam<TipPanelParam>
{
	[Header("UI")]
	[SerializeField]
	private RectTransform tipRoot;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text tipText;

	[Header("Animation")]
	[SerializeField]
	private float slideDistance = 80f;

	[SerializeField]
	private float fadeInTime = 0.25f;

	[SerializeField]
	private float fadeOutTime = 0.25f;

	[SerializeField]
	private float defaultStayTime = 2f;

	[Header("Audio (可选)")]
	[SerializeField]
	private string audio_event_success;

	[SerializeField]
	private string audio_event_info;

	[SerializeField]
	private string audio_event_fail;

	[Header("Text Color")]
	[SerializeField]
	private Color normalTextColor = Color.white;

	[SerializeField]
	private Color successTextColor = Color.white;

	[SerializeField]
	private Color infoTextColor = Color.white;

	[SerializeField]
	private Color failTextColor = Color.white;

	private Coroutine currentRoutine;

	private Vector2 originPos;

	private TipPanelParam tipParam;

	protected override void Awake()
	{
		base.Awake();
		if (!tipRoot)
		{
			tipRoot = GetComponent<RectTransform>();
		}
		if (!canvasGroup)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		if (!tipText)
		{
			tipText = base.transform.Find("Image/Text").GetComponent<Text>();
		}
		originPos = tipRoot.anchoredPosition;
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		tipRoot.gameObject.SetActive(value: true);
	}

	public override void OnShow()
	{
		base.OnShow();
		if (tipParam != null && !string.IsNullOrEmpty(tipParam.Content))
		{
			switch (tipParam.Type)
			{
			case TipType.Success:
				ShowSuccess(tipParam.Content, tipParam.StayTime, tipParam.UseCustomTextColor, tipParam.TextColor);
				break;
			case TipType.Info:
				ShowInfo(tipParam.Content, tipParam.StayTime, tipParam.UseCustomTextColor, tipParam.TextColor);
				break;
			case TipType.Fail:
				ShowFail(tipParam.Content, tipParam.StayTime, tipParam.UseCustomTextColor, tipParam.TextColor);
				break;
			default:
				Show(tipParam.Content, tipParam.StayTime, tipParam.UseCustomTextColor, tipParam.TextColor);
				break;
			}
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		if (currentRoutine != null)
		{
			StopCoroutine(currentRoutine);
			currentRoutine = null;
		}
		canvasGroup.alpha = 0f;
		tipRoot.anchoredPosition = originPos;
		tipParam = null;
	}

	public void Show(string content, float stayTime = -1f, bool useCustomTextColor = false, Color textColor = default(Color))
	{
		ShowInternal(content, TipType.Normal, stayTime, useCustomTextColor, textColor);
	}

	public void ShowSuccess(string content, float stayTime = -1f, bool useCustomTextColor = false, Color textColor = default(Color))
	{
		ShowInternal(content, TipType.Success, stayTime, useCustomTextColor, textColor);
	}

	public void ShowInfo(string content, float stayTime = -1f, bool useCustomTextColor = false, Color textColor = default(Color))
	{
		ShowInternal(content, TipType.Info, stayTime, useCustomTextColor, textColor);
	}

	public void ShowFail(string content, float stayTime = -1f, bool useCustomTextColor = false, Color textColor = default(Color))
	{
		ShowInternal(content, TipType.Fail, stayTime, useCustomTextColor, textColor);
	}

	private void ShowInternal(string content, TipType type, float stayTime, bool useCustomTextColor, Color textColor)
	{
		PlayAudioByType(type);
		if (stayTime <= 0f)
		{
			stayTime = defaultStayTime;
		}
		if (currentRoutine != null)
		{
			StopCoroutine(currentRoutine);
		}
		tipText.text = content;
		tipText.color = (useCustomTextColor ? textColor : GetDefaultTextColor(type));
		currentRoutine = StartCoroutine(PlayTip(stayTime));
	}

	private void PlayAudioByType(TipType type)
	{
		switch (type)
		{
		case TipType.Success:
			if (!string.IsNullOrEmpty(audio_event_success))
			{
				RuntimeManager.PlayOneShot(audio_event_success, base.transform.position);
			}
			break;
		case TipType.Info:
			if (!string.IsNullOrEmpty(audio_event_info))
			{
				RuntimeManager.PlayOneShot(audio_event_info, base.transform.position);
			}
			break;
		case TipType.Fail:
			if (!string.IsNullOrEmpty(audio_event_fail))
			{
				RuntimeManager.PlayOneShot(audio_event_fail, base.transform.position);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("type", type, null);
		case TipType.Normal:
			break;
		}
	}

	private Color GetDefaultTextColor(TipType type)
	{
		return type switch
		{
			TipType.Success => successTextColor, 
			TipType.Info => infoTextColor, 
			TipType.Fail => failTextColor, 
			_ => normalTextColor, 
		};
	}

	private IEnumerator PlayTip(float stayTime)
	{
		canvasGroup.alpha = 0f;
		tipRoot.anchoredPosition = originPos + Vector2.up * slideDistance;
		yield return FadeAndMove(0f, 1f, tipRoot.anchoredPosition, originPos, fadeInTime);
		yield return new WaitForSecondsRealtime(stayTime);
		yield return FadeAndMove(1f, 0f, originPos, originPos + Vector2.up * slideDistance, fadeOutTime);
		currentRoutine = null;
		Singleton<UIManager>.Instance.HidePanel<TipPanel>();
	}

	private IEnumerator FadeAndMove(float fromAlpha, float toAlpha, Vector2 fromPos, Vector2 toPos, float duration)
	{
		float t = 0f;
		while (t < duration)
		{
			t += Time.unscaledDeltaTime;
			float t2 = EaseOutCubic(Mathf.Clamp01(t / duration));
			canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t2);
			tipRoot.anchoredPosition = Vector2.Lerp(fromPos, toPos, t2);
			yield return null;
		}
		canvasGroup.alpha = toAlpha;
		tipRoot.anchoredPosition = toPos;
	}

	private static float EaseOutCubic(float x)
	{
		return 1f - Mathf.Pow(1f - x, 3f);
	}

	public void SetParam(TipPanelParam param)
	{
		tipParam = param;
	}
}
