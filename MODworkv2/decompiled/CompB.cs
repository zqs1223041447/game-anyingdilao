using System.Collections;
using System.Collections.Generic;
using Entity.Comp.CompState.State_A;
using Entity.Comp.CompState.State_B;
using Entity.Comp.CompState.State_B.States;
using Entity.Comp.CompanionAI;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using Pathfinding;
using UnityEngine;

public class CompB : MonoBehaviour
{
	[HideInInspector]
	public AIDestinationSetter AIDS;

	public Transform ATpoint;

	[HideInInspector]
	public Companion companion;

	[HideInInspector]
	public PlayerManager playerManager;

	[HideInInspector]
	public float JStime;

	[HideInInspector]
	public float JStimeA;

	public GameObject Lit;

	public GameObject Big;

	public float HighA;

	public float HighB;

	public Transform Shadow;

	public float SizeA;

	public float SizeB;

	public CapsuleCollider2D col;

	public float colSizeA;

	public float colSizeB;

	public CompStateMachine fsm;

	private bool fsmStarted;

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

	private void HandleActionFinished()
	{
		companion.IsAttack = false;
		companion.IsSkill = false;
		if (fsm.CurrentType == CompStateType.Attack)
		{
			fsm.RequestState(CompStateType.Idle);
		}
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

	private void Awake()
	{
		companion = GetComponent<Companion>();
		ATpoint = base.transform.Find("main/Spirit");
		Lit = base.transform.Find("main/Spirit/Lit").gameObject;
		Big = base.transform.Find("main/Spirit/Big").gameObject;
		col = base.transform.Find("main").GetComponent<CapsuleCollider2D>();
		Shadow = base.transform.Find("shadow").transform;
		AIDS = GetComponent<AIDestinationSetter>();
		playerManager = SingletonMonoScope<PlayerManager>.Instance;
		Dictionary<CompStateType, ICompState> dictionary = new Dictionary<CompStateType, ICompState>
		{
			{
				CompStateType.Idle,
				new CompState_B_Idle(this)
			},
			{
				CompStateType.Walk,
				new CompState_B_Walk(this)
			},
			{
				CompStateType.Follow,
				new CompState_B_Follow(this)
			},
			{
				CompStateType.Attack,
				new CompState_B_Attack(this)
			},
			{
				CompStateType.Patrol,
				new CompState_B_Patrol(this)
			},
			{
				CompStateType.Die,
				new CompState_B_Die(this)
			}
		};
		fsm = new CompStateMachine(dictionary);
		foreach (ICompState value in dictionary.Values)
		{
			(value as CompStateBase_B)?.BindFSM(fsm);
		}
		companion.OnRequestIdle += HandleIdleRequest;
		companion.OnRequestFollow += HandleFollowRequest;
		companion.OnEnterAttack += HandleEnterAttack;
		companion.OnActionFinished += HandleActionFinished;
		companion.OnRequestPatrol += HandlePatrolRequest;
	}

	private void OnDestroy()
	{
		if ((bool)companion)
		{
			companion.OnRequestIdle -= HandleIdleRequest;
			companion.OnRequestFollow -= HandleFollowRequest;
			companion.OnEnterAttack -= HandleEnterAttack;
			companion.OnActionFinished -= HandleActionFinished;
			companion.OnRequestPatrol -= HandlePatrolRequest;
		}
	}

	private void OnEnable()
	{
		JStimeA = 0f;
		JStime = companion.AttackSpeed_JG_Last;
		fsm.Reset();
		fsmStarted = false;
		StartCoroutine(DelayStartFSM());
	}

	private IEnumerator DelayStartFSM()
	{
		yield return null;
		SetStart();
	}

	public void SetStart()
	{
		if (!fsmStarted)
		{
			fsmStarted = true;
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
		if (fsmStarted)
		{
			CompanionIntentFsmDriver.Drive(companion, fsm.RequestState);
			fsm.Tick();
			JStimeA += Time.deltaTime;
			if (JStimeA >= 0.2f)
			{
				companion.path.maxSpeed = companion.MoveSpeed_Path * companion.MoveSpeed_Last;
				JStimeA = 0f;
				ChangeType();
			}
			JStime += Time.deltaTime;
			if (JStime >= companion.AttackSpeed_JG_Last)
			{
				JStime = companion.AttackSpeed_JG_Last;
			}
		}
	}

	public void ChangeType()
	{
		switch (companion.BStype)
		{
		case 0:
			Lit.SetActive(value: true);
			Big.SetActive(value: false);
			ATpoint.localPosition = new Vector3(0f, HighA, 0f);
			Shadow.localScale = new Vector3(SizeA, SizeA, SizeA);
			col.size = new Vector2(colSizeA, colSizeA);
			col.offset = new Vector2(0f, HighA);
			break;
		case 1:
			Lit.SetActive(value: false);
			Big.SetActive(value: true);
			ATpoint.localPosition = new Vector3(0f, HighB, 0f);
			Shadow.localScale = new Vector3(SizeB, SizeB, SizeB);
			col.size = new Vector2(colSizeB, colSizeB);
			col.offset = new Vector2(0f, HighB);
			break;
		case 2:
			break;
		}
	}

	private void SpawnAttackPrefab(GameObject prefab, Quaternion rotation, bool change)
	{
		for (int i = 0; i < AttackCastCount; i++)
		{
			SkillOBJ_DT_SP component = LeanPool.Spawn(prefab, ATpoint.transform.position, rotation).GetComponent<SkillOBJ_DT_SP>();
			SetAT_Data(component, IsAT: true, change);
		}
	}

	public void changeST(int type)
	{
		switch (type)
		{
		case 0:
			AIDS.target = companion.MVTarget;
			companion.path.canMove = false;
			break;
		case 1:
			AIDS.target = companion.MVTarget;
			companion.path.canMove = true;
			break;
		case 2:
			AIDS.target = companion.GetFollowPoint();
			companion.path.canMove = true;
			break;
		case 3:
			AIDS.target = companion.MVTarget;
			companion.path.canMove = false;
			if (companion.ATSrate > 0f)
			{
				if ((float)Random.Range(0, 101) < companion.ATSrate)
				{
					companion.IsSkill = true;
					if (!companion.MVTarget)
					{
						break;
					}
					Enemy component = companion.MVTarget.GetComponent<Enemy>();
					Vector3 vector = companion.ATTarget.transform.position - ATpoint.position;
					float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
					Vector3 vector2 = companion.ATTarget.transform.position - companion.yao.transform.position;
					float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
					switch (companion.SKApos)
					{
					case 0:
					{
						SkillOBJ_DT_SP component6 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], ATpoint.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<SkillOBJ_DT_SP>();
						SetAT_Data(component6, IsAT: false, Change: false);
						break;
					}
					case 1:
					{
						SkillOBJ_DT_SP component5 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
						SetAT_Data(component5, IsAT: false, Change: false);
						break;
					}
					case 2:
					{
						SkillOBJ_DT_SP component4 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], companion.yao.transform.position, Quaternion.Euler(0f, 0f, z2)).GetComponent<SkillOBJ_DT_SP>();
						SetAT_Data(component4, IsAT: false, Change: false);
						break;
					}
					case 3:
					{
						SkillOBJ_DT_SP component3 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], component.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
						SetAT_Data(component3, IsAT: false, Change: false);
						break;
					}
					case 4:
					{
						SkillOBJ_DT_SP component2 = LeanPool.Spawn(companion.OBJ_SKA[companion.SK_ZD], companion.ATTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
						SetAT_Data(component2, IsAT: false, Change: false);
						break;
					}
					}
				}
				else
				{
					companion.IsAttack = true;
					Vector3 vector3 = companion.ATTarget.transform.position - ATpoint.position;
					float z3 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
					if (companion.Change_AT > 0f)
					{
						SpawnAttackPrefab(companion.OBJ_ATC[companion.AT_ZD], Quaternion.Euler(0f, 0f, z3), change: true);
					}
					else
					{
						SpawnAttackPrefab(companion.OBJ_AT[companion.AT_ZD], Quaternion.Euler(0f, 0f, z3), change: false);
					}
				}
			}
			else
			{
				companion.IsAttack = true;
				Vector3 vector4 = companion.ATTarget.transform.position - ATpoint.position;
				float z4 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
				if (companion.Change_AT > 0f)
				{
					SpawnAttackPrefab(companion.OBJ_ATC[companion.AT_ZD], Quaternion.Euler(0f, 0f, z4), change: true);
				}
				else
				{
					SpawnAttackPrefab(companion.OBJ_AT[companion.AT_ZD], Quaternion.Euler(0f, 0f, z4), change: false);
				}
			}
			if (companion.ATSrate > 0f)
			{
				if ((float)Random.Range(0, 101) < companion.ATSrate)
				{
					companion.NextAT_Type = 1;
				}
				else
				{
					companion.NextAT_Type = 0;
				}
			}
			else
			{
				companion.NextAT_Type = 0;
			}
			JStime = 0f;
			break;
		case 4:
			AIDS.target = null;
			companion.path.canMove = false;
			if (Random.Range(0, 101) < companion.SO_DieRate)
			{
				RuntimeManager.PlayOneShot(companion.SO_Die, companion.yao.transform.position);
			}
			break;
		case 5:
		{
			Vector3 companionFollowBasePosition = companion.PL.GetCompanionFollowBasePosition();
			companion.XLpoint.transform.position = new Vector3(companionFollowBasePosition.x + Random.Range(-1.5f, 1.5f), companionFollowBasePosition.y - Random.Range(-1.5f, 1.5f), 0f);
			AIDS.target = companion.XLpoint.transform;
			companion.path.canMove = true;
			break;
		}
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
		sp.AttackTypeA = true;
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
