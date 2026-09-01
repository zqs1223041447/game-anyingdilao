using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Inputs;

[RequireComponent(typeof(Button))]
public class GamepadButtonStateAdapter : MonoBehaviour
{
	private Button button;

	private ColorBlock originalColors;

	private void Awake()
	{
		button = GetComponent<Button>();
		originalColors = button.colors;
	}

	private void OnEnable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged += HandleDeviceChanged;
		HandleDeviceChanged(SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType);
	}

	private void OnDisable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged -= HandleDeviceChanged;
	}

	private void HandleDeviceChanged(InputDeviceType deviceType)
	{
		ColorBlock colors = originalColors;
		if (deviceType == InputDeviceType.PC)
		{
			colors.selectedColor = colors.normalColor;
		}
		else
		{
			colors.highlightedColor = colors.normalColor;
		}
		button.colors = colors;
	}
}
