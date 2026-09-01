using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoedbMod;

[Serializable]
public class ModSkill : IModSkill
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("name_zh")]
	public string NameZh { get; set; }

	[JsonProperty("tags")]
	public List<string> Tags { get; set; } = new List<string>();


	[JsonProperty("skill_type")]
	public string SkillType { get; set; } = "active";


	[JsonProperty("description")]
	public string Description { get; set; }

	[JsonProperty("description_zh")]
	public string DescriptionZh { get; set; }

	[JsonProperty("support_type")]
	public string SupportType { get; set; }

	[JsonProperty("supported_tags")]
	public List<string> SupportedTags { get; set; } = new List<string>();


	[JsonProperty("restrictions")]
	public List<string> Restrictions { get; set; } = new List<string>();


	[JsonProperty("cost_multiplier")]
	public double? CostMultiplier { get; set; }

	[JsonProperty("level_scaling")]
	public LevelScalingData LevelScaling { get; set; }

	[JsonProperty("source_url")]
	public string SourceUrl { get; set; }

	[JsonProperty("shadow_dungeon_mapping")]
	public ShadowDungeonMapping Mapping { get; set; }

	IReadOnlyList<string> IModSkill.Tags => Tags ?? new List<string>();

	IReadOnlyDictionary<string, string> IModSkill.ColumnOverrides => Mapping?.ColumnOverrides ?? new Dictionary<string, string>();

	IReadOnlyList<string> IModSkill.SupportedTags => SupportedTags ?? new List<string>();

	IReadOnlyList<string> IModSkill.Restrictions => Restrictions ?? new List<string>();
}
