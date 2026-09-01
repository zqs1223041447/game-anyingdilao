using UnityEngine;

namespace Inputs;

public class KeyBind : BindKey
{
	private readonly KeyCode key;

	public KeyBind(KeyCode key)
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
