using System;
using System.Collections.Generic;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class InterfaceSettingDataContainer
{
	public List<InterfaceSettingData> items = new List<InterfaceSettingData>();
}
