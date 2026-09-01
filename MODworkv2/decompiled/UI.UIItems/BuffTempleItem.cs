using Core;
using FinkFramework.Runtime.Singleton;
using UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UIItems;

public class BuffTempleItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Image iconImg;

	public Image cover;

	private CanvasGroup tip;

	private Text timeText;

	private Text buffText;

	private bool isHovering;

	public float remainTime;

	public float maxTime;

	public int Type;

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

	public void RefreshBuffText()
	{
		switch (Type)
		{
		case 0:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple0"), 20, LOC.MM.GetMain("Experience Gain"));
			break;
		case 1:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple1"), 20, LOC.MM.GetMain("HealthMax"));
			break;
		case 2:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple2"), 5, LOC.MM.GetMain("mana recovery"));
			break;
		case 3:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple3"), 15, LOC.MM.GetMain("damage"));
			break;
		case 4:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple4"), 10, LOC.MM.GetMain("BJrate"));
			break;
		case 5:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple5"), 10, LOC.MM.GetMain("AttackSpeed"));
			break;
		case 6:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple6"), 10, LOC.MM.GetMain("AllChuan"));
			break;
		case 7:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple7"), 10, LOC.MM.GetMain("AllAnti"));
			break;
		case 8:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple8"), 10, LOC.MM.GetMain("CD"));
			break;
		case 9:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple9"), 20, LOC.MM.GetMain("DropRate"));
			break;
		case 10:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple10"), 20, LOC.MM.GetMain("Comp HealthMax"));
			break;
		case 11:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple11"), 20, LOC.MM.GetMain("fire damage"));
			break;
		case 12:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple12"), 20, LOC.MM.GetMain("frozen damage"));
			break;
		case 13:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple13"), 20, LOC.MM.GetMain("thunder damage"));
			break;
		case 14:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple14"), 20, LOC.MM.GetMain("poison damage"));
			break;
		case 15:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple15"), 20, LOC.MM.GetMain("physics damage"));
			break;
		case 16:
			buffText.text = string.Format("{0} +{1}% {2}", LOC.MM.GetMain("Temple16"), 20, LOC.MM.GetMain("shadow damage"));
			break;
		}
	}

	public void RefreshTimeText()
	{
		timeText.text = $"{remainTime} S";
	}

	private static Color GetColor(int type)
	{
		return type switch
		{
			0 => new Color32(byte.MaxValue, byte.MaxValue, 160, byte.MaxValue), 
			1 => new Color32(byte.MaxValue, 90, 120, byte.MaxValue), 
			2 => new Color32(91, 137, byte.MaxValue, byte.MaxValue), 
			3 => Color.white, 
			4 => new Color32(203, 184, 139, byte.MaxValue), 
			5 => new Color32(248, 134, byte.MaxValue, byte.MaxValue), 
			6 => new Color32(140, byte.MaxValue, 134, byte.MaxValue), 
			7 => new Color32(134, 165, byte.MaxValue, byte.MaxValue), 
			8 => new Color32(190, 134, byte.MaxValue, byte.MaxValue), 
			9 => new Color32(134, 148, byte.MaxValue, byte.MaxValue), 
			10 => new Color32(134, byte.MaxValue, 199, byte.MaxValue), 
			11 => new Color32(134, 248, byte.MaxValue, byte.MaxValue), 
			12 => new Color32(byte.MaxValue, byte.MaxValue, 134, byte.MaxValue), 
			13 => Color.red, 
			14 => new Color32(80, 230, byte.MaxValue, byte.MaxValue), 
			15 => Color.yellow, 
			16 => Color.green, 
			17 => new Color32(150, 0, byte.MaxValue, byte.MaxValue), 
			18 => new Color32(byte.MaxValue, 151, 222, byte.MaxValue), 
			19 => new Color32(byte.MaxValue, 150, 0, byte.MaxValue), 
			20 => Color.white, 
			_ => default(Color), 
		};
	}

	public void Cover()
	{
		switch (Type)
		{
		case 0:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("Experience Gain")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(0));
			break;
		case 1:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("HealthMax")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(1));
			break;
		case 2:
			GameManager.ShowTip(string.Format("+{0}% {1}", 5, LOC.MM.GetMain("mana recovery")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(2));
			break;
		case 3:
			GameManager.ShowTip(string.Format("+{0}% {1}", 15, LOC.MM.GetMain("damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(3));
			break;
		case 4:
			GameManager.ShowTip(string.Format("+{0}% {1}", 10, LOC.MM.GetMain("BJrate")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(5));
			break;
		case 5:
			GameManager.ShowTip(string.Format("+{0}% {1}", 10, LOC.MM.GetMain("AttackSpeed")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(6));
			break;
		case 6:
			GameManager.ShowTip(string.Format("+{0}% {1}", 10, LOC.MM.GetMain("AllChuan")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(7));
			break;
		case 7:
			GameManager.ShowTip(string.Format("+{0}% {1}", 10, LOC.MM.GetMain("AllAnti")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(8));
			break;
		case 8:
			GameManager.ShowTip(string.Format("+{0}% {1}", 10, LOC.MM.GetMain("CD")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(11));
			break;
		case 9:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("DropRate")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(12));
			break;
		case 10:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("Comp damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(10));
			break;
		case 11:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("fire damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(13));
			break;
		case 12:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("frozen damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(14));
			break;
		case 13:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("thunder damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(15));
			break;
		case 14:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("poison damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(16));
			break;
		case 15:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("physics damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(17));
			break;
		case 16:
			GameManager.ShowTip(string.Format("+{0}% {1}", 20, LOC.MM.GetMain("shadow damage")), TipType.Normal, -1f, useCustomTextColor: true, GetColor(18));
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
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
			switch (Type)
			{
			case 0:
				instance.Xp_Bei_Tmp += 20f;
				break;
			case 1:
				instance.Health_Bei_Tmp += 20f;
				break;
			case 2:
				instance.Mana_Percent_Tmp += 5f;
				break;
			case 3:
				instance.Damage_Bei_Tmp += 15f;
				break;
			case 4:
				instance.BJrate_Tmp += 10f;
				break;
			case 5:
				instance.ATSpeed_Tmp += 10f;
				break;
			case 6:
				instance.FireChuan_Tmp += 10f;
				instance.FrozenChuan_Tmp += 10f;
				instance.ThunderChuan_Tmp += 10f;
				instance.PoisonChuan_Tmp += 10f;
				instance.PhysicsChuan_Tmp += 10f;
				instance.ShadowChuan_Tmp += 10f;
				break;
			case 7:
				instance.FireAnti_Tmp += 10f;
				instance.FrozenAnti_Tmp += 10f;
				instance.ThunderAnti_Tmp += 10f;
				instance.PoisonAnti_Tmp += 10f;
				instance.PhysicsAnti_Tmp += 10f;
				instance.ShadowAnti_Tmp += 10f;
				break;
			case 8:
				instance.CoolDown_Tmp += 10f;
				break;
			case 9:
				instance.ItemDrop_Rate_buff_Tmp += 20f;
				break;
			case 10:
				instance.C_Damage_Tmp += 20f;
				break;
			case 11:
				instance.FireDamage_Bei_Tmp += 20f;
				break;
			case 12:
				instance.FrozenDamage_Bei_Tmp += 20f;
				break;
			case 13:
				instance.ThunderDamage_Bei_Tmp += 20f;
				break;
			case 14:
				instance.PoisonDamage_Bei_Tmp += 20f;
				break;
			case 15:
				instance.PhysicsDamage_Bei_Tmp += 20f;
				break;
			case 16:
				instance.ShadowDamage_Bei_Tmp += 20f;
				break;
			}
			InitBuffUI(icon);
		}
	}

	public void DelBuff()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
			switch (Type)
			{
			case 0:
				instance.Xp_Bei_Tmp -= 20f;
				break;
			case 1:
				instance.Health_Bei_Tmp -= 20f;
				break;
			case 2:
				instance.Mana_Percent_Tmp -= 5f;
				break;
			case 3:
				instance.Damage_Bei_Tmp -= 15f;
				break;
			case 4:
				instance.BJrate_Tmp -= 10f;
				break;
			case 5:
				instance.ATSpeed_Tmp -= 10f;
				break;
			case 6:
				instance.FireChuan_Tmp -= 10f;
				instance.FrozenChuan_Tmp -= 10f;
				instance.ThunderChuan_Tmp -= 10f;
				instance.PoisonChuan_Tmp -= 10f;
				instance.PhysicsChuan_Tmp -= 10f;
				instance.ShadowChuan_Tmp -= 10f;
				break;
			case 7:
				instance.FireAnti_Tmp -= 10f;
				instance.FrozenAnti_Tmp -= 10f;
				instance.ThunderAnti_Tmp -= 10f;
				instance.PoisonAnti_Tmp -= 10f;
				instance.PhysicsAnti_Tmp -= 10f;
				instance.ShadowAnti_Tmp -= 10f;
				break;
			case 8:
				instance.CoolDown_Tmp -= 10f;
				break;
			case 9:
				instance.ItemDrop_Rate_buff_Tmp -= 20f;
				break;
			case 10:
				instance.C_Damage_Tmp -= 20f;
				break;
			case 11:
				instance.FireDamage_Bei_Tmp -= 20f;
				break;
			case 12:
				instance.FrozenDamage_Bei_Tmp -= 20f;
				break;
			case 13:
				instance.ThunderDamage_Bei_Tmp -= 20f;
				break;
			case 14:
				instance.PoisonDamage_Bei_Tmp -= 20f;
				break;
			case 15:
				instance.PhysicsDamage_Bei_Tmp -= 20f;
				break;
			case 16:
				instance.ShadowDamage_Bei_Tmp -= 20f;
				break;
			}
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
