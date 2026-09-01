using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Localization;
using UnityEngine;
using UnityEngine.UI;

public class WoodTest : MonoBehaviour
{
	public int Index;

	public EnemyPoint emp1;

	public EnemyPoint emp2;

	public Text Max;

	public Text DPS;

	public Text Dot;

	[Header("DPS木桩")]
	[HideInInspector]
	public int emp1Count;

	[HideInInspector]
	public int emp2Count;

	[HideInInspector]
	public float emp1SpawnRadius;

	[HideInInspector]
	public float fullHealDelay;

	[HideInInspector]
	public float textRefreshInterval;

	[HideInInspector]
	public float maxDpsResetDelay;

	public List<Enemy> emp1Enemies = new List<Enemy>();

	public List<Enemy> emp2Enemies = new List<Enemy>();

	private readonly Dictionary<Enemy, float> lastDamageTimes = new Dictionary<Enemy, float>();

	private float textTimer;

	private float maxDpsResetTimer;

	private bool hasDpsDamage;

	private bool initialized;

	private bool subscribedToDps;

	private bool subscribedToLocalization;

	private void OnEnable()
	{
		emp1Count = 15;
		emp2Count = 1;
		emp1SpawnRadius = 1.5f;
		fullHealDelay = 3f;
		textRefreshInterval = 0.3f;
		maxDpsResetDelay = 5f;
		SubscribeDpsManager();
		SubscribeLocalization();
	}

	private void Start()
	{
		SubscribeDpsManager();
		SubscribeLocalization();
		TryInit();
		RefreshText();
	}

	private void OnDisable()
	{
		if (subscribedToDps && SingletonMonoScope<DPSManager>.HasInstance)
		{
			SingletonMonoScope<DPSManager>.Instance.OnDamageRecorded -= HandleDamageRecorded;
		}
		subscribedToDps = false;
		if (subscribedToLocalization)
		{
			LOC.MM.OnLanguageChanged -= HandleLanguageChanged;
			subscribedToLocalization = false;
		}
		UnregisterAllTargets(emp1Enemies);
		UnregisterAllTargets(emp2Enemies);
		lastDamageTimes.Clear();
		ResetMaxDpsResetState();
	}

	private void Update()
	{
		SubscribeDpsManager();
		SubscribeLocalization();
		if (!initialized)
		{
			TryInit();
		}
		MaintainList(emp1Enemies, emp1, emp1Count, randomInCircle: true);
		MaintainList(emp2Enemies, emp2, emp2Count, randomInCircle: false);
		HealIdleTargets(emp1Enemies);
		HealIdleTargets(emp2Enemies);
		UpdateMaxDpsResetTimer();
		RefreshTextByTimer();
	}

	private void TryInit()
	{
		if (!SingletonMonoScope<LevelManager>.HasInstance || !SingletonMonoScope<GameDataManager>.HasInstance || !SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return;
		}
		SubscribeDpsManager();
		if (LevelManager.GetMB(Index) != null)
		{
			if ((bool)emp1)
			{
				emp1.AutoSpawn = false;
			}
			if ((bool)emp2)
			{
				emp2.AutoSpawn = false;
			}
			MaintainList(emp1Enemies, emp1, emp1Count, randomInCircle: true);
			MaintainList(emp2Enemies, emp2, emp2Count, randomInCircle: false);
			initialized = emp1Enemies.Count >= emp1Count && emp2Enemies.Count >= emp2Count;
		}
	}

	private void SubscribeDpsManager()
	{
		if (!subscribedToDps && SingletonMonoScope<DPSManager>.HasInstance)
		{
			SingletonMonoScope<DPSManager>.Instance.OnDamageRecorded += HandleDamageRecorded;
			SingletonMonoScope<DPSManager>.Instance.ResetDps();
			ResetMaxDpsResetState();
			RegisterExistingTargets(emp1Enemies);
			RegisterExistingTargets(emp2Enemies);
			subscribedToDps = true;
		}
	}

	private void SubscribeLocalization()
	{
		if (!subscribedToLocalization)
		{
			LOC.MM.OnLanguageChanged += HandleLanguageChanged;
			subscribedToLocalization = true;
		}
	}

	private void HandleLanguageChanged(LanguageType lang)
	{
		RefreshText();
	}

