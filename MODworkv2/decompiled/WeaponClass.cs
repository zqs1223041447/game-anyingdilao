using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Settings;
using Entity.InteractableObjects.Item;
using FinkFramework.Runtime.Singleton;
using PoedbMod;
using UnityEngine;

[Serializable]
public class WeaponClass : ItemClass, IDropItemData
{
	private enum ElementMainLabel
	{
		Damage,
		Penetration,
		Resistance,
		Reduction
	}

	private const string ElementEssenceCountColor = "#5AF0DA";

	private const string NormalEssenceCountColor = "#FF8BCB";

	public bool Enchanted;

	public int Reb_CountMax;

	public int ZQ_CountMax;

	[NonSerialized]
	public bool Craft_LockPrefix;

	[NonSerialized]
	public bool Craft_LockSuffix;

	[NonSerialized]
	public bool Craft_NoAttack;

	[NonSerialized]
	public bool Craft_NoCaster;

	public int HHCount;

	public int SKCount;

	public int JHEL_Count;

	public int JH_Count;

	public int SkillFW_CountMax;

	public int PLtype;

	public string WeaponType;

	public int CharType;

	public float Damage;

	public float Health;

	public float Mana;

	public bool BaseValueDoubled;

	public float BaseValueMultiplier;

	public float Fire;

	public float Frozen;

	public float Thunder;

	public float Poison;

	public float Physics;

	public float Shadow;

	public int WP_SkillCount;

	public List<WPSkill> WPSK = new List<WPSkill>();

	public int MaxAocaoCount;

	public int AocaoCount;

	public List<WPAocao> Aocao = new List<WPAocao>();

	public WPDT_A[] Main;

	public WPDT_A[] DOT;

	public WPDT_B[] SK;

	public WPDT_B[] CP;

	public WPFW_Base FW_Base;

	public List<WPSPC> SPC = new List<WPSPC>();

	public float SPC_DMG_Bei;

	public int Set_Index;

	public int BS_Set_Index;

	public Set_DT SetRuntimeData;

	public int DropScene;

	public int MJ_Level;

	public int ByPrice => Price * 8;

	public float DamageFinal => Damage * GetBaseValueMultiplier();

	public float HealthFinal => Health * GetBaseValueMultiplier();

	public float ManaFinal => Mana * GetBaseValueMultiplier();

	public bool HasBaseValueDouble
	{
		get
		{
			if (!BaseValueDoubled)
			{
				return GetBaseValueMultiplier() > 1f;
			}
			return true;
		}
	}

	int IDropItemData.ItemType => ItemType;

	private static void EL_Float(int el, float number, ref float el0, ref float el1, ref float el2, ref float el3, ref float el4, ref float el5)
	{
		switch (Mathf.Clamp(el, 0, 5))
		{
		case 0:
			el0 += number;
			break;
		case 1:
			el1 += number;
			break;
		case 2:
			el2 += number;
			break;
		case 3:
			el3 += number;
			break;
		case 4:
			el4 += number;
			break;
		case 5:
			el5 += number;
			break;
		}
	}

	private static void EL_Int(int el, int number, ref int el0, ref int el1, ref int el2, ref int el3, ref int el4, ref int el5)
	{
		switch (Mathf.Clamp(el, 0, 5))
		{
		case 0:
			el0 += number;
			break;
		case 1:
			el1 += number;
			break;
		case 2:
			el2 += number;
			break;
		case 3:
			el3 += number;
			break;
		case 4:
			el4 += number;
			break;
		case 5:
			el5 += number;
			break;
		}
	}

	private static void EL_to_Bool(int el, bool value, ref bool el0, ref bool el1, ref bool el2, ref bool el3, ref bool el4, ref bool el5)
	{
		switch (Mathf.Clamp(el, 0, 5))
		{
		case 0:
			el0 = value;
			break;
		case 1:
			el1 = value;
			break;
		case 2:
			el2 = value;
			break;
		case 3:
			el3 = value;
			break;
		case 4:
			el4 = value;
			break;
		case 5:
			el5 = value;
			break;
		}
	}

	public WeaponClass()
	{
		InitDefault();
	}

	private void InitDefault()
	{
		Enchanted = false;
		Reb_CountMax = 0;
		ZQ_CountMax = 0;
		Craft_LockPrefix = false;
		Craft_LockSuffix = false;
		Craft_NoAttack = false;
		Craft_NoCaster = false;
		HHCount = 0;
		SKCount = 0;
		JHEL_Count = 0;
		JH_Count = 0;
		SetRuntimeData = null;
		BS_Set_Index = 0;
		DropScene = 0;
		MJ_Level = 0;
		SkillFW_CountMax = 0;
		SPC_DMG_Bei = 100f;
		BaseValueDoubled = false;
		BaseValueMultiplier = 1f;
		WP_SkillCount = 0;
		AocaoCount = 0;
		WPSK.Clear();
		Aocao.Clear();
		if (SPC == null)
		{
			SPC = new List<WPSPC>();
		}
		else
		{
			SPC.Clear();
		}
		for (int i = 0; i < 6; i++)
		{
			WPSK.Add(new WPSkill());
			Aocao.Add(new WPAocao
			{
				HasAocao = false
			});
		}
		SPC.Add(new WPSPC());
		ResetSkillFWCountMax();
	}

	public int GetDefaultSkillFWCountMax()
	{
		if (IsArmorType())
		{
			return 4;
		}
		if (IsAccessoryType())
		{
			return 2;
		}
		return 8;
	}

	public int GetSkillFWHardCountMax()
	{
		if (IsArmorType())
		{
			return 8;
		}
		if (IsAccessoryType())
		{
			return 4;
		}
		return 12;
	}

	public void ResetSkillFWCountMax()
	{
		SkillFW_CountMax = GetDefaultSkillFWCountMax();
	}

	public void NormalizeSkillFWCountMax()
	{
		int defaultSkillFWCountMax = GetDefaultSkillFWCountMax();
		int skillFWHardCountMax = GetSkillFWHardCountMax();
		SkillFW_CountMax = Mathf.Clamp((SkillFW_CountMax <= 0) ? defaultSkillFWCountMax : SkillFW_CountMax, defaultSkillFWCountMax, skillFWHardCountMax);
	}

	public bool TryAddSkillFWCountMax(int count = 1)
	{
		NormalizeSkillFWCountMax();
		int skillFW_CountMax = SkillFW_CountMax;
		SkillFW_CountMax = Mathf.Clamp(SkillFW_CountMax + Mathf.Max(0, count), GetDefaultSkillFWCountMax(), GetSkillFWHardCountMax());
		return SkillFW_CountMax > skillFW_CountMax;
	}

	public int GetSkillFWCountMaxRemain()
	{
		NormalizeSkillFWCountMax();
		return Mathf.Max(0, GetSkillFWHardCountMax() - SkillFW_CountMax);
	}

	public bool CanSocketSkillFW(int currentSkillFWCount)
	{
		NormalizeSkillFWCountMax();
		return Mathf.Max(0, currentSkillFWCount) < SkillFW_CountMax;
	}

	public int GetSkillFWSocketRemain(int currentSkillFWCount)
	{
		NormalizeSkillFWCountMax();
		return Mathf.Max(0, SkillFW_CountMax - Mathf.Max(0, currentSkillFWCount));
	}

	private bool IsArmorType()
	{
		if ((CharType < 2 || CharType > 5) && !(WeaponType == "head") && !(WeaponType == "body") && !(WeaponType == "hand"))
		{
			return WeaponType == "leg";
		}
		return true;
	}

	private bool IsAccessoryType()
	{
		if (CharType < 6 || CharType > 9)
		{
			return WeaponType == "little";
		}
		return true;
	}

	public void EnsureSPCSlot(int index)
	{
		if (SPC == null)
		{
			SPC = new List<WPSPC>();
		}
		while (SPC.Count <= index)
		{
			SPC.Add(new WPSPC());
		}
		if (SPC[index] == null)
		{
			SPC[index] = new WPSPC();
		}
	}

	public WPSPC GetSPCData(int index)
	{
		EnsureSPCSlot(index);
		return SPC[index];
	}

	public void SetSPCData(int index, int spcIndex, int element, float prc)
	{
		WPSPC sPCData = GetSPCData(index);
		sPCData.Index = spcIndex;
		sPCData.EL = element;
		sPCData.PRC = prc;
	}

	public void NormalizeSPCDamageBei()
	{
		if (SPC_DMG_Bei <= 0f)
		{
			SPC_DMG_Bei = 100f;
		}
	}

	public void NormalizeBaseValueMultiplier()
	{
		if (BaseValueMultiplier <= 0f)
		{
			BaseValueMultiplier = (BaseValueDoubled ? 2f : 1f);
		}
		if (BaseValueDoubled && BaseValueMultiplier < 1.0001f)
		{
			BaseValueMultiplier = 2f;
		}
		if (!BaseValueDoubled && BaseValueMultiplier < 1f)
		{
			BaseValueMultiplier = 1f;
		}
	}

	public float GetBaseValueMultiplier()
	{
		if (BaseValueMultiplier <= 0f)
		{
			if (!BaseValueDoubled)
			{
				return 1f;
			}
			return 2f;
		}
		return Mathf.Max(1f, BaseValueMultiplier);
	}

	public bool TryApplyBaseValueDouble()
	{
		NormalizeBaseValueMultiplier();
		if (BaseValueDoubled || BaseValueMultiplier > 1f)
		{
			return false;
		}
		BaseValueDoubled = true;
		BaseValueMultiplier = 2f;
		return true;
	}

	public int GetBaseValueDoubleIconIndex()
	{
		if (IsArmorType())
		{
			return 1;
		}
		if (IsAccessoryType())
		{
			return 2;
		}
		return 0;
	}

	public float GetSPCPRC(WPSPC spc)
	{
		if (spc == null)
		{
			return 0f;
		}
		NormalizeSPCDamageBei();
		return spc.PRC * SPC_DMG_Bei / 100f;
	}

	public bool TryGetSPCData(int index, out WPSPC spc)
	{
		spc = null;
		if (SPC == null || index < 0 || index >= SPC.Count || SPC[index] == null)
		{
			return false;
		}
		spc = SPC[index];
		return true;
	}

	public bool HasSPC(int index)
	{
		if (TryGetSPCData(index, out var spc))
		{
			return spc.Index > 0;
		}
		return false;
	}

	public bool TryGetSPCTemplate(int index, out WPSPC spc, out SPC_MB mb)
	{
		mb = null;
		if (!TryGetSPCData(index, out spc) || spc.Index <= 0)
		{
			return false;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return false;
		}
		if (index != 0)
		{
			return SingletonMonoScope<ItemManager>.Instance.TryGetSPCMBByIndex(spc.Index, out mb);
		}
		return SingletonMonoScope<ItemManager>.Instance.TryGetWeaponSPCMBByIndex(spc.Index, out mb);
	}

	public void MigrateLegacySPCData(int spcIndex, int element, float prc)
	{
		if (SPC == null || SPC.Count <= 0)
		{
			SetSPCData(0, spcIndex, element, prc);
		}
	}

	public override void Reset()
	{
		base.Reset();
		InitDefault();
	}

	public override string GetTitle(bool displayEnhance = true)
	{
		string text = "<color=" + QualityColor.Colors[Quality] + ">" + LOC.MM.GetItem(ItemName) + "</color>";
		if (displayEnhance)
		{
			if (ZQ_CountMax > 0)
			{
				text += string.Format(" <color={0}>+{1}</color>", "#" + ColorUtility.ToHtmlStringRGB(SettingsLoader.Instance.weaponSettings.textColor), ZQ_CountMax);
			}
			if (JHEL_Count > 0)
			{
				text += string.Format(" <color={0}>+{1}</color>", "#5AF0DA", JHEL_Count);
			}
			if (JH_Count > 0)
			{
				text += string.Format(" <color={0}>+{1}</color>", "#FF8BCB", JH_Count);
			}
		}
		return text;
	}

