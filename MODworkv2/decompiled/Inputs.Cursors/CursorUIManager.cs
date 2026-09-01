using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inputs.Cursors;

public class CursorUIManager : SingletonMonoGlobal<CursorUIManager>
{
	private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

	private GameObject currentHoverObject;

	private GameObject leftPressObject;

	private GameObject rightPressObject;

	private PointerEventData pointerEventData;

	private static int _leftClickConsumedFrame = -1;

	private static int _rightClickConsumedFrame = -1;

	private static int _shiftConsumedFrame = -1;

	private static int _ctrlConsumedFrame = -1;

	private static bool _blockGameplayLeftUntilRelease;

	private static bool _blockGameplayRightUntilRelease;

	private static bool _blockGameplayShiftUntilRelease;

	private static bool _blockGameplayCtrlUntilRelease;

	public void Init()
	{
		LogUtil.Info("虚拟光标UI操作管理器初始化完成");
	}

	private void Update()
	{
		if ((bool)EventSystem.current)
		{
			if (!SingletonMonoGlobal<VirtualCursorManager>.HasInstance || !SingletonMonoGlobal<VirtualCursorManager>.Instance.ShouldUseVirtualCursor)
			{
				ClearHover();
			}
			else if (SingletonMonoScope<CursorInputManager>.HasInstance)
			{
				UpdateHover();
				UpdateLeftClick();
				UpdateRightClick();
			}
		}
	}

	public bool IsPointerOverUI()
	{
		return currentHoverObject;
	}

	private void UpdateHover()
	{
		GameObject gameObject = RaycastTopUI();
		if (!(gameObject == currentHoverObject))
		{
			if ((bool)currentHoverObject)
			{
				PointerEventData eventData = CreatePointerData(PointerEventData.InputButton.Left);
				ExecuteEvents.Execute(currentHoverObject, eventData, ExecuteEvents.pointerExitHandler);
			}
			currentHoverObject = gameObject;
			if ((bool)currentHoverObject)
			{
				PointerEventData eventData2 = CreatePointerData(PointerEventData.InputButton.Left);
				ExecuteEvents.Execute(currentHoverObject, eventData2, ExecuteEvents.pointerEnterHandler);
			}
		}
	}

	private void UpdateLeftClick()
	{
		if (SingletonMonoScope<CursorInputManager>.Instance.LeftButtonDown)
		{
			leftPressObject = RaycastTopUI();
			if ((bool)leftPressObject)
			{
				_leftClickConsumedFrame = Time.frameCount;
				_blockGameplayLeftUntilRelease = true;
				PointerEventData pointerEventData = CreatePointerData(PointerEventData.InputButton.Left);
				pointerEventData.pointerPressRaycast = FindTopRaycast(pointerEventData.position);
				ExecuteEvents.ExecuteHierarchy(leftPressObject, pointerEventData, ExecuteEvents.pointerDownHandler);
			}
		}
		if (!SingletonMonoScope<CursorInputManager>.Instance.LeftButtonUp)
		{
			return;
		}
		GameObject gameObject = RaycastTopUI();
		if ((bool)leftPressObject)
		{
			PointerEventData pointerEventData2 = CreatePointerData(PointerEventData.InputButton.Left);
			pointerEventData2.pointerPressRaycast = FindTopRaycast(pointerEventData2.position);
			ExecuteEvents.ExecuteHierarchy(leftPressObject, pointerEventData2, ExecuteEvents.pointerUpHandler);
			if (gameObject == leftPressObject)
			{
				ExecuteEvents.ExecuteHierarchy(leftPressObject, pointerEventData2, ExecuteEvents.pointerClickHandler);
			}
		}
		leftPressObject = null;
	}

	private void UpdateRightClick()
	{
		if (SingletonMonoScope<CursorInputManager>.Instance.RightButtonDown)
		{
			rightPressObject = RaycastTopUI();
			if ((bool)rightPressObject)
			{
				_rightClickConsumedFrame = Time.frameCount;
				_blockGameplayRightUntilRelease = true;
				PointerEventData pointerEventData = CreatePointerData(PointerEventData.InputButton.Right);
				pointerEventData.pointerPressRaycast = FindTopRaycast(pointerEventData.position);
				ExecuteEvents.ExecuteHierarchy(rightPressObject, pointerEventData, ExecuteEvents.pointerDownHandler);
			}
		}
		if (!SingletonMonoScope<CursorInputManager>.Instance.RightButtonUp)
		{
			return;
		}
		GameObject gameObject = RaycastTopUI();
		if ((bool)rightPressObject)
		{
			PointerEventData pointerEventData2 = CreatePointerData(PointerEventData.InputButton.Right);
			pointerEventData2.pointerPressRaycast = FindTopRaycast(pointerEventData2.position);
			ExecuteEvents.ExecuteHierarchy(rightPressObject, pointerEventData2, ExecuteEvents.pointerUpHandler);
			if (gameObject == rightPressObject)
			{
				ExecuteEvents.ExecuteHierarchy(rightPressObject, pointerEventData2, ExecuteEvents.pointerClickHandler);
			}
		}
		rightPressObject = null;
	}

