namespace SK.Framework;

public interface IAimableObject
{
	string Description { get; }

	float AimableDistance { get; }

	void Enter();

	void Exit();

	void Stay();
}
