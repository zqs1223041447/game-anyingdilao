using Data.AutoGen.DataClass.Settings;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Cursors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ACT_Auto : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private const float TipDelay = 0.2f;

	public Sprite Main;

	public Sprite Lit;

	public string ClickSound;

	private Image image;

	private Button button;

	private GameUIManager gameUIManager;

	private float hoverTime;

	private bool hovering;

	private bool tipShown;

	private bool lastVisualState;

	private Vector2 lastPointerPosition;

	private bool pointerPositionInitialized;

	private bool pointerMovedSinceEnable;

	private void Awake()
	{
		image = GetComponent<Image>();
		button = GetComponent<Button>();
		gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		if ((bool)button)
		{
			button.onClick.AddListener(ToggleAutoAttack);
		}
	}

	private void OnDestroy()
	{
		if ((bool)button)
		{
			button.onClick.RemoveListener(ToggleAutoAttack);
		}
	}

	private void OnEnable()
	{
		ResetHoverState();
		pointerPositionInitialized = false;
		pointerMovedSinceEnable = false;
		if (IsHoverInputAllowed())
		{
			lastPointerPosition = GetActivePointerPosition();
			pointerPositionInitialized = true;
		}
		RefreshVisual(force: true);
	}

	private void OnDisable()
	{
		HideTip();
		ResetHoverState();
	}

	private void Update()
	{
		RefreshVisual(force: false);
		UpdatePointerMovement();
		if (!IsHoverInputAllowed())
		{
			if (hovering)
			{
				HideTip();
				ResetHoverState();
			}
		}
		else
		{
			if (!hovering)
			{
				return;
			}
			if (!tipShown)
			{
				hoverTime += Time.deltaTime;
				if (hoverTime < 0.2f)
				{
					return;
				}
				tipShown = true;
			}
			ShowTip();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (IsHoverInputAllowed(eventData))
		{
			UpdatePointerMovement(eventData.position);
			if (pointerMovedSinceEnable)
			{
				hovering = true;
				tipShown = false;
				hoverTime = 0f;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HideTip();
		ResetHoverState();
	}

	public void ToggleAutoAttackFromShortcut()
	{
		ToggleAutoAttack();
		HideTip();
		ResetHoverState();
	}

	private void ToggleAutoAttack()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			bool autoAttackEnabled = !SingletonMonoScope<PlayerManager>.Instance.AutoAttackEnabled;
			SingletonMonoScope<PlayerManager>.Instance.AutoAttackEnabled = autoAttackEnabled;
			SyncSetting(autoAttackEnabled);
			RefreshVisual(force: true);
			PlayClickSound();
			if (SaveManager.HasRuntime)
			{
				SaveManager.RequestSave();
			}
		}
	}

	private void RefreshVisual(bool force)
	{
		bool flag = SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<PlayerManager>.Instance.AutoAttackEnabled;
		if (force || flag != lastVisualState)
		{
			lastVisualState = flag;
			if ((bool)image)
			{
				image.sprite = (flag ? Lit : Main);
			}
		}
	}

	private void ShowTip()
	{
		if (!gameUIManager)
		{
			return;
		}
		int num;
		object obj;
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			num = (SingletonMonoScope<PlayerManager>.Instance.AutoAttackEnabled ? 1 : 0);
			if (num != 0)
			{
				obj = "AutoAttackStatusOn";
				goto IL_0036;
			}
		}
		else
		{
			num = 0;
		}
		obj = "AutoAttackStatusOff";
		goto IL_0036;
		IL_0036:
		string text = (string)obj;
		string secondaryText = ((num != 0) ? "AutoAttackClickOff" : "AutoAttackClickOn");
		gameUIManager.ShowTipWithShortcutInline(base.transform, text, ControlAction.AutoAT, secondaryText);
	}

	private void PlayClickSound()
	{
		string text = ClickSound;
		if (string.IsNullOrWhiteSpace(text) && SingletonMonoGlobal<AudioManager>.HasInstance && (bool)SingletonMonoGlobal<AudioManager>.Instance.audioData)
		{
			text = SingletonMonoGlobal<AudioManager>.Instance.audioData.Add_Point_3;
		}
		if (!string.IsNullOrWhiteSpace(text))
		{
			RuntimeManager.PlayOneShot(text);
		}
	}

	private static void SyncSetting(bool enabled)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		GameSettingData game = instance.GetGame();
		if (game != null)
		{
			game.auto_attack = enabled;
			instance.SaveGame();
		}
		if (instance.IsEditing)
		{
			GameSettingData editingGame = instance.GetEditingGame();
			if (editingGame != null)
			{
				editingGame.auto_attack = enabled;
			}
		}
	}

	private void ResetHoverState()
	{
		hovering = false;
		tipShown = false;
		hoverTime = 0f;
	}

	private void HideTip()
	{
		if ((bool)gameUIManager)
		{
			gameUIManager.HideEmptyTip();
		}
	}

	private bool IsHoverInputAllowed()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return false;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return true;
		}
		return CursorInputManager.IsUsingVirtualMouse;
	}

	private bool IsHoverInputAllowed(PointerEventData eventData)
	{
		if (!IsHoverInputAllowed())
		{
			return false;
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return true;
		}
		if (!SingletonMonoScope<CursorInputManager>.HasInstance || !CursorInputManager.IsUsingVirtualMouse)
		{
			return false;
		}
		Vector2 vector = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		return (eventData.position - vector).sqrMagnitude <= 1f;
	}

	private void UpdatePointerMovement()
	{
		if (IsHoverInputAllowed())
		{
			UpdatePointerMovement(GetActivePointerPosition());
		}
	}

	private void UpdatePointerMovement(Vector2 currentPosition)
	{
		if (!pointerPositionInitialized)
		{
			lastPointerPosition = currentPosition;
			pointerPositionInitialized = true;
			return;
		}
		if ((currentPosition - lastPointerPosition).sqrMagnitude > 0.01f)
		{
			pointerMovedSinceEnable = true;
		}
		lastPointerPosition = currentPosition;
	}

	private static Vector2 GetActivePointerPosition()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent() && SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			return SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		}
		return Input.mousePosition;
	}
}
