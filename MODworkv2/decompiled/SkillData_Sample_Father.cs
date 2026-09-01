using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class SkillData_Sample_Father : SkillData
{
	public string SonA;

	public string SonB;

	public string SonC;

	public bool SampleSkill;

	public bool BS_Skill;

	public bool LastSkill;

	public bool DashSkill;

	public bool TPSkill;

	public int UseAni;

	public int FStype;

	public int LockType;

	public int Father_type;

	public int OBJ;

	public int RTtypeOBJ;

	public int RTtypeFX;

	public float Distance;

	public int SpecialType;

	public float ManaCost_Base;

	public float CoolDown_Base;

	public DamageType damageType;

	public int MainEL;

	public int ThroughType;

	public bool AttackType;

	public float Damage_Base;

	public float Damage_Level;

	public float BJrate_Base;

	public float BJDamage_Base;

	public float JYrate_Base;

	public float Through_Base;

	public float FlySpeed_Base;

	public float MoveSpeedCut_Base;

	public float AttackSpeedCut_Base;

	public float AntiCut_Base;

	public float BF_Damage_Base;

	public float BF_Damage_Level;

	public float BF_EL_Damage_Base;

	public float BF_EL_Damage_Level;

	public float BF_EL_Chuan_Base;

	public float BF_EL_Chuan_Level;

	public float BF_BJrate_Base;

	public float BF_BJrate_Level;

	public float BF_JYrate_Base;

	public float BF_JYrate_Level;

	public float BF_GeDang_Base;

	public float BF_GeDang_Level;

	public float BF_AttackSpeed_Base;

	public float BF_AttackSpeed_Level;

	public float BF_MoveSpeed_Base;

	public float BF_MoveSpeed_Level;

	public float BF_DamageAnti_Base;

	public float BF_DamageAnti_Level;

	public float BF_Health_Prc_Base;

	public float BF_Health_Prc_Level;

	public float C_Damage_Base;

	public float C_Damage_Level;

	public float C_ATspeed_Base;

	public float C_ATspeed_Level;

	public float C_MVspeed_Base;

	public float C_MVspeed_Level;

	public float C_Health_Prc_Base;

	public float C_Health_Prc_Level;

	public string BSAT;

	public int BSAT_Count;

	public int BSAT_Angle;

	public int Is_BS;

	public int ChangeSkin;

	public int SkinIndex;

	public int Reborn;

	public int NoTime;

	public float BuffTime_Base;

	public float DebuffTime;

	public float Field_time;

	public float ORB_time;

	public float EXP_time;

	public float ZD_time_F;

	public float ZD_time_S;

	public int Layer_SubA;

	public int Layer_SubB;

	public int ORB;

	public int ZD_F;

	public int ZD_S;

	public int ZD_AB;

	public int EXP_F;

	public int EXP_S;

	public int EXP_AB;

	public int Dic_F;

	public int Dic_S;

	public int FX_F;

	public int FX_S;

	public int Sound;

	public int Count_ORB;

	public int Count_ATtarget_Base;

	public int CF_Count;

	public int Count_F_Base;

	public int Count_S_Base;

	public int Count_AB;

	public int CountMulti;

	public int CountEXP;

	public int TypeORB;

	public int CF_Type;

	public int Type_F;

	public int Type_S;

	public int Type_AB;

	public int TypeDIC_F;

	public int TypeDIC_S;

	public int TypeEXP_F;

	public int TypeEXP_S;

	public int TypeEXP_AB;

	public float Size;

	public float High;

	public float JG;

	public float AngleA;

	public float AngleB;

	public float Range1;

	public float Range2;

	public float Range_AT;

	public float FStime1;

	public float FStime2;

	public float Speed1;

	public float Speed2;

	public float Speed3;

	public float Speed4;

	public int Follow_F;

	public int Follow_S;

	public int AllChuan_F;

	public int AllChuan_S;

	public int Slow_F;

	public int Slow_S;

	public int RDSpeed_F;

	public int RDSpeed_S;

	public int HasFX;

	public int S_HasFX;

	public int A_HasFX;

	public int colEXP;

	public int colEXP_A;

	public int S_colEXP;

	public int A_colEXP;

	public int TimeEXP;

	public int TimeEXP_A;

	public int LastEXP;

	public int LastEXP_A;

	public int S_LastEXP;

	public int A_LastEXP;

	public int EXPpos;

	public int EXPpos_A;

	public int S_EXPpos;

	public int A_EXPpos;

	public int AngleEXP;

	public int AngleEXP_A;

	public int OBJ_Group;

	public int FS_ZD_F = 100000;

	public int FS_ZD_S = 100000;

	public int FS_Dic_F = 100000;

	public int FS_Type_F = 100000;

	public int FS_Type_Dic_F = 100000;

	public float FS_DMG = 100f;

	public int FS_CT_F;

	public int FS_CT_S;

	public int FS_CT_AT;

	public int FS_CT_Multi;

	public int FS_Time1 = 100;

	public int FS_Time2 = 100;

	public int FS_Range1 = 100;

	public int FS_AngleA = 100;

	public int CT_F;

	public int CT_S;

	public int CT_AT;

	public int CT_Mul;

	public string[] LinkSK;

	public bool LinkAll;

	public bool EveryLink;

	public string JCskill;

	public bool AutoUse;

	public int Refresh;

	public int ATtar_DMG;

	public int CompUP_DMG;

	public int ATtarUP;

	public int MS_Dead;

	public int GD_Use;

	public int BSAT_DMG;

	public bool Double;

	public float WD;

	public int Crit_Time;

	public int Crit_CD;

	public int Over_Prc;

	public int CutSpeedZone;

	public int UseDMG;

	public int UseATS;

	public int UseMVS;

	public int UseCP_DMG;

	public int UseCP_ATS;

	public int UseDMG_EL0;

	public int UseDMG_EL1;

	public int UseDMG_EL2;

	public int UseDMG_EL3;

	public int UseDMG_EL4;

	public int UseDMG_EL5;

	public int UseChuan0;

	public int UseChuan1;

	public int UseChuan2;

	public int UseChuan3;

	public int UseChuan4;

	public int UseChuan5;

	public int Has_DMG;

	public int Has_ATS;

	public int Has_MVS;

	public int Has_BJR;

	public int Has_BJD;

	public int Has_DotTimeCut;

	public int Has_DMG_Cut;

	public int Has_GD;

	public int Has_ORB_DMG;

	public int Has_XJ_DMG;

	public int Has_Dot_DMG;

	public int Has_CP_DMG;

	public float ManaCost_Last => Mathf.FloorToInt((ManaCost_Base + SingletonMonoScope<TalentManager>.Instance.GetManaSample(Xi, base.IndexName) + (ManaCost_Base + SingletonMonoScope<TalentManager>.Instance.GetManaSample(Xi, base.IndexName)) * 0.01f * Mathf.Pow(1.065f, SingletonMonoScope<PlayerManager>.Instance.Level)) * (100f - SingletonMonoScope<PlayerManager>.Instance.ManaXH) / 100f * SingletonMonoScope<PlayerManager>.Instance.GetB_DMG_Mana);

	public float CoolDown_Last => CoolDown_Base - CoolDown_Base * SingletonMonoScope<TalentManager>.Instance.GetCD_Sample(Xi, base.IndexName) / 100f;

	public bool SubAttackTypeA => SingletonMonoScope<TalentManager>.Instance.GetSubAttackTypeA(Xi, base.IndexName);

	public bool SubAttackTypeB => SingletonMonoScope<TalentManager>.Instance.GetSubAttackTypeB(Xi, base.IndexName);

	public float Damage_Max
	{
		get
		{
			if (base.Level_Base > 0)
			{
				if (base.Level_Base_Last > 1)
				{
					return (Damage_Base + Damage_Level * (float)(base.Level_Base_Last - 1)) * FS_DMG_Last / 100f * (float)DoubleDMG;
				}
				return Damage_Base * FS_DMG_Last / 100f * (float)DoubleDMG;
			}
			return Damage_Base * FS_DMG_Last / 100f * (float)DoubleDMG;
		}
	}

	public float Sub_DamageA => SingletonMonoScope<TalentManager>.Instance.GetSub_DamageA(Xi, base.IndexName) * FS_DMG_Last / 100f * (float)DoubleDMG;

	public float Sub_DamageB => SingletonMonoScope<TalentManager>.Instance.GetSub_DamageB(Xi, base.IndexName) * FS_DMG_Last / 100f * (float)DoubleDMG;

	public float BJrate_Last => BJrate_Base + SingletonMonoScope<TalentManager>.Instance.GetBJrate(Xi, base.IndexName);

	public float BJDamage_Last => BJDamage_Base + SingletonMonoScope<TalentManager>.Instance.Get_BJDamage(Xi, base.IndexName);

	public float JYrate_Last => JYrate_Base + SingletonMonoScope<TalentManager>.Instance.GetJYrate(Xi, base.IndexName);

	public float Through_Last => Through_Base + SingletonMonoScope<TalentManager>.Instance.GetThrough(Xi, base.IndexName);

	public float FlySpeed_Last => FlySpeed_Base + SingletonMonoScope<TalentManager>.Instance.GetFlySpeed(Xi, base.IndexName);

	public float MoveSpeedCut_Last => MoveSpeedCut_Base + SingletonMonoScope<TalentManager>.Instance.GetMVspeedCut(Xi, base.IndexName);

	public float AttackSpeedCut_Last => AttackSpeedCut_Base + SingletonMonoScope<TalentManager>.Instance.GetATspeedCut(Xi, base.IndexName);

	public float AntiCut_Last => AntiCut_Base + SingletonMonoScope<TalentManager>.Instance.GetAntiCut(Xi, base.IndexName);

	public float BF_Damage_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_Damage_Base + BF_Damage_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_Damage(Xi, base.IndexName);
			}
			return BF_Damage_Base + SingletonMonoScope<TalentManager>.Instance.GetBF_Damage(Xi, base.IndexName);
		}
	}

	public float BF_EL_Damage_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_EL_Damage_Base + BF_EL_Damage_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_EL_Damage(Xi, base.IndexName);
			}
			return BF_EL_Damage_Base + SingletonMonoScope<TalentManager>.Instance.GetBF_EL_Damage(Xi, base.IndexName);
		}
	}

	public float BF_EL_Chuan_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_EL_Chuan_Base + BF_EL_Chuan_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_EL_Chuan(Xi, base.IndexName);
			}
			return BF_EL_Chuan_Base;
		}
	}

	public float BF_BJrate_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_BJrate_Base + BF_BJrate_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_BJrate(Xi, base.IndexName);
			}
			return BF_BJrate_Base;
		}
	}

	public float BF_JYrate_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_JYrate_Base + BF_JYrate_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_JYrate(Xi, base.IndexName);
			}
			return BF_JYrate_Base;
		}
	}

	public float BF_GeDang_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_GeDang_Base + BF_GeDang_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_GeDang(Xi, base.IndexName);
			}
			return BF_GeDang_Base;
		}
	}

	public float BF_AttackSpeed_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_AttackSpeed_Base + BF_AttackSpeed_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_AttackSpeed(Xi, base.IndexName);
			}
			return BF_AttackSpeed_Base;
		}
	}

	public float BF_MoveSpeed_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_MoveSpeed_Base + BF_MoveSpeed_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_MoveSpeed(Xi, base.IndexName);
			}
			return BF_MoveSpeed_Base;
		}
	}

	public float BF_DamageAnti_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_DamageAnti_Base + BF_DamageAnti_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_DamageCut(Xi, base.IndexName);
			}
			return BF_DamageAnti_Base;
		}
	}

	public float BF_Health_Prc_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return BF_Health_Prc_Base + BF_Health_Prc_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetBF_Health_Prc(Xi, base.IndexName);
			}
			return BF_Health_Prc_Base;
		}
	}

	public float C_Damage_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return C_Damage_Base + C_Damage_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetCompDamage(Xi, base.IndexName);
			}
			return C_Damage_Base;
		}
	}

	public float C_ATspeed_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return C_ATspeed_Base + C_ATspeed_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetCompAttackSpeed(Xi, base.IndexName);
			}
			return C_ATspeed_Base;
		}
	}

	public float C_MVspeed_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return C_MVspeed_Base + C_MVspeed_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetCompMoveSpeed(Xi, base.IndexName);
			}
			return C_MVspeed_Base;
		}
	}

	public float C_Health_Prc_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return C_Health_Prc_Base + C_Health_Prc_Level * (float)(base.Level_Base_Last - 1) + SingletonMonoScope<TalentManager>.Instance.GetCompHealth_Prc(Xi, base.IndexName);
			}
			return C_Health_Prc_Base;
		}
	}

	public float CF_Rate => SingletonMonoScope<TalentManager>.Instance.GetCF_Rate(Xi, base.IndexName);

	public float BSAT_Damage => SingletonMonoScope<TalentManager>.Instance.GetBSAT_Damage(Xi, base.IndexName);

	public float BuffTime_Last => BuffTime_Base + SingletonMonoScope<TalentManager>.Instance.GetBuffTime(Xi, base.IndexName);

	public int ZD_F_Last
	{
		get
		{
			if (FS_ZD_F == 100000)
			{
				return ZD_F;
			}
			return FS_ZD_F;
		}
	}

	public int Type_F_Last
	{
		get
		{
			if (FS_Type_F != 100000)
			{
				return FS_Type_F;
			}
			return Type_F;
		}
	}

	public int ZD_S_Last
	{
		get
		{
			if (FS_ZD_S != 100000)
			{
				return FS_ZD_S;
			}
			return ZD_S;
		}
	}

	public int Dic_F_Last
	{
		get
		{
			if (FS_Dic_F != 100000)
			{
				return FS_Dic_F;
			}
			return Dic_F;
		}
	}

	public int Count_ATtarget_Last => ApplyFSCount(Count_ATtarget_Base + SingletonMonoScope<TalentManager>.Instance.GetCount_AtTarget(Xi, base.IndexName) + CT_AT, FS_CT_AT);

	public int CF_Count_Last => ApplyFSCount(CF_Count, FS_CT_F);

	public int Count_F_Last => ApplyFSCount(Count_F_Base + SingletonMonoScope<TalentManager>.Instance.GetCount_father(Xi, base.IndexName) + CT_F, FS_CT_F);

	public int Count_S_Last => ApplyFSCount(Count_S_Base + SingletonMonoScope<TalentManager>.Instance.GetCount_son(Xi, base.IndexName) + CT_S, FS_CT_S);

	public int Count_AB_Last => ApplyFSCount(Count_AB + CT_S, FS_CT_S);

	public int CountMulti_Last => ApplyFSCount(CountMulti + CT_Mul, FS_CT_Multi);

	public int TypeDIC_F_Last
	{
		get
		{
			if (FS_Type_Dic_F != 100000)
			{
				return FS_Type_Dic_F;
			}
			return TypeDIC_F;
		}
	}

	public float AngleA_Last => AngleA * (float)FS_AngleA_Last / 100f;

	public float Range1_Last => Range1 * (float)FS_Range1_Last / 100f;

	public float FStime1_Last => FStime1 * (float)FS_Time1_Last / 100f;

	public float FStime2_Last => FStime2 * (float)FS_Time2_Last / 100f;

	private float FS_DMG_Last
	{
		get
		{
			if (FS_DMG != 0f)
			{
				return FS_DMG;
			}
			return 100f;
		}
	}

	private int FS_Time1_Last
	{
		get
		{
			if (FS_Time1 != 0)
			{
				return FS_Time1;
			}
			return 100;
		}
	}

	private int FS_Time2_Last
	{
		get
		{
			if (FS_Time2 != 0)
			{
				return FS_Time2;
			}
			return 100;
		}
	}

	private int FS_Range1_Last
	{
		get
		{
			if (FS_Range1 != 0)
			{
				return FS_Range1;
			}
			return 100;
		}
	}

	private int FS_AngleA_Last
	{
		get
		{
			if (FS_AngleA != 0)
			{
				return FS_AngleA;
			}
			return 100;
		}
	}

	public int DoubleDMG
	{
		get
		{
			if (Double)
			{
				return 2;
			}
			return 1;
		}
	}

	private int ApplyFSCount(int value, int fsCount)
	{
		switch (fsCount)
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
			return value * (fsCount + 1);
		default:
			return value;
		}
	}

	public override string GetInfoA()
	{
		string empty = string.Empty;
		empty += LOC.MM.GetSkill(Info);
		empty += "\n";
		empty = ((base.Level_Base <= 0) ? (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Next Level"), base.Level_Base + 1)) : (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Current Level"), base.Level_Base_Last)));
		switch (Father_type)
		{
		case 0:
		{
			float number = Damage_Max / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType);
			empty += $"{Damage_Max}% ({DamgeTextManager.FormatDamageNumber(number)}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			if (!AttackType)
			{
				empty += LOC.MM.GetMain("/S");
			}
			break;
		}
		case 1:
			empty += string.Format(" + {0}% {1}", BF_Damage_Last, LOC.MM.GetMain("damage"));
			break;
		case 2:
			empty += $" + {BF_EL_Damage_Last}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 3:
			empty += $" + {BF_EL_Chuan_Last}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 4:
			empty += string.Format(" + {0}% {1}", BF_BJrate_Last, LOC.MM.GetMain("BJrate"));
			break;
		case 5:
			empty += string.Format(" + {0}% {1}", BF_JYrate_Last, LOC.MM.GetMain("JYrate"));
			break;
		case 6:
			empty += string.Format(" + {0}% {1}", BF_GeDang_Last, LOC.MM.GetMain("GeDang"));
			break;
		case 7:
			empty += string.Format(" + {0}% {1}", BF_AttackSpeed_Last, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 8:
			empty += string.Format(" + {0}% {1}", BF_MoveSpeed_Last, LOC.MM.GetMain("MoveSpeed"));
			break;
		case 9:
			empty += string.Format(" + {0}% {1}", BF_DamageAnti_Last, LOC.MM.GetMain("DamageAnti"));
			break;
		case 10:
			empty += string.Format(" + {0}% {1}", BF_Health_Prc_Last, LOC.MM.GetMain("HealthPrc"));
			break;
		case 11:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp damage"), C_Damage_Last);
			break;
		case 12:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp AttackSpeed"), C_ATspeed_Last);
			break;
		case 13:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp MoveSpeed"), C_MVspeed_Last);
			break;
		case 14:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp HealthPrc"), C_Health_Prc_Last);
			break;
		}
		if (Reborn > 0)
		{
			empty += string.Format("\n{0} <color=#CDFF45>{1}%</color>", LOC.MM.GetMain("Reborn"), Reborn);
		}
		return empty;
	}

	public override string GetInfoB()
	{
		string empty = string.Empty;
		empty += string.Format("<color=#FFE397>{0}：{1}</color>\n", LOC.MM.GetMain("Next Level"), base.Level_Base_Last + 1);
		switch (Father_type)
		{
		case 0:
			empty += $"{Damage_Max + Damage_Level}% ({Mathf.Floor((Damage_Max + Damage_Level) / 100f * SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType))}) {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			if (!AttackType)
			{
				empty += LOC.MM.GetMain("/S");
			}
			break;
		case 1:
			empty += string.Format(" + {0}% {1}", BF_Damage_Last + BF_Damage_Level, LOC.MM.GetMain("damage"));
			break;
		case 2:
			empty += $" + {BF_EL_Damage_Last + BF_EL_Damage_Level}% {LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			break;
		case 3:
			empty += $" + {BF_EL_Chuan_Base + BF_EL_Chuan_Level}% {LOC.MM.GetMain(SWS.El_Chuan(damageType))}";
			break;
		case 4:
			empty += string.Format(" + {0}% {1}", BF_BJrate_Last + BF_BJrate_Level, LOC.MM.GetMain("BJrate"));
			break;
		case 5:
			empty += string.Format(" + {0}% {1}", BF_JYrate_Last + BF_JYrate_Level, LOC.MM.GetMain("JYrate"));
			break;
		case 6:
			empty += string.Format(" + {0}% {1}", BF_GeDang_Last + BF_GeDang_Level, LOC.MM.GetMain("GeDang"));
			break;
		case 7:
			empty += string.Format(" + {0}% {1}", BF_AttackSpeed_Last + BF_AttackSpeed_Level, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 8:
			empty += string.Format(" + {0}% {1}", BF_MoveSpeed_Last + BF_MoveSpeed_Level, LOC.MM.GetMain("MoveSpeed"));
			break;
		case 9:
			empty += string.Format(" + {0}% {1}", BF_DamageAnti_Last + BF_DamageAnti_Level, LOC.MM.GetMain("DamageAnti"));
			break;
		case 10:
			empty += string.Format(" + {0}% {1}", BF_Health_Prc_Last + BF_Health_Prc_Level, LOC.MM.GetMain("HealthPrc"));
			break;
		case 11:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp damage"), C_Damage_Last + C_Damage_Level);
			break;
		case 12:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp AttackSpeed"), C_ATspeed_Last + C_ATspeed_Level);
			break;
		case 13:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp MoveSpeed"), C_MVspeed_Last + C_MVspeed_Level);
			break;
		case 14:
			empty += string.Format("{0} + {1}%", LOC.MM.GetMain("Comp HealthPrc"), C_Health_Prc_Last + C_Health_Prc_Level);
			break;
		}
		if (Reborn > 0)
		{
			empty += string.Format("\n{0} <color=#CDFF45>{1}%</color>", LOC.MM.GetMain("Reborn"), Reborn);
		}
		return empty;
	}
}
