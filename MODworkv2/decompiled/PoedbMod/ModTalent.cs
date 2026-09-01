using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoedbMod;

[Serializable]
public class ModTalent : IModTalent
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("type")]
	public string Type { get; set; } = "normal";


	[JsonProperty("stats")]
	public List<string> Stats { get; set; } = new List<string>();


	[JsonProperty("is_jewel_socket")]
	public bool IsJewelSocket { get; set; }

	[JsonProperty("jewel_radius")]
	public int? JewelRadius { get; set; }

	[JsonProperty("connected_to")]
	public List<int> ConnectedTo { get; set; } = new List<int>();


	IReadOnlyList<string> IModTalent.Stats => Stats ?? new List<string>();

	IReadOnlyList<int> IModTalent.ConnectedTo => ConnectedTo ?? new List<int>();
}
