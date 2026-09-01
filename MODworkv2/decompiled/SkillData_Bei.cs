using System;

[Serializable]
public class SkillData_Bei : SkillData
{
	public DamageType damageType;

	public int B_Type;

	public float B_Number;

	public override string GetInfoA()
	{
		string empty = string.Empty;
		empty += LOC.MM.GetMain("BeiDong");
		empty += "\n";
		empty = ((base.Level_Base <= 0) ? (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Next Level"), base.Level_Base + 1)) : (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Current Level"), base.Level_Base_Last)));
		switch (B_Type)
		{
		case 0:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("HealthMax"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("HealthMax"))));
			break;
		case 1:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("ManaMax"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("ManaMax"))));
			break;
		case 2:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("AttackSpeed"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("AttackSpeed"))));
			break;
		case 3:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("MoveSpeed"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("MoveSpeed"))));
			break;
		case 4:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("CD"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("CD"))));
			break;
		case 5:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("GeDang"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("GeDang"))));
			break;
		case 6:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("- {0}% {1}{2}", B_Number, LOC.MM.GetMain("Dot"), LOC.MM.GetMain("Duration"))) : (empty + string.Format("- {0}% {1}{2}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Dot"), LOC.MM.GetMain("Duration"))));
			break;
		case 7:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Throughrate"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Throughrate"))));
			break;
		case 8:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("DamageAnti"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("DamageAnti"))));
			break;
		case 9:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}"));
			break;
		case 10:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}"));
			break;
		case 11:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}"));
			break;
		case 12:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}"));
			break;
		case 13:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}"));
			break;
		case 14:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}"));
			break;
		case 15:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}"));
			break;
		case 16:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}"));
			break;
		case 17:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}"));
			break;
		case 18:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}"));
			break;
		case 19:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}"));
			break;
		case 20:
			empty = ((base.Level_Base <= 0) ? (empty + $"+ {B_Number}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}") : (empty + $"+ {B_Number * (float)base.Level_Base_Last}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}"));
			break;
		case 21:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Comp HealthMax"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Comp HealthMax"))));
			break;
		case 22:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Comp damage"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Comp damage"))));
			break;
		case 23:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Comp AttackSpeed"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Comp AttackSpeed"))));
			break;
		case 24:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Comp MoveSpeed"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Comp MoveSpeed"))));
			break;
		case 25:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Comp AllAnti"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Comp AllAnti"))));
			break;
		case 26:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("ATR_health"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("ATR_health"))));
			break;
		case 27:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("ATR_mana"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("ATR_mana"))));
			break;
		case 28:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("damage"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("damage"))));
			break;
		case 29:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("BJrate"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("BJrate"))));
			break;
		case 30:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("BJDamage"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("BJDamage"))));
			break;
		case 31:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Projectile Speed"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("Projectile Speed"))));
			break;
		case 32:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("+ {0}% {1}", B_Number, LOC.MM.GetMain("Projectile Speed"))) : (empty + string.Format("+ {0}% {1}", B_Number * (float)base.Level_Base_Last, LOC.MM.GetMain("SP Damage"))));
			break;
		}
		return empty;
	}

	public override string GetInfoB()
	{
		string empty = string.Empty;
		empty += string.Format("<color=#FFE397>{0}：{1}</color>\n", LOC.MM.GetMain("Next Level"), base.Level_Base_Last + 1);
		switch (B_Type)
		{
		case 0:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("HealthMax"));
			break;
		case 1:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("ManaMax"));
			break;
		case 2:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("AttackSpeed"));
			break;
		case 3:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("MoveSpeed"));
			break;
		case 4:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("CD"));
			break;
		case 5:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("GeDang"));
			break;
		case 6:
			empty += string.Format("+ {0}% {1}{2}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Dot"), LOC.MM.GetMain("Duration"));
			break;
		case 7:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Throughrate"));
			break;
		case 8:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("DamageAnti"));
			break;
		case 9:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 10:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 11:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 12:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 13:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 14:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 15:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 16:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 17:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 18:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 19:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 20:
			empty += $"+ {B_Number * (float)(base.Level_Base_Last + 1)}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 21:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Comp HealthMax"));
			break;
		case 22:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Comp damage"));
			break;
		case 23:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Comp AttackSpeed"));
			break;
		case 24:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Comp MoveSpeed"));
			break;
		case 25:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Comp AllAnti"));
			break;
		case 26:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("ATR_health"));
			break;
		case 27:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("ATR_mana"));
			break;
		case 28:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("damage"));
			break;
		case 29:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("BJrate"));
			break;
		case 30:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("BJDamage"));
			break;
		case 31:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("Projectile Speed"));
			break;
		case 32:
			empty += string.Format("+ {0}% {1}", B_Number * (float)(base.Level_Base_Last + 1), LOC.MM.GetMain("SP Damage"));
			break;
		}
		return empty;
	}
}
