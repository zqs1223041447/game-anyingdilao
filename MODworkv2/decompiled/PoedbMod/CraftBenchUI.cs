using System;
using System.Collections.Generic;
using Container.Util;
using Core;
using Entity.InteractableObjects.Item;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Interact;
using UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PoedbMod;

public class CraftBenchUI : MonoBehaviour
{
	private class Row
	{
		public Button Btn;

		public Text Label;

		public Image Bg;

		public CraftBenchOps.Op Op;

		public bool IsToggle;

		public bool IsDivider;

		public int LockId;
	}

	private class RowDef
	{
		public string Name;

		public string Desc;

		public CraftBenchOps.Op Op;

		public bool IsToggle;

		public bool IsDivider;

		public int LockId;

		private RowDef()
		{
		}

		public RowDef(string name, string desc, CraftBenchOps.Op op)
		{
			Name = name;
			Desc = desc;
			Op = op;
			LockId = -1;
		}

		public RowDef(string name, string desc, int lockId)
		{
			Name = name;
			Desc = desc;
			IsToggle = true;
			LockId = lockId;
		}

		public static RowDef Divider(string name)
		{
			return new RowDef
			{
				Name = name,
				IsDivider = true
			};
		}
	}

	private static readonly RowDef[] RowDefs = new RowDef[23]
	{
		RowDef.Divider("── 货币工艺 ──"),
		new RowDef("蜕变石", "普通 → 魔法，随机新增 1 条词缀", CraftBenchOps.Op.Transmute),
		new RowDef("增幅石", "魔法装备新增 1 条词缀", CraftBenchOps.Op.Augment),
		new RowDef("改造石", "重骰魔法装备全部词缀", CraftBenchOps.Op.Alteration),
		new RowDef("富豪石", "魔法 → 稀有，新增 1 条词缀", CraftBenchOps.Op.Regal),
		new RowDef("点金石", "普通 → 稀有，新增 4~上限 条词缀", CraftBenchOps.Op.Alchemy),
		new RowDef("点金石·精致", "普通 → 精致，新增 4~上限 条词缀", CraftBenchOps.Op.AlchemyExquisite),
		new RowDef("点金石·史诗", "普通 → 史诗，新增 4~上限 条词缀", CraftBenchOps.Op.AlchemyEpic),
		new RowDef("传说石", "普通 → 传说，新增 4~上限 条词缀", CraftBenchOps.Op.LegendaryStone),
		new RowDef("神话石", "普通 → 神话，新增 4~上限 条词缀", CraftBenchOps.Op.MythicStone),
		new RowDef("混沌石", "重骰稀有及以上装备全部词缀", CraftBenchOps.Op.Chaos),
		new RowDef("隐匿混沌石", "重骰稀有词缀，无视攻击/法术禁骰", CraftBenchOps.Op.HiddenChaos),
		new RowDef("崇高石", "稀有及以上新增 1 条词缀", CraftBenchOps.Op.Exalted),
		new RowDef("无效石", "移除 1 条随机词缀", CraftBenchOps.Op.Annulment),
		new RowDef("神圣石", "重骰现有词缀的数值", CraftBenchOps.Op.Divine),
		new RowDef("重铸石", "清空词缀回到普通（锁定组保留）", CraftBenchOps.Op.Scouring),
		new RowDef("兽猎·移前增后", "移除 1 条前缀并新增 1 条后缀", CraftBenchOps.Op.BestiaryPreToSuf),
		new RowDef("兽猎·移后增前", "移除 1 条后缀并新增 1 条前缀", CraftBenchOps.Op.BestiarySufToPre),
		RowDef.Divider("── 工艺限制（变形词缀，附加在装备上）──"),
		new RowDef("前缀无法被变更", "重骰时保留全部输出词条", 0),
		new RowDef("后缀无法被变更", "重骰时保留全部功能词条", 1),
		new RowDef("无法骰出攻击词缀", "新增/重骰不生成攻击词缀", 2),
		new RowDef("无法骰出法术词缀", "新增/重骰不生成法术词缀", 3)
	};

	private const float RowHeight = 38f;

	private const float RowGap = 4f;

	private static readonly Color RowColor = new Color(0.16f, 0.18f, 0.24f, 1f);

	private static readonly Color RowHoverColor = new Color(0.26f, 0.31f, 0.42f, 1f);

