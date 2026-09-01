using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_JG : MonoBehaviour
{
	public GameObject FX;

	public ParticleSystem[] parLoop;

	public float speed;

	public float DotMulti;

	[Header("=========")]
	public GameObject Sub;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public CircleCollider2D MainCOL;

	private int ATCountTmp;

	private float speedTMP;

	private bool CanMV;

	private float timeA;

	private float timeB;

	private float timeC;

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	[HideInInspector]
	public List<Companion> cp = new List<Companion>();

	[HideInInspector]
	public List<PlayerManager> pl = new List<PlayerManager>();

	[HideInInspector]
	public List<Enemy> NOem = new List<Enemy>();

	[HideInInspector]
	public List<Companion> NOcp = new List<Companion>();

	public Collider2D[] hitEM = new Collider2D[10];

	public Collider2D[] hitCP = new Collider2D[10];

	public Collider2D[] hitPL = new Collider2D[1];

	[HideInInspector]
	public Enemy targetEM;

	[HideInInspector]
	public Companion targetCP;

	[HideInInspector]
	public PlayerManager targetPL;

	private bool CanACT;

	public float range;

	private int RD;

	private float tmp;

	private int ATCount;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<CircleCollider2D>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		em.Clear();
		cp.Clear();
		pl.Clear();
		NOem.Clear();
		NOcp.Clear();
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		for (int j = 0; j < hitCP.Length; j++)
		{
			hitCP[j] = null;
		}
		hitPL[0] = null;
		targetEM = null;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanMV = false;
		CanACT = false;
		ATCountTmp = 0;
		speedTMP = speed;
		MainCOL.enabled = false;
		initialized = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CanMV && collision.CompareTag("BodyCOL"))
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if (dic.sp.ZY)
			{
				if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS && !NOem.Contains(component.peo.em))
				{
					component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
					if (dic.Index == 0 && CanACT)
					{
						SingletonMonoScope<ACTbar>.Instance.CreatACT_Hit(dic.sp.skillName, component.peo.em, base.transform.right);
						CanACT = false;
					}
					RD = Random.Range(0, 101);
					if (RD < 20)
					{
						if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && Sub != null)
						{
							Dicform component2 = LeanPool.Spawn(Sub, component.peo.em.yao.transform.position, Quaternion.identity, component.peo.em.yao.transform).GetComponent<Dicform>();
							component2.sp = dic.sp;
							component2.SetCount(dic.sp.ZY);
							component2.SubType = 1;
							component2.Index = dic.Index + 1;
						}
						if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && Sub != null)
						{
							Dicform component3 = LeanPool.Spawn(Sub, component.peo.em.yao.transform.position, Quaternion.identity, component.peo.em.yao.transform).GetComponent<Dicform>();
							component3.sp = dic.sp;
							component3.SetCount(dic.sp.ZY);
							component3.SubType = 2;
							component3.Index = dic.Index + 1;
						}
					}
					LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.em.yao.transform);
					ATCountTmp++;
					TryAddATTargetFromSkill();
					if (component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
					{
						if (em.Count > 1)
						{
							NOem.Add(component.peo.em);
							em.Remove(component.peo.em);
							if (em.Count > 1)
							{
								targetEM = em[Random.Range(0, em.Count)];
							}
							else
							{
								targetEM = em[0];
							}
							base.transform.right = targetEM.yao.transform.position - base.transform.position;
						}
						else if (NOem.Count > 0)
						{
							Enemy item = NOem[0];
							NOem.Remove(item);
							em.Add(item);
							NOem.Add(component.peo.em);
							em.Remove(component.peo.em);
							targetEM = em[0];
							base.transform.right = targetEM.yao.transform.position - base.transform.position;
						}
						else
						{
							NOem.Add(component.peo.em);
							em.Remove(component.peo.em);
						}
					}
					else if (em.Count > 1)
					{
						NOem.Add(component.peo.em);
						em.Remove(component.peo.em);
						if (em.Count > 1)
						{
							targetEM = em[Random.Range(0, em.Count)];
						}
						else
						{
							targetEM = em[0];
						}
						base.transform.right = targetEM.yao.transform.position - base.transform.position;
					}
					else if (NOem.Count > 0)
					{
						Enemy item2 = NOem[0];
						NOem.Remove(item2);
						em.Add(item2);
						em.Remove(component.peo.em);
						targetEM = em[0];
						base.transform.right = targetEM.yao.transform.position - base.transform.position;
					}
					else
					{
						em.Remove(component.peo.em);
					}
				}
			}
			else
			{
				if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive && !NOcp.Contains(component.peo.cp))
				{
					component.peo.CP_Set(dic.sp, dic.SubType);
					RD = Random.Range(0, 101);
					if (RD < 20)
					{
						if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && Sub != null)
						{
							Dicform component4 = LeanPool.Spawn(Sub, component.peo.cp.yao.transform.position, Quaternion.identity, component.peo.cp.yao.transform).GetComponent<Dicform>();
							component4.sp = dic.sp;
							component4.SetCount(dic.sp.ZY);
							component4.SubType = 1;
							component4.Index = dic.Index + 1;
						}
						if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && Sub != null)
						{
							Dicform component5 = LeanPool.Spawn(Sub, component.peo.cp.yao.transform.position, Quaternion.identity, component.peo.cp.yao.transform).GetComponent<Dicform>();
							component5.sp = dic.sp;
							component5.SetCount(dic.sp.ZY);
							component5.SubType = 2;
							component5.Index = dic.Index + 1;
						}
					}
					LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.cp.yao.transform);
					ATCountTmp++;
					if (component.peo.cp.IsAlive)
					{
						if (pl.Count > 0 && cp.Count > 1)
						{
							if (Random.Range(0, 100) < 80)
							{
								NOcp.Add(component.peo.cp);
								cp.Remove(component.peo.cp);
								if (cp.Count > 1)
								{
									targetCP = cp[Random.Range(0, cp.Count)];
								}
								else
								{
									targetCP = cp[0];
								}
								base.transform.right = targetCP.yao.transform.position - base.transform.position;
							}
							else
							{
								NOcp.Add(component.peo.cp);
								cp.Remove(component.peo.cp);
								base.transform.right = targetPL.yao.transform.position - base.transform.position;
							}
						}
						else if (pl.Count > 0 && cp.Count == 1)
						{
							NOcp.Add(component.peo.cp);
							cp.Remove(component.peo.cp);
							base.transform.right = targetPL.yao.transform.position - base.transform.position;
						}
						else if (pl.Count == 0 && cp.Count > 1)
						{
							NOcp.Add(component.peo.cp);
							cp.Remove(component.peo.cp);
							if (cp.Count > 1)
							{
								targetCP = cp[Random.Range(0, cp.Count)];
							}
							else
							{
								targetCP = cp[0];
							}
							base.transform.right = targetCP.yao.transform.position - base.transform.position;
						}
						else if (pl.Count == 0 && cp.Count == 1)
						{
							NOcp.Add(component.peo.cp);
							cp.Remove(component.peo.cp);
							targetCP = cp[0];
							base.transform.right = targetCP.yao.transform.position - base.transform.position;
						}
						else if (NOcp.Count > 0)
						{
							Companion item3 = NOcp[0];
							NOcp.Remove(item3);
							cp.Add(item3);
							NOcp.Add(component.peo.cp);
							cp.Remove(component.peo.cp);
							targetCP = cp[0];
							base.transform.right = targetCP.yao.transform.position - base.transform.position;
						}
						else
						{
							NOcp.Add(component.peo.cp);
							cp.Remove(component.peo.cp);
						}
					}
					else
					{
						cp.Remove(component.peo.cp);
						if (pl.Count > 0 && cp.Count > 1)
						{
							if (Random.Range(0, 100) < 80)
							{
								if (cp.Count > 1)
								{
									targetCP = cp[Random.Range(0, cp.Count)];
								}
								else
								{
									targetCP = cp[0];
								}
								base.transform.right = targetCP.yao.transform.position - base.transform.position;
							}
							else
							{
								base.transform.right = targetPL.yao.transform.position - base.transform.position;
							}
						}
						else if (pl.Count > 0 && cp.Count == 1)
						{
							base.transform.right = targetPL.yao.transform.position - base.transform.position;
						}
						else if (pl.Count == 0 && cp.Count > 1)
						{
							if (cp.Count > 1)
							{
								targetCP = cp[Random.Range(0, cp.Count)];
							}
							else
							{
								targetCP = cp[0];
							}
							base.transform.right = targetCP.yao.transform.position - base.transform.position;
						}
						else if (pl.Count == 0 && cp.Count == 1)
						{
							targetCP = cp[0];
							base.transform.right = targetCP.yao.transform.position - base.transform.position;
						}
						else if (NOcp.Count > 0)
						{
							Companion item4 = NOcp[0];
							NOcp.Remove(item4);
							cp.Add(item4);
							targetCP = cp[0];
							base.transform.right = targetCP.yao.transform.position - base.transform.position;
						}
					}
				}
				if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
				{
					component.peo.PL_Set(dic.sp, dic.SubType);
					RD = Random.Range(0, 101);
					if (RD < 20)
					{
						if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && Sub != null)
						{
							Dicform component6 = LeanPool.Spawn(Sub, component.peo.pl.yao.transform.position, Quaternion.identity, component.peo.pl.yao.transform).GetComponent<Dicform>();
							component6.sp = dic.sp;
							component6.SetCount(dic.sp.ZY);
							component6.SubType = 1;
							component6.Index = dic.Index + 1;
						}
						if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && Sub != null)
						{
							Dicform component7 = LeanPool.Spawn(Sub, component.peo.pl.yao.transform.position, Quaternion.identity, component.peo.pl.yao.transform).GetComponent<Dicform>();
							component7.sp = dic.sp;
							component7.SetCount(dic.sp.ZY);
							component7.SubType = 2;
							component7.Index = dic.Index + 1;
						}
					}
					LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.pl.yao.transform);
					ATCountTmp++;
					if (cp.Count > 0)
					{
						if (cp.Count > 1)
						{
							targetCP = cp[Random.Range(0, cp.Count)];
						}
						else
						{
							targetCP = cp[0];
						}
						base.transform.right = targetCP.yao.transform.position - base.transform.position;
					}
				}
			}
		}
		if (collision.CompareTag("DoomBall"))
		{
			SK_Doom_Ball component8 = collision.GetComponent<SK_Doom_Ball>();
			if (dic.sp.ZY)
			{
				component8.SetHit(dic, base.transform.right);
			}
			else if (component8.father.sp.TypeDIC_F > 0 && Random.Range(0, 101) < component8.father.sp.TypeDIC_F)
			{
				Stop();
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
			Stop();
		}
	}

	private void Update()
	{
		if (CanMV)
		{
			timeA += Time.deltaTime;
			if (timeA >= tmp / 5f)
			{
				Stop();
			}
			timeB += Time.deltaTime;
			if (timeB >= 0.1f)
			{
				Refresh();
				timeB = 0f;
			}
			base.transform.Translate(Vector2.right * (speedTMP * Time.deltaTime));
			if (ATCountTmp == ATCount)
			{
				Stop();
			}
			timeC += Time.deltaTime;
			if (timeC >= 0.1f)
			{
				CanACT = true;
				timeC = 0f;
			}
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
		CanMV = true;
		CanACT = true;
		Refresh();
		MainCOL.enabled = true;
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		tmp = dic.sp.Count_ATtarget;
		ATCount = dic.sp.Count_ATtarget;
	}

	private void TryAddATTargetFromSkill()
	{
		if (!(dic == null) && !(dic.sp == null) && dic.sp.ATtarUP > 0 && Random.value < (float)dic.sp.ATtarUP * 0.01f)
		{
			ATCount++;
		}
	}

	public void Stop()
	{
		timeA = 0f;
		speedTMP = 0f;
		CanMV = false;
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		this.wait(0.5f, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void Refresh()
	{
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
						if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !NOem.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS)
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
						if (component2.peo.CharacterType == 1 && component2.peo.cp.IsAlive && !cp.Contains(component2.peo.cp) && !NOcp.Contains(component2.peo.cp))
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
		if (dic.sp.ZY)
		{
			for (int l = 0; l < em.Count; l++)
			{
				if (!em[l].IsAlive || em[l].IsYS || em[l].IsJump || Vector3.Distance(em[l].transform.position, base.transform.position) > range + 1f)
				{
					em.Remove(em[l]);
					l--;
				}
			}
			em.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
			for (int m = 0; m < NOem.Count; m++)
			{
				if (!NOem[m].IsAlive || NOem[m].IsYS || NOem[m].IsJump || Vector3.Distance(NOem[m].transform.position, base.transform.position) > range + 1f)
				{
					NOem.Remove(NOem[m]);
					m--;
				}
			}
			NOem.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
			return;
		}
		for (int n = 0; n < cp.Count; n++)
		{
			if (!cp[n].IsAlive || Vector3.Distance(cp[n].transform.position, base.transform.position) > range + 1f)
			{
				cp.Remove(cp[n]);
				n--;
			}
		}
		cp.Sort((Companion t1, Companion t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
		for (int num4 = 0; num4 < NOcp.Count; num4++)
		{
			if (!NOcp[num4].IsAlive || Vector3.Distance(NOcp[num4].transform.position, base.transform.position) > range + 1f)
			{
				NOcp.Remove(NOcp[num4]);
				num4--;
			}
		}
		NOcp.Sort((Companion t1, Companion t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
		if (pl.Count > 0 && (!pl[0].IsAlive || Vector3.Distance(pl[0].transform.position, base.transform.position) > range + 1f))
		{
			pl.Remove(pl[0]);
		}
	}
}
