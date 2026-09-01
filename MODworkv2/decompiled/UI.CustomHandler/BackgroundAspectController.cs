using UnityEngine;

namespace UI.CustomHandler;

[RequireComponent(typeof(RectTransform))]
public class BackgroundAspectController : MonoBehaviour
{
	private const float MaxAllowedAspect = 1.7777778f;

	private RectTransform rect;

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		Apply();
	}

	public void Apply()
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (num <= 1.7777778f)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
		}
		else
		{
			float num2 = 1.7777778f / num;
			float num3 = (1f - num2) / 2f;
			rect.anchorMin = new Vector2(num3, 0f);
			rect.anchorMax = new Vector2(1f - num3, 1f);
		}
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
	}
}
