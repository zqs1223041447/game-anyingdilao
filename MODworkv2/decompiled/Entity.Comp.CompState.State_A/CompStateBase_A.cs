using Entity.Comp.CompanionAI;

namespace Entity.Comp.CompState.State_A;

public abstract class CompStateBase_A : ICompState
{
	protected readonly CompA go;

	protected CompStateMachine fsm;

	public abstract CompStateType Type { get; }

	protected CompStateBase_A(CompA go)
	{
		this.go = go;
	}

	public void BindFSM(CompStateMachine state)
	{
		fsm = state;
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
