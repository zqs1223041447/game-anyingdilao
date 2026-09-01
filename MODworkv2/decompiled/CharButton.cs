using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image back;

	public Image icon;

	public bool hasWeapon;

	public int charType;

	public ItemScript ItemOBJ;

	public WeaponClass weapon;

	public UnityEvent leftClick;

	public UnityEvent rightClick;

	private InventoryManager IV;

	private Hand hand;

	public void OnPointerClick(PointerEventData eventData)
	{
		switch (eventData.button)
		{
		case PointerEventData.InputButton.Left:
			leftClick.Invoke();
			break;
		case PointerEventData.InputButton.Right:
			rightClick.Invoke();
			break;
		case PointerEventData.InputButton.Middle:
			break;
		}
	}

	private void Awake()
	{
		back = base.transform.Find("back").GetComponent<Image>();
		icon = base.transform.Find("icon").GetComponent<Image>();
		IV = SingletonMonoScope<InventoryManager>.Instance;
		hand = Hand.Instance;
		SetStartData();
	}

	private void Start()
	{
		leftClick.AddListener(ButtonLeftClick);
		rightClick.AddListener(ButtonRightClick);
		icon.color = new Color32(0, 0, 0, 0);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		IV.MouseCharBT = this;
		if (InventoryManager.IsHandHoldingEquippable())
		{
			icon.color = (InventoryManager.CanEquipToSlot(hand.weapon, charType) ? new Color32(0, byte.MaxValue, 0, 15) : new Color32(byte.MaxValue, 0, 0, 15));
		}
		else if (hand.isDragItem && hand.itemType == 1)
		{
			icon.color = (hasWeapon ? new Color32(0, byte.MaxValue, 0, 15) : new Color32(byte.MaxValue, 0, 0, 15));
			if (hasWeapon)
			{
				ShowAocao();
			}
		}
		else if (hand.isDragItem)
		{
			icon.color = new Color32(byte.MaxValue, 0, 0, 15);
		}
		else
		{
			icon.color = new Color32(0, 0, 0, 0);
			if (hasWeapon)
			{
				ShowAocao();
			}
		}
		if (hasWeapon)
		{
			SingletonMonoScope<GameUIManager>.Instance.ShowWPTipB(base.transform.position, weapon);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		IV.MouseCharBT = null;
		SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
		icon.color = new Color32(0, 0, 0, 0);
		HideAocao();
	}

	public void SetStartData()
	{
		back.gameObject.SetActive(value: true);
		hasWeapon = false;
		for (int i = 0; i < 6; i++)
		{
			WPSkill item = new WPSkill();
			weapon.WPSK.Add(item);
			WPAocao wPAocao = new WPAocao();
			wPAocao.HasAocao = false;
			weapon.Aocao.Add(wPAocao);
		}
	}

	private void ButtonLeftClick()
	{
		if ((bool)ItemOBJ)
		{
			if (InventoryManager.IsHandHoldingEquippable())
			{
				if (InventoryManager.CanEquipToSlot(hand.weapon, charType))
				{
					IV.EquipSWAPHand();
				}
			}
			else if (hand.isDragItem && hand.itemType == 1)
			{
				IV.TryApplyHeldBaoshiToEquipment(this);
			}
			else if (!hand.isDragItem)
			{
				IV.DeEquipmentHand();
			}
		}
		else if (InventoryManager.IsHandHoldingEquippable() && InventoryManager.CanEquipToSlot(hand.weapon, charType))
		{
			IV.EquipmentHand();
		}
	}

	public void ShowAocao()
	{
		if ((bool)ItemOBJ)
		{
			if (IV.MouseCharBT != this)
			{
				ItemOBJ.HideSocketDisplay();
			}
			else
			{
				ItemOBJ.RefreshSocketDisplay(weapon, showEmptySockets: true);
			}
		}
	}

	private void HideAocao()
	{
		if ((bool)ItemOBJ)
		{
			ItemOBJ.HideSocketDisplay();
		}
	}

	private void ButtonRightClick()
	{
		if (!Hand.Instance.isDragItem && (bool)ItemOBJ && IV.CheckEmpty(weapon.Size) != null)
		{
			IV.DeEquipmentSlot();
		}
	}
}
