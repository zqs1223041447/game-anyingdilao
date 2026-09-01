using System.Collections.Generic;
using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_IceCore : MonoBehaviour
{
	public string SoundA;

	public string SoundAT;

	public GameObject[] OBJ;

	public GameObject spark;

	public float DelayTime;

	private bool CanAT;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Transform point;

	[HideInInspector]
	public Transform target;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	[HideInInspector]
	public List<Companion> cp = new List<Companion>();

	[HideInInspector]
	public List<PlayerManager> pl = new List<PlayerManager>();

	public Collider2D[] hitEM = new Collider2D[5];

	public Collider2D[] hitCP = new Collider2D[3];

	public Collider2D[] hitPL = new Collider2D[1];

	private float range;

	private int CountMulti;

	private float FStime;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		point = base.transform.Find("trans/point");
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
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
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		CanAT = false;
		target = null;
		initialized = false;
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= FStime)
			{
				timeA = 0f;
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
					Vector3 vector = target.position - point.transform.position;
					float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
					LeanPool.Spawn(spark, point.position, Quaternion.identity);
					for (int i = 0; i < CountMulti; i++)
					{
						Dicform component = LeanPool.Spawn(OBJ[dic.sp.ZD_F], point.position, Quaternion.Euler(0f, 0f, num + Random.Range((0f - dic.sp.AngleA) * 2f, dic.sp.AngleA * 2f))).GetComponent<Dicform>();
						component.sp = dic.sp;
						component.SetCount(dic.sp.ZY);
						component.SubType = 0;
						component.Index = dic.Index;
					}
					if (Random.Range(0, 101) < 60 && SoundAT != null)
					{
						RuntimeManager.PlayOneShot(SoundAT, base.transform.position);
					}
				}
			}
		}
		timeD += Time.deltaTime;
		if (timeD >= 0.23f)
		{
			if (dic.sp.ZY)
			{
				int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("FootCOLem"));
				if (num2 > 0)
				{
					for (int j = 0; j < num2; j++)
					{
						FootCOL component2 = hitEM[j].GetComponent<FootCOL>();
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
				int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitCP, LayerMask.GetMask("FootCOLcp"));
				if (num3 > 0)
				{
					for (int k = 0; k < num3; k++)
					{
						FootCOL component3 = hitCP[k].GetComponent<FootCOL>();
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
				int num4 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitPL, LayerMask.GetMask("FootCOLpl"));
				if (num4 > 0)
				{
					for (int l = 0; l < num4; l++)
					{
						FootCOL component4 = hitPL[l].GetComponent<FootCOL>();
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
			timeD = 0f;
		}
		timeB += Time.deltaTime;
		if (timeB >= dic.sp.BuffTime)
		{
			timeB = 0f;
			LeanPool.Despawn(this);
		}
		timeC += Time.deltaTime;
		if (timeC >= DelayTime)
		{
			timeC = 0f;
			CanAT = true;
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
		range = dic.sp.Range_AT;
		if (dic.Index == 0)
		{
			CountMulti = dic.sp.CountMulti * 4;
			FStime = dic.sp.FStime1 * 10f;
		}
		else
		{
			CountMulti = dic.sp.CountMulti * 4;
			FStime = dic.sp.FStime2 * 10f;
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
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
