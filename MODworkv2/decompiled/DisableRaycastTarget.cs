using System.Collections.Generic;
using FinkFramework.Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

public class DisableRaycastTarget : MonoBehaviour
{
	[Header("是否包含未激活的子物体")]
	[SerializeField]
	private bool includeInactive = true;

	[Header("是否开启 Raycast Target")]
	[SerializeField]
	private bool enableRaycastTarget;

	[Header("按物体名称排除（命中后该物体及其子物体全部跳过）")]
	[SerializeField]
	private List<string> excludeObjectNames = new List<string>();

	[ContextMenu("按当前配置执行 Raycast Target 设置")]
	public void ApplyRaycastTargetSetting()
	{
		int skipCount;
		int totalCount;
		int num = ApplyImageRaycastTarget(enableRaycastTarget, out skipCount, out totalCount);
		int skipCount2;
		int totalCount2;
		int num2 = ApplyTextRaycastTarget(enableRaycastTarget, out skipCount2, out totalCount2);
		string text = (enableRaycastTarget ? "开启" : "关闭");
		LogUtil.Success("[DisableRaycastTarget] 已" + text + " Raycast Target。" + $" Image：修改 {num} 个，跳过 {skipCount} 个，扫描总数 {totalCount}。" + $" Text：修改 {num2} 个，跳过 {skipCount2} 个，扫描总数 {totalCount2}。");
	}

	[ContextMenu("强制开启自己和所有子物体 Image + Text 的 Raycast Target")]
	public void EnableAllRaycastTarget()
	{
		int skipCount;
		int totalCount;
		int num = ApplyImageRaycastTarget(targetState: true, out skipCount, out totalCount);
		int skipCount2;
		int totalCount2;
		int num2 = ApplyTextRaycastTarget(targetState: true, out skipCount2, out totalCount2);
		LogUtil.Success("[DisableRaycastTarget] 已强制开启 Raycast Target。" + $" Image：修改 {num} 个，跳过 {skipCount} 个，扫描总数 {totalCount}。" + $" Text：修改 {num2} 个，跳过 {skipCount2} 个，扫描总数 {totalCount2}。");
	}

	[ContextMenu("强制关闭自己和所有子物体 Image + Text 的 Raycast Target")]
	public void DisableAllRaycastTarget()
	{
		int skipCount;
		int totalCount;
		int num = ApplyImageRaycastTarget(targetState: false, out skipCount, out totalCount);
		int skipCount2;
		int totalCount2;
		int num2 = ApplyTextRaycastTarget(targetState: false, out skipCount2, out totalCount2);
		LogUtil.Success("[DisableRaycastTarget] 已强制关闭 Raycast Target。" + $" Image：修改 {num} 个，跳过 {skipCount} 个，扫描总数 {totalCount}。" + $" Text：修改 {num2} 个，跳过 {skipCount2} 个，扫描总数 {totalCount2}。");
	}

	private int ApplyImageRaycastTarget(bool targetState, out int skipCount, out int totalCount)
	{
		Image[] componentsInChildren = GetComponentsInChildren<Image>(includeInactive);
		int num = 0;
		skipCount = 0;
		totalCount = componentsInChildren.Length;
		Image[] array = componentsInChildren;
		foreach (Image image in array)
		{
			if ((bool)image)
			{
				if (ShouldSkipByName(image.transform))
				{
					skipCount++;
				}
				else if (image.raycastTarget != targetState)
				{
					image.raycastTarget = targetState;
					num++;
				}
			}
		}
		return num;
	}

	private int ApplyTextRaycastTarget(bool targetState, out int skipCount, out int totalCount)
	{
		Text[] componentsInChildren = GetComponentsInChildren<Text>(includeInactive);
		int num = 0;
		skipCount = 0;
		totalCount = componentsInChildren.Length;
		Text[] array = componentsInChildren;
		foreach (Text text in array)
		{
			if ((bool)text)
			{
				if (ShouldSkipByName(text.transform))
				{
					skipCount++;
				}
				else if (text.raycastTarget != targetState)
				{
					text.raycastTarget = targetState;
					num++;
				}
			}
		}
		return num;
	}

	private bool ShouldSkipByName(Transform current)
	{
		if (excludeObjectNames == null || excludeObjectNames.Count == 0)
		{
			return false;
		}
		Transform transform = current;
		while ((bool)transform)
		{
			for (int i = 0; i < excludeObjectNames.Count; i++)
			{
				string text = excludeObjectNames[i];
				if (!string.IsNullOrWhiteSpace(text) && transform.name == text)
				{
					return true;
				}
			}
			transform = transform.parent;
		}
		return false;
	}
}
