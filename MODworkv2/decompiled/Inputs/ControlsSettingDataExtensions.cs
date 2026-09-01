using System;
using System.Collections.Generic;
using System.Reflection;
using Data.AutoGen.DataClass.Settings;

namespace Inputs;

public static class ControlsSettingDataExtensions
{
	private static readonly Dictionary<ControlAction, FieldInfo> _fieldCache;

	static ControlsSettingDataExtensions()
	{
		_fieldCache = new Dictionary<ControlAction, FieldInfo>();
		FieldInfo[] fields = typeof(ControlsSettingData).GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!(fieldInfo.FieldType != typeof(string)) && Enum.TryParse<ControlAction>(fieldInfo.Name, out var result))
			{
				_fieldCache[result] = fieldInfo;
			}
		}
	}

	public static string GetBind(this ControlsSettingData data, ControlAction action)
	{
		if (data == null)
		{
			return null;
		}
		if (!_fieldCache.TryGetValue(action, out var value))
		{
			return null;
		}
		return value.GetValue(data) as string;
	}

	public static bool TryGetBind(this ControlsSettingData data, ControlAction action, out string bind)
	{
		bind = null;
		if (data == null)
		{
			return false;
		}
		if (!_fieldCache.TryGetValue(action, out var value))
		{
			return false;
		}
		bind = value.GetValue(data) as string;
		return true;
	}

	public static string GetBindKey(this ControlsSettingData data, ControlAction action)
	{
		return data.GetBind(action);
	}

	public static bool SetBindKey(this ControlsSettingData data, ControlAction action, string key)
	{
		if (data == null)
		{
			return false;
		}
		if (!_fieldCache.TryGetValue(action, out var value))
		{
			return false;
		}
		value.SetValue(data, key);
		return true;
	}

	public static bool SetBindData(this ControlsSettingData data, ControlAction action, string bindKey)
	{
		if (data == null)
		{
			return false;
		}
		if (!_fieldCache.TryGetValue(action, out var value))
		{
			return false;
		}
		value.SetValue(data, bindKey);
		return true;
	}

	public static IEnumerable<KeyValuePair<ControlAction, string>> GetAllBinds(this ControlsSettingData data)
	{
		if (data == null)
		{
			yield break;
		}
		foreach (KeyValuePair<ControlAction, FieldInfo> item in _fieldCache)
		{
			string value = item.Value.GetValue(data) as string;
			if (!string.IsNullOrEmpty(value))
			{
				yield return new KeyValuePair<ControlAction, string>(item.Key, value);
			}
		}
	}
}
