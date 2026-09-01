using System;
using System.Globalization;
using System.Text;
using Entity.InteractableObjects.Item;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class BaoshiClass : ItemClass, IDropItemData
{
	public string BStype;

	public int UseType;

	public int BS_Quality;

	public int Number;

	public int MstackSize;

	public int CstackSize;

	public int DropSpriteSize;

	public string SKname;

	public int FWtype;

	public int Index;

	public int EL;

	public float PRC;

	public int priceQulity;

	public int Xi;

	public int DropScene;

	public int NumberLast => Mathf.FloorToInt((float)(Number + SingletonMonoScope<PlayerManager>.Instance.BS_Add) + (float)(Number + SingletonMonoScope<PlayerManager>.Instance.BS_Add) * SingletonMonoScope<PlayerManager>.Instance.BS_Multi / 100f);

	public int MaxPrice => Price * CstackSize;

	public int ByPrice => MaxPrice * 8;

	int IDropItemData.ItemType => ItemType;

	public override void Reset()
	{
		base.Reset();
		BStype = string.Empty;
		UseType = 0;
		BS_Quality = 0;
		Number = 0;
		MstackSize = 0;
		CstackSize = 0;
		DropSpriteSize = 0;
		SKname = string.Empty;
		FWtype = 0;
		Index = 0;
		EL = 0;
		PRC = 0f;
		priceQulity = 0;
		Xi = 0;
	}

	public string GetMain()
	{
		LOC mM = LOC.MM;
		string text = string.Empty;
		if (PoeItemMod.TryGetDescription(ItemName, out var text2))
		{
			text = text + "<color=#00E5FF>" + text2 + "</color>\n";
		}
		switch (UseType)
		{
		case 0:
			text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed") + ":</color>\n";
			switch (BStype)
			{
			case "red":
				text += string.Format("{0} +{1}% {2}\n{3} +{4}% {5}\n{6} +{7}% {8}\n{9} +{10}% {11}\n{12} +{13}% {14}", mM.GetMain("Weapon"), NumberLast, mM.GetMain("fire damage"), mM.GetMain("head"), NumberLast, mM.GetMain("HealthMax"), mM.GetMain("body"), NumberLast, mM.GetMain("fire Anti"), mM.GetMain("hand"), NumberLast, mM.GetMain("fire chuan"), mM.GetMain("leg"), NumberLast, mM.GetMain("HealthMax"));
				break;
			case "yellow":
				text += string.Format("{0} +{1}% {2}\n{3} +{4}% {5}\n{6} +{7}% {8}\n{9} +{10}% {11}\n{12} +{13}% {14}", mM.GetMain("Weapon"), NumberLast, mM.GetMain("thunder damage"), mM.GetMain("head"), NumberLast, mM.GetMain("DropRate"), mM.GetMain("body"), NumberLast, mM.GetMain("thunder Anti"), mM.GetMain("hand"), NumberLast, mM.GetMain("thunder chuan"), mM.GetMain("leg"), NumberLast, mM.GetMain("DropRate"));
				break;
			case "green":
				text += string.Format("{0} +{1}% {2}\n{3} +{4}% {5}\n{6} +{7}% {8}\n{9} +{10}% {11}\n{12} +{13}% {14}", mM.GetMain("Weapon"), NumberLast, mM.GetMain("poison damage"), mM.GetMain("head"), NumberLast, mM.GetMain("Comp HealthMax"), mM.GetMain("body"), NumberLast, mM.GetMain("poison Anti"), mM.GetMain("hand"), NumberLast, mM.GetMain("poison chuan"), mM.GetMain("leg"), NumberLast, mM.GetMain("Comp AttackSpeed"));
				break;
			case "blue":
				text += string.Format("{0} +{1}% {2}\n{3} +{4}% {5}\n{6} +{7}% {8}\n{9} +{10}% {11}\n{12} +{13}% {14}", mM.GetMain("Weapon"), NumberLast, mM.GetMain("frozen damage"), mM.GetMain("head"), NumberLast, mM.GetMain("ManaMax"), mM.GetMain("body"), NumberLast, mM.GetMain("frozen Anti"), mM.GetMain("hand"), NumberLast, mM.GetMain("frozen chuan"), mM.GetMain("leg"), NumberLast, mM.GetMain("ManaMax"));
				break;
			case "purple":
				text += string.Format("{0} +{1}% {2}\n{3} +{4}% {5}\n{6} +{7}% {8}\n{9} +{10}% {11}\n{12} +{13}% {14}", mM.GetMain("Weapon"), NumberLast, mM.GetMain("shadow damage"), mM.GetMain("head"), NumberLast, mM.GetMain("Comp damage"), mM.GetMain("body"), NumberLast, mM.GetMain("shadow Anti"), mM.GetMain("hand"), NumberLast, mM.GetMain("shadow chuan"), mM.GetMain("leg"), NumberLast, mM.GetMain("MoveSpeed"));
				break;
			case "white":
				text += string.Format("{0} +{1}% {2}\n{3} +{4}% {5}\n{6} +{7}% {8}\n{9} +{10}% {11}\n{12} +{13}% {14}", mM.GetMain("Weapon"), NumberLast, mM.GetMain("physics damage"), mM.GetMain("head"), NumberLast, mM.GetMain("AttackSpeed"), mM.GetMain("body"), NumberLast, mM.GetMain("physics Anti"), mM.GetMain("hand"), NumberLast, mM.GetMain("physics chuan"), mM.GetMain("leg"), NumberLast, mM.GetMain("AttackSpeed"));
				break;
			case "projectile":
				text = text + "+ " + string.Format(LOC.MM.GetItem("PoeJewelVolley_Info"), Number);
				break;
			}
			break;
		case 1:
			switch (BStype)
			{
			case "JH_damage":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be fused into weapon") + ":</color>\n";
				text += string.Format("{0} +{1}%", mM.GetMain("damage"), Number);
				break;
			case "JH_ats":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be fused into weapon") + ":</color>\n";
				text += string.Format("{0} +{1}%", mM.GetMain("AttackSpeed"), Number);
				break;
			case "JH_CPdamage":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be fused into weapon") + ":</color>\n";
				text += string.Format("{0} +{1}%", mM.GetMain("Comp damage"), Number);
				break;
			case "JH_heal":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be fused into armor") + ":</color>\n";
				text += string.Format("{0} +{1}%", mM.GetMain("HealthMax"), Number);
				break;
			case "JH_mana":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be fused into armor") + ":</color>\n";
				text += string.Format("{0} +{1}%", mM.GetMain("ManaMax"), Number);
				break;
			case "JH_CPheal":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be fused into armor") + ":</color>\n";
				text += string.Format("{0} +{1}%", mM.GetMain("Comp HealthMax"), Number);
				break;
			case "JHEL0":
				text += GetJHELEssenceStats("fire damage", "fire chuan", "fire Anti");
				break;
			case "JHEL1":
				text += GetJHELEssenceStats("frozen damage", "frozen chuan", "frozen Anti");
				break;
			case "JHEL2":
				text += GetJHELEssenceStats("thunder damage", "thunder chuan", "thunder Anti");
				break;
			case "JHEL3":
				text += GetJHELEssenceStats("poison damage", "poison chuan", "poison Anti");
				break;
			case "JHEL4":
				text += GetJHELEssenceStats("physics damage", "physics chuan", "physics Anti");
				break;
			case "JHEL5":
				text += GetJHELEssenceStats("shadow damage", "shadow chuan", "shadow Anti");
				break;
			}
			break;
		case 2:
			switch (BStype)
			{
			case "Stone_KZ":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be used on equipment") + "</color>\n";
				text += FormatMainLocalization("Stone_KZ", FormatStoneNumber(Number));
				break;
			case "Stone_FS":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be used on equipment") + "</color>\n";
				text += FormatMainLocalization("Stone_FS");
				break;
			case "Stone_HH":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be used on equipment") + "</color>\n";
				text += FormatMainLocalization("Stone_HH", FormatStoneNumber(Number));
				break;
			case "Stone_AM":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be used on equipment") + "</color>\n";
				text += FormatMainLocalization("Stone_AM", FormatStoneNumber(Number));
				break;
			case "Stone_HM":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Each weapon can be socketed once") + "</color>\n";
				text += FormatMainLocalization("Stone_HM");
				break;
			case "Stone_CG":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Each armor can be socketed once") + "</color>\n";
				text += FormatMainLocalization("Stone_CG");
				break;
			case "Stone_LC":
				text = text + "<color=#E5CCAB>" + mM.GetMain("Each accessory can be socketed once") + "</color>\n";
				text += FormatMainLocalization("Stone_LC");
				break;
			case "Stone_FM":
				text += FormatMainLocalization("Stone_FM");
				break;
			case "Stone_HD":
				text += FormatMainLocalization("Stone_HD");
				break;
			case "Stone_XL":
				text += FormatMainLocalization("Stone_XL");
				break;
			case "Stone_CL":
				text += FormatMainLocalization("Stone_CL");
				break;
			}
			break;
		case 3:
			text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed") + ":</color>\n";
			text = text + "<color=" + DamageColor.Colors[SWS.DMtype(EL)] + ">" + mM.GetSkill(SKname) + " + 1</color>";
			break;
		case 4:
			switch (FWtype)
			{
			case 0:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into weapon") + ":</color>\n";
				break;
			case 1:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into helmet and armor") + ":</color>\n";
				break;
			case 2:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into gloves and boots") + ":</color>\n";
				break;
			case 3:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into amulet and ring") + ":</color>\n";
				break;
			case 4:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into orb and jewelry") + ":</color>\n";
				break;
			}
			text += GetSPCFWRuneSpecial();
			break;
		case 5:
			switch (FWtype)
			{
			case 0:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into weapon") + ":</color>\n";
				break;
			case 1:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into armor") + ":</color>\n";
				break;
			case 2:
				text = text + "<color=#E5CCAB>" + mM.GetMain("Can be socketed into accessory") + ":</color>\n";
				break;
			}
			switch (BStype)
			{
			case "DMG":
				text += string.Format("{0} +{1}%", mM.GetMain("damage"), Number);
				break;
			case "ATS":
				text += string.Format("{0} +{1}%", mM.GetMain("AttackSpeed"), Number);
				break;
			case "BJD":
				text += string.Format("{0} +{1}%", mM.GetMain("BJDamage"), Number);
				break;
			case "ALLC":
				text += string.Format("{0} +{1}%", mM.GetMain("AllChuan"), Number);
				break;
			case "DOT":
				text += string.Format("{0} +{1}%", mM.GetMain("Character_DotDamage"), Number);
				break;
			case "C_DMG":
				text += string.Format("{0} +{1}%", mM.GetMain("Comp damage"), Number);
				break;
			case "C_ATS":
				text += string.Format("{0} +{1}%", mM.GetMain("Comp AttackSpeed"), Number);
				break;
			case "Heal":
				text += string.Format("{0} +{1}%", mM.GetMain("HealthMax"), Number);
				break;
			case "Mana":
				text += string.Format("{0} +{1}%", mM.GetMain("ManaMax"), Number);
				break;
			case "Anti":
				text += string.Format("{0} +{1}%", mM.GetMain("AllAnti"), Number);
				break;
			case "MVS":
				text += string.Format("{0} +{1}%", mM.GetMain("MoveSpeed"), Number);
				break;
			case "C_Heal":
				text += string.Format("{0} +{1}%", mM.GetMain("Comp HealthMax"), Number);
				break;
			case "C_Anti":
				text += string.Format("{0} +{1}%", mM.GetMain("Comp AllAnti"), Number);
				break;
			case "ORB_DMG":
				text += string.Format("{0} +{1}%", mM.GetMain("SP Damage"), Number);
				break;
			case "XJ_DMG":
				text += string.Format("{0} +{1}%", mM.GetMain("Character_TrapDamage"), Number);
				break;
			case "Drop":
				text += string.Format("{0} +{1}%", mM.GetMain("DropRate"), Number);
				break;
			}
			break;
		}
		return text;
	}

	private static string GetJHELEssenceStats(string damageKey, string penetrationKey, string resistanceKey)
	{
		LOC mM = LOC.MM;
		return "<color=#E5CCAB>" + mM.GetMain("Can be fused into equipment") + ":</color>\n" + mM.GetMain("Main hand weapon") + " +4% " + mM.GetMain(damageKey) + "\n" + mM.GetMain("Off hand weapon") + " +1% " + mM.GetMain(penetrationKey) + "\n" + mM.GetMain("Armor") + " +1% " + mM.GetMain(resistanceKey) + "\n" + mM.GetMain("Amulet") + " +1% " + mM.GetMain(resistanceKey) + "\n" + mM.GetMain("Ring") + " +1% " + mM.GetMain(penetrationKey) + "\n" + mM.GetMain("Orb") + " +3% " + mM.GetMain(damageKey) + "\n" + mM.GetMain("Jewelry") + " +3% " + mM.GetMain(damageKey);
	}

	private static string FormatMainLocalization(string key, params object[] args)
	{
		string main = LOC.MM.GetMain(key);
		if (args == null || args.Length == 0)
		{
			return main;
		}
		try
		{
			return string.Format(CultureInfo.InvariantCulture, main, args);
		}
		catch (FormatException)
		{
			return main;
		}
	}

	private static string FormatStoneNumber(float value)
	{
		if (!Mathf.Approximately(value, Mathf.Round(value)))
		{
			return value.ToString("0.##", CultureInfo.InvariantCulture);
		}
		return Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture);
	}

	private string GetSPCFWRuneSpecial()
	{
		if (Index <= 0 || !SingletonMonoScope<ItemManager>.HasInstance || !SingletonMonoScope<ItemManager>.Instance.TryGetSPCMBByIndex(Index, out var mb))
		{
			return string.Empty;
		}
		WPSPC spc = new WPSPC
		{
			Index = Index,
			EL = EL,
			PRC = ((PRC > 0f) ? PRC : 1f)
		};
		return new WeaponClass().GetSpecial(spc, mb);
	}

	public float GetNameSize()
	{
		return Encoding.Default.GetByteCount(LOC.MM.GetItem(ItemName));
	}
}
