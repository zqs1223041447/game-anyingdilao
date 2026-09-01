using System;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UI.CustomHandler;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inputs;

public class GamepadUINavigationManager : SingletonMonoGlobal<GamepadUINavigationManager>
{
	[Header("导航参数")]
	public const float moveThreshold = 0.5f;

	public const float firstRepeatDelay = 0.35f;

	public const float repeatInterval = 0.12f;

	public static UINavigationDirection holdMoveDirection;

	public static float nextMoveTime;

	private static float nextEnsureFocusTime;

	private const float ensureFocusInterval = 0.1f;

	private static Selectable pendingForcedSelected;

	private static bool hasPendingForcedSelected;

	private Selectable lastSelectableBeforePc;

	public static bool BlockGamepadUIInput { get; set; }

	public void Init()
	{
		LogUtil.Info("手柄导航管理器初始化完成");
		CurrentInputManager.OnCurrentInputDeviceChanged += HandleInputDeviceChanged;
	}

	private void Update()
	{
		if ((bool)EventSystem.current && SingletonMonoGlobal<CurrentInputManager>.HasInstance && !BlockGamepadUIInput)
		{
			if (!SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				GamepadUIActionManager.HandleKeyboardCancel();
				return;
			}
			EnsureFocus();
			HandleMove();
			HandleRightStickAction();
			GamepadUIActionManager.HandleSubmit();
			GamepadUIActionManager.HandleCancel();
		}
	}

	public void HandleInputDeviceChanged(InputDeviceType deviceType)
	{
		if ((bool)EventSystem.current)
		{
			if (deviceType == InputDeviceType.PC)
			{
				HandleSwitchedToPc();
			}
			else
			{
				HandleSwitchedToGamepad();
			}
		}
	}