	private static void RegisterExistingTargets(List<Enemy> list)
	{
		if (!SingletonMonoScope<DPSManager>.HasInstance)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Enemy enemy = list[i];
			if ((bool)enemy)
			{
				SingletonMonoScope<DPSManager>.Instance.RegisterTarget(enemy);
			}
		}
	}

	private void MaintainList(List<Enemy> list, EnemyPoint point, int targetCount, bool randomInCircle)
	{
		if (targetCount <= 0)
		{
			return;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			Enemy enemy = list[num];
			if (!enemy || !enemy.gameObject.activeInHierarchy || !enemy.IsAlive)
			{
				RemoveTarget(list, enemy);
			}
		}
		while (list.Count < targetCount)
		{
			Enemy enemy2 = SpawnDpsEnemy(point, randomInCircle);
			if ((bool)enemy2)
			{
				list.Add(enemy2);
				continue;
			}
			break;
		}
	}

	private Enemy SpawnDpsEnemy(EnemyPoint point, bool randomInCircle)
	{
		if (!SingletonMonoScope<LevelManager>.HasInstance)
		{
			return null;
		}
		Transform transform = (point ? point.transform : base.transform);
		List<Enemy> list = SingletonMonoScope<LevelManager>.Instance.CreatTest(transform, Index, 1);
		if (list == null || list.Count == 0 || !list[0])
		{
			return null;
		}
		Enemy enemy = list[0];
		Vector3 position = transform.position;
		if (randomInCircle)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * emp1SpawnRadius;
			position += new Vector3(vector.x, vector.y, 0f);
		}
		PrepareDpsEnemy(enemy, point, position);
		return enemy;
	}

	private void PrepareDpsEnemy(Enemy enemy, EnemyPoint point, Vector3 spawnPos)
	{
		enemy.transform.position = spawnPos;
		enemy.IS_Comp = false;
		enemy.IS_FS = false;
		enemy.Father = null;
		enemy.BindSpawnPoint(point);
		enemy.IsDpsTarget = true;
		enemy.OnEnemyDie = (Action<Enemy>)Delegate.Remove(enemy.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
		enemy.OnEnemyDie = (Action<Enemy>)Delegate.Combine(enemy.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
		enemy.EnemyClear();
		enemy.Quality = 2;
		enemy.Xp = 0;
		enemy.Can_DieBoom = false;
		enemy.SK_Rate = 0;
		enemy.SK_Rate_Comp = 0;
		enemy.SK_Rate_FS = 0;
		enemy.SK_Rate_ELSS = 0;
		enemy.CF_Rate = 0;
		enemy.FS_Count = 0;
		enemy.ELSS_Break = false;
		for (int i = 0; i < enemy.SSIndex.Length; i++)
		{
			enemy.SSIndex[i] = 0;
		}
		float num = (enemy.Health_Base = Mathf.Max(1f, GetPlayerBaseDamage() * 10000f));
		enemy.Health_Bei = 0f;
		enemy.Health_Prc = 0f;
		enemy.Damage_Base = 1f;
		enemy.Damage_Bei = 0f;
		enemy.Damage_Cut = 0f;
		enemy.Damage_Last = 1f;
		enemy.AttackSpeed_Bei = 0f;
		enemy.AttackSpeed_Cut = 0f;
		enemy.MoveSpeed_Bei = 0f;
		enemy.MoveSpeed_Cut = 0f;
		enemy.Chuan = 0f;
		enemy.BJRate = 0f;
		enemy.GeDang = 0f;
		enemy.yunAnti = 0f;
		enemy.yunAnti_Last = 0f;
		enemy.DamageAnti = 10f;
		enemy.DotDamage = 0f;
		enemy.DotTime = 0f;
		enemy.DotTimeCut = 0f;
		enemy.AntiSlow = 0f;
		SetAllAnti(enemy, 50f);
		if ((bool)enemy.HealthStat)
		{
			enemy.HealthStat.Initialize(num, num);
		}
		if (enemy.RuntimeState != null)
		{
			enemy.RuntimeState.Position = spawnPos;
			enemy.RuntimeState.Hp = num;
			enemy.RuntimeState.QQ = enemy.Quality;
			enemy.RuntimeState.IsDead = false;
		}
		lastDamageTimes[enemy] = Time.time;
		if (SingletonMonoScope<DPSManager>.HasInstance)
		{
			SingletonMonoScope<DPSManager>.Instance.RegisterTarget(enemy);
		}
	}

	private static void SetAllAnti(Enemy enemy, float value)
	{
		enemy.FireAnti = value;
		enemy.FrozenAnti = value;
		enemy.ThunderAnti = value;
		enemy.PoisonAnti = value;
		enemy.PhysicsAnti = value;
		enemy.ShadowAnti = value;
		enemy.FireAnti_Last = value;
		enemy.FrozenAnti_Last = value;
		enemy.ThunderAnti_Last = value;
		enemy.PoisonAnti_Last = value;
		enemy.PhysicsAnti_Last = value;
		enemy.ShadowAnti_Last = value;
		enemy.FireAntiCut_Simple = 0f;
		enemy.FrozenAntiCut_Simple = 0f;
		enemy.ThunderAntiCut_Simple = 0f;
		enemy.PoisonAntiCut_Simple = 0f;
		enemy.PhysicsAntiCut_Simple = 0f;
		enemy.ShadowAntiCut_Simple = 0f;
		enemy.FireAntiCut_Dot = 0f;
		enemy.FrozenAntiCut_Dot = 0f;
		enemy.ThunderAntiCut_Dot = 0f;
		enemy.PoisonAntiCut_Dot = 0f;
		enemy.PhysicsAntiCut_Dot = 0f;
		enemy.ShadowAntiCut_Dot = 0f;
	}

	private static float GetPlayerBaseDamage()
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return 1f;
		}
		float num = 0f;
		for (int i = 0; i < 6; i++)
		{
			num = Mathf.Max(num, SingletonMonoScope<PlayerManager>.Instance.GiveDamage(i));
		}
		return Mathf.Max(1f, num);
	}

	private void HandleEnemyDie(Enemy enemy)
	{
		RemoveTarget(emp1Enemies, enemy);
		RemoveTarget(emp2Enemies, enemy);
	}

	private void HandleDamageRecorded(Enemy enemy, float damage, bool dotDamage)
	{
		if ((bool)enemy)
		{
			if (lastDamageTimes.ContainsKey(enemy))
			{
				lastDamageTimes[enemy] = Time.time;
			}
			hasDpsDamage = true;
			maxDpsResetTimer = 0f;
		}
	}

	private void ResetMaxDpsResetState()
	{
		hasDpsDamage = false;
		maxDpsResetTimer = 0f;
	}

	private void UpdateMaxDpsResetTimer()
	{
		if (hasDpsDamage && SingletonMonoScope<DPSManager>.HasInstance)
		{
			maxDpsResetTimer += Time.deltaTime;
			if (!(maxDpsResetTimer < maxDpsResetDelay))
			{
				SingletonMonoScope<DPSManager>.Instance.ResetMaxDps();
				ResetMaxDpsResetState();
				RefreshText();
			}
		}
	}

	private void HealIdleTargets(List<Enemy> list)
	{
		float time = Time.time;
		for (int i = 0; i < list.Count; i++)
		{
			Enemy enemy = list[i];
			if ((bool)enemy && enemy.IsAlive && (bool)enemy.HealthStat)
			{
				if (!lastDamageTimes.TryGetValue(enemy, out var value))
				{
					lastDamageTimes[enemy] = time;
				}
				else if (time - value >= fullHealDelay && enemy.HealthStat.CurrentValue < enemy.HealthStat.MaxValue)
				{
					enemy.HealthStat.SetCurrent(enemy.HealthStat.MaxValue);
				}
			}
		}
	}

	private void RemoveTarget(List<Enemy> list, Enemy enemy)
	{
		if ((bool)enemy)
		{
			enemy.OnEnemyDie = (Action<Enemy>)Delegate.Remove(enemy.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
			enemy.UnbindSpawnPoint();
			if (SingletonMonoScope<DPSManager>.HasInstance)
			{
				SingletonMonoScope<DPSManager>.Instance.UnregisterTarget(enemy);
			}
			lastDamageTimes.Remove(enemy);
		}
		list.Remove(enemy);
	}

	private void UnregisterAllTargets(List<Enemy> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Enemy enemy = list[i];
			if ((bool)enemy)
			{
				enemy.OnEnemyDie = (Action<Enemy>)Delegate.Remove(enemy.OnEnemyDie, new Action<Enemy>(HandleEnemyDie));
				if (SingletonMonoScope<DPSManager>.HasInstance)
				{
					SingletonMonoScope<DPSManager>.Instance.UnregisterTarget(enemy);
				}
			}
		}
	}

	private void RefreshTextByTimer()
	{
		textTimer += Time.deltaTime;
		if (!(textTimer < textRefreshInterval))
		{
			textTimer = 0f;
			RefreshText();
		}
	}

	private void RefreshText()
	{
		float number = 0f;
		float number2 = 0f;
		float number3 = 0f;
		if (SingletonMonoScope<DPSManager>.HasInstance)
		{
			number = SingletonMonoScope<DPSManager>.Instance.MaxDisplayDps;
			number2 = SingletonMonoScope<DPSManager>.Instance.CurrentDps;
			number3 = SingletonMonoScope<DPSManager>.Instance.CurrentDotDps;
		}
		if ((bool)Max)
		{
			Max.text = GetLabel("DPS_MaxDamage") + "：" + DPSManager.FormatDamageNumber(number);
		}
		if ((bool)DPS)
		{
			DPS.text = GetLabel("DPS_DamagePerSecond") + "：" + DPSManager.FormatDamageNumber(number2);
		}
		if ((bool)Dot)
		{
			Dot.text = GetLabel("DPS_DotDamage") + "：" + DPSManager.FormatDamageNumber(number3);
		}
	}

	private static string GetLabel(string key)
	{
		if (LOC.MM == null)
		{
			return key;
		}
		return LOC.MM.GetMain(key);
	}
}
