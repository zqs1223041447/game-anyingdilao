using System.Collections.Generic;
using Data.RuntimeData.Skills.CompSkill;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Gamepad;
using Lean.Pool;
using Scenes;
using Spine;
using UnityEngine;
using Utils;

public class Gun : ScopedSingletonMono<Gun>
{
	private struct CastSnapshot
	{
		public ACTListSkillBT Skill;

		public Vector3 TargetPos;

		public bool HasTargetPos;

		public int SkillSlotIndex;
	}

	public Transform pointA;

	public Transform pointB;

	public Transform MGCpointA;

	public Transform MGCpointB;

	public Transform MGCpointC;

	public Transform MGCpointD;

	public Transform SQSpointA;

	public Transform SQSpointB;

	public Transform SQSpointC;

	public Transform SQSpointD;

	public Transform ARCpointA;

	public Transform ARCpointB;

	public Transform ARCpointC;

	public Transform ARCpointD;

	public Transform DEADPointA;

	public Transform DEADPointB;

	public Transform DEADPointC;

	public Transform DEADPointD;

	public static Vector3 MousePos;

	public static Vector3 FootPos;

	[HideInInspector]
	public int Index;

	private RaycastHit2D ray;

	private Vector3 Limit;

	private PlayerManager pl;

	private ACTbar ACT;

	private GameDataManager _gameDataManager;

	private float MainZ;

	private Vector3 MainVecter;

	private ACTListSkillBT directActSkill;

	private Vector3 directTargetPos;

	private bool directHasTargetPos;

	private readonly Queue<CastSnapshot> castSnapshots = new Queue<CastSnapshot>();

	private readonly Dictionary<TrackEntry, CastSnapshot> castSnapshotsByTrack = new Dictionary<TrackEntry, CastSnapshot>();

	private CastSnapshot activeCastSnapshot;

	private bool hasActiveCastSnapshot;

	private TrackEntry currentEventTrack;

	private bool targetPointCastUsingAutoLock;

	private const float FlySGamepadAimDistance = 5f;

	private Transform flySGamepadAimTarget;

	private Vector3 ATtrans { get; set; }

	private int CurrentSimpleObj => GetCurrentActSkill().DT.simple.OBJ;

	private int CurrentCompObj => GetCurrentActSkill().DT.comp.OBJ;

	protected override void Awake()
	{
		base.Awake();
		pointA = base.transform.Find("pointA").GetComponent<Transform>();
		pointB = base.transform.Find("pointB").GetComponent<Transform>();
	}

