using System.Runtime.InteropServices;
using UnityEngine;

namespace Container.Inventory;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SlotColor
{
	public static Color32 Green => new Color32(0, byte.MaxValue, 0, 15);

	public static Color32 Yellow => new Color32(byte.MaxValue, byte.MaxValue, 0, 20);

	public static Color32 Red => new Color32(byte.MaxValue, 0, 0, 15);

	public static Color32 TouMing => new Color32(0, 0, 0, 0);
}
