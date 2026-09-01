using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Field : MonoBehaviour
{
	public string SoundA;

	public string SoundB;

	public float SoDelay;

	public float size;

	public bool UseDicLifeTime;

	public float LifeTime;

	public float StartTime;

	public float DelDelay;

	public float DotMulti;

	public bool Body;

	public bool HasLight;

	public ParticleSystem[] parOne;

	public ParticleSystem[] parLoop;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public LightEXP litEXP;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private float timeE;

	private bool CloseLit;

	private bool startAT;

	private bool CanAT;

	private bool SoundOK;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		if (HasLight)
		{
			litEXP = GetComponent<LightEXP>();
			if ((bool)litEXP)
			{
				litEXP.UseSkillTime = true;
			}
		}
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		timeE = 0f;
		SoundOK = false;
		CanAT = false;
		CloseLit = true;
		startAT = false;
		if (parOne.Length != 0)
		{
			for (int i = 0; i < parOne.Length; i++)
			{
				parOne[i].Stop();
			}
		}
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		if (CanAT)
		{
			if (startAT)
			{
				timeA += Time.deltaTime;
				if (timeA >= 0.5f)
				{
					Fashe();
					timeA = 0f;
				}
			}
			if (HasLight && !CloseLit)
			{
				timeC += Time.deltaTime;
				if (timeC >= LifeTime * 5f / 6f)
				{
					litEXP.LightDown = true;
					CloseLit = true;
					timeC = 0f;
				}
			}
		}
		timeB += Time.deltaTime;
		if (timeB >= LifeTime)
		{
			timeB = 0f;
			Stop();
		}
		if (!startAT)
		{
			timeD += Time.deltaTime;
			if (timeD >= StartTime)
			{
				timeD = 0f;
				startAT = true;
			}
		}
		if (SoundOK)
		{
			return;
		}
		timeE += Time.deltaTime;
		if (timeE >= SoDelay)
		{
			if (SoundB != null)
			{
				RuntimeManager.PlayOneShot(SoundB, base.transform.position);
			}
			timeE = 0f;
			SoundOK = true;
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
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
		if (UseDicLifeTime)
		{
			if (dic.sp.Field_time == 0f)
			{
				LifeTime = dic.sp.BuffTime;
			}
			else
			{
				LifeTime = dic.sp.Field_time;
			}
		}
		if (parOne.Length != 0)
		{
			for (int i = 0; i < parOne.Length; i++)
			{
				ParticleSystem.MainModule main = parOne[i].main;
				main.startLifetime = LifeTime + LifeTime / 5f;
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
		CloseLit = false;
		Fashe();
	}

	public void Fashe()
	{
		EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = dic.sp;
		component2.SetCount(dic.sp.ZY);
		component2.SubType = dic.SubType;
		component.size = size;
		component.Body = Body;
		component.DotMulti = DotMulti;
		component.lifeTime = 0.1f;
		component.IsGround = true;
		component.IsGround = false;
	}

	public void Stop()
	{
		CanAT = false;
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