	public string GetMain(WeaponClass old_data = null)
	{
		string target2 = string.Empty;
		if (PoeItemMod.TryGetDescription(ItemName, out var text))
		{
			AppendLine(ref target2, "<color=#00E5FF>" + text + "</color>");
		}
		float damageFinal = DamageFinal;
		float healthFinal = HealthFinal;
		float manaFinal = ManaFinal;
		if (damageFinal > 0f)
		{
			AppendLine(ref target2, string.Format("{0} + {1}{2}", LOC.MM.GetMain("damage"), (int)damageFinal, GetDiffText(old_data?.DamageFinal ?? 0f, damageFinal)));
		}
		if (healthFinal > 0f)
		{
			AppendLine(ref target2, string.Format("{0} + {1}{2}", LOC.MM.GetMain("Health"), (int)healthFinal, GetDiffText(old_data?.HealthFinal ?? 0f, healthFinal)));
		}
		if (manaFinal > 0f)
		{
			AppendLine(ref target2, string.Format("{0} + {1}{2}", LOC.MM.GetMain("Mana"), (int)manaFinal, GetDiffText(old_data?.ManaFinal ?? 0f, manaFinal)));
		}
		AppendMainArrayLines(ref target2, old_data);
		switch (WeaponType)
		{
		case "staff":
		case "sword":
		case "bow":
		case "bone":
			if (ShouldShowElementLine(Fire))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.fire], LOC.MM.GetMain("fire damage"), (int)Fire, GetElementDiffText(old_data?.Fire ?? 0f, Fire)));
			}
			if (ShouldShowElementLine(Frozen))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.frozen], LOC.MM.GetMain("frozen damage"), (int)Frozen, GetElementDiffText(old_data?.Frozen ?? 0f, Frozen)));
			}
			if (ShouldShowElementLine(Thunder))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.thunder], LOC.MM.GetMain("thunder damage"), (int)Thunder, GetElementDiffText(old_data?.Thunder ?? 0f, Thunder)));
			}
			if (ShouldShowElementLine(Poison))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.poison], LOC.MM.GetMain("poison damage"), (int)Poison, GetElementDiffText(old_data?.Poison ?? 0f, Poison)));
			}
			if (ShouldShowElementLine(Physics))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.physics], LOC.MM.GetMain("physics damage"), (int)Physics, GetElementDiffText(old_data?.Physics ?? 0f, Physics)));
			}
			if (ShouldShowElementLine(Shadow))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.shadow], LOC.MM.GetMain("shadow damage"), (int)Shadow, GetElementDiffText(old_data?.Shadow ?? 0f, Shadow)));
			}
			break;
		case "spell":
		case "arrow":
		case "corpse":
		case "shield":
			if (ShouldShowElementLine(Fire))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.fire], LOC.MM.GetMain("fire chuan"), (int)Fire, GetElementDiffText(old_data?.Fire ?? 0f, Fire)));
			}
			if (ShouldShowElementLine(Frozen))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.frozen], LOC.MM.GetMain("frozen chuan"), (int)Frozen, GetElementDiffText(old_data?.Frozen ?? 0f, Frozen)));
			}
			if (ShouldShowElementLine(Thunder))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.thunder], LOC.MM.GetMain("thunder chuan"), (int)Thunder, GetElementDiffText(old_data?.Thunder ?? 0f, Thunder)));
			}
			if (ShouldShowElementLine(Poison))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.poison], LOC.MM.GetMain("poison chuan"), (int)Poison, GetElementDiffText(old_data?.Poison ?? 0f, Poison)));
			}
			if (ShouldShowElementLine(Physics))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.physics], LOC.MM.GetMain("physics chuan"), (int)Physics, GetElementDiffText(old_data?.Physics ?? 0f, Physics)));
			}
			if (ShouldShowElementLine(Shadow))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.shadow], LOC.MM.GetMain("shadow chuan"), (int)Shadow, GetElementDiffText(old_data?.Shadow ?? 0f, Shadow)));
			}
			break;
		case "leg":
		case "hand":
		case "head":
		case "body":
			if (ShouldShowElementLine(Fire))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.fire], LOC.MM.GetMain("fire Anti"), (int)Fire, GetElementDiffText(old_data?.Fire ?? 0f, Fire)));
			}
			if (ShouldShowElementLine(Frozen))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.frozen], LOC.MM.GetMain("frozen Anti"), (int)Frozen, GetElementDiffText(old_data?.Frozen ?? 0f, Frozen)));
			}
			if (ShouldShowElementLine(Thunder))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.thunder], LOC.MM.GetMain("thunder Anti"), (int)Thunder, GetElementDiffText(old_data?.Thunder ?? 0f, Thunder)));
			}
			if (ShouldShowElementLine(Poison))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.poison], LOC.MM.GetMain("poison Anti"), (int)Poison, GetElementDiffText(old_data?.Poison ?? 0f, Poison)));
			}
			if (ShouldShowElementLine(Physics))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.physics], LOC.MM.GetMain("physics Anti"), (int)Physics, GetElementDiffText(old_data?.Physics ?? 0f, Physics)));
			}
			if (ShouldShowElementLine(Shadow))
			{
				AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.shadow], LOC.MM.GetMain("shadow Anti"), (int)Shadow, GetElementDiffText(old_data?.Shadow ?? 0f, Shadow)));
			}
			break;
		case "little":
			switch (CharType)
			{
			case 6:
				if (ShouldShowElementLine(Fire))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.fire], LOC.MM.GetMain("fire Anti"), (int)Fire, GetElementDiffText(old_data?.Fire ?? 0f, Fire)));
				}
				if (ShouldShowElementLine(Frozen))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.frozen], LOC.MM.GetMain("frozen Anti"), (int)Frozen, GetElementDiffText(old_data?.Frozen ?? 0f, Frozen)));
				}
				if (ShouldShowElementLine(Thunder))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.thunder], LOC.MM.GetMain("thunder Anti"), (int)Thunder, GetElementDiffText(old_data?.Thunder ?? 0f, Thunder)));
				}
				if (ShouldShowElementLine(Poison))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.poison], LOC.MM.GetMain("poison Anti"), (int)Poison, GetElementDiffText(old_data?.Poison ?? 0f, Poison)));
				}
				if (ShouldShowElementLine(Physics))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.physics], LOC.MM.GetMain("physics Anti"), (int)Physics, GetElementDiffText(old_data?.Physics ?? 0f, Physics)));
				}
				if (ShouldShowElementLine(Shadow))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.shadow], LOC.MM.GetMain("shadow Anti"), (int)Shadow, GetElementDiffText(old_data?.Shadow ?? 0f, Shadow)));
				}
				break;
			case 7:
			case 9:
				if (ShouldShowElementLine(Fire))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.fire], LOC.MM.GetMain("fire damage"), (int)Fire, GetElementDiffText(old_data?.Fire ?? 0f, Fire)));
				}
				if (ShouldShowElementLine(Frozen))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.frozen], LOC.MM.GetMain("frozen damage"), (int)Frozen, GetElementDiffText(old_data?.Frozen ?? 0f, Frozen)));
				}
				if (ShouldShowElementLine(Thunder))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.thunder], LOC.MM.GetMain("thunder damage"), (int)Thunder, GetElementDiffText(old_data?.Thunder ?? 0f, Thunder)));
				}
				if (ShouldShowElementLine(Poison))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.poison], LOC.MM.GetMain("poison damage"), (int)Poison, GetElementDiffText(old_data?.Poison ?? 0f, Poison)));
				}
				if (ShouldShowElementLine(Physics))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.physics], LOC.MM.GetMain("physics damage"), (int)Physics, GetElementDiffText(old_data?.Physics ?? 0f, Physics)));
				}
				if (ShouldShowElementLine(Shadow))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.shadow], LOC.MM.GetMain("shadow damage"), (int)Shadow, GetElementDiffText(old_data?.Shadow ?? 0f, Shadow)));
				}
				break;
			case 8:
				if (ShouldShowElementLine(Fire))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.fire], LOC.MM.GetMain("fire chuan"), (int)Fire, GetElementDiffText(old_data?.Fire ?? 0f, Fire)));
				}
				if (ShouldShowElementLine(Frozen))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.frozen], LOC.MM.GetMain("frozen chuan"), (int)Frozen, GetElementDiffText(old_data?.Frozen ?? 0f, Frozen)));
				}
				if (ShouldShowElementLine(Thunder))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.thunder], LOC.MM.GetMain("thunder chuan"), (int)Thunder, GetElementDiffText(old_data?.Thunder ?? 0f, Thunder)));
				}
				if (ShouldShowElementLine(Poison))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.poison], LOC.MM.GetMain("poison chuan"), (int)Poison, GetElementDiffText(old_data?.Poison ?? 0f, Poison)));
				}
				if (ShouldShowElementLine(Physics))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.physics], LOC.MM.GetMain("physics chuan"), (int)Physics, GetElementDiffText(old_data?.Physics ?? 0f, Physics)));
				}
				if (ShouldShowElementLine(Shadow))
				{
					AppendLine(ref target2, string.Format("<color={0}>{1} + {2}%{3}</color>", DamageColor.Colors[DamageType.shadow], LOC.MM.GetMain("shadow chuan"), (int)Shadow, GetElementDiffText(old_data?.Shadow ?? 0f, Shadow)));
				}
				break;
			}
			break;
		}
		return target2;
		static void AppendLine(ref string target, string line)
		{
			if (!string.IsNullOrEmpty(line))
			{
				if (string.IsNullOrEmpty(target))
				{
					target = line;
				}
				else
				{
					target = target + "\n" + line;
				}
			}
		}
		string GetDiffText(float oldValue, float newValue)
		{
			if (old_data == null)
			{
				return string.Empty;
			}
			int num2 = Mathf.FloorToInt(oldValue);
			int num3 = Mathf.FloorToInt(newValue) - num2;
			if (num3 == 0)
			{
				return string.Empty;
			}
			if (num3 > 0)
			{
				return $" <color=#00FF00>+ {num3}</color>";
			}
			return $" <color=#FF0000>- {Mathf.Abs(num3)}</color>";
		}
		string GetElementDiffText(float oldValue, float newValue)
		{
			if (old_data == null)
			{
				return string.Empty;
			}
			float num = newValue - oldValue;
			if (Mathf.Approximately(num, 0f))
			{
				return string.Empty;
			}
			if (num > 0f)
			{
				return $" <color=#00FF00>+ {num}</color>";
			}
			return $" <color=#FF0000>- {Mathf.Abs(num)}</color>";
		}
		static bool ShouldShowElementLine(float newValue)
		{
			return newValue > 0f;
		}
	}

	private void AppendMainArrayLines(ref string stats, WeaponClass old_data)
	{
		if (Main == null)
		{
			return;
		}
		for (int i = 0; i < Main.Length; i++)
		{
			WPDT_A wPDT_A = Main[i];
			if (wPDT_A == null || wPDT_A.Index == 0)
			{
				continue;
			}
			string mainArrayLine = GetMainArrayLine(wPDT_A, old_data);
			if (!string.IsNullOrEmpty(mainArrayLine))
			{
				mainArrayLine += AffixTierDisplay.SuffixA(this, wPDT_A, isDotGroup: false);
				if (string.IsNullOrEmpty(stats))
				{
					stats = mainArrayLine;
				}
				else
				{
					stats = stats + "\n" + mainArrayLine;
				}
			}
		}
	}

	private string GetMainArrayLine(WPDT_A mainStat, WeaponClass old_data)
	{
		float number = mainStat.number;
		string valueText = ItemManager.FormatWeaponStatValue(mainStat.Index, number);
		string diffText = GetMainArrayDiffText(mainStat, old_data);
		switch (mainStat.Index)
		{
		case 1:
			return PlainPercent(LOC.MM.GetMain("HealthMax"));
		case 2:
			return PlainPercent(LOC.MM.GetMain("ManaMax"));
		case 3:
			return PlainNumber(LOC.MM.GetMain("health recovery"));
		case 4:
			return PlainNumber(LOC.MM.GetMain("mana recovery"));
		case 5:
			return PlainNumber(LOC.MM.GetMain("ATR_health"));
		case 6:
			return PlainNumber(LOC.MM.GetMain("ATR_mana"));
		case 10:
			return PlainPercent(LOC.MM.GetMain("damage"));
		case 11:
			return PlainPercent(LOC.MM.GetMain("AttackSpeed"));
		case 12:
			return PlainPercent(LOC.MM.GetMain("MoveSpeed"));
		case 13:
			return PlainPercent(LOC.MM.GetMain("BJrate"));
		case 14:
			return PlainPercent(LOC.MM.GetMain("BJDamage"));
		case 15:
			return PlainPercent(LOC.MM.GetMain("CD"));
		case 16:
			return PlainPercent(LOC.MM.GetMain("Character_ManaCostReduction"));
		case 17:
			return PlainPercent(LOC.MM.GetMain("GeDang"));
		case 18:
			return PlainPercent(LOC.MM.GetMain("DamageAnti"));
		case 19:
			return PlainPercent(LOC.MM.GetMain("DOTcut"));
		case 20:
			return PlainPercent(LOC.MM.GetMain("AntiSlow"));
		case 21:
			return PlainPercent(LOC.MM.GetMain("AllChuan"));
		case 22:
			return PlainPercent(LOC.MM.GetMain("AllAnti"));
		case 30:
			return MainText("MainStat_CriticalDamageTakenReduction", new object[1] { valueText });
		case 31:
			return MainText("MainStat_EliteBossDamage", new object[1] { valueText });
		case 32:
			return MainText("MainStat_EliteBossDamageReduction", new object[1] { valueText });
		case 50:
			return PlainPercent(LOC.MM.GetMain("DropRate"));
		case 51:
			return PlainPercent(LOC.MM.GetMain("Projectile Speed"));
		case 52:
			return PlainPercent(LOC.MM.GetMain("Character_AccessoryDamage"));
		case 53:
			return PlainPercent(LOC.MM.GetMain("Character_StunRate"));
		case 54:
			return PlainPercent(LOC.MM.GetMain("Character_PierceRate"));
		case 60:
			return PlainPercent(LOC.MM.GetMain("HealthPrc"));
		case 61:
			return PlainPercent(LOC.MM.GetMain("ManaPrc"));
		case 62:
			return MainText("MainStat_DamageToHealthRecovery", new object[1] { valueText });
		case 63:
			return MainText("MainStat_DamageToManaRecovery", new object[1] { valueText });
		case 80:
			return PlainNumber(DisplayLabel("MainDisplay_Label_GemEffect"));
		case 81:
			return PlainPercent(DisplayLabel("MainDisplay_Label_GemEffect"));
		case 100:
			return PlainPercent(LOC.MM.GetMain("Character_CompHealth"));
		case 101:
			return PlainPercent(LOC.MM.GetMain("Character_CompDamage"));
		case 102:
			return PlainPercent(LOC.MM.GetMain("Character_CompAttackSpeed"));
		case 103:
			return PlainPercent(LOC.MM.GetMain("Character_CompMoveSpeed"));
		case 104:
			return PlainPercent(LOC.MM.GetMain("Character_CompAllResistance"));
		case 150:
			return PlainPercent(DisplayLabel("MainDisplay_Label_WeaponSkillDamage"));
		case 151:
			return PlainPercent(DisplayLabel("MainDisplay_Label_WeaponSkillTriggerChance"));
		case 170:
			return PlainPercent(DisplayLabel("MainDisplay_Label_TempleDuration"));
		case 171:
			return PlainPercent(DisplayLabel("MainDisplay_Label_PotionDuration"));
		case 200:
			return PlainPercent(LOC.MM.GetMain("CD") + " " + DisplayLabel("MainDisplay_Label_Limit"));
		case 201:
			return PlainPercent(LOC.MM.GetMain("GeDang") + " " + DisplayLabel("MainDisplay_Label_Limit"));
		case 202:
			return PlainPercent(LOC.MM.GetMain("DamageAnti") + " " + DisplayLabel("MainDisplay_Label_Limit"));
		case 203:
			return MainText("MainStat_EnemyDamageReductionCap", new object[1] { valueText });
		case 204:
			return MainText("MainStat_EnemyAttackSpeedReductionCap", new object[1] { valueText });
		case 205:
			return MainText("MainStat_EnemyMoveSpeedReductionCap", new object[1] { valueText });
		case 300:
			return PlainPercent(LOC.MM.GetMain("Character_DotDamage"));
		case 301:
			return PlainPercent(DisplayLabel("MainDisplay_Label_DebuffDuration"));
		case 302:
			return PlainNumber(DisplayLabel("MainDisplay_Label_DebuffStacks"));
		case 303:
			return MainText("MainStat_MovingEnemyDotDamageTaken", new object[1] { valueText });
		case 304:
			return MainText("MainStat_EliteEnemyDotDamageTaken", new object[1] { valueText });
		case 305:
			return MainText("MainStat_DifferentDotDamageTaken", new object[1] { valueText });
		case 306:
			return MainText("MainStat_EachDebuffDamageTaken", new object[1] { valueText });
		case 307:
			return Enabled(DisplayLabel("MainDisplay_Label_ExecuteEliteFullAndHeal"));
		case 400:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("damage"));
		case 401:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("AttackSpeed"));
		case 402:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("MoveSpeed"));
		case 403:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("BJrate"));
		case 404:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("BJDamage"));
		case 405:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("HealthPrc"));
		case 406:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("ManaPrc"));
		case 407:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_CompHealth"));
		case 408:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_CompDamage"));
		case 409:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_CompAttackSpeed"));
		case 410:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_CompMoveSpeed"));
		case 411:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_CompAllResistance"));
		case 412:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_DotDamage"));
		case 413:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_TrapDamage"));
		case 414:
			return ContextPercent("MainStat_PerEnhancedWeapon", LOC.MM.GetMain("Character_AccessoryDamage"));
		case 415:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("damage"));
		case 416:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("AttackSpeed"));
		case 417:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("MoveSpeed"));
		case 418:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("BJrate"));
		case 419:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("BJDamage"));
		case 420:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("HealthPrc"));
		case 421:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("ManaPrc"));
		case 422:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_CompHealth"));
		case 423:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_CompDamage"));
		case 424:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_CompAttackSpeed"));
		case 425:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_CompMoveSpeed"));
		case 426:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_CompAllResistance"));
		case 427:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_DotDamage"));
		case 428:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_TrapDamage"));
		case 429:
			return ContextPercent("MainStat_PerWeaponWithSkill", LOC.MM.GetMain("Character_AccessoryDamage"));
		case 430:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("damage"));
		case 431:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("AttackSpeed"));
		case 432:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("MoveSpeed"));
		case 433:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("BJrate"));
		case 434:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("BJDamage"));
		case 435:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("HealthPrc"));
		case 436:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("ManaPrc"));
		case 437:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_CompHealth"));
		case 438:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_CompDamage"));
		case 439:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_CompAttackSpeed"));
		case 440:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_CompMoveSpeed"));
		case 441:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_CompAllResistance"));
		case 442:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_DotDamage"));
		case 443:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_TrapDamage"));
		case 444:
			return ContextPercent("MainStat_PerTransmutedWeapon", LOC.MM.GetMain("Character_AccessoryDamage"));
		case 445:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("damage"));
		case 446:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("AttackSpeed"));
		case 447:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("MoveSpeed"));
		case 448:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("Character_CompHealth"));
		case 449:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("Character_CompDamage"));
		case 450:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("Character_CompAttackSpeed"));
		case 451:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("Character_CompAllResistance"));
		case 452:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("Character_TrapDamage"));
		case 453:
			return ContextPercent("MainStat_PerWeaponSkillPoint", LOC.MM.GetMain("Character_AccessoryDamage"));
		case 454:
			return ContextNumber("MainStat_PerWeaponSkillPointNumber", DisplayLabel("MainDisplay_Label_AccessoryOrbCount"));
		case 455:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("damage"));
		case 456:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("AttackSpeed"));
		case 457:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("MoveSpeed"));
		case 458:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("Character_CompHealth"));
		case 459:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("Character_CompDamage"));
		case 460:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("Character_CompAttackSpeed"));
		case 461:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("Character_CompAllResistance"));
		case 462:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("Character_TrapDamage"));
		case 463:
			return ContextPercent("MainStat_PerSocketedGem", LOC.MM.GetMain("Character_AccessoryDamage"));
		case 464:
			return ContextNumber("MainStat_PerSocketedGemNumber", DisplayLabel("MainDisplay_Label_AccessoryOrbCount"));
		case 500:
			return WhenBelow(LOC.MM.GetMain("Health"), "20", LOC.MM.GetMain("damage"));
		case 501:
			return WhenBelow(LOC.MM.GetMain("Health"), "50", LOC.MM.GetMain("damage"));
		case 502:
			return WhenAbove(LOC.MM.GetMain("Health"), "90", LOC.MM.GetMain("damage"));
		case 503:
			return WhenFull(LOC.MM.GetMain("Health"), LOC.MM.GetMain("damage"));
		case 504:
			return MainText("MainStat_LowHealthHitSkillChance", new object[1] { valueText });
		case 505:
			return MainText("MainStat_FullHealthHitSkillChance", new object[1] { valueText });
		case 506:
			return WhenBelow(LOC.MM.GetMain("Health"), "20", LOC.MM.GetMain("DamageAnti"));
		case 507:
			return WhenBelow(LOC.MM.GetMain("Health"), "50", LOC.MM.GetMain("DamageAnti"));
		case 508:
			return Enabled(DisplayLabel("MainDisplay_Label_LowHealthCritImmunity"));
		case 509:
			return WhenBelow(LOC.MM.GetMain("Mana"), "20", LOC.MM.GetMain("damage"));
		case 510:
			return WhenBelow(LOC.MM.GetMain("Mana"), "50", LOC.MM.GetMain("damage"));
		case 511:
			return WhenAbove(LOC.MM.GetMain("Mana"), "90", LOC.MM.GetMain("damage"));
		case 512:
			return WhenFull(LOC.MM.GetMain("Mana"), LOC.MM.GetMain("damage"));
		case 513:
			return MainText("MainStat_LowManaHitSkillChance", new object[1] { valueText });
		case 514:
			return MainText("MainStat_FullManaHitSkillChance", new object[1] { valueText });
		case 550:
			return StateWhile(DisplayLabel("MainDisplay_Label_Moving"), LOC.MM.GetMain("damage"));
		case 551:
			return StateWhile(DisplayLabel("MainDisplay_Label_Moving"), LOC.MM.GetMain("AttackSpeed"));
		case 552:
			return StateWhile(DisplayLabel("MainDisplay_Label_Moving"), LOC.MM.GetMain("GeDang"));
		case 553:
			return StateWhile(DisplayLabel("MainDisplay_Label_Still"), LOC.MM.GetMain("damage"));
		case 554:
			return StateWhile(DisplayLabel("MainDisplay_Label_Still"), LOC.MM.GetMain("AttackSpeed"));
		case 555:
			return StateWhile(DisplayLabel("MainDisplay_Label_Still"), LOC.MM.GetMain("DamageAnti"));
		case 556:
			return StateWhile(DisplayLabel("MainDisplay_Label_Still"), LOC.MM.GetMain("HealthPrc"));
		case 557:
			return StateWhile(DisplayLabel("MainDisplay_Label_Still"), LOC.MM.GetMain("ManaPrc"));
		case 558:
			return StateWhile(DisplayLabel("MainDisplay_Label_Dashing"), LOC.MM.GetMain("damage"));
		case 559:
			return StateWhile(DisplayLabel("MainDisplay_Label_Dashing"), LOC.MM.GetMain("DamageAnti"));
		case 600:
			return ConvertToDamage(LOC.MM.GetMain("HealthMax"));
		case 601:
			return ConvertToDamage(DisplayLabel("MainDisplay_Label_LostHealth"));
		case 602:
			return ConvertToDamage(LOC.MM.GetMain("ManaMax"));
		case 603:
			return ConvertToDamage(LOC.MM.GetMain("Mana"));
		case 604:
			return ConvertToDamage(DisplayLabel("MainDisplay_Label_MissingMana"));
		case 610:
			return ConvertToElementDamage(LOC.MM.GetMain("HealthMax"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 611:
			return ConvertToElementDamage(LOC.MM.GetMain("ManaMax"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 612:
			return ConvertToElementDamage(LOC.MM.GetMain("CD"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 613:
			return ConvertToElementDamage(GetElementMainLabel(mainStat.EL, ElementMainLabel.Resistance), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 614:
			return ConvertToElementDamage(GetElementMainLabel(mainStat.EL, ElementMainLabel.Penetration), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 615:
			return ConvertToElementDamage(LOC.MM.GetMain("GeDang"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 616:
			return ConvertToElementDamage(LOC.MM.GetMain("BJrate"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 617:
			return ConvertToElementDamage(LOC.MM.GetMain("DamageAnti"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 618:
			return ConvertToElementDamage(LOC.MM.GetMain("Character_PierceRate"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 650:
			return ConvertToCompanionDamage(LOC.MM.GetMain("CD"));
		case 651:
			return ConvertToCompanionDamage(LOC.MM.GetMain("AttackSpeed"));
		case 652:
			return ConvertToDamage(LOC.MM.GetMain("MoveSpeed"));
		case 653:
			return ConvertToAttackSpeed(LOC.MM.GetMain("MoveSpeed"));
		case 654:
			return MainText("MainStat_ExcessCritToCritDamage", Array.Empty<object>());
		case 655:
			return ConvertToCritDamage(GetElementMainLabel(mainStat.EL, ElementMainLabel.Penetration));
		case 700:
			return MainText("MainStat_EachMissingHealthDamage", new object[1] { valueText });
		case 701:
			return MainText("MainStat_EachMissingManaDropRate", new object[1] { valueText });
		case 750:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageManaTradeoff"));
		case 751:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageHurtTradeoff"));
		case 752:
			return Enabled(DisplayLabel("MainDisplay_Label_DotDirectTradeoff"));
		case 753:
			return Enabled(DisplayLabel("MainDisplay_Label_BlockHealthTradeoff"));
		case 800:
			return OnSkillCast(LOC.MM.GetMain("damage"), "4", "5");
		case 801:
			return OnSkillCast(LOC.MM.GetMain("damage"), "3", "6");
		case 802:
			return OnSkillCast(LOC.MM.GetMain("AttackSpeed"), "3", "6");
		case 803:
			return OnSkillCast(LOC.MM.GetMain("AttackSpeed"), "3", "8");
		case 804:
			return OnSkillCast(LOC.MM.GetMain("AllChuan"), "3", "6");
		case 805:
			return OnSkillCast(LOC.MM.GetMain("BJrate"), "3", "10");
		case 806:
			return OnSkillCast(LOC.MM.GetMain("BJDamage"), "3", "12");
		case 807:
			return OnSkillCast(LOC.MM.GetMain("Character_DotDamage"), "3", "6");
		case 808:
			return OnSkillCast(LOC.MM.GetMain("Character_DotDamage"), "4", "8");
		case 850:
			return PerBuffCompanionDamage();
		case 851:
			return PerDebuffElementDamage();
		case 852:
			return PerDebuffMoveSpeed();
		case 853:
			return ClearDebuffEvery("5");
		case 854:
			return ClearDebuffEvery("3");
		case 855:
			return OnBlockStack(LOC.MM.GetMain("damage"), "6", "6");
		case 856:
			return AfterUltimateOtherElementDamage();
		case 857:
			return MainText("MainStat_AfterGemPickupMoveSpeed", new object[1] { valueText });
		case 858:
			return NoSkillDamageStack("5");
		case 859:
			return NoSkillDamageStack("8");
		case 860:
			return OnDisplacementSkillDamage();
		case 861:
			return MovingChargeDamage();
		case 862:
			return FatalCounterattack();
		case 863:
			return FatalRage();
		case 864:
			return FatalStealth();
		case 1000:
		case 1001:
		case 1002:
		case 1003:
		case 1004:
		case 1005:
		case 1006:
		case 1007:
			return PerCompanionStat(GetCompanionMainLabel(mainStat.Index));
		case 1010:
		case 1011:
			return PerCompanionStat(GetElementMainLabel(mainStat.EL, (mainStat.Index != 1010) ? ElementMainLabel.Penetration : ElementMainLabel.Damage));
		case 1020:
		case 1021:
		case 1022:
		case 1023:
			return PerCompanionStat(GetCompanionMainLabel(mainStat.Index));
		case 1024:
		case 1025:
		case 1026:
		case 1027:
		case 1028:
		case 1029:
		case 1030:
		case 1031:
			return PerCompanionTypeStat(GetCompanionMainLabel(mainStat.Index));
		case 1040:
			return PerCompanionTypeStat(GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage));
		case 1041:
			return PerCompanionTypeStat(GetElementMainLabel(mainStat.EL, ElementMainLabel.Penetration));
		case 1050:
		case 1051:
		case 1052:
		case 1053:
		case 1054:
			return PerCompanionTypeStat(GetCompanionMainLabel(mainStat.Index));
		case 1100:
		case 1101:
		case 1102:
		case 1103:
		case 1104:
		case 1105:
		case 1106:
		case 1107:
		case 1108:
		case 1109:
		case 1110:
		case 1111:
		case 1112:
		case 1113:
		case 1114:
		case 1115:
		case 1116:
		case 1117:
		case 1118:
		case 1119:
		case 1120:
		case 1121:
		case 1122:
		case 1123:
		case 1124:
		case 1125:
		case 1126:
		case 1127:
		case 1128:
		case 1129:
		case 1130:
		case 1131:
		case 1132:
		case 1133:
		case 1134:
		case 1135:
		case 1136:
		case 1137:
		case 1138:
		case 1139:
		case 1140:
		case 1141:
		case 1142:
		case 1143:
		case 1144:
		case 1145:
		case 1146:
			return OnFieldPlayerDamage(GetOnFieldObjectName(mainStat.Index));
		case 1150:
			return OnFieldCompanionDamage(DisplayLabel("MainDisplay_Label_Trap"));
		case 1200:
			return NearbyEnemyPlayerBonus(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("damage"), "20");
		case 1201:
			return NearbyEnemyPlayerBonus(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("damage"), "48");
		case 1202:
			return NearbyEnemyPlayerBonus(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("DamageAnti"), "9");
		case 1203:
			return NearbyEnemyPlayerBonus(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("GeDang"), "12");
		case 1204:
			return NearbyEnemyPlayerBonus(DisplayLabel("MainDisplay_Label_EliteEnemy"), LOC.MM.GetMain("damage"), "15");
		case 1205:
			return NearbyEnemyPlayerBonus(DisplayLabel("MainDisplay_Label_EliteEnemy"), LOC.MM.GetMain("AttackSpeed"), "24");
		case 1206:
			return NearbyEnemyPlayerBonus(DisplayLabel("MainDisplay_Label_EliteEnemy"), LOC.MM.GetMain("BJDamage"), "16");
		case 1250:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("damage"), "3", "8");
		case 1251:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("damage"), "5", "10");
		case 1252:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("AttackSpeed"), "3", "5");
		case 1253:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("AttackSpeed"), "5", "8");
		case 1260:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), GetElementMainLabel(mainStat.EL, ElementMainLabel.Damage), "6", "8");
		case 1270:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("Character_CompDamage"), "2", "4");
		case 1271:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("Character_CompDamage"), "3", "6");
		case 1272:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("Character_CompAttackSpeed"), "2", "5");
		case 1273:
			return OnKillStatStack(LOC.MM.GetMain("enemy"), LOC.MM.GetMain("Character_CompAttackSpeed"), "3", "6");
		case 1274:
			return OnKillStatStack(DisplayLabel("MainDisplay_Label_EliteEnemy"), LOC.MM.GetMain("damage"), "5", "3");
		case 1275:
			return OnKillStatStack(DisplayLabel("MainDisplay_Label_EliteEnemy"), LOC.MM.GetMain("AllAnti"), "3", "3");
		case 1276:
			return OnKillRefreshAllSkills();
		case 1300:
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_ElementalSkill"), GetElementName(mainStat.EL), valueText, "8") + diffText;
		case 1301:
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_ElementalSkill"), GetElementName(mainStat.EL), valueText, "5") + diffText;
		case 1302:
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_ElementalSkill"), GetElementName(mainStat.EL), valueText, "3") + diffText;
		case 1330:
			return MainText(GetBurnLifeDescriptionKey(mainStat.EL), new object[1] { valueText });
		case 1350:
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_ElementalIgnore"), GetElementName(mainStat.EL));
		case 1360:
			return Enabled(DisplayLabel("MainDisplay_Label_AllAttacksIgnoreReduction"));
		case 1370:
			return MainText("MainStat_EnemyBelowHpDamageTaken", new object[2] { "20", valueText });
		case 1371:
			return MainText("MainStat_EnemyBelowHpDamageTaken", new object[2] { "50", valueText });
		case 1372:
			return MainText("MainStat_EnemyAboveHpDamageTaken", new object[2] { "60", valueText });
		case 1373:
			return MainText("MainStat_EnemyFullHpDamageTaken", new object[1] { valueText });
		case 1374:
			return MainText("MainStat_FullHpEnemyAlwaysCrit", new object[1] { valueText });
		case 1390:
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_Distance"), "5", valueText, LOC.MM.GetMain("damage")) + diffText;
		case 1391:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageIncreasesWithDistance"));
		case 1395:
			return MainText("MainStat_CriticalExplosion", new object[1] { valueText });
		case 1396:
			return MainText("MainStat_CriticalCorpseExplosion", new object[1] { valueText });
		case 1397:
			return MainText("MainStat_CriticalInstantKill", new object[1] { valueText });
		case 1500:
			return PlainNumber(DisplayLabel("MainDisplay_Label_AccessoryOrbBaseCount"));
		case 1501:
			return Enabled(DisplayLabel("MainDisplay_Label_AccessoryOrbCountDoubled"));
		case 1502:
			return MainText("MainStat_AccessoryOrbCountAboveDamage", new object[2] { "80", valueText });
		case 1503:
			return MainText("MainStat_AccessoryOrbCountAboveDamage", new object[2] { "120", valueText });
		case 1504:
			return PlainPercent(DisplayLabel("MainDisplay_Label_AccessoryOrbDamage"));
		case 1505:
			return MainText("MainStat_FullManaAccessoryOrbDamage", new object[1] { valueText });
		case 1506:
			return MinusPercent(DisplayLabel("MainDisplay_Label_AccessoryOrbSpawnInterval"));
		case 1507:
			return PlainPercent(DisplayLabel("MainDisplay_Label_SummonBowDamage"));
		case 1508:
			return MinusPercent(DisplayLabel("MainDisplay_Label_SummonBowInterval"));
		case 1509:
			return MainText("MainStat_EachSummonBowAccessoryDamage", new object[1] { valueText });
		case 1510:
			return MainText("MainStat_EachSummonBowDamageReduction", new object[1] { valueText });
		case 1600:
			return PlainPercent(LOC.MM.GetMain("Character_TrapDamage"));
		case 1601:
			return PlainPercent(DisplayLabel("MainDisplay_Label_TrapDuration"));
		case 1602:
			return PlainPercent(DisplayLabel("MainDisplay_Label_TotemEffect"));
		case 1603:
			return PlainPercent(DisplayLabel("MainDisplay_Label_TotemDuration"));
		case 1604:
			return Enabled(DisplayLabel("MainDisplay_Label_PlayerFullTotemEffect"));
		case 1800:
			return MainText("MainStat_NoDebuffCritDamage", new object[1] { valueText });
		case 1801:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageAndMana"));
		case 1802:
			return MainText("MainStat_ManaCostToSelfHeal", new object[1] { valueText });
		case 1803:
			return MainText("MainStat_ManaRegenToHealthRegen", Array.Empty<object>());
		case 1804:
			return Enabled(DisplayLabel("MainDisplay_Label_CompanionSamePercentHealthRegen"));
		case 1805:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageReflection"));
		case 1806:
			return MainText("MainStat_MaxManaExtraHitDamage", new object[1] { valueText });
		case 1807:
			return Enabled(DisplayLabel("MainDisplay_Label_Turtle"));
		case 1808:
			return MainText("MainStat_BlockHitSkillChance", new object[1] { valueText });
		case 1809:
			return Enabled(DisplayLabel("MainDisplay_Label_BloodLost"));
		case 1810:
			return MainText("MainStat_GroundDotImmunity", Array.Empty<object>());
		case 1811:
			return MainText("MainStat_CompanionDebuffUnaffected", Array.Empty<object>());
		case 1812:
			return MainText("MainStat_CompanionGroundDotImmunity", Array.Empty<object>());
		case 1813:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageHpCost1"));
		case 1814:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageHpCost3"));
		case 1815:
			return MainText("MainStat_CompanionSharePlayerDamage", new object[1] { valueText });
		case 1816:
			return Enabled(DisplayLabel("MainDisplay_Label_BladeSoulCountDoubled"));
		case 1817:
			return MainText("MainStat_DifferentElementHitExtraDamage", new object[1] { valueText });
		case 1818:
			return PlainPercent(LOC.MM.GetMain("Character_ExplosionSkillRange"));
		case 1819:
			return MainText("MainStat_TotemBuffSkillRange", new object[1] { valueText });
		case 1820:
			return Enabled(DisplayLabel("MainDisplay_Label_DamageFromGold"));
		case 1821:
			return Enabled(DisplayLabel("MainDisplay_Label_Automatic"));
		case 1822:
			return Enabled(DisplayLabel("MainDisplay_Label_DeathExplosion"));
		case 1900:
			return WhenBelowEnabled(LOC.MM.GetMain("Health"), "20", DisplayLabel("MainDisplay_Label_AutoHealthPotion"));
		case 1901:
			return WhenBelowEnabled(LOC.MM.GetMain("Mana"), "10", DisplayLabel("MainDisplay_Label_AutoManaPotion"));
		case 1905:
			return Enabled(DisplayLabel("MainDisplay_Label_CompanionHealthFromPotion"));
		case 1910:
			return MainText("MainStat_PermanentPotionExtraHealth", new object[1] { valueText });
		case 1911:
			return MainText("MainStat_PermanentPotionExtraMana", new object[1] { valueText });
		case 1912:
			return MainText("MainStat_PermanentPotionExtraDamage", new object[1] { valueText });
		case 1950:
			return PlainPercent(LOC.MM.GetMain("Character_AutoPickupRange"));
		case 1951:
			return PlainPercent(LOC.MM.GetMain("Character_FairyPickupRange"));
		case 1952:
			return MainText("MainStat_FairyDismantlePrice", new object[1] { valueText });
		case 1953:
			return EachOwnedFairyBonus(LOC.MM.GetMain("damage"));
		case 1954:
			return EachOwnedFairyBonus(LOC.MM.GetMain("DropRate"));
		case 1955:
			return MinusPercent(DisplayLabel("MainDisplay_Label_FairySkillInterval"));
		default:
			return string.Empty;
		}
		string AfterUltimateOtherElementDamage()
		{
			return MainText("MainStat_AfterUltimateOtherElementDamage", new object[1] { valueText });
		}
		string ClearDebuffEvery(string seconds)
		{
			return MainText("MainStat_ClearDebuffEvery", new object[2] { seconds, valueText });
		}
		string ContextNumber(string key, string label)
		{
			return MainText(key, new object[2] { label, valueText });
		}
		string ContextPercent(string key, string label)
		{
			return MainText(key, new object[2] { label, valueText });
		}
		string ConvertToAttackSpeed(string source)
		{
			return MainText("MainStat_ConvertToAttackSpeed", new object[2] { source, valueText });
		}
		string ConvertToCompanionDamage(string source)
		{
			return MainText("MainStat_ConvertToCompanionDamage", new object[2] { source, valueText });
		}
		string ConvertToCritDamage(string source)
		{
			return MainText("MainStat_ConvertToCritDamage", new object[2] { source, valueText });
		}
		string ConvertToDamage(string source)
		{
			return MainText("MainStat_ConvertToDamage", new object[2] { source, valueText });
		}
		string ConvertToElementDamage(string source, string target)
		{
			return MainText("MainStat_ConvertToElementDamage", new object[3] { source, valueText, target });
		}
		static string DisplayLabel(string key)
		{
			return LOC.MM.Get("MainDisplay_FY." + key);
		}
		string EachOwnedFairyBonus(string label)
		{
			return MainText("MainStat_EachOwnedFairyBonus", new object[2] { valueText, label });
		}
		static string Enabled(string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_Enabled"), label);
		}
		string FatalCounterattack()
		{
			return MainText("MainStat_FatalCounterattack", new object[1] { valueText });
		}
		string FatalRage()
		{
			return MainText("MainStat_FatalRage", new object[1] { valueText });
		}
		string FatalStealth()
		{
			return MainText("MainStat_FatalStealth", new object[1] { valueText });
		}
		string MainText(string key, object[] args)
		{
			return ItemDisplayText(key, args) + diffText;
		}
		string MinusPercent(string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_MinusPercent"), valueText, label) + diffText;
		}
		string MovingChargeDamage()
		{
			return MainText("MainStat_MovingChargeDamage", new object[1] { valueText });
		}
		string NearbyEnemyPlayerBonus(string thing, string label, string max)
		{
			return MainText("MainStat_NearbyEnemyPlayerBonus", new object[4] { thing, valueText, label, max });
		}
		string NoSkillDamageStack(string layers)
		{
			return MainText("MainStat_NoSkillDamageStack", new object[2] { valueText, layers });
		}
		string OnBlockStack(string label, string duration, string layers)
		{
			return MainText("MainStat_OnBlockStack", new object[4] { label, valueText, duration, layers });
		}
		string OnDisplacementSkillDamage()
		{
			return MainText("MainStat_OnDisplacementSkillDamage", new object[1] { valueText });
		}
		string OnFieldCompanionDamage(string thing)
		{
			return MainText("MainStat_OnFieldCompanionDamage", new object[2] { thing, valueText });
		}
		string OnFieldPlayerDamage(string thing)
		{
			return MainText("MainStat_OnFieldPlayerDamage", new object[2] { thing, valueText });
		}
		string OnKillRefreshAllSkills()
		{
			return MainText("MainStat_OnKillRefreshAllSkills", new object[1] { valueText });
		}
		string OnKillStatStack(string thing, string label, string duration, string layers)
		{
			return MainText("MainStat_OnKillStatStack", new object[5] { thing, valueText, label, duration, layers });
		}
		string OnSkillCast(string label, string duration, string layers)
		{
			return MainText("MainStat_OnSkillCast", new object[4] { label, valueText, duration, layers });
		}
		string PerBuffCompanionDamage()
		{
			return MainText("MainStat_PerBuffCompanionDamage", new object[1] { valueText });
		}
		string PerCompanionStat(string label)
		{
			return MainText("MainStat_PerCompanionStat", new object[2] { valueText, label });
		}
		string PerCompanionTypeStat(string label)
		{
			return MainText("MainStat_PerCompanionTypeStat", new object[2] { valueText, label });
		}
		string PerDebuffElementDamage()
		{
			return MainText("MainStat_PerDebuffElementDamage", new object[1] { valueText });
		}
		string PerDebuffMoveSpeed()
		{
			return MainText("MainStat_PerDebuffMoveSpeed", new object[1] { valueText });
		}
		string PlainNumber(string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_PlainNumber"), valueText, label) + diffText;
		}
		string PlainPercent(string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_PlainPercent"), valueText, label) + diffText;
		}
		string StateWhile(string state, string label)
		{
			return MainText("MainStat_StateWhile", new object[3] { state, valueText, label });
		}
		string WhenAbove(string resource, string threshold, string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_WhenAbove"), resource, threshold, valueText, label) + diffText;
		}
		string WhenBelow(string resource, string threshold, string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_WhenBelow"), resource, threshold, valueText, label) + diffText;
		}
		static string WhenBelowEnabled(string resource, string threshold, string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_WhenBelowEnabled"), resource, threshold, label);
		}
		string WhenFull(string resource, string label)
		{
			return string.Format(LOC.MM.Get("MainDisplay_FY.MainStat_WhenFull"), resource, valueText, label) + diffText;
		}
	}

	private string GetMainArrayDiffText(WPDT_A mainStat, WeaponClass old_data)
	{
		if (old_data == null || old_data.Main == null)
		{
			return string.Empty;
		}
		for (int i = 0; i < old_data.Main.Length; i++)
		{
			WPDT_A wPDT_A = old_data.Main[i];
			if (wPDT_A != null && wPDT_A.Index == mainStat.Index && wPDT_A.EL == mainStat.EL)
			{
				float num = mainStat.number - wPDT_A.number;
				if (Mathf.Approximately(num, 0f))
				{
					return string.Empty;
				}
				string text = ItemManager.FormatWeaponStatValue(mainStat.Index, Mathf.Abs(num));
				if (!(num > 0f))
				{
					return " <color=#FF0000>- " + text + "</color>";
				}
				return " <color=#00FF00>+ " + text + "</color>";
			}
		}
		return string.Empty;
	}

	private string GetElementMainLabel(int element, ElementMainLabel label)
	{
		DamageType type = (DamageType)Mathf.Clamp(element, 0, 5);
		switch (label)
		{
		case ElementMainLabel.Penetration:
			return LOC.MM.GetMain(SWS.El_Chuan(type));
		case ElementMainLabel.Resistance:
		case ElementMainLabel.Reduction:
			return LOC.MM.GetMain(SWS.El_Anti(type));
		default:
			return LOC.MM.GetMain(SWS.El_DMG(type));
		}
	}

	private string GetElementName(int element)
	{
		return Mathf.Clamp(element, 0, 5) switch
		{
			0 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_Fire"), 
			1 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_Frost"), 
			2 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_Lightning"), 
			3 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_Poison"), 
			4 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_Physical"), 
			_ => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_Shadow"), 
		};
	}

	private string GetBurnLifeDescriptionKey(int element)
	{
		return Mathf.Clamp(element, 0, 5) switch
		{
			0 => "MainStat_BurnLife_Fire", 
			1 => "MainStat_BurnLife_Frost", 
			2 => "MainStat_BurnLife_Thunder", 
			3 => "MainStat_BurnLife_Poison", 
			4 => "MainStat_BurnLife_Physics", 
			_ => "MainStat_BurnLife_Shadow", 
		};
	}

	private string GetCompanionMainLabel(int index)
	{
		switch (index)
		{
		case 1000:
		case 1024:
			return LOC.MM.GetMain("damage");
		case 1001:
		case 1025:
			return LOC.MM.GetMain("AttackSpeed");
		case 1002:
		case 1026:
			return LOC.MM.GetMain("MoveSpeed");
		case 1003:
		case 1027:
			return LOC.MM.GetMain("HealthMax");
		case 1004:
		case 1028:
			return LOC.MM.GetMain("ManaMax");
		case 1005:
		case 1029:
			return LOC.MM.GetMain("DamageAnti");
		case 1006:
		case 1030:
			return LOC.MM.GetMain("DropRate");
		case 1007:
		case 1031:
			return LOC.MM.GetMain("Character_AccessoryDamage");
		case 1011:
		case 1041:
			return LOC.MM.GetMain("AllChuan");
		case 1020:
		case 1050:
			return LOC.MM.GetMain("Character_CompHealth");
		case 1021:
		case 1051:
			return LOC.MM.GetMain("Character_CompDamage");
		case 1022:
		case 1052:
			return LOC.MM.GetMain("Character_CompAttackSpeed");
		case 1023:
		case 1053:
			return LOC.MM.GetMain("Character_CompAllResistance");
		case 1054:
			return LOC.MM.GetMain("Character_DotDamage");
		default:
			return LOC.MM.GetMain("Comp");
		}
	}

	private string GetOnFieldObjectName(int index)
	{
		return index switch
		{
			1100 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Orb"), 
			1101 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Arrow"), 
			1102 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Javelin"), 
			1103 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_FlyingSword"), 
			1104 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_FlyingSpear"), 
			1105 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_FlyingInsect"), 
			1106 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Fairy"), 
			1107 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Dart"), 
			1108 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Claw"), 
			1109 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Hammer"), 
			1110 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Scythe"), 
			1111 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_IceCrystal"), 
			1112 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_DeathReap"), 
			1113 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_SkeletonOrb"), 
			1114 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_LaserProjectile"), 
			1115 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Lightning"), 
			1116 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_SwordEnergy"), 
			1117 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_AccessoryOrb"), 
			1118 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Laser"), 
			1119 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_BladeSoulSword"), 
			1120 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_SwordMarkSlash"), 
			1121 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_GroundSpike"), 
			1122 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_BallLightning"), 
			1123 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_IonSpark"), 
			1124 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_GroundSkill"), 
			1125 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Domain"), 
			1126 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Tornado"), 
			1127 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_LightningStrike"), 
			1128 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_MagicCircle"), 
			1129 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Nova"), 
			1130 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_DeathExplosion"), 
			1131 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_CorpseExplosion"), 
			1132 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_FallingObject"), 
			1133 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_TrapOrb"), 
			1134 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Trap"), 
			1135 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_LaserField"), 
			1136 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Volcano"), 
			1137 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_EnergyCore"), 
			1138 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Hydra"), 
			1139 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Totem"), 
			1140 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Teleport"), 
			1141 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Transformation"), 
			1142 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Buff"), 
			1143 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Path"), 
			1144 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Dash"), 
			1145 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_Shield"), 
			1146 => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_FieldObject_SummonedBowArrow"), 
			_ => LOC.MM.Get("MainDisplay_FY.MainDisplay_Label_Skill"), 
		};
	}

	public string GetDot()
	{
		string stats = string.Empty;
		if (DOT == null)
		{
			return stats;
		}
		for (int i = 0; i < DOT.Length; i++)
		{
			WPDT_A wPDT_A = DOT[i];
			if (wPDT_A != null && wPDT_A.Index != 0)
			{
				string text = GetDotArrayLine(wPDT_A);
				if (!string.IsNullOrEmpty(text))
				{
					text += AffixTierDisplay.SuffixA(this, wPDT_A, isDotGroup: true);
				}
				AppendTextBlock(ref stats, text);
			}
		}
		return stats;
	}

	private string GetDotArrayLine(WPDT_A dotStat, bool requireOwnedSkill = false)
	{
		if (dotStat == null)
		{
			return string.Empty;
		}
		DamageType damageType = SWS.DMtype(dotStat.EL);
		string text = DamageColor.Colors[damageType];
		SkillData_Dot_Father dotSkill;
		SkillXiData xiData;
		if (requireOwnedSkill)
		{
			if (!TryGetDotSkill(dotStat.EL, requireOwnedSkill: true, out dotSkill, out xiData))
			{
				return string.Empty;
			}
		}
		else
		{
			TryGetDotSkill(dotStat.EL, requireOwnedSkill: false, out dotSkill, out xiData);
		}
		string text2 = ItemManager.FormatWeaponStatValue(dotStat.Index, dotStat.number);
		string text3 = ((dotSkill != null) ? GetColorSkillName(dotSkill.IndexName, text) : ("<color=" + text + ">" + LOC.MM.GetMain(SWS.El_Name(damageType)) + "</color>"));
		return dotStat.Index switch
		{
			2000 => DotText("DotStat_ExtraLayerOnApply", text3, text2), 
			2001 => DotText("DotStat_CritAddsOneLayer", text3), 
			2002 => DotText("DotStat_CounterAddsOneLayer", text3, text2), 
			2003 => DotText("DotStat_DamageTickAddsLayer", text3, text2), 
			2004 => DotText("DotStat_ApplyFullStackChance", text3, text2), 
			2005 => DotText("DotStat_DoubleLayer", text3), 
			2100 => GetDotDeathExplosionLine(dotSkill, xiData, text, "DotStat_InfectEnabled"), 
			2101 => GetDotDeathExplosionLine(dotSkill, xiData, text, "DotStat_InfectLayer", text2), 
			2102 => GetDotDeathExplosionLine(dotSkill, xiData, text, "DotStat_InfectAllLayers"), 
			2200 => DotText("DotStat_ExplodeEveryThreeSeconds", text3), 
			2201 => DotText("DotStat_ExplosionHalfLayer", text3), 
			2202 => DotText("DotStat_ExplosionAddsLayer", text3, text2), 
			2203 => DotText("DotStat_ExplosionInstantKillChance", text3, text2), 
			2300 => DotText("DotStat_Vulnerable", text3, text2), 
			2301 => DotText("DotStat_TearWound", text3), 
			2302 => DotText("DotStat_Silence", text3), 
			2303 => DotText("DotStat_ConfuseDeathExplosion", text3, text2), 
			2304 => DotText("DotStat_CurseNoCritNoDebuff", text3), 
			2305 => DotText("DotStat_StunAddsLayer", text3, text2), 
			2306 => DotText("DotStat_EliteDeathRandomExplosion", text3, text2), 
			2400 => DotText("DotStat_CanCrit", text3), 
			2401 => DotText("DotStat_ChildSkillDamage", text3, text2), 
			2402 => DotText("DotStat_EightLayerHpLoss", text3, text2), 
			2450 => DotText("DotStat_CompanionDamageAgainstDot", text3, text2), 
			2500 => DotText("DotStat_PlayerDamagePerNearbyDotEnemy", text3, text2), 
			2501 => DotText("DotStat_DamageBelowHalfHealth", text3, text2), 
			2550 => DotText("DotStat_DamageWhenPlayerLowHealth", text3, text2), 
			2551 => DotText("DotStat_DamageWhenPlayerFullHealth", text3, text2), 
			2552 => DotText("DotStat_DamageWhenPlayerLowMana", text3, text2), 
			2600 => DotText("DotStat_FreezeForever", text3, text2), 
			2601 => DotText("DotStat_FreezeCutMaxHealth", text3, text2), 
			2602 => DotText("DotStat_FreezeChanceBelowThirty", text3, text2), 
			2603 => DotText("DotStat_FrozenEnemyDamageTaken", text3, text2), 
			2604 => DotText("DotStat_FrozenForeverDot", text3), 
			_ => string.Empty, 
		};
	}

	private static string DotText(string key, params object[] args)
	{
		return ItemDisplayText(key, args);
	}

	private static string SKText(string key, params object[] args)
	{
		return ItemDisplayText(key, args);
	}

	private static string ItemDisplayText(string key, params object[] args)
	{
		string text = LOC.MM.Get("MainDisplay_FY." + key);
		try
		{
			return string.Format(text, args);
		}
		catch (FormatException ex)
		{
			Debug.LogWarning("MainDisplay_FY." + key + " format error: " + ex.Message + ". Text: " + text);
			return text;
		}
	}

	private static string GetDotDeathExplosionLine(SkillData_Dot_Father dotSkill, SkillXiData xiData, string color, string key, params object[] extraArgs)
	{
		if (!TryGetDeathExplosionDotChild(dotSkill, xiData, out var skillName))
		{
			return string.Empty;
		}
		object[] array = new object[1 + ((extraArgs != null) ? extraArgs.Length : 0)];
		array[0] = GetColorSkillName(skillName, color);
		if (extraArgs != null)
		{
			for (int i = 0; i < extraArgs.Length; i++)
			{
				array[i + 1] = extraArgs[i];
			}
		}
		return DotText(key, array);
	}

	private static bool TryGetOwnedDotSkill(int element, out SkillData_Dot_Father dotSkill, out SkillXiData xiData)
	{
		return TryGetDotSkill(element, requireOwnedSkill: true, out dotSkill, out xiData);
	}

	private static bool TryGetDotSkill(int element, bool requireOwnedSkill, out SkillData_Dot_Father dotSkill, out SkillXiData xiData)
	{
		dotSkill = null;
		xiData = null;
		if (!SingletonMonoScope<TalentManager>.HasInstance || SingletonMonoScope<TalentManager>.Instance.XiData == null)
		{
			return false;
		}
		DamageType damageType = SWS.DMtype(element);
		SkillXiData[] xiData2 = SingletonMonoScope<TalentManager>.Instance.XiData;
		foreach (SkillXiData skillXiData in xiData2)
		{
			if (skillXiData == null || skillXiData.Dot_F == null)
			{
				continue;
			}
			foreach (KeyValuePair<string, SkillData_Dot_Father> item in skillXiData.Dot_F)
			{
				SkillData_Dot_Father value = item.Value;
				if (value != null && value.damageType == damageType && (!requireOwnedSkill || value.Level_Base > 0))
				{
					dotSkill = value;
					xiData = skillXiData;
					return true;
				}
			}
		}
		return false;
	}

	private static bool TryGetDeathExplosionDotChild(SkillData_Dot_Father dotSkill, SkillXiData xiData, out string skillName)
	{
		skillName = string.Empty;
		if (dotSkill == null || xiData == null || xiData.Dot_S == null)
		{
			return false;
		}
		if (!TryGetDeathExplosionDotChild(dotSkill.SonA, xiData, out skillName) && !TryGetDeathExplosionDotChild(dotSkill.SonB, xiData, out skillName) && !TryGetDeathExplosionDotChild(dotSkill.SonC, xiData, out skillName))
		{
			return TryGetDeathExplosionDotChild(dotSkill.SonD, xiData, out skillName);
		}
		return true;
	}

	private static bool TryGetDeathExplosionDotChild(string childName, SkillXiData xiData, out string skillName)
	{
		skillName = string.Empty;
		if (string.IsNullOrEmpty(childName) || xiData == null || xiData.Dot_S == null)
		{
			return false;
		}
		if (!xiData.Dot_S.TryGetValue(childName, out var value) || value == null || value.SonType != 7)
		{
			return false;
		}
		skillName = value.IndexName;
		return true;
	}

	private static string GetColorSkillName(string skillName, string color)
	{
		if (string.IsNullOrEmpty(skillName))
		{
			return string.Empty;
		}
		return "<color=" + color + ">" + LOC.MM.GetSkill(skillName) + "</color>";
	}

	private static void AppendTextBlock(ref string stats, string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (string.IsNullOrEmpty(stats))
			{
				stats = text;
			}
			else
			{
				stats = stats + "\n" + text;
			}
		}
	}

	public string GetSK()
	{
		string stats = string.Empty;
		if (SK == null)
		{
			return stats;
		}
		for (int i = 0; i < SK.Length; i++)
		{
			WPDT_B wPDT_B = SK[i];
			if (wPDT_B != null && wPDT_B.Index != 0)
			{
				string text = GetSKArrayLine(wPDT_B);
				if (!string.IsNullOrEmpty(text))
				{
					text += AffixTierDisplay.SuffixB(this, wPDT_B, isCompanion: false);
				}
				AppendTextBlock(ref stats, text);
			}
		}
		return stats;
	}

	private string GetSKArrayLine(WPDT_B skStat)
	{
		if (skStat == null || string.IsNullOrEmpty(skStat.SkillName))
		{
			return string.Empty;
		}
		TryGetSampleSkill(skStat.SkillName, out var skill);
		DamageType key2 = skill?.damageType ?? SWS.DMtype(skStat.EL);
		string text = DamageColor.Colors[key2];
		string skillName = GetColorSkillName(skStat.SkillName, text);
		if (string.IsNullOrEmpty(skillName))
		{
			skillName = skStat.SkillName;
		}
		string valueText = ItemManager.FormatWeaponStatValue(skStat.Index, skStat.number);
		switch (skStat.Index)
		{
		case 3000:
		{
			string skillChangeName = GetSkillChangeName(skStat.GlobleID, text);
			if (!string.IsNullOrEmpty(skillChangeName))
			{
				return SKText("SKStat_Transform", skillName, skillChangeName);
			}
			return string.Empty;
		}
		case 3100:
			return CountAdd("SKStat_Label_FireCount");
		case 3101:
			return CountAdd("SKStat_Label_SplitCount");
		case 3102:
			return CountAdd("SKStat_Label_TargetCount");
		case 3103:
			return CountAdd("SKStat_Label_MultiShotCount");
		case 3200:
		{
			string linkedSampleSkillName2 = GetLinkedSampleSkillName(skStat.LinkSK, text);
			if (!string.IsNullOrEmpty(linkedSampleSkillName2))
			{
				return SKText("SKStat_LinkFire", skillName, linkedSampleSkillName2);
			}
			return string.Empty;
		}
		case 3201:
			return SKText("SKStat_LinkAllReady", skillName);
		case 3202:
			return SKText("SKStat_EverySkillLink", skillName);
		case 3203:
		{
			string linkedSampleSkillName = GetLinkedSampleSkillName(skStat.LinkSK, text);
			if (!string.IsNullOrEmpty(linkedSampleSkillName))
			{
				return SKText("SKStat_DamageInherit", skillName, linkedSampleSkillName);
			}
			return string.Empty;
		}
		case 3300:
			return SKText("SKStat_AutoUse", skillName);
		case 3301:
			return SKText("SKStat_RefreshChance", skillName, valueText);
		case 3302:
			return SKText("SKStat_DamagePerHitTarget", skillName, valueText);
		case 3303:
			return SKText("SKStat_DamagePerCompanion", skillName, valueText);
		case 3304:
			return SKText("SKStat_RicochetChance", skillName, valueText);
		case 3305:
			return SKText("SKStat_ExecuteNonElite", skillName, valueText);
		case 3306:
			return SKText("SKStat_BlockFireChance", skillName, valueText);
		case 3307:
			return SKText("SKStat_TransformAttackSkillDouble", skillName, valueText);
		case 3308:
			return SKText("SKStat_DamageDouble", skillName);
		case 3400:
			return SKText("SKStat_InvincibleAfterUse", skillName, valueText);
		case 3401:
			return SKText("SKStat_CritExtendDuration", skillName, valueText);
		case 3402:
			return SKText("SKStat_CritReduceCooldown", skillName, valueText);
		case 3403:
			return SKText("SKStat_HealOnEnd", skillName, valueText);
		case 3404:
			return SKText("SKStat_WindErosionZone", skillName, valueText);
		case 3500:
			return UseBuff(LOC.MM.GetMain("damage"), "4", "5");
		case 3501:
			return UseBuff(LOC.MM.GetMain("AttackSpeed"), "3", "4");
		case 3502:
			return UseBuff(LOC.MM.GetMain("MoveSpeed"), "2", "3");
		case 3503:
			return UseBuff(LOC.MM.GetMain("Comp damage"), "3", "4");
		case 3504:
			return UseBuff(LOC.MM.GetMain("Comp AttackSpeed"), "2", "4");
		case 3530:
			return UseBuff(GetColoredElementLabel(skStat.EL, isDamage: true), "3", "4");
		case 3535:
			return UseBuff(GetColoredElementLabel(skStat.EL, isDamage: false), "2", "3");
		case 3550:
			return WhileExists(LOC.MM.GetMain("damage"));
		case 3551:
			return WhileExists(LOC.MM.GetMain("AttackSpeed"));
		case 3552:
			return WhileExists(LOC.MM.GetMain("MoveSpeed"));
		case 3553:
			return WhileExists(LOC.MM.GetMain("BJrate"));
		case 3554:
			return WhileExists(LOC.MM.GetMain("BJDamage"));
		case 3555:
			return WhileExists(LOC.MM.GetMain("Character_DebuffDurationReduction"));
		case 3556:
			return WhileExists(LOC.MM.GetMain("DamageAnti"));
		case 3557:
			return WhileExists(LOC.MM.GetMain("GeDang"));
		case 3558:
			return WhileExists(LOC.MM.GetMain("Character_AccessoryDamage"));
		case 3559:
			return WhileExists(LOC.MM.GetMain("Character_TrapDamage"));
		case 3560:
			return WhileExists(LOC.MM.GetMain("Character_DotDamage"));
		case 3561:
			return WhileExists(LOC.MM.GetMain("Character_CompDamage"));
		default:
			return string.Empty;
		}
		string CountAdd(string labelKey)
		{
			return skillName + ": " + SKText("MainStat_PlainNumber", valueText, Label(labelKey));
		}
		static string Label(string key)
		{
			return LOC.MM.Get("MainDisplay_FY." + key);
		}
		string UseBuff(string label, string duration, string maxStacks)
		{
			return SKText("SKStat_UseBuffStack", skillName, valueText, label, duration, maxStacks);
		}
		string WhileExists(string label)
		{
			return SKText("SKStat_WhileExists", skillName, valueText, label);
		}
	}

	private static bool TryGetSampleSkill(string skillName, out SkillData_Sample_Father skill, bool usedOnly = false)
	{
		skill = null;
		if (string.IsNullOrEmpty(skillName) || !SingletonMonoScope<TalentManager>.HasInstance || SingletonMonoScope<TalentManager>.Instance.XiData == null)
		{
			return false;
		}
		SkillXiData[] xiData = SingletonMonoScope<TalentManager>.Instance.XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData != null && (!usedOnly || skillXiData.Used) && skillXiData.Sample_F != null && skillXiData.Sample_F.TryGetValue(skillName, out skill) && skill != null)
			{
				return true;
			}
		}
		return false;
	}

	private static string GetLinkedSampleSkillName(string skillName, string fallbackColor)
	{
		if (string.IsNullOrEmpty(skillName))
		{
			return string.Empty;
		}
		if (TryGetSampleSkill(skillName, out var skill))
		{
			return GetColorSkillName(skill.IndexName, DamageColor.Colors[skill.damageType]);
		}
		return GetColorSkillName(skillName, fallbackColor);
	}

	private static string GetSkillChangeName(int globalId, string fallbackColor)
	{
		if (!SingletonMonoScope<TalentManager>.HasInstance || SingletonMonoScope<TalentManager>.Instance.SKC_Data == null)
		{
			return string.Empty;
		}
		SkilChangeData skilChangeData = SingletonMonoScope<TalentManager>.Instance.SKC_Data.FirstOrDefault((SkilChangeData x) => x != null && x.GlobleID == globalId);
		if (skilChangeData == null || string.IsNullOrEmpty(skilChangeData.IndexName))
		{
			return string.Empty;
		}
		return GetColorSkillName(skilChangeData.IndexName, fallbackColor);
	}

	private static string GetColoredElementLabel(int element, bool isDamage)
	{
		return GetColoredElementLabel(SWS.DMtype(element), isDamage);
	}

	private static string GetColoredElementLabel(DamageType damageType, bool isDamage)
	{
		string key = (isDamage ? SWS.El_DMG(damageType) : SWS.El_Chuan(damageType));
		return "<color=" + DamageColor.Colors[damageType] + ">" + LOC.MM.GetMain(key) + "</color>";
	}

	public string GetCP()
	{
		string stats = string.Empty;
		if (CP == null)
		{
			return stats;
		}
		for (int i = 0; i < CP.Length; i++)
		{
			WPDT_B wPDT_B = CP[i];
			if (wPDT_B != null && wPDT_B.Index != 0)
			{
				string text = GetCPArrayLine(wPDT_B);
				if (!string.IsNullOrEmpty(text))
				{
					text += AffixTierDisplay.SuffixB(this, wPDT_B, isCompanion: true);
				}
				AppendTextBlock(ref stats, text);
			}
		}
		return stats;
	}

	private string GetCPArrayLine(WPDT_B cpStat)
	{
		if (cpStat == null || string.IsNullOrEmpty(cpStat.SkillName))
		{
			return string.Empty;
		}
		TryGetCompSkill(cpStat.SkillName, out var skill);
		DamageType damageType = skill?.damageType ?? SWS.DMtype(cpStat.EL);
		string text = DamageColor.Colors[damageType];
		string skillName = GetColorSkillName(cpStat.SkillName, text);
		if (string.IsNullOrEmpty(skillName))
		{
			skillName = cpStat.SkillName;
		}
		string valueText = ItemManager.FormatWeaponStatValue(cpStat.Index, cpStat.number);
		switch (cpStat.Index)
		{
		case 4000:
		{
			string compSkillChangeName = GetCompSkillChangeName(cpStat.GlobleID, text);
			if (!string.IsNullOrEmpty(compSkillChangeName))
			{
				return SKText("CPStat_Transform", skillName, compSkillChangeName);
			}
			return string.Empty;
		}
		case 4050:
			return SKText("CPStat_AutoUse", skillName);
		case 4100:
			return SKText("CPStat_SummonCountAdd", skillName, valueText);
		case 4101:
		{
			string cPCountModeText2 = GetCPCountModeText(Mathf.FloorToInt(cpStat.number));
			if (!string.IsNullOrEmpty(cPCountModeText2))
			{
				return SKText("CPStat_SummonCountMode", skillName, cPCountModeText2);
			}
			return string.Empty;
		}
		case 4200:
			return SKText("CPStat_FireCountAdd", skillName, valueText);
		case 4201:
		{
			string cPCountModeText = GetCPCountModeText(Mathf.FloorToInt(cpStat.number));
			if (!string.IsNullOrEmpty(cPCountModeText))
			{
				return SKText("CPStat_FireCountMode", skillName, cPCountModeText);
			}
			return string.Empty;
		}
		case 4202:
			return SKText("CPStat_DoubleAttack", skillName);
		case 4300:
			return SKText("CPStat_BlockHeal", skillName, valueText);
		case 4301:
			return SKText("CPStat_BloodDie", skillName, valueText);
		case 4302:
			return SKText("CPStat_SoulExplosion", skillName, valueText, GetColorSkillName("Soul Explosion", text));
		case 4303:
			return SKText("CPStat_DotLayer", skillName, valueText);
		case 4304:
			return SKText("CPStat_CritNoDebuff", skillName);
		case 4305:
			return SKText("CPStat_IgnoreDamageReduction", skillName);
		case 4306:
			return SKText("CPStat_FieldRange", skillName, valueText);
		case 4307:
			return SKText("CPStat_KillHeal", skillName, valueText);
		case 4308:
			return SKText("CPStat_HurtReflect", skillName, valueText);
		case 4400:
			return SKText("CPStat_EveryDamage", skillName, valueText);
		case 4401:
			return SKText("CPStat_EveryElementPenetration", skillName, valueText, GetColoredElementLabel(damageType, isDamage: false));
		case 4402:
			return EveryPlayerStat(LOC.MM.GetMain("AttackSpeed"));
		case 4403:
			return EveryPlayerStat(LOC.MM.GetMain("MoveSpeed"));
		case 4404:
			return EveryPlayerStat(LOC.MM.GetMain("HealthMax"));
		case 4405:
			return EveryPlayerStat(LOC.MM.GetMain("ManaMax"));
		case 4406:
			return EveryPlayerStat(LOC.MM.GetMain("CD"));
		case 4407:
			return EveryPlayerStat(LOC.MM.GetMain("BJrate"));
		case 4408:
			return EveryPlayerStat(LOC.MM.GetMain("BJDamage"));
		case 4409:
			return EveryPlayerStat(LOC.MM.GetMain("GeDang"));
		case 4410:
			return EveryPlayerStat(LOC.MM.GetMain("DamageAnti"));
		case 4411:
			return EveryPlayerStat(LOC.MM.GetMain("Character_DebuffDurationReduction"));
		case 4412:
			return EveryPlayerStat(LOC.MM.GetMain("AllChuan"));
		case 4413:
			return EveryPlayerStat(LOC.MM.GetMain("AllAnti"));
		case 4414:
			return EveryPlayerStat(LOC.MM.GetMain("DropRate"));
		case 4415:
			return EveryPlayerStat(LOC.MM.GetMain("Character_TrapDamage"));
		case 4416:
			return EveryPlayerStat(LOC.MM.GetMain("Character_AccessoryDamage"));
		case 4417:
			return EveryPlayerStat(LOC.MM.GetMain("Character_DotDamage"));
		default:
			return string.Empty;
		}
		string EveryPlayerStat(string label)
		{
			return SKText("CPStat_EveryElementPenetration", skillName, valueText, label);
		}
	}

	private static bool TryGetCompSkill(string skillName, out SkillData_Comp_Father skill, bool usedOnly = false)
	{
		skill = null;
		if (string.IsNullOrEmpty(skillName) || !SingletonMonoScope<TalentManager>.HasInstance || SingletonMonoScope<TalentManager>.Instance.XiData == null)
		{
			return false;
		}
		SkillXiData[] xiData = SingletonMonoScope<TalentManager>.Instance.XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData != null && (!usedOnly || skillXiData.Used) && skillXiData.Comp_F != null && skillXiData.Comp_F.TryGetValue(skillName, out skill) && skill != null)
			{
				return true;
			}
		}
		return false;
	}

	private static string GetCompSkillChangeName(int globalId, string fallbackColor)
	{
		if (!SingletonMonoScope<TalentManager>.HasInstance || SingletonMonoScope<TalentManager>.Instance.CPC_Data == null)
		{
			return string.Empty;
		}
		if (!SingletonMonoScope<TalentManager>.Instance.CPC_Data.TryGetValue(globalId, out var value) || value == null || string.IsNullOrEmpty(value.IndexName))
		{
			return string.Empty;
		}
		return GetColorSkillName(value.IndexName, fallbackColor);
	}

	private static string GetCPCountModeText(int mode)
	{
		return mode switch
		{
			1 => "x2", 
			2 => "x3", 
			3 => "x4", 
			4 => "x5", 
			5 => SKText("CPStat_SetToOne"), 
			_ => string.Empty, 
		};
	}

	private bool TryGetSetData(out Set_DT setData)
	{
		setData = null;
		if (Set_Index <= 0)
		{
			return false;
		}
		if (SetRuntimeData != null && SetRuntimeData.SetID == Set_Index)
		{
			setData = SetRuntimeData;
			return true;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance || SingletonMonoScope<ItemManager>.Instance.SET == null)
		{
			return false;
		}
		if (SingletonMonoScope<ItemManager>.Instance.SET.TryGetValue(Set_Index, out setData))
		{
			return setData != null;
		}
		return false;
	}

	public string GetSetName()
	{
		if (!TryGetSetData(out var setData) || string.IsNullOrEmpty(setData.SetName))
		{
			return string.Empty;
		}
		return ItemDisplayText("SetStat_Name", LOC.MM.GetItem(setData.SetName));
	}

	public string GetSet()
	{
		string stats = string.Empty;
		if (!TryGetSetData(out var setData) || setData.Lit == null)
		{
			return stats;
		}
		int num = (SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.GetEquippedSetCount(Set_Index) : 0);
		int num2 = 2;
		int num3 = Mathf.Min(3, setData.Lit.Length);
		int num4 = 0;
		while (num4 < num3)
		{
			string text = GetSetArrayLine(setData, setData.Lit[num4], num2);
			if (!string.IsNullOrEmpty(text) && num2 > num)
			{
				text = GetInactiveSetLine(text);
			}
			AppendTextBlock(ref stats, text);
			num4++;
			num2++;
		}
		return stats;
	}

	private static string GetInactiveSetLine(string line)
	{
		if (string.IsNullOrEmpty(line))
		{
			return string.Empty;
		}
		string text = Regex.Replace(line, "</?color[^>]*>", string.Empty);
		return "<color=#808080>" + text + "</color>";
	}

	private string GetSetArrayLine(Set_DT setData, Set_DT_Lit lit, int pieceCount)
	{
		if (setData == null || lit == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		switch (lit.MainTP)
		{
		case 0:
			text = GetMainArrayLine(new WPDT_A
			{
				Index = lit.Index,
				EL = lit.EL,
				number = lit.Number
			}, null);
			break;
		case 1:
			text = GetDotArrayLine(new WPDT_A
			{
				Index = lit.Index,
				EL = lit.EL,
				number = lit.Number
			});
			break;
		case 2:
			text = GetSKArrayLine(new WPDT_B
			{
				SkillName = lit.SkillName,
				Index = lit.Index,
				GlobleID = lit.GlobleID,
				EL = lit.EL,
				number = lit.Number,
				LinkSK = lit.LinkSK
			});
			break;
		case 3:
			text = GetCPArrayLine(new WPDT_B
			{
				SkillName = lit.SkillName,
				Index = lit.Index,
				GlobleID = lit.GlobleID,
				EL = lit.EL,
				number = lit.Number,
				LinkSK = lit.LinkSK
			});
			break;
		case 10:
			text = GetSetLayerBuffLine(setData, lit);
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		return FormatSetPieceLine(pieceCount, text);
	}

	private static string FormatSetPieceLine(int pieceCount, string line)
	{
		string text = pieceCount.ToString(CultureInfo.InvariantCulture);
		string text2 = ItemDisplayText("SetStat_Piece", text, line);
		string text3 = text + ": ";
		if (text2.StartsWith(text3))
		{
			text2 = text + ":\u00a0" + text2.Substring(text3.Length);
		}
		return PrepareSetLineWrapping(text2);
	}

	private static string PrepareSetLineWrapping(string text)
	{
		if (string.IsNullOrEmpty(text) || !ContainsCjkText(text))
		{
			return text;
		}
		text = text.Replace(" + ", "\u00a0+\u00a0");
		StringBuilder stringBuilder = new StringBuilder(text.Length + 8);
		bool flag = false;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '<')
			{
				flag = true;
			}
			stringBuilder.Append(c);
			if (c == '>')
			{
				flag = false;
			}
			else if (!flag && IsCjkWrapPunctuation(c) && (i + 1 >= text.Length || text[i + 1] != '\u200b'))
			{
				stringBuilder.Append('\u200b');
			}
		}
		return stringBuilder.ToString();
	}

	private static bool ContainsCjkText(string text)
	{
		foreach (char c in text)
		{
			if (c >= '一' && c <= '\u9fff')
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsCjkWrapPunctuation(char value)
	{
		if (value != '，' && value != '；' && value != '、')
		{
			return value == '。';
		}
		return true;
	}

	private string GetSetLayerBuffLine(Set_DT setData, Set_DT_Lit lit)
	{
		if (setData == null || string.IsNullOrEmpty(setData.BuffName) || setData.LayerMax <= 0)
		{
			return string.Empty;
		}
		string buff = LOC.MM.GetBuff(setData.BuffName);
		string setLayerStatLabel = GetSetLayerStatLabel(setData.TP_Layer, lit.EL, isMax: false);
		string setLayerStatLabel2 = GetSetLayerStatLabel(setData.TP_Max, lit.EL, isMax: true);
		if (string.IsNullOrEmpty(setLayerStatLabel) || string.IsNullOrEmpty(setLayerStatLabel2))
		{
			return string.Empty;
		}
		return ItemDisplayText((setData.BuffType == 1) ? "SetStat_StackKill" : "SetStat_StackAttack", buff, setData.NumberL.ToString("0.##", CultureInfo.InvariantCulture), setLayerStatLabel, setData.NumberM.ToString("0.##", CultureInfo.InvariantCulture), setLayerStatLabel2, setData.LayerMax);
	}

	private string GetSetLayerStatLabel(int type, int element, bool isMax)
	{
		if (!isMax)
		{
			switch (type)
			{
			case 1:
				return LOC.MM.GetMain("damage");
			case 2:
				return GetColoredElementLabel(element, isDamage: true);
			case 3:
				return GetColoredElementLabel(element, isDamage: false);
			case 4:
				return LOC.MM.GetMain("BJrate");
			case 5:
				return LOC.MM.GetMain("AttackSpeed");
			case 6:
				return LOC.MM.GetMain("MoveSpeed");
			}
		}
		else
		{
			switch (type)
			{
			case 1:
				return LOC.MM.GetMain("DamageAnti");
			case 2:
				return LOC.MM.GetMain("HealthPrc");
			case 3:
				return LOC.MM.GetMain("Character_DotDamage");
			case 4:
				return LOC.MM.GetMain("Character_CompDamage");
			case 5:
				return LOC.MM.GetMain("BJDamage");
			case 6:
				return LOC.MM.GetMain("MoveSpeed");
			case 7:
				return GetColoredElementLabel(element, isDamage: false);
			}
		}
		return string.Empty;
	}

	public void RestoreEquip()
	{
		Equip(0);
	}

	private void AddEquippedSPC()
	{
		if (SPC == null)
		{
			return;
		}
		for (int i = 0; i < SPC.Count; i++)
		{
			if (TryGetSPCTemplate(i, out var spc, out var mb))
			{
				SingletonMonoScope<ACTbar>.Instance.AddWP_SPC(mb, spc.EL, GlobalID, i, CharType, GetSPCPRC(spc));
			}
		}
	}

	private void DelEquippedSPC()
	{
		if (SPC == null)
		{
			return;
		}
		for (int i = 0; i < SPC.Count; i++)
		{
			if (TryGetSPCTemplate(i, out var _, out var mb))
			{
				SingletonMonoScope<ACTbar>.Instance.DelWP_SPC(mb, GlobalID, i, CharType);
			}
		}
	}

	private bool HasValidEquippedSPC()
	{
		if (SPC == null)
		{
			return false;
		}
		for (int i = 0; i < SPC.Count; i++)
		{
			if (TryGetSPCTemplate(i, out var _, out var _))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsValidWeaponSkillPoint(WPSkill skill)
	{
		if (skill != null && GetWeaponSkillPointValue(skill) > 0 && !string.IsNullOrEmpty(skill.IndexName))
		{
			return skill.IndexName != "0";
		}
		return false;
	}

	private static int GetWeaponSkillPointValue(WPSkill skill)
	{
		if (skill != null)
		{
			return skill.Number + skill.Number2;
		}
		return 0;
	}

	private int GetEquippedWeaponSkillPointCount()
	{
		if (WPSK == null || WPSK.Count <= 0 || WP_SkillCount <= 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = Mathf.Min(WP_SkillCount, WPSK.Count);
		for (int i = 0; i < num2; i++)
		{
			if (IsValidWeaponSkillPoint(WPSK[i]))
			{
				num += GetWeaponSkillPointValue(WPSK[i]);
			}
		}
		return num;
	}

	private static void RefreshSkillButton(SkillData data)
	{
		if (data != null)
		{
			if (!data.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
			{
				SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
			}
			if ((bool)data.skillbt)
			{
				data.skillbt.Refresh(data.Level_Base, data.Level_Max, data.Level_WeaponOn);
			}
		}
	}

	private bool IsValidSocketedGem(WPAocao socket)
	{
		if (socket != null && socket.HasAocao && socket.HasBaoshi)
		{
			return !string.IsNullOrEmpty(socket.Name);
		}
		return false;
	}

	private int GetActiveSocketCount()
	{
		if (Aocao == null || AocaoCount <= 0)
		{
			return 0;
		}
		return Mathf.Min(AocaoCount, Aocao.Count);
	}

	private int GetEquippedSocketedGemCount()
	{
		int num = 0;
		for (int i = 0; i < GetActiveSocketCount(); i++)
		{
			if (IsValidSocketedGem(Aocao[i]))
			{
				num++;
			}
		}
		return num;
	}

	private void ApplyEquippedConditionalCounts(PlayerManager player, int direction)
	{
		if (!(player == null) && direction != 0)
		{
			if (ZQ_CountMax > 0)
			{
				player.BE_ZQ_Count += direction;
			}
			if (HasValidEquippedSPC())
			{
				player.BE_SPC_Count += direction;
			}
			if (SKCount > 0)
			{
				player.BE_HH_Count += direction;
			}
			player.BE_SK_Count += GetEquippedWeaponSkillPointCount() * direction;
			player.BE_BS_Count += GetEquippedSocketedGemCount() * direction;
		}
	}

	private void ApplyFWBase(PlayerManager player, bool isEquip)
	{
		if (player == null || FW_Base == null || string.IsNullOrEmpty(FW_Base.type))
		{
			return;
		}
		float num = (isEquip ? FW_Base.number : (0f - FW_Base.number));
		string type = FW_Base.type;
		if (type == null)
		{
			return;
		}
		switch (type.Length)
		{
		case 3:
			switch (type[1])
			{
			case 'M':
				if (type == "DMG")
				{
					player.Damage_Bei += num;
				}
				break;
			case 'T':
				if (type == "ATS")
				{
					player.ATSpeed_Bei += num;
				}
				break;
			case 'J':
				if (type == "BJD")
				{
					player.BJDamage += num;
				}
				break;
			case 'O':
				if (type == "DOT")
				{
					player.AllDot_DMG += num;
				}
				break;
			case 'V':
				if (type == "MVS")
				{
					player.MVSpeed_Bei += num;
				}
				break;
			}
			break;
		case 4:
			switch (type[1])
			{
			case 'L':
				if (type == "ALLC")
				{
					player.AllChuan += num;
				}
				break;
			case 'e':
				if (type == "Heal")
				{
					player.Health_Bei += num;
				}
				break;
			case 'a':
				if (type == "Mana")
				{
					player.Mana_Bei += num;
				}
				break;
			case 'n':
				if (type == "Anti")
				{
					player.AllAnti += num;
				}
				break;
			case 'r':
				if (type == "Drop")
				{
					player.ItemDrop_Rate += num;
				}
				break;
			}
			break;
		case 5:
			switch (type[2])
			{
			case 'D':
				if (type == "C_DMG")
				{
					player.C_Damage += num;
				}
				break;
			case 'A':
				if (type == "C_ATS")
				{
					player.C_ATSpeed += num;
				}
				break;
			}
			break;
		case 6:
			switch (type[2])
			{
			case 'H':
				if (type == "C_Heal")
				{
					player.C_Health += num;
				}
				break;
			case 'A':
				if (type == "C_Anti")
				{
					player.C_AllAnti += num;
				}
				break;
			case '_':
				if (type == "XJ_DMG")
				{
					player.XJ_DMG += Mathf.RoundToInt(num);
				}
				break;
			}
			break;
		case 7:
			if (type == "ORB_DMG")
			{
				player.WPSPC_DMG += Mathf.RoundToInt(num);
			}
			break;
		}
	}

	private static int GetSocketedGemNumber(WPAocao socket, int bsAdd, float bsMulti)
	{
		if (socket == null)
		{
			return 0;
		}
		float num = socket.Number + (float)bsAdd;
		return Mathf.FloorToInt(num + num * bsMulti / 100f);
	}

	private void ApplySocketedGemStats(PlayerManager player, bool isEquip, int bsAdd, float bsMulti)
	{
		if (player == null || Aocao == null || AocaoCount <= 0)
		{
			return;
		}
		int num = (isEquip ? 1 : (-1));
		for (int i = 0; i < GetActiveSocketCount(); i++)
		{
			WPAocao wPAocao = Aocao[i];
			if (IsValidSocketedGem(wPAocao))
			{
				int num2 = GetSocketedGemNumber(wPAocao, bsAdd, bsMulti) * num;
				switch (wPAocao.Type)
				{
				case 0:
					player.Health_Bei += num2;
					break;
				case 1:
					player.FireAnti += num2;
					break;
				case 2:
					player.FireChuan += num2;
					break;
				case 3:
					player.FireDamage_Bei += num2;
					break;
				case 4:
					player.ItemDrop_Rate += num2;
					break;
				case 5:
					player.ThunderAnti += num2;
					break;
				case 6:
					player.ThunderChuan += num2;
					break;
				case 7:
					player.ThunderDamage_Bei += num2;
					break;
				case 8:
					player.C_Health += num2;
					break;
				case 9:
					player.PoisonAnti += num2;
					break;
				case 10:
					player.PoisonChuan += num2;
					break;
				case 11:
					player.C_ATSpeed += num2;
					break;
				case 12:
					player.PoisonDamage_Bei += num2;
					break;
				case 13:
					player.Mana_Bei += num2;
					break;
				case 14:
					player.FrozenAnti += num2;
					break;
				case 15:
					player.FrozenChuan += num2;
					break;
				case 16:
					player.FrozenDamage_Bei += num2;
					break;
				case 17:
					player.C_Damage += num2;
					break;
				case 18:
					player.ShadowAnti += num2;
					break;
				case 19:
					player.ShadowChuan += num2;
					break;
				case 20:
					player.MVSpeed_Bei += num2;
					break;
				case 21:
					player.ShadowDamage_Bei += num2;
					break;
				case 22:
					player.ATSpeed_Bei += num2;
					break;
				case 23:
					player.PhysicsAnti += num2;
					break;
				case 24:
					player.PhysicsChuan += num2;
					break;
				case 25:
					player.PhysicsDamage_Bei += num2;
					break;
				case 26:
					player.BS_ExtraProjectiles += num2;
					break;
				}
			}
		}
	}

	private static void RefreshEquippedSocketedGemStats(PlayerManager player, WeaponClass changedWeapon, int oldBsAdd, float oldBsMulti, int newBsAdd, float newBsMulti)
	{
		if (player == null || !SingletonMonoScope<InventoryManager>.HasInstance || SingletonMonoScope<InventoryManager>.Instance.CharBT == null || (oldBsAdd == newBsAdd && Mathf.Approximately(oldBsMulti, newBsMulti)))
		{
			return;
		}
		CharButton[] charBT = SingletonMonoScope<InventoryManager>.Instance.CharBT;
		foreach (CharButton charButton in charBT)
		{
			if ((bool)charButton && charButton.hasWeapon && charButton.weapon != null && charButton.weapon != changedWeapon)
			{
				charButton.weapon.ApplySocketedGemStats(player, isEquip: false, oldBsAdd, oldBsMulti);
				charButton.weapon.ApplySocketedGemStats(player, isEquip: true, newBsAdd, newBsMulti);
			}
		}
	}

	private static void RefreshPlayerEquipmentValues(PlayerManager player)
	{
		if (!(player == null))
		{
			player.RefreshRuntimeDerivedStats();
			if (Character.Instance != null)
			{
				Character.Instance.RefreshUI();
			}
		}
	}

	public void Equip(int EQtype)
	{
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		TalentManager instance2 = SingletonMonoScope<TalentManager>.Instance;
		int bS_Add = instance.BS_Add;
		float bS_Multi = instance.BS_Multi;
		switch (EQtype)
		{
		case 0:
		{
			instance.Damage_Base += DamageFinal;
			instance.Health += HealthFinal;
			instance.Mana += ManaFinal;
			switch (WeaponType)
			{
			case "hand":
			case "head":
			case "body":
			case "leg":
				instance.FireAnti += Fire;
				instance.FrozenAnti += Frozen;
				instance.ThunderAnti += Thunder;
				instance.PoisonAnti += Poison;
				instance.PhysicsAnti += Physics;
				instance.ShadowAnti += Shadow;
				break;
			case "bone":
			case "bow":
			case "staff":
			case "sword":
				instance.FireDamage_Bei += Fire;
				instance.FrozenDamage_Bei += Frozen;
				instance.ThunderDamage_Bei += Thunder;
				instance.PoisonDamage_Bei += Poison;
				instance.PhysicsDamage_Bei += Physics;
				instance.ShadowDamage_Bei += Shadow;
				break;
			case "spell":
			case "arrow":
			case "corpse":
			case "shield":
				instance.FireChuan += Fire;
				instance.FrozenChuan += Frozen;
				instance.ThunderChuan += Thunder;
				instance.PoisonChuan += Poison;
				instance.PhysicsChuan += Physics;
				instance.ShadowChuan += Shadow;
				break;
			case "little":
				switch (CharType)
				{
				case 6:
					instance.FireAnti += Fire;
					instance.FrozenAnti += Frozen;
					instance.ThunderAnti += Thunder;
					instance.PoisonAnti += Poison;
					instance.PhysicsAnti += Physics;
					instance.ShadowAnti += Shadow;
					break;
				case 7:
					instance.FireDamage_Bei += Fire;
					instance.FrozenDamage_Bei += Frozen;
					instance.ThunderDamage_Bei += Thunder;
					instance.PoisonDamage_Bei += Poison;
					instance.PhysicsDamage_Bei += Physics;
					instance.ShadowDamage_Bei += Shadow;
					break;
				case 8:
					instance.FireChuan += Fire;
					instance.FrozenChuan += Frozen;
					instance.ThunderChuan += Thunder;
					instance.PoisonChuan += Poison;
					instance.PhysicsChuan += Physics;
					instance.ShadowChuan += Shadow;
					break;
				case 9:
					instance.FireDamage_Bei += Fire;
					instance.FrozenDamage_Bei += Frozen;
					instance.ThunderDamage_Bei += Thunder;
					instance.PoisonDamage_Bei += Poison;
					instance.PhysicsDamage_Bei += Physics;
					instance.ShadowDamage_Bei += Shadow;
					break;
				}
				break;
			}
			if (Main != null)
			{
				for (int n = 0; n < Main.Length; n++)
				{
					ApplyMain(instance, Main[n], isEquip: true);
				}
			}
			if (DOT != null)
			{
				for (int num = 0; num < DOT.Length; num++)
				{
					ApplyDot(instance, DOT[num], isEquip: true);
				}
			}
			if (SK != null)
			{
				for (int num2 = 0; num2 < SK.Length; num2++)
				{
					ApplySK(instance, SK[num2], isEquip: true);
				}
			}
			if (CP != null)
			{
				for (int num3 = 0; num3 < CP.Length; num3++)
				{
					ApplyCP(instance, CP[num3], isEquip: true);
				}
			}
			ApplySet(instance, isEquip: true);
			RefreshSkillRuntimeData();
			for (int num4 = 0; num4 < WP_SkillCount && num4 < WPSK.Count; num4++)
			{
				string indexName2 = WPSK[num4].IndexName;
				if (string.IsNullOrEmpty(indexName2) || !instance2.SKI.TryGetValue(indexName2, out var value12))
				{
					continue;
				}
				if (SingletonMonoScope<TalentManager>.HasInstance)
				{
					SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
				}
				switch (value12.type)
				{
				case 0:
				{
					instance2.XiData[value12.Xi].Sample_F.TryGetValue(WPSK[num4].IndexName, out var value16);
					value16.Level_WeaponOn += GetWeaponSkillPointValue(WPSK[num4]);
					if (!value16.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(value12.Xi, value12.type, value16);
					RefreshSkillButton(value16);
					break;
				}
				case 1:
				{
					instance2.XiData[value12.Xi].Sample_S.TryGetValue(WPSK[num4].IndexName, out var value14);
					instance2.XiData[value12.Xi].Sample_F.TryGetValue(value14.FatherSkill, out var value15);
					value14.Level_WeaponOn += GetWeaponSkillPointValue(WPSK[num4]);
					switch (value14.FrontSkillType)
					{
					case 0:
						if (!value14.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(value12.Xi, value12.type, value15);
						RefreshSkillButton(value14);
						break;
					case 1:
						if (!value14.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(value12.Xi, value12.type, value15);
						RefreshSkillButton(value14);
						break;
					}
					break;
				}
				case 2:
				{
					instance2.XiData[value12.Xi].Comp_F.TryGetValue(WPSK[num4].IndexName, out var value20);
					value20.Level_WeaponOn += GetWeaponSkillPointValue(WPSK[num4]);
					if (!value20.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(value12.Xi, value12.type, value20);
					RefreshSkillButton(value20);
					break;
				}
				case 3:
				{
					instance2.XiData[value12.Xi].Comp_S.TryGetValue(WPSK[num4].IndexName, out var value18);
					instance2.XiData[value12.Xi].Comp_F.TryGetValue(value18.FatherSkill, out var value19);
					value18.Level_WeaponOn += GetWeaponSkillPointValue(WPSK[num4]);
					switch (value18.FrontSkillType)
					{
					case 2:
						if (!value18.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(value12.Xi, value12.type, value19);
						RefreshSkillButton(value18);
						Debug.Log(1);
						break;
					case 3:
						if (!value18.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(value12.Xi, value12.type, value19);
						RefreshSkillButton(value18);
						Debug.Log(2);
						break;
					}
					break;
				}
				case 4:
				{
					instance2.XiData[value12.Xi].Dot_F.TryGetValue(WPSK[num4].IndexName, out var value17);
					value17.Level_WeaponOn += GetWeaponSkillPointValue(WPSK[num4]);
					if (!value17.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					SingletonMonoScope<ACTbar>.Instance.SetDot(value17);
					RefreshSkillButton(value17);
					break;
				}
				case 5:
				{
					instance2.XiData[value12.Xi].Dot_S.TryGetValue(WPSK[num4].IndexName, out var value21);
					instance2.XiData[value12.Xi].Dot_F.TryGetValue(value21.FatherSkill, out var value22);
					value21.Level_WeaponOn += GetWeaponSkillPointValue(WPSK[num4]);
					switch (value21.FrontSkillType)
					{
					case 4:
						if (!value21.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.SetDot(value22);
						RefreshSkillButton(value21);
						break;
					case 5:
						if (!value21.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.SetDot(value22);
						RefreshSkillButton(value21);
						break;
					}
					break;
				}
				case 6:
				{
					instance2.XiData[value12.Xi].Bei.TryGetValue(WPSK[num4].IndexName, out var value13);
					value13.Level_WeaponOn += GetWeaponSkillPointValue(WPSK[num4]);
					if (!value13.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					if (value13.Level_Base > 0)
					{
						instance.SetSkillBeiBuff(add: true, value13.B_Type, value13.B_Number, GetWeaponSkillPointValue(WPSK[num4]));
					}
					RefreshSkillButton(value13);
					break;
				}
				}
			}
			if (CP != null)
			{
				RefreshCPRuntime(CP);
			}
			RefreshEquippedSocketedGemStats(instance, this, bS_Add, bS_Multi, instance.BS_Add, instance.BS_Multi);
			ApplySocketedGemStats(instance, isEquip: true, instance.BS_Add, instance.BS_Multi);
			ApplyFWBase(instance, isEquip: true);
			AddEquippedSPC();
			ApplyEquippedConditionalCounts(instance, 1);
			SingletonMonoScope<ACTbar>.Instance.RefreshCD();
			SingletonMonoScope<ACTbar>.Instance.RebuildDotRuntimeDataFromTalent();
			instance.BuffRuntime?.ClearAllRuntimeBuffs();
			RefreshPlayerEquipmentValues(instance);
			break;
		}
		case 1:
		{
			instance.Damage_Base -= DamageFinal;
			instance.Health -= HealthFinal;
			instance.Mana -= ManaFinal;
			switch (WeaponType)
			{
			case "hand":
			case "head":
			case "body":
			case "leg":
				instance.FireAnti -= Fire;
				instance.FrozenAnti -= Frozen;
				instance.ThunderAnti -= Thunder;
				instance.PoisonAnti -= Poison;
				instance.PhysicsAnti -= Physics;
				instance.ShadowAnti -= Shadow;
				break;
			case "bone":
			case "bow":
			case "staff":
			case "sword":
				instance.FireDamage_Bei -= Fire;
				instance.FrozenDamage_Bei -= Frozen;
				instance.ThunderDamage_Bei -= Thunder;
				instance.PoisonDamage_Bei -= Poison;
				instance.PhysicsDamage_Bei -= Physics;
				instance.ShadowDamage_Bei -= Shadow;
				break;
			case "spell":
			case "arrow":
			case "corpse":
			case "shield":
				instance.FireChuan -= Fire;
				instance.FrozenChuan -= Frozen;
				instance.ThunderChuan -= Thunder;
				instance.PoisonChuan -= Poison;
				instance.PhysicsChuan -= Physics;
				instance.ShadowChuan -= Shadow;
				break;
			case "little":
				switch (CharType)
				{
				case 6:
					instance.FireAnti -= Fire;
					instance.FrozenAnti -= Frozen;
					instance.ThunderAnti -= Thunder;
					instance.PoisonAnti -= Poison;
					instance.PhysicsAnti -= Physics;
					instance.ShadowAnti -= Shadow;
					break;
				case 7:
					instance.FireDamage_Bei -= Fire;
					instance.FrozenDamage_Bei -= Frozen;
					instance.ThunderDamage_Bei -= Thunder;
					instance.PoisonDamage_Bei -= Poison;
					instance.PhysicsDamage_Bei -= Physics;
					instance.ShadowDamage_Bei -= Shadow;
					break;
				case 8:
					instance.FireChuan -= Fire;
					instance.FrozenChuan -= Frozen;
					instance.ThunderChuan -= Thunder;
					instance.PoisonChuan -= Poison;
					instance.PhysicsChuan -= Physics;
					instance.ShadowChuan -= Shadow;
					break;
				case 9:
					instance.FireDamage_Bei -= Fire;
					instance.FrozenDamage_Bei -= Frozen;
					instance.ThunderDamage_Bei -= Thunder;
					instance.PoisonDamage_Bei -= Poison;
					instance.PhysicsDamage_Bei -= Physics;
					instance.ShadowDamage_Bei -= Shadow;
					break;
				}
				break;
			}
			if (Main != null)
			{
				for (int i = 0; i < Main.Length; i++)
				{
					ApplyMain(instance, Main[i], isEquip: false);
				}
			}
			if (DOT != null)
			{
				for (int j = 0; j < DOT.Length; j++)
				{
					ApplyDot(instance, DOT[j], isEquip: false);
				}
			}
			if (SK != null)
			{
				for (int k = 0; k < SK.Length; k++)
				{
					ApplySK(instance, SK[k], isEquip: false);
				}
			}
			if (CP != null)
			{
				for (int l = 0; l < CP.Length; l++)
				{
					ApplyCP(instance, CP[l], isEquip: false);
				}
			}
			ApplySet(instance, isEquip: false);
			RefreshSkillRuntimeData();
			for (int m = 0; m < WP_SkillCount && m < WPSK.Count; m++)
			{
				string indexName = WPSK[m].IndexName;
				if (string.IsNullOrEmpty(indexName) || !instance2.SKI.TryGetValue(indexName, out var value))
				{
					continue;
				}
				if (SingletonMonoScope<TalentManager>.HasInstance)
				{
					SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
				}
				switch (value.type)
				{
				case 0:
				{
					instance2.XiData[value.Xi].Sample_F.TryGetValue(WPSK[m].IndexName, out var value5);
					value5.Level_WeaponOn -= GetWeaponSkillPointValue(WPSK[m]);
					if (!value5.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(value.Xi, value.type, value5);
					RefreshSkillButton(value5);
					break;
				}
				case 1:
				{
					instance2.XiData[value.Xi].Sample_S.TryGetValue(WPSK[m].IndexName, out var value3);
					instance2.XiData[value.Xi].Sample_F.TryGetValue(value3.FatherSkill, out var value4);
					value3.Level_WeaponOn -= GetWeaponSkillPointValue(WPSK[m]);
					switch (value3.FrontSkillType)
					{
					case 0:
						if (!value3.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(value.Xi, value.type, value4);
						RefreshSkillButton(value3);
						break;
					case 1:
						if (!value3.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(value.Xi, value.type, value4);
						RefreshSkillButton(value3);
						break;
					}
					break;
				}
				case 2:
				{
					instance2.XiData[value.Xi].Comp_F.TryGetValue(WPSK[m].IndexName, out var value9);
					value9.Level_WeaponOn -= GetWeaponSkillPointValue(WPSK[m]);
					if (!value9.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(value.Xi, value.type, value9);
					SingletonMonoScope<ACTbar>.Instance.ValidateCompCount(value9.IndexName);
					RefreshSkillButton(value9);
					break;
				}
				case 3:
				{
					instance2.XiData[value.Xi].Comp_S.TryGetValue(WPSK[m].IndexName, out var value7);
					instance2.XiData[value.Xi].Comp_F.TryGetValue(value7.FatherSkill, out var value8);
					value7.Level_WeaponOn -= GetWeaponSkillPointValue(WPSK[m]);
					switch (value7.FrontSkillType)
					{
					case 2:
						if (!value7.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(value.Xi, value.type, value8);
						SingletonMonoScope<ACTbar>.Instance.ValidateCompCount(value8.IndexName);
						RefreshSkillButton(value7);
						break;
					case 3:
						if (!value7.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(value.Xi, value.type, value8);
						SingletonMonoScope<ACTbar>.Instance.ValidateCompCount(value8.IndexName);
						RefreshSkillButton(value7);
						break;
					}
					break;
				}
				case 4:
				{
					instance2.XiData[value.Xi].Dot_F.TryGetValue(WPSK[m].IndexName, out var value6);
					value6.Level_WeaponOn -= GetWeaponSkillPointValue(WPSK[m]);
					if (!value6.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					SingletonMonoScope<ACTbar>.Instance.SetDot(value6);
					RefreshSkillButton(value6);
					break;
				}
				case 5:
				{
					instance2.XiData[value.Xi].Dot_S.TryGetValue(WPSK[m].IndexName, out var value10);
					instance2.XiData[value.Xi].Dot_F.TryGetValue(value10.FatherSkill, out var value11);
					value10.Level_WeaponOn -= GetWeaponSkillPointValue(WPSK[m]);
					switch (value10.FrontSkillType)
					{
					case 4:
						if (!value10.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.SetDot(value11);
						RefreshSkillButton(value10);
						break;
					case 5:
						if (!value10.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
						{
							SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
						}
						SingletonMonoScope<ACTbar>.Instance.SetDot(value11);
						RefreshSkillButton(value10);
						break;
					}
					break;
				}
				case 6:
				{
					instance2.XiData[value.Xi].Bei.TryGetValue(WPSK[m].IndexName, out var value2);
					value2.Level_WeaponOn -= GetWeaponSkillPointValue(WPSK[m]);
					if (!value2.skillbt && SingletonMonoScope<TalentManager>.HasInstance)
					{
						SingletonMonoScope<TalentManager>.Instance.RebindAllSkillBT();
					}
					if (value2.Level_Base > 0)
					{
						instance.SetSkillBeiBuff(add: true, value2.B_Type, value2.B_Number, -GetWeaponSkillPointValue(WPSK[m]));
					}
					RefreshSkillButton(value2);
					break;
				}
				}
			}
			if (CP != null)
			{
				RefreshCPRuntime(CP);
			}
			ApplySocketedGemStats(instance, isEquip: false, bS_Add, bS_Multi);
			RefreshEquippedSocketedGemStats(instance, this, bS_Add, bS_Multi, instance.BS_Add, instance.BS_Multi);
			ApplyFWBase(instance, isEquip: false);
			DelEquippedSPC();
			ApplyEquippedConditionalCounts(instance, -1);
			SingletonMonoScope<ACTbar>.Instance.RefreshCD();
			SingletonMonoScope<ACTbar>.Instance.RebuildDotRuntimeDataFromTalent();
			instance.BuffRuntime?.ClearAllRuntimeBuffs();
			RefreshEquippedBoolParameters(instance, this);
			RefreshPlayerEquipmentValues(instance);
			break;
		}
		}
	}

	private static void RefreshEquippedBoolParameters(PlayerManager player, WeaponClass removedWeapon)
	{
		if (player == null || !SingletonMonoScope<InventoryManager>.HasInstance || SingletonMonoScope<InventoryManager>.Instance.CharBT == null)
		{
			return;
		}
		Dictionary<int, WeaponClass> dictionary = new Dictionary<int, WeaponClass>();
		CharButton[] charBT = SingletonMonoScope<InventoryManager>.Instance.CharBT;
		foreach (CharButton charButton in charBT)
		{
			if ((bool)charButton && charButton.hasWeapon && charButton.weapon != null && charButton.weapon != removedWeapon)
			{
				WeaponClass weapon = charButton.weapon;
				ApplyEquippedWeaponBoolParameters(player, weapon);
				if (weapon.Set_Index > 0 && !dictionary.ContainsKey(weapon.Set_Index))
				{
					dictionary.Add(weapon.Set_Index, weapon);
				}
			}
		}
		foreach (KeyValuePair<int, WeaponClass> item in dictionary)
		{
			WeaponClass value = item.Value;
			if (value != null && value.TryGetSetData(out var setData) && setData.Lit != null)
			{
				int num = Mathf.Min(player.GetEquippedSetCount(item.Key) - 2, setData.Lit.Length - 1);
				for (int j = 0; j <= num; j++)
				{
					ApplySetBoolLit(player, setData, setData.Lit[j]);
				}
			}
		}
		RefreshSkillRuntimeData();
	}

	private static void ApplyEquippedWeaponBoolParameters(PlayerManager player, WeaponClass weapon)
	{
		if (player == null || weapon == null)
		{
			return;
		}
		if (weapon.Main != null)
		{
			for (int i = 0; i < weapon.Main.Length; i++)
			{
				ApplyMain(player, weapon.Main[i], isEquip: true, boolOnly: true);
			}
		}
		if (weapon.DOT != null)
		{
			for (int j = 0; j < weapon.DOT.Length; j++)
			{
				ApplyDot(player, weapon.DOT[j], isEquip: true, boolOnly: true);
			}
		}
		if (weapon.SK != null)
		{
			for (int k = 0; k < weapon.SK.Length; k++)
			{
				ApplySK(player, weapon.SK[k], isEquip: true, boolOnly: true);
			}
		}
		if (weapon.CP != null)
		{
			for (int l = 0; l < weapon.CP.Length; l++)
			{
				ApplyCP(player, weapon.CP[l], isEquip: true, boolOnly: true);
			}
		}
	}

	private static void ApplySetBoolLit(PlayerManager player, Set_DT setData, Set_DT_Lit lit)
	{
		if (!(player == null) && setData != null && lit != null)
		{
			switch (lit.MainTP)
			{
			case 0:
				ApplyMain(player, new WPDT_A
				{
					Index = lit.Index,
					EL = lit.EL,
					number = lit.Number
				}, isEquip: true, boolOnly: true);
				break;
			case 1:
				ApplyDot(player, new WPDT_A
				{
					Index = lit.Index,
					EL = lit.EL,
					number = lit.Number
				}, isEquip: true, boolOnly: true);
				break;
			case 2:
				ApplySK(player, new WPDT_B
				{
					SkillName = lit.SkillName,
					Index = lit.Index,
					GlobleID = lit.GlobleID,
					EL = lit.EL,
					number = lit.Number,
					LinkSK = lit.LinkSK
				}, isEquip: true, boolOnly: true);
				break;
			case 3:
				ApplyCP(player, new WPDT_B
				{
					SkillName = lit.SkillName,
					Index = lit.Index,
					GlobleID = lit.GlobleID,
					EL = lit.EL,
					number = lit.Number,
					LinkSK = lit.LinkSK
				}, isEquip: true, boolOnly: true);
				break;
			}
		}
	}

	internal static bool IsMainBoolIndex(int index)
	{
		switch (index)
		{
		case 307:
		case 508:
		case 654:
		case 750:
		case 751:
		case 752:
		case 753:
		case 862:
		case 863:
		case 864:
		case 1350:
		case 1360:
		case 1391:
		case 1501:
		case 1604:
		case 1801:
		case 1803:
		case 1804:
		case 1805:
		case 1807:
		case 1809:
		case 1810:
		case 1811:
		case 1812:
		case 1813:
		case 1814:
		case 1816:
		case 1820:
		case 1821:
		case 1822:
		case 1900:
		case 1901:
		case 1905:
			return true;
		default:
			return false;
		}
	}

	internal static bool IsDotBoolIndex(int index)
	{
		switch (index)
		{
		case 2001:
		case 2005:
		case 2100:
		case 2102:
		case 2200:
		case 2201:
		case 2301:
		case 2302:
		case 2304:
		case 2400:
		case 2604:
			return true;
		default:
			return false;
		}
	}

	internal static bool IsSKBoolIndex(int index)
	{
		if ((uint)(index - 3201) <= 1u || index == 3300 || index == 3308)
		{
			return true;
		}
		return false;
	}

	internal static bool IsCPBoolIndex(int index)
	{
		if (index == 4050 || index == 4202 || (uint)(index - 4304) <= 1u)
		{
			return true;
		}
		return false;
	}

	private static void ApplyMain(PlayerManager PL, WPDT_A mainStat, bool isEquip, bool boolOnly = false)
	{
		if (PL == null || mainStat == null || (boolOnly && !IsMainBoolIndex(mainStat.Index)))
		{
			return;
		}
		float num = (isEquip ? mainStat.number : (0f - mainStat.number));
		int num2 = Mathf.FloorToInt(mainStat.number);
		if (!isEquip)
		{
			num2 = -num2;
		}
		bool flag = isEquip;
		switch (mainStat.Index)
		{
		case 1:
			PL.Health_Bei += num;
			break;
		case 2:
			PL.Mana_Bei += num;
			break;
		case 3:
			PL.Health_R_Base += num;
			break;
		case 4:
			PL.Mana_R_Base += num;
			break;
		case 5:
			PL.Attack_R_health_Base += num;
			break;
		case 6:
			PL.Attack_R_mana_Base += num;
			break;
		case 10:
			PL.Damage_Bei += num;
			break;
		case 11:
			PL.ATSpeed_Bei += num;
			break;
		case 12:
			PL.MVSpeed_Bei += num;
			break;
		case 13:
			PL.BJrate += num;
			break;
		case 14:
			PL.BJDamage += num;
			break;
		case 15:
			PL.CoolDown += num;
			break;
		case 16:
			PL.ManaXH += num;
			break;
		case 17:
			PL.GeDang += num;
			break;
		case 18:
			PL.Damage_Anti += num;
			break;
		case 19:
			PL.DOTcut += num;
			break;
		case 20:
			PL.AntiSlow += num;
			break;
		case 21:
			PL.AllChuan += num;
			break;
		case 22:
			PL.AllAnti += num;
			break;
		case 30:
			PL.BJD_Anti += num;
			break;
		case 31:
			PL.JYBoss_DMG += num2;
			break;
		case 32:
			PL.JYBoss_Anti += num2;
			break;
		case 50:
			PL.ItemDrop_Rate += num;
			break;
		case 51:
			PL.FlySpeed += num;
			break;
		case 52:
			PL.ORB_Damage += num;
			break;
		case 53:
			PL.JYrate += num;
			break;
		case 54:
			PL.ThroughRate += num;
			break;
		case 60:
			PL.Health_Percent += num;
			break;
		case 61:
			PL.Mana_Percent += num;
			break;
		case 62:
			PL.DMG_R_H += num;
			break;
		case 63:
			PL.DMG_R_M += num;
			break;
		case 80:
			PL.BS_Add += num2;
			break;
		case 81:
			PL.BS_Multi += num;
			break;
		case 100:
			PL.C_Health += num;
			break;
		case 101:
			PL.C_Damage += num;
			break;
		case 102:
			PL.C_ATSpeed += num;
			break;
		case 103:
			PL.C_MVSpeed += num;
			break;
		case 104:
			PL.C_AllAnti += num;
			break;
		case 150:
			PL.WPSPC_DMG += num2;
			break;
		case 151:
			PL.WPSPC_Rate += num2;
			break;
		case 170:
			PL.BuffT_Temple += num;
			break;
		case 171:
			PL.BuffT_Drink += num;
			break;
		case 200:
			PL.Top_CD += num2;
			break;
		case 201:
			PL.Top_GD += num2;
			break;
		case 202:
			PL.Top_Anti += num2;
			break;
		case 203:
			PL.Top_Cut_DMG += num;
			break;
		case 204:
			PL.Top_Cut_ATS += num;
			break;
		case 205:
			PL.Top_Cut_MVS += num;
			break;
		case 300:
			PL.AllDot_DMG += num;
			break;
		case 301:
			PL.AllDot_Time += num;
			break;
		case 302:
			PL.AllDot_Layer += num2;
			break;
		case 303:
			PL.AllDot_MV += num;
			break;
		case 304:
			PL.AllDot_JY += num;
			break;
		case 305:
			PL.DiffDotDMG += num;
			break;
		case 306:
			PL.DiffDebuff_DMG += num2;
			break;
		case 307:
			PL.Dot_MSAll = flag;
			break;
		case 400:
			PL.BE_ZQ_DMG += num;
			break;
		case 401:
			PL.BE_ZQ_ATS += num;
			break;
		case 402:
			PL.BE_ZQ_MVS += num;
			break;
		case 403:
			PL.BE_ZQ_BJR += num;
			break;
		case 404:
			PL.BE_ZQ_BJD += num;
			break;
		case 405:
			PL.BE_ZQ_Heal += num;
			break;
		case 406:
			PL.BE_ZQ_Mana += num;
			break;
		case 407:
			PL.BE_ZQ_CP_Heal += num;
			break;
		case 408:
			PL.BE_ZQ_CP_DMG += num;
			break;
		case 409:
			PL.BE_ZQ_CP_ATS += num;
			break;
		case 410:
			PL.BE_ZQ_CP_MVS += num;
			break;
		case 411:
			PL.BE_ZQ_CP_Anti += num;
			break;
		case 412:
			PL.BE_ZQ_Dot += num;
			break;
		case 413:
			PL.BE_ZQ_XJ_DMG += num;
			break;
		case 414:
			PL.BE_ZQ_Orb_DMG += num;
			break;
		case 415:
			PL.BE_SPC_DMG += num;
			break;
		case 416:
			PL.BE_SPC_ATS += num;
			break;
		case 417:
			PL.BE_SPC_MVS += num;
			break;
		case 418:
			PL.BE_SPC_BJR += num;
			break;
		case 419:
			PL.BE_SPC_BJD += num;
			break;
		case 420:
			PL.BE_SPC_Heal += num;
			break;
		case 421:
			PL.BE_SPC_Mana += num;
			break;
		case 422:
			PL.BE_SPC_CP_Heal += num;
			break;
		case 423:
			PL.BE_SPC_CP_DMG += num;
			break;
		case 424:
			PL.BE_SPC_CP_ATS += num;
			break;
		case 425:
			PL.BE_SPC_CP_MVS += num;
			break;
		case 426:
			PL.BE_SPC_CP_Anti += num;
			break;
		case 427:
			PL.BE_SPC_Dot += num;
			break;
		case 428:
			PL.BE_SPC_XJ_DMG += num;
			break;
		case 429:
			PL.BE_SPC_Orb_DMG += num;
			break;
		case 430:
			PL.BE_HH_DMG += num;
			break;
		case 431:
			PL.BE_HH_ATS += num;
			break;
		case 432:
			PL.BE_HH_MVS += num;
			break;
		case 433:
			PL.BE_HH_BJR += num;
			break;
		case 434:
			PL.BE_HH_BJD += num;
			break;
		case 435:
			PL.BE_HH_Heal += num;
			break;
		case 436:
			PL.BE_HH_Mana += num;
			break;
		case 437:
			PL.BE_HH_CP_Heal += num;
			break;
		case 438:
			PL.BE_HH_CP_DMG += num;
			break;
		case 439:
			PL.BE_HH_CP_ATS += num;
			break;
		case 440:
			PL.BE_HH_CP_MVS += num;
			break;
		case 441:
			PL.BE_HH_CP_Anti += num;
			break;
		case 442:
			PL.BE_HH_Dot += num;
			break;
		case 443:
			PL.BE_HH_XJ_DMG += num;
			break;
		case 444:
			PL.BE_HH_Orb_DMG += num;
			break;
		case 445:
			PL.BE_SK_DMG += num;
			break;
		case 446:
			PL.BE_SK_ATS += num;
			break;
		case 447:
			PL.BE_SK_MVS += num;
			break;
		case 448:
			PL.BE_SK_CP_Heal += num;
			break;
		case 449:
			PL.BE_SK_CP_DMG += num;
			break;
		case 450:
			PL.BE_SK_CP_ATS += num;
			break;
		case 451:
			PL.BE_SK_CP_Anti += num;
			break;
		case 452:
			PL.BE_SK_XJ_DMG += num;
			break;
		case 453:
			PL.BE_SK_Orb_DMG += num;
			break;
		case 454:
			PL.BE_SK_FQ_Count += num2;
			break;
		case 455:
			PL.BE_BS_DMG += num;
			break;
		case 456:
			PL.BE_BS_ATS += num;
			break;
		case 457:
			PL.BE_BS_MVS += num;
			break;
		case 458:
			PL.BE_BS_CP_Heal += num;
			break;
		case 459:
			PL.BE_BS_CP_DMG += num;
			break;
		case 460:
			PL.BE_BS_CP_ATS += num;
			break;
		case 461:
			PL.BE_BS_CP_Anti += num;
			break;
		case 462:
			PL.BE_BS_XJ_DMG += num;
			break;
		case 463:
			PL.BE_BS_Orb_DMG += num;
			break;
		case 464:
			PL.BE_BS_FQ_Count += num2;
			break;
		case 500:
			PL.LowH_DMG20 += num2;
			break;
		case 501:
			PL.LowH_DMG50 += num2;
			break;
		case 502:
			PL.HighH_DMG90 += num2;
			break;
		case 503:
			PL.HighH_DMG100 += num2;
			break;
		case 504:
			PL.LowH_HurtR20 += num2;
			break;
		case 505:
			PL.HighH_HurtR100 += num2;
			break;
		case 506:
			PL.LowH_DMGAnti20 += num2;
			break;
		case 507:
			PL.LowH_DMGAnti50 += num2;
			break;
		case 508:
			PL.LowH_CritAnti10 = flag;
			break;
		case 509:
			PL.LowM_DMG20 += num2;
			break;
		case 510:
			PL.LowM_DMG50 += num2;
			break;
		case 511:
			PL.HighM_DMG90 += num2;
			break;
		case 512:
			PL.HighM_DMG100 += num2;
			break;
		case 513:
			PL.LowM_HurtR20 += num2;
			break;
		case 514:
			PL.HighM_HurtR100 += num2;
			break;
		case 550:
			PL.ST_MV_DMG += num2;
			break;
		case 551:
			PL.ST_MV_ATS += num2;
			break;
		case 552:
			PL.ST_MV_GD += num2;
			break;
		case 553:
			PL.ST_NoMV_DMG += num2;
			break;
		case 554:
			PL.ST_NoMV_ATS += num2;
			break;
		case 555:
			PL.ST_NoMV_DMGAnti += num2;
			break;
		case 556:
			PL.ST_NoMV_HealPrc += num;
			break;
		case 557:
			PL.ST_NoMV_ManaPrc += num;
			break;
		case 558:
			PL.ST_Chong_DMG += num2;
			break;
		case 559:
			PL.ST_Chong_Anti += num2;
			break;
		case 600:
			PL.Z_Hmax_DMG += num;
			break;
		case 601:
			PL.Z_Huse_DMG += num;
			break;
		case 602:
			PL.Z_Mmax_DMG += num;
			break;
		case 603:
			PL.Z_Mcur_DMG += num;
			break;
		case 604:
			PL.Z_Muse_DMG += num;
			break;
		case 610:
			EL_Float(mainStat.EL, num, ref PL.Z_Hmax_EL0, ref PL.Z_Hmax_EL1, ref PL.Z_Hmax_EL2, ref PL.Z_Hmax_EL3, ref PL.Z_Hmax_EL4, ref PL.Z_Hmax_EL5);
			break;
		case 611:
			EL_Float(mainStat.EL, num, ref PL.Z_Mmax_EL0, ref PL.Z_Mmax_EL1, ref PL.Z_Mmax_EL2, ref PL.Z_Mmax_EL3, ref PL.Z_Mmax_EL4, ref PL.Z_Mmax_EL5);
			break;
		case 612:
			EL_Float(mainStat.EL, num, ref PL.Z_CD_EL0, ref PL.Z_CD_EL1, ref PL.Z_CD_EL2, ref PL.Z_CD_EL3, ref PL.Z_CD_EL4, ref PL.Z_CD_EL5);
			break;
		case 613:
			EL_Int(mainStat.EL, num2, ref PL.Z_Anti0_EL0, ref PL.Z_Anti0_EL1, ref PL.Z_Anti0_EL2, ref PL.Z_Anti0_EL3, ref PL.Z_Anti0_EL4, ref PL.Z_Anti0_EL5);
			break;
		case 614:
			EL_Int(mainStat.EL, num2, ref PL.Z_Chuan0_EL0, ref PL.Z_Chuan0_EL1, ref PL.Z_Chuan0_EL2, ref PL.Z_Chuan0_EL3, ref PL.Z_Chuan0_EL4, ref PL.Z_Chuan0_EL5);
			break;
		case 615:
			EL_Int(mainStat.EL, num2, ref PL.Z_GD_EL0, ref PL.Z_GD_EL1, ref PL.Z_GD_EL2, ref PL.Z_GD_EL3, ref PL.Z_GD_EL4, ref PL.Z_GD_EL5);
			break;
		case 616:
			EL_Int(mainStat.EL, num2, ref PL.Z_BJR_EL0, ref PL.Z_BJR_EL1, ref PL.Z_BJR_EL2, ref PL.Z_BJR_EL3, ref PL.Z_BJR_EL4, ref PL.Z_BJR_EL5);
			break;
		case 617:
			EL_Int(mainStat.EL, num2, ref PL.Z_DMGCut_EL0, ref PL.Z_DMGCut_EL1, ref PL.Z_DMGCut_EL2, ref PL.Z_DMGCut_EL3, ref PL.Z_DMGCut_EL4, ref PL.Z_DMGCut_EL5);
			break;
		case 618:
			EL_Int(mainStat.EL, num2, ref PL.Z_Thr_EL0, ref PL.Z_Thr_EL1, ref PL.Z_Thr_EL2, ref PL.Z_Thr_EL3, ref PL.Z_Thr_EL4, ref PL.Z_Thr_EL5);
			break;
		case 650:
			PL.Z_CD_CP_DMG += num;
			break;
		case 651:
			PL.Z_ATS_CP_DMG += num;
			break;
		case 652:
			PL.Z_MVS_DMG += num;
			break;
		case 653:
			PL.Z_MVS_ATS += num;
			break;
		case 654:
			PL.Z_BJR_BJD = flag;
			break;
		case 655:
			EL_Int(mainStat.EL, num2, ref PL.Z_Chuan0_BJD, ref PL.Z_Chuan1_BJD, ref PL.Z_Chuan2_BJD, ref PL.Z_Chuan3_BJD, ref PL.Z_Chuan4_BJD, ref PL.Z_Chuan5_BJD);
			break;
		case 700:
			PL.ST_EveryH_DMG += num;
			break;
		case 701:
			PL.ST_EveryM_Drop += num;
			break;
		case 750:
			PL.AB_DMG_Mana = flag;
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshCD();
			}
			break;
		case 751:
			PL.AB_DMG_Hurt = flag;
			break;
		case 752:
			PL.AB_Dot_DMG = flag;
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RebuildDotRuntimeDataFromTalent();
			}
			break;
		case 753:
			PL.NoGD = flag;
			break;
		case 800:
			PL.Attack_DMG1 += num2;
			break;
		case 801:
			PL.Attack_DMG2 += num2;
			break;
		case 802:
			PL.Attack_ATS1 += num2;
			break;
		case 803:
			PL.Attack_ATS2 += num2;
			break;
		case 804:
			PL.Attack_Chuan += num2;
			break;
		case 805:
			PL.Attack_BJR += num2;
			break;
		case 806:
			PL.Attack_BJD += num2;
			break;
		case 807:
			PL.Attack_DotDMG1 += num2;
			break;
		case 808:
			PL.Attack_DotDMG2 += num2;
			break;
		case 850:
			PL.BuffEvery_CP += num2;
			break;
		case 851:
			PL.Z_Dot_EL += num2;
			break;
		case 852:
			PL.Z_Dot_MV += num2;
			break;
		case 853:
			PL.Clear1 += num2;
			break;
		case 854:
			PL.Clear2 += num2;
			break;
		case 855:
			PL.GD_DMG += num2;
			break;
		case 856:
			PL.Final_Diff_DMG += num2;
			break;
		case 857:
			PL.PickBS_MVS += num2;
			break;
		case 858:
			PL.NoUseSK_DMG1 += num2;
			break;
		case 859:
			PL.NoUseSK_DMG2 += num2;
			break;
		case 860:
			PL.TP_DMG += num2;
			break;
		case 861:
			PL.MV_DMG += num2;
			break;
		case 862:
			PL.DeadWD = flag;
			break;
		case 863:
			PL.DeadRageWD = flag;
			break;
		case 864:
			PL.DeadStealthWD = flag;
			break;
		case 1000:
			PL.CP1_DMG += num;
			break;
		case 1001:
			PL.CP1_ATS += num;
			break;
		case 1002:
			PL.CP1_MVS += num;
			break;
		case 1003:
			PL.CP1_Heal += num;
			break;
		case 1004:
			PL.CP1_Mana += num;
			break;
		case 1005:
			PL.CP1_DMG_Anti += num;
			break;
		case 1006:
			PL.CP1_DropR += num;
			break;
		case 1007:
			PL.CP1_ORB_DMG += num;
			break;
		case 1010:
			EL_Float(mainStat.EL, num, ref PL.CP1_DMG0, ref PL.CP1_DMG1, ref PL.CP1_DMG2, ref PL.CP1_DMG3, ref PL.CP1_DMG4, ref PL.CP1_DMG5);
			break;
		case 1011:
			EL_Float(mainStat.EL, num, ref PL.CP1_Chuan0, ref PL.CP1_Chuan1, ref PL.CP1_Chuan2, ref PL.CP1_Chuan3, ref PL.CP1_Chuan4, ref PL.CP1_Chuan5);
			break;
		case 1020:
			PL.CP1_CP_Heal += num;
			break;
		case 1021:
			PL.CP1_CP_DMG += num;
			break;
		case 1022:
			PL.CP1_CP_ATS += num;
			break;
		case 1023:
			PL.CP1_CP_AllAnti += num;
			break;
		case 1024:
			PL.CLass_DMG += num;
			break;
		case 1025:
			PL.CLass_ATS += num;
			break;
		case 1026:
			PL.CLass_MVS += num;
			break;
		case 1027:
			PL.CLass_Heal += num;
			break;
		case 1028:
			PL.CLass_Mana += num;
			break;
		case 1029:
			PL.CLass_DMG_Anti += num;
			break;
		case 1030:
			PL.CLass_DropR += num;
			break;
		case 1031:
			PL.CLass_ORB_DMG += num;
			break;
		case 1040:
			EL_Float(mainStat.EL, num, ref PL.CLass_DMG0, ref PL.CLass_DMG1, ref PL.CLass_DMG2, ref PL.CLass_DMG3, ref PL.CLass_DMG4, ref PL.CLass_DMG5);
			break;
		case 1041:
			EL_Float(mainStat.EL, num, ref PL.CLass_Chuan0, ref PL.CLass_Chuan1, ref PL.CLass_Chuan2, ref PL.CLass_Chuan3, ref PL.CLass_Chuan4, ref PL.CLass_Chuan5);
			break;
		case 1050:
			PL.CLass_CP_Heal += num;
			break;
		case 1051:
			PL.CLass_CP_DMG += num;
			break;
		case 1052:
			PL.CLass_CP_ATS += num;
			break;
		case 1053:
			PL.CLass_CP_AllAnti += num;
			break;
		case 1054:
			PL.Class_CP_DotDMG += num;
			break;
		case 1100:
			PL.DMG_1 += num;
			break;
		case 1101:
			PL.DMG_2 += num;
			break;
		case 1102:
			PL.DMG_3 += num;
			break;
		case 1103:
			PL.DMG_4 += num;
			break;
		case 1104:
			PL.DMG_5 += num;
			break;
		case 1105:
			PL.DMG_6 += num;
			break;
		case 1106:
			PL.DMG_7 += num;
			break;
		case 1107:
			PL.DMG_8 += num;
			break;
		case 1108:
			PL.DMG_9 += num;
			break;
		case 1109:
			PL.DMG_10 += num;
			break;
		case 1110:
			PL.DMG_11 += num;
			break;
		case 1111:
			PL.DMG_12 += num;
			break;
		case 1112:
			PL.DMG_13 += num;
			break;
		case 1113:
			PL.DMG_14 += num;
			break;
		case 1114:
			PL.DMG_15 += num;
			break;
		case 1115:
			PL.DMG_16 += num;
			break;
		case 1116:
			PL.DMG_17 += num;
			break;
		case 1117:
			PL.DMG_18 += num;
			break;
		case 1118:
			PL.DMG_19 += num;
			break;
		case 1119:
			PL.DMG_20 += num;
			break;
		case 1120:
			PL.DMG_30 += num;
			break;
		case 1121:
			PL.DMG_40 += num;
			break;
		case 1122:
			PL.DMG_41 += num;
			break;
		case 1123:
			PL.DMG_42 += num;
			break;
		case 1124:
			PL.DMG_43 += num;
			break;
		case 1125:
			PL.DMG_44 += num;
			break;
		case 1126:
			PL.DMG_45 += num;
			break;
		case 1127:
			PL.DMG_51 += num;
			break;
		case 1128:
			PL.DMG_52 += num;
			break;
		case 1129:
			PL.DMG_53 += num;
			break;
		case 1130:
			PL.DMG_54 += num;
			break;
		case 1131:
			PL.DMG_55 += num;
			break;
		case 1132:
			PL.DMG_56 += num;
			break;
		case 1133:
			PL.DMG_70 += num;
			break;
		case 1134:
			PL.DMG_71 += num;
			break;
		case 1135:
			PL.DMG_72 += num;
			break;
		case 1136:
			PL.DMG_73 += num;
			break;
		case 1137:
			PL.DMG_74 += num;
			break;
		case 1138:
			PL.DMG_75 += num;
			break;
		case 1139:
			PL.DMG_80 += num;
			break;
		case 1140:
			PL.DMG_81 += num;
			break;
		case 1141:
			PL.DMG_82 += num;
			break;
		case 1142:
			PL.DMG_83 += num;
			break;
		case 1143:
			PL.DMG_84 += num;
			break;
		case 1144:
			PL.DMG_85 += num;
			break;
		case 1145:
			PL.DMG_86 += num;
			break;
		case 1146:
			PL.DMG_90 += num;
			break;
		case 1150:
			PL.XJ_Count_CP_DMG += num2;
			break;
		case 1200:
			PL.EMC_DMG_20 += num;
			break;
		case 1201:
			PL.EMC_DMG_48 += num;
			break;
		case 1202:
			PL.EMC_Anti_9 += num;
			break;
		case 1203:
			PL.EMC_GD_12 += num;
			break;
		case 1204:
			PL.JYC_DMG_15 += num;
			break;
		case 1205:
			PL.JYC_ATS_24 += num;
			break;
		case 1206:
			PL.JYC_BJD_24 += num;
			break;
		case 1250:
			PL.Kem_DMG1 += num2;
			break;
		case 1251:
			PL.Kem_DMG2 += num2;
			break;
		case 1252:
			PL.Kem_ATS1 += num2;
			break;
		case 1253:
			PL.Kem_ATS2 += num2;
			break;
		case 1260:
			EL_Int(mainStat.EL, num2, ref PL.Kem_EL0, ref PL.Kem_EL1, ref PL.Kem_EL2, ref PL.Kem_EL3, ref PL.Kem_EL4, ref PL.Kem_EL5);
			break;
		case 1270:
			PL.Kem_CP_DMG1 += num2;
			break;
		case 1271:
			PL.Kem_CP_DMG2 += num2;
			break;
		case 1272:
			PL.Kem_CP_ATS1 += num2;
			break;
		case 1273:
			PL.Kem_CP_ATS2 += num2;
			break;
		case 1274:
			PL.Kjy_DMG += num2;
			break;
		case 1275:
			PL.Kjy_AllAnti += num2;
			break;
		case 1276:
			PL.Kem_Refresh += num2;
			break;
		case 1300:
			EL_Int(mainStat.EL, num2, ref PL.PrcCut0, ref PL.PrcCut1, ref PL.PrcCut2, ref PL.PrcCut3, ref PL.PrcCut4, ref PL.PrcCut5);
			break;
		case 1301:
			EL_Int(mainStat.EL, num2, ref PL.PrcCut5P0, ref PL.PrcCut5P1, ref PL.PrcCut5P2, ref PL.PrcCut5P3, ref PL.PrcCut5P4, ref PL.PrcCut5P5);
			break;
		case 1302:
			EL_Int(mainStat.EL, num2, ref PL.PrcCut3P0, ref PL.PrcCut3P1, ref PL.PrcCut3P2, ref PL.PrcCut3P3, ref PL.PrcCut3P4, ref PL.PrcCut3P5);
			break;
		case 1330:
			EL_Int(mainStat.EL, num2, ref PL.BurnLife0, ref PL.BurnLife1, ref PL.BurnLife2, ref PL.BurnLife3, ref PL.BurnLife4, ref PL.BurnLife5);
			break;
		case 1350:
			EL_to_Bool(mainStat.EL, flag, ref PL.WS_Anti0, ref PL.WS_Anti1, ref PL.WS_Anti2, ref PL.WS_Anti3, ref PL.WS_Anti4, ref PL.WS_Anti5);
			break;
		case 1360:
			PL.WS_All = flag;
			break;
		case 1370:
			PL.EM_LowH_DMG20 += num2;
			break;
		case 1371:
			PL.EM_LowH_DMG50 += num2;
			break;
		case 1372:
			PL.EM_HighH_DMG60 += num2;
			break;
		case 1373:
			PL.EM_HighH_DMG100 += num2;
			break;
		case 1374:
			PL.EM_Heal_Crit += num2;
			break;
		case 1390:
			PL.Dis_In += num2;
			break;
		case 1391:
			PL.Dis_Out = flag;
			break;
		case 1395:
			PL.Crit_BoomEXP += num2;
			break;
		case 1396:
			PL.Crit_BoomDie_Rate += num2;
			break;
		case 1397:
			PL.Crit_MS += num2;
			break;
		case 1500:
			PL.ORB_FQ_Count += num2;
			break;
		case 1501:
			PL.ORB_FQ_Count_Double = flag;
			break;
		case 1502:
			PL.ORB_FQ_DMG80_Base += num2;
			break;
		case 1503:
			PL.ORB_FQ_DMG120_Base += num2;
			break;
		case 1504:
			PL.Orb_Universe_DMG_Base += num;
			break;
		case 1505:
			PL.HighMana_DMG100_FQ += num2;
			break;
		case 1506:
			PL.Orb_Universe_ATS += num;
			break;
		case 1507:
			PL.Orb_Bow_DMG += num;
			break;
		case 1508:
			PL.Orb_Bow_ATS += num;
			break;
		case 1509:
			PL.Orb_Bow_DMG_ORB += num;
			break;
		case 1510:
			PL.Orb_Bow_DMG_Anti += num;
			break;
		case 1600:
			PL.XJ_DMG += num2;
			break;
		case 1601:
			PL.XJ_Time += num2;
			break;
		case 1602:
			PL.TuT_Buff += num2;
			break;
		case 1603:
			PL.TuT_Time += num2;
			break;
		case 1604:
			PL.TuT_PlayerAll = flag;
			break;
		case 1800:
			PL.NoDot_BJD += num2;
			break;
		case 1801:
			PL.HealCutMana = flag;
			break;
		case 1802:
			PL.ManaUse_Rheal += num2;
			break;
		case 1803:
			PL.RMana_RHeal = flag;
			break;
		case 1804:
			PL.CP_Same_RHeal = flag;
			break;
		case 1805:
			PL.FT = flag;
			break;
		case 1806:
			PL.DMG_ManaPRC += num2;
			break;
		case 1807:
			PL.Turtle = flag;
			break;
		case 1808:
			PL.GD_HurtR += num2;
			break;
		case 1809:
			PL.BloodLost = flag;
			break;
		case 1810:
			PL.NoGround = flag;
			break;
		case 1811:
			PL.CPNoBad = flag;
			break;
		case 1812:
			PL.CPNoGround = flag;
			break;
		case 1813:
			PL.AT_UseHeal1 = flag;
			break;
		case 1814:
			PL.AT_UseHeal2 = flag;
			break;
		case 1815:
			PL.DMGsplit += num;
			break;
		case 1816:
			PL.BladeSoul_Double = flag;
			break;
		case 1817:
			PL.Diff_EL += num2;
			break;
		case 1818:
			PL.EXP_Range += num;
			break;
		case 1819:
			PL.Buff_Range += num;
			break;
		case 1820:
			PL.MoneyTO_DMG = flag;
			break;
		case 1821:
			PL.AutoJH = flag;
			break;
		case 1822:
			PL.DieEXP = flag;
			break;
		case 1900:
			PL.AutoDrinkH = flag;
			break;
		case 1901:
			PL.AutoDrinkM = flag;
			break;
		case 1905:
			PL.Drink_CP = flag;
			break;
		case 1910:
			PL.DrinkPre_Heal += num2;
			break;
		case 1911:
			PL.DrinkPre_Mana += num2;
			break;
		case 1912:
			PL.DrinkPre_DMG += num2;
			break;
		case 1950:
			PL.Pick_PL_Bei += num;
			break;
		case 1951:
			PL.Pick_XJL_Bei += num;
			break;
		case 1952:
			PL.XJL_SellPrice += num;
			break;
		case 1953:
			PL.XJL_DMG += num;
			break;
		case 1954:
			PL.XJL_DropMulti += num;
			break;
		case 1955:
			PL.XJL_UseSKTime += num;
			break;
		}
	}

	private static void ApplyDot(PlayerManager PL, WPDT_A dotStat, bool isEquip, bool boolOnly = false)
	{
		if (PL == null || dotStat == null || (boolOnly && !IsDotBoolIndex(dotStat.Index)))
		{
			return;
		}
		PL.EnsurePlayerDotData();
		if (PL.DOT == null || dotStat.EL < 0 || dotStat.EL >= PL.DOT.Length)
		{
			return;
		}
		PlayerDotData playerDotData = PL.DOT[dotStat.EL];
		if (playerDotData != null)
		{
			int num = Mathf.FloorToInt(dotStat.number);
			if (!isEquip)
			{
				num = -num;
			}
			bool flag = isEquip;
			switch (dotStat.Index)
			{
			case 2000:
				playerDotData.Every_Layer += num;
				break;
			case 2001:
				playerDotData.Crit_One = flag;
				break;
			case 2002:
				playerDotData.FJ += num;
				break;
			case 2003:
				playerDotData.DMG_AddOne += num;
				break;
			case 2004:
				playerDotData.All_LayerR += num;
				break;
			case 2005:
				playerDotData.Double_Layer = flag;
				break;
			case 2100:
				playerDotData.Dot_Infect = flag;
				break;
			case 2101:
				playerDotData.Dot_Infect_Layer += num;
				break;
			case 2102:
				playerDotData.Dot_Infect_All = flag;
				break;
			case 2200:
				playerDotData.YB = flag;
				break;
			case 2201:
				playerDotData.YB_half = flag;
				break;
			case 2202:
				playerDotData.YB_Add += num;
				break;
			case 2203:
				playerDotData.YB_MS += num;
				break;
			case 2300:
				playerDotData.YS += num;
				break;
			case 2301:
				playerDotData.SL = flag;
				break;
			case 2302:
				playerDotData.CM = flag;
				break;
			case 2303:
				playerDotData.MH += num;
				break;
			case 2304:
				playerDotData.ZZ = flag;
				break;
			case 2305:
				playerDotData.JY += num;
				break;
			case 2306:
				playerDotData.Dead += num;
				break;
			case 2400:
				playerDotData.Dot_Crit = flag;
				break;
			case 2401:
				playerDotData.BoomDMGUp += num;
				break;
			case 2402:
				playerDotData.LayerPRC += num;
				break;
			case 2450:
				playerDotData.BE_CP += num;
				break;
			case 2500:
				playerDotData.BF_DMG += num;
				break;
			case 2501:
				playerDotData.DMG50 += num;
				break;
			case 2550:
				playerDotData.LowH_50 += num;
				break;
			case 2551:
				playerDotData.HighH_100 += num;
				break;
			case 2552:
				playerDotData.LowM_40 += num;
				break;
			case 2600:
				playerDotData.FrozenFoever += num;
				break;
			case 2601:
				playerDotData.FrozenCut += num;
				break;
			case 2602:
				playerDotData.Frozen30 += num;
				break;
			case 2603:
				playerDotData.FrozenHurtDMG += num;
				break;
			case 2604:
				playerDotData.FrozenForeverDot = flag;
				break;
			}
		}
	}

	private static void ApplySK(PlayerManager PL, WPDT_B skStat, bool isEquip, bool boolOnly = false)
	{
		if (PL == null || skStat == null || string.IsNullOrEmpty(skStat.SkillName) || !SingletonMonoScope<TalentManager>.HasInstance || (boolOnly && !IsSKBoolIndex(skStat.Index)))
		{
			return;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance == null || instance.XiData == null)
		{
			return;
		}
		float num = (isEquip ? skStat.number : (0f - skStat.number));
		int num2 = Mathf.FloorToInt(skStat.number);
		if (!isEquip)
		{
			num2 = -num2;
		}
		bool flag = isEquip;
		for (int i = 0; i < instance.XiData.Length; i++)
		{
			SkillXiData skillXiData = instance.XiData[i];
			if (skillXiData != null && skillXiData.Sample_F != null && skillXiData.Sample_F.TryGetValue(skStat.SkillName, out var value) && value != null)
			{
				switch (skStat.Index)
				{
				case 3000:
					ApplySkillChangeData(instance, value, skStat, isEquip);
					break;
				case 3100:
					value.CT_F += num2;
					break;
				case 3101:
					value.CT_S += num2;
					break;
				case 3102:
					value.CT_AT += num2;
					break;
				case 3103:
					value.CT_Mul += num2;
					break;
				case 3200:
					ApplySkillLink(value, skStat.LinkSK, isEquip);
					break;
				case 3201:
					value.LinkAll = flag;
					break;
				case 3202:
					value.EveryLink = flag;
					break;
				case 3203:
					ApplySkillJcSkill(value, skStat.LinkSK, isEquip);
					break;
				case 3300:
					value.AutoUse = flag;
					break;
				case 3301:
					value.Refresh += num2;
					break;
				case 3302:
					value.ATtar_DMG += num2;
					break;
				case 3303:
					value.CompUP_DMG += num2;
					break;
				case 3304:
					value.ATtarUP += num2;
					break;
				case 3305:
					value.MS_Dead += num2;
					break;
				case 3306:
					value.GD_Use += num2;
					break;
				case 3307:
					value.BSAT_DMG += num2;
					break;
				case 3308:
					value.Double = flag;
					break;
				case 3400:
					value.WD += num;
					break;
				case 3401:
					value.Crit_Time += num2;
					break;
				case 3402:
					value.Crit_CD += num2;
					break;
				case 3403:
					value.Over_Prc += num2;
					break;
				case 3404:
					value.CutSpeedZone += num2;
					break;
				case 3500:
					value.UseDMG += num2;
					break;
				case 3501:
					value.UseATS += num2;
					break;
				case 3502:
					value.UseMVS += num2;
					break;
				case 3503:
					value.UseCP_DMG += num2;
					break;
				case 3504:
					value.UseCP_ATS += num2;
					break;
				case 3530:
					ApplySkillUseDamageElement(value, skStat.EL, num2);
					break;
				case 3535:
					ApplySkillUseChuanElement(value, skStat.EL, num2);
					break;
				case 3550:
					value.Has_DMG += num2;
					break;
				case 3551:
					value.Has_ATS += num2;
					break;
				case 3552:
					value.Has_MVS += num2;
					break;
				case 3553:
					value.Has_BJR += num2;
					break;
				case 3554:
					value.Has_BJD += num2;
					break;
				case 3555:
					value.Has_DotTimeCut += num2;
					break;
				case 3556:
					value.Has_DMG_Cut += num2;
					break;
				case 3557:
					value.Has_GD += num2;
					break;
				case 3558:
					value.Has_ORB_DMG += num2;
					break;
				case 3559:
					value.Has_XJ_DMG += num2;
					break;
				case 3560:
					value.Has_Dot_DMG += num2;
					break;
				case 3561:
					value.Has_CP_DMG += num2;
					break;
				}
			}
		}
	}

	private static void ApplyCP(PlayerManager PL, WPDT_B cpStat, bool isEquip, bool boolOnly = false)
	{
		if (PL == null || cpStat == null || string.IsNullOrEmpty(cpStat.SkillName) || !SingletonMonoScope<TalentManager>.HasInstance || (boolOnly && !IsCPBoolIndex(cpStat.Index)))
		{
			return;
		}
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (instance == null || instance.XiData == null)
		{
			return;
		}
		int num = Mathf.FloorToInt(cpStat.number);
		if (!isEquip)
		{
			num = -num;
		}
		bool flag = isEquip;
		for (int i = 0; i < instance.XiData.Length; i++)
		{
			SkillXiData skillXiData = instance.XiData[i];
			if (skillXiData != null && skillXiData.Comp_F != null && skillXiData.Comp_F.TryGetValue(cpStat.SkillName, out var value) && value != null)
			{
				switch (cpStat.Index)
				{
				case 4000:
					ApplyCompSkillChangeData(instance, value, cpStat, isEquip);
					break;
				case 4050:
					value.AutoSummonOnReborn = flag;
					break;
				case 4100:
					value.Summon_count_Other += num;
					break;
				case 4101:
					value.Summon_count_Type = (isEquip ? Mathf.FloorToInt(cpStat.number) : 0);
					break;
				case 4200:
					value.CT_FS += num;
					break;
				case 4201:
					value.CT_Double = (isEquip ? Mathf.FloorToInt(cpStat.number) : 0);
					break;
				case 4202:
					value.AT_Double = flag;
					break;
				case 4300:
					value.GD_R_Heal += num;
					break;
				case 4301:
					value.BloodDie += num;
					break;
				case 4302:
					value.TGYJ += num;
					break;
				case 4303:
					value.AT_DotLayer += num;
					break;
				case 4304:
					value.BJ_NoDot = flag;
					break;
				case 4305:
					value.WS_All = flag;
					break;
				case 4306:
					value.Field_Range += num;
					break;
				case 4307:
					value.Kill_R_Heal += num;
					break;
				case 4308:
					value.Hurt_FT += num;
					break;
				case 4400:
					value.EveryDMG += num;
					break;
				case 4401:
					value.EveryChuan += num;
					break;
				case 4402:
					value.EveryATS += num;
					break;
				case 4403:
					value.EveryMVS += num;
					break;
				case 4404:
					value.EveryHeal += num;
					break;
				case 4405:
					value.EveryMana += num;
					break;
				case 4406:
					value.EveryCD += num;
					break;
				case 4407:
					value.EveryBJR += num;
					break;
				case 4408:
					value.EveryBJD += num;
					break;
				case 4409:
					value.EveryGD += num;
					break;
				case 4410:
					value.EveryDMG_Anti += num;
					break;
				case 4411:
					value.EveryDotTimeCut += num;
					break;
				case 4412:
					value.EveryAllChuan += num;
					break;
				case 4413:
					value.EveryAllAnti += num;
					break;
				case 4414:
					value.EveryDrop += num;
					break;
				case 4415:
					value.EveryXJ_DMG += num;
					break;
				case 4416:
					value.EveryORB_DMG += num;
					break;
				case 4417:
					value.EveryDot_DMG += num;
					break;
				}
			}
		}
	}

	private void ApplySet(PlayerManager PL, bool isEquip)
	{
		if (PL == null || Set_Index <= 0 || !SingletonMonoScope<ItemManager>.HasInstance || !TryGetSetData(out var setData) || setData.Lit == null)
		{
			return;
		}
		int num = (isEquip ? PL.AddEquippedSetCount(Set_Index) : PL.RemoveEquippedSetCount(Set_Index));
		int equippedSetCount = PL.GetEquippedSetCount(Set_Index);
		if (num != equippedSetCount)
		{
			int num2 = (isEquip ? (equippedSetCount - 2) : (num - 2));
			if (num2 >= 0 && num2 < setData.Lit.Length)
			{
				ApplySetLit(PL, setData, setData.Lit[num2], isEquip);
			}
		}
	}

	private static void ApplySetLit(PlayerManager PL, Set_DT setData, Set_DT_Lit lit, bool isEquip)
	{
		if (!(PL == null) && setData != null && lit != null)
		{
			switch (lit.MainTP)
			{
			case 0:
				ApplyMain(PL, new WPDT_A
				{
					Index = lit.Index,
					EL = lit.EL,
					number = lit.Number
				}, isEquip);
				break;
			case 1:
				ApplyDot(PL, new WPDT_A
				{
					Index = lit.Index,
					EL = lit.EL,
					number = lit.Number
				}, isEquip);
				break;
			case 2:
				ApplySK(PL, new WPDT_B
				{
					SkillName = lit.SkillName,
					Index = lit.Index,
					GlobleID = lit.GlobleID,
					EL = lit.EL,
					number = lit.Number,
					LinkSK = lit.LinkSK
				}, isEquip);
				break;
			case 3:
				ApplyCP(PL, new WPDT_B
				{
					SkillName = lit.SkillName,
					Index = lit.Index,
					GlobleID = lit.GlobleID,
					EL = lit.EL,
					number = lit.Number,
					LinkSK = lit.LinkSK
				}, isEquip);
				break;
			case 10:
				ApplySetLayerBuff(PL, setData, lit, isEquip);
				break;
			}
		}
	}

	private static void ApplySetLayerBuff(PlayerManager PL, Set_DT setData, Set_DT_Lit lit, bool isEquip)
	{
		if (!(PL == null) && !(PL.BuffRuntime == null) && setData != null && setData.SetID > 0)
		{
			if (!isEquip)
			{
				PL.BuffRuntime.UnregisterSetLayerBuff(setData.SetID);
			}
			else if (!string.IsNullOrEmpty(setData.BuffName) && setData.LayerMax > 0)
			{
				Buff_PL_Layer buff_PL_Layer = new Buff_PL_Layer
				{
					BuffName = setData.BuffName,
					BuffType = setData.BuffType,
					BuffTime = setData.BuffTime,
					LayerMax = setData.LayerMax,
					Type_Layer = setData.TP_Layer,
					Number_Layer = setData.NumberL,
					Type_Max = setData.TP_Max,
					Number_Max = setData.NumberM,
					damageType = TalentManager.GiveElement(lit.EL)
				};
				buff_PL_Layer.Normalize();
				PL.BuffRuntime.RegisterSetLayerBuff(setData.SetID, buff_PL_Layer);
			}
		}
	}

	private static void RefreshCPRuntime(WPDT_B[] cpStats)
	{
		if (!SingletonMonoScope<ACTbar>.HasInstance)
		{
			return;
		}
		SingletonMonoScope<ACTbar>.Instance.RefreshStep();
		if (cpStats == null)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (WPDT_B wPDT_B in cpStats)
		{
			if (wPDT_B != null && !string.IsNullOrEmpty(wPDT_B.SkillName) && hashSet.Add(wPDT_B.SkillName))
			{
				SingletonMonoScope<ACTbar>.Instance.ValidateCompCount(wPDT_B.SkillName);
			}
		}
	}

	private static void RefreshSkillRuntimeData()
	{
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RefreshCD();
		}
	}

	private static void ApplyCompSkillChangeData(TalentManager TL, SkillData_Comp_Father skill, WPDT_B cpStat, bool isEquip)
	{
		CompSkillChangeData value;
		if (!isEquip)
		{
			ResetCompSkillChangeData(skill);
		}
		else if (TL.CPC_Data != null && TL.CPC_Data.TryGetValue(cpStat.GlobleID, out value) && value != null)
		{
			skill.BStype = value.BStype;
			skill.AT_ZD = value.AT_ZD;
			skill.AT_FStype = value.AT_FStype;
			skill.AT_DMG = value.AT_DMG;
			skill.AT_CT = value.AT_CT;
			skill.AT_CT_AT = value.AT_CT_AT;
			skill.AT_CT_Multi = value.AT_CT_Multi;
			skill.AT_FStime = value.AT_FStime;
			skill.AT_Angle = value.AT_Angle;
			skill.SK_ZD = value.SK_ZD;
			skill.SK_FStype = value.SK_FStype;
			skill.SK_DMG = value.SK_DMG;
			skill.SK_CT = value.SK_CT;
			skill.SK_CT_AT = value.SK_CT_AT;
			skill.SK_CT_Multi = value.SK_CT_Multi;
			skill.SK_FStime = value.SK_FStime;
			skill.SK_Angle = value.SK_Angle;
		}
	}

	private static void ResetCompSkillChangeData(SkillData_Comp_Father skill)
	{
		skill.BStype = 0;
		skill.AT_ZD = 100000;
		skill.AT_FStype = 100000;
		skill.AT_DMG = 100;
		skill.AT_CT = 0;
		skill.AT_CT_AT = 0;
		skill.AT_CT_Multi = 0;
		skill.AT_FStime = 100;
		skill.AT_Angle = 100;
		skill.SK_ZD = 100000;
		skill.SK_FStype = 100000;
		skill.SK_DMG = 100;
		skill.SK_CT = 0;
		skill.SK_CT_AT = 0;
		skill.SK_CT_Multi = 0;
		skill.SK_FStime = 100;
		skill.SK_Angle = 100;
	}

	private static void ApplySkillChangeData(TalentManager TL, SkillData_Sample_Father skill, WPDT_B skStat, bool isEquip)
	{
		if (!isEquip)
		{
			skill.OBJ_Group = 0;
			skill.FS_ZD_F = 100000;
			skill.FS_ZD_S = 100000;
			skill.FS_Dic_F = 100000;
			skill.FS_Type_F = 100000;
			skill.FS_Type_Dic_F = 100000;
			skill.FS_DMG = 100f;
			skill.FS_CT_F = 0;
			skill.FS_CT_S = 0;
			skill.FS_CT_AT = 0;
			skill.FS_CT_Multi = 0;
			skill.FS_Time1 = 100;
			skill.FS_Time2 = 100;
			skill.FS_Range1 = 100;
			skill.FS_AngleA = 100;
		}
		else if (TL.SKC_Data != null)
		{
			SkilChangeData skilChangeData = TL.SKC_Data.FirstOrDefault((SkilChangeData x) => x != null && x.GlobleID == skStat.GlobleID);
			if (skilChangeData != null)
			{
				skill.OBJ_Group = skilChangeData.OBJ_Group;
				skill.FS_ZD_F = skilChangeData.FS_ZD_F;
				skill.FS_ZD_S = skilChangeData.FS_ZD_S;
				skill.FS_Dic_F = skilChangeData.FS_Dic_F;
				skill.FS_Type_F = skilChangeData.FS_Type_F;
				skill.FS_Type_Dic_F = skilChangeData.FS_Type_Dic_F;
				skill.FS_DMG = skilChangeData.FS_DMG;
				skill.FS_CT_F = skilChangeData.FS_CT_F;
				skill.FS_CT_S = skilChangeData.FS_CT_S;
				skill.FS_CT_AT = skilChangeData.FS_CT_AT;
				skill.FS_CT_Multi = skilChangeData.FS_CT_Multi;
				skill.FS_Time1 = skilChangeData.FS_Time1;
				skill.FS_Time2 = skilChangeData.FS_Time2;
				skill.FS_Range1 = skilChangeData.FS_Range1;
				skill.FS_AngleA = skilChangeData.FS_AngleA;
			}
		}
	}

	private static void ApplySkillLink(SkillData_Sample_Father skill, string linkSkill, bool isEquip)
	{
		if (string.IsNullOrEmpty(linkSkill))
		{
			return;
		}
		if (isEquip)
		{
			if (skill.LinkSK == null)
			{
				skill.LinkSK = new string[1] { linkSkill };
			}
			else if (!skill.LinkSK.Contains(linkSkill))
			{
				List<string> list = new List<string>(skill.LinkSK);
				list.Add(linkSkill);
				skill.LinkSK = list.ToArray();
			}
		}
		else if (skill.LinkSK != null)
		{
			skill.LinkSK = skill.LinkSK.Where((string x) => x != linkSkill).ToArray();
		}
	}

	private static void ApplySkillJcSkill(SkillData_Sample_Father skill, string linkSkill, bool isEquip)
	{
		if (!string.IsNullOrEmpty(linkSkill))
		{
			if (isEquip)
			{
				skill.JCskill = linkSkill;
			}
			else if (skill.JCskill == linkSkill)
			{
				skill.JCskill = string.Empty;
			}
		}
	}

	private static void ApplySkillUseDamageElement(SkillData_Sample_Father skill, int el, int intNumber)
	{
		switch (el)
		{
		case 0:
			skill.UseDMG_EL0 += intNumber;
			break;
		case 1:
			skill.UseDMG_EL1 += intNumber;
			break;
		case 2:
			skill.UseDMG_EL2 += intNumber;
			break;
		case 3:
			skill.UseDMG_EL3 += intNumber;
			break;
		case 4:
			skill.UseDMG_EL4 += intNumber;
			break;
		case 5:
			skill.UseDMG_EL5 += intNumber;
			break;
		}
	}

	private static void ApplySkillUseChuanElement(SkillData_Sample_Father skill, int el, int intNumber)
	{
		switch (el)
		{
		case 0:
			skill.UseChuan0 += intNumber;
			break;
		case 1:
			skill.UseChuan1 += intNumber;
			break;
		case 2:
			skill.UseChuan2 += intNumber;
			break;
		case 3:
			skill.UseChuan3 += intNumber;
			break;
		case 4:
			skill.UseChuan4 += intNumber;
			break;
		case 5:
			skill.UseChuan5 += intNumber;
			break;
		}
	}

	public string GetSpecial(int index)
	{
		if (!TryGetSPCTemplate(index, out var spc, out var mb))
		{
			return string.Empty;
		}
		return GetSpecial(spc, mb);
	}

	public string GetSpecial(WPSPC spc, SPC_MB mb)
	{
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		string text = string.Empty;
		if (spc == null || mb == null)
		{
			return text;
		}
		int eL = spc.EL;
		float sPCPRC = GetSPCPRC(spc);
		float num = Mathf.Floor(mb.DamageLast * sPCPRC);
		float number = Mathf.Floor(mb.DamageLast * sPCPRC * instance.GiveDamage(SWS.DMtype(eL)) / 100f);
		string text2 = $"<color={DamageColor.Colors[SWS.DMtype(eL)]}>{num}% ({DamgeTextManager.FormatDamageNumber(number)}) {LOC.MM.GetMain(SWS.El_DMG(eL))}</color>";
		switch (mb.SPCtype)
		{
		case 1:
			text += string.Format("<color=#FFDBB6>{0}</color> <color=#FFB532>{1}</color> <color=#FFDBB6>{2}% {3}</color>  <color=#FFB532>{4}</color>\n{5}", LOC.MM.GetMain("Use skill"), LOC.MM.GetSkill(mb.SkillName), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 2:
			text += string.Format("<color=#FFB532>{0}</color> <color=#FFDBB6>{1} {2}% {3}</color>  <color=#FFB532>{4}</color>\n{5}", LOC.MM.GetSkill(mb.SkillName), LOC.MM.GetMain("Hit enemy"), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 3:
			text += string.Format("<color=#FFDBB6>{0} {1}% {2}</color> <color=#FFB532>{3}</color>\n{4}", LOC.MM.GetMain("Enemy Dead"), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 4:
			text += string.Format("<color=#FFDBB6>{0} {1}% {2}</color> <color=#FFB532>{3}</color>\n{4}", LOC.MM.GetMain("Be Attacked"), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 5:
			text += string.Format("<color=#FFDBB6>{0} {1}% {2}</color>  <color=#FFB532>{3}</color>\n{4}", LOC.MM.GetMain("GeDang"), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 10:
			if (mb.OBJ == 50)
			{
				text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>  " + text2;
				text += string.Format("\n{0}  {1}", LOC.MM.GetMain("Count"), mb.Count_ORB);
				text += string.Format("\n{0} {1} - {2} {3}", LOC.MM.GetMain("AttackInterval"), mb.FStime1, mb.FStime2, LOC.MM.GetMain("S"));
			}
			if (mb.OBJ == 51)
			{
				text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>  " + text2;
				text += string.Format("\n{0}  {1}", LOC.MM.GetMain("Count"), mb.Count_ORB);
				text += string.Format("\n{0} {1} {2}", LOC.MM.GetMain("AttackInterval"), mb.FStime1, LOC.MM.GetMain("S"));
			}
			if (mb.OBJ == 52)
			{
				text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>  " + text2;
				text += string.Format("\n{0}  {1}", LOC.MM.GetMain("Count"), mb.Count_F);
				text += string.Format("\n{0} {1} {2}", LOC.MM.GetMain("AttackInterval"), mb.FStime1, LOC.MM.GetMain("S"));
			}
			if (mb.OBJ == 53 || mb.OBJ == 54 || mb.OBJ == 55 || mb.OBJ == 56)
			{
				text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>  " + text2;
				text += string.Format("\n{0}  {1}", LOC.MM.GetMain("Count"), mb.Count_ORB);
			}
			if (mb.OBJ == 60)
			{
				text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>  " + text2;
				text = ((mb.TypeORB != 10) ? (text + string.Format("\n{0} {1} {2}", LOC.MM.GetMain("AttackInterval"), mb.FStime1, LOC.MM.GetMain("S"))) : (text + string.Format("\n{0} {1} {2}%", LOC.MM.GetMain("CritFire"), LOC.MM.GetMain("Rate"), mb.Count_F)));
			}
			if (mb.OBJ == 210)
			{
				text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>  " + text2;
				if (mb.ZQName != "0")
				{
					string destroyCoreProjectileName = GetDestroyCoreProjectileName(mb.ZD_F);
					text = text + "\n" + string.Format(LOC.MM.GetSPC("DestroyCore_FireOnHit"), "<color=#FFB532>" + LOC.MM.GetSkill(mb.ZQName) + "</color>", "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>", "<color=#FFB532>" + destroyCoreProjectileName + "</color>");
					if (mb.TypeORB == 0)
					{
						text = text + "\n<color=#53A3FF>" + LOC.MM.GetSPC("DestroyCore_AutoTarget") + "</color>";
					}
				}
				if (mb.TypeDIC_S > 0)
				{
					text = text + "\n" + string.Format(LOC.MM.GetSPC("DestroyCore_FlyThroughDamage"), "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>", "<color=#FFB532>" + FormatXJLNumber(mb.TypeDIC_S) + "</color>");
				}
				if (mb.TypeDIC_F > 0)
				{
					text = text + "\n" + string.Format(LOC.MM.GetSPC("DestroyCore_DestroyEnemyProjectile"), "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>", "<color=#FFB532>" + FormatXJLNumber(mb.TypeDIC_F) + "</color>");
				}
			}
			if (mb.OBJ == 221)
			{
				text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>  " + text2;
				text += string.Format("\n{0}  {1}", LOC.MM.GetMain("Count"), mb.Count_ORB);
				text += string.Format("\n{0} {1} - {2} {3}", LOC.MM.GetMain("AttackInterval"), mb.FStime1, mb.FStime2, LOC.MM.GetMain("S"));
			}
			if (mb.OBJ == 230)
			{
				switch (mb.TypeORB)
				{
				case 0:
					text += string.Format("<color=#FFB532>{0}</color>  {1} + {2}%  {3}", LOC.MM.GetSPC(mb.SPCname), LOC.MM.GetMain("CompanionsInRange"), Mathf.Floor(mb.ORB), LOC.MM.GetMain("damage"));
					break;
				case 1:
					text += string.Format("<color=#FFB532>{0}</color>  {1} + {2}%  {3}", LOC.MM.GetSPC(mb.SPCname), LOC.MM.GetMain("CompanionsInRange"), Mathf.Floor(mb.ORB), LOC.MM.GetMain("AttackSpeed"));
					break;
				case 2:
					text += string.Format("<color=#FFB532>{0}</color>  {1} + {2}%  {3}", LOC.MM.GetSPC(mb.SPCname), LOC.MM.GetMain("CompanionsInRange"), Mathf.Floor(mb.ORB), LOC.MM.GetMain("MoveSpeed"));
					break;
				case 3:
					text += string.Format("<color=#FFB532>{0}</color>  {1} + {2}%  {3}", LOC.MM.GetSPC(mb.SPCname), LOC.MM.GetMain("CompanionsInRange"), Mathf.Floor(mb.ORB), LOC.MM.GetMain("HealthPercentRecovery"));
					break;
				}
			}
			if (mb.OBJ == 240)
			{
				text += string.Format("{0}  <color=#FFB532>{1}</color>  {2}\n{3} {4} {5}", LOC.MM.GetMain("PeriodicGenerateNearby"), LOC.MM.GetSPC(mb.SPCname), text2, LOC.MM.GetMain("SpawnInterval"), mb.FStime1, LOC.MM.GetMain("S"));
			}
			break;
		case 11:
			text = text + "<color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "\n" + LOC.MM.GetMain("Automatically picks up items around the player") + "</color>";
			switch (mb.OBJ)
			{
			case 0:
				text = text + "\n" + string.Format(LOC.MM.GetSPC("XJL_DESC_0"), FormatXJLNumber(mb.Damage));
				break;
			case 1:
				text = text + "\n" + string.Format(LOC.MM.GetSPC("XJL_DESC_1"), FormatXJLNumber(mb.Damage));
				break;
			case 2:
				text = text + "\n" + string.Format(LOC.MM.GetSPC("XJL_DESC_2"), FormatXJLNumber(mb.Damage));
				break;
			case 3:
				text = text + "\n" + string.Format(LOC.MM.GetSPC("XJL_DESC_3"), FormatXJLNumber(mb.Damage), FormatXJLNumber(mb.Damage * 2f), FormatXJLNumber(mb.FStime2), LOC.MM.GetMain("S"));
				break;
			case 4:
				text = text + "\n" + string.Format(LOC.MM.GetSPC("XJL_DESC_4"), FormatXJLNumber(mb.Damage));
				break;
			case 5:
				text = text + "\n" + string.Format(LOC.MM.GetSPC("XJL_DESC_5"), FormatXJLNumber(mb.Damage), FormatXJLNumber(3f), FormatXJLNumber(mb.FStime2), LOC.MM.GetMain("S"));
				break;
			case 6:
				text = text + "\n" + string.Format(LOC.MM.GetSPC("XJL_DESC_6"), FormatXJLNumber(mb.Damage));
				break;
			}
			break;
		case 20:
			text += string.Format("<color=#FFB532>{0}</color> <color=#FFDBB6>{1} {2}% {3}</color> <color=#FFB532>{4}</color>\n{5}", LOC.MM.GetSkill(mb.SkillName), LOC.MM.GetMain("Attack"), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 21:
			text += string.Format("<color=#FFB532>{0}</color> <color=#FFDBB6>{1} {2}% {3}</color> <color=#FFB532>{4}</color>\n{5}", LOC.MM.GetSkill(mb.SkillName), LOC.MM.GetMain("Be Attacked"), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 22:
			text += string.Format("<color=#FFB532>{0}</color> <color=#FFDBB6>{1} {2}% {3}</color> <color=#FFB532>{4}</color>\n{5}", LOC.MM.GetSkill(mb.SkillName), LOC.MM.GetMain("Death"), mb.RateLast, LOC.MM.GetMain("Rate Generation"), LOC.MM.GetSPC(mb.SPCname), text2);
			break;
		case 23:
			text = text + "<color=#FFB532>" + LOC.MM.GetSkill(mb.SkillName) + "</color> <color=#FFDBB6>" + LOC.MM.GetMain("Attached") + "</color> <color=#FFB532>" + LOC.MM.GetSPC(mb.SPCname) + "</color>\n" + text2;
			if (mb.OBJ == 410)
			{
				text += string.Format("\n{0}  {1}", LOC.MM.GetMain("Count"), mb.Count_F);
				text += string.Format("\n{0} {1} {2}", LOC.MM.GetMain("AttackInterval"), mb.FStime1, LOC.MM.GetMain("S"));
			}
			if (mb.OBJ == 415)
			{
				text += string.Format("\n{0} {1} {2}", LOC.MM.GetMain("AttackInterval"), mb.FStime1, LOC.MM.GetMain("S"));
			}
			break;
		case 30:
			text += string.Format("<color=#FFB532>{0}</color> <color=#FFDBB6>{1}</color> <color=#FFDBB6>{2}% {3}</color>  <color=#FFB532>{4}</color>", LOC.MM.GetSkill(mb.SkillName), LOC.MM.GetMain("Attack"), mb.RateLast, LOC.MM.GetMain("ChanceFirePlayerSkill"), LOC.MM.GetSkill(mb.ZQName));
			break;
		case 31:
			text += string.Format("<color=#FFDBB6>{0}</color> <color=#FFB532>{1}</color>\n<color=#FFB532>{2}</color> <color=#FFDBB6>{3}% {4}</color>", LOC.MM.GetMain("Use skill"), LOC.MM.GetSkill(mb.ZQName), LOC.MM.GetSkill(mb.SkillName), mb.RateLast, LOC.MM.GetMain("ChanceFireSameSkill"));
			break;
		case 32:
			text += string.Format("<color=#FFDBB6>{0}</color> <color=#FFB532>{1}</color>\n<color=#FFB532>{2}</color> <color=#FFDBB6>{3}% {4}</color>", LOC.MM.GetMain("Use skill"), LOC.MM.GetSkill(mb.ZQName), LOC.MM.GetSkill(mb.SkillName), mb.RateLast, LOC.MM.GetMain("FireOwnSkill"));
			break;
		}
		if (HHCount > 0)
		{
			text += string.Format("\n{0} +{1}", LOC.MM.GetMain("HasHH"), HHCount);
		}
		return text;
	}

	public string GetFW_Base()
	{
		if (FW_Base == null || string.IsNullOrEmpty(FW_Base.FWname) || string.IsNullOrEmpty(FW_Base.type))
		{
			return string.Empty;
		}
		string text = LOC.MM.GetItem(FW_Base.FWname) + "\n";
		switch (FW_Base.type)
		{
		case "DMG":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("damage"), FW_Base.number);
			break;
		case "ATS":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("AttackSpeed"), FW_Base.number);
			break;
		case "BJD":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("BJDamage"), FW_Base.number);
			break;
		case "ALLC":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("AllChuan"), FW_Base.number);
			break;
		case "DOT":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("Character_DotDamage"), FW_Base.number);
			break;
		case "C_DMG":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp damage"), FW_Base.number);
			break;
		case "C_ATS":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp AttackSpeed"), FW_Base.number);
			break;
		case "Heal":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("HealthMax"), FW_Base.number);
			break;
		case "Mana":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("ManaMax"), FW_Base.number);
			break;
		case "Anti":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("AllAnti"), FW_Base.number);
			break;
		case "MVS":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("MoveSpeed"), FW_Base.number);
			break;
		case "C_Heal":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp HealthMax"), FW_Base.number);
			break;
		case "C_Anti":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp AllAnti"), FW_Base.number);
			break;
		case "ORB_DMG":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("SP Damage"), FW_Base.number);
			break;
		case "XJ_DMG":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("Character_TrapDamage"), FW_Base.number);
			break;
		case "Drop":
			text += string.Format("{0} + {1}%", LOC.MM.GetMain("DropRate"), FW_Base.number);
			break;
		}
		return text;
	}

	private static string FormatXJLNumber(float value)
	{
		return value.ToString("0.##", CultureInfo.InvariantCulture);
	}

	private static string GetDestroyCoreProjectileName(int projectileId)
	{
		switch (projectileId)
		{
		case 325:
			return LOC.MM.GetSPC("Hit_ZD_JG");
		case 380:
			return LOC.MM.GetSPC("DestroyCore_Laser");
		case 383:
			return LOC.MM.GetSPC("DestroyCore_BouncingLaserProjectile");
		case 501:
			return LOC.MM.GetSPC("MagicFlameBall");
		case 271:
			return LOC.MM.GetSPC("SK_Zhui");
		case 505:
			return LOC.MM.GetSPC("SoulCrystal");
		case 110:
			return LOC.MM.GetSPC("SK_TDC");
		case 71:
			return LOC.MM.GetSPC("ORB_Long_A");
		case 72:
			return LOC.MM.GetSPC("ORB_Long_B");
		case 290:
			return LOC.MM.GetSPC("SK_FlyChui");
		case 291:
			return LOC.MM.GetSPC("SK_FlyChui");
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
			return LOC.MM.GetSPC("ORB_Ball_A");
		default:
			return string.Empty;
		}
	}

	public string GetBaoshi(int a)
	{
		string text = string.Empty;
		int bsAdd = (SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.BS_Add : 0);
		float bsMulti = (SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.BS_Multi : 0f);
		int socketedGemNumber = GetSocketedGemNumber(Aocao[a], bsAdd, bsMulti);
		switch (Aocao[a].Type)
		{
		case 0:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("HealthMax"));
			break;
		case 1:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("fire Anti"));
			break;
		case 2:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("fire chuan"));
			break;
		case 3:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("fire damage"));
			break;
		case 4:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("DropRate"));
			break;
		case 5:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("thunder Anti"));
			break;
		case 6:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("thunder chuan"));
			break;
		case 7:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("thunder damage"));
			break;
		case 8:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("Comp HealthMax"));
			break;
		case 9:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("poison Anti"));
			break;
		case 10:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("poison chuan"));
			break;
		case 11:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("Comp AttackSpeed"));
			break;
		case 12:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("poison damage"));
			break;
		case 13:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("ManaMax"));
			break;
		case 14:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("frozen Anti"));
			break;
		case 15:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("frozen chuan"));
			break;
		case 16:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("frozen damage"));
			break;
		case 17:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("Comp damage"));
			break;
		case 18:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("shadow Anti"));
			break;
		case 19:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("shadow chuan"));
			break;
		case 20:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("MoveSpeed"));
			break;
		case 21:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("shadow damage"));
			break;
		case 22:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 23:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("physics Anti"));
			break;
		case 24:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("physics chuan"));
			break;
		case 25:
			text += string.Format("{0} +{1}% {2}", LOC.MM.GetItem(Aocao[a].Name), socketedGemNumber, LOC.MM.GetMain("physics damage"));
			break;
		}
		return text;
	}

	public float GetNameSize()
	{
		return Encoding.Default.GetByteCount(LOC.MM.GetItem(ItemName));
	}
}
