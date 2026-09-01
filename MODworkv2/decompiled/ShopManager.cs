using System.Collections.Generic;
using Container.Inventory;
using Container.Managers;
using Container.Util;
using Core;
using Entity.InteractableObjects.Item;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Lean.Pool;
using UI.Panels;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : ContainerManager<ShopManager>
{
	[Header("商店刷新 - 开关与次数")]
	[Tooltip("商店刷新次数上限。达到上限后不能继续主动刷新。")]
	[SerializeField]
	private int refreshCountLimit = 20;

	[Header("商店刷新 - 冷却时间")]
	[Tooltip("关闭商店后，经过多少秒才会重新允许下次打开商店时自动刷新商品。")]
	[SerializeField]
	private float refreshUnlockCooldown = 60f;

	[Tooltip("刷新次数自然恢复的时间间隔（秒）。每到一次时间，当前刷新次数减少 1，最低降到 1。")]
	[SerializeField]
	private float refreshCountRecoverInterval = 2f;

	[Header("商店刷新 - 价格公式")]
	[Tooltip("刷新商店的基础价格。")]
	[SerializeField]
	private long refreshPriceBase = 500L;

	[Tooltip("每增加 1 次刷新次数时，刷新价格乘上的成长系数。")]
	[SerializeField]
	private float refreshPriceCountMultiplier = 1.2f;

	[Tooltip("玩家每提升 1 级时，刷新价格乘上的成长系数。")]
	[SerializeField]
	private float refreshPriceLevelMultiplier = 1.065f;

	[Header("商店刷新 - 品质概率公式")]
	[Tooltip("高品质物品/药剂的基础出现率。")]
	[SerializeField]
	private float premiumDropRateBase = 10f;

	[Tooltip("每增加 1 次刷新次数时，高品质物品出现率乘上的成长系数。")]
	[SerializeField]
	private float premiumDropRateRefreshMultiplier = 1.1f;

	[Tooltip("玩家最终掉宝率对商店高品质出现率的换算比例。最终加成为 ItemDrop_Rate_Last / 该值。")]
	[SerializeField]
	private float playerDropRateToShopDropDivisor = 5f;

	[Header("商店刷新 - 调试只读")]
	[Tooltip("当前计算出的刷新价格。")]
	[SerializeField]
	private long debugCurrentRefreshPrice;

	[Tooltip("当前计算出的高品质出现率。")]
	[SerializeField]
	private float debugCurrentPremiumDropRate;

	[Tooltip("当前累计刷新次数。数值越高，刷新价格越贵，同时高品质物品出现率越高。")]
	[SerializeField]
	private int currentRefreshCount = 1;

	private Text shopPageTypeText;

	[HideInInspector]
	public bool Opened;

	private float refreshUnlockTimer;

	private float refreshCountRecoverTimer;

	public Text Rtext;

	public Text rePriceText;

	private bool canRefreshOnOpen = true;

	protected override ContainerType ContainerType => ContainerType.Shop;

	public override int ContainerMpos => 2;

	public long CurrentRefreshPrice => (long)((float)refreshPriceBase * Mathf.Pow(refreshPriceCountMultiplier, currentRefreshCount) * Mathf.Pow(refreshPriceLevelMultiplier, SingletonMonoScope<PlayerManager>.Instance.Level));

	public float CurrentPremiumDropRate => premiumDropRateBase * Mathf.Pow(premiumDropRateRefreshMultiplier, currentRefreshCount) + SingletonMonoScope<PlayerManager>.Instance.ItemDrop_Rate_Last / playerDropRateToShopDropDivisor;

	protected override void Awake()
	{
		base.Awake();
		Rtext = base.transform.Find("Refresh/Refresh/Text").GetComponent<Text>();
		rePriceText = base.transform.Find("Refresh/price/Text").GetComponent<Text>();
		shopPageTypeText = base.transform.Find("page/ShopText").GetComponent<Text>();
		pageText.gameObject.SetActive(value: false);
		closeBtn.onClick.AddListener(CloseShop);
		ShopStart();
		CreateSlots(ContainerType.Shop);
		CreatePage();
		CreatePage();
	}

	private void Start()
	{
		Rtext.text = LOC.MM.GetMain("Refresh Item");
	}

	public void ShopStart()
	{
		PageNumber = 0;
		CurPage = 1;
		Opened = false;
		canRefreshOnOpen = true;
		currentRefreshCount = 1;
		refreshUnlockTimer = 0f;
		refreshCountRecoverTimer = 0f;
		RefreshShopPageTypeText();
	}

	private void Update()
	{
		if (ContainerManager<ShopManager>.GetCursorLeftDown() && IsMouseOnShopSlot())
		{
			if (HasItemInHand())
			{
				TrySellHandItemToShop();
			}
			else
			{
				TryBuyToHand();
			}
		}
		if (ContainerManager<ShopManager>.GetCursorRightDown() && IsMouseOnShopSlot() && !HasItemInHand())
		{
			TryQuickBuyToInventory();
		}
		UpdateRefreshUnlockCooldown();
		UpdateRefreshCountRecovery();
	}

	private void LateUpdate()
	{
		debugCurrentRefreshPrice = CurrentRefreshPrice;
		debugCurrentPremiumDropRate = CurrentPremiumDropRate;
	}

	private void UpdateRefreshUnlockCooldown()
	{
		if (!Opened && !canRefreshOnOpen)
		{
			refreshUnlockTimer += Time.deltaTime;
			if (refreshUnlockTimer >= refreshUnlockCooldown)
			{
				canRefreshOnOpen = true;
				refreshUnlockTimer = 0f;
			}
		}
	}

	private void UpdateRefreshCountRecovery()
	{
		if (currentRefreshCount > 1)
		{
			refreshCountRecoverTimer += Time.deltaTime;
			if (refreshCountRecoverTimer >= refreshCountRecoverInterval)
			{
				currentRefreshCount--;
				refreshCountRecoverTimer = 0f;
				RefreshPriceText();
			}
		}
	}

	public void ClearPage()
	{
		ClearPagesRange(1, MainPages.Count - 1);
	}

	public void ClearShop()
	{
		ClearPagesRange(0, 0);
	}

	public void OpenShop()
	{
		pageText.gameObject.SetActive(value: false);
		SingletonMonoScope<GameUIManager>.Instance.Opened_shop = true;
		Opened = true;
		Rtext.text = LOC.MM.GetMain("Refresh Item");
		rePriceText.text = $"{CurrentRefreshPrice}";
		cav.blocksRaycasts = true;
		cav.alpha = 1f;
		SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 1f;
		SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = true;
		while (CurPage > 1)
		{
			ChangePage(left: true);
		}
		if (canRefreshOnOpen)
		{
			ClearShop();
			SingletonMonoScope<ItemManager>.Instance.CreatShop();
			SortBuy();
		}
		RefreshPriceText();
		pageText.text = $"{CurPage}/{PageNumber}";
		RefreshShopPageTypeText();
	}

	public void CloseShop()
	{
		pageText.gameObject.SetActive(value: false);
		canRefreshOnOpen = false;
		refreshUnlockTimer = 0f;
		SingletonMonoScope<GameUIManager>.Instance.Opened_shop = false;
		Opened = false;
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			SingletonMonoScope<InputManager>.Instance.ResetSellAllQualityShortcut();
		}
		cav.blocksRaycasts = false;
		cav.alpha = 0f;
		SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 0f;
		SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = false;
		SingletonMonoScope<GameUIManager>.Instance.Opened_IV = false;
		ClearPage();
	}

	public void SortBuy()
	{
		if (MainPages == null || MainPages.Count == 0)
		{
			return;
		}
		ClearAllBuyBuckets();
		foreach (SlotData item in new List<SlotData>(MainPages[0].MainList))
		{
			if (item != null && item.isMain)
			{
				ItemScript itemOBJ = item.ItemOBJ;
				item.ItemOBJ = null;
				switch (item.ItemType)
				{
				case 0:
					AddWeaponToSortBucket(ItemCloneUtil.CloneWeapon(item.weapon));
					break;
				case 1:
					AddBaoshiToSortBucket(ItemCloneUtil.CloneBaoshi(item.baoshi));
					break;
				case 2:
					AddUseItemToSortBucket(ItemCloneUtil.CloneUseItem(item.useitem));
					break;
				}
				if ((bool)itemOBJ)
				{
					LeanPool.Despawn(itemOBJ);
				}
			}
		}
		SortAllBuyBuckets();
		ClearShop();
		foreach (WeaponClass item2 in ContainerManager<ShopManager>.AllWP.BxD)
		{
			TryPlaceSortedWeapon(item2);
		}
		foreach (WeaponClass item3 in ContainerManager<ShopManager>.AllWP.BxC)
		{
			TryPlaceSortedWeapon(item3);
		}
		foreach (WeaponClass item4 in ContainerManager<ShopManager>.AllWP.BxB)
		{
			TryPlaceSortedWeapon(item4);
		}
		foreach (UseItemClass item5 in ContainerManager<ShopManager>.AllUSE.BxB)
		{
			TryPlaceSortedUseItem(item5);
		}
		foreach (WeaponClass item6 in ContainerManager<ShopManager>.AllWP.AxD)
		{
			TryPlaceSortedWeapon(item6);
		}
		foreach (WeaponClass item7 in ContainerManager<ShopManager>.AllWP.AxC)
		{
			TryPlaceSortedWeapon(item7);
		}
		foreach (WeaponClass item8 in ContainerManager<ShopManager>.AllWP.AxA)
		{
			TryPlaceSortedWeapon(item8);
		}
		foreach (BaoshiClass item9 in ContainerManager<ShopManager>.AllBS.AxA)
		{
			TryPlaceSortedBaoshi(item9);
		}
		foreach (UseItemClass item10 in ContainerManager<ShopManager>.AllUSE.AxA)
		{
			TryPlaceSortedUseItem(item10);
		}
		ClearAllBuyBuckets();
		PoeItemMod.VerifyShopStock(this);
	}

	private static void AddWeaponToSortBucket(WeaponClass wp)
	{
		if (wp != null)
		{
			if (wp.Size.x == 2 && wp.Size.y == 4)
			{
				ContainerManager<ShopManager>.AllWP.BxD.Add(wp);
			}
			else if (wp.Size.x == 2 && wp.Size.y == 3)
			{
				ContainerManager<ShopManager>.AllWP.BxC.Add(wp);
			}
			else if (wp.Size.x == 2 && wp.Size.y == 2)
			{
				ContainerManager<ShopManager>.AllWP.BxB.Add(wp);
			}
			else if (wp.Size.x == 1 && wp.Size.y == 4)
			{
				ContainerManager<ShopManager>.AllWP.AxD.Add(wp);
			}
			else if (wp.Size.x == 1 && wp.Size.y == 3)
			{
				ContainerManager<ShopManager>.AllWP.AxC.Add(wp);
			}
			else if (wp.Size.x == 1 && wp.Size.y == 1)
			{
				ContainerManager<ShopManager>.AllWP.AxA.Add(wp);
			}
		}
	}

	private static void AddBaoshiToSortBucket(BaoshiClass bs)
	{
		if (bs != null)
		{
			ContainerManager<ShopManager>.AllBS.AxA.Add(bs);
		}
	}

	private static void AddUseItemToSortBucket(UseItemClass use)
	{
		if (use != null)
		{
			if (use.Size.x == 2 && use.Size.y == 2)
			{
				ContainerManager<ShopManager>.AllUSE.BxB.Add(use);
			}
			else if (use.Size.x == 1 && use.Size.y == 1)
			{
				ContainerManager<ShopManager>.AllUSE.AxA.Add(use);
			}
		}
	}

	private void TryPlaceSortedWeapon(WeaponClass wp)
	{
		if (wp == null)
		{
			return;
		}
		SlotData slotData = CheckEmptyBuy(wp.Size);
		if (slotData == null)
		{
			return;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (!itemScript)
			{
				return;
			}
		}
		SlotData slotData2 = PrepareWeaponMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], wp, itemScript);
		if (slotData2 != null)
		{
			ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
			if ((bool)itemScript)
			{
				BindItemObjToRegion(slotData.Page, slotData.GridPos, wp.Size, itemScript);
				itemScript.SetWP(slotData2.weapon);
			}
		}
	}

	private void TryPlaceSortedBaoshi(BaoshiClass bs)
	{
		if (bs == null)
		{
			return;
		}
		SlotData slotData = CheckEmptyBuy(bs.Size);
		if (slotData == null)
		{
			return;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (!itemScript)
			{
				return;
			}
		}
		SlotData slotData2 = PrepareBaoshiMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], bs, itemScript);
		if (slotData2 != null)
		{
			ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
			if (itemScript != null)
			{
				BindItemObjToRegion(slotData.Page, slotData.GridPos, bs.Size, itemScript);
				itemScript.SetBS(slotData2.baoshi, 2);
			}
		}
	}

	private void TryPlaceSortedUseItem(UseItemClass use)
	{
		if (use == null)
		{
			return;
		}
		SlotData slotData = CheckEmptyBuy(use.Size);
		if (slotData == null)
		{
			return;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (itemScript == null)
			{
				return;
			}
		}
		SlotData slotData2 = PrepareUseItemMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], use, itemScript);
		if (slotData2 != null)
		{
			ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
			if ((bool)itemScript)
			{
				BindItemObjToRegion(slotData.Page, slotData.GridPos, use.Size, itemScript);
				itemScript.SetUse(slotData2.useitem, 2);
			}
		}
	}

	private static void SortAllBuyBuckets()
	{
		ContainerManager<ShopManager>.AllWP.BxD.Sort((WeaponClass a, WeaponClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllWP.BxC.Sort((WeaponClass a, WeaponClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllWP.BxB.Sort((WeaponClass a, WeaponClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllWP.AxD.Sort((WeaponClass a, WeaponClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllWP.AxC.Sort((WeaponClass a, WeaponClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllWP.AxA.Sort((WeaponClass a, WeaponClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllBS.AxA.Sort((BaoshiClass a, BaoshiClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllUSE.BxB.Sort((UseItemClass a, UseItemClass b) => b.Price.CompareTo(a.Price));
		ContainerManager<ShopManager>.AllUSE.AxA.Sort((UseItemClass a, UseItemClass b) => b.Price.CompareTo(a.Price));
	}

	private static void ClearAllBuyBuckets()
	{
		ContainerManager<ShopManager>.AllWP.BxD.Clear();
		ContainerManager<ShopManager>.AllWP.BxC.Clear();
		ContainerManager<ShopManager>.AllWP.BxB.Clear();
		ContainerManager<ShopManager>.AllWP.AxD.Clear();
		ContainerManager<ShopManager>.AllWP.AxC.Clear();
		ContainerManager<ShopManager>.AllWP.AxA.Clear();
		ContainerManager<ShopManager>.AllBS.AxA.Clear();
		ContainerManager<ShopManager>.AllUSE.BxB.Clear();
		ContainerManager<ShopManager>.AllUSE.AxA.Clear();
	}

	public SlotData CheckEmptySell(IntVector2 itemSizeL)
	{
		for (int i = 1; i < Page.Count && i < EPPages.Count; i++)
		{
			SlotData slotData = ContainerGridUtil.FindEmptyInPage(Page[i], EPPages[i], itemSizeL, inventorySize);
			if (slotData != null)
			{
				return slotData;
			}
		}
		return null;
	}

	public SlotData CheckEmptyBuy(IntVector2 itemSize)
	{
		if (Page.Count <= 0 || EPPages.Count <= 0)
		{
			return null;
		}
		return ContainerGridUtil.FindEmptyInPage(Page[0], EPPages[0], itemSize, inventorySize);
	}

	public override void ChangePage(bool left)
	{
		base.ChangePage(left);
		RefreshShopPageTypeText();
	}

	public void RefreshShop()
	{
		long currentRefreshPrice = CurrentRefreshPrice;
		if (currentRefreshCount >= refreshCountLimit)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Money_Null_3);
			GameManager.ShowTipLocalStartKey("shop_refresh_limit", TipType.Fail);
			RefreshPriceText();
			return;
		}
		if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < currentRefreshPrice)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Money_Null_3);
			GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			RefreshPriceText();
			return;
		}
		RuntimeManager.PlayOneShot(_audioManager.audioData.Store_Refresh);
		while (CurPage > 1)
		{
			ChangePage(left: true);
		}
		ClearShop();
		SingletonMonoScope<ItemManager>.Instance.CreatShop();
		SortBuy();
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			GameManager.ShowTip(LOC.MM.GetMainFormat("refresh_shop_success", currentRefreshPrice));
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(currentRefreshPrice, add: false);
		}
		currentRefreshCount++;
		refreshCountRecoverTimer = 0f;
		RefreshPriceText();
	}

	public void RefreshPriceText()
	{
		rePriceText.text = $"{CurrentRefreshPrice}";
	}

	private void RefreshShopPageTypeText()
	{
		if ((bool)shopPageTypeText)
		{
			if (CurPage <= 1)
			{
				shopPageTypeText.text = LOC.MM.GetMain("shop_page_buy");
			}
			else
			{
				shopPageTypeText.text = LOC.MM.GetMain("shop_page_buyback");
			}
		}
	}

	private void TryBuyToHand()
	{
		if (CanBuyCurrentMouseItem())
		{
			if (!HasEnoughMoneyForCurrentMouseItem())
			{
				GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			}
			else
			{
				BuyItem();
			}
		}
	}

	private void TryQuickBuyToInventory()
	{
		if (!CanBuyCurrentMouseItem())
		{
			return;
		}
		if (!HasEnoughMoneyForCurrentMouseItem())
		{
			GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
			return;
		}
		bool flag = false;
		long num = 0L;
		switch (base.MouseSlotDT.ItemType)
		{
		case 0:
			if (SingletonMonoScope<InventoryManager>.Instance.CheckEmpty(base.MouseSlotDT.ItemSize) != null)
			{
				num = base.MouseSlotDT.weapon.ByPrice;
				SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(base.MouseSlotDT.weapon.ByPrice, add: false);
				SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(base.MouseSlotDT.weapon);
				flag = true;
			}
			else
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			}
			break;
		case 1:
			if (SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(base.MouseSlotDT.baoshi, base.MouseSlotDT).Success)
			{
				num = base.MouseSlotDT.baoshi.ByPrice;
				SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(num, add: false);
				flag = true;
			}
			else
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			}
			break;
		case 2:
			if (SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(base.MouseSlotDT.useitem, base.MouseSlotDT).Success)
			{
				num = base.MouseSlotDT.useitem.ByPrice;
				SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(num, add: false);
				flag = true;
				if (SingletonMonoScope<ACTbar>.HasInstance)
				{
					SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(base.MouseSlotDT.useitem);
					SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
				}
			}
			else
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			}
			break;
		}
		if (flag)
		{
			ThrowItem();
			GameManager.ShowTip(LOC.MM.GetMainFormat("shop_buy_success", num));
			RefreshColor(enter: false);
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
		}
	}

	public void BuyItem()
	{
		SlotData slotData = Page[CurPage - 1].DT[MouseSlot.GridPos.x, MouseSlot.GridPos.y];
		if (slotData == null || !slotData.isOC)
		{
			return;
		}
		long num = 0L;
		switch (slotData.ItemType)
		{
		case 0:
			num = slotData.weapon?.ByPrice ?? 0;
			break;
		case 1:
			num = slotData.baoshi?.ByPrice ?? 0;
			break;
		case 2:
			num = slotData.useitem?.ByPrice ?? 0;
			break;
		}
		if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney >= num)
		{
			Hand.Instance.TakeItem(slotData);
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(num, add: false);
			ContainerSlotUtil.ColorChange(SlotColor.TouMing, slotData.ItemSize, slotData.StartPos, slotGrid);
			ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages);
			GameManager.ShowTip(LOC.MM.GetMainFormat("shop_buy_success", num));
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
			RefreshColor(enter: true);
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
		}
	}

	private void TrySellHandItemToShop()
	{
		if (IsMouseOnShopSlot() && HasItemInHand())
		{
			switch (Hand.Instance.itemType)
			{
			default:
				return;
			case 0:
				TrySellToShop(Hand.Instance.weapon);
				break;
			case 1:
				TrySellToShop(Hand.Instance.baoshi);
				break;
			case 2:
				TrySellToShop(Hand.Instance.useitem);
				break;
			}
			Hand.Instance.DELItem();
			Sector.sec.SetPosOffset();
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
			RefreshColor(enter: false);
		}
	}

	public bool TrySellToShop(WeaponClass wp, bool playFeedback = true)
	{
		if (wp == null)
		{
			return false;
		}
		while (!TryPlaceSoldWeapon(wp))
		{
			CreatePage();
		}
		if (playFeedback)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Money);
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(wp.Price, add: true);
			GameManager.ShowTip(LOC.MM.GetMainFormat("shop_sell_success", wp.Price));
		}
		else
		{
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(wp.Price, add: true);
		}
		return true;
	}

	public bool TrySellToShop(BaoshiClass bs, bool playFeedback = true)
	{
		if (bs == null)
		{
			return false;
		}
		while (!TryPlaceSoldBaoshi(bs))
		{
			CreatePage();
		}
		if (playFeedback)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Money);
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(bs.MaxPrice, add: true);
			GameManager.ShowTip(LOC.MM.GetMainFormat("shop_sell_success", bs.MaxPrice));
		}
		else
		{
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(bs.MaxPrice, add: true);
		}
		return true;
	}

	public bool TrySellToShop(UseItemClass use, bool playFeedback = true)
	{
		if (use == null)
		{
			return false;
		}
		while (!TryPlaceSoldUseItem(use))
		{
			CreatePage();
		}
		if (playFeedback)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Money);
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(use.MaxPrice, add: true);
			GameManager.ShowTip(LOC.MM.GetMainFormat("shop_sell_success", use.MaxPrice));
		}
		else
		{
			SingletonMonoScope<InventoryManager>.Instance.ChangeMoney(use.MaxPrice, add: true);
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(use);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
		return true;
	}

	private bool TryPlaceSoldWeapon(WeaponClass wp)
	{
		if (wp == null)
		{
			return false;
		}
		SlotData slotData = CheckEmptySell(wp.Size);
		if (slotData == null)
		{
			return false;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (!itemScript)
			{
				return false;
			}
		}
		SlotData slotData2 = PrepareWeaponMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], wp, itemScript);
		if (slotData2 == null)
		{
			return false;
		}
		ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
		if ((bool)itemScript)
		{
			BindItemObjToRegion(slotData.Page, slotData.GridPos, wp.Size, itemScript);
			itemScript.SetWP(slotData2.weapon);
		}
		return true;
	}

	private bool TryPlaceSoldBaoshi(BaoshiClass bs)
	{
		if (bs == null)
		{
			return false;
		}
		SlotData slotData = CheckEmptySell(bs.Size);
		if (slotData == null)
		{
			return false;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (!itemScript)
			{
				return false;
			}
		}
		SlotData slotData2 = PrepareBaoshiMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], bs, itemScript);
		if (slotData2 == null)
		{
			return false;
		}
		ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
		if ((bool)itemScript)
		{
			BindItemObjToRegion(slotData.Page, slotData.GridPos, bs.Size, itemScript);
			itemScript.SetBS(slotData2.baoshi, 2);
		}
		return true;
	}

	private bool TryPlaceSoldUseItem(UseItemClass use)
	{
		if (use == null)
		{
			return false;
		}
		SlotData slotData = CheckEmptySell(use.Size);
		if (slotData == null)
		{
			return false;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (!itemScript)
			{
				return false;
			}
		}
		SlotData slotData2 = PrepareUseItemMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], use, itemScript);
		if (slotData2 == null)
		{
			return false;
		}
		ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
		if ((bool)itemScript)
		{
			BindItemObjToRegion(slotData.Page, slotData.GridPos, use.Size, itemScript);
			itemScript.SetUse(slotData2.useitem, 2);
		}
		return true;
	}

	public void CreatWP(SlotData slot)
	{
		if (slot?.weapon == null)
		{
			return;
		}
		WeaponClass weaponClass = ItemCloneUtil.CloneWeapon(slot.weapon);
		if (weaponClass == null)
		{
			return;
		}
		SlotData slotData = ResolvePreparedBuySlot(slot, weaponClass.Size);
		if (slotData == null)
		{
			return;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (!itemScript)
			{
				return;
			}
		}
		SlotData slotData2 = PrepareWeaponMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], weaponClass, itemScript);
		if (slotData2 != null)
		{
			ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
			if ((bool)itemScript)
			{
				BindItemObjToRegion(slotData.Page, slotData.GridPos, weaponClass.Size, itemScript);
				itemScript.SetWP(slotData2.weapon);
			}
		}
	}

	public void CreatUSE(SlotData slot)
	{
		if (slot?.useitem == null)
		{
			return;
		}
		UseItemClass useItemClass = ItemCloneUtil.CloneUseItem(slot.useitem);
		if (useItemClass == null)
		{
			return;
		}
		SlotData slotData = ResolvePreparedBuySlot(slot, useItemClass.Size);
		if (slotData == null)
		{
			return;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (!itemScript)
			{
				return;
			}
		}
		SlotData slotData2 = PrepareUseItemMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], useItemClass, itemScript);
		if (slotData2 != null)
		{
			ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
			if ((bool)itemScript)
			{
				BindItemObjToRegion(slotData.Page, slotData.GridPos, useItemClass.Size, itemScript);
				itemScript.SetUse(slotData2.useitem, 2);
			}
		}
	}

	public void CreatBS(SlotData slot)
	{
		if (slot?.baoshi == null)
		{
			return;
		}
		BaoshiClass baoshiClass = ItemCloneUtil.CloneBaoshi(slot.baoshi);
		if (baoshiClass == null)
		{
			return;
		}
		SlotData slotData = ResolvePreparedBuySlot(slot, baoshiClass.Size);
		if (slotData == null)
		{
			return;
		}
		bool num = slotData.Page + 1 == CurPage;
		ItemScript itemScript = null;
		if (num)
		{
			itemScript = SpawnContainerItemObj(slotData.Page, slotData.GridPos);
			if (itemScript == null)
			{
				return;
			}
		}
		SlotData slotData2 = PrepareBaoshiMainSlot(Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y], baoshiClass, itemScript);
		if (slotData2 != null)
		{
			ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
			if ((bool)itemScript)
			{
				BindItemObjToRegion(slotData.Page, slotData.GridPos, baoshiClass.Size, itemScript);
				itemScript.SetBS(slotData2.baoshi, 2);
			}
		}
	}

	private SlotData ResolvePreparedBuySlot(SlotData preparedSlot, IntVector2 itemSize)
	{
		if (preparedSlot == null)
		{
			return CheckEmptyBuy(itemSize);
		}
		int page = preparedSlot.Page;
		IntVector2 gridPos = preparedSlot.GridPos;
		if (page == 0 && page < Page.Count && page < MainPages.Count && Page[page]?.DT != null && gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x < Page[page].DT.GetLength(0) && gridPos.y < Page[page].DT.GetLength(1) && Page[page].DT[gridPos.x, gridPos.y] == preparedSlot && !MainPages[page].MainList.Contains(preparedSlot))
		{
			preparedSlot.ClearItemIndex();
			if (ContainerGridUtil.CanPlaceAt(Page[page], gridPos, itemSize, inventorySize))
			{
				return preparedSlot;
			}
		}
		return CheckEmptyBuy(itemSize);
	}

	private void ClearPagesRange(int startPageIndex, int endPageIndex)
	{
		for (int i = startPageIndex; i <= endPageIndex; i++)
		{
			if (i < 0 || i >= MainPages.Count)
			{
				continue;
			}
			foreach (SlotData item in new List<SlotData>(MainPages[i].MainList))
			{
				if (item != null && item.isMain)
				{
					ItemScript itemOBJ = item.ItemOBJ;
					ContainerGridUtil.ClearRegion(item, Page, EPPages, MainPages);
					if ((bool)itemOBJ)
					{
						LeanPool.Despawn(itemOBJ);
					}
				}
			}
		}
	}

	private bool CanBuyCurrentMouseItem()
	{
		if (IsMouseOnShopSlot() && !HasItemInHand() && base.MouseSlotDT != null)
		{
			return base.MouseSlotDT.isOC;
		}
		return false;
	}

	private static bool HasItemInHand()
	{
		return Hand.Instance.ItemOBJ;
	}

	private bool IsMouseOnShopSlot()
	{
		if ((bool)MouseSlot)
		{
			return Hand.Instance.Mpos == 2;
		}
		return false;
	}

	private long GetCurrentMouseItemBuyPrice()
	{
		if (base.MouseSlotDT == null)
		{
			return 0L;
		}
		return base.MouseSlotDT.ItemType switch
		{
			0 => base.MouseSlotDT.weapon?.ByPrice ?? 0, 
			1 => base.MouseSlotDT.baoshi?.ByPrice ?? 0, 
			2 => base.MouseSlotDT.useitem?.ByPrice ?? 0, 
			_ => 0L, 
		};
	}

	private bool HasEnoughMoneyForCurrentMouseItem()
	{
		return SingletonMonoScope<InventoryManager>.Instance.GlobalMoney >= GetCurrentMouseItemBuyPrice();
	}
}
