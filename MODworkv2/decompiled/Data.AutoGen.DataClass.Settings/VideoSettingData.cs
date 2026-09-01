using System;
using Display;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class VideoSettingData
{
	public ResolutionPreset resolution;

	public ScreenMode fullScreenMode;

	public float global_light;

	public bool vsync;

	public bool bloom;

	public int frame;
}
