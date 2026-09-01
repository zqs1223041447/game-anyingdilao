using Lean.Pool;
using UnityEngine;

public class SK_FSQ_DICI : MonoBehaviour
{
	public GameObject[] OBJ;

	public bool hasOBJ;

	public int CiType;

	public float ATtime;

	public float range;

	[Header("=========")]
	public bool MainAT;

	public float MainCOLsize;

	[Header("=========")]
	public bool DotAT;

	public GameObject colOBJ;

	public bool Body;

	public float SetColTime;

	public float ColLifeTime;

	public float size;

	[Header("=========")]
	public bool UseDicLifeTime;

	public float LifeTime;

	public float DelDelay;

	public float speed;

	public bool hasMainDamage;

	public float DotMulti;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public CircleCollider2D MainCOL;

	private bool CanMove;

	private bool CanAT;

	private float speedTMP;

	private float timeA;

	private float timeB;

	private float timeC;

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
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		speedTMP = speed;
		MainCOL.enabled = false;
		initialized = false;
	}

	private void Update()
	{
		if (CanMove)
		{
			base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
		}
		if (hasOBJ && CanAT)
		{
			timeC += Time.deltaTime;
			if (timeC >= ATtime)
			{
				timeC = 0f;
				switch (CiType)
				{
				case 0:
				{
					if (Random.Range(0, 101) > 50)
					{
						Vector2 vector4 = Random.insideUnitCircle * range;
						LeanPool.Spawn(OBJ[0], new Vector3(base.transform.position.x + vector4.x, base.transform.position.y + vector4.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					else
					{
						Vector2 vector5 = Random.insideUnitCircle * range;
						LeanPool.Spawn(OBJ[1], new Vector3(base.transform.position.x + vector5.x, base.transform.position.y + vector5.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					for (int l = 0; l < 1; l++)
					{
						Vector2 vector6 = Random.insideUnitCircle * (range * 1.8f);
						LeanPool.Spawn(OBJ[2], new Vector3(base.transform.position.x + vector6.x, base.transform.position.y + vector6.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					for (int m = 0; m < 1; m++)
					{
						Vector2 vector7 = Random.insideUnitCircle * (range * 1.8f);
						LeanPool.Spawn(OBJ[3], new Vector3(base.transform.position.x + vector7.x, base.transform.position.y + vector7.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					for (int n = 0; n < 3; n++)
					{
						Vector2 vector8 = Random.insideUnitCircle * (range * 2.2f);
						LeanPool.Spawn(OBJ[4], new Vector3(base.transform.position.x + vector8.x, base.transform.position.y + vector8.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					for (int num = 0; num < 3; num++)
					{
						Vector2 vector9 = Random.insideUnitCircle * (range * 2.2f);
						LeanPool.Spawn(OBJ[5], new Vector3(base.transform.position.x + vector9.x, base.transform.position.y + vector9.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					for (int num2 = 0; num2 < 2; num2++)
					{
						Vector2 vector10 = Random.insideUnitCircle * (range * 1f);
						LeanPool.Spawn(OBJ[6], new Vector3(base.transform.position.x + vector10.x, base.transform.position.y + vector10.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					break;
				}
				case 1:
				{
					for (int i = 0; i < 1; i++)
					{
						Vector2 vector = Random.insideUnitCircle * (range * 0.8f);
						LeanPool.Spawn(OBJ[0], new Vector3(base.transform.position.x + vector.x, base.transform.position.y + vector.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					for (int j = 0; j < 1; j++)
					{
						Vector2 vector2 = Random.insideUnitCircle * (range * 1.4f);
						LeanPool.Spawn(OBJ[1], new Vector3(base.transform.position.x + vector2.x, base.transform.position.y + vector2.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					for (int k = 0; k < 2; k++)
					{
						Vector2 vector3 = Random.insideUnitCircle * (range * 1.8f);
						LeanPool.Spawn(OBJ[2], new Vector3(base.transform.position.x + vector3.x, base.transform.position.y + vector3.y, 0f), Quaternion.Euler(0f, 0f, Random.Range(-5, 5)));
					}
					break;
				}
				case 2:
					LeanPool.Spawn(OBJ[0], base.transform.position, Quaternion.identity);
					break;
				}
			}
		}
		if (!CanAT)
		{
			return;
		}
		if (DotAT)
		{
			timeB += Time.deltaTime;
			if (timeB >= SetColTime)
			{
				timeB = 0f;
				GameObject obj = LeanPool.Spawn(colOBJ, base.transform.position, Quaternion.identity);
				Dicform component = obj.GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				SK_Field component2 = obj.GetComponent<SK_Field>();
				component2.UseDicLifeTime = false;
				component2.LifeTime = ColLifeTime;
				component2.size = size;
				component2.Body = Body;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= LifeTime)
		{
			timeA = 0f;
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
		if (UseDicLifeTime)
		{
			LifeTime = dic.sp.BuffTime;
		}
		CanMove = true;
		CanAT = true;
		MainCOL.enabled = true;
	}

	public void Stop()
	{
		speedTMP = 0f;
		CanMove = false;
		CanAT = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (MainAT && CanAT)
		{
			if (Body)
			{
				if (collision.CompareTag("BodyCOL"))
				{
					BodyCOL component = collision.GetComponent<BodyCOL>();
					if (CanMove)
					{
						if (dic.sp.ZY)
						{
							if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS && hasMainDamage)
							{
								component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
							}
						}
						else
						{
							if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive && hasMainDamage)
							{
								component.peo.PL_Set(dic.sp, dic.SubType);
							}
							if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive && hasMainDamage)
							{
								component.peo.CP_Set(dic.sp, dic.SubType);
							}
						}
					}
				}
			}
			else if (collision.CompareTag("FootCOL"))
			{
				FootCOL component2 = collision.GetComponent<FootCOL>();
				if (CanMove)
				{
					if (dic.sp.ZY)
					{
						if (component2.peo.CharacterType == 2 && component2.peo.em.IsAlive && !component2.peo.em.IsJump && !component2.peo.em.IsYS && hasMainDamage)
						{
							component2.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
						}
					}
					else
					{
						if (component2.peo.CharacterType == 0 && component2.peo.pl.IsAlive && hasMainDamage)
						{
							component2.peo.PL_Set(dic.sp, dic.SubType);
						}
						if (component2.peo.CharacterType == 1 && component2.peo.cp.IsAlive && hasMainDamage)
						{
							component2.peo.CP_Set(dic.sp, dic.SubType);
						}
					}
				}
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockWALL"))
		{
			Stop();
		}
	}
}