	private static readonly Color RowDisabledColor = new Color(0.1f, 0.11f, 0.15f, 0.8f);

	private static readonly Color DividerColor = new Color(0.72f, 0.64f, 0.42f, 1f);

	private static CraftBenchUI _inst;

	private WeaponManager _owner;

	private GameObject _toggleBtn;

	private GameObject _panel;

	private CanvasGroup _panelCav;

	private Text _targetText;

	private Text _moneyText;

	private readonly List<Row> _rows = new List<Row>();

	private SlotData _targetSlot;

	private bool _benchOpen;

	private long _lastMoney = -1L;

	private WeaponClass _lastTargetWeapon;

	public static void Install(WeaponManager owner)
	{
		try
		{
			if (!(owner == null) && (!(_inst != null) || !(_inst._owner != null)))
			{
				CraftBenchUI craftBenchUI = owner.gameObject.AddComponent<CraftBenchUI>();
				craftBenchUI._owner = owner;
				craftBenchUI.BuildToggleButton();
				_inst = craftBenchUI;
				Debug.Log("[CraftBench] installed on " + owner.name);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CraftBench] install failed: " + ex);
		}
	}

	private void BuildToggleButton()
	{
		try
		{
			Button closeBtn = _owner.GetCloseBtn();
			if (closeBtn == null)
			{
				Debug.LogWarning("[CraftBench] close btn not found, toggle skipped");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(closeBtn.gameObject, closeBtn.transform.parent);
			gameObject.name = "CraftToggleBtn";
			RectTransform component = gameObject.GetComponent<RectTransform>();
			RectTransform component2 = closeBtn.GetComponent<RectTransform>();
			if (component != null && component2 != null)
			{
				component.anchoredPosition = component2.anchoredPosition + new Vector2(-80f, 0f);
				component.sizeDelta = component2.sizeDelta + new Vector2(30f, 0f);
			}
			Button component3 = gameObject.GetComponent<Button>();
			component3.onClick.RemoveAllListeners();
			component3.onClick.AddListener(ToggleBench);
			Text componentInChildren = gameObject.GetComponentInChildren<Text>(includeInactive: true);
			if (componentInChildren != null)
			{
				componentInChildren.text = "工艺台";
				componentInChildren.fontSize = 20;
				componentInChildren.horizontalOverflow = HorizontalWrapMode.Overflow;
				componentInChildren.verticalOverflow = VerticalWrapMode.Overflow;
				componentInChildren.alignment = TextAnchor.MiddleCenter;
			}
			gameObject.SetActive(value: true);
			_toggleBtn = gameObject;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CraftBench] build toggle failed: " + ex);
		}
	}

	private void ToggleBench()
	{
		if (_benchOpen)
		{
			CloseBench();
		}
		else
		{
			OpenBench();
		}
	}

