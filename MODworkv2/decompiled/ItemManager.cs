using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Settings;
using Entity.InteractableObjects.Item;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using Level.LevelStates;
using Level.StateData.LevelStates;
using Mijing;
using UI.Panels;
using UnityEngine;

public class ItemManager : SingletonMonoScope<ItemManager>
{
	public struct WeaponDropContext
	{
		public int DropScene;

		public int MJ_Level;

		public bool IsMijing => DropScene > 0;
	}

	public enum WeaponStatGroup
	{
		Main,
		Dot,
		Skill,
		Companion
	}

	private const string HomeScrollName = "Teleport Scroll";

	private const string SkillTextAssetPath = "Assets/Scripts/Container/UFT/0 4 Skill.csv";

	private const int SPCFWTypeCount = 5;

	private static readonly int[] SPCFWTypeWeights = new int[5] { 40, 20, 20, 10, 10 };

	public GameObject dropOBJ;

	public IconData[] IconData;

	public IconData IconBaoshi;

	public IconData IconUse;

	public Sprite[] SkillFW_Icon;

	public Sprite SPCFW_Icon;

	public Sprite BaseFW_Icon;

	public Sprite[] Double_Icon;

	public TextAsset WPtext;

	public TextAsset SPCtext;

	public TextAsset BStext;

	public TextAsset USEtext;

	public TextAsset Skilltext;

	public TextAsset Settext;

	public TextAsset Maintext;

	public TextAsset Dottext;

	public TextAsset SKtext;

	public PLtype_Group Weapon;

	public readonly Dictionary<int, SPC_MB> SPC = new Dictionary<int, SPC_MB>();

	public readonly Dictionary<int, SPC_MB> SPC_Rune = new Dictionary<int, SPC_MB>();

	public List<BaoshiClass> Baoshi = new List<BaoshiClass>();

	public List<BaoshiClass> BaoshiJH = new List<BaoshiClass>();

	public List<BaoshiClass> BaoshiSPC = new List<BaoshiClass>();

	public BaoshiClass SkillFW = new BaoshiClass();

	public BaoshiClass SPCFW = new BaoshiClass();

	public List<BaoshiClass> BaseFW = new List<BaoshiClass>();

	public List<UseItemClass> Potion = new List<UseItemClass>();

	public List<UseItemClass> BuffPotion = new List<UseItemClass>();

	public readonly Dictionary<string, UseItemClass> Scroll = new Dictionary<string, UseItemClass>();

	public List<UseItemClass> PremPotion = new List<UseItemClass>();

	public readonly Dictionary<string, UseItemClass> SpcPotion = new Dictionary<string, UseItemClass>();

	public readonly Dictionary<string, UseItemClass> SpcItem = new Dictionary<string, UseItemClass>();

	public readonly SPCMB_Group SPCMB = new SPCMB_Group();

	public readonly Dictionary<int, Set_DT> SET = new Dictionary<int, Set_DT>();

	public readonly Dictionary<int, WPDT_RandomA> WP_Main = new Dictionary<int, WPDT_RandomA>();

	public readonly Dictionary<int, WPDT_RandomA> WP_DOT = new Dictionary<int, WPDT_RandomA>();

	public readonly Dictionary<int, WPDT_RandomB> WP_SK = new Dictionary<int, WPDT_RandomB>();

	public readonly Dictionary<int, WPDT_RandomB> WP_CP = new Dictionary<int, WPDT_RandomB>();

	[HideInInspector]
	public float DR_EM;

	[HideInInspector]
	public float RandomCount;

	[HideInInspector]
	public float MultiLevelA;

	[HideInInspector]
	public float MultiLevelB;

	[HideInInspector]
	public float RDEL;

	public List<NoSameRD> NoS_EL = new List<NoSameRD>();

	public NoSameRD[] RD_EL = new NoSameRD[6];

	public List<NoSameRD> NoS_CP = new List<NoSameRD>();

	public NoSameRD[] RD_CP = new NoSameRD[5];

	private int Cur_Q;

	private PlayerManager PL;

	private TalentManager TL;

	private ShopManager shop;

	private readonly Dictionary<Sprite, BaoshiClass> baoshiByIcon = new Dictionary<Sprite, BaoshiClass>();

	private readonly Dictionary<string, BaoshiClass> baoshiByItemName = new Dictionary<string, BaoshiClass>();

	private readonly HashSet<DropItemController> AliveDropItems = new HashSet<DropItemController>();

	public float DR_Normal => Mathf.Max(0f, 2400f - 1.5f * (PL.ItemDrop_Rate_Last + DR_EM));

	public float DR_Magic => Mathf.Max(0f, 1600f - 0.5f * (PL.ItemDrop_Rate_Last + DR_EM));

	public float DR_Rare => Mathf.Max(0f, 900f + 0.3f * (PL.ItemDrop_Rate_Last + DR_EM));

	public float DR_Exquisite => Mathf.Max(0f, 500f + 0.8f * (PL.ItemDrop_Rate_Last + DR_EM));

	public float DR_Epic => Mathf.Max(0f, 180f + 0.9f * (PL.ItemDrop_Rate_Last + DR_EM));

	public float DR_Legendary => Mathf.Max(0f, 60f + 1f * (PL.ItemDrop_Rate_Last + DR_EM));

	public float DR_Mythical => Mathf.Max(0f, 10f + 1.1f * (PL.ItemDrop_Rate_Last + DR_EM));

	public float DR_Max => DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic + DR_Legendary + DR_Mythical;

	protected override void Awake()
	{
		base.Awake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		dropOBJ = Singleton<ResManager>.Instance.Load<GameObject>("res://Item/Drop Item");
		PL = SingletonMonoScope<PlayerManager>.Instance;
		TL = SingletonMonoScope<TalentManager>.Instance;
		shop = SingletonMonoScope<ShopManager>.Instance;
		if ((bool)TL)
		{
			TL.EnsureTalentTablesLoaded();
		}
		LoadData_RandomA(Maintext, WP_Main);
		LoadData_RandomA(Dottext, WP_DOT);
		LoadData_RandomMergedSkillB(SKtext);
		LoadData_WP(WPtext);
		LoadData_SPC(SPCtext);
		LoadData_BS(BStext);
		LoadData_USE(USEtext);
		LoadData_SET(Settext);
		LoadData_Skill(Skilltext);
		RD_EL[0].Index = 0;
		RD_EL[1].Index = 1;
		RD_EL[2].Index = 2;
		RD_EL[3].Index = 3;
		RD_EL[4].Index = 4;
		RD_EL[5].Index = 5;
		RD_CP[0].Index = 0;
		RD_CP[1].Index = 1;
		RD_CP[2].Index = 2;
		RD_CP[3].Index = 3;
		RD_CP[4].Index = 4;
	}

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	private void Start()
	{
		RandomCount = 0.005f;
		MultiLevelA = 1.066f;
		MultiLevelB = 1.03f;
		RDEL = 0.3f;
	}

	public void ChestDrop(Transform trans, float high, int quality)
	{
		int currentEnemyLevel = LevelManager.GetCurrentEnemyLevel();
		if (quality == 0)
		{
			int num = UnityEngine.Random.Range(0, 101);
			if (num < 10)
			{
				for (int i = 0; i < 2; i++)
				{
					DropWeapon(trans, high, currentEnemyLevel, UnityEngine.Random.Range(0, 4), PL.ItemDrop_Rate_Last);
				}
			}
			else if (num < 80)
			{
				DropWeapon(trans, high, currentEnemyLevel, UnityEngine.Random.Range(0, 4), PL.ItemDrop_Rate_Last);
			}
			if (ItemRoll(0.05f))
			{
				DropBaoshi(trans, high, currentEnemyLevel);
			}
			int num2 = UnityEngine.Random.Range(0, 3);
			for (int j = 0; j < num2; j++)
			{
				DropAnyPotion(trans, high, PL.Level);
			}
			EM_Drop_Item(trans, high, currentEnemyLevel, 0.03f, 0.003f, 0.001f, 5E-05f, 5E-05f, 0.01f, 0.001f, 3E-05f, 2E-05f);
			EM_Drop_MJ_SPC(trans, high, currentEnemyLevel, 0.002f, 0.002f, 0.0004f, 0.005f, 0.002f, 0.002f, 8E-05f, 8E-05f, 8E-05f, 4E-05f, 4E-05f, 8E-05f, 8E-05f, 8E-05f);
			return;
		}
		int num3 = UnityEngine.Random.Range(0, 101);
		if (num3 < 20)
		{
			for (int k = 0; k < 3; k++)
			{
				DropWeapon(trans, high, currentEnemyLevel, UnityEngine.Random.Range(0, 4), PL.ItemDrop_Rate_Last * 2f);
			}
		}
		else if (num3 < 80)
		{
			for (int l = 0; l < 2; l++)
			{
				DropWeapon(trans, high, currentEnemyLevel, UnityEngine.Random.Range(0, 4), PL.ItemDrop_Rate_Last * 2f);
			}
		}
		else
		{
			DropWeapon(trans, high, currentEnemyLevel, UnityEngine.Random.Range(0, 4), PL.ItemDrop_Rate_Last * 2f);
		}
		if (ItemRoll(0.3f))
		{
			DropBaoshi(trans, high, currentEnemyLevel);
		}
		int num4 = UnityEngine.Random.Range(1, 3);
		for (int m = 0; m < num4; m++)
		{
			DropAnyPotion(trans, high, PL.Level);
		}
		EM_Drop_Item(trans, high, currentEnemyLevel, 0.1f, 0.0073f, 0.002f, 0.0005f, 5E-05f, 0.05f, 0.004f, 0.0003f, 0.0002f);
		EM_Drop_MJ_SPC(trans, high, currentEnemyLevel, 0.01f, 0.01f, 0.003f, 0.03f, 0.01f, 0.01f, 0.001f, 0.001f, 0.001f, 0.0005f, 0.0005f, 0.001f, 0.001f, 0.001f);
		if (ItemRoll(0.003f))
		{
			DropScroll(trans, high, currentEnemyLevel, "Random Challenge Scroll");
		}
	}

	public void BreakDrop(Transform trans, int type)
	{
		int currentEnemyLevel = LevelManager.GetCurrentEnemyLevel();
		switch (type)
		{
		case 0:
		case 1:
		case 2:
			if (WPRoll(0.03f))
			{
				DropWeapon(trans, 0f, currentEnemyLevel, 0, PL.ItemDrop_Rate_Last);
			}
			if (ItemRoll(0.02f))
			{
				DropBaoshi(trans, 0f, currentEnemyLevel);
			}
			if (ItemRoll(0.03f))
			{
				DropAnyPotion(trans, 0f, PL.Level);
			}
			if (ItemRoll(0.01f))
			{
				DropBuffPotion(trans, 0f, PL.Level);
			}
			if (ItemRoll(0.0005f))
			{
				DropBuffPotion(trans, 0f, PL.Level);
			}
			break;
		case 3:
			if (ItemRoll(0.05f))
			{
				DropBaoshi(trans, 0f, currentEnemyLevel);
			}
			break;
		case 4:
			break;
		}
	}

