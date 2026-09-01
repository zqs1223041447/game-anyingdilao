using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Road : MonoBehaviour
{
	public ParticleSystem[] par;

	public GameObject colOBJ;

	public float colTime;

	private float colTimeTmp;

	public float colDELtime;

	public float size;

	[Header("=========")]
	public float DotMulti;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public List<SkillCOL> colList = new List<SkillCOL>();

	[HideInInspector]
	public List<FootCOL> em = new List<FootCOL>();

	private bool CanAT;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeO;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		colList.Clear();
		em.Clear();
		CanAT = false;
		timeA = 0f;
		timeB = 0f;
		timeO = 0f;
		colTimeTmp = colTime;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		timeC += Time.deltaTime;
		if (timeC >= colTimeTmp)
		{
			timeC = 0f;
			if (PL.IsAlive)
			{
				SkillCOL component = LeanPool.Spawn(colOBJ, base.transform.position, Quaternion.identity).GetComponent<SkillCOL>();
				component.father = this;
				component.size = size;
				component.lifeTime = colDELtime;
				colList.Add(component);
			}
		}
		if (PL.IsChong)
		{
			colTimeTmp = colTime / 5f;
		}
		else
		{
			colTimeTmp = colTime;
		}
		timeB += Time.deltaTime;
		if (timeB >= 0.5f)
		{
			timeB = 0f;
			for (int i = 0; i < em.Count; i++)
			{
				if (em[i].peo.em.IsAlive)
				{
					em[i].peo.EM_Set(sp, DotMulti, 0, Dot_Infect: false, 0, 0f);
				}
			}
		}
		if (sp.NoTime == 1)
		{
			timeA += Time.deltaTime;
			if (timeA >= sp.BuffTime)
			{
				timeA = 0f;
				TimeStop();
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
		else if (mg.ORBStop)
		{
			Stop();
		}
		if (sp.SpecialType == 10)
		{
			timeO += Time.deltaTime;
			if (timeO >= 0.15f)
			{
				if (sp.SpecialType == 10)
				{
					PL.RefreshORB(sp, 0);
				}
				timeO = 0f;
			}
		}
		if (mg.NeedStop)
		{
			Stop();
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
		for (int i = 0; i < par.Length; i++)
		{
			ParticleSystem.MainModule main = par[i].main;
			main.loop = true;
		}
		if (sp.indexType == 0)
		{
			PL.PrefabCount(84, add: true);
		}
		CanAT = true;
	}

	public void Add(FootCOL col)
	{
		if (!em.Contains(col))
		{
			em.Add(col);
		}
	}

	public void Del(FootCOL col)
	{
		if (em.Contains(col))
		{
			em.Remove(col);
		}
	}

	public void TimeStop()
	{
		for (int i = 0; i < par.Length; i++)
		{
			ParticleSystem.MainModule main = par[i].main;
			main.loop = false;
		}
		this.wait(2f, delegate
		{
			CanAT = false;
		});
		this.wait(4f, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void Stop()
	{
		for (int i = 0; i < par.Length; i++)
		{
			ParticleSystem.MainModule main = par[i].main;
			main.loop = false;
		}
		CanAT = false;
		if (sp.indexType == 0)
		{
			PL.PrefabCount(84, add: false);
		}
		this.wait(4f, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
