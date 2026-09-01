using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace UI.DebugUI;

public static class UILog
{
	public static void Info(string msg, float time = 1.5f)
	{
		DebugLogUI.Log(msg, Color.white, time);
		LogUtil.Info(msg);
	}

	public static void Log(string msg, float time = 1.5f)
	{
		DebugLogUI.Log(msg, Color.white, time);
		LogUtil.Info(msg);
	}

	public static void Success(string msg, float time = 1.5f)
	{
		DebugLogUI.Log(msg, new Color(0.4f, 1f, 0.4f), time);
		LogUtil.Success(msg);
	}

	public static void Warn(string msg, float time = 3f)
	{
		DebugLogUI.Log(msg, new Color(1f, 0.8f, 0.2f), time);
		LogUtil.Warn(msg);
	}

	public static void Error(string msg, float time = 5f)
	{
		DebugLogUI.Log(msg, Color.red, time);
		LogUtil.Error(msg);
	}

	public static void L(string msg, float time = 1.5f)
	{
		Log(msg, time);
	}

	public static void I(string msg, float time = 1.5f)
	{
		Info(msg, time);
	}

	public static void S(string msg, float time = 1.5f)
	{
		Success(msg, time);
	}

	public static void W(string msg, float time = 2f)
	{
		Warn(msg, time);
	}

	public static void E(string msg, float time = 2f)
	{
		Error(msg, time);
	}
}
