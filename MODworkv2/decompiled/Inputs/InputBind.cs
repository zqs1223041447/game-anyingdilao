using System;
using System.Collections.Generic;
using Data.AutoGen.DataClass.Settings;
using FinkFramework.Runtime.Singleton;

namespace Inputs;

public static class InputBind
{
	private readonly struct BindCacheKey : IEquatable<BindCacheKey>
	{
		private readonly InputDeviceType DeviceType;

		private readonly ControlAction Action;

		public BindCacheKey(InputDeviceType deviceType, ControlAction action)
		{
			DeviceType = deviceType;
			Action = action;
		}

		public bool Equals(BindCacheKey other)
		{
			if (DeviceType == other.DeviceType)
			{
				return Action == other.Action;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is BindCacheKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((int)DeviceType * 397) ^ (int)Action;
		}
	}

	private sealed class CachedBind
	{
		public string RawBindValue;

		public BindKey Bind;
	}

	private static readonly Dictionary<BindCacheKey, CachedBind> cache = new Dictionary<BindCacheKey, CachedBind>();

	private static readonly HashSet<ControlAction> suppressedUntilRelease = new HashSet<ControlAction>();

	public static void ClearCache()
	{
		cache.Clear();
	}

	public static void SuppressHeldUntilRelease(params ControlAction[] actions)
	{
		if (actions == null)
		{
			return;
		}
		foreach (ControlAction controlAction in actions)
		{
			BindKey bind = GetBind(controlAction);
			if (bind != null && bind.Get())
			{
				suppressedUntilRelease.Add(controlAction);
			}
			else
			{
				suppressedUntilRelease.Remove(controlAction);
			}
		}
	}

	public static bool GetDown(ControlAction action)
	{
		BindKey bind = GetBind(action);
		if (bind == null)
		{
			return false;
		}
		if (IsSuppressed(action, bind))
		{
			return false;
		}
		return bind.GetDown();
	}

	public static bool Get(ControlAction action)
	{
		BindKey bind = GetBind(action);
		if (bind == null)
		{
			return false;
		}
		if (IsSuppressed(action, bind))
		{
			return false;
		}
		return bind.Get();
	}

	public static bool GetUp(ControlAction action)
	{
		BindKey bind = GetBind(action);
		if (bind == null)
		{
			return false;
		}
		if (suppressedUntilRelease.Contains(action))
		{
			if (!bind.Get())
			{
				suppressedUntilRelease.Remove(action);
			}
			return false;
		}
		return bind.GetUp();
	}

	private static bool IsSuppressed(ControlAction action, BindKey key)
	{
		if (!suppressedUntilRelease.Contains(action))
		{
			return false;
		}
		if (key.Get())
		{
			return true;
		}
		suppressedUntilRelease.Remove(action);
		return false;
	}

	public static BindKey GetBind(ControlAction action)
	{
		InputDeviceType deviceType = (SingletonMonoGlobal<CurrentInputManager>.HasInstance ? SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType : InputDeviceType.PC);
		ControlsSettingData control = Singleton<SettingDataManager>.Instance.GetControl(deviceType);
		if (control == null)
		{
			return null;
		}
		string bind = control.GetBind(action);
		if (string.IsNullOrWhiteSpace(bind))
		{
			return null;
		}
		bind = KeyNameUtil.NormalizeKeyName(bind);
		BindCacheKey key = new BindCacheKey(deviceType, action);
		if (cache.TryGetValue(key, out var value) && string.Equals(value.RawBindValue, bind, StringComparison.Ordinal))
		{
			return value.Bind;
		}
		BindKey bindKey = BindKey.Parse(bind);
		cache[key] = new CachedBind
		{
			RawBindValue = bind,
			Bind = bindKey
		};
		return bindKey;
	}

	public static string GetBindKeyName(ControlAction action)
	{
		InputDeviceType deviceType = (SingletonMonoGlobal<CurrentInputManager>.HasInstance ? SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType : InputDeviceType.PC);
		ControlsSettingData control = Singleton<SettingDataManager>.Instance.GetControl(deviceType);
		if (control == null)
		{
			return null;
		}
		string bindKey = control.GetBindKey(action);
		if (string.IsNullOrWhiteSpace(bindKey))
		{
			return null;
		}
		return KeyNameUtil.NormalizeKeyName(bindKey);
	}
}
