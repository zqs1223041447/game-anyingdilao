using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FlySowrdFSQ : MonoBehaviour
{
	public int type;

	public float speed;

	public float lerpAngle;

	public string[] SoundA;

	public GameObject[] OBJ;

	public GameObject[] FX;

	public Transform[] point;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public List<SK_FlySowrd> sowrd = new List<SK_FlySowrd>();

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	public Collider2D[] hitEM = new Collider2D[6];

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public Transform core;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private bool CanAT;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		core = base.transform.Find("core");
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		em.Clear();
		sowrd.Clear();
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		if (sp.NoTime == 1)
		{
			timeA += Time.deltaTime;
			if (timeA >= sp.BuffTime)
			{
				Stop();
				return;
			}
			if ((bool)mg && mg.NeedStop)
			{
				Stop();
				return;
			}
		}
		else if ((bool)mg && mg.ORBStop)
		{
			Stop();
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= 0.25f)
		{
			timeB = 0f;
			if (sp.SpecialType == 10 && (bool)PL)
			{
				PL.RefreshORB(sp, 0);
			}
			ScanEnemies();
			Refresh();
		}
		timeC += Time.deltaTime;
		if (timeC >= 2f)
		{
			timeC = 0f;
			RefreshB();
		}
		timeD += Time.deltaTime;
		if (timeD >= 0.2f)
		{
			timeD = 0f;
			CleanupSwordList();
			EnsureSwords();
		}
	}

	private void LateUpdate()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized && CanInitialize())
		{
			initialized = true;
			SetStart();
		}
	}

	private bool CanInitialize()
	{
		Dicform component = GetComponent<Dicform>();
		if (component != null && component.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		if (point != null && point.Length != 0 && OBJ != null && (bool)sp && sp.MainEL >= 0 && sp.MainEL < OBJ.Length && (bool)OBJ[sp.MainEL])
		{
			CanAT = true;
			if ((bool)core)
			{
				core.localScale = new Vector3(sp.Size, sp.Size, sp.Size);
			}
			EnsureSwords();
			if (SoundA != null && sp.Sound >= 0 && sp.Sound < SoundA.Length && !string.IsNullOrEmpty(SoundA[sp.Sound]))
			{
				RuntimeManager.PlayOneShot(SoundA[sp.Sound], base.transform.position);
			}
		}
	}

	private void ScanEnemies()
	{
		if (hitEM == null || hitEM.Length == 0)
		{
			return;
		}
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, sp.Range1, hitEM, LayerMask.GetMask("BodyCOLem"));
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = hitEM[i];
			hitEM[i] = null;
			if (!collider2D)
			{
				continue;
			}
			BodyCOL component = collider2D.GetComponent<BodyCOL>();
			if ((bool)component && (bool)component.peo && (bool)component.peo.em)
			{
				Enemy enemy = component.peo.em;
				if (component.peo.CharacterType == 2 && enemy.IsAlive && !enemy.IsJump && !enemy.IsYS && !em.Contains(enemy))
				{
					em.Add(enemy);
				}
			}
		}
	}

	private void CleanupSwordList()
	{
		for (int num = sowrd.Count - 1; num >= 0; num--)
		{
			SK_FlySowrd sK_FlySowrd = sowrd[num];
			if (!sK_FlySowrd || !sK_FlySowrd.gameObject.activeInHierarchy)
			{
				sowrd.RemoveAt(num);
			}
		}
	}

	private void EnsureSwords()
	{
		if (!CanAT || !sp || point == null || point.Length == 0 || OBJ == null || sp.MainEL < 0 || sp.MainEL >= OBJ.Length || !OBJ[sp.MainEL])
		{
			return;
		}
		int targetSwordCount = GetTargetSwordCount();
		TrimSwords(targetSwordCount);
		for (int i = 0; i < targetSwordCount; i++)
		{
			if (!point[i])
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < sowrd.Count; j++)
			{
				if ((bool)sowrd[j] && sowrd[j].point == point[i])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			SK_FlySowrd sK_FlySowrd = SpawnSword(i);
			if ((bool)sK_FlySowrd && (bool)PL)
			{
				bool countedPrefab = false;
				switch (type)
				{
				case 0:
					PL.PrefabCount(6, add: true);
					countedPrefab = true;
					break;
				case 1:
					PL.PrefabCount(20, add: true);
					countedPrefab = true;
					break;
				case 2:
					PL.PrefabCount(7, add: true);
					countedPrefab = true;
					break;
				}
				sK_FlySowrd.countedPrefab = countedPrefab;
			}
		}
	}

	private int GetTargetSwordCount()
	{
		if (!sp || point == null)
		{
			return 0;
		}
		int num = Mathf.Max(0, sp.Count_ORB);
		if (type == 1 && (bool)PL && PL.BladeSoul_Double)
		{
			num *= 2;
		}
		return Mathf.Min(num, point.Length);
	}

	private void TrimSwords(int targetCount)
	{
		for (int num = sowrd.Count - 1; num >= 0; num--)
		{
			SK_FlySowrd sK_FlySowrd = sowrd[num];
			if (!sK_FlySowrd)
			{
				sowrd.RemoveAt(num);
			}
			else if (!ShouldKeepSword(sK_FlySowrd, targetCount))
			{
				sK_FlySowrd.Stop();
			}
		}
	}

	private bool ShouldKeepSword(SK_FlySowrd sword, int targetCount)
	{
		if (!sword || !sword.point || point == null)
		{
			return false;
		}
		for (int i = 0; i < targetCount; i++)
		{
			if (sword.point == point[i])
			{
				return true;
			}
		}
		return false;
	}

	private SK_FlySowrd SpawnSword(int index)
	{
		if (index < 0 || point == null || index >= point.Length || point[index] == null)
		{
			return null;
		}
		if (!sp)
		{
			return null;
		}
		if (OBJ == null || sp.MainEL < 0 || sp.MainEL >= OBJ.Length || !OBJ[sp.MainEL])
		{
			return null;
		}
		Vector3 right = point[index].right;
		float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
		GameObject gameObject = LeanPool.Spawn(OBJ[sp.MainEL], point[index].position, Quaternion.Euler(0f, 0f, num + 90f));
		if (!gameObject)
		{
			return null;
		}
		SK_FlySowrd component = gameObject.GetComponent<SK_FlySowrd>();
		if (!component)
		{
			LeanPool.Despawn(gameObject);
			return null;
		}
		component.point = point[index];
		component.father = this;
		component.speed = speed;
		component.target = null;
		component.countedPrefab = false;
		sowrd.Add(component);
		return component;
	}

	public void NotifySwordDespawn(SK_FlySowrd sword)
	{
		if ((bool)sword)
		{
			sowrd.Remove(sword);
		}
	}

	public void Stop()
	{
		CanAT = false;
		em.Clear();
		for (int num = sowrd.Count - 1; num >= 0; num--)
		{
			if ((bool)sowrd[num])
			{
				sowrd[num].Stop();
			}
		}
		sowrd.Clear();
		this.wait(0.0001f, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void Refresh()
	{
		for (int num = em.Count - 1; num >= 0; num--)
		{
			Enemy enemy = em[num];
			if (!enemy || !enemy.IsAlive || enemy.IsJump || enemy.IsYS || Vector3.Distance(enemy.yao.transform.position, base.transform.position) > sp.Range1)
			{
				for (int i = 0; i < sowrd.Count; i++)
				{
					if ((bool)sowrd[i] && sowrd[i].target == enemy)
					{
						sowrd[i].target = null;
					}
				}
				em.RemoveAt(num);
			}
		}
	}

	public void RefreshB()
	{
		em.Sort(delegate(Enemy t1, Enemy t2)
		{
			if (!t1 && !t2)
			{
				return 0;
			}
			if (!t1)
			{
				return 1;
			}
			return t2 ? Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)) : (-1);
		});
	}
}
