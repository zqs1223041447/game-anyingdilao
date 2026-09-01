using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public class QuestionsHandler
{
	private List<QuestionBase> questions;

	private int currentSequence;

	public QuestionsHandler(QuestionsProfile profile)
	{
		Init(profile);
	}

	public QuestionsHandler(string resourcesPath)
	{
		QuestionsProfile questionsProfile = Resources.Load<QuestionsProfile>(resourcesPath);
		if (questionsProfile == null)
		{
			Log.Error("<color=red><b>[SKFramework.Question.Error]</b></color> 加载配置文件失败 {0}", resourcesPath);
		}
		else
		{
			Init(questionsProfile);
		}
	}

	private void Init(QuestionsProfile profile)
	{
		questions = new List<QuestionBase>();
		for (int i = 0; i < profile.Judges.Count; i++)
		{
			questions.Add(profile.Judges[i]);
		}
		for (int j = 0; j < profile.SingleChoices.Count; j++)
		{
			questions.Add(profile.SingleChoices[j]);
		}
		for (int k = 0; k < profile.MultipleChoices.Count; k++)
		{
			questions.Add(profile.MultipleChoices[k]);
		}
		for (int l = 0; l < profile.Completions.Count; l++)
		{
			questions.Add(profile.Completions[l]);
		}
		for (int m = 0; m < profile.Essays.Count; m++)
		{
			questions.Add(profile.Essays[m]);
		}
	}

	public QuestionBase Last()
	{
		currentSequence--;
		currentSequence = Mathf.Clamp(currentSequence, 1, questions.Count);
		return questions.Find((QuestionBase m) => m.Sequence == currentSequence);
	}

	public QuestionBase Next()
	{
		currentSequence++;
		currentSequence = Mathf.Clamp(currentSequence, 1, questions.Count);
		return questions.Find((QuestionBase m) => m.Sequence == currentSequence);
	}

	public QuestionBase Switch(int sequence)
	{
		currentSequence = sequence;
		currentSequence = Mathf.Clamp(currentSequence, 1, questions.Count);
		return questions.Find((QuestionBase m) => m.Sequence == currentSequence);
	}

	public T Get<T>(int sequence) where T : QuestionBase
	{
		QuestionBase questionBase = questions.Find((QuestionBase m) => m.Sequence == sequence);
		if (questionBase == null)
		{
			return null;
		}
		return questionBase as T;
	}
}
