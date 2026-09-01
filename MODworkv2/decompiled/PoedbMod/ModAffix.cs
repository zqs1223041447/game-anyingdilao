using System;
using Newtonsoft.Json;

namespace PoedbMod;

[Serializable]
public class ModAffix : IModAffix
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("level")]
	public int Level { get; set; }

	[JsonProperty("pre_suf")]
	public string PreSuf { get; set; }

	[JsonProperty("description")]
	public string Description { get; set; }

	[JsonProperty("weight")]
	public string Weight { get; set; }
}
