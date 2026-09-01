using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_XJ_F : MonoBehaviour
{
	[HideInInspector]
	private List<Vector2> allPos = new List<Vector2>();

	private int FaSheCountTmp;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeC;

	private bool CanFS;

	private float range;

	private int IndexA;

	private int IndexB;

	private GameDataManager _gameDataManager;

	private int type;

	private int Count;

	private float FStime;

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
		timeC = 0f;
		CanFS = false;
		FaSheCountTmp = 0;
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
		CanFS = true;
		range = sp.Range1;
		IndexA = sp.ORB;
		IndexB = sp.MainEL;
		FStime = sp.ORB_time;
		type = sp.TypeORB;
		Count = sp.Count_ORB;
		Drop();
		if (type == 0)
		{
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
				component.dic = Vector2.zero;
				FaSheCountTmp++;
			}
		}
		else
		{
			Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
			component2.sp = sp;
			component2.SetCount(sp.ZY);
			component2.SubType = 0;
			component2.Index = 0;
			component2.dic = Vector2.zero;
			FaSheCountTmp++;
		}
	}

	private void Update()
	{
		if (!CanFS)
		{
			return;
		}
		if (type == 1)
		{
			if (FaSheCountTmp < Count)
			{
				timeC += Time.deltaTime;
				if (timeC > FStime)
				{
					Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 0;
					component.Index = 0;
					component.dic = Vector2.zero;
					FaSheCountTmp++;
					timeC = 0f;
				}
			}
			timeA += Time.deltaTime;
			if (timeA > sp.BuffTime + (float)Count * FStime)
			{
				timeA = 0f;
				LeanPool.Despawn(this);
			}
		}
		else
		{
			timeA += Time.deltaTime;
			if (timeA > sp.BuffTime)
			{
				timeA = 0f;
				LeanPool.Despawn(this);
			}
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
