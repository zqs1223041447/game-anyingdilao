using System;

namespace Data.SaveData;

[Serializable]
public class SaveSlotData
{
	public int SlotId;

	public string GameVersion;

	public long PlayTimeSeconds;

	public int PlayerType;

	public string playerName;

	public int level;

	public int dfLevel;
}
