using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefab", menuName = "Enemy/EnemyPrefab", order = 1)]
public class EnemyPrefab : ScriptableObject
{
	public EnemyOBJ_Class[] Enemy;

	public EnemyOBJ_Class[] Boss;
}
