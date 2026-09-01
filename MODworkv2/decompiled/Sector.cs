using Container.Inventory;
using Container.Util;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sector : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public int QuadNum;

	public static IntVector2 posOffset;

	public static Sector sec;

	public SlotScript parent;

	private void Start()
	{
		parent = GetComponentInParent<SlotScript>();
	}

	private static void RefreshWeaponSocketDisplay(SlotData dt)
	{
		if (dt != null && dt.weapon != null && (bool)dt.ItemOBJ)
		{
			dt.ItemOBJ.RefreshBS(dt);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		sec = this;
		Hand.Instance.Mpos = (int)parent.type;
		switch (Hand.Instance.Mpos)
		{
		case 0:
			SingletonMonoScope<InventoryManager>.Instance.MouseSlot = parent;
			SetPosOffset();
			SingletonMonoScope<InventoryManager>.Instance.RefreshColor(enter: true);
			if (!Hand.Instance.ItemOBJ)
			{
				if (SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT != null && SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT.isOC)
				{
					SlotData mouseSlotDT5 = SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT;
					ContainerSlotUtil.ColorChange(SlotColor.TouMing, mouseSlotDT5.ItemSize, mouseSlotDT5.StartPos, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
					switch (mouseSlotDT5.ItemType)
					{
					case 0:
						SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(mouseSlotDT5.weapon, SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
						RefreshWeaponSocketDisplay(mouseSlotDT5);
						break;
					case 1:
						SingletonMonoScope<GameUIManager>.Instance.ShowBSTip(mouseSlotDT5.baoshi, mouseSlotDT5, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
						break;
					case 2:
						SingletonMonoScope<GameUIManager>.Instance.ShowUseTip(mouseSlotDT5.useitem, mouseSlotDT5, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
						break;
					}
				}
				else
				{
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
				}
			}
			else
			{
				if (!Hand.Instance.ItemOBJ || Hand.Instance.itemType != 1)
				{
					break;
				}
				if (SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT != null && SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT.isOC)
				{
					SlotData mouseSlotDT6 = SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT;
					ContainerSlotUtil.ColorChange(SlotColor.TouMing, mouseSlotDT6.ItemSize, mouseSlotDT6.StartPos, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
					if (mouseSlotDT6.ItemType == 0)
					{
						SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(mouseSlotDT6.weapon, mouseSlotDT6, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
						RefreshWeaponSocketDisplay(mouseSlotDT6);
					}
				}
				else
				{
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
				}
			}
			break;
		case 1:
			SingletonMonoScope<WarehouseManager>.Instance.MouseSlot = parent;
			SetPosOffset();
			SingletonMonoScope<WarehouseManager>.Instance.RefreshColor(enter: true);
			if (!Hand.Instance.ItemOBJ)
			{
				if (SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT != null && SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT.isOC)
				{
					SlotData mouseSlotDT3 = SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT;
					ContainerSlotUtil.ColorChange(SlotColor.TouMing, mouseSlotDT3.ItemSize, mouseSlotDT3.StartPos, SingletonMonoScope<WarehouseManager>.Instance.slotGrid);
					switch (mouseSlotDT3.ItemType)
					{
					case 0:
						SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(mouseSlotDT3.weapon, SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT, SingletonMonoScope<WarehouseManager>.Instance.slotGrid);
						RefreshWeaponSocketDisplay(mouseSlotDT3);
						break;
					case 1:
						SingletonMonoScope<GameUIManager>.Instance.ShowBSTip(mouseSlotDT3.baoshi, mouseSlotDT3, SingletonMonoScope<WarehouseManager>.Instance.slotGrid);
						break;
					case 2:
						SingletonMonoScope<GameUIManager>.Instance.ShowUseTip(mouseSlotDT3.useitem, mouseSlotDT3, SingletonMonoScope<WarehouseManager>.Instance.slotGrid);
						break;
					}
				}
				else
				{
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
				}
			}
			else
			{
				if (!(Hand.Instance.ItemOBJ != null) || Hand.Instance.itemType != 1)
				{
					break;
				}
				if (SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT != null && SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT.isOC)
				{
					SlotData mouseSlotDT4 = SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT;
					ContainerSlotUtil.ColorChange(SlotColor.TouMing, mouseSlotDT4.ItemSize, mouseSlotDT4.StartPos, SingletonMonoScope<WarehouseManager>.Instance.slotGrid);
					if (mouseSlotDT4.ItemType == 0)
					{
						SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(mouseSlotDT4.weapon, mouseSlotDT4, SingletonMonoScope<WarehouseManager>.Instance.slotGrid);
						RefreshWeaponSocketDisplay(mouseSlotDT4);
					}
				}
				else
				{
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
				}
			}
			break;
		case 2:
			SingletonMonoScope<ShopManager>.Instance.MouseSlot = parent;
			SetPosOffset();
			SingletonMonoScope<ShopManager>.Instance.RefreshColor(enter: true);
			if (!Hand.Instance.ItemOBJ)
			{
				if (SingletonMonoScope<ShopManager>.Instance.MouseSlotDT != null && SingletonMonoScope<ShopManager>.Instance.MouseSlotDT.isOC)
				{
					SlotData mouseSlotDT = SingletonMonoScope<ShopManager>.Instance.MouseSlotDT;
					ContainerSlotUtil.ColorChange(SlotColor.TouMing, mouseSlotDT.ItemSize, mouseSlotDT.StartPos, SingletonMonoScope<ShopManager>.Instance.slotGrid);
					switch (mouseSlotDT.ItemType)
					{
					case 0:
						SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(mouseSlotDT.weapon, SingletonMonoScope<ShopManager>.Instance.MouseSlotDT, SingletonMonoScope<ShopManager>.Instance.slotGrid);
						RefreshWeaponSocketDisplay(mouseSlotDT);
						break;
					case 1:
						SingletonMonoScope<GameUIManager>.Instance.ShowBSTip(mouseSlotDT.baoshi, mouseSlotDT, SingletonMonoScope<ShopManager>.Instance.slotGrid);
						break;
					case 2:
						SingletonMonoScope<GameUIManager>.Instance.ShowUseTip(mouseSlotDT.useitem, mouseSlotDT, SingletonMonoScope<ShopManager>.Instance.slotGrid);
						break;
					}
				}
				else
				{
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
				}
			}
			else
			{
				if (!Hand.Instance.ItemOBJ || Hand.Instance.itemType != 1)
				{
					break;
				}
				if (SingletonMonoScope<ShopManager>.Instance.MouseSlotDT != null && SingletonMonoScope<ShopManager>.Instance.MouseSlotDT.isOC)
				{
					SlotData mouseSlotDT2 = SingletonMonoScope<ShopManager>.Instance.MouseSlotDT;
					ContainerSlotUtil.ColorChange(SlotColor.TouMing, mouseSlotDT2.ItemSize, mouseSlotDT2.StartPos, SingletonMonoScope<ShopManager>.Instance.slotGrid);
					if (mouseSlotDT2.ItemType == 0)
					{
						SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(mouseSlotDT2.weapon, mouseSlotDT2, SingletonMonoScope<ShopManager>.Instance.slotGrid);
						RefreshWeaponSocketDisplay(mouseSlotDT2);
					}
				}
				else
				{
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
				}
			}
			break;
		case 3:
			break;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		sec = null;
		SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
		SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
		posOffset = IntVector2.Zero;
		switch (Hand.Instance.Mpos)
		{
		case 0:
		{
			SingletonMonoScope<InventoryManager>.Instance.RefreshColor(enter: false);
			SlotData mouseSlotDT3 = SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT;
			if (mouseSlotDT3 != null && mouseSlotDT3.ItemOBJ != null)
			{
				mouseSlotDT3.ItemOBJ.HideSocketDisplay();
			}
			SingletonMonoScope<InventoryManager>.Instance.MouseSlot = null;
			break;
		}
		case 1:
		{
			SingletonMonoScope<WarehouseManager>.Instance.RefreshColor(enter: false);
			SlotData mouseSlotDT2 = SingletonMonoScope<WarehouseManager>.Instance.MouseSlotDT;
			if (mouseSlotDT2 != null && mouseSlotDT2.ItemOBJ != null)
			{
				mouseSlotDT2.ItemOBJ.HideSocketDisplay();
			}
			SingletonMonoScope<WarehouseManager>.Instance.MouseSlot = null;
			break;
		}
		case 2:
		{
			SingletonMonoScope<ShopManager>.Instance.RefreshColor(enter: false);
			SlotData mouseSlotDT = SingletonMonoScope<ShopManager>.Instance.MouseSlotDT;
			if (mouseSlotDT != null && mouseSlotDT.ItemOBJ != null)
			{
				mouseSlotDT.ItemOBJ.HideSocketDisplay();
			}
			SingletonMonoScope<ShopManager>.Instance.MouseSlot = null;
			break;
		}
		}
		Hand.Instance.Mpos = 4;
	}

	public void SetPosOffset()
	{
		if (Hand.Instance.itemSize.x != 0 && Hand.Instance.itemSize.x % 2 == 0)
		{
			switch (QuadNum)
			{
			case 1:
				posOffset.x = 0;
				break;
			case 2:
				posOffset.x = -1;
				break;
			case 3:
				posOffset.x = 0;
				break;
			case 4:
				posOffset.x = -1;
				break;
			}
		}
		if (Hand.Instance.itemSize.y != 0 && Hand.Instance.itemSize.y % 2 == 0)
		{
			switch (QuadNum)
			{
			case 1:
				posOffset.y = 0;
				break;
			case 2:
				posOffset.y = 0;
				break;
			case 3:
				posOffset.y = -1;
				break;
			case 4:
				posOffset.y = -1;
				break;
			}
		}
	}
}
