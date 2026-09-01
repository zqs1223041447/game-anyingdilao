using UnityEngine;

namespace SK.Framework;

public static class CameraExtension
{
	public static Texture2D Capture(this Camera self, int width, int height)
	{
		Rect source = new Rect(0f, 0f, width, height);
		RenderTexture targetTexture = self.targetTexture;
		RenderTexture renderTexture2 = (self.targetTexture = new RenderTexture(width, height, 0));
		RenderTexture renderTexture3 = renderTexture2;
		self.Render();
		RenderTexture.active = renderTexture3;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		texture2D.ReadPixels(source, 0, 0);
		texture2D.Apply();
		self.targetTexture = targetTexture;
		RenderTexture.active = null;
		Object.Destroy(renderTexture3);
		return texture2D;
	}

	public static Texture2D Capture(this Camera self, Vector2 resolution)
	{
		return self.Capture((int)resolution.x, (int)resolution.y);
	}

	public static Camera SetFieldOfView(this Camera self, float fieldOfView)
	{
		self.fieldOfView = fieldOfView;
		return self;
	}

	public static Camera SetDepth(this Camera self, int depth)
	{
		self.depth = depth;
		return self;
	}
}
