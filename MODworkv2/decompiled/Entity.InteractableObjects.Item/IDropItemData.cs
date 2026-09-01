namespace Entity.InteractableObjects.Item;

public interface IDropItemData
{
	int ItemType { get; }

	string GetTitle(bool display = true);

	float GetNameSize();
}
