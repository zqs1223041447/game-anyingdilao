using System;
using System.Collections.Generic;
using Core.Settings;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using Level.StateData.LevelStates;
using Pathfinding;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public EnemyState RuntimeState;

	private Vector3 spawnPosCached;

	public Action<Enemy> OnEnemyDie;

	private Companion lastDamageCompanion;

	[HideInInspector]
	public EnemyStat HealthStat;

	[HideInInspector]
	public CanvasGroup canvas;

	[HideInInspector]
	public PlayerManager playerManager;

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
	public Vector3 StartPS;

	[HideInInspector]
	public People peo;

	public RaycastHit2D ray;

	private readonly Collider2D[] critBoomHits = new Collider2D[5];

	private readonly Collider2D[] deadDotHits = new Collider2D[5];

	[HideInInspector]
	public BuffMG_EM BuffMG;

	[HideInInspector]
	public AIPath path;

	[HideInInspector]
	public AIDestinationSetter AIDS;

	[HideInInspector]
	public SkeletonAnimation spine;

	[HideInInspector]
	public MeshRenderer SpineRender;

	[HideInInspector]
	public Material mat;

	public MaterialPropertyBlock mpb;

	[HideInInspector]
	public Boss BS;

	[HideInInspector]
	public CapsuleCollider2D B_Col;

	[HideInInspector]
	public CircleCollider2D F_Col;

	[HideInInspector]
	public SpriteRenderer SD;

	[HideInInspector]
	public ItemManager itemManager;

	[HideInInspector]
	public int GlobalID;

	[HideInInspector]
	public string IndexName;

	[HideInInspector]
	public int Level;

	[HideInInspector]
	public int Xp;

	[HideInInspector]
	public int size;

	[HideInInspector]
	public float CompOffset;

	[HideInInspector]
	public float TuiSpeed;

	[HideInInspector]
	public float ItemDropPos;

	[HideInInspector]
	public int SpineType;

	[HideInInspector]
	public int MainElement;

	[HideInInspector]
	public DamageType MainELType;

	[HideInInspector]
	public int ColorIndex;

	[HideInInspector]
	public int CP_FX;

	[HideInInspector]
	public string SkinName;

	[HideInInspector]
	public int Flip;

	[HideInInspector]
	public Vector4 MainMix;

	[HideInInspector]
	public int MainHue;

	[HideInInspector]
	public float MainSat;

	[HideInInspector]
	[ColorUsage(true, true)]
	public Color MainColor;

	[HideInInspector]
	[ColorUsage(true, true)]
	public Color DisloveColor;

	[HideInInspector]
	[ColorUsage(true, true)]
	public Color AlphaColor;

	public int DieColor;

	[HideInInspector]
	public int RDcolor;

	[HideInInspector]
	public int EnemyType;

	[HideInInspector]
	public float Health_Base;

	[HideInInspector]
	public float Health_Bei;

	[HideInInspector]
	public float AT_Idle_Min;

	[HideInInspector]
	public float AT_Idle_Max;

	[HideInInspector]
	public float AT_Idle_Cur;

	[HideInInspector]
	public float AttackSpeed_JG;

	[HideInInspector]
	public float AttackSpeed_Base;

	[HideInInspector]
	public float AttackSpeed_Bei;

	[HideInInspector]
	public float AttackSpeed_Cut;

	[HideInInspector]
	public float AttackSpeed_Last;

	public float MoveSpeed_Base;

	[HideInInspector]
	public float MoveSpeed_Bei;

	[HideInInspector]
	public float MoveSpeed_Cut;

	[HideInInspector]
	public float MoveSpeed_Last;

	[HideInInspector]
	public float ChongSpeedMulti;

	[HideInInspector]
	public float BJRate;

	[HideInInspector]
	public float GeDang;

	[HideInInspector]
	public float yunAnti;

	[HideInInspector]
	public float yunAnti_Last;

	[HideInInspector]
	public float yunAnti_Cut;

	[HideInInspector]
	public int yunAntiCut_Layer;

	[HideInInspector]
	public float Damage_Base;

	[HideInInspector]
	public float Damage_Bei;

	[HideInInspector]
	public float Damage_Cut;

	[HideInInspector]
	public float Damage_Last;

	[HideInInspector]
	public float FireAnti;

	[HideInInspector]
	public float FrozenAnti;

	[HideInInspector]
	public float ThunderAnti;

	[HideInInspector]
	public float PoisonAnti;

	[HideInInspector]
	public float PhysicsAnti;

	[HideInInspector]
	public float ShadowAnti;

	[HideInInspector]
	public float FireAnti_Last;

	[HideInInspector]
	public float FrozenAnti_Last;

	[HideInInspector]
	public float ThunderAnti_Last;

	[HideInInspector]
	public float PoisonAnti_Last;

	[HideInInspector]
	public float PhysicsAnti_Last;

	[HideInInspector]
	public float ShadowAnti_Last;

	[HideInInspector]
	public float FireAntiCut_Simple;

	[HideInInspector]
	public float FrozenAntiCut_Simple;

	[HideInInspector]
	public float ThunderAntiCut_Simple;

	[HideInInspector]
	public float PoisonAntiCut_Simple;

	[HideInInspector]
	public float PhysicsAntiCut_Simple;

	[HideInInspector]
	public float ShadowAntiCut_Simple;

	[HideInInspector]
	public float FireAntiCut_Dot;

	[HideInInspector]
	public float FrozenAntiCut_Dot;

	[HideInInspector]
	public float ThunderAntiCut_Dot;

	[HideInInspector]
	public float PoisonAntiCut_Dot;

	[HideInInspector]
	public float PhysicsAntiCut_Dot;

	[HideInInspector]
	public float ShadowAntiCut_Dot;

	[HideInInspector]
	public int FireAntiCut_Layer;

	[HideInInspector]
	public int FrozenAntiCut_Layer;

	[HideInInspector]
	public int ThunderAntiCut_Layer;

	[HideInInspector]
	public int PoisonAntiCut_Layer;

	[HideInInspector]
	public int PhysicsAntiCut_Layer;

	[HideInInspector]
	public int ShadowAntiCut_Layer;

	[HideInInspector]
	public float Through;

	[HideInInspector]
	public float Chuan;

	[HideInInspector]
	public float Health_Prc;

	[HideInInspector]
	public float DamageAnti;

	[HideInInspector]
	public float FlySpeed;

	[HideInInspector]
	public float DotDamage;

	[HideInInspector]
	public float DotTime;

	[HideInInspector]
	public float AntiSlow;

	[HideInInspector]
	public float DotTimeCut;

	[HideInInspector]
	public bool ELSS_Break;

	[HideInInspector]
	public int HurtEL;

	[HideInInspector]
	public float H_HurtDMG_Buff;

	[HideInInspector]
	public int Quality;

	[HideInInspector]
	public int[] SSIndex = new int[5];

	public List<GameObject> AuraList = new List<GameObject>();

	public SK_BloodPool LQJQ;

	public int LQtype;

	[HideInInspector]
	public int Comp_EveryCount;

	[HideInInspector]
	public int Comp_Count;

	public List<Enemy> cpList = new List<Enemy>();

	[HideInInspector]
	public int FS_EveryCount;

	[HideInInspector]
	public int FS_Count;

	public List<Enemy> fsList = new List<Enemy>();

	public bool UseBrainAI;

	[HideInInspector]
	public Transform BrainMovePoint;

	[HideInInspector]
	public bool hasTarget;

	[HideInInspector]
	public bool CanSeeTarget;

	public List<Companion> CompCandidates = new List<Companion>();

	[HideInInspector]
	public int Anger;

	[HideInInspector]
	public float Range_Base;

	[HideInInspector]
	public float Range_Anger;

	[HideInInspector]
	public float Range_Far;

	[HideInInspector]
	public float Range_ATplayer_multi;

	[HideInInspector]
	public float Range_ATplayer_multi_B;

	[HideInInspector]
	public bool UseRange_ATplayer_multi_B;

	private const float BossTargetPriorityMultiBScale = 8f;

	private const float BossTargetPriorityMultiBAdd = 6f;

	[HideInInspector]
	public bool CanSeeMVTarget;

	[HideInInspector]
	public bool attackPL;

	private float _distToPlayer;

	private float _distToTarget;

	[HideInInspector]
	public int CF_Rate;

	[HideInInspector]
	public int SK_Cur_Index;

	[HideInInspector]
	public int HitFX;

	[HideInInspector]
	public EM_Skill_SP SK_AT;

	[HideInInspector]
	public int AT_Ani;

	[HideInInspector]
	public bool AT_Fang;

	[HideInInspector]
	public float AT_Distans;

	[HideInInspector]
	public EM_Skill_SP SK_A;

	[HideInInspector]
	public int SK_Ani;

	[HideInInspector]
	public bool SK_Fang;

	[HideInInspector]
	public float SK_Distans;

	[HideInInspector]
	public EM_Skill_CP SK_Comp;

	[HideInInspector]
	public EM_Skill_FS SK_FS;

	[HideInInspector]
	public EM_Skill_SP SK_Die;

	[HideInInspector]
	public EM_Skill_SP SK_ELSS;

	[HideInInspector]
	public int ELSS_Ani;

	[HideInInspector]
	public bool ELSS_Fang;

	[HideInInspector]
	public float ELSS_Distans;

	[HideInInspector]
	public EM_Skill_SP SK_Sustain;

	[HideInInspector]
	public int SK_Rate;

	[HideInInspector]
	public int SK_Rate_Comp;

	[HideInInspector]
	public int SK_Rate_FS;

	[HideInInspector]
	public int SK_Rate_ELSS;

	[HideInInspector]
	public bool Can_DieBoom;

	public EM_FXsustain FXsustain;

	[HideInInspector]
	public int SPtype;

	[HideInInspector]
	public int Die_Index;

	[HideInInspector]
	public int DieType;

	[HideInInspector]
	public int DiePos;

	[HideInInspector]
	public float DieFX_TimeDelay;

	[HideInInspector]
	public float DieDelay;

	[HideInInspector]
	public bool CanLie;

	[HideInInspector]
	public int Lie_Index;

	[HideInInspector]
	public int LiePos;

	[HideInInspector]
	public int FSDie_Index;

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
	public string SO_ChuiDi;

	public StudioEventEmitter emitter;

	[HideInInspector]
	public bool IsAttack;

	[HideInInspector]
	public bool IsMove;

	[HideInInspector]
	public bool IsYun;

	[HideInInspector]
	public bool IsBaTi;

	[HideInInspector]
	public bool IsWuDi;

	[HideInInspector]
	public bool IsJump;

	[HideInInspector]
	public bool IsYS;

	[HideInInspector]
	public bool IsChong;

	[HideInInspector]
	public bool IsFang;

	[HideInInspector]
	public bool IS_Frozen;

	[HideInInspector]
	public float FrozenTime;

	[HideInInspector]
	public float FrozenJSTime;

	[HideInInspector]
	public bool IS_Boss;

	[HideInInspector]
	public bool IS_Comp;

	[HideInInspector]
	public bool IS_FS;

	[HideInInspector]
	public bool IsDpsTarget;

	[HideInInspector]
	public bool IsBattle;

	[HideInInspector]
	public float BattleTime;

	private bool HurtOK;

	public bool EMstartOK;

	[HideInInspector]
	public Enemy Father;

	[HideInInspector]
	public GameObject UseFX_OBJ;

	public List<SK_BuffA> SkillBuffList = new List<SK_BuffA>();

	public GameObject[] Spirit;

	public float MoveSpeed_Path;

	public TrackEntry MoveTrack;

	public TrackEntry AttackTrack;

	public TrackEntry SkillTrack;

	public Collider2D[] hitCP = new Collider2D[6];

	private float JStimeA;

	private float JStimeB;

	private float JStimeC;

	private float timeC;

	private float timeF;

	[HideInInspector]
	public SKprefab PB;

	private Transform mapIconRoot;

	private SpriteRenderer mapIconRenderer;

	private static bool s_isQuitting;

	public bool IsTower => EnemyType == 100;

	public EnemyPoint OwnerPoint { get; private set; }

	public Vector3 SpawnPos
	{
		get
		{
			if ((bool)Father && (bool)Father.gameObject)
			{
				return Father.transform.position;
			}
			return spawnPosCached;
		}
	}

	[HideInInspector]
	public float Health_Max => Health_Base + Health_Base * Health_Bei / 100f;

	[HideInInspector]
	public float AttackSpeed_JG_Last => AttackSpeed_JG / AttackSpeed_Last;

	[HideInInspector]
	public float AttackSpeed_Max => AttackSpeed_Base + AttackSpeed_Base * (AttackSpeed_Bei / 100f);

	[HideInInspector]
	public float SkillSpeed_Max => AttackSpeed_Base + (AttackSpeed_Last - AttackSpeed_Base) / 2f;

	[HideInInspector]
	public float MoveSpeed_Max => MoveSpeed_Base + MoveSpeed_Base * (MoveSpeed_Bei / 100f);

	[HideInInspector]
	public float Damage_Max => Damage_Base + Damage_Base * (Damage_Bei / 100f);

	public float H_HurtDMG
	{
		get
		{
			float num = H_HurtDMG_Buff;
			if (IS_Frozen && peo != null && peo.DotEM != null)
			{
				num += (float)peo.DotEM.GerDotFrozenHurtDMG();
			}
			if (HealthStat.CurrentValue < HealthStat.MaxValue * 0.2f)
			{
				return (float)(playerManager.EM_LowH_DMG20 + playerManager.EM_LowH_DMG50) + num;
			}
			if (HealthStat.CurrentValue < HealthStat.MaxValue * 0.5f)
			{
				return (float)playerManager.EM_LowH_DMG50 + num;
			}
			if (HealthStat.CurrentValue + 1f > HealthStat.MaxValue)
			{
				return (float)(playerManager.EM_HighH_DMG60 + playerManager.EM_HighH_DMG100) + num;
			}
			if (HealthStat.CurrentValue > HealthStat.MaxValue * 0.6f)
			{
				return (float)playerManager.EM_HighH_DMG60 + num;
			}
			return num;
		}
	}

	public float AttackEnterRange => Mathf.Max(0.05f, Range_AT - 0.2f);

	public float AttackExitRange => Mathf.Max(AttackEnterRange, Range_AT + 0.35f);

	public bool InAttackRange => DistToTarget <= AttackEnterRange;

	public bool KeepAttackRange => DistToTarget <= AttackExitRange;

	public bool hadTarget
	{
		get
		{
			if (CompCandidates.Count <= 0)
			{
				return _distToPlayer < Range_Cur;
			}
			return true;
		}
	}

	public float Range_Cur
	{
		get
		{
			if (Anger > 0)
			{
				return Range_Base + Range_Anger;
			}
			return Range_Base;
		}
	}

	public bool UnderPlayer => _distToPlayer < Range_Cur;

	public bool FarAway
	{
		get
		{
			if (_distToPlayer > Range_Far)
			{
				return CompCandidates.Count == 0;
			}
			return false;
		}
	}

	private float Range_ATplayer_multi_Cur
	{
		get
		{
			if (!UseRange_ATplayer_multi_B)
			{
				return Range_ATplayer_multi;
			}
			return GetBossTargetPriorityMultiB();
		}
	}

	private float Range_ATplayer_compare_Dist
	{
		get
		{
			if (!UseRange_ATplayer_multi_B)
			{
				return _distToTarget;
			}
			return Mathf.Max(1f, GetClosestCompanionDistance());
		}
	}

	public bool nearPlayer => _distToPlayer < Range_ATplayer_multi_Cur * Range_ATplayer_compare_Dist;

	public bool canXL
	{
		get
		{
			if (_distToPlayer < Range_Far && _distToPlayer > Range_Cur)
			{
				return CompCandidates.Count == 0;
			}
			return false;
		}
	}

	public bool canAttack => RemainingDistanceToTarget <= AttackEnterRange;

	public bool canKeepAttack => RemainingDistanceToTarget <= AttackExitRange;

	private float RemainingDistanceToTarget
	{
		get
		{
			if (!path || !hasTarget || !MVTarget)
			{
				return float.PositiveInfinity;
			}
			float remainingDistance = path.remainingDistance;
			if (float.IsNaN(remainingDistance) || float.IsInfinity(remainingDistance) || remainingDistance <= 0f)
			{
				return DistToTarget;
			}
			return remainingDistance;
		}
	}

	public float DistToTarget
	{
		get
		{
			if (!hasTarget || !MVTarget)
			{
				return float.PositiveInfinity;
			}
			return Vector2.Distance(base.transform.position, MVTarget.position);
		}
	}

	public bool AttackLost
	{
		get
		{
			if (!MVTarget)
			{
				return true;
			}
			float sqrMagnitude = (MVTarget.position - base.transform.position).sqrMagnitude;
			float num = Range_AT + Range_AT_Hurt;
			return sqrMagnitude > num * num;
		}
	}

	[HideInInspector]
	public int SK_Rate_CompFS => SK_Rate_Comp + SK_Rate_FS;

	public float Range_AT_Hurt
	{
		get
		{
			switch (SK_Cur_Index)
			{
			case 0:
				if (SK_AT != null)
				{
					return AT_Distans * 0.1f;
				}
				break;
			case 1:
				if (SK_A != null)
				{
					return SK_Distans * 0.2f;
				}
				break;
			case 4:
				if (SK_ELSS != null)
				{
					return ELSS_Distans * 0.3f;
				}
				break;
			}
			return 0.1f;
		}
	}

	public float Range_AT
	{
		get
		{
			switch (SK_Cur_Index)
			{
			case 0:
				if (SK_AT != null)
				{
					return AT_Distans;
				}
				break;
			case 1:
				if (SK_A != null)
				{
					return SK_Distans;
				}
				break;
			case 2:
				if (SK_Comp != null)
				{
					return Range_Cur;
				}
				break;
			case 3:
				if (SK_FS != null)
				{
					return Range_Cur;
				}
				break;
			case 4:
				if (SK_ELSS != null)
				{
					return ELSS_Distans;
				}
				break;
			}
			return 3f;
		}
	}

	public bool IsAlive => HealthStat.CurrentValue > 0f;

	public static event Action OnPlayerCritDamageEnemy;

	public event Action OnDirectDamaged;

	public void BindSpawnPoint(EnemyPoint point)
	{
		if (!IsTower)
		{
			OwnerPoint = point;
			if ((bool)Father && (bool)Father.gameObject)
			{
				spawnPosCached = Father.transform.position;
			}
			else
			{
				spawnPosCached = (point ? point.transform.position : base.transform.position);
			}
			StartPS = spawnPosCached;
		}
	}

	public void UnbindSpawnPoint()
	{
		OwnerPoint = null;
	}

	public float DistanceToPoint()
	{
		if ((bool)Father && (bool)Father.gameObject)
		{
			return Vector3.Distance(base.transform.position, Father.transform.position);
		}
		Vector3 b = (OwnerPoint ? OwnerPoint.transform.position : SpawnPos);
		return Vector3.Distance(base.transform.position, b);
	}

	public void UpdateStateHp(float hp)
	{
		RuntimeState.Hp = hp;
	}

	public void UpdateStatePos(Vector3 pos)
	{
		RuntimeState.Position = pos;
	}

	public void SetBrainTarget(Transform mvTarget, Transform atTarget, bool isPlayer)
	{
		EnsureBrainMovePoint();
		if (!mvTarget)
		{
			hasTarget = false;
			MVTarget = BrainMovePoint;
			ATTarget = null;
			attackPL = false;
			if ((bool)AIDS)
			{
				AIDS.target = MVTarget;
			}
		}
		else
		{
			hasTarget = true;
			MVTarget = mvTarget;
			ATTarget = atTarget;
			attackPL = isPlayer;
			if ((bool)AIDS)
			{
				AIDS.target = MVTarget;
			}
		}
	}

	public void CanShu()
	{
		AttackSpeed_Cut = 0f;
		MoveSpeed_Cut = 0f;
		H_HurtDMG_Buff = 0f;
		yunAnti_Cut = 0f;
		yunAntiCut_Layer = 0;
		Damage_Cut = 0f;
		FireAntiCut_Simple = 0f;
		FrozenAntiCut_Simple = 0f;
		ThunderAntiCut_Simple = 0f;
		PoisonAntiCut_Simple = 0f;
		PhysicsAntiCut_Simple = 0f;
		ShadowAntiCut_Simple = 0f;
		FireAntiCut_Dot = 0f;
		FrozenAntiCut_Dot = 0f;
		ThunderAntiCut_Dot = 0f;
		PoisonAntiCut_Dot = 0f;
		PhysicsAntiCut_Dot = 0f;
		ShadowAntiCut_Dot = 0f;
		FireAntiCut_Layer = 0;
		FrozenAntiCut_Layer = 0;
		ThunderAntiCut_Layer = 0;
		PoisonAntiCut_Layer = 0;
		PhysicsAntiCut_Layer = 0;
		ShadowAntiCut_Layer = 0;
		ChongSpeedMulti = 1f;
		ResetBossTargetPriorityMulti();
		FrozenTime = 0f;
		FrozenJSTime = 0f;
		IS_Frozen = false;
		IsMove = false;
		MoveTrack = new TrackEntry();
		AttackTrack = new TrackEntry();
		SkillTrack = new TrackEntry();
	}

	public void ClearActionState()
	{
		IsAttack = false;
		IsChong = false;
		IsJump = false;
		IsYS = false;
		IsWuDi = false;
		IsBaTi = false;
		IsFang = false;
		ChongSpeedMulti = 1f;
	}

	public void ResetBossTargetPriorityMulti()
	{
		UseRange_ATplayer_multi_B = false;
		Range_ATplayer_multi_B = GetBossTargetPriorityMultiBDefault();
	}

	public void ClearBossTargetPriorityMultiB()
	{
		UseRange_ATplayer_multi_B = false;
	}

	public bool TryEnableBossTargetPriorityMultiB(float chance)
	{
		if (!IS_Boss || UseRange_ATplayer_multi_B || chance <= 0f)
		{
			return false;
		}
		if (!playerManager || !playerManager.IsAlive || CompCandidates.Count == 0)
		{
			return false;
		}
		float closestCompanionDistance = GetClosestCompanionDistance();
		if (float.IsInfinity(closestCompanionDistance))
		{
			return false;
		}
		closestCompanionDistance = Mathf.Max(1f, closestCompanionDistance);
		Range_ATplayer_multi_B = GetBossTargetPriorityMultiB();
		if (Vector2.Distance(base.transform.position, playerManager.transform.position) >= Range_ATplayer_multi_B * closestCompanionDistance)
		{
			return false;
		}
		if (UnityEngine.Random.value >= chance)
		{
			return false;
		}
		UseRange_ATplayer_multi_B = true;
		return true;
	}

	private float GetClosestCompanionDistance()
	{
		float num = float.PositiveInfinity;
		for (int i = 0; i < CompCandidates.Count; i++)
		{
			Companion companion = CompCandidates[i];
			if ((bool)companion && companion.IsAlive)
			{
				float num2 = Vector2.Distance(base.transform.position, companion.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	private float GetBossTargetPriorityMultiB()
	{
		return Mathf.Max(Range_ATplayer_multi_B, GetBossTargetPriorityMultiBDefault());
	}

	private float GetBossTargetPriorityMultiBDefault()
	{
		return Mathf.Max(Range_ATplayer_multi * 8f, Range_ATplayer_multi + 6f);
	}

	public void GongShi()
	{
		if (IsAlive)
		{
			if (IS_Frozen)
			{
				AttackSpeed_Last = 0f;
				MoveSpeed_Last = 0f;
				SetAni();
				FrozenJSTime += Time.deltaTime;
				if (FrozenJSTime >= FrozenTime)
				{
					IS_Frozen = false;
					FrozenJSTime = 0f;
					peo?.DotEM?.ReleaseFrozenDotFxIfIdle();
				}
			}
			else
			{
				if (AttackSpeed_Cut >= 60f)
				{
					AttackSpeed_Last = AttackSpeed_Max * 0.4f;
				}
				else
				{
					AttackSpeed_Last = AttackSpeed_Max - AttackSpeed_Max * (AttackSpeed_Cut / 100f) * (1f - AntiSlow / 100f);
				}
				if (MoveSpeed_Cut > 80f)
				{
					MoveSpeed_Last = MoveSpeed_Max * 0.2f * ChongSpeedMulti;
				}
				else
				{
					MoveSpeed_Last = (MoveSpeed_Max - MoveSpeed_Max * (MoveSpeed_Cut / 100f) * (1f - AntiSlow / 100f)) * ChongSpeedMulti;
				}
			}
			if (!IS_Frozen)
			{
				RefreshSpeedAndSetAni();
			}
		}
		else
		{
			if (AttackSpeed_Cut >= 60f + playerManager.Top_Cut_ATS)
			{
				AttackSpeed_Last = AttackSpeed_Max * (1f - (60f + playerManager.Top_Cut_ATS) / 100f);
			}
			else
			{
				AttackSpeed_Last = AttackSpeed_Max - AttackSpeed_Max * (AttackSpeed_Cut / 100f) * (1f - AntiSlow / 100f);
			}
			if (MoveSpeed_Cut > 80f + playerManager.Top_Cut_MVS)
			{
				MoveSpeed_Last = MoveSpeed_Max * (1f - (80f + playerManager.Top_Cut_MVS) / 100f) * ChongSpeedMulti;
			}
			else
			{
				MoveSpeed_Last = (MoveSpeed_Max - MoveSpeed_Max * (MoveSpeed_Cut / 100f) * (1f - AntiSlow / 100f)) * ChongSpeedMulti;
			}
		}
		if ((bool)path)
		{
			path.maxSpeed = MoveSpeed_Path * MoveSpeed_Last;
		}
		if (Damage_Cut > 60f + playerManager.Top_Cut_DMG)
		{
			Damage_Last = Damage_Max * (1f - (60f + playerManager.Top_Cut_DMG) / 100f);
		}
		else
		{
			Damage_Last = Damage_Max - Damage_Max * (Damage_Cut / 100f);
		}
		if (yunAnti - yunAnti_Cut * (float)yunAntiCut_Layer > 0f)
		{
			yunAnti_Last = yunAnti - yunAnti_Cut * (float)yunAntiCut_Layer;
		}
		else
		{
			yunAnti_Last = 0f;
		}
		if (FireAnti - FireAntiCut_Simple - FireAntiCut_Dot * (float)FireAntiCut_Layer > -30f)
		{
			FireAnti_Last = FireAnti - FireAntiCut_Simple - FireAntiCut_Dot * (float)FireAntiCut_Layer;
		}
		else
		{
			FireAnti_Last = -30f;
		}
		if (FrozenAnti - FrozenAntiCut_Simple - FrozenAntiCut_Dot * (float)FrozenAntiCut_Layer > -30f)
		{
			FrozenAnti_Last = FrozenAnti - FrozenAntiCut_Simple - FrozenAntiCut_Dot * (float)FrozenAntiCut_Layer;
		}
		else
		{
			FrozenAnti_Last = -30f;
		}
		if (ThunderAnti - ThunderAntiCut_Simple - ThunderAntiCut_Dot * (float)ThunderAntiCut_Layer > -30f)
		{
			ThunderAnti_Last = ThunderAnti - ThunderAntiCut_Simple - ThunderAntiCut_Dot * (float)ThunderAntiCut_Layer;
		}
		else
		{
			ThunderAnti_Last = -30f;
		}
		if (PoisonAnti - PoisonAntiCut_Simple - PoisonAntiCut_Dot * (float)PoisonAntiCut_Layer > -30f)
		{
			PoisonAnti_Last = PoisonAnti - PoisonAntiCut_Simple - PoisonAntiCut_Dot * (float)PoisonAntiCut_Layer;
		}
		else
		{
			PoisonAnti_Last = -30f;
		}
		if (PhysicsAnti - PhysicsAntiCut_Simple - PhysicsAntiCut_Dot * (float)PhysicsAntiCut_Layer > -30f)
		{
			PhysicsAnti_Last = PhysicsAnti - PhysicsAntiCut_Simple - PhysicsAntiCut_Dot * (float)PhysicsAntiCut_Layer;
		}
		else
		{
			PhysicsAnti_Last = -30f;
		}
		if (ShadowAnti - ShadowAntiCut_Simple - ShadowAntiCut_Dot * (float)ShadowAntiCut_Layer > -30f)
		{
			ShadowAnti_Last = ShadowAnti - ShadowAntiCut_Simple - ShadowAntiCut_Dot * (float)ShadowAntiCut_Layer;
		}
		else
		{
			ShadowAnti_Last = -30f;
		}
	}

	private void EnsureMapIcon()
	{
		if ((bool)mapIconRoot && (bool)mapIconRenderer)
		{
			return;
		}
		Transform transform = base.transform.Find("Map");
		if ((bool)transform)
		{
			mapIconRoot = transform;
			mapIconRenderer = transform.GetComponent<SpriteRenderer>();
			if (!mapIconRenderer)
			{
				mapIconRenderer = transform.gameObject.AddComponent<SpriteRenderer>();
			}
			return;
		}
		GameObject gameObject = new GameObject("Map");
		mapIconRoot = gameObject.transform;
		mapIconRoot.SetParent(base.transform, worldPositionStays: false);
		mapIconRoot.localPosition = Vector3.zero;
		mapIconRoot.localRotation = Quaternion.identity;
		mapIconRenderer = gameObject.AddComponent<SpriteRenderer>();
		int num = LayerMask.NameToLayer("NOrender");
		if (num != -1)
		{
			gameObject.layer = num;
		}
		else
		{
			Debug.LogError("没有找到 NOrender 层，请先在 Unity 的 Layer 里创建它。");
		}
	}

	public void RefreshMapIcon()
	{
		EnsureMapIcon();
		if ((bool)mapIconRoot && (bool)mapIconRenderer)
		{
			if (!mapIconRenderer.enabled)
			{
				mapIconRenderer.enabled = true;
			}
			Vector3 enemyFinalScale = SettingsLoader.Instance.iconSettings.GetEnemyFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
			Vector3 bossFinalScale = SettingsLoader.Instance.iconSettings.GetBossFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
			if (IS_Boss && (bool)SettingsLoader.Instance.iconSettings.boss)
			{
				Vector3 lossyScale = base.transform.lossyScale;
				mapIconRoot.localScale = new Vector3((lossyScale.x != 0f) ? (bossFinalScale.x / lossyScale.x) : bossFinalScale.x, (lossyScale.y != 0f) ? (bossFinalScale.y / lossyScale.y) : bossFinalScale.y, (lossyScale.z != 0f) ? (bossFinalScale.z / lossyScale.z) : bossFinalScale.z);
				mapIconRenderer.sprite = SettingsLoader.Instance.iconSettings.boss;
				mapIconRenderer.color = SettingsLoader.Instance.iconSettings.bossColor;
			}
			else if ((bool)SettingsLoader.Instance.iconSettings.enemy)
			{
				Vector3 lossyScale2 = base.transform.lossyScale;
				mapIconRoot.localScale = new Vector3((lossyScale2.x != 0f) ? (enemyFinalScale.x / lossyScale2.x) : enemyFinalScale.x, (lossyScale2.y != 0f) ? (enemyFinalScale.y / lossyScale2.y) : enemyFinalScale.y, (lossyScale2.z != 0f) ? (enemyFinalScale.z / lossyScale2.z) : enemyFinalScale.z);
				mapIconRenderer.sprite = SettingsLoader.Instance.iconSettings.enemy;
				mapIconRenderer.color = SettingsLoader.Instance.iconSettings.enemyColor;
			}
			else
			{
				mapIconRenderer.sprite = null;
			}
		}
	}

	private void Awake()
	{
		EnsureMapIcon();
		UseBrainAI = (bool)GetComponent<EnemyA>() || (bool)GetComponent<EnemyB>();
		if (IsTower)
		{
			UseBrainAI = false;
		}
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		HealthStat = base.transform.Find("Canvas/Image").transform.Find("Health").GetComponent<EnemyStat>();
		headUp = base.transform.Find("main/FX up").gameObject;
		head = base.transform.Find("main/FX head").gameObject;
		body = base.transform.Find("main/FX BD").gameObject;
		yao = base.transform.Find("main/FX yao").gameObject;
		foot = base.transform.Find("shadow").gameObject;
		SD = base.transform.Find("shadow").GetComponent<SpriteRenderer>();
		B_Col = base.transform.Find("main").GetComponent<CapsuleCollider2D>();
		F_Col = base.transform.Find("shadow").GetComponent<CircleCollider2D>();
		playerManager = (playerManager = SingletonMonoScope<PlayerManager>.Instance);
		BuffMG = base.transform.Find("People").GetComponent<BuffMG_EM>();
		path = GetComponent<AIPath>();
		AIDS = GetComponent<AIDestinationSetter>();
		PB = SingletonMonoScope<GameDataManager>.Instance.SKPB;
		itemManager = SingletonMonoScope<ItemManager>.Instance;
		if (UseBrainAI)
		{
			EnsureBrainMovePoint();
		}
	}

	private void OnEnable()
	{
		if (UseBrainAI)
		{
			EnsureBrainMovePoint();
			BrainMovePoint.position = base.transform.position;
			if ((bool)AIDS)
			{
				AIDS.target = BrainMovePoint;
			}
		}
		XLpoint = SingletonMonoScope<LevelManager>.Instance.CreatMovePoint(base.transform.position);
		JStimeA = 0f;
		JStimeB = 0f;
		JStimeC = 0f;
		timeC = 0f;
		timeF = 0f;
		CanLie = false;
		IsYun = false;
		IsBaTi = false;
		IsWuDi = false;
		IsYS = false;
		IsJump = false;
		IsChong = false;
		IsFang = false;
		EMstartOK = false;
		HurtOK = false;
		Idle_Time_Tmp = 0f;
		CanSO_Idle = false;
		CompCandidates.Clear();
		for (int i = 0; i < hitCP.Length; i++)
		{
			hitCP[i] = null;
		}
		Anger = 0;
		B_Col.enabled = true;
		F_Col.enabled = true;
		this.wait(1E-05f, SetStart);
	}

	private void OnDisable()
	{
		if (!s_isQuitting && (bool)XLpoint)
		{
			DespawnXLPointSafe();
		}
	}

	private void EnsureBrainMovePoint()
	{
		if (!BrainMovePoint)
		{
			GameObject gameObject = new GameObject(base.name + "_BrainMovePoint");
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			gameObject.transform.position = base.transform.position;
			BrainMovePoint = gameObject.transform;
		}
	}

	private void Update()
	{
		if (EMstartOK)
		{
			if ((bool)playerManager)
			{
				_distToPlayer = Vector2.Distance(base.transform.position, playerManager.transform.position);
			}
			else
			{
				_distToPlayer = float.MaxValue;
			}
			if (CompCandidates.Count > 0 && (bool)CompCandidates[0])
			{
				_distToTarget = Vector2.Distance(base.transform.position, CompCandidates[0].transform.position);
			}
			else
			{
				_distToTarget = float.MaxValue;
			}
			if ((bool)AstarPath.active)
			{
				GongShi();
				JSQ();
			}
		}
	}

	public void SetStart()
	{
		RefreshMapIcon();
		EnemyState runtimeState = RuntimeState;
		if (runtimeState != null && runtimeState.Hp > 0f)
		{
			HealthStat.Initialize(Mathf.Min(RuntimeState.Hp, Health_Max), Health_Max);
		}
		else
		{
			HealthStat.Initialize(Health_Max, Health_Max);
		}
		SK_Cur_Index = 0;
		if (IS_Boss)
		{
			BS.SK_Cur_IndexA = 0;
			BS.SK_Cur_IndexB = 0;
		}
		Idle_Time_Cur = UnityEngine.Random.Range(Idle_Time_Min, Idle_Time_Max);
		canvas.alpha = 1f;
		CanShu();
		StartPS = base.transform.position;
		if ((bool)FXsustain)
		{
			FXsustain.SetColor(MainElement);
		}
		AT_Idle_Cur = UnityEngine.Random.Range(AT_Idle_Min / AttackSpeed_Last, AT_Idle_Max / AttackSpeed_Last);
		EMstartOK = true;
	}

	public void JSQ()
	{
		JSQ_ChildKillSelf();
		if (UseBrainAI)
		{
			JSQ_BrainOnly();
		}
		else
		{
			JSQ_LegacyOnly();
		}
	}

	private void JSQ_ChildKillSelf()
	{
		if (!playerManager)
		{
			return;
		}
		JStimeC += Time.deltaTime;
		if (JStimeC >= 1f)
		{
			if ((bool)Father && !MathUtil.CheckObjDistance(base.transform.position, Father.transform.position, 14f))
			{
				HealthStat.SetCurrent(0f);
			}
			JStimeC = 0f;
		}
	}

	private void JSQ_BrainOnly()
	{
		if (!EMstartOK || !IsAlive)
		{
			return;
		}
		JStimeB += Time.deltaTime;
		if (JStimeB >= 1f)
		{
			ApplyHealthRegenOrTornWoundDamage();
			JStimeB = 0f;
		}
		if (CanSO_Idle)
		{
			Idle_Time_Tmp += Time.deltaTime;
			if (Idle_Time_Tmp >= Idle_Time_Cur)
			{
				if (IS_Boss)
				{
					int index = UnityEngine.Random.Range(0, BS.SO_Idle.Count);
					if (UnityEngine.Random.Range(0, 101) < SO_IdleRate)
					{
						RuntimeManager.PlayOneShot(BS.SO_Idle[index], yao.transform.position);
					}
				}
				else if (SO_Idle != null && UnityEngine.Random.Range(0, 101) < SO_IdleRate)
				{
					RuntimeManager.PlayOneShot(SO_Idle, yao.transform.position);
				}
				Idle_Time_Cur = UnityEngine.Random.Range(Idle_Time_Min, Idle_Time_Max);
				Idle_Time_Tmp = 0f;
			}
		}
		if (SK_ELSS != null && SK_ELSS.ATmod == 2)
		{
			timeF += Time.deltaTime;
			if (timeF >= SK_ELSS.HurtSK_JG)
			{
				HurtOK = true;
				timeF = 0f;
			}
		}
	}

	private void ApplyHealthRegenOrTornWoundDamage()
	{
		if (!HealthStat || Health_Prc <= 0f)
		{
			return;
		}
		float num = HealthStat.MaxValue * Health_Prc / 100f;
		if (!(num <= 0f))
		{
			if (peo != null && peo.DotEM != null && peo.DotEM.GerDotSL())
			{
				HealthStat.SetCurrent(HealthStat.CurrentValue - num);
			}
			else if (HealthStat.CurrentValue < HealthStat.MaxValue)
			{
				float current = Mathf.Min(HealthStat.CurrentValue + num, HealthStat.MaxValue);
				HealthStat.SetCurrent(current);
			}
		}
	}

	public int GetDotSilencedSkillRate(int skillRate)
	{
		if (skillRate <= 0 || IS_Boss)
		{
			return skillRate;
		}
		if (peo != null && peo.DotEM != null && peo.DotEM.GerDotCM())
		{
			return Mathf.FloorToInt((float)skillRate * 0.5f);
		}
		return skillRate;
	}

	public void JSQ_LegacyOnly()
	{
		if (!EMstartOK || !IsAlive)
		{
			return;
		}
		JStimeA += Time.deltaTime;
		if (JStimeA >= 0.23f)
		{
			bool num = playerManager;
			float num2 = float.MaxValue;
			if (num)
			{
				num2 = Vector2.Distance(base.transform.position, playerManager.transform.position);
			}
			if (CompCandidates.Count > 0 || num2 < Range_Cur)
			{
				for (int i = 0; i < CompCandidates.Count; i++)
				{
					Companion companion = CompCandidates[i];
					if (!companion || !companion.IsAlive || Vector2.Distance(base.transform.position, companion.transform.position) > Range_Cur + 0.5f)
					{
						CompCandidates.RemoveAt(i);
						i--;
					}
				}
				CompCandidates.Sort(delegate(Companion t1, Companion t2)
				{
					if (!t1 && !t2)
					{
						return 0;
					}
					if (!t1)
					{
						return 1;
					}
					if (!t2)
					{
						return -1;
					}
					float num4 = Vector2.Distance(t1.transform.position, base.transform.position);
					float value = Vector2.Distance(t2.transform.position, base.transform.position);
					return num4.CompareTo(value);
				});
				if ((bool)MVTarget)
				{
					Vector2 vector = MVTarget.transform.position - base.transform.position;
					float magnitude = vector.magnitude;
					ray = Physics2D.Raycast(base.transform.position, vector.normalized, magnitude, LayerMask.GetMask("block"));
					CanSeeMVTarget = !ray.collider;
				}
				else
				{
					CanSeeMVTarget = false;
				}
			}
			else
			{
				CanSeeMVTarget = false;
			}
			int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, Range_Cur, hitCP, LayerMask.GetMask("FootCOLcp"));
			if (num3 > 0)
			{
				for (int j = 0; j < num3; j++)
				{
					FootCOL component = hitCP[j].GetComponent<FootCOL>();
					if ((bool)component)
					{
						if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive && !CompCandidates.Contains(component.peo.cp))
						{
							CompCandidates.Add(component.peo.cp);
						}
						hitCP[j] = null;
					}
				}
			}
			if (Anger > 0)
			{
				Anger -= 5;
			}
			JStimeA = 0f;
		}
		JStimeB += Time.deltaTime;
		if (JStimeB >= 1f)
		{
			ApplyHealthRegenOrTornWoundDamage();
			JStimeB = 0f;
		}
		if (IsBattle && FarAway)
		{
			timeC += Time.deltaTime;
			if (timeC >= BattleTime)
			{
				IsBattle = false;
				timeC = 0f;
			}
		}
		if (CanSO_Idle)
		{
			Idle_Time_Tmp += Time.deltaTime;
			if (Idle_Time_Tmp >= Idle_Time_Cur)
			{
				if (IS_Boss)
				{
					int index = UnityEngine.Random.Range(0, BS.SO_Idle.Count);
					if (UnityEngine.Random.Range(0, 101) < SO_IdleRate)
					{
						RuntimeManager.PlayOneShot(BS.SO_Idle[index], yao.transform.position);
					}
				}
				else if (SO_Idle != null && UnityEngine.Random.Range(0, 101) < SO_IdleRate)
				{
					RuntimeManager.PlayOneShot(SO_Idle, yao.transform.position);
				}
				Idle_Time_Cur = UnityEngine.Random.Range(Idle_Time_Min, Idle_Time_Max);
				Idle_Time_Tmp = 0f;
			}
		}
		if (SK_ELSS != null && SK_ELSS.ATmod == 2)
		{
			timeF += Time.deltaTime;
			if (timeF >= SK_ELSS.HurtSK_JG)
			{
				HurtOK = true;
				timeF = 0f;
			}
		}
	}

	private void RefreshSpeedStats()
	{
		if (IsAlive)
		{
			if (AttackSpeed_Cut >= 60f)
			{
				AttackSpeed_Last = AttackSpeed_Max * 0.4f;
			}
			else
			{
				AttackSpeed_Last = AttackSpeed_Max - AttackSpeed_Max * (AttackSpeed_Cut / 100f) * (1f - AntiSlow / 100f);
			}
			if (MoveSpeed_Cut > 80f)
			{
				MoveSpeed_Last = MoveSpeed_Max * 0.2f * ChongSpeedMulti;
			}
			else
			{
				MoveSpeed_Last = (MoveSpeed_Max - MoveSpeed_Max * (MoveSpeed_Cut / 100f) * (1f - AntiSlow / 100f)) * ChongSpeedMulti;
			}
		}
		else
		{
			if (AttackSpeed_Cut >= 60f + playerManager.Top_Cut_ATS)
			{
				AttackSpeed_Last = AttackSpeed_Max * (1f - (60f + playerManager.Top_Cut_ATS) / 100f);
			}
			else
			{
				AttackSpeed_Last = AttackSpeed_Max - AttackSpeed_Max * (AttackSpeed_Cut / 100f) * (1f - AntiSlow / 100f);
			}
			if (MoveSpeed_Cut > 80f + playerManager.Top_Cut_MVS)
			{
				MoveSpeed_Last = MoveSpeed_Max * (1f - (80f + playerManager.Top_Cut_MVS) / 100f) * ChongSpeedMulti;
			}
			else
			{
				MoveSpeed_Last = (MoveSpeed_Max - MoveSpeed_Max * (MoveSpeed_Cut / 100f) * (1f - AntiSlow / 100f)) * ChongSpeedMulti;
			}
		}
		if ((bool)path)
		{
			path.maxSpeed = MoveSpeed_Path * MoveSpeed_Last;
		}
	}

	public void RefreshSpeedAndSetAni()
	{
		if (IsAlive && IS_Frozen)
		{
			AttackSpeed_Last = 0f;
			MoveSpeed_Last = 0f;
		}
		else
		{
			RefreshSpeedStats();
		}
		SetAni();
	}

	public void SetAni()
	{
		bool flag = IsAlive && IS_Frozen;
		if (MoveTrack != null)
		{
			MoveTrack.TimeScale = (flag ? 0f : MoveSpeed_Last);
		}
		if (AttackTrack != null)
		{
			AttackTrack.TimeScale = (flag ? 0f : AttackSpeed_Last);
		}
		if (SkillTrack != null)
		{
			SkillTrack.TimeScale = (flag ? 0f : SkillSpeed_Max);
		}
	}

	public void Fighting()
	{
		if (!hadTarget)
		{
			return;
		}
		bool flag = (bool)playerManager && playerManager.IsAlive;
		bool flag2 = flag && nearPlayer && (UnderPlayer || UseRange_ATplayer_multi_B);
		if (UseRange_ATplayer_multi_B && !flag2)
		{
			ClearBossTargetPriorityMultiB();
		}
		if (flag2)
		{
			MVTarget = playerManager.transform;
			ATTarget = playerManager.yao.transform;
			attackPL = true;
		}
		else if (CompCandidates.Count > 0)
		{
			MVTarget = CompCandidates[0].transform;
			ATTarget = CompCandidates[0].yao.transform;
			attackPL = false;
		}
		else if (UnderPlayer)
		{
			if (flag)
			{
				MVTarget = playerManager.transform;
				ATTarget = playerManager.yao.transform;
				attackPL = true;
			}
			else
			{
				MVTarget = null;
				ATTarget = null;
				attackPL = false;
			}
		}
		AIDS.target = MVTarget;
	}

	public void TakeDamage(float damage, float chuan, float BJrate, float BJDamage, float MSrate, float MSnumber, float yun, DamageType type, int indexType, PlayerManager pl, Companion cp, SkillOBJ_DT_SP skillSource = null)
	{
		Anger = 300;
		IsBattle = true;
		if (IsWuDi)
		{
			return;
		}
		bool flag = indexType == 1 && cp != null && cp.BJ_NoDot;
		bool ignoreDamageAnti = playerManager.WS_All || (indexType == 1 && cp != null && cp.WS_All);
		int num = ((indexType == 1 && cp != null) ? Mathf.Max(0, cp.AT_DotLayer) : 0);
		playerManager.EnsurePlayerDotData();
		bool flag2 = false;
		if (playerManager.EM_Heal_Crit > 0 && HealthStat.CurrentValue + 1f > HealthStat.MaxValue)
		{
			damage *= 2f + (BJDamage + (float)playerManager.EM_Heal_Crit) / 100f;
			flag2 = true;
			if (!flag && playerManager.DOT[playerManager.GiveInt(type)].Crit_One)
			{
				ACT_DOT dot = SingletonMonoScope<ACTbar>.Instance.GiveDot(type);
				peo.DotEM.AddDot(type, dot, 1 + num);
			}
		}
		if (BJrate >= 100f || UnityEngine.Random.value < BJrate * 0.01f)
		{
			if (!IsDpsTarget && Quality < 3 && HealthStat.CurrentValue < HealthStat.MaxValue * 0.3f && UnityEngine.Random.Range(0, 101) < playerManager.Crit_MS)
			{
				float num2 = HealthStat.CurrentValue + 1f;
				CaptureLastDamageCompanion(indexType, cp, num2);
				HealthStat.CurrentValue -= num2;
				SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num2, type, crit: false);
				if (SingletonMonoScope<DPSManager>.HasInstance)
				{
					SingletonMonoScope<DPSManager>.Instance.RecordDamage(this, num2, dotDamage: false);
				}
				if (indexType == 0 && num2 > 0.0001f)
				{
					Enemy.OnPlayerCritDamageEnemy?.Invoke();
				}
				Crit_BoomDie(type);
				return;
			}
			damage *= 2f + BJDamage / 100f;
			flag2 = true;
			if (!flag && playerManager.DOT[playerManager.GiveInt(type)].Crit_One)
			{
				ACT_DOT dot2 = SingletonMonoScope<ACTbar>.Instance.GiveDot(type);
				peo.DotEM.AddDot(type, dot2, 1 + num);
			}
		}
		if (UnityEngine.Random.value < GeDang * 0.01f)
		{
			return;
		}
		float elementAnti = GetElementAnti(type);
		float num3 = CalculateDamage(damage, elementAnti, chuan, ignoreDamageAnti);
		float num4 = 0f;
		num4 += H_HurtDMG;
		num4 += (float)(playerManager.DiffDebuff_DMG * peo.BuffEM.GetDebuffCount());
		if (playerManager.Dis_Out)
		{
			num4 += playerManager.Dis_OutLast(base.transform);
		}
		if (Vector2.Distance(base.transform.position, playerManager.transform.position) < 5f)
		{
			num4 += (float)playerManager.Dis_In;
		}
		if (Quality > 2)
		{
			num4 += (float)pl.JYBoss_DMG;
		}
		int num5 = 0;
		switch (type)
		{
		case DamageType.fire:
			num5 = 0;
			break;
		case DamageType.frozen:
			num5 = 1;
			break;
		case DamageType.thunder:
			num5 = 2;
			break;
		case DamageType.poison:
			num5 = 3;
			break;
		case DamageType.physics:
			num5 = 4;
			break;
		case DamageType.shadow:
			num5 = 5;
			break;
		}
		if (HurtEL != num5)
		{
			num4 += (float)pl.Diff_EL;
			HurtEL = num5;
		}
		num4 += (float)peo.DotEM.GerDotYS();
		if (indexType == 1)
		{
			num4 += (float)peo.DotEM.GerDotBE_CP();
		}
		num3 += num3 * num4 / 100f;
		TryApplyPrcCutDamage(ref num3, playerManager.GetPrcCut(type), GetPrcCutDamagePercent());
		TryApplyPrcCutDamage(ref num3, playerManager.GetPrcCut5P(type), 0.05f);
		TryApplyPrcCutDamage(ref num3, playerManager.GetPrcCut3P(type), 0.03f);
		num3 += pl.ManaStat.Max * (float)pl.DMG_ManaPRC / 100f;
		num3 *= playerManager.GetB_Dot_DMG;
		num3 = Mathf.Max(num3, 0f);
		if (playerManager.IsAlive)
		{
			playerManager.HealStat.Cur += num3 * playerManager.DMG_R_H / 100f;
			playerManager.ManaStat.Cur += num3 * playerManager.DMG_R_M / 100f;
		}
		if (flag2)
		{
			if (indexType == 0 && num3 > 0.0001f)
			{
				Enemy.OnPlayerCritDamageEnemy?.Invoke();
			}
			if ((bool)skillSource)
			{
				playerManager.BuffRuntime?.OnSkillCrit(skillSource);
			}
			Crit_BoomEXP(num3, type);
			if (HealthStat.CurrentValue < num3)
			{
				Crit_BoomDie(type);
			}
		}
		CaptureLastDamageCompanion(indexType, cp, num3);
		HealthStat.SetCurrent(HealthStat.CurrentValue - num3);
		SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num3, type, crit: false);
		if (SingletonMonoScope<DPSManager>.HasInstance)
		{
			SingletonMonoScope<DPSManager>.Instance.RecordDamage(this, num3, dotDamage: false);
		}
		if (num3 > 0.0001f)
		{
			this.OnDirectDamaged?.Invoke();
			UI_EnemyTip.TryShowByGamepadHit(this, pl, cp);
		}
		TryExecuteKill(num3, MSrate, MSnumber);
		TryStun(num3, yun);
		TryHurtSkill();
	}

	private float GetElementAnti(DamageType type)
	{
		switch (type)
		{
		case DamageType.fire:
			if (playerManager.WS_Anti0)
			{
				return 0f;
			}
			return FireAnti_Last;
		case DamageType.frozen:
			if (playerManager.WS_Anti1)
			{
				return 0f;
			}
			return FrozenAnti_Last;
		case DamageType.thunder:
			if (playerManager.WS_Anti2)
			{
				return 0f;
			}
			return ThunderAnti_Last;
		case DamageType.poison:
			if (playerManager.WS_Anti3)
			{
				return 0f;
			}
			return PoisonAnti_Last;
		case DamageType.physics:
			if (playerManager.WS_Anti4)
			{
				return 0f;
			}
			return PhysicsAnti_Last;
		case DamageType.shadow:
			if (playerManager.WS_Anti5)
			{
				return 0f;
			}
			return ShadowAnti_Last;
		default:
			return 0f;
		}
	}

	private float CalculateDamage(float damage, float anti, float chuan)
	{
		return CalculateDamage(damage, anti, chuan, playerManager.WS_All);
	}

	private float CalculateDamage(float damage, float anti, float chuan, bool ignoreDamageAnti)
	{
		float a = anti - chuan;
		a = Mathf.Max(a, -80f);
		float num = 100f / (100f + a);
		float num2 = damage * num;
		if (!ignoreDamageAnti)
		{
			num2 *= 1f - DamageAnti / 100f;
		}
		return Mathf.Max(num2, 0f);
	}

	private float GetPercentDamageRatio()
	{
		if (IsDpsTarget && Quality >= 2)
		{
			return 0.3f;
		}
		if (Quality > 4)
		{
			return 0.02f;
		}
		if (Quality > 3)
		{
			return 0.1f;
		}
		if (Quality > 2)
		{
			return 0.3f;
		}
		return 1f;
	}

	private float GetPrcCutDamagePercent()
	{
		if (IsDpsTarget && Quality >= 2)
		{
			return 0.08f * GetPercentDamageRatio();
		}
		if (Quality < 3)
		{
			return 0.08f;
		}
		if (Quality == 3)
		{
			return 0.05f;
		}
		if (Quality == 4)
		{
			return 0.03f;
		}
		return 0.01f;
	}

	private void TryApplyPrcCutDamage(ref float finalDamage, int rate, float percent)
	{
		if (rate > 0 && !(percent <= 0f) && UnityEngine.Random.Range(0, 101) < rate)
		{
			finalDamage += HealthStat.MaxValue * percent;
		}
	}

	private void CaptureLastDamageCompanion(int indexType, Companion cp, float finalDamage)
	{
		if (!(finalDamage <= 0.0001f))
		{
			lastDamageCompanion = ((indexType == 1 && cp != null) ? cp : null);
		}
	}

	private void ClearLastDamageCompanion()
	{
		lastDamageCompanion = null;
	}

	public void Crit_BoomEXP(float damage, DamageType type)
	{
		if (!(damage <= 0f) && UnityEngine.Random.Range(0, 101) < playerManager.Crit_BoomEXP)
		{
			Enemy enemy = FindNearestEnemyByDistance();
			if ((bool)enemy)
			{
				enemy.TakeDirectDamage(damage, type);
			}
		}
	}

	public void TakeDirectDamage(float damage, DamageType type)
	{
		if (!IsAlive || IsWuDi)
		{
			return;
		}
		float num = Mathf.Max(damage, 0f);
		if (!(num <= 0f))
		{
			ClearLastDamageCompanion();
			HealthStat.SetCurrent(HealthStat.CurrentValue - num);
			SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num, type, crit: false);
			if (SingletonMonoScope<DPSManager>.HasInstance)
			{
				SingletonMonoScope<DPSManager>.Instance.RecordDamage(this, num, dotDamage: false);
			}
			this.OnDirectDamaged?.Invoke();
		}
	}

	private Enemy FindNearestEnemyByDistance()
	{
		Enemy result = null;
		float num = float.MaxValue;
		int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, 1f, critBoomHits, LayerMask.GetMask("FootCOLem"));
		for (int i = 0; i < num2; i++)
		{
			Collider2D collider2D = critBoomHits[i];
			if (!collider2D)
			{
				continue;
			}
			FootCOL component = collider2D.GetComponent<FootCOL>();
			if (!component || !component.peo || component.peo.CharacterType != 2)
			{
				critBoomHits[i] = null;
				continue;
			}
			Enemy em = component.peo.em;
			if (!em || em == this || !em.IsAlive || em.IsJump || em.IsYS)
			{
				critBoomHits[i] = null;
				continue;
			}
			float sqrMagnitude = (em.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = em;
			}
			critBoomHits[i] = null;
		}
		return result;
	}

	private void TryDeadDotExplosion()
	{
		Enemy enemy = FindRandomEnemyByDeadDotRange();
		if ((bool)enemy && !enemy.IsDpsTarget && !(enemy.HealthStat == null))
		{
			enemy.HealthStat.SetCurrent(0f);
		}
	}

	private Enemy FindRandomEnemyByDeadDotRange()
	{
		Enemy result = null;
		int num = 0;
		int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, 2f, deadDotHits, LayerMask.GetMask("FootCOLem"));
		for (int i = 0; i < num2; i++)
		{
			Collider2D collider2D = deadDotHits[i];
			if (!collider2D)
			{
				continue;
			}
			FootCOL component = collider2D.GetComponent<FootCOL>();
			if (!component || !component.peo || component.peo.CharacterType != 2)
			{
				deadDotHits[i] = null;
				continue;
			}
			Enemy em = component.peo.em;
			if (!em || em == this || em.IsDpsTarget || !em.IsAlive || em.IsJump || em.IsYS)
			{
				deadDotHits[i] = null;
				continue;
			}
			num++;
			if (UnityEngine.Random.Range(0, num) == 0)
			{
				result = em;
			}
			deadDotHits[i] = null;
		}
		return result;
	}

	public void Crit_BoomDie(DamageType type)
	{
		if (UnityEngine.Random.Range(0, 101) < playerManager.Crit_BoomDie_Rate)
		{
			SkillOBJ_DT_SP component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.Skill[17].OBJ[playerManager.GiveInt(type)], base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			component.indexType = 0;
			component.pl = SingletonMonoScope<PlayerManager>.Instance;
			component.ZY = true;
			component.Dot_Infect = false;
			component.Dot_Infect_Layer = 0;
			component.TargetPos = Vector3.zero;
			component.skillName = null;
			component.LockType = 0;
			component.RTtypeOBJ = 0;
			component.Distance = 0f;
			component.GlobalID = 0;
			component.SpecialType = 0;
			component.damageType = type;
			component.MainEL = playerManager.GiveInt(type);
			component.ThroughType = 0;
			component.AttackType = true;
			component.AttackTypeA = true;
			component.AttackTypeB = true;
			component.Damage = HealthStat.MaxValue * 0.5f;
			component.DamageA = 0f;
			component.DamageB = 0f;
			component.SPC_Damage = 0f;
			component.SPC_DamageA = 0f;
			component.SPC_DamageB = 0f;
			component.BJrate = 0f;
			component.BJDamage = 0f;
			component.JYrate = playerManager.JYrate_Last;
			component.Through = playerManager.ThroughRate;
			component.FlySpeed = playerManager.FlySpeed;
			component.MoveSpeedCut = 0f;
			component.AttackSpeedCut = 0f;
			component.AntiCut = 0f;
			component.BF_Damage = 0f;
			component.BF_EL_Damage = 0f;
			component.BF_EL_Chuan = 0f;
			component.BF_BJrate = 0f;
			component.BF_JYrate = 0f;
			component.BF_GeDang = 0f;
			component.BF_AttackSpeed = 0f;
			component.BF_MoveSpeed = 0f;
			component.BF_DamageAnti = 0f;
			component.BF_Health_Prc = 0f;
			component.C_Damage = 0f;
			component.C_ATspeed = 0f;
			component.C_MVspeed = 0f;
			component.C_Health_Prc = 0f;
			component.CF_Rate = 0f;
			component.BSAT = null;
			component.Is_BS = 1;
			component.ChangeSkin = 1;
			component.SkinIndex = 0;
			component.Reborn = 0;
			component.NoTime = 0;
			component.BuffTime = 1f;
			component.DebuffTime = 1f;
			component.Field_time = 0f;
			component.ORB_time = 0f;
			component.EXP_time = 0f;
			component.ZD_time_F = 0f;
			component.ZD_time_S = 0f;
			component.Layer_SubA = 0;
			component.Layer_SubB = 0;
			component.ORB = 0;
			component.ZD_F = 140;
			component.ZD_S = 0;
			component.ZD_AB = 0;
			component.EXP_F = 0;
			component.EXP_S = 0;
			component.EXP_AB = 0;
			component.Dic_F = 0;
			component.Dic_S = 0;
			component.FX_F = 0;
			component.FX_S = 0;
			component.Sound = 0;
			component.Count_ORB = 0;
			component.Count_ATtarget = 0;
			component.ATtar_DMG = 0;
			component.CF_Count = 0;
			component.Count_F = 1;
			component.Count_S = 0;
			component.Count_AB = 0;
			component.CountMulti = 0;
			component.CountEXP = 0;
			component.TypeORB = 0;
			component.CF_Type = 0;
			component.Type_F = 1;
			component.Type_S = 0;
			component.Type_AB = 0;
			component.TypeDIC_F = 0;
			component.TypeDIC_S = 0;
			component.TypeEXP_F = 0;
			component.TypeEXP_S = 0;
			component.TypeEXP_AB = 0;
			component.Size = 0f;
			component.High = 0f;
			component.JG = 0f;
			component.AngleA = 0f;
			component.AngleB = 0f;
			component.Range1 = 0.1f;
			component.Range2 = 0f;
			component.Range_AT = 0f;
			component.FStime1 = 0.1f;
			component.FStime2 = 0f;
			component.Speed1 = 0f;
			component.Speed2 = 0f;
			component.Speed3 = 0f;
			component.Speed4 = 0f;
			component.Follow_F = 0;
			component.Follow_S = 0;
			component.AllChuan_F = 0;
			component.AllChuan_S = 0;
			component.Slow_F = 0;
			component.Slow_S = 0;
			component.RDSpeed_F = 0;
			component.RDSpeed_S = 0;
			component.HasFX = 0;
			component.S_HasFX = 0;
			component.AB_HasFX = 0;
			component.colEXP = 0;
			component.colEXP_A = 0;
			component.S_colEXP = 0;
			component.AB_colEXP = 0;
			component.TimeEXP = 0;
			component.TimeEXP_AB = 0;
			component.LastEXP = 0;
			component.LastEXP_AB = 0;
			component.S_LastEXP = 0;
			component.AB_LastEXP = 0;
			component.EXPpos = 0;
			component.EXPpos_AB = 0;
			component.S_EXPpos = 0;
			component.AB_EXPpos = 0;
			component.AngleEXP = 0;
			component.AngleEXP_AB = 0;
		}
	}

	private void TryExecuteKill(float damage, float rate, float MSnumber)
	{
		if (IsDpsTarget || damage <= 0f || rate <= 0f || UnityEngine.Random.value > rate * 0.01f)
		{
			return;
		}
		float num = HealthStat.CurrentValue / HealthStat.MaxValue;
		if (playerManager.Dot_MSAll)
		{
			if (num < MSnumber / 100f)
			{
				HealthStat.SetCurrent(0f);
			}
			if (playerManager.IsAlive)
			{
				playerManager.HealStat.Cur += playerManager.HealStat.Max * 0.01f;
			}
			return;
		}
		switch (Quality)
		{
		case 0:
			if (num < MSnumber / 100f)
			{
				HealthStat.SetCurrent(0f);
			}
			break;
		case 1:
			if (num < MSnumber / 150f)
			{
				HealthStat.SetCurrent(0f);
			}
			break;
		case 2:
			if (num < MSnumber / 200f)
			{
				HealthStat.SetCurrent(0f);
			}
			break;
		case 3:
			if (num < MSnumber / 300f)
			{
				HealthStat.SetCurrent(0f);
			}
			break;
		}
	}

	private void TryStun(float damage, float yun)
	{
		if (IS_Frozen || IsBaTi || IsYun)
		{
			return;
		}
		float maxValue = HealthStat.MaxValue;
		bool flag = false;
		if (IS_Boss)
		{
			if (damage > maxValue / 20f)
			{
				flag = true;
			}
			else if (damage > maxValue / 50f)
			{
				float num = Mathf.Clamp01((yun * 2f - yunAnti_Last) * 0.01f);
				if (UnityEngine.Random.value < num)
				{
					flag = true;
				}
			}
		}
		else if (damage > maxValue / 2f)
		{
			flag = true;
		}
		else if (damage > maxValue / 3f)
		{
			float num2 = Mathf.Clamp01((yun * 2f - yunAnti_Last) * 0.01f);
			if (UnityEngine.Random.value < num2)
			{
				flag = true;
			}
		}
		else if (damage > maxValue / 5f)
		{
			if (UnityEngine.Random.value < (yun * 1.5f - yunAnti_Last) * 0.01f)
			{
				flag = true;
			}
		}
		else if (damage > maxValue / 10f && UnityEngine.Random.value < (yun - yunAnti_Last) * 0.01f)
		{
			flag = true;
		}
		if (flag)
		{
			IsYun = true;
			if (peo != null && peo.DotEM != null)
			{
				peo.DotEM.TryDotJYOnStun();
			}
		}
	}

	private void TryHurtSkill()
	{
		if (SK_ELSS != null && SK_ELSS.ATmod == 2 && HurtOK && UnityEngine.Random.value < (float)SK_ELSS.HurtSK_Rate * 0.01f)
		{
			SetHurtSK();
			HurtOK = false;
		}
	}

	public void TakeDotDamage(DamageType type, float damage, float chuan)
	{
		if (!IsWuDi)
		{
			if (Anger < 300)
			{
				Anger++;
			}
			IsBattle = true;
			ClearLastDamageCompanion();
			playerManager.EnsurePlayerDotData();
			float elementAnti = GetElementAnti(type);
			float num = CalculateDamage(damage, elementAnti, chuan);
			float num2 = 0f;
			if (Quality > 2)
			{
				num2 += (float)playerManager.JYBoss_DMG;
			}
			if (Quality == 3)
			{
				num2 += playerManager.AllDot_JY;
			}
			if (IsMove)
			{
				num2 += playerManager.AllDot_MV;
			}
			num2 += playerManager.DiffDotDMG * (float)peo.DotEM.GetDotCount();
			num2 += (float)(playerManager.DiffDebuff_DMG * peo.BuffEM.GetDebuffCount());
			if (playerManager.Dis_Out)
			{
				num2 += playerManager.Dis_OutLast(base.transform);
			}
			if (Vector2.Distance(base.transform.position, playerManager.transform.position) < 5f)
			{
				num2 += (float)playerManager.Dis_In;
			}
			if (HealthStat.CurrentValue < HealthStat.MaxValue * 0.5f)
			{
				num2 += (float)playerManager.DOT[playerManager.GiveInt(type)].DMG50;
			}
			num2 += (float)playerManager.DOT[playerManager.GiveInt(type)].LowH_50Last;
			num2 += (float)playerManager.DOT[playerManager.GiveInt(type)].HighH_100Last;
			num2 += (float)playerManager.DOT[playerManager.GiveInt(type)].LowM_40Last;
			num += num * num2 / 100f;
			if (playerManager.DOT[playerManager.GiveInt(type)].Dot_Crit && (float)UnityEngine.Random.Range(0, 101) < playerManager.BJrate_Last)
			{
				num *= 2f + playerManager.BJDamage_Last / 100f;
			}
			num += num * H_HurtDMG / 100f;
			if (playerManager.IsAlive)
			{
				playerManager.HealStat.Cur += num * playerManager.DMG_R_H / 100f;
				playerManager.ManaStat.Cur += num * playerManager.DMG_R_M / 100f;
			}
			HealthStat.SetCurrent(HealthStat.CurrentValue - num);
			SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num, type, crit: false);
			if (SingletonMonoScope<DPSManager>.HasInstance)
			{
				SingletonMonoScope<DPSManager>.Instance.RecordDamage(this, num, dotDamage: true);
			}
		}
	}

	public void TakeCutJumpDamage(DamageType type, float percent)
	{
		if (!IsWuDi)
		{
			IsBattle = true;
			ClearLastDamageCompanion();
			float percentDamageRatio = GetPercentDamageRatio();
			float num = HealthStat.MaxValue * percent * 0.01f * percentDamageRatio;
			num *= 1f - DamageAnti / 100f;
			float num2 = 0f;
			if (Quality > 2)
			{
				num2 += (float)playerManager.JYBoss_DMG;
			}
			num2 += (float)(playerManager.DiffDebuff_DMG * peo.BuffEM.GetDebuffCount());
			if (playerManager.Dis_Out)
			{
				num2 += playerManager.Dis_OutLast(base.transform);
			}
			if (Vector2.Distance(base.transform.position, playerManager.transform.position) < 5f)
			{
				num2 += (float)playerManager.Dis_In;
			}
			num += num * num2 / 100f;
			if (playerManager.IsAlive)
			{
				playerManager.HealStat.Cur += num * playerManager.DMG_R_H / 100f;
				playerManager.ManaStat.Cur += num * playerManager.DMG_R_M / 100f;
			}
			HealthStat.SetCurrent(HealthStat.CurrentValue - num);
			SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num, type, crit: false);
			if (SingletonMonoScope<DPSManager>.HasInstance)
			{
				SingletonMonoScope<DPSManager>.Instance.RecordDamage(this, num, dotDamage: false);
			}
		}
	}

	public void TakeDotDebuff(bool add, float atk, float move, float dmg)
	{
		float num = (add ? 1f : (-1f));
		if (IS_Boss)
		{
			atk /= 3f;
			move /= 3f;
		}
		AttackSpeed_Cut += atk * num;
		MoveSpeed_Cut += move * num;
		Damage_Cut += dmg * num;
		RefreshSpeedAndSetAni();
	}

	public void TakeDotDebuffLayer(bool add, float anti, float yun, int layer, DamageType type)
	{
		float num = (add ? anti : 0f);
		int num2 = (add ? layer : 0);
		switch (type)
		{
		default:
			return;
		case DamageType.fire:
			FireAntiCut_Dot = num;
			FireAntiCut_Layer = num2;
			break;
		case DamageType.frozen:
			FrozenAntiCut_Dot = num;
			FrozenAntiCut_Layer = num2;
			break;
		case DamageType.thunder:
			ThunderAntiCut_Dot = num;
			ThunderAntiCut_Layer = num2;
			break;
		case DamageType.poison:
			PoisonAntiCut_Dot = num;
			PoisonAntiCut_Layer = num2;
			break;
		case DamageType.physics:
			PhysicsAntiCut_Dot = num;
			PhysicsAntiCut_Layer = num2;
			break;
		case DamageType.shadow:
			ShadowAntiCut_Dot = num;
			ShadowAntiCut_Layer = num2;
			break;
		}
		yunAnti_Cut = num;
		yunAntiCut_Layer = num2;
	}

	public void SetSkin(int a)
	{
	}

	public void SetSpiritColor(int cl)
	{
		GameObject[] spirit = Spirit;
		for (int i = 0; i < spirit.Length; i++)
		{
			spirit[i].gameObject.SetActive(value: false);
		}
		Spirit[cl].gameObject.SetActive(value: true);
	}

	private void OnApplicationQuit()
	{
		s_isQuitting = true;
	}

	public void OnDespawn()
	{
		if (s_isQuitting)
		{
			return;
		}
		B_Col.enabled = false;
		F_Col.enabled = false;
		DespawnXLPointSafe();
		ATTarget = null;
		if ((bool)UseFX_OBJ)
		{
			GameObject useFX_OBJ = UseFX_OBJ;
			UseFX_OBJ = null;
			LeanPool.Despawn(useFX_OBJ);
		}
		EnemyClear();
		if ((bool)FXsustain)
		{
			FXsustain.StopFX();
		}
		if (SkillBuffList.Count > 0)
		{
			foreach (SK_BuffA skillBuff in SkillBuffList)
			{
				skillBuff.StopBuff();
			}
		}
		SkillBuffList.Clear();
		BuffMG.DelAll();
		SK_AT = null;
		SK_A = null;
		SK_Comp = null;
		SK_FS = null;
		SK_ELSS = null;
		SK_Die = null;
		ChildAllDie();
		ClearChildren(yao.transform);
		ClearChildren(head.transform);
		ClearChildren(body.transform);
		LeanPool.Despawn(base.gameObject);
	}

	public static void ClearChildren(Transform parent)
	{
		if (s_isQuitting)
		{
			return;
		}
		for (int num = parent.childCount - 1; num >= 0; num--)
		{
			Transform child = parent.GetChild(num);
			if ((bool)child)
			{
				child.SetParent(null, worldPositionStays: false);
				LeanPool.Despawn(child.gameObject);
			}
		}
	}

	private void DespawnXLPointSafe()
	{
		if ((bool)XLpoint)
		{
			LeanPool.Despawn(XLpoint);
			XLpoint = null;
		}
	}

	private void OnDestroy()
	{
		if ((bool)BrainMovePoint)
		{
			UnityEngine.Object.Destroy(BrainMovePoint.gameObject);
			BrainMovePoint = null;
		}
	}

	public void OnDie()
	{
		if (s_isQuitting)
		{
			return;
		}
		bool confusedDeathBoom = !IsDpsTarget && Can_DieBoom && peo != null && peo.DotEM != null && peo.DotEM.GerDotMH();
		bool num = !IsDpsTarget && Quality == 3 && peo != null && peo.DotEM != null && peo.DotEM.GerDotDead();
		bool cursedDeathAttack = peo != null && peo.DotEM != null && peo.DotEM.GerDotZZ();
		if (!IsTower && RuntimeState != null)
		{
			RuntimeState.IsDead = true;
			RuntimeState.Hp = 0f;
		}
		if ((bool)mapIconRenderer)
		{
			mapIconRenderer.enabled = false;
		}
		OnEnemyDie?.Invoke(this);
		B_Col.enabled = false;
		F_Col.enabled = false;
		DespawnXLPointSafe();
		ATTarget = null;
		if ((bool)Father)
		{
			if (IS_FS)
			{
				Father.fsList.Remove(this);
			}
			else
			{
				Father.cpList.Remove(this);
			}
			Father = null;
		}
		if ((bool)UseFX_OBJ)
		{
			GameObject useFX_OBJ = UseFX_OBJ;
			UseFX_OBJ = null;
			LeanPool.Despawn(useFX_OBJ);
		}
		EnemyClear();
		if ((bool)FXsustain)
		{
			FXsustain.StopFX();
		}
		if (!IsDpsTarget && Can_DieBoom)
		{
			SetDieEXP(confusedDeathBoom, cursedDeathAttack);
		}
		if (num)
		{
			TryDeadDotExplosion();
		}
		if (SkillBuffList.Count > 0)
		{
			foreach (SK_BuffA skillBuff in SkillBuffList)
			{
				skillBuff.StopBuff();
			}
		}
		SkillBuffList.Clear();
		BuffMG.DelAll();
		if ((bool)emitter)
		{
			emitter.Stop();
		}
		if (IS_Boss)
		{
			GetComponent<Boss>().BossDie();
			BS.AT = null;
			BS.SK = null;
			BS.SKC = null;
			UI_BossTip.Instance.boss.Remove(this);
		}
		else
		{
			SK_AT = null;
			SK_A = null;
			SK_Comp = null;
			SK_FS = null;
			SK_ELSS = null;
		}
		SK_Die = null;
		ChildClear();
		if (!IsDpsTarget && !IS_FS)
		{
			itemManager.EM_Drop(this);
		}
		if (!IsDpsTarget)
		{
			playerManager.GainXp(Xp);
			if (playerManager.IsAlive)
			{
				playerManager.KillRecver();
				lastDamageCompanion?.TryTriggerKillHeal();
				lastDamageCompanion = null;
				playerManager.BuffRuntime?.OnEnemyKilled(this);
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_Die(this);
		}
	}

	public void SetDieEXP(bool confusedDeathBoom, bool cursedDeathAttack)
	{
		SkillOBJ_DT_SP skillOBJ_DT_SP = SK_Die.FStype switch
		{
			4 => LeanPool.Spawn(PB.Skill[SK_Die.OBJ].OBJ[MainElement], base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			5 => LeanPool.Spawn(PB.Skill[SK_Die.OBJ].OBJ[MainElement], yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			_ => LeanPool.Spawn(PB.Skill[SK_Die.OBJ].OBJ[MainElement], base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
		};
		skillOBJ_DT_SP.indexType = 2;
		skillOBJ_DT_SP.em = this;
		skillOBJ_DT_SP.DotZZ = cursedDeathAttack;
		skillOBJ_DT_SP.ZY = confusedDeathBoom;
		skillOBJ_DT_SP.Dot_Infect = false;
		skillOBJ_DT_SP.Dot_Infect_Layer = 0;
		if (confusedDeathBoom)
		{
			skillOBJ_DT_SP.pl = playerManager;
		}
		skillOBJ_DT_SP.TargetPos = Vector3.zero;
		skillOBJ_DT_SP.skillName = "Die";
		skillOBJ_DT_SP.dic = Vector3.zero;
		skillOBJ_DT_SP.Distance = 0f;
		skillOBJ_DT_SP.GlobalID = 100000;
		skillOBJ_DT_SP.damageType = MainELType;
		skillOBJ_DT_SP.MainEL = MainElement;
		skillOBJ_DT_SP.ThroughType = SK_Die.ThroughType;
		skillOBJ_DT_SP.AttackType = SK_Die.AttackType;
		skillOBJ_DT_SP.AttackTypeA = SK_Die.AttackTypeA;
		skillOBJ_DT_SP.AttackTypeB = SK_Die.AttackTypeB;
		skillOBJ_DT_SP.Damage = SK_Die.Damage / 100f * Damage_Last;
		skillOBJ_DT_SP.DamageA = 0f;
		skillOBJ_DT_SP.DamageB = 0f;
		skillOBJ_DT_SP.BJrate = BJRate;
		skillOBJ_DT_SP.Through = Through;
		skillOBJ_DT_SP.FlySpeed = FlySpeed;
		skillOBJ_DT_SP.Chuan = Chuan;
		skillOBJ_DT_SP.MoveSpeedCut = SK_Die.SpeedCut;
		skillOBJ_DT_SP.AttackSpeedCut = SK_Die.SpeedCut;
		skillOBJ_DT_SP.BF_EL_Chuan = 0f;
		skillOBJ_DT_SP.BF_BJrate = 0f;
		skillOBJ_DT_SP.BF_GeDang = 0f;
		skillOBJ_DT_SP.BF_DamageAnti = 0f;
		skillOBJ_DT_SP.C_Damage = 0f;
		skillOBJ_DT_SP.C_ATspeed = 0f;
		skillOBJ_DT_SP.C_MVspeed = 0f;
		skillOBJ_DT_SP.C_Health_Prc = 0f;
		skillOBJ_DT_SP.BF_Through = 0f;
		skillOBJ_DT_SP.CF_Rate = 0f;
		skillOBJ_DT_SP.ChangeSkin = 1;
		skillOBJ_DT_SP.SkinIndex = 0;
		skillOBJ_DT_SP.Reborn = 0;
		skillOBJ_DT_SP.DotRate = SK_Die.DotRate;
		skillOBJ_DT_SP.DotDamage = SK_Die.DotDamage / 100f * Damage_Last;
		skillOBJ_DT_SP.NoTime = 1;
		skillOBJ_DT_SP.BuffTime = SK_Die.BuffTime;
		skillOBJ_DT_SP.DebuffTime = SK_Die.DebuffTime;
		skillOBJ_DT_SP.EXP_time = SK_Die.EXP_time;
		skillOBJ_DT_SP.ZD_time_F = 0f;
		skillOBJ_DT_SP.ZD_time_S = 0f;
		skillOBJ_DT_SP.ZD_F = SK_Die.ZD_F;
		skillOBJ_DT_SP.ZD_S = SK_Die.ZD_S;
		skillOBJ_DT_SP.EXP_F = SK_Die.EXP_F;
		skillOBJ_DT_SP.EXP_S = SK_Die.EXP_S;
		skillOBJ_DT_SP.Dic_F = SK_Die.Dic_F;
		skillOBJ_DT_SP.Dic_S = SK_Die.Dic_S;
		skillOBJ_DT_SP.FX_F = 0;
		skillOBJ_DT_SP.FX_S = 0;
		skillOBJ_DT_SP.Sound = SK_Die.Sound;
		skillOBJ_DT_SP.Count_F = SK_Die.Count_F;
		skillOBJ_DT_SP.Count_S = SK_Die.Count_S;
		skillOBJ_DT_SP.CountMulti = SK_Die.CountMulti;
		skillOBJ_DT_SP.CountEXP = SK_Die.CountEXP;
		skillOBJ_DT_SP.CF_Type = 0;
		skillOBJ_DT_SP.Type_F = SK_Die.Type_F;
		skillOBJ_DT_SP.Type_S = SK_Die.Type_S;
		skillOBJ_DT_SP.TypeDIC_F = SK_Die.TypeDIC_F;
		skillOBJ_DT_SP.TypeDIC_S = SK_Die.TypeDIC_S;
		skillOBJ_DT_SP.TypeEXP_F = SK_Die.TypeEXP_F;
		skillOBJ_DT_SP.TypeEXP_S = SK_Die.TypeEXP_S;
		skillOBJ_DT_SP.JG = SK_Die.JG;
		skillOBJ_DT_SP.AngleA = SK_Die.AngleA;
		skillOBJ_DT_SP.AngleB = SK_Die.AngleB;
		skillOBJ_DT_SP.Range1 = SK_Die.Range1;
		skillOBJ_DT_SP.Range2 = SK_Die.Range2;
		skillOBJ_DT_SP.Range_AT = SK_Die.Range_AT;
		skillOBJ_DT_SP.FStime1 = SK_Die.FStime1;
		skillOBJ_DT_SP.FStime2 = SK_Die.FStime2;
		skillOBJ_DT_SP.Speed1 = SK_Die.Speed1;
		skillOBJ_DT_SP.Speed2 = SK_Die.Speed2;
		skillOBJ_DT_SP.Speed3 = SK_Die.Speed3;
		skillOBJ_DT_SP.Speed4 = SK_Die.Speed4;
		skillOBJ_DT_SP.Follow_F = SK_Die.Follow_F;
		skillOBJ_DT_SP.Follow_S = SK_Die.Follow_F;
		skillOBJ_DT_SP.AllChuan_F = SK_Die.AllChuan_F;
		skillOBJ_DT_SP.AllChuan_S = SK_Die.AllChuan_F;
		skillOBJ_DT_SP.Slow_F = 1;
		skillOBJ_DT_SP.Slow_S = 1;
		skillOBJ_DT_SP.RDSpeed_F = SK_Die.RDSpeed_F;
		skillOBJ_DT_SP.RDSpeed_S = SK_Die.RDSpeed_F;
		skillOBJ_DT_SP.HasFX = 0;
		skillOBJ_DT_SP.S_HasFX = 1;
		skillOBJ_DT_SP.AB_HasFX = 1;
		skillOBJ_DT_SP.colEXP = 1;
		skillOBJ_DT_SP.colEXP_A = 1;
		skillOBJ_DT_SP.S_colEXP = 1;
		skillOBJ_DT_SP.AB_colEXP = 1;
		skillOBJ_DT_SP.TimeEXP = 1;
		skillOBJ_DT_SP.TimeEXP_AB = 1;
		skillOBJ_DT_SP.LastEXP = 1;
		skillOBJ_DT_SP.LastEXP_AB = 1;
		skillOBJ_DT_SP.S_LastEXP = 1;
		skillOBJ_DT_SP.AB_LastEXP = 1;
	}

	public void SetHurtSK()
	{
		object obj = SK_ELSS.FStype switch
		{
			4 => LeanPool.Spawn(PB.Skill[SK_ELSS.OBJ].OBJ[MainElement], base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			5 => LeanPool.Spawn(PB.Skill[SK_ELSS.OBJ].OBJ[MainElement], yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			_ => LeanPool.Spawn(PB.Skill[SK_ELSS.OBJ].OBJ[MainElement], base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
		};
		((SkillOBJ_DT_SP)obj).indexType = 2;
		((SkillOBJ_DT_SP)obj).em = this;
		((SkillOBJ_DT_SP)obj).ZY = false;
		((SkillOBJ_DT_SP)obj).Dot_Infect = false;
		((SkillOBJ_DT_SP)obj).Dot_Infect_Layer = 0;
		((SkillOBJ_DT_SP)obj).TargetPos = Vector3.zero;
		((SkillOBJ_DT_SP)obj).skillName = SK_ELSS.IndexName;
		((SkillOBJ_DT_SP)obj).dic = Vector3.zero;
		((SkillOBJ_DT_SP)obj).Distance = 0f;
		((SkillOBJ_DT_SP)obj).GlobalID = 100000;
		((SkillOBJ_DT_SP)obj).damageType = MainELType;
		((SkillOBJ_DT_SP)obj).MainEL = MainElement;
		((SkillOBJ_DT_SP)obj).ThroughType = SK_Die.ThroughType;
		((SkillOBJ_DT_SP)obj).AttackType = SK_Die.AttackType;
		((SkillOBJ_DT_SP)obj).AttackTypeA = SK_Die.AttackTypeA;
		((SkillOBJ_DT_SP)obj).AttackTypeB = SK_Die.AttackTypeB;
		((SkillOBJ_DT_SP)obj).Damage = SK_Die.Damage / 100f * Damage_Last;
		((SkillOBJ_DT_SP)obj).DamageA = 0f;
		((SkillOBJ_DT_SP)obj).DamageB = 0f;
		((SkillOBJ_DT_SP)obj).BJrate = BJRate;
		((SkillOBJ_DT_SP)obj).Through = Through;
		((SkillOBJ_DT_SP)obj).FlySpeed = FlySpeed;
		((SkillOBJ_DT_SP)obj).Chuan = Chuan;
		((SkillOBJ_DT_SP)obj).MoveSpeedCut = SK_Die.SpeedCut;
		((SkillOBJ_DT_SP)obj).AttackSpeedCut = SK_Die.SpeedCut;
		((SkillOBJ_DT_SP)obj).BF_EL_Chuan = 0f;
		((SkillOBJ_DT_SP)obj).BF_BJrate = 0f;
		((SkillOBJ_DT_SP)obj).BF_GeDang = 0f;
		((SkillOBJ_DT_SP)obj).BF_DamageAnti = 0f;
		((SkillOBJ_DT_SP)obj).C_Damage = 0f;
		((SkillOBJ_DT_SP)obj).C_ATspeed = 0f;
		((SkillOBJ_DT_SP)obj).C_MVspeed = 0f;
		((SkillOBJ_DT_SP)obj).C_Health_Prc = 0f;
		((SkillOBJ_DT_SP)obj).BF_Through = 0f;
		((SkillOBJ_DT_SP)obj).CF_Rate = 0f;
		((SkillOBJ_DT_SP)obj).ChangeSkin = 1;
		((SkillOBJ_DT_SP)obj).SkinIndex = 0;
		((SkillOBJ_DT_SP)obj).Reborn = 0;
		((SkillOBJ_DT_SP)obj).DotRate = SK_Die.DotRate;
		((SkillOBJ_DT_SP)obj).DotDamage = SK_Die.DotDamage / 100f * Damage_Last;
		((SkillOBJ_DT_SP)obj).NoTime = 1;
		((SkillOBJ_DT_SP)obj).BuffTime = SK_Die.BuffTime;
		((SkillOBJ_DT_SP)obj).DebuffTime = SK_Die.DebuffTime;
		((SkillOBJ_DT_SP)obj).EXP_time = SK_Die.EXP_time;
		((SkillOBJ_DT_SP)obj).ZD_time_F = 0f;
		((SkillOBJ_DT_SP)obj).ZD_time_S = 0f;
		((SkillOBJ_DT_SP)obj).ZD_F = SK_Die.ZD_F;
		((SkillOBJ_DT_SP)obj).ZD_S = SK_Die.ZD_S;
		((SkillOBJ_DT_SP)obj).EXP_F = SK_Die.EXP_F;
		((SkillOBJ_DT_SP)obj).EXP_S = SK_Die.EXP_S;
		((SkillOBJ_DT_SP)obj).Dic_F = SK_Die.Dic_F;
		((SkillOBJ_DT_SP)obj).Dic_S = SK_Die.Dic_S;
		((SkillOBJ_DT_SP)obj).FX_F = 0;
		((SkillOBJ_DT_SP)obj).FX_S = 0;
		((SkillOBJ_DT_SP)obj).Sound = SK_Die.Sound;
		((SkillOBJ_DT_SP)obj).Count_F = SK_Die.Count_F;
		((SkillOBJ_DT_SP)obj).Count_S = SK_Die.Count_S;
		((SkillOBJ_DT_SP)obj).CountMulti = SK_Die.CountMulti;
		((SkillOBJ_DT_SP)obj).CountEXP = SK_Die.CountEXP;
		((SkillOBJ_DT_SP)obj).CF_Type = 0;
		((SkillOBJ_DT_SP)obj).Type_F = SK_Die.Type_F;
		((SkillOBJ_DT_SP)obj).Type_S = SK_Die.Type_S;
		((SkillOBJ_DT_SP)obj).TypeDIC_F = SK_Die.TypeDIC_F;
		((SkillOBJ_DT_SP)obj).TypeDIC_S = SK_Die.TypeDIC_S;
		((SkillOBJ_DT_SP)obj).TypeEXP_F = SK_Die.TypeEXP_F;
		((SkillOBJ_DT_SP)obj).TypeEXP_S = SK_Die.TypeEXP_S;
		((SkillOBJ_DT_SP)obj).JG = SK_Die.JG;
		((SkillOBJ_DT_SP)obj).AngleA = SK_Die.AngleA;
		((SkillOBJ_DT_SP)obj).AngleB = SK_Die.AngleB;
		((SkillOBJ_DT_SP)obj).Range1 = SK_Die.Range1;
		((SkillOBJ_DT_SP)obj).Range2 = SK_Die.Range2;
		((SkillOBJ_DT_SP)obj).Range_AT = SK_Die.Range_AT;
		((SkillOBJ_DT_SP)obj).FStime1 = SK_Die.FStime1;
		((SkillOBJ_DT_SP)obj).FStime2 = SK_Die.FStime2;
		((SkillOBJ_DT_SP)obj).Speed1 = SK_Die.Speed1;
		((SkillOBJ_DT_SP)obj).Speed2 = SK_Die.Speed2;
		((SkillOBJ_DT_SP)obj).Speed3 = SK_Die.Speed3;
		((SkillOBJ_DT_SP)obj).Speed4 = SK_Die.Speed4;
		((SkillOBJ_DT_SP)obj).Follow_F = SK_Die.Follow_F;
		((SkillOBJ_DT_SP)obj).Follow_S = SK_Die.Follow_F;
		((SkillOBJ_DT_SP)obj).AllChuan_F = SK_Die.AllChuan_F;
		((SkillOBJ_DT_SP)obj).AllChuan_S = SK_Die.AllChuan_F;
		((SkillOBJ_DT_SP)obj).Slow_F = 1;
		((SkillOBJ_DT_SP)obj).Slow_S = 1;
		((SkillOBJ_DT_SP)obj).RDSpeed_F = SK_Die.RDSpeed_F;
		((SkillOBJ_DT_SP)obj).RDSpeed_S = SK_Die.RDSpeed_F;
		((SkillOBJ_DT_SP)obj).HasFX = 0;
		((SkillOBJ_DT_SP)obj).S_HasFX = 1;
		((SkillOBJ_DT_SP)obj).AB_HasFX = 1;
		((SkillOBJ_DT_SP)obj).colEXP = 1;
		((SkillOBJ_DT_SP)obj).colEXP_A = 1;
		((SkillOBJ_DT_SP)obj).S_colEXP = 1;
		((SkillOBJ_DT_SP)obj).AB_colEXP = 1;
		((SkillOBJ_DT_SP)obj).TimeEXP = 1;
		((SkillOBJ_DT_SP)obj).TimeEXP_AB = 1;
		((SkillOBJ_DT_SP)obj).LastEXP = 1;
		((SkillOBJ_DT_SP)obj).LastEXP_AB = 1;
		((SkillOBJ_DT_SP)obj).S_LastEXP = 1;
		((SkillOBJ_DT_SP)obj).AB_LastEXP = 1;
	}

	public void ChildClear()
	{
		cpList.Clear();
		foreach (Enemy fs in fsList)
		{
			fs.HealthStat.SetCurrent(0f);
		}
		fsList.Clear();
	}

	public void ChildAllDie()
	{
		foreach (Enemy cp in cpList)
		{
			cp.HealthStat.SetCurrent(0f);
		}
		foreach (Enemy fs in fsList)
		{
			fs.HealthStat.SetCurrent(0f);
		}
		fsList.Clear();
		cpList.Clear();
	}

	public void EnemyClear()
	{
		if ((bool)peo)
		{
			peo.DotEM.ClearDot();
		}
		if (AuraList.Count > 0)
		{
			foreach (GameObject aura in AuraList)
			{
				LeanPool.Despawn(aura);
			}
			AuraList.Clear();
		}
		if ((bool)LQJQ)
		{
			GameObject clone = LQJQ.gameObject;
			LQJQ = null;
			LeanPool.Despawn(clone);
		}
	}

	public void OffBuffFX()
	{
		foreach (GameObject aura in AuraList)
		{
			aura.gameObject.SetActive(value: false);
		}
		SD.enabled = false;
		canvas.alpha = 0f;
	}

	public void OnBuffFX()
	{
		foreach (GameObject aura in AuraList)
		{
			aura.gameObject.SetActive(value: true);
		}
		SD.enabled = true;
		canvas.alpha = 1f;
	}
}
