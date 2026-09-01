using System.Collections.Generic;
using Data.AutoGen.DataClass.Settings;
using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using Inputs.Visual_Keyboard;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inputs;

public class GamepadUIActionManager : Singleton<GamepadUIActionManager>
{
	public class SliderInputState
	{
		public UINavigationDirection HoldDirection;

		public float NextStepTime;

		public bool AxisReturnedToCenter = true;

		public float HoldStartTime = -1f;
	}

	private enum GamepadActionSemantic
	{
		None,
		Submit,
		Cancel,
		ShiftLike,
		CtrlLike
	}

	[Header("自定义滚动视图参数")]
	private const float customScrollThreshold = 0.25f;

	private const float customScrollSpeed = 1.4f;

	private const bool invertCustomScrollY = false;

	[Header("Slider 导航参数")]
	private const float sliderMoveThreshold = 0.3f;

	private const float sliderMoveSpeed = 1f;

	private const bool invertSliderMoveX = false;

	private const float smallWholeNumberRangeThreshold = 8f;

	private const float largeWholeNumberMaxRepeatInterval = 0.2f;

	private const float largeWholeNumberMinRepeatInterval = 0.035f;

	private const float largeWholeNumberHoldAccelTime = 1.2f;

	private const float largeWholeNumberHoldAccelMultiplier = 0.35f;

	private readonly SliderInputState primarySliderState = new SliderInputState();

	public readonly SliderInputState rightSliderState = new SliderInputState();

	private float nextSliderStepTime;

	private int lastSliderStepFrame = -1;

	private static int _submitConsumedFrame = -1;

	private static int _cancelConsumedFrame = -1;

	private static bool _blockGameplaySubmitUntilRelease;

	private static bool _blockGameplayCancelUntilRelease;

	private readonly Dictionary<ControlAction, GamepadActionSemantic> _actionSemanticCache = new Dictionary<ControlAction, GamepadActionSemantic>();

	private InputDeviceType _cachedDeviceType;

	private bool _cacheBuilt;

	private GamepadUIActionManager()
	{
	}

	public void HandleMoveForSlider(Slider slider)
	{
		float leftStickXRaw = GamepadInputManager.GetLeftStickXRaw();
		float leftStickYRaw = GamepadInputManager.GetLeftStickYRaw();
		float dPadXRaw = GamepadInputManager.GetDPadXRaw();
		float dPadYRaw = GamepadInputManager.GetDPadYRaw();
		float num = ((Mathf.Abs(dPadXRaw) > Mathf.Abs(leftStickXRaw)) ? dPadXRaw : leftStickXRaw);
		float num2 = ((Mathf.Abs(dPadYRaw) > Mathf.Abs(leftStickYRaw)) ? dPadYRaw : leftStickYRaw);
		bool flag = Mathf.Abs(num) >= 0.3f;
		bool flag2 = Mathf.Abs(num2) >= 0.5f;
		if (!flag && !flag2)
		{
			GamepadUINavigationManager.holdMoveDirection = UINavigationDirection.None;
			primarySliderState.HoldDirection = UINavigationDirection.None;
			primarySliderState.AxisReturnedToCenter = true;
			return;
		}
		float unscaledTime = Time.unscaledTime;
		if (flag && Mathf.Abs(num) >= Mathf.Abs(num2))
		{
			GamepadUINavigationManager.holdMoveDirection = UINavigationDirection.None;
			HandleSliderInput(slider, num, unscaledTime, primarySliderState);
			return;
		}
		primarySliderState.HoldDirection = UINavigationDirection.None;
		primarySliderState.AxisReturnedToCenter = true;
		UINavigationDirection uINavigationDirection = ((num2 > 0f) ? UINavigationDirection.Up : UINavigationDirection.Down);
		if (GamepadUINavigationManager.holdMoveDirection != uINavigationDirection)
		{
			GamepadUINavigationManager.holdMoveDirection = uINavigationDirection;
			GamepadUINavigationManager.nextMoveTime = unscaledTime + 0.35f;
			GamepadUINavigationManager.MoveSelection(uINavigationDirection);
		}
		else if (unscaledTime >= GamepadUINavigationManager.nextMoveTime)
		{
			GamepadUINavigationManager.nextMoveTime = unscaledTime + 0.12f;
			GamepadUINavigationManager.MoveSelection(uINavigationDirection);
		}
	}

