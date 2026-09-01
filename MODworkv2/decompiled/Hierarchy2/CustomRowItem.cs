using System;
using UnityEngine;

namespace Hierarchy2;

[Serializable]
public class CustomRowItem
{
	public enum BackgroundStyle
	{
		Solid,
		Ramp
	}

	public enum BackgroundMode
	{
		Full,
		Name
	}

	public GameObject gameObject;

	public bool useBackground;

	public Color backgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 41);

	public BackgroundStyle backgroundStyle;

	public BackgroundMode backgroundMode;

	public CustomRowItem()
	{
	}

	public CustomRowItem(GameObject gameObject)
	{
		this.gameObject = gameObject;
	}
}
