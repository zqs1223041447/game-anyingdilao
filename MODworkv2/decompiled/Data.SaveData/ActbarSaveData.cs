using System;
using System.Collections.Generic;

namespace Data.SaveData;

[Serializable]
public class ActbarSaveData
{
	public List<ActbarSkillSlotSaveData> SkillSlots;

	public List<ActbarUseSlotSaveData> UseSlots;

	public static ActbarSaveData CreateDefault()
	{
		return new ActbarSaveData
		{
			SkillSlots = new List<ActbarSkillSlotSaveData>(),
			UseSlots = new List<ActbarUseSlotSaveData>()
		};
	}
}
