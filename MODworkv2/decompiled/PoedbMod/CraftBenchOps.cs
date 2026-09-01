using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace PoedbMod;

public static class CraftBenchOps
{
	public enum Op
	{
		Transmute,
		Augment,
		Alteration,
		Regal,
		Alchemy,
		AlchemyExquisite,
		AlchemyEpic,
		LegendaryStone,
		MythicStone,
		Chaos,
		HiddenChaos,
		Exalted,
		Annulment,
		Divine,
		Scouring,
		BestiaryPreToSuf,
		BestiarySufToPre
	}

	private struct CraftRemoveRef
	{
		public int Kind;

		public int Index;

		public string Name;
	}

	public const long CraftPrice = 1L;

	public static readonly string[] LockNames = new string[4] { "前缀无法被变更", "后缀无法被变更", "无法骰出攻击词缀", "无法骰出法术词缀" };

	private const int TagPrefix = 1;

	private const int TagSuffix = 2;

	private const int TagAttack = 4;

	private const int TagCaster = 8;

	private static ItemManager IM => SingletonMonoScope<ItemManager>.Instance;

	public static string QualityName(int quality)
	{
		return Mathf.Clamp(quality, 0, 6) switch
		{
			0 => "普通", 
			1 => "魔法", 
			2 => "稀有", 
			3 => "精致", 
			4 => "史诗", 
			5 => "传说", 
			_ => "神话", 
		};
	}

	public static bool CanUse(Op op, WeaponClass w)
	{
		if (w == null)
		{
			return false;
		}
		switch (op)
		{
		case Op.Transmute:
		case Op.Alchemy:
		case Op.AlchemyExquisite:
		case Op.AlchemyEpic:
		case Op.LegendaryStone:
		case Op.MythicStone:
			return w.Quality == 0;
		case Op.Augment:
		case Op.Alteration:
		case Op.Regal:
			return w.Quality == 1;
		case Op.Chaos:
		case Op.HiddenChaos:
		case Op.Exalted:
			return w.Quality >= 2;
		case Op.Annulment:
		case Op.Divine:
		case Op.Scouring:
		case Op.BestiaryPreToSuf:
		case Op.BestiarySufToPre:
			return w.Quality >= 1;
		default:
			return false;
		}
	}

	public static bool ToggleLock(WeaponClass w, int lockId, out string msg)
	{
		msg = null;
		if (w == null)
		{
			msg = "未选择装备";
			return false;
		}
		if (w.Quality < 1)
		{
			msg = "普通装备无法附加工艺限制（需魔法及以上）";
			return false;
		}
		if (lockId < 0 || lockId >= LockNames.Length)
		{
			msg = "未知的工艺限制";
			return false;
		}
		bool flag;
		switch (lockId)
		{
		case 0:
			w.Craft_LockPrefix = !w.Craft_LockPrefix;
			flag = w.Craft_LockPrefix;
			break;
		case 1:
			w.Craft_LockSuffix = !w.Craft_LockSuffix;
			flag = w.Craft_LockSuffix;
			break;
		case 2:
			w.Craft_NoAttack = !w.Craft_NoAttack;
			flag = w.Craft_NoAttack;
			break;
		default:
			w.Craft_NoCaster = !w.Craft_NoCaster;
			flag = w.Craft_NoCaster;
			break;
		}
		msg = string.Format("{0}：<color={1}>{2}</color>", LockNames[lockId], flag ? "#00FF00" : "#AAAAAA", flag ? "已附加" : "已解除");
		return true;
	}

