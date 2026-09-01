using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.CustomHandler;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class UISliderState : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
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

	private Slider slider;

	private Image targetImage;

	private bool isPointerInside;

	private bool isPressed;

	private bool isSelected;

	private Sprite normalSprite;

	private void Awake()
	{
		slider = GetComponent<Slider>();
		targetImage = slider.handleRect.GetComponent<Image>();
		if ((bool)targetImage)
		{
			normalSprite = targetImage.sprite;
		}
	}

	private void OnEnable()
	{
		isPointerInside = false;
		isPressed = false;
		RefreshVisual();
	}

	private void OnDisable()
	{
		isPointerInside = false;
		isPressed = false;
		isSelected = false;
		RefreshVisual();
	}

	private void Update()
	{
		RefreshVisual();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			isPointerInside = true;
			RefreshVisual();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			isPointerInside = false;
			RefreshVisual();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!(slider == null) && slider.IsInteractable())
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
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			isSelected = true;
			RefreshVisual();
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			isSelected = false;
			isPressed = false;
			RefreshVisual();
		}
	}

	private void RefreshVisual()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !slider || !targetImage)
		{
			return;
		}
		bool num = slider.IsInteractable();
		bool num2 = ((SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType != InputDeviceType.Gamepad) ? isPointerInside : isSelected);
		Sprite sprite = normalSprite;
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
			if (!keepHighlightedSpriteWhenDisabled && (bool)normalSprite)
			{
				sprite = normalSprite;
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

	public void ForceRefresh()
	{
		RefreshVisual();
	}

	public void SetPressed(bool pressed)
	{
		isPressed = pressed;
		RefreshVisual();
	}
}
