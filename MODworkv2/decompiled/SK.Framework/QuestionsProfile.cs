using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

[CreateAssetMenu(fileName = "New Questions Profile", menuName = "Question Profile")]
public sealed class QuestionsProfile : ScriptableObject
{
	public List<JudgeQuestion> Judges = new List<JudgeQuestion>(0);

	public List<SingleChoiceQuestion> SingleChoices = new List<SingleChoiceQuestion>(0);

	public List<MultipleChoiceQuestion> MultipleChoices = new List<MultipleChoiceQuestion>(0);

	public List<CompletionQuestion> Completions = new List<CompletionQuestion>(0);

	public List<EssayQuestion> Essays = new List<EssayQuestion>();

	public QuestionBase Get(int sequence)
	{
		JudgeQuestion judgeQuestion = Judges.Find((JudgeQuestion m) => m.Sequence == sequence);
		if (judgeQuestion != null)
		{
			return judgeQuestion;
		}
		SingleChoiceQuestion singleChoiceQuestion = SingleChoices.Find((SingleChoiceQuestion m) => m.Sequence == sequence);
		if (singleChoiceQuestion != null)
		{
			return singleChoiceQuestion;
		}
		MultipleChoiceQuestion multipleChoiceQuestion = MultipleChoices.Find((MultipleChoiceQuestion m) => m.Sequence == sequence);
		if (multipleChoiceQuestion != null)
		{
			return multipleChoiceQuestion;
		}
		CompletionQuestion completionQuestion = Completions.Find((CompletionQuestion m) => m.Sequence == sequence);
		if (completionQuestion != null)
		{
			return completionQuestion;
		}
		EssayQuestion essayQuestion = Essays.Find((EssayQuestion m) => m.Sequence == sequence);
		if (essayQuestion != null)
		{
			return essayQuestion;
		}
		return null;
	}
}
