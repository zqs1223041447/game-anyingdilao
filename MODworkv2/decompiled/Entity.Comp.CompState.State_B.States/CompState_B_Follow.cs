using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_B.States;

public class CompState_B_Follow : CompStateBase_B
{
	public override CompStateType Type => CompStateType.Follow;

	public CompState_B_Follow(CompB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.companion.ChangeIntent(Companion.CompanionIntentState.Follow);
		go.changeST(2);
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
			return;
		}
		if (go.companion.IsYun)
		{
			Change(CompStateType.Hurt);
			return;
		}
		go.companion.RefreshFollowPoint();
		if (go.companion.HasReachedFollowPoint())
		{
			go.companion.RequestIdle();
		}
	}
}
