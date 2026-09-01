using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoedbMod;

[Serializable]
public class ModCrafting : IModCrafting
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("mod")]
	public string Mod { get; set; }

	[JsonProperty("require")]
	public string Require { get; set; }

	[JsonProperty("item_classes")]
	public List<string> ItemClasses { get; set; } = new List<string>();


	[JsonProperty("unlock")]
	public string Unlock { get; set; }

	[JsonProperty("source_url")]
	public string SourceUrl { get; set; }

	IReadOnlyList<string> IModCrafting.ItemClasses => ItemClasses ?? new List<string>();
}