	private GameObject RaycastTopUI()
	{
		raycastResults.Clear();
		PointerEventData eventData = CreatePointerData(PointerEventData.InputButton.Left);
		EventSystem.current.RaycastAll(eventData, raycastResults);
		for (int i = 0; i < raycastResults.Count; i++)
		{
			if (raycastResults[i].gameObject.activeInHierarchy)
			{
				return raycastResults[i].gameObject;
			}
		}
		return null;
	}

	private RaycastResult FindTopRaycast(Vector2 position)
	{
		raycastResults.Clear();
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = position;
		EventSystem.current.RaycastAll(pointerEventData, raycastResults);
		for (int i = 0; i < raycastResults.Count; i++)
		{
			if (raycastResults[i].gameObject.activeInHierarchy)
			{
				return raycastResults[i];
			}
		}
		return default(RaycastResult);
	}

	private PointerEventData CreatePointerData(PointerEventData.InputButton button)
	{
		if (pointerEventData == null)
		{
			pointerEventData = new PointerEventData(EventSystem.current);
		}
		pointerEventData.Reset();
		pointerEventData.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		pointerEventData.button = button;
		return pointerEventData;
	}

	private void ClearHover()
	{
		if ((bool)currentHoverObject && (bool)EventSystem.current)
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = (SingletonMonoScope<CursorInputManager>.HasInstance ? ((Vector2)SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition) : Vector2.zero);
			ExecuteEvents.Execute(currentHoverObject, pointerEventData, ExecuteEvents.pointerExitHandler);
			currentHoverObject = null;
			leftPressObject = null;
			rightPressObject = null;
		}
	}

	public static void ConsumeShiftModifier()
	{
		_shiftConsumedFrame = Time.frameCount;
		_blockGameplayShiftUntilRelease = true;
	}

	public static void ConsumeCtrlModifier()
	{
		_ctrlConsumedFrame = Time.frameCount;
		_blockGameplayCtrlUntilRelease = true;
	}

	public static bool IsGameplayShiftBlocked()
	{
		if (_shiftConsumedFrame == Time.frameCount)
		{
			return true;
		}
		if (_blockGameplayShiftUntilRelease)
		{
			if ((SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent()) ? Input.GetKey(KeyCode.JoystickButton8) : Input.GetKey(KeyCode.LeftShift))
			{
				return true;
			}
			_blockGameplayShiftUntilRelease = false;
		}
		return false;
	}

	public static bool IsGameplayCtrlBlocked()
	{
		if (_ctrlConsumedFrame == Time.frameCount)
		{
			return true;
		}
		if (_blockGameplayCtrlUntilRelease)
		{
			if ((SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent()) ? Input.GetKey(KeyCode.JoystickButton9) : Input.GetKey(KeyCode.LeftControl))
			{
				return true;
			}
			_blockGameplayCtrlUntilRelease = false;
		}
		return false;
	}

	public static bool IsGameplayLeftClickBlocked()
	{
		if (_leftClickConsumedFrame == Time.frameCount)
		{
			return true;
		}
		if (_blockGameplayLeftUntilRelease)
		{
			if (SingletonMonoScope<CursorInputManager>.HasInstance && SingletonMonoScope<CursorInputManager>.Instance.LeftButton)
			{
				return true;
			}
			_blockGameplayLeftUntilRelease = false;
		}
		return false;
	}

	public static bool IsGameplayRightClickBlocked()
	{
		if (_rightClickConsumedFrame == Time.frameCount)
		{
			return true;
		}
		if (_blockGameplayRightUntilRelease)
		{
			if (SingletonMonoScope<CursorInputManager>.HasInstance && SingletonMonoScope<CursorInputManager>.Instance.RightButton)
			{
				return true;
			}
			_blockGameplayRightUntilRelease = false;
		}
		return false;
	}
}
