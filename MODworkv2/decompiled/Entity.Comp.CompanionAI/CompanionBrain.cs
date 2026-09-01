using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Entity.Comp.CompanionAI;

public class CompanionBrain : MonoBehaviour
{
	private Companion companion;

	private Transform player;

	private CompAIRuntimeConfig runtimeConfig;

	private Enemy lastTarget;

	private float lastTargetSwitchTime;

	private float nextDecisionTime;

	private float idleUntilTime;

	private float patrolUntilTime;

	private bool hasPatrolPoint;

	private bool isForceFollowing;

	private void Awake()
	{
		companion = GetComponent<Companion>();
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			player = SingletonMonoScope<PlayerManager>.Instance.transform;
		}
		if (!companion)
		{
			LogUtil.Error("CompanionBrain missing Companion component");
		}
		runtimeConfig = CompAIRuntimeConfig.CreateRandom();
		nextDecisionTime = Time.time;
	}

	private void OnEnable()
	{
		if (SingletonMonoScope<CompanionManager>.HasInstance)
		{
			SingletonMonoScope<CompanionManager>.Instance.RequestRefreshNextFrame();
		}
		if ((bool)companion)
		{
			companion.OnExitedFollow += OnFollowExited;
		}
		float time = Time.time;
		runtimeConfig = CompAIRuntimeConfig.CreateRandom();
		idleUntilTime = time + Random.Range(2f, 5.5f);
		patrolUntilTime = 0f;
		hasPatrolPoint = false;
		isForceFollowing = false;
	}

	private void OnDisable()
	{
		if (SingletonMonoScope<CompanionManager>.HasInstance)
		{
			SingletonMonoScope<CompanionManager>.Instance.RequestRefreshNextFrame();
		}
		if ((bool)companion)
		{
			companion.OnExitedFollow -= OnFollowExited;
		}
	}

	private void OnFollowExited()
	{
		isForceFollowing = false;
	}

	private void Update()
	{
		if ((bool)companion)
		{
			CheckForceFollow();
			if (!(Time.time < nextDecisionTime))
			{
				nextDecisionTime = Time.time + Random.Range(0.1f, 0.3f);
				TickDecisionCore();
			}
		}
	}

	public void ForceImmediateDecision()
	{
		if ((bool)companion)
		{
			nextDecisionTime = Mathf.Min(nextDecisionTime, Time.time);
			TickDecisionCore();
		}
	}

	private void TickDecisionCore()
	{
		if ((bool)companion && !companion.IsDead)
		{
			TargetScoreResult result = EvaluateBestResult();
			DecideBehavior(result);
		}
	}

	private void CheckForceFollow()
	{
		if (!player || !companion)
		{
			return;
		}
		float num = Vector3.Distance(companion.transform.position, player.position);
		if (num > companion.MaxTeleportDistance)
		{
			companion.TeleportToPlayer();
			companion.RequestIdle();
			return;
		}
		float dynamicForceFollowDistance = GetDynamicForceFollowDistance();
		if (num > dynamicForceFollowDistance)
		{
			if (!isForceFollowing)
			{
				isForceFollowing = true;
				companion.BreakCombat();
				companion.RequestFollow();
			}
		}
		else if (isForceFollowing && num <= runtimeConfig.FollowReturnDistance && companion.CanAcceptNewIntent)
		{
			companion.RequestIdle();
		}
	}

	private float GetDynamicForceFollowDistance()
	{
		bool num = companion.CurrentIntent == Companion.CompanionIntentState.Combat && (bool)companion.MVTarget;
		float maxForceFollowDistance = companion.MaxForceFollowDistance;
		if (num)
		{
			return maxForceFollowDistance + runtimeConfig.EngagedFollowSlack;
		}
		return Mathf.Max(runtimeConfig.MinFollowDistance, maxForceFollowDistance - runtimeConfig.IdleFollowTighten);
	}

	private void DecideBehavior(TargetScoreResult result)
	{
		if (isForceFollowing || !companion.CanAcceptNewIntent)
		{
			return;
		}
		if ((bool)result.Target && result.Score >= 2f)
		{
			if (result.Target != lastTarget)
			{
				lastTarget = result.Target;
				lastTargetSwitchTime = Time.time;
			}
			DecideCombatBehavior(result);
		}
		else
		{
			DecideNonCombatBehavior(result);
		}
	}

	private void DecideCombatBehavior(TargetScoreResult result)
	{
		if (!result.Target)
		{
			if (companion.CurrentIntent == Companion.CompanionIntentState.Combat)
			{
				companion.RequestIdle();
			}
		}
		else if (companion.CurrentIntent != Companion.CompanionIntentState.Combat || companion.MVTarget != result.Target.transform)
		{
			companion.RequestCombatTarget(result.Target);
		}
	}

	private void DecideNonCombatBehavior(TargetScoreResult result)
	{
		if ((bool)result.Target)
		{
			return;
		}
		float time = Time.time;
		float num = (player ? Vector3.Distance(companion.transform.position, player.position) : 0f);
		if ((bool)player && num > runtimeConfig.FollowReturnDistance)
		{
			companion.RequestFollow();
			idleUntilTime = time + Random.Range(2f, 5.5f);
			return;
		}
		switch (companion.CurrentIntent)
		{
		case Companion.CompanionIntentState.Idle:
			if (time >= idleUntilTime)
			{
				companion.RequestPatrol();
				patrolUntilTime = time + Random.Range(1f, 3f);
				hasPatrolPoint = false;
			}
			break;
		case Companion.CompanionIntentState.Patrol:
			if (!hasPatrolPoint)
			{
				companion.GenerateNewPatrolPoint();
				hasPatrolPoint = true;
			}
			if (time >= patrolUntilTime || companion.HasReachedPatrolPoint())
			{
				companion.RequestIdle();
				idleUntilTime = time + Random.Range(3f, 9f);
			}
			break;
		default:
			if ((bool)player && num <= runtimeConfig.MinFollowDistance)
			{
				companion.RequestIdle();
			}
			idleUntilTime = time + Random.Range(3f, 9f);
			break;
		}
	}

	private TargetScoreResult EvaluateBestResult()
	{
		TargetScoreResult result = TargetScoreResult.Invalid;
		List<Enemy> attackableEnemies = companion.GetAttackableEnemies();
		if (attackableEnemies == null || attackableEnemies.Count == 0)
		{
			return result;
		}
		float time = Time.time;
		foreach (Enemy item in attackableEnemies)
		{
			if ((bool)item && item.IsAlive)
			{
				float num = CalculateTargetScore(item);
				if (item == lastTarget && num > 0f)
				{
					num += num * 0.3f;
				}
				if (num > result.Score)
				{
					TargetScoreResult targetScoreResult = default(TargetScoreResult);
					targetScoreResult.Target = item;
					targetScoreResult.Score = num;
					result = targetScoreResult;
				}
			}
		}
		if ((bool)lastTarget && result.Target != lastTarget && time - lastTargetSwitchTime < 0.5f)
		{
			TargetScoreResult result2 = default(TargetScoreResult);
			result2.Target = lastTarget;
			result2.Score = result.Score;
			return result2;
		}
		return result;
	}

	private float CalculateTargetScore(Enemy target)
	{
		return GetBaseTargetWeight(target) + GetDynamicIntentBonus(target) - GetDistancePenalty(target);
	}

	private float GetBaseTargetWeight(Enemy target)
	{
		if (!target)
		{
			return 0f;
		}
		if (!target.IS_Boss)
		{
			return 10f;
		}
		return 30f;
	}

	private float GetDynamicIntentBonus(Enemy target)
	{
		if (!target || !player)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = Vector3.Distance(target.transform.position, player.position);
		if (num2 <= runtimeConfig.DefendPlayerRadius)
		{
			float num3 = 1f - Mathf.Clamp01(num2 / runtimeConfig.DefendPlayerRadius);
			num += num3 * runtimeConfig.DefendPlayerWeight;
		}
		if (companion.CurrentIntent == Companion.CompanionIntentState.Combat)
		{
			num += 1.1f;
			if (companion.MVTarget == target.transform)
			{
				num += 1.4f;
			}
		}
		return num;
	}

	private float GetDistancePenalty(Enemy target)
	{
		float num = Vector3.Distance(companion.transform.position, target.transform.position);
		if (num <= runtimeConfig.SafeDistance)
		{
			return 0f;
		}
		return (num - runtimeConfig.SafeDistance) * 1.2f;
	}

	public void SetCombatMode(CompanionCombatMode mode)
	{
		ForceImmediateDecision();
	}

	public CompanionCombatMode GetCombatMode()
	{
		return CompanionCombatMode.Auto;
	}
}
