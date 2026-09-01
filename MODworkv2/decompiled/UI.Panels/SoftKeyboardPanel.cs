using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.UI.Base;
using Inputs;
using Inputs.Visual_Keyboard;
using UnityEngine.UI;
using VisualKeyboard;

namespace UI.Panels;

public class SoftKeyboardPanel : GamepadSelectablePanel, IPanelParam<InputField>
{
	private global::VisualKeyboard.VisualKeyboard keyboard;

	private Button firstButton;

	private Button lastSelectedButton;

	private Selectable closeReturnTarget;

	protected override void Awake()
	{
		base.Awake();
		keyboard = GetComponentInChildren<global::VisualKeyboard.VisualKeyboard>();
		firstButton = GetControl<Button>(" key A");
	}

	public override void OnShow()
	{
		base.OnShow();
		keyboard.ResetKeyboardState();
		Button openFocusButton = GetOpenFocusButton();
		SetFirstSelected(openFocusButton, selectNow: false);
		GamepadUINavigationManager.RequestForceFocus(openFocusButton);
	}

	public override void OnHide()
	{
		Button button = GamepadUINavigationManager.GetCurrentSelectable() as Button;
		if (IsKeyboardButtonValid(button))
		{
			lastSelectedButton = button;
		}
		base.OnHide();
		keyboard.ResetKeyboardState();
		if ((bool)closeReturnTarget && closeReturnTarget.gameObject.activeInHierarchy && closeReturnTarget.IsInteractable())
		{
			GamepadUINavigationManager.RequestForceFocus(closeReturnTarget);
		}
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent() && Singleton<SoftKeyboardManager>.Instance.IsOpen)
		{
			Singleton<UIManager>.Instance.HidePanel<SoftKeyboardPanel>();
		}
	}

	public void SetParam(InputField param)
	{
		keyboard.ResetKeyboardState();
		keyboard.SetTargetInputField(param);
		closeReturnTarget = param;
	}

	private Button GetOpenFocusButton()
	{
		if (IsKeyboardButtonValid(lastSelectedButton))
		{
			return lastSelectedButton;
		}
		return firstButton;
	}

	private static bool IsKeyboardButtonValid(Button button)
	{
		if (!button)
		{
			return false;
		}
		if (!button.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (!button.IsInteractable())
		{
			return false;
		}
		return true;
	}
}
