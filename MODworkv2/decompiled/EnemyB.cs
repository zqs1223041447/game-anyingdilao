using System.Collections;
using System.Collections.Generic;
using Entity.Enemies.EnemyAI;
using Entity.Enemies.EnemyState;
using Entity.Enemies.EnemyState.State_B;
using Entity.Enemies.EnemyState.State_B.States;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using Pathfinding;
using Spine;
using Spine.Unity;
using UnityEngine;

public class EnemyB : MonoBehaviour
{
	private static readonly int mainAlpha = Shader.PropertyToID("_MainAlpha");

	private static readonly int fxSat = Shader.PropertyToID("_FXSat");

	private static readonly int fxColor = Shader.PropertyToID("_FXColor");

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

	[HideInInspector]
	public SkeletonAnimation ani;

	private Spine.AnimationState stat;

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

	public EnemyBrain brain;

	private EnemyStateMachine fsm;

	private float spawnStabilizeTimer;

	private int faceDir = 1;

	public bool atCD => JStime >= em.AttackSpeed_JG_Last - 0.0001f;

	private void SetupAI()
	{
		brain = new EnemyBrain(em, em.SpawnPos);
		Dictionary<EnemyStateType, IEnemyState> dictionary = new Dictionary<EnemyStateType, IEnemyState>
		{
			{
				EnemyStateType.Idle,
				new EnemyState_B_Idle(this)
			},
			{
				EnemyStateType.Walk,
				new EnemyState_B_Walk(this)
			},
			{
				EnemyStateType.Attack,
				new EnemyState_B_Attack(this)
			},
			{
				EnemyStateType.Hurt,
				new EnemyState_B_Hurt(this)
			},
			{
				EnemyStateType.Return,
				new EnemyState_B_Return(this)
			},
			{
				EnemyStateType.Patrol,
				new EnemyState_B_Patrol(this)
			},
			{
				EnemyStateType.Die,
				new EnemyState_B_Die(this)
			}
		};
		fsm = new EnemyStateMachine(dictionary, () => (bool)em && em.IS_Frozen);
		foreach (KeyValuePair<EnemyStateType, IEnemyState> item in dictionary)
		{
			if (item.Value is EnemyStateBase_B enemyStateBase_B)
			{
				enemyStateBase_B.BindFSM(fsm);
			}
		}
		fsm.SetInitialState(EnemyStateType.Idle);
	}

	private void ApplyBrainResult()
	{
		if (!em.IsAlive)
		{
			RequestDie();
		}
		else
		{
			if (fsm == null)
			{
				return;
			}
			switch (fsm.CurrentType)
			{
			case EnemyStateType.Attack:
				if (em.IsYun)
				{
					RequestState(EnemyStateType.Hurt);
				}
				return;
			case EnemyStateType.Hurt:
				return;
			case EnemyStateType.Idle:
				if (brain.DesiredState == EnemyStateType.Walk && em.hasTarget && em.canKeepAttack && em.CanSeeTarget)
				{
					return;
				}
				break;
			}
			RequestState(brain.DesiredState);
		}
	}

	public void RequestState(EnemyStateType type)
	{
		fsm?.RequestState(type);
	}

