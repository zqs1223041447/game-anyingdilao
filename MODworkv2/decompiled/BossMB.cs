using System.Collections.Generic;
using UnityEngine;

public class BossMB
{
	public int GlobalID;

	public List<string> IndexName = new List<string>();

	public int IndexA;

	public int IndexB;

	[HideInInspector]
	public int Quality;

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
	public int BossType;

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

	public float Range_Base;

	public float Range_Anger;

	public float Range_Far;

	public float Range_ATplayer_multi;

	[HideInInspector]
	public int SK_Rate;

	[HideInInspector]
	public int SK_Rate_Comp;

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
	public string SO_Walk;

	[HideInInspector]
	public string SO_Hurt;

	[HideInInspector]
	public string SO_Die;

	[HideInInspector]
	public List<string> SO_Idle = new List<string>();

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
	public string SO_AttackD;

	[HideInInspector]
	public string SO_SayD;

	[HideInInspector]
	public string SO_AttackE;

	[HideInInspector]
	public string SO_SayE;

	[HideInInspector]
	public string SO_ChongStart;

	[HideInInspector]
	public string SO_ChongEnd;

	[HideInInspector]
	public string SO_Jump;

	[HideInInspector]
	public string SO_Land;

	[HideInInspector]
	public string SO_SPC1;

	[HideInInspector]
	public string SO_SPC2;

	[HideInInspector]
	public string SO_SPC3;

	public List<EM_Skill_SP> AT = new List<EM_Skill_SP>();

	public List<EM_Skill_SP> SK = new List<EM_Skill_SP>();

	public EM_Skill_CP SKC = new EM_Skill_CP();

	public int SK_Die_Index;
}
