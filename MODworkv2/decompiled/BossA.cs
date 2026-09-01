using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using Pathfinding;
using Spine;
using Spine.Unity;
using UnityEngine;

public class BossA : MonoBehaviour
{
	private static readonly int mainAlpha = Shader.PropertyToID("_MainAlpha");

	private const float BossTargetPriorityMultiBChance = 0.2f;

	[HideInInspector]
	public GameObject tar;

	[HideInInspector]
	public GameObject tar2;

	[HideInInspector]
	public SkeletonAnimation spine;

	[HideInInspector]
	public AIDestinationSetter AIDS;

	public Transform ATpoint;

	public Transform ATpoint2;

	public Transform ATpoint3;

	public Transform ATpoint4;

	public Transform ATpoint5;

	public Transform ATpoint6;

	public Transform ATpoint7;

	public Transform ATpoint8;

	[HideInInspector]
	public PlayerManager playerManager;

	[HideInInspector]
	public Enemy em;

	private BossAstat st;

	[HideInInspector]
	public SkeletonAnimation ani;

	private Spine.AnimationState stat;

	private TrackEntry currentActionTrack;

	private TrackEntry currentHurtTrack;

	private bool actionCompleteHandled;

	[SpineEvent("", "", true, false, false)]
	public string[] SPevent;

	[SpineAnimation("", "", true, false)]
	public string idle;

	[SpineAnimation("", "", true, false)]
	public string walk;

	[SpineAnimation("", "", true, false)]
	public string hurt;

	[SpineAnimation("", "", true, false)]
	public string die;

	[SpineAnimation("", "", true, false)]
	public string[] attack;

	[HideInInspector]
	public float JStime;

	[HideInInspector]
	public float JStimeA;

	[HideInInspector]
	public SKprefab PB;

	private bool StartOK;

	private float ATJG;

	public bool atCD => Mathf.Approximately(JStime, em.AttackSpeed_JG_Last);

