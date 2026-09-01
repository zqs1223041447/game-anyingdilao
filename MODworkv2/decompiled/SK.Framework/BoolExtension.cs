using System;

namespace SK.Framework;

public static class BoolExtension
{
	public static bool Execute(this bool self, Action action)
	{
		if (self)
		{
			action();
		}
		return self;
	}

	public static bool Execute(this bool self, Action<bool> action)
	{
		action(self);
		return self;
	}

	public static bool Execute(this bool self, Action actionIfTrue, Action actionIfFalse)
	{
		if (self)
		{
			actionIfTrue();
		}
		else
		{
			actionIfFalse();
		}
		return self;
	}
}
