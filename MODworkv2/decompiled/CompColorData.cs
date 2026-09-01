using System;
using UnityEngine;

[Serializable]
public class CompColorData
{
	public int Flip;

	public Vector4 MainMix;

	public int MainHue;

	public float MainSat;

	[ColorUsage(true, true)]
	public Color MainColor;

	[ColorUsage(true, true)]
	public Color DisloveColor;

	[ColorUsage(true, true)]
	public Color AlphaColor;
}
