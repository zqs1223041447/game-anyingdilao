namespace Inputs;

public static class GamepadKeys
{
	public const string PadA = "Pad_A";

	public const string PadB = "Pad_B";

	public const string PadX = "Pad_X";

	public const string PadY = "Pad_Y";

	public const string PadLB = "Pad_LB";

	public const string PadLT = "Pad_LT";

	public const string PadRB = "Pad_RB";

	public const string PadRT = "Pad_RT";

	public const string PadDPadUp = "Pad_DPadUp";

	public const string PadDPadDown = "Pad_DPadDown";

	public const string PadDPadLeft = "Pad_DPadLeft";

	public const string PadDPadRight = "Pad_DPadRight";

	public const string PadLStickPress = "Pad_LStickPress";

	public const string PadRStickPress = "Pad_RStickPress";

	public const string PadBack = "Pad_Back";

	public const string PadMenu = "Pad_Menu";

	public const string PadLeftStickUp = "Pad_LeftStickUp";

	public const string PadLeftStickDown = "Pad_LeftStickDown";

	public const string PadLeftStickLeft = "Pad_LeftStickLeft";

	public const string PadLeftStickRight = "Pad_LeftStickRight";

	public const string PadRightStickUp = "Pad_RightStickUp";

	public const string PadRightStickDown = "Pad_RightStickDown";

	public const string PadRightStickLeft = "Pad_RightStickLeft";

	public const string PadRightStickRight = "Pad_RightStickRight";

	public static bool CanBind(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		switch (key)
		{
		case "Pad_X":
		case "Pad_Y":
		case "Pad_A":
		case "Pad_B":
		case "Pad_LB":
		case "Pad_LT":
		case "Pad_RB":
		case "Pad_RT":
		case "Pad_DPadDown":
		case "Pad_DPadLeft":
		case "Pad_LStickPress":
		case "Pad_RStickPress":
		case "Pad_DPadUp":
		case "Pad_DPadRight":
		case "Pad_Back":
			return true;
		default:
			return false;
		}
	}

	public static bool IsSystemOnlyKey(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		switch (key)
		{
		case "Pad_Back":
		case "Pad_Menu":
		case "Pad_LeftStickDown":
		case "Pad_LeftStickLeft":
		case "Pad_RightStickDown":
		case "Pad_RightStickLeft":
		case "Pad_LeftStickRight":
		case "Pad_LeftStickUp":
		case "Pad_RightStickUp":
		case "Pad_RightStickRight":
			return true;
		default:
			return false;
		}
	}

	public static bool IsStickDirection(string key)
	{
		switch (key)
		{
		case "Pad_LeftStickDown":
		case "Pad_LeftStickLeft":
		case "Pad_RightStickDown":
		case "Pad_RightStickLeft":
		case "Pad_LeftStickRight":
		case "Pad_LeftStickUp":
		case "Pad_RightStickUp":
		case "Pad_RightStickRight":
			return true;
		default:
			return false;
		}
	}
}
