using System;
using System.Collections.Generic;
using Container.Util;
using Core;
using Core.Settings;
using Entity.InteractableObjects.Item;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs;
using Inputs.Cursors;
using Interact;
using Localization;
using Scenes;
using UI.CustomHandler;
using UI.Panels;
using UI.UIItems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Managers;

public class BaoshiManager : ScopedSingletonMono<BaoshiManager>
{
	private enum SplitTargetType
	{
		Gem,
		TalentSkillRune,
		EquipmentSkillRune,
		EquipmentAttributeRune
	}

	private class BaoshiForgeContext
	{
		public SplitTargetType TargetType;

		public SlotData Slot;

		public WeaponClass RuntimeWeapon;

		public int SocketIndex;

		public WPAocao RuntimeSocket;

		public int SkillIndex;

		public int SpcIndex;

		public BaoshiClass RuntimeBaoshi;

		public long Price;

		public bool IsValid;

		public void Clear()
		{
			TargetType = SplitTargetType.Gem;
			Slot = null;
			RuntimeWeapon = null;
			SocketIndex = -1;
			RuntimeSocket = null;
			SkillIndex = -1;
			SpcIndex = -1;
			RuntimeBaoshi = null;
			Price = 0L;
			IsValid = false;
		}
	}

	private readonly BaoshiForgeContext forgeContext = new BaoshiForgeContext();

	[HideInInspector]
	public bool Opened;

	[Header("引用变量")]
	[SerializeField]
	private GameObject helpGroup;

	[SerializeField]
	private GameObject BaoshiGroup;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text canCreateText;

	[SerializeField]
	private Text childNumText;

	[SerializeField]
	private Text numText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private GameObject baoShiTip;

	[SerializeField]
	private CanvasGroup baoShiTipCanvasGroup;

	[SerializeField]
	private Text baoshiTipNameText;

	[SerializeField]
	private Text[] baoShiTipShortcutTexts;

	[SerializeField]
	private Text[] baoShiTipActionTexts;

	[SerializeField]
	private Button add5NumBtn;

	[SerializeField]
	private Button add1NumBtn;

	[SerializeField]
	private Button red1NumBtn;

	[SerializeField]
	private Button red5NumBtn;

	[SerializeField]
	private Button createBtn;

	[SerializeField]
	private Button splitBtn;

	[SerializeField]
	private Button splitTalentSkillRuneBtn;

	[SerializeField]
	private Button splitEquipmentSkillRuneBtn;

	[SerializeField]
	private Button splitEquipmentAttributeRuneBtn;

	[SerializeField]
	private Button helpBtn;

	[SerializeField]
	private Button backBtn;

	[SerializeField]
	private Button closeBtn;

	private BaoshiItem currentCreateSelected;

	private BaoshiClass currentCreateSelectedData;

	private int currentCreateSelectedNum = 1;

	private BaoshiItem pointerHoveredBaoshiItem;

	private readonly List<BaoshiItem> baoshiItems = new List<BaoshiItem>();

	private BaoshiSettings baoshiSettings;

	private SplitTargetType currentSplitType;

	private const string baoshiPath = "UI/Components/Game/baoshiBtn";

	private GameObject baoshiPrefab;

	private static readonly string[] SplitButtonTextKeys = new string[4] { "split_baoshi", "split_talent_skill_rune", "split_equipment_skill_rune", "split_equipment_attribute_rune" };

	private static readonly string[] BaoShiTipActionKeys = new string[4] { "baoshi_tip_create_one", "baoshi_tip_create_five", "baoshi_tip_create_one_to_bag", "baoshi_tip_create_five_to_bag" };

	private const float BaoShiTipRightOffsetPixels = 100f;

	private float refreshTimer;

	[SerializeField]
	private float refreshInterval = 0.2f;

	private bool suppressNextGamepadLeftCreateClick;

	private bool suppressNextGamepadRightCreateClick;

	private bool gamepadLeftCreateClickUseFive;

	private bool gamepadRightCreateClickUseFive;

	private int gamepadLeftCreateReleaseFrame = -1;

	private int gamepadRightCreateReleaseFrame = -1;

	private void ClearForgeContext()
	{
		forgeContext.Clear();
	}

	private void RefreshForgeContext()
	{
		forgeContext.Clear();
		if (!SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return;
		}
		SlotData mainSlot = ContainerGridUtil.GetMainSlot(SingletonMonoScope<InventoryManager>.Instance.MouseSlotDT, SingletonMonoScope<InventoryManager>.Instance.Page);
		if (!IsValidForgeSlot(mainSlot))
		{
			return;
		}
		WeaponClass weapon = mainSlot.weapon;
		if (weapon != null)
		{
			forgeContext.TargetType = currentSplitType;
			forgeContext.Slot = mainSlot;
			forgeContext.RuntimeWeapon = weapon;
			switch (currentSplitType)
			{
			case SplitTargetType.Gem:
				RefreshGemSplitContext(weapon);
				break;
			case SplitTargetType.TalentSkillRune:
				RefreshTalentSkillRuneSplitContext(weapon);
				break;
			case SplitTargetType.EquipmentSkillRune:
				RefreshEquipmentSkillRuneSplitContext(weapon);
				break;
			case SplitTargetType.EquipmentAttributeRune:
				RefreshEquipmentAttributeRuneSplitContext(weapon);
				break;
			}
		}
	}

	private void RefreshGemSplitContext(WeaponClass runtimeWeapon)
	{
		int lastSocketedGemIndex = GetLastSocketedGemIndex(runtimeWeapon);
		if (lastSocketedGemIndex < 0 || runtimeWeapon.Aocao == null || lastSocketedGemIndex >= runtimeWeapon.Aocao.Count)
		{
			LogUtil.Warn($"获取到非法宝石凹槽序列: {lastSocketedGemIndex}，自动清空跳过");
			return;
		}
		WPAocao wPAocao = runtimeWeapon.Aocao[lastSocketedGemIndex];
		if (wPAocao == null || !wPAocao.HasAocao || !wPAocao.HasBaoshi || string.IsNullOrEmpty(wPAocao.Name))
		{
			LogUtil.Warn($"获取到非法凹槽宝石数据: {lastSocketedGemIndex}，自动清空跳过");
			return;
		}
		BaoshiClass baoshiClass = BuildRuntimeBaoshiFromSocket(wPAocao);
		if (baoshiClass == null)
		{
			LogUtil.Warn("无法根据凹槽数据构建运行时宝石: " + wPAocao.Name);
			return;
		}
		forgeContext.SocketIndex = lastSocketedGemIndex;
		forgeContext.RuntimeSocket = wPAocao;
		SetValidForgeContext(baoshiClass);
	}

	private void RefreshTalentSkillRuneSplitContext(WeaponClass runtimeWeapon)
	{
		if (TryGetLastTalentSkillRune(runtimeWeapon, out var skillIndex, out var skill) && SingletonMonoScope<ItemManager>.HasInstance)
		{
			int skillRuneUnitPrice = GetSkillRuneUnitPrice(skill);
			if (SingletonMonoScope<ItemManager>.Instance.TryCreateSkillRuneFromWeaponSkill(skill, skillRuneUnitPrice, out var baoshi))
			{
				forgeContext.SkillIndex = skillIndex;
				SetValidForgeContext(baoshi);
			}
		}
	}

	private void RefreshEquipmentSkillRuneSplitContext(WeaponClass runtimeWeapon)
	{
		if (TryGetLastEquipmentSkillRune(runtimeWeapon, out var spcIndex, out var spc) && SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.TryCreateSPCRuneFromWeaponSPC(spc, out var baoshi))
		{
			forgeContext.SpcIndex = spcIndex;
			SetValidForgeContext(baoshi);
		}
	}

