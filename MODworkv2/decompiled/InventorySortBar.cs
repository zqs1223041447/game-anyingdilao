using System;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

public static class InventorySortBar
{
	private const float ButtonWidth = 120f;

	private const float ButtonHeight = 28f;

	private const float ButtonSpacing = 12f;

	private const float GroupLeftGap = 8f;

	private const float GroupTotalHeight = 68f;

	private static readonly string[] ButtonNames = new string[2] { "等级", "稀有度" };

	private static readonly int[] SortModeBase = new int[2] { 2, 0 };

	private static readonly int[] DefaultDirections = new int[2] { 1, 1 };

	private static bool _shown;

	private static bool _building;

	private static GameObject _barRoot;

	private static readonly Button[] _buttons = new Button[2];

	private static readonly Text[] _labels = new Text[2];

	private static readonly Color[] _baseBg = new Color[2];

	private static Font _cachedFont;

	private static int _activeField = -1;

	private static bool _activeAsc;

	public static void Tick(InventoryManager inv)
	{
		try
		{
			if (inv == null || inv.cav == null)
			{
				return;
			}
			bool flag = inv.cav.alpha > 0.5f && inv.cav.blocksRaycasts;
			if (flag && !_shown)
			{
				_shown = true;
				EnsureBuilt(inv);
				UpdateVisuals();
			}
			else if (!flag && _shown)
			{
				_shown = false;
				_activeField = -1;
				if (_barRoot != null)
				{
					_barRoot.SetActive(value: false);
				}
			}
			else if (flag && _barRoot != null && !_barRoot.activeSelf)
			{
				_barRoot.SetActive(value: true);
				UpdateVisuals();
			}
		}
		catch (Exception ex)
		{
			LogUtil.Info("InventorySortBar.Tick 异常: " + ex);
		}
	}

	private static void EnsureBuilt(InventoryManager inv)
	{
		if (_barRoot != null)
		{
			_barRoot.SetActive(value: true);
		}
		else
		{
			if (_building || inv == null)
			{
				return;
			}
			_building = true;
			try
			{
				RectTransform rectTransform = inv.transform.Find("Gird") as RectTransform;
				if (rectTransform == null && inv.IVgird != null)
				{
					rectTransform = inv.IVgird.transform as RectTransform;
				}
				if (rectTransform == null)
				{
					return;
				}
				GameObject gameObject = new GameObject("InventorySortBar", typeof(RectTransform));
				RectTransform rectTransform2 = (RectTransform)gameObject.transform;
				rectTransform2.SetParent(rectTransform, worldPositionStays: false);
				rectTransform2.anchorMin = new Vector2(0f, 0.5f);
				rectTransform2.anchorMax = new Vector2(0f, 0.5f);
				rectTransform2.pivot = new Vector2(1f, 0.5f);
				rectTransform2.sizeDelta = new Vector2(120f, 68f);
				rectTransform2.anchoredPosition = new Vector2(-8f, 0f);
				rectTransform2.localScale = Vector3.one;
				_barRoot = gameObject;
				Button button = inv.closeBtn;
				if (button == null)
				{
					button = inv.leftPage;
				}
				Sprite bgSprite = null;
				Color bgColor = new Color32(30, 30, 30, 200);
				if (button != null)
				{
					Image component = button.GetComponent<Image>();
					if (component != null)
					{
						bgSprite = component.sprite;
						bgColor = component.color;
					}
				}
				Font font = ResolveFont(inv);
				for (int i = 0; i < 2; i++)
				{
					BuildSortButton(rectTransform2, bgSprite, bgColor, font, i);
				}
				UpdateVisuals();
			}
			catch (Exception ex)
			{
				LogUtil.Info("InventorySortBar.EnsureBuilt 异常: " + ex);
			}
			finally
			{
				_building = false;
			}
		}
	}

