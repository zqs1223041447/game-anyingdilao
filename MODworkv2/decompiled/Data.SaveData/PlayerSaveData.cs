using System;
using System.Collections.Generic;

namespace Data.SaveData;

[Serializable]
public class PlayerSaveData
{
	public string PlayerName;

	public int PlayerType;

	public Dictionary<int, int> EquippedSetCounts;

	public bool AutoAttackEnabled;

	public bool AutoJH;

	public bool AutoDrinkH;

	public bool AutoDrinkM;

	public int CompCount;

	public float XJL_DMG;

	public float XJL_UseSKTime;

	public int QH_Price;

	public int QH_Bei;

	public float Temple_HealPrc;

	public int Level;

	public float Health;

	public float Health_Bei;

	public float Health_R_Base;

	public float Health_Percent;

	public float Mana;

	public float Mana_Bei;

	public float Mana_R_Base;

	public float Mana_Percent;

	public float Xp_Total;

	public float Xp_CurrentLevel;

	public int DFLevel;

	public float DFXp_Total;

	public float DFXp_CurrentLevel;

	public float Attack_R_health_Base;

	public float Attack_R_health_Percent;

	public float Attack_R_mana_Base;

	public float Attack_R_mana_Percent;

	public float ATSpeed_Bei;

	public float MVSpeed_Bei;

	public float AntiSlow;

	public float CoolDown;

	public float GeDang;

	public float BJrate;

	public float BJDamage;

	public float JYrate;

	public float ThroughRate;

	public float ItemDrop_Rate;

	public float DOTcut;

	public float Damage_Anti;

	public float FlySpeed;

	public float ORB_Damage;

	public float Damage_Base;

	public float Damage_Bei;

	public float FireDamageXi;

	public float FrozenDamageXi;

	public float ThunderDamageXi;

	public float PoisonDamageXi;

	public float PhysicsDamageXi;

	public float ShadowDamageXi;

	public float FireDamage_Bei;

	public float FrozenDamage_Bei;

	public float ThunderDamage_Bei;

	public float PoisonDamage_Bei;

	public float PhysicsDamage_Bei;

	public float ShadowDamage_Bei;

	public float FireChuan;

	public float FrozenChuan;

	public float ThunderChuan;

	public float PoisonChuan;

	public float PhysicsChuan;

	public float ShadowChuan;

	public float FireAnti;

	public float FrozenAnti;

	public float ThunderAnti;

	public float PoisonAnti;

	public float PhysicsAnti;

	public float ShadowAnti;

	public float C_Health;

	public float C_Damage;

	public float C_ATSpeed;

	public float C_MVSpeed;

	public float C_AllAnti;

	public float Pick_PL_Base;

	public float Pick_PL_Bei;

	public float Pick_XJL_Base;

	public float Pick_XJL_Bei;

	public float Pick_PL_Percent;

	public float Pick_XJL_Percent;

	public float XJL_SellPrice;

	public int Reforge_Inc;

	public int QH_Inc;

	public int HH_Inc;

	public int SK_Inc;

	public float ManaXH;

	public float BJD_Anti;

	public float AllChuan;

	public float AllAnti;

	public float BuffT_Temple;

	public float BuffT_Drink;

	public int WPSPC_DMG;

	public int WPSPC_Rate;

	public int JYBoss_DMG;

	public int JYBoss_Anti;

	public float DMG_R_H;

	public float DMG_R_M;

	public int BS_Add;

	public float BS_Multi;

	public int Temple_DMG;

	public int Temple_ATS;

	public int Temple_MVS;

	public int Temple_BS;

	public float BE_ZQ_DMG;

	public float BE_ZQ_ATS;

	public float BE_ZQ_MVS;

	public float BE_ZQ_BJR;

	public float BE_ZQ_BJD;

	public float BE_ZQ_Heal;

	public float BE_ZQ_Mana;

	public float BE_ZQ_CP_Heal;

	public float BE_ZQ_CP_DMG;

	public float BE_ZQ_CP_ATS;

	public float BE_ZQ_CP_MVS;

	public float BE_ZQ_CP_Anti;

	public float BE_ZQ_Dot;

	public float BE_ZQ_XJ_DMG;

