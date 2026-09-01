using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

public static class WeaponBaoshiApplyUtil
{
	private const int ElementEssenceMaxCount = 12;

	private const int NormalEssenceMaxCount = 8;

	public static bool TryApply(WeaponClass weapon, BaoshiClass baoshi, out bool refreshSocketDisplay)
	{
		refreshSocketDisplay = false;
		if (weapon == null || baoshi == null)
		{
			return false;
		}
		return baoshi.UseType switch
		{
			0 => TryApplySocketedGem(weapon, baoshi, out refreshSocketDisplay), 
			1 => TryApplyEssence(weapon, baoshi), 
			2 => TryApplyStone(weapon, baoshi, out refreshSocketDisplay), 
			3 => TryApplySkillRune(weapon, baoshi), 
			4 => TryApplySPCRune(weapon, baoshi), 
			5 => TryApplyAttributeRune(weapon, baoshi), 
			_ => false, 
		};
	}

	private static bool TryApplySocketedGem(WeaponClass weapon, BaoshiClass baoshi, out bool refreshSocketDisplay)
	{
		refreshSocketDisplay = false;
		if (weapon.Aocao == null || weapon.AocaoCount <= 0)
		{
			return false;
		}
		int num = -1;
		int num2 = Mathf.Min(weapon.AocaoCount, weapon.Aocao.Count);
		for (int i = 0; i < num2; i++)
		{
			WPAocao wPAocao = weapon.Aocao[i];
			if (wPAocao != null && wPAocao.HasAocao && !wPAocao.HasBaoshi)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return false;
		}
		int socketType = GetSocketType(baoshi.BStype, weapon.WeaponType);
		if (socketType < 0)
		{
			return false;
		}
		WPAocao wPAocao2 = weapon.Aocao[num];
		wPAocao2.HasBaoshi = true;
		wPAocao2.Icon = baoshi.Icon;
		wPAocao2.Name = baoshi.ItemName;
		wPAocao2.Number = baoshi.Number;
		wPAocao2.UseType = baoshi.UseType;
		wPAocao2.BS_Quality = baoshi.BS_Quality;
		wPAocao2.Type = socketType;
		weapon.Price += baoshi.Price;
		refreshSocketDisplay = true;
		return true;
	}

	private static int GetSocketType(string baoshiType, string weaponType)
	{
		switch (baoshiType)
		{
		case "red":
			switch (weaponType)
			{
			case "head":
			case "leg":
				return 0;
			case "body":
				return 1;
			case "hand":
				return 2;
			case "bone":
			case "bow":
			case "sword":
			case "staff":
			case "arrow":
			case "spell":
			case "corpse":
			case "shield":
				return 3;
			}
			break;
		case "yellow":
			switch (weaponType)
			{
			case "head":
			case "leg":
				return 4;
			case "body":
				return 5;
			case "hand":
				return 6;
			case "bone":
			case "bow":
			case "sword":
			case "staff":
			case "arrow":
			case "spell":
			case "corpse":
			case "shield":
				return 7;
			}
			break;
		case "green":
			switch (weaponType)
			{
			case "head":
				return 8;
			case "body":
				return 9;
			case "hand":
				return 10;
			case "leg":
				return 11;
			case "bone":
			case "bow":
			case "sword":
			case "staff":
			case "arrow":
			case "spell":
			case "corpse":
			case "shield":
				return 12;
			}
			break;
		case "blue":
			switch (weaponType)
			{
			case "head":
			case "leg":
				return 13;
			case "body":
				return 14;
			case "hand":
				return 15;
			case "bone":
			case "bow":
			case "sword":
			case "staff":
			case "arrow":
			case "spell":
			case "corpse":
			case "shield":
				return 16;
			}
			break;
		case "purple":
			switch (weaponType)
			{
			case "head":
				return 17;
			case "body":
				return 18;
			case "hand":
				return 19;
			case "leg":
				return 20;
			case "bone":
			case "bow":
			case "sword":
			case "staff":
			case "arrow":
			case "spell":
			case "corpse":
			case "shield":
				return 21;
			}
			break;
		case "white":
			switch (weaponType)
			{
			case "head":
			case "leg":
				return 22;
			case "body":
				return 23;
			case "hand":
				return 24;
			case "bone":
			case "bow":
			case "sword":
			case "staff":
			case "arrow":
			case "spell":
			case "corpse":
			case "shield":
				return 25;
			}
			break;
		case "projectile":
			return 26;
		}
		return -1;
	}

