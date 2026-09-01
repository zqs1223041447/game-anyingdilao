using System;
using UnityEngine;

namespace Display;

public static class DisplayMappingUtil
{
	private const float RatioTolerance = 0.04f;

	public static ResolutionPreset FromResolution(int w, int h)
	{
		if (w <= 0 || h <= 0)
		{
			return ResolutionPreset.R1280x800;
		}
		float num = (float)w / (float)h;
		ResolutionPreset result = ResolutionPreset.R1280x800;
		float num2 = float.MaxValue;
		foreach (ResolutionPreset value in Enum.GetValues(typeof(ResolutionPreset)))
		{
			ResolutionInfo resolution = GetResolution(value);
			float num3 = ((Mathf.Abs((float)resolution.width / (float)resolution.height - num) > 0.04f) ? 100000f : 0f);
			int num4 = w * h;
			float num5 = Mathf.Abs(resolution.width * resolution.height - num4);
			float num6 = num3 + num5;
			if (num6 < num2)
			{
				num2 = num6;
				result = value;
			}
		}
		return result;
	}

	public static ResolutionPreset GetDefaultPresetWithOneStepBoost(ResolutionPreset detectedPreset)
	{
		return detectedPreset switch
		{
			ResolutionPreset.R1280x720 => ResolutionPreset.R1366x768, 
			ResolutionPreset.R1366x768 => ResolutionPreset.R1600x900, 
			ResolutionPreset.R1600x900 => ResolutionPreset.R1920x1080, 
			ResolutionPreset.R1280x800 => ResolutionPreset.R1920x1200, 
			ResolutionPreset.R1440x900 => ResolutionPreset.R1680x1050, 
			ResolutionPreset.R1680x1050 => ResolutionPreset.R1920x1200, 
			ResolutionPreset.R2560x1080 => ResolutionPreset.R3440x1440, 
			_ => detectedPreset, 
		};
	}

	public static ResolutionPreset GetDefaultPresetByResolution(int w, int h)
	{
		return GetDefaultPresetWithOneStepBoost(FromResolution(w, h));
	}

	public static bool IsCurrentUltraWide()
	{
		return IsUltraWide(Screen.width, Screen.height);
	}

	public static bool IsUltraWide(int width, int height)
	{
		if (width <= 0 || height <= 0)
		{
			return false;
		}
		return (float)width / (float)height >= 2f;
	}

	public static FullScreenMode GetFullScreenMode(ScreenMode mode)
	{
		return mode switch
		{
			ScreenMode.FullScreenWindow => FullScreenMode.FullScreenWindow, 
			ScreenMode.MaximizedWindow => FullScreenMode.MaximizedWindow, 
			ScreenMode.Windowed => FullScreenMode.Windowed, 
			_ => FullScreenMode.FullScreenWindow, 
		};
	}

