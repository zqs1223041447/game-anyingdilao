public class EMC_Out : EnemyCstat
{
	private EnemyC go;

	public float time;

	public void Enter(EnemyC parent)
	{
		go = parent;
		go.changeST(6);
	}

	public void Exit()
	{
		time = 0f;
	}

	public void Update()
	{
	}
}
