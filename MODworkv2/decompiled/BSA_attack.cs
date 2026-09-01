public class BSA_attack : BossAstat
{
	private BossA go;

	public float JStime;

	public void Enter(BossA parent)
	{
		go = parent;
		go.changeST(2);
		go.JStime = 0f;
	}

	public void Exit()
	{
		go.em.ClearActionState();
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
				if ((bool)go.tar && !go.em.IS_Frozen)
				{
					go.tar.transform.position = go.em.ATTarget.transform.position;
				}
			}
			else
			{
				go.ChangeState(new BSA_idle());
			}
		}
		else
		{
			go.ChangeState(new BSA_die());
		}
	}
}
