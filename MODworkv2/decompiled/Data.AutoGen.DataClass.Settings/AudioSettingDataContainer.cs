using System;
using System.Collections.Generic;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class AudioSettingDataContainer
{
	public List<AudioSettingData> items = new List<AudioSettingData>();
}
