using UnityEngine;

public class SkillOBJ_DT_CP : MonoBehaviour
{
	[HideInInspector]
	public int indexType;

	[HideInInspector]
	public PlayerManager pl;

	[HideInInspector]
	public Companion cp;

	[HideInInspector]
	public Enemy em;

	[HideInInspector]
	public bool ZY;

	[HideInInspector]
	public string skillName;

	[HideInInspector]
	public float Distance;

	[HideInInspector]
	public float Damage;

	[HideInInspector]
	public float Health;

	[HideInInspector]
	public float Health_Prc;

	[HideInInspector]
	public float AttackSpeed;

	[HideInInspector]
	public float GeDang;

	[HideInInspector]
	public DamageType damageType;

	[HideInInspector]
	public DamageType damageType_Change;

	[HideInInspector]
	public float Change_AT;

	[HideInInspector]
	public float ATSrate;

	[HideInInspector]
	public DamageType ChangeEL_SK;

	[HideInInspector]
	public float ATS_Damage;

	[HideInInspector]
	public DamageType ChangeEL_AR;

	[HideInInspector]
	public float ARS_Damage;

	[HideInInspector]
	public int DotMultiA;

	[HideInInspector]
	public int DotMultiB;

	[HideInInspector]
	public int GD_R_Heal;

	[HideInInspector]
	public int BloodDie;

	[HideInInspector]
	public int TGYJ;

	[HideInInspector]
	public int Kill_R_Heal;

	[HideInInspector]
	public int Hurt_FT;

	[HideInInspector]
	public int AT_DotLayer;

	[HideInInspector]
	public bool BJ_NoDot;

	[HideInInspector]
	public bool WS_All;

	[HideInInspector]
	public int Field_Range;

	[HideInInspector]
	public float DisA;

	[HideInInspector]
	public float DisB;

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
	public int Type_A;

	[HideInInspector]
	public int Type_B;

	[HideInInspector]
	public int TypeDIC_A;

	[HideInInspector]
	public int TypeDIC_B;

	[HideInInspector]
	public float JG_A;

	[HideInInspector]
	public float JG_B;

	[HideInInspector]
	public float AngleA;

	[HideInInspector]
	public float AngleB;

	[HideInInspector]
	public float FStimeA;

	[HideInInspector]
	public float FStimeB;

	[HideInInspector]
	public int Count_A;

	[HideInInspector]
	public int Count_B;

	[HideInInspector]
	public bool AT_Double;

	[HideInInspector]
	public int Count_ATtarget_A;

	[HideInInspector]
	public int Count_ATtarget_B;

	[HideInInspector]
	public int CountMulti_A;

	[HideInInspector]
	public int CountMulti_B;

	[HideInInspector]
	public int Follow_A;

	[HideInInspector]
	public int Follow_B;

	[HideInInspector]
	public int AllChuan_A;

	[HideInInspector]
	public int AllChuan_B;

	[HideInInspector]
	public int RDSpeed_A;

	[HideInInspector]
	public int RDSpeed_B;

	[HideInInspector]
	public int HasFX_A;

	[HideInInspector]
	public int HasFX_B;

	[HideInInspector]
	public int colEXP_A;

	[HideInInspector]
	public int colEXP_B;

	[HideInInspector]
	public int EXPpos_A;

	[HideInInspector]
	public int EXPpos_B;

	[HideInInspector]
	public float BStype;

	[HideInInspector]
	public int AT_ZD;

	[HideInInspector]
	public int SK_ZD;

	[HideInInspector]
	public int AT_DMG = 100;

	[HideInInspector]
	public int SK_DMG = 100;

	public SkillOBJ_DT_CP CloneRuntime()
	{
		return Object.Instantiate(this);
	}
}