	private void RefreshEquipmentAttributeRuneSplitContext(WeaponClass runtimeWeapon)
	{
		if (HasEquipmentAttributeRune(runtimeWeapon) && SingletonMonoScope<ItemManager>.HasInstance)
		{
			int equipmentAttributeRuneType = GetEquipmentAttributeRuneType(runtimeWeapon);
			if (SingletonMonoScope<ItemManager>.Instance.TryCreateAttributeRuneFromWeaponBase(runtimeWeapon.FW_Base, equipmentAttributeRuneType, out var baoshi))
			{
				SetValidForgeContext(baoshi);
			}
		}
	}

	private void SetValidForgeContext(BaoshiClass runtimeBaoshi)
	{
		forgeContext.RuntimeBaoshi = runtimeBaoshi;
		forgeContext.Price = GetCurrentSplitPrice(runtimeBaoshi);
		forgeContext.IsValid = true;
	}

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		if (!helpGroup)
		{
			helpGroup = base.transform.Find("MainGroup/HelpGroup").gameObject;
		}
		if (!BaoshiGroup)
		{
			BaoshiGroup = base.transform.Find("MainGroup/Content/BaoshiGroup").gameObject;
		}
		if (!canvasGroup)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		ResolveBaoShiTipReferences();
		if (!numText)
		{
			numText = base.transform.Find("MainGroup/Content/Number_option/Num/NumText").GetComponent<Text>();
		}
		if (!createBtn)
		{
			createBtn = base.transform.Find("MainGroup/Content/MainBtns/CreateBtn").GetComponent<Button>();
		}
		if (!splitBtn)
		{
			splitBtn = base.transform.Find("MainGroup/Content/MainBtns/SplitBtn").GetComponent<Button>();
		}
		ResolveSplitButtons();
		if (!canCreateText)
		{
			canCreateText = createBtn.transform.Find("CreateTip/canCreateNumText").GetComponent<Text>();
		}
		if (!childNumText)
		{
			childNumText = createBtn.transform.Find("CreateTip/childNumText").GetComponent<Text>();
		}
		if (!add1NumBtn)
		{
			add1NumBtn = base.transform.Find("MainGroup/Content/Number_option/Add1").GetComponent<Button>();
		}
		add1NumBtn.onClick.AddListener(OnAdd1);
		if (!add5NumBtn)
		{
			add5NumBtn = base.transform.Find("MainGroup/Content/Number_option/Add5").GetComponent<Button>();
		}
		add5NumBtn.onClick.AddListener(OnAdd5);
		if (!red1NumBtn)
		{
			red1NumBtn = base.transform.Find("MainGroup/Content/Number_option/Red1").GetComponent<Button>();
		}
		red1NumBtn.onClick.AddListener(OnRed1);
		if (!red5NumBtn)
		{
			red5NumBtn = base.transform.Find("MainGroup/Content/Number_option/Red5").GetComponent<Button>();
		}
		red5NumBtn.onClick.AddListener(OnRed5);
		if (!closeBtn)
		{
			closeBtn = base.transform.Find("CloseBtn").GetComponent<Button>();
		}
		closeBtn.onClick.AddListener(OnClickClose);
		if (!helpBtn)
		{
			helpBtn = base.transform.Find("HelpBtn").GetComponent<Button>();
		}
		helpBtn.onClick.AddListener(OnClickHelpOpen);
		if (!backBtn)
		{
			backBtn = helpGroup.transform.Find("HelpBg/BackBtn").GetComponent<Button>();
		}
		backBtn.onClick.AddListener(OnClickHelpBack);
		baoshiSettings = SettingsLoader.Instance.baoshiSettings;
		if (!baoshiPrefab)
		{
			baoshiPrefab = Singleton<ResManager>.Instance.Load<GameObject>("UI/Components/Game/baoshiBtn");
		}
		helpGroup.SetActive(value: false);
		BindSplitButton(splitBtn, SplitTargetType.Gem);
		BindSplitButton(splitTalentSkillRuneBtn, SplitTargetType.TalentSkillRune);
		BindSplitButton(splitEquipmentSkillRuneBtn, SplitTargetType.EquipmentSkillRune);
		BindSplitButton(splitEquipmentAttributeRuneBtn, SplitTargetType.EquipmentAttributeRune);
		RefreshSplitButtonTexts();
		createBtn.onClick.AddListener(CreateBaoshi);
		HideBaoShiTip();
		HideSplitButtonTips();
		ClearSplitTip();
	}

	private void OnEnable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged += OnCurrentInputDeviceChanged;
		LOC.MM.OnLanguageChanged += OnLanguageChanged;
	}

	private void OnDisable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged -= OnCurrentInputDeviceChanged;
		LOC.MM.OnLanguageChanged -= OnLanguageChanged;
		ClearCreateHoverTipState();
	}

	private void Update()
	{
		if (Opened && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			refreshTimer += Time.unscaledDeltaTime;
			if (refreshTimer >= refreshInterval)
			{
				refreshTimer = 0f;
				RefreshCreatePanelCountsOnly();
			}
			RefreshBaoShiTipHoverState();
			HandleCreateGamepadInput();
			RefreshGamepadCreateClickSuppressions();
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.BaoshiSplit)
			{
				RefreshSplitTip();
				HandleSplitInput();
			}
		}
	}

	private void OnClickClose()
	{
		CloseBaoshi();
	}

	private void OnClickHelpOpen()
	{
		helpGroup.SetActive(value: true);
	}

	private void OnClickHelpBack()
	{
		helpGroup.SetActive(value: false);
	}

	public void EnterSplitBaoshi()
	{
		EnterSplitMode(SplitTargetType.Gem);
	}

	public void EnterSplitTalentSkillRune()
	{
		EnterSplitMode(SplitTargetType.TalentSkillRune);
	}

	public void EnterSplitEquipmentSkillRune()
	{
		EnterSplitMode(SplitTargetType.EquipmentSkillRune);
	}

	public void EnterSplitEquipmentAttributeRune()
	{
		EnterSplitMode(SplitTargetType.EquipmentAttributeRune);
	}

	public void OpenBaoshi()
	{
		if (SettingsLoader.Instance.BaoshiToggle && SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.Opened_baoshi = true;
			Opened = true;
			ClearCreateHoverTipState();
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1f;
			SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 1f;
			SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = true;
			SingletonMonoScope<GameUIManager>.Instance.Opened_IV = true;
			RefreshUI();
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.BaoshiSplit)
			{
				ExitSplitBaoshi();
			}
			HideBaoShiTip();
		}
	}

	public void CloseBaoshi()
	{
		ClearCreateHoverTipState();
		if (SettingsLoader.Instance.BaoshiToggle && SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.Opened_baoshi = false;
			Opened = false;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0f;
			refreshTimer = 0f;
			SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 0f;
			SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = false;
			SingletonMonoScope<GameUIManager>.Instance.Opened_IV = false;
			if (SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.BaoshiSplit)
			{
				ExitSplitBaoshi();
			}
		}
	}

	private void EnterSplitMode(SplitTargetType targetType)
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return;
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
		currentSplitType = targetType;
		SingletonMonoScope<GameUIManager>.Instance.EnterBaoshiSplitMode();
		RefreshSplitTip();
	}

	public void ExitSplitBaoshi()
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
			ClearForgeContext();
			currentSplitType = SplitTargetType.Gem;
			SingletonMonoScope<GameUIManager>.Instance.ClearSplitBaoshiTip();
			SingletonMonoScope<GameUIManager>.Instance.ExitBaoshiSplitMode();
		}
	}

	private void HandleSplitInput()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.BaoshiSplit && IsSubmitDown())
		{
			RefreshForgeContext();
			if (forgeContext.IsValid && CanTrySplit())
			{
				TrySplitCurrentItem();
			}
		}
	}

	private bool CanTrySplit()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeBaoshi == null)
		{
			return false;
		}
		if (forgeContext.RuntimeWeapon == null)
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
		if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < forgeContext.Price)
		{
			GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			return false;
		}
		return true;
	}

	private void TrySplitCurrentItem()
	{
		if (!forgeContext.IsValid || forgeContext.RuntimeBaoshi == null || !SingletonMonoScope<InventoryManager>.HasInstance || !SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return;
		}
		InventoryManager instance = SingletonMonoScope<InventoryManager>.Instance;
		if (instance.GlobalMoney < forgeContext.Price)
		{
			GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			return;
		}
		BaoshiClass baoshiClass = ItemCloneUtil.CloneBaoshi(forgeContext.RuntimeBaoshi);
		if (baoshiClass == null)
		{
			return;
		}
		baoshiClass.CstackSize = 1;
		if (RemoveCurrentSplitItemFromWeapon())
		{
			instance.RemoveMoney(forgeContext.Price);
			GiveSplitBaoshiResult(baoshiClass);
			SlotData mainSlot = ContainerGridUtil.GetMainSlot(forgeContext.Slot, instance.Page);
			if (mainSlot != null && (bool)mainSlot.ItemOBJ)
			{
				mainSlot.ItemOBJ.RefreshBS(mainSlot);
			}
			GameManager.ShowTip(LOC.MM.GetMainFormat(GetSplitSuccessKey(forgeContext.TargetType), forgeContext.Price), TipType.Success);
			RefreshSplitTip();
		}
	}

	private void RefreshSplitTip()
	{
		if (SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			RefreshForgeContext();
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			if (forgeContext.IsValid)
			{
				SingletonMonoScope<GameUIManager>.Instance.RefreshSplitBaoshiTip(isShow: true, forgeContext.RuntimeBaoshi, forgeContext.Price);
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.ClearSplitBaoshiTip();
			}
		}
	}

	private bool RemoveCurrentSplitItemFromWeapon()
	{
		if (!forgeContext.IsValid)
		{
			return false;
		}
		if (forgeContext.RuntimeWeapon == null)
		{
			return false;
		}
		return forgeContext.TargetType switch
		{
			SplitTargetType.Gem => RemoveCurrentSplitBaoshiFromWeapon(), 
			SplitTargetType.TalentSkillRune => RemoveCurrentSplitTalentSkillRuneFromWeapon(), 
			SplitTargetType.EquipmentSkillRune => RemoveCurrentSplitEquipmentSkillRuneFromWeapon(), 
			SplitTargetType.EquipmentAttributeRune => RemoveCurrentSplitEquipmentAttributeRuneFromWeapon(), 
			_ => false, 
		};
	}

	private bool RemoveCurrentSplitBaoshiFromWeapon()
	{
		if (forgeContext.RuntimeWeapon == null || forgeContext.RuntimeSocket == null)
		{
			return false;
		}
		int socketIndex = forgeContext.SocketIndex;
		if (socketIndex < 0 || forgeContext.RuntimeWeapon.Aocao == null || socketIndex >= forgeContext.RuntimeWeapon.Aocao.Count)
		{
			return false;
		}
		WPAocao wPAocao = forgeContext.RuntimeWeapon.Aocao[socketIndex];
		if (wPAocao == null || !wPAocao.HasAocao || !wPAocao.HasBaoshi)
		{
			return false;
		}
		if (forgeContext.RuntimeBaoshi != null)
		{
			forgeContext.RuntimeWeapon.Price = Mathf.Max(0, forgeContext.RuntimeWeapon.Price - forgeContext.RuntimeBaoshi.Price);
		}
		wPAocao.HasBaoshi = false;
		wPAocao.Name = string.Empty;
		wPAocao.Type = 0;
		wPAocao.UseType = 0;
		wPAocao.BS_Quality = 0;
		wPAocao.Number = 0f;
		wPAocao.Icon = null;
		BindCurrentWeaponToInventory();
		return true;
	}

	private bool RemoveCurrentSplitTalentSkillRuneFromWeapon()
	{
		WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
		int skillIndex = forgeContext.SkillIndex;
		if (runtimeWeapon?.WPSK == null || skillIndex < 0 || skillIndex >= runtimeWeapon.WPSK.Count)
		{
			return false;
		}
		WPSkill wPSkill = runtimeWeapon.WPSK[skillIndex];
		if (wPSkill == null || wPSkill.Number2 <= 0)
		{
			return false;
		}
		int num = ((forgeContext.RuntimeBaoshi != null) ? Mathf.Max(0, forgeContext.RuntimeBaoshi.Price) : GetSkillRuneUnitPrice(wPSkill));
		runtimeWeapon.Price = Mathf.Max(0, runtimeWeapon.Price - num);
		wPSkill.price = Mathf.Max(0, wPSkill.price - num);
		wPSkill.Number2 = Mathf.Max(0, wPSkill.Number2 - 1);
		runtimeWeapon.SKCount = Mathf.Max(0, runtimeWeapon.SKCount - 1);
		if (wPSkill.Number <= 0 && wPSkill.Number2 <= 0)
		{
			wPSkill.IndexName = "0";
			wPSkill.Number = 0;
			wPSkill.price = 0;
		}
		NormalizeWeaponSkillCount(runtimeWeapon);
		BindCurrentWeaponToInventory();
		return true;
	}

	private bool RemoveCurrentSplitEquipmentSkillRuneFromWeapon()
	{
		WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
		int spcIndex = forgeContext.SpcIndex;
		if (runtimeWeapon?.SPC == null || spcIndex < 0 || spcIndex >= runtimeWeapon.SPC.Count)
		{
			return false;
		}
		WPSPC wPSPC = runtimeWeapon.SPC[spcIndex];
		if (wPSPC == null || wPSPC.Index <= 0)
		{
			return false;
		}
		int num = Mathf.Max(0, wPSPC.price);
		runtimeWeapon.Price = Mathf.Max(0, runtimeWeapon.Price - num);
		wPSPC.Index = 0;
		wPSPC.EL = 0;
		wPSPC.PRC = 0f;
		wPSPC.price = 0;
		BindCurrentWeaponToInventory();
		return true;
	}

	private bool RemoveCurrentSplitEquipmentAttributeRuneFromWeapon()
	{
		WeaponClass runtimeWeapon = forgeContext.RuntimeWeapon;
		if (!HasEquipmentAttributeRune(runtimeWeapon))
		{
			return false;
		}
		int num = Mathf.Max(0, runtimeWeapon.FW_Base.price);
		runtimeWeapon.Price = Mathf.Max(0, runtimeWeapon.Price - num);
		runtimeWeapon.FW_Base = null;
		BindCurrentWeaponToInventory();
		return true;
	}

	private void BindCurrentWeaponToInventory()
	{
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			ContainerGridUtil.BindWeaponToRegion(forgeContext.Slot, SingletonMonoScope<InventoryManager>.Instance.Page);
		}
	}

	private static void GiveSplitBaoshiResult(BaoshiClass baoshi)
	{
		if (baoshi != null && (!SingletonMonoScope<InventoryManager>.HasInstance || !SingletonMonoScope<InventoryManager>.Instance.TryAddBaoshiToInventory(baoshi)) && SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.ThrowBS(baoshi);
		}
	}

	private static string GetSplitSuccessKey(SplitTargetType targetType)
	{
		if (targetType != 0)
		{
			return "split_rune_success";
		}
		return "split_baoshi_success";
	}

	public void RefreshUI()
	{
		currentCreateSelectedNum = 1;
		BuildAllBaoshi();
		RefreshCreatePanelCountsOnly();
		RefreshNum();
	}

	private void RefreshCreatePanelCountsOnly()
	{
		for (int i = 0; i < baoshiItems.Count; i++)
		{
			if ((bool)baoshiItems[i] && baoshiItems[i].gameObject.activeInHierarchy)
			{
				baoshiItems[i].RefreshCount();
			}
		}
	}

	private static void ClearSplitTip()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.ClearSplitBaoshiTip();
		}
	}

	private void ResolveSplitButtons()
	{
		Transform transform = base.transform.Find("MainGroup/Content/MainBtns");
		if ((bool)transform)
		{
			if (!splitBtn)
			{
				splitBtn = transform.Find("SplitBtn")?.GetComponent<Button>();
			}
			if (!splitTalentSkillRuneBtn)
			{
				splitTalentSkillRuneBtn = ResolveSplitButton(transform, "SplitTalentSkillRuneBtn", "SplitFW", splitBtn, 1);
			}
			if (!splitEquipmentSkillRuneBtn)
			{
				splitEquipmentSkillRuneBtn = ResolveSplitButton(transform, "SplitEquipmentSkillRuneBtn", null, splitBtn, 2);
			}
			if (!splitEquipmentAttributeRuneBtn)
			{
				splitEquipmentAttributeRuneBtn = ResolveSplitButton(transform, "SplitEquipmentAttributeRuneBtn", null, splitBtn, 3);
			}
			SetSplitButtonOrder();
		}
	}

	private Button ResolveSplitButton(Transform mainBtns, string name, string fallbackName, Button template, int siblingIndex)
	{
		Transform transform = mainBtns.Find(name);
		if (!transform && !string.IsNullOrEmpty(fallbackName))
		{
			transform = mainBtns.Find(fallbackName);
			if ((bool)transform)
			{
				transform.name = name;
			}
		}
		if (!transform && (bool)template)
		{
			GameObject obj = UnityEngine.Object.Instantiate(template.gameObject, mainBtns);
			obj.name = name;
			transform = obj.transform;
		}
		if (!transform)
		{
			return null;
		}
		transform.gameObject.SetActive(value: true);
		transform.SetSiblingIndex(siblingIndex);
		return transform.GetComponent<Button>();
	}

	private void SetSplitButtonOrder()
	{
		if ((bool)splitBtn)
		{
			splitBtn.transform.SetSiblingIndex(0);
		}
		if ((bool)splitTalentSkillRuneBtn)
		{
			splitTalentSkillRuneBtn.transform.SetSiblingIndex(1);
		}
		if ((bool)splitEquipmentSkillRuneBtn)
		{
			splitEquipmentSkillRuneBtn.transform.SetSiblingIndex(2);
		}
		if ((bool)splitEquipmentAttributeRuneBtn)
		{
			splitEquipmentAttributeRuneBtn.transform.SetSiblingIndex(3);
		}
	}

	private void BindSplitButton(Button button, SplitTargetType targetType)
	{
		if ((bool)button)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate
			{
				EnterSplitMode(targetType);
			});
		}
	}

	private void RefreshSplitButtonTexts()
	{
		Button[] array = new Button[4] { splitBtn, splitTalentSkillRuneBtn, splitEquipmentSkillRuneBtn, splitEquipmentAttributeRuneBtn };
		for (int i = 0; i < array.Length && i < SplitButtonTextKeys.Length; i++)
		{
			RefreshSplitButtonTipText(array[i], SplitButtonTextKeys[i]);
		}
	}

	private void RefreshSplitButtonTipText(Button button, string titleKey)
	{
		if ((bool)button)
		{
			Transform transform = button.transform.Find("SplitTip");
			if ((bool)transform)
			{
				SetLocalizedChildText(transform, "Text", titleKey);
				SetLocalizedChildText(transform, "Text (1)", "split_weapon");
			}
		}
	}

	private static void SetLocalizedChildText(Transform root, string childName, string key)
	{
		Transform transform = root.Find(childName);
		if ((bool)transform)
		{
			LocalizedText component = transform.GetComponent<LocalizedText>();
			if ((bool)component)
			{
				component.Set(LocalizationExcelList.Main_FY, key);
			}
			Text component2 = transform.GetComponent<Text>();
			if ((bool)component2)
			{
				component2.text = LOC.MM.GetMain(key);
			}
		}
	}

	private void HideSplitButtonTips()
	{
		Button[] array = new Button[4] { splitBtn, splitTalentSkillRuneBtn, splitEquipmentSkillRuneBtn, splitEquipmentAttributeRuneBtn };
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i])
			{
				UIButtonState component = array[i].GetComponent<UIButtonState>();
				if ((bool)component)
				{
					component.ForceHoverExit();
					component.SetPressed(pressed: false);
				}
				Transform transform = array[i].transform.Find("SplitTip");
				if ((bool)transform)
				{
					transform.gameObject.SetActive(value: false);
				}
			}
		}
	}

	private void RefreshPriceText()
	{
		if (!priceText)
		{
			return;
		}
		if (!currentCreateSelected || currentCreateSelectedData == null)
		{
			priceText.text = string.Empty;
			if ((bool)baoshiTipNameText)
			{
				baoshiTipNameText.text = string.Empty;
			}
			return;
		}
		if ((bool)baoshiTipNameText)
		{
			baoshiTipNameText.text = currentCreateSelectedData.GetTitle();
		}
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			long currentCreatePrice = GetCurrentCreatePrice();
			string text = ((currentCreatePrice <= SingletonMonoScope<InventoryManager>.Instance.GlobalMoney && currentCreatePrice > 0) ? ("<color=#00FF00>" + LOC.MM.GetLevelFormat("mijing_need_price", currentCreatePrice) + "</color>") : ((currentCreatePrice > SingletonMonoScope<InventoryManager>.Instance.GlobalMoney) ? ("<color=#FF0000>" + LOC.MM.GetLevelFormat("mijing_need_price", currentCreatePrice) + "</color>") : ((currentCreatePrice != 0L) ? currentCreatePrice.ToString() : ("<color=#FFFFFF>" + LOC.MM.GetLevelFormat("mijing_need_price", currentCreatePrice) + "</color>"))));
			priceText.text = text;
		}
	}

	private long GetCurrentCreatePrice()
	{
		if (currentCreateSelectedData == null || !baoshiSettings)
		{
			return 0L;
		}
		return currentCreateSelectedData.BS_Quality switch
		{
			7 => baoshiSettings.createPrice7 * currentCreateSelectedNum, 
			6 => baoshiSettings.createPrice6 * currentCreateSelectedNum, 
			5 => baoshiSettings.createPrice5 * currentCreateSelectedNum, 
			4 => baoshiSettings.createPrice4 * currentCreateSelectedNum, 
			3 => baoshiSettings.createPrice3 * currentCreateSelectedNum, 
			2 => baoshiSettings.createPrice2 * currentCreateSelectedNum, 
			1 => baoshiSettings.createPrice1 * currentCreateSelectedNum, 
			_ => 0L, 
		};
	}

	private long GetCurrentSplitPrice(BaoshiClass data)
	{
		if (data == null || !baoshiSettings)
		{
			return 0L;
		}
		return data.BS_Quality switch
		{
			7 => baoshiSettings.splitPrice7, 
			6 => baoshiSettings.splitPrice6, 
			5 => baoshiSettings.splitPrice5, 
			4 => baoshiSettings.splitPrice4, 
			3 => baoshiSettings.splitPrice3, 
			2 => baoshiSettings.splitPrice2, 
			1 => baoshiSettings.splitPrice1, 
			0 => baoshiSettings.splitPrice0, 
			_ => 0L, 
		};
	}

	public void BuildAllBaoshi()
	{
		if (!SingletonMonoScope<ItemManager>.HasInstance || !BaoshiGroup)
		{
			return;
		}
		string text = ((currentCreateSelectedData != null) ? currentCreateSelectedData.ItemName : null);
		BaoshiItem baoshiItem = null;
		baoshiItems.Clear();
		currentCreateSelected = null;
		currentCreateSelectedData = null;
		List<BaoshiClass> baoshi = SingletonMonoScope<ItemManager>.Instance.Baoshi;
		if (baoshi == null || baoshi.Count == 0 || baoshi.Count < 48)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				int num2 = j * 8;
				int num3 = num2 + i;
				int num4 = num2 + i + 1;
				if (num3 < 0 || num3 >= baoshi.Count || num4 < 0 || num4 >= baoshi.Count)
				{
					continue;
				}
				BaoshiClass baoshiClass = baoshi[num3];
				BaoshiClass baoshiClass2 = baoshi[num4];
				if (baoshiClass == null || baoshiClass2 == null)
				{
					continue;
				}
				BaoshiItem baoshiItem2 = null;
				if (num < BaoshiGroup.transform.childCount)
				{
					Transform child = BaoshiGroup.transform.GetChild(num);
					if ((bool)child)
					{
						baoshiItem2 = child.GetComponent<BaoshiItem>();
					}
				}
				if (!baoshiItem2)
				{
					baoshiItem2 = UnityEngine.Object.Instantiate(baoshiPrefab, BaoshiGroup.transform).GetComponent<BaoshiItem>();
				}
				if ((bool)baoshiItem2)
				{
					baoshiItem2.gameObject.SetActive(value: true);
					baoshiItem2.Init(this, baoshiClass, baoshiClass2, baoshiSettings.needCount);
					baoshiItems.Add(baoshiItem2);
					if (baoshiItem == null && !string.IsNullOrEmpty(text) && baoshiClass.ItemName == text)
					{
						baoshiItem = baoshiItem2;
					}
					num++;
				}
			}
		}
		for (int k = num; k < BaoshiGroup.transform.childCount; k++)
		{
			Transform child2 = BaoshiGroup.transform.GetChild(k);
			if ((bool)child2)
			{
				child2.gameObject.SetActive(value: false);
			}
		}
		if ((bool)baoshiItem)
		{
			SelectItem(baoshiItem);
		}
		else if (baoshiItems.Count > 0)
		{
			SelectItem(baoshiItems[0]);
		}
	}

	public void SelectItem(BaoshiItem item)
	{
		if (!item)
		{
			return;
		}
		if (currentCreateSelected == item)
		{
			ShowBaoShiTip();
			return;
		}
		if ((bool)currentCreateSelected)
		{
			currentCreateSelected.SetSelected(selected: false);
		}
		currentCreateSelected = item;
		currentCreateSelectedData = currentCreateSelected.baoshiData;
		currentCreateSelected.SetSelected(selected: true);
		currentCreateSelectedNum = 1;
		RefreshNum();
		ShowBaoShiTip();
	}

	public BaoshiItem GetCurrentSelected()
	{
		return currentCreateSelected;
	}

	public void ClearSelectedItem(BaoshiItem item)
	{
		if ((bool)item && !(currentCreateSelected != item))
		{
			pointerHoveredBaoshiItem = null;
			currentCreateSelected.SetSelected(selected: false);
			currentCreateSelected = null;
			currentCreateSelectedData = null;
			currentCreateSelectedNum = 1;
			RefreshNum();
			HideBaoShiTip();
		}
	}

	private void RefreshBaoShiTipHoverState()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.BaoshiSplit)
		{
			pointerHoveredBaoshiItem = null;
			HideBaoShiTip();
			return;
		}
		BaoshiItem baoshiItemUnderPointer = GetBaoshiItemUnderPointer();
		if ((bool)baoshiItemUnderPointer)
		{
			pointerHoveredBaoshiItem = baoshiItemUnderPointer;
			SelectItem(baoshiItemUnderPointer);
		}
		else if ((bool)pointerHoveredBaoshiItem)
		{
			pointerHoveredBaoshiItem = null;
			if ((bool)currentCreateSelected)
			{
				currentCreateSelected.SetSelected(selected: false);
				currentCreateSelected = null;
				currentCreateSelectedData = null;
				currentCreateSelectedNum = 1;
				RefreshNum();
			}
			HideBaoShiTip();
		}
	}

	private void ClearCreateHoverTipState()
	{
		pointerHoveredBaoshiItem = null;
		if ((bool)currentCreateSelected)
		{
			currentCreateSelected.SetSelected(selected: false);
		}
		HideBaoShiTip();
		HideSplitButtonTips();
	}

	private BaoshiItem GetBaoshiItemUnderPointer()
	{
		if (baoshiItems.Count == 0)
		{
			return null;
		}
		Vector2 screenPoint = CursorManager.GetCurrentPointerScreenPosition();
		Camera uiCamera = GetUiCamera();
		for (int i = 0; i < baoshiItems.Count; i++)
		{
			BaoshiItem baoshiItem = baoshiItems[i];
			if ((bool)baoshiItem && baoshiItem.gameObject.activeInHierarchy)
			{
				RectTransform component = baoshiItem.GetComponent<RectTransform>();
				if ((bool)component && RectTransformUtility.RectangleContainsScreenPoint(component, screenPoint, uiCamera))
				{
					return baoshiItem;
				}
			}
		}
		return null;
	}

	private Camera GetUiCamera()
	{
		Canvas canvas = (canvasGroup ? canvasGroup.GetComponentInParent<Canvas>() : GetComponentInParent<Canvas>());
		if (!canvas || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return null;
		}
		return canvas.worldCamera;
	}

	private void OnAdd1()
	{
		int maxSelectableNum = GetMaxSelectableNum();
		if (currentCreateSelectedNum < maxSelectableNum)
		{
			currentCreateSelectedNum++;
			RefreshNum();
		}
	}

	private void OnAdd5()
	{
		int maxSelectableNum = GetMaxSelectableNum();
		if (currentCreateSelectedNum < maxSelectableNum)
		{
			currentCreateSelectedNum += 5;
			if (currentCreateSelectedNum > maxSelectableNum)
			{
				currentCreateSelectedNum = maxSelectableNum;
			}
			RefreshNum();
		}
	}

	private void OnRed1()
	{
		if (currentCreateSelectedNum > 1)
		{
			currentCreateSelectedNum--;
			RefreshNum();
		}
	}

	private void OnRed5()
	{
		if (currentCreateSelectedNum > 1)
		{
			currentCreateSelectedNum -= 5;
			if (currentCreateSelectedNum < 1)
			{
				currentCreateSelectedNum = 1;
			}
			RefreshNum();
		}
	}

	private int GetMaxSelectableNum()
	{
		if (!SingletonMonoScope<InventoryManager>.HasInstance || !currentCreateSelected || currentCreateSelected.childBaoshiData == null)
		{
			return 0;
		}
		if (baoshiSettings.needCount <= 0)
		{
			return 0;
		}
		return SingletonMonoScope<InventoryManager>.Instance.GetBaoshiTotalCountInInv(currentCreateSelected.childBaoshiData.ItemName) / baoshiSettings.needCount;
	}

	private void RefreshNum()
	{
		int maxSelectableNum = GetMaxSelectableNum();
		if (maxSelectableNum <= 0)
		{
			currentCreateSelectedNum = 0;
		}
		else
		{
			if (currentCreateSelectedNum < 1)
			{
				currentCreateSelectedNum = 1;
			}
			if (currentCreateSelectedNum > maxSelectableNum)
			{
				currentCreateSelectedNum = maxSelectableNum;
			}
		}
		if ((bool)numText)
		{
			numText.text = currentCreateSelectedNum.ToString();
		}
		if ((bool)red1NumBtn)
		{
			red1NumBtn.interactable = currentCreateSelectedNum > 1;
		}
		if ((bool)red5NumBtn)
		{
			red5NumBtn.interactable = currentCreateSelectedNum > 1;
		}
		if ((bool)add1NumBtn)
		{
			add1NumBtn.interactable = currentCreateSelectedNum < maxSelectableNum;
		}
		if ((bool)add5NumBtn)
		{
			add5NumBtn.interactable = currentCreateSelectedNum < maxSelectableNum;
		}
		RefreshCreateInfo();
	}

	private void RefreshCreateInfo()
	{
		if (!currentCreateSelected || !SingletonMonoScope<InventoryManager>.HasInstance || currentCreateSelected.childBaoshiData == null)
		{
			if ((bool)canCreateText)
			{
				canCreateText.text = LOC.MM.GetStart("can_create_baoshi_count") + ": 0";
			}
			if ((bool)childNumText)
			{
				childNumText.text = LOC.MM.GetStart("child_baoshi_count") + ": 0/0";
			}
			RefreshPriceText();
			return;
		}
		int baoshiTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetBaoshiTotalCountInInv(GetCurrentSelected().childBaoshiData.ItemName);
		int num = ((baoshiSettings.needCount > 0) ? (baoshiTotalCountInInv / baoshiSettings.needCount) : 0);
		int num2 = currentCreateSelectedNum * baoshiSettings.needCount;
		if ((bool)canCreateText)
		{
			if (num >= 1)
			{
				canCreateText.text = LOC.MM.GetStart("can_create_baoshi_count") + ": " + $"<color=#00FF00>{num}</color>";
			}
			else
			{
				canCreateText.text = LOC.MM.GetStart("can_create_baoshi_count") + ": " + $"<color=#FF0000>{num}</color>";
			}
		}
		string text = ((baoshiTotalCountInInv < num2) ? $"<color=#FF0000>{baoshiTotalCountInInv}/{num2}</color>" : $"<color=#00FF00>{baoshiTotalCountInInv}/{num2}</color>");
		if ((bool)childNumText)
		{
			childNumText.text = LOC.MM.GetStart("child_baoshi_count") + ": " + text;
		}
		RefreshPriceText();
	}

	public void CreateBaoshi()
	{
		TryCreateFromItem(currentCreateSelected, currentCreateSelectedNum, directToInventory: false);
	}

	public bool IsGamepadCreateShortcutDown()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			if (!Input.GetKey(KeyCode.JoystickButton0) && !Input.GetKey(KeyCode.JoystickButton1) && !Input.GetKey(KeyCode.JoystickButton2))
			{
				return Input.GetKey(KeyCode.JoystickButton3);
			}
			return true;
		}
		return false;
	}

	public bool IsDirectCreateToInventoryInput()
	{
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			return true;
		}
		return false;
	}

	private void HandleCreateGamepadInput()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent() || SingletonMonoScope<GameUIManager>.Instance.CurrentModalState == GlobalUiModalState.BaoshiSplit)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			gamepadLeftCreateClickUseFive = false;
			gamepadLeftCreateReleaseFrame = -1;
			if (TryCreateFromItem(currentCreateSelected, 1, directToInventory: false))
			{
				suppressNextGamepadLeftCreateClick = true;
			}
		}
		else if (Input.GetKeyDown(KeyCode.JoystickButton1))
		{
			gamepadRightCreateClickUseFive = false;
			gamepadRightCreateReleaseFrame = -1;
			if (TryCreateFromItem(currentCreateSelected, 1, directToInventory: true))
			{
				suppressNextGamepadRightCreateClick = true;
			}
		}
		else if (Input.GetKeyDown(KeyCode.JoystickButton2))
		{
			TryCreateFromItem(currentCreateSelected, 5, directToInventory: false);
		}
		else if (Input.GetKeyDown(KeyCode.JoystickButton3))
		{
			TryCreateFromItem(currentCreateSelected, 5, directToInventory: true);
		}
	}

	public bool ConsumeGamepadCreatePointerClick(PointerEventData.InputButton button)
	{
		switch ((int)button)
		{
		case 0:
			if (suppressNextGamepadLeftCreateClick)
			{
				suppressNextGamepadLeftCreateClick = false;
				gamepadLeftCreateClickUseFive = false;
				gamepadLeftCreateReleaseFrame = -1;
				return true;
			}
			break;
		case 1:
			if (suppressNextGamepadRightCreateClick)
			{
				suppressNextGamepadRightCreateClick = false;
				gamepadRightCreateClickUseFive = false;
				gamepadRightCreateReleaseFrame = -1;
				return true;
			}
			break;
		}
		return false;
	}

	public bool ConsumeGamepadCreateBulkClick(PointerEventData.InputButton button)
	{
		switch ((int)button)
		{
		case 0:
			if (gamepadLeftCreateClickUseFive)
			{
				gamepadLeftCreateClickUseFive = false;
				gamepadLeftCreateReleaseFrame = -1;
				return true;
			}
			break;
		case 1:
			if (gamepadRightCreateClickUseFive)
			{
				gamepadRightCreateClickUseFive = false;
				gamepadRightCreateReleaseFrame = -1;
				return true;
			}
			break;
		}
		return false;
	}

	private void RefreshGamepadCreateClickSuppressions()
	{
		RefreshGamepadCreateClickState(ref suppressNextGamepadLeftCreateClick, ref gamepadLeftCreateClickUseFive, ref gamepadLeftCreateReleaseFrame, KeyCode.JoystickButton0);
		RefreshGamepadCreateClickState(ref suppressNextGamepadRightCreateClick, ref gamepadRightCreateClickUseFive, ref gamepadRightCreateReleaseFrame, KeyCode.JoystickButton1);
	}

	private void RefreshGamepadCreateClickState(ref bool suppressClick, ref bool bulkClick, ref int releaseFrame, KeyCode key)
	{
		if (!suppressClick && !bulkClick)
		{
			releaseFrame = -1;
			return;
		}
		if (Input.GetKeyUp(key))
		{
			releaseFrame = Time.frameCount;
		}
		if (releaseFrame >= 0 && Time.frameCount > releaseFrame)
		{
			suppressClick = false;
			bulkClick = false;
			releaseFrame = -1;
		}
	}

	public bool TryCreateFromItem(BaoshiItem item, int requestedCount, bool directToInventory)
	{
		if (!SingletonMonoScope<InventoryManager>.HasInstance || !item || item.baoshiData == null || item.childBaoshiData == null)
		{
			return false;
		}
		if (!baoshiSettings || baoshiSettings.needCount <= 0)
		{
			return false;
		}
		SelectItem(item);
		InventoryManager instance = SingletonMonoScope<InventoryManager>.Instance;
		BaoshiClass baoshiData = item.baoshiData;
		BaoshiClass childBaoshiData = item.childBaoshiData;
		int count = Mathf.Max(1, requestedCount);
		if (!directToInventory && !ClampCreateCountByHand(baoshiData, ref count))
		{
			return false;
		}
		int baoshiTotalCountInInv = instance.GetBaoshiTotalCountInInv(childBaoshiData.ItemName);
		count = Mathf.Min(count, baoshiTotalCountInInv / baoshiSettings.needCount);
		long createUnitPrice = GetCreateUnitPrice(baoshiData);
		if (createUnitPrice > 0)
		{
			count = (int)Math.Min(count, instance.GlobalMoney / createUnitPrice);
		}
		if (count <= 0)
		{
			if (createUnitPrice > 0 && instance.GlobalMoney < createUnitPrice)
			{
				GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			}
			return false;
		}
		int count2 = count * baoshiSettings.needCount;
		long num = createUnitPrice * count;
		if (directToInventory)
		{
			if (!instance.RemoveBaoshiCountInInv(childBaoshiData.ItemName, count2))
			{
				return false;
			}
			instance.RemoveMoney(num);
			GiveCreatedBaoshiToInventoryOrGround(baoshiData, count);
		}
		else
		{
			bool handHadItem = (bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ;
			if (!GiveCreatedBaoshiToHand(instance, baoshiData, count))
			{
				return false;
			}
			if (!instance.RemoveBaoshiCountInInv(childBaoshiData.ItemName, count2))
			{
				RollbackCreatedBaoshiInHand(baoshiData, count, handHadItem);
				RefreshNum();
				return false;
			}
			instance.RemoveMoney(num);
		}
		GameManager.ShowTip(LOC.MM.GetMainFormat("create_baoshi_success", num), TipType.Success);
		RefreshUI();
		SingletonMonoScope<GameUIManager>.Instance.Toggle_IV(show: true);
		return true;
	}

	private long GetCreateUnitPrice(BaoshiClass data)
	{
		if (data == null || !baoshiSettings)
		{
			return 0L;
		}
		return data.BS_Quality switch
		{
			7 => baoshiSettings.createPrice7, 
			6 => baoshiSettings.createPrice6, 
			5 => baoshiSettings.createPrice5, 
			4 => baoshiSettings.createPrice4, 
			3 => baoshiSettings.createPrice3, 
			2 => baoshiSettings.createPrice2, 
			1 => baoshiSettings.createPrice1, 
			_ => 0L, 
		};
	}

	private bool ClampCreateCountByHand(BaoshiClass targetData, ref int count)
	{
		if (targetData == null || !Hand.Instance)
		{
			return false;
		}
		if (!Hand.Instance.ItemOBJ)
		{
			if (targetData.MstackSize > 0)
			{
				count = Mathf.Min(count, targetData.MstackSize);
			}
			return count > 0;
		}
		if (Hand.Instance.itemType != 1 || Hand.Instance.baoshi == null)
		{
			return false;
		}
		if (!IsSameBaoshi(Hand.Instance.baoshi, targetData))
		{
			return false;
		}
		int num = Hand.Instance.baoshi.MstackSize - Hand.Instance.baoshi.CstackSize;
		if (num <= 0)
		{
			return false;
		}
		count = Mathf.Min(count, num);
		return count > 0;
	}

	private bool GiveCreatedBaoshiToHand(InventoryManager inventory, BaoshiClass targetData, int count)
	{
		if (!Hand.Instance || targetData == null || count <= 0)
		{
			return false;
		}
		if ((bool)Hand.Instance.ItemOBJ)
		{
			if (Hand.Instance.itemType != 1 || Hand.Instance.baoshi == null || !IsSameBaoshi(Hand.Instance.baoshi, targetData))
			{
				return false;
			}
			if (Hand.Instance.baoshi.CstackSize + count > Hand.Instance.baoshi.MstackSize)
			{
				return false;
			}
			Hand.Instance.baoshi.CstackSize += count;
			Hand.Instance.ItemOBJ.RefreshStackHand(0);
			return true;
		}
		if (inventory != null)
		{
			return inventory.TryCreateBaoshiToHand(targetData, count);
		}
		return false;
	}

	private static void RollbackCreatedBaoshiInHand(BaoshiClass targetData, int count, bool handHadItem)
	{
		if ((bool)Hand.Instance && (bool)Hand.Instance.ItemOBJ)
		{
			if (handHadItem && Hand.Instance.itemType == 1 && IsSameBaoshi(Hand.Instance.baoshi, targetData))
			{
				Hand.Instance.baoshi.CstackSize = Mathf.Max(0, Hand.Instance.baoshi.CstackSize - count);
				Hand.Instance.ItemOBJ.RefreshStackHand(0);
			}
			else
			{
				Hand.Instance.DELItem();
			}
		}
	}

	private static void GiveCreatedBaoshiToInventoryOrGround(BaoshiClass targetData, int count)
	{
		if (targetData == null || count <= 0)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			BaoshiClass baoshiClass = ItemCloneUtil.CloneBaoshi(targetData);
			if (baoshiClass != null)
			{
				baoshiClass.CstackSize = 1;
				if ((!SingletonMonoScope<InventoryManager>.HasInstance || !SingletonMonoScope<InventoryManager>.Instance.TryAddBaoshiToInventory(baoshiClass)) && SingletonMonoScope<ItemManager>.HasInstance)
				{
					SingletonMonoScope<ItemManager>.Instance.ThrowBS(baoshiClass);
				}
			}
		}
	}

	private static bool IsSameBaoshi(BaoshiClass a, BaoshiClass b)
	{
		if (a != null && b != null)
		{
			return a.ItemName == b.ItemName;
		}
		return false;
	}

	private static bool IsSubmitDown()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return GamepadInputManager.GetSubmitDown();
		}
		return Input.GetMouseButtonDown(0);
	}

	private void ResolveBaoShiTipReferences()
	{
		Transform transform = (baoShiTip ? baoShiTip.transform : null);
		if (!transform && (bool)base.transform.parent)
		{
			transform = base.transform.parent.Find("BaoShiTip");
		}
		if (!transform)
		{
			GameObject gameObject = GameObject.Find("Game UI/UICanvas/BaoShiTip");
			if ((bool)gameObject)
			{
				transform = gameObject.transform;
			}
		}
		if ((bool)transform)
		{
			baoShiTip = transform.gameObject;
			if (!baoShiTipCanvasGroup)
			{
				baoShiTipCanvasGroup = transform.GetComponent<CanvasGroup>();
			}
			if (!baoshiTipNameText)
			{
				baoshiTipNameText = transform.Find("BSname")?.GetComponent<Text>();
			}
			if (!priceText)
			{
				priceText = transform.Find("PriceText")?.GetComponent<Text>();
			}
			if (baoShiTipShortcutTexts == null || baoShiTipShortcutTexts.Length != 4)
			{
				baoShiTipShortcutTexts = new Text[4];
			}
			if (baoShiTipActionTexts == null || baoShiTipActionTexts.Length != 4)
			{
				baoShiTipActionTexts = new Text[4];
			}
			ResolveTipText(transform, "Main/left/main", baoShiTipShortcutTexts, 0);
			ResolveTipText(transform, "Main/left/main (1)", baoShiTipShortcutTexts, 1);
			ResolveTipText(transform, "Main/left/main (2)", baoShiTipShortcutTexts, 2);
			ResolveTipText(transform, "Main/left/main (3)", baoShiTipShortcutTexts, 3);
			ResolveTipText(transform, "Main/Right/main", baoShiTipActionTexts, 0);
			ResolveTipText(transform, "Main/Right/main (1)", baoShiTipActionTexts, 1);
			ResolveTipText(transform, "Main/Right/main (2)", baoShiTipActionTexts, 2);
			ResolveTipText(transform, "Main/Right/main (3)", baoShiTipActionTexts, 3);
		}
		if (!priceText)
		{
			priceText = base.transform.Find("MainGroup/Content/PriceBg/PriceText")?.GetComponent<Text>();
		}
	}

	private static void ResolveTipText(Transform root, string path, Text[] texts, int index)
	{
		if ((bool)root && texts != null && index >= 0 && index < texts.Length && !texts[index])
		{
			texts[index] = root.Find(path)?.GetComponent<Text>();
		}
	}

	private void ShowBaoShiTip()
	{
		if ((bool)baoShiTip && (bool)currentCreateSelected && currentCreateSelectedData != null)
		{
			PositionBaoShiTip(currentCreateSelected);
			baoShiTip.SetActive(value: true);
			SetBaoShiTipVisible(visible: true);
			RefreshBaoShiTipTexts();
		}
	}

	private void PositionBaoShiTip(BaoshiItem item)
	{
		RectTransform rectTransform = (baoShiTip ? baoShiTip.GetComponent<RectTransform>() : null);
		RectTransform rectTransform2 = (item ? item.GetComponent<RectTransform>() : null);
		if ((bool)rectTransform && (bool)rectTransform2)
		{
			rectTransform.pivot = new Vector2(0f, 0.5f);
			Canvas componentInParent = rectTransform.GetComponentInParent<Canvas>();
			Camera cam = (((bool)componentInParent && componentInParent.renderMode != 0) ? componentInParent.worldCamera : null);
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(cam, rectTransform2.TransformPoint(rectTransform2.rect.center));
			vector.x += 100f;
			RectTransform rectTransform3 = rectTransform.parent as RectTransform;
			if ((bool)rectTransform3 && RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform3, vector, cam, out var localPoint))
			{
				rectTransform.anchoredPosition = localPoint;
			}
			else
			{
				rectTransform.position = vector;
			}
		}
	}

	private void HideBaoShiTip()
	{
		SetBaoShiTipVisible(visible: false);
		if ((bool)baoShiTip)
		{
			baoShiTip.SetActive(value: false);
		}
	}

	private void SetBaoShiTipVisible(bool visible)
	{
		if (!baoShiTipCanvasGroup && (bool)baoShiTip)
		{
			baoShiTipCanvasGroup = baoShiTip.GetComponent<CanvasGroup>();
		}
		if ((bool)baoShiTipCanvasGroup)
		{
			baoShiTipCanvasGroup.alpha = (visible ? 1f : 0f);
			baoShiTipCanvasGroup.interactable = false;
			baoShiTipCanvasGroup.blocksRaycasts = false;
		}
	}

	private void RefreshBaoShiTipTexts()
	{
		RefreshBaoShiTipShortcutTexts();
		RefreshBaoShiTipActionTexts();
		RefreshPriceText();
	}

	private void RefreshBaoShiTipShortcutTexts()
	{
		if (baoShiTipShortcutTexts == null || baoShiTipShortcutTexts.Length < 4)
		{
			return;
		}
		string[] array = ((SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent()) ? new string[4]
		{
			KeyDisplayUtil.ToDisplayName("Pad_A"),
			KeyDisplayUtil.ToDisplayName("Pad_X"),
			KeyDisplayUtil.ToDisplayName("Pad_B"),
			KeyDisplayUtil.ToDisplayName("Pad_Y")
		} : new string[4]
		{
			KeyDisplayUtil.ToDisplayName("Mouse0"),
			FormatShortcut("LeftShift", "Mouse0"),
			KeyDisplayUtil.ToDisplayName("Mouse1"),
			FormatShortcut("LeftShift", "Mouse1")
		});
		for (int i = 0; i < baoShiTipShortcutTexts.Length && i < array.Length; i++)
		{
			if ((bool)baoShiTipShortcutTexts[i])
			{
				baoShiTipShortcutTexts[i].text = array[i];
			}
		}
	}

	private void RefreshBaoShiTipActionTexts()
	{
		if (baoShiTipActionTexts == null || baoShiTipActionTexts.Length < 4)
		{
			return;
		}
		for (int i = 0; i < baoShiTipActionTexts.Length && i < BaoShiTipActionKeys.Length; i++)
		{
			if ((bool)baoShiTipActionTexts[i])
			{
				baoShiTipActionTexts[i].text = LOC.MM.GetMain(BaoShiTipActionKeys[i]);
			}
		}
	}

	private static string FormatShortcut(string modifierRaw, string keyRaw)
	{
		string text = KeyDisplayUtil.ToDisplayName(modifierRaw);
		string text2 = KeyDisplayUtil.ToDisplayName(keyRaw);
		if (string.IsNullOrEmpty(text))
		{
			return text2;
		}
		if (string.IsNullOrEmpty(text2))
		{
			return text;
		}
		return text + "+" + text2;
	}

	private void OnCurrentInputDeviceChanged(InputDeviceType deviceType)
	{
		RefreshBaoShiTipShortcutTexts();
	}

	private void OnLanguageChanged(LanguageType language)
	{
		RefreshBaoShiTipTexts();
		RefreshSplitButtonTexts();
	}

	private bool IsValidForgeSlot(SlotData slot)
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
		WeaponClass weapon = slot.weapon;
		if (weapon == null)
		{
			return false;
		}
		int skillIndex;
		WPSkill skill;
		int spcIndex;
		WPSPC spc;
		return currentSplitType switch
		{
			SplitTargetType.Gem => GetLastSocketedGemIndex(weapon) >= 0, 
			SplitTargetType.TalentSkillRune => TryGetLastTalentSkillRune(weapon, out skillIndex, out skill), 
			SplitTargetType.EquipmentSkillRune => TryGetLastEquipmentSkillRune(weapon, out spcIndex, out spc), 
			SplitTargetType.EquipmentAttributeRune => HasEquipmentAttributeRune(weapon), 
			_ => false, 
		};
	}

	private static int GetLastSocketedGemIndex(WeaponClass weapon)
	{
		if (weapon?.Aocao == null || weapon.AocaoCount <= 0)
		{
			return -1;
		}
		for (int num = Mathf.Min(weapon.AocaoCount, weapon.Aocao.Count) - 1; num >= 0; num--)
		{
			WPAocao wPAocao = weapon.Aocao[num];
			if (wPAocao != null && wPAocao.HasAocao && wPAocao.HasBaoshi && !string.IsNullOrEmpty(wPAocao.Name))
			{
				return num;
			}
		}
		return -1;
	}

	private static bool TryGetLastTalentSkillRune(WeaponClass weapon, out int skillIndex, out WPSkill skill)
	{
		skillIndex = -1;
		skill = null;
		if (weapon?.WPSK == null)
		{
			return false;
		}
		for (int num = weapon.WPSK.Count - 1; num >= 0; num--)
		{
			WPSkill wPSkill = weapon.WPSK[num];
			if (wPSkill != null && wPSkill.Number2 > 0 && !string.IsNullOrEmpty(wPSkill.IndexName) && !(wPSkill.IndexName == "0"))
			{
				skillIndex = num;
				skill = wPSkill;
				return true;
			}
		}
		return false;
	}

	private static int GetSkillRuneUnitPrice(WPSkill skill)
	{
		if (skill == null)
		{
			return 0;
		}
		if (skill.Number2 <= 1)
		{
			return Mathf.Max(0, skill.price);
		}
		return Mathf.Max(0, Mathf.RoundToInt((float)skill.price / (float)skill.Number2));
	}

	private static bool TryGetLastEquipmentSkillRune(WeaponClass weapon, out int spcIndex, out WPSPC spc)
	{
		spcIndex = -1;
		spc = null;
		if (weapon?.SPC == null)
		{
			return false;
		}
		for (int num = weapon.SPC.Count - 1; num >= 1; num--)
		{
			WPSPC wPSPC = weapon.SPC[num];
			if (wPSPC != null && wPSPC.Index > 0)
			{
				spcIndex = num;
				spc = wPSPC;
				return true;
			}
		}
		return false;
	}

	private static bool HasEquipmentAttributeRune(WeaponClass weapon)
	{
		if (weapon?.FW_Base != null)
		{
			if (string.IsNullOrEmpty(weapon.FW_Base.FWname))
			{
				return !string.IsNullOrEmpty(weapon.FW_Base.type);
			}
			return true;
		}
		return false;
	}

	private static int GetEquipmentAttributeRuneType(WeaponClass weapon)
	{
		if (weapon == null)
		{
			return 0;
		}
		if ((weapon.CharType >= 2 && weapon.CharType <= 5) || weapon.WeaponType == "head" || weapon.WeaponType == "body" || weapon.WeaponType == "hand" || weapon.WeaponType == "leg")
		{
			return 1;
		}
		if ((weapon.CharType >= 6 && weapon.CharType <= 9) || weapon.WeaponType == "little")
		{
			return 2;
		}
		return 0;
	}

	private static void NormalizeWeaponSkillCount(WeaponClass weapon)
	{
		if (weapon?.WPSK == null)
		{
			return;
		}
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < weapon.WPSK.Count; i++)
		{
			WPSkill wPSkill = weapon.WPSK[i];
			if (wPSkill != null)
			{
				if (!string.IsNullOrEmpty(wPSkill.IndexName) && wPSkill.IndexName != "0" && (wPSkill.Number != 0 || wPSkill.Number2 != 0))
				{
					num = i;
				}
				if (wPSkill.Number2 > 0)
				{
					num2 += wPSkill.Number2;
				}
			}
		}
		weapon.WP_SkillCount = num + 1;
		weapon.SKCount = Mathf.Max(0, num2);
	}

	private static BaoshiClass BuildRuntimeBaoshiFromSocket(WPAocao socket)
	{
		if (socket == null || !socket.HasBaoshi || string.IsNullOrEmpty(socket.Name))
		{
			return null;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance)
		{
			return null;
		}
		SingletonMonoScope<ItemManager>.Instance.TryGetBaoshiByItemName(socket.Name, out var data);
		if (data == null)
		{
			return null;
		}
		BaoshiClass baoshiClass = ItemCloneUtil.CloneBaoshi(data);
		if (baoshiClass == null)
		{
			return null;
		}
		baoshiClass.Number = (int)socket.Number;
		if (socket.UseType != 0)
		{
			baoshiClass.UseType = socket.UseType;
		}
		if (socket.BS_Quality > 0)
		{
			baoshiClass.BS_Quality = socket.BS_Quality;
		}
		baoshiClass.Icon = (socket.Icon ? socket.Icon : baoshiClass.Icon);
		baoshiClass.CstackSize = 1;
		return baoshiClass;
	}
}
