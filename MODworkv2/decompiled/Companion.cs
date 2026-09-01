using System;
using System.Collections.Generic;
using Entity.Comp.CompanionAI;
using Entity.Misc;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using Pathfinding;
using Spine;
using UnityEngine;

public class Companion : MonoBehaviour
{
	public enum CompanionIntentState
	{
		None,
		Idle,
		Follow,
		Combat,
		Patrol
	}

	[HideInInspector]
	public CompStat HealthStat;

	[HideInInspector]
	public CanvasGroup canvas;

	[HideInInspector]
	public PlayerManager PL;

	[HideInInspector]
	public GameObject headUp;

	[HideInInspector]
	public GameObject head;

	[HideInInspector]
	public GameObject body;

	[HideInInspector]
	public GameObject yao;

	[HideInInspector]
	public GameObject foot;

	[HideInInspector]
	public Transform MVTarget;

	[HideInInspector]
	public Transform ATTarget;

	[HideInInspector]
	public GameObject XLpoint;

	[HideInInspector]
	public People peo;

	[HideInInspector]
	public BuffMG_CP BuffMG;

	[HideInInspector]
	public AIPath path;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public MeshRenderer SpineRender;

	[HideInInspector]
	public Material mat;

	public MaterialPropertyBlock mpb;

	[HideInInspector]
	public CapsuleCollider2D B_Col;

	[HideInInspector]
	public CircleCollider2D F_Col;

	private bool CanBeSystemDeleted;

	private RaycastHit2D ray;

	[HideInInspector]
	public int Index;

	[HideInInspector]
	public string Name;

	public int size;

	[HideInInspector]
	public float AttackSpeed_JG;

	[HideInInspector]
	public float AttackSpeed_Base;

	[HideInInspector]
	public float AttackSpeed_Max;

	[HideInInspector]
	public float AttackSpeed_Bei;

	[HideInInspector]
	public float AttackSpeed_Cut;

	[HideInInspector]
	public float AttackSpeed_Last;

	[HideInInspector]
	public float MoveSpeed_Base;

	[HideInInspector]
	public float MoveSpeed_Max;

	[HideInInspector]
	public float MoveSpeed_Bei;

	[HideInInspector]
	public float MoveSpeed_Cut;

	[HideInInspector]
	public float MoveSpeed_Last;

	[HideInInspector]
	public float GeDang_Base;

	[HideInInspector]
	public float GeDang_Max;

	[HideInInspector]
	public float Damage_Base;

	[HideInInspector]
	public float Damage_Max;

	[HideInInspector]
	public float Damage_Bei;

	[HideInInspector]
	public float Damage_Cut;

	[HideInInspector]
	public float Damage_Last;

	[HideInInspector]
	public float AllAnti_Base;

	[HideInInspector]
	public float AllAnti_Max;

	[HideInInspector]
	public float Damage_Anti_Base;

	[HideInInspector]
	public float Damage_Anti_Max;

	[HideInInspector]
	public float Health_Bei;

	[HideInInspector]
	public float Health_Prc_Base;

	[HideInInspector]
	public float Health_Prc_Bei;

	[HideInInspector]
	public float Health_Prc_Max;

	[HideInInspector]
	public float FlySpeed;

	[HideInInspector]
	public int BStype;

	[HideInInspector]
	public int AT_ZD;

	[HideInInspector]
	public int SK_ZD;

	[HideInInspector]
	public int AT_DMG = 100;

	[HideInInspector]
	public int SK_DMG = 100;

	[HideInInspector]
	public SK_CP_Universe[] universe;

	[HideInInspector]
	public SK_CP_Forever[] forever;

	[HideInInspector]
	public SK_CP_Round[] round;

	[HideInInspector]
	public float MaxForceFollowDistance;

	[HideInInspector]
	public float MaxTeleportDistance;

	public bool CanSeeMVTarget;

	public int ATmod_Sample;

	public int ATmod_Change;

	public float Range_AT_Hurt;

	[HideInInspector]
	public int NextAT_Type;

	public float Range_AT_Lit;

	public float Range_AT_Mid;

	public float Range_AT_Big;

	public bool NoYun;

	public int YunAnti;

	public float Range_EM;

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

	public GameObject AT_FX;

	public GameObject AT_Hit;

	public GameObject ATC_FX;

	public GameObject ATC_Hit;

	public int SKApos;

	public GameObject SKA_FX;

	public GameObject SKB_OBJ;

	public int DieType;

	public int DiePos;

	public GameObject Die_OBJ;

	public GameObject[] OBJ_AT;

	public GameObject[] OBJ_ATC;

	public GameObject[] OBJ_SKA;

	[HideInInspector]
	public bool CanSO_Idle;

	[HideInInspector]
	public float Idle_Time_Min;

	[HideInInspector]
	public float Idle_Time_Max;

	[HideInInspector]
	public float Idle_Time_Cur;

