using UnityEngine;

[CreateAssetMenu(fileName = "MusicData", menuName = "Audio/MusicData", order = 2)]
public class MusicData : ScriptableObject
{
	public string StartUI;

	public string StartLoop;

	public string[] HomeStart;

	public string[] HomeVictory;

	public string complete;

	public MusicGroup Level_BGM;

	public MusicGroup ATOM;

	public MusicChallengeGroup challengeGroups;

	public MusicMijingGroup mijingGroups;
}
