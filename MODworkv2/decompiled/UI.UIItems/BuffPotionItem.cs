using System;
using Core;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UIItems;

public class BuffPotionItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Image iconImg;

	public Image cover;

	private GameObject tipPrefab;

	private CanvasGroup tip;

	private Text timeText;

	private Text buffText;

	private bool isHovering;

	public string IndexName;

	public float remainTime;

	public float maxTime;

	public string UseType;

	public int Number;

	public DamageType damageType;

	private void Awake()
	{
		if (!tip)
		{
			if (SingletonMonoScope<GameUIManager>.HasInstance)
			{
				tip = SingletonMonoScope<GameUIManager>.Instance.buffTip.GetComponent<CanvasGroup>();
			}
			tip.alpha = 0f;
		}
		if (!timeText)
		{
			timeText = tip.transform.Find("TimeText").GetComponent<Text>();
		}
		if (!buffText)
		{
			buffText = tip.transform.Find("BuffText").GetComponent<Text>();
		}
	}

	public void RefreshTime()
	{
		cover.fillAmount = ((maxTime > 0f) ? (remainTime / maxTime) : 0f);
		if (isHovering && (bool)tip && tip.alpha > 0f)
		{
			RefreshTimeText();
		}
	}

	private void InitBuffUI(Sprite icon)
	{
		if (iconImg.sprite != icon)
		{
			iconImg.sprite = icon;
		}
		cover.fillAmount = ((maxTime > 0f) ? (remainTime / maxTime) : 0f);
		RefreshBuffText();
		RefreshTimeText();
	}

	private void RefreshBuffText()
	{
		string useType = UseType;
		if (useType == null)
		{
			return;
		}
		switch (useType.Length)
		{
		case 7:
			switch (useType[0])
			{
			case 'E':
				if (useType == "EL_Anti")
				{
					buffText.text = $"{LOC.MM.GetItem(IndexName)}+{Number}%{LOC.MM.GetMain(SWS.El_Anti(damageType))}";
				}
				break;
			case 'x':
				if (useType == "xingyun")
				{
					buffText.text = string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("DropRate"));
				}
				break;
			}
			break;
		case 9:
			if (useType == "EL_Damage")
			{
				buffText.text = $"{LOC.MM.GetItem(IndexName)}+{Number}%{LOC.MM.GetMain(SWS.El_DMG(damageType))}";
			}
			break;
		case 6:
			if (useType == "xueshi")
			{
				buffText.text = string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("Experience Gain"));
			}
			break;
		case 8:
			if (useType == "zhaohuan")
			{
				buffText.text = string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("Comp damage"));
			}
			break;
		case 14:
			if (useType == "poe_flask_gale")
			{
				buffText.text = string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("MoveSpeed"));
			}
			break;
		case 17:
			if (useType == "poe_flask_insight")
			{
				buffText.text = string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("BJrate"));
			}
			break;
		}
	}

	private void RefreshTimeText()
	{
		timeText.text = $"{remainTime} S";
	}

	public void Cover()
	{
		string useType = UseType;
		if (useType == null)
		{
			return;
		}
		switch (useType.Length)
		{
		case 7:
			switch (useType[0])
			{
			case 'E':
				if (useType == "EL_Anti")
				{
					GameManager.ShowTip($"{LOC.MM.GetItem(IndexName)}+{Number}%{LOC.MM.GetMain(SWS.El_Anti(damageType))}");
				}
				break;
			case 'x':
				if (useType == "xingyun")
				{
					GameManager.ShowTip(string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("DropRate")));
				}
				break;
			}
			break;
		case 9:
			if (useType == "EL_Damage")
			{
				GameManager.ShowTip($"{LOC.MM.GetItem(IndexName)}+{Number}%{LOC.MM.GetMain(SWS.El_DMG(damageType))}");
			}
			break;
		case 6:
			if (useType == "xueshi")
			{
				GameManager.ShowTip(string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("Experience Gain")));
			}
			break;
		case 8:
			if (useType == "zhaohuan")
			{
				GameManager.ShowTip(string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("Comp damage")));
			}
			break;
		case 14:
			if (useType == "poe_flask_gale")
			{
				GameManager.ShowTip(string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("MoveSpeed")));
			}
			break;
		case 17:
			if (useType == "poe_flask_insight")
			{
				GameManager.ShowTip(string.Format("{0}+{1}%{2}", LOC.MM.GetItem(IndexName), Number, LOC.MM.GetMain("BJrate")));
			}
			break;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return;
		}
		isHovering = true;
		RefreshBuffText();
		RefreshTimeText();
		tip.alpha = 1f;
		RectTransform rectTransform = base.transform as RectTransform;
		RectTransform rectTransform2 = tip.transform as RectTransform;
		RectTransform rectTransform3 = rectTransform2.parent as RectTransform;
		if ((bool)rectTransform && (bool)rectTransform2 && (bool)rectTransform3)
		{
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform3, screenPoint, null, out var localPoint))
			{
				rectTransform2.anchoredPosition = localPoint + new Vector2(0f, 50f);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovering = false;
		tip.alpha = 0f;
	}

	public void Init(Sprite icon)
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return;
		}
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		switch (UseType)
		{
		case "EL_Damage":
			switch (damageType)
			{
			case DamageType.fire:
				instance.FireDamage_Bei_Tmp += Number;
				break;
			case DamageType.frozen:
				instance.FrozenDamage_Bei_Tmp += Number;
				break;
			case DamageType.thunder:
				instance.ThunderDamage_Bei_Tmp += Number;
				break;
			case DamageType.poison:
				instance.PoisonDamage_Bei_Tmp += Number;
				break;
			case DamageType.physics:
				instance.PhysicsDamage_Bei_Tmp += Number;
				break;
			case DamageType.shadow:
				instance.ShadowDamage_Bei_Tmp += Number;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case "EL_Anti":
			switch (damageType)
			{
			case DamageType.fire:
				instance.FireAnti_Tmp += Number;
				break;
			case DamageType.frozen:
				instance.FrozenAnti_Tmp += Number;
				break;
			case DamageType.thunder:
				instance.ThunderAnti_Tmp += Number;
				break;
			case DamageType.poison:
				instance.PoisonAnti_Tmp += Number;
				break;
			case DamageType.physics:
				instance.PhysicsAnti_Tmp += Number;
				break;
			case DamageType.shadow:
				instance.ShadowAnti_Tmp += Number;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case "xueshi":
			instance.Xp_Bei_Tmp += Number;
			break;
		case "xingyun":
			instance.ItemDrop_Rate_buff_Tmp += Number;
			break;
		case "zhaohuan":
			instance.C_Damage_Tmp += Number;
			break;
		case "poe_flask_gale":
			instance.MVSpeed_Tmp += Number;
			break;
		case "poe_flask_insight":
			instance.BJrate_Tmp += Number;
			break;
		}
		InitBuffUI(icon);
	}

	public void DelBuff()
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return;
		}
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		string useType = UseType;
		if (useType == null)
		{
			return;
		}
		switch (useType.Length)
		{
		case 7:
			switch (useType[0])
			{
			case 'E':
				if (useType == "EL_Anti")
				{
					switch (damageType)
					{
					case DamageType.fire:
						instance.FireAnti_Tmp -= Number;
						break;
					case DamageType.frozen:
						instance.FrozenAnti_Tmp -= Number;
						break;
					case DamageType.thunder:
						instance.ThunderAnti_Tmp -= Number;
						break;
					case DamageType.poison:
						instance.PoisonAnti_Tmp -= Number;
						break;
					case DamageType.physics:
						instance.PhysicsAnti_Tmp -= Number;
						break;
					case DamageType.shadow:
						instance.ShadowAnti_Tmp -= Number;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				break;
			case 'x':
				if (useType == "xingyun")
				{
					instance.ItemDrop_Rate_buff_Tmp -= Number;
				}
				break;
			}
			break;
		case 9:
			if (useType == "EL_Damage")
			{
				switch (damageType)
				{
				case DamageType.fire:
					instance.FireDamage_Bei_Tmp -= Number;
					break;
				case DamageType.frozen:
					instance.FrozenDamage_Bei_Tmp -= Number;
					break;
				case DamageType.thunder:
					instance.ThunderDamage_Bei_Tmp -= Number;
					break;
				case DamageType.poison:
					instance.PoisonDamage_Bei_Tmp -= Number;
					break;
				case DamageType.physics:
					instance.PhysicsDamage_Bei_Tmp -= Number;
					break;
				case DamageType.shadow:
					instance.ShadowDamage_Bei_Tmp -= Number;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			break;
		case 6:
			if (useType == "xueshi")
			{
				instance.Xp_Bei_Tmp -= Number;
			}
			break;
		case 8:
			if (useType == "zhaohuan")
			{
				instance.C_Damage_Tmp -= Number;
			}
			break;
		case 14:
			if (useType == "poe_flask_gale")
			{
				instance.MVSpeed_Tmp -= Number;
			}
			break;
		case 17:
			if (useType == "poe_flask_insight")
			{
				instance.BJrate_Tmp -= Number;
			}
			break;
		}
	}

	private void OnDisable()
	{
		isHovering = false;
		if ((bool)tip)
		{
			tip.alpha = 0f;
		}
	}
}
