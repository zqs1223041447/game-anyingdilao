namespace Entity.Enemies.EnemyState.State_B.States;

public class EnemyState_B_Die : EnemyStateBase_B
{
	public override EnemyStateType Type => EnemyStateType.Die;

	public EnemyState_B_Die(EnemyB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			go.changeST(3);
		}
	}

	public override void OnUpdate()
	{
	}
}
