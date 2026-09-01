using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_GroundBall : MonoBehaviour
{
	public GameObject colOBJ;

	public GameObject[] parOBJ;

	public GameObject FX;

	public GameObject EXP;

	[Header("=========")]
	public float DotMulti;

	public float lifeTime;

	public float DelDelay;

	public float FaSheTime;

	public float speed;

	public bool Slow;

	public float LerpSpeed;

	[Header("=========")]
	public float MainCOLsize;

	public bool Body;

	[Header("=========")]
	public bool HasGround;

	public float SetColTime;

	public float ColLifeTime;

	public float size;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public CircleCollider2D MainCOL;

	private bool CanMove;

	private bool CanAT;

	private float FXcd;

	private bool CanFX;

	private bool CanACT;

	private float speedTMP;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private float timeE;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<CircleCollider2D>();
		MainCOL.radius = MainCOLsize;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		CanMove = false;
		CanAT = false;
		CanACT = false;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		timeE = 0f;
		speedTMP = speed;
		MainCOL.enabled = false;
		initialized = false;
	}

	private void Update()
	{
		if (CanMove)
		{
			base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
			if (Slow)
			{
				speedTMP = Mathf.Lerp(speedTMP, 0f, Time.deltaTime * LerpSpeed);
			}
		}
		if (HasGround && CanAT)
		{
			timeB += Time.deltaTime;
			if (timeB >= SetColTime)
			{
				timeB = 0f;
				GameObject obj = LeanPool.Spawn(colOBJ, base.transform.position, Quaternion.identity);
				Dicform component = obj.GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = 3;
				component.Index = dic.Index + 1;
				SK_Field component2 = obj.GetComponent<SK_Field>();
				component2.UseDicLifeTime = false;
				component2.LifeTime = ColLifeTime;
				component2.size = size;
				component2.Body = Body;
			}
		}
		if (!CanAT)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= lifeTime)
		{
			timeA = 0f;
			Stop();
		}
		timeC += Time.deltaTime;
		if (timeC >= FXcd)
		{
			FXcd = Random.Range(0.3f, 0.6f);
			CanFX = true;
			timeC = 0f;
		}
		timeD += Time.deltaTime;
		if (timeD >= FaSheTime)
		{
			if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)SubA)
			{
				Vector3 vector = dic.dic;
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				Dicform component3 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.Euler(0f, 0f, num + (float)Random.Range(-40, 40))).GetComponent<Dicform>();
				component3.sp = dic.sp;
				component3.SetCount(dic.sp.ZY);
				component3.SubType = 1;
				component3.Index = dic.Index + 1;
			}
			timeD = 0f;
		}
		timeE += Time.deltaTime;
		if (timeE >= 0.3f)
		{
			CanACT = true;
			timeE = 0f;
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
		FXcd = Random.Range(0.3f, 0.6f);
		CanFX = true;
		CanACT = true;
		if (parOBJ.Length != 0)
		{
			for (int i = 0; i < parOBJ.Length; i++)
			{
				parOBJ[i].SetActive(value: true);
			}
		}
		CanMove = true;
		CanAT = true;
		MainCOL.enabled = true;
	}

	public void Stop()
	{
		CanMove = false;
		speedTMP = 0f;
		CanAT = false;
		if (parOBJ.Length != 0)
		{
			for (int i = 0; i < parOBJ.Length; i++)
			{
				parOBJ[i].SetActive(value: false);
			}
		}
		if ((bool)EXP)
		{
			Dicform component = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index + 1;
		}
		this.wait(0.2f, DelayEXP);
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void DelayEXP()
	{
		if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)SubB)
		{
			Dicform component = LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = 2;
			component.Index = dic.Index + 1;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CanAT && collision.CompareTag("FootCOL"))
		{
			FootCOL component = collision.GetComponent<FootCOL>();
			if (CanMove)
			{
				if (dic.sp.ZY)
				{
					if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
					{
						component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
						if (dic.Index == 0 && CanACT)
						{
							SingletonMonoScope<ACTbar>.Instance.CreatACT_Hit(dic.sp.skillName, component.peo.em, dic.dic);
							CanACT = false;
						}
						if (FX != null && CanFX)
						{
							LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
							CanFX = false;
						}
					}
				}
				else
				{
					if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
					{
						component.peo.PL_Set(dic.sp, dic.SubType);
						if (FX != null && CanFX)
						{
							LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
						}
					}
					if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
					{
						component.peo.CP_Set(dic.sp, dic.SubType);
						if (FX != null && CanFX)
						{
							LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
						}
					}
				}
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			speedTMP = 0f;
			timeA += 1f;
		}
	}
}
