using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class ACT_SPC
{
	public int GlobalID;

	public int SPCindex = -1;

	public int SPCSlotIndex = -1;

	public int SPCItemCharType = -1;

	public int SpecialType;

	public string Name;

	public int FStype;

	public int LockType;

	public string info;

	public int Price;

	public string skillName;

	public string ZQName;

	public int RTtypeOBJ;

	public float Distance;

	public float SPC_PRC;

	public float Rate;

	public float Damage;

	public DamageType damageType;

	public int MainEL;

	public int ThroughType;

	public bool AttackType;

	public bool AttackTypeA;

	public bool AttackTypeB;

	public float DamageA;

	public float DamageB;

	public int NoTime;

	public float BuffTime;

	public float DebuffTime;

	public float Field_time;

	public float ORB_time;

	public float EXP_time;

	public float ZD_time_F;

	public float ZD_time_S;

	public int OBJ;

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

	public int Count_A;

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

	public float RateLast => Mathf.FloorToInt(Rate + Rate * (float)SingletonMonoScope<PlayerManager>.Instance.WPSPC_Rate / 100f);

	public float DamageLast => Damage + Damage * (float)SingletonMonoScope<PlayerManager>.Instance.WPSPC_DMG / 100f;
}
