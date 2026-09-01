using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Circle_Follow : MonoBehaviour
{
	[HideInInspector]
	private List<Vector2> allPos = new List<Vector2>();

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private int FaSheCountTmp;

	private bool CanAT;

	private float range;

	private int IndexA;

	private int IndexB;

	private int Count;

	private GameDataManager _gameDataManager;

	[HideInInspector]
	public StudioEventEmitter emt;

	public SK_BuffA mg;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		mg = GetComponent<SK_BuffA>();
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
		if (!CanAT)
		{
			return;
		}
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
				component.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
			}
			if (sp.CountMulti > 2)
			{
				Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], new Vector3(base.transform.position.x + allPos[Random.Range(0, allPos.Count)].x, base.transform.position.y + allPos[Random.Range(0, allPos.Count)].y, 0f), Quaternion.identity).GetComponent<Dicform>();
				component3.sp = sp;
				component.SetCount(sp.ZY);
				component3.SubType = 0;
				component3.Index = 0;
			}
			FaSheCountTmp++;
			timeB = 0f;
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.BuffTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
		if (mg.NeedStop)
		{
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
		Count = Mathf.FloorToInt(sp.BuffTime / sp.FStime1) + 20;
		emt.EventReference = _gameDataManager.SKPB.SoundRain[sp.Sound];
		emt.Play();
		Drop();
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
