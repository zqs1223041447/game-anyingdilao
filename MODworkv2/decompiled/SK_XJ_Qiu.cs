using Lean.Pool;
using UnityEngine;

public class SK_XJ_Qiu : MonoBehaviour
{
	public ParticleSystem[] parLoop;

	public GameObject OBJ;

	public float ATtime;

	public float DelDelay;

	public GameObject ZD;

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

	private bool initialized;

	private void Awake()
	{
		point = base.transform.Find("main/point").gameObject;
		qiu = base.transform.Find("main/point/qiu").gameObject;
		dic = GetComponent<Dicform>();
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
			Dicform component = LeanPool.Spawn(OBJ, point.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index + 1;
			EXPcountTmp++;
			if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)ZD)
			{
				for (int i = 0; i < dic.sp.Count_AB; i++)
				{
					Dicform component2 = LeanPool.Spawn(ZD, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SetCount(dic.sp.ZY);
					component2.SubType = 1;
					component2.Index = dic.Index + 1;
				}
			}
			if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)ZD)
			{
				for (int j = 0; j < dic.sp.Count_AB; j++)
				{
					Dicform component3 = LeanPool.Spawn(ZD, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
					component3.sp = dic.sp;
					component3.SetCount(dic.sp.ZY);
					component3.SubType = 1;
					component3.Index = dic.Index + 1;
				}
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= ATtime + 0.3f)
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
