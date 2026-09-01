using System.Collections.Generic;

namespace PoedbMod;

public interface IModEquipment
{
	string Id { get; }

	string Name { get; }

	string BaseType { get; }

	string Rarity { get; }

	IReadOnlyList<string> ImplicitMods { get; }

	IReadOnlyList<string> ExplicitMods { get; }

	string FlavourText { get; }

	IReadOnlyList<string> Tags { get; }
}