	public void HandleSliderInput(Slider slider, float horizontal, float now, SliderInputState state)
	{
		float num = Mathf.Abs(horizontal);
		float num2 = slider.maxValue - slider.minValue;
		if (num < 0.3f)
		{
			state.HoldDirection = UINavigationDirection.None;
			state.AxisReturnedToCenter = true;
			state.HoldStartTime = -1f;
		}
		else if (!slider.wholeNumbers)
		{
			HandleContinuousSliderMove(slider, horizontal);
			state.HoldDirection = UINavigationDirection.None;
			state.AxisReturnedToCenter = false;
		}
		else if (num2 <= 8f)
		{
			HandleSmallRangeWholeNumberSlider(slider, horizontal, state);
		}
		else
		{
			HandleLargeRangeWholeNumberSlider(slider, horizontal, now, state);
		}
	}

	private void HandleLargeRangeWholeNumberSlider(Slider slider, float horizontal, float now, SliderInputState state)
	{
		UINavigationDirection uINavigationDirection = ((horizontal > 0f) ? UINavigationDirection.Right : UINavigationDirection.Left);
		float value = Mathf.Abs(horizontal);
		float t = Mathf.InverseLerp(0.3f, 1f, value);
		float baseInterval = Mathf.Lerp(0.2f, 0.035f, t);
		if (state.HoldDirection != uINavigationDirection)
		{
			state.HoldDirection = uINavigationDirection;
			state.AxisReturnedToCenter = false;
			state.HoldStartTime = now;
			float acceleratedRepeatInterval = GetAcceleratedRepeatInterval(baseInterval, now, state);
			state.NextStepTime = now + acceleratedRepeatInterval;
			StepWholeNumberSlider(slider, uINavigationDirection);
		}
		else if (state.AxisReturnedToCenter)
		{
			state.AxisReturnedToCenter = false;
			state.HoldStartTime = now;
			float acceleratedRepeatInterval2 = GetAcceleratedRepeatInterval(baseInterval, now, state);
			state.NextStepTime = now + acceleratedRepeatInterval2;
			StepWholeNumberSlider(slider, uINavigationDirection);
		}
		else
		{
			float acceleratedRepeatInterval3 = GetAcceleratedRepeatInterval(baseInterval, now, state);
			if (now >= state.NextStepTime)
			{
				state.NextStepTime = now + acceleratedRepeatInterval3;
				StepWholeNumberSlider(slider, uINavigationDirection);
			}
		}
	}

	private void HandleSmallRangeWholeNumberSlider(Slider slider, float horizontal, SliderInputState state)
	{
		UINavigationDirection uINavigationDirection = ((horizontal > 0f) ? UINavigationDirection.Right : UINavigationDirection.Left);
		if (state.AxisReturnedToCenter)
		{
			state.AxisReturnedToCenter = false;
			state.HoldDirection = uINavigationDirection;
			state.HoldStartTime = Time.unscaledTime;
			StepWholeNumberSlider(slider, uINavigationDirection);
		}
	}

	private static void HandleContinuousSliderMove(Slider slider, float horizontal)
	{
		float num = slider.maxValue - slider.minValue;
		float num2 = horizontal * 1f * num * Time.unscaledDeltaTime;
		slider.value = Mathf.Clamp(slider.value + num2, slider.minValue, slider.maxValue);
	}

	private void StepWholeNumberSlider(Slider slider, UINavigationDirection direction)
	{
		if (lastSliderStepFrame != Time.frameCount)
		{
			lastSliderStepFrame = Time.frameCount;
			float num = ((direction == UINavigationDirection.Right) ? 1f : (-1f));
			float num2 = Mathf.Clamp(slider.value + num, slider.minValue, slider.maxValue);
			if (!Mathf.Approximately(num2, slider.value))
			{
				slider.value = num2;
			}
		}
	}

	private static float GetAcceleratedRepeatInterval(float baseInterval, float now, SliderInputState state)
	{
		if (state.HoldStartTime < 0f)
		{
			return baseInterval;
		}
		float t = Mathf.Clamp01((now - state.HoldStartTime) / 1.2f);
		float num = Mathf.Lerp(1f, 0.35f, t);
		float b = baseInterval * num;
		return Mathf.Max(0.035f, b);
	}