	private void HandleSwitchedToPc()
	{
		Selectable currentSelectable = GetCurrentSelectable();
		if (IsSelectableValid(currentSelectable))
		{
			lastSelectableBeforePc = currentSelectable;
		}
		else
		{
			lastSelectableBeforePc = null;
		}
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void HandleSwitchedToGamepad()
	{
		if (IsSelectableValid(lastSelectableBeforePc))
		{
			RequestForceFocus(lastSelectableBeforePc);
			return;
		}
		lastSelectableBeforePc = null;
		EventSystem.current.SetSelectedGameObject(null);
	}

	public static void RequestForceFocus(Selectable selectable)
	{
		if (IsSelectableValid(selectable))
		{
			pendingForcedSelected = selectable;
			hasPendingForcedSelected = true;
		}
	}

	private static void EnsureFocus()
	{
		if (Time.unscaledTime < nextEnsureFocusTime)
		{
			return;
		}
		nextEnsureFocusTime = Time.unscaledTime + 0.1f;
		if (!EventSystem.current)
		{
			return;
		}
		if (hasPendingForcedSelected)
		{
			if (IsSelectableValid(pendingForcedSelected))
			{
				EventSystem.current.SetSelectedGameObject(null);
				EventSystem.current.SetSelectedGameObject(pendingForcedSelected.gameObject);
			}
			pendingForcedSelected = null;
			hasPendingForcedSelected = false;
			return;
		}
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if ((bool)currentSelectedGameObject)
		{
			if (IsSelectableValid(currentSelectedGameObject.GetComponent<Selectable>()))
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
		GamepadSelectablePanel topActiveGamepadPanel = GetTopActiveGamepadPanel();
		if (!topActiveGamepadPanel)
		{
			return;
		}
		Selectable returnSelected = topActiveGamepadPanel.GetReturnSelected();
		if (IsSelectableValid(returnSelected))
		{
			EventSystem.current.SetSelectedGameObject(returnSelected.gameObject);
			return;
		}
		returnSelected = topActiveGamepadPanel.GetFirstSelected();
		if (IsSelectableValid(returnSelected))
		{
			EventSystem.current.SetSelectedGameObject(returnSelected.gameObject);
			return;
		}
		returnSelected = FindFirstValidSelectableInPanel(topActiveGamepadPanel);
		if (IsSelectableValid(returnSelected))
		{
			EventSystem.current.SetSelectedGameObject(returnSelected.gameObject);
		}
	}

	public static GamepadSelectablePanel GetTopActiveGamepadPanel()
	{
		GamepadSelectablePanel[] array = UnityEngine.Object.FindObjectsOfType<GamepadSelectablePanel>();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			GamepadSelectablePanel gamepadSelectablePanel = array[num];
			if ((bool)gamepadSelectablePanel && gamepadSelectablePanel.gameObject.activeInHierarchy)
			{
				return gamepadSelectablePanel;
			}
		}
		return null;
	}

	private static Selectable FindFirstValidSelectableInPanel(GamepadSelectablePanel panel)
	{
		if (!panel || !panel.gameObject.activeInHierarchy)
		{
			return null;
		}
		Selectable[] componentsInChildren = panel.GetComponentsInChildren<Selectable>(includeInactive: true);
		foreach (Selectable selectable in componentsInChildren)
		{
			if (IsSelectableValid(selectable))
			{
				return selectable;
			}
		}
		return null;
	}

	private static void HandleMove()
	{
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if (!currentSelectedGameObject)
		{
			holdMoveDirection = UINavigationDirection.None;
			return;
		}
		Slider slider = currentSelectedGameObject.GetComponent<Slider>();
		if (!slider)
		{
			slider = currentSelectedGameObject.GetComponentInParent<Slider>();
		}
		if ((bool)slider && slider.IsInteractable() && slider.gameObject.activeInHierarchy)
		{
			Singleton<GamepadUIActionManager>.Instance.HandleMoveForSlider(slider);
		}
		else
		{
			HandleNormalMove();
		}
	}

	private static void HandleRightStickAction()
	{
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if ((bool)currentSelectedGameObject)
		{
			Slider slider = currentSelectedGameObject.GetComponent<Slider>();
			if (!slider)
			{
				slider = currentSelectedGameObject.GetComponentInParent<Slider>();
			}
			CustomScrollView customScrollView = currentSelectedGameObject.GetComponentInParent<CustomScrollView>();
			ScrollViewContent componentInParent = currentSelectedGameObject.GetComponentInParent<ScrollViewContent>();
			if (!customScrollView && (bool)componentInParent)
			{
				customScrollView = componentInParent.customView;
			}
			float rightStickXRaw = GamepadInputManager.GetRightStickXRaw();
			float rightStickYRaw = GamepadInputManager.GetRightStickYRaw();
			if ((bool)slider && slider.IsInteractable() && slider.gameObject.activeInHierarchy)
			{
				Singleton<GamepadUIActionManager>.Instance.HandleSliderInput(slider, rightStickXRaw, Time.unscaledTime, Singleton<GamepadUIActionManager>.Instance.rightSliderState);
			}
			if ((bool)customScrollView && customScrollView.isActiveAndEnabled)
			{
				GamepadUIActionManager.HandleCustomScrollView(customScrollView, rightStickYRaw);
			}
		}
	}

	private static void HandleNormalMove()
	{
		if (!TryGetMoveDirection(out var direction))
		{
			holdMoveDirection = UINavigationDirection.None;
			return;
		}
		float unscaledTime = Time.unscaledTime;
		if (holdMoveDirection != direction)
		{
			holdMoveDirection = direction;
			nextMoveTime = unscaledTime + 0.35f;
			MoveSelection(direction);
		}
		else if (unscaledTime >= nextMoveTime)
		{
			nextMoveTime = unscaledTime + 0.12f;
			MoveSelection(direction);
		}
	}

	public static void TryAutoMoveAfterSubmit(Selectable originalSelectable)
	{
		if (!EventSystem.current || !originalSelectable)
		{
			return;
		}
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if ((!currentSelectedGameObject || !(currentSelectedGameObject != originalSelectable.gameObject)) && (!originalSelectable.gameObject.activeInHierarchy || !originalSelectable.IsInteractable()))
		{
			Selectable selectable = FindNextAvailableSelectable(originalSelectable);
			if (!selectable)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(selectable.gameObject);
			}
		}
	}

	private static Selectable FindNextAvailableSelectable(Selectable from)
	{
		if (!from)
		{
			return null;
		}
		Selectable validSelectable = GetValidSelectable(from.FindSelectableOnRight());
		if ((bool)validSelectable)
		{
			return validSelectable;
		}
		validSelectable = GetValidSelectable(from.FindSelectableOnDown());
		if ((bool)validSelectable)
		{
			return validSelectable;
		}
		validSelectable = GetValidSelectable(from.FindSelectableOnLeft());
		if ((bool)validSelectable)
		{
			return validSelectable;
		}
		validSelectable = GetValidSelectable(from.FindSelectableOnUp());
		if ((bool)validSelectable)
		{
			return validSelectable;
		}
		return null;
	}

	private static Selectable GetValidSelectable(Selectable selectable)
	{
		if (!IsSelectableValid(selectable))
		{
			return null;
		}
		return selectable;
	}

