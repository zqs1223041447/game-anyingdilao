using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_A;

public class CompState_A_Die : CompStateBase_A
{
	public override CompStateType Type => CompStateType.Die;

	public CompState_A_Die(CompA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.changeST(4);
	}

	public override void OnUpdate()
	{
	}
}
