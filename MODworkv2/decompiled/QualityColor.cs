using System.Collections.Generic;
using UnityEngine;

public static class QualityColor
{
	private const byte SlotColorAlpha = 40;

	private const byte NewItemSlotColorAlpha = 100;

	public static Dictionary<int, string> Colors { get; } = new Dictionary<int, string>
	{
		{ 0, "#ffffffff" },
		{ 1, "#53FF6B" },
		{ 2, "#37C5FF" },
		{ 3, "#B63EFF" },
		{ 4, "#FF50B5" },
		{ 5, "#FF7200" },
		{ 6, "#FFCA00" },
		{ 7, "#FFCEE4" },
		{ 8, "#E5CCAB" }
	};


	public static Dictionary<int, Color32> SlotColors { get; } = new Dictionary<int, Color32>
	{
		{
			0,
			new Color32(160, 160, 160, 40)
		},
		{
			1,
			new Color32(83, byte.MaxValue, 107, 40)
		},
		{
			2,
			new Color32(55, 197, byte.MaxValue, 40)
		},
		{
			3,
			new Color32(182, 62, byte.MaxValue, 40)
		},
		{
			4,
			new Color32(byte.MaxValue, 80, 181, 40)
		},
		{
			5,
			new Color32(byte.MaxValue, 114, 0, 40)
		},
		{
			6,
			new Color32(byte.MaxValue, 202, 0, 40)
		}
	};


	public static Color32 NewItemSlotColor => new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 100);

	public static Color32 GetSlotColor(int quality)
	{
		if (!SlotColors.TryGetValue(quality, out var value))
		{
			return SlotColors[0];
		}
		return value;
	}
}