	public static bool Execute(Op op, WeaponClass w, out string msg)
	{
		msg = null;
		if (w == null)
		{
			msg = "未选择装备";
			return false;
		}
		if (!CanUse(op, w))
		{
			msg = "当前装备品质不满足该工艺的条件";
			return false;
		}
		try
		{
			switch (op)
			{
			case Op.Transmute:
				return ExecTransmute(w, out msg);
			case Op.Augment:
				return ExecAugment(w, out msg);
			case Op.Alteration:
				return ExecAlteration(w, out msg);
			case Op.Regal:
				return ExecRegal(w, out msg);
			case Op.Alchemy:
				return ExecAlchemy(w, 2, "点金", out msg);
			case Op.AlchemyExquisite:
				return ExecAlchemy(w, 3, "点金·精致", out msg);
			case Op.AlchemyEpic:
				return ExecAlchemy(w, 4, "点金·史诗", out msg);
			case Op.LegendaryStone:
				return ExecAlchemy(w, 5, "传说石", out msg);
			case Op.MythicStone:
				return ExecAlchemy(w, 6, "神话石", out msg);
			case Op.Chaos:
				return ExecRerollRare(w, ignoreAttackCasterLocks: false, "混沌", out msg);
			case Op.HiddenChaos:
				return ExecRerollRare(w, ignoreAttackCasterLocks: true, "隐匿混沌", out msg);
			case Op.Exalted:
				return ExecExalted(w, out msg);
			case Op.Annulment:
				return ExecAnnulment(w, out msg);
			case Op.Divine:
				return ExecDivine(w, out msg);
			case Op.Scouring:
				return ExecScouring(w, out msg);
			case Op.BestiaryPreToSuf:
				return ExecBestiary(w, removePrefix: true, out msg);
			case Op.BestiarySufToPre:
				return ExecBestiary(w, removePrefix: false, out msg);
			default:
				msg = "未知工艺";
				return false;
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CraftBench] op " + op.ToString() + " failed: " + ex);
			msg = "工艺执行异常，已中止（详情见 Player.log）";
			return false;
		}
	}

	private static bool ExecTransmute(WeaponClass w, out string msg)
	{
		Item_MB tpl = PickPool(w, 1);
		w.Quality = 1;
		if (!TryAddAffix(w, tpl, allowMain: true, allowDot: true, allowSkill: true, allowComp: true, allowSpc: true))
		{
			w.Quality = 0;
			msg = "蜕变失败：没有可用的魔法词缀池";
			return false;
		}
		msg = "蜕变成功：普通 → 魔法（新增 1 条词缀）";
		return true;
	}

	private static bool ExecAugment(WeaponClass w, out string msg)
	{
		if (CountAffixLines(w) >= AffixCap(w.Quality))
		{
			msg = "增幅失败：词缀数量已达上限";
			return false;
		}
		Item_MB tpl = PickPool(w, w.Quality);
		if (!TryAddAffix(w, tpl, !w.Craft_NoAttack, !w.Craft_NoCaster, !w.Craft_NoAttack, !w.Craft_NoCaster, allowSpc: true))
		{
			msg = "增幅失败：没有可新增的词缀（可能被攻击/法术限制阻挡）";
			return false;
		}
		msg = "增幅成功：新增 1 条词缀";
		return true;
	}

	private static bool ExecAlteration(WeaponClass w, out string msg)
	{
		Item_MB tpl = PickPool(w, w.Quality);
		bool craft_LockPrefix = w.Craft_LockPrefix;
		bool craft_LockSuffix = w.Craft_LockSuffix;
		bool flag = IsArmorOrAccessory(w);
		if (!RerollAffixes(w, tpl, !craft_LockPrefix, !craft_LockPrefix, !craft_LockPrefix, !craft_LockSuffix, !craft_LockSuffix, flag ? (!craft_LockSuffix) : (!craft_LockPrefix), blockAttack: false, blockCaster: false))
		{
			msg = "改造失败：没有可重骰的词缀";
			return false;
		}
		msg = "改造成功：已重骰全部未锁定的词缀";
		return true;
	}

	private static bool ExecRegal(WeaponClass w, out string msg)
	{
		if (CountAffixLines(w) >= AffixCap(2))
		{
			msg = "富豪失败：词缀数量已达上限";
			return false;
		}
		Item_MB tpl = PickPool(w, 2);
		w.Quality = 2;
		if (!TryAddAffix(w, tpl, !w.Craft_NoAttack, !w.Craft_NoCaster, !w.Craft_NoAttack, !w.Craft_NoCaster, allowSpc: true))
		{
			w.Quality = 1;
			msg = "富豪失败：没有可新增的词缀（可能被攻击/法术限制阻挡）";
			return false;
		}
		msg = "富豪成功：魔法 → 稀有（新增 1 条词缀）";
		return true;
	}

