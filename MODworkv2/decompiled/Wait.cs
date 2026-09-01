using System;
using SK.Framework;
using UnityEngine;

public static class Wait
{
	public static void wait(this MonoBehaviour self, float delay, Action action)
	{
		self.Sequence().Delay(delay, action).Begin();
	}
}
