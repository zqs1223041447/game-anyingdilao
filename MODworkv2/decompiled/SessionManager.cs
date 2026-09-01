using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;

public class SessionManager : SingletonMonoGlobal<SessionManager>
{
	private readonly Dictionary<ProcessScope, ProcessRoot> _roots = new Dictionary<ProcessScope, ProcessRoot>();

	private readonly Dictionary<ProcessScope, List<UnityEngine.Object>> _scopeObjects = new Dictionary<ProcessScope, List<UnityEngine.Object>>();

	public bool HasScope(ProcessScope scope)
	{
		if (_roots.TryGetValue(scope, out var value))
		{
			return value;
		}
		return false;
	}

	private void CleanupDeadObjects(ProcessScope scope)
	{
		if (!_scopeObjects.TryGetValue(scope, out var value))
		{
			return;
		}
		for (int num = value.Count - 1; num >= 0; num--)
		{
			if (!value[num])
			{
				value.RemoveAt(num);
			}
		}
	}

	public ProcessRoot CreateScope(ProcessScope scope)
	{
		if (_roots.TryGetValue(scope, out var value))
		{
			if ((bool)value)
			{
				return value;
			}
			_roots.Remove(scope);
		}
		value = new GameObject($"[{scope}Managers]").AddComponent<ProcessRoot>();
		value.Init(scope);
		_roots[scope] = value;
		return value;
	}

	public void UnregisterFromScope(UnityEngine.Object obj, ProcessScope scope)
	{
		if (_scopeObjects.TryGetValue(scope, out var value))
		{
			value.Remove(obj);
		}
	}

	public void RegisterToScope(UnityEngine.Object obj, ProcessScope scope)
	{
		if (!_scopeObjects.TryGetValue(scope, out var value))
		{
			value = new List<UnityEngine.Object>();
			_scopeObjects[scope] = value;
		}
		for (int num = value.Count - 1; num >= 0; num--)
		{
			if (!value[num])
			{
				value.RemoveAt(num);
			}
		}
		if (!value.Contains(obj))
		{
			value.Add(obj);
		}
	}

	private void DestroyRegisteredObjects(ProcessScope scope)
	{
		if (!_scopeObjects.TryGetValue(scope, out var value))
		{
			return;
		}
		CleanupDeadObjects(scope);
		foreach (UnityEngine.Object item in value)
		{
			if ((bool)item)
			{
				UnityEngine.Object.Destroy(item);
			}
		}
		value.Clear();
		_scopeObjects.Remove(scope);
	}

	public void DestroyScope(ProcessScope scope)
	{
		DestroyRegisteredObjects(scope);
		if (_roots.TryGetValue(scope, out var value))
		{
			UnityEngine.Object.Destroy(value.gameObject);
			_roots.Remove(scope);
		}
	}

	public Transform GetScopeRoot(ProcessScope scope)
	{
		if (!_roots.TryGetValue(scope, out var value) || !value)
		{
			return null;
		}
		return value.transform;
	}

	public void Attach(MonoBehaviour obj, ProcessScope scope)
	{
		Type baseType = obj.GetType().BaseType;
		if ((object)baseType != null && baseType.IsGenericType && obj.GetType().BaseType.GetGenericTypeDefinition() == typeof(SingletonMonoGlobal<>))
		{
			LogUtil.Error("Global Singleton 不允许 Attach 到 Scope: " + obj.name);
			return;
		}
		Transform parent = CreateScope(scope).transform;
		obj.transform.SetParent(parent, worldPositionStays: false);
		RegisterToScope(obj, scope);
	}

	private void OnEnable()
	{
		Application.quitting += OnGameQuit;
	}

	private void OnDisable()
	{
		Application.quitting -= OnGameQuit;
	}

	private static void OnGameQuit()
	{
		if (!SaveManager.SaveAndExitBlocking())
		{
			LogUtil.Warn("Quit save failed or current state cannot be saved.");
		}
	}
}
