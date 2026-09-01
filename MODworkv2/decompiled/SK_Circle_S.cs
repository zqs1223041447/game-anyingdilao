using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Circle_S : MonoBehaviour
{
	[HideInInspector]
	private List<Vector2> allPos = new List<Vector2>();

	[HideInInspector]
	public Dicform dic;

	private float timeA;

	private float timeB;

	private int FaSheCountTmp;

	private bool CanAT;

	private float FStime;

	private int type;

	private int IndexA;

	private int IndexB;

	private int Count;

	private float range;

	private GameDataManager _gameDataManager;

	[HideInInspector]
	public StudioEventEmitter emt;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		emt = GetComponent<StudioEventEmitter>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		FaSheCountTmp = 0;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT || type != 0)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= dic.sp.BuffTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
		timeB += Time.deltaTime;
		if (timeB >= FStime)
		{
			Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index;
			if (dic.sp.CountMulti == 2)
			{
				Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[allPos.Count - 1 - FaSheCountTmp].x, base.transform.position.y + allPos[allPos.Count - 1 - FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = dic.SubType;
				component2.Index = dic.Index;
			}
			if (dic.sp.CountMulti > 2)
			{
				Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[Random.Range(0, allPos.Count)].x, base.transform.position.y + allPos[Random.Range(0, allPos.Count)].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component3.sp = dic.sp;
				component3.SetCount(dic.sp.ZY);
				component3.SubType = dic.SubType;
				component3.Index = dic.Index;
			}
			FaSheCountTmp++;
			timeB = 0f;
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
		range = dic.sp.Range2;
		FStime = dic.sp.FStime2;
		switch (dic.SubType)
		{
		case 0:
			IndexA = dic.sp.ZD_S;
			IndexB = dic.sp.MainEL;
			type = dic.sp.Type_S;
			if (type == 0)
			{
				Count = Mathf.FloorToInt(dic.sp.BuffTime / dic.sp.FStime2) + 20;
			}
			else
			{
				Count = dic.sp.Count_S;
			}
			break;
		case 1:
		case 2:
			IndexA = dic.sp.ZD_AB;
			IndexB = dic.sp.MainEL;
			type = dic.sp.Type_AB;
			if (type == 0)
			{
				Count = Mathf.FloorToInt(dic.sp.BuffTime / FStime) + 20;
			}
			else
			{
				Count = dic.sp.Count_AB;
			}
			break;
		}
		emt.EventReference = _gameDataManager.SKPB.SoundRain[dic.sp.Sound];
		emt.Play();
		Drop();
		if (type == 1)
		{
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				FaSheCountTmp++;
			}
			LeanPool.Despawn(this);
		}
	}

	public void Drop()
	{
		allPos.Clear();
		int count = Count;
		Vector2[] array = new Vector2[4]
		{
			new Vector2(range, range),
			new Vector2(range, 0f - range),
			new Vector2(0f - range, range),
			new Vector2(0f - range, 0f - range)
		};
		for (int i = 0; i < count; i++)
		{
			Vector2 vector = Random.insideUnitCircle * range;
			float x = Mathf.Abs(vector.x);
			float y = Mathf.Abs(vector.y);
			int num = i % array.Length;
			Vector2 item = new Vector2(x, y) * array[num];
			allPos.Add(item);
		}
	}
}
