using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Data.SaveData;

public static class SaveDataEquipmentSanitizer
{
	private static readonly Dictionary<string, FieldInfo> PlayerFields = BuildFieldMap(typeof(PlayerSaveData));

	private static readonly Dictionary<string, FieldInfo> DotFields = BuildFieldMap(typeof(PlayerDotData));

	private static readonly Dictionary<int, string> MainFloatFields = new Dictionary<int, string>
	{
		{ 1, "Health_Bei" },
		{ 2, "Mana_Bei" },
		{ 3, "Health_R_Base" },
		{ 4, "Mana_R_Base" },
		{ 5, "Attack_R_health_Base" },
		{ 6, "Attack_R_mana_Base" },
		{ 10, "Damage_Bei" },
		{ 11, "ATSpeed_Bei" },
		{ 12, "MVSpeed_Bei" },
		{ 13, "BJrate" },
		{ 14, "BJDamage" },
		{ 15, "CoolDown" },
		{ 16, "ManaXH" },
		{ 17, "GeDang" },
		{ 18, "Damage_Anti" },
		{ 19, "DOTcut" },
		{ 20, "AntiSlow" },
		{ 21, "AllChuan" },
		{ 22, "AllAnti" },
		{ 30, "BJD_Anti" },
		{ 50, "ItemDrop_Rate" },
		{ 51, "FlySpeed" },
		{ 52, "ORB_Damage" },
		{ 53, "JYrate" },
		{ 54, "ThroughRate" },
		{ 60, "Health_Percent" },
		{ 61, "Mana_Percent" },
		{ 62, "DMG_R_H" },
		{ 63, "DMG_R_M" },
		{ 81, "BS_Multi" },
		{ 100, "C_Health" },
		{ 101, "C_Damage" },
		{ 102, "C_ATSpeed" },
		{ 103, "C_MVSpeed" },
		{ 104, "C_AllAnti" },
		{ 170, "BuffT_Temple" },
		{ 171, "BuffT_Drink" },
		{ 203, "Top_Cut_DMG" },
		{ 204, "Top_Cut_ATS" },
		{ 205, "Top_Cut_MVS" },
		{ 300, "AllDot_DMG" },
		{ 301, "AllDot_Time" },
		{ 303, "AllDot_MV" },
		{ 304, "AllDot_JY" },
		{ 305, "DiffDotDMG" },
		{ 400, "BE_ZQ_DMG" },
		{ 401, "BE_ZQ_ATS" },
		{ 402, "BE_ZQ_MVS" },
		{ 403, "BE_ZQ_BJR" },
		{ 404, "BE_ZQ_BJD" },
		{ 405, "BE_ZQ_Heal" },
		{ 406, "BE_ZQ_Mana" },
		{ 407, "BE_ZQ_CP_Heal" },
		{ 408, "BE_ZQ_CP_DMG" },
		{ 409, "BE_ZQ_CP_ATS" },
		{ 410, "BE_ZQ_CP_MVS" },
		{ 411, "BE_ZQ_CP_Anti" },
		{ 412, "BE_ZQ_Dot" },
		{ 413, "BE_ZQ_XJ_DMG" },
		{ 414, "BE_ZQ_Orb_DMG" },
		{ 415, "BE_SPC_DMG" },
		{ 416, "BE_SPC_ATS" },
		{ 417, "BE_SPC_MVS" },
		{ 418, "BE_SPC_BJR" },
		{ 419, "BE_SPC_BJD" },
		{ 420, "BE_SPC_Heal" },
		{ 421, "BE_SPC_Mana" },
		{ 422, "BE_SPC_CP_Heal" },
		{ 423, "BE_SPC_CP_DMG" },
		{ 424, "BE_SPC_CP_ATS" },
		{ 425, "BE_SPC_CP_MVS" },
		{ 426, "BE_SPC_CP_Anti" },
		{ 427, "BE_SPC_Dot" },
		{ 428, "BE_SPC_XJ_DMG" },
		{ 429, "BE_SPC_Orb_DMG" },
		{ 430, "BE_HH_DMG" },
		{ 431, "BE_HH_ATS" },
		{ 432, "BE_HH_MVS" },
		{ 433, "BE_HH_BJR" },
		{ 434, "BE_HH_BJD" },
		{ 435, "BE_HH_Heal" },
		{ 436, "BE_HH_Mana" },
		{ 437, "BE_HH_CP_Heal" },
		{ 438, "BE_HH_CP_DMG" },
		{ 439, "BE_HH_CP_ATS" },
		{ 440, "BE_HH_CP_MVS" },
		{ 441, "BE_HH_CP_Anti" },
		{ 442, "BE_HH_Dot" },
		{ 443, "BE_HH_XJ_DMG" },
		{ 444, "BE_HH_Orb_DMG" },
		{ 445, "BE_SK_DMG" },
		{ 446, "BE_SK_ATS" },
		{ 447, "BE_SK_MVS" },
		{ 448, "BE_SK_CP_Heal" },
		{ 449, "BE_SK_CP_DMG" },
		{ 450, "BE_SK_CP_ATS" },
		{ 451, "BE_SK_CP_Anti" },
		{ 452, "BE_SK_XJ_DMG" },
		{ 453, "BE_SK_Orb_DMG" },
		{ 455, "BE_BS_DMG" },
		{ 456, "BE_BS_ATS" },
		{ 457, "BE_BS_MVS" },
		{ 458, "BE_BS_CP_Heal" },
		{ 459, "BE_BS_CP_DMG" },
		{ 460, "BE_BS_CP_ATS" },
		{ 461, "BE_BS_CP_Anti" },
		{ 462, "BE_BS_XJ_DMG" },
		{ 463, "BE_BS_Orb_DMG" },
		{ 556, "ST_NoMV_HealPrc" },
		{ 557, "ST_NoMV_ManaPrc" },
		{ 600, "Z_Hmax_DMG" },
		{ 601, "Z_Huse_DMG" },
		{ 602, "Z_Mmax_DMG" },
		{ 603, "Z_Mcur_DMG" },
		{ 604, "Z_Muse_DMG" },
		{ 650, "Z_CD_CP_DMG" },
		{ 651, "Z_ATS_CP_DMG" },
		{ 652, "Z_MVS_DMG" },
		{ 653, "Z_MVS_ATS" },
		{ 700, "ST_EveryH_DMG" },
		{ 701, "ST_EveryM_Drop" },
		{ 1000, "CP1_DMG" },
		{ 1001, "CP1_ATS" },
		{ 1002, "CP1_MVS" },
		{ 1003, "CP1_Heal" },
		{ 1004, "CP1_Mana" },
		{ 1005, "CP1_DMG_Anti" },
		{ 1006, "CP1_DropR" },
		{ 1007, "CP1_ORB_DMG" },
		{ 1020, "CP1_CP_Heal" },
		{ 1021, "CP1_CP_DMG" },
		{ 1022, "CP1_CP_ATS" },
		{ 1023, "CP1_CP_AllAnti" },
		{ 1024, "CLass_DMG" },
		{ 1025, "CLass_ATS" },
		{ 1026, "CLass_MVS" },
		{ 1027, "CLass_Heal" },
		{ 1028, "CLass_Mana" },
		{ 1029, "CLass_DMG_Anti" },
		{ 1030, "CLass_DropR" },
		{ 1031, "CLass_ORB_DMG" },
		{ 1050, "CLass_CP_Heal" },
		{ 1051, "CLass_CP_DMG" },
		{ 1052, "CLass_CP_ATS" },
		{ 1053, "CLass_CP_AllAnti" },
		{ 1054, "Class_CP_DotDMG" },
		{ 1200, "EMC_DMG_20" },
		{ 1201, "EMC_DMG_48" },
		{ 1202, "EMC_Anti_9" },
		{ 1203, "EMC_GD_12" },
		{ 1204, "JYC_DMG_15" },
		{ 1205, "JYC_ATS_24" },
		{ 1206, "JYC_BJD_24" },
		{ 1504, "Orb_Universe_DMG_Base" },
		{ 1506, "Orb_Universe_ATS" },
		{ 1507, "Orb_Bow_DMG" },
		{ 1508, "Orb_Bow_ATS" },
		{ 1815, "DMGsplit" },
		{ 1818, "EXP_Range" },
		{ 1819, "Buff_Range" },
		{ 1950, "Pick_PL_Bei" },
		{ 1951, "Pick_XJL_Bei" },
		{ 1952, "XJL_SellPrice" },
		{ 1953, "XJL_DMG" },
		{ 1955, "XJL_UseSKTime" }
	};

