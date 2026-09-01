using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_XJ_pen : MonoBehaviour
{
	public string SoundA;

	public string SoundPen;

	public GameObject[] OBJ;

	public float ATtime;

	public float DelDelay;

	[Header("=========")]
	public GameObject SubA;

	[HideInInspector]
	public Transform target;

	[HideInInspector]
	public GameObject point;

	[HideInInspector]
	public GameObject qiu;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public List<SK_Pen> OBJpen = new List<SK_Pen>();

	private bool canAT;

	private float timeA;

	private float timeB;

	private float timeC;

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
		if (dic.sp.ZY)
		{
			if (em.Count > 0)
			{
				target = em[0].peo.em.yao.transform;
			}
			else
			{
				OBJpen.Clear();
				target = null;
			}
		}
		else if (cp.Count > 0)
		{
			if (pl.Count > 0)
			{
				if (Vector2.Distance(base.transform.position, pl[0].yao.transform.position) < Vector2.Distance(base.transform.position, pl[0].yao.transform.position))
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
			OBJpen.Clear();
			target = null;
		}
		if (target != null)
		{
			Vector3 vector = target.position - point.transform.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (OBJpen.Count > 0)
			{
				for (int i = 0; i < OBJpen.Count; i++)
				{
					OBJpen[i].transform.rotation = Quaternion.Euler(0f, 0f, z);
				}
			}
			timeB += Time.deltaTime;
			if (timeB >= ATtime)
			{
				timeB = 0f;
				GameObject gameObject = LeanPool.Spawn(OBJ[dic.sp.ZD_S], point.transform.position, Quaternion.Euler(0f, 0f, z), point.transform);
				Dicform component = gameObject.GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index + 1;
				SK_Pen sk = gameObject.GetComponent<SK_Pen>();
				sk.ATtime = ATtime;
				sk.InitPen(ATtime);
				OBJpen.Add(sk);
				this.wait(ATtime, delegate
				{
					OBJpen.Remove(sk);
				});
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= 0.1f)
		{
			timeA = 0f;
			if (dic.sp.ZY)
			{
				int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("BodyCOLem"));
				if (num > 0)
				{
					for (int j = 0; j < num; j++)
					{
						BodyCOL component2 = hitEM[j].GetComponent<BodyCOL>();
						if ((bool)component2)
						{
							if (component2.peo.CharacterType == 2 && component2.peo.em.IsAlive && !em.Contains(component2.peo.em) && !component2.peo.em.IsJump && !component2.peo.em.IsYS)
							{
								em.Add(component2.peo.em);
							}
							hitEM[j] = null;
						}
					}
				}
			}
			else
			{
				int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitCP, LayerMask.GetMask("BodyCOLcp"));
				if (num2 > 0)
				{
					for (int k = 0; k < num2; k++)
					{
						BodyCOL component3 = hitCP[k].GetComponent<BodyCOL>();
						if ((bool)component3)
						{
							if (component3.peo.CharacterType == 1 && component3.peo.cp.IsAlive && !cp.Contains(component3.peo.cp))
							{
								cp.Add(component3.peo.cp);
							}
							hitCP[k] = null;
						}
					}
				}
				int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitPL, LayerMask.GetMask("BodyCOLpl"));
				if (num3 > 0)
				{
					for (int l = 0; l < num3; l++)
					{
						BodyCOL component4 = hitPL[l].GetComponent<BodyCOL>();
						if ((bool)component4)
						{
							if (component4.peo.CharacterType == 0 && component4.peo.pl.IsAlive && !pl.Contains(component4.peo.pl))
							{
								pl.Add(component4.peo.pl);
							}
							hitPL[l] = null;
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
		if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && SubA != null)
		{
			Dicform component = LeanPool.Spawn(SubA, new Vector3(base.transform.position.x, base.transform.position.y - 0.03f, base.transform.position.z), Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = 1;
			component.Index = dic.Index + 1;
		}
		dic.sp.ApplyTrapDamageBonusOnce(PL);
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		target = null;
		canAT = false;
		this.wait(DelDelay, del);
	}

	public void del()
	{
		qiu.SetActive(value: false);
		if (OBJpen.Count > 0)
		{
			for (int i = 0; i < OBJpen.Count; i++)
			{
				LeanPool.Despawn(OBJpen[i]);
			}
		}
		OBJpen.Clear();
		LeanPool.Despawn(this);
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
