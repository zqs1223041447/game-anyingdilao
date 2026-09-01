using System;
using UnityEngine;
using UnityEngine.UI;

namespace SK.Framework;

[Serializable]
public class UIAnimation
{
	public bool moveToggle;

	public UIMoveAnimation moveAnimation;

	public bool rotateToggle;

	public UIRotateAnimation rotateAnimation;

	public bool scaleToggle;

	public UIScaleAnimation scaleAnimation;

	public bool fadeToggle;

	public UIFadeAnimation fadeAnimation;

	public bool IsAnyAnimation
	{
		get
		{
			if (!moveToggle && !rotateToggle && !scaleToggle)
			{
				return fadeToggle;
			}
			return true;
		}
	}

	public float Duration => MathUtility.Max(moveAnimation.duration + moveAnimation.delay, rotateAnimation.duration + rotateAnimation.delay, scaleAnimation.duration + scaleAnimation.delay, fadeAnimation.duration + fadeAnimation.delay);

	public IActionChain Play(MonoBehaviour behaviour, RectTransform rectTransform, bool instant = false, Action callback = null)
	{
		ConcurrentActionChain concurrentActionChain = new ConcurrentActionChain();
		if (moveToggle)
		{
			concurrentActionChain.Tween(() => moveAnimation.Play(rectTransform, instant));
		}
		if (rotateToggle)
		{
			concurrentActionChain.Tween(() => rotateAnimation.Play(rectTransform, instant));
		}
		if (scaleToggle)
		{
			concurrentActionChain.Tween(() => scaleAnimation.Play(rectTransform, instant));
		}
		if (fadeToggle)
		{
			concurrentActionChain.Tween(() => fadeAnimation.Play(rectTransform.GetComponent<Graphic>(), instant));
		}
		return behaviour.Sequence().Append(concurrentActionChain).Event(delegate
		{
			callback?.Invoke();
		})
			.Begin();
	}
}
