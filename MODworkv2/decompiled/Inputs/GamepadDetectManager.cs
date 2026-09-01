using System;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Inputs;

public class GamepadDetectManager : SingletonMonoGlobal<GamepadDetectManager>
{
	private float checkTimer;

	private const float CheckInterval = 1f;

	public bool HasGamepad { get; private set; }

	public string[] CurrentJoystickNames { get; private set; }

	public static event Action<bool> OnGamepadConnectionChanged;

	public void Init()
	{
		LogUtil.Info("手柄全局管理器初始化完成");
	}

	protected override void Awake()
	{
		base.Awake();
		CurrentJoystickNames = Input.GetJoystickNames();
		HasGamepad = HasAnyValidJoystick(CurrentJoystickNames);
	}

	private void Update()
	{
		checkTimer += Time.unscaledDeltaTime;
		if (!(checkTimer < 1f))
		{
			checkTimer = 0f;
			RefreshNow();
		}
	}

	private void RefreshNow()
	{
		string[] joystickNames = Input.GetJoystickNames();
		bool flag = HasAnyValidJoystick(joystickNames);
		bool num = flag != HasGamepad;
		HasGamepad = flag;
		CurrentJoystickNames = joystickNames;
		if (num)
		{
			GamepadDetectManager.OnGamepadConnectionChanged?.Invoke(HasGamepad);
		}
	}

	private static bool HasAnyValidJoystick(string[] names)
	{
		if (names == null || names.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < names.Length; i++)
		{
			if (!string.IsNullOrWhiteSpace(names[i]))
			{
				return true;
			}
		}
		return false;
	}
}
