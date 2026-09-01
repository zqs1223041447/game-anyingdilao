using System;
using System.Collections.Generic;

namespace Data.SaveData.GlobalSave;

[Serializable]
public class GlobalChestSaveData
{
	public const int MaxSafePageCount = 10000;

	public int PageCount;

	public List<ContainerItemSaveData> ChestItems = new List<ContainerItemSaveData>();

	public static GlobalChestSaveData CreateDefault()
	{
		return new GlobalChestSaveData
		{
			PageCount = 1,
			ChestItems = new List<ContainerItemSaveData>()
		};
	}

	public void PostLoadFix()
	{
		if (PageCount <= 0)
		{
			PageCount = 1;
		}
		else if (PageCount > 10000)
		{
			PageCount = 10000;
		}
		ChestItems = ChestItems ?? new List<ContainerItemSaveData>();
		SaveDataEquipmentSanitizer.SanitizeGlobalChestItems(ChestItems);
	}
}
