namespace Inputs;

public static class KeyNameUtil
{
	public static string NormalizeKeyName(string raw)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return string.Empty;
		}
		raw = raw.Trim();
		switch (raw)
		{
		case "Alt":
		case "Alt_L":
		case "LeftAlt":
			return "LeftAlt";
		case "Alt_R":
		case "RightAlt":
			return "RightAlt";
		case "Control":
		case "Ctrl_L":
		case "LeftControl":
		case "Ctrl":
			return "LeftControl";
		case "Ctrl_R":
		case "RightControl":
			return "RightControl";
		case "Shift":
		case "Shift_L":
		case "LeftShift":
			return "LeftShift";
		case "Shift_R":
		case "RightShift":
			return "RightShift";
		case "Enter":
		case "Return":
			return "Return";
		case "Esc":
		case "Escape":
			return "Escape";
		case "Pad_LS":
		case "Pad_LeftStickPress":
			return "Pad_LStickPress";
		case "Pad_RS":
		case "Pad_RightStickPress":
			return "Pad_RStickPress";
		case "Pad_DPad_Up":
			return "Pad_DPadUp";
		case "Pad_DPad_Down":
			return "Pad_DPadDown";
		case "Pad_DPadLeft":
		case "Pad_DPad_Left":
			return "Pad_DPadLeft";
		case "Pad_DPadRight":
		case "Pad_DPad_Right":
			return "Pad_DPadRight";
		default:
			return raw;
		}
	}
}
