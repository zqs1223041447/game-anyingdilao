using System;

namespace SK.Framework;

public abstract class AbstractAction : IAction
{
	protected bool isCompleted;

	protected Action onCompleted;

	public bool Invoke()
	{
		if (!isCompleted)
		{
			OnInvoke();
		}
		if (isCompleted)
		{
			onCompleted?.Invoke();
		}
		return isCompleted;
	}

	public void Reset()
	{
		isCompleted = false;
		OnReset();
	}

	protected abstract void OnInvoke();

	protected virtual void OnReset()
	{
	}
}
