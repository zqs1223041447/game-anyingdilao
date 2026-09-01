public class EMC_hurt : EnemyCstat
{
	private EnemyC go;

	public void Enter(EnemyC parent)
	{
		go = parent;
		go.changeST(4);
		if (!go.em.IS_Frozen)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
		go.em.IsAttack = false;
	}

	public void Exit()
	{
		go.em.IsYun = false;
	}

	public void Update()
	{
		if (!go.em.IsAlive)
		{
			go.ChangeState(new EMC_die());
		}
	}
}
