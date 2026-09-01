using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Circle_F : MonoBehaviour
{
	private readonly List<Vector2> allPos = new List<Vector2>();

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private int FaSheCountTmp;

	private bool CanAT;

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
		sp = GetComponent<SkillOBJ_DT_SP>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		emt = GetComponent<StudioEventEmitter>();
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
		if (!CanAT)
		{
			return;
		}
		if (sp.Type_F == 0)
		{
			timeB += Time.deltaTime;
			if (timeB >= sp.FStime1)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
				if (sp.CountMulti == 2)
				{
					Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[allPos.Count - 1 - FaSheCountTmp].x, base.transform.position.y + allPos[allPos.Count - 1 - FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
					component2.sp = sp;
					component2.SetCount(sp.ZY);
					component2.SubType = 0;
					component2.Index = 0;
				}
				if (sp.CountMulti > 2)
				{
					Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[Random.Range(0, allPos.Count)].x, base.transform.position.y + allPos[Random.Range(0, allPos.Count)].y, 0f), Quaternion.identity).GetComponent<Dicform>();
					component3.sp = sp;
					component3.SetCount(sp.ZY);
					component3.SubType = 0;
					component3.Index = 0;
				}
				FaSheCountTmp++;
				timeB = 0f;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.BuffTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
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
		range = sp.Range1;
		IndexA = sp.ZD_F;
		IndexB = sp.MainEL;
		if (sp.Type_F == 0)
		{
			Count = Mathf.FloorToInt(sp.BuffTime / sp.FStime1) + 20;
		}
		else
		{
			Count = sp.Count_F;
		}
		emt.EventReference = _gameDataManager.SKPB.SoundRain[sp.Sound];
		emt.Play();
		Drop();
		if (sp.Type_F == 1)
		{
			for (int i = 0; i < sp.Count_F; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[FaSheCountTmp].x, base.transform.position.y + allPos[FaSheCountTmp].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
				FaSheCountTmp++;
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