	private static bool ExecAlchemy(WeaponClass w, int targetQuality, string opName, out string msg)
	{
		Item_MB item_MB = PickPool(w, targetQuality);
		if (item_MB == null)
		{
			msg = opName + "失败：找不到该部位在目标品质档的词缀池";
			return false;
		}
		w.Quality = targetQuality;
		int num = UnityEngine.Random.Range(4, AffixCap(targetQuality) + 1);
		int i;
		for (i = 0; i < num; i++)
		{
			if (CountAffixLines(w) >= AffixCap(targetQuality))
			{
				break;
			}
			if (!TryAddAffix(w, item_MB, !w.Craft_NoAttack, !w.Craft_NoCaster, !w.Craft_NoAttack, !w.Craft_NoCaster, allowSpc: true))
			{
				break;
			}
		}
		if (i == 0)
		{
			w.Quality = 0;
			msg = opName + "失败：没有可用的目标品质词缀池";
			return false;
		}
		msg = opName + "成功：普通 → " + QualityName(targetQuality) + "（新增 " + i + " 条词缀）";
		return true;
	}

	private static bool ExecRerollRare(WeaponClass w, bool ignoreAttackCasterLocks, string opName, out string msg)
	{
		Item_MB tpl = PickPool(w, w.Quality);
		bool craft_LockPrefix = w.Craft_LockPrefix;
		bool craft_LockSuffix = w.Craft_LockSuffix;
		bool flag = IsArmorOrAccessory(w);
		bool blockAttack = !ignoreAttackCasterLocks && w.Craft_NoAttack;
		bool blockCaster = !ignoreAttackCasterLocks && w.Craft_NoCaster;
		if (!RerollAffixes(w, tpl, !craft_LockPrefix, !craft_LockPrefix, !craft_LockPrefix, !craft_LockSuffix, !craft_LockSuffix, flag ? (!craft_LockSuffix) : (!craft_LockPrefix), blockAttack, blockCaster))
		{
			msg = opName + "失败：没有可重骰的词缀";
			return false;
		}
		msg = opName + "成功：已重骰全部词缀" + (ignoreAttackCasterLocks ? "" : "（锁定组与禁骰规则已生效）");
		return true;
	}

	private static bool ExecExalted(WeaponClass w, out string msg)
	{
		if (CountAffixLines(w) >= AffixCap(w.Quality))
		{
			msg = "崇高失败：词缀数量已达上限";
			return false;
		}
		Item_MB tpl = PickPool(w, w.Quality);
		if (!TryAddAffix(w, tpl, !w.Craft_NoAttack, !w.Craft_NoCaster, !w.Craft_NoAttack, !w.Craft_NoCaster, allowSpc: true))
		{
			msg = "崇高失败：没有可新增的词缀（可能被攻击/法术限制阻挡）";
			return false;
		}
		msg = "崇高成功：新增 1 条词缀";
		return true;
	}

	private static bool ExecAnnulment(WeaponClass w, out string msg)
	{
		if (!TryRemoveAffix(w, !w.Craft_NoAttack, !w.Craft_NoCaster, w.Craft_LockPrefix, w.Craft_LockSuffix, 0, out var removedName))
		{
			msg = "无效失败：没有可移除的词条（全部被工艺限制保护）";
			return false;
		}
		msg = "无效成功：移除了 <color=#FF7070>" + removedName + "</color>";
		return true;
	}

	private static bool ExecDivine(WeaponClass w, out string msg)
	{
		Item_MB item_MB = PickPool(w, w.Quality);
		if (item_MB == null)
		{
			msg = "神圣失败：找不到该装备的模板词缀池";
			return false;
		}
		bool craft_LockPrefix = w.Craft_LockPrefix;
		bool craft_LockSuffix = w.Craft_LockSuffix;
		bool flag = IsArmorOrAccessory(w);
		SingletonMonoScope<ItemManager>.Instance.CraftRerollStatValues(w, item_MB, Mathf.Max(1, w.Level), SingletonMonoScope<ItemManager>.Instance.CraftGetDropContext(w), !craft_LockPrefix, !craft_LockPrefix, !craft_LockPrefix, !craft_LockSuffix, !craft_LockSuffix, flag ? (!craft_LockSuffix) : (!craft_LockPrefix));
		msg = "神圣成功：已重骰全部未锁定词条的数值";
		return true;
	}

