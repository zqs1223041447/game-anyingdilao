using System.Collections.Generic;

namespace SK.Framework;

public class ScoreGroup
{
	public readonly List<ScoreItem> Scores;

	public string Description { get; private set; }

	public ValueMode ValueMode { get; private set; }

	public ScoreGroup(string description, ValueMode valueMode, params ScoreItem[] scores)
	{
		Description = description;
		ValueMode = valueMode;
		Scores = new List<ScoreItem>(scores);
	}

	public bool Obtain(string flag)
	{
		ScoreItem scoreItem = Scores.Find((ScoreItem m) => m.Flag == flag);
		if (scoreItem != null)
		{
			switch (ValueMode)
			{
			case ValueMode.Additive:
				scoreItem.IsObtained = true;
				break;
			case ValueMode.MutuallyExclusive:
			{
				for (int i = 0; i < Scores.Count; i++)
				{
					Scores[i].IsObtained = Scores[i] == scoreItem;
				}
				break;
			}
			}
			Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 获得分数组合[{0}]中标识为[{1}]的分数项的分值", Description, flag);
			return true;
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 分数组合[{0}]不存在标识为[{1}]的分数项", Description, flag);
		return false;
	}

	public bool Cancle(string flag)
	{
		ScoreItem scoreItem = Scores.Find((ScoreItem m) => m.Flag == flag);
		if (scoreItem != null)
		{
			if (scoreItem.IsObtained)
			{
				scoreItem.IsObtained = false;
				Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 取消分数组合[{0}]中标识为[{1}]的分数项的分值", Description, flag);
				return true;
			}
			return false;
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 分数组合[{0}]不存在标识为[{1}]的分数项", Description, flag);
		return false;
	}

	public bool Delete(string flag)
	{
		ScoreItem scoreItem = Scores.Find((ScoreItem m) => m.Flag == flag);
		if (scoreItem != null)
		{
			Scores.Remove(scoreItem);
			Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 分数组合[{0}]删除标识为[{1}]的分数项", Description, flag);
			return true;
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 分数组合[{0}]不存在标识为[{1}]的分数项", Description, flag);
		return false;
	}

	public float GetSum()
	{
		float num = 0f;
		for (int i = 0; i < Scores.Count; i++)
		{
			ScoreItem scoreItem = Scores[i];
			num += (scoreItem.IsObtained ? scoreItem.Value : 0f);
		}
		return num;
	}
}
