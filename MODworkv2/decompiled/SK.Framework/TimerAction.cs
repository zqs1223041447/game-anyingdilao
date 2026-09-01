using System;
using UnityEngine;

namespace SK.Framework;

public class TimerAction : AbstractAction
{
	private readonly float duration;

	private readonly bool isReverse;

	private float beginTime;

	private bool isBegan;

	private readonly Action<float> action;

	public TimerAction(float duration, bool isReverse, Action<float> action)
	{
		this.duration = duration;
		this.isReverse = isReverse;
		this.action = action;
	}

	protected override void OnInvoke()
	{
		if (!isBegan)
		{
			isBegan = true;
			beginTime = Time.time;
		}
		float num = Time.time - beginTime;
		action(Mathf.Clamp(isReverse ? (duration - num) : num, 0f, duration));
		isCompleted = num >= duration;
	}

	protected override void OnReset()
	{
		isBegan = false;
	}
}
