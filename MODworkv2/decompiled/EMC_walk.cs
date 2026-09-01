public class EMC_walk : EnemyCstat
{
	private EnemyC go;

	public float timeA;

	public float timeB;

	public void Enter(EnemyC parent)
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
	}

	public void Exit()
	{
		go.em.CanSO_Idle = false;
		if (go.em.emitter != null)
		{
			go.em.emitter.Stop();
		}
	}

	public void Update()
	{
		if (go.em.IsAlive)
		{
			if (go.em.FarAway)
			{
				go.ChangeState(new EMC_Out());
			}
			if (go.em.canAttack && go.em.CanSeeMVTarget)
			{
				if (go.tar != null && !go.em.IS_Frozen)
				{
					go.tar.transform.position = go.em.ATTarget.transform.position;
				}
				if (go.atCD)
				{
					go.ChangeState(new EMC_Out());
				}
			}
		}
		else
		{
			go.ChangeState(new EMC_die());
		}
	}
}
