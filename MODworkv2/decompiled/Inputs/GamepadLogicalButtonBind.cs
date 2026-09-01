namespace Inputs;

public class GamepadLogicalButtonBind : BindKey
{
	private readonly string rawKey;

	public GamepadLogicalButtonBind(string rawKey)
	{
		this.rawKey = rawKey;
	}

	public override bool GetDown()
	{
		return GamepadInputManager.GetKeyDown(rawKey);
	}

	public override bool Get()
	{
		return GamepadInputManager.GetKey(rawKey);
	}

	public override bool GetUp()
	{
		return GamepadInputManager.GetKeyUp(rawKey);
	}
}
