using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Entity.Enemies.EnemyState.State_B.States;

public class EnemyState_B_Idle : EnemyStateBase_B
{
	private float attackWaitTimer;

	public override EnemyStateType Type => EnemyStateType.Idle;

	public EnemyState_B_Idle(EnemyB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		attackWaitTimer = 0f;
		if (!go || !go.em || !SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return;
		}
		go.changeST(0);
		go.em.CanSO_Idle = true;
		if (!go.tar)
		{
			return;
		}
		if (go.em.canAttack && go.em.CanSeeTarget && (bool)go.em.ATTarget)
		{
			if (!go.em.IS_Frozen && (bool)go.tar && go.tar.transform != go.em.playerManager.transform)
			{
				go.tar.transform.position = go.em.ATTarget.transform.position;
			}
		}
		else if (!go.em.IS_Frozen && (bool)go.tar2 && go.tar2.transform != go.em.playerManager.transform)
		{
			go.tar.transform.position = go.tar2.transform.position;
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
		else if (go.em.IsYun)
		{
			Change(EnemyStateType.Hurt);
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
