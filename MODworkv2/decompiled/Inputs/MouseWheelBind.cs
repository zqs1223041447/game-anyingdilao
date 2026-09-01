using UnityEngine;

namespace Inputs;

public class MouseWheelBind : BindKey
{
	private readonly int direction;

	public MouseWheelBind(int direction)
	{
		this.direction = ((direction >= 0) ? 1 : (-1));
	}

	public override bool GetDown()
	{
		float y = Input.mouseScrollDelta.y;
		if (direction <= 0)
		{
			return y < 0f;
		}
		return y > 0f;
	}

	public override bool Get()
	{
		return false;
	}

	public override bool GetUp()
	{
		return false;
	}
}
