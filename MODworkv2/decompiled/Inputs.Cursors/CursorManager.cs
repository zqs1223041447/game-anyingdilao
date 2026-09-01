using System.Collections.Generic;
using Core.Settings;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inputs.Cursors;

public class CursorManager : SingletonMonoGlobal<CursorManager>
{
	private const float LargeSourceBaseScale = 0.8f;

	private LayerMask interactLayers;

	public CursorState currentState;

	private CursorSizeTier currentSizeTier = CursorSizeTier.Medium;

	private float currentScale = 1f;

	private Camera mainCam;

	private Texture2D currentTexture;

	private Texture2D scaledUiTexture;

	private Texture2D scaledAimTexture;

	private Vector2 currentCursorSize;

	private static readonly List<RaycastResult> results = new List<RaycastResult>(16);

	private static PointerEventData cachedPointerEventData;

	private Vector3 lastPointerScreenPos;

	private bool hasLastPointerPos;

	private bool lastCursorVisible;

	private Texture2D lastAppliedTexture;

	private Vector2 lastAppliedHotspot;

	private bool lastIsGamepad;

	private static bool globalForceHidden;

	private static Texture2D hiddenCursorTexture;

	private float nextStateCheckTime;

	[Header("鼠标检测状态间隔时间")]
	public float stateCheckInterval = 0.02f;

	[Header("Hotspot 调试")]
	[SerializeField]
	private bool debugDrawHotspot;

	[SerializeField]
	private bool debugDrawCursorPreview;

	[SerializeField]
	private float debugPointSize = 8f;

	[SerializeField]
	private Vector2 debugPreviewOffset = new Vector2(30f, 30f);

	private static bool enableCursorManager => SettingsLoader.Instance.cursorSettings.enableCursorManager;

	private static bool enableCursorStateDetection => SettingsLoader.Instance.cursorSettings.enableCursorState;

	public Vector2 CurrentCursorSize => currentCursorSize;

	public float CurrentScale => currentScale;

	public float CurrentVisualScale => currentScale * 0.8f;

	public static bool IsUsingVirtualCursor
	{
		get
		{
			if (SingletonMonoScope<CursorInputManager>.HasInstance)
			{
				return CursorInputManager.IsUsingVirtualMouse;
			}
			return false;
		}
	}

	public Sprite CurrentCursorSprite => GetSourceSprite(currentState);

	public bool IsForceHidden => globalForceHidden;

	public void Init()
	{
		if (enableCursorManager)
		{
			GlobalSettings globalSettings = SettingsLoader.Instance;
			interactLayers = globalSettings.interactLayers;
			currentSizeTier = CursorSizeTier.Medium;
			currentScale = 1f;
			mainCam = Camera.main;
			currentState = (enableCursorStateDetection ? CursorState.Aim : CursorState.UI);
			ForceRefreshAll();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		CurrentInputManager.OnCurrentInputDeviceChanged += HandleInputDeviceChanged;
	}

	protected override void OnDestroy()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged -= HandleInputDeviceChanged;
		DestroyGeneratedTextures();
		base.OnDestroy();
	}

	private void Update()
	{
		if (!enableCursorManager)
		{
			return;
		}
		if (globalForceHidden)
		{
			if (Cursor.visible)
			{
				EnforceGlobalForceHidden();
			}
			lastCursorVisible = false;
			return;
		}
		if (!mainCam)
		{
			mainCam = Camera.main;
		}
		bool flag = SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent();
		if (flag != lastIsGamepad)
		{
			lastIsGamepad = flag;
			RefreshSystemCursorVisibleImmediate();
			ApplyCurrentState();
		}
		Vector3 currentPointerScreenPosition = GetCurrentPointerScreenPosition();
		if (hasLastPointerPos)
		{
			bool num = (currentPointerScreenPosition - lastPointerScreenPos).sqrMagnitude > 0.01f;
			bool flag2 = Time.unscaledTime >= nextStateCheckTime;
			if (!num && !flag2)
			{
				return;
			}
		}
		hasLastPointerPos = true;
		lastPointerScreenPos = currentPointerScreenPosition;
		nextStateCheckTime = Time.unscaledTime + stateCheckInterval;
		if (!enableCursorStateDetection)
		{
			SetState(CursorState.UI);
		}
		else
		{
			RefreshCursorStateImmediate(currentPointerScreenPosition);
		}
	}

	private void HandleInputDeviceChanged(InputDeviceType deviceType)
	{
		RefreshSystemCursorVisibleImmediate();
		ApplyCurrentState();
	}