	public void RequestDie()
	{
		RequestState(EnemyStateType.Die);
	}

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
		PB = SingletonMonoScope<GameDataManager>.Instance.SKPB;
		SetupAI();
	}

	public void ResetState()
	{
		brain.Reset(em.SpawnPos);
		fsm.Reset();
		fsm.SetInitialState(EnemyStateType.Idle);
	}

	private void OnEnable()
	{
		if ((bool)em)
		{
			if ((bool)em.path)
			{
				em.path.canMove = false;
			}
			em.mpb.SetFloat(mainAlpha, 0f);
			em.mpb.SetFloat(fxSat, 1f);
			em.mpb.SetColor(fxColor, Color.white);
			em.SpineRender.SetPropertyBlock(em.mpb);
			JStimeA = 0f;
			StartOK = false;
			ani.AnimationState.SetEmptyAnimation(1, 0f);
			StartCoroutine(CoStartNextFrame());
			em.OnDirectDamaged += HandleDirectDamaged;
		}
	}

	private void OnDisable()
	{
		em.OnDirectDamaged -= HandleDirectDamaged;
	}

	private void HandleDirectDamaged()
	{
		brain?.OnHit();
	}

	private IEnumerator CoStartNextFrame()
	{
		yield return null;
		SetStart();
	}

	private void Update()
	{
		if (!StartOK || !em)
		{
			return;
		}
		if (spawnStabilizeTimer > 0f)
		{
			spawnStabilizeTimer -= Time.deltaTime;
			em.path.canMove = false;
			return;
		}
		brain.Tick(Time.deltaTime);
		ApplyBrainResult();
		fsm.Tick();
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
				JStimeA = 0f;
			}
			JStime += Time.deltaTime;
			if (JStime >= em.AttackSpeed_JG_Last)
			{
				JStime = em.AttackSpeed_JG_Last;
			}
		}
	}

	public void SetStart()
	{
		StartOK = true;
		JStime = em.AttackSpeed_JG_Last;
		ResetState();
		spawnStabilizeTimer = 0.15f;
	}

	public void FangXiang()
	{
		if (!em || em.IS_Frozen || em.IsFang || !spine)
		{
			return;
		}
		float num;
		if (em.hasTarget && (bool)em.MVTarget)
		{
			num = em.MVTarget.position.x - base.transform.position.x;
		}
		else
		{
			if (!(em.path != null))
			{
				return;
			}
			num = em.path.steeringTarget.x - base.transform.position.x;
		}
		if (num > 0.05f)
		{
			faceDir = 1;
		}
		else
		{
			if (!(num < -0.05f))
			{
				return;
			}
			faceDir = -1;
		}
		spine.skeleton.ScaleX = faceDir;
	}

	public void changeST(int type)
	{
		switch (type)
		{
		case 0:
			SetMoveAnimation(idle, GetMoveTimeScale());
			em.path.canMove = false;
			break;
		case 1:
			SetMoveAnimation(walk, em.MoveSpeed_Last);
			em.path.canMove = true;
			break;
		case 2:
			em.IsAttack = true;
			switch (em.SK_Cur_Index)
			{
			case 0:
				em.path.canMove = false;
				em.AttackTrack = ani.AnimationState.SetAnimation(0, attack[em.AT_Ani], loop: false);
				em.AttackTrack.TimeScale = em.AttackSpeed_Last;
				em.AttackTrack.Complete += OnSpineAnimationComplete;
				em.IsBaTi = em.SK_AT.BaTi;
				em.IsWuDi = em.SK_AT.WuDi;
				switch (em.SK_AT.CJY)
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
				em.ChongSpeedMulti = em.SK_AT.ChongSpeedMulti;
				em.IsFang = em.AT_Fang;
				break;
			case 1:
				em.path.canMove = false;
				em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.SK_Ani], loop: false);
				em.SkillTrack.TimeScale = em.SkillSpeed_Max;
				em.SkillTrack.Complete += OnSpineAnimationComplete;
				em.IsBaTi = em.SK_A.BaTi;
				em.IsWuDi = em.SK_A.WuDi;
				switch (em.SK_A.CJY)
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
				em.ChongSpeedMulti = em.SK_A.ChongSpeedMulti;
				em.IsFang = em.SK_Fang;
				break;
			case 2:
				em.path.canMove = false;
				em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.SK_Comp.UseAni], loop: false);
				em.SkillTrack.TimeScale = em.SkillSpeed_Max;
				em.SkillTrack.Complete += OnSpineAnimationComplete;
				em.IsBaTi = true;
				break;
			case 3:
				em.path.canMove = false;
				em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.SK_FS.UseAni], loop: false);
				em.SkillTrack.TimeScale = em.SkillSpeed_Max;
				em.SkillTrack.Complete += OnSpineAnimationComplete;
				em.IsBaTi = true;
				break;
			case 4:
				em.path.canMove = false;
				em.SkillTrack = ani.AnimationState.SetAnimation(0, attack[em.ELSS_Ani], loop: false);
				em.SkillTrack.TimeScale = em.SkillSpeed_Max;
				em.SkillTrack.Complete += OnSpineAnimationComplete;
				em.IsBaTi = em.SK_ELSS.BaTi;
				em.IsWuDi = em.SK_ELSS.WuDi;
				switch (em.SK_ELSS.CJY)
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
				em.ChongSpeedMulti = em.SK_ELSS.ChongSpeedMulti;
				em.IsFang = em.ELSS_Fang;
				break;
			}
			JStime = 0f;
			break;
		case 3:
			ani.AnimationState.SetAnimation(0, die, loop: false);
			ani.AnimationState.SetEmptyAnimation(1, 0f);
			em.canvas.alpha = 0f;
			em.path.canMove = false;
			if (Random.Range(0, 101) < em.SO_DieRate)
			{
				RuntimeManager.PlayOneShot(em.SO_Die, em.yao.transform.position);
			}
			em.OnDie();
			break;
		case 4:
			em.path.canMove = false;
			SetMoveAnimation(idle, GetMoveTimeScale());
			ani.AnimationState.SetAnimation(1, hurt, loop: false).Complete += OnSpineAnimationComplete;
			if (Random.Range(0, 101) < em.SO_HurtRate)
			{
				RuntimeManager.PlayOneShot(em.SO_Hurt, em.yao.transform.position);
			}
			break;
		case 5:
			SetMoveAnimation(walk, GetMoveTimeScale());
			em.path.canMove = true;
			break;
		}
	}

	private float GetMoveTimeScale()
	{
		if (!(em.MoveSpeed_Last <= 0f))
		{
			return em.MoveSpeed_Last;
		}
		return 1f;
	}

	private void SetMoveAnimation(string animationName, float timeScale)
	{
		TrackEntry current = ani.AnimationState.GetCurrent(0);
		if (current == null || current.Animation == null || current.Animation.Name != animationName)
		{
			em.MoveTrack = ani.AnimationState.SetAnimation(0, animationName, loop: true);
		}
		else
		{
			em.MoveTrack = current;
		}
		if (em.MoveTrack != null)
		{
			em.MoveTrack.TimeScale = timeScale;
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
			UseFX();
		}
		if (e.Data.Name == SPevent[4])
		{
			UseFX();
		}
		if (e.Data.Name == SPevent[5])
		{
			RuntimeManager.PlayOneShot(em.SO_ChuiDi, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[6] && em.SO_AttackA != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.SO_AttackA, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[7] && em.SO_AttackB != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.SO_AttackB, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[8] && em.SO_AttackC != null && Random.Range(0, 101) < em.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(em.SO_AttackC, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[9] && em.SO_SayA != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.SO_SayA, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[10] && em.SO_SayB != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.SO_SayB, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[11] && em.SO_SayC != null && Random.Range(0, 101) < em.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(em.SO_SayC, em.yao.transform.position);
		}
		if (e.Data.Name == SPevent[12] && em.SO_Walk != null)
		{
			RuntimeManager.PlayOneShot(em.SO_Walk, em.transform.position);
		}
		if (e.Data.Name == SPevent[13])
		{
			em.IsYun = false;
			ani.AnimationState.SetEmptyAnimation(1, 0f);
		}
		if (e.Data.Name == SPevent[14])
		{
			em.path.canMove = true;
		}
		if (e.Data.Name == SPevent[15])
		{
			em.path.canMove = false;
		}
	}

	public void OnSpineAnimationComplete(TrackEntry trackEntry)
	{
		if (em.IsAttack)
		{
			em.ClearActionState();
			int num = Random.Range(0, 101);
			int dotSilencedSkillRate = em.GetDotSilencedSkillRate(em.SK_Rate);
			if (num < dotSilencedSkillRate)
			{
				if (Random.Range(0, 101) < em.SK_Rate_ELSS && em.SK_ELSS.ATmod != 2)
				{
					em.SK_Cur_Index = 4;
				}
				else if (em.SK_Rate_CompFS > 0)
				{
					if (em.SK_Rate_Comp > 0 && em.SK_Rate_FS > 0)
					{
						if (Random.Range(0, 101) < 30)
						{
							if (Random.Range(0, 101) < em.SK_Rate_FS)
							{
								em.SK_Cur_Index = 3;
							}
							else
							{
								em.SK_Cur_Index = 1;
							}
						}
						else if (Random.Range(0, 101) < em.SK_Rate_Comp && em.cpList.Count < em.Comp_Count)
						{
							em.SK_Cur_Index = 2;
						}
						else
						{
							em.SK_Cur_Index = 1;
						}
					}
					else if (em.SK_Rate_Comp > 0 && em.SK_Rate_FS == 0)
					{
						if (Random.Range(0, 101) < em.SK_Rate_Comp && em.cpList.Count < em.Comp_Count)
						{
							em.SK_Cur_Index = 2;
						}
						else
						{
							em.SK_Cur_Index = 1;
						}
					}
					else if (em.SK_Rate_Comp == 0 && em.SK_Rate_FS > 0)
					{
						if (Random.Range(0, 101) < em.SK_Rate_FS)
						{
							em.SK_Cur_Index = 3;
						}
						else
						{
							em.SK_Cur_Index = 1;
						}
					}
				}
				else
				{
					em.SK_Cur_Index = 1;
				}
			}
			else
			{
				em.SK_Cur_Index = 0;
			}
			em.AT_Idle_Cur = Random.Range(em.AT_Idle_Min / em.AttackSpeed_Last, em.AT_Idle_Max / em.AttackSpeed_Last);
			ani.AnimationState.SetEmptyAnimation(0, 0f);
		}
		if (em.IsYun)
		{
			em.IsYun = false;
			ani.AnimationState.SetEmptyAnimation(1, 0f);
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
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint;
			break;
		case 1:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint3.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint3;
			break;
		case 2:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint5.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint5;
			break;
		case 3:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint7.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint7;
			break;
		case 4:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.transform;
			break;
		case 5:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.yao.transform;
			break;
		case 6:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.transform.position, Quaternion.identity, em.transform).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.transform;
			break;
		case 7:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.yao.transform;
			break;
		case 8:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.headUp.transform.position, Quaternion.identity, em.headUp.transform).GetComponent<SkillOBJ_DT_SP>();
			transform2 = em.yao.transform;
			break;
		case 9:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.MVTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = base.transform;
			break;
		case 10:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.ATTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = base.transform;
			break;
		default:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			transform2 = ATpoint;
			break;
		}
		if (dt.RTtypeOBJ == 0)
		{
			Vector3 vector = Vector3.zero;
			if ((bool)transform && (bool)transform2)
			{
				vector = transform.position - transform2.position;
			}
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
		component.damageType = em.MainELType;
		component.MainEL = em.MainElement;
		component.ThroughType = dt.ThroughType;
		component.AttackType = dt.AttackType;
		component.AttackTypeA = dt.AttackTypeA;
		component.AttackTypeB = dt.AttackTypeB;
		component.Damage = dt.Damage / 100f * em.Damage_Last;
		component.DamageA = dt.DamageA / 100f * em.Damage_Last;
		component.DamageB = dt.DamageB / 100f * em.Damage_Last;
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
		component.DotDamage = dt.DotDamage / 100f * em.Damage_Last;
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
			GameObject gameObject = LeanPool.Spawn(PB.ATFX[dt.ATFX].OBJ[em.MainElement], transform3.transform.position, Quaternion.identity);
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
		skillOBJ_DT_SP.damageType = em.MainELType;
		skillOBJ_DT_SP.MainEL = em.MainElement;
		skillOBJ_DT_SP.ThroughType = dt.ThroughType;
		skillOBJ_DT_SP.AttackType = dt.AttackType;
		skillOBJ_DT_SP.AttackTypeA = dt.AttackTypeA;
		skillOBJ_DT_SP.AttackTypeB = dt.AttackTypeB;
		skillOBJ_DT_SP.Damage = dt.Damage / 100f * em.Damage_Last;
		skillOBJ_DT_SP.DamageA = 0f;
		skillOBJ_DT_SP.DamageB = 0f;
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
		skillOBJ_DT_SP.CF_Rate = 0f;
		skillOBJ_DT_SP.ChangeSkin = 1;
		skillOBJ_DT_SP.SkinIndex = 0;
		skillOBJ_DT_SP.Reborn = dt.Reborn;
		skillOBJ_DT_SP.DotRate = dt.DotRate;
		skillOBJ_DT_SP.DotDamage = dt.DotDamage / 100f * em.Damage_Last;
		skillOBJ_DT_SP.DebuffTime = dt.DebuffTime;
		if (em.attackPL)
		{
			PlayerManager component = em.MVTarget.GetComponent<PlayerManager>();
			component.peo.PL_Set(skillOBJ_DT_SP, 0);
			if (em.HitFX > 0 && Random.Range(0, 101) < dt.HitFX_Rate)
			{
				LeanPool.Spawn(PB.HitFX[em.HitFX].OBJ[em.MainElement], component.yao.transform.position, Quaternion.identity, component.yao.transform);
			}
		}
		else
		{
			Companion component2 = em.MVTarget.GetComponent<Companion>();
			if (!component2)
			{
				return;
			}
			if ((bool)component2.peo && (bool)skillOBJ_DT_SP)
			{
				component2.peo.CP_Set(skillOBJ_DT_SP, 0);
			}
			if (em.HitFX > 0 && Random.Range(0, 101) < dt.HitFX_Rate)
			{
				LeanPool.Spawn(PB.HitFX[em.HitFX].OBJ[em.MainElement], component2.yao.transform.position, Quaternion.identity, component2.yao.transform);
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
		int num = em.Comp_Count - em.cpList.Count;
		int num2 = ((num < em.Comp_EveryCount) ? num : em.Comp_EveryCount);
		float num3 = ((num2 < 2) ? 1f : ((num2 >= 4) ? 2f : 1.5f));
		int eL = Random.Range(0, 6);
		for (int i = 0; i < num2; i++)
		{
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			SK_FSQ_compEM component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.CompMB, new Vector3(base.transform.position.x + Random.Range(0f - num3, num3), base.transform.position.y + Random.Range(0f - num3, num3), base.transform.position.z), Quaternion.identity).GetComponent<SK_FSQ_compEM>();
			component.UseType = 0;
			component.FX = em.CP_FX;
			component.offset = GetMB(em.GlobalID).CompOffset;
			component.skCP = em.SK_Comp;
			component.EL = eL;
			component.em = em;
		}
		if (dt.CPFX > 0)
		{
			Vector3 position = (dt.FSFXtype switch
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

	public void SetFS_Data(EM_Skill_FS dt)
	{
		float num = ((em.FS_EveryCount < 4) ? 1.2f : ((em.FS_EveryCount <= 3 || em.FS_EveryCount >= 8) ? 2.5f : 1.8f));
		if (em.FS_Count - em.fsList.Count < em.FS_EveryCount)
		{
			int num2 = em.FS_EveryCount - (em.FS_Count - em.fsList.Count);
			int num3;
			for (num3 = 0; num3 < num2; num3++)
			{
				Enemy enemy = em.fsList[num3];
				em.fsList.Remove(enemy);
				num3--;
				enemy.HealthStat.CurrentValue = 0f;
			}
		}
		for (int i = 0; i < em.FS_EveryCount; i++)
		{
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			SK_FSQ_compEM component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.CompMB, new Vector3(base.transform.position.x + Random.Range(0f - num, num), base.transform.position.y + Random.Range(0f - num, num), base.transform.position.z), Quaternion.identity).GetComponent<SK_FSQ_compEM>();
			component.UseType = 1;
			component.FX = em.CP_FX;
			component.offset = GetMB(em.GlobalID).CompOffset;
			component.EL = em.MainElement;
			component.em = em;
		}
		if (dt.CPFX > 0)
		{
			Vector3 position = (dt.FSFXtype switch
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

	public static EnemyMB GetMB(int id)
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

	public void UseAT()
	{
		switch (em.SK_Cur_Index)
		{
		case 0:
			if (em.SK_AT.ATmod == 0)
			{
				if (!em.AttackLost)
				{
					SetAT_DataJZ(em.SK_AT);
				}
			}
			else
			{
				SetAT_Data(em.SK_AT);
			}
			break;
		case 1:
			if (em.SK_A.ATmod == 0)
			{
				if (!em.AttackLost)
				{
					SetAT_DataJZ(em.SK_A);
				}
			}
			else
			{
				SetAT_Data(em.SK_A);
			}
			break;
		case 2:
			SetCP_Data(em.SK_Comp);
			break;
		case 3:
			SetFS_Data(em.SK_FS);
			break;
		case 4:
			SetAT_Data(em.SK_ELSS);
			break;
		}
	}

	public void UseFX()
	{
		switch (em.SK_Cur_Index)
		{
		case 1:
			if (em.SK_A.StarFX > 0)
			{
				EM_Skill_SP sK_A = em.SK_A;
				GameObject prefab2 = PB.StartFX[sK_A.StarFX].OBJ[em.MainElement];
				switch (sK_A.StarFX_pos)
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
		case 4:
			if (em.SK_ELSS.StarFX > 0)
			{
				EM_Skill_SP sK_ELSS = em.SK_ELSS;
				GameObject prefab = PB.StartFX[sK_ELSS.StarFX].OBJ[em.MainElement];
				switch (sK_ELSS.StarFX_pos)
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
}
