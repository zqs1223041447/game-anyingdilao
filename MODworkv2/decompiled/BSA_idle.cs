using UnityEngine;

public class BSA_idle : BossAstat
{
	private BossA go;

	public float timeA;

	public float timeB;

	public float timeC;

	public void Enter(BossA parent)
	{
		timeA = 0f;
		timeB = 0f;
		go = parent;
		go.changeST(0);
		go.em.CanSO_Idle = true;
		if (go.em.BS.canAttack && go.em.CanSeeMVTarget)
		{
			if (!go.em.IS_Frozen && (bool)go.tar && (bool)go.em.ATTarget)
			{
				go.tar.transform.position = go.em.ATTarget.position;
			}
		}
		else if (!go.em.IS_Frozen && (bool)go.tar && (bool)go.tar2)
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
				go.ChangeState(new BSA_hurt());
			}
			if (go.em.hadTarget && go.em.playerManager.IsAlive)
			{
				if (go.em.BS.canAttack && go.em.CanSeeMVTarget)
				{
					if ((bool)go.tar && !go.em.IS_Frozen && (bool)go.em.ATTarget && (bool)go && (bool)go.em)
					{
						go.tar.transform.position = go.em.ATTarget.transform.position;
					}
					if (!go.atCD)
					{
						return;
					}
					timeB += Time.deltaTime;
					if (!(timeB > go.em.AT_Idle_Cur))
					{
						return;
					}
					if (go.em.BS.SK_Cur_IndexA == 1)
					{
						switch (go.em.BS.SK[go.em.BS.SK_Cur_IndexB].CJY)
						{
						case 0:
							go.ChangeState(new BSA_attack());
							break;
						case 1:
							go.ChangeState(new BSA_Jump());
							break;
						case 2:
							go.ChangeState(new BSA_Jump());
							break;
						case 3:
							go.ChangeState(new BSA_Chuan());
							break;
						}
					}
					else
					{
						go.ChangeState(new BSA_attack());
					}
				}
				else
				{
					go.ChangeState(new BSA_walk());
				}
			}
			else if (go.em.canXL)
			{
				timeA += Time.deltaTime;
				if (timeA > Random.Range(2f, 8f))
				{
					timeA = 0f;
					go.ChangeState(new BSA_XL());
				}
			}
		}
		else
		{
			go.ChangeState(new BSA_die());
		}
	}
}
