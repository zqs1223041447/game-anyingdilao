using UnityEngine;

namespace Inputs;

public class MouseBind : BindKey
{
	private readonly int button;

	public MouseBind(int button)
	{
		this.button = button;
	}

	public override bool GetDown()
	{
		return Input.GetMouseButtonDown(button);
	}

	public override bool Get()
	{
		return Input.GetMouseButton(button);
	}

	public override bool GetUp()
	{
		return Input.GetMouseButtonUp(button);
	}
}
