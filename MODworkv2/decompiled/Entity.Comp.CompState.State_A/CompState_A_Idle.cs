using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_A;

public class CompState_A_Idle : CompStateBase_A
{
	public override CompStateType Type => CompStateType.Idle;

	public CompState_A_Idle(CompA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.changeST(0);
		go.companion.CanSO_Idle = true;
		if ((bool)go.tar)
		{
			if ((bool)go.companion.ATTarget)
			{
				go.tar.transform.position = go.companion.ATTarget.transform.position;
			}
			else
			{
				go.tar.transform.position = go.tar2.transform.position;
			}
		}
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
