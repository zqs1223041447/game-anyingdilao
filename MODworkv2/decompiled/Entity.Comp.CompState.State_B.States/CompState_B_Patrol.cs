using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_B.States;

public class CompState_B_Patrol : CompStateBase_B
{
	public override CompStateType Type => CompStateType.Patrol;

	public CompState_B_Patrol(CompB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.companion.ChangeIntent(Companion.CompanionIntentState.Patrol);
		go.changeST(5);
		go.companion.CanSO_Idle = true;
	}

	public override void OnExit()
	{
		go.companion.CanSO_Idle = false;
	}

	public override void OnUpdate()
	{
		if (!go.companion.IsAlive)
		{
			Change(CompStateType.Die);
		}
		else if (go.companion.IsYun)
		{
			Change(CompStateType.Hurt);
		}
		else if (go.companion.HasReachedPatrolPoint())
		{
			Change(CompStateType.Idle);
		}
	}
}