	private static bool ExecScouring(WeaponClass w, out string msg)
	{
		bool craft_LockPrefix = w.Craft_LockPrefix;
		bool craft_LockSuffix = w.Craft_LockSuffix;
		if (!craft_LockPrefix)
		{
			w.Main = new WPDT_A[0];
			w.DOT = new WPDT_A[0];
			w.SK = new WPDT_B[0];
		}
		if (!craft_LockSuffix)
		{
			w.CP = new WPDT_B[0];
		}
		if (IsArmorOrAccessory(w))
		{
			if (!craft_LockSuffix)
			{
				ClearElements(w);
			}
		}
		else if (!craft_LockPrefix)
		{
			ClearElements(w);
		}
		if (!craft_LockSuffix)
		{
			ClearSpcSlots(w);
		}
		bool flag = (craft_LockPrefix && HasAnyLineOfTag(w, 1)) || (craft_LockSuffix && HasAnyLineOfTag(w, 2));
		if (!flag)
		{
			w.Quality = 0;
		}
		w.Craft_LockPrefix = false;
		w.Craft_LockSuffix = false;
		w.Craft_NoAttack = false;
		w.Craft_NoCaster = false;
		msg = (flag ? "重铸完成：已清空未锁定词条，锁定组保留" : "重铸完成：全部词条已清空，装备回到普通品质");
		return true;
	}

	private static bool ExecBestiary(WeaponClass w, bool removePrefix, out string msg)
	{
		string text = (removePrefix ? "移前增后" : "移后增前");
		if (removePrefix ? w.Craft_LockSuffix : w.Craft_LockPrefix)
		{
			msg = text + "失败：新增方向的词缀组被工艺限制锁定";
			return false;
		}
		int wantTagMask = (removePrefix ? 1 : 2);
		if (!TryRemoveAffix(w, !w.Craft_NoAttack, !w.Craft_NoCaster, w.Craft_LockPrefix, w.Craft_LockSuffix, wantTagMask, out var removedName))
		{
			msg = text + "失败：没有可移除的" + (removePrefix ? "前缀" : "后缀") + "词条";
			return false;
		}
		Item_MB tpl = PickPool(w, w.Quality);
		bool flag = !removePrefix && !w.Craft_NoAttack;
		bool allowDot = !removePrefix && !w.Craft_NoCaster;
		bool allowComp = removePrefix && !w.Craft_NoCaster;
		if (TryAddAffix(w, tpl, flag, allowDot, flag, allowComp, removePrefix))
		{
			msg = text + "成功：移除 <color=#FF7070>" + removedName + "</color>，新增 1 条" + (removePrefix ? "后缀" : "前缀") + "词缀";
			return true;
		}
		msg = text + "完成：移除了 <color=#FF7070>" + removedName + "</color>，但没有可新增的词缀池";
		return true;
	}

	private static Item_MB PickPool(WeaponClass w, int quality)
	{
		ItemManager iM = IM;
		Item_MB item_MB = ((iM != null) ? iM.CraftPickPoolTemplate(w, quality) : null);
		if (item_MB == null)
		{
			ItemManager iM2 = IM;
			item_MB = ((iM2 != null) ? iM2.CraftFindTemplate(w) : null);
		}
		return item_MB;
	}

	public static int AffixCap(int quality)
	{
		return quality switch
		{
			1 => 4, 
			2 => 6, 
			3 => 7, 
			4 => 8, 
			5 => 9, 
			6 => 10, 
			_ => 0, 
		};
	}

	public static int CountAffixLines(WeaponClass w)
	{
		if (w == null)
		{
			return 0;
		}
		int num = 0;
		if (w.Main != null)
		{
			for (int i = 0; i < w.Main.Length; i++)
			{
				if (w.Main[i] != null && w.Main[i].Index != 0)
				{
					num++;
				}
			}
		}
		if (w.DOT != null)
		{
			for (int j = 0; j < w.DOT.Length; j++)
			{
				if (w.DOT[j] != null && w.DOT[j].Index != 0)
				{
					num++;
				}
			}
		}
		if (w.SK != null)
		{
			for (int k = 0; k < w.SK.Length; k++)
			{
				if (w.SK[k] != null && !ItemManager.CraftIsNoneSkill(w.SK[k].SkillName))
				{
					num++;
				}
			}
		}
		if (w.CP != null)
		{
			for (int l = 0; l < w.CP.Length; l++)
			{
				if (w.CP[l] != null && !ItemManager.CraftIsNoneSkill(w.CP[l].SkillName))
				{
					num++;
				}
			}
		}
		if (w.SPC != null)
		{
			for (int m = 0; m < w.SPC.Count; m++)
			{
				if (w.SPC[m] != null && w.SPC[m].Index != 0)
				{
					num++;
				}
			}
		}
		if (HasAnyElement(w))
		{
			num++;
		}
		return num;
	}

