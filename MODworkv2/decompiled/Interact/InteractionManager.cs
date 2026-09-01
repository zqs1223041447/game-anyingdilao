using System.Collections;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Cursors;
using UnityEngine;

namespace Interact;

public class InteractionManager : SingletonMonoScope<InteractionManager>
{
	[Header("Raycast")]
	public LayerMask interactLayer;

	public static bool AllInteractToggle = true;

	private IInteractable cursorHover;

	private IInteractable keyHover;

	public Camera mainCam;

	private static readonly Collider2D[] overlapResults = new Collider2D[12];

	private Coroutine recoverInteractCoroutine;

	private static bool _blockLeftInteractUntilRelease;

	private static bool _blockRightInteractUntilRelease;

	private static bool _blockSubmitInteractUntilRelease;

	private static bool _blockCancelInteractUntilRelease;

	public IInteractable CurrentTarget
	{
		get
		{
			if (!IsCursorMode)
			{
				return keyHover;
			}
			return cursorHover;
		}
	}

	public static WorldInteractMode CurrentMode
	{
		get
		{
			if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
			{
				return WorldInteractMode.Cursor;
			}
			if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				if (ShouldUseCursorModeInGamepad())
				{
					return WorldInteractMode.Cursor;
				}
				return WorldInteractMode.Key;
			}
			return WorldInteractMode.Cursor;
		}
	}

	public static bool IsCursorMode => CurrentMode == WorldInteractMode.Cursor;

	public static bool IsKeyMode => CurrentMode == WorldInteractMode.Key;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	private void Start()
	{
		if (!mainCam && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			mainCam = SingletonMonoScope<PlayerManager>.Instance.mainCam;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			CurrentInputManager.OnCurrentInputDeviceChanged += HandleInputDeviceChanged;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			CurrentInputManager.OnCurrentInputDeviceChanged -= HandleInputDeviceChanged;
		}
	}

	private void Update()
	{
		if (IsCursorMode)
		{
			ClearKeyHover();
			IInteractable target = DetectBestCursorTarget();
			UpdateCursorHover(target);
		}
		else
		{
			ClearCursorHover();
		}
	}

	private void HandleInputDeviceChanged(InputDeviceType deviceType)
	{
		if (CurrentInputManager.IsGamepad(deviceType))
		{
			ClearCursorHover();
		}
		else
		{
			ClearKeyHover();
		}
	}

	private static bool ShouldUseCursorModeInGamepad()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return false;
		}
		if (!SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			return false;
		}
		return CursorInputManager.IsUsingVirtualMouse;
	}

	private IInteractable DetectBestCursorTarget()
	{
		if (!mainCam || !CanInteractNow())
		{
			return null;
		}
		Vector2 mouseWorld = SingletonMonoScope<CursorInputManager>.Instance.WorldPosition;
		int num = CollectInteractables(mouseWorld, overlapResults);
		if (num <= 0)
		{
			return null;
		}
		return SelectBest(overlapResults, num, mouseWorld);
	}

	private int CollectInteractables(Vector2 mouseWorld, Collider2D[] results)
	{
		return Physics2D.OverlapPointNonAlloc(mouseWorld, results, interactLayer);
	}

	public static bool CanInteractNow()
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance || !SingletonMonoScope<PlayerManager>.Instance.IsAlive)
		{
			return false;
		}
		if (!AllInteractToggle)
		{
			return false;
		}
		if (Time.timeScale == 0f)
		{
			return false;
		}
		if (IsKeyMode && SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.IsAnyPanelOpened())
		{
			return false;
		}
		if (IsBlockedByPendingRelease())
		{
			return false;
		}
		return true;
	}

	private static IInteractable SelectBest(Collider2D[] hits, int hitCount, Vector2 mouseWorld)
	{
		IInteractable interactable = null;
		Vector2 bPos = default(Vector2);
		for (int i = 0; i < hitCount; i++)
		{
			Collider2D collider2D = hits[i];
			if (!collider2D)
			{
				continue;
			}
			IInteractable component = collider2D.GetComponent<IInteractable>();
			if (component != null)
			{
				Vector2 vector = collider2D.transform.position;
				if (IsBetter(component, vector, interactable, bPos, mouseWorld))
				{
					interactable = component;
					bPos = vector;
				}
			}
		}
		return interactable;
	}

	private static bool IsBetter(IInteractable a, Vector2 aPos, IInteractable b, Vector2 bPos, Vector2 mouseWorld)
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
		float num = Vector2.SqrMagnitude(aPos - mouseWorld);
		float num2 = Vector2.SqrMagnitude(bPos - mouseWorld);
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

	private void UpdateCursorHover(IInteractable target)
	{
		if (!CanInteractNow())
		{
			ClearCursorHover();
			return;
		}
		if (target != null && !target.CanHover())
		{
			target = null;
		}
		if (target != cursorHover)
		{
			cursorHover?.OnHoverExit();
			target?.OnHoverEnter();
			cursorHover = target;
		}
	}

	private void ClearCursorHover()
	{
		if (cursorHover != null)
		{
			cursorHover.OnHoverExit();
			cursorHover = null;
		}
	}

	public void SetGamepadTarget(IInteractable target)
	{
		if (!CanInteractNow())
		{
			ClearKeyHover();
			return;
		}
		if (target != null && !target.CanHover())
		{
			target = null;
		}
		if (target != keyHover)
		{
			keyHover?.OnHoverExit();
			target?.OnHoverEnter();
			keyHover = target;
		}
	}

	public void ClearGamepadTarget(IInteractable target = null)
	{
		if (target == null || keyHover == target)
		{
			ClearKeyHover();
		}
	}

	private void ClearKeyHover()
	{
		if (keyHover != null)
		{
			keyHover.OnHoverExit();
			keyHover = null;
		}
	}

	public void ClearHover(IInteractable target)
	{
		if (cursorHover == target)
		{
			cursorHover.OnHoverExit();
			cursorHover = null;
		}
		if (keyHover == target)
		{
			keyHover.OnHoverExit();
			keyHover = null;
		}
	}

	public void ClearAllHover()
	{
		ClearCursorHover();
		ClearKeyHover();
	}

	public void LockInteractTemporarily(float delay)
	{
		AllInteractToggle = false;
		if (recoverInteractCoroutine != null)
		{
			StopCoroutine(recoverInteractCoroutine);
		}
		recoverInteractCoroutine = StartCoroutine(CoRecoverInteract(delay));
	}

	private IEnumerator CoRecoverInteract(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		AllInteractToggle = true;
		recoverInteractCoroutine = null;
	}

	public static void BlockInteractUntilRelease(bool left, bool right, bool submit, bool cancel)
	{
		if (left)
		{
			_blockLeftInteractUntilRelease = Input.GetMouseButton(0);
		}
		if (right)
		{
			_blockRightInteractUntilRelease = Input.GetMouseButton(1);
		}
		if (submit)
		{
			_blockSubmitInteractUntilRelease = GamepadInputManager.GetSubmit();
		}
		if (cancel)
		{
			_blockCancelInteractUntilRelease = GamepadInputManager.GetCancel();
		}
	}

	private static bool IsBlockedByPendingRelease()
	{
		if (_blockLeftInteractUntilRelease)
		{
			if (Input.GetMouseButton(0))
			{
				return true;
			}
			_blockLeftInteractUntilRelease = false;
		}
		if (_blockRightInteractUntilRelease)
		{
			if (Input.GetMouseButton(1))
			{
				return true;
			}
			_blockRightInteractUntilRelease = false;
		}
		if (_blockSubmitInteractUntilRelease)
		{
			if (GamepadInputManager.GetSubmit())
			{
				return true;
			}
			_blockSubmitInteractUntilRelease = false;
		}
		if (_blockCancelInteractUntilRelease)
		{
			if (GamepadInputManager.GetCancel())
			{
				return true;
			}
			_blockCancelInteractUntilRelease = false;
		}
		return false;
	}

	public static void ClearPendingReleaseBlocks()
	{
		_blockLeftInteractUntilRelease = false;
		_blockRightInteractUntilRelease = false;
		_blockSubmitInteractUntilRelease = false;
		_blockCancelInteractUntilRelease = false;
	}
}
