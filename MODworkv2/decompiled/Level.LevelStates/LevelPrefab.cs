using UnityEngine;

namespace Level.LevelStates;

[CreateAssetMenu(fileName = "LevelPrefab", menuName = "关卡/关卡预制体设置")]
public class LevelPrefab : ScriptableObject
{
	public GameObject Home;

	public GameObject[] Normal;

	public GameObject[] Boss;

	public GameObject[] Optional;

	public GameObject[] Challenge;

	public GameObject[] Mijing;
}
