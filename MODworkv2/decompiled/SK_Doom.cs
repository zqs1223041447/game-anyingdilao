using System;
using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Doom : MonoBehaviour
{
	public string SoundA;

	public string SoundComp;

	public GameObject[] OBJ;

	public float RotateSpeed;

	public Transform core;

	[HideInInspector]
	public List<SK_Doom_Ball> QQ = new List<SK_Doom_Ball>();

	private List<Vector2> allPos = new List<Vector2>();

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float range;

	private float timeA;

	private float timeB;

	private float timeC;

	private int RDA;

	private bool CanAT;

	private int CountTmp;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager PL;

	private int Qcount;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		core = base.transform.Find("core");
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CountTmp = 0;
		CanAT = false;
		core.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		RDA = UnityEngine.Random.Range(0, 101);
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
		Qcount = sp.Count_ORB;
		switch (Qcount)
		{
		case 2:
			range = 1.2f;
			break;
		case 3:
			range = 1.2f;
			break;
		case 4:
			range = 1.3f;
			break;
		case 5:
			range = 1.3f;
			break;
		case 6:
			range = 1.4f;
			break;
		case 7:
			range = 1.4f;
			break;
		case 8:
			range = 1.5f;
			break;
		}
		Drop();
		CanAT = true;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		if (RDA < 50)
		{
			core.transform.Rotate(new Vector3(0f, 0f, 1f), RotateSpeed * Time.deltaTime);
		}
		else
		{
			core.transform.Rotate(new Vector3(0f, 0f, 1f), (0f - RotateSpeed) * Time.deltaTime);
		}
		if (PL.IsAlive)
		{
			if (CountTmp < Qcount)
			{
				timeA += Time.deltaTime;
				if (timeA > 0.2f)
				{
					timeA = 0f;
					float angle = (float)CountTmp / (float)Qcount * 360f + core.eulerAngles.z;
					Vector3 vector = AnglePOS(core.position, range, angle);
					Vector3 vector2 = Gun.MousePos - vector;
					float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
					SK_Doom_Ball component = LeanPool.Spawn(OBJ[sp.MainEL], vector, Quaternion.Euler(0f, 0f, z), core).GetComponent<SK_Doom_Ball>();
					component.father = this;
					QQ.Add(component);
					if (SoundComp != null && UnityEngine.Random.Range(0, 101) < 70)
					{
						RuntimeManager.PlayOneShot(SoundComp, base.transform.position);
					}
					CountTmp++;
				}
			}
		}
		else
		{
			timeA = 0f;
		}
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
		timeC += Time.deltaTime;
		if (timeC >= 0.15f)
		{
			if (sp.SpecialType == 10)
			{
				PL.RefreshORB(sp, 0);
			}
			timeC = 0f;
		}
	}

	public void Drop()
	{
		allPos.Clear();
		int qcount = Qcount;
		Vector2[] array = new Vector2[4]
		{
			new Vector2(range, range),
			new Vector2(range, 0f - range),
			new Vector2(0f - range, range),
			new Vector2(0f - range, 0f - range)
		};
		for (int i = 0; i < qcount; i++)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * range;
			float x = Mathf.Abs(vector.x);
			float y = Mathf.Abs(vector.y);
			int num = i % array.Length;
			Vector2 item = new Vector2(x, y) * array[num];
			allPos.Add(item);
		}
	}

	private static Vector3 AnglePOS(Vector3 center, float radius, float angle)
	{
		float f = angle * ((float)Math.PI / 180f);
		float x = center.x + radius * Mathf.Cos(f);
		float y = center.y + radius * Mathf.Sin(f);
		return new Vector3(x, y, 0f);
	}

	public void Stop()
	{
		CanAT = false;
		int num;
		for (num = 0; num < QQ.Count; num++)
		{
			QQ[num].father = null;
			GameObject clone = QQ[num].gameObject;
			QQ.Remove(QQ[num]);
			LeanPool.Despawn(clone);
			num--;
		}
		LeanPool.Despawn(this);
	}
}
