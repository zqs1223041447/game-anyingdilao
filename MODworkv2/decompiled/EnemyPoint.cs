using System;
using System.Collections.Generic;
using Core.Settings;
using Display;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Level.LevelStates;
using Level.StateData.LevelStates;
using Mijing;
using UnityEngine;
using Utils;

public class EnemyPoint : MonoBehaviour
{
	[Header("生成控制：是否自动生成")]
	public bool AutoSpawn = true;

	[Header("生成类型")]
	public EnemySpawnType SpawnType;

	[Header("Boss ID：仅当生成类型为Boss时启用")]
	public List<int> BossIds;

	[Header("距离检测：检测间隔")]
	public float time = 0.5f;

	private const float triggerAdd = 3f;

	private const float UnloadAdd = 3f;

	private string runtimeId;

	private EnemyPointLevelState pointState;

	private float checkTimer;

	private bool isBossSpawned;

	private float unloadBlockedUntil;

	private readonly List<Enemy> activeEnemies = new List<Enemy>();

	private readonly HashSet<EnemyState> activeStateSet = new HashSet<EnemyState>();

	public static int TotalSpawned;

	public static int TotalDespawned;

	public string RuntimeId => runtimeId;

	private static float TriggerDistance
	{
		get
		{
			if (DisplayMappingUtil.IsCurrentUltraWide())
			{
				return SettingsLoader.Instance.enemyTriggerDis + 3f;
			}
			return SettingsLoader.Instance.enemyTriggerDis;
		}
	}

	private static float UnloadDistance
	{
		get
		{
			if (DisplayMappingUtil.IsCurrentUltraWide())
			{
				return SettingsLoader.Instance.enemyUnloadDis + 3f;
			}
			return SettingsLoader.Instance.enemyUnloadDis;
		}
	}

	public int ActiveCount => activeEnemies.Count;

	public int TotalCount => (pointState?.EnemyStates?.Count).GetValueOrDefault();

	public int DeadCount
	{
		get
		{
			if (pointState?.EnemyStates == null)
			{
				return 0;
			}
			int num = 0;
			List<EnemyState> enemyStates = pointState.EnemyStates;
			for (int i = 0; i < enemyStates.Count; i++)
			{
				if (enemyStates[i].IsDead)
				{
					num++;
				}
			}
			return num;
		}
	}

	private void OnEnable()
	{
		if (SingletonMonoScene<EnemyPointManager>.HasInstance)
		{
			SingletonMonoScene<EnemyPointManager>.Instance.Register(this);
		}
	}

	private void OnDisable()
	{
		if (SingletonMonoScene<EnemyPointManager>.HasInstance)
		{
			SingletonMonoScene<EnemyPointManager>.Instance.Unregister(this);
		}
	}

	private void Awake()
	{
		if (SpawnType != EnemySpawnType.Boss)
		{
			runtimeId = RuntimeIdUtil.GenerateByIndex(base.transform);
			RegisterStateData();
		}
	}

	private void Update()
	{
		if (SpawnType == EnemySpawnType.Boss)
		{
			if (!SingletonMonoScope<PlayerManager>.HasInstance)
			{
				return;
			}
			checkTimer += Time.deltaTime;
			if (!(checkTimer < time))
			{
				checkTimer = 0f;
				if (AutoSpawn)
				{
					InitSpawn(SingletonMonoScope<PlayerManager>.Instance.transform.position);
				}
			}
		}
		else
		{
			if (!SingletonMonoScope<PlayerManager>.HasInstance)
			{
				return;
			}
			checkTimer += Time.deltaTime;
			if (!(checkTimer < time))
			{
				checkTimer = 0f;
				Vector3 position = SingletonMonoScope<PlayerManager>.Instance.transform.position;
				if (AutoSpawn)
				{
					InitSpawn(position);
				}
				HandleRestore(position);
				HandleUnload(position);
			}
		}
	}

	private void RegisterStateData()
	{
		LevelState currentLevelState = SingletonMonoGlobal<StateDataManager>.Instance.GetCurrentLevelState();
		bool flag = LevelManager.ShouldPersistLevelState(LevelManager.GetCurLevel());
		if (currentLevelState != null && flag)
		{
			if (currentLevelState.EnemyPoints == null)
			{
				currentLevelState.EnemyPoints = new Dictionary<string, EnemyPointLevelState>();
			}
			if (!currentLevelState.EnemyPoints.TryGetValue(runtimeId, out pointState))
			{
				pointState = new EnemyPointLevelState
				{
					EnemyStates = new List<EnemyState>()
				};
				currentLevelState.EnemyPoints.Add(runtimeId, pointState);
			}
		}
		else
		{
			pointState = new EnemyPointLevelState
			{
				EnemyStates = new List<EnemyState>()
			};
		}
	}

