using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Cursors;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIItems;

public class CursorIconItem : MonoBehaviour
{
	[SerializeField]
	private RectTransform cursorRect;

	[SerializeField]
	private Image cursorImage;

	[HideInInspector]
	public bool IsShow;

	private static float visualScale => SettingsLoader.Instance.cursorSettings.virualCursorScale;

	private void Reset()
	{
		cursorRect = base.transform as RectTransform;
		cursorImage = GetComponent<Image>();
		if ((bool)cursorImage)
		{
			cursorImage.raycastTarget = false;
		}
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

	public void ShowIcon()
	{
		IsShow = true;
		cursorImage.enabled = true;
		if (Cursor.visible)
		{
			Cursor.visible = false;
		}
		SingletonMonoScope<GameUIManager>.Instance.virtualCursor.SetForceHide(value: true);
	}

	public void HideIcon()
	{
		IsShow = false;
		cursorImage.enabled = false;
		if (!Cursor.visible && SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			Cursor.visible = true;
		}
		SingletonMonoScope<GameUIManager>.Instance.virtualCursor.SetForceHide(value: false);
	}

	private void LateUpdate()
	{
		if ((bool)cursorRect && (bool)cursorImage && IsShow)
		{
			if (Cursor.visible)
			{
				Cursor.visible = false;
			}
			SingletonMonoScope<GameUIManager>.Instance.virtualCursor.SetForceHide(value: true);
			if (!SingletonMonoScope<CursorInputManager>.HasInstance || !SingletonMonoGlobal<CursorManager>.HasInstance)
			{
				cursorImage.enabled = false;
				return;
			}
			ApplyPosition();
			ApplySize();
			cursorImage.enabled = true;
		}
	}

	private void ApplyPosition()
	{
		RectTransform rectTransform = cursorRect.parent as RectTransform;
		if ((bool)rectTransform)
		{
			if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, SingletonMonoScope<CursorInputManager>.Instance.VirtualScreenPosition, null, out var localPoint);
				cursorRect.anchoredPosition = localPoint;
			}
			else
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition, null, out var localPoint2);
				cursorRect.anchoredPosition = localPoint2;
			}
		}
		else if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			cursorRect.position = SingletonMonoScope<CursorInputManager>.Instance.VirtualScreenPosition;
		}
		else
		{
			cursorRect.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		}
	}

	private void ApplySize()
	{
		Vector2 currentCursorSize = SingletonMonoGlobal<CursorManager>.Instance.CurrentCursorSize;
		currentCursorSize *= visualScale;
		if (cursorRect.sizeDelta != currentCursorSize)
		{
			cursorRect.sizeDelta = currentCursorSize;
		}
	}
}
