using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace Container.Util;

public static class ContainerPageViewUtil
{
	public static void SpawnItemUI(SlotData slot, int curPage, GameObject itemPrefab, Transform dropParent, SlotScript[,] slotGrid, List<SlotDataPage> pages, int containerType)
	{
		if (slot == null || !slot.isMain || slot.Page != curPage - 1 || slot.Page < 0 || slot.Page >= pages.Count || slot.GridPos.x < 0 || slot.GridPos.y < 0 || slot.GridPos.x >= slotGrid.GetLength(0) || slot.GridPos.y >= slotGrid.GetLength(1))
		{
			return;
		}
		GameObject gameObject = LeanPool.Spawn(itemPrefab);
		ItemScript component = gameObject.GetComponent<ItemScript>();
		if (!component)
		{
			LeanPool.Despawn(gameObject);
			return;
		}
		slot.ItemOBJ = component;
		RectTransform component2 = component.GetComponent<RectTransform>();
		component.transform.SetParent(dropParent);
		component2.pivot = Vector2.up;
		component2.localScale = Vector3.one;
		component.transform.position = slotGrid[slot.GridPos.x, slot.GridPos.y].transform.position;
		component.page = slot.Page;
		component.saveSlot = new IntVector2(slot.GridPos.x, slot.GridPos.y);
		component.NewPagePut(slot, containerType);
		IntVector2 itemSize = slot.ItemSize;
		IntVector2 gridPos = slot.GridPos;
		for (int i = 0; i < itemSize.y; i++)
		{
			for (int j = 0; j < itemSize.x; j++)
			{
				int num = gridPos.x + j;
				int num2 = gridPos.y + i;
				if (num >= 0 && num2 >= 0 && num < slotGrid.GetLength(0) && num2 < slotGrid.GetLength(1))
				{
					pages[slot.Page].DT[num, num2].ItemOBJ = component;
				}
			}
		}
	}

	public static void HidePageItems(int pageIndex, List<MainSlotPage> mainPages, List<SlotDataPage> pages, IntVector2 inventorySize, List<GameObject> cacheToDespawn)
	{
		if (pageIndex < 0 || mainPages == null || pages == null || pageIndex >= mainPages.Count || pageIndex >= pages.Count)
		{
			return;
		}
		MainSlotPage mainSlotPage = mainPages[pageIndex];
		if (mainSlotPage?.MainList == null || mainSlotPage.MainList.Count == 0)
		{
			return;
		}
		foreach (SlotData main in mainSlotPage.MainList)
		{
			if ((bool)main?.ItemOBJ)
			{
				GameObject gameObject = main.ItemOBJ.gameObject;
				if (!cacheToDespawn.Contains(gameObject))
				{
					cacheToDespawn.Add(gameObject);
				}
			}
		}
		for (int i = 0; i < inventorySize.y; i++)
		{
			for (int j = 0; j < inventorySize.x; j++)
			{
				SlotData slotData = pages[pageIndex].DT[j, i];
				if (slotData != null)
				{
					slotData.ItemOBJ = null;
				}
			}
		}
		foreach (GameObject item in cacheToDespawn)
		{
			if ((bool)item)
			{
				LeanPool.Despawn(item);
			}
		}
		cacheToDespawn.Clear();
	}

	public static void ShowPageItems(int pageIndex, int curPage, int containerType, List<MainSlotPage> mainPages, GameObject itemPrefab, Transform dropParent, SlotScript[,] slotGrid, List<SlotDataPage> pages)
	{
		if (pageIndex < 0 || mainPages == null || pages == null || pageIndex >= mainPages.Count || pageIndex >= pages.Count)
		{
			return;
		}
		MainSlotPage mainSlotPage = mainPages[pageIndex];
		if (mainSlotPage?.MainList == null || mainSlotPage.MainList.Count == 0)
		{
			return;
		}
		foreach (SlotData main in mainSlotPage.MainList)
		{
			if (main != null && main.isMain)
			{
				SpawnItemUI(main, curPage, itemPrefab, dropParent, slotGrid, pages, containerType);
			}
		}
	}
}
