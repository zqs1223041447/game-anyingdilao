using System;

namespace SK.Framework;

public class TimeUtility
{
	public static float Convert2Seconds(float v, TimeUnit timeUnit)
	{
		return timeUnit switch
		{
			TimeUnit.Millsecond => v * 0.001f, 
			TimeUnit.Minute => v * 60f, 
			TimeUnit.Hour => v * 3600f, 
			TimeUnit.Day => v * 3600f * 24f, 
			_ => v, 
		};
	}

	public static double GetTimeStamp(DateTime dt)
	{
		return (dt - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
	}

	public static string ToStandardTimeFormat(float seconds)
	{
		int num = (int)seconds;
		int num2 = num / 3600;
		int num3 = num % 3600 / 60;
		num = num % 3600 % 60;
		return $"{num2:D2}:{num3:D2}:{num:D2}";
	}

	public static string ToMSTimeFormat(float seconds)
	{
		int num = (int)seconds;
		int num2 = num / 60;
		num %= 60;
		return $"{num2:D2}:{num:D2}";
	}

	public static string ToHMSFTimeFormat(float seconds)
	{
		int num = (int)(seconds * 1000f);
		int num2 = num / 3600000;
		int num3 = num % 3600000 / 60000;
		int num4 = num % 3600000 % 60000 / 1000;
		num = num % 3600000 % 60000 % 1000;
		return $"{num2:D2}:{num3:D2}:{num4:D2}:{num:D3}";
	}

	public static string ToMSFTimeFormat(float seconds)
	{
		int num = (int)(seconds * 1000f);
		int num2 = num % 3600000 / 60000;
		int num3 = num % 3600000 % 60000 / 1000;
		num = num % 3600000 % 60000 % 1000;
		return $"{num2:D2}:{num3:D2}:{num:D3}";
	}
}
