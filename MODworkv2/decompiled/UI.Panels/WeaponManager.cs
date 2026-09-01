using System;
using System.Collections.Generic;
using Container.Util;
using Core;
using Core.Settings;
using Entity.InteractableObjects.Item;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Interact;
using PoedbMod;
using Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class WeaponManager : ScopedSingletonMono<WeaponManager>
{
	private enum WeaponForgeMode
	{
		None,
		Elm,
		Spc,
		Enh
	}

	private class WeaponForgeContext
	{
		public SlotData Slot;

		public WeaponClass RuntimeWeapon;

		public long Price;

		public bool IsValid;

		public void Clear()
		{
			Slot = null;
			RuntimeWeapon = null;
			Price = 0L;
			IsValid = false;
		}
	}

	private enum ElmRebuildMode
	{
		Keep,
		Average,
		SmallBoost,
		BigBoost
	}

	private struct ElementValueData
	{
		public readonly int Type;

		public float Value;

		public ElementValueData(int type, float value)
		{
			Type = type;
			Value = value;
		}
	}

	private readonly WeaponForgeContext forgeContext = new WeaponForgeContext();

	[Header("音效事件")]
	[SerializeField]
	private string audio_event;

	[HideInInspector]
	public bool Opened;

	[Header("引用变量")]
	[SerializeField]
	private CanvasGroup cav;

	[SerializeField]
	private Button closeBtn;

	[SerializeField]
	private Button elmBtn;

	[SerializeField]
	private Button spcBtn;

	[SerializeField]
	private Button enhBtn;

	[SerializeField]
	private Text ElmWeaponText;

	[SerializeField]
	private Text ElmPriceText;

	[SerializeField]
	private Text SpcWeaponText;

	[SerializeField]
	private Text SpcPriceText;

	[SerializeField]
	private Text EnhWeaponText;

	[SerializeField]
	private Text EnhPriceText;

	private void ClearForgeContext()
	{
		forgeContext.Clear();
	}

	private void RefreshForgeContext(WeaponForgeMode mode)
	{
		forgeContext.Clear();
		if (!SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return;
		}
		SlotData mainSlot = ContainerGridUtil.GetMainSlot(SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT, SingletonMonoScope<InventoryManager>.Instance.Page);
		if (!IsValidForgeWeaponSlot(mainSlot))
		{
			return;
		}
		WeaponClass weapon = mainSlot.weapon;
		if (weapon != null)
		{
			forgeContext.Slot = mainSlot;
			forgeContext.RuntimeWeapon = weapon;
			switch (mode)
			{
			case WeaponForgeMode.Elm:
			case WeaponForgeMode.Spc:
				forgeContext.Price = GetRebuildPrice(weapon);
				break;
			case WeaponForgeMode.Enh:
				forgeContext.Price = GetEnhancePrice(weapon);
				break;
			default:
				throw new ArgumentOutOfRangeException("mode", mode, null);
			case WeaponForgeMode.None:
				break;
			}
			forgeContext.IsValid = true;
		}
	}

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		if (!closeBtn)
		{
			closeBtn = base.transform.Find("Close").GetComponent<Button>();
		}
		if (!cav)
		{
			cav = GetComponent<CanvasGroup>();
		}
		if (!elmBtn)
		{
			elmBtn = base.transform.Find("MainGroup/ELM/ElmGroup/ElmBtn").GetComponent<Button>();
		}
		if (!spcBtn)
		{
			spcBtn = base.transform.Find("MainGroup/SPC/SpcGroup/SpcBtn").GetComponent<Button>();
		}
		if (!enhBtn)
		{
			enhBtn = base.transform.Find("MainGroup/ENH/EnhGroup/EnhBtn").GetComponent<Button>();
		}
		if (!SpcWeaponText)
		{
			SpcWeaponText = base.transform.Find("MainGroup/SPC/SpcGroup/SpcTip/SpcWeaponText").GetComponent<Text>();
		}
		if (!SpcPriceText)
		{
			SpcPriceText = base.transform.Find("MainGroup/SPC/SpcGroup/SpcTip/SpcPriceText").GetComponent<Text>();
		}
		if (!ElmWeaponText)
		{
			ElmWeaponText = base.transform.Find("MainGroup/ELM/ElmGroup/ElmTip/ElmWeaponText").GetComponent<Text>();
		}
		if (!ElmPriceText)
		{
			ElmPriceText = base.transform.Find("MainGroup/ELM/ElmGroup/ElmTip/ElmPriceText").GetComponent<Text>();
		}
		if (!EnhWeaponText)
		{
			EnhWeaponText = base.transform.Find("MainGroup/ENH/EnhGroup/EnhTip/EnhWeaponText").GetComponent<Text>();
		}
		if (!EnhPriceText)
		{
			EnhPriceText = base.transform.Find("MainGroup/ENH/EnhGroup/EnhTip/EnhPriceText").GetComponent<Text>();
		}
		elmBtn.onClick.AddListener(OnClickElm);
		spcBtn.onClick.AddListener(OnClickSpc);
		enhBtn.onClick.AddListener(OnClickEnh);
		closeBtn.onClick.AddListener(OnClickClose);
		ClearElmTip();
		ClearSpcTip();
		ClearEnhTip();
		CraftBenchUI.Install(this);
	}

	public Button GetCloseBtn()
	{
		return closeBtn;
	}

	public string GetForgeAudioEvent()
	{
		return audio_event;
	}

	private void Update()
	{
		if (Opened && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			switch (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState)
			{
			case GlobalUiModalState.WeaponElm:
				RefreshElmTip();
				HandleElmInput();
				break;
			case GlobalUiModalState.WeaponSpc:
				RefreshSpcTip();
				HandleSpcInput();
				break;
			case GlobalUiModalState.WeaponEnh:
				RefreshEnhTip();
				HandleEnhInput();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case GlobalUiModalState.None:
			case GlobalUiModalState.BaoshiSplit:
				break;
			}
		}
	}

	public void OnClickElm()
	{
		EnterElm();
	}

	public void OnClickSpc()
	{
		EnterSpc();
	}

	public void OnClickEnh()
	{
		EnterEnh();
	}

	public void OnClickClose()
	{
		CloseWeapon();
	}

	public void OpenWeapon()
	{
		if (SettingsLoader.Instance.WeaponToggle && SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.Opened_weapon = true;
			Opened = true;
			cav.blocksRaycasts = true;
			cav.alpha = 1f;
			SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 1f;
			SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = true;
			SingletonMonoScope<GameUIManager>.Instance.Opened_IV = true;
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponElm)
			{
				ExitElm();
			}
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponSpc)
			{
				ExitSpc();
			}
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponEnh)
			{
				ExitEnh();
			}
		}
	}

	public void CloseWeapon()
	{
		if (SettingsLoader.Instance.WeaponToggle && SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.Opened_weapon = false;
			Opened = false;
			cav.blocksRaycasts = false;
			cav.alpha = 0f;
			SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 0f;
			SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = false;
			SingletonMonoScope<GameUIManager>.Instance.Opened_IV = false;
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponElm)
			{
				ExitElm();
			}
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponSpc)
			{
				ExitSpc();
			}
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponEnh)
			{
				ExitEnh();
			}
		}
	}

	public void EnterElm()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return;
		}
		if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponSpc)
		{
			ExitSpc();
		}
		if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponEnh)
		{
			ExitEnh();
		}
		if ((bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ)
		{
			GameManager.ShowTipLocalStartKey("please_take_off_hand_item", TipType.Fail);
			return;
		}
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			InteractionManager.AllInteractToggle = false;
		}
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: false);
		}
		SingletonMonoScope<GameUIManager>.Instance.EnterWeaponElmMode();
		RefreshElmTip();
	}

	public void ExitElm()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			if (SingletonMonoScope<InteractionManager>.HasInstance)
			{
				InteractionManager.AllInteractToggle = true;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: true);
			}
			ClearElmTip();
			ClearForgeContext();
			SingletonMonoScope<GameUIManager>.Instance.ExitWeaponElmMode();
		}
	}

	public void RefreshElmTip()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponElm)
		{
			RefreshForgeContext(WeaponForgeMode.Elm);
			if (!forgeContext.IsValid)
			{
				ClearElmTip();
				return;
			}
			WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
			int remainRebuildCount = GetRemainRebuildCount(runtimeWeapon);
			ElmWeaponText.text = LOC.MM.GetMain("selected_weapon") + runtimeWeapon.GetTitle() + "     <color=#E6C96A>" + LOC.MM.GetMain("rebuild_weapon_time") + remainRebuildCount + "</color>";
			ElmPriceText.text = GetPriceRichText(forgeContext.Price);
		}
	}

	private void ClearElmTip()
	{
		if ((bool)ElmWeaponText)
		{
			ElmWeaponText.text = "<color=#FFFFFF>" + LOC.MM.GetMain("selected_weapon") + LOC.MM.GetMain("weapon_null") + "     " + LOC.MM.GetMain("rebuild_weapon_time") + 0 + "</color>";
		}
		if ((bool)ElmPriceText)
		{
			ElmPriceText.text = "<color=#FFFFFF>" + LOC.MM.GetLevelFormat("mijing_need_price", 0) + "</color>";
		}
	}

	private void HandleElmInput()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponElm && IsSubmitDown())
		{
			RefreshForgeContext(WeaponForgeMode.Elm);
			if (forgeContext.IsValid && CanTryForgeElm())
			{
				TryRandomElm();
			}
		}
	}

	private bool CanTryForgeElm()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeWeapon == null)
		{
			return false;
		}
		if ((bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ)
		{
			GameManager.ShowTipLocalStartKey("please_take_off_hand_item", TipType.Fail);
			return false;
		}
		if (!SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return false;
		}
		if (GetRemainRebuildCount(forgeContext.RuntimeWeapon) <= 0)
		{
			GameManager.ShowTip(LOC.MM.GetMain("rebuild_max"), TipType.Fail);
			return false;
		}
		if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < forgeContext.Price)
		{
			GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			return false;
		}
		return true;
	}

	private void TryRandomElm()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeWeapon == null || !SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return;
		}
		WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
		if (GetRemainRebuildCount(runtimeWeapon) <= 0)
		{
			GameManager.ShowTip(LOC.MM.GetMain("rebuild_max"), TipType.Fail);
		}
		else
		{
			if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < forgeContext.Price)
			{
				return;
			}
			List<ElementValueData> list = CollectActiveElements(runtimeWeapon);
			if (list.Count <= 0)
			{
				GameManager.ShowTip(LOC.MM.GetMain("weapon_elm_null"), TipType.Fail);
				return;
			}
			int count = list.Count;
			List<int> randomDistinctElementTypes = GetRandomDistinctElementTypes(list);
			List<ElementValueData> list2 = CalculateElmRebuildValues(list, randomDistinctElementTypes);
			if (list2 != null && list2.Count == count)
			{
				ApplyElementValuesToWeapon(runtimeWeapon, list2);
				SingletonMonoScope<InventoryManager>.Instance.RemoveMoney(forgeContext.Price);
				runtimeWeapon.Reb_CountMax++;
				ContainerGridUtil.BindWeaponToRegion(forgeContext.Slot, SingletonMonoScope<InventoryManager>.Instance.Page);
				if (ItemCloneUtil.CloneWeapon(runtimeWeapon) != null)
				{
					RuntimeManager.PlayOneShot(audio_event, base.transform.position);
					SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(forgeContext.Slot.weapon, forgeContext.Slot, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
					RefreshElmTip();
				}
			}
		}
	}

	private static List<ElementValueData> CollectActiveElements(WeaponClass weapon)
	{
		List<ElementValueData> list = new List<ElementValueData>();
		if (weapon == null)
		{
			return list;
		}
		if (weapon.Fire > 0f)
		{
			list.Add(new ElementValueData(0, weapon.Fire));
		}
		if (weapon.Frozen > 0f)
		{
			list.Add(new ElementValueData(1, weapon.Frozen));
		}
		if (weapon.Thunder > 0f)
		{
			list.Add(new ElementValueData(2, weapon.Thunder));
		}
		if (weapon.Poison > 0f)
		{
			list.Add(new ElementValueData(3, weapon.Poison));
		}
		if (weapon.Physics > 0f)
		{
			list.Add(new ElementValueData(4, weapon.Physics));
		}
		if (weapon.Shadow > 0f)
		{
			list.Add(new ElementValueData(5, weapon.Shadow));
		}
		return list;
	}

	private static List<int> GetRandomDistinctElementTypes(List<ElementValueData> oldElements)
	{
		List<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };
		List<int> list2 = new List<int>();
		if (oldElements == null || oldElements.Count <= 0)
		{
			return list2;
		}
		int num = Mathf.Clamp(oldElements.Count, 0, list.Count);
		for (int i = 0; i < num; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		if (num >= 6)
		{
			return list2;
		}
		if (AreElementTypesExactlySame(oldElements, list2))
		{
			int index2 = UnityEngine.Random.Range(0, list2.Count);
			int num2 = list2[index2];
			int num3 = UnityEngine.Random.Range(0, 6);
			if (num3 == num2)
			{
				num3 = (num3 + UnityEngine.Random.Range(1, 6)) % 6;
			}
			while (list2.Contains(num3))
			{
				num3 = (num3 + UnityEngine.Random.Range(1, 6)) % 6;
			}
			list2[index2] = num3;
		}
		return list2;
	}

	private static bool AreElementTypesExactlySame(List<ElementValueData> oldElements, List<int> newTypes)
	{
		if (oldElements == null || newTypes == null)
		{
			return false;
		}
		if (oldElements.Count != newTypes.Count)
		{
			return false;
		}
		for (int i = 0; i < oldElements.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < newTypes.Count; j++)
			{
				if (oldElements[i].Type == newTypes[j])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	private static ElmRebuildMode GetRandomElmMode()
	{
		int num = UnityEngine.Random.Range(0, 100);
		if (num < 20)
		{
			return ElmRebuildMode.Average;
		}
		if (num < 40)
		{
			return ElmRebuildMode.SmallBoost;
		}
		if (num < 50)
		{
			return ElmRebuildMode.BigBoost;
		}
		return ElmRebuildMode.Keep;
	}

	private static List<ElementValueData> CalculateElmRebuildValues(List<ElementValueData> oldElements, List<int> targetElementTypes)
	{
		List<ElementValueData> list = new List<ElementValueData>();
		if (oldElements == null || targetElementTypes == null)
		{
			return list;
		}
		int count = oldElements.Count;
		if (count <= 0 || targetElementTypes.Count != count)
		{
			return list;
		}
		ElmRebuildMode randomElmMode = GetRandomElmMode();
		float num = 0f;
		for (int i = 0; i < oldElements.Count; i++)
		{
			num += oldElements[i].Value;
		}
		switch (randomElmMode)
		{
		case ElmRebuildMode.Keep:
		{
			List<float> list4 = new List<float>();
			for (int m = 0; m < oldElements.Count; m++)
			{
				list4.Add(oldElements[m].Value);
			}
			ShuffleFloatList(list4);
			for (int n = 0; n < count; n++)
			{
				list.Add(new ElementValueData(targetElementTypes[n], list4[n]));
			}
			break;
		}
		case ElmRebuildMode.Average:
		{
			float value = num / (float)count;
			for (int k = 0; k < count; k++)
			{
				list.Add(new ElementValueData(targetElementTypes[k], value));
			}
			break;
		}
		case ElmRebuildMode.SmallBoost:
		{
			List<float> list3 = CreateBoostDistribution(oldElements, num, 0.15f);
			for (int l = 0; l < count; l++)
			{
				list.Add(new ElementValueData(targetElementTypes[l], list3[l]));
			}
			break;
		}
		case ElmRebuildMode.BigBoost:
		{
			List<float> list2 = CreateBoostDistribution(oldElements, num, 0.3f);
			for (int j = 0; j < count; j++)
			{
				list.Add(new ElementValueData(targetElementTypes[j], list2[j]));
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		NormalizeElementTotal(list, num);
		ClampElementValues(list);
		return list;
	}

	private static List<float> CreateBoostDistribution(List<ElementValueData> oldElements, float totalValue, float boostPercent)
	{
		int count = oldElements.Count;
		List<float> list = new List<float>();
		if (count <= 0)
		{
			return list;
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(oldElements[i].Value);
		}
		int num = UnityEngine.Random.Range(0, count);
		float num2 = totalValue * boostPercent;
		list[num] += num2;
		if (count > 1)
		{
			float num3 = num2 / (float)(count - 1);
			for (int j = 0; j < count; j++)
			{
				if (j != num)
				{
					list[j] -= num3;
					if (list[j] < 0f)
					{
						list[j] = 0f;
					}
				}
			}
		}
		return list;
	}

	private static void NormalizeElementTotal(List<ElementValueData> elements, float targetTotal)
	{
		if (elements == null || elements.Count == 0)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < elements.Count; i++)
		{
			num += elements[i].Value;
		}
		if (Mathf.Approximately(num, targetTotal))
		{
			return;
		}
		float num2 = targetTotal - num;
		if (num2 > 0f)
		{
			int index = UnityEngine.Random.Range(0, elements.Count);
			ElementValueData value = elements[index];
			value.Value += num2;
			elements[index] = value;
			return;
		}
		float num3 = 0f - num2;
		for (int j = 0; j < elements.Count; j++)
		{
			if (num3 <= 0f)
			{
				break;
			}
			ElementValueData value2 = elements[j];
			if (!(value2.Value <= 0f))
			{
				float num4 = Mathf.Min(value2.Value, num3);
				value2.Value -= num4;
				num3 -= num4;
				elements[j] = value2;
			}
		}
	}

	private static void ClampElementValues(List<ElementValueData> elements)
	{
		if (elements == null)
		{
			return;
		}
		for (int i = 0; i < elements.Count; i++)
		{
			ElementValueData value = elements[i];
			if (value.Value < 0f)
			{
				value.Value = 0f;
			}
			elements[i] = value;
		}
	}

	private static void ShuffleFloatList(List<float> list)
	{
		if (list != null && list.Count > 1)
		{
			for (int num = list.Count - 1; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num + 1);
				int index = num;
				int index2 = num2;
				float num3 = list[num2];
				float num4 = list[num];
				float num6 = (list[index] = num3);
				num6 = (list[index2] = num4);
			}
		}
	}

	private static void ApplyElementValuesToWeapon(WeaponClass weapon, List<ElementValueData> elements)
	{
		if (weapon == null)
		{
			return;
		}
		weapon.Fire = 0f;
		weapon.Frozen = 0f;
		weapon.Thunder = 0f;
		weapon.Poison = 0f;
		weapon.Physics = 0f;
		weapon.Shadow = 0f;
		if (elements == null)
		{
			return;
		}
		for (int i = 0; i < elements.Count; i++)
		{
			ElementValueData elementValueData = elements[i];
			switch (elementValueData.Type)
			{
			case 0:
				weapon.Fire = elementValueData.Value;
				break;
			case 1:
				weapon.Frozen = elementValueData.Value;
				break;
			case 2:
				weapon.Thunder = elementValueData.Value;
				break;
			case 3:
				weapon.Poison = elementValueData.Value;
				break;
			case 4:
				weapon.Physics = elementValueData.Value;
				break;
			case 5:
				weapon.Shadow = elementValueData.Value;
				break;
			}
		}
	}

	public void EnterSpc()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return;
		}
		if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponEnh)
		{
			ExitEnh();
		}
		if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponElm)
		{
			ExitElm();
		}
		if ((bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ)
		{
			GameManager.ShowTipLocalStartKey("please_take_off_hand_item", TipType.Fail);
			return;
		}
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			InteractionManager.AllInteractToggle = false;
		}
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: false);
		}
		SingletonMonoScope<GameUIManager>.Instance.EnterWeaponSpcMode();
		RefreshSpcTip();
	}

	public void ExitSpc()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			if (SingletonMonoScope<InteractionManager>.HasInstance)
			{
				InteractionManager.AllInteractToggle = true;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: true);
			}
			ClearSpcTip();
			ClearForgeContext();
			SingletonMonoScope<GameUIManager>.Instance.ExitWeaponSpcMode();
		}
	}

	public void RefreshSpcTip()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponSpc)
		{
			RefreshForgeContext(WeaponForgeMode.Spc);
			if (!forgeContext.IsValid)
			{
				ClearSpcTip();
				return;
			}
			WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
			int remainRebuildCount = GetRemainRebuildCount(runtimeWeapon);
			SpcWeaponText.text = LOC.MM.GetMain("selected_weapon") + runtimeWeapon.GetTitle() + "     <color=#E6C96A>" + LOC.MM.GetMain("rebuild_weapon_time") + remainRebuildCount + "</color>";
			SpcPriceText.text = GetPriceRichText(forgeContext.Price);
		}
	}

	private void ClearSpcTip()
	{
		if ((bool)SpcWeaponText)
		{
			SpcWeaponText.text = "<color=#FFFFFF>" + LOC.MM.GetMain("selected_weapon") + LOC.MM.GetMain("weapon_null") + "     " + LOC.MM.GetMain("rebuild_weapon_time") + 0 + "</color>";
		}
		if ((bool)SpcPriceText)
		{
			SpcPriceText.text = "<color=#FFFFFF>" + LOC.MM.GetLevelFormat("mijing_need_price", 0) + "</color>";
		}
	}

	private void HandleSpcInput()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponSpc && IsSubmitDown())
		{
			RefreshForgeContext(WeaponForgeMode.Spc);
			if (forgeContext.IsValid && CanTryForgeSpc())
			{
				TryRandomSpc();
			}
		}
	}

	private bool CanTryForgeSpc()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeWeapon == null)
		{
			return false;
		}
		if ((bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ)
		{
			GameManager.ShowTipLocalStartKey("please_take_off_hand_item", TipType.Fail);
			return false;
		}
		if (!SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return false;
		}
		if (GetRemainRebuildCount(forgeContext.RuntimeWeapon) <= 0)
		{
			GameManager.ShowTip(LOC.MM.GetMain("rebuild_max"), TipType.Fail);
			return false;
		}
		if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < forgeContext.Price)
		{
			GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			return false;
		}
		return true;
	}

	private void TryRandomSpc()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeWeapon == null || !SingletonMonoScope<InventoryManager>.HasInstance || !SingletonMonoScope<ItemManager>.HasInstance)
		{
			return;
		}
		WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
		WPSPC spc;
		SPC_MB mb;
		if (!runtimeWeapon.HasSPC(0))
		{
			GameManager.ShowTip(LOC.MM.GetMain("weapon_spc_null"), TipType.Fail);
		}
		else if (!runtimeWeapon.TryGetSPCTemplate(0, out spc, out mb) || mb == null)
		{
			GameManager.ShowTip(LOC.MM.GetMain("weapon_spc_null"), TipType.Fail);
		}
		else if (GetRemainRebuildCount(runtimeWeapon) <= 0)
		{
			GameManager.ShowTip(LOC.MM.GetMain("rebuild_max"), TipType.Fail);
		}
		else
		{
			if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < forgeContext.Price || ItemCloneUtil.CloneWeapon(runtimeWeapon) == null)
			{
				return;
			}
			int eL = spc.EL;
			int num = UnityEngine.Random.Range(0, 6);
			if (num == eL)
			{
				num = (num + UnityEngine.Random.Range(1, 6)) % 6;
			}
			for (int i = 0; i < 2; i++)
			{
				if (runtimeWeapon.TryGetSPCData(i, out var spc2) && spc2.Index > 0)
				{
					spc2.EL = num;
				}
			}
			SingletonMonoScope<InventoryManager>.Instance.RemoveMoney(forgeContext.Price);
			runtimeWeapon.Reb_CountMax++;
			ContainerGridUtil.BindWeaponToRegion(forgeContext.Slot, SingletonMonoScope<InventoryManager>.Instance.Page);
			if (ItemCloneUtil.CloneWeapon(runtimeWeapon) != null)
			{
				RuntimeManager.PlayOneShot(audio_event, base.transform.position);
				SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(forgeContext.Slot.weapon, forgeContext.Slot, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
				RefreshSpcTip();
			}
		}
	}

	public void EnterEnh()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return;
		}
		if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponElm)
		{
			ExitElm();
		}
		if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponSpc)
		{
			ExitSpc();
		}
		if ((bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ)
		{
			GameManager.ShowTipLocalStartKey("please_take_off_hand_item", TipType.Fail);
			return;
		}
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			InteractionManager.AllInteractToggle = false;
		}
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: false);
		}
		SingletonMonoScope<GameUIManager>.Instance.EnterWeaponEnhMode();
		RefreshEnhTip();
	}

	public void ExitEnh()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			if (SingletonMonoScope<InteractionManager>.HasInstance)
			{
				InteractionManager.AllInteractToggle = true;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				SingletonMonoScope<InventoryManager>.Instance.ToggleInteract(isOn: true);
			}
			ClearEnhTip();
			ClearForgeContext();
			SingletonMonoScope<GameUIManager>.Instance.ExitWeaponEnhMode();
		}
	}

	public void RefreshEnhTip()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponEnh)
		{
			RefreshForgeContext(WeaponForgeMode.Enh);
			if (!forgeContext.IsValid)
			{
				ClearEnhTip();
				return;
			}
			WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
			int remainEnhanceCount = GetRemainEnhanceCount(runtimeWeapon);
			EnhWeaponText.text = LOC.MM.GetMain("selected_weapon") + runtimeWeapon.GetTitle() + "     <color=#E6C96A>" + LOC.MM.GetMain("enhance_weapon_time") + remainEnhanceCount + "</color>";
			EnhPriceText.text = GetPriceRichText(forgeContext.Price);
		}
	}

	private void ClearEnhTip()
	{
		if ((bool)EnhWeaponText)
		{
			EnhWeaponText.text = "<color=#FFFFFF>" + LOC.MM.GetMain("selected_weapon") + LOC.MM.GetMain("weapon_null") + "     " + LOC.MM.GetMain("enhance_weapon_time") + 0 + "</color>";
		}
		if ((bool)EnhPriceText)
		{
			EnhPriceText.text = "<color=#FFFFFF>" + LOC.MM.GetLevelFormat("mijing_need_price", 0) + "</color>";
		}
	}

	private void HandleEnhInput()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.WeaponEnh && IsSubmitDown())
		{
			RefreshForgeContext(WeaponForgeMode.Enh);
			if (forgeContext.IsValid && CanTryForgeEnh())
			{
				TryRandomEnh();
			}
		}
	}

	private bool CanTryForgeEnh()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeWeapon == null)
		{
			return false;
		}
		if ((bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ)
		{
			GameManager.ShowTipLocalStartKey("please_take_off_hand_item", TipType.Fail);
			return false;
		}
		if (!SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return false;
		}
		if (GetRemainEnhanceCount(forgeContext.RuntimeWeapon) <= 0)
		{
			GameManager.ShowTip(LOC.MM.GetMain("enhance_max"), TipType.Fail);
			return false;
		}
		if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < forgeContext.Price)
		{
			GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			return false;
		}
		return true;
	}

	private void TryRandomEnh()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeWeapon == null || !SingletonMonoScope<InventoryManager>.HasInstance || !SettingsLoader.Instance || !SettingsLoader.Instance.weaponSettings)
		{
			return;
		}
		WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
		WeaponSettings weaponSettings = SettingsLoader.Instance.weaponSettings;
		if (GetRemainEnhanceCount(runtimeWeapon) <= 0)
		{
			GameManager.ShowTip(LOC.MM.GetMain("enhance_max"), TipType.Fail);
		}
		else
		{
			if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < forgeContext.Price || ItemCloneUtil.CloneWeapon(runtimeWeapon) == null)
			{
				return;
			}
			float b = UnityEngine.Random.Range(weaponSettings.ZQ_Min, weaponSettings.ZQ_Max);
			float num = Mathf.Max(0f, b);
			if (SingletonMonoScope<PlayerManager>.HasInstance)
			{
				num *= 1f + (float)Mathf.Max(0, SingletonMonoScope<PlayerManager>.Instance.QH_Bei) / 100f;
			}
			float damage = runtimeWeapon.Damage;
			float health = runtimeWeapon.Health;
			float mana = runtimeWeapon.Mana;
			bool flag = false;
			if (runtimeWeapon.Damage > 0f)
			{
				runtimeWeapon.Damage = GetEnhancedValue(runtimeWeapon.Damage, num);
				flag = true;
			}
			if (runtimeWeapon.Health > 0f)
			{
				runtimeWeapon.Health = GetEnhancedValue(runtimeWeapon.Health, num);
				flag = true;
			}
			if (runtimeWeapon.Mana > 0f)
			{
				runtimeWeapon.Mana = GetEnhancedValue(runtimeWeapon.Mana, num);
				flag = true;
			}
			if (!flag)
			{
				GameManager.ShowTip(LOC.MM.GetMain("weapon_enhance_null"), TipType.Fail);
				return;
			}
			float num2 = Mathf.Max(0f, runtimeWeapon.Damage - damage);
			float num3 = Mathf.Max(0f, runtimeWeapon.Health - health);
			float num4 = Mathf.Max(0f, runtimeWeapon.Mana - mana);
			int b2 = Mathf.FloorToInt(num2 + num3 + num4);
			runtimeWeapon.Price += Mathf.Max(0, b2);
			SingletonMonoScope<InventoryManager>.Instance.RemoveMoney(forgeContext.Price);
			runtimeWeapon.ZQ_CountMax++;
			ContainerGridUtil.BindWeaponToRegion(forgeContext.Slot, SingletonMonoScope<InventoryManager>.Instance.Page);
			if (ItemCloneUtil.CloneWeapon(runtimeWeapon) != null)
			{
				RuntimeManager.PlayOneShot(audio_event, base.transform.position);
				SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(forgeContext.Slot.weapon, forgeContext.Slot, SingletonMonoScope<InventoryManager>.Instance.slotGrid);
				RefreshEnhTip();
			}
		}
	}

	private static bool IsSubmitDown()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return GamepadInputManager.GetSubmitDown();
		}
		return Input.GetMouseButtonDown(0);
	}

	private static bool IsValidForgeWeaponSlot(SlotData slot)
	{
		if (slot == null)
		{
			return false;
		}
		if (!slot.isOC)
		{
			return false;
		}
		if (slot.ItemType != 0)
		{
			return false;
		}
		if (slot.weapon == null)
		{
			return false;
		}
		return true;
	}

	private static float GetEnhancedValue(float baseValue, float enhanceScale)
	{
		if (baseValue <= 0f || enhanceScale <= 0f)
		{
			return baseValue;
		}
		float num = Mathf.Max(1f, Mathf.Round(baseValue * enhanceScale));
		return baseValue + num;
	}

	private static int GetRemainRebuildCount(WeaponClass weapon)
	{
		if (weapon == null || !SettingsLoader.Instance || !SettingsLoader.Instance.weaponSettings)
		{
			return 0;
		}
		int num = SettingsLoader.Instance.weaponSettings.Reb_CountMax + SingletonMonoScope<PlayerManager>.Instance.Reforge_Inc;
		return Mathf.Max(0, num - weapon.Reb_CountMax);
	}

	private static int GetRemainEnhanceCount(WeaponClass weapon)
	{
		if (weapon == null || !SettingsLoader.Instance || !SettingsLoader.Instance.weaponSettings)
		{
			return 0;
		}
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		return Mathf.Max(0, weapon.Quality switch
		{
			0 => SettingsLoader.Instance.weaponSettings.maxZQ0 + instance.QH_Inc, 
			1 => SettingsLoader.Instance.weaponSettings.maxZQ1 + instance.QH_Inc, 
			2 => SettingsLoader.Instance.weaponSettings.maxZQ2 + instance.QH_Inc, 
			3 => SettingsLoader.Instance.weaponSettings.maxZQ3 + instance.QH_Inc, 
			4 => SettingsLoader.Instance.weaponSettings.maxZQ4 + instance.QH_Inc, 
			5 => SettingsLoader.Instance.weaponSettings.maxZQ5 + instance.QH_Inc, 
			6 => SettingsLoader.Instance.weaponSettings.maxZQ6 + instance.QH_Inc, 
			_ => SettingsLoader.Instance.weaponSettings.maxZQ0 + instance.QH_Inc, 
		} - weapon.ZQ_CountMax);
	}

	private static long GetRebuildPrice(WeaponClass weapon)
	{
		if (weapon == null || !SettingsLoader.Instance || !SettingsLoader.Instance.weaponSettings)
		{
			return 0L;
		}
		WeaponSettings weaponSettings = SettingsLoader.Instance.weaponSettings;
		float num = Mathf.Pow(weaponSettings.Reb_PriceUP_Level, Mathf.Max(0, weapon.Level));
		float num2 = Mathf.Pow(weaponSettings.Reb_PriceUP_Count, Mathf.Max(0, weapon.Reb_CountMax));
		float f = weaponSettings.Reb_Price_Base * num * num2;
		return ApplyForgePriceCut(Mathf.Max(0, Mathf.RoundToInt(f)));
	}

	private static long GetEnhancePrice(WeaponClass weapon)
	{
		if (weapon == null || !SettingsLoader.Instance || !SettingsLoader.Instance.weaponSettings)
		{
			return 0L;
		}
		WeaponSettings weaponSettings = SettingsLoader.Instance.weaponSettings;
		float num = Mathf.Pow(weaponSettings.ZQ_Price_Level, Mathf.Max(0, weapon.Level));
		float num2 = Mathf.Pow(weaponSettings.ZQ_Price_Count, Mathf.Max(0, weapon.ZQ_CountMax));
		float f = weaponSettings.ZQ_Price_Base * num * num2;
		return ApplyForgePriceCut(Mathf.Max(0, Mathf.RoundToInt(f)));
	}

	private static long ApplyForgePriceCut(long price)
	{
		if (price <= 0 || !SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return price;
		}
		int num = Mathf.Clamp(SingletonMonoScope<PlayerManager>.Instance.QH_Price, 0, 100);
		return Mathf.Max(0, Mathf.RoundToInt((float)(price * (100 - num)) / 100f));
	}

	private static string GetPriceRichText(long price)
	{
		if (!SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return price.ToString();
		}
		if (price <= 0)
		{
			return "<color=#FFFFFF>0</color>";
		}
		if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < price)
		{
			return "<color=#FF0000>" + LOC.MM.GetLevelFormat("mijing_need_price", price) + "</color>";
		}
		return "<color=#00FF00>" + LOC.MM.GetLevelFormat("mijing_need_price", price) + "</color>";
	}
}
