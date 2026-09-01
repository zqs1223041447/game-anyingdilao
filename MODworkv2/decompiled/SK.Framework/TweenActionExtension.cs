using System;
using DG.Tweening;

namespace SK.Framework;

public static class TweenActionExtension
{
	public static IActionChain Tween(this IActionChain chain, Func<Tween> tweenAction)
	{
		return chain.Append(new TweenAction(tweenAction));
	}
}
