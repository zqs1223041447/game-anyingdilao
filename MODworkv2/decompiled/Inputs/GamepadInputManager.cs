using UnityEngine;

namespace Inputs;

public static class GamepadInputManager
{
	public const string LeftStickXAxis = "Pad_UI_LeftStickX";

	public const string LeftStickYAxis = "Pad_UI_LeftStickY";

	public const string DPadXAxis = "Pad_UI_DPadX";

	public const string DPadYAxis = "Pad_UI_DPadY";

	public const string RightStickXAxis = "Pad_UI_RightStickX";

	public const string RightStickYAxis = "Pad_UI_RightStickY";

	public const string SubmitButton = "Pad_UI_Submit";

	public const string CancelButton = "Pad_UI_Cancel";

	public const string MenuButton = "Pad_UI_Menu";

	public const string LeftTriggerAxis = "Pad_LT";

	public const string RightTriggerAxis = "Pad_RT";

	public const string SubmitRawKey = "Pad_A";

	public const string CancelRawKey = "Pad_B";

	public static bool invertLeftStickX;

	public static bool invertLeftStickY;

	public static bool invertRightStickX;

	public static bool invertRightStickY;

	public const float MoveDeadZone = 0.2f;

	private static bool _leftStickSuppressedUntilNeutral;

	private const float DPadThreshold = 0.5f;

	private const float TriggerThreshold = 0.5f;

	private static bool _lastLTPressed;

	private static bool _lastRTPressed;

	private static bool _lastDPadUpPressed;

	private static bool _lastDPadDownPressed;

	private static bool _lastDPadLeftPressed;

	private static bool _lastDPadRightPressed;

	private static bool _lastDeviceSwitchLTPressed;

	private static bool _lastDeviceSwitchRTPressed;

	private static bool _lastDeviceSwitchDPadUpPressed;

	private static bool _lastDeviceSwitchDPadDownPressed;

	private static bool _lastDeviceSwitchDPadLeftPressed;

	private static bool _lastDeviceSwitchDPadRightPressed;

	private static bool _lastDeviceSwitchLeftStickActive;

	private static bool _lastDeviceSwitchRightStickActive;

	private const float StickChangeThreshold = 0.1f;

	private static float _lastLeftStickX;

	private static float _lastLeftStickY;

	private static float _lastRightStickX;

	private static float _lastRightStickY;

	private static bool _stickStateInitialized;

	public static void SuppressLeftStickUntilNeutral()
	{
		_leftStickSuppressedUntilNeutral = GetLeftStickMagnitudeRaw() >= 0.2f;
	}

	public static void SetInvertLeftX(bool i)
	{
		invertLeftStickX = i;
	}

	public static void SetInvertLeftY(bool i)
	{
		invertLeftStickY = !i;
	}

	public static void SetInvertRightX(bool i)
	{
		invertRightStickX = i;
	}

	public static void SetInvertRightY(bool i)
	{
		invertRightStickY = !i;
	}

	public static bool GetSubmit()
	{
		return Input.GetButton("Pad_UI_Submit");
	}

	public static bool GetCancel()
	{
		return Input.GetButton("Pad_UI_Cancel");
	}

	public static bool GetSubmitDown()
	{
		return Input.GetButtonDown("Pad_UI_Submit");
	}

	public static bool GetCancelDown()
	{
		return Input.GetButtonDown("Pad_UI_Cancel");
	}

	public static bool GetSubmitUp()
	{
		return Input.GetButtonUp("Pad_UI_Submit");
	}

	public static bool GetCancelUp()
	{
		return Input.GetButtonUp("Pad_UI_Cancel");
	}

	public static bool GetMenuDown()
	{
		return Input.GetButtonDown("Pad_UI_Menu");
	}

	public static bool IsAnyPressed()
	{
		if (GetKey("Pad_A"))
		{
			return true;
		}
		if (GetKey("Pad_B"))
		{
			return true;
		}
		if (GetKey("Pad_X"))
		{
			return true;
		}
		if (GetKey("Pad_Y"))
		{
			return true;
		}
		if (GetKey("Pad_LB"))
		{
			return true;
		}
		if (GetKey("Pad_LT"))
		{
			return true;
		}
		if (GetKey("Pad_RB"))
		{
			return true;
		}
		if (GetKey("Pad_RT"))
		{
			return true;
		}
		if (GetKey("Pad_DPadUp"))
		{
			return true;
		}
		if (GetKey("Pad_DPadDown"))
		{
			return true;
		}
		if (GetKey("Pad_DPadLeft"))
		{
			return true;
		}
		if (GetKey("Pad_DPadRight"))
		{
			return true;
		}
		if (GetKey("Pad_LStickPress"))
		{
			return true;
		}
		if (GetKey("Pad_RStickPress"))
		{
			return true;
		}
		if (GetKey("Pad_Back"))
		{
			return true;
		}
		if (GetKey("Pad_Menu"))
		{
			return true;
		}
		return false;
	}

