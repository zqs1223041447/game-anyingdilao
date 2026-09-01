using Entity.Comp.CompState.State_A;
using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_B;

public abstract class CompStateBase_B : ICompState
{
	protected readonly CompB go;

	protected CompStateMachine fsm;

	public abstract CompStateType Type { get; }

	protected CompStateBase_B(CompB go)
	{
		this.go = go;
	}

	public void BindFSM(CompStateMachine machine)
	{
		fsm = machine;
	}

	public virtual void OnEnter()
	{
	}

	public virtual void OnUpdate()
	{
	}

	public virtual void OnExit()
	{
	}

	protected void Change(CompStateType type)
	{
		fsm.RequestState(type);
	}
}
