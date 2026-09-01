using System;

namespace SK.Framework;

public static class ArrayExtension
{
	public static T[] ForEach<T>(this T[] self, Action<int, T> action)
	{
		for (int i = 0; i < self.Length; i++)
		{
			action(i, self[i]);
		}
		return self;
	}

	public static T[] ForEachReverse<T>(this T[] self, Action<T> action)
	{
		for (int num = self.Length - 1; num >= 0; num--)
		{
			action(self[num]);
		}
		return self;
	}

	public static T[] ForEachReverse<T>(this T[] self, Action<int, T> action)
	{
		for (int num = self.Length - 1; num >= 0; num--)
		{
			action(num, self[num]);
		}
		return self;
	}

	public static T[] Merge<T>(this T[] self, T[] target)
	{
		T[] array = new T[self.Length + target.Length];
		for (int i = 0; i < self.Length; i++)
		{
			array[i] = self[i];
		}
		for (int j = 0; j < target.Length; j++)
		{
			array[j + self.Length] = target[j];
		}
		return array;
	}

	public static int[] SortInsertion(this int[] self)
	{
		for (int i = 1; i < self.Length; i++)
		{
			int num = self[i];
			int num2 = i;
			while (num2 > 0 && self[num2 - 1] > num)
			{
				self[num2] = self[num2 - 1];
				num2--;
			}
			self[num2] = num;
		}
		return self;
	}

	public static int[] SortShell(this int[] self)
	{
		int num;
		for (num = 1; num <= self.Length / 9; num = 3 * num + 1)
		{
		}
		while (num > 0)
		{
			for (int i = num + 1; i <= self.Length; i += num)
			{
				int num2 = self[i - 1];
				int num3 = i;
				while (num3 > num && self[num3 - num - 1] > num2)
				{
					self[num3 - 1] = self[num3 - num - 1];
					num3 -= num;
				}
				self[num3 - 1] = num2;
			}
			num /= 3;
		}
		return self;
	}

	public static int[] SortSelection(this int[] self)
	{
		for (int i = 0; i < self.Length - 1; i++)
		{
			int num = i;
			for (int j = i + 1; j < self.Length; j++)
			{
				if (self[j] < self[num])
				{
					num = j;
				}
			}
			int num2 = self[num];
			self[num] = self[i];
			self[i] = num2;
		}
		return self;
	}

	public static int[] SortBubble(this int[] self)
	{
		for (int i = 0; i < self.Length; i++)
		{
			for (int num = self.Length - 2; num >= i; num--)
			{
				if (self[num + 1] < self[num])
				{
					int num2 = self[num + 1];
					self[num + 1] = self[num];
					self[num] = num2;
				}
			}
		}
		return self;
	}
}
