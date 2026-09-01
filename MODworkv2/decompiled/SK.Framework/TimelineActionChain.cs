using UnityEngine;

namespace SK.Framework;

public class TimelineActionChain : AbstractActionChain
{
	public float CurrentTime { get; set; }

	public float Speed { get; set; } = 1f;


	public TimelineActionChain()
	{
	}

	public TimelineActionChain(MonoBehaviour executer)
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
			CurrentTime += Time.deltaTime * Speed;
			for (int i = 0; i < invokeList.Count; i++)
			{
				IAction action = invokeList[i];
				if (action is TimelineAction)
				{
					TimelineAction obj = action as TimelineAction;
					obj.currentTime = CurrentTime;
					obj.Invoke();
				}
			}
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
		CurrentTime = 0f;
		Speed = 1f;
	}
}
