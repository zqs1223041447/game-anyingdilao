using System.Collections.Generic;
using System.Linq;
using Entity.InteractableObjects.Item;
using UnityEngine;

namespace Level.StateData.LevelStates;

public sealed class WeaponState : ItemLevelState
{
	public int RebuildTime;

	public int EnhanceTime;

	public int HHTime;

	public int SkillFWTime;

	public int JHEL_Count;

	public int JH_Count;

	public int PLtype;

	public string WeaponType;

	public int CharType;

	public int Set_Index;

	public Set_DT SetRuntimeData;

	public int BS_Set_Index;

	public int DropScene;

	public int MJ_Level;

	public int SkillFW_CountMax;

	public float SPC_DMG_Bei;

	public bool BaseValueDoubled;

	public float BaseValueMultiplier;

	public WPDT_A[] Main;

	public WPDT_A[] DOT;

	public WPDT_B[] SK;

	public WPDT_B[] CP;

	public WPFW_Base FW_Base;

	public float Damage;

	public float Health;

	public float Mana;

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

	public List<WPSPC> SPC = new List<WPSPC>();

	public int SPCindex;

	public int SPC_EL;

	public float SPC_PRC;

	public int ItemType;

	public int GlobalID;

	public string ItemName;

	public int Price;

	public int Quality;

	public IntVector2 Size;

	public IntVector2 SaveSlot;

	public Sprite Icon;

	public int Level;

	public int SoundDrop;

	public int SoundUse;

	public int RotateType;

	public static WeaponState FromRuntime(WeaponClass w)
	{
		w.NormalizeSkillFWCountMax();
		w.NormalizeSPCDamageBei();
		w.NormalizeBaseValueMultiplier();
		WPSPC sPCData = w.GetSPCData(0);
		return new WeaponState
		{
			DropItemType = DropItemType.Weapon,
			RebuildTime = w.Reb_CountMax,
			EnhanceTime = w.ZQ_CountMax,
			HHTime = w.HHCount,
			SkillFWTime = w.SKCount,
			JHEL_Count = w.JHEL_Count,
			JH_Count = w.JH_Count,
			ItemType = w.ItemType,
			GlobalID = w.GlobalID,
			ItemName = w.ItemName,
			Price = w.Price,
			Quality = w.Quality,
			Size = w.Size,
			SaveSlot = w.SaveSlot,
			Icon = w.Icon,
			Level = w.Level,
			SoundDrop = w.SoundDrop,
			SoundUse = w.SoundUse,
			RotateType = w.RotateType,
			PLtype = w.PLtype,
			WeaponType = w.WeaponType,
			CharType = w.CharType,
			Set_Index = w.Set_Index,
			SetRuntimeData = SetDataUtil.Clone(w.SetRuntimeData),
			BS_Set_Index = w.BS_Set_Index,
			DropScene = w.DropScene,
			MJ_Level = w.MJ_Level,
			SkillFW_CountMax = w.SkillFW_CountMax,
			SPC_DMG_Bei = w.SPC_DMG_Bei,
			BaseValueDoubled = w.BaseValueDoubled,
			BaseValueMultiplier = w.BaseValueMultiplier,
			Main = CopyWeaponDataA(w.Main),
			DOT = CopyWeaponDataA(w.DOT),
			SK = CopyWeaponDataB(w.SK),
			CP = CopyWeaponDataB(w.CP),
			FW_Base = CopyWeaponFwBase(w.FW_Base),
			Damage = w.Damage,
			Health = w.Health,
			Mana = w.Mana,
			Fire = w.Fire,
			Frozen = w.Frozen,
			Thunder = w.Thunder,
			Poison = w.Poison,
			Physics = w.Physics,
			Shadow = w.Shadow,
			WP_SkillCount = w.WP_SkillCount,
			WPSK = ((w.WPSK != null) ? w.WPSK.ToList() : new List<WPSkill>()),
			MaxAocaoCount = w.MaxAocaoCount,
			AocaoCount = w.AocaoCount,
			Aocao = ((w.Aocao != null) ? w.Aocao.ToList() : new List<WPAocao>()),
			SPC = CopySPCList(w.SPC),
			SPCindex = sPCData.Index,
			SPC_EL = sPCData.EL,
			SPC_PRC = sPCData.PRC
		};
	}

