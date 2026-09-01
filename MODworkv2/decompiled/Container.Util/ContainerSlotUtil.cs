using System.Collections.Generic;
using Container.Inventory;
using UnityEngine;

namespace Container.Util;

public class ContainerSlotUtil
{
	public static void ColorChange(Color32 color, IntVector2 size, IntVector2 startPos, SlotScript[,] grid)
	{
		if (grid == null)
		{
			return;
		}
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				if (startPos.x + j < 0 || startPos.y + i < 0 || startPos.x + j >= grid.GetLength(0) || startPos.y + i >= grid.GetLength(1))
				{
					continue;
				}
				SlotScript slotScript = grid[startPos.x + j, startPos.y + i];
				if ((bool)slotScript && (bool)slotScript.image)
				{
					if ((float)(int)color.a <= 0f)
					{
						slotScript.image.color = SlotColor.TouMing;
					}
					else
					{
						slotScript.image.color = color;
					}
				}
			}
		}
	}

	public static void ClearColor(IntVector2 size, IntVector2 startPos, SlotScript[,] grid)
	{
		if (grid == null)
		{
			return;
		}
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				if (startPos.x + j >= 0 && startPos.y + i >= 0 && startPos.x + j < grid.GetLength(0) && startPos.y + i < grid.GetLength(1))
				{
					SlotScript slotScript = grid[startPos.x + j, startPos.y + i];
					if ((bool)slotScript)
					{
						slotScript.ClearItemColor();
					}
				}
			}
		}
	}

	public static void ApplyItemColor(SlotData slot, SlotScript[,] grid)
	{
		if (slot == null || grid == null || !slot.isOC)
		{
			return;
		}
		IntVector2 itemSize = ContainerGridUtil.GetItemSize(slot);
		IntVector2 startPos = slot.StartPos;
		if (itemSize.x <= 0 || itemSize.y <= 0 || startPos.x < 0 || startPos.y < 0 || startPos.x + itemSize.x > grid.GetLength(0) || startPos.y + itemSize.y > grid.GetLength(1))
		{
			return;
		}
		bool flag = slot.ItemData != null && slot.ItemData.IsNewlyPicked;
		if ((bool)slot.ItemOBJ)
		{
			slot.ItemOBJ.IsNewlyPicked = flag;
		}
		Color32 color;
		switch (slot.ItemType)
		{
		case 0:
			color = (flag ? QualityColor.NewItemSlotColor : QualityColor.GetSlotColor(GetItemQuality(slot)));
			break;
		case 1:
		case 2:
			color = (flag ? QualityColor.NewItemSlotColor : SlotColor.TouMing);
			break;
		default:
			color = SlotColor.TouMing;
			break;
		}
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				SlotScript slotScript = grid[startPos.x + j, startPos.y + i];
				if ((bool)slotScript)
				{
					slotScript.SetItemColor(color);
				}
			}
		}
	}

	public static void MarkItemAsViewed(SlotData slot, SlotScript[,] grid, List<SlotDataPage> pages = null)
	{
		if (slot == null || !slot.isOC)
		{
			return;
		}
		SlotData slotData = ContainerGridUtil.GetMainSlot(slot, pages) ?? slot;
		IntVector2 itemSize = ContainerGridUtil.GetItemSize(slotData);
		IntVector2 startPos = slotData.StartPos;
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				SlotData slotData2 = slot;
				if (pages != null && slotData.Page >= 0 && slotData.Page < pages.Count && pages[slotData.Page]?.DT != null)
				{
					int num = startPos.x + j;
					int num2 = startPos.y + i;
					if (num >= 0 && num2 >= 0 && num < pages[slotData.Page].DT.GetLength(0) && num2 < pages[slotData.Page].DT.GetLength(1))
					{
						slotData2 = pages[slotData.Page].DT[num, num2];
					}
				}
				if (slotData2?.ItemData != null)
				{
					slotData2.ItemData.IsNewlyPicked = false;
				}
				if ((bool)slotData2?.ItemOBJ)
				{
					slotData2.ItemOBJ.IsNewlyPicked = false;
				}
			}
		}
		ApplyItemColor(slotData, grid);
	}

	public static void MarkItemAsNewlyPicked(SlotData slot, SlotScript[,] grid, List<SlotDataPage> pages = null)
	{
		if (slot == null || !slot.isOC)
		{
			return;
		}
		SlotData slotData = ContainerGridUtil.GetMainSlot(slot, pages) ?? slot;
		IntVector2 itemSize = ContainerGridUtil.GetItemSize(slotData);
		IntVector2 startPos = slotData.StartPos;
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				SlotData slotData2 = slot;
				if (pages != null && slotData.Page >= 0 && slotData.Page < pages.Count && pages[slotData.Page]?.DT != null)
				{
					int num = startPos.x + j;
					int num2 = startPos.y + i;
					if (num >= 0 && num2 >= 0 && num < pages[slotData.Page].DT.GetLength(0) && num2 < pages[slotData.Page].DT.GetLength(1))
					{
						slotData2 = pages[slotData.Page].DT[num, num2];
					}
				}
				if (slotData2?.ItemData != null)
				{
					slotData2.ItemData.IsNewlyPicked = true;
				}
				if ((bool)slotData2?.ItemOBJ)
				{
					slotData2.ItemOBJ.IsNewlyPicked = true;
				}
			}
		}
		ApplyItemColor(slotData, grid);
	}

	public static void RefreshPageColors(List<MainSlotPage> mainPages, int pageIndex, SlotScript[,] grid)
	{
		if (grid == null)
		{
			return;
		}
		for (int i = 0; i < grid.GetLength(1); i++)
		{
			for (int j = 0; j < grid.GetLength(0); j++)
			{
				SlotScript slotScript = grid[j, i];
				if ((bool)slotScript)
				{
					slotScript.ClearItemColor();
				}
			}
		}
		if (mainPages == null || pageIndex < 0 || pageIndex >= mainPages.Count)
		{
			return;
		}
		MainSlotPage mainSlotPage = mainPages[pageIndex];
		if (mainSlotPage?.MainList == null)
		{
			return;
		}
		foreach (SlotData main in mainSlotPage.MainList)
		{
			if (main != null && main.isMain && main.isOC)
			{
				ApplyItemColor(main, grid);
			}
		}
	}

	private static int GetItemQuality(SlotData slot)
	{
		if (slot == null)
		{
			return 0;
		}
		switch (slot.ItemType)
		{
		case 0:
			if (slot.weapon == null)
			{
				return 0;
			}
			return slot.weapon.Quality;
		case 1:
			if (slot.baoshi == null)
			{
				return 0;
			}
			return slot.baoshi.Quality;
		case 2:
			if (slot.useitem == null)
			{
				return 0;
			}
			return slot.useitem.Quality;
		default:
			return 0;
		}
	}

	public static SlotAreaCheckResult CheckAreaOccupy(SlotData[,] dtArray, IntVector2 startPos, IntVector2 size)
	{
		SlotAreaCheckResult result = default(SlotAreaCheckResult);
		bool flag = false;
		IntVector2 intVector = default(IntVector2);
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				SlotData slotData = dtArray[startPos.x + j, startPos.y + i];
				if (slotData.isOC)
				{
					IntVector2 intVector2 = (slotData.isMain ? slotData.GridPos : slotData.StartPos);
					if (!flag)
					{
						flag = true;
						intVector = intVector2;
						result.OtherItem = slotData.ItemOBJ;
						result.OtherItemPos = intVector2;
						result.OtherItemSize = slotData.ItemSize;
					}
					else if (intVector != intVector2)
					{
						result.State = 2;
						return result;
					}
				}
			}
		}
		result.State = (flag ? 1 : 0);
		return result;
	}
}
