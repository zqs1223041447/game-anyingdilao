using System;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Cursors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.CustomHandler;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class UIButtonState : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
	[Serializable]
	public class BoolEvent : UnityEvent<bool>
	{
	}

	[Header("状态图片")]
	[SerializeField]
	private Sprite highlightedSprite;

	[Header("状态颜色")]
	[SerializeField]
	private Color normalColor = Color.white;

	[SerializeField]
	private Color pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);

	[SerializeField]
	private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	[Header("是否在不可交互时仍保留高亮图")]
	[SerializeField]
	private bool keepHighlightedSpriteWhenDisabled;

	[Header("Hover事件")]
	public UnityEvent onHoverEnter;

	public UnityEvent onHoverExit;

	public BoolEvent onHoverChanged;

	[Header("Selected事件")]
	public UnityEvent onSelectedEnter;

	public UnityEvent onSelectedExit;

	public BoolEvent onSelectedChanged;

	private Button button;

	private Image targetImage;

	private bool isPointerInside;

	private bool isPressed;

	private bool isSelected;

	private Sprite baseSprite;

	private InputDeviceType lastDeviceType;

	private bool hasLastDeviceType;

	private void Awake()
	{
		button = GetComponent<Button>();
		if (!targetImage && (bool)button.targetGraphic)
		{
			targetImage = button.targetGraphic.GetComponent<Image>();
		}
		if ((bool)targetImage)
		{
			baseSprite = targetImage.sprite;
		}
		button.transition = Selectable.Transition.None;
	}

	private void OnEnable()
	{
		isPointerInside = false;
		isPressed = false;
		isSelected = false;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			lastDeviceType = SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType;
			hasLastDeviceType = true;
		}
		else
		{
			hasLastDeviceType = false;
		}
		RefreshVisual();
	}

	private void OnDisable()
	{
		bool num = isPointerInside;
		bool flag = isSelected;
		isPointerInside = false;
		isPressed = false;
		isSelected = false;
		if (num)
		{
			InvokeHoverExit();
		}
		if (flag)
		{
			InvokeSelectedExit();
		}
		RefreshVisual();
	}

	private void Update()
	{
		HandleInputDeviceSwitch();
		RefreshVisual();
	}

	private void HandleInputDeviceSwitch()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			InputDeviceType currentDeviceType = SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType;
			if (!hasLastDeviceType)
			{
				lastDeviceType = currentDeviceType;
				hasLastDeviceType = true;
			}
			else if (currentDeviceType != lastDeviceType)
			{
				OnInputDeviceChanged(lastDeviceType, currentDeviceType);
				lastDeviceType = currentDeviceType;
			}
		}
	}

	private void OnInputDeviceChanged(InputDeviceType oldDeviceType, InputDeviceType newDeviceType)
	{
		isPressed = false;
		if (newDeviceType == InputDeviceType.PC)
		{
			ExitSelectedState();
		}
		if (CurrentInputManager.IsGamepad(newDeviceType))
		{
			ExitHoverState();
		}
		RefreshVisual();
	}

	private void ExitHoverState()
	{
		if (isPointerInside)
		{
			isPointerInside = false;
			InvokeHoverExit();
		}
	}

	private void ExitSelectedState()
	{
		if (isSelected)
		{
			isSelected = false;
			InvokeSelectedExit();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!IsPureGamepadNavigationMode() && !isPointerInside)
		{
			isPointerInside = true;
			InvokeHoverEnter();
			RefreshVisual();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!IsPureGamepadNavigationMode() && isPointerInside)
		{
			isPointerInside = false;
			InvokeHoverExit();
			RefreshVisual();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if ((bool)button && button.IsInteractable())
		{
			isPressed = true;
			RefreshVisual();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
		RefreshVisual();
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (IsPureGamepadNavigationMode() && !isSelected)
		{
			isSelected = true;
			isPressed = false;
			InvokeSelectedEnter();
			RefreshVisual();
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (IsPureGamepadNavigationMode() && isSelected)
		{
			isSelected = false;
			isPressed = false;
			InvokeSelectedExit();
			RefreshVisual();
		}
	}

	private static bool IsUsingVirtualCursorMode()
	{
		return CursorInputManager.IsUsingVirtualMouse;
	}

	private static bool IsPureGamepadNavigationMode()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return !IsUsingVirtualCursorMode();
		}
		return false;
	}

	private void RefreshVisual()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !button || !targetImage)
		{
			return;
		}
		bool num = button.IsInteractable();
		bool num2 = (IsUsingVirtualCursorMode() ? isPointerInside : ((SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType != InputDeviceType.Gamepad) ? isPointerInside : isSelected));
		Sprite sprite = baseSprite;
		if (num2 && (bool)highlightedSprite)
		{
			sprite = highlightedSprite;
		}
		Color color;
		if (num)
		{
			color = ((!isPressed) ? normalColor : pressedColor);
		}
		else
		{
			color = disabledColor;
			if (!keepHighlightedSpriteWhenDisabled)
			{
				sprite = baseSprite;
			}
		}
		if (targetImage.sprite != sprite)
		{
			targetImage.sprite = sprite;
		}
		if (targetImage.color != color)
		{
			targetImage.color = color;
		}
	}

	private void InvokeHoverEnter()
	{
		onHoverEnter?.Invoke();
		onHoverChanged?.Invoke(arg0: true);
	}

	private void InvokeHoverExit()
	{
		onHoverExit?.Invoke();
		onHoverChanged?.Invoke(arg0: false);
	}

	private void InvokeSelectedEnter()
	{
		onSelectedEnter?.Invoke();
		onSelectedChanged?.Invoke(arg0: true);
	}

	private void InvokeSelectedExit()
	{
		onSelectedExit?.Invoke();
		onSelectedChanged?.Invoke(arg0: false);
	}

	public void SetBaseSprite(Sprite sprite)
	{
		baseSprite = sprite;
		RefreshVisual();
	}

	public void ForceRefresh()
	{
		RefreshVisual();
	}

	public void SetPressed(bool pressed)
	{
		isPressed = pressed;
		RefreshVisual();
	}

	public void ForceHoverEnter()
	{
		if (!isPointerInside)
		{
			isPointerInside = true;
			InvokeHoverEnter();
			RefreshVisual();
		}
	}

	public void ForceHoverExit()
	{
		if (isPointerInside)
		{
			isPointerInside = false;
			InvokeHoverExit();
			RefreshVisual();
		}
	}
}
