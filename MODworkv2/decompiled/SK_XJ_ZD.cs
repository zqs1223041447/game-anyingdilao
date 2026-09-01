using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_XJ_ZD : MonoBehaviour
{
	public string SoundA;

	public string SoundAT;

	public ParticleSystem[] parLoop;

	public GameObject[] OBJ;

	public float ATtime;

	public float DelDelay;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public GameObject point;

	[HideInInspector]
	public GameObject qiu;

	[HideInInspector]
	public Transform target;

	private bool canAT;

	private float timeA;

	private float timeB;

	private float timeC;

	private int FScount;

	private int IndexA;

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	[HideInInspector]
	public List<Companion> cp = new List<Companion>();

	[HideInInspector]
	public List<PlayerManager> pl = new List<PlayerManager>();

	public Collider2D[] hitEM = new Collider2D[6];

	public Collider2D[] hitCP = new Collider2D[3];

	public Collider2D[] hitPL = new Collider2D[1];

	private float range;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		point = base.transform.Find("main/point").gameObject;
		qiu = base.transform.Find("main/point/qiu").gameObject;
		dic = GetComponent<Dicform>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		qiu.SetActive(value: true);
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		em.Clear();
		cp.Clear();
		pl.Clear();
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		for (int j = 0; j < hitCP.Length; j++)
		{
			hitCP[j] = null;
		}
		hitPL[0] = null;
		canAT = false;
		target = null;
		initialized = false;
	}

	private void Update()
	{
		if (!canAT)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= ATtime)
		{
			timeB = 0f;
			if (dic.sp.ZY)
			{
				if (em.Count > 0)
				{
					target = em[0].yao.transform;
				}
				else
				{
					target = null;
				}
			}
			else if (cp.Count > 0)
			{
				if (pl.Count > 0)
				{
					if (Vector2.Distance(base.transform.position, pl[0].yao.transform.position) < Vector2.Distance(base.transform.position, cp[0].yao.transform.position))
					{
						target = pl[0].yao.transform;
					}
					else
					{
						target = cp[0].yao.transform;
					}
				}
				else
				{
					target = cp[0].yao.transform;
				}
			}
			else if (pl.Count > 0)
			{
				target = pl[0].yao.transform;
			}
			else
			{
				target = null;
			}
			if (target != null)
			{
				AT();
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= 0.15f)
		{
			timeA = 0f;
			if (dic.sp.ZY)
			{
				int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("BodyCOLem"));
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						BodyCOL component = hitEM[i].GetComponent<BodyCOL>();
						if ((bool)component)
						{
							if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS)
							{
								em.Add(component.peo.em);
							}
							hitEM[i] = null;
						}
					}
				}
			}
			else
			{
				int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitCP, LayerMask.GetMask("BodyCOLcp"));
				if (num2 > 0)
				{
					for (int j = 0; j < num2; j++)
					{
						BodyCOL component2 = hitCP[j].GetComponent<BodyCOL>();
						if ((bool)component2)
						{
							if (component2.peo.CharacterType == 1 && component2.peo.cp.IsAlive && !cp.Contains(component2.peo.cp))
							{
								cp.Add(component2.peo.cp);
							}
							hitCP[j] = null;
						}
					}
				}
				int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitPL, LayerMask.GetMask("BodyCOLpl"));
				if (num3 > 0)
				{
					for (int k = 0; k < num3; k++)
					{
						BodyCOL component3 = hitPL[k].GetComponent<BodyCOL>();
						if ((bool)component3)
						{
							if (component3.peo.CharacterType == 0 && component3.peo.pl.IsAlive && !pl.Contains(component3.peo.pl))
							{
								pl.Add(component3.peo.pl);
							}
							hitPL[k] = null;
						}
					}
				}
			}
			Refresh();
		}
		timeC += Time.deltaTime;
		if (timeC >= dic.sp.BuffTime + dic.sp.BuffTime * (float)SingletonMonoScope<PlayerManager>.Instance.XJ_Time / 100f)
		{
			timeC = 0f;
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
		canAT = true;
		range = dic.sp.Range_AT;
		if (dic.Index == 0)
		{
			IndexA = dic.sp.ZD_F;
			FScount = dic.sp.Count_F;
		}
		else
		{
			switch (dic.SubType)
			{
			case 0:
				IndexA = dic.sp.ZD_S;
				FScount = dic.sp.Count_S;
				break;
			case 1:
			case 2:
				IndexA = dic.sp.ZD_AB;
				FScount = dic.sp.Count_AB;
				break;
			}
		}
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		dic.sp.ApplyTrapDamageBonusOnce(PL);
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void AT()
	{
		Vector3 vector = target.position - point.transform.position;
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		if (dic.sp.Count_F % 2 == 1)
		{
			Dicform component = LeanPool.Spawn(OBJ[IndexA], point.transform.position, Quaternion.Euler(0f, 0f, num)).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index;
			if (FScount > 1)
			{
				for (int i = 0; i < FScount / 2; i++)
				{
					GameObject obj = LeanPool.Spawn(OBJ[IndexA], point.transform.position, Quaternion.Euler(0f, 0f, num + dic.sp.AngleA * (float)(i + 1)));
					GameObject gameObject = LeanPool.Spawn(OBJ[IndexA], point.transform.position, Quaternion.Euler(0f, 0f, num - dic.sp.AngleA * (float)(i + 1)));
					Dicform component2 = obj.GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SetCount(dic.sp.ZY);
					component2.SubType = dic.SubType;
					component2.Index = dic.Index;
					Dicform component3 = gameObject.GetComponent<Dicform>();
					component3.sp = dic.sp;
					component3.SetCount(dic.sp.ZY);
					component3.SubType = dic.SubType;
					component3.Index = dic.Index;
				}
			}
		}
		else
		{
			for (int j = 0; j < FScount / 2; j++)
			{
				GameObject gameObject2 = LeanPool.Spawn(OBJ[IndexA], point.transform.position, Quaternion.Euler(0f, 0f, num + dic.sp.AngleA * (float)(j + 1) - dic.sp.AngleA / 2f));
				GameObject obj2 = LeanPool.Spawn(OBJ[IndexA], point.transform.position, Quaternion.Euler(0f, 0f, num - dic.sp.AngleA * (float)(j + 1) + dic.sp.AngleA / 2f));
				Dicform component4 = gameObject2.GetComponent<Dicform>();
				component4.sp = dic.sp;
				component4.SetCount(dic.sp.ZY);
				component4.SubType = dic.SubType;
				component4.Index = dic.Index;
				Dicform component5 = obj2.GetComponent<Dicform>();
				component5.sp = dic.sp;
				component5.SetCount(dic.sp.ZY);
				component5.SubType = dic.SubType;
				component5.Index = dic.Index;
			}
		}
		if (Random.Range(0, 101) < 30 && SoundAT != null)
		{
			RuntimeManager.PlayOneShot(SoundAT, base.transform.position);
		}
	}

	public void Stop()
	{
		canAT = false;
		qiu.SetActive(value: false);
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void Refresh()
	{
		if (dic.sp.ZY)
		{
			for (int i = 0; i < em.Count; i++)
			{
				if (!em[i].IsAlive || em[i].IsJump || em[i].IsYS || Vector3.Distance(em[i].transform.position, base.transform.position) > range)
				{
					em.Remove(em[i]);
					i--;
				}
			}
			em.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
			return;
		}
		for (int j = 0; j < cp.Count; j++)
		{
			if (!cp[j].IsAlive || Vector3.Distance(cp[j].transform.position, base.transform.position) > range)
			{
				cp.Remove(cp[j]);
				j--;
			}
		}
		cp.Sort((Companion t1, Companion t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
		if (pl.Count > 0 && (!pl[0].IsAlive || Vector3.Distance(pl[0].transform.position, base.transform.position) > range))
		{
			pl.Remove(pl[0]);
		}
	}
}
