using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class SkillData_Comp_Son : SkillData
{
	public string FrontSkill;

	public int FrontSkillType;

	public string FatherSkill;

	public float ManaCost;

	public int SonType;

	public float BaseA;

	public float LevelA;

	public float BaseB;

	public float LevelB;

	public int Summon_count;

	public DamageType ChangeEL_SK;

	public DamageType ChangeEL_AR;

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

	public override string GetInfoA()
	{
		string empty = string.Empty;
		empty = empty + "<color=#E5BA60>" + LOC.MM.GetSkill(FatherSkill) + " </color>";
		empty += LOC.MM.GetSkill(Info);
		empty += "\n";
		empty = ((base.Level_Base <= 0) ? (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Next Level"), base.Level_Base + 1)) : (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Current Level"), base.Level_Base_Last)));
		switch (SonType)
		{
		case 0:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 1:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("GeDang"));
			break;
		case 2:
			empty = ((base.Level_Base_Last <= 0) ? (empty + string.Format("{0} + {1}", LOC.MM.GetMain("SummonCount"), Summon_count)) : (empty + string.Format("{0} + {1}", LOC.MM.GetMain("SummonCount"), Summon_count + Summon_count * (base.Level_Base_Last - 1))));
			break;
		case 3:
			empty += string.Format("+ {0}% {1}", LastA_Tip, LOC.MM.GetMain("damage"));
			break;
		case 4:
			empty += string.Format("{0}% {1}", LastA_Tip, LOC.MM.GetMain("Rate"));
			empty += $"\n{LastB_Tip}% ({Mathf.Floor(LastB_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(ChangeEL_SK))}) {LOC.MM.GetMain(SWS.El_DMG(ChangeEL_SK))}";
			break;
		case 5:
			empty += string.Format("{0}% ({1}) {2} {3}", LastB_Tip, Mathf.Floor(LastB_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(ChangeEL_SK)), LOC.MM.GetMain(SWS.El_DMG(ChangeEL_SK)), LOC.MM.GetMain("/S"));
			break;
		}
		return empty;
	}

	public override string GetInfoB()
	{
		string empty = string.Empty;
		empty += string.Format("<color=#FFE397>{0}：{1}</color>\n", LOC.MM.GetMain("Next Level"), base.Level_Base_Last + 1);
		switch (SonType)
		{
		case 0:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 1:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("GeDang"));
			break;
		case 2:
			empty += string.Format("{0} + {1}", LOC.MM.GetMain("SummonCount"), Summon_count + Summon_count * base.Level_Base_Last);
			break;
		case 3:
			empty += string.Format("+ {0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("damage"));
			break;
		case 4:
			empty += string.Format("{0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Rate"));
			empty += $"\n{LastB_Tip + LevelB}% ({Mathf.Floor((LastB_Tip + LevelB) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(ChangeEL_SK))}) {LOC.MM.GetMain(SWS.El_DMG(ChangeEL_SK))}";
			break;
		case 5:
			empty += string.Format("{0}% ({1}) {2} {3}", LastB_Tip + LevelB, Mathf.Floor((LastB_Tip + LevelB) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(ChangeEL_AR)), LOC.MM.GetMain(SWS.El_DMG(ChangeEL_AR)), LOC.MM.GetMain("/S"));
			break;
		}
		return empty;
	}
}
