using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "SKprefab", menuName = "SK/SKprefab", order = 1)]
public class SKprefab : ScriptableObject
{
	public GameObject EmptyCol;

	public GameObject EmptyCol_BF;

	[Header("=========")]
	public SKprefab_OBJ[] SK_Group;

	public GameObject[] SK_FX;

	public GameObject[] CP_OBJ;

	public GameObject[] CP_FX;

	public GameObject[] CP_SPC;

	[Header("=========")]
	public SKprefab_OBJ[] Skill;

	public SKprefab_OBJ[] ATFX;

	public SKprefab_OBJ[] HitFX;

	public SKprefab_OBJ[] StartFX;

	public SKprefab_OBJ[] DieSP;

	[Header("=========")]
	public SKprefab_Multi[] Angle;

	public SKprefab_Multi[] Dic;

	public SKprefab_OBJ[] SubDic;

	public SKprefab_OBJ[] POS;

	public SKprefab_OBJ[] FX_shan;

	public SKprefab_OBJ[] FX_quan;

	public EventReference[] SoundRain;

	[Header("=========")]
	public GameObject[] Aura_SP;

	public GameObject[] Aura_EL;

	public GameObject[] LQJQ;

	public GameObject[] FrozenFX;

	public GameObject[] CutJump;

	public SKprefab_OBJ[] DotFX;

	public SKprefab_OBJ[] CPFX;

	public GameObject LevelUP;

	[Header("=========")]
	public GameObject[] XJ_Pen_1;

	public GameObject[] XJ_Pen_2;

	public GameObject[] spBreak;

	public GameObject[] spKuang;

	public GameObject[] spBaoshi;

	public GameObject[] spFlower;
}