	private static bool TryApplyEssence(WeaponClass weapon, BaoshiClass baoshi)
	{
		string bStype = baoshi.BStype;
		if (string.IsNullOrEmpty(bStype))
		{
			return false;
		}
		if (bStype.StartsWith("JHEL", StringComparison.Ordinal))
		{
			if (weapon.JHEL_Count >= 12)
			{
				return false;
			}
			if (!int.TryParse(bStype.Substring(4), out var result) || result < 0 || result > 5)
			{
				return false;
			}
			float elementEssenceValue = GetElementEssenceValue(weapon);
			if (elementEssenceValue <= 0f)
			{
				return false;
			}
			AddElementValue(weapon, result, elementEssenceValue);
			weapon.JHEL_Count++;
			return true;
		}
		if (weapon.JH_Count >= 8)
		{
			return false;
		}
		bool flag;
		switch (bStype)
		{
		case "JH_damage":
			flag = IsWeaponEquipment(weapon) && AddMainStat(weapon, 10, baoshi.Number);
			break;
		case "JH_heal":
			flag = IsArmorEquipment(weapon) && AddMainStat(weapon, 1, baoshi.Number);
			break;
		case "JH_mana":
			flag = IsArmorEquipment(weapon) && AddMainStat(weapon, 2, baoshi.Number);
			break;
		case "JH_ats":
			flag = IsWeaponEquipment(weapon) && AddMainStat(weapon, 11, baoshi.Number);
			break;
		case "JH_CPdamage":
			flag = IsWeaponEquipment(weapon) && AddMainStat(weapon, 101, baoshi.Number);
			break;
		case "JH_CPheal":
			flag = IsArmorEquipment(weapon) && AddMainStat(weapon, 100, baoshi.Number);
			break;
		default:
			return false;
		}
		if (!flag)
		{
			return false;
		}
		weapon.JH_Count++;
		return true;
	}

	private static bool TryApplyStone(WeaponClass weapon, BaoshiClass baoshi, out bool refreshSocketDisplay)
	{
		refreshSocketDisplay = false;
		switch (baoshi.BStype)
		{
		case "Stone_KZ":
			if (IsWeaponEquipment(weapon) || IsArmorEquipment(weapon))
			{
				return TryAddSocket(weapon, out refreshSocketDisplay);
			}
			return false;
		case "Stone_CG":
			if (IsArmorEquipment(weapon))
			{
				return TryApplyBaseValueDouble(weapon);
			}
			return false;
		case "Stone_HH":
			return TryAddTransmutation(weapon, baoshi.Number);
		case "Stone_FS":
			if (!CanRegenerateWeapon(weapon))
			{
				return false;
			}
			return refreshSocketDisplay = SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.TryRegenerateWeaponFromTemplate(weapon, SingletonMonoScope<PlayerManager>.Instance.Level);
		case "Stone_LC":
			if (IsAccessoryEquipment(weapon))
			{
				return TryApplyBaseValueDouble(weapon);
			}
			return false;
		case "Stone_HM":
			if (IsWeaponEquipment(weapon))
			{
				return TryApplyBaseValueDouble(weapon);
			}
			return false;
		case "Stone_AM":
			return weapon.TryAddSkillFWCountMax();
		default:
			return false;
		}
	}

