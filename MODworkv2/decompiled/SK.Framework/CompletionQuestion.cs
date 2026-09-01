using System;
using System.Collections.Generic;

namespace SK.Framework;

[Serializable]
public class CompletionQuestion : QuestionBase
{
	public List<string> Answers = new List<string>(0);

	public override bool IsCorrect(params object[] answers)
	{
		if (answers.Length != Answers.Count)
		{
			return false;
		}
		for (int i = 0; i < answers.Length; i++)
		{
			if (answers[i] as string != Answers[i])
			{
				return false;
			}
		}
		return true;
	}
}
