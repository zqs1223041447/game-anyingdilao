using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_sonCircle : MonoBehaviour
{
	public GameObject OBJ;

	public float FaSheTime;

	public float range;

	public float FaSheDelay;

	private List<Vector2> allPos = new List<Vector2>();

	public bool SingleFS;

	public bool UseDICtime;

	public float LifeTime;

	public float DelDelay;

	public bool UseDICcount;

	public int FaSheCount;

	[HideInInspector]
	public Dicform dic;

	private bool startFS;

	private float timeA;

	private float timeB;

	private float timeC;

	private int FaSheCountTmp;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		FaSheCountTmp = 0;
		startFS = false;
		initialized = false;
	}

	private void Update()
	{
		if (!SingleFS && startFS)
		{
			timeC += Time.deltaTime;
			if (timeC >= FaSheTime)
			{
				Dicform component = LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				timeC = 0f;
				FaSheCountTmp++;
			}
		}
		timeB += Time.deltaTime;
		if (timeB >= FaSheDelay)
		{
			timeB = 0f;
			startFS = true;
			if (SingleFS)
			{
				for (int i = 0; i < FaSheCount; i++)
				{
					Dicform component2 = LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SetCount(dic.sp.ZY);
					component2.SubType = dic.SubType;
					component2.Index = dic.Index;
					FaSheCountTmp++;
				}
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= LifeTime)
		{
			timeA = 0f;
			this.wait(DelDelay, delegate
			{
				LeanPool.Despawn(this);
			});
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
		if (UseDICcount)
		{
			switch (dic.SubType)
			{
			case 0:
				FaSheCount = dic.sp.Count_S;
				break;
			case 1:
				FaSheCount = dic.sp.Count_AB;
				break;
			case 2:
				FaSheCount = dic.sp.Count_AB;
				break;
			}
		}
		if (UseDICtime)
		{
			LifeTime = dic.sp.BuffTime;
		}
		Drop();
	}

	public void Drop()
	{
		allPos.Clear();
		int faSheCount = FaSheCount;
		Vector2[] array = new Vector2[4]
		{
			new Vector2(range, range),
			new Vector2(range, 0f - range),
			new Vector2(0f - range, range),
			new Vector2(0f - range, 0f - range)
		};
		for (int i = 0; i < faSheCount; i++)
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
