using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SK_FSQ_compEM : MonoBehaviour
{
	public float TimeDelay;

	[HideInInspector]
	public int UseType;

	[HideInInspector]
	public int FX;

	[HideInInspector]
	public float offset;

	[HideInInspector]
	public EM_Skill_CP skCP;

	[HideInInspector]
	public int EL;

	[HideInInspector]
	public Enemy em;

	[HideInInspector]
	public int Name_Index;

	private float timeA;

	private float timeB;

	[HideInInspector]
	public bool HasCreat;

	[HideInInspector]
	public GameObject go;

	[HideInInspector]
	public ColorGP GP;

	[HideInInspector]
	public SKprefab PB;

	[HideInInspector]
	public LevelManager LV;

	private bool initialized;

	private void Awake()
	{
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			GP = SingletonMonoScope<GameDataManager>.Instance.colorGP;
			PB = SingletonMonoScope<GameDataManager>.Instance.SKPB;
			LV = SingletonMonoScope<LevelManager>.Instance;
		}
	}

	private void OnEnable()
	{
		HasCreat = false;
		timeA = 0f;
		timeB = 0f;
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		if (!HasCreat)
		{
			timeA += Time.deltaTime;
			if (timeA >= TimeDelay)
			{
				if (UseType == 0)
				{
					SetCompData(skCP);
				}
				else
				{
					SetFSData();
				}
				HasCreat = true;
				timeA = 0f;
			}
		}
		timeB += Time.deltaTime;
		if (timeB >= TimeDelay + 1f)
		{
			timeB = 0f;
			LeanPool.Despawn(go);
			LeanPool.Despawn(this);
		}
	}

	private void LateUpdate()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized && CanInitialize())
		{
			initialized = true;
			SetStart();
		}
	}

	private bool CanInitialize()
	{
		Dicform component = GetComponent<Dicform>();
		if (component != null && component.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		go = LeanPool.Spawn(PB.CPFX[FX].OBJ[em.MainElement], new Vector3(base.transform.position.x, base.transform.position.y + offset, base.transform.position.z), Quaternion.identity, base.transform);
	}

	public void SetCompData(EM_Skill_CP skill)
	{
		EnemyMB eMMB = GetEMMB(skill.GlobalID);
		Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.EMPB.Enemy[eMMB.IndexA].Enemy[eMMB.IndexB], new Vector3(base.transform.position.x, base.transform.position.y + 0.02f, base.transform.position.z), Quaternion.identity).GetComponent<Enemy>();
		component.GlobalID = eMMB.GlobalID;
		component.Quality = 0;
		component.Level = em.Level;
		component.IndexName = eMMB.IndexName[Name_Index];
		component.Xp = Mathf.FloorToInt(eMMB.Xp * Mathf.Pow(LV.XPmulti, component.Level) * 0.5f);
		component.size = eMMB.size;
		component.CompOffset = eMMB.CompOffset;
		component.TuiSpeed = eMMB.TuiSpeed;
		component.ItemDropPos = eMMB.ItemDropPos;
		component.MainElement = skill.MainElement;
		switch (component.MainElement)
		{
		case 0:
			component.MainELType = DamageType.fire;
			break;
		case 1:
			component.MainELType = DamageType.frozen;
			break;
		case 2:
			component.MainELType = DamageType.thunder;
			break;
		case 3:
			component.MainELType = DamageType.poison;
			break;
		case 4:
			component.MainELType = DamageType.physics;
			break;
		case 5:
			component.MainELType = DamageType.shadow;
			break;
		}
		component.ColorIndex = eMMB.ColorIndex;
		component.SpineType = eMMB.SpineType;
		if (component.SpineType == 0)
		{
			EnemyColorDT enemyColorDT = GP.GP[component.ColorIndex].XI[component.MainElement].CL[skill.RDcolor];
			SkeletonAnimation spine = component.spine;
			if (enemyColorDT.ChangeSK)
			{
				Skin skin = new Skin("skin");
				skin.Clear();
				skin.AddSkin(spine.Skeleton.Data.FindSkin(enemyColorDT.SkinName));
				spine.Skeleton.SetSkin(skin);
				spine.Skeleton.SetSlotsToSetupPose();
			}
			component.SkinName = enemyColorDT.SkinName;
			component.Flip = enemyColorDT.Flip;
			component.MainMix = enemyColorDT.MainMix;
			component.MainHue = enemyColorDT.MainHue;
			component.MainSat = enemyColorDT.MainSat;
			component.MainColor = enemyColorDT.MainColor;
			component.DisloveColor = enemyColorDT.DisloveColor;
			component.AlphaColor = enemyColorDT.AlphaColor;
			component.DieColor = enemyColorDT.DieColor;
			component.RDcolor = skill.RDcolor;
		}
		else
		{
			component.SetSpiritColor(component.MainElement);
			component.DieColor = component.MainElement;
		}
		if (component.FXsustain != null)
		{
			component.FXsustain.SetColor(component.MainElement);
		}
		component.EnemyType = eMMB.EnemyType;
		component.Health_Base = Mathf.Floor(eMMB.Health * Mathf.Pow(LV.HealthMulti, component.Level) * LevelManager.GetEnemyHealthCurveMultiplier(component.Level) * HealthMulti(em.Quality));
		component.Health_Bei = 0f;
		component.AttackSpeed_JG = eMMB.AttackSpeed_JG;
		component.AttackSpeed_Base = eMMB.ATSpeed;
		component.AttackSpeed_Bei = 0f;
		component.MoveSpeed_Base = eMMB.MVSpeed;
		component.MoveSpeed_Bei = 0f;
		component.Damage_Base = Mathf.Floor(eMMB.Damage * Mathf.Pow(LV.DamageMulti, component.Level) * DamageMulti(em.Quality));
		component.Damage_Bei = 0f;
		component.FireAnti = LevelManager.GetAnti();
		component.FrozenAnti = LevelManager.GetAnti();
		component.ThunderAnti = LevelManager.GetAnti();
		component.PoisonAnti = LevelManager.GetAnti();
		component.PhysicsAnti = LevelManager.GetAnti();
		component.ShadowAnti = LevelManager.GetAnti();
		component.Chuan = LevelManager.GetChuan();
		component.DamageAnti = LevelManager.GetDMG_Anti();
		component.FlySpeed = 0f;
		component.Range_Base = eMMB.Range_Base;
		component.Range_Anger = eMMB.Range_Anger;
		component.Range_Far = eMMB.Range_Far;
		component.SK_Rate = eMMB.SK_Rate;
		component.SK_Rate_Comp = 0;
		component.SK_Rate_FS = 0;
		component.SK_Rate_ELSS = 0;
		component.Can_DieBoom = false;
		component.FSDie_Index = eMMB.FSDie_Index;
		component.Idle_Time_Min = eMMB.Idle_Time_Min;
		component.Idle_Time_Max = eMMB.Idle_Time_Max;
		component.SO_IdleRate = eMMB.SO_IdleRate;
		component.SO_AttackRate = eMMB.SO_AttackRate;
		component.SO_SayRate = eMMB.SO_SayRate;
		component.SO_HurtRate = eMMB.SO_HurtRate;
		component.SO_DieRate = eMMB.SO_DieRate;
		component.SO_Idle = eMMB.SO_Idle;
		component.SO_Walk = eMMB.SO_Walk;
		component.SO_AttackA = eMMB.SO_AttackA;
		component.SO_SayA = eMMB.SO_SayA;
		component.SO_AttackB = eMMB.SO_AttackB;
		component.SO_SayB = eMMB.SO_SayB;
		component.SO_AttackC = eMMB.SO_AttackC;
		component.SO_SayC = eMMB.SO_SayC;
		component.SO_Hurt = eMMB.SO_Hurt;
		component.SO_Die = eMMB.SO_Die;
		component.SO_ChuiDi = eMMB.SO_ChuiDi;
		component.IS_Boss = false;
		component.CF_Rate = 0;
		if (em.IS_Boss)
		{
			if (component.Level <= 30)
			{
				component.SK_AT = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.AT1].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.AT1].SK.Count)];
			}
			else if (component.Level > 30 && component.Level <= 60)
			{
				component.SK_AT = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.AT2].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.AT2].SK.Count)];
			}
			else
			{
				component.SK_AT = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.AT3].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.AT3].SK.Count)];
			}
			component.AT_Ani = eMMB.AT_Ani;
			component.AT_Fang = eMMB.AT_Fang;
			component.AT_Distans = eMMB.AT_Distans * SWS.DistanceRandom(eMMB.AT_Distans);
			if (component.Level <= 20)
			{
				component.SK_A = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK1].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK1].SK.Count)];
			}
			else if (component.Level > 20 && component.Level <= 40)
			{
				component.SK_A = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK2].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK2].SK.Count)];
			}
			else if (component.Level > 40 && component.Level <= 60)
			{
				component.SK_A = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK3].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK3].SK.Count)];
			}
			else if (component.Level > 60 && component.Level <= 80)
			{
				component.SK_A = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK4].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK4].SK.Count)];
			}
			else
			{
				component.SK_A = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK5].SK[Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[eMMB.SK5].SK.Count)];
			}
			component.SK_Ani = eMMB.SK_Ani;
			component.SK_Fang = eMMB.SK_Fang;
			component.SK_Distans = eMMB.SK_Distans * SWS.DistanceRandom(eMMB.SK_Distans);
		}
		else
		{
			component.SK_AT = em.SK_AT;
			component.AT_Ani = em.AT_Ani;
			component.AT_Fang = em.AT_Fang;
			component.AT_Distans = em.AT_Distans * SWS.DistanceRandom(em.AT_Distans);
			component.SK_A = em.SK_A;
			component.SK_Ani = em.SK_Ani;
			component.SK_Fang = em.SK_Fang;
			component.SK_Distans = em.SK_Distans * SWS.DistanceRandom(em.SK_Distans);
		}
		component.SK_Comp = null;
		component.SK_FS = null;
		component.SK_Die = null;
		component.SK_ELSS = null;
		for (int i = 0; i < 5; i++)
		{
			component.SSIndex[i] = 0;
		}
		component.IS_Comp = true;
		component.IS_FS = false;
		component.Father = em;
		em.cpList.Add(component);
	}

	public void SetFSData()
	{
		EnemyMB eMMB = GetEMMB(em.GlobalID);
		Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.EMPB.Enemy[eMMB.IndexA].Enemy[eMMB.IndexB], new Vector3(base.transform.position.x, base.transform.position.y + 0.02f, base.transform.position.z), Quaternion.identity).GetComponent<Enemy>();
		component.GlobalID = em.GlobalID;
		component.Quality = em.Quality;
		component.IndexName = em.IndexName;
		component.Level = em.Level;
		float num = em.Xp;
		component.Xp = Mathf.FloorToInt(num / 10f);
		component.size = em.size;
		component.CompOffset = em.CompOffset;
		component.TuiSpeed = em.TuiSpeed;
		component.ItemDropPos = em.ItemDropPos;
		component.MainElement = EL;
		switch (component.MainElement)
		{
		case 0:
			component.MainELType = DamageType.fire;
			break;
		case 1:
			component.MainELType = DamageType.frozen;
			break;
		case 2:
			component.MainELType = DamageType.thunder;
			break;
		case 3:
			component.MainELType = DamageType.poison;
			break;
		case 4:
			component.MainELType = DamageType.physics;
			break;
		case 5:
			component.MainELType = DamageType.shadow;
			break;
		}
		component.ColorIndex = em.ColorIndex;
		component.SpineType = em.SpineType;
		if (component.SpineType == 0)
		{
			EnemyColorDT enemyColorDT = GP.GP[eMMB.ColorIndex].XI[component.MainElement].CL[em.RDcolor];
			SkeletonAnimation spine = em.spine;
			if (enemyColorDT.ChangeSK)
			{
				Skin skin = new Skin("skin");
				skin.Clear();
				skin.AddSkin(spine.Skeleton.Data.FindSkin(enemyColorDT.SkinName));
				spine.Skeleton.SetSkin(skin);
				spine.Skeleton.SetSlotsToSetupPose();
			}
			component.SkinName = enemyColorDT.SkinName;
			component.Flip = enemyColorDT.Flip;
			component.MainMix = enemyColorDT.MainMix;
			component.MainHue = enemyColorDT.MainHue;
			component.MainSat = enemyColorDT.MainSat;
			component.MainColor = enemyColorDT.MainColor;
			component.DisloveColor = enemyColorDT.DisloveColor;
			component.AlphaColor = enemyColorDT.AlphaColor;
			component.DieColor = enemyColorDT.DieColor;
			component.RDcolor = em.RDcolor;
		}
		else
		{
			component.SetSpiritColor(component.MainElement);
			em.DieColor = em.MainElement;
		}
		if (component.FXsustain != null)
		{
			component.FXsustain.SetColor(component.MainElement);
		}
		component.EnemyType = em.EnemyType;
		component.Health_Base = em.Health_Max / 3f;
		component.Health_Bei = 0f;
		component.AT_Idle_Min = em.AT_Idle_Min;
		component.AT_Idle_Max = em.AT_Idle_Max;
		component.AttackSpeed_JG = em.AttackSpeed_JG;
		component.AttackSpeed_Base = em.AttackSpeed_Max;
		component.AttackSpeed_Bei = 0f;
		component.MoveSpeed_Base = em.MoveSpeed_Max;
		component.MoveSpeed_Bei = 0f;
		component.BJRate = em.BJRate;
		component.GeDang = em.GeDang;
		component.yunAnti = em.yunAnti;
		component.Damage_Base = em.Damage_Max;
		component.Damage_Bei = 0f;
		component.FireAnti = em.FireAnti;
		component.FrozenAnti = em.FrozenAnti;
		component.ThunderAnti = em.ThunderAnti;
		component.PoisonAnti = em.PoisonAnti;
		component.PhysicsAnti = em.PhysicsAnti;
		component.ShadowAnti = em.ShadowAnti;
		component.Through = em.Through;
		component.Chuan = em.Chuan;
		component.Health_Prc = em.Health_Prc;
		component.DamageAnti = em.DamageAnti;
		component.FlySpeed = em.FlySpeed;
		component.DotDamage = em.DotDamage;
		component.DotTime = em.DotTime;
		component.AntiSlow = em.AntiSlow;
		component.DotTimeCut = em.DotTimeCut;
		component.Comp_EveryCount = em.Comp_EveryCount;
		component.Comp_Count = em.Comp_Count;
		component.FS_EveryCount = em.FS_EveryCount;
		component.FS_Count = em.FS_Count;
		component.Range_Base = em.Range_Base;
		component.Range_Anger = em.Range_Anger;
		component.Range_Far = em.Range_Far;
		component.Range_ATplayer_multi = em.Range_ATplayer_multi;
		component.SK_Rate = em.SK_Rate;
		component.SK_Rate_Comp = 0;
		component.SK_Rate_FS = 0;
		component.SK_Rate_ELSS = 0;
		component.Can_DieBoom = false;
		component.FSDie_Index = eMMB.FSDie_Index;
		component.CanSO_Idle = em.CanSO_Idle;
		component.Idle_Time_Min = em.Idle_Time_Min;
		component.Idle_Time_Max = em.Idle_Time_Max;
		component.SO_IdleRate = em.SO_IdleRate;
		component.SO_AttackRate = em.SO_AttackRate;
		component.SO_SayRate = em.SO_SayRate;
		component.SO_HurtRate = em.SO_HurtRate;
		component.SO_DieRate = em.SO_DieRate;
		component.SO_Idle = em.SO_Idle;
		component.SO_Walk = em.SO_Walk;
		component.SO_AttackA = em.SO_AttackA;
		component.SO_SayA = em.SO_SayA;
		component.SO_AttackB = em.SO_AttackB;
		component.SO_SayB = em.SO_SayB;
		component.SO_AttackC = em.SO_AttackC;
		component.SO_SayC = em.SO_SayC;
		component.SO_Hurt = em.SO_Hurt;
		component.SO_Die = em.SO_Die;
		component.IS_Boss = false;
		component.CF_Rate = 0;
		component.SK_AT = em.SK_AT;
		component.AT_Ani = em.AT_Ani;
		component.AT_Fang = em.AT_Fang;
		component.AT_Distans = em.AT_Distans;
		component.SK_A = em.SK_A;
		component.SK_Ani = em.SK_Ani;
		component.SK_Fang = em.SK_Fang;
		component.SK_Distans = em.SK_Distans;
		component.SK_Comp = null;
		component.SK_FS = null;
		component.SK_Die = null;
		component.SK_ELSS = null;
		for (int i = 0; i < 5; i++)
		{
			component.SSIndex[i] = 0;
		}
		component.IS_Comp = false;
		component.IS_FS = true;
		component.Father = em;
		em.fsList.Add(component);
	}

	public static EnemyMB GetEMMB(int id)
	{
		foreach (EnemyMB item in SingletonMonoScope<GameDataManager>.Instance.EMMB)
		{
			if (item.GlobalID == id)
			{
				return item;
			}
		}
		return null;
	}

	public float HealthMulti(int Quality)
	{
		switch (Quality)
		{
		case 0:
		case 1:
			return 1f;
		case 2:
			return 1.5f;
		case 3:
			return 2f;
		case 4:
			return 6f;
		case 5:
			return 10f;
		default:
			return 1f;
		}
	}

	public float DamageMulti(int Quality)
	{
		switch (Quality)
		{
		case 0:
		case 1:
			return 1f;
		case 2:
			return 1.2f;
		case 3:
			return 1.5f;
		case 4:
			return 1.8f;
		case 5:
			return 2f;
		default:
			return 1f;
		}
	}
}
