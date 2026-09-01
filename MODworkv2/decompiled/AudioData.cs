using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData", order = 1)]
public class AudioData : ScriptableObject
{
	[Header("=========")]
	public string StartGame;

	public string OpenClose_UI;

	public string Exc_Open1;

	public string Exc_Open2;

	public string Exc_Open3;

	public string Save_Game_1;

	public string Save_Game_2;

	public string Save_Game_3;

	public string Save_Game_4;

	public string Save_Game_5;

	public string Setting_Check_Mark;

	public string Slider_1;

	public string Slider_2;

	[Header("=========")]
	public string Character;

	public string Chuan_Close;

	public string Chuan_Loading;

	public string Chuan_Open_1;

	public string Chuan_Open_2;

	public string Chuan_Open_3;

	public string Close_Panel;

	public string Enchant_Close;

	public string Enchant_Open;

	public string Map_1;

	public string Map_2;

	public string Quest_Close;

	public string Quest_Complete_1;

	public string Quest_Complete_2;

	public string Quest_Open;

	[Header("=========")]
	public string Chest_PutAll;

	public string Chest_TaketAll;

	public string IV_Change_Page;

	public string IV_Close;

	public string IV_Full;

	public string IV_Opem;

	public string IV_Organize_1;

	public string IV_Organize_2;

	public string IV_Organize_3;

	public string Money;

	public string Money_Lit;

	public string Money_Null_1;

	public string Money_Null_2;

	public string Money_Null_3;

	[Header("=========")]
	public string Shop_Close;

	public string Store_Open;

	public string Store_Refresh;

	[Header("=========")]
	public string SkillTree_Open;

	public string Xi_Select;

	public string Quick_SK_Open;

	public string Quick_SK_Select;

	public string Skill_LevelMax;

	public string Skill_NoPoint1;

	public string Skill_NoPoint2;

	public string Skill_NoPoint3;

	[Header("=========")]
	public string Add_Point_1;

	public string Add_Point_2;

	public string Add_Point_3;

	public string Add_Point_4;

	public string Add_Point_5;

	public string Add_Point_6;

	public string Add_Point_7;

	public string Add_Point_8;

	public string Add_Point_9;

	public string Battle_Mod_1;

	public string Battle_Mod_2;

	public string LitButton_1;

	public string LitButton_2;

	public string LitButton_3;

	public string LitButton_4;

	public string LitButton_5;

	public string LitButton_6;

	public string Reforge;

	public string Reforge_Point_1;

	public string Reforge_Point_2;

	public string Reforge_Point_3;

	[Header("=========")]
	public string Pick_Item;

	public string Pick_Money;

	[Header("=========")]
	public WP_Sound WP_Staff;

	public WP_Sound WP_Book;

	public WP_Sound WP_Sword;

	public WP_Sound WP_Dun;

	public WP_Sound WP_Bow;

	public WP_Sound WP_Arrow;

	public WP_Sound WP_Offering;

	public WP_Sound WP_Head;

	public WP_Sound WP_Armor;

	public WP_Sound WP_Hand;

	public WP_Sound WP_Shoes;

	public WP_Sound WP_ORB;

	public USE_Sound Baoshi;

	public USE_Sound Potion;

	public USE_Sound Scoll;

	public USE_Sound SPC;

	[Header("=========")]
	public SoundGroup PenFire;

	[Header("=========")]
	public string[] BGM;

	public string[] Atom;

	public List<string> BossMB = new List<string>();

	[Header("=========")]
	public string[] SoundChest;

	public string[] SoundBreak;

	public string[] SoundTemple;

	[Header("=========")]
	public string[] StartSceneUI;
}
