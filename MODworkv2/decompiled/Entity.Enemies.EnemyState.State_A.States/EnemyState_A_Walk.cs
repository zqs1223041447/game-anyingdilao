namespace Entity.Enemies.EnemyState.State_A.States;

public class EnemyState_A_Walk : EnemyStateBase_A
{
	public override EnemyStateType Type => EnemyStateType.Walk;

	public EnemyState_A_Walk(EnemyA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			go.changeST(1);
			go.em.CanSO_Idle = true;
			if ((bool)go.em.emitter)
			{
				go.em.emitter.Play();
			}
			go.em.IsMove = true;
		}
	}

	public override void OnExit()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.CanSO_Idle = false;
			if ((bool)go.em.emitter)
			{
				go.em.emitter.Stop();
			}
			go.em.IsMove = false;
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
			return;
		}
		if (!go.em.playerManager || !go.em.playerManager.IsAlive)
		{
			Change(EnemyStateType.Patrol);
			return;
		}
		float num = go.em.DistanceToPoint();
		if (!go.em.IsDpsTarget && num >= go.brain.runtimeConfig.GiveUpDistanceFromPoint)
		{
			Change(EnemyStateType.Return);
		}
		else if (!go.em.hasTarget || !go.em.playerManager || !go.em.playerManager.IsAlive)
		{
			Change(EnemyStateType.Idle);
		}
		else if (go.em.canAttack && go.em.CanSeeTarget)
		{
			if (go.atCD)
			{
				Change(EnemyStateType.Attack);
			}
			else
			{
				Change(EnemyStateType.Idle);
			}
		}
	}
}
