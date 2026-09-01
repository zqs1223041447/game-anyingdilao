using Lean.Pool;
using UnityEngine;

public class SK_FlyC : MonoBehaviour
{
	public GameObject[] par;

	public ParticleSystem[] parLoop;

	[Header("=========")]
	public float DotMulti;

	public float LifeTime;

	public float DelDelay;

	public float MoveSpeed;

	public bool Slow;

	public float LerpSpeed;

	private float speedTMP;

	public int DamCount;

	[Header("=========")]
	public bool hasMainDamage;

	public bool DICmove;

	[Header("=========")]
	public float ExpTimeMin;

	public float ExpTimeMax;

	public GameObject FX;

	public GameObject LastFX;

	public int ExpPosFX;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Collider2D MainCOL;

	private bool CanMV;

	private float FXcd;

	private bool CanFX;

	private bool canDAM;

	private float timeA;

	private float timeB;

	private int DamCountTmp;

	private bool initialized;

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
		timeB = 0f;
		timeA = 0f;
		CanFX = false;
		FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
		MainCOL.enabled = false;
		speedTMP = 0f;
		canDAM = false;
		CanMV = false;
		DamCountTmp = DamCount;
		initialized = false;
	}

	private void Update()
	{
		timeB += Time.deltaTime;
		if (timeB > LifeTime)
		{
			timeB = 0f;
			Stop();
		}
		timeA += Time.deltaTime;
		if (timeA >= FXcd)
		{
			FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
			CanFX = true;
			timeA = 0f;
		}
		if (CanMV)
		{
			if (DICmove)
			{
				base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
			}
			else
			{
				base.transform.Translate(Vector2.right * (speedTMP * Time.deltaTime));
			}
			if (Slow)
			{
				speedTMP = Mathf.Lerp(speedTMP, 0f, Time.deltaTime * LerpSpeed);
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
		speedTMP = MoveSpeed;
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: true);
			}
		}
		if (parLoop.Length != 0)
		{
			for (int j = 0; j < parLoop.Length; j++)
			{
				ParticleSystem.MainModule main = parLoop[j].main;
				main.loop = true;
			}
		}
		CanFX = true;
		MainCOL.enabled = true;
		canDAM = true;
		CanMV = true;
	}

	public void Stop()
	{
		if (LastFX != null)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		canDAM = false;
		CanMV = false;
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: false);
			}
		}
		if (parLoop.Length != 0)
		{
			for (int j = 0; j < parLoop.Length; j++)
			{
				ParticleSystem.MainModule main = parLoop[j].main;
				main.loop = false;
			}
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (canDAM)
		{
			if (DamCountTmp > 0)
			{
				if (collision.CompareTag("BodyCOL"))
				{
					BodyCOL component = collision.GetComponent<BodyCOL>();
					if (dic.sp.ZY)
					{
						if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
						{
							DamCountTmp--;
							if (hasMainDamage)
							{
								component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
							}
							if (FX != null && CanFX)
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
								case 2:
									gameObject.transform.position = component.peo.em.transform.position;
									break;
								}
							}
						}
					}
					else
					{
						if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
						{
							DamCountTmp--;
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
								case 2:
									gameObject2.transform.position = component.peo.pl.transform.position;
									break;
								}
							}
						}
						if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
						{
							DamCountTmp--;
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
								case 2:
									gameObject3.transform.position = component.peo.cp.transform.position;
									break;
								}
							}
						}
					}
				}
			}
			else
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
			Stop();
		}
	}
}
