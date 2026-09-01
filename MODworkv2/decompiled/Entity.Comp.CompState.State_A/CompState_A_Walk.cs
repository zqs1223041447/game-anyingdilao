using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_A;

public class CompState_A_Walk : CompStateBase_A
{
	public override CompStateType Type => CompStateType.Walk;

	public CompState_A_Walk(CompA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.changeST(1);
		if ((bool)go.tar)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
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