	private static bool TryGetMoveDirection(out UINavigationDirection direction)
	{
		direction = UINavigationDirection.None;
		float leftStickXRaw = GamepadInputManager.GetLeftStickXRaw();
		float leftStickYRaw = GamepadInputManager.GetLeftStickYRaw();
		float dPadXRaw = GamepadInputManager.GetDPadXRaw();
		float dPadYRaw = GamepadInputManager.GetDPadYRaw();
		Vector2 vector = new Vector2(leftStickXRaw, leftStickYRaw);
		Vector2 vector2 = new Vector2(dPadXRaw, dPadYRaw);
		Vector2 vector3 = ((vector2.sqrMagnitude > vector.sqrMagnitude) ? vector2 : vector);
		if (Mathf.Abs(vector3.x) < 0.5f && Mathf.Abs(vector3.y) < 0.5f)
		{
			return false;
		}
		if (Mathf.Abs(vector3.x) > Mathf.Abs(vector3.y))
		{
			direction = ((vector3.x > 0f) ? UINavigationDirection.Right : UINavigationDirection.Left);
		}
		else
		{
			direction = ((vector3.y > 0f) ? UINavigationDirection.Up : UINavigationDirection.Down);
		}
		return true;
	}

	public static void MoveSelection(UINavigationDirection direction)
	{
		Selectable currentSelectable = GetCurrentSelectable();
		if ((bool)currentSelectable)
		{
			Selectable selectable = FindNextSelectableSkippingInvalid(currentSelectable, direction);
			if ((bool)selectable)
			{
				EventSystem.current.SetSelectedGameObject(selectable.gameObject);
			}
		}
	}

	private static Selectable FindNextSelectableSkippingInvalid(Selectable start, UINavigationDirection direction)
	{
		if (!start)
		{
			return null;
		}
		Selectable nextSelectableInDirection = GetNextSelectableInDirection(start, direction);
		int num = 0;
		while ((bool)nextSelectableInDirection && num < 64)
		{
			if (IsSelectableValid(nextSelectableInDirection))
			{
				return nextSelectableInDirection;
			}
			nextSelectableInDirection = GetNextSelectableInDirection(nextSelectableInDirection, direction);
			num++;
		}
		return null;
	}

	private static Selectable GetNextSelectableInDirection(Selectable selectable, UINavigationDirection direction)
	{
		if (!selectable)
		{
			return null;
		}
		return direction switch
		{
			UINavigationDirection.Up => selectable.FindSelectableOnUp(), 
			UINavigationDirection.Down => selectable.FindSelectableOnDown(), 
			UINavigationDirection.Left => selectable.FindSelectableOnLeft(), 
			UINavigationDirection.Right => selectable.FindSelectableOnRight(), 
			UINavigationDirection.None => null, 
			_ => throw new ArgumentOutOfRangeException("direction", direction, null), 
		};
	}

	private static bool IsSelectableValid(Selectable selectable)
	{
		if (!selectable)
		{
			return false;
		}
		if (!selectable.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (!selectable.IsInteractable())
		{
			return false;
		}
		if (IsActbarUtilityButton(selectable))
		{
			return false;
		}
		return true;
	}

	public static bool IsSelectableValidForGamepad(Selectable selectable)
	{
		return IsSelectableValid(selectable);
	}

	private static bool IsActbarUtilityButton(Selectable selectable)
	{
		if (!selectable.GetComponent<ACT_Auto>() && !selectable.GetComponent<ACT_TP>() && !selectable.GetComponent<ACT_skillBT>() && !selectable.GetComponent<ACT_UseBT>() && !selectable.GetComponent<ACTListSkillBT>())
		{
			return selectable.GetComponent<ACTListUseBT>();
		}
		return true;
	}

	public static Selectable GetCurrentSelectable()
	{
		if (!EventSystem.current)
		{
			return null;
		}
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if (!currentSelectedGameObject)
		{
			return null;
		}
		Selectable component = currentSelectedGameObject.GetComponent<Selectable>();
		if (!IsSelectableValid(component))
		{
			return null;
		}
		return component;
	}

	public static GamepadSelectablePanel GetCurrentGamepadPanel()
	{
		if (!EventSystem.current)
		{
			return GetTopActiveGamepadPanel();
		}
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if (!currentSelectedGameObject)
		{
			return GetTopActiveGamepadPanel();
		}
		if (!currentSelectedGameObject.activeInHierarchy)
		{
			return GetTopActiveGamepadPanel();
		}
		GamepadSelectablePanel componentInParent = currentSelectedGameObject.GetComponentInParent<GamepadSelectablePanel>();
		if ((bool)componentInParent && componentInParent.gameObject.activeInHierarchy)
		{
			return componentInParent;
		}
		return GetTopActiveGamepadPanel();
	}
}
