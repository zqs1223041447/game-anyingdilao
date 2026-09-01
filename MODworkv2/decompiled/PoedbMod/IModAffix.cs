namespace PoedbMod;

public interface IModAffix
{
	string Id { get; }

	string Name { get; }

	int Level { get; }

	string PreSuf { get; }

	string Description { get; }

	string Weight { get; }
}
