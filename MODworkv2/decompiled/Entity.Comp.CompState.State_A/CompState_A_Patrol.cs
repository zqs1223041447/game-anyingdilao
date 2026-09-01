using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_A;

public class CompState_A_Patrol : CompStateBase_A
{
	public override CompStateType Type => CompStateType.Patrol;

	public CompState_A_Patrol(CompA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.changeST(6);
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
		}
		else if (go.companion.IsYun)
		{
			Change(CompStateType.Hurt);
		}
	}
}
