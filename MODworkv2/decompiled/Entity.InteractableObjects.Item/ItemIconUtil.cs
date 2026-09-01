using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Entity.InteractableObjects.Item;

public static class ItemIconUtil
{
	public static Sprite GetWeaponIcon(WeaponClass weapon)
	{
		if (weapon == null)
		{
			return null;
		}
		int num = ((!WeaponPlayerType.IsGeneric(weapon.PLtype)) ? weapon.PLtype : 0);
		if (num < 0 || num >= SingletonMonoScope<ItemManager>.Instance.Weapon.GP.Length)
		{
			return null;
		}
		switch (weapon.Quality)
		{
		case 0:
			foreach (Item_MB item in SingletonMonoScope<ItemManager>.Instance.Weapon.GP[num].QL[weapon.CharType].Normal)
			{
				if (item.GlobalID == weapon.GlobalID)
				{
					return SingletonMonoScope<ItemManager>.Instance.IconData[item.IconType].icon[item.Icon];
				}
			}
			break;
		case 1:
			foreach (Item_MB item2 in SingletonMonoScope<ItemManager>.Instance.Weapon.GP[num].QL[weapon.CharType].Magic)
			{
				if (item2.GlobalID == weapon.GlobalID)
				{
					return SingletonMonoScope<ItemManager>.Instance.IconData[item2.IconType].icon[item2.Icon];
				}
			}
			break;
		case 2:
			foreach (Item_MB item3 in SingletonMonoScope<ItemManager>.Instance.Weapon.GP[num].QL[weapon.CharType].Rare)
			{
				if (item3.GlobalID == weapon.GlobalID)
				{
					return SingletonMonoScope<ItemManager>.Instance.IconData[item3.IconType].icon[item3.Icon];
				}
			}
			break;
		case 3:
			foreach (Item_MB item4 in SingletonMonoScope<ItemManager>.Instance.Weapon.GP[num].QL[weapon.CharType].Exquisite)
			{
				if (item4.GlobalID == weapon.GlobalID)
				{
					return SingletonMonoScope<ItemManager>.Instance.IconData[item4.IconType].icon[item4.Icon];
				}
			}
			break;
		case 4:
			foreach (Item_MB item5 in SingletonMonoScope<ItemManager>.Instance.Weapon.GP[num].QL[weapon.CharType].Epic)
			{
				if (item5.GlobalID == weapon.GlobalID)
				{
					return SingletonMonoScope<ItemManager>.Instance.IconData[item5.IconType].icon[item5.Icon];
				}
			}
			break;
		case 5:
			foreach (Item_MB item6 in SingletonMonoScope<ItemManager>.Instance.Weapon.GP[num].QL[weapon.CharType].Legendary)
			{
				if (item6.GlobalID == weapon.GlobalID)
				{
					return SingletonMonoScope<ItemManager>.Instance.IconData[item6.IconType].icon[item6.Icon];
				}
			}
			break;
		case 6:
			foreach (Item_MB item7 in SingletonMonoScope<ItemManager>.Instance.Weapon.GP[num].QL[weapon.CharType].Mythical)
			{
				if (item7.GlobalID == weapon.GlobalID)
				{
					return SingletonMonoScope<ItemManager>.Instance.IconData[item7.IconType].icon[item7.Icon];
				}
			}
			break;
		}
		LogUtil.Error("未找到该装备的图标文件");
		return null;
	}

	public static Sprite GetBaoshiIcon(string itemName)
	{
		if (SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.TryGetBaoshiByItemName(itemName, out var data);
			if (data != null)
			{
				return data.Icon;
			}
		}
		LogUtil.Error("未找到该宝石的图标文件");
		return null;
	}

	public static Sprite GetBaoshiIcon(BaoshiClass baoshi)
	{
		if (baoshi == null)
		{
			return null;
		}
		switch (baoshi.UseType)
		{
		case 3:
			if (SingletonMonoScope<ItemManager>.Instance.SkillFW_Icon != null && SingletonMonoScope<ItemManager>.Instance.SkillFW_Icon.Length != 0)
			{
				return SingletonMonoScope<ItemManager>.Instance.SkillFW_Icon[Mathf.Clamp(baoshi.EL, 0, SingletonMonoScope<ItemManager>.Instance.SkillFW_Icon.Length - 1)];
			}
			break;
		case 4:
			if ((bool)SingletonMonoScope<ItemManager>.Instance.SPCFW_Icon)
			{
				return SingletonMonoScope<ItemManager>.Instance.SPCFW_Icon;
			}
			break;
		case 5:
			if ((bool)SingletonMonoScope<ItemManager>.Instance.BaseFW_Icon)
			{
				return SingletonMonoScope<ItemManager>.Instance.BaseFW_Icon;
			}
			break;
		}
		return GetBaoshiIcon(baoshi.ItemName);
	}

	public static Sprite GetUseItemIcon(UseItemClass use)
	{
		switch (use.InfoType)
		{
		case 0:
			foreach (UseItemClass item in SingletonMonoScope<ItemManager>.Instance.Potion)
			{
				if (item.GlobalID == use.GlobalID)
				{
					return item.Icon;
				}
			}
			break;
		case 1:
			foreach (UseItemClass item2 in SingletonMonoScope<ItemManager>.Instance.BuffPotion)
			{
				if (item2.GlobalID == use.GlobalID)
				{
					return item2.Icon;
				}
			}
			break;
		case 2:
			foreach (UseItemClass value in SingletonMonoScope<ItemManager>.Instance.Scroll.Values)
			{
				if (value.GlobalID == use.GlobalID)
				{
					return value.Icon;
				}
			}
			break;
		case 3:
		case 4:
			foreach (UseItemClass item3 in SingletonMonoScope<ItemManager>.Instance.PremPotion)
			{
				if (item3.GlobalID == use.GlobalID)
				{
					return item3.Icon;
				}
			}
			break;
		case 5:
			foreach (UseItemClass value2 in SingletonMonoScope<ItemManager>.Instance.SpcPotion.Values)
			{
				if (value2.GlobalID == use.GlobalID)
				{
					return value2.Icon;
				}
			}
			break;
		case 6:
		case 7:
			foreach (UseItemClass value3 in SingletonMonoScope<ItemManager>.Instance.SpcItem.Values)
			{
				if (value3.GlobalID == use.GlobalID)
				{
					return value3.Icon;
				}
			}
			break;
		}
		LogUtil.Error("未找到该消耗品的图标文件");
		return null;
	}
}
