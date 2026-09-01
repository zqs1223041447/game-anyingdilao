using System;
using UnityEngine;

[Serializable]
public abstract class ItemClass
{
	public int ItemType;

	public int GlobalID;

	public string ItemName;

	public int Price;

	public int Quality;

	public IntVector2 Size;

	public IntVector2 SaveSlot;

	public Sprite Icon;

	public int Level;

	public int SoundDrop;

	public int SoundUse;

	public int RotateType;

	public virtual string GetTitle(bool displayEnhance = true)
	{
		string empty = string.Empty;
		return empty + "<color=" + QualityColor.Colors[Quality] + ">" + LOC.MM.GetItem(ItemName) + "</color>";
	}

	public virtual void Reset()
	{
		ItemName = string.Empty;
		GlobalID = 0;
		ItemType = 0;
		Quality = 0;
		Size = default(IntVector2);
		Icon = null;
		Level = 0;
		Price = 0;
		SoundDrop = 0;
		SoundUse = 0;
	}
}
