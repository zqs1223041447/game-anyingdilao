using System;
using System.Collections.Generic;

namespace Data.SaveData;

[Serializable]
public class TalentSaveData
{
	public int P_Base;

	public int P_Used;

	public int P_Used_DF;

	public bool HasAppliedDFLieBonuses;

	public bool HasOpenedTalentPanel;

	public bool HasAddedAnySkillPoint;

	public bool HasOpenedActSkillListAfterFirstSkillPoint;

	public Dictionary<string, SkillSaveData> All_Skill_Datas;

	public Dictionary<string, XiSaveData> All_Xi_Datas;

	public static TalentSaveData CreateDefault()
	{
		return new TalentSaveData
		{
			P_Base = 1,
			P_Used = 0,
			P_Used_DF = 0,
			All_Skill_Datas = new Dictionary<string, SkillSaveData>(),
			All_Xi_Datas = new Dictionary<string, XiSaveData>()
		};
	}
}
