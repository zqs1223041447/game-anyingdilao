namespace Entity.Enemies.EnemyState.State_A;

public abstract class EnemyStateBase_A : IEnemyState
{
	protected readonly EnemyA go;

	protected EnemyStateMachine fsm;

	public abstract EnemyStateType Type { get; }

	protected EnemyStateBase_A(EnemyA go)
	{
		this.go = go;
	}

	public void BindFSM(EnemyStateMachine state)
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

	protected void Change(EnemyStateType type)
	{
		fsm.RequestState(type);
	}
}
