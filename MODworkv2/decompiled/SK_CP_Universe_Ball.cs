using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class SK_CP_Universe_Ball : MonoBehaviour
{
	private const int MaxTrackedEnemyCount = 5;

	public GameObject FX;

	public GameObject EXP;

	public bool hasEXP;

	public bool CoverFX;

	public float DotMulti;

	public float DelDelay;

	public float StartATtime;

	public float speed;

	public float lerpAngle;

	public bool Body;

	public float SetRangeTime;

	public float min;

	public float max;

	[HideInInspector]
	public SK_CP_Universe father;

	[HideInInspector]
	public SK_CP_Universe_Aixs ownerAxis;

	[Header("=========")]
	[HideInInspector]
	public Transform target;

	[HideInInspector]
	public Enemy TargetEM;

	[HideInInspector]
	public Dicform dic;

	public List<Enemy> em = new List<Enemy>();

	public Collider2D[] hitList = new Collider2D[4];

	private float timeA;

	private float timeB;

	private float timeG;

	private float range;

	[HideInInspector]
	public GameObject qiu;

	[HideInInspector]
	public bool StartFollow;

	private bool CanAT;

	private bool SetAngle;

	private bool CanMove;

	private bool _isStopping;

	private void Awake()
	{
		qiu = base.transform.Find("qiu").gameObject;
		dic = GetComponent<Dicform>();
	}

	private void OnEnable()
	{
		em.Clear();
		timeA = 0f;
		timeB = 0f;
		timeG = 0f;
		CanAT = false;
		StartFollow = false;
		target = null;
		SetAngle = false;
		CanMove = true;
		TargetEM = null;
		range = min;
		father = null;
		ownerAxis = null;
		_isStopping = false;
		qiu.SetActive(value: true);
		for (int i = 0; i < hitList.Length; i++)
		{
			hitList[i] = null;
		}
	}

	private void Update()
	{
		if (!target)
		{
			Stop(base.transform, self: true);
			return;
		}
		if (!CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA > StartATtime)
			{
				timeA = 0f;
				CanAT = true;
			}
		}
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= SetRangeTime)
			{
				if (range < max)
				{
					range += 1f;
				}
				timeA = 0f;
			}
			timeG += Time.deltaTime;
			if (timeG >= 0.16f)
			{
				Refresh();
				if (em.Count < 5 && Body)
				{
					int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitList, LayerMask.GetMask("BodyCOLem"));
					if (num > 0)
					{
						for (int i = 0; i < num; i++)
						{
							BodyCOL component = hitList[i].GetComponent<BodyCOL>();
							if ((bool)component)
							{
								if (em.Count < 5 && component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS)
								{
									em.Add(component.peo.em);
								}
								hitList[i] = null;
							}
						}
					}
				}
				else if (em.Count < 5)
				{
					int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitList, LayerMask.GetMask("FootCOLem"));
					if (num2 > 0)
					{
						for (int j = 0; j < num2; j++)
						{
							FootCOL component2 = hitList[j].GetComponent<FootCOL>();
							if ((bool)component2 && em.Count < 5 && component2.peo.CharacterType == 2 && component2.peo.em.IsAlive && !em.Contains(component2.peo.em) && !component2.peo.em.IsJump && !component2.peo.em.IsYS)
							{
								em.Add(component2.peo.em);
							}
							hitList[j] = null;
						}
					}
				}
				Refresh();
				timeG = 0f;
			}
			if (StartFollow)
			{
				timeB += Time.deltaTime;
				if (timeB > 5f)
				{
					timeB = 0f;
					Stop(base.transform, self: true);
				}
				if (!CanMove)
				{
					return;
				}
				if (TargetEM != null && TargetEM.IsAlive)
				{
					if (!SetAngle)
					{
						Vector3 vector = target.position - base.transform.position;
						float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
						base.transform.rotation = Quaternion.Euler(0f, 0f, z);
						SetAngle = true;
					}
					base.transform.position += base.transform.right * (speed * Time.deltaTime);
					base.transform.right = Vector3.Slerp(base.transform.right, target.position - base.transform.position, lerpAngle / Vector3.Distance(target.position, base.transform.position));
					return;
				}
				base.transform.Translate(Vector2.right * (speed * Time.deltaTime));
				if (em.Count > 0)
				{
					Refresh();
					if (em.Count > 0)
					{
						target = em[0].yao.transform;
						StartFollow = true;
						TargetEM = em[0];
					}
				}
			}
			else
			{
				base.transform.position = target.position;
				if (em.Count > 0)
				{
					target = (Body ? em[0].yao.transform : em[0].transform);
					StartFollow = true;
					TargetEM = em[0];
				}
			}
		}
		else
		{
			base.transform.position = target.position;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!CanAT || !StartFollow || !CanMove)
		{
			return;
		}
		if (collision.CompareTag("BodyCOL"))
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if (dic.sp.ZY)
			{
				if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
				{
					component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
					Stop(component.peo.em.yao.transform, self: false);
				}
			}
			else
			{
				if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
				{
					component.peo.PL_Set(dic.sp, dic.SubType);
					Stop(component.peo.pl.yao.transform, self: false);
				}
				if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
				{
					component.peo.CP_Set(dic.sp, dic.SubType);
					Stop(component.peo.cp.yao.transform, self: false);
				}
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			Stop(base.transform, self: true);
		}
	}

	public void Stop(Transform trans, bool self)
	{
		if (_isStopping)
		{
			return;
		}
		_isStopping = true;
		if (hasEXP)
		{
			Dicform component = (self ? LeanPool.Spawn(EXP, trans.position, Quaternion.identity) : LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity, trans)).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index;
		}
		if ((bool)FX && !CoverFX)
		{
			if (self)
			{
				LeanPool.Spawn(FX, trans.position, Quaternion.identity);
			}
			else
			{
				LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, trans);
			}
		}
		qiu.SetActive(value: false);
		CanMove = false;
		this.wait(DelDelay, DespawnSelf);
	}

	private void DespawnSelf()
	{
		if ((bool)father)
		{
			father.NotifyBallDespawn(this);
		}
		LeanPool.Despawn(this);
	}

	public void Refresh()
	{
		for (int i = 0; i < em.Count; i++)
		{
			if (!em[i].IsAlive || em[i].IsJump || em[i].IsYS)
			{
				em.Remove(em[i]);
				i--;
			}
		}
		if (Body)
		{
			em.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
		}
		else
		{
			em.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.transform.position, base.transform.position)));
		}
	}
}
