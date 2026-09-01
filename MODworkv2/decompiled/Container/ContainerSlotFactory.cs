using System.Collections.Generic;

namespace Container;

public static class ContainerSlotFactory
{
	public static SlotData CreateSlotData(int x, int y, int pageIndex, IntVector2 inventorySize, List<ContainerItemData> itemList = null, SlotIndexPage indexPage = null)
	{
		SlotData slotData = new SlotData();
		slotData.GridPos = new IntVector2(x, y);
		slotData.number = x + 1 + inventorySize.x * y;
		slotData.Page = pageIndex;
		slotData.BindRuntimeData(itemList, indexPage);
		return slotData;
	}
}
