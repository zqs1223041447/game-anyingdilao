namespace UI.Talent;

public interface ISkillLevelData
{
	string IndexName { get; }

	int Level_Base { get; set; }

	int Level_WeaponOn { get; set; }
}
