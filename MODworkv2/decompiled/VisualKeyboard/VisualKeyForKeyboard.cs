using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VisualKeyboard;

public sealed class VisualKeyForKeyboard : MonoBehaviour
{
	[Header("Keyboard Key")]
	[Tooltip("Keycode for old input system.")]
	public KeyCode oldKeyCode;

	[Tooltip("Control path for new Unity's Input System.")]
	public string controlPath;

	[Tooltip("Character is produced when button is pressed.")]
	public char character;

	[Tooltip("Character is produced when button is pressed (with SHIFT hold).")]
	public char shiftedCharacter;

	[Tooltip("Normalized position of key on keyboard panel, from left bottom corner. \r\n For example, LeftCTRL = (0f, 0f), ESC = (0f, 1f), NumpadEnter = (1f, 0f) etc...")]
	public Vector2 normalizedPosition;

	public Image overlay;

	private Color defaultOverlayColor;

	private bool hasCachedDefaultOverlayColor;

	public static event Action<VisualKeyForKeyboard> OnKeyboardButtonClick;

	private void Awake()
	{
		CacheDefaultState();
	}

	private void OnEnable()
	{
		CacheDefaultState();
	}

	private void CacheDefaultState()
	{
		if ((bool)overlay && !hasCachedDefaultOverlayColor)
		{
			defaultOverlayColor = overlay.color;
			hasCachedDefaultOverlayColor = true;
		}
	}

	public void Highlight(bool isOn)
	{
		if (isOn)
		{
			HighlightON();
		}
		else
		{
			HighlightOFF();
		}
	}

	[ContextMenu("Highlight OFF")]
	public void HighlightOFF()
	{
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(value: false);
			if (hasCachedDefaultOverlayColor)
			{
				overlay.color = defaultOverlayColor;
			}
		}
	}

	[ContextMenu("Highlight ON")]
	public void HighlightON()
	{
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(value: true);
		}
	}

	public void HighlightON(Color color)
	{
		if ((bool)overlay)
		{
			color.a = Mathf.Clamp(color.a, 0.19f, 0.21f);
			overlay.color = color;
			overlay.gameObject.SetActive(value: true);
		}
	}

	public void HighlightAnimation(Color color, float fadeTime)
	{
		if ((bool)overlay)
		{
			StopAllCoroutines();
			StartCoroutine(HighlightAnimating(color, fadeTime));
		}
	}

	private IEnumerator HighlightAnimating(Color color, float fadeTime)
	{
		if ((bool)overlay)
		{
			float endTime = Time.time + fadeTime;
			color.a = Mathf.Clamp(color.a, 0.19f, 0.21f);
			float startAlpha = color.a;
			overlay.color = color;
			overlay.gameObject.SetActive(value: true);
			while (color.a > 0.01f)
			{
				yield return null;
				float t = 1f - (endTime - Time.time) / fadeTime;
				Color color2 = overlay.color;
				color2.a = Mathf.Lerp(startAlpha, 0f, t);
				overlay.color = color2;
			}
			overlay.gameObject.SetActive(value: false);
			if (hasCachedDefaultOverlayColor)
			{
				overlay.color = defaultOverlayColor;
			}
		}
	}

	public void ResetVisualState()
	{
		StopAllCoroutines();
		if ((bool)overlay)
		{
			if (hasCachedDefaultOverlayColor)
			{
				overlay.color = defaultOverlayColor;
			}
			overlay.gameObject.SetActive(value: false);
		}
	}

	public void UI_Click()
	{
		VisualKeyForKeyboard.OnKeyboardButtonClick?.Invoke(this);
	}
}
