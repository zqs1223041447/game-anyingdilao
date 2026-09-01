using System.Collections.Generic;
using Entity.Enemies.EnemyState;
using UnityEngine;

namespace Entity.Enemies.EnemyAI;

public sealed class EnemyBrain
{
	private readonly Enemy em;

	private Vector3 spawnPos;

	private float hitBoost01;

	private float decisionTimer;

	private float returnLockTimer;

	private float hardTargetDistanceTimer;

	private float idleUntilTime;

	private float patrolUntilTime;

	private bool patrolTripActive;

	private Transform currentTarget;

	private bool currentIsPlayer;

	private float currentScore;

	private bool forceReturnToSpawn;

	private bool softReturnCommitted;

	private bool isChasingOrAttacking;

	public EnemyAIRuntimeConfig runtimeConfig;

	public readonly List<Companion> compCandidates = new List<Companion>(8);

	private readonly Collider2D[] compHits = new Collider2D[24];

	private float compScanTimer;

	private readonly int maskFootCp;

	private readonly int maskBlock;

	public float Aggro01 { get; private set; }

	public EnemyStateType DesiredState { get; private set; }

	private static float CompScanRadius => 10f;

	public EnemyBrain(Enemy em, Vector3 spawnPos)
	{
		this.em = em;
		this.spawnPos = spawnPos;
		maskFootCp = LayerMask.GetMask("FootCOLcp");
		maskBlock = LayerMask.GetMask("block");
		Reset(spawnPos);
	}

	public void Reset(Vector3 newSpawnPos)
	{
		spawnPos = newSpawnPos;
		hitBoost01 = 0f;
		decisionTimer = 0f;
		returnLockTimer = 0f;
		hardTargetDistanceTimer = 0f;
		compScanTimer = 0f;
		compCandidates.Clear();
		forceReturnToSpawn = false;
		DesiredState = EnemyStateType.Idle;
		softReturnCommitted = false;
		isChasingOrAttacking = false;
		runtimeConfig = EnemyAIRuntimeConfig.CreateRandom();
		Aggro01 = 0f;
		idleUntilTime = Time.time + Random.Range(3f, 6f);
		patrolUntilTime = 0f;
		patrolTripActive = false;
		ClearTarget();
	}

	public void OnHit()
	{
		hitBoost01 = Mathf.Clamp01(hitBoost01 + 0.26f);
	}

	public void Tick(float dt)
	{
		if (!em)
		{
			DesiredState = EnemyStateType.Idle;
			return;
		}
		if (!em.IsAlive)
		{
			DesiredState = EnemyStateType.Die;
			ClearTarget();
			return;
		}
		if (hitBoost01 > 0f)
		{
			hitBoost01 = Mathf.Max(0f, hitBoost01 - 0.13f * dt);
		}
		if (returnLockTimer > 0f)
		{
			returnLockTimer = Mathf.Max(0f, returnLockTimer - dt);
		}
		compScanTimer += dt;
		if (compScanTimer >= 0.25f)
		{
			compScanTimer = 0f;
			ScanCompanions();
		}
		decisionTimer += dt;
		if (!(decisionTimer < 0.25f))
		{
			decisionTimer = 0f;
			DoDecision();
		}
	}

