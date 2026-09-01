using System;
using System.Collections.Generic;

namespace SK.Framework;

public static class StackExtension
{
	public static Stack<T> ForEach<T>(this Stack<T> self, Action<T> action)
	{
		foreach (T item in self)
		{
			action(item);
		}
		return self;
	}
}
