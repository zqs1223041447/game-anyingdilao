using System;
using System.Collections.Generic;
using Container.Inventory;
using Container.Managers;
using Container.Util;
using Core;
using Data.SaveData;
using Entity.InteractableObjects.Item;
using FMODUnity;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs.Cursors;
using Lean.Pool;
using UI.Managers;
using UI.Panels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : ContainerManager<InventoryManager>
{
	private struct SortEntry
	{
		public int ItemType;

		public int Seq;

		public WeaponClass Wp;

		public BaoshiClass Bs;

		public UseItemClass Use;
	}

	public InventorySaveData SaveData;

	public int PageSaveCount;

	private long globalMoney;

	public Text moneyText;

	public CharButton MouseCharBT;

	public CharButton[] CharBT;

	private PlayerManager PL;

	[HideInInspector]
	public XJL_FSQ xjl;

	protected override ContainerType ContainerType => ContainerType.Inventory;

	public override int MaxPageCount => 1000;

	public override int ContainerMpos => 0;

	public bool IsInventoryDataReady { get; private set; }

	public bool InvInteractToggle { get; private set; } = true;


	public long GlobalMoney
	{
		get
		{
			return globalMoney;
		}
		set
		{
			globalMoney = Math.Max(0L, value);
			if ((bool)moneyText)
			{
				moneyText.text = globalMoney.ToString();
			}
		}
	}

	public void InitFromSaveData(InventorySaveData data)
	{
		IsInventoryDataReady = false;
		SaveData = DataUtil.DeepClone(data);
		ApplySaveData(SaveData);
	}

	public InventorySaveData ExportSaveData()
	{
		PageSaveCount = PageNumber;
		InventorySaveData inventorySaveData = new InventorySaveData
		{
			Money = GlobalMoney,
			PageCount = PageSaveCount
		};
		SaveEquipments(inventorySaveData);
		ContainerSaveUtil.SaveContainerItems(ItemList, inventorySaveData.InventoryItems);
		return inventorySaveData;
	}

	public void SaveEquipments(InventorySaveData data)
	{
		if (data == null)
		{
			return;
		}
		data.Equipments.Clear();
		CharButton[] charBT = CharBT;
		foreach (CharButton charButton in charBT)
		{
			if ((bool)charButton && charButton.hasWeapon && charButton.weapon != null)
			{
				data.Equipments.Add(WeaponSaveData.FromRuntime(charButton.weapon));
			}
			else
			{
				data.Equipments.Add(null);
			}
		}
	}

	public void ApplySaveData(InventorySaveData data)
	{
		if (data == null)
		{
			data = InventorySaveData.CreateDefault();
		}
		GlobalMoney = data.Money;
		PageSaveCount = data.PageCount;
		RestoreSaveData(data);
	}

	public void RestoreSaveData(InventorySaveData data)
	{
		if (data != null)
		{
			CreateSlots(ContainerType.Inventory);
			for (int i = 0; i < PageSaveCount; i++)
			{
				CreatePage();
			}
			RefreshPageText();
			SingletonMonoScope<PlayerManager>.Instance?.ClearEquippedSetCounts();
			LoadEquipments(data);
			RestoreInventoryItems(data);
			IsInventoryDataReady = true;
		}
	}

	public void LoadEquipments(InventorySaveData data)
	{
		ClearEquipmentButtons();
		if (data?.Equipments == null)
		{
			return;
		}
		int count = data.Equipments.Count;
		for (int i = 0; i < count; i++)
		{
			WeaponSaveData weaponSaveData = data.Equipments[i];
			if (weaponSaveData == null)
			{
				continue;
			}
			CharButton equipmentButtonForSave = GetEquipmentButtonForSave(weaponSaveData, i);
			if ((bool)equipmentButtonForSave && !equipmentButtonForSave.hasWeapon)
			{
				if (equipmentButtonForSave.weapon == null)
				{
					equipmentButtonForSave.weapon = new WeaponClass();
				}
				weaponSaveData.ApplyToRuntime(equipmentButtonForSave.weapon);
				ContainerRestoreUtil.RestoreWeaponRuntimeRefs(equipmentButtonForSave.weapon);
				EquipWeaponCore(equipmentButtonForSave, isRestore: true);
			}
		}
	}

	private void ClearEquipmentButtons()
	{
		if (CharBT == null)
		{
			return;
		}
		for (int i = 0; i < CharBT.Length; i++)
		{
			CharButton charButton = CharBT[i];
			if ((bool)charButton)
			{
				if ((bool)charButton.ItemOBJ)
				{
					LeanPool.Despawn(charButton.ItemOBJ.gameObject);
					charButton.ItemOBJ = null;
				}
				charButton.hasWeapon = false;
				charButton.back.gameObject.SetActive(value: true);
				charButton.icon.color = new Color32(0, 0, 0, 0);
			}
		}
	}

	private CharButton GetEquipmentButtonForSave(WeaponSaveData save, int fallbackIndex)
	{
		if (save == null)
		{
			return null;
		}
		CharButton charButton = ReturnCharBT(save.CharType);
		if ((bool)charButton)
		{
			return charButton;
		}
		if (CharBT != null && fallbackIndex >= 0 && fallbackIndex < CharBT.Length)
		{
			return CharBT[fallbackIndex];
		}
		return null;
	}

	public void RestoreInventoryItems(InventorySaveData data)
	{
		if (data?.InventoryItems == null)
		{
			return;
		}
		foreach (ContainerItemSaveData inventoryItem in data.InventoryItems)
		{
			if (inventoryItem != null)
			{
				SlotData slotData = ContainerRestoreUtil.RestoreOneItemToPage(inventoryItem, Page, MainPages, inventorySize);
				if (slotData != null)
				{
					ContainerGridUtil.OccupyRegion(slotData, Page, EPPages, MainPages);
					ContainerPageViewUtil.SpawnItemUI(slotData, CurPage, IVitem, dropParent, slotGrid, Page, 0);
				}
			}
		}
	}

	public void ToggleInteract(bool isOn)
	{
		InvInteractToggle = isOn;
	}

	public void AddMoney(long value)
	{
		GlobalMoney += value;
	}

	public void RemoveMoney(long value)
	{
		GlobalMoney -= value;
	}

	protected override void Awake()
	{
		base.Awake();
		moneyText = base.transform.Find("Money/Text").GetComponent<Text>();
		closeBtn.onClick.AddListener(CloseUI);
		PL = SingletonMonoScope<PlayerManager>.Instance;
		xjl = PL.GetComponent<XJL_FSQ>();
	}

	private void Start()
	{
		if ((bool)moneyText)
		{
			moneyText.text = GlobalMoney.ToString();
		}
	}

	private void Update()
	{
		if (!InvInteractToggle || (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.IsInModalState))
		{
			return;
		}
		InventorySortBar.Tick(this);
		KeyCodeThrowItem(0);
		bool flag = SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.Opened_baoshi && SingletonMonoScope<BaoshiManager>.HasInstance && SingletonMonoScope<BaoshiManager>.Instance.IsGamepadCreateShortcutDown();
		bool flag2 = ContainerManager<InventoryManager>.GetCursorRightDown();
		if (!flag && TryConsumeGamepadShiftRightDown())
		{
			flag2 = true;
		}
		if (ContainerManager<InventoryManager>.GetShiftModifier())
		{
			if (ContainerManager<InventoryManager>.GetCursorLeftDown() && Hand.Instance.Mpos == 0)
			{
				if (SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse && (bool)MouseSlot && !Hand.Instance.ItemOBJ && base.MouseSlotDT != null && base.MouseSlotDT.isOC)
				{
					CursorUIManager.ConsumeShiftModifier();
					switch (base.MouseSlotDT.ItemType)
					{
					case 0:
						if (SingletonMonoScope<WarehouseManager>.Instance.IVtoChest(base.MouseSlotDT.weapon))
						{
							ThrowItem();
							SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
						}
						else
						{
							GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
							LogUtil.Info("仓库空间不足");
						}
						break;
					case 1:
					{
						TransferResult transferResult2 = SingletonMonoScope<WarehouseManager>.Instance.IVtoChest(base.MouseSlotDT.baoshi, base.MouseSlotDT);
						if (transferResult2.Success)
						{
							if (transferResult2.IsComplete)
							{
								ThrowItem();
								SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
							}
							else if ((bool)base.MouseSlotDT.ItemOBJ)
							{
								base.MouseSlotDT.ItemOBJ.RefreshStackIV(0);
							}
						}
						else
						{
							GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
							LogUtil.Info("仓库空间不足");
						}
						break;
					}
					case 2:
					{
						TransferResult transferResult = SingletonMonoScope<WarehouseManager>.Instance.IVtoChest(base.MouseSlotDT.useitem, base.MouseSlotDT);
						if (transferResult.Success)
						{
							SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(base.MouseSlotDT.useitem);
							if (transferResult.IsComplete)
							{
								ThrowItem();
								SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
							}
							else if ((bool)base.MouseSlotDT.ItemOBJ)
							{
								base.MouseSlotDT.ItemOBJ.RefreshStackIV(1);
							}
						}
						else
						{
							GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
							LogUtil.Info("仓库空间不足");
						}
						break;
					}
					}
				}
				if (SingletonMonoScope<GameUIManager>.Instance.Opened_shop && (bool)MouseSlot && !Hand.Instance.ItemOBJ && base.MouseSlotDT != null && base.MouseSlotDT.isOC)
				{
					CursorUIManager.ConsumeShiftModifier();
					QuickSell();
				}
			}
			if (!flag2 || Hand.Instance.Mpos != 0 || !MouseSlot || base.MouseSlotDT == null || !base.MouseSlotDT.isOC)
			{
				return;
			}
			CursorUIManager.ConsumeShiftModifier();
			if (!Hand.Instance.ItemOBJ)
			{
				switch (base.MouseSlotDT.ItemType)
				{
				case 1:
					SplitFirst();
					break;
				case 2:
					SplitFirst();
					SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(base.MouseSlotDT.useitem);
					break;
				case 0:
					break;
				}
				return;
			}
			switch (base.MouseSlotDT.ItemType)
			{
			case 1:
				if (Hand.Instance.baoshi.ItemName == base.MouseSlotDT.baoshi.ItemName)
				{
					SplitSecond();
				}
				break;
			case 2:
				if (Hand.Instance.useitem.ItemName == base.MouseSlotDT.useitem.ItemName)
				{
					SplitSecond();
					SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(base.MouseSlotDT.useitem);
				}
				break;
			case 0:
				break;
			}
			return;
		}
		if (ContainerManager<InventoryManager>.GetCursorLeftDown())
		{
			if (CanPutHandItemToMouseSlot())
			{
				switch (checkState)
				{
				case 0:
					switch (Hand.Instance.itemType)
					{
					case 0:
					case 1:
						PutItem();
						break;
					case 2:
						PutItem();
						break;
					}
					Hand.Instance.ClearItem();
					break;
				case 1:
				{
					SlotData slotData = null;
					if (otherItemPos.x >= 0 && otherItemPos.y >= 0 && otherItemPos.x < Page[CurPage - 1].DT.GetLength(0) && otherItemPos.y < Page[CurPage - 1].DT.GetLength(1))
					{
						slotData = Page[CurPage - 1].DT[otherItemPos.x, otherItemPos.y];
					}
					if (slotData == null || !slotData.isOC)
					{
						break;
					}
					switch (Hand.Instance.itemType)
					{
					case 0:
						switch (slotData.ItemType)
						{
						case 0:
						case 1:
							SwapItem();
							break;
						case 2:
							SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
							SwapItem();
							break;
						}
						break;
					case 1:
						switch (slotData.ItemType)
						{
						case 0:
							switch (Hand.Instance.baoshi.UseType)
							{
							case 0:
								if (slotData.weapon.AocaoCount > 0)
								{
									if (GetEMac(slotData.weapon) != null)
									{
										if (!_audioManager && SingletonMonoGlobal<AudioManager>.HasInstance)
										{
											_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
										}
										RuntimeManager.PlayOneShot(_audioManager.audioData.Baoshi.Use[Hand.Instance.baoshi.SoundUse]);
										XiangQian(slotData.weapon, Hand.Instance.baoshi, GetEMacNumber(slotData.weapon));
										SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(slotData.weapon, slotData, slotGrid);
										EnsureCurrentPageItemObj(slotData);
										if ((bool)slotData.ItemOBJ)
										{
											slotData.ItemOBJ.RefreshBS(slotData);
										}
										Sector.sec.SetPosOffset();
										if (Hand.Instance.baoshi.CstackSize > 1)
										{
											Hand.Instance.baoshi.CstackSize--;
											Hand.Instance.ItemOBJ.RefreshStackHand(0);
											break;
										}
										Hand.Instance.DELItem();
										Sector.sec.SetPosOffset();
										RefreshColor(enter: true);
										SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(slotData.weapon, slotData, slotGrid);
									}
									else
									{
										SwapItem();
									}
								}
								else
								{
									SwapItem();
								}
								break;
							case 1:
								TryApplyHeldSpecialBaoshiToWeapon(slotData);
								break;
							case 2:
								TryApplyHeldSpecialBaoshiToWeapon(slotData);
								break;
							case 3:
								TryApplyHeldSpecialBaoshiToWeapon(slotData);
								break;
							case 4:
								TryApplyHeldSpecialBaoshiToWeapon(slotData);
								break;
							case 5:
								TryApplyHeldSpecialBaoshiToWeapon(slotData);
								break;
							}
							break;
						case 1:
						{
							BaoshiClass baoshi = slotData.baoshi;
							BaoshiClass baoshi2 = Hand.Instance.baoshi;
							_audioManager.PlaySO_Item(1, baoshi2.BStype, baoshi2.SoundDrop, 0);
							if (baoshi.ItemName == baoshi2.ItemName)
							{
								if (baoshi.CstackSize == baoshi.MstackSize)
								{
									SwapItem();
								}
								else if (baoshi.CstackSize + baoshi2.CstackSize <= baoshi.MstackSize)
								{
									baoshi.CstackSize += baoshi2.CstackSize;
									slotData.ItemOBJ.RefreshStackIV(0);
									Hand.Instance.DELItem();
									SingletonMonoScope<GameUIManager>.Instance.ShowBSTip(slotData.baoshi, slotData, slotGrid);
									Sector.sec.SetPosOffset();
									RefreshColor(enter: true);
								}
								else
								{
									baoshi2.CstackSize -= baoshi.MstackSize - baoshi.CstackSize;
									baoshi.CstackSize = baoshi.MstackSize;
									slotData.ItemOBJ.RefreshStackIV(0);
									Hand.Instance.ItemOBJ.RefreshStackHand(0);
								}
							}
							else
							{
								SwapItem();
							}
							Sector.sec.SetPosOffset();
							ContainerSlotUtil.ColorChange(SlotColor.TouMing, otherItemSize, otherItemPos, slotGrid);
							RefreshColor(enter: true);
							break;
						}
						case 2:
							SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
							SwapItem();
							break;
						}
						break;
					case 2:
						switch (slotData.ItemType)
						{
						case 0:
							SwapItem();
							SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
							break;
						case 1:
							SwapItem();
							SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
							break;
						case 2:
						{
							UseItemClass useitem = slotData.useitem;
							UseItemClass useitem2 = Hand.Instance.useitem;
							_audioManager.PlaySO_Item(2, useitem2.UseType, useitem2.SoundDrop, useitem2.InfoType);
							if (useitem.ItemName == useitem2.ItemName)
							{
								if (useitem.CstackSize == useitem.MstackSize)
								{
									SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
									SwapItem();
									SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
								}
								else if (useitem.CstackSize + useitem2.CstackSize <= useitem.MstackSize)
								{
									useitem.CstackSize += useitem2.CstackSize;
									SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
									slotData.ItemOBJ.RefreshStackIV(1);
									Hand.Instance.DELItem();
									SingletonMonoScope<GameUIManager>.Instance.ShowUseTip(slotData.useitem, slotData, slotGrid);
									Sector.sec.SetPosOffset();
									RefreshColor(enter: true);
								}
								else
								{
									SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
									useitem2.CstackSize -= useitem.MstackSize - useitem.CstackSize;
									useitem.CstackSize = useitem.MstackSize;
									slotData.ItemOBJ.RefreshStackIV(1);
									Hand.Instance.ItemOBJ.RefreshStackHand(1);
								}
							}
							else
							{
								SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
								SwapItem();
								SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
							}
							break;
						}
						}
						break;
					}
					break;
				}
				}
			}
			else if (CanOperateMouseSlotItem())
			{
				switch (base.MouseSlotDT.ItemType)
				{
				case 0:
				case 1:
					TakeItem();
					break;
				case 2:
					TakeItem();
					break;
				}
			}
		}
		if (flag || !ContainerManager<InventoryManager>.GetCursorRightDown() || !CanOperateMouseSlotItem())
		{
			return;
		}
		switch (base.MouseSlotDT.ItemType)
		{
		case 0:
		{
			SlotData mouseSlotDT = base.MouseSlotDT;
			if (mouseSlotDT?.weapon == null || (mouseSlotDT.weapon.PLtype != PL.PLType && !WeaponPlayerType.IsGeneric(mouseSlotDT.weapon.PLtype)))
			{
				break;
			}
			EnsureCurrentPageItemObj(mouseSlotDT);
			CharButton charButton = ReturnCharBT(mouseSlotDT.weapon.CharType);
			if ((bool)charButton && mouseSlotDT.weapon.Level <= PL.Level && (bool)mouseSlotDT.ItemOBJ)
			{
				if (charButton.hasWeapon)
				{
					EquipSwapSlot();
				}
				else
				{
					EquipmentSlot();
				}
			}
			break;
		}
		case 2:
			switch (base.MouseSlotDT.useitem.InfoType)
			{
			case 0:
				if (SingletonMonoScope<SimplePotionManager>.Instance.HasSameDrink(base.MouseSlotDT.useitem))
				{
					break;
				}
				switch (base.MouseSlotDT.useitem.UseType)
				{
				case "health":
					if (PL.HealStat.Cur < PL.HealStat.Max)
					{
						UseItem();
					}
					break;
				case "mana":
					if (PL.ManaStat.Cur < PL.ManaStat.Max)
					{
						UseItem();
					}
					break;
				case "huoli":
					if (PL.HealStat.Cur < PL.HealStat.Max || PL.ManaStat.Cur < PL.ManaStat.Max)
					{
						UseItem();
					}
					break;
				}
				break;
			case 1:
				UseItem();
				break;
			case 2:
				if (!SingletonMonoScope<SimplePotionManager>.Instance.HasSameDrink(base.MouseSlotDT.useitem))
				{
					if (!CheckScrollUseLimit(checkHomeScene: false))
					{
						GameManager.ShowTip(LOC.MM.GetLevel("scroll_hint_no"), TipType.Fail);
						return;
					}
					UseItem();
				}
				break;
			case 3:
				UseItem();
				break;
			case 4:
				UseItem();
				break;
			case 5:
				switch (base.MouseSlotDT.useitem.UseType)
				{
				case "yiwang":
					if (SingletonMonoScope<TalentManager>.Instance.P_Used > 0)
					{
						UseItem();
					}
					break;
				case "lunhui":
					if (SingletonMonoScope<TalentManager>.Instance.HasUsedDFTalentPoint())
					{
						UseItem();
					}
					break;
				case "shenyou":
					UseItem();
					break;
				case "juexing":
					UseItem();
					break;
				}
				break;
			case 6:
				UseItem();
				break;
			case 7:
				UseItem();
				break;
			}
			break;
		}
		Sector.sec.SetPosOffset();
		RefreshColor(enter: true);
	}

	public void PutItem()
	{
		SlotData slotData = Page[CurPage - 1].DT[totalOffset.x, totalOffset.y];
		if (slotData == null)
		{
			return;
		}
		ItemScript itemOBJ = Hand.Instance.ItemOBJ;
		if (!itemOBJ)
		{
			return;
		}
		switch (Hand.Instance.itemType)
		{
		case 0:
		{
			WeaponClass weapon = Hand.Instance.weapon;
			_audioManager.PlaySO_Item(weapon.ItemType, weapon.WeaponType, weapon.SoundDrop, 0);
			slotData.Page = CurPage - 1;
			slotData.GridPos = totalOffset;
			slotData.StartPos = totalOffset;
			slotData.ItemType = 0;
			slotData.ItemSize = weapon.Size;
			slotData.ItemOBJ = itemOBJ;
			ItemCloneUtil.CopyWeaponTo(slotData.weapon, weapon);
			ContainerGridUtil.OccupyRegion(slotData, Page, EPPages, MainPages, slotGrid, SlotColor.TouMing);
			for (int i = 0; i < slotData.weapon.AocaoCount; i++)
			{
				if (!Hand.Instance.isDragItem)
				{
					itemOBJ.aocao[i].gameObject.SetActive(value: true);
					if (slotData.weapon.Aocao[i].HasBaoshi)
					{
						itemOBJ.aocao[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
						itemOBJ.BS[i].sprite = slotData.weapon.Aocao[i].Icon;
						itemOBJ.BS[i].gameObject.SetActive(value: true);
						itemOBJ.BS[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					}
					else
					{
						itemOBJ.aocao[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 100);
						itemOBJ.BS[i].gameObject.SetActive(value: false);
						itemOBJ.BS[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
					}
				}
				else
				{
					itemOBJ.aocao[i].gameObject.SetActive(value: false);
				}
			}
			SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(base.MouseSlotDT.weapon, base.MouseSlotDT, slotGrid);
			break;
		}
		case 1:
		{
			BaoshiClass baoshi = Hand.Instance.baoshi;
			_audioManager.PlaySO_Item(baoshi.ItemType, baoshi.BStype, baoshi.SoundDrop, 0);
			slotData.Page = CurPage - 1;
			slotData.GridPos = totalOffset;
			slotData.StartPos = totalOffset;
			slotData.ItemType = 1;
			slotData.ItemSize = baoshi.Size;
			slotData.ItemOBJ = itemOBJ;
			ItemCloneUtil.CopyBaoshiTo(slotData.baoshi, baoshi);
			ContainerGridUtil.OccupyRegion(slotData, Page, EPPages, MainPages, slotGrid, SlotColor.TouMing);
			SingletonMonoScope<GameUIManager>.Instance.ShowBSTip(slotData.baoshi, slotData, slotGrid);
			break;
		}
		case 2:
		{
			UseItemClass useitem = Hand.Instance.useitem;
			_audioManager.PlaySO_Item(useitem.ItemType, useitem.UseType, useitem.SoundDrop, useitem.InfoType);
			slotData.Page = CurPage - 1;
			slotData.GridPos = totalOffset;
			slotData.StartPos = totalOffset;
			slotData.ItemType = 2;
			slotData.ItemSize = useitem.Size;
			slotData.ItemOBJ = itemOBJ;
			ItemCloneUtil.CopyUseItemTo(slotData.useitem, useitem);
			ContainerGridUtil.OccupyRegion(slotData, Page, EPPages, MainPages, slotGrid, SlotColor.TouMing);
			SingletonMonoScope<GameUIManager>.Instance.ShowUseTip(slotData.useitem, slotData, slotGrid);
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			}
			break;
		}
		}
		itemOBJ.transform.SetParent(dropParent);
		itemOBJ.GetComponent<RectTransform>().pivot = Vector2.up;
		itemOBJ.transform.position = slotGrid[totalOffset.x, totalOffset.y].transform.position;
		itemOBJ.page = CurPage - 1;
		itemOBJ.saveSlot = new IntVector2(totalOffset.x, totalOffset.y);
		Sector.sec.SetPosOffset();
	}

	public void TakeItem()
	{
		SlotData slotData = Page[CurPage - 1].DT[MouseSlot.GridPos.x, MouseSlot.GridPos.y];
		if (slotData != null && slotData.isOC)
		{
			UseItemClass useItemClass = null;
			if (slotData.ItemType == 2 && slotData.useitem != null)
			{
				useItemClass = slotData.useitem;
			}
			Hand.Instance.TakeItem(slotData);
			ContainerSlotUtil.ColorChange(SlotColor.TouMing, slotData.ItemSize, slotData.StartPos, slotGrid);
			ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages);
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			if (useItemClass != null && SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useItemClass);
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			}
			else if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			}
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
		}
	}

	public void SwapItem()
	{
		SlotData slotData = Page[CurPage - 1].DT[otherItemPos.x, otherItemPos.y];
		if (slotData != null)
		{
			ItemScript itemOBJ = slotData.ItemOBJ;
			ContainerSlotUtil.ColorChange(SlotColor.TouMing, slotData.ItemSize, slotData.StartPos, slotGrid);
			switch (slotData.ItemType)
			{
			case 0:
			{
				WeaponClass wp = ItemCloneUtil.CloneWeapon(slotData.weapon);
				ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages);
				PutItem();
				Hand.Instance.TakeWP(wp, itemOBJ);
				break;
			}
			case 1:
			{
				BaoshiClass bS = ItemCloneUtil.CloneBaoshi(slotData.baoshi);
				ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages);
				PutItem();
				Hand.Instance.TakeBS(bS, itemOBJ);
				break;
			}
			case 2:
			{
				UseItemClass uSE = ItemCloneUtil.CloneUseItem(slotData.useitem);
				ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages);
				PutItem();
				Hand.Instance.TakeUSE(uSE, itemOBJ);
				break;
			}
			}
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
		}
	}

	public void HandItemDrop()
	{
		UseItemClass useItemClass = null;
		if (Hand.Instance.itemType == 2 && Hand.Instance.useitem != null)
		{
			useItemClass = ItemCloneUtil.CloneUseItem(Hand.Instance.useitem);
		}
		switch (Hand.Instance.itemType)
		{
		case 0:
			SingletonMonoScope<ItemManager>.Instance.ThrowWP(Hand.Instance.weapon);
			Hand.Instance.DELItem();
			break;
		case 1:
			SingletonMonoScope<ItemManager>.Instance.ThrowBS(Hand.Instance.baoshi);
			Hand.Instance.DELItem();
			break;
		case 2:
			SingletonMonoScope<ItemManager>.Instance.ThrowUSE(Hand.Instance.useitem);
			Hand.Instance.DELItem();
			break;
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			if (useItemClass != null)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useItemClass);
			}
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
	}

	public void EquipmentSlot()
	{
		WeaponClass weapon = base.MouseSlotDT.weapon;
		if (weapon == null)
		{
			return;
		}
		CharButton charButton = ReturnCharBT(weapon.CharType);
		if ((bool)charButton)
		{
			ItemCloneUtil.CopyWeaponTo(charButton.weapon, weapon);
			EquipWeaponCore(charButton, isRestore: false, weapon);
			ThrowItem();
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			RefreshColor(enter: true);
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
		}
	}

	public void EquipSwapSlot()
	{
		SlotData mouseSlotDT = base.MouseSlotDT;
		SlotData slotData = ContainerGridUtil.GetMainSlot(mouseSlotDT, Page) ?? mouseSlotDT;
		if (slotData == null || !slotData.isMain || !slotData.isOC || slotData.weapon == null)
		{
			return;
		}
		EnsureCurrentPageItemObj(slotData);
		WeaponClass weaponClass = ItemCloneUtil.CloneWeapon(slotData.weapon);
		if (weaponClass == null)
		{
			return;
		}
		CharButton charButton = ReturnCharBT(weaponClass.CharType);
		if (!charButton || !charButton.hasWeapon || charButton.weapon == null)
		{
			return;
		}
		WeaponClass weaponClass2 = ItemCloneUtil.CloneWeapon(charButton.weapon);
		if (weaponClass2 == null)
		{
			return;
		}
		ItemScript itemScript = slotData.ItemOBJ;
		if (!itemScript)
		{
			itemScript = SpawnLooseItemObj();
			if (!itemScript)
			{
				return;
			}
			itemScript.SetWP(weaponClass);
		}
		ItemScript itemOBJ = charButton.ItemOBJ;
		int page = slotData.Page;
		IntVector2 startPos = slotData.StartPos;
		IntVector2 itemSize = slotData.ItemSize;
		ContainerSlotUtil.ColorChange(SlotColor.TouMing, itemSize, startPos, slotGrid);
		ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages, slotGrid, SlotColor.TouMing);
		SlotData slotData2 = null;
		slotData2 = ((page < 0 || page >= Page.Count || Page[page]?.DT == null || startPos.x < 0 || startPos.y < 0 || startPos.x >= Page[page].DT.GetLength(0) || startPos.y >= Page[page].DT.GetLength(1) || !ContainerGridUtil.CanPlaceAt(Page[page], startPos, weaponClass2.Size, inventorySize)) ? CheckEmpty(weaponClass2.Size) : Page[page].DT[startPos.x, startPos.y]);
		SlotData mainSlot;
		if (slotData2 == null)
		{
			TryPlaceWeaponInContainer(Page[page].DT[startPos.x, startPos.y], weaponClass, itemScript, out mainSlot);
			GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			LogUtil.Info("背包空间不足");
			SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(weaponClass, base.MouseSlotDT, slotGrid);
			RefreshColor(enter: true);
			return;
		}
		if (!TryPlaceWeaponInContainer(slotData2, weaponClass2, itemOBJ, out var mainSlot2))
		{
			TryPlaceWeaponInContainer(Page[page].DT[startPos.x, startPos.y], weaponClass, itemScript, out mainSlot);
			GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			LogUtil.Info("背包空间不足");
			RefreshColor(enter: true);
			return;
		}
		charButton.weapon.Equip(1);
		charButton.ItemOBJ = null;
		_audioManager.PlaySO_Item(0, weaponClass.WeaponType, weaponClass.SoundDrop, 0);
		if (PutWeaponOnEquipmentButton(charButton, weaponClass, itemScript, applyEquipEffect: true))
		{
			if (mainSlot2 != null)
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(weaponClass2, weaponClass, mainSlot2, slotGrid);
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			}
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
			RefreshColor(enter: true);
		}
	}

	private bool TryPlaceWeaponInContainer(SlotData targetPos, WeaponClass weapon, ItemScript itemObj, out SlotData mainSlot)
	{
		mainSlot = null;
		if (targetPos == null || weapon == null)
		{
			return false;
		}
		int page = targetPos.Page;
		IntVector2 gridPos = targetPos.GridPos;
		if (page < 0 || page >= Page.Count || Page[page]?.DT == null)
		{
			return false;
		}
		if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= Page[page].DT.GetLength(0) || gridPos.y >= Page[page].DT.GetLength(1))
		{
			return false;
		}
		bool flag = page == CurPage - 1;
		ItemScript itemScript = (flag ? itemObj : null);
		if (flag && !itemScript)
		{
			itemScript = SpawnContainerItemObj(page, gridPos);
			if (!itemScript)
			{
				return false;
			}
		}
		mainSlot = PrepareWeaponMainSlot(Page[page].DT[gridPos.x, gridPos.y], weapon, itemScript);
		if (mainSlot == null)
		{
			return false;
		}
		ContainerGridUtil.OccupyRegion(mainSlot, Page, EPPages, MainPages, slotGrid, SlotColor.TouMing);
		if (flag && (bool)itemScript)
		{
			BindItemObjToRegion(page, gridPos, weapon.Size, itemScript);
			itemScript.SetWP(mainSlot.weapon);
			itemScript.transform.SetParent(dropParent);
			RectTransform component = itemScript.GetComponent<RectTransform>();
			if ((bool)component)
			{
				component.pivot = Vector2.up;
				component.localScale = Vector3.one;
			}
			itemScript.transform.position = slotGrid[gridPos.x, gridPos.y].transform.position;
			itemScript.page = page;
			itemScript.saveSlot = new IntVector2(gridPos.x, gridPos.y);
			if (ShouldShowSocketDisplay(mainSlot))
			{
				itemScript.RefreshBS(mainSlot);
			}
			else
			{
				itemScript.HideSocketDisplay();
			}
		}
		else if ((bool)itemObj)
		{
			LeanPool.Despawn(itemObj);
		}
		return true;
	}

	private bool ShouldShowSocketDisplay(SlotData slot)
	{
		SlotData mouseSlotDT = base.MouseSlotDT;
		if (slot != null && mouseSlotDT != null && mouseSlotDT.isOC && mouseSlotDT.ItemType == 0 && slot.Page == mouseSlotDT.Page)
		{
			return slot.StartPos == mouseSlotDT.StartPos;
		}
		return false;
	}

	private bool PutWeaponOnEquipmentButton(CharButton bt, WeaponClass weapon, ItemScript itemObj, bool applyEquipEffect)
	{
		if (!bt || weapon == null)
		{
			return false;
		}
		if (!itemObj)
		{
			itemObj = SpawnLooseItemObj();
			if (!itemObj)
			{
				return false;
			}
		}
		bt.ItemOBJ = itemObj;
		ItemCloneUtil.CopyWeaponTo(bt.weapon, weapon);
		itemObj.SetWP(bt.weapon);
		itemObj.page = -1;
		itemObj.saveSlot = IntVector2.Zero;
		itemObj.transform.SetParent(dropParent);
		RectTransform component = itemObj.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.pivot = new Vector2(0.5f, 0.5f);
			component.localScale = Vector3.one;
		}
		itemObj.transform.position = bt.transform.position;
		if (applyEquipEffect)
		{
			bt.weapon.Equip(0);
		}
		bt.hasWeapon = true;
		bt.back.gameObject.SetActive(value: false);
		bt.icon.color = new Color32(0, 0, 0, 0);
		bt.ShowAocao();
		SingletonMonoScope<GameUIManager>.Instance.ShowWPTipB(bt.transform.position, bt.weapon);
		return true;
	}

	private void EquipSwapSlotLegacy()
	{
		WeaponClass weaponClass = ItemCloneUtil.CloneWeapon(base.MouseSlotDT.weapon);
		ItemScript itemOBJ = base.MouseSlotDT.ItemOBJ;
		SlotData slotData = Page[CurPage - 1].DT[base.MouseSlotDT.StartPos.x, base.MouseSlotDT.StartPos.y];
		ContainerSlotUtil.ColorChange(SlotColor.TouMing, base.MouseSlotDT.ItemSize, base.MouseSlotDT.StartPos, slotGrid);
		IntVector2 intVector = new IntVector2(base.MouseSlotDT.weapon.Size.x, base.MouseSlotDT.weapon.Size.y);
		for (int i = 0; i < intVector.y; i++)
		{
			for (int j = 0; j < intVector.x; j++)
			{
				SlotData slotData2 = Page[CurPage - 1].DT[base.MouseSlotDT.StartPos.x + j, base.MouseSlotDT.StartPos.y + i];
				if (j == 0 && i == 0)
				{
					slotData2.isMain = false;
					MainPages[CurPage - 1].MainList.Remove(slotData2);
				}
				slotData2.isOC = false;
				slotData2.ItemOBJ = null;
				EPPages[CurPage - 1].EPList.Add(slotData2);
			}
		}
		EPPages[CurPage - 1].EPList.Sort((SlotData t1, SlotData t2) => t1.number.CompareTo(t2.number));
		CharButton charButton = ReturnCharBT(weaponClass.CharType);
		WeaponClass weaponClass2 = ItemCloneUtil.CloneWeapon(charButton.weapon);
		ItemScript itemOBJ2 = charButton.ItemOBJ;
		if (CheckEmpty(weaponClass2.Size) != null)
		{
			SlotData slotData3 = ((weaponClass2.Size.x > weaponClass.Size.x || weaponClass2.Size.y > weaponClass.Size.y) ? CheckEmpty(weaponClass2.Size) : slotData);
			_audioManager.PlaySO_Item(0, weaponClass.WeaponType, weaponClass.SoundDrop, 0);
			charButton.weapon.Equip(1);
			IntVector2 intVector2 = new IntVector2(weaponClass2.Size.x, weaponClass2.Size.y);
			for (int k = 0; k < intVector2.y; k++)
			{
				for (int l = 0; l < intVector2.x; l++)
				{
					SlotData slotData4 = Page[slotData3.Page].DT[slotData3.GridPos.x + l, slotData3.GridPos.y + k];
					if (l == 0 && k == 0)
					{
						slotData4.isMain = true;
						MainPages[slotData3.Page].MainList.Add(slotData4);
					}
					slotData4.Page = slotData3.Page;
					slotData4.ItemOBJ = ((slotData3.Page == CurPage - 1) ? itemOBJ2 : null);
					slotData4.ItemType = 0;
					slotData4.ItemSize = weaponClass2.Size;
					slotData4.StartPos = slotData3.GridPos;
					slotData4.isOC = true;
					slotGrid[slotData3.GridPos.x + l, slotData3.GridPos.y + k].image.color = SlotColor.TouMing;
					ItemCloneUtil.CopyWeaponTo(slotData4.weapon, weaponClass2);
					EPPages[slotData3.Page].EPList.Remove(slotData4);
				}
			}
			if (slotData3.Page == CurPage - 1)
			{
				itemOBJ2.SetWP(weaponClass2);
				itemOBJ2.transform.SetParent(dropParent);
				itemOBJ2.GetComponent<RectTransform>().pivot = Vector2.up;
				itemOBJ2.GetComponent<RectTransform>().localScale = Vector3.one;
				itemOBJ2.transform.position = slotGrid[slotData3.GridPos.x, slotData3.GridPos.y].transform.position;
				itemOBJ2.page = CurPage - 1;
				itemOBJ2.saveSlot = new IntVector2(slotData3.GridPos.x, slotData3.GridPos.y);
				itemOBJ2.RefreshBS(Page[slotData3.Page].DT[slotData3.GridPos.x, slotData3.GridPos.y]);
			}
			else
			{
				LeanPool.Despawn(itemOBJ2);
			}
			charButton.ItemOBJ = itemOBJ;
			ItemCloneUtil.CopyWeaponTo(charButton.weapon, weaponClass);
			itemOBJ.SetWP(charButton.weapon);
			charButton.weapon.Equip(0);
			itemOBJ.transform.SetParent(dropParent);
			itemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
			itemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
			itemOBJ.transform.position = charButton.transform.position;
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
			if ((bool)base.MouseSlotDT.ItemOBJ)
			{
				itemOBJ.RefreshBS(base.MouseSlotDT);
				if (base.MouseSlotDT != null)
				{
					SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(weaponClass2, weaponClass, base.MouseSlotDT, slotGrid);
				}
				else
				{
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
					SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
				}
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			}
		}
		else
		{
			GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			LogUtil.Info("背包空间不足");
			for (int m = 0; m < intVector.y; m++)
			{
				for (int n = 0; n < intVector.x; n++)
				{
					SlotData slotData5 = Page[CurPage - 1].DT[slotData.GridPos.x + n, slotData.GridPos.y + m];
					if (n == 0 && m == 0)
					{
						slotData5.isMain = true;
						MainPages[CurPage - 1].MainList.Add(slotData5);
					}
					slotData5.Page = CurPage - 1;
					slotData5.ItemOBJ = itemOBJ;
					slotData5.ItemType = 0;
					slotData5.ItemSize = weaponClass.Size;
					slotData5.StartPos = slotData.GridPos;
					slotData5.isOC = true;
					slotGrid[slotData.GridPos.x + n, slotData.GridPos.y + m].image.color = SlotColor.TouMing;
					ItemCloneUtil.CopyWeaponTo(slotData5.weapon, weaponClass);
					EPPages[CurPage - 1].EPList.Remove(slotData5);
				}
			}
			itemOBJ.SetWP(weaponClass);
			itemOBJ.RefreshBS(base.MouseSlotDT);
			SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(weaponClass, base.MouseSlotDT, slotGrid);
		}
		Sector.sec.SetPosOffset();
		RefreshColor(enter: true);
	}

	public void DeEquipmentSlot()
	{
		if (!MouseCharBT || !MouseCharBT.hasWeapon || MouseCharBT.weapon == null)
		{
			return;
		}
		WeaponClass weaponClass = ItemCloneUtil.CloneWeapon(MouseCharBT.weapon);
		if (weaponClass == null)
		{
			return;
		}
		SlotData slotData = CheckEmpty(weaponClass.Size);
		if (slotData == null)
		{
			GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			LogUtil.Info("背包空间不足");
			return;
		}
		ItemScript itemOBJ = MouseCharBT.ItemOBJ;
		_audioManager.PlaySO_Item(0, weaponClass.WeaponType, weaponClass.SoundDrop, 0);
		if (TryPlaceWeaponInContainer(slotData, weaponClass, itemOBJ, out var _))
		{
			MouseCharBT.weapon.Equip(1);
			MouseCharBT.hasWeapon = false;
			MouseCharBT.back.gameObject.SetActive(value: true);
			MouseCharBT.icon.color = new Color32(0, 0, 0, 0);
			MouseCharBT.ItemOBJ = null;
			SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
			RefreshColor(enter: true);
		}
	}

	private void DeEquipmentSlotLegacy()
	{
		WeaponClass weapon = MouseCharBT.weapon;
		_audioManager.PlaySO_Item(0, weapon.WeaponType, weapon.SoundDrop, 0);
		SlotData slotData = CheckEmpty(weapon.Size);
		for (int i = 0; i < weapon.Size.y; i++)
		{
			for (int j = 0; j < weapon.Size.x; j++)
			{
				SlotData slotData2 = Page[slotData.Page].DT[slotData.GridPos.x + j, slotData.GridPos.y + i];
				if (j == 0 && i == 0)
				{
					slotData2.isMain = true;
					MainPages[slotData.Page].MainList.Add(slotData2);
				}
				slotData2.Page = slotData.Page;
				slotData2.ItemSize = weapon.Size;
				ItemCloneUtil.CopyWeaponTo(slotData2.weapon, weapon);
				slotData2.StartPos = slotData.GridPos;
				slotData2.isOC = true;
				slotData2.ItemType = weapon.ItemType;
				EPPages[slotData.Page].EPList.Remove(slotData2);
			}
		}
		ItemScript itemOBJ = MouseCharBT.ItemOBJ;
		if (slotData.Page == CurPage - 1)
		{
			for (int k = 0; k < weapon.Size.y; k++)
			{
				for (int l = 0; l < weapon.Size.x; l++)
				{
					Page[slotData.Page].DT[slotData.GridPos.x + l, slotData.GridPos.y + k].ItemOBJ = itemOBJ;
				}
			}
			itemOBJ.SetWP(weapon);
			itemOBJ.transform.SetParent(dropParent);
			itemOBJ.GetComponent<RectTransform>().pivot = Vector2.up;
			itemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
			itemOBJ.transform.position = slotGrid[slotData.GridPos.x, slotData.GridPos.y].transform.position;
			itemOBJ.page = slotData.Page;
			itemOBJ.saveSlot = new IntVector2(slotData.GridPos.x, slotData.GridPos.y);
		}
		else
		{
			LeanPool.Despawn(itemOBJ);
		}
		MouseCharBT.weapon.Equip(1);
		MouseCharBT.hasWeapon = false;
		MouseCharBT.back.gameObject.SetActive(value: true);
		MouseCharBT.icon.color = new Color32(0, 0, 0, 0);
		MouseCharBT.ItemOBJ = null;
		SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
	}

	public void EquipmentHand()
	{
		CharButton charButton = ReturnCharBT(Hand.Instance.weapon.CharType);
		ItemScript itemOBJ = Hand.Instance.ItemOBJ;
		_audioManager.PlaySO_Item(0, itemOBJ.weapon.WeaponType, itemOBJ.weapon.SoundDrop, 0);
		Hand.Instance.ItemOBJ = null;
		Hand.Instance.isDragItem = false;
		charButton.ItemOBJ = itemOBJ;
		ItemCloneUtil.CopyWeaponTo(charButton.weapon, Hand.Instance.weapon);
		itemOBJ.SetWP(Hand.Instance.weapon);
		itemOBJ.transform.SetParent(dropParent);
		itemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
		itemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
		itemOBJ.transform.position = charButton.transform.position;
		Hand.Instance.weapon.Equip(0);
		Hand.Instance.DELItem();
		charButton.hasWeapon = true;
		charButton.back.gameObject.SetActive(value: false);
		charButton.icon.color = new Color32(0, 0, 0, 0);
		charButton.ShowAocao();
		SingletonMonoScope<GameUIManager>.Instance.ShowWPTipB(charButton.transform.position, charButton.weapon);
	}

	public void DeEquipmentHand()
	{
		RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		WeaponClass weapon = MouseCharBT.weapon;
		ItemScript itemOBJ = MouseCharBT.ItemOBJ;
		itemOBJ.SetWP(weapon);
		MouseCharBT.weapon.Equip(1);
		MouseCharBT.hasWeapon = false;
		MouseCharBT.back.gameObject.SetActive(value: true);
		MouseCharBT.icon.color = new Color32(0, byte.MaxValue, 0, 15);
		Hand.Instance.Dequip(itemOBJ, weapon);
		MouseCharBT.ItemOBJ = null;
		SingletonMonoScope<GameUIManager>.Instance.HideTooltipB();
	}

	public void EquipSWAPHand()
	{
		WeaponClass wp = ItemCloneUtil.CloneWeapon(MouseCharBT.weapon);
		WeaponClass source = ItemCloneUtil.CloneWeapon(Hand.Instance.weapon);
		ItemScript itemOBJ = MouseCharBT.ItemOBJ;
		ItemScript itemOBJ2 = Hand.Instance.ItemOBJ;
		_audioManager.PlaySO_Item(0, itemOBJ.weapon.WeaponType, itemOBJ.weapon.SoundDrop, 0);
		MouseCharBT.weapon.Equip(1);
		MouseCharBT.ItemOBJ = null;
		ItemCloneUtil.CopyWeaponTo(MouseCharBT.weapon, source);
		MouseCharBT.ItemOBJ = itemOBJ2;
		itemOBJ2.SetWP(MouseCharBT.weapon);
		itemOBJ2.transform.SetParent(dropParent);
		itemOBJ2.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
		itemOBJ2.GetComponent<RectTransform>().localScale = Vector3.one;
		itemOBJ2.transform.position = MouseCharBT.transform.position;
		MouseCharBT.weapon.Equip(0);
		Hand.Instance.ItemOBJ = null;
		Hand.Instance.Dequip(itemOBJ, wp);
		MouseCharBT.hasWeapon = true;
		MouseCharBT.back.gameObject.SetActive(value: false);
		MouseCharBT.icon.color = new Color32(0, 0, 0, 0);
		MouseCharBT.ShowAocao();
		SingletonMonoScope<GameUIManager>.Instance.ShowWPTipB(MouseCharBT.transform.position, MouseCharBT.weapon);
	}

	public bool PickAUTOWP(DropItemController it, bool Auto)
	{
		SlotData slotData = CheckSameBaoshi(it);
		if (slotData != null)
		{
			if (slotData.isOC)
			{
				if (slotData.baoshi.ItemName == it.baoshi.ItemName)
				{
					if (slotData.baoshi.CstackSize + it.baoshi.CstackSize <= slotData.baoshi.MstackSize)
					{
						int cstackSize = it.baoshi.CstackSize;
						slotData.baoshi.CstackSize += it.baoshi.CstackSize;
						if ((bool)slotData.ItemOBJ)
						{
							slotData.ItemOBJ.RefreshStackIV(0);
						}
						if (SingletonMonoScope<ItemTipManager>.HasInstance && it.baoshi != null)
						{
							SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.baoshi.GetTitle(), cstackSize, it.baoshi.Icon);
						}
						LeanPool.Despawn(it.gameObject);
						return true;
					}
					it.baoshi.CstackSize -= slotData.baoshi.MstackSize - slotData.baoshi.CstackSize;
					slotData.baoshi.CstackSize = slotData.baoshi.MstackSize;
					if ((bool)slotData.ItemOBJ)
					{
						slotData.ItemOBJ.RefreshStackIV(0);
					}
					return PickAUTObaoshi(it, Auto);
				}
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
				return false;
			}
			int cstackSize2 = it.baoshi.CstackSize;
			SetSlotData(it);
			if (SingletonMonoScope<ItemTipManager>.HasInstance && it.baoshi != null)
			{
				SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.baoshi.GetTitle(), cstackSize2, it.baoshi.Icon);
			}
			LeanPool.Despawn(it.gameObject);
			return true;
		}
		if (Auto)
		{
			if (it.CanAutoPick)
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			}
		}
		else
		{
			GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
		}
		return false;
	}

	public bool PickAUTObaoshi(DropItemController it, bool Auto)
	{
		SlotData slotData = CheckSameBaoshi(it);
		if (slotData != null)
		{
			if (slotData.isOC)
			{
				if (slotData.baoshi.ItemName == it.baoshi.ItemName)
				{
					if (slotData.baoshi.CstackSize + it.baoshi.CstackSize <= slotData.baoshi.MstackSize)
					{
						int cstackSize = it.baoshi.CstackSize;
						slotData.baoshi.CstackSize += it.baoshi.CstackSize;
						if ((bool)slotData.ItemOBJ)
						{
							slotData.ItemOBJ.RefreshStackIV(0);
						}
						if (SingletonMonoScope<ItemTipManager>.HasInstance && it.baoshi != null)
						{
							SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.baoshi.GetTitle(), cstackSize, it.baoshi.Icon);
						}
						LeanPool.Despawn(it.gameObject);
						if (Auto)
						{
							NotifyAutoPickedBaoshiRuntimeBuff();
						}
						return true;
					}
					it.baoshi.CstackSize -= slotData.baoshi.MstackSize - slotData.baoshi.CstackSize;
					slotData.baoshi.CstackSize = slotData.baoshi.MstackSize;
					if ((bool)slotData.ItemOBJ)
					{
						slotData.ItemOBJ.RefreshStackIV(0);
					}
					return PickAUTObaoshi(it, Auto);
				}
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
				return false;
			}
			int cstackSize2 = it.baoshi.CstackSize;
			SetSlotData(it);
			if (SingletonMonoScope<ItemTipManager>.HasInstance && it.baoshi != null)
			{
				SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.baoshi.GetTitle(), cstackSize2, it.baoshi.Icon);
			}
			LeanPool.Despawn(it.gameObject);
			if (Auto)
			{
				NotifyAutoPickedBaoshiRuntimeBuff();
			}
			return true;
		}
		if (Auto)
		{
			if (it.CanAutoPick)
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			}
		}
		else
		{
			GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
		}
		return false;
	}

	public bool PickAutoUseItem(DropItemController it, bool Auto)
	{
		while (true)
		{
			SlotData slotData = CheckSameUse(it);
			if (slotData == null)
			{
				break;
			}
			if (slotData.isOC)
			{
				if (!(slotData.useitem.ItemName == it.useitem.ItemName))
				{
					continue;
				}
				if (slotData.useitem.CstackSize + it.useitem.CstackSize <= slotData.useitem.MstackSize)
				{
					int cstackSize = it.useitem.CstackSize;
					slotData.useitem.CstackSize += it.useitem.CstackSize;
					if ((bool)slotData.ItemOBJ)
					{
						slotData.ItemOBJ.RefreshStackIV(1);
					}
					SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(it.useitem);
					if (SingletonMonoScope<ItemTipManager>.HasInstance && it.useitem != null)
					{
						SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.useitem.GetTitle(), cstackSize, it.useitem.Icon);
					}
					LeanPool.Despawn(it.gameObject);
					SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
					return true;
				}
				int num = slotData.useitem.MstackSize - slotData.useitem.CstackSize;
				it.useitem.CstackSize -= num;
				slotData.useitem.CstackSize = slotData.useitem.MstackSize;
				if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackIV(1);
				}
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(it.useitem);
				continue;
			}
			int cstackSize2 = it.useitem.CstackSize;
			SetSlotData(it);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(it.useitem);
			if (SingletonMonoScope<ItemTipManager>.HasInstance && it.useitem != null)
			{
				SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.useitem.GetTitle(), cstackSize2, it.useitem.Icon);
			}
			LeanPool.Despawn(it.gameObject);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			return true;
		}
		if (Auto)
		{
			if (it.CanAutoPick)
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
			}
		}
		else
		{
			GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
		}
		SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		return false;
	}

	public void SplitFirst()
	{
		switch (base.MouseSlotDT.ItemType)
		{
		case 1:
			if (base.MouseSlotDT.baoshi.CstackSize > 1)
			{
				for (int k = 0; k < base.MouseSlotDT.baoshi.Size.y; k++)
				{
					for (int l = 0; l < base.MouseSlotDT.baoshi.Size.x; l++)
					{
						Page[CurPage - 1].DT[MouseSlot.GridPos.x + l, MouseSlot.GridPos.y + k].baoshi.CstackSize--;
					}
				}
				base.MouseSlotDT.ItemOBJ.RefreshStackIV(0);
				ItemScript component2 = LeanPool.Spawn(IVitem).GetComponent<ItemScript>();
				SlotData dt2 = Page[CurPage - 1].DT[MouseSlot.GridPos.x, MouseSlot.GridPos.y];
				Hand.Instance.ItemOBJ = component2;
				Hand.Instance.TakeOne(dt2);
				component2.RefreshStackHand(0);
				Sector.sec.SetPosOffset();
				RefreshColor(enter: true);
			}
			else
			{
				TakeItem();
			}
			break;
		case 2:
			if (base.MouseSlotDT.useitem.CstackSize > 1)
			{
				for (int i = 0; i < base.MouseSlotDT.useitem.Size.y; i++)
				{
					for (int j = 0; j < base.MouseSlotDT.useitem.Size.x; j++)
					{
						Page[CurPage - 1].DT[MouseSlot.GridPos.x + j, MouseSlot.GridPos.y + i].useitem.CstackSize--;
					}
				}
				base.MouseSlotDT.ItemOBJ.RefreshStackIV(1);
				ItemScript component = LeanPool.Spawn(IVitem).GetComponent<ItemScript>();
				SlotData dt = Page[CurPage - 1].DT[MouseSlot.GridPos.x, MouseSlot.GridPos.y];
				Hand.Instance.ItemOBJ = component;
				Hand.Instance.TakeOne(dt);
				component.RefreshStackHand(1);
				Sector.sec.SetPosOffset();
				RefreshColor(enter: true);
			}
			else
			{
				TakeItem();
			}
			break;
		}
		RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
	}

	public void SplitSecond()
	{
		switch (base.MouseSlotDT.ItemType)
		{
		case 1:
			if (base.MouseSlotDT.baoshi.CstackSize > 1)
			{
				for (int m = 0; m < base.MouseSlotDT.baoshi.Size.y; m++)
				{
					for (int n = 0; n < base.MouseSlotDT.baoshi.Size.x; n++)
					{
						Page[CurPage - 1].DT[MouseSlot.GridPos.x + n, MouseSlot.GridPos.y + m].baoshi.CstackSize--;
					}
				}
				base.MouseSlotDT.ItemOBJ.RefreshStackIV(0);
				Hand.Instance.baoshi.CstackSize++;
				Hand.Instance.ItemOBJ.RefreshStackHand(0);
			}
			else
			{
				SlotData slotData3 = Page[CurPage - 1].DT[MouseSlot.GridPos.x, MouseSlot.GridPos.y];
				IntVector2 intVector3 = new IntVector2(slotData3.StartPos.x, slotData3.StartPos.y);
				ItemScript itemOBJ2 = slotData3.ItemOBJ;
				Hand.Instance.baoshi.CstackSize++;
				Hand.Instance.ItemOBJ.RefreshStackHand(0);
				ContainerSlotUtil.ColorChange(SlotColor.TouMing, slotData3.ItemSize, slotData3.StartPos, slotGrid);
				IntVector2 intVector4 = new IntVector2(slotData3.baoshi.Size.x, slotData3.baoshi.Size.y);
				for (int num = 0; num < intVector4.y; num++)
				{
					for (int num2 = 0; num2 < intVector4.x; num2++)
					{
						SlotData slotData4 = Page[CurPage - 1].DT[intVector3.x + num2, intVector3.y + num];
						if (num2 == 0 && num == 0)
						{
							slotData4.isMain = false;
							MainPages[CurPage - 1].MainList.Remove(slotData4);
						}
						slotData4.isOC = false;
						slotData4.ItemOBJ = null;
						EPPages[CurPage - 1].EPList.Add(slotData4);
						EPPages[CurPage - 1].EPList.Sort((SlotData t1, SlotData t2) => t1.number.CompareTo(t2.number));
					}
				}
				LeanPool.Despawn(itemOBJ2);
				SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			}
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
			break;
		case 2:
			if (base.MouseSlotDT.useitem.CstackSize > 1)
			{
				for (int i = 0; i < base.MouseSlotDT.useitem.Size.y; i++)
				{
					for (int j = 0; j < base.MouseSlotDT.useitem.Size.x; j++)
					{
						Page[CurPage - 1].DT[MouseSlot.GridPos.x + j, MouseSlot.GridPos.y + i].useitem.CstackSize--;
					}
				}
				base.MouseSlotDT.ItemOBJ.RefreshStackIV(1);
				Hand.Instance.useitem.CstackSize++;
				Hand.Instance.ItemOBJ.RefreshStackHand(1);
			}
			else
			{
				SlotData slotData = Page[CurPage - 1].DT[MouseSlot.GridPos.x, MouseSlot.GridPos.y];
				IntVector2 intVector = new IntVector2(slotData.StartPos.x, slotData.StartPos.y);
				ItemScript itemOBJ = slotData.ItemOBJ;
				Hand.Instance.useitem.CstackSize++;
				Hand.Instance.ItemOBJ.RefreshStackHand(1);
				ContainerSlotUtil.ColorChange(SlotColor.TouMing, slotData.ItemSize, slotData.StartPos, slotGrid);
				IntVector2 intVector2 = new IntVector2(slotData.useitem.Size.x, slotData.useitem.Size.y);
				for (int k = 0; k < intVector2.y; k++)
				{
					for (int l = 0; l < intVector2.x; l++)
					{
						SlotData slotData2 = Page[CurPage - 1].DT[intVector.x + l, intVector.y + k];
						if (l == 0 && k == 0)
						{
							slotData2.isMain = false;
							MainPages[CurPage - 1].MainList.Remove(slotData2);
						}
						slotData2.isOC = false;
						slotData2.ItemOBJ = null;
						EPPages[CurPage - 1].EPList.Add(slotData2);
						EPPages[CurPage - 1].EPList.Sort((SlotData t1, SlotData t2) => t1.number.CompareTo(t2.number));
					}
				}
				LeanPool.Despawn(itemOBJ);
				SingletonMonoScope<GameUIManager>.Instance.HideTooltipA();
			}
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
			break;
		}
		RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
	}

	public void EquipWeaponCore(CharButton bt, bool isRestore, WeaponClass sourceWeapon = null)
	{
		if (!bt || bt.weapon == null)
		{
			return;
		}
		if ((bool)bt.ItemOBJ)
		{
			LeanPool.Despawn(bt.ItemOBJ.gameObject);
			bt.ItemOBJ = null;
		}
		GameObject gameObject = LeanPool.Spawn(IVitem);
		if (!gameObject)
		{
			return;
		}
		ItemScript component = gameObject.GetComponent<ItemScript>();
		if ((bool)component)
		{
			bt.ItemOBJ = component;
			component.SetWP(bt.weapon);
			RectTransform component2 = component.GetComponent<RectTransform>();
			component.transform.SetParent(dropParent);
			if ((bool)component2)
			{
				component2.pivot = new Vector2(0.5f, 0.5f);
				component2.localScale = Vector3.one;
			}
			component.transform.position = bt.transform.position;
			bt.hasWeapon = true;
			bt.back.gameObject.SetActive(value: false);
			bt.icon.color = new Color32(0, 0, 0, 0);
			bt.ShowAocao();
			if (isRestore)
			{
				bt.weapon.RestoreEquip();
				return;
			}
			_audioManager.PlaySO_Item(0, sourceWeapon.WeaponType, sourceWeapon.SoundDrop, 0);
			bt.weapon.Equip(0);
		}
	}

	public void PickUp(DropItemController it, bool rightclick)
	{
		bool flag = false;
		if (cav.alpha == 0f)
		{
			if (!rightclick)
			{
				switch (it.ItemType)
				{
				case 0:
					if (CheckEmpty(it.weapon.Size) != null)
					{
						SetSlotData(it);
						if (SingletonMonoScope<ItemTipManager>.HasInstance && it.weapon != null)
						{
							SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.weapon.GetTitle(), 1, it.weapon.Icon);
						}
						LeanPool.Despawn(it.gameObject);
						flag = true;
					}
					else
					{
						GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
					}
					break;
				case 1:
					flag = PickAUTObaoshi(it, Auto: false);
					break;
				case 2:
					flag = PickAutoUseItem(it, Auto: false);
					break;
				}
			}
		}
		else if (Mathf.Approximately(cav.alpha, 1f))
		{
			if (rightclick)
			{
				switch (it.ItemType)
				{
				case 0:
					if (CheckEmpty(it.weapon.Size) != null)
					{
						SetSlotData(it);
						if (SingletonMonoScope<ItemTipManager>.HasInstance && it.weapon != null)
						{
							SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.weapon.GetTitle(), 1, it.weapon.Icon);
						}
						LeanPool.Despawn(it.gameObject);
						flag = true;
					}
					else
					{
						GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
					}
					break;
				case 1:
					flag = PickAUTObaoshi(it, Auto: false);
					break;
				case 2:
					flag = PickAutoUseItem(it, Auto: false);
					break;
				}
			}
			else
			{
				string text = "";
				int count = 1;
				Sprite icon = null;
				switch (it.ItemType)
				{
				case 0:
					if (it.weapon != null)
					{
						text = it.weapon.GetTitle();
						count = 1;
						icon = it.weapon.Icon;
					}
					break;
				case 1:
					if (it.baoshi != null)
					{
						text = it.baoshi.GetTitle();
						count = it.baoshi.CstackSize;
						icon = it.baoshi.Icon;
					}
					break;
				case 2:
					if (it.useitem != null)
					{
						text = it.useitem.GetTitle();
						count = it.useitem.CstackSize;
						icon = it.useitem.Icon;
					}
					break;
				}
				ItemScript component = LeanPool.Spawn(IVitem).GetComponent<ItemScript>();
				component.SetItem(it, handTake: true);
				Hand.Instance.PickUPItem(it, component);
				if (SingletonMonoScope<ItemTipManager>.HasInstance && !string.IsNullOrEmpty(text))
				{
					SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(text, count, icon);
				}
				LeanPool.Despawn(it.gameObject);
				flag = true;
			}
		}
		if (flag)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		}
	}

	private static void NotifyAutoPickedBaoshiRuntimeBuff()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<PlayerManager>.Instance.IsAlive)
		{
			SingletonMonoScope<PlayerManager>.Instance.BuffRuntime?.OnPickGem();
		}
	}

	private bool TryAutoPickWeapon(DropItemController it, bool showItemTip = true)
	{
		if (!it || it.weapon == null)
		{
			return false;
		}
		if (CheckEmpty(it.weapon.Size) == null)
		{
			return false;
		}
		SetSlotData(it);
		if (showItemTip && SingletonMonoScope<ItemTipManager>.HasInstance)
		{
			SingletonMonoScope<ItemTipManager>.Instance.AddItemTip(it.weapon.GetTitle(), 1, it.weapon.Icon);
		}
		LeanPool.Despawn(it.gameObject);
		return true;
	}

	public bool CanXJLAutoHandle(DropItemController it)
	{
		if (!it)
		{
			return false;
		}
		switch (it.ItemType)
		{
		case 0:
			if (it.weapon != null)
			{
				if (!ShouldXJLSalvageWeapon(it.weapon))
				{
					return ShouldXJLPickWeapon(it.weapon);
				}
				return true;
			}
			return false;
		case 1:
		case 2:
			return true;
		default:
			return false;
		}
	}

	public void AutoPickUpByXJL(DropItemController it)
	{
		if (!it)
		{
			return;
		}
		bool flag = false;
		it.CanAutoPick = false;
		switch (it.ItemType)
		{
		case 0:
			if (ShouldXJLSalvageWeapon(it.weapon))
			{
				SalvageXJLWeapon(it);
			}
			else if (ShouldXJLPickWeapon(it.weapon))
			{
				flag = TryAutoPickWeapon(it);
			}
			break;
		case 1:
			flag = PickAUTObaoshi(it, Auto: true);
			break;
		case 2:
			flag = PickAutoUseItem(it, Auto: true);
			break;
		}
		if (flag)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		}
	}

	public void AutoPickUp(DropItemController it)
	{
		bool flag = false;
		it.CanAutoPick = false;
		switch (it.ItemType)
		{
		case 0:
			flag = TryAutoPickWeapon(it);
			break;
		case 1:
			flag = PickAUTObaoshi(it, Auto: true);
			break;
		case 2:
			flag = PickAutoUseItem(it, Auto: true);
			break;
		}
		if (flag)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		}
	}

	private static bool ShouldXJLPickWeapon(WeaponClass weapon)
	{
		if (weapon != null && SingletonMonoGlobal<FilterManager>.HasInstance)
		{
			return FilterManager.CanPickByQuality(SingletonMonoGlobal<FilterManager>.Instance.SPPick, weapon.Quality);
		}
		return false;
	}

	private static bool ShouldXJLSalvageWeapon(WeaponClass weapon)
	{
		if (weapon != null && SingletonMonoGlobal<FilterManager>.HasInstance)
		{
			return FilterManager.CanSalvageByQuality(SingletonMonoGlobal<FilterManager>.Instance.SPFJ, weapon.Quality);
		}
		return false;
	}

	private bool SalvageXJLWeapon(DropItemController it)
	{
		if (!it || it.weapon == null)
		{
			return false;
		}
		long xJLSalvagePrice = GetXJLSalvagePrice(it.weapon);
		AddMoney(xJLSalvagePrice);
		LeanPool.Despawn(it.gameObject);
		return true;
	}

	private static long GetXJLSalvagePrice(WeaponClass weapon)
	{
		if (weapon == null)
		{
			return 0L;
		}
		float num = (SingletonMonoScope<PlayerManager>.HasInstance ? Mathf.Max(0f, SingletonMonoScope<PlayerManager>.Instance.XJL_SellPrice) : 0f);
		double value = (double)Math.Max(0, weapon.Price) * (1.0 + (double)num / 100.0);
		return Math.Max(0L, (long)Math.Round(value, MidpointRounding.AwayFromZero));
	}

	public void SetSlotData(DropItemController controller)
	{
		switch (controller.ItemType)
		{
		case 0:
		{
			SlotData slotData3 = CheckEmpty(controller.weapon.Size);
			for (int m = 0; m < controller.weapon.Size.y; m++)
			{
				for (int n = 0; n < controller.weapon.Size.x; n++)
				{
					SlotData slotData4 = Page[slotData3.Page].DT[slotData3.GridPos.x + n, slotData3.GridPos.y + m];
					slotData4.Page = slotData3.Page;
					slotData4.ItemSize = controller.weapon.Size;
					ItemCloneUtil.CopyWeaponTo(slotData4.weapon, controller.weapon);
					slotData4.StartPos = slotData3.GridPos;
					slotData4.isOC = true;
					slotData4.ItemType = controller.ItemType;
					if (n == 0 && m == 0)
					{
						slotData4.isMain = true;
						MainPages[slotData3.Page].MainList.Add(slotData4);
					}
					EPPages[slotData3.Page].EPList.Remove(slotData4);
				}
			}
			if (slotData3.Page + 1 != CurPage)
			{
				break;
			}
			ItemScript component2 = LeanPool.Spawn(IVitem).GetComponent<ItemScript>();
			for (int num = 0; num < controller.weapon.Size.y; num++)
			{
				for (int num2 = 0; num2 < controller.weapon.Size.x; num2++)
				{
					Page[CurPage - 1].DT[slotData3.GridPos.x + num2, slotData3.GridPos.y + num].ItemOBJ = component2;
				}
			}
			component2.transform.SetParent(dropParent);
			component2.GetComponent<RectTransform>().pivot = Vector2.up;
			component2.GetComponent<RectTransform>().localScale = Vector3.one;
			component2.transform.position = slotGrid[slotData3.GridPos.x, slotData3.GridPos.y].transform.position;
			component2.page = slotData3.Page;
			component2.saveSlot = new IntVector2(slotData3.GridPos.x, slotData3.GridPos.y);
			component2.SetItem(controller, handTake: false);
			break;
		}
		case 1:
		{
			SlotData slotData5 = CheckEmpty(controller.baoshi.Size);
			for (int num3 = 0; num3 < controller.baoshi.Size.y; num3++)
			{
				for (int num4 = 0; num4 < controller.baoshi.Size.x; num4++)
				{
					SlotData slotData6 = Page[slotData5.Page].DT[slotData5.GridPos.x + num4, slotData5.GridPos.y + num3];
					slotData6.Page = slotData5.Page;
					slotData6.ItemSize = controller.baoshi.Size;
					ItemCloneUtil.CopyBaoshiTo(slotData6.baoshi, controller.baoshi);
					slotData6.StartPos = slotData5.GridPos;
					slotData6.isOC = true;
					slotData6.ItemType = controller.ItemType;
					if (num4 == 0 && num3 == 0)
					{
						slotData6.isMain = true;
						MainPages[slotData5.Page].MainList.Add(slotData6);
					}
					EPPages[slotData5.Page].EPList.Remove(slotData6);
				}
			}
			if (slotData5.Page + 1 != CurPage)
			{
				break;
			}
			ItemScript component3 = LeanPool.Spawn(IVitem).GetComponent<ItemScript>();
			for (int num5 = 0; num5 < controller.baoshi.Size.y; num5++)
			{
				for (int num6 = 0; num6 < controller.baoshi.Size.x; num6++)
				{
					Page[CurPage - 1].DT[slotData5.GridPos.x + num6, slotData5.GridPos.y + num5].ItemOBJ = component3;
				}
			}
			component3.transform.SetParent(dropParent);
			component3.GetComponent<RectTransform>().pivot = Vector2.up;
			component3.GetComponent<RectTransform>().localScale = Vector3.one;
			component3.transform.position = slotGrid[slotData5.GridPos.x, slotData5.GridPos.y].transform.position;
			component3.page = slotData5.Page;
			component3.saveSlot = new IntVector2(slotData5.GridPos.x, slotData5.GridPos.y);
			component3.SetItem(controller, handTake: false);
			break;
		}
		case 2:
		{
			SlotData slotData = CheckEmpty(controller.useitem.Size);
			for (int i = 0; i < controller.useitem.Size.y; i++)
			{
				for (int j = 0; j < controller.useitem.Size.x; j++)
				{
					SlotData slotData2 = Page[slotData.Page].DT[slotData.GridPos.x + j, slotData.GridPos.y + i];
					slotData2.Page = slotData.Page;
					slotData2.ItemSize = controller.useitem.Size;
					ItemCloneUtil.CopyUseItemTo(slotData2.useitem, controller.useitem);
					slotData2.StartPos = slotData.GridPos;
					slotData2.isOC = true;
					slotData2.ItemType = controller.ItemType;
					if (j == 0 && i == 0)
					{
						slotData2.isMain = true;
						MainPages[slotData.Page].MainList.Add(slotData2);
					}
					EPPages[slotData.Page].EPList.Remove(slotData2);
				}
			}
			if (slotData.Page + 1 != CurPage)
			{
				break;
			}
			ItemScript component = LeanPool.Spawn(IVitem).GetComponent<ItemScript>();
			for (int k = 0; k < controller.useitem.Size.y; k++)
			{
				for (int l = 0; l < controller.useitem.Size.x; l++)
				{
					Page[CurPage - 1].DT[slotData.GridPos.x + l, slotData.GridPos.y + k].ItemOBJ = component;
				}
			}
			component.transform.SetParent(dropParent);
			component.GetComponent<RectTransform>().pivot = Vector2.up;
			component.GetComponent<RectTransform>().localScale = Vector3.one;
			component.transform.position = slotGrid[slotData.GridPos.x, slotData.GridPos.y].transform.position;
			component.page = slotData.Page;
			component.saveSlot = new IntVector2(slotData.GridPos.x, slotData.GridPos.y);
			component.SetItem(controller, handTake: false);
			break;
		}
		}
	}

	public SlotData CheckEmpty(IntVector2 itemSizeL)
	{
		return ContainerGridUtil.FindEmptyAcrossPages(Page, EPPages, itemSizeL, inventorySize);
	}

	public SlotData CheckEmptyCur(IntVector2 itemSizeL)
	{
		int num = CurPage - 1;
		if (num < 0 || num >= Page.Count || num >= EPPages.Count)
		{
			return null;
		}
		return ContainerGridUtil.FindEmptyInPage(Page[num], EPPages[num], itemSizeL, inventorySize);
	}

	public SlotData CheckSameBaoshi(DropItemController its)
	{
		if (its?.baoshi == null)
		{
			return null;
		}
		return FindSameStackOrEmptyAcrossPages(1, its.baoshi.ItemName, its.baoshi.Size);
	}

	public SlotData CheckSameUse(DropItemController its)
	{
		if (its?.useitem == null)
		{
			return null;
		}
		return FindSameStackOrEmptyAcrossPages(2, its.useitem.ItemName, its.useitem.Size);
	}

	public SlotData CheckSameBS_sort(BaoshiClass its)
	{
		if (its == null)
		{
			return null;
		}
		return FindSameStackOrEmptyAcrossPages(1, its.ItemName, its.Size);
	}

	public SlotData CheckSameUSE_sort(UseItemClass its)
	{
		if (its == null)
		{
			return null;
		}
		return FindSameStackOrEmptyAcrossPages(2, its.ItemName, its.Size);
	}

	public SlotData CheckSameBS_sortCur(BaoshiClass its)
	{
		if (its == null)
		{
			return null;
		}
		return FindSameStackOrEmptyInCurrentPage(1, its.ItemName, its.Size);
	}

	public SlotData CheckSameUSE_sortCur(UseItemClass its)
	{
		if (its == null)
		{
			return null;
		}
		return FindSameStackOrEmptyInCurrentPage(2, its.ItemName, its.Size);
	}

	private static WPAocao GetEMac(WeaponClass wp)
	{
		foreach (WPAocao item in wp.Aocao)
		{
			if (item.HasAocao && !item.HasBaoshi)
			{
				return item;
			}
		}
		return null;
	}

	public static int GetEMacNumber(WeaponClass wp)
	{
		for (int i = 0; i < wp.Aocao.Count; i++)
		{
			if (wp.Aocao[i].HasAocao && !wp.Aocao[i].HasBaoshi)
			{
				return i;
			}
		}
		return 0;
	}

	private bool TryApplyHeldSpecialBaoshiToWeapon(SlotData slot)
	{
		if (slot == null || Hand.Instance.baoshi == null)
		{
			return false;
		}
		SlotData slotData = ResolveWeaponMainSlot(slot);
		if (slotData == null || slotData.weapon == null)
		{
			return false;
		}
		if (!WeaponBaoshiApplyUtil.TryApply(slotData.weapon, Hand.Instance.baoshi, out var refreshSocketDisplay))
		{
			return false;
		}
		SyncWeaponRegion(slotData);
		FinishHeldBaoshiWeaponApply(slotData, refreshSocketDisplay);
		return true;
	}

	private SlotData ResolveWeaponMainSlot(SlotData slot)
	{
		if (slot == null)
		{
			return null;
		}
		int num = ((slot.Page >= 0) ? slot.Page : (CurPage - 1));
		if (num < 0 || num >= Page.Count || Page[num]?.DT == null)
		{
			return slot;
		}
		IntVector2 startPos = slot.StartPos;
		if (startPos.x < 0 || startPos.y < 0 || startPos.x >= Page[num].DT.GetLength(0) || startPos.y >= Page[num].DT.GetLength(1))
		{
			return slot;
		}
		return Page[num].DT[startPos.x, startPos.y] ?? slot;
	}

	private void SyncWeaponRegion(SlotData mainSlot)
	{
		if (mainSlot == null || mainSlot.weapon == null)
		{
			return;
		}
		int page = mainSlot.Page;
		if (page < 0 || page >= Page.Count || Page[page]?.DT == null)
		{
			return;
		}
		WeaponClass weapon = mainSlot.weapon;
		IntVector2 startPos = mainSlot.StartPos;
		IntVector2 size = weapon.Size;
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				int num = startPos.x + j;
				int num2 = startPos.y + i;
				if (num >= 0 && num2 >= 0 && num < Page[page].DT.GetLength(0) && num2 < Page[page].DT.GetLength(1))
				{
					SlotData slotData = Page[page].DT[num, num2];
					if (slotData?.weapon != null && slotData.weapon != weapon)
					{
						ItemCloneUtil.CopyWeaponTo(slotData.weapon, weapon);
					}
				}
			}
		}
	}

	private void FinishHeldBaoshiWeaponApply(SlotData slot, bool refreshSocketDisplay)
	{
		PlayHeldBaoshiUseSound();
		EnsureCurrentPageItemObj(slot);
		if ((bool)slot.ItemOBJ && refreshSocketDisplay)
		{
			slot.ItemOBJ.SetWP(slot.weapon);
			slot.ItemOBJ.RefreshBS(slot);
		}
		SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(slot.weapon, slot, slotGrid);
		Sector.sec.SetPosOffset();
		if (Hand.Instance.baoshi.CstackSize > 1)
		{
			Hand.Instance.baoshi.CstackSize--;
			if ((bool)Hand.Instance.ItemOBJ)
			{
				Hand.Instance.ItemOBJ.RefreshStackHand(0);
			}
		}
		else
		{
			Hand.Instance.DELItem();
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
			SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(slot.weapon, slot, slotGrid);
		}
	}

	public bool TryApplyHeldBaoshiToEquipment(CharButton targetButton)
	{
		if (!targetButton || !targetButton.hasWeapon || targetButton.weapon == null)
		{
			return false;
		}
		if (!Hand.Instance || !Hand.Instance.isDragItem || Hand.Instance.itemType != 1 || Hand.Instance.baoshi == null)
		{
			return false;
		}
		WeaponClass weaponClass = ItemCloneUtil.CloneWeapon(targetButton.weapon);
		if (weaponClass == null || !WeaponBaoshiApplyUtil.TryApply(weaponClass, Hand.Instance.baoshi, out var refreshSocketDisplay))
		{
			return false;
		}
		targetButton.weapon.Equip(1);
		ItemCloneUtil.CopyWeaponTo(targetButton.weapon, weaponClass);
		targetButton.weapon.Equip(0);
		if ((bool)targetButton.ItemOBJ)
		{
			targetButton.ItemOBJ.SetWP(targetButton.weapon);
			if (refreshSocketDisplay && MouseCharBT == targetButton)
			{
				targetButton.ItemOBJ.RefreshSocketDisplay(targetButton.weapon, showEmptySockets: true);
			}
		}
		PlayHeldBaoshiUseSound();
		ConsumeHeldBaoshi();
		if ((bool)Sector.sec)
		{
			Sector.sec.SetPosOffset();
		}
		RefreshColor(enter: true);
		targetButton.ShowAocao();
		SingletonMonoScope<GameUIManager>.Instance.ShowWPTipB(targetButton.transform.position, targetButton.weapon);
		return true;
	}

	private void ConsumeHeldBaoshi()
	{
		if (!Hand.Instance || Hand.Instance.baoshi == null)
		{
			return;
		}
		if (Hand.Instance.baoshi.CstackSize > 1)
		{
			Hand.Instance.baoshi.CstackSize--;
			if ((bool)Hand.Instance.ItemOBJ)
			{
				Hand.Instance.ItemOBJ.RefreshStackHand(0);
			}
		}
		else
		{
			Hand.Instance.DELItem();
		}
	}

	private void PlayHeldBaoshiUseSound()
	{
		BaoshiClass baoshi = Hand.Instance.baoshi;
		if (baoshi != null)
		{
			if (!_audioManager && SingletonMonoGlobal<AudioManager>.HasInstance)
			{
				_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
			}
			if (_audioManager?.audioData?.Baoshi?.Use != null && _audioManager.audioData.Baoshi.Use.Length != 0)
			{
				int num = Mathf.Clamp(baoshi.SoundUse, 0, _audioManager.audioData.Baoshi.Use.Length - 1);
				RuntimeManager.PlayOneShot(_audioManager.audioData.Baoshi.Use[num]);
			}
		}
	}

	public void XiangQian(WeaponClass wp, BaoshiClass BS, int index)
	{
		for (int i = 0; i < wp.Size.y; i++)
		{
			for (int j = 0; j < wp.Size.x; j++)
			{
				SlotData slotData = Page[CurPage - 1].DT[base.MouseSlotDT.StartPos.x + j, base.MouseSlotDT.StartPos.y + i];
				slotData.weapon.Price += BS.Price;
				slotData.weapon.Aocao[index].HasBaoshi = true;
				slotData.weapon.Aocao[index].Icon = BS.Icon;
				slotData.weapon.Aocao[index].Name = BS.ItemName;
				slotData.weapon.Aocao[index].Number = BS.Number;
				slotData.weapon.Aocao[index].UseType = BS.UseType;
				slotData.weapon.Aocao[index].BS_Quality = BS.BS_Quality;
				switch (BS.BStype)
				{
				case "red":
					switch (wp.WeaponType)
					{
					case "head":
					case "leg":
						slotData.weapon.Aocao[index].Type = 0;
						break;
					case "body":
						slotData.weapon.Aocao[index].Type = 1;
						break;
					case "hand":
						slotData.weapon.Aocao[index].Type = 2;
						break;
					case "bone":
					case "bow":
					case "sword":
					case "staff":
					case "arrow":
					case "spell":
					case "corpse":
					case "shield":
						slotData.weapon.Aocao[index].Type = 3;
						break;
					}
					break;
				case "yellow":
					switch (wp.WeaponType)
					{
					case "head":
					case "leg":
						slotData.weapon.Aocao[index].Type = 4;
						break;
					case "body":
						slotData.weapon.Aocao[index].Type = 5;
						break;
					case "hand":
						slotData.weapon.Aocao[index].Type = 6;
						break;
					case "bone":
					case "bow":
					case "sword":
					case "staff":
					case "arrow":
					case "spell":
					case "corpse":
					case "shield":
						slotData.weapon.Aocao[index].Type = 7;
						break;
					}
					break;
				case "green":
					switch (wp.WeaponType)
					{
					case "head":
						slotData.weapon.Aocao[index].Type = 8;
						break;
					case "body":
						slotData.weapon.Aocao[index].Type = 9;
						break;
					case "hand":
						slotData.weapon.Aocao[index].Type = 10;
						break;
					case "leg":
						slotData.weapon.Aocao[index].Type = 11;
						break;
					case "bone":
					case "bow":
					case "sword":
					case "staff":
					case "arrow":
					case "spell":
					case "corpse":
					case "shield":
						slotData.weapon.Aocao[index].Type = 12;
						break;
					}
					break;
				case "blue":
					switch (wp.WeaponType)
					{
					case "head":
					case "leg":
						slotData.weapon.Aocao[index].Type = 13;
						break;
					case "body":
						slotData.weapon.Aocao[index].Type = 14;
						break;
					case "hand":
						slotData.weapon.Aocao[index].Type = 15;
						break;
					case "bone":
					case "bow":
					case "sword":
					case "staff":
					case "arrow":
					case "spell":
					case "corpse":
					case "shield":
						slotData.weapon.Aocao[index].Type = 16;
						break;
					}
					break;
				case "purple":
					switch (wp.WeaponType)
					{
					case "head":
						slotData.weapon.Aocao[index].Type = 17;
						break;
					case "body":
						slotData.weapon.Aocao[index].Type = 18;
						break;
					case "hand":
						slotData.weapon.Aocao[index].Type = 19;
						break;
					case "leg":
						slotData.weapon.Aocao[index].Type = 20;
						break;
					case "bone":
					case "bow":
					case "sword":
					case "staff":
					case "arrow":
					case "spell":
					case "corpse":
					case "shield":
						slotData.weapon.Aocao[index].Type = 21;
						break;
					}
					break;
				case "white":
					switch (wp.WeaponType)
					{
					case "head":
					case "leg":
						slotData.weapon.Aocao[index].Type = 22;
						break;
					case "body":
						slotData.weapon.Aocao[index].Type = 23;
						break;
					case "hand":
						slotData.weapon.Aocao[index].Type = 24;
						break;
					case "bone":
					case "bow":
					case "sword":
					case "staff":
					case "arrow":
					case "spell":
					case "corpse":
					case "shield":
						slotData.weapon.Aocao[index].Type = 25;
						break;
					}
					break;
				}
			}
		}
	}

	public void UseItem()
	{
		UseItemClass useitem = base.MouseSlotDT.useitem;
		int soundUse = useitem.SoundUse;
		int infoType = useitem.InfoType;
		if (!useitem.Use())
		{
			return;
		}
		if (PoeItemMod.IsRepeatableFlask(useitem))
		{
			_audioManager.PlaySO_UseItem(soundUse, infoType);
			return;
		}
		IntVector2 intVector = new IntVector2(MouseSlot.GridPos.x, MouseSlot.GridPos.y);
		if (useitem.CstackSize > 1)
		{
			for (int i = 0; i < useitem.Size.y; i++)
			{
				for (int j = 0; j < useitem.Size.x; j++)
				{
					Page[CurPage - 1].DT[intVector.x + j, intVector.y + i].useitem.CstackSize--;
				}
			}
			base.MouseSlotDT.ItemOBJ.RefreshStackIV(1);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useitem);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
		else
		{
			ThrowItem();
		}
		_audioManager.PlaySO_UseItem(soundUse, infoType);
	}

	public void UseItemACT(string itemName, int index)
	{
		SlotData slotData = ReturnSameUse(itemName);
		if (slotData != null)
		{
			switch (slotData.useitem.UseType)
			{
			case "health":
				if (PL.HealStat.Cur < PL.HealStat.Max)
				{
					UseItemACT_Use(slotData, index);
				}
				break;
			case "mana":
				if (PL.ManaStat.Cur < PL.ManaStat.Max)
				{
					UseItemACT_Use(slotData, index);
				}
				break;
			case "huoli":
				if (PL.HealStat.Cur < PL.HealStat.Max || PL.ManaStat.Cur < PL.ManaStat.Max)
				{
					UseItemACT_Use(slotData, index);
				}
				break;
			case "green":
				if (!CheckScrollUseLimit(checkHomeScene: false))
				{
					GameManager.ShowTip(LOC.MM.GetLevel("scroll_hint_no"), TipType.Fail);
					return;
				}
				UseItemACT_Use(slotData, index);
				break;
			case "blue":
				if (!CheckScrollUseLimit(checkHomeScene: false))
				{
					GameManager.ShowTip(LOC.MM.GetLevel("scroll_hint_no"), TipType.Fail);
					return;
				}
				UseItemACT_Use(slotData, index);
				break;
			case "purple":
				if (!CheckScrollUseLimit(checkHomeScene: false))
				{
					GameManager.ShowTip(LOC.MM.GetLevel("scroll_hint_no"), TipType.Fail);
					return;
				}
				UseItemACT_Use(slotData, index);
				break;
			case "red":
				if (!CheckScrollUseLimit(checkHomeScene: false))
				{
					GameManager.ShowTip(LOC.MM.GetLevel("scroll_hint_no"), TipType.Fail);
					return;
				}
				UseItemACT_Use(slotData, index);
				break;
			case "yellow":
				if (!CheckScrollUseLimit(checkHomeScene: false))
				{
					GameManager.ShowTip(LOC.MM.GetLevel("scroll_hint_no"), TipType.Fail);
					return;
				}
				UseItemACT_Use(slotData, index);
				break;
			case "poe_flask_gale":
			case "poe_flask_insight":
				UseItemACT_Use(slotData, index);
				break;
			case "yiwang":
				if (SingletonMonoScope<TalentManager>.Instance.P_Used > 0)
				{
					UseItemACT_Use(slotData, index);
				}
				break;
			case "lunhui":
				if (SingletonMonoScope<TalentManager>.Instance.HasUsedDFTalentPoint())
				{
					UseItemACT_Use(slotData, index);
				}
				break;
			case "shenyou":
				UseItemACT_Use(slotData, index);
				break;
			case "juexing":
				UseItemACT_Use(slotData, index);
				break;
			}
		}
		else if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			if (!SingletonMonoScope<ACTbar>.Instance.GetAutoReplaceUseBinding())
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
				return;
			}
			SingletonMonoScope<ACTbar>.Instance.ExchangeUseFromActbar(index);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			return;
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
	}

	public static bool CheckScrollUseLimit(bool checkHomeScene)
	{
		if (checkHomeScene && SceneManager.GetActiveScene().name == "HomeScene")
		{
			return false;
		}
		if (LevelManager.GetIsChallenge())
		{
			return false;
		}
		if (LevelManager.GetIsBoss())
		{
			return false;
		}
		return true;
	}

	public void UseItemACT_Use(SlotData sl, int index)
	{
		if (sl?.useitem == null || !SingletonMonoScope<SimplePotionManager>.Instance || !SingletonMonoScope<ACTbar>.HasInstance || SingletonMonoScope<SimplePotionManager>.Instance.HasSameDrink(sl.useitem))
		{
			return;
		}
		int cstackSize = sl.useitem.CstackSize;
		IntVector2 intVector = new IntVector2(sl.GridPos.x, sl.GridPos.y);
		UseItemClass useitem = sl.useitem;
		int soundUse = useitem.SoundUse;
		int infoType = useitem.InfoType;
		if (!useitem.Use())
		{
			return;
		}
		if (PoeItemMod.IsRepeatableFlask(useitem))
		{
			_audioManager.PlaySO_UseItem(soundUse, infoType);
		}
		else if (cstackSize > 1)
		{
			for (int i = 0; i < useitem.Size.y; i++)
			{
				for (int j = 0; j < useitem.Size.x; j++)
				{
					Page[sl.Page].DT[intVector.x + j, intVector.y + i].useitem.CstackSize--;
				}
			}
			if ((bool)sl.ItemOBJ)
			{
				sl.ItemOBJ.RefreshStackIV(1);
			}
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useitem);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			_audioManager.PlaySO_UseItem(soundUse, infoType);
		}
		else
		{
			ItemScript itemOBJ = sl.ItemOBJ;
			ContainerGridUtil.ClearRegion(sl, Page, EPPages, MainPages, slotGrid, SlotColor.TouMing);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useitem);
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			if ((bool)itemOBJ)
			{
				LeanPool.Despawn(itemOBJ);
			}
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
			_audioManager.PlaySO_UseItem(soundUse, infoType);
		}
	}

	public bool UseAllDurationBuffPotionsFromShortcut()
	{
		if ((bool)Hand.Instance.ItemOBJ)
		{
			return false;
		}
		if (!SingletonMonoScope<ACTbar>.HasInstance || !SingletonMonoScope<SimplePotionManager>.HasInstance)
		{
			return false;
		}
		List<SlotData> list = new List<SlotData>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (MainSlotPage mainPage in MainPages)
		{
			if (mainPage?.MainList == null)
			{
				continue;
			}
			foreach (SlotData main in mainPage.MainList)
			{
				if (main != null && main.isMain && main.isOC && main.ItemType == 2 && main.useitem != null)
				{
					UseItemClass useitem = main.useitem;
					if (useitem.InfoType == 1 && useitem.Duration > 0 && useitem.CstackSize > 0 && hashSet.Add(useitem.ItemName))
					{
						list.Add(main);
					}
				}
			}
		}
		int num = 0;
		foreach (SlotData item in list)
		{
			if (item != null && item.isMain && item.isOC && item.ItemType == 2 && item.useitem != null && item.useitem.InfoType == 1 && item.useitem.Duration > 0 && item.useitem.CstackSize > 0)
			{
				int cstackSize = item.useitem.CstackSize;
				UseItemACT_Use(item, -1);
				if (!item.isOC || item.ItemType != 2 || item.useitem == null || item.useitem.CstackSize < cstackSize)
				{
					num++;
				}
			}
		}
		return num > 0;
	}

	public SlotData ReturnSameUse(string itemName)
	{
		foreach (MainSlotPage mainPage in MainPages)
		{
			foreach (SlotData main in mainPage.MainList)
			{
				if (main.ItemType == 2 && main.useitem.ItemName == itemName)
				{
					return main;
				}
			}
		}
		return null;
	}

	public void SortAutoWeapon(WeaponClass wp, bool single)
	{
		if (wp == null)
		{
			return;
		}
		SlotData slotData = (single ? CheckEmptyCur(wp.Size) : CheckEmpty(wp.Size));
		if (slotData == null)
		{
			SingletonMonoScope<ItemManager>.Instance.ThrowWP(wp);
			return;
		}
		SlotData slotData2 = Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y];
		if (slotData2 == null)
		{
			SingletonMonoScope<ItemManager>.Instance.ThrowWP(wp);
			return;
		}
		slotData2.Page = slotData.Page;
		slotData2.StartPos = slotData.GridPos;
		slotData2.ItemSize = wp.Size;
		slotData2.ItemType = wp.ItemType;
		slotData2.isOC = true;
		slotData2.isMain = true;
		ItemCloneUtil.CopyWeaponTo(slotData2.weapon, wp);
		ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
		if (slotData2.Page + 1 == CurPage)
		{
			ContainerPageViewUtil.SpawnItemUI(slotData2, CurPage, IVitem, dropParent, slotGrid, Page, 0);
		}
	}

	public void SortAutoBaoshi(BaoshiClass bs, bool single)
	{
		if (bs == null)
		{
			return;
		}
		while (true)
		{
			SlotData slotData = (single ? CheckSameBS_sortCur(bs) : CheckSameBS_sort(bs));
			if (slotData == null)
			{
				break;
			}
			if (slotData.isOC)
			{
				int num = slotData.baoshi.MstackSize - slotData.baoshi.CstackSize;
				if (bs.CstackSize <= num)
				{
					slotData.baoshi.CstackSize += bs.CstackSize;
					if ((bool)slotData.ItemOBJ)
					{
						slotData.ItemOBJ.RefreshStackIV(0);
					}
					break;
				}
				slotData.baoshi.CstackSize = slotData.baoshi.MstackSize;
				bs.CstackSize -= num;
				if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackIV(0);
				}
				continue;
			}
			SlotData slotData2 = Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y];
			if (slotData2 != null)
			{
				slotData2.Page = slotData.Page;
				slotData2.StartPos = slotData.GridPos;
				slotData2.ItemSize = bs.Size;
				slotData2.ItemType = bs.ItemType;
				slotData2.isOC = true;
				slotData2.isMain = true;
				ItemCloneUtil.CopyBaoshiTo(slotData2.baoshi, bs);
				ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
				if (slotData2.Page + 1 == CurPage)
				{
					ContainerPageViewUtil.SpawnItemUI(slotData2, CurPage, IVitem, dropParent, slotGrid, Page, 0);
				}
			}
			break;
		}
	}

	public void SortAutoUseItem(UseItemClass use, bool single)
	{
		if (use == null)
		{
			return;
		}
		while (true)
		{
			SlotData slotData = (single ? CheckSameUSE_sortCur(use) : CheckSameUSE_sort(use));
			if (slotData == null)
			{
				break;
			}
			if (slotData.isOC)
			{
				int num = slotData.useitem.MstackSize - slotData.useitem.CstackSize;
				if (use.CstackSize <= num)
				{
					slotData.useitem.CstackSize += use.CstackSize;
					if ((bool)slotData.ItemOBJ)
					{
						slotData.ItemOBJ.RefreshStackIV(1);
					}
					break;
				}
				slotData.useitem.CstackSize = slotData.useitem.MstackSize;
				use.CstackSize -= num;
				if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackIV(1);
				}
				continue;
			}
			SlotData slotData2 = Page[slotData.Page].DT[slotData.GridPos.x, slotData.GridPos.y];
			if (slotData2 != null)
			{
				slotData2.Page = slotData.Page;
				slotData2.StartPos = slotData.GridPos;
				slotData2.ItemSize = use.Size;
				slotData2.ItemType = use.ItemType;
				slotData2.isOC = true;
				slotData2.isMain = true;
				ItemCloneUtil.CopyUseItemTo(slotData2.useitem, use);
				ContainerGridUtil.OccupyRegion(slotData2, Page, EPPages, MainPages);
				if (slotData2.Page + 1 == CurPage)
				{
					ContainerPageViewUtil.SpawnItemUI(slotData2, CurPage, IVitem, dropParent, slotGrid, Page, 0);
				}
			}
			break;
		}
	}

	public void SortAll()
	{
		RuntimeManager.PlayOneShot(_audioManager.audioData.IV_Organize_2);
		int num = 0;
		foreach (EmptySlotPage ePPage in EPPages)
		{
			if (ePPage?.EPList != null)
			{
				num += ePPage.EPList.Count;
			}
		}
		if (Hand.Instance.isDragItem || num <= 1)
		{
			return;
		}
		List<SlotData> list = new List<SlotData>();
		foreach (MainSlotPage mainPage in MainPages)
		{
			if (mainPage?.MainList != null)
			{
				list.AddRange(mainPage.MainList);
			}
		}
		foreach (SlotData item in list)
		{
			if (item != null && item.isMain)
			{
				CacheSortItem(item);
				ItemScript itemOBJ = item.ItemOBJ;
				if ((bool)itemOBJ)
				{
					LeanPool.Despawn(itemOBJ);
				}
				ContainerGridUtil.ClearRegion(item, Page, EPPages, MainPages);
			}
		}
		SortAllBucketsByPrice();
		RebuildAllPagesByBuckets();
		ClearAllBuckets();
		RebindVisibleItemObjRegions();
		EnsureCurrentPageItemObjs();
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RebuildUseListFromInventory();
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
	}

	public void SortCur()
	{
		RuntimeManager.PlayOneShot(_audioManager.audioData.IV_Organize_1);
		int index = CurPage - 1;
		if (Hand.Instance.isDragItem || EPPages[index].EPList.Count <= 1)
		{
			return;
		}
		foreach (SlotData item in new List<SlotData>(MainPages[index].MainList))
		{
			if (item != null && item.isMain)
			{
				CacheSortItem(item);
				ItemScript itemOBJ = item.ItemOBJ;
				if ((bool)itemOBJ)
				{
					LeanPool.Despawn(itemOBJ);
				}
				ContainerGridUtil.ClearRegion(item, Page, EPPages, MainPages);
			}
		}
		SortAllBucketsByPrice();
		RebuildCurPageByBuckets();
		ClearAllBuckets();
		RebindVisibleItemObjRegions();
		EnsureCurrentPageItemObjs();
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RebuildUseListFromInventory();
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
	}

	public void ApplySort(InventorySortMode mode)
	{
		try
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.IV_Organize_1);
			if (Hand.Instance.isDragItem)
			{
				return;
			}
			List<SortEntry> list = new List<SortEntry>();
			int num = 0;
			foreach (MainSlotPage mainPage in MainPages)
			{
				if (mainPage?.MainList == null)
				{
					continue;
				}
				foreach (SlotData item2 in new List<SlotData>(mainPage.MainList))
				{
					if (item2 == null || !item2.isMain)
					{
						continue;
					}
					SortEntry item = default(SortEntry);
					item.ItemType = item2.ItemType;
					item.Seq = num++;
					switch (item2.ItemType)
					{
					case 0:
						if (item2.weapon == null)
						{
							continue;
						}
						item.Wp = ItemCloneUtil.CloneWeapon(item2.weapon);
						break;
					case 1:
						if (item2.baoshi == null)
						{
							continue;
						}
						item.Bs = ItemCloneUtil.CloneBaoshi(item2.baoshi);
						break;
					case 2:
						if (item2.useitem == null)
						{
							continue;
						}
						item.Use = ItemCloneUtil.CloneUseItem(item2.useitem);
						break;
					default:
						continue;
					}
					list.Add(item);
					ItemScript itemOBJ = item2.ItemOBJ;
					if ((bool)itemOBJ)
					{
						LeanPool.Despawn(itemOBJ);
					}
					ContainerGridUtil.ClearRegion(item2, Page, EPPages, MainPages);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			list.Sort((SortEntry a, SortEntry b) => CompareSortEntries(a, b, mode));
			foreach (SortEntry item3 in list)
			{
				switch (item3.ItemType)
				{
				case 0:
					SortAutoWeapon(item3.Wp, single: false);
					break;
				case 1:
					SortAutoBaoshi(item3.Bs, single: false);
					break;
				case 2:
					SortAutoUseItem(item3.Use, single: false);
					break;
				}
			}
			RebindVisibleItemObjRegions();
			EnsureCurrentPageItemObjs();
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RebuildUseListFromInventory();
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			}
		}
		catch (Exception ex)
		{
			LogUtil.Info("ApplySort 异常: " + ex);
		}
	}

	private static int CompareSortEntries(SortEntry a, SortEntry b, InventorySortMode mode)
	{
		int num = (int)mode / 2;
		bool flag = (int)mode % 2 == 0;
		int num2 = 0;
		switch (num)
		{
		case 0:
			num2 = ((!flag) ? QualityOf(b).CompareTo(QualityOf(a)) : QualityOf(a).CompareTo(QualityOf(b)));
			break;
		case 1:
			num2 = ((!flag) ? LevelOf(b).CompareTo(LevelOf(a)) : LevelOf(a).CompareTo(LevelOf(b)));
			break;
		}
		if (num2 != 0)
		{
			return num2;
		}
		num2 = PriceOf(b).CompareTo(PriceOf(a));
		if (num2 != 0)
		{
			return num2;
		}
		return a.Seq.CompareTo(b.Seq);
	}

	private static int QualityOf(SortEntry e)
	{
		switch (e.ItemType)
		{
		case 0:
			if (e.Wp == null)
			{
				return 0;
			}
			return e.Wp.Quality;
		case 1:
			if (e.Bs == null)
			{
				return 0;
			}
			return e.Bs.Quality;
		default:
			if (e.Use == null)
			{
				return 0;
			}
			return e.Use.Quality;
		}
	}

	private static int LevelOf(SortEntry e)
	{
		switch (e.ItemType)
		{
		case 0:
			if (e.Wp == null)
			{
				return 0;
			}
			return e.Wp.Level;
		case 1:
			if (e.Bs == null)
			{
				return 0;
			}
			return e.Bs.Level;
		default:
			if (e.Use == null)
			{
				return 0;
			}
			return e.Use.Level;
		}
	}

	private static int PriceOf(SortEntry e)
	{
		switch (e.ItemType)
		{
		case 0:
			if (e.Wp == null)
			{
				return 0;
			}
			return e.Wp.Price;
		case 1:
			if (e.Bs == null)
			{
				return 0;
			}
			return e.Bs.Price;
		default:
			if (e.Use == null)
			{
				return 0;
			}
			return e.Use.Price;
		}
	}

	private void CacheSortItem(SlotData slot)
	{
		switch (slot.ItemType)
		{
		case 0:
			CacheWeapon(slot.weapon);
			break;
		case 1:
			if (slot.baoshi != null)
			{
				ContainerManager<InventoryManager>.AllBS.AxA.Add(ItemCloneUtil.CloneBaoshi(slot.baoshi));
			}
			break;
		case 2:
			CacheUseItem(slot.useitem);
			break;
		}
	}

	private void CacheWeapon(WeaponClass wp)
	{
		if (wp == null)
		{
			return;
		}
		switch (wp.Size.x)
		{
		case 2:
			if (wp.Size.y == 4)
			{
				ContainerManager<InventoryManager>.AllWP.BxD.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 3)
			{
				ContainerManager<InventoryManager>.AllWP.BxC.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 2)
			{
				ContainerManager<InventoryManager>.AllWP.BxB.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			break;
		case 1:
			if (wp.Size.y == 4)
			{
				ContainerManager<InventoryManager>.AllWP.AxD.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 3)
			{
				ContainerManager<InventoryManager>.AllWP.AxC.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 1)
			{
				ContainerManager<InventoryManager>.AllWP.AxA.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			break;
		}
	}

	private void CacheUseItem(UseItemClass use)
	{
		if (use == null)
		{
			return;
		}
		switch (use.Size.x)
		{
		case 2:
			if (use.Size.y == 2)
			{
				ContainerManager<InventoryManager>.AllUSE.BxB.Add(ItemCloneUtil.CloneUseItem(use));
			}
			break;
		case 1:
			if (use.Size.y == 1)
			{
				ContainerManager<InventoryManager>.AllUSE.AxA.Add(ItemCloneUtil.CloneUseItem(use));
			}
			break;
		}
	}

	private void SortAllBucketsByPrice()
	{
		SortWeaponBucket(ContainerManager<InventoryManager>.AllWP.BxD);
		SortWeaponBucket(ContainerManager<InventoryManager>.AllWP.BxC);
		SortWeaponBucket(ContainerManager<InventoryManager>.AllWP.AxD);
		SortWeaponBucket(ContainerManager<InventoryManager>.AllWP.AxC);
		SortWeaponBucket(ContainerManager<InventoryManager>.AllWP.BxB);
		SortWeaponBucket(ContainerManager<InventoryManager>.AllWP.AxA);
		SortOneSlotItemBucket(ContainerManager<InventoryManager>.AllBS.AxA);
		SortUseItemBucket(ContainerManager<InventoryManager>.AllUSE.BxB);
		SortOneSlotItemBucket(ContainerManager<InventoryManager>.AllUSE.AxA);
	}

	private static void SortWeaponBucket(List<WeaponClass> list)
	{
		if (list != null && list.Count > 1)
		{
			list.Sort((WeaponClass a, WeaponClass b) => b.Price.CompareTo(a.Price));
		}
	}

	private static void SortBaoshiBucket(List<BaoshiClass> list)
	{
		if (list != null && list.Count > 1)
		{
			list.Sort((BaoshiClass a, BaoshiClass b) => b.Price.CompareTo(a.Price));
		}
	}

	private static void SortUseItemBucket(List<UseItemClass> list)
	{
		if (list != null && list.Count > 1)
		{
			list.Sort((UseItemClass a, UseItemClass b) => b.Price.CompareTo(a.Price));
		}
	}

	private static void SortOneSlotItemBucket<T>(List<T> list) where T : ItemClass
	{
		if (list != null && list.Count > 1)
		{
			list.Sort(delegate(T a, T b)
			{
				int num = a.GlobalID.CompareTo(b.GlobalID);
				return (num == 0) ? b.Price.CompareTo(a.Price) : num;
			});
		}
	}

	private void RebuildAllPagesByBuckets()
	{
		RebuildWeaponBucketAll(ContainerManager<InventoryManager>.AllWP.BxD);
		RebuildWeaponBucketAll(ContainerManager<InventoryManager>.AllWP.BxC);
		RebuildWeaponBucketAll(ContainerManager<InventoryManager>.AllWP.AxD);
		RebuildWeaponBucketAll(ContainerManager<InventoryManager>.AllWP.AxC);
		RebuildWeaponBucketAll(ContainerManager<InventoryManager>.AllWP.BxB);
		RebuildUseItemBucketAll(ContainerManager<InventoryManager>.AllUSE.BxB);
		RebuildWeaponBucketAll(ContainerManager<InventoryManager>.AllWP.AxA);
		RebuildBaoshiBucketAll(ContainerManager<InventoryManager>.AllBS.AxA);
		RebuildUseItemBucketAll(ContainerManager<InventoryManager>.AllUSE.AxA);
	}

	private void RebuildWeaponBucketAll(List<WeaponClass> list)
	{
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (WeaponClass item in list)
		{
			SortAutoWeapon(item, single: false);
		}
	}

	private void RebuildBaoshiBucketAll(List<BaoshiClass> list)
	{
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (BaoshiClass item in list)
		{
			SortAutoBaoshi(item, single: false);
		}
	}

	private void RebuildUseItemBucketAll(List<UseItemClass> list)
	{
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (UseItemClass item in list)
		{
			SortAutoUseItem(item, single: false);
		}
	}

	private void RebuildCurPageByBuckets()
	{
		RebuildWeaponBucket(ContainerManager<InventoryManager>.AllWP.BxD);
		RebuildWeaponBucket(ContainerManager<InventoryManager>.AllWP.BxC);
		RebuildWeaponBucket(ContainerManager<InventoryManager>.AllWP.AxD);
		RebuildWeaponBucket(ContainerManager<InventoryManager>.AllWP.AxC);
		RebuildWeaponBucket(ContainerManager<InventoryManager>.AllWP.BxB);
		RebuildUseItemBucket(ContainerManager<InventoryManager>.AllUSE.BxB);
		RebuildWeaponBucket(ContainerManager<InventoryManager>.AllWP.AxA);
		RebuildBaoshiBucket(ContainerManager<InventoryManager>.AllBS.AxA);
		RebuildUseItemBucket(ContainerManager<InventoryManager>.AllUSE.AxA);
	}

	private void RebuildWeaponBucket(List<WeaponClass> list)
	{
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (WeaponClass item in list)
		{
			SortAutoWeapon(item, single: true);
		}
	}

	private void RebuildBaoshiBucket(List<BaoshiClass> list)
	{
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (BaoshiClass item in list)
		{
			SortAutoBaoshi(item, single: true);
		}
	}

	private void RebuildUseItemBucket(List<UseItemClass> list)
	{
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (UseItemClass item in list)
		{
			SortAutoUseItem(item, single: true);
		}
	}

	private static void ClearAllBuckets()
	{
		ContainerManager<InventoryManager>.AllWP.BxD.Clear();
		ContainerManager<InventoryManager>.AllWP.BxC.Clear();
		ContainerManager<InventoryManager>.AllWP.AxD.Clear();
		ContainerManager<InventoryManager>.AllWP.AxC.Clear();
		ContainerManager<InventoryManager>.AllWP.BxB.Clear();
		ContainerManager<InventoryManager>.AllWP.AxA.Clear();
		ContainerManager<InventoryManager>.AllBS.AxA.Clear();
		ContainerManager<InventoryManager>.AllUSE.BxB.Clear();
		ContainerManager<InventoryManager>.AllUSE.AxA.Clear();
	}

	public bool ChesttoIV(WeaponClass wp)
	{
		if (wp == null)
		{
			return false;
		}
		_audioManager.PlaySO_Item(0, wp.WeaponType, wp.SoundDrop, 0);
		return TryPlaceWeapon(wp);
	}

	public TransferResult ChesttoIV(BaoshiClass bs, SlotData it, bool allowHandOverflow = true)
	{
		TransferResult transferResult = new TransferResult();
		if (bs == null)
		{
			return transferResult;
		}
		int cstackSize = bs.CstackSize;
		_audioManager.PlaySO_Item(1, bs.BStype, bs.SoundDrop, 0);
		if (TryStackBaoshi(bs, CheckSameBS_sort, delegate(SlotData sd)
		{
			if ((bool)sd?.ItemOBJ)
			{
				sd.ItemOBJ.RefreshStackIV(0);
			}
		}) || TryPlaceBaoshi(bs, 0))
		{
			transferResult.Success = true;
			transferResult.MovedCount = cstackSize;
			transferResult.IsComplete = true;
			return transferResult;
		}
		ItemScript itemScript = SpawnLooseItemObj();
		if (allowHandOverflow && !Hand.Instance.ItemOBJ && (bool)itemScript)
		{
			Hand.Instance.BuyItemSurplus(it, itemScript);
			transferResult.Success = true;
			transferResult.MovedCount = cstackSize;
			transferResult.IsComplete = true;
			return transferResult;
		}
		if ((bool)itemScript)
		{
			LeanPool.Despawn(itemScript);
		}
		int num = cstackSize - bs.CstackSize;
		if (num > 0)
		{
			transferResult.Success = true;
			transferResult.MovedCount = num;
			transferResult.IsComplete = false;
		}
		return transferResult;
	}

	public TransferResult ChesttoIV(UseItemClass use, SlotData it, bool allowHandOverflow = true)
	{
		TransferResult transferResult = new TransferResult();
		if (use == null)
		{
			return transferResult;
		}
		int cstackSize = use.CstackSize;
		_audioManager.PlaySO_Item(2, use.UseType, use.SoundDrop, use.InfoType);
		if (TryStackUseItem(use, CheckSameUSE_sort, null, delegate(SlotData sd)
		{
			if ((bool)sd?.ItemOBJ)
			{
				sd.ItemOBJ.RefreshStackIV(1);
			}
		}) || TryPlaceUseItem(use, 0))
		{
			transferResult.Success = true;
			transferResult.MovedCount = cstackSize;
			transferResult.IsComplete = true;
			return transferResult;
		}
		ItemScript itemScript = SpawnLooseItemObj();
		if (allowHandOverflow && !Hand.Instance.ItemOBJ && (bool)itemScript)
		{
			Hand.Instance.BuyItemSurplus(it, itemScript);
			transferResult.Success = true;
			transferResult.MovedCount = cstackSize;
			transferResult.IsComplete = true;
			return transferResult;
		}
		if ((bool)itemScript)
		{
			LeanPool.Despawn(itemScript);
		}
		int num = cstackSize - use.CstackSize;
		if (num > 0)
		{
			transferResult.Success = true;
			transferResult.MovedCount = num;
			transferResult.IsComplete = false;
		}
		return transferResult;
	}

	public void DelItem(SlotData sl)
	{
		if (sl != null)
		{
			ItemScript itemOBJ = sl.ItemOBJ;
			ContainerGridUtil.ClearRegion(sl, Page, EPPages, MainPages);
			if ((bool)itemOBJ)
			{
				LeanPool.Despawn(itemOBJ);
			}
		}
	}

	public bool RemoveBaoshiCountInInv(string itemName, int count)
	{
		if (string.IsNullOrEmpty(itemName) || count <= 0)
		{
			return false;
		}
		if (GetBaoshiTotalCountInInv(itemName) < count)
		{
			return false;
		}
		int num = count;
		for (int i = 0; i < MainPages.Count; i++)
		{
			if (num <= 0)
			{
				break;
			}
			MainSlotPage mainSlotPage = MainPages[i];
			if (mainSlotPage?.MainList == null || mainSlotPage.MainList.Count == 0)
			{
				continue;
			}
			List<SlotData> list = new List<SlotData>(mainSlotPage.MainList);
			for (int j = 0; j < list.Count; j++)
			{
				if (num <= 0)
				{
					break;
				}
				SlotData slotData = list[j];
				if (slotData == null || !slotData.isMain || !slotData.isOC || slotData.ItemType != 1 || slotData.baoshi == null || slotData.baoshi.ItemName != itemName)
				{
					continue;
				}
				int cstackSize = slotData.baoshi.CstackSize;
				if (cstackSize > num)
				{
					slotData.baoshi.CstackSize -= num;
					if ((bool)slotData.ItemOBJ)
					{
						slotData.ItemOBJ.RefreshStackIV(0);
					}
					num = 0;
					break;
				}
				num -= cstackSize;
				DelItem(slotData);
			}
		}
		if ((bool)Sector.sec)
		{
			Sector.sec.SetPosOffset();
		}
		return num <= 0;
	}

	public bool TryCreateBaoshiToHand(BaoshiClass baoshiData, int count)
	{
		if (baoshiData == null || count <= 0)
		{
			return false;
		}
		if (!Hand.Instance || (bool)Hand.Instance.ItemOBJ)
		{
			return false;
		}
		ItemScript itemScript = SpawnLooseItemObj();
		if (!itemScript)
		{
			return false;
		}
		BaoshiClass baoshiClass = ItemCloneUtil.CloneBaoshi(baoshiData);
		if (baoshiClass == null)
		{
			LeanPool.Despawn(itemScript);
			return false;
		}
		baoshiClass.CstackSize = count;
		itemScript.SetBS(baoshiClass, 3);
		Hand.Instance.TakeBS(baoshiClass, itemScript);
		itemScript.RefreshStackHand(0);
		return true;
	}

	public void QuickSell()
	{
		if (!SingletonMonoScope<GameUIManager>.Instance.Opened_shop || !MouseSlot || (bool)Hand.Instance.ItemOBJ || base.MouseSlotDT == null || !base.MouseSlotDT.isOC || Hand.Instance.Mpos != 0)
		{
			return;
		}
		SlotData mouseSlotDT = base.MouseSlotDT;
		bool flag = false;
		switch (mouseSlotDT.ItemType)
		{
		case 0:
			flag = SingletonMonoScope<ShopManager>.Instance.TrySellToShop(mouseSlotDT.weapon);
			break;
		case 1:
			flag = SingletonMonoScope<ShopManager>.Instance.TrySellToShop(mouseSlotDT.baoshi);
			break;
		case 2:
			flag = SingletonMonoScope<ShopManager>.Instance.TrySellToShop(mouseSlotDT.useitem);
			break;
		}
		if (flag)
		{
			UseItemClass useItemClass = null;
			if (mouseSlotDT.ItemType == 2 && mouseSlotDT.useitem != null)
			{
				useItemClass = ItemCloneUtil.CloneUseItem(mouseSlotDT.useitem);
			}
			DelItem(mouseSlotDT);
			if (useItemClass != null && SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useItemClass);
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			}
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			RefreshColor(enter: true);
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
		}
	}

	public bool TryGamepadQuickSellUnderCursor()
	{
		if (!SingletonMonoScope<ShopManager>.HasInstance || !SingletonMonoScope<ShopManager>.Instance.Opened)
		{
			return false;
		}
		if (!TryRefreshMouseSlotFromCursor() || !CanOperateMouseSlotItem())
		{
			return false;
		}
		QuickSell();
		return true;
	}

	public bool TryGamepadDropUnderCursor()
	{
		if (!TryRefreshMouseSlotFromCursor() || !CanOperateMouseSlotItem())
		{
			return false;
		}
		SlotData mouseSlotDT = base.MouseSlotDT;
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
		default:
			return false;
		}
		ThrowItem();
		SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
		return true;
	}

	public bool TryGamepadSendToWarehouseUnderCursor()
	{
		if (!SingletonMonoScope<WarehouseManager>.HasInstance || !SingletonMonoScope<GameUIManager>.HasInstance || !SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse)
		{
			return false;
		}
		if (!TryRefreshMouseSlotFromCursor() || !CanOperateMouseSlotItem())
		{
			return false;
		}
		SlotData mouseSlotDT = base.MouseSlotDT;
		switch (mouseSlotDT.ItemType)
		{
		case 0:
			if (!SingletonMonoScope<WarehouseManager>.Instance.IVtoChest(mouseSlotDT.weapon))
			{
				GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
				return true;
			}
			ThrowItem();
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			return true;
		case 1:
		{
			TransferResult transferResult2 = SingletonMonoScope<WarehouseManager>.Instance.IVtoChest(mouseSlotDT.baoshi, mouseSlotDT);
			if (!transferResult2.Success)
			{
				GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
				return true;
			}
			if (transferResult2.IsComplete)
			{
				ThrowItem();
				SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			}
			else if ((bool)mouseSlotDT.ItemOBJ)
			{
				mouseSlotDT.ItemOBJ.RefreshStackIV(0);
			}
			return true;
		}
		case 2:
		{
			TransferResult transferResult = SingletonMonoScope<WarehouseManager>.Instance.IVtoChest(mouseSlotDT.useitem, mouseSlotDT);
			if (!transferResult.Success)
			{
				GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
				return true;
			}
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(mouseSlotDT.useitem);
			if (transferResult.IsComplete)
			{
				ThrowItem();
				SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			}
			else if ((bool)mouseSlotDT.ItemOBJ)
			{
				mouseSlotDT.ItemOBJ.RefreshStackIV(1);
			}
			return true;
		}
		default:
			return false;
		}
	}

	public void SellAuto(int quality)
	{
		if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened && !Hand.Instance.isDragItem)
		{
			bool flag = IsAutoSellOnlyOtherClasses();
			if (flag)
			{
				CursorUIManager.ConsumeShiftModifier();
			}
			SellAutoWeapons(quality, flag);
		}
	}

	public void SellAutoOtherClassWeaponsAllQualities()
	{
		if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened && !Hand.Instance.isDragItem)
		{
			CursorUIManager.ConsumeShiftModifier();
			SellAutoWeapons(6, onlySellOtherClasses: true);
		}
	}

	private void SellAutoWeapons(int quality, bool onlySellOtherClasses)
	{
		quality = Mathf.Clamp(quality, 0, 6);
		List<SlotData> list = new List<SlotData>();
		for (int i = 0; i < MainPages.Count; i++)
		{
			if (MainPages[i] == null || MainPages[i].MainList == null)
			{
				continue;
			}
			foreach (SlotData main in MainPages[i].MainList)
			{
				if (main != null && main.isMain && main.isOC && main.ItemType == 0 && main.weapon != null && main.weapon.Quality <= quality && (!onlySellOtherClasses || IsOtherClassWeapon(main.weapon)))
				{
					list.Add(main);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		long num = 0L;
		int num2 = 0;
		foreach (SlotData item in list)
		{
			if (item != null && item.isMain && item.isOC && item.ItemType == 0 && item.weapon != null)
			{
				long num3 = item.weapon.Price;
				if (SingletonMonoScope<ShopManager>.Instance.TrySellToShop(item.weapon, playFeedback: false))
				{
					DelItem(item);
					num += num3;
					num2++;
				}
			}
		}
		if (num2 > 0)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Money);
			GameManager.ShowTip(LOC.MM.GetMainFormat("shop_sell_auto_success", num2, num), TipType.Success);
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			RefreshColor(enter: true);
			if ((bool)Sector.sec)
			{
				Sector.sec.SetPosOffset();
			}
			RefreshPointerSlotStateAndTip();
		}
	}

	private bool IsOtherClassWeapon(WeaponClass weapon)
	{
		if (weapon != null && weapon.PLtype != SingletonMonoScope<PlayerManager>.Instance.PLType)
		{
			return !WeaponPlayerType.IsGeneric(weapon.PLtype);
		}
		return false;
	}

	private bool IsAutoSellOnlyOtherClasses()
	{
		if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
		{
			return ContainerManager<InventoryManager>.GetShiftModifier();
		}
		return true;
	}

	public void ChangeMoney(long price, bool add)
	{
		if (add)
		{
			AddMoney(price);
		}
		else
		{
			RemoveMoney(price);
		}
	}

	public void CreateWeapon(SlotData slot)
	{
		if (slot?.weapon != null)
		{
			WeaponClass weaponClass = new WeaponClass();
			ItemCloneUtil.CopyWeaponTo(weaponClass, slot.weapon);
			if (!TryPlaceWeapon(weaponClass, preferCurrentPage: false))
			{
				LogUtil.Info("背包空间不足，无法生成测试物品");
			}
		}
	}

	public void CreateBaoshi(SlotData slot)
	{
		if (slot?.baoshi != null)
		{
			BaoshiClass baoshiClass = new BaoshiClass();
			ItemCloneUtil.CopyBaoshiTo(baoshiClass, slot.baoshi);
			if (!TryPlaceBaoshi(baoshiClass, 0, preferCurrentPage: false))
			{
				LogUtil.Info("背包空间不足，无法生成测试物品");
			}
		}
	}

	public void CreatePotion(SlotData slot)
	{
		if (slot?.useitem != null)
		{
			UseItemClass useItemClass = new UseItemClass();
			ItemCloneUtil.CopyUseItemTo(useItemClass, slot.useitem);
			if (!TryPlaceUseItem(useItemClass, 0, preferCurrentPage: false))
			{
				LogUtil.Info("背包空间不足，无法生成测试物品");
			}
			else if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useItemClass);
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			}
		}
	}

	public void DelAllWeapon(int quality)
	{
		RemoveItemsWhere((SlotData item) => item != null && item.ItemType == 0 && item.weapon != null && item.weapon.Quality <= quality, refreshUseBindingStack: true);
	}

	public void DelAllBaoshi()
	{
		RemoveItemsWhere((SlotData item) => item != null && item.ItemType == 1 && item.baoshi != null);
	}

	public void DelAllUseItem()
	{
		RemoveItemsWhere((SlotData item) => item != null && item.ItemType == 2 && item.useitem != null, refreshUseBindingStack: true);
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.ClearListUse();
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
	}

	public void DelAllItems()
	{
		DelAllWeapon(6);
		DelAllBaoshi();
		DelAllUseItem();
	}

	public bool TryAddBaoshiToInventory(BaoshiClass baoshi)
	{
		if (baoshi == null)
		{
			return false;
		}
		BaoshiClass baoshiClass = ItemCloneUtil.CloneBaoshi(baoshi);
		if (baoshiClass == null)
		{
			return false;
		}
		baoshiClass.CstackSize = 1;
		SlotData slotData = FindStackableBaoshiSlot(baoshiClass.ItemName);
		if (slotData != null && slotData.baoshi != null)
		{
			slotData.baoshi.CstackSize++;
			if ((bool)slotData.ItemOBJ)
			{
				slotData.ItemOBJ.RefreshStackIV(0);
			}
			return true;
		}
		return TryPlaceBaoshi(baoshiClass, 0);
	}

	private SlotData FindStackableBaoshiSlot(string itemName)
	{
		if (string.IsNullOrEmpty(itemName))
		{
			return null;
		}
		for (int i = 0; i < Page.Count; i++)
		{
			SlotDataPage slotDataPage = Page[i];
			if (slotDataPage?.DT == null)
			{
				continue;
			}
			for (int j = 0; j < inventorySize.y; j++)
			{
				for (int k = 0; k < inventorySize.x; k++)
				{
					SlotData slotData = slotDataPage.DT[k, j];
					if (slotData != null && slotData.isOC && slotData.isMain && slotData.ItemType == 1 && slotData.baoshi != null && !(slotData.baoshi.ItemName != itemName) && slotData.baoshi.CstackSize < slotData.baoshi.MstackSize)
					{
						return slotData;
					}
				}
			}
		}
		return null;
	}

	public static bool IsHandHoldingEquippable()
	{
		if (Hand.Instance.isDragItem && Hand.Instance.itemType == 0)
		{
			return Hand.Instance.weapon != null;
		}
		return false;
	}

	public static bool CanEquipToSlot(WeaponClass weapon, int slotCharType)
	{
		if (weapon == null)
		{
			return false;
		}
		if (weapon.Level > SingletonMonoScope<PlayerManager>.Instance.Level)
		{
			return false;
		}
		if (weapon.PLtype != SingletonMonoScope<PlayerManager>.Instance.PLType && !WeaponPlayerType.IsGeneric(weapon.PLtype))
		{
			return false;
		}
		if (weapon.CharType != slotCharType)
		{
			return false;
		}
		return true;
	}

	public void CloseUI()
	{
		cav.alpha = 0f;
		cav.blocksRaycasts = false;
		SingletonMonoScope<GameUIManager>.Instance.Opened_IV = false;
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse)
		{
			Storage.Instance.CloseChest();
		}
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_shop)
		{
			SingletonMonoScope<ShopManager>.Instance.CloseShop();
		}
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_weapon)
		{
			SingletonMonoScope<WeaponManager>.Instance.CloseWeapon();
		}
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_baoshi)
		{
			SingletonMonoScope<BaoshiManager>.Instance.CloseBaoshi();
		}
	}

	public CharButton ReturnCharBT(int a)
	{
		CharButton[] charBT = CharBT;
		foreach (CharButton charButton in charBT)
		{
			if (charButton.charType == a)
			{
				return charButton;
			}
		}
		return null;
	}
}
