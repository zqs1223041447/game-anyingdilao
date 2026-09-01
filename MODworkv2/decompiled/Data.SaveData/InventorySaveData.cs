using System;
using System.Collections.Generic;

namespace Data.SaveData;

[Serializable]
public class InventorySaveData
{
	public const int MaxSafePageCount = 1000;

	public long Money;

	public int PageCount;

	public List<WeaponSaveData> Equipments = new List<WeaponSaveData>();

	public List<ContainerItemSaveData> InventoryItems = new List<ContainerItemSaveData>();

	public static InventorySaveData CreateDefault()
	{
		return new InventorySaveData
		{
			Money = 0L,
			PageCount = 1,
			Equipments = new List<WeaponSaveData>(),
			InventoryItems = new List<ContainerItemSaveData>()
		};
	}

	public void PostLoadFix()
	{
		if (Money < 0)
		{
			Money = 0L;
		}
		if (PageCount <= 0)
		{
			PageCount = 1;
		}
		else if (PageCount > 1000)
		{
			PageCount = 1000;
		}
		Equipments = Equipments ?? new List<WeaponSaveData>();
		InventoryItems = InventoryItems ?? new List<ContainerItemSaveData>();
	}
}
