using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.AutoGen.DataClass.Level;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using Level.LevelStates;
using Level.StateData.LevelStates;
using Mijing;
using Spine;
using Spine.Unity;
using UnityEngine;

public class LevelManager : SingletonMonoScope<LevelManager>
{
	public const string LEVEL_HOME = "Home";

	private static string CurLevel = "Home";

	public static LevelData CurLevelData;

	public List<NoSameRD> NoSameList = new List<NoSameRD>();

	public NoSameRD[] RDindex = new NoSameRD[16];

	[HideInInspector]
	public ColorGP GP;

	[HideInInspector]
	public SKprefab PB;

	[HideInInspector]
	public WDprefab WD;

	public float XPmulti;

	public float HealthMulti;

	public float DamageMulti;

	public GameObject MovePoint;

	private UniTaskCompletionSource _allPointsReadyTcs;

	public LevelPrefab levelPrefab;

	public static int SceneQulity { get; private set; }

	public void BeginLevel()
	{
		_allPointsReadyTcs = new UniTaskCompletionSource();
	}

	public UniTask WaitAllLevelPointsReadyAsync()
	{
		return _allPointsReadyTcs.Task;
	}

	public void NotifyAllLevelPointsReady()
	{
		_allPointsReadyTcs.TrySetResult();
	}

