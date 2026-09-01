using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_B.States;

public class CompState_B_Die : CompStateBase_B
{
	public override CompStateType Type => CompStateType.Die;

	public CompState_B_Die(CompB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.changeST(4);
	}
}
