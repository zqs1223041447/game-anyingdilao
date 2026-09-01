using System;
using System.Runtime.InteropServices;

namespace SK.Framework;

public static class StructExtension
{
	public static byte[] ToBytes<T>(this T self) where T : struct
	{
		int num = Marshal.SizeOf(self);
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.StructureToPtr(self, intPtr, fDeleteOld: false);
			byte[] array = new byte[num];
			Marshal.Copy(intPtr, array, 0, num);
			return array;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static object ToStruct<T>(this byte[] self) where T : struct
	{
		int num = Marshal.SizeOf(typeof(T));
		if (num > self.Length)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(self, 0, intPtr, num);
		object result = Marshal.PtrToStructure(intPtr, typeof(T));
		Marshal.FreeHGlobal(intPtr);
		return result;
	}
}