	private void DoDecision()
	{
		Vector3 position = em.transform.position;
		float num = Vector3.Distance(position, spawnPos);
		if (em.IsDpsTarget)
		{
			num = 0f;
			forceReturnToSpawn = false;
			softReturnCommitted = false;
			returnLockTimer = 0f;
		}
		if (!IsCurrentTargetStillValid())
		{
			ClearTarget();
		}
		if (forceReturnToSpawn)
		{
			if (!(num <= 6f))
			{
				hardTargetDistanceTimer = 0f;
				ClearTarget();
				Aggro01 = 0f;
				CancelPatrolTrip();
				DesiredState = EnemyStateType.Return;
				return;
			}
			forceReturnToSpawn = false;
			returnLockTimer = runtimeConfig.ReturnLockTime;
		}
		if (softReturnCommitted)
		{
			if (!(num <= 4.5f))
			{
				hardTargetDistanceTimer = 0f;
				ClearTarget();
				Aggro01 = 0f;
				CancelPatrolTrip();
				DesiredState = EnemyStateType.Return;
				isChasingOrAttacking = false;
				return;
			}
			softReturnCommitted = false;
			returnLockTimer = Mathf.Max(returnLockTimer, runtimeConfig.ReturnLockTime * 0.5f);
		}
		if (num >= runtimeConfig.GiveUpDistanceFromPoint)
		{
			forceReturnToSpawn = true;
			hardTargetDistanceTimer = 0f;
			ClearTarget();
			Aggro01 = 0f;
			CancelPatrolTrip();
			DesiredState = EnemyStateType.Return;
			return;
		}
		bool alive;
		Transform transform = TryGetPlayerTransform(out alive);
		bool hasComp;
		Transform transform2 = TryFindBestCompanion(out hasComp);
		if (!(((bool)transform && alive) || hasComp))
		{
			ClearTarget();
			WhenTargetIsNull(num);
			return;
		}
		float chaseReferenceDistance = GetChaseReferenceDistance(position, transform, alive, transform2, hasComp);
		float num2 = InverseLerp01(2.5f, runtimeConfig.LeashZeroDist, num);
		float num3 = InverseLerp01(2f, runtimeConfig.ChaseZeroDist, chaseReferenceDistance);
		float num4 = num2 * num3;
		Aggro01 = Mathf.Clamp01(num4 + hitBoost01);
		if (returnLockTimer > 0f)
		{
			ClearTarget();
			CancelPatrolTrip();
			DesiredState = ((num > runtimeConfig.SoftReturnDistFromPoint) ? EnemyStateType.Return : EnemyStateType.Idle);
			return;
		}
		float num5 = float.NegativeInfinity;
		float num6 = float.NegativeInfinity;
		float num7 = float.PositiveInfinity;
		float num8 = float.PositiveInfinity;
		if ((bool)transform && alive)
		{
			num7 = Vector3.Distance(position, transform.position);
			num5 = CalcDistScore(num7) + 0.04f;
		}
		if (hasComp && (bool)transform2)
		{
			num8 = Vector3.Distance(position, transform2.position);
			num6 = CalcDistScore(num8);
		}
		if (num5 > float.NegativeInfinity && num6 > float.NegativeInfinity)
		{
			float x = Mathf.Abs(num7 - num8);
			float num9 = 1f - InverseLerp01(0.8f, 1.6f, x);
			float num10 = 0.12f * Aggro01 * num9;
			num5 += num10;
		}
		float aggroGiveUpThreshold = runtimeConfig.AggroGiveUpThreshold;
		if (isChasingOrAttacking)
		{
			if (Aggro01 < aggroGiveUpThreshold)
			{
				if ((bool)currentTarget)
				{
					returnLockTimer = runtimeConfig.ReturnLockTime;
				}
				hardTargetDistanceTimer = 0f;
				ClearTarget();
				isChasingOrAttacking = false;
				if (num > runtimeConfig.SoftReturnDistFromPoint)
				{
					softReturnCommitted = true;
					CancelPatrolTrip();
					DesiredState = EnemyStateType.Return;
				}
				else
				{
					BeginIdlePause();
					DesiredState = EnemyStateType.Idle;
				}
				return;
			}
		}
		else if (Aggro01 < 0.2f)
		{
			ClearTarget();
			WhenTargetIsNull(num);
			return;
		}
		Transform transform3;
		bool isPlayer;
		float num11;
		if (num5 >= num6)
		{
			transform3 = transform;
			isPlayer = true;
			num11 = num5;
		}
		else
		{
			transform3 = transform2;
			isPlayer = false;
			num11 = num6;
		}
		if (!currentTarget)
		{
			SetTarget(transform3, isPlayer, num11);
		}
		else
		{
			float num12 = Vector3.Distance(position, currentTarget.position);
			float num13 = CalcDistScore(num12);
			if (currentIsPlayer)
			{
				num13 += 0.04f;
				if ((bool)transform2)
				{
					float x2 = Mathf.Abs(num12 - num8);
					float num14 = 1f - InverseLerp01(0.8f, 1.6f, x2);
					float num15 = 0.12f * Aggro01 * num14;
					num13 += num15;
				}
			}
			currentScore = num13;
			if ((bool)transform3 && transform3 != currentTarget && num11 > currentScore + 0.04f)
			{
				SetTarget(transform3, isPlayer, num11);
			}
		}
		if ((bool)currentTarget)
		{
			float num16 = Vector3.Distance(position, currentTarget.position);
			if (!em.IsDpsTarget && num16 > 15.5f)
			{
				hardTargetDistanceTimer += 0.25f;
				if (hardTargetDistanceTimer >= 3f)
				{
					returnLockTimer = runtimeConfig.ReturnLockTime;
					hardTargetDistanceTimer = 0f;
					ClearTarget();
					CancelPatrolTrip();
					DesiredState = EnemyStateType.Return;
					return;
				}
			}
			else
			{
				hardTargetDistanceTimer = 0f;
			}
		}
		else
		{
			hardTargetDistanceTimer = 0f;
		}
		if ((bool)currentTarget)
		{
			ApplyTargetToEnemy(currentTarget, currentIsPlayer);
			bool flag = em.canAttack && em.CanSeeTarget;
			DesiredState = ((!flag) ? EnemyStateType.Walk : EnemyStateType.Idle);
			isChasingOrAttacking = true;
			CancelPatrolTrip();
		}
		else
		{
			ClearTarget();
			isChasingOrAttacking = false;
			WhenTargetIsNull(num);
		}
	}

