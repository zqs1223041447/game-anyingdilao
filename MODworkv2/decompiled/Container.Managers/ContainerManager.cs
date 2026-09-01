using System;
using System.Collections.Generic;
using Container.Inventory;
using Container.Util;
using Entity.InteractableObjects.Item;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Cursors;
using Lean.Pool;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Container.Managers;

public class ContainerManager<T> : SingletonMonoScope<T> where T : MonoBehaviour
{
	private const ProcessScope scope = ProcessScope.Game;

	public CanvasGroup cav;

	public GameObject IVgird;

	public GameObject slotPrefab;

	public IntVector2 inventorySize;

	public float slotSize;

	public Transform dropParent;

	public bool isOverEdge;

	public GameObject IVitem;

	public SlotScript MouseSlot;

	public IntVector2 totalOffset;

	public IntVector2 checkStartPos;

	public IntVector2 checkSize;

	public int checkState;

	public IntVector2 otherItemPos;

	public IntVector2 otherItemSize;

	public SlotScript[,] slotGrid;

	public List<SlotDataPage> Page = new List<SlotDataPage>();

	public int PageNumber;

	public int CurPage;

	public Text pageText;

	public List<EmptySlotPage> EPPages = new List<EmptySlotPage>();

	public List<MainSlotPage> MainPages = new List<MainSlotPage>();

	public List<ContainerItemData> ItemList = new List<ContainerItemData>();

	public List<SlotIndexPage> SlotIndexPages = new List<SlotIndexPage>();

	public List<GameObject> del = new List<GameObject>();

	protected static AllWeapon AllWP;

	protected static AllBaoshi AllBS;

	protected static AllUseitem AllUSE;

	public Button leftPage;

	public Button rightPage;

	public Button closeBtn;

	[SerializeField]
	private bool showLegacyPlacementPreviewColors;

	protected AudioManager _audioManager;

	private int gamepadShiftRightPendingUntilFrame = -1;

	protected virtual ContainerType ContainerType => ContainerType.Inventory;

	public virtual int ContainerMpos { get; protected set; }

	public SlotData MouseSlotDT
	{
		get
		{
			if (!TryGetMouseSlotData(out var slotData))
			{
				return null;
			}
			return slotData;
		}
	}

	public virtual int MaxPageCount => int.MaxValue;

