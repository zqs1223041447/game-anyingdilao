using System;
using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

[Serializable]
public class ViewAnimation
{
	public UIAnimationType type;

	public List<UIAnimationActor> actors = new List<UIAnimationActor>(0);

	public string stateName;

	public IActionChain Play(UIView view, bool instant = false, Action callback = null)
	{
		switch (type)
		{
		case UIAnimationType.Tween:
		{
			IActionChain actionChain = new ConcurrentActionChain();
			for (int i = 0; i < actors.Count; i++)
			{
				UIAnimationActor uIAnimationActor = actors[i];
				actionChain.Append(uIAnimationActor.animation.Play(view, uIAnimationActor.actor, instant) as IAction);
			}
			return view.Sequence().Append(actionChain as IAction).Event(callback)
				.Begin();
		}
		case UIAnimationType.Animator:
			return view.Sequence().Animate(view.GetComponent<Animator>(), stateName).Event(callback)
				.Begin();
		default:
			return null;
		}
	}
}
