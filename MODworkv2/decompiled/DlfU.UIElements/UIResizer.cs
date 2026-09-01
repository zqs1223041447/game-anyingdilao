using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DlfU.UIElements;

public class UIResizer : MouseManipulator
{
	private Vector2 m_Start;

	protected bool m_Active;

	private VisualElement targetResize;

	public Action mouseDownAction;

	public Action mouseMoveAction;

	public Action mouseUpAction;

	public UIResizer(VisualElement targetResize = null)
	{
		this.targetResize = ((targetResize == null) ? base.target : targetResize);
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
		m_Active = false;
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
		if (m_Active)
		{
			e.StopImmediatePropagation();
		}
		else if (CanStartManipulation(e))
		{
			m_Start = e.localMousePosition;
			m_Active = true;
			mouseDownAction?.Invoke();
			base.target.CaptureMouse();
			e.StopPropagation();
		}
	}

	protected void OnMouseMove(MouseMoveEvent e)
	{
		if (m_Active && base.target.HasMouseCapture())
		{
			Vector2 vector = e.localMousePosition - m_Start;
			float value = targetResize.layout.height + vector.y;
			float value2 = targetResize.layout.width + vector.x;
			Vector2 vector2 = targetResize.StyleMinSize();
			Vector2 vector3 = targetResize.StyleMaxSize();
			value = Mathf.Clamp(value, vector2.y, vector3.y);
			value2 = Mathf.Clamp(value2, vector2.x, vector3.x);
			while (value2 + targetResize.layout.xMin > targetResize.parent.layout.width)
			{
				value2 -= 1f;
			}
			while (value + targetResize.layout.yMin > targetResize.parent.layout.height)
			{
				value -= 1f;
			}
			targetResize.style.height = value;
			targetResize.style.width = value2;
			mouseMoveAction?.Invoke();
			e.StopPropagation();
		}
	}

	protected void OnMouseUp(MouseUpEvent e)
	{
		if (m_Active && base.target.HasMouseCapture() && CanStopManipulation(e))
		{
			m_Active = false;
			mouseUpAction?.Invoke();
			base.target.ReleaseMouse();
			e.StopPropagation();
		}
	}
}
