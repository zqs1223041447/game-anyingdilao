using System;
using System.Collections.Generic;

namespace SK.Framework;

[Serializable]
public class MultipleChoiceQuestion : QuestionBase
{
	public ChoiceType choiceType;

	public List<QuestionChoice> Choices = new List<QuestionChoice>(0);

	public List<int> Answers = new List<int>(0);

	public override bool IsCorrect(params object[] answers)
	{
		if (Answers.Count != answers.Length)
		{
			return false;
		}
		for (int i = 0; i < answers.Length; i++)
		{
			int num = (int)answers[i];
			if (Answers[i] != num)
			{
				return false;
			}
		}
		return true;
	}
}
