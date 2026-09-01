using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_BloodPool : MonoBehaviour
{
	public float size;

	public float DelDelay;

	public float FaSheTimeBF;

	public float DotMulti;

	public bool Body;

	public bool NoAT;

	public ParticleSystem[] parLoop;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private bool CanAT;

	private bool CanStop;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager _playerManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		CanStop = false;
		CanAT = false;
		initialized = false;
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
		CanAT = true;
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array = parLoop;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				main.loop = true;
			}
		}
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		EmptyCOL_BF component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol_BF, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL_BF>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = sp;
		component2.SetCount(sp.ZY);
		component.size = size;
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

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= FaSheTimeBF)
			{
				timeA = 0f;
				FasheBF();
			}
			if (sp.NoTime == 1)
			{
				timeB += Time.deltaTime;
				if (timeB >= sp.BuffTime)
				{
					timeB = 0f;
					Stop();
				}
			}
			if (!NoAT)
			{
				timeC += Time.deltaTime;
				if (timeC >= 0.5f)
				{
					timeC = 0f;
					FasheAT();
				}
			}
			if ((bool)mg && mg.NeedStop)
			{
				Stop();
			}
		}
		if (CanStop)
		{
			if (NoAT)
			{
				LeanPool.SafeDespawn(this);
			}
			timeD += Time.deltaTime;
			if (timeD >= DelDelay)
			{
				timeD = 0f;
				LeanPool.SafeDespawn(this);
			}
		}
	}

	public void FasheAT()
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
	}

	public void FasheBF()
	{
		EmptyCOL_BF component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol_BF, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL_BF>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = sp;
		component2.SetCount(sp.ZY);
		component.size = size;
	}

	public void Stop()
	{
		CanAT = false;
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array = parLoop;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				main.loop = false;
			}
		}
		CanStop = true;
	}
}
