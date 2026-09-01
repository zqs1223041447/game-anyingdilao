using System;

namespace SK.Framework;

public interface IState
{
	string Name { get; set; }

	void OnInitialization();

	void OnEnter();

	void OnStay();

	void OnExit();

	void OnTermination();

	void SwitchWhen(Func<bool> predicate, string targetStateName);
}