	private static bool TryApplySkillRune(WeaponClass weapon, BaoshiClass baoshi)
	{
		string sKname = baoshi.SKname;
		if (string.IsNullOrEmpty(sKname) || sKname == "0")
		{
			return false;
		}
		if (SingletonMonoScope<TalentManager>.HasInstance && (SingletonMonoScope<TalentManager>.Instance.SKI == null || !SingletonMonoScope<TalentManager>.Instance.SKI.ContainsKey(sKname)))
		{
			return false;
		}
		if (!weapon.CanSocketSkillFW(weapon.SKCount))
		{
			return false;
		}
		EnsureSkillSlots(weapon);
		int num = FindSkillSlot(weapon, sKname);
		if (num >= 0)
		{
			WPSkill wPSkill = weapon.WPSK[num];
			wPSkill.Number2++;
			wPSkill.price += baoshi.Price;
			weapon.Price += baoshi.Price;
			weapon.SKCount++;
			weapon.WP_SkillCount = Mathf.Max(weapon.WP_SkillCount, num + 1);
			return true;
		}
		int num2 = FindEmptySkillSlot(weapon);
		if (num2 < 0)
		{
			return false;
		}
		WPSkill wPSkill2 = weapon.WPSK[num2];
		wPSkill2.IndexName = sKname;
		wPSkill2.Number = 0;
		wPSkill2.Number2++;
		wPSkill2.price += baoshi.Price;
		weapon.Price += baoshi.Price;
		weapon.SKCount++;
		weapon.WP_SkillCount = Mathf.Max(weapon.WP_SkillCount, num2 + 1);
		return true;
	}

	private static bool TryApplySPCRune(WeaponClass weapon, BaoshiClass baoshi)
	{
		if (baoshi.Index <= 0 || !SingletonMonoScope<ItemManager>.HasInstance)
		{
			return false;
		}
		if (!SingletonMonoScope<ItemManager>.Instance.TryGetSPCMBByIndex(baoshi.Index, out var mb) || mb == null)
		{
			return false;
		}
		if (!CanSocketSPCFWType(weapon, baoshi.FWtype))
		{
			return false;
		}
		WPSPC sPCData = weapon.GetSPCData(1);
		if (sPCData.Index > 0)
		{
			return false;
		}
		if (!CanSocketSPCForPlayer(weapon, mb))
		{
			return false;
		}
		sPCData.Index = baoshi.Index;
		sPCData.EL = baoshi.EL;
		sPCData.PRC = baoshi.PRC;
		sPCData.price = baoshi.Price;
		weapon.Price += baoshi.Price;
		return true;
	}

	private static bool TryApplyAttributeRune(WeaponClass weapon, BaoshiClass baoshi)
	{
		if (!CanSocketAttributeFWType(weapon, baoshi.FWtype))
		{
			return false;
		}
		if (weapon.FW_Base != null && !string.IsNullOrEmpty(weapon.FW_Base.FWname))
		{
			return false;
		}
		weapon.FW_Base = new WPFW_Base
		{
			FWname = (string.IsNullOrEmpty(baoshi.SKname) ? baoshi.ItemName : baoshi.SKname),
			type = baoshi.BStype,
			number = baoshi.Number,
			price = baoshi.Price
		};
		weapon.Price += baoshi.Price;
		return true;
	}

	private static bool TryAddSocket(WeaponClass weapon, out bool refreshSocketDisplay)
	{
		refreshSocketDisplay = false;
		if (weapon.Aocao == null || weapon.Aocao.Count <= 0)
		{
			return false;
		}
		int num = Mathf.Min(weapon.MaxAocaoCount, weapon.Aocao.Count);
		if (weapon.AocaoCount >= num)
		{
			return false;
		}
		int index = Mathf.Clamp(weapon.AocaoCount, 0, weapon.Aocao.Count - 1);
		weapon.AocaoCount++;
		weapon.Aocao[index].HasAocao = true;
		weapon.Aocao[index].HasBaoshi = false;
		refreshSocketDisplay = true;
		return true;
	}

	private static bool TryReduceEnhanceCount(WeaponClass weapon, int count)
	{
		if (weapon.ZQ_CountMax <= 0 || count <= 0)
		{
			return false;
		}
		weapon.ZQ_CountMax = Mathf.Max(0, weapon.ZQ_CountMax - count);
		return true;
	}

