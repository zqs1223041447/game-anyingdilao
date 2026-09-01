using UnityEngine;
using UnityEngine.Events;

namespace SK.Framework;

public class AimableObject : MonoBehaviour, IAimableObject
{
	[SerializeField]
	protected string description;

	[SerializeField]
	protected float aimableDistance = 2f;

	[SerializeField]
	protected UnityEvent onEnter;

	[SerializeField]
	protected UnityEvent onExit;

	[SerializeField]
	protected UnityEvent onStay;

	public string Description => description;

	public float AimableDistance => aimableDistance;

	protected virtual void OnEnter()
	{
	}

	protected virtual void OnExit()
	{
	}

	protected virtual void OnStay()
	{
	}

	public void Enter()
	{
		OnEnter();
		onEnter?.Invoke();
	}

	public void Exit()
	{
		OnExit();
		onExit?.Invoke();
	}

	public void Stay()
	{
		OnStay();
		onStay?.Invoke();
	}
}