	private void HandleEnemyDie(Enemy enemy)
	{
		if ((bool)enemy)
		{
			enemy.OnEnemyDie = (Action<Enemy>)Delegate.Remove(enemy.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
		}
		if (SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetIsMijing() && SingletonMonoScope<MijingManager>.HasInstance)
		{
			switch (enemy.Quality)
			{
			case 0:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore0);
				break;
			case 1:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore1);
				break;
			case 2:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore2);
				break;
			case 3:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore3);
				break;
			case 4:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore4);
				break;
			case 5:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore5);
				break;
			}
		}
		activeEnemies.Remove(enemy);
		activeStateSet.Remove(enemy.RuntimeState);
		enemy.UnbindSpawnPoint();
	}

	private void InitSpawn(Vector3 playerPos)
	{
		float sqrMagnitude = (playerPos - base.transform.position).sqrMagnitude;
		float num = TriggerDistance * TriggerDistance;
		if (sqrMagnitude <= num)
		{
			if (SpawnType == EnemySpawnType.Boss)
			{
				SpawnBoss();
			}
			else if (pointState != null && pointState.EnemyStates.Count == 0 && activeEnemies.Count == 0)
			{
				SpawnEnemy();
			}
		}
	}

	public void PrewarmForTeleport(Vector3 targetPos, float unloadGraceTime)
	{
		if (!AutoSpawn)
		{
			return;
		}
		if (SpawnType == EnemySpawnType.Boss)
		{
			if (IsInTriggerRange(targetPos, base.transform.position))
			{
				InitSpawn(targetPos);
			}
		}
		else if (ShouldPrewarmForTeleport(targetPos))
		{
			unloadBlockedUntil = Mathf.Max(unloadBlockedUntil, Time.time + Mathf.Max(0f, unloadGraceTime));
			InitSpawn(targetPos);
			HandleRestore(targetPos);
		}
	}

	private bool ShouldPrewarmForTeleport(Vector3 targetPos)
	{
		if (IsInTriggerRange(targetPos, base.transform.position))
		{
			return true;
		}
		for (int i = 0; i < activeEnemies.Count; i++)
		{
			Enemy enemy = activeEnemies[i];
			if ((bool)enemy && IsInTriggerRange(targetPos, enemy.transform.position))
			{
				return true;
			}
		}
		if (pointState?.EnemyStates == null)
		{
			return false;
		}
		for (int j = 0; j < pointState.EnemyStates.Count; j++)
		{
			EnemyState enemyState = pointState.EnemyStates[j];
			if (!enemyState.IsDead && IsInTriggerRange(targetPos, enemyState.Position))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsInTriggerRange(Vector3 targetPos, Vector3 pointPos)
	{
		float num = TriggerDistance * TriggerDistance;
		return (targetPos - pointPos).sqrMagnitude <= num;
	}

	private void HandleRestore(Vector3 playerPos)
	{
		if (pointState?.EnemyStates == null || SpawnType == EnemySpawnType.Boss)
		{
			return;
		}
		float num = TriggerDistance * TriggerDistance;
		foreach (EnemyState enemyState in pointState.EnemyStates)
		{
			if (!enemyState.IsDead && !activeStateSet.Contains(enemyState) && (playerPos - enemyState.Position).sqrMagnitude <= num)
			{
				Enemy enemy;
				switch (SpawnType)
				{
				case EnemySpawnType.Normal:
					enemy = SingletonMonoScope<LevelManager>.Instance.RestoreEnemy(enemyState);
					break;
				case EnemySpawnType.Jingying:
					enemy = SingletonMonoScope<LevelManager>.Instance.RestoreJY(enemyState);
					break;
				default:
					LogUtil.Error("BOSS模式下不能恢复怪物数据！");
					return;
				}
				enemy.BindSpawnPoint(this);
				Enemy enemy2 = enemy;
				enemy2.OnEnemyDie = (Action<Enemy>)Delegate.Combine(enemy2.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
				activeEnemies.Add(enemy);
				activeStateSet.Add(enemyState);
			}
		}
	}

	private void SpawnEnemy()
	{
		List<Enemy> list;
		switch (SpawnType)
		{
		default:
			return;
		case EnemySpawnType.Normal:
			list = SingletonMonoScope<LevelManager>.Instance.CreatEnemies(base.transform);
			break;
		case EnemySpawnType.Jingying:
			list = SingletonMonoScope<LevelManager>.Instance.CreatJYs(base.transform);
			break;
		case EnemySpawnType.Boss:
			return;
		}
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (Enemy item in list)
		{
			ResetEnemyState(item);
			item.BindSpawnPoint(this);
			item.OnEnemyDie = (Action<Enemy>)Delegate.Combine(item.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
			activeEnemies.Add(item);
			if (item.RuntimeState != null)
			{
				pointState.EnemyStates.Add(item.RuntimeState);
				activeStateSet.Add(item.RuntimeState);
			}
			TotalSpawned++;
		}
	}

	private void SpawnBoss()
	{
		if (SpawnType != EnemySpawnType.Boss)
		{
			return;
		}
		int index = UnityEngine.Random.Range(0, BossIds.Count);
		if (!isBossSpawned)
		{
			Enemy enemy = SingletonMonoScope<LevelManager>.Instance.CreatBoss(this, base.transform, BossIds[index]);
			if ((bool)enemy)
			{
				enemy.OnEnemyDie = (Action<Enemy>)Delegate.Combine(enemy.OnEnemyDie, new Action<Enemy>(HandleBossDie));
			}
		}
		isBossSpawned = true;
	}

	private static void HandleBossDie(Enemy enemy)
	{
		if ((bool)enemy)
		{
			enemy.OnEnemyDie = (Action<Enemy>)Delegate.Remove(enemy.OnEnemyDie, new Action<Enemy>(HandleBossDie));
		}
		if (SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetIsMijing() && SingletonMonoScope<MijingManager>.HasInstance)
		{
			switch (enemy.Quality)
			{
			case 0:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore0);
				break;
			case 1:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore1);
				break;
			case 2:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore2);
				break;
			case 3:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore3);
				break;
			case 4:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore4);
				break;
			case 5:
				SingletonMonoScope<MijingManager>.Instance.AddCurrentScore(SingletonMonoScope<MijingManager>.Instance.mijingSettings.EmScore5);
				break;
			}
		}
	}

	private void HandleUnload(Vector3 playerPos)
	{
		if (SpawnType == EnemySpawnType.Boss || Time.time < unloadBlockedUntil)
		{
			return;
		}
		float num = UnloadDistance * UnloadDistance;
		for (int num2 = activeEnemies.Count - 1; num2 >= 0; num2--)
		{
			Enemy enemy = activeEnemies[num2];
			if (!enemy)
			{
				activeEnemies.RemoveAt(num2);
			}
			else if (!enemy.IsDpsTarget && (playerPos - enemy.transform.position).sqrMagnitude >= num)
			{
				if (enemy.RuntimeState != null && !enemy.RuntimeState.IsDead)
				{
					enemy.UpdateStatePos(enemy.transform.position);
					enemy.UpdateStateHp(enemy.HealthStat.CurrentValue);
				}
				enemy.OnEnemyDie = (Action<Enemy>)Delegate.Remove(enemy.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
				activeStateSet.Remove(enemy.RuntimeState);
				enemy.UnbindSpawnPoint();
				enemy.OnDespawn();
				activeEnemies.RemoveAt(num2);
				TotalDespawned++;
			}
		}
	}

	public void FlushActiveEnemiesToState()
	{
		if (SpawnType == EnemySpawnType.Boss || pointState == null)
		{
			return;
		}
		for (int num = activeEnemies.Count - 1; num >= 0; num--)
		{
			Enemy enemy = activeEnemies[num];
			if (!enemy)
			{
				activeEnemies.RemoveAt(num);
			}
			else if (enemy.RuntimeState != null && !enemy.RuntimeState.IsDead)
			{
				enemy.UpdateStatePos(enemy.transform.position);
				enemy.UpdateStateHp(enemy.HealthStat.CurrentValue);
			}
		}
	}

	private static void ResetEnemyState(Enemy enemy)
	{
		enemy.HealthStat.SetCurrent(enemy.HealthStat.MaxValue);
	}

	public void SpawnTestEnemy(int id, int count)
	{
		List<Enemy> list = SingletonMonoScope<LevelManager>.Instance.CreatTest(base.transform, id, count);
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (Enemy item in list)
		{
			ResetEnemyState(item);
			item.BindSpawnPoint(this);
			item.OnEnemyDie = (Action<Enemy>)Delegate.Combine(item.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
			activeEnemies.Add(item);
			if (item.RuntimeState != null)
			{
				pointState.EnemyStates.Add(item.RuntimeState);
				activeStateSet.Add(item.RuntimeState);
			}
			TotalSpawned++;
		}
	}

	public void SpawnTestJY()
	{
		List<Enemy> list = SingletonMonoScope<LevelManager>.Instance.CreatJYs(base.transform);
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (Enemy item in list)
		{
			ResetEnemyState(item);
			item.BindSpawnPoint(this);
			item.OnEnemyDie = (Action<Enemy>)Delegate.Combine(item.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
			activeEnemies.Add(item);
			if (item.RuntimeState != null)
			{
				pointState.EnemyStates.Add(item.RuntimeState);
				activeStateSet.Add(item.RuntimeState);
			}
			TotalSpawned++;
		}
	}
}
