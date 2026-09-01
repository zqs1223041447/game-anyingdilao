using System.Collections.Generic;

namespace PoedbMod;

public interface IModTalent
{
	string Id { get; }

	string Name { get; }

	string Type { get; }

	IReadOnlyList<string> Stats { get; }

	bool IsJewelSocket { get; }

	int? JewelRadius { get; }

	IReadOnlyList<int> ConnectedTo { get; }
}
