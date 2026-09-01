namespace Entity.Comp.CompanionAI;

public interface ICompState
{
	CompStateType Type { get; }

	void OnEnter();

	void OnUpdate();

	void OnExit();
}
