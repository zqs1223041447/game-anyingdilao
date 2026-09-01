namespace SK.Framework;

public class ScoreItem
{
	public string Flag { get; private set; }

	public string Description { get; private set; }

	public float Value { get; private set; }

	public bool IsObtained { get; set; }

	public ScoreItem(string flag, string description, float value)
	{
		Flag = flag;
		Description = description;
		Value = value;
	}
}
