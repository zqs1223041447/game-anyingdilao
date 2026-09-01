using System;

[Serializable]
public class ContainerItemData
{
	public int ItemType = -1;

	public WeaponClass weapon;

	public BaoshiClass baoshi;

	public UseItemClass useitem;

	public ItemScript ItemOBJ;

	[NonSerialized]
	public bool IsNewlyPicked;

	public int Page;

	public IntVector2 MainSlot;

	public IntVector2 ItemSize;

	public bool IsValid => ItemType >= 0;
}
