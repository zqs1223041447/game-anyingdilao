using System.Collections.Generic;
using UnityEngine;

public class EnemyMB
{
	public int GlobalID;

	public List<string> IndexName = new List<string>();

	public int IndexA;

	public int IndexB;

	[HideInInspector]
	public float Xp;

	[HideInInspector]
	public int size;

	[HideInInspector]
	public float CompOffset;

	[HideInInspector]
	public float TuiSpeed;

	[HideInInspector]
	public float ItemDropPos;

	[HideInInspector]
	public int SpineType;

	[HideInInspector]
	public int ColorIndex;

	[HideInInspector]
	public int CP_FX;

	[HideInInspector]
	public int EnemyType;

	[HideInInspector]
	public float Health;

	[HideInInspector]
	public float AttackSpeed_JG;

	[HideInInspector]
	public float ATSpeed;

	[HideInInspector]
	public float MVSpeed;

	[HideInInspector]
	public float Damage;

	[HideInInspector]
	public float Range_Base;

	[HideInInspector]
	public float Range_Anger;

	[HideInInspector]
	public float Range_Far;

	[HideInInspector]
	public int SK_Rate;

	[HideInInspector]
	public int SPtype;

	[HideInInspector]
	public int Die_Index;

	[HideInInspector]
	public int DieType;

	[HideInInspector]
	public int DiePos;

	[HideInInspector]
	public float DieFX_TimeDelay;

	[HideInInspector]
	public float DieDelay;

	[HideInInspector]
	public int Lie_Index;

	[HideInInspector]
	public int LiePos;

	[HideInInspector]
	public int FSDie_Index;

	[HideInInspector]
	public float Idle_Time_Min;

	[HideInInspector]
	public float Idle_Time_Max;

	[HideInInspector]
	public int SO_IdleRate;

	[HideInInspector]
	public int SO_AttackRate;

	[HideInInspector]
	public int SO_SayRate;

	[HideInInspector]
	public int SO_HurtRate;

	[HideInInspector]
	public int SO_DieRate;

	[HideInInspector]
	public string SO_Idle;

	[HideInInspector]
	public string SO_Walk;

	[HideInInspector]
	public string SO_AttackA;

	[HideInInspector]
	public string SO_SayA;

	[HideInInspector]
	public string SO_AttackB;

	[HideInInspector]
	public string SO_SayB;

	[HideInInspector]
	public string SO_AttackC;

	[HideInInspector]
	public string SO_SayC;

	[HideInInspector]
	public string SO_Hurt;

	[HideInInspector]
	public string SO_Die;

	[HideInInspector]
	public string SO_ChuiDi;

	public int CF_Rate;

	public int HitFX;

	public float AT_Distans;

	public int AT1;

	public int AT2;

	public int AT3;

	public int AT_Ani;

	public bool AT_Fang;

	public float SK_Distans;

	public int SK1;

	public int SK2;

	public int SK3;

	public int SK4;

	public int SK5;

	public int SK_Ani;

	public bool SK_Fang;

	public EM_Skill_CP SK_Comp;

	public EM_Skill_FS SK_FS;

	public int SK_Die_Index;

	public float ELSS_Distans;

	public int ELSS_Index;

	public int ELSS_Ani;

	public bool ELSS_Fang;
}
