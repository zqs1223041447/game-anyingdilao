namespace Entity.Comp.CompanionAI;

public static class CompAIConfig
{
	public const float safeDistance = 5f;

	public const float distancePenaltyWeight = 1.2f;

	public const float normalEnemyBaseWeight = 10f;

	public const float bossBaseWeight = 30f;

	public const float combatEnterThreshold = 2f;

	public const float lastTargetScoreFactor = 0.3f;

	public const float minTargetHoldTime = 0.5f;

	public const float decisionIntervalA = 0.1f;

	public const float decisionIntervalB = 0.3f;

	public const float minFollowDistance = 0.5f;

	public const float engagedFollowSlack = 1f;

	public const float idleFollowTighten = 1.4f;

	public const float followReturnDistance = 1.8f;

	public const float followPointMinRadius = 0.3f;

	public const float followPointMaxRadius = 1f;

	public const float followPointReachedDistance = 0.3f;

	public const float defendPlayerRadius = 5f;

	public const float defendPlayerWeight = 2.5f;

	public const float engagedPlayerWeight = 1.1f;

	public const float currentCombatTargetBonus = 1.4f;

	public const float idleToPatrolDelayMin = 2f;

	public const float idleToPatrolDelayMax = 5.5f;

	public const float patrolDurationMin = 1f;

	public const float patrolDurationMax = 3f;

	public const float postPatrolIdleDelayMin = 3f;

	public const float postPatrolIdleDelayMax = 9f;
}
