using System;
using UnityEngine;

namespace SK.Framework;

[Serializable]
public sealed class KeyInput : AbstractInput<KeyCode, bool>
{
	public override bool IsValid => key != KeyCode.None;

	public bool IsPressed
	{
		get
		{
			if (InputMaster.Toggle)
			{
				return InputMaster.Key.GetKeyDown(this);
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
				return InputMaster.Key.GetKey(this);
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
				return InputMaster.Key.GetKeyUp(this);
			}
			return false;
		}
	}

	public KeyInput()
	{
	}

	public KeyInput(KeyCode keyCode)
		: base(keyCode)
	{
	}

	protected override bool IsEqual(KeyCode k1, KeyCode k2)
	{
		return k1 == k2;
	}

	protected override void Register()
	{
		InputMaster.Key.Register(this);
	}

	protected override void Unregister()
	{
		InputMaster.Key.Unregister(this);
	}
}
