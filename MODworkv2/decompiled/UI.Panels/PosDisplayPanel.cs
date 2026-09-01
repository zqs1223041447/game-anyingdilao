using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class PosDisplayPanel : MonoBehaviour
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
	private float fadeInTime = 0.25f;

	[SerializeField]
	private float fadeOutTime = 0.25f;

	[SerializeField]
	private float defaultStayTime = 2f;

	[Header("Audio (可选)")]
	[SerializeField]
	private string audio_event;

	private Coroutine currentRoutine;

	private void Awake()
	{
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
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		tipRoot.gameObject.SetActive(value: true);
	}

	public void Show(string content, float stayTime = -1f)
	{
		if (stayTime <= 0f)
		{
			stayTime = defaultStayTime;
		}
		if (currentRoutine != null)
		{
			StopCoroutine(currentRoutine);
		}
		tipText.text = content;
		currentRoutine = StartCoroutine(PlayRoutine(stayTime));
	}

	public void ClearImmediate()
	{
		if (currentRoutine != null)
		{
			StopCoroutine(currentRoutine);
			currentRoutine = null;
		}
		canvasGroup.alpha = 0f;
		tipText.text = string.Empty;
	}

	private IEnumerator PlayRoutine(float stayTime)
	{
		canvasGroup.alpha = 0f;
		if (audio_event == "" || string.IsNullOrEmpty(audio_event))
		{
			RuntimeManager.PlayOneShot(audio_event, base.transform.position);
		}
		yield return Fade(0f, 1f, fadeInTime);
		yield return new WaitForSeconds(stayTime);
		yield return Fade(1f, 0f, fadeOutTime);
		currentRoutine = null;
	}

	private IEnumerator Fade(float from, float to, float duration)
	{
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			float t2 = EaseInOutCubic(Mathf.Clamp01(t / duration));
			canvasGroup.alpha = Mathf.Lerp(from, to, t2);
			yield return null;
		}
		canvasGroup.alpha = to;
	}

	private static float EaseInOutCubic(float x)
	{
		if (!(x < 0.5f))
		{
			return 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
		}
		return 4f * x * x * x;
	}
}
