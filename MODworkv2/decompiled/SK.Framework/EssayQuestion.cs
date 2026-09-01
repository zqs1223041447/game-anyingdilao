using System;

namespace SK.Framework;

[Serializable]
public class EssayQuestion : QuestionBase
{
	public string Answer;

	public override bool IsCorrect(params object[] answers)
	{
		return answers[0] as string == Answer;
	}
}
