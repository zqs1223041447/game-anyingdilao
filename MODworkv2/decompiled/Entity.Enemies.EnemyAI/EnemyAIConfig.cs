namespace Entity.Enemies.EnemyAI;

public static class EnemyAIConfig
{
	public const float DecisionInterval = 0.25f;

	public const float LeashFullDist = 2.5f;

	public const float LeashZeroDist = 13f;

	public const float ChaseFullDist = 2f;

	public const float ChaseZeroDist = 7f;

	public const float AggroGiveUpThreshold = 0.1f;

	public const float HitBoostAdd = 0.26f;

	public const float BoostDecayPerSec = 0.13f;

	public const float ReturnLockTime = 2.5f;

	public const float SoftReturnDistFromPoint = 8f;

	public const float AggroReengageThreshold = 0.2f;

	public const float SoftReturnReleaseDist = 4.5f;

	public const float MaxConsiderDist = 10f;

	public const float PlayerBasePriority = 0.04f;

	public const float PlayerBiasMax = 0.12f;

	public const float GapPreferStart = 0.8f;

	public const float GapPreferEnd = 1.6f;

	public const float SwitchMargin = 0.04f;

	public const float GiveUpDistanceFromPoint = 16f;

	public const float ReturnReleaseDistFromPoint = 6f;

	public const float MaxTargetDistanceHard = 15.5f;

	public const float MaxTargetDistanceHardTime = 3f;

	public const float ReturnOffsetRadius = 1f;

	public const float ArriveDist = 0.3f;

	public const float AttackEnterRangeBuffer = 0.2f;

	public const float AttackExitRangeBuffer = 0.35f;

	public const float IdleToPatrolDelayMin = 3f;

	public const float IdleToPatrolDelayMax = 6f;

	public const float PatrolDurationMin = 1.3f;

	public const float PatrolDurationMax = 2.5f;

	public const float PatrolRadius = 1.5f;

	public const float ScanRadius = 10f;
}
