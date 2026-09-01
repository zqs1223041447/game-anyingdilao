using System.Collections.Generic;

namespace Level.StateData.GlobalStates;

public class CompsGlobalState
{
	public string SkillIndexName;

	public readonly List<CompState> CompStates = new List<CompState>();
}
