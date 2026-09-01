using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CustomLayoutGroup : LayoutGroup
{
	[Header("Layout Mode")]
	public FlowDirection direction;

	[Header("Grid Options")]
	[Tooltip("一排最多多少个，0 表示不限制")]
	public int maxItemsPerLine;

	[Tooltip("强制一排多少个，0 表示不强制")]
	public int fixedItemsPerLine;

	[Header("Cell")]
	public bool useChildPreferredSize = true;

	public Vector2 cellSize = new Vector2(100f, 100f);

	[Header("Spacing")]
	public Vector2 spacing = new Vector2(10f, 10f);

	[Header("Alignment Extension")]
	[Tooltip("当使用 Center 对齐时，如果当前行未满 itemsPerLine，则按左对齐处理")]
	public bool centerOnlyWhenFullLine = true;

	public override void CalculateLayoutInputVertical()
	{
		float num = base.padding.vertical;
		int count = base.rectChildren.Count;
		if (count == 0)
		{
			SetLayoutInputForAxis(0f, 0f, 0f, 1);
			return;
		}
		int num2 = ResolveItemsPerLine();
		float num3 = 0f;
		int num4 = 0;
		for (int i = 0; i < count; i++)
		{
			RectTransform child = base.rectChildren[i];
			num3 = Mathf.Max(num3, GetChildSize(child).y);
			num4++;
			if (num4 >= num2)
			{
				num += num3;
				num += spacing.y;
				num3 = 0f;
				num4 = 0;
			}
		}
		if (num4 > 0)
		{
			num += num3;
		}
		if (count > num2)
		{
			num -= spacing.y;
		}
		SetLayoutInputForAxis(num, num, num, 1);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		SetDirty();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SetDirty();
	}

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		float num = base.padding.horizontal;
		int count = base.rectChildren.Count;
		if (count == 0)
		{
			SetLayoutInputForAxis(0f, 0f, 0f, 0);
			return;
		}
		int num2 = ResolveItemsPerLine();
		float num3 = 0f;
		float num4 = 0f;
		int num5 = 0;
		for (int i = 0; i < count; i++)
		{
			num3 += GetChildSize(base.rectChildren[i]).x;
			num5++;
			if (num5 < num2)
			{
				num3 += spacing.x;
			}
			if (num5 >= num2)
			{
				num4 = Mathf.Max(num4, num3);
				num3 = 0f;
				num5 = 0;
			}
		}
		if (num5 > 0)
		{
			num4 = Mathf.Max(num4, num3);
		}
		num += num4;
		SetLayoutInputForAxis(num, num, num, 0);
	}

	public override void SetLayoutHorizontal()
	{
		SetLayout();
	}

	public override void SetLayoutVertical()
	{
		SetLayout();
	}

	private void SetLayout()
	{
		int count = base.rectChildren.Count;
		if (count == 0)
		{
			return;
		}
		int num = ResolveItemsPerLine();
		int num2 = 0;
		float num3 = base.padding.top;
		while (num2 < count)
		{
			float num4 = 0f;
			float num5 = 0f;
			int num6 = num2;
			int num7 = Mathf.Min(num2 + num, count);
			for (int i = num6; i < num7; i++)
			{
				Vector2 childSize = GetChildSize(base.rectChildren[i]);
				num4 += childSize.x;
				if (i > num6)
				{
					num4 += spacing.x;
				}
				num5 = Mathf.Max(num5, childSize.y);
			}
			int num8 = num7 - num6;
			bool flag = Mathf.Approximately(GetAlignmentOnAxis(0), 0.5f);
			bool flag2 = num8 >= num;
			float num9 = num4;
			if (centerOnlyWhenFullLine && flag && !flag2 && fixedItemsPerLine > 0)
			{
				float x = GetChildSize(base.rectChildren[num6]).x;
				int num10 = fixedItemsPerLine - num8;
				num9 += (float)num10 * x;
				num9 += (float)num10 * spacing.x;
			}
			float num11 = GetStartOffset(0, num9);
			for (int j = num6; j < num7; j++)
			{
				RectTransform rectTransform = base.rectChildren[j];
				Vector2 childSize2 = GetChildSize(rectTransform);
				SetChildAlongAxis(rectTransform, 0, num11, childSize2.x);
				SetChildAlongAxis(rectTransform, 1, num3, childSize2.y);
				num11 += childSize2.x + spacing.x;
			}
			num3 += num5 + spacing.y;
			num2 = num7;
		}
	}

	private int ResolveItemsPerLine()
	{
		switch (direction)
		{
		case FlowDirection.Vertical:
			return 1;
		case FlowDirection.Grid:
			if (fixedItemsPerLine > 0)
			{
				return fixedItemsPerLine;
			}
			if (maxItemsPerLine > 0)
			{
				return maxItemsPerLine;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case FlowDirection.Horizontal:
			break;
		}
		return int.MaxValue;
	}

	private Vector2 GetChildSize(RectTransform child)
	{
		if (useChildPreferredSize)
		{
			float x = LayoutUtility.GetPreferredWidth(child);
			float y = LayoutUtility.GetPreferredHeight(child);
			return new Vector2(x, y);
		}
		return cellSize;
	}
}