	private static void BuildSortButton(RectTransform parent, Sprite bgSprite, Color bgColor, Font font, int index)
	{
		GameObject gameObject = new GameObject("SortBtn_" + ButtonNames[index], typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		gameObject.transform.localScale = Vector3.one;
		RectTransform obj = (RectTransform)gameObject.transform;
		obj.anchorMin = new Vector2(1f, 0.5f);
		obj.anchorMax = new Vector2(1f, 0.5f);
		obj.pivot = new Vector2(1f, 0.5f);
		obj.anchoredPosition = new Vector2(0f, 14f - (float)index * 40f);
		obj.sizeDelta = new Vector2(120f, 28f);
		Image component = gameObject.GetComponent<Image>();
		component.sprite = bgSprite;
		component.color = bgColor;
		component.type = ((bgSprite != null) ? Image.Type.Sliced : Image.Type.Simple);
		component.raycastTarget = true;
		_baseBg[index] = bgColor;
		Button button = gameObject.AddComponent<Button>();
		button.targetGraphic = component;
		button.transition = Selectable.Transition.ColorTint;
		button.onClick.RemoveAllListeners();
		int captured = index;
		button.onClick.AddListener(delegate
		{
			try
			{
				OnSortClicked(captured);
			}
			catch (Exception ex)
			{
				LogUtil.Info("InventorySortBar 点击异常: " + ex);
			}
		});
		_buttons[index] = button;
		GameObject gameObject2 = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		RectTransform obj2 = (RectTransform)gameObject2.transform;
		obj2.anchorMin = Vector2.zero;
		obj2.anchorMax = Vector2.one;
		obj2.offsetMin = Vector2.zero;
		obj2.offsetMax = Vector2.zero;
		Text component2 = gameObject2.GetComponent<Text>();
		component2.font = font;
		component2.fontSize = 17;
		component2.alignment = TextAnchor.MiddleCenter;
		component2.color = Color.white;
		component2.raycastTarget = false;
		component2.horizontalOverflow = HorizontalWrapMode.Overflow;
		component2.verticalOverflow = VerticalWrapMode.Overflow;
		component2.text = ButtonNames[index];
		_labels[index] = component2;
	}

	private static Font ResolveFont(InventoryManager inv)
	{
		if (_cachedFont != null)
		{
			return _cachedFont;
		}
		Text[] array = Resources.FindObjectsOfTypeAll<Text>();
		foreach (Text text in array)
		{
			if (text != null && text.font != null && ContainsCjk(text.text))
			{
				_cachedFont = text.font;
				return _cachedFont;
			}
		}
		string[] array2 = new string[10] { "SimHei", "YaHei", "MSYH", "SourceHan", "Noto Sans CJK", "CJK", "PingFang", "Hiragino", "WenQuanYi", "Hei" };
		Font[] array3 = Resources.FindObjectsOfTypeAll<Font>();
		foreach (Font font in array3)
		{
			if (font == null)
			{
				continue;
			}
			string[] array4 = array2;
			foreach (string value in array4)
			{
				if (font.name.Contains(value))
				{
					_cachedFont = font;
					return _cachedFont;
				}
			}
		}
		if (inv != null && inv.pageText != null && inv.pageText.font != null)
		{
			_cachedFont = inv.pageText.font;
			return _cachedFont;
		}
		_cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
		return _cachedFont;
	}

	private static bool ContainsCjk(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return false;
		}
		foreach (char c in s)
		{
			if (c >= '一' && c <= '\u9fff')
			{
				return true;
			}
		}
		return false;
	}

	private static void OnSortClicked(int index)
	{
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			InventoryManager instance = SingletonMonoScope<InventoryManager>.Instance;
			if (!(instance == null))
			{
				InventorySortMode inventorySortMode = NextModeFor(index);
				_activeField = index;
				_activeAsc = (int)inventorySortMode % 2 == 0;
				UpdateVisuals();
				instance.ApplySort(inventorySortMode);
			}
		}
	}

	private static InventorySortMode NextModeFor(int index)
	{
		int num = ((_activeField == index) ? (_activeAsc ? 1 : 0) : DefaultDirections[index]);
		return (InventorySortMode)(SortModeBase[index] + num);
	}

	private static void UpdateVisuals()
	{
		for (int i = 0; i < 2; i++)
		{
			bool flag = _activeField == i;
			if (_labels[i] != null)
			{
				_labels[i].text = (flag ? (ButtonNames[i] + (_activeAsc ? " ↑" : " ↓")) : ButtonNames[i]);
				_labels[i].color = (flag ? Color.white : new Color(0.78f, 0.78f, 0.78f, 0.62f));
			}
			if (_buttons[i] != null)
			{
				Image image = _buttons[i].image;
				if (image != null)
				{
					Color color = _baseBg[i];
					image.color = (flag ? color : new Color(color.r, color.g, color.b, color.a * 0.45f));
				}
			}
		}
	}
}
