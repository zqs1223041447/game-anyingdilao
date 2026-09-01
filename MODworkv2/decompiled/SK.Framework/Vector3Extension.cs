using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public static class Vector3Extension
{
	public static float[] ToArray(this Vector3 self)
	{
		return new float[3] { self.x, self.y, self.z };
	}

	public static Quaternion ToQuaternion(this Vector3 self)
	{
		return Quaternion.Euler(self);
	}

	public static List<Vector3> GetMin(this List<Vector3> self, out Vector3 min)
	{
		min = self[0];
		for (int i = 1; i < self.Count; i++)
		{
			min = Vector3.Min(min, self[i]);
		}
		return self;
	}

	public static List<Vector3> GetMax(this List<Vector3> self, out Vector3 max)
	{
		max = self[0];
		for (int i = 1; i < self.Count; i++)
		{
			max = Vector3.Max(max, self[i]);
		}
		return self;
	}

	public static Vector3[] GetPositions(this List<Transform> self)
	{
		Vector3[] array = new Vector3[self.Count];
		for (int i = 0; i < self.Count; i++)
		{
			array[i] = self[i].position;
		}
		return array;
	}

	public static Vector3[] GetPositions(this Transform[] self)
	{
		Vector3[] array = new Vector3[self.Length];
		for (int i = 0; i < self.Length; i++)
		{
			array[i] = self[i].position;
		}
		return array;
	}

	public static Mesh GenerateMesh(this Vector3[] self)
	{
		Mesh mesh = new Mesh();
		List<int> list = new List<int>();
		for (int i = 0; i < self.Length - 1; i++)
		{
			list.Add(i);
			list.Add(i + 1);
			list.Add(self.Length - i - 1);
		}
		mesh.vertices = self;
		mesh.triangles = list.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		return mesh;
	}

	public static Vector3[] GenerateBeizer(this Vector3 self, Vector3 startPoint, Vector3 endPoint, int count)
	{
		Vector3[] array = new Vector3[count];
		for (int i = 1; i <= count; i++)
		{
			float num = (float)i / (float)count;
			float num2 = 1f - num;
			float num3 = Mathf.Pow(num, 2f);
			Vector3 vector = Mathf.Pow(num2, 2f) * startPoint;
			vector += 2f * num2 * num * self;
			vector += num3 * endPoint;
			array[i - 1] = vector;
		}
		return array;
	}

	public static bool IsInRange(this Vector3 self, Vector3[] points, float height)
	{
		if (self.y > height || self.y < 0f - height)
		{
			return false;
		}
		Vector3 vector = (points[0] + points[1]) * 0.5f;
		vector += (vector - self).normalized * 10000f;
		int num = 0;
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 vector2 = points[i % points.Length];
			Vector3 vector3 = points[(i + 1) % points.Length];
			float a = Mathf.Sign(Vector3.Cross(vector - self, vector2 - self).y);
			float b = Mathf.Sign(Vector3.Cross(vector - self, vector3 - self).y);
			if (!Mathf.Approximately(a, b))
			{
				float a2 = Mathf.Sign(Vector3.Cross(vector3 - vector2, self - vector2).y);
				float b2 = Mathf.Sign(Vector3.Cross(vector3 - vector2, vector - vector2).y);
				if (!Mathf.Approximately(a2, b2))
				{
					num++;
				}
			}
		}
		return num % 2 == 1;
	}

	public static bool IsInPlane(this Vector3 self, Vector3[] points)
	{
		float num = 0f;
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < points.Length; i++)
		{
			if (i == 0)
			{
				vector = points[i] - self;
			}
			zero = ((i != points.Length - 1) ? (points[i + 1] - self) : (points[0] - self));
			num += Mathf.Acos(Vector3.Dot(vector.normalized, zero.normalized)) * 57.29578f;
			vector = zero;
		}
		return Mathf.Abs(num - 360f) < 0.1f;
	}

	public static Vector3 GetIntersectWithPlane(this Vector3 self, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
	{
		float num = Vector3.Dot(planePoint - self, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
		num = ((num < 0f) ? 0f : num);
		return num * direct.normalized + self;
	}
}
