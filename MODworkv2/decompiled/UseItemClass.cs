using System;
using System.Text;
using Core;
using Core.Teleport;
using Entity.InteractableObjects.Item;
using FinkFramework.Runtime.Singleton;
using UI.Managers;
using UI.Panels;
using UnityEngine;

[Serializable]
public class UseItemClass : ItemClass, IDropItemData
{
	public int InfoType;

	public string UseType;

	public DamageType damageType;

	public int Number;

	public float CDTime;

	public int Duration;

	public int MstackSize;

	public int CstackSize;

	public int DropSpriteSize;

	public int DropScene;

	public float DurationLast => Mathf.FloorToInt((float)Duration + (float)Duration * SingletonMonoScope<PlayerManager>.Instance.BuffT_Drink / 100f);

	public int MaxPrice => Price * CstackSize;

	public int ByPrice => MaxPrice * 8;

	int IDropItemData.ItemType => ItemType;

	public override void Reset()
	{
		base.Reset();
		InfoType = 0;
		UseType = string.Empty;
		damageType = DamageType.fire;
		Number = 0;
		CDTime = 0f;
		Duration = 0;
		MstackSize = 0;
		CstackSize = 0;
		DropSpriteSize = 0;
	}

	public bool Use()
	{
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		switch (InfoType)
		{
		case 0:
			switch (UseType)
			{
			case "health":
				instance.HealStat.Cur += Mathf.Floor((float)Number + (float)(instance.Level * 3) * Mathf.Pow(1.065f, Level));
				if (SingletonMonoScope<PlayerManager>.Instance.Drink_CP)
				{
					for (int i = 0; i < SingletonMonoScope<ACTbar>.Instance.actListSkill.Count; i++)
					{
						if (SingletonMonoScope<ACTbar>.Instance.actListSkill[i].DT.type == 1)
						{
							for (int j = 0; j < SingletonMonoScope<ACTbar>.Instance.actListSkill[i].cpList.Count; j++)
							{
								SingletonMonoScope<ACTbar>.Instance.actListSkill[i].cpList[j].HealthStat.CurrentValue += Mathf.Floor((float)Number + (float)(instance.Level * 3) * Mathf.Pow(1.065f, Level));
							}
						}
					}
				}
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			case "mana":
				instance.ManaStat.Cur += Mathf.Floor((float)Number + (float)instance.Level * 0.8f * Mathf.Pow(1.065f, Level));
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			case "huoli":
				instance.HealStat.Cur += instance.HealStat.Max * (float)Number;
				instance.ManaStat.Cur += instance.ManaStat.Max * (float)Number;
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			}
			break;
		case 1:
			switch (UseType)
			{
			case "EL_Damage":
				SingletonMonoScope<BuffManager>.Instance.AddPotionBuff(this);
				break;
			case "EL_Anti":
				SingletonMonoScope<BuffManager>.Instance.AddPotionBuff(this);
				break;
			case "xueshi":
				SingletonMonoScope<BuffManager>.Instance.AddPotionBuff(this);
				break;
			case "xingyun":
				SingletonMonoScope<BuffManager>.Instance.AddPotionBuff(this);
				break;
			case "zhaohuan":
				SingletonMonoScope<BuffManager>.Instance.AddPotionBuff(this);
				break;
			case "poe_flask_gale":
			case "poe_flask_insight":
				SingletonMonoScope<BuffManager>.Instance.AddPotionBuff(this);
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			}
			break;
		case 2:
			switch (UseType)
			{
			case "green":
				SingletonMonoScope<PortalManager>.Instance.RequestOpenChallengePortal();
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			case "blue":
				SingletonMonoScope<PortalManager>.Instance.RequestOpenChallengePortal(2);
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			case "purple":
				SingletonMonoScope<PortalManager>.Instance.RequestOpenChallengePortal(3);
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			case "red":
				SingletonMonoScope<PortalManager>.Instance.RequestOpenChallengePortal(4);
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			case "yellow":
				SingletonMonoScope<PortalManager>.Instance.RequestOpenChallengePortal();
				SingletonMonoScope<SimplePotionManager>.Instance.AddSimpleDrink(this);
				break;
			}
			break;
		case 3:
			switch (UseType)
			{
			case "taitan":
			case "taitan1":
			case "taitan2":
			case "taitan3":
				instance.Health += Number;
				break;
			case "zhihui":
			case "zhihui1":
			case "zhihui2":
			case "zhihui3":
				instance.Mana += Number;
				break;
			case "zhandou":
			case "zhandou1":
			case "zhandou2":
			case "zhandou3":
				instance.Damage_Base += Number;
				break;
			case "fusu":
			case "fusu1":
			case "fusu2":
			case "fusu3":
				instance.Health_R_Base += Number;
				break;
			case "shanguang1":
			case "shanguang2":
			case "shanguang3":
			case "shanguang":
				instance.Mana_R_Base += Number;
				break;
			case "fire":
				instance.FireDamage_Bei += Number;
				break;
			case "frozen":
				instance.FrozenDamage_Bei += Number;
				break;
			case "thunder":
				instance.ThunderDamage_Bei += Number;
				break;
			case "poison":
				instance.PoisonDamage_Bei += Number;
				break;
			case "physics":
				instance.PhysicsDamage_Bei += Number;
				break;
			case "shadow":
				instance.ShadowDamage_Bei += Number;
				break;
			}
			instance.Health += instance.DrinkPre_Heal;
			instance.Mana += instance.DrinkPre_Mana;
			instance.Damage_Base += instance.DrinkPre_DMG;
			instance.RefreshRuntimeDerivedStats();
			GameManager.ShowTip(GetMain(), TipType.Success);
			break;
		case 4:
			switch (UseType)
			{
			case "ST_Fire":
				instance.FireAnti += Number;
				break;
			case "ST_Ice":
				instance.FrozenAnti += Number;
				break;
			case "ST_TD":
				instance.ThunderAnti += Number;
				break;
			case "ST_Du":
				instance.PoisonAnti += Number;
				break;
			case "ST_Phy":
				instance.PhysicsAnti += Number;
				break;
			case "ST_SD":
				instance.ShadowAnti += Number;
				break;
			}
			instance.Health += instance.DrinkPre_Heal;
			instance.Mana += instance.DrinkPre_Mana;
			instance.Damage_Base += instance.DrinkPre_DMG;
			instance.RefreshRuntimeDerivedStats();
			GameManager.ShowTip(GetMain(), TipType.Success);
			break;
		case 5:
			switch (UseType)
			{
			case "yiwang":
				SingletonMonoScope<TalentManager>.Instance.Restore();
				break;
			case "lunhui":
				SingletonMonoScope<TalentManager>.Instance.RestoreDF();
				break;
			case "shenyou":
				SingletonMonoScope<TalentManager>.Instance.LevelUP();
				break;
			case "juexing":
				instance.GainXp(Number);
				break;
			}
			GameManager.ShowTip(GetMain(), TipType.Success);
			break;
		case 6:
		{
			string useType = UseType;
			if (!(useType == "bag"))
			{
				if (useType == "keyA")
				{
					if (!SingletonMonoScope<WarehouseManager>.HasInstance || !SingletonMonoScope<WarehouseManager>.Instance.CreatePage())
					{
						GameManager.ShowTip(LOC.MM.GetMain("page_limit_reached"), TipType.Fail);
						return false;
					}
					GameManager.ShowTip(LOC.MM.GetMain("storge_add"), TipType.Success);
				}
			}
			else
			{
				if (!SingletonMonoScope<InventoryManager>.HasInstance || !SingletonMonoScope<InventoryManager>.Instance.CreatePage())
				{
					GameManager.ShowTip(LOC.MM.GetMain("page_limit_reached"), TipType.Fail);
					return false;
				}
				GameManager.ShowTip(LOC.MM.GetMain("bag_add"), TipType.Success);
			}
			break;
		}
		case 7:
			switch (UseType)
			{
			case "hammerA":
				instance.Reforge_Inc++;
				GameManager.ShowTip(string.Format("{0} +{1}", LOC.MM.GetMain("hammerA_add"), Number), TipType.Success);
				break;
			case "hammerB":
				instance.QH_Inc++;
				GameManager.ShowTip(string.Format("{0} +{1}", LOC.MM.GetMain("hammerB_add"), Number), TipType.Success);
				break;
			case "hammerC":
				instance.HH_Inc++;
				GameManager.ShowTip(string.Format("{0} +{1}", LOC.MM.GetMain("hammerC_add"), Number), TipType.Success);
				break;
			case "hammerD":
				instance.SK_Inc++;
				GameManager.ShowTip(string.Format("{0} +{1}", LOC.MM.GetMain("hammerD_add"), Number), TipType.Success);
				break;
			case "mirrorA":
				instance.Pick_PL_Bei += Number;
				GameManager.ShowTip(string.Format("{0} +{1}%", LOC.MM.GetMain("mirrorA_add"), Number), TipType.Success);
				break;
			case "mirrorB":
				instance.Pick_XJL_Bei += Number;
				GameManager.ShowTip(string.Format("{0} +{1}%", LOC.MM.GetMain("mirrorB_add"), Number), TipType.Success);
				break;
			case "mirrorC":
				instance.XJL_SellPrice += Number;
				GameManager.ShowTip(string.Format("{0} +{1}%", LOC.MM.GetMain("mirrorC_add"), Number), TipType.Success);
				break;
			}
			break;
		}
		return true;
	}

