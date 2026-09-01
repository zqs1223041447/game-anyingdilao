using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.CustomHandler;

[DisallowMultipleComponent]
[RequireComponent(typeof(Toggle))]
public class UIToggleState : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
	[Header("主图状态图片")]
	[SerializeField]
	private Sprite highlightedSprite;

	[Header("勾选图状态图片")]
	[SerializeField]
	private Sprite checkmarkHighlightedSprite;

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

	[Header("勾选图是否跟随状态颜色变化")]
	[SerializeField]
	private bool tintCheckmarkWithState = true;

	private Image targetImage;

	private Image checkmarkImage;

	private Toggle toggle;

	private bool isPointerInside;

	private bool isPressed;

	private bool isSelected;

	private Sprite normalSprite;

	private Sprite normalCheckmarkSprite;

	private void Awake()
	{
		toggle = GetComponent<Toggle>();
		targetImage = GetComponent<Image>();
		if (!checkmarkImage && (bool)toggle && (bool)toggle.graphic)
		{
			checkmarkImage = toggle.graphic as Image;
		}
		if ((bool)targetImage)
		{
			normalSprite = targetImage.sprite;
		}
		if ((bool)checkmarkImage)
		{
			normalCheckmarkSprite = checkmarkImage.sprite;
		}
	}

	private void OnEnable()
	{
		isPointerInside = false;
		isPressed = false;
		if ((bool)toggle)
		{
			toggle.onValueChanged.AddListener(OnToggleValueChanged);
		}
		RefreshVisual();
	}

	private void OnDisable()
	{
		if ((bool)toggle)
		{
			toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
		}
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
		if ((bool)toggle && toggle.IsInteractable())
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

	private void OnToggleValueChanged(bool isOn)
	{
		RefreshVisual();
	}

	private void RefreshVisual()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && (bool)toggle && (bool)targetImage)
		{
			bool flag = toggle.IsInteractable();
			bool useHighlighted = ((!SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent()) ? isPointerInside : isSelected);
			Color finalColor = ((!flag) ? disabledColor : ((!isPressed) ? normalColor : pressedColor));
			RefreshTargetImage(flag, useHighlighted, finalColor);
			RefreshCheckmarkImage(flag, useHighlighted, finalColor);
		}
	}

	private void RefreshTargetImage(bool interactable, bool useHighlighted, Color finalColor)
	{
		if ((bool)targetImage)
		{
			Sprite sprite = normalSprite;
			if (useHighlighted && (bool)highlightedSprite)
			{
				sprite = highlightedSprite;
			}
			if (!interactable && !keepHighlightedSpriteWhenDisabled && (bool)normalSprite)
			{
				sprite = normalSprite;
			}
			if (targetImage.sprite != sprite)
			{
				targetImage.sprite = sprite;
			}
			if (targetImage.color != finalColor)
			{
				targetImage.color = finalColor;
			}
		}
	}

	private void RefreshCheckmarkImage(bool interactable, bool useHighlighted, Color finalColor)
	{
		if (!checkmarkImage)
		{
			return;
		}
		checkmarkImage.gameObject.SetActive(toggle.isOn);
		if (toggle.isOn)
		{
			Sprite sprite = normalCheckmarkSprite;
			if (useHighlighted && (bool)checkmarkHighlightedSprite)
			{
				sprite = checkmarkHighlightedSprite;
			}
			if (!interactable && !keepHighlightedSpriteWhenDisabled && (bool)normalCheckmarkSprite)
			{
				sprite = normalCheckmarkSprite;
			}
			if (checkmarkImage.sprite != sprite)
			{
				checkmarkImage.sprite = sprite;
			}
			if (tintCheckmarkWithState && checkmarkImage.color != finalColor)
			{
				checkmarkImage.color = finalColor;
			}
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