	public void ApplyToRuntime(WeaponClass w)
	{
		w.Reb_CountMax = RebuildTime;
		w.ZQ_CountMax = EnhanceTime;
		w.HHCount = HHTime;
		w.SKCount = SkillFWTime;
		w.JHEL_Count = JHEL_Count;
		w.JH_Count = JH_Count;
		w.ItemType = ItemType;
		w.GlobalID = GlobalID;
		w.ItemName = ItemName;
		w.Price = Price;
		w.Quality = Quality;
		w.Size = Size;
		w.SaveSlot = SaveSlot;
		w.Icon = Icon;
		w.Level = Level;
		w.SoundDrop = SoundDrop;
		w.SoundUse = SoundUse;
		w.RotateType = RotateType;
		w.PLtype = PLtype;
		w.WeaponType = WeaponType;
		w.CharType = CharType;
		w.Set_Index = Set_Index;
		w.SetRuntimeData = SetDataUtil.Clone(SetRuntimeData);
		w.BS_Set_Index = BS_Set_Index;
		w.DropScene = DropScene;
		w.MJ_Level = MJ_Level;
		w.SkillFW_CountMax = SkillFW_CountMax;
		w.NormalizeSkillFWCountMax();
		w.SPC_DMG_Bei = SPC_DMG_Bei;
		w.NormalizeSPCDamageBei();
		w.BaseValueDoubled = BaseValueDoubled;
		w.BaseValueMultiplier = BaseValueMultiplier;
		w.NormalizeBaseValueMultiplier();
		w.Main = CopyWeaponDataA(Main);
		w.DOT = CopyWeaponDataA(DOT);
		w.SK = CopyWeaponDataB(SK);
		w.CP = CopyWeaponDataB(CP);
		w.FW_Base = CopyWeaponFwBase(FW_Base);
		w.Damage = Damage;
		w.Health = Health;
		w.Mana = Mana;
		w.Fire = Fire;
		w.Frozen = Frozen;
		w.Thunder = Thunder;
		w.Poison = Poison;
		w.Physics = Physics;
		w.Shadow = Shadow;
		w.WP_SkillCount = WP_SkillCount;
		w.WPSK = ((WPSK != null) ? WPSK.ToList() : new List<WPSkill>());
		w.MaxAocaoCount = MaxAocaoCount;
		w.AocaoCount = AocaoCount;
		w.Aocao = ((Aocao != null) ? Aocao.ToList() : new List<WPAocao>());
		w.SPC = CopySPCList(SPC);
		if (w.SPC == null || w.SPC.Count == 0)
		{
			w.SetSPCData(0, SPCindex, SPC_EL, SPC_PRC);
		}
	}

	private static WPDT_A[] CopyWeaponDataA(WPDT_A[] source)
	{
		if (source == null)
		{
			return null;
		}
		WPDT_A[] array = new WPDT_A[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			WPDT_A wPDT_A = source[i];
			array[i] = ((wPDT_A == null) ? null : new WPDT_A
			{
				Index = wPDT_A.Index,
				EL = wPDT_A.EL,
				number = wPDT_A.number
			});
		}
		return array;
	}

	private static WPDT_B[] CopyWeaponDataB(WPDT_B[] source)
	{
		if (source == null)
		{
			return null;
		}
		WPDT_B[] array = new WPDT_B[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			WPDT_B wPDT_B = source[i];
			array[i] = ((wPDT_B == null) ? null : new WPDT_B
			{
				SkillName = wPDT_B.SkillName,
				Index = wPDT_B.Index,
				GlobleID = wPDT_B.GlobleID,
				EL = wPDT_B.EL,
				number = wPDT_B.number,
				LinkSK = wPDT_B.LinkSK
			});
		}
		return array;
	}

	private static WPFW_Base CopyWeaponFwBase(WPFW_Base source)
	{
		if (source == null)
		{
			return null;
		}
		return new WPFW_Base
		{
			FWname = source.FWname,
			type = source.type,
			number = source.number,
			price = source.price
		};
	}

	private static List<WPSPC> CopySPCList(List<WPSPC> source)
	{
		List<WPSPC> list = new List<WPSPC>();
		if (source == null)
		{
			return list;
		}
		foreach (WPSPC item in source)
		{
			list.Add((item == null) ? null : new WPSPC
			{
				Index = item.Index,
				EL = item.EL,
				PRC = item.PRC,
				price = item.price
			});
		}
		return list;
	}
}
