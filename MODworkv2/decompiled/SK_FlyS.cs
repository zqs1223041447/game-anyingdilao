using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FlyS : MonoBehaviour
{
	public SpriteRenderer Arrow;

	public TrailRenderer[] trail;

	public float[] trTime;

	public GameObject[] par;

	[Header("=========")]
	public float DotMulti;

	public float LifeTime;

	public float DelDelay;

	public float MoveSpeed;

	public float angle;

	public bool BeforAT;

	[Header("=========")]
	[HideInInspector]
	public bool hasMainDamage;

	public float ExpTimeMin;

	public float ExpTimeMax;

	[Header("=========")]
	public GameObject FX;

	public GameObject EXP;

	public int FXrate;

	public int ExpPosFX;

	public bool hasLastFX;

	[HideInInspector]
	public bool hasEXP;

	[HideInInspector]
	public bool ZY;

	private bool hasTarget;

	private bool CanMV;

	private bool CanRound;

	private float timeA;

	private float timeB;

	private float FXcd;

	private bool CanFX;

	[HideInInspector]
	public bool AllChuan;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Collider2D MainCOL;

	private float angleTmp;

	public Gun gun;

	public PlayerManager playerManager;

	private float speedTMP;

	private bool initialized;

	private void Awake()
	{
		MainCOL = GetComponent<Collider2D>();
		dic = GetComponent<Dicform>();
		gun = SingletonMonoScope<Gun>.Instance;
		playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanMV = false;
		CanFX = false;
		CanRound = true;
		FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
		MainCOL.enabled = false;
		angleTmp = Random.Range(0f - angle, angle);
		speedTMP = 0f;
		initialized = false;
	}

	private void Update()
	{
		if (CanMV)
		{
			base.transform.Translate(Vector2.right * (MoveSpeed * (1f + dic.sp.FlySpeed / 100f) * Time.deltaTime));
			timeA += Time.deltaTime;
			if (timeA > LifeTime)
			{
				Stop();
				timeA = 0f;
			}
			timeB += Time.deltaTime;
			if (timeB >= FXcd)
			{
				FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
				CanFX = true;
				timeB = 0f;
			}
		}
		if (!CanRound)
		{
			return;
		}
		if (dic.sp.ZY)
		{
			if (playerManager.IsAlive)
			{
				Vector3 vector = Gun.GetFlySAimWorldPos() - base.transform.position;
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				base.transform.rotation = Quaternion.Euler(0f, 0f, num + angleTmp);
				timeB += Time.deltaTime;
				if (timeB >= FXcd)
				{
					FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
					CanFX = true;
					timeB = 0f;
				}
			}
		}
		else if (dic.sp.em.IsAlive)
		{
			Vector3 vector2 = ((!dic.sp.em.ATTarget) ? (playerManager.yao.transform.position - base.transform.position) : (dic.sp.em.ATTarget.transform.position - base.transform.position));
			float num2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, num2 + angleTmp);
			timeB += Time.deltaTime;
			if (timeB >= FXcd)
			{
				FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
				CanFX = true;
				timeB = 0f;
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
		if (par != null && par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				if ((bool)par[i])
				{
					par[i].SetActive(value: true);
				}
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: true);
		}
		if (Random.Range(0, 10) > FXrate)
		{
			CanFX = true;
		}
		MainCOL.enabled = true;
		if (dic.sp.AllChuan_F == 0)
		{
			AllChuan = true;
		}
		else
		{
			AllChuan = false;
		}
		if (dic.sp.colEXP == 0 && EXP != null)
		{
			hasEXP = true;
			hasMainDamage = false;
		}
		else
		{
			hasEXP = false;
			hasMainDamage = true;
		}
		speedTMP = MoveSpeed;
	}

	public void FaShe()
	{
		CanMV = true;
		CanRound = false;
		base.transform.SetParent(null);
		if (trail == null || trail.Length == 0)
		{
			return;
		}
		for (int i = 0; i < trail.Length; i++)
		{
			if ((bool)trail[i])
			{
				trail[i].emitting = true;
				if (trTime != null && i < trTime.Length)
				{
					trail[i].time = trTime[i];
				}
			}
		}
	}

	public void Stop()
	{
		if ((bool)FX && hasLastFX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		CanMV = false;
		if (par != null && par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				if ((bool)par[i])
				{
					par[i].SetActive(value: false);
				}
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: false);
		}
		if (trail != null && trail.Length != 0)
		{
			for (int j = 0; j < trail.Length; j++)
			{
				if ((bool)trail[j])
				{
					trail[j].emitting = false;
				}
			}
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private bool CanTriggerBeforATDamage()
	{
		return Random.Range(0, 100) < 60;
	}

	private void SpawnEXP(Transform hitPoint)
	{
		if (!EXP)
		{
			return;
		}
		GameObject gameObject = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
		if (!gameObject)
		{
			return;
		}
		if ((bool)hitPoint)
		{
			switch (ExpPosFX)
			{
			case 0:
				gameObject.transform.position = hitPoint.position;
				gameObject.transform.SetParent(hitPoint);
				break;
			case 1:
				gameObject.transform.SetParent(hitPoint);
				break;
			}
		}
		Dicform component = gameObject.GetComponent<Dicform>();
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.SubType = dic.SubType;
		component.Index = dic.Index;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (BeforAT)
		{
			if (CanMV)
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
							if (hasEXP)
							{
								SpawnEXP(component.peo.em.yao.transform);
							}
							else if (FX != null && CanFX)
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
									}
									break;
								}
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
							if (hasEXP)
							{
								SpawnEXP(component.peo.pl.yao.transform);
							}
							else if (FX != null && CanFX)
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
							if (hasEXP)
							{
								SpawnEXP(component.peo.cp.yao.transform);
							}
							else if (FX != null && CanFX)
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
				if (collision.CompareTag("blockFLY"))
				{
					Stop();
				}
			}
			else if (collision.CompareTag("BodyCOL"))
			{
				if (!CanTriggerBeforATDamage())
				{
					return;
				}
				BodyCOL component2 = collision.GetComponent<BodyCOL>();
				if (dic.sp.ZY)
				{
					if (playerManager.IsAlive && component2.peo.CharacterType == 2 && component2.peo.em.IsAlive && !component2.peo.em.IsJump && !component2.peo.em.IsYS)
					{
						component2.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
						if (FX != null && CanFX)
						{
							CanFX = false;
							GameObject gameObject4 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
							switch (ExpPosFX)
							{
							case 0:
								gameObject4.transform.position = component2.peo.em.yao.transform.position;
								gameObject4.transform.SetParent(component2.peo.em.yao.transform);
								break;
							case 1:
								gameObject4.transform.SetParent(component2.peo.em.yao.transform);
								break;
							}
						}
					}
				}
				else
				{
					if (component2.peo.CharacterType == 0 && component2.peo.pl.IsAlive)
					{
						component2.peo.PL_Set(dic.sp, dic.SubType);
						if (FX != null && CanFX)
						{
							CanFX = false;
							GameObject gameObject5 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
							switch (ExpPosFX)
							{
							case 0:
								gameObject5.transform.position = component2.peo.pl.yao.transform.position;
								gameObject5.transform.SetParent(component2.peo.pl.yao.transform);
								break;
							case 1:
								gameObject5.transform.SetParent(component2.peo.pl.yao.transform);
								break;
							}
						}
					}
					if (component2.peo.CharacterType == 1 && component2.peo.cp.IsAlive)
					{
						component2.peo.CP_Set(dic.sp, dic.SubType);
						if (FX != null && CanFX)
						{
							CanFX = false;
							GameObject gameObject6 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
							switch (ExpPosFX)
							{
							case 0:
								gameObject6.transform.position = component2.peo.cp.yao.transform.position;
								gameObject6.transform.SetParent(component2.peo.cp.yao.transform);
								break;
							case 1:
								gameObject6.transform.SetParent(component2.peo.cp.yao.transform);
								break;
							}
						}
					}
				}
			}
			if (collision.CompareTag("Break"))
			{
				collision.GetComponent<BreakOBJ>().Break();
			}
		}
		else
		{
			if (!CanMV)
			{
				return;
			}
			if (collision.CompareTag("BodyCOL"))
			{
				BodyCOL component3 = collision.GetComponent<BodyCOL>();
				if (dic.sp.ZY)
				{
					if (component3.peo.CharacterType == 2 && component3.peo.em.IsAlive && !component3.peo.em.IsJump && !component3.peo.em.IsYS)
					{
						if (hasMainDamage)
						{
							component3.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
						}
						if (hasEXP)
						{
							SpawnEXP(component3.peo.em.yao.transform);
						}
						else if (FX != null && CanFX)
						{
							CanFX = false;
							GameObject gameObject7 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
							switch (ExpPosFX)
							{
							case 0:
								gameObject7.transform.position = component3.peo.em.yao.transform.position;
								gameObject7.transform.SetParent(component3.peo.em.yao.transform);
								break;
							case 1:
								gameObject7.transform.SetParent(component3.peo.em.yao.transform);
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
				else
				{
					if (component3.peo.CharacterType == 0 && component3.peo.pl.IsAlive)
					{
						if (hasMainDamage)
						{
							component3.peo.PL_Set(dic.sp, dic.SubType);
						}
						if (hasEXP)
						{
							SpawnEXP(component3.peo.pl.yao.transform);
						}
						else if (FX != null && CanFX)
						{
							CanFX = false;
							GameObject gameObject8 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
							switch (ExpPosFX)
							{
							case 0:
								gameObject8.transform.position = component3.peo.pl.yao.transform.position;
								gameObject8.transform.SetParent(component3.peo.pl.yao.transform);
								break;
							case 1:
								gameObject8.transform.SetParent(component3.peo.pl.yao.transform);
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
					if (component3.peo.CharacterType == 1 && component3.peo.cp.IsAlive)
					{
						if (hasMainDamage)
						{
							component3.peo.CP_Set(dic.sp, dic.SubType);
						}
						if (hasEXP)
						{
							SpawnEXP(component3.peo.cp.yao.transform);
						}
						else if (FX != null && CanFX)
						{
							CanFX = false;
							GameObject gameObject9 = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
							switch (ExpPosFX)
							{
							case 0:
								gameObject9.transform.position = component3.peo.cp.yao.transform.position;
								gameObject9.transform.SetParent(component3.peo.cp.yao.transform);
								break;
							case 1:
								gameObject9.transform.SetParent(component3.peo.cp.yao.transform);
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
				SK_StromLord component4 = collision.GetComponent<SK_StromLord>();
				if (dic.sp.ZY)
				{
					component4.BuffZD(dic);
				}
				else if (component4.sp.CutSpeedZone > 0 && !dic.CutSpeed)
				{
					speedTMP = speedTMP / 100f * (float)(100 - component4.sp.CutSpeedZone);
					dic.CutSpeed = true;
				}
			}
			if (collision.CompareTag("DoomBall"))
			{
				SK_Doom_Ball component5 = collision.GetComponent<SK_Doom_Ball>();
				if (dic.sp.ZY)
				{
					component5.SetHit(dic, base.transform.right);
				}
				else if (component5.father.sp.TypeDIC_F > 0 && Random.Range(0, 101) < component5.father.sp.TypeDIC_F)
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
}
