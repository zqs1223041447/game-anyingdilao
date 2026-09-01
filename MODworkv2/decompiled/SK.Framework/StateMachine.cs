using System;
using System.Collections.Generic;

namespace SK.Framework;

public class StateMachine
{
	protected readonly List<IState> states = new List<IState>();

	protected List<StateSwitchCondition> conditions = new List<StateSwitchCondition>();

	public string Name { get; set; }

	public IState CurrentState { get; protected set; }

	public bool Add(IState state)
	{
		if (!states.Contains(state) && states.Find((IState m) => m.Name == state.Name) == null)
		{
			states.Add(state);
			state.OnInitialization();
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]添加状态[{1}]", Name, state.Name);
			return true;
		}
		Log.Error("<color=red><b>[SKFramework.FSM.Error]</b></color> 状态机[{0}]已包含状态[{1}] 无需重复添加", Name, state.Name);
		return false;
	}

	public bool Add<T>(string stateName = null) where T : IState, new()
	{
		Type typeFromHandle = typeof(T);
		T val = (T)Activator.CreateInstance(typeFromHandle);
		ref T reference = ref val;
		T val2 = default(T);
		if (val2 == null)
		{
			val2 = reference;
			reference = ref val2;
		}
		reference.Name = (string.IsNullOrEmpty(stateName) ? typeFromHandle.Name : stateName);
		return Add(val);
	}

	public bool Remove(IState state)
	{
		if (states.Contains(state))
		{
			if (CurrentState == state)
			{
				CurrentState.OnExit();
				CurrentState = null;
			}
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]移除状态[{1}]", Name, state.Name);
			state.OnTermination();
			states.Remove(state);
			return true;
		}
		Log.Error("<color=red><b>[SKFramework.FSM.Error]</b></color> 状态机[{0}]不包含状态[{1}] 移除失败", Name, state.Name);
		return false;
	}

	public bool Remove(string stateName)
	{
		int num = states.FindIndex((IState m) => m.Name == stateName);
		if (num != -1)
		{
			IState state = states[num];
			if (CurrentState == state)
			{
				CurrentState.OnExit();
				CurrentState = null;
			}
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]移除状态[{1}]", Name, stateName);
			state.OnTermination();
			return states.Remove(state);
		}
		Log.Error("<color=red><b>[SKFramework.FSM.Error]</b></color> 状态机[{0}]不包含状态[{1}] 移除失败", Name, stateName);
		return false;
	}

	public bool Remove<T>() where T : IState
	{
		return Remove(typeof(T).Name);
	}

	public bool Switch(IState state)
	{
		if (CurrentState == state)
		{
			return false;
		}
		CurrentState?.OnExit();
		if (!states.Contains(state))
		{
			return false;
		}
		CurrentState = state;
		if (CurrentState != null)
		{
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]切换至状态[{1}]", Name, CurrentState.Name);
			CurrentState.OnEnter();
		}
		return true;
	}

	public bool Switch(string stateName)
	{
		IState state = states.Find((IState m) => m.Name == stateName);
		return Switch(state);
	}

	public bool Switch<T>() where T : IState
	{
		return Switch(typeof(T).Name);
	}

	public void Switch2Next()
	{
		if (states.Count != 0)
		{
			if (CurrentState != null)
			{
				int num = states.IndexOf(CurrentState);
				num = ((num + 1 < states.Count) ? (num + 1) : 0);
				IState currentState = states[num];
				CurrentState.OnExit();
				CurrentState = currentState;
			}
			else
			{
				CurrentState = states[0];
			}
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]切换至下一状态[{1}]", Name, CurrentState.Name);
			CurrentState.OnEnter();
		}
	}

	public void Switch2Last()
	{
		if (states.Count != 0)
		{
			if (CurrentState != null)
			{
				int num = states.IndexOf(CurrentState);
				num = ((num - 1 >= 0) ? (num - 1) : (states.Count - 1));
				IState currentState = states[num];
				CurrentState.OnExit();
				CurrentState = currentState;
			}
			else
			{
				CurrentState = states[states.Count - 1];
			}
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]切换至上一状态[{1}]", Name, CurrentState.Name);
			CurrentState.OnEnter();
		}
	}

	public void Switch2Null()
	{
		if (CurrentState != null)
		{
			CurrentState.OnExit();
			CurrentState = null;
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]退出当前状态", Name);
		}
	}

	public T GetState<T>(string stateName) where T : IState
	{
		return (T)states.Find((IState m) => m.Name == stateName);
	}

	public T GetState<T>() where T : IState
	{
		return (T)states.Find((IState m) => m.Name == typeof(T).Name);
	}

	public void Destroy()
	{
		FSM.Instance.Destroy(this);
	}

	public void OnUpdate()
	{
		CurrentState?.OnStay();
		for (int i = 0; i < conditions.Count; i++)
		{
			StateSwitchCondition stateSwitchCondition = conditions[i];
			if (stateSwitchCondition.predicate())
			{
				if (string.IsNullOrEmpty(stateSwitchCondition.sourceStateName))
				{
					Switch(stateSwitchCondition.targetStateName);
				}
				else if (CurrentState.Name == stateSwitchCondition.sourceStateName)
				{
					Switch(stateSwitchCondition.targetStateName);
				}
			}
		}
	}

	public void OnDestroy()
	{
		for (int i = 0; i < states.Count; i++)
		{
			states[i].OnTermination();
		}
	}

	public StateMachine SwitchWhen(Func<bool> predicate, string targetStateName)
	{
		conditions.Add(new StateSwitchCondition(predicate, null, targetStateName));
		Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]添加切换至状态[{1}]的条件", Name, targetStateName);
		return this;
	}

	public StateMachine SwitchWhen(Func<bool> predicate, string sourceStateName, string targetStateName)
	{
		conditions.Add(new StateSwitchCondition(predicate, sourceStateName, targetStateName));
		Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]添加状态[{0}]切换至状态[{2}]的条件", Name, sourceStateName, targetStateName);
		return this;
	}

	public StateBuilder<T> Build<T>(string stateName = null) where T : State, new()
	{
		Type typeFromHandle = typeof(T);
		string name = (string.IsNullOrEmpty(stateName) ? typeFromHandle.Name : stateName);
		if (states.Find((IState m) => m.Name == name) == null)
		{
			T val = (T)Activator.CreateInstance(typeFromHandle);
			val.Name = name;
			states.Add(val);
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 状态机[{0}]构建状态[{1}]", Name, name);
			return new StateBuilder<T>(val, this);
		}
		Log.Error("<color=red><b>[SKFramework.FSM.Error]</b></color> 状态机[{0}]已包含名为[{1}]的状态 构建失败", Name, name);
		return null;
	}

	public static StateMachine Create(string stateMachineName)
	{
		return FSM.Instance.Create<StateMachine>(stateMachineName);
	}

	public static T Create<T>(string stateMachineName = null) where T : StateMachine, new()
	{
		return FSM.Instance.Create<T>(stateMachineName);
	}

	public static bool Destroy(string stateMachineName)
	{
		return FSM.Instance.Destroy(stateMachineName);
	}

	public static bool Destroy<T>() where T : StateMachine
	{
		return FSM.Instance.Destroy(typeof(T).Name);
	}

	public static StateMachine Get(string stateMachineName)
	{
		return FSM.Instance.GetMachine<StateMachine>(stateMachineName);
	}

	public static T Get<T>(string stateMachineName) where T : StateMachine
	{
		return FSM.Instance.GetMachine<T>(stateMachineName);
	}

	public static T Get<T>() where T : StateMachine
	{
		return FSM.Instance.GetMachine<T>(typeof(T).Name);
	}
}
