using UnityEngine;

namespace SK.Framework;

public class ConcurrentActionChain : AbstractActionChain
{
	public ConcurrentActionChain()
	{
	}

	public ConcurrentActionChain(MonoBehaviour executer)
		: base(executer)
	{
	}

	protected override void OnInvoke()
	{
		if (stopWhen != null && stopWhen())
		{
			isCompleted = true;
		}
		else if (!base.IsPaused)
		{
			for (int i = 0; i < invokeList.Count; i++)
			{
				if (invokeList[i].Invoke())
				{
					invokeList.RemoveAt(i);
					i--;
				}
			}
			isCompleted = invokeList.Count == 0;
		}
		if (isCompleted)
		{
			loops--;
			if (loops != 0)
			{
				Reset();
			}
			else
			{
				isCompleted = true;
			}
		}
	}

	protected override void OnReset()
	{
		base.IsPaused = false;
		for (int i = 0; i < cacheList.Count; i++)
		{
			cacheList[i].Reset();
			invokeList.Add(cacheList[i]);
		}
	}
}
