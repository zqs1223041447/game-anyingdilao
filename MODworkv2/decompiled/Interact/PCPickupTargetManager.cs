using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;

namespace Interact;

public class PCPickupTargetManager : SingletonMonoScope<PCPickupTargetManager>
{
	[Header("扫描设置")]
	[SerializeField]
	private float scanRadius = 3.2f;

	[SerializeField]
	private LayerMask interactLayer;

	[SerializeField]
	private bool drawGizmos = true;

	private Transform playerTf;

	private readonly List<IInteractable> candidates = new List<IInteractable>(16);

	private static readonly Collider2D[] overlapResults = new Collider2D[32];

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	private void Start()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			playerTf = SingletonMonoScope<PlayerManager>.Instance.transform;
		}
		if (interactLayer.value == 0 && SingletonMonoScope<InteractionManager>.HasInstance)
		{
			interactLayer = SingletonMonoScope<InteractionManager>.Instance.interactLayer;
		}
	}

	public static string GetLocalMode(PcPickupMode mode)
	{
		return mode switch
		{
			PcPickupMode.Off => LOC.MM.GetStart("pc_pickup_mode_off"), 
			PcPickupMode.Nearest => LOC.MM.GetStart("pc_pickup_mode_nearest"), 
			PcPickupMode.Best => LOC.MM.GetStart("pc_pickup_mode_best"), 
			_ => throw new ArgumentOutOfRangeException("mode", mode, null), 
		};
	}

	public bool TryPickup(PcPickupMode mode)
	{
		if (!CanRun(mode))
		{
			return false;
		}
		BuildCandidates();
		if (candidates.Count == 0)
		{
			return false;
		}
		IInteractable interactable = SelectTarget(mode);
		if (interactable == null)
		{
			return false;
		}
		if (!interactable.CanInteract())
		{
			return false;
		}
		if (interactable.Type == InteractionType.Item)
		{
			interactable.OnLeftClick();
		}
		else
		{
			interactable.Interact();
		}
		return true;
	}

	private bool CanRun(PcPickupMode mode)
	{
		if (mode == PcPickupMode.Off)
		{
			return false;
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return false;
		}
		if (!SingletonMonoScope<PlayerManager>.HasInstance || !SingletonMonoScope<PlayerManager>.Instance.IsAlive)
		{
			return false;
		}
		if (!SingletonMonoScope<InteractionManager>.HasInstance || !InteractionManager.CanInteractNow())
		{
			return false;
		}
		if ((bool)Hand.Instance && Hand.Instance.isDragItem)
		{
			return false;
		}
		if (!playerTf && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			playerTf = SingletonMonoScope<PlayerManager>.Instance.transform;
		}
		return playerTf;
	}

	private void BuildCandidates()
	{
		candidates.Clear();
		int num = Physics2D.OverlapCircleNonAlloc(playerTf.position, scanRadius, overlapResults, interactLayer);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = overlapResults[i];
			if ((bool)collider2D)
			{
				IInteractable component = collider2D.GetComponent<IInteractable>();
				if (component != null && IsPickupCandidateValid(component) && !candidates.Contains(component))
				{
					candidates.Add(component);
				}
			}
		}
	}

	private bool IsPickupCandidateValid(IInteractable target)
	{
		if (target == null)
		{
			return false;
		}
		if (target is UnityEngine.Object @object && !@object)
		{
			return false;
		}
		if (target.Type != InteractionType.Item && target.Type != InteractionType.Temple && target.Type != InteractionType.Chest)
		{
			return false;
		}
		if (!target.CanInteract())
		{
			return false;
		}
		MonoBehaviour monoBehaviour = target as MonoBehaviour;
		if (!monoBehaviour)
		{
			return false;
		}
		if (!monoBehaviour.isActiveAndEnabled)
		{
			return false;
		}
		if (((Vector2)monoBehaviour.transform.position - (Vector2)playerTf.position).sqrMagnitude > scanRadius * scanRadius)
		{
			return false;
		}
		return true;
	}

	private IInteractable SelectTarget(PcPickupMode mode)
	{
		return mode switch
		{
			PcPickupMode.Nearest => SelectNearestTarget(), 
			PcPickupMode.Best => SelectBestTarget(), 
			_ => null, 
		};
	}

	private IInteractable SelectNearestTarget()
	{
		IInteractable result = null;
		float num = float.MaxValue;
		Vector2 vector = playerTf.position;
		for (int i = 0; i < candidates.Count; i++)
		{
			IInteractable interactable = candidates[i];
			if (interactable == null)
			{
				continue;
			}
			MonoBehaviour monoBehaviour = interactable as MonoBehaviour;
			if ((bool)monoBehaviour)
			{
				float sqrMagnitude = ((Vector2)monoBehaviour.transform.position - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = interactable;
				}
			}
		}
		return result;
	}

	private IInteractable SelectBestTarget()
	{
		IInteractable interactable = null;
		Vector2 bPos = default(Vector2);
		Vector2 centerPos = playerTf.position;
		for (int i = 0; i < candidates.Count; i++)
		{
			IInteractable interactable2 = candidates[i];
			if (interactable2 == null)
			{
				continue;
			}
			MonoBehaviour monoBehaviour = interactable2 as MonoBehaviour;
			if ((bool)monoBehaviour)
			{
				Vector2 vector = monoBehaviour.transform.position;
				if (IsBetterPickupTarget(interactable2, vector, interactable, bPos, centerPos))
				{
					interactable = interactable2;
					bPos = vector;
				}
			}
		}
		return interactable;
	}

	private static bool IsBetterPickupTarget(IInteractable a, Vector2 aPos, IInteractable b, Vector2 bPos, Vector2 centerPos)
	{
		if (b == null)
		{
			return true;
		}
		int priority = a.Priority;
		int priority2 = b.Priority;
		if (priority != priority2)
		{
			return priority > priority2;
		}
		float num = Vector2.SqrMagnitude(aPos - centerPos);
		float num2 = Vector2.SqrMagnitude(bPos - centerPos);
		return num < num2;
	}

	private void OnDrawGizmosSelected()
	{
		if (drawGizmos)
		{
			Transform transform = playerTf;
			if (!transform && SingletonMonoScope<PlayerManager>.HasInstance)
			{
				transform = SingletonMonoScope<PlayerManager>.Instance.transform;
			}
			if ((bool)transform)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position, scanRadius);
			}
		}
	}
}
