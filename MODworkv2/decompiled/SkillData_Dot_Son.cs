using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class SkillData_Dot_Son : SkillData
{
	public string FrontSkill;

	public int FrontSkillType;

	public string FatherSkill;

	public DamageType damageType;

	public int SonType;

	public float BaseA;

	public float LevelA;

	public float BaseB;

	public float LevelB;

	public int Layer;

	public float LastA
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base > 1)
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
				if (base.Level_Base > 1)
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
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("damage"), LastA_Tip);
			break;
		case 1:
			empty = ((base.Level_Base <= 0) ? (empty + string.Format("{0} + {1}", LOC.MM.GetMain("Overlay"), Layer)) : (empty + string.Format("{0} + {1}", LOC.MM.GetMain("Overlay"), Layer + Layer * (base.Level_Base_Last - 1))));
			break;
		case 2:
			empty += string.Format("{0} + {1} {2}", LOC.MM.GetMain("Duration"), LastA_Tip, LOC.MM.GetMain("S"));
			break;
		case 3:
			empty += string.Format("{0} - {1}% {2}", LOC.MM.GetMain("Per stack"), LastA_Tip, LOC.MM.GetMain(SWS.El_Anti(damageType)));
			break;
		case 4:
			empty += string.Format("{0} - {1}% {2}{3}", LOC.MM.GetMain("Per stack"), LastA_Tip, LOC.MM.GetMain("enemy"), LOC.MM.GetMain("YunAnti"));
			break;
		case 5:
			empty += string.Format("{0} - {1}% {2}{3}", LOC.MM.GetMain("Per stack"), LastA_Tip, LOC.MM.GetMain("enemy"), LOC.MM.GetMain("damage"));
			break;
		case 6:
			empty += string.Format("{0}% {1}", LastA_Tip, LOC.MM.GetMain("MSrate"));
			empty += string.Format("\n{0} {1}%", LOC.MM.GetMain("MSnumber"), LastB_Tip);
			break;
		case 7:
			empty += string.Format("{0}% {1}", LastA_Tip, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n{0}% ({1}){2}", LastB_Tip, Mathf.Floor(LastB_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType)), LOC.MM.GetMain("damage"));
			break;
		case 8:
			empty += string.Format("{0}% {1}", LastA_Tip, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n{0}% ({1}){2}", LastB_Tip, Mathf.Floor(LastB_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType)), LOC.MM.GetMain("damage"));
			break;
		case 9:
			empty += string.Format("{0}% {1}", LastA_Tip, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n-{0}% {1}", LastB_Tip, LOC.MM.GetMain("HealthMax"));
			break;
		case 10:
			empty += string.Format("{0}% {1}", LastA_Tip, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n{0}S {1}{2}", LastB_Tip, LOC.MM.GetMain("Freeze"), LOC.MM.GetMain("Duration"));
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
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("damage"), LastA_Tip + LevelA);
			break;
		case 1:
			empty += string.Format("{0} + {1}", LOC.MM.GetMain("Overlay"), Layer * (base.Level_Base_Last + 1));
			break;
		case 2:
			empty += string.Format("+{0} {1} {2}", LastA_Tip + LevelA, LOC.MM.GetMain("S"), LOC.MM.GetMain("Duration"));
			break;
		case 3:
			empty += string.Format("{0} - {1}% {2}", LOC.MM.GetMain("Per stack"), LastA_Tip + LevelA, LOC.MM.GetMain(SWS.El_Anti(damageType)));
			break;
		case 4:
			empty += string.Format("{0} - {1}% {2}{3}", LOC.MM.GetMain("Per stack"), LastA_Tip + LevelA, LOC.MM.GetMain("enemy"), LOC.MM.GetMain("YunAnti"));
			break;
		case 5:
			empty += string.Format("{0} - {1}% {2}{3}", LOC.MM.GetMain("Per stack"), LastA_Tip + LevelA, LOC.MM.GetMain("enemy"), LOC.MM.GetMain("damage"));
			break;
		case 6:
			empty += string.Format("{0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("MSrate"));
			empty += string.Format("\n{0} {1}%", LOC.MM.GetMain("MSnumber"), LastB_Tip + LevelB);
			break;
		case 7:
			empty += string.Format("{0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n{0}% ({1} {2})", LastB_Tip + LevelB, Mathf.Floor((LastB_Tip + LevelB) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType)), LOC.MM.GetMain("damage"));
			break;
		case 8:
			empty += string.Format("{0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n{0}% ({1} {2})", LastB_Tip + LevelB, Mathf.Floor((LastB_Tip + LevelB) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType)), LOC.MM.GetMain("damage"));
			break;
		case 9:
			empty += string.Format("{0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n-{0}% {1}", LastB_Tip + LevelB, LOC.MM.GetMain("HealthMax"));
			break;
		case 10:
			empty += string.Format("{0}% {1}", LastA_Tip + LevelA, LOC.MM.GetMain("Rate"));
			empty += string.Format("\n+{0}S {1}{2}", LastB_Tip + LevelB, LOC.MM.GetMain("Freeze"), LOC.MM.GetMain("Duration"));
			break;
		}
		return empty;
	}
}
