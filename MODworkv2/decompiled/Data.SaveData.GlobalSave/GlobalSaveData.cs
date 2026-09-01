using System;

namespace Data.SaveData.GlobalSave;

[Serializable]
public class GlobalSaveData
{
	public int LastWriterSlotId;

	public string SaveTransactionId;

	public long SaveCreatedUtcTicks;

	public GlobalChestSaveData GlobalChestData;

	public static GlobalSaveData CreateNew()
	{
		return new GlobalSaveData
		{
			LastWriterSlotId = -1,
			SaveTransactionId = "",
			SaveCreatedUtcTicks = DateTime.UtcNow.Ticks,
			GlobalChestData = GlobalChestSaveData.CreateDefault()
		};
	}

	public void PostLoadFix()
	{
		if (LastWriterSlotId < -1)
		{
			LastWriterSlotId = -1;
		}
		SaveTransactionId = SaveTransactionId ?? "";
		if (SaveCreatedUtcTicks < 0)
		{
			SaveCreatedUtcTicks = 0L;
		}
		GlobalChestData = GlobalChestData ?? GlobalChestSaveData.CreateDefault();
		GlobalChestData.PostLoadFix();
	}
}
