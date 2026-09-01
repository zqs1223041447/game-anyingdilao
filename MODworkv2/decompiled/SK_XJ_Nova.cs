using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_XJ_Nova : MonoBehaviour
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

	private float timeA;

	private float timeB;

	private int EXPcountTmp;

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
		EXPcountTmp = 0;
		initialized = false;
	}

	private void Update()
	{
		if (!canAT)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= ATtime)
		{
			timeB = 0f;
			switch (dic.sp.Type_S)
			{
			case 0:
				if (EXPcountTmp < dic.sp.Count_ATtarget)
				{
					Dicform component2 = LeanPool.Spawn(OBJ[dic.sp.ZD_S], point.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SetCount(dic.sp.ZY);
					component2.SubType = dic.SubType;
					component2.Index = dic.Index;
					EXPcountTmp++;
				}
				else
				{
					Stop();
				}
				break;
			case 1:
				if (EXPcountTmp < dic.sp.Count_ATtarget)
				{
					Dicform component = LeanPool.Spawn(OBJ[dic.sp.ZD_S], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = dic.sp;
					component.SetCount(dic.sp.ZY);
					component.SubType = dic.SubType;
					component.Index = dic.Index;
					EXPcountTmp++;
				}
				else
				{
					Stop();
				}
				break;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= dic.sp.BuffTime)
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
		dic.sp.ApplyTrapDamageBonusOnce(PL);
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
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
