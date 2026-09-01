using System.Collections.Generic;
using Entity.Comp.CompanionAI;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Entity.Comp.CompState.State_A;

public class CompStateMachine
{
	private readonly Dictionary<CompStateType, ICompState> states;

	private ICompState current;

	private ICompState pending;

	private ICompState previous;

	private bool isTransitioning;

	private float stateEnterTime;

	public CompStateType CurrentType => current?.Type ?? CompStateType.Idle;

	public CompStateType PendingType => pending?.Type ?? CompStateType.Idle;

	public CompStateType PreviousType => previous?.Type ?? CompStateType.Idle;

	public float StateElapsedTime => Time.time - stateEnterTime;

	public CompStateMachine(Dictionary<CompStateType, ICompState> states)
	{
		this.states = states;
	}

	public bool CanTransition(float minTime)
	{
		return StateElapsedTime >= minTime;
	}

	public void RequestState(CompStateType type)
	{
		if (!isTransitioning && (type == CompStateType.Attack || type == CompStateType.Hurt || CanTransition(0.05f)) && (current == null || current.Type != type))
		{
			if (!states.TryGetValue(type, out var value))
			{
				LogUtil.Error("A类玩家同伴状态机", $"{type} 状态未注册");
			}
			else
			{
				pending = value;
			}
		}
	}

	public void Tick()
	{
		if (pending != null)
		{
			PerformTransition();
		}
		current?.OnUpdate();
	}

	private void PerformTransition()
	{
		isTransitioning = true;
		current?.OnExit();
		previous = current;
		current = pending;
		pending = null;
		stateEnterTime = Time.time;
		current.OnEnter();
		isTransitioning = false;
	}

	public void SetInitialState(CompStateType type)
	{
		if (current != null)
		{
			LogUtil.Error("A类玩家同伴状态机", "初始状态已经被设置");
			return;
		}
		if (!states.TryGetValue(type, out current))
		{
			LogUtil.Error("A类玩家同伴状态机", $"未发现该初始状态: {type}");
			return;
		}
		pending = null;
		current.OnEnter();
	}

	public void Reset()
	{
		current = null;
		pending = null;
		previous = null;
		isTransitioning = false;
		stateEnterTime = 0f;
	}
}