	public static void HandleCustomScrollView(CustomScrollView scrollView, float y)
	{
		if (!(Mathf.Abs(y) < 0.25f))
		{
			float delta = (0f - y) * 1.4f * Time.unscaledDeltaTime;
			scrollView.AddValue(delta);
		}
	}

	public static void HandleSubmit()
	{
		if (!GamepadInputManager.GetSubmitDown())
		{
			return;
		}
		Selectable currentSelectable = GamepadUINavigationManager.GetCurrentSelectable();
		if (!currentSelectable)
		{
			return;
		}
		_submitConsumedFrame = Time.frameCount;
		_blockGameplaySubmitUntilRelease = true;
		InputField component = currentSelectable.GetComponent<InputField>();
		if ((bool)component)
		{
			if (!Singleton<SoftKeyboardManager>.Instance.IsOpen)
			{
				Singleton<SoftKeyboardManager>.Instance.Show(component);
			}
		}
		else
		{
			BaseEventData eventData = new BaseEventData(EventSystem.current);
			ExecuteEvents.Execute(currentSelectable.gameObject, eventData, ExecuteEvents.submitHandler);
			GamepadUINavigationManager.TryAutoMoveAfterSubmit(currentSelectable);
		}
	}

	public static bool IsGameplaySubmitBlocked()
	{
		if (_submitConsumedFrame == Time.frameCount)
		{
			return true;
		}
		if (_blockGameplaySubmitUntilRelease)
		{
			if (GamepadInputManager.GetSubmit())
			{
				return true;
			}
			_blockGameplaySubmitUntilRelease = false;
		}
		return false;
	}

	public static void HandleCancel()
	{
		if (!GamepadInputManager.GetCancelDown())
		{
			return;
		}
		GamepadSelectablePanel currentGamepadPanel = GamepadUINavigationManager.GetCurrentGamepadPanel();
		if ((bool)currentGamepadPanel)
		{
			_cancelConsumedFrame = Time.frameCount;
			_blockGameplayCancelUntilRelease = true;
			if (Singleton<SoftKeyboardManager>.Instance.IsOpen)
			{
				Singleton<SoftKeyboardManager>.Instance.Hide();
			}
			else
			{
				currentGamepadPanel.OnCancel();
			}
		}
	}

	public static void HandleKeyboardCancel()
	{
		if (!Input.GetKeyDown(KeyCode.Escape) || IsCancelConsumedThisFrame())
		{
			return;
		}
		GamepadSelectablePanel topActiveGamepadPanel = GamepadUINavigationManager.GetTopActiveGamepadPanel();
		if ((bool)topActiveGamepadPanel)
		{
			ConsumeCancelForCurrentFrame();
			if (Singleton<SoftKeyboardManager>.Instance.IsOpen)
			{
				Singleton<SoftKeyboardManager>.Instance.Hide();
			}
			else
			{
				topActiveGamepadPanel.OnCancel();
			}
		}
	}

	public static void ConsumeCancelForCurrentFrame()
	{
		_cancelConsumedFrame = Time.frameCount;
	}

	public static bool IsCancelConsumedThisFrame()
	{
		return _cancelConsumedFrame == Time.frameCount;
	}

	public static bool IsGameplayCancelBlocked()
	{
		if (_cancelConsumedFrame == Time.frameCount)
		{
			return true;
		}
		if (_blockGameplayCancelUntilRelease)
		{
			if (GamepadInputManager.GetCancel())
			{
				return true;
			}
			_blockGameplayCancelUntilRelease = false;
		}
		return false;
	}

