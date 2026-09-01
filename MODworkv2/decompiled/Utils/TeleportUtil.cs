using UnityEngine;

namespace Utils;

public static class TeleportUtil
{
	public static bool GetSafeTeleportPosition(Vector2 origin, Vector2 mousePos, float maxDistance, CapsuleCollider2D capsule, LayerMask blockMask, out Vector2 safePos)
	{
		safePos = origin;
		Vector2 vector = mousePos - origin;
		if (vector.sqrMagnitude <= 0.0001f)
		{
			return false;
		}
		vector.Normalize();
		Vector2 size = capsule.size;
		float z = capsule.transform.eulerAngles.z;
		if ((bool)Physics2D.OverlapCapsule(origin, size, capsule.direction, z, blockMask))
		{
			return false;
		}
		float num = Mathf.Min(Vector2.Distance(origin, mousePos), maxDistance);
		RaycastHit2D raycastHit2D = Physics2D.CapsuleCast(origin, size, capsule.direction, z, vector, num, blockMask);
		if ((bool)raycastHit2D.collider)
		{
			float num2 = raycastHit2D.distance - 0.5f;
			if (num2 <= 0f)
			{
				return false;
			}
			safePos = origin + vector * num2;
			return true;
		}
		safePos = origin + vector * num;
		return true;
	}
}
