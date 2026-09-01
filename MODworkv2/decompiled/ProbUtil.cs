using System;
using System.Collections.Generic;
using UnityEngine;

public static class ProbUtil
{
	public static bool Chance(float probability)
	{
		probability = Mathf.Clamp01(probability);
		return UnityEngine.Random.value < probability;
	}

	public static bool ChancePercent(float percent)
	{
		percent = Mathf.Clamp(percent, 0f, 100f);
		return UnityEngine.Random.Range(0f, 100f) < percent;
	}

	public static int Roll(params int[] weights)
	{
		int num = 0;
		for (int i = 0; i < weights.Length; i++)
		{
			num += weights[i];
		}
		if (num <= 0)
		{
			return 0;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		int num3 = 0;
		for (int j = 0; j < weights.Length; j++)
		{
			num3 += weights[j];
			if (num2 < num3)
			{
				return j;
			}
		}
		return weights.Length - 1;
	}

	public static int RollNormalized(params float[] weights)
	{
		float num = 0f;
		for (int i = 0; i < weights.Length; i++)
		{
			num += weights[i];
		}
		if (num <= 0f)
		{
			return 0;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		for (int j = 0; j < weights.Length; j++)
		{
			num3 += weights[j];
			if (num2 < num3)
			{
				return j;
			}
		}
		return weights.Length - 1;
	}

	public static T Roll<T>(IList<T> items, IList<int> weights)
	{
		if (items == null || weights == null || items.Count == 0 || items.Count != weights.Count)
		{
			return default(T);
		}
		int num = 0;
		for (int i = 0; i < weights.Count; i++)
		{
			num += weights[i];
		}
		if (num <= 0)
		{
			return items[0];
		}
		int num2 = UnityEngine.Random.Range(0, num);
		int num3 = 0;
		for (int j = 0; j < items.Count; j++)
		{
			num3 += weights[j];
			if (num2 < num3)
			{
				return items[j];
			}
		}
		return items[items.Count - 1];
	}

	public static T Pick<T>(T[] array)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	public static T Pick<T>(List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T PickAndRemove<T>(List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		T result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public static int RangeExcept(int min, int max, int except)
	{
		if (max - min <= 1)
		{
			return min;
		}
		int num;
		do
		{
			num = UnityEngine.Random.Range(min, max);
		}
		while (num == except);
		return num;
	}

	public static T RandomEnum<T>() where T : Enum
	{
		T[] array = (T[])Enum.GetValues(typeof(T));
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	public static bool Bool()
	{
		return UnityEngine.Random.value > 0.5f;
	}

	public static Vector2 RandomDirection2D()
	{
		float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
	}

	public static Vector3 RandomDirection3D()
	{
		return UnityEngine.Random.onUnitSphere;
	}

	public static Vector2 RandomPointInCircle(float radius)
	{
		return UnityEngine.Random.insideUnitCircle * radius;
	}

	public static Vector3 RandomPointInSphere(float radius)
	{
		return UnityEngine.Random.insideUnitSphere * radius;
	}

	public static void Shuffle<T>(IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int num2 = UnityEngine.Random.Range(0, num + 1);
			int index = num;
			int index2 = num2;
			T val = list[num2];
			T val2 = list[num];
			T val4 = (list[index] = val);
			val4 = (list[index2] = val2);
		}
	}
}
