using System;

namespace SK.Framework;

[Serializable]
public sealed class AxisInput : AbstractInput<string, float>
{
	public override bool IsValid => !string.IsNullOrEmpty(key);

	public float ReadValue()
	{
		if (!InputMaster.Toggle)
		{
			return 0f;
		}
		return InputMaster.Axis.GetAxis(this);
	}

	public float ReadRawValue()
	{
		if (!InputMaster.Toggle)
		{
			return 0f;
		}
		return InputMaster.Axis.GetAxisRaw(this);
	}

	public AxisInput()
	{
	}

	public AxisInput(string axisName)
		: base(axisName)
	{
	}

	protected override bool IsEqual(string k1, string k2)
	{
		return k1 == k2;
	}

	protected override void Register()
	{
		InputMaster.Axis.Register(this);
	}

	protected override void Unregister()
	{
		InputMaster.Axis.Unregister(this);
	}
}