	public float BE_ZQ_Orb_DMG;

	public float BE_SPC_DMG;

	public float BE_SPC_ATS;

	public float BE_SPC_MVS;

	public float BE_SPC_BJR;

	public float BE_SPC_BJD;

	public float BE_SPC_Heal;

	public float BE_SPC_Mana;

	public float BE_SPC_CP_Heal;

	public float BE_SPC_CP_DMG;

	public float BE_SPC_CP_ATS;

	public float BE_SPC_CP_MVS;

	public float BE_SPC_CP_Anti;

	public float BE_SPC_Dot;

	public float BE_SPC_XJ_DMG;

	public float BE_SPC_Orb_DMG;

	public float BE_HH_DMG;

	public float BE_HH_ATS;

	public float BE_HH_MVS;

	public float BE_HH_BJR;

	public float BE_HH_BJD;

	public float BE_HH_Heal;

	public float BE_HH_Mana;

	public float BE_HH_CP_Heal;

	public float BE_HH_CP_DMG;

	public float BE_HH_CP_ATS;

	public float BE_HH_CP_MVS;

	public float BE_HH_CP_Anti;

	public float BE_HH_Dot;

	public float BE_HH_XJ_DMG;

	public float BE_HH_Orb_DMG;

	public float BE_SK_DMG;

	public float BE_SK_ATS;

	public float BE_SK_MVS;

	public float BE_SK_CP_Heal;

	public float BE_SK_CP_DMG;

	public float BE_SK_CP_ATS;

	public float BE_SK_CP_Anti;

	public float BE_SK_XJ_DMG;

	public float BE_SK_Orb_DMG;

	public int BE_SK_FQ_Count;

	public float BE_BS_DMG;

	public float BE_BS_ATS;

	public float BE_BS_MVS;

	public float BE_BS_CP_Heal;

	public float BE_BS_CP_DMG;

	public float BE_BS_CP_ATS;

	public float BE_BS_CP_Anti;

	public float BE_BS_XJ_DMG;

	public float BE_BS_Orb_DMG;

	public int BE_BS_FQ_Count;

	public int Crit_BoomEXP;

	public int Crit_BoomDie_Rate;

	public int Crit_MS;

	public int LowH_DMG20;

	public int LowH_DMG50;

	public int HighH_DMG90;

	public int HighH_DMG100;

	public int LowH_HurtR20;

	public int HighH_HurtR100;

	public int LowH_DMGAnti20;

	public int LowH_DMGAnti50;

	public bool LowH_CritAnti10;

	public int LowM_DMG20;

	public int LowM_DMG50;

	public int HighM_DMG90;

	public int HighM_DMG100;

	public int LowM_HurtR20;

	public int HighM_HurtR100;

	public int ST_MV_DMG;

	public int ST_MV_ATS;

	public int ST_MV_GD;

	public int ST_NoMV_DMG;

	public int ST_NoMV_ATS;

	public int ST_NoMV_DMGAnti;

	public float ST_NoMV_HealPrc;

	public float ST_NoMV_ManaPrc;

	public int ST_Chong_DMG;

	public int ST_Chong_Anti;

	public int EM_LowH_DMG20;

	public int EM_LowH_DMG50;

	public int EM_HighH_DMG60;

	public int EM_HighH_DMG100;

	public int EM_Heal_Crit;

	public float CP1_DMG;

	public float CP1_ATS;

	public float CP1_MVS;

	public float CP1_Heal;

	public float CP1_Mana;

	public float CP1_DMG_Anti;

	public float CP1_DropR;

	public float CP1_ORB_DMG;

	public float CP1_DMG0;

	public float CP1_DMG1;

	public float CP1_DMG2;

	public float CP1_DMG3;

	public float CP1_DMG4;

	public float CP1_DMG5;

	public float CP1_Chuan0;

	public float CP1_Chuan1;

	public float CP1_Chuan2;

	public float CP1_Chuan3;

	public float CP1_Chuan4;

	public float CP1_Chuan5;

	public float CP1_CP_Heal;

	public float CP1_CP_DMG;

	public float CP1_CP_ATS;

	public float CP1_CP_AllAnti;

	public float CLass_DMG;

