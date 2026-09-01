using System;
using System.Collections.Generic;
using Data.SaveData.GlobalSave;
using UnityEngine;

namespace Data.SaveData;

[Serializable]
public class SaveData
{
	public string GameVersion;

	public SaveBackupKind BackupKind;

	public long SaveCreatedUtcTicks;

	public long SessionBaselineUtcTicks;

	public string SessionId;

	public string SaveTransactionId;

	public GlobalSaveData EmbeddedGlobalData;

	public bool PlayerDataSavedWithoutEquipment;

	public long PlayTimeSeconds;

	public PlayerSaveData PlayerData;

	public TalentSaveData TalentData;

	public ActbarSaveData ActbarData;

	public InventorySaveData InventoryData;

	public DialogSaveData DialogData;

	public HashSet<int> UnlockedChapterIds;

	public HashSet<string> UnlockedLevelIds;

	public HashSet<string> DefeatedBossLevelIds;

	public string LastPlayLevelId;

	public bool UnlockedMijing;

	public int mijingFloor_easy;

	public int mijingFloor_medium;

	public int mijingFloor_hard;

	public int mijingFloor_master;

	public static SaveData CreateNew()
	{
		return new SaveData
		{
			GameVersion = Application.version,
			BackupKind = SaveBackupKind.EntryBaseline,
			SaveCreatedUtcTicks = DateTime.UtcNow.Ticks,
			SessionBaselineUtcTicks = 0L,
			SessionId = "",
			SaveTransactionId = "",
			EmbeddedGlobalData = null,
			PlayerDataSavedWithoutEquipment = true,
			PlayTimeSeconds = 0L,
			PlayerData = PlayerSaveData.CreateDefault(),
			TalentData = TalentSaveData.CreateDefault(),
			ActbarData = ActbarSaveData.CreateDefault(),
			InventoryData = InventorySaveData.CreateDefault(),
			DialogData = DialogSaveData.CreateDefault(),
			UnlockedChapterIds = new HashSet<int> { 1 },
			UnlockedLevelIds = new HashSet<string> { "01_01" },
			DefeatedBossLevelIds = new HashSet<string>(),
			LastPlayLevelId = "",
			UnlockedMijing = false,
			mijingFloor_medium = 1,
			mijingFloor_easy = 1,
			mijingFloor_hard = 1,
			mijingFloor_master = 1
		};
	}

	public void PostLoadFix()
	{
		GameVersion = GameVersion ?? Application.version;
		if (SaveCreatedUtcTicks <= 0)
		{
			SaveCreatedUtcTicks = DateTime.UtcNow.Ticks;
		}
		if (SessionBaselineUtcTicks < 0)
		{
			SessionBaselineUtcTicks = 0L;
		}
		SessionId = SessionId ?? "";
		SaveTransactionId = SaveTransactionId ?? "";
		if (EmbeddedGlobalData != null)
		{
			EmbeddedGlobalData.PostLoadFix();
		}
		PlayerData = PlayerData ?? PlayerSaveData.CreateDefault();
		TalentData = TalentData ?? TalentSaveData.CreateDefault();
		ActbarData = ActbarData ?? ActbarSaveData.CreateDefault();
		InventoryData = InventoryData ?? InventorySaveData.CreateDefault();
		InventoryData.PostLoadFix();
		DialogData = DialogData ?? DialogSaveData.CreateDefault();
		UnlockedChapterIds = UnlockedChapterIds ?? new HashSet<int> { 1 };
		UnlockedLevelIds = UnlockedLevelIds ?? new HashSet<string> { "01_01" };
		DefeatedBossLevelIds = DefeatedBossLevelIds ?? new HashSet<string>();
		if (mijingFloor_easy <= 0)
		{
			mijingFloor_easy = 1;
		}
		if (mijingFloor_medium <= 0)
		{
			mijingFloor_medium = 1;
		}
		if (mijingFloor_hard <= 0)
		{
			mijingFloor_hard = 1;
		}
		if (mijingFloor_master <= 0)
		{
			mijingFloor_master = 1;
		}
		SaveDataEquipmentSanitizer.PostLoadFix(this);
	}
}
