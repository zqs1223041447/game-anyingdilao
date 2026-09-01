using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace PoedbMod;

public static class DataLoader
{
	[Serializable]
	private class CategoryFile<T>
	{
		public string schema_version;

		public string category;

		public string source;

		public string fetched_at;

		public List<T> items;
	}

	public const string DefaultDataRoot = "poedb";

	private static readonly Dictionary<string, List<object>> _cache = new Dictionary<string, List<object>>(StringComparer.Ordinal);

	private static bool _initialized;

	public static void Init()
	{
		try
		{
			_cache.Clear();
			_initialized = true;
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.DataLoader] Init 异常: " + ex);
		}
	}

	public static List<T> LoadCategory<T>(string category) where T : class
	{
		try
		{
			if (!_initialized)
			{
				Init();
			}
			string key = category + "|" + typeof(T).Name;
			if (_cache.TryGetValue(key, out var value))
			{
				List<T> list = new List<T>();
				foreach (object item in value)
				{
					list.Add((T)item);
				}
				return list;
			}
			string dataPath = GetDataPath(category);
			if (string.IsNullOrEmpty(dataPath) || !File.Exists(dataPath))
			{
				Debug.LogWarning("[PoedbMod.DataLoader] 数据文件不存在: " + dataPath);
				return new List<T>();
			}
			string text = File.ReadAllText(dataPath, Encoding.UTF8);
			if (!string.IsNullOrEmpty(text) && text[0] == '\ufeff')
			{
				text = text.Substring(1);
			}
			List<T> list2 = JsonConvert.DeserializeObject<CategoryFile<T>>(text)?.items ?? new List<T>();
			List<object> list3 = new List<object>();
			foreach (T item2 in list2)
			{
				list3.Add(item2);
			}
			_cache[key] = list3;
			return list2;
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.DataLoader] LoadCategory 异常: " + ex);
			return new List<T>();
		}
	}

	public static string GetDataPath(string category)
	{
		try
		{
			string text = PoedbModConfig.DataRoot;
			if (string.IsNullOrEmpty(text))
			{
				text = "poedb";
			}
			return Path.Combine(Application.streamingAssetsPath, text, category + ".json");
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.DataLoader] GetDataPath 异常: " + ex);
			return null;
		}
	}
}
