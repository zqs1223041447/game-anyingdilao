using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace PoedbMod;

public static class AffixTierDisplay
{
	private enum RollKind
	{
		Fixed,
		Float,
		Recovery,
		IntGrowth,
		MijingInt
	}

	public const bool EnableDisplay = true;

	private const string ColorTier1 = "#FFD24A";

	private const string ColorOther = "#8F8F8F";

	private const float OverMaxRatio = 1.02f;

	private const int GroupMain = 0;

	private const int GroupDot = 1;

	private const int GroupSkill = 2;

	private const int GroupCompanion = 3;

	private static readonly Dictionary<int, Item_MB> TemplateCache = new Dictionary<int, Item_MB>();

	private static readonly Dictionary<string, float[]> GlobalTierCache = new Dictionary<string, float[]>();

	public static string SuffixA(WeaponClass weapon, WPDT_A stat, bool isDotGroup)
	{
		if (weapon == null || stat == null || stat.Index == 0)
		{
			return string.Empty;
		}
		return BuildSuffix(weapon, isDotGroup ? 1 : 0, stat.Index, stat.number, null);
	}

	public static string SuffixB(WeaponClass weapon, WPDT_B stat, bool isCompanion)
	{
		if (weapon == null || stat == null || stat.Index == 0 || string.IsNullOrEmpty(stat.SkillName))
		{
			return string.Empty;
		}
		return BuildSuffix(weapon, isCompanion ? 3 : 2, stat.Index, stat.number, stat.SkillName);
	}

