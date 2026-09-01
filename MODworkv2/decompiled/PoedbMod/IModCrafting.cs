using System.Collections.Generic;

namespace PoedbMod;

public interface IModCrafting
{
	string Id { get; }

	string Mod { get; }

	string Require { get; }

	IReadOnlyList<string> ItemClasses { get; }

	string Unlock { get; }
}
