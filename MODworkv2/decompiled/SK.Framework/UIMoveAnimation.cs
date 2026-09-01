using System;
using DG.Tweening;
using UnityEngine;

namespace SK.Framework;

[Serializable]
public class UIMoveAnimation
{
	public enum MoveMode
	{
		MoveIn,
		MoveOut
	}

	public float duration = 1f;

	public float delay;

	public Ease ease = Ease.Linear;

	public UIMoveAnimationDirection direction;

	public bool isCustom;

	public Vector3 startValue;

	public Vector3 endValue;

	public MoveMode moveMode;

	public Tween Play(RectTransform target, bool instant = false)
	{
		Vector3 vector = Vector3.zero;
		float num = target.rect.width / 2f + target.rect.width * target.pivot.x;
		float num2 = target.rect.height / 2f + target.rect.height * target.pivot.y;
		switch (direction)
		{
		case UIMoveAnimationDirection.Left:
			vector = new Vector3(0f - num, 0f, 0f);
			break;
		case UIMoveAnimationDirection.Right:
			vector = new Vector3(num, 0f, 0f);
			break;
		case UIMoveAnimationDirection.Top:
			vector = new Vector3(0f, num2, 0f);
			break;
		case UIMoveAnimationDirection.Bottom:
			vector = new Vector3(0f, 0f - num2, 0f);
			break;
		case UIMoveAnimationDirection.TopLeft:
			vector = new Vector3(0f - num, num2, 0f);
			break;
		case UIMoveAnimationDirection.TopRight:
			vector = new Vector3(num, num2, 0f);
			break;
		case UIMoveAnimationDirection.MiddleCenter:
			vector = Vector3.zero;
			break;
		case UIMoveAnimationDirection.BottomLeft:
			vector = new Vector3(0f - num, 0f - num2, 0f);
			break;
		case UIMoveAnimationDirection.BottomRight:
			vector = new Vector3(num, 0f - num2, 0f);
			break;
		}
		switch (moveMode)
		{
		case MoveMode.MoveIn:
			target.anchoredPosition3D = (isCustom ? startValue : vector);
			return target.DOAnchorPos3D(endValue, instant ? 0f : duration).SetDelay(instant ? 0f : delay).SetEase(ease);
		case MoveMode.MoveOut:
			target.anchoredPosition3D = startValue;
			return target.DOAnchorPos3D(isCustom ? endValue : vector, instant ? 0f : duration).SetDelay(instant ? 0f : delay).SetEase(ease);
		default:
			return null;
		}
	}
}