	public void EM_Drop(Enemy em)
	{
		int num = 10;
		switch (em.Quality)
		{
		case 0:
			num = 10;
			break;
		case 1:
			num = 30;
			break;
		case 2:
			num = 60;
			break;
		case 3:
			num = 100;
			break;
		case 4:
			num = 350;
			break;
		case 5:
			num = 600;
			break;
		}
		switch (em.Quality)
		{
		case 0:
			if (WPRoll(0.03f))
			{
				DropWeapon(em.transform, em.ItemDropPos, em.Level, 0, num);
			}
			if (ItemRoll(0.005f))
			{
				DropBaoshi(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.005f))
			{
				DropAnyPotion(em.transform, em.ItemDropPos, em.Level);
			}
			EM_Drop_Item(em.transform, em.ItemDropPos, em.Level, 0.005f, 0.0001f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
			break;
		case 1:
			if (WPRoll(0.05f))
			{
				DropWeapon(em.transform, em.ItemDropPos, em.Level, 0, num);
			}
			if (ItemRoll(0.01f))
			{
				DropBaoshi(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.01f))
			{
				DropAnyPotion(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.0003f))
			{
				DropScroll(em.transform, em.ItemDropPos, em.Level, "Random Challenge Scroll");
			}
			EM_Drop_Item(em.transform, em.ItemDropPos, em.Level, 0.01f, 0.002f, 0.001f, 0.0001f, 5E-05f, 0.002f, 0.0004f, 0.0003f, 0.0002f);
			EM_Drop_MJ_SPC(em.transform, em.ItemDropPos, em.Level, 0.0008f, 0.0008f, 0.0002f, 0.002f, 0.0008f, 0.0008f, 3E-05f, 3E-05f, 3E-05f, 1E-05f, 1E-05f, 3E-05f, 3E-05f, 3E-05f);
			break;
		case 2:
			if (WPRoll(0.08f))
			{
				DropWeapon(em.transform, em.ItemDropPos, em.Level, 0, num);
			}
			if (ItemRoll(0.02f))
			{
				DropBaoshi(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.1f))
			{
				DropAnyPotion(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.002f))
			{
				DropScroll(em.transform, em.ItemDropPos, em.Level, "Random Challenge Scroll");
			}
			EM_Drop_Item(em.transform, em.ItemDropPos, em.Level, 0.02f, 0.005f, 0.003f, 0.0003f, 0.0001f, 0.005f, 0.0012f, 0.0006f, 0.0004f);
			EM_Drop_MJ_SPC(em.transform, em.ItemDropPos, em.Level, 0.002f, 0.002f, 0.0004f, 0.005f, 0.002f, 0.002f, 8E-05f, 8E-05f, 8E-05f, 4E-05f, 4E-05f, 8E-05f, 8E-05f, 8E-05f);
			break;
		case 3:
		{
			int num5 = UnityEngine.Random.Range(1, 3);
			for (int l = 0; l < num5; l++)
			{
				DropWeapon(em.transform, em.ItemDropPos, em.Level, 0, num);
			}
			if (ItemRoll(0.1f))
			{
				DropBaoshi(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.1f))
			{
				DropAnyPotion(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.01f))
			{
				DropScroll(em.transform, em.ItemDropPos, em.Level, "Random Challenge Scroll");
			}
			EM_Drop_Item(em.transform, em.ItemDropPos, em.Level, 0.15f, 0.03f, 0.01f, 0.01f, 0.003f, 0.02f, 0.008f, 0.006f, 0.004f);
			EM_Drop_MJ_SPC(em.transform, em.ItemDropPos, em.Level, 0.015f, 0.015f, 0.003f, 0.03f, 0.015f, 0.015f, 0.0006f, 0.0006f, 0.0006f, 0.0003f, 0.0003f, 0.0006f, 0.0006f, 0.0006f);
			break;
		}
		case 4:
		{
			int num6 = UnityEngine.Random.Range(3, 6);
			for (int m = 0; m < num6; m++)
			{
				DropWeapon(em.transform, em.ItemDropPos, em.Level, 0, num);
			}
			if (ItemRoll(0.5f))
			{
				DropBaoshi(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.8f))
			{
				DropAnyPotion(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.03f))
			{
				DropScroll(em.transform, em.ItemDropPos, em.Level, "Random Challenge Scroll");
			}
			EM_Drop_Item(em.transform, em.ItemDropPos, em.Level, 0.7f, 0.2f, 0.1f, 0.02f, 0.01f, 0.1f, 0.02f, 0.012f, 0.008f);
			EM_Drop_MJ_SPC(em.transform, em.ItemDropPos, em.Level, 0.4f, 0.4f, 0.1f, 0.9f, 0.4f, 0.4f, 0.05f, 0.05f, 0.05f, 0.025f, 0.025f, 0.05f, 0.05f, 0.05f);
			break;
		}
		case 5:
		{
			int num2 = UnityEngine.Random.Range(4, 9);
			for (int i = 0; i < num2; i++)
			{
				DropWeapon(em.transform, em.ItemDropPos, em.Level, 0, num);
			}
			int num3 = UnityEngine.Random.Range(1, 3);
			for (int j = 0; j < num3; j++)
			{
				DropBaoshi(em.transform, em.ItemDropPos, em.Level);
			}
			int num4 = UnityEngine.Random.Range(1, 4);
			for (int k = 0; k < num4; k++)
			{
				DropAnyPotion(em.transform, em.ItemDropPos, em.Level);
			}
			if (ItemRoll(0.08f))
			{
				DropScroll(em.transform, em.ItemDropPos, em.Level, "Random Challenge Scroll");
			}
			EM_Drop_Item(em.transform, em.ItemDropPos, em.Level, 0.9f, 0.4f, 0.1f, 0.03f, 0.01f, 0.1f, 0.04f, 0.02f, 0.01f);
			EM_Drop_MJ_SPC(em.transform, em.ItemDropPos, em.Level, 0.7f, 0.7f, 0.2f, 0.9f, 0.7f, 0.7f, 0.1f, 0.1f, 0.1f, 0.05f, 0.05f, 0.1f, 0.1f, 0.1f);
			break;
		}
		}
	}

	public void BossDropFirst(Transform trans, float high, bool isChapterFinal)
	{
		if (isChapterFinal)
		{
			DropSpcPotion(trans, high, "Blessing Potion");
			DropSpcItem(trans, high, "Void Treasure Bag");
			DropSpcItem(trans, high, "Chest Key");
			switch (ProbUtil.Roll(70, 30))
			{
			case 0:
				MustDropPremPotion(trans, high);
				MustDropPremPotion(trans, high);
				break;
			case 1:
				MustDropPremPotion(trans, high);
				MustDropPremPotion(trans, high);
				MustDropPremPotion(trans, high);
				break;
			}
		}
		else
		{
			switch (ProbUtil.Roll(70, 30))
			{
			case 0:
				MustDropPremPotion(trans, high);
				break;
			case 1:
				MustDropPremPotion(trans, high);
				MustDropPremPotion(trans, high);
				break;
			}
		}
	}

	public void EM_Drop_Item(Transform trans, float high, int level, float A, float B, float C, float D, float E, float F, float G, float H, float I)
	{
		if (ItemRoll(A))
		{
			DropBuffPotion(trans, high, level);
		}
		if (ItemRoll(B))
		{
			DropPremPotion(trans, high, level);
		}
		ItemRoll(C);
		if (ItemRoll(D))
		{
			DropSpcPotion(trans, high, level, "Blessing Potion");
		}
		if (ItemRoll(E))
		{
			DropSpcPotion(trans, high, level, "Awakening Potion");
		}
		if (ItemRoll(F))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_KZ");
		}
		if (ItemRoll(G))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_FS");
		}
		if (ItemRoll(H))
		{
			DropSpcItem(trans, high, level, "Void Treasure Bag");
		}
		if (ItemRoll(I))
		{
			DropSpcItem(trans, high, level, "Chest Key");
		}
	}

	public void EM_Drop_MJ_SPC(Transform trans, float high, float A, float B, float C, float D, float E, float F, float G, float H, float I, float J, float K, float L, float M, float N)
	{
		EM_Drop_MJ_SPC(trans, high, LevelManager.GetCurrentEnemyLevel(), A, B, C, D, E, F, G, H, I, J, K, L, M, N);
	}

	public void EM_Drop_MJ_SPC(Transform trans, float high, int level, float A, float B, float C, float D, float E, float F, float G, float H, float I, float J, float K, float L, float M, float N)
	{
		if (MJRoll(A))
		{
			DropSkillFW(trans, high);
		}
		if (MJRoll(B))
		{
			DropSPCFW(trans, high);
		}
		if (MJRoll(C))
		{
			DropBaseFW(trans, high);
		}
		if (MJRoll(D))
		{
			DropRandomEssence(trans, high, level);
		}
		if (MJRoll(E))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_HH");
		}
		if (MJRoll(F))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_AM");
		}
		if (MJRoll(G))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_HM");
		}
		if (MJRoll(H))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_CG");
		}
		if (MJRoll(I))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_LC");
		}
		if (MJRoll(J))
		{
			DropSpcItem(trans, high, level, "Gold Hammer");
		}
		if (MJRoll(K))
		{
			DropSpcItem(trans, high, level, "Platinum Hammer");
		}
		if (MJRoll(L))
		{
			DropSpcItem(trans, high, level, "Treasure Mirror");
		}
		if (MJRoll(M))
		{
			DropSpcItem(trans, high, level, "Elf Mirror");
		}
		if (MJRoll(N))
		{
			DropSpcItem(trans, high, level, "Disassembly Mirror");
		}
	}

	public void Chest_Drop_Item(Transform trans, float high, int level, float A, float B, float C, float D, float E, float F, float G, float H, float I)
	{
		if (ItemRoll(A))
		{
			DropBuffPotion(trans, high, level);
		}
		if (ItemRoll(B))
		{
			DropPremPotion(trans, high, level);
		}
		if (ItemRoll(C))
		{
			DropSpcPotion(trans, high, level, "Forgetfulness Potion");
		}
		if (ItemRoll(D))
		{
			DropSpcPotion(trans, high, level, "Blessing Potion");
		}
		if (ItemRoll(E))
		{
			DropSpcPotion(trans, high, level, "Awakening Potion");
		}
		if (ItemRoll(F))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_KZ");
		}
		if (ItemRoll(G))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_FS");
		}
		if (ItemRoll(H))
		{
			DropSpcItem(trans, high, level, "Void Treasure Bag");
		}
		if (ItemRoll(I))
		{
			DropSpcItem(trans, high, level, "Chest Key");
		}
	}

	public void Chest_Drop_MJ_SPC(Transform trans, float high, float A, float B, float C, float D, float E, float F, float G, float H, float I, float J, float K, float L, float M, float N)
	{
		Chest_Drop_MJ_SPC(trans, high, LevelManager.GetCurrentEnemyLevel(), A, B, C, D, E, F, G, H, I, J, K, L, M, N);
	}

	public void Chest_Drop_MJ_SPC(Transform trans, float high, int level, float A, float B, float C, float D, float E, float F, float G, float H, float I, float J, float K, float L, float M, float N)
	{
		if (MJRoll(A))
		{
			DropSkillFW(trans, high);
		}
		if (MJRoll(B))
		{
			DropSPCFW(trans, high);
		}
		if (MJRoll(C))
		{
			DropBaseFW(trans, high);
		}
		if (MJRoll(D))
		{
			DropRandomEssence(trans, high, level);
		}
		if (MJRoll(E))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_HH");
		}
		if (MJRoll(F))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_AM");
		}
		if (MJRoll(G))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_HM");
		}
		if (MJRoll(H))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_CG");
		}
		if (MJRoll(I))
		{
			DropSpecialBaoshi(trans, high, level, "Stone_LC");
		}
		if (MJRoll(J))
		{
			DropSpcItem(trans, high, level, "Gold Hammer");
		}
		if (MJRoll(K))
		{
			DropSpcItem(trans, high, level, "Platinum Hammer");
		}
		if (MJRoll(L))
		{
			DropSpcItem(trans, high, level, "Treasure Mirror");
		}
		if (MJRoll(M))
		{
			DropSpcItem(trans, high, level, "Elf Mirror");
		}
		if (MJRoll(N))
		{
			DropSpcItem(trans, high, level, "Disassembly Mirror");
		}
	}

	public void WorldRollAll(Transform trans, float high, int level, float A, float B, float C, float D, float E, float F, float G, float H, float I, float J, float K, float L, float M, float N, float O, float P, float Q, float R, float S, float T, float U, float V, float W)
	{
		if (ItemRoll(A))
		{
			DropScroll(trans, high, level, "Random Challenge Scroll");
		}
	}

	private static void SpawnWeaponToInventory(Action<WeaponClass> setupAction)
	{
		if (setupAction != null)
		{
			SlotData slotData = new SlotData
			{
				ItemType = 0,
				weapon = new WeaponClass()
			};
			setupAction(slotData.weapon);
			SingletonMonoScope<InventoryManager>.Instance.CreateWeapon(slotData);
		}
	}

	private static void SpawnBaoshiToInventory(Action<BaoshiClass> setupAction)
	{
		if (setupAction != null)
		{
			SlotData slotData = new SlotData
			{
				ItemType = 1,
				baoshi = new BaoshiClass()
			};
			setupAction(slotData.baoshi);
			SingletonMonoScope<InventoryManager>.Instance.CreateBaoshi(slotData);
		}
	}

	private static void SpawnUseItemToInventory(Action<UseItemClass> setupAction)
	{
		if (setupAction != null)
		{
			SlotData slotData = new SlotData
			{
				ItemType = 2,
				useitem = new UseItemClass()
			};
			setupAction(slotData.useitem);
			SingletonMonoScope<InventoryManager>.Instance.CreatePotion(slotData);
		}
	}

	public void CreatSingleWeaponAll(int level, int type, int quality)
	{
		List<Item_MB> list = null;
		switch (quality)
		{
		case 0:
			list = Weapon.GP[PL.PLType].QL[type].Normal;
			break;
		case 1:
			list = Weapon.GP[PL.PLType].QL[type].Magic;
			break;
		case 2:
			list = Weapon.GP[PL.PLType].QL[type].Rare;
			break;
		case 3:
			list = Weapon.GP[PL.PLType].QL[type].Exquisite;
			break;
		case 4:
			list = Weapon.GP[PL.PLType].QL[type].Epic;
			break;
		case 5:
			list = Weapon.GP[PL.PLType].QL[type].Legendary;
			break;
		case 6:
			list = Weapon.GP[PL.PLType].QL[type].Mythical;
			break;
		}
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (Item_MB mb in list)
		{
			if (mb != null)
			{
				SpawnWeaponToInventory(delegate(WeaponClass wp)
				{
					SetWPdata(wp, mb, level);
				});
			}
		}
	}

	public void CreatBaoshiAll()
	{
		for (int i = 0; i < Baoshi.Count; i++)
		{
			BaoshiClass source3 = Baoshi[i];
			SpawnBaoshiToInventory(delegate(BaoshiClass bs)
			{
				ApplyDebugBaoshiTemplate(bs, source3);
			});
		}
		for (int j = 0; j < BaoshiJH.Count; j++)
		{
			BaoshiClass source2 = BaoshiJH[j];
			SpawnBaoshiToInventory(delegate(BaoshiClass bs)
			{
				ApplyDebugBaoshiTemplate(bs, source2);
			});
		}
		for (int k = 0; k < BaoshiSPC.Count; k++)
		{
			BaoshiClass source = BaoshiSPC[k];
			SpawnBaoshiToInventory(delegate(BaoshiClass bs)
			{
				ApplyDebugBaoshiTemplate(bs, source);
			});
		}
	}

	public void CreatFW_SK_All()
	{
		CreateAllSkillFWRuneDebugItems();
	}

	public void CreatFW_SPC_All()
	{
		CreateAllSPCFWRuneDebugItems();
	}

	public void CreatFW_Base_All()
	{
		for (int i = 0; i < BaseFW.Count; i++)
		{
			BaoshiClass source = BaseFW[i];
			SpawnBaoshiToInventory(delegate(BaoshiClass bs)
			{
				ApplyDebugBaoshiTemplate(bs, source);
			});
		}
	}

	private void CreateAllSkillFWRuneDebugItems()
	{
		if (SkillFW == null)
		{
			return;
		}
		TalentManager talentManager = (TL ? TL : (SingletonMonoScope<TalentManager>.HasInstance ? SingletonMonoScope<TalentManager>.Instance : null));
		if (talentManager == null)
		{
			return;
		}
		talentManager.EnsureSkillFWLibrary();
		SKFW_Group fW = talentManager.FW;
		if (fW?.Char == null)
		{
			return;
		}
		for (int i = 0; i < fW.Char.Length; i++)
		{
			SKFW_Char sKFW_Char = fW.Char[i];
			if (sKFW_Char?.Xi == null)
			{
				continue;
			}
			for (int j = 0; j < sKFW_Char.Xi.Length; j++)
			{
				SKFW[] array = sKFW_Char.Xi[j]?.FW;
				if (array == null)
				{
					continue;
				}
				SKFW[] array2 = array;
				foreach (SKFW rune in array2)
				{
					if (rune != null && !string.IsNullOrEmpty(rune.SkillName))
					{
						SpawnBaoshiToInventory(delegate(BaoshiClass bs)
						{
							ApplyDebugBaoshiTemplate(bs, SkillFW);
							bs.UseType = 3;
							bs.SKname = rune.SkillName;
							bs.Index = rune.index;
							bs.EL = rune.EL;
							bs.Xi = rune.Xi;
							bs.priceQulity = rune.Price;
							bs.Price = GetSkillRunePrice(rune.SkillName);
							bs.Number = Mathf.Max(1, bs.Number);
							bs.MstackSize = Mathf.Max(1, bs.MstackSize);
							bs.CstackSize = Mathf.Max(1, bs.CstackSize);
							bs.Icon = GetSkillFWIcon(rune.EL);
						});
					}
				}
			}
		}
	}

	private void CreateAllSPCFWRuneDebugItems()
	{
		if (SPCFW == null || SPCMB?.MB == null)
		{
			return;
		}
		SPC_MB[] mB = SPCMB.MB;
		foreach (SPC_MB rune in mB)
		{
			if (rune == null || rune.SPCindex <= 0 || rune.SPCtype <= 0)
			{
				continue;
			}
			SpawnBaoshiToInventory(delegate(BaoshiClass bs)
			{
				ApplyDebugBaoshiTemplate(bs, SPCFW);
				bs.ItemType = 1;
				bs.UseType = 4;
				bs.SKname = rune.SPCname;
				bs.Index = rune.SPCindex;
				bs.EL = 0;
				bs.PRC = GivePRC_SPC(bs.Level, bs.Quality);
				bs.priceQulity = rune.Price;
				bs.Price = GetSPCRunePrice(bs.Index, bs.PRC);
				bs.FWtype = NormalizeSPCFWType(rune.FWtype);
				bs.Number = Mathf.Max(1, bs.Number);
				bs.MstackSize = Mathf.Max(1, bs.MstackSize);
				bs.CstackSize = Mathf.Max(1, bs.CstackSize);
				if ((bool)SPCFW_Icon)
				{
					bs.Icon = SPCFW_Icon;
				}
			});
		}
	}

	public void CreateUseAll()
	{
		for (int i = 0; i < Potion.Count; i++)
		{
			int index3 = i;
			SpawnUseItemToInventory(delegate(UseItemClass use)
			{
				SetPTdata(use, index3);
			});
		}
		for (int j = 0; j < BuffPotion.Count; j++)
		{
			int index2 = j;
			SpawnUseItemToInventory(delegate(UseItemClass use)
			{
				SetBuffPTdata(use, index2);
			});
		}
		CreateNamedScroll("Random Challenge Scroll");
		CreateNamedScroll("Arena Scroll");
		CreateNamedScroll("Demon Challenge Scroll");
		CreateNamedScroll("Infernal Challenge Scroll");
		CreateNamedScroll("Treasure Map");
		CreateNamedSpecialPotion("Blessing Potion");
		CreateNamedSpecialPotion("Awakening Potion");
		for (int k = 0; k < PremPotion.Count; k++)
		{
			int index = k;
			SpawnUseItemToInventory(delegate(UseItemClass use)
			{
				SetPremPTdata(use, index);
			});
		}
		CreateNamedSpecialItem("Void Treasure Bag");
		CreateNamedSpecialItem("Chest Key");
		CreateNamedSpecialItem("Silver Hammer");
		CreateNamedSpecialItem("Gold Hammer");
		CreateNamedSpecialItem("Platinum Hammer");
		CreateNamedSpecialItem("Titanium Hammer");
		CreateNamedSpecialItem("Arcanite Hammer");
		CreateNamedSpecialItem("Aurora Hammer");
		CreateNamedSpecialItem("Demon Hammer");
		CreateNamedSpecialItem("Nebula Hammer");
		CreateNamedSpecialItem("Treasure Mirror");
		CreateNamedSpecialItem("Elf Mirror");
		CreateNamedSpecialItem("Disassembly Mirror");
		CreateNamedSpecialItem("Ascension Mirror");
		CreateNamedSpecialItem("Godblessed Mirror");
		CreateNamedSpecialPotion("Forgetfulness Potion");
		CreateNamedSpecialPotion("Reincarnation Potion");
	}

	private void CreateNamedScroll(string itemName)
	{
		if (!string.IsNullOrEmpty(itemName))
		{
			SpawnUseItemToInventory(delegate(UseItemClass use)
			{
				SetSCata(use, itemName);
			});
		}
	}

	private void CreateNamedSpecialPotion(string itemName)
	{
		if (!string.IsNullOrEmpty(itemName))
		{
			SpawnUseItemToInventory(delegate(UseItemClass use)
			{
				SetSPCPTdata(use, itemName);
			});
		}
	}

	private void CreateNamedSpecialItem(string itemName)
	{
		if (!string.IsNullOrEmpty(itemName))
		{
			SpawnUseItemToInventory(delegate(UseItemClass use)
			{
				SetSPCItemdata(use, itemName);
			});
		}
	}

	private static void ApplyDebugBaoshiTemplate(BaoshiClass target, BaoshiClass source)
	{
		if (target != null && source != null)
		{
			ItemCloneUtil.CopyBaoshiTo(target, source);
			ApplyDebugRuneIcon(target);
			target.CstackSize *= 100;
		}
	}

	private static void ApplyDebugRuneIcon(BaoshiClass baoshi)
	{
		if (baoshi == null || !SingletonMonoScope<ItemManager>.HasInstance)
		{
			return;
		}
		ItemManager instance = SingletonMonoScope<ItemManager>.Instance;
		switch (baoshi.UseType)
		{
		case 3:
		{
			Sprite skillFWIcon = instance.GetSkillFWIcon(baoshi.EL);
			if ((bool)skillFWIcon)
			{
				baoshi.Icon = skillFWIcon;
			}
			break;
		}
		case 4:
			if ((bool)instance.SPCFW_Icon)
			{
				baoshi.Icon = instance.SPCFW_Icon;
			}
			break;
		case 5:
			if ((bool)instance.BaseFW_Icon)
			{
				baoshi.Icon = instance.BaseFW_Icon;
			}
			break;
		}
	}

	private static void ApplyDebugUseItemTemplate(UseItemClass target, UseItemClass source)
	{
		if (target != null && source != null)
		{
			ItemCloneUtil.CopyUseItemTo(target, source);
			target.CstackSize *= 100;
		}
	}

	public void CreatIVWeapon(int level, float dropRate)
	{
		DR_EM = dropRate;
		int charType = UnityEngine.Random.Range(0, 4);
		if (UnityEngine.Random.Range(0, 4) == 0)
		{
			charType = PL.PLType;
		}
		int num = UnityEngine.Random.Range(0, 2);
		int num2 = UnityEngine.Random.Range(0, 10);
		int weaponType = ((UnityEngine.Random.Range(0, 2) == 0) ? num2 : num);
		float qualityRoll = UnityEngine.Random.Range(0f, DR_Max + dropRate * 6f);
		List<Item_MB> weaponSourceList = GetWeaponSourceList(charType, weaponType, qualityRoll);
		if (weaponSourceList == null || weaponSourceList.Count == 0)
		{
			return;
		}
		List<Item_MB> list = FilterValidWeaponListByLevel(weaponSourceList, level);
		if (list.Count == 0)
		{
			return;
		}
		Item_MB mb = list[UnityEngine.Random.Range(0, list.Count)];
		if (mb != null)
		{
			SpawnWeaponToInventory(delegate(WeaponClass wp)
			{
				SetWPdata(wp, mb, level);
			});
		}
	}

	private List<Item_MB> GetWeaponSourceList(int charType, int weaponType, float qualityRoll)
	{
		if (qualityRoll <= DR_Normal)
		{
			return Weapon.GP[charType].QL[weaponType].Normal;
		}
		if (qualityRoll <= DR_Normal + DR_Magic)
		{
			return Weapon.GP[charType].QL[weaponType].Magic;
		}
		if (qualityRoll <= DR_Normal + DR_Magic + DR_Rare)
		{
			return Weapon.GP[charType].QL[weaponType].Rare;
		}
		if (qualityRoll <= DR_Normal + DR_Magic + DR_Rare + DR_Exquisite)
		{
			return Weapon.GP[charType].QL[weaponType].Exquisite;
		}
		if (qualityRoll <= DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic)
		{
			return Weapon.GP[charType].QL[weaponType].Epic;
		}
		if (qualityRoll <= DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic + DR_Legendary)
		{
			return Weapon.GP[charType].QL[weaponType].Legendary;
		}
		return Weapon.GP[charType].QL[weaponType].Mythical;
	}

	public bool TryRegenerateWeaponFromTemplate(WeaponClass weapon, int playerLevel)
	{
		if (weapon == null)
		{
			return false;
		}
		Item_MB item_MB = FindWeaponTemplate(weapon);
		if (item_MB == null)
		{
			return false;
		}
		int level = Mathf.Max(weapon.Level, playerLevel);
		SetWPdata(weapon, item_MB, level, GetWeaponDropContext(weapon));
		return true;
	}

	private Item_MB FindWeaponTemplate(WeaponClass weapon)
	{
		if (weapon == null || Weapon?.GP == null)
		{
			return null;
		}
		if (weapon.PLtype >= 0 && weapon.PLtype < Weapon.GP.Length)
		{
			Item_MB item_MB = FindWeaponTemplateInPlayer(weapon.PLtype, weapon);
			if (item_MB != null)
			{
				return item_MB;
			}
		}
		for (int i = 0; i < Weapon.GP.Length; i++)
		{
			Item_MB item_MB2 = FindWeaponTemplateInPlayer(i, weapon);
			if (item_MB2 != null)
			{
				return item_MB2;
			}
		}
		return null;
	}

	private Item_MB FindWeaponTemplateInPlayer(int plType, WeaponClass weapon)
	{
		if (plType < 0 || Weapon?.GP == null || plType >= Weapon.GP.Length || weapon == null)
		{
			return null;
		}
		Weapon_Group weapon_Group = Weapon.GP[plType];
		if (weapon_Group?.QL == null || weapon.CharType < 0 || weapon.CharType >= weapon_Group.QL.Length)
		{
			return null;
		}
		Quality_Group group = weapon_Group.QL[weapon.CharType];
		Item_MB item_MB = FindWeaponTemplateInList(GetWeaponQualityList(group, weapon.Quality), weapon);
		if (item_MB != null)
		{
			return item_MB;
		}
		for (int i = 0; i <= 6; i++)
		{
			item_MB = FindWeaponTemplateInList(GetWeaponQualityList(group, i), weapon);
			if (item_MB != null)
			{
				return item_MB;
			}
		}
		return null;
	}

	private static Item_MB FindWeaponTemplateInList(List<Item_MB> list, WeaponClass weapon)
	{
		if (list == null || weapon == null)
		{
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Item_MB item_MB = list[i];
			if (item_MB != null)
			{
				if (weapon.GlobalID > 0 && item_MB.GlobalID == weapon.GlobalID)
				{
					return item_MB;
				}
				if (!string.IsNullOrEmpty(weapon.ItemName) && item_MB.ItemName == weapon.ItemName)
				{
					return item_MB;
				}
			}
		}
		return null;
	}

	private static List<Item_MB> GetWeaponQualityList(Quality_Group group, int quality)
	{
		if (group == null)
		{
			return null;
		}
		return quality switch
		{
			0 => group.Normal, 
			1 => group.Magic, 
			2 => group.Rare, 
			3 => group.Exquisite, 
			4 => group.Epic, 
			5 => group.Legendary, 
			6 => group.Mythical, 
			_ => null, 
		};
	}

	private static List<Item_MB> FilterValidWeaponListByLevel(List<Item_MB> sourceList, int level)
	{
		List<Item_MB> list = new List<Item_MB>();
		if (sourceList == null || sourceList.Count == 0)
		{
			return list;
		}
		for (int i = 0; i < sourceList.Count; i++)
		{
			Item_MB item_MB = sourceList[i];
			if (item_MB != null && item_MB.DropLevelStart <= level)
			{
				list.Add(item_MB);
			}
		}
		return list;
	}

	public void SetBSdata(BaoshiClass bs, int index)
	{
		if (bs != null && index >= 0 && index < Baoshi.Count)
		{
			ApplyDebugBaoshiTemplate(bs, Baoshi[index]);
		}
	}

	public void SetPTdata(UseItemClass pt, int index)
	{
		if (pt != null && index >= 0 && index < Potion.Count)
		{
			ApplyDebugUseItemTemplate(pt, Potion[index]);
		}
	}

	public void SetBuffPTdata(UseItemClass bf, int index)
	{
		if (bf != null && index >= 0 && index < BuffPotion.Count)
		{
			ApplyDebugUseItemTemplate(bf, BuffPotion[index]);
		}
	}

	public void SetSCata(UseItemClass sc, string itemName)
	{
		if (sc != null && !string.IsNullOrEmpty(itemName) && Scroll.TryGetValue(itemName, out var value) && value != null)
		{
			ApplyDebugUseItemTemplate(sc, value);
		}
	}

	public void SetSPCPTdata(UseItemClass spcPT, string itemName)
	{
		if (spcPT != null && !string.IsNullOrEmpty(itemName) && SpcPotion.TryGetValue(itemName, out var value) && value != null)
		{
			ApplyDebugUseItemTemplate(spcPT, value);
		}
	}

	public void SetSPCItemdata(UseItemClass item, string itemName)
	{
		if (item != null && !string.IsNullOrEmpty(itemName) && SpcItem.TryGetValue(itemName, out var value) && value != null)
		{
			ApplyDebugUseItemTemplate(item, value);
		}
	}

	public void SetPremPTdata(UseItemClass bf, int index)
	{
		if (bf != null && index >= 0 && index < PremPotion.Count)
		{
			ApplyDebugUseItemTemplate(bf, PremPotion[index]);
		}
	}

	public void CreatShop()
	{
		float num = PL.Level;
		int num2 = UnityEngine.Random.Range(20 + Mathf.FloorToInt(num / 5f), 30 + Mathf.FloorToInt(num / 5f));
		for (int i = 0; i < num2; i++)
		{
			CreatWeapon(PL.Level, shop.CurrentPremiumDropRate);
		}
		int num3 = UnityEngine.Random.Range(4, 10);
		for (int j = 0; j < num3; j++)
		{
			CreatPotion(PL.Level);
		}
		int num4 = UnityEngine.Random.Range(2, 6);
		for (int k = 0; k < num4; k++)
		{
			if ((float)UnityEngine.Random.Range(0, 101) < 20f + shop.CurrentPremiumDropRate / 10f)
			{
				CreatBuffPotion(PL.Level);
			}
		}
		int num5 = UnityEngine.Random.Range(1, 3);
		for (int l = 0; l < num5; l++)
		{
			if ((float)UnityEngine.Random.Range(0, 101) < 5f + shop.CurrentPremiumDropRate / 10f)
			{
				CreatSpcPotion(PL.Level);
			}
		}
		CreatSpcItem(PL.Level, "Forgetfulness Potion");
		CreatSpcItem(PL.Level, "Reincarnation Potion");
		PoeItemMod.StageShopItems(this);
		LogUtil.Success("商店创建完成");
	}

	public void CreatWeapon(int level, float DropRate)
	{
		DR_EM = DropRate;
		int num = UnityEngine.Random.Range(0, 4);
		if (UnityEngine.Random.Range(0, 4) == 0)
		{
			num = PL.PLType;
		}
		int num2 = UnityEngine.Random.Range(0, 2);
		int num3 = UnityEngine.Random.Range(0, 10);
		int num4 = ((UnityEngine.Random.Range(0, 2) != 0) ? num2 : num3);
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num5 = 0;
		float num6 = ((level < 5) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DropRate) : ((level < 10) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + DropRate) : ((level < 20) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + PL.ItemDrop_Rate_Last + DropRate) : ((level < 30) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + DR_Epic + PL.ItemDrop_Rate_Last + DropRate) : ((level >= 40) ? UnityEngine.Random.Range(0f, DR_Max + PL.ItemDrop_Rate_Last + DropRate) : UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + DR_Epic + DR_Legendary + PL.ItemDrop_Rate_Last + DropRate))))));
		if (num6 < DR_Normal)
		{
			Cur_Q = 0;
		}
		else if (num6 > DR_Normal && num6 < DR_Normal + DR_Magic)
		{
			Cur_Q = 1;
		}
		else if (num6 > DR_Normal + DR_Magic && num6 < DR_Normal + DR_Magic + DR_Rare)
		{
			for (int i = 0; i < Weapon.GP[num].QL[num4].Rare.Count; i++)
			{
				if (Weapon.GP[num].QL[num4].Rare[i].DropLevelStart <= level)
				{
					dictionary.Add(num5, i);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 2;
			}
			else
			{
				Cur_Q = UnityEngine.Random.Range(0, 2);
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare && num6 < DR_Normal + DR_Magic + DR_Rare + DR_Exquisite)
		{
			for (int j = 0; j < Weapon.GP[num].QL[num4].Exquisite.Count; j++)
			{
				if (Weapon.GP[num].QL[num4].Exquisite[j].DropLevelStart <= level)
				{
					dictionary.Add(num5, j);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 3;
			}
			else
			{
				for (int k = 0; k < Weapon.GP[num].QL[num4].Rare.Count; k++)
				{
					if (Weapon.GP[num].QL[num4].Rare[k].DropLevelStart <= level)
					{
						dictionary.Add(num5, k);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 2;
				}
				else
				{
					Cur_Q = UnityEngine.Random.Range(0, 2);
				}
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare + DR_Exquisite && num6 < DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic)
		{
			for (int l = 0; l < Weapon.GP[num].QL[num4].Epic.Count; l++)
			{
				if (Weapon.GP[num].QL[num4].Epic[l].DropLevelStart <= level)
				{
					dictionary.Add(num5, l);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 4;
			}
			else
			{
				for (int m = 0; m < Weapon.GP[num].QL[num4].Exquisite.Count; m++)
				{
					if (Weapon.GP[num].QL[num4].Exquisite[m].DropLevelStart <= level)
					{
						dictionary.Add(num5, m);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 3;
				}
				else
				{
					for (int n = 0; n < Weapon.GP[num].QL[num4].Rare.Count; n++)
					{
						if (Weapon.GP[num].QL[num4].Rare[n].DropLevelStart <= level)
						{
							dictionary.Add(num5, n);
							num5++;
						}
					}
					if (num5 > 0)
					{
						Cur_Q = 2;
					}
					else
					{
						Cur_Q = UnityEngine.Random.Range(0, 2);
					}
				}
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic && num6 < DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic + DR_Legendary)
		{
			for (int num7 = 0; num7 < Weapon.GP[num].QL[num4].Legendary.Count; num7++)
			{
				if (Weapon.GP[num].QL[num4].Legendary[num7].DropLevelStart <= level)
				{
					dictionary.Add(num5, num7);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 5;
			}
			else
			{
				for (int num8 = 0; num8 < Weapon.GP[num].QL[num4].Epic.Count; num8++)
				{
					if (Weapon.GP[num].QL[num4].Epic[num8].DropLevelStart <= level)
					{
						dictionary.Add(num5, num8);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 4;
				}
				else
				{
					for (int num9 = 0; num9 < Weapon.GP[num].QL[num4].Exquisite.Count; num9++)
					{
						if (Weapon.GP[num].QL[num4].Exquisite[num9].DropLevelStart <= level)
						{
							dictionary.Add(num5, num9);
							num5++;
						}
					}
					if (num5 > 0)
					{
						Cur_Q = 3;
					}
					else
					{
						for (int num10 = 0; num10 < Weapon.GP[num].QL[num4].Rare.Count; num10++)
						{
							if (Weapon.GP[num].QL[num4].Rare[num10].DropLevelStart <= level)
							{
								dictionary.Add(num5, num10);
								num5++;
							}
						}
						if (num5 > 0)
						{
							Cur_Q = 2;
						}
						else
						{
							Cur_Q = UnityEngine.Random.Range(0, 2);
						}
					}
				}
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic + DR_Legendary)
		{
			for (int num11 = 0; num11 < Weapon.GP[num].QL[num4].Mythical.Count; num11++)
			{
				if (Weapon.GP[num].QL[num4].Mythical[num11].DropLevelStart <= level)
				{
					dictionary.Add(num5, num11);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 6;
			}
			else
			{
				for (int num12 = 0; num12 < Weapon.GP[num].QL[num4].Legendary.Count; num12++)
				{
					if (Weapon.GP[num].QL[num4].Legendary[num12].DropLevelStart <= level)
					{
						dictionary.Add(num5, num12);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 5;
				}
				else
				{
					for (int num13 = 0; num13 < Weapon.GP[num].QL[num4].Epic.Count; num13++)
					{
						if (Weapon.GP[num].QL[num4].Epic[num13].DropLevelStart <= level)
						{
							dictionary.Add(num5, num13);
							num5++;
						}
					}
					if (num5 > 0)
					{
						Cur_Q = 4;
					}
					else
					{
						for (int num14 = 0; num14 < Weapon.GP[num].QL[num4].Exquisite.Count; num14++)
						{
							if (Weapon.GP[num].QL[num4].Exquisite[num14].DropLevelStart <= level)
							{
								dictionary.Add(num5, num14);
								num5++;
							}
						}
						if (num5 > 0)
						{
							Cur_Q = 3;
						}
						else
						{
							for (int num15 = 0; num15 < Weapon.GP[num].QL[num4].Rare.Count; num15++)
							{
								if (Weapon.GP[num].QL[num4].Rare[num15].DropLevelStart <= level)
								{
									dictionary.Add(num5, num15);
									num5++;
								}
							}
							if (num5 > 0)
							{
								Cur_Q = 2;
							}
							else
							{
								Cur_Q = UnityEngine.Random.Range(0, 2);
							}
						}
					}
				}
			}
		}
		switch (Cur_Q)
		{
		case 0:
		{
			for (int num17 = 0; num17 < Weapon.GP[num].QL[num4].Normal.Count; num17++)
			{
				if (Weapon.GP[num].QL[num4].Normal[num17].DropLevelStart <= level)
				{
					dictionary.Add(num5, num17);
					num5++;
				}
			}
			int key2 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key2, out var value2);
			Item_MB item_MB2 = Weapon.GP[num].QL[num4].Normal[value2];
			SlotData slotData2 = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(item_MB2.SizeX, item_MB2.SizeY));
			slotData2.ItemType = item_MB2.ItemType;
			SetWPdata(slotData2.weapon, item_MB2, level);
			SingletonMonoScope<ShopManager>.Instance.CreatWP(slotData2);
			break;
		}
		case 1:
		{
			for (int num21 = 0; num21 < Weapon.GP[num].QL[num4].Magic.Count; num21++)
			{
				if (Weapon.GP[num].QL[num4].Magic[num21].DropLevelStart <= level)
				{
					dictionary.Add(num5, num21);
					num5++;
				}
			}
			int key6 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key6, out var value6);
			Item_MB item_MB6 = Weapon.GP[num].QL[num4].Magic[value6];
			SlotData slotData6 = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(item_MB6.SizeX, item_MB6.SizeY));
			slotData6.ItemType = item_MB6.ItemType;
			SetWPdata(slotData6.weapon, item_MB6, level);
			SingletonMonoScope<ShopManager>.Instance.CreatWP(slotData6);
			break;
		}
		case 2:
		{
			for (int num18 = 0; num18 < Weapon.GP[num].QL[num4].Rare.Count; num18++)
			{
				if (Weapon.GP[num].QL[num4].Rare[num18].DropLevelStart <= level)
				{
					dictionary.Add(num5, num18);
					num5++;
				}
			}
			int key3 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key3, out var value3);
			Item_MB item_MB3 = Weapon.GP[num].QL[num4].Rare[value3];
			SlotData slotData3 = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(item_MB3.SizeX, item_MB3.SizeY));
			slotData3.ItemType = item_MB3.ItemType;
			SetWPdata(slotData3.weapon, item_MB3, level);
			SingletonMonoScope<ShopManager>.Instance.CreatWP(slotData3);
			break;
		}
		case 3:
		{
			for (int num20 = 0; num20 < Weapon.GP[num].QL[num4].Exquisite.Count; num20++)
			{
				if (Weapon.GP[num].QL[num4].Exquisite[num20].DropLevelStart <= level)
				{
					dictionary.Add(num5, num20);
					num5++;
				}
			}
			int key5 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key5, out var value5);
			Item_MB item_MB5 = Weapon.GP[num].QL[num4].Exquisite[value5];
			SlotData slotData5 = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(item_MB5.SizeX, item_MB5.SizeY));
			slotData5.ItemType = item_MB5.ItemType;
			SetWPdata(slotData5.weapon, item_MB5, level);
			SingletonMonoScope<ShopManager>.Instance.CreatWP(slotData5);
			break;
		}
		case 4:
		{
			for (int num22 = 0; num22 < Weapon.GP[num].QL[num4].Epic.Count; num22++)
			{
				if (Weapon.GP[num].QL[num4].Epic[num22].DropLevelStart <= level)
				{
					dictionary.Add(num5, num22);
					num5++;
				}
			}
			int key7 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key7, out var value7);
			Item_MB item_MB7 = Weapon.GP[num].QL[num4].Epic[value7];
			SlotData slotData7 = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(item_MB7.SizeX, item_MB7.SizeY));
			slotData7.ItemType = item_MB7.ItemType;
			SetWPdata(slotData7.weapon, item_MB7, level);
			SingletonMonoScope<ShopManager>.Instance.CreatWP(slotData7);
			break;
		}
		case 5:
		{
			for (int num19 = 0; num19 < Weapon.GP[num].QL[num4].Legendary.Count; num19++)
			{
				if (Weapon.GP[num].QL[num4].Legendary[num19].DropLevelStart <= level)
				{
					dictionary.Add(num5, num19);
					num5++;
				}
			}
			int key4 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key4, out var value4);
			Item_MB item_MB4 = Weapon.GP[num].QL[num4].Legendary[value4];
			SlotData slotData4 = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(item_MB4.SizeX, item_MB4.SizeY));
			slotData4.ItemType = item_MB4.ItemType;
			SetWPdata(slotData4.weapon, item_MB4, level);
			SingletonMonoScope<ShopManager>.Instance.CreatWP(slotData4);
			break;
		}
		case 6:
		{
			for (int num16 = 0; num16 < Weapon.GP[num].QL[num4].Mythical.Count; num16++)
			{
				if (Weapon.GP[num].QL[num4].Mythical[num16].DropLevelStart <= level)
				{
					dictionary.Add(num5, num16);
					num5++;
				}
			}
			int key = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key, out var value);
			Item_MB item_MB = Weapon.GP[num].QL[num4].Mythical[value];
			SlotData slotData = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(item_MB.SizeX, item_MB.SizeY));
			slotData.ItemType = item_MB.ItemType;
			SetWPdata(slotData.weapon, item_MB, level);
			SingletonMonoScope<ShopManager>.Instance.CreatWP(slotData);
			break;
		}
		}
	}

	public void CreatPotion(int level)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 0;
		if (level < 10)
		{
			for (int i = 0; i < Potion.Count; i++)
			{
				if (Potion[i].Level < 10)
				{
					dictionary.Add(num, i);
					num++;
				}
			}
		}
		else if (level < 20)
		{
			for (int j = 0; j < Potion.Count; j++)
			{
				if (Potion[j].Level >= 10 && Potion[j].Level < 20)
				{
					dictionary.Add(num, j);
					num++;
				}
			}
		}
		else if (level < 30)
		{
			for (int k = 0; k < Potion.Count; k++)
			{
				if (Potion[k].Level >= 20 && Potion[k].Level < 30)
				{
					dictionary.Add(num, k);
					num++;
				}
			}
		}
		else if (level < 40)
		{
			for (int l = 0; l < Potion.Count; l++)
			{
				if (Potion[l].Level >= 30 && Potion[l].Level < 40)
				{
					dictionary.Add(num, l);
					num++;
				}
			}
		}
		else if (level < 50)
		{
			for (int m = 0; m < Potion.Count; m++)
			{
				if (Potion[m].Level >= 40 && Potion[m].Level < 50)
				{
					dictionary.Add(num, m);
					num++;
				}
			}
		}
		else if (level < 60)
		{
			for (int n = 0; n < Potion.Count; n++)
			{
				if (Potion[n].Level >= 50 && Potion[n].Level < 60)
				{
					dictionary.Add(num, n);
					num++;
				}
			}
		}
		else if (level < 70)
		{
			for (int num2 = 0; num2 < Potion.Count; num2++)
			{
				if (Potion[num2].Level >= 60 && Potion[num2].Level < 70)
				{
					dictionary.Add(num, num2);
					num++;
				}
			}
		}
		else if (level < 80)
		{
			for (int num3 = 0; num3 < Potion.Count; num3++)
			{
				if (Potion[num3].Level >= 70 && Potion[num3].Level < 80)
				{
					dictionary.Add(num, num3);
					num++;
				}
			}
		}
		else if (level < 90)
		{
			for (int num4 = 0; num4 < Potion.Count; num4++)
			{
				if (Potion[num4].Level >= 80 && Potion[num4].Level < 90)
				{
					dictionary.Add(num, num4);
					num++;
				}
			}
		}
		else if (level < 100)
		{
			for (int num5 = 0; num5 < Potion.Count; num5++)
			{
				if (Potion[num5].Level >= 90 && Potion[num5].Level < 100)
				{
					dictionary.Add(num, num5);
					num++;
				}
			}
		}
		else
		{
			for (int num6 = 0; num6 < Potion.Count; num6++)
			{
				if (Potion[num6].Level >= 90)
				{
					dictionary.Add(num, num6);
					num++;
				}
			}
		}
		int key = UnityEngine.Random.Range(0, num);
		dictionary.TryGetValue(key, out var value);
		SlotData slotData = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(1, 1));
		SetPotiondataShop(slotData, value);
		SingletonMonoScope<ShopManager>.Instance.CreatUSE(slotData);
	}

	public void CreatBuffPotion(int level)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 0;
		for (int i = 0; i < BuffPotion.Count; i++)
		{
			if (BuffPotion[i].Level <= level)
			{
				dictionary.Add(num, i);
				num++;
			}
		}
		if (num > 0)
		{
			int key = UnityEngine.Random.Range(0, num);
			dictionary.TryGetValue(key, out var value);
			SlotData slotData = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(1, 1));
			SetBuffPotiondataShop(slotData, value);
			SingletonMonoScope<ShopManager>.Instance.CreatUSE(slotData);
		}
	}

	public void CreatSpcPotion(int level)
	{
		if (TryGetCurrentPremPotionIndex(level, out var index))
		{
			SlotData slotData = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(1, 1));
			SetPremPotiondataShop(slotData, index);
			SingletonMonoScope<ShopManager>.Instance.CreatUSE(slotData);
		}
	}

	public void CreatPremPotion(int level)
	{
		if (TryGetCurrentPremPotionIndex(level, out var index))
		{
			SlotData slotData = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(1, 1));
			SetPremPotiondataShop(slotData, index);
			SingletonMonoScope<ShopManager>.Instance.CreatUSE(slotData);
		}
	}

	public void CreatSpcItem(int level, string potionName)
	{
		SpcPotion.TryGetValue(potionName, out var value);
		SlotData slotData = SingletonMonoScope<ShopManager>.Instance.CheckEmptyBuy(new IntVector2(1, 1));
		SetSPCShop(slotData, value);
		SingletonMonoScope<ShopManager>.Instance.CreatUSE(slotData);
	}

	public void SetPotiondataShop(SlotData drop, int a)
	{
		drop.ItemType = 2;
		drop.useitem.GlobalID = Potion[a].GlobalID;
		drop.useitem.ItemType = Potion[a].ItemType;
		drop.useitem.ItemName = Potion[a].ItemName;
		drop.useitem.Price = Potion[a].Price;
		drop.useitem.Quality = Potion[a].Quality;
		drop.useitem.Size = Potion[a].Size;
		drop.useitem.Icon = Potion[a].Icon;
		drop.useitem.Level = Potion[a].Level;
		drop.useitem.SoundDrop = Potion[a].SoundDrop;
		drop.useitem.SoundUse = Potion[a].SoundUse;
		drop.useitem.RotateType = Potion[a].RotateType;
		drop.useitem.InfoType = Potion[a].InfoType;
		drop.useitem.UseType = Potion[a].UseType;
		drop.useitem.damageType = Potion[a].damageType;
		drop.useitem.Number = Potion[a].Number;
		drop.useitem.CDTime = Potion[a].CDTime;
		drop.useitem.Duration = Potion[a].Duration;
		drop.useitem.MstackSize = Potion[a].MstackSize;
		int cstackSize = UnityEngine.Random.Range(3, 7);
		drop.useitem.CstackSize = cstackSize;
		drop.useitem.DropSpriteSize = Potion[a].DropSpriteSize;
	}

	public void SetBuffPotiondataShop(SlotData drop, int a)
	{
		drop.ItemType = 2;
		drop.useitem.GlobalID = BuffPotion[a].GlobalID;
		drop.useitem.ItemType = BuffPotion[a].ItemType;
		drop.useitem.ItemName = BuffPotion[a].ItemName;
		drop.useitem.Price = BuffPotion[a].Price;
		drop.useitem.Quality = BuffPotion[a].Quality;
		drop.useitem.Size = BuffPotion[a].Size;
		drop.useitem.Icon = BuffPotion[a].Icon;
		drop.useitem.Level = BuffPotion[a].Level;
		drop.useitem.SoundDrop = BuffPotion[a].SoundDrop;
		drop.useitem.SoundUse = BuffPotion[a].SoundUse;
		drop.useitem.RotateType = BuffPotion[a].RotateType;
		drop.useitem.InfoType = BuffPotion[a].InfoType;
		drop.useitem.UseType = BuffPotion[a].UseType;
		drop.useitem.damageType = BuffPotion[a].damageType;
		drop.useitem.Number = BuffPotion[a].Number;
		drop.useitem.CDTime = BuffPotion[a].CDTime;
		drop.useitem.Duration = BuffPotion[a].Duration;
		drop.useitem.MstackSize = BuffPotion[a].MstackSize;
		drop.useitem.CstackSize = BuffPotion[a].CstackSize;
		drop.useitem.DropSpriteSize = BuffPotion[a].DropSpriteSize;
	}

	public void SetSPCdataShop(SlotData drop, UseItemClass it)
	{
		drop.ItemType = 2;
		drop.useitem.GlobalID = it.GlobalID;
		drop.useitem.ItemType = it.ItemType;
		drop.useitem.ItemName = it.ItemName;
		drop.useitem.Price = it.Price;
		drop.useitem.Quality = it.Quality;
		drop.useitem.Size = it.Size;
		drop.useitem.Icon = it.Icon;
		drop.useitem.Level = it.Level;
		drop.useitem.SoundDrop = it.SoundDrop;
		drop.useitem.SoundUse = it.SoundUse;
		drop.useitem.RotateType = it.RotateType;
		drop.useitem.InfoType = it.InfoType;
		drop.useitem.UseType = it.UseType;
		drop.useitem.damageType = it.damageType;
		drop.useitem.Number = it.Number;
		drop.useitem.CDTime = it.CDTime;
		drop.useitem.Duration = it.Duration;
		drop.useitem.MstackSize = it.MstackSize;
		int cstackSize = UnityEngine.Random.Range(2, 6);
		drop.useitem.CstackSize = cstackSize;
		drop.useitem.DropSpriteSize = it.DropSpriteSize;
	}

	public void SetPremPotiondataShop(SlotData drop, int a)
	{
		drop.ItemType = 2;
		drop.useitem.GlobalID = PremPotion[a].GlobalID;
		drop.useitem.ItemType = PremPotion[a].ItemType;
		drop.useitem.ItemName = PremPotion[a].ItemName;
		drop.useitem.Price = PremPotion[a].Price;
		drop.useitem.Quality = PremPotion[a].Quality;
		drop.useitem.Size = PremPotion[a].Size;
		drop.useitem.Icon = PremPotion[a].Icon;
		drop.useitem.Level = PremPotion[a].Level;
		drop.useitem.SoundDrop = PremPotion[a].SoundDrop;
		drop.useitem.SoundUse = PremPotion[a].SoundUse;
		drop.useitem.RotateType = PremPotion[a].RotateType;
		drop.useitem.InfoType = PremPotion[a].InfoType;
		drop.useitem.UseType = PremPotion[a].UseType;
		drop.useitem.damageType = PremPotion[a].damageType;
		drop.useitem.Number = PremPotion[a].Number;
		drop.useitem.CDTime = PremPotion[a].CDTime;
		drop.useitem.Duration = PremPotion[a].Duration;
		drop.useitem.MstackSize = PremPotion[a].MstackSize;
		drop.useitem.CstackSize = PremPotion[a].CstackSize;
		drop.useitem.DropSpriteSize = PremPotion[a].DropSpriteSize;
	}

	public void SetSPCShop(SlotData drop, UseItemClass it)
	{
		drop.ItemType = 2;
		drop.useitem.GlobalID = it.GlobalID;
		drop.useitem.ItemType = it.ItemType;
		drop.useitem.ItemName = it.ItemName;
		drop.useitem.Price = it.Price;
		drop.useitem.Quality = it.Quality;
		drop.useitem.Size = it.Size;
		drop.useitem.Icon = it.Icon;
		drop.useitem.Level = it.Level;
		drop.useitem.SoundDrop = it.SoundDrop;
		drop.useitem.SoundUse = it.SoundUse;
		drop.useitem.RotateType = it.RotateType;
		drop.useitem.InfoType = it.InfoType;
		drop.useitem.UseType = it.UseType;
		drop.useitem.damageType = it.damageType;
		drop.useitem.Number = it.Number;
		drop.useitem.CDTime = it.CDTime;
		drop.useitem.Duration = it.Duration;
		drop.useitem.MstackSize = it.MstackSize;
		drop.useitem.CstackSize = it.CstackSize;
		drop.useitem.DropSpriteSize = it.DropSpriteSize;
	}

	private bool TryGetCurrentPremPotionIndex(int level, out int index)
	{
		List<int> currentPremPotionIndexes = GetCurrentPremPotionIndexes(level);
		if (currentPremPotionIndexes.Count <= 0)
		{
			index = -1;
			return false;
		}
		index = currentPremPotionIndexes[UnityEngine.Random.Range(0, currentPremPotionIndexes.Count)];
		return true;
	}

	private List<int> GetCurrentPremPotionIndexes(int level)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int i = 0; i < PremPotion.Count; i++)
		{
			UseItemClass useItemClass = PremPotion[i];
			if (useItemClass.Level > level)
			{
				continue;
			}
			string premPotionTierKey = GetPremPotionTierKey(useItemClass.UseType);
			if (dictionary.TryGetValue(premPotionTierKey, out var value))
			{
				UseItemClass useItemClass2 = PremPotion[value];
				if (useItemClass.Level > useItemClass2.Level)
				{
					dictionary[premPotionTierKey] = i;
				}
			}
			else
			{
				dictionary.Add(premPotionTierKey, i);
			}
		}
		return dictionary.Values.ToList();
	}

	private static string GetPremPotionTierKey(string useType)
	{
		if (string.IsNullOrEmpty(useType))
		{
			return string.Empty;
		}
		int num = useType.Length;
		while (num > 0 && char.IsDigit(useType[num - 1]))
		{
			num--;
		}
		if (num != useType.Length)
		{
			return useType.Substring(0, num);
		}
		return useType;
	}

	private static WeaponDropContext GetCurrentWeaponDropContext()
	{
		if (!LevelManager.GetIsMijing())
		{
			return NormalizeWeaponDropContext(0, 0);
		}
		int num = LevelManager.SceneQulity;
		if (num <= 0 && SingletonMonoScope<MijingManager>.HasInstance)
		{
			num = SingletonMonoScope<MijingManager>.Instance.GetCurrentSceneQulity();
		}
		if (num <= 0)
		{
			num = 1;
		}
		int mjLevel = ((!SingletonMonoScope<MijingManager>.HasInstance) ? 1 : SingletonMonoScope<MijingManager>.Instance.GetCurrentFloor());
		return NormalizeWeaponDropContext(num, mjLevel);
	}

	private static WeaponDropContext GetWeaponDropContext(WeaponClass weapon)
	{
		if (weapon == null)
		{
			return NormalizeWeaponDropContext(0, 0);
		}
		return NormalizeWeaponDropContext(weapon.DropScene, weapon.MJ_Level);
	}

	private static WeaponDropContext NormalizeWeaponDropContext(int dropScene, int mjLevel)
	{
		dropScene = Mathf.Clamp(dropScene, 0, 4);
		mjLevel = ((dropScene > 0) ? Mathf.Max(1, mjLevel) : 0);
		WeaponDropContext result = default(WeaponDropContext);
		result.DropScene = dropScene;
		result.MJ_Level = mjLevel;
		return result;
	}

	private static MijingSettings GetMijingSettings()
	{
		if (SingletonMonoScope<MijingManager>.HasInstance && (bool)SingletonMonoScope<MijingManager>.Instance.mijingSettings)
		{
			return SingletonMonoScope<MijingManager>.Instance.mijingSettings;
		}
		GlobalSettings instance = SettingsLoader.Instance;
		if (!instance)
		{
			return null;
		}
		return instance.mijingSettings;
	}

	private static MijingDifficultyFormulaConfig GetMijingDifficultyConfig(WeaponDropContext dropContext)
	{
		MijingSettings mijingSettings = GetMijingSettings();
		if (!mijingSettings)
		{
			return default(MijingDifficultyFormulaConfig);
		}
		return mijingSettings.GetDifficultyConfig(GetMijingDifficulty(dropContext.DropScene));
	}

	private static DifficultType GetMijingDifficulty(int dropScene)
	{
		return Mathf.Clamp(dropScene, 1, 4) switch
		{
			1 => DifficultType.Easy, 
			2 => DifficultType.Medium, 
			3 => DifficultType.Hard, 
			4 => DifficultType.Master, 
			_ => DifficultType.Easy, 
		};
	}

	private static float GetMijingWPDamageMultiplier(WeaponDropContext dropContext)
	{
		return GetMijingDifficultyConfig(dropContext).WP_DMG.EvaluateFromFirstFloorWithFallback(dropContext.MJ_Level, 1f, 1f);
	}

	private static float GetMijingWPPRCMultiplier(WeaponDropContext dropContext)
	{
		return GetMijingDifficultyConfig(dropContext).WP_PRC.EvaluateFromFirstFloorWithFallback(dropContext.MJ_Level, 1f, 1f);
	}

	private static float GetMijingSPCDamageMultiplier(WeaponDropContext dropContext)
	{
		return GetMijingDifficultyConfig(dropContext).SPC_DMG.EvaluateFromFirstFloorWithFallback(dropContext.MJ_Level, 1f, 1f);
	}

	public void SetWPdata(WeaponClass it, Item_MB mb, int level)
	{
		SetWPdata(it, mb, level, GetCurrentWeaponDropContext());
	}

	private void SetWPdata(WeaponClass it, Item_MB mb, int level, WeaponDropContext dropContext)
	{
		if (it == null || mb == null)
		{
			return;
		}
		dropContext = NormalizeWeaponDropContext(dropContext.DropScene, dropContext.MJ_Level);
		it.ItemName = mb.ItemName;
		it.GlobalID = mb.GlobalID;
		it.ItemType = mb.ItemType;
		it.Quality = mb.Quality;
		it.Size.x = mb.SizeX;
		it.Size.y = mb.SizeY;
		it.Icon = IconData[mb.IconType].icon[mb.Icon];
		it.Level = level;
		it.SoundDrop = mb.SoundDrop;
		it.SoundUse = mb.SoundUse;
		it.RotateType = mb.RotateType;
		it.PLtype = mb.PLtype;
		it.WeaponType = mb.WeaponType;
		it.CharType = mb.CharType;
		it.Reb_CountMax = 0;
		it.ZQ_CountMax = 0;
		it.HHCount = 0;
		it.SKCount = 0;
		it.JHEL_Count = 0;
		it.JH_Count = 0;
		it.Craft_LockPrefix = false;
		it.Craft_LockSuffix = false;
		it.Craft_NoAttack = false;
		it.Craft_NoCaster = false;
		it.SPC_DMG_Bei = 100f;
		it.BaseValueDoubled = false;
		it.BaseValueMultiplier = 1f;
		it.ResetSkillFWCountMax();
		it.FW_Base = null;
		if (it.SPC == null)
		{
			it.SPC = new List<WPSPC>();
		}
		else
		{
			it.SPC.Clear();
		}
		it.SPC.Add(new WPSPC());
		if (it.WPSK == null)
		{
			it.WPSK = new List<WPSkill>();
		}
		while (it.WPSK.Count < 6)
		{
			it.WPSK.Add(new WPSkill());
		}
		for (int i = 0; i < it.WPSK.Count; i++)
		{
			it.WPSK[i].IndexName = "0";
			it.WPSK[i].Number = 0;
			it.WPSK[i].Number2 = 0;
			it.WPSK[i].price = 0;
		}
		if (it.Aocao == null)
		{
			it.Aocao = new List<WPAocao>();
		}
		while (it.Aocao.Count < 6)
		{
			it.Aocao.Add(new WPAocao());
		}
		float num = GivePRC_Base(level, dropContext);
		it.Damage = Mathf.Floor(mb.Damage * Mathf.Pow(MultiLevelA, level) * (1f + UnityEngine.Random.Range(0f - RandomCount, RandomCount)) * num);
		it.Health = Mathf.Floor(mb.Health * Mathf.Pow(MultiLevelA, level) * (1f + UnityEngine.Random.Range(0f - RandomCount, RandomCount)) * num);
		it.Mana = Mathf.Floor(mb.Mana * Mathf.Pow(MultiLevelA, level) * (1f + UnityEngine.Random.Range(0f - RandomCount, RandomCount)) * num);
		ApplyElement(it, mb.Element, level, dropContext);
		it.Main = BuildWeaponRuntimeDataA(mb.Main, mb.RateMain, resolveRuntimeElement: true, level, it.Quality, dropContext, WeaponStatGroup.Main, scaleMainRecoveryValues: true);
		it.DOT = BuildWeaponRuntimeDataA(mb.DOT, mb.RateDot, resolveRuntimeElement: true, level, it.Quality, dropContext, WeaponStatGroup.Dot);
		int generatedWeaponSkillCount = GetGeneratedWeaponSkillCount(it.Quality, dropContext);
		BuildWeaponSkillRuntimeData(mb.SK, mb.CP, mb.RateSK, generatedWeaponSkillCount, resolveRuntimeElement: true, level, it.Quality, dropContext, out it.SK, out it.CP);
		ApplyQualityAttributeRemoval(it);
		it.Set_Index = mb.Set_Index;
		it.SetRuntimeData = CloneSetData(it.Set_Index);
		ApplySetAttributeCollisions(it, mb, level, dropContext, generatedWeaponSkillCount);
		it.BS_Set_Index = 0;
		it.DropScene = dropContext.DropScene;
		it.MJ_Level = dropContext.MJ_Level;
		it.WP_SkillCount = mb.WP_SkillCount;
		it.WPSK[0].IndexName = mb.SkillA;
		it.WPSK[0].Number = mb.SkillA_count + WPSK_multi(it.Quality);
		it.WPSK[1].IndexName = mb.SkillB;
		it.WPSK[1].Number = mb.SkillB_count + WPSK_multi(it.Quality);
		it.WPSK[2].IndexName = mb.SkillC;
		it.WPSK[2].Number = mb.SkillC_count + WPSK_multi(it.Quality);
		it.WPSK[3].IndexName = mb.SkillD;
		it.WPSK[3].Number = mb.SkillD_count + WPSK_multi(it.Quality);
		it.WPSK[4].IndexName = mb.SkillE;
		it.WPSK[4].Number = mb.SkillE_count + WPSK_multi(it.Quality);
		it.WPSK[5].IndexName = mb.SkillF;
		it.WPSK[5].Number = mb.SkillF_count + WPSK_multi(it.Quality);
		it.MaxAocaoCount = mb.MaxAocaoCount;
		it.AocaoCount = UnityEngine.Random.Range(0, mb.CurAocaoCount + 1);
		for (int j = 0; j < 6; j++)
		{
			if (j < it.AocaoCount)
			{
				it.Aocao[j].HasAocao = true;
			}
			else
			{
				it.Aocao[j].HasAocao = false;
			}
			it.Aocao[j].HasBaoshi = false;
		}
		WPSPC randomWeaponSPC = GetRandomWeaponSPC(mb.SPC);
		if (randomWeaponSPC != null)
		{
			it.SetSPCData(0, randomWeaponSPC.Index, UnityEngine.Random.Range(0, 6), GivePRC_SPC(it.Level, it.Quality, dropContext));
		}
		else
		{
			it.SetSPCData(0, 0, 0, 0f);
		}
		WPSPC sPCData = it.GetSPCData(0);
		SPC.TryGetValue(sPCData.Index, out var value);
		it.Price = 0;
		if (value != null && value.SPCtype != 0)
		{
			float sPCPRC = it.GetSPCPRC(sPCData);
			if (it.CharType > 5)
			{
				if (UnityEngine.Random.Range(0, 101) < 70)
				{
					it.Price += GetWeaponSPCPrice(value, sPCPRC);
				}
				else
				{
					sPCData.Index = 0;
					if (it.Quality < 4)
					{
						it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.1f, 1.3f));
						it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.1f, 1.3f));
						it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.1f, 1.3f));
					}
					else if (it.Quality < 5)
					{
						it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.1f, 1.4f));
						it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.1f, 1.4f));
						it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.1f, 1.4f));
					}
					else
					{
						it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.2f, 1.4f));
						it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.2f, 1.4f));
						it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.2f, 1.4f));
					}
				}
			}
			else if (it.CharType > 1 && it.CharType < 6)
			{
				if (UnityEngine.Random.Range(0, 101) < 70)
				{
					it.Price += GetWeaponSPCPrice(value, sPCPRC);
				}
				else
				{
					sPCData.Index = 0;
					if (it.Quality < 4)
					{
						it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.1f, 1.3f));
						it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.1f, 1.3f));
						it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.1f, 1.3f));
					}
					else if (it.Quality < 5)
					{
						it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.1f, 1.4f));
						it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.1f, 1.4f));
						it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.1f, 1.4f));
					}
					else
					{
						it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.2f, 1.4f));
						it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.2f, 1.4f));
						it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.2f, 1.4f));
					}
				}
			}
			else if (UnityEngine.Random.Range(0, 101) < 80)
			{
				it.Price += GetWeaponSPCPrice(value, sPCPRC);
			}
			else
			{
				sPCData.Index = 0;
				if (it.Quality < 4)
				{
					it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.1f, 1.4f));
					it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.1f, 1.4f));
					it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.1f, 1.4f));
				}
				else if (it.Quality < 5)
				{
					it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.1f, 1.5f));
					it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.1f, 1.5f));
					it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.1f, 1.5f));
				}
				else
				{
					it.Damage = Mathf.Floor(it.Damage * UnityEngine.Random.Range(1.2f, 1.5f));
					it.Health = Mathf.Floor(it.Health * UnityEngine.Random.Range(1.2f, 1.5f));
					it.Mana = Mathf.Floor(it.Mana * UnityEngine.Random.Range(1.2f, 1.5f));
				}
			}
		}
		it.Price += Mathf.FloorToInt((it.Damage + it.Health + it.Mana) * 1f);
		float num2 = 0f;
		num2 += (it.Fire + it.Frozen + it.Thunder + it.Poison + it.Physics + it.Shadow) * 20f;
		num2 += (float)GetWeaponMainArrayPrice(it.Main);
		num2 += (float)GetWeaponDotArrayPrice(it.DOT);
		num2 += (float)GetWeaponSKArrayPrice(it.SK);
		num2 += (float)GetWeaponCPArrayPrice(it.CP);
		if (level > 30 && level <= 40)
		{
			num2 *= 1.1f;
		}
		if (level > 40 && level <= 50)
		{
			num2 *= 1.2f;
		}
		else if (level > 50 && level <= 60)
		{
			num2 *= 1.33f;
		}
		num2 = ((level > 60 && level <= 70) ? (num2 * 1.45f) : ((level > 70 && level <= 80) ? (num2 * 1.6f) : ((level <= 80 || level > 90) ? (num2 * 2f) : (num2 * 1.8f))));
		it.Price += Mathf.FloorToInt(num2);
		if (mb.Set_Index != 0)
		{
			it.Price += Mathf.FloorToInt((float)it.Price * 0.3f);
		}
		if (it.WP_SkillCount > 0)
		{
			for (int k = 0; k < it.WP_SkillCount; k++)
			{
				if (!(it.WPSK[k].IndexName != "0"))
				{
					continue;
				}
				int num3 = it.WPSK[k].Number + it.WPSK[k].Number2;
				if (num3 > 0)
				{
					TL.SKI.TryGetValue(it.WPSK[k].IndexName, out var value2);
					switch (value2.type)
					{
					case 0:
					{
						TL.XiData[value2.Xi].Sample_F.TryGetValue(it.WPSK[k].IndexName, out var value9);
						it.Price += value9.Price * num3;
						break;
					}
					case 1:
					{
						TL.XiData[value2.Xi].Sample_S.TryGetValue(it.WPSK[k].IndexName, out var value8);
						it.Price += value8.Price * num3;
						break;
					}
					case 2:
					{
						TL.XiData[value2.Xi].Comp_F.TryGetValue(it.WPSK[k].IndexName, out var value7);
						it.Price += value7.Price * num3;
						break;
					}
					case 3:
					{
						TL.XiData[value2.Xi].Comp_S.TryGetValue(it.WPSK[k].IndexName, out var value6);
						it.Price += value6.Price * num3;
						break;
					}
					case 4:
					{
						TL.XiData[value2.Xi].Dot_F.TryGetValue(it.WPSK[k].IndexName, out var value5);
						it.Price += value5.Price * num3;
						break;
					}
					case 5:
					{
						TL.XiData[value2.Xi].Dot_S.TryGetValue(it.WPSK[k].IndexName, out var value4);
						it.Price += value4.Price * num3;
						break;
					}
					case 6:
					{
						TL.XiData[value2.Xi].Bei.TryGetValue(it.WPSK[k].IndexName, out var value3);
						it.Price += value3.Price * num3;
						break;
					}
					}
				}
			}
		}
		if (it.AocaoCount > 0)
		{
			it.Price += AocaoPrice.Price[it.AocaoCount];
		}
	}

	private static void ApplyQualityAttributeRemoval(WeaponClass item)
	{
		float chance = 0f;
		float chance2 = 0f;
		switch (item.Quality)
		{
		case 1:
			chance = 0.3f;
			break;
		case 2:
			chance = 0.2f;
			chance2 = 0.3f;
			break;
		case 3:
		case 4:
			chance = 0.2f;
			chance2 = 0.2f;
			break;
		case 5:
			chance = 0.1f;
			chance2 = 0.1f;
			break;
		case 6:
			chance2 = 0f;
			break;
		}
		int num = 0;
		if (TryRemoveLastStat(ref item.Main, chance))
		{
			num++;
		}
		if (TryRemoveLastStat(ref item.DOT, chance2))
		{
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			item.Damage = Mathf.Floor(item.Damage * UnityEngine.Random.Range(1.1f, 1.2f));
			item.Health = Mathf.Floor(item.Health * UnityEngine.Random.Range(1.1f, 1.2f));
			item.Mana = Mathf.Floor(item.Mana * UnityEngine.Random.Range(1.1f, 1.2f));
		}
	}

	private static bool TryRemoveLastStat<T>(ref T[] stats, float chance) where T : class
	{
		if (stats == null || stats.Length == 0 || chance <= 0f || UnityEngine.Random.value >= chance)
		{
			return false;
		}
		T[] array = new T[stats.Length - 1];
		if (array.Length != 0)
		{
			Array.Copy(stats, array, array.Length);
		}
		stats = array;
		return true;
	}

	private static bool TryRemoveRandomStat<T>(ref T[] stats, float chance) where T : class
	{
		if (stats == null || stats.Length == 0 || chance <= 0f || UnityEngine.Random.value >= chance)
		{
			return false;
		}
		int num = UnityEngine.Random.Range(0, stats.Length);
		T[] array = new T[stats.Length - 1];
		if (num > 0)
		{
			Array.Copy(stats, 0, array, 0, num);
		}
		int num2 = stats.Length - num - 1;
		if (num2 > 0)
		{
			Array.Copy(stats, num + 1, array, num, num2);
		}
		stats = array;
		return true;
	}

	private Set_DT CloneSetData(int setIndex)
	{
		if (setIndex <= 0 || !SET.TryGetValue(setIndex, out var value))
		{
			return null;
		}
		return SetDataUtil.Clone(value);
	}

	private void ApplySetAttributeCollisions(WeaponClass item, Item_MB mb, int level, WeaponDropContext dropContext, int generatedSkillCount)
	{
		if (item == null || mb == null || item.SetRuntimeData == null || item.SetRuntimeData.Lit == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		for (int i = 0; i < item.SetRuntimeData.Lit.Length; i++)
		{
			Set_DT_Lit set_DT_Lit = item.SetRuntimeData.Lit[i];
			if (set_DT_Lit != null && set_DT_Lit.Index > 0)
			{
				switch (set_DT_Lit.MainTP)
				{
				case 0:
					flag |= WeaponClass.IsMainBoolIndex(set_DT_Lit.Index) && ContainsWeaponDataA(item.Main, set_DT_Lit.Index);
					break;
				case 1:
					flag2 |= WeaponClass.IsDotBoolIndex(set_DT_Lit.Index) && ContainsWeaponDataA(item.DOT, set_DT_Lit.Index);
					break;
				case 2:
					flag3 |= WeaponClass.IsSKBoolIndex(set_DT_Lit.Index) && ContainsWeaponDataB(item.SK, set_DT_Lit.Index);
					break;
				case 3:
					flag4 |= WeaponClass.IsCPBoolIndex(set_DT_Lit.Index) && ContainsWeaponDataB(item.CP, set_DT_Lit.Index);
					break;
				}
			}
		}
		if (flag)
		{
			item.Main = BuildWeaponRuntimeDataA(mb.Main, mb.RateMain, resolveRuntimeElement: true, level, item.Quality, dropContext, WeaponStatGroup.Main, scaleMainRecoveryValues: true);
		}
		if (flag2)
		{
			item.DOT = BuildWeaponRuntimeDataA(mb.DOT, mb.RateDot, resolveRuntimeElement: true, level, item.Quality, dropContext, WeaponStatGroup.Dot);
		}
		if (flag3 || flag4)
		{
			BuildWeaponSkillRuntimeData(mb.SK, mb.CP, mb.RateSK, generatedSkillCount, resolveRuntimeElement: true, level, item.Quality, dropContext, out item.SK, out item.CP);
		}
		for (int j = 0; j < item.SetRuntimeData.Lit.Length; j++)
		{
			Set_DT_Lit set_DT_Lit2 = item.SetRuntimeData.Lit[j];
			if (set_DT_Lit2 == null || set_DT_Lit2.Index <= 0)
			{
				continue;
			}
			switch (set_DT_Lit2.MainTP)
			{
			case 0:
				if (!WeaponClass.IsMainBoolIndex(set_DT_Lit2.Index))
				{
					MergeWeaponDataAIntoSet(ref item.Main, set_DT_Lit2);
				}
				break;
			case 1:
				if (!WeaponClass.IsDotBoolIndex(set_DT_Lit2.Index))
				{
					MergeWeaponDataAIntoSet(ref item.DOT, set_DT_Lit2);
				}
				break;
			case 2:
				if (!WeaponClass.IsSKBoolIndex(set_DT_Lit2.Index))
				{
					MergeWeaponDataBIntoSet(ref item.SK, set_DT_Lit2);
				}
				break;
			case 3:
				if (!WeaponClass.IsCPBoolIndex(set_DT_Lit2.Index))
				{
					MergeWeaponDataBIntoSet(ref item.CP, set_DT_Lit2);
				}
				break;
			}
		}
	}

	private static bool ContainsWeaponDataA(WPDT_A[] data, int index)
	{
		if (data == null)
		{
			return false;
		}
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] != null && data[i].Index == index)
			{
				return true;
			}
		}
		return false;
	}

	private static bool ContainsWeaponDataB(WPDT_B[] data, int index)
	{
		if (data == null)
		{
			return false;
		}
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] != null && data[i].Index == index)
			{
				return true;
			}
		}
		return false;
	}

	private static void MergeWeaponDataAIntoSet(ref WPDT_A[] data, Set_DT_Lit lit)
	{
		if (data == null || lit == null)
		{
			return;
		}
		List<WPDT_A> list = new List<WPDT_A>();
		for (int i = 0; i < data.Length; i++)
		{
			WPDT_A wPDT_A = data[i];
			if (wPDT_A != null && wPDT_A.Index == lit.Index)
			{
				lit.Number += Mathf.RoundToInt(wPDT_A.number);
			}
			else if (wPDT_A != null)
			{
				list.Add(wPDT_A);
			}
		}
		data = list.ToArray();
	}

	private static void MergeWeaponDataBIntoSet(ref WPDT_B[] data, Set_DT_Lit lit)
	{
		if (data == null || lit == null)
		{
			return;
		}
		List<WPDT_B> list = new List<WPDT_B>();
		for (int i = 0; i < data.Length; i++)
		{
			WPDT_B wPDT_B = data[i];
			if (wPDT_B != null && wPDT_B.Index == lit.Index)
			{
				lit.Number += Mathf.RoundToInt(wPDT_B.number);
			}
			else if (wPDT_B != null)
			{
				list.Add(wPDT_B);
			}
		}
		data = list.ToArray();
	}

	private static int GetWeaponMainArrayPrice(WPDT_A[] stats)
	{
		int num = 0;
		if (stats == null)
		{
			return num;
		}
		for (int i = 0; i < stats.Length; i++)
		{
			num += GetWeaponMainStatPrice(stats[i]);
		}
		return num;
	}

	private static int GetWeaponDotArrayPrice(WPDT_A[] stats)
	{
		int num = 0;
		if (stats == null)
		{
			return num;
		}
		for (int i = 0; i < stats.Length; i++)
		{
			num += GetWeaponDotStatPrice(stats[i]);
		}
		return num;
	}

	private int GetWeaponSKArrayPrice(WPDT_B[] stats)
	{
		int num = 0;
		if (stats == null)
		{
			return num;
		}
		foreach (WPDT_B wPDT_B in stats)
		{
			if (wPDT_B != null && wPDT_B.Index != 0 && !IsCsvNoneText(wPDT_B.SkillName))
			{
				num += GetWeaponSKStatPrice(wPDT_B);
			}
		}
		return num;
	}

	private int GetWeaponCPArrayPrice(WPDT_B[] stats)
	{
		int num = 0;
		if (stats == null)
		{
			return num;
		}
		foreach (WPDT_B wPDT_B in stats)
		{
			if (wPDT_B != null && wPDT_B.Index != 0 && !IsCsvNoneText(wPDT_B.SkillName))
			{
				num += GetWeaponCPStatPrice(wPDT_B);
			}
		}
		return num;
	}

	private static int GetWeaponMainStatPrice(WPDT_A stat)
	{
		if (stat == null)
		{
			return 0;
		}
		return stat.Index switch
		{
			0 => 0, 
			1 => Mathf.FloorToInt(stat.number * 30f), 
			2 => Mathf.FloorToInt(stat.number * 30f), 
			3 => Mathf.FloorToInt(stat.number * 5f), 
			4 => Mathf.FloorToInt(stat.number * 5f), 
			5 => Mathf.FloorToInt(stat.number * 5f), 
			6 => Mathf.FloorToInt(stat.number * 5f), 
			10 => Mathf.FloorToInt(stat.number * 30f), 
			11 => Mathf.FloorToInt(stat.number * 30f), 
			12 => Mathf.FloorToInt(stat.number * 40f), 
			13 => Mathf.FloorToInt(stat.number * 40f), 
			14 => Mathf.FloorToInt(stat.number * 30f), 
			15 => Mathf.FloorToInt(stat.number * 40f), 
			16 => Mathf.FloorToInt(stat.number * 40f), 
			17 => Mathf.FloorToInt(stat.number * 40f), 
			18 => Mathf.FloorToInt(stat.number * 40f), 
			19 => Mathf.FloorToInt(stat.number * 40f), 
			20 => Mathf.FloorToInt(stat.number * 40f), 
			21 => Mathf.FloorToInt(stat.number * 180f), 
			22 => Mathf.FloorToInt(stat.number * 100f), 
			30 => Mathf.FloorToInt(stat.number * 30f), 
			31 => Mathf.FloorToInt(stat.number * 20f), 
			32 => Mathf.FloorToInt(stat.number * 20f), 
			50 => Mathf.FloorToInt(stat.number * 40f), 
			51 => Mathf.FloorToInt(stat.number * 30f), 
			52 => Mathf.FloorToInt(stat.number * 20f), 
			53 => Mathf.FloorToInt(stat.number * 20f), 
			54 => Mathf.FloorToInt(stat.number * 40f), 
			60 => Mathf.FloorToInt(stat.number * 800f), 
			61 => Mathf.FloorToInt(stat.number * 800f), 
			62 => Mathf.FloorToInt(stat.number * 800f), 
			63 => Mathf.FloorToInt(stat.number * 800f), 
			80 => Mathf.FloorToInt(stat.number * 1200f), 
			81 => Mathf.FloorToInt(stat.number * 120f), 
			100 => Mathf.FloorToInt(stat.number * 25f), 
			101 => Mathf.FloorToInt(stat.number * 25f), 
			102 => Mathf.FloorToInt(stat.number * 25f), 
			103 => Mathf.FloorToInt(stat.number * 25f), 
			104 => Mathf.FloorToInt(stat.number * 25f), 
			150 => Mathf.FloorToInt(stat.number * 30f), 
			151 => Mathf.FloorToInt(stat.number * 40f), 
			170 => Mathf.FloorToInt(stat.number * 20f), 
			171 => Mathf.FloorToInt(stat.number * 50f), 
			200 => Mathf.FloorToInt(stat.number * 500f), 
			201 => Mathf.FloorToInt(stat.number * 500f), 
			202 => Mathf.FloorToInt(stat.number * 500f), 
			203 => Mathf.FloorToInt(stat.number * 300f), 
			204 => Mathf.FloorToInt(stat.number * 300f), 
			205 => Mathf.FloorToInt(stat.number * 300f), 
			300 => Mathf.FloorToInt(stat.number * 20f), 
			301 => Mathf.FloorToInt(stat.number * 20f), 
			302 => Mathf.FloorToInt(stat.number * 400f), 
			303 => Mathf.FloorToInt(stat.number * 10f), 
			304 => Mathf.FloorToInt(stat.number * 10f), 
			305 => Mathf.FloorToInt(stat.number * 15f), 
			306 => Mathf.FloorToInt(stat.number * 40f), 
			307 => 1000, 
			400 => Mathf.FloorToInt(stat.number * 300f), 
			401 => Mathf.FloorToInt(stat.number * 300f), 
			402 => Mathf.FloorToInt(stat.number * 400f), 
			403 => Mathf.FloorToInt(stat.number * 400f), 
			404 => Mathf.FloorToInt(stat.number * 300f), 
			405 => Mathf.FloorToInt(stat.number * 8000f), 
			406 => Mathf.FloorToInt(stat.number * 8000f), 
			407 => Mathf.FloorToInt(stat.number * 200f), 
			408 => Mathf.FloorToInt(stat.number * 200f), 
			409 => Mathf.FloorToInt(stat.number * 200f), 
			410 => Mathf.FloorToInt(stat.number * 200f), 
			411 => Mathf.FloorToInt(stat.number * 200f), 
			412 => Mathf.FloorToInt(stat.number * 200f), 
			413 => Mathf.FloorToInt(stat.number * 200f), 
			414 => Mathf.FloorToInt(stat.number * 200f), 
			415 => Mathf.FloorToInt(stat.number * 300f), 
			416 => Mathf.FloorToInt(stat.number * 300f), 
			417 => Mathf.FloorToInt(stat.number * 400f), 
			418 => Mathf.FloorToInt(stat.number * 400f), 
			419 => Mathf.FloorToInt(stat.number * 300f), 
			420 => Mathf.FloorToInt(stat.number * 8000f), 
			421 => Mathf.FloorToInt(stat.number * 8000f), 
			422 => Mathf.FloorToInt(stat.number * 200f), 
			423 => Mathf.FloorToInt(stat.number * 200f), 
			424 => Mathf.FloorToInt(stat.number * 200f), 
			425 => Mathf.FloorToInt(stat.number * 200f), 
			426 => Mathf.FloorToInt(stat.number * 200f), 
			427 => Mathf.FloorToInt(stat.number * 200f), 
			428 => Mathf.FloorToInt(stat.number * 200f), 
			429 => Mathf.FloorToInt(stat.number * 200f), 
			430 => Mathf.FloorToInt(stat.number * 300f), 
			431 => Mathf.FloorToInt(stat.number * 300f), 
			432 => Mathf.FloorToInt(stat.number * 400f), 
			433 => Mathf.FloorToInt(stat.number * 400f), 
			434 => Mathf.FloorToInt(stat.number * 300f), 
			435 => Mathf.FloorToInt(stat.number * 8000f), 
			436 => Mathf.FloorToInt(stat.number * 8000f), 
			437 => Mathf.FloorToInt(stat.number * 200f), 
			438 => Mathf.FloorToInt(stat.number * 200f), 
			439 => Mathf.FloorToInt(stat.number * 200f), 
			440 => Mathf.FloorToInt(stat.number * 200f), 
			441 => Mathf.FloorToInt(stat.number * 200f), 
			442 => Mathf.FloorToInt(stat.number * 200f), 
			443 => Mathf.FloorToInt(stat.number * 200f), 
			444 => Mathf.FloorToInt(stat.number * 200f), 
			445 => Mathf.FloorToInt(stat.number * 1800f), 
			446 => Mathf.FloorToInt(stat.number * 1800f), 
			447 => Mathf.FloorToInt(stat.number * 2400f), 
			448 => Mathf.FloorToInt(stat.number * 1200f), 
			449 => Mathf.FloorToInt(stat.number * 1200f), 
			450 => Mathf.FloorToInt(stat.number * 1200f), 
			451 => Mathf.FloorToInt(stat.number * 1200f), 
			452 => Mathf.FloorToInt(stat.number * 1200f), 
			453 => Mathf.FloorToInt(stat.number * 1200f), 
			454 => Mathf.FloorToInt(stat.number * 1200f), 
			455 => Mathf.FloorToInt(stat.number * 900f), 
			456 => Mathf.FloorToInt(stat.number * 900f), 
			457 => Mathf.FloorToInt(stat.number * 1200f), 
			458 => Mathf.FloorToInt(stat.number * 600f), 
			459 => Mathf.FloorToInt(stat.number * 600f), 
			460 => Mathf.FloorToInt(stat.number * 600f), 
			461 => Mathf.FloorToInt(stat.number * 600f), 
			462 => Mathf.FloorToInt(stat.number * 600f), 
			463 => Mathf.FloorToInt(stat.number * 600f), 
			464 => Mathf.FloorToInt(stat.number * 600f), 
			500 => Mathf.FloorToInt(stat.number * 10f), 
			501 => Mathf.FloorToInt(stat.number * 20f), 
			502 => Mathf.FloorToInt(stat.number * 30f), 
			503 => Mathf.FloorToInt(stat.number * 25f), 
			504 => Mathf.FloorToInt(stat.number * 5f), 
			505 => Mathf.FloorToInt(stat.number * 5f), 
			506 => Mathf.FloorToInt(stat.number * 20f), 
			507 => Mathf.FloorToInt(stat.number * 30f), 
			508 => 500, 
			509 => Mathf.FloorToInt(stat.number * 10f), 
			510 => Mathf.FloorToInt(stat.number * 25f), 
			511 => Mathf.FloorToInt(stat.number * 20f), 
			512 => Mathf.FloorToInt(stat.number * 10f), 
			513 => Mathf.FloorToInt(stat.number * 5f), 
			514 => Mathf.FloorToInt(stat.number * 10f), 
			550 => Mathf.FloorToInt(stat.number * 25f), 
			551 => Mathf.FloorToInt(stat.number * 25f), 
			552 => Mathf.FloorToInt(stat.number * 35f), 
			553 => Mathf.FloorToInt(stat.number * 20f), 
			554 => Mathf.FloorToInt(stat.number * 20f), 
			555 => Mathf.FloorToInt(stat.number * 30f), 
			556 => Mathf.FloorToInt(stat.number * 500f), 
			557 => Mathf.FloorToInt(stat.number * 500f), 
			558 => Mathf.FloorToInt(stat.number * 10f), 
			559 => Mathf.FloorToInt(stat.number * 20f), 
			600 => Mathf.FloorToInt(stat.number * 150f), 
			601 => Mathf.FloorToInt(stat.number * 80f), 
			602 => Mathf.FloorToInt(stat.number * 150f), 
			603 => Mathf.FloorToInt(stat.number * 80f), 
			604 => Mathf.FloorToInt(stat.number * 80f), 
			610 => Mathf.FloorToInt(stat.number * 20f), 
			611 => Mathf.FloorToInt(stat.number * 20f), 
			612 => Mathf.FloorToInt(stat.number * 10f), 
			613 => Mathf.FloorToInt(stat.number * 10f), 
			614 => Mathf.FloorToInt(stat.number * 10f), 
			615 => Mathf.FloorToInt(stat.number * 10f), 
			616 => Mathf.FloorToInt(stat.number * 10f), 
			617 => Mathf.FloorToInt(stat.number * 10f), 
			618 => Mathf.FloorToInt(stat.number * 10f), 
			650 => Mathf.FloorToInt(stat.number * 20f), 
			651 => Mathf.FloorToInt(stat.number * 20f), 
			652 => Mathf.FloorToInt(stat.number * 20f), 
			653 => Mathf.FloorToInt(stat.number * 20f), 
			654 => 200, 
			655 => Mathf.FloorToInt(stat.number * 20f), 
			700 => Mathf.FloorToInt(stat.number * 20f), 
			701 => Mathf.FloorToInt(stat.number * 20f), 
			750 => 800, 
			751 => 800, 
			752 => 2000, 
			753 => 8000, 
			800 => Mathf.FloorToInt(stat.number * 120f), 
			801 => Mathf.FloorToInt(stat.number * 150f), 
			802 => Mathf.FloorToInt(stat.number * 150f), 
			803 => Mathf.FloorToInt(stat.number * 200f), 
			804 => Mathf.FloorToInt(stat.number * 800f), 
			805 => Mathf.FloorToInt(stat.number * 250f), 
			806 => Mathf.FloorToInt(stat.number * 300f), 
			807 => Mathf.FloorToInt(stat.number * 90f), 
			808 => Mathf.FloorToInt(stat.number * 120f), 
			850 => Mathf.FloorToInt(stat.number * 80f), 
			851 => Mathf.FloorToInt(stat.number * 30f), 
			852 => Mathf.FloorToInt(stat.number * 40f), 
			853 => Mathf.FloorToInt(stat.number * 50f), 
			854 => Mathf.FloorToInt(stat.number * 100f), 
			855 => Mathf.FloorToInt(stat.number * 150f), 
			856 => Mathf.FloorToInt(stat.number * 120f), 
			857 => Mathf.FloorToInt(stat.number * 10f), 
			858 => Mathf.FloorToInt(stat.number * 60f), 
			859 => Mathf.FloorToInt(stat.number * 90f), 
			860 => Mathf.FloorToInt(stat.number * 20f), 
			861 => Mathf.FloorToInt(stat.number * 20f), 
			862 => 3000, 
			1000 => Mathf.FloorToInt(stat.number * 300f), 
			1001 => Mathf.FloorToInt(stat.number * 300f), 
			1002 => Mathf.FloorToInt(stat.number * 400f), 
			1003 => Mathf.FloorToInt(stat.number * 300f), 
			1004 => Mathf.FloorToInt(stat.number * 300f), 
			1005 => Mathf.FloorToInt(stat.number * 400f), 
			1006 => Mathf.FloorToInt(stat.number * 400f), 
			1007 => Mathf.FloorToInt(stat.number * 200f), 
			1010 => Mathf.FloorToInt(stat.number * 200f), 
			1011 => Mathf.FloorToInt(stat.number * 200f), 
			1020 => Mathf.FloorToInt(stat.number * 200f), 
			1021 => Mathf.FloorToInt(stat.number * 200f), 
			1022 => Mathf.FloorToInt(stat.number * 200f), 
			1023 => Mathf.FloorToInt(stat.number * 200f), 
			1024 => Mathf.FloorToInt(stat.number * 90f), 
			1025 => Mathf.FloorToInt(stat.number * 90f), 
			1026 => Mathf.FloorToInt(stat.number * 120f), 
			1027 => Mathf.FloorToInt(stat.number * 90f), 
			1028 => Mathf.FloorToInt(stat.number * 90f), 
			1029 => Mathf.FloorToInt(stat.number * 120f), 
			1030 => Mathf.FloorToInt(stat.number * 120f), 
			1031 => Mathf.FloorToInt(stat.number * 60f), 
			1040 => Mathf.FloorToInt(stat.number * 60f), 
			1041 => Mathf.FloorToInt(stat.number * 90f), 
			1050 => Mathf.FloorToInt(stat.number * 60f), 
			1051 => Mathf.FloorToInt(stat.number * 60f), 
			1052 => Mathf.FloorToInt(stat.number * 60f), 
			1053 => Mathf.FloorToInt(stat.number * 60f), 
			1054 => Mathf.FloorToInt(stat.number * 60f), 
			1100 => Mathf.FloorToInt(stat.number * 400f), 
			1101 => Mathf.FloorToInt(stat.number * 400f), 
			1102 => Mathf.FloorToInt(stat.number * 200f), 
			1103 => Mathf.FloorToInt(stat.number * 400f), 
			1104 => Mathf.FloorToInt(stat.number * 400f), 
			1105 => Mathf.FloorToInt(stat.number * 100f), 
			1106 => Mathf.FloorToInt(stat.number * 100f), 
			1107 => Mathf.FloorToInt(stat.number * 400f), 
			1108 => Mathf.FloorToInt(stat.number * 200f), 
			1109 => Mathf.FloorToInt(stat.number * 400f), 
			1110 => Mathf.FloorToInt(stat.number * 200f), 
			1111 => Mathf.FloorToInt(stat.number * 400f), 
			1112 => Mathf.FloorToInt(stat.number * 100f), 
			1113 => Mathf.FloorToInt(stat.number * 300f), 
			1114 => Mathf.FloorToInt(stat.number * 500f), 
			1115 => Mathf.FloorToInt(stat.number * 400f), 
			1116 => Mathf.FloorToInt(stat.number * 400f), 
			1117 => Mathf.FloorToInt(stat.number * 3000f), 
			1118 => Mathf.FloorToInt(stat.number * 300f), 
			1119 => Mathf.FloorToInt(stat.number * 500f), 
			1120 => Mathf.FloorToInt(stat.number * 100f), 
			1121 => Mathf.FloorToInt(stat.number * 150f), 
			1122 => Mathf.FloorToInt(stat.number * 200f), 
			1123 => Mathf.FloorToInt(stat.number * 150f), 
			1124 => Mathf.FloorToInt(stat.number * 80f), 
			1125 => Mathf.FloorToInt(stat.number * 80f), 
			1126 => Mathf.FloorToInt(stat.number * 400f), 
			1127 => Mathf.FloorToInt(stat.number * 200f), 
			1128 => Mathf.FloorToInt(stat.number * 200f), 
			1129 => Mathf.FloorToInt(stat.number * 80f), 
			1130 => Mathf.FloorToInt(stat.number * 200f), 
			1131 => Mathf.FloorToInt(stat.number * 200f), 
			1132 => Mathf.FloorToInt(stat.number * 400f), 
			1133 => Mathf.FloorToInt(stat.number * 300f), 
			1134 => Mathf.FloorToInt(stat.number * 200f), 
			1135 => Mathf.FloorToInt(stat.number * 100f), 
			1136 => Mathf.FloorToInt(stat.number * 100f), 
			1137 => Mathf.FloorToInt(stat.number * 100f), 
			1138 => Mathf.FloorToInt(stat.number * 200f), 
			1139 => Mathf.FloorToInt(stat.number * 80f), 
			1140 => Mathf.FloorToInt(stat.number * 60f), 
			1141 => Mathf.FloorToInt(stat.number * 60f), 
			1142 => Mathf.FloorToInt(stat.number * 100f), 
			1143 => Mathf.FloorToInt(stat.number * 80f), 
			1144 => Mathf.FloorToInt(stat.number * 60f), 
			1145 => Mathf.FloorToInt(stat.number * 80f), 
			1146 => Mathf.FloorToInt(stat.number * 160f), 
			1150 => Mathf.FloorToInt(stat.number * 120f), 
			1200 => Mathf.FloorToInt(stat.number * 20f), 
			1201 => Mathf.FloorToInt(stat.number * 25f), 
			1202 => Mathf.FloorToInt(stat.number * 30f), 
			1203 => Mathf.FloorToInt(stat.number * 30f), 
			1204 => Mathf.FloorToInt(stat.number * 15f), 
			1205 => Mathf.FloorToInt(stat.number * 15f), 
			1206 => Mathf.FloorToInt(stat.number * 20f), 
			1250 => Mathf.FloorToInt(stat.number * 200f), 
			1251 => Mathf.FloorToInt(stat.number * 240f), 
			1252 => Mathf.FloorToInt(stat.number * 120f), 
			1253 => Mathf.FloorToInt(stat.number * 200f), 
			1260 => Mathf.FloorToInt(stat.number * 120f), 
			1270 => Mathf.FloorToInt(stat.number * 60f), 
			1271 => Mathf.FloorToInt(stat.number * 90f), 
			1272 => Mathf.FloorToInt(stat.number * 70f), 
			1273 => Mathf.FloorToInt(stat.number * 90f), 
			1274 => Mathf.FloorToInt(stat.number * 10f), 
			1275 => Mathf.FloorToInt(stat.number * 20f), 
			1276 => Mathf.FloorToInt(stat.number * 80f), 
			1300 => Mathf.FloorToInt(stat.number * 800f), 
			1330 => Mathf.FloorToInt(stat.number * 4000f), 
			1350 => 2000, 
			1360 => 8000, 
			1370 => Mathf.FloorToInt(stat.number * 20f), 
			1371 => Mathf.FloorToInt(stat.number * 30f), 
			1372 => Mathf.FloorToInt(stat.number * 15f), 
			1373 => Mathf.FloorToInt(stat.number * 10f), 
			1374 => Mathf.FloorToInt(stat.number * 10f), 
			1390 => Mathf.FloorToInt(stat.number * 20f), 
			1391 => 5000, 
			1395 => Mathf.FloorToInt(stat.number * 15f), 
			1396 => Mathf.FloorToInt(stat.number * 400f), 
			1397 => Mathf.FloorToInt(stat.number * 400f), 
			1500 => Mathf.FloorToInt(stat.number * 300f), 
			1501 => 10000, 
			1502 => Mathf.FloorToInt(stat.number * 15f), 
			1503 => Mathf.FloorToInt(stat.number * 15f), 
			1504 => Mathf.FloorToInt(stat.number * 20f), 
			1505 => Mathf.FloorToInt(stat.number * 10f), 
			1506 => Mathf.FloorToInt(stat.number * 30f), 
			1507 => Mathf.FloorToInt(stat.number * 20f), 
			1508 => Mathf.FloorToInt(stat.number * 30f), 
			1509 => Mathf.FloorToInt(stat.number * 100f), 
			1510 => Mathf.FloorToInt(stat.number * 300f), 
			1600 => Mathf.FloorToInt(stat.number * 20f), 
			1601 => Mathf.FloorToInt(stat.number * 20f), 
			1602 => Mathf.FloorToInt(stat.number * 15f), 
			1603 => Mathf.FloorToInt(stat.number * 15f), 
			1604 => 3000, 
			1800 => Mathf.FloorToInt(stat.number * 20f), 
			1801 => 5000, 
			1802 => Mathf.FloorToInt(stat.number * 800f), 
			1803 => 2000, 
			1804 => 2000, 
			1805 => 8000, 
			1806 => Mathf.FloorToInt(stat.number * 1000f), 
			1807 => 8000, 
			1808 => Mathf.FloorToInt(stat.number * 10f), 
			1809 => 5000, 
			1810 => 1500, 
			1811 => 1500, 
			1812 => 1500, 
			1813 => 500, 
			1814 => 1000, 
			1815 => Mathf.FloorToInt(stat.number * 40f), 
			1816 => 8000, 
			1817 => Mathf.FloorToInt(stat.number * 20f), 
			1818 => Mathf.FloorToInt(stat.number * 40f), 
			1819 => Mathf.FloorToInt(stat.number * 20f), 
			1820 => 8000, 
			1821 => 1000, 
			1822 => 2000, 
			1900 => 1000, 
			1901 => 1000, 
			1905 => 2000, 
			1910 => Mathf.FloorToInt(stat.number * 10f), 
			1911 => Mathf.FloorToInt(stat.number * 10f), 
			1912 => Mathf.FloorToInt(stat.number * 20f), 
			1950 => Mathf.FloorToInt(stat.number * 40f), 
			1951 => Mathf.FloorToInt(stat.number * 40f), 
			1952 => Mathf.FloorToInt(stat.number * 40f), 
			1953 => Mathf.FloorToInt(stat.number * 90f), 
			1954 => Mathf.FloorToInt(stat.number * 120f), 
			1955 => Mathf.FloorToInt(stat.number * 10f), 
			_ => 0, 
		};
	}

	private static int GetWeaponDotStatPrice(WPDT_A stat)
	{
		if (stat == null)
		{
			return 0;
		}
		return stat.Index switch
		{
			0 => 0, 
			2000 => Mathf.FloorToInt(stat.number * 400f), 
			2001 => 600, 
			2002 => Mathf.FloorToInt(stat.number * 80f), 
			2003 => Mathf.FloorToInt(stat.number * 200f), 
			2004 => Mathf.FloorToInt(stat.number * 800f), 
			2005 => 5000, 
			2100 => 1000, 
			2101 => Mathf.FloorToInt(stat.number * 500f), 
			2102 => 5000, 
			2200 => 2000, 
			2201 => 2000, 
			2202 => Mathf.FloorToInt(stat.number * 400f), 
			2203 => Mathf.FloorToInt(stat.number * 600f), 
			2300 => Mathf.FloorToInt(stat.number * 50f), 
			2301 => 2000, 
			2302 => 2000, 
			2303 => Mathf.FloorToInt(stat.number * 50f), 
			2304 => 3000, 
			2305 => Mathf.FloorToInt(stat.number * 300f), 
			2306 => Mathf.FloorToInt(stat.number * 20f), 
			2400 => 2000, 
			2401 => Mathf.FloorToInt(stat.number * 10f), 
			2402 => Mathf.FloorToInt(stat.number * 400f), 
			2450 => Mathf.FloorToInt(stat.number * 20f), 
			2500 => Mathf.FloorToInt(stat.number * 600f), 
			2501 => Mathf.FloorToInt(stat.number * 10f), 
			2550 => Mathf.FloorToInt(stat.number * 10f), 
			2551 => Mathf.FloorToInt(stat.number * 10f), 
			2552 => Mathf.FloorToInt(stat.number * 10f), 
			2600 => Mathf.FloorToInt(stat.number * 20f), 
			2601 => Mathf.FloorToInt(stat.number * 50f), 
			2602 => Mathf.FloorToInt(stat.number * 10f), 
			_ => 0, 
		};
	}

	private static int GetWeaponSKStatPrice(WPDT_B stat)
	{
		if (stat == null)
		{
			return 0;
		}
		return stat.Index switch
		{
			0 => 0, 
			3000 => 8000, 
			3100 => Mathf.FloorToInt(stat.number * 150f), 
			3101 => Mathf.FloorToInt(stat.number * 150f), 
			3102 => Mathf.FloorToInt(stat.number * 150f), 
			3103 => Mathf.FloorToInt(stat.number * 150f), 
			3200 => 3000, 
			3201 => 6000, 
			3202 => 2000, 
			3203 => 3000, 
			3300 => 2000, 
			3301 => Mathf.FloorToInt(stat.number * 100f), 
			3302 => Mathf.FloorToInt(stat.number * 500f), 
			3303 => Mathf.FloorToInt(stat.number * 300f), 
			3304 => Mathf.FloorToInt(stat.number * 8f), 
			3305 => Mathf.FloorToInt(stat.number * 150f), 
			3306 => Mathf.FloorToInt(stat.number * 6f), 
			3307 => Mathf.FloorToInt(stat.number * 10f), 
			3308 => 6000, 
			3400 => Mathf.FloorToInt(stat.number * 1000f), 
			3401 => Mathf.FloorToInt(stat.number * 80f), 
			3402 => Mathf.FloorToInt(stat.number * 80f), 
			3403 => Mathf.FloorToInt(stat.number * 80f), 
			3404 => Mathf.FloorToInt(stat.number * 30f), 
			3500 => Mathf.FloorToInt(stat.number * 120f), 
			3501 => Mathf.FloorToInt(stat.number * 100f), 
			3502 => Mathf.FloorToInt(stat.number * 100f), 
			3503 => Mathf.FloorToInt(stat.number * 60f), 
			3504 => Mathf.FloorToInt(stat.number * 60f), 
			3530 => Mathf.FloorToInt(stat.number * 60f), 
			3535 => Mathf.FloorToInt(stat.number * 70f), 
			3550 => Mathf.FloorToInt(stat.number * 30f), 
			3551 => Mathf.FloorToInt(stat.number * 30f), 
			3552 => Mathf.FloorToInt(stat.number * 40f), 
			3553 => Mathf.FloorToInt(stat.number * 40f), 
			3554 => Mathf.FloorToInt(stat.number * 40f), 
			3555 => Mathf.FloorToInt(stat.number * 40f), 
			3556 => Mathf.FloorToInt(stat.number * 40f), 
			_ => 0, 
		};
	}

	private static int GetWeaponCPStatPrice(WPDT_B stat)
	{
		if (stat == null)
		{
			return 0;
		}
		return stat.Index switch
		{
			0 => 0, 
			4000 => 8000, 
			4050 => 1000, 
			4100 => Mathf.FloorToInt(stat.number * 200f), 
			4101 => Mathf.FloorToInt(stat.number * 2000f), 
			4200 => Mathf.FloorToInt(stat.number * 50f), 
			4201 => Mathf.FloorToInt(stat.number * 1000f), 
			4202 => 1000, 
			4300 => Mathf.FloorToInt(stat.number * 500f), 
			4301 => Mathf.FloorToInt(stat.number * 8f), 
			4302 => Mathf.FloorToInt(stat.number * 150f), 
			4303 => Mathf.FloorToInt(stat.number * 400f), 
			4304 => 5000, 
			4305 => 5000, 
			4306 => Mathf.FloorToInt(stat.number * 8f), 
			4400 => Mathf.FloorToInt(stat.number * 240f), 
			4401 => Mathf.FloorToInt(stat.number * 240f), 
			_ => 0, 
		};
	}

	private WPDT_A[] BuildWeaponRuntimeDataA(WPDT_A[] baseData, WPDT_A[] rateData, bool resolveRuntimeElement = false, int level = 0, int quality = 0, WeaponDropContext dropContext = default(WeaponDropContext), WeaponStatGroup statGroup = WeaponStatGroup.Main, bool scaleMainRecoveryValues = false)
	{
		List<WPDT_A> list = new List<WPDT_A>();
		AddWeaponDataA(list, baseData, resolveRuntimeElement, level, quality, dropContext, statGroup, scaleMainRecoveryValues);
		WPDT_A randomWeaponDataA = GetRandomWeaponDataA(rateData);
		if (randomWeaponDataA != null)
		{
			AddWeaponDataA(list, new WPDT_A[1] { randomWeaponDataA }, resolveRuntimeElement, level, quality, dropContext, statGroup, scaleMainRecoveryValues);
		}
		return list.ToArray();
	}

	private WPDT_B[] BuildWeaponRuntimeDataB(WPDT_B[] baseData, WPDT_B[] rateData, bool resolveRuntimeElement = false, int level = 0, int quality = 0, WeaponDropContext dropContext = default(WeaponDropContext), WeaponStatGroup statGroup = WeaponStatGroup.Skill)
	{
		List<WPDT_B> list = new List<WPDT_B>();
		AddWeaponDataB(list, baseData, resolveRuntimeElement, level, quality, dropContext, statGroup);
		WPDT_B randomWeaponDataB = GetRandomWeaponDataB(rateData);
		if (randomWeaponDataB != null)
		{
			AddWeaponDataB(list, new WPDT_B[1] { randomWeaponDataB }, resolveRuntimeElement, level, quality, dropContext, statGroup);
		}
		return list.ToArray();
	}

	private void BuildWeaponSkillRuntimeData(WPDT_B[] skillBaseData, WPDT_B[] companionBaseData, WPDT_B[] rateData, int randomSkillCount, bool resolveRuntimeElement, int level, int quality, WeaponDropContext dropContext, out WPDT_B[] skillData, out WPDT_B[] companionData)
	{
		List<WPDT_B> list = new List<WPDT_B>();
		List<WPDT_B> list2 = new List<WPDT_B>();
		HashSet<string> hashSet = new HashSet<string>();
		AddWeaponSkillData(list, list2, skillBaseData, resolveRuntimeElement, level, quality, dropContext, WeaponStatGroup.Skill, hashSet, skipUnknownSkillType: false);
		AddWeaponSkillData(list, list2, companionBaseData, resolveRuntimeElement, level, quality, dropContext, WeaponStatGroup.Companion, hashSet, skipUnknownSkillType: false);
		for (int i = 0; i < randomSkillCount; i++)
		{
			WPDT_B randomWeaponDataB = GetRandomWeaponDataB(rateData, hashSet);
			if (randomWeaponDataB == null)
			{
				break;
			}
			AddWeaponSkillData(list, list2, new WPDT_B[1] { randomWeaponDataB }, resolveRuntimeElement, level, quality, dropContext, WeaponStatGroup.Skill, hashSet, skipUnknownSkillType: true);
		}
		skillData = list.ToArray();
		companionData = list2.ToArray();
	}

	private void AddWeaponDataA(List<WPDT_A> target, WPDT_A[] source, bool resolveRuntimeElement = false, int level = 0, int quality = 0, WeaponDropContext dropContext = default(WeaponDropContext), WeaponStatGroup statGroup = WeaponStatGroup.Main, bool scaleMainRecoveryValues = false)
	{
		if (target == null || source == null)
		{
			return;
		}
		foreach (WPDT_A wPDT_A in source)
		{
			if (wPDT_A != null && wPDT_A.Index != 0)
			{
				float number = GenerateWeaponStatValue(wPDT_A.number, wPDT_A.Index, level, quality, dropContext, statGroup, scaleMainRecoveryValues);
				target.Add(new WPDT_A
				{
					Index = wPDT_A.Index,
					EL = (resolveRuntimeElement ? ResolveGeneratedWeaponElement(wPDT_A.EL) : wPDT_A.EL),
					number = number
				});
			}
		}
	}

	private static bool IsMainRecoveryIndex(int index)
	{
		if (index >= 3)
		{
			return index <= 6;
		}
		return false;
	}

	private void AddWeaponSkillData(List<WPDT_B> skillTarget, List<WPDT_B> companionTarget, WPDT_B[] source, bool resolveRuntimeElement, int level, int quality, WeaponDropContext dropContext, WeaponStatGroup fallbackGroup, HashSet<string> usedSkillEffects, bool skipUnknownSkillType)
	{
		if (skillTarget == null || companionTarget == null || source == null)
		{
			return;
		}
		foreach (WPDT_B wPDT_B in source)
		{
			if (wPDT_B == null || IsCsvNoneText(wPDT_B.SkillName))
			{
				continue;
			}
			int num = GetMergedRandomSkillType(wPDT_B.SkillName);
			if (num != 0 && num != 2)
			{
				if (skipUnknownSkillType)
				{
					continue;
				}
				num = ((fallbackGroup == WeaponStatGroup.Companion) ? 2 : 0);
			}
			string weaponSkillEffectKey = GetWeaponSkillEffectKey(wPDT_B);
			if (usedSkillEffects == null || usedSkillEffects.Add(weaponSkillEffectKey))
			{
				WeaponStatGroup statGroup = ((num == 2) ? WeaponStatGroup.Companion : WeaponStatGroup.Skill);
				((num == 2) ? companionTarget : skillTarget).Add(new WPDT_B
				{
					SkillName = wPDT_B.SkillName,
					Index = wPDT_B.Index,
					GlobleID = wPDT_B.GlobleID,
					EL = (resolveRuntimeElement ? ResolveGeneratedWeaponElement(wPDT_B.EL) : wPDT_B.EL),
					number = GenerateWeaponStatValue(wPDT_B.number, wPDT_B.Index, level, quality, dropContext, statGroup),
					LinkSK = wPDT_B.LinkSK
				});
			}
		}
	}

	private void AddWeaponDataB(List<WPDT_B> target, WPDT_B[] source, bool resolveRuntimeElement = false, int level = 0, int quality = 0, WeaponDropContext dropContext = default(WeaponDropContext), WeaponStatGroup statGroup = WeaponStatGroup.Skill)
	{
		if (target == null || source == null)
		{
			return;
		}
		foreach (WPDT_B wPDT_B in source)
		{
			if (wPDT_B != null && !IsCsvNoneText(wPDT_B.SkillName))
			{
				target.Add(new WPDT_B
				{
					SkillName = wPDT_B.SkillName,
					Index = wPDT_B.Index,
					GlobleID = wPDT_B.GlobleID,
					EL = (resolveRuntimeElement ? ResolveGeneratedWeaponElement(wPDT_B.EL) : wPDT_B.EL),
					number = GenerateWeaponStatValue(wPDT_B.number, wPDT_B.Index, level, quality, dropContext, statGroup),
					LinkSK = wPDT_B.LinkSK
				});
			}
		}
	}

	private float GenerateWeaponStatValue(float sourceValue, int index, int level, int quality, WeaponDropContext dropContext, WeaponStatGroup statGroup, bool scaleMainRecoveryValues = false)
	{
		if (scaleMainRecoveryValues && statGroup == WeaponStatGroup.Main && IsMainRecoveryIndex(index))
		{
			float num = GivePRC_Base(level, dropContext);
			return sourceValue * Mathf.Pow(MultiLevelA, level) * (1f + UnityEngine.Random.Range(0f - RandomCount, RandomCount)) * num;
		}
		if (IsWeaponIntegerGrowthIndex(index))
		{
			return ApplyWeaponIntegerGrowth(sourceValue, level, quality, dropContext);
		}
		if (IsMijingExtraIntegerIndex(index))
		{
			return ApplyMijingExtraIntegerGrowth(sourceValue, quality, dropContext);
		}
		if (IsWeaponFloatWholeIndex(index) || IsWeaponFloatOneDecimalIndex(index))
		{
			return sourceValue * GetWeaponStatRandomMultiplier(level, dropContext);
		}
		return sourceValue;
	}

	private static float GetWeaponStatRandomMultiplier(int level, WeaponDropContext dropContext)
	{
		if (dropContext.IsMijing)
		{
			return Mathf.Clamp(dropContext.DropScene, 1, 4) switch
			{
				1 => UnityEngine.Random.Range(1.2f, 1.3f), 
				2 => UnityEngine.Random.Range(1.2f, 1.4f), 
				3 => UnityEngine.Random.Range(1.3f, 1.5f), 
				_ => UnityEngine.Random.Range(1.4f, 1.6f), 
			};
		}
		if (level < 40)
		{
			return UnityEngine.Random.Range(0.9f, 1f);
		}
		if (level < 50)
		{
			return UnityEngine.Random.Range(0.9f, 1.1f);
		}
		if (level < 70)
		{
			return UnityEngine.Random.Range(1f, 1.1f);
		}
		if (level < 80)
		{
			return UnityEngine.Random.Range(1f, 1.2f);
		}
		if (level < 90)
		{
			return UnityEngine.Random.Range(1f, 1.3f);
		}
		return UnityEngine.Random.Range(1.1f, 1.3f);
	}

	private static float ApplyWeaponIntegerGrowth(float sourceValue, int level, int quality, WeaponDropContext dropContext)
	{
		int num = 0;
		if (dropContext.IsMijing)
		{
			if (UnityEngine.Random.value < 0.8f)
			{
				num = ((quality < 5) ? 1 : ((UnityEngine.Random.value < 0.5f) ? 1 : 2));
			}
		}
		else if (level >= 50 && level < 80)
		{
			if (UnityEngine.Random.value < 0.3f)
			{
				num = 1;
			}
		}
		else if (level >= 80 && UnityEngine.Random.value < 0.5f)
		{
			num = ((quality < 5) ? 1 : ((UnityEngine.Random.value < 0.7f) ? 1 : 2));
		}
		return Mathf.Floor(sourceValue) + (float)num;
	}

	private static float ApplyMijingExtraIntegerGrowth(float sourceValue, int quality, WeaponDropContext dropContext)
	{
		if (!dropContext.IsMijing || quality < 5)
		{
			return Mathf.Floor(sourceValue);
		}
		int num = Mathf.FloorToInt(sourceValue);
		float num2;
		int num3;
		if (num < 2)
		{
			num2 = 0.1f;
			num3 = 1;
		}
		else if (num < 5)
		{
			num2 = 0.3f;
			num3 = 1;
		}
		else if (num < 9)
		{
			num2 = 0.5f;
			num3 = ((UnityEngine.Random.value < 0.7f) ? 1 : 2);
		}
		else
		{
			num2 = 0.8f;
			float value = UnityEngine.Random.value;
			num3 = ((value < 0.4f) ? 1 : ((value < 0.7f) ? 2 : 3));
		}
		return num + ((UnityEngine.Random.value < num2) ? num3 : 0);
	}

	public static string FormatWeaponStatValue(int index, float value)
	{
		if (IsWeaponFloatWholeIndex(index))
		{
			return Mathf.FloorToInt(value).ToString(CultureInfo.InvariantCulture);
		}
		string text = (IsWeaponFloatOneDecimalIndex(index) ? "0.0" : "0.##");
		return value.ToString(text, CultureInfo.InvariantCulture);
	}

	private static bool IsWeaponFloatWholeIndex(int index)
	{
		switch (index)
		{
		case 556:
		case 557:
			return false;
		case 1:
		case 2:
		case 81:
		case 150:
		case 151:
		case 170:
		case 171:
		case 650:
		case 651:
		case 652:
		case 653:
		case 655:
		case 1300:
		case 1502:
		case 1503:
		case 1504:
		case 1505:
		case 1506:
		case 1507:
		case 1508:
		case 1509:
		case 1510:
		case 1808:
		case 1815:
		case 1817:
		case 1818:
		case 1819:
		case 2401:
		case 2450:
		case 2501:
		case 2550:
		case 2551:
		case 2552:
		case 3403:
		case 3404:
		case 3530:
		case 3535:
			return true;
		default:
			if ((index >= 10 && index <= 22) || (index >= 30 && index <= 32) || (index >= 50 && index <= 54) || (index >= 100 && index <= 104) || (index >= 300 && index <= 301) || (index >= 303 && index <= 306) || (index >= 500 && index <= 507) || (index >= 509 && index <= 514) || (index >= 550 && index <= 559) || (index >= 610 && index <= 618) || (index >= 800 && index <= 808) || (index >= 850 && index <= 852) || (index >= 855 && index <= 861) || (index >= 1250 && index <= 1253) || (index >= 1270 && index <= 1276) || (index >= 1370 && index <= 1374) || (index >= 1395 && index <= 1397) || (index >= 1600 && index <= 1603) || (index >= 1950 && index <= 1955) || (index >= 3550 && index <= 3561) || (index >= 4400 && index <= 4417))
			{
				return true;
			}
			if (index != 1260 && index != 1275 && index != 2300 && index != 2303 && index != 2305 && index != 2306 && index != 2600 && index != 2601 && index != 2602 && index != 2603 && index != 3301 && index != 3302 && index != 3303 && index != 3304 && index != 3306 && index != 3307 && index != 3500 && index != 3501 && index != 3502 && index != 3503 && index != 3504 && index != 4301 && index != 4302 && index != 4306)
			{
				return index == 4308;
			}
			return true;
		}
	}

	private static bool IsWeaponFloatOneDecimalIndex(int index)
	{
		if ((uint)(index - 400) > 64u)
		{
			switch (index)
			{
			case 556:
			case 557:
				return true;
			default:
				if ((index >= 1020 && index <= 1031) || (index >= 1050 && index <= 1054) || (index >= 1200 && index <= 1206))
				{
					break;
				}
				if (index < 1000 || index > 1007)
				{
					switch (index)
					{
					default:
						if ((uint)(index - 60) > 3u && (uint)(index - 700) > 1u && index != 1802 && index != 1806 && index != 2203 && index != 2402 && index != 2500 && index != 3305)
						{
							return index == 4307;
						}
						break;
					case 600:
					case 601:
					case 602:
					case 603:
					case 604:
					case 1010:
					case 1011:
					case 1040:
					case 1041:
					case 1150:
						break;
					}
				}
				return true;
			case 1100:
			case 1101:
			case 1102:
			case 1103:
			case 1104:
			case 1105:
			case 1106:
			case 1107:
			case 1108:
			case 1109:
			case 1110:
			case 1111:
			case 1112:
			case 1113:
			case 1114:
			case 1115:
			case 1116:
			case 1117:
			case 1118:
			case 1119:
			case 1120:
			case 1121:
			case 1122:
			case 1123:
			case 1124:
			case 1125:
			case 1126:
			case 1127:
			case 1128:
			case 1129:
			case 1130:
			case 1131:
			case 1132:
			case 1133:
			case 1134:
			case 1135:
			case 1136:
			case 1137:
			case 1138:
			case 1139:
			case 1140:
			case 1141:
			case 1142:
			case 1143:
			case 1144:
			case 1145:
			case 1146:
				break;
			}
		}
		return true;
	}

	private static bool IsWeaponIntegerGrowthIndex(int index)
	{
		switch (index)
		{
		default:
			return index == 4303;
		case 302:
		case 1500:
		case 1910:
		case 1911:
		case 1912:
		case 2000:
		case 2101:
		case 2202:
			return true;
		}
	}

	private static bool IsMijingExtraIntegerIndex(int index)
	{
		if (index != 80 && (uint)(index - 3100) > 3u && index != 4100)
		{
			return index == 4200;
		}
		return true;
	}

	private static int ResolveGeneratedWeaponElement(int el)
	{
		return el switch
		{
			6 => GetRandomCurrentTalentTreeElement(), 
			7 => UnityEngine.Random.Range(0, 6), 
			_ => Mathf.Clamp(el, 0, 5), 
		};
	}

	private static int GetRandomCurrentTalentTreeElement()
	{
		List<int> list = new List<int>();
		if (SingletonMonoScope<TalentManager>.HasInstance && SingletonMonoScope<TalentManager>.Instance.XiData != null)
		{
			SkillXiData[] xiData = SingletonMonoScope<TalentManager>.Instance.XiData;
			int num = Mathf.Clamp((SingletonMonoScope<PlayerManager>.HasInstance ? Mathf.Clamp(SingletonMonoScope<PlayerManager>.Instance.PLType, 0, 3) : 0) * 3, 0, xiData.Length);
			int num2 = Mathf.Min(num + 3, xiData.Length);
			for (int i = num; i < num2; i++)
			{
				SkillXiData skillXiData = xiData[i];
				if (skillXiData != null)
				{
					list.Add(DamageTypeToElementIndex(skillXiData.damageType));
				}
			}
		}
		if (list.Count <= 0)
		{
			return 0;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private static int DamageTypeToElementIndex(DamageType damageType)
	{
		return damageType switch
		{
			DamageType.fire => 0, 
			DamageType.frozen => 1, 
			DamageType.thunder => 2, 
			DamageType.poison => 3, 
			DamageType.physics => 4, 
			DamageType.shadow => 5, 
			_ => 0, 
		};
	}

	private static int GetGeneratedWeaponSkillCount(int quality, WeaponDropContext dropContext)
	{
		if (quality < 4)
		{
			if (!RollWeaponSkillChance(dropContext.IsMijing ? GetMijingSkillChance(dropContext, 70f, 80f, 90f, 100f) : 60f))
			{
				return 0;
			}
			return 1;
		}
		switch (quality)
		{
		case 4:
		{
			float chance = (dropContext.IsMijing ? GetMijingSkillChance(dropContext, 50f, 70f, 90f, 100f) : 30f);
			return 1 + (RollWeaponSkillChance(chance) ? 1 : 0);
		}
		case 5:
			if (dropContext.IsMijing)
			{
				float mijingSkillChance2 = GetMijingSkillChance(dropContext, 0f, 10f, 20f, 30f);
				return 2 + (RollWeaponSkillChance(mijingSkillChance2) ? 1 : 0);
			}
			return 1 + (RollWeaponSkillChance(80f) ? 1 : 0);
		default:
		{
			int num = 2;
			if (dropContext.IsMijing)
			{
				float mijingSkillChance = GetMijingSkillChance(dropContext, 10f, 20f, 30f, 40f);
				num += (RollWeaponSkillChance(mijingSkillChance) ? 1 : 0);
			}
			return num;
		}
		}
	}

	private static float GetMijingSkillChance(WeaponDropContext dropContext, float normal, float hard, float nightmare, float inferno)
	{
		return Mathf.Clamp(dropContext.DropScene, 1, 4) switch
		{
			1 => normal, 
			2 => hard, 
			3 => nightmare, 
			_ => inferno, 
		};
	}

	private static bool RollWeaponSkillChance(float chance)
	{
		if (chance <= 0f)
		{
			return false;
		}
		if (!(chance >= 100f))
		{
			return UnityEngine.Random.value < chance * 0.01f;
		}
		return true;
	}

	private static WPDT_A GetRandomWeaponDataA(WPDT_A[] source)
	{
		if (source == null || source.Length == 0)
		{
			return null;
		}
		List<WPDT_A> list = new List<WPDT_A>();
		foreach (WPDT_A wPDT_A in source)
		{
			if (wPDT_A != null && wPDT_A.Index != 0)
			{
				list.Add(wPDT_A);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private static WPDT_B GetRandomWeaponDataB(WPDT_B[] source, HashSet<string> blockedSkillEffects = null)
	{
		if (source == null || source.Length == 0)
		{
			return null;
		}
		List<WPDT_B> list = new List<WPDT_B>();
		foreach (WPDT_B wPDT_B in source)
		{
			if (wPDT_B != null && !IsCsvNoneText(wPDT_B.SkillName) && (blockedSkillEffects == null || !blockedSkillEffects.Contains(GetWeaponSkillEffectKey(wPDT_B))))
			{
				list.Add(wPDT_B);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private static string GetWeaponSkillEffectKey(WPDT_B data)
	{
		if (data == null)
		{
			return string.Empty;
		}
		return (data.SkillName ?? string.Empty) + "|" + data.Index.ToString(CultureInfo.InvariantCulture);
	}

	private static WPSPC GetRandomWeaponSPC(List<WPSPC> source)
	{
		if (source == null || source.Count == 0)
		{
			return null;
		}
		List<WPSPC> list = new List<WPSPC>();
		for (int i = 0; i < source.Count; i++)
		{
			WPSPC wPSPC = source[i];
			if (wPSPC != null && wPSPC.Index != 0)
			{
				list.Add(wPSPC);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private void ApplyElement(WeaponClass weapon, float baseValue, int level)
	{
		ApplyElement(weapon, baseValue, level, GetCurrentWeaponDropContext());
	}

	private void ApplyElement(WeaponClass weapon, float baseValue, int level, WeaponDropContext dropContext)
	{
		if (weapon == null)
		{
			return;
		}
		weapon.Fire = 0f;
		weapon.Frozen = 0f;
		weapon.Thunder = 0f;
		weapon.Poison = 0f;
		weapon.Physics = 0f;
		weapon.Shadow = 0f;
		if (!(baseValue <= 0f))
		{
			int elementEnhanceSplitCount = GetElementEnhanceSplitCount(baseValue);
			float num = baseValue / (float)elementEnhanceSplitCount + (float)Mathf.FloorToInt(baseValue * GivePRC_PRC(level, dropContext));
			List<int> randomDistinctElementTypes = GetRandomDistinctElementTypes(elementEnhanceSplitCount);
			for (int i = 0; i < randomDistinctElementTypes.Count; i++)
			{
				float value = Mathf.FloorToInt(num * UnityEngine.Random.Range(1f - RDEL, 1f + RDEL));
				SetElementEnhanceValue(weapon, randomDistinctElementTypes[i], value);
			}
		}
	}

	private static int GetElementEnhanceSplitCount(float value)
	{
		int min;
		int num;
		if (value < 10f)
		{
			min = 1;
			num = 1;
		}
		else if (value < 25f)
		{
			min = 1;
			num = 2;
		}
		else if (value < 45f)
		{
			min = 2;
			num = 3;
		}
		else
		{
			min = 2;
			num = 4;
		}
		return UnityEngine.Random.Range(min, num + 1);
	}

	private static List<int> GetRandomDistinctElementTypes(int count)
	{
		List<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };
		List<int> list2 = new List<int>();
		count = Mathf.Clamp(count, 0, list.Count);
		for (int i = 0; i < count; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		return list2;
	}

	private static void SetElementEnhanceValue(WeaponClass weapon, int element, float value)
	{
		switch (Mathf.Clamp(element, 0, 5))
		{
		case 0:
			weapon.Fire = value;
			break;
		case 1:
			weapon.Frozen = value;
			break;
		case 2:
			weapon.Thunder = value;
			break;
		case 3:
			weapon.Poison = value;
			break;
		case 4:
			weapon.Physics = value;
			break;
		case 5:
			weapon.Shadow = value;
			break;
		}
	}

	public void SetBaoshidata(DropItemController drop, int index)
	{
		if ((bool)drop && drop.baoshi != null && index >= 0 && index < Baoshi.Count)
		{
			drop.ItemType = 1;
			ItemCloneUtil.CopyBaoshiTo(drop.baoshi, Baoshi[index]);
		}
	}

	public void SetPotiondata(DropItemController drop, int index)
	{
		if ((bool)drop && drop.useitem != null && index >= 0 && index < Potion.Count)
		{
			drop.ItemType = 2;
			ItemCloneUtil.CopyUseItemTo(drop.useitem, Potion[index]);
		}
	}

	public void SetBuffPotiondata(DropItemController drop, int index)
	{
		if ((bool)drop && drop.useitem != null && index >= 0 && index < BuffPotion.Count)
		{
			drop.ItemType = 2;
			ItemCloneUtil.CopyUseItemTo(drop.useitem, BuffPotion[index]);
		}
	}

	public void SetPremPotiondata(DropItemController drop, int index)
	{
		if ((bool)drop && drop.useitem != null && index >= 0 && index < PremPotion.Count)
		{
			drop.ItemType = 2;
			ItemCloneUtil.CopyUseItemTo(drop.useitem, PremPotion[index]);
		}
	}

	public void SetSPCdata(DropItemController drop, UseItemClass source)
	{
		if ((bool)drop && drop.useitem != null && source != null)
		{
			drop.ItemType = 2;
			ItemCloneUtil.CopyUseItemTo(drop.useitem, source);
		}
	}

	public void ThrowWP(WeaponClass it)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(PL.transform.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), PL.transform.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
		component.ItemType = 0;
		component.PlayerDrop = true;
		ItemCloneUtil.CopyWeaponTo(component.weapon, it);
		WeaponState weaponState = WeaponState.FromRuntime(component.weapon);
		weaponState.Position = component.transform.position;
		component.RuntimeState = weaponState;
		component.InitDrop(component.weapon, 0.3f);
	}

	public void ThrowBS(BaoshiClass it)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(PL.transform.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), PL.transform.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
		component.ItemType = 1;
		component.PlayerDrop = true;
		ItemCloneUtil.CopyBaoshiTo(component.baoshi, it);
		BaoshiState baoshiState = BaoshiState.FromRuntime(component.baoshi);
		baoshiState.Position = component.transform.position;
		component.RuntimeState = baoshiState;
		component.InitDrop(component.baoshi, 0.3f);
	}

	public void ThrowUSE(UseItemClass it)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(PL.transform.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), PL.transform.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
		component.ItemType = 2;
		component.PlayerDrop = true;
		ItemCloneUtil.CopyUseItemTo(component.useitem, it);
		UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
		useItemState.Position = component.transform.position;
		component.RuntimeState = useItemState;
		component.InitDrop(component.useitem, 0.3f);
	}

	public static float GivePRC_Base(int level)
	{
		return GivePRC_Base(level, GetCurrentWeaponDropContext());
	}

	private static float GivePRC_Base(int level, WeaponDropContext dropContext)
	{
		if (level >= 100 && dropContext.IsMijing)
		{
			return GetMijingWPDamageMultiplier(dropContext);
		}
		return 1f;
	}

	public static float GivePRC_PRC(int level)
	{
		return GivePRC_PRC(level, GetCurrentWeaponDropContext());
	}

	private static float GivePRC_PRC(int level, WeaponDropContext dropContext)
	{
		if (level <= 20)
		{
			return 0f;
		}
		if (level <= 40)
		{
			return 0.05f;
		}
		if (level <= 60)
		{
			return 0.1f;
		}
		if (level <= 80)
		{
			return 0.15f;
		}
		if (level <= 90)
		{
			return 0.2f;
		}
		if (level <= 99)
		{
			return 0.23f;
		}
		float num = (dropContext.IsMijing ? GetMijingWPPRCMultiplier(dropContext) : 1f);
		return 0.25f * num;
	}

	public static float GivePRC_SPC(int level, int quality)
	{
		return GivePRC_SPC(level, quality, GetCurrentWeaponDropContext());
	}

	private static float GivePRC_SPC(int level, int quality, WeaponDropContext dropContext)
	{
		float num = UnityEngine.Random.Range(0.9f, 1.1f);
		float num2 = num * quality switch
		{
			0 => 1f, 
			1 => 1f, 
			2 => 1f, 
			3 => 1.2f, 
			4 => 1.4f, 
			5 => 1.6f, 
			6 => 1.8f, 
			_ => 1f, 
		};
		if (level <= 6)
		{
			return 0.5f * num2;
		}
		if (level <= 10)
		{
			return 0.6f * num2;
		}
		if (level <= 15)
		{
			return 0.8f * num2;
		}
		if (level <= 20)
		{
			return 1f * num2;
		}
		if (level <= 30)
		{
			return 1.2f * num2;
		}
		if (level <= 40)
		{
			return 1.5f * num2;
		}
		if (level <= 50)
		{
			return 1.9f * num2;
		}
		if (level <= 60)
		{
			return 2.5f * num2;
		}
		if (level <= 65)
		{
			return 3.4f * num2;
		}
		if (level <= 70)
		{
			return 4.5f * num2;
		}
		if (level <= 75)
		{
			return 5.7f * num2;
		}
		if (level <= 80)
		{
			return 7f * num2;
		}
		if (level <= 85)
		{
			return 8.5f * num2;
		}
		if (level <= 90)
		{
			return 10.5f * num2;
		}
		if (level <= 95)
		{
			return 13f * num2;
		}
		if (level <= 99)
		{
			return 15f * num2;
		}
		float num3 = (dropContext.IsMijing ? GetMijingSPCDamageMultiplier(dropContext) : 1f);
		return 15f * num2 * num3;
	}

	public static float DR_Level_multi()
	{
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 20)
		{
			return 1f;
		}
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 30)
		{
			return 1.05f;
		}
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 40)
		{
			return 1.1f;
		}
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 50)
		{
			return 1.15f;
		}
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 60)
		{
			return 1.2f;
		}
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 70)
		{
			return 1.25f;
		}
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 80)
		{
			return 1.3f;
		}
		if (SingletonMonoScope<PlayerManager>.Instance.Level < 90)
		{
			return 1.35f;
		}
		return 1.4f;
	}

	public static int WPSK_multi(int quality)
	{
		switch (quality)
		{
		case 0:
			return 0;
		case 1:
			return 0;
		case 2:
		{
			int num9 = UnityEngine.Random.Range(0, 101);
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 40)
			{
				return 0;
			}
			if (num9 < 95)
			{
				return 0;
			}
			return 1;
		}
		case 3:
		{
			int num7 = UnityEngine.Random.Range(0, 101);
			int num8 = UnityEngine.Random.Range(0, 101);
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 30)
			{
				return 0;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 70)
			{
				if (num7 < 95)
				{
					return 0;
				}
				return 1;
			}
			if (num7 < 80)
			{
				return 0;
			}
			if (num8 < 80)
			{
				return 1;
			}
			return 2;
		}
		case 4:
		{
			int num10 = UnityEngine.Random.Range(0, 101);
			int num11 = UnityEngine.Random.Range(0, 101);
			int num12 = UnityEngine.Random.Range(0, 101);
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 30)
			{
				return 0;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 70)
			{
				if (num10 < 95)
				{
					return 0;
				}
				return 1;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 90)
			{
				if (num10 < 70)
				{
					return 0;
				}
				if (num11 < 80)
				{
					return 1;
				}
				return 2;
			}
			if (num12 < 60)
			{
				return 0;
			}
			if (num12 < 90)
			{
				return 1;
			}
			return 2;
		}
		case 5:
		{
			int num4 = UnityEngine.Random.Range(0, 101);
			int num5 = UnityEngine.Random.Range(0, 101);
			int num6 = UnityEngine.Random.Range(0, 101);
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 30)
			{
				return 0;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 70)
			{
				if (num4 < 95)
				{
					return 0;
				}
				return 1;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 90)
			{
				if (num4 < 70)
				{
					return 0;
				}
				if (num5 < 80)
				{
					return 1;
				}
				return 2;
			}
			if (num6 < 60)
			{
				return 0;
			}
			if (num6 < 90)
			{
				return 1;
			}
			return 2;
		}
		case 6:
		{
			int num = UnityEngine.Random.Range(0, 101);
			int num2 = UnityEngine.Random.Range(0, 101);
			int num3 = UnityEngine.Random.Range(0, 101);
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 30)
			{
				return 0;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 70)
			{
				if (num < 95)
				{
					return 0;
				}
				return 1;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level < 90)
			{
				if (num < 70)
				{
					return 0;
				}
				if (num2 < 80)
				{
					return 1;
				}
				return 2;
			}
			if (SingletonMonoScope<PlayerManager>.Instance.Level >= 100)
			{
				if (num3 < 60)
				{
					return 0;
				}
				if (num3 < 90)
				{
					return 1;
				}
				return 2;
			}
			return 0;
		}
		default:
			return 0;
		}
	}

	private static bool WPRoll(float chance)
	{
		return UnityEngine.Random.value < Mathf.Clamp01(chance);
	}

	private static bool ItemRoll(float chance)
	{
		return UnityEngine.Random.value < Mathf.Clamp01(chance * DR_Level_multi());
	}

	private static bool MJRoll(float chance)
	{
		if (!LevelManager.GetIsMijing() || !SingletonMonoScope<MijingManager>.HasInstance)
		{
			return false;
		}
		float rareItemDropRateMultiplier = SingletonMonoScope<MijingManager>.Instance.GetRareItemDropRateMultiplier();
		return UnityEngine.Random.value < Mathf.Clamp01(chance * rareItemDropRateMultiplier);
	}

	public void ReLoad()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				Weapon.GP[i].QL[j].Normal.Clear();
				Weapon.GP[i].QL[j].Magic.Clear();
				Weapon.GP[i].QL[j].Rare.Clear();
				Weapon.GP[i].QL[j].Exquisite.Clear();
				Weapon.GP[i].QL[j].Epic.Clear();
				Weapon.GP[i].QL[j].Legendary.Clear();
				Weapon.GP[i].QL[j].Mythical.Clear();
			}
		}
		SPC.Clear();
		SPC_Rune.Clear();
		Baoshi.Clear();
		Potion.Clear();
		BuffPotion.Clear();
		Scroll.Clear();
		PremPotion.Clear();
		SpcPotion.Clear();
		SpcItem.Clear();
		SET.Clear();
		WP_Main.Clear();
		WP_DOT.Clear();
		WP_SK.Clear();
		WP_CP.Clear();
		LoadData_RandomA(Maintext, WP_Main);
		LoadData_RandomA(Dottext, WP_DOT);
		LoadData_RandomMergedSkillB(SKtext);
		LoadData_WP(WPtext);
		LoadData_SPC(SPCtext);
		LoadData_BS(BStext);
		LoadData_USE(USEtext);
		LoadData_SET(Settext);
		LoadData_Skill(Skilltext);
	}

	private void LoadData_WP(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		if (array == null)
		{
			return;
		}
		for (int i = 1; i < array.Length - 1; i++)
		{
			string[] array2 = array[i];
			if (array2 == null || array2.Length <= 2)
			{
				continue;
			}
			Item_MB item_MB = new Item_MB();
			int num = 2;
			item_MB.ItemName = GetCsvCell(array2, num);
			num++;
			item_MB.GlobalID = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.ItemType = 0;
			item_MB.DropLevelStart = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.Quality = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.SizeX = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.SizeY = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.MaxAocaoCount = item_MB.SizeX * item_MB.SizeY;
			item_MB.CurAocaoCount = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.IconType = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.Icon = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.SoundDrop = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.SoundUse = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.RotateType = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.PLtype = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.WeaponType = GetCsvCell(array2, num);
			num++;
			item_MB.CharType = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.Damage = ParseCsvFloat(GetCsvCell(array2, num));
			num++;
			item_MB.Health = ParseCsvFloat(GetCsvCell(array2, num));
			num++;
			item_MB.Mana = ParseCsvFloat(GetCsvCell(array2, num));
			num++;
			item_MB.Element = ParseCsvFloat(GetCsvCell(array2, num));
			num++;
			item_MB.Main = ReadWeaponDataA(array2, ref num, 5);
			item_MB.DOT = ReadWeaponDataA(array2, ref num, 2);
			item_MB.SK = ReadWeaponDataB(array2, ref num, 1);
			item_MB.CP = ReadWeaponDataB(array2, ref num, 1);
			item_MB.RateMain = ReadWeaponRateA(array2, ref num, WP_Main);
			item_MB.RateDot = ReadWeaponRateA(array2, ref num, WP_DOT);
			int id = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			item_MB.RateSK = GetWeaponRateB(id, WP_SK);
			item_MB.RateCP = new WPDT_B[0];
			item_MB.WP_SkillCount = 0;
			ReadWeaponSkill(array2, ref num, out item_MB.SkillA, out item_MB.SkillA_count, ref item_MB.WP_SkillCount);
			ReadWeaponSkill(array2, ref num, out item_MB.SkillB, out item_MB.SkillB_count, ref item_MB.WP_SkillCount);
			ReadWeaponSkill(array2, ref num, out item_MB.SkillC, out item_MB.SkillC_count, ref item_MB.WP_SkillCount);
			ReadWeaponSkill(array2, ref num, out item_MB.SkillD, out item_MB.SkillD_count, ref item_MB.WP_SkillCount);
			ReadWeaponSkill(array2, ref num, out item_MB.SkillE, out item_MB.SkillE_count, ref item_MB.WP_SkillCount);
			ReadWeaponSkill(array2, ref num, out item_MB.SkillF, out item_MB.SkillF_count, ref item_MB.WP_SkillCount);
			item_MB.SPC.Clear();
			ReadWeaponSPC(array2, ref num, item_MB.SPC);
			ReadWeaponSPC(array2, ref num, item_MB.SPC);
			ReadWeaponSPC(array2, ref num, item_MB.SPC);
			item_MB.Set_Index = ParseCsvInt(GetCsvCell(array2, num));
			num++;
			switch (item_MB.Quality)
			{
			case 0:
				if (WeaponPlayerType.IsGeneric(item_MB.PLtype))
				{
					Weapon.GP[0].QL[item_MB.CharType].Normal.Add(item_MB);
					Weapon.GP[1].QL[item_MB.CharType].Normal.Add(item_MB);
					Weapon.GP[2].QL[item_MB.CharType].Normal.Add(item_MB);
					Weapon.GP[3].QL[item_MB.CharType].Normal.Add(item_MB);
				}
				else
				{
					Weapon.GP[item_MB.PLtype].QL[item_MB.CharType].Normal.Add(item_MB);
				}
				break;
			case 1:
				if (WeaponPlayerType.IsGeneric(item_MB.PLtype))
				{
					Weapon.GP[0].QL[item_MB.CharType].Magic.Add(item_MB);
					Weapon.GP[1].QL[item_MB.CharType].Magic.Add(item_MB);
					Weapon.GP[2].QL[item_MB.CharType].Magic.Add(item_MB);
					Weapon.GP[3].QL[item_MB.CharType].Magic.Add(item_MB);
				}
				else
				{
					Weapon.GP[item_MB.PLtype].QL[item_MB.CharType].Magic.Add(item_MB);
				}
				break;
			case 2:
				if (WeaponPlayerType.IsGeneric(item_MB.PLtype))
				{
					Weapon.GP[0].QL[item_MB.CharType].Rare.Add(item_MB);
					Weapon.GP[1].QL[item_MB.CharType].Rare.Add(item_MB);
					Weapon.GP[2].QL[item_MB.CharType].Rare.Add(item_MB);
					Weapon.GP[3].QL[item_MB.CharType].Rare.Add(item_MB);
				}
				else
				{
					Weapon.GP[item_MB.PLtype].QL[item_MB.CharType].Rare.Add(item_MB);
				}
				break;
			case 3:
				if (WeaponPlayerType.IsGeneric(item_MB.PLtype))
				{
					Weapon.GP[0].QL[item_MB.CharType].Exquisite.Add(item_MB);
					Weapon.GP[1].QL[item_MB.CharType].Exquisite.Add(item_MB);
					Weapon.GP[2].QL[item_MB.CharType].Exquisite.Add(item_MB);
					Weapon.GP[3].QL[item_MB.CharType].Exquisite.Add(item_MB);
				}
				else
				{
					Weapon.GP[item_MB.PLtype].QL[item_MB.CharType].Exquisite.Add(item_MB);
				}
				break;
			case 4:
				if (WeaponPlayerType.IsGeneric(item_MB.PLtype))
				{
					Weapon.GP[0].QL[item_MB.CharType].Epic.Add(item_MB);
					Weapon.GP[1].QL[item_MB.CharType].Epic.Add(item_MB);
					Weapon.GP[2].QL[item_MB.CharType].Epic.Add(item_MB);
					Weapon.GP[3].QL[item_MB.CharType].Epic.Add(item_MB);
				}
				else
				{
					Weapon.GP[item_MB.PLtype].QL[item_MB.CharType].Epic.Add(item_MB);
				}
				break;
			case 5:
				if (WeaponPlayerType.IsGeneric(item_MB.PLtype))
				{
					Weapon.GP[0].QL[item_MB.CharType].Legendary.Add(item_MB);
					Weapon.GP[1].QL[item_MB.CharType].Legendary.Add(item_MB);
					Weapon.GP[2].QL[item_MB.CharType].Legendary.Add(item_MB);
					Weapon.GP[3].QL[item_MB.CharType].Legendary.Add(item_MB);
				}
				else
				{
					Weapon.GP[item_MB.PLtype].QL[item_MB.CharType].Legendary.Add(item_MB);
				}
				break;
			case 6:
				if (WeaponPlayerType.IsGeneric(item_MB.PLtype))
				{
					Weapon.GP[0].QL[item_MB.CharType].Mythical.Add(item_MB);
					Weapon.GP[1].QL[item_MB.CharType].Mythical.Add(item_MB);
					Weapon.GP[2].QL[item_MB.CharType].Mythical.Add(item_MB);
					Weapon.GP[3].QL[item_MB.CharType].Mythical.Add(item_MB);
				}
				else
				{
					Weapon.GP[item_MB.PLtype].QL[item_MB.CharType].Mythical.Add(item_MB);
				}
				break;
			}
		}
		PoeItemMod.TryRegisterWeaponRows(this);
	}

	private static WPDT_A[] ReadWeaponDataA(string[] row, ref int s, int groupCount)
	{
		List<WPDT_A> list = new List<WPDT_A>(groupCount);
		for (int i = 0; i < groupCount; i++)
		{
			int num = ParseCsvInt(GetCsvCell(row, s++));
			int eL = ParseCsvInt(GetCsvCell(row, s++));
			float number = ParseCsvFloat(GetCsvCell(row, s++));
			if (num != 0)
			{
				list.Add(new WPDT_A
				{
					Index = num,
					EL = eL,
					number = number
				});
			}
		}
		return list.ToArray();
	}

	private static WPDT_B[] ReadWeaponDataB(string[] row, ref int s, int groupCount)
	{
		List<WPDT_B> list = new List<WPDT_B>(groupCount);
		for (int i = 0; i < groupCount; i++)
		{
			s++;
			string text = NormalizeSkillCsvText(GetCsvCell(row, s++));
			int index = ParseCsvInt(GetCsvCell(row, s++));
			int globleID = ParseCsvInt(GetCsvCell(row, s++));
			int eL = ParseCsvInt(GetCsvCell(row, s++));
			float number = ParseCsvFloat(GetCsvCell(row, s++));
			s++;
			string linkSK = NormalizeSkillCsvText(GetCsvCell(row, s++));
			if (!IsCsvNoneText(text))
			{
				list.Add(new WPDT_B
				{
					SkillName = text,
					Index = index,
					GlobleID = globleID,
					EL = eL,
					number = number,
					LinkSK = linkSK
				});
			}
		}
		return list.ToArray();
	}

	private static WPDT_A[] ReadWeaponRateA(string[] row, ref int s, Dictionary<int, WPDT_RandomA> source)
	{
		int num = ParseCsvInt(GetCsvCell(row, s++));
		if (num == 0 || source == null || !source.TryGetValue(num, out var value) || value == null)
		{
			return new WPDT_A[0];
		}
		return CopyWeaponDataA(value.RD);
	}

	private static WPDT_B[] GetWeaponRateB(int id, Dictionary<int, WPDT_RandomB> source)
	{
		if (id == 0 || source == null || !source.TryGetValue(id, out var value) || value == null)
		{
			return new WPDT_B[0];
		}
		return CopyWeaponDataB(value.RD);
	}

	private static WPDT_A[] CopyWeaponDataA(WPDT_A[] source)
	{
		if (source == null || source.Length == 0)
		{
			return new WPDT_A[0];
		}
		List<WPDT_A> list = new List<WPDT_A>(source.Length);
		foreach (WPDT_A wPDT_A in source)
		{
			if (wPDT_A != null && wPDT_A.Index != 0)
			{
				list.Add(new WPDT_A
				{
					Index = wPDT_A.Index,
					EL = wPDT_A.EL,
					number = wPDT_A.number
				});
			}
		}
		return list.ToArray();
	}

	private static WPDT_B[] CopyWeaponDataB(WPDT_B[] source)
	{
		if (source == null || source.Length == 0)
		{
			return new WPDT_B[0];
		}
		List<WPDT_B> list = new List<WPDT_B>(source.Length);
		foreach (WPDT_B wPDT_B in source)
		{
			if (wPDT_B != null && !IsCsvNoneText(wPDT_B.SkillName))
			{
				list.Add(new WPDT_B
				{
					SkillName = wPDT_B.SkillName,
					Index = wPDT_B.Index,
					GlobleID = wPDT_B.GlobleID,
					EL = wPDT_B.EL,
					number = wPDT_B.number,
					LinkSK = wPDT_B.LinkSK
				});
			}
		}
		return list.ToArray();
	}

	private static void ReadWeaponSkill(string[] row, ref int s, out string skillName, out int count, ref int skillCount)
	{
		s++;
		skillName = GetCsvCell(row, s++);
		count = ParseCsvInt(GetCsvCell(row, s++));
		if (count > 0)
		{
			skillCount++;
		}
	}

	private static void ReadWeaponSPC(string[] row, ref int s, List<WPSPC> target)
	{
		s++;
		int num = ParseCsvInt(GetCsvCell(row, s++));
		if (num != 0)
		{
			target.Add(new WPSPC
			{
				Index = num
			});
		}
	}

	private static string NormalizeSkillCsvText(string value)
	{
		if (!IsCsvNoneText(value))
		{
			return value.Trim();
		}
		return string.Empty;
	}

	private static bool IsCsvNoneText(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return true;
		}
		value = value.Trim();
		if (!(value == "0") && !value.Equals("none", StringComparison.OrdinalIgnoreCase))
		{
			return value == "无";
		}
		return true;
	}

	private void LoadData_SPC(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SPC_MB sPC_MB = new SPC_MB();
			int num = 1;
			sPC_MB.SPCindex = int.Parse(array[i][num]);
			num++;
			sPC_MB.SPCtype = int.Parse(array[i][num]);
			num++;
			sPC_MB.FWtype = int.Parse(array[i][num]);
			num++;
			sPC_MB.SPCname = array[i][num];
			num++;
			sPC_MB.FStype = int.Parse(array[i][num]);
			num++;
			sPC_MB.LockType = int.Parse(array[i][num]);
			num++;
			sPC_MB.info = array[i][num];
			num++;
			sPC_MB.Price = int.Parse(array[i][num]);
			num++;
			num++;
			sPC_MB.SkillName = array[i][num];
			num++;
			num++;
			sPC_MB.ZQName = array[i][num];
			num++;
			sPC_MB.RTtypeOBJ = int.Parse(array[i][num]);
			num++;
			sPC_MB.Distance = int.Parse(array[i][num]);
			num++;
			sPC_MB.Rate = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Damage = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.DamageA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.DamageB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.ThroughType = int.Parse(array[i][num]);
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				sPC_MB.AttackType = true;
			}
			else
			{
				sPC_MB.AttackType = false;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				sPC_MB.AttackTypeA = true;
			}
			else
			{
				sPC_MB.AttackTypeA = false;
			}
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				sPC_MB.AttackTypeB = true;
			}
			else
			{
				sPC_MB.AttackTypeB = false;
			}
			num++;
			sPC_MB.NoTime = int.Parse(array[i][num]);
			num++;
			sPC_MB.BuffTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.DebuffTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Field_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.ORB_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.EXP_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.ZD_time_F = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.ZD_time_S = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.OBJ = int.Parse(array[i][num]);
			num++;
			sPC_MB.Layer_SubA = int.Parse(array[i][num]);
			num++;
			sPC_MB.Layer_SubB = int.Parse(array[i][num]);
			num++;
			sPC_MB.ORB = int.Parse(array[i][num]);
			num++;
			sPC_MB.ZD_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.ZD_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.ZD_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.EXP_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.EXP_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.EXP_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.Dic_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.Dic_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.FX_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.FX_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.Sound = int.Parse(array[i][num]);
			num++;
			sPC_MB.Count_ORB = int.Parse(array[i][num]);
			num++;
			sPC_MB.Count_ATtarget = int.Parse(array[i][num]);
			num++;
			sPC_MB.Count_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.Count_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.Count_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.CountMulti = int.Parse(array[i][num]);
			num++;
			sPC_MB.CountEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.TypeORB = int.Parse(array[i][num]);
			num++;
			sPC_MB.Type_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.Type_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.Type_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.TypeDIC_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.TypeDIC_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.TypeEXP_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.TypeEXP_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.TypeEXP_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.Size = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.High = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.JG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.AngleA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.AngleB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Range1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Range2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Range_AT = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.FStime1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.FStime2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Speed1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Speed2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Speed3 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Speed4 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			sPC_MB.Follow_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.Follow_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.AllChuan_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.AllChuan_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.Slow_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.Slow_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.RDSpeed_F = int.Parse(array[i][num]);
			num++;
			sPC_MB.RDSpeed_S = int.Parse(array[i][num]);
			num++;
			sPC_MB.HasFX = int.Parse(array[i][num]);
			num++;
			sPC_MB.S_HasFX = int.Parse(array[i][num]);
			num++;
			sPC_MB.AB_HasFX = int.Parse(array[i][num]);
			num++;
			sPC_MB.colEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.colEXP_A = int.Parse(array[i][num]);
			num++;
			sPC_MB.S_colEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.AB_colEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.TimeEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.TimeEXP_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.LastEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.LastEXP_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.S_LastEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.AB_LastEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.EXPpos = int.Parse(array[i][num]);
			num++;
			sPC_MB.EXPpos_AB = int.Parse(array[i][num]);
			num++;
			sPC_MB.S_EXPpos = int.Parse(array[i][num]);
			num++;
			sPC_MB.AB_EXPpos = int.Parse(array[i][num]);
			num++;
			sPC_MB.AngleEXP = int.Parse(array[i][num]);
			num++;
			sPC_MB.AngleEXP_AB = int.Parse(array[i][num]);
			RegisterWeaponSPCTemplate(sPC_MB);
		}
	}

	private void LoadData_BS(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			BaoshiClass baoshiClass = new BaoshiClass();
			int num = 1;
			baoshiClass.GlobalID = int.Parse(array[i][num]);
			num++;
			baoshiClass.ItemType = 1;
			baoshiClass.ItemName = array[i][num];
			num++;
			baoshiClass.priceQulity = int.Parse(array[i][num]);
			baoshiClass.Price = BaoshiPrice.Price[int.Parse(array[i][num])];
			num++;
			baoshiClass.Quality = int.Parse(array[i][num]);
			num++;
			baoshiClass.Size.x = 1;
			baoshiClass.Size.y = 1;
			baoshiClass.Icon = IconBaoshi.icon[int.Parse(array[i][num])];
			num++;
			baoshiClass.Level = int.Parse(array[i][num]);
			num++;
			baoshiClass.UseType = int.Parse(array[i][num]);
			num++;
			baoshiClass.BS_Quality = int.Parse(array[i][num]);
			num++;
			baoshiClass.SoundDrop = int.Parse(array[i][num]);
			num++;
			baoshiClass.SoundUse = int.Parse(array[i][num]);
			num++;
			baoshiClass.RotateType = int.Parse(array[i][num]);
			num++;
			baoshiClass.BStype = array[i][num];
			num++;
			baoshiClass.Number = int.Parse(array[i][num]);
			num++;
			baoshiClass.MstackSize = int.Parse(array[i][num]);
			num++;
			baoshiClass.CstackSize = int.Parse(array[i][num]);
			num++;
			baoshiClass.DropSpriteSize = int.Parse(array[i][num]);
			num++;
			baoshiClass.FWtype = int.Parse(array[i][num]);
			num++;
			baoshiClass.DropScene = int.Parse(array[i][num]);
			switch (baoshiClass.UseType)
			{
			case 0:
				Baoshi.Add(baoshiClass);
				break;
			case 1:
				BaoshiJH.Add(baoshiClass);
				break;
			case 2:
				BaoshiSPC.Add(baoshiClass);
				break;
			case 3:
				SkillFW = baoshiClass;
				break;
			case 4:
				SPCFW = baoshiClass;
				break;
			case 5:
				BaseFW.Add(baoshiClass);
				break;
			}
		}
		BuildBaoshiIconMap();
	}

	private void LoadData_USE(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			UseItemClass useItemClass = new UseItemClass();
			int num = 1;
			useItemClass.GlobalID = int.Parse(array[i][num]);
			num++;
			useItemClass.ItemType = 2;
			useItemClass.ItemName = array[i][num];
			num++;
			useItemClass.Price = int.Parse(array[i][num]);
			num++;
			useItemClass.Quality = int.Parse(array[i][num]);
			num++;
			useItemClass.Size.x = 1;
			useItemClass.Size.y = 1;
			useItemClass.Icon = IconUse.icon[int.Parse(array[i][num])];
			num++;
			useItemClass.Level = int.Parse(array[i][num]);
			num++;
			useItemClass.SoundDrop = int.Parse(array[i][num]);
			num++;
			useItemClass.SoundUse = int.Parse(array[i][num]);
			num++;
			useItemClass.RotateType = int.Parse(array[i][num]);
			num++;
			useItemClass.InfoType = int.Parse(array[i][num]);
			num++;
			useItemClass.UseType = array[i][num];
			num++;
			useItemClass.damageType = TalentManager.GiveElement(int.Parse(array[i][num]));
			num++;
			useItemClass.Number = int.Parse(array[i][num]);
			num++;
			useItemClass.CDTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			useItemClass.Duration = int.Parse(array[i][num]);
			num++;
			useItemClass.MstackSize = int.Parse(array[i][num]);
			num++;
			useItemClass.CstackSize = int.Parse(array[i][num]);
			num++;
			useItemClass.DropSpriteSize = int.Parse(array[i][num]);
			num++;
			useItemClass.DropScene = int.Parse(array[i][num]);
			switch (useItemClass.InfoType)
			{
			case 0:
				Potion.Add(useItemClass);
				break;
			case 1:
				BuffPotion.Add(useItemClass);
				break;
			case 2:
				Scroll.Add(useItemClass.ItemName, useItemClass);
				break;
			case 3:
			case 4:
				PremPotion.Add(useItemClass);
				break;
			case 5:
				SpcPotion.Add(useItemClass.ItemName, useItemClass);
				break;
			case 6:
				SpcItem.Add(useItemClass.ItemName, useItemClass);
				break;
			case 7:
				SpcItem.Add(useItemClass.ItemName, useItemClass);
				break;
			}
		}
	}

	private void LoadData_SET(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		if (array == null)
		{
			return;
		}
		for (int i = 1; i < array.Length - 1; i++)
		{
			Set_DT set_DT = new Set_DT();
			int num = 1;
			set_DT.SetID = int.Parse(array[i][num]);
			num++;
			set_DT.SetName = array[i][num];
			num++;
			set_DT.Lit = new Set_DT_Lit[3];
			for (int j = 0; j < set_DT.Lit.Length; j++)
			{
				Set_DT_Lit set_DT_Lit = new Set_DT_Lit();
				set_DT_Lit.MainTP = int.Parse(array[i][num]);
				num++;
				num++;
				set_DT_Lit.SkillName = array[i][num];
				num++;
				set_DT_Lit.Index = int.Parse(array[i][num]);
				num++;
				set_DT_Lit.GlobleID = int.Parse(array[i][num]);
				num++;
				set_DT_Lit.EL = int.Parse(array[i][num]);
				num++;
				set_DT_Lit.Number = int.Parse(array[i][num]);
				num++;
				num++;
				set_DT_Lit.LinkSK = array[i][num];
				num++;
				set_DT.Lit[j] = set_DT_Lit;
			}
			num++;
			set_DT.BuffName = array[i][num];
			num++;
			if (array[i].Length >= 33)
			{
				set_DT.BuffType = (int.TryParse(array[i][num], out var result) ? result : 0);
				num++;
			}
			else
			{
				set_DT.BuffType = 0;
			}
			set_DT.BuffTime = int.Parse(array[i][num]);
			num++;
			set_DT.LayerMax = int.Parse(array[i][num]);
			num++;
			set_DT.TP_Layer = int.Parse(array[i][num]);
			num++;
			set_DT.NumberL = int.Parse(array[i][num]);
			num++;
			set_DT.TP_Max = int.Parse(array[i][num]);
			num++;
			set_DT.NumberM = int.Parse(array[i][num]);
			SET.Add(set_DT.SetID, set_DT);
		}
	}

	private void LoadData_RandomA(TextAsset csvFile, Dictionary<int, WPDT_RandomA> target)
	{
		if (target == null)
		{
			return;
		}
		target.Clear();
		string[][] array = LoadTextFile(csvFile);
		if (array == null || array.Length <= 1 || array[0] == null)
		{
			return;
		}
		int a = Mathf.Max(0, (array[0].Length - 2) / 3);
		for (int i = 1; i < array.Length - 1; i++)
		{
			string[] array2 = array[i];
			if (array2 == null || array2.Length <= 1)
			{
				continue;
			}
			int key = ParseCsvInt(GetCsvCell(array2, 1));
			int b = Mathf.Max(0, (array2.Length - 2) / 3);
			int num = Mathf.Min(a, b);
			List<WPDT_A> list = new List<WPDT_A>(num);
			int num2 = 2;
			for (int j = 0; j < num; j++)
			{
				int num3 = ParseCsvInt(GetCsvCell(array2, num2++));
				int eL = ParseCsvInt(GetCsvCell(array2, num2++));
				float number = ParseCsvFloat(GetCsvCell(array2, num2++));
				if (num3 != 0)
				{
					list.Add(new WPDT_A
					{
						Index = num3,
						EL = eL,
						number = number
					});
				}
			}
			target[key] = new WPDT_RandomA
			{
				RD = list.ToArray()
			};
		}
	}

	private void LoadData_RandomMergedSkillB(TextAsset csvFile)
	{
		WP_SK.Clear();
		WP_CP.Clear();
		string[][] array = LoadTextFile(csvFile);
		if (array == null || array.Length <= 1 || array[0] == null)
		{
			return;
		}
		int a = Mathf.Max(0, (array[0].Length - 2) / 8);
		for (int i = 1; i < array.Length - 1; i++)
		{
			string[] array2 = array[i];
			if (array2 == null || array2.Length <= 1)
			{
				continue;
			}
			int key = ParseCsvInt(GetCsvCell(array2, 1));
			int b = Mathf.Max(0, (array2.Length - 2) / 8);
			int num = Mathf.Min(a, b);
			List<WPDT_B> list = new List<WPDT_B>(num);
			int num2 = 2;
			for (int j = 0; j < num; j++)
			{
				num2++;
				string text = NormalizeSkillCsvText(GetCsvCell(array2, num2++));
				int index = ParseCsvInt(GetCsvCell(array2, num2++));
				int globleID = ParseCsvInt(GetCsvCell(array2, num2++));
				int eL = ParseCsvInt(GetCsvCell(array2, num2++));
				float number = ParseCsvFloat(GetCsvCell(array2, num2++));
				num2++;
				string linkSK = NormalizeSkillCsvText(GetCsvCell(array2, num2++));
				if (!IsCsvNoneText(text))
				{
					WPDT_B item = new WPDT_B
					{
						SkillName = text,
						Index = index,
						GlobleID = globleID,
						EL = eL,
						number = number,
						LinkSK = linkSK
					};
					int mergedRandomSkillType = GetMergedRandomSkillType(text);
					if (mergedRandomSkillType == 0 || mergedRandomSkillType == 2)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				WP_SK[key] = new WPDT_RandomB
				{
					RD = list.ToArray()
				};
			}
		}
	}

	private int GetMergedRandomSkillType(string skillName)
	{
		if (IsCsvNoneText(skillName))
		{
			return -1;
		}
		TalentManager talentManager = (TL ? TL : (SingletonMonoScope<TalentManager>.HasInstance ? SingletonMonoScope<TalentManager>.Instance : null));
		if ((bool)talentManager)
		{
			talentManager.EnsureTalentTablesLoaded();
			if (talentManager.SKI != null && talentManager.SKI.TryGetValue(skillName, out var value))
			{
				if (value.type != 0 && value.type != 2)
				{
					return -1;
				}
				return value.type;
			}
		}
		return -1;
	}

	private void LoadData_Skill(TextAsset csvFile)
	{
		csvFile = ResolveSkillTextAsset(csvFile);
		InitSPCMBLibrary();
		SPC_Rune.Clear();
		if (csvFile == null)
		{
			return;
		}
		string[][] array = LoadTextFile(csvFile);
		if (array == null)
		{
			return;
		}
		for (int i = 1; i < array.Length - 1; i++)
		{
			if (array[i] != null && array[i].Length > 1 && !string.IsNullOrWhiteSpace(array[i][1]))
			{
				SPC_MB mb = CreateSPCMBFromGridRow(array[i]);
				RegisterSPCRuneTemplate(mb);
				AddSPCMBToAll(mb);
				AddSPCMBByTalentFW(mb);
			}
		}
	}

	private TextAsset ResolveSkillTextAsset(TextAsset csvFile)
	{
		if ((bool)csvFile)
		{
			return csvFile;
		}
		return null;
	}

	private void InitSPCMBLibrary()
	{
		SPCMB.MB = new SPC_MB[0];
		SPCMB.PL = new SPCMB_Player[4];
		for (int i = 0; i < SPCMB.PL.Length; i++)
		{
			SPCMB.PL[i] = CreateSPCMBPlayer();
		}
	}

	private static SPCMB_Player CreateSPCMBPlayer()
	{
		return new SPCMB_Player
		{
			TP = CreateSPCMBTypes()
		};
	}

	private static SPCMB_Type[] CreateSPCMBTypes()
	{
		SPCMB_Type[] array = new SPCMB_Type[5];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new SPCMB_Type
			{
				MB = new SPC_MB[0]
			};
		}
		return array;
	}

	private static void EnsureSPCMBTypes(SPCMB_Player player)
	{
		if (player == null)
		{
			return;
		}
		if (player.TP == null)
		{
			player.TP = new SPCMB_Type[5];
		}
		if (player.TP.Length < 5)
		{
			Array.Resize(ref player.TP, 5);
		}
		for (int i = 0; i < player.TP.Length; i++)
		{
			if (player.TP[i] == null)
			{
				player.TP[i] = new SPCMB_Type
				{
					MB = new SPC_MB[0]
				};
			}
			else if (player.TP[i].MB == null)
			{
				player.TP[i].MB = new SPC_MB[0];
			}
		}
	}

	private static int NormalizeSPCFWType(int fwType)
	{
		return Mathf.Clamp(fwType, 0, 4);
	}

	private void AddSPCMBByTalentFW(SPC_MB mb)
	{
		if (mb == null || mb.SPCindex <= 0 || mb.SPCtype <= 0)
		{
			return;
		}
		string skillName = mb.SkillName;
		if (IsCommonSPCSkillName(skillName))
		{
			for (int i = 0; i < SPCMB.PL.Length; i++)
			{
				AddSPCMBToPlayer(i, mb);
			}
			return;
		}
		TalentManager talentManager = (TL ? TL : (SingletonMonoScope<TalentManager>.HasInstance ? SingletonMonoScope<TalentManager>.Instance : null));
		if (talentManager != null && talentManager.TryGetSkillFWPlayerType(skillName, out var plType))
		{
			AddSPCMBToPlayer(plType, mb);
		}
	}

	private void AddSPCMBToAll(SPC_MB mb)
	{
		if (mb == null || mb.SPCindex <= 0 || mb.SPCtype <= 0)
		{
			return;
		}
		SPC_MB[] array = SPCMB.MB;
		if (array == null)
		{
			array = new SPC_MB[0];
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].SPCindex == mb.SPCindex)
			{
				return;
			}
		}
		Array.Resize(ref array, array.Length + 1);
		array[array.Length - 1] = mb;
		SPCMB.MB = array;
	}

	private void RegisterWeaponSPCTemplate(SPC_MB mb)
	{
		if (mb != null && mb.SPCindex > 0)
		{
			SPC[mb.SPCindex] = mb;
		}
	}

	private void RegisterSPCRuneTemplate(SPC_MB mb)
	{
		if (mb != null && mb.SPCindex > 0)
		{
			SPC_Rune[mb.SPCindex] = mb;
		}
	}

	public bool TryGetWeaponSPCMBByIndex(int spcIndex, out SPC_MB mb)
	{
		mb = null;
		if (spcIndex > 0 && SPC.TryGetValue(spcIndex, out mb))
		{
			return mb != null;
		}
		return false;
	}

	private bool TryGetSPCMBByIndexFromAll(int spcIndex, out SPC_MB mb)
	{
		mb = null;
		SPC_MB[] mB = SPCMB.MB;
		if (mB == null)
		{
			return false;
		}
		for (int i = 0; i < mB.Length; i++)
		{
			if (mB[i] != null && mB[i].SPCindex == spcIndex)
			{
				mb = mB[i];
				return true;
			}
		}
		return false;
	}

	private void AddSPCMBToPlayer(int plType, SPC_MB mb)
	{
		if (plType < 0 || SPCMB.PL == null || plType >= SPCMB.PL.Length || mb == null)
		{
			return;
		}
		if (SPCMB.PL[plType] == null)
		{
			SPCMB.PL[plType] = CreateSPCMBPlayer();
		}
		SPCMB_Player sPCMB_Player = SPCMB.PL[plType];
		EnsureSPCMBTypes(sPCMB_Player);
		int num = NormalizeSPCFWType(mb.FWtype);
		SPC_MB[] array = sPCMB_Player.TP[num].MB;
		if (array == null)
		{
			array = new SPC_MB[0];
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].SPCindex == mb.SPCindex)
			{
				return;
			}
		}
		Array.Resize(ref array, array.Length + 1);
		array[array.Length - 1] = mb;
		sPCMB_Player.TP[num].MB = array;
	}

	public bool TryGetSPCMBByIndex(int spcIndex, out SPC_MB mb)
	{
		mb = null;
		if (spcIndex <= 0)
		{
			return false;
		}
		if (SPC_Rune.TryGetValue(spcIndex, out mb) && mb != null)
		{
			return true;
		}
		if (SPCMB.PL != null && SPCMB.PL.Length != 0)
		{
			int plType = ((PL != null) ? Mathf.Clamp(PL.PLType, 0, SPCMB.PL.Length - 1) : 0);
			if (TryGetSPCMBByIndexFromPlayer(plType, spcIndex, out mb))
			{
				RegisterSPCRuneTemplate(mb);
				return true;
			}
		}
		if (TryGetSPCMBByIndexFromAll(spcIndex, out mb))
		{
			RegisterSPCRuneTemplate(mb);
			return true;
		}
		return false;
	}

	private bool TryGetSPCMBByIndexFromPlayer(int plType, int spcIndex, out SPC_MB mb)
	{
		mb = null;
		if (plType < 0 || SPCMB.PL == null || plType >= SPCMB.PL.Length)
		{
			return false;
		}
		SPCMB_Type[] array = SPCMB.PL[plType]?.TP;
		if (array == null)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			SPC_MB[] array2 = array[i]?.MB;
			if (array2 == null)
			{
				continue;
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] != null && array2[j].SPCindex == spcIndex)
				{
					mb = array2[j];
					return true;
				}
			}
		}
		return false;
	}

	private static bool IsCommonSPCSkillName(string spcName)
	{
		if (string.IsNullOrWhiteSpace(spcName))
		{
			return true;
		}
		string text = spcName.Trim();
		if (!(text == "0") && !text.Equals("none", StringComparison.OrdinalIgnoreCase) && !(text == "无"))
		{
			return text == "無";
		}
		return true;
	}

	private static SPC_MB CreateSPCMBFromGridRow(string[] row)
	{
		SPC_MB sPC_MB = new SPC_MB();
		int num = 1;
		sPC_MB.SPCindex = int.Parse(row[num]);
		num++;
		sPC_MB.SPCtype = int.Parse(row[num]);
		num++;
		sPC_MB.FWtype = int.Parse(row[num]);
		num++;
		sPC_MB.SPCname = row[num];
		num++;
		sPC_MB.FStype = int.Parse(row[num]);
		num++;
		sPC_MB.LockType = int.Parse(row[num]);
		num++;
		sPC_MB.info = row[num];
		num++;
		sPC_MB.Price = int.Parse(row[num]);
		num++;
		num++;
		sPC_MB.SkillName = row[num];
		num++;
		num++;
		sPC_MB.ZQName = row[num];
		num++;
		sPC_MB.RTtypeOBJ = int.Parse(row[num]);
		num++;
		sPC_MB.Distance = int.Parse(row[num]);
		num++;
		sPC_MB.Rate = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Damage = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.DamageA = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.DamageB = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.ThroughType = int.Parse(row[num]);
		num++;
		sPC_MB.AttackType = int.Parse(row[num]) == 0;
		num++;
		sPC_MB.AttackTypeA = int.Parse(row[num]) == 0;
		num++;
		sPC_MB.AttackTypeB = int.Parse(row[num]) == 0;
		num++;
		sPC_MB.NoTime = int.Parse(row[num]);
		num++;
		sPC_MB.BuffTime = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.DebuffTime = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Field_time = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.ORB_time = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.EXP_time = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.ZD_time_F = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.ZD_time_S = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.OBJ = int.Parse(row[num]);
		num++;
		sPC_MB.Layer_SubA = int.Parse(row[num]);
		num++;
		sPC_MB.Layer_SubB = int.Parse(row[num]);
		num++;
		sPC_MB.ORB = int.Parse(row[num]);
		num++;
		sPC_MB.ZD_F = int.Parse(row[num]);
		num++;
		sPC_MB.ZD_S = int.Parse(row[num]);
		num++;
		sPC_MB.ZD_AB = int.Parse(row[num]);
		num++;
		sPC_MB.EXP_F = int.Parse(row[num]);
		num++;
		sPC_MB.EXP_S = int.Parse(row[num]);
		num++;
		sPC_MB.EXP_AB = int.Parse(row[num]);
		num++;
		sPC_MB.Dic_F = int.Parse(row[num]);
		num++;
		sPC_MB.Dic_S = int.Parse(row[num]);
		num++;
		sPC_MB.FX_F = int.Parse(row[num]);
		num++;
		sPC_MB.FX_S = int.Parse(row[num]);
		num++;
		sPC_MB.Sound = int.Parse(row[num]);
		num++;
		sPC_MB.Count_ORB = int.Parse(row[num]);
		num++;
		sPC_MB.Count_ATtarget = int.Parse(row[num]);
		num++;
		sPC_MB.Count_F = int.Parse(row[num]);
		num++;
		sPC_MB.Count_S = int.Parse(row[num]);
		num++;
		sPC_MB.Count_AB = int.Parse(row[num]);
		num++;
		sPC_MB.CountMulti = int.Parse(row[num]);
		num++;
		sPC_MB.CountEXP = int.Parse(row[num]);
		num++;
		sPC_MB.TypeORB = int.Parse(row[num]);
		num++;
		sPC_MB.Type_F = int.Parse(row[num]);
		num++;
		sPC_MB.Type_S = int.Parse(row[num]);
		num++;
		sPC_MB.Type_AB = int.Parse(row[num]);
		num++;
		sPC_MB.TypeDIC_F = int.Parse(row[num]);
		num++;
		sPC_MB.TypeDIC_S = int.Parse(row[num]);
		num++;
		sPC_MB.TypeEXP_F = int.Parse(row[num]);
		num++;
		sPC_MB.TypeEXP_S = int.Parse(row[num]);
		num++;
		sPC_MB.TypeEXP_AB = int.Parse(row[num]);
		num++;
		sPC_MB.Size = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.High = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.JG = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.AngleA = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.AngleB = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Range1 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Range2 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Range_AT = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.FStime1 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.FStime2 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Speed1 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Speed2 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Speed3 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Speed4 = float.Parse(row[num], CultureInfo.InvariantCulture);
		num++;
		sPC_MB.Follow_F = int.Parse(row[num]);
		num++;
		sPC_MB.Follow_S = int.Parse(row[num]);
		num++;
		sPC_MB.AllChuan_F = int.Parse(row[num]);
		num++;
		sPC_MB.AllChuan_S = int.Parse(row[num]);
		num++;
		sPC_MB.Slow_F = int.Parse(row[num]);
		num++;
		sPC_MB.Slow_S = int.Parse(row[num]);
		num++;
		sPC_MB.RDSpeed_F = int.Parse(row[num]);
		num++;
		sPC_MB.RDSpeed_S = int.Parse(row[num]);
		num++;
		sPC_MB.HasFX = int.Parse(row[num]);
		num++;
		sPC_MB.S_HasFX = int.Parse(row[num]);
		num++;
		sPC_MB.AB_HasFX = int.Parse(row[num]);
		num++;
		sPC_MB.colEXP = int.Parse(row[num]);
		num++;
		sPC_MB.colEXP_A = int.Parse(row[num]);
		num++;
		sPC_MB.S_colEXP = int.Parse(row[num]);
		num++;
		sPC_MB.AB_colEXP = int.Parse(row[num]);
		num++;
		sPC_MB.TimeEXP = int.Parse(row[num]);
		num++;
		sPC_MB.TimeEXP_AB = int.Parse(row[num]);
		num++;
		sPC_MB.LastEXP = int.Parse(row[num]);
		num++;
		sPC_MB.LastEXP_AB = int.Parse(row[num]);
		num++;
		sPC_MB.S_LastEXP = int.Parse(row[num]);
		num++;
		sPC_MB.AB_LastEXP = int.Parse(row[num]);
		num++;
		sPC_MB.EXPpos = int.Parse(row[num]);
		num++;
		sPC_MB.EXPpos_AB = int.Parse(row[num]);
		num++;
		sPC_MB.S_EXPpos = int.Parse(row[num]);
		num++;
		sPC_MB.AB_EXPpos = int.Parse(row[num]);
		num++;
		sPC_MB.AngleEXP = int.Parse(row[num]);
		num++;
		sPC_MB.AngleEXP_AB = int.Parse(row[num]);
		return sPC_MB;
	}

	public static string[][] LoadTextFile(TextAsset textFile)
	{
		if ((bool)textFile)
		{
			string[] array = textFile.text.Split('\n');
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Split(',');
			}
			return array2.ToArray();
		}
		return null;
	}

	private static string GetCsvCell(string[] row, int index)
	{
		if (row == null || index < 0 || index >= row.Length)
		{
			return string.Empty;
		}
		return row[index]?.Trim() ?? string.Empty;
	}

	private static string NormalizeCsvText(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return string.Empty;
		}
		value = value.Trim();
		if (!(value == "0"))
		{
			return value;
		}
		return string.Empty;
	}

	private static int ParseCsvInt(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return 0;
		}
		value = value.Trim();
		if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		if (int.TryParse(value.Replace(".", string.Empty), NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result2))
		{
			return Mathf.RoundToInt(result2);
		}
		return 0;
	}

	private static float ParseCsvFloat(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return 0f;
		}
		if (float.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return 0f;
	}

	public void BuildBaoshiIconMap()
	{
		baoshiByIcon.Clear();
		baoshiByItemName.Clear();
		AddBaoshiCollectionToMap(Baoshi);
		AddBaoshiCollectionToMap(BaoshiJH);
		AddBaoshiCollectionToMap(BaoshiSPC);
		AddBaoshiToMap(SkillFW);
		AddBaoshiToMap(SPCFW);
		AddBaoshiCollectionToMap(BaseFW);
	}

	private void AddBaoshiCollectionToMap(IEnumerable<BaoshiClass> baoshiList)
	{
		if (baoshiList == null)
		{
			return;
		}
		foreach (BaoshiClass baoshi in baoshiList)
		{
			AddBaoshiToMap(baoshi);
		}
	}

	private void AddBaoshiToMap(BaoshiClass baoshi)
	{
		if (baoshi != null)
		{
			if ((bool)baoshi.Icon && !baoshiByIcon.ContainsKey(baoshi.Icon))
			{
				baoshiByIcon.Add(baoshi.Icon, baoshi);
			}
			if (!string.IsNullOrEmpty(baoshi.ItemName) && !baoshiByItemName.ContainsKey(baoshi.ItemName))
			{
				baoshiByItemName.Add(baoshi.ItemName, baoshi);
			}
		}
	}

	public void TryGetBaoshiByIcon(Sprite icon, out BaoshiClass data)
	{
		if ((bool)icon && baoshiByIcon != null)
		{
			baoshiByIcon.TryGetValue(icon, out data);
		}
		else
		{
			data = null;
		}
	}

	public void TryGetBaoshiByItemName(string itemName, out BaoshiClass data)
	{
		if (!string.IsNullOrEmpty(itemName) && baoshiByItemName != null)
		{
			baoshiByItemName.TryGetValue(itemName, out data);
		}
		else
		{
			data = null;
		}
	}

	private void FillBaoshiExtraData(BaoshiClass baoshi)
	{
		if (baoshi == null)
		{
			return;
		}
		TryGetBaoshiByItemName(baoshi.ItemName, out var data);
		if (data != null)
		{
			if (baoshi.UseType == 0 && data.UseType != 0)
			{
				baoshi.UseType = data.UseType;
			}
			if (baoshi.BS_Quality == 0 && data.BS_Quality != 0)
			{
				baoshi.BS_Quality = data.BS_Quality;
			}
		}
	}

	private void RestoreWeapon(WeaponState st)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, st.Position, Quaternion.identity).GetComponent<DropItemController>();
		component.RuntimeState = st;
		st.ApplyToRuntime(component.weapon);
		component.InitDrop(component.weapon, 0f, playAnim: false);
	}

	public void DropWeapon(Transform trans, float high, int level, int type, float DropRate)
	{
		DR_EM = DropRate;
		int num = UnityEngine.Random.Range(0, 4);
		if (UnityEngine.Random.Range(0, 100) < 60)
		{
			num = PL.PLType;
		}
		int num2 = UnityEngine.Random.Range(0, 2);
		int num3 = UnityEngine.Random.Range(0, 10);
		int num4 = ((UnityEngine.Random.Range(0, 2) != 0) ? num2 : num3);
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num5 = 0;
		float num6 = ((level < 5) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DropRate) : ((level < 10) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + DropRate) : ((level < 20) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + PL.ItemDrop_Rate_Last + DropRate) : ((level < 30) ? UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + DR_Epic + PL.ItemDrop_Rate_Last + DropRate) : ((level >= 40) ? UnityEngine.Random.Range(0f, DR_Max + PL.ItemDrop_Rate_Last + DropRate) : UnityEngine.Random.Range(0f, DR_Normal + DR_Magic + DR_Rare + DR_Epic + DR_Legendary + PL.ItemDrop_Rate_Last + DropRate))))));
		if (num6 < DR_Normal)
		{
			Cur_Q = 0;
		}
		else if (num6 > DR_Normal && num6 < DR_Normal + DR_Magic)
		{
			Cur_Q = 1;
		}
		else if (num6 > DR_Normal + DR_Magic && num6 < DR_Normal + DR_Magic + DR_Rare)
		{
			for (int i = 0; i < Weapon.GP[num].QL[num4].Rare.Count; i++)
			{
				if (Weapon.GP[num].QL[num4].Rare[i].DropLevelStart <= level)
				{
					dictionary.Add(num5, i);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 2;
			}
			else
			{
				Cur_Q = UnityEngine.Random.Range(0, 2);
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare && num6 < DR_Normal + DR_Magic + DR_Rare + DR_Exquisite)
		{
			for (int j = 0; j < Weapon.GP[num].QL[num4].Exquisite.Count; j++)
			{
				if (Weapon.GP[num].QL[num4].Exquisite[j].DropLevelStart <= level)
				{
					dictionary.Add(num5, j);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 3;
			}
			else
			{
				for (int k = 0; k < Weapon.GP[num].QL[num4].Rare.Count; k++)
				{
					if (Weapon.GP[num].QL[num4].Rare[k].DropLevelStart <= level)
					{
						dictionary.Add(num5, k);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 2;
				}
				else
				{
					Cur_Q = UnityEngine.Random.Range(0, 2);
				}
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare + DR_Exquisite && num6 < DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic)
		{
			for (int l = 0; l < Weapon.GP[num].QL[num4].Epic.Count; l++)
			{
				if (Weapon.GP[num].QL[num4].Epic[l].DropLevelStart <= level)
				{
					dictionary.Add(num5, l);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 4;
			}
			else
			{
				for (int m = 0; m < Weapon.GP[num].QL[num4].Exquisite.Count; m++)
				{
					if (Weapon.GP[num].QL[num4].Exquisite[m].DropLevelStart <= level)
					{
						dictionary.Add(num5, m);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 3;
				}
				else
				{
					for (int n = 0; n < Weapon.GP[num].QL[num4].Rare.Count; n++)
					{
						if (Weapon.GP[num].QL[num4].Rare[n].DropLevelStart <= level)
						{
							dictionary.Add(num5, n);
							num5++;
						}
					}
					if (num5 > 0)
					{
						Cur_Q = 2;
					}
					else
					{
						Cur_Q = UnityEngine.Random.Range(0, 2);
					}
				}
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic && num6 < DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic + DR_Legendary)
		{
			for (int num7 = 0; num7 < Weapon.GP[num].QL[num4].Legendary.Count; num7++)
			{
				if (Weapon.GP[num].QL[num4].Legendary[num7].DropLevelStart <= level)
				{
					dictionary.Add(num5, num7);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 5;
			}
			else
			{
				for (int num8 = 0; num8 < Weapon.GP[num].QL[num4].Epic.Count; num8++)
				{
					if (Weapon.GP[num].QL[num4].Epic[num8].DropLevelStart <= level)
					{
						dictionary.Add(num5, num8);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 4;
				}
				else
				{
					for (int num9 = 0; num9 < Weapon.GP[num].QL[num4].Exquisite.Count; num9++)
					{
						if (Weapon.GP[num].QL[num4].Exquisite[num9].DropLevelStart <= level)
						{
							dictionary.Add(num5, num9);
							num5++;
						}
					}
					if (num5 > 0)
					{
						Cur_Q = 3;
					}
					else
					{
						for (int num10 = 0; num10 < Weapon.GP[num].QL[num4].Rare.Count; num10++)
						{
							if (Weapon.GP[num].QL[num4].Rare[num10].DropLevelStart <= level)
							{
								dictionary.Add(num5, num10);
								num5++;
							}
						}
						if (num5 > 0)
						{
							Cur_Q = 2;
						}
						else
						{
							Cur_Q = UnityEngine.Random.Range(0, 2);
						}
					}
				}
			}
		}
		else if (num6 > DR_Normal + DR_Magic + DR_Rare + DR_Exquisite + DR_Epic + DR_Legendary)
		{
			for (int num11 = 0; num11 < Weapon.GP[num].QL[num4].Mythical.Count; num11++)
			{
				if (Weapon.GP[num].QL[num4].Mythical[num11].DropLevelStart <= level)
				{
					dictionary.Add(num5, num11);
					num5++;
				}
			}
			if (num5 > 0)
			{
				Cur_Q = 6;
			}
			else
			{
				for (int num12 = 0; num12 < Weapon.GP[num].QL[num4].Legendary.Count; num12++)
				{
					if (Weapon.GP[num].QL[num4].Legendary[num12].DropLevelStart <= level)
					{
						dictionary.Add(num5, num12);
						num5++;
					}
				}
				if (num5 > 0)
				{
					Cur_Q = 5;
				}
				else
				{
					for (int num13 = 0; num13 < Weapon.GP[num].QL[num4].Epic.Count; num13++)
					{
						if (Weapon.GP[num].QL[num4].Epic[num13].DropLevelStart <= level)
						{
							dictionary.Add(num5, num13);
							num5++;
						}
					}
					if (num5 > 0)
					{
						Cur_Q = 4;
					}
					else
					{
						for (int num14 = 0; num14 < Weapon.GP[num].QL[num4].Exquisite.Count; num14++)
						{
							if (Weapon.GP[num].QL[num4].Exquisite[num14].DropLevelStart <= level)
							{
								dictionary.Add(num5, num14);
								num5++;
							}
						}
						if (num5 > 0)
						{
							Cur_Q = 3;
						}
						else
						{
							for (int num15 = 0; num15 < Weapon.GP[num].QL[num4].Rare.Count; num15++)
							{
								if (Weapon.GP[num].QL[num4].Rare[num15].DropLevelStart <= level)
								{
									dictionary.Add(num5, num15);
									num5++;
								}
							}
							if (num5 > 0)
							{
								Cur_Q = 2;
							}
							else
							{
								Cur_Q = UnityEngine.Random.Range(0, 2);
							}
						}
					}
				}
			}
		}
		Item_MB mb;
		switch (Cur_Q)
		{
		case 0:
		{
			for (int num17 = 0; num17 < Weapon.GP[num].QL[num4].Normal.Count; num17++)
			{
				if (Weapon.GP[num].QL[num4].Normal[num17].DropLevelStart <= level)
				{
					dictionary.Add(num5, num17);
					num5++;
				}
			}
			int key2 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key2, out var value2);
			mb = Weapon.GP[num].QL[num4].Normal[value2];
			break;
		}
		case 1:
		{
			for (int num21 = 0; num21 < Weapon.GP[num].QL[num4].Magic.Count; num21++)
			{
				if (Weapon.GP[num].QL[num4].Magic[num21].DropLevelStart <= level)
				{
					dictionary.Add(num5, num21);
					num5++;
				}
			}
			int key6 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key6, out var value6);
			mb = Weapon.GP[num].QL[num4].Magic[value6];
			break;
		}
		case 2:
		{
			for (int num18 = 0; num18 < Weapon.GP[num].QL[num4].Rare.Count; num18++)
			{
				if (Weapon.GP[num].QL[num4].Rare[num18].DropLevelStart <= level)
				{
					dictionary.Add(num5, num18);
					num5++;
				}
			}
			int key3 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key3, out var value3);
			mb = Weapon.GP[num].QL[num4].Rare[value3];
			break;
		}
		case 3:
		{
			for (int num20 = 0; num20 < Weapon.GP[num].QL[num4].Exquisite.Count; num20++)
			{
				if (Weapon.GP[num].QL[num4].Exquisite[num20].DropLevelStart <= level)
				{
					dictionary.Add(num5, num20);
					num5++;
				}
			}
			int key5 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key5, out var value5);
			mb = Weapon.GP[num].QL[num4].Exquisite[value5];
			break;
		}
		case 4:
		{
			for (int num22 = 0; num22 < Weapon.GP[num].QL[num4].Epic.Count; num22++)
			{
				if (Weapon.GP[num].QL[num4].Epic[num22].DropLevelStart <= level)
				{
					dictionary.Add(num5, num22);
					num5++;
				}
			}
			int key7 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key7, out var value7);
			mb = Weapon.GP[num].QL[num4].Epic[value7];
			break;
		}
		case 5:
		{
			for (int num19 = 0; num19 < Weapon.GP[num].QL[num4].Legendary.Count; num19++)
			{
				if (Weapon.GP[num].QL[num4].Legendary[num19].DropLevelStart <= level)
				{
					dictionary.Add(num5, num19);
					num5++;
				}
			}
			int key4 = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key4, out var value4);
			mb = Weapon.GP[num].QL[num4].Legendary[value4];
			break;
		}
		case 6:
		{
			for (int num16 = 0; num16 < Weapon.GP[num].QL[num4].Mythical.Count; num16++)
			{
				if (Weapon.GP[num].QL[num4].Mythical[num16].DropLevelStart <= level)
				{
					dictionary.Add(num5, num16);
					num5++;
				}
			}
			int key = UnityEngine.Random.Range(0, num5);
			dictionary.TryGetValue(key, out var value);
			mb = Weapon.GP[num].QL[num4].Mythical[value];
			break;
		}
		default:
			mb = null;
			break;
		}
		SpawnNewWeapon(trans, mb, level, high);
	}

	public void SpawnNewWeapon(Transform trans, Item_MB mb, int level, float high)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
		component.PlayerDrop = false;
		SetWPdata(component.weapon, mb, level);
		WeaponState weaponState = WeaponState.FromRuntime(component.weapon);
		weaponState.Position = component.transform.position;
		component.RuntimeState = weaponState;
		component.InitDrop(component.weapon, high);
	}

	private void RestoreBaoshi(BaoshiState st)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, st.Position, Quaternion.identity).GetComponent<DropItemController>();
		component.RuntimeState = st;
		st.ApplyToRuntime(component.baoshi);
		FillBaoshiExtraData(component.baoshi);
		component.InitDrop(component.baoshi, 0f, playAnim: false);
	}

	public void DropBaoshi(Transform trans, float high, int level)
	{
		int bestBaoshiDropIndex = GetBestBaoshiDropIndex(level);
		if (bestBaoshiDropIndex >= 0)
		{
			SpawnNewBaoshi(trans, bestBaoshiDropIndex, high);
		}
	}

	private int GetBestBaoshiDropIndex(int level)
	{
		if (Baoshi == null || Baoshi.Count == 0)
		{
			return -1;
		}
		List<int> list = new List<int>();
		BaoshiClass best = null;
		for (int i = 0; i < Baoshi.Count; i++)
		{
			BaoshiClass baoshiClass = Baoshi[i];
			if (CanDropBaoshi(baoshiClass, level))
			{
				int num = CompareBaoshiDropPriority(baoshiClass, best);
				if (num > 0)
				{
					best = baoshiClass;
					list.Clear();
					list.Add(i);
				}
				else if (num == 0)
				{
					list.Add(i);
				}
			}
		}
		if (list.Count <= 0)
		{
			return -1;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private static bool CanDropBaoshi(BaoshiClass baoshi, int level)
	{
		if (baoshi != null && IsBaoshiInDropLevelRange(baoshi, level))
		{
			return CanDropBaoshiInCurrentScene(baoshi);
		}
		return false;
	}

	private static bool IsBaoshiInDropLevelRange(BaoshiClass baoshi, int level)
	{
		if (level < 15)
		{
			return baoshi.Level < 15;
		}
		if (level < 30)
		{
			if (baoshi.Level >= 15)
			{
				return baoshi.Level < 30;
			}
			return false;
		}
		if (level < 50)
		{
			if (baoshi.Level >= 30)
			{
				return baoshi.Level < 50;
			}
			return false;
		}
		if (level < 70)
		{
			if (baoshi.Level >= 50)
			{
				return baoshi.Level < 70;
			}
			return false;
		}
		if (level < 90)
		{
			if (baoshi.Level >= 70)
			{
				return baoshi.Level < 90;
			}
			return false;
		}
		return baoshi.Level >= 90;
	}

	private static bool CanDropBaoshiInCurrentScene(BaoshiClass baoshi)
	{
		if (LevelManager.GetIsMijing())
		{
			if (!SingletonMonoScope<MijingManager>.HasInstance)
			{
				return false;
			}
			return baoshi.DropScene == SingletonMonoScope<MijingManager>.Instance.GetCurrentSceneQulity();
		}
		return baoshi.DropScene == 0;
	}

	private static int CompareBaoshiDropPriority(BaoshiClass baoshi, BaoshiClass best)
	{
		if (best == null)
		{
			return 1;
		}
		if (baoshi == null)
		{
			return -1;
		}
		if (baoshi.Number != best.Number)
		{
			return baoshi.Number.CompareTo(best.Number);
		}
		if (baoshi.BS_Quality != best.BS_Quality)
		{
			return baoshi.BS_Quality.CompareTo(best.BS_Quality);
		}
		if (baoshi.Quality != best.Quality)
		{
			return baoshi.Quality.CompareTo(best.Quality);
		}
		if (baoshi.priceQulity != best.priceQulity)
		{
			return baoshi.priceQulity.CompareTo(best.priceQulity);
		}
		return baoshi.Price.CompareTo(best.Price);
	}

	public void SpawnNewBaoshi(Transform trans, int a, float high)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
		SetBaoshidata(component, a);
		BaoshiState baoshiState = BaoshiState.FromRuntime(component.baoshi);
		baoshiState.Position = component.transform.position;
		component.RuntimeState = baoshiState;
		component.InitDrop(component.baoshi, high);
	}

	private void DropRandomEssence(Transform trans, float high)
	{
		DropRandomEssence(trans, high, LevelManager.GetCurrentEnemyLevel());
	}

	private void DropRandomEssence(Transform trans, float high, int level)
	{
		if (BaoshiJH == null || BaoshiJH.Count == 0)
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < BaoshiJH.Count; i++)
		{
			if (CanDropBaoshiByLevel(BaoshiJH[i], level))
			{
				list.Add(i);
			}
		}
		if (list.Count != 0)
		{
			DropBaoshiTemplate(trans, high, BaoshiJH[list[UnityEngine.Random.Range(0, list.Count)]]);
		}
	}

	private void DropSpecialBaoshi(Transform trans, float high, string bsType)
	{
		DropSpecialBaoshi(trans, high, LevelManager.GetCurrentEnemyLevel(), bsType);
	}

	private void DropSpecialBaoshi(Transform trans, float high, int level, string bsType)
	{
		if (BaoshiSPC == null || string.IsNullOrEmpty(bsType))
		{
			return;
		}
		for (int i = 0; i < BaoshiSPC.Count; i++)
		{
			BaoshiClass baoshiClass = BaoshiSPC[i];
			if (baoshiClass != null && baoshiClass.BStype == bsType && CanDropBaoshiByLevel(baoshiClass, level))
			{
				DropBaoshiTemplate(trans, high, baoshiClass);
				break;
			}
		}
	}

	private static bool CanDropBaoshiByLevel(BaoshiClass baoshi, int level)
	{
		if (baoshi != null)
		{
			return baoshi.Level <= level;
		}
		return false;
	}

	private void DropBaoshiTemplate(Transform trans, float high, BaoshiClass source)
	{
		if (source != null)
		{
			SpawnNewBaoshi(trans, high, delegate(BaoshiClass baoshi)
			{
				ItemCloneUtil.CopyBaoshiTo(baoshi, source);
			});
		}
	}

	public void DropSkillFW(Transform trans, float high)
	{
		if (SkillFW == null)
		{
			return;
		}
		SKFW fw = GetRandomSkillFWForCurrentPlayer();
		if (fw != null)
		{
			SpawnNewBaoshi(trans, high, delegate(BaoshiClass baoshi)
			{
				ItemCloneUtil.CopyBaoshiTo(baoshi, SkillFW);
				baoshi.SKname = fw.SkillName;
				baoshi.Index = fw.index;
				baoshi.EL = fw.EL;
				baoshi.priceQulity = fw.Price;
				baoshi.Price = GetSkillRunePrice(fw.SkillName);
				baoshi.Xi = fw.Xi;
				baoshi.Number = Mathf.Max(1, baoshi.Number);
				baoshi.Icon = GetSkillFWIcon(fw.EL);
			});
		}
	}

	public void DropSPCFW(Transform trans, float high)
	{
		if (SPCFW == null)
		{
			return;
		}
		int randomSPCFWType = GetRandomSPCFWType();
		SPC_MB mb = GetRandomSPCMBForCurrentPlayer(randomSPCFWType);
		if (mb == null)
		{
			return;
		}
		SpawnNewBaoshi(trans, high, delegate(BaoshiClass baoshi)
		{
			ItemCloneUtil.CopyBaoshiTo(baoshi, SPCFW);
			baoshi.ItemType = 1;
			baoshi.UseType = 4;
			baoshi.SKname = mb.SPCname;
			baoshi.Index = mb.SPCindex;
			baoshi.EL = UnityEngine.Random.Range(0, 6);
			baoshi.PRC = GivePRC_SPC(baoshi.Level, baoshi.Quality);
			baoshi.priceQulity = mb.Price;
			baoshi.Price = GetSPCRunePrice(baoshi.Index, baoshi.PRC);
			baoshi.FWtype = mb.FWtype;
			baoshi.Number = Mathf.Max(1, baoshi.Number);
			baoshi.MstackSize = Mathf.Max(1, baoshi.MstackSize);
			baoshi.CstackSize = Mathf.Max(1, baoshi.CstackSize);
			if ((bool)SPCFW_Icon)
			{
				baoshi.Icon = SPCFW_Icon;
			}
		});
	}

	public void DropBaseFW(Transform trans, float high)
	{
		if (BaseFW == null || BaseFW.Count == 0)
		{
			return;
		}
		List<BaoshiClass> list = new List<BaoshiClass>();
		for (int i = 0; i < BaseFW.Count; i++)
		{
			BaoshiClass baoshiClass = BaseFW[i];
			if (CanDropBaseFWInCurrentScene(baoshiClass))
			{
				list.Add(baoshiClass);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		BaoshiClass source = list[UnityEngine.Random.Range(0, list.Count)];
		if (source == null)
		{
			return;
		}
		SpawnNewBaoshi(trans, high, delegate(BaoshiClass baoshi)
		{
			ItemCloneUtil.CopyBaoshiTo(baoshi, source);
			baoshi.Number = Mathf.RoundToInt((float)baoshi.Number * UnityEngine.Random.Range(0.7f, 1.1f));
			if ((bool)BaseFW_Icon)
			{
				baoshi.Icon = BaseFW_Icon;
			}
		});
	}

	private static bool CanDropBaseFWInCurrentScene(BaoshiClass fw)
	{
		if (fw == null)
		{
			return false;
		}
		if (LevelManager.GetIsMijing())
		{
			if (!SingletonMonoScope<MijingManager>.HasInstance)
			{
				return false;
			}
			int currentSceneQulity = SingletonMonoScope<MijingManager>.Instance.GetCurrentSceneQulity();
			if (fw.DropScene > 0)
			{
				return fw.DropScene <= currentSceneQulity;
			}
			return false;
		}
		return fw.DropScene == 0;
	}

	private static int GetWeaponSPCPrice(SPC_MB spc, float spcPRC)
	{
		if (spc == null || !SPCPrice.Price.TryGetValue(spc.Price, out var value))
		{
			return 0;
		}
		return value + Mathf.FloorToInt((float)value * spcPRC);
	}

	private int GetSkillRunePrice(string skillName)
	{
		if (!TryGetSkillRunePriceQuality(skillName, out var priceQuality) || !SkillPrice.Price.TryGetValue(priceQuality, out var value))
		{
			return 0;
		}
		return value * 10;
	}

	public bool TryCreateSkillRuneFromWeaponSkill(WPSkill skill, int storedPrice, out BaoshiClass baoshi)
	{
		baoshi = null;
		if (SkillFW == null || skill == null || IsCsvNoneText(skill.IndexName))
		{
			return false;
		}
		baoshi = ItemCloneUtil.CloneBaoshi(SkillFW);
		if (baoshi == null)
		{
			return false;
		}
		baoshi.UseType = 3;
		baoshi.SKname = skill.IndexName;
		baoshi.Price = Mathf.Max(0, storedPrice);
		baoshi.CstackSize = 1;
		baoshi.MstackSize = 1;
		if (TryGetSkillRuneData(skill.IndexName, out var skillRune))
		{
			baoshi.Index = skillRune.index;
			baoshi.EL = skillRune.EL;
			baoshi.priceQulity = skillRune.Price;
			baoshi.Xi = skillRune.Xi;
			baoshi.Icon = GetSkillFWIcon(skillRune.EL);
		}
		if (!baoshi.Icon)
		{
			baoshi.Icon = GetSkillFWIcon(baoshi.EL);
		}
		return true;
	}

	public bool TryCreateSPCRuneFromWeaponSPC(WPSPC spc, out BaoshiClass baoshi)
	{
		baoshi = null;
		if (SPCFW == null || spc == null || spc.Index <= 0)
		{
			return false;
		}
		if (!TryGetSPCMBByIndex(spc.Index, out var mb) || mb == null)
		{
			return false;
		}
		baoshi = ItemCloneUtil.CloneBaoshi(SPCFW);
		if (baoshi == null)
		{
			return false;
		}
		baoshi.ItemType = 1;
		baoshi.UseType = 4;
		baoshi.SKname = mb.SPCname;
		baoshi.Index = spc.Index;
		baoshi.EL = spc.EL;
		baoshi.PRC = spc.PRC;
		baoshi.priceQulity = mb.Price;
		baoshi.Price = Mathf.Max(0, spc.price);
		baoshi.FWtype = mb.FWtype;
		baoshi.Number = Mathf.Max(1, baoshi.Number);
		baoshi.MstackSize = 1;
		baoshi.CstackSize = 1;
		if ((bool)SPCFW_Icon)
		{
			baoshi.Icon = SPCFW_Icon;
		}
		return true;
	}

	public bool TryCreateAttributeRuneFromWeaponBase(WPFW_Base fwBase, int fwType, out BaoshiClass baoshi)
	{
		baoshi = null;
		if (fwBase == null || string.IsNullOrEmpty(fwBase.FWname))
		{
			return false;
		}
		BaoshiClass baoshiClass = FindBaseRuneTemplate(fwBase.FWname, fwBase.type);
		if (baoshiClass != null)
		{
			baoshi = ItemCloneUtil.CloneBaoshi(baoshiClass);
		}
		else
		{
			baoshi = new BaoshiClass();
			baoshi.Reset();
			baoshi.ItemType = 1;
			baoshi.UseType = 5;
			baoshi.ItemName = fwBase.FWname;
			baoshi.BStype = fwBase.type;
			baoshi.Quality = ((BaseFW != null && BaseFW.Count > 0 && BaseFW[0] != null) ? BaseFW[0].Quality : 6);
			baoshi.Level = ((BaseFW != null && BaseFW.Count > 0 && BaseFW[0] != null) ? BaseFW[0].Level : 0);
			baoshi.MstackSize = 1;
		}
		if (baoshi == null)
		{
			return false;
		}
		baoshi.UseType = 5;
		baoshi.SKname = fwBase.FWname;
		baoshi.ItemName = fwBase.FWname;
		baoshi.BStype = fwBase.type;
		baoshi.FWtype = fwType;
		baoshi.Number = Mathf.RoundToInt(fwBase.number);
		baoshi.Price = Mathf.Max(0, fwBase.price);
		baoshi.CstackSize = 1;
		baoshi.MstackSize = 1;
		if ((bool)BaseFW_Icon)
		{
			baoshi.Icon = BaseFW_Icon;
		}
		return true;
	}

	private bool TryGetSkillRunePriceQuality(string skillName, out int priceQuality)
	{
		priceQuality = 0;
		if (IsCsvNoneText(skillName))
		{
			return false;
		}
		TalentManager talentManager = (TL ? TL : (SingletonMonoScope<TalentManager>.HasInstance ? SingletonMonoScope<TalentManager>.Instance : null));
		if (talentManager == null)
		{
			return false;
		}
		talentManager.EnsureSkillFWLibrary();
		SKFW_Group fW = talentManager.FW;
		if (fW?.Char == null)
		{
			return false;
		}
		for (int i = 0; i < fW.Char.Length; i++)
		{
			SKFW_Char sKFW_Char = fW.Char[i];
			if (sKFW_Char?.Xi == null)
			{
				continue;
			}
			for (int j = 0; j < sKFW_Char.Xi.Length; j++)
			{
				SKFW[] array = sKFW_Char.Xi[j]?.FW;
				if (array == null)
				{
					continue;
				}
				SKFW[] array2 = array;
				foreach (SKFW sKFW in array2)
				{
					if (sKFW != null && sKFW.SkillName == skillName)
					{
						priceQuality = sKFW.Price;
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool TryGetSkillRuneData(string skillName, out SKFW skillRune)
	{
		skillRune = null;
		if (IsCsvNoneText(skillName))
		{
			return false;
		}
		TalentManager talentManager = (TL ? TL : (SingletonMonoScope<TalentManager>.HasInstance ? SingletonMonoScope<TalentManager>.Instance : null));
		if (talentManager == null)
		{
			return false;
		}
		talentManager.EnsureSkillFWLibrary();
		SKFW_Group fW = talentManager.FW;
		if (fW?.Char == null)
		{
			return false;
		}
		for (int i = 0; i < fW.Char.Length; i++)
		{
			SKFW_Char sKFW_Char = fW.Char[i];
			if (sKFW_Char?.Xi == null)
			{
				continue;
			}
			for (int j = 0; j < sKFW_Char.Xi.Length; j++)
			{
				SKFW[] array = sKFW_Char.Xi[j]?.FW;
				if (array == null)
				{
					continue;
				}
				SKFW[] array2 = array;
				foreach (SKFW sKFW in array2)
				{
					if (sKFW != null && sKFW.SkillName == skillName)
					{
						skillRune = sKFW;
						return true;
					}
				}
			}
		}
		return false;
	}

	private BaoshiClass FindBaseRuneTemplate(string itemName, string runeType)
	{
		if (BaseFW == null)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(itemName))
		{
			for (int i = 0; i < BaseFW.Count; i++)
			{
				BaoshiClass baoshiClass = BaseFW[i];
				if (baoshiClass != null && baoshiClass.ItemName == itemName)
				{
					return baoshiClass;
				}
			}
		}
		if (!string.IsNullOrEmpty(runeType))
		{
			for (int j = 0; j < BaseFW.Count; j++)
			{
				BaoshiClass baoshiClass2 = BaseFW[j];
				if (baoshiClass2 != null && baoshiClass2.BStype == runeType)
				{
					return baoshiClass2;
				}
			}
		}
		return null;
	}

	private int GetSPCRunePrice(int spcIndex, float spcPRC)
	{
		if (!TryGetSPCMBByIndex(spcIndex, out var mb))
		{
			return 0;
		}
		return GetWeaponSPCPrice(mb, spcPRC) * 5;
	}

	private void SpawnNewBaoshi(Transform trans, float high, Action<BaoshiClass> setupAction)
	{
		if ((bool)trans && setupAction != null)
		{
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			component.baoshi.Reset();
			setupAction(component.baoshi);
			component.ItemType = 1;
			BaoshiState baoshiState = BaoshiState.FromRuntime(component.baoshi);
			baoshiState.Position = component.transform.position;
			component.RuntimeState = baoshiState;
			component.InitDrop(component.baoshi, high);
		}
	}

	private SKFW GetRandomSkillFWForCurrentPlayer()
	{
		TalentManager talentManager = (TL ? TL : (SingletonMonoScope<TalentManager>.HasInstance ? SingletonMonoScope<TalentManager>.Instance : null));
		if (talentManager == null)
		{
			return null;
		}
		talentManager.EnsureSkillFWLibrary();
		int num = Mathf.Clamp(PL.PLType, 0, 3);
		SKFW_Char sKFW_Char = ((talentManager.FW?.Char != null && num < talentManager.FW.Char.Length) ? talentManager.FW.Char[num] : null);
		if (sKFW_Char?.Xi == null)
		{
			return null;
		}
		List<SKFW> list = new List<SKFW>();
		for (int i = 0; i < sKFW_Char.Xi.Length; i++)
		{
			SKFW[] array = sKFW_Char.Xi[i]?.FW;
			if (array == null)
			{
				continue;
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null && !string.IsNullOrEmpty(array[j].SkillName))
				{
					list.Add(array[j]);
				}
			}
		}
		if (list.Count != 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	private SPC_MB GetRandomSPCMBForCurrentPlayer(int fwType)
	{
		if (SPCMB == null)
		{
			return null;
		}
		fwType = NormalizeSPCFWType(fwType);
		int playerType = ((PL != null) ? Mathf.Clamp(PL.PLType, 0, 3) : 0);
		SPC_MB[] array = GetSPCMBListByPlayerAndFWType(playerType, fwType);
		if (array == null || array.Length == 0)
		{
			array = GetSPCMBListByFWTypeFromAll(fwType);
		}
		if (array == null || array.Length == 0)
		{
			return null;
		}
		List<SPC_MB> list = new List<SPC_MB>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].SPCindex > 0 && array[i].SPCtype > 0)
			{
				list.Add(array[i]);
			}
		}
		if (list.Count != 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	private int GetRandomSPCFWType()
	{
		if (SPCMB == null)
		{
			return ProbUtil.Roll(SPCFWTypeWeights);
		}
		int playerType = ((PL != null) ? Mathf.Clamp(PL.PLType, 0, 3) : 0);
		int[] array = new int[5];
		for (int i = 0; i < array.Length; i++)
		{
			if (HasSPCMBForFWType(playerType, i))
			{
				array[i] = SPCFWTypeWeights[i];
			}
		}
		return ProbUtil.Roll(array);
	}

	private bool HasSPCMBForFWType(int playerType, int fwType)
	{
		if (HasValidSPCMB(GetSPCMBListByPlayerAndFWType(playerType, fwType)))
		{
			return true;
		}
		return HasValidSPCMBForFWTypeFromAll(fwType);
	}

	private static bool HasValidSPCMB(SPC_MB[] list)
	{
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i] != null && list[i].SPCindex > 0 && list[i].SPCtype > 0)
			{
				return true;
			}
		}
		return false;
	}

	private bool HasValidSPCMBForFWTypeFromAll(int fwType)
	{
		SPC_MB[] mB = SPCMB.MB;
		if (mB == null)
		{
			return false;
		}
		for (int i = 0; i < mB.Length; i++)
		{
			if (mB[i] != null && mB[i].SPCindex > 0 && mB[i].SPCtype > 0 && NormalizeSPCFWType(mB[i].FWtype) == fwType)
			{
				return true;
			}
		}
		return false;
	}

	private SPC_MB[] GetSPCMBListByPlayerAndFWType(int playerType, int fwType)
	{
		if (SPCMB.PL == null || playerType < 0 || playerType >= SPCMB.PL.Length)
		{
			return null;
		}
		SPCMB_Player sPCMB_Player = SPCMB.PL[playerType];
		if (sPCMB_Player?.TP == null || fwType < 0 || fwType >= sPCMB_Player.TP.Length)
		{
			return null;
		}
		return sPCMB_Player.TP[fwType]?.MB;
	}

	private SPC_MB[] GetSPCMBListByFWTypeFromAll(int fwType)
	{
		SPC_MB[] mB = SPCMB.MB;
		if (mB == null || mB.Length == 0)
		{
			return null;
		}
		List<SPC_MB> list = new List<SPC_MB>();
		for (int i = 0; i < mB.Length; i++)
		{
			if (mB[i] != null && NormalizeSPCFWType(mB[i].FWtype) == fwType)
			{
				list.Add(mB[i]);
			}
		}
		if (list.Count != 0)
		{
			return list.ToArray();
		}
		return null;
	}

	private Sprite GetSkillFWIcon(int el)
	{
		if (SkillFW_Icon == null || SkillFW_Icon.Length == 0)
		{
			if (SkillFW == null)
			{
				return null;
			}
			return SkillFW.Icon;
		}
		return SkillFW_Icon[Mathf.Clamp(el, 0, SkillFW_Icon.Length - 1)];
	}

	public void DropPotionById(Transform trans, float high, int count, int globalId)
	{
		int num = Potion.FindIndex((UseItemClass p) => p.GlobalID == globalId);
		if (num == -1)
		{
			LogUtil.Warn($"Potion 列表里没有找到 GlobalID = {globalId} 的药品");
			return;
		}
		for (int i = 0; i < count; i++)
		{
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			SetPotiondata(component, num);
			UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
			useItemState.Position = component.transform.position;
			component.RuntimeState = useItemState;
			component.InitDrop(component.useitem, high);
		}
	}

	private void RestoreUseItem(UseItemState st)
	{
		DropItemController component = LeanPool.Spawn(dropOBJ, st.Position, Quaternion.identity).GetComponent<DropItemController>();
		component.RuntimeState = st;
		st.ApplyToRuntime(component.useitem);
		component.InitDrop(component.useitem, 0f, playAnim: false);
	}

	public void DropPotion(Transform trans, float high, int level)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 0;
		if (level < 10)
		{
			for (int i = 0; i < Potion.Count; i++)
			{
				if (Potion[i].Level < 10)
				{
					dictionary.Add(num, i);
					num++;
				}
			}
		}
		else if (level < 20)
		{
			for (int j = 0; j < Potion.Count; j++)
			{
				if (Potion[j].Level >= 10 && Potion[j].Level < 20)
				{
					dictionary.Add(num, j);
					num++;
				}
			}
		}
		else if (level < 30)
		{
			for (int k = 0; k < Potion.Count; k++)
			{
				if (Potion[k].Level >= 20 && Potion[k].Level < 30)
				{
					dictionary.Add(num, k);
					num++;
				}
			}
		}
		else if (level < 40)
		{
			for (int l = 0; l < Potion.Count; l++)
			{
				if (Potion[l].Level >= 30 && Potion[l].Level < 40)
				{
					dictionary.Add(num, l);
					num++;
				}
			}
		}
		else if (level < 50)
		{
			for (int m = 0; m < Potion.Count; m++)
			{
				if (Potion[m].Level >= 40 && Potion[m].Level < 50)
				{
					dictionary.Add(num, m);
					num++;
				}
			}
		}
		else if (level < 60)
		{
			for (int n = 0; n < Potion.Count; n++)
			{
				if (Potion[n].Level >= 50 && Potion[n].Level < 60)
				{
					dictionary.Add(num, n);
					num++;
				}
			}
		}
		else if (level < 70)
		{
			for (int num2 = 0; num2 < Potion.Count; num2++)
			{
				if (Potion[num2].Level >= 60 && Potion[num2].Level < 70)
				{
					dictionary.Add(num, num2);
					num++;
				}
			}
		}
		else if (level < 80)
		{
			for (int num3 = 0; num3 < Potion.Count; num3++)
			{
				if (Potion[num3].Level >= 70 && Potion[num3].Level < 80)
				{
					dictionary.Add(num, num3);
					num++;
				}
			}
		}
		else if (level < 90)
		{
			for (int num4 = 0; num4 < Potion.Count; num4++)
			{
				if (Potion[num4].Level >= 80 && Potion[num4].Level < 90)
				{
					dictionary.Add(num, num4);
					num++;
				}
			}
		}
		else if (level < 100)
		{
			for (int num5 = 0; num5 < Potion.Count; num5++)
			{
				if (Potion[num5].Level >= 90 && Potion[num5].Level < 100)
				{
					dictionary.Add(num, num5);
					num++;
				}
			}
		}
		else
		{
			for (int num6 = 0; num6 < Potion.Count; num6++)
			{
				if (Potion[num6].Level >= 90)
				{
					dictionary.Add(num, num6);
					num++;
				}
			}
		}
		int key = UnityEngine.Random.Range(0, num);
		dictionary.TryGetValue(key, out var value);
		DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
		SetPotiondata(component, value);
		UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
		useItemState.Position = component.transform.position;
		component.RuntimeState = useItemState;
		component.InitDrop(component.useitem, high);
	}

	public void DropAnyPotion(Transform trans, float high, int level)
	{
		DropPotion(trans, high, level);
	}

	public void MustDropPremPotion(Transform trans, float high)
	{
		if (TryGetCurrentPremPotionIndex(SingletonMonoScope<PlayerManager>.Instance.Level, out var index))
		{
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			SetPremPotiondata(component, index);
			UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
			useItemState.Position = component.transform.position;
			component.RuntimeState = useItemState;
			component.InitDrop(component.useitem, high);
		}
	}

	public void DropBuffPotion(Transform trans, float high, int level)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 0;
		for (int i = 0; i < BuffPotion.Count; i++)
		{
			if (BuffPotion[i].Level <= level)
			{
				dictionary.Add(num, i);
				num++;
			}
		}
		if (num > 0)
		{
			int key = UnityEngine.Random.Range(0, num);
			dictionary.TryGetValue(key, out var value);
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			SetBuffPotiondata(component, value);
			UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
			useItemState.Position = component.transform.position;
			component.RuntimeState = useItemState;
			component.InitDrop(component.useitem, high);
		}
	}

	public void DropScroll(Transform trans, float high, string scrollName)
	{
		DropScroll(trans, high, LevelManager.GetCurrentEnemyLevel(), scrollName);
	}

	public void DropScroll(Transform trans, float high, int level, string scrollName)
	{
		if (!(scrollName == "Teleport Scroll") && Scroll.TryGetValue(scrollName, out var value) && CanDropUseItemByLevel(value, level))
		{
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			SetSPCdata(component, value);
			UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
			useItemState.Position = component.transform.position;
			component.RuntimeState = useItemState;
			component.InitDrop(component.useitem, high);
		}
	}

	public void DropPremPotion(Transform trans, float high, int level)
	{
		if (TryGetCurrentPremPotionIndex(SingletonMonoScope<PlayerManager>.Instance.Level, out var index))
		{
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			SetPremPotiondata(component, index);
			UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
			useItemState.Position = component.transform.position;
			component.RuntimeState = useItemState;
			component.InitDrop(component.useitem, high);
		}
	}

	public void DropSpcPotion(Transform trans, float high, string potionName)
	{
		DropSpcPotion(trans, high, LevelManager.GetCurrentEnemyLevel(), potionName);
	}

	public void DropSpcPotion(Transform trans, float high, int level, string potionName)
	{
		if (SpcPotion.TryGetValue(potionName, out var value) && CanDropUseItemByLevel(value, level))
		{
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			SetSPCdata(component, value);
			UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
			useItemState.Position = component.transform.position;
			component.RuntimeState = useItemState;
			component.InitDrop(component.useitem, high);
		}
	}

	public void DropSpcItem(Transform trans, float high, string spcItemName)
	{
		DropSpcItem(trans, high, LevelManager.GetCurrentEnemyLevel(), spcItemName);
	}

	public void DropSpcItem(Transform trans, float high, int level, string spcItemName)
	{
		if (SpcItem.TryGetValue(spcItemName, out var value) && CanDropUseItemByLevel(value, level))
		{
			DropItemController component = LeanPool.Spawn(dropOBJ, new Vector3(trans.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), trans.position.y + UnityEngine.Random.Range(-0.1f, 0.1f), 0f), Quaternion.identity).GetComponent<DropItemController>();
			SetSPCdata(component, value);
			UseItemState useItemState = UseItemState.FromRuntime(component.useitem);
			useItemState.Position = component.transform.position;
			component.RuntimeState = useItemState;
			component.InitDrop(component.useitem, high);
		}
	}

	private static bool CanDropUseItemByLevel(UseItemClass item, int level)
	{
		if (item != null)
		{
			return item.Level <= level;
		}
		return false;
	}

	public void Register(DropItemController item)
	{
		if ((bool)item)
		{
			AliveDropItems.Add(item);
		}
	}

	public void Unregister(DropItemController item)
	{
		if ((bool)item)
		{
			AliveDropItems.Remove(item);
		}
	}

	public void ClearAlive()
	{
		AliveDropItems.Clear();
	}

	public void FlushToState(string levelId)
	{
		LevelState levelStateByLevelId = SingletonMonoGlobal<StateDataManager>.Instance.GetLevelStateByLevelId(levelId);
		if (levelStateByLevelId == null)
		{
			return;
		}
		if (levelStateByLevelId.ItemLevelStates == null)
		{
			levelStateByLevelId.ItemLevelStates = new List<ItemLevelState>();
		}
		levelStateByLevelId.ItemLevelStates.Clear();
		foreach (DropItemController aliveDropItem in AliveDropItems)
		{
			if ((bool)aliveDropItem)
			{
				ItemLevelState runtimeState = aliveDropItem.RuntimeState;
				if (runtimeState != null)
				{
					runtimeState.Position = aliveDropItem.transform.position;
					levelStateByLevelId.ItemLevelStates.Add(runtimeState);
				}
			}
		}
	}

	public void RestoreAllDropItems()
	{
		if (SingletonMonoScope<LevelManager>.HasInstance && !LevelManager.ShouldPersistLevelState(LevelManager.GetCurLevel()))
		{
			return;
		}
		List<ItemLevelState> list = SingletonMonoGlobal<StateDataManager>.Instance.GetCurrentLevelState()?.ItemLevelStates;
		if (list == null || list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			ItemLevelState itemLevelState = list[i];
			switch (itemLevelState.DropItemType)
			{
			default:
				return;
			case DropItemType.Weapon:
				RestoreWeapon((WeaponState)itemLevelState);
				break;
			case DropItemType.Baoshi:
				RestoreBaoshi((BaoshiState)itemLevelState);
				break;
			case DropItemType.UseItem:
				RestoreUseItem((UseItemState)itemLevelState);
				break;
			}
		}
	}

	public Item_MB CraftFindTemplate(WeaponClass weapon)
	{
		return FindWeaponTemplate(weapon);
	}

	public WeaponDropContext CraftGetDropContext(WeaponClass weapon)
	{
		return GetWeaponDropContext(weapon);
	}

	public Item_MB CraftPickPoolTemplate(WeaponClass weapon, int quality)
	{
		if (weapon == null || Weapon == null || Weapon.GP == null)
		{
			return null;
		}
		int num = Weapon.GP.Length;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < num + 1; j++)
			{
				int num2 = ((j == 0) ? weapon.PLtype : (j - 1));
				if (num2 < 0 || num2 >= num)
				{
					continue;
				}
				Weapon_Group weapon_Group = Weapon.GP[num2];
				if (weapon_Group == null || weapon_Group.QL == null || weapon.CharType < 0 || weapon.CharType >= weapon_Group.QL.Length)
				{
					continue;
				}
				List<Item_MB> weaponQualityList = GetWeaponQualityList(weapon_Group.QL[weapon.CharType], quality);
				if (weaponQualityList == null || weaponQualityList.Count == 0)
				{
					continue;
				}
				List<Item_MB> list = new List<Item_MB>();
				foreach (Item_MB item in weaponQualityList)
				{
					if (item != null && (i != 0 || string.IsNullOrEmpty(weapon.WeaponType) || !(item.WeaponType != weapon.WeaponType)))
					{
						list.Add(item);
					}
				}
				if (list.Count > 0)
				{
					return list[UnityEngine.Random.Range(0, list.Count)];
				}
			}
		}
		return null;
	}

	public WPDT_A CraftRollEntryA(WPDT_A[] pool, int level, int quality, WeaponDropContext ctx, bool isMainGroup, HashSet<int> excludeIndex)
	{
		if (pool == null || pool.Length == 0)
		{
			return null;
		}
		List<WPDT_A> list = new List<WPDT_A>();
		foreach (WPDT_A wPDT_A in pool)
		{
			if (wPDT_A != null && wPDT_A.Index != 0 && (excludeIndex == null || !excludeIndex.Contains(wPDT_A.Index)))
			{
				list.Add(wPDT_A);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		WPDT_A wPDT_A2 = list[UnityEngine.Random.Range(0, list.Count)];
		return new WPDT_A
		{
			Index = wPDT_A2.Index,
			EL = ResolveGeneratedWeaponElement(wPDT_A2.EL),
			number = GenerateWeaponStatValue(wPDT_A2.number, wPDT_A2.Index, level, quality, ctx, (!isMainGroup) ? WeaponStatGroup.Dot : WeaponStatGroup.Main, isMainGroup)
		};
	}

	public WPDT_B CraftRollEntryB(WPDT_B[] pool, int level, int quality, WeaponDropContext ctx, bool isSkillGroup, HashSet<string> excludeKeys)
	{
		if (pool == null || pool.Length == 0)
		{
			return null;
		}
		List<WPDT_B> list = new List<WPDT_B>();
		foreach (WPDT_B wPDT_B in pool)
		{
			if (wPDT_B != null && !IsCsvNoneText(wPDT_B.SkillName) && (excludeKeys == null || !excludeKeys.Contains(GetWeaponSkillEffectKey(wPDT_B))))
			{
				list.Add(wPDT_B);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		WPDT_B wPDT_B2 = list[UnityEngine.Random.Range(0, list.Count)];
		return new WPDT_B
		{
			SkillName = wPDT_B2.SkillName,
			Index = wPDT_B2.Index,
			GlobleID = wPDT_B2.GlobleID,
			EL = ResolveGeneratedWeaponElement(wPDT_B2.EL),
			number = GenerateWeaponStatValue(wPDT_B2.number, wPDT_B2.Index, level, quality, ctx, isSkillGroup ? WeaponStatGroup.Skill : WeaponStatGroup.Companion),
			LinkSK = wPDT_B2.LinkSK
		};
	}

	public static string CraftSkillEffectKey(WPDT_B data)
	{
		return GetWeaponSkillEffectKey(data);
	}

	public static bool CraftIsNoneSkill(string value)
	{
		return IsCsvNoneText(value);
	}

	public WPSPC CraftRollSPC(Item_MB tpl, int level, int quality, WeaponDropContext ctx)
	{
		WPSPC randomWeaponSPC = GetRandomWeaponSPC(tpl?.SPC);
		if (randomWeaponSPC == null)
		{
			return null;
		}
		return new WPSPC
		{
			Index = randomWeaponSPC.Index,
			EL = UnityEngine.Random.Range(0, 6),
			PRC = GivePRC_SPC(level, quality, ctx),
			price = 0
		};
	}

	public bool CraftHasSpcPool(Item_MB tpl)
	{
		return GetRandomWeaponSPC(tpl?.SPC) != null;
	}

	public void CraftRerollElement(WeaponClass weapon, float elementBase, int level, WeaponDropContext ctx)
	{
		ApplyElement(weapon, elementBase, level, ctx);
	}

	public void CraftRerollStatValues(WeaponClass it, Item_MB tpl, int level, WeaponDropContext ctx, bool rerollMain, bool rerollDot, bool rerollSkill, bool rerollComp, bool rerollSpc, bool rerollElement)
	{
		if (it == null || tpl == null)
		{
			return;
		}
		if (rerollMain && it.Main != null)
		{
			for (int i = 0; i < it.Main.Length; i++)
			{
				WPDT_A wPDT_A = it.Main[i];
				if (wPDT_A != null && wPDT_A.Index != 0)
				{
					float num = FindCraftBaseNumberA(tpl.Main, tpl.RateMain, wPDT_A.Index);
					if (num > 0f)
					{
						wPDT_A.number = GenerateWeaponStatValue(num, wPDT_A.Index, level, it.Quality, ctx, WeaponStatGroup.Main, scaleMainRecoveryValues: true);
					}
				}
			}
		}
		if (rerollDot && it.DOT != null)
		{
			for (int j = 0; j < it.DOT.Length; j++)
			{
				WPDT_A wPDT_A2 = it.DOT[j];
				if (wPDT_A2 != null && wPDT_A2.Index != 0)
				{
					float num2 = FindCraftBaseNumberA(tpl.DOT, tpl.RateDot, wPDT_A2.Index);
					if (num2 > 0f)
					{
						wPDT_A2.number = GenerateWeaponStatValue(num2, wPDT_A2.Index, level, it.Quality, ctx, WeaponStatGroup.Dot);
					}
				}
			}
		}
		if (rerollSkill && it.SK != null)
		{
			for (int k = 0; k < it.SK.Length; k++)
			{
				WPDT_B wPDT_B = it.SK[k];
				if (wPDT_B != null && !IsCsvNoneText(wPDT_B.SkillName))
				{
					float num3 = FindCraftBaseNumberB(tpl.SK, tpl.RateSK, wPDT_B);
					if (num3 > 0f)
					{
						wPDT_B.number = GenerateWeaponStatValue(num3, wPDT_B.Index, level, it.Quality, ctx, WeaponStatGroup.Skill);
					}
				}
			}
		}
		if (rerollComp && it.CP != null)
		{
			for (int l = 0; l < it.CP.Length; l++)
			{
				WPDT_B wPDT_B2 = it.CP[l];
				if (wPDT_B2 != null && !IsCsvNoneText(wPDT_B2.SkillName))
				{
					float num4 = FindCraftBaseNumberB(tpl.CP, tpl.RateCP, wPDT_B2);
					if (num4 > 0f)
					{
						wPDT_B2.number = GenerateWeaponStatValue(num4, wPDT_B2.Index, level, it.Quality, ctx, WeaponStatGroup.Companion);
					}
				}
			}
		}
		if (rerollSpc && it.SPC != null)
		{
			for (int m = 0; m < it.SPC.Count; m++)
			{
				WPSPC wPSPC = it.SPC[m];
				if (wPSPC != null && wPSPC.Index != 0)
				{
					wPSPC.PRC = GivePRC_SPC(level, it.Quality, ctx);
				}
			}
		}
		if (rerollElement)
		{
			ApplyElement(it, tpl.Element, level, ctx);
		}
	}

	private static float FindCraftBaseNumberA(WPDT_A[] fixedPool, WPDT_A[] ratePool, int index)
	{
		if (fixedPool != null)
		{
			for (int i = 0; i < fixedPool.Length; i++)
			{
				if (fixedPool[i] != null && fixedPool[i].Index == index)
				{
					return fixedPool[i].number;
				}
			}
		}
		if (ratePool != null)
		{
			for (int j = 0; j < ratePool.Length; j++)
			{
				if (ratePool[j] != null && ratePool[j].Index == index)
				{
					return ratePool[j].number;
				}
			}
		}
		return 0f;
	}

	private static float FindCraftBaseNumberB(WPDT_B[] fixedPool, WPDT_B[] ratePool, WPDT_B cur)
	{
		if (cur == null)
		{
			return 0f;
		}
		float result = 0f;
		bool flag = false;
		WPDT_B[][] array = new WPDT_B[2][] { fixedPool, ratePool };
		foreach (WPDT_B[] array2 in array)
		{
			if (array2 == null)
			{
				continue;
			}
			foreach (WPDT_B wPDT_B in array2)
			{
				if (wPDT_B != null && !IsCsvNoneText(wPDT_B.SkillName))
				{
					if (wPDT_B.SkillName == cur.SkillName && wPDT_B.Index == cur.Index)
					{
						return wPDT_B.number;
					}
					if (!flag && wPDT_B.SkillName == cur.SkillName)
					{
						result = wPDT_B.number;
						flag = true;
					}
				}
			}
		}
		return result;
	}
}
