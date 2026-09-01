using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VisualKeyboard;

public sealed class VisualKeyboard : MonoBehaviour
{
	[Header("Keyboard")]
	[Tooltip("A list of all keys.")]
	public List<VisualKeyForKeyboard> keys = new List<VisualKeyForKeyboard>(104);

	[Tooltip("If 'Shift' is hold right now? Or CapsLock mode is ON?")]
	public bool isShiftHold;

	[Tooltip("Small UI highlight mark over CapsLock key.")]
	[SerializeField]
	private Image shiftIndicator;

	[Tooltip("可选：用于接收软键盘输入的 InputField")]
	[SerializeField]
	private InputField targetInputField;

	[Tooltip("可选：仅用于调试显示输入结果的文本")]
	[SerializeField]
	private Text inputTextLabel;

	[Tooltip("Should we play sound when user press a key?")]
	public bool keyPressSound;

	[Tooltip("Should we play light animation when user press a key?")]
	public bool keyPressAnimation;

	[Tooltip("A color for key press animation.")]
	public Color keyPressAnimationColor;

	[SerializeField]
	private AudioSource audioSource;

	public event Action<VisualKeyForKeyboard> OnKeyClick;

	public event Action<char> OnCharacterInput;

	private void OnEnable()
	{
		VisualKeyForKeyboard.OnKeyboardButtonClick += OnKeyboardButtonClick;
	}

	private void OnDisable()
	{
		VisualKeyForKeyboard.OnKeyboardButtonClick -= OnKeyboardButtonClick;
	}

	public void SetTargetInputField(InputField inputField)
	{
		targetInputField = inputField;
		RefreshPreviewText();
	}

	private string GetCurrentText()
	{
		if ((bool)targetInputField)
		{
			return targetInputField.text;
		}
		if ((bool)inputTextLabel)
		{
			return inputTextLabel.text;
		}
		return string.Empty;
	}

	public void ResetKeyboardState()
	{
		isShiftHold = false;
		if ((bool)shiftIndicator)
		{
			shiftIndicator.enabled = false;
		}
		foreach (VisualKeyForKeyboard key in keys)
		{
			if ((bool)key)
			{
				key.ResetVisualState();
			}
		}
		RefreshPreviewText();
	}

	private void SetCurrentText(string value)
	{
		if ((bool)targetInputField)
		{
			targetInputField.text = value;
			targetInputField.MoveTextEnd(shift: false);
		}
		if ((bool)inputTextLabel)
		{
			inputTextLabel.text = value;
		}
	}

	private void RefreshPreviewText()
	{
		if ((bool)inputTextLabel)
		{
			inputTextLabel.text = GetCurrentText();
		}
	}

	public void HighlightAllKeys(bool isON)
	{
		foreach (VisualKeyForKeyboard key in keys)
		{
			key.Highlight(isON);
		}
	}

	private void OnKeyboardButtonClick(VisualKeyForKeyboard key)
	{
		if (keyPressSound)
		{
			audioSource.Play();
		}
		if (keyPressAnimation)
		{
			key.HighlightAnimation(keyPressAnimationColor, 1f);
		}
		this.OnKeyClick?.Invoke(key);
		if (key.oldKeyCode == KeyCode.LeftShift || key.oldKeyCode == KeyCode.RightShift || key.oldKeyCode == KeyCode.CapsLock)
		{
			isShiftHold = !isShiftHold;
			shiftIndicator.enabled = isShiftHold;
		}
		else if (key.oldKeyCode == KeyCode.Backspace)
		{
			string currentText = GetCurrentText();
			if (currentText.Length > 0)
			{
				SetCurrentText(currentText.Substring(0, currentText.Length - 1));
			}
		}
		else if (key.character != 0)
		{
			char obj = (isShiftHold ? key.shiftedCharacter : key.character);
			string currentText2 = GetCurrentText();
			SetCurrentText(currentText2 + obj);
			this.OnCharacterInput?.Invoke(obj);
		}
	}

	public VisualKeyForKeyboard GetKeyboardKey(char character)
	{
		foreach (VisualKeyForKeyboard key in keys)
		{
			if (key.character == character)
			{
				return key;
			}
		}
		return null;
	}

	public VisualKeyForKeyboard GetKey(string controlPath)
	{
		foreach (VisualKeyForKeyboard key in keys)
		{
			if (key.controlPath == controlPath)
			{
				return key;
			}
		}
		return null;
	}
}