	private void Start()
	{
		Index = 0;
		pl = SingletonMonoScope<PlayerManager>.Instance;
		ACT = SingletonMonoScope<ACTbar>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Update()
	{
		AimContext currentAimContext = AimProvider.GetCurrentAimContext();
		Vector3 vector = new Vector3(currentAimContext.WorldPoint.x, currentAimContext.WorldPoint.y, 0f);
		MousePos = (pl ? pl.GetBattleAimWorldPosition(vector, null, null) : vector);
		FootPos = MousePos;
		if ((bool)pl && pl.IsAutoLockActive() && pl.TryGetAutoLockFootPosition(out var position))
		{
			FootPos = position;
		}
		UpdateFlySGamepadAimTarget();
		MainVecter = MousePos - base.transform.position;
		MainZ = Mathf.Atan2(MainVecter.y, MainVecter.x) * 57.29578f;
		base.transform.rotation = Quaternion.Euler(0f, 0f, MainZ);
	}

	public SkillOBJ_DT_SP CreatSP()
	{
		BeginAnimationEventCast();
		ACTListSkillBT currentActSkill = GetCurrentActSkill();
		ACT_skillSample simple = currentActSkill.DT.simple;
		simple.EnsureRuntimeBuffDefaults();
		SkillOBJ_DT_SP component = LeanPool.Spawn(GetSkillPrefab(simple), base.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
		component.FStype = simple.FStype;
		component.LockType = simple.LockType;
		component.indexType = 0;
		component.pl = pl;
		component.ZY = true;
		component.TargetPos = ResolveTargetPoint(simple.Distance, simple.FStype != 3, allowAutoLock: true, simple.LockType);
		component.skillName = currentActSkill.IndexName;
		component.RTtypeOBJ = simple.RTtypeOBJ;
		component.RTtypeFX = simple.RTtypeFX;
		component.Distance = simple.Distance;
		component.GlobalID = 100000;
		component.damageType = simple.damageType;
		component.MainEL = simple.MainEL;
		component.ThroughType = simple.ThroughType;
		component.AttackType = simple.AttackType;
		component.AttackTypeA = simple.AttackTypeA;
		component.AttackTypeB = simple.AttackTypeB;
		float jCSkillDamage = ACT.GetJCSkillDamage(currentActSkill);
		float num = jCSkillDamage * 0.3f;
		component.Damage = (simple.Damage + jCSkillDamage) / 100f * pl.GiveDamage(component.damageType);
		component.DamageA = (simple.DamageA + num) / 100f * pl.GiveDamage(component.damageType);
		component.DamageB = (simple.DamageB + num) / 100f * pl.GiveDamage(component.damageType);
		component.BJrate = simple.BJrate + pl.BJrate_Last;
		component.BJDamage = simple.BJDamage + pl.BJDamage_Last;
		component.JYrate = simple.JYrate + pl.JYrate_Last;
		component.Through = simple.Through + pl.ThroughRate;
		component.FlySpeed = simple.FlySpeed + pl.FlySpeed;
		component.MoveSpeedCut = simple.MoveSpeedCut;
		component.AttackSpeedCut = simple.AttackSpeedCut;
		component.AntiCut = simple.AntiCut;
		component.BF_Damage = simple.BF_Damage;
		component.BF_EL_Damage = simple.BF_EL_Damage;
		component.BF_EL_Chuan = simple.BF_EL_Chuan;
		component.BF_BJrate = simple.BF_BJrate;
		component.BF_JYrate = simple.BF_JYrate;
		component.BF_GeDang = simple.BF_GeDang;
		component.BF_AttackSpeed = simple.BF_AttackSpeed;
		component.BF_MoveSpeed = simple.BF_MoveSpeed;
		component.BF_DamageAnti = simple.BF_DamageAnti;
		component.BF_Health_Prc = simple.BF_Health_Prc;
		component.C_Damage = simple.C_Damage;
		component.C_ATspeed = simple.C_ATspeed;
		component.C_MVspeed = simple.C_MVspeed;
		component.C_Health_Prc = simple.C_Health_Prc;
		component.CF_Rate = simple.CF_Rate;
		component.CF_Type = simple.CF_Type;
		component.CF_Count = simple.CF_Count;
		component.Layer_SubA = simple.Layer_SubA;
		component.Layer_SubB = simple.Layer_SubB;
		component.BSAT = simple.BSAT;
		component.BSAT_Count = simple.BSAT_Count;
		component.BSAT_Angle = simple.BSAT_Angle;
		component.Is_BS = simple.Is_BS;
		component.ChangeSkin = simple.ChangeSkin;
		component.SkinIndex = simple.SkinIndex;
		component.Reborn = simple.Reborn;
		component.BSAT_Damage = simple.BSAT_Damage;
		component.BSAT_DMG = simple.BSAT_DMG;
		component.AutoUse = simple.AutoUse;
		component.Refresh = simple.Refresh;
		component.CompUP_DMG = simple.CompUP_DMG;
		component.ATtarUP = simple.ATtarUP;
		component.MS_Dead = simple.MS_Dead;
		component.GD_Use = simple.GD_Use;
		component.JCskill = simple.JCskill;
		component.LinkSK = simple.LinkSK;
		component.LinkAll = simple.LinkAll;
		component.EveryLink = simple.EveryLink;
		component.LastSkill = simple.LastSkill;
		component.DashSkill = simple.DashSkill;
		component.TPSkill = simple.TPSkill;
		component.UseDMG = simple.UseDMG;
		component.UseATS = simple.UseATS;
		component.UseMVS = simple.UseMVS;
		if (component.UseDMG_EL == null || component.UseDMG_EL.Length < 6)
		{
			component.UseDMG_EL = new int[6];
		}
		if (component.UseChuan == null || component.UseChuan.Length < 6)
		{
			component.UseChuan = new int[6];
		}
		for (int i = 0; i < 6; i++)
		{
			component.UseDMG_EL[i] = ((simple.UseDMG_EL != null && simple.UseDMG_EL.Length > i) ? simple.UseDMG_EL[i] : 0);
			component.UseChuan[i] = ((simple.UseChuan != null && simple.UseChuan.Length > i) ? simple.UseChuan[i] : 0);
		}
		component.UseCP_DMG = simple.UseCP_DMG;
		component.UseCP_ATS = simple.UseCP_ATS;
		component.Has_DMG = simple.Has_DMG;
		component.Has_ATS = simple.Has_ATS;
		component.Has_MVS = simple.Has_MVS;
		component.Has_BJR = simple.Has_BJR;
		component.Has_BJD = simple.Has_BJD;
		component.Has_DotTimeCut = simple.Has_DotTimeCut;
		component.Has_DMG_Cut = simple.Has_DMG_Cut;
		component.Has_GD = simple.Has_GD;
		component.Has_ORB_DMG = simple.Has_ORB_DMG;
		component.Has_XJ_DMG = simple.Has_XJ_DMG;
		component.Has_Dot_DMG = simple.Has_Dot_DMG;
		component.Has_CP_DMG = simple.Has_CP_DMG;
		component.WD = simple.WD;
		component.Crit_Time = simple.Crit_Time;
		component.Crit_CD = simple.Crit_CD;
		component.Over_Prc = simple.Over_Prc;
		component.CutSpeedZone = simple.CutSpeedZone;
		component.SPC_Damage = simple.Damage;
		component.SPC_DamageA = simple.DamageA;
		component.SPC_DamageB = simple.DamageB;
		component.TypeORB = simple.TypeORB;
		simple.CF_Type = component.CF_Type;
		component.Type_F = simple.Type_F;
		component.Type_S = simple.Type_S;
		component.Type_AB = simple.Type_AB;
		component.TypeDIC_F = simple.TypeDIC_F;
		component.TypeDIC_S = simple.TypeDIC_S;
		component.TypeEXP_F = simple.TypeEXP_F;
		component.TypeEXP_S = simple.TypeEXP_S;
		component.TypeEXP_AB = simple.TypeEXP_AB;
		component.Size = simple.Size;
		component.High = simple.High;
		component.JG = simple.JG;
		component.AngleA = simple.AngleA;
		component.AngleB = simple.AngleB;
		component.Range1 = simple.Range1;
		component.Range2 = simple.Range2;
		component.Range_AT = simple.Range_AT;
		component.FStime1 = simple.FStime1;
		component.FStime2 = simple.FStime2;
		component.Speed1 = simple.Speed1;
		component.Speed2 = simple.Speed2;
		component.Speed3 = simple.Speed3;
		component.Speed4 = simple.Speed4;
		component.Count_ORB = simple.Count_ORB;
		component.Count_ATtarget = simple.Count_ATtarget;
		component.ATtar_DMG = simple.ATtar_DMG;
		component.Count_F = simple.Count_F;
		component.Count_S = simple.Count_S;
		component.Count_AB = simple.Count_AB;
		component.CountMulti = simple.CountMulti;
		component.CountEXP = simple.CountEXP;
		component.NoTime = simple.NoTime;
		component.BuffTime = simple.BuffTime;
		component.DebuffTime = simple.DebuffTime;
		component.Field_time = simple.Field_time;
		component.ORB_time = simple.ORB_time;
		component.EXP_time = simple.EXP_time;
		component.ZD_time_F = simple.ZD_time_F;
		component.ZD_time_S = simple.ZD_time_S;
		component.ORB = simple.ORB;
		component.ZD_F = simple.ZD_F;
		component.ZD_S = simple.ZD_S;
		component.ZD_AB = simple.ZD_AB;
		component.EXP_F = simple.EXP_F;
		component.EXP_S = simple.EXP_S;
		component.EXP_AB = simple.EXP_AB;
		component.Dic_F = simple.Dic_F;
		component.Dic_S = simple.Dic_S;
		component.FX_F = simple.FX_F;
		component.FX_S = simple.FX_S;
		component.Sound = simple.Sound;
		component.Follow_F = simple.Follow_F;
		component.Follow_S = simple.Follow_S;
		component.AllChuan_F = simple.AllChuan_F;
		component.AllChuan_S = simple.AllChuan_S;
		component.Slow_F = simple.Slow_F;
		component.Slow_S = simple.Slow_S;
		component.RDSpeed_F = simple.RDSpeed_F;
		component.RDSpeed_S = simple.RDSpeed_S;
		component.HasFX = simple.HasFX;
		component.S_HasFX = simple.S_HasFX;
		component.AB_HasFX = simple.A_HasFX;
		component.colEXP = simple.colEXP;
		component.colEXP_A = simple.colEXP_A;
		component.S_colEXP = simple.S_colEXP;
		component.AB_colEXP = simple.A_colEXP;
		component.TimeEXP = simple.TimeEXP;
		component.TimeEXP_AB = simple.TimeEXP_A;
		component.LastEXP = simple.LastEXP;
		component.LastEXP_AB = simple.LastEXP_A;
		component.S_LastEXP = simple.S_LastEXP;
		component.AB_LastEXP = simple.A_LastEXP;
		component.EXPpos = simple.EXPpos;
		component.EXPpos_AB = simple.EXPpos_A;
		component.S_EXPpos = simple.S_EXPpos;
		component.AB_EXPpos = simple.A_EXPpos;
		component.AngleEXP = simple.AngleEXP;
		component.AngleEXP_AB = simple.AngleEXP_A;
		component.Dot_Infect = false;
		component.Dot_Infect_Layer = 0;
		pl.BuffRuntime?.RegisterSkillInstance(currentActSkill, component);
		return component;
	}

	private GameObject GetSkillPrefab(ACT_skillSample sp)
	{
		return _gameDataManager.SKPB.SK_Group[sp.OBJ_Group].OBJ[sp.OBJ];
	}

	private ACTListSkillBT GetCurrentActSkill()
	{
		if ((bool)directActSkill)
		{
			return directActSkill;
		}
		if (hasActiveCastSnapshot && (bool)activeCastSnapshot.Skill)
		{
			return activeCastSnapshot.Skill;
		}
		return ACT.skillBT[pl.CurUseSK].actL;
	}

	private Vector3 GetCurrentTargetPos()
	{
		if (directHasTargetPos)
		{
			return directTargetPos;
		}
		if (hasActiveCastSnapshot && activeCastSnapshot.HasTargetPos)
		{
			return activeCastSnapshot.TargetPos;
		}
		return MousePos;
	}

	private Vector3 GetSkillAimPosition(SkillOBJ_DT_SP dt)
	{
		if ((bool)dt && dt.LockType == 1 && pl != null && pl.IsAutoLockActive() && pl.TryGetAutoLockFootPosition(out var position))
		{
			return position;
		}
		return MousePos;
	}

	private Vector3 GetSkillAimVector(SkillOBJ_DT_SP dt, Vector3 origin)
	{
		return GetSkillAimPosition(dt) - origin;
	}

	private float GetMouseAimAngle(Vector3 origin)
	{
		Vector3 vector = MousePos - origin;
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	private Vector3 ResolveTargetPoint(float distance, bool useGamepadSkillDistance, bool allowAutoLock, int lockType = 1)
	{
		targetPointCastUsingAutoLock = false;
		if (pl != null && allowAutoLock && pl.IsAutoLockActive() && TryGetAutoLockTargetPosition(lockType, out var autoTarget))
		{
			targetPointCastUsingAutoLock = true;
			return ClampPointToDistance(base.transform.position, autoTarget, Mathf.Max(0f, distance));
		}
		Vector3 currentTargetPos = GetCurrentTargetPos();
		if (!IsGamepadCurrent())
		{
			return currentTargetPos;
		}
		float targetPointCastDistance = GetTargetPointCastDistance(distance, useGamepadSkillDistance);
		return ProjectPointToDistance(base.transform.position, currentTargetPos, targetPointCastDistance);
	}

	private bool TryGetAutoLockTargetPosition(int lockType, out Vector3 autoTarget)
	{
		if (lockType == 1)
		{
			return pl.TryGetAutoLockFootPosition(out autoTarget);
		}
		return pl.TryGetAutoLockYaoPosition(out autoTarget);
	}

	private float GetTargetPointCastDistance(float distance, bool useGamepadSkillDistance)
	{
		if (!useGamepadSkillDistance || !IsGamepadCurrent() || targetPointCastUsingAutoLock || IsCurrentSkillNormalAttack())
		{
			return distance;
		}
		int currentSkillSlotIndex = GetCurrentSkillSlotIndex();
		int gamepadSkillDistancePercent = Singleton<SettingDataManager>.Instance.GetGamepadSkillDistancePercent(currentSkillSlotIndex);
		gamepadSkillDistancePercent = Mathf.Clamp(gamepadSkillDistancePercent, 10, 100);
		return distance * (float)gamepadSkillDistancePercent / 100f;
	}

	private bool IsCurrentSkillNormalAttack()
	{
		ACTListSkillBT currentActSkill = GetCurrentActSkill();
		if (currentActSkill != null && currentActSkill.DT != null)
		{
			return currentActSkill.DT.SampleSkill;
		}
		return false;
	}

	private int GetCurrentSkillSlotIndex()
	{
		if ((bool)directActSkill)
		{
			int num = FindSkillSlotIndex(directActSkill);
			if (num > 0)
			{
				return num;
			}
		}
		if (hasActiveCastSnapshot && activeCastSnapshot.SkillSlotIndex > 0)
		{
			return Mathf.Clamp(activeCastSnapshot.SkillSlotIndex, 1, 8);
		}
		int num2 = FindSkillSlotIndex(GetCurrentActSkill());
		if (num2 > 0)
		{
			return num2;
		}
		if (pl != null)
		{
			return Mathf.Clamp(pl.CurUseSK + 1, 1, 8);
		}
		return 0;
	}

	private int FindSkillSlotIndex(ACTListSkillBT skill)
	{
		if (!skill || ACT == null || ACT.skillBT == null)
		{
			return 0;
		}
		for (int i = 0; i < ACT.skillBT.Length && i < 8; i++)
		{
			if ((bool)ACT.skillBT[i] && ACT.skillBT[i].actL == skill)
			{
				return i + 1;
			}
		}
		return 0;
	}

	private bool IsGamepadCurrent()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent();
		}
		return false;
	}

	public static Vector3 GetFlySAimWorldPos()
	{
		Gun instance = SingletonMonoScope<Gun>.Instance;
		if (!instance || !instance.IsGamepadCurrent())
		{
			return MousePos;
		}
		Transform transform = instance.GetFlySGamepadAimTarget();
		if (!transform)
		{
			return MousePos;
		}
		return transform.position;
	}

	private Transform GetFlySGamepadAimTarget()
	{
		UpdateFlySGamepadAimTarget();
		return flySGamepadAimTarget;
	}

	private void UpdateFlySGamepadAimTarget()
	{
		if (IsGamepadCurrent())
		{
			EnsureFlySGamepadAimTarget();
			if ((bool)flySGamepadAimTarget)
			{
				flySGamepadAimTarget.position = CalculateFlySGamepadAimPosition();
			}
		}
	}

	private void EnsureFlySGamepadAimTarget()
	{
		if (!flySGamepadAimTarget)
		{
			GameObject gameObject = new GameObject("FlyS_GamepadAimDistance6");
			Transform flySGamepadAimOrigin = GetFlySGamepadAimOrigin();
			if ((bool)flySGamepadAimOrigin)
			{
				gameObject.transform.SetParent(flySGamepadAimOrigin);
			}
			flySGamepadAimTarget = gameObject.transform;
		}
	}

	private Transform GetFlySGamepadAimOrigin()
	{
		if ((bool)pl)
		{
			return pl.transform;
		}
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		if (!instance)
		{
			return base.transform;
		}
		return instance.transform;
	}

	private Vector3 CalculateFlySGamepadAimPosition()
	{
		Transform flySGamepadAimOrigin = GetFlySGamepadAimOrigin();
		Vector3 vector = (flySGamepadAimOrigin ? flySGamepadAimOrigin.position : base.transform.position);
		Vector3 vector2 = MousePos - vector;
		vector2.z = 0f;
		if (vector2.sqrMagnitude <= 0.0001f)
		{
			vector2 = base.transform.right;
		}
		Vector3 result = vector + vector2.normalized * 5f;
		result.z = 0f;
		return result;
	}

	private static Vector3 ClampPointToDistance(Vector3 origin, Vector3 target, float distance)
	{
		Vector3 vector = target - origin;
		vector.z = 0f;
		if (vector.sqrMagnitude <= 0.0001f)
		{
			return target;
		}
		if (distance <= 0f)
		{
			return origin;
		}
		float magnitude = vector.magnitude;
		if (magnitude <= distance)
		{
			return target;
		}
		return origin + vector / magnitude * distance;
	}

	private static Vector3 ProjectPointToDistance(Vector3 origin, Vector3 target, float distance)
	{
		Vector3 vector = target - origin;
		vector.z = 0f;
		if (vector.sqrMagnitude <= 0.0001f)
		{
			return target;
		}
		if (distance <= 0f)
		{
			return origin;
		}
		return origin + vector.normalized * distance;
	}

	private void NotifyCurrentSkillUsed()
	{
		if (!directActSkill)
		{
			pl.NotifyCompanionFollowSkillFired(MousePos);
			pl.BuffRuntime?.OnSkillUsed(GetCurrentActSkill());
		}
	}

	public void QueueCastSnapshot(ACTListSkillBT skill, Vector3 targetPos, int skillSlotIndex = 0)
	{
		if ((bool)skill && skill.DT != null)
		{
			castSnapshots.Clear();
			castSnapshots.Enqueue(new CastSnapshot
			{
				Skill = skill,
				TargetPos = targetPos,
				HasTargetPos = IsTargetPointSkill(skill),
				SkillSlotIndex = ((skillSlotIndex > 0) ? skillSlotIndex : FindSkillSlotIndex(skill))
			});
		}
	}

	public void BindQueuedCastSnapshotToTrack(TrackEntry trackEntry)
	{
		if (trackEntry != null && castSnapshots.Count > 0)
		{
			CastSnapshot value = castSnapshots.Dequeue();
			castSnapshotsByTrack[trackEntry] = value;
			trackEntry.End -= RemoveCastSnapshotTrack;
			trackEntry.Dispose -= RemoveCastSnapshotTrack;
			trackEntry.End += RemoveCastSnapshotTrack;
			trackEntry.Dispose += RemoveCastSnapshotTrack;
		}
	}

	public void SetAnimationEventTrack(TrackEntry trackEntry)
	{
		currentEventTrack = trackEntry;
	}

	private void RemoveCastSnapshotTrack(TrackEntry trackEntry)
	{
		castSnapshotsByTrack.Remove(trackEntry);
		if (currentEventTrack == trackEntry)
		{
			currentEventTrack = null;
		}
	}

	private bool TryDequeueLatestCastSnapshot(out CastSnapshot snapshot)
	{
		snapshot = default(CastSnapshot);
		if (castSnapshots.Count <= 0)
		{
			return false;
		}
		while (castSnapshots.Count > 0)
		{
			snapshot = castSnapshots.Dequeue();
		}
		return snapshot.Skill != null;
	}

	private bool IsTargetPointSkill(ACTListSkillBT skill)
	{
		if ((bool)skill && skill.DT != null && skill.DT.type == 0 && skill.DT.simple != null)
		{
			return skill.DT.simple.FStype == 10;
		}
		return false;
	}

	private void BeginAnimationEventCast()
	{
		if ((bool)directActSkill)
		{
			return;
		}
		activeCastSnapshot = default(CastSnapshot);
		hasActiveCastSnapshot = false;
		if (TryDequeueLatestCastSnapshot(out activeCastSnapshot))
		{
			hasActiveCastSnapshot = true;
			if (currentEventTrack != null)
			{
				castSnapshotsByTrack[currentEventTrack] = activeCastSnapshot;
			}
		}
		else if (currentEventTrack != null && castSnapshotsByTrack.TryGetValue(currentEventTrack, out activeCastSnapshot))
		{
			hasActiveCastSnapshot = activeCastSnapshot.Skill;
			_ = hasActiveCastSnapshot;
		}
	}

	private void EndAnimationEventCast()
	{
		if (!directActSkill)
		{
			activeCastSnapshot = default(CastSnapshot);
			hasActiveCastSnapshot = false;
			currentEventTrack = null;
			targetPointCastUsingAutoLock = false;
		}
	}

	private bool BeginTargetPointCast(SkillOBJ_DT_SP dt, out Vector3 oldMousePos)
	{
		oldMousePos = MousePos;
		targetPointCastUsingAutoLock = false;
		if (!dt || dt.FStype != 10)
		{
			return false;
		}
		MousePos = ResolveTargetPoint(dt.Distance, useGamepadSkillDistance: true, allowAutoLock: true, dt.LockType);
		dt.TargetPos = MousePos;
		return true;
	}

	private void EndTargetPointCast(bool usedSnapshotTarget, Vector3 oldMousePos)
	{
		targetPointCastUsingAutoLock = false;
		if (usedSnapshotTarget)
		{
			MousePos = oldMousePos;
		}
	}

	public void CastDirect(ACTListSkillBT skill)
	{
		if (!skill || skill.DT == null)
		{
			return;
		}
		ACTListSkillBT aCTListSkillBT = directActSkill;
		Vector3 vector = directTargetPos;
		bool flag = directHasTargetPos;
		directActSkill = skill;
		directTargetPos = MousePos;
		directHasTargetPos = IsTargetPointSkill(skill);
		int curUseSK = pl.CurUseSK;
		bool iScomp = pl.IScomp;
		try
		{
			switch (skill.DT.type)
			{
			case 0:
				pl.IScomp = false;
				CastCurrentSampleByPlayerType();
				break;
			case 1:
				pl.IScomp = true;
				Summon();
				break;
			}
		}
		finally
		{
			pl.IScomp = iScomp;
			pl.CurUseSK = curUseSK;
			directActSkill = aCTListSkillBT;
			directTargetPos = vector;
			directHasTargetPos = flag;
		}
	}

	private void CastCurrentSampleByPlayerType()
	{
		switch (pl.PLType)
		{
		case 0:
			MGCattack();
			break;
		case 1:
			SQSattack();
			break;
		case 2:
			ARCattack();
			break;
		case 3:
			DEADattack();
			break;
		}
	}

	public SK_FSQ_comp CreatCP(out CompanionRuntimeData data)
	{
		ACTListSkillBT currentActSkill = GetCurrentActSkill();
		ACT_skillComp comp = currentActSkill.DT.comp;
		data = null;
		if (comp == null)
		{
			return null;
		}
		string indexName = currentActSkill.IndexName;
		SK_FSQ_comp component = LeanPool.Spawn(_gameDataManager.SKPB.CP_OBJ[comp.OBJ], base.transform.position, Quaternion.identity).GetComponent<SK_FSQ_comp>();
		data = SetCPData(new CompanionRuntimeData(), comp, indexName);
		return component;
	}

	public CompanionRuntimeData SetCPData(CompanionRuntimeData data, ACT_skillComp cp, string skillIndexName)
	{
		data.indexType = 0;
		data.pl = SingletonMonoScope<PlayerManager>.Instance;
		data.ZY = true;
		data.skillName = skillIndexName;
		data.Distance = cp.Distance;
		data.BStype = cp.BStype;
		data.AT_ZD = cp.AT_ZD;
		data.SK_ZD = cp.SK_ZD;
		data.AT_DMG = cp.AT_DMG;
		data.SK_DMG = cp.SK_DMG;
		data.Damage = cp.Damage;
		data.Health = cp.Health;
		data.Health_Prc = cp.Health_Prc;
		data.AttackSpeed = cp.AttackSpeed;
		data.GeDang = cp.GeDang;
		data.damageType = cp.damageType;
		data.damageType_Change = cp.damageType_Change;
		data.Change_AT = cp.Change_AT;
		data.ATSrate = cp.ATSrate;
		data.ChangeEL_SK = cp.ChangeEL_SK;
		data.ATS_Damage = cp.ATS_Damage;
		data.ChangeEL_AR = cp.ChangeEL_AR;
		data.ARS_Damage = cp.ARS_Damage;
		data.DotMultiA = cp.DotMultiA;
		data.DotMultiB = cp.DotMultiB;
		data.GD_R_Heal = cp.GD_R_Heal;
		data.BloodDie = cp.BloodDie;
		data.TGYJ = cp.TGYJ;
		data.Kill_R_Heal = cp.Kill_R_Heal;
		data.Hurt_FT = cp.Hurt_FT;
		data.AT_DotLayer = cp.AT_DotLayer;
		data.BJ_NoDot = cp.BJ_NoDot;
		data.WS_All = cp.WS_All;
		data.Field_Range = cp.Field_Range;
		data.DisA = cp.DisA;
		data.DisB = cp.DisB;
		data.Idle_Time_Min = cp.Idle_Time_Min;
		data.Idle_Time_Max = cp.Idle_Time_Max;
		data.SO_IdleRate = cp.SO_IdleRate;
		data.SO_AttackRate = cp.SO_AttackRate;
		data.SO_SayRate = cp.SO_SayRate;
		data.SO_HurtRate = cp.SO_HurtRate;
		data.SO_DieRate = cp.SO_DieRate;
		data.SO_Idle = cp.SO_Idle;
		data.SO_Walk = cp.SO_Walk;
		data.SO_AttackA = cp.SO_AttackA;
		data.SO_SayA = cp.SO_SayA;
		data.SO_AttackB = cp.SO_AttackB;
		data.SO_SayB = cp.SO_SayB;
		data.SO_AttackC = cp.SO_AttackC;
		data.SO_SayC = cp.SO_SayC;
		data.SO_Hurt = cp.SO_Hurt;
		data.SO_Die = cp.SO_Die;
		data.Type_A = cp.Type_A;
		data.Type_B = cp.Type_B;
		data.TypeDIC_A = cp.TypeDIC_A;
		data.TypeDIC_B = cp.TypeDIC_B;
		data.JG_A = cp.JG_A;
		data.JG_B = cp.JG_B;
		data.AngleA = cp.AngleA;
		data.AngleB = cp.AngleB;
		data.FStimeA = cp.FStimeA;
		data.FStimeB = cp.FStimeB;
		data.Count_A = cp.Count_A;
		data.Count_B = cp.Count_B;
		data.AT_Double = cp.AT_Double;
		data.Count_ATtarget_A = cp.Count_ATtarget_A;
		data.Count_ATtarget_B = cp.Count_ATtarget_B;
		data.CountMulti_A = cp.CountMulti_A;
		data.CountMulti_B = cp.CountMulti_B;
		data.Follow_A = cp.Follow_A;
		data.Follow_B = cp.Follow_B;
		data.AllChuan_A = cp.AllChuan_A;
		data.AllChuan_B = cp.AllChuan_B;
		data.RDSpeed_A = cp.RDSpeed_A;
		data.RDSpeed_B = cp.RDSpeed_B;
		data.HasFX_A = cp.HasFX_A;
		data.HasFX_B = cp.HasFX_B;
		data.colEXP_A = cp.colEXP_A;
		data.colEXP_B = cp.colEXP_B;
		data.EXPpos_A = cp.EXPpos_A;
		data.EXPpos_B = cp.EXPpos_B;
		return data;
	}

	public void MGCattack()
	{
		SkillOBJ_DT_SP dt = CreatSP();
		Vector3 oldMousePos;
		bool usedSnapshotTarget = BeginTargetPointCast(dt, out oldMousePos);
		switch (dt.FStype)
		{
		case 0:
		{
			Vector3 skillAimVector9 = GetSkillAimVector(dt, MGCpointA.transform.position);
			float num2 = Mathf.Atan2(skillAimVector9.y, skillAimVector9.x) * 57.29578f;
			dt.transform.position = MGCpointA.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, num2);
				break;
			case 1:
				dt.dic = new Vector2(Mathf.Cos(num2 * 3.14f / 180f), Mathf.Sin(num2 * 3.14f / 180f));
				break;
			}
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointA.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(MGCpointA.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointA.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, MGCpointA, num2, GetSkillAimPosition(dt));
			break;
		}
		case 1:
		{
			Vector3 skillAimVector2 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z = Mathf.Atan2(skillAimVector2.y, skillAimVector2.x) * 57.29578f;
			dt.transform.position = pointA.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, z);
				break;
			case 1:
				dt.dic = new Vector2(skillAimVector2.x, skillAimVector2.y);
				break;
			}
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointA.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.yao.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointA.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pointA, z, GetSkillAimPosition(dt));
			break;
		}
		case 2:
		{
			Vector3 skillAimVector3 = GetSkillAimVector(dt, pl.head.transform.position);
			float z2 = Mathf.Atan2(skillAimVector3.y, skillAimVector3.x) * 57.29578f;
			dt.transform.position = MGCpointC.position;
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointC.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.head.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointC.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, MGCpointC, z2, GetSkillAimPosition(dt));
			break;
		}
		case 3:
		{
			CapsuleCollider2D component = pl.GetComponent<CapsuleCollider2D>();
			Vector2 origin = pl.transform.position;
			Vector3 movementSkillWorldPoint = pl.GetMovementSkillWorldPoint(dt.Distance);
			if (TeleportUtil.GetSafeTeleportPosition(origin, movementSkillWorldPoint, dt.Distance, component, LayerMask.GetMask("block"), out var safePos))
			{
				StartCoroutine(SingletonMonoScope<PlayerManager>.Instance.TeleportRoutine(safePos));
				pl.ChuanSongPOS = safePos;
				dt.transform.position = safePos;
			}
			else
			{
				StartCoroutine(SingletonMonoScope<PlayerManager>.Instance.TeleportRoutine(pl.transform.position));
				pl.ChuanSongPOS = pl.transform.position;
				dt.transform.position = pl.transform.position;
			}
			Vector3 vector = movementSkillWorldPoint - pl.transform.position;
			float z8 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, dt.transform, z8, GetSkillAimPosition(dt));
			break;
		}
		case 4:
		{
			Vector3 skillAimVector8 = GetSkillAimVector(dt, pl.transform.position);
			float z7 = Mathf.Atan2(skillAimVector8.y, skillAimVector8.x) * 57.29578f;
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z7, GetSkillAimPosition(dt));
			break;
		}
		case 5:
		{
			Vector3 skillAimVector7 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z6 = Mathf.Atan2(skillAimVector7.y, skillAimVector7.x) * 57.29578f;
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z6, GetSkillAimPosition(dt));
			break;
		}
		case 6:
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, 0f, GetSkillAimPosition(dt));
			break;
		case 7:
		{
			Vector3 skillAimVector5 = GetSkillAimVector(dt, pl.transform.position);
			float z4 = Mathf.Atan2(skillAimVector5.y, skillAimVector5.x) * 57.29578f;
			dt.transform.SetParent(pl.transform);
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z4, GetSkillAimPosition(dt));
			break;
		}
		case 8:
		{
			Vector3 skillAimVector6 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z5 = Mathf.Atan2(skillAimVector6.y, skillAimVector6.x) * 57.29578f;
			dt.transform.SetParent(pl.yao.transform);
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z5, GetSkillAimPosition(dt));
			break;
		}
		case 9:
		{
			Vector3 skillAimVector4 = GetSkillAimVector(dt, pl.headUp.transform.position);
			float z3 = Mathf.Atan2(skillAimVector4.y, skillAimVector4.x) * 57.29578f;
			dt.transform.SetParent(pl.headUp.transform);
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z3, GetSkillAimPosition(dt));
			break;
		}
		case 10:
		{
			Vector3 skillAimVector = GetSkillAimVector(dt, base.transform.position);
			float num = Mathf.Atan2(skillAimVector.y, skillAimVector.x) * 57.29578f;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], MGCpointB.position, Quaternion.identity);
			float targetPointCastDistance = GetTargetPointCastDistance(dt.Distance, useGamepadSkillDistance: true);
			float x = base.transform.position.x + targetPointCastDistance * Mathf.Cos(num * 3.14f / 180f);
			float y = base.transform.position.y + targetPointCastDistance * Mathf.Sin(num * 3.14f / 180f);
			Limit = new Vector3(x, y, base.transform.position.z);
			ray = Physics2D.Raycast(base.transform.position, MousePos - base.transform.position, Vector2.Distance(base.transform.position, MousePos), LayerMask.GetMask("block"));
			if ((bool)ray.collider)
			{
				if (ray.collider.CompareTag("blockWALL"))
				{
					ATtrans = new Vector3(ray.point.x, ray.point.y, 0f);
					if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
					{
						if (Vector3.Distance(Limit, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
						{
							dt.transform.position = ATtrans;
						}
						else
						{
							dt.transform.position = Limit;
						}
					}
					else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
					{
						dt.transform.position = ATtrans;
					}
					else
					{
						dt.transform.position = MousePos;
					}
				}
				else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
				{
					dt.transform.position = Limit;
				}
				else
				{
					dt.transform.position = MousePos;
				}
			}
			else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
			{
				dt.transform.position = Limit;
				Debug.Log(7111);
			}
			else
			{
				dt.transform.position = MousePos;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, dt.transform, num, GetSkillAimPosition(dt));
			break;
		}
		}
		PoeItemMod.SpawnExtraProjectiles(this, dt);
		if ((bool)SingletonMonoScope<ACTbar>.Instance.ATprefab)
		{
			Vector3 actPrefabTargetPos = dt.TargetPos;
			this.wait(0.0001f, delegate
			{
				ACTprefabFS(dt, actPrefabTargetPos);
			});
		}
		ACT.TryReleaseLinkedSkills(GetCurrentActSkill());
		NotifyCurrentSkillUsed();
		ACT.TryRefreshSkillCooldown(GetCurrentActSkill());
		EndTargetPointCast(usedSnapshotTarget, oldMousePos);
		EndAnimationEventCast();
	}

	public void SQSattack()
	{
		SkillOBJ_DT_SP dt = CreatSP();
		Vector3 oldMousePos;
		bool usedSnapshotTarget = BeginTargetPointCast(dt, out oldMousePos);
		switch (dt.FStype)
		{
		case 0:
		{
			Vector3 skillAimVector3 = GetSkillAimVector(dt, pl.yao.transform.position);
			float num2 = Mathf.Atan2(skillAimVector3.y, skillAimVector3.x) * 57.29578f;
			dt.transform.position = pointA.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, num2);
				break;
			case 1:
				dt.dic = new Vector2(Mathf.Cos(num2 * 3.14f / 180f), Mathf.Sin(num2 * 3.14f / 180f));
				break;
			}
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], pointA.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.yao.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], pointA.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pointA, num2, GetSkillAimPosition(dt));
			break;
		}
		case 1:
		{
			Vector3 skillAimVector10 = GetSkillAimVector(dt, pl.head.transform.position);
			float z8 = Mathf.Atan2(skillAimVector10.y, skillAimVector10.x) * 57.29578f;
			dt.transform.position = SQSpointB.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, z8);
				break;
			case 1:
				dt.dic = new Vector2(skillAimVector10.x, skillAimVector10.y);
				break;
			}
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointB.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.head.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointB.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, SQSpointB, z8, GetSkillAimPosition(dt));
			break;
		}
		case 2:
		{
			Vector3 skillAimVector2 = GetSkillAimVector(dt, pl.head.transform.position);
			float z = Mathf.Atan2(skillAimVector2.y, skillAimVector2.x) * 57.29578f;
			dt.transform.position = SQSpointC.position;
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointC.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.head.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointC.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, SQSpointC, z, GetSkillAimPosition(dt));
			break;
		}
		case 4:
		{
			Vector3 skillAimVector4 = GetSkillAimVector(dt, pl.transform.position);
			float z2 = Mathf.Atan2(skillAimVector4.y, skillAimVector4.x) * 57.29578f;
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointA.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z2, GetSkillAimPosition(dt));
			break;
		}
		case 5:
		{
			Vector3 skillAimVector9 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z7 = Mathf.Atan2(skillAimVector9.y, skillAimVector9.x) * 57.29578f;
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointA.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z7, GetSkillAimPosition(dt));
			break;
		}
		case 6:
		{
			Vector3 skillAimVector8 = GetSkillAimVector(dt, pl.headUp.transform.position);
			float z6 = Mathf.Atan2(skillAimVector8.y, skillAimVector8.x) * 57.29578f;
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointA.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z6, GetSkillAimPosition(dt));
			break;
		}
		case 7:
		{
			Vector3 skillAimVector6 = GetSkillAimVector(dt, pl.transform.position);
			float z4 = Mathf.Atan2(skillAimVector6.y, skillAimVector6.x) * 57.29578f;
			dt.transform.SetParent(pl.transform);
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointA.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z4, GetSkillAimPosition(dt));
			break;
		}
		case 8:
		{
			Vector3 skillAimVector5 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z3 = Mathf.Atan2(skillAimVector5.y, skillAimVector5.x) * 57.29578f;
			dt.transform.SetParent(pl.yao.transform);
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointA.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z3, GetSkillAimPosition(dt));
			break;
		}
		case 9:
		{
			Vector3 skillAimVector7 = GetSkillAimVector(dt, pl.head.transform.position);
			float z5 = Mathf.Atan2(skillAimVector7.y, skillAimVector7.x) * 57.29578f;
			dt.transform.SetParent(pl.headUp.transform);
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointA.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z5, GetSkillAimPosition(dt));
			break;
		}
		case 10:
		{
			Vector3 skillAimVector = GetSkillAimVector(dt, base.transform.position);
			float num = Mathf.Atan2(skillAimVector.y, skillAimVector.x) * 57.29578f;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], SQSpointA.position, Quaternion.identity);
			float targetPointCastDistance = GetTargetPointCastDistance(dt.Distance, useGamepadSkillDistance: true);
			float x = base.transform.position.x + targetPointCastDistance * Mathf.Cos(num * 3.14f / 180f);
			float y = base.transform.position.y + targetPointCastDistance * Mathf.Sin(num * 3.14f / 180f);
			Limit = new Vector3(x, y, base.transform.position.z);
			ray = Physics2D.Raycast(base.transform.position, MousePos - base.transform.position, Vector2.Distance(base.transform.position, MousePos), LayerMask.GetMask("block"));
			if ((bool)ray.collider)
			{
				if (ray.collider.CompareTag("blockWALL"))
				{
					ATtrans = new Vector3(ray.point.x, ray.point.y, 0f);
					if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
					{
						if (Vector3.Distance(Limit, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
						{
							dt.transform.position = ATtrans;
						}
						else
						{
							dt.transform.position = Limit;
						}
					}
					else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
					{
						dt.transform.position = ATtrans;
					}
					else
					{
						dt.transform.position = MousePos;
					}
				}
				else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
				{
					dt.transform.position = Limit;
				}
				else
				{
					dt.transform.position = MousePos;
				}
			}
			else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
			{
				dt.transform.position = Limit;
			}
			else
			{
				dt.transform.position = MousePos;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, dt.transform, num, GetSkillAimPosition(dt));
			break;
		}
		}
		PoeItemMod.SpawnExtraProjectiles(this, dt);
		if ((bool)SingletonMonoScope<ACTbar>.Instance.ATprefab)
		{
			Vector3 actPrefabTargetPos = dt.TargetPos;
			this.wait(0.0001f, delegate
			{
				ACTprefabFS(dt, actPrefabTargetPos);
			});
		}
		ACT.TryReleaseLinkedSkills(GetCurrentActSkill());
		NotifyCurrentSkillUsed();
		ACT.TryRefreshSkillCooldown(GetCurrentActSkill());
		EndTargetPointCast(usedSnapshotTarget, oldMousePos);
		EndAnimationEventCast();
	}

	public void ARCattack()
	{
		SkillOBJ_DT_SP dt = CreatSP();
		Vector3 oldMousePos;
		bool usedSnapshotTarget = BeginTargetPointCast(dt, out oldMousePos);
		switch (dt.FStype)
		{
		case 0:
		{
			Vector3 skillAimVector2 = GetSkillAimVector(dt, pl.head.transform.position);
			float num2 = Mathf.Atan2(skillAimVector2.y, skillAimVector2.x) * 57.29578f;
			dt.transform.position = ARCpointA.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, num2);
				break;
			case 1:
				dt.dic = new Vector2(Mathf.Cos(num2 * 3.14f / 180f), Mathf.Sin(num2 * 3.14f / 180f));
				break;
			}
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointA.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.head.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointA.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, ARCpointA, num2, GetSkillAimPosition(dt));
			break;
		}
		case 1:
		{
			Vector3 skillAimVector10 = GetSkillAimVector(dt, pl.transform.position);
			float z10 = Mathf.Atan2(skillAimVector10.y, skillAimVector10.x) * 57.29578f;
			dt.transform.position = pl.transform.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, z10);
				break;
			case 1:
				dt.dic = new Vector2(skillAimVector10.x, skillAimVector10.y);
				break;
			}
			Vector3 vector3 = ARCpointA.position - pl.yao.transform.position;
			float z11 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointA.position, Quaternion.Euler(0f, 0f, z11));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointA.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z10, GetSkillAimPosition(dt));
			break;
		}
		case 2:
		{
			Vector3 skillAimVector3 = GetSkillAimVector(dt, ARCpointC.position);
			float z2 = Mathf.Atan2(skillAimVector3.y, skillAimVector3.x) * 57.29578f;
			dt.transform.position = ARCpointC.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, z2);
				break;
			case 1:
				dt.dic = new Vector2(skillAimVector3.x, skillAimVector3.y);
				break;
			}
			Vector3 vector2 = ARCpointA.position - pl.yao.transform.position;
			float z3 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.Euler(0f, 0f, z3));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, ARCpointC, z2, GetSkillAimPosition(dt));
			break;
		}
		case 3:
		{
			CapsuleCollider2D component = pl.GetComponent<CapsuleCollider2D>();
			Vector2 origin = pl.transform.position;
			Vector3 movementSkillWorldPoint = pl.GetMovementSkillWorldPoint(dt.Distance);
			if (TeleportUtil.GetSafeTeleportPosition(origin, movementSkillWorldPoint, dt.Distance, component, LayerMask.GetMask("block"), out var safePos))
			{
				pl.ChuanSongPOS = safePos;
				dt.transform.position = safePos;
			}
			else
			{
				pl.ChuanSongPOS = pl.transform.position;
				dt.transform.position = pl.transform.position;
			}
			Vector3 vector4 = movementSkillWorldPoint - pl.transform.position;
			float z12 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z12, GetSkillAimPosition(dt));
			break;
		}
		case 4:
		{
			Vector3 skillAimVector9 = GetSkillAimVector(dt, pl.transform.position);
			float z9 = Mathf.Atan2(skillAimVector9.y, skillAimVector9.x) * 57.29578f;
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z9, GetSkillAimPosition(dt));
			break;
		}
		case 5:
		{
			Vector3 skillAimVector8 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z8 = Mathf.Atan2(skillAimVector8.y, skillAimVector8.x) * 57.29578f;
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z8, GetSkillAimPosition(dt));
			break;
		}
		case 6:
		{
			Vector3 skillAimVector6 = GetSkillAimVector(dt, pl.headUp.transform.position);
			float z6 = Mathf.Atan2(skillAimVector6.y, skillAimVector6.x) * 57.29578f;
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z6, GetSkillAimPosition(dt));
			break;
		}
		case 7:
		{
			Vector3 skillAimVector5 = GetSkillAimVector(dt, pl.transform.position);
			float z5 = Mathf.Atan2(skillAimVector5.y, skillAimVector5.x) * 57.29578f;
			dt.transform.SetParent(pl.transform);
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z5, GetSkillAimPosition(dt));
			break;
		}
		case 8:
		{
			Vector3 skillAimVector7 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z7 = Mathf.Atan2(skillAimVector7.y, skillAimVector7.x) * 57.29578f;
			dt.transform.SetParent(pl.yao.transform);
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z7, GetSkillAimPosition(dt));
			break;
		}
		case 9:
		{
			Vector3 skillAimVector4 = GetSkillAimVector(dt, pl.head.transform.position);
			float z4 = Mathf.Atan2(skillAimVector4.y, skillAimVector4.x) * 57.29578f;
			dt.transform.SetParent(pl.headUp.transform);
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointC.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z4, GetSkillAimPosition(dt));
			break;
		}
		case 10:
		{
			Vector3 skillAimVector = GetSkillAimVector(dt, base.transform.position);
			float num = Mathf.Atan2(skillAimVector.y, skillAimVector.x) * 57.29578f;
			Vector3 vector = ARCpointA.position - pl.yao.transform.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], ARCpointA.position, Quaternion.Euler(0f, 0f, z));
			float targetPointCastDistance = GetTargetPointCastDistance(dt.Distance, useGamepadSkillDistance: true);
			float x = base.transform.position.x + targetPointCastDistance * Mathf.Cos(num * 3.14f / 180f);
			float y = base.transform.position.y + targetPointCastDistance * Mathf.Sin(num * 3.14f / 180f);
			Limit = new Vector3(x, y, base.transform.position.z);
			ray = Physics2D.Raycast(base.transform.position, MousePos - base.transform.position, Vector2.Distance(base.transform.position, MousePos), LayerMask.GetMask("block"));
			if ((bool)ray.collider)
			{
				if (ray.collider.CompareTag("blockWALL"))
				{
					ATtrans = new Vector3(ray.point.x, ray.point.y, 0f);
					if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
					{
						if (Vector3.Distance(Limit, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
						{
							dt.transform.position = ATtrans;
						}
						else
						{
							dt.transform.position = Limit;
						}
					}
					else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
					{
						dt.transform.position = ATtrans;
					}
					else
					{
						dt.transform.position = MousePos;
					}
				}
				else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
				{
					dt.transform.position = Limit;
				}
				else
				{
					dt.transform.position = MousePos;
				}
			}
			else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
			{
				dt.transform.position = Limit;
			}
			else
			{
				dt.transform.position = MousePos;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, dt.transform, num, GetSkillAimPosition(dt));
			break;
		}
		}
		PoeItemMod.SpawnExtraProjectiles(this, dt);
		if ((bool)SingletonMonoScope<ACTbar>.Instance.ATprefab)
		{
			Vector3 actPrefabTargetPos = dt.TargetPos;
			this.wait(0.0001f, delegate
			{
				ACTprefabFS(dt, actPrefabTargetPos);
			});
		}
		ACT.TryReleaseLinkedSkills(GetCurrentActSkill());
		NotifyCurrentSkillUsed();
		ACT.TryRefreshSkillCooldown(GetCurrentActSkill());
		EndTargetPointCast(usedSnapshotTarget, oldMousePos);
		EndAnimationEventCast();
	}

	public void DEADattack()
	{
		SkillOBJ_DT_SP dt = CreatSP();
		Vector3 oldMousePos;
		bool usedSnapshotTarget = BeginTargetPointCast(dt, out oldMousePos);
		switch (dt.FStype)
		{
		case 0:
		{
			Vector3 skillAimVector3 = GetSkillAimVector(dt, DEADPointA.transform.position);
			float num2 = Mathf.Atan2(skillAimVector3.y, skillAimVector3.x) * 57.29578f;
			dt.transform.position = DEADPointA.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, num2);
				break;
			case 1:
				dt.dic = new Vector2(Mathf.Cos(num2 * 3.14f / 180f), Mathf.Sin(num2 * 3.14f / 180f));
				break;
			}
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointA.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(DEADPointA.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointA.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, DEADPointA, num2, GetSkillAimPosition(dt));
			break;
		}
		case 1:
		{
			Vector3 skillAimVector10 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z8 = Mathf.Atan2(skillAimVector10.y, skillAimVector10.x) * 57.29578f;
			dt.transform.position = pointA.position;
			switch (dt.RTtypeOBJ)
			{
			case 0:
				dt.transform.rotation = Quaternion.Euler(0f, 0f, z8);
				break;
			case 1:
				dt.dic = new Vector2(skillAimVector10.x, skillAimVector10.y);
				break;
			}
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointA.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.yao.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointA.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pointA, z8, GetSkillAimPosition(dt));
			break;
		}
		case 2:
		{
			Vector3 skillAimVector2 = GetSkillAimVector(dt, pl.head.transform.position);
			float z = Mathf.Atan2(skillAimVector2.y, skillAimVector2.x) * 57.29578f;
			dt.transform.position = DEADPointC.position;
			switch (dt.RTtypeFX)
			{
			case 0:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointC.position, Quaternion.Euler(0f, 0f, GetMouseAimAngle(pl.head.transform.position)));
				break;
			case 1:
				LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointC.position, Quaternion.identity);
				break;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, DEADPointC, z, GetSkillAimPosition(dt));
			break;
		}
		case 4:
		{
			Vector3 skillAimVector4 = GetSkillAimVector(dt, pl.transform.position);
			float z2 = Mathf.Atan2(skillAimVector4.y, skillAimVector4.x) * 57.29578f;
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z2, GetSkillAimPosition(dt));
			break;
		}
		case 5:
		{
			Vector3 skillAimVector9 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z7 = Mathf.Atan2(skillAimVector9.y, skillAimVector9.x) * 57.29578f;
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z7, GetSkillAimPosition(dt));
			break;
		}
		case 6:
		{
			Vector3 skillAimVector8 = GetSkillAimVector(dt, pl.headUp.transform.position);
			float z6 = Mathf.Atan2(skillAimVector8.y, skillAimVector8.x) * 57.29578f;
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z6, GetSkillAimPosition(dt));
			break;
		}
		case 7:
		{
			Vector3 skillAimVector6 = GetSkillAimVector(dt, pl.transform.position);
			float z4 = Mathf.Atan2(skillAimVector6.y, skillAimVector6.x) * 57.29578f;
			dt.transform.SetParent(pl.transform);
			dt.transform.position = pl.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.transform, z4, GetSkillAimPosition(dt));
			break;
		}
		case 8:
		{
			Vector3 skillAimVector5 = GetSkillAimVector(dt, pl.yao.transform.position);
			float z3 = Mathf.Atan2(skillAimVector5.y, skillAimVector5.x) * 57.29578f;
			dt.transform.SetParent(pl.yao.transform);
			dt.transform.position = pl.yao.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z3, GetSkillAimPosition(dt));
			break;
		}
		case 9:
		{
			Vector3 skillAimVector7 = GetSkillAimVector(dt, pl.head.transform.position);
			float z5 = Mathf.Atan2(skillAimVector7.y, skillAimVector7.x) * 57.29578f;
			dt.transform.SetParent(pl.headUp.transform);
			dt.transform.position = pl.headUp.transform.position;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointB.position, Quaternion.identity);
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, pl.yao.transform, z5, GetSkillAimPosition(dt));
			break;
		}
		case 10:
		{
			Vector3 skillAimVector = GetSkillAimVector(dt, base.transform.position);
			float num = Mathf.Atan2(skillAimVector.y, skillAimVector.x) * 57.29578f;
			LeanPool.Spawn(_gameDataManager.SKPB.SK_FX[CurrentSimpleObj], DEADPointB.position, Quaternion.identity);
			float targetPointCastDistance = GetTargetPointCastDistance(dt.Distance, useGamepadSkillDistance: true);
			float x = base.transform.position.x + targetPointCastDistance * Mathf.Cos(num * 3.14f / 180f);
			float y = base.transform.position.y + targetPointCastDistance * Mathf.Sin(num * 3.14f / 180f);
			Limit = new Vector3(x, y, base.transform.position.z);
			ray = Physics2D.Raycast(base.transform.position, MousePos - base.transform.position, Vector2.Distance(base.transform.position, MousePos), LayerMask.GetMask("block"));
			if ((bool)ray.collider)
			{
				if (ray.collider.CompareTag("blockWALL"))
				{
					ATtrans = new Vector3(ray.point.x, ray.point.y, 0f);
					if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
					{
						if (Vector3.Distance(Limit, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
						{
							dt.transform.position = ATtrans;
						}
						else
						{
							dt.transform.position = Limit;
						}
					}
					else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
					{
						dt.transform.position = ATtrans;
					}
					else
					{
						dt.transform.position = MousePos;
					}
				}
				else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
				{
					dt.transform.position = Limit;
				}
				else
				{
					dt.transform.position = MousePos;
				}
			}
			else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
			{
				dt.transform.position = Limit;
			}
			else
			{
				dt.transform.position = MousePos;
			}
			SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(dt.skillName, dt.transform, num, GetSkillAimPosition(dt));
			break;
		}
		}
		Debug.Log(1234);
		PoeItemMod.SpawnExtraProjectiles(this, dt);
		if ((bool)SingletonMonoScope<ACTbar>.Instance.ATprefab)
		{
			Vector3 actPrefabTargetPos = dt.TargetPos;
			this.wait(0.0001f, delegate
			{
				ACTprefabFS(dt, actPrefabTargetPos);
			});
		}
		ACT.TryReleaseLinkedSkills(GetCurrentActSkill());
		NotifyCurrentSkillUsed();
		ACT.TryRefreshSkillCooldown(GetCurrentActSkill());
		EndTargetPointCast(usedSnapshotTarget, oldMousePos);
		EndAnimationEventCast();
	}

	public void Summon()
	{
		BeginAnimationEventCast();
		CompanionRuntimeData data;
		SK_FSQ_comp sK_FSQ_comp = CreatCP(out data);
		if (!sK_FSQ_comp)
		{
			EndAnimationEventCast();
			return;
		}
		Vector3 mousePos = MousePos;
		MousePos = ResolveTargetPoint(data.Distance, useGamepadSkillDistance: true, allowAutoLock: true);
		Vector3 vector = MousePos - base.transform.position;
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		if (!sK_FSQ_comp)
		{
			EndAnimationEventCast();
			return;
		}
		switch (pl.PLType)
		{
		case 0:
			LeanPool.Spawn(_gameDataManager.SKPB.CP_FX[CurrentCompObj], MGCpointB.position, Quaternion.identity);
			break;
		case 1:
			LeanPool.Spawn(_gameDataManager.SKPB.CP_FX[CurrentCompObj], SQSpointA.position, Quaternion.identity);
			break;
		case 2:
			LeanPool.Spawn(_gameDataManager.SKPB.CP_FX[CurrentCompObj], ARCpointC.position, Quaternion.identity);
			break;
		case 3:
			LeanPool.Spawn(_gameDataManager.SKPB.CP_FX[CurrentCompObj], DEADPointB.position, Quaternion.identity);
			break;
		}
		float x = base.transform.position.x + data.Distance * Mathf.Cos(num * 3.14f / 180f);
		float y = base.transform.position.y + data.Distance * Mathf.Sin(num * 3.14f / 180f);
		Limit = new Vector3(x, y, base.transform.position.z);
		ray = Physics2D.Raycast(base.transform.position, MousePos - base.transform.position, Vector2.Distance(base.transform.position, MousePos), LayerMask.GetMask("block"));
		if ((bool)ray.collider)
		{
			if (ray.collider.CompareTag("blockWALL"))
			{
				ATtrans = new Vector3(ray.point.x, ray.point.y, 0f);
				if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
				{
					if (Vector3.Distance(Limit, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
					{
						sK_FSQ_comp.transform.position = ATtrans;
					}
					else
					{
						sK_FSQ_comp.transform.position = Limit;
					}
				}
				else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(ATtrans, base.transform.position))
				{
					sK_FSQ_comp.transform.position = ATtrans;
				}
				else
				{
					sK_FSQ_comp.transform.position = MousePos;
				}
			}
			else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
			{
				sK_FSQ_comp.transform.position = Limit;
			}
			else
			{
				sK_FSQ_comp.transform.position = MousePos;
			}
		}
		else if (Vector3.Distance(MousePos, base.transform.position) > Vector3.Distance(Limit, base.transform.position))
		{
			sK_FSQ_comp.transform.position = Limit;
		}
		else
		{
			sK_FSQ_comp.transform.position = MousePos;
		}
		SingletonMonoScope<ACTbar>.Instance.CreatACT_SK(data.skillName, sK_FSQ_comp.transform, num);
		sK_FSQ_comp.Init(data);
		NotifyCurrentSkillUsed();
		ACT.TryRefreshSkillCooldown(GetCurrentActSkill());
		MousePos = mousePos;
		EndAnimationEventCast();
	}

	public void ACTprefabFS(SkillOBJ_DT_SP dt, Vector3 targetPos)
	{
		if (dt.BSAT == pl.BSname)
		{
			SkillOBJ_DT_SP aTprefabSP = ACT.ATprefabSP;
			Transform transform = ACT.ATprefabSP.Type_F switch
			{
				0 => pointA.transform, 
				1 => pointA.transform, 
				2 => pointA.transform, 
				3 => pl.body.transform, 
				4 => pl.body.transform, 
				5 => pl.body.transform, 
				6 => pl.PLType switch
				{
					0 => pointA.transform, 
					1 => pointA.transform, 
					2 => ARCpointA.transform, 
					3 => pointA.transform, 
					_ => pointA.transform, 
				}, 
				_ => pointA.transform, 
			};
			Vector3 vector = targetPos - transform.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			SkillOBJ_DT_SP component = LeanPool.Spawn(ACT.ATprefab, transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<SkillOBJ_DT_SP>();
			component.indexType = 0;
			component.pl = pl;
			component.cp = null;
			component.ZY = aTprefabSP.ZY;
			component.Dot_Infect = false;
			component.Dot_Infect_Layer = 0;
			component.skillName = aTprefabSP.skillName;
			component.GlobalID = 100000;
			component.damageType = aTprefabSP.damageType;
			component.ThroughType = aTprefabSP.ThroughType;
			component.AttackType = true;
			component.Damage = aTprefabSP.Damage / 100f * pl.GiveDamage(aTprefabSP.damageType) * (1f + (float)Mathf.Max(0, dt.BSAT_DMG) / 100f);
			component.DamageA = 0f;
			component.DamageB = 0f;
			component.BJrate = aTprefabSP.BJrate + pl.BJrate_Last;
			component.BJDamage = aTprefabSP.BJDamage + pl.BJDamage_Last;
			component.JYrate = aTprefabSP.JYrate + pl.JYrate_Last;
			component.Through = aTprefabSP.Through + pl.ThroughRate;
			component.FlySpeed = aTprefabSP.FlySpeed + pl.FlySpeed;
			component.CutSpeedZone = aTprefabSP.CutSpeedZone;
			component.NoTime = 1;
			switch (aTprefabSP.Type_F)
			{
			case 0:
				component.Count_F = aTprefabSP.Count_F * dt.BSAT_Count * ACT.AT_Layer;
				component.CountMulti = aTprefabSP.CountMulti;
				break;
			case 1:
			case 2:
				component.Count_F = aTprefabSP.Count_F * dt.BSAT_Count * ACT.AT_Layer;
				component.CountMulti = aTprefabSP.CountMulti;
				break;
			case 3:
			case 4:
				component.Count_F = aTprefabSP.Count_F * dt.BSAT_Count * ACT.AT_Layer;
				component.CountMulti = aTprefabSP.CountMulti;
				break;
			case 5:
			case 6:
				component.Count_F = aTprefabSP.Count_F;
				component.CountMulti = aTprefabSP.CountMulti * dt.BSAT_Count * ACT.AT_Layer;
				break;
			}
			component.Count_ATtarget = aTprefabSP.Count_ATtarget;
			component.ATtar_DMG = 0;
			component.ATtarUP = aTprefabSP.ATtarUP;
			component.MS_Dead = aTprefabSP.MS_Dead;
			component.Type_F = aTprefabSP.Type_F;
			component.TypeDIC_F = aTprefabSP.TypeDIC_F;
			component.JG = aTprefabSP.JG;
			component.AngleA = aTprefabSP.AngleA * (float)dt.BSAT_Angle;
			component.AngleB = aTprefabSP.AngleB * (float)dt.BSAT_Angle;
			component.Range1 = aTprefabSP.Range1;
			component.Range2 = aTprefabSP.Range2;
			component.FStime1 = aTprefabSP.FStime1;
			component.FStime2 = aTprefabSP.FStime2;
			component.Speed1 = aTprefabSP.Speed1;
			component.Speed2 = aTprefabSP.Speed2;
			component.Speed3 = aTprefabSP.Speed3;
			component.Speed4 = aTprefabSP.Speed4;
			component.Follow_F = aTprefabSP.Follow_F;
			component.AllChuan_F = aTprefabSP.AllChuan_F;
			component.Slow_F = aTprefabSP.Slow_F;
			component.RDSpeed_F = aTprefabSP.RDSpeed_F;
			component.HasFX = aTprefabSP.HasFX;
			component.colEXP = aTprefabSP.colEXP;
			component.colEXP = aTprefabSP.colEXP;
			component.TimeEXP = aTprefabSP.TimeEXP;
			component.colEXP = aTprefabSP.colEXP;
			component.LastEXP = aTprefabSP.LastEXP;
			component.EXPpos = aTprefabSP.EXPpos;
			component.AngleEXP = aTprefabSP.AngleEXP;
			Debug.Log(1234);
		}
	}
}
