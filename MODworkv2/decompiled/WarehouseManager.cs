using System.Collections.Generic;
using Container.Inventory;
using Container.Managers;
using Container.Util;
using Core;
using Data.SaveData;
using Data.SaveData.GlobalSave;
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

public class WarehouseManager : ContainerManager<WarehouseManager>
{
	public GlobalChestSaveData SaveData;

	public int PageSaveCount;

	protected override ContainerType ContainerType => ContainerType.Warehouse;

	public override int MaxPageCount => 10000;

	public override int ContainerMpos => 1;

	public void InitFromSaveData(GlobalChestSaveData data)
	{
		SaveData = DataUtil.DeepClone(data);
		ApplySaveData(SaveData);
	}

	public GlobalChestSaveData ExportGlobalSaveData()
	{
		PageSaveCount = PageNumber;
		GlobalChestSaveData globalChestSaveData = new GlobalChestSaveData
		{
			PageCount = PageSaveCount
		};
		ContainerSaveUtil.SaveContainerItems(ItemList, globalChestSaveData.ChestItems);
		return globalChestSaveData;
	}

	public void ApplySaveData(GlobalChestSaveData data)
	{
		if (data == null)
		{
			data = GlobalChestSaveData.CreateDefault();
		}
		PageSaveCount = data.PageCount;
		RestoreSaveData(data);
	}

	public void RestoreSaveData(GlobalChestSaveData data)
	{
		if (data != null)
		{
			CreateSlots(ContainerType.Warehouse);
			for (int i = 0; i < PageSaveCount; i++)
			{
				CreatePage();
			}
			RefreshPageText();
			RestoreWarehouseItems(data);
		}
	}

