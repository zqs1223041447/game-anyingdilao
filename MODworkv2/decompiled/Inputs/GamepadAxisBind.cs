using UnityEngine;

namespace Inputs;

public class GamepadAxisBind : BindKey
{
	private readonly string axisName;

	private readonly int direction;

	private readonly float threshold;

	private bool lastPressed;

	public GamepadAxisBind(string axisName, int direction, float threshold = 0.5f)
	{
		this.axisName = axisName;
		this.direction = direction;
		this.threshold = Mathf.Abs(threshold);
	}

	public override bool GetDown()
	{
		bool flag = IsPressed();
		bool result = flag && !lastPressed;
		lastPressed = flag;
		return result;
	}

	public override bool Get()
	{
		return lastPressed = IsPressed();
	}

	public override bool GetUp()
	{
		bool flag = IsPressed();
		bool result = !flag && lastPressed;
		lastPressed = flag;
		return result;
	}

	private bool IsPressed()
	{
		float axisRaw = Input.GetAxisRaw(axisName);
		if (direction > 0)
		{
			return axisRaw >= threshold;
		}
		return axisRaw <= 0f - threshold;
	}
}
