using UnityEngine;

namespace Entity.Comp.CompanionAI;

public sealed class CompAIRuntimeConfig
{
	public float SafeDistance;

	public float MinFollowDistance;

	public float EngagedFollowSlack;

	public float IdleFollowTighten;

	public float FollowReturnDistance;

	public float DefendPlayerRadius;

	public float DefendPlayerWeight;

	public static CompAIRuntimeConfig CreateRandom()
	{
		return new CompAIRuntimeConfig
		{
			SafeDistance = 5f * Random.Range(0.9f, 1.15f),
			MinFollowDistance = 0.5f * Random.Range(0.4f, 0.8f),
			EngagedFollowSlack = 1f * Random.Range(0.9f, 1.15f),
			IdleFollowTighten = 1.4f * Random.Range(0.9f, 1.1f),
			FollowReturnDistance = 1.8f * Random.Range(0.92f, 1.18f),
			DefendPlayerRadius = 5f * Random.Range(0.9f, 1.15f),
			DefendPlayerWeight = 2.5f * Random.Range(0.9f, 1.1f)
		};
	}
}
