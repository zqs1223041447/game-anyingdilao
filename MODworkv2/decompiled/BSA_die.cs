using UnityEngine;

public class BSA_die : BossAstat
{
	private float time;

	private BossA go;

	public void Enter(BossA parent)
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
