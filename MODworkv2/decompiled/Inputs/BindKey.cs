using System;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Inputs;

public abstract class BindKey
{
	public abstract bool GetDown();

	public abstract bool Get();

	public abstract bool GetUp();

	public static BindKey Parse(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}
		value = KeyNameUtil.NormalizeKeyName(value);
		if (GamepadInputManager.TryCreateBind(value, out var bindKey))
		{
			return bindKey;
		}
		if (string.Equals(value, "Mouse_WheelUp", StringComparison.OrdinalIgnoreCase))
		{
			return new MouseWheelBind(1);
		}
		if (string.Equals(value, "Mouse_WheelDown", StringComparison.OrdinalIgnoreCase))
		{
			return new MouseWheelBind(-1);
		}
		if (value.StartsWith("Mouse", StringComparison.OrdinalIgnoreCase))
		{
			if (int.TryParse(value.Substring("Mouse".Length), out var result))
			{
				return new MouseBind(result);
			}
			LogUtil.Warn("非法鼠标键: " + value);
			return null;
		}
		if (Enum.TryParse<KeyCode>(value, ignoreCase: true, out var result2))
		{
			return new KeyBind(result2);
		}
		LogUtil.Warn("不可解析的按键: " + value);
		return null;
	}
}
