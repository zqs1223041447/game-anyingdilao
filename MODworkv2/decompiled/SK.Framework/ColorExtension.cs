using System.Globalization;
using UnityEngine;

namespace SK.Framework;

public static class ColorExtension
{
	public static bool IsApproximatelyBlack(this Color self)
	{
		return self.r + self.g + self.b <= Mathf.Epsilon;
	}

	public static bool IsApproximatelyWhite(this Color self)
	{
		return self.r + self.g + self.b >= 1f - Mathf.Epsilon;
	}

	public static Color Invert(this Color self)
	{
		self.r = 1f - self.r;
		self.g = 1f - self.g;
		self.b = 1f - self.b;
		self.a = 1f - self.a;
		return self;
	}

	public static Color Alpha(this Color self, float alpha)
	{
		self.a = alpha;
		return self;
	}

	public static Color From255(this Color self, float r, float g, float b, float a = 255f)
	{
		self.r = r / 255f;
		self.g = g / 255f;
		self.b = b / 255f;
		self.a = a / 255f;
		return self;
	}

	public static Color FromHex(this Color self, string hexValue, float alpha = 1f)
	{
		if (string.IsNullOrEmpty(hexValue))
		{
			return Color.clear;
		}
		if (hexValue[0] == '#')
		{
			hexValue = hexValue.TrimStart('#');
		}
		if (hexValue.Length > 6)
		{
			hexValue = hexValue.Remove(6, hexValue.Length - 6);
		}
		int num = int.Parse(hexValue, NumberStyles.HexNumber);
		int num2 = (num >> 16) & 0xFF;
		int num3 = (num >> 8) & 0xFF;
		int num4 = num & 0xFF;
		float a = 255f * alpha;
		return self.From255(num2, num3, num4, a);
	}
}
