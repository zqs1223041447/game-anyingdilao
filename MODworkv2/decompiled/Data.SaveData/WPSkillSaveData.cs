using System;

namespace Data.SaveData;

[Serializable]
public class WPSkillSaveData
{
	public string IndexName;

	public int Number;

	public int Number2;

	public int price;

	public static WPSkillSaveData FromRuntime(WPSkill skill)
	{
		if (skill == null)
		{
			return null;
		}
		return new WPSkillSaveData
		{
			IndexName = skill.IndexName,
			Number = skill.Number,
			Number2 = skill.Number2,
			price = skill.price
		};
	}

	public void ApplyToRuntime(WPSkill skill)
	{
		if (skill != null)
		{
			skill.IndexName = IndexName;
			skill.Number = Number;
			skill.Number2 = Number2;
			skill.price = price;
		}
	}
}
