using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using UnityEngine;

namespace Inputs.Gamepad;

public static class AimProvider
{
	public static Vector3 GetPointerWorldPos()
	{
		Vector3 result = Vector3.zero;
		if (SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			result = SingletonMonoScope<CursorInputManager>.Instance.WorldPosition;
		}
		result.z = 0f;
		return result;
	}

	public static Vector3 GetAimWorldPos()
	{
		Vector3 worldPoint = GetCurrentAimContext().WorldPoint;
		worldPoint.z = 0f;
		return worldPoint;
	}

	public static AimContext GetCurrentAimContext()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent() && SingletonMonoScope<GamepadAimManager>.HasInstance && GamepadAimManager.IsUsingGamepadAim)
		{
			return SingletonMonoScope<GamepadAimManager>.Instance.GetAimContext();
		}
		Vector3 vector = Vector3.zero;
		Vector2 direction = Vector2.right;
		if (SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			vector = SingletonMonoScope<CursorInputManager>.Instance.WorldPosition;
		}
		if (SingletonMonoScope<PlayerManager>.HasInstance && (bool)SingletonMonoScope<PlayerManager>.Instance.transform)
		{
			Vector3 position = SingletonMonoScope<PlayerManager>.Instance.transform.position;
			Vector2 vector2 = vector - position;
			if (vector2.sqrMagnitude > 0.0001f)
			{
				direction = vector2.normalized;
			}
		}
		AimContext result = default(AimContext);
		result.WorldPoint = vector;
		result.Direction = direction;
		result.HasDirection = true;
		result.HasTargetPoint = true;
		result.IsGamepad = false;
		return result;
	}
}
