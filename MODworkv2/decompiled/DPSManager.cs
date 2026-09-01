using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DPSManager : SingletonMonoScope<DPSManager>
{
	private struct DamageSample
	{
		public float Time;

		public float Damage;

		public float DotDamage;
	}

	private const float AverageWindow = 1f;

	private const float SecondSettleInterval = 1f;

	private const float MaxDisplayInterval = 1f;

	private const string HomeSceneName = "HomeScene";

	private readonly List<DamageSample> damageSamples = new List<DamageSample>(128);

	private readonly HashSet<Enemy> dpsTargets = new HashSet<Enemy>();

	private float secondTimer;

	private float maxDisplayTimer;

	private float secondDamage;

	private float currentDps;

	private float currentDotDps;

	private float maxDps;

	private float maxDisplayDps;

	public float CurrentDps => currentDps;

	public float CurrentDotDps => currentDotDps;

	public float MaxDps => maxDps;

	public float MaxDisplayDps => maxDisplayDps;

	public event Action<Enemy, float, bool> OnDamageRecorded;

	protected override void OnSingletonAwake()
	{
		ResetDps();
	}

	private void Update()
	{
		TickDps();
	}

	public void RegisterTarget(Enemy enemy)
	{
		if ((bool)enemy)
		{
			dpsTargets.Add(enemy);
		}
	}

	public void UnregisterTarget(Enemy enemy)
	{
		if ((bool)enemy)
		{
			dpsTargets.Remove(enemy);
		}
	}

	public void ResetDps()
	{
		damageSamples.Clear();
		secondTimer = 0f;
		maxDisplayTimer = 0f;
		secondDamage = 0f;
		currentDps = 0f;
		currentDotDps = 0f;
		maxDps = 0f;
		maxDisplayDps = 0f;
	}

	public void ResetMaxDps()
	{
		secondTimer = 0f;
		maxDisplayTimer = 0f;
		secondDamage = 0f;
		maxDps = 0f;
		maxDisplayDps = 0f;
	}

	public void RecordDamage(Enemy enemy, float damage, bool dotDamage)
	{
		if (!(damage <= 0f) && IsHomeScene() && (bool)enemy && enemy.IsDpsTarget && (dpsTargets.Count <= 0 || dpsTargets.Contains(enemy)))
		{
			float time = Time.time;
			damageSamples.Add(new DamageSample
			{
				Time = time,
				Damage = damage,
				DotDamage = (dotDamage ? damage : 0f)
			});
			secondDamage += damage;
			this.OnDamageRecorded?.Invoke(enemy, damage, dotDamage);
		}
	}

	private void TickDps()
	{
		float time = Time.time;
		TrimSamples(time);
		RecalculateAverages(time);
		SettleMaxDamage();
	}

	private void TrimSamples(float now)
	{
		float num = now - 1f;
		for (int num2 = damageSamples.Count - 1; num2 >= 0; num2--)
		{
			if (damageSamples[num2].Time < num)
			{
				damageSamples.RemoveAt(num2);
			}
		}
	}

	private void RecalculateAverages(float now)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = now - 1f;
		for (int i = 0; i < damageSamples.Count; i++)
		{
			DamageSample damageSample = damageSamples[i];
			if (!(damageSample.Time < num3))
			{
				num += damageSample.Damage;
				num2 += damageSample.DotDamage;
			}
		}
		currentDps = num / 1f;
		currentDotDps = num2 / 1f;
	}

	private void SettleMaxDamage()
	{
		secondTimer += Time.deltaTime;
		maxDisplayTimer += Time.deltaTime;
		if (secondTimer >= 1f)
		{
			if (secondDamage > maxDps)
			{
				maxDps = secondDamage;
			}
			secondDamage = 0f;
			secondTimer = 0f;
		}
		if (maxDisplayTimer >= 1f)
		{
			maxDisplayDps = maxDps;
			maxDisplayTimer = 0f;
		}
	}

	private static bool IsHomeScene()
	{
		return SceneManager.GetActiveScene().name == "HomeScene";
	}

	public static string FormatDamageNumber(float number)
	{
		return DamgeTextManager.FormatDamageNumber(number);
	}
}
