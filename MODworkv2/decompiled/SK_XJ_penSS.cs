using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_XJ_penSS : MonoBehaviour
{
	public string SoundA;

	public ParticleSystem[] parLoop;

	public GameObject[] OBJ;

	public float ATtime;

	public float DelDelay;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public GameObject point;

	[HideInInspector]
	public GameObject qiu;

	private bool canAT;

	private bool FaSheOK;

	private float timeA;

	private float timeB;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		point = base.transform.Find("main/point").gameObject;
		qiu = base.transform.Find("main/point/qiu").gameObject;
		dic = GetComponent<Dicform>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		qiu.SetActive(value: true);
		timeA = 0f;
		timeB = 0f;
		canAT = false;
		FaSheOK = false;
		initialized = false;
	}

	private void Update()
	{
		if (!canAT)
		{
			return;
		}
		if (!FaSheOK)
		{
			timeB += Time.deltaTime;
			if (timeB >= ATtime)
			{
				timeB = 0f;
				switch (dic.sp.Type_S)
				{
				case 0:
				{
					GameObject obj2 = LeanPool.Spawn(OBJ[0], point.transform.position, Quaternion.identity);
					Dicform component3 = obj2.GetComponent<Dicform>();
					component3.sp = dic.sp;
					component3.SetCount(dic.sp.ZY);
					component3.SubType = dic.SubType;
					component3.Index = dic.Index;
					obj2.GetComponent<SK_Pen>().LifeTime = dic.sp.BuffTime;
					break;
				}
				case 1:
				{
					GameObject obj = LeanPool.Spawn(OBJ[1], point.transform.position, Quaternion.identity);
					Dicform component = obj.GetComponent<Dicform>();
					component.sp = dic.sp;
					component.SetCount(dic.sp.ZY);
					component.SubType = dic.SubType;
					component.Index = dic.Index;
					obj.GetComponent<SK_Pen>().LifeTime = dic.sp.BuffTime;
					Dicform component2 = LeanPool.Spawn(OBJ[1], point.transform.position, Quaternion.Euler(0f, 0f, 90f)).GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SetCount(dic.sp.ZY);
					component2.SubType = dic.SubType;
					component2.Index = dic.Index;
					obj.GetComponent<SK_Pen>().LifeTime = dic.sp.BuffTime;
					break;
				}
				}
				FaSheOK = true;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= dic.sp.BuffTime + dic.sp.BuffTime * (float)SingletonMonoScope<PlayerManager>.Instance.XJ_Time / 100f)
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
		canAT = true;
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		dic.sp.ApplyTrapDamageBonusOnce(PL);
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		timeA = 0f;
		canAT = false;
		qiu.SetActive(value: false);
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
