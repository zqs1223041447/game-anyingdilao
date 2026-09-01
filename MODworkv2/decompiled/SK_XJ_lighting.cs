using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_XJ_lighting : MonoBehaviour
{
	public string SoundA;

	public GameObject TD;

	public float range;

	public int DotMulti;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public GameObject point;

	[HideInInspector]
	public GameObject qiu;

	[HideInInspector]
	public List<SK_Thunder_LZD> dian = new List<SK_Thunder_LZD>();

	[HideInInspector]
	public List<BodyCOL> emList = new List<BodyCOL>();

	[HideInInspector]
	public List<BodyCOL> emATList = new List<BodyCOL>();

	[HideInInspector]
	public List<BodyCOL> cpList = new List<BodyCOL>();

	[HideInInspector]
	public List<BodyCOL> cpATList = new List<BodyCOL>();

	[HideInInspector]
	public SK_Thunder_LZD dianPL;

	[HideInInspector]
	public BodyCOL pl;

	private bool IsATplayer;

	[HideInInspector]
	public StudioEventEmitter Emitter;

	private float SoundFloat;

	private float timeA;

	private float timeB;

	private bool canAT;

	private float tmp;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		point = base.transform.Find("main/point").gameObject;
		qiu = base.transform.Find("main/point/qiu").gameObject;
		Emitter = GetComponent<StudioEventEmitter>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		qiu.SetActive(value: true);
		timeB = 0f;
		timeA = 0f;
		canAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (canAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= 0.1f)
			{
				SetDianEM();
				timeA = 0f;
			}
			timeB += Time.deltaTime;
			if (timeB >= dic.sp.BuffTime + dic.sp.BuffTime * (float)SingletonMonoScope<PlayerManager>.Instance.XJ_Time / 100f)
			{
				Stop();
			}
		}
		if (emATList.Count != 0)
		{
			if (emATList.Count > 7)
			{
				SoundFloat = 1f;
			}
			else
			{
				SoundFloat = tmp / 7f;
			}
		}
		else
		{
			SoundFloat = 0f;
		}
		Emitter.SetParameter("CountAT", SoundFloat);
	}

	public void SetDianEM()
	{
		if (!dic.sp.ZY)
		{
			return;
		}
		if (emList.Count > 0)
		{
			for (int i = 0; i < emList.Count; i++)
			{
				if (Vector3.Distance(base.transform.position, emList[i].transform.position) > range || !emList[i].peo.em.IsAlive || emList[i].peo.em.IsJump || emList[i].peo.em.IsYS)
				{
					DespawnDianFor(emList[i]);
					emList.Remove(emList[i]);
					i--;
				}
			}
		}
		if (emATList.Count >= dic.sp.Count_ATtarget || emList.Count <= 0)
		{
			return;
		}
		if (emList.Count > 1)
		{
			emList.Sort((BodyCOL t1, BodyCOL t2) => Vector3.Distance(t1.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.transform.position, base.transform.position)));
		}
		emATList.Add(emList[0]);
		GameObject obj = LeanPool.Spawn(TD, emList[0].peo.em.yao.transform.position, Quaternion.identity, emList[0].peo.em.yao.transform);
		Dicform component = obj.GetComponent<Dicform>();
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.SubType = dic.SubType;
		component.Index = dic.Index;
		obj.SetActive(value: false);
		SK_Thunder_LZD component2 = obj.GetComponent<SK_Thunder_LZD>();
		component2.LT = this;
		component2.range = range;
		component2.DotMulti = DotMulti;
		component2.col = emList[0];
		component2.parent = point.transform;
		component2.type = 2;
		dian.Add(component2);
		emList.Remove(emList[0]);
		Transform transform = component2.col.peo.em.yao.transform;
		Vector3 vector = transform.position - base.transform.position;
		float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		component2.transform.rotation = Quaternion.Euler(0f, 0f, z);
		component2.transform.position = new Vector2((transform.position.x + base.transform.position.x) / 2f, (transform.position.y + base.transform.position.y) / 2f);
		component2.transform.localScale = new Vector2(component2.size * Vector2.Distance(transform.position, base.transform.position), 1f);
		obj.SetActive(value: true);
	}

	public void RefreshDian(SK_Thunder_LZD lzd)
	{
		if (!lzd)
		{
			return;
		}
		if ((bool)lzd.col)
		{
			emATList.Remove(lzd.col);
		}
		for (int i = 0; i < emList.Count; i++)
		{
			if (Vector3.Distance(base.transform.position, emList[i].transform.position) > range || !emList[i].peo.em.IsAlive || emList[i].peo.em.IsJump || emList[i].peo.em.IsYS)
			{
				DespawnDianFor(emList[i]);
				emList.Remove(emList[i]);
				i--;
			}
		}
		if (emList.Count > 0)
		{
			emATList.Add(emList[0]);
			lzd.transform.SetParent(emList[0].peo.em.yao.transform);
			lzd.col = emList[0];
			emList.Remove(emList[0]);
		}
		else
		{
			DespawnDian(lzd);
		}
	}

	public void Forget(SK_Thunder_LZD lzd)
	{
		if (!lzd)
		{
			return;
		}
		dian.Remove(lzd);
		if ((bool)lzd.col)
		{
			emList.Remove(lzd.col);
			emATList.Remove(lzd.col);
			cpList.Remove(lzd.col);
			cpATList.Remove(lzd.col);
			if (pl == lzd.col)
			{
				pl = null;
				IsATplayer = false;
			}
		}
		if (dianPL == lzd)
		{
			dianPL = null;
			IsATplayer = false;
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
		canAT = true;
		tmp = emATList.Count;
		dic.sp.ApplyTrapDamageBonusOnce(PL);
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		qiu.SetActive(value: false);
		for (int num = dian.Count - 1; num >= 0; num--)
		{
			DespawnDian(dian[num]);
		}
		if ((bool)dianPL)
		{
			DespawnDian(dianPL);
		}
		dian.Clear();
		timeB = 0f;
		timeA = 0f;
		canAT = false;
		emList.Clear();
		emATList.Clear();
		cpList.Clear();
		cpATList.Clear();
		pl = null;
		this.wait(1.5f, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void DespawnDian(SK_Thunder_LZD lzd)
	{
		if ((bool)lzd)
		{
			dian.Remove(lzd);
			if (dianPL == lzd)
			{
				dianPL = null;
				IsATplayer = false;
			}
			lzd.LT = null;
			lzd.col = null;
			lzd.parent = null;
			LeanPool.Despawn(lzd);
		}
	}

	private void DespawnDianFor(BodyCOL col)
	{
		for (int num = dian.Count - 1; num >= 0; num--)
		{
			if ((bool)dian[num] && dian[num].col == col)
			{
				DespawnDian(dian[num]);
			}
		}
		if ((bool)dianPL && dianPL.col == col)
		{
			DespawnDian(dianPL);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("BodyCOL"))
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if (component.peo.CharacterType == 2 && component.peo.em.IsAlive)
			{
				emList.Add(component);
			}
			if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
			{
				pl = component;
			}
			if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
			{
				cpList.Add(component);
			}
		}
	}
}