	public bool CanCreatePage => PageNumber < MaxPageCount;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.RegisterToScope(this, ProcessScope.Game);
	}

	protected override void OnDestroy()
	{
		if (SingletonMonoGlobal<SessionManager>.HasInstance)
		{
			SingletonMonoGlobal<SessionManager>.Instance.UnregisterFromScope(this, ProcessScope.Game);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		InitUIReferences();
		InitRuntimeStates();
		InitManagersAndCaches();
	}

	private void InitUIReferences()
	{
		cav = GetComponent<CanvasGroup>();
		IVgird = base.transform.Find("Gird").gameObject;
		pageText = base.transform.Find("page/Text").GetComponent<Text>();
		dropParent = base.transform.Find("Drop Parent").GetComponent<Transform>();
		if (!leftPage)
		{
			leftPage = base.transform.Find("left").GetComponent<Button>();
		}
		if (!rightPage)
		{
			rightPage = base.transform.Find("right").GetComponent<Button>();
		}
		if (!closeBtn)
		{
			closeBtn = base.transform.Find("Close").GetComponent<Button>();
		}
		leftPage.onClick.AddListener(delegate
		{
			ChangePage(left: true);
		});
		rightPage.onClick.AddListener(delegate
		{
			ChangePage(left: false);
		});
	}

	private void InitRuntimeStates()
	{
		isOverEdge = false;
		slotGrid = new SlotScript[inventorySize.x, inventorySize.y];
		CurPage = 1;
		PageNumber = 0;
	}

	private void InitManagersAndCaches()
	{
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
		AllWP = new AllWeapon();
		AllBS = new AllBaoshi();
		AllUSE = new AllUseitem();
	}

	public bool TryGetMouseSlotData(out SlotData slotData)
	{
		slotData = null;
		if (!MouseSlot)
		{
			return false;
		}
		if (Page == null || Page.Count == 0)
		{
			return false;
		}
		if (CurPage <= 0 || CurPage > Page.Count)
		{
			return false;
		}
		SlotDataPage slotDataPage = Page[CurPage - 1];
		if (slotDataPage == null || slotDataPage.DT == null)
		{
			return false;
		}
		int x = MouseSlot.GridPos.x;
		int y = MouseSlot.GridPos.y;
		if (x < 0 || y < 0 || x >= slotDataPage.DT.GetLength(0) || y >= slotDataPage.DT.GetLength(1))
		{
			return false;
		}
		slotData = slotDataPage.DT[x, y];
		EnsureCurrentPageItemObj(slotData);
		return slotData != null;
	}

	protected SlotData EnsureCurrentPageItemObj(SlotData slotData)
	{
		SlotData mainSlot = ContainerGridUtil.GetMainSlot(slotData, Page);
		if (mainSlot == null)
		{
			return slotData;
		}
		int num = CurPage - 1;
		if (mainSlot.Page != num)
		{
			return slotData;
		}
		if (!mainSlot.ItemOBJ)
		{
			ItemScript itemScript = FindItemObjInRegion(mainSlot);
			if ((bool)itemScript)
			{
				mainSlot.ItemOBJ = itemScript;
				RectTransform component = itemScript.GetComponent<RectTransform>();
				itemScript.transform.SetParent(dropParent);
				if ((bool)component)
				{
					component.pivot = Vector2.up;
					component.localScale = Vector3.one;
				}
				itemScript.transform.position = slotGrid[mainSlot.GridPos.x, mainSlot.GridPos.y].transform.position;
				itemScript.page = mainSlot.Page;
				itemScript.saveSlot = mainSlot.GridPos;
				itemScript.NewPagePut(mainSlot, (int)ContainerType);
				ContainerGridUtil.BindItemObjToRegion(mainSlot.Page, mainSlot.StartPos, mainSlot.ItemSize, itemScript, Page);
			}
			else
			{
				ContainerPageViewUtil.SpawnItemUI(mainSlot, CurPage, IVitem, dropParent, slotGrid, Page, (int)ContainerType);
			}
		}
		else
		{
			ContainerGridUtil.BindItemObjToRegion(mainSlot.Page, mainSlot.StartPos, mainSlot.ItemSize, mainSlot.ItemOBJ, Page);
		}
		return ContainerGridUtil.GetMainSlot(slotData, Page) ?? slotData;
	}

	private ItemScript FindItemObjInRegion(SlotData mainSlot)
	{
		if (mainSlot == null || Page == null)
		{
			return null;
		}
		int page = mainSlot.Page;
		if (page < 0 || page >= Page.Count || Page[page]?.DT == null)
		{
			return null;
		}
		IntVector2 startPos = mainSlot.StartPos;
		IntVector2 itemSize = mainSlot.ItemSize;
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				int num = startPos.x + j;
				int num2 = startPos.y + i;
				if (num >= 0 && num2 >= 0 && num < Page[page].DT.GetLength(0) && num2 < Page[page].DT.GetLength(1))
				{
					SlotData slotData = Page[page].DT[num, num2];
					if ((bool)slotData?.ItemOBJ)
					{
						return slotData.ItemOBJ;
					}
				}
			}
		}
		return null;
	}

	protected void EnsureCurrentPageItemObjs()
	{
		int num = CurPage - 1;
		if (num < 0 || num >= MainPages.Count)
		{
			return;
		}
		MainSlotPage mainSlotPage = MainPages[num];
		if (mainSlotPage?.MainList == null || mainSlotPage.MainList.Count == 0)
		{
			return;
		}
		foreach (SlotData item in new List<SlotData>(mainSlotPage.MainList))
		{
			EnsureCurrentPageItemObj(item);
		}
	}

	protected void RebindVisibleItemObjRegions()
	{
		if (MainPages == null || Page == null)
		{
			return;
		}
		for (int i = 0; i < MainPages.Count; i++)
		{
			MainSlotPage mainSlotPage = MainPages[i];
			if (mainSlotPage?.MainList == null || mainSlotPage.MainList.Count == 0)
			{
				continue;
			}
			foreach (SlotData main in mainSlotPage.MainList)
			{
				if (main != null && main.isMain && main.isOC && (bool)main.ItemOBJ)
				{
					ContainerGridUtil.BindItemObjToRegion(main.Page, main.StartPos, main.ItemSize, main.ItemOBJ, Page);
				}
			}
		}
	}

	public bool TryGetMouseMainSlotTransform(out Transform result)
	{
		result = null;
		if (slotGrid == null)
		{
			return false;
		}
		if (!TryGetMouseSlotData(out var slotData))
		{
			return false;
		}
		IntVector2 startPos = slotData.StartPos;
		if (startPos.x < 0 || startPos.y < 0)
		{
			return false;
		}
		if (startPos.x >= slotGrid.GetLength(0) || startPos.y >= slotGrid.GetLength(1))
		{
			return false;
		}
		SlotScript slotScript = slotGrid[startPos.x, startPos.y];
		if (!slotScript)
		{
			return false;
		}
		result = slotScript.transform;
		return true;
	}

	protected void RefreshPageText()
	{
		pageText.text = $"{CurPage}/{PageNumber}";
	}

	public void CreateSlots(ContainerType type)
	{
		for (int i = 0; i < inventorySize.y; i++)
		{
			for (int j = 0; j < inventorySize.x; j++)
			{
				SlotScript component = UnityEngine.Object.Instantiate(slotPrefab).GetComponent<SlotScript>();
				component.transform.name = "slot[" + j + "," + i + "]";
				component.transform.SetParent(IVgird.transform);
				component.GetComponent<RectTransform>().localPosition = new Vector3((float)j * slotSize, (float)i * (0f - slotSize), 0f);
				component.GridPos = new IntVector2(j, i);
				component.number = j + 1 + inventorySize.x * i;
				component.type = type;
				slotGrid[j, i] = component;
			}
		}
	}

	public bool CreatePage()
	{
		if (!CanCreatePage)
		{
			return false;
		}
		SlotDataPage slotDataPage = new SlotDataPage();
		EmptySlotPage emptySlotPage = new EmptySlotPage();
		SlotIndexPage slotIndexPage = new SlotIndexPage();
		slotDataPage.DT = new SlotData[inventorySize.x, inventorySize.y];
		slotIndexPage.Indexes = new int[inventorySize.x, inventorySize.y];
		for (int i = 0; i < inventorySize.y; i++)
		{
			for (int j = 0; j < inventorySize.x; j++)
			{
				slotIndexPage.Indexes[j, i] = -1;
			}
		}
		for (int k = 0; k < inventorySize.y; k++)
		{
			for (int l = 0; l < inventorySize.x; l++)
			{
				SlotData slotData = ContainerSlotFactory.CreateSlotData(l, k, Page.Count, inventorySize, ItemList, slotIndexPage);
				emptySlotPage.EPList.Add(slotData);
				slotDataPage.DT[l, k] = slotData;
			}
		}
		PageNumber++;
		RefreshPageText();
		EPPages.Add(emptySlotPage);
		Page.Add(slotDataPage);
		SlotIndexPages.Add(slotIndexPage);
		MainSlotPage item = new MainSlotPage();
		MainPages.Add(item);
		return true;
	}

	public virtual void ChangePage(bool left)
	{
		if (left)
		{
			if (CurPage > 1)
			{
				RuntimeManager.PlayOneShot(_audioManager.audioData.IV_Change_Page);
				SetPageData(left: true);
				CurPage--;
				pageText.text = $"{CurPage}/{PageNumber}";
				EnsureCurrentPageItemObjs();
			}
		}
		else if (CurPage < PageNumber)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.IV_Change_Page);
			SetPageData(left: false);
			CurPage++;
			pageText.text = $"{CurPage}/{PageNumber}";
			EnsureCurrentPageItemObjs();
		}
	}

	public void SetPageData(bool left)
	{
		int pageIndex = CurPage - 1;
		int pageIndex2 = (left ? (CurPage - 2) : CurPage);
		ContainerPageViewUtil.HidePageItems(pageIndex, MainPages, Page, inventorySize, del);
		ContainerPageViewUtil.ShowPageItems(pageIndex2, left ? (CurPage - 1) : (CurPage + 1), (int)ContainerType, MainPages, IVitem, dropParent, slotGrid, Page);
	}

	protected SlotData FindEmptySlotPreferCurrentPage(IntVector2 size)
	{
		SlotData slotData = CheckEmptyCurInternal(size);
		if (slotData != null)
		{
			return slotData;
		}
		return CheckEmptyInternal(size);
	}

	protected SlotData CheckEmptyInternal(IntVector2 itemSize)
	{
		return ContainerGridUtil.FindEmptyAcrossPages(Page, EPPages, itemSize, inventorySize);
	}

	protected SlotData CheckEmptyCurInternal(IntVector2 itemSize)
	{
		int num = CurPage - 1;
		if (num < 0 || num >= Page.Count || num >= EPPages.Count)
		{
			return null;
		}
		return ContainerGridUtil.FindEmptyInPage(Page[num], EPPages[num], itemSize, inventorySize);
	}

	protected SlotData FindSameStackOrEmptyAcrossPages(int itemType, string itemName, IntVector2 itemSize)
	{
		foreach (SlotDataPage item in Page)
		{
			for (int i = 0; i < inventorySize.y; i++)
			{
				for (int j = 0; j < inventorySize.x; j++)
				{
					SlotData slotData = item.DT[j, i];
					if (slotData == null || !slotData.isOC || !slotData.isMain || slotData.ItemType != itemType)
					{
						continue;
					}
					switch (itemType)
					{
					case 1:
						if (slotData.baoshi != null && slotData.baoshi.ItemName == itemName && slotData.baoshi.CstackSize < slotData.baoshi.MstackSize)
						{
							return slotData;
						}
						break;
					case 2:
						if (slotData.useitem != null && slotData.useitem.ItemName == itemName && slotData.useitem.CstackSize < slotData.useitem.MstackSize)
						{
							return slotData;
						}
						break;
					}
				}
			}
		}
		return CheckEmptyInternal(itemSize);
	}

	protected SlotData FindSameStackOrEmptyInCurrentPage(int itemType, string itemName, IntVector2 itemSize, bool fallbackToAllPages = false)
	{
		int num = CurPage - 1;
		if (num < 0 || num >= Page.Count)
		{
			return null;
		}
		SlotDataPage slotDataPage = Page[num];
		for (int i = 0; i < inventorySize.y; i++)
		{
			for (int j = 0; j < inventorySize.x; j++)
			{
				SlotData slotData = slotDataPage.DT[j, i];
				if (slotData == null || !slotData.isOC || !slotData.isMain || slotData.ItemType != itemType)
				{
					continue;
				}
				switch (itemType)
				{
				case 1:
					if (slotData.baoshi != null && slotData.baoshi.ItemName == itemName && slotData.baoshi.CstackSize < slotData.baoshi.MstackSize)
					{
						return slotData;
					}
					break;
				case 2:
					if (slotData.useitem != null && slotData.useitem.ItemName == itemName && slotData.useitem.CstackSize < slotData.useitem.MstackSize)
					{
						return slotData;
					}
					break;
				}
			}
		}
		if (!fallbackToAllPages)
		{
			return CheckEmptyCurInternal(itemSize);
		}
		return CheckEmptyInternal(itemSize);
	}

	protected ItemScript SpawnContainerItemObj(int page, IntVector2 startPos)
	{
		GameObject gameObject = LeanPool.Spawn(IVitem);
		if (!gameObject)
		{
			return null;
		}
		ItemScript component = gameObject.GetComponent<ItemScript>();
		if (!component)
		{
			LeanPool.Despawn(gameObject);
			return null;
		}
		component.transform.SetParent(dropParent);
		RectTransform component2 = component.GetComponent<RectTransform>();
		if ((bool)component2)
		{
			component2.pivot = Vector2.up;
			component2.localScale = Vector3.one;
		}
		component.transform.position = slotGrid[startPos.x, startPos.y].transform.position;
		component.page = page;
		component.saveSlot = startPos;
		return component;
	}

	protected ItemScript SpawnLooseItemObj()
	{
		GameObject gameObject = LeanPool.Spawn(IVitem);
		if (!gameObject)
		{
			return null;
		}
		ItemScript component = gameObject.GetComponent<ItemScript>();
		if (!component)
		{
			LeanPool.Despawn(gameObject);
			return null;
		}
		component.transform.SetParent(dropParent);
		RectTransform component2 = component.GetComponent<RectTransform>();
		if ((bool)component2)
		{
			component2.pivot = Vector2.up;
			component2.localScale = Vector3.one;
		}
		return component;
	}

	protected void BindItemObjToRegion(int page, IntVector2 startPos, IntVector2 size, ItemScript itemObj)
	{
		ContainerGridUtil.BindItemObjToRegion(page, startPos, size, itemObj, Page);
	}

	protected SlotData PrepareWeaponMainSlot(SlotData targetSlot, WeaponClass wp, ItemScript itemObj)
	{
		if (targetSlot == null || wp == null)
		{
			return null;
		}
		targetSlot.StartPos = targetSlot.GridPos;
		targetSlot.ItemType = wp.ItemType;
		targetSlot.ItemSize = wp.Size;
		targetSlot.weapon = new WeaponClass();
		targetSlot.baoshi = new BaoshiClass();
		targetSlot.useitem = new UseItemClass();
		ItemCloneUtil.CopyWeaponTo(targetSlot.weapon, wp);
		targetSlot.ItemOBJ = itemObj;
		return targetSlot;
	}

	protected SlotData PrepareBaoshiMainSlot(SlotData targetSlot, BaoshiClass bs, ItemScript itemObj)
	{
		if (targetSlot == null || bs == null)
		{
			return null;
		}
		targetSlot.StartPos = targetSlot.GridPos;
		targetSlot.ItemType = bs.ItemType;
		targetSlot.ItemSize = bs.Size;
		targetSlot.weapon = new WeaponClass();
		targetSlot.baoshi = new BaoshiClass();
		targetSlot.useitem = new UseItemClass();
		ItemCloneUtil.CopyBaoshiTo(targetSlot.baoshi, bs);
		targetSlot.ItemOBJ = itemObj;
		return targetSlot;
	}

	protected SlotData PrepareUseItemMainSlot(SlotData targetSlot, UseItemClass use, ItemScript itemObj)
	{
		if (targetSlot == null || use == null)
		{
			return null;
		}
		targetSlot.StartPos = targetSlot.GridPos;
		targetSlot.ItemType = use.ItemType;
		targetSlot.ItemSize = use.Size;
		targetSlot.weapon = new WeaponClass();
		targetSlot.baoshi = new BaoshiClass();
		targetSlot.useitem = new UseItemClass();
		ItemCloneUtil.CopyUseItemTo(targetSlot.useitem, use);
		targetSlot.ItemOBJ = itemObj;
		return targetSlot;
	}

	protected bool TryPlaceWeapon(WeaponClass wp, bool preferCurrentPage = true, bool onlySpawnWhenVisible = true)
	{
		if (wp == null)
		{
			return false;
		}
		SlotData slotData = (preferCurrentPage ? FindEmptySlotPreferCurrentPage(wp.Size) : CheckEmptyInternal(wp.Size));
		if (slotData == null)
		{
			return false;
		}
		bool num = !onlySpawnWhenVisible || slotData.Page + 1 == CurPage;
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

	protected bool TryPlaceBaoshi(BaoshiClass bs, int uiType, bool preferCurrentPage = true, bool onlySpawnWhenVisible = true)
	{
		if (bs == null)
		{
			return false;
		}
		SlotData slotData = (preferCurrentPage ? FindEmptySlotPreferCurrentPage(bs.Size) : CheckEmptyInternal(bs.Size));
		if (slotData == null)
		{
			return false;
		}
		bool num = !onlySpawnWhenVisible || slotData.Page + 1 == CurPage;
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
			itemScript.SetBS(slotData2.baoshi, uiType);
		}
		return true;
	}

	protected bool TryPlaceUseItem(UseItemClass use, int uiType, bool preferCurrentPage = true, bool onlySpawnWhenVisible = true)
	{
		if (use == null)
		{
			return false;
		}
		SlotData slotData = (preferCurrentPage ? FindEmptySlotPreferCurrentPage(use.Size) : CheckEmptyInternal(use.Size));
		if (slotData == null)
		{
			return false;
		}
		bool num = !onlySpawnWhenVisible || slotData.Page + 1 == CurPage;
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
			itemScript.SetUse(use, uiType);
		}
		return true;
	}

	protected bool TryStackBaoshi(BaoshiClass bs, Func<BaoshiClass, SlotData> findSameFunc, Action<SlotData> onRefresh = null)
	{
		if (bs == null)
		{
			return true;
		}
		if (findSameFunc == null)
		{
			return false;
		}
		SlotData slotData;
		while (true)
		{
			slotData = findSameFunc(bs);
			if (slotData == null || !slotData.isOC)
			{
				return false;
			}
			if (slotData.baoshi == null || slotData.baoshi.ItemName != bs.ItemName)
			{
				return false;
			}
			int num = slotData.baoshi.MstackSize - slotData.baoshi.CstackSize;
			if (num <= 0)
			{
				return false;
			}
			if (bs.CstackSize <= num)
			{
				break;
			}
			slotData.baoshi.CstackSize = slotData.baoshi.MstackSize;
			bs.CstackSize -= num;
			onRefresh?.Invoke(slotData);
		}
		slotData.baoshi.CstackSize += bs.CstackSize;
		onRefresh?.Invoke(slotData);
		return true;
	}

	protected bool TryStackUseItem(UseItemClass use, Func<UseItemClass, SlotData> findSameFunc, Action<int> onStackAdded = null, Action<SlotData> onRefresh = null)
	{
		if (use == null)
		{
			return true;
		}
		if (findSameFunc == null)
		{
			return false;
		}
		SlotData slotData;
		while (true)
		{
			slotData = findSameFunc(use);
			if (slotData == null || !slotData.isOC)
			{
				return false;
			}
			if (slotData.useitem == null || slotData.useitem.ItemName != use.ItemName)
			{
				return false;
			}
			int num = slotData.useitem.MstackSize - slotData.useitem.CstackSize;
			if (num <= 0)
			{
				return false;
			}
			if (use.CstackSize <= num)
			{
				break;
			}
			onStackAdded?.Invoke(num);
			slotData.useitem.CstackSize = slotData.useitem.MstackSize;
			use.CstackSize -= num;
			onRefresh?.Invoke(slotData);
		}
		onStackAdded?.Invoke(use.CstackSize);
		slotData.useitem.CstackSize += use.CstackSize;
		onRefresh?.Invoke(slotData);
		return true;
	}

	protected void RemoveItemAt(SlotData slot, bool despawnItemObj = true)
	{
		if (slot != null && slot.isOC)
		{
			ItemScript itemOBJ = slot.ItemOBJ;
			ContainerGridUtil.ClearRegion(slot, Page, EPPages, MainPages);
			if (despawnItemObj && (bool)itemOBJ)
			{
				LeanPool.Despawn(itemOBJ);
			}
		}
	}

	public SlotData GiveItem()
	{
		if (MainPages == null || CurPage - 1 < 0 || CurPage - 1 >= MainPages.Count)
		{
			return null;
		}
		List<SlotData> mainList = MainPages[CurPage - 1].MainList;
		if (mainList == null || mainList.Count == 0)
		{
			return null;
		}
		mainList.Sort((SlotData a, SlotData b) => a.number.CompareTo(b.number));
		return mainList[0];
	}

	protected void RemoveItemsWhere(Predicate<SlotData> match, bool refreshUseBindingStack = false)
	{
		if (match == null)
		{
			return;
		}
		for (int i = 0; i < MainPages.Count; i++)
		{
			List<SlotData> list = null;
			List<SlotData> mainList = MainPages[i].MainList;
			if (mainList == null || mainList.Count == 0)
			{
				continue;
			}
			for (int j = 0; j < mainList.Count; j++)
			{
				SlotData slotData = mainList[j];
				if (slotData != null && match(slotData))
				{
					list = list ?? new List<SlotData>();
					list.Add(slotData);
				}
			}
			if (list != null && list.Count != 0)
			{
				for (int k = 0; k < list.Count; k++)
				{
					RemoveItemAt(list[k]);
				}
				EPPages[i].EPList.Sort((SlotData a, SlotData b) => a.number.CompareTo(b.number));
			}
		}
		if (refreshUseBindingStack && SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
		if ((bool)Sector.sec)
		{
			Sector.sec.SetPosOffset();
		}
	}

	public int GetUseItemTotalCountInInv(string itemName)
	{
		int num = 0;
		foreach (MainSlotPage mainPage in MainPages)
		{
			if (mainPage?.MainList == null)
			{
				continue;
			}
			foreach (SlotData main in mainPage.MainList)
			{
				if (main != null && main.isOC && main.useitem != null && main.useitem.ItemName == itemName)
				{
					num += main.useitem.CstackSize;
				}
			}
		}
		return num;
	}

	public int GetBaoshiTotalCountInInv(string itemName)
	{
		int num = 0;
		foreach (MainSlotPage mainPage in MainPages)
		{
			if (mainPage?.MainList == null)
			{
				continue;
			}
			foreach (SlotData main in mainPage.MainList)
			{
				if (main != null && main.isOC && main.baoshi != null && main.baoshi.ItemName == itemName)
				{
					num += main.baoshi.CstackSize;
				}
			}
		}
		return num;
	}

	public void RefreshColor(bool enter)
	{
		if (enter)
		{
			if ((bool)Hand.Instance && (bool)MouseSlot && Page != null && CurPage > 0 && CurPage <= Page.Count && Page[CurPage - 1] != null && Page[CurPage - 1].DT != null)
			{
				IntVector2 itemSize = Hand.Instance.itemSize;
				totalOffset = ContainerGridUtil.GetPlaceStartPos(MouseSlot.GridPos, itemSize, Sector.posOffset);
				checkStartPos = totalOffset;
				checkSize = itemSize;
				isOverEdge = ContainerGridUtil.TryClampAreaToGrid(totalOffset, itemSize, inventorySize, out checkStartPos, out checkSize);
				if (isOverEdge)
				{
					checkState = 2;
				}
				else
				{
					SlotAreaCheckResult slotAreaCheckResult = ContainerSlotUtil.CheckAreaOccupy(Page[CurPage - 1].DT, checkStartPos, checkSize);
					checkState = slotAreaCheckResult.State;
					otherItemPos = slotAreaCheckResult.OtherItemPos;
					otherItemSize = slotAreaCheckResult.OtherItemSize;
				}
				ApplyPreviewColor();
			}
		}
		else
		{
			ClearPreviewColor();
		}
	}

	private void ApplyPreviewColor()
	{
		switch (checkState)
		{
		case 0:
			ContainerSlotUtil.ColorChange(SlotColor.Green, checkSize, checkStartPos, slotGrid);
			break;
		case 1:
			ContainerSlotUtil.ColorChange(SlotColor.Yellow, otherItemSize, otherItemPos, slotGrid);
			ContainerSlotUtil.ColorChange(SlotColor.Green, checkSize, checkStartPos, slotGrid);
			break;
		case 2:
			ContainerSlotUtil.ColorChange(SlotColor.Red, checkSize, checkStartPos, slotGrid);
			break;
		}
	}

	private void ClearPreviewColor()
	{
		isOverEdge = false;
		ContainerSlotUtil.ColorChange(SlotColor.TouMing, checkSize, checkStartPos, slotGrid);
		if (checkState == 1)
		{
			ContainerSlotUtil.ColorChange(SlotColor.TouMing, otherItemSize, otherItemPos, slotGrid);
		}
	}

	protected void KeyCodeThrowItem(int containerType)
	{
		if (!GetCtrlModifier() || !GetCursorLeftDown())
		{
			return;
		}
		CursorUIManager.ConsumeCtrlModifier();
		SlotData mouseSlotDT = MouseSlotDT;
		if ((bool)MouseSlot && !Hand.Instance.ItemOBJ && mouseSlotDT != null && mouseSlotDT.isOC && Hand.Instance.Mpos == containerType)
		{
			switch (mouseSlotDT.ItemType)
			{
			case 0:
				SingletonMonoScope<ItemManager>.Instance.ThrowWP(mouseSlotDT.weapon);
				break;
			case 1:
				SingletonMonoScope<ItemManager>.Instance.ThrowBS(mouseSlotDT.baoshi);
				break;
			case 2:
				SingletonMonoScope<ItemManager>.Instance.ThrowUSE(mouseSlotDT.useitem);
				break;
			}
			ThrowItem();
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
		}
	}

	public void ThrowItem()
	{
		SlotData slotData = Page[CurPage - 1].DT[MouseSlot.GridPos.x, MouseSlot.GridPos.y];
		if (slotData != null && slotData.isOC)
		{
			ItemScript itemOBJ = slotData.ItemOBJ;
			UseItemClass useItemClass = null;
			if (ContainerType == ContainerType.Inventory && slotData.ItemType == 2 && slotData.useitem != null)
			{
				useItemClass = slotData.useitem;
			}
			ContainerSlotUtil.ColorChange(SlotColor.TouMing, slotData.ItemSize, slotData.StartPos, slotGrid);
			ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages);
			if (ContainerType == ContainerType.Inventory && useItemClass != null && SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useItemClass);
			}
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			if ((bool)itemOBJ)
			{
				LeanPool.Despawn(itemOBJ);
			}
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			RefreshColor(enter: true);
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
		}
	}

	protected bool CanOperateMouseSlotItem()
	{
		SlotData mouseSlotDT = MouseSlotDT;
		if ((bool)MouseSlot && !Hand.Instance.ItemOBJ && mouseSlotDT != null && mouseSlotDT.isOC)
		{
			return Hand.Instance.Mpos == ContainerMpos;
		}
		return false;
	}

	protected bool CanPutHandItemToMouseSlot()
	{
		if ((bool)MouseSlot && (bool)Hand.Instance.ItemOBJ && !isOverEdge)
		{
			return Hand.Instance.Mpos == ContainerMpos;
		}
		return false;
	}

	protected static bool GetCursorLeftDown()
	{
		if (SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			return SingletonMonoScope<CursorInputManager>.Instance.LeftButtonDown;
		}
		return false;
	}

	protected static bool GetCursorRightDown()
	{
		if (SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			return SingletonMonoScope<CursorInputManager>.Instance.RightButtonDown;
		}
		return false;
	}

	protected static bool GetGamepadShiftRightDown()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent() && GamepadInputManager.GetKey("Pad_LStickPress"))
		{
			return Input.GetKeyDown(KeyCode.JoystickButton1);
		}
		return false;
	}

	protected bool TryConsumeGamepadShiftRightDown()
	{
		if (GetGamepadShiftRightDown())
		{
			gamepadShiftRightPendingUntilFrame = Time.frameCount + 5;
		}
		if (gamepadShiftRightPendingUntilFrame < Time.frameCount)
		{
			return false;
		}
		if (!GetShiftModifier())
		{
			gamepadShiftRightPendingUntilFrame = -1;
			return false;
		}
		if (!TryRefreshMouseSlotFromCursor())
		{
			return false;
		}
		gamepadShiftRightPendingUntilFrame = -1;
		return true;
	}

	protected bool TryRefreshMouseSlotFromCursor()
	{
		if (!SingletonMonoScope<CursorInputManager>.HasInstance || !EventSystem.current)
		{
			return false;
		}
		SlotScript slotScript = null;
		Sector sector = null;
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i].gameObject;
			if ((bool)gameObject && gameObject.activeInHierarchy)
			{
				SlotScript componentInParent = gameObject.GetComponentInParent<SlotScript>();
				if ((bool)componentInParent && componentInParent.type == ContainerType)
				{
					slotScript = componentInParent;
					sector = gameObject.GetComponent<Sector>() ?? gameObject.GetComponentInParent<Sector>();
					break;
				}
			}
		}
		if (!slotScript)
		{
			slotScript = FindSlotAtCursorScreenPosition();
		}
		if (!slotScript)
		{
			if ((bool)Hand.Instance && Hand.Instance.Mpos == ContainerMpos)
			{
				MouseSlot = null;
				Hand.Instance.Mpos = 4;
			}
			return false;
		}
		if (!sector)
		{
			sector = slotScript.GetComponentInChildren<Sector>(includeInactive: true);
		}
		MouseSlot = slotScript;
		Hand.Instance.Mpos = ContainerMpos;
		if ((bool)sector)
		{
			Sector.sec = sector;
			Sector.sec.SetPosOffset();
		}
		return true;
	}

	public void RefreshPointerSlotStateAndTip()
	{
		HideCurrentPageSocketDisplays();
		if (!SingletonMonoScope<GameUIManager>.HasInstance || !TryRefreshMouseSlotFromCursor())
		{
			if (SingletonMonoScope<GameUIManager>.HasInstance)
			{
				SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			}
			return;
		}
		RefreshColor(enter: true);
		SlotData mouseSlotDT = MouseSlotDT;
		if (mouseSlotDT == null || !mouseSlotDT.isOC)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			return;
		}
		if ((bool)Hand.Instance.ItemOBJ && Hand.Instance.itemType != 1)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			return;
		}
		switch (mouseSlotDT.ItemType)
		{
		case 0:
			if ((bool)Hand.Instance.ItemOBJ && Hand.Instance.itemType == 1)
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(mouseSlotDT.weapon, mouseSlotDT, slotGrid);
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(mouseSlotDT.weapon, mouseSlotDT, slotGrid);
			}
			if ((bool)mouseSlotDT.ItemOBJ)
			{
				mouseSlotDT.ItemOBJ.RefreshBS(mouseSlotDT);
			}
			break;
		case 1:
			if (!Hand.Instance.ItemOBJ)
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowBSTip(mouseSlotDT.baoshi, mouseSlotDT, slotGrid);
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			}
			break;
		case 2:
			if (!Hand.Instance.ItemOBJ)
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowUseTip(mouseSlotDT.useitem, mouseSlotDT, slotGrid);
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			}
			break;
		default:
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			break;
		}
	}

	private void HideCurrentPageSocketDisplays()
	{
		int num = CurPage - 1;
		if (MainPages == null || num < 0 || num >= MainPages.Count || MainPages[num] == null)
		{
			return;
		}
		List<SlotData> mainList = MainPages[num].MainList;
		if (mainList == null)
		{
			return;
		}
		for (int i = 0; i < mainList.Count; i++)
		{
			ItemScript itemScript = mainList[i]?.ItemOBJ;
			if ((bool)itemScript)
			{
				itemScript.HideSocketDisplay();
			}
		}
	}

	private SlotScript FindSlotAtCursorScreenPosition()
	{
		if (slotGrid == null || !SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			return null;
		}
		Vector2 screenPoint = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		for (int i = 0; i < slotGrid.GetLength(1); i++)
		{
			for (int j = 0; j < slotGrid.GetLength(0); j++)
			{
				SlotScript slotScript = slotGrid[j, i];
				if (!slotScript || slotScript.type != ContainerType || !slotScript.gameObject.activeInHierarchy)
				{
					continue;
				}
				RectTransform component = slotScript.GetComponent<RectTransform>();
				if ((bool)component)
				{
					Canvas componentInParent = component.GetComponentInParent<Canvas>();
					Camera cam = (((bool)componentInParent && componentInParent.renderMode != 0) ? componentInParent.worldCamera : null);
					if (RectTransformUtility.RectangleContainsScreenPoint(component, screenPoint, cam))
					{
						return slotScript;
					}
				}
			}
		}
		return null;
	}

	protected static bool GetShiftModifier()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return GamepadInputManager.GetKey("Pad_LStickPress");
		}
		return Input.GetKey(KeyCode.LeftShift);
	}

	protected static bool GetCtrlModifier()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return GamepadInputManager.GetKey("Pad_RStickPress");
		}
		return Input.GetKey(KeyCode.LeftControl);
	}
}
