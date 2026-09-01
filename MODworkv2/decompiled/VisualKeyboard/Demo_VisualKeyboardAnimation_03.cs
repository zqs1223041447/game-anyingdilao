using System.Collections;
using UnityEngine;

namespace VisualKeyboard;

public class Demo_VisualKeyboardAnimation_03 : MonoBehaviour
{
	[Header("Wave lightning")]
	public string animatedText = "Hello world";

	public float randomLightDelay = 0.35f;

	public bool autoStart = true;

	public VisualKeyboard keyboard;

	protected virtual void OnEnable()
	{
		if (keyboard == null)
		{
			keyboard = GetComponent<VisualKeyboard>();
		}
		if (keyboard == null)
		{
			keyboard = GetComponentInParent<VisualKeyboard>();
		}
		if (keyboard == null)
		{
			keyboard = GetComponentInChildren<VisualKeyboard>();
		}
		if (autoStart)
		{
			StartAnimation();
		}
	}

	protected virtual void OnDisable()
	{
		StopAllCoroutines();
	}

	public virtual void StartAnimation()
	{
		StopAllCoroutines();
		if (keyboard != null)
		{
			StartCoroutine(WordsAnimation());
		}
	}

	protected virtual IEnumerator WordsAnimation()
	{
		string text = animatedText;
		string text2 = text;
		foreach (char ch in text2)
		{
			if (ch == ' ')
			{
				yield return new WaitForSeconds(randomLightDelay * 2f);
				continue;
			}
			yield return new WaitForSeconds(randomLightDelay);
			VisualKeyForKeyboard key = keyboard.GetKeyboardKey(ch);
			if (key != null)
			{
				key.HighlightON();
			}
			yield return new WaitForSeconds(randomLightDelay);
			if (key != null)
			{
				key.HighlightOFF();
			}
		}
	}
}
