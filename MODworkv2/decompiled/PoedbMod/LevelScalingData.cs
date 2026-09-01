using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoedbMod;

[Serializable]
public class LevelScalingData
{
	[JsonProperty("levels")]
	public List<Dictionary<string, object>> Levels { get; set; } = new List<Dictionary<string, object>>();

}
