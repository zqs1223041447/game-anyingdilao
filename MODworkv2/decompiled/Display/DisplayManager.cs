using System;
using Data.AutoGen.DataClass.Settings;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Display;

public class DisplayManager : Singleton<DisplayManager>
{
	public ResolutionInfo CurrentResolutionInfo { get; private set; }

	public FullScreenMode FullScreenMode { get; private set; }

	public int TargetFrameRate { get; private set; }

	public event Action<ResolutionInfo, FullScreenMode> OnDisplayChanged;

	private DisplayManager()
	{
	}

	public void Apply(VideoSettingData data)
	{
		ResolutionInfo resolution = DisplayMappingUtil.GetResolution(data.resolution);
		FullScreenMode fullScreenMode = DisplayMappingUtil.GetFullScreenMode(data.fullScreenMode);
		bool num = CurrentResolutionInfo.width != resolution.width || CurrentResolutionInfo.height != resolution.height || CurrentResolutionInfo.refreshRate != resolution.refreshRate || FullScreenMode != fullScreenMode;
		Screen.SetResolution(resolution.width, resolution.height, fullScreenMode, resolution.refreshRate);
		CurrentResolutionInfo = resolution;
		FullScreenMode = fullScreenMode;
		ApplyFrameRate(data.frame);
		if (num)
		{
			this.OnDisplayChanged?.Invoke(CurrentResolutionInfo, FullScreenMode);
		}
	}

	private void ApplyFrameRate(int frame)
	{
		TargetFrameRate = frame;
		if (frame <= 0)
		{
			Application.targetFrameRate = -1;
		}
		else
		{
			Application.targetFrameRate = frame;
		}
	}
}