	public float CLass_ATS;

	public float CLass_MVS;

	public float CLass_Heal;

	public float CLass_Mana;

	public float CLass_DMG_Anti;

	public float CLass_DropR;

	public float CLass_ORB_DMG;

	public float CLass_DMG0;

	public float CLass_DMG1;

	public float CLass_DMG2;

	public float CLass_DMG3;

	public float CLass_DMG4;

	public float CLass_DMG5;

	public float CLass_Chuan0;

	public float CLass_Chuan1;

	public float CLass_Chuan2;

	public float CLass_Chuan3;

	public float CLass_Chuan4;

	public float CLass_Chuan5;

	public float CLass_CP_Heal;

	public float CLass_CP_DMG;

	public float CLass_CP_ATS;

	public float CLass_CP_AllAnti;

	public float Class_CP_DotDMG;

	public int XJ_DMG;

	public int XJ_Time;

	public int TuT_Buff;

	public int TuT_Time;

	public bool TuT_PlayerAll;

	public int Top_CD;

	public int Top_GD;

	public int Top_Anti;

	public float Top_Cut_DMG;

	public float Top_Cut_MVS;

	public float Top_Cut_ATS;

	public float AllDot_DMG;

	public float AllDot_Time;

	public int AllDot_Layer;

	public float AllDot_MV;

	public float AllDot_JY;

	public float DiffDotDMG;

	public int DiffDebuff_DMG;

	public bool Dot_MSAll;

	public int DrinkPre_Heal;

	public int DrinkPre_Mana;

	public int DrinkPre_DMG;

	public bool Drink_CP;

	public float Z_Hmax_DMG;

	public float Z_Huse_DMG;

	public float Z_Mmax_DMG;

	public float Z_Mcur_DMG;

	public float Z_Muse_DMG;

	public float Z_Hmax_EL0;

	public float Z_Hmax_EL1;

	public float Z_Hmax_EL2;

	public float Z_Hmax_EL3;

	public float Z_Hmax_EL4;

	public float Z_Hmax_EL5;

	public float Z_Mmax_EL0;

	public float Z_Mmax_EL1;

	public float Z_Mmax_EL2;

	public float Z_Mmax_EL3;

	public float Z_Mmax_EL4;

	public float Z_Mmax_EL5;

	public float Z_CD_EL0;

	public float Z_CD_EL1;

	public float Z_CD_EL2;

	public float Z_CD_EL3;

	public float Z_CD_EL4;

	public float Z_CD_EL5;

	public int Z_Anti0_EL0;

	public int Z_Anti0_EL1;

	public int Z_Anti0_EL2;

	public int Z_Anti0_EL3;

	public int Z_Anti0_EL4;

	public int Z_Anti0_EL5;

	public int Z_Chuan0_EL0;

	public int Z_Chuan0_EL1;

	public int Z_Chuan0_EL2;

	public int Z_Chuan0_EL3;

	public int Z_Chuan0_EL4;

	public int Z_Chuan0_EL5;

	public int Z_GD_EL0;

	public int Z_GD_EL1;

	public int Z_GD_EL2;

	public int Z_GD_EL3;

	public int Z_GD_EL4;

	public int Z_GD_EL5;

	public int Z_BJR_EL0;

	public int Z_BJR_EL1;

	public int Z_BJR_EL2;

	public int Z_BJR_EL3;

	public int Z_BJR_EL4;

	public int Z_BJR_EL5;

	public int Z_DMGCut_EL0;

	public int Z_DMGCut_EL1;

	public int Z_DMGCut_EL2;

	public int Z_DMGCut_EL3;

	public int Z_DMGCut_EL4;

	public int Z_DMGCut_EL5;

	public int Z_Thr_EL0;

	public int Z_Thr_EL1;

	public int Z_Thr_EL2;

	public int Z_Thr_EL3;

	public int Z_Thr_EL4;

	public int Z_Thr_EL5;

	public float Z_CD_CP_DMG;

	public float Z_ATS_CP_DMG;

	public float Z_MVS_DMG;

	public float Z_MVS_ATS;

	public bool Z_BJR_BJD;

	public int Z_Chuan0_BJD;

	public int Z_Chuan1_BJD;

