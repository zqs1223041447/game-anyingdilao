using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoedbMod;

[Serializable]
public class ShadowDungeonMapping
{
	[JsonProperty("template_index_name")]
	public string TemplateIndexName { get; set; }

	[JsonProperty("index_name")]
	public string IndexName { get; set; }

	[JsonProperty("info_key")]
	public string InfoKey { get; set; }

	[JsonProperty("column_overrides")]
	public Dictionary<string, string> ColumnOverrides { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);

}
