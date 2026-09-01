using System;
using System.Collections.Generic;

[Serializable]
public class Item_MB
{
	public string ItemName;

	public int GlobalID;

	public int ItemType;

	public int DropLevelStart;

	public int Quality;

	public int SizeX;

	public int SizeY;

	public int IconType;

	public int Icon;

	public int SoundDrop;

	public int SoundUse;

	public int RotateType;

	public int PLtype;

	public string WeaponType;

	public int CharType;

	public float Damage;

	public float Health;

	public float Mana;

	public float Element;

	public WPDT_A[] Main;

	public WPDT_A[] DOT;

	public WPDT_B[] SK;

	public WPDT_B[] CP;

	public WPDT_A[] RateMain;

	public WPDT_A[] RateDot;

	public WPDT_B[] RateSK;

	public WPDT_B[] RateCP;

	public int WP_SkillCount;

	public string SkillA;

	public int SkillA_count;

	public string SkillB;

	public int SkillB_count;

	public string SkillC;

	public int SkillC_count;

	public string SkillD;

	public int SkillD_count;

	public string SkillE;

	public int SkillE_count;

	public string SkillF;

	public int SkillF_count;

	public int MaxAocaoCount;

	public int CurAocaoCount;

	public List<WPSPC> SPC = new List<WPSPC>();

	public int Set_Index;
}
