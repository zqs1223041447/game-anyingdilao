using System;

namespace Data.SaveData;

[Serializable]
public class BaoshiSaveData
{
	public string BStype;

	public int UseType;

	public int BS_Quality;

	public int Number;

	public int MstackSize;

	public int CstackSize;

	public int DropSpriteSize;

	public string SKname;

	public int FWtype;

	public int Index;

	public int EL;

	public float PRC;

	public int priceQulity;

	public int Xi;

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

	public static BaoshiSaveData FromRuntime(BaoshiClass w)
	{
		return new BaoshiSaveData
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
			BStype = w.BStype,
			UseType = w.UseType,
			BS_Quality = w.BS_Quality,
			Number = w.Number,
			MstackSize = w.MstackSize,
			CstackSize = w.CstackSize,
			DropSpriteSize = w.DropSpriteSize,
			SKname = w.SKname,
			FWtype = w.FWtype,
			Index = w.Index,
			EL = w.EL,
			PRC = w.PRC,
			priceQulity = w.priceQulity,
			Xi = w.Xi
		};
	}

	public void ApplyToRuntime(BaoshiClass w)
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
		w.BStype = BStype;
		w.UseType = UseType;
		w.BS_Quality = BS_Quality;
		w.Number = Number;
		w.MstackSize = MstackSize;
		w.CstackSize = CstackSize;
		w.DropSpriteSize = DropSpriteSize;
		w.SKname = SKname;
		w.FWtype = FWtype;
		w.Index = Index;
		w.EL = EL;
		w.PRC = PRC;
		w.priceQulity = priceQulity;
		w.Xi = Xi;
	}
}
