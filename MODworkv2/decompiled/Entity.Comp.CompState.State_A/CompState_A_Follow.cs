using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_A;

public class CompState_A_Follow : CompStateBase_A
{
	public override CompStateType Type => CompStateType.Follow;

	public CompState_A_Follow(CompA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.companion.ChangeIntent(Companion.CompanionIntentState.Follow);
		go.changeST(2);
		if ((bool)go.tar)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
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
