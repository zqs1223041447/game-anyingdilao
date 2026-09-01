using UnityEngine;

namespace SK.Framework;

public class SwitchableObjectHandler : SwitchableObject
{
	[SerializeField]
	private SwitchableObject[] handleArray;

	public override void Open()
	{
		if (state != 0)
		{
			state = SwitchState.Open;
			for (int i = 0; i < handleArray.Length; i++)
			{
				handleArray[i].Open();
			}
		}
	}

	public override void Close()
	{
		if (state != SwitchState.Close)
		{
			state = SwitchState.Close;
			for (int i = 0; i < handleArray.Length; i++)
			{
				handleArray[i].Close();
			}
		}
	}
}
