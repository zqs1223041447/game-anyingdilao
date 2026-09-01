using System;
using System.Collections.Generic;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class FilterDataContainer
{
	public List<FilterData> items = new List<FilterData>();
}
