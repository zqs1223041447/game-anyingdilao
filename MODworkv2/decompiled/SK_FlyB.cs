using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FlyB : MonoBehaviour
{
	public GameObject FX;

	public int FXrate;

	public float LifeTime;

	public float speed;

	public int ChuanRate;

	public float DotMulti;

	[HideInInspector]
	public bool AllChuan;

	[HideInInspector]
	public Collider2D MainCOL;

	[HideInInspector]
	public Dicform dic;

	private int rate;

	private bool CanMove;

	private float timeA;

	private float timeB;

	private float angleTmp;

	private bool CanACT;

	private bool initialized;

	private float speedTMP;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<Collider2D>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanMove = false;
		CanACT = false;
		initialized = false;
		MainCOL.enabled = false;
		speedTMP = 0f;
	}

	private void LateUpdate()
	{
		Initialize();
	}

	private void Update()
	{
		if (CanMove)
		{
			base.transform.Translate(Vector2.right * (speedTMP * Time.deltaTime));
			timeA += Time.deltaTime;
			if (timeA > LifeTime)
			{
				timeA = 0f;
				LeanPool.Despawn(this);
			}
			timeB += Time.deltaTime;
			if (timeB > 0.2f)
			{
				timeB = 0f;
				CanACT = true;
			}
		}
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
		if (dic == null || dic.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		if (dic == null || dic.sp == null)
		{
			return;
		}
		initialized = true;
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
		CanMove = true;
		MainCOL.enabled = true;
		CanACT = true;
		speedTMP = speed;
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (!CanMove)
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
					if (FX != null && FXrate > 0)
					{
						rate = Random.Range(0, 101);
						if (rate < FXrate)
						{
							LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.em.yao.transform);
						}
					}
					if (dic.Index == 0 && CanACT)
					{
						SingletonMonoScope<ACTbar>.Instance.CreatACT_Hit(dic.sp.skillName, component.peo.em, base.transform.right);
						CanACT = false;
					}
					if (!AllChuan && Random.Range(0, 101) > ChuanRate)
					{
						LeanPool.Despawn(this);
					}
				}
			}
			else
			{
				if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
				{
					component.peo.PL_Set(dic.sp, dic.SubType);
					if (FX != null && FXrate > 0)
					{
						rate = Random.Range(0, 101);
						if (rate < FXrate)
						{
							LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.pl.yao.transform);
						}
					}
					if (!AllChuan && Random.Range(0, 101) > ChuanRate)
					{
						LeanPool.Despawn(this);
					}
				}
				if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
				{
					component.peo.CP_Set(dic.sp, dic.SubType);
					if (FX != null && FXrate > 0)
					{
						rate = Random.Range(0, 101);
						if (rate < FXrate)
						{
							LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.cp.yao.transform);
						}
					}
					if (!AllChuan && Random.Range(0, 101) > ChuanRate)
					{
						LeanPool.Despawn(this);
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
				LeanPool.Despawn(this);
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			LeanPool.Despawn(this);
		}
	}
}
