namespace Level.StateData.LevelStates;

public class InteractableLevelState
{
	public bool IsOpened;

	public InteractableType InteractableType;

	public InteractableLevelState(InteractableType type)
	{
		InteractableType = type;
	}
}
