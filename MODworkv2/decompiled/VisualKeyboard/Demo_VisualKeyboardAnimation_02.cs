using System.Collections;
using UnityEngine;

namespace VisualKeyboard;

public class Demo_VisualKeyboardAnimation_02 : MonoBehaviour
{
	[Header("Random lights")]
	public Color color = Color.yellow;

	public float randomLightDelay = 0.3f;

	public float lightDuration = 1f;

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
			StartCoroutine(RandomLightAnimation());
		}
	}

	protected virtual IEnumerator RandomLightAnimation()
	{
		while (true)
		{
			yield return new WaitForSeconds(randomLightDelay);
			keyboard.keys[Random.Range(0, keyboard.keys.Count)].HighlightAnimation(color, lightDuration);
		}
	}
}