	private static readonly Dictionary<int, string> MainIntFields = new Dictionary<int, string>
	{
		{ 31, "JYBoss_DMG" },
		{ 32, "JYBoss_Anti" },
		{ 80, "BS_Add" },
		{ 150, "WPSPC_DMG" },
		{ 151, "WPSPC_Rate" },
		{ 200, "Top_CD" },
		{ 201, "Top_GD" },
		{ 202, "Top_Anti" },
		{ 302, "AllDot_Layer" },
		{ 306, "DiffDebuff_DMG" },
		{ 454, "BE_SK_FQ_Count" },
		{ 464, "BE_BS_FQ_Count" },
		{ 500, "LowH_DMG20" },
		{ 501, "LowH_DMG50" },
		{ 502, "HighH_DMG90" },
		{ 503, "HighH_DMG100" },
		{ 504, "LowH_HurtR20" },
		{ 505, "HighH_HurtR100" },
		{ 506, "LowH_DMGAnti20" },
		{ 507, "LowH_DMGAnti50" },
		{ 509, "LowM_DMG20" },
		{ 510, "LowM_DMG50" },
		{ 511, "HighM_DMG90" },
		{ 512, "HighM_DMG100" },
		{ 513, "LowM_HurtR20" },
		{ 514, "HighM_HurtR100" },
		{ 550, "ST_MV_DMG" },
		{ 551, "ST_MV_ATS" },
		{ 552, "ST_MV_GD" },
		{ 553, "ST_NoMV_DMG" },
		{ 554, "ST_NoMV_ATS" },
		{ 555, "ST_NoMV_DMGAnti" },
		{ 558, "ST_Chong_DMG" },
		{ 559, "ST_Chong_Anti" },
		{ 1150, "XJ_Count_CP_DMG" },
		{ 1370, "EM_LowH_DMG20" },
		{ 1371, "EM_LowH_DMG50" },
		{ 1372, "EM_HighH_DMG60" },
		{ 1373, "EM_HighH_DMG100" },
		{ 1374, "EM_Heal_Crit" },
		{ 1390, "Dis_In" },
		{ 1395, "Crit_BoomEXP" },
		{ 1396, "Crit_BoomDie_Rate" },
		{ 1397, "Crit_MS" },
		{ 1500, "ORB_FQ_Count" },
		{ 1502, "ORB_FQ_DMG80_Base" },
		{ 1503, "ORB_FQ_DMG120_Base" },
		{ 1505, "HighMana_DMG100_FQ" },
		{ 1600, "XJ_DMG" },
		{ 1601, "XJ_Time" },
		{ 1602, "TuT_Buff" },
		{ 1603, "TuT_Time" },
		{ 1800, "NoDot_BJD" },
		{ 1802, "ManaUse_Rheal" },
		{ 1806, "DMG_ManaPRC" },
		{ 1808, "GD_HurtR" },
		{ 1817, "Diff_EL" },
		{ 1910, "DrinkPre_Heal" },
		{ 1911, "DrinkPre_Mana" },
		{ 1912, "DrinkPre_DMG" }
	};

	private static readonly Dictionary<int, string[]> MainElementFloatFields = new Dictionary<int, string[]>
	{
		{
			610,
			new string[6] { "Z_Hmax_EL0", "Z_Hmax_EL1", "Z_Hmax_EL2", "Z_Hmax_EL3", "Z_Hmax_EL4", "Z_Hmax_EL5" }
		},
		{
			611,
			new string[6] { "Z_Mmax_EL0", "Z_Mmax_EL1", "Z_Mmax_EL2", "Z_Mmax_EL3", "Z_Mmax_EL4", "Z_Mmax_EL5" }
		},
		{
			612,
			new string[6] { "Z_CD_EL0", "Z_CD_EL1", "Z_CD_EL2", "Z_CD_EL3", "Z_CD_EL4", "Z_CD_EL5" }
		},
		{
			1010,
			new string[6] { "CP1_DMG0", "CP1_DMG1", "CP1_DMG2", "CP1_DMG3", "CP1_DMG4", "CP1_DMG5" }
		},
		{
			1011,
			new string[6] { "CP1_Chuan0", "CP1_Chuan1", "CP1_Chuan2", "CP1_Chuan3", "CP1_Chuan4", "CP1_Chuan5" }
		},
		{
			1040,
			new string[6] { "CLass_DMG0", "CLass_DMG1", "CLass_DMG2", "CLass_DMG3", "CLass_DMG4", "CLass_DMG5" }
		},
		{
			1041,
			new string[6] { "CLass_Chuan0", "CLass_Chuan1", "CLass_Chuan2", "CLass_Chuan3", "CLass_Chuan4", "CLass_Chuan5" }
		}
	};

	private static readonly Dictionary<int, string[]> MainElementIntFields = new Dictionary<int, string[]>
	{
		{
			613,
			new string[6] { "Z_Anti0_EL0", "Z_Anti0_EL1", "Z_Anti0_EL2", "Z_Anti0_EL3", "Z_Anti0_EL4", "Z_Anti0_EL5" }
		},
		{
			614,
			new string[6] { "Z_Chuan0_EL0", "Z_Chuan0_EL1", "Z_Chuan0_EL2", "Z_Chuan0_EL3", "Z_Chuan0_EL4", "Z_Chuan0_EL5" }
		},
		{
			615,
			new string[6] { "Z_GD_EL0", "Z_GD_EL1", "Z_GD_EL2", "Z_GD_EL3", "Z_GD_EL4", "Z_GD_EL5" }
		},
		{
			616,
			new string[6] { "Z_BJR_EL0", "Z_BJR_EL1", "Z_BJR_EL2", "Z_BJR_EL3", "Z_BJR_EL4", "Z_BJR_EL5" }
		},
		{
			617,
			new string[6] { "Z_DMGCut_EL0", "Z_DMGCut_EL1", "Z_DMGCut_EL2", "Z_DMGCut_EL3", "Z_DMGCut_EL4", "Z_DMGCut_EL5" }
		},
		{
			618,
			new string[6] { "Z_Thr_EL0", "Z_Thr_EL1", "Z_Thr_EL2", "Z_Thr_EL3", "Z_Thr_EL4", "Z_Thr_EL5" }
		},
		{
			655,
			new string[6] { "Z_Chuan0_BJD", "Z_Chuan1_BJD", "Z_Chuan2_BJD", "Z_Chuan3_BJD", "Z_Chuan4_BJD", "Z_Chuan5_BJD" }
		},
		{
			1300,
			new string[6] { "PrcCut0", "PrcCut1", "PrcCut2", "PrcCut3", "PrcCut4", "PrcCut5" }
		},
		{
			1301,
			new string[6] { "PrcCut5P0", "PrcCut5P1", "PrcCut5P2", "PrcCut5P3", "PrcCut5P4", "PrcCut5P5" }
		},
		{
			1302,
			new string[6] { "PrcCut3P0", "PrcCut3P1", "PrcCut3P2", "PrcCut3P3", "PrcCut3P4", "PrcCut3P5" }
		},
		{
			1330,
			new string[6] { "BurnLife0", "BurnLife1", "BurnLife2", "BurnLife3", "BurnLife4", "BurnLife5" }
		}
	};

	private static readonly Dictionary<int, string> MainBoolFields = new Dictionary<int, string>
	{
		{ 307, "Dot_MSAll" },
		{ 508, "LowH_CritAnti10" },
		{ 654, "Z_BJR_BJD" },
		{ 750, "AB_DMG_Mana" },
		{ 751, "AB_DMG_Hurt" },
		{ 752, "AB_Dot_DMG" },
		{ 753, "NoGD" },
		{ 862, "DeadWD" },
		{ 863, "DeadRageWD" },
		{ 864, "DeadStealthWD" },
		{ 1360, "WS_All" },
		{ 1391, "Dis_Out" },
		{ 1501, "ORB_FQ_Count_Double" },
		{ 1604, "TuT_PlayerAll" },
		{ 1801, "HealCutMana" },
		{ 1803, "RMana_RHeal" },
		{ 1804, "CP_Same_RHeal" },
		{ 1805, "FT" },
		{ 1807, "Turtle" },
		{ 1809, "BloodLost" },
		{ 1810, "NoGround" },
		{ 1811, "CPNoBad" },
		{ 1812, "CPNoGround" },
		{ 1813, "AT_UseHeal1" },
		{ 1814, "AT_UseHeal2" },
		{ 1816, "BladeSoul_Double" },
		{ 1820, "MoneyTO_DMG" },
		{ 1821, "AutoJH" },
		{ 1822, "DieEXP" },
		{ 1900, "AutoDrinkH" },
		{ 1901, "AutoDrinkM" },
		{ 1905, "Drink_CP" }
	};

	private static readonly Dictionary<int, string> DotIntFields = new Dictionary<int, string>
	{
		{ 2000, "Every_Layer" },
		{ 2002, "FJ" },
		{ 2003, "DMG_AddOne" },
		{ 2004, "All_LayerR" },
		{ 2101, "Dot_Infect_Layer" },
		{ 2202, "YB_Add" },
		{ 2203, "YB_MS" },
		{ 2300, "YS" },
		{ 2303, "MH" },
		{ 2305, "JY" },
		{ 2306, "Dead" },
		{ 2401, "BoomDMGUp" },
		{ 2402, "LayerPRC" },
		{ 2450, "BE_CP" },
		{ 2500, "BF_DMG" },
		{ 2501, "DMG50" },
		{ 2550, "LowH_50" },
		{ 2551, "HighH_100" },
		{ 2552, "LowM_40" },
		{ 2600, "FrozenFoever" },
		{ 2601, "FrozenCut" },
		{ 2602, "Frozen30" },
		{ 2603, "FrozenHurtDMG" }
	};

