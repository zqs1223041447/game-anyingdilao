using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;

namespace Interact;

public class GamepadInteractTargetManager : SingletonMonoScope<GamepadInteractTargetManager>
{
	[Header("扫描设置")]
	[SerializeField]
	private float scanRadius = 3.2f;

	[SerializeField]
	private LayerMask interactLayer;

	[SerializeField]
	private float rescanInterval = 0.12f;

	[Header("调试")]
	[SerializeField]
	private bool drawGizmos = true;

	private readonly List<IInteractable> candidates = new List<IInteractable>(16);

	private static readonly Collider2D[] overlapResults = new Collider2D[24];

	private float lastScanTime = -999f;

	private Transform playerTf;

	private IInteractable currentTarget;

	[SerializeField]
	private float rightStickDeadZone = 0.45f;

	[SerializeField]
	private float switchCooldown = 0.15f;

	[SerializeField]
	private float directionDotThreshold = 0.35f;

	private float lastSwitchTime = -999f;

	private bool stickReleased = true;

	public IInteractable CurrentTarget => currentTarget;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	private void OnEnable()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			CurrentInputManager.OnCurrentInputDeviceChanged += HandleInputDeviceChanged;
		}
	}

	private void OnDisable()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			CurrentInputManager.OnCurrentInputDeviceChanged -= HandleInputDeviceChanged;
		}
		ClearCurrentTarget();
	}

	private void HandleInputDeviceChanged(InputDeviceType deviceType)
	{
		ClearCurrentTarget();
	}

	private void Start()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			playerTf = SingletonMonoScope<PlayerManager>.Instance.gameObject.transform;
		}
		if (interactLayer.value == 0 && SingletonMonoScope<InteractionManager>.HasInstance)
		{
			interactLayer = SingletonMonoScope<InteractionManager>.Instance.interactLayer;
		}
	}

	private void Update()
	{
		if (!ShouldRun())
		{
			ClearCurrentTarget();
			stickReleased = true;
			return;
		}
		if (NeedRefresh())
		{
			RefreshTarget();
		}
		HandleRightStickSwitch();
	}

	private void HandleRightStickSwitch()
	{
		Vector2 vector = ReadRightStick();
		if (vector == Vector2.zero)
		{
			stickReleased = true;
		}
		else if (stickReleased && !(Time.time - lastSwitchTime < switchCooldown))
		{
			stickReleased = false;
			lastSwitchTime = Time.time;
			TrySwitchTarget(vector);
		}
	}

	private Vector2 ReadRightStick()
	{
		float rightStickXRaw = GamepadInputManager.GetRightStickXRaw();
		float rightStickYRaw = GamepadInputManager.GetRightStickYRaw();
		Vector2 vector = new Vector2(rightStickXRaw, rightStickYRaw);
		if (vector.sqrMagnitude < rightStickDeadZone * rightStickDeadZone)
		{
			return Vector2.zero;
		}
		return vector.normalized;
	}

	private void TrySwitchTarget(Vector2 inputDir)
	{
		BuildCandidates();
		if (candidates.Count == 0)
		{
			SetCurrentTarget(null);
			return;
		}
		IInteractable interactable = ((currentTarget != null) ? FindBestTargetFromCurrent(inputDir) : FindBestTargetFromPlayer(inputDir));
		if (interactable != null)
		{
			SetCurrentTarget(interactable);
		}
	}

	private IInteractable FindBestTargetFromCurrent(Vector2 inputDir)
	{
		if (currentTarget == null)
		{
			return null;
		}
		MonoBehaviour monoBehaviour = currentTarget as MonoBehaviour;
		if (!monoBehaviour)
		{
			return null;
		}
		Vector2 vector = monoBehaviour.transform.position;
		IInteractable result = null;
		float num = float.MinValue;
		for (int i = 0; i < candidates.Count; i++)
		{
			IInteractable interactable = candidates[i];
			if (interactable == null || interactable == currentTarget)
			{
				continue;
			}
			MonoBehaviour monoBehaviour2 = interactable as MonoBehaviour;
			if (!monoBehaviour2)
			{
				continue;
			}
			Vector2 vector2 = (Vector2)monoBehaviour2.transform.position - vector;
			float magnitude = vector2.magnitude;
			if (magnitude <= 0.001f)
			{
				continue;
			}
			Vector2 rhs = vector2 / magnitude;
			float num2 = Vector2.Dot(inputDir, rhs);
			if (!(num2 < directionDotThreshold))
			{
				int finalPriority = GetFinalPriority(interactable);
				float num3 = num2 * 1000f + (float)finalPriority * 0.1f - magnitude * 2f;
				if (num3 > num)
				{
					num = num3;
					result = interactable;
				}
			}
		}
		return result;
	}

	private IInteractable FindBestTargetFromPlayer(Vector2 inputDir)
	{
		Vector2 vector = playerTf.position;
		IInteractable interactable = null;
		float num = float.MinValue;
		for (int i = 0; i < candidates.Count; i++)
		{
			IInteractable interactable2 = candidates[i];
			if (interactable2 == null)
			{
				continue;
			}
			MonoBehaviour monoBehaviour = interactable2 as MonoBehaviour;
			if (!monoBehaviour)
			{
				continue;
			}
			Vector2 vector2 = (Vector2)monoBehaviour.transform.position - vector;
			float magnitude = vector2.magnitude;
			if (magnitude <= 0.001f)
			{
				continue;
			}
			Vector2 rhs = vector2 / magnitude;
			float num2 = Vector2.Dot(inputDir, rhs);
			if (!(num2 < directionDotThreshold))
			{
				int finalPriority = GetFinalPriority(interactable2);
				float num3 = num2 * 1000f + (float)finalPriority * 0.1f - magnitude * 2f;
				if (num3 > num)
				{
					num = num3;
					interactable = interactable2;
				}
			}
		}
		if (interactable != null)
		{
			return interactable;
		}
		return SelectBestTarget();
	}

	private bool ShouldRun()
	{
		if (!SingletonMonoScope<InteractionManager>.HasInstance)
		{
			return false;
		}
		if (!InteractionManager.IsKeyMode)
		{
			return false;
		}
		if (!InteractionManager.CanInteractNow())
		{
			return false;
		}
		if (!SingletonMonoScope<PlayerManager>.HasInstance || !SingletonMonoScope<PlayerManager>.Instance.IsAlive)
		{
			return false;
		}
		if (!playerTf && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			playerTf = SingletonMonoScope<PlayerManager>.Instance.transform;
		}
		return playerTf;
	}

	private bool NeedRefresh()
	{
		if (currentTarget == null)
		{
			return true;
		}
		if (SingletonMonoScope<InteractionManager>.HasInstance && SingletonMonoScope<InteractionManager>.Instance.CurrentTarget != currentTarget)
		{
			return true;
		}
		if (!IsTargetValid(currentTarget))
		{
			return true;
		}
		if (Time.time - lastScanTime >= rescanInterval)
		{
			return true;
		}
		return false;
	}

	private void RefreshTarget()
	{
		lastScanTime = Time.time;
		BuildCandidates();
		if (currentTarget != null && IsTargetValid(currentTarget) && candidates.Contains(currentTarget))
		{
			SyncCurrentTargetToInteractionManager();
		}
		else
		{
			SetCurrentTarget(SelectBestTarget());
		}
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
				if (component != null && IsTargetValid(component) && !candidates.Contains(component))
				{
					candidates.Add(component);
				}
			}
		}
	}

	private IInteractable SelectBestTarget()
	{
		if (candidates.Count == 0)
		{
			return null;
		}
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
				if (IsBetter(interactable2, vector, interactable, bPos, centerPos))
				{
					interactable = interactable2;
					bPos = vector;
				}
			}
		}
		return interactable;
	}

	private static bool IsBetter(IInteractable a, Vector2 aPos, IInteractable b, Vector2 bPos, Vector2 centerPos)
	{
		if (b == null)
		{
			return true;
		}
		int finalPriority = GetFinalPriority(a);
		int finalPriority2 = GetFinalPriority(b);
		if (finalPriority != finalPriority2)
		{
			return finalPriority > finalPriority2;
		}
		float num = Vector2.SqrMagnitude(aPos - centerPos);
		float num2 = Vector2.SqrMagnitude(bPos - centerPos);
		return num < num2;
	}

	private static int GetFinalPriority(IInteractable target)
	{
		if (target == null)
		{
			return int.MinValue;
		}
		return (int)target.Type * 1000 + target.Priority;
	}

	private bool IsTargetValid(IInteractable target)
	{
		if (target == null)
		{
			return false;
		}
		if (target is Object @object && !@object)
		{
			return false;
		}
		if (!target.CanHover())
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

	private void SetCurrentTarget(IInteractable target)
	{
		if (currentTarget == target)
		{
			SyncCurrentTargetToInteractionManager();
			return;
		}
		IInteractable interactable = currentTarget;
		currentTarget = target;
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			if (interactable != null)
			{
				SingletonMonoScope<InteractionManager>.Instance.ClearGamepadTarget(interactable);
			}
			if (currentTarget != null)
			{
				SingletonMonoScope<InteractionManager>.Instance.SetGamepadTarget(currentTarget);
			}
		}
	}

	private void SyncCurrentTargetToInteractionManager()
	{
		if (SingletonMonoScope<InteractionManager>.HasInstance && currentTarget != null && SingletonMonoScope<InteractionManager>.Instance.CurrentTarget != currentTarget)
		{
			SingletonMonoScope<InteractionManager>.Instance.SetGamepadTarget(currentTarget);
		}
	}

	public void ClearCurrentTarget()
	{
		if (currentTarget != null)
		{
			IInteractable target = currentTarget;
			currentTarget = null;
			if (SingletonMonoScope<InteractionManager>.HasInstance)
			{
				SingletonMonoScope<InteractionManager>.Instance.ClearGamepadTarget(target);
			}
		}
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
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(transform.position, scanRadius);
			}
		}
	}
}
