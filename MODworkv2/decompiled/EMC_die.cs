using UnityEngine;

public class EMC_die : EnemyCstat
{
	private float time;

	private EnemyC go;

	public void Enter(EnemyC parent)
	{
		time = 0f;
		go = parent;
		go.changeST(3);
	}

	public void Exit()
	{
	}

	public void Update()
	{
		time += Time.deltaTime;
		if (time > 5f)
		{
			time = 0f;
		}
	}
}
