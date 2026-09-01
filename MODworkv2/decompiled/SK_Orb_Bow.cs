using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Orb_Bow : MonoBehaviour
{
	public string SoundA;

	public Skill_PB_List[] bow;

	public float RotateSpeed;

	[HideInInspector]
	public float coreSize;

	[Header("=========")]
	public Transform[] point;

	public Transform[] BowPoint;

	[HideInInspector]
	public List<GameObject> BowList = new List<GameObject>();

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public GameObject core;

	[HideInInspector]
	public GameObject round;

	private int RDA;

	private float RDB;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool ISbattle;

	private bool CanAT;

	private int Cur_Bow;

	private Gun gun;

	private float MainFL;

	private Vector3 MainVecter;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager PL;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		core = base.transform.Find("core").gameObject;
		round = base.transform.Find("round").gameObject;
		mg = GetComponent<SK_BuffA>();
		gun = SingletonMonoScope<Gun>.Instance;
		PL = SingletonMonoScope<PlayerManager>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanAT = false;
		Cur_Bow = 0;
		core.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		ISbattle = false;
		RDA = Random.Range(0, 101);
		RDB = Random.Range(0f, RotateSpeed / 2f);
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		if (RDA < 50)
		{
			core.transform.Rotate(new Vector3(0f, 0f, 1f), (RotateSpeed + RDB) * Time.deltaTime);
		}
		else
		{
			core.transform.Rotate(new Vector3(0f, 0f, 1f), (0f - RotateSpeed - RDB) * Time.deltaTime);
		}
		MainVecter = Gun.MousePos - base.transform.position;
		MainFL = Mathf.Atan2(MainVecter.y, MainVecter.x) * 57.29578f;
		round.transform.rotation = Quaternion.Euler(0f, 0f, MainFL);
		if (sp.NoTime == 1)
		{
			timeB += Time.deltaTime;
			if (timeB >= sp.BuffTime)
			{
				timeB = 0f;
				Stop();
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
		else if (mg.ORBStop)
		{
			Stop();
		}
		if (ISbattle)
		{
			for (int i = 0; i < BowList.Count; i++)
			{
				BowList[i].transform.position = BowPoint[i].position;
				BowList[i].transform.rotation = Quaternion.Euler(0f, 0f, MainFL);
			}
			if (!PL.IsBattle)
			{
				ISbattle = false;
			}
			timeA += Time.deltaTime;
			if (timeA >= sp.FStime1 - sp.FStime1 * PL.Orb_Bow_ATS / 100f)
			{
				timeA = 0f;
				if (PL.IsAlive)
				{
					Fashe();
				}
			}
		}
		else
		{
			for (int j = 0; j < BowList.Count; j++)
			{
				BowList[j].transform.position = Vector2.Lerp(BowList[j].transform.position, point[j].position, 0.3f);
				if (PL.arc.ISright)
				{
					BowList[j].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				}
				else
				{
					BowList[j].transform.rotation = Quaternion.Euler(0f, 0f, 180f);
				}
			}
			if (PL.IsBattle)
			{
				ISbattle = true;
			}
		}
		timeC += Time.deltaTime;
		if (timeC >= 0.15f)
		{
			if (sp.SpecialType == 10)
			{
				PL.RefreshORB(sp, 4);
			}
			timeC = 0f;
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
		coreSize = sp.Size;
		core.transform.localScale = new Vector3(coreSize, coreSize, coreSize);
		round.transform.localScale = new Vector3(coreSize, coreSize, coreSize);
		for (int i = 0; i < sp.Count_ORB; i++)
		{
			GameObject item = LeanPool.Spawn(bow[sp.MainEL].PB[sp.ORB], base.transform.position, Quaternion.identity, base.transform);
			BowList.Add(item);
			PL.PrefabCount(90, add: true);
		}
		CanAT = true;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Fashe()
	{
		switch (sp.Type_F)
		{
		case 0:
		{
			for (int i = 0; i < BowList.Count; i++)
			{
				Dicform component2 = ((sp.TypeORB != 0) ? LeanPool.Spawn(_gameDataManager.SKPB.Dic[sp.ZD_F].OBJ[sp.MainEL], BowPoint[i].transform.position, Quaternion.identity) : LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], BowPoint[i].transform.position, Quaternion.Euler(0f, 0f, MainFL))).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
				component2.dic = MainVecter;
			}
			break;
		}
		case 1:
		{
			Dicform component = ((sp.TypeORB != 0) ? LeanPool.Spawn(_gameDataManager.SKPB.Dic[sp.ZD_F].OBJ[sp.MainEL], BowPoint[Cur_Bow].transform.position, Quaternion.identity) : LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], BowPoint[Cur_Bow].transform.position, Quaternion.Euler(0f, 0f, MainFL))).GetComponent<Dicform>();
			component.sp = sp;
			component.SetCount(sp.ZY);
			component.SubType = 0;
			component.Index = 0;
			component.dic = MainVecter;
			if (Cur_Bow < BowList.Count - 1)
			{
				Cur_Bow++;
			}
			else
			{
				Cur_Bow = 0;
			}
			break;
		}
		}
	}

	public void Stop()
	{
		CanAT = false;
		int num;
		for (num = 0; num < BowList.Count; num++)
		{
			PL.PrefabCount(90, add: false);
			GameObject gameObject = BowList[num];
			LeanPool.Despawn(gameObject);
			BowList.Remove(gameObject);
			num--;
		}
		LeanPool.Despawn(this);
	}
}
