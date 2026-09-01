namespace Display;

public static class DisplayLabelUtil
{
	public static string Resolution(ResolutionPreset preset)
	{
		ResolutionInfo resolution = DisplayMappingUtil.GetResolution(preset);
		return resolution.width + " × " + resolution.height;
	}

	public static string Frame(int frame)
	{
		if (frame > 0)
		{
			return frame + " FPS";
		}
		return LOC.MM.GetStart("setting_unlimited");
	}

	public static string GetScreenModeName(ScreenMode mode)
	{
		return mode switch
		{
			ScreenMode.FullScreenWindow => LOC.MM.GetStart("setting_fullwin"), 
			ScreenMode.MaximizedWindow => LOC.MM.GetStart("setting_maxwin"), 
			ScreenMode.Windowed => LOC.MM.GetStart("setting_wined"), 
			_ => mode.ToString(), 
		};
	}
}
