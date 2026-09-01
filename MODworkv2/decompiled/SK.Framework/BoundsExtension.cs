using UnityEngine;

namespace SK.Framework;

public static class BoundsExtension
{
	public static Bounds Transform(this Bounds self, Matrix4x4 transformMatrix)
	{
		Vector3 vector = transformMatrix.GetColumn(0);
		Vector3 vector2 = transformMatrix.GetColumn(1);
		Vector3 vector3 = transformMatrix.GetColumn(2);
		Vector3 vector4 = vector * self.extents.x;
		Vector3 vector5 = vector2 * self.extents.y;
		Vector3 vector6 = vector3 * self.extents.z;
		float x = (Mathf.Abs(vector4.x) + Mathf.Abs(vector5.x) + Mathf.Abs(vector6.x)) * 2f;
		float y = (Mathf.Abs(vector4.y) + Mathf.Abs(vector5.y) + Mathf.Abs(vector6.y)) * 2f;
		float z = (Mathf.Abs(vector4.z) + Mathf.Abs(vector5.z) + Mathf.Abs(vector6.z)) * 2f;
		Bounds result = default(Bounds);
		result.center = transformMatrix.MultiplyPoint(self.center);
		result.size = new Vector3(x, y, z);
		return result;
	}
}