	[HideInInspector]
	public float Idle_Time_Tmp;

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

	public List<SK_BuffA> SkillBuffList = new List<SK_BuffA>();

	private float JStimeA;

	private float JStimeB;

	private float timeC;

	[HideInInspector]
	public bool IsAttack;

	[HideInInspector]
	public bool IsSkill;

	[HideInInspector]
	public bool IsYun;

	[HideInInspector]
	public bool IsChong;

	public float DieDelay;

	public float MoveSpeed_Path;

	public TrackEntry MoveTrack;

	public TrackEntry AttackTrack;

	public TrackEntry SkillTrack;

	public List<Enemy> TargetList = new List<Enemy>();

	public Collider2D[] hitEM = new Collider2D[6];

	[HideInInspector]
	public CompA CompA;

	[HideInInspector]
	public CompB CompB;

	private CompanionBrain brain;

	private float followOffsetAngle;

	private float followOffsetRadius;

	[HideInInspector]
	public float AttackSpeed_JG_Last => AttackSpeed_JG / AttackSpeed_Last;

	[HideInInspector]
	public float SkillSpeed_Max => AttackSpeed_Base + (AttackSpeed_Last - AttackSpeed_Base) / 1.5f;

	private float SkillNormalAttackBonusRate
	{
		get
		{
			if (!PL)
			{
				return 0f;
			}
			if (PL.Level >= 90)
			{
				return 0.4f;
			}
			if (PL.Level >= 80)
			{
				return 0.3f;
			}
			if (PL.Level >= 70)
			{
				return 0.2f;
			}
			if (PL.Level >= 60)
			{
				return 0.1f;
			}
			return 0f;
		}
	}

	public float SkillDamageMultiplier
	{
		get
		{
			float num = (float)Mathf.Max(0, AT_DMG) / 100f;
			return (float)SK_DMG / 100f * (1f + num * SkillNormalAttackBonusRate);
		}
	}

	public bool canAttack
	{
		get
		{
			if (!MVTarget)
			{
				return false;
			}
			return Vector2.Distance(base.transform.position, MVTarget.position) <= CurrentAttackEnterRange;
		}
	}

	public bool AttackLost
	{
		get
		{
			if (!MVTarget)
			{
				return false;
			}
			return Vector2.Distance(base.transform.position, MVTarget.position) > Range_Attack_Cur + Range_AT_Hurt;
		}
	}

	public bool IsAlive => !IsDead;

	public bool IsDead { get; private set; }

	public CompanionDeathMode DeathMode { get; private set; }

	public float Range_Attack_Cur
	{
		get
		{
			if (NextAT_Type == 0)
			{
				if (Change_AT > 0f)
				{
					return Range_AT_Mid;
				}
				return Range_AT_Lit;
			}
			return Range_AT_Big;
		}
	}

	public bool IsCurrentAttackRanged
	{
		get
		{
			if (NextAT_Type == 1)
			{
				return true;
			}
			if (Change_AT > 0f)
			{
				return ATmod_Change != 0;
			}
			return ATmod_Sample != 0;
		}
	}

	public float CurrentAttackEnterRange => Range_Attack_Cur;

	public bool IsReady { get; private set; }

	public bool CompType
	{
		get
		{
			if (TryGetComponent<CompA>(out CompA))
			{
				return true;
			}
			if (TryGetComponent<CompB>(out CompB))
			{
				return false;
			}
			LogUtil.Error("companion未挂载控制器(CompA/B)");
			return false;
		}
	}

	public bool CanAcceptNewIntent
	{
		get
		{
			if (!IsAlive)
			{
				return false;
			}
			if (IsAttack || IsSkill || IsYun)
			{
				return false;
			}
			return true;
		}
	}

	public bool IsAttackCooldownReady
	{
		get
		{
			if (CompType)
			{
				if ((bool)CompA)
				{
					return CompA.atCD;
				}
				return false;
			}
			if ((bool)CompB)
			{
				return CompB.atCD;
			}
			return false;
		}
	}

	public CompanionIntentState CurrentIntent { get; private set; }

	public bool CanRefreshData
	{
		get
		{
			if (this != null && base.gameObject != null && base.gameObject.activeInHierarchy && HealthStat != null)
			{
				return PL != null;
			}
			return false;
		}
	}

	public event Action OnRequestPatrol;

	public event Action OnRequestIdle;

	public event Action OnRequestFollow;

	public event Action OnExitedFollow;

	public event Action<Enemy> OnEnterAttack;

	public event Action OnActionFinished;

	public List<Enemy> GetAttackableEnemies()
	{
		return TargetList;
	}

	public void ChangeIntent(CompanionIntentState newIntent)
	{
		if (CurrentIntent != newIntent)
		{
			CompanionIntentState currentIntent = CurrentIntent;
			CurrentIntent = newIntent;
			if (currentIntent == CompanionIntentState.Follow && newIntent != CompanionIntentState.Follow)
			{
				this.OnExitedFollow?.Invoke();
			}
		}
	}

