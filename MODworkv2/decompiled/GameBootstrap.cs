using System;
using System.Collections.Generic;
using Core;
using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using PostProcess;
using UI.DebugUI;

public class GameBootstrap
{
	private static bool _booted;

	private static readonly List<Action> _initSteps = new List<Action> { InitSteam, InitCursor, InitGame, InitPostProcess, InitSettings, InitLocalization, InitAudio, InitApplicationPause, InitFilter, InitDebugUI };

	public static void Boot()
	{
		if (_booted)
		{
			return;
		}
		_booted = true;
		foreach (Action initStep in _initSteps)
		{
			initStep?.Invoke();
		}
	}

	private static void InitSteam()
	{
		SingletonMonoGlobal<SteamManager>.Instance.InitSteam();
	}

	private static void InitCursor()
	{
		SingletonMonoGlobal<CursorManager>.Instance.Init();
	}

	private static void InitGame()
	{
		GameManager.Init();
	}

	private static void InitSettings()
	{
		Singleton<SettingDataManager>.Instance.Init();
	}

	private static void InitLocalization()
	{
		LOC.MM.Init();
	}

	private static void InitAudio()
	{
		SingletonMonoGlobal<AudioManager>.Instance.Init();
	}

	private static void InitApplicationPause()
	{
		SingletonMonoGlobal<ApplicationPauseManager>.Instance.Init();
	}

	private static void InitPostProcess()
	{
		SingletonMonoGlobal<PostProcessManager>.Instance.Init();
	}

	private static void InitDebugUI()
	{
		SingletonMonoGlobal<PerformanceUI>.Instance.Init();
		SingletonMonoGlobal<TestUI>.Instance.Init();
		SingletonMonoGlobal<DebugLogUI>.Instance.Init();
		SingletonMonoGlobal<EnemyRuntimeUI>.Instance.Init();
	}

	private static void InitFilter()
	{
		SingletonMonoGlobal<FilterManager>.Instance.Init();
	}
}
