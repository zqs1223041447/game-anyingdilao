using System;
using System.Collections.Generic;
using Level.LevelStates;
using UnityEngine;

namespace Data.AutoGen.DataClass.Level;

[Serializable]
public class LevelData
{
	public string GlobalID;

	public string LocalName;

	public int prefabIndex;

	public LevelType Type;

	public string ParentMainId;

	public int BGM_Index;

	public int ATOM_Index;

	public float Globle_Intensity;

	public Color Globle_Color;

	public int MapLevel;

	public float Tower_Health;

	public float Tower_Damage;

	public bool IsMJ;

	public int JY_Rate;

	public List<int> Enemy_list;

	public List<int> JY_list;

	public int ChestIndex;

	public float ChestDis;

	public bool IsFinal;
}
