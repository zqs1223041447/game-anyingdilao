using System;

namespace SK.Framework;

public static class DateTimeExtension
{
	public static double GetTimeStamp(this DateTime self)
	{
		return (self - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
	}

	public static string ToChinese(this DayOfWeek self, string prefix)
	{
		return self switch
		{
			DayOfWeek.Monday => prefix + "一", 
			DayOfWeek.Tuesday => prefix + "二", 
			DayOfWeek.Wednesday => prefix + "三", 
			DayOfWeek.Thursday => prefix + "四", 
			DayOfWeek.Friday => prefix + "五", 
			DayOfWeek.Saturday => prefix + "六", 
			DayOfWeek.Sunday => prefix + "日", 
			_ => null, 
		};
	}
}
