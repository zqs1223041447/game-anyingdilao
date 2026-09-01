using System;

namespace SK.Framework;

public interface IActionChain
{
	bool IsPaused { get; }

	IActionChain Append(IAction action);

	IActionChain Begin();

	void Stop();

	void Pause();

	void Resume();

	IActionChain StopWhen(Func<bool> predicate);

	IActionChain OnStop(Action action);

	IActionChain SetLoops(int loops);
}
