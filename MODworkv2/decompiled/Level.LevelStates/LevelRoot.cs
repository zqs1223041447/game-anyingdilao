using System.Collections.Generic;
using Core.Settings;
using Core.Teleport;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Mijing;
using SaveSystem;
using Scenes;
using UI.Map;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Level.LevelStates;

public class LevelRoot : MonoBehaviour
{
	[Header("通关本层需要的分数 填-1则自动使用默认值")]
	public int needScore = -1;

	public GameObject Mijing;

	public GameObject Baoshi;

	private Tilemap mapSprite;

	private BossLevelManager bossLevelManager;

	private readonly List<ILevelLockable> lockables = new List<ILevelLockable>();

	private int expectedPointCount;

	private int registeredPointCount;

	private bool registerPointFinished;

	private static bool CurIsBoss
	{
		get
		{
			if (SingletonMonoScope<LevelManager>.HasInstance)
			{
				return LevelManager.GetIsBoss();
			}
			return false;
		}
	}

	private static bool CurIsHome => SceneManager.GetActiveScene().name == "HomeScene";

	public void MijingToggle()
	{
		if (!Mijing)
		{
			Mijing = base.transform.Find("Mijing").gameObject;
		}
		if (SaveManager.RuntimeData.UnlockedMijing && SettingsLoader.Instance.MijingToggle)
		{
			if (!Mijing.activeSelf)
			{
				Mijing.SetActive(value: true);
			}
		}
		else
		{
			Mijing.SetActive(value: false);
		}
	}

	public void BaoshiToggle()
	{
		if (!Baoshi)
		{
			Baoshi = base.transform.Find("Baoshi").gameObject;
		}
		if (SettingsLoader.Instance.BaoshiToggle)
		{
			if (!Baoshi.activeSelf)
			{
				Baoshi.SetActive(value: true);
			}
		}
		else
		{
			Baoshi.SetActive(value: false);
		}
	}

	private void CacheMapSprite()
	{
		if (!(Object)(object)mapSprite)
		{
			Transform transform = base.transform.Find("TileMap/Mini Map");
			if ((bool)transform)
			{
				mapSprite = transform.GetComponent<Tilemap>();
			}
		}
	}

	private void RegisterToMapManager()
	{
		if (SingletonMonoScope<MapManager>.HasInstance)
		{
			SingletonMonoScope<MapManager>.Instance.RegisterLevelRoot(this);
		}
	}

	private void UnregisterFromMapManager()
	{
		if (SingletonMonoScope<MapManager>.HasInstance)
		{
			SingletonMonoScope<MapManager>.Instance.UnregisterLevelRoot(this);
		}
	}

	public Tilemap GetMapSprite()
	{
		CacheMapSprite();
		return mapSprite;
	}

	public void SetLevelMapAlpha(float alpha)
	{
		CacheMapSprite();
		if ((bool)(Object)(object)mapSprite)
		{
			Color color = mapSprite.color;
			color.a = alpha;
			mapSprite.color = color;
		}
	}

	public void RegisterLockable(ILevelLockable obj)
	{
		if (!CurIsHome && CurIsBoss)
		{
			lockables.Add(obj);
		}
	}

	public void ClearAll()
	{
		lockables.Clear();
	}

	public void SetAllLocked(bool locked)
	{
		if (CurIsHome || !CurIsBoss)
		{
			return;
		}
		foreach (ILevelLockable lockable in lockables)
		{
			lockable.SetLocked(locked);
		}
	}

	public void OnBossAllDefeated()
	{
		if (CurIsHome || !CurIsBoss)
		{
			return;
		}
		SetAllLocked(locked: false);
		GameObject gameObject = Singleton<ResManager>.Instance.Load<GameObject>("World/Build/BossPortal");
		if (!gameObject)
		{
			LogUtil.Error("BossPortal 资源加载失败");
			return;
		}
		Vector3 position = Vector3.zero;
		if (SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<PortalManager>.HasInstance)
		{
			position = PortalManager.GetPortalSpawnPos(SingletonMonoScope<PlayerManager>.Instance.transform.position);
		}
		else
		{
			LogUtil.Error("击败Boss后未找到玩家管理器或传送门管理器");
		}
		BossPortal component = Object.Instantiate(gameObject, position, Quaternion.identity).GetComponent<BossPortal>();
		if (LevelManager.GetIsCurChapterFinal())
		{
			component.Init(BossPortalType.GoHome);
		}
		else
		{
			component.Init(BossPortalType.GoLevel);
		}
		if (SingletonMonoScope<AutoSaveManager>.HasInstance)
		{
			SingletonMonoScope<AutoSaveManager>.Instance.TrySaveWithIcon();
		}
	}

