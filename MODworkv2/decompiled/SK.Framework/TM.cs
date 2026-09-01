using System;
using UnityEngine;

namespace SK.Framework;

public sealed class TM : ITimer
{
	private float beginTime;

	private readonly float duration;

	private float pausedTime;

	private readonly bool isIgnoreTimeScale;

	private readonly MonoBehaviour executer;

	private Action onLaunch;

	private Action<float> onExecute;

	private Action onPause;

	private Action onResume;

	private Action onStop;

	private Func<bool> stopWhen;

	public float RemainingTime { get; private set; }

	public bool IsCompleted { get; private set; }

	public bool IsPaused { get; private set; }

	public TM(float duration, bool isIgnoreTimeScale = false, MonoBehaviour executer = null)
	{
		this.duration = duration;
		this.isIgnoreTimeScale = isIgnoreTimeScale;
		this.executer = executer;
	}

	public TM OnLaunch(Action onLaunch)
	{
		this.onLaunch = onLaunch;
		return this;
	}

	public TM OnExecute(Action<float> onExecute)
	{
		this.onExecute = onExecute;
		return this;
	}

	public TM OnPause(Action onPause)
	{
		this.onPause = onPause;
		return this;
	}

	public TM OnResume(Action onResume)
	{
		this.onResume = onResume;
		return this;
	}

	public TM OnStop(Action onStop)
	{
		this.onStop = onStop;
		return this;
	}

	public TM StopWhen(Func<bool> predicate)
	{
		stopWhen = predicate;
		return this;
	}

	public void Start()
	{
		beginTime = (isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time);
		onLaunch?.Invoke();
		this.Begin((executer != null) ? executer : Timer.Instance);
	}

	public void Pause()
	{
		IsPaused = true;
		pausedTime = (isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time);
		onPause?.Invoke();
	}

	public void Resume()
	{
		IsPaused = false;
		beginTime += (isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time) - pausedTime;
		onResume?.Invoke();
	}

	public void Stop()
	{
		IsCompleted = true;
	}

	public bool Execute()
	{
		if (!IsCompleted && !IsPaused)
		{
			RemainingTime = duration - ((isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time) - beginTime);
			RemainingTime = Mathf.Clamp(RemainingTime, 0f, duration);
			onExecute?.Invoke(RemainingTime);
		}
		IsCompleted = RemainingTime <= 0f;
		if (!IsCompleted && stopWhen != null && stopWhen())
		{
			IsCompleted = true;
		}
		if (IsCompleted)
		{
			onStop?.Invoke();
		}
		return IsCompleted;
	}
}
