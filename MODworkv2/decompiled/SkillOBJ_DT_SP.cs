using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class SkillOBJ_DT_SP : MonoBehaviour
{
	[HideInInspector]
	public int indexType;

	[HideInInspector]
	public int FStype;

	[HideInInspector]
	public int LockType;

	[HideInInspector]
	public PlayerManager pl;

	[HideInInspector]
	public Companion cp;

	private Enemy emOwner;

	private readonly HashSet<Enemy> skillMSDeadTargets = new HashSet<Enemy>();

	[HideInInspector]
	public bool ZY;

	[HideInInspector]
	public bool DotZZ;

	[HideInInspector]
	public Vector3 TargetPos;

	[HideInInspector]
	public string skillName;

	[HideInInspector]
	public string ZQName;

	[HideInInspector]
	public Vector2 dic;

	[HideInInspector]
	public int RTtypeOBJ;

	[HideInInspector]
	public int RTtypeFX;

	[HideInInspector]
	public float Distance;

	[HideInInspector]
	public int GlobalID;

	[HideInInspector]
	public int SPCindex = -1;

	[HideInInspector]
	public int SPCSlotIndex = -1;

	[HideInInspector]
	public int SPCItemCharType = -1;

	[HideInInspector]
	public int SpecialType;

	[HideInInspector]
	public DamageType damageType;

	[HideInInspector]
	public int MainEL;

	[HideInInspector]
	public int ThroughType;

	[HideInInspector]
	public bool AttackType;

	[HideInInspector]
	public bool AttackTypeA;

	[HideInInspector]
	public bool AttackTypeB;

	[HideInInspector]
	public float Damage;

	[HideInInspector]
	public float DamageA;

	[HideInInspector]
	public float DamageB;

	[HideInInspector]
	public bool TrapDamageBonusApplied;

	[HideInInspector]
	public float SPC_Damage;

	[HideInInspector]
	public float SPC_DamageA;

	[HideInInspector]
	public float SPC_DamageB;

	[HideInInspector]
	public float BJrate;

	[HideInInspector]
	public float BJDamage;

	[HideInInspector]
	public float JYrate;

	[HideInInspector]
	public float Through;

	[HideInInspector]
	public float FlySpeed;

	[HideInInspector]
	public float MoveSpeedCut;

	[HideInInspector]
	public float AttackSpeedCut;

	[HideInInspector]
	public float AntiCut;

	[HideInInspector]
	public float BF_Damage;

	[HideInInspector]
	public float BF_EL_Damage;

	[HideInInspector]
	public float BF_EL_Chuan;

	[HideInInspector]
	public float BF_BJrate;

	[HideInInspector]
	public float BF_JYrate;

	[HideInInspector]
	public float BF_GeDang;

	[HideInInspector]
	public float BF_AttackSpeed;

	[HideInInspector]
	public float BF_MoveSpeed;

	[HideInInspector]
	public float BF_DamageAnti;

	[HideInInspector]
	public float BF_Health_Prc;

	[HideInInspector]
	public float C_Damage;

	[HideInInspector]
	public float C_ATspeed;

	[HideInInspector]
	public float C_MVspeed;

	[HideInInspector]
	public float C_Health_Prc;

	[HideInInspector]
	public float BF_Through;

	[HideInInspector]
	public float CF_Rate;

	[HideInInspector]
	public string BSAT;

	[HideInInspector]
	public int BSAT_Count;

	[HideInInspector]
	public int BSAT_Angle;

	[HideInInspector]
	public int Is_BS;

	[HideInInspector]
	public int ChangeSkin;

	[HideInInspector]
	public int SkinIndex;

	[HideInInspector]
	public int Reborn;

	[HideInInspector]
	public float BSAT_Damage;

	[HideInInspector]
	public int BSAT_DMG;

	[HideInInspector]
	public bool AutoUse;

	[HideInInspector]
	public int Refresh;

	[HideInInspector]
	public int CompUP_DMG;

	[HideInInspector]
	public int ATtarUP;

	[HideInInspector]
	public int MS_Dead;

	[HideInInspector]
	public int GD_Use;

	[HideInInspector]
	public string JCskill;

	[HideInInspector]
	public string[] LinkSK;

	[HideInInspector]
	public bool LinkAll;

	[HideInInspector]
	public bool EveryLink;

	[HideInInspector]
	public bool LastSkill;

	[HideInInspector]
	public bool DashSkill;

	[HideInInspector]
	public bool TPSkill;

	[HideInInspector]
	public int UseDMG;

	[HideInInspector]
	public int UseATS;

	[HideInInspector]
	public int UseMVS;

	[HideInInspector]
	public int[] UseDMG_EL = new int[6];

	[HideInInspector]
	public int[] UseChuan = new int[6];

	[HideInInspector]
	public int UseCP_DMG;

	[HideInInspector]
	public int UseCP_ATS;

	[HideInInspector]
	public int Has_DMG;

	[HideInInspector]
	public int Has_ATS;

	[HideInInspector]
	public int Has_MVS;

	[HideInInspector]
	public int Has_BJR;

	[HideInInspector]
	public int Has_BJD;

	[HideInInspector]
	public int Has_DotTimeCut;

	[HideInInspector]
	public int Has_DMG_Cut;

	[HideInInspector]
	public int Has_GD;

	[HideInInspector]
	public int Has_ORB_DMG;

	[HideInInspector]
	public int Has_XJ_DMG;

	[HideInInspector]
	public int Has_Dot_DMG;

	[HideInInspector]
	public int Has_CP_DMG;

	[HideInInspector]
	public float WD;

	[HideInInspector]
	public int Crit_Time;

	[HideInInspector]
	public int Crit_CD;

	[HideInInspector]
	public int Over_Prc;

	[HideInInspector]
	public float Chuan;

	[HideInInspector]
	public float DotRate;

	[HideInInspector]
	public float DotDamage;

	[HideInInspector]
	public int AT_DotLayer;

	[HideInInspector]
	public bool BJ_NoDot;

	[HideInInspector]
	public bool WS_All;

	[HideInInspector]
	public int Field_Range;

	[HideInInspector]
	public int NoTime;

	[HideInInspector]
	public float BuffTime;

	[HideInInspector]
	public float DebuffTime;

	[HideInInspector]
	public float Field_time;

	[HideInInspector]
	public float ORB_time;

	[HideInInspector]
	public float EXP_time;

	[HideInInspector]
	public float ZD_time_F;

	[HideInInspector]
	public float ZD_time_S;

	[HideInInspector]
	public int Layer_SubA;

	[HideInInspector]
	public int Layer_SubB;

	[HideInInspector]
	public int ORB;

	[HideInInspector]
	public int ZD_F;

	[HideInInspector]
	public int ZD_S;

	[HideInInspector]
	public int ZD_AB;

	[HideInInspector]
	public int EXP_F;

	[HideInInspector]
	public int EXP_S;

	[HideInInspector]
	public int EXP_AB;

	[HideInInspector]
	public int Dic_F;

	[HideInInspector]
	public int Dic_S;

	[HideInInspector]
	public int FX_F;

	[HideInInspector]
	public int FX_S;

	[HideInInspector]
	public int Sound;

	[HideInInspector]
	public int Count_ORB;

	[HideInInspector]
	public int Count_ATtarget;

	[HideInInspector]
	public int ATtar_DMG;

	[HideInInspector]
	public int CF_Count;

	[HideInInspector]
	public int Count_F;

	[HideInInspector]
	public int Count_S;

	[HideInInspector]
	public int Count_AB;

	[HideInInspector]
	public int CountMulti;

	[HideInInspector]
	public int CountEXP;

	[HideInInspector]
	public int TypeORB;

	[HideInInspector]
	public int CF_Type;

	[HideInInspector]
	public int Type_F;

	[HideInInspector]
	public int Type_S;

	[HideInInspector]
	public int Type_AB;

	[HideInInspector]
	public int TypeDIC_F;

	[HideInInspector]
	public int TypeDIC_S;

	[HideInInspector]
	public int TypeEXP_F;

	[HideInInspector]
	public int TypeEXP_S;

	[HideInInspector]
	public int TypeEXP_AB;

	[HideInInspector]
	public float Size;

	[HideInInspector]
	public float High;

	[HideInInspector]
	public float JG;

	[HideInInspector]
	public float AngleA;

	[HideInInspector]
	public float AngleB;

	[HideInInspector]
	public float Range1;

	[HideInInspector]
	public float Range2;

	[HideInInspector]
	public float Range_AT;

	[HideInInspector]
	public float FStime1;

	[HideInInspector]
	public float FStime2;

	[HideInInspector]
	public float Speed1;

	[HideInInspector]
	public float Speed2;

	[HideInInspector]
	public float Speed3;

	[HideInInspector]
	public float Speed4;

	[HideInInspector]
	public int Follow_F;

	[HideInInspector]
	public int Follow_S;

	[HideInInspector]
	public int AllChuan_F;

	[HideInInspector]
	public int AllChuan_S;

	[HideInInspector]
	public int Slow_F;

	[HideInInspector]
	public int Slow_S;

	[HideInInspector]
	public int RDSpeed_F;

	[HideInInspector]
	public int RDSpeed_S;

	[HideInInspector]
	public int HasFX;

	[HideInInspector]
	public int S_HasFX;

	[HideInInspector]
	public int AB_HasFX;

	[HideInInspector]
	public int colEXP;

	[HideInInspector]
	public int colEXP_A;

	[HideInInspector]
	public int S_colEXP;

	[HideInInspector]
	public int AB_colEXP;

	[HideInInspector]
	public int TimeEXP;

	[HideInInspector]
	public int TimeEXP_AB;

	[HideInInspector]
	public int LastEXP;

	[HideInInspector]
	public int LastEXP_AB;

	[HideInInspector]
	public int S_LastEXP;

	[HideInInspector]
	public int AB_LastEXP;

	[HideInInspector]
	public int EXPpos;

	[HideInInspector]
	public int EXPpos_AB;

	[HideInInspector]
	public int S_EXPpos;

	[HideInInspector]
	public int AB_EXPpos;

	[HideInInspector]
	public int AngleEXP;

	[HideInInspector]
	public int AngleEXP_AB;

	[HideInInspector]
	public int CutSpeedZone;

	[HideInInspector]
	public bool Dot_Infect;

	[HideInInspector]
	public int Dot_Infect_Layer;

	public Enemy em
	{
		get
		{
			return emOwner;
		}
		set
		{
			emOwner = value;
			CaptureDotZZ();
		}
	}

	private void OnEnable()
	{
		emOwner = null;
		skillMSDeadTargets.Clear();
		DotZZ = false;
		ATtar_DMG = 0;
		AT_DotLayer = 0;
		BJ_NoDot = false;
		WS_All = false;
		Field_Range = 0;
		LastSkill = false;
		DashSkill = false;
		TPSkill = false;
		UseDMG = 0;
		UseATS = 0;
		UseMVS = 0;
		ClearRuntimeBuffArray(UseDMG_EL);
		ClearRuntimeBuffArray(UseChuan);
		UseCP_DMG = 0;
		UseCP_ATS = 0;
		Has_DMG = 0;
		Has_ATS = 0;
		Has_MVS = 0;
		Has_BJR = 0;
		Has_BJD = 0;
		Has_DotTimeCut = 0;
		Has_DMG_Cut = 0;
		Has_GD = 0;
		Has_ORB_DMG = 0;
		Has_XJ_DMG = 0;
		Has_Dot_DMG = 0;
		Has_CP_DMG = 0;
		WD = 0f;
		Crit_Time = 0;
		Crit_CD = 0;
		Over_Prc = 0;
		LockType = 0;
		ZQName = null;
		SPCindex = -1;
		SPCSlotIndex = -1;
		SPCItemCharType = -1;
		Dot_Infect = false;
		Dot_Infect_Layer = 0;
		TrapDamageBonusApplied = false;
	}

	public bool TryClaimMSDeadTarget(Enemy target)
	{
		if (target != null)
		{
			return skillMSDeadTargets.Add(target);
		}
		return false;
	}

	private static void ClearRuntimeBuffArray(int[] values)
	{
		if (values != null)
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = 0;
			}
		}
	}

	public void CaptureDotZZ()
	{
		DotZZ = emOwner != null && emOwner.peo != null && emOwner.peo.DotEM != null && emOwner.peo.DotEM.GerDotZZ();
	}

	public void ApplyTrapDamageBonusOnce(PlayerManager owner)
	{
		if (!TrapDamageBonusApplied && !(owner == null))
		{
			TrapDamageBonusApplied = true;
			float num = (float)owner.XJ_DMG + owner.Runtime_XJ_DMG_Tmp + (SingletonMonoScope<ACTbar>.HasInstance ? ((float)SingletonMonoScope<ACTbar>.Instance.GetEveryCompXJ_DMG()) : 0f) + owner.BE_ZQ_XJ_DMG * (float)owner.BE_ZQ_Count + owner.BE_SPC_XJ_DMG * (float)owner.BE_SPC_Count + owner.BE_HH_XJ_DMG * (float)owner.BE_HH_Count + owner.BE_SK_XJ_DMG * (float)owner.BE_SK_Count + owner.BE_BS_XJ_DMG * (float)owner.BE_BS_Count;
			Damage += Damage * num / 100f;
			DamageA += DamageA * num / 100f;
			DamageB += DamageB * num / 100f;
		}
	}
}
