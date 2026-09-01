public class EMC_In : EnemyCstat
{
	private EnemyC go;

	public float time;

	public void Enter(EnemyC parent)
	{
		go = parent;
		go.changeST(5);
	}

	public void Exit()
	{
		time = 0f;
	}

	public void Update()
	{
	}
}