	public void RequestCombatTarget(Enemy enemy)
	{
		if (IsAlive && (bool)enemy && enemy.IsAlive)
		{
			MVTarget = enemy.transform;
			ATTarget = (enemy.yao ? enemy.yao.transform : enemy.transform);
			PrepareNextAttackType();
			ChangeIntent(CompanionIntentState.Combat);
		}
	}

	public void NotifyActionFinished()
	{
		this.OnActionFinished?.Invoke();
	}

	public void BreakCombat()
	{
		MVTarget = null;
		ATTarget = null;
		IsAttack = false;
		IsSkill = false;
		IsYun = false;
		NextAT_Type = 0;
	}

	public bool HasReachedPatrolPoint(float threshold = 0.1f)
	{
		if (!XLpoint)
		{
			return true;
		}
		return Vector3.Distance(base.transform.position, XLpoint.transform.position) <= threshold;
	}

	public void GenerateNewPatrolPoint()
	{
		if (!PL)
		{
			return;
		}
		if (!XLpoint)
		{
			XLpoint = SingletonMonoScope<LevelManager>.Instance.CreatMovePoint(base.transform.position);
			if (!XLpoint)
			{
				return;
			}
		}
		Vector3 companionFollowBasePosition = PL.GetCompanionFollowBasePosition();
		Vector2 vector = UnityEngine.Random.insideUnitCircle * 1.5f;
		Vector3 position = new Vector3(companionFollowBasePosition.x + vector.x, companionFollowBasePosition.y + vector.y, 0f);
		XLpoint.transform.position = position;
	}

	public void RefreshFollowPoint()
	{
		if ((bool)PL)
		{
			EnsureMovePoint();
			if ((bool)XLpoint)
			{
				Vector2 vector = new Vector2(Mathf.Cos(followOffsetAngle), Mathf.Sin(followOffsetAngle)) * followOffsetRadius;
				Vector3 companionFollowBasePosition = PL.GetCompanionFollowBasePosition();
				XLpoint.transform.position = new Vector3(companionFollowBasePosition.x + vector.x, companionFollowBasePosition.y + vector.y, 0f);
			}
		}
	}

	public Transform GetFollowPoint()
	{
		RefreshFollowPoint();
		if (!XLpoint)
		{
			return null;
		}
		return XLpoint.transform;
	}

	public bool HasReachedFollowPoint()
	{
		if (!XLpoint)
		{
			return true;
		}
		return Vector3.Distance(base.transform.position, XLpoint.transform.position) <= 0.3f;
	}

	private void EnsureMovePoint()
	{
		if (!XLpoint && SingletonMonoScope<LevelManager>.HasInstance)
		{
			XLpoint = SingletonMonoScope<LevelManager>.Instance.CreatMovePoint(base.transform.position);
		}
	}

	public void RequestFollow()
	{
		if (IsAlive && CurrentIntent != CompanionIntentState.Follow)
		{
			ChangeIntent(CompanionIntentState.Follow);
			this.OnRequestFollow?.Invoke();
		}
	}

	public void RequestIdle()
	{
		if (IsAlive && CurrentIntent != CompanionIntentState.Idle)
		{
			ChangeIntent(CompanionIntentState.Idle);
			this.OnRequestIdle?.Invoke();
		}
	}

	public void RequestPatrol()
	{
		if (IsAlive && CurrentIntent != CompanionIntentState.Patrol)
		{
			ChangeIntent(CompanionIntentState.Patrol);
			this.OnRequestPatrol?.Invoke();
		}
	}

	public void RequestAttack(Enemy enemy)
	{
		if ((CurrentIntent != CompanionIntentState.Combat || !(MVTarget == enemy.transform)) && IsAlive && (bool)enemy && enemy.IsAlive)
		{
			MVTarget = enemy.transform;
			ATTarget = (enemy.yao ? enemy.yao.transform : enemy.transform);
			ChangeIntent(CompanionIntentState.Combat);
			PrepareNextAttackType();
			this.OnEnterAttack?.Invoke(enemy);
		}
	}

	public void PrepareNextAttackType()
	{
		if (ATSrate > 0f)
		{
			int num = UnityEngine.Random.Range(0, 101);
			NextAT_Type = (((float)num < ATSrate) ? 1 : 0);
		}
		else
		{
			NextAT_Type = 0;
		}
	}

	public void TeleportToPlayer()
	{
		base.transform.position = PL.transform.position;
	}

	public void InitComp()
	{
		MoveSpeed_Base = 2f;
		MoveSpeed_Bei = 0f;
		MoveSpeed_Cut = 0f;
		AttackSpeed_JG = 1f;
		AttackSpeed_Base = 1f;
		AttackSpeed_Bei = 0f;
		AttackSpeed_Cut = 0f;
		Damage_Bei = 0f;
		Damage_Cut = 0f;
		Health_Prc_Bei = 0f;
		Damage_Anti_Base = 0f;
		FlySpeed = 0f;
		if ((bool)SKB_OBJ)
		{
			SKB_OBJ.SetActive(value: false);
		}
		MoveTrack = new TrackEntry();
		AttackTrack = new TrackEntry();
		SkillTrack = new TrackEntry();
	}

