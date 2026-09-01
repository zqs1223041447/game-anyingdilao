using System;
using FinkFramework.Runtime.Singleton;
using Inputs;

public static class KeyDisplayUtil
{
	public static string ToDisplayName(string raw)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return string.Empty;
		}
		raw = raw.Trim();
		raw = KeyNameUtil.NormalizeKeyName(raw);
		string gamepadDisplayName = GetGamepadDisplayName(raw);
		if (!string.IsNullOrEmpty(gamepadDisplayName))
		{
			return gamepadDisplayName;
		}
		if (raw.StartsWith("Mouse", StringComparison.OrdinalIgnoreCase))
		{
			return raw switch
			{
				"Mouse0" => LOC.MM.GetStart("control_mouse0"), 
				"Mouse1" => LOC.MM.GetStart("control_mouse1"), 
				"Mouse2" => LOC.MM.GetStart("control_mouse2"), 
				"Mouse3" => LOC.MM.GetStart("control_mouse3"), 
				"Mouse4" => LOC.MM.GetStart("control_mouse4"), 
				"Mouse5" => LOC.MM.GetStart("control_mouse5"), 
				"Mouse6" => LOC.MM.GetStart("control_mouse6"), 
				"Mouse7" => LOC.MM.GetStart("control_mouse7"), 
				"Mouse8" => LOC.MM.GetStart("control_mouse8"), 
				"Mouse_WheelUp" => LOC.MM.GetStart("control_mouse_wheel_up"), 
				"Mouse_WheelDown" => LOC.MM.GetStart("control_mouse_wheel_down"), 
				_ => raw, 
			};
		}
		if (raw.StartsWith("Keypad", StringComparison.OrdinalIgnoreCase))
		{
			return raw.Replace("Keypad", "Num ");
		}
		if (raw.StartsWith("Alpha", StringComparison.OrdinalIgnoreCase))
		{
			return raw.Replace("Alpha", "");
		}
		switch (raw)
		{
		case "Space":
			return "Space";
		case "LeftShift":
			return "Shift";
		case "RightShift":
			return "Shift";
		case "LeftControl":
			return "Ctrl";
		case "RightControl":
			return "Ctrl";
		case "LeftAlt":
			return "Alt";
		case "RightAlt":
			return "Alt";
		case "Return":
			return "Enter";
		case "KeypadEnter":
			return "Num Enter";
		case "Escape":
			return "Esc";
		case "Tab":
			return "Tab";
		case "CapsLock":
			return "Caps";
		case "Backspace":
			return "Backspace";
		case "Delete":
			return "Delete";
		case "Insert":
			return "Insert";
		case "Home":
			return "Home";
		case "End":
			return "End";
		case "PageUp":
			return "PgUp";
		case "PageDown":
			return "PgDn";
		case "BackQuote":
			return "`";
		case "Minus":
			return "-";
		case "Equals":
			return "=";
		case "LeftBracket":
			return "[";
		case "RightBracket":
			return "]";
		case "Backslash":
			return "\\";
		case "Semicolon":
			return ";";
		case "Quote":
			return "'";
		case "Comma":
			return ",";
		case "Period":
			return ".";
		case "Slash":
			return "/";
		case "UpArrow":
			return "↑";
		case "DownArrow":
			return "↓";
		case "LeftArrow":
			return "←";
		case "RightArrow":
			return "→";
		default:
			IsFunctionKey(raw);
			return raw;
		}
	}

	public static bool TryGetSpriteRichText(string raw, out string richText)
	{
		richText = null;
		if (string.IsNullOrEmpty(raw))
		{
			return false;
		}
		raw = raw.Trim();
		if (TryGetSpriteTag(raw, out richText))
		{
			return true;
		}
		return false;
	}

	public static string GetLocalKeyName(ControlAction action)
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return ToDisplayName(Singleton<SettingDataManager>.Instance.GetControl(InputDeviceType.PC).GetBind(action));
		}
		return ToDisplayName(Singleton<SettingDataManager>.Instance.GetControl(InputDeviceType.Gamepad).GetBind(action));
	}

	public static string ToRichDisplay(string raw)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return string.Empty;
		}
		raw = raw.Trim();
		if (TryGetSpriteTag(raw, out var spriteTag))
		{
			return spriteTag;
		}
		return ToDisplayName(raw);
	}

	private static string GetGamepadDisplayName(string raw)
	{
		string result;
		char c;
		switch (raw)
		{
		case "Pad_LT":
			return "LT";
		case "Pad_RT":
			return "RT";
		case "Pad_LStickPress":
			return "LS";
		case "Pad_RStickPress":
			return "RS";
		case "Pad_Back":
			return "Back";
		case "Pad_Menu":
			return "Menu";
		case "Pad_DPadUp":
			return "DPad Up";
		case "Pad_DPadDown":
			return "DPad Down";
		case "Pad_DPadLeft":
			return "DPad Left";
		case "Pad_DPadRight":
			return "DPad Right";
		case "Pad_LeftStickUp":
			return "Left Stick Up";
		case "Pad_LeftStickDown":
			return "Left Stick Down";
		case "Pad_LeftStickLeft":
			return "Left Stick Left";
		case "Pad_LeftStickRight":
			return "Left Stick Right";
		default:
			switch (raw.Length)
			{
			case 5:
				break;
			case 6:
				goto IL_030d;
			case 8:
				goto IL_032a;
			case 12:
				goto IL_0347;
			case 10:
				goto IL_0409;
			case 13:
				goto IL_0439;
			default:
				goto IL_04a8;
			}
			c = raw[4];
			if ((uint)c <= 66u)
			{
				if (c != 'A')
				{
					if (c == 'B' && raw == "Pad_B")
					{
						result = "B";
						break;
					}
				}
				else if (raw == "Pad_A")
				{
					result = "A";
					break;
				}
			}
			else if (c != 'X')
			{
				if (c == 'Y' && raw == "Pad_Y")
				{
					result = "Y";
					break;
				}
			}
			else if (raw == "Pad_X")
			{
				result = "X";
				break;
			}
			goto IL_04a8;
		case null:
			goto IL_04a8;
			IL_0439:
			if (!(raw == "Pad_DPadRight"))
			{
				goto IL_04a8;
			}
			result = "十字键右";
			break;
			IL_0409:
			if (!(raw == "Pad_DPadUp"))
			{
				goto IL_04a8;
			}
			result = "十字键上";
			break;
			IL_04a8:
			result = null;
			break;
			IL_0347:
			c = raw[8];
			if (c != 'D')
			{
				if (c == 'L' && raw == "Pad_DPadLeft")
				{
					result = "十字键左";
					break;
				}
			}
			else if (raw == "Pad_DPadDown")
			{
				result = "十字键下";
				break;
			}
			goto IL_04a8;
			IL_032a:
			c = raw[4];
			if (c != 'B')
			{
				if (c == 'M' && raw == "Pad_Menu")
				{
					result = "菜单键";
					break;
				}
			}
			else if (raw == "Pad_Back")
			{
				result = "返回键";
				break;
			}
			goto IL_04a8;
			IL_030d:
			c = raw[4];
			if (c != 'L')
			{
				if (c == 'R' && raw == "Pad_RB")
				{
					result = "RB";
					break;
				}
			}
			else if (raw == "Pad_LB")
			{
				result = "LB";
				break;
			}
			goto IL_04a8;
		}
		return result;
	}

	private static bool IsFunctionKey(string raw)
	{
		if (string.IsNullOrEmpty(raw) || raw.Length < 2 || raw[0] != 'F')
		{
			return false;
		}
		if (int.TryParse(raw.Substring(1), out var result) && result >= 1)
		{
			return result <= 24;
		}
		return false;
	}

	public static bool TryGetSpriteTag(string raw, out string spriteTag)
	{
		spriteTag = null;
		if (string.IsNullOrEmpty(raw))
		{
			return false;
		}
		raw = KeyNameUtil.NormalizeKeyName(raw);
		if (!TryGetSpriteName(raw, out var spriteName))
		{
			return false;
		}
		spriteTag = "<sprite name=\"" + spriteName + "\">";
		return true;
	}

	public static bool TryGetSpriteName(string raw, out string spriteName)
	{
		spriteName = null;
		if (string.IsNullOrEmpty(raw))
		{
			return false;
		}
		raw = KeyNameUtil.NormalizeKeyName(raw);
		raw = raw.Trim();
		if (raw != null)
		{
			switch (raw.Length)
			{
			case 5:
			{
				char c = raw[4];
				if ((uint)c <= 89u)
				{
					switch (c)
					{
					case 'A':
						if (!(raw == "Pad_A"))
						{
							break;
						}
						spriteName = "xa";
						return true;
					case 'B':
						if (!(raw == "Pad_B"))
						{
							break;
						}
						spriteName = "xb";
						return true;
					case 'X':
						if (!(raw == "Pad_X"))
						{
							break;
						}
						spriteName = "xx";
						return true;
					case 'Y':
						if (!(raw == "Pad_Y"))
						{
							break;
						}
						spriteName = "xy";
						return true;
					}
					break;
				}
				if ((uint)c <= 101u)
				{
					if (c != 'a')
					{
						if (c != 'e')
						{
							break;
						}
						switch (raw)
						{
						case "Space":
							spriteName = "space";
							return true;
						case "Quote":
							break;
						case "Pause":
							spriteName = "pmenu";
							return true;
						default:
							goto end_IL_002d;
						}
						goto IL_0dae;
					}
					if (!(raw == "Comma"))
					{
						break;
					}
					goto IL_0db7;
				}
				if (c != 'h')
				{
					if (c != 's')
					{
						if (c != 't' || !(raw == "Print"))
						{
							break;
						}
						goto IL_0ddb;
					}
					if (!(raw == "Minus"))
					{
						break;
					}
					goto IL_0d78;
				}
				if (!(raw == "Slash"))
				{
					break;
				}
				goto IL_0dc9;
			}
			case 6:
				switch (raw[5])
				{
				case 'B':
					break;
				case 'T':
					goto IL_058e;
				case '0':
					if (!(raw == "Mouse0"))
					{
						goto end_IL_002d;
					}
					spriteName = "ml";
					return true;
				case '1':
					if (!(raw == "Mouse1"))
					{
						goto end_IL_002d;
					}
					spriteName = "mr";
					return true;
				case '2':
					if (!(raw == "Mouse2"))
					{
						goto end_IL_002d;
					}
					spriteName = "mm";
					return true;
				case 'n':
					goto IL_05f2;
				case 'e':
					goto IL_0607;
				case 't':
					if (!(raw == "Insert"))
					{
						goto end_IL_002d;
					}
					spriteName = "ins";
					return true;
				case 'p':
					if (!(raw == "PageUp"))
					{
						goto end_IL_002d;
					}
					spriteName = "pageup";
					return true;
				case 's':
					goto IL_0656;
				case 'd':
					goto IL_066b;
				case 'q':
					goto IL_0680;
				default:
					goto end_IL_002d;
				}
				if (!(raw == "Pad_LB"))
				{
					if (!(raw == "Pad_RB"))
					{
						break;
					}
					spriteName = "rb";
					return true;
				}
				spriteName = "lb";
				return true;
			case 8:
			{
				char c = raw[4];
				if ((uint)c <= 76u)
				{
					switch (c)
					{
					case 'B':
						if (!(raw == "Pad_Back"))
						{
							break;
						}
						spriteName = "xwin";
						return true;
					case 'L':
						if (!(raw == "CapsLock"))
						{
							break;
						}
						spriteName = "capslock";
						return true;
					case 'D':
						if (!(raw == "PageDown"))
						{
							break;
						}
						spriteName = "pagedown";
						return true;
					}
					break;
				}
				if (c != 'M')
				{
					if (c != 'r')
					{
						if (c != 't' || !(raw == "RightAlt"))
						{
							break;
						}
						goto IL_0ce8;
					}
					if (!(raw == "Asterisk"))
					{
						break;
					}
					goto IL_0e1a;
				}
				if (!(raw == "Pad_Menu"))
				{
					if (!(raw == "LeftMeta"))
					{
						break;
					}
					goto IL_0df6;
				}
				spriteName = "xmenu";
				return true;
			}
			case 10:
			{
				char c = raw[6];
				if ((uint)c <= 97u)
				{
					switch (c)
					{
					case 'a':
						if (!(raw == "Pad_DPadUp"))
						{
							break;
						}
						spriteName = "du";
						return true;
					case 'P':
						if (!(raw == "KeypadPlus"))
						{
							break;
						}
						spriteName = "+";
						return true;
					}
					break;
				}
				if (c != 'h')
				{
					if (c != 'p')
					{
						if (c != 'r' || !(raw == "RightArrow"))
						{
							break;
						}
						spriteName = "kr";
						return true;
					}
					if (!(raw == "RightApple"))
					{
						break;
					}
					goto IL_0dff;
				}
				if (!(raw == "RightShift"))
				{
					break;
				}
				goto IL_0cd6;
			}
			case 12:
				switch (raw[8])
				{
				case 'D':
					if (!(raw == "Pad_DPadDown"))
					{
						goto end_IL_002d;
					}
					spriteName = "dd";
					return true;
				case 'L':
					if (!(raw == "Pad_DPadLeft"))
					{
						goto end_IL_002d;
					}
					spriteName = "dl";
					return true;
				case 't':
					break;
				case 'c':
					goto IL_07cb;
				case 'd':
					goto IL_07e0;
				case 'm':
					goto IL_07f5;
				case 'v':
					if (!(raw == "KeypadDivide"))
					{
						goto end_IL_002d;
					}
					spriteName = "slash";
					return true;
				case 'r':
					goto IL_081f;
				case 'u':
					if (!(raw == "KeypadEquals"))
					{
						goto end_IL_002d;
					}
					spriteName = "=";
					return true;
				default:
					goto end_IL_002d;
				}
				if (!(raw == "RightControl"))
				{
					break;
				}
				goto IL_0cdf;
			case 13:
			{
				char c = raw[0];
				if (c != 'K')
				{
					if (c != 'P' || !(raw == "Pad_DPadRight"))
					{
						break;
					}
					spriteName = "dr";
					return true;
				}
				if (!(raw == "KeypadDecimal"))
				{
					break;
				}
				goto IL_0e2c;
			}
			case 15:
			{
				char c = raw[4];
				if (c != 'L')
				{
					if (c != 'R' || !(raw == "Pad_RStickPress"))
					{
						break;
					}
					spriteName = "jrclick";
					return true;
				}
				if (!(raw == "Pad_LStickPress"))
				{
					if (!(raw == "Pad_LeftStickUp"))
					{
						break;
					}
					goto IL_0ca0;
				}
				spriteName = "jlclick";
				return true;
			}
			case 17:
			{
				char c = raw[13];
				if (c != 'D')
				{
					if (c != 'L' || !(raw == "Pad_LeftStickLeft"))
					{
						break;
					}
				}
				else if (!(raw == "Pad_LeftStickDown"))
				{
					break;
				}
				goto IL_0ca0;
			}
			case 18:
			{
				char c = raw[14];
				if (c != 'D')
				{
					if (c != 'L')
					{
						if (c != 'i' || !(raw == "Pad_LeftStickRight"))
						{
							break;
						}
						goto IL_0ca0;
					}
					if (!(raw == "Pad_RightStickLeft"))
					{
						break;
					}
				}
				else if (!(raw == "Pad_RightStickDown"))
				{
					break;
				}
				goto IL_0ca9;
			}
			case 9:
				switch (raw[5])
				{
				case 'h':
					break;
				case 'p':
					goto IL_0955;
				case 'r':
					goto IL_097a;
				case 'l':
					goto IL_099f;
				case 'o':
					goto IL_09b4;
				case 'u':
					goto IL_09c9;
				case 'M':
					goto IL_09de;
				default:
					goto end_IL_002d;
				}
				if (!(raw == "LeftShift"))
				{
					break;
				}
				goto IL_0cd6;
			case 11:
			{
				char c = raw[7];
				if ((uint)c <= 105u)
				{
					if (c != 'c')
					{
						if (c != 'd')
						{
							if (c != 'i' || !(raw == "KeypadMinus"))
							{
								break;
							}
							spriteName = "-";
							return true;
						}
						if (!(raw == "LeftWindows"))
						{
							break;
						}
						goto IL_0df6;
					}
					if (!(raw == "LeftBracket"))
					{
						break;
					}
					goto IL_0d8a;
				}
				if ((uint)c <= 110u)
				{
					if (c != 'm')
					{
						if (c != 'n' || !(raw == "KeypadEnter"))
						{
							break;
						}
						goto IL_0cf1;
					}
					if (!(raw == "LeftCommand"))
					{
						break;
					}
					goto IL_0dff;
				}
				if (c != 'r')
				{
					if (c != 't' || !(raw == "LeftControl"))
					{
						break;
					}
					goto IL_0cdf;
				}
				if (!(raw == "PrintScreen"))
				{
					break;
				}
				goto IL_0ddb;
			}
			case 7:
				switch (raw[6])
				{
				case 't':
					break;
				case 'w':
					if (!(raw == "UpArrow"))
					{
						goto end_IL_002d;
					}
					spriteName = "ku";
					return true;
				case 'k':
					if (!(raw == "Numlock") && !(raw == "NumLock"))
					{
						goto end_IL_002d;
					}
					spriteName = "numlock";
					return true;
				case '0':
					if (!(raw == "Keypad0"))
					{
						goto end_IL_002d;
					}
					spriteName = "0";
					return true;
				case '1':
					if (!(raw == "Keypad1"))
					{
						goto end_IL_002d;
					}
					spriteName = "1";
					return true;
				case '2':
					if (!(raw == "Keypad2"))
					{
						goto end_IL_002d;
					}
					spriteName = "2";
					return true;
				case '3':
					if (!(raw == "Keypad3"))
					{
						goto end_IL_002d;
					}
					spriteName = "3";
					return true;
				case '4':
					if (!(raw == "Keypad4"))
					{
						goto end_IL_002d;
					}
					spriteName = "4";
					return true;
				case '5':
					if (!(raw == "Keypad5"))
					{
						goto end_IL_002d;
					}
					spriteName = "5";
					return true;
				case '6':
					if (!(raw == "Keypad6"))
					{
						goto end_IL_002d;
					}
					spriteName = "6";
					return true;
				case '7':
					if (!(raw == "Keypad7"))
					{
						goto end_IL_002d;
					}
					spriteName = "7";
					return true;
				case '8':
					if (!(raw == "Keypad8"))
					{
						goto end_IL_002d;
					}
					spriteName = "8";
					return true;
				case '9':
					if (!(raw == "Keypad9"))
					{
						goto end_IL_002d;
					}
					spriteName = "9";
					return true;
				default:
					goto end_IL_002d;
				}
				if (!(raw == "LeftAlt"))
				{
					break;
				}
				goto IL_0ce8;
			case 3:
				switch (raw[0])
				{
				case 'T':
					if (!(raw == "Tab"))
					{
						break;
					}
					spriteName = "tab";
					return true;
				case 'E':
					if (!(raw == "End"))
					{
						break;
					}
					spriteName = "end";
					return true;
				}
				break;
			case 4:
			{
				char c = raw[0];
				if (c != 'H')
				{
					if (c != 'P' || !(raw == "Plus"))
					{
						break;
					}
					goto IL_0d81;
				}
				if (!(raw == "Home"))
				{
					break;
				}
				spriteName = "home";
				return true;
			}
			case 1:
				switch (raw[0])
				{
				case '-':
					break;
				case '+':
				case '=':
					goto IL_0d81;
				case '[':
					goto IL_0d8a;
				case ']':
					goto IL_0d93;
				case '\\':
					goto IL_0d9c;
				case ';':
					goto IL_0da5;
				case '\'':
					goto IL_0dae;
				case '<':
					goto IL_0db7;
				case '>':
					goto IL_0dc0;
				case '/':
					goto IL_0dc9;
				case '`':
				case '~':
					goto IL_0dd2;
				default:
					goto end_IL_002d;
				}
				goto IL_0d78;
			case 16:
				if (!(raw == "Pad_RightStickUp"))
				{
					break;
				}
				goto IL_0ca9;
			case 19:
				if (!(raw == "Pad_RightStickRight"))
				{
					break;
				}
				goto IL_0ca9;
			case 14:
				{
					if (!(raw == "KeypadMultiply"))
					{
						break;
					}
					goto IL_0e1a;
				}
				IL_0cd6:
				spriteName = "shift";
				return true;
				IL_0dae:
				spriteName = "quote";
				return true;
				IL_0cf1:
				spriteName = "enter";
				return true;
				IL_0d78:
				spriteName = "-";
				return true;
				IL_0ca0:
				spriteName = "jl";
				return true;
				IL_05f2:
				if (!(raw == "Return"))
				{
					break;
				}
				goto IL_0cf1;
				IL_0dc9:
				spriteName = "slash";
				return true;
				IL_058e:
				if (!(raw == "Pad_LT"))
				{
					if (!(raw == "Pad_RT"))
					{
						break;
					}
					spriteName = "rt";
					return true;
				}
				spriteName = "lt";
				return true;
				IL_0ce8:
				spriteName = "alt";
				return true;
				IL_0d8a:
				spriteName = "[";
				return true;
				IL_0db7:
				spriteName = "less";
				return true;
				IL_0e1a:
				spriteName = "asterisk";
				return true;
				IL_081f:
				if (!(raw == "KeypadPeriod"))
				{
					break;
				}
				goto IL_0e2c;
				IL_07f5:
				if (!(raw == "RightCommand"))
				{
					break;
				}
				goto IL_0dff;
				IL_0e2c:
				spriteName = "del";
				return true;
				IL_07e0:
				if (!(raw == "RightWindows"))
				{
					break;
				}
				goto IL_0df6;
				IL_0d93:
				spriteName = "]";
				return true;
				IL_07cb:
				if (!(raw == "RightBracket"))
				{
					break;
				}
				goto IL_0d93;
				IL_09de:
				if (!(raw == "RightMeta"))
				{
					break;
				}
				goto IL_0df6;
				IL_09c9:
				if (!(raw == "BackQuote"))
				{
					break;
				}
				goto IL_0dd2;
				IL_0dd2:
				spriteName = "~";
				return true;
				IL_09b4:
				if (!(raw == "Semicolon"))
				{
					break;
				}
				goto IL_0da5;
				IL_0da5:
				spriteName = ";";
				return true;
				IL_099f:
				if (!(raw == "Backslash"))
				{
					break;
				}
				goto IL_0d9c;
				IL_0d9c:
				spriteName = "slash";
				return true;
				IL_097a:
				if (!(raw == "DownArrow"))
				{
					if (!(raw == "LeftArrow"))
					{
						break;
					}
					spriteName = "kl";
					return true;
				}
				spriteName = "kd";
				return true;
				IL_0cdf:
				spriteName = "ctrl";
				return true;
				IL_0ddb:
				spriteName = "prtscrn";
				return true;
				IL_0955:
				if (!(raw == "Backspace"))
				{
					if (!(raw == "LeftApple"))
					{
						break;
					}
					goto IL_0dff;
				}
				spriteName = "backspace";
				return true;
				IL_0680:
				if (!(raw == "SysReq"))
				{
					break;
				}
				goto IL_0ddb;
				IL_0df6:
				spriteName = "win";
				return true;
				IL_0dc0:
				spriteName = "more";
				return true;
				IL_0607:
				if (!(raw == "Escape"))
				{
					if (!(raw == "Delete"))
					{
						break;
					}
					spriteName = "del";
					return true;
				}
				spriteName = "esc";
				return true;
				IL_0dff:
				spriteName = "mac";
				return true;
				IL_0656:
				if (!(raw == "Equals"))
				{
					break;
				}
				goto IL_0d81;
				IL_0d81:
				spriteName = "+";
				return true;
				IL_0ca9:
				spriteName = "jr";
				return true;
				IL_066b:
				if (!(raw == "Period"))
				{
					break;
				}
				goto IL_0dc0;
				end_IL_002d:
				break;
			}
		}
		if (raw.StartsWith("Alpha", StringComparison.OrdinalIgnoreCase) && int.TryParse(raw.Substring("Alpha".Length), out var result) && result >= 0 && result <= 9)
		{
			spriteName = result.ToString();
			return true;
		}
		if (raw.Length == 1)
		{
			char c2 = char.ToLowerInvariant(raw[0]);
			if (c2 >= 'a' && c2 <= 'z')
			{
				spriteName = c2.ToString();
				return true;
			}
		}
		if (raw.Length >= 2 && (raw[0] == 'F' || raw[0] == 'f') && int.TryParse(raw.Substring(1), out var result2) && result2 >= 1 && result2 <= 12)
		{
			spriteName = "f" + result2;
			return true;
		}
		return false;
	}
}
