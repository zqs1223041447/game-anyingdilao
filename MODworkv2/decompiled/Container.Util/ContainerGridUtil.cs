using System.Collections.Generic;
using UnityEngine;

namespace Container.Util;

public static class ContainerGridUtil
{
	public static SlotData GetMainSlot(SlotData slot, List<SlotDataPage> pages)
	{
		if (slot == null || pages == null)
		{
			return null;
		}
		if (!slot.isOC)
		{
			return null;
		}
		int page = slot.Page;
		if (page < 0 || page >= pages.Count)
		{
			return null;
		}
		SlotDataPage slotDataPage = pages[page];
		if (slotDataPage?.DT == null)
		{
			return null;
		}
		IntVector2 intVector = (slot.isMain ? slot.GridPos : slot.StartPos);
		if (intVector.x < 0 || intVector.y < 0 || intVector.x >= slotDataPage.DT.GetLength(0) || intVector.y >= slotDataPage.DT.GetLength(1))
		{
			return null;
		}
		return slotDataPage.DT[intVector.x, intVector.y];
	}

	public static bool CanPlaceAt(SlotDataPage page, IntVector2 startPos, IntVector2 size, IntVector2 inventorySize)
	{
		if (page?.DT == null)
		{
			return false;
		}
		if (size.x <= 0 || size.y <= 0)
		{
			return false;
		}
		if (startPos.x < 0 || startPos.y < 0)
		{
			return false;
		}
		if (startPos.x + size.x > inventorySize.x)
		{
			return false;
		}
		if (startPos.y + size.y > inventorySize.y)
		{
			return false;
		}
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				SlotData slotData = page.DT[startPos.x + j, startPos.y + i];
				if (slotData == null || slotData.isOC)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static SlotData FindEmptyInPage(SlotDataPage page, EmptySlotPage emptyPage, IntVector2 size, IntVector2 inventorySize)
	{
		if (page == null || emptyPage?.EPList == null)
		{
			return null;
		}
		foreach (SlotData eP in emptyPage.EPList)
		{
			if (eP != null && !eP.isOC && CanPlaceAt(page, eP.GridPos, size, inventorySize))
			{
				return eP;
			}
		}
		return null;
	}

	public static SlotData FindEmptyAcrossPages(List<SlotDataPage> pages, List<EmptySlotPage> emptyPages, IntVector2 size, IntVector2 inventorySize)
	{
		if (pages == null || emptyPages == null)
		{
			return null;
		}
		int num = ((pages.Count < emptyPages.Count) ? pages.Count : emptyPages.Count);
		for (int i = 0; i < num; i++)
		{
			SlotData slotData = FindEmptyInPage(pages[i], emptyPages[i], size, inventorySize);
			if (slotData != null)
			{
				return slotData;
			}
		}
		return null;
	}

	public static IntVector2 GetItemSize(SlotData slot)
	{
		if (slot == null)
		{
			return default(IntVector2);
		}
		return slot.ItemType switch
		{
			0 => slot.weapon?.Size ?? slot.ItemSize, 
			1 => slot.baoshi?.Size ?? slot.ItemSize, 
			2 => slot.useitem?.Size ?? slot.ItemSize, 
			_ => slot.ItemSize, 
		};
	}

	public static void OccupyRegion(SlotData mainSlot, List<SlotDataPage> pages, List<EmptySlotPage> emptyPages, List<MainSlotPage> mainPages, SlotScript[,] slotGrid = null, Color? fillColor = null)
	{
		if (mainSlot == null || pages == null || emptyPages == null || mainPages == null)
		{
			return;
		}
		int page = mainSlot.Page;
		if (page < 0 || page >= pages.Count || page >= emptyPages.Count || page >= mainPages.Count)
		{
			return;
		}
		IntVector2 intVector = mainSlot.StartPos;
		if (intVector == default(IntVector2))
		{
			intVector = mainSlot.GridPos;
		}
		IntVector2 itemSize = GetItemSize(mainSlot);
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				int num = intVector.x + j;
				int num2 = intVector.y + i;
				if (num < 0 || num2 < 0 || num >= pages[page].DT.GetLength(0) || num2 >= pages[page].DT.GetLength(1))
				{
					continue;
				}
				SlotData slotData = pages[page].DT[num, num2];
				if (slotData != null)
				{
					slotData.Page = page;
					slotData.GridPos = new IntVector2(intVector.x + j, intVector.y + i);
					slotData.ItemIndex = mainSlot.ItemIndex;
					slotData.StartPos = intVector;
					slotData.ItemType = mainSlot.ItemType;
					slotData.ItemSize = itemSize;
					slotData.isOC = true;
					slotData.isMain = j == 0 && i == 0;
					slotData.ItemOBJ = mainSlot.ItemOBJ;
					emptyPages[page].EPList.Remove(slotData);
					if (slotData.isMain && !mainPages[page].MainList.Contains(slotData))
					{
						mainPages[page].MainList.Add(slotData);
					}
					if (slotGrid != null && fillColor.HasValue)
					{
						slotGrid[intVector.x + j, intVector.y + i].image.color = fillColor.Value;
					}
				}
			}
		}
		emptyPages[page].EPList.Sort((SlotData t1, SlotData t2) => t1.number.CompareTo(t2.number));
	}

	public static void BindWeaponToRegion(SlotData slot, List<SlotDataPage> pages)
	{
		SlotData mainSlot = GetMainSlot(slot, pages);
		if (mainSlot == null || pages == null || mainSlot.ItemType != 0 || mainSlot.weapon == null)
		{
			return;
		}
		int page = mainSlot.Page;
		if (page < 0 || page >= pages.Count)
		{
			return;
		}
		SlotDataPage slotDataPage = pages[page];
		if (slotDataPage?.DT == null)
		{
			return;
		}
		IntVector2 startPos = (mainSlot.isMain ? mainSlot.GridPos : mainSlot.StartPos);
		IntVector2 itemSize = GetItemSize(mainSlot);
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				int num = startPos.x + j;
				int num2 = startPos.y + i;
				if (num >= 0 && num2 >= 0 && num < slotDataPage.DT.GetLength(0) && num2 < slotDataPage.DT.GetLength(1))
				{
					SlotData slotData = slotDataPage.DT[num, num2];
					if (slotData != null && slotData.isOC && slotData.ItemType == 0)
					{
						slotData.Page = page;
						slotData.ItemIndex = mainSlot.ItemIndex;
						slotData.StartPos = startPos;
						slotData.ItemSize = itemSize;
					}
				}
			}
		}
	}

	public static void ClearRegion(SlotData slot, List<SlotDataPage> pages, List<EmptySlotPage> emptyPages, List<MainSlotPage> mainPages, SlotScript[,] slotGrid = null, Color? slotColor = null)
	{
		if (slot == null || pages == null || emptyPages == null || mainPages == null)
		{
			return;
		}
		int page = slot.Page;
		if (page < 0 || page >= pages.Count || page >= emptyPages.Count || page >= mainPages.Count)
		{
			return;
		}
		IntVector2 intVector = (slot.isMain ? slot.GridPos : slot.StartPos);
		if (intVector.x < 0 || intVector.y < 0 || intVector.x >= pages[page].DT.GetLength(0) || intVector.y >= pages[page].DT.GetLength(1))
		{
			return;
		}
		SlotData slotData = pages[page].DT[intVector.x, intVector.y];
		if (slotData == null)
		{
			return;
		}
		IntVector2 itemSize = GetItemSize(slotData);
		mainPages[page].MainList.Remove(slotData);
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				int num = intVector.x + j;
				int num2 = intVector.y + i;
				if (num < 0 || num2 < 0 || num >= pages[page].DT.GetLength(0) || num2 >= pages[page].DT.GetLength(1))
				{
					continue;
				}
				SlotData slotData2 = pages[page].DT[num, num2];
				if (slotData2 != null)
				{
					slotData2.Page = page;
					slotData2.ClearItemIndex();
					if (!emptyPages[page].EPList.Contains(slotData2))
					{
						emptyPages[page].EPList.Add(slotData2);
					}
					if (slotGrid != null && slotColor.HasValue)
					{
						slotGrid[intVector.x + j, intVector.y + i].image.color = slotColor.Value;
					}
				}
			}
		}
		emptyPages[page].EPList.Sort((SlotData t1, SlotData t2) => t1.number.CompareTo(t2.number));
	}

	public static IntVector2 GetHalfOffset(IntVector2 size)
	{
		return new IntVector2(size.x / 2, size.y / 2);
	}

	public static IntVector2 GetPlaceStartPos(IntVector2 mouseGridPos, IntVector2 itemSize, IntVector2 sectorOffset)
	{
		return mouseGridPos - (GetHalfOffset(itemSize) + sectorOffset);
	}

	public static bool TryClampAreaToGrid(IntVector2 startPos, IntVector2 itemSize, IntVector2 gridSize, out IntVector2 clampedStartPos, out IntVector2 clampedSize)
	{
		clampedStartPos = startPos;
		clampedSize = itemSize;
		bool result = false;
		IntVector2 intVector = startPos + itemSize;
		if (intVector.x > gridSize.x)
		{
			clampedSize.x = gridSize.x - startPos.x;
			result = true;
		}
		if (startPos.x < 0)
		{
			clampedSize.x = itemSize.x + startPos.x;
			clampedStartPos.x = 0;
			result = true;
		}
		if (intVector.y > gridSize.y)
		{
			clampedSize.y = gridSize.y - startPos.y;
			result = true;
		}
		if (startPos.y < 0)
		{
			clampedSize.y = itemSize.y + startPos.y;
			clampedStartPos.y = 0;
			result = true;
		}
		if (clampedSize.x < 0)
		{
			clampedSize.x = 0;
		}
		if (clampedSize.y < 0)
		{
			clampedSize.y = 0;
		}
		return result;
	}

	public static void BindItemObjToRegion(int page, IntVector2 startPos, IntVector2 size, ItemScript itemObj, List<SlotDataPage> pages)
	{
		if (pages == null || !itemObj || page < 0 || page >= pages.Count)
		{
			return;
		}
		SlotDataPage slotDataPage = pages[page];
		if (slotDataPage?.DT == null)
		{
			return;
		}
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				int num = startPos.x + j;
				int num2 = startPos.y + i;
				if (num >= 0 && num2 >= 0 && num < slotDataPage.DT.GetLength(0) && num2 < slotDataPage.DT.GetLength(1))
				{
					SlotData slotData = slotDataPage.DT[num, num2];
					if (slotData != null)
					{
						slotData.ItemOBJ = itemObj;
					}
				}
			}
		}
	}
}
