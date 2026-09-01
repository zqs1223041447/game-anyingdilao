using System;
using System.Collections.Generic;

namespace SK.Framework;

[Serializable]
public class SingleChoiceQuestion : QuestionBase
{
	public ChoiceType choiceType;

	public List<QuestionChoice> Choices = new List<QuestionChoice>(0);

	public int Answer;

	public override bool IsCorrect(params object[] answers)
	{
		return (int)answers[0] == Answer;
	}
}
