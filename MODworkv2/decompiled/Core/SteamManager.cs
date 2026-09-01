using System;
using Core.Settings;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Steamworks;
using UnityEngine;

namespace Core;

public class SteamManager : SingletonMonoGlobal<SteamManager>
{
	private const uint appId = 1104194u;

	private static bool steamInited;

	private static bool steamActivityWarningLogged;

	public void InitSteam()
	{
		if (!SettingsLoader.Instance.SteamToggle)
		{
			return;
		}
		try
		{
			SteamClient.Init(1104194u);
			steamInited = true;
			LogUtil.Success("Steam 初始化成功");
		}
		catch (Exception ex)
		{
			steamInited = false;
			LogUtil.Error("Steam 初始化失败: " + ex.Message);
		}
	}

	private void Update()
	{
		if (SettingsLoader.Instance.SteamToggle && steamInited)
		{
			SteamClient.RunCallbacks();
		}
	}

	private void OnApplicationQuit()
	{
		if (SettingsLoader.Instance.SteamToggle)
		{
			ShutdownSteam();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (SettingsLoader.Instance.SteamToggle)
		{
			ShutdownSteam();
		}
	}

	private static void ShutdownSteam()
	{
		if (SettingsLoader.Instance.SteamToggle && steamInited)
		{
			SteamClient.Shutdown();
			steamInited = false;
			LogUtil.Info("Steam 已关闭");
		}
	}

	public static bool IsSteamReady()
	{
		if (!SettingsLoader.Instance.SteamToggle)
		{
			return false;
		}
		if (steamInited)
		{
			return SteamClient.IsValid;
		}
		return false;
	}

	public static bool TryGetSecondsSinceAppActive(out uint seconds)
	{
		seconds = 0u;
		if (!IsSteamReady())
		{
			return false;
		}
		try
		{
			seconds = SteamUtils.SecondsSinceAppActive;
			steamActivityWarningLogged = false;
			return true;
		}
		catch (Exception ex)
		{
			if (!steamActivityWarningLogged)
			{
				LogUtil.Warn("Steam 应用活跃状态检测失败: " + ex.Message);
				steamActivityWarningLogged = true;
			}
			return false;
		}
	}

	public static bool IsRunningOnSteamDeck()
	{
		if (HasSteamDeckFallbackHint())
		{
			return true;
		}
		if (!IsSteamReady())
		{
			return false;
		}
		try
		{
			return SteamUtils.IsRunningOnSteamDeck;
		}
		catch (Exception ex)
		{
			LogUtil.Warn("Steam Deck 检测失败: " + ex.Message);
			return false;
		}
	}

	private static bool HasSteamDeckFallbackHint()
	{
		if (IsTruthyEnvironmentValue(Environment.GetEnvironmentVariable("SteamDeck")))
		{
			return true;
		}
		if (!ContainsIgnoreCase(SystemInfo.deviceModel, "Steam Deck") && !ContainsIgnoreCase(SystemInfo.deviceName, "Steam Deck") && !ContainsIgnoreCase(SystemInfo.deviceModel, "Jupiter"))
		{
			return ContainsIgnoreCase(SystemInfo.deviceModel, "Galileo");
		}
		return true;
	}

	private static bool IsTruthyEnvironmentValue(string value)
	{
		if (!string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) && !string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static bool ContainsIgnoreCase(string value, string target)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return value.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return false;
	}
}
