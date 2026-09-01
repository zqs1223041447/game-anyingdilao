using System.Collections.Generic;
using Data.SaveData;
using Entity.InteractableObjects.Item;
using FinkFramework.Runtime.Singleton;

namespace Container.Util;

public static class ContainerRestoreUtil
{
	public static void RestoreWeaponRuntimeRefs(WeaponClass weapon)
	{
		if (weapon == null)
		{
			return;
		}
		weapon.Icon = ItemIconUtil.GetWeaponIcon(weapon);
		if (weapon.Aocao == null)
		{
			return;
		}
		for (int i = 0; i < weapon.Aocao.Count; i++)
		{
			WPAocao wPAocao = weapon.Aocao[i];
			if (wPAocao != null)
			{
				if (!wPAocao.HasBaoshi)
				{
					wPAocao.Icon = null;
					wPAocao.UseType = 0;
					wPAocao.BS_Quality = 0;
				}
				else
				{
					wPAocao.Icon = ItemIconUtil.GetBaoshiIcon(wPAocao.Name);
					FillBaoshiSocketExtraData(wPAocao);
				}
			}
		}
	}

	private static void FillBaoshiSocketExtraData(WPAocao aocao)
	{
		if (aocao == null || !SingletonMonoScope<ItemManager>.HasInstance)
		{
			return;
		}
		SingletonMonoScope<ItemManager>.Instance.TryGetBaoshiByItemName(aocao.Name, out var data);
		if (data != null)
		{
			if (aocao.UseType == 0 && data.UseType != 0)
			{
				aocao.UseType = data.UseType;
			}
			if (aocao.BS_Quality == 0 && data.BS_Quality != 0)
			{
				aocao.BS_Quality = data.BS_Quality;
			}
		}
	}

	private static void FillBaoshiExtraData(BaoshiClass baoshi)
	{
		if (baoshi == null || !SingletonMonoScope<ItemManager>.HasInstance)
		{
			return;
		}
		SingletonMonoScope<ItemManager>.Instance.TryGetBaoshiByItemName(baoshi.ItemName, out var data);
		if (data != null)
		{
			if (baoshi.UseType == 0 && data.UseType != 0)
			{
				baoshi.UseType = data.UseType;
			}
			if (baoshi.BS_Quality == 0 && data.BS_Quality != 0)
			{
				baoshi.BS_Quality = data.BS_Quality;
			}
		}
	}

	public static SlotData RestoreOneItemToPage(ContainerItemSaveData item, List<SlotDataPage> pages, List<MainSlotPage> mainPages, IntVector2 inventorySize)
	{
		if (item == null || pages == null || mainPages == null)
		{
			return null;
		}
		if (SaveDataEquipmentSanitizer.ShouldDropContainerItemOnLoad(item))
		{
			return null;
		}
		int page = item.Page;
		int gridX = item.GridX;
		int gridY = item.GridY;
		if (page < 0 || page >= pages.Count)
		{
			return null;
		}
		if (gridX < 0 || gridY < 0 || gridX >= inventorySize.x || gridY >= inventorySize.y)
		{
			return null;
		}
		SlotData slotData = pages[page].DT[gridX, gridY];
		if (slotData == null || slotData.isOC)
		{
			return null;
		}
		slotData.Page = page;
		slotData.GridPos = new IntVector2(gridX, gridY);
		slotData.StartPos = new IntVector2(gridX, gridY);
		slotData.isOC = true;
		slotData.isMain = true;
		slotData.ItemType = item.ItemType;
		switch (item.ItemType)
		{
		case 0:
		{
			if (item.Weapon == null)
			{
				return null;
			}
			WeaponClass weaponClass = new WeaponClass();
			item.Weapon.ApplyToRuntime(weaponClass);
			RestoreWeaponRuntimeRefs(weaponClass);
			slotData.weapon = weaponClass;
			slotData.baoshi = null;
			slotData.useitem = null;
			slotData.ItemSize = weaponClass.Size;
			break;
		}
		case 1:
		{
			if (item.Baoshi == null)
			{
				return null;
			}
			BaoshiClass baoshiClass = new BaoshiClass();
			item.Baoshi.ApplyToRuntime(baoshiClass);
			FillBaoshiExtraData(baoshiClass);
			baoshiClass.Icon = ItemIconUtil.GetBaoshiIcon(baoshiClass);
			slotData.weapon = null;
			slotData.baoshi = baoshiClass;
			slotData.useitem = null;
			slotData.ItemSize = baoshiClass.Size;
			break;
		}
		case 2:
		{
			if (item.UseItem == null)
			{
				return null;
			}
			UseItemClass useItemClass = new UseItemClass();
			item.UseItem.ApplyToRuntime(useItemClass);
			useItemClass.Icon = ItemIconUtil.GetUseItemIcon(useItemClass);
			slotData.weapon = null;
			slotData.baoshi = null;
			slotData.useitem = useItemClass;
			slotData.ItemSize = useItemClass.Size;
			break;
		}
		default:
			return null;
		}
		if (!mainPages[page].MainList.Contains(slotData))
		{
			mainPages[page].MainList.Add(slotData);
		}
		return slotData;
	}
}