	public static bool HasAnyInputForDeviceSwitch()
	{
		bool flag = false;
		for (int i = 0; i <= 19; i++)
		{
			if (Input.GetKeyDown((KeyCode)(330 + i)))
			{
				flag = true;
				break;
			}
		}
		bool num = Input.GetAxisRaw("Pad_LT") >= 0.5f;
		bool flag2 = Input.GetAxisRaw("Pad_RT") >= 0.5f;
		float axisRaw = Input.GetAxisRaw("Pad_UI_DPadX");
		float axisRaw2 = Input.GetAxisRaw("Pad_UI_DPadY");
		bool flag3 = axisRaw2 >= 0.5f;
		bool flag4 = axisRaw2 <= -0.5f;
		bool flag5 = axisRaw <= -0.5f;
		bool flag6 = axisRaw >= 0.5f;
		bool flag7 = new Vector2(Input.GetAxisRaw("Pad_UI_LeftStickX"), Input.GetAxisRaw("Pad_UI_LeftStickY")).magnitude >= 0.5f;
		bool flag8 = new Vector2(Input.GetAxisRaw("Pad_UI_RightStickX"), Input.GetAxisRaw("Pad_UI_RightStickY")).magnitude >= 0.5f;
		bool flag9 = (num && !_lastDeviceSwitchLTPressed) || (flag2 && !_lastDeviceSwitchRTPressed) || (flag3 && !_lastDeviceSwitchDPadUpPressed) || (flag4 && !_lastDeviceSwitchDPadDownPressed) || (flag5 && !_lastDeviceSwitchDPadLeftPressed) || (flag6 && !_lastDeviceSwitchDPadRightPressed) || (flag7 && !_lastDeviceSwitchLeftStickActive) || (flag8 && !_lastDeviceSwitchRightStickActive);
		_lastDeviceSwitchLTPressed = num;
		_lastDeviceSwitchRTPressed = flag2;
		_lastDeviceSwitchDPadUpPressed = flag3;
		_lastDeviceSwitchDPadDownPressed = flag4;
		_lastDeviceSwitchDPadLeftPressed = flag5;
		_lastDeviceSwitchDPadRightPressed = flag6;
		_lastDeviceSwitchLeftStickActive = flag7;
		_lastDeviceSwitchRightStickActive = flag8;
		return flag || flag9;
	}