	public void RegisterExpectedPoint()
	{
		if (!CurIsHome)
		{
			if (registerPointFinished)
			{
				LogUtil.Error("传送点需要在关卡根节点初始化之后注册！");
			}
			else
			{
				expectedPointCount++;
			}
		}
	}

	public void NotifyPointReady()
	{
		if (!CurIsHome)
		{
			registeredPointCount++;
			if (registerPointFinished && registeredPointCount >= expectedPointCount && SingletonMonoScene<LevelSceneManager>.HasInstance)
			{
				SingletonMonoScene<LevelSceneManager>.Instance.FinalNotifyLevelReady();
			}
		}
	}

	public void FinishRegister()
	{
		if (!CurIsHome)
		{
			registerPointFinished = true;
			if (registeredPointCount >= expectedPointCount)
			{
				SingletonMonoScene<LevelSceneManager>.Instance.FinalNotifyLevelReady();
			}
		}
	}

	private void RegisterMijingEnterPoint()
	{
		if (CurIsHome)
		{
			return;
		}
		if (!SingletonMonoScope<TeleportManager>.HasInstance)
		{
			LogUtil.Error("LevelRoot", "TeleportManager 不存在，无法注册秘境入口");
			return;
		}
		SingletonMonoScope<TeleportManager>.Instance.ResetMijingEnter();
		MijingEnterPoint[] componentsInChildren = GetComponentsInChildren<MijingEnterPoint>(includeInactive: true);
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			LogUtil.Warn("LevelRoot", "当前关卡 " + LevelManager.GetCurLevel() + " 未找到秘境入口标记点");
			return;
		}
		MijingEnterPoint[] array = componentsInChildren;
		foreach (MijingEnterPoint mijingEnterPoint in array)
		{
			if ((bool)mijingEnterPoint)
			{
				SingletonMonoScope<TeleportManager>.Instance.RegisterMijingEnter(mijingEnterPoint.transform.position, mijingEnterPoint);
			}
		}
	}

	private void Awake()
	{
		if (SingletonMonoScope<LevelManager>.HasInstance)
		{
			if (!bossLevelManager)
			{
				bossLevelManager = GetComponent<BossLevelManager>();
			}
			if (!CurIsHome && CurIsBoss && !bossLevelManager)
			{
				LogUtil.Error("当前Boss关卡 " + LevelManager.GetCurLevel() + " 未添加 BossLevelManager组件！！");
			}
			if (!base.gameObject.TryGetComponent<EnemyPointManager>(out var _))
			{
				LogUtil.Error("当前关卡 " + LevelManager.GetCurLevel() + " 未添加EnemyPointMnager组件！！");
			}
			if (!base.gameObject.TryGetComponent<LevelInteractablesManager>(out var _))
			{
				LogUtil.Error("当前关卡 " + LevelManager.GetCurLevel() + " 未添加LevelInteractablesManager组件！！");
			}
		}
		if (SceneManager.GetActiveScene().name == "HomeScene")
		{
			MijingToggle();
			BaoshiToggle();
		}
		if (SingletonMonoScope<MijingManager>.HasInstance)
		{
			int score = ((needScore >= 0) ? needScore : SingletonMonoScope<MijingManager>.Instance.mijingSettings.needScore);
			SingletonMonoScope<MijingManager>.Instance.RegisterNeedScore(score);
		}
		CacheMapSprite();
	}

	private void OnEnable()
	{
		if (!CurIsHome && CurIsBoss && (bool)bossLevelManager)
		{
			bossLevelManager.OnAllBossDefeated += OnBossAllDefeated;
		}
	}

	private void OnDisable()
	{
		if (!CurIsHome && CurIsBoss && (bool)bossLevelManager)
		{
			bossLevelManager.OnAllBossDefeated -= OnBossAllDefeated;
		}
	}

	private void OnDestroy()
	{
		UnregisterFromMapManager();
		ClearAll();
	}

	private void Start()
	{
		RegisterToMapManager();
		if (CurIsHome)
		{
			return;
		}
		if (SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetIsMijing())
		{
			RegisterMijingEnterPoint();
		}
		FinishRegister();
		if (SingletonMonoScope<LevelManager>.HasInstance)
		{
			if (CurIsBoss)
			{
				SetAllLocked(locked: true);
			}
			else
			{
				SetAllLocked(locked: false);
			}
		}
	}
}
