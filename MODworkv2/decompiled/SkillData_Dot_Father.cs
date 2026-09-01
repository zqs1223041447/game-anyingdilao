using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

[Serializable]
public class SkillData_Dot_Father : SkillData
{
	public string SonA;

	public string SonB;

	public string SonC;

	public string SonD;

	public DamageType damageType;

	public int Layer_Base;

	public float DOTrate_Base;

	public float DOTrate_Level;

	public float Damage_base;

	public float Time_base;

	public float ATSpeedCut_Base;

	public float ATSpeedCut_Level;

	public float MVSpeedCut_Base;

	public float MVSpeedCut_Level;

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

	public int CutJump_OBJ;

	public int CutJump_Pos;

	public int Layer_Max
	{
		get
		{
			int num = Layer_Base + SingletonMonoScope<TalentManager>.Instance.GetLayer_Dot(Xi, base.IndexName);
			int num2 = (SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.AllDot_Layer : 0);
			int num3 = ((!SingletonMonoScope<PlayerManager>.HasInstance) ? 1 : SingletonMonoScope<PlayerManager>.Instance.GetPlayerDotData(damageType).Double_LayerLast);
			return (num + num * num2 / 100) * num3;
		}
	}

	public float DOTrate_Max
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return DOTrate_Base + DOTrate_Level * (float)(base.Level_Base_Last - 1);
			}
			return DOTrate_Base;
		}
	}

	public float Damage_Max => (Damage_base + SingletonMonoScope<TalentManager>.Instance.GetDamage_Dot(Xi, base.IndexName) + SingletonMonoScope<PlayerManager>.Instance.Class_CP_DotDMG * (float)SingletonMonoScope<ACTbar>.Instance.GetCPClass_CT() + (Damage_base + SingletonMonoScope<TalentManager>.Instance.GetDamage_Dot(Xi, base.IndexName) + SingletonMonoScope<PlayerManager>.Instance.Class_CP_DotDMG * (float)SingletonMonoScope<ACTbar>.Instance.GetCPClass_CT()) * (SingletonMonoScope<PlayerManager>.Instance.AllDot_DMG + SingletonMonoScope<PlayerManager>.Instance.BE_ZQ_Dot * (float)SingletonMonoScope<PlayerManager>.Instance.BE_ZQ_Count + SingletonMonoScope<PlayerManager>.Instance.BE_SPC_Dot * (float)SingletonMonoScope<PlayerManager>.Instance.BE_SPC_Count + SingletonMonoScope<PlayerManager>.Instance.BE_HH_Dot * (float)SingletonMonoScope<PlayerManager>.Instance.BE_HH_Count) / 100f) * SingletonMonoScope<PlayerManager>.Instance.GetA_Dot_DMG;

	public float Time_Max => Time_base + SingletonMonoScope<TalentManager>.Instance.GetTime_Dot(Xi, base.IndexName) + (Time_base + SingletonMonoScope<TalentManager>.Instance.GetTime_Dot(Xi, base.IndexName)) * SingletonMonoScope<PlayerManager>.Instance.AllDot_Time / 100f;

	public float ATSpeedCut_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return ATSpeedCut_Base + ATSpeedCut_Level * (float)(base.Level_Base_Last - 1);
			}
			return ATSpeedCut_Base;
		}
	}

	public float MVSpeedCut_Last
	{
		get
		{
			if (base.Level_Base > 0)
			{
				return MVSpeedCut_Base + MVSpeedCut_Level * (float)(base.Level_Base_Last - 1);
			}
			return MVSpeedCut_Base;
		}
	}

	public float ELAntiCut => SingletonMonoScope<TalentManager>.Instance.GetELAntiCut_Dot(Xi, base.IndexName);

	public float YunCut => SingletonMonoScope<TalentManager>.Instance.GetYunCut_Dot(Xi, base.IndexName);

	public float DamageLow => SingletonMonoScope<TalentManager>.Instance.GetDamageLow_Dot(Xi, base.IndexName);

	public float MSrate => SingletonMonoScope<TalentManager>.Instance.GetMSrate_Dot(Xi, base.IndexName);

	public float MSnumber => SingletonMonoScope<TalentManager>.Instance.GetMSnumber_Dot(Xi, base.IndexName);

	public float BoomDie_Rate => SingletonMonoScope<TalentManager>.Instance.GetBoomDie_Rate_Dot(Xi, base.IndexName);

	public float BoomDie_Damage => SingletonMonoScope<TalentManager>.Instance.GetBoomDie_Damage(Xi, base.IndexName);

	public float BoomJump_Rate => SingletonMonoScope<TalentManager>.Instance.GetBoomJump_Rate(Xi, base.IndexName);

	public float BoomJump_Damage => SingletonMonoScope<TalentManager>.Instance.GetBoomJump_Damage(Xi, base.IndexName);

	public float CutJump_Rate => SingletonMonoScope<TalentManager>.Instance.GetCutJump_Rate(Xi, base.IndexName);

	public float CutJump_Damage => SingletonMonoScope<TalentManager>.Instance.GetCutJump_Damage(Xi, base.IndexName);

	public float FrozenJump_Rate => SingletonMonoScope<TalentManager>.Instance.GetFrozenJump_Rate(Xi, base.IndexName);

	public float FrozenJump_Time => SingletonMonoScope<TalentManager>.Instance.GetFrozenJump_Time(Xi, base.IndexName);

	private string GetDotDamageText()
	{
		float num = (SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType) : 0f);
		return string.Format("{0}%({1}) {2}{3}", Damage_Max, Mathf.Floor(Damage_Max / 100f * num), LOC.MM.GetMain(SWS.Dot_DMG(damageType)), LOC.MM.GetMain("/S"));
	}

	private string GetBoomDamageText(float damagePercent)
	{
		float num = (SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.GiveDamage(damageType) : 0f);
		return string.Format("{0}%({1}) {2}", damagePercent, Mathf.Floor(damagePercent / 100f * num), LOC.MM.GetMain("damage"));
	}

	private void AppendExtraDotInfoA(ref string stats)
	{
		if (ELAntiCut > 0f)
		{
			stats += string.Format("\n{0} : -{1}% {2}", LOC.MM.GetMain("Per stack"), ELAntiCut, LOC.MM.GetMain(SWS.El_Anti(damageType)));
		}
		if (YunCut > 0f)
		{
			stats += string.Format("\n{0} : -{1}% {2}{3}", LOC.MM.GetMain("Per stack"), YunCut, LOC.MM.GetMain("enemy"), LOC.MM.GetMain("YunAnti"));
		}
		if (DamageLow > 0f)
		{
			stats += string.Format("\n{0} : -{1}% {2}{3}", LOC.MM.GetMain("Per stack"), DamageLow, LOC.MM.GetMain("enemy"), LOC.MM.GetMain("damage"));
		}
		if (CutJump_Rate > 0f || CutJump_Damage > 0f)
		{
			stats += string.Format("\n{0} : {1}%", LOC.MM.GetMain("Rate"), CutJump_Rate);
			stats += string.Format("\n-{0}% {1}", CutJump_Damage, LOC.MM.GetMain("HealthMax"));
		}
	}

	private void AppendExtraDotInfoB(ref string stats)
	{
		if (FrozenJump_Rate > 0f || FrozenJump_Time > 0f)
		{
			stats += string.Format("\n{0} {1} : {2}%", LOC.MM.GetMain("Freeze"), LOC.MM.GetMain("Rate"), FrozenJump_Rate);
			stats += string.Format("\n{0}{1} {2}{3}", FrozenJump_Time, LOC.MM.GetMain("S"), LOC.MM.GetMain("Freeze"), LOC.MM.GetMain("Duration"));
		}
		if (MSnumber > 0f || MSrate > 0f)
		{
			stats += string.Format("\n{0} : {1}%", LOC.MM.GetMain("MSrate"), MSrate);
			stats += string.Format("\n{0} : {1}%", LOC.MM.GetMain("MSnumber"), MSnumber);
		}
		if (BoomJump_Rate > 0f || BoomJump_Damage > 0f)
		{
			stats += string.Format("\n{0} : {1}% {2}", LOC.MM.GetMain("Rate"), BoomJump_Rate, LOC.MM.GetMain("DamageTickExplosion"));
			stats = stats + "\n" + GetBoomDamageText(BoomJump_Damage);
		}
		if (BoomDie_Rate > 0f || BoomDie_Damage > 0f)
		{
			stats += string.Format("\n{0} : {1}% {2}", LOC.MM.GetMain("Rate"), BoomDie_Rate, LOC.MM.GetMain("DeathExplosion"));
			stats = stats + "\n" + GetBoomDamageText(BoomDie_Damage);
		}
	}

	public override string GetInfoA()
	{
		string empty = string.Empty;
		empty += LOC.MM.GetSkill(Info);
		empty += "\n";
		empty = ((base.Level_Base <= 0) ? (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Next Level"), base.Level_Base + 1)) : (empty + string.Format("<color=#FFE397>{0}：{1}</color> \n", LOC.MM.GetMain("Current Level"), base.Level_Base_Last)));
		empty += $"{LOC.MM.GetMain(SWS.Dot_R(damageType))} : {DOTrate_Max}% ";
		empty += string.Format("\n{0} : {1} ", LOC.MM.GetMain("Overlay"), Layer_Max);
		empty = empty + "\n" + LOC.MM.GetMain("EveryLayer") + " : " + GetDotDamageText();
		if (ATSpeedCut_Last > 0f)
		{
			empty += string.Format("\n{0} : -{1}% ", LOC.MM.GetMain("AttackSpeed"), ATSpeedCut_Last);
		}
		if (MVSpeedCut_Last > 0f)
		{
			empty += string.Format("\n{0} : -{1}% ", LOC.MM.GetMain("MoveSpeed"), MVSpeedCut_Last);
		}
		AppendExtraDotInfoA(ref empty);
		empty += string.Format("\n{0} : {1} {2}", LOC.MM.GetMain("Duration"), Time_Max, LOC.MM.GetMain("S"));
		AppendExtraDotInfoB(ref empty);
		return empty;
	}

	public override string GetInfoB()
	{
		string empty = string.Empty;
		empty += string.Format("<color=#FFE397>{0}：{1}</color>\n", LOC.MM.GetMain("Next Level"), base.Level_Base_Last + 1);
		empty += $"{LOC.MM.GetMain(SWS.Dot_R(damageType))} : {DOTrate_Max + DOTrate_Level}% ";
		empty += string.Format("\n{0} : {1} ", LOC.MM.GetMain("Overlay"), Layer_Max);
		empty = empty + "\n" + LOC.MM.GetMain("EveryLayer") + " : " + GetDotDamageText() + " ";
		if (ATSpeedCut_Last + ATSpeedCut_Level > 0f)
		{
			empty += string.Format("\n{0} : -{1}% ", LOC.MM.GetMain("AttackSpeed"), ATSpeedCut_Last + ATSpeedCut_Level);
		}
		if (MVSpeedCut_Last + MVSpeedCut_Level > 0f)
		{
			empty += string.Format("\n{0} : -{1}% ", LOC.MM.GetMain("MoveSpeed"), MVSpeedCut_Last + MVSpeedCut_Level);
		}
		AppendExtraDotInfoA(ref empty);
		empty += string.Format("\n{0} : {1} {2}", LOC.MM.GetMain("Duration"), Time_Max, LOC.MM.GetMain("S"));
		AppendExtraDotInfoB(ref empty);
		return empty;
	}
}
