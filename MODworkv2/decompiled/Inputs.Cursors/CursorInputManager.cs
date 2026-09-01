using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Inputs.Cursors;

public class CursorInputManager : SingletonMonoScope<CursorInputManager>
{
	[Header("引用")]
	[SerializeField]
	private Camera targetCamera;

	[Header("手柄虚拟鼠标")]
	private const float ReferenceScreenWidth = 3840f;

	private const float ReferenceScreenHeight = 2160f;

	private const float BaseMoveSpeedMultiplier = 100f;

	private const float AccelerationStartTime = 0.3f;

	private const float AccelerationFullTime = 1.5f;

	private const float MaxAccelerationMultiplier = 1.8f;

	private static float baseMoveSpeed = 12f;

	private static int actualMoveSpeed = 1200;

	private static int cachedScreenWidth = -1;

	private static int cachedScreenHeight = -1;

	[SerializeField]
	private float deadZone = 0.05f;

	[Header("手柄按键映射")]
	[SerializeField]
	private KeyCode gamepadLeftClickKey = KeyCode.JoystickButton0;

	[SerializeField]
	private KeyCode gamepadRightClickKey = KeyCode.JoystickButton1;

	private Vector3 virtualMouseScreenPos;

	private Vector3 lastHiddenScreenPos;

	private bool hasLastHiddenScreenPos;

	private bool isMoving;

	private float movementStartTime;

	public static int ActualMoveSpeed => actualMoveSpeed;

	public Vector3 VirtualScreenPosition => virtualMouseScreenPos;

	public Vector3 ScreenPosition { get; private set; }

	public Vector3 WorldPosition { get; private set; }

	public bool LeftButtonDown { get; private set; }

	public bool LeftButton { get; private set; }

	public bool LeftButtonUp { get; private set; }

	public bool RightButtonDown { get; private set; }

	public bool RightButton { get; private set; }

	public bool RightButtonUp { get; private set; }

	public static bool IsUsingVirtualMouse
	{
		get
		{
			if (SingletonMonoGlobal<VirtualCursorManager>.HasInstance)
			{
				return SingletonMonoGlobal<VirtualCursorManager>.Instance.ShouldUseVirtualCursor;
			}
			return false;
		}
	}

	public static void SetMoveSpeed(float speed)
	{
		baseMoveSpeed = Mathf.Clamp(speed, 2f, 20f) * 100f;
		RefreshActualMoveSpeed();
	}

