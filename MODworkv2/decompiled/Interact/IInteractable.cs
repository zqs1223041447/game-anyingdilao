namespace Interact;

public interface IInteractable
{
	InteractionType Type { get; }

	int Priority { get; }

	bool CanHover();

	void OnHoverEnter();

	void OnHoverExit();

	void OnLeftClick();

	void OnRightClick();

	bool CanInteract();

	void Interact();
}
