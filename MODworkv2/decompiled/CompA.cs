using System.Collections;
using System.Collections.Generic;
using Entity.Comp.CompState.State_A;
using Entity.Comp.CompanionAI;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using Pathfinding;
using Spine;
using Spine.Unity;
using UnityEngine;

public class CompA : MonoBehaviour
{
	public CompStateMachine fsm;

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

	[HideInInspector]
	public PlayerManager playerManager;

	[HideInInspector]
	public Companion companion;

	public CompanionBrain brain;

	public SkeletonAnimation ani;

	private Spine.AnimationState stat;

	[SpineEvent("", "", true, false, false)]
	public string[] SPevent;

	[SpineAnimation("", "", true, false)]
	public string idle;

	[SpineAnimation("", "", true, false)]
	public string walk;

	[SpineAnimation("", "", true, false)]
	public string attack;

	[SpineAnimation("", "", true, false)]
	public string attackChange;

	[SpineAnimation("", "", true, false)]
	public string die;

	[SpineAnimation("", "", true, false)]
	public string hurt;

	[SpineAnimation("", "", true, false)]
	public string skill;

	[HideInInspector]
	public float JStime;

	[HideInInspector]
	public float JStimeA;

	private bool _fsmStarted;

	public bool atCD => JStime + 0.001f >= companion.AttackSpeed_JG_Last;

	private int AttackCastCount
	{
		get
		{
			if (!(companion != null) || !companion.AT_Double)
			{
				return 1;
			}
			return 2;
		}
	}

	private void Awake()
	{
		brain = GetComponent<CompanionBrain>();
		companion = GetComponent<Companion>();
		ani = base.transform.Find("main/Spine").gameObject.GetComponent<SkeletonAnimation>();
		ATpoint = base.transform.Find("main/Spine/AT");
		ATpoint2 = base.transform.Find("main/Spine/AT/AT2");
		stat = ani.AnimationState;
		stat.Event += OnUserDefinedEvent;
		playerManager = SingletonMonoScope<PlayerManager>.Instance;
		AIDS = GetComponent<AIDestinationSetter>();
		spine = base.transform.Find("main/Spine").GetComponent<SkeletonAnimation>();
		tar = base.transform.Find("main/Spine/SkeletonUtility-SkeletonRoot/tar").gameObject;
		tar2 = base.transform.Find("main/Spine/SkeletonUtility-SkeletonRoot/tar2").gameObject;
		companion.mat = ani.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
		companion.SpineRender = base.transform.Find("main/Spine").gameObject.GetComponent<MeshRenderer>();
		companion.mpb = new MaterialPropertyBlock();
		companion.SpineRender.SetPropertyBlock(companion.mpb);
		Dictionary<CompStateType, ICompState> dictionary = new Dictionary<CompStateType, ICompState>
		{
			{
				CompStateType.Idle,
				new CompState_A_Idle(this)
			},
			{
				CompStateType.Walk,
				new CompState_A_Walk(this)
			},
			{
				CompStateType.Follow,
				new CompState_A_Follow(this)
			},
			{
				CompStateType.Attack,
				new CompState_A_Attack(this)
			},
			{
				CompStateType.Hurt,
				new CompState_A_Hurt(this)
			},
			{
				CompStateType.Patrol,
				new CompState_A_Patrol(this)
			},
			{
				CompStateType.Die,
				new CompState_A_Die(this)
			}
		};
		fsm = new CompStateMachine(dictionary);
		foreach (ICompState value in dictionary.Values)
		{
			(value as CompStateBase_A)?.BindFSM(fsm);
		}
		companion.OnRequestIdle += HandleIdleRequest;
		companion.OnRequestFollow += HandleFollowRequest;
		companion.OnEnterAttack += HandleEnterAttack;
		companion.OnActionFinished += HandleActionFinished;
		companion.OnRequestPatrol += HandlePatrolRequest;
	}

