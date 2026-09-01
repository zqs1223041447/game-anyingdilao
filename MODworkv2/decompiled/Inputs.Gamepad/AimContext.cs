using UnityEngine;

namespace Inputs.Gamepad;

public struct AimContext
{
	public Vector3 WorldPoint;

	public Vector2 Direction;

	public bool HasDirection;

	public bool HasTargetPoint;

	public bool IsGamepad;
}
