using Entity.InteractableObjects.Item;
using UnityEngine;

namespace Level.StateData.LevelStates;

public abstract class ItemLevelState
{
	public DropItemType DropItemType;

	public Vector3 Position;
}
