namespace Entity.Enemies.EnemyState.State_A.States;

public class EnemyState_A_Attack : EnemyStateBase_A
{
	private bool fired;

	public override EnemyStateType Type => EnemyStateType.Attack;

	public EnemyState_A_Attack(EnemyA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		fired = false;
		if ((bool)go && (bool)go.em)
		{
			go.PrepareAttackEnter();
			go.changeST(2);
			Change(EnemyStateType.Idle);
		}
	}

	public override void OnExit()
	{
		base.OnExit();
		go.em.ClearActionState();
	}

	public override void OnUpdate()
	{
		if ((bool)go && (bool)go.em)
		{
			if (!go.em.IsAlive)
			{
				Change(EnemyStateType.Die);
			}
			else if (!fired)
			{
				fired = true;
			}
			else
			{
				Change(EnemyStateType.Idle);
			}
		}
	}
}
