using System;
using UnityEngine;

namespace SK.Framework;

public static class AudioClipExtension
{
	public static byte[] ToPCM16Data(this AudioClip self)
	{
		float[] array = new float[self.samples * self.channels];
		self.GetData(array, 0);
		short[] array2 = new short[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			float num = array[i];
			array2[i] = (short)(num * 32767f);
		}
		byte[] array3 = new byte[array2.Length * 2];
		Buffer.BlockCopy(array2, 0, array3, 0, array3.Length);
		return array3;
	}
}