	private static bool TryApplyBaseValueDouble(WeaponClass weapon)
	{
		return weapon?.TryApplyBaseValueDouble() ?? false;
	}

	private static bool TryAddTransmutation(WeaponClass weapon, float number)
	{
		int num = (SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.HH_Inc : 10);
		if (num <= 0)
		{
			num = 10;
		}
		if (weapon.HHCount >= num)
		{
			return false;
		}
		weapon.HHCount++;
		weapon.NormalizeSPCDamageBei();
		weapon.SPC_DMG_Bei += number;
		return true;
	}

	private static bool CanRegenerateWeapon(WeaponClass weapon)
	{
		if (weapon == null)
		{
			return false;
		}
		if (!HasSocketedSPCRune(weapon) && !HasAttributeRune(weapon) && !HasSocketedGem(weapon))
		{
			return !HasSkillRuneBonus(weapon);
		}
		return false;
	}

	private static bool HasSocketedSPCRune(WeaponClass weapon)
	{
		if (weapon.SPC == null)
		{
			return false;
		}
		for (int i = 1; i < weapon.SPC.Count; i++)
		{
			if (weapon.SPC[i] != null && weapon.SPC[i].Index > 0)
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasAttributeRune(WeaponClass weapon)
	{
		if (weapon.FW_Base != null)
		{
			if (string.IsNullOrEmpty(weapon.FW_Base.FWname))
			{
				return !string.IsNullOrEmpty(weapon.FW_Base.type);
			}
			return true;
		}
		return false;
	}

	private static bool HasSocketedGem(WeaponClass weapon)
	{
		if (weapon.Aocao == null)
		{
			return false;
		}
		for (int i = 0; i < weapon.Aocao.Count; i++)
		{
			WPAocao wPAocao = weapon.Aocao[i];
			if (wPAocao != null && (wPAocao.HasBaoshi || !string.IsNullOrEmpty(wPAocao.Name)))
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasSkillRuneBonus(WeaponClass weapon)
	{
		if (weapon.WPSK == null)
		{
			return false;
		}
		for (int i = 0; i < weapon.WPSK.Count; i++)
		{
			if (weapon.WPSK[i] != null && weapon.WPSK[i].Number2 != 0)
			{
				return true;
			}
		}
		return false;
	}

	private static void AddElementValue(WeaponClass weapon, int element, float number)
	{
		switch (element)
		{
		case 0:
			weapon.Fire += number;
			break;
		case 1:
			weapon.Frozen += number;
			break;
		case 2:
			weapon.Thunder += number;
			break;
		case 3:
			weapon.Poison += number;
			break;
		case 4:
			weapon.Physics += number;
			break;
		case 5:
			weapon.Shadow += number;
			break;
		}
	}

	private static float GetElementEssenceValue(WeaponClass weapon)
	{
		if (weapon == null)
		{
			return 0f;
		}
		switch (weapon.CharType)
		{
		case 0:
			return 4f;
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 8:
			return 1f;
		case 7:
		case 9:
			return 3f;
		default:
			return 0f;
		}
	}

	private static bool AddMainStat(WeaponClass weapon, int index, float number)
	{
		if (weapon.Main == null)
		{
			weapon.Main = new WPDT_A[1] { CreateMainStat(index, number) };
			return true;
		}
		for (int i = 0; i < weapon.Main.Length; i++)
		{
			if (weapon.Main[i] != null && weapon.Main[i].Index == index)
			{
				weapon.Main[i].number += number;
				return true;
			}
		}
		Array.Resize(ref weapon.Main, weapon.Main.Length + 1);
		weapon.Main[weapon.Main.Length - 1] = CreateMainStat(index, number);
		return true;
	}

	private static WPDT_A CreateMainStat(int index, float number)
	{
		return new WPDT_A
		{
			Index = index,
			EL = 0,
			number = number
		};
	}

	private static void EnsureSkillSlots(WeaponClass weapon)
	{
		if (weapon.WPSK == null)
		{
			weapon.WPSK = new List<WPSkill>();
		}
		while (weapon.WPSK.Count < 6)
		{
			weapon.WPSK.Add(new WPSkill());
		}
	}

	private static int FindSkillSlot(WeaponClass weapon, string skillName)
	{
		for (int i = 0; i < weapon.WPSK.Count; i++)
		{
			if (weapon.WPSK[i] != null && weapon.WPSK[i].IndexName == skillName)
			{
				return i;
			}
		}
		return -1;
	}

	private static int FindEmptySkillSlot(WeaponClass weapon)
	{
		for (int i = 0; i < weapon.WPSK.Count; i++)
		{
			WPSkill wPSkill = weapon.WPSK[i];
			if (wPSkill == null)
			{
				weapon.WPSK[i] = new WPSkill();
				return i;
			}
			if (string.IsNullOrEmpty(wPSkill.IndexName) || wPSkill.IndexName == "0")
			{
				return i;
			}
		}
		return -1;
	}

	private static bool CanSocketSPCForPlayer(WeaponClass weapon, SPC_MB mb)
	{
		if (WeaponPlayerType.IsGeneric(weapon.PLtype))
		{
			return true;
		}
		string skillName = mb.SkillName;
		if (IsCommonSPCSkillName(skillName))
		{
			return true;
		}
		if (!SingletonMonoScope<TalentManager>.HasInstance)
		{
			return false;
		}
		if (SingletonMonoScope<TalentManager>.Instance.TryGetSkillFWPlayerType(skillName, out var plType))
		{
			return plType == weapon.PLtype;
		}
		return false;
	}

	private static bool IsCommonSPCSkillName(string spcName)
	{
		if (string.IsNullOrWhiteSpace(spcName))
		{
			return true;
		}
		string text = spcName.Trim();
		if (!(text == "0") && !(text == "无"))
		{
			return text.Equals("none", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static bool CanSocketSPCFWType(WeaponClass weapon, int fwType)
	{
		switch (fwType)
		{
		case 0:
			return IsWeaponEquipment(weapon);
		case 1:
			if (weapon.CharType != 2)
			{
				return weapon.CharType == 3;
			}
			return true;
		case 2:
			if (weapon.CharType != 4)
			{
				return weapon.CharType == 5;
			}
			return true;
		case 3:
			if (weapon.CharType != 6)
			{
				return weapon.CharType == 8;
			}
			return true;
		case 4:
			if (weapon.CharType != 7)
			{
				return weapon.CharType == 9;
			}
			return true;
		default:
			return false;
		}
	}

	private static bool CanSocketAttributeFWType(WeaponClass weapon, int fwType)
	{
		return fwType switch
		{
			0 => IsWeaponEquipment(weapon), 
			1 => IsArmorEquipment(weapon), 
			2 => IsAccessoryEquipment(weapon), 
			_ => false, 
		};
	}

	private static bool IsWeaponEquipment(WeaponClass weapon)
	{
		if (weapon != null)
		{
			if (weapon.CharType != 0)
			{
				return weapon.CharType == 1;
			}
			return true;
		}
		return false;
	}

	private static bool IsMainhandWeapon(WeaponClass weapon)
	{
		if (weapon != null)
		{
			return weapon.CharType == 0;
		}
		return false;
	}

	private static bool IsOffhandWeapon(WeaponClass weapon)
	{
		if (weapon != null)
		{
			return weapon.CharType == 1;
		}
		return false;
	}

	private static bool IsArmorEquipment(WeaponClass weapon)
	{
		if (weapon != null && weapon.CharType >= 2)
		{
			return weapon.CharType <= 5;
		}
		return false;
	}

	private static bool IsAccessoryEquipment(WeaponClass weapon)
	{
		if (weapon != null && weapon.CharType >= 6)
		{
			return weapon.CharType <= 9;
		}
		return false;
	}
}
