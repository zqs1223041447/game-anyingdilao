using System;
using System.Collections.Generic;

namespace SK.Framework;

public static class QueueExtension
{
	public static Queue<T> ForEach<T>(this Queue<T> self, Action<T> action)
	{
		foreach (T item in self)
		{
			action(item);
		}
		return self;
	}

	public static Queue<T> Merge<T>(this Queue<T> self, Queue<T> target)
	{
		int count = target.Count;
		for (int i = 0; i < count; i++)
		{
			self.Enqueue(target.Dequeue());
		}
		return self;
	}

	public static Queue<T> Copy<T>(this Queue<T> self)
	{
		Queue<T> queue = new Queue<T>(self.Count);
		foreach (T item in self)
		{
			queue.Enqueue(item);
		}
		return queue;
	}
}
