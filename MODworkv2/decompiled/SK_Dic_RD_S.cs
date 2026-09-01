using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Dic_RD_S : MonoBehaviour
{
	[HideInInspector]
	public Dicform dic;

	private float LifeTime;

	private float timeA;

	private float timeB;

	private bool CanAT;

	private int FaSheCount;

	private List<Vector2> allPos = new List<Vector2>();

	private float range;

	private int IndexA;

	private int IndexB;

	private GameDataManager _gameDataManager;

	private int Count;

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
		CanAT = false;
		timeA = 0f;
		timeB = 0f;
		FaSheCount = 0;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
		if (dic.sp.TypeDIC_S == 1)
		{
			timeB += Time.deltaTime;
			if (timeB > dic.sp.FStime2)
			{
				timeB = 0f;
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCount].x, base.transform.position.y + allPos[FaSheCount].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				FaSheCount++;
			}
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
		IndexA = dic.sp.Dic_S;
		IndexB = dic.sp.MainEL;
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
		Drop();
		switch (dic.sp.TypeDIC_S)
		{
		case 0:
		{
			LifeTime = dic.sp.BuffTime;
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCount].x, base.transform.position.y + allPos[FaSheCount].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			}
			break;
		}
		case 1:
			LifeTime = (float)Count * dic.sp.FStime2 + 0.08f;
			break;
		}
		FaSheCount = 0;
		if (_gameDataManager.SKPB.Dic[IndexA].ST[IndexB] != null)
		{
			RuntimeManager.PlayOneShot(_gameDataManager.SKPB.Dic[IndexA].ST[IndexB], base.transform.position);
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
