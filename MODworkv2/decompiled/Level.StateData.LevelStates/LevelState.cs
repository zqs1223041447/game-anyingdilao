using System.Collections.Generic;

namespace Level.StateData.LevelStates;

public class LevelState
{
	public string LevelId;

	public Dictionary<string, EnemyPointLevelState> EnemyPoints;

	public List<ItemLevelState> ItemLevelStates;

	public Dictionary<string, InteractableLevelState> Interactables;

	public Dictionary<string, ChestPointLevelState> ChestPoints;
}