	public void RestoreWarehouseItems(GlobalChestSaveData data)
	{
		if (data?.ChestItems == null)
		{
			return;
		}
		foreach (ContainerItemSaveData chestItem in data.ChestItems)
		{
			if (chestItem != null)
			{
				SlotData slotData = ContainerRestoreUtil.RestoreOneItemToPage(chestItem, Page, MainPages, inventorySize);
				if (slotData != null)
				{
					ContainerGridUtil.OccupyRegion(slotData, Page, EPPages, MainPages);
					ContainerPageViewUtil.SpawnItemUI(slotData, CurPage, IVitem, dropParent, slotGrid, Page, 1);
				}
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		closeBtn.onClick.AddListener(CloseUI);
	}

	public void Update()
	{
		KeyCodeThrowItem(1);
		bool flag = SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.Opened_baoshi && SingletonMonoScope<BaoshiManager>.HasInstance && SingletonMonoScope<BaoshiManager>.Instance.IsGamepadCreateShortcutDown();
		bool flag2 = ContainerManager<WarehouseManager>.GetCursorRightDown();
		if (!flag && TryConsumeGamepadShiftRightDown())
		{
			flag2 = true;
		}
		if (ContainerManager<WarehouseManager>.GetShiftModifier())
		{
			if (ContainerManager<WarehouseManager>.GetCursorLeftDown() && Hand.Instance.Mpos == 1 && SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse && (bool)MouseSlot && !Hand.Instance.ItemOBJ && base.MouseSlotDT != null && base.MouseSlotDT.isOC)
			{
				CursorUIManager.ConsumeShiftModifier();
				switch (base.MouseSlotDT.ItemType)
				{
				case 0:
					if (SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(base.MouseSlotDT.weapon))
					{
						ThrowItem();
						SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
					}
					else
					{
						GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
						LogUtil.Info("背包空间不足");
					}
					break;
				case 1:
				{
					TransferResult transferResult2 = SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(base.MouseSlotDT.baoshi, base.MouseSlotDT);
					if (transferResult2.Success)
					{
						if (transferResult2.IsComplete)
						{
							ThrowItem();
							SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
						}
						else if ((bool)base.MouseSlotDT.ItemOBJ)
						{
							base.MouseSlotDT.ItemOBJ.RefreshStackChest(0);
						}
					}
					else
					{
						GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
						LogUtil.Info("背包空间不足");
					}
					break;
				}
				case 2:
				{
					TransferResult transferResult = SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(base.MouseSlotDT.useitem, base.MouseSlotDT);
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
							base.MouseSlotDT.ItemOBJ.RefreshStackChest(1);
						}
					}
					else
					{
						GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
						LogUtil.Info("背包空间不足");
					}
					break;
				}
				}
			}
			if (!flag2 || Hand.Instance.Mpos != 1 || !MouseSlot || base.MouseSlotDT == null || !base.MouseSlotDT.isOC)
			{
				return;
			}
			CursorUIManager.ConsumeShiftModifier();
			if (!Hand.Instance.ItemOBJ)
			{
				int itemType = base.MouseSlotDT.ItemType;
				if (itemType != 0 && (uint)(itemType - 1) <= 1u)
				{
					SplitFirst();
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
				}
				break;
			case 0:
				break;
			}
			return;
		}
		if (ContainerManager<WarehouseManager>.GetCursorLeftDown())
		{
			if (CanPutHandItemToMouseSlot())
			{
				switch (checkState)
				{
				case 0:
					PutItem();
					Hand.Instance.ClearItem();
					break;
				case 1:
					switch (Hand.Instance.itemType)
					{
					case 0:
						SwapItem();
						break;
					case 1:
						switch (base.MouseSlotDT.ItemType)
						{
						case 0:
							if (Hand.Instance.baoshi.UseType != 0)
							{
								TryApplyHeldSpecialBaoshiToWeapon(base.MouseSlotDT);
							}
							else if (base.MouseSlotDT.weapon.AocaoCount > 0)
							{
								if (GetEMac(base.MouseSlotDT.weapon) != null)
								{
									if (!_audioManager && SingletonMonoGlobal<AudioManager>.HasInstance)
									{
										_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
									}
									RuntimeManager.PlayOneShot(_audioManager.audioData.Baoshi.Use[Hand.Instance.baoshi.SoundUse]);
									XiangQian(base.MouseSlotDT.weapon, Hand.Instance.baoshi, GetEMacNumber(base.MouseSlotDT.weapon));
									SingletonMonoScope<GameUIManager>.Instance.ShowWPTipA(base.MouseSlotDT.weapon, base.MouseSlotDT, slotGrid);
									EnsureCurrentPageItemObj(base.MouseSlotDT);
									if ((bool)base.MouseSlotDT.ItemOBJ)
									{
										base.MouseSlotDT.ItemOBJ.RefreshBS(base.MouseSlotDT);
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
									SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(base.MouseSlotDT.weapon, base.MouseSlotDT, slotGrid);
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
						{
							BaoshiClass baoshi = base.MouseSlotDT.baoshi;
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
									base.MouseSlotDT.ItemOBJ.RefreshStackChest(0);
									Hand.Instance.DELItem();
									SingletonMonoScope<GameUIManager>.Instance.ShowBSTip(base.MouseSlotDT.baoshi, base.MouseSlotDT, slotGrid);
									Sector.sec.SetPosOffset();
									RefreshColor(enter: true);
								}
								else
								{
									baoshi2.CstackSize -= baoshi.MstackSize - baoshi.CstackSize;
									baoshi.CstackSize = baoshi.MstackSize;
									base.MouseSlotDT.ItemOBJ.RefreshStackChest(0);
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
							SwapItem();
							break;
						}
						break;
					case 2:
						switch (base.MouseSlotDT.ItemType)
						{
						case 0:
							SwapItem();
							break;
						case 1:
							SwapItem();
							break;
						case 2:
						{
							UseItemClass useitem = base.MouseSlotDT.useitem;
							UseItemClass useitem2 = Hand.Instance.useitem;
							if (useitem.ItemName == useitem2.ItemName)
							{
								if (useitem.CstackSize == useitem.MstackSize)
								{
									SwapItem();
								}
								else if (useitem.CstackSize + useitem2.CstackSize <= useitem.MstackSize)
								{
									useitem.CstackSize += useitem2.CstackSize;
									base.MouseSlotDT.ItemOBJ.RefreshStackChest(1);
									Hand.Instance.DELItem();
									SingletonMonoScope<GameUIManager>.Instance.ShowUseTip(base.MouseSlotDT.useitem, base.MouseSlotDT, slotGrid);
									Sector.sec.SetPosOffset();
									RefreshColor(enter: true);
								}
								else
								{
									useitem2.CstackSize -= useitem.MstackSize - useitem.CstackSize;
									useitem.CstackSize = useitem.MstackSize;
									base.MouseSlotDT.ItemOBJ.RefreshStackChest(1);
									Hand.Instance.ItemOBJ.RefreshStackHand(1);
								}
							}
							else
							{
								SwapItem();
							}
							break;
						}
						}
						break;
					}
					break;
				}
			}
			else if (CanOperateMouseSlotItem())
			{
				TakeItem();
			}
		}
		if (flag || !ContainerManager<WarehouseManager>.GetCursorRightDown() || !CanOperateMouseSlotItem())
		{
			return;
		}
		switch (base.MouseSlotDT.ItemType)
		{
		case 0:
		{
			SlotData mouseSlotDT = base.MouseSlotDT;
			if (mouseSlotDT?.weapon == null || (mouseSlotDT.weapon.PLtype != SingletonMonoScope<PlayerManager>.Instance.PLType && !WeaponPlayerType.IsGeneric(mouseSlotDT.weapon.PLtype)))
			{
				break;
			}
			EnsureCurrentPageItemObj(mouseSlotDT);
			CharButton charButton = ReturnCharBT(mouseSlotDT.weapon.CharType);
			if ((bool)charButton && mouseSlotDT.weapon.Level <= SingletonMonoScope<PlayerManager>.Instance.Level && (bool)mouseSlotDT.ItemOBJ)
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
					if (SingletonMonoScope<PlayerManager>.Instance.HealStat.Cur < SingletonMonoScope<PlayerManager>.Instance.HealStat.Max)
					{
						UseItem();
					}
					break;
				case "mana":
					if (SingletonMonoScope<PlayerManager>.Instance.ManaStat.Cur < SingletonMonoScope<PlayerManager>.Instance.ManaStat.Max)
					{
						UseItem();
					}
					break;
				case "huoli":
					if (SingletonMonoScope<PlayerManager>.Instance.HealStat.Cur < SingletonMonoScope<PlayerManager>.Instance.HealStat.Max || SingletonMonoScope<PlayerManager>.Instance.ManaStat.Cur < SingletonMonoScope<PlayerManager>.Instance.ManaStat.Max)
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
					if (!InventoryManager.CheckScrollUseLimit(checkHomeScene: false))
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
			SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(slotData.weapon, base.MouseSlotDT, slotGrid);
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
			Hand.Instance.TakeItem(slotData);
			ContainerSlotUtil.ColorChange(SlotColor.TouMing, slotData.ItemSize, slotData.StartPos, slotGrid);
			ContainerGridUtil.ClearRegion(slotData, Page, EPPages, MainPages);
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
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
		SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
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
			SingletonMonoScope<InventoryManager>.Instance.EquipWeaponCore(charButton, isRestore: false, weapon);
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
			TryPlaceWeaponInWarehouse(slotData2 ?? Page[page].DT[startPos.x, startPos.y], weaponClass, itemScript, out mainSlot);
			GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
			LogUtil.Info("仓库空间不足");
			RefreshColor(enter: true);
			return;
		}
		if (!TryPlaceWeaponInWarehouse(slotData2, weaponClass2, itemOBJ, out var mainSlot2))
		{
			TryPlaceWeaponInWarehouse(Page[page].DT[startPos.x, startPos.y], weaponClass, itemScript, out mainSlot);
			GameManager.ShowTip(LOC.MM.GetMain("warehouse_is_full"), TipType.Fail);
			LogUtil.Info("仓库空间不足");
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

	private bool TryPlaceWeaponInWarehouse(SlotData targetPos, WeaponClass weapon, ItemScript itemObj, out SlotData mainSlot)
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
		Transform parent = (SingletonMonoScope<InventoryManager>.HasInstance ? SingletonMonoScope<InventoryManager>.Instance.dropParent : dropParent);
		itemObj.transform.SetParent(parent);
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
				EPPages[CurPage - 1].EPList.Sort((SlotData t1, SlotData t2) => t1.number.CompareTo(t2.number));
			}
		}
		if (CheckEmpty(ReturnCharBT(weaponClass.CharType).weapon.Size) != null)
		{
			if (ReturnCharBT(weaponClass.CharType).weapon.Size.x > weaponClass.Size.x || ReturnCharBT(weaponClass.CharType).weapon.Size.y > weaponClass.Size.y)
			{
				slotData = CheckEmpty(ReturnCharBT(weaponClass.CharType).weapon.Size);
			}
			CharButton charButton = ReturnCharBT(weaponClass.CharType);
			WeaponClass weapon = charButton.weapon;
			IntVector2 intVector2 = new IntVector2(weapon.Size.x, weapon.Size.y);
			ItemScript itemOBJ2 = ReturnCharBT(weaponClass.CharType).ItemOBJ;
			weapon.Equip(1);
			for (int k = 0; k < intVector2.y; k++)
			{
				for (int l = 0; l < intVector2.x; l++)
				{
					SlotData slotData2 = Page[CurPage - 1].DT[slotData.GridPos.x + l, slotData.GridPos.y + k];
					if (l == 0 && k == 0)
					{
						slotData2.isMain = true;
						MainPages[CurPage - 1].MainList.Add(slotData2);
					}
					slotData2.Page = CurPage - 1;
					slotData2.ItemOBJ = itemOBJ2;
					slotData2.ItemType = 0;
					slotData2.ItemSize = weapon.Size;
					slotData2.StartPos = slotData.GridPos;
					slotData2.isOC = true;
					slotGrid[slotData.GridPos.x + l, slotData.GridPos.y + k].image.color = SlotColor.TouMing;
					ItemCloneUtil.CopyWeaponTo(slotData2.weapon, weapon);
					EPPages[CurPage - 1].EPList.Remove(slotData2);
				}
			}
			itemOBJ2.SetWP(weapon);
			itemOBJ2.transform.SetParent(dropParent);
			itemOBJ2.GetComponent<RectTransform>().pivot = Vector2.up;
			itemOBJ2.GetComponent<RectTransform>().localScale = Vector3.one;
			itemOBJ2.transform.position = slotGrid[slotData.GridPos.x, slotData.GridPos.y].transform.position;
			itemOBJ2.page = CurPage - 1;
			itemOBJ2.saveSlot = new IntVector2(slotData.GridPos.x, slotData.GridPos.y);
			charButton.ItemOBJ = itemOBJ;
			charButton.weapon = weaponClass;
			itemOBJ.SetWP(weaponClass);
			weaponClass.Equip(0);
			itemOBJ.transform.SetParent(SingletonMonoScope<InventoryManager>.Instance.dropParent);
			itemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
			itemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
			itemOBJ.transform.position = charButton.transform.position;
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
			if ((bool)base.MouseSlotDT.ItemOBJ)
			{
				itemOBJ.RefreshBS(base.MouseSlotDT);
				SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(weapon, base.MouseSlotDT, slotGrid);
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			}
		}
		else
		{
			for (int m = 0; m < intVector.y; m++)
			{
				for (int n = 0; n < intVector.x; n++)
				{
					SlotData slotData2 = Page[CurPage - 1].DT[slotData.GridPos.x + n, slotData.GridPos.y + m];
					if (n == 0 && m == 0)
					{
						slotData2.isMain = true;
						MainPages[CurPage - 1].MainList.Add(slotData2);
					}
					slotData2.Page = CurPage - 1;
					slotData2.ItemOBJ = itemOBJ;
					slotData2.ItemType = 0;
					slotData2.ItemSize = weaponClass.Size;
					slotData2.StartPos = slotData.GridPos;
					slotData2.isOC = true;
					slotGrid[slotData.GridPos.x + n, slotData.GridPos.y + m].image.color = SlotColor.TouMing;
					ItemCloneUtil.CopyWeaponTo(slotData2.weapon, weaponClass);
					EPPages[CurPage - 1].EPList.Remove(slotData2);
				}
			}
			itemOBJ.SetWP(weaponClass);
			itemOBJ.RefreshBS(base.MouseSlotDT);
			SingletonMonoScope<GameUIManager>.Instance.ShowCompareWeaponTips(weaponClass, base.MouseSlotDT, slotGrid);
		}
		Sector.sec.SetPosOffset();
		RefreshColor(enter: true);
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
			ContainerPageViewUtil.SpawnItemUI(slotData2, CurPage, IVitem, dropParent, slotGrid, Page, 1);
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
						slotData.ItemOBJ.RefreshStackChest(0);
					}
					break;
				}
				slotData.baoshi.CstackSize = slotData.baoshi.MstackSize;
				bs.CstackSize -= num;
				if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackChest(0);
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
					ContainerPageViewUtil.SpawnItemUI(slotData2, CurPage, IVitem, dropParent, slotGrid, Page, 1);
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
						slotData.ItemOBJ.RefreshStackChest(1);
					}
					break;
				}
				slotData.useitem.CstackSize = slotData.useitem.MstackSize;
				use.CstackSize -= num;
				if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackChest(1);
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
					ContainerPageViewUtil.SpawnItemUI(slotData2, CurPage, IVitem, dropParent, slotGrid, Page, 1);
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
				ContainerManager<WarehouseManager>.AllBS.AxA.Add(ItemCloneUtil.CloneBaoshi(slot.baoshi));
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
				ContainerManager<WarehouseManager>.AllWP.BxD.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 3)
			{
				ContainerManager<WarehouseManager>.AllWP.BxC.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 2)
			{
				ContainerManager<WarehouseManager>.AllWP.BxB.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			break;
		case 1:
			if (wp.Size.y == 4)
			{
				ContainerManager<WarehouseManager>.AllWP.AxD.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 3)
			{
				ContainerManager<WarehouseManager>.AllWP.AxC.Add(ItemCloneUtil.CloneWeapon(wp));
			}
			else if (wp.Size.y == 1)
			{
				ContainerManager<WarehouseManager>.AllWP.AxA.Add(ItemCloneUtil.CloneWeapon(wp));
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
				ContainerManager<WarehouseManager>.AllUSE.BxB.Add(ItemCloneUtil.CloneUseItem(use));
			}
			break;
		case 1:
			if (use.Size.y == 1)
			{
				ContainerManager<WarehouseManager>.AllUSE.AxA.Add(ItemCloneUtil.CloneUseItem(use));
			}
			break;
		}
	}

	private void SortAllBucketsByPrice()
	{
		SortWeaponBucket(ContainerManager<WarehouseManager>.AllWP.BxD);
		SortWeaponBucket(ContainerManager<WarehouseManager>.AllWP.BxC);
		SortWeaponBucket(ContainerManager<WarehouseManager>.AllWP.AxD);
		SortWeaponBucket(ContainerManager<WarehouseManager>.AllWP.AxC);
		SortWeaponBucket(ContainerManager<WarehouseManager>.AllWP.BxB);
		SortWeaponBucket(ContainerManager<WarehouseManager>.AllWP.AxA);
		SortOneSlotItemBucket(ContainerManager<WarehouseManager>.AllBS.AxA);
		SortUseItemBucket(ContainerManager<WarehouseManager>.AllUSE.BxB);
		SortOneSlotItemBucket(ContainerManager<WarehouseManager>.AllUSE.AxA);
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
				return (num != 0) ? num : b.Price.CompareTo(a.Price);
			});
		}
	}

	private void RebuildAllPagesByBuckets()
	{
		RebuildWeaponBucketAll(ContainerManager<WarehouseManager>.AllWP.BxD);
		RebuildWeaponBucketAll(ContainerManager<WarehouseManager>.AllWP.BxC);
		RebuildWeaponBucketAll(ContainerManager<WarehouseManager>.AllWP.AxD);
		RebuildWeaponBucketAll(ContainerManager<WarehouseManager>.AllWP.AxC);
		RebuildWeaponBucketAll(ContainerManager<WarehouseManager>.AllWP.BxB);
		RebuildUseItemBucketAll(ContainerManager<WarehouseManager>.AllUSE.BxB);
		RebuildWeaponBucketAll(ContainerManager<WarehouseManager>.AllWP.AxA);
		RebuildBaoshiBucketAll(ContainerManager<WarehouseManager>.AllBS.AxA);
		RebuildUseItemBucketAll(ContainerManager<WarehouseManager>.AllUSE.AxA);
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
		RebuildWeaponBucket(ContainerManager<WarehouseManager>.AllWP.BxD);
		RebuildWeaponBucket(ContainerManager<WarehouseManager>.AllWP.BxC);
		RebuildWeaponBucket(ContainerManager<WarehouseManager>.AllWP.AxD);
		RebuildWeaponBucket(ContainerManager<WarehouseManager>.AllWP.AxC);
		RebuildWeaponBucket(ContainerManager<WarehouseManager>.AllWP.BxB);
		RebuildUseItemBucket(ContainerManager<WarehouseManager>.AllUSE.BxB);
		RebuildWeaponBucket(ContainerManager<WarehouseManager>.AllWP.AxA);
		RebuildBaoshiBucket(ContainerManager<WarehouseManager>.AllBS.AxA);
		RebuildUseItemBucket(ContainerManager<WarehouseManager>.AllUSE.AxA);
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

	private void ClearAllBuckets()
	{
		ContainerManager<WarehouseManager>.AllWP.BxD.Clear();
		ContainerManager<WarehouseManager>.AllWP.BxC.Clear();
		ContainerManager<WarehouseManager>.AllWP.AxD.Clear();
		ContainerManager<WarehouseManager>.AllWP.AxC.Clear();
		ContainerManager<WarehouseManager>.AllWP.BxB.Clear();
		ContainerManager<WarehouseManager>.AllWP.AxA.Clear();
		ContainerManager<WarehouseManager>.AllBS.AxA.Clear();
		ContainerManager<WarehouseManager>.AllUSE.BxB.Clear();
		ContainerManager<WarehouseManager>.AllUSE.AxA.Clear();
	}

	public void UseItem()
	{
		if (!MouseSlot)
		{
			return;
		}
		SlotData mouseSlotDT = base.MouseSlotDT;
		if (mouseSlotDT == null || !mouseSlotDT.isOC || mouseSlotDT.ItemType != 2 || mouseSlotDT.useitem == null)
		{
			return;
		}
		UseItemClass useitem = mouseSlotDT.useitem;
		int soundUse = useitem.SoundUse;
		int infoType = useitem.InfoType;
		if (!useitem.Use())
		{
			return;
		}
		IntVector2 startPos = mouseSlotDT.StartPos;
		if (useitem.CstackSize > 1)
		{
			for (int i = 0; i < useitem.Size.y; i++)
			{
				for (int j = 0; j < useitem.Size.x; j++)
				{
					Page[CurPage - 1].DT[startPos.x + j, startPos.y + i].useitem.CstackSize--;
				}
			}
			if ((bool)mouseSlotDT.ItemOBJ)
			{
				mouseSlotDT.ItemOBJ.RefreshStackChest(1);
			}
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useitem);
		}
		else
		{
			SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(useitem);
			ThrowItem();
		}
		_audioManager.PlaySO_UseItem(soundUse, infoType);
	}

	public static WPAocao GetEMac(WeaponClass wp)
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
					case "sword":
					case "bow":
					case "staff":
					case "bone":
					case "shield":
					case "arrow":
					case "spell":
					case "corpse":
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
					case "sword":
					case "bow":
					case "staff":
					case "bone":
					case "shield":
					case "arrow":
					case "spell":
					case "corpse":
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
					case "sword":
					case "bow":
					case "staff":
					case "bone":
					case "shield":
					case "arrow":
					case "spell":
					case "corpse":
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
					case "sword":
					case "bow":
					case "staff":
					case "bone":
					case "shield":
					case "arrow":
					case "spell":
					case "corpse":
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
					case "sword":
					case "bow":
					case "staff":
					case "bone":
					case "shield":
					case "arrow":
					case "spell":
					case "corpse":
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
					case "sword":
					case "bow":
					case "staff":
					case "bone":
					case "shield":
					case "arrow":
					case "spell":
					case "corpse":
						slotData.weapon.Aocao[index].Type = 25;
						break;
					}
					break;
				}
			}
		}
	}

	public bool IVtoChest(WeaponClass wp)
	{
		if (wp == null)
		{
			return false;
		}
		_audioManager.PlaySO_Item(0, wp.WeaponType, wp.SoundDrop, 0);
		return TryPlaceWeapon(wp);
	}

	public TransferResult IVtoChest(BaoshiClass bs, SlotData it, bool allowHandOverflow = true)
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
				sd.ItemOBJ.RefreshStackChest(0);
			}
		}) || TryPlaceBaoshi(bs, 1))
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

	public TransferResult IVtoChest(UseItemClass use, SlotData it, bool allowHandOverflow = true)
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
				sd.ItemOBJ.RefreshStackChest(1);
			}
		}) || TryPlaceUseItem(use, 1))
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

	public void PutAll()
	{
		while (true)
		{
			SlotData slotData = SingletonMonoScope<InventoryManager>.Instance.GiveItem();
			if (slotData == null)
			{
				break;
			}
			switch (slotData.ItemType)
			{
			case 0:
				if (!IVtoChest(slotData.weapon))
				{
					return;
				}
				SingletonMonoScope<InventoryManager>.Instance.DelItem(slotData);
				break;
			case 1:
			{
				TransferResult transferResult2 = IVtoChest(slotData.baoshi, slotData, allowHandOverflow: false);
				if (!transferResult2.Success)
				{
					return;
				}
				if (transferResult2.IsComplete)
				{
					SingletonMonoScope<InventoryManager>.Instance.DelItem(slotData);
				}
				else if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackIV(0);
				}
				break;
			}
			case 2:
			{
				TransferResult transferResult = IVtoChest(slotData.useitem, slotData, allowHandOverflow: false);
				if (!transferResult.Success)
				{
					return;
				}
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
				if (transferResult.IsComplete)
				{
					SingletonMonoScope<InventoryManager>.Instance.DelItem(slotData);
				}
				else if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackIV(1);
				}
				break;
			}
			}
		}
	}

	public void TakeAll()
	{
		if (!SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return;
		}
		while (true)
		{
			SlotData slotData = GiveItem();
			if (slotData == null)
			{
				break;
			}
			switch (slotData.ItemType)
			{
			case 0:
				if (!SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(slotData.weapon))
				{
					return;
				}
				DelItem(slotData);
				break;
			case 1:
			{
				TransferResult transferResult2 = SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(slotData.baoshi, slotData, allowHandOverflow: false);
				if (!transferResult2.Success)
				{
					return;
				}
				if (transferResult2.IsComplete)
				{
					DelItem(slotData);
				}
				else if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackChest(0);
				}
				break;
			}
			case 2:
			{
				TransferResult transferResult = SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(slotData.useitem, slotData, allowHandOverflow: false);
				if (!transferResult.Success)
				{
					return;
				}
				SingletonMonoScope<ACTbar>.Instance.RefreshUseListOne(slotData.useitem);
				if (transferResult.IsComplete)
				{
					DelItem(slotData);
				}
				else if ((bool)slotData.ItemOBJ)
				{
					slotData.ItemOBJ.RefreshStackChest(1);
				}
				break;
			}
			}
		}
	}

	public bool TryGamepadSendToInventoryUnderCursor()
	{
		if (!SingletonMonoScope<InventoryManager>.HasInstance || !SingletonMonoScope<GameUIManager>.HasInstance || !SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse)
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
			if (!SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(mouseSlotDT.weapon))
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
				return true;
			}
			ThrowItem();
			SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			return true;
		case 1:
		{
			TransferResult transferResult2 = SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(mouseSlotDT.baoshi, mouseSlotDT);
			if (!transferResult2.Success)
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
				return true;
			}
			if (transferResult2.IsComplete)
			{
				ThrowItem();
				SingletonMonoScope<GameUIManager>.Instance.HideAllWeaponTips();
			}
			else if ((bool)mouseSlotDT.ItemOBJ)
			{
				mouseSlotDT.ItemOBJ.RefreshStackChest(0);
			}
			return true;
		}
		case 2:
		{
			TransferResult transferResult = SingletonMonoScope<InventoryManager>.Instance.ChesttoIV(mouseSlotDT.useitem, mouseSlotDT);
			if (!transferResult.Success)
			{
				GameManager.ShowTip(LOC.MM.GetMain("bag_is_full"), TipType.Fail);
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
				mouseSlotDT.ItemOBJ.RefreshStackChest(1);
			}
			return true;
		}
		default:
			return false;
		}
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
				base.MouseSlotDT.ItemOBJ.RefreshStackChest(0);
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
				base.MouseSlotDT.ItemOBJ.RefreshStackChest(1);
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
	}

	public void SplitSecond()
	{
		switch (base.MouseSlotDT.ItemType)
		{
		case 1:
		{
			if (base.MouseSlotDT.baoshi.CstackSize > 1)
			{
				for (int m = 0; m < base.MouseSlotDT.baoshi.Size.y; m++)
				{
					for (int n = 0; n < base.MouseSlotDT.baoshi.Size.x; n++)
					{
						Page[CurPage - 1].DT[MouseSlot.GridPos.x + n, MouseSlot.GridPos.y + m].baoshi.CstackSize--;
					}
				}
				base.MouseSlotDT.ItemOBJ.RefreshStackChest(0);
				Hand.Instance.baoshi.CstackSize++;
				Hand.Instance.ItemOBJ.RefreshStackHand(0);
				Sector.sec.SetPosOffset();
				RefreshColor(enter: true);
				break;
			}
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
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
			break;
		}
		case 2:
		{
			if (base.MouseSlotDT.useitem.CstackSize > 1)
			{
				for (int i = 0; i < base.MouseSlotDT.useitem.Size.y; i++)
				{
					for (int j = 0; j < base.MouseSlotDT.useitem.Size.x; j++)
					{
						Page[CurPage - 1].DT[MouseSlot.GridPos.x + j, MouseSlot.GridPos.y + i].useitem.CstackSize--;
					}
				}
				base.MouseSlotDT.ItemOBJ.RefreshStackChest(1);
				Hand.Instance.useitem.CstackSize++;
				Hand.Instance.ItemOBJ.RefreshStackHand(1);
				Sector.sec.SetPosOffset();
				RefreshColor(enter: true);
				break;
			}
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
			Sector.sec.SetPosOffset();
			RefreshColor(enter: true);
			break;
		}
		}
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

	public void CloseUI()
	{
		if ((bool)Storage.Instance)
		{
			Storage.Instance.CloseChestUI();
		}
	}

	public static CharButton ReturnCharBT(int a)
	{
		return SingletonMonoScope<InventoryManager>.Instance.ReturnCharBT(a);
	}
}