	private static bool IsArmorOrAccessory(WeaponClass w)
	{
		if (w == null)
		{
			return false;
		}
		if (w.CharType >= 2 && w.CharType <= 9)
		{
			return true;
		}
		string weaponType = w.WeaponType;
		switch (weaponType)
		{
		case "head":
		case "body":
		case "hand":
		case "leg":
			return true;
		default:
			return weaponType == "little";
		}
	}

	private static bool HasFreeSpcSlot(WeaponClass w)
	{
		return FreeSpcSlot(w) >= 0;
	}

	private static int FreeSpcSlot(WeaponClass w)
	{
		if (w == null)
		{
			return -1;
		}
		int num = ((w.SPC != null) ? w.SPC.Count : 0);
		int num2 = Mathf.Min(num, 2);
		for (int i = 0; i < num2; i++)
		{
			if (w.SPC[i] == null || w.SPC[i].Index == 0)
			{
				return i;
			}
		}
		if (num < 2)
		{
			return num;
		}
		return -1;
	}

	private static HashSet<int> CollectAIndexes(WPDT_A[] arr)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (arr != null)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] != null && arr[i].Index != 0)
				{
					hashSet.Add(arr[i].Index);
				}
			}
		}
		return hashSet;
	}

	private static HashSet<string> CollectBKeys(WPDT_B[] arr)
	{
		HashSet<string> hashSet = new HashSet<string>();
		if (arr != null)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] != null && !ItemManager.CraftIsNoneSkill(arr[i].SkillName))
				{
					hashSet.Add(ItemManager.CraftSkillEffectKey(arr[i]));
				}
			}
		}
		return hashSet;
	}

	private static void AppendA(WeaponClass w, bool mainGroup, WPDT_A entry)
	{
		WPDT_A[] array = (mainGroup ? w.Main : w.DOT);
		int num = ((array != null) ? array.Length : 0);
		WPDT_A[] array2 = new WPDT_A[num + 1];
		if (num > 0)
		{
			Array.Copy(array, array2, num);
		}
		array2[num] = entry;
		if (mainGroup)
		{
			w.Main = array2;
		}
		else
		{
			w.DOT = array2;
		}
	}

	private static void AppendB(WeaponClass w, bool skillGroup, WPDT_B entry)
	{
		WPDT_B[] array = (skillGroup ? w.SK : w.CP);
		int num = ((array != null) ? array.Length : 0);
		WPDT_B[] array2 = new WPDT_B[num + 1];
		if (num > 0)
		{
			Array.Copy(array, array2, num);
		}
		array2[num] = entry;
		if (skillGroup)
		{
			w.SK = array2;
		}
		else
		{
			w.CP = array2;
		}
	}

	private static bool TryAddAffix(WeaponClass w, Item_MB tpl, bool allowMain, bool allowDot, bool allowSkill, bool allowComp, bool allowSpc)
	{
		if (w == null || tpl == null)
		{
			return false;
		}
		int level = Mathf.Max(1, w.Level);
		ItemManager.WeaponDropContext ctx = IM.CraftGetDropContext(w);
		List<int> list = new List<int>();
		if (allowMain)
		{
			list.Add(0);
		}
		if (allowDot)
		{
			list.Add(1);
		}
		if (allowSkill)
		{
			list.Add(2);
		}
		if (allowComp)
		{
			list.Add(3);
		}
		if (allowSpc)
		{
			list.Add(4);
		}
		Shuffle(list);
		HashSet<int> excludeIndex = CollectAIndexes(w.Main);
		HashSet<int> excludeIndex2 = CollectAIndexes(w.DOT);
		HashSet<string> excludeKeys = CollectBKeys(w.SK);
		HashSet<string> excludeKeys2 = CollectBKeys(w.CP);
		ItemManager iM = IM;
		for (int i = 0; i < list.Count; i++)
		{
			switch (list[i])
			{
			case 0:
			{
				WPDT_A wPDT_A2 = iM.CraftRollEntryA(tpl.RateMain, level, w.Quality, ctx, isMainGroup: true, excludeIndex);
				if (wPDT_A2 != null)
				{
					AppendA(w, mainGroup: true, wPDT_A2);
					return true;
				}
				break;
			}
			case 1:
			{
				WPDT_A wPDT_A = iM.CraftRollEntryA(tpl.RateDot, level, w.Quality, ctx, isMainGroup: false, excludeIndex2);
				if (wPDT_A != null)
				{
					AppendA(w, mainGroup: false, wPDT_A);
					return true;
				}
				break;
			}
			case 2:
			{
				WPDT_B wPDT_B = iM.CraftRollEntryB(tpl.RateSK, level, w.Quality, ctx, isSkillGroup: true, excludeKeys);
				if (wPDT_B != null)
				{
					AppendB(w, skillGroup: true, wPDT_B);
					return true;
				}
				break;
			}
			case 3:
			{
				WPDT_B wPDT_B2 = iM.CraftRollEntryB(tpl.RateCP, level, w.Quality, ctx, isSkillGroup: false, excludeKeys2);
				if (wPDT_B2 != null)
				{
					AppendB(w, skillGroup: false, wPDT_B2);
					return true;
				}
				break;
			}
			case 4:
			{
				int num = FreeSpcSlot(w);
				if (num >= 0 && iM.CraftHasSpcPool(tpl))
				{
					WPSPC wPSPC = iM.CraftRollSPC(tpl, level, w.Quality, ctx);
					if (wPSPC != null)
					{
						w.EnsureSPCSlot(num);
						w.SetSPCData(num, wPSPC.Index, wPSPC.EL, wPSPC.PRC);
						return true;
					}
				}
				break;
			}
			}
		}
		return false;
	}

	private static bool TryRemoveAffix(WeaponClass w, bool canRemoveAttack, bool canRemoveCaster, bool prefixLocked, bool suffixLocked, int wantTagMask, out string removedName)
	{
		removedName = null;
		if (w == null)
		{
			return false;
		}
		List<CraftRemoveRef> list = new List<CraftRemoveRef>();
		if (w.Main != null)
		{
			for (int i = 0; i < w.Main.Length; i++)
			{
				if (w.Main[i] != null && w.Main[i].Index != 0)
				{
					Collect(0, i, 5, "主属性词条");
				}
			}
		}
		if (w.DOT != null)
		{
			for (int j = 0; j < w.DOT.Length; j++)
			{
				if (w.DOT[j] != null && w.DOT[j].Index != 0)
				{
					Collect(1, j, 9, "持续词条");
				}
			}
		}
		if (w.SK != null)
		{
			for (int k = 0; k < w.SK.Length; k++)
			{
				if (w.SK[k] != null && !ItemManager.CraftIsNoneSkill(w.SK[k].SkillName))
				{
					Collect(2, k, 5, "技能词条");
				}
			}
		}
		if (w.CP != null)
		{
			for (int l = 0; l < w.CP.Length; l++)
			{
				if (w.CP[l] != null && !ItemManager.CraftIsNoneSkill(w.CP[l].SkillName))
				{
					Collect(3, l, 10, "同伴词条");
				}
			}
		}
		if (w.SPC != null)
		{
			for (int m = 0; m < w.SPC.Count; m++)
			{
				if (w.SPC[m] != null && w.SPC[m].Index != 0)
				{
					Collect(4, m, 2, "特效词条");
				}
			}
		}
		int tag2 = (IsArmorOrAccessory(w) ? 2 : 5);
		if (w.Fire > 0f)
		{
			Collect(5, 0, tag2, "元素词条");
		}
		if (w.Frozen > 0f)
		{
			Collect(5, 1, tag2, "元素词条");
		}
		if (w.Thunder > 0f)
		{
			Collect(5, 2, tag2, "元素词条");
		}
		if (w.Poison > 0f)
		{
			Collect(5, 3, tag2, "元素词条");
		}
		if (w.Physics > 0f)
		{
			Collect(5, 4, tag2, "元素词条");
		}
		if (w.Shadow > 0f)
		{
			Collect(5, 5, tag2, "元素词条");
		}
		if (list.Count == 0)
		{
			return false;
		}
		CraftRemoveRef craftRemoveRef = list[UnityEngine.Random.Range(0, list.Count)];
		RemoveAt(w, craftRemoveRef.Kind, craftRemoveRef.Index);
		removedName = craftRemoveRef.Name;
		return true;
		void Collect(int kind, int index, int tag, string name)
		{
			if ((wantTagMask == 0 || (tag & wantTagMask) != 0) && (!prefixLocked || (tag & 1) == 0) && (!suffixLocked || (tag & 2) == 0) && (canRemoveAttack || (tag & 4) == 0) && (canRemoveCaster || (tag & 8) == 0))
			{
				list.Add(new CraftRemoveRef
				{
					Kind = kind,
					Index = index,
					Name = name
				});
			}
		}
	}

	private static void RemoveAt(WeaponClass w, int kind, int index)
	{
		switch (kind)
		{
		case 0:
			w.Main = RemoveArrayAt(w.Main, index);
			break;
		case 1:
			w.DOT = RemoveArrayAt(w.DOT, index);
			break;
		case 2:
			w.SK = RemoveArrayAt(w.SK, index);
			break;
		case 3:
			w.CP = RemoveArrayAt(w.CP, index);
			break;
		case 4:
			w.SetSPCData(index, 0, 0, 0f);
			break;
		default:
			SetElement(w, index, 0f);
			break;
		}
	}

	private static T[] RemoveArrayAt<T>(T[] arr, int index) where T : class
	{
		if (arr == null || index < 0 || index >= arr.Length)
		{
			return arr;
		}
		T[] array = new T[arr.Length - 1];
		int num = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			if (i != index)
			{
				array[num++] = arr[i];
			}
		}
		return array;
	}

	private static bool RerollAffixes(WeaponClass w, Item_MB tpl, bool rerollMain, bool rerollDot, bool rerollSkill, bool rerollComp, bool rerollSpc, bool rerollElement, bool blockAttack, bool blockCaster)
	{
		if (w == null)
		{
			return false;
		}
		int level = Mathf.Max(1, w.Level);
		ItemManager.WeaponDropContext ctx = IM.CraftGetDropContext(w);
		bool result = false;
		if (rerollMain)
		{
			int num = ((w.Main != null) ? CountA(w.Main) : 0);
			if (blockAttack)
			{
				if (num > 0)
				{
					w.Main = new WPDT_A[0];
					result = true;
				}
			}
			else if (num > 0 && tpl != null)
			{
				w.Main = RollGroupA(tpl.RateMain, level, w, num, ctx, isMainGroup: true);
				result = true;
			}
		}
		if (rerollDot)
		{
			int num2 = ((w.DOT != null) ? CountA(w.DOT) : 0);
			if (blockCaster)
			{
				if (num2 > 0)
				{
					w.DOT = new WPDT_A[0];
					result = true;
				}
			}
			else if (num2 > 0 && tpl != null)
			{
				w.DOT = RollGroupA(tpl.RateDot, level, w, num2, ctx, isMainGroup: false);
				result = true;
			}
		}
		if (rerollSkill)
		{
			int num3 = ((w.SK != null) ? CountB(w.SK) : 0);
			if (blockAttack)
			{
				if (num3 > 0)
				{
					w.SK = new WPDT_B[0];
					result = true;
				}
			}
			else if (num3 > 0 && tpl != null)
			{
				w.SK = RollGroupB(tpl.RateSK, level, w, num3, ctx, isSkillGroup: true);
				result = true;
			}
		}
		if (rerollComp)
		{
			int num4 = ((w.CP != null) ? CountB(w.CP) : 0);
			if (blockCaster)
			{
				if (num4 > 0)
				{
					w.CP = new WPDT_B[0];
					result = true;
				}
			}
			else if (num4 > 0 && tpl != null)
			{
				w.CP = RollGroupB(tpl.RateCP, level, w, num4, ctx, isSkillGroup: false);
				result = true;
			}
		}
		if (rerollSpc && w.SPC != null && tpl != null)
		{
			for (int i = 0; i < w.SPC.Count && i < 2; i++)
			{
				if (w.SPC[i] != null && w.SPC[i].Index != 0)
				{
					WPSPC wPSPC = IM.CraftRollSPC(tpl, level, w.Quality, ctx);
					if (wPSPC != null)
					{
						w.SetSPCData(i, wPSPC.Index, wPSPC.EL, wPSPC.PRC);
						result = true;
					}
				}
			}
		}
		if (rerollElement)
		{
			if (blockAttack && !IsArmorOrAccessory(w))
			{
				if (HasAnyElement(w))
				{
					ClearElements(w);
					result = true;
				}
			}
			else if (tpl != null)
			{
				IM.CraftRerollElement(w, tpl.Element, level, ctx);
				result = true;
			}
		}
		return result;
	}

	private static WPDT_A[] RollGroupA(WPDT_A[] pool, int level, WeaponClass w, int count, ItemManager.WeaponDropContext ctx, bool isMainGroup)
	{
		if (pool == null || pool.Length == 0 || count <= 0)
		{
			return null;
		}
		HashSet<int> hashSet = new HashSet<int>();
		List<WPDT_A> list = new List<WPDT_A>();
		ItemManager iM = IM;
		for (int i = 0; i < count; i++)
		{
			WPDT_A wPDT_A = iM.CraftRollEntryA(pool, level, w.Quality, ctx, isMainGroup, hashSet);
			if (wPDT_A == null)
			{
				break;
			}
			hashSet.Add(wPDT_A.Index);
			list.Add(wPDT_A);
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list.ToArray();
	}

	private static WPDT_B[] RollGroupB(WPDT_B[] pool, int level, WeaponClass w, int count, ItemManager.WeaponDropContext ctx, bool isSkillGroup)
	{
		if (pool == null || pool.Length == 0 || count <= 0)
		{
			return null;
		}
		HashSet<string> hashSet = new HashSet<string>();
		List<WPDT_B> list = new List<WPDT_B>();
		ItemManager iM = IM;
		for (int i = 0; i < count; i++)
		{
			WPDT_B wPDT_B = iM.CraftRollEntryB(pool, level, w.Quality, ctx, isSkillGroup, hashSet);
			if (wPDT_B == null)
			{
				break;
			}
			hashSet.Add(ItemManager.CraftSkillEffectKey(wPDT_B));
			list.Add(wPDT_B);
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list.ToArray();
	}

	private static int CountA(WPDT_A[] arr)
	{
		if (arr == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			if (arr[i] != null && arr[i].Index != 0)
			{
				num++;
			}
		}
		return num;
	}

	private static int CountB(WPDT_B[] arr)
	{
		if (arr == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			if (arr[i] != null && !ItemManager.CraftIsNoneSkill(arr[i].SkillName))
			{
				num++;
			}
		}
		return num;
	}

	private static void ClearElements(WeaponClass w)
	{
		if (w != null)
		{
			w.Fire = 0f;
			w.Frozen = 0f;
			w.Thunder = 0f;
			w.Poison = 0f;
			w.Physics = 0f;
			w.Shadow = 0f;
		}
	}

	private static void SetElement(WeaponClass w, int el, float v)
	{
		if (w != null)
		{
			switch (Mathf.Clamp(el, 0, 5))
			{
			case 0:
				w.Fire = v;
				break;
			case 1:
				w.Frozen = v;
				break;
			case 2:
				w.Thunder = v;
				break;
			case 3:
				w.Poison = v;
				break;
			case 4:
				w.Physics = v;
				break;
			case 5:
				w.Shadow = v;
				break;
			}
		}
	}

	private static void ClearSpcSlots(WeaponClass w)
	{
		if (w == null || w.SPC == null)
		{
			return;
		}
		for (int i = 0; i < w.SPC.Count; i++)
		{
			if (w.SPC[i] != null && w.SPC[i].Index != 0)
			{
				w.SetSPCData(i, 0, 0, 0f);
			}
		}
	}

	private static bool HasAnyElement(WeaponClass w)
	{
		if (w == null)
		{
			return false;
		}
		if (w.Fire > 0f || w.Frozen > 0f || w.Thunder > 0f || w.Poison > 0f || w.Physics > 0f)
		{
			return true;
		}
		return w.Shadow > 0f;
	}

	private static bool HasAnyLineOfTag(WeaponClass w, int tag)
	{
		if (w == null)
		{
			return false;
		}
		if (((uint)tag & (true ? 1u : 0u)) != 0 && (CountA(w.Main) > 0 || CountA(w.DOT) > 0 || CountB(w.SK) > 0 || (!IsArmorOrAccessory(w) && HasAnyElement(w))))
		{
			return true;
		}
		if (((uint)tag & 2u) != 0 && (CountB(w.CP) > 0 || HasActiveSpc(w) || (IsArmorOrAccessory(w) && HasAnyElement(w))))
		{
			return true;
		}
		return false;
	}

	private static bool HasActiveSpc(WeaponClass w)
	{
		if (w == null || w.SPC == null)
		{
			return false;
		}
		for (int i = 0; i < w.SPC.Count; i++)
		{
			if (w.SPC[i] != null && w.SPC[i].Index != 0)
			{
				return true;
			}
		}
		return false;
	}

	private static void Shuffle(List<int> list)
	{
		if (list != null && list.Count > 1)
		{
			for (int num = list.Count - 1; num > 0; num--)
			{
				int index = UnityEngine.Random.Range(0, num + 1);
				int value = list[num];
				list[num] = list[index];
				list[index] = value;
			}
		}
	}
}
