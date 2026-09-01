using System;
using UnityEngine;

namespace SK.Framework;

public sealed class Clock : ITimer
{
	private float beginTime;

	private float pausedTime;

	private readonly bool isIgnoreTimeScale;

	private readonly MonoBehaviour executer;

	private Action onLaunch;

	private Action<float> onExecute;

	private Action onPause;

	private Action onResume;

	private Action onStop;

	private Func<bool> stopWhen;

	public float ElapsedTime { get; private set; }

	public bool IsCompleted { get; private set; }

	public bool IsPaused { get; private set; }

	public Clock(bool isIgnoreTimeScale, MonoBehaviour executer)
	{
		this.isIgnoreTimeScale = isIgnoreTimeScale;
		this.executer = executer;
	}

	public Clock OnLaunch(Action onLaunch)
	{
		this.onLaunch = onLaunch;
		return this;
	}

	public Clock OnExecute(Action<float> onExecute)
	{
		this.onExecute = onExecute;
		return this;
	}

	public Clock OnPause(Action onPause)
	{
		this.onPause = onPause;
		return this;
	}

	public Clock OnResume(Action onResume)
	{
		this.onResume = onResume;
		return this;
	}

	public Clock OnStop(Action onStop)
	{
		this.onStop = onStop;
		return this;
	}

	public Clock StopWhen(Func<bool> predicate)
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
			ElapsedTime = (isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time) - beginTime;
			onExecute?.Invoke(ElapsedTime);
		}
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