	private void Awake()
	{
		em = GetComponent<Enemy>();
		ani = base.transform.Find("main/Spine").gameObject.GetComponent<SkeletonAnimation>();
		ATpoint = base.transform.Find("main/Spine/AT");
		ATpoint2 = base.transform.Find("main/Spine/AT/AT2");
		ATpoint3 = base.transform.Find("main/Spine/AT3");
		ATpoint4 = base.transform.Find("main/Spine/AT3/AT4");
		stat = ani.AnimationState;
		stat.Event += OnUserDefinedEvent;
		playerManager = SingletonMonoScope<PlayerManager>.Instance;
		AIDS = GetComponent<AIDestinationSetter>();
		spine = base.transform.Find("main/Spine").GetComponent<SkeletonAnimation>();
		em.spine = spine;
		tar = base.transform.Find("main/Spine/SkeletonUtility-SkeletonRoot/tar").gameObject;
		tar2 = base.transform.Find("main/Spine/SkeletonUtility-SkeletonRoot/tar2").gameObject;
		em.mat = ani.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
		em.SpineRender = base.transform.Find("main/Spine").gameObject.GetComponent<MeshRenderer>();
		em.mpb = new MaterialPropertyBlock();
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			PB = SingletonMonoScope<GameDataManager>.Instance.SKPB;
		}
	}

	private void OnEnable()
	{
		JStimeA = 0f;
		ClearTrackCompleteBindings();
		StartOK = false;
		this.wait(1E-06f, SetStart);
	}

	private void OnDisable()
	{
		ClearTrackCompleteBindings();
	}

	private void Update()
	{
		if (!StartOK)
		{
			return;
		}
		if (em.IsAlive)
		{
			if (!em.IsYun)
			{
				FangXiang();
			}
			JStimeA += Time.deltaTime;
			if (JStimeA >= 0.1f)
			{
				em.path.maxSpeed = em.MoveSpeed_Path * em.MoveSpeed_Last;
				em.Fighting();
				JStimeA = 0f;
			}
			JStime += Time.deltaTime;
			if (JStime >= em.AttackSpeed_JG_Last)
			{
				JStime = em.AttackSpeed_JG_Last;
			}
			ATJG += Time.deltaTime;
			if (ATJG >= 6f)
			{
				ChangeFarSkill();
				ATJG = 0f;
			}
		}
		st.Update();
	}

	public void SetStart()
	{
		StartOK = true;
		JStime = em.AttackSpeed_JG_Last;
		ChangeState(new BSA_idle());
	}

	public void FangXiang()
	{
		if (!em.IS_Frozen && !em.IsFang)
		{
			if (em.path.steeringTarget.x - base.transform.position.x > 0f)
			{
				spine.skeleton.ScaleX = 1f;
			}
			else
			{
				spine.skeleton.ScaleX = -1f;
			}
		}
	}

	public void OnUserDefinedEvent(TrackEntry trackEntry, Spine.Event e)
	{
		if (e.Data.Name == SPevent[0])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[1])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[2])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[3])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[4])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[5])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[6])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[7])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[8])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[9])
		{
			UseAT();
		}
		if (e.Data.Name == SPevent[10])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[11])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[12])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[13])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[14])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[15])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[16])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[17])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[18])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[19])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[20] && em.BS.SO_AttackA != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackA, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[21] && em.BS.SO_AttackB != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackB, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[22] && em.BS.SO_AttackC != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackC, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[23] && em.BS.SO_AttackD != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackD, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[24] && em.BS.SO_AttackE != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackE, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[25] && em.BS.SO_AttackA != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackA, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[26] && em.BS.SO_AttackB != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackB, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[27] && em.BS.SO_AttackC != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackC, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[28] && em.BS.SO_AttackC != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackC, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[29] && em.BS.SO_AttackC != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_AttackC, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[30] && em.BS.SO_SayA != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_SayA, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[31] && em.BS.SO_SayB != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_SayB, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[32] && em.BS.SO_SayC != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_SayC, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[33] && em.BS.SO_SayD != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_SayD, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[34] && em.BS.SO_SayE != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_SayE, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[35] && em.SO_Walk != null)
		{
			RuntimeManager.PlayOneShot(em.SO_Walk, em.transform.position);
		}
		if (e.Data.Name == SPevent[36])
		{
			ani.AnimationState.SetEmptyAnimation(1, 0f);
			ChangeState(new BSA_idle());
		}
		if (e.Data.Name == SPevent[37])
		{
			UseAT();
			em.path.canMove = true;
			if (em.BS.SO_ChongStart != null)
			{
				RuntimeManager.PlayOneShot(em.BS.SO_ChongStart, em.yao.transform.position);
			}
		}
		if (e.Data.Name == SPevent[38])
		{
			em.ChongSpeedMulti = 1f;
			if (em.BS.SO_ChongEnd != null)
			{
				RuntimeManager.PlayOneShot(em.BS.SO_ChongEnd, em.yao.transform.position);
			}
		}
		if (e.Data.Name == SPevent[39])
		{
			em.path.canMove = true;
			if (em.BS.SO_Jump != null)
			{
				RuntimeManager.PlayOneShot(em.BS.SO_Jump, em.yao.transform.position);
			}
		}
		if (e.Data.Name == SPevent[40])
		{
			UseAT();
			em.path.canMove = false;
			em.ChongSpeedMulti = 1f;
			if (em.BS.SO_Land != null)
			{
				RuntimeManager.PlayOneShot(em.BS.SO_Land, em.yao.transform.position);
			}
		}
		if (e.Data.Name == SPevent[41])
		{
			em.mpb.SetFloat(mainAlpha, 0f);
			em.SpineRender.SetPropertyBlock(em.mpb);
			em.OffBuffFX();
			em.path.canMove = true;
		}
		if (e.Data.Name == SPevent[42])
		{
			em.mpb.SetFloat(mainAlpha, 1f);
			em.SpineRender.SetPropertyBlock(em.mpb);
			em.OnBuffFX();
			em.IsYS = false;
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.CPFX[0].OBJ[em.MainElement], em.yao.transform.position, Quaternion.identity);
		}
		if (e.Data.Name == SPevent[43])
		{
			if (em.BS.SO_SPC1 != null)
			{
				RuntimeManager.PlayOneShot(em.BS.SO_SPC1, em.yao.transform.position);
			}
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.ATFX[105].OBJ[em.MainElement], em.yao.transform.position, Quaternion.identity, em.yao.transform);
		}
		if (e.Data.Name == SPevent[44] && em.BS.SO_SPC2 != null)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_SPC2, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[45] && em.BS.SO_SPC3 != null)
		{
			RuntimeManager.PlayOneShot(em.BS.SO_SPC3, em.yao.transform.position);
		}
	}

	public void UseAT()
	{
		switch (em.BS.SK_Cur_IndexA)
		{
		case 0:
			if (em.BS.AT[em.BS.SK_Cur_IndexB].ATmod == 0)
			{
				if (!em.BS.AttackLost)
				{
					SetAT_DataJZ(em.BS.AT[em.BS.SK_Cur_IndexB]);
				}
			}
			else
			{
				SetAT_Data(em.BS.AT[em.BS.SK_Cur_IndexB]);
			}
			break;
		case 1:
			if (em.BS.SK[em.BS.SK_Cur_IndexB].ATmod == 0)
			{
				if (!em.BS.AttackLost)
				{
					SetAT_DataJZ(em.BS.SK[em.BS.SK_Cur_IndexB]);
				}
			}
			else
			{
				SetAT_Data(em.BS.SK[em.BS.SK_Cur_IndexB]);
			}
			break;
		case 2:
			SetCP_Data(em.BS.SKC);
			break;
		}
	}

	public void UseFX()
	{
		switch (em.BS.SK_Cur_IndexA)
		{
		case 0:
			if (em.BS.AT[em.BS.SK_Cur_IndexB].StarFX > 0)
			{
				EM_Skill_SP eM_Skill_SP2 = em.BS.AT[em.BS.SK_Cur_IndexB];
				GameObject prefab2 = PB.StartFX[eM_Skill_SP2.StarFX].OBJ[eM_Skill_SP2.MainEL];
				switch (eM_Skill_SP2.StarFX_pos)
				{
				case 0:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint.transform.position, Quaternion.identity);
					break;
				case 1:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint3.transform.position, Quaternion.identity);
					break;
				case 2:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint5.transform.position, Quaternion.identity);
					break;
				case 3:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint7.transform.position, Quaternion.identity);
					break;
				case 4:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint.transform.position, Quaternion.identity, ATpoint.transform);
					break;
				case 5:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint3.transform.position, Quaternion.identity, ATpoint3.transform);
					break;
				case 6:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint5.transform.position, Quaternion.identity, ATpoint5.transform);
					break;
				case 7:
					em.UseFX_OBJ = LeanPool.Spawn(prefab2, ATpoint7.transform.position, Quaternion.identity, ATpoint7.transform);
					break;
				}
			}
			break;
		case 1:
			if (em.BS.SK[em.BS.SK_Cur_IndexB].StarFX > 0)
			{
				EM_Skill_SP eM_Skill_SP = em.BS.SK[em.BS.SK_Cur_IndexB];
				GameObject prefab = PB.StartFX[eM_Skill_SP.StarFX].OBJ[eM_Skill_SP.MainEL];
				switch (eM_Skill_SP.StarFX_pos)
				{
				case 0:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint.transform.position, Quaternion.identity);
					break;
				case 1:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint3.transform.position, Quaternion.identity);
					break;
				case 2:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint5.transform.position, Quaternion.identity);
					break;
				case 3:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint7.transform.position, Quaternion.identity);
					break;
				case 4:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint.transform.position, Quaternion.identity, ATpoint.transform);
					break;
				case 5:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint3.transform.position, Quaternion.identity, ATpoint3.transform);
					break;
				case 6:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint5.transform.position, Quaternion.identity, ATpoint5.transform);
					break;
				case 7:
					em.UseFX_OBJ = LeanPool.Spawn(prefab, ATpoint7.transform.position, Quaternion.identity, ATpoint7.transform);
					break;
				}
			}
			break;
		}
	}

	public void SetAT_Data(EM_Skill_SP dt)
	{
		Transform transform = ((dt.TypeTar != 0) ? em.MVTarget : em.ATTarget);
		SkillOBJ_DT_SP component;
		Transform transform2;
		switch (dt.FStype)
		{
		case 0:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint;
			break;
		case 1:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], ATpoint3.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint3;
			break;
		case 2:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], ATpoint5.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint5;
			break;
		case 3:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], ATpoint7.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint7;
			break;
		case 4:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], em.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.transform;
			break;
		case 5:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], em.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.yao.transform;
			break;
		case 6:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], em.transform.position, Quaternion.identity, em.transform).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.transform;
			break;
		case 7:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.yao.transform;
			break;
		case 8:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], em.headUp.transform.position, Quaternion.identity, em.headUp.transform).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.yao.transform;
			break;
		case 9:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], em.MVTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = base.transform;
			break;
		case 10:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], em.ATTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = base.transform;
			break;
		default:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[dt.MainEL], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint;
			break;
		}
		if (dt.RTtypeOBJ == 0)
		{
			Vector3 vector = transform.position - transform2.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			component.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
		component.indexType = 2;
		component.em = em;
		component.ZY = false;
		component.Dot_Infect = false;
		component.Dot_Infect_Layer = 0;
		component.TargetPos = transform.position;
		component.skillName = dt.IndexName;
		component.dic = transform.position - transform2.position;
		component.RTtypeOBJ = dt.RTtypeOBJ;
		component.RTtypeFX = dt.RTtypeFX;
		component.Distance = dt.Distance;
		component.GlobalID = 100000;
		component.damageType = dt.damageType;
		component.MainEL = dt.MainEL;
		component.ThroughType = dt.ThroughType;
		component.AttackType = dt.AttackType;
		component.AttackTypeA = dt.AttackTypeA;
		component.AttackTypeB = dt.AttackTypeB;
		component.Damage = dt.Damage / 100f * em.Damage_Last;
		component.DamageA = dt.DamageA;
		component.DamageB = dt.DamageB;
		component.BJrate = em.BJRate;
		component.Through = em.Through;
		component.FlySpeed = em.FlySpeed;
		component.Chuan = em.Chuan;
		component.MoveSpeedCut = dt.SpeedCut;
		component.AttackSpeedCut = dt.SpeedCut;
		component.BF_EL_Chuan = 0f;
		component.BF_BJrate = 0f;
		component.BF_GeDang = 0f;
		component.BF_DamageAnti = dt.BF_DamageAnti;
		component.C_Damage = dt.C_Damage;
		component.C_ATspeed = dt.CompAttackSpeed;
		component.C_MVspeed = 0f;
		component.C_Health_Prc = 0f;
		component.BF_Through = 0f;
		component.CF_Rate = em.CF_Rate;
		component.ChangeSkin = 1;
		component.SkinIndex = 0;
		component.Reborn = dt.Reborn;
		component.DotRate = dt.DotRate;
		component.DotDamage = dt.DotDamage;
		component.NoTime = 1;
		component.BuffTime = dt.BuffTime;
		component.DebuffTime = dt.DebuffTime;
		component.Field_time = 0f;
		component.ORB_time = dt.ORB_time;
		component.EXP_time = dt.EXP_time;
		component.ZD_time_F = 0f;
		component.ZD_time_S = 0f;
		component.ORB = dt.ORB;
		component.ZD_F = dt.ZD_F;
		component.ZD_S = dt.ZD_S;
		component.ZD_AB = dt.ZD_AB;
		component.EXP_F = dt.EXP_F;
		component.EXP_S = dt.EXP_S;
		component.EXP_AB = dt.EXP_AB;
		component.Dic_F = dt.Dic_F;
		component.Dic_S = dt.Dic_S;
		component.FX_F = 0;
		component.FX_S = 0;
		component.Sound = dt.Sound;
		component.Count_ORB = dt.Count_ORB;
		component.Count_ATtarget = dt.Count_ATtarget;
		component.CF_Count = dt.CF_Count;
		component.Count_F = dt.Count_F;
		component.Count_S = dt.Count_S;
		component.Count_AB = dt.Count_AB;
		component.CountMulti = dt.CountMulti;
		component.CountEXP = dt.CountEXP;
		component.TypeORB = dt.TypeORB;
		component.CF_Type = dt.CF_Type;
		component.Type_F = dt.Type_F;
		component.Type_S = dt.Type_S;
		component.Type_AB = dt.Type_AB;
		component.TypeDIC_F = dt.TypeDIC_F;
		component.TypeDIC_S = dt.TypeDIC_S;
		component.TypeEXP_F = dt.TypeEXP_F;
		component.TypeEXP_S = dt.TypeEXP_S;
		component.TypeEXP_AB = dt.TypeEXP_AB;
		component.Size = dt.Size;
		component.High = dt.High;
		component.JG = dt.JG;
		component.AngleA = dt.AngleA;
		component.AngleB = dt.AngleB;
		component.Range1 = dt.Range1;
		component.Range2 = dt.Range2;
		component.Range_AT = dt.Range_AT;
		component.FStime1 = dt.FStime1;
		component.FStime2 = dt.FStime2;
		component.Speed1 = dt.Speed1;
		component.Speed2 = dt.Speed2;
		component.Speed3 = dt.Speed3;
		component.Speed4 = dt.Speed4;
		component.Follow_F = dt.Follow_F;
		component.Follow_S = dt.Follow_S;
		component.AllChuan_F = dt.AllChuan_F;
		component.AllChuan_S = dt.AllChuan_S;
		component.Slow_F = 1;
		component.Slow_S = 1;
		component.RDSpeed_F = dt.RDSpeed_F;
		component.RDSpeed_S = dt.RDSpeed_S;
		component.HasFX = dt.HasFX;
		component.S_HasFX = dt.S_HasFX;
		component.AB_HasFX = dt.AB_HasFX;
		component.colEXP = dt.colEXP;
		component.colEXP_A = dt.colEXP_AB;
		component.S_colEXP = dt.S_colEXP;
		component.AB_colEXP = dt.AB_colEXP;
		component.TimeEXP = dt.TimeEXP;
		component.TimeEXP_AB = dt.TimeEXP_AB;
		component.LastEXP = 1;
		component.LastEXP_AB = 1;
		component.S_LastEXP = 1;
		component.AB_LastEXP = 1;
		component.EXPpos = dt.EXPpos;
		component.EXPpos_AB = dt.EXPpos_AB;
		component.S_EXPpos = dt.S_EXPpos;
		component.AB_EXPpos = dt.AB_EXPpos;
		component.AngleEXP = dt.AngleEXP;
		component.AngleEXP_AB = dt.AngleEXP_AB;
		if (dt.ATFX > 0)
		{
			Transform transform3;
			Transform transform4;
			switch (dt.FSFXtype)
			{
			case 0:
				transform3 = ATpoint;
				transform4 = ATpoint2;
				break;
			case 1:
				transform3 = ATpoint3;
				transform4 = ATpoint4;
				break;
			case 2:
				transform3 = ATpoint5;
				transform4 = ATpoint6;
				break;
			case 3:
				transform3 = ATpoint7;
				transform4 = ATpoint8;
				break;
			default:
				transform3 = ATpoint;
				transform4 = ATpoint2;
				break;
			}
			Vector3 vector2 = transform4.position - transform3.position;
			float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			GameObject gameObject = LeanPool.Spawn(PB.ATFX[dt.ATFX].OBJ[dt.MainEL], transform3.transform.position, Quaternion.identity);
			if (dt.RTtypeFX == 0)
			{
				gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z2);
			}
		}
	}

	public void SetAT_DataJZ(EM_Skill_SP dt)
	{
		SkillOBJ_DT_SP skillOBJ_DT_SP = tar2.AddComponent<SkillOBJ_DT_SP>();
		skillOBJ_DT_SP.indexType = 2;
		skillOBJ_DT_SP.em = em;
		skillOBJ_DT_SP.ZY = false;
		skillOBJ_DT_SP.Dot_Infect = false;
		skillOBJ_DT_SP.Dot_Infect_Layer = 0;
		skillOBJ_DT_SP.TargetPos = Vector3.zero;
		skillOBJ_DT_SP.skillName = null;
		skillOBJ_DT_SP.dic = Vector3.zero;
		skillOBJ_DT_SP.RTtypeOBJ = dt.RTtypeOBJ;
		skillOBJ_DT_SP.RTtypeFX = dt.RTtypeFX;
		skillOBJ_DT_SP.Distance = dt.Distance;
		skillOBJ_DT_SP.GlobalID = 100000;
		skillOBJ_DT_SP.damageType = dt.damageType;
		skillOBJ_DT_SP.MainEL = dt.MainEL;
		skillOBJ_DT_SP.ThroughType = dt.ThroughType;
		skillOBJ_DT_SP.AttackType = dt.AttackType;
		skillOBJ_DT_SP.AttackTypeA = dt.AttackTypeA;
		skillOBJ_DT_SP.AttackTypeB = dt.AttackTypeB;
		skillOBJ_DT_SP.Damage = dt.Damage / 100f * em.Damage_Last;
		skillOBJ_DT_SP.DamageA = dt.DamageA / 100f * em.Damage_Last;
		skillOBJ_DT_SP.DamageB = dt.DamageB / 100f * em.Damage_Last;
		skillOBJ_DT_SP.BJrate = em.BJRate;
		skillOBJ_DT_SP.Through = em.Through;
		skillOBJ_DT_SP.FlySpeed = em.FlySpeed;
		skillOBJ_DT_SP.Chuan = em.Chuan;
		skillOBJ_DT_SP.MoveSpeedCut = dt.SpeedCut;
		skillOBJ_DT_SP.AttackSpeedCut = dt.SpeedCut;
		skillOBJ_DT_SP.BF_EL_Chuan = 0f;
		skillOBJ_DT_SP.BF_BJrate = 0f;
		skillOBJ_DT_SP.BF_GeDang = 0f;
		skillOBJ_DT_SP.BF_DamageAnti = dt.BF_DamageAnti;
		skillOBJ_DT_SP.C_Damage = dt.C_Damage;
		skillOBJ_DT_SP.C_ATspeed = dt.CompAttackSpeed;
		skillOBJ_DT_SP.C_MVspeed = 0f;
		skillOBJ_DT_SP.C_Health_Prc = 0f;
		skillOBJ_DT_SP.BF_Through = 0f;
		skillOBJ_DT_SP.ChangeSkin = 1;
		skillOBJ_DT_SP.SkinIndex = 0;
		skillOBJ_DT_SP.Reborn = dt.Reborn;
		skillOBJ_DT_SP.DotRate = dt.DotRate;
		skillOBJ_DT_SP.DotDamage = dt.DotDamage;
		skillOBJ_DT_SP.DebuffTime = dt.DebuffTime;
		if (em.attackPL)
		{
			PlayerManager component = em.MVTarget.GetComponent<PlayerManager>();
			component.peo.PL_Set(skillOBJ_DT_SP, 0);
			if (dt.HitFX > 0 && Random.Range(0, 101) < dt.HitFX_Rate)
			{
				LeanPool.Spawn(PB.HitFX[dt.HitFX].OBJ[dt.MainEL], component.yao.transform.position, Quaternion.identity, component.yao.transform);
			}
		}
		else
		{
			Companion component2 = em.MVTarget.GetComponent<Companion>();
			component2.peo.CP_Set(skillOBJ_DT_SP, 0);
			if (dt.HitFX > 0 && Random.Range(0, 101) < dt.HitFX_Rate)
			{
				LeanPool.Spawn(PB.HitFX[dt.HitFX].OBJ[dt.MainEL], component2.yao.transform.position, Quaternion.identity, component2.yao.transform);
			}
		}
		if (dt.ATFX > 0)
		{
			Transform transform;
			Transform transform2;
			switch (dt.FSFXtype)
			{
			case 0:
				transform = ATpoint;
				transform2 = ATpoint2;
				break;
			case 1:
				transform = ATpoint3;
				transform2 = ATpoint4;
				break;
			case 2:
				transform = ATpoint5;
				transform2 = ATpoint6;
				break;
			case 3:
				transform = ATpoint7;
				transform2 = ATpoint8;
				break;
			default:
				transform = ATpoint;
				transform2 = ATpoint2;
				break;
			}
			Vector3 vector = transform2.position - transform.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			GameObject gameObject = LeanPool.Spawn(PB.ATFX[dt.ATFX].OBJ[dt.MainEL], transform.transform.position, Quaternion.identity);
			if (dt.RTtypeFX == 0)
			{
				gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z);
			}
		}
	}

	public void SetCP_Data(EM_Skill_CP dt)
	{
		if (GetEMMB(em.BS.SKC.GlobalID) == null)
		{
			LogUtil.Error("根据敌人的GlobalID获取到的MB为空");
			return;
		}
		int num = em.Comp_Count - em.cpList.Count;
		int num2 = ((num < em.Comp_EveryCount) ? num : em.Comp_EveryCount);
		float num3 = ((num2 < 2) ? 1f : ((num2 >= 4) ? 2f : 1.5f));
		int eL = Random.Range(0, 6);
		int name_Index = Random.Range(0, 3);
		for (int i = 0; i < num2; i++)
		{
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			SK_FSQ_compEM component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.CompMB, new Vector3(base.transform.position.x + Random.Range(0f - num3, num3), base.transform.position.y + Random.Range(0f - num3, num3), base.transform.position.z), Quaternion.identity).GetComponent<SK_FSQ_compEM>();
			component.UseType = 0;
			component.FX = em.CP_FX;
			component.offset = GetEMMB(em.BS.SKC.GlobalID).CompOffset;
			component.skCP = em.BS.SKC;
			component.EL = eL;
			component.em = em;
			component.Name_Index = name_Index;
		}
		if (dt.CPFX > 0)
		{
			Vector3 position = (em.SK_FS.FSFXtype switch
			{
				0 => ATpoint, 
				1 => ATpoint3, 
				2 => ATpoint5, 
				3 => ATpoint7, 
				_ => ATpoint, 
			}).transform.position;
			LeanPool.Spawn(PB.ATFX[dt.CPFX].OBJ[em.MainElement], position, Quaternion.identity);
		}
	}

	public EnemyMB GetEMMB(int id)
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return null;
		}
		foreach (EnemyMB item in SingletonMonoScope<GameDataManager>.Instance.EMMB)
		{
			if (item.GlobalID == id)
			{
				return item;
			}
		}
		return null;
	}

	public BossMB GetBossMB(int id)
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return null;
		}
		foreach (BossMB item in SingletonMonoScope<GameDataManager>.Instance.BossMB)
		{
			if (item.GlobalID == id)
			{
				return item;
			}
		}
		return null;
	}

	public void ChangeState(BossAstat Astat)
	{
		if (!em || !em.IsAlive || !em.IS_Frozen || Astat is BSA_die)
		{
			st?.Exit();
			st = Astat;
			st.Enter(this);
		}
	}

	public void changeST(int type)
	{
		switch (type)
		{
		case 0:
			em.MoveTrack = ani.AnimationState.SetAnimation(0, idle, loop: true);
			em.MoveTrack.TimeScale = em.MoveSpeed_Last;
			em.path.canMove = false;
			break;
		case 1:
			em.MoveTrack = ani.AnimationState.SetAnimation(0, walk, loop: true);
			em.MoveTrack.TimeScale = em.MoveSpeed_Last;
			em.path.canMove = true;
			break;
		case 2:
			em.IsAttack = true;
			switch (em.BS.SK_Cur_IndexA)
			{
			case 0:
				em.AttackTrack = ani.AnimationState.SetAnimation(0, attack[em.BS.AT[em.BS.SK_Cur_IndexB].UseAni], loop: false);
				em.AttackTrack.TimeScale = em.AttackSpeed_Last;
				BindActionComplete(em.AttackTrack);
				em.IsBaTi = em.BS.AT[em.BS.SK_Cur_IndexB].BaTi;
				em.IsFang = em.BS.AT[em.BS.SK_Cur_IndexB].Fang;
				em.path.canMove = false;
				break;
			case 1:
				em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.BS.SK[em.BS.SK_Cur_IndexB].UseAni], loop: false);
				em.SkillTrack.TimeScale = em.SkillSpeed_Max;
				BindActionComplete(em.SkillTrack);
				em.IsBaTi = em.BS.SK[em.BS.SK_Cur_IndexB].BaTi;
				em.IsFang = em.BS.SK[em.BS.SK_Cur_IndexB].Fang;
				em.path.canMove = false;
				break;
			case 2:
				em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.BS.SKC.UseAni], loop: false);
				em.SkillTrack.TimeScale = em.SkillSpeed_Max;
				BindActionComplete(em.SkillTrack);
				em.IsBaTi = true;
				em.path.canMove = false;
				break;
			}
			JStime = 0f;
			break;
		case 3:
			if (die != null)
			{
				ani.AnimationState.SetAnimation(0, die, loop: false);
				ani.AnimationState.SetEmptyAnimation(1, 0f);
			}
			em.canvas.alpha = 0f;
			em.path.canMove = false;
			if (Random.Range(0, 101) < em.SO_DieRate)
			{
				RuntimeManager.PlayOneShot(em.SO_Die, em.yao.transform.position);
			}
			em.OnDie();
			break;
		case 4:
		{
			em.path.canMove = false;
			em.MoveTrack = ani.AnimationState.SetAnimation(0, idle, loop: true);
			em.MoveTrack.TimeScale = em.MoveSpeed_Last;
			TrackEntry trackEntry = ani.AnimationState.SetAnimation(1, hurt, loop: false);
			BindHurtComplete(trackEntry);
			if (Random.Range(0, 101) < em.SO_HurtRate)
			{
				RuntimeManager.PlayOneShot(em.SO_Hurt, em.yao.transform.position);
			}
			break;
		}
		case 5:
			em.XLpoint.transform.position = new Vector3(em.StartPS.x + Random.Range(-1.5f, 1.5f), em.StartPS.y - Random.Range(-1.5f, 1.5f), 0f);
			em.AIDS.target = em.XLpoint.transform;
			em.MoveTrack = ani.AnimationState.SetAnimation(0, walk, loop: true);
			em.MoveTrack.TimeScale = em.MoveSpeed_Last;
			em.path.canMove = true;
			break;
		case 6:
			em.IsAttack = true;
			em.peo.BuffEM.DelAll();
			em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.BS.SK[em.BS.SK_Cur_IndexB].UseAni], loop: false);
			em.SkillTrack.TimeScale = em.SkillSpeed_Max;
			BindActionComplete(em.SkillTrack);
			em.IsBaTi = em.BS.SK[em.BS.SK_Cur_IndexB].BaTi;
			em.IsWuDi = em.BS.SK[em.BS.SK_Cur_IndexB].WuDi;
			switch (em.BS.SK[em.BS.SK_Cur_IndexB].CJY)
			{
			case 0:
				em.IsChong = false;
				em.IsJump = false;
				em.IsYS = false;
				break;
			case 1:
				em.IsChong = true;
				em.IsJump = false;
				em.IsYS = false;
				break;
			case 2:
				em.IsChong = false;
				em.IsJump = true;
				em.IsYS = false;
				break;
			case 3:
				em.IsChong = false;
				em.IsJump = false;
				em.IsYS = true;
				break;
			}
			em.ChongSpeedMulti = em.BS.SK[em.BS.SK_Cur_IndexB].ChongSpeedMulti;
			em.IsFang = em.BS.SK[em.BS.SK_Cur_IndexB].Fang;
			em.path.canMove = true;
			JStime = 0f;
			break;
		case 7:
			em.IsAttack = true;
			em.peo.BuffEM.DelAll();
			em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.BS.SK[em.BS.SK_Cur_IndexB].UseAni], loop: false);
			em.SkillTrack.TimeScale = em.SkillSpeed_Max;
			BindActionComplete(em.SkillTrack);
			em.IsBaTi = em.BS.SK[em.BS.SK_Cur_IndexB].BaTi;
			em.IsWuDi = em.BS.SK[em.BS.SK_Cur_IndexB].WuDi;
			switch (em.BS.SK[em.BS.SK_Cur_IndexB].CJY)
			{
			case 0:
				em.IsChong = false;
				em.IsJump = false;
				em.IsYS = false;
				break;
			case 1:
				em.IsChong = true;
				em.IsJump = false;
				em.IsYS = false;
				break;
			case 2:
				em.IsChong = false;
				em.IsJump = true;
				em.IsYS = false;
				break;
			case 3:
				em.IsChong = false;
				em.IsJump = false;
				em.IsYS = true;
				break;
			}
			em.ChongSpeedMulti = em.BS.SK[em.BS.SK_Cur_IndexB].ChongSpeedMulti;
			em.IsFang = em.BS.SK[em.BS.SK_Cur_IndexB].Fang;
			em.path.canMove = false;
			JStime = 0f;
			break;
		}
		em?.RefreshSpeedAndSetAni();
	}

	private void BindActionComplete(TrackEntry trackEntry)
	{
		if (currentActionTrack != null)
		{
			currentActionTrack.Complete -= OnSpineAnimationComplete;
		}
		currentActionTrack = trackEntry;
		actionCompleteHandled = false;
		if (currentActionTrack != null)
		{
			currentActionTrack.Complete -= OnSpineAnimationComplete;
			currentActionTrack.Complete += OnSpineAnimationComplete;
		}
	}

	private void BindHurtComplete(TrackEntry trackEntry)
	{
		if (currentHurtTrack != null)
		{
			currentHurtTrack.Complete -= OnSpineAnimationComplete;
		}
		currentHurtTrack = trackEntry;
		if (currentHurtTrack != null)
		{
			currentHurtTrack.Complete -= OnSpineAnimationComplete;
			currentHurtTrack.Complete += OnSpineAnimationComplete;
		}
	}

	private void ClearTrackCompleteBindings()
	{
		if (currentActionTrack != null)
		{
			currentActionTrack.Complete -= OnSpineAnimationComplete;
			currentActionTrack = null;
		}
		if (currentHurtTrack != null)
		{
			currentHurtTrack.Complete -= OnSpineAnimationComplete;
			currentHurtTrack = null;
		}
		actionCompleteHandled = false;
	}

	public void OnSpineAnimationComplete(TrackEntry trackEntry)
	{
		if (trackEntry == currentActionTrack)
		{
			if (actionCompleteHandled)
			{
				return;
			}
			actionCompleteHandled = true;
			currentActionTrack.Complete -= OnSpineAnimationComplete;
			currentActionTrack = null;
			if (!em.IsAttack)
			{
				return;
			}
			bool num = em.UseRange_ATplayer_multi_B && em.attackPL;
			em.ClearActionState();
			if (num)
			{
				em.ClearBossTargetPriorityMultiB();
			}
			ChangeSkill();
			em.TryEnableBossTargetPriorityMultiB(0.2f);
			em.Fighting();
			em.AT_Idle_Cur = Random.Range(em.AT_Idle_Min / em.AttackSpeed_Last, em.AT_Idle_Max / em.AttackSpeed_Last);
			if (em.BS.canAttack && em.CanSeeMVTarget)
			{
				ChangeState(new BSA_idle());
			}
			else
			{
				ChangeState(new BSA_walk());
			}
		}
		if (trackEntry == currentHurtTrack)
		{
			currentHurtTrack.Complete -= OnSpineAnimationComplete;
			currentHurtTrack = null;
			if (em.IsYun)
			{
				ani.AnimationState.SetEmptyAnimation(1, 0f);
				ChangeState(new BSA_idle());
			}
		}
	}

	public void ChangeSkill()
	{
		if (Random.Range(0, 101) < em.SK_Rate)
		{
			if (em.SK_Rate_Comp > 0)
			{
				if (Random.Range(0, 101) < em.SK_Rate_Comp)
				{
					em.BS.SK_Cur_IndexA = 2;
				}
				else
				{
					em.BS.SK_Cur_IndexA = 1;
				}
			}
			else
			{
				em.BS.SK_Cur_IndexA = 1;
			}
		}
		else
		{
			em.BS.SK_Cur_IndexA = 0;
		}
		switch (em.BS.SK_Cur_IndexA)
		{
		case 0:
			em.BS.SK_Cur_IndexB = Random.Range(0, em.BS.AT.Count);
			break;
		case 1:
			em.BS.SK_Cur_IndexB = Random.Range(0, em.BS.SK.Count);
			break;
		case 2:
			em.BS.SK_Cur_IndexB = 0;
			break;
		case 3:
			em.BS.SK_Cur_IndexB = 0;
			break;
		}
	}

	public void ChangeFarSkill()
	{
		em.BS.SK_Cur_IndexA = 1;
		em.BS.SK_Cur_IndexB = GetFarSkill();
	}

	public int GetFarSkill()
	{
		float num = 0f;
		int result = 0;
		for (int i = 0; i < em.BS.SK.Count; i++)
		{
			if (em.BS.SK[i].Distance > num)
			{
				num = em.BS.SK[i].Distance;
				result = i;
			}
		}
		return result;
	}
}
