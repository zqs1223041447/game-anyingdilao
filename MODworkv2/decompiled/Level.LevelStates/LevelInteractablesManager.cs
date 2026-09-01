using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Level.StateData.LevelStates;

namespace Level.LevelStates;

public class LevelInteractablesManager : SingletonMonoScene<LevelInteractablesManager>
{
	private readonly HashSet<IInteractableState> interactables = new HashSet<IInteractableState>();

	protected override void Awake()
	{
		base.Awake();
		ClearAllRefs();
	}

	public void Register(IInteractableState i)
	{
		if (i != null)
		{
			interactables.Add(i);
		}
	}

	public void Unregister(IInteractableState i)
	{
		if (i != null)
		{
			interactables.Remove(i);
		}
	}

	public void ClearAllRefs()
	{
		interactables.Clear();
	}

	public void RestoreAll()
	{
		foreach (IInteractableState interactable in interactables)
		{
			interactable?.RestoreState();
		}
	}

	public void FlushAll()
	{
		foreach (IInteractableState interactable in interactables)
		{
			interactable?.FlushToState();
		}
	}
}
