using System;

[Serializable]
public class ACT_DOT
{
	public bool Opened;

	public DamageType damageType;

	public int Layer_Max;

	public float DOTrate;

	public float Damage;

	public float lifeTime;

	public float ATSpeedCut;

	public float MVSpeedCut;

	public float ELAntiCut;

	public float YunCut;

	public float DamageLow;

	public float MSnumber;

	public float MSrate;

	public float BoomDie_Rate;

	public float BoomDie_Damage;

	public int BoomDie_OBJ;

	public int BoomDie_Pos;

	public bool AttackType_BD;

	public int Type_BD;

	public int TypeDIC_BD;

	public int TypeEXP_BD;

	public float Range_BD;

	public float SpeedMin_BD;

	public float SpeedMax_BD;

	public int Count_BD;

	public int CountMulti_BD;

	public float BuffTime_BD;

	public float ZD_time_BD;

	public int ZD_BD;

	public int EXP_BD;

	public int Dic_BD;

	public float BoomJump_Rate;

	public float BoomJump_Damage;

	public int BoomJump_OBJ;

	public int BoomJump_Pos;

	public bool AttackType_BJ;

	public int Type_BJ;

	public int TypeDIC_BJ;

	public int TypeEXP_BJ;

	public float Range_BJ;

	public float SpeedMin_BJ;

	public float SpeedMax_BJ;

	public int Count_BJ;

	public int CountMulti_BJ;

	public float BuffTime_BJ;

	public float ZD_time_BJ;

	public int ZD_BJ;

	public int EXP_BJ;

	public int Dic_BJ;

	public float CutJump_Rate;

	public float CutJump_Damage;

	public int CutJump_OBJ;

	public int CutJump_Pos;

	public float FrozenJump_Rate;

	public float FrozenJump_Time;

	public static ACT_DOT CreateDefault(DamageType damageType)
	{
		return new ACT_DOT
		{
			damageType = damageType
		};
	}
}
