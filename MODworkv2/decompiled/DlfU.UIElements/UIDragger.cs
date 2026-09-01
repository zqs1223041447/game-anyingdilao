using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DlfU.UIElements;

public class UIDragger : MouseManipulator
{
	protected bool active;

	protected VisualElement moveTarget;

	public Action mouseDownAction;

	public Action mouseMoveAction;

	public Action mouseUpAction;

	protected bool alwaysIn;

	public UIDragger(VisualElement targetMove = null, bool alwaysIn = true)
	{
		moveTarget = ((targetMove == null) ? base.target : targetMove);
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
		active = false;
		this.alwaysIn = alwaysIn;
	}

	protected override void RegisterCallbacksOnTarget()
	{
		base.target.RegisterCallback<MouseDownEvent>(OnMouseDown);
		base.target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
		base.target.RegisterCallback<MouseUpEvent>(OnMouseUp);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		base.target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
		base.target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
		base.target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
	}

	protected void OnMouseDown(MouseDownEvent e)
	{
		if (active)
		{
			e.StopImmediatePropagation();
		}
		else if (CanStartManipulation(e))
		{
			active = true;
			mouseDownAction?.Invoke();
			base.target.CaptureMouse();
			e.StopPropagation();
		}
	}

	protected void OnMouseMove(MouseMoveEvent evt)
	{
		if (active)
		{
			float num = moveTarget.layout.x + evt.mouseDelta.x;
			float num2 = moveTarget.layout.y + evt.mouseDelta.y;
			if (alwaysIn)
			{
				moveTarget.style.left = Mathf.Clamp(num, 0f, moveTarget.parent.layout.width - moveTarget.layout.width);
				moveTarget.style.top = Mathf.Clamp(num2, 0f, moveTarget.parent.layout.height - moveTarget.layout.height);
				moveTarget.style.right = float.NaN;
				moveTarget.style.bottom = float.NaN;
			}
			else
			{
				moveTarget.StyleLeft(num);
				moveTarget.StyleTop(num2);
			}
			mouseMoveAction?.Invoke();
			evt.StopPropagation();
		}
	}

	protected void OnMouseUp(MouseUpEvent evt)
	{
		active = false;
		mouseUpAction?.Invoke();
		if (base.target.HasMouseCapture())
		{
			base.target.ReleaseMouse();
		}
		evt.StopPropagation();
	}
}
