using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class SkillData_Sample_Son : SkillData
{
	public string FrontSkill;

	public int FrontSkillType;

	public string FatherSkill;

	public float ManaCost;

	public DamageType damageType;

	public int SonType;

	public float BaseA;

	public float LevelA;

	public float BaseB;

	public float LevelB;

	public int SubAttackTypeA;

	public int SubAttackTypeB;

	public int multiCount_Type;

	public int Count;

	public float LastA
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base_Last > 1)
				{
					return BaseA + LevelA * (float)(base.Level_Base_Last - 1);
				}
				return BaseA;
			}
			return 0f;
		}
	}

	public float LastA_Tip
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BaseA + LevelA * (float)(base.Level_Base_Last - 1);
			}
			return BaseA;
		}
	}

	public float LastB
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base_Last > 1)
				{
					return BaseB + LevelB * (float)(base.Level_Base_Last - 1);
				}
				return BaseB;
			}
			return 0f;
		}
	}

	public float LastB_Tip
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BaseB + LevelB * (float)(base.Level_Base_Last - 1);
			}
			return BaseB;
		}
	}

	public int Count_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base_Last > 1)
				{
					return Count + Count * (base.Level_Base_Last - 1);
				}
				return Count;
			}
			return 0;
		}
	}

	public float Count_Last_Tip
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return Count + Count * (base.Level_Base_Last - 1);
			}
			return Count;
		}
	}

	public override string GetInfoA()
	{
		string empty = string.Empty;
		empty = empty + "<color=#E5BA60>" + LOC.MM.GetSkill(FatherSkill) + " </color>";
		empty += LOC.MM.GetSkill(Info);
		empty += "\n";
		empty = ((base.Level_Base <= 0) ? (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Next Level"), base.Level_Base + 1)) : (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Current Level"), base.Level_Base_Last)));
		SingletonMonoScope<TalentManager>.Instance.XiData[Xi].Sample_F.TryGetValue(FatherSkill, out var value);
		switch (SonType)
		{
		case 0:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("CD"));
			break;
		case 1:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("BJrate"));
			break;
		case 2:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("JYrate"));
			break;
		case 3:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("Throughrate"));
			break;
		case 4:
			empty += string.Format("- {0}% {1}", LastA_Tip, LOC.MM.GetMain("MoveSpeed"));
			if (LastB_Tip > 0f)
			{
				empty += string.Format("\n- {0}% {1}", LastB_Tip, LOC.MM.GetMain("AttackSpeed"));
			}
			empty += string.Format("\n{0} {1}{2}", LOC.MM.GetMain("Duration"), value.DebuffTime, LOC.MM.GetMain("S"));
			break;
		case 5:
			empty += $"- {LastA_Tip}% {LOC.MM.GetMain(SWS.El_Anti(damageType))}";
			empty += string.Format("\n{0} {1} {2}", LOC.MM.GetMain("Duration"), value.DebuffTime, LOC.MM.GetMain("S"));
			break;
		case 6:
			empty += string.Format("+ {0} {1} {2}", LastA_Tip, LOC.MM.GetMain("S"), LOC.MM.GetMain("Duration"));
			break;
		case 7:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("damage"));
			break;
		case 8:
			empty += $"+ {LastA_Tip}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 9:
			empty += $"+ {LastA_Tip}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 10:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("BJrate"));
			break;
		case 11:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("JYrate"));
			break;
		case 12:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("GeDang"));
			break;
		case 13:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 14:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("MoveSpeed"));
			break;
		case 15:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("DamageAnti"));
			break;
		case 16:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("HealthPrc"));
			break;
		case 17:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("Comp damage"));
			break;
		case 18:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("Comp AttackSpeed"));
			break;
		case 19:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("Comp MoveSpeed"));
			break;
		case 20:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("Comp HealthPrc"));
			break;
		case 21:
			empty += string.Format(" {0} : {1}%", LOC.MM.GetMain("Rate"), LastA_Tip);
			break;
		case 22:
			empty += string.Format(" {0} + {1}", LOC.MM.GetMain("Count"), Count_Last_Tip);
			break;
		case 23:
			empty += $"{LastA_Tip}% ({Mathf.Floor(LastA_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			if (SubAttackTypeA == 1)
			{
				empty += LOC.MM.GetMain("/S");
			}
			break;
		case 24:
			empty += $"{LastB_Tip}% ({Mathf.Floor(LastB_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			if (SubAttackTypeB == 1)
			{
				empty += LOC.MM.GetMain("/S");
			}
			break;
		case 25:
			empty += $"{LastA_Tip}% ({Mathf.Floor(LastA_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 30:
			empty += string.Format("+ {0} {1}", LastA_Tip, LOC.MM.GetMain("BJDamage"));
			break;
		case 31:
			empty += string.Format("+ {0} {1}", LastA_Tip, LOC.MM.GetMain("Thorn Projectile Speed"));
			break;
		}
		return empty;
	}

	public override string GetInfoB()
	{
		string empty = string.Empty;
		empty += string.Format("<color=#FFE397>{0}：{1}</color>\n", LOC.MM.GetMain("Next Level"), base.Level_Base_Last + 1);
		SingletonMonoScope<TalentManager>.Instance.XiData[Xi].Sample_F.TryGetValue(FatherSkill, out var value);
		switch (SonType)
		{
		case 0:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("CD"));
			break;
		case 1:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("BJrate"));
			break;
		case 2:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("JYrate"));
			break;
		case 3:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Throughrate"));
			break;
		case 4:
			empty += string.Format("- {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("MoveSpeed"));
			if (LastB_Tip + LevelB > 0f)
			{
				empty += string.Format("\n- {0}% {1}", LastB_Tip + LevelB, LOC.MM.GetMain("AttackSpeed"));
			}
			empty += string.Format("\n{0} {1} {2}", LOC.MM.GetMain("Duration"), value.DebuffTime, LOC.MM.GetMain("S"));
			break;
		case 5:
			empty += $"- {LastA_Tip + LevelA}% {LOC.MM.GetMain(SWS.El_Anti(damageType))}";
			empty += string.Format("\n{0} {1} {2}", LOC.MM.GetMain("Duration"), value.DebuffTime, LOC.MM.GetMain("S"));
			break;
		case 6:
			empty += string.Format("+ {0} {1} {2}", LastA_Tip + LevelA, LOC.MM.GetMain("S"), LOC.MM.GetMain("Duration"));
			break;
		case 7:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("damage"));
			break;
		case 8:
			empty += $"+ {LastA_Tip + LevelA}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 9:
			empty += $"+ {LastA_Tip + LevelA}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 10:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("BJrate"));
			break;
		case 11:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("JYrate"));
			break;
		case 12:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("GeDang"));
			break;
		case 13:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 14:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("MoveSpeed"));
			break;
		case 15:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("DamageAnti"));
			break;
		case 16:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("HealthPrc"));
			break;
		case 17:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Comp damage"));
			break;
		case 18:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Comp AttackSpeed"));
			break;
		case 19:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Comp MoveSpeed"));
			break;
		case 20:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Comp HealthPrc"));
			break;
		case 21:
			empty += string.Format("{0} : {1}%", LOC.MM.GetMain("Rate"), LastA_Tip + LevelA);
			break;
		case 22:
			empty += string.Format("{0} + {1}", LOC.MM.GetMain("Count"), Count_Last_Tip + (float)Count);
			break;
		case 23:
			empty += $"{LastA_Tip + LevelA}% ({Mathf.Floor((LastA_Tip + LevelA) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			if (SubAttackTypeA == 1)
			{
				empty += LOC.MM.GetMain("/S");
			}
			break;
		case 24:
			empty += $"{LastB_Tip + LevelB}% ({Mathf.Floor((LastB_Tip + LevelB) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			if (SubAttackTypeB == 1)
			{
				empty += LOC.MM.GetMain("/S");
			}
			break;
		case 25:
			empty += $"{LastA_Tip + LevelA}% ({Mathf.Floor((LastA_Tip + LevelA) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 30:
			empty += string.Format("+ {0} {1}", LastA_Tip + LevelA, LOC.MM.GetMain("BJDamage"));
			break;
		case 31:
			empty += string.Format("+ {0} {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Thorn Projectile Speed"));
			break;
		}
		return empty;
	}
}
