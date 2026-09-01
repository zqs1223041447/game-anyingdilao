using System;

namespace SK.Framework;

public class SimpleAction : AbstractAction
{
	public SimpleAction(Action action)
	{
		onCompleted = action;
	}

	protected override void OnInvoke()
	{
		isCompleted = true;
	}
}
