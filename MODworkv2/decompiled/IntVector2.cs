using System;
using UnityEngine;

[Serializable]
public struct IntVector2 : IEquatable<IntVector2>
{
	public int x;

	public int y;

	public static IntVector2 One => new IntVector2(1, 1);

	public static IntVector2 OneNeg => new IntVector2(-1, -1);

	public static IntVector2 Zero => new IntVector2(0, 0);

	public static IntVector2 Up => new IntVector2(0, 1);

	public static IntVector2 Down => new IntVector2(0, -1);

	public static IntVector2 Left => new IntVector2(-1, 0);

	public static IntVector2 Right => new IntVector2(1, 0);

	public IntVector2(int num1, int num2)
	{
		x = num1;
		y = num2;
	}

	public IntVector2(float num1, float num2)
	{
		x = (int)num1;
		y = (int)num2;
	}

	public IntVector2(Vector2 a)
	{
		x = (int)a.x;
		y = (int)a.y;
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ")";
	}

	public static IntVector2 operator +(IntVector2 a, IntVector2 b)
	{
		return new IntVector2(a.x + b.x, a.y + b.y);
	}

	public static IntVector2 operator +(IntVector2 a, int b)
	{
		return new IntVector2(a.x + b, a.y + b);
	}

	public static IntVector2 operator -(IntVector2 a, IntVector2 b)
	{
		return new IntVector2(a.x - b.x, a.y - b.y);
	}

	public static IntVector2 operator -(IntVector2 a, int b)
	{
		return new IntVector2(a.x - b, a.y - b);
	}

	public static IntVector2 operator *(IntVector2 a, int b)
	{
		return new IntVector2(a.x * b, a.y * b);
	}

	public static IntVector2 operator /(IntVector2 a, int b)
	{
		return new IntVector2(a.x / b, a.y / b);
	}

	public static int Area(IntVector2 a)
	{
		return a.x * a.y;
	}

	public static float Slope(IntVector2 a)
	{
		return (float)a.y / (float)a.x;
	}

	public static void Swap(ref IntVector2 a)
	{
		ref int reference = ref a.x;
		ref int reference2 = ref a.y;
		int num = a.y;
		int num2 = a.x;
		reference = num;
		reference2 = num2;
	}

	public static Vector2 Vector2(IntVector2 a)
	{
		return new Vector2(a.x, a.y);
	}

	public static Vector3 Vector3(IntVector2 a)
	{
		return new Vector3(a.x, a.y);
	}

	public static bool operator ==(IntVector2 a, IntVector2 b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}

	public static bool operator !=(IntVector2 a, IntVector2 b)
	{
		return !(a == b);
	}

	public static bool OrGreater(IntVector2 a, IntVector2 b)
	{
		if (a.x > b.x || a.y > b.y)
		{
			return true;
		}
		return false;
	}

	public static bool OrGreater(IntVector2 a, int b)
	{
		if (a.x > b || a.y > b)
		{
			return true;
		}
		return false;
	}

	public static bool OrLesser(IntVector2 a, IntVector2 b)
	{
		if (a.x < b.x || a.y < b.y)
		{
			return true;
		}
		return false;
	}

	public static bool OrLesser(IntVector2 a, int b)
	{
		if (a.x < b || a.y < b)
		{
			return true;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is IntVector2 intVector)
		{
			if (x == intVector.x)
			{
				return y == intVector.y;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (x * 397) ^ y;
	}

	public bool Equals(IntVector2 other)
	{
		if (x == other.x)
		{
			return y == other.y;
		}
		return false;
	}
}