	private static string BuildSuffix(WeaponClass weapon, int group, int index, float value, string skillName)
	{
		try
		{
			int itemLevel = GetItemLevel(weapon);
			bool isMijing = weapon.DropScene > 0;
			int quality = weapon.Quality;
			RollKind kind = ClassifyRollKind(index, group == 0);
			float integerGrowthMax = GetIntegerGrowthMax(kind, itemLevel, quality, isMijing);
			float[] array = CollectPoolTiers(weapon, group, index, skillName);
			if (array == null || array.Length == 0)
			{
				array = CollectGlobalTiers(group, index, skillName);
				if (array == null || array.Length == 0)
				{
					return string.Empty;
				}
			}
			BuildIntervals(array, kind, itemLevel, quality, integerGrowthMax, isMijing, weapon.DropScene, out var intervalLo, out var intervalHi, out var rangeLo, out var rangeHi);
			if (!ContainsAny(intervalLo, intervalHi, value))
			{
				float[] array2 = CollectFamilyPoolTiers(weapon, group, index, skillName);
				if (array2 != null && array2.Length > 1)
				{
					BuildIntervals(array2, kind, itemLevel, quality, integerGrowthMax, isMijing, weapon.DropScene, out var intervalLo2, out var intervalHi2, out var rangeLo2, out var rangeHi2);
					if (ContainsAny(intervalLo2, intervalHi2, value))
					{
						array = array2;
						intervalLo = intervalLo2;
						intervalHi = intervalHi2;
						rangeLo = rangeLo2;
						rangeHi = rangeHi2;
					}
				}
			}
			if (array.Length == 1 && rangeHi - rangeLo <= Mathf.Epsilon)
			{
				return string.Empty;
			}
			int rank = FindRank(value, array, intervalLo, intervalHi);
			int tier = GetTier(value, rank, array.Length, rangeLo, rangeHi, kind);
			bool isOverMax = value > rangeHi * 1.02f;
			return Format(tier, rangeLo, rangeHi, index, isOverMax);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	private static int GetItemLevel(WeaponClass weapon)
	{
		if (weapon.Level > 0)
		{
			return weapon.Level;
		}
		return GetTemplate(weapon)?.DropLevelStart ?? 1;
	}

	private static ItemManager GetItemManager()
	{
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return null;
		}
		return SingletonMonoScope<ItemManager>.Instance;
	}

	private static Item_MB GetTemplate(WeaponClass weapon)
	{
		int globalID = weapon.GlobalID;
		if (globalID > 0 && TemplateCache.TryGetValue(globalID, out var value))
		{
			return value;
		}
		ItemManager itemManager = GetItemManager();
		Item_MB item_MB = (((UnityEngine.Object)(object)itemManager != (UnityEngine.Object)null) ? itemManager.CraftFindTemplate(weapon) : null);
		if (item_MB != null && globalID > 0)
		{
			TemplateCache[globalID] = item_MB;
		}
		return item_MB;
	}

	private static float[] CollectPoolTiers(WeaponClass weapon, int group, int index, string skillName)
	{
		Item_MB template = GetTemplate(weapon);
		if (template == null)
		{
			return null;
		}
		return group switch
		{
			0 => DistinctDesc(CollectA(template.RateMain, index)), 
			1 => DistinctDesc(CollectA(template.RateDot, index)), 
			2 => DistinctDesc(CollectB(template.RateSK, index, skillName)), 
			3 => DistinctDesc(CollectB(template.RateCP, index, skillName)), 
			_ => null, 
		};
	}

	private static List<float> CollectA(WPDT_A[] pool, int index)
	{
		List<float> list = new List<float>();
		if (pool != null)
		{
			foreach (WPDT_A wPDT_A in pool)
			{
				if (wPDT_A != null && wPDT_A.Index == index)
				{
					list.Add(wPDT_A.number);
				}
			}
		}
		return list;
	}

	private static List<float> CollectB(WPDT_B[] pool, int index, string skillName)
	{
		List<float> list = new List<float>();
		if (pool != null)
		{
			foreach (WPDT_B wPDT_B in pool)
			{
				if (wPDT_B != null && wPDT_B.Index == index && string.Equals(wPDT_B.SkillName, skillName, StringComparison.Ordinal))
				{
					list.Add(wPDT_B.number);
				}
			}
		}
		return list;
	}

	private static float[] CollectGlobalTiers(int group, int index, string skillName)
	{
		string key = group + "|" + index + "|" + (skillName ?? string.Empty);
		if (GlobalTierCache.TryGetValue(key, out var value))
		{
			return value;
		}
		List<float> list = new List<float>();
		ItemManager itemManager = GetItemManager();
		if ((UnityEngine.Object)(object)itemManager != (UnityEngine.Object)null)
		{
			if (group == 0 || group == 1)
			{
				foreach (WPDT_RandomA value2 in ((group == 0) ? itemManager.WP_Main : itemManager.WP_DOT).Values)
				{
					if (value2?.RD == null)
					{
						continue;
					}
					WPDT_A[] rD = value2.RD;
					foreach (WPDT_A wPDT_A in rD)
					{
						if (wPDT_A != null && wPDT_A.Index == index)
						{
							list.Add(wPDT_A.number);
						}
					}
				}
			}
			else
			{
				foreach (WPDT_RandomB value3 in ((group == 2) ? itemManager.WP_SK : itemManager.WP_CP).Values)
				{
					if (value3?.RD == null)
					{
						continue;
					}
					WPDT_B[] rD2 = value3.RD;
					foreach (WPDT_B wPDT_B in rD2)
					{
						if (wPDT_B != null && wPDT_B.Index == index && string.Equals(wPDT_B.SkillName, skillName, StringComparison.Ordinal))
						{
							list.Add(wPDT_B.number);
						}
					}
				}
			}
		}
		float[] array = DistinctDesc(list);
		GlobalTierCache[key] = array;
		return array;
	}

	private static float[] CollectFamilyPoolTiers(WeaponClass weapon, int group, int index, string skillName)
	{
		ItemManager itemManager = GetItemManager();
		if ((UnityEngine.Object)(object)itemManager == (UnityEngine.Object)null || itemManager.Weapon?.GP == null || string.IsNullOrEmpty(weapon.ItemName))
		{
			return null;
		}
		List<float> list = new List<float>();
		try
		{
			for (int i = 0; i < itemManager.Weapon.GP.Length; i++)
			{
				Weapon_Group weapon_Group = itemManager.Weapon.GP[i];
				if (weapon_Group?.QL != null && weapon.CharType >= 0 && weapon.CharType < weapon_Group.QL.Length)
				{
					Quality_Group quality_Group = weapon_Group.QL[weapon.CharType];
					if (quality_Group != null)
					{
						CollectFamilyList(list, quality_Group.Normal, group, index, skillName, weapon.ItemName);
						CollectFamilyList(list, quality_Group.Magic, group, index, skillName, weapon.ItemName);
						CollectFamilyList(list, quality_Group.Rare, group, index, skillName, weapon.ItemName);
						CollectFamilyList(list, quality_Group.Exquisite, group, index, skillName, weapon.ItemName);
						CollectFamilyList(list, quality_Group.Epic, group, index, skillName, weapon.ItemName);
						CollectFamilyList(list, quality_Group.Legendary, group, index, skillName, weapon.ItemName);
						CollectFamilyList(list, quality_Group.Mythical, group, index, skillName, weapon.ItemName);
					}
				}
			}
		}
		catch (Exception)
		{
		}
		if (list.Count == 0)
		{
			return null;
		}
		return DistinctDesc(list);
	}

	private static void CollectFamilyList(List<float> target, List<Item_MB> list, int group, int index, string skillName, string itemName)
	{
		if (list == null)
		{
			return;
		}
		foreach (Item_MB item in list)
		{
			if (item != null && !(item.ItemName != itemName))
			{
				switch (group)
				{
				case 0:
					target.AddRange(CollectA(item.RateMain, index));
					break;
				case 1:
					target.AddRange(CollectA(item.RateDot, index));
					break;
				case 2:
					target.AddRange(CollectB(item.RateSK, index, skillName));
					break;
				case 3:
					target.AddRange(CollectB(item.RateCP, index, skillName));
					break;
				}
			}
		}
	}

	private static float[] DistinctDesc(List<float> values)
	{
		if (values == null || values.Count == 0)
		{
			return null;
		}
		values.Sort((float a, float b) => b.CompareTo(a));
		List<float> list = new List<float>(values.Count);
		foreach (float value in values)
		{
			if (list.Count == 0 || Mathf.Abs(list[list.Count - 1] - value) > 0.0001f)
			{
				list.Add(value);
			}
		}
		return list.ToArray();
	}

	private static RollKind ClassifyRollKind(int index, bool isMainGroup)
	{
		if (isMainGroup && index >= 3 && index <= 6)
		{
			return RollKind.Recovery;
		}
		if (IsWeaponIntegerGrowthIndex(index))
		{
			return RollKind.IntGrowth;
		}
		if (IsMijingExtraIntegerIndex(index))
		{
			return RollKind.MijingInt;
		}
		if (IsWeaponFloatWholeIndex(index) || IsWeaponFloatOneDecimalIndex(index))
		{
			return RollKind.Float;
		}
		return RollKind.Fixed;
	}

	private static float GetIntegerGrowthMax(RollKind kind, int level, int quality, bool isMijing)
	{
		switch (kind)
		{
		case RollKind.MijingInt:
			if (!isMijing || quality < 5)
			{
				return 0f;
			}
			return 3f;
		default:
			return 0f;
		case RollKind.IntGrowth:
			if (isMijing)
			{
				if (quality < 5)
				{
					return 1f;
				}
				return 2f;
			}
			if (level >= 80)
			{
				if (quality < 5)
				{
					return 1f;
				}
				return 2f;
			}
			if (level >= 50)
			{
				return 1f;
			}
			return 0f;
		}
	}

	private static void BuildIntervals(float[] tiers, RollKind kind, int level, int quality, float gMax, bool isMijing, int dropScene, out float[] intervalLo, out float[] intervalHi, out float rangeLo, out float rangeHi)
	{
		intervalLo = new float[tiers.Length];
		intervalHi = new float[tiers.Length];
		rangeLo = float.MaxValue;
		rangeHi = float.MinValue;
		for (int i = 0; i < tiers.Length; i++)
		{
			GetAchievableInterval(kind, tiers[i], level, quality, gMax, isMijing, dropScene, out intervalLo[i], out intervalHi[i]);
			if (intervalLo[i] < rangeLo)
			{
				rangeLo = intervalLo[i];
			}
			if (intervalHi[i] > rangeHi)
			{
				rangeHi = intervalHi[i];
			}
		}
	}

	private static bool ContainsAny(float[] intervalLo, float[] intervalHi, float value)
	{
		float num = 0.01f + Mathf.Abs(value) * 0.005f;
		for (int i = 0; i < intervalLo.Length; i++)
		{
			if (value >= intervalLo[i] - num && value <= intervalHi[i] + num)
			{
				return true;
			}
		}
		return false;
	}

	private static void GetAchievableInterval(RollKind kind, float baseNumber, int level, int quality, float gMax, bool isMijing, int dropScene, out float lo, out float hi)
	{
		switch (kind)
		{
		case RollKind.Recovery:
		{
			float num = Mathf.Pow(1.066f, (float)level);
			lo = baseNumber * num * 0.995f;
			hi = baseNumber * num * 1.005f;
			break;
		}
		case RollKind.Float:
		{
			GetFloatMultiplierBracket(level, isMijing, dropScene, out var lo2, out var hi2);
			lo = baseNumber * lo2;
			hi = baseNumber * hi2;
			break;
		}
		case RollKind.IntGrowth:
		case RollKind.MijingInt:
			lo = Mathf.Floor(baseNumber);
			hi = lo + gMax;
			break;
		default:
			lo = baseNumber;
			hi = baseNumber;
			break;
		}
	}

	private static void GetFloatMultiplierBracket(int level, bool isMijing, int dropScene, out float lo, out float hi)
	{
		if (isMijing)
		{
			switch (Mathf.Clamp(dropScene, 1, 4))
			{
			case 1:
				lo = 1.2f;
				hi = 1.3f;
				break;
			case 2:
				lo = 1.2f;
				hi = 1.4f;
				break;
			case 3:
				lo = 1.3f;
				hi = 1.5f;
				break;
			default:
				lo = 1.4f;
				hi = 1.6f;
				break;
			}
		}
		else if (level < 40)
		{
			lo = 0.9f;
			hi = 1f;
		}
		else if (level < 50)
		{
			lo = 0.9f;
			hi = 1.1f;
		}
		else if (level < 70)
		{
			lo = 1f;
			hi = 1.1f;
		}
		else if (level < 80)
		{
			lo = 1f;
			hi = 1.2f;
		}
		else if (level < 90)
		{
			lo = 1f;
			hi = 1.3f;
		}
		else
		{
			lo = 1.1f;
			hi = 1.3f;
		}
	}

	private static int FindRank(float value, float[] tiers, float[] intervalLo, float[] intervalHi)
	{
		float num = 0.01f + Mathf.Abs(value) * 0.005f;
		int num2 = -1;
		float num3 = float.MaxValue;
		int num4 = 0;
		float num5 = float.MaxValue;
		for (int i = 0; i < tiers.Length; i++)
		{
			float num6 = Mathf.Abs((intervalLo[i] + intervalHi[i]) * 0.5f - value);
			if (num6 < num5)
			{
				num4 = i;
				num5 = num6;
			}
			if (value >= intervalLo[i] - num && value <= intervalHi[i] + num && num6 < num3)
			{
				num2 = i;
				num3 = num6;
			}
		}
		return ((num2 >= 0) ? num2 : num4) + 1;
	}

	private static int GetTier(float value, int rank, int tierCount, float rangeLo, float rangeHi, RollKind kind)
	{
		if (tierCount > 1)
		{
			return Mathf.Clamp(rank, 1, tierCount);
		}
		if (kind == RollKind.Recovery || rangeHi - rangeLo <= Mathf.Epsilon)
		{
			return 1;
		}
		float num = Mathf.Clamp01((value - rangeLo) / (rangeHi - rangeLo));
		if (num >= 0.8f)
		{
			return 1;
		}
		if (num >= 0.6f)
		{
			return 2;
		}
		if (num >= 0.4f)
		{
			return 3;
		}
		if (num >= 0.2f)
		{
			return 4;
		}
		return 5;
	}

	private static string Format(int tier, float lo, float hi, int index, bool isOverMax)
	{
		string arg = ((tier <= 1) ? "#FFD24A" : "#8F8F8F");
		string arg2 = ((tier <= 1 && isOverMax) ? "T1+" : ("T" + tier));
		string arg3 = ItemManager.FormatWeaponStatValue(index, Mathf.Max(0f, lo)) + "-" + ItemManager.FormatWeaponStatValue(index, hi);
		return $" <color={arg}>{arg2} | [{arg3}]</color>";
	}

	private static bool IsWeaponIntegerGrowthIndex(int index)
	{
		switch (index)
		{
		case 302:
		case 1500:
		case 1910:
		case 1911:
		case 1912:
		case 2000:
		case 2101:
		case 2202:
		case 4303:
			return true;
		default:
			return false;
		}
	}

	private static bool IsMijingExtraIntegerIndex(int index)
	{
		switch (index)
		{
		case 80:
		case 3100:
		case 3101:
		case 3102:
		case 3103:
		case 4100:
		case 4200:
			return true;
		default:
			return false;
		}
	}

	private static bool IsWeaponFloatWholeIndex(int index)
	{
		switch (index)
		{
		case 1:
		case 2:
		case 81:
		case 150:
		case 151:
		case 170:
		case 171:
		case 650:
		case 651:
		case 652:
		case 653:
		case 655:
		case 1300:
		case 1502:
		case 1503:
		case 1504:
		case 1505:
		case 1506:
		case 1507:
		case 1508:
		case 1509:
		case 1510:
		case 1808:
		case 1815:
		case 1817:
		case 1818:
		case 1819:
		case 2401:
		case 2450:
		case 2501:
		case 2550:
		case 2551:
		case 2552:
		case 3403:
		case 3404:
		case 3530:
		case 3535:
			return true;
		default:
			if ((index >= 10 && index <= 22) || (index >= 30 && index <= 32) || (index >= 50 && index <= 54) || (index >= 100 && index <= 104) || (index >= 300 && index <= 301) || (index >= 303 && index <= 306) || (index >= 500 && index <= 507) || (index >= 509 && index <= 514) || (index >= 550 && index <= 559) || (index >= 610 && index <= 618) || (index >= 800 && index <= 808) || (index >= 850 && index <= 852) || (index >= 855 && index <= 861) || (index >= 1250 && index <= 1253) || (index >= 1270 && index <= 1276) || (index >= 1370 && index <= 1374) || (index >= 1395 && index <= 1397) || (index >= 1600 && index <= 1603) || (index >= 1950 && index <= 1955) || (index >= 3550 && index <= 3561) || (index >= 4400 && index <= 4417))
			{
				return true;
			}
			switch (index)
			{
			case 1260:
			case 1275:
			case 2300:
			case 2303:
			case 2305:
			case 2306:
			case 2600:
			case 2601:
			case 2602:
			case 2603:
			case 3301:
			case 3302:
			case 3303:
			case 3304:
			case 3306:
			case 3307:
			case 3500:
			case 3501:
			case 3502:
			case 3503:
			case 3504:
			case 4301:
			case 4302:
			case 4306:
			case 4308:
				return true;
			default:
				return false;
			}
		}
	}

	private static bool IsWeaponFloatOneDecimalIndex(int index)
	{
		switch (index)
		{
		case 556:
		case 557:
			return true;
		default:
			if (index >= 600 && index <= 604)
			{
				break;
			}
			switch (index)
			{
			default:
				if (index >= 1020 && index <= 1031)
				{
					break;
				}
				switch (index)
				{
				default:
					if (index >= 1100 && index <= 1146)
					{
						break;
					}
					switch (index)
					{
					default:
						if (index != 1802 && index != 1806 && index != 2203 && index != 2402 && index != 2500 && index != 3305 && index != 4307)
						{
							return false;
						}
						break;
					case 1150:
					case 1200:
					case 1201:
					case 1202:
					case 1203:
					case 1204:
					case 1205:
					case 1206:
						break;
					}
					break;
				case 1010:
				case 1011:
				case 1040:
				case 1041:
				case 1050:
				case 1051:
				case 1052:
				case 1053:
				case 1054:
					break;
				}
				break;
			case 60:
			case 61:
			case 62:
			case 63:
			case 700:
			case 701:
			case 1000:
			case 1001:
			case 1002:
			case 1003:
			case 1004:
			case 1005:
			case 1006:
			case 1007:
				break;
			}
			break;
		case 400:
		case 401:
		case 402:
		case 403:
		case 404:
		case 405:
		case 406:
		case 407:
		case 408:
		case 409:
		case 410:
		case 411:
		case 412:
		case 413:
		case 414:
		case 415:
		case 416:
		case 417:
		case 418:
		case 419:
		case 420:
		case 421:
		case 422:
		case 423:
		case 424:
		case 425:
		case 426:
		case 427:
		case 428:
		case 429:
		case 430:
		case 431:
		case 432:
		case 433:
		case 434:
		case 435:
		case 436:
		case 437:
		case 438:
		case 439:
		case 440:
		case 441:
		case 442:
		case 443:
		case 444:
		case 445:
		case 446:
		case 447:
		case 448:
		case 449:
		case 450:
		case 451:
		case 452:
		case 453:
		case 454:
		case 455:
		case 456:
		case 457:
		case 458:
		case 459:
		case 460:
		case 461:
		case 462:
		case 463:
		case 464:
			break;
		}
		return true;
	}
}
