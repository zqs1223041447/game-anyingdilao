using System;
using System.Collections.Generic;

[Serializable]
public class SlotData
{
	public IntVector2 GridPos;

	public int number;

	public int Page;

	[NonSerialized]
	public List<ContainerItemData> ItemList;

	[NonSerialized]
	public SlotIndexPage IndexPage;

	[NonSerialized]
	private ContainerItemData standaloneItem;

	[NonSerialized]
	private int standaloneItemIndex = -1;

	public int ItemIndex
	{
		get
		{
			if (IndexPage?.Indexes != null && GridPos.x >= 0 && GridPos.y >= 0 && GridPos.x < IndexPage.Indexes.GetLength(0) && GridPos.y < IndexPage.Indexes.GetLength(1))
			{
				return IndexPage.Indexes[GridPos.x, GridPos.y];
			}
			return standaloneItemIndex;
		}
		set
		{
			standaloneItemIndex = value;
			if (IndexPage?.Indexes != null && GridPos.x >= 0 && GridPos.y >= 0 && GridPos.x < IndexPage.Indexes.GetLength(0) && GridPos.y < IndexPage.Indexes.GetLength(1))
			{
				IndexPage.Indexes[GridPos.x, GridPos.y] = value;
			}
		}
	}

	public ContainerItemData ItemData
	{
		get
		{
			int itemIndex = ItemIndex;
			if (ItemList != null && itemIndex >= 0 && itemIndex < ItemList.Count)
			{
				return ItemList[itemIndex];
			}
			return standaloneItem;
		}
	}

	public IntVector2 ItemSize
	{
		get
		{
			return ItemData?.ItemSize ?? IntVector2.Zero;
		}
		set
		{
			if (!(value == IntVector2.Zero) || ItemData != null)
			{
				EnsureItemData().ItemSize = value;
			}
		}
	}

	public IntVector2 StartPos
	{
		get
		{
			return ItemData?.MainSlot ?? GridPos;
		}
		set
		{
			TryAttachToExistingMainItem(value);
			if (ItemData != null || !(value == GridPos))
			{
				ContainerItemData containerItemData = EnsureItemData();
				containerItemData.MainSlot = value;
				containerItemData.Page = Page;
			}
		}
	}

	public bool isOC
	{
		get
		{
			ContainerItemData itemData = ItemData;
			if (ItemIndex >= 0 && itemData != null)
			{
				return itemData.IsValid;
			}
			return false;
		}
		set
		{
			if (value)
			{
				EnsureItemData();
			}
			else
			{
				ClearItemIndex();
			}
		}
	}

	public bool isMain
	{
		get
		{
			ContainerItemData itemData = ItemData;
			if (itemData != null && itemData.Page == Page)
			{
				return itemData.MainSlot == GridPos;
			}
			return false;
		}
		set
		{
			if (value)
			{
				ContainerItemData containerItemData = EnsureItemData();
				containerItemData.Page = Page;
				containerItemData.MainSlot = GridPos;
			}
		}
	}

	public int ItemType
	{
		get
		{
			return ItemData?.ItemType ?? (-1);
		}
		set
		{
			if (value >= 0 || ItemData != null)
			{
				ContainerItemData containerItemData = EnsureItemData();
				containerItemData.ItemType = value;
				containerItemData.Page = Page;
			}
		}
	}

	public WeaponClass weapon
	{
		get
		{
			ContainerItemData itemData = ItemData;
			if (itemData == null)
			{
				return null;
			}
			if (itemData.weapon == null && (itemData.ItemType == 0 || itemData.ItemType < 0))
			{
				itemData.weapon = new WeaponClass();
			}
			return itemData.weapon;
		}
		set
		{
			if (value != null || ItemData != null)
			{
				EnsureItemData().weapon = value;
			}
		}
	}

	public BaoshiClass baoshi
	{
		get
		{
			ContainerItemData itemData = ItemData;
			if (itemData == null)
			{
				return null;
			}
			if (itemData.baoshi == null && (itemData.ItemType == 1 || itemData.ItemType < 0))
			{
				itemData.baoshi = new BaoshiClass();
			}
			return itemData.baoshi;
		}
		set
		{
			if (value != null || ItemData != null)
			{
				EnsureItemData().baoshi = value;
			}
		}
	}

	public UseItemClass useitem
	{
		get
		{
			ContainerItemData itemData = ItemData;
			if (itemData == null)
			{
				return null;
			}
			if (itemData.useitem == null && (itemData.ItemType == 2 || itemData.ItemType < 0))
			{
				itemData.useitem = new UseItemClass();
			}
			return itemData.useitem;
		}
		set
		{
			if (value != null || ItemData != null)
			{
				EnsureItemData().useitem = value;
			}
		}
	}

	public ItemScript ItemOBJ
	{
		get
		{
			return ItemData?.ItemOBJ;
		}
		set
		{
			if ((bool)value || ItemData != null)
			{
				EnsureItemData().ItemOBJ = value;
			}
		}
	}

	public void BindRuntimeData(List<ContainerItemData> itemList, SlotIndexPage indexPage)
	{
		ItemList = itemList;
		IndexPage = indexPage;
		ItemIndex = -1;
	}

	private ContainerItemData EnsureItemData()
	{
		int itemIndex = ItemIndex;
		if (ItemList != null)
		{
			if (itemIndex >= 0 && itemIndex < ItemList.Count && ItemList[itemIndex] != null)
			{
				return ItemList[itemIndex];
			}
			ContainerItemData containerItemData = CreateDefaultItemData();
			ItemList.Add(containerItemData);
			ItemIndex = ItemList.Count - 1;
			return containerItemData;
		}
		if (standaloneItem == null)
		{
			standaloneItem = CreateDefaultItemData();
		}
		if (standaloneItemIndex < 0)
		{
			ItemIndex = 0;
		}
		return standaloneItem;
	}

	private ContainerItemData CreateDefaultItemData()
	{
		return new ContainerItemData
		{
			ItemType = -1,
			Page = Page,
			MainSlot = GridPos,
			ItemSize = IntVector2.Zero
		};
	}

	public void ClearItemIndex()
	{
		int itemIndex = ItemIndex;
		if (ItemList != null && itemIndex >= 0 && itemIndex < ItemList.Count)
		{
			ContainerItemData containerItemData = ItemList[itemIndex];
			if (containerItemData != null && containerItemData.Page == Page && containerItemData.MainSlot == GridPos)
			{
				ItemList[itemIndex] = null;
			}
		}
		else if (standaloneItemIndex == itemIndex)
		{
			standaloneItem = null;
		}
		ItemIndex = -1;
	}

	private void TryAttachToExistingMainItem(IntVector2 mainSlot)
	{
		if (mainSlot == GridPos || IndexPage?.Indexes == null || mainSlot.x < 0 || mainSlot.y < 0 || mainSlot.x >= IndexPage.Indexes.GetLength(0) || mainSlot.y >= IndexPage.Indexes.GetLength(1))
		{
			return;
		}
		int num = IndexPage.Indexes[mainSlot.x, mainSlot.y];
		if (num >= 0 && num != ItemIndex)
		{
			int itemIndex = ItemIndex;
			if (ItemList != null && itemIndex >= 0 && itemIndex < ItemList.Count)
			{
				ItemList[itemIndex] = null;
			}
			ItemIndex = num;
		}
	}
}
