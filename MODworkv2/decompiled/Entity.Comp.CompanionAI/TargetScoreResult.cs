namespace Entity.Comp.CompanionAI;

public struct TargetScoreResult
{
	public Enemy Target;

	public float Score;

	public static TargetScoreResult Invalid
	{
		get
		{
			TargetScoreResult result = default(TargetScoreResult);
			result.Target = null;
			result.Score = -999999f;
			return result;
		}
	}
}
