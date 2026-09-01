using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_B.States;

public class CompState_B_Walk : CompStateBase_B
{
	public override CompStateType Type => CompStateType.Walk;

	public CompState_B_Walk(CompB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.changeST(1);
		if (!go.companion.MVTarget)
		{
			Change(CompStateType.Idle);
		}
		else
		{
			go.companion.CanSO_Idle = true;
		}
	}

	public override void OnExit()
	{
		go.companion.CanSO_Idle = false;
	}

	public override void OnUpdate()
	{
		if (!go.companion.MVTarget)
		{
			Change(CompStateType.Follow);
		}
		else if (!go.companion.IsAlive)
		{
			Change(CompStateType.Die);
		}
		else if (go.companion.IsYun)
		{
			Change(CompStateType.Hurt);
		}
	}
}
