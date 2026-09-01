using System;
using Core;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Inputs;

public class CurrentInputManager : SingletonMonoGlobal<CurrentInputManager>
{
	[Header("鼠标移动检测")]
	[SerializeField]
	private float mouseMoveThreshold = 20f;

	private Vector3 _lastMousePosition;

	private bool _mousePositionInitialized;

	public InputDeviceType CurrentDeviceType { get; private set; }

	public static event Action<InputDeviceType> OnCurrentInputDeviceChanged;

	public void Init()
	{
		_lastMousePosition = Input.mousePosition;
		_mousePositionInitialized = true;
		ApplyInitialDevicePreference();
		LogUtil.Info("输入设备全局检测器初始化完成");
	}

	protected override void Awake()
	{
		base.Awake();
		_lastMousePosition = Input.mousePosition;
		_mousePositionInitialized = true;
	}

	private void Update()
	{
		if (GamepadInputManager.HasAnyInputForDeviceSwitch())
		{
			SetCurrentDevice(InputDeviceType.Gamepad);
		}
		else if (HasMouseInput() || HasMouseMoveInput() || HasKeyboardInput())
		{
			SetCurrentDevice(InputDeviceType.PC);
		}
	}

	private void ApplyInitialDevicePreference()
	{
		if (SteamManager.IsRunningOnSteamDeck())
		{
			SetCurrentDevice(InputDeviceType.Gamepad);
		}
	}

	private static bool HasMouseInput()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			return true;
		}
		return false;
	}

	private bool HasMouseMoveInput()
	{
		Vector3 mousePosition = Input.mousePosition;
		if (!_mousePositionInitialized)
		{
			_lastMousePosition = mousePosition;
			_mousePositionInitialized = true;
			return false;
		}
		Vector3 vector = mousePosition - _lastMousePosition;
		_lastMousePosition = mousePosition;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = mouseMoveThreshold * mouseMoveThreshold;
		return sqrMagnitude >= num;
	}

	private static bool HasKeyboardInput()
	{
		if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
		{
			return !Input.GetMouseButtonDown(2);
		}
		return false;
	}

	public void SetCurrentDevice(InputDeviceType deviceType)
	{
		if (CurrentDeviceType != deviceType)
		{
			CurrentDeviceType = deviceType;
			GamepadInputManager.ResetStickChangeState();
			CurrentInputManager.OnCurrentInputDeviceChanged?.Invoke(CurrentDeviceType);
		}
	}

	public static bool IsGamepad(InputDeviceType deviceType)
	{
		if (deviceType != InputDeviceType.Gamepad && deviceType != InputDeviceType.Xbox && deviceType != InputDeviceType.PlayStation)
		{
			return deviceType == InputDeviceType.Switch;
		}
		return true;
	}

	public bool IsGamepadCurrent()
	{
		if (CurrentDeviceType != InputDeviceType.Gamepad && CurrentDeviceType != InputDeviceType.Xbox && CurrentDeviceType != InputDeviceType.PlayStation)
		{
			return CurrentDeviceType == InputDeviceType.Switch;
		}
		return true;
	}

	public bool IsPcCurrent()
	{
		return CurrentDeviceType == InputDeviceType.PC;
	}
}
