using System.Collections.Generic;
using Core.Settings;
using Data.AutoGen.DataClass.Level;
using Data.SaveData;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour
{
	public List<EnemyPoint> point;

	public Text WPlevelText;

	public Text EMlevelText;

	private CanvasGroup canvas;

	private GameDataManager _gameDataManager;

	private ItemManager _itemManager;

	private PlayerManager PL;

	private InventoryManager IV;

	private LevelManager LV;

	private TalentManager _talentManager;

	private bool Opened;

	private int WPLevel;

	private int WPtype;

	private int Count;

	private int EMtype;

	private int EMlevel;

	private int EMCount;

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (!SettingsLoader.Instance.DebugMode)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		canvas = GetComponent<CanvasGroup>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		_itemManager = SingletonMonoScope<ItemManager>.Instance;
		PL = SingletonMonoScope<PlayerManager>.Instance;
		IV = SingletonMonoScope<InventoryManager>.Instance;
		LV = SingletonMonoScope<LevelManager>.Instance;
		_talentManager = SingletonMonoScope<TalentManager>.Instance;
		point.Clear();
		EnemyPoint[] array = Object.FindObjectsOfType<EnemyPoint>();
		foreach (EnemyPoint item in array)
		{
			point.Add(item);
		}
	}

	private void OnEnable()
	{
		if (SettingsLoader.Instance.DebugMode)
		{
			WPtype = 0;
			canvas.alpha = 0f;
			canvas.blocksRaycasts = false;
			Opened = false;
			EMCount = 8;
			EMlevel = 1;
			EMtype = 0;
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Init();
	}

	private void Update()
	{
		if (!SettingsLoader.Instance.DebugMode)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			if (Opened)
			{
				Opened = false;
				canvas.alpha = 0f;
				canvas.blocksRaycasts = false;
			}
			else
			{
				Opened = true;
				canvas.alpha = 1f;
				canvas.blocksRaycasts = true;
			}
		}
		if (point.Count == 0)
		{
			EnemyPoint[] array = Object.FindObjectsOfType<EnemyPoint>();
			foreach (EnemyPoint item in array)
			{
				point.Add(item);
			}
		}
	}

	public void AddPage()
	{
		for (int i = 0; i < 10; i++)
		{
			IV.CreatePage();
		}
	}

	public void ClearIV()
	{
		IV.DelAllItems();
	}

	public void AddSkill()
	{
		_talentManager.P_Base += 200;
	}

	public void LevelUP(int A)
	{
		switch (A)
		{
		case 0:
			PL.GainXp(10000);
			break;
		case 1:
			PL.GainXp(20000);
			break;
		case 2:
			PL.GainXp(80000);
			break;
		case 3:
			PL.GainXp(100000);
			break;
		case 4:
			PL.GainXp(3000000);
			break;
		case 5:
			PL.GainXp(10000000);
			break;
		case 6:
			PL.GainXp(30000000);
			break;
		case 7:
			PL.GainXp(60000000);
			break;
		case 8:
			PL.GainXp(900000000);
			break;
		}
	}

	public void PL_Die()
	{
		PL.SetPlayerDead();
	}

	public void PL_Reborn()
	{
		PL.SetPlayerReborn();
	}

	public void PL_Add(int A)
	{
		switch (A)
		{
		case 0:
			PL.Damage_Base *= 1.2f;
			break;
		case 1:
			PL.Health *= 1.2f;
			break;
		case 2:
			PL.Mana *= 1.2f;
			break;
		case 3:
			PL.CoolDown += 10f;
			break;
		case 4:
			PL.ThroughRate += 10f;
			break;
		case 5:
			PL.ATSpeed_Tmp += 10f;
			break;
		case 6:
			PL.MVSpeed_Tmp += 10f;
			break;
		case 7:
			PL.ItemDrop_Rate += 10f;
			break;
		}
	}

	public void PL_Del(int A)
	{
		switch (A)
		{
		case 0:
			PL.Damage_Base *= 0.9f;
			break;
		case 1:
			PL.Health *= 0.9f;
			break;
		case 2:
			PL.Mana *= 0.9f;
			break;
		case 3:
			PL.CoolDown -= 10f;
			break;
		case 4:
			PL.ThroughRate -= 10f;
			break;
		case 5:
			PL.ATSpeed_Tmp -= 10f;
			break;
		case 6:
			PL.MVSpeed_Tmp -= 10f;
			break;
		case 7:
			PL.ItemDrop_Rate -= 10f;
			break;
		}
	}

	public void ReloadWP()
	{
		_itemManager.ReLoad();
		_gameDataManager.ReLoadAllData();
	}

	public void SetWPlevel(int A)
	{
		WPLevel = A;
		WPlevelText.text = WPLevel.ToString();
	}

	public void SetWPlevelPlayer()
	{
		WPLevel = PL.Level;
		WPlevelText.text = WPLevel.ToString();
	}

	public void SetWPtype(int A)
	{
		WPtype = A;
	}

	public void CreatWP(int quality)
	{
		_itemManager.CreatSingleWeaponAll(WPLevel, WPtype, quality);
	}

	public void CreatWP_RD()
	{
		for (int i = 0; i < 50; i++)
		{
			_itemManager.CreatIVWeapon(PL.Level, 100f);
		}
	}

	public void CreatBS()
	{
		_itemManager.CreatBaoshiAll();
	}

	public void CreatFW_SK()
	{
		_itemManager.CreatFW_SK_All();
	}

	public void CreatFW_SPC()
	{
		_itemManager.CreatFW_SPC_All();
	}

	public void CreatFW_Basel()
	{
		_itemManager.CreatFW_Base_All();
	}

	public void CreatPT()
	{
		_itemManager.CreateUseAll();
	}

	public void SetEMlevel(int A)
	{
		EMlevel = A;
		EMlevelText.text = EMlevel.ToString();
	}

	public void SetEMcount(int A)
	{
		EMCount = A;
	}

	public void SetEMtype(int A)
	{
		EMtype = A;
	}

	public void CreatEM(int index)
	{
		foreach (EnemyPoint item in point)
		{
			switch (EMtype)
			{
			case 0:
				item.SpawnTestEnemy(_gameDataManager.EMMB[index].GlobalID, EMCount);
				break;
			case 1:
				item.SpawnTestJY();
				break;
			case 2:
				LV.CreatBoss(item, item.transform, _gameDataManager.BossMB[index].GlobalID);
				break;
			}
		}
	}

	public void SetCount(int A)
	{
		Count = A;
	}

	public void UnlockLevel()
	{
		if (!SaveManager.HasRuntime || SaveManager.RuntimeData == null)
		{
			LogUtil.Warn("ControlUI", "SaveRuntime 未初始化，无法解锁地图章节");
			return;
		}
		if (_gameDataManager == null)
		{
			_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		}
		if (_gameDataManager == null || _gameDataManager.levelList == null || _gameDataManager.levelList.Count == 0)
		{
			LogUtil.Warn("ControlUI", "关卡数据未初始化，无法解锁地图章节");
			return;
		}
		SaveData runtimeData = SaveManager.RuntimeData;
		if (runtimeData.UnlockedChapterIds == null)
		{
			runtimeData.UnlockedChapterIds = new HashSet<int>();
		}
		if (runtimeData.UnlockedLevelIds == null)
		{
			runtimeData.UnlockedLevelIds = new HashSet<string>();
		}
		int currentUnlockedChapterId = GetCurrentUnlockedChapterId(runtimeData.UnlockedChapterIds, runtimeData.UnlockedLevelIds);
		string chapterLastMainLevelId = GetChapterLastMainLevelId(currentUnlockedChapterId);
		bool flag = !string.IsNullOrEmpty(chapterLastMainLevelId) && runtimeData.UnlockedLevelIds.Contains(chapterLastMainLevelId);
		int num = currentUnlockedChapterId;
		if (flag)
		{
			int nextChapterId = GetNextChapterId(currentUnlockedChapterId);
			if (nextChapterId <= 0)
			{
				LogUtil.Success($"当前已解锁到第 {currentUnlockedChapterId} 章，且没有下一章节可解锁");
				return;
			}
			num = nextChapterId;
		}
		int num2 = UnlockChapterMaps(runtimeData.UnlockedChapterIds, runtimeData.UnlockedLevelIds, num);
		RefreshHomeTeleportStations();
		string arg = (flag ? $"第 {currentUnlockedChapterId} 章最后一关 {chapterLastMainLevelId} 已解锁，本次解锁第 {num} 章" : $"当前解锁到第 {currentUnlockedChapterId} 章，本次补齐到章节最后一关 {chapterLastMainLevelId}");
		LogUtil.Success($"{arg}，新增 {num2} 个关卡");
		SaveManager.RequestSave();
	}

	private int GetCurrentUnlockedChapterId(HashSet<int> unlockedChapterIds, HashSet<string> unlockedLevelIds)
	{
		int num = 1;
		if (unlockedChapterIds != null)
		{
			foreach (int unlockedChapterId in unlockedChapterIds)
			{
				if (unlockedChapterId > num)
				{
					num = unlockedChapterId;
				}
			}
		}
		if (unlockedLevelIds != null)
		{
			foreach (string unlockedLevelId in unlockedLevelIds)
			{
				int chapterId = LevelManager.GetChapterId(unlockedLevelId);
				if (chapterId > num)
				{
					num = chapterId;
				}
			}
		}
		return num;
	}

	private string GetChapterLastMainLevelId(int chapterId)
	{
		foreach (LevelData level in _gameDataManager.levelList)
		{
			if (level != null && !string.IsNullOrEmpty(level.GlobalID) && LevelManager.IsMainlineType(level.Type) && LevelManager.GetChapterId(level.GlobalID) == chapterId && LevelManager.IsLevelLastInItsChapter(level.GlobalID))
			{
				return level.GlobalID;
			}
		}
		return null;
	}

	private int GetNextChapterId(int currentChapterId)
	{
		int num = int.MaxValue;
		foreach (LevelData level in _gameDataManager.levelList)
		{
			if (level != null && !string.IsNullOrEmpty(level.GlobalID) && LevelManager.IsMainlineType(level.Type))
			{
				int chapterId = LevelManager.GetChapterId(level.GlobalID);
				if (chapterId > currentChapterId && chapterId < num)
				{
					num = chapterId;
				}
			}
		}
		if (num != int.MaxValue)
		{
			return num;
		}
		return -1;
	}

	private int UnlockChapterMaps(HashSet<int> unlockedChapterIds, HashSet<string> unlockedLevelIds, int chapterId)
	{
		unlockedChapterIds.Add(chapterId);
		int num = 0;
		foreach (LevelData level in _gameDataManager.levelList)
		{
			if (level != null && !string.IsNullOrEmpty(level.GlobalID) && LevelManager.GetChapterId(level.GlobalID) == chapterId && unlockedLevelIds.Add(level.GlobalID))
			{
				num++;
			}
		}
		return num;
	}

	private void RefreshHomeTeleportStations()
	{
		if (!SingletonMonoScene<HomeSceneManager>.HasInstance)
		{
			return;
		}
		foreach (TeleportStation station in SingletonMonoScene<HomeSceneManager>.Instance.stations)
		{
			if ((bool)station)
			{
				station.ControlInteractInHome();
			}
		}
	}
}
