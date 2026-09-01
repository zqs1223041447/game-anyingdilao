using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public class AudioDatabaseController
{
	private readonly List<AudioDatabase> list;

	public AudioDatabaseController()
	{
		list = new List<AudioDatabase>();
	}

	public bool Load(string resourcesPath, out AudioDatabase database)
	{
		database = Resources.Load<AudioDatabase>(resourcesPath);
		if (database != null)
		{
			string databaseName = database.databaseName;
			if (list.FindIndex((AudioDatabase m) => m.databaseName == databaseName) == -1)
			{
				list.Add(database);
				Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 成功加载音频库[{0}]", database.name);
				return true;
			}
			Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 音频库[{0}]已存在 无需重复加载", database.name);
			return false;
		}
		Log.Error("<color=red><b>[SKFramework.Audio.Error]</b></color> 加载音频库失败 {0}", resourcesPath);
		return false;
	}

	public bool Unload(string databaseName)
	{
		AudioDatabase audioDatabase = list.Find((AudioDatabase m) => m.databaseName == databaseName);
		if (audioDatabase != null)
		{
			list.Remove(audioDatabase);
			Resources.UnloadAsset(audioDatabase);
			Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 成功卸载音频库[{0}]", databaseName);
			return true;
		}
		Log.Error("<color=red><b>[SKFramework.Audio.Error]</b></color> 卸载音频库失败[{0}]失败 不存在", databaseName);
		return false;
	}

	public AudioDatabase Get(string databaseName)
	{
		return list.Find((AudioDatabase m) => m.databaseName == databaseName);
	}
}
