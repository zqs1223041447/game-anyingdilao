using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Data.AutoGen.DataClass.Level;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class GameDataManager : SingletonMonoScope<GameDataManager>
{
	public EnemyPrefab EMPB;

	public TextAsset EnemyCSV;

	public TextAsset BossCSV;

	public readonly List<EnemyMB> EMMB = new List<EnemyMB>();

	public readonly List<BossMB> BossMB = new List<BossMB>();

	public ColorGP colorGP;

	public List<LevelData> levelList = new List<LevelData>();

	public readonly Dictionary<string, LevelData> levelDatas = new Dictionary<string, LevelData>();

	public TextAsset DieCSV;

	public EM_SkillGroup[] SKG_Die = new EM_SkillGroup[50];

	public TextAsset ELSS_CSV;

	public EM_SkillGroup[] SKG_ELSS = new EM_SkillGroup[1700];

	public SKprefab SKPB;

	public GameObject CompMB;

	public GameObject buffer_em;

	public GameObject buffer_pl;

	public GameObject buffer_cp;

	public WDprefab WDPB;

	private bool isInited;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		InitData();
	}

	public void InitData()
	{
		if (!isInited)
		{
			LoadLevelData();
			LoadData_EM(EnemyCSV);
			LoadData_Boss(BossCSV);
			LoadData_Die(DieCSV);
			LoadData_ELSS(ELSS_CSV);
			isInited = true;
		}
	}

	private void LoadLevelData()
	{
		levelList = FilesUtil.LoadDefaultData<LevelDataContainer>().items;
		foreach (LevelData level in levelList)
		{
			levelDatas.Add(level.GlobalID, level);
		}
	}

	private void LoadData_EM(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			EnemyMB enemyMB = new EnemyMB();
			int num = 1;
			enemyMB.GlobalID = int.Parse(array[i][num]);
			num++;
			enemyMB.IndexName.Add(array[i][num]);
			num++;
			enemyMB.IndexName.Add(array[i][num]);
			num++;
			enemyMB.IndexName.Add(array[i][num]);
			num++;
			enemyMB.IndexA = int.Parse(array[i][num]);
			num++;
			enemyMB.IndexB = int.Parse(array[i][num]);
			num++;
			enemyMB.Xp = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.size = int.Parse(array[i][num]);
			num++;
			enemyMB.CompOffset = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.TuiSpeed = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.ItemDropPos = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.SpineType = int.Parse(array[i][num]);
			num++;
			enemyMB.ColorIndex = int.Parse(array[i][num]);
			num++;
			enemyMB.CP_FX = int.Parse(array[i][num]);
			num++;
			enemyMB.EnemyType = int.Parse(array[i][num]);
			num++;
			enemyMB.Health = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.AttackSpeed_JG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.ATSpeed = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.MVSpeed = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.Damage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.Range_Base = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.Range_Anger = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.Range_Far = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.SK_Rate = int.Parse(array[i][num]);
			num++;
			enemyMB.SPtype = int.Parse(array[i][num]);
			num++;
			enemyMB.Die_Index = int.Parse(array[i][num]);
			num++;
			enemyMB.DieType = int.Parse(array[i][num]);
			num++;
			enemyMB.DiePos = int.Parse(array[i][num]);
			num++;
			enemyMB.DieFX_TimeDelay = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.DieDelay = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.Lie_Index = int.Parse(array[i][num]);
			num++;
			enemyMB.LiePos = int.Parse(array[i][num]);
			num++;
			enemyMB.FSDie_Index = int.Parse(array[i][num]);
			num++;
			enemyMB.Idle_Time_Min = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.Idle_Time_Max = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.SO_IdleRate = int.Parse(array[i][num]);
			num++;
			enemyMB.SO_AttackRate = int.Parse(array[i][num]);
			num++;
			enemyMB.SO_SayRate = int.Parse(array[i][num]);
			num++;
			enemyMB.SO_HurtRate = int.Parse(array[i][num]);
			num++;
			enemyMB.SO_DieRate = int.Parse(array[i][num]);
			num++;
			enemyMB.SO_Idle = array[i][num];
			num++;
			enemyMB.SO_Walk = array[i][num];
			num++;
			enemyMB.SO_AttackA = array[i][num];
			num++;
			enemyMB.SO_SayA = array[i][num];
			num++;
			enemyMB.SO_AttackB = array[i][num];
			num++;
			enemyMB.SO_SayB = array[i][num];
			num++;
			enemyMB.SO_AttackC = array[i][num];
			num++;
			enemyMB.SO_SayC = array[i][num];
			num++;
			enemyMB.SO_Hurt = array[i][num];
			num++;
			enemyMB.SO_Die = array[i][num];
			num++;
			enemyMB.SO_ChuiDi = array[i][num];
			num++;
			enemyMB.HitFX = int.Parse(array[i][num]);
			num++;
			num += 3;
			enemyMB.AT1 = int.Parse(array[i][num]);
			num++;
			enemyMB.AT2 = int.Parse(array[i][num]);
			num++;
			enemyMB.AT3 = int.Parse(array[i][num]);
			num++;
			enemyMB.AT_Ani = int.Parse(array[i][num]);
			num++;
			enemyMB.AT_Fang = SWS.GetBool(int.Parse(array[i][num]));
			num++;
			enemyMB.AT_Distans = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			enemyMB.SK1 = int.Parse(array[i][num]);
			num++;
			enemyMB.SK2 = int.Parse(array[i][num]);
			num++;
			enemyMB.SK3 = int.Parse(array[i][num]);
			num++;
			enemyMB.SK4 = int.Parse(array[i][num]);
			num++;
			enemyMB.SK5 = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_Ani = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_Fang = SWS.GetBool(int.Parse(array[i][num]));
			num++;
			enemyMB.SK_Distans = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			num++;
			enemyMB.SK_Comp = new EM_Skill_CP();
			enemyMB.SK_Comp.GlobalID = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_Comp.UseAni = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_Comp.CPFX = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_Comp.FSFXtype = int.Parse(array[i][num]);
			num++;
			num++;
			enemyMB.SK_FS = new EM_Skill_FS();
			enemyMB.SK_FS.UseAni = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_FS.CPFX = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_FS.FSFXtype = int.Parse(array[i][num]);
			num++;
			enemyMB.SK_Die_Index = int.Parse(array[i][num]);
			num++;
			enemyMB.ELSS_Index = int.Parse(array[i][num]);
			num++;
			enemyMB.ELSS_Ani = int.Parse(array[i][num]);
			num++;
			enemyMB.ELSS_Fang = SWS.GetBool(int.Parse(array[i][num]));
			num++;
			enemyMB.ELSS_Distans = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			EMMB.Add(enemyMB);
		}
	}

	private void LoadData_Die(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			EM_Skill_SP eM_Skill_SP = new EM_Skill_SP();
			int num = 2;
			eM_Skill_SP.FStype = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ThroughType = int.Parse(array[i][num]);
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.AttackType = true;
			}
			else
			{
				eM_Skill_SP.AttackType = false;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.AttackTypeA = true;
			}
			else
			{
				eM_Skill_SP.AttackTypeA = false;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.AttackTypeB = true;
			}
			else
			{
				eM_Skill_SP.AttackTypeB = false;
			}
			num++;
			eM_Skill_SP.Damage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.SpeedCut = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.DotRate = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.DotDamage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.BuffTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.DebuffTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.EXP_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.OBJ = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ZD_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ZD_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.EXP_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.EXP_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Dic_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Dic_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Sound = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Count_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Count_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.CountMulti = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.CountEXP = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Type_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Type_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeDIC_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeDIC_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeEXP_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeEXP_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.JG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.AngleA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.AngleB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Range1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Range2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Range_AT = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.FStime1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.FStime2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed3 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed4 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Follow_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AllChuan_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.RDSpeed_F = int.Parse(array[i][num]);
			SKG_Die[int.Parse(array[i][1])].SK.Add(eM_Skill_SP);
		}
	}

	private void LoadData_ELSS(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			EM_Skill_SP eM_Skill_SP = new EM_Skill_SP();
			eM_Skill_SP.IndexName = "ELSS";
			int num = 2;
			eM_Skill_SP.HitFX_Rate = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ATFX = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.StarFX = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.StarFX_pos = int.Parse(array[i][num]);
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.BaTi = true;
			}
			else
			{
				eM_Skill_SP.BaTi = false;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.WuDi = true;
			}
			else
			{
				eM_Skill_SP.WuDi = false;
			}
			num++;
			eM_Skill_SP.CJY = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ChongSpeedMulti = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.ATmod = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.FStype = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.FSFXtype = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.RTtypeOBJ = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeTar = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.RTtypeFX = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Distance = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.ThroughType = int.Parse(array[i][num]);
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.AttackType = true;
			}
			else
			{
				eM_Skill_SP.AttackType = false;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.AttackTypeA = true;
			}
			else
			{
				eM_Skill_SP.AttackTypeA = false;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				eM_Skill_SP.AttackTypeB = true;
			}
			else
			{
				eM_Skill_SP.AttackTypeB = false;
			}
			num++;
			eM_Skill_SP.Damage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.DamageA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.DamageB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.SpeedCut = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.BF_DamageAnti = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.CompAttackSpeed = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.C_Damage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Reborn = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.DotRate = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.DotDamage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.BuffTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.DebuffTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.ORB_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.EXP_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.OBJ = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Layer_SubA = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Layer_SubB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ORB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ZD_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ZD_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.ZD_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.EXP_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.EXP_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.EXP_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Dic_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Dic_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Sound = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Count_ORB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Count_ATtarget = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.CF_Count = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Count_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Count_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Count_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.CountMulti = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.CountEXP = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeORB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.CF_Type = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Type_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Type_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Type_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeDIC_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeDIC_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeEXP_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeEXP_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TypeEXP_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Size = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.High = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.JG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.AngleA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.AngleB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Range1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Range2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Range_AT = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.FStime1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.FStime2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed3 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Speed4 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.Follow_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.Follow_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AllChuan_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AllChuan_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.RDSpeed_F = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.RDSpeed_S = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.HasFX = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.S_HasFX = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AB_HasFX = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.colEXP = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.colEXP_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.S_colEXP = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AB_colEXP = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TimeEXP = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.TimeEXP_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.EXPpos = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.EXPpos_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.S_EXPpos = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AB_EXPpos = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AngleEXP = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.AngleEXP_AB = int.Parse(array[i][num]);
			num++;
			eM_Skill_SP.HurtSK_JG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			eM_Skill_SP.HurtSK_Rate = int.Parse(array[i][num]);
			SKG_ELSS[int.Parse(array[i][1])].SK.Add(eM_Skill_SP);
		}
	}

	public void ReLoadAllData()
	{
		levelDatas.Clear();
		EMMB.Clear();
		BossMB.Clear();
		EM_SkillGroup[] sKG_Die = SKG_Die;
		for (int i = 0; i < sKG_Die.Length; i++)
		{
			sKG_Die[i].SK.Clear();
		}
		sKG_Die = SKG_ELSS;
		for (int j = 0; j < sKG_Die.Length; j++)
		{
			sKG_Die[j].SK.Clear();
		}
		LoadLevelData();
		LoadData_EM(EnemyCSV);
		LoadData_Boss(BossCSV);
		LoadData_Die(DieCSV);
		LoadData_ELSS(ELSS_CSV);
	}

	private void LoadData_Boss(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			BossMB bossMB = new BossMB();
			int num = 1;
			bossMB.GlobalID = int.Parse(array[i][num]);
			num++;
			bossMB.IndexName.Add(array[i][num]);
			num++;
			bossMB.IndexName.Add(array[i][num]);
			num++;
			bossMB.IndexName.Add(array[i][num]);
			num++;
			bossMB.IndexName.Add(array[i][num]);
			num++;
			bossMB.IndexA = int.Parse(array[i][num]);
			num++;
			bossMB.IndexB = int.Parse(array[i][num]);
			num++;
			bossMB.Quality = int.Parse(array[i][num]);
			num++;
			bossMB.Xp = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.size = int.Parse(array[i][num]);
			num++;
			bossMB.CompOffset = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.TuiSpeed = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.ItemDropPos = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.SpineType = int.Parse(array[i][num]);
			num++;
			bossMB.ColorIndex = int.Parse(array[i][num]);
			num++;
			bossMB.CP_FX = int.Parse(array[i][num]);
			num++;
			bossMB.BossType = int.Parse(array[i][num]);
			num++;
			bossMB.Health = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.AttackSpeed_JG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.ATSpeed = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.MVSpeed = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.Damage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.Range_Base = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.Range_Anger = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.Range_Far = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.Range_ATplayer_multi = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.SK_Rate = int.Parse(array[i][num]);
			num++;
			bossMB.SK_Rate_Comp = int.Parse(array[i][num]);
			num++;
			bossMB.SPtype = int.Parse(array[i][num]);
			num++;
			bossMB.Die_Index = int.Parse(array[i][num]);
			num++;
			bossMB.DieType = int.Parse(array[i][num]);
			num++;
			bossMB.DiePos = int.Parse(array[i][num]);
			num++;
			bossMB.DieFX_TimeDelay = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.DieDelay = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.Lie_Index = int.Parse(array[i][num]);
			num++;
			bossMB.LiePos = int.Parse(array[i][num]);
			num++;
			bossMB.Idle_Time_Min = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.Idle_Time_Max = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			bossMB.SO_IdleRate = int.Parse(array[i][num]);
			num++;
			bossMB.SO_AttackRate = int.Parse(array[i][num]);
			num++;
			bossMB.SO_SayRate = int.Parse(array[i][num]);
			num++;
			bossMB.SO_HurtRate = int.Parse(array[i][num]);
			num++;
			bossMB.SO_DieRate = int.Parse(array[i][num]);
			num++;
			if (array[i][num] != null)
			{
				bossMB.SO_Idle.Add(array[i][num]);
			}
			num++;
			if (array[i][num] != null)
			{
				bossMB.SO_Idle.Add(array[i][num]);
			}
			num++;
			if (array[i][num] != null)
			{
				bossMB.SO_Idle.Add(array[i][num]);
			}
			num++;
			bossMB.SO_Walk = array[i][num];
			num++;
			bossMB.SO_AttackA = array[i][num];
			num++;
			bossMB.SO_SayA = array[i][num];
			num++;
			bossMB.SO_AttackB = array[i][num];
			num++;
			bossMB.SO_SayB = array[i][num];
			num++;
			bossMB.SO_AttackC = array[i][num];
			num++;
			bossMB.SO_SayC = array[i][num];
			num++;
			bossMB.SO_AttackD = array[i][num];
			num++;
			bossMB.SO_SayD = array[i][num];
			num++;
			bossMB.SO_AttackE = array[i][num];
			num++;
			bossMB.SO_SayE = array[i][num];
			num++;
			bossMB.SO_Hurt = array[i][num];
			num++;
			bossMB.SO_Die = array[i][num];
			num++;
			bossMB.SO_ChongStart = array[i][num];
			num++;
			bossMB.SO_ChongEnd = array[i][num];
			num++;
			bossMB.SO_Jump = array[i][num];
			num++;
			bossMB.SO_Land = array[i][num];
			num++;
			bossMB.SO_SPC1 = array[i][num];
			num++;
			bossMB.SO_SPC2 = array[i][num];
			num++;
			bossMB.SO_SPC3 = array[i][num];
			num++;
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP = new EM_Skill_SP();
				eM_Skill_SP.IndexName = "AT1";
				num = SetAT(bossMB.AT, eM_Skill_SP, array, i, num);
			}
			else
			{
				num += 98;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP2 = new EM_Skill_SP();
				eM_Skill_SP2.IndexName = "AT2";
				num = SetAT(bossMB.AT, eM_Skill_SP2, array, i, num);
			}
			else
			{
				num += 98;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP3 = new EM_Skill_SP();
				eM_Skill_SP3.IndexName = "AT3";
				num = SetAT(bossMB.AT, eM_Skill_SP3, array, i, num);
			}
			else
			{
				num += 98;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP4 = new EM_Skill_SP();
				eM_Skill_SP4.IndexName = "SK1";
				num = SetSK(bossMB.SK, eM_Skill_SP4, array, i, num);
			}
			else
			{
				num += 105;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP5 = new EM_Skill_SP();
				eM_Skill_SP5.IndexName = "SK2";
				num = SetSK(bossMB.SK, eM_Skill_SP5, array, i, num);
			}
			else
			{
				num += 105;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP6 = new EM_Skill_SP();
				eM_Skill_SP6.IndexName = "SK3";
				num = SetSK(bossMB.SK, eM_Skill_SP6, array, i, num);
			}
			else
			{
				num += 105;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP7 = new EM_Skill_SP();
				eM_Skill_SP7.IndexName = "SK4";
				num = SetSK(bossMB.SK, eM_Skill_SP7, array, i, num);
			}
			else
			{
				num += 105;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				num++;
				EM_Skill_SP eM_Skill_SP8 = new EM_Skill_SP();
				eM_Skill_SP8.IndexName = "SK5";
				num = SetSK(bossMB.SK, eM_Skill_SP8, array, i, num);
			}
			else
			{
				num += 105;
			}
			num++;
			bossMB.SKC = new EM_Skill_CP();
			bossMB.SKC.GlobalID = int.Parse(array[i][num]);
			num++;
			bossMB.SKC.UseAni = int.Parse(array[i][num]);
			num++;
			bossMB.SKC.CPFX = int.Parse(array[i][num]);
			num++;
			bossMB.SKC.FSFXtype = int.Parse(array[i][num]);
			num++;
			bossMB.SK_Die_Index = int.Parse(array[i][num]);
			BossMB.Add(bossMB);
		}
	}

	public static int SetAT(List<EM_Skill_SP> List, EM_Skill_SP dt, string[][] grid, int i, int S)
	{
		dt.UseAni = int.Parse(grid[i][S]);
		S++;
		dt.HitFX = int.Parse(grid[i][S]);
		S++;
		dt.HitFX_Rate = int.Parse(grid[i][S]);
		S++;
		dt.ATFX = int.Parse(grid[i][S]);
		S++;
		dt.StarFX = int.Parse(grid[i][S]);
		S++;
		dt.StarFX_pos = int.Parse(grid[i][S]);
		S++;
		dt.BaTi = false;
		dt.WuDi = false;
		dt.CJY = 0;
		dt.ChongSpeedMulti = 1f;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.Fang = true;
		}
		else
		{
			dt.Fang = false;
		}
		S++;
		dt.ATmod = int.Parse(grid[i][S]);
		S++;
		dt.FStype = int.Parse(grid[i][S]);
		S++;
		dt.FSFXtype = int.Parse(grid[i][S]);
		S++;
		dt.RTtypeOBJ = int.Parse(grid[i][S]);
		S++;
		dt.TypeTar = int.Parse(grid[i][S]);
		S++;
		dt.RTtypeFX = int.Parse(grid[i][S]);
		S++;
		dt.Distance = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range_Hurt = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.MainEL = int.Parse(grid[i][S]);
		S++;
		switch (dt.MainEL)
		{
		case 0:
			dt.damageType = DamageType.fire;
			break;
		case 1:
			dt.damageType = DamageType.frozen;
			break;
		case 2:
			dt.damageType = DamageType.thunder;
			break;
		case 3:
			dt.damageType = DamageType.poison;
			break;
		case 4:
			dt.damageType = DamageType.physics;
			break;
		case 5:
			dt.damageType = DamageType.shadow;
			break;
		}
		dt.ThroughType = int.Parse(grid[i][S]);
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.AttackType = true;
		}
		else
		{
			dt.AttackType = false;
		}
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.AttackTypeA = true;
		}
		else
		{
			dt.AttackTypeA = false;
		}
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.AttackTypeB = true;
		}
		else
		{
			dt.AttackTypeB = false;
		}
		S++;
		dt.Damage = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DamageA = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DamageB = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.SpeedCut = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.BF_DamageAnti = 0f;
		dt.CompAttackSpeed = 0f;
		dt.C_Damage = 0f;
		dt.Reborn = int.Parse(grid[i][S]);
		S++;
		dt.DotRate = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DotDamage = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.BuffTime = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DebuffTime = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.ORB_time = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.EXP_time = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.OBJ = int.Parse(grid[i][S]);
		S++;
		dt.Layer_SubA = int.Parse(grid[i][S]);
		S++;
		dt.Layer_SubB = int.Parse(grid[i][S]);
		S++;
		dt.ORB = int.Parse(grid[i][S]);
		S++;
		dt.ZD_F = int.Parse(grid[i][S]);
		S++;
		dt.ZD_S = int.Parse(grid[i][S]);
		S++;
		dt.ZD_AB = int.Parse(grid[i][S]);
		S++;
		dt.EXP_F = int.Parse(grid[i][S]);
		S++;
		dt.EXP_S = int.Parse(grid[i][S]);
		S++;
		dt.EXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.Dic_F = int.Parse(grid[i][S]);
		S++;
		dt.Dic_S = int.Parse(grid[i][S]);
		S++;
		dt.Sound = int.Parse(grid[i][S]);
		S++;
		dt.Count_ORB = int.Parse(grid[i][S]);
		S++;
		dt.Count_ATtarget = int.Parse(grid[i][S]);
		S++;
		dt.CF_Count = int.Parse(grid[i][S]);
		S++;
		dt.Count_F = int.Parse(grid[i][S]);
		S++;
		dt.Count_S = int.Parse(grid[i][S]);
		S++;
		dt.Count_AB = int.Parse(grid[i][S]);
		S++;
		dt.CountMulti = int.Parse(grid[i][S]);
		S++;
		dt.CountEXP = int.Parse(grid[i][S]);
		S++;
		dt.TypeORB = int.Parse(grid[i][S]);
		S++;
		dt.CF_Type = int.Parse(grid[i][S]);
		S++;
		dt.Type_F = int.Parse(grid[i][S]);
		S++;
		dt.Type_S = int.Parse(grid[i][S]);
		S++;
		dt.Type_AB = int.Parse(grid[i][S]);
		S++;
		dt.TypeDIC_F = int.Parse(grid[i][S]);
		S++;
		dt.TypeDIC_S = int.Parse(grid[i][S]);
		S++;
		dt.TypeEXP_F = int.Parse(grid[i][S]);
		S++;
		dt.TypeEXP_S = int.Parse(grid[i][S]);
		S++;
		dt.TypeEXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.Size = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.High = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.JG = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.AngleA = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.AngleB = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range1 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range2 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range_AT = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.FStime1 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.FStime2 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed1 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed2 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed3 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed4 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Follow_F = int.Parse(grid[i][S]);
		S++;
		dt.Follow_S = int.Parse(grid[i][S]);
		S++;
		dt.AllChuan_F = int.Parse(grid[i][S]);
		S++;
		dt.AllChuan_S = int.Parse(grid[i][S]);
		S++;
		dt.RDSpeed_F = int.Parse(grid[i][S]);
		S++;
		dt.RDSpeed_S = int.Parse(grid[i][S]);
		S++;
		dt.HasFX = int.Parse(grid[i][S]);
		S++;
		dt.S_HasFX = int.Parse(grid[i][S]);
		S++;
		dt.AB_HasFX = int.Parse(grid[i][S]);
		S++;
		dt.colEXP = int.Parse(grid[i][S]);
		S++;
		dt.colEXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.S_colEXP = int.Parse(grid[i][S]);
		S++;
		dt.AB_colEXP = int.Parse(grid[i][S]);
		S++;
		dt.TimeEXP = int.Parse(grid[i][S]);
		S++;
		dt.TimeEXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.EXPpos = int.Parse(grid[i][S]);
		S++;
		dt.EXPpos_AB = int.Parse(grid[i][S]);
		S++;
		dt.S_EXPpos = int.Parse(grid[i][S]);
		S++;
		dt.AB_EXPpos = int.Parse(grid[i][S]);
		S++;
		dt.AngleEXP = int.Parse(grid[i][S]);
		S++;
		dt.AngleEXP_AB = int.Parse(grid[i][S]);
		S++;
		List.Add(dt);
		return S;
	}

	public static int SetSK(List<EM_Skill_SP> List, EM_Skill_SP dt, string[][] grid, int i, int S)
	{
		dt.UseAni = int.Parse(grid[i][S]);
		S++;
		dt.HitFX = int.Parse(grid[i][S]);
		S++;
		dt.HitFX_Rate = int.Parse(grid[i][S]);
		S++;
		dt.ATFX = int.Parse(grid[i][S]);
		S++;
		dt.StarFX = int.Parse(grid[i][S]);
		S++;
		dt.StarFX_pos = int.Parse(grid[i][S]);
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.BaTi = true;
		}
		else
		{
			dt.BaTi = false;
		}
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.WuDi = true;
		}
		else
		{
			dt.WuDi = false;
		}
		S++;
		dt.CJY = int.Parse(grid[i][S]);
		S++;
		dt.ChongSpeedMulti = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.Fang = true;
		}
		else
		{
			dt.Fang = false;
		}
		S++;
		dt.ATmod = int.Parse(grid[i][S]);
		S++;
		dt.FStype = int.Parse(grid[i][S]);
		S++;
		dt.FSFXtype = int.Parse(grid[i][S]);
		S++;
		dt.RTtypeOBJ = int.Parse(grid[i][S]);
		S++;
		dt.TypeTar = int.Parse(grid[i][S]);
		S++;
		dt.RTtypeFX = int.Parse(grid[i][S]);
		S++;
		dt.Distance = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range_Hurt = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.MainEL = int.Parse(grid[i][S]);
		S++;
		switch (dt.MainEL)
		{
		case 0:
			dt.damageType = DamageType.fire;
			break;
		case 1:
			dt.damageType = DamageType.frozen;
			break;
		case 2:
			dt.damageType = DamageType.thunder;
			break;
		case 3:
			dt.damageType = DamageType.poison;
			break;
		case 4:
			dt.damageType = DamageType.physics;
			break;
		case 5:
			dt.damageType = DamageType.shadow;
			break;
		}
		dt.ThroughType = int.Parse(grid[i][S]);
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.AttackType = true;
		}
		else
		{
			dt.AttackType = false;
		}
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.AttackTypeA = true;
		}
		else
		{
			dt.AttackTypeA = false;
		}
		S++;
		if (int.Parse(grid[i][S]) == 0)
		{
			dt.AttackTypeB = true;
		}
		else
		{
			dt.AttackTypeB = false;
		}
		S++;
		dt.Damage = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DamageA = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DamageB = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.SpeedCut = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.BF_DamageAnti = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.CompAttackSpeed = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.C_Damage = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Reborn = int.Parse(grid[i][S]);
		S++;
		dt.DotRate = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DotDamage = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.BuffTime = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.DebuffTime = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.ORB_time = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.EXP_time = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.OBJ = int.Parse(grid[i][S]);
		S++;
		dt.Layer_SubA = int.Parse(grid[i][S]);
		S++;
		dt.Layer_SubB = int.Parse(grid[i][S]);
		S++;
		dt.ORB = int.Parse(grid[i][S]);
		S++;
		dt.ZD_F = int.Parse(grid[i][S]);
		S++;
		dt.ZD_S = int.Parse(grid[i][S]);
		S++;
		dt.ZD_AB = int.Parse(grid[i][S]);
		S++;
		dt.EXP_F = int.Parse(grid[i][S]);
		S++;
		dt.EXP_S = int.Parse(grid[i][S]);
		S++;
		dt.EXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.Dic_F = int.Parse(grid[i][S]);
		S++;
		dt.Dic_S = int.Parse(grid[i][S]);
		S++;
		dt.Sound = int.Parse(grid[i][S]);
		S++;
		dt.Count_ORB = int.Parse(grid[i][S]);
		S++;
		dt.Count_ATtarget = int.Parse(grid[i][S]);
		S++;
		dt.CF_Count = int.Parse(grid[i][S]);
		S++;
		dt.Count_F = int.Parse(grid[i][S]);
		S++;
		dt.Count_S = int.Parse(grid[i][S]);
		S++;
		dt.Count_AB = int.Parse(grid[i][S]);
		S++;
		dt.CountMulti = int.Parse(grid[i][S]);
		S++;
		dt.CountEXP = int.Parse(grid[i][S]);
		S++;
		dt.TypeORB = int.Parse(grid[i][S]);
		S++;
		dt.CF_Type = int.Parse(grid[i][S]);
		S++;
		dt.Type_F = int.Parse(grid[i][S]);
		S++;
		dt.Type_S = int.Parse(grid[i][S]);
		S++;
		dt.Type_AB = int.Parse(grid[i][S]);
		S++;
		dt.TypeDIC_F = int.Parse(grid[i][S]);
		S++;
		dt.TypeDIC_S = int.Parse(grid[i][S]);
		S++;
		dt.TypeEXP_F = int.Parse(grid[i][S]);
		S++;
		dt.TypeEXP_S = int.Parse(grid[i][S]);
		S++;
		dt.TypeEXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.Size = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.High = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.JG = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.AngleA = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.AngleB = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range1 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range2 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Range_AT = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.FStime1 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.FStime2 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed1 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed2 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed3 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Speed4 = float.Parse(grid[i][S], CultureInfo.InvariantCulture);
		S++;
		dt.Follow_F = int.Parse(grid[i][S]);
		S++;
		dt.Follow_S = int.Parse(grid[i][S]);
		S++;
		dt.AllChuan_F = int.Parse(grid[i][S]);
		S++;
		dt.AllChuan_S = int.Parse(grid[i][S]);
		S++;
		dt.RDSpeed_F = int.Parse(grid[i][S]);
		S++;
		dt.RDSpeed_S = int.Parse(grid[i][S]);
		S++;
		dt.HasFX = int.Parse(grid[i][S]);
		S++;
		dt.S_HasFX = int.Parse(grid[i][S]);
		S++;
		dt.AB_HasFX = int.Parse(grid[i][S]);
		S++;
		dt.colEXP = int.Parse(grid[i][S]);
		S++;
		dt.colEXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.S_colEXP = int.Parse(grid[i][S]);
		S++;
		dt.AB_colEXP = int.Parse(grid[i][S]);
		S++;
		dt.TimeEXP = int.Parse(grid[i][S]);
		S++;
		dt.TimeEXP_AB = int.Parse(grid[i][S]);
		S++;
		dt.EXPpos = int.Parse(grid[i][S]);
		S++;
		dt.EXPpos_AB = int.Parse(grid[i][S]);
		S++;
		dt.S_EXPpos = int.Parse(grid[i][S]);
		S++;
		dt.AB_EXPpos = int.Parse(grid[i][S]);
		S++;
		dt.AngleEXP = int.Parse(grid[i][S]);
		S++;
		dt.AngleEXP_AB = int.Parse(grid[i][S]);
		S++;
		List.Add(dt);
		return S;
	}

	public static string[][] LoadTextFile(TextAsset textFile)
	{
		if ((bool)textFile)
		{
			string[] array = textFile.text.Split('\n');
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Split(',');
			}
			return array2.ToArray();
		}
		return null;
	}
}
