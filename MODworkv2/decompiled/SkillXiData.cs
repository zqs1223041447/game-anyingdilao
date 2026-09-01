using System;
using System.Collections.Generic;

[Serializable]
public class SkillXiData
{
	public bool Used;

	public string IndexName;

	public int Level_Base;

	public DamageType damageType;

	public float Number;

	public Dictionary<string, SkillData_Sample_Father> Sample_F = new Dictionary<string, SkillData_Sample_Father>();

	public Dictionary<string, SkillData_Sample_Son> Sample_S = new Dictionary<string, SkillData_Sample_Son>();

	public Dictionary<string, SkillData_Comp_Father> Comp_F = new Dictionary<string, SkillData_Comp_Father>();

	public Dictionary<string, SkillData_Comp_Son> Comp_S = new Dictionary<string, SkillData_Comp_Son>();

	public Dictionary<string, SkillData_Dot_Father> Dot_F = new Dictionary<string, SkillData_Dot_Father>();

	public Dictionary<string, SkillData_Dot_Son> Dot_S = new Dictionary<string, SkillData_Dot_Son>();

	public Dictionary<string, SkillData_Bei> Bei = new Dictionary<string, SkillData_Bei>();

	public string GetInfoA()
	{
		return string.Empty + string.Format("{0} {1}% {2}", LOC.MM.GetMain("Increases per skill point invested"), Number, LOC.MM.GetMain(SWS.El_DMG(damageType)));
	}

	public string GetInfoB()
	{
		return string.Empty + string.Format("{0} : + {1}% {2}", LOC.MM.GetMain("Current bonus"), Number * (float)Level_Base, LOC.MM.GetMain(SWS.El_DMG(damageType)));
	}
}
