using System.Collections;
using UI.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIItems;

public class ItemTipItem : MonoBehaviour
{
	[Header("引用")]
	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text itemNameText;

	[SerializeField]
	private Text countText;

	[SerializeField]
	private Image iconImage;

	[Header("动画参数")]
	[SerializeField]
	private float enterDuration = 0.2f;

	[SerializeField]
	private float moveDuration = 0.15f;

	[SerializeField]
	private float stayDuration = 3f;

	[SerializeField]
	private float exitDuration = 0.3f;

	[SerializeField]
	private float enterOffsetX = 120f;

	[SerializeField]
	private float exitOffsetY = 30f;

	private ItemTipManager manager;

	private Coroutine moveCoroutine;

	private Coroutine lifeCoroutine;

	private Coroutine enterCoroutine;

	private Coroutine exitCoroutine;

	public bool IsExiting { get; private set; }

	private void Awake()
	{
		if (!rectTransform)
		{
			rectTransform = base.transform as RectTransform;
		}
		if (!canvasGroup)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
	}

	private void OnEnable()
	{
		ResetState();
	}

	private void ResetState()
	{
		IsExiting = false;
		if (moveCoroutine != null)
		{
			StopCoroutine(moveCoroutine);
			moveCoroutine = null;
		}
		if (lifeCoroutine != null)
		{
			StopCoroutine(lifeCoroutine);
			lifeCoroutine = null;
		}
		if (enterCoroutine != null)
		{
			StopCoroutine(enterCoroutine);
			enterCoroutine = null;
		}
		if (exitCoroutine != null)
		{
			StopCoroutine(exitCoroutine);
			exitCoroutine = null;
		}
		if ((bool)rectTransform)
		{
			rectTransform.anchoredPosition = Vector2.zero;
		}
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = 0f;
		}
		if ((bool)itemNameText)
		{
			itemNameText.text = string.Empty;
		}
		if ((bool)countText)
		{
			countText.text = string.Empty;
		}
		if ((bool)iconImage)
		{
			iconImage.sprite = null;
			iconImage.gameObject.SetActive(value: false);
		}
	}

	public void Init(ItemTipManager owner, string itemName, int count, Sprite icon)
	{
		manager = owner;
		if ((bool)itemNameText)
		{
			itemNameText.text = itemName;
		}
		if ((bool)countText)
		{
			countText.text = $"x {count}";
		}
		if ((bool)iconImage)
		{
			if ((bool)icon)
			{
				iconImage.sprite = icon;
				iconImage.gameObject.SetActive(value: true);
			}
			else
			{
				iconImage.gameObject.SetActive(value: false);
			}
		}
	}

	public void PlayEnter(Vector2 targetPos)
	{
		Vector2 anchoredPosition = targetPos + Vector2.right * enterOffsetX;
		rectTransform.anchoredPosition = anchoredPosition;
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = 0f;
		}
		if (enterCoroutine != null)
		{
			StopCoroutine(enterCoroutine);
		}
		enterCoroutine = StartCoroutine(CoEnter(targetPos));
		if (lifeCoroutine != null)
		{
			StopCoroutine(lifeCoroutine);
		}
		lifeCoroutine = StartCoroutine(CoLife());
	}

	public void SetTargetPosition(Vector2 targetPos)
	{
		if (!IsExiting)
		{
			if (moveCoroutine != null)
			{
				StopCoroutine(moveCoroutine);
			}
			moveCoroutine = StartCoroutine(CoMove(targetPos));
		}
	}

	public void ForceExit()
	{
		if (!IsExiting)
		{
			if (lifeCoroutine != null)
			{
				StopCoroutine(lifeCoroutine);
				lifeCoroutine = null;
			}
			if (exitCoroutine != null)
			{
				StopCoroutine(exitCoroutine);
			}
			exitCoroutine = StartCoroutine(CoExit());
		}
	}

	private IEnumerator CoLife()
	{
		yield return new WaitForSecondsRealtime(stayDuration);
		yield return CoExit();
	}

	private IEnumerator CoEnter(Vector2 targetPos)
	{
		Vector2 startPos = rectTransform.anchoredPosition;
		float time = 0f;
		while (time < enterDuration)
		{
			time += Time.unscaledDeltaTime;
			float t = Mathf.Clamp01(time / enterDuration);
			rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
			if ((bool)canvasGroup)
			{
				canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
			}
			yield return null;
		}
		rectTransform.anchoredPosition = targetPos;
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = 1f;
		}
		enterCoroutine = null;
	}

	private IEnumerator CoMove(Vector2 targetPos)
	{
		Vector2 startPos = rectTransform.anchoredPosition;
		float time = 0f;
		while (time < moveDuration)
		{
			time += Time.unscaledDeltaTime;
			float t = Mathf.Clamp01(time / moveDuration);
			rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
			yield return null;
		}
		rectTransform.anchoredPosition = targetPos;
		moveCoroutine = null;
	}

	private IEnumerator CoExit()
	{
		IsExiting = true;
		Vector2 startPos = rectTransform.anchoredPosition;
		Vector2 endPos = startPos + Vector2.up * exitOffsetY;
		float startAlpha = (canvasGroup ? canvasGroup.alpha : 1f);
		float time = 0f;
		while (time < exitDuration)
		{
			time += Time.unscaledDeltaTime;
			float t = Mathf.Clamp01(time / exitDuration);
			rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
			if ((bool)canvasGroup)
			{
				canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
			}
			yield return null;
		}
		exitCoroutine = null;
		manager?.NotifyTipExitComplete(this);
	}
}
