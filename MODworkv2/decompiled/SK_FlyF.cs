using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FlyF : MonoBehaviour
{
	public SpriteRenderer Arrow;

	public TrailRenderer[] trail;

	public float[] trTime;

	public GameObject[] par;

	[Header("=========")]
	public float LifeTime;

	public float DelDelay;

	public float speed;

	public float DotMulti;

	public bool hasMainDamage;

	[Header("=========")]
	public float ExpTimeMin;

	public float ExpTimeMax;

	public bool hasFX;

	public GameObject FX;

	public int ExpPosFX;

	public bool hasLastFX;

	[HideInInspector]
	public bool AllChuan;

	[HideInInspector]
	public Collider2D MainCOL;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Rigidbody2D rb;

	[HideInInspector]
	public Vector2 direction;

	private float FXcd;

	private bool CanFX;

	private bool CanMove;

	private float ACTcd;

	private bool CanACT;

	private float timeA;

	private float timeB;

	private float timeC;

	private float speedTMP;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		rb = GetComponent<Rigidbody2D>();
		MainCOL = GetComponent<Collider2D>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		direction = base.transform.right;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanMove = false;
		MainCOL.enabled = false;
		CanFX = false;
		CanACT = false;
		FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
		if (trail.Length != 0)
		{
			for (int i = 0; i < trail.Length; i++)
			{
				trail[i].emitting = true;
				trail[i].time = trTime[i];
			}
		}
		if (par.Length != 0)
		{
			for (int j = 0; j < par.Length; j++)
			{
				par[j].SetActive(value: true);
			}
		}
		if (Arrow != null)
		{
			Arrow.gameObject.SetActive(value: true);
		}
		speedTMP = 0f;
		initialized = false;
	}

	private void Update()
	{
		if (CanMove)
		{
			rb.MovePosition(rb.position + direction * (speedTMP * (1f + dic.sp.FlySpeed / 100f) * Time.deltaTime));
		}
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			Stop();
		}
		timeB += Time.deltaTime;
		if (timeB >= FXcd)
		{
			FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
			CanFX = true;
			timeB = 0f;
		}
		timeC += Time.deltaTime;
		if (timeC >= ACTcd)
		{
			ACTcd = Random.Range(ExpTimeMin, ExpTimeMax);
			CanACT = true;
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
		CanFX = true;
		CanMove = true;
		CanACT = true;
		MainCOL.enabled = true;
		if (dic.Index == 0)
		{
			if (dic.sp.AllChuan_F == 0)
			{
				AllChuan = true;
			}
			else
			{
				AllChuan = false;
			}
		}
		else if (dic.sp.AllChuan_S == 0)
		{
			AllChuan = true;
		}
		else
		{
			AllChuan = false;
		}
		speedTMP = speed;
	}

	public void Stop()
	{
		if (hasLastFX && (bool)FX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: false);
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: false);
		}
		if (trail.Length != 0)
		{
			for (int j = 0; j < trail.Length; j++)
			{
				trail[j].emitting = false;
			}
		}
		CanMove = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("BodyCOL"))
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if (dic.sp.ZY)
			{
				if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
				{
					if (hasMainDamage)
					{
						component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
					}
					if (dic.Index == 0 && CanACT)
					{
						SingletonMonoScope<ACTbar>.Instance.CreatACT_Hit(dic.sp.skillName, component.peo.em, base.transform.right);
						CanACT = false;
					}
					if (hasFX && FX != null && CanFX)
					{
						CanFX = false;
						GameObject gameObject = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
						switch (ExpPosFX)
						{
						case 0:
							gameObject.transform.position = component.peo.em.yao.transform.position;
							gameObject.transform.SetParent(component.peo.em.yao.transform);
							break;
						case 1:
							gameObject.transform.SetParent(component.peo.em.yao.transform);
							break;
						}
					}
					if (!AllChuan)
					{
						switch (dic.sp.ThroughType)
						{
						case 0:
							Stop();
							break;
						case 1:
							if ((float)Random.Range(0, 101) >= dic.sp.Through)
							{
								Stop();
								break;
							}
							dic.sp.Damage -= dic.sp.Damage * 0.02f;
							dic.sp.DamageA -= dic.sp.DamageA * 0.02f;
							dic.sp.DamageB -= dic.sp.DamageB * 0.02f;
							break;
						case 2:
							dic.sp.Damage -= dic.sp.Damage * 0.02f;
							dic.sp.DamageA -= dic.sp.DamageA * 0.02f;
							dic.sp.DamageB -= dic.sp.DamageB * 0.02f;
							break;
						}
					}
					else
					{
						dic.sp.Damage -= dic.sp.Damage * 0.02f;
						dic.sp.DamageA -= dic.sp.DamageA * 0.02f;
						dic.sp.DamageB -= dic.sp.DamageB * 0.02f;
					}
				}
			}
			else
			{
				if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
				{
					if (hasMainDamage)
					{
						component.peo.PL_Set(dic.sp, dic.SubType);
					}
					if (FX != null && CanFX)
					{
						CanFX = false;
						GameObject gameObject2 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
						switch (ExpPosFX)
						{
						case 0:
							gameObject2.transform.position = component.peo.pl.yao.transform.position;
							gameObject2.transform.SetParent(component.peo.pl.yao.transform);
							break;
						case 1:
							gameObject2.transform.SetParent(component.peo.pl.yao.transform);
							break;
						}
					}
					if (!AllChuan)
					{
						switch (dic.sp.ThroughType)
						{
						case 0:
							Stop();
							break;
						case 1:
							if ((float)Random.Range(0, 101) >= dic.sp.Through)
							{
								Stop();
							}
							break;
						}
					}
				}
				if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
				{
					if (hasMainDamage)
					{
						component.peo.CP_Set(dic.sp, dic.SubType);
					}
					if (FX != null && CanFX)
					{
						CanFX = false;
						GameObject gameObject3 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
						switch (ExpPosFX)
						{
						case 0:
							gameObject3.transform.position = component.peo.cp.yao.transform.position;
							gameObject3.transform.SetParent(component.peo.cp.yao.transform);
							break;
						case 1:
							gameObject3.transform.SetParent(component.peo.cp.yao.transform);
							break;
						}
					}
					if (!AllChuan)
					{
						switch (dic.sp.ThroughType)
						{
						case 0:
							Stop();
							break;
						case 1:
							if ((float)Random.Range(0, 101) >= dic.sp.Through)
							{
								Stop();
							}
							break;
						}
					}
				}
			}
		}
		if (collision.CompareTag("ZoneSK"))
		{
			SK_StromLord component2 = collision.GetComponent<SK_StromLord>();
			if (dic.sp.ZY)
			{
				component2.BuffZD(dic);
			}
			else if (component2.sp.CutSpeedZone > 0 && !dic.CutSpeed)
			{
				speedTMP = speedTMP / 100f * (float)(100 - component2.sp.CutSpeedZone);
				dic.CutSpeed = true;
			}
		}
		if (collision.CompareTag("DoomBall"))
		{
			SK_Doom_Ball component3 = collision.GetComponent<SK_Doom_Ball>();
			if (dic.sp.ZY)
			{
				component3.SetHit(dic, base.transform.right);
			}
			else if (component3.father.sp.TypeDIC_F > 0 && Random.Range(0, 101) < component3.father.sp.TypeDIC_F)
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
			Vector2 vector = collision.ClosestPoint(base.transform.position);
			Vector2 vector2 = (base.transform.position - new Vector3(vector.x, vector.y, 0f)).normalized;
			direction = base.transform.right;
			direction = Vector2.Reflect(direction, vector2);
			base.transform.position = vector + vector2 * 0.1f;
			float z = Mathf.Atan2(direction.y, direction.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
	}
}
