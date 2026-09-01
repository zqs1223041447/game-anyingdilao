using UnityEngine;

namespace Entity.Enemies.EnemyAI;

public class EnemyAIRuntimeConfig
{
	public float ChaseZeroDist;

	public float LeashZeroDist;

	public float AggroGiveUpThreshold;

	public float ReturnLockTime;

	public float SoftReturnDistFromPoint;

	public float GiveUpDistanceFromPoint;

	public static EnemyAIRuntimeConfig CreateRandom()
	{
		return new EnemyAIRuntimeConfig
		{
			ChaseZeroDist = 7f * Random.Range(0.9f, 1.1f),
			LeashZeroDist = 13f * Random.Range(0.9f, 1.08f),
			AggroGiveUpThreshold = 0.1f * Random.Range(0.9f, 1.1f),
			ReturnLockTime = 2.5f * Random.Range(0.85f, 1.2f),
			SoftReturnDistFromPoint = 8f * Random.Range(0.85f, 1.2f),
			GiveUpDistanceFromPoint = 16f * Random.Range(0.85f, 1.2f)
		};
	}
}
