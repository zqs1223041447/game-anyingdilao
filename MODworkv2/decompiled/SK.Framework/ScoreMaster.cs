using System;
using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public class ScoreMaster : MonoBehaviour
{
	private static ScoreMaster instance;

	[SerializeField]
	private ScoreProfile profile;

	private readonly Dictionary<string, ScoreGroup> groups = new Dictionary<string, ScoreGroup>();

	private const string ungrouped = "未分组";

	public static ScoreMaster Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<ScoreMaster>();
			}
			if (instance == null)
			{
				instance = new GameObject("[SKFramework.Score]").AddComponent<ScoreMaster>();
				instance.profile = Resources.Load<ScoreProfile>("Score Profile");
				if (instance.profile == null)
				{
					Log.Error((object)"<color=red><b>[SKFramework.Score.Error]</b></color> 加载配置文件失败");
				}
			}
			return instance;
		}
	}

	public string Create(int id)
	{
		ScoreInfo scoreInfo = Array.Find(profile.scores, (ScoreInfo m) => m.id == id);
		if (scoreInfo != null)
		{
			string text = Guid.NewGuid().ToString();
			ScoreItem scoreItem = new ScoreItem(text, scoreInfo.description, scoreInfo.value);
			Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 创建分数ID为[{0}]的分数项 [{1}]", id, scoreInfo.description);
			if (!groups.ContainsKey("未分组"))
			{
				groups.Add("未分组", new ScoreGroup("未分组", ValueMode.Additive, scoreItem));
			}
			else
			{
				groups["未分组"].Scores.Add(scoreItem);
			}
			return text;
		}
		Log.Error("<color=red><b>[SKFramework.Score.Error]</b></color> 配置表中不存在ID为[{0}]的分数信息", id);
		return null;
	}

	public string[] CreateGroup(string groupDescription, ValueMode valueMode, params int[] idArray)
	{
		ScoreItem[] array = new ScoreItem[idArray.Length];
		string[] array2 = new string[idArray.Length];
		int i;
		for (i = 0; i < idArray.Length; i++)
		{
			ScoreInfo scoreInfo = Array.Find(profile.scores, (ScoreInfo m) => m.id == idArray[i]);
			if (scoreInfo != null)
			{
				string text = Guid.NewGuid().ToString();
				array2[i] = text;
				array[i] = new ScoreItem(text, scoreInfo.description, scoreInfo.value);
				Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 创建分数ID为[{0}]的分数项 [{1}]", idArray[i], scoreInfo.description);
			}
			else
			{
				Log.Error("<color=red><b>[SKFramework.Score.Error]</b></color> 配置表中不存在ID为[{0}]的分数信息", idArray[i]);
			}
		}
		ScoreGroup value = new ScoreGroup(groupDescription, valueMode, array);
		groups.Add(groupDescription, value);
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 创建分数组合[{0}] 计分模式[{1}]", groupDescription, valueMode);
		return array2;
	}

	public bool Delete(string flag)
	{
		if (groups.ContainsKey("未分组"))
		{
			return groups["未分组"].Delete(flag);
		}
		return false;
	}

	public bool DeleteGroup(string groupDescription)
	{
		if (groups.ContainsKey(groupDescription))
		{
			groups.Remove(groupDescription);
			Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 删除分数组合[{0}]", groupDescription);
			return true;
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 不存在分数组合[{0}]", groupDescription);
		return false;
	}

	public bool DeleteGroupItem(string groupDescription, string flag)
	{
		if (groups.TryGetValue(groupDescription, out var value))
		{
			return value.Delete(flag);
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 不存在分数组合[{0}]", groupDescription);
		return false;
	}

	public bool Obtain(string flag)
	{
		if (groups.ContainsKey("未分组"))
		{
			return groups["未分组"].Obtain(flag);
		}
		return false;
	}

	public bool Obtain(string groupDescription, string flag)
	{
		if (groups.TryGetValue(groupDescription, out var value))
		{
			return value.Obtain(flag);
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 不存在分数组合[{0}]", groupDescription);
		return false;
	}

	public bool Cancle(string flag)
	{
		if (groups.ContainsKey("未分组"))
		{
			return groups["未分组"].Cancle(flag);
		}
		return false;
	}

	public bool Cancle(string groupDescription, string flag)
	{
		if (groups.TryGetValue(groupDescription, out var value))
		{
			return value.Cancle(flag);
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 不存在分数组合[{0}]", groupDescription);
		return false;
	}

	public float GetGroupSum(string groupDescription)
	{
		if (groups.TryGetValue(groupDescription, out var value))
		{
			return value.GetSum();
		}
		Log.Info("<color=cyan><b>[SKFramework.Score.Info]</b></color> 不存在分数组合[{0}]", groupDescription);
		return 0f;
	}

	public float GetSum()
	{
		float num = 0f;
		foreach (KeyValuePair<string, ScoreGroup> group in groups)
		{
			num += group.Value.GetSum();
		}
		return num;
	}
}
