using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace SK.Framework;

public class FSM : MonoBehaviour
{
	private static FSM instance;

	private List<StateMachine> machines;

	public static FSM Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("[SKFramework.FSM]").AddComponent<FSM>();
				instance.machines = new List<StateMachine>();
				UnityEngine.Object.DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	private void Awake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.RegisterToScope(this, ProcessScope.Game);
	}

	private void Update()
	{
		foreach (StateMachine machine in machines)
		{
			machine.OnUpdate();
		}
	}

	private void OnDestroy()
	{
		if (SingletonMonoGlobal<SessionManager>.HasInstance)
		{
			SingletonMonoGlobal<SessionManager>.Instance.UnregisterFromScope(this, ProcessScope.Game);
		}
		instance = null;
	}

	public T Create<T>(string stateMachineName) where T : StateMachine, new()
	{
		Type typeFromHandle = typeof(T);
		stateMachineName = (string.IsNullOrEmpty(stateMachineName) ? typeFromHandle.Name : stateMachineName);
		if (machines.Find((StateMachine m) => m.Name == stateMachineName) == null)
		{
			T val = (T)Activator.CreateInstance(typeFromHandle);
			val.Name = stateMachineName;
			machines.Add(val);
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 成功创建状态机[{0}]", stateMachineName);
			return val;
		}
		Log.Error("<color=red><b>[SKFramework.FSM.Error]</b></color> 已存在名称为[{0}]的状态机 创建失败", stateMachineName);
		return null;
	}

	public bool Destroy(string stateMachineName)
	{
		StateMachine stateMachine = machines.Find((StateMachine m) => m.Name == stateMachineName);
		if (stateMachine != null)
		{
			stateMachine.OnDestroy();
			machines.Remove(stateMachine);
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 成功销毁状态机[{0}]", stateMachineName);
			return true;
		}
		Log.Error("<color=red><b>[SKFramework.FSM.Error]</b></color> 不存在名称为[{0}]的状态机 销毁失败", stateMachineName);
		return false;
	}

	public bool Destroy<T>(T stateMachine) where T : StateMachine, new()
	{
		if (machines.Contains(stateMachine))
		{
			Log.Info("<color=cyan><b>[SKFramework.FSM.Info]</b></color> 成功销毁状态机[{0}]", stateMachine.Name);
			stateMachine.OnDestroy();
			machines.Remove(stateMachine);
			return true;
		}
		Log.Error((object)"<color=red><b>[SKFramework.FSM.Error]</b></color> 销毁状态机失败");
		return false;
	}

	public T GetMachine<T>(string stateMachineName) where T : StateMachine
	{
		return (T)machines.Find((StateMachine m) => m.Name == stateMachineName);
	}
}