	public static ResolutionInfo GetResolution(ResolutionPreset preset)
	{
		int num = Screen.currentResolution.refreshRate;
		if (num <= 0)
		{
			num = 60;
		}
		switch (preset)
		{
		case ResolutionPreset.R3840x2160:
		{
			ResolutionInfo result21 = default(ResolutionInfo);
			result21.width = 3840;
			result21.height = 2160;
			result21.refreshRate = num;
			return result21;
		}
		case ResolutionPreset.R2560x1440:
		{
			ResolutionInfo result20 = default(ResolutionInfo);
			result20.width = 2560;
			result20.height = 1440;
			result20.refreshRate = num;
			return result20;
		}
		case ResolutionPreset.R1920x1080:
		{
			ResolutionInfo result19 = default(ResolutionInfo);
			result19.width = 1920;
			result19.height = 1080;
			result19.refreshRate = num;
			return result19;
		}
		case ResolutionPreset.R1600x900:
		{
			ResolutionInfo result18 = default(ResolutionInfo);
			result18.width = 1600;
			result18.height = 900;
			result18.refreshRate = num;
			return result18;
		}
		case ResolutionPreset.R1366x768:
		{
			ResolutionInfo result17 = default(ResolutionInfo);
			result17.width = 1366;
			result17.height = 768;
			result17.refreshRate = num;
			return result17;
		}
		case ResolutionPreset.R1280x720:
		{
			ResolutionInfo result16 = default(ResolutionInfo);
			result16.width = 1280;
			result16.height = 720;
			result16.refreshRate = num;
			return result16;
		}
		case ResolutionPreset.R2560x1600:
		{
			ResolutionInfo result15 = default(ResolutionInfo);
			result15.width = 2560;
			result15.height = 1600;
			result15.refreshRate = num;
			return result15;
		}
		case ResolutionPreset.R1920x1200:
		{
			ResolutionInfo result14 = default(ResolutionInfo);
			result14.width = 1920;
			result14.height = 1200;
			result14.refreshRate = num;
			return result14;
		}
		case ResolutionPreset.R1680x1050:
		{
			ResolutionInfo result13 = default(ResolutionInfo);
			result13.width = 1680;
			result13.height = 1050;
			result13.refreshRate = num;
			return result13;
		}
		case ResolutionPreset.R1440x900:
		{
			ResolutionInfo result12 = default(ResolutionInfo);
			result12.width = 1440;
			result12.height = 900;
			result12.refreshRate = num;
			return result12;
		}
		case ResolutionPreset.R1280x800:
		{
			ResolutionInfo result11 = default(ResolutionInfo);
			result11.width = 1280;
			result11.height = 800;
			result11.refreshRate = num;
			return result11;
		}
		case ResolutionPreset.R5120x2160:
		{
			ResolutionInfo result10 = default(ResolutionInfo);
			result10.width = 5120;
			result10.height = 2160;
			result10.refreshRate = num;
			return result10;
		}
		case ResolutionPreset.R3840x1600:
		{
			ResolutionInfo result9 = default(ResolutionInfo);
			result9.width = 3840;
			result9.height = 1600;
			result9.refreshRate = num;
			return result9;
		}
		case ResolutionPreset.R3440x1440:
		{
			ResolutionInfo result8 = default(ResolutionInfo);
			result8.width = 3440;
			result8.height = 1440;
			result8.refreshRate = num;
			return result8;
		}
		case ResolutionPreset.R2560x1080:
		{
			ResolutionInfo result7 = default(ResolutionInfo);
			result7.width = 2560;
			result7.height = 1080;
			result7.refreshRate = num;
			return result7;
		}
		case ResolutionPreset.R7680x2160:
		{
			ResolutionInfo result6 = default(ResolutionInfo);
			result6.width = 7680;
			result6.height = 2160;
			result6.refreshRate = num;
			return result6;
		}
		case ResolutionPreset.R5120x1440:
		{
			ResolutionInfo result5 = default(ResolutionInfo);
			result5.width = 5120;
			result5.height = 1440;
			result5.refreshRate = num;
			return result5;
		}
		case ResolutionPreset.R3840x1080:
		{
			ResolutionInfo result4 = default(ResolutionInfo);
			result4.width = 3840;
			result4.height = 1080;
			result4.refreshRate = num;
			return result4;
		}
		case ResolutionPreset.R5120x1600:
		{
			ResolutionInfo result3 = default(ResolutionInfo);
			result3.width = 5120;
			result3.height = 1600;
			result3.refreshRate = num;
			return result3;
		}
		case ResolutionPreset.R3840x1200:
		{
			ResolutionInfo result2 = default(ResolutionInfo);
			result2.width = 3840;
			result2.height = 1200;
			result2.refreshRate = num;
			return result2;
		}
		default:
		{
			ResolutionInfo result = default(ResolutionInfo);
			result.width = 1280;
			result.height = 800;
			result.refreshRate = num;
			return result;
		}
		}
	}
}