	public string GetMain()
	{
		string text = string.Empty;
		if (PoeItemMod.TryGetDescription(ItemName, out var text2))
		{
			text = text + "<color=#00E5FF>" + text2 + "</color>\n";
		}
		switch (InfoType)
		{
		case 0:
			switch (UseType)
			{
			case "health":
				text += string.Format("{0}{1}{2}", LOC.MM.GetMain("Now Restores"), Mathf.Floor((float)Number + (float)(SingletonMonoScope<PlayerManager>.Instance.Level * 3) * Mathf.Pow(1.065f, Level)), LOC.MM.GetMain("Health"));
				break;
			case "mana":
				text += string.Format("{0}{1}{2}", LOC.MM.GetMain("Now Restores"), Mathf.Floor((float)Number + (float)SingletonMonoScope<PlayerManager>.Instance.Level * 0.8f * Mathf.Pow(1.065f, Level)), LOC.MM.GetMain("Mana"));
				break;
			case "huoli":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Restores"), Number, LOC.MM.GetMain("HealthMax"));
				text += string.Format("\n{0} {1}% {2}", LOC.MM.GetMain("Restores"), Number, LOC.MM.GetMain("ManaMax"));
				break;
			}
			break;
		case 1:
			switch (UseType)
			{
			case "EL_Damage":
				text += $"+{Number}%{LOC.MM.GetMain(SWS.El_DMG(damageType))}";
				break;
			case "EL_Anti":
				text += $"+{Number}%{LOC.MM.GetMain(SWS.El_Anti(damageType))}";
				break;
			case "xueshi":
				text += string.Format("+{0}%{1}", Number, LOC.MM.GetMain("Experience Gain"));
				break;
			case "xingyun":
				text += string.Format("+{0}%{1}", Number, LOC.MM.GetMain("DropRate"));
				break;
			case "zhaohuan":
				text += string.Format("+{0}%{1}", Number, LOC.MM.GetMain("Comp damage"));
				break;
			case "poe_flask_gale":
				text += string.Format("+{0}%{1}", Number, LOC.MM.GetMain("MoveSpeed"));
				text += string.Format("\n{0} {1}s / CD {2}s", LOC.MM.GetMain("Duration"), Duration, CDTime);
				break;
			case "poe_flask_insight":
				text += string.Format("+{0}%{1}", Number, LOC.MM.GetMain("BJrate"));
				text += string.Format("\n{0} {1}s / CD {2}s", LOC.MM.GetMain("Duration"), Duration, CDTime);
				break;
			}
			break;
		case 2:
			switch (UseType)
			{
			case "green":
				text += LOC.MM.GetMain("Summon a Random Challenge Portal");
				break;
			case "blue":
				text += LOC.MM.GetMain("Summon an Arena Portal");
				break;
			case "purple":
				text += LOC.MM.GetMain("Summon a Demon Challenge Portal");
				break;
			case "red":
				text += LOC.MM.GetMain("Summon an Inferno Challenge Portal");
				break;
			case "yellow":
				text += LOC.MM.GetMain("Summon a Treasure Realm Portal");
				break;
			}
			break;
		case 3:
			switch (UseType)
			{
			case "taitan":
			case "taitan1":
			case "taitan2":
			case "taitan3":
				text += string.Format("{0} {1} {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("Health"));
				break;
			case "zhihui":
			case "zhihui1":
			case "zhihui2":
			case "zhihui3":
				text += string.Format("{0} {1} {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("Mana"));
				break;
			case "zhandou":
			case "zhandou1":
			case "zhandou2":
			case "zhandou3":
				text += string.Format("{0} {1} {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("damage"));
				break;
			case "fusu":
			case "fusu1":
			case "fusu2":
			case "fusu3":
				text += string.Format("{0} {1} {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("health recovery"));
				break;
			case "shanguang1":
			case "shanguang2":
			case "shanguang3":
			case "shanguang":
				text += string.Format("{0} {1} {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("mana recovery"));
				break;
			case "fire":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("fire damage"));
				break;
			case "frozen":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("frozen damage"));
				break;
			case "thunder":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("thunder damage"));
				break;
			case "poison":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("poison damage"));
				break;
			case "physics":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("physics damage"));
				break;
			case "shadow":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("shadow damage"));
				break;
			}
			break;
		case 4:
			switch (UseType)
			{
			case "ST_Fire":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("fire Anti"));
				break;
			case "ST_Ice":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("frozen Anti"));
				break;
			case "ST_TD":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("thunder Anti"));
				break;
			case "ST_Du":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("poison Anti"));
				break;
			case "ST_Phy":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("physics Anti"));
				break;
			case "ST_SD":
				text += string.Format("{0} {1}% {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("shadow Anti"));
				break;
			}
			break;
		case 5:
			switch (UseType)
			{
			case "yiwang":
				text += LOC.MM.GetMain("Reset skill talent");
				break;
			case "lunhui":
				text += LOC.MM.GetMain("Reset DF talent");
				break;
			case "shenyou":
				text += string.Format("+{0} {1}", Number, LOC.MM.GetMain("Skill Points"));
				break;
			case "juexing":
				text += string.Format("{0} {1}", LOC.MM.GetMain("Gain Experience"), Number);
				break;
			}
			break;
		case 6:
		{
			string useType = UseType;
			if (!(useType == "bag"))
			{
				if (useType == "keyA")
				{
					text += string.Format("{0} {1} {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("Chest Pages"));
				}
			}
			else
			{
				text += string.Format("{0} {1} {2}", LOC.MM.GetMain("Permanent Increase"), Number, LOC.MM.GetMain("Backpack Pages"));
			}
			break;
		}
		case 7:
			switch (UseType)
			{
			case "hammerA":
				text += string.Format("{0} +{1}", LOC.MM.GetMain("hammerA Use"), Number);
				break;
			case "hammerB":
				text += string.Format("{0} +{1}", LOC.MM.GetMain("hammerB Use"), Number);
				break;
			case "hammerC":
				text += string.Format("{0} +{1}", LOC.MM.GetMain("hammerC Use"), Number);
				break;
			case "hammerD":
				text += string.Format("{0} +{1}", LOC.MM.GetMain("hammerD Use"), Number);
				break;
			case "mirrorA":
				text += string.Format("{0} +{1}%", LOC.MM.GetMain("mirrorA Use"), Number);
				break;
			case "mirrorB":
				text += string.Format("{0} +{1}%", LOC.MM.GetMain("mirrorB Use"), Number);
				break;
			case "mirrorC":
				text += string.Format("{0} +{1}%", LOC.MM.GetMain("mirrorC Use"), Number);
				break;
			}
			break;
		}
		return text;
	}

	public float GetNameSize()
	{
		return Encoding.Default.GetByteCount(LOC.MM.GetItem(ItemName));
	}
}
