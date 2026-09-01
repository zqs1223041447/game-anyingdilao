using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_SonB : MonoBehaviour
{
	public GameObject OBJ;

	public GameObject FX;

	public ParticleSystem[] par;

	public GameObject dao;

	public int DotMulti;

	public bool Body;

	public float size;

	[Header("=========")]
	public float MoveSpeed;

	[HideInInspector]
	public bool Slow;

	public float LerpSpeed;

	private float MoveSpeedTmp;

	[Header("=========")]
	public float DelDelay;

	public int EveryCount;

	public float angleRange;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Collider2D MainCOL;

	[HideInInspector]
	public bool hasEmptyCOL;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	private bool CanMV;

	private int Count;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanAT = false;
		CanMV = false;
		MoveSpeedTmp = MoveSpeed;
		MainCOL.enabled = false;
		for (int i = 0; i < par.Length; i++)
		{
			ParticleSystem.MainModule main = par[i].main;
			main.loop = true;
		}
		initialized = false;
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
		CanAT = true;
		CanMV = true;
		MainCOL.enabled = true;
		if ((bool)dao)
		{
			dao.SetActive(value: true);
		}
		Count = dic.sp.Count_S;
		if (dic.Index == 0)
		{
			if (dic.sp.Slow_F == 0)
			{
				Slow = true;
			}
			else
			{
				Slow = false;
			}
		}
		else if (dic.sp.Slow_S == 0)
		{
			Slow = true;
		}
		else
		{
			Slow = false;
		}
		if (Slow)
		{
			hasEmptyCOL = true;
		}
		else
		{
			hasEmptyCOL = false;
		}
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		if ((bool)dao)
		{
			dao.transform.Rotate(new Vector3(0f, 0f, 1f), 720f * Time.deltaTime);
		}
		if (CanMV)
		{
			base.transform.Translate(Vector2.right * (MoveSpeedTmp * Time.deltaTime));
			if (Slow)
			{
				MoveSpeedTmp = Mathf.Lerp(MoveSpeedTmp, 0f, Time.deltaTime * LerpSpeed);
			}
		}
		if (!CanAT)
		{
			return;
		}
		if ((bool)OBJ && Count > 1)
		{
			timeB += Time.deltaTime;
			if (timeB > 1f / (float)EveryCount)
			{
				timeB = 0f;
				switch (dic.sp.Type_S)
				{
				case 0:
				{
					for (int j = 0; j < Mathf.RoundToInt((float)Count / (dic.sp.BuffTime * (float)EveryCount)); j++)
					{
						Dicform component2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).GetComponent<Dicform>();
						component2.sp = dic.sp;
						component2.SetCount(dic.sp.ZY);
						component2.SubType = dic.SubType;
						component2.Index = dic.Index;
					}
					break;
				}
				case 1:
				{
					for (int i = 0; i < Count / 200; i++)
					{
						Vector3 right = base.transform.right;
						float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
						Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - angleRange, angleRange))).GetComponent<Dicform>();
						component.sp = dic.sp;
						component.SetCount(dic.sp.ZY);
						component.SubType = dic.SubType;
						component.Index = dic.Index;
					}
					break;
				}
				}
			}
		}
		if (hasEmptyCOL)
		{
			timeC += Time.deltaTime;
			if (timeC > 0.25f)
			{
				timeC = 0f;
				EmptyCOL component3 = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity, base.transform).GetComponent<EmptyCOL>();
				Dicform component4 = component3.GetComponent<Dicform>();
				component4.sp = dic.sp;
				component4.SetCount(dic.sp.ZY);
				component4.SubType = dic.SubType;
				component4.Index = dic.Index;
				component3.size = size;
				component3.Body = Body;
				component3.DotMulti = DotMulti;
				component3.lifeTime = 0.1f;
				component3.IsGround = false;
			}
		}
		timeA += Time.deltaTime;
		if (timeA > dic.sp.BuffTime)
		{
			timeA = 0f;
			Stop();
		}
	}

	public void Stop()
	{
		CanAT = false;
		CanMV = false;
		for (int i = 0; i < 30; i++)
		{
			Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index;
		}
		for (int j = 0; j < par.Length; j++)
		{
			ParticleSystem.MainModule main = par[j].main;
			main.loop = false;
		}
		if ((bool)dao)
		{
			dao.SetActive(value: false);
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!CanMV)
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
				}
			}
			else
			{
				if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
				{
					component.peo.PL_Set(dic.sp, dic.SubType);
				}
				if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
				{
					component.peo.CP_Set(dic.sp, dic.SubType);
				}
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			Stop();
		}
	}
}
