using UnityEngine;

public class BSA_XL : BossAstat
{
	private BossA go;

	public float time;

	public void Enter(BossA parent)
	{
		go = parent;
		go.changeST(5);
		if (!go.em.IS_Frozen)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
		go.em.CanSO_Idle = true;
	}

	public void Exit()
	{
		time = 0f;
		go.em.CanSO_Idle = false;
	}

	public void Update()
	{
		if (go.em.IsAlive)
		{
			if (go.em.IsYun)
			{
				go.ChangeState(new BSA_hurt());
			}
			time += Time.deltaTime;
			if (time > Random.Range(1f, 2f) || go.em.transform.position == go.em.XLpoint.transform.position)
			{
				go.ChangeState(new BSA_idle());
			}
			if (go.em.hadTarget && go.em.playerManager.IsAlive)
			{
				go.ChangeState(new BSA_walk());
			}
		}
		else
		{
			go.ChangeState(new BSA_die());
		}
	}
}