	private void WhenTargetIsNull(float distToSpawn)
	{
		if (distToSpawn > runtimeConfig.SoftReturnDistFromPoint)
		{
			CancelPatrolTrip();
			DesiredState = EnemyStateType.Return;
		}
		else if (isChasingOrAttacking)
		{
			isChasingOrAttacking = false;
			BeginIdlePause();
			DesiredState = EnemyStateType.Idle;
		}
		else if (patrolTripActive)
		{
			bool num = Time.time >= patrolUntilTime;
			bool flag = !em.BrainMovePoint || Vector3.Distance(em.transform.position, em.BrainMovePoint.position) <= 0.3f;
			if (!num && !flag)
			{
				DesiredState = EnemyStateType.Patrol;
				return;
			}
			BeginIdlePause();
			DesiredState = EnemyStateType.Idle;
		}
		else if (Time.time < idleUntilTime)
		{
			DesiredState = EnemyStateType.Idle;
		}
		else
		{
			BeginPatrolTrip();
			DesiredState = EnemyStateType.Patrol;
		}
	}

	private void BeginIdlePause()
	{
		patrolTripActive = false;
		patrolUntilTime = 0f;
		idleUntilTime = Time.time + Random.Range(3f, 6f);
	}

	private void BeginPatrolTrip()
	{
		patrolTripActive = true;
		patrolUntilTime = Time.time + Random.Range(1.3f, 2.5f);
	}

	private void CancelPatrolTrip()
	{
		patrolTripActive = false;
		patrolUntilTime = 0f;
	}

	private void ClearTarget()
	{
		currentTarget = null;
		currentIsPlayer = false;
		currentScore = 0f;
		ApplyTargetToEnemy(null, isPlayer: false);
	}

	private void SetTarget(Transform tf, bool isPlayer, float score)
	{
		currentTarget = tf;
		currentIsPlayer = isPlayer;
		currentScore = score;
		hardTargetDistanceTimer = 0f;
	}

	private void ApplyTargetToEnemy(Transform target, bool isPlayer)
	{
		if (!IsTargetTransformUsable(target))
		{
			em.SetBrainTarget(null, null, isPlayer: false);
			em.CanSeeTarget = false;
			return;
		}
		if (!target)
		{
			em.SetBrainTarget(null, null, isPlayer: false);
			em.CanSeeTarget = false;
			return;
		}
		Transform transform = null;
		Companion component;
		if (isPlayer)
		{
			if ((bool)em.playerManager && (bool)em.playerManager.yao)
			{
				transform = em.playerManager.yao.transform;
			}
		}
		else if (target.TryGetComponent<Companion>(out component) && (bool)component.yao)
		{
			transform = component.yao.transform;
		}
		if (!transform)
		{
			transform = target;
		}
		em.SetBrainTarget(target, transform, isPlayer);
		Vector2 vector = target.position - em.transform.position;
		float magnitude = vector.magnitude;
		if (magnitude <= 0.001f)
		{
			em.CanSeeTarget = true;
			return;
		}
		RaycastHit2D raycastHit2D = Physics2D.Raycast(em.transform.position, vector.normalized, magnitude, maskBlock);
		em.CanSeeTarget = !raycastHit2D.collider;
	}