	private static readonly Dictionary<int, string> DotBoolFields = new Dictionary<int, string>
	{
		{ 2001, "Crit_One" },
		{ 2005, "Double_Layer" },
		{ 2100, "Dot_Infect" },
		{ 2102, "Dot_Infect_All" },
		{ 2200, "YB" },
		{ 2201, "YB_half" },
		{ 2301, "SL" },
		{ 2302, "CM" },
		{ 2304, "ZZ" },
		{ 2400, "Dot_Crit" },
		{ 2604, "FrozenForeverDot" }
	};

	private static readonly Dictionary<int, string> BeiFloatFields = new Dictionary<int, string>
	{
		{ 0, "Health_Bei" },
		{ 1, "Mana_Bei" },
		{ 2, "ATSpeed_Bei" },
		{ 3, "MVSpeed_Bei" },
		{ 4, "CoolDown" },
		{ 5, "GeDang" },
		{ 6, "DOTcut" },
		{ 7, "ThroughRate" },
		{ 8, "Damage_Anti" },
		{ 9, "FireDamage_Bei" },
		{ 10, "FrozenDamage_Bei" },
		{ 11, "ThunderDamage_Bei" },
		{ 12, "PoisonDamage_Bei" },
		{ 13, "PhysicsDamage_Bei" },
		{ 14, "ShadowDamage_Bei" },
		{ 15, "FireChuan" },
		{ 16, "FrozenChuan" },
		{ 17, "ThunderChuan" },
		{ 18, "PoisonChuan" },
		{ 19, "PhysicsChuan" },
		{ 20, "ShadowChuan" },
		{ 21, "C_Health" },
		{ 22, "C_Damage" },
		{ 23, "C_ATSpeed" },
		{ 24, "C_MVSpeed" },
		{ 25, "C_AllAnti" },
		{ 26, "Attack_R_health_Percent" },
		{ 27, "Attack_R_mana_Percent" },
		{ 28, "Damage_Bei" },
		{ 29, "BJrate" },
		{ 30, "BJDamage" },
		{ 31, "FlySpeed" },
		{ 32, "ORB_Damage" }
	};

