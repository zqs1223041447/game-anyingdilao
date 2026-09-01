namespace SK.Framework;

public interface ISwitchableObject
{
	SwitchState State { get; }

	void Switch();

	void Open();

	void Close();
}
