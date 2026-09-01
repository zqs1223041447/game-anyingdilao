using UnityEngine;

namespace Utils;

public class RuntimeIdUtil
{
	public static string GenerateByIndex(Transform transform)
	{
		if (!transform)
		{
			return string.Empty;
		}
		Transform transform2 = transform;
		string text = $"{transform2.GetSiblingIndex()}";
		while ((bool)transform2.parent)
		{
			transform2 = transform2.parent;
			text = $"{transform2.GetSiblingIndex()}/{text}";
		}
		return text;
	}

	public static string GenerateByPos(Transform t)
	{
		Vector3 position = t.position;
		return $"{position.x:F2}_{position.y:F2}_{position.z:F2}";
	}
}
