using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Entity.Enemies.EnemyState;

public class EnemyStateMachine
{
	private readonly Dictionary<EnemyStateType, IEnemyState> states;

	private IEnemyState current;

	private IEnemyState pending;

	private IEnemyState previous;

	private readonly Func<bool> stateLock;

	private bool isTransitioning;

	private float stateEnterTime;

	public EnemyStateType CurrentType => current?.Type ?? EnemyStateType.Idle;

	public EnemyStateType PendingType => pending?.Type ?? EnemyStateType.Idle;

	public EnemyStateType PreviousType => previous?.Type ?? EnemyStateType.Idle;

	public float StateElapsedTime => Time.time - stateEnterTime;

	public EnemyStateMachine(Dictionary<EnemyStateType, IEnemyState> states, Func<bool> stateLock = null)
	{
		this.states = states;
		this.stateLock = stateLock;
	}

	public bool CanTransition(float minTime)
	{
		return StateElapsedTime >= minTime;
	}

	public void RequestState(EnemyStateType type)
	{
		if (!isTransitioning && (type == EnemyStateType.Die || stateLock == null || !stateLock()) && (type == EnemyStateType.Attack || type == EnemyStateType.Hurt || CanTransition(0.05f)) && (current == null || current.Type != type))
		{
			if (!states.TryGetValue(type, out var value))
			{
				LogUtil.Error("EnemyStateMachine", $"{type} 状态未注册");
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

	public void SetInitialState(EnemyStateType type)
	{
		if (current != null)
		{
			LogUtil.Error("EnemyStateMachine", "已初始化");
			return;
		}
		if (!states.TryGetValue(type, out current))
		{
			LogUtil.Error("EnemyStateMachine", $"未发现该初始化状态: {type}");
			return;
		}
		pending = null;
		stateEnterTime = Time.time;
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
