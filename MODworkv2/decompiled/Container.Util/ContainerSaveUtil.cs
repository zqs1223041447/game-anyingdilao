using System.Collections.Generic;
using Data.SaveData;

namespace Container.Util;

public class ContainerSaveUtil
{
	public static void SaveContainerItems(List<ContainerItemData> items, List<ContainerItemSaveData> targetList)
	{
		if (targetList == null)
		{
			return;
		}
		targetList.Clear();
		if (items == null || items.Count == 0)
		{
			return;
		}
		foreach (ContainerItemData item in items)
		{
			if (item == null || !item.IsValid)
			{
				continue;
			}
			ContainerItemSaveData containerItemSaveData = new ContainerItemSaveData
			{
				Page = item.Page,
				GridX = item.MainSlot.x,
				GridY = item.MainSlot.y,
				ItemType = item.ItemType
			};
			switch (item.ItemType)
			{
			case 0:
				if (item.weapon != null)
				{
					containerItemSaveData.Weapon = WeaponSaveData.FromRuntime(item.weapon);
				}
				break;
			case 1:
				if (item.baoshi != null)
				{
					containerItemSaveData.Baoshi = BaoshiSaveData.FromRuntime(item.baoshi);
				}
				break;
			case 2:
				if (item.useitem != null)
				{
					containerItemSaveData.UseItem = UseItemSaveData.FromRuntime(item.useitem);
				}
				break;
			}
			targetList.Add(containerItemSaveData);
		}
	}

	public static void SaveContainerItems(List<MainSlotPage> mainPages, List<ContainerItemSaveData> targetList)
	{
		if (targetList == null)
		{
			return;
		}
		targetList.Clear();
		if (mainPages == null || mainPages.Count == 0)
		{
			return;
		}
		for (int i = 0; i < mainPages.Count; i++)
		{
			MainSlotPage mainSlotPage = mainPages[i];
			if (mainSlotPage?.MainList == null || mainSlotPage.MainList.Count == 0)
			{
				continue;
			}
			foreach (SlotData main in mainSlotPage.MainList)
			{
				if (main == null || !main.isMain || !main.isOC)
				{
					continue;
				}
				ContainerItemSaveData containerItemSaveData = new ContainerItemSaveData
				{
					Page = i,
					GridX = main.GridPos.x,
					GridY = main.GridPos.y,
					ItemType = main.ItemType
				};
				switch (main.ItemType)
				{
				case 0:
					if (main.weapon != null)
					{
						containerItemSaveData.Weapon = WeaponSaveData.FromRuntime(main.weapon);
					}
					break;
				case 1:
					if (main.baoshi != null)
					{
						containerItemSaveData.Baoshi = BaoshiSaveData.FromRuntime(main.baoshi);
					}
					break;
				case 2:
					if (main.useitem != null)
					{
						containerItemSaveData.UseItem = UseItemSaveData.FromRuntime(main.useitem);
					}
					break;
				}
				targetList.Add(containerItemSaveData);
			}
		}
	}
}
