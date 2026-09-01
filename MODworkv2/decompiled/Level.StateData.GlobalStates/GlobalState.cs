using System.Collections.Generic;
using Level.StateData.LevelStates;

namespace Level.StateData.GlobalStates;

public class GlobalState
{
	public readonly List<CompsGlobalState> CompsDataList = new List<CompsGlobalState>();

	public LevelState HomeLevelState = new LevelState
	{
		LevelId = "Home"
	};
}
