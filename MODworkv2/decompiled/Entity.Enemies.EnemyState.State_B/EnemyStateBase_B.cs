namespace Entity.Enemies.EnemyState.State_B;

public abstract class EnemyStateBase_B : IEnemyState
{
	protected readonly EnemyB go;

	protected EnemyStateMachine fsm;

	public abstract EnemyStateType Type { get; }

	protected EnemyStateBase_B(EnemyB go)
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