	public void GongShi()
	{
		path.maxSpeed = MoveSpeed_Path * MoveSpeed_Last;
		AttackSpeed_Max = AttackSpeed_Base + AttackSpeed_Base * ((AttackSpeed_Bei + PL.C_ATSpeed_Last) / 100f);
		if (AttackSpeed_Cut >= 60f)
		{
			AttackSpeed_Last = AttackSpeed_Max * 0.4f;
		}
		else
		{
			AttackSpeed_Last = AttackSpeed_Max - AttackSpeed_Max * (AttackSpeed_Cut / 100f);
		}
		MoveSpeed_Max = MoveSpeed_Base + MoveSpeed_Base * ((MoveSpeed_Bei + PL.C_MVSpeed_Last) / 100f);
		if (MoveSpeed_Cut > 80f)
		{
			MoveSpeed_Last = MoveSpeed_Max * 0.2f;
		}
		else
		{
			MoveSpeed_Last = MoveSpeed_Max - MoveSpeed_Max * (MoveSpeed_Cut / 100f);
		}
		Damage_Max = Damage_Base + Damage_Bei + (float)BloodDie + PL.C_Damage_Last;
		if (Damage_Cut > 60f)
		{
			Damage_Last = Damage_Max * 0.4f;
		}
		else
		{
			Damage_Last = Damage_Max - Damage_Max * (Damage_Cut / 100f);
		}
		AllAnti_Max = AllAnti_Base + PL.C_AllAnti_Last;
		if (PL.CP_Same_RHeal)
		{
			Health_Prc_Max = Health_Prc_Base + Health_Prc_Bei + PL.Health_Percent_Last;
		}
		else
		{
			Health_Prc_Max = Health_Prc_Base + Health_Prc_Bei;
		}
		if (GeDang_Base > 80f)
		{
			GeDang_Max = 80f;
		}
		else
		{
			GeDang_Max = GeDang_Base;
		}
		if (Damage_Anti_Base > 90f)
		{
			Damage_Anti_Max = 90f;
		}
		else
		{
			Damage_Anti_Max = Damage_Anti_Base;
		}
	}

	private void Awake()
	{
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		if (!HealthStat && !base.transform.Find("Canvas/Image/Health").TryGetComponent<CompStat>(out HealthStat))
		{
			HealthStat = base.transform.Find("Canvas/Image/Health").gameObject.AddComponent<CompStat>();
		}
		headUp = base.transform.Find("main/FX up").gameObject;
		head = base.transform.Find("main/FX head").gameObject;
		body = base.transform.Find("main/FX BD").gameObject;
		yao = base.transform.Find("main/FX yao").gameObject;
		foot = base.transform.Find("shadow").gameObject;
		B_Col = base.transform.Find("main").GetComponent<CapsuleCollider2D>();
		F_Col = base.transform.Find("shadow").GetComponent<CircleCollider2D>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		BuffMG = base.transform.Find("People").GetComponent<BuffMG_CP>();
		sp = GetComponent<SkillOBJ_DT_SP>();
		path = GetComponent<AIPath>();
		if ((bool)SKB_OBJ)
		{
			dic = SKB_OBJ.GetComponent<Dicform>();
			dic.sp = sp;
		}
		if (!base.gameObject.TryGetComponent<CompanionBrain>(out var _))
		{
			brain = base.gameObject.AddComponent<CompanionBrain>();
		}
	}

	public void OnHealthInitialized()
	{
		IsReady = true;
		if (CurrentIntent == CompanionIntentState.None)
		{
			RequestIdle();
		}
		if ((bool)brain)
		{
			brain.ForceImmediateDecision();
		}
		HealthStat.OnZero -= OnHealthZero;
		HealthStat.OnZero += OnHealthZero;
		CanBeSystemDeleted = true;
	}

	private void OnEnable()
	{
		IsDead = false;
		DeathMode = CompanionDeathMode.Normal;
		CanBeSystemDeleted = false;
		universe = null;
		forever = null;
		round = null;
		XLpoint = SingletonMonoScope<LevelManager>.Instance.CreatMovePoint(base.transform.position);
		followOffsetAngle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		followOffsetRadius = UnityEngine.Random.Range(0.3f, 1f);
		JStimeA = 0f;
		JStimeB = 0f;
		timeC = 0f;
		Idle_Time_Tmp = 0f;
		NextAT_Type = 0;
		CanSO_Idle = false;
		IsYun = false;
		B_Col.enabled = true;
		F_Col.enabled = true;
		TargetList.Clear();
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		this.wait(1E-05f, SetStart);
		IsReady = false;
		HealthStat.OnInitialized += OnHealthInitialized;
		if (HealthStat.IsInitialized)
		{
			OnHealthInitialized();
		}
	}

