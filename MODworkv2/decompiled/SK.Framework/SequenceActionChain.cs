using UnityEngine;

namespace SK.Framework;

public class SequenceActionChain : AbstractActionChain
{
	public SequenceActionChain()
	{
	}

	public SequenceActionChain(MonoBehaviour executer)
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
			if (invokeList.Count > 0 && invokeList[0].Invoke())
			{
				invokeList.RemoveAt(0);
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
