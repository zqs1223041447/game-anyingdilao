using System.Collections;
using UnityEngine;

namespace VisualKeyboard;

public class Demo_VisualKeyboardAnimation_01 : MonoBehaviour
{
	[Header("Wave lightning")]
	public float waveSpeed = 0.3f;

	public bool autoStart = true;

	public VisualKeyboard keyboard;

	private void OnEnable()
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

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	[ContextMenu("Wave light animation")]
	public virtual void StartAnimation()
	{
		StopAllCoroutines();
		if (keyboard != null)
		{
			StartCoroutine(WaveLightAnimation());
		}
	}

	private IEnumerator WaveLightAnimation()
	{
		foreach (VisualKeyForKeyboard key in keyboard.keys)
		{
			key.HighlightOFF();
		}
		while (true)
		{
			yield return null;
			foreach (VisualKeyForKeyboard key2 in keyboard.keys)
			{
				float num = Time.time * waveSpeed + key2.normalizedPosition.x;
				if (num > 1f)
				{
					num -= Mathf.Floor(num);
				}
				Color color = Random.ColorHSV(num, num, 0.3f, 0.301f, 0.999f, 1f, 0.999f, 1f);
				key2.HighlightON(color);
			}
		}
	}
}
