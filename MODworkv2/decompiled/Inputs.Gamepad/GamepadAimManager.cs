using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using UnityEngine;

namespace Inputs.Gamepad;

public class GamepadAimManager : SingletonMonoScope<GamepadAimManager>
{
	[Header("施法参数")]
	[SerializeField]
	private float castAimRadius = 2.5f;

	[SerializeField]
	private float rightStickDeadZone = 0.2f;

	[Header("无输入时是否保留最近方向")]
	[SerializeField]
	private bool keepLastDirection = true;

	[Header("瞄准点 UI 显示")]
	public static bool showAimPointImage = true;

	[SerializeField]
	private Canvas aimUiCanvas;

	[SerializeField]
	private RectTransform aimPointImageRect;

	[SerializeField]
	private Vector2 aimPointOffset = Vector2.zero;

	private Camera mainCam;

	private Vector2 currentAimDirection = Vector2.right;

	private bool hasValidAimDirection;

	private GameObject aimPointVisualObject;

	private SpriteRenderer aimPointVisualRenderer;

	public Vector2 CurrentAimDirection => currentAimDirection;

	public bool HasValidAimDirection => hasValidAimDirection;

	public Vector3 CurrentAimWorldPoint
	{
		get
		{
			Vector3 playerPosition = GetPlayerPosition();
			Vector2 resolvedAimDirection = GetResolvedAimDirection();
			return playerPosition + new Vector3(resolvedAimDirection.x, resolvedAimDirection.y, 0f) * castAimRadius;
		}
	}

	public static bool IsUsingGamepadAim
	{
		get
		{
			if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				return false;
			}
			if (SingletonMonoGlobal<VirtualCursorManager>.HasInstance && SingletonMonoGlobal<VirtualCursorManager>.Instance.ShouldUseVirtualCursor)
			{
				return false;
			}
			if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.IsAnyPanelOpened())
			{
				return false;
			}
			if (!SingletonMonoScope<PlayerManager>.HasInstance || !SingletonMonoScope<PlayerManager>.Instance.IsAlive)
			{
				return false;
			}
			if (Time.timeScale == 0f)
			{
				return false;
			}
			return true;
		}
	}

	public static void SetAimPointImage(bool show)
	{
		showAimPointImage = show;
	}

	private void Start()
	{
		if (!mainCam)
		{
			mainCam = Camera.main;
		}
		if (!aimUiCanvas && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			aimUiCanvas = SingletonMonoScope<GameUIManager>.Instance.transform.Find("UICanvas").GetComponent<Canvas>();
		}
		if (!aimPointImageRect && (bool)aimUiCanvas)
		{
			aimPointImageRect = aimUiCanvas.transform.Find("AimPointImage").GetComponent<RectTransform>();
		}
		if (Singleton<SettingDataManager>.Instance.Game != null)
		{
			SetAimPointImage(Singleton<SettingDataManager>.Instance.Interface.aim_point);
		}
	}

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	private void Update()
	{
		if (IsUsingGamepadAim)
		{
			UpdateAimDirection();
		}
		UpdateAimPointImage();
	}

	private void UpdateAimDirection()
	{
		float rightStickXRaw = GamepadInputManager.GetRightStickXRaw();
		float rightStickYRaw = GamepadInputManager.GetRightStickYRaw();
		Vector2 vector = new Vector2(rightStickXRaw, rightStickYRaw);
		if (vector.magnitude < rightStickDeadZone)
		{
			if (!keepLastDirection)
			{
				hasValidAimDirection = false;
			}
		}
		else
		{
			currentAimDirection = vector.normalized;
			hasValidAimDirection = true;
		}
	}

	public Vector2 GetResolvedAimDirection()
	{
		if (hasValidAimDirection)
		{
			return currentAimDirection;
		}
		return Vector2.right;
	}

	public AimContext GetAimContext()
	{
		Vector2 resolvedAimDirection = GetResolvedAimDirection();
		AimContext result = default(AimContext);
		result.WorldPoint = CurrentAimWorldPoint;
		result.Direction = resolvedAimDirection;
		result.HasDirection = hasValidAimDirection;
		result.HasTargetPoint = true;
		result.IsGamepad = IsUsingGamepadAim;
		return result;
	}

	private static Vector3 GetPlayerPosition()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
			if ((bool)instance.yao && (bool)instance.yao.transform)
			{
				return instance.yao.transform.position;
			}
			if ((bool)instance.transform)
			{
				return instance.transform.position;
			}
		}
		return Vector3.zero;
	}

	private void UpdateAimPointImage()
	{
		if (!aimPointImageRect || !aimUiCanvas)
		{
			return;
		}
		bool flag = showAimPointImage && IsUsingGamepadAim;
		aimPointImageRect.gameObject.SetActive(flag);
		if (!flag || !mainCam)
		{
			return;
		}
		Vector3 currentAimWorldPoint = CurrentAimWorldPoint;
		Vector3 vector = mainCam.WorldToScreenPoint(currentAimWorldPoint);
		if (vector.z < 0f)
		{
			aimPointImageRect.gameObject.SetActive(value: false);
			return;
		}
		RectTransform rectTransform = aimUiCanvas.transform as RectTransform;
		if ((bool)rectTransform)
		{
			Camera cam = ((aimUiCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : aimUiCanvas.worldCamera);
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, vector, cam, out var localPoint))
			{
				aimPointImageRect.anchoredPosition = localPoint + aimPointOffset;
			}
		}
	}
}
