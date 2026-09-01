using UnityEngine;

namespace SK.Framework;

public abstract class SwitchableObject : MonoBehaviour, ISwitchableObject
{
	[SerializeField]
	protected SwitchState state = SwitchState.Close;

	public SwitchState State => state;

	public void Switch()
	{
		switch (State)
		{
		case SwitchState.Open:
			Close();
			break;
		case SwitchState.Close:
			Open();
			break;
		}
	}

	public abstract void Open();

	public abstract void Close();
}
