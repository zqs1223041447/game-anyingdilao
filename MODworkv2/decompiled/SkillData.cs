using System;
using UI.Talent;
using UnityEngine;

[Serializable]
public abstract class SkillData : ISkillLevelData
{
	public Sprite icon;

	public Sprite iconB;

	public int Price;

	public int UnLock_Point;

	public int Xi;

	public SkillBT skillbt;

	public int Level_Max;

	public string Info;

	public string IndexName { get; set; }

	public int Level_Base { get; set; }

	public int Level_WeaponOn { get; set; }

	public int Level_Base_Last => Level_Base + Level_WeaponOn;

	public virtual string GetInfoA()
	{
		return null;
	}

	public virtual string GetInfoB()
	{
		return null;
	}
}