	public static bool TryGetPressedKeyForRebind(out string rawKey)
	{
		rawKey = null;
		if (Input.GetKey(KeyCode.JoystickButton0))
		{
			rawKey = "Pad_A";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton1))
		{
			rawKey = "Pad_B";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton2))
		{
			rawKey = "Pad_X";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton3))
		{
			rawKey = "Pad_Y";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton4))
		{
			rawKey = "Pad_LB";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton5))
		{
			rawKey = "Pad_RB";
			return true;
		}
		if (Input.GetAxisRaw("Pad_LT") >= 0.5f)
		{
			rawKey = "Pad_LT";
			return true;
		}
		if (Input.GetAxisRaw("Pad_RT") >= 0.5f)
		{
			rawKey = "Pad_RT";
			return true;
		}
		float axisRaw = Input.GetAxisRaw("Pad_UI_DPadX");
		float axisRaw2 = Input.GetAxisRaw("Pad_UI_DPadY");
		if (axisRaw2 >= 0.5f)
		{
			rawKey = "Pad_DPadUp";
			return true;
		}
		if (axisRaw2 <= -0.5f)
		{
			rawKey = "Pad_DPadDown";
			return true;
		}
		if (axisRaw <= -0.5f)
		{
			rawKey = "Pad_DPadLeft";
			return true;
		}
		if (axisRaw >= 0.5f)
		{
			rawKey = "Pad_DPadRight";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton8))
		{
			rawKey = "Pad_LStickPress";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton9))
		{
			rawKey = "Pad_RStickPress";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton6))
		{
			rawKey = "Pad_Back";
			return true;
		}
		if (Input.GetKey(KeyCode.JoystickButton7))
		{
			rawKey = "Pad_Menu";
			return true;
		}
		return false;
	}

	public static bool TryCreateBind(string rawKey, out BindKey bindKey)
	{
		bindKey = null;
		if (string.IsNullOrEmpty(rawKey))
		{
			return false;
		}
		switch (rawKey)
		{
		case "Pad_A":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton0);
			return true;
		case "Pad_B":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton1);
			return true;
		case "Pad_X":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton2);
			return true;
		case "Pad_Y":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton3);
			return true;
		case "Pad_LB":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton4);
			return true;
		case "Pad_RB":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton5);
			return true;
		case "Pad_LT":
			bindKey = new GamepadAxisBind("Pad_LT", 1);
			return true;
		case "Pad_RT":
			bindKey = new GamepadAxisBind("Pad_RT", 1);
			return true;
		case "Pad_Back":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton6);
			return true;
		case "Pad_Menu":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton7);
			return true;
		case "Pad_LStickPress":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton8);
			return true;
		case "Pad_RStickPress":
			bindKey = new GamepadButtonBind(KeyCode.JoystickButton9);
			return true;
		case "Pad_DPadUp":
			bindKey = new GamepadAxisBind("Pad_UI_DPadY", 1);
			return true;
		case "Pad_DPadDown":
			bindKey = new GamepadAxisBind("Pad_UI_DPadY", -1);
			return true;
		case "Pad_DPadLeft":
			bindKey = new GamepadAxisBind("Pad_UI_DPadX", -1);
			return true;
		case "Pad_DPadRight":
			bindKey = new GamepadAxisBind("Pad_UI_DPadX", 1);
			return true;
		case "Pad_LeftStickUp":
			bindKey = new GamepadAxisBind("Pad_UI_LeftStickY", 1);
			return true;
		case "Pad_LeftStickDown":
			bindKey = new GamepadAxisBind("Pad_UI_LeftStickY", -1);
			return true;
		case "Pad_LeftStickLeft":
			bindKey = new GamepadAxisBind("Pad_UI_LeftStickX", -1);
			return true;
		case "Pad_LeftStickRight":
			bindKey = new GamepadAxisBind("Pad_UI_LeftStickX", 1);
			return true;
		case "Pad_RightStickUp":
			bindKey = new GamepadAxisBind("Pad_UI_RightStickY", 1);
			return true;
		case "Pad_RightStickDown":
			bindKey = new GamepadAxisBind("Pad_UI_RightStickY", -1);
			return true;
		case "Pad_RightStickLeft":
			bindKey = new GamepadAxisBind("Pad_UI_RightStickX", -1);
			return true;
		case "Pad_RightStickRight":
			bindKey = new GamepadAxisBind("Pad_UI_RightStickX", 1);
			return true;
		default:
			return false;
		}
	}

	public static bool GetKeyDown(string rawKey)
	{
		if (string.IsNullOrEmpty(rawKey))
		{
			return false;
		}
		return rawKey switch
		{
			"Pad_A" => Input.GetKeyDown(KeyCode.JoystickButton0), 
			"Pad_B" => Input.GetKeyDown(KeyCode.JoystickButton1), 
			"Pad_X" => Input.GetKeyDown(KeyCode.JoystickButton2), 
			"Pad_Y" => Input.GetKeyDown(KeyCode.JoystickButton3), 
			"Pad_LB" => Input.GetKeyDown(KeyCode.JoystickButton4), 
			"Pad_RB" => Input.GetKeyDown(KeyCode.JoystickButton5), 
			"Pad_Back" => Input.GetKeyDown(KeyCode.JoystickButton6), 
			"Pad_Menu" => Input.GetKeyDown(KeyCode.JoystickButton7), 
			"Pad_LStickPress" => Input.GetKeyDown(KeyCode.JoystickButton8), 
			"Pad_RStickPress" => Input.GetKeyDown(KeyCode.JoystickButton9), 
			_ => false, 
		};
	}

	public static bool GetKeyUp(string rawKey)
	{
		if (string.IsNullOrEmpty(rawKey))
		{
			return false;
		}
		return rawKey switch
		{
			"Pad_A" => Input.GetKeyUp(KeyCode.JoystickButton0), 
			"Pad_B" => Input.GetKeyUp(KeyCode.JoystickButton1), 
			"Pad_X" => Input.GetKeyUp(KeyCode.JoystickButton2), 
			"Pad_Y" => Input.GetKeyUp(KeyCode.JoystickButton3), 
			"Pad_LB" => Input.GetKeyUp(KeyCode.JoystickButton4), 
			"Pad_RB" => Input.GetKeyUp(KeyCode.JoystickButton5), 
			"Pad_Back" => Input.GetKeyUp(KeyCode.JoystickButton6), 
			"Pad_Menu" => Input.GetKeyUp(KeyCode.JoystickButton7), 
			"Pad_LStickPress" => Input.GetKeyUp(KeyCode.JoystickButton8), 
			"Pad_RStickPress" => Input.GetKeyUp(KeyCode.JoystickButton9), 
			"Pad_LT" => Input.GetAxisRaw("Pad_LT") < 0.5f, 
			"Pad_RT" => Input.GetAxisRaw("Pad_RT") < 0.5f, 
			"Pad_DPadUp" => Input.GetAxisRaw("Pad_UI_DPadY") < 0.5f, 
			"Pad_DPadDown" => Input.GetAxisRaw("Pad_UI_DPadY") > -0.5f, 
			"Pad_DPadLeft" => Input.GetAxisRaw("Pad_UI_DPadX") > -0.5f, 
			"Pad_DPadRight" => Input.GetAxisRaw("Pad_UI_DPadX") < 0.5f, 
			_ => false, 
		};
	}

	public static bool GetAnyKeyDown(out string rawKey)
	{
		rawKey = null;
		bool keyDown = Input.GetKeyDown(KeyCode.JoystickButton0);
		bool keyDown2 = Input.GetKeyDown(KeyCode.JoystickButton1);
		bool keyDown3 = Input.GetKeyDown(KeyCode.JoystickButton2);
		bool keyDown4 = Input.GetKeyDown(KeyCode.JoystickButton3);
		bool keyDown5 = Input.GetKeyDown(KeyCode.JoystickButton4);
		bool keyDown6 = Input.GetKeyDown(KeyCode.JoystickButton5);
		bool keyDown7 = Input.GetKeyDown(KeyCode.JoystickButton6);
		bool keyDown8 = Input.GetKeyDown(KeyCode.JoystickButton7);
		bool keyDown9 = Input.GetKeyDown(KeyCode.JoystickButton8);
		bool keyDown10 = Input.GetKeyDown(KeyCode.JoystickButton9);
		bool flag = Input.GetAxisRaw("Pad_LT") >= 0.5f;
		bool flag2 = Input.GetAxisRaw("Pad_RT") >= 0.5f;
		float axisRaw = Input.GetAxisRaw("Pad_UI_DPadX");
		float axisRaw2 = Input.GetAxisRaw("Pad_UI_DPadY");
		bool flag3 = axisRaw2 >= 0.5f;
		bool flag4 = axisRaw2 <= -0.5f;
		bool flag5 = axisRaw <= -0.5f;
		bool num = axisRaw >= 0.5f;
		bool flag6 = flag && !_lastLTPressed;
		bool flag7 = flag2 && !_lastRTPressed;
		bool flag8 = flag3 && !_lastDPadUpPressed;
		bool flag9 = flag4 && !_lastDPadDownPressed;
		bool flag10 = flag5 && !_lastDPadLeftPressed;
		bool flag11 = num && !_lastDPadRightPressed;
		_lastLTPressed = flag;
		_lastRTPressed = flag2;
		_lastDPadUpPressed = flag3;
		_lastDPadDownPressed = flag4;
		_lastDPadLeftPressed = flag5;
		_lastDPadRightPressed = num;
		if (keyDown)
		{
			rawKey = "Pad_A";
			return true;
		}
		if (keyDown2)
		{
			rawKey = "Pad_B";
			return true;
		}
		if (keyDown3)
		{
			rawKey = "Pad_X";
			return true;
		}
		if (keyDown4)
		{
			rawKey = "Pad_Y";
			return true;
		}
		if (keyDown5)
		{
			rawKey = "Pad_LB";
			return true;
		}
		if (keyDown6)
		{
			rawKey = "Pad_RB";
			return true;
		}
		if (keyDown7)
		{
			rawKey = "Pad_Back";
			return true;
		}
		if (keyDown8)
		{
			rawKey = "Pad_Menu";
			return true;
		}
		if (keyDown9)
		{
			rawKey = "Pad_LStickPress";
			return true;
		}
		if (keyDown10)
		{
			rawKey = "Pad_RStickPress";
			return true;
		}
		if (flag6)
		{
			rawKey = "Pad_LT";
			return true;
		}
		if (flag7)
		{
			rawKey = "Pad_RT";
			return true;
		}
		if (flag8)
		{
			rawKey = "Pad_DPadUp";
			return true;
		}
		if (flag9)
		{
			rawKey = "Pad_DPadDown";
			return true;
		}
		if (flag10)
		{
			rawKey = "Pad_DPadLeft";
			return true;
		}
		if (flag11)
		{
			rawKey = "Pad_DPadRight";
			return true;
		}
		return false;
	}

	public static bool GetKey(string rawKey)
	{
		if (string.IsNullOrEmpty(rawKey))
		{
			return false;
		}
		return rawKey switch
		{
			"Pad_A" => Input.GetKey(KeyCode.JoystickButton0), 
			"Pad_B" => Input.GetKey(KeyCode.JoystickButton1), 
			"Pad_X" => Input.GetKey(KeyCode.JoystickButton2), 
			"Pad_Y" => Input.GetKey(KeyCode.JoystickButton3), 
			"Pad_LB" => Input.GetKey(KeyCode.JoystickButton4), 
			"Pad_RB" => Input.GetKey(KeyCode.JoystickButton5), 
			"Pad_Back" => Input.GetKey(KeyCode.JoystickButton6), 
			"Pad_Menu" => Input.GetKey(KeyCode.JoystickButton7), 
			"Pad_LStickPress" => Input.GetKey(KeyCode.JoystickButton8), 
			"Pad_RStickPress" => Input.GetKey(KeyCode.JoystickButton9), 
			"Pad_LT" => Input.GetAxisRaw("Pad_LT") >= 0.5f, 
			"Pad_RT" => Input.GetAxisRaw("Pad_RT") >= 0.5f, 
			"Pad_DPadUp" => Input.GetAxisRaw("Pad_UI_DPadY") >= 0.5f, 
			"Pad_DPadDown" => Input.GetAxisRaw("Pad_UI_DPadY") <= -0.5f, 
			"Pad_DPadLeft" => Input.GetAxisRaw("Pad_UI_DPadX") <= -0.5f, 
			"Pad_DPadRight" => Input.GetAxisRaw("Pad_UI_DPadX") >= 0.5f, 
			_ => false, 
		};
	}

	public static float GetLeftStickXRaw()
	{
		float axisRaw = Input.GetAxisRaw("Pad_UI_LeftStickX");
		axisRaw = (invertLeftStickX ? (0f - axisRaw) : axisRaw);
		if (!IsLeftStickSuppressed())
		{
			return axisRaw;
		}
		return 0f;
	}

	public static float GetLeftStickYRaw()
	{
		float axisRaw = Input.GetAxisRaw("Pad_UI_LeftStickY");
		axisRaw = (invertLeftStickY ? (0f - axisRaw) : axisRaw);
		if (!IsLeftStickSuppressed())
		{
			return axisRaw;
		}
		return 0f;
	}

	public static float GetRightStickXRaw()
	{
		float axisRaw = Input.GetAxisRaw("Pad_UI_RightStickX");
		if (!invertRightStickX)
		{
			return axisRaw;
		}
		return 0f - axisRaw;
	}

	public static float GetRightStickYRaw()
	{
		float axisRaw = Input.GetAxisRaw("Pad_UI_RightStickY");
		if (!invertRightStickY)
		{
			return axisRaw;
		}
		return 0f - axisRaw;
	}

	public static float GetDPadXRaw()
	{
		return Input.GetAxisRaw("Pad_UI_DPadX");
	}

	public static float GetDPadYRaw()
	{
		return Input.GetAxisRaw("Pad_UI_DPadY");
	}

	public static float ApplyDeadZone(float value, float deadZone = 0.2f)
	{
		if (!(Mathf.Abs(value) < deadZone))
		{
			return value;
		}
		return 0f;
	}

	private static bool IsLeftStickSuppressed()
	{
		if (!_leftStickSuppressedUntilNeutral)
		{
			return false;
		}
		if (GetLeftStickMagnitudeRaw() >= 0.2f)
		{
			return true;
		}
		_leftStickSuppressedUntilNeutral = false;
		return false;
	}

	private static float GetLeftStickMagnitudeRaw()
	{
		float num = Input.GetAxisRaw("Pad_UI_LeftStickX");
		float num2 = Input.GetAxisRaw("Pad_UI_LeftStickY");
		if (invertLeftStickX)
		{
			num = 0f - num;
		}
		if (invertLeftStickY)
		{
			num2 = 0f - num2;
		}
		return new Vector2(num, num2).magnitude;
	}

	public static bool HasLeftStickValueChanged(float deadZone = 0.2f, float changeThreshold = 0.1f)
	{
		float num = ApplyDeadZone(GetLeftStickXRaw(), deadZone);
		float num2 = ApplyDeadZone(GetLeftStickYRaw(), deadZone);
		if (!_stickStateInitialized)
		{
			_lastLeftStickX = num;
			_lastLeftStickY = num2;
			_lastRightStickX = ApplyDeadZone(GetRightStickXRaw(), deadZone);
			_lastRightStickY = ApplyDeadZone(GetRightStickYRaw(), deadZone);
			_stickStateInitialized = true;
			return false;
		}
		bool result = Mathf.Abs(num - _lastLeftStickX) >= changeThreshold || Mathf.Abs(num2 - _lastLeftStickY) >= changeThreshold;
		_lastLeftStickX = num;
		_lastLeftStickY = num2;
		return result;
	}

	public static bool HasRightStickValueChanged(float deadZone = 0.2f, float changeThreshold = 0.1f)
	{
		float num = ApplyDeadZone(GetRightStickXRaw(), deadZone);
		float num2 = ApplyDeadZone(GetRightStickYRaw(), deadZone);
		if (!_stickStateInitialized)
		{
			_lastLeftStickX = ApplyDeadZone(GetLeftStickXRaw(), deadZone);
			_lastLeftStickY = ApplyDeadZone(GetLeftStickYRaw(), deadZone);
			_lastRightStickX = num;
			_lastRightStickY = num2;
			_stickStateInitialized = true;
			return false;
		}
		bool result = Mathf.Abs(num - _lastRightStickX) >= changeThreshold || Mathf.Abs(num2 - _lastRightStickY) >= changeThreshold;
		_lastRightStickX = num;
		_lastRightStickY = num2;
		return result;
	}

	public static bool HasAnyStickValueChanged(float deadZone = 0.2f, float changeThreshold = 0.1f)
	{
		float num = ApplyDeadZone(GetLeftStickXRaw(), deadZone);
		float num2 = ApplyDeadZone(GetLeftStickYRaw(), deadZone);
		float num3 = ApplyDeadZone(GetRightStickXRaw(), deadZone);
		float num4 = ApplyDeadZone(GetRightStickYRaw(), deadZone);
		if (!_stickStateInitialized)
		{
			_lastLeftStickX = num;
			_lastLeftStickY = num2;
			_lastRightStickX = num3;
			_lastRightStickY = num4;
			_stickStateInitialized = true;
			return false;
		}
		bool result = Mathf.Abs(num - _lastLeftStickX) >= changeThreshold || Mathf.Abs(num2 - _lastLeftStickY) >= changeThreshold || Mathf.Abs(num3 - _lastRightStickX) >= changeThreshold || Mathf.Abs(num4 - _lastRightStickY) >= changeThreshold;
		_lastLeftStickX = num;
		_lastLeftStickY = num2;
		_lastRightStickX = num3;
		_lastRightStickY = num4;
		return result;
	}

	public static bool IsAnyStickActive(float deadZone = 0.2f)
	{
		float a = ApplyDeadZone(GetLeftStickXRaw(), deadZone);
		float a2 = ApplyDeadZone(GetLeftStickYRaw(), deadZone);
		float a3 = ApplyDeadZone(GetRightStickXRaw(), deadZone);
		float a4 = ApplyDeadZone(GetRightStickYRaw(), deadZone);
		if (Mathf.Approximately(a, 0f) && Mathf.Approximately(a2, 0f) && Mathf.Approximately(a3, 0f))
		{
			return !Mathf.Approximately(a4, 0f);
		}
		return true;
	}

	public static void ResetStickChangeState()
	{
		_stickStateInitialized = false;
		_lastLeftStickX = 0f;
		_lastLeftStickY = 0f;
		_lastRightStickX = 0f;
		_lastRightStickY = 0f;
	}
}
