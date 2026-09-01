using System.Collections.Generic;

namespace Level.StateData.LevelStates;

public class ChestPointLevelState
{
	public string RuntimeId;

	public readonly List<ChestSpawnInfo> Chests = new List<ChestSpawnInfo>();
}
