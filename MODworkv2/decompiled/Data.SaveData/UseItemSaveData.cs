using System;

namespace Data.SaveData;

[Serializable]
public class UseItemSaveData
{
	public int InfoType;

	public string UseType;

	public DamageType damageType;

	public int Number;

	public float CDTime;

	public int Duration;

	public int MstackSize;

	public int CstackSize;

	public int DropSpriteSize;

	public int ItemType;

	public int GlobalID;

	public string ItemName;

	public int Price;

	public int Quality;

	public IntVector2 Size;

	public IntVector2 SaveSlot;

	public int Level;

	public int SoundDrop;

	public int SoundUse;

	public int RotateType;

	public static UseItemSaveData FromRuntime(UseItemClass w)
	{
		return new UseItemSaveData
		{
			ItemType = w.ItemType,
			GlobalID = w.GlobalID,
			ItemName = w.ItemName,
			Price = w.Price,
			Quality = w.Quality,
			Size = w.Size,
			SaveSlot = w.SaveSlot,
			Level = w.Level,
			SoundDrop = w.SoundDrop,
			SoundUse = w.SoundUse,
			RotateType = w.RotateType,
			InfoType = w.InfoType,
			UseType = w.UseType,
			damageType = w.damageType,
			Number = w.Number,
			CDTime = w.CDTime,
			Duration = w.Duration,
			MstackSize = w.MstackSize,
			CstackSize = w.CstackSize,
			DropSpriteSize = w.DropSpriteSize
		};
	}

	public void ApplyToRuntime(UseItemClass w)
	{
		w.ItemType = ItemType;
		w.GlobalID = GlobalID;
		w.ItemName = ItemName;
		w.Price = Price;
		w.Quality = Quality;
		w.Size = Size;
		w.SaveSlot = SaveSlot;
		w.Level = Level;
		w.SoundDrop = SoundDrop;
		w.SoundUse = SoundUse;
		w.RotateType = RotateType;
		w.InfoType = InfoType;
		w.UseType = UseType;
		w.damageType = damageType;
		w.Number = Number;
		w.CDTime = CDTime;
		w.Duration = Duration;
		w.MstackSize = MstackSize;
		w.CstackSize = CstackSize;
		w.DropSpriteSize = DropSpriteSize;
	}
}
