using System;

namespace SK.Framework;

public static class ClassExtension
{
	public static bool Execute<T>(this T self, Action<T> action) where T : class
	{
		if (self != null)
		{
			action(self);
			return true;
		}
		return false;
	}
}