	private static readonly Dictionary<string, string> FwBaseFloatFields = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		{ "DMG", "Damage_Bei" },
		{ "ATS", "ATSpeed_Bei" },
		{ "BJD", "BJDamage" },
		{ "ALLC", "AllChuan" },
		{ "DOT", "AllDot_DMG" },
		{ "C_DMG", "C_Damage" },
		{ "C_ATS", "C_ATSpeed" },
		{ "Heal", "Health_Bei" },
		{ "Mana", "Mana_Bei" },
		{ "Anti", "AllAnti" },
		{ "MVS", "MVSpeed_Bei" },
		{ "C_Heal", "C_Health" },
		{ "C_Anti", "C_AllAnti" },
		{ "Drop", "ItemDrop_Rate" }
	};

	private static readonly Dictionary<string, string> FwBaseIntFields = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		{ "ORB_DMG", "WPSPC_DMG" },
		{ "XJ_DMG", "XJ_DMG" }
	};

	private static readonly Dictionary<int, string> GemFloatFields = new Dictionary<int, string>
	{
		{ 0, "Health_Bei" },
		{ 1, "FireAnti" },
		{ 2, "FireChuan" },
		{ 3, "FireDamage_Bei" },
		{ 4, "ItemDrop_Rate" },
		{ 5, "ThunderAnti" },
		{ 6, "ThunderChuan" },
		{ 7, "ThunderDamage_Bei" },
		{ 8, "C_Health" },
		{ 9, "PoisonAnti" },
		{ 10, "PoisonChuan" },
		{ 11, "C_ATSpeed" },
		{ 12, "PoisonDamage_Bei" },
		{ 13, "Mana_Bei" },
		{ 14, "FrozenAnti" },
		{ 15, "FrozenChuan" },
		{ 16, "FrozenDamage_Bei" },
		{ 17, "C_Damage" },
		{ 18, "ShadowAnti" },
		{ 19, "ShadowChuan" },
		{ 20, "MVSpeed_Bei" },
		{ 21, "ShadowDamage_Bei" },
		{ 22, "ATSpeed_Bei" },
		{ 23, "PhysicsAnti" },
		{ 24, "PhysicsChuan" },
		{ 25, "PhysicsDamage_Bei" },
		{ 26, "BS_ExtraProjectiles" }
	};

	private static readonly HashSet<int> SkElementIndexes = new HashSet<int> { 3530, 3535 };

	private static readonly HashSet<int> CpElementIndexes = new HashSet<int> { 4401 };

	public static void PrepareForWrite(SaveData data)
	{
		if (data != null)
		{
			ClearWeaponTalentLevels(data.TalentData);
			bool playerDataSavedWithoutEquipment = TryStripEquippedWeaponEffects(data);
			SanitizeInventory(data.InventoryData, pruneEffects: false);
			data.PlayerDataSavedWithoutEquipment = playerDataSavedWithoutEquipment;
		}
	}

	public static void PostLoadFix(SaveData data)
	{
		if (data != null)
		{
			ClearWeaponTalentLevels(data.TalentData);
			if (!data.PlayerDataSavedWithoutEquipment && CanStripEquippedWeaponEffects(data.InventoryData) && TryStripEquippedWeaponEffects(data))
			{
				data.PlayerDataSavedWithoutEquipment = true;
			}
			SanitizeInventory(data.InventoryData, pruneEffects: false);
		}
	}

	public static void SanitizeGlobalChestItems(List<ContainerItemSaveData> items)
	{
		SanitizeContainerItems(items, pruneEffects: false);
	}

	public static bool ShouldDropContainerItemOnLoad(ContainerItemSaveData item)
	{
		return IsRemovedRuneContainerItem(item);
	}

	private static bool TryStripEquippedWeaponEffects(SaveData data)
	{
		if (data == null || data.PlayerData == null)
		{
			return true;
		}
		if (!CanStripEquippedWeaponEffects(data.InventoryData))
		{
			return false;
		}
		StripEquippedWeaponEffects(data.PlayerData, data.InventoryData);
		return true;
	}

	private static bool CanStripEquippedWeaponEffects(InventorySaveData inventory)
	{
		if (inventory == null || inventory.Equipments == null)
		{
			return true;
		}
		foreach (WeaponSaveData equipment in inventory.Equipments)
		{
			if (equipment != null)
			{
				NormalizeWeaponShape(equipment);
				if (!CanStripSetEffect(equipment))
				{
					return false;
				}
				if (!CanStripWeaponSkillPointBonuses(equipment))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool CanStripSetEffect(WeaponSaveData weapon)
	{
		if (weapon == null || weapon.Set_Index <= 0)
		{
			return true;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return false;
		}
		if (SingletonMonoScope<ItemManager>.Instance.SET != null)
		{
			return SingletonMonoScope<ItemManager>.Instance.SET.Count > 0;
		}
		return false;
	}

	private static bool CanStripWeaponSkillPointBonuses(WeaponSaveData weapon)
	{
		if (!HasActiveWeaponSkillSocket(weapon))
		{
			return true;
		}
		if (!SingletonMonoScope<TalentManager>.HasInstance)
		{
			return false;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance == null || instance.SKI == null || instance.SKI.Count == 0 || instance.XiData == null)
		{
			return false;
		}
		return true;
	}

	private static void StripEquippedWeaponEffects(PlayerSaveData player, InventorySaveData inventory)
	{
		if (player == null)
		{
			return;
		}
		EnsurePlayerDotData(player);
		if (inventory == null || inventory.Equipments == null)
		{
			player.EquippedSetCounts = new Dictionary<int, int>();
			ClampCorePlayerValues(player);
			return;
		}
		Dictionary<int, int> setCounts = new Dictionary<int, int>();
		foreach (WeaponSaveData equipment in inventory.Equipments)
		{
			if (equipment != null)
			{
				NormalizeWeaponShape(equipment);
				StripWeaponBaseValues(player, equipment);
				StripWeaponElementValues(player, equipment);
				StripMainEffects(player, equipment.Main);
				StripDotEffects(player, equipment.DOT);
				StripWeaponSkillPointBonuses(player, equipment);
				StripGemEffects(player, equipment);
				StripFwBaseEffect(player, equipment.FW_Base);
				StripSetEffect(player, equipment, setCounts);
			}
		}
		player.EquippedSetCounts = new Dictionary<int, int>();
		ClampCorePlayerValues(player);
	}

	private static void StripWeaponBaseValues(PlayerSaveData player, WeaponSaveData weapon)
	{
		float baseValueMultiplier = GetBaseValueMultiplier(weapon);
		player.Damage_Base -= weapon.Damage * baseValueMultiplier;
		player.Health -= weapon.Health * baseValueMultiplier;
		player.Mana -= weapon.Mana * baseValueMultiplier;
	}

	private static void StripWeaponElementValues(PlayerSaveData player, WeaponSaveData weapon)
	{
		string weaponType = weapon.WeaponType;
		if (weaponType == null)
		{
			return;
		}
		switch (weaponType.Length)
		{
		case 4:
		{
			char c = weaponType[1];
			if (c != 'a')
			{
				if (c != 'e')
				{
					if (c != 'o')
					{
						break;
					}
					if (!(weaponType == "body"))
					{
						if (!(weaponType == "bone"))
						{
							break;
						}
						goto IL_01de;
					}
				}
				else if (!(weaponType == "head"))
				{
					break;
				}
			}
			else if (!(weaponType == "hand"))
			{
				break;
			}
			goto IL_018f;
		}
		case 3:
		{
			char c = weaponType[0];
			if (c != 'b')
			{
				if (c != 'l' || !(weaponType == "leg"))
				{
					break;
				}
				goto IL_018f;
			}
			if (!(weaponType == "bow"))
			{
				break;
			}
			goto IL_01de;
		}
		case 5:
			switch (weaponType[1])
			{
			case 't':
				break;
			case 'w':
				goto IL_0129;
			case 'p':
				goto IL_013a;
			case 'r':
				goto IL_014b;
			default:
				return;
			}
			if (!(weaponType == "staff"))
			{
				break;
			}
			goto IL_01de;
		case 6:
			{
				switch (weaponType[0])
				{
				default:
					return;
				case 's':
					if (!(weaponType == "shield"))
					{
						return;
					}
					break;
				case 'c':
					if (!(weaponType == "corpse"))
					{
						return;
					}
					break;
				case 'l':
					if (weaponType == "little")
					{
						if (weapon.CharType == 6)
						{
							AddElementAnti(player, 0f - weapon.Fire, 0f - weapon.Frozen, 0f - weapon.Thunder, 0f - weapon.Poison, 0f - weapon.Physics, 0f - weapon.Shadow);
						}
						else if (weapon.CharType == 7 || weapon.CharType == 9)
						{
							AddElementDamage(player, 0f - weapon.Fire, 0f - weapon.Frozen, 0f - weapon.Thunder, 0f - weapon.Poison, 0f - weapon.Physics, 0f - weapon.Shadow);
						}
						else if (weapon.CharType == 8)
						{
							AddElementChuan(player, 0f - weapon.Fire, 0f - weapon.Frozen, 0f - weapon.Thunder, 0f - weapon.Poison, 0f - weapon.Physics, 0f - weapon.Shadow);
						}
					}
					return;
				}
				goto IL_022d;
			}
			IL_014b:
			if (!(weaponType == "arrow"))
			{
				break;
			}
			goto IL_022d;
			IL_013a:
			if (!(weaponType == "spell"))
			{
				break;
			}
			goto IL_022d;
			IL_022d:
			AddElementChuan(player, 0f - weapon.Fire, 0f - weapon.Frozen, 0f - weapon.Thunder, 0f - weapon.Poison, 0f - weapon.Physics, 0f - weapon.Shadow);
			break;
			IL_0129:
			if (!(weaponType == "sword"))
			{
				break;
			}
			goto IL_01de;
			IL_018f:
			AddElementAnti(player, 0f - weapon.Fire, 0f - weapon.Frozen, 0f - weapon.Thunder, 0f - weapon.Poison, 0f - weapon.Physics, 0f - weapon.Shadow);
			break;
			IL_01de:
			AddElementDamage(player, 0f - weapon.Fire, 0f - weapon.Frozen, 0f - weapon.Thunder, 0f - weapon.Poison, 0f - weapon.Physics, 0f - weapon.Shadow);
			break;
		}
	}

	private static void StripMainEffects(PlayerSaveData player, WPDT_A[] stats)
	{
		if (stats != null)
		{
			foreach (WPDT_A stat in stats)
			{
				ApplyMainEffectToPlayerData(player, stat, -1);
			}
		}
	}

	private static void StripDotEffects(PlayerSaveData player, WPDT_A[] stats)
	{
		if (stats != null)
		{
			foreach (WPDT_A stat in stats)
			{
				ApplyDotEffectToPlayerData(player, stat, -1);
			}
		}
	}

	private static void StripGemEffects(PlayerSaveData player, WeaponSaveData weapon)
	{
		if (weapon == null || weapon.Aocao == null || weapon.AocaoCount <= 0)
		{
			return;
		}
		int num = Mathf.Min(weapon.AocaoCount, weapon.Aocao.Count);
		for (int i = 0; i < num; i++)
		{
			WPAocaoSaveData wPAocaoSaveData = weapon.Aocao[i];
			if (IsActiveGemSocket(wPAocaoSaveData) && GemFloatFields.TryGetValue(wPAocaoSaveData.Type, out var value))
			{
				AddFloat(PlayerFields, player, value, 0f - wPAocaoSaveData.Number);
			}
		}
	}

	private static void StripFwBaseEffect(PlayerSaveData player, WPFW_Base fwBase)
	{
		if (player != null && fwBase != null && !string.IsNullOrEmpty(fwBase.type))
		{
			if (FwBaseFloatFields.TryGetValue(fwBase.type, out var value))
			{
				AddFloat(PlayerFields, player, value, 0f - fwBase.number);
			}
			else if (FwBaseIntFields.TryGetValue(fwBase.type, out value))
			{
				AddInt(PlayerFields, player, value, -Mathf.RoundToInt(fwBase.number));
			}
		}
	}

	private static void StripSetEffect(PlayerSaveData player, WeaponSaveData weapon, Dictionary<int, int> setCounts)
	{
		if (player == null || weapon == null || weapon.Set_Index <= 0 || setCounts == null || !SingletonMonoScope<ItemManager>.HasInstance)
		{
			return;
		}
		Set_DT value = weapon.SetRuntimeData;
		if (((value != null && value.SetID == weapon.Set_Index) || (SingletonMonoScope<ItemManager>.Instance.SET != null && SingletonMonoScope<ItemManager>.Instance.SET.TryGetValue(weapon.Set_Index, out value) && value != null)) && value.Lit != null)
		{
			setCounts.TryGetValue(weapon.Set_Index, out var value2);
			int num = value2 + 1;
			setCounts[weapon.Set_Index] = num;
			int num2 = num - 2;
			if (num2 >= 0 && num2 < value.Lit.Length)
			{
				StripSetLit(player, value.Lit[num2]);
			}
		}
	}

	private static void StripSetLit(PlayerSaveData player, Set_DT_Lit lit)
	{
		if (player != null && lit != null)
		{
			if (lit.MainTP == 0)
			{
				ApplyMainEffectToPlayerData(player, new WPDT_A
				{
					Index = lit.Index,
					EL = lit.EL,
					number = lit.Number
				}, -1);
			}
			else if (lit.MainTP == 1)
			{
				ApplyDotEffectToPlayerData(player, new WPDT_A
				{
					Index = lit.Index,
					EL = lit.EL,
					number = lit.Number
				}, -1);
			}
		}
	}

	private static void StripWeaponSkillPointBonuses(PlayerSaveData player, WeaponSaveData weapon)
	{
		if (player == null || weapon == null || !SingletonMonoScope<TalentManager>.HasInstance || weapon.WPSK == null || weapon.WP_SkillCount <= 0)
		{
			return;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance == null || instance.SKI == null || instance.XiData == null)
		{
			return;
		}
		int num = Mathf.Min(weapon.WP_SkillCount, weapon.WPSK.Count);
		for (int i = 0; i < num; i++)
		{
			WPSkillSaveData wPSkillSaveData = weapon.WPSK[i];
			if (wPSkillSaveData == null || IsEmptyName(wPSkillSaveData.IndexName))
			{
				continue;
			}
			int num2 = wPSkillSaveData.Number + wPSkillSaveData.Number2;
			if (num2 <= 0)
			{
				continue;
			}
			SKindex value = null;
			if (instance.SKI.TryGetValue(wPSkillSaveData.IndexName, out value) && value != null && value.type == 6 && value.Xi >= 0 && value.Xi < instance.XiData.Length)
			{
				SkillXiData skillXiData = instance.XiData[value.Xi];
				if (skillXiData != null && skillXiData.Bei != null && skillXiData.Bei.TryGetValue(wPSkillSaveData.IndexName, out var value2) && value2 != null && value2.Level_Base > 0)
				{
					ApplyBeiEffectToPlayerData(player, value2.B_Type, value2.B_Number, -num2);
				}
			}
		}
	}

	private static void ApplyMainEffectToPlayerData(PlayerSaveData player, WPDT_A stat, int direction)
	{
		if (player != null && stat != null && direction != 0)
		{
			float value = stat.number * (float)direction;
			int value2 = Mathf.FloorToInt(stat.number) * direction;
			string[] value4;
			if (MainFloatFields.TryGetValue(stat.Index, out var value3))
			{
				AddFloat(PlayerFields, player, value3, value);
			}
			else if (MainIntFields.TryGetValue(stat.Index, out value3))
			{
				AddInt(PlayerFields, player, value3, value2);
			}
			else if (MainBoolFields.TryGetValue(stat.Index, out value3))
			{
				SetBool(PlayerFields, player, value3, direction > 0);
			}
			else if (MainElementFloatFields.TryGetValue(stat.Index, out value4))
			{
				AddElementFloat(player, value4, stat.EL, value);
			}
			else if (MainElementIntFields.TryGetValue(stat.Index, out value4))
			{
				AddElementInt(player, value4, stat.EL, value2);
			}
		}
	}

	private static void ApplyDotEffectToPlayerData(PlayerSaveData player, WPDT_A stat, int direction)
	{
		if (player == null || stat == null || direction == 0 || !IsElement(stat.EL))
		{
			return;
		}
		PlayerDotData dotByElement = GetDotByElement(player, stat.EL);
		if (dotByElement != null)
		{
			if (DotIntFields.TryGetValue(stat.Index, out var value))
			{
				AddInt(DotFields, dotByElement, value, Mathf.FloorToInt(stat.number) * direction);
			}
			else if (DotBoolFields.TryGetValue(stat.Index, out value))
			{
				SetBool(DotFields, dotByElement, value, direction > 0);
			}
		}
	}

	private static void ApplyBeiEffectToPlayerData(PlayerSaveData player, int type, float number, int level)
	{
		if (BeiFloatFields.TryGetValue(type, out var value))
		{
			float num = ((type == 26 || type == 27) ? 0.01f : 1f);
			AddFloat(PlayerFields, player, value, number * num * (float)level);
		}
	}

	private static void SanitizeInventory(InventorySaveData data, bool pruneEffects)
	{
		if (data == null)
		{
			return;
		}
		data.Equipments = data.Equipments ?? new List<WeaponSaveData>();
		data.InventoryItems = data.InventoryItems ?? new List<ContainerItemSaveData>();
		for (int i = 0; i < data.Equipments.Count; i++)
		{
			WeaponSaveData weaponSaveData = data.Equipments[i];
			if (weaponSaveData != null)
			{
				SanitizeWeapon(weaponSaveData, pruneEffects);
			}
		}
		SanitizeContainerItems(data.InventoryItems, pruneEffects);
	}

	private static void SanitizeContainerItems(List<ContainerItemSaveData> items, bool pruneEffects)
	{
		if (items == null)
		{
			return;
		}
		for (int num = items.Count - 1; num >= 0; num--)
		{
			ContainerItemSaveData containerItemSaveData = items[num];
			if (containerItemSaveData == null)
			{
				items.RemoveAt(num);
			}
			else if (ShouldDropContainerItemOnLoad(containerItemSaveData))
			{
				items.RemoveAt(num);
			}
			else if (containerItemSaveData.ItemType == 0 && containerItemSaveData.Weapon != null)
			{
				SanitizeWeapon(containerItemSaveData.Weapon, pruneEffects);
			}
		}
	}

	private static void SanitizeWeapon(WeaponSaveData weapon, bool pruneEffects)
	{
		if (weapon != null)
		{
			NormalizeWeaponShape(weapon);
			if (pruneEffects)
			{
				weapon.Main = SanitizeAArray(weapon.Main, IsValidMainEffect, requiresElement: false);
				weapon.DOT = SanitizeAArray(weapon.DOT, IsValidDotEffect, requiresElement: true);
				weapon.SK = SanitizeBArray(weapon.SK, IsValidSkEffect, SkElementIndexes);
				weapon.CP = SanitizeBArray(weapon.CP, IsValidCpEffect, CpElementIndexes);
				SanitizeSetIndex(weapon);
			}
			else
			{
				weapon.Main = RemoveEmptyAArrayEntries(weapon.Main, requiresElement: false);
				weapon.DOT = RemoveEmptyAArrayEntries(weapon.DOT, requiresElement: true);
				weapon.SK = RemoveEmptyBArrayEntries(weapon.SK);
				weapon.CP = RemoveEmptyBArrayEntries(weapon.CP);
			}
			SanitizeFwBase(weapon);
			SanitizeSpcData(weapon, pruneEffects);
			SanitizeWeaponSkillSockets(weapon, pruneEffects);
			SanitizeGemSockets(weapon, pruneEffects);
		}
	}

	private static void NormalizeWeaponShape(WeaponSaveData weapon)
	{
		if (weapon != null)
		{
			if (weapon.RebuildTime < 0)
			{
				weapon.RebuildTime = 0;
			}
			if (weapon.EnhanceTime < 0)
			{
				weapon.EnhanceTime = 0;
			}
			if (weapon.HHTime < 0)
			{
				weapon.HHTime = 0;
			}
			if (weapon.SkillFWTime < 0)
			{
				weapon.SkillFWTime = 0;
			}
			if (weapon.JHEL_Count < 0)
			{
				weapon.JHEL_Count = 0;
			}
			if (weapon.JH_Count < 0)
			{
				weapon.JH_Count = 0;
			}
			weapon.DropScene = Mathf.Clamp(weapon.DropScene, 0, 4);
			weapon.MJ_Level = ((weapon.DropScene > 0) ? Mathf.Max(1, weapon.MJ_Level) : 0);
			if (weapon.SkillFW_CountMax < 0)
			{
				weapon.SkillFW_CountMax = 0;
			}
			if (weapon.SPC_DMG_Bei <= 0f)
			{
				weapon.SPC_DMG_Bei = 100f;
			}
			if (weapon.BaseValueMultiplier <= 0f)
			{
				weapon.BaseValueMultiplier = (weapon.BaseValueDoubled ? 2f : 1f);
			}
			if (weapon.BaseValueDoubled && weapon.BaseValueMultiplier < 1.0001f)
			{
				weapon.BaseValueMultiplier = 2f;
			}
			if (!weapon.BaseValueDoubled && weapon.BaseValueMultiplier < 1f)
			{
				weapon.BaseValueMultiplier = 1f;
			}
			weapon.WeaponType = weapon.WeaponType ?? "";
			weapon.ItemName = weapon.ItemName ?? "";
			weapon.WPSK = weapon.WPSK ?? new List<WPSkillSaveData>();
			weapon.Aocao = weapon.Aocao ?? new List<WPAocaoSaveData>();
			weapon.SPC = weapon.SPC ?? new List<WPSPC>();
			if (weapon.WP_SkillCount < 0)
			{
				weapon.WP_SkillCount = 0;
			}
			if (weapon.WP_SkillCount > weapon.WPSK.Count)
			{
				weapon.WP_SkillCount = weapon.WPSK.Count;
			}
			if (weapon.MaxAocaoCount < 0)
			{
				weapon.MaxAocaoCount = 0;
			}
			if (weapon.AocaoCount < 0)
			{
				weapon.AocaoCount = 0;
			}
			if (weapon.AocaoCount > weapon.Aocao.Count)
			{
				weapon.AocaoCount = weapon.Aocao.Count;
			}
			if (weapon.MaxAocaoCount > 0 && weapon.AocaoCount > weapon.MaxAocaoCount)
			{
				weapon.AocaoCount = weapon.MaxAocaoCount;
			}
		}
	}

	private static WPDT_A[] SanitizeAArray(WPDT_A[] source, Func<int, bool> isValidEffect, bool requiresElement)
	{
		if (source == null)
		{
			return null;
		}
		List<WPDT_A> list = new List<WPDT_A>();
		foreach (WPDT_A wPDT_A in source)
		{
			if (wPDT_A != null && wPDT_A.Index > 0 && isValidEffect(wPDT_A.Index) && (!requiresElement || IsElement(wPDT_A.EL)) && (requiresElement || (!MainElementFloatFields.ContainsKey(wPDT_A.Index) && !MainElementIntFields.ContainsKey(wPDT_A.Index)) || IsElement(wPDT_A.EL)))
			{
				list.Add(wPDT_A);
			}
		}
		return list.ToArray();
	}

	private static WPDT_A[] RemoveEmptyAArrayEntries(WPDT_A[] source, bool requiresElement)
	{
		if (source == null)
		{
			return null;
		}
		List<WPDT_A> list = new List<WPDT_A>();
		foreach (WPDT_A wPDT_A in source)
		{
			if (wPDT_A != null && wPDT_A.Index > 0 && (!requiresElement || IsElement(wPDT_A.EL)))
			{
				list.Add(wPDT_A);
			}
		}
		return list.ToArray();
	}

	private static WPDT_B[] SanitizeBArray(WPDT_B[] source, Func<int, bool> isValidEffect, HashSet<int> elementIndexes)
	{
		if (source == null)
		{
			return null;
		}
		List<WPDT_B> list = new List<WPDT_B>();
		foreach (WPDT_B wPDT_B in source)
		{
			if (wPDT_B != null && wPDT_B.Index > 0 && isValidEffect(wPDT_B.Index) && !IsEmptyName(wPDT_B.SkillName) && (elementIndexes == null || !elementIndexes.Contains(wPDT_B.Index) || IsElement(wPDT_B.EL)) && HasTalentSkill(wPDT_B.SkillName) && (wPDT_B.Index != 3000 || HasSkillChangeData(wPDT_B.GlobleID)) && (wPDT_B.Index != 4000 || HasCompSkillChangeData(wPDT_B.GlobleID)))
			{
				list.Add(wPDT_B);
			}
		}
		return list.ToArray();
	}

	private static WPDT_B[] RemoveEmptyBArrayEntries(WPDT_B[] source)
	{
		if (source == null)
		{
			return null;
		}
		List<WPDT_B> list = new List<WPDT_B>();
		foreach (WPDT_B wPDT_B in source)
		{
			if (wPDT_B != null && wPDT_B.Index > 0 && !IsEmptyName(wPDT_B.SkillName))
			{
				list.Add(wPDT_B);
			}
		}
		return list.ToArray();
	}

	private static void SanitizeSetIndex(WeaponSaveData weapon)
	{
		if (weapon != null && weapon.Set_Index > 0 && SingletonMonoScope<ItemManager>.HasInstance)
		{
			if (weapon.SetRuntimeData != null && weapon.SetRuntimeData.SetID != weapon.Set_Index)
			{
				weapon.SetRuntimeData = null;
			}
			if (SingletonMonoScope<ItemManager>.Instance.SET != null && SingletonMonoScope<ItemManager>.Instance.SET.Count > 0 && (!SingletonMonoScope<ItemManager>.Instance.SET.TryGetValue(weapon.Set_Index, out var value) || value == null))
			{
				weapon.Set_Index = 0;
				weapon.SetRuntimeData = null;
			}
		}
	}

	private static void SanitizeFwBase(WeaponSaveData weapon)
	{
		if (weapon != null && weapon.FW_Base != null && (string.IsNullOrEmpty(weapon.FW_Base.type) || (!FwBaseFloatFields.ContainsKey(weapon.FW_Base.type) && !FwBaseIntFields.ContainsKey(weapon.FW_Base.type))))
		{
			weapon.FW_Base.type = "";
			weapon.FW_Base.FWname = "";
			weapon.FW_Base.number = 0f;
			weapon.FW_Base.price = 0;
		}
	}

	private static void SanitizeSpcData(WeaponSaveData weapon, bool pruneEffects)
	{
		if (weapon == null)
		{
			return;
		}
		if (weapon.SPC == null)
		{
			weapon.SPC = new List<WPSPC>();
		}
		for (int i = 0; i < weapon.SPC.Count; i++)
		{
			if (weapon.SPC[i] == null)
			{
				weapon.SPC[i] = new WPSPC();
			}
			if (weapon.SPC[i].Index <= 0)
			{
				ClearSpc(weapon.SPC[i]);
			}
			else if (pruneEffects && !HasSpcTemplate(i, weapon.SPC[i].Index))
			{
				ClearSpc(weapon.SPC[i]);
			}
		}
		if (weapon.SPC.Count > 0)
		{
			WPSPC wPSPC = weapon.SPC[0];
			weapon.SPCindex = wPSPC?.Index ?? 0;
			weapon.SPC_EL = wPSPC?.EL ?? 0;
			weapon.SPC_PRC = wPSPC?.PRC ?? 0f;
		}
		else if (weapon.SPCindex <= 0 || (pruneEffects && !HasSpcTemplate(0, weapon.SPCindex)))
		{
			weapon.SPCindex = 0;
			weapon.SPC_EL = 0;
			weapon.SPC_PRC = 0f;
		}
	}

	private static void SanitizeWeaponSkillSockets(WeaponSaveData weapon, bool pruneEffects)
	{
		if (weapon == null || weapon.WPSK == null)
		{
			return;
		}
		for (int i = 0; i < weapon.WPSK.Count; i++)
		{
			if (weapon.WPSK[i] == null)
			{
				weapon.WPSK[i] = new WPSkillSaveData();
			}
			WPSkillSaveData wPSkillSaveData = weapon.WPSK[i];
			if (i >= weapon.WP_SkillCount || IsEmptyName(wPSkillSaveData.IndexName) || wPSkillSaveData.Number + wPSkillSaveData.Number2 <= 0)
			{
				ClearSkillSocket(wPSkillSaveData);
			}
			else if (pruneEffects && !HasApplicableTalentSkill(wPSkillSaveData.IndexName))
			{
				ClearSkillSocket(wPSkillSaveData);
			}
		}
	}

	private static void SanitizeGemSockets(WeaponSaveData weapon, bool pruneEffects)
	{
		if (weapon == null || weapon.Aocao == null)
		{
			return;
		}
		for (int i = 0; i < weapon.Aocao.Count; i++)
		{
			if (weapon.Aocao[i] == null)
			{
				weapon.Aocao[i] = new WPAocaoSaveData();
			}
			WPAocaoSaveData wPAocaoSaveData = weapon.Aocao[i];
			if (i >= weapon.AocaoCount || !wPAocaoSaveData.HasAocao)
			{
				CloseGemSocket(wPAocaoSaveData);
			}
			else if (!wPAocaoSaveData.HasBaoshi)
			{
				ClearGem(wPAocaoSaveData);
			}
			else if (wPAocaoSaveData.Type < 0 || wPAocaoSaveData.Type > 25 || IsEmptyName(wPAocaoSaveData.Name) || (pruneEffects && !HasBaoshi(wPAocaoSaveData.Name)))
			{
				ClearGem(wPAocaoSaveData);
			}
		}
	}

	private static bool IsValidMainEffect(int index)
	{
		if (index <= 0)
		{
			return false;
		}
		if (SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.WP_Main != null && SingletonMonoScope<ItemManager>.Instance.WP_Main.Count > 0)
		{
			return SingletonMonoScope<ItemManager>.Instance.WP_Main.ContainsKey(index);
		}
		return true;
	}

	private static bool IsValidDotEffect(int index)
	{
		if (index <= 0)
		{
			return false;
		}
		if (SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.WP_DOT != null && SingletonMonoScope<ItemManager>.Instance.WP_DOT.Count > 0)
		{
			return SingletonMonoScope<ItemManager>.Instance.WP_DOT.ContainsKey(index);
		}
		return true;
	}

	private static bool IsValidSkEffect(int index)
	{
		if (index <= 0)
		{
			return false;
		}
		if (SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.WP_SK != null && SingletonMonoScope<ItemManager>.Instance.WP_SK.Count > 0)
		{
			return HasWeaponBEffect(SingletonMonoScope<ItemManager>.Instance.WP_SK, index);
		}
		return true;
	}

	private static bool IsValidCpEffect(int index)
	{
		if (index <= 0)
		{
			return false;
		}
		if (SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.WP_CP != null && SingletonMonoScope<ItemManager>.Instance.WP_CP.Count > 0)
		{
			return HasWeaponBEffect(SingletonMonoScope<ItemManager>.Instance.WP_CP, index);
		}
		if (SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.WP_SK != null && SingletonMonoScope<ItemManager>.Instance.WP_SK.Count > 0)
		{
			return HasWeaponBEffect(SingletonMonoScope<ItemManager>.Instance.WP_SK, index);
		}
		return true;
	}

	private static bool HasWeaponBEffect(Dictionary<int, WPDT_RandomB> source, int index)
	{
		if (source == null || source.Count == 0)
		{
			return false;
		}
		foreach (WPDT_RandomB value in source.Values)
		{
			if (value == null || value.RD == null)
			{
				continue;
			}
			for (int i = 0; i < value.RD.Length; i++)
			{
				WPDT_B wPDT_B = value.RD[i];
				if (wPDT_B != null && wPDT_B.Index == index)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool HasSpcTemplate(int slotIndex, int spcIndex)
	{
		if (spcIndex <= 0)
		{
			return false;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return true;
		}
		if (!HasLoadedSpcTemplates(slotIndex == 0))
		{
			return true;
		}
		SPC_MB mb;
		if (slotIndex != 0)
		{
			return SingletonMonoScope<ItemManager>.Instance.TryGetSPCMBByIndex(spcIndex, out mb);
		}
		return SingletonMonoScope<ItemManager>.Instance.TryGetWeaponSPCMBByIndex(spcIndex, out mb);
	}

	private static bool HasBaoshi(string itemName)
	{
		if (IsEmptyName(itemName))
		{
			return false;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return true;
		}
		if (!HasLoadedBaoshiTemplates())
		{
			return true;
		}
		SingletonMonoScope<ItemManager>.Instance.TryGetBaoshiByItemName(itemName, out var data);
		return data != null;
	}

	private static bool IsRemovedRuneContainerItem(ContainerItemSaveData item)
	{
		if (item == null || item.ItemType != 1 || item.Baoshi == null)
		{
			return false;
		}
		return item.Baoshi.UseType switch
		{
			3 => !HasSkillRuneTemplate(item.Baoshi), 
			4 => !HasSpcRuneTemplate(item.Baoshi), 
			_ => false, 
		};
	}

	private static bool HasSkillRuneTemplate(BaoshiSaveData baoshi)
	{
		if (baoshi == null)
		{
			return false;
		}
		if (!SingletonMonoScope<TalentManager>.HasInstance)
		{
			return true;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance == null)
		{
			return true;
		}
		instance.EnsureSkillFWLibrary();
		if (!HasLoadedSkillRuneTemplates(instance))
		{
			return true;
		}
		bool flag = !IsEmptyName(baoshi.SKname);
		if (flag && TryGetSkillRuneByName(instance, baoshi.SKname, out var skillRune))
		{
			return true;
		}
		if (baoshi.Index > 0 && TryGetSkillRuneByIndex(instance, baoshi.Index, out skillRune))
		{
			if (flag)
			{
				return string.Equals(skillRune.SkillName, baoshi.SKname, StringComparison.Ordinal);
			}
			return true;
		}
		return false;
	}

	private static bool HasSpcRuneTemplate(BaoshiSaveData baoshi)
	{
		if (baoshi == null || baoshi.Index <= 0)
		{
			return false;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return true;
		}
		if (!HasLoadedSpcTemplates(weaponSlot: false))
		{
			return true;
		}
		if (SingletonMonoScope<ItemManager>.Instance.TryGetSPCMBByIndex(baoshi.Index, out var mb))
		{
			return mb != null;
		}
		return false;
	}

	private static bool HasLoadedSkillRuneTemplates(TalentManager talent)
	{
		if (talent == null || talent.FW == null || talent.FW.Char == null)
		{
			return false;
		}
		for (int i = 0; i < talent.FW.Char.Length; i++)
		{
			SKFW_Char sKFW_Char = talent.FW.Char[i];
			if (sKFW_Char == null || sKFW_Char.Xi == null)
			{
				continue;
			}
			for (int j = 0; j < sKFW_Char.Xi.Length; j++)
			{
				SKFW_Xi sKFW_Xi = sKFW_Char.Xi[j];
				if (sKFW_Xi == null || sKFW_Xi.FW == null)
				{
					continue;
				}
				for (int k = 0; k < sKFW_Xi.FW.Length; k++)
				{
					if (sKFW_Xi.FW[k] != null && !IsEmptyName(sKFW_Xi.FW[k].SkillName))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool TryGetSkillRuneByName(TalentManager talent, string skillName, out SKFW skillRune)
	{
		skillRune = null;
		if (talent == null || talent.FW == null || talent.FW.Char == null || IsEmptyName(skillName))
		{
			return false;
		}
		for (int i = 0; i < talent.FW.Char.Length; i++)
		{
			SKFW_Char sKFW_Char = talent.FW.Char[i];
			if (sKFW_Char == null || sKFW_Char.Xi == null)
			{
				continue;
			}
			for (int j = 0; j < sKFW_Char.Xi.Length; j++)
			{
				SKFW_Xi sKFW_Xi = sKFW_Char.Xi[j];
				if (sKFW_Xi == null || sKFW_Xi.FW == null)
				{
					continue;
				}
				for (int k = 0; k < sKFW_Xi.FW.Length; k++)
				{
					SKFW sKFW = sKFW_Xi.FW[k];
					if (sKFW != null && string.Equals(sKFW.SkillName, skillName, StringComparison.Ordinal))
					{
						skillRune = sKFW;
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool TryGetSkillRuneByIndex(TalentManager talent, int skillIndex, out SKFW skillRune)
	{
		skillRune = null;
		if (talent == null || talent.FW == null || talent.FW.Char == null || skillIndex <= 0)
		{
			return false;
		}
		for (int i = 0; i < talent.FW.Char.Length; i++)
		{
			SKFW_Char sKFW_Char = talent.FW.Char[i];
			if (sKFW_Char == null || sKFW_Char.Xi == null)
			{
				continue;
			}
			for (int j = 0; j < sKFW_Char.Xi.Length; j++)
			{
				SKFW_Xi sKFW_Xi = sKFW_Char.Xi[j];
				if (sKFW_Xi == null || sKFW_Xi.FW == null)
				{
					continue;
				}
				for (int k = 0; k < sKFW_Xi.FW.Length; k++)
				{
					SKFW sKFW = sKFW_Xi.FW[k];
					if (sKFW != null && sKFW.index == skillIndex)
					{
						skillRune = sKFW;
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool HasLoadedSpcTemplates(bool weaponSlot)
	{
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return false;
		}
		ItemManager instance = SingletonMonoScope<ItemManager>.Instance;
		if (instance == null)
		{
			return false;
		}
		if (weaponSlot)
		{
			if (instance.SPC != null)
			{
				return instance.SPC.Count > 0;
			}
			return false;
		}
		if (instance.SPC_Rune != null && instance.SPC_Rune.Count > 0)
		{
			return true;
		}
		return HasAnySpcMb(instance.SPCMB);
	}

	private static bool HasAnySpcMb(SPCMB_Group group)
	{
		if (group == null)
		{
			return false;
		}
		if (group.MB != null && group.MB.Any((SPC_MB mb) => mb != null))
		{
			return true;
		}
		if (group.PL == null)
		{
			return false;
		}
		for (int i = 0; i < group.PL.Length; i++)
		{
			SPCMB_Player sPCMB_Player = group.PL[i];
			if (sPCMB_Player == null || sPCMB_Player.TP == null)
			{
				continue;
			}
			for (int j = 0; j < sPCMB_Player.TP.Length; j++)
			{
				SPCMB_Type sPCMB_Type = sPCMB_Player.TP[j];
				if (sPCMB_Type != null && sPCMB_Type.MB != null && sPCMB_Type.MB.Any((SPC_MB mb) => mb != null))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool HasLoadedBaoshiTemplates()
	{
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return false;
		}
		ItemManager instance = SingletonMonoScope<ItemManager>.Instance;
		if (instance == null)
		{
			return false;
		}
		if (!HasAnyItem(instance.Baoshi) && !HasAnyItem(instance.BaoshiJH))
		{
			return HasAnyItem(instance.BaoshiSPC);
		}
		return true;
	}

	private static bool HasAnyItem(IEnumerable<BaoshiClass> items)
	{
		return items?.Any((BaoshiClass item) => HasItemName(item)) ?? false;
	}

	private static bool HasItemName(ItemClass item)
	{
		if (item != null)
		{
			return !IsEmptyName(item.ItemName);
		}
		return false;
	}

	private static bool HasTalentSkill(string indexName)
	{
		if (IsEmptyName(indexName))
		{
			return false;
		}
		if (!SingletonMonoScope<TalentManager>.HasInstance)
		{
			return true;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance.SKI != null && instance.SKI.Count != 0)
		{
			return instance.SKI.ContainsKey(indexName);
		}
		return true;
	}

	private static bool HasApplicableTalentSkill(string indexName)
	{
		if (IsEmptyName(indexName))
		{
			return false;
		}
		if (!SingletonMonoScope<TalentManager>.HasInstance)
		{
			return true;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance == null || instance.SKI == null || instance.SKI.Count == 0 || instance.XiData == null)
		{
			return true;
		}
		if (!instance.SKI.TryGetValue(indexName, out var value) || value == null)
		{
			return false;
		}
		return HasSkillRuntimeData(instance, value, indexName);
	}

	private static bool HasSkillRuntimeData(TalentManager talent, SKindex skillIndex, string indexName)
	{
		if (talent == null || skillIndex == null || talent.XiData == null || skillIndex.Xi < 0 || skillIndex.Xi >= talent.XiData.Length)
		{
			return false;
		}
		SkillXiData skillXiData = talent.XiData[skillIndex.Xi];
		if (skillXiData == null)
		{
			return false;
		}
		switch (skillIndex.type)
		{
		case 0:
		{
			if (skillXiData.Sample_F != null && skillXiData.Sample_F.TryGetValue(indexName, out var value2))
			{
				return value2 != null;
			}
			return false;
		}
		case 1:
		{
			if (skillXiData.Sample_S == null || !skillXiData.Sample_S.TryGetValue(indexName, out var value8) || value8 == null)
			{
				return false;
			}
			if (skillXiData.Sample_F != null && skillXiData.Sample_F.TryGetValue(value8.FatherSkill, out var value9))
			{
				return value9 != null;
			}
			return false;
		}
		case 2:
		{
			if (skillXiData.Comp_F != null && skillXiData.Comp_F.TryGetValue(indexName, out var value5))
			{
				return value5 != null;
			}
			return false;
		}
		case 3:
		{
			if (skillXiData.Comp_S == null || !skillXiData.Comp_S.TryGetValue(indexName, out var value3) || value3 == null)
			{
				return false;
			}
			if (skillXiData.Comp_F != null && skillXiData.Comp_F.TryGetValue(value3.FatherSkill, out var value4))
			{
				return value4 != null;
			}
			return false;
		}
		case 4:
		{
			if (skillXiData.Dot_F != null && skillXiData.Dot_F.TryGetValue(indexName, out var value10))
			{
				return value10 != null;
			}
			return false;
		}
		case 5:
		{
			if (skillXiData.Dot_S == null || !skillXiData.Dot_S.TryGetValue(indexName, out var value6) || value6 == null)
			{
				return false;
			}
			if (skillXiData.Dot_F != null && skillXiData.Dot_F.TryGetValue(value6.FatherSkill, out var value7))
			{
				return value7 != null;
			}
			return false;
		}
		case 6:
		{
			if (skillXiData.Bei != null && skillXiData.Bei.TryGetValue(indexName, out var value))
			{
				return value != null;
			}
			return false;
		}
		default:
			return false;
		}
	}

	private static bool HasSkillChangeData(int globalId)
	{
		if (globalId <= 0)
		{
			return false;
		}
		if (!SingletonMonoScope<TalentManager>.HasInstance)
		{
			return true;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance.SKC_Data != null && instance.SKC_Data.Count != 0)
		{
			return instance.SKC_Data.Any((SkilChangeData data) => data != null && data.GlobleID == globalId);
		}
		return true;
	}

	private static bool HasCompSkillChangeData(int globalId)
	{
		if (globalId <= 0)
		{
			return false;
		}
		if (!SingletonMonoScope<TalentManager>.HasInstance)
		{
			return true;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance.CPC_Data != null && instance.CPC_Data.Count != 0)
		{
			return instance.CPC_Data.ContainsKey(globalId);
		}
		return true;
	}

	private static bool HasActiveWeaponSkillSocket(WeaponSaveData weapon)
	{
		if (weapon == null || weapon.WPSK == null || weapon.WP_SkillCount <= 0)
		{
			return false;
		}
		int num = Mathf.Min(weapon.WP_SkillCount, weapon.WPSK.Count);
		for (int i = 0; i < num; i++)
		{
			WPSkillSaveData wPSkillSaveData = weapon.WPSK[i];
			if (wPSkillSaveData != null && !IsEmptyName(wPSkillSaveData.IndexName) && wPSkillSaveData.Number + wPSkillSaveData.Number2 > 0)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsActiveGemSocket(WPAocaoSaveData socket)
	{
		if (socket != null && socket.HasAocao && socket.HasBaoshi)
		{
			return !IsEmptyName(socket.Name);
		}
		return false;
	}

	private static bool IsEmptyName(string value)
	{
		if (!string.IsNullOrWhiteSpace(value))
		{
			return value == "0";
		}
		return true;
	}

	private static bool IsElement(int element)
	{
		if (element >= 0)
		{
			return element < 6;
		}
		return false;
	}

	private static float GetBaseValueMultiplier(WeaponSaveData weapon)
	{
		if (weapon == null)
		{
			return 1f;
		}
		if (weapon.BaseValueMultiplier <= 0f)
		{
			if (!weapon.BaseValueDoubled)
			{
				return 1f;
			}
			return 2f;
		}
		return Mathf.Max(1f, weapon.BaseValueMultiplier);
	}

	private static void ClearWeaponTalentLevels(TalentSaveData data)
	{
		if (data == null || data.All_Skill_Datas == null)
		{
			return;
		}
		foreach (SkillSaveData value in data.All_Skill_Datas.Values)
		{
			if (value != null)
			{
				value.Level_WeaponOn = 0;
			}
		}
	}

	private static void EnsurePlayerDotData(PlayerSaveData player)
	{
		if (player != null)
		{
			player.Dot_Fire = player.Dot_Fire ?? PlayerDotData.CreateDefault();
			player.Dot_Ice = player.Dot_Ice ?? PlayerDotData.CreateDefault();
			player.Dot_TD = player.Dot_TD ?? PlayerDotData.CreateDefault();
			player.Dot_Du = player.Dot_Du ?? PlayerDotData.CreateDefault();
			player.Dot_Phy = player.Dot_Phy ?? PlayerDotData.CreateDefault();
			player.Dot_SD = player.Dot_SD ?? PlayerDotData.CreateDefault();
		}
	}

	private static PlayerDotData GetDotByElement(PlayerSaveData player, int element)
	{
		EnsurePlayerDotData(player);
		return element switch
		{
			0 => player.Dot_Fire, 
			1 => player.Dot_Ice, 
			2 => player.Dot_TD, 
			3 => player.Dot_Du, 
			4 => player.Dot_Phy, 
			5 => player.Dot_SD, 
			_ => null, 
		};
	}

	private static void ClampCorePlayerValues(PlayerSaveData player)
	{
		PlayerSaveData playerSaveData = PlayerSaveData.CreateDefault();
		player.Level = Mathf.Max(1, player.Level);
		player.DFLevel = Mathf.Max(1, player.DFLevel);
		player.Health = Mathf.Max(playerSaveData.Health, player.Health);
		player.Mana = Mathf.Max(playerSaveData.Mana, player.Mana);
		player.Damage_Base = Mathf.Max(playerSaveData.Damage_Base, player.Damage_Base);
		player.EquippedSetCounts = player.EquippedSetCounts ?? new Dictionary<int, int>();
	}

	private static void AddElementDamage(PlayerSaveData player, float fire, float frozen, float thunder, float poison, float physics, float shadow)
	{
		player.FireDamage_Bei += fire;
		player.FrozenDamage_Bei += frozen;
		player.ThunderDamage_Bei += thunder;
		player.PoisonDamage_Bei += poison;
		player.PhysicsDamage_Bei += physics;
		player.ShadowDamage_Bei += shadow;
	}

	private static void AddElementChuan(PlayerSaveData player, float fire, float frozen, float thunder, float poison, float physics, float shadow)
	{
		player.FireChuan += fire;
		player.FrozenChuan += frozen;
		player.ThunderChuan += thunder;
		player.PoisonChuan += poison;
		player.PhysicsChuan += physics;
		player.ShadowChuan += shadow;
	}

	private static void AddElementAnti(PlayerSaveData player, float fire, float frozen, float thunder, float poison, float physics, float shadow)
	{
		player.FireAnti += fire;
		player.FrozenAnti += frozen;
		player.ThunderAnti += thunder;
		player.PoisonAnti += poison;
		player.PhysicsAnti += physics;
		player.ShadowAnti += shadow;
	}

	private static void AddElementFloat(PlayerSaveData player, string[] fields, int element, float value)
	{
		if (IsElement(element) && fields != null && element < fields.Length)
		{
			AddFloat(PlayerFields, player, fields[element], value);
		}
	}

	private static void AddElementInt(PlayerSaveData player, string[] fields, int element, int value)
	{
		if (IsElement(element) && fields != null && element < fields.Length)
		{
			AddInt(PlayerFields, player, fields[element], value);
		}
	}

	private static void AddFloat(Dictionary<string, FieldInfo> fields, object target, string fieldName, float value)
	{
		if (target != null && !string.IsNullOrEmpty(fieldName) && !Mathf.Approximately(value, 0f) && fields.TryGetValue(fieldName, out var value2) && !(value2.FieldType != typeof(float)))
		{
			value2.SetValue(target, (float)value2.GetValue(target) + value);
		}
	}

	private static void AddInt(Dictionary<string, FieldInfo> fields, object target, string fieldName, int value)
	{
		if (target != null && !string.IsNullOrEmpty(fieldName) && value != 0 && fields.TryGetValue(fieldName, out var value2) && !(value2.FieldType != typeof(int)))
		{
			value2.SetValue(target, (int)value2.GetValue(target) + value);
		}
	}

	private static void SetBool(Dictionary<string, FieldInfo> fields, object target, string fieldName, bool value)
	{
		if (target != null && !string.IsNullOrEmpty(fieldName) && fields.TryGetValue(fieldName, out var value2) && !(value2.FieldType != typeof(bool)))
		{
			value2.SetValue(target, value);
		}
	}

	private static Dictionary<string, FieldInfo> BuildFieldMap(Type type)
	{
		Dictionary<string, FieldInfo> dictionary = new Dictionary<string, FieldInfo>(StringComparer.Ordinal);
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			dictionary[fieldInfo.Name] = fieldInfo;
		}
		return dictionary;
	}

	private static void ClearSpc(WPSPC spc)
	{
		if (spc != null)
		{
			spc.Index = 0;
			spc.EL = 0;
			spc.PRC = 0f;
			spc.price = 0;
		}
	}

	private static void ClearSkillSocket(WPSkillSaveData socket)
	{
		if (socket != null)
		{
			socket.IndexName = "0";
			socket.Number = 0;
			socket.Number2 = 0;
			socket.price = 0;
		}
	}

	private static void CloseGemSocket(WPAocaoSaveData socket)
	{
		if (socket != null)
		{
			socket.HasAocao = false;
			ClearGem(socket);
		}
	}

	private static void ClearGem(WPAocaoSaveData socket)
	{
		if (socket != null)
		{
			socket.HasBaoshi = false;
			socket.Name = "";
			socket.Type = 0;
			socket.UseType = 0;
			socket.BS_Quality = 0;
			socket.Number = 0f;
		}
	}
}
