namespace SK.Framework;

public interface ITimer
{
	bool IsCompleted { get; }

	bool IsPaused { get; }

	void Start();

	void Pause();

	void Resume();

	void Stop();

	bool Execute();
}
