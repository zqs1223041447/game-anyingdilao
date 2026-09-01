using Core.Settings;
using FinkFramework.Runtime.Singleton;
using UI.CustomHandler;
using UI.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UIItems;

public class BaoshiItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public Text baoshiCount;

	public Image childIcon;

	public Image baoshiIcon;

	public Image isSelectedIcon;

	public Button button;

	[HideInInspector]
	public BaoshiClass baoshiData;

	[HideInInspector]
	public BaoshiClass childBaoshiData;

	private BaoshiManager owner;

	private Image buttonHitImage;

	private int ownCount;

	private void Awake()
	{
		if (!button)
		{
			button = GetComponent<Button>();
		}
		ResolveIconReferences();
		DisableButtonHoverVisuals();
		DisableChildRaycastTargets();
		KeepKuangVisible();
	}

	public void Init(BaoshiManager manager, BaoshiClass baoshi, BaoshiClass childBaoshi, int needCount)
	{
		owner = manager;
		baoshiData = baoshi;
		childBaoshiData = childBaoshi;
		if ((bool)baoshiIcon)
		{
			baoshiIcon.sprite = baoshiData?.Icon;
		}
		if ((bool)childIcon)
		{
			childIcon.sprite = childBaoshiData?.Icon;
		}
		KeepButtonBodyTransparent();
		KeepKuangVisible();
		RefreshCount();
	}

	private void ResolveIconReferences()
	{
		buttonHitImage = GetComponent<Image>();
		Transform transform = base.transform.Find("Baoshi");
		Image image = (transform ? transform.GetComponent<Image>() : null);
		if ((bool)image && (!baoshiIcon || baoshiIcon.gameObject == base.gameObject))
		{
			baoshiIcon = image;
		}
	}

	private void DisableButtonHoverVisuals()
	{
		UIButtonState component = GetComponent<UIButtonState>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		if ((bool)button)
		{
			button.transition = Selectable.Transition.None;
			button.targetGraphic = null;
		}
		KeepButtonBodyTransparent();
	}

	private void KeepButtonBodyTransparent()
	{
		if (!buttonHitImage)
		{
			buttonHitImage = GetComponent<Image>();
		}
		if ((bool)buttonHitImage)
		{
			Color color = buttonHitImage.color;
			color.a = 0f;
			buttonHitImage.color = color;
			buttonHitImage.raycastTarget = true;
		}
	}

	public void RefreshCount()
	{
		if ((bool)baoshiCount)
		{
			if (!SingletonMonoScope<InventoryManager>.HasInstance || childBaoshiData == null)
			{
				baoshiCount.text = "<color=#FF0000>0 / 0 x</color>";
				RefreshTextWidth(baoshiCount, 2f);
				return;
			}
			int baoshiTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetBaoshiTotalCountInInv(childBaoshiData.ItemName);
			int needCount = SettingsLoader.Instance.baoshiSettings.needCount;
			baoshiCount.text = ((baoshiTotalCountInInv >= needCount) ? $"<color=#00FF00>{baoshiTotalCountInInv} / {needCount} x</color>" : $"<color=#FF0000>{baoshiTotalCountInInv} / {needCount} x</color>");
			RefreshTextWidth(baoshiCount, 2f);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		KeepButtonBodyTransparent();
		KeepKuangVisible();
		owner?.SelectItem(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		KeepButtonBodyTransparent();
		KeepKuangVisible();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData == null)
		{
			return;
		}
		switch (eventData.button)
		{
		case PointerEventData.InputButton.Left:
			if ((!owner || !owner.ConsumeGamepadCreatePointerClick(eventData.button)) && (bool)owner)
			{
				bool flag2 = owner.ConsumeGamepadCreateBulkClick(eventData.button) || owner.IsDirectCreateToInventoryInput();
				owner.TryCreateFromItem(this, (!flag2) ? 1 : 5, directToInventory: false);
			}
			break;
		case PointerEventData.InputButton.Right:
			if ((!owner || !owner.ConsumeGamepadCreatePointerClick(eventData.button)) && (bool)owner)
			{
				bool flag = owner.ConsumeGamepadCreateBulkClick(eventData.button) || owner.IsDirectCreateToInventoryInput();
				owner.TryCreateFromItem(this, (!flag) ? 1 : 5, directToInventory: true);
			}
			break;
		}
	}

	public void SetSelected(bool selected)
	{
		KeepKuangVisible();
	}

	private void KeepKuangVisible()
	{
		if ((bool)isSelectedIcon)
		{
			isSelectedIcon.gameObject.SetActive(value: true);
		}
	}

	public static void RefreshTextWidth(Text text, float extraPadding = 0f)
	{
		if ((bool)text)
		{
			RectTransform component = text.GetComponent<RectTransform>();
			if ((bool)component)
			{
				TextGenerationSettings generationSettings = text.GetGenerationSettings(Vector2.zero);
				float preferredWidth = text.cachedTextGeneratorForLayout.GetPreferredWidth(text.text, generationSettings);
				preferredWidth /= text.pixelsPerUnit;
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth + extraPadding);
			}
		}
	}

	private void DisableChildRaycastTargets()
	{
		Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if ((bool)componentsInChildren[i] && componentsInChildren[i].gameObject != base.gameObject)
			{
				componentsInChildren[i].raycastTarget = false;
			}
		}
	}
}
