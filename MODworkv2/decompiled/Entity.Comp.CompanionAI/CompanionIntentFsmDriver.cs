using System;

namespace Entity.Comp.CompanionAI;

public class CompanionIntentFsmDriver
{
	public static void Drive(Companion companion, Action<CompStateType> requestState)
	{
		if (!companion || requestState == null || companion.IsAttack || companion.IsSkill)
		{
			return;
		}
		if (!companion.IsAlive)
		{
			requestState(CompStateType.Die);
			return;
		}
		if (companion.IsYun)
		{
			requestState(CompStateType.Hurt);
			return;
		}
		switch (companion.CurrentIntent)
		{
		case Companion.CompanionIntentState.Follow:
			requestState(CompStateType.Follow);
			break;
		case Companion.CompanionIntentState.Patrol:
			requestState(CompStateType.Patrol);
			break;
		case Companion.CompanionIntentState.None:
		case Companion.CompanionIntentState.Idle:
			requestState(CompStateType.Idle);
			break;
		case Companion.CompanionIntentState.Combat:
		{
			if (!companion.MVTarget || !companion.MVTarget.TryGetComponent<Enemy>(out var component) || !component.IsAlive)
			{
				companion.ChangeIntent(Companion.CompanionIntentState.Idle);
				companion.MVTarget = null;
				companion.ATTarget = null;
				requestState(CompStateType.Idle);
			}
			else if (!companion.MVTarget || !companion.ATTarget)
			{
				companion.ChangeIntent(Companion.CompanionIntentState.Idle);
				requestState(CompStateType.Idle);
			}
			else if (!companion.canAttack)
			{
				requestState(CompStateType.Walk);
			}
			else if (companion.IsAttackCooldownReady && !companion.IsAttack && !companion.IsSkill)
			{
				requestState(CompStateType.Attack);
			}
			else
			{
				requestState(CompStateType.Idle);
			}
			break;
		}
		}
	}
}
