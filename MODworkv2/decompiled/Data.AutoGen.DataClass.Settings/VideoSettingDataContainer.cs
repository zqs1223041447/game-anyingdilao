using System;
using System.Collections.Generic;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class VideoSettingDataContainer
{
	public List<VideoSettingData> items = new List<VideoSettingData>();
}
