using UnityEngine;

namespace Inputs;

public class GamepadButtonBind : BindKey
{
	private readonly KeyCode key;

	public GamepadButtonBind(KeyCode key)
	{
		this.key = key;
	}

	public override bool GetDown()
	{
		return Input.GetKeyDown(key);
	}

	public override bool Get()
	{
		return Input.GetKey(key);
	}

	public override bool GetUp()
	{
		return Input.GetKeyUp(key);
	}
}
