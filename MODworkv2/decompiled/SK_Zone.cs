using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Zone : MonoBehaviour
{
	public GameObject FX;

	public ParticleSystem[] parOne;

	public ParticleSystem[] parLoop;

	public float size;

	public float DelDelay;

	public float DotMulti;

	public bool Body;

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private bool CanAT;

	private float timeA;

	private float timeB;

	private float timeO;

	private PlayerManager _playerManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeO = 0f;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= 0.5f)
		{
			switch (sp.indexType)
			{
			case 0:
				if (_playerManager.IsAlive)
				{
					Fashe();
				}
				break;
			case 1:
				if (sp.cp.IsAlive)
				{
					Fashe();
				}
				break;
			case 2:
				if (sp.em.IsAlive)
				{
					Fashe();
				}
				break;
			}
			timeA = 0f;
		}
		if (sp.NoTime == 1)
		{
			timeB += Time.deltaTime;
			if (timeB >= sp.BuffTime)
			{
				timeB = 0f;
				Stop();
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
		if (sp.SpecialType != 10)
		{
			return;
		}
		timeO += Time.deltaTime;
		if (timeO >= 0.15f)
		{
			if (sp.SpecialType == 10)
			{
				_playerManager.RefreshORB(sp, 0);
			}
			timeO = 0f;
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
		if (parOne.Length != 0)
		{
			for (int i = 0; i < parOne.Length; i++)
			{
				ParticleSystem.MainModule main = parOne[i].main;
				main.startLifetime = sp.BuffTime + sp.BuffTime / 6f;
				parOne[i].Play();
			}
		}
		if (parLoop.Length != 0)
		{
			for (int j = 0; j < parLoop.Length; j++)
			{
				ParticleSystem.MainModule main2 = parLoop[j].main;
				main2.loop = true;
			}
		}
		CanAT = true;
		if (sp.Reborn > 0)
		{
			int indexType = sp.indexType;
			if (indexType != 0)
			{
				_ = indexType - 1;
				_ = 1;
			}
			else
			{
				_playerManager.HealStat.Cur += _playerManager.HealStat.Max * (float)sp.Reborn / 100f;
			}
		}
	}

	public void Fashe()
	{
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL>();
			Dicform component2 = component.GetComponent<Dicform>();
			component2.sp = sp;
			component2.SetCount(sp.ZY);
			component2.SubType = 0;
			component.size = size;
			component.Body = Body;
			component.DotMulti = DotMulti;
			component.lifeTime = 0.1f;
			component.IsGround = false;
			if (FX != null)
			{
				component.FX = FX;
			}
		}
	}

	public void Stop()
	{
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		CanAT = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
