using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIItems;

public class VirtualCursorItem : MonoBehaviour
{
	[SerializeField]
	private RectTransform cursorRect;

	[SerializeField]
	private Image cursorImage;

	public bool forceHide;

	private bool lastShow;

	private static float visualScale => SettingsLoader.Instance.cursorSettings.virualCursorScale;

	private void Reset()
	{
		cursorRect = base.transform as RectTransform;
		cursorImage = GetComponent<Image>();
		if ((bool)cursorImage)
		{
			cursorImage.raycastTarget = false;
		}
		forceHide = false;
	}

	public void SetForceHide(bool value)
	{
		forceHide = value;
	}

	private void Awake()
	{
		if (!cursorRect)
		{
			cursorRect = base.transform as RectTransform;
		}
		if (!cursorImage)
		{
			cursorImage = GetComponent<Image>();
		}
		if ((bool)cursorImage)
		{
			cursorImage.raycastTarget = false;
			cursorImage.enabled = false;
		}
	}

	private void LateUpdate()
	{
		if (!cursorRect || !cursorImage)
		{
			return;
		}
		if (!SingletonMonoScope<CursorInputManager>.HasInstance || !SingletonMonoGlobal<CursorManager>.HasInstance || forceHide || SingletonMonoGlobal<CursorManager>.Instance.IsForceHidden)
		{
			cursorImage.enabled = false;
			lastShow = false;
			return;
		}
		if (!CursorInputManager.IsUsingVirtualMouse && !forceHide)
		{
			cursorImage.enabled = false;
			lastShow = false;
			return;
		}
		if (!lastShow)
		{
			SingletonMonoGlobal<CursorManager>.Instance.RefreshCursorStateImmediate(CursorManager.GetCurrentPointerScreenPosition());
		}
		ApplyPosition();
		ApplySpriteAndSize();
		cursorImage.enabled = true;
		lastShow = true;
	}

	private void ApplyPosition()
	{
		RectTransform rectTransform = cursorRect.parent as RectTransform;
		if ((bool)rectTransform)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, SingletonMonoScope<CursorInputManager>.Instance.VirtualScreenPosition, null, out var localPoint);
			cursorRect.anchoredPosition = localPoint;
		}
		else
		{
			cursorRect.position = SingletonMonoScope<CursorInputManager>.Instance.VirtualScreenPosition;
		}
	}

	private void ApplySpriteAndSize()
	{
		Sprite currentCursorSprite = SingletonMonoGlobal<CursorManager>.Instance.CurrentCursorSprite;
		if (cursorImage.sprite != currentCursorSprite)
		{
			cursorImage.sprite = currentCursorSprite;
		}
		Vector2 vector = (currentCursorSprite ? currentCursorSprite.rect.size : SingletonMonoGlobal<CursorManager>.Instance.CurrentCursorSize);
		vector *= visualScale * SingletonMonoGlobal<CursorManager>.Instance.CurrentVisualScale;
		if (cursorRect.sizeDelta != vector)
		{
			cursorRect.sizeDelta = vector;
		}
	}
}
