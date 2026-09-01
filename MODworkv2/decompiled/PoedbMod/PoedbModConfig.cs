using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace PoedbMod;

public static class PoedbModConfig
{
	[Serializable]
	private class ConfigFile
	{
		public string dataRoot;

		public bool verboseLog;
	}

	public static string DataRoot = "poedb";

	public static bool VerboseLog = false;

	public const string ModVersion = "1.0.0";

	public static void TryLoadFromFile()
	{
		try
		{
			string path = Path.Combine(Application.streamingAssetsPath, DataRoot, "mod_config.json");
			if (!File.Exists(path))
			{
				return;
			}
			ConfigFile configFile = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(path));
			if (configFile != null)
			{
				if (!string.IsNullOrEmpty(configFile.dataRoot))
				{
					DataRoot = configFile.dataRoot;
				}
				VerboseLog = configFile.verboseLog;
				if (VerboseLog)
				{
					Debug.Log("[PoedbModConfig] 已加载配置: DataRoot=" + DataRoot);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbModConfig] TryLoadFromFile 异常（已忽略）: " + ex.Message);
		}
	}
}
