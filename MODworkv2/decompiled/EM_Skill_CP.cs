using System;
using UnityEngine;

[Serializable]
public class EM_Skill_CP
{
	public int GlobalID;

	public int UseAni;

	public int CPFX;

	public int FSFXtype;

	public DamageType damageType;

	public int MainElement;

	public int RDcolor;

	public int ColorIndex;

	public bool ChangeSkin;

	public string SkinName;

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