	private void OpenBench()
	{
		try
		{
			if (_owner == null || !_owner.Opened)
			{
				return;
			}
			GameUIManager instance = SingletonMonoScope<GameUIManager>.Instance;
			if (instance != null)
			{
				if (instance.CurrentModalState == GlobalUiModalState.WeaponElm)
				{
					_owner.ExitElm();
				}
				if (instance.CurrentModalState == GlobalUiModalState.WeaponSpc)
				{
					_owner.ExitSpc();
				}
				if (instance.CurrentModalState == GlobalUiModalState.WeaponEnh)
				{
					_owner.ExitEnh();
				}
			}
			if (_panel == null)
			{
				BuildPanel();
			}
			if (SingletonMonoScope<InteractionManager>.HasInstance)
			{
				InteractionManager.AllInteractToggle = false;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: false);
			}
			_targetSlot = null;
			_lastTargetWeapon = null;
			_lastMoney = -1L;
			_benchOpen = true;
			if (_panelCav != null)
			{
				_panelCav.alpha = 1f;
				_panelCav.blocksRaycasts = true;
				_panelCav.interactable = true;
			}
			RefreshRows(force: true);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CraftBench] open failed: " + ex);
		}
	}

	private void CloseBench()
	{
		if (_benchOpen || (!(_panel == null) && !(_panelCav.alpha <= 0f)))
		{
			_benchOpen = false;
			if (_panelCav != null)
			{
				_panelCav.alpha = 0f;
				_panelCav.blocksRaycasts = false;
				_panelCav.interactable = false;
			}
			if (SingletonMonoScope<InteractionManager>.HasInstance)
			{
				InteractionManager.AllInteractToggle = true;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: true);
			}
		}
	}

	private void Update()
	{
		if (_owner == null || _panel == null)
		{
			return;
		}
		if (_benchOpen && !_owner.Opened)
		{
			CloseBench();
		}
		else
		{
			if (!_benchOpen)
			{
				return;
			}
			HandleTargetClick();
			bool flag = false;
			InventoryManager instance = SingletonMonoScope<InventoryManager>.Instance;
			long num = ((instance != null) ? instance.GlobalMoney : 0);
			if (num != _lastMoney)
			{
				_lastMoney = num;
				flag = true;
				if (_moneyText != null)
				{
					_moneyText.text = "金币：" + num;
				}
			}
			WeaponClass weaponClass = ((_targetSlot != null) ? _targetSlot.weapon : null);
			if (weaponClass != _lastTargetWeapon)
			{
				_lastTargetWeapon = weaponClass;
				flag = true;
				if (_targetText != null)
				{
					if (weaponClass == null)
					{
						_targetText.text = "目标：（<color=#E6C96A>点击</color>背包中的装备选择）";
					}
					else
					{
						string text = ((QualityColor.Colors != null && QualityColor.Colors.ContainsKey(weaponClass.Quality)) ? QualityColor.Colors[weaponClass.Quality] : "#ffffff");
						_targetText.text = "目标：" + weaponClass.GetTitle() + "<color=" + text + ">\u3000品质：" + CraftBenchOps.QualityName(weaponClass.Quality) + "</color>\u3000词缀 " + CraftBenchOps.CountAffixLines(weaponClass) + "/" + CraftBenchOps.AffixCap(weaponClass.Quality) + "\n<size=15><color=#AAAAAA>锁定[" + (weaponClass.Craft_LockPrefix ? "前缀√" : "") + (weaponClass.Craft_LockSuffix ? " 后缀√" : "") + (weaponClass.Craft_NoAttack ? " 禁攻√" : "") + (weaponClass.Craft_NoCaster ? " 禁法√" : "") + "] </color></size>";
					}
				}
			}
			if (flag)
			{
				RefreshRows(force: true);
			}
		}
	}

	private void HandleTargetClick()
	{
		try
		{
			if (!Input.GetMouseButtonDown(0) || IsPointerOverBenchPanel() || !SingletonMonoScope<InventoryManager>.HasInstance)
			{
				return;
			}
			InventoryManager instance = SingletonMonoScope<InventoryManager>.Instance;
			SlotData mainSlot = ContainerGridUtil.GetMainSlot(instance.MouseSlotDT, instance.Page);
			if (mainSlot != null && mainSlot.isOC && mainSlot.ItemType == 0 && mainSlot.weapon != null)
			{
				WeaponClass weapon = mainSlot.weapon;
				WeaponClass weaponClass = ((_targetSlot != null) ? _targetSlot.weapon : null);
				if (weapon != weaponClass)
				{
					_targetSlot = mainSlot;
					_lastTargetWeapon = null;
					RefreshRows(force: true);
					GameManager.ShowTip("已选择工艺目标：" + weapon.GetTitle(), TipType.Info);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CraftBench] click select failed: " + ex);
		}
	}

	private bool IsPointerOverBenchPanel()
	{
		if (_panel == null || EventSystem.current == null)
		{
			return false;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = Input.mousePosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gameObject.transform.IsChildOf(_panel.transform))
			{
				return true;
			}
		}
		return false;
	}

	private void OnRowClicked(Row row)
	{
		try
		{
			if (row == null || row.IsDivider)
			{
				return;
			}
			SlotData targetSlot = _targetSlot;
			WeaponClass weaponClass = targetSlot?.weapon;
			if (targetSlot == null || weaponClass == null)
			{
				GameManager.ShowTip("请先点击背包中的装备，选择工艺目标", TipType.Info);
			}
			else if (Hand.Instance != null && (bool)Hand.Instance.ItemOBJ)
			{
				GameManager.ShowTipLocalStartKey("please_take_off_hand_item", TipType.Fail);
			}
			else
			{
				if (!SingletonMonoScope<InventoryManager>.HasInstance)
				{
					return;
				}
				InventoryManager instance = SingletonMonoScope<InventoryManager>.Instance;
				if (instance.GlobalMoney < 1)
				{
					GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
				}
				else
				{
					if (ItemCloneUtil.CloneWeapon(weaponClass) == null)
					{
						return;
					}
					if (!(row.IsToggle ? CraftBenchOps.ToggleLock(weaponClass, row.LockId, out var msg) : CraftBenchOps.Execute(row.Op, weaponClass, out msg)))
					{
						GameManager.ShowTip(msg, TipType.Fail);
						_lastMoney = -1L;
						_lastTargetWeapon = null;
						RefreshRows(force: true);
						return;
					}
					instance.RemoveMoney(1L);
					ContainerGridUtil.BindWeaponToRegion(targetSlot, instance.Page);
					if (ItemCloneUtil.CloneWeapon(weaponClass) != null && _owner != null)
					{
						try
						{
							RuntimeManager.PlayOneShot(_owner.GetForgeAudioEvent(), base.transform.position);
						}
						catch (Exception)
						{
						}
						if (SingletonMonoScope<GameUIManager>.HasInstance)
						{
							SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(targetSlot.weapon, targetSlot, instance.slotGrid);
						}
					}
					GameManager.ShowTip(msg, TipType.Success);
					_lastMoney = -1L;
					_lastTargetWeapon = null;
					RefreshRows(force: true);
				}
			}
		}
		catch (Exception ex2)
		{
			Debug.LogWarning("[CraftBench] row click failed: " + ex2);
		}
	}

	private void RefreshRows(bool force)
	{
		WeaponClass weaponClass = ((_targetSlot != null) ? _targetSlot.weapon : null);
		for (int i = 0; i < _rows.Count && i < RowDefs.Length; i++)
		{
			Row row = _rows[i];
			if (row == null || row.Label == null || RowDefs[i].IsDivider)
			{
				continue;
			}
			RowDef rowDef = RowDefs[i];
			if (!(row.Btn == null))
			{
				bool flag = weaponClass != null;
				bool flag2 = flag;
				string text;
				if (row.IsToggle)
				{
					flag2 = flag && weaponClass.Quality >= 1;
					bool flag3 = flag && IsLockOn(weaponClass, row.LockId);
					text = "<b>" + rowDef.Name + "</b>\u3000状态：<color=" + (flag3 ? "#00FF00" : "#AAAAAA") + ">" + (flag3 ? "已附加" : "未附加") + "</color>\u3000<color=#E6C96A>切换 1 金币</color>";
				}
				else
				{
					flag2 = flag && CraftBenchOps.CanUse(rowDef.Op, weaponClass);
					text = ((!flag || CraftBenchOps.CanUse(rowDef.Op, weaponClass)) ? ("<b>" + rowDef.Name + "</b>\u3000" + rowDef.Desc + "\u3000<color=#E6C96A>1 金币</color>") : ("<b>" + rowDef.Name + "</b>\u3000<color=#FF7070>" + RowReqText(rowDef.Op) + "</color>\u3000<color=#E6C96A>1 金币</color>"));
				}
				row.Btn.interactable = flag2;
				if (!flag)
				{
					text = "<b>" + rowDef.Name + "</b>\u3000<color=#AAAAAA>（未选择装备）</color>";
				}
				row.Label.text = text;
				row.Label.color = (flag2 ? new Color(0.93f, 0.93f, 0.93f, 1f) : new Color(0.5f, 0.5f, 0.52f, 1f));
			}
		}
	}

	private static bool IsLockOn(WeaponClass w, int lockId)
	{
		return lockId switch
		{
			0 => w.Craft_LockPrefix, 
			1 => w.Craft_LockSuffix, 
			2 => w.Craft_NoAttack, 
			3 => w.Craft_NoCaster, 
			_ => false, 
		};
	}

	private static string RowReqText(CraftBenchOps.Op op)
	{
		switch (op)
		{
		case CraftBenchOps.Op.Transmute:
		case CraftBenchOps.Op.Alchemy:
		case CraftBenchOps.Op.AlchemyExquisite:
		case CraftBenchOps.Op.AlchemyEpic:
		case CraftBenchOps.Op.LegendaryStone:
		case CraftBenchOps.Op.MythicStone:
			return "需要普通品质";
		case CraftBenchOps.Op.Augment:
		case CraftBenchOps.Op.Alteration:
		case CraftBenchOps.Op.Regal:
			return "需要魔法品质";
		case CraftBenchOps.Op.Chaos:
		case CraftBenchOps.Op.HiddenChaos:
		case CraftBenchOps.Op.Exalted:
			return "需要稀有及以上";
		default:
			return "需要魔法及以上";
		}
	}

	private void BuildPanel()
	{
		Font builtinResource = Resources.GetBuiltinResource<Font>("Arial.ttf");
		GameObject gameObject = new GameObject("CraftBenchPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
		gameObject.transform.SetParent(_owner.transform, worldPositionStays: false);
		RectTransform rectTransform = (RectTransform)gameObject.transform;
		RectTransform rectTransform2 = _owner.transform.Find("MainGroup") as RectTransform;
		if (rectTransform2 != null)
		{
			rectTransform.anchorMin = rectTransform2.anchorMin;
			rectTransform.anchorMax = rectTransform2.anchorMax;
			rectTransform.anchoredPosition = rectTransform2.anchoredPosition;
			rectTransform.sizeDelta = rectTransform2.sizeDelta;
			rectTransform.pivot = rectTransform2.pivot;
		}
		else
		{
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = new Vector2(660f, 760f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
		}
		gameObject.transform.SetAsLastSibling();
		Image component = gameObject.GetComponent<Image>();
		component.color = new Color(0.04f, 0.05f, 0.08f, 0.97f);
		component.raycastTarget = true;
		_panelCav = gameObject.GetComponent<CanvasGroup>();
		_panelCav.alpha = 0f;
		_panelCav.blocksRaycasts = false;
		_panelCav.interactable = false;
		_panel = gameObject;
		Text text = MakeText(gameObject.transform, "Title", builtinResource, 28, TextAnchor.MiddleCenter);
		SetRect(text.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -34f), new Vector2(-24f, 48f));
		text.text = "<b>铁匠工艺台 · POE 工艺</b>\u3000<color=#E6C96A>每项 1 金币</color>";
		_moneyText = MakeText(gameObject.transform, "Money", builtinResource, 19, TextAnchor.UpperRight);
		SetRect(_moneyText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -68f), new Vector2(-30f, 30f));
		_moneyText.text = "金币：0";
		_targetText = MakeText(gameObject.transform, "Target", builtinResource, 19, TextAnchor.UpperLeft);
		SetRect(_targetText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -112f), new Vector2(-300f, 60f));
		_targetText.text = "目标：（<color=#E6C96A>点击</color>背包中的装备选择）";
		GameObject gameObject2 = new GameObject("CraftScroll", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		SetRect((RectTransform)gameObject2.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 26f), new Vector2(-24f, -184f));
		Image component2 = gameObject2.GetComponent<Image>();
		component2.color = new Color(0f, 0f, 0f, 0.3f);
		component2.raycastTarget = true;
		ScrollRect component3 = gameObject2.GetComponent<ScrollRect>();
		GameObject gameObject3 = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
		gameObject3.transform.SetParent(gameObject2.transform, worldPositionStays: false);
		RectTransform rectTransform3 = (RectTransform)gameObject3.transform;
		SetRect(rectTransform3, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
		GameObject gameObject4 = new GameObject("Content", typeof(RectTransform));
		gameObject4.transform.SetParent(gameObject3.transform, worldPositionStays: false);
		RectTransform rectTransform4 = (RectTransform)gameObject4.transform;
		rectTransform4.anchorMin = new Vector2(0f, 1f);
		rectTransform4.anchorMax = new Vector2(1f, 1f);
		rectTransform4.pivot = new Vector2(0.5f, 1f);
		rectTransform4.anchoredPosition = Vector2.zero;
		float y = (float)RowDefs.Length * 42f + 4f;
		rectTransform4.sizeDelta = new Vector2(0f, y);
		component3.viewport = rectTransform3;
		component3.content = rectTransform4;
		component3.vertical = true;
		component3.horizontal = false;
		component3.movementType = ScrollRect.MovementType.Clamped;
		component3.scrollSensitivity = 18f;
		for (int i = 0; i < RowDefs.Length; i++)
		{
			RowDef rowDef = RowDefs[i];
			GameObject gameObject5 = new GameObject("Row_" + i, typeof(RectTransform), typeof(Image));
			gameObject5.transform.SetParent(gameObject4.transform, worldPositionStays: false);
			RectTransform obj = (RectTransform)gameObject5.transform;
			obj.anchorMin = new Vector2(0f, 1f);
			obj.anchorMax = new Vector2(1f, 1f);
			obj.pivot = new Vector2(0.5f, 1f);
			obj.anchoredPosition = new Vector2(0f, -4f - (float)i * 42f);
			obj.sizeDelta = new Vector2(-10f, 38f);
			Image component4 = gameObject5.GetComponent<Image>();
			component4.raycastTarget = true;
			Text text2 = MakeText(gameObject5.transform, "Label", builtinResource, 19, TextAnchor.MiddleLeft);
			SetRect(text2.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
			text2.rectTransform.offsetMin = new Vector2(12f, 2f);
			text2.rectTransform.offsetMax = new Vector2(-12f, -2f);
			text2.horizontalOverflow = HorizontalWrapMode.Overflow;
			Row row3 = new Row
			{
				Op = rowDef.Op,
				IsToggle = rowDef.IsToggle,
				IsDivider = rowDef.IsDivider,
				LockId = rowDef.LockId,
				Bg = component4,
				Label = text2
			};
			if (rowDef.IsDivider)
			{
				component4.color = new Color(0f, 0f, 0f, 0f);
				text2.alignment = TextAnchor.MiddleCenter;
				text2.color = DividerColor;
				text2.fontSize = 17;
				text2.text = rowDef.Name;
			}
			else
			{
				component4.color = RowColor;
				Button button = gameObject5.AddComponent<Button>();
				button.targetGraphic = component4;
				button.transition = Selectable.Transition.ColorTint;
				ColorBlock colors = button.colors;
				colors.highlightedColor = RowHoverColor;
				colors.pressedColor = new Color(0.13f, 0.15f, 0.2f, 1f);
				colors.disabledColor = RowDisabledColor;
				colors.fadeDuration = 0.05f;
				button.colors = colors;
				text2.text = rowDef.Name;
				Row row2 = row3;
				button.onClick.AddListener(delegate
				{
					OnRowClicked(row2);
				});
			}
			_rows.Add(row3);
		}
		Text text3 = MakeText(gameObject.transform, "Legend", builtinResource, 15, TextAnchor.LowerLeft);
		SetRect(text3.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 42f), new Vector2(-230f, 50f));
		text3.color = new Color(0.62f, 0.62f, 0.62f, 1f);
		text3.text = "前缀=输出词条（主属性/持续/技能/武器元素）\u3000后缀=功能词条（同伴/抗性/特效）\n词缀上限：魔法4 稀有6 精致7 史诗8 传说9 神话10\u3000重铸石会清除全部工艺限制";
		GameObject obj2 = new GameObject("CraftCloseBtn", typeof(RectTransform), typeof(Image), typeof(Button));
		obj2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		SetRect((RectTransform)obj2.transform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-88f, 26f), new Vector2(144f, 40f));
		Image component5 = obj2.GetComponent<Image>();
		component5.color = new Color(0.34f, 0.27f, 0.16f, 1f);
		Button component6 = obj2.GetComponent<Button>();
		component6.targetGraphic = component5;
		component6.onClick.AddListener(CloseBench);
		Text text4 = MakeText(obj2.transform, "Text", builtinResource, 19, TextAnchor.MiddleCenter);
		SetRect(text4.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
		text4.text = "返回锻造";
		RefreshRows(force: true);
	}

	private static Text MakeText(Transform parent, string name, Font font, int size, TextAnchor anchor)
	{
		GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
		obj.transform.SetParent(parent, worldPositionStays: false);
		Text component = obj.GetComponent<Text>();
		component.font = font;
		component.fontSize = size;
		component.alignment = anchor;
		component.color = new Color(0.92f, 0.92f, 0.92f, 1f);
		component.supportRichText = true;
		component.horizontalOverflow = HorizontalWrapMode.Wrap;
		component.verticalOverflow = VerticalWrapMode.Overflow;
		component.raycastTarget = false;
		return component;
	}

	private static void SetRect(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
	{
		rt.anchorMin = anchorMin;
		rt.anchorMax = anchorMax;
		rt.pivot = new Vector2(0.5f, 0.5f);
		rt.anchoredPosition = anchoredPos;
		rt.sizeDelta = sizeDelta;
	}
}