	public int Z_Chuan2_BJD;

	public int Z_Chuan3_BJD;

	public int Z_Chuan4_BJD;

	public int Z_Chuan5_BJD;

	public int PrcCut0;

	public int PrcCut1;

	public int PrcCut2;

	public int PrcCut3;

	public int PrcCut4;

	public int PrcCut5;

	public int PrcCut5P0;

	public int PrcCut5P1;

	public int PrcCut5P2;

	public int PrcCut5P3;

	public int PrcCut5P4;

	public int PrcCut5P5;

	public int PrcCut3P0;

	public int PrcCut3P1;

	public int PrcCut3P2;

	public int PrcCut3P3;

	public int PrcCut3P4;

	public int PrcCut3P5;

	public bool DeadWD;

	public bool DeadRageWD;

	public bool DeadStealthWD;

	public bool WS_Anti0;

	public bool WS_Anti1;

	public bool WS_Anti2;

	public bool WS_Anti3;

	public bool WS_Anti4;

	public bool WS_Anti5;

	public bool WS_All;

	public float EMC_DMG_20;

	public float EMC_DMG_48;

	public float EMC_Anti_9;

	public float EMC_GD_12;

	public float JYC_DMG_15;

	public float JYC_ATS_24;

	public float JYC_BJD_24;

	public int SKUP_Xi;

	public int SKUP_SP;

	public int SKUP_CP;

	public int SKUP_Bei;

	public int SKUP_Final;

	public int SKUP_AT;

	public int Dis_In;

	public bool Dis_Out;

	public bool AB_DMG_Mana;

	public bool AB_DMG_Hurt;

	public bool AB_Dot_DMG;

	public bool NoGD;

	public float ST_EveryH_DMG;

	public float ST_EveryM_Drop;

	public int ORB_FQ_Count;

	public bool ORB_FQ_Count_Double;

	public int ORB_FQ_DMG80_Base;

	public int ORB_FQ_DMG120_Base;

	public float Orb_Universe_DMG_Base;

	public int HighMana_DMG100_FQ;

	public float Orb_Universe_ATS;

	public float Orb_Bow_DMG;

	public float Orb_Bow_ATS;

	public int XJ_Count_CP_DMG;

	public int BurnLife0;

	public int BurnLife1;

	public int BurnLife2;

	public int BurnLife3;

	public int BurnLife4;

	public int BurnLife5;

	public bool DieEXP;

	public int NoDot_BJD;

	public bool HealCutMana;

	public bool AT_UseHeal1;

	public int ManaUse_Rheal;

	public bool RMana_RHeal;

	public bool CP_Same_RHeal;

	public bool FT;

	public int DMG_ManaPRC;

	public bool Turtle;

	public int GD_HurtR;

	public bool BloodLost;

	public bool NoGround;

	public bool CPNoBad;

	public bool CPNoGround;

	public bool AT_UseHeal2;

	public float DMGsplit;

	public bool BladeSoul_Double;

	public int Diff_EL;

	public float EXP_Range;

	public float Buff_Range;

	public bool MoneyTO_DMG;

	public PlayerDotData Dot_Fire;

	public PlayerDotData Dot_Ice;

	public PlayerDotData Dot_TD;

	public PlayerDotData Dot_Du;

	public PlayerDotData Dot_Phy;

	public PlayerDotData Dot_SD;

