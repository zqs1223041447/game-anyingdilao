using System;
using Container.Inventory;
using Container.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
	public IntVector2 GridPos;

	public bool isOC;

	public Image image;

	public int number;

	public ContainerType type;

	[NonSerialized]
	private Color itemColor = SlotColor.TouMing;

	private RectTransform _rectTransform;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
	}

	private void Start()
	{
		ForceStyle(_rectTransform);
	}

	private void OnEnable()
	{
		ForceStyle(_rectTransform);
	}

	private void OnDisable()
	{
		ForceStyle(_rectTransform);
	}

	public void SetItemColor(Color color)
	{
		itemColor = color;
		if ((bool)image)
		{
			image.color = color;
		}
	}

	public void RestoreItemColor()
	{
		if ((bool)image)
		{
			image.color = itemColor;
		}
	}

	public void ClearItemColor()
	{
		itemColor = SlotColor.TouMing;
		if ((bool)image)
		{
			image.color = SlotColor.TouMing;
		}
	}

	public static void ForceStyle(RectTransform rt)
	{
		if ((bool)rt)
		{
			rt.anchorMin = new Vector2(0f, 0f);
			rt.anchorMax = new Vector2(0f, 0f);
			rt.pivot = new Vector2(0f, 1f);
			rt.sizeDelta = new Vector2(60f, 60f);
			rt.localRotation = Quaternion.identity;
			rt.localScale = Vector3.one;
		}
	}
}