	private Transform TryGetPlayerTransform(out bool alive)
	{
		alive = false;
		if (!em.playerManager)
		{
			return null;
		}
		alive = em.playerManager.IsAlive && em.playerManager.gameObject.activeInHierarchy && em.playerManager.transform.gameObject.activeInHierarchy;
		if (!alive)
		{
			return null;
		}
		return em.playerManager.transform;
	}

	private Transform TryFindBestCompanion(out bool hasComp)
	{
		hasComp = false;
		if (compCandidates == null || compCandidates.Count == 0)
		{
			return null;
		}
		Companion companion = compCandidates[0];
		if (!IsCompanionTargetValid(companion))
		{
			return null;
		}
		hasComp = true;
		return companion.transform;
	}

	private float GetChaseReferenceDistance(Vector3 selfPos, Transform playerTf, bool playerAlive, Transform compTf, bool hasComp)
	{
		float num = float.PositiveInfinity;
		if ((bool)playerTf && playerAlive)
		{
			num = Mathf.Min(num, Vector3.Distance(selfPos, playerTf.position));
		}
		if (hasComp && (bool)compTf)
		{
			num = Mathf.Min(num, Vector3.Distance(selfPos, compTf.position));
		}
		if (float.IsPositiveInfinity(num))
		{
			num = runtimeConfig.ChaseZeroDist;
		}
		return num;
	}

	private void ScanCompanions()
	{
		compCandidates.Clear();
		int num = Physics2D.OverlapCircleNonAlloc(em.transform.position, CompScanRadius, compHits, maskFootCp);
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = compHits[i];
			compHits[i] = null;
			if (!collider2D)
			{
				continue;
			}
			FootCOL component = collider2D.GetComponent<FootCOL>();
			if ((bool)component && (bool)component.peo && component.peo.CharacterType == 1)
			{
				Companion cp = component.peo.cp;
				if (IsCompanionTargetValid(cp) && !compCandidates.Contains(cp))
				{
					compCandidates.Add(cp);
				}
			}
		}
		if (compCandidates.Count <= 1)
		{
			return;
		}
		Vector3 selfPos = em.transform.position;
		compCandidates.Sort(delegate(Companion a, Companion b)
		{
			if (!a && !b)
			{
				return 0;
			}
			if (!a)
			{
				return 1;
			}
			if (!b)
			{
				return -1;
			}
			float num2 = Vector2.Distance(selfPos, a.transform.position);
			float value = Vector2.Distance(selfPos, b.transform.position);
			return num2.CompareTo(value);
		});
	}

	private static float CalcDistScore(float dist)
	{
		float num = Mathf.InverseLerp(0f, 10f, dist);
		return Mathf.Clamp01(1f - num);
	}

	private static float InverseLerp01(float fullDist, float zeroDist, float x)
	{
		if (x <= fullDist)
		{
			return 1f;
		}
		if (x >= zeroDist)
		{
			return 0f;
		}
		float num = (x - fullDist) / (zeroDist - fullDist);
		return Mathf.Clamp01(1f - num);
	}

	private bool IsCurrentTargetStillValid()
	{
		if (!currentTarget)
		{
			return false;
		}
		if (!IsTargetTransformUsable(currentTarget))
		{
			return false;
		}
		if (currentIsPlayer)
		{
			return IsPlayerTargetValid();
		}
		if (currentTarget.TryGetComponent<Companion>(out var component))
		{
			return IsCompanionTargetValid(component);
		}
		return false;
	}

	private bool IsPlayerTargetValid()
	{
		if ((bool)em.playerManager && em.playerManager.IsAlive && em.playerManager.gameObject.activeInHierarchy)
		{
			return em.playerManager.transform.gameObject.activeInHierarchy;
		}
		return false;
	}

	private static bool IsCompanionTargetValid(Companion companion)
	{
		if ((bool)companion && companion.IsAlive && companion.gameObject.activeInHierarchy)
		{
			return companion.transform.gameObject.activeInHierarchy;
		}
		return false;
	}

	private static bool IsTargetTransformUsable(Transform target)
	{
		if ((bool)target && target.gameObject.activeInHierarchy)
		{
			return target.gameObject.activeSelf;
		}
		return false;
	}
}