	private void RefreshSystemCursorVisibleImmediate()
	{
		bool flag = SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent();
		bool flag2 = !globalForceHidden && !flag;
		if (lastCursorVisible != flag2 || Cursor.visible != flag2)
		{
			Cursor.visible = flag2;
			lastCursorVisible = flag2;
		}
		if (flag2 && (bool)currentTexture)
		{
			Vector2 hotspot = GetHotspot(currentTexture);
			if (lastAppliedTexture != currentTexture || lastAppliedHotspot != hotspot)
			{
				Cursor.SetCursor(currentTexture, hotspot, CursorMode.ForceSoftware);
				lastAppliedTexture = currentTexture;
				lastAppliedHotspot = hotspot;
			}
		}
	}

	public void SetState(CursorState state, bool force = false)
	{
		if (enableCursorManager && (force || currentState != state))
		{
			currentState = state;
			ApplyCurrentState();
		}
	}

	public void SetSizeTier(CursorSizeTier tier)
	{
		SetScale(tier switch
		{
			CursorSizeTier.Small => 0.5f, 
			CursorSizeTier.Large => 1f, 
			_ => 0.75f, 
		});
	}

	public void SetScale(float scale)
	{
		if (enableCursorManager)
		{
			float b = Mathf.Clamp(scale, 0.5f, 2f);
			if (!Mathf.Approximately(currentScale, b))
			{
				currentScale = b;
				currentSizeTier = ((!(currentScale < 0.875f)) ? ((!(currentScale > 1.125f)) ? CursorSizeTier.Medium : CursorSizeTier.Large) : CursorSizeTier.Small);
				DestroyGeneratedTextures();
				lastAppliedTexture = null;
				ApplyCurrentState();
			}
		}
	}

	public void SetForceHidden(bool hidden)
	{
		SetGlobalForceHidden(hidden);
	}

	public static void SetGlobalForceHidden(bool hidden)
	{
		globalForceHidden = hidden;
		if (SingletonMonoGlobal<CursorManager>.HasInstance)
		{
			SingletonMonoGlobal<CursorManager>.Instance.ApplyForceHiddenState();
			return;
		}
		if (hidden)
		{
			EnforceGlobalForceHidden();
			return;
		}
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		Cursor.visible = !SingletonMonoGlobal<CurrentInputManager>.HasInstance || SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent();
	}

	public static void EnforceGlobalForceHidden()
	{
		if (globalForceHidden)
		{
			Cursor.SetCursor(GetHiddenCursorTexture(), Vector2.zero, CursorMode.ForceSoftware);
			Cursor.visible = false;
		}
	}

	private void ApplyForceHiddenState()
	{
		hasLastPointerPos = false;
		if (globalForceHidden)
		{
			EnforceGlobalForceHidden();
			lastCursorVisible = false;
			lastAppliedTexture = hiddenCursorTexture;
			lastAppliedHotspot = Vector2.zero;
		}
		else
		{
			RefreshSystemCursorVisibleImmediate();
			ApplyCurrentState();
		}
	}

	public static Vector3 GetCurrentPointerScreenPosition()
	{
		if (IsUsingVirtualCursor && SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			return SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		}
		return Input.mousePosition;
	}

	public void ForceRefreshAll()
	{
		if (enableCursorManager)
		{
			hasLastPointerPos = false;
			RefreshSystemCursorVisibleImmediate();
			if (!enableCursorStateDetection)
			{
				SetState(CursorState.UI, force: true);
				return;
			}
			ApplyCurrentState();
			RefreshCursorStateImmediate(GetCurrentPointerScreenPosition());
		}
	}

