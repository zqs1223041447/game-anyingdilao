namespace Entity.Enemies.EnemyState.State_B.States;

public class EnemyState_B_Attack : EnemyStateBase_B
{
	public override EnemyStateType Type => EnemyStateType.Attack;

	public EnemyState_B_Attack(EnemyB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			go.changeST(2);
			go.JStime = 0f;
		}
	}

	public override void OnExit()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.ClearActionState();
		}
	}

	public override void OnUpdate()
	{
		if (!go || !go.em)
		{
			return;
		}
		if (!go.em.IsAlive)
		{
			Change(EnemyStateType.Die);
		}
		else if (go.em.IsYun)
		{
			Change(EnemyStateType.Hurt);
		}
		else if ((!go.em.playerManager || !go.em.playerManager.IsAlive) && go.em.MoveSpeed_Path > 0f)
		{
			Change(EnemyStateType.Patrol);
		}
		else if (go.em.hasTarget)
		{
			if ((bool)go.tar && !go.em.IS_Frozen && (bool)go.em.ATTarget && go.tar.transform != go.em.playerManager.transform)
			{
				go.tar.transform.position = go.em.ATTarget.transform.position;
			}
			if (!go.em.IsAttack)
			{
				if (go.em.canKeepAttack)
				{
					Change(EnemyStateType.Idle);
				}
				else
				{
					Change((go.em.MoveSpeed_Path > 0f) ? EnemyStateType.Walk : EnemyStateType.Idle);
				}
			}
		}
		else
		{
			Change(EnemyStateType.Idle);
		}
	}
}
