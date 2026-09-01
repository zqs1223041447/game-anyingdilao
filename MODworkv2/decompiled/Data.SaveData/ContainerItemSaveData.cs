using System;

namespace Data.SaveData;

[Serializable]
public class ContainerItemSaveData
{
	public int Page;

	public int GridX;

	public int GridY;

	public int ItemType;

	public WeaponSaveData Weapon;

	public BaoshiSaveData Baoshi;

	public UseItemSaveData UseItem;
}
