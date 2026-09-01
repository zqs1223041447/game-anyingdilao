using System;
using UnityEngine;

namespace SK.Framework;

public class TimelineAction : AbstractAction
{
	protected float beginTime;

	protected float duration;

	protected Action<float> playAction;

	public float currentTime;

	protected override void OnInvoke()
	{
		float value = (currentTime - beginTime) / duration;
		value = Mathf.Clamp01(value);
		playAction(value);
	}

	public TimelineAction(float beginTime, float duration, Action<float> playAction)
	{
		this.beginTime = beginTime;
		this.duration = duration;
		this.playAction = playAction;
	}
}
