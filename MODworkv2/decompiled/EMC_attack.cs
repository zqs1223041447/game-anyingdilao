public class EMC_attack : EnemyCstat
{
	private EnemyC go;

	public float JStime;

	public void Enter(EnemyC parent)
	{
		go = parent;
		go.changeST(2);
		go.JStime = 0f;
	}

	public void Exit()
	{
		go.em.IsAttack = false;
		go.em.IsChong = false;
		go.em.IsBaTi = false;
		go.em.IsWuDi = false;
		go.em.IsFang = false;
	}

	public void Update()
	{
		if (go.em.IsAlive)
		{
			if (go.em.IsYun)
			{
				go.ChangeState(new EMC_hurt());
			}
			if (go.em.hadTarget)
			{
				if (go.tar != null && !go.em.IS_Frozen)
				{
					go.tar.transform.position = go.em.ATTarget.transform.position;
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