	private void Start()
	{
		if (!targetCamera)
		{
			targetCamera = Camera.main;
		}
		if (Singleton<SettingDataManager>.Instance.Game != null)
		{
			SetMoveSpeed(Singleton<SettingDataManager>.Instance.Interface.cursor_speed);
		}
	}

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		if (!targetCamera)
		{
			targetCamera = Camera.main;
		}
		virtualMouseScreenPos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		ScreenPosition = virtualMouseScreenPos;
		VirtualCursorManager.OnVirtualCursorStateChanged += HandleVirtualCursorStateChanged;
	}

	protected override void OnDestroy()
	{
		VirtualCursorManager.OnVirtualCursorStateChanged -= HandleVirtualCursorStateChanged;
		base.OnDestroy();
	}

	private void HandleVirtualCursorStateChanged(bool enable)
	{
		if (!enable)
		{
			lastHiddenScreenPos = virtualMouseScreenPos;
			hasLastHiddenScreenPos = true;
			return;
		}
		if (hasLastHiddenScreenPos)
		{
			ForceSetScreenPosition(lastHiddenScreenPos);
		}
		else
		{
			ForceSetScreenPosition(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
		}
		if (SingletonMonoGlobal<CursorManager>.HasInstance)
		{
			SingletonMonoGlobal<CursorManager>.Instance.RefreshCursorStateImmediate(CursorManager.GetCurrentPointerScreenPosition());
		}
	}

	private void Update()
	{
		UpdatePosition();
		UpdateButtons();
		UpdateWorldPosition();
	}

	private void UpdatePosition()
	{
		if (IsUsingVirtualMouse)
		{
			RefreshActualMoveSpeedIfResolutionChanged();
			float rightStickXRaw = GamepadInputManager.GetRightStickXRaw();
			float rightStickYRaw = GamepadInputManager.GetRightStickYRaw();
			Vector2 vector = new Vector2(rightStickXRaw, rightStickYRaw);
			float magnitude = vector.magnitude;
			Vector2 vector2;
			if (magnitude < deadZone)
			{
				vector2 = Vector2.zero;
			}
			else
			{
				Vector2 normalized = vector.normalized;
				float num = Mathf.InverseLerp(deadZone, 1f, Mathf.Clamp01(magnitude));
				vector2 = normalized * num;
			}
			float accelerationMultiplier = GetAccelerationMultiplier(vector2.sqrMagnitude > 0f);
			float num2 = (float)actualMoveSpeed * accelerationMultiplier;
			virtualMouseScreenPos += new Vector3(vector2.x * num2 * Time.unscaledDeltaTime, vector2.y * num2 * Time.unscaledDeltaTime, 0f);
			virtualMouseScreenPos.x = Mathf.Clamp(virtualMouseScreenPos.x, 0f, Screen.width);
			virtualMouseScreenPos.y = Mathf.Clamp(virtualMouseScreenPos.y, 0f, Screen.height);
			ScreenPosition = virtualMouseScreenPos;
		}
		else
		{
			ScreenPosition = Input.mousePosition;
		}
	}

	private static void RefreshActualMoveSpeedIfResolutionChanged()
	{
		if (cachedScreenWidth != Screen.width || cachedScreenHeight != Screen.height)
		{
			RefreshActualMoveSpeed();
		}
	}

	private static void RefreshActualMoveSpeed()
	{
		cachedScreenWidth = Screen.width;
		cachedScreenHeight = Screen.height;
		if (cachedScreenWidth <= 0 || cachedScreenHeight <= 0)
		{
			actualMoveSpeed = Mathf.RoundToInt(baseMoveSpeed);
			return;
		}
		float a = (float)cachedScreenWidth / 3840f;
		float b = (float)cachedScreenHeight / 2160f;
		float num = Mathf.Clamp01(Mathf.Min(a, b));
		actualMoveSpeed = Mathf.Max(1, Mathf.RoundToInt(baseMoveSpeed * num));
	}

	private float GetAccelerationMultiplier(bool hasMovementInput)
	{
		if (!hasMovementInput)
		{
			isMoving = false;
			return 1f;
		}
		if (!isMoving)
		{
			isMoving = true;
			movementStartTime = Time.unscaledTime;
			return 1f;
		}
		float num = Time.unscaledTime - movementStartTime;
		if (num <= 0.3f)
		{
			return 1f;
		}
		float t = Mathf.InverseLerp(0.3f, 1.5f, num);
		return Mathf.Lerp(1f, 1.8f, t);
	}

	private void UpdateButtons()
	{
		if (IsUsingVirtualMouse)
		{
			LeftButtonDown = GamepadInputManager.GetSubmitDown() || Input.GetKeyDown(gamepadLeftClickKey);
			LeftButton = GamepadInputManager.GetSubmit() || Input.GetKey(gamepadLeftClickKey);
			LeftButtonUp = GamepadInputManager.GetSubmitUp() || Input.GetKeyUp(gamepadLeftClickKey);
			RightButtonDown = GamepadInputManager.GetCancelDown() || Input.GetKeyDown(gamepadRightClickKey);
			RightButton = GamepadInputManager.GetCancel() || Input.GetKey(gamepadRightClickKey);
			RightButtonUp = GamepadInputManager.GetCancelUp() || Input.GetKeyUp(gamepadRightClickKey);
		}
		else
		{
			LeftButtonDown = Input.GetMouseButtonDown(0);
			LeftButton = Input.GetMouseButton(0);
			LeftButtonUp = Input.GetMouseButtonUp(0);
			RightButtonDown = Input.GetMouseButtonDown(1);
			RightButton = Input.GetMouseButton(1);
			RightButtonUp = Input.GetMouseButtonUp(1);
		}
	}

	private void UpdateWorldPosition()
	{
		if (!targetCamera && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			targetCamera = SingletonMonoScope<PlayerManager>.Instance.mainCam;
			if (!targetCamera)
			{
				return;
			}
		}
		Vector3 worldPosition = targetCamera.ScreenToWorldPoint(ScreenPosition);
		worldPosition.z = 0f;
		WorldPosition = worldPosition;
	}

	public void ForceSetScreenPosition(Vector3 screenPos)
	{
		screenPos.z = 0f;
		screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
		screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);
		virtualMouseScreenPos = screenPos;
		ScreenPosition = screenPos;
	}
}
