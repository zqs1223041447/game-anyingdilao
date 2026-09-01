using System;

namespace SK.Framework;

[Serializable]
public sealed class MouseButtonInput : AbstractInput<int, bool>
{
	public override bool IsValid
	{
		get
		{
			if (key != 0 && key != 1)
			{
				return key == 2;
			}
			return true;
		}
	}

	public bool IsPressed
	{
		get
		{
			if (InputMaster.Toggle)
			{
				return InputMaster.Mouse.GetKeyDown(this);
			}
			return false;
		}
	}

	public bool IsHeld
	{
		get
		{
			if (InputMaster.Toggle)
			{
				return InputMaster.Mouse.GetKey(this);
			}
			return false;
		}
	}

	public bool IsReleased
	{
		get
		{
			if (InputMaster.Toggle)
			{
				return InputMaster.Mouse.GetKeyUp(this);
			}
			return false;
		}
	}

	public MouseButtonInput()
	{
	}

	public MouseButtonInput(int mouseButton)
		: base(mouseButton)
	{
	}

	public MouseButtonInput(MouseButtonCode mouseButtonCode)
	{
		key = (int)mouseButtonCode;
	}

	protected override bool IsEqual(int k1, int k2)
	{
		return k1 == k2;
	}

	protected override void Register()
	{
		InputMaster.Mouse.Register(this);
	}

	protected override void Unregister()
	{
		InputMaster.Mouse.Unregister(this);
	}
}
