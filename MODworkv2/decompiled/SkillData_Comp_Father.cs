using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class SkillData_Comp_Father : SkillData
{
	public string SonA;

	public string SonB;

	public string SonC;

	public bool SampleSkill;

	public bool BS_Skill;

	public bool LastSkill;

	public int UseAni;

	public int obj;

	public float Distance;

	public float ManaCost_Base;

	public float CoolDown_Base;

	public float Damage_Base;

	public float Damage_Level;

	public float Health_Base;

	public float Health_Level;

	public float Health_Prc_Base;

	public float Health_Prc_Level;

	public int Summon_count_Base;

	public DamageType damageType;

	public DamageType damageType_Change;

	public DamageType ChangeEL_SK;

	public DamageType ChangeEL_AR;

	public int DotMultiA;

	public int DotMultiB;

	public float DisA;

	public float DisB;

	public float Idle_Time_Min;

	public float Idle_Time_Max;

	public int SO_IdleRate;

	public int SO_AttackRate;

	public int SO_SayRate;

	public int SO_HurtRate;

	public int SO_DieRate;

	public string SO_Idle;

	public string SO_Walk;

	public string SO_AttackA;

	public string SO_SayA;

	public string SO_AttackB;

	public string SO_SayB;

	public string SO_AttackC;

	public string SO_SayC;

	public string SO_Hurt;

	public string SO_Die;

	public int Type_A;

	public int Type_B;

	public int TypeDIC_A;

	public int TypeDIC_B;

	public float JG_A;

	public float JG_B;

	public float AngleA;

	public float AngleB;

	public float FStimeA;

	public float FStimeB;

	public int Count_A;

	public int Count_B;

	public int Count_ATtarget_A;

	public int Count_ATtarget_B;

	public int CountMulti_A;

	public int CountMulti_B;

	public int Follow_A;

	public int Follow_B;

	public int AllChuan_A;

	public int AllChuan_B;

	public int RDSpeed_A;

	public int RDSpeed_B;

	public int HasFX_A;

	public int HasFX_B;

	public int colEXP_A;

	public int colEXP_B;

	public int EXPpos_A;

	public int EXPpos_B;

	public int BStype;

	public int AT_ZD = 100000;

	public int AT_FStype = 100000;

	public int AT_DMG = 100;

	public int AT_CT;

	public int AT_CT_AT;

	public int AT_CT_Multi;

	public int AT_FStime = 100;

	public int AT_Angle = 100;

	public int SK_ZD = 100000;

	public int SK_FStype = 100000;

	public int SK_DMG = 100;

	public int SK_CT;

	public int SK_CT_AT;

	public int SK_CT_Multi;

	public int SK_FStime = 100;

	public int SK_Angle = 100;

	public int Summon_count_Other;

	public int Summon_count_Type;

	public int CT_FS;

	public int CT_Double;

	public bool AT_Double;

	public int GD_R_Heal;

	public int BloodDie;

	public int TGYJ;

	public int AT_DotLayer;

	public bool BJ_NoDot;

	public bool WS_All;

	public int Field_Range;

	public int Kill_R_Heal;

	public int Hurt_FT;

	public int EveryDMG;

	public int EveryChuan;

	public int EveryATS;

	public int EveryMVS;

	public int EveryHeal;

	public int EveryMana;

	public int EveryCD;

	public int EveryBJR;

	public int EveryBJD;

	public int EveryGD;

	public int EveryDMG_Anti;

	public int EveryDotTimeCut;

	public int EveryAllChuan;

	public int EveryAllAnti;

	public int EveryDrop;

	public int EveryXJ_DMG;

	public int EveryORB_DMG;

	public int EveryDot_DMG;

	public bool AutoSummonOnReborn;

	public float ManaCost_Last => Mathf.FloorToInt((ManaCost_Base + SingletonMonoScope<TalentManager>.Instance.GetManaComp(Xi, base.IndexName)) * (100f - SingletonMonoScope<PlayerManager>.Instance.ManaXH) / 100f * SingletonMonoScope<PlayerManager>.Instance.GetB_DMG_Mana);

	public float CoolDown_Last => CoolDown_Base - CoolDown_Base * SingletonMonoScope<TalentManager>.Instance.GetCD_Comp(Xi, base.IndexName) / 100f;

	public float Damage_Max
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base > 1)
				{
					return Damage_Base + Damage_Level * (float)(base.Level_Base - 1) + (Damage_Base + Damage_Level * (float)(base.Level_Base - 1)) * Change_AT / 100f;
				}
				return Damage_Base + Damage_Base * Change_AT / 100f;
			}
			return 0f;
		}
	}

	public float Damage_Max_Tip
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return Damage_Base + Damage_Level * (float)(base.Level_Base_Last - 1) + (Damage_Base + Damage_Level * (float)(base.Level_Base_Last - 1)) * Change_AT / 100f;
			}
			return Damage_Base + Damage_Base * Change_AT / 100f;
		}
	}

	public float Health_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base_Last > 1)
				{
					return Health_Base + Health_Level * (float)(base.Level_Base - 1);
				}
				return Health_Base;
			}
			return 0f;
		}
	}

	public float Health_Last_Tip
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return Health_Base + Health_Level * (float)(base.Level_Base - 1);
			}
			return Health_Base;
		}
	}

	public float Health_Prc_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base_Last > 1)
				{
					return Health_Prc_Base + Health_Prc_Level * (float)(base.Level_Base - 1);
				}
				return Health_Prc_Base;
			}
			return 0f;
		}
	}

	public float Health_Prc_Last_Tip
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return Health_Prc_Base + Health_Prc_Level * (float)(base.Level_Base - 1);
			}
			return Health_Prc_Base;
		}
	}

	public float AttackSpeed_Last => SingletonMonoScope<TalentManager>.Instance.GetAttackSpeed_Comp(Xi, base.IndexName);

	public float GeDang_Last => SingletonMonoScope<TalentManager>.Instance.GetGeDang_Comp(Xi, base.IndexName);

	public int Summon_count_Last
	{
		get
		{
			int num = Summon_count_Base + SingletonMonoScope<TalentManager>.Instance.GetSummon_count_Comp(Xi, base.IndexName) + Summon_count_Other + SingletonMonoScope<PlayerManager>.Instance.CompCount;
			if (Summon_count_Type == 10)
			{
				return 1;
			}
			if (Summon_count_Type >= 1 && Summon_count_Type <= 9)
			{
				return num * (Summon_count_Type + 1);
			}
			return num;
		}
	}

	public float Change_AT => SingletonMonoScope<TalentManager>.Instance.GetChange_AT_Comp(Xi, base.IndexName);

	public float ATSrate => SingletonMonoScope<TalentManager>.Instance.GetATSrate_Comp(Xi, base.IndexName);

	public float ATS_Damage => SingletonMonoScope<TalentManager>.Instance.GetATS_Damage_Comp(Xi, base.IndexName);

	public float ARS_Damage => SingletonMonoScope<TalentManager>.Instance.GetARS_Damage_Comp(Xi, base.IndexName);

	public int Count_A_Last => ApplyCTDoubleCount(Count_A + CT_FS, CT_Double);

	public int Count_B_Last => ApplyCTDoubleCount(Count_B + CT_FS, CT_Double);

	public int Count_A_Change_Last => ApplyCTCount(Count_A_Last, AT_CT);

	public int Count_B_Change_Last => ApplyCTCount(Count_B_Last, SK_CT);

	public int Count_ATtarget_A_Change_Last => ApplyCTCount(Count_ATtarget_A, AT_CT_AT);

	public int Count_ATtarget_B_Change_Last => ApplyCTCount(Count_ATtarget_B, SK_CT_AT);

	public int CountMulti_A_Change_Last => ApplyCTCount(CountMulti_A, AT_CT_Multi);

	public int CountMulti_B_Change_Last => ApplyCTCount(CountMulti_B, SK_CT_Multi);

	public float FStimeA_Change_Last => FStimeA * (float)AT_FStime / 100f;

	public float AngleA_Change_Last => AngleA * (float)AT_Angle / 100f;

	public float FStimeB_Change_Last => FStimeB * (float)SK_FStime / 100f;

	public float AngleB_Change_Last => AngleB * (float)SK_Angle / 100f;

	private static int ApplyCTCount(int count, int type)
	{
		return type switch
		{
			1 => count * 2, 
			2 => count * 3, 
			3 => count * 4, 
			4 => count * 5, 
			5 => 1, 
			_ => count, 
		};
	}

	private static int ApplyCTDoubleCount(int count, int type)
	{
		switch (type)
		{
		case 10:
			return 1;
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
			return count * (type + 1);
		default:
			return count;
		}
	}

	public override string GetInfoA()
	{
		string empty = string.Empty;
		empty += LOC.MM.GetSkill(Info);
		empty += "\n";
		empty = ((base.Level_Base <= 0) ? (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Next Level"), base.Level_Base + 1)) : (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Current Level"), base.Level_Base_Last)));
		empty += string.Format("{0} : {1}", LOC.MM.GetMain("SummonCount"), Summon_count_Last);
		empty += $"\n{Damage_Max_Tip}% ({Mathf.Floor(Damage_Max_Tip / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
		empty += string.Format("\n{0} {1} ({2} {3} + {4} {5})", Health_Last_Tip + SingletonMonoScope<PlayerManager>.Instance.Damage_Last, LOC.MM.GetMain("Health"), LOC.MM.GetMain("Base Health"), Health_Last_Tip, LOC.MM.GetMain("Player LastDamage"), SingletonMonoScope<PlayerManager>.Instance.Damage_Last * 2f);
		return empty + string.Format("\n{0}% {1}", Health_Prc_Last_Tip, LOC.MM.GetMain("HealthPrc"));
	}

	public override string GetInfoB()
	{
		return string.Concat(string.Concat(string.Concat(string.Concat(string.Empty + string.Format("<color=#FFE397>{0}：{1}</color>\n", LOC.MM.GetMain("Next Level"), base.Level_Base_Last + 1), string.Format("{0} : {1}", LOC.MM.GetMain("SummonCount"), Summon_count_Last)), $"\n{Damage_Max_Tip + Damage_Level}% ({Mathf.Floor((Damage_Max_Tip + Damage_Level) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}"), string.Format("\n{0} {1} ({2} {3} + {4} {5})", Health_Last_Tip + Health_Level + SingletonMonoScope<PlayerManager>.Instance.Damage_Last, LOC.MM.GetMain("Health"), LOC.MM.GetMain("Base Health"), Health_Last_Tip + Health_Level, LOC.MM.GetMain("Player LastDamage"), SingletonMonoScope<PlayerManager>.Instance.Damage_Last * 2f)), string.Format("\n{0}% {1}", Health_Prc_Last_Tip + Health_Prc_Level, LOC.MM.GetMain("HealthPrc")));
	}
}