	private void RebuildActionSemanticCache()
	{
		_actionSemanticCache.Clear();
		_cacheBuilt = false;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			InputDeviceType currentDeviceType = SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType;
			ControlsSettingData control = Singleton<SettingDataManager>.Instance.GetControl(currentDeviceType);
			if (control != null)
			{
				_cachedDeviceType = currentDeviceType;
				CacheActionSemantic(control, ControlAction.Skill1);
				CacheActionSemantic(control, ControlAction.Skill2);
				CacheActionSemantic(control, ControlAction.Skill3);
				CacheActionSemantic(control, ControlAction.Skill4);
				CacheActionSemantic(control, ControlAction.Skill5);
				CacheActionSemantic(control, ControlAction.Skill6);
				CacheActionSemantic(control, ControlAction.Skill7);
				CacheActionSemantic(control, ControlAction.Skill8);
				CacheActionSemantic(control, ControlAction.Item1);
				CacheActionSemantic(control, ControlAction.Item2);
				CacheActionSemantic(control, ControlAction.QuickUse);
				CacheActionSemantic(control, ControlAction.Mercenary);
				CacheActionSemantic(control, ControlAction.Talent);
				CacheActionSemantic(control, ControlAction.Stats);
				CacheActionSemantic(control, ControlAction.Bag);
				CacheActionSemantic(control, ControlAction.TP);
				CacheActionSemantic(control, ControlAction.Sell);
				CacheActionSemantic(control, ControlAction.SellAll);
				CacheActionSemantic(control, ControlAction.PageL);
				CacheActionSemantic(control, ControlAction.PageR);
				CacheActionSemantic(control, ControlAction.SortAll);
				CacheActionSemantic(control, ControlAction.Sort);
				CacheActionSemantic(control, ControlAction.AutoAT);
				_cacheBuilt = true;
			}
		}
	}

	private void EnsureActionSemanticCache()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			if (!_cacheBuilt)
			{
				RebuildActionSemanticCache();
			}
			else if (_cachedDeviceType != SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType)
			{
				RebuildActionSemanticCache();
			}
		}
	}

	private void CacheActionSemantic(ControlsSettingData controlsData, ControlAction action)
	{
		string bindKey = controlsData.GetBindKey(action);
		if (string.IsNullOrWhiteSpace(bindKey))
		{
			_actionSemanticCache[action] = GamepadActionSemantic.None;
			return;
		}
		bindKey = KeyNameUtil.NormalizeKeyName(bindKey);
		if (string.Equals(bindKey, "Pad_A"))
		{
			_actionSemanticCache[action] = GamepadActionSemantic.Submit;
		}
		else if (string.Equals(bindKey, "Pad_B"))
		{
			_actionSemanticCache[action] = GamepadActionSemantic.Cancel;
		}
		else if (string.Equals(bindKey, "Pad_LStickPress"))
		{
			_actionSemanticCache[action] = GamepadActionSemantic.ShiftLike;
		}
		else if (string.Equals(bindKey, "Pad_RStickPress"))
		{
			_actionSemanticCache[action] = GamepadActionSemantic.CtrlLike;
		}
		else
		{
			_actionSemanticCache[action] = GamepadActionSemantic.None;
		}
	}

	public void MarkActionSemanticCacheDirty()
	{
		_cacheBuilt = false;
	}

	public static bool IsGameplayActionBlocked(ControlAction action)
	{
		return Singleton<GamepadUIActionManager>.Instance.IsGameplayActionBlockedCached(action);
	}

	public bool IsGameplayActionBlockedCached(ControlAction action)
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return false;
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return false;
		}
		EnsureActionSemanticCache();
		if (!_actionSemanticCache.TryGetValue(action, out var value))
		{
			return false;
		}
		if (SingletonMonoGlobal<VirtualCursorManager>.HasInstance && SingletonMonoGlobal<VirtualCursorManager>.Instance.ShouldUseVirtualCursor)
		{
			switch (value)
			{
			case GamepadActionSemantic.Submit:
				return CursorUIManager.IsGameplayLeftClickBlocked();
			case GamepadActionSemantic.Cancel:
				return CursorUIManager.IsGameplayRightClickBlocked();
			case GamepadActionSemantic.ShiftLike:
				return CursorUIManager.IsGameplayShiftBlocked();
			case GamepadActionSemantic.CtrlLike:
				return CursorUIManager.IsGameplayCtrlBlocked();
			}
		}
		return value switch
		{
			GamepadActionSemantic.Submit => IsGameplaySubmitBlocked(), 
			GamepadActionSemantic.Cancel => IsGameplayCancelBlocked(), 
			_ => false, 
		};
	}
}
