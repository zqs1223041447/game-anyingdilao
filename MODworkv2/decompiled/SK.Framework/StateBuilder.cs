using System;

namespace SK.Framework;

public class StateBuilder<T> where T : State, new()
{
	private readonly T state;

	private readonly StateMachine stateMachine;

	public StateBuilder(T state, StateMachine stateMachine)
	{
		this.state = state;
		this.stateMachine = stateMachine;
	}

	public StateBuilder<T> OnInitialization(Action<T> onInitialization)
	{
		state.onInitialization = delegate
		{
			onInitialization(state);
		};
		return this;
	}

	public StateBuilder<T> OnEnter(Action<T> onEnter)
	{
		state.onEnter = delegate
		{
			onEnter(state);
		};
		return this;
	}

	public StateBuilder<T> OnStay(Action<T> onStay)
	{
		state.onStay = delegate
		{
			onStay(state);
		};
		return this;
	}

	public StateBuilder<T> OnExit(Action<T> onExit)
	{
		state.onExit = delegate
		{
			onExit(state);
		};
		return this;
	}

	public StateBuilder<T> OnTermination(Action<T> onTermination)
	{
		state.onTermination = delegate
		{
			onTermination(state);
		};
		return this;
	}

	public StateBuilder<T> SwitchWhen(Func<bool> predicate, string targetStateName)
	{
		state.SwitchWhen(predicate, targetStateName);
		return this;
	}

	public StateMachine Complete()
	{
		state.OnInitialization();
		return stateMachine;
	}
}
