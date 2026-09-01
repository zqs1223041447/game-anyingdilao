using System;
using System.Collections.Generic;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class GameSettingDataContainer
{
	public List<GameSettingData> items = new List<GameSettingData>();
}
