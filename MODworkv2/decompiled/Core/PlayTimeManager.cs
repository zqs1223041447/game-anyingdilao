using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Core;

public class PlayTimeManager : Singleton<PlayTimeManager>
{
	private static double totalSeconds;

	private static bool running;

	private static double startRealtime;

	public static bool IsRunning => running;

	public static void ResetAndRun(double initialSeconds = 0.0)
	{
		totalSeconds = initialSeconds;
		StartCount();
	}

	public static void StartCount()
	{
		if (!running)
		{
			running = true;
			startRealtime = Time.realtimeSinceStartup;
		}
	}

	public static void StopCount()
	{
		if (running)
		{
			totalSeconds += (double)Time.realtimeSinceStartup - startRealtime;
			running = false;
		}
	}

	public static long GetTotalSeconds()
	{
		if (running)
		{
			return (long)(totalSeconds + ((double)Time.realtimeSinceStartup - startRealtime));
		}
		return (long)totalSeconds;
	}

	public static void Clear()
	{
		totalSeconds = 0.0;
		running = false;
		startRealtime = 0.0;
	}
}