	public static PlayerSaveData CreateDefault()
	{
		return new PlayerSaveData
		{
			Level = 1,
			EquippedSetCounts = new Dictionary<int, int>(),
			AutoAttackEnabled = false,
			AutoJH = false,
			AutoDrinkH = false,
			AutoDrinkM = false,
			CompCount = 0,
			XJL_DMG = 0f,
			XJL_UseSKTime = 0f,
			QH_Price = 0,
			QH_Bei = 0,
			Temple_HealPrc = 0f,
			Health = 500f,
			Health_Bei = 0f,
			Health_R_Base = 1f,
			Health_Percent = 0f,
			Mana = 150f,
			Mana_Bei = 0f,
			Mana_R_Base = 1f,
			Mana_Percent = 0f,
			Xp_Total = 0f,
			Xp_CurrentLevel = 0f,
			DFLevel = 1,
			DFXp_Total = 0f,
			DFXp_CurrentLevel = 0f,
			Attack_R_health_Percent = 0f,
			Attack_R_health_Base = 0f,
			Attack_R_mana_Base = 0f,
			Attack_R_mana_Percent = 0f,
			ATSpeed_Bei = 0f,
			MVSpeed_Bei = 0f,
			AntiSlow = 0f,
			CoolDown = 0f,
			GeDang = 0f,
			BJrate = 0f,
			BJDamage = 0f,
			JYrate = 0f,
			ThroughRate = 0f,
			ItemDrop_Rate = 0f,
			DOTcut = 0f,
			Damage_Anti = 0f,
			FlySpeed = 0f,
			ORB_Damage = 0f,
			Damage_Base = 50f,
			Damage_Bei = 0f,
			FireDamageXi = 0f,
			FrozenDamageXi = 0f,
			ThunderDamageXi = 0f,
			PoisonDamageXi = 0f,
			PhysicsDamageXi = 0f,
			ShadowDamageXi = 0f,
			FireDamage_Bei = 0f,
			FrozenDamage_Bei = 0f,
			ThunderDamage_Bei = 0f,
			PoisonDamage_Bei = 0f,
			PhysicsDamage_Bei = 0f,
			ShadowDamage_Bei = 0f,
			FireChuan = 0f,
			FrozenChuan = 0f,
			ThunderChuan = 0f,
			PoisonChuan = 0f,
			PhysicsChuan = 0f,
			ShadowChuan = 0f,
			FireAnti = 0f,
			FrozenAnti = 0f,
			ThunderAnti = 0f,
			PoisonAnti = 0f,
			PhysicsAnti = 0f,
			ShadowAnti = 0f,
			C_Health = 0f,
			C_Damage = 0f,
			C_ATSpeed = 0f,
			C_MVSpeed = 0f,
			C_AllAnti = 0f,
			Pick_PL_Base = 0.8f,
			Pick_PL_Bei = 0f,
			Pick_XJL_Base = 0.8f,
			Pick_XJL_Bei = 0f,
			XJL_SellPrice = 0f,
			Reforge_Inc = 0,
			QH_Inc = 0,
			HH_Inc = 10,
			SK_Inc = 0,
			ManaXH = 0f,
			BJD_Anti = 0f,
			AllChuan = 0f,
			AllAnti = 0f,
			BuffT_Temple = 0f,
			BuffT_Drink = 0f,
			WPSPC_DMG = 0,
			WPSPC_Rate = 0,
			JYBoss_DMG = 0,
			JYBoss_Anti = 0,
			DMG_R_H = 0f,
			DMG_R_M = 0f,
			BS_Add = 0,
			BS_Multi = 0f,
			Temple_DMG = 0,
			Temple_ATS = 0,
			Temple_MVS = 0,
			Temple_BS = 0,
			BE_ZQ_DMG = 0f,
			BE_ZQ_ATS = 0f,
			BE_ZQ_MVS = 0f,
			BE_ZQ_BJR = 0f,
			BE_ZQ_BJD = 0f,
			BE_ZQ_Heal = 0f,
			BE_ZQ_Mana = 0f,
			BE_ZQ_CP_Heal = 0f,
			BE_ZQ_CP_DMG = 0f,
			BE_ZQ_CP_ATS = 0f,
			BE_ZQ_CP_MVS = 0f,
			BE_ZQ_CP_Anti = 0f,
			BE_ZQ_Dot = 0f,
			BE_ZQ_XJ_DMG = 0f,
			BE_ZQ_Orb_DMG = 0f,
			BE_SPC_DMG = 0f,
			BE_SPC_ATS = 0f,
			BE_SPC_MVS = 0f,
			BE_SPC_BJR = 0f,
			BE_SPC_BJD = 0f,
			BE_SPC_Heal = 0f,
			BE_SPC_Mana = 0f,
			BE_SPC_CP_Heal = 0f,
			BE_SPC_CP_DMG = 0f,
			BE_SPC_CP_ATS = 0f,
			BE_SPC_CP_MVS = 0f,
			BE_SPC_CP_Anti = 0f,
			BE_SPC_Dot = 0f,
			BE_SPC_XJ_DMG = 0f,
			BE_SPC_Orb_DMG = 0f,
			BE_HH_DMG = 0f,
			BE_HH_ATS = 0f,
			BE_HH_MVS = 0f,
			BE_HH_BJR = 0f,
			BE_HH_BJD = 0f,
			BE_HH_Heal = 0f,
			BE_HH_Mana = 0f,
			BE_HH_CP_Heal = 0f,
			BE_HH_CP_DMG = 0f,
			BE_HH_CP_ATS = 0f,
			BE_HH_CP_MVS = 0f,
			BE_HH_CP_Anti = 0f,
			BE_HH_Dot = 0f,
			BE_HH_XJ_DMG = 0f,
			BE_HH_Orb_DMG = 0f,
			BE_SK_DMG = 0f,
			BE_SK_ATS = 0f,
			BE_SK_MVS = 0f,
			BE_SK_CP_Heal = 0f,
			BE_SK_CP_DMG = 0f,
			BE_SK_CP_ATS = 0f,
			BE_SK_CP_Anti = 0f,
			BE_SK_XJ_DMG = 0f,
			BE_SK_Orb_DMG = 0f,
			BE_SK_FQ_Count = 0,
			BE_BS_DMG = 0f,
			BE_BS_ATS = 0f,
			BE_BS_MVS = 0f,
			BE_BS_CP_Heal = 0f,
			BE_BS_CP_DMG = 0f,
			BE_BS_CP_ATS = 0f,
			BE_BS_CP_Anti = 0f,
			BE_BS_XJ_DMG = 0f,
			BE_BS_Orb_DMG = 0f,
			BE_BS_FQ_Count = 0,
			Crit_BoomEXP = 0,
			Crit_BoomDie_Rate = 0,
			Crit_MS = 0,
			LowH_DMG20 = 0,
			LowH_DMG50 = 0,
			HighH_DMG90 = 0,
			HighH_DMG100 = 0,
			LowH_HurtR20 = 0,
			HighH_HurtR100 = 0,
			LowH_DMGAnti20 = 0,
			LowH_DMGAnti50 = 0,
			LowH_CritAnti10 = false,
			LowM_DMG20 = 0,
			LowM_DMG50 = 0,
			HighM_DMG90 = 0,
			HighM_DMG100 = 0,
			LowM_HurtR20 = 0,
			HighM_HurtR100 = 0,
			ST_MV_DMG = 0,
			ST_MV_ATS = 0,
			ST_MV_GD = 0,
			ST_NoMV_DMG = 0,
			ST_NoMV_ATS = 0,
			ST_NoMV_DMGAnti = 0,
			ST_NoMV_HealPrc = 0f,
			ST_NoMV_ManaPrc = 0f,
			ST_Chong_DMG = 0,
			ST_Chong_Anti = 0,
			EM_LowH_DMG20 = 0,
			EM_LowH_DMG50 = 0,
			EM_HighH_DMG60 = 0,
			EM_HighH_DMG100 = 0,
			EM_Heal_Crit = 0,
			CP1_DMG = 0f,
			CP1_ATS = 0f,
			CP1_MVS = 0f,
			CP1_Heal = 0f,
			CP1_Mana = 0f,
			CP1_DMG_Anti = 0f,
			CP1_DropR = 0f,
			CP1_ORB_DMG = 0f,
			CP1_DMG0 = 0f,
			CP1_DMG1 = 0f,
			CP1_DMG2 = 0f,
			CP1_DMG3 = 0f,
			CP1_DMG4 = 0f,
			CP1_DMG5 = 0f,
			CP1_Chuan0 = 0f,
			CP1_Chuan1 = 0f,
			CP1_Chuan2 = 0f,
			CP1_Chuan3 = 0f,
			CP1_Chuan4 = 0f,
			CP1_Chuan5 = 0f,
			CP1_CP_Heal = 0f,
			CP1_CP_DMG = 0f,
			CP1_CP_ATS = 0f,
			CP1_CP_AllAnti = 0f,
			CLass_DMG = 0f,
			CLass_ATS = 0f,
			CLass_MVS = 0f,
			CLass_Heal = 0f,
			CLass_Mana = 0f,
			CLass_DMG_Anti = 0f,
			CLass_DropR = 0f,
			CLass_ORB_DMG = 0f,
			CLass_DMG0 = 0f,
			CLass_DMG1 = 0f,
			CLass_DMG2 = 0f,
			CLass_DMG3 = 0f,
			CLass_DMG4 = 0f,
			CLass_DMG5 = 0f,
			CLass_Chuan0 = 0f,
			CLass_Chuan1 = 0f,
			CLass_Chuan2 = 0f,
			CLass_Chuan3 = 0f,
			CLass_Chuan4 = 0f,
			CLass_Chuan5 = 0f,
			CLass_CP_Heal = 0f,
			CLass_CP_DMG = 0f,
			CLass_CP_ATS = 0f,
			CLass_CP_AllAnti = 0f,
			Class_CP_DotDMG = 0f,
			XJ_DMG = 0,
			XJ_Time = 0,
			TuT_Buff = 0,
			TuT_Time = 0,
			TuT_PlayerAll = false,
			Top_CD = 0,
			Top_GD = 0,
			Top_Anti = 0,
			Top_Cut_DMG = 0f,
			Top_Cut_MVS = 0f,
			Top_Cut_ATS = 0f,
			AllDot_DMG = 0f,
			AllDot_Time = 0f,
			AllDot_Layer = 0,
			AllDot_MV = 0f,
			AllDot_JY = 0f,
			DiffDotDMG = 0f,
			DiffDebuff_DMG = 0,
			Dot_MSAll = false,
			DrinkPre_Heal = 0,
			DrinkPre_Mana = 0,
			DrinkPre_DMG = 0,
			Drink_CP = false,
			Z_Hmax_DMG = 0f,
			Z_Huse_DMG = 0f,
			Z_Mmax_DMG = 0f,
			Z_Mcur_DMG = 0f,
			Z_Muse_DMG = 0f,
			Z_Hmax_EL0 = 0f,
			Z_Hmax_EL1 = 0f,
			Z_Hmax_EL2 = 0f,
			Z_Hmax_EL3 = 0f,
			Z_Hmax_EL4 = 0f,
			Z_Hmax_EL5 = 0f,
			Z_Mmax_EL0 = 0f,
			Z_Mmax_EL1 = 0f,
			Z_Mmax_EL2 = 0f,
			Z_Mmax_EL3 = 0f,
			Z_Mmax_EL4 = 0f,
			Z_Mmax_EL5 = 0f,
			Z_CD_EL0 = 0f,
			Z_CD_EL1 = 0f,
			Z_CD_EL2 = 0f,
			Z_CD_EL3 = 0f,
			Z_CD_EL4 = 0f,
			Z_CD_EL5 = 0f,
			Z_Anti0_EL0 = 0,
			Z_Anti0_EL1 = 0,
			Z_Anti0_EL2 = 0,
			Z_Anti0_EL3 = 0,
			Z_Anti0_EL4 = 0,
			Z_Anti0_EL5 = 0,
			Z_Chuan0_EL0 = 0,
			Z_Chuan0_EL1 = 0,
			Z_Chuan0_EL2 = 0,
			Z_Chuan0_EL3 = 0,
			Z_Chuan0_EL4 = 0,
			Z_Chuan0_EL5 = 0,
			Z_GD_EL0 = 0,
			Z_GD_EL1 = 0,
			Z_GD_EL2 = 0,
			Z_GD_EL3 = 0,
			Z_GD_EL4 = 0,
			Z_GD_EL5 = 0,
			Z_BJR_EL0 = 0,
			Z_BJR_EL1 = 0,
			Z_BJR_EL2 = 0,
			Z_BJR_EL3 = 0,
			Z_BJR_EL4 = 0,
			Z_BJR_EL5 = 0,
			Z_DMGCut_EL0 = 0,
			Z_DMGCut_EL1 = 0,
			Z_DMGCut_EL2 = 0,
			Z_DMGCut_EL3 = 0,
			Z_DMGCut_EL4 = 0,
			Z_DMGCut_EL5 = 0,
			Z_Thr_EL0 = 0,
			Z_Thr_EL1 = 0,
			Z_Thr_EL2 = 0,
			Z_Thr_EL3 = 0,
			Z_Thr_EL4 = 0,
			Z_Thr_EL5 = 0,
			Z_CD_CP_DMG = 0f,
			Z_ATS_CP_DMG = 0f,
			Z_MVS_DMG = 0f,
			Z_MVS_ATS = 0f,
			Z_BJR_BJD = false,
			Z_Chuan0_BJD = 0,
			Z_Chuan1_BJD = 0,
			Z_Chuan2_BJD = 0,
			Z_Chuan3_BJD = 0,
			Z_Chuan4_BJD = 0,
			Z_Chuan5_BJD = 0,
			PrcCut0 = 0,
			PrcCut1 = 0,
			PrcCut2 = 0,
			PrcCut3 = 0,
			PrcCut4 = 0,
			PrcCut5 = 0,
			PrcCut5P0 = 0,
			PrcCut5P1 = 0,
			PrcCut5P2 = 0,
			PrcCut5P3 = 0,
			PrcCut5P4 = 0,
			PrcCut5P5 = 0,
			PrcCut3P0 = 0,
			PrcCut3P1 = 0,
			PrcCut3P2 = 0,
			PrcCut3P3 = 0,
			PrcCut3P4 = 0,
			PrcCut3P5 = 0,
			DeadWD = false,
			DeadRageWD = false,
			DeadStealthWD = false,
			WS_Anti0 = false,
			WS_Anti1 = false,
			WS_Anti2 = false,
			WS_Anti3 = false,
			WS_Anti4 = false,
			WS_Anti5 = false,
			WS_All = false,
			EMC_DMG_20 = 0f,
			EMC_DMG_48 = 0f,
			EMC_Anti_9 = 0f,
			EMC_GD_12 = 0f,
			JYC_DMG_15 = 0f,
			JYC_ATS_24 = 0f,
			JYC_BJD_24 = 0f,
			SKUP_Xi = 0,
			SKUP_SP = 0,
			SKUP_CP = 0,
			SKUP_Bei = 0,
			SKUP_Final = 0,
			SKUP_AT = 0,
			Dis_In = 0,
			Dis_Out = false,
			AB_DMG_Mana = false,
			AB_DMG_Hurt = false,
			AB_Dot_DMG = false,
			NoGD = false,
			ST_EveryH_DMG = 0f,
			ST_EveryM_Drop = 0f,
			ORB_FQ_Count = 0,
			ORB_FQ_Count_Double = false,
			ORB_FQ_DMG80_Base = 0,
			ORB_FQ_DMG120_Base = 0,
			Orb_Universe_DMG_Base = 0f,
			HighMana_DMG100_FQ = 0,
			Orb_Universe_ATS = 0f,
			Orb_Bow_DMG = 0f,
			Orb_Bow_ATS = 0f,
			XJ_Count_CP_DMG = 0,
			BurnLife0 = 0,
			BurnLife1 = 0,
			BurnLife2 = 0,
			BurnLife3 = 0,
			BurnLife4 = 0,
			BurnLife5 = 0,
			DieEXP = false,
			NoDot_BJD = 0,
			HealCutMana = false,
			AT_UseHeal1 = false,
			ManaUse_Rheal = 0,
			RMana_RHeal = false,
			CP_Same_RHeal = false,
			FT = false,
			DMG_ManaPRC = 0,
			Turtle = false,
			GD_HurtR = 0,
			BloodLost = false,
			NoGround = false,
			CPNoBad = false,
			CPNoGround = false,
			AT_UseHeal2 = false,
			DMGsplit = 0f,
			BladeSoul_Double = false,
			Diff_EL = 0,
			EXP_Range = 0f,
			Buff_Range = 0f,
			MoneyTO_DMG = false,
			Dot_Fire = PlayerDotData.CreateDefault(),
			Dot_Ice = PlayerDotData.CreateDefault(),
			Dot_TD = PlayerDotData.CreateDefault(),
			Dot_Du = PlayerDotData.CreateDefault(),
			Dot_Phy = PlayerDotData.CreateDefault(),
			Dot_SD = PlayerDotData.CreateDefault()
		};
	}
}