	private void OnDisable()
	{
		ClearPermanentSkills();
		HealthStat.OnInitialized -= OnHealthInitialized;
	}

	public void OnHealthZero()
	{
		SetDie();
	}

	public void SetStart()
	{
		Idle_Time_Cur = UnityEngine.Random.Range(Idle_Time_Min, Idle_Time_Max);
		canvas.alpha = 1f;
		InitComp();
	}

	private void Update()
	{
		if (IsReady)
		{
			GongShi();
			JSQ();
		}
	}

	public void JSQ()
	{
		if (IsAlive)
		{
			JStimeA += Time.deltaTime;
			if (JStimeA >= 0.1f)
			{
				if (TargetList.Count > 0)
				{
					for (int i = 0; i < TargetList.Count; i++)
					{
						if (Vector2.Distance(base.transform.position, TargetList[i].transform.position) > Range_EM || !TargetList[i].IsAlive || TargetList[i].IsYS || TargetList[i].IsJump)
						{
							TargetList.Remove(TargetList[i]);
							i--;
						}
					}
					TargetList.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.transform.position, base.transform.position)));
					if ((bool)MVTarget)
					{
						ray = Physics2D.Raycast(base.transform.position, MVTarget.transform.position - base.transform.position, Vector2.Distance(base.transform.position, MVTarget.transform.position), LayerMask.GetMask("block"));
						if ((bool)ray.collider)
						{
							if (Vector2.Distance(base.transform.position, MVTarget.transform.position) < 1f)
							{
								CanSeeMVTarget = true;
							}
							else
							{
								CanSeeMVTarget = false;
							}
						}
						else
						{
							CanSeeMVTarget = true;
						}
					}
				}
				else
				{
					MVTarget = null;
				}
				int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, Range_EM, hitEM, LayerMask.GetMask("FootCOLem"));
				if (num > 0)
				{
					for (int j = 0; j < num; j++)
					{
						FootCOL component = hitEM[j].GetComponent<FootCOL>();
						if ((bool)component)
						{
							if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !TargetList.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS)
							{
								TargetList.Add(component.peo.em);
							}
							hitEM[j] = null;
						}
					}
				}
				JStimeA = 0f;
			}
			JStimeB += Time.deltaTime;
			if (JStimeB >= 1f)
			{
				if (HealthStat.CurrentValue < HealthStat.MaxValue)
				{
					HealthStat.SetCurrent(HealthStat.CurrentValue + HealthStat.MaxValue * Health_Prc_Max / 100f);
				}
				if (BloodDie > 0)
				{
					HealthStat.SetCurrent(HealthStat.CurrentValue - HealthStat.MaxValue * 0.03f);
				}
				JStimeB = 0f;
			}
			if (CanSO_Idle)
			{
				Idle_Time_Tmp += Time.deltaTime;
				if (Idle_Time_Tmp >= Idle_Time_Cur)
				{
					if (UnityEngine.Random.Range(0, 101) < SO_IdleRate)
					{
						RuntimeManager.PlayOneShot(SO_Idle, yao.transform.position);
					}
					Idle_Time_Cur = UnityEngine.Random.Range(Idle_Time_Min, Idle_Time_Max);
					Idle_Time_Tmp = 0f;
				}
			}
		}
		if (IsDead)
		{
			timeC += Time.deltaTime;
			if (timeC >= DieDelay)
			{
				LeanPool.Despawn(base.gameObject);
				timeC = 0f;
			}
		}
	}

	public void TakeDamage(float damage, float chuan, float BJrate, float BJDamage, float yun, DamageType type, Enemy em)
	{
		if (BloodDie > 0 && HealthStat.CurrentValue < HealthStat.MaxValue * 0.5f)
		{
			SetDie();
			return;
		}
		if ((float)UnityEngine.Random.Range(0, 101) < BJrate)
		{
			damage *= 2f + BJDamage / 100f;
		}
		if ((float)UnityEngine.Random.Range(0, 101) > GeDang_Max)
		{
			float num = 1f - (AllAnti_Max - chuan) / (100f + AllAnti_Max - chuan);
			float num2 = ((!(num <= 1f)) ? (damage * (1f - Damage_Anti_Max / 100f) + damage * (num - 1f) * 0.5f * (1f - Damage_Anti_Max / 100f)) : (damage * num * (1f - Damage_Anti_Max / 100f)));
			bool flag = HealthStat.CurrentValue > 0f;
			HealthStat.SetCurrent(HealthStat.CurrentValue - num2);
			SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num2, type, crit: false);
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.CreatACT_CPHurt(this);
				if (flag && HealthStat.CurrentValue <= 0f)
				{
					SingletonMonoScope<ACTbar>.Instance.CreatACT_CPDie(this);
				}
			}
			TryTriggerHurtReflect(em, type);
			if (TryTriggerTGYJ(em) || NoYun)
			{
				return;
			}
			int num3 = UnityEngine.Random.Range(0, 101);
			if (num2 > HealthStat.MaxValue / 10f && num2 < HealthStat.MaxValue / 5f)
			{
				if ((float)num3 < yun - (float)YunAnti)
				{
					IsYun = true;
				}
			}
			else if (num2 > HealthStat.MaxValue / 5f && num2 < HealthStat.MaxValue / 3f)
			{
				if ((double)num3 < (double)yun * 1.5 - (double)YunAnti)
				{
					IsYun = true;
				}
			}
			else if (num2 > HealthStat.MaxValue / 3f && num2 < HealthStat.MaxValue / 2f)
			{
				if ((float)num3 < yun * 2f - (float)YunAnti)
				{
					IsYun = true;
				}
			}
			else if (num2 > HealthStat.MaxValue / 2f)
			{
				IsYun = true;
			}
		}
		else
		{
			TryTriggerHurtReflect(em, type);
			TryTriggerGDHeal();
		}
	}

	public void TryTriggerKillHeal()
	{
		if (Kill_R_Heal > 0 && !(PL == null) && !(PL.HealStat == null) && !(PL.HealStat.Max <= 0f) && PL.IsAlive)
		{
			PL.HealStat.Cur = Mathf.Min(PL.HealStat.Cur + PL.HealStat.Max * (float)Kill_R_Heal / 100f, PL.HealStat.Max);
		}
	}

	private void TryTriggerGDHeal()
	{
		if (GD_R_Heal > 0 && !(PL == null) && !(PL.HealStat == null) && !(PL.HealStat.Max <= 0f))
		{
			PL.HealStat.Cur = Mathf.Min(PL.HealStat.Cur + PL.HealStat.Max * (float)GD_R_Heal / 100f, PL.HealStat.Max);
		}
	}

	private void TryTriggerHurtReflect(Enemy attacker, DamageType type)
	{
		if (Hurt_FT > 0 && (bool)attacker && !attacker.IsDpsTarget && attacker.IsAlive && !(PL == null) && !(PL.HealStat == null) && !(PL.HealStat.Max <= 0f))
		{
			float damage = PL.HealStat.Max * (float)Hurt_FT / 100f;
			attacker.TakeDamage(damage, PL.GiveChuan(type), PL.BJrate_Last, PL.BJDamage_Last, 0f, 0f, 100f, type, 1, PL, this);
		}
	}

	private bool TryTriggerTGYJ(Enemy attacker)
	{
		if (TGYJ <= 0 || !attacker || attacker.IsDpsTarget || !attacker.IsAlive || IsDead)
		{
			return false;
		}
		if (attacker.IS_Boss)
		{
			return false;
		}
		int num = Mathf.Clamp(TGYJ, 0, 100);
		if (attacker.Quality > 2)
		{
			num = Mathf.FloorToInt((float)num * 0.5f);
		}
		if (UnityEngine.Random.Range(0, 100) >= num)
		{
			return false;
		}
		attacker.HealthStat.SetCurrent(0f);
		SetDie();
		return true;
	}

	public void TakeDotDamage(DamageType type, float damage, float chuan)
	{
		float num = 1f - (AllAnti_Max - chuan) / (100f + AllAnti_Max - chuan);
		float num2 = ((!(num <= 1f)) ? (damage * (1f - Damage_Anti_Max / 100f) + damage * (num - 1f) * 0.5f * (1f - Damage_Anti_Max / 100f)) : (damage * num * (1f - Damage_Anti_Max / 100f)));
		HealthStat.SetCurrent(HealthStat.CurrentValue - num2);
		SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num2, type, crit: false);
	}

	public void SetSkin(int a)
	{
	}

	public void SetARSkill()
	{
		if ((bool)SKB_OBJ)
		{
			SKB_OBJ.SetActive(value: true);
		}
	}

	public void SetDie(CompanionDeathMode deathMode = CompanionDeathMode.Normal)
	{
		if (!IsDead)
		{
			ClearPermanentSkills();
			DeathMode = deathMode;
			if (deathMode != CompanionDeathMode.Dismiss)
			{
				TryCreateDeathDamageBoom();
			}
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RemoveFromCompList(sp.skillName, this);
			}
			IsDead = true;
			if (HealthStat.CurrentValue > 0f)
			{
				HealthStat.SetCurrent(0f);
			}
			path.canMove = false;
			IsAttack = false;
			IsSkill = false;
		}
	}

	private void TryCreateDeathDamageBoom()
	{
	}

	public void SystemDelete()
	{
		if (!CanBeSystemDeleted)
		{
			LogUtil.Info("尚未完成出生流程，系统删除被拦截");
		}
		else if (!IsDead)
		{
			ClearPermanentSkills();
			IsDead = true;
			path.canMove = false;
			IsAttack = false;
			IsSkill = false;
			GetComponent<FxControl_CPA>()?.DieFX(DieType);
			DeleteSelf();
		}
	}

	public void ClearUniverse()
	{
		if (universe == null)
		{
			return;
		}
		SK_CP_Universe[] array = universe;
		universe = null;
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i])
			{
				array[i].Stop();
			}
		}
	}

	public void ClearUniverse(int globalID)
	{
		if (universe == null)
		{
			return;
		}
		for (int num = universe.Length - 1; num >= 0; num--)
		{
			SK_CP_Universe sK_CP_Universe = universe[num];
			if (!sK_CP_Universe || !sK_CP_Universe.sp || sK_CP_Universe.sp.GlobalID == globalID)
			{
				RemoveUniverse(sK_CP_Universe);
				if ((bool)sK_CP_Universe)
				{
					sK_CP_Universe.Stop();
				}
			}
		}
	}

	public void AddUniverse(SK_CP_Universe item)
	{
		if ((bool)item)
		{
			RemoveUniverse(item);
			int num = ((universe != null) ? universe.Length : 0);
			SK_CP_Universe[] array = new SK_CP_Universe[num + 1];
			for (int i = 0; i < num; i++)
			{
				array[i] = universe[i];
			}
			array[num] = item;
			universe = array;
		}
	}

	public void RemoveUniverse(SK_CP_Universe item)
	{
		if (universe == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < universe.Length; i++)
		{
			if ((bool)universe[i] && universe[i] != item)
			{
				num++;
			}
		}
		if (num == 0)
		{
			universe = null;
			return;
		}
		SK_CP_Universe[] array = new SK_CP_Universe[num];
		int num2 = 0;
		for (int j = 0; j < universe.Length; j++)
		{
			if ((bool)universe[j] && universe[j] != item)
			{
				array[num2] = universe[j];
				num2++;
			}
		}
		universe = array;
	}

	public void ClearForever()
	{
		if (forever == null)
		{
			return;
		}
		SK_CP_Forever[] array = forever;
		forever = null;
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i])
			{
				array[i].Stop();
			}
		}
	}

	public void ClearForever(int globalID)
	{
		if (forever == null)
		{
			return;
		}
		for (int num = forever.Length - 1; num >= 0; num--)
		{
			SK_CP_Forever sK_CP_Forever = forever[num];
			if (!sK_CP_Forever || !sK_CP_Forever.sp || sK_CP_Forever.sp.GlobalID == globalID)
			{
				RemoveForever(sK_CP_Forever);
				if ((bool)sK_CP_Forever)
				{
					sK_CP_Forever.Stop();
				}
			}
		}
	}

	public void AddForever(SK_CP_Forever item)
	{
		if ((bool)item)
		{
			RemoveForever(item);
			int num = ((forever != null) ? forever.Length : 0);
			SK_CP_Forever[] array = new SK_CP_Forever[num + 1];
			for (int i = 0; i < num; i++)
			{
				array[i] = forever[i];
			}
			array[num] = item;
			forever = array;
		}
	}

	public void RemoveForever(SK_CP_Forever item)
	{
		if (forever == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < forever.Length; i++)
		{
			if ((bool)forever[i] && forever[i] != item)
			{
				num++;
			}
		}
		if (num == 0)
		{
			forever = null;
			return;
		}
		SK_CP_Forever[] array = new SK_CP_Forever[num];
		int num2 = 0;
		for (int j = 0; j < forever.Length; j++)
		{
			if ((bool)forever[j] && forever[j] != item)
			{
				array[num2] = forever[j];
				num2++;
			}
		}
		forever = array;
	}

	public void ClearPermanentSkill(int globalID)
	{
		ClearUniverse(globalID);
		ClearForever(globalID);
		ClearRound(globalID);
	}

	public void ClearPermanentSkills()
	{
		ClearUniverse();
		ClearForever();
		ClearRound();
	}

	public void ClearRound()
	{
		if (round == null)
		{
			return;
		}
		SK_CP_Round[] array = round;
		round = null;
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i])
			{
				array[i].Stop();
			}
		}
	}

	public void ClearRound(int globalID)
	{
		if (round == null)
		{
			return;
		}
		for (int num = round.Length - 1; num >= 0; num--)
		{
			SK_CP_Round sK_CP_Round = round[num];
			if (!sK_CP_Round || !sK_CP_Round.sp || sK_CP_Round.sp.GlobalID == globalID)
			{
				RemoveRound(sK_CP_Round);
				if ((bool)sK_CP_Round)
				{
					sK_CP_Round.Stop();
				}
			}
		}
	}

	public void AddRound(SK_CP_Round item)
	{
		if ((bool)item)
		{
			RemoveRound(item);
			int num = ((round != null) ? round.Length : 0);
			SK_CP_Round[] array = new SK_CP_Round[num + 1];
			for (int i = 0; i < num; i++)
			{
				array[i] = round[i];
			}
			array[num] = item;
			round = array;
		}
	}

	public void RemoveRound(SK_CP_Round item)
	{
		if (round == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < round.Length; i++)
		{
			if ((bool)round[i] && round[i] != item)
			{
				num++;
			}
		}
		if (num == 0)
		{
			round = null;
			return;
		}
		SK_CP_Round[] array = new SK_CP_Round[num];
		int num2 = 0;
		for (int j = 0; j < round.Length; j++)
		{
			if ((bool)round[j] && round[j] != item)
			{
				array[num2] = round[j];
				num2++;
			}
		}
		round = array;
	}

	public void RefreshData(ACT_skillData DT)
	{
		if (CanRefreshData)
		{
			int bStype = BStype;
			BStype = DT.comp.BStype;
			if (bStype != BStype)
			{
				GetComponent<FxControl_CPA>()?.ApplyColorData();
			}
			AT_ZD = DT.comp.AT_ZD;
			SK_ZD = DT.comp.SK_ZD;
			AT_DMG = DT.comp.AT_DMG;
			SK_DMG = DT.comp.SK_DMG;
			Damage_Base = DT.comp.Damage;
			AttackSpeed_Bei = DT.comp.AttackSpeed;
			GeDang_Base = DT.comp.GeDang;
			Health_Prc_Base = DT.comp.Health_Prc;
			damageType = DT.comp.damageType;
			damageType_Change = DT.comp.damageType_Change;
			Change_AT = DT.comp.Change_AT;
			ATSrate = DT.comp.ATSrate;
			ChangeEL_SK = DT.comp.ChangeEL_SK;
			ATS_Damage = DT.comp.ATS_Damage;
			ChangeEL_AR = DT.comp.ChangeEL_AR;
			ARS_Damage = DT.comp.ARS_Damage;
			DotMultiA = DT.comp.DotMultiA;
			DotMultiB = DT.comp.DotMultiB;
			GD_R_Heal = DT.comp.GD_R_Heal;
			BloodDie = DT.comp.BloodDie;
			TGYJ = DT.comp.TGYJ;
			Kill_R_Heal = DT.comp.Kill_R_Heal;
			Hurt_FT = DT.comp.Hurt_FT;
			AT_DotLayer = DT.comp.AT_DotLayer;
			BJ_NoDot = DT.comp.BJ_NoDot;
			WS_All = DT.comp.WS_All;
			Field_Range = DT.comp.Field_Range;
			MaxForceFollowDistance = DT.comp.DisA;
			MaxTeleportDistance = DT.comp.DisB;
			HealthStat.MaxValue = (DT.comp.Health + PL.Damage_Last * 3f + PL.HealStat.Max * 3f) * (1f + PL.C_Health_Last / 100f);
			sp.Damage = Damage_Last / 100f * PL.GiveDamage(damageType) * (float)AT_DMG / 100f;
			sp.DamageA = Damage_Last * ATS_Damage / Damage_Base / 100f * PL.GiveDamage(ChangeEL_SK) * SkillDamageMultiplier;
			sp.DamageB = Damage_Last * ARS_Damage / Damage_Base / 100f * PL.GiveDamage(ChangeEL_AR) * SkillDamageMultiplier;
			if (sp.DamageB > 0f && (bool)SKB_OBJ)
			{
				SKB_OBJ.SetActive(value: true);
			}
			sp.BJrate = (BJ_NoDot ? 100f : PL.BJrate_Last);
			sp.JYrate = PL.JYrate_Last;
			sp.Chuan = PL.GiveChuan(ChangeEL_AR);
			sp.BJDamage = PL.BJDamage_Last;
			sp.FlySpeed = FlySpeed;
			sp.AT_DotLayer = AT_DotLayer;
			sp.BJ_NoDot = BJ_NoDot;
			sp.WS_All = WS_All;
			sp.Field_Range = Field_Range;
			Count_A = DT.comp.Count_A;
			Count_B = DT.comp.Count_B;
			AT_Double = DT.comp.AT_Double;
			Count_ATtarget_A = DT.comp.Count_ATtarget_A;
			Count_ATtarget_B = DT.comp.Count_ATtarget_B;
			CountMulti_A = DT.comp.CountMulti_A;
			CountMulti_B = DT.comp.CountMulti_B;
		}
	}

	public void DeleteSelf()
	{
		ClearPermanentSkills();
		if ((bool)SKB_OBJ)
		{
			SKB_OBJ.SetActive(value: false);
		}
		LeanPool.Despawn(XLpoint);
		canvas.alpha = 0f;
		if (SkillBuffList.Count > 0)
		{
			foreach (SK_BuffA skillBuff in SkillBuffList)
			{
				skillBuff.StopBuff();
			}
		}
		SkillBuffList.Clear();
		BuffMG.DelAll();
		B_Col.enabled = false;
		F_Col.enabled = false;
	}
}
