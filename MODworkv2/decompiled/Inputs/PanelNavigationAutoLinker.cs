using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inputs;

public class PanelNavigationAutoLinker : MonoBehaviour
{
	[Header("作用域")]
	[SerializeField]
	private Transform root;

	[Header("执行时机")]
	[SerializeField]
	private bool autoRebuildOnValidate;

	[SerializeField]
	private bool autoRebuildOnEnable;

	[SerializeField]
	private bool rebuildNextFrameOnEnable;

	[SerializeField]
	private int delayedFrameCount = 1;

	[Header("扫描选项")]
	[SerializeField]
	private bool includeInactive;

	[SerializeField]
	private bool ignoreNonInteractable = true;

	[Header("组件类型过滤")]
	[SerializeField]
	private bool includeButton = true;

	[SerializeField]
	private bool includeToggle = true;

	[SerializeField]
	private bool includeSlider = true;

	[SerializeField]
	private bool includeDropdown = true;

	[SerializeField]
	private bool includeInputField = true;

	[SerializeField]
	private bool includeScrollbar;

	[Header("方向判定")]
	[Tooltip("候选方向与目标方向夹角越大越容易被排除。数值越小越严格。")]
	[Range(0.1f, 1f)]
	[SerializeField]
	private float directionDotThreshold = 0.35f;

	[Tooltip("距离惩罚权重。越大越倾向最近按钮。")]
	[SerializeField]
	private float distanceWeight = 0.25f;

	[Tooltip("轴向偏移惩罚权重。越大越偏好真正同一行/列。")]
	[SerializeField]
	private float axisPenaltyWeight = 1f;

	private Coroutine rebuildCoroutine;

	private void Reset()
	{
		root = base.transform;
	}

	private void OnEnable()
	{
		if (autoRebuildOnEnable)
		{
			RebuildNavigation();
		}
		if (rebuildNextFrameOnEnable)
		{
			RequestRebuildDelayed();
		}
	}

	[ContextMenu("延迟重建导航")]
	public void RequestRebuildDelayed()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (rebuildCoroutine != null)
			{
				StopCoroutine(rebuildCoroutine);
			}
			rebuildCoroutine = StartCoroutine(CoRebuildDelayed());
		}
	}

	private IEnumerator CoRebuildDelayed()
	{
		int frameCount = Mathf.Max(1, delayedFrameCount);
		for (int i = 0; i < frameCount; i++)
		{
			yield return null;
		}
		Canvas.ForceUpdateCanvases();
		if ((root ? root : base.transform) is RectTransform layoutRoot)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
		}
		Canvas.ForceUpdateCanvases();
		RebuildNavigation();
		rebuildCoroutine = null;
	}

	[ContextMenu("立即重建导航")]
	public void RebuildNavigation()
	{
		Transform scopeRoot = (root ? root : base.transform);
		List<Selectable> list = CollectSelectables(scopeRoot);
		if (list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Selectable selectable = list[i];
			if (IsSelectableUsable(selectable))
			{
				Navigation navigation = selectable.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = FindBest(selectable, list, Vector2.up);
				navigation.selectOnDown = FindBest(selectable, list, Vector2.down);
				navigation.selectOnLeft = FindBest(selectable, list, Vector2.left);
				navigation.selectOnRight = FindBest(selectable, list, Vector2.right);
				selectable.navigation = navigation;
			}
		}
	}

	[ContextMenu("清除导航")]
	public void ClearNavigation()
	{
		Transform scopeRoot = (root ? root : base.transform);
		List<Selectable> list = CollectSelectables(scopeRoot);
		for (int i = 0; i < list.Count; i++)
		{
			if ((bool)list[i])
			{
				Navigation navigation = list[i].navigation;
				navigation.mode = Navigation.Mode.None;
				navigation.selectOnUp = null;
				navigation.selectOnDown = null;
				navigation.selectOnLeft = null;
				navigation.selectOnRight = null;
				list[i].navigation = navigation;
			}
		}
	}

	private List<Selectable> CollectSelectables(Transform scopeRoot)
	{
		List<Selectable> list = new List<Selectable>();
		Selectable[] componentsInChildren = scopeRoot.GetComponentsInChildren<Selectable>(includeInactive);
		foreach (Selectable selectable in componentsInChildren)
		{
			if ((bool)selectable && IsSelectableTypeAllowed(selectable) && (!ignoreNonInteractable || selectable.IsInteractable()) && (includeInactive || selectable.gameObject.activeInHierarchy))
			{
				list.Add(selectable);
			}
		}
		return list;
	}

	private bool IsSelectableTypeAllowed(Selectable s)
	{
		if ((object)s != null)
		{
			if (s is Button)
			{
				return includeButton;
			}
			if (s is Toggle)
			{
				return includeToggle;
			}
			if (s is Slider)
			{
				return includeSlider;
			}
			if (s is Dropdown)
			{
				return includeDropdown;
			}
			if (s is InputField)
			{
				return includeInputField;
			}
			if (s is Scrollbar)
			{
				return includeScrollbar;
			}
		}
		return true;
	}

	private bool IsSelectableUsable(Selectable s)
	{
		if (!s)
		{
			return false;
		}
		if (!includeInactive && !s.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (ignoreNonInteractable && !s.IsInteractable())
		{
			return false;
		}
		return true;
	}

	private Selectable FindBest(Selectable from, List<Selectable> candidates, Vector2 wantedDir)
	{
		RectTransform rectTransform = from.transform as RectTransform;
		if (!rectTransform)
		{
			return null;
		}
		Vector2 worldCenter = GetWorldCenter(rectTransform);
		Selectable result = null;
		float num = float.MaxValue;
		for (int i = 0; i < candidates.Count; i++)
		{
			Selectable selectable = candidates[i];
			if (!selectable || selectable == from || !IsSelectableUsable(selectable))
			{
				continue;
			}
			RectTransform rectTransform2 = selectable.transform as RectTransform;
			if (!rectTransform2)
			{
				continue;
			}
			Vector2 vector = GetWorldCenter(rectTransform2) - worldCenter;
			if (vector.sqrMagnitude <= 0.0001f)
			{
				continue;
			}
			float num2 = Vector2.Dot(vector.normalized, wantedDir);
			if (!(num2 <= directionDotThreshold))
			{
				float num3;
				float num4;
				if (wantedDir == Vector2.up || wantedDir == Vector2.down)
				{
					num3 = Mathf.Abs(vector.y);
					num4 = Mathf.Abs(vector.x);
				}
				else
				{
					num3 = Mathf.Abs(vector.x);
					num4 = Mathf.Abs(vector.y);
				}
				float num5 = num3 + num4 * axisPenaltyWeight + vector.magnitude * distanceWeight - num2 * 100f;
				if (num5 < num)
				{
					num = num5;
					result = selectable;
				}
			}
		}
		return result;
	}

	private static Vector2 GetWorldCenter(RectTransform rect)
	{
		Vector3[] array = new Vector3[4];
		rect.GetWorldCorners(array);
		return (array[0] + array[2]) * 0.5f;
	}
}
