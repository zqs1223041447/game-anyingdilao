namespace Entity.Enemies.EnemyState;

public interface IEnemyState
{
	EnemyStateType Type { get; }

	void OnEnter();

	void OnUpdate();

	void OnExit();
}
