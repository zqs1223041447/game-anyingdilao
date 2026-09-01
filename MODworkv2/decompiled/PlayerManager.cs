using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Settings;
using Data.RuntimeData;
using Data.SaveData;
using Entity.Comp.CompanionAI;
using FMODUnity;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Inputs;
using Inputs.Gamepad;
using Interact;
using Lean.Pool;
using Level.LevelStates;
using Localization;
using Pathfinding;
using Spine;
using Spine.Unity;
using UI.Managers;
using UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : SingletonMonoScope<PlayerManager>
{
	[Tooltip("法师 0 圣骑士1 游侠2 死灵法师3")]
	public int PLType;

	public PlayerData[] PLDT;

	public string PlayerName;

	public bool AutoAttackEnabled;

	[HideInInspector]
	public MGC mgc;

	[HideInInspector]
	public SQS sqs;

	[HideInInspector]
	public ARC arc;

	[HideInInspector]
	public DEAD dead;

	[HideInInspector]
	public Stat HealStat;

	[HideInInspector]
	public Stat ManaStat;

	[HideInInspector]
	public XpStat XpStat;

	[HideInInspector]
	public XpStat_DF DFXpStat;

	public string[] SO_Hurt;

	public string[] SO_Die;

	[HideInInspector]
	public Vector2 Direction;

	[HideInInspector]
	public Rigidbody2D rigBD;

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
	public bool CanMove;

	[HideInInspector]
	public bool IsChong;

	[HideInInspector]
	public bool IsYun;

	[HideInInspector]
	public bool IsAttack;

	[HideInInspector]
	public bool IsSkill;

	[HideInInspector]
	public bool IsAttackAnimationSkill;

	[HideInInspector]
	public bool IsBattle;

	public int CurUseSK;

	public string BSname;

	[HideInInspector]
	public int FStype;

	[HideInInspector]
	public bool ChuanSong;

	[HideInInspector]
	public Vector3 ChuanSongPOS;

	[HideInInspector]
	public bool IScomp;

	public TrackEntry MoveTrack;

	public TrackEntry AttackTrack;

	public TrackEntry UpBOWTrack;

	public TrackEntry SkillTrack;

	public List<Enemy> em = new List<Enemy>();

	public List<Companion> cp = new List<Companion>();

	public Collider2D[] hitEM = new Collider2D[10];

	public float EM_Range;

	private const float AutoLockEnemyRange = 5f;

	private const float BurnLifeEnemyRange = 5f;

	private const float DeathExplosionEnemyRange = 5f;

	private const float DotBuffDamageRange = 5f;

	private const float NearbyEnemyStatRange = 5f;

	private Collider2D[] enemyRangeHits = new Collider2D[10];

	private List<Enemy> burnLifeEnemyBuffer = new List<Enemy>(50);

	private List<Enemy> deathExplosionEnemyBuffer = new List<Enemy>(50);

	private List<Enemy> dotBuffEnemyBuffer = new List<Enemy>(50);

	private List<Enemy> autoLockEnemyBuffer = new List<Enemy>(20);

	private List<Enemy> nearbyEnemyStatBuffer = new List<Enemy>(50);

	private int footCOLemLayerMask;

	private int blockWallLayerMask;

	private Transform autoLockYaoTarget;

	private Transform autoLockFootTarget;

	private bool autoLockRefreshInProgress;

	private GameObject movementDirectionTargetObject;

	private Transform movementDirectionTarget;

	private Vector2 lastMovementAimDirection = Vector2.right;

	private Vector2 lastGamepadLeftStickAimDirection = Vector2.right;

	private Transform companionFollow;

	private Transform companionFollowPoint;

	private List<Enemy> companionFollowEnemyBuffer = new List<Enemy>(30);

	private bool hasNearbyCompanionFollowEnemy;

	private float companionFollowAttackPush;

	private float companionFollowDirectionalPush;

	private int companionFollowBattleCount;

	private Vector3 lastCompanionFollowSkillAim;

	private Vector2 lastCompanionFollowSkillDirection;

	private bool hasLastCompanionFollowSkillAim;

	private const float CompanionFollowEnemyRange = 7f;

	private const float CompanionFollowBaseX = 1.5f;

	private const float CompanionFollowAttackPushX = 1f;

	private const float CompanionFollowDirectionalPushX = 1f;

	private const float CompanionFollowPushDecayDuration = 3f;

	public Collider2D[] DPIT = new Collider2D[10];

	private float timeLevel;

	private bool IsLevelUP;

	public Camera mainCam;

	public Camera minimapCam;

	public Camera worldmapCam;

	public bool lockMove;

	private bool isTeleporting;

	private const float TeleportEnemyPrewarmUnloadGrace = 1f;

	private AIPath mouseMovePath;

	private AIDestinationSetter mouseMoveDestinationSetter;

	private Seeker mouseMoveSeeker;

	private List<MonoBehaviour> mouseMovePathBehaviours = new List<MonoBehaviour>();

	private List<Collider2D> mouseMoveBlockingColliders = new List<Collider2D>();

	private List<bool> mouseMoveBlockingColliderOriginalEnabled = new List<bool>();

	private GameObject mouseMoveTargetObject;

	private Transform mouseMoveTarget;

	private bool mouseMoveRuntimeActive;

	private bool mouseMoveRuntimeInitialized;

	private bool mouseMoveHasTarget;

	private bool mouseMovePointerStartedOnUi;

	private bool mouseMoveWaitingForPath;

	private bool mouseMoveStationaryAttackBlocked;

	private bool mouseMoveResumeAfterMouseSkill;

	private bool mouseMovePendingClick;

	private Vector3 mouseMovePendingClickPosition;

	private float mouseMovePendingClickTime;

	private float mouseMovePathWaitStartedTime;

	private int bodyCOLemLayerMask;

	private Collider2D[] mouseMovePointerHits = new Collider2D[8];

	private const float MouseMoveMinTargetDistance = 0.3f;

	private const float MouseMoveStopDistance = 0.05f;

	private const float MouseMovePendingClickTimeout = 1f;

	private const float MouseMovePathWaitTimeout = 0.5f;

	[HideInInspector]
	public Vector3 mousePosition;

	[HideInInspector]
	public BuffMG_PL BuffMG;

	[HideInInspector]
	public PlayerBuffRuntime BuffRuntime;

	[HideInInspector]
	public Dictionary<int, int> EquippedSetCounts = new Dictionary<int, int>();

	[HideInInspector]
	public People peo;

	[HideInInspector]
	public XJL_FSQ xjl;

	public BuffManager TempleMG;

	public CompanionManager CPMG;

	public ACTbar ACT;

	[HideInInspector]
	public float ChongSpeed;

	[HideInInspector]
	public float TimeA;

	[HideInInspector]
	public float TimeB;

	[HideInInspector]
	public float TimeC;

	[HideInInspector]
	public float Health;

	[HideInInspector]
	public float Health_Bei;

	[HideInInspector]
	public float Health_Bei_Tmp;

	[HideInInspector]
	public float Health_R_Base;

	[HideInInspector]
	public float Health_R_Max;

	[HideInInspector]
	public float Health_Percent;

	[HideInInspector]
	public float Health_Percent_Tmp;

	[HideInInspector]
	public float Mana;

	[HideInInspector]
	public float Mana_Bei;

	[HideInInspector]
	public float Mana_R_Base;

	[HideInInspector]
	public float Mana_R_Max;

	[HideInInspector]
	public float Mana_Percent;

	[HideInInspector]
	public float Mana_Percent_Tmp;

	[HideInInspector]
	public float Attack_R_health_Base;

	[HideInInspector]
	public float Attack_R_health_Percent;

	[HideInInspector]
	public float Attack_R_mana_Base;

	[HideInInspector]
	public float Attack_R_mana_Percent;

	[HideInInspector]
	public int Level;

	[HideInInspector]
	public float Xp_Total;

	[HideInInspector]
	public float Xp_CurrentLevel;

	[HideInInspector]
	public int DFLevel;

	[HideInInspector]
	public float DFXp_Total;

	[HideInInspector]
	public float DFXp_CurrentLevel;

	[HideInInspector]
	public float Xp_Bei_Tmp;

	[HideInInspector]
	public float ATSpeed_Base;

	[HideInInspector]
	public float ATSpeed_Bei;

	[HideInInspector]
	public float ATSpeed_Tmp;

	[HideInInspector]
	public float ATSpeed_Tmp_Cut;

	[HideInInspector]
	public float ATSpeed_Last;

	[HideInInspector]
	public float MVSpeed_Base;

	[HideInInspector]
	public float MVSpeed_Bei;

	[HideInInspector]
	public float MVSpeed_Tmp;

	[HideInInspector]
	public float MVSpeed_Tmp_Cut;

	[HideInInspector]
	public float MVSpeed_Last;

	[HideInInspector]
	public float AntiSlow;

	[HideInInspector]
	public float CoolDown;

	[HideInInspector]
	public float CoolDown_Tmp;

	[HideInInspector]
	public float GeDang;

	[HideInInspector]
	public float GeDang_Tmp;

	[HideInInspector]
	public float BJrate;

	[HideInInspector]
	public float BJrate_Tmp;

	[HideInInspector]
	public float BJDamage;

	[HideInInspector]
	public float BJDamage_Tmp;

	[HideInInspector]
	public float BF_DMG_Last;

	[HideInInspector]
	public float JYrate;

	[HideInInspector]
	public float JYrate_Tmp;

	[HideInInspector]
	public float ThroughRate;

	[HideInInspector]
	public float ItemDrop_Rate;

	[HideInInspector]
	public float ItemDrop_Rate_buff_Tmp;

	[HideInInspector]
	public float ItemDrop_Rate_mijing_Tmp;

	[HideInInspector]
	public float DOTcut;

	[HideInInspector]
	public float Damage_Anti;

	[HideInInspector]
	public float Damage_Anti_Tmp;

	[HideInInspector]
	public float FlySpeed;

	[NonSerialized]
	[HideInInspector]
	public float BS_ExtraProjectiles;

	[HideInInspector]
	public float ORB_Damage;

	[HideInInspector]
	public float Damage_Base;

	[HideInInspector]
	public float Damage_Bei;

	[HideInInspector]
	public float Damage_Bei_Tmp;

	[HideInInspector]
	public float Damage_Cut;

	[HideInInspector]
	public float FireDamageXi;

	[HideInInspector]
	public float FrozenDamageXi;

	[HideInInspector]
	public float ThunderDamageXi;

	[HideInInspector]
	public float PoisonDamageXi;

	[HideInInspector]
	public float PhysicsDamageXi;

	[HideInInspector]
	public float ShadowDamageXi;

	[HideInInspector]
	public float FireDamage_Bei;

	[HideInInspector]
	public float FrozenDamage_Bei;

	[HideInInspector]
	public float ThunderDamage_Bei;

	[HideInInspector]
	public float PoisonDamage_Bei;

	[HideInInspector]
	public float PhysicsDamage_Bei;

	[HideInInspector]
	public float ShadowDamage_Bei;

	[HideInInspector]
	public float FireDamage_Bei_Tmp;

	[HideInInspector]
	public float FrozenDamage_Bei_Tmp;

	[HideInInspector]
	public float ThunderDamage_Bei_Tmp;

	[HideInInspector]
	public float PoisonDamage_Bei_Tmp;

	[HideInInspector]
	public float PhysicsDamage_Bei_Tmp;

	[HideInInspector]
	public float ShadowDamage_Bei_Tmp;

	[HideInInspector]
	public float FireChuan;

	[HideInInspector]
	public float FrozenChuan;

	[HideInInspector]
	public float ThunderChuan;

	[HideInInspector]
	public float PoisonChuan;

	[HideInInspector]
	public float PhysicsChuan;

	[HideInInspector]
	public float ShadowChuan;

	[HideInInspector]
	public float FireChuan_Tmp;

	[HideInInspector]
	public float FrozenChuan_Tmp;

	[HideInInspector]
	public float ThunderChuan_Tmp;

	[HideInInspector]
	public float PoisonChuan_Tmp;

	[HideInInspector]
	public float PhysicsChuan_Tmp;

	[HideInInspector]
	public float ShadowChuan_Tmp;

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
	public float FireAnti_Tmp;

	[HideInInspector]
	public float FrozenAnti_Tmp;

	[HideInInspector]
	public float ThunderAnti_Tmp;

	[HideInInspector]
	public float PoisonAnti_Tmp;

	[HideInInspector]
	public float PhysicsAnti_Tmp;

	[HideInInspector]
	public float ShadowAnti_Tmp;

	[HideInInspector]
	public float C_Health;

	[HideInInspector]
	public float C_Damage;

	[HideInInspector]
	public float C_ATSpeed;

	[HideInInspector]
	public float C_MVSpeed;

	[HideInInspector]
	public float C_AllAnti;

	[HideInInspector]
	public float C_Health_Tmp;

	[HideInInspector]
	public float C_Damage_Tmp;

	[HideInInspector]
	public float C_ATSpeed_Tmp;

	[HideInInspector]
	public float C_MVSpeed_Tmp;

	[HideInInspector]
	public float C_AllAnti_Tmp;

	[HideInInspector]
	public float Runtime_DotDamage_Tmp;

	[HideInInspector]
	public float Runtime_DotTimeCut_Tmp;

	[HideInInspector]
	public float Runtime_ORB_Damage_Tmp;

	[HideInInspector]
	public float Runtime_XJ_DMG_Tmp;

	[HideInInspector]
	public float ManaXH;

	[HideInInspector]
	public float BJD_Anti;

	[HideInInspector]
	public float AllChuan;

	[HideInInspector]
	public float AllAnti;

	[HideInInspector]
	public float BuffT_Temple;

	[HideInInspector]
	public float BuffT_Drink;

	[HideInInspector]
	public int WPSPC_DMG;

	[HideInInspector]
	public int WPSPC_Rate;

	[HideInInspector]
	public int JYBoss_DMG;

	[HideInInspector]
	public int JYBoss_Anti;

	[HideInInspector]
	public float DMG_R_H;

	[HideInInspector]
	public float DMG_R_M;

	[HideInInspector]
	public int BS_Add;

	[HideInInspector]
	public float BS_Multi;

	[HideInInspector]
	public int Top_CD;

	[HideInInspector]
	public int Top_GD;

	[HideInInspector]
	public int Top_Anti;

	[HideInInspector]
	public float Top_Cut_DMG;

	[HideInInspector]
	public float Top_Cut_ATS;

	[HideInInspector]
	public float Top_Cut_MVS;

	[HideInInspector]
	public PlayerDotData[] DOT;

	private const int PlayerDotElementCount = 6;

	[HideInInspector]
	public float AllDot_DMG;

	[HideInInspector]
	public float AllDot_Time;

	[HideInInspector]
	public int AllDot_Layer;

	[HideInInspector]
	public float AllDot_MV;

	[HideInInspector]
	public float AllDot_JY;

	[HideInInspector]
	public float DiffDotDMG;

	[HideInInspector]
	public int DiffDebuff_DMG;

	[HideInInspector]
	public bool Dot_MSAll;

	[HideInInspector]
	public int BE_ZQ_Count;

	[HideInInspector]
	public float BE_ZQ_DMG;

	[HideInInspector]
	public float BE_ZQ_ATS;

	[HideInInspector]
	public float BE_ZQ_MVS;

	[HideInInspector]
	public float BE_ZQ_BJR;

	[HideInInspector]
	public float BE_ZQ_BJD;

	[HideInInspector]
	public float BE_ZQ_Heal;

	[HideInInspector]
	public float BE_ZQ_Mana;

	[HideInInspector]
	public float BE_ZQ_CP_Heal;

	[HideInInspector]
	public float BE_ZQ_CP_DMG;

	[HideInInspector]
	public float BE_ZQ_CP_ATS;

	[HideInInspector]
	public float BE_ZQ_CP_MVS;

	[HideInInspector]
	public float BE_ZQ_CP_Anti;

	[HideInInspector]
	public float BE_ZQ_Dot;

	[HideInInspector]
	public float BE_ZQ_XJ_DMG;

	[HideInInspector]
	public float BE_ZQ_Orb_DMG;

	[HideInInspector]
	public int BE_SPC_Count;

	[HideInInspector]
	public float BE_SPC_DMG;

	[HideInInspector]
	public float BE_SPC_ATS;

	[HideInInspector]
	public float BE_SPC_MVS;

	[HideInInspector]
	public float BE_SPC_BJR;

	[HideInInspector]
	public float BE_SPC_BJD;

	[HideInInspector]
	public float BE_SPC_Heal;

	[HideInInspector]
	public float BE_SPC_Mana;

	[HideInInspector]
	public float BE_SPC_CP_Heal;

	[HideInInspector]
	public float BE_SPC_CP_DMG;

	[HideInInspector]
	public float BE_SPC_CP_ATS;

	[HideInInspector]
	public float BE_SPC_CP_MVS;

	[HideInInspector]
	public float BE_SPC_CP_Anti;

	[HideInInspector]
	public float BE_SPC_Dot;

	[HideInInspector]
	public float BE_SPC_XJ_DMG;

	[HideInInspector]
	public float BE_SPC_Orb_DMG;

	[HideInInspector]
	public int BE_HH_Count;

	[HideInInspector]
	public float BE_HH_DMG;

	[HideInInspector]
	public float BE_HH_ATS;

	[HideInInspector]
	public float BE_HH_MVS;

	[HideInInspector]
	public float BE_HH_BJR;

	[HideInInspector]
	public float BE_HH_BJD;

	[HideInInspector]
	public float BE_HH_Heal;

	[HideInInspector]
	public float BE_HH_Mana;

	[HideInInspector]
	public float BE_HH_CP_Heal;

	[HideInInspector]
	public float BE_HH_CP_DMG;

	[HideInInspector]
	public float BE_HH_CP_ATS;

	[HideInInspector]
	public float BE_HH_CP_MVS;

	[HideInInspector]
	public float BE_HH_CP_Anti;

	[HideInInspector]
	public float BE_HH_Dot;

	[HideInInspector]
	public float BE_HH_XJ_DMG;

	[HideInInspector]
	public float BE_HH_Orb_DMG;

	[HideInInspector]
	public int BE_SK_Count;

	[HideInInspector]
	public float BE_SK_DMG;

	[HideInInspector]
	public float BE_SK_ATS;

	[HideInInspector]
	public float BE_SK_MVS;

	[HideInInspector]
	public float BE_SK_CP_Heal;

	[HideInInspector]
	public float BE_SK_CP_DMG;

	[HideInInspector]
	public float BE_SK_CP_ATS;

	[HideInInspector]
	public float BE_SK_CP_Anti;

	[HideInInspector]
	public float BE_SK_XJ_DMG;

	[HideInInspector]
	public float BE_SK_Orb_DMG;

	[HideInInspector]
	public int BE_SK_FQ_Count;

	[HideInInspector]
	public int BE_BS_Count;

	[HideInInspector]
	public float BE_BS_DMG;

	[HideInInspector]
	public float BE_BS_ATS;

	[HideInInspector]
	public float BE_BS_MVS;

	[HideInInspector]
	public float BE_BS_CP_Heal;

	[HideInInspector]
	public float BE_BS_CP_DMG;

	[HideInInspector]
	public float BE_BS_CP_ATS;

	[HideInInspector]
	public float BE_BS_CP_Anti;

	[HideInInspector]
	public float BE_BS_XJ_DMG;

	[HideInInspector]
	public float BE_BS_Orb_DMG;

	[HideInInspector]
	public int BE_BS_FQ_Count;

	[HideInInspector]
	public int LowH_DMG20;

	[HideInInspector]
	public int LowH_DMG50;

	[HideInInspector]
	public int HighH_DMG90;

	[HideInInspector]
	public int HighH_DMG100;

	[HideInInspector]
	public int LowH_HurtR20;

	[HideInInspector]
	public int HighH_HurtR100;

	[HideInInspector]
	public int LowH_DMGAnti20;

	[HideInInspector]
	public int LowH_DMGAnti50;

	[HideInInspector]
	public bool LowH_CritAnti10;

	[HideInInspector]
	public int LowM_DMG20;

	[HideInInspector]
	public int LowM_DMG50;

	[HideInInspector]
	public int HighM_DMG90;

	[HideInInspector]
	public int HighM_DMG100;

	[HideInInspector]
	public int LowM_HurtR20;

	[HideInInspector]
	public int HighM_HurtR100;

	[HideInInspector]
	public int ST_MV_DMG;

	[HideInInspector]
	public int ST_MV_ATS;

	[HideInInspector]
	public int ST_MV_GD;

	[HideInInspector]
	public int ST_NoMV_DMG;

	[HideInInspector]
	public int ST_NoMV_ATS;

	[HideInInspector]
	public int ST_NoMV_DMGAnti;

	[HideInInspector]
	public float ST_NoMV_HealPrc;

	[HideInInspector]
	public float ST_NoMV_ManaPrc;

	[HideInInspector]
	public int ST_Chong_DMG;

	[HideInInspector]
	public int ST_Chong_Anti;

	[HideInInspector]
	public float Z_Hmax_DMG;

	[HideInInspector]
	public float Z_Huse_DMG;

	[HideInInspector]
	public float Z_Mmax_DMG;

	[HideInInspector]
	public float Z_Mcur_DMG;

	[HideInInspector]
	public float Z_Muse_DMG;

	[HideInInspector]
	public float Z_Hmax_EL0;

	[HideInInspector]
	public float Z_Hmax_EL1;

	[HideInInspector]
	public float Z_Hmax_EL2;

	[HideInInspector]
	public float Z_Hmax_EL3;

	[HideInInspector]
	public float Z_Hmax_EL4;

	[HideInInspector]
	public float Z_Hmax_EL5;

	[HideInInspector]
	public float Z_Mmax_EL0;

	[HideInInspector]
	public float Z_Mmax_EL1;

	[HideInInspector]
	public float Z_Mmax_EL2;

	[HideInInspector]
	public float Z_Mmax_EL3;

	[HideInInspector]
	public float Z_Mmax_EL4;

	[HideInInspector]
	public float Z_Mmax_EL5;

	[HideInInspector]
	public float Z_CD_EL0;

	[HideInInspector]
	public float Z_CD_EL1;

	[HideInInspector]
	public float Z_CD_EL2;

	[HideInInspector]
	public float Z_CD_EL3;

	[HideInInspector]
	public float Z_CD_EL4;

	[HideInInspector]
	public float Z_CD_EL5;

	[HideInInspector]
	public int Z_Anti0_EL0;

	[HideInInspector]
	public int Z_Anti0_EL1;

	[HideInInspector]
	public int Z_Anti0_EL2;

	[HideInInspector]
	public int Z_Anti0_EL3;

	[HideInInspector]
	public int Z_Anti0_EL4;

	[HideInInspector]
	public int Z_Anti0_EL5;

	[HideInInspector]
	public int Z_Chuan0_EL0;

	[HideInInspector]
	public int Z_Chuan0_EL1;

	[HideInInspector]
	public int Z_Chuan0_EL2;

	[HideInInspector]
	public int Z_Chuan0_EL3;

	[HideInInspector]
	public int Z_Chuan0_EL4;

	[HideInInspector]
	public int Z_Chuan0_EL5;

	[HideInInspector]
	public int Z_GD_EL0;

	[HideInInspector]
	public int Z_GD_EL1;

	[HideInInspector]
	public int Z_GD_EL2;

	[HideInInspector]
	public int Z_GD_EL3;

	[HideInInspector]
	public int Z_GD_EL4;

	[HideInInspector]
	public int Z_GD_EL5;

	[HideInInspector]
	public int Z_BJR_EL0;

	[HideInInspector]
	public int Z_BJR_EL1;

	[HideInInspector]
	public int Z_BJR_EL2;

	[HideInInspector]
	public int Z_BJR_EL3;

	[HideInInspector]
	public int Z_BJR_EL4;

	[HideInInspector]
	public int Z_BJR_EL5;

	[HideInInspector]
	public int Z_DMGCut_EL0;

	[HideInInspector]
	public int Z_DMGCut_EL1;

	[HideInInspector]
	public int Z_DMGCut_EL2;

	[HideInInspector]
	public int Z_DMGCut_EL3;

	[HideInInspector]
	public int Z_DMGCut_EL4;

	[HideInInspector]
	public int Z_DMGCut_EL5;

	[HideInInspector]
	public int Z_Thr_EL0;

	[HideInInspector]
	public int Z_Thr_EL1;

	[HideInInspector]
	public int Z_Thr_EL2;

	[HideInInspector]
	public int Z_Thr_EL3;

	[HideInInspector]
	public int Z_Thr_EL4;

	[HideInInspector]
	public int Z_Thr_EL5;

	[HideInInspector]
	public float Z_CD_CP_DMG;

	[HideInInspector]
	public float Z_ATS_CP_DMG;

	[HideInInspector]
	public float Z_MVS_DMG;

	[HideInInspector]
	public float Z_MVS_ATS;

	[HideInInspector]
	public bool Z_BJR_BJD;

	[HideInInspector]
	public int Z_Chuan0_BJD;

	[HideInInspector]
	public int Z_Chuan1_BJD;

	[HideInInspector]
	public int Z_Chuan2_BJD;

	[HideInInspector]
	public int Z_Chuan3_BJD;

	[HideInInspector]
	public int Z_Chuan4_BJD;

	[HideInInspector]
	public int Z_Chuan5_BJD;

	[HideInInspector]
	public float ST_EveryH_DMG;

	[HideInInspector]
	public float ST_EveryM_Drop;

	[HideInInspector]
	public bool AB_DMG_Mana;

	[HideInInspector]
	public bool AB_DMG_Hurt;

	[HideInInspector]
	public bool AB_Dot_DMG;

	[HideInInspector]
	public bool NoGD;

	[HideInInspector]
	public int Attack_DMG1;

	[HideInInspector]
	public int Attack_DMG2;

	[HideInInspector]
	public int Attack_ATS1;

	[HideInInspector]
	public int Attack_ATS2;

	[HideInInspector]
	public int Attack_Chuan;

	[HideInInspector]
	public int Attack_BJR;

	[HideInInspector]
	public int Attack_BJD;

	[HideInInspector]
	public int Attack_DotDMG1;

	[HideInInspector]
	public int Attack_DotDMG2;

	[HideInInspector]
	public int BuffEvery_CP;

	[HideInInspector]
	public int Z_Dot_EL;

	[HideInInspector]
	public int Z_Dot_MV;

	[HideInInspector]
	public int Clear1;

	[HideInInspector]
	public int Clear2;

	[HideInInspector]
	public int GD_DMG;

	[HideInInspector]
	public int Final_Diff_DMG;

	[HideInInspector]
	public int PickBS_MVS;

	[HideInInspector]
	public int NoUseSK_DMG1;

	[HideInInspector]
	public int NoUseSK_DMG2;

	[HideInInspector]
	public int TP_DMG;

	[HideInInspector]
	public int MV_DMG;

	[HideInInspector]
	public bool DeadWD;

	[HideInInspector]
	public bool DeadRageWD;

	[HideInInspector]
	public bool DeadStealthWD;

	public int CompCount;

	[HideInInspector]
	public float CP1_DMG;

	[HideInInspector]
	public float CP1_ATS;

	[HideInInspector]
	public float CP1_MVS;

	[HideInInspector]
	public float CP1_Heal;

	[HideInInspector]
	public float CP1_Mana;

	[HideInInspector]
	public float CP1_DMG_Anti;

	[HideInInspector]
	public float CP1_DropR;

	[HideInInspector]
	public float CP1_ORB_DMG;

	[HideInInspector]
	public float CP1_DMG0;

	[HideInInspector]
	public float CP1_DMG1;

	[HideInInspector]
	public float CP1_DMG2;

	[HideInInspector]
	public float CP1_DMG3;

	[HideInInspector]
	public float CP1_DMG4;

	[HideInInspector]
	public float CP1_DMG5;

	[HideInInspector]
	public float CP1_Chuan0;

	[HideInInspector]
	public float CP1_Chuan1;

	[HideInInspector]
	public float CP1_Chuan2;

	[HideInInspector]
	public float CP1_Chuan3;

	[HideInInspector]
	public float CP1_Chuan4;

	[HideInInspector]
	public float CP1_Chuan5;

	[HideInInspector]
	public float CP1_CP_Heal;

	[HideInInspector]
	public float CP1_CP_DMG;

	[HideInInspector]
	public float CP1_CP_ATS;

	[HideInInspector]
	public float CP1_CP_AllAnti;

	[HideInInspector]
	public float CLass_DMG;

	[HideInInspector]
	public float CLass_ATS;

	[HideInInspector]
	public float CLass_MVS;

	[HideInInspector]
	public float CLass_Heal;

	[HideInInspector]
	public float CLass_Mana;

	[HideInInspector]
	public float CLass_DMG_Anti;

	[HideInInspector]
	public float CLass_DropR;

	[HideInInspector]
	public float CLass_ORB_DMG;

	[HideInInspector]
	public float CLass_DMG0;

	[HideInInspector]
	public float CLass_DMG1;

	[HideInInspector]
	public float CLass_DMG2;

	[HideInInspector]
	public float CLass_DMG3;

	[HideInInspector]
	public float CLass_DMG4;

	[HideInInspector]
	public float CLass_DMG5;

	[HideInInspector]
	public float CLass_Chuan0;

	[HideInInspector]
	public float CLass_Chuan1;

	[HideInInspector]
	public float CLass_Chuan2;

	[HideInInspector]
	public float CLass_Chuan3;

	[HideInInspector]
	public float CLass_Chuan4;

	[HideInInspector]
	public float CLass_Chuan5;

	[HideInInspector]
	public float CLass_CP_Heal;

	[HideInInspector]
	public float CLass_CP_DMG;

	[HideInInspector]
	public float CLass_CP_ATS;

	[HideInInspector]
	public float CLass_CP_AllAnti;

	[HideInInspector]
	public float Class_CP_DotDMG;

	[HideInInspector]
	public float DMG_1;

	[HideInInspector]
	public float DMG_2;

	[HideInInspector]
	public float DMG_3;

	[HideInInspector]
	public float DMG_4;

	[HideInInspector]
	public float DMG_5;

	[HideInInspector]
	public float DMG_6;

	[HideInInspector]
	public float DMG_7;

	[HideInInspector]
	public float DMG_8;

	[HideInInspector]
	public float DMG_9;

	[HideInInspector]
	public float DMG_10;

	[HideInInspector]
	public float DMG_11;

	[HideInInspector]
	public float DMG_12;

	[HideInInspector]
	public float DMG_13;

	[HideInInspector]
	public float DMG_14;

	[HideInInspector]
	public float DMG_15;

	[HideInInspector]
	public float DMG_16;

	[HideInInspector]
	public float DMG_17;

	[HideInInspector]
	public float DMG_18;

	[HideInInspector]
	public float DMG_19;

	[HideInInspector]
	public float DMG_20;

	[HideInInspector]
	public float DMG_30;

	[HideInInspector]
	public float DMG_40;

	[HideInInspector]
	public float DMG_41;

	[HideInInspector]
	public float DMG_42;

	[HideInInspector]
	public float DMG_43;

	[HideInInspector]
	public float DMG_44;

	[HideInInspector]
	public float DMG_45;

	[HideInInspector]
	public float DMG_51;

	[HideInInspector]
	public float DMG_52;

	[HideInInspector]
	public float DMG_53;

	[HideInInspector]
	public float DMG_54;

	[HideInInspector]
	public float DMG_55;

	[HideInInspector]
	public float DMG_56;

	[HideInInspector]
	public float DMG_70;

	[HideInInspector]
	public float DMG_71;

	[HideInInspector]
	public float DMG_72;

	[HideInInspector]
	public float DMG_73;

	[HideInInspector]
	public float DMG_74;

	[HideInInspector]
	public float DMG_75;

	[HideInInspector]
	public float DMG_80;

	[HideInInspector]
	public float DMG_81;

	[HideInInspector]
	public float DMG_82;

	[HideInInspector]
	public float DMG_83;

	[HideInInspector]
	public float DMG_84;

	[HideInInspector]
	public float DMG_85;

	[HideInInspector]
	public float DMG_86;

	[HideInInspector]
	public float DMG_90;

	[HideInInspector]
	public int XJ_Count_CP_DMG;

	[HideInInspector]
	public int Q_1;

	[HideInInspector]
	public int Q_2;

	[HideInInspector]
	public int Q_3;

	[HideInInspector]
	public int Q_4;

	[HideInInspector]
	public int Q_5;

	[HideInInspector]
	public int Q_6;

	[HideInInspector]
	public int Q_7;

	[HideInInspector]
	public int Q_8;

	[HideInInspector]
	public int Q_9;

	[HideInInspector]
	public int Q_10;

	[HideInInspector]
	public int Q_11;

	[HideInInspector]
	public int Q_12;

	[HideInInspector]
	public int Q_13;

	[HideInInspector]
	public int Q_14;

	[HideInInspector]
	public int Q_15;

	[HideInInspector]
	public int Q_16;

	[HideInInspector]
	public int Q_17;

	[HideInInspector]
	public int Q_18;

	[HideInInspector]
	public int Q_19;

	[HideInInspector]
	public int Q_20;

	[HideInInspector]
	public int Q_30;

	[HideInInspector]
	public int Q_40;

	[HideInInspector]
	public int Q_41;

	[HideInInspector]
	public int Q_42;

	[HideInInspector]
	public int Q_43;

	[HideInInspector]
	public int Q_44;

	[HideInInspector]
	public int Q_45;

	[HideInInspector]
	public int Q_51;

	[HideInInspector]
	public int Q_52;

	[HideInInspector]
	public int Q_53;

	[HideInInspector]
	public int Q_54;

	[HideInInspector]
	public int Q_55;

	[HideInInspector]
	public int Q_56;

	[HideInInspector]
	public int Q_70;

	[HideInInspector]
	public int Q_71;

	[HideInInspector]
	public int Q_72;

	[HideInInspector]
	public int Q_73;

	[HideInInspector]
	public int Q_74;

	[HideInInspector]
	public int Q_75;

	[HideInInspector]
	public int Q_80;

	[HideInInspector]
	public int Q_81;

	[HideInInspector]
	public int Q_82;

	[HideInInspector]
	public int Q_83;

	[HideInInspector]
	public int Q_84;

	[HideInInspector]
	public int Q_85;

	[HideInInspector]
	public int Q_86;

	[HideInInspector]
	public int Q_90;

	[HideInInspector]
	public int NearEMC;

	[HideInInspector]
	public int NearJYC;

	[HideInInspector]
	public float EMC_DMG_20;

	[HideInInspector]
	public float EMC_DMG_48;

	[HideInInspector]
	public float EMC_Anti_9;

	[HideInInspector]
	public float EMC_GD_12;

	[HideInInspector]
	public float JYC_DMG_15;

	[HideInInspector]
	public float JYC_ATS_24;

	[HideInInspector]
	public float JYC_BJD_24;

	[HideInInspector]
	public int Kem_DMG1;

	[HideInInspector]
	public int Kem_DMG2;

	[HideInInspector]
	public int Kem_ATS1;

	[HideInInspector]
	public int Kem_ATS2;

	[HideInInspector]
	public int Kem_EL0;

	[HideInInspector]
	public int Kem_EL1;

	[HideInInspector]
	public int Kem_EL2;

	[HideInInspector]
	public int Kem_EL3;

	[HideInInspector]
	public int Kem_EL4;

	[HideInInspector]
	public int Kem_EL5;

	[HideInInspector]
	public int Kem_CP_DMG1;

	[HideInInspector]
	public int Kem_CP_DMG2;

	[HideInInspector]
	public int Kem_CP_ATS1;

	[HideInInspector]
	public int Kem_CP_ATS2;

	[HideInInspector]
	public int Kjy_DMG;

	[HideInInspector]
	public int Kjy_AllAnti;

	[HideInInspector]
	public int Kem_Refresh;

	[HideInInspector]
	public int PrcCut0;

	[HideInInspector]
	public int PrcCut1;

	[HideInInspector]
	public int PrcCut2;

	[HideInInspector]
	public int PrcCut3;

	[HideInInspector]
	public int PrcCut4;

	[HideInInspector]
	public int PrcCut5;

	[HideInInspector]
	public int PrcCut5P0;

	[HideInInspector]
	public int PrcCut5P1;

	[HideInInspector]
	public int PrcCut5P2;

	[HideInInspector]
	public int PrcCut5P3;

	[HideInInspector]
	public int PrcCut5P4;

	[HideInInspector]
	public int PrcCut5P5;

	[HideInInspector]
	public int PrcCut3P0;

	[HideInInspector]
	public int PrcCut3P1;

	[HideInInspector]
	public int PrcCut3P2;

	[HideInInspector]
	public int PrcCut3P3;

	[HideInInspector]
	public int PrcCut3P4;

	[HideInInspector]
	public int PrcCut3P5;

	[HideInInspector]
	public int BurnLife0;

	[HideInInspector]
	public int BurnLife1;

	[HideInInspector]
	public int BurnLife2;

	[HideInInspector]
	public int BurnLife3;

	[HideInInspector]
	public int BurnLife4;

	[HideInInspector]
	public int BurnLife5;

	[HideInInspector]
	public bool WS_Anti0;

	[HideInInspector]
	public bool WS_Anti1;

	[HideInInspector]
	public bool WS_Anti2;

	[HideInInspector]
	public bool WS_Anti3;

	[HideInInspector]
	public bool WS_Anti4;

	[HideInInspector]
	public bool WS_Anti5;

	[HideInInspector]
	public bool WS_All;

	[HideInInspector]
	public int EM_LowH_DMG20;

	[HideInInspector]
	public int EM_LowH_DMG50;

	[HideInInspector]
	public int EM_HighH_DMG60;

	[HideInInspector]
	public int EM_HighH_DMG100;

	[HideInInspector]
	public int EM_Heal_Crit;

	[HideInInspector]
	public int Dis_In;

	[HideInInspector]
	public bool Dis_Out;

	[HideInInspector]
	public int Crit_BoomEXP;

	[HideInInspector]
	public int Crit_BoomDie_Rate;

	[HideInInspector]
	public int Crit_MS;

	[HideInInspector]
	public int ORB_FQ_Count;

	[HideInInspector]
	public bool ORB_FQ_Count_Double;

	[HideInInspector]
	public int ORB_FQ_DMG80_Base;

	[HideInInspector]
	public int ORB_FQ_DMG120_Base;

	[HideInInspector]
	public float Orb_Universe_DMG_Base;

	[HideInInspector]
	public int HighMana_DMG100_FQ;

	[HideInInspector]
	public float Orb_Universe_ATS;

	[HideInInspector]
	public float Orb_Bow_DMG;

	[HideInInspector]
	public float Orb_Bow_ATS;

	[HideInInspector]
	public float Orb_Bow_DMG_ORB;

	[HideInInspector]
	public float Orb_Bow_DMG_Anti;

	[HideInInspector]
	public int XJ_DMG;

	[HideInInspector]
	public int XJ_Time;

	[HideInInspector]
	public int TuT_Buff;

	[HideInInspector]
	public int TuT_Time;

	[HideInInspector]
	public bool TuT_PlayerAll;

	[HideInInspector]
	public int NoDot_BJD;

	[HideInInspector]
	public bool HealCutMana;

	[HideInInspector]
	public int ManaUse_Rheal;

	[HideInInspector]
	public bool RMana_RHeal;

	[HideInInspector]
	public bool CP_Same_RHeal;

	[HideInInspector]
	public bool FT;

	[HideInInspector]
	public int DMG_ManaPRC;

	[HideInInspector]
	public bool Turtle;

	[HideInInspector]
	public int GD_HurtR;

	[HideInInspector]
	public bool BloodLost;

	[HideInInspector]
	public bool NoGround;

	[HideInInspector]
	public bool CPNoBad;

	[HideInInspector]
	public bool CPNoGround;

	[HideInInspector]
	public bool AT_UseHeal1;

	[HideInInspector]
	public bool AT_UseHeal2;

	[HideInInspector]
	public float DMGsplit;

	[HideInInspector]
	public bool BladeSoul_Double;

	[HideInInspector]
	public int Diff_EL;

	[HideInInspector]
	public float EXP_Range;

	[HideInInspector]
	public float Buff_Range;

	[HideInInspector]
	public bool MoneyTO_DMG;

	[HideInInspector]
	public bool AutoJH;

	private const float AutoJHInterval = 0.15f;

	private const float AutoJHRange = 2f;

	private float autoJHNextCheckTime;

	private Collider2D[] autoJHColliders = new Collider2D[5];

	[HideInInspector]
	public bool DieEXP;

	private const float AutoDrinkHealthPercent = 0.2f;

	private const float AutoDrinkManaPercent = 0.1f;

	[HideInInspector]
	public bool AutoDrinkH;

	[HideInInspector]
	public bool AutoDrinkM;

	[HideInInspector]
	public bool Drink_CP;

	[HideInInspector]
	public int DrinkPre_Heal;

	[HideInInspector]
	public int DrinkPre_Mana;

	[HideInInspector]
	public int DrinkPre_DMG;

	[HideInInspector]
	public float Pick_PL_Base;

	[HideInInspector]
	public float Pick_PL_Bei;

	public float Pick_PL_Percent;

	[HideInInspector]
	public float Pick_XJL_Base;

	[HideInInspector]
	public float Pick_XJL_Bei;

	[HideInInspector]
	public float Pick_XJL_Percent;

	[HideInInspector]
	public float XJL_SellPrice;

	[HideInInspector]
	public float XJL_Count;

	[HideInInspector]
	public float XJL_DMG;

	[HideInInspector]
	public float XJL_DropMulti;

	[HideInInspector]
	public float XJL_UseSKTime;

	[HideInInspector]
	public float XJL_BJD_Anti_Tmp;

	[HideInInspector]
	public int Reforge_Inc;

	[HideInInspector]
	public int QH_Inc;

	[HideInInspector]
	public int HH_Inc;

	[HideInInspector]
	public int SK_Inc;

	[HideInInspector]
	public int QH_Price;

	[HideInInspector]
	public int QH_Bei;

	[HideInInspector]
	public int Temple_DMG;

	[HideInInspector]
	public int Temple_ATS;

	[HideInInspector]
	public int Temple_MVS;

	[HideInInspector]
	public float Temple_HealPrc;

	[HideInInspector]
	public int Temple_BS;

	[HideInInspector]
	public int SKUP_Xi;

	[HideInInspector]
	public int SKUP_SP;

	[HideInInspector]
	public int SKUP_CP;

	[HideInInspector]
	public int SKUP_Bei;

	[HideInInspector]
	public int SKUP_Final;

	[HideInInspector]
	public int SKUP_AT;

	public PlayerSaveData SaveData;

	private const int MaxPlayerLevel = 100;

	public bool hasDeaded;

	[HideInInspector]
	public bool PendingRebornAutoSummons;

	private bool deathStateCheckEnabled;

	public bool IsAttacking { get; set; }

	public bool IsMoving
	{
		get
		{
			if (Direction.x == 0f)
			{
				return Direction.y != 0f;
			}
			return true;
		}
	}

	public bool IsAlive
	{
		get
		{
			Stat healStat = HealStat;
			if ((object)healStat == null)
			{
				return false;
			}
			return healStat.Cur > 0f;
		}
	}

	public float MoveMulti => MVSpeed_Last * 0.65f;

	public float MoveAnimationTimeScale => Mathf.Max(0.05f, MoveMulti);

	public float AttackAnimationTimeScale => Mathf.Max(0.05f, (ATSpeed_Last > 0f) ? ATSpeed_Last : ATSpeed_Base);

	public float SkillAnimationTimeScale
	{
		get
		{
			if (ATSpeed_Last <= 0f)
			{
				return 1f;
			}
			return Mathf.Max(0.05f, SkillSpeed_Max);
		}
	}

	[HideInInspector]
	public float Health_Bei_Last
	{
		get
		{
			int num = (ACT ? ACT.GetCP_CT() : 0);
			int num2 = (ACT ? ACT.GetCPClass_CT() : 0);
			return Health_Bei + Health_Bei_Tmp + CP1_Heal * (float)num + CLass_Heal * (float)num2 + GetBoGD + (float)ACT.GetEveryCompHeal();
		}
	}

	[HideInInspector]
	public float Health_Percent_Last
	{
		get
		{
			int num = ((TempleMG != null) ? TempleMG.TempleList.Count : 0);
			float num2 = Temple_HealPrc * (float)num;
			if (IsMoving)
			{
				return Health_Percent + Health_Percent_Tmp + BE_ZQ_Heal * (float)BE_ZQ_Count + BE_SPC_Heal * (float)BE_SPC_Count + BE_HH_Heal * (float)BE_HH_Count + num2;
			}
			return Health_Percent + Health_Percent_Tmp + ST_NoMV_HealPrc + BE_ZQ_Heal * (float)BE_ZQ_Count + BE_SPC_Heal * (float)BE_SPC_Count + BE_HH_Heal * (float)BE_HH_Count + num2;
		}
	}

	[HideInInspector]
	public float Mana_Bei_Last
	{
		get
		{
			int num = (ACT ? ACT.GetCP_CT() : 0);
			int num2 = (ACT ? ACT.GetCPClass_CT() : 0);
			return Mana_Bei + CP1_Mana * (float)num + CLass_Mana * (float)num2 + (float)ACT.GetEveryCompMana();
		}
	}

	[HideInInspector]
	public float Mana_Percent_Last
	{
		get
		{
			if (IsMoving)
			{
				return Mana_Percent + Mana_Percent_Tmp + BE_ZQ_Mana * (float)BE_ZQ_Count + BE_SPC_Mana * (float)BE_SPC_Count + BE_HH_Mana * (float)BE_HH_Count;
			}
			return Mana_Percent + Mana_Percent_Tmp + ST_NoMV_ManaPrc + BE_ZQ_Mana * (float)BE_ZQ_Count + BE_SPC_Mana * (float)BE_SPC_Count + BE_HH_Mana * (float)BE_HH_Count;
		}
	}

	[HideInInspector]
	public float Attack_R_health_Max => Attack_R_health_Base + HealStat.Max * Attack_R_health_Percent;

	[HideInInspector]
	public float Attack_R_mana_Max => Attack_R_mana_Base + ManaStat.Max * Attack_R_mana_Percent;

	[HideInInspector]
	public float ATSpeed_Max
	{
		get
		{
			if (IsMoving)
			{
				return ATSpeed_Base + ATSpeed_Base * ((ATSpeed_Bei + ATSpeed_Tmp + (float)ST_MV_ATS + (float)(Temple_ATS * TempleMG.TempleList.Count) + CP1_ATS * (float)ACT.GetCP_CT() + CLass_ATS * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompATS() + GetMoveSpeedToAttackSpeed + JYC_ATS + BE_ZQ_ATS * (float)BE_ZQ_Count + BE_SPC_ATS * (float)BE_SPC_Count + BE_HH_ATS * (float)BE_HH_Count + BE_SK_ATS * (float)BE_SK_Count + BE_BS_ATS * (float)BE_BS_Count + BloodLost_Number) / 100f);
			}
			return ATSpeed_Base + ATSpeed_Base * ((ATSpeed_Bei + ATSpeed_Tmp + (float)ST_NoMV_ATS + (float)(Temple_ATS * TempleMG.TempleList.Count) + CP1_ATS * (float)ACT.GetCP_CT() + CLass_ATS * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompATS() + GetMoveSpeedToAttackSpeed + JYC_ATS + BE_ZQ_ATS * (float)BE_ZQ_Count + BE_SPC_ATS * (float)BE_SPC_Count + BE_HH_ATS * (float)BE_HH_Count + BE_SK_ATS * (float)BE_SK_Count + BE_BS_ATS * (float)BE_BS_Count + BloodLost_Number) / 100f);
		}
	}

	[HideInInspector]
	public float SkillSpeed_Max => 1f + (ATSpeed_Last - ATSpeed_Base) / 2f;

	[HideInInspector]
	public float MVSpeed_Max => MVSpeed_Base + MVSpeed_Base * ((MVSpeed_Bei + MVSpeed_Tmp + (float)(Temple_MVS * TempleMG.TempleList.Count) + CP1_MVS * (float)ACT.GetCP_CT() + CLass_MVS * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompMVS() + BE_ZQ_MVS * (float)BE_ZQ_Count + BE_SPC_MVS * (float)BE_SPC_Count + BE_HH_MVS * (float)BE_HH_Count + BE_SK_MVS * (float)BE_SK_Count + BE_BS_MVS * (float)BE_BS_Count + BloodLost_Number) / 100f);

	[HideInInspector]
	public float AntiSlow_Max
	{
		get
		{
			if (AntiSlow <= 90f)
			{
				return AntiSlow;
			}
			return 90f;
		}
	}

	[HideInInspector]
	public float CoolDown_Max
	{
		get
		{
			if (CoolDown + CoolDown_Tmp + (float)ACT.GetEveryCompCD() <= (float)(70 + Top_CD))
			{
				return CoolDown + CoolDown_Tmp + (float)ACT.GetEveryCompCD();
			}
			return 70 + Top_CD;
		}
	}

	[HideInInspector]
	public float GeDang_Max
	{
		get
		{
			if (IsMoving)
			{
				if (GeDang + GeDang_Tmp + (float)ST_MV_GD + EMC_GD + (float)ACT.GetEveryCompGD() <= (float)(70 + Top_GD))
				{
					return GeDang + GeDang_Tmp + (float)ST_MV_GD + EMC_GD + (float)ACT.GetEveryCompGD();
				}
				return 70 + Top_GD;
			}
			if (GeDang + GeDang_Tmp + EMC_GD + (float)ACT.GetEveryCompGD() <= (float)(70 + Top_GD))
			{
				return GeDang + GeDang_Tmp + EMC_GD + (float)ACT.GetEveryCompGD();
			}
			return 70 + Top_GD;
		}
	}

	[HideInInspector]
	public float BJrate_Last => BJrate + BJrate_Tmp + (float)ACT.GetEveryCompBJR() + BE_ZQ_BJR * (float)BE_ZQ_Count + BE_SPC_BJR * (float)BE_SPC_Count + BE_HH_BJR * (float)BE_HH_Count;

	[HideInInspector]
	public float BJDamage_Last
	{
		get
		{
			float num = BJDamage + BJDamage_Tmp + (float)ACT.GetEveryCompBJD() + GetElementPenetrationToCritDamage + JYC_BJD + BE_ZQ_BJD * (float)BE_ZQ_Count + BE_SPC_BJD * (float)BE_SPC_Count + BE_HH_BJD * (float)BE_HH_Count;
			if (NoDot_BJD > 0)
			{
				num += (float)NoDot_BJD;
			}
			if (Z_BJR_BJD && BJrate_Last > 100f)
			{
				return num + BJrate_Last - 100f;
			}
			return num;
		}
	}

	[HideInInspector]
	public float JYrate_Last => JYrate + JYrate_Tmp;

	[HideInInspector]
	public float ItemDrop_Rate_Last
	{
		get
		{
			int num = (ACT ? ACT.GetCP_CT() : 0);
			int num2 = (ACT ? ACT.GetCPClass_CT() : 0);
			return ItemDrop_Rate_buff_Tmp + ItemDrop_Rate_mijing_Tmp + ItemDrop_Rate + XJL_Count * XJL_DropMulti + CP1_DropR * (float)num + CLass_DropR * (float)num2 + GetST_EveryM_Drop + (float)ACT.GetEveryCompDrop();
		}
	}

	[HideInInspector]
	public float DOTcut_Max
	{
		get
		{
			if (DOTcut <= 80f)
			{
				return DOTcut;
			}
			return 80f;
		}
	}

	[HideInInspector]
	public float DOTcut_Last => Mathf.Clamp(DOTcut_Max + Runtime_DotTimeCut_Tmp + (float)ACT.GetEveryCompDotTimeCut(), 0f, 95f);

	[HideInInspector]
	public float Damage_Anti_Max
	{
		get
		{
			if (IsMoving)
			{
				if (IsChong)
				{
					if (Damage_Anti + Damage_Anti_Tmp + H_DMGAnti + (float)ST_Chong_Anti + CP1_DMG_Anti * (float)ACT.GetCP_CT() + CLass_DMG_Anti * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompDMG_Anti() + EMC_Anti + Orb_Bow_DMG_Anti * (float)Q_90 <= (float)(80 + Top_Anti))
					{
						return Damage_Anti + Damage_Anti_Tmp + H_DMGAnti + (float)ST_Chong_Anti + CP1_DMG_Anti * (float)ACT.GetCP_CT() + CLass_DMG_Anti * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompDMG_Anti() + EMC_Anti + Orb_Bow_DMG_Anti * (float)Q_90;
					}
					return 80 + Top_Anti;
				}
				if (Damage_Anti + Damage_Anti_Tmp + H_DMGAnti + CP1_DMG_Anti * (float)ACT.GetCP_CT() + CLass_DMG_Anti * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompDMG_Anti() + EMC_Anti + Orb_Bow_DMG_Anti * (float)Q_90 <= (float)(80 + Top_Anti))
				{
					return Damage_Anti + Damage_Anti_Tmp + H_DMGAnti + CP1_DMG_Anti * (float)ACT.GetCP_CT() + CLass_DMG_Anti * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompDMG_Anti() + EMC_Anti + Orb_Bow_DMG_Anti * (float)Q_90;
				}
				return 80 + Top_Anti;
			}
			if (Damage_Anti + Damage_Anti_Tmp + H_DMGAnti + (float)ST_NoMV_DMGAnti + CP1_DMG_Anti * (float)ACT.GetCP_CT() + CLass_DMG_Anti * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompDMG_Anti() + EMC_Anti + Orb_Bow_DMG_Anti * (float)Q_90 <= (float)(80 + Top_Anti))
			{
				return Damage_Anti + Damage_Anti_Tmp + H_DMGAnti + (float)ST_NoMV_DMGAnti + CP1_DMG_Anti * (float)ACT.GetCP_CT() + CLass_DMG_Anti * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompDMG_Anti() + EMC_Anti + Orb_Bow_DMG_Anti * (float)Q_90;
			}
			return 80 + Top_Anti;
		}
	}

	[HideInInspector]
	public float ORB_Damage_Last => ORB_Damage + Runtime_ORB_Damage_Tmp + (float)ACT.GetEveryCompORB_DMG() + CP1_ORB_DMG * (float)ACT.GetCP_CT() + CLass_ORB_DMG * (float)ACT.GetCPClass_CT() + ORB_FQ_DMG80 + ORB_FQ_DMG120 + Orb_Bow_DMG_ORB * (float)Q_90 + BE_ZQ_Orb_DMG * (float)BE_ZQ_Count + BE_SPC_Orb_DMG * (float)BE_SPC_Count + BE_HH_Orb_DMG * (float)BE_HH_Count + BE_SK_Orb_DMG * (float)BE_SK_Count + BE_BS_Orb_DMG * (float)BE_BS_Count;

	[HideInInspector]
	public float Damage_Max => Damage_Base + Damage_Base * (Damage_Bei_Last / 100f) + HealStat.Max * Z_Hmax_DMG / 100f + (HealStat.Max - HealStat.Cur) * Z_Huse_DMG / 100f + ManaStat.Max * Z_Mmax_DMG / 100f + ManaStat.Cur * Z_Mcur_DMG / 100f + (ManaStat.Max - ManaStat.Cur) * Z_Muse_DMG / 100f + EMC_DMG + AT_UseHeal1_DMG + AT_UseHeal2_DMG + Money_DMG;

	[HideInInspector]
	public float Damage_Bei_Last
	{
		get
		{
			float num = Damage_Bei_Tmp + Damage_Bei + XJL_Count * XJL_DMG + (float)(Temple_DMG * TempleMG.TempleList.Count) + H_DMG + M_DMG + CP1_DMG * (float)ACT.GetCP_CT() + CLass_DMG * (float)ACT.GetCPClass_CT() + (float)ACT.GetEveryCompDMG() + JYC_DMG + BE_ZQ_DMG * (float)BE_ZQ_Count + BE_SPC_DMG * (float)BE_SPC_Count + BE_HH_DMG * (float)BE_HH_Count + BE_SK_DMG * (float)BE_SK_Count + BE_BS_DMG * (float)BE_BS_Count + BF_DMG_Last + BloodLost_Number + GetA_DMG_Mana + GetA_DMG_Hurt + GetST_EveryH_DMG + GetMoveSpeedToDamage;
			if (IsMoving)
			{
				if (IsChong)
				{
					return num + DMG_Single + (float)ST_MV_DMG + (float)ST_Chong_DMG;
				}
				return num + DMG_Single + (float)ST_MV_DMG;
			}
			return num + DMG_Single + (float)ST_NoMV_DMG;
		}
	}

	[HideInInspector]
	public float Damage_Last
	{
		get
		{
			if (Damage_Cut > 80f)
			{
				return Damage_Max * 0.2f;
			}
			return Damage_Max - Damage_Max * (Damage_Cut / 100f);
		}
	}

	[HideInInspector]
	public float FireDamage => Damage_Last + Damage_Last * ((FireDamage_Bei_Last + FireDamageXi) / 100f);

	[HideInInspector]
	public float FrozenDamage => Damage_Last + Damage_Last * ((FrozenDamage_Bei_Last + FrozenDamageXi) / 100f);

	[HideInInspector]
	public float ThunderDamage => Damage_Last + Damage_Last * ((ThunderDamage_Bei_Last + ThunderDamageXi) / 100f);

	[HideInInspector]
	public float PoisonDamage => Damage_Last + Damage_Last * ((PoisonDamage_Bei_Last + PoisonDamageXi) / 100f);

	[HideInInspector]
	public float PhysicsDamage => Damage_Last + Damage_Last * ((PhysicsDamage_Bei_Last + PhysicsDamageXi) / 100f);

	[HideInInspector]
	public float ShadowDamage => Damage_Last + Damage_Last * ((ShadowDamage_Bei_Last + ShadowDamageXi) / 100f);

	[HideInInspector]
	public float FireDamage_Bei_Last
	{
		get
		{
			_ = IsMoving;
			return FireDamage_Bei + FireDamage_Bei_Tmp + CP1_DMG0 * (float)ACT.GetCP_CT() + CLass_DMG0 * (float)ACT.GetCPClass_CT() + GetElementDamageConversion(0);
		}
	}

	[HideInInspector]
	public float FrozenDamage_Bei_Last
	{
		get
		{
			_ = IsMoving;
			return FrozenDamage_Bei + FrozenDamage_Bei_Tmp + CP1_DMG1 * (float)ACT.GetCP_CT() + CLass_DMG1 * (float)ACT.GetCPClass_CT() + GetElementDamageConversion(1);
		}
	}

	[HideInInspector]
	public float ThunderDamage_Bei_Last
	{
		get
		{
			_ = IsMoving;
			return ThunderDamage_Bei + ThunderDamage_Bei_Tmp + CP1_DMG2 * (float)ACT.GetCP_CT() + CLass_DMG2 * (float)ACT.GetCPClass_CT() + GetElementDamageConversion(2);
		}
	}

	[HideInInspector]
	public float PoisonDamage_Bei_Last
	{
		get
		{
			_ = IsMoving;
			return PoisonDamage_Bei + PoisonDamage_Bei_Tmp + CP1_DMG3 * (float)ACT.GetCP_CT() + CLass_DMG3 * (float)ACT.GetCPClass_CT() + GetElementDamageConversion(3);
		}
	}

	[HideInInspector]
	public float PhysicsDamage_Bei_Last
	{
		get
		{
			_ = IsMoving;
			return PhysicsDamage_Bei + PhysicsDamage_Bei_Tmp + CP1_DMG4 * (float)ACT.GetCP_CT() + CLass_DMG4 * (float)ACT.GetCPClass_CT() + GetElementDamageConversion(4);
		}
	}

	[HideInInspector]
	public float ShadowDamage_Bei_Last
	{
		get
		{
			_ = IsMoving;
			return ShadowDamage_Bei + ShadowDamage_Bei_Tmp + CP1_DMG5 * (float)ACT.GetCP_CT() + CLass_DMG5 * (float)ACT.GetCPClass_CT() + GetElementDamageConversion(5);
		}
	}

	[HideInInspector]
	public float FireChuan_Last => FireChuan + FireChuan_Tmp + AllChuan + (float)ACT.GetEveryCompChuan() + CP1_Chuan0 * (float)ACT.GetCP_CT() + CLass_Chuan0 * (float)ACT.GetCPClass_CT();

	[HideInInspector]
	public float FrozenChuan_Last => FrozenChuan + FrozenChuan_Tmp + AllChuan + (float)ACT.GetEveryCompChuan() + CP1_Chuan1 * (float)ACT.GetCP_CT() + CLass_Chuan1 * (float)ACT.GetCPClass_CT();

	[HideInInspector]
	public float ThunderChuan_Last => ThunderChuan + ThunderChuan_Tmp + AllChuan + (float)ACT.GetEveryCompChuan() + CP1_Chuan2 * (float)ACT.GetCP_CT() + CLass_Chuan2 * (float)ACT.GetCPClass_CT();

	[HideInInspector]
	public float PoisonChuan_Last => PoisonChuan + PoisonChuan_Tmp + AllChuan + (float)ACT.GetEveryCompChuan() + CP1_Chuan3 * (float)ACT.GetCP_CT() + CLass_Chuan3 * (float)ACT.GetCPClass_CT();

	[HideInInspector]
	public float PhysicsChuan_Last => PhysicsChuan + PhysicsChuan_Tmp + AllChuan + (float)ACT.GetEveryCompChuan() + CP1_Chuan4 * (float)ACT.GetCP_CT() + CLass_Chuan4 * (float)ACT.GetCPClass_CT();

	[HideInInspector]
	public float ShadowChuan_Last => ShadowChuan + ShadowChuan_Tmp + AllChuan + (float)ACT.GetEveryCompChuan() + CP1_Chuan5 * (float)ACT.GetCP_CT() + CLass_Chuan5 * (float)ACT.GetCPClass_CT();

	[HideInInspector]
	public float FireAnti_Last => FireAnti + FireAnti_Tmp + AllAnti + (float)ACT.GetEveryCompAllAnti();

	[HideInInspector]
	public float FrozenAnti_Last => FrozenAnti + FrozenAnti_Tmp + AllAnti + (float)ACT.GetEveryCompAllAnti();

	[HideInInspector]
	public float ThunderAnti_Last => ThunderAnti + ThunderAnti_Tmp + AllAnti + (float)ACT.GetEveryCompAllAnti();

	[HideInInspector]
	public float PoisonAnti_Last => PoisonAnti + PoisonAnti_Tmp + AllAnti + (float)ACT.GetEveryCompAllAnti();

	[HideInInspector]
	public float PhysicsAnti_Last => PhysicsAnti + PhysicsAnti_Tmp + AllAnti + (float)ACT.GetEveryCompAllAnti();

	[HideInInspector]
	public float ShadowAnti_Last => ShadowAnti + ShadowAnti_Tmp + AllAnti + (float)ACT.GetEveryCompAllAnti();

	[HideInInspector]
	public float C_Health_Last => C_Health + C_Health_Tmp + CP1_CP_Heal * (float)ACT.GetCP_CT() + CLass_CP_Heal * (float)ACT.GetCPClass_CT() + BE_ZQ_CP_Heal * (float)BE_ZQ_Count + BE_SPC_CP_Heal * (float)BE_SPC_Count + BE_HH_CP_Heal * (float)BE_HH_Count + BE_SK_CP_Heal * (float)BE_SK_Count + BE_BS_CP_Heal * (float)BE_BS_Count;

	[HideInInspector]
	public float C_Damage_Last => C_Damage + C_Damage_Tmp + CP1_CP_DMG * (float)ACT.GetCP_CT() + CLass_CP_DMG * (float)ACT.GetCPClass_CT() + GetCooldownToCompanionDamage + GetAttackSpeedToCompanionDamage + (float)(XJ_Count_CP_DMG * Q_71) + BE_ZQ_CP_DMG * (float)BE_ZQ_Count + BE_SPC_CP_DMG * (float)BE_SPC_Count + BE_HH_CP_DMG * (float)BE_HH_Count + BE_SK_CP_DMG * (float)BE_SK_Count + BE_BS_CP_DMG * (float)BE_BS_Count;

	[HideInInspector]
	public float C_ATSpeed_Last => C_ATSpeed + C_ATSpeed_Tmp + CP1_CP_ATS * (float)ACT.GetCP_CT() + CLass_CP_ATS * (float)ACT.GetCPClass_CT() + BE_ZQ_CP_ATS * (float)BE_ZQ_Count + BE_SPC_CP_ATS * (float)BE_SPC_Count + BE_HH_CP_ATS * (float)BE_HH_Count + BE_SK_CP_ATS * (float)BE_SK_Count + BE_BS_CP_ATS * (float)BE_BS_Count;

	[HideInInspector]
	public float C_MVSpeed_Last => C_MVSpeed + C_MVSpeed_Tmp + BE_ZQ_CP_MVS * (float)BE_ZQ_Count + BE_SPC_CP_MVS * (float)BE_SPC_Count + BE_HH_CP_MVS * (float)BE_HH_Count;

	[HideInInspector]
	public float C_AllAnti_Last => C_AllAnti + C_AllAnti_Tmp + CP1_CP_AllAnti * (float)ACT.GetCP_CT() + CLass_CP_AllAnti * (float)ACT.GetCPClass_CT() + BE_ZQ_CP_Anti * (float)BE_ZQ_Count + BE_SPC_CP_Anti * (float)BE_SPC_Count + BE_HH_CP_Anti * (float)BE_HH_Count + BE_SK_CP_Anti * (float)BE_SK_Count + BE_BS_CP_Anti * (float)BE_BS_Count;

	[HideInInspector]
	public float FireCut => FireAnti_Last / (100f + FireAnti_Last) * 100f;

	[HideInInspector]
	public float FrozenCut => FrozenAnti_Last / (100f + FrozenAnti_Last) * 100f;

	[HideInInspector]
	public float ThunderCut => ThunderAnti_Last / (100f + ThunderAnti_Last) * 100f;

	[HideInInspector]
	public float PoisonCut => PoisonAnti_Last / (100f + PoisonAnti_Last) * 100f;

	[HideInInspector]
	public float PhysicsCut => PhysicsAnti_Last / (100f + PhysicsAnti_Last) * 100f;

	[HideInInspector]
	public float ShadowCut => ShadowAnti_Last / (100f + ShadowAnti_Last) * 100f;

	public float H_DMG
	{
		get
		{
			if (HealStat.Cur < HealStat.Max * 0.2f)
			{
				return LowH_DMG20 + LowH_DMG50;
			}
			if (HealStat.Cur < HealStat.Max * 0.5f)
			{
				return LowH_DMG50;
			}
			if (HealStat.Cur + 1f > HealStat.Max)
			{
				return HighH_DMG90 + HighH_DMG100;
			}
			if (HealStat.Cur > HealStat.Max * 0.9f)
			{
				return HighH_DMG90;
			}
			return 0f;
		}
	}

	public float H_hurtR
	{
		get
		{
			if (HealStat.Cur < HealStat.Max * 0.2f)
			{
				return LowH_HurtR20;
			}
			if (HealStat.Cur + 1f > HealStat.Max)
			{
				return HighH_HurtR100;
			}
			return 0f;
		}
	}

	public float H_DMGAnti
	{
		get
		{
			if (HealStat.Cur < HealStat.Max * 0.2f)
			{
				return LowH_DMGAnti20 + LowH_DMGAnti50;
			}
			if (HealStat.Cur < HealStat.Max * 0.5f)
			{
				return LowH_DMGAnti50;
			}
			return 0f;
		}
	}

	public float M_DMG
	{
		get
		{
			if (ManaStat.Cur < ManaStat.Max * 0.2f)
			{
				return LowM_DMG20 + LowM_DMG50;
			}
			if (ManaStat.Cur < ManaStat.Max * 0.5f)
			{
				return LowM_DMG50;
			}
			if (ManaStat.Cur + 1f > ManaStat.Max)
			{
				return HighM_DMG90 + HighM_DMG100;
			}
			if (ManaStat.Cur > ManaStat.Max * 0.9f)
			{
				return HighM_DMG90;
			}
			return 0f;
		}
	}

	public float M_hurtR
	{
		get
		{
			if (ManaStat.Cur < ManaStat.Max * 0.2f)
			{
				return LowM_HurtR20;
			}
			if (ManaStat.Cur + 1f > ManaStat.Max)
			{
				return HighM_HurtR100;
			}
			return 0f;
		}
	}

	private float HealthPercentForConversion => 100f + Health_Bei_Last;

	private float ManaPercentForConversion => 100f + Mana_Bei_Last;

	private float AttackSpeedPercentForConversion
	{
		get
		{
			int num = ((TempleMG != null) ? TempleMG.TempleList.Count : 0);
			int num2 = (ACT ? ACT.GetCP_CT() : 0);
			int num3 = (ACT ? ACT.GetCPClass_CT() : 0);
			float num4 = ATSpeed_Bei + ATSpeed_Tmp + (float)(Temple_ATS * num) + CP1_ATS * (float)num2 + CLass_ATS * (float)num3 + GetMoveSpeedToAttackSpeed + JYC_ATS + BE_ZQ_ATS * (float)BE_ZQ_Count + BE_SPC_ATS * (float)BE_SPC_Count + BE_HH_ATS * (float)BE_HH_Count + BE_SK_ATS * (float)BE_SK_Count + BE_BS_ATS * (float)BE_BS_Count + BloodLost_Number;
			if (IsMoving)
			{
				return num4 + (float)ST_MV_ATS;
			}
			return num4 + (float)ST_NoMV_ATS;
		}
	}

	private float MoveSpeedPercentForConversion
	{
		get
		{
			int num = ((TempleMG != null) ? TempleMG.TempleList.Count : 0);
			int num2 = (ACT ? ACT.GetCP_CT() : 0);
			int num3 = (ACT ? ACT.GetCPClass_CT() : 0);
			return MVSpeed_Bei + MVSpeed_Tmp + (float)(Temple_MVS * num) + CP1_MVS * (float)num2 + CLass_MVS * (float)num3 + BE_ZQ_MVS * (float)BE_ZQ_Count + BE_SPC_MVS * (float)BE_SPC_Count + BE_HH_MVS * (float)BE_HH_Count + BE_SK_MVS * (float)BE_SK_Count + BE_BS_MVS * (float)BE_BS_Count + BloodLost_Number;
		}
	}

	private float GetMoveSpeedToDamage
	{
		get
		{
			if (Z_MVS_DMG != 0f)
			{
				return MoveSpeedPercentForConversion * Z_MVS_DMG / 100f;
			}
			return 0f;
		}
	}

	private float GetMoveSpeedToAttackSpeed
	{
		get
		{
			if (Z_MVS_ATS != 0f)
			{
				return MoveSpeedPercentForConversion * Z_MVS_ATS / 100f;
			}
			return 0f;
		}
	}

	private float GetCooldownToCompanionDamage
	{
		get
		{
			if (Z_CD_CP_DMG != 0f)
			{
				return CoolDown_Max * Z_CD_CP_DMG / 100f;
			}
			return 0f;
		}
	}

	private float GetAttackSpeedToCompanionDamage
	{
		get
		{
			if (Z_ATS_CP_DMG != 0f)
			{
				return AttackSpeedPercentForConversion * Z_ATS_CP_DMG / 100f;
			}
			return 0f;
		}
	}

	private float GetElementPenetrationToCritDamage => FireChuan_Last * (float)Z_Chuan0_BJD / 100f + FrozenChuan_Last * (float)Z_Chuan1_BJD / 100f + ThunderChuan_Last * (float)Z_Chuan2_BJD / 100f + PoisonChuan_Last * (float)Z_Chuan3_BJD / 100f + PhysicsChuan_Last * (float)Z_Chuan4_BJD / 100f + ShadowChuan_Last * (float)Z_Chuan5_BJD / 100f;

	public float GetST_EveryH_DMG => (HealStat.Max - HealStat.Cur) / HealStat.Max * 100f * ST_EveryH_DMG;

	public float GetST_EveryM_Drop => (ManaStat.Max - ManaStat.Cur) / ManaStat.Max * 100f * ST_EveryM_Drop;

	public float GetA_DMG_Mana
	{
		get
		{
			if (AB_DMG_Mana)
			{
				return 30f;
			}
			return 0f;
		}
	}

	public float GetB_DMG_Mana
	{
		get
		{
			if (AB_DMG_Mana)
			{
				return 3f;
			}
			return 1f;
		}
	}

	public float GetA_DMG_Hurt
	{
		get
		{
			if (AB_DMG_Hurt)
			{
				return 40f;
			}
			return 0f;
		}
	}

	public float GetB_DMG_Hurt
	{
		get
		{
			if (AB_DMG_Hurt)
			{
				return 20f;
			}
			return 0f;
		}
	}

	public float GetA_Dot_DMG
	{
		get
		{
			if (AB_Dot_DMG)
			{
				return 3f + (Runtime_DotDamage_Tmp + (float)ACT.GetEveryCompDot_DMG()) / 100f;
			}
			return 1f + (Runtime_DotDamage_Tmp + (float)ACT.GetEveryCompDot_DMG()) / 100f;
		}
	}

	public float GetB_Dot_DMG
	{
		get
		{
			if (AB_Dot_DMG)
			{
				return 0.7f;
			}
			return 1f;
		}
	}

	public float GetBoGD
	{
		get
		{
			if (NoGD)
			{
				return 500f;
			}
			return 0f;
		}
	}

	public float DMG_Single => DMG_1 * (float)Q_1 + DMG_2 * (float)Q_2 + DMG_3 * (float)Q_3 + DMG_4 * (float)Q_4 + DMG_5 * (float)Q_5 + DMG_6 * (float)Q_6 + DMG_7 * (float)Q_7 + DMG_8 * (float)Q_8 + DMG_9 * (float)Q_9 + DMG_10 * (float)Q_10 + DMG_11 * (float)Q_11 + DMG_12 * (float)Q_12 + DMG_13 * (float)Q_13 + DMG_14 * (float)Q_14 + DMG_15 * (float)Q_15 + DMG_16 * (float)Q_16 + DMG_17 * (float)Q_17 + DMG_18 * (float)Q_18 + DMG_19 * (float)Q_19 + DMG_20 * (float)Q_20 + DMG_30 * (float)Q_30 + DMG_40 * (float)Q_40 + DMG_41 * (float)Q_41 + DMG_42 * (float)Q_42 + DMG_43 * (float)Q_43 + DMG_44 * (float)Q_44 + DMG_45 * (float)Q_45 + DMG_51 * (float)Q_51 + DMG_52 * (float)Q_52 + DMG_53 * (float)Q_53 + DMG_54 * (float)Q_54 + DMG_55 * (float)Q_55 + DMG_56 * (float)Q_56 + DMG_70 * (float)Q_70 + DMG_71 * (float)Q_71 + DMG_72 * (float)Q_72 + DMG_73 * (float)Q_73 + DMG_74 * (float)Q_74 + DMG_75 * (float)Q_75 + DMG_80 * (float)Q_80 + DMG_81 * (float)Q_81 + DMG_82 * (float)Q_82 + DMG_83 * (float)Q_83 + DMG_84 * (float)Q_84 + DMG_85 * (float)Q_85 + DMG_86 * (float)Q_86 + DMG_90 * (float)Q_90;

	[HideInInspector]
	public float EMC_DMG => Mathf.Min(EMC_DMG_20 * (float)NearEMC, 20f) + Mathf.Min(EMC_DMG_48 * (float)NearEMC, 48f);

	[HideInInspector]
	public float EMC_Anti => Mathf.Min(EMC_Anti_9 * (float)NearEMC, 9f);

	[HideInInspector]
	public float EMC_GD => Mathf.Min(EMC_GD_12 * (float)NearEMC, 12f);

	[HideInInspector]
	public float JYC_DMG => Mathf.Min(JYC_DMG_15 * (float)NearJYC, 15f);

	[HideInInspector]
	public float JYC_ATS => Mathf.Min(JYC_ATS_24 * (float)NearJYC, 24f);

	[HideInInspector]
	public float JYC_BJD => Mathf.Min(JYC_BJD_24 * (float)NearJYC, 16f);

	public bool IsBurnLifeEffectActive
	{
		get
		{
			if (HasBurnLifeStat() && (bool)HealStat && HealStat.Max > 0f)
			{
				return HealStat.Cur > HealStat.Max * 0.01f;
			}
			return false;
		}
	}

	public float ORB_FQ_Double
	{
		get
		{
			if (ORB_FQ_Count_Double)
			{
				return 2f;
			}
			return 1f;
		}
	}

	public float ORB_FQ_DMG80
	{
		get
		{
			if (Q_18 > 80)
			{
				return ORB_FQ_DMG80_Base;
			}
			return 0f;
		}
	}

	public float ORB_FQ_DMG120
	{
		get
		{
			if (Q_18 > 120)
			{
				return ORB_FQ_DMG120_Base;
			}
			return 0f;
		}
	}

	public float Orb_Universe_DMG_Last
	{
		get
		{
			if (ManaStat.Cur > ManaStat.Max - 1f)
			{
				return Orb_Universe_DMG_Base + (float)HighMana_DMG100_FQ;
			}
			return Orb_Universe_DMG_Base;
		}
	}

	public float TuT_PlayerAllLast => TuT_PlayerAll ? 1 : 2;

	public float BloodLost_Number
	{
		get
		{
			if (BloodLost && HealStat.Max > 0f)
			{
				return (HealStat.Max - HealStat.Cur) / HealStat.Max * 30f;
			}
			return 0f;
		}
	}

	public float AT_UseHeal1_DMG
	{
		get
		{
			if (AT_UseHeal1 && HealStat.Cur > HealStat.Max / 20f)
			{
				return 10f;
			}
			return 0f;
		}
	}

	public float AT_UseHeal2_DMG
	{
		get
		{
			if (AT_UseHeal2 && HealStat.Cur > HealStat.Max / 20f)
			{
				return 36f;
			}
			return 0f;
		}
	}

	public float Money_DMG
	{
		get
		{
			if (MoneyTO_DMG && SingletonMonoScope<InventoryManager>.HasInstance)
			{
				return (float)SingletonMonoScope<InventoryManager>.Instance.GlobalMoney * 1E-06f;
			}
			return 0f;
		}
	}

	[HideInInspector]
	public float Pick_PL_Max => Pick_PL_Base * Pick_PL_Percent / 100f;

	[HideInInspector]
	public float Pick_XJL_Max => Pick_XJL_Base * Pick_XJL_Percent / 100f;

	public void EnsurePlayerDotData()
	{
		if (DOT == null || DOT.Length != 6)
		{
			PlayerDotData[] dOT = DOT;
			DOT = new PlayerDotData[6];
			if (dOT != null)
			{
				int num = Mathf.Min(dOT.Length, 6);
				for (int i = 0; i < num; i++)
				{
					DOT[i] = dOT[i];
				}
			}
		}
		for (int j = 0; j < 6; j++)
		{
			if (DOT[j] == null)
			{
				DOT[j] = PlayerDotData.CreateDefault();
			}
		}
	}

	public PlayerDotData GetPlayerDotData(DamageType type)
	{
		EnsurePlayerDotData();
		return DOT[Mathf.Clamp(GiveInt(type), 0, 5)];
	}

	private static PlayerDotData ClonePlayerDotData(PlayerDotData data)
	{
		if (data != null)
		{
			return DataUtil.DeepClone(data);
		}
		return PlayerDotData.CreateDefault();
	}

	private static PlayerDotData[] CreatePlayerDotDataArray(PlayerSaveData data)
	{
		return new PlayerDotData[6]
		{
			ClonePlayerDotData(data?.Dot_Fire),
			ClonePlayerDotData(data?.Dot_Ice),
			ClonePlayerDotData(data?.Dot_TD),
			ClonePlayerDotData(data?.Dot_Du),
			ClonePlayerDotData(data?.Dot_Phy),
			ClonePlayerDotData(data?.Dot_SD)
		};
	}

	private float GetElementDamageConversion(int elementIndex)
	{
		return elementIndex switch
		{
			0 => HealthPercentForConversion * Z_Hmax_EL0 / 100f + ManaPercentForConversion * Z_Mmax_EL0 / 100f + CoolDown_Max * Z_CD_EL0 / 100f + FireAnti_Last * (float)Z_Anti0_EL0 / 100f + FireChuan_Last * (float)Z_Chuan0_EL0 / 100f + GeDang_Max * (float)Z_GD_EL0 / 100f + BJrate_Last * (float)Z_BJR_EL0 / 100f + Damage_Anti_Max * (float)Z_DMGCut_EL0 / 100f + ThroughRate * (float)Z_Thr_EL0 / 100f, 
			1 => HealthPercentForConversion * Z_Hmax_EL1 / 100f + ManaPercentForConversion * Z_Mmax_EL1 / 100f + CoolDown_Max * Z_CD_EL1 / 100f + FrozenAnti_Last * (float)Z_Anti0_EL1 / 100f + FrozenChuan_Last * (float)Z_Chuan0_EL1 / 100f + GeDang_Max * (float)Z_GD_EL1 / 100f + BJrate_Last * (float)Z_BJR_EL1 / 100f + Damage_Anti_Max * (float)Z_DMGCut_EL1 / 100f + ThroughRate * (float)Z_Thr_EL1 / 100f, 
			2 => HealthPercentForConversion * Z_Hmax_EL2 / 100f + ManaPercentForConversion * Z_Mmax_EL2 / 100f + CoolDown_Max * Z_CD_EL2 / 100f + ThunderAnti_Last * (float)Z_Anti0_EL2 / 100f + ThunderChuan_Last * (float)Z_Chuan0_EL2 / 100f + GeDang_Max * (float)Z_GD_EL2 / 100f + BJrate_Last * (float)Z_BJR_EL2 / 100f + Damage_Anti_Max * (float)Z_DMGCut_EL2 / 100f + ThroughRate * (float)Z_Thr_EL2 / 100f, 
			3 => HealthPercentForConversion * Z_Hmax_EL3 / 100f + ManaPercentForConversion * Z_Mmax_EL3 / 100f + CoolDown_Max * Z_CD_EL3 / 100f + PoisonAnti_Last * (float)Z_Anti0_EL3 / 100f + PoisonChuan_Last * (float)Z_Chuan0_EL3 / 100f + GeDang_Max * (float)Z_GD_EL3 / 100f + BJrate_Last * (float)Z_BJR_EL3 / 100f + Damage_Anti_Max * (float)Z_DMGCut_EL3 / 100f + ThroughRate * (float)Z_Thr_EL3 / 100f, 
			4 => HealthPercentForConversion * Z_Hmax_EL4 / 100f + ManaPercentForConversion * Z_Mmax_EL4 / 100f + CoolDown_Max * Z_CD_EL4 / 100f + PhysicsAnti_Last * (float)Z_Anti0_EL4 / 100f + PhysicsChuan_Last * (float)Z_Chuan0_EL4 / 100f + GeDang_Max * (float)Z_GD_EL4 / 100f + BJrate_Last * (float)Z_BJR_EL4 / 100f + Damage_Anti_Max * (float)Z_DMGCut_EL4 / 100f + ThroughRate * (float)Z_Thr_EL4 / 100f, 
			5 => HealthPercentForConversion * Z_Hmax_EL5 / 100f + ManaPercentForConversion * Z_Mmax_EL5 / 100f + CoolDown_Max * Z_CD_EL5 / 100f + ShadowAnti_Last * (float)Z_Anti0_EL5 / 100f + ShadowChuan_Last * (float)Z_Chuan0_EL5 / 100f + GeDang_Max * (float)Z_GD_EL5 / 100f + BJrate_Last * (float)Z_BJR_EL5 / 100f + Damage_Anti_Max * (float)Z_DMGCut_EL5 / 100f + ThroughRate * (float)Z_Thr_EL5 / 100f, 
			_ => 0f, 
		};
	}

	public int GetPrcCut(DamageType type)
	{
		return type switch
		{
			DamageType.fire => PrcCut0, 
			DamageType.frozen => PrcCut1, 
			DamageType.thunder => PrcCut2, 
			DamageType.poison => PrcCut3, 
			DamageType.physics => PrcCut4, 
			DamageType.shadow => PrcCut5, 
			_ => 0, 
		};
	}

	public int GetPrcCut5P(DamageType type)
	{
		return type switch
		{
			DamageType.fire => PrcCut5P0, 
			DamageType.frozen => PrcCut5P1, 
			DamageType.thunder => PrcCut5P2, 
			DamageType.poison => PrcCut5P3, 
			DamageType.physics => PrcCut5P4, 
			DamageType.shadow => PrcCut5P5, 
			_ => 0, 
		};
	}

	public int GetPrcCut3P(DamageType type)
	{
		return type switch
		{
			DamageType.fire => PrcCut3P0, 
			DamageType.frozen => PrcCut3P1, 
			DamageType.thunder => PrcCut3P2, 
			DamageType.poison => PrcCut3P3, 
			DamageType.physics => PrcCut3P4, 
			DamageType.shadow => PrcCut3P5, 
			_ => 0, 
		};
	}

	public float Dis_OutLast(Transform trans)
	{
		if (Vector2.Distance(base.transform.position, trans.position) * 3f < 30f)
		{
			return Vector2.Distance(base.transform.position, trans.position) * 3f;
		}
		return 20f;
	}

	public int GetEquippedSetCount(int setId)
	{
		if (EquippedSetCounts == null || setId <= 0)
		{
			return 0;
		}
		if (!EquippedSetCounts.TryGetValue(setId, out var value))
		{
			return 0;
		}
		return value;
	}

	public int AddEquippedSetCount(int setId)
	{
		if (setId <= 0)
		{
			return 0;
		}
		if (EquippedSetCounts == null)
		{
			EquippedSetCounts = new Dictionary<int, int>();
		}
		int equippedSetCount = GetEquippedSetCount(setId);
		int value = Mathf.Clamp(equippedSetCount + 1, 0, 4);
		EquippedSetCounts[setId] = value;
		return equippedSetCount;
	}

	public int RemoveEquippedSetCount(int setId)
	{
		if (setId <= 0)
		{
			return 0;
		}
		if (EquippedSetCounts == null)
		{
			EquippedSetCounts = new Dictionary<int, int>();
		}
		int equippedSetCount = GetEquippedSetCount(setId);
		int num = Mathf.Clamp(equippedSetCount - 1, 0, 4);
		if (num > 0)
		{
			EquippedSetCounts[setId] = num;
			return equippedSetCount;
		}
		EquippedSetCounts.Remove(setId);
		return equippedSetCount;
	}

	public void ClearEquippedSetCounts()
	{
		if (EquippedSetCounts == null)
		{
			EquippedSetCounts = new Dictionary<int, int>();
		}
		else
		{
			EquippedSetCounts.Clear();
		}
		BuffRuntime?.ClearSetLayerBuffs();
	}

	private void LoadEquippedSetCounts(Dictionary<int, int> source)
	{
		ClearEquippedSetCounts();
		if (source == null)
		{
			return;
		}
		foreach (KeyValuePair<int, int> item in source)
		{
			if (item.Key > 0 && item.Value > 0)
			{
				EquippedSetCounts[item.Key] = Mathf.Clamp(item.Value, 1, 4);
			}
		}
	}

	private Dictionary<int, int> ExportEquippedSetCounts()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		if (EquippedSetCounts == null)
		{
			return dictionary;
		}
		foreach (KeyValuePair<int, int> equippedSetCount in EquippedSetCounts)
		{
			if (equippedSetCount.Key > 0 && equippedSetCount.Value > 0)
			{
				dictionary[equippedSetCount.Key] = Mathf.Clamp(equippedSetCount.Value, 1, 4);
			}
		}
		return dictionary;
	}

	public void InitFromSaveData(PlayerSaveData data)
	{
		SaveData = DataUtil.DeepClone(data);
		ApplySaveData(SaveData);
	}

	public void ApplySaveData(PlayerSaveData data)
	{
		if (data == null)
		{
			data = PlayerSaveData.CreateDefault();
		}
		if ((bool)xjl)
		{
			xjl.ClearAllXJL();
		}
		if ((bool)BuffRuntime)
		{
			BuffRuntime.ClearAllRuntimeBuffs();
		}
		ResetRuntimeTempStats();
		LoadEquippedSetCounts(data.EquippedSetCounts);
		PlayerName = data.PlayerName;
		PLType = data.PlayerType;
		AutoAttackEnabled = Singleton<SettingDataManager>.Instance.GetGame().auto_attack;
		AutoJH = data.AutoJH;
		AutoDrinkH = data.AutoDrinkH;
		AutoDrinkM = data.AutoDrinkM;
		CompCount = data.CompCount;
		Level = data.Level;
		Health = data.Health;
		Health_Bei = data.Health_Bei;
		Health_R_Base = data.Health_R_Base;
		Health_Percent = data.Health_Percent;
		Mana = data.Mana;
		Mana_Bei = data.Mana_Bei;
		Mana_R_Base = data.Mana_R_Base;
		Mana_Percent = data.Mana_Percent;
		Xp_Total = data.Xp_Total;
		Xp_CurrentLevel = data.Xp_CurrentLevel;
		DFLevel = Mathf.Max(1, data.DFLevel);
		DFXp_Total = data.DFXp_Total;
		DFXp_CurrentLevel = data.DFXp_CurrentLevel;
		Attack_R_health_Percent = data.Attack_R_health_Percent;
		Attack_R_health_Base = data.Attack_R_health_Base;
		Attack_R_mana_Percent = data.Attack_R_mana_Percent;
		Attack_R_mana_Base = data.Attack_R_mana_Base;
		ATSpeed_Bei = data.ATSpeed_Bei;
		MVSpeed_Bei = data.MVSpeed_Bei;
		AntiSlow = data.AntiSlow;
		CoolDown = data.CoolDown;
		GeDang = data.GeDang;
		BJrate = data.BJrate;
		BJDamage = data.BJDamage;
		JYrate = data.JYrate;
		ThroughRate = data.ThroughRate;
		ItemDrop_Rate = data.ItemDrop_Rate;
		DOTcut = data.DOTcut;
		Damage_Anti = data.Damage_Anti;
		FlySpeed = data.FlySpeed;
		ORB_Damage = data.ORB_Damage;
		Damage_Base = data.Damage_Base;
		Damage_Bei = data.Damage_Bei;
		FireDamageXi = data.FireDamageXi;
		FrozenDamageXi = data.FrozenDamageXi;
		ThunderDamageXi = data.ThunderDamageXi;
		PoisonDamageXi = data.PoisonDamageXi;
		PhysicsDamageXi = data.PhysicsDamageXi;
		ShadowDamageXi = data.ShadowDamageXi;
		FireDamage_Bei = data.FireDamage_Bei;
		FrozenDamage_Bei = data.FrozenDamage_Bei;
		ThunderDamage_Bei = data.ThunderDamage_Bei;
		PoisonDamage_Bei = data.PoisonDamage_Bei;
		PhysicsDamage_Bei = data.PhysicsDamage_Bei;
		ShadowDamage_Bei = data.ShadowDamage_Bei;
		FireChuan = data.FireChuan;
		FrozenChuan = data.FrozenChuan;
		ThunderChuan = data.ThunderChuan;
		PoisonChuan = data.PoisonChuan;
		PhysicsChuan = data.PhysicsChuan;
		ShadowChuan = data.ShadowChuan;
		FireAnti = data.FireAnti;
		FrozenAnti = data.FrozenAnti;
		ThunderAnti = data.ThunderAnti;
		PoisonAnti = data.PoisonAnti;
		PhysicsAnti = data.PhysicsAnti;
		ShadowAnti = data.ShadowAnti;
		C_Health = data.C_Health;
		C_Damage = data.C_Damage;
		C_ATSpeed = data.C_ATSpeed;
		C_MVSpeed = data.C_MVSpeed;
		C_AllAnti = data.C_AllAnti;
		Pick_PL_Base = data.Pick_PL_Base;
		Pick_PL_Bei = data.Pick_PL_Bei;
		Pick_XJL_Base = data.Pick_XJL_Base;
		Pick_XJL_Bei = data.Pick_XJL_Bei;
		XJL_SellPrice = data.XJL_SellPrice;
		XJL_DMG = data.XJL_DMG;
		XJL_UseSKTime = data.XJL_UseSKTime;
		Reforge_Inc = data.Reforge_Inc;
		QH_Inc = data.QH_Inc;
		HH_Inc = ((data.HH_Inc <= 0) ? 10 : data.HH_Inc);
		SK_Inc = data.SK_Inc;
		QH_Price = data.QH_Price;
		QH_Bei = data.QH_Bei;
		ManaXH = data.ManaXH;
		BJD_Anti = data.BJD_Anti;
		AllChuan = data.AllChuan;
		AllAnti = data.AllAnti;
		BuffT_Temple = data.BuffT_Temple;
		BuffT_Drink = data.BuffT_Drink;
		WPSPC_DMG = data.WPSPC_DMG;
		WPSPC_Rate = data.WPSPC_Rate;
		JYBoss_DMG = data.JYBoss_DMG;
		JYBoss_Anti = data.JYBoss_Anti;
		DMG_R_H = data.DMG_R_H;
		DMG_R_M = data.DMG_R_M;
		BS_Add = data.BS_Add;
		BS_Multi = data.BS_Multi;
		Temple_DMG = data.Temple_DMG;
		Temple_ATS = data.Temple_ATS;
		Temple_MVS = data.Temple_MVS;
		Temple_HealPrc = data.Temple_HealPrc;
		Temple_BS = data.Temple_BS;
		BE_ZQ_DMG = data.BE_ZQ_DMG;
		BE_ZQ_ATS = data.BE_ZQ_ATS;
		BE_ZQ_MVS = data.BE_ZQ_MVS;
		BE_ZQ_BJR = data.BE_ZQ_BJR;
		BE_ZQ_BJD = data.BE_ZQ_BJD;
		BE_ZQ_Heal = data.BE_ZQ_Heal;
		BE_ZQ_Mana = data.BE_ZQ_Mana;
		BE_ZQ_CP_Heal = data.BE_ZQ_CP_Heal;
		BE_ZQ_CP_DMG = data.BE_ZQ_CP_DMG;
		BE_ZQ_CP_ATS = data.BE_ZQ_CP_ATS;
		BE_ZQ_CP_MVS = data.BE_ZQ_CP_MVS;
		BE_ZQ_CP_Anti = data.BE_ZQ_CP_Anti;
		BE_ZQ_Dot = data.BE_ZQ_Dot;
		BE_ZQ_XJ_DMG = data.BE_ZQ_XJ_DMG;
		BE_ZQ_Orb_DMG = data.BE_ZQ_Orb_DMG;
		BE_SPC_DMG = data.BE_SPC_DMG;
		BE_SPC_ATS = data.BE_SPC_ATS;
		BE_SPC_MVS = data.BE_SPC_MVS;
		BE_SPC_BJR = data.BE_SPC_BJR;
		BE_SPC_BJD = data.BE_SPC_BJD;
		BE_SPC_Heal = data.BE_SPC_Heal;
		BE_SPC_Mana = data.BE_SPC_Mana;
		BE_SPC_CP_Heal = data.BE_SPC_CP_Heal;
		BE_SPC_CP_DMG = data.BE_SPC_CP_DMG;
		BE_SPC_CP_ATS = data.BE_SPC_CP_ATS;
		BE_SPC_CP_MVS = data.BE_SPC_CP_MVS;
		BE_SPC_CP_Anti = data.BE_SPC_CP_Anti;
		BE_SPC_Dot = data.BE_SPC_Dot;
		BE_SPC_XJ_DMG = data.BE_SPC_XJ_DMG;
		BE_SPC_Orb_DMG = data.BE_SPC_Orb_DMG;
		BE_HH_DMG = data.BE_HH_DMG;
		BE_HH_ATS = data.BE_HH_ATS;
		BE_HH_MVS = data.BE_HH_MVS;
		BE_HH_BJR = data.BE_HH_BJR;
		BE_HH_BJD = data.BE_HH_BJD;
		BE_HH_Heal = data.BE_HH_Heal;
		BE_HH_Mana = data.BE_HH_Mana;
		BE_HH_CP_Heal = data.BE_HH_CP_Heal;
		BE_HH_CP_DMG = data.BE_HH_CP_DMG;
		BE_HH_CP_ATS = data.BE_HH_CP_ATS;
		BE_HH_CP_MVS = data.BE_HH_CP_MVS;
		BE_HH_CP_Anti = data.BE_HH_CP_Anti;
		BE_HH_Dot = data.BE_HH_Dot;
		BE_HH_XJ_DMG = data.BE_HH_XJ_DMG;
		BE_HH_Orb_DMG = data.BE_HH_Orb_DMG;
		BE_SK_DMG = data.BE_SK_DMG;
		BE_SK_ATS = data.BE_SK_ATS;
		BE_SK_MVS = data.BE_SK_MVS;
		BE_SK_CP_Heal = data.BE_SK_CP_Heal;
		BE_SK_CP_DMG = data.BE_SK_CP_DMG;
		BE_SK_CP_ATS = data.BE_SK_CP_ATS;
		BE_SK_CP_Anti = data.BE_SK_CP_Anti;
		BE_SK_XJ_DMG = data.BE_SK_XJ_DMG;
		BE_SK_Orb_DMG = data.BE_SK_Orb_DMG;
		BE_SK_FQ_Count = data.BE_SK_FQ_Count;
		BE_BS_DMG = data.BE_BS_DMG;
		BE_BS_ATS = data.BE_BS_ATS;
		BE_BS_MVS = data.BE_BS_MVS;
		BE_BS_CP_Heal = data.BE_BS_CP_Heal;
		BE_BS_CP_DMG = data.BE_BS_CP_DMG;
		BE_BS_CP_ATS = data.BE_BS_CP_ATS;
		BE_BS_CP_Anti = data.BE_BS_CP_Anti;
		BE_BS_XJ_DMG = data.BE_BS_XJ_DMG;
		BE_BS_Orb_DMG = data.BE_BS_Orb_DMG;
		BE_BS_FQ_Count = data.BE_BS_FQ_Count;
		Crit_BoomEXP = data.Crit_BoomEXP;
		Crit_BoomDie_Rate = data.Crit_BoomDie_Rate;
		Crit_MS = data.Crit_MS;
		LowH_DMG20 = data.LowH_DMG20;
		LowH_DMG50 = data.LowH_DMG50;
		HighH_DMG90 = data.HighH_DMG90;
		HighH_DMG100 = data.HighH_DMG100;
		LowH_HurtR20 = data.LowH_HurtR20;
		HighH_HurtR100 = data.HighH_HurtR100;
		LowH_DMGAnti20 = data.LowH_DMGAnti20;
		LowH_DMGAnti50 = data.LowH_DMGAnti50;
		LowH_CritAnti10 = data.LowH_CritAnti10;
		LowM_DMG20 = data.LowM_DMG20;
		LowM_DMG50 = data.LowM_DMG50;
		HighM_DMG90 = data.HighM_DMG90;
		HighM_DMG100 = data.HighM_DMG100;
		LowM_HurtR20 = data.LowM_HurtR20;
		HighM_HurtR100 = data.HighM_HurtR100;
		ST_MV_DMG = data.ST_MV_DMG;
		ST_MV_ATS = data.ST_MV_ATS;
		ST_MV_GD = data.ST_MV_GD;
		ST_NoMV_DMG = data.ST_NoMV_DMG;
		ST_NoMV_ATS = data.ST_NoMV_ATS;
		ST_NoMV_DMGAnti = data.ST_NoMV_DMGAnti;
		ST_NoMV_HealPrc = data.ST_NoMV_HealPrc;
		ST_NoMV_ManaPrc = data.ST_NoMV_ManaPrc;
		ST_Chong_DMG = data.ST_Chong_DMG;
		ST_Chong_Anti = data.ST_Chong_Anti;
		EM_LowH_DMG20 = data.EM_LowH_DMG20;
		EM_LowH_DMG50 = data.EM_LowH_DMG50;
		EM_HighH_DMG60 = data.EM_HighH_DMG60;
		EM_HighH_DMG100 = data.EM_HighH_DMG100;
		EM_Heal_Crit = data.EM_Heal_Crit;
		CP1_DMG = data.CP1_DMG;
		CP1_ATS = data.CP1_ATS;
		CP1_MVS = data.CP1_MVS;
		CP1_Heal = data.CP1_Heal;
		CP1_Mana = data.CP1_Mana;
		CP1_DMG_Anti = data.CP1_DMG_Anti;
		CP1_DropR = data.CP1_DropR;
		CP1_ORB_DMG = data.CP1_ORB_DMG;
		CP1_DMG0 = data.CP1_DMG0;
		CP1_DMG1 = data.CP1_DMG1;
		CP1_DMG2 = data.CP1_DMG2;
		CP1_DMG3 = data.CP1_DMG3;
		CP1_DMG4 = data.CP1_DMG4;
		CP1_DMG5 = data.CP1_DMG5;
		CP1_Chuan0 = data.CP1_Chuan0;
		CP1_Chuan1 = data.CP1_Chuan1;
		CP1_Chuan2 = data.CP1_Chuan2;
		CP1_Chuan3 = data.CP1_Chuan3;
		CP1_Chuan4 = data.CP1_Chuan4;
		CP1_Chuan5 = data.CP1_Chuan5;
		CP1_CP_Heal = data.CP1_CP_Heal;
		CP1_CP_DMG = data.CP1_CP_DMG;
		CP1_CP_ATS = data.CP1_CP_ATS;
		CP1_CP_AllAnti = data.CP1_CP_AllAnti;
		CLass_DMG = data.CLass_DMG;
		CLass_ATS = data.CLass_ATS;
		CLass_MVS = data.CLass_MVS;
		CLass_Heal = data.CLass_Heal;
		CLass_Mana = data.CLass_Mana;
		CLass_DMG_Anti = data.CLass_DMG_Anti;
		CLass_DropR = data.CLass_DropR;
		CLass_ORB_DMG = data.CLass_ORB_DMG;
		CLass_DMG0 = data.CLass_DMG0;
		CLass_DMG1 = data.CLass_DMG1;
		CLass_DMG2 = data.CLass_DMG2;
		CLass_DMG3 = data.CLass_DMG3;
		CLass_DMG4 = data.CLass_DMG4;
		CLass_DMG5 = data.CLass_DMG5;
		CLass_Chuan0 = data.CLass_Chuan0;
		CLass_Chuan1 = data.CLass_Chuan1;
		CLass_Chuan2 = data.CLass_Chuan2;
		CLass_Chuan3 = data.CLass_Chuan3;
		CLass_Chuan4 = data.CLass_Chuan4;
		CLass_Chuan5 = data.CLass_Chuan5;
		CLass_CP_Heal = data.CLass_CP_Heal;
		CLass_CP_DMG = data.CLass_CP_DMG;
		CLass_CP_ATS = data.CLass_CP_ATS;
		CLass_CP_AllAnti = data.CLass_CP_AllAnti;
		Class_CP_DotDMG = data.Class_CP_DotDMG;
		XJ_DMG = data.XJ_DMG;
		XJ_Time = data.XJ_Time;
		TuT_Buff = data.TuT_Buff;
		TuT_Time = data.TuT_Time;
		TuT_PlayerAll = data.TuT_PlayerAll;
		Top_CD = data.Top_CD;
		Top_GD = data.Top_GD;
		Top_Anti = data.Top_Anti;
		Top_Cut_DMG = data.Top_Cut_DMG;
		Top_Cut_MVS = data.Top_Cut_MVS;
		Top_Cut_ATS = data.Top_Cut_ATS;
		AllDot_DMG = data.AllDot_DMG;
		AllDot_Time = data.AllDot_Time;
		AllDot_Layer = data.AllDot_Layer;
		AllDot_MV = data.AllDot_MV;
		AllDot_JY = data.AllDot_JY;
		DiffDotDMG = data.DiffDotDMG;
		DiffDebuff_DMG = data.DiffDebuff_DMG;
		Dot_MSAll = data.Dot_MSAll;
		DOT = CreatePlayerDotDataArray(data);
		EnsurePlayerDotData();
		DrinkPre_Heal = data.DrinkPre_Heal;
		DrinkPre_Mana = data.DrinkPre_Mana;
		DrinkPre_DMG = data.DrinkPre_DMG;
		Drink_CP = data.Drink_CP;
		Z_Hmax_DMG = data.Z_Hmax_DMG;
		Z_Huse_DMG = data.Z_Huse_DMG;
		Z_Mmax_DMG = data.Z_Mmax_DMG;
		Z_Mcur_DMG = data.Z_Mcur_DMG;
		Z_Muse_DMG = data.Z_Muse_DMG;
		Z_Hmax_EL0 = data.Z_Hmax_EL0;
		Z_Hmax_EL1 = data.Z_Hmax_EL1;
		Z_Hmax_EL2 = data.Z_Hmax_EL2;
		Z_Hmax_EL3 = data.Z_Hmax_EL3;
		Z_Hmax_EL4 = data.Z_Hmax_EL4;
		Z_Hmax_EL5 = data.Z_Hmax_EL5;
		Z_Mmax_EL0 = data.Z_Mmax_EL0;
		Z_Mmax_EL1 = data.Z_Mmax_EL1;
		Z_Mmax_EL2 = data.Z_Mmax_EL2;
		Z_Mmax_EL3 = data.Z_Mmax_EL3;
		Z_Mmax_EL4 = data.Z_Mmax_EL4;
		Z_Mmax_EL5 = data.Z_Mmax_EL5;
		Z_CD_EL0 = data.Z_CD_EL0;
		Z_CD_EL1 = data.Z_CD_EL1;
		Z_CD_EL2 = data.Z_CD_EL2;
		Z_CD_EL3 = data.Z_CD_EL3;
		Z_CD_EL4 = data.Z_CD_EL4;
		Z_CD_EL5 = data.Z_CD_EL5;
		Z_Anti0_EL0 = data.Z_Anti0_EL0;
		Z_Anti0_EL1 = data.Z_Anti0_EL1;
		Z_Anti0_EL2 = data.Z_Anti0_EL2;
		Z_Anti0_EL3 = data.Z_Anti0_EL3;
		Z_Anti0_EL4 = data.Z_Anti0_EL4;
		Z_Anti0_EL5 = data.Z_Anti0_EL5;
		Z_Chuan0_EL0 = data.Z_Chuan0_EL0;
		Z_Chuan0_EL1 = data.Z_Chuan0_EL1;
		Z_Chuan0_EL2 = data.Z_Chuan0_EL2;
		Z_Chuan0_EL3 = data.Z_Chuan0_EL3;
		Z_Chuan0_EL4 = data.Z_Chuan0_EL4;
		Z_Chuan0_EL5 = data.Z_Chuan0_EL5;
		Z_GD_EL0 = data.Z_GD_EL0;
		Z_GD_EL1 = data.Z_GD_EL1;
		Z_GD_EL2 = data.Z_GD_EL2;
		Z_GD_EL3 = data.Z_GD_EL3;
		Z_GD_EL4 = data.Z_GD_EL4;
		Z_GD_EL5 = data.Z_GD_EL5;
		Z_BJR_EL0 = data.Z_BJR_EL0;
		Z_BJR_EL1 = data.Z_BJR_EL1;
		Z_BJR_EL2 = data.Z_BJR_EL2;
		Z_BJR_EL3 = data.Z_BJR_EL3;
		Z_BJR_EL4 = data.Z_BJR_EL4;
		Z_BJR_EL5 = data.Z_BJR_EL5;
		Z_DMGCut_EL0 = data.Z_DMGCut_EL0;
		Z_DMGCut_EL1 = data.Z_DMGCut_EL1;
		Z_DMGCut_EL2 = data.Z_DMGCut_EL2;
		Z_DMGCut_EL3 = data.Z_DMGCut_EL3;
		Z_DMGCut_EL4 = data.Z_DMGCut_EL4;
		Z_DMGCut_EL5 = data.Z_DMGCut_EL5;
		Z_Thr_EL0 = data.Z_Thr_EL0;
		Z_Thr_EL1 = data.Z_Thr_EL1;
		Z_Thr_EL2 = data.Z_Thr_EL2;
		Z_Thr_EL3 = data.Z_Thr_EL3;
		Z_Thr_EL4 = data.Z_Thr_EL4;
		Z_Thr_EL5 = data.Z_Thr_EL5;
		Z_CD_CP_DMG = data.Z_CD_CP_DMG;
		Z_ATS_CP_DMG = data.Z_ATS_CP_DMG;
		Z_MVS_DMG = data.Z_MVS_DMG;
		Z_MVS_ATS = data.Z_MVS_ATS;
		Z_BJR_BJD = data.Z_BJR_BJD;
		Z_Chuan0_BJD = data.Z_Chuan0_BJD;
		Z_Chuan1_BJD = data.Z_Chuan1_BJD;
		Z_Chuan2_BJD = data.Z_Chuan2_BJD;
		Z_Chuan3_BJD = data.Z_Chuan3_BJD;
		Z_Chuan4_BJD = data.Z_Chuan4_BJD;
		Z_Chuan5_BJD = data.Z_Chuan5_BJD;
		PrcCut0 = data.PrcCut0;
		PrcCut1 = data.PrcCut1;
		PrcCut2 = data.PrcCut2;
		PrcCut3 = data.PrcCut3;
		PrcCut4 = data.PrcCut4;
		PrcCut5 = data.PrcCut5;
		PrcCut5P0 = data.PrcCut5P0;
		PrcCut5P1 = data.PrcCut5P1;
		PrcCut5P2 = data.PrcCut5P2;
		PrcCut5P3 = data.PrcCut5P3;
		PrcCut5P4 = data.PrcCut5P4;
		PrcCut5P5 = data.PrcCut5P5;
		PrcCut3P0 = data.PrcCut3P0;
		PrcCut3P1 = data.PrcCut3P1;
		PrcCut3P2 = data.PrcCut3P2;
		PrcCut3P3 = data.PrcCut3P3;
		PrcCut3P4 = data.PrcCut3P4;
		PrcCut3P5 = data.PrcCut3P5;
		DeadWD = data.DeadWD;
		DeadRageWD = data.DeadRageWD;
		DeadStealthWD = data.DeadStealthWD;
		WS_Anti0 = data.WS_Anti0;
		WS_Anti1 = data.WS_Anti1;
		WS_Anti2 = data.WS_Anti2;
		WS_Anti3 = data.WS_Anti3;
		WS_Anti4 = data.WS_Anti4;
		WS_Anti5 = data.WS_Anti5;
		WS_All = data.WS_All;
		EMC_DMG_20 = data.EMC_DMG_20;
		EMC_DMG_48 = data.EMC_DMG_48;
		EMC_Anti_9 = data.EMC_Anti_9;
		EMC_GD_12 = data.EMC_GD_12;
		JYC_DMG_15 = data.JYC_DMG_15;
		JYC_ATS_24 = data.JYC_ATS_24;
		JYC_BJD_24 = data.JYC_BJD_24;
		SKUP_Xi = data.SKUP_Xi;
		SKUP_SP = data.SKUP_SP;
		SKUP_CP = data.SKUP_CP;
		SKUP_Bei = data.SKUP_Bei;
		SKUP_Final = data.SKUP_Final;
		SKUP_AT = data.SKUP_AT;
		Dis_In = data.Dis_In;
		Dis_Out = data.Dis_Out;
		AB_DMG_Mana = data.AB_DMG_Mana;
		AB_DMG_Hurt = data.AB_DMG_Hurt;
		AB_Dot_DMG = data.AB_Dot_DMG;
		NoGD = data.NoGD;
		ST_EveryH_DMG = data.ST_EveryH_DMG;
		ST_EveryM_Drop = data.ST_EveryM_Drop;
		ORB_FQ_Count = data.ORB_FQ_Count;
		ORB_FQ_Count_Double = data.ORB_FQ_Count_Double;
		ORB_FQ_DMG80_Base = data.ORB_FQ_DMG80_Base;
		ORB_FQ_DMG120_Base = data.ORB_FQ_DMG120_Base;
		Orb_Universe_DMG_Base = data.Orb_Universe_DMG_Base;
		HighMana_DMG100_FQ = data.HighMana_DMG100_FQ;
		Orb_Universe_ATS = data.Orb_Universe_ATS;
		Orb_Bow_DMG = data.Orb_Bow_DMG;
		Orb_Bow_ATS = data.Orb_Bow_ATS;
		XJ_Count_CP_DMG = data.XJ_Count_CP_DMG;
		BurnLife0 = data.BurnLife0;
		BurnLife1 = data.BurnLife1;
		BurnLife2 = data.BurnLife2;
		BurnLife3 = data.BurnLife3;
		BurnLife4 = data.BurnLife4;
		BurnLife5 = data.BurnLife5;
		DieEXP = data.DieEXP;
		NoDot_BJD = data.NoDot_BJD;
		HealCutMana = data.HealCutMana;
		AT_UseHeal1 = data.AT_UseHeal1;
		ManaUse_Rheal = data.ManaUse_Rheal;
		RMana_RHeal = data.RMana_RHeal;
		CP_Same_RHeal = data.CP_Same_RHeal;
		FT = data.FT;
		DMG_ManaPRC = data.DMG_ManaPRC;
		Turtle = data.Turtle;
		GD_HurtR = data.GD_HurtR;
		BloodLost = data.BloodLost;
		NoGround = data.NoGround;
		CPNoBad = data.CPNoBad;
		CPNoGround = data.CPNoGround;
		AT_UseHeal2 = data.AT_UseHeal2;
		DMGsplit = data.DMGsplit;
		BladeSoul_Double = data.BladeSoul_Double;
		Diff_EL = data.Diff_EL;
		EXP_Range = data.EXP_Range;
		Buff_Range = data.Buff_Range;
		MoneyTO_DMG = data.MoneyTO_DMG;
	}

	public PlayerSaveData ExportSaveData()
	{
		EnsurePlayerDotData();
		return new PlayerSaveData
		{
			PlayerName = PlayerName,
			PlayerType = PLType,
			EquippedSetCounts = ExportEquippedSetCounts(),
			AutoAttackEnabled = Singleton<SettingDataManager>.Instance.GetGame().auto_attack,
			AutoJH = AutoJH,
			AutoDrinkH = AutoDrinkH,
			AutoDrinkM = AutoDrinkM,
			CompCount = CompCount,
			Level = Level,
			Health = Health,
			Health_Bei = Health_Bei,
			Health_R_Base = Health_R_Base,
			Health_Percent = Health_Percent,
			Mana = Mana,
			Mana_Bei = Mana_Bei,
			Mana_R_Base = Mana_R_Base,
			Mana_Percent = Mana_Percent,
			Xp_Total = Xp_Total,
			Xp_CurrentLevel = Xp_CurrentLevel,
			DFLevel = DFLevel,
			DFXp_Total = DFXp_Total,
			DFXp_CurrentLevel = DFXp_CurrentLevel,
			Attack_R_health_Percent = Attack_R_health_Percent,
			Attack_R_health_Base = Attack_R_health_Base,
			Attack_R_mana_Percent = Attack_R_mana_Percent,
			Attack_R_mana_Base = Attack_R_mana_Base,
			ATSpeed_Bei = ATSpeed_Bei,
			MVSpeed_Bei = MVSpeed_Bei,
			AntiSlow = AntiSlow,
			CoolDown = CoolDown,
			GeDang = GeDang,
			BJrate = BJrate,
			BJDamage = BJDamage,
			JYrate = JYrate,
			ThroughRate = ThroughRate,
			ItemDrop_Rate = ItemDrop_Rate,
			DOTcut = DOTcut,
			Damage_Anti = Damage_Anti,
			FlySpeed = FlySpeed,
			ORB_Damage = ORB_Damage,
			Damage_Base = Damage_Base,
			Damage_Bei = Damage_Bei,
			FireDamageXi = FireDamageXi,
			FrozenDamageXi = FrozenDamageXi,
			ThunderDamageXi = ThunderDamageXi,
			PoisonDamageXi = PoisonDamageXi,
			PhysicsDamageXi = PhysicsDamageXi,
			ShadowDamageXi = ShadowDamageXi,
			FireDamage_Bei = FireDamage_Bei,
			FrozenDamage_Bei = FrozenDamage_Bei,
			ThunderDamage_Bei = ThunderDamage_Bei,
			PoisonDamage_Bei = PoisonDamage_Bei,
			PhysicsDamage_Bei = PhysicsDamage_Bei,
			ShadowDamage_Bei = ShadowDamage_Bei,
			FireChuan = FireChuan,
			FrozenChuan = FrozenChuan,
			ThunderChuan = ThunderChuan,
			PoisonChuan = PoisonChuan,
			PhysicsChuan = PhysicsChuan,
			ShadowChuan = ShadowChuan,
			FireAnti = FireAnti,
			FrozenAnti = FrozenAnti,
			ThunderAnti = ThunderAnti,
			PoisonAnti = PoisonAnti,
			PhysicsAnti = PhysicsAnti,
			ShadowAnti = ShadowAnti,
			C_Health = C_Health,
			C_AllAnti = C_AllAnti,
			C_ATSpeed = C_ATSpeed,
			C_Damage = C_Damage,
			C_MVSpeed = C_MVSpeed,
			Pick_PL_Base = Pick_PL_Base,
			Pick_PL_Bei = Pick_PL_Bei,
			Pick_XJL_Base = Pick_XJL_Base,
			Pick_XJL_Bei = Pick_XJL_Bei,
			XJL_SellPrice = XJL_SellPrice,
			XJL_DMG = XJL_DMG,
			XJL_UseSKTime = XJL_UseSKTime,
			Reforge_Inc = Reforge_Inc,
			QH_Inc = QH_Inc,
			HH_Inc = HH_Inc,
			SK_Inc = SK_Inc,
			QH_Price = QH_Price,
			QH_Bei = QH_Bei,
			ManaXH = ManaXH,
			BJD_Anti = BJD_Anti,
			AllChuan = AllChuan,
			AllAnti = AllAnti,
			BuffT_Temple = BuffT_Temple,
			BuffT_Drink = BuffT_Drink,
			WPSPC_DMG = WPSPC_DMG,
			WPSPC_Rate = WPSPC_Rate,
			JYBoss_DMG = JYBoss_DMG,
			JYBoss_Anti = JYBoss_Anti,
			DMG_R_H = DMG_R_H,
			DMG_R_M = DMG_R_M,
			BS_Add = BS_Add,
			BS_Multi = BS_Multi,
			Temple_DMG = Temple_DMG,
			Temple_ATS = Temple_ATS,
			Temple_MVS = Temple_MVS,
			Temple_HealPrc = Temple_HealPrc,
			Temple_BS = Temple_BS,
			BE_ZQ_DMG = BE_ZQ_DMG,
			BE_ZQ_ATS = BE_ZQ_ATS,
			BE_ZQ_MVS = BE_ZQ_MVS,
			BE_ZQ_BJR = BE_ZQ_BJR,
			BE_ZQ_BJD = BE_ZQ_BJD,
			BE_ZQ_Heal = BE_ZQ_Heal,
			BE_ZQ_Mana = BE_ZQ_Mana,
			BE_ZQ_CP_Heal = BE_ZQ_CP_Heal,
			BE_ZQ_CP_DMG = BE_ZQ_CP_DMG,
			BE_ZQ_CP_ATS = BE_ZQ_CP_ATS,
			BE_ZQ_CP_MVS = BE_ZQ_CP_MVS,
			BE_ZQ_CP_Anti = BE_ZQ_CP_Anti,
			BE_ZQ_Dot = BE_ZQ_Dot,
			BE_ZQ_XJ_DMG = BE_ZQ_XJ_DMG,
			BE_ZQ_Orb_DMG = BE_ZQ_Orb_DMG,
			BE_SPC_DMG = BE_SPC_DMG,
			BE_SPC_ATS = BE_SPC_ATS,
			BE_SPC_MVS = BE_SPC_MVS,
			BE_SPC_BJR = BE_SPC_BJR,
			BE_SPC_BJD = BE_SPC_BJD,
			BE_SPC_Heal = BE_SPC_Heal,
			BE_SPC_Mana = BE_SPC_Mana,
			BE_SPC_CP_Heal = BE_SPC_CP_Heal,
			BE_SPC_CP_DMG = BE_SPC_CP_DMG,
			BE_SPC_CP_ATS = BE_SPC_CP_ATS,
			BE_SPC_CP_MVS = BE_SPC_CP_MVS,
			BE_SPC_CP_Anti = BE_SPC_CP_Anti,
			BE_SPC_Dot = BE_SPC_Dot,
			BE_SPC_XJ_DMG = BE_SPC_XJ_DMG,
			BE_SPC_Orb_DMG = BE_SPC_Orb_DMG,
			BE_HH_DMG = BE_HH_DMG,
			BE_HH_ATS = BE_HH_ATS,
			BE_HH_MVS = BE_HH_MVS,
			BE_HH_BJR = BE_HH_BJR,
			BE_HH_BJD = BE_HH_BJD,
			BE_HH_Heal = BE_HH_Heal,
			BE_HH_Mana = BE_HH_Mana,
			BE_HH_CP_Heal = BE_HH_CP_Heal,
			BE_HH_CP_DMG = BE_HH_CP_DMG,
			BE_HH_CP_ATS = BE_HH_CP_ATS,
			BE_HH_CP_MVS = BE_HH_CP_MVS,
			BE_HH_CP_Anti = BE_HH_CP_Anti,
			BE_HH_Dot = BE_HH_Dot,
			BE_HH_XJ_DMG = BE_HH_XJ_DMG,
			BE_HH_Orb_DMG = BE_HH_Orb_DMG,
			BE_SK_DMG = BE_SK_DMG,
			BE_SK_ATS = BE_SK_ATS,
			BE_SK_MVS = BE_SK_MVS,
			BE_SK_CP_Heal = BE_SK_CP_Heal,
			BE_SK_CP_DMG = BE_SK_CP_DMG,
			BE_SK_CP_ATS = BE_SK_CP_ATS,
			BE_SK_CP_Anti = BE_SK_CP_Anti,
			BE_SK_XJ_DMG = BE_SK_XJ_DMG,
			BE_SK_Orb_DMG = BE_SK_Orb_DMG,
			BE_SK_FQ_Count = BE_SK_FQ_Count,
			BE_BS_DMG = BE_BS_DMG,
			BE_BS_ATS = BE_BS_ATS,
			BE_BS_MVS = BE_BS_MVS,
			BE_BS_CP_Heal = BE_BS_CP_Heal,
			BE_BS_CP_DMG = BE_BS_CP_DMG,
			BE_BS_CP_ATS = BE_BS_CP_ATS,
			BE_BS_CP_Anti = BE_BS_CP_Anti,
			BE_BS_XJ_DMG = BE_BS_XJ_DMG,
			BE_BS_Orb_DMG = BE_BS_Orb_DMG,
			BE_BS_FQ_Count = BE_BS_FQ_Count,
			Crit_BoomEXP = Crit_BoomEXP,
			Crit_BoomDie_Rate = Crit_BoomDie_Rate,
			Crit_MS = Crit_MS,
			LowH_DMG20 = LowH_DMG20,
			LowH_DMG50 = LowH_DMG50,
			HighH_DMG90 = HighH_DMG90,
			HighH_DMG100 = HighH_DMG100,
			LowH_HurtR20 = LowH_HurtR20,
			HighH_HurtR100 = HighH_HurtR100,
			LowH_DMGAnti20 = LowH_DMGAnti20,
			LowH_DMGAnti50 = LowH_DMGAnti50,
			LowH_CritAnti10 = LowH_CritAnti10,
			LowM_DMG20 = LowM_DMG20,
			LowM_DMG50 = LowM_DMG50,
			HighM_DMG90 = HighM_DMG90,
			HighM_DMG100 = HighM_DMG100,
			LowM_HurtR20 = LowM_HurtR20,
			HighM_HurtR100 = HighM_HurtR100,
			ST_MV_DMG = ST_MV_DMG,
			ST_MV_ATS = ST_MV_ATS,
			ST_MV_GD = ST_MV_GD,
			ST_NoMV_DMG = ST_NoMV_DMG,
			ST_NoMV_ATS = ST_NoMV_ATS,
			ST_NoMV_DMGAnti = ST_NoMV_DMGAnti,
			ST_NoMV_HealPrc = ST_NoMV_HealPrc,
			ST_NoMV_ManaPrc = ST_NoMV_ManaPrc,
			ST_Chong_DMG = ST_Chong_DMG,
			ST_Chong_Anti = ST_Chong_Anti,
			EM_LowH_DMG20 = EM_LowH_DMG20,
			EM_LowH_DMG50 = EM_LowH_DMG50,
			EM_HighH_DMG60 = EM_HighH_DMG60,
			EM_HighH_DMG100 = EM_HighH_DMG100,
			EM_Heal_Crit = EM_Heal_Crit,
			CP1_DMG = CP1_DMG,
			CP1_ATS = CP1_ATS,
			CP1_MVS = CP1_MVS,
			CP1_Heal = CP1_Heal,
			CP1_Mana = CP1_Mana,
			CP1_DMG_Anti = CP1_DMG_Anti,
			CP1_DropR = CP1_DropR,
			CP1_ORB_DMG = CP1_ORB_DMG,
			CP1_DMG0 = CP1_DMG0,
			CP1_DMG1 = CP1_DMG1,
			CP1_DMG2 = CP1_DMG2,
			CP1_DMG3 = CP1_DMG3,
			CP1_DMG4 = CP1_DMG4,
			CP1_DMG5 = CP1_DMG5,
			CP1_Chuan0 = CP1_Chuan0,
			CP1_Chuan1 = CP1_Chuan1,
			CP1_Chuan2 = CP1_Chuan2,
			CP1_Chuan3 = CP1_Chuan3,
			CP1_Chuan4 = CP1_Chuan4,
			CP1_Chuan5 = CP1_Chuan5,
			CP1_CP_Heal = CP1_CP_Heal,
			CP1_CP_DMG = CP1_CP_DMG,
			CP1_CP_ATS = CP1_CP_ATS,
			CP1_CP_AllAnti = CP1_CP_AllAnti,
			CLass_DMG = CLass_DMG,
			CLass_ATS = CLass_ATS,
			CLass_MVS = CLass_MVS,
			CLass_Heal = CLass_Heal,
			CLass_Mana = CLass_Mana,
			CLass_DMG_Anti = CLass_DMG_Anti,
			CLass_DropR = CLass_DropR,
			CLass_ORB_DMG = CLass_ORB_DMG,
			CLass_DMG0 = CLass_DMG0,
			CLass_DMG1 = CLass_DMG1,
			CLass_DMG2 = CLass_DMG2,
			CLass_DMG3 = CLass_DMG3,
			CLass_DMG4 = CLass_DMG4,
			CLass_DMG5 = CLass_DMG5,
			CLass_Chuan0 = CLass_Chuan0,
			CLass_Chuan1 = CLass_Chuan1,
			CLass_Chuan2 = CLass_Chuan2,
			CLass_Chuan3 = CLass_Chuan3,
			CLass_Chuan4 = CLass_Chuan4,
			CLass_Chuan5 = CLass_Chuan5,
			CLass_CP_Heal = CLass_CP_Heal,
			CLass_CP_DMG = CLass_CP_DMG,
			CLass_CP_ATS = CLass_CP_ATS,
			CLass_CP_AllAnti = CLass_CP_AllAnti,
			Class_CP_DotDMG = Class_CP_DotDMG,
			XJ_DMG = XJ_DMG,
			XJ_Time = XJ_Time,
			TuT_Buff = TuT_Buff,
			TuT_Time = TuT_Time,
			TuT_PlayerAll = TuT_PlayerAll,
			Top_CD = Top_CD,
			Top_GD = Top_GD,
			Top_Anti = Top_Anti,
			Top_Cut_DMG = Top_Cut_DMG,
			Top_Cut_MVS = Top_Cut_MVS,
			Top_Cut_ATS = Top_Cut_ATS,
			AllDot_DMG = AllDot_DMG,
			AllDot_Time = AllDot_Time,
			AllDot_Layer = AllDot_Layer,
			AllDot_MV = AllDot_MV,
			AllDot_JY = AllDot_JY,
			DiffDotDMG = DiffDotDMG,
			DiffDebuff_DMG = DiffDebuff_DMG,
			Dot_MSAll = Dot_MSAll,
			DrinkPre_Heal = DrinkPre_Heal,
			DrinkPre_Mana = DrinkPre_Mana,
			DrinkPre_DMG = DrinkPre_DMG,
			Drink_CP = Drink_CP,
			Z_Hmax_DMG = Z_Hmax_DMG,
			Z_Huse_DMG = Z_Huse_DMG,
			Z_Mmax_DMG = Z_Mmax_DMG,
			Z_Mcur_DMG = Z_Mcur_DMG,
			Z_Muse_DMG = Z_Muse_DMG,
			Z_Hmax_EL0 = Z_Hmax_EL0,
			Z_Hmax_EL1 = Z_Hmax_EL1,
			Z_Hmax_EL2 = Z_Hmax_EL2,
			Z_Hmax_EL3 = Z_Hmax_EL3,
			Z_Hmax_EL4 = Z_Hmax_EL4,
			Z_Hmax_EL5 = Z_Hmax_EL5,
			Z_Mmax_EL0 = Z_Mmax_EL0,
			Z_Mmax_EL1 = Z_Mmax_EL1,
			Z_Mmax_EL2 = Z_Mmax_EL2,
			Z_Mmax_EL3 = Z_Mmax_EL3,
			Z_Mmax_EL4 = Z_Mmax_EL4,
			Z_Mmax_EL5 = Z_Mmax_EL5,
			Z_CD_EL0 = Z_CD_EL0,
			Z_CD_EL1 = Z_CD_EL1,
			Z_CD_EL2 = Z_CD_EL2,
			Z_CD_EL3 = Z_CD_EL3,
			Z_CD_EL4 = Z_CD_EL4,
			Z_CD_EL5 = Z_CD_EL5,
			Z_Anti0_EL0 = Z_Anti0_EL0,
			Z_Anti0_EL1 = Z_Anti0_EL1,
			Z_Anti0_EL2 = Z_Anti0_EL2,
			Z_Anti0_EL3 = Z_Anti0_EL3,
			Z_Anti0_EL4 = Z_Anti0_EL4,
			Z_Anti0_EL5 = Z_Anti0_EL5,
			Z_Chuan0_EL0 = Z_Chuan0_EL0,
			Z_Chuan0_EL1 = Z_Chuan0_EL1,
			Z_Chuan0_EL2 = Z_Chuan0_EL2,
			Z_Chuan0_EL3 = Z_Chuan0_EL3,
			Z_Chuan0_EL4 = Z_Chuan0_EL4,
			Z_Chuan0_EL5 = Z_Chuan0_EL5,
			Z_GD_EL0 = Z_GD_EL0,
			Z_GD_EL1 = Z_GD_EL1,
			Z_GD_EL2 = Z_GD_EL2,
			Z_GD_EL3 = Z_GD_EL3,
			Z_GD_EL4 = Z_GD_EL4,
			Z_GD_EL5 = Z_GD_EL5,
			Z_BJR_EL0 = Z_BJR_EL0,
			Z_BJR_EL1 = Z_BJR_EL1,
			Z_BJR_EL2 = Z_BJR_EL2,
			Z_BJR_EL3 = Z_BJR_EL3,
			Z_BJR_EL4 = Z_BJR_EL4,
			Z_BJR_EL5 = Z_BJR_EL5,
			Z_DMGCut_EL0 = Z_DMGCut_EL0,
			Z_DMGCut_EL1 = Z_DMGCut_EL1,
			Z_DMGCut_EL2 = Z_DMGCut_EL2,
			Z_DMGCut_EL3 = Z_DMGCut_EL3,
			Z_DMGCut_EL4 = Z_DMGCut_EL4,
			Z_DMGCut_EL5 = Z_DMGCut_EL5,
			Z_Thr_EL0 = Z_Thr_EL0,
			Z_Thr_EL1 = Z_Thr_EL1,
			Z_Thr_EL2 = Z_Thr_EL2,
			Z_Thr_EL3 = Z_Thr_EL3,
			Z_Thr_EL4 = Z_Thr_EL4,
			Z_Thr_EL5 = Z_Thr_EL5,
			Z_CD_CP_DMG = Z_CD_CP_DMG,
			Z_ATS_CP_DMG = Z_ATS_CP_DMG,
			Z_MVS_DMG = Z_MVS_DMG,
			Z_MVS_ATS = Z_MVS_ATS,
			Z_BJR_BJD = Z_BJR_BJD,
			Z_Chuan0_BJD = Z_Chuan0_BJD,
			Z_Chuan1_BJD = Z_Chuan1_BJD,
			Z_Chuan2_BJD = Z_Chuan2_BJD,
			Z_Chuan3_BJD = Z_Chuan3_BJD,
			Z_Chuan4_BJD = Z_Chuan4_BJD,
			Z_Chuan5_BJD = Z_Chuan5_BJD,
			PrcCut0 = PrcCut0,
			PrcCut1 = PrcCut1,
			PrcCut2 = PrcCut2,
			PrcCut3 = PrcCut3,
			PrcCut4 = PrcCut4,
			PrcCut5 = PrcCut5,
			PrcCut5P0 = PrcCut5P0,
			PrcCut5P1 = PrcCut5P1,
			PrcCut5P2 = PrcCut5P2,
			PrcCut5P3 = PrcCut5P3,
			PrcCut5P4 = PrcCut5P4,
			PrcCut5P5 = PrcCut5P5,
			PrcCut3P0 = PrcCut3P0,
			PrcCut3P1 = PrcCut3P1,
			PrcCut3P2 = PrcCut3P2,
			PrcCut3P3 = PrcCut3P3,
			PrcCut3P4 = PrcCut3P4,
			PrcCut3P5 = PrcCut3P5,
			DeadWD = DeadWD,
			DeadRageWD = DeadRageWD,
			DeadStealthWD = DeadStealthWD,
			WS_Anti0 = WS_Anti0,
			WS_Anti1 = WS_Anti1,
			WS_Anti2 = WS_Anti2,
			WS_Anti3 = WS_Anti3,
			WS_Anti4 = WS_Anti4,
			WS_Anti5 = WS_Anti5,
			WS_All = WS_All,
			EMC_DMG_20 = EMC_DMG_20,
			EMC_DMG_48 = EMC_DMG_48,
			EMC_Anti_9 = EMC_Anti_9,
			EMC_GD_12 = EMC_GD_12,
			JYC_DMG_15 = JYC_DMG_15,
			JYC_ATS_24 = JYC_ATS_24,
			JYC_BJD_24 = JYC_BJD_24,
			SKUP_Xi = SKUP_Xi,
			SKUP_SP = SKUP_SP,
			SKUP_CP = SKUP_CP,
			SKUP_Bei = SKUP_Bei,
			SKUP_Final = SKUP_Final,
			SKUP_AT = SKUP_AT,
			Dis_In = Dis_In,
			Dis_Out = Dis_Out,
			AB_DMG_Mana = AB_DMG_Mana,
			AB_DMG_Hurt = AB_DMG_Hurt,
			AB_Dot_DMG = AB_Dot_DMG,
			NoGD = NoGD,
			ST_EveryH_DMG = ST_EveryH_DMG,
			ST_EveryM_Drop = ST_EveryM_Drop,
			ORB_FQ_Count = ORB_FQ_Count,
			ORB_FQ_Count_Double = ORB_FQ_Count_Double,
			ORB_FQ_DMG80_Base = ORB_FQ_DMG80_Base,
			ORB_FQ_DMG120_Base = ORB_FQ_DMG120_Base,
			Orb_Universe_DMG_Base = Orb_Universe_DMG_Base,
			HighMana_DMG100_FQ = HighMana_DMG100_FQ,
			Orb_Universe_ATS = Orb_Universe_ATS,
			Orb_Bow_DMG = Orb_Bow_DMG,
			Orb_Bow_ATS = Orb_Bow_ATS,
			XJ_Count_CP_DMG = XJ_Count_CP_DMG,
			BurnLife0 = BurnLife0,
			BurnLife1 = BurnLife1,
			BurnLife2 = BurnLife2,
			BurnLife3 = BurnLife3,
			BurnLife4 = BurnLife4,
			BurnLife5 = BurnLife5,
			DieEXP = DieEXP,
			NoDot_BJD = NoDot_BJD,
			HealCutMana = HealCutMana,
			AT_UseHeal1 = AT_UseHeal1,
			ManaUse_Rheal = ManaUse_Rheal,
			RMana_RHeal = RMana_RHeal,
			CP_Same_RHeal = CP_Same_RHeal,
			FT = FT,
			DMG_ManaPRC = DMG_ManaPRC,
			Turtle = Turtle,
			GD_HurtR = GD_HurtR,
			BloodLost = BloodLost,
			NoGround = NoGround,
			CPNoBad = CPNoBad,
			CPNoGround = CPNoGround,
			AT_UseHeal2 = AT_UseHeal2,
			DMGsplit = DMGsplit,
			BladeSoul_Double = BladeSoul_Double,
			Diff_EL = Diff_EL,
			EXP_Range = EXP_Range,
			Buff_Range = Buff_Range,
			MoneyTO_DMG = MoneyTO_DMG,
			Dot_Fire = ClonePlayerDotData(DOT[0]),
			Dot_Ice = ClonePlayerDotData(DOT[1]),
			Dot_TD = ClonePlayerDotData(DOT[2]),
			Dot_Du = ClonePlayerDotData(DOT[3]),
			Dot_Phy = ClonePlayerDotData(DOT[4]),
			Dot_SD = ClonePlayerDotData(DOT[5])
		};
	}

	public void SetPosition(Vector3 pos)
	{
		base.gameObject.transform.position = pos;
	}

	private void ResetRuntimeTempStats()
	{
		Xp_Bei_Tmp = 0f;
		ATSpeed_Tmp = 0f;
		ATSpeed_Tmp_Cut = 0f;
		MVSpeed_Tmp = 0f;
		MVSpeed_Tmp_Cut = 0f;
		BJDamage_Tmp = 0f;
		Health_Bei_Tmp = 0f;
		Health_Percent_Tmp = 0f;
		Mana_Percent_Tmp = 0f;
		CoolDown_Tmp = 0f;
		GeDang_Tmp = 0f;
		BJrate_Tmp = 0f;
		XJL_BJD_Anti_Tmp = 0f;
		JYrate_Tmp = 0f;
		ItemDrop_Rate_buff_Tmp = 0f;
		ItemDrop_Rate_mijing_Tmp = 0f;
		Damage_Anti_Tmp = 0f;
		Damage_Bei_Tmp = 0f;
		FireDamage_Bei_Tmp = 0f;
		FrozenDamage_Bei_Tmp = 0f;
		ThunderDamage_Bei_Tmp = 0f;
		PoisonDamage_Bei_Tmp = 0f;
		PhysicsDamage_Bei_Tmp = 0f;
		ShadowDamage_Bei_Tmp = 0f;
		FireChuan_Tmp = 0f;
		FrozenChuan_Tmp = 0f;
		ThunderChuan_Tmp = 0f;
		PoisonChuan_Tmp = 0f;
		PhysicsChuan_Tmp = 0f;
		ShadowChuan_Tmp = 0f;
		FireAnti_Tmp = 0f;
		FrozenAnti_Tmp = 0f;
		ThunderAnti_Tmp = 0f;
		PoisonAnti_Tmp = 0f;
		PhysicsAnti_Tmp = 0f;
		ShadowAnti_Tmp = 0f;
		C_Health_Tmp = 0f;
		C_Damage_Tmp = 0f;
		C_ATSpeed_Tmp = 0f;
		C_MVSpeed_Tmp = 0f;
		C_AllAnti_Tmp = 0f;
		Runtime_DotDamage_Tmp = 0f;
		Runtime_DotTimeCut_Tmp = 0f;
		Runtime_ORB_Damage_Tmp = 0f;
		Runtime_XJ_DMG_Tmp = 0f;
	}

	public void InitPlayer()
	{
		EnsurePlayerDotData();
		if ((bool)xjl)
		{
			xjl.ClearAllXJL();
		}
		ClearEquippedSetCounts();
		ChongSpeed = 1f;
		TimeA = 0f;
		TimeB = 0f;
		TimeC = 0f;
		ATSpeed_Base = 1.4f;
		ResetRuntimeTempStats();
		MVSpeed_Base = 2.3f;
		CompCount = 0;
		Pick_PL_Base = 0.8f;
		Pick_PL_Bei = 0f;
		Pick_XJL_Base = 0.8f;
		Pick_XJL_Bei = 0f;
		XJL_SellPrice = 30f;
		XJL_DMG = 0f;
		XJL_UseSKTime = 0f;
		Reforge_Inc = 0;
		QH_Inc = 0;
		HH_Inc = 10;
		SK_Inc = 0;
		QH_Price = 0;
		QH_Bei = 0;
		ManaXH = 0f;
		BJD_Anti = 0f;
		AllChuan = 0f;
		AllAnti = 0f;
		BuffT_Temple = 0f;
		BuffT_Drink = 0f;
		WPSPC_DMG = 0;
		WPSPC_Rate = 0;
		JYBoss_DMG = 0;
		JYBoss_Anti = 0;
		DMG_R_H = 0f;
		DMG_R_M = 0f;
		BS_Add = 0;
		BS_Multi = 0f;
		Temple_DMG = 0;
		Temple_ATS = 0;
		Temple_MVS = 0;
		Temple_HealPrc = 0f;
		Temple_BS = 0;
		BE_ZQ_DMG = 0f;
		BE_ZQ_ATS = 0f;
		BE_ZQ_MVS = 0f;
		BE_ZQ_BJR = 0f;
		BE_ZQ_BJD = 0f;
		BE_ZQ_Heal = 0f;
		BE_ZQ_Mana = 0f;
		BE_ZQ_CP_Heal = 0f;
		BE_ZQ_CP_DMG = 0f;
		BE_ZQ_CP_ATS = 0f;
		BE_ZQ_CP_MVS = 0f;
		BE_ZQ_CP_Anti = 0f;
		BE_ZQ_Dot = 0f;
		BE_ZQ_XJ_DMG = 0f;
		BE_ZQ_Orb_DMG = 0f;
		BE_SPC_DMG = 0f;
		BE_SPC_ATS = 0f;
		BE_SPC_MVS = 0f;
		BE_SPC_BJR = 0f;
		BE_SPC_BJD = 0f;
		BE_SPC_Heal = 0f;
		BE_SPC_Mana = 0f;
		BE_SPC_CP_Heal = 0f;
		BE_SPC_CP_DMG = 0f;
		BE_SPC_CP_ATS = 0f;
		BE_SPC_CP_MVS = 0f;
		BE_SPC_CP_Anti = 0f;
		BE_SPC_Dot = 0f;
		BE_SPC_XJ_DMG = 0f;
		BE_SPC_Orb_DMG = 0f;
		BE_HH_DMG = 0f;
		BE_HH_ATS = 0f;
		BE_HH_MVS = 0f;
		BE_HH_BJR = 0f;
		BE_HH_BJD = 0f;
		BE_HH_Heal = 0f;
		BE_HH_Mana = 0f;
		BE_HH_CP_Heal = 0f;
		BE_HH_CP_DMG = 0f;
		BE_HH_CP_ATS = 0f;
		BE_HH_CP_MVS = 0f;
		BE_HH_CP_Anti = 0f;
		BE_HH_Dot = 0f;
		BE_HH_XJ_DMG = 0f;
		BE_HH_Orb_DMG = 0f;
		BE_SK_DMG = 0f;
		BE_SK_ATS = 0f;
		BE_SK_MVS = 0f;
		BE_SK_CP_Heal = 0f;
		BE_SK_CP_DMG = 0f;
		BE_SK_CP_ATS = 0f;
		BE_SK_CP_Anti = 0f;
		BE_SK_XJ_DMG = 0f;
		BE_SK_Orb_DMG = 0f;
		BE_SK_FQ_Count = 0;
		BE_BS_DMG = 0f;
		BE_BS_ATS = 0f;
		BE_BS_MVS = 0f;
		BE_BS_CP_Heal = 0f;
		BE_BS_CP_DMG = 0f;
		BE_BS_CP_ATS = 0f;
		BE_BS_CP_Anti = 0f;
		BE_BS_XJ_DMG = 0f;
		BE_BS_Orb_DMG = 0f;
		BE_BS_FQ_Count = 0;
		Crit_BoomEXP = 0;
		Crit_BoomDie_Rate = 0;
		Crit_MS = 0;
		LowH_DMG20 = 0;
		LowH_DMG50 = 0;
		HighH_DMG90 = 0;
		HighH_DMG100 = 0;
		LowH_HurtR20 = 0;
		HighH_HurtR100 = 0;
		LowH_DMGAnti20 = 0;
		LowH_DMGAnti50 = 0;
		LowH_CritAnti10 = false;
		LowM_DMG20 = 0;
		LowM_DMG50 = 0;
		HighM_DMG90 = 0;
		HighM_DMG100 = 0;
		LowM_HurtR20 = 0;
		HighM_HurtR100 = 0;
		ST_MV_DMG = 0;
		ST_MV_ATS = 0;
		ST_MV_GD = 0;
		ST_NoMV_DMG = 0;
		ST_NoMV_ATS = 0;
		ST_NoMV_DMGAnti = 0;
		ST_NoMV_HealPrc = 0f;
		ST_NoMV_ManaPrc = 0f;
		ST_Chong_DMG = 0;
		ST_Chong_Anti = 0;
		EM_LowH_DMG20 = 0;
		EM_LowH_DMG50 = 0;
		EM_HighH_DMG60 = 0;
		EM_HighH_DMG100 = 0;
		EM_Heal_Crit = 0;
		CP1_DMG = 0f;
		CP1_ATS = 0f;
		CP1_MVS = 0f;
		CP1_Heal = 0f;
		CP1_Mana = 0f;
		CP1_DMG_Anti = 0f;
		CP1_DropR = 0f;
		CP1_ORB_DMG = 0f;
		CP1_DMG0 = 0f;
		CP1_DMG1 = 0f;
		CP1_DMG2 = 0f;
		CP1_DMG3 = 0f;
		CP1_DMG4 = 0f;
		CP1_DMG5 = 0f;
		CP1_Chuan0 = 0f;
		CP1_Chuan1 = 0f;
		CP1_Chuan2 = 0f;
		CP1_Chuan3 = 0f;
		CP1_Chuan4 = 0f;
		CP1_Chuan5 = 0f;
		CP1_CP_Heal = 0f;
		CP1_CP_DMG = 0f;
		CP1_CP_ATS = 0f;
		CP1_CP_AllAnti = 0f;
		CLass_DMG = 0f;
		CLass_ATS = 0f;
		CLass_MVS = 0f;
		CLass_Heal = 0f;
		CLass_Mana = 0f;
		CLass_DMG_Anti = 0f;
		CLass_DropR = 0f;
		CLass_ORB_DMG = 0f;
		CLass_DMG0 = 0f;
		CLass_DMG1 = 0f;
		CLass_DMG2 = 0f;
		CLass_DMG3 = 0f;
		CLass_DMG4 = 0f;
		CLass_DMG5 = 0f;
		CLass_Chuan0 = 0f;
		CLass_Chuan1 = 0f;
		CLass_Chuan2 = 0f;
		CLass_Chuan3 = 0f;
		CLass_Chuan4 = 0f;
		CLass_Chuan5 = 0f;
		CLass_CP_Heal = 0f;
		CLass_CP_DMG = 0f;
		CLass_CP_ATS = 0f;
		CLass_CP_AllAnti = 0f;
		Class_CP_DotDMG = 0f;
		XJ_DMG = 0;
		XJ_Time = 0;
		TuT_Buff = 0;
		TuT_Time = 0;
		TuT_PlayerAll = false;
		Top_CD = 0;
		Top_GD = 0;
		Top_Anti = 0;
		Top_Cut_DMG = 0f;
		Top_Cut_MVS = 0f;
		Top_Cut_ATS = 0f;
		AllDot_DMG = 0f;
		AllDot_Time = 0f;
		AllDot_Layer = 0;
		AllDot_MV = 0f;
		AllDot_JY = 0f;
		DiffDotDMG = 0f;
		DiffDebuff_DMG = 0;
		Dot_MSAll = false;
		DrinkPre_Heal = 0;
		DrinkPre_Mana = 0;
		DrinkPre_DMG = 0;
		Drink_CP = false;
		Z_Hmax_DMG = 0f;
		Z_Huse_DMG = 0f;
		Z_Mmax_DMG = 0f;
		Z_Mcur_DMG = 0f;
		Z_Muse_DMG = 0f;
		Z_Hmax_EL0 = 0f;
		Z_Hmax_EL1 = 0f;
		Z_Hmax_EL2 = 0f;
		Z_Hmax_EL3 = 0f;
		Z_Hmax_EL4 = 0f;
		Z_Hmax_EL5 = 0f;
		Z_Mmax_EL0 = 0f;
		Z_Mmax_EL1 = 0f;
		Z_Mmax_EL2 = 0f;
		Z_Mmax_EL3 = 0f;
		Z_Mmax_EL4 = 0f;
		Z_Mmax_EL5 = 0f;
		Z_CD_EL0 = 0f;
		Z_CD_EL1 = 0f;
		Z_CD_EL2 = 0f;
		Z_CD_EL3 = 0f;
		Z_CD_EL4 = 0f;
		Z_CD_EL5 = 0f;
		Z_Anti0_EL0 = 0;
		Z_Anti0_EL1 = 0;
		Z_Anti0_EL2 = 0;
		Z_Anti0_EL3 = 0;
		Z_Anti0_EL4 = 0;
		Z_Anti0_EL5 = 0;
		Z_Chuan0_EL0 = 0;
		Z_Chuan0_EL1 = 0;
		Z_Chuan0_EL2 = 0;
		Z_Chuan0_EL3 = 0;
		Z_Chuan0_EL4 = 0;
		Z_Chuan0_EL5 = 0;
		Z_GD_EL0 = 0;
		Z_GD_EL1 = 0;
		Z_GD_EL2 = 0;
		Z_GD_EL3 = 0;
		Z_GD_EL4 = 0;
		Z_GD_EL5 = 0;
		Z_BJR_EL0 = 0;
		Z_BJR_EL1 = 0;
		Z_BJR_EL2 = 0;
		Z_BJR_EL3 = 0;
		Z_BJR_EL4 = 0;
		Z_BJR_EL5 = 0;
		Z_DMGCut_EL0 = 0;
		Z_DMGCut_EL1 = 0;
		Z_DMGCut_EL2 = 0;
		Z_DMGCut_EL3 = 0;
		Z_DMGCut_EL4 = 0;
		Z_DMGCut_EL5 = 0;
		Z_Thr_EL0 = 0;
		Z_Thr_EL1 = 0;
		Z_Thr_EL2 = 0;
		Z_Thr_EL3 = 0;
		Z_Thr_EL4 = 0;
		Z_Thr_EL5 = 0;
		Z_CD_CP_DMG = 0f;
		Z_ATS_CP_DMG = 0f;
		Z_MVS_DMG = 0f;
		Z_MVS_ATS = 0f;
		Z_BJR_BJD = false;
		Z_Chuan0_BJD = 0;
		Z_Chuan1_BJD = 0;
		Z_Chuan2_BJD = 0;
		Z_Chuan3_BJD = 0;
		Z_Chuan4_BJD = 0;
		Z_Chuan5_BJD = 0;
		PrcCut0 = 0;
		PrcCut1 = 0;
		PrcCut2 = 0;
		PrcCut3 = 0;
		PrcCut4 = 0;
		PrcCut5 = 0;
		PrcCut5P0 = 0;
		PrcCut5P1 = 0;
		PrcCut5P2 = 0;
		PrcCut5P3 = 0;
		PrcCut5P4 = 0;
		PrcCut5P5 = 0;
		PrcCut3P0 = 0;
		PrcCut3P1 = 0;
		PrcCut3P2 = 0;
		PrcCut3P3 = 0;
		PrcCut3P4 = 0;
		PrcCut3P5 = 0;
		DeadWD = false;
		DeadRageWD = false;
		DeadStealthWD = false;
		WS_Anti0 = false;
		WS_Anti1 = false;
		WS_Anti2 = false;
		WS_Anti3 = false;
		WS_Anti4 = false;
		WS_Anti5 = false;
		WS_All = false;
		EMC_DMG_20 = 0f;
		EMC_DMG_48 = 0f;
		EMC_Anti_9 = 0f;
		EMC_GD_12 = 0f;
		JYC_DMG_15 = 0f;
		JYC_ATS_24 = 0f;
		JYC_BJD_24 = 0f;
		SKUP_Xi = 0;
		SKUP_SP = 0;
		SKUP_CP = 0;
		SKUP_Bei = 0;
		SKUP_Final = 0;
		SKUP_AT = 0;
		Dis_In = 0;
		Dis_Out = false;
		AB_DMG_Mana = false;
		AB_DMG_Hurt = false;
		AB_Dot_DMG = false;
		NoGD = false;
		ST_EveryH_DMG = 0f;
		ST_EveryM_Drop = 0f;
		ORB_FQ_Count = 0;
		ORB_FQ_Count_Double = false;
		ORB_FQ_DMG80_Base = 0;
		ORB_FQ_DMG120_Base = 0;
		Orb_Universe_DMG_Base = 0f;
		HighMana_DMG100_FQ = 0;
		Orb_Universe_ATS = 0f;
		Orb_Bow_DMG = 0f;
		Orb_Bow_ATS = 0f;
		XJ_Count_CP_DMG = 0;
		BurnLife0 = 0;
		BurnLife1 = 0;
		BurnLife2 = 0;
		BurnLife3 = 0;
		BurnLife4 = 0;
		BurnLife5 = 0;
		DieEXP = false;
		NoDot_BJD = 0;
		HealCutMana = false;
		AT_UseHeal1 = false;
		ManaUse_Rheal = 0;
		RMana_RHeal = false;
		CP_Same_RHeal = false;
		FT = false;
		DMG_ManaPRC = 0;
		Turtle = false;
		GD_HurtR = 0;
		BloodLost = false;
		NoGround = false;
		CPNoBad = false;
		CPNoGround = false;
		AT_UseHeal2 = false;
		DMGsplit = 0f;
		BladeSoul_Double = false;
		Diff_EL = 0;
		EXP_Range = 0f;
		Buff_Range = 0f;
		BuffEvery_CP = 0;
		Z_Dot_EL = 0;
		Z_Dot_MV = 0;
		Clear1 = 0;
		Clear2 = 0;
		GD_DMG = 0;
		Final_Diff_DMG = 0;
		PickBS_MVS = 0;
		NoUseSK_DMG1 = 0;
		NoUseSK_DMG2 = 0;
		TP_DMG = 0;
		MV_DMG = 0;
		DeadWD = false;
		DeadRageWD = false;
		DeadStealthWD = false;
		Attack_DMG1 = 0;
		Attack_DMG2 = 0;
		Attack_ATS1 = 0;
		Attack_ATS2 = 0;
		Attack_Chuan = 0;
		Attack_BJR = 0;
		Attack_BJD = 0;
		Attack_DotDMG1 = 0;
		Attack_DotDMG2 = 0;
		Kem_DMG1 = 0;
		Kem_DMG2 = 0;
		Kem_ATS1 = 0;
		Kem_ATS2 = 0;
		Kem_EL0 = 0;
		Kem_EL1 = 0;
		Kem_EL2 = 0;
		Kem_EL3 = 0;
		Kem_EL4 = 0;
		Kem_EL5 = 0;
		Kem_CP_DMG1 = 0;
		Kem_CP_DMG2 = 0;
		Kem_CP_ATS1 = 0;
		Kem_CP_ATS2 = 0;
		Kjy_DMG = 0;
		Kjy_AllAnti = 0;
		Kem_Refresh = 0;
		MoneyTO_DMG = false;
		XJL_Count = 0f;
		XJL_DropMulti = 0f;
		BE_ZQ_Count = 0;
		BE_SPC_Count = 0;
		BE_HH_Count = 0;
		BE_SK_Count = 0;
		BE_BS_Count = 0;
		NearEMC = 0;
		NearJYC = 0;
		Orb_Bow_DMG_ORB = 0f;
		Orb_Bow_DMG_Anti = 0f;
		Q_1 = 0;
		Q_2 = 0;
		Q_3 = 0;
		Q_4 = 0;
		Q_5 = 0;
		Q_6 = 0;
		Q_7 = 0;
		Q_8 = 0;
		Q_9 = 0;
		Q_10 = 0;
		Q_11 = 0;
		Q_12 = 0;
		Q_13 = 0;
		Q_14 = 0;
		Q_15 = 0;
		Q_16 = 0;
		Q_17 = 0;
		Q_18 = 0;
		Q_19 = 0;
		Q_20 = 0;
		Q_30 = 0;
		Q_40 = 0;
		Q_41 = 0;
		Q_42 = 0;
		Q_43 = 0;
		Q_44 = 0;
		Q_45 = 0;
		Q_51 = 0;
		Q_52 = 0;
		Q_53 = 0;
		Q_54 = 0;
		Q_55 = 0;
		Q_56 = 0;
		Q_70 = 0;
		Q_71 = 0;
		Q_72 = 0;
		Q_73 = 0;
		Q_74 = 0;
		Q_75 = 0;
		Q_80 = 0;
		Q_81 = 0;
		Q_82 = 0;
		Q_83 = 0;
		Q_84 = 0;
		Q_85 = 0;
		Q_86 = 0;
		Q_90 = 0;
		DMG_1 = 0f;
		DMG_2 = 0f;
		DMG_3 = 0f;
		DMG_4 = 0f;
		DMG_5 = 0f;
		DMG_6 = 0f;
		DMG_7 = 0f;
		DMG_8 = 0f;
		DMG_9 = 0f;
		DMG_10 = 0f;
		DMG_11 = 0f;
		DMG_12 = 0f;
		DMG_13 = 0f;
		DMG_14 = 0f;
		DMG_15 = 0f;
		DMG_16 = 0f;
		DMG_17 = 0f;
		DMG_18 = 0f;
		DMG_19 = 0f;
		DMG_20 = 0f;
		DMG_30 = 0f;
		DMG_40 = 0f;
		DMG_41 = 0f;
		DMG_42 = 0f;
		DMG_43 = 0f;
		DMG_44 = 0f;
		DMG_45 = 0f;
		DMG_51 = 0f;
		DMG_52 = 0f;
		DMG_53 = 0f;
		DMG_54 = 0f;
		DMG_55 = 0f;
		DMG_56 = 0f;
		DMG_70 = 0f;
		DMG_71 = 0f;
		DMG_72 = 0f;
		DMG_73 = 0f;
		DMG_74 = 0f;
		DMG_75 = 0f;
		DMG_80 = 0f;
		DMG_81 = 0f;
		DMG_82 = 0f;
		DMG_83 = 0f;
		DMG_84 = 0f;
		DMG_85 = 0f;
		DMG_86 = 0f;
		DMG_90 = 0f;
		IsAttack = false;
		IsSkill = false;
		IsAttackAnimationSkill = false;
		IsBattle = false;
	}

	public void RefreshRuntimeDerivedStats()
	{
		HealStat.Max = Health + Health * Health_Bei_Last / 100f;
		ManaStat.Max = Mana + Mana * Mana_Bei_Last / 100f;
		if (RMana_RHeal)
		{
			Health_R_Max = Health_R_Base + HealStat.Max * (Health_Percent_Last / 100f) + Mana_R_Base + ManaStat.Max * (Mana_Percent_Last / 100f);
			Mana_R_Max = 0f;
		}
		else
		{
			Health_R_Max = Health_R_Base + HealStat.Max * (Health_Percent_Last / 100f);
			Mana_R_Max = Mana_R_Base + ManaStat.Max * (Mana_Percent_Last / 100f);
		}
		if (MVSpeed_Tmp_Cut - MVSpeed_Tmp_Cut * AntiSlow_Max / 100f > 80f)
		{
			MVSpeed_Last = MVSpeed_Max * 0.2f;
		}
		else
		{
			MVSpeed_Last = MVSpeed_Max - MVSpeed_Max * ((MVSpeed_Tmp_Cut - MVSpeed_Tmp_Cut * AntiSlow_Max / 100f) / 100f);
		}
		if (ATSpeed_Tmp_Cut - ATSpeed_Tmp_Cut * AntiSlow_Max / 100f > 70f)
		{
			ATSpeed_Last = ATSpeed_Max * 0.3f;
		}
		else
		{
			ATSpeed_Last = ATSpeed_Max - ATSpeed_Max * ((ATSpeed_Tmp_Cut - ATSpeed_Tmp_Cut * AntiSlow_Max / 100f) / 100f);
		}
	}

	public void ApplySkillCastCost(ACT_skillData dt)
	{
		if (dt != null && !(ManaStat == null) && !(HealStat == null))
		{
			float num = Mathf.Max(0f, dt.ManaCost);
			ManaStat.Cur = Mathf.Max(0f, ManaStat.Cur - num);
			if (ManaUse_Rheal > 0 && num > 0f)
			{
				HealStat.Cur = Mathf.Min(HealStat.Max, HealStat.Cur + num * (float)ManaUse_Rheal / 100f);
			}
			if (AT_UseHeal1 && HealStat.Cur > HealStat.Max * 0.05f)
			{
				HealStat.Cur = Mathf.Max(0f, HealStat.Cur - HealStat.Max * 0.01f);
			}
			if (AT_UseHeal2 && HealStat.Cur > HealStat.Max * 0.05f)
			{
				HealStat.Cur = Mathf.Max(0f, HealStat.Cur - HealStat.Max * 0.03f);
			}
		}
	}

	public PlayerManager(int plType)
	{
		PLType = plType;
	}

	public void SetStart()
	{
		mgc.gameObject.SetActive(value: false);
		sqs.gameObject.SetActive(value: false);
		arc.gameObject.SetActive(value: false);
		dead.gameObject.SetActive(value: false);
		switch (PLType)
		{
		case 0:
			mgc.gameObject.SetActive(value: true);
			break;
		case 1:
			sqs.gameObject.SetActive(value: true);
			break;
		case 2:
			arc.gameObject.SetActive(value: true);
			break;
		case 3:
			dead.gameObject.SetActive(value: true);
			break;
		}
	}

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		HealStat = SingletonMonoScope<GameUIManager>.Instance.Health;
		ManaStat = SingletonMonoScope<GameUIManager>.Instance.Mana;
		XpStat = SingletonMonoScope<GameUIManager>.Instance.XP;
		DFXpStat = SingletonMonoScope<GameUIManager>.Instance.DFXP;
		if ((bool)XpStat)
		{
			XpStat.SetForceFullAtMaxPlayerLevel(value: true);
		}
		rigBD = GetComponent<Rigidbody2D>();
		mgc = base.transform.Find("main/MGC").GetComponent<MGC>();
		sqs = base.transform.Find("main/SQS").GetComponent<SQS>();
		arc = base.transform.Find("main/ARC").GetComponent<ARC>();
		dead = base.transform.Find("main/DEAD").GetComponent<DEAD>();
		worldmapCam = base.transform.Find("WorldMap Camera").GetComponent<Camera>();
		minimapCam = base.transform.Find("MiniMap Camera").GetComponent<Camera>();
		headUp = base.transform.Find("main/FX up").gameObject;
		head = base.transform.Find("main/FX head").gameObject;
		body = base.transform.Find("main/FX BD").gameObject;
		yao = base.transform.Find("main/FX yao").gameObject;
		foot = base.transform.transform.Find("shadow").gameObject;
		BuffMG = base.transform.transform.Find("People").GetComponent<BuffMG_PL>();
		BuffRuntime = base.transform.transform.Find("People").GetComponent<PlayerBuffRuntime>();
		if (!BuffRuntime)
		{
			BuffRuntime = base.transform.transform.Find("People").gameObject.AddComponent<PlayerBuffRuntime>();
		}
		xjl = GetComponent<XJL_FSQ>();
		TempleMG = SingletonMonoScope<BuffManager>.Instance;
		CPMG = SingletonMonoScope<CompanionManager>.Instance;
		ACT = SingletonMonoScope<ACTbar>.Instance;
		CacheMouseMoveRuntime();
		EnsureMovementDirectionTarget();
		EnsureCompanionFollowTarget();
		CurrentInputManager.OnCurrentInputDeviceChanged += HandleCurrentInputDeviceChanged;
		InitPlayer();
	}

	public void Start()
	{
		SetStart();
		footCOLemLayerMask = LayerMask.GetMask("FootCOLem");
		blockWallLayerMask = LayerMask.GetMask("blockwall", "blockWALL", "block");
		bodyCOLemLayerMask = LayerMask.GetMask("BodyCOLem");
		CanMove = true;
		HealStat.Initialize(Health + Health * Health_Bei_Last / 100f, Health + Health * Health_Bei_Last / 100f);
		ManaStat.Initialize(Mana + Mana * Mana_Bei_Last / 100f, Mana + Mana * Mana_Bei_Last / 100f);
		if ((bool)XpStat)
		{
			XpStat.Initialize(Xp_CurrentLevel, GetRequiredXpForLevel(Level));
		}
		if ((bool)DFXpStat)
		{
			DFXpStat.Initialize(DFXp_CurrentLevel, GetRequiredDFXpForLevel(DFLevel));
		}
		ChuanSong = false;
		IScomp = false;
		timeLevel = 0f;
		IsLevelUP = false;
		RefreshMouseMoveRuntime();
	}

	public void Update()
	{
		CheckDeathState();
		if (IsAlive)
		{
			UpdateAutoJH();
			RefreshMouseMoveRuntime();
			if (CanMove)
			{
				GetInput();
			}
			JSQ();
		}
		UpdateMovementDirectionTarget();
		RefreshAutoLockTargets();
		mousePosition = AimProvider.GetAimWorldPos();
		UpdateCompanionFollowTarget(Time.deltaTime);
	}

	private void UpdateAutoJH()
	{
		if (!AutoJH || Time.time < autoJHNextCheckTime)
		{
			return;
		}
		autoJHNextCheckTime = Time.time + 0.15f;
		EnsureAutoJHColliders();
		int num = (SingletonMonoScope<InteractionManager>.HasInstance ? SingletonMonoScope<InteractionManager>.Instance.interactLayer.value : SettingsLoader.Instance.interactLayers.value);
		if (num == 0)
		{
			num = SettingsLoader.Instance.interactLayers.value;
		}
		int num2;
		while ((num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, 2f, autoJHColliders, num)) == autoJHColliders.Length)
		{
			Array.Resize(ref autoJHColliders, autoJHColliders.Length * 2);
		}
		for (int i = 0; i < num2; i++)
		{
			Collider2D collider2D = autoJHColliders[i];
			autoJHColliders[i] = null;
			if ((bool)collider2D)
			{
				IInteractable interactable = collider2D.GetComponent<IInteractable>() ?? collider2D.GetComponentInParent<IInteractable>();
				if ((interactable is Chest || interactable is Coffin || interactable is GMcoffin || interactable is Temple) && interactable.CanInteract())
				{
					interactable.Interact();
				}
			}
		}
	}

	private void EnsureAutoJHColliders()
	{
		if (autoJHColliders == null || autoJHColliders.Length == 0)
		{
			autoJHColliders = new Collider2D[5];
		}
	}

	private void UpdateAutoDrink()
	{
		if ((AutoDrinkH || AutoDrinkM) && SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<SimplePotionManager>.HasInstance && SingletonMonoScope<ACTbar>.HasInstance && (!AutoDrinkH || !IsStatBelowPercent(HealStat, 0.2f) || !TryAutoDrinkPotion("health")) && AutoDrinkM && IsStatBelowPercent(ManaStat, 0.1f))
		{
			TryAutoDrinkPotion("mana");
		}
	}

	private static bool IsStatBelowPercent(Stat stat, float percent)
	{
		if (stat != null && stat.Max > 0f)
		{
			return stat.Cur < stat.Max * percent;
		}
		return false;
	}

	private bool TryAutoDrinkPotion(string restoreUseType)
	{
		SlotData slotData = FindBestAutoDrinkPotion("huoli");
		if (slotData == null)
		{
			slotData = FindBestAutoDrinkPotion(restoreUseType);
		}
		if (slotData == null)
		{
			return false;
		}
		SingletonMonoScope<InventoryManager>.Instance.UseItemACT_Use(slotData, -1);
		return true;
	}

	private SlotData FindBestAutoDrinkPotion(string useType)
	{
		if (string.IsNullOrEmpty(useType))
		{
			return null;
		}
		SlotData slotData = null;
		foreach (MainSlotPage mainPage in SingletonMonoScope<InventoryManager>.Instance.MainPages)
		{
			if (mainPage?.MainList == null)
			{
				continue;
			}
			foreach (SlotData main in mainPage.MainList)
			{
				if (IsAutoDrinkPotionSlot(main, useType) && (slotData == null || IsBetterAutoDrinkPotion(main.useitem, slotData.useitem)))
				{
					slotData = main;
				}
			}
		}
		return slotData;
	}

	private static bool IsAutoDrinkPotionSlot(SlotData slot, string useType)
	{
		if (slot == null || !slot.isMain || !slot.isOC || slot.ItemType != 2 || slot.useitem == null)
		{
			return false;
		}
		UseItemClass useitem = slot.useitem;
		if (useitem.InfoType != 0 || useitem.CstackSize <= 0 || useitem.UseType != useType)
		{
			return false;
		}
		return !SingletonMonoScope<SimplePotionManager>.Instance.HasSameDrink(useitem);
	}

	private static bool IsBetterAutoDrinkPotion(UseItemClass candidate, UseItemClass current)
	{
		if (candidate == null)
		{
			return false;
		}
		if (current == null)
		{
			return true;
		}
		if (candidate.Quality != current.Quality)
		{
			return candidate.Quality > current.Quality;
		}
		if (candidate.Level != current.Level)
		{
			return candidate.Level > current.Level;
		}
		if (candidate.Number != current.Number)
		{
			return candidate.Number > current.Number;
		}
		return candidate.Price > current.Price;
	}

	public void FixedUpdate()
	{
		if (CanMove)
		{
			Move();
		}
		else if (!IsChong)
		{
			Direction = Vector2.zero;
			rigBD.velocity = Vector2.zero;
		}
		if (IsAlive)
		{
			ChongUP();
		}
	}

	protected override void OnDestroy()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged -= HandleCurrentInputDeviceChanged;
		RestoreMouseMoveBlockingColliders();
		if ((bool)mouseMoveTargetObject)
		{
			UnityEngine.Object.Destroy(mouseMoveTargetObject);
		}
		base.OnDestroy();
	}

	private void LateUpdate()
	{
		if (IsAlive)
		{
			EnsureSqsMoveAnimation();
			UpdateAnimationSpeed();
		}
	}

	private void EnsureSqsMoveAnimation()
	{
		if (PLType != 1 || !sqs || !sqs.gameObject.activeInHierarchy || sqs.stat == null)
		{
			return;
		}
		if (IsMoving)
		{
			if (sqs.Zheng)
			{
				sqs.walkON();
			}
			else
			{
				sqs.walkBON();
			}
		}
		else
		{
			sqs.idleON();
		}
	}

	public void Move()
	{
		if (IsAlive)
		{
			if (isTeleporting)
			{
				rigBD.velocity = Vector2.zero;
			}
			else
			{
				rigBD.velocity = Direction.normalized * MVSpeed_Last;
			}
		}
	}

	public void GetInput()
	{
		if (isTeleporting)
		{
			Direction = Vector2.zero;
			return;
		}
		Direction = Vector2.zero;
		if (lockMove)
		{
			return;
		}
		bool flag = IsMouseMoveSettingEnabled() && Input.GetMouseButtonDown(0);
		if (flag && SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			SingletonMonoGlobal<CurrentInputManager>.Instance.SetCurrentDevice(InputDeviceType.PC);
		}
		RefreshMouseMoveRuntime();
		if (mouseMoveRuntimeActive)
		{
			HandleMouseMoveInput();
			UpdateMouseMoveDirection();
			return;
		}
		if (flag)
		{
			QueuePendingMouseMoveClickIfAllowed();
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			Vector2 zero = Vector2.zero;
			if (InputBind.Get(ControlAction.Up))
			{
				zero += Vector2.up;
			}
			if (InputBind.Get(ControlAction.Left))
			{
				zero += Vector2.left;
			}
			if (InputBind.Get(ControlAction.Down))
			{
				zero += Vector2.down;
			}
			if (InputBind.Get(ControlAction.Right))
			{
				zero += Vector2.right;
			}
			Direction = zero.normalized;
		}
		else
		{
			float leftStickXRaw = GamepadInputManager.GetLeftStickXRaw();
			float leftStickYRaw = GamepadInputManager.GetLeftStickYRaw();
			Vector2 vector = new Vector2(leftStickXRaw, leftStickYRaw);
			if (vector.magnitude < 0.2f)
			{
				vector = Vector2.zero;
			}
			else
			{
				float num = Mathf.InverseLerp(0.2f, 1f, vector.magnitude);
				vector = vector.normalized * num;
			}
			Direction = Vector2.ClampMagnitude(vector, 1f);
		}
	}

	private void CacheMouseMoveRuntime()
	{
		if (mouseMovePathBehaviours == null)
		{
			mouseMovePathBehaviours = new List<MonoBehaviour>();
		}
		if (mouseMoveBlockingColliders == null)
		{
			mouseMoveBlockingColliders = new List<Collider2D>();
		}
		if (mouseMoveBlockingColliderOriginalEnabled == null)
		{
			mouseMoveBlockingColliderOriginalEnabled = new List<bool>();
		}
		mouseMovePath = GetComponent<AIPath>();
		mouseMoveDestinationSetter = GetComponent<AIDestinationSetter>();
		mouseMoveSeeker = GetComponent<Seeker>();
		mouseMovePathBehaviours.Clear();
		MonoBehaviour[] components = GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if ((bool)monoBehaviour)
			{
				Type type = monoBehaviour.GetType();
				if (type.Namespace != null && type.Namespace.StartsWith("Pathfinding"))
				{
					mouseMovePathBehaviours.Add(monoBehaviour);
				}
			}
		}
		mouseMoveBlockingColliders.Clear();
		mouseMoveBlockingColliderOriginalEnabled.Clear();
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>(includeInactive: true);
		foreach (Collider2D collider2D in componentsInChildren)
		{
			if ((bool)collider2D && !collider2D.isTrigger)
			{
				mouseMoveBlockingColliders.Add(collider2D);
				mouseMoveBlockingColliderOriginalEnabled.Add(collider2D.enabled);
			}
		}
	}

	public void RefreshMouseMoveRuntime()
	{
		if (!mouseMovePath && !mouseMoveDestinationSetter)
		{
			CacheMouseMoveRuntime();
		}
		bool flag = IsMouseMoveSettingEnabled() && AstarPath.active != null && IsAlive && CanMove && !IsChong && !isTeleporting && !lockMove && (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent());
		if (mouseMoveRuntimeInitialized && mouseMoveRuntimeActive == flag)
		{
			if (mouseMoveRuntimeActive)
			{
				SyncMouseMoveSpeed();
			}
			return;
		}
		mouseMoveRuntimeInitialized = true;
		mouseMoveRuntimeActive = flag;
		if (mouseMoveRuntimeActive)
		{
			SetMouseMovePathBehavioursEnabled(enable: true);
			SetMouseMoveBlockingCollidersHidden(hidden: false);
			SyncMouseMoveSpeed();
			if ((bool)mouseMoveDestinationSetter)
			{
				mouseMoveDestinationSetter.target = (mouseMoveHasTarget ? mouseMoveTarget : null);
			}
			if ((bool)mouseMovePath)
			{
				mouseMovePath.canMove = false;
				mouseMovePath.canSearch = mouseMoveHasTarget;
				mouseMovePath.isStopped = !mouseMoveHasTarget;
			}
		}
		else
		{
			bool flag2 = mouseMovePendingClick && IsMouseMoveSettingEnabled() && AstarPath.active == null;
			StopMousePathMovement(clearTarget: true, !flag2);
			SetMouseMovePathBehavioursEnabled(enable: false);
			SetMouseMoveBlockingCollidersHidden(hidden: false);
		}
	}

	private bool IsMouseMoveSettingEnabled()
	{
		if (Singleton<SettingDataManager>.Instance.GetGame() != null)
		{
			return Singleton<SettingDataManager>.Instance.GetGame().mouse_move;
		}
		return false;
	}

	private bool IsForceMoveSettingEnabled()
	{
		if (Singleton<SettingDataManager>.Instance.GetGame() != null)
		{
			return Singleton<SettingDataManager>.Instance.GetGame().QZ_Move;
		}
		return false;
	}

	private bool ShouldUseMousePathMovement()
	{
		if (mouseMoveRuntimeActive)
		{
			return mouseMovePath;
		}
		return false;
	}

	private void HandleCurrentInputDeviceChanged(InputDeviceType deviceType)
	{
		RefreshMouseMoveRuntime();
	}

	private void HandleMouseMoveInput()
	{
		if (TryConsumePendingMouseMoveClick())
		{
			return;
		}
		if (Input.GetMouseButtonUp(0))
		{
			mouseMovePointerStartedOnUi = false;
			mouseMoveStationaryAttackBlocked = false;
			mouseMoveResumeAfterMouseSkill = false;
			return;
		}
		if (mouseMoveResumeAfterMouseSkill && !Input.GetMouseButton(0))
		{
			mouseMoveResumeAfterMouseSkill = false;
		}
		if (ShouldBlockMousePathForStationaryAttack())
		{
			mouseMoveStationaryAttackBlocked = true;
			StopMousePathMovement(clearTarget: true);
		}
		else if (Input.GetMouseButtonDown(0))
		{
			mouseMovePointerStartedOnUi = (bool)EventSystem.current && EventSystem.current.IsPointerOverGameObject();
			if (!mouseMovePointerStartedOnUi)
			{
				mouseMoveStationaryAttackBlocked = false;
				if (ShouldLetInteractionConsumeMouseMoveClick())
				{
					StopMousePathMovement(clearTarget: true);
				}
				else
				{
					SetMouseMoveTarget(AimProvider.GetAimWorldPos(), forceSearch: true);
				}
			}
		}
		else if (Input.GetMouseButton(0) && mouseMovePointerStartedOnUi)
		{
			if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject())
			{
				mouseMovePointerStartedOnUi = false;
				if (ShouldBlockMousePathForStationaryAttack())
				{
					mouseMoveStationaryAttackBlocked = true;
					StopMousePathMovement(clearTarget: true);
				}
				else if (ShouldLetInteractionConsumeMouseMoveClick())
				{
					StopMousePathMovement(clearTarget: true);
				}
				else
				{
					mouseMoveStationaryAttackBlocked = false;
					SetMouseMoveTarget(AimProvider.GetAimWorldPos(), !mouseMoveHasTarget);
				}
			}
		}
		else if (Input.GetMouseButton(0) && !mouseMovePointerStartedOnUi && (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject()))
		{
			if (ShouldLetInteractionConsumeMouseMoveClick())
			{
				StopMousePathMovement(clearTarget: true);
				return;
			}
			mouseMoveStationaryAttackBlocked = false;
			SetMouseMoveTarget(AimProvider.GetAimWorldPos(), !mouseMoveHasTarget);
		}
	}

	private void QueuePendingMouseMoveClickIfAllowed()
	{
		if ((!EventSystem.current || !EventSystem.current.IsPointerOverGameObject()) && !ShouldBlockMousePathForStationaryAttack() && !ShouldLetInteractionConsumeMouseMoveClick())
		{
			Vector3 aimWorldPos = AimProvider.GetAimWorldPos();
			if (!IsMouseMoveTargetTooClose(aimWorldPos))
			{
				mouseMovePendingClick = true;
				mouseMovePendingClickPosition = aimWorldPos;
				mouseMovePendingClickTime = Time.unscaledTime;
			}
		}
	}

	private bool TryConsumePendingMouseMoveClick()
	{
		if (!mouseMovePendingClick)
		{
			return false;
		}
		mouseMovePendingClick = false;
		if (Time.unscaledTime - mouseMovePendingClickTime > 1f)
		{
			return false;
		}
		SetMouseMoveTarget(mouseMovePendingClickPosition, forceSearch: true);
		return true;
	}

	private bool ShouldBlockMousePathForStationaryAttack()
	{
		if (!Input.GetMouseButton(0))
		{
			return false;
		}
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			return true;
		}
		if (IsForceMoveSettingEnabled())
		{
			return false;
		}
		return IsPointerOverEnemyBodyCollider();
	}

	public void PauseMousePathForHeldMouseSkill(ControlAction action)
	{
		if (Input.GetMouseButton(0) && (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent()) && string.Equals(InputBind.GetBindKeyName(action), "Mouse0", StringComparison.OrdinalIgnoreCase))
		{
			mouseMoveResumeAfterMouseSkill = true;
		}
	}

	private bool IsPointerOverEnemyBodyCollider()
	{
		if (mouseMovePointerHits == null || mouseMovePointerHits.Length == 0)
		{
			mouseMovePointerHits = new Collider2D[8];
		}
		int num = ((bodyCOLemLayerMask != 0) ? bodyCOLemLayerMask : LayerMask.GetMask("BodyCOLem"));
		if (num == 0)
		{
			return false;
		}
		int num2 = Physics2D.OverlapPointNonAlloc(AimProvider.GetAimWorldPos(), mouseMovePointerHits, num);
		for (int i = 0; i < num2; i++)
		{
			Collider2D collider2D = mouseMovePointerHits[i];
			mouseMovePointerHits[i] = null;
			if ((bool)collider2D)
			{
				BodyCOL component = collider2D.GetComponent<BodyCOL>();
				if ((bool)component && component.peo != null && component.peo.CharacterType == 2 && component.peo.em != null && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool ShouldLetInteractionConsumeMouseMoveClick()
	{
		if (!SingletonMonoScope<InteractionManager>.HasInstance || !InteractionManager.IsCursorMode)
		{
			return false;
		}
		if (!InteractionManager.CanInteractNow())
		{
			return false;
		}
		IInteractable currentTarget = SingletonMonoScope<InteractionManager>.Instance.CurrentTarget;
		if (currentTarget == null || !currentTarget.CanInteract())
		{
			return false;
		}
		return ShouldPrioritizeInteractableOverMouseMove(currentTarget);
	}

	private static bool ShouldPrioritizeInteractableOverMouseMove(IInteractable target)
	{
		if (target == null)
		{
			return false;
		}
		if (target.Type != 0)
		{
			return target.Type != InteractionType.Enemy;
		}
		return false;
	}

	private void SetMouseMoveTarget(Vector3 worldPos, bool forceSearch)
	{
		if (IsMouseMoveTargetTooClose(worldPos))
		{
			StopMousePathMovement(clearTarget: true);
			return;
		}
		EnsureMouseMoveTarget();
		worldPos.z = base.transform.position.z;
		mouseMoveTarget.position = worldPos;
		mouseMoveTargetObject.SetActive(value: true);
		mouseMoveHasTarget = true;
		if ((bool)mouseMoveDestinationSetter)
		{
			mouseMoveDestinationSetter.enabled = true;
			mouseMoveDestinationSetter.target = mouseMoveTarget;
		}
		if (!mouseMovePath)
		{
			return;
		}
		SyncMouseMoveSpeed();
		mouseMovePath.enabled = true;
		mouseMovePath.canMove = false;
		mouseMovePath.canSearch = true;
		mouseMovePath.isStopped = false;
		mouseMovePath.destination = worldPos;
		if (forceSearch)
		{
			mouseMoveWaitingForPath = true;
			mouseMovePathWaitStartedTime = Time.unscaledTime;
			if (AstarPath.active != null)
			{
				mouseMovePath.SearchPath();
			}
		}
	}

	private void EnsureMouseMoveTarget()
	{
		if (!mouseMoveTargetObject)
		{
			mouseMoveTargetObject = new GameObject("Player Mouse Move Target");
			mouseMoveTarget = mouseMoveTargetObject.transform;
			mouseMoveTargetObject.SetActive(value: false);
		}
	}

	private bool IsMouseMoveTargetTooClose(Vector3 worldPos)
	{
		Vector3 mouseMoveFootPosition = GetMouseMoveFootPosition();
		worldPos.z = mouseMoveFootPosition.z;
		return ((Vector2)worldPos - (Vector2)mouseMoveFootPosition).sqrMagnitude <= 0.09f;
	}

	private Vector3 GetMouseMoveFootPosition()
	{
		if (!foot)
		{
			return base.transform.position;
		}
		return foot.transform.position;
	}

	private void UpdateMouseMoveDirection()
	{
		SyncMouseMoveSpeed();
		if (!mouseMoveHasTarget || !mouseMovePath)
		{
			Direction = Vector2.zero;
			return;
		}
		if (!mouseMoveTarget || !mouseMoveTargetObject)
		{
			StopMousePathMovement(clearTarget: true);
			return;
		}
		mouseMovePath.MovementUpdate(0f, out var _, out var _);
		if (Vector2.Distance(base.transform.position, mouseMoveTarget.position) <= 0.05f || mouseMovePath.reachedDestination || (!mouseMovePath.pathPending && mouseMovePath.reachedEndOfPath))
		{
			StopMousePathMovement(clearTarget: true);
			return;
		}
		if (TryGetDirectMouseMoveDirection(out var direction))
		{
			mouseMoveWaitingForPath = false;
			Direction = direction;
			return;
		}
		if (mouseMovePath.hasPath)
		{
			mouseMoveWaitingForPath = false;
		}
		if ((mouseMovePath.pathPending || mouseMoveWaitingForPath) && !mouseMovePath.hasPath)
		{
			if (!mouseMoveWaitingForPath || !(Time.unscaledTime - mouseMovePathWaitStartedTime > 0.5f) || mouseMovePath.pathPending)
			{
				Direction = Vector2.zero;
				return;
			}
			mouseMoveWaitingForPath = false;
		}
		bool hasPath = mouseMovePath.hasPath;
		if (!hasPath && !mouseMovePath.pathPending && !mouseMoveWaitingForPath)
		{
			StopMousePathMovement(clearTarget: true);
			return;
		}
		Vector2 vector = (Vector2)(hasPath ? mouseMovePath.steeringTarget : mouseMoveTarget.position) - (Vector2)base.transform.position;
		if (vector.sqrMagnitude > 0.0001f)
		{
			Direction = vector.normalized;
		}
		else
		{
			Direction = ((Vector2)mouseMoveTarget.position - (Vector2)base.transform.position).normalized;
		}
	}

	private bool TryGetDirectMouseMoveDirection(out Vector2 direction)
	{
		direction = Vector2.zero;
		if (!mouseMoveTarget)
		{
			return false;
		}
		Vector2 vector = base.transform.position;
		Vector2 vector2 = (Vector2)mouseMoveTarget.position - vector;
		float magnitude = vector2.magnitude;
		if (magnitude <= 0.0001f)
		{
			return false;
		}
		Vector2 vector3 = vector2 / magnitude;
		if ((bool)Physics2D.Raycast(vector, vector3, magnitude, LayerMask.GetMask("block")).collider)
		{
			return false;
		}
		direction = vector3;
		return true;
	}

	private void SyncMouseMoveSpeed()
	{
		if ((bool)mouseMovePath)
		{
			mouseMovePath.maxSpeed = MVSpeed_Last;
		}
	}

	private void StopMousePathMovement(bool clearTarget, bool clearPendingClick = true)
	{
		if ((bool)mouseMovePath)
		{
			mouseMovePath.destination = base.transform.position;
			mouseMovePath.SetPath(null);
			if (AstarPath.active != null)
			{
				mouseMovePath.Teleport(base.transform.position, clearPath: false);
			}
			mouseMovePath.canMove = false;
			mouseMovePath.isStopped = true;
		}
		if ((bool)mouseMoveSeeker)
		{
			mouseMoveSeeker.CancelCurrentPathRequest();
		}
		if (clearTarget)
		{
			mouseMoveHasTarget = false;
			mouseMoveWaitingForPath = false;
			if (clearPendingClick)
			{
				mouseMovePendingClick = false;
			}
			if ((bool)mouseMoveDestinationSetter)
			{
				mouseMoveDestinationSetter.target = null;
			}
			if ((bool)mouseMoveTargetObject)
			{
				mouseMoveTargetObject.SetActive(value: false);
			}
		}
		Direction = Vector2.zero;
		mouseMovePointerStartedOnUi = false;
		if (!Input.GetMouseButton(0))
		{
			mouseMoveStationaryAttackBlocked = false;
			mouseMoveResumeAfterMouseSkill = false;
		}
		if ((bool)rigBD)
		{
			rigBD.velocity = Vector2.zero;
		}
	}

	private void SetMouseMovePathBehavioursEnabled(bool enable)
	{
		if (mouseMovePathBehaviours == null)
		{
			return;
		}
		for (int i = 0; i < mouseMovePathBehaviours.Count; i++)
		{
			MonoBehaviour monoBehaviour = mouseMovePathBehaviours[i];
			if ((bool)monoBehaviour)
			{
				monoBehaviour.enabled = enable;
			}
		}
	}

	private void SetMouseMoveBlockingCollidersHidden(bool hidden)
	{
		if (mouseMoveBlockingColliders == null || mouseMoveBlockingColliderOriginalEnabled == null)
		{
			return;
		}
		for (int i = 0; i < mouseMoveBlockingColliders.Count; i++)
		{
			Collider2D collider2D = mouseMoveBlockingColliders[i];
			if ((bool)collider2D)
			{
				bool flag = ((i < mouseMoveBlockingColliderOriginalEnabled.Count) ? mouseMoveBlockingColliderOriginalEnabled[i] : collider2D.enabled);
				collider2D.enabled = !hidden && flag;
			}
		}
	}

	private void RestoreMouseMoveBlockingColliders()
	{
		SetMouseMoveBlockingCollidersHidden(hidden: false);
	}

	public IEnumerator TeleportRoutine(Vector2 pos)
	{
		isTeleporting = true;
		StopMousePathMovement(clearTarget: true);
		rigBD.velocity = Vector2.zero;
		Direction = Vector2.zero;
		if (SingletonMonoScene<EnemyPointManager>.HasInstance)
		{
			SingletonMonoScene<EnemyPointManager>.Instance.PrewarmForTeleport(pos, 1f);
		}
		rigBD.position = pos;
		Physics2D.SyncTransforms();
		yield return null;
		isTeleporting = false;
	}

	public void ChongUP()
	{
		if (IsChong)
		{
			CanMove = false;
			ChongSpeed = 7f * SkillSpeed_Max;
			Direction = GetMovementSkillWorldPoint(1f) - base.transform.position;
			rigBD.velocity = Direction.normalized * ChongSpeed;
		}
	}

	public void ClearAllTargets()
	{
		em.Clear();
		cp.Clear();
		ClearAutoLockTargets();
	}

	public bool IsAutoLockActive()
	{
		return Singleton<SettingDataManager>.Instance.IsAutoLockActiveForCurrentInput();
	}

	public bool IsGamepadAutoLockActive()
	{
		return Singleton<SettingDataManager>.Instance.GetGame()?.auto_lock1 ?? false;
	}

	public bool TryGetAutoLockYaoPosition(out Vector3 position)
	{
		if (!autoLockYaoTarget && !autoLockRefreshInProgress)
		{
			RefreshAutoLockTargets();
		}
		if ((bool)autoLockYaoTarget)
		{
			position = autoLockYaoTarget.position;
			position.z = 0f;
			return true;
		}
		position = Vector3.zero;
		return false;
	}

	public bool TryGetAutoLockFootPosition(out Vector3 position)
	{
		if (!autoLockFootTarget && !autoLockRefreshInProgress)
		{
			RefreshAutoLockTargets();
		}
		if ((bool)autoLockFootTarget)
		{
			position = autoLockFootTarget.position;
			position.z = 0f;
			return true;
		}
		position = Vector3.zero;
		return false;
	}

	public Vector3 GetBattleAimWorldPosition(Vector3 fallbackAim, Transform currentTar, Transform tar2)
	{
		if (IsChong)
		{
			return GetMovementSkillWorldPoint(1f);
		}
		if (IsAutoLockActive() && TryGetAutoLockYaoPosition(out var position))
		{
			return position;
		}
		return fallbackAim;
	}

	public Vector3 GetMovementSkillWorldPoint(float distance)
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return AimProvider.GetPointerWorldPos();
		}
		Vector2 currentMovementAimDirection = GetCurrentMovementAimDirection();
		return UpdateMovementDirectionTargetPosition(currentMovementAimDirection, distance);
	}

	public Vector3 GetCompanionFollowBasePosition()
	{
		EnsureCompanionFollowTarget();
		if (!companionFollowPoint)
		{
			return base.transform.position;
		}
		return companionFollowPoint.position;
	}

	public void NotifyCompanionFollowSkillFired(Vector3 aimWorldPosition)
	{
		aimWorldPosition.z = 0f;
		Vector2 to = aimWorldPosition - base.transform.position;
		if (to.sqrMagnitude <= 0.0001f)
		{
			return;
		}
		to.Normalize();
		if (!hasLastCompanionFollowSkillAim || companionFollowBattleCount <= 0)
		{
			companionFollowBattleCount = 1;
			companionFollowAttackPush = 1f;
			companionFollowDirectionalPush = 0f;
			lastCompanionFollowSkillAim = aimWorldPosition;
			lastCompanionFollowSkillDirection = to;
			hasLastCompanionFollowSkillAim = true;
			return;
		}
		Vector2 from = lastCompanionFollowSkillDirection;
		if (from.sqrMagnitude <= 0.0001f)
		{
			from = lastCompanionFollowSkillAim - base.transform.position;
		}
		if (from.sqrMagnitude <= 0.0001f)
		{
			companionFollowBattleCount = 1;
			companionFollowAttackPush = 1f;
			companionFollowDirectionalPush = 0f;
			lastCompanionFollowSkillAim = aimWorldPosition;
			lastCompanionFollowSkillDirection = to;
			hasLastCompanionFollowSkillAim = true;
			return;
		}
		from.Normalize();
		float num = Vector2.Angle(from, to);
		if (num >= 50f)
		{
			companionFollowBattleCount = 0;
			companionFollowAttackPush = 0f;
			companionFollowDirectionalPush = 0f;
			lastCompanionFollowSkillAim = aimWorldPosition;
			lastCompanionFollowSkillDirection = to;
			hasLastCompanionFollowSkillAim = true;
			return;
		}
		companionFollowAttackPush = 1f;
		if (num <= 20f)
		{
			companionFollowBattleCount = Mathf.Min(2, companionFollowBattleCount + 1);
			companionFollowDirectionalPush = 1f;
		}
		else
		{
			companionFollowBattleCount = Mathf.Clamp(companionFollowBattleCount, 1, 2);
			companionFollowDirectionalPush = 0f;
		}
		lastCompanionFollowSkillAim = aimWorldPosition;
		lastCompanionFollowSkillDirection = to;
		hasLastCompanionFollowSkillAim = true;
	}

	private void EnsureMovementDirectionTarget()
	{
		if (!movementDirectionTarget)
		{
			movementDirectionTargetObject = new GameObject("MovementDirectionTarget");
			movementDirectionTargetObject.transform.SetParent(base.transform);
			movementDirectionTargetObject.transform.localPosition = Vector3.right;
			movementDirectionTarget = movementDirectionTargetObject.transform;
		}
	}

	private void UpdateMovementDirectionTarget()
	{
		Vector2 currentMovementAimDirection = GetCurrentMovementAimDirection();
		UpdateMovementDirectionTargetPosition(currentMovementAimDirection, 1f);
	}

	private Vector3 UpdateMovementDirectionTargetPosition(Vector2 dir, float distance)
	{
		EnsureMovementDirectionTarget();
		float num = Mathf.Max(0.1f, distance);
		Vector3 vector = base.transform.position + new Vector3(dir.x, dir.y, 0f) * num;
		vector.z = 0f;
		if ((bool)movementDirectionTarget)
		{
			movementDirectionTarget.position = vector;
		}
		return vector;
	}

	private Vector2 GetCurrentMovementAimDirection()
	{
		Vector2 vector = Vector2.zero;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return GetCurrentGamepadLeftStickAimDirection();
		}
		if (mouseMoveRuntimeActive && mouseMoveHasTarget && (bool)mouseMoveTarget)
		{
			vector = mouseMoveTarget.position - base.transform.position;
		}
		else if (Direction.sqrMagnitude > 0.0001f)
		{
			vector = Direction;
		}
		else
		{
			if (InputBind.Get(ControlAction.Up))
			{
				vector += Vector2.up;
			}
			if (InputBind.Get(ControlAction.Left))
			{
				vector += Vector2.left;
			}
			if (InputBind.Get(ControlAction.Down))
			{
				vector += Vector2.down;
			}
			if (InputBind.Get(ControlAction.Right))
			{
				vector += Vector2.right;
			}
		}
		if (vector.sqrMagnitude > 0.0001f)
		{
			lastMovementAimDirection = vector.normalized;
		}
		if (!(lastMovementAimDirection.sqrMagnitude > 0.0001f))
		{
			return Vector2.right;
		}
		return lastMovementAimDirection;
	}

	private Vector2 GetCurrentGamepadLeftStickAimDirection()
	{
		Vector2 vector = new Vector2(GamepadInputManager.GetLeftStickXRaw(), GamepadInputManager.GetLeftStickYRaw());
		if (vector.magnitude >= 0.2f)
		{
			lastGamepadLeftStickAimDirection = vector.normalized;
		}
		if (!(lastGamepadLeftStickAimDirection.sqrMagnitude > 0.0001f))
		{
			return Vector2.right;
		}
		return lastGamepadLeftStickAimDirection;
	}

	private void EnsureCompanionFollowTarget()
	{
		if (!companionFollow)
		{
			companionFollow = base.transform.Find("Follow");
			if (!companionFollow)
			{
				GameObject gameObject = new GameObject("Follow");
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				companionFollow = gameObject.transform;
			}
		}
		if (!companionFollowPoint)
		{
			companionFollowPoint = companionFollow.Find("point");
			if (!companionFollowPoint)
			{
				GameObject gameObject2 = new GameObject("point");
				gameObject2.transform.SetParent(companionFollow);
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				companionFollowPoint = gameObject2.transform;
			}
		}
	}

	private void UpdateCompanionFollowTarget(float dt)
	{
		EnsureCompanionFollowTarget();
		float num = 1f / 3f;
		companionFollowAttackPush = Mathf.MoveTowards(companionFollowAttackPush, 0f, num * dt);
		companionFollowDirectionalPush = Mathf.MoveTowards(companionFollowDirectionalPush, 0f, num * dt);
		if (companionFollowAttackPush <= 0f && companionFollowDirectionalPush <= 0f)
		{
			companionFollowBattleCount = 0;
		}
		if (companionFollow.parent == base.transform)
		{
			companionFollow.localPosition = Vector3.zero;
		}
		else
		{
			companionFollow.position = base.transform.position;
		}
		hasNearbyCompanionFollowEnemy = HasNearbyCompanionFollowEnemy();
		bool flag = HasCurrentMovementForCompanionFollow();
		if (!hasNearbyCompanionFollowEnemy && !flag)
		{
			companionFollowPoint.localPosition = Vector3.zero;
			companionFollowBattleCount = 0;
			hasLastCompanionFollowSkillAim = false;
			companionFollowAttackPush = 0f;
			companionFollowDirectionalPush = 0f;
			lastCompanionFollowSkillDirection = Vector2.zero;
		}
		else
		{
			Vector2 currentAimDirectionForCompanionFollow = GetCurrentAimDirectionForCompanionFollow();
			float z = Mathf.Atan2(currentAimDirectionForCompanionFollow.y, currentAimDirectionForCompanionFollow.x) * 57.29578f;
			companionFollow.rotation = Quaternion.Euler(0f, 0f, z);
			float x = 1.5f + companionFollowAttackPush + companionFollowDirectionalPush + GetCompanionFollowMovementPull(currentAimDirectionForCompanionFollow);
			companionFollowPoint.localPosition = new Vector3(x, 0f, 0f);
		}
	}

	private bool HasNearbyCompanionFollowEnemy()
	{
		EnsureEnemyRangeBuffers();
		if (companionFollowEnemyBuffer == null)
		{
			return false;
		}
		CollectEnemiesInRange(7f, companionFollowEnemyBuffer, onlyNormalEnemy: false);
		for (int i = 0; i < companionFollowEnemyBuffer.Count; i++)
		{
			if (IsAutoLockEnemyValid(companionFollowEnemyBuffer[i]))
			{
				return true;
			}
		}
		return false;
	}

	private Vector2 GetCurrentAimDirectionForCompanionFollow()
	{
		if (IsAutoLockActive())
		{
			if (TryGetAutoLockYaoPosition(out var position))
			{
				Vector2 vector = position - base.transform.position;
				if (vector.sqrMagnitude > 0.0001f)
				{
					return vector.normalized;
				}
			}
			Vector2 currentMovementAimDirection = GetCurrentMovementAimDirection();
			if (currentMovementAimDirection.sqrMagnitude > 0.0001f)
			{
				return currentMovementAimDirection.normalized;
			}
			if (!companionFollow)
			{
				return Vector2.right;
			}
			return companionFollow.right;
		}
		Vector2 vector2 = AimProvider.GetAimWorldPos() - base.transform.position;
		if (vector2.sqrMagnitude <= 0.0001f)
		{
			vector2 = (companionFollow ? ((Vector2)companionFollow.right) : Vector2.right);
		}
		if (!(vector2.sqrMagnitude > 0.0001f))
		{
			return Vector2.right;
		}
		return vector2.normalized;
	}

	private bool HasCurrentMovementForCompanionFollow()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent() && new Vector2(GamepadInputManager.GetLeftStickXRaw(), GamepadInputManager.GetLeftStickYRaw()).magnitude >= 0.2f)
		{
			return true;
		}
		if (mouseMoveRuntimeActive && mouseMoveHasTarget && (bool)mouseMoveTarget)
		{
			return true;
		}
		if (Direction.sqrMagnitude > 0.0001f)
		{
			return true;
		}
		if (!InputBind.Get(ControlAction.Up) && !InputBind.Get(ControlAction.Left) && !InputBind.Get(ControlAction.Down))
		{
			return InputBind.Get(ControlAction.Right);
		}
		return true;
	}

	private float GetCompanionFollowMovementPull(Vector2 followDir)
	{
		Vector2 currentMovementAimDirection = GetCurrentMovementAimDirection();
		if (currentMovementAimDirection.sqrMagnitude <= 0.0001f || followDir.sqrMagnitude <= 0.0001f)
		{
			return 0f;
		}
		float num = Vector2.Angle(currentMovementAimDirection, followDir);
		if (num > 130f)
		{
			return -1.2f;
		}
		if (num >= 100f)
		{
			return -0.6f;
		}
		return 0f;
	}

	private void RefreshAutoLockTargets()
	{
		if (autoLockRefreshInProgress)
		{
			return;
		}
		autoLockRefreshInProgress = true;
		try
		{
			if (!IsAutoLockActive())
			{
				ClearAutoLockTargets();
				return;
			}
			Enemy enemy = FindNearestAutoLockEnemy();
			if (!IsAutoLockEnemyValid(enemy))
			{
				ClearAutoLockTargets();
				return;
			}
			autoLockFootTarget = enemy.transform;
			autoLockYaoTarget = (enemy.yao ? enemy.yao.transform : enemy.transform);
		}
		finally
		{
			autoLockRefreshInProgress = false;
		}
	}

	private Enemy FindNearestAutoLockEnemy()
	{
		EnsureEnemyRangeBuffers();
		CollectEnemiesInRange(5f, autoLockEnemyBuffer, onlyNormalEnemy: false, requireLineOfSight: true);
		Enemy result = null;
		float num = float.MaxValue;
		for (int i = 0; i < autoLockEnemyBuffer.Count; i++)
		{
			Enemy enemy = autoLockEnemyBuffer[i];
			if (IsAutoLockEnemyValid(enemy))
			{
				float sqrMagnitude = (enemy.transform.position - base.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = enemy;
				}
			}
		}
		return result;
	}

	private bool IsAutoLockEnemyValid(Enemy enemy)
	{
		if ((bool)enemy && enemy.IsAlive && !enemy.IsJump && !enemy.IsYS)
		{
			return !IsEnemyBlockedByWall(enemy);
		}
		return false;
	}

	private bool IsEnemyBlockedByWall(Enemy enemy)
	{
		if (!enemy)
		{
			return true;
		}
		Vector2 vector = base.transform.position;
		Vector2 vector2 = (Vector2)enemy.transform.position - vector;
		float magnitude = vector2.magnitude;
		if (magnitude <= 0.0001f)
		{
			return false;
		}
		int num = ((blockWallLayerMask != 0) ? blockWallLayerMask : LayerMask.GetMask("blockwall", "blockWALL", "block"));
		if (num == 0)
		{
			return false;
		}
		return Physics2D.Raycast(vector, vector2 / magnitude, magnitude, num).collider;
	}

	private void EnsureEnemyRangeBuffers()
	{
		if (enemyRangeHits == null || enemyRangeHits.Length == 0)
		{
			enemyRangeHits = new Collider2D[10];
		}
		if (burnLifeEnemyBuffer == null)
		{
			burnLifeEnemyBuffer = new List<Enemy>(50);
		}
		if (deathExplosionEnemyBuffer == null)
		{
			deathExplosionEnemyBuffer = new List<Enemy>(50);
		}
		if (dotBuffEnemyBuffer == null)
		{
			dotBuffEnemyBuffer = new List<Enemy>(50);
		}
		if (autoLockEnemyBuffer == null)
		{
			autoLockEnemyBuffer = new List<Enemy>(20);
		}
		if (nearbyEnemyStatBuffer == null)
		{
			nearbyEnemyStatBuffer = new List<Enemy>(50);
		}
		if (companionFollowEnemyBuffer == null)
		{
			companionFollowEnemyBuffer = new List<Enemy>(30);
		}
	}

	private void ClearAutoLockTargets()
	{
		autoLockYaoTarget = null;
		autoLockFootTarget = null;
		if (autoLockEnemyBuffer != null)
		{
			autoLockEnemyBuffer.Clear();
		}
	}

	public void JSQ()
	{
		em.RemoveAll((Enemy e) => !e);
		cp.RemoveAll((Companion c) => !c);
		if (IsLevelUP)
		{
			timeLevel += Time.deltaTime;
			if (timeLevel >= 0.5f)
			{
				timeLevel = 0f;
				IsLevelUP = false;
			}
		}
		if (IsAlive)
		{
			TimeA += Time.deltaTime;
			if (TimeA >= 1f)
			{
				HealStat.Cur += Health_R_Max;
				ManaStat.Cur += Mana_R_Max;
				if (BloodLost && HealStat.Cur > HealStat.Max / 2f)
				{
					HealStat.Cur -= HealStat.Max / 3f;
				}
				ApplyBurnLife();
				RefreshNearbyDotBuffDamage();
				TimeA = 0f;
			}
		}
		TimeB += Time.deltaTime;
		if (TimeB >= 0.2f)
		{
			RefreshNearbyEnemyStatCounts();
			RefreshRuntimeDerivedStats();
			UpdateAutoDrink();
			em.RemoveAll((Enemy e) => !e || !e.IsAlive || Vector2.Distance(base.transform.position, e.transform.position) > EM_Range || IsEnemyBlockedByWall(e));
			em.Sort(delegate(Enemy a, Enemy b)
			{
				if (!a && !b)
				{
					return 0;
				}
				if (!a)
				{
					return 1;
				}
				return b ? Vector3.Distance(a.transform.position, base.transform.position).CompareTo(Vector3.Distance(b.transform.position, base.transform.position)) : (-1);
			});
			int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, EM_Range, hitEM, LayerMask.GetMask("FootCOLem"));
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					FootCOL component = hitEM[i].GetComponent<FootCOL>();
					if ((bool)component)
					{
						if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS && !IsEnemyBlockedByWall(component.peo.em))
						{
							em.Add(component.peo.em);
						}
						hitEM[i] = null;
					}
				}
			}
			int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, Pick_PL_Max, DPIT, LayerMask.GetMask("AutoPick"));
			if (num2 > 0)
			{
				for (int j = 0; j < num2; j++)
				{
					DropItemController component2 = DPIT[j].GetComponent<DropItemController>();
					if (component2.CanAutoPick)
					{
						if (component2.ItemType > 0)
						{
							component2.AutoPickUp();
						}
						else
						{
							switch (SingletonMonoGlobal<FilterManager>.Instance.PLPick)
							{
							case QulityType.Filter_Normal:
								component2.AutoPickUp();
								break;
							case QulityType.Filter_Magic:
								if (component2.weapon.Quality > 0)
								{
									component2.AutoPickUp();
								}
								break;
							case QulityType.Filter_Rare:
								if (component2.weapon.Quality > 1)
								{
									component2.AutoPickUp();
								}
								break;
							case QulityType.Filter_Exquisite:
								if (component2.weapon.Quality > 2)
								{
									component2.AutoPickUp();
								}
								break;
							case QulityType.Filter_Epic:
								if (component2.weapon.Quality > 3)
								{
									component2.AutoPickUp();
								}
								break;
							case QulityType.Filter_Legendary:
								if (component2.weapon.Quality > 4)
								{
									component2.AutoPickUp();
								}
								break;
							case QulityType.Filter_Mythical:
								if (component2.weapon.Quality > 5)
								{
									component2.AutoPickUp();
								}
								break;
							}
						}
					}
					DPIT[j] = null;
				}
			}
			TimeB = 0f;
		}
		TimeC += Time.deltaTime;
		if (TimeC >= 3f)
		{
			TriggerEnemyDotExplosions();
			TimeC = 0f;
		}
	}

	private void TriggerEnemyDotExplosions()
	{
		if (em == null || em.Count <= 0)
		{
			return;
		}
		for (int num = em.Count - 1; num >= 0; num--)
		{
			Enemy enemy = em[num];
			if ((bool)enemy && enemy.IsAlive && !enemy.IsJump && !enemy.IsYS && !(enemy.peo == null) && !(enemy.peo.DotEM == null))
			{
				enemy.peo.DotEM.DotYB();
			}
		}
	}

	private float GetElementCut(DamageType type)
	{
		return type switch
		{
			DamageType.fire => FireCut, 
			DamageType.frozen => FrozenCut, 
			DamageType.thunder => ThunderCut, 
			DamageType.poison => PoisonCut, 
			DamageType.physics => PhysicsCut, 
			DamageType.shadow => ShadowCut, 
			_ => 0f, 
		};
	}

	private float CalculateFinalDamage(float damage, float anti, float chuan)
	{
		float a = anti - chuan;
		a = Mathf.Max(a, -80f);
		float num = 100f / (100f + a);
		float num2 = damage * num;
		float num3 = Mathf.Clamp(Damage_Anti_Max, 0f, 95f);
		return Mathf.Max(num2 * (1f - num3 / 100f), 0f);
	}

	public void TakeDamage(float damage, float chuan, float bJrate, float bJDamage, DamageType type, Enemy enemy)
	{
		if ((bool)BuffRuntime && BuffRuntime.IsWuDi())
		{
			return;
		}
		float num = Mathf.Clamp(bJrate, 0f, 100f);
		float num2 = 0f;
		if (!NoGD)
		{
			num2 = Mathf.Clamp(GeDang_Max, 0f, 100f);
		}
		if (!LowH_CritAnti10 && UnityEngine.Random.value < num * 0.01f)
		{
			float num3 = Mathf.Max(0f, bJDamage);
			damage *= 2f + num3 / 100f;
			damage *= (100f - Mathf.Clamp(BJD_Anti + XJL_BJD_Anti_Tmp, 0f, 100f)) / 100f;
		}
		if (UnityEngine.Random.value < num2 * 0.01f)
		{
			BuffRuntime?.OnBlock();
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.CreatACT_Hurt(GD_HurtR);
				SingletonMonoScope<ACTbar>.Instance.TryGDUseSkills();
				SingletonMonoScope<ACTbar>.Instance.CreatACT_GD();
			}
			if (!enemy || !enemy.IsAlive || enemy.IsYS || enemy.IsJump)
			{
				return;
			}
			EnsurePlayerDotData();
			for (int i = 0; i < DOT.Length; i++)
			{
				if (DOT[i].FJ > 0 && UnityEngine.Random.Range(0, 101) < DOT[i].FJ)
				{
					DamageType type2 = SWS.DMtype(i);
					ACT_DOT dot = SingletonMonoScope<ACTbar>.Instance.GiveDot(type2);
					enemy.peo.DotEM.AddDot(type2, dot, 1);
				}
			}
			return;
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.CreatACT_Hurt(0);
		}
		float elementCut = GetElementCut(type);
		float num4 = CalculateFinalDamage(damage, elementCut, chuan);
		if ((bool)enemy && enemy.Quality > 2)
		{
			num4 -= num4 * (float)JYBoss_Anti / 100f;
		}
		float num5 = 0f;
		num5 += GetB_DMG_Hurt;
		if (HealCutMana)
		{
			num5 += 10f;
		}
		num4 += num4 * num5 / 100f;
		if (Turtle)
		{
			if (num4 > HealStat.Max * 0.03f)
			{
				if (num4 > HealStat.Max * 0.4f)
				{
					if (TryPreventRuntimeFatalDamage(num4, 2, enemy))
					{
						return;
					}
					BuffRuntime?.OnPlayerHit();
					CutHealth(num4, 2, type, enemy);
					SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num4 / 2f, type, crit: false);
				}
				else
				{
					if (TryPreventRuntimeFatalDamage(num4, 1, enemy))
					{
						return;
					}
					BuffRuntime?.OnPlayerHit();
					CutHealth(num4, 1, type, enemy);
					SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num4, type, crit: false);
				}
			}
		}
		else
		{
			if (TryPreventRuntimeFatalDamage(num4, 1, enemy))
			{
				return;
			}
			BuffRuntime?.OnPlayerHit();
			CutHealth(num4, 1, type, enemy);
			SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num4, type, crit: false);
		}
		if (FT && (bool)enemy)
		{
			enemy.TakeDamage(HealStat.Max * 0.1f, FireChuan_Last, BJrate_Last, BJDamage_Last, 0f, 0f, 100f, DamageType.fire, 0, this, null);
		}
		if (UnityEngine.Random.value < 0.05f && SO_Hurt != null && PLType >= 0 && PLType < SO_Hurt.Length && yao != null)
		{
			RuntimeManager.PlayOneShot(SO_Hurt[PLType], yao.transform.position);
		}
	}

	public void TakeDotDamage(DamageType type, float damage, float chuan)
	{
		if (!BuffRuntime || !BuffRuntime.IsWuDi())
		{
			float elementCut = GetElementCut(type);
			float num = CalculateFinalDamage(damage, elementCut, chuan);
			if (!TryPreventRuntimeFatalDamage(num, 1, null))
			{
				CutHealth(num, 1, type, null);
				SingletonMonoScope<DamgeTextManager>.Instance.CreatCombatText(base.transform.position, num, type, crit: false);
			}
		}
	}

	public static float GetRequiredXpForLevel(int level)
	{
		level = Mathf.Max(1, level);
		return Mathf.Floor(300f * Mathf.Pow(1.1f, level));
	}

	public static float GetRequiredDFXpForLevel(int dfLevel)
	{
		dfLevel = Mathf.Max(1, dfLevel);
		return Mathf.Floor(GetRequiredXpForLevel(100) / 2f * Mathf.Pow(1.013f, dfLevel - 1));
	}

	private float GetXpAfterBonus(int xp)
	{
		return (float)xp + (float)xp * Xp_Bei_Tmp / 100f;
	}

	private bool CanGainXp()
	{
		if (IsAlive)
		{
			return !hasDeaded;
		}
		return false;
	}

	public void GainXp(int xp)
	{
		if (CanGainXp())
		{
			float xpAfterBonus = GetXpAfterBonus(xp);
			Xp_Total += xpAfterBonus;
			if (Level >= 100)
			{
				AddDFXp(xpAfterBonus);
				return;
			}
			float num = ((Level <= 1) ? 6f : ((Level <= 2) ? 5f : ((Level <= 3) ? 4f : ((Level <= 5) ? 3f : ((Level > 10) ? 1f : 1.5f)))));
			AddNormalXp(xpAfterBonus * num);
		}
	}

	private void AddNormalXp(float addXp)
	{
		Xp_CurrentLevel += addXp;
		if ((bool)XpStat)
		{
			XpStat.CurrentValue = Xp_CurrentLevel;
		}
		float num = (XpStat ? XpStat.MaxValue : GetRequiredXpForLevel(Level));
		if (Xp_CurrentLevel >= num)
		{
			StartCoroutine(LevelUP());
		}
	}

	private IEnumerator LevelUP()
	{
		while ((bool)XpStat && !XpStat.IsFull)
		{
			yield return null;
		}
		float num = (XpStat ? XpStat.MaxValue : GetRequiredXpForLevel(Level));
		float num2 = Xp_CurrentLevel - num;
		Level++;
		if (Level >= 100)
		{
			Level = 100;
		}
		GameManager.ShowTip(LOC.MM.GetMainFormat("player_level_reached", Level), TipType.Success);
		if (!IsLevelUP && SingletonMonoScope<GameDataManager>.HasInstance)
		{
			LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.LevelUP, base.transform.position, Quaternion.identity);
			IsLevelUP = true;
		}
		SingletonMonoScope<TalentManager>.Instance.LevelUP();
		Health += 30f * Mathf.Pow(Level, 0.13f);
		Mana += 15f * Mathf.Pow(Level, 0.105f);
		ItemDrop_Rate += 2f;
		if (Level >= 100)
		{
			Xp_CurrentLevel = 0f;
			if ((bool)XpStat)
			{
				XpStat.MaxValue = GetRequiredXpForLevel(100);
				XpStat.CurrentValue = XpStat.MaxValue;
				XpStat.Reset();
			}
			if (num2 > 0f)
			{
				AddDFXp(num2);
			}
		}
		else
		{
			Xp_CurrentLevel = num2;
			if ((bool)XpStat)
			{
				XpStat.MaxValue = GetRequiredXpForLevel(Level);
				XpStat.CurrentValue = Xp_CurrentLevel;
				XpStat.Reset();
			}
			if (Xp_CurrentLevel >= GetRequiredXpForLevel(Level))
			{
				StartCoroutine(LevelUP());
			}
		}
		HealStat.Cur = HealStat.Max;
		ManaStat.Cur = ManaStat.Max;
	}

	public void GainDFXp(int xp)
	{
		if (CanGainXp())
		{
			float xpAfterBonus = GetXpAfterBonus(xp);
			Xp_Total += xpAfterBonus;
			AddDFXp(xpAfterBonus);
		}
	}

	private void AddDFXp(float addXp)
	{
		DFXp_Total += addXp;
		DFXp_CurrentLevel += addXp;
		if ((bool)DFXpStat)
		{
			DFXpStat.CurrentValue = DFXp_CurrentLevel;
		}
		float num = (DFXpStat ? DFXpStat.MaxValue : GetRequiredDFXpForLevel(DFLevel));
		if (DFXp_CurrentLevel >= num)
		{
			StartCoroutine(DFLevelUP());
		}
	}

	private IEnumerator DFLevelUP()
	{
		while ((bool)DFXpStat && !DFXpStat.IsFull)
		{
			yield return null;
		}
		float num = (DFXpStat ? DFXpStat.MaxValue : GetRequiredDFXpForLevel(DFLevel));
		float dFXp_CurrentLevel = DFXp_CurrentLevel - num;
		DFLevel++;
		GameManager.ShowTip(LOC.MM.GetMainFormat("player_df_level_reached", DFLevel), TipType.Success);
		if (!IsLevelUP && SingletonMonoScope<GameDataManager>.HasInstance)
		{
			LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.LevelUP, base.transform.position, Quaternion.identity);
			IsLevelUP = true;
		}
		SingletonMonoScope<TalentManager>.Instance.LevelUP();
		DFXp_CurrentLevel = dFXp_CurrentLevel;
		if ((bool)DFXpStat)
		{
			DFXpStat.MaxValue = GetRequiredDFXpForLevel(DFLevel);
			DFXpStat.CurrentValue = DFXp_CurrentLevel;
			DFXpStat.Reset();
		}
		if (DFXp_CurrentLevel >= GetRequiredDFXpForLevel(DFLevel))
		{
			StartCoroutine(DFLevelUP());
		}
		HealStat.Cur = HealStat.Max;
		ManaStat.Cur = ManaStat.Max;
	}

	public void KillRecver()
	{
		HealStat.Cur += Attack_R_health_Max;
		ManaStat.Cur += Attack_R_mana_Max;
	}

	public float GiveDamage(DamageType type)
	{
		return type switch
		{
			DamageType.fire => FireDamage, 
			DamageType.frozen => FrozenDamage, 
			DamageType.thunder => ThunderDamage, 
			DamageType.poison => PoisonDamage, 
			DamageType.physics => PhysicsDamage, 
			DamageType.shadow => ShadowDamage, 
			_ => 0f, 
		};
	}

	public float GiveDamage(int type)
	{
		return type switch
		{
			0 => FireDamage, 
			1 => FrozenDamage, 
			2 => ThunderDamage, 
			3 => PoisonDamage, 
			4 => PhysicsDamage, 
			5 => ShadowDamage, 
			_ => 0f, 
		};
	}

	private bool TryPreventRuntimeFatalDamage(float finalDamage, int cutRate, Enemy enemy)
	{
		if (!BuffRuntime)
		{
			return false;
		}
		float num = finalDamage;
		if (DMGsplit > 0f && ACT.GetCP_CT() > 0 && (bool)enemy)
		{
			num *= 1f - DMGsplit / 100f;
		}
		float num2 = num / (float)cutRate;
		if (HealCutMana)
		{
			num2 = Mathf.Max(0f, num2 - ManaStat.Cur);
		}
		if (num2 <= 0f || HealStat.Cur - num2 > 0f)
		{
			return false;
		}
		if (!BuffRuntime.TryPreventFatalDamage())
		{
			return false;
		}
		HealStat.Cur = Mathf.Max(HealStat.Cur, 1f);
		return true;
	}

	public void CutHealth(float finalDamage, int A, DamageType type, Enemy ememy)
	{
		if (DMGsplit > 0f && ACT.GetCP_CT() > 0 && (bool)ememy)
		{
			ACT.CP_DMGsplit(finalDamage * DMGsplit / 100f, type, ememy);
			finalDamage *= 1f - DMGsplit / 100f;
		}
		if (HealCutMana)
		{
			if (ManaStat.Cur >= finalDamage / (float)A)
			{
				ManaStat.Cur -= finalDamage / (float)A;
				return;
			}
			HealStat.Cur -= finalDamage / (float)A - ManaStat.Cur;
			ManaStat.Cur -= finalDamage / (float)A;
		}
		else
		{
			HealStat.Cur -= finalDamage / (float)A;
		}
	}

	public void SetSkillBeiBuff(bool add, int type, float number, int level)
	{
		if (add)
		{
			switch (type)
			{
			case 0:
				Health_Bei += number * (float)level;
				break;
			case 1:
				Mana_Bei += number * (float)level;
				break;
			case 2:
				ATSpeed_Bei += number * (float)level;
				break;
			case 3:
				MVSpeed_Bei += number * (float)level;
				break;
			case 4:
				CoolDown += number * (float)level;
				break;
			case 5:
				GeDang += number * (float)level;
				break;
			case 6:
				DOTcut += number * (float)level;
				break;
			case 7:
				ThroughRate += number * (float)level;
				break;
			case 8:
				Damage_Anti += number * (float)level;
				break;
			case 9:
				FireDamage_Bei += number * (float)level;
				break;
			case 10:
				FrozenDamage_Bei += number * (float)level;
				break;
			case 11:
				ThunderDamage_Bei += number * (float)level;
				break;
			case 12:
				PoisonDamage_Bei += number * (float)level;
				break;
			case 13:
				PhysicsDamage_Bei += number * (float)level;
				break;
			case 14:
				ShadowDamage_Bei += number * (float)level;
				break;
			case 15:
				FireChuan += number * (float)level;
				break;
			case 16:
				FrozenChuan += number * (float)level;
				break;
			case 17:
				ThunderChuan += number * (float)level;
				break;
			case 18:
				PoisonChuan += number * (float)level;
				break;
			case 19:
				PhysicsChuan += number * (float)level;
				break;
			case 20:
				ShadowChuan += number * (float)level;
				break;
			case 21:
				C_Health += number * (float)level;
				break;
			case 22:
				C_Damage += number * (float)level;
				break;
			case 23:
				C_ATSpeed += number * (float)level;
				break;
			case 24:
				C_MVSpeed += number * (float)level;
				break;
			case 25:
				C_AllAnti += number * (float)level;
				break;
			case 26:
				Attack_R_health_Percent += number / 100f * (float)level;
				break;
			case 27:
				Attack_R_mana_Percent += number / 100f * (float)level;
				break;
			case 28:
				Damage_Bei += number * (float)level;
				break;
			case 29:
				BJrate += number * (float)level;
				break;
			case 30:
				BJDamage += number * (float)level;
				break;
			case 31:
				FlySpeed += number * (float)level;
				break;
			case 32:
				ORB_Damage += number * (float)level;
				break;
			}
		}
		else
		{
			switch (type)
			{
			case 0:
				Health_Bei -= number * (float)level;
				break;
			case 1:
				Mana_Bei -= number * (float)level;
				break;
			case 2:
				ATSpeed_Bei -= number * (float)level;
				break;
			case 3:
				MVSpeed_Bei -= number * (float)level;
				break;
			case 4:
				CoolDown -= number * (float)level;
				break;
			case 5:
				GeDang -= number * (float)level;
				break;
			case 6:
				DOTcut -= number * (float)level;
				break;
			case 7:
				ThroughRate -= number * (float)level;
				break;
			case 8:
				Damage_Anti -= number * (float)level;
				break;
			case 9:
				FireDamage_Bei -= number * (float)level;
				break;
			case 10:
				FrozenDamage_Bei -= number * (float)level;
				break;
			case 11:
				ThunderDamage_Bei -= number * (float)level;
				break;
			case 12:
				PoisonDamage_Bei -= number * (float)level;
				break;
			case 13:
				PhysicsDamage_Bei -= number * (float)level;
				break;
			case 14:
				ShadowDamage_Bei -= number * (float)level;
				break;
			case 15:
				FireChuan -= number * (float)level;
				break;
			case 16:
				FrozenChuan -= number * (float)level;
				break;
			case 17:
				ThunderChuan -= number * (float)level;
				break;
			case 18:
				PoisonChuan -= number * (float)level;
				break;
			case 19:
				PhysicsChuan -= number * (float)level;
				break;
			case 20:
				ShadowChuan -= number * (float)level;
				break;
			case 21:
				C_Health -= number * (float)level;
				break;
			case 22:
				C_Damage -= number * (float)level;
				break;
			case 23:
				C_ATSpeed -= number * (float)level;
				break;
			case 24:
				C_MVSpeed -= number * (float)level;
				break;
			case 25:
				C_AllAnti -= number * (float)level;
				break;
			case 26:
				Attack_R_health_Percent -= number / 100f * (float)level;
				break;
			case 27:
				Attack_R_mana_Percent -= number / 100f * (float)level;
				break;
			case 28:
				Damage_Bei -= number * (float)level;
				break;
			case 29:
				BJrate -= number * (float)level;
				break;
			case 30:
				BJDamage -= number * (float)level;
				break;
			case 31:
				FlySpeed -= number * (float)level;
				break;
			case 32:
				ORB_Damage -= number * (float)level;
				break;
			}
		}
	}

	public void SetDFSkillBuff(bool add, int type, float number, int level)
	{
		float num = number * (float)level;
		if (!add)
		{
			num = 0f - num;
		}
		switch (type)
		{
		case 0:
			Health += num;
			break;
		case 1:
			Mana += num;
			break;
		case 2:
			Damage_Base += num;
			break;
		case 3:
			FireDamage_Bei += num;
			break;
		case 4:
			FrozenDamage_Bei += num;
			break;
		case 5:
			ThunderDamage_Bei += num;
			break;
		case 6:
			PoisonDamage_Bei += num;
			break;
		case 7:
			PhysicsDamage_Bei += num;
			break;
		case 8:
			ShadowDamage_Bei += num;
			break;
		case 9:
			Health_Bei += num;
			break;
		case 10:
			Mana_Bei += num;
			break;
		case 11:
			C_Health += num;
			break;
		case 12:
			C_ATSpeed += num;
			break;
		case 13:
			C_MVSpeed += num;
			break;
		case 14:
			C_AllAnti += num;
			break;
		case 15:
			C_Damage += num;
			break;
		case 16:
			DOTcut += num;
			break;
		case 17:
			BS_Multi += num;
			break;
		case 18:
			WPSPC_DMG += Mathf.RoundToInt(num);
			break;
		case 19:
			Damage_Bei += num;
			break;
		case 20:
			JYBoss_DMG += Mathf.RoundToInt(num);
			break;
		case 21:
			BJDamage += num;
			break;
		case 22:
			AllDot_DMG += num;
			break;
		case 23:
			ATSpeed_Bei += num;
			break;
		case 24:
			MVSpeed_Bei += num;
			break;
		case 25:
			XJL_UseSKTime += num;
			break;
		case 26:
			ItemDrop_Rate += num;
			break;
		case 27:
			FireChuan += num;
			break;
		case 28:
			FrozenChuan += num;
			break;
		case 29:
			ThunderChuan += num;
			break;
		case 30:
			PoisonChuan += num;
			break;
		case 31:
			PhysicsChuan += num;
			break;
		case 32:
			ShadowChuan += num;
			break;
		case 33:
			QH_Price += Mathf.RoundToInt(num);
			break;
		case 34:
			XJL_SellPrice += num;
			break;
		case 35:
			BuffT_Temple += num;
			break;
		case 36:
			Top_CD += Mathf.RoundToInt(num);
			break;
		case 37:
			BJrate += num;
			break;
		case 38:
			BuffT_Drink += num;
			break;
		case 39:
			Top_GD += Mathf.RoundToInt(num);
			break;
		case 40:
			Top_Anti += Mathf.RoundToInt(num);
			break;
		case 41:
			Temple_DMG += Mathf.RoundToInt(num);
			break;
		case 42:
			Temple_ATS += Mathf.RoundToInt(num);
			break;
		case 43:
			AllDot_Layer += Mathf.RoundToInt(num);
			break;
		case 44:
			ATSpeed_Bei += num;
			break;
		case 45:
			Damage_Bei += num;
			break;
		case 46:
			AllDot_DMG += num;
			break;
		case 47:
			C_Health += num;
			break;
		case 48:
			C_Damage += num;
			break;
		case 49:
			QH_Bei += Mathf.RoundToInt(num);
			break;
		case 50:
			Temple_HealPrc += num;
			break;
		case 51:
			BJDamage += num;
			break;
		case 52:
			C_Damage += num;
			break;
		case 53:
			ORB_Damage += num;
			break;
		case 54:
			CoolDown += num;
			break;
		case 55:
			CompCount += Mathf.RoundToInt(num);
			break;
		case 56:
			AllDot_DMG += num;
			break;
		case 57:
			XJL_DMG += num;
			break;
		case 58:
			XJL_DropMulti += num;
			break;
		}
		RefreshRuntimeDerivedStats();
	}

	public void RefreshORB(SkillOBJ_DT_SP sp, int type)
	{
		float num = type switch
		{
			0 => ORB_Damage_Last, 
			1 => ORB_Damage_Last, 
			2 => ORB_Damage_Last, 
			3 => ORB_Damage_Last + Orb_Universe_DMG_Last, 
			4 => ORB_Damage_Last + Orb_Bow_DMG, 
			_ => ORB_Damage_Last, 
		};
		sp.Damage = GiveDamage(sp.damageType) * sp.SPC_Damage / 100f * (1f + num / 100f);
		sp.DamageA = GiveDamage(sp.damageType) * sp.SPC_DamageA / 100f * (1f + num / 100f);
		sp.DamageB = GiveDamage(sp.damageType) * sp.SPC_DamageB / 100f * (1f + num / 100f);
		sp.JYrate = JYrate_Last;
		sp.BJrate = BJrate_Last;
		sp.BJDamage = BJDamage_Last;
		sp.Through = ThroughRate;
		sp.FlySpeed = FlySpeed;
	}

	public void PlayerSP(int ani)
	{
		switch (PLType)
		{
		case 0:
			mgc.ACT(ani);
			break;
		case 1:
			sqs.ACT(ani);
			break;
		case 2:
			arc.ACT(ani);
			break;
		case 3:
			dead.ACT(ani);
			break;
		}
	}

	public void PlayerCP(int ani)
	{
		switch (PLType)
		{
		case 0:
			mgc.ACT(ani);
			break;
		case 1:
			sqs.ACT(ani);
			break;
		case 2:
			arc.ACT(ani);
			break;
		case 3:
			dead.ACT(ani);
			break;
		}
	}

	public bool ReturnAni()
	{
		return PLType switch
		{
			0 => mgc.ReturnAni(), 
			1 => sqs.ReturnAni(), 
			2 => arc.ReturnAni(), 
			3 => dead.ReturnAni(), 
			_ => false, 
		};
	}

	public float GiveChuan(DamageType type)
	{
		return type switch
		{
			DamageType.fire => FireChuan_Last, 
			DamageType.frozen => FrozenChuan_Last, 
			DamageType.thunder => ThunderChuan_Last, 
			DamageType.poison => PoisonChuan_Last, 
			DamageType.physics => PhysicsChuan_Last, 
			DamageType.shadow => ShadowChuan_Last, 
			_ => 0f, 
		};
	}

	public float GiveChuan(int type)
	{
		return type switch
		{
			0 => FireChuan_Last, 
			1 => FrozenChuan_Last, 
			2 => ThunderChuan_Last, 
			3 => PoisonChuan_Last, 
			4 => PhysicsChuan_Last, 
			5 => ShadowChuan_Last, 
			_ => 0f, 
		};
	}

	public int GiveInt(DamageType type)
	{
		return type switch
		{
			DamageType.fire => 0, 
			DamageType.frozen => 1, 
			DamageType.thunder => 2, 
			DamageType.poison => 3, 
			DamageType.physics => 4, 
			DamageType.shadow => 5, 
			_ => 0, 
		};
	}

	public bool HasDebuff()
	{
		if (BuffMG.HasDebuff())
		{
			return true;
		}
		return false;
	}

	public void ClearAllDebuff()
	{
		BuffMG.DelAllDebuff();
	}

	private void CheckDeathState()
	{
		if ((bool)HealStat && !(HealStat.Cur > 0f) && !hasDeaded)
		{
			OnDead();
		}
	}

	private void ApplyBurnLife()
	{
		if (IsBurnLifeEffectActive)
		{
			BuffMG?.DelAllDebuff();
			ApplyBurnLife(BurnLife0, DamageType.fire);
			ApplyBurnLife(BurnLife1, DamageType.frozen);
			ApplyBurnLife(BurnLife2, DamageType.thunder);
			ApplyBurnLife(BurnLife3, DamageType.poison);
			ApplyBurnLife(BurnLife4, DamageType.physics);
			ApplyBurnLife(BurnLife5, DamageType.shadow);
		}
	}

	private void ApplyBurnLife(int percent, DamageType type)
	{
		if (percent > 0 && IsBurnLifeEffectActive)
		{
			float num = HealStat.Max * (float)percent / 100f;
			if (!(num <= 0f))
			{
				HealStat.Cur = Mathf.Max(HealStat.Max * 0.01f, HealStat.Cur - num);
				DamageEnemiesInRange(5f, burnLifeEnemyBuffer, num, type, onlyNormalEnemy: false, killEnemy: false);
			}
		}
	}

	private bool HasBurnLifeStat()
	{
		if (BurnLife0 <= 0 && BurnLife1 <= 0 && BurnLife2 <= 0 && BurnLife3 <= 0 && BurnLife4 <= 0)
		{
			return BurnLife5 > 0;
		}
		return true;
	}

	private void ApplyDieEXP()
	{
		if (DieEXP)
		{
			DamageEnemiesInRange(5f, deathExplosionEnemyBuffer, float.MaxValue, DamageType.physics, onlyNormalEnemy: true, killEnemy: true);
		}
	}

	private void DamageEnemiesInRange(float range, List<Enemy> enemies, float damage, DamageType type, bool onlyNormalEnemy, bool killEnemy)
	{
		if (enemies == null)
		{
			return;
		}
		CollectEnemiesInRange(range, enemies, onlyNormalEnemy);
		for (int i = 0; i < enemies.Count; i++)
		{
			Enemy enemy = enemies[i];
			if ((bool)enemy && enemy.IsAlive && !enemy.IsWuDi && !enemy.IsJump && !enemy.IsYS && (!onlyNormalEnemy || enemy.Quality < 2))
			{
				float num = (killEnemy ? (enemy.HealthStat.CurrentValue + 1f) : damage);
				if (!killEnemy && enemy.IS_Boss)
				{
					num *= 0.5f;
				}
				enemy.TakeDirectDamage(num, type);
			}
		}
	}

	private void RefreshNearbyDotBuffDamage()
	{
		BF_DMG_Last = 0f;
		if (!HasDotBuffDamage())
		{
			return;
		}
		CollectEnemiesInRange(5f, dotBuffEnemyBuffer, onlyNormalEnemy: false);
		for (int i = 0; i < dotBuffEnemyBuffer.Count; i++)
		{
			Enemy enemy = dotBuffEnemyBuffer[i];
			if ((bool)enemy && !(enemy.peo == null) && !(enemy.peo.DotEM == null))
			{
				BF_DMG_Last += enemy.peo.DotEM.GerDotBF_DMG();
			}
		}
	}

	private void RefreshNearbyEnemyStatCounts()
	{
		NearEMC = 0;
		NearJYC = 0;
		if (!HasNearbyEnemyStatBonus())
		{
			return;
		}
		CollectEnemiesInRange(5f, nearbyEnemyStatBuffer, onlyNormalEnemy: false);
		NearEMC = nearbyEnemyStatBuffer.Count;
		for (int i = 0; i < nearbyEnemyStatBuffer.Count; i++)
		{
			Enemy enemy = nearbyEnemyStatBuffer[i];
			if ((bool)enemy && (enemy.IS_Boss || enemy.Quality > 2))
			{
				NearJYC++;
			}
		}
	}

	private bool HasNearbyEnemyStatBonus()
	{
		if (EMC_DMG_20 == 0f && EMC_DMG_48 == 0f && EMC_Anti_9 == 0f && EMC_GD_12 == 0f && JYC_DMG_15 == 0f && JYC_ATS_24 == 0f)
		{
			return JYC_BJD_24 != 0f;
		}
		return true;
	}

	private bool HasDotBuffDamage()
	{
		if (DOT == null)
		{
			return false;
		}
		for (int i = 0; i < DOT.Length; i++)
		{
			if (DOT[i] != null && DOT[i].BF_DMG > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void CollectEnemiesInRange(float range, List<Enemy> result, bool onlyNormalEnemy, bool requireLineOfSight = false)
	{
		EnsureEnemyRangeBuffers();
		if (result == null)
		{
			return;
		}
		result.Clear();
		if (range <= 0f || enemyRangeHits == null)
		{
			return;
		}
		int num = ((footCOLemLayerMask != 0) ? footCOLemLayerMask : LayerMask.GetMask("FootCOLem"));
		if (num == 0)
		{
			return;
		}
		int num2;
		while ((num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, enemyRangeHits, num)) == enemyRangeHits.Length && enemyRangeHits.Length < 128)
		{
			Array.Resize(ref enemyRangeHits, enemyRangeHits.Length * 2);
		}
		for (int i = 0; i < num2; i++)
		{
			Collider2D collider2D = enemyRangeHits[i];
			enemyRangeHits[i] = null;
			if (!collider2D || !collider2D.TryGetComponent<FootCOL>(out var component) || !component)
			{
				continue;
			}
			People people = component.peo;
			if ((bool)people && people.CharacterType == 2)
			{
				Enemy enemy = people.em;
				if ((bool)enemy && enemy.IsAlive && !enemy.IsWuDi && !enemy.IsJump && !enemy.IsYS && (!onlyNormalEnemy || enemy.Quality < 2) && (!requireLineOfSight || !IsEnemyBlockedByWall(enemy)) && !result.Contains(enemy))
				{
					result.Add(enemy);
				}
			}
		}
	}

	public void OnDead()
	{
		SingletonMonoScope<GameUIManager>.Instance.CloseAll();
		if (hasDeaded)
		{
			return;
		}
		hasDeaded = true;
		ApplyDieEXP();
		if (SettingsLoader.Instance.canDead && !Singleton<UIManager>.Instance.IsPanelOpened<DeadPanel>())
		{
			Singleton<UIManager>.Instance.ShowExclusivePanel<DeadPanel>();
			ApplyDeathMoneyPenalty();
		}
		EventSystem.current?.SetSelectedGameObject(null);
		RuntimeManager.PlayOneShot(SO_Die[PLType], yao.transform.position);
		ManaStat.Cur = 0f;
		CanMove = false;
		StopMousePathMovement(clearTarget: true);
		mouseMoveRuntimeActive = false;
		SetMouseMovePathBehavioursEnabled(enable: false);
		SetMouseMoveBlockingCollidersHidden(hidden: false);
		rigBD.velocity = Vector2.zero;
		Direction = Vector2.zero;
		InputManager.AllActionToggle = false;
		InteractionManager.AllInteractToggle = false;
		if (SingletonMonoGlobal<StateDataManager>.HasInstance)
		{
			SingletonMonoGlobal<StateDataManager>.Instance.ClearAllPortalStatesInMijing();
		}
		BuffMG.DelAll();
		BuffRuntime?.ClearAllRuntimeBuffs();
		if (SingletonMonoScope<BuffManager>.HasInstance)
		{
			SingletonMonoScope<BuffManager>.Instance.ClearAll();
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			if (SingletonMonoScope<ACTbar>.Instance.SkillBuffList.Count > 0)
			{
				foreach (SK_BuffA skillBuff in SingletonMonoScope<ACTbar>.Instance.SkillBuffList)
				{
					skillBuff.StopBuff();
				}
				SingletonMonoScope<ACTbar>.Instance.ResetATPrefabState();
				BSname = null;
				SingletonMonoScope<ACTbar>.Instance.SkillBuffList.Clear();
			}
			BuffMG.ClearSkillBuff();
			foreach (ACTListSkillBT item in SingletonMonoScope<ACTbar>.Instance.actListSkill)
			{
				if ((bool)item)
				{
					item.ResetCD();
					item.ClearCpList();
				}
			}
		}
		IsAttack = false;
		IsSkill = false;
		IsAttackAnimationSkill = false;
		IsBattle = false;
		IsChong = false;
		IScomp = false;
		CurUseSK = -1;
		lockMove = true;
		isTeleporting = false;
		ChuanSong = false;
		ChuanSongPOS = Vector3.zero;
		IsYun = false;
		Direction = Vector2.zero;
		PlayDeathAnimation();
	}

	private static void ApplyDeathMoneyPenalty()
	{
		if (SaveManager.HasRuntime && SaveManager.RuntimeData?.InventoryData != null && SaveManager.RuntimeData.InventoryData.Money > 0)
		{
			long num = SingletonMonoScope<InventoryManager>.Instance.GlobalMoney / 10;
			SingletonMonoScope<InventoryManager>.Instance.RemoveMoney(num);
			GlobalRuntimeData.SetDeathLostMoney(num);
		}
	}

	public void SetPlayerReborn()
	{
		HealStat.Cur = HealStat.Max;
		ManaStat.Cur = ManaStat.Max;
		hasDeaded = false;
		CanMove = true;
		lockMove = false;
		IsAttack = false;
		IsSkill = false;
		IsAttackAnimationSkill = false;
		IsBattle = false;
		IsChong = false;
		IScomp = false;
		isTeleporting = false;
		ChuanSong = false;
		ChuanSongPOS = Vector3.zero;
		IsYun = false;
		Direction = Vector2.zero;
		CurUseSK = -1;
		if ((bool)rigBD)
		{
			rigBD.velocity = Vector2.zero;
			rigBD.angularVelocity = 0f;
		}
		ClearAllPlayerAnimationState();
		InteractionManager.AllInteractToggle = true;
		InputManager.AllActionToggle = true;
		if ((bool)BuffMG)
		{
			BuffMG.DelAll();
			BuffMG.ClearSkillBuff();
		}
		BuffRuntime?.ClearAllRuntimeBuffs();
		if (SingletonMonoScope<BuffManager>.HasInstance)
		{
			SingletonMonoScope<BuffManager>.Instance.ClearAll();
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			foreach (ACTListSkillBT item in SingletonMonoScope<ACTbar>.Instance.actListSkill)
			{
				if ((bool)item)
				{
					item.ResetCD();
					item.ClearCpList();
				}
			}
			SingletonMonoScope<ACTbar>.Instance.ResetATPrefabState();
			BSname = null;
			SingletonMonoScope<ACTbar>.Instance.SkillBuffList.Clear();
		}
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			SingletonMonoScope<InputManager>.Instance.ForceClearInteractionLocks();
		}
	}

	public void SetPlayerDead()
	{
		HealStat.Cur = 0f;
	}

	public void ClearAllPlayerAnimationState()
	{
		ClearPlayerAnimation(mgc ? mgc.ani : null);
		ClearPlayerAnimation(sqs ? sqs.ani : null);
		ClearPlayerAnimation(arc ? arc.ani : null);
		ClearPlayerAnimation(dead ? dead.ani : null);
		MoveTrack = null;
		AttackTrack = null;
		SkillTrack = null;
		UpBOWTrack = null;
	}

	private static void ClearPlayerAnimation(SkeletonAnimation animation)
	{
		if ((bool)animation && animation.gameObject.activeInHierarchy)
		{
			if (animation.AnimationState != null)
			{
				animation.AnimationState.ClearTracks();
			}
			if (animation.Skeleton != null)
			{
				animation.Skeleton.SetToSetupPose();
			}
		}
	}

	public void ResetActionStateForGameplayUnlock()
	{
		Direction = Vector2.zero;
		if ((bool)rigBD)
		{
			rigBD.velocity = Vector2.zero;
		}
		IsAttack = false;
		IsSkill = false;
		IsAttackAnimationSkill = false;
		IsBattle = false;
		IsChong = false;
		ChuanSong = false;
		CurUseSK = -1;
		RefreshRuntimeDerivedStats();
		ClearAllPlayerAnimationState();
		ForceIdleAnimation();
		ResetMoveAnimationStateMachine();
	}

	private void ForceIdleAnimation()
	{
		switch (PLType)
		{
		case 0:
			if ((bool)mgc && mgc.gameObject.activeInHierarchy && mgc.stat != null)
			{
				mgc.idleON();
			}
			break;
		case 1:
			if ((bool)sqs && sqs.gameObject.activeInHierarchy && sqs.stat != null)
			{
				sqs.idleON();
			}
			break;
		case 2:
			if ((bool)arc && arc.gameObject.activeInHierarchy && arc.stat != null)
			{
				arc.idleON();
			}
			break;
		case 3:
			if ((bool)dead && dead.gameObject.activeInHierarchy && dead.stat != null)
			{
				dead.idleON();
			}
			break;
		}
	}

	private void PlayDeathAnimation()
	{
		MoveTrack = null;
		AttackTrack = null;
		SkillTrack = null;
		UpBOWTrack = null;
		switch (PLType)
		{
		case 0:
			if ((bool)mgc && mgc.gameObject.activeInHierarchy && mgc.stat != null)
			{
				mgc.Die();
			}
			break;
		case 1:
			if ((bool)sqs && sqs.gameObject.activeInHierarchy && sqs.stat != null)
			{
				sqs.Die();
			}
			break;
		case 2:
			if ((bool)arc && arc.gameObject.activeInHierarchy && arc.stat != null)
			{
				arc.Die();
			}
			break;
		case 3:
			if ((bool)dead && dead.gameObject.activeInHierarchy && dead.stat != null)
			{
				dead.Die();
			}
			break;
		}
	}

	private void ResetMoveAnimationStateMachine()
	{
		switch (PLType)
		{
		case 0:
			mgc?.STA?.Switch2Null();
			break;
		case 1:
			sqs?.STA?.Switch2Null();
			break;
		case 2:
			arc?.STA?.Switch2Null();
			break;
		case 3:
			dead?.STA?.Switch2Null();
			break;
		}
	}

	public void PlayMoveAnimationIfNeeded(Spine.AnimationState stat, string animationName, bool loop, float timeScale)
	{
		if (stat != null && !string.IsNullOrEmpty(animationName))
		{
			TrackEntry current = stat.GetCurrent(0);
			if (current == null || current.Animation == null || current.Animation.Name != animationName)
			{
				MoveTrack = stat.SetAnimation(0, animationName, loop);
			}
			else
			{
				MoveTrack = current;
			}
			if (MoveTrack != null)
			{
				MoveTrack.TimeScale = timeScale;
			}
		}
	}

	private void UpdateAnimationSpeed()
	{
		ValidateTrackedAnimationEntries();
		if (MoveTrack != null)
		{
			if (ReturnAni())
			{
				MoveTrack.TimeScale = 1f;
			}
			else
			{
				MoveTrack.TimeScale = MoveAnimationTimeScale;
			}
		}
		if (AttackTrack != null)
		{
			AttackTrack.TimeScale = (IsSkill ? SkillAnimationTimeScale : AttackAnimationTimeScale);
		}
		if (SkillTrack != null)
		{
			SkillTrack.TimeScale = SkillAnimationTimeScale;
		}
	}

	private Spine.AnimationState GetCurrentPlayerAnimationState()
	{
		switch (PLType)
		{
		case 0:
			if (!mgc || !mgc.gameObject.activeInHierarchy)
			{
				return null;
			}
			return mgc.stat;
		case 1:
			if (!sqs || !sqs.gameObject.activeInHierarchy)
			{
				return null;
			}
			return sqs.stat;
		case 2:
			if (!arc || !arc.gameObject.activeInHierarchy)
			{
				return null;
			}
			return arc.stat;
		case 3:
			if (!dead || !dead.gameObject.activeInHierarchy)
			{
				return null;
			}
			return dead.stat;
		default:
			return null;
		}
	}

	private void ValidateTrackedAnimationEntries()
	{
		Spine.AnimationState currentPlayerAnimationState = GetCurrentPlayerAnimationState();
		if (!IsCurrentTrackEntry(currentPlayerAnimationState, MoveTrack, 0))
		{
			MoveTrack = null;
		}
		if (!IsCurrentActionTrackEntry(currentPlayerAnimationState, AttackTrack))
		{
			AttackTrack = null;
		}
		if (!IsCurrentActionTrackEntry(currentPlayerAnimationState, UpBOWTrack))
		{
			UpBOWTrack = null;
		}
		if (!IsCurrentActionTrackEntry(currentPlayerAnimationState, SkillTrack))
		{
			SkillTrack = null;
		}
	}

	private static bool IsCurrentTrackEntry(Spine.AnimationState stat, TrackEntry trackEntry, int trackIndex)
	{
		if (stat == null || trackEntry == null || trackEntry.TrackIndex != trackIndex || IsEmptyTrackEntry(trackEntry))
		{
			return false;
		}
		return stat.GetCurrent(trackIndex) == trackEntry;
	}

	private static bool IsCurrentActionTrackEntry(Spine.AnimationState stat, TrackEntry trackEntry)
	{
		if (stat == null || trackEntry == null || trackEntry.TrackIndex <= 0 || IsEmptyTrackEntry(trackEntry))
		{
			return false;
		}
		return stat.GetCurrent(trackEntry.TrackIndex) == trackEntry;
	}

	private static bool IsEmptyTrackEntry(TrackEntry trackEntry)
	{
		if (trackEntry.Animation != null)
		{
			return trackEntry.Animation.Name == "<empty>";
		}
		return true;
	}

	public void PrefabCount(int type, bool add)
	{
		switch (type)
		{
		case 1:
			ChangePrefabCount(ref Q_1, add);
			break;
		case 2:
			ChangePrefabCount(ref Q_2, add);
			break;
		case 3:
			ChangePrefabCount(ref Q_3, add);
			break;
		case 4:
			ChangePrefabCount(ref Q_4, add);
			break;
		case 5:
			ChangePrefabCount(ref Q_5, add);
			break;
		case 6:
			ChangePrefabCount(ref Q_6, add);
			break;
		case 7:
			ChangePrefabCount(ref Q_7, add);
			break;
		case 8:
			ChangePrefabCount(ref Q_8, add);
			break;
		case 9:
			ChangePrefabCount(ref Q_9, add);
			break;
		case 10:
			ChangePrefabCount(ref Q_10, add);
			break;
		case 11:
			ChangePrefabCount(ref Q_11, add);
			break;
		case 12:
			ChangePrefabCount(ref Q_12, add);
			break;
		case 13:
			ChangePrefabCount(ref Q_13, add);
			break;
		case 14:
			ChangePrefabCount(ref Q_14, add);
			break;
		case 15:
			ChangePrefabCount(ref Q_15, add);
			break;
		case 16:
			ChangePrefabCount(ref Q_16, add);
			break;
		case 17:
			ChangePrefabCount(ref Q_17, add);
			break;
		case 18:
			ChangePrefabCount(ref Q_18, add);
			break;
		case 19:
			ChangePrefabCount(ref Q_19, add);
			break;
		case 20:
			ChangePrefabCount(ref Q_20, add);
			break;
		case 30:
			ChangePrefabCount(ref Q_30, add);
			break;
		case 40:
			ChangePrefabCount(ref Q_40, add);
			break;
		case 41:
			ChangePrefabCount(ref Q_41, add);
			break;
		case 42:
			ChangePrefabCount(ref Q_42, add);
			break;
		case 43:
			ChangePrefabCount(ref Q_43, add);
			break;
		case 44:
			ChangePrefabCount(ref Q_44, add);
			break;
		case 45:
			ChangePrefabCount(ref Q_45, add);
			break;
		case 51:
			ChangePrefabCount(ref Q_51, add);
			break;
		case 52:
			ChangePrefabCount(ref Q_52, add);
			break;
		case 53:
			ChangePrefabCount(ref Q_53, add);
			break;
		case 54:
			ChangePrefabCount(ref Q_54, add);
			break;
		case 55:
			ChangePrefabCount(ref Q_55, add);
			break;
		case 56:
			ChangePrefabCount(ref Q_56, add);
			break;
		case 70:
			ChangePrefabCount(ref Q_70, add);
			break;
		case 71:
			ChangePrefabCount(ref Q_71, add);
			break;
		case 72:
			ChangePrefabCount(ref Q_72, add);
			break;
		case 73:
			ChangePrefabCount(ref Q_73, add);
			break;
		case 74:
			ChangePrefabCount(ref Q_74, add);
			break;
		case 75:
			ChangePrefabCount(ref Q_75, add);
			break;
		case 80:
			ChangePrefabCount(ref Q_80, add);
			break;
		case 81:
			ChangePrefabCount(ref Q_81, add);
			break;
		case 82:
			ChangePrefabCount(ref Q_82, add);
			break;
		case 83:
			ChangePrefabCount(ref Q_83, add);
			break;
		case 84:
			ChangePrefabCount(ref Q_84, add);
			break;
		case 85:
			ChangePrefabCount(ref Q_85, add);
			break;
		case 86:
			ChangePrefabCount(ref Q_86, add);
			break;
		case 90:
			ChangePrefabCount(ref Q_90, add);
			break;
		case 0:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 46:
		case 47:
		case 48:
		case 49:
		case 50:
		case 57:
		case 58:
		case 59:
		case 60:
		case 61:
		case 62:
		case 63:
		case 64:
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 76:
		case 77:
		case 78:
		case 79:
		case 87:
		case 88:
		case 89:
			break;
		}
	}

	private void ChangePrefabCount(ref int count, bool add)
	{
		if (add)
		{
			count++;
		}
		else if (count > 0)
		{
			count--;
		}
	}
}
