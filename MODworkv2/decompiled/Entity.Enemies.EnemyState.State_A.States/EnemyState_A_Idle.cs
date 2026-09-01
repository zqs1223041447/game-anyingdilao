using UnityEngine;

namespace Entity.Enemies.EnemyState.State_A.States;

public class EnemyState_A_Idle : EnemyStateBase_A
{
	private float attackWaitTimer;

	public override EnemyStateType Type => EnemyStateType.Idle;

	public EnemyState_A_Idle(EnemyA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		attackWaitTimer = 0f;
		if ((bool)go && (bool)go.em)
		{
			go.changeST(0);
			go.em.CanSO_Idle = true;
		}
	}

	public override void OnExit()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.CanSO_Idle = false;
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
		else if (!go.em.hasTarget || !go.em.playerManager || !go.em.playerManager.IsAlive || go.em.IS_Frozen)
		{
			attackWaitTimer = 0f;
		}
		else if (!go.em.canKeepAttack || !go.em.CanSeeTarget)
		{
			attackWaitTimer = 0f;
			Change(EnemyStateType.Walk);
		}
		else if (go.atCD)
		{
			attackWaitTimer += Time.deltaTime;
			if (attackWaitTimer >= go.em.AT_Idle_Cur)
			{
				attackWaitTimer = 0f;
				Change(EnemyStateType.Attack);
			}
		}
		else
		{
			attackWaitTimer = 0f;
		}
	}
}
