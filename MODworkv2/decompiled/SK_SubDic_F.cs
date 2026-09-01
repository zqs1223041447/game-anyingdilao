using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_SubDic_F : MonoBehaviour
{
	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private int FScountTMP;

	private bool CanAT;

	private float timeA;

	private float timeB;

	private int type;

	private int Count;

	private int IndexA;

	private int IndexB;

	private GameDataManager _gameDataManager;

	private float RDspeedTmp;

	private float distans;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
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
			if (timeB > sp.FStime1)
			{
				if (type == 2)
				{
					Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.SubDic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 0;
					component.Index = 0;
					component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
					component.speed = Random.Range(sp.Speed1, sp.Speed2);
				}
				timeB = 0f;
				FScountTMP++;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.FStime1 * (float)Count + 0.2f)
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
		IndexA = sp.Dic_F;
		IndexB = sp.MainEL;
		if (sp.CF_Rate > 0f)
		{
			if ((float)Random.Range(0, 101) < sp.CF_Rate)
			{
				type = sp.CF_Type;
				Count = sp.CF_Count;
				RDspeedTmp = sp.Speed4 * 2f;
			}
			else
			{
				type = sp.TypeDIC_F;
				Count = sp.Count_F;
				RDspeedTmp = sp.Speed4;
			}
		}
		else
		{
			type = sp.TypeDIC_F;
			Count = sp.Count_F;
			RDspeedTmp = sp.Speed4;
		}
		switch (type)
		{
		case 0:
		{
			for (int j = 0; j < Count; j++)
			{
				Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.SubDic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
				component2.dic = sp.TargetPos - base.transform.position;
				distans = Dis();
				component2.dic = new Vector2(component2.dic.x + Random.Range(RDspeedTmp, 0f - RDspeedTmp), component2.dic.y + Random.Range(RDspeedTmp, 0f - RDspeedTmp));
				if (distans > 5f)
				{
					distans = 5f;
				}
				component2.speed = (distans + Random.Range(RDspeedTmp, 0f - RDspeedTmp)) * 2.3f;
			}
			break;
		}
		case 1:
		{
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.SubDic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
				component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				component.speed = Random.Range(sp.Speed1, sp.Speed2);
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
		float num = Vector2.Distance(sp.TargetPos, base.transform.position);
		if (num < sp.Distance)
		{
			return num;
		}
		return sp.Distance;
	}
}
