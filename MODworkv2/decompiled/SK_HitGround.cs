using Lean.Pool;
using UnityEngine;

public class SK_HitGround : MonoBehaviour
{
	public GameObject FXqiu;

	public GameObject FXLast;

	[HideInInspector]
	public GameObject Main;

	[HideInInspector]
	public Transform trans;

	[Header("=========")]
	public float MainCOLsize;

	[Header("=========")]
	public bool DotAT;

	public GameObject colOBJ;

	public float SetColTime;

	public float ColLifeTime;

	public float size;

	[Header("=========")]
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

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<CircleCollider2D>();
		MainCOL.radius = MainCOLsize;
		Main = base.transform.Find("qiu").gameObject;
		trans = base.transform.Find("qiu/qiu");
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
		speedTMP = speed;
		Main.SetActive(value: true);
		MainCOL.enabled = false;
		initialized = false;
	}

	private void Update()
	{
		if (CanMove)
		{
			base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
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
				component2.Body = false;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= dic.sp.BuffTime)
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
		CanMove = true;
		CanAT = true;
		MainCOL.enabled = true;
	}

	public void Stop()
	{
		speedTMP = 0f;
		CanMove = false;
		CanAT = false;
		Main.SetActive(value: false);
		if ((bool)FXqiu)
		{
			LeanPool.Spawn(FXqiu, trans.position, Quaternion.identity);
		}
		if ((bool)FXLast)
		{
			LeanPool.Spawn(FXLast, base.transform.position, Quaternion.identity);
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
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
		if (collision.CompareTag("blockWALL"))
		{
			Stop();
		}
	}
}
