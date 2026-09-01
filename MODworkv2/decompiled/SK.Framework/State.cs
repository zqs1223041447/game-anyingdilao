using System;

namespace SK.Framework;

public class State : IState
{
	public StateMachine machine;

	public Action onInitialization;

	public Action onEnter;

	public Action onStay;

	public Action onExit;

	public Action onTermination;

	public string Name { get; set; }

	public virtual void OnInitialization()
	{
		onInitialization?.Invoke();
	}

	public virtual void OnEnter()
	{
		onEnter?.Invoke();
	}

	public virtual void OnStay()
	{
		onStay?.Invoke();
	}

	public virtual void OnExit()
	{
		onExit?.Invoke();
	}

	public virtual void OnTermination()
	{
		onTermination?.Invoke();
	}

	public void SwitchWhen(Func<bool> predicate, string targetStateName)
	{
		machine.SwitchWhen(predicate, Name, targetStateName);
	}
}
