using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoedbMod;

[Serializable]
public class ModEquipment : IModEquipment
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("base_type")]
	public string BaseType { get; set; }

	[JsonProperty("rarity")]
	public string Rarity { get; set; }

	[JsonProperty("implicit_mods")]
	public List<string> ImplicitMods { get; set; } = new List<string>();


	[JsonProperty("explicit_mods")]
	public List<string> ExplicitMods { get; set; } = new List<string>();


	[JsonProperty("flavour_text")]
	public string FlavourText { get; set; }

	[JsonProperty("tags")]
	public List<string> Tags { get; set; } = new List<string>();


	[JsonProperty("source_url")]
	public string SourceUrl { get; set; }

	IReadOnlyList<string> IModEquipment.ImplicitMods => ImplicitMods ?? new List<string>();

	IReadOnlyList<string> IModEquipment.ExplicitMods => ExplicitMods ?? new List<string>();

	IReadOnlyList<string> IModEquipment.Tags => Tags ?? new List<string>();
}