	private void HandleActionFinished()
	{
		companion.IsAttack = false;
		companion.IsSkill = false;
		if (fsm.CurrentType == CompStateType.Attack)
		{
			fsm.RequestState(CompStateType.Idle);
		}
	}

	private void OnDestroy()
	{
		companion.OnRequestIdle -= HandleIdleRequest;
		companion.OnRequestFollow -= HandleFollowRequest;
		companion.OnEnterAttack -= HandleEnterAttack;
		companion.OnActionFinished -= HandleActionFinished;
		companion.OnRequestPatrol -= HandlePatrolRequest;
	}

	private void HandlePatrolRequest()
	{
		fsm?.RequestState(CompStateType.Patrol);
	}

	private void HandleFollowRequest()
	{
		fsm?.RequestState(CompStateType.Follow);
	}

	private void HandleIdleRequest()
	{
		fsm?.RequestState(CompStateType.Idle);
	}

	private void HandleEnterAttack(Enemy enemy)
	{
		if (fsm != null && (bool)enemy && enemy.IsAlive)
		{
			fsm.RequestState(CompStateType.Attack);
		}
	}

	private void DriveFsmByIntent()
	{
		if (fsm != null && (bool)companion)
		{
			CompanionIntentFsmDriver.Drive(companion, fsm.RequestState);
		}
	}

	private void OnEnable()
	{
		JStimeA = 0f;
		JStime = companion.AttackSpeed_JG_Last;
		ani.AnimationState.SetEmptyAnimation(1, 0f);
		ani.AnimationState.Complete -= OnSpineAnimationComplete;
		ani.AnimationState.Complete += OnSpineAnimationComplete;
		fsm.Reset();
		_fsmStarted = false;
		StartCoroutine(DelayStartFSM());
	}

	private IEnumerator DelayStartFSM()
	{
		yield return null;
		SetStart();
	}

	public void SetStart()
	{
		if (!_fsmStarted)
		{
			_fsmStarted = true;
			fsm.SetInitialState(CompStateType.Idle);
			changeST(0);
			if (companion.ARS_Damage > 0f)
			{
				companion.SetARSkill();
			}
		}
	}

	private void Update()
	{
		if (_fsmStarted)
		{
			if (companion.IsAlive && !companion.IsYun)
			{
				UpdateFacing();
			}
			DriveFsmByIntent();
			fsm.Tick();
			JStimeA += Time.deltaTime;
			if (JStimeA >= 0.1f)
			{
				companion.path.maxSpeed = companion.MoveSpeed_Path * companion.MoveSpeed_Last;
				JStimeA = 0f;
			}
			JStime += Time.deltaTime;
			if (JStime >= companion.AttackSpeed_JG_Last)
			{
				JStime = companion.AttackSpeed_JG_Last;
			}
		}
	}

	private void UpdateFacing()
	{
		Vector3 vector = companion.path.steeringTarget - base.transform.position;
		if (!(Mathf.Abs(vector.x) < 0.01f))
		{
			spine.skeleton.ScaleX = ((vector.x > 0f) ? 1 : (-1));
		}
	}

	private void ApplyMeleeHit(Enemy target, int dotMulti, GameObject hitPrefab)
	{
		for (int i = 0; i < AttackCastCount; i++)
		{
			target.peo.EM_Set(companion.sp, dotMulti, 0, Dot_Infect: false, 0, 0f);
			LeanPool.Spawn(hitPrefab, target.yao.transform.position, Quaternion.identity, target.yao.transform);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_Hit(companion.Name, target, base.transform.right);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_CPSK(companion.Name, companion, target.yao.transform, GetAttackAngle(target.yao.transform.position));
		}
	}

	private float GetAttackAngle(Vector3 targetPosition)
	{
		Vector3 vector = (((bool)companion && (bool)companion.yao) ? companion.yao.transform.position : base.transform.position);
		Vector3 vector2 = targetPosition - vector;
		return Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
	}

