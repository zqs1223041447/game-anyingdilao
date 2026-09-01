using System.Collections.Generic;

namespace PoedbMod;

public interface IModSkill
{
	string Id { get; }

	string Name { get; }

	string NameZh { get; }

	IReadOnlyList<string> Tags { get; }

	string SkillType { get; }

	string Description { get; }

	string DescriptionZh { get; }

	IReadOnlyDictionary<string, string> ColumnOverrides { get; }

	IReadOnlyList<string> SupportedTags { get; }

	IReadOnlyList<string> Restrictions { get; }
}