	private void ApplyCurrentState()
	{
		if (!enableCursorManager)
		{
			return;
		}
		if (globalForceHidden)
		{
			EnforceGlobalForceHidden();
			lastCursorVisible = false;
			return;
		}
		Texture2D scaledTexture = GetScaledTexture(currentState);
		if (!scaledTexture)
		{
			return;
		}
		currentTexture = scaledTexture;
		currentCursorSize = new Vector2(scaledTexture.width, scaledTexture.height);
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			Vector2 hotspot = GetHotspot(scaledTexture);
			if (!(lastAppliedTexture == scaledTexture) || !(lastAppliedHotspot == hotspot) || !Cursor.visible)
			{
				Cursor.SetCursor(scaledTexture, hotspot, CursorMode.ForceSoftware);
				lastAppliedTexture = scaledTexture;
				lastAppliedHotspot = hotspot;
			}
		}
	}

	private void OnGUI()
	{
		if (debugDrawHotspot && enableCursorManager && (bool)currentTexture)
		{
			DrawScreenHotspotMarker();
			if (debugDrawCursorPreview)
			{
				DrawCursorPreviewWithHotspot();
			}
		}
	}

	private void DrawScreenHotspotMarker()
	{
		Vector3 currentPointerScreenPosition = GetCurrentPointerScreenPosition();
		float x = currentPointerScreenPosition.x;
		float num = (float)Screen.height - currentPointerScreenPosition.y;
		float num2 = debugPointSize;
		float num3 = num2 * 0.5f;
		GUI.color = Color.red;
		GUI.DrawTexture(new Rect(x - 10f, num - 1f, 20f, 2f), Texture2D.whiteTexture);
		GUI.DrawTexture(new Rect(x - 1f, num - 10f, 2f, 20f), Texture2D.whiteTexture);
		GUI.color = Color.yellow;
		GUI.DrawTexture(new Rect(x - num3, num - num3, num2, num2), Texture2D.whiteTexture);
		GUI.color = Color.white;
		GUI.Label(new Rect(x + 12f, num - 20f, 220f, 20f), $"Hotspot Screen: ({currentPointerScreenPosition.x:F1}, {currentPointerScreenPosition.y:F1})");
	}

	private Vector2 GetHotspot(Texture2D tex)
	{
		Vector2 sourceHotspotRatio = GetSourceHotspotRatio(currentState);
		return new Vector2((float)tex.width * sourceHotspotRatio.x, (float)tex.height * sourceHotspotRatio.y);
	}

	private void DrawCursorPreviewWithHotspot()
	{
		if ((bool)currentTexture)
		{
			Vector2 vector = GetCurrentPointerScreenPosition();
			Vector2 hotspot = GetHotspot(currentTexture);
			float num = vector.x + debugPreviewOffset.x;
			float num2 = (float)Screen.height - vector.y + debugPreviewOffset.y;
			float width = currentTexture.width;
			float height = currentTexture.height;
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(num, num2, width, height), currentTexture);
			float num3 = num + hotspot.x;
			float num4 = num2 + hotspot.y;
			GUI.color = Color.red;
			GUI.DrawTexture(new Rect(num3 - 4f, num4 - 1f, 8f, 2f), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect(num3 - 1f, num4 - 4f, 2f, 8f), Texture2D.whiteTexture);
			GUI.color = Color.green;
			GUI.DrawTexture(new Rect(num3 - 2f, num4 - 2f, 4f, 4f), Texture2D.whiteTexture);
			GUI.color = Color.white;
			GUI.Label(new Rect(num, num2 - 20f, 260f, 20f), $"Texture Hotspot: ({hotspot.x:F1}, {hotspot.y:F1})  Size: {currentTexture.width}x{currentTexture.height}");
		}
	}

	private Texture2D GetScaledTexture(CursorState state)
	{
		Texture2D texture2D = ((state == CursorState.UI) ? scaledUiTexture : scaledAimTexture);
		if ((bool)texture2D)
		{
			return texture2D;
		}
		Texture2D sourceTexture = GetSourceTexture(state);
		if (!sourceTexture)
		{
			return null;
		}
		Texture2D result = CreateScaledTexture(sourceTexture, CurrentVisualScale);
		if (state == CursorState.UI)
		{
			scaledUiTexture = result;
		}
		else
		{
			scaledAimTexture = result;
		}
		return result;
	}

	private static Texture2D CreateScaledTexture(Texture2D source, float scale)
	{
		int num = Mathf.Clamp(Mathf.RoundToInt((float)source.width * scale), 16, 256);
		int num2 = Mathf.Clamp(Mathf.RoundToInt((float)source.height * scale), 16, 256);
		Color32[] pixels = source.GetPixels32();
		Color32[] array = new Color32[num * num2];
		for (int i = 0; i < num2; i++)
		{
			int num3 = Mathf.Min(source.height - 1, i * source.height / num2) * source.width;
			int num4 = i * num;
			for (int j = 0; j < num; j++)
			{
				int num5 = Mathf.Min(source.width - 1, j * source.width / num);
				array[num4 + j] = pixels[num3 + num5];
			}
		}
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, mipChain: false);
		texture2D.name = $"{source.name}_Cursor_{Mathf.RoundToInt(scale * 100f)}";
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.hideFlags = HideFlags.DontSave;
		texture2D.SetPixels32(array);
		texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		return texture2D;
	}

	private static Texture2D GetSourceTexture(CursorState state)
	{
		CursorSettings cursorSettings = SettingsLoader.Instance.cursorSettings;
		switch (state)
		{
		case CursorState.UI:
			if (!cursorSettings.uiCursorLarge)
			{
				if (!cursorSettings.uiCursorMedium)
				{
					return cursorSettings.uiCursorSmall;
				}
				return cursorSettings.uiCursorMedium;
			}
			return cursorSettings.uiCursorLarge;
		case CursorState.Aim:
			if (!cursorSettings.aimCursorLarge)
			{
				if (!cursorSettings.aimCursorMedium)
				{
					return cursorSettings.aimCursorSmall;
				}
				return cursorSettings.aimCursorMedium;
			}
			return cursorSettings.aimCursorLarge;
		default:
			if (!cursorSettings.uiCursorLarge)
			{
				if (!cursorSettings.uiCursorMedium)
				{
					return cursorSettings.uiCursorSmall;
				}
				return cursorSettings.uiCursorMedium;
			}
			return cursorSettings.uiCursorLarge;
		}
	}

	private static Sprite GetSourceSprite(CursorState state)
	{
		CursorSettings cursorSettings = SettingsLoader.Instance.cursorSettings;
		switch (state)
		{
		case CursorState.UI:
			if (!cursorSettings.uiCursorLargeSprite)
			{
				if (!cursorSettings.uiCursorMediumSprite)
				{
					return cursorSettings.uiCursorSmallSprite;
				}
				return cursorSettings.uiCursorMediumSprite;
			}
			return cursorSettings.uiCursorLargeSprite;
		case CursorState.Aim:
			if (!cursorSettings.aimCursorLargeSprite)
			{
				if (!cursorSettings.aimCursorMediumSprite)
				{
					return cursorSettings.aimCursorSmallSprite;
				}
				return cursorSettings.aimCursorMediumSprite;
			}
			return cursorSettings.aimCursorLargeSprite;
		default:
			if (!cursorSettings.uiCursorLargeSprite)
			{
				if (!cursorSettings.uiCursorMediumSprite)
				{
					return cursorSettings.uiCursorSmallSprite;
				}
				return cursorSettings.uiCursorMediumSprite;
			}
			return cursorSettings.uiCursorLargeSprite;
		}
	}

	private static Vector2 GetSourceHotspotRatio(CursorState state)
	{
		CursorSettings cursorSettings = SettingsLoader.Instance.cursorSettings;
		if ((bool)((state == CursorState.Aim) ? cursorSettings.aimCursorLarge : cursorSettings.uiCursorLarge))
		{
			return cursorSettings.largeHot;
		}
		if (!((state == CursorState.Aim) ? cursorSettings.aimCursorMedium : cursorSettings.uiCursorMedium))
		{
			return cursorSettings.smallHot;
		}
		return cursorSettings.mediumHot;
	}

	private void DestroyGeneratedTextures()
	{
		if ((bool)scaledUiTexture)
		{
			Object.Destroy(scaledUiTexture);
		}
		if ((bool)scaledAimTexture)
		{
			Object.Destroy(scaledAimTexture);
		}
		scaledUiTexture = null;
		scaledAimTexture = null;
		currentTexture = null;
	}

	private static Texture2D GetHiddenCursorTexture()
	{
		if ((bool)hiddenCursorTexture)
		{
			return hiddenCursorTexture;
		}
		hiddenCursorTexture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false)
		{
			name = "HiddenCursor",
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp,
			hideFlags = HideFlags.DontSave
		};
		hiddenCursorTexture.SetPixels32(new Color32[4]
		{
			new Color32(0, 0, 0, 0),
			new Color32(0, 0, 0, 0),
			new Color32(0, 0, 0, 0),
			new Color32(0, 0, 0, 0)
		});
		hiddenCursorTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		return hiddenCursorTexture;
	}

	public static bool IsScreenPositionOverUI(Vector2 screenPos)
	{
		if (!EventSystem.current)
		{
			return false;
		}
		if (cachedPointerEventData == null)
		{
			cachedPointerEventData = new PointerEventData(EventSystem.current);
		}
		cachedPointerEventData.position = screenPos;
		results.Clear();
		EventSystem.current.RaycastAll(cachedPointerEventData, results);
		return results.Count > 0;
	}

	public void RefreshCursorStateImmediate(Vector3 pointerScreenPos)
	{
		if (!enableCursorManager)
		{
			return;
		}
		if (!enableCursorStateDetection)
		{
			SetState(CursorState.UI);
		}
		else if (IsScreenPositionOverUI(pointerScreenPos))
		{
			SetState(CursorState.UI);
		}
		else if ((bool)mainCam)
		{
			if ((bool)Physics2D.OverlapPoint(mainCam.ScreenToWorldPoint(pointerScreenPos), interactLayers))
			{
				SetState(CursorState.UI);
			}
			else
			{
				SetState(CursorState.Aim);
			}
		}
	}
}
