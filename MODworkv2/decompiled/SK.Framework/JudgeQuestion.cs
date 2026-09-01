using System;

namespace SK.Framework;

[Serializable]
public class JudgeQuestion : QuestionBase
{
	public string Positive = "正确";

	public string Negative = "错误";

	public bool Answer;

	public override bool IsCorrect(params object[] answers)
	{
		return (bool)answers[0] == Answer;
	}
}
