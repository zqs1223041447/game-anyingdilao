using System;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Interact;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
	public virtual InteractionType Type => InteractionType.None;

	public virtual int Priority => 0;

	public bool IsHovering { get; private set; }

	protected virtual void OnDisable()
	{
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			SingletonMonoScope<InteractionManager>.Instance.ClearHover(this);
		}
	}

	public bool CanHover()
	{
		return true;
	}

	public virtual void OnHoverEnter()
	{
		IsHovering = true;
		OnHover(isHovering: true);
	}

	public virtual void OnHoverExit()
	{
		IsHovering = false;
		OnHover(isHovering: false);
	}

	protected virtual void OnHover(bool isHovering)
	{
	}

	public virtual void OnLeftClick()
	{
		if (CanInteract())
		{
			try
			{
				Interact();
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				throw;
			}
		}
	}

	public virtual void OnRightClick()
	{
	}

	public virtual bool CanInteract()
	{
		return true;
	}

	public abstract void Interact();
}
