public class BSA_Chuan : BossAstat
{
	private BossA go;

	public float time;

	public void Enter(BossA parent)
	{
		go = parent;
		go.changeST(7);
		if (!go.em.IS_Frozen)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
	}

	public void Exit()
	{
		time = 0f;
		go.em.ClearActionState();
	}

	public void Update()
	{
	}
}
