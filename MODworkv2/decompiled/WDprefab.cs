using UnityEngine;

[CreateAssetMenu(fileName = "WDprefab", menuName = "World/WDprefab", order = 1)]
public class WDprefab : ScriptableObject
{
	public GameObject[] Temple;

	public GameObject[] TempleFX;

	public GameObject TempleSpark;

	public GameObject[] Chest;

	public SKprefab_OBJ[] Break;

	public GameObject[] Skill;
}