	public static bool ShouldPersistLevelState(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			return false;
		}
		if (levelId == "Home")
		{
			return true;
		}
		LevelData levelData = GetLevelData(levelId);
		if (levelData == null)
		{
			return false;
		}
		switch (levelData.Type)
		{
		case LevelType.Normal:
		case LevelType.Optional:
			return true;
		case LevelType.Boss:
		case LevelType.Challenge:
		case LevelType.Mijing:
			return false;
		default:
			return false;
		}
	}

	public static string GetCurLevelLocalKey()
	{
		return GetLevelData(CurLevel)?.LocalName;
	}

	public static string GetLevelLocalKey(string levelId)
	{
		return GetLevelData(levelId)?.LocalName;
	}

	private static bool TryParseOptionalLevelId(string levelId, out int optionalIndex)
	{
		optionalIndex = -1;
		if (string.IsNullOrEmpty(levelId))
		{
			return false;
		}
		if (levelId.Length < 2)
		{
			return false;
		}
		if (levelId[0] != 'Z' && levelId[0] != 'z')
		{
			return false;
		}
		return int.TryParse(levelId.Substring(1), out optionalIndex);
	}

	public static List<string> GetOptionalChildLevelIds(string parentMainId)
	{
		List<string> list = new List<string>();
		if (string.IsNullOrEmpty(parentMainId))
		{
			return list;
		}
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return list;
		}
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			string key = item.Key;
			LevelData value = item.Value;
			if (value != null && value.Type == LevelType.Optional && string.Equals(value.ParentMainId, parentMainId, StringComparison.Ordinal) && TryParseOptionalLevelId(key, out var _))
			{
				list.Add(key);
			}
		}
		list.Sort(delegate(string a, string b)
		{
			int optionalIndex2;
			bool flag = TryParseOptionalLevelId(a, out optionalIndex2);
			int optionalIndex3;
			bool flag2 = TryParseOptionalLevelId(b, out optionalIndex3);
			if (flag && flag2)
			{
				return optionalIndex2.CompareTo(optionalIndex3);
			}
			if (flag)
			{
				return -1;
			}
			return flag2 ? 1 : string.CompareOrdinal(a, b);
		});
		return list;
	}

	public static string GetFirstOptionalChildLevelId(string parentMainId)
	{
		List<string> optionalChildLevelIds = GetOptionalChildLevelIds(parentMainId);
		if (optionalChildLevelIds.Count <= 0)
		{
			return null;
		}
		return optionalChildLevelIds[0];
	}

	public static string GetOptionalChildLevelIdByOrder(string parentMainId, int order)
	{
		if (order < 0)
		{
			return null;
		}
		List<string> optionalChildLevelIds = GetOptionalChildLevelIds(parentMainId);
		if (order >= optionalChildLevelIds.Count)
		{
			return null;
		}
		return optionalChildLevelIds[order];
	}

	public static bool GetIsFinal()
	{
		return GetLevelData(CurLevel)?.IsFinal ?? false;
	}

	public static void SetCurLevel(string id)
	{
		SetCurLevel(id, -1);
	}

	public static void SetCurLevel(string id, int sceneQulity)
	{
		CurLevel = id;
		CurLevelData = GetLevelData(id);
		SceneQulity = ((sceneQulity >= 0) ? sceneQulity : GetDefaultSceneQulity(CurLevelData));
	}

	public static void SetSceneQulity(int sceneQulity)
	{
		SceneQulity = Mathf.Max(0, sceneQulity);
	}

	private static int GetDefaultSceneQulity(LevelData levelData)
	{
		if (levelData == null)
		{
			return 0;
		}
		if (levelData.Type == LevelType.Mijing)
		{
			if (!SingletonMonoScope<MijingManager>.HasInstance)
			{
				return 1;
			}
			return SingletonMonoScope<MijingManager>.Instance.GetCurrentSceneQulity();
		}
		return 0;
	}

	public static string GetCurLevel()
	{
		return CurLevel;
	}

	public static bool GetIsBoss()
	{
		LevelData levelData = GetLevelData(CurLevel);
		if (levelData != null)
		{
			return levelData.Type == LevelType.Boss;
		}
		return false;
	}

	public static bool GetIsOptionalById(string id)
	{
		LevelData levelData = GetLevelData(id);
		if (levelData != null)
		{
			return levelData.Type == LevelType.Optional;
		}
		return false;
	}

	public static string GetOptionalParentMainId(string id)
	{
		LevelData levelData = GetLevelData(id);
		if (levelData != null && levelData.Type == LevelType.Optional && !string.IsNullOrEmpty(levelData.ParentMainId))
		{
			return levelData.ParentMainId;
		}
		return null;
	}

	public static bool GetIsOptional()
	{
		return GetIsOptionalById(CurLevel);
	}

	public static bool GetIsChallenge()
	{
		LevelData levelData = GetLevelData(CurLevel);
		if (levelData != null)
		{
			return levelData.Type == LevelType.Challenge;
		}
		return false;
	}

	public static bool GetIsMijing()
	{
		LevelData levelData = GetLevelData(CurLevel);
		if (levelData != null)
		{
			return levelData.Type == LevelType.Mijing;
		}
		return false;
	}

	public static bool GetIsMijing(string id)
	{
		LevelData levelData = GetLevelData(id);
		if (levelData != null)
		{
			return levelData.Type == LevelType.Mijing;
		}
		return false;
	}

	public static int GetCurrentEnemyLevel()
	{
		int num = ((!SingletonMonoScope<PlayerManager>.HasInstance) ? 1 : SingletonMonoScope<PlayerManager>.Instance.Level);
		LevelData levelData = CurLevelData ?? GetLevelData(CurLevel);
		if (levelData == null)
		{
			return num;
		}
		if (levelData.Type == LevelType.Challenge || levelData.Type == LevelType.Mijing)
		{
			return num;
		}
		if (levelData.Type != 0 && levelData.Type != LevelType.Boss && levelData.Type != LevelType.Optional)
		{
			return num;
		}
		int num2 = Mathf.Max(1, levelData.MapLevel);
		int min = Mathf.Max(1, num2 - 5);
		return Mathf.Clamp(num, min, num2);
	}

	public static float GetEnemyHealthCurveMultiplier(int enemyLevel)
	{
		if (enemyLevel < 20)
		{
			return 1f;
		}
		if (enemyLevel <= 40)
		{
			float t = Mathf.InverseLerp(20f, 40f, enemyLevel);
			return Mathf.SmoothStep(1.03f, 1.3f, t);
		}
		if (enemyLevel <= 50)
		{
			float t2 = Mathf.InverseLerp(40f, 50f, enemyLevel);
			return Mathf.SmoothStep(1.3f, 1.03f, t2);
		}
		return 1.03f;
	}

	public static int GetCurChapterId()
	{
		return GetChapterId(CurLevel);
	}

	public static int GetChapterId(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			return -1;
		}
		if (string.Equals(levelId, "Home", StringComparison.OrdinalIgnoreCase) || string.Equals(levelId, "Home", StringComparison.OrdinalIgnoreCase))
		{
			return 0;
		}
		LevelData levelData = GetLevelData(levelId);
		if (levelData == null)
		{
			return -1;
		}
		if (TryParseMainLevelId(levelId, out var chapterId, out var levelIndex))
		{
			return chapterId;
		}
		if (levelData.Type == LevelType.Optional && !string.IsNullOrEmpty(levelData.ParentMainId) && TryParseMainLevelId(levelData.ParentMainId, out var chapterId2, out levelIndex))
		{
			return chapterId2;
		}
		if (!string.IsNullOrEmpty(levelData.ParentMainId) && TryParseMainLevelId(levelData.ParentMainId, out var chapterId3, out levelIndex))
		{
			return chapterId3;
		}
		return -1;
	}

	public static bool GetIsCurChapterFinal()
	{
		return IsLevelLastInItsChapter(CurLevel);
	}

	public static bool GetIsCurChapterFirst()
	{
		return IsLevelFirstInItsChapter(CurLevel);
	}

	public static bool IsLevelFirstInItsChapter(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			return false;
		}
		LevelData levelData = GetLevelData(levelId);
		if (levelData == null)
		{
			return false;
		}
		if (!IsMainlineType(levelData.Type))
		{
			return false;
		}
		if (!TryParseMainLevelId(levelId, out var chapterId, out var levelIndex))
		{
			return false;
		}
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return false;
		}
		int num = int.MaxValue;
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			string key = item.Key;
			LevelData value = item.Value;
			if (value != null && IsMainlineType(value.Type) && TryParseMainLevelId(key, out var chapterId2, out var levelIndex2) && chapterId2 == chapterId && levelIndex2 < num)
			{
				num = levelIndex2;
			}
		}
		return levelIndex == num;
	}

	public static string GetNextLevelId()
	{
		return GetNextMainLevelId(CurLevel);
	}

	public static string GetPrevLevelId()
	{
		return GetPrevMainLevelId(CurLevel);
	}

	public static string GetPrevMainLevelId(string levelId)
	{
		if (!TryParseMainLevelId(levelId, out var chapterId, out var levelIndex))
		{
			return null;
		}
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return null;
		}
		string result = null;
		int num = int.MinValue;
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			string key = item.Key;
			LevelData value = item.Value;
			if (value != null && IsMainlineType(value.Type) && TryParseMainLevelId(key, out var chapterId2, out var levelIndex2) && chapterId2 == chapterId && levelIndex2 < levelIndex && levelIndex2 > num)
			{
				num = levelIndex2;
				result = key;
			}
		}
		return result;
	}

	public static string GetNextMainLevelId(string levelId)
	{
		if (!TryParseMainLevelId(levelId, out var chapterId, out var levelIndex))
		{
			return null;
		}
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return null;
		}
		string result = null;
		int num = int.MaxValue;
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			string key = item.Key;
			LevelData value = item.Value;
			if (value != null && IsMainlineType(value.Type) && TryParseMainLevelId(key, out var chapterId2, out var levelIndex2) && chapterId2 == chapterId && levelIndex2 > levelIndex && levelIndex2 < num)
			{
				num = levelIndex2;
				result = key;
			}
		}
		return result;
	}

	public static string GetFirstMainLevelIdInNextChapter(string levelId)
	{
		if (!TryParseMainLevelId(levelId, out var chapterId, out var _))
		{
			return null;
		}
		int num = chapterId + 1;
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return null;
		}
		string result = null;
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			string key = item.Key;
			LevelData value = item.Value;
			if (value != null && IsMainlineType(value.Type) && TryParseMainLevelId(key, out var chapterId2, out var levelIndex2) && chapterId2 == num && levelIndex2 == 1)
			{
				result = key;
			}
		}
		return result;
	}

	public static bool IsLevelLastInItsChapter(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			return false;
		}
		LevelData levelData = GetLevelData(levelId);
		if (levelData == null)
		{
			return false;
		}
		if (!IsMainlineType(levelData.Type))
		{
			return false;
		}
		if (!TryParseMainLevelId(levelId, out var chapterId, out var levelIndex))
		{
			return false;
		}
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return false;
		}
		int num = -1;
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			string key = item.Key;
			LevelData value = item.Value;
			if (value != null && IsMainlineType(value.Type) && TryParseMainLevelId(key, out var chapterId2, out var levelIndex2) && chapterId2 == chapterId && levelIndex2 > num)
			{
				num = levelIndex2;
			}
		}
		return levelIndex == num;
	}

	public static bool IsMainlineType(LevelType type)
	{
		if (type != 0)
		{
			return type == LevelType.Boss;
		}
		return true;
	}

	private static bool TryParseMainLevelId(string levelId, out int chapterId, out int levelIndex)
	{
		chapterId = -1;
		levelIndex = -1;
		if (string.IsNullOrEmpty(levelId))
		{
			return false;
		}
		string[] array = levelId.Split('_');
		if (array.Length != 2)
		{
			return false;
		}
		if (!int.TryParse(array[0], out chapterId))
		{
			return false;
		}
		if (!int.TryParse(array[1], out levelIndex))
		{
			return false;
		}
		return true;
	}

	private static Dictionary<string, LevelData> GetAllLevelDatas()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return null;
		}
		return SingletonMonoScope<GameDataManager>.Instance.levelDatas;
	}

	public static List<string> GetAllMijingLevelIds()
	{
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		List<string> list = new List<string>();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return list;
		}
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			LevelData value = item.Value;
			if (value != null && !string.IsNullOrEmpty(value.GlobalID) && value.GlobalID.StartsWith("M"))
			{
				list.Add(item.Key);
			}
		}
		return list;
	}

	public static List<string> GetAllChallengeLevelIds()
	{
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		List<string> list = new List<string>();
		if (allLevelDatas == null || allLevelDatas.Count == 0)
		{
			return list;
		}
		foreach (KeyValuePair<string, LevelData> item in allLevelDatas)
		{
			LevelData value = item.Value;
			if (value != null && !string.IsNullOrEmpty(value.GlobalID) && value.GlobalID.StartsWith("C"))
			{
				list.Add(item.Key);
			}
		}
		return list;
	}

	public static LevelData GetLevelData(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		Dictionary<string, LevelData> allLevelDatas = GetAllLevelDatas();
		if (allLevelDatas == null)
		{
			return null;
		}
		allLevelDatas.TryGetValue(id, out var value);
		return value;
	}

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			SingletonMonoScope<GameDataManager>.Instance.InitData();
		}
		if (!levelPrefab)
		{
			levelPrefab = Singleton<ResManager>.Instance.Load<LevelPrefab>("World/Map/Levels/LevelPrefab");
		}
		CurLevelData = GetLevelData(CurLevel);
		GP = SingletonMonoScope<GameDataManager>.Instance.colorGP;
		PB = SingletonMonoScope<GameDataManager>.Instance.SKPB;
		WD = SingletonMonoScope<GameDataManager>.Instance.WDPB;
		RDindex[0].Index = 1;
		RDindex[1].Index = 2;
		RDindex[2].Index = 3;
		RDindex[3].Index = 4;
		RDindex[4].Index = 5;
		RDindex[5].Index = 6;
		RDindex[6].Index = 7;
		RDindex[7].Index = 8;
		RDindex[8].Index = 9;
		RDindex[9].Index = 10;
		RDindex[10].Index = 11;
		RDindex[11].Index = 12;
		RDindex[12].Index = 13;
		RDindex[13].Index = 14;
		RDindex[14].Index = 15;
		RDindex[15].Index = 16;
		XPmulti = 1.073f;
		HealthMulti = 1.144f;
		DamageMulti = 1.07f;
	}

	public void BreakSP(int a, int number, Transform trans)
	{
		switch (a)
		{
		case 0:
			LeanPool.Spawn(PB.spBreak[number], trans.position, trans.rotation, base.transform);
			break;
		case 1:
			LeanPool.Spawn(PB.spBreak[number], trans.position, trans.rotation, base.transform);
			break;
		case 2:
			LeanPool.Spawn(PB.spBreak[number], trans.position, trans.rotation, base.transform);
			break;
		case 3:
			LeanPool.Spawn(PB.spKuang[number], trans.position, trans.rotation, base.transform);
			break;
		case 4:
			LeanPool.Spawn(PB.spBaoshi[number], trans.position, trans.rotation, base.transform);
			break;
		}
	}

	public void BreakFlower(int a, Transform trans)
	{
		LeanPool.Spawn(PB.spFlower[a], trans.position, trans.rotation, base.transform);
	}

	public GameObject CreatMovePoint(Vector3 pos)
	{
		return LeanPool.Spawn(MovePoint, pos, Quaternion.identity);
	}

	public static EnemyMB GetMB(int id)
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || SingletonMonoScope<GameDataManager>.Instance.EMMB == null)
		{
			return null;
		}
		foreach (EnemyMB item in SingletonMonoScope<GameDataManager>.Instance.EMMB)
		{
			if (item != null && item.GlobalID == id)
			{
				return item;
			}
		}
		return null;
	}

	private static bool TryGetRandomMonsterMBFromPool(List<int> monsterIds, string logContext, out int id, out EnemyMB mb)
	{
		id = 0;
		mb = null;
		if (monsterIds == null || monsterIds.Count == 0)
		{
			LogUtil.Warn("[" + logContext + "] 当前地图怪物库为空");
			return false;
		}
		int num = (id = monsterIds[UnityEngine.Random.Range(0, monsterIds.Count)]);
		mb = GetMB(id);
		if (mb != null)
		{
			return true;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < monsterIds.Count; i++)
		{
			int num2 = monsterIds[i];
			if (num2 != num && GetMB(num2) != null)
			{
				list.Add(num2);
			}
		}
		if (list.Count == 0)
		{
			LogUtil.Warn($"[{logContext}] 怪物 id = {num} 查不到，当前地图怪物库也没有其他有效怪物");
			return false;
		}
		id = list[UnityEngine.Random.Range(0, list.Count)];
		mb = GetMB(id);
		LogUtil.Warn($"[{logContext}] 怪物 id = {num} 查不到，已重新随机为 id = {id}");
		return mb != null;
	}

	public List<Enemy> CreatTest(Transform trans, int id, int Count)
	{
		EnemyMB mB = GetMB(id);
		int num;
		int num2;
		if (CurLevelData.MapLevel <= 30)
		{
			num = 70;
			num2 = 25;
		}
		else if (CurLevelData.MapLevel > 30 && CurLevelData.MapLevel <= 60)
		{
			num = 60;
			num2 = 35;
		}
		else if (CurLevelData.MapLevel > 60 && CurLevelData.MapLevel <= 80)
		{
			num = 50;
			num2 = 40;
		}
		else if (CurLevelData.MapLevel > 80 && CurLevelData.MapLevel < 100)
		{
			num = 40;
			num2 = 45;
		}
		else if (CurLevelData.IsMJ)
		{
			num = 30 - CurLevelData.JY_Rate;
			num2 = 50 + Mathf.FloorToInt((float)CurLevelData.JY_Rate * 0.5f);
		}
		else
		{
			num = 30;
			num2 = 50;
		}
		int num3 = UnityEngine.Random.Range(0, 101);
		int num4 = ((num3 > num) ? ((num3 > num && num3 <= num + num2) ? 1 : 2) : 0);
		int healthMulti;
		int damageMulti;
		switch (num4)
		{
		case 0:
			healthMulti = 0;
			damageMulti = 0;
			break;
		case 1:
			healthMulti = 60;
			damageMulti = 50;
			break;
		default:
			healthMulti = 150;
			damageMulti = 100;
			break;
		}
		int a = num4 switch
		{
			0 => 0, 
			1 => UnityEngine.Random.Range(1, 3), 
			_ => UnityEngine.Random.Range(2, 4), 
		};
		int[] array = new int[5];
		for (int i = 0; i < 14; i++)
		{
			NoSameList.Add(RDindex[i]);
		}
		for (int j = 0; j < 5; j++)
		{
			array[j] = 0;
		}
		int num5 = Mathf.Min(a, array.Length);
		for (int k = 0; k < num5; k++)
		{
			if (NoSameList.Count == 0)
			{
				break;
			}
			int index = UnityEngine.Random.Range(0, NoSameList.Count);
			array[k] = NoSameList[index].Index;
			NoSameList.RemoveAt(index);
		}
		int num6 = UnityEngine.Random.Range(0, 6);
		int randomColor = UnityEngine.Random.Range(0, GP.GP[mB.ColorIndex].XI[num6].CL.Length);
		int mainIndex = UnityEngine.Random.Range(0, 3);
		EM_Skill_SP sK_AT = ((CurLevelData.MapLevel <= 30) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.AT1].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.AT1].SK.Count)] : ((CurLevelData.MapLevel <= 30 || CurLevelData.MapLevel > 60) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.AT3].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.AT3].SK.Count)] : SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.AT2].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.AT2].SK.Count)]));
		EM_Skill_SP sK_A = ((CurLevelData.MapLevel <= 20) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK1].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK1].SK.Count)] : ((CurLevelData.MapLevel > 20 && CurLevelData.MapLevel <= 40) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK2].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK2].SK.Count)] : ((CurLevelData.MapLevel > 40 && CurLevelData.MapLevel <= 60) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK3].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK3].SK.Count)] : ((CurLevelData.MapLevel <= 60 || CurLevelData.MapLevel > 80) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK5].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK5].SK.Count)] : SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK4].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mB.SK4].SK.Count)]))));
		EM_Skill_SP sK_Die = SingletonMonoScope<GameDataManager>.Instance.SKG_Die[mB.SK_Die_Index].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_Die[mB.SK_Die_Index].SK.Count)];
		EM_Skill_SP sK_ELSS = null;
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			GameDataManager instance = SingletonMonoScope<GameDataManager>.Instance;
			if ((bool)instance && instance.SKG_ELSS != null && mB.ELSS_Index >= 0 && mB.ELSS_Index < instance.SKG_ELSS.Length && instance.SKG_ELSS[mB.ELSS_Index] != null && instance.SKG_ELSS[mB.ELSS_Index].SK != null && instance.SKG_ELSS[mB.ELSS_Index].SK.Count > 0)
			{
				List<EM_Skill_SP> sK = instance.SKG_ELSS[mB.ELSS_Index].SK;
				sK_ELSS = sK[UnityEngine.Random.Range(0, sK.Count)];
			}
			else
			{
				LogUtil.Warn($"元素增强技能随机失败，ELSS_Index = {mB.ELSS_Index}");
			}
		}
		else
		{
			LogUtil.Warn("GameDataManager无实例！");
		}
		int fS_Count_Add = UnityEngine.Random.Range(1, 6);
		List<Enemy> list = new List<Enemy>();
		for (int l = 0; l < Count; l++)
		{
			Enemy enemy = SpawnNewEnemy(id, Count, trans, num4, mainIndex, num6, randomColor, healthMulti, damageMulti, array, sK_AT, sK_A, sK_Die, sK_ELSS, fS_Count_Add);
			SaveEnemyToState(id, enemy, num4, mainIndex, num6, randomColor, healthMulti, damageMulti, array, sK_AT, sK_A, sK_Die, sK_ELSS, fS_Count_Add, enemy.SK_Comp, enemy.SK_FS);
			list.Add(enemy);
		}
		NoSameList.Clear();
		return list;
	}

	public Enemy RestoreEnemy(EnemyState state)
	{
		EnemyMB mB = GetMB(state.MonsterId);
		Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.EMPB.Enemy[mB.IndexA].Enemy[mB.IndexB], state.Position, Quaternion.identity).GetComponent<Enemy>();
		SetEnemyData(state.MonsterId, component, state.QQ, state.MainIndex, state.EL, state.randomColor, state.healthMulti, state.damageMulti, state.SS, state.SK_AT, state.SK_A, state.SK_Die, state.SK_ELSS, state.FS_Count_Add, state.SK_Comp, state.SK_FS, isRestore: true);
		component.RuntimeState = state;
		component.HealthStat.SetCurrent(state.Hp);
		return component;
	}

	public List<Enemy> CreatEnemies(Transform trans)
	{
		if (!TryGetRandomMonsterMBFromPool(CurLevelData.Enemy_list, "LevelManager.CreatEnemies", out var id, out var mb))
		{
			return new List<Enemy>();
		}
		int num;
		int num2;
		if (CurLevelData.MapLevel <= 30)
		{
			num = 70;
			num2 = 25;
		}
		else if (CurLevelData.MapLevel > 30 && CurLevelData.MapLevel <= 60)
		{
			num = 60;
			num2 = 35;
		}
		else if (CurLevelData.MapLevel > 60 && CurLevelData.MapLevel <= 80)
		{
			num = 50;
			num2 = 40;
		}
		else if (CurLevelData.MapLevel > 80 && CurLevelData.MapLevel < 100)
		{
			num = 40;
			num2 = 45;
		}
		else if (CurLevelData.IsMJ)
		{
			num = 30 - CurLevelData.JY_Rate;
			num2 = 50 + Mathf.FloorToInt((float)CurLevelData.JY_Rate * 0.5f);
		}
		else
		{
			num = 30;
			num2 = 50;
		}
		int num3 = UnityEngine.Random.Range(0, 101);
		int num4 = ((num3 > num) ? ((num3 > num && num3 <= num + num2) ? 1 : 2) : 0);
		int num5 = 1;
		int num6 = ((!SingletonMonoScope<PlayerManager>.HasInstance) ? 1 : SingletonMonoScope<PlayerManager>.Instance.Level);
		switch (num4)
		{
		case 0:
			if (SingletonMonoScope<PlayerManager>.HasInstance)
			{
				num5 = ((num6 <= 10) ? UnityEngine.Random.Range(7, 12) : ((num6 > 20) ? UnityEngine.Random.Range(14, 18) : UnityEngine.Random.Range(10, 15)));
			}
			break;
		case 1:
			if (SingletonMonoScope<PlayerManager>.HasInstance)
			{
				num5 = ((num6 <= 10) ? UnityEngine.Random.Range(5, 9) : ((num6 > 20) ? UnityEngine.Random.Range(8, 12) : UnityEngine.Random.Range(6, 11)));
			}
			break;
		case 2:
			if (SingletonMonoScope<PlayerManager>.HasInstance)
			{
				num5 = ((num6 <= 10) ? UnityEngine.Random.Range(1, 4) : ((num6 > 20) ? UnityEngine.Random.Range(4, 8) : UnityEngine.Random.Range(2, 6)));
			}
			break;
		}
		int healthMulti;
		int damageMulti;
		switch (num4)
		{
		case 0:
			healthMulti = 0;
			damageMulti = 0;
			break;
		case 1:
			healthMulti = 60;
			damageMulti = 50;
			break;
		default:
			healthMulti = 150;
			damageMulti = 100;
			break;
		}
		int a = num4 switch
		{
			0 => 0, 
			1 => UnityEngine.Random.Range(1, 3), 
			_ => UnityEngine.Random.Range(2, 4), 
		};
		int[] array = new int[5];
		for (int i = 0; i < 14; i++)
		{
			NoSameList.Add(RDindex[i]);
		}
		for (int j = 0; j < 5; j++)
		{
			array[j] = 0;
		}
		int num7 = Mathf.Min(a, array.Length);
		for (int k = 0; k < num7; k++)
		{
			if (NoSameList.Count == 0)
			{
				break;
			}
			int index = UnityEngine.Random.Range(0, NoSameList.Count);
			array[k] = NoSameList[index].Index;
			NoSameList.RemoveAt(index);
		}
		int num8 = UnityEngine.Random.Range(0, 6);
		int randomColor = 0;
		if (mb == null)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] GetMB 返回 null，怪物 id = {id}");
		}
		else if (!GP)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] GP 为 null，怪物 id = {id}");
		}
		else if (GP.GP == null)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] GP.GP 为 null，怪物 id = {id}");
		}
		else if (mb.ColorIndex < 0 || mb.ColorIndex >= GP.GP.Length)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] ColorIndex 越界，怪物 id = {id}, ColorIndex = {mb.ColorIndex}, GP.GP.Length = {GP.GP.Length}");
		}
		else if (GP.GP[mb.ColorIndex] == null)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] GP.GP[{mb.ColorIndex}] 为 null，怪物 id = {id}");
		}
		else if (GP.GP[mb.ColorIndex].XI == null)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] XI 为 null，怪物 id = {id}, ColorIndex = {mb.ColorIndex}");
		}
		else if (num8 < 0 || num8 >= GP.GP[mb.ColorIndex].XI.Length)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] EL 越界，怪物 id = {id}, EL = {num8}, XI.Length = {GP.GP[mb.ColorIndex].XI.Length}");
		}
		else if (GP.GP[mb.ColorIndex].XI[num8] == null)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] XI[{num8}] 为 null，怪物 id = {id}, ColorIndex = {mb.ColorIndex}");
		}
		else if (GP.GP[mb.ColorIndex].XI[num8].CL == null || GP.GP[mb.ColorIndex].XI[num8].CL.Length == 0)
		{
			LogUtil.Warn($"[LevelManager.CreatEnemies] CL 未配置，怪物 id = {id}, ColorIndex = {mb.ColorIndex}, EL = {num8}");
		}
		else
		{
			randomColor = UnityEngine.Random.Range(0, GP.GP[mb.ColorIndex].XI[num8].CL.Length);
		}
		int mainIndex = UnityEngine.Random.Range(0, 3);
		EM_Skill_SP sK_AT = null;
		GameDataManager instance = SingletonMonoScope<GameDataManager>.Instance;
		if (SingletonMonoScope<GameDataManager>.HasInstance && (bool)instance && instance.SKG_ELSS != null && mb != null)
		{
			int num9 = ((CurLevelData.MapLevel <= 30) ? mb.AT1 : ((CurLevelData.MapLevel > 60) ? mb.AT3 : mb.AT2));
			if (num9 >= 0 && instance.SKG_ELSS[num9] != null && instance.SKG_ELSS[num9].SK != null && instance.SKG_ELSS[num9].SK.Count > 0)
			{
				List<EM_Skill_SP> sK = instance.SKG_ELSS[num9].SK;
				sK_AT = sK[UnityEngine.Random.Range(0, sK.Count)];
			}
			else
			{
				LogUtil.Warn(string.Format("敌人普通攻击配置异常，怪物={0}，攻击组索引={1}", (mb != null) ? mb.IndexName.ToString() : "null", num9));
			}
		}
		else
		{
			LogUtil.Warn("敌人普通攻击随机失败：GameDataManager / SKG_ELSS / mb 有空引用");
		}
		EM_Skill_SP sK_A = ((CurLevelData.MapLevel <= 20) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK1].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK1].SK.Count)] : ((CurLevelData.MapLevel > 20 && CurLevelData.MapLevel <= 40) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK2].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK2].SK.Count)] : ((CurLevelData.MapLevel > 40 && CurLevelData.MapLevel <= 60) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK3].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK3].SK.Count)] : ((CurLevelData.MapLevel <= 60 || CurLevelData.MapLevel > 80) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK5].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK5].SK.Count)] : SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK4].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK4].SK.Count)]))));
		EM_Skill_SP sK_Die = SingletonMonoScope<GameDataManager>.Instance.SKG_Die[mb.SK_Die_Index].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_Die[mb.SK_Die_Index].SK.Count)];
		EM_Skill_SP sK_ELSS = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.ELSS_Index].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.ELSS_Index].SK.Count)];
		int fS_Count_Add = UnityEngine.Random.Range(1, 6);
		List<Enemy> list = new List<Enemy>();
		for (int l = 0; l < num5; l++)
		{
			Enemy enemy = SpawnNewEnemy(id, num5, trans, num4, mainIndex, num8, randomColor, healthMulti, damageMulti, array, sK_AT, sK_A, sK_Die, sK_ELSS, fS_Count_Add);
			SaveEnemyToState(id, enemy, num4, mainIndex, num8, randomColor, healthMulti, damageMulti, array, sK_AT, sK_A, sK_Die, sK_ELSS, fS_Count_Add, enemy.SK_Comp, enemy.SK_FS);
			list.Add(enemy);
		}
		NoSameList.Clear();
		return list;
	}

	public Enemy SpawnNewEnemy(int id, int Count, Transform trans, int QQ, int MainIndex, int EL, int randomColor, int healthMulti, int damageMulti, int[] SS, EM_Skill_SP SK_AT, EM_Skill_SP SK_A, EM_Skill_SP SK_Die, EM_Skill_SP SK_ELSS, int FS_Count_Add)
	{
		EnemyMB mB = GetMB(id);
		Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.EMPB.Enemy[mB.IndexA].Enemy[mB.IndexB], new Vector3(trans.position.x + UnityEngine.Random.Range(0f - FanWei(Count), FanWei(Count)), trans.position.y + UnityEngine.Random.Range(0f - FanWei(Count), FanWei(Count)), trans.position.z), Quaternion.identity).GetComponent<Enemy>();
		SetEnemyData(id, component, QQ, MainIndex, EL, randomColor, healthMulti, damageMulti, SS, SK_AT, SK_A, SK_Die, SK_ELSS, FS_Count_Add, null, null, isRestore: false);
		return component;
	}

	private static void SaveEnemyToState(int id, Enemy em, int QQ, int MainIndex, int EL, int randomColor, int healthMulti, int damageMulti, int[] SS, EM_Skill_SP SK_AT, EM_Skill_SP SK_A, EM_Skill_SP SK_Die, EM_Skill_SP SK_ELSS, int FS_Count_Add, EM_Skill_CP SK_Comp, EM_Skill_FS SK_FS)
	{
		EnemyState runtimeState = new EnemyState
		{
			MonsterId = id,
			Position = em.transform.position,
			QQ = QQ,
			MainIndex = MainIndex,
			EL = EL,
			randomColor = randomColor,
			healthMulti = healthMulti,
			damageMulti = damageMulti,
			SS = (int[])SS.Clone(),
			SK_AT = SK_AT,
			SK_A = SK_A,
			SK_Die = SK_Die,
			SK_ELSS = SK_ELSS,
			FS_Count_Add = FS_Count_Add,
			SK_Comp = SK_Comp,
			SK_FS = SK_FS,
			Hp = 0f,
			IsDead = false
		};
		em.RuntimeState = runtimeState;
	}

	public void SetEnemyData(int id, Enemy em, int QQ, int MainIndex, int EL, int randomColor, int healthMulti, int damageMulti, int[] SS, EM_Skill_SP SK_AT, EM_Skill_SP SK_A, EM_Skill_SP SK_Die, EM_Skill_SP SK_ELSS, int FS_Count_Add, EM_Skill_CP SK_Comp, EM_Skill_FS SK_FS, bool isRestore)
	{
		EnemyMB mB = GetMB(id);
		em.GlobalID = mB.GlobalID;
		em.Quality = QQ;
		em.IndexName = mB.IndexName[MainIndex];
		em.Level = GetCurrentEnemyLevel();
		em.Xp = GetXP(QQ, mB.Xp, em.Level);
		em.size = mB.size;
		em.CompOffset = mB.CompOffset;
		em.TuiSpeed = mB.TuiSpeed;
		em.ItemDropPos = mB.ItemDropPos;
		em.MainElement = EL;
		switch (em.MainElement)
		{
		case 0:
			em.MainELType = DamageType.fire;
			break;
		case 1:
			em.MainELType = DamageType.frozen;
			break;
		case 2:
			em.MainELType = DamageType.thunder;
			break;
		case 3:
			em.MainELType = DamageType.poison;
			break;
		case 4:
			em.MainELType = DamageType.physics;
			break;
		case 5:
			em.MainELType = DamageType.shadow;
			break;
		}
		em.ColorIndex = mB.ColorIndex;
		em.SpineType = mB.SpineType;
		em.CP_FX = mB.CP_FX;
		if (em.SpineType == 0)
		{
			EnemyColorDT enemyColorDT = GP.GP[mB.ColorIndex].XI[em.MainElement].CL[randomColor];
			SkeletonAnimation spine = em.spine;
			if (enemyColorDT.ChangeSK)
			{
				Skin skin = new Skin("skin");
				skin.Clear();
				skin.AddSkin(spine.Skeleton.Data.FindSkin(enemyColorDT.SkinName));
				spine.Skeleton.SetSkin(skin);
				spine.Skeleton.SetSlotsToSetupPose();
			}
			em.SkinName = enemyColorDT.SkinName;
			em.Flip = enemyColorDT.Flip;
			em.MainMix = enemyColorDT.MainMix;
			em.MainHue = enemyColorDT.MainHue;
			em.MainSat = enemyColorDT.MainSat;
			em.MainColor = enemyColorDT.MainColor;
			em.DisloveColor = enemyColorDT.DisloveColor;
			em.AlphaColor = enemyColorDT.AlphaColor;
			em.DieColor = enemyColorDT.DieColor;
			em.RDcolor = randomColor;
		}
		else
		{
			em.SetSpiritColor(em.MainElement);
			em.DieColor = EL;
		}
		if ((bool)em.FXsustain)
		{
			em.FXsustain.SetColor(em.MainElement);
		}
		em.EnemyType = mB.EnemyType;
		em.Health_Base = Mathf.Floor(mB.Health * Mathf.Pow(HealthMulti, em.Level) * GetEnemyHealthCurveMultiplier(em.Level) * GetHeal());
		em.Health_Bei = 0f;
		SetEnemyBaseData(em);
		em.AttackSpeed_JG = mB.AttackSpeed_JG;
		em.AttackSpeed_Base = mB.ATSpeed;
		em.AttackSpeed_Bei = 0f;
		em.MoveSpeed_Base = mB.MVSpeed;
		em.MoveSpeed_Bei = 0f;
		em.Damage_Base = Mathf.Floor(mB.Damage * Mathf.Pow(DamageMulti, em.Level) * GetDMG());
		em.Damage_Bei = 0f;
		em.Chuan = GetChuan();
		em.FireAnti = GetAnti();
		em.FrozenAnti = GetAnti();
		em.ThunderAnti = GetAnti();
		em.PoisonAnti = GetAnti();
		em.PhysicsAnti = GetAnti();
		em.ShadowAnti = GetAnti();
		em.DamageAnti += GetDMG_Anti();
		em.FlySpeed = 0f;
		em.Range_Base = mB.Range_Base;
		em.Range_Anger = mB.Range_Anger;
		em.Range_Far = mB.Range_Far;
		float num = em.Level;
		em.SK_Rate = mB.SK_Rate + Mathf.FloorToInt(num / 20f);
		em.SK_Rate_ELSS = 0;
		em.Can_DieBoom = false;
		em.SPtype = mB.SPtype;
		em.Die_Index = mB.Die_Index;
		em.DieType = mB.DieType;
		em.DiePos = mB.DiePos;
		em.DieFX_TimeDelay = mB.DieFX_TimeDelay;
		em.DieDelay = mB.DieDelay;
		em.Lie_Index = mB.Lie_Index;
		em.LiePos = mB.LiePos;
		em.FSDie_Index = mB.FSDie_Index;
		em.Idle_Time_Min = mB.Idle_Time_Min;
		em.Idle_Time_Max = mB.Idle_Time_Max;
		em.SO_IdleRate = mB.SO_IdleRate;
		em.SO_AttackRate = mB.SO_AttackRate;
		em.SO_SayRate = mB.SO_SayRate;
		em.SO_HurtRate = mB.SO_HurtRate;
		em.SO_DieRate = mB.SO_DieRate;
		em.SO_Idle = mB.SO_Idle;
		em.SO_Walk = mB.SO_Walk;
		em.SO_AttackA = mB.SO_AttackA;
		em.SO_SayA = mB.SO_SayA;
		em.SO_AttackB = mB.SO_AttackB;
		em.SO_SayB = mB.SO_SayB;
		em.SO_AttackC = mB.SO_AttackC;
		em.SO_SayC = mB.SO_SayC;
		em.SO_Hurt = mB.SO_Hurt;
		em.SO_Die = mB.SO_Die;
		em.SO_ChuiDi = mB.SO_ChuiDi;
		em.IS_Boss = false;
		em.IS_Comp = false;
		em.IS_FS = false;
		em.IsDpsTarget = false;
		em.CF_Rate = mB.CF_Rate;
		em.HitFX = mB.HitFX;
		if (em.SK_Comp == null)
		{
			em.SK_Comp = new EM_Skill_CP();
		}
		if (em.SK_FS == null)
		{
			em.SK_FS = new EM_Skill_FS();
		}
		em.SK_AT = SK_AT;
		em.AT_Ani = mB.AT_Ani;
		em.AT_Fang = mB.AT_Fang;
		em.AT_Distans = mB.AT_Distans * SWS.DistanceRandom(mB.AT_Distans);
		em.SK_A = SK_A;
		em.SK_Ani = mB.SK_Ani;
		em.SK_Fang = mB.SK_Fang;
		em.SK_Distans = mB.SK_Distans * SWS.DistanceRandom(mB.SK_Distans);
		if (isRestore)
		{
			if (SK_Comp != null)
			{
				SetSkillData_CP(em.SK_Comp, SK_Comp);
			}
			if (SK_FS != null)
			{
				SetSkillData_FS(em.SK_FS, SK_FS);
			}
		}
		else
		{
			SetSkillData_CP(em.SK_Comp, mB.SK_Comp);
			SetSkillData_FS(em.SK_FS, mB.SK_FS);
		}
		em.SK_Die = SK_Die;
		em.SK_ELSS = SK_ELSS;
		em.ELSS_Ani = mB.ELSS_Ani;
		em.ELSS_Fang = mB.ELSS_Fang;
		em.ELSS_Distans = mB.ELSS_Distans * SWS.DistanceRandom(mB.ELSS_Distans);
		em.Health_Bei += healthMulti;
		em.Damage_Bei += damageMulti;
		ApplyEnemyQualitySoundRate(em);
		for (int i = 0; i < em.SSIndex.Length; i++)
		{
			em.SSIndex[i] = SS[i];
		}
		int[] sSIndex = em.SSIndex;
		for (int j = 0; j < sSIndex.Length; j++)
		{
			switch (sSIndex[j])
			{
			case 1:
				em.Damage_Bei += 100f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[0], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 2:
				em.BJRate += 30f;
				em.Chuan += 30f;
				em.FlySpeed += 20f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[1], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 3:
				em.DotDamage += 100f;
				em.DotTime += 100f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[2], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 4:
				em.AttackSpeed_Bei += 50f;
				em.MoveSpeed_Bei += 50f;
				em.AntiSlow += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[3], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 5:
				em.SK_Rate += 25;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[4], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 6:
				em.FS_Count += FS_Count_Add;
				em.SK_Rate_FS += 20;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[5], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 7:
				em.Health_Bei += 100f;
				em.MoveSpeed_Bei += 20f;
				em.AntiSlow += 10f;
				em.yunAnti += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[6], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 8:
				em.Health_Prc += 3f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[7], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 9:
				em.DamageAnti += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[8], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 10:
				em.FireAnti += 30f;
				em.FrozenAnti += 30f;
				em.ThunderAnti += 30f;
				em.PoisonAnti += 30f;
				em.PhysicsAnti += 30f;
				em.ShadowAnti += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[9], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 11:
				em.DotTimeCut += 50f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[10], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 12:
				em.Can_DieBoom = true;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[11], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 13:
				em.SK_Rate_ELSS += 30;
				switch (em.MainELType)
				{
				case DamageType.fire:
					em.FireAnti += 30f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[0], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.frozen:
					em.FrozenAnti += 30f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[1], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.thunder:
					em.ThunderAnti += 30f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[2], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.poison:
					em.PoisonAnti += 30f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[3], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.physics:
					em.PhysicsAnti += 30f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[4], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.shadow:
					em.ShadowAnti += 30f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[5], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				}
				break;
			}
		}
	}

	public Enemy RestoreJY(EnemyState state)
	{
		EnemyMB mB = GetMB(state.MonsterId);
		Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.EMPB.Enemy[mB.IndexA].Enemy[mB.IndexB], state.Position, Quaternion.identity).GetComponent<Enemy>();
		SetJYData(state.MonsterId, component, state.MainIndex, state.SS, state.EL, state.randomColor, state.SK_AT, state.SK_A, state.SK_Die, state.SK_ELSS, state.Qi, state.FS_Count_Add, state.Comp_Count_Add, state.SK_Comp, state.SK_FS, isRestore: true);
		component.RuntimeState = state;
		component.HealthStat.SetCurrent(state.Hp);
		return component;
	}

	public List<Enemy> CreatJYs(Transform trans)
	{
		if (!TryGetRandomMonsterMBFromPool(CurLevelData.JY_list, "LevelManager.CreatJYs", out var id, out var mb))
		{
			return new List<Enemy>();
		}
		int level = SingletonMonoScope<PlayerManager>.Instance.Level;
		int num = ((level <= 10) ? UnityEngine.Random.Range(1, 2) : ((level > 20) ? UnityEngine.Random.Range(1, 4) : UnityEngine.Random.Range(1, 3)));
		int num2 = UnityEngine.Random.Range(3, 5);
		int[] array = new int[5];
		NoSameRD[] rDindex = RDindex;
		foreach (NoSameRD item in rDindex)
		{
			NoSameList.Add(item);
		}
		for (int j = 0; j < num2; j++)
		{
			array[j] = 0;
		}
		for (int k = 0; k < num2; k++)
		{
			int index = UnityEngine.Random.Range(1, RDindex.Length - k);
			array[k] = NoSameList[index].Index;
			NoSameList.Remove(NoSameList[index]);
		}
		int mainIndex = UnityEngine.Random.Range(0, 3);
		int num3 = UnityEngine.Random.Range(0, 6);
		int randomColor = UnityEngine.Random.Range(0, GP.GP[mb.ColorIndex].XI[num3].CL.Length);
		EM_Skill_SP sK_AT = ((CurLevelData.MapLevel <= 30) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.AT1].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.AT1].SK.Count)] : ((CurLevelData.MapLevel <= 30 || CurLevelData.MapLevel > 60) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.AT3].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.AT3].SK.Count)] : SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.AT2].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.AT2].SK.Count)]));
		EM_Skill_SP sK_A = ((CurLevelData.MapLevel <= 20) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK1].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK1].SK.Count)] : ((CurLevelData.MapLevel > 20 && CurLevelData.MapLevel <= 40) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK2].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK2].SK.Count)] : ((CurLevelData.MapLevel > 40 && CurLevelData.MapLevel <= 60) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK3].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK3].SK.Count)] : ((CurLevelData.MapLevel <= 60 || CurLevelData.MapLevel > 80) ? SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK5].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK5].SK.Count)] : SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK4].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS[mb.SK4].SK.Count)]))));
		EM_Skill_SP sK_Die = SingletonMonoScope<GameDataManager>.Instance.SKG_Die[mb.SK_Die_Index].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_Die[mb.SK_Die_Index].SK.Count)];
		EM_Skill_SP sK_ELSS = new EM_Skill_SP();
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			int eLSS_Index = mb.ELSS_Index;
			EM_SkillGroup[] sKG_ELSS = SingletonMonoScope<GameDataManager>.Instance.SKG_ELSS;
			if (sKG_ELSS != null)
			{
				if (eLSS_Index >= 0 && eLSS_Index < sKG_ELSS.Length)
				{
					EM_SkillGroup eM_SkillGroup = sKG_ELSS[eLSS_Index];
					if (eM_SkillGroup?.SK != null)
					{
						if (eM_SkillGroup.SK.Count > 0)
						{
							int index2 = UnityEngine.Random.Range(0, eM_SkillGroup.SK.Count);
							sK_ELSS = eM_SkillGroup.SK[index2];
						}
						else
						{
							LogUtil.Warn($"[LevelManager.CreatJYs] 元素增强技能列表为空，mb.ELSS_Index = {eLSS_Index}，敌人Id = {id}");
						}
					}
					else
					{
						LogUtil.Warn($"[LevelManager.CreatJYs] 元素增强技能组为空，mb.ELSS_Index = {eLSS_Index}，敌人Id = {id}");
					}
				}
				else
				{
					LogUtil.Warn($"[LevelManager.CreatJYs] mb.ELSS_Index 越界，mb.ELSS_Index = {eLSS_Index}，SKG_ELSS.Length = {sKG_ELSS.Length}，敌人Id = {id}");
				}
			}
			else
			{
				LogUtil.Warn("[LevelManager.CreatJYs] GameDataManager.Instance.SKG_ELSS 为 null");
			}
		}
		int qi = UnityEngine.Random.Range(0, 6);
		int fS_Count_Add = UnityEngine.Random.Range(1, 6);
		int comp_Count_Add = UnityEngine.Random.Range(2, 5);
		List<Enemy> list = new List<Enemy>();
		for (int l = 0; l < num; l++)
		{
			Enemy enemy = SpawnNewJY(id, num, trans, mainIndex, array, num3, randomColor, sK_AT, sK_A, sK_Die, sK_ELSS, qi, fS_Count_Add, comp_Count_Add);
			SaveJYToState(id, enemy, mainIndex, num3, randomColor, array, sK_AT, sK_A, sK_Die, sK_ELSS, qi, fS_Count_Add, comp_Count_Add, enemy.SK_Comp, enemy.SK_FS);
			list.Add(enemy);
		}
		return list;
	}

	private Enemy SpawnNewJY(int id, int Count, Transform trans, int MainIndex, int[] SS, int EL, int randomColor, EM_Skill_SP SK_AT, EM_Skill_SP SK_A, EM_Skill_SP SK_Die, EM_Skill_SP SK_ELSS, int Qi, int FS_Count_Add, int Comp_Count_Add)
	{
		EnemyMB mB = GetMB(id);
		Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.EMPB.Enemy[mB.IndexA].Enemy[mB.IndexB], new Vector3(trans.position.x + UnityEngine.Random.Range(0f - FanWei(Count), FanWei(Count)), trans.position.y + UnityEngine.Random.Range(0f - FanWei(Count), FanWei(Count)), trans.position.z), Quaternion.identity).GetComponent<Enemy>();
		SetJYData(id, component, MainIndex, SS, EL, randomColor, SK_AT, SK_A, SK_Die, SK_ELSS, Qi, FS_Count_Add, Comp_Count_Add, null, null, isRestore: false);
		return component;
	}

	public void SetJYData(int id, Enemy em, int MainIndex, int[] SS, int EL, int randomColor, EM_Skill_SP SK_AT, EM_Skill_SP SK_A, EM_Skill_SP SK_Die, EM_Skill_SP SK_ELSS, int Qi, int FS_Count_Add, int Comp_Count_Add, EM_Skill_CP SK_Comp, EM_Skill_FS SK_FS, bool isRestore)
	{
		EnemyMB mB = GetMB(id);
		em.GlobalID = mB.GlobalID;
		em.Quality = 3;
		em.MainElement = EL;
		em.IndexName = mB.IndexName[MainIndex];
		em.Level = GetCurrentEnemyLevel();
		em.Xp = GetXP(em.Quality, mB.Xp, em.Level);
		em.size = mB.size;
		em.CompOffset = mB.CompOffset;
		em.ItemDropPos = mB.ItemDropPos;
		switch (em.MainElement)
		{
		case 0:
			em.MainELType = DamageType.fire;
			break;
		case 1:
			em.MainELType = DamageType.frozen;
			break;
		case 2:
			em.MainELType = DamageType.thunder;
			break;
		case 3:
			em.MainELType = DamageType.poison;
			break;
		case 4:
			em.MainELType = DamageType.physics;
			break;
		case 5:
			em.MainELType = DamageType.shadow;
			break;
		}
		em.ColorIndex = mB.ColorIndex;
		em.SpineType = mB.SpineType;
		em.CP_FX = mB.CP_FX;
		if (em.SpineType == 0)
		{
			EnemyColorDT enemyColorDT = GP.GP[mB.ColorIndex].XI[em.MainElement].CL[randomColor];
			SkeletonAnimation spine = em.spine;
			if (enemyColorDT.ChangeSK)
			{
				Skin skin = new Skin("skin");
				skin.Clear();
				skin.AddSkin(spine.Skeleton.Data.FindSkin(enemyColorDT.SkinName));
				spine.Skeleton.SetSkin(skin);
				spine.Skeleton.SetSlotsToSetupPose();
			}
			em.SkinName = enemyColorDT.SkinName;
			em.Flip = enemyColorDT.Flip;
			em.MainMix = enemyColorDT.MainMix;
			em.MainHue = enemyColorDT.MainHue;
			em.MainSat = enemyColorDT.MainSat;
			em.MainColor = enemyColorDT.MainColor;
			em.DisloveColor = enemyColorDT.DisloveColor;
			em.AlphaColor = enemyColorDT.AlphaColor;
			em.DieColor = enemyColorDT.DieColor;
			em.RDcolor = randomColor;
		}
		else
		{
			em.SetSpiritColor(em.MainElement);
			em.DieColor = em.MainElement;
		}
		if ((bool)em.FXsustain)
		{
			em.FXsustain.SetColor(em.MainElement);
		}
		em.EnemyType = mB.EnemyType;
		em.Health_Base = Mathf.Floor(mB.Health * Mathf.Pow(HealthMulti, em.Level) * GetEnemyHealthCurveMultiplier(em.Level) * GetHeal());
		em.Health_Bei = 0f;
		SetEnemyBaseData(em);
		em.AttackSpeed_JG = mB.AttackSpeed_JG;
		em.AttackSpeed_Base = mB.ATSpeed;
		em.AttackSpeed_Bei = 0f;
		em.MoveSpeed_Base = mB.MVSpeed;
		em.MoveSpeed_Bei = 0f;
		em.Damage_Base = Mathf.Floor(mB.Damage * Mathf.Pow(DamageMulti, em.Level) * GetDMG());
		em.Damage_Bei = 0f;
		em.Chuan = GetChuan();
		em.FireAnti = GetAnti();
		em.FrozenAnti = GetAnti();
		em.ThunderAnti = GetAnti();
		em.PoisonAnti = GetAnti();
		em.PhysicsAnti = GetAnti();
		em.ShadowAnti = GetAnti();
		em.DamageAnti = GetDMG_Anti();
		em.FlySpeed = 0f;
		em.Range_Base = mB.Range_Base;
		em.Range_Anger = mB.Range_Anger;
		em.Range_Far = mB.Range_Far;
		float num = em.Level;
		em.SK_Rate = mB.SK_Rate + Mathf.FloorToInt(num / 10f);
		em.SK_Rate_ELSS = 0;
		em.Can_DieBoom = false;
		em.SPtype = mB.SPtype;
		em.Die_Index = mB.Die_Index;
		em.DieType = mB.DieType;
		em.DiePos = mB.DiePos;
		em.DieFX_TimeDelay = mB.DieFX_TimeDelay;
		em.DieDelay = mB.DieDelay;
		em.Lie_Index = mB.Lie_Index;
		em.LiePos = mB.LiePos;
		em.FSDie_Index = mB.FSDie_Index;
		em.Idle_Time_Min = mB.Idle_Time_Min;
		em.Idle_Time_Max = mB.Idle_Time_Max;
		em.SO_IdleRate = mB.SO_IdleRate;
		em.SO_AttackRate = mB.SO_AttackRate;
		em.SO_SayRate = mB.SO_SayRate;
		em.SO_HurtRate = mB.SO_HurtRate;
		em.SO_DieRate = mB.SO_DieRate;
		em.SO_Idle = mB.SO_Idle;
		em.SO_Walk = mB.SO_Walk;
		em.SO_AttackA = mB.SO_AttackA;
		em.SO_SayA = mB.SO_SayA;
		em.SO_AttackB = mB.SO_AttackB;
		em.SO_SayB = mB.SO_SayB;
		em.SO_AttackC = mB.SO_AttackC;
		em.SO_SayC = mB.SO_SayC;
		em.SO_Hurt = mB.SO_Hurt;
		em.SO_Die = mB.SO_Die;
		em.SO_ChuiDi = mB.SO_ChuiDi;
		em.IS_Boss = false;
		em.IS_Comp = false;
		em.IS_FS = false;
		em.IsDpsTarget = false;
		em.CF_Rate = mB.CF_Rate;
		em.HitFX = mB.HitFX;
		if (em.SK_Comp == null)
		{
			em.SK_Comp = new EM_Skill_CP();
		}
		if (em.SK_FS == null)
		{
			em.SK_FS = new EM_Skill_FS();
		}
		em.SK_AT = SK_AT;
		em.AT_Ani = mB.AT_Ani;
		em.AT_Fang = mB.AT_Fang;
		em.AT_Distans = mB.AT_Distans * SWS.DistanceRandom(mB.AT_Distans);
		em.SK_A = SK_A;
		em.SK_Ani = mB.SK_Ani;
		em.SK_Fang = mB.SK_Fang;
		em.SK_Distans = mB.SK_Distans * SWS.DistanceRandom(mB.SK_Distans);
		if (isRestore)
		{
			if (SK_Comp != null)
			{
				SetSkillData_CP(em.SK_Comp, SK_Comp);
			}
			if (SK_FS != null)
			{
				SetSkillData_FS(em.SK_FS, SK_FS);
			}
		}
		else
		{
			SetSkillData_CP(em.SK_Comp, mB.SK_Comp);
			SetSkillData_FS(em.SK_FS, mB.SK_FS);
		}
		em.SK_Die = SK_Die;
		em.SK_ELSS = SK_ELSS;
		em.ELSS_Ani = mB.ELSS_Ani;
		em.ELSS_Fang = mB.ELSS_Fang;
		em.ELSS_Distans = mB.ELSS_Distans * SWS.DistanceRandom(mB.ELSS_Distans);
		em.Health_Bei += 600f;
		em.Damage_Bei += 250f;
		ApplyEnemyQualitySoundRate(em);
		em.SSIndex = SS;
		int[] sSIndex = em.SSIndex;
		for (int i = 0; i < sSIndex.Length; i++)
		{
			switch (sSIndex[i])
			{
			case 1:
				em.Damage_Bei += 200f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[0], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 2:
				em.BJRate += 30f;
				em.Chuan += 30f;
				em.FlySpeed += 20f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[1], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 3:
				em.DotDamage += 100f;
				em.DotTime += 100f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[2], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 4:
				em.AttackSpeed_Bei += 50f;
				em.MoveSpeed_Bei += 50f;
				em.AntiSlow += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[3], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 5:
				em.SK_Rate += 30;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[4], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 6:
				em.FS_Count += FS_Count_Add;
				em.SK_Rate_FS += 20;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[5], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 7:
				em.Health_Bei += 300f;
				em.MoveSpeed_Bei += 20f;
				em.AntiSlow += 10f;
				em.yunAnti += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[6], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 8:
				em.Health_Prc += 3f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[7], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 9:
				em.DamageAnti += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[8], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 10:
				em.FireAnti += 30f;
				em.FrozenAnti += 30f;
				em.ThunderAnti += 30f;
				em.PoisonAnti += 30f;
				em.PhysicsAnti += 30f;
				em.ShadowAnti += 30f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[9], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 11:
				em.DotTimeCut += 50f;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[10], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 12:
				em.Can_DieBoom = true;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[11], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 13:
				em.SK_Rate_ELSS += 50;
				switch (em.MainELType)
				{
				case DamageType.fire:
					em.FireAnti += 50f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[6], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.frozen:
					em.FrozenAnti += 50f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[7], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.thunder:
					em.ThunderAnti += 50f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[8], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.poison:
					em.PoisonAnti += 50f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[9], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.physics:
					em.PhysicsAnti += 50f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[10], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				case DamageType.shadow:
					em.ShadowAnti += 50f;
					em.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[11], em.foot.transform.position, Quaternion.identity, em.foot.transform));
					break;
				}
				break;
			case 14:
			{
				SK_BloodPool component;
				switch (Qi)
				{
				case 0:
				{
					component = LeanPool.Spawn(PB.LQJQ[0], em.foot.transform.position, Quaternion.identity, em.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component7 = component.GetComponent<SkillOBJ_DT_SP>();
					component7.indexType = 2;
					component7.em = em;
					component7.ZY = false;
					component7.Dot_Infect = false;
					component7.Dot_Infect_Layer = 0;
					component7.BuffTime = 2f;
					component7.DebuffTime = 2f;
					component7.NoTime = 0;
					component7.damageType = DamageType.fire;
					component7.C_Damage = 20f;
					component7.BF_BJrate = 10f;
					break;
				}
				case 1:
				{
					component = LeanPool.Spawn(PB.LQJQ[1], em.foot.transform.position, Quaternion.identity, em.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component6 = component.GetComponent<SkillOBJ_DT_SP>();
					component6.indexType = 2;
					component6.em = em;
					component6.ZY = false;
					component6.Dot_Infect = false;
					component6.Dot_Infect_Layer = 0;
					component6.BuffTime = 2f;
					component6.DebuffTime = 2f;
					component6.NoTime = 0;
					component6.damageType = DamageType.fire;
					component6.BF_EL_Chuan = 30f;
					component6.BF_Through = 20f;
					break;
				}
				case 2:
				{
					component = LeanPool.Spawn(PB.LQJQ[2], em.foot.transform.position, Quaternion.identity, em.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component5 = component.GetComponent<SkillOBJ_DT_SP>();
					component5.indexType = 2;
					component5.em = em;
					component5.ZY = false;
					component5.Dot_Infect = false;
					component5.Dot_Infect_Layer = 0;
					component5.BuffTime = 2f;
					component5.DebuffTime = 2f;
					component5.NoTime = 0;
					component5.damageType = DamageType.fire;
					component5.BF_GeDang = 20f;
					break;
				}
				case 3:
				{
					component = LeanPool.Spawn(PB.LQJQ[3], em.foot.transform.position, Quaternion.identity, em.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component4 = component.GetComponent<SkillOBJ_DT_SP>();
					component4.indexType = 2;
					component4.em = em;
					component4.ZY = false;
					component4.Dot_Infect = false;
					component4.Dot_Infect_Layer = 0;
					component4.BuffTime = 2f;
					component4.DebuffTime = 2f;
					component4.NoTime = 0;
					component4.damageType = DamageType.fire;
					component4.C_ATspeed = 20f;
					component4.C_MVspeed = 20f;
					break;
				}
				case 4:
				{
					component = LeanPool.Spawn(PB.LQJQ[4], em.foot.transform.position, Quaternion.identity, em.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component3 = component.GetComponent<SkillOBJ_DT_SP>();
					component3.indexType = 2;
					component3.em = em;
					component3.ZY = false;
					component3.Dot_Infect = false;
					component3.Dot_Infect_Layer = 0;
					component3.BuffTime = 2f;
					component3.DebuffTime = 2f;
					component3.NoTime = 0;
					component3.damageType = DamageType.fire;
					component3.BF_DamageAnti = 20f;
					break;
				}
				case 5:
				{
					component = LeanPool.Spawn(PB.LQJQ[5], em.foot.transform.position, Quaternion.identity, em.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component2 = component.GetComponent<SkillOBJ_DT_SP>();
					component2.indexType = 2;
					component2.em = em;
					component2.ZY = false;
					component2.Dot_Infect = false;
					component2.Dot_Infect_Layer = 0;
					component2.BuffTime = 2f;
					component2.DebuffTime = 2f;
					component2.NoTime = 0;
					component2.damageType = DamageType.fire;
					component2.C_Health_Prc = 2f;
					break;
				}
				default:
					component = LeanPool.Spawn(PB.LQJQ[0], em.foot.transform.position, Quaternion.identity, em.foot.transform).GetComponent<SK_BloodPool>();
					break;
				}
				em.LQJQ = component;
				em.LQtype = Qi;
				break;
			}
			case 15:
				em.Comp_Count += Comp_Count_Add;
				em.SK_Rate_Comp += 30;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[12], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			case 16:
				em.CF_Rate += 30;
				em.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[13], em.foot.transform.position, Quaternion.identity, em.foot.transform));
				break;
			}
		}
		NoSameList.Clear();
	}

	private static void SaveJYToState(int id, Enemy em, int MainIndex, int EL, int randomColor, int[] SS, EM_Skill_SP SK_AT, EM_Skill_SP SK_A, EM_Skill_SP SK_Die, EM_Skill_SP SK_ELSS, int Qi, int FS_Count_Add, int Comp_Count_Add, EM_Skill_CP SK_Comp, EM_Skill_FS SK_FS)
	{
		EnemyState runtimeState = new EnemyState
		{
			MonsterId = id,
			Position = em.transform.position,
			MainIndex = MainIndex,
			EL = EL,
			randomColor = randomColor,
			SS = (int[])SS.Clone(),
			SK_AT = SK_AT,
			SK_A = SK_A,
			SK_Die = SK_Die,
			SK_ELSS = SK_ELSS,
			Qi = Qi,
			FS_Count_Add = FS_Count_Add,
			Comp_Count_Add = Comp_Count_Add,
			SK_Comp = SK_Comp,
			SK_FS = SK_FS,
			Hp = 0f,
			IsDead = false
		};
		em.RuntimeState = runtimeState;
	}

	public Enemy CreatBoss(EnemyPoint emp, Transform trans, int id)
	{
		BossMB bossMB = GetBossMB(id);
		Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.EMPB.Boss[bossMB.IndexA].Enemy[bossMB.IndexB], trans.position, Quaternion.identity).GetComponent<Enemy>();
		component.BS = component.GetComponent<Boss>();
		component.GlobalID = bossMB.GlobalID;
		component.Quality = bossMB.Quality;
		component.IndexName = bossMB.IndexName[0];
		component.Level = GetCurrentEnemyLevel();
		component.Xp = GetXP(component.Quality, bossMB.Xp, component.Level);
		component.size = bossMB.size;
		component.CompOffset = bossMB.CompOffset;
		component.TuiSpeed = bossMB.TuiSpeed;
		component.ItemDropPos = bossMB.ItemDropPos;
		component.MainElement = UnityEngine.Random.Range(0, 6);
		switch (component.MainElement)
		{
		case 0:
			component.MainELType = DamageType.fire;
			break;
		case 1:
			component.MainELType = DamageType.frozen;
			break;
		case 2:
			component.MainELType = DamageType.thunder;
			break;
		case 3:
			component.MainELType = DamageType.poison;
			break;
		case 4:
			component.MainELType = DamageType.physics;
			break;
		case 5:
			component.MainELType = DamageType.shadow;
			break;
		}
		component.ColorIndex = bossMB.ColorIndex;
		component.SpineType = bossMB.SpineType;
		if (component.SpineType == 0)
		{
			EnemyColorDT enemyColorDT = GP.GP[bossMB.ColorIndex].XI[component.MainElement].CL[0];
			SkeletonAnimation spine = component.spine;
			if (enemyColorDT.ChangeSK)
			{
				Skin skin = new Skin("skin");
				skin.Clear();
				skin.AddSkin(spine.Skeleton.Data.FindSkin(enemyColorDT.SkinName));
				spine.Skeleton.SetSkin(skin);
				spine.Skeleton.SetSlotsToSetupPose();
			}
			component.SkinName = enemyColorDT.SkinName;
			component.Flip = enemyColorDT.Flip;
			component.MainMix = enemyColorDT.MainMix;
			component.MainHue = enemyColorDT.MainHue;
			component.MainSat = enemyColorDT.MainSat;
			component.MainColor = enemyColorDT.MainColor;
			component.DisloveColor = enemyColorDT.DisloveColor;
			component.AlphaColor = enemyColorDT.AlphaColor;
			component.DieColor = enemyColorDT.DieColor;
			component.RDcolor = 0;
		}
		else
		{
			component.SetSpiritColor(component.MainElement);
			component.DieColor = component.MainElement;
		}
		if ((bool)component.FXsustain)
		{
			component.FXsustain.SetColor(component.MainElement);
		}
		component.EnemyType = bossMB.BossType;
		component.Health_Base = Mathf.Floor(bossMB.Health * Mathf.Pow(HealthMulti, component.Level) * GetEnemyHealthCurveMultiplier(component.Level) * GetHeal());
		component.Health_Bei = 0f;
		component.AttackSpeed_JG = bossMB.AttackSpeed_JG;
		component.AttackSpeed_Base = bossMB.ATSpeed;
		component.AttackSpeed_Bei = 0f;
		component.MoveSpeed_Base = bossMB.MVSpeed;
		component.MoveSpeed_Bei = 0f;
		component.Damage_Base = Mathf.Floor(bossMB.Damage * Mathf.Pow(DamageMulti, component.Level) * GetDMG());
		component.Damage_Bei = 0f;
		SetBossData(component);
		component.FireAnti = GetAnti();
		component.FrozenAnti = GetAnti();
		component.ThunderAnti = GetAnti();
		component.PoisonAnti = GetAnti();
		component.PhysicsAnti = GetAnti();
		component.ShadowAnti = GetAnti();
		component.Chuan = GetChuan();
		component.DamageAnti += GetDMG_Anti();
		component.FlySpeed = 0f;
		component.Range_Base = bossMB.Range_Base;
		component.Range_Anger = bossMB.Range_Anger;
		component.Range_Far = bossMB.Range_Far;
		component.Range_ATplayer_multi = bossMB.Range_ATplayer_multi;
		component.ResetBossTargetPriorityMulti();
		component.SK_Rate = bossMB.SK_Rate;
		component.SK_Rate_Comp = bossMB.SK_Rate_Comp;
		component.SK_Rate_ELSS = 0;
		component.Can_DieBoom = false;
		component.SPtype = bossMB.SPtype;
		component.Die_Index = bossMB.Die_Index;
		component.DieType = bossMB.DieType;
		component.DiePos = bossMB.DiePos;
		component.DieFX_TimeDelay = bossMB.DieFX_TimeDelay;
		component.DieDelay = bossMB.DieDelay;
		component.Lie_Index = bossMB.Lie_Index;
		component.LiePos = bossMB.LiePos;
		component.Idle_Time_Min = bossMB.Idle_Time_Min;
		component.Idle_Time_Max = bossMB.Idle_Time_Max;
		component.SO_IdleRate = bossMB.SO_IdleRate;
		component.SO_AttackRate = bossMB.SO_AttackRate;
		component.SO_SayRate = bossMB.SO_SayRate;
		component.SO_HurtRate = bossMB.SO_HurtRate;
		component.SO_DieRate = bossMB.SO_DieRate;
		ApplyEnemyQualitySoundRate(component);
		component.BS.SO_Idle = bossMB.SO_Idle;
		component.SO_Walk = bossMB.SO_Walk;
		component.SO_Hurt = bossMB.SO_Hurt;
		component.SO_Die = bossMB.SO_Die;
		component.BS.SO_AttackA = bossMB.SO_AttackA;
		component.BS.SO_SayA = bossMB.SO_SayA;
		component.BS.SO_AttackB = bossMB.SO_AttackB;
		component.BS.SO_SayB = bossMB.SO_SayB;
		component.BS.SO_AttackC = bossMB.SO_AttackC;
		component.BS.SO_SayC = bossMB.SO_SayC;
		component.BS.SO_AttackD = bossMB.SO_AttackD;
		component.BS.SO_SayD = bossMB.SO_SayD;
		component.BS.SO_AttackE = bossMB.SO_AttackE;
		component.BS.SO_SayE = bossMB.SO_SayE;
		component.BS.SO_ChongStart = bossMB.SO_ChongStart;
		component.BS.SO_ChongEnd = bossMB.SO_ChongEnd;
		component.BS.SO_Jump = bossMB.SO_Jump;
		component.BS.SO_Land = bossMB.SO_Land;
		component.BS.SO_SPC1 = bossMB.SO_SPC1;
		component.BS.SO_SPC2 = bossMB.SO_SPC2;
		component.BS.SO_SPC3 = bossMB.SO_SPC3;
		component.IS_Boss = true;
		component.BS.em = component;
		component.IS_Comp = false;
		component.IS_FS = false;
		component.IsDpsTarget = false;
		component.Can_DieBoom = false;
		if (component.BS.AT == null)
		{
			component.BS.AT = new List<EM_Skill_SP>();
		}
		if (component.BS.SK == null)
		{
			component.BS.SK = new List<EM_Skill_SP>();
		}
		if (component.BS.SKC == null)
		{
			component.BS.SKC = new EM_Skill_CP();
		}
		if (component.SK_Die == null)
		{
			component.SK_Die = new EM_Skill_SP();
		}
		component.BS.AT = bossMB.AT;
		component.BS.SK = bossMB.SK;
		SetSkillData_CP(component.BS.SKC, bossMB.SKC);
		component.SK_Die = SingletonMonoScope<GameDataManager>.Instance.SKG_Die[bossMB.SK_Die_Index].SK[UnityEngine.Random.Range(0, SingletonMonoScope<GameDataManager>.Instance.SKG_Die[bossMB.SK_Die_Index].SK.Count)];
		if (!CurLevelData.IsMJ)
		{
			component.SSIndex[0] = 2;
			component.SSIndex[1] = 7;
			component.SSIndex[2] = 9;
			component.SSIndex[3] = 10;
			component.SSIndex[4] = 11;
		}
		else
		{
			NoSameRD[] rDindex = RDindex;
			foreach (NoSameRD item in rDindex)
			{
				NoSameList.Add(item);
			}
			for (int j = 0; j < 5; j++)
			{
				component.SSIndex[j] = 0;
			}
			for (int k = 0; k < 5; k++)
			{
				int index = UnityEngine.Random.Range(1, RDindex.Length - k);
				component.SSIndex[k] = NoSameList[index].Index;
				NoSameList.Remove(NoSameList[index]);
			}
		}
		int[] sSIndex = component.SSIndex;
		for (int l = 0; l < sSIndex.Length; l++)
		{
			switch (sSIndex[l])
			{
			case 1:
				component.Damage_Bei += 100f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[0], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 2:
				component.BJRate += 30f;
				component.Chuan += 30f;
				component.FlySpeed += 20f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[1], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 3:
				component.DotDamage += 100f;
				component.DotTime += 100f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[2], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 4:
				component.AttackSpeed_Bei += 50f;
				component.MoveSpeed_Bei += 50f;
				component.AntiSlow += 30f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[3], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 5:
				component.SK_Rate += 30;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[4], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 6:
				component.FS_Count += UnityEngine.Random.Range(1, 6);
				component.SK_Rate_FS += 20;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[5], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 7:
				component.Health_Bei += 100f;
				component.MoveSpeed_Bei += 20f;
				component.AntiSlow += 10f;
				component.yunAnti += 30f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[6], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 8:
				component.Health_Prc += 3f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[7], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 9:
				component.DamageAnti += 30f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[8], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 10:
				component.FireAnti += 30f;
				component.FrozenAnti += 30f;
				component.ThunderAnti += 30f;
				component.PoisonAnti += 30f;
				component.PhysicsAnti += 30f;
				component.ShadowAnti += 30f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[9], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 11:
				component.DotTimeCut += 50f;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[10], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 12:
				component.Can_DieBoom = true;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[11], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 13:
				component.SK_Rate_ELSS += 50;
				switch (component.MainELType)
				{
				case DamageType.fire:
					component.FireAnti += 50f;
					component.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[6], component.foot.transform.position, Quaternion.identity, component.foot.transform));
					break;
				case DamageType.frozen:
					component.FrozenAnti += 50f;
					component.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[7], component.foot.transform.position, Quaternion.identity, component.foot.transform));
					break;
				case DamageType.thunder:
					component.ThunderAnti += 50f;
					component.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[8], component.foot.transform.position, Quaternion.identity, component.foot.transform));
					break;
				case DamageType.poison:
					component.PoisonAnti += 50f;
					component.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[9], component.foot.transform.position, Quaternion.identity, component.foot.transform));
					break;
				case DamageType.physics:
					component.PhysicsAnti += 50f;
					component.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[10], component.foot.transform.position, Quaternion.identity, component.foot.transform));
					break;
				case DamageType.shadow:
					component.ShadowAnti += 50f;
					component.AuraList.Add(LeanPool.Spawn(PB.Aura_EL[11], component.foot.transform.position, Quaternion.identity, component.foot.transform));
					break;
				}
				break;
			case 14:
			{
				int num = UnityEngine.Random.Range(0, 6);
				SK_BloodPool component2;
				switch (num)
				{
				case 0:
				{
					component2 = LeanPool.Spawn(PB.LQJQ[0], component.foot.transform.position, Quaternion.identity, component.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component8 = component2.GetComponent<SkillOBJ_DT_SP>();
					component8.indexType = 2;
					component8.em = component;
					component8.ZY = false;
					component8.Dot_Infect = false;
					component8.Dot_Infect_Layer = 0;
					component8.BuffTime = 2f;
					component8.DebuffTime = 2f;
					component8.NoTime = 0;
					component8.damageType = DamageType.fire;
					component8.C_Damage = 20f;
					component8.BF_BJrate = 10f;
					break;
				}
				case 1:
				{
					component2 = LeanPool.Spawn(PB.LQJQ[1], component.foot.transform.position, Quaternion.identity, component.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component7 = component2.GetComponent<SkillOBJ_DT_SP>();
					component7.indexType = 2;
					component7.em = component;
					component7.ZY = false;
					component7.Dot_Infect = false;
					component7.Dot_Infect_Layer = 0;
					component7.BuffTime = 2f;
					component7.DebuffTime = 2f;
					component7.NoTime = 0;
					component7.damageType = DamageType.fire;
					component7.BF_EL_Chuan = 30f;
					component7.BF_Through = 20f;
					break;
				}
				case 2:
				{
					component2 = LeanPool.Spawn(PB.LQJQ[2], component.foot.transform.position, Quaternion.identity, component.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component6 = component2.GetComponent<SkillOBJ_DT_SP>();
					component6.indexType = 2;
					component6.em = component;
					component6.ZY = false;
					component6.Dot_Infect = false;
					component6.Dot_Infect_Layer = 0;
					component6.BuffTime = 2f;
					component6.DebuffTime = 2f;
					component6.NoTime = 0;
					component6.damageType = DamageType.fire;
					component6.BF_GeDang = 20f;
					break;
				}
				case 3:
				{
					component2 = LeanPool.Spawn(PB.LQJQ[3], component.foot.transform.position, Quaternion.identity, component.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component5 = component2.GetComponent<SkillOBJ_DT_SP>();
					component5.indexType = 2;
					component5.em = component;
					component5.ZY = false;
					component5.Dot_Infect = false;
					component5.Dot_Infect_Layer = 0;
					component5.BuffTime = 2f;
					component5.DebuffTime = 2f;
					component5.NoTime = 0;
					component5.damageType = DamageType.fire;
					component5.C_ATspeed = 20f;
					component5.C_MVspeed = 20f;
					break;
				}
				case 4:
				{
					component2 = LeanPool.Spawn(PB.LQJQ[4], component.foot.transform.position, Quaternion.identity, component.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component4 = component2.GetComponent<SkillOBJ_DT_SP>();
					component4.indexType = 2;
					component4.em = component;
					component4.ZY = false;
					component4.Dot_Infect = false;
					component4.Dot_Infect_Layer = 0;
					component4.BuffTime = 2f;
					component4.DebuffTime = 2f;
					component4.NoTime = 0;
					component4.damageType = DamageType.fire;
					component4.BF_DamageAnti = 20f;
					break;
				}
				case 5:
				{
					component2 = LeanPool.Spawn(PB.LQJQ[5], component.foot.transform.position, Quaternion.identity, component.foot.transform).GetComponent<SK_BloodPool>();
					SkillOBJ_DT_SP component3 = component2.GetComponent<SkillOBJ_DT_SP>();
					component3.indexType = 2;
					component3.em = component;
					component3.ZY = false;
					component3.Dot_Infect = false;
					component3.Dot_Infect_Layer = 0;
					component3.BuffTime = 2f;
					component3.DebuffTime = 2f;
					component3.NoTime = 0;
					component3.damageType = DamageType.fire;
					component3.C_Health_Prc = 2f;
					break;
				}
				default:
					component2 = LeanPool.Spawn(PB.LQJQ[0], component.foot.transform.position, Quaternion.identity, component.foot.transform).GetComponent<SK_BloodPool>();
					break;
				}
				component.LQJQ = component2;
				component.LQtype = num;
				break;
			}
			case 15:
				component.Comp_Count += UnityEngine.Random.Range(2, 5);
				component.SK_Rate_Comp += 30;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[12], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			case 16:
				component.CF_Rate += 30;
				component.AuraList.Add(LeanPool.Spawn(PB.Aura_SP[13], component.foot.transform.position, Quaternion.identity, component.foot.transform));
				break;
			}
		}
		UI_BossTip.Instance.boss.Add(component);
		return component;
	}

	public static BossMB GetBossMB(int id)
	{
		foreach (BossMB item in SingletonMonoScope<GameDataManager>.Instance.BossMB)
		{
			if (item.GlobalID == id)
			{
				return item;
			}
		}
		return null;
	}

	public List<ChestSpawnInfo> CreateChest(string pointId, Transform trans, bool isVertical)
	{
		List<ChestSpawnInfo> result = new List<ChestSpawnInfo>();
		int num = ProbUtil.Roll(50, 45, 5);
		float num2 = UnityEngine.Random.Range(CurLevelData.ChestDis - 0.1f, CurLevelData.ChestDis + 0.2f);
		int spawnIndex = 0;
		if (isVertical)
		{
			switch (num)
			{
			case 1:
				Spawn(trans.position);
				break;
			case 2:
				Spawn(new Vector3(trans.position.x, trans.position.y - num2, 0f));
				Spawn(new Vector3(trans.position.x, trans.position.y + num2, 0f));
				break;
			}
		}
		else
		{
			switch (num)
			{
			case 1:
				Spawn(trans.position);
				break;
			case 2:
				Spawn(new Vector3(trans.position.x - num2, trans.position.y, 0f));
				Spawn(new Vector3(trans.position.x + num2, trans.position.y, 0f));
				break;
			}
		}
		return result;
		void Spawn(Vector3 pos)
		{
			int chestIndex = CurLevelData.ChestIndex;
			Chest chest = SetChest(chestIndex, pos);
			string text = $"{pointId}_Chest_{spawnIndex++}";
			chest.InitRuntimeId(text);
			result.Add(new ChestSpawnInfo
			{
				RuntimeId = text,
				ChestIndex = chestIndex,
				Position = pos
			});
		}
	}

	public Chest SetChest(int index, Vector3 pos)
	{
		Chest component = UnityEngine.Object.Instantiate(WD.Chest[index], pos, Quaternion.identity).GetComponent<Chest>();
		component.Quality = 0;
		return component;
	}

	public void SetSkillData_SP(EM_Skill_SP sp, EM_Skill_SP mb)
	{
		sp.IndexName = mb.IndexName;
		sp.UseAni = mb.UseAni;
		sp.HitFX = mb.HitFX;
		sp.HitFX_Rate = mb.HitFX_Rate;
		sp.ATFX = mb.ATFX;
		sp.StarFX = mb.StarFX;
		sp.StarFX_pos = mb.StarFX_pos;
		sp.BaTi = mb.BaTi;
		sp.WuDi = mb.WuDi;
		sp.CJY = mb.CJY;
		sp.ChongSpeedMulti = mb.ChongSpeedMulti;
		sp.Fang = mb.Fang;
		sp.ATmod = mb.ATmod;
		sp.FStype = mb.FStype;
		sp.FSFXtype = mb.FSFXtype;
		sp.RTtypeOBJ = mb.RTtypeOBJ;
		sp.TypeTar = mb.TypeTar;
		sp.RTtypeFX = mb.RTtypeFX;
		sp.Distance = mb.Distance * UnityEngine.Random.Range(0.9f, 1.2f);
		sp.Range_Hurt = mb.Range_Hurt;
		sp.damageType = mb.damageType;
		sp.MainEL = mb.MainEL;
		sp.ThroughType = mb.ThroughType;
		sp.AttackType = mb.AttackType;
		sp.AttackTypeA = mb.AttackTypeA;
		sp.AttackTypeB = mb.AttackTypeB;
		sp.Damage = mb.Damage;
		sp.DamageA = mb.DamageA;
		sp.DamageB = mb.DamageB;
		sp.SpeedCut = mb.SpeedCut;
		sp.BF_DamageAnti = mb.BF_DamageAnti;
		sp.CompAttackSpeed = mb.CompAttackSpeed;
		sp.C_Damage = mb.C_Damage;
		sp.Reborn = mb.Reborn;
		sp.DotRate = mb.DotRate;
		sp.DotDamage = mb.DotDamage;
		sp.BuffTime = mb.BuffTime;
		sp.DebuffTime = mb.DebuffTime;
		sp.ORB_time = mb.ORB_time;
		sp.EXP_time = mb.EXP_time;
		sp.OBJ = mb.OBJ;
		sp.Layer_SubA = mb.Layer_SubA;
		sp.Layer_SubB = mb.Layer_SubB;
		sp.ORB = mb.ORB;
		sp.ZD_F = mb.ZD_F;
		sp.ZD_S = mb.ZD_S;
		sp.ZD_AB = mb.ZD_AB;
		sp.EXP_F = mb.EXP_F;
		sp.EXP_S = mb.EXP_S;
		sp.EXP_AB = mb.EXP_AB;
		sp.Dic_F = mb.Dic_F;
		sp.ZD_F = mb.ZD_F;
		sp.Sound = mb.Sound;
		sp.Count_ORB = mb.Count_ORB;
		sp.Count_ATtarget = mb.Count_ATtarget;
		sp.CF_Count = mb.CF_Count;
		sp.Count_F = mb.Count_F;
		sp.Count_S = mb.Count_S;
		sp.Count_AB = mb.Count_AB;
		sp.CountMulti = mb.CountMulti;
		sp.CountEXP = mb.CountEXP;
		sp.TypeORB = mb.TypeORB;
		sp.CF_Type = mb.CF_Type;
		sp.Type_F = mb.Type_F;
		sp.Type_S = mb.Type_S;
		sp.Type_AB = mb.Type_AB;
		sp.TypeDIC_F = mb.TypeDIC_F;
		sp.TypeDIC_S = mb.TypeDIC_S;
		sp.TypeEXP_F = mb.TypeEXP_F;
		sp.TypeEXP_S = mb.TypeEXP_S;
		sp.TypeEXP_AB = mb.TypeEXP_AB;
		sp.Size = mb.Size;
		sp.High = mb.High;
		sp.JG = mb.JG;
		sp.AngleA = mb.AngleA;
		sp.AngleB = mb.AngleB;
		sp.Range1 = mb.Range1;
		sp.Range2 = mb.Range2;
		sp.Range_AT = mb.Range_AT;
		sp.FStime1 = mb.FStime1;
		sp.FStime2 = mb.FStime2;
		sp.Speed1 = mb.Speed1;
		sp.Speed2 = mb.Speed2;
		sp.Speed3 = mb.Speed3;
		sp.Speed4 = mb.Speed4;
		sp.Follow_F = mb.Follow_F;
		sp.Follow_S = mb.Follow_S;
		sp.AllChuan_F = mb.AllChuan_F;
		sp.AllChuan_S = mb.AllChuan_S;
		sp.RDSpeed_F = mb.RDSpeed_F;
		sp.RDSpeed_S = mb.RDSpeed_S;
		sp.HasFX = mb.HasFX;
		sp.S_HasFX = mb.S_HasFX;
		sp.AB_HasFX = mb.AB_HasFX;
		sp.colEXP = mb.colEXP;
		sp.colEXP_AB = mb.colEXP_AB;
		sp.S_colEXP = mb.S_colEXP;
		sp.AB_colEXP = mb.AB_colEXP;
		sp.TimeEXP = mb.TimeEXP;
		sp.TimeEXP_AB = mb.TimeEXP_AB;
		sp.EXPpos = mb.EXPpos;
		sp.EXPpos_AB = mb.EXPpos_AB;
		sp.S_EXPpos = mb.S_EXPpos;
		sp.AB_EXPpos = mb.AB_EXPpos;
		sp.AngleEXP = mb.AngleEXP;
		sp.AngleEXP_AB = mb.AngleEXP_AB;
		sp.HurtSK_JG = mb.HurtSK_JG;
		sp.HurtSK_Rate = mb.HurtSK_Rate;
	}

	public void SetSkillData_CP(EM_Skill_CP cp, EM_Skill_CP mb)
	{
		cp.GlobalID = mb.GlobalID;
		cp.UseAni = mb.UseAni;
		cp.CPFX = mb.CPFX;
		cp.FSFXtype = mb.FSFXtype;
		cp.MainElement = UnityEngine.Random.Range(0, 6);
		switch (cp.MainElement)
		{
		case 0:
			cp.damageType = DamageType.fire;
			break;
		case 1:
			cp.damageType = DamageType.frozen;
			break;
		case 2:
			cp.damageType = DamageType.thunder;
			break;
		case 3:
			cp.damageType = DamageType.poison;
			break;
		case 4:
			cp.damageType = DamageType.physics;
			break;
		case 5:
			cp.damageType = DamageType.shadow;
			break;
		}
		EnemyMB mB = GetMB(cp.GlobalID);
		if (mB != null)
		{
			cp.ColorIndex = mB.ColorIndex;
		}
		int num = UnityEngine.Random.Range(0, GP.GP[cp.ColorIndex].XI[cp.MainElement].CL.Length);
		cp.Flip = GP.GP[cp.ColorIndex].XI[cp.MainElement].CL[num].Flip;
		cp.MainMix = GP.GP[cp.ColorIndex].XI[cp.MainElement].CL[num].MainMix;
		cp.MainHue = GP.GP[cp.ColorIndex].XI[cp.MainElement].CL[num].MainHue;
		cp.MainSat = GP.GP[cp.ColorIndex].XI[cp.MainElement].CL[num].MainSat;
		cp.MainColor = GP.GP[cp.ColorIndex].XI[cp.MainElement].CL[num].MainColor;
		cp.DisloveColor = GP.GP[cp.ColorIndex].XI[cp.MainElement].CL[num].DisloveColor;
		cp.AlphaColor = GP.GP[cp.ColorIndex].XI[cp.MainElement].CL[num].AlphaColor;
		cp.RDcolor = num;
	}

	public void SetSkillData_FS(EM_Skill_FS fs, EM_Skill_FS mb)
	{
		fs.UseAni = mb.UseAni;
		fs.CPFX = mb.CPFX;
		fs.FSFXtype = mb.FSFXtype;
	}

	public static float FanWei(int Count)
	{
		if (Count > 0 && Count < 7)
		{
			return 0.3f;
		}
		if (Count >= 7 && Count < 10)
		{
			return 0.57f;
		}
		if (Count >= 10 && Count < 15)
		{
			return 0.8f;
		}
		if (Count >= 15 && Count < 22)
		{
			return 1f;
		}
		return 1.2f;
	}

	public void SetEnemyBaseData(Enemy em)
	{
		switch (em.EnemyType)
		{
		case 0:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 0f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.5f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 1:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 3f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 10f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 2f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 2:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 3f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 20f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 10f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 3;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.7f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 10:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 0f;
			em.Through = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 3;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.7f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 11:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 3f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 10f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 10f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 3;
			em.FS_EveryCount = 2;
			em.FS_Count = 4;
			em.Range_ATplayer_multi = 2f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 5;
			break;
		case 12:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 10f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 10f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 2f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 13:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 5f;
			em.GeDang = 0f;
			em.yunAnti = 20f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 10f;
			em.DotTimeCut = 10f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 2f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 14:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 3f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 30f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 1f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 10f;
			em.DotTimeCut = 10f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 2f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 20:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 5f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 0f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.6f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 21:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 5f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 0f;
			em.Through = 10f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.6f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 22:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 0f;
			em.Through = 10f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 10f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.6f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 30:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 0f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 3;
			em.FS_EveryCount = 2;
			em.FS_Count = 4;
			em.Range_ATplayer_multi = 1.6f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 31:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 10f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 0f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 4;
			em.FS_EveryCount = 2;
			em.FS_Count = 5;
			em.Range_ATplayer_multi = 1.6f;
			em.SK_Rate_Comp = 5;
			em.SK_Rate_FS = 5;
			break;
		case 32:
			em.AT_Idle_Min = 2f;
			em.AT_Idle_Max = 6f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 10f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 10f;
			em.DotTime = 20f;
			em.AntiSlow = 10f;
			em.DotTimeCut = 10f;
			em.Comp_EveryCount = 2;
			em.Comp_Count = 5;
			em.FS_EveryCount = 3;
			em.FS_Count = 6;
			em.Range_ATplayer_multi = 1.8f;
			em.SK_Rate_Comp = 5;
			em.SK_Rate_FS = 5;
			break;
		case 50:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 20f;
			em.Through = 0f;
			em.DamageAnti = 15f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 10f;
			em.DotTimeCut = 10f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.6f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		case 80:
			em.AT_Idle_Min = 1f;
			em.AT_Idle_Max = 4f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 0f;
			em.Through = 0f;
			em.DamageAnti = 0f;
			em.Health_Prc = 0f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 10f;
			em.DotTimeCut = 0f;
			em.Comp_EveryCount = 1;
			em.Comp_Count = 2;
			em.FS_EveryCount = 1;
			em.FS_Count = 3;
			em.Range_ATplayer_multi = 1.6f;
			em.SK_Rate_Comp = 0;
			em.SK_Rate_FS = 0;
			break;
		}
	}

	public void SetBossData(Enemy em)
	{
		switch (em.EnemyType)
		{
		case 0:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 30f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0.1f;
			em.DotDamage = 10f;
			em.DotTime = 10f;
			em.AntiSlow = 20f;
			em.DotTimeCut = 30f;
			em.Comp_EveryCount = 6;
			em.Comp_Count = 24;
			break;
		case 1:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 0f;
			em.GeDang = 0f;
			em.yunAnti = 20f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0f;
			em.DotDamage = 10f;
			em.DotTime = 0f;
			em.AntiSlow = 20f;
			em.DotTimeCut = 20f;
			em.Comp_EveryCount = 6;
			em.Comp_Count = 24;
			break;
		case 2:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 5f;
			em.GeDang = 0f;
			em.yunAnti = 30f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0.1f;
			em.DotDamage = 10f;
			em.DotTime = 0f;
			em.AntiSlow = 30f;
			em.DotTimeCut = 30f;
			em.Comp_EveryCount = 6;
			em.Comp_Count = 24;
			break;
		case 3:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 40f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0.1f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 40f;
			em.DotTimeCut = 40f;
			em.Comp_EveryCount = 7;
			em.Comp_Count = 30;
			break;
		case 4:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 40f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0.1f;
			em.DotDamage = 20f;
			em.DotTime = 0f;
			em.AntiSlow = 30f;
			em.DotTimeCut = 30f;
			em.Comp_EveryCount = 8;
			em.Comp_Count = 40;
			break;
		case 5:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 20f;
			em.GeDang = 0f;
			em.yunAnti = 30f;
			em.Through = 10f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0f;
			em.DotDamage = 10f;
			em.DotTime = 0f;
			em.AntiSlow = 20f;
			em.DotTimeCut = 20f;
			em.Comp_EveryCount = 6;
			em.Comp_Count = 24;
			break;
		case 6:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 30f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0.1f;
			em.DotDamage = 20f;
			em.DotTime = 0f;
			em.AntiSlow = 30f;
			em.DotTimeCut = 30f;
			em.Comp_EveryCount = 7;
			em.Comp_Count = 30;
			break;
		case 7:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 30f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0.1f;
			em.DotDamage = 10f;
			em.DotTime = 20f;
			em.AntiSlow = 30f;
			em.DotTimeCut = 30f;
			em.Comp_EveryCount = 8;
			em.Comp_Count = 40;
			break;
		case 8:
			em.AT_Idle_Min = 0f;
			em.AT_Idle_Max = 0.5f;
			em.BJRate = 10f;
			em.GeDang = 0f;
			em.yunAnti = 50f;
			em.Through = 0f;
			em.DamageAnti = 10f;
			em.Health_Prc = 0.1f;
			em.DotDamage = 0f;
			em.DotTime = 0f;
			em.AntiSlow = 40f;
			em.DotTimeCut = 30f;
			em.Comp_EveryCount = 6;
			em.Comp_Count = 24;
			break;
		}
	}

	public int GetXP(int Quality, float xp, int level)
	{
		float num = 1f;
		if (CurLevelData.IsMJ && SingletonMonoScope<MijingManager>.HasInstance)
		{
			num = SingletonMonoScope<MijingManager>.Instance.GetEnemyXpMultiplier();
		}
		return Quality switch
		{
			0 => Mathf.FloorToInt(xp * Mathf.Pow(XPmulti, level) * num), 
			1 => Mathf.FloorToInt(xp * Mathf.Pow(XPmulti, level) * 2f * num), 
			2 => Mathf.FloorToInt(xp * Mathf.Pow(XPmulti, level) * 3f * num), 
			3 => Mathf.FloorToInt(xp * Mathf.Pow(XPmulti, level) * 5f * num), 
			4 => Mathf.FloorToInt(xp * Mathf.Pow(XPmulti, level) * 20f * num), 
			5 => Mathf.FloorToInt(xp * Mathf.Pow(XPmulti, level) * 40f * num), 
			_ => Mathf.FloorToInt(xp * Mathf.Pow(XPmulti, level)), 
		};
	}

	private static void ApplyEnemyQualitySoundRate(Enemy em)
	{
		if (!(em == null))
		{
			switch (em.Quality)
			{
			case 2:
				em.SO_AttackRate += 10;
				em.SO_SayRate += 10;
				em.SO_HurtRate += 10;
				em.SO_DieRate += 30;
				break;
			case 3:
				em.SO_IdleRate += 10;
				em.SO_AttackRate += 20;
				em.SO_SayRate += 20;
				em.SO_HurtRate += 30;
				em.SO_DieRate += 50;
				break;
			}
		}
	}

	public static float GetHeal()
	{
		if (CurLevelData.IsMJ && SingletonMonoScope<MijingManager>.HasInstance)
		{
			return SingletonMonoScope<MijingManager>.Instance.GetEnemyHealthMultiplier();
		}
		return 1f;
	}

	public static float GetDMG()
	{
		if (CurLevelData.IsMJ && SingletonMonoScope<MijingManager>.HasInstance)
		{
			return SingletonMonoScope<MijingManager>.Instance.GetEnemyDamageMultiplier();
		}
		return 1f;
	}

	public static float GetAnti()
	{
		if (CurLevelData.IsMJ)
		{
			float num = CurLevelData.MapLevel;
			return (20f + num / 10f) * 1.1f;
		}
		float num2 = CurLevelData.MapLevel;
		return 20f + num2 / 10f;
	}

	public static float GetChuan()
	{
		float num = CurLevelData.MapLevel;
		if (CurLevelData.IsMJ && SingletonMonoScope<MijingManager>.HasInstance)
		{
			return (20f + num / 10f) * SingletonMonoScope<MijingManager>.Instance.GetEnemyPenetrationMultiplier();
		}
		return 20f + num / 10f;
	}

	public static float GetDMG_Anti()
	{
		if (CurLevelData.IsMJ && SingletonMonoScope<MijingManager>.HasInstance)
		{
			return (float)CurLevelData.MapLevel * 0.05f * SingletonMonoScope<MijingManager>.Instance.GetEnemyDamageReductionMultiplier();
		}
		return (float)CurLevelData.MapLevel * 0.05f;
	}
}
