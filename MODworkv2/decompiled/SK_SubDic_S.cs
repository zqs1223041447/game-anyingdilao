using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_SubDic_S : MonoBehaviour
{
	[HideInInspector]
	public Dicform dic;

	private int FScountTMP;

	private bool CanAT;

	private float timeA;

	private float timeB;

	private int IndexA;

	private int IndexB;

	private int Count;

	private float FStime;

	private int type;

	private float spMin;

	private float spMax;

	private float spRD;

	private float distans;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanAT = false;
		FScountTMP = 0;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		if (FScountTMP < Count)
		{
			timeB += Time.deltaTime;
			if (timeB > FStime)
			{
				if (type == 2)
				{
					Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.SubDic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = dic.sp;
					component.SetCount(dic.sp.ZY);
					component.SubType = dic.SubType;
					component.Index = dic.Index;
					component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
					component.speed = Random.Range(spMin, spMax);
				}
				timeB = 0f;
				FScountTMP++;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= FStime * (float)Count + 0.2f)
		{
			timeA = 0f;
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
		CanAT = true;
		spRD = dic.sp.Speed4;
		FStime = dic.sp.FStime2;
		type = dic.sp.TypeDIC_S;
		IndexA = dic.sp.Dic_S;
		IndexB = dic.sp.MainEL;
		spMin = dic.sp.Speed3;
		spMax = dic.sp.Speed4;
		switch (dic.SubType)
		{
		case 0:
			Count = dic.sp.Count_S;
			break;
		case 1:
			Count = dic.sp.Count_AB;
			break;
		case 2:
			Count = dic.sp.Count_AB;
			break;
		}
		switch (type)
		{
		case 0:
		{
			for (int j = 0; j < Count; j++)
			{
				Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.SubDic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = dic.SubType;
				component2.Index = dic.Index;
				component2.dic = dic.sp.TargetPos - base.transform.position;
				distans = Dis();
				component2.dic = new Vector2(component2.dic.x + Random.Range(spRD, 0f - spRD), component2.dic.y + Random.Range(spRD, 0f - spRD));
				if (distans > 5f)
				{
					distans = 5f;
				}
				component2.speed = (distans + Random.Range(spRD, 0f - spRD)) * 2.3f;
			}
			break;
		}
		case 1:
		{
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.SubDic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				component.speed = Random.Range(spMin, spMax);
			}
			break;
		}
		}
	}

	public void Stop()
	{
		CanAT = false;
		LeanPool.Despawn(this);
	}

	public float Dis()
	{
		float num = Vector2.Distance(dic.sp.TargetPos, base.transform.position);
		if (num < dic.sp.Distance)
		{
			return num;
		}
		return dic.sp.Distance;
	}
}