	private void SpawnAttackPrefab(GameObject prefab, Vector3 position, Quaternion rotation, bool change)
	{
		for (int i = 0; i < AttackCastCount; i++)
		{
			SkillOBJ_DT_SP component = LeanPool.Spawn(prefab, position, rotation).GetComponent<SkillOBJ_DT_SP>();
			SetAT_Data(component, IsAT: true, change);
		}
	}

	public void OnUserDefinedEvent(TrackEntry trackEntry, Spine.Event e)
	{
		if (e.Data.Name == SPevent[0] && companion.IsAttack && companion.Change_AT == 0f && (bool)companion.MVTarget)
		{
			if (companion.ATmod_Sample == 0)
			{
				if (!companion.AttackLost)
				{
					if ((bool)companion.MVTarget)
					{
						Enemy component = companion.MVTarget.GetComponent<Enemy>();
						if ((bool)component)
						{
							ApplyMeleeHit(component, companion.DotMultiA, companion.AT_Hit);
						}
					}
					if ((bool)companion.AT_FX)
					{
						Vector3 vector = ATpoint2.position - ATpoint.position;
						float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
						LeanPool.Spawn(companion.AT_FX, ATpoint.transform.position, Quaternion.Euler(0f, 0f, z));
					}
				}
			}
			else
			{
				Vector3 vector2 = companion.ATTarget.transform.position - ATpoint.position;
				float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
				SpawnAttackPrefab(companion.OBJ_AT[companion.AT_ZD], ATpoint.transform.position, Quaternion.Euler(0f, 0f, z2), change: false);
			}
		}
		if (e.Data.Name == SPevent[1] && companion.IsAttack && companion.Change_AT > 0f && (bool)companion.MVTarget)
		{
			if (companion.ATmod_Change == 0 && (bool)companion.MVTarget)
			{
				if (!companion.AttackLost)
				{
					if ((bool)companion.MVTarget)
					{
						Enemy component2 = companion.MVTarget.GetComponent<Enemy>();
						if ((bool)component2)
						{
							ApplyMeleeHit(component2, companion.DotMultiB, companion.ATC_Hit);
						}
					}
					if ((bool)companion.ATC_FX)
					{
						Vector3 vector3 = ATpoint2.position - ATpoint.position;
						float z3 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
						LeanPool.Spawn(companion.ATC_FX, ATpoint.transform.position, Quaternion.Euler(0f, 0f, z3));
					}
				}
			}
			else
			{
				Vector3 vector4 = companion.ATTarget.transform.position - ATpoint.position;
				float z4 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
				SpawnAttackPrefab(companion.OBJ_ATC[companion.AT_ZD], ATpoint.transform.position, Quaternion.Euler(0f, 0f, z4), change: true);
			}
		}
		if (e.Data.Name == SPevent[2] && companion.IsSkill)
		{
			if (!companion.MVTarget)
			{
				return;
			}
			Enemy component3 = companion.MVTarget.GetComponent<Enemy>();
			Vector3 vector5 = companion.ATTarget.transform.position - ATpoint.position;
			float z5 = Mathf.Atan2(vector5.y, vector5.x) * 57.29578f;
			Vector3 vector6 = companion.ATTarget.transform.position - companion.yao.transform.position;
			float z6 = Mathf.Atan2(vector6.y, vector6.x) * 57.29578f;
			switch (companion.SKApos)
			{
			case 0:
			{
				SkillOBJ_DT_SP component8 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], ATpoint.transform.position, Quaternion.Euler(0f, 0f, z5)).GetComponent<SkillOBJ_DT_SP>();
				SetAT_Data(component8, IsAT: false, Change: false);
				break;
			}
			case 1:
			{
				SkillOBJ_DT_SP component7 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				SetAT_Data(component7, IsAT: false, Change: false);
				break;
			}
			case 2:
			{
				SkillOBJ_DT_SP component6 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], companion.yao.transform.position, Quaternion.Euler(0f, 0f, z6)).GetComponent<SkillOBJ_DT_SP>();
				SetAT_Data(component6, IsAT: false, Change: false);
				break;
			}
			case 3:
			{
				SkillOBJ_DT_SP component5 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], component3.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				SetAT_Data(component5, IsAT: false, Change: false);
				break;
			}
			case 4:
			{
				SkillOBJ_DT_SP component4 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], companion.ATTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				SetAT_Data(component4, IsAT: false, Change: false);
				break;
			}
			}
		}
		if (e.Data.Name == SPevent[3] && companion.SO_AttackA != null && Random.Range(0, 101) < companion.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(companion.SO_AttackA, companion.yao.transform.position);
		}
		if (e.Data.Name == SPevent[4] && companion.SO_AttackB != null && Random.Range(0, 101) < companion.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(companion.SO_AttackB, companion.yao.transform.position);
		}
		if (e.Data.Name == SPevent[5] && companion.SO_AttackC != null && Random.Range(0, 101) < companion.SO_AttackRate)
		{
			RuntimeManager.PlayOneShot(companion.SO_AttackC, companion.yao.transform.position);
		}
		if (e.Data.Name == SPevent[6] && companion.SO_SayA != null && Random.Range(0, 101) < companion.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(companion.SO_SayA, companion.yao.transform.position);
		}
		if (e.Data.Name == SPevent[7] && companion.SO_SayB != null && Random.Range(0, 101) < companion.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(companion.SO_SayB, companion.yao.transform.position);
		}
		if (e.Data.Name == SPevent[8] && companion.SO_SayC != null && Random.Range(0, 101) < companion.SO_SayRate)
		{
			RuntimeManager.PlayOneShot(companion.SO_SayC, companion.yao.transform.position);
		}
		if (e.Data.Name == SPevent[9])
		{
			fsm.RequestState(CompStateType.Idle);
		}
		if (e.Data.Name == SPevent[10] && companion.SO_Walk != null)
		{
			RuntimeManager.PlayOneShot(companion.SO_Walk, companion.transform.position);
		}
	}

	public void changeST(int type)
	{
		switch (type)
		{
		case 0:
			AIDS.target = companion.MVTarget;
			companion.MoveTrack = ani.AnimationState.SetAnimation(0, idle, loop: true);
			companion.MoveTrack.TimeScale = companion.MoveSpeed_Last * 0.7f;
			companion.path.canMove = false;
			break;
		case 1:
			AIDS.target = companion.MVTarget;
			companion.MoveTrack = ani.AnimationState.SetAnimation(0, walk, loop: true);
			companion.MoveTrack.TimeScale = companion.MoveSpeed_Last;
			companion.path.canMove = true;
			break;
		case 2:
			AIDS.target = companion.GetFollowPoint();
			companion.MoveTrack = ani.AnimationState.SetAnimation(0, walk, loop: true);
			companion.MoveTrack.TimeScale = companion.MoveSpeed_Last;
			companion.path.canMove = true;
			break;
		case 3:
			AIDS.target = companion.MVTarget;
			companion.path.canMove = false;
			if (companion.NextAT_Type == 1)
			{
				companion.IsSkill = true;
				companion.SkillTrack = ani.AnimationState.SetAnimation(0, skill, loop: false);
				companion.SkillTrack.TimeScale = companion.SkillSpeed_Max;
				companion.SkillTrack.Complete += OnSpineAnimationComplete;
			}
			else
			{
				companion.IsAttack = true;
				if (companion.Change_AT > 0f)
				{
					companion.AttackTrack = ani.AnimationState.SetAnimation(0, attackChange, loop: false);
					companion.AttackTrack.TimeScale = companion.AttackSpeed_Last;
					companion.AttackTrack.Complete += OnSpineAnimationComplete;
				}
				else
				{
					companion.AttackTrack = ani.AnimationState.SetAnimation(0, attack, loop: false);
					companion.AttackTrack.TimeScale = companion.AttackSpeed_Last;
					companion.AttackTrack.Complete += OnSpineAnimationComplete;
				}
			}
			JStime = 0f;
			break;
		case 4:
			if (companion.DieType == 0 || companion.DieType == 2)
			{
				companion.MoveTrack = ani.AnimationState.SetAnimation(0, die, loop: false);
				companion.MoveTrack.TimeScale = companion.MoveSpeed_Last;
				ani.AnimationState.SetEmptyAnimation(1, 0f);
			}
			AIDS.target = null;
			companion.path.canMove = false;
			if (Random.Range(0, 101) < companion.SO_DieRate)
			{
				RuntimeManager.PlayOneShot(companion.SO_Die, companion.yao.transform.position);
			}
			break;
		case 5:
			companion.path.canMove = false;
			companion.MoveTrack = ani.AnimationState.SetAnimation(0, idle, loop: true);
			companion.MoveTrack.TimeScale = companion.MoveSpeed_Last;
			ani.AnimationState.SetAnimation(1, hurt, loop: false).Complete += OnSpineAnimationComplete;
			if (Random.Range(0, 101) < companion.SO_HurtRate)
			{
				RuntimeManager.PlayOneShot(companion.SO_Hurt, companion.yao.transform.position);
			}
			break;
		case 6:
		{
			Vector3 companionFollowBasePosition = companion.PL.GetCompanionFollowBasePosition();
			companion.XLpoint.transform.position = new Vector3(companionFollowBasePosition.x + Random.Range(-1.5f, 1.5f), companionFollowBasePosition.y - Random.Range(-1.5f, 1.5f), 0f);
			AIDS.target = companion.XLpoint.transform;
			companion.MoveTrack = ani.AnimationState.SetAnimation(0, walk, loop: true);
			companion.MoveTrack.TimeScale = companion.MoveSpeed_Last;
			companion.path.canMove = true;
			break;
		}
		}
	}

	public void OnSpineAnimationComplete(TrackEntry trackEntry)
	{
		if (companion.IsAttack || companion.IsSkill)
		{
			if (companion.ATSrate > 0f)
			{
				int num = Random.Range(0, 101);
				companion.NextAT_Type = (((float)num < companion.ATSrate) ? 1 : 0);
			}
			else
			{
				companion.NextAT_Type = 0;
			}
			ani.AnimationState.SetEmptyAnimation(0, 0f);
			companion.NotifyActionFinished();
			fsm.RequestState(CompStateType.Idle);
		}
		if (companion.IsYun)
		{
			ani.AnimationState.SetEmptyAnimation(1, 0f);
			companion.NotifyActionFinished();
			fsm.RequestState(CompStateType.Idle);
		}
	}

	public void SetAT_Data(SkillOBJ_DT_SP sp, bool IsAT, bool Change)
	{
		sp.indexType = 1;
		sp.pl = playerManager;
		sp.cp = companion;
		sp.ZY = true;
		sp.Dot_Infect = false;
		sp.Dot_Infect_Layer = 0;
		sp.skillName = companion.Name;
		sp.AttackType = true;
		if (IsAT)
		{
			if (Change)
			{
				sp.damageType = companion.damageType_Change;
				sp.Chuan = playerManager.GiveChuan(companion.damageType_Change);
				sp.Damage = companion.Damage_Last / 100f * playerManager.GiveDamage(companion.damageType_Change) * (float)companion.AT_DMG / 100f;
			}
			else
			{
				sp.damageType = companion.damageType;
				sp.Chuan = playerManager.GiveChuan(companion.damageType);
				sp.Damage = companion.Damage_Last / 100f * playerManager.GiveDamage(companion.damageType) * (float)companion.AT_DMG / 100f;
			}
			sp.Type_F = companion.Type_A;
			sp.Type_S = companion.Type_A;
			sp.TypeDIC_F = companion.TypeDIC_A;
			sp.TypeDIC_S = companion.TypeDIC_A;
			sp.JG = companion.JG_A;
			sp.AngleA = companion.AngleA;
			sp.AngleB = companion.AngleA;
			sp.FStime1 = companion.FStimeA;
			sp.FStime2 = companion.FStimeA;
			sp.Count_F = companion.Count_A;
			sp.Count_S = companion.Count_A;
			sp.Count_ATtarget = companion.Count_ATtarget_A;
			sp.CountMulti = companion.CountMulti_A;
			sp.Follow_F = companion.Follow_A;
			sp.Follow_S = companion.Follow_A;
			sp.AllChuan_F = companion.AllChuan_A;
			sp.RDSpeed_F = companion.RDSpeed_A;
			sp.HasFX = companion.HasFX_A;
			sp.colEXP = companion.colEXP_A;
			sp.EXPpos = companion.EXPpos_A;
		}
		else
		{
			sp.damageType = companion.ChangeEL_SK;
			sp.Chuan = playerManager.GiveChuan(companion.ChangeEL_SK);
			sp.Damage = companion.Damage_Last * companion.ATS_Damage / companion.Damage_Base / 100f * playerManager.GiveDamage(companion.ChangeEL_SK) * companion.SkillDamageMultiplier;
			sp.Type_F = companion.Type_B;
			sp.Type_S = companion.Type_B;
			sp.TypeDIC_F = companion.TypeDIC_B;
			sp.TypeDIC_S = companion.TypeDIC_B;
			sp.JG = companion.JG_B;
			sp.AngleA = companion.AngleB;
			sp.AngleB = companion.AngleB;
			sp.FStime1 = companion.FStimeB;
			sp.FStime2 = companion.FStimeB;
			sp.Count_F = companion.Count_B;
			sp.Count_S = companion.Count_B;
			sp.Count_ATtarget = companion.Count_ATtarget_B;
			sp.CountMulti = companion.CountMulti_B;
			sp.Follow_F = companion.Follow_B;
			sp.Follow_S = companion.Follow_B;
			sp.AllChuan_F = companion.AllChuan_B;
			sp.RDSpeed_F = companion.RDSpeed_B;
			sp.HasFX = companion.HasFX_B;
			sp.colEXP = companion.colEXP_B;
			sp.EXPpos = companion.EXPpos_B;
		}
		sp.BJrate = (companion.BJ_NoDot ? 100f : playerManager.BJrate_Last);
		sp.JYrate = playerManager.JYrate_Last;
		sp.BJDamage = playerManager.BJDamage_Last;
		sp.FlySpeed = companion.FlySpeed;
		sp.AT_DotLayer = companion.AT_DotLayer;
		sp.BJ_NoDot = companion.BJ_NoDot;
		sp.WS_All = companion.WS_All;
		sp.Field_Range = companion.Field_Range;
		sp.TargetPos = companion.MVTarget.transform.position;
		sp.Distance = companion.Range_EM;
		sp.Layer_SubA = 0;
		sp.Layer_SubB = 0;
		sp.NoTime = 1;
		sp.BuffTime = 1f;
		sp.Slow_F = 1;
		sp.Slow_S = 1;
		sp.AB_HasFX = 1;
		sp.colEXP_A = 1;
		sp.AB_colEXP = 1;
		sp.TimeEXP = 1;
		sp.TimeEXP_AB = 1;
		sp.LastEXP = 1;
		sp.LastEXP_AB = 1;
		sp.S_LastEXP = 1;
		sp.AB_LastEXP = 1;
		sp.AngleEXP = 1;
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.CreatACT_CPSK(companion.Name, companion, sp.transform, sp.transform.eulerAngles.z);
		}
	}
}
