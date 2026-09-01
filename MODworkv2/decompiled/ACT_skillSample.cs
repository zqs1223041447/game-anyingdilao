using System;

[Serializable]
public class ACT_skillSample
{
	[NonSerialized]
	public string SkillName;

	public int FStype;

	public int LockType;

	public int OBJ;

	public int OBJ_Group;

	public int RTtypeOBJ;

	public int RTtypeFX;

	public float Distance;

	public float CoolDown;

	public DamageType damageType;

	public int MainEL;

	public int ThroughType;

	public bool AttackType;

	public bool AttackTypeA;

	public bool AttackTypeB;

	public float Damage;

	public float DamageA;

	public float DamageB;

	public float BJrate;

	public float BJDamage;

	public float JYrate;

	public float Through;

	public float FlySpeed;

	public float MoveSpeedCut;

	public float AttackSpeedCut;

	public float AntiCut;

	public float BF_Damage;

	public float BF_EL_Damage;

	public float BF_EL_Chuan;

	public float BF_BJrate;

	public float BF_JYrate;

	public float BF_GeDang;

	public float BF_AttackSpeed;

	public float BF_MoveSpeed;

	public float BF_DamageAnti;

	public float BF_Health_Prc;

	public float C_Damage;

	public float C_ATspeed;

	public float C_MVspeed;

	public float C_Health_Prc;

	public float CF_Rate;

	public string BSAT;

	public int BSAT_Count;

	public int BSAT_Angle;

	public int Is_BS;

	public int ChangeSkin;

	public int SkinIndex;

	public int Reborn;

	public float BSAT_Damage;

	public int BSAT_DMG;

	public bool AutoUse;

	public int Refresh;

	public int CompUP_DMG;

	public int ATtarUP;

	public int MS_Dead;

	public int GD_Use;

	public string JCskill;

	public string[] LinkSK;

	public bool LinkAll;

	public bool EveryLink;

	public bool LastSkill;

	public bool DashSkill;

	public bool TPSkill;

	public int UseDMG;

	public int UseATS;

	public int UseMVS;

	public int[] UseDMG_EL = new int[6];

	public int[] UseChuan = new int[6];

	public int UseCP_DMG;

	public int UseCP_ATS;

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

	public float WD;

	public int Crit_Time;

	public int Crit_CD;

	public int Over_Prc;

	public int CutSpeedZone;

	public int NoTime;

	public float BuffTime;

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

	public int Count_ATtarget;

	public int ATtar_DMG;

	public int CF_Count;

	public int Count_F;

	public int Count_S;

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

	public void EnsureRuntimeBuffDefaults()
	{
		if (UseDMG_EL == null || UseDMG_EL.Length < 6)
		{
			UseDMG_EL = new int[6];
		}
		if (UseChuan == null || UseChuan.Length < 6)
		{
			UseChuan = new int[6];
		}
	}

	public bool HasAnyRuntimePresenceBuff()
	{
		if (Has_DMG == 0 && Has_ATS == 0 && Has_MVS == 0 && Has_BJR == 0 && Has_BJD == 0 && Has_DotTimeCut == 0 && Has_DMG_Cut == 0 && Has_GD == 0 && Has_ORB_DMG == 0 && Has_XJ_DMG == 0 && Has_Dot_DMG == 0)
		{
			return Has_CP_DMG != 0;
		}
		return true;
	}
}
