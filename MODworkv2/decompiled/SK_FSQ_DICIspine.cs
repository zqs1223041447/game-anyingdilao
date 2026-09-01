using Lean.Pool;
using UnityEngine;

public class SK_FSQ_DICIspine : MonoBehaviour
{
	public GameObject[] OBJ;

	public float speed;

	public float MainCOLsize;

	public float ciTime;

	public float DotMulti;

	public bool UseDicLifeTime;

	public float LifeTime;

	public float DelDelay;

	private float timeA;

	private float timeB;

	public bool single;

	public bool hasMainDamage;

	private bool CanMove;

	private bool CanAT;

	private int count;

	private float speedTMP;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public CircleCollider2D MainCOL;

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
		count = 0;
		speedTMP = speed;
		MainCOL.enabled = false;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		if (!single)
		{
			if (CanMove)
			{
				base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
			}
			timeB += Time.deltaTime;
			if (timeB >= ciTime && count < OBJ.Length)
			{
				timeB = 0f;
				LeanPool.Spawn(OBJ[count], base.transform.position, Quaternion.identity).GetComponent<SK_DICIspine>().SetColor(dic.sp.damageType);
				count++;
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
		if (single)
		{
			LeanPool.Spawn(OBJ[0], base.transform.position, Quaternion.identity).GetComponent<SK_DICIspine>().SetColor(dic.sp.damageType);
		}
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
