using System.Collections.Generic;
using UnityEngine;

namespace Entity.InteractableObjects.Item;

public static class ItemCloneUtil
{
	public static WeaponClass CloneWeapon(WeaponClass source)
	{
		if (source == null)
		{
			return null;
		}
		WeaponClass weaponClass = new WeaponClass();
		CopyWeaponTo(weaponClass, source);
		return weaponClass;
	}

	public static void CopyWeaponTo(WeaponClass target, WeaponClass source)
	{
		if (target == null || source == null)
		{
			return;
		}
		target.Reb_CountMax = source.Reb_CountMax;
		target.ZQ_CountMax = source.ZQ_CountMax;
		target.Craft_LockPrefix = source.Craft_LockPrefix;
		target.Craft_LockSuffix = source.Craft_LockSuffix;
		target.Craft_NoAttack = source.Craft_NoAttack;
		target.Craft_NoCaster = source.Craft_NoCaster;
		target.HHCount = source.HHCount;
		target.SKCount = source.SKCount;
		target.JHEL_Count = source.JHEL_Count;
		target.JH_Count = source.JH_Count;
		target.ItemName = source.ItemName;
		target.GlobalID = source.GlobalID;
		target.ItemType = source.ItemType;
		target.Quality = source.Quality;
		target.Size = source.Size;
		target.Icon = source.Icon;
		target.Level = source.Level;
		target.SoundDrop = source.SoundDrop;
		target.SoundUse = source.SoundUse;
		target.RotateType = source.RotateType;
		target.PLtype = source.PLtype;
		target.WeaponType = source.WeaponType;
		target.CharType = source.CharType;
		target.SkillFW_CountMax = source.SkillFW_CountMax;
		target.NormalizeSkillFWCountMax();
		target.SPC_DMG_Bei = source.SPC_DMG_Bei;
		target.NormalizeSPCDamageBei();
		target.BaseValueDoubled = source.BaseValueDoubled;
		target.BaseValueMultiplier = source.BaseValueMultiplier;
		target.NormalizeBaseValueMultiplier();
		target.Damage = source.Damage;
		target.Health = source.Health;
		target.Mana = source.Mana;
		target.Fire = source.Fire;
		target.Frozen = source.Frozen;
		target.Thunder = source.Thunder;
		target.Poison = source.Poison;
		target.Physics = source.Physics;
		target.Shadow = source.Shadow;
		target.Main = CopyWeaponDataA(source.Main);
		target.DOT = CopyWeaponDataA(source.DOT);
		target.SK = CopyWeaponDataB(source.SK);
		target.CP = CopyWeaponDataB(source.CP);
		target.FW_Base = CopyWeaponFwBase(source.FW_Base);
		target.Set_Index = source.Set_Index;
		target.SetRuntimeData = SetDataUtil.Clone(source.SetRuntimeData);
		target.BS_Set_Index = source.BS_Set_Index;
		target.DropScene = source.DropScene;
		target.MJ_Level = source.MJ_Level;
		target.WP_SkillCount = source.WP_SkillCount;
		target.MaxAocaoCount = source.MaxAocaoCount;
		target.AocaoCount = source.AocaoCount;
		if (target.WPSK != null && source.WPSK != null)
		{
			int num = Mathf.Min(target.WPSK.Count, source.WPSK.Count);
			for (int i = 0; i < num; i++)
			{
				if (target.WPSK[i] != null && source.WPSK[i] != null)
				{
					target.WPSK[i].IndexName = source.WPSK[i].IndexName;
					target.WPSK[i].Number = source.WPSK[i].Number;
					target.WPSK[i].Number2 = source.WPSK[i].Number2;
					target.WPSK[i].price = source.WPSK[i].price;
				}
			}
		}
		if (target.Aocao != null && source.Aocao != null)
		{
			int num2 = Mathf.Min(target.Aocao.Count, source.Aocao.Count);
			for (int j = 0; j < num2; j++)
			{
				if (target.Aocao[j] != null && source.Aocao[j] != null)
				{
					target.Aocao[j].HasAocao = source.Aocao[j].HasAocao;
					target.Aocao[j].HasBaoshi = source.Aocao[j].HasBaoshi;
					target.Aocao[j].Name = source.Aocao[j].Name;
					target.Aocao[j].Type = source.Aocao[j].Type;
					target.Aocao[j].UseType = source.Aocao[j].UseType;
					target.Aocao[j].BS_Quality = source.Aocao[j].BS_Quality;
					target.Aocao[j].Number = source.Aocao[j].Number;
					target.Aocao[j].Icon = source.Aocao[j].Icon;
				}
			}
		}
		target.SPC = new List<WPSPC>();
		if (source.SPC != null)
		{
			foreach (WPSPC item in source.SPC)
			{
				target.SPC.Add((item == null) ? null : new WPSPC
				{
					Index = item.Index,
					EL = item.EL,
					PRC = item.PRC,
					price = item.price
				});
			}
		}
		target.Price = source.Price;
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

	public static BaoshiClass CloneBaoshi(BaoshiClass source)
	{
		if (source == null)
		{
			return null;
		}
		BaoshiClass baoshiClass = new BaoshiClass();
		CopyBaoshiTo(baoshiClass, source);
		return baoshiClass;
	}

	public static void CopyBaoshiTo(BaoshiClass target, BaoshiClass source)
	{
		if (target != null && source != null)
		{
			target.GlobalID = source.GlobalID;
			target.ItemType = source.ItemType;
			target.ItemName = source.ItemName;
			target.Price = source.Price;
			target.Quality = source.Quality;
			target.Size = source.Size;
			target.Icon = source.Icon;
			target.Level = source.Level;
			target.SoundDrop = source.SoundDrop;
			target.SoundUse = source.SoundUse;
			target.RotateType = source.RotateType;
			target.BStype = source.BStype;
			target.UseType = source.UseType;
			target.BS_Quality = source.BS_Quality;
			target.Number = source.Number;
			target.MstackSize = source.MstackSize;
			target.CstackSize = source.CstackSize;
			target.DropSpriteSize = source.DropSpriteSize;
			target.SKname = source.SKname;
			target.FWtype = source.FWtype;
			target.Index = source.Index;
			target.EL = source.EL;
			target.PRC = source.PRC;
			target.priceQulity = source.priceQulity;
			target.Xi = source.Xi;
		}
	}

	public static UseItemClass CloneUseItem(UseItemClass source)
	{
		if (source == null)
		{
			return null;
		}
		UseItemClass useItemClass = new UseItemClass();
		CopyUseItemTo(useItemClass, source);
		return useItemClass;
	}

	public static void CopyUseItemTo(UseItemClass target, UseItemClass source)
	{
		if (target != null && source != null)
		{
			target.GlobalID = source.GlobalID;
			target.ItemType = source.ItemType;
			target.ItemName = source.ItemName;
			target.Price = source.Price;
			target.Quality = source.Quality;
			target.Size = source.Size;
			target.Icon = source.Icon;
			target.Level = source.Level;
			target.SoundDrop = source.SoundDrop;
			target.SoundUse = source.SoundUse;
			target.RotateType = source.RotateType;
			target.InfoType = source.InfoType;
			target.UseType = source.UseType;
			target.damageType = source.damageType;
			target.Number = source.Number;
			target.CDTime = source.CDTime;
			target.Duration = source.Duration;
			target.MstackSize = source.MstackSize;
			target.CstackSize = source.CstackSize;
			target.DropSpriteSize = source.DropSpriteSize;
		}
	}
}
