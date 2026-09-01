namespace SK.Framework;

public abstract class QuestionBase
{
	public int Sequence;

	public string Question;

	public QuestionType Type;

	public string Analysis;

	public abstract bool IsCorrect(params object[] answers);
}
