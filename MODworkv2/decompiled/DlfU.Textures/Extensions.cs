using System;
using UnityEngine;

namespace DlfU.Textures;

public static class Extensions
{
	public static string PNGImageEncodeBase64(this Texture2D texture2D)
	{
		return Convert.ToBase64String(texture2D.EncodeToPNG());
	}

	public static Texture2D PNGImageDecodeBase64(this string base64)
	{
		return Convert.FromBase64String(base64).PNGImageDecode();
	}

	public static Texture2D PNGImageDecode(this byte[] bytes)
	{
		Texture2D obj = new Texture2D(0, 0, TextureFormat.RGBA32, mipChain: false)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		obj.LoadImage(bytes);
		obj.Apply();
		return obj;
	}
}
