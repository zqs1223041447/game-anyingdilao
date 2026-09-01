using System;
using DG.Tweening;

namespace SK.Framework;

public class TweenAction : AbstractAction
{
	private Tween tween;

	private readonly Func<Tween> action;

	private bool isBegan;

	public TweenAction(Func<Tween> action)
	{
		this.action = action;
	}

	protected override void OnInvoke()
	{
		if (!isBegan)
		{
			isBegan = true;
			tween = action();
		}
		isCompleted = !tween.IsPlaying();
	}

	protected override void OnReset()
	{
		isBegan = false;
	}
}
