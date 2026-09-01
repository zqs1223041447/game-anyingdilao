public class BSA_walk : BossAstat
{
	private BossA go;

	public float timeA;

	public float timeB;

	public void Enter(BossA parent)
	{
		timeA = 0f;
		timeB = 0f;
		go = parent;
		go.changeST(1);
		if (go.tar != null && !go.em.IS_Frozen)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
		go.em.CanSO_Idle = true;
		if (go.em.emitter != null)
		{
			go.em.emitter.Play();
		}
		go.em.IsMove = true;
	}

	public void Exit()
	{
		go.em.CanSO_Idle = false;
		if (go.em.emitter != null)
		{
			go.em.emitter.Stop();
		}
		go.em.IsMove = false;
	}

	public void Update()
	{
		if (go.em.IsAlive)
		{
			if (go.em.IsYun)
			{
				go.ChangeState(new BSA_hurt());
			}
			if (go.em.FarAway)
			{
				go.ChangeState(new BSA_idle());
			}
			if (!go.em.playerManager.IsAlive)
			{
				go.ChangeState(new BSA_XL());
			}
			if (!go.em.BS.canAttack || !go.em.CanSeeMVTarget)
			{
				return;
			}
			if (go.tar != null && !go.em.IS_Frozen)
			{
				go.tar.transform.position = go.em.ATTarget.transform.position;
			}
			if (!go.atCD)
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
			go.ChangeState(new BSA_die());
		}
	}
}
