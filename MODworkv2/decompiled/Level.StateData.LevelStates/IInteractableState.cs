namespace Level.StateData.LevelStates;

public interface IInteractableState
{
	void FlushToState();

	void RestoreState();
}
