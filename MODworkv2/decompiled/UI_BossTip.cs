using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossTip : MonoBehaviour
{
	private static UI_BossTip instance;

	public List<Enemy> boss = new List<Enemy>();

	public EnemyStat stat;

	public Text text;

	public float timeA;

	public float timeB;

	public Text[] SPCtext;

	[HideInInspector]
	public PlayerManager PL;

	public static UI_BossTip Instance
	{
		get
		{
			if (!instance)
			{
				instance = Object.FindObjectOfType<UI_BossTip>();
			}
			return instance;
		}
	}

	public static bool HasInstance => instance != null;

	private void Awake()
	{
		if ((bool)SingletonMonoScope<PlayerManager>.Instance)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
		}
	}

	private void Start()
	{
		timeA = 0f;
		timeB = 0f;
		if (boss.Count > 0)
		{
			text.text = boss[0].IndexName;
		}
	}

	private void Update()
	{
		boss.RemoveAll((Enemy e) => !e);
		if (boss.Count > 0)
		{
			stat.CurrentValue = boss[0].HealthStat.CurrentValue;
			stat.MaxValue = boss[0].HealthStat.MaxValue;
			timeA += Time.deltaTime;
			if (!(timeA >= 0.5f))
			{
				return;
			}
			if (boss.Count > 1 && (bool)PL)
			{
				boss.Sort(delegate(Enemy a, Enemy b)
				{
					if (!a && !b)
					{
						return 0;
					}
					if (!a)
					{
						return 1;
					}
					return b ? Vector3.Distance(a.transform.position, PL.transform.position).CompareTo(Vector3.Distance(b.transform.position, PL.transform.position)) : (-1);
				});
			}
			if (Vector3.Distance(PL.transform.position, boss[0].transform.position) > 10f)
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowBossTip(0);
			}
			else
			{
				ShowBoss();
			}
			timeA = 0f;
		}
		else
		{
			SingletonMonoScope<GameUIManager>.Instance.ShowBossTip(0);
		}
	}

	public void ShowBoss()
	{
		stat.Initialize(boss[0].HealthStat.CurrentValue, boss[0].HealthStat.MaxValue);
		text.text = $"{LOC.MM.GetEnemy(boss[0].IndexName)}   LV.{boss[0].Level}";
		SPCtext[0].text = LOC.MM.GetMain("Force_SS");
		SPCtext[1].text = LOC.MM.GetMain("Strong_SS");
		SPCtext[2].text = LOC.MM.GetMain("StoneSkin_SS");
		SPCtext[3].text = LOC.MM.GetMain("MY_SS");
		SPCtext[4].text = LOC.MM.GetMain("MagicAnti_SS");
		SingletonMonoScope<GameUIManager>.Instance.ShowBossTip(1);
	}

	public void ClearBoss()
	{
		boss.Clear();
		SingletonMonoScope<GameUIManager>.Instance.ShowBossTip(0);
	}
}
