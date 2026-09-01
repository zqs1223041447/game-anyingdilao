using UnityEngine;

public class EMC_idle : EnemyCstat
{
	private EnemyC go;

	public float timeA;

	public float timeB;

	public float timeC;

	public void Enter(EnemyC parent)
	{
		timeA = 0f;
		go = parent;
		go.changeST(0);
		go.em.CanSO_Idle = true;
		if (go.em.canAttack && go.em.CanSeeMVTarget)
		{
			if (go.tar != null && !go.em.IS_Frozen)
			{
				go.tar.transform.position = go.em.ATTarget.transform.position;
			}
		}
		else if (go.tar != null && !go.em.IS_Frozen)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
	}

	public void Exit()
	{
		go.em.CanSO_Idle = false;
	}

	public void Update()
	{
		if (go.em.IsAlive)
		{
			if (go.em.IsYun)
			{
				go.ChangeState(new EMC_hurt());
			}
			if (go.em.hadTarget && go.em.playerManager.IsAlive)
			{
				if (go.em.canAttack && go.em.CanSeeMVTarget)
				{
					if (go.tar != null && !go.em.IS_Frozen)
					{
						go.tar.transform.position = go.em.ATTarget.transform.position;
					}
					if (go.atCD)
					{
						timeB += Time.deltaTime;
						if (timeB > go.em.AT_Idle_Cur)
						{
							go.ChangeState(new EMC_attack());
						}
					}
				}
				else
				{
					go.ChangeState(new EMC_In());
				}
			}
			else
			{
				go.ChangeState(new EMC_In());
			}
		}
		else
		{
			go.ChangeState(new EMC_die());
		}
	}
}
