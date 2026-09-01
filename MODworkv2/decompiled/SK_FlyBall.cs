using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FlyBall : MonoBehaviour
{
	private const float FxFrameFps = 14f;

	[NonSerialized]
	private bool _fxFlipOn;

	[NonSerialized]
	private float _fxFrameClock;

	[NonSerialized]
	private bool _fxBurstDone;

	public SpriteRenderer Arrow;

	public SpriteRenderer[] spr;

	public TrailRenderer[] trail;

	public float[] trTime;

	public GameObject[] par;

	public ParticleSystem[] parLoop;

	[Header("=========")]
	public float DotMulti;

	public float LifeTime;

	public float DelDelay;

	private float LifeTimeTmp;

	public float MoveSpeed;

	[HideInInspector]
	public bool Slow;

	[HideInInspector]
	public bool RandomSpeed;

	private float speedTMP;

	[Header("=========")]
	[HideInInspector]
	public bool hasMainDamage;

	public bool DICmove;

	[Header("=========")]
	[HideInInspector]
	public bool Follow;

	[HideInInspector]
	public bool Body;

	public float starFollowTime;

	public float lerpAngle;

	public float SetRangeTime;

	public float min;

	public float max;

	[Header("=========")]
	public float ExpTimeMin;

	public float ExpTimeMax;

	public GameObject FX;

	public GameObject EXP;

	public bool NoLastFX;

	[HideInInspector]
	public bool hasFX;

	[HideInInspector]
	public bool colEXP;

	[HideInInspector]
	public int ExpPos;

	[HideInInspector]
	public bool AngleEXP;

	[HideInInspector]
	public bool TimeEXP;

	[HideInInspector]
	public bool LastEXP;

	[HideInInspector]
	public int ZDtimeCount;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	[HideInInspector]
	public int SubApos;

	[HideInInspector]
	public bool AngleEXP_A;

	[HideInInspector]
	public bool timeEXP_A;

	[HideInInspector]
	public bool colEXP_A;

	[HideInInspector]
	public bool AllChuan;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Collider2D MainCOL;

	private bool CanMV;

	private bool canFollow;

	private bool startFollow;

	private float FXcd;

	private float EXPAcd;

	private bool CanFX;

	private bool CanEXPA;

	private float ACTcd;

	private bool CanACT;

	private float starFollowTimeTmp;

	private float range;

	private bool canDAM;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private float timeF;

	private float timeG;

	private float timeH;

	private float timeI;

	private float timeJ;

	public List<Enemy> em = new List<Enemy>();

	[HideInInspector]
	public List<Companion> cp = new List<Companion>();

	[HideInInspector]
	public List<PlayerManager> pl = new List<PlayerManager>();

	public Collider2D[] hitEM = new Collider2D[6];

	public Collider2D[] hitCP = new Collider2D[3];

	public Collider2D[] hitPL = new Collider2D[1];

	[HideInInspector]
	public Transform target;

	private bool initialized;

	[NonSerialized]
	public bool ReturnToPlayer;

	private bool returning;

	private int pierceLeft;

	private float returnClock;

	private bool FxIsFireBall
	{
		get
		{
			try
			{
				return dic != null && dic.sp != null && dic.sp.skillName == "FireBall";
			}
			catch
			{
				return false;
			}
		}
	}

	private Material FirstParticleMaterial()
	{
		try
		{
			if (parLoop != null && parLoop.Length != 0 && (bool)parLoop[0])
			{
				ParticleSystemRenderer component = parLoop[0].GetComponent<ParticleSystemRenderer>();
				if (component != null && component.sharedMaterial != null)
				{
					return component.sharedMaterial;
				}
			}
			if (par != null && par.Length != 0 && (bool)par[0])
			{
				ParticleSystemRenderer componentInChildren = par[0].GetComponentInChildren<ParticleSystemRenderer>();
				if (componentInChildren != null && componentInChildren.sharedMaterial != null)
				{
					return componentInChildren.sharedMaterial;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		em.Clear();
		cp.Clear();
		pl.Clear();
		ReturnToPlayer = false;
		returning = false;
		pierceLeft = 0;
		returnClock = 0f;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		timeF = 0f;
		timeG = 0f;
		timeH = 0f;
		timeI = 0f;
		timeJ = 0f;
		starFollowTimeTmp = UnityEngine.Random.Range(starFollowTime * 0.4f, starFollowTime);
		FXcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		EXPAcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		ACTcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		CanEXPA = false;
		CanACT = false;
		MainCOL.enabled = false;
		canDAM = false;
		CanMV = false;
		startFollow = false;
		canFollow = false;
		speedTMP = 0f;
		range = min;
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		for (int j = 0; j < hitCP.Length; j++)
		{
			hitCP[j] = null;
		}
		hitPL[0] = null;
		initialized = false;
	}

	private void Update()
	{
		if (_fxFlipOn && (bool)Arrow)
		{
			_fxFrameClock += Time.deltaTime;
			Sprite sprite = FxSpriteFactory.FireFrame(Mathf.FloorToInt(_fxFrameClock * 14f) % 8);
			if (sprite != null && Arrow.sprite != sprite)
			{
				Arrow.sprite = sprite;
			}
		}
		if (!CanMV)
		{
			return;
		}
		if (returning)
		{
			returnClock += Time.deltaTime;
			if (target == null || returnClock > 5f)
			{
				Stop();
				return;
			}
			Vector3 vector = target.position - base.transform.position;
			base.transform.position += vector.normalized * (speedTMP * Time.deltaTime);
			if (Vector3.Distance(target.position, base.transform.position) < 0.6f)
			{
				Stop();
			}
			return;
		}
		if (Follow)
		{
			timeG += Time.deltaTime;
			if (timeG >= 0.18f)
			{
				if (dic.sp.ZY)
				{
					if (Body)
					{
						int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("BodyCOLem"));
						if (num > 0)
						{
							for (int i = 0; i < num; i++)
							{
								BodyCOL component = hitEM[i].GetComponent<BodyCOL>();
								if (component != null)
								{
									if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS)
									{
										em.Add(component.peo.em);
									}
									hitEM[i] = null;
								}
							}
						}
					}
					else
					{
						int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("FootCOLem"));
						if (num2 > 0)
						{
							for (int j = 0; j < num2; j++)
							{
								FootCOL component2 = hitEM[j].GetComponent<FootCOL>();
								if (component2 != null)
								{
									if (component2.peo.CharacterType == 2 && component2.peo.em.IsAlive && !em.Contains(component2.peo.em) && !component2.peo.em.IsJump && !component2.peo.em.IsYS)
									{
										em.Add(component2.peo.em);
									}
									hitEM[j] = null;
								}
							}
						}
					}
				}
				else if (Body)
				{
					int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitCP, LayerMask.GetMask("BodyCOLcp"));
					if (num3 > 0)
					{
						for (int k = 0; k < num3; k++)
						{
							BodyCOL component3 = hitCP[k].GetComponent<BodyCOL>();
							if (component3 != null)
							{
								if (component3.peo.CharacterType == 1 && component3.peo.cp.IsAlive && !cp.Contains(component3.peo.cp))
								{
									cp.Add(component3.peo.cp);
								}
								hitCP[k] = null;
							}
						}
					}
					int num4 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitPL, LayerMask.GetMask("BodyCOLpl"));
					if (num4 > 0)
					{
						for (int l = 0; l < num4; l++)
						{
							BodyCOL component4 = hitPL[l].GetComponent<BodyCOL>();
							if (component4 != null)
							{
								if (component4.peo.CharacterType == 0 && component4.peo.pl.IsAlive && !pl.Contains(component4.peo.pl))
								{
									pl.Add(component4.peo.pl);
								}
								hitPL[l] = null;
							}
						}
					}
				}
				else
				{
					int num5 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitCP, LayerMask.GetMask("FootCOLcp"));
					if (num5 > 0)
					{
						for (int m = 0; m < num5; m++)
						{
							FootCOL component5 = hitCP[m].GetComponent<FootCOL>();
							if (component5 != null)
							{
								if (component5.peo.CharacterType == 1 && component5.peo.cp.IsAlive && !cp.Contains(component5.peo.cp))
								{
									cp.Add(component5.peo.cp);
								}
								hitCP[m] = null;
							}
						}
					}
					int num6 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitPL, LayerMask.GetMask("FootCOLpl"));
					if (num6 > 0)
					{
						for (int n = 0; n < num6; n++)
						{
							FootCOL component6 = hitPL[n].GetComponent<FootCOL>();
							if (component6 != null)
							{
								if (component6.peo.CharacterType == 0 && component6.peo.pl.IsAlive && !pl.Contains(component6.peo.pl))
								{
									pl.Add(component6.peo.pl);
								}
								hitPL[n] = null;
							}
						}
					}
				}
				Refresh();
				timeG = 0f;
			}
			timeJ += Time.deltaTime;
			if (timeJ >= SetRangeTime)
			{
				if (range < max)
				{
					range += 1f;
				}
				timeJ = 0f;
			}
		}
		timeC += Time.deltaTime;
		if (timeC > 0.6f)
		{
			if (canDAM)
			{
				SetEXP();
			}
			timeC = 0f;
		}
		timeD += Time.deltaTime;
		if (timeD > 0.03f && canDAM)
		{
			timeD = 0f;
			SetZiDan();
		}
		timeB += Time.deltaTime;
		if (timeB > LifeTimeTmp)
		{
			timeB = 0f;
			TimeStop();
		}
		if (!startFollow)
		{
			timeA += Time.deltaTime;
			if (timeA > starFollowTimeTmp)
			{
				startFollow = true;
				timeA = 0f;
			}
		}
		timeF += Time.deltaTime;
		if (timeF >= FXcd)
		{
			FXcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanFX = true;
			timeF = 0f;
		}
		timeH += Time.deltaTime;
		if (timeH >= EXPAcd)
		{
			EXPAcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanEXPA = true;
			timeH = 0f;
		}
		timeI += Time.deltaTime;
		if (timeI >= ACTcd)
		{
			ACTcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanACT = true;
			timeI = 0f;
		}
		if (!Follow)
		{
			SimpleMV();
			if (Slow)
			{
				speedTMP = Mathf.Lerp(speedTMP, 0f, Time.deltaTime * 2f);
			}
		}
		else if (startFollow)
		{
			if (dic.sp.ZY)
			{
				if (em.Count > 0)
				{
					if (Body)
					{
						target = em[0].yao.transform;
					}
					else
					{
						target = em[0].transform;
					}
					FollowMV();
				}
				else
				{
					SimpleMV();
				}
			}
			else if (cp.Count > 0 || pl.Count > 0)
			{
				if (cp.Count > 0 && pl.Count > 0)
				{
					if (Body)
					{
						if (Vector3.Distance(pl[0].yao.transform.position, base.transform.position) - Vector3.Distance(cp[0].yao.transform.position, base.transform.position) < 0.3f)
						{
							target = pl[0].yao.transform;
						}
						else
						{
							target = cp[0].yao.transform;
						}
					}
					else if (Vector3.Distance(pl[0].transform.position, base.transform.position) - Vector3.Distance(cp[0].transform.position, base.transform.position) < 0.3f)
					{
						target = pl[0].transform;
					}
					else
					{
						target = cp[0].transform;
					}
					FollowMV();
					return;
				}
				if (pl.Count > 0)
				{
					if (Body)
					{
						target = pl[0].yao.transform;
					}
					else
					{
						target = pl[0].transform;
					}
				}
				else if (Body)
				{
					target = cp[0].yao.transform;
				}
				else
				{
					target = cp[0].transform;
				}
				FollowMV();
			}
			else
			{
				SimpleMV();
			}
		}
		else
		{
			SimpleMV();
		}
	}

	public void FollowMV()
	{
		if (DICmove)
		{
			dic.dic = target.position - base.transform.position;
			if (Vector2.Distance(target.transform.position, base.transform.position) > 0.3f)
			{
				base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
			}
			else
			{
				base.transform.Translate(dic.dic.normalized * speedTMP / 5f * Time.deltaTime);
			}
		}
		else if (!AllChuan && dic.sp.Through == 0f)
		{
			base.transform.position += base.transform.right * (speedTMP * Time.deltaTime);
			base.transform.right = Vector3.Slerp(base.transform.right, target.position - base.transform.position, lerpAngle / Vector3.Distance(target.position, base.transform.position));
		}
		else if (AllChuan || dic.sp.Through > 0f)
		{
			if (Vector2.Distance(target.transform.position, base.transform.position) > 0.3f)
			{
				base.transform.position += base.transform.right * (speedTMP * Time.deltaTime);
				base.transform.right = Vector3.Slerp(base.transform.right, target.position - base.transform.position, lerpAngle / Vector3.Distance(target.position, base.transform.position));
			}
			else if (Vector2.Distance(target.transform.position, base.transform.position) < 0.3f && Vector2.Distance(target.transform.position, base.transform.position) > 0.1f)
			{
				base.transform.position += base.transform.right * speedTMP / 3f * Time.deltaTime;
				base.transform.right = Vector3.Slerp(base.transform.right, target.position - base.transform.position, lerpAngle / Vector3.Distance(target.position, base.transform.position));
			}
		}
	}

	public void SimpleMV()
	{
		if (DICmove)
		{
			base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
		}
		else
		{
			base.transform.Translate(Vector2.right * (speedTMP * Time.deltaTime));
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
		if (!dic || !dic.sp)
		{
			return;
		}
		try
		{
			ReturnToPlayer = PoeItemMod.ReturnEquipped;
		}
		catch
		{
			ReturnToPlayer = false;
		}
		pierceLeft = (ReturnToPlayer ? 1 : 0);
		_fxFlipOn = false;
		_fxBurstDone = false;
		try
		{
			if (FxIsFireBall)
			{
				_fxFlipOn = true;
				_fxFrameClock = 0f;
				Sprite sprite = FxSpriteFactory.FireFrame(0);
				if ((bool)Arrow && sprite != null)
				{
					Arrow.sprite = sprite;
				}
			}
		}
		catch
		{
		}
		if (par.Length != 0)
		{
			GameObject[] array = par;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array2 = parLoop;
			for (int j = 0; j < array2.Length; j++)
			{
				ParticleSystem.MainModule main = array2[j].main;
				main.loop = true;
			}
		}
		if (trail.Length != 0)
		{
			for (int k = 0; k < trail.Length; k++)
			{
				trail[k].emitting = true;
				trail[k].time = trTime[k];
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: true);
		}
		if (spr.Length != 0)
		{
			SpriteRenderer[] array3 = spr;
			for (int l = 0; l < array3.Length; l++)
			{
				array3[l].gameObject.SetActive(value: true);
			}
		}
		if (dic.Index == 0)
		{
			ZDtimeCount = dic.sp.CountMulti;
			if (dic.sp.ZD_time_F == 0f)
			{
				LifeTimeTmp = LifeTime;
			}
			else
			{
				LifeTimeTmp = dic.sp.ZD_time_F;
			}
			if (dic.sp.AllChuan_F == 0)
			{
				AllChuan = true;
			}
			else
			{
				AllChuan = false;
			}
			if (dic.sp.Follow_F == 0)
			{
				Follow = true;
			}
			else
			{
				Follow = false;
			}
			if (dic.sp.Slow_F == 0)
			{
				Slow = true;
			}
			else
			{
				Slow = false;
			}
			if (ExpPos == 2)
			{
				Body = false;
			}
			else
			{
				Body = true;
			}
			if (dic.sp.RDSpeed_F == 0)
			{
				RandomSpeed = true;
				speedTMP = UnityEngine.Random.Range(dic.sp.Speed1, dic.sp.Speed2);
			}
			else
			{
				RandomSpeed = false;
				if (dic.sp.Speed1 == 0f)
				{
					speedTMP = MoveSpeed * (1f + dic.sp.FlySpeed / 100f);
				}
				else
				{
					speedTMP = dic.sp.Speed1 * (1f + dic.sp.FlySpeed / 100f);
				}
			}
			if (dic.sp.HasFX == 0)
			{
				hasFX = true;
			}
			else
			{
				hasFX = false;
			}
			if (dic.sp.colEXP == 0)
			{
				colEXP = true;
				hasMainDamage = false;
			}
			else
			{
				colEXP = false;
				hasMainDamage = true;
			}
			if (dic.sp.TimeEXP == 0)
			{
				TimeEXP = true;
			}
			else
			{
				TimeEXP = false;
			}
			if (dic.sp.LastEXP == 0)
			{
				LastEXP = true;
			}
			else
			{
				LastEXP = false;
			}
			ExpPos = dic.sp.EXPpos;
			if (dic.sp.AngleEXP == 0)
			{
				AngleEXP = true;
			}
			else
			{
				AngleEXP = false;
			}
			SubApos = dic.sp.EXPpos_AB;
			if (dic.sp.AngleEXP_AB == 0)
			{
				AngleEXP_A = true;
			}
			else
			{
				AngleEXP_A = false;
			}
			if (dic.sp.TimeEXP_AB == 0)
			{
				timeEXP_A = true;
			}
			else
			{
				timeEXP_A = false;
			}
			if (dic.sp.colEXP_A == 0)
			{
				colEXP_A = true;
			}
			else
			{
				colEXP_A = false;
			}
		}
		else
		{
			ZDtimeCount = dic.sp.CountMulti;
			if (dic.sp.Slow_S == 0)
			{
				Slow = true;
			}
			else
			{
				Slow = false;
			}
			if (ExpPos == 2)
			{
				Body = false;
			}
			else
			{
				Body = true;
			}
			if (dic.sp.RDSpeed_S == 0)
			{
				RandomSpeed = true;
				speedTMP = UnityEngine.Random.Range(dic.sp.Speed3, dic.sp.Speed4);
			}
			else
			{
				RandomSpeed = false;
				if (dic.sp.Speed3 == 0f)
				{
					speedTMP = MoveSpeed * (1f + dic.sp.FlySpeed / 100f);
				}
				else
				{
					speedTMP = dic.sp.Speed3 * (1f + dic.sp.FlySpeed / 100f);
				}
			}
			if (dic.sp.AllChuan_S == 0)
			{
				AllChuan = true;
			}
			else
			{
				AllChuan = false;
			}
			if (dic.sp.Follow_S == 0)
			{
				Follow = true;
			}
			else
			{
				Follow = false;
			}
			switch (dic.SubType)
			{
			case 0:
				if (dic.sp.ZD_time_S == 0f)
				{
					LifeTimeTmp = LifeTime;
				}
				else
				{
					LifeTimeTmp = dic.sp.ZD_time_S;
				}
				if (dic.sp.S_HasFX == 0)
				{
					hasFX = true;
				}
				else
				{
					hasFX = false;
				}
				if (dic.sp.S_colEXP == 0)
				{
					colEXP = true;
					hasMainDamage = false;
				}
				else
				{
					colEXP = false;
					hasMainDamage = true;
				}
				TimeEXP = false;
				if (dic.sp.S_LastEXP == 0)
				{
					LastEXP = true;
				}
				else
				{
					LastEXP = false;
				}
				ExpPos = dic.sp.S_EXPpos;
				AngleEXP = false;
				SubApos = dic.sp.EXPpos_AB;
				if (dic.sp.AngleEXP_AB == 0)
				{
					AngleEXP_A = true;
				}
				else
				{
					AngleEXP_A = false;
				}
				if (dic.sp.TimeEXP_AB == 0)
				{
					timeEXP_A = true;
				}
				else
				{
					timeEXP_A = false;
				}
				if (dic.sp.colEXP_A == 0)
				{
					colEXP_A = true;
				}
				else
				{
					colEXP_A = false;
				}
				break;
			case 1:
			case 2:
				if (dic.sp.ZD_time_S == 0f)
				{
					LifeTimeTmp = LifeTime;
				}
				else
				{
					LifeTimeTmp = dic.sp.ZD_time_S;
				}
				if (dic.sp.AB_HasFX == 0)
				{
					hasFX = true;
				}
				else
				{
					hasFX = false;
				}
				if (dic.sp.AB_colEXP == 0)
				{
					colEXP = true;
					hasMainDamage = false;
				}
				else
				{
					colEXP = false;
					hasMainDamage = true;
				}
				TimeEXP = false;
				if (dic.sp.AB_LastEXP == 0)
				{
					LastEXP = true;
				}
				else
				{
					LastEXP = false;
				}
				ExpPos = dic.sp.AB_EXPpos;
				AngleEXP = false;
				break;
			}
		}
		if (AllChuan)
		{
			if (UnityEngine.Random.Range(0, 10) > 4)
			{
				CanFX = true;
			}
			else
			{
				CanFX = false;
			}
		}
		else
		{
			CanFX = true;
		}
		CanEXPA = true;
		CanACT = true;
		MainCOL.enabled = true;
		canDAM = true;
		CanMV = true;
	}

	public void TimeStop()
	{
		if (ReturnToPlayer && !returning && dic != null && dic.sp != null && dic.sp.ZY)
		{
			StartReturn();
			if (returning)
			{
				return;
			}
		}
		try
		{
			if (!_fxBurstDone && FxIsFireBall)
			{
				_fxBurstDone = true;
				FxSpriteFactory.SpawnFlipbookBurst(base.transform.position, FirstParticleMaterial());
			}
		}
		catch
		{
		}
		if (LastEXP)
		{
			Dicform component = ((!AngleEXP) ? LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity) : LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation)).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.UPDamage = dic.UPDamage;
			component.UPDamage = dic.UPDamage;
			component.SubType = dic.SubType;
			component.Index = dic.Index + 1;
		}
		if (LastEXP && dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)SubA)
		{
			Dicform component2 = ((!AngleEXP_A) ? LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation)).GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SetCount(dic.sp.ZY);
			component2.SubType = 1;
			component2.Index = dic.Index + 1;
		}
		if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)SubB)
		{
			for (int i = 0; i < 10; i++)
			{
				Dicform component3 = LeanPool.Spawn(SubB, base.transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f))).GetComponent<Dicform>();
				component3.sp = dic.sp;
				component3.SetCount(dic.sp.ZY);
				component3.SubType = dic.SubType;
				component3.Index = dic.Index + 1;
			}
		}
		if ((bool)FX && !LastEXP && !NoLastFX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		target = null;
		canDAM = false;
		CanMV = false;
		if (par.Length != 0)
		{
			GameObject[] array = par;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].SetActive(value: false);
			}
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array2 = parLoop;
			for (int k = 0; k < array2.Length; k++)
			{
				ParticleSystem.MainModule main = array2[k].main;
				main.loop = false;
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: false);
		}
		if (spr.Length != 0)
		{
			SpriteRenderer[] array3 = spr;
			for (int l = 0; l < array3.Length; l++)
			{
				array3[l].gameObject.SetActive(value: false);
			}
		}
		if (trail.Length != 0)
		{
			TrailRenderer[] array4 = trail;
			for (int m = 0; m < array4.Length; m++)
			{
				array4[m].emitting = false;
			}
		}
		LeanPool.Despawn(base.gameObject, DelDelay);
	}

	private void StartReturn()
	{
		if (dic == null || dic.sp == null || !dic.sp.ZY)
		{
			return;
		}
		Transform transform = null;
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
			if (!(instance == null) && !(instance.yao == null))
			{
				transform = instance.yao.transform;
			}
		}
		if (!(transform == null))
		{
			target = transform;
			returning = true;
			returnClock = 0f;
			em.Clear();
			cp.Clear();
			pl.Clear();
			CanMV = true;
		}
	}

	private void ReturnHit(Collider2D collision)
	{
		try
		{
			if (!canDAM || !collision.CompareTag("BodyCOL") || dic == null || dic.sp == null || !dic.sp.ZY)
			{
				return;
			}
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if ((bool)component && !(component.peo == null) && (bool)component.peo.em && component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS && !em.Contains(component.peo.em))
			{
				em.Add(component.peo.em);
				if (hasMainDamage)
				{
					component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
				}
			}
		}
		catch
		{
		}
	}

	public void Stop()
	{
		if (ReturnToPlayer && !returning && dic != null && dic.sp != null && dic.sp.ZY)
		{
			if (pierceLeft > 0)
			{
				pierceLeft--;
				return;
			}
			StartReturn();
			if (returning)
			{
				return;
			}
		}
		try
		{
			if (!_fxBurstDone && FxIsFireBall)
			{
				_fxBurstDone = true;
				FxSpriteFactory.SpawnFlipbookBurst(base.transform.position, FirstParticleMaterial());
			}
		}
		catch
		{
		}
		target = null;
		canDAM = false;
		CanMV = false;
		if (par.Length != 0)
		{
			GameObject[] array = par;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array2 = parLoop;
			for (int j = 0; j < array2.Length; j++)
			{
				ParticleSystem.MainModule main = array2[j].main;
				main.loop = false;
			}
		}
		if (Arrow != null)
		{
			Arrow.gameObject.SetActive(value: false);
		}
		if (spr.Length != 0)
		{
			SpriteRenderer[] array3 = spr;
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k].gameObject.SetActive(value: false);
			}
		}
		if (trail.Length != 0)
		{
			TrailRenderer[] array4 = trail;
			for (int l = 0; l < array4.Length; l++)
			{
				array4[l].emitting = false;
			}
		}
		LeanPool.Despawn(base.gameObject, DelDelay);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (returning)
		{
			ReturnHit(collision);
			return;
		}
		if (canDAM && collision.CompareTag("BodyCOL"))
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
					if (colEXP && CanFX)
					{
						CanFX = false;
						GameObject gameObject;
						if (AngleEXP)
						{
							gameObject = LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation);
						}
						else
						{
							gameObject = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
							switch (ExpPos)
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
						Dicform component2 = gameObject.GetComponent<Dicform>();
						component2.sp = dic.sp;
						component2.SetCount(dic.sp.ZY);
						component2.UPDamage = dic.UPDamage;
						component2.SubType = dic.SubType;
						component2.Index = dic.Index + 1;
					}
					if (colEXP_A && dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && CanEXPA && (bool)SubA)
					{
						CanEXPA = false;
						GameObject gameObject2;
						if (AngleEXP_A)
						{
							gameObject2 = LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation);
						}
						else
						{
							gameObject2 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity);
							switch (SubApos)
							{
							case 0:
								gameObject2.transform.position = component.peo.em.yao.transform.position;
								gameObject2.transform.SetParent(component.peo.em.yao.transform);
								break;
							case 1:
								gameObject2.transform.SetParent(component.peo.em.yao.transform);
								break;
							case 2:
								gameObject2.transform.position = component.peo.em.transform.position;
								break;
							}
						}
						Dicform component3 = gameObject2.GetComponent<Dicform>();
						component3.sp = dic.sp;
						component3.SetCount(dic.sp.ZY);
						component3.SubType = 1;
						component3.Index = dic.Index + 1;
					}
					if (hasFX && FX != null && CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.em.yao.transform);
					}
					if (!AllChuan)
					{
						switch (dic.sp.ThroughType)
						{
						case 0:
							Stop();
							break;
						case 1:
							if ((float)UnityEngine.Random.Range(0, 101) >= dic.sp.Through)
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
					if (colEXP && CanFX)
					{
						CanFX = false;
						GameObject gameObject3;
						if (AngleEXP)
						{
							gameObject3 = LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation);
						}
						else
						{
							gameObject3 = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
							switch (ExpPos)
							{
							case 0:
								gameObject3.transform.position = component.peo.pl.yao.transform.position;
								gameObject3.transform.SetParent(component.peo.pl.yao.transform);
								break;
							case 1:
								gameObject3.transform.SetParent(component.peo.pl.yao.transform);
								break;
							case 2:
								gameObject3.transform.position = component.peo.pl.transform.position;
								break;
							}
						}
						Dicform component4 = gameObject3.GetComponent<Dicform>();
						component4.sp = dic.sp;
						component4.SetCount(dic.sp.ZY);
						component4.UPDamage = dic.UPDamage;
						component4.SubType = dic.SubType;
						component4.Index = dic.Index + 1;
					}
					if (colEXP_A && dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && CanEXPA && (bool)SubA)
					{
						CanEXPA = false;
						GameObject gameObject4;
						if (AngleEXP_A)
						{
							gameObject4 = LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation);
						}
						else
						{
							gameObject4 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity);
							switch (SubApos)
							{
							case 0:
								gameObject4.transform.position = component.peo.pl.yao.transform.position;
								gameObject4.transform.SetParent(component.peo.pl.yao.transform);
								break;
							case 1:
								gameObject4.transform.SetParent(component.peo.pl.yao.transform);
								break;
							case 2:
								gameObject4.transform.position = component.peo.pl.transform.position;
								break;
							}
						}
						Dicform component5 = gameObject4.GetComponent<Dicform>();
						component5.sp = dic.sp;
						component5.SetCount(dic.sp.ZY);
						component5.SubType = 1;
						component5.Index = dic.Index + 1;
					}
					if (hasFX && FX != null && CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.pl.yao.transform);
					}
					if (!AllChuan)
					{
						switch (dic.sp.ThroughType)
						{
						case 0:
							Stop();
							break;
						case 1:
							if ((float)UnityEngine.Random.Range(0, 101) >= dic.sp.Through)
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
					if (colEXP && CanFX)
					{
						CanFX = false;
						GameObject gameObject5;
						if (AngleEXP)
						{
							gameObject5 = LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation);
						}
						else
						{
							gameObject5 = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
							switch (ExpPos)
							{
							case 0:
								gameObject5.transform.position = component.peo.cp.yao.transform.position;
								gameObject5.transform.SetParent(component.peo.cp.yao.transform);
								break;
							case 1:
								gameObject5.transform.SetParent(component.peo.cp.yao.transform);
								break;
							case 2:
								gameObject5.transform.position = component.peo.cp.transform.position;
								break;
							}
						}
						Dicform component6 = gameObject5.GetComponent<Dicform>();
						component6.sp = dic.sp;
						component6.SetCount(dic.sp.ZY);
						component6.UPDamage = dic.UPDamage;
						component6.SubType = dic.SubType;
						component6.Index = dic.Index + 1;
					}
					if (colEXP_A && dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && CanEXPA && (bool)SubA)
					{
						CanEXPA = false;
						GameObject gameObject6;
						if (AngleEXP_A)
						{
							gameObject6 = LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation);
						}
						else
						{
							gameObject6 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity);
							switch (SubApos)
							{
							case 0:
								gameObject6.transform.position = component.peo.cp.yao.transform.position;
								gameObject6.transform.SetParent(component.peo.cp.yao.transform);
								break;
							case 1:
								gameObject6.transform.SetParent(component.peo.cp.yao.transform);
								break;
							case 2:
								gameObject6.transform.position = component.peo.cp.transform.position;
								break;
							}
						}
						Dicform component7 = gameObject6.GetComponent<Dicform>();
						component7.sp = dic.sp;
						component7.SetCount(dic.sp.ZY);
						component7.SubType = 1;
						component7.Index = dic.Index + 1;
					}
					if (hasFX && FX != null && CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.cp.yao.transform);
					}
					if (!AllChuan)
					{
						switch (dic.sp.ThroughType)
						{
						case 0:
							Stop();
							break;
						case 1:
							if ((float)UnityEngine.Random.Range(0, 101) >= dic.sp.Through)
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
			SK_StromLord component8 = collision.GetComponent<SK_StromLord>();
			if (dic.sp.ZY)
			{
				component8.BuffZD(dic);
			}
			else if (component8.sp.CutSpeedZone > 0 && !dic.CutSpeed)
			{
				speedTMP = speedTMP / 100f * (float)(100 - component8.sp.CutSpeedZone);
				dic.CutSpeed = true;
			}
		}
		if (collision.CompareTag("DoomBall"))
		{
			SK_Doom_Ball component9 = collision.GetComponent<SK_Doom_Ball>();
			if (dic.sp.ZY)
			{
				component9.SetHit(dic, base.transform.right);
			}
			else if (component9.father.sp.TypeDIC_F > 0 && UnityEngine.Random.Range(0, 101) < component9.father.sp.TypeDIC_F)
			{
				TimeStop();
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			if (TimeEXP)
			{
				speedTMP = 0f;
			}
			else
			{
				TimeStop();
			}
		}
	}

	public void SetEXP()
	{
		if (TimeEXP)
		{
			Dicform component = ((!AngleEXP) ? LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity) : LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation)).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.UPDamage = dic.UPDamage;
			component.SubType = dic.SubType;
			component.Index = dic.Index + 1;
		}
		if (timeEXP_A && dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)SubA)
		{
			Dicform component2 = ((!AngleEXP_A) ? LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation)).GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SetCount(dic.sp.ZY);
			component2.SubType = 1;
			component2.Index = dic.Index + 1;
		}
	}

	public void SetZiDan()
	{
		if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && SubB != null)
		{
			for (int i = 0; i < ZDtimeCount; i++)
			{
				Dicform component = LeanPool.Spawn(SubB, base.transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f))).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = 2;
				component.Index = dic.Index + 1;
			}
		}
	}

	public void Refresh()
	{
		if (dic.sp.ZY)
		{
			for (int i = 0; i < em.Count; i++)
			{
				if (!em[i].IsAlive || em[i].IsYS || em[i].IsJump || Vector3.Distance(em[i].transform.position, base.transform.position) > range)
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
			return;
		}
		for (int j = 0; j < cp.Count; j++)
		{
			if (!cp[j].IsAlive || Vector3.Distance(cp[j].transform.position, base.transform.position) > range)
			{
				cp.Remove(cp[j]);
				j--;
			}
		}
		if (Body)
		{
			cp.Sort((Companion t1, Companion t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
		}
		else
		{
			cp.Sort((Companion t1, Companion t2) => Vector3.Distance(t1.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.transform.position, base.transform.position)));
		}
		if (pl.Count > 0 && (!pl[0].IsAlive || Vector3.Distance(pl[0].transform.position, base.transform.position) > range))
		{
			pl.Remove(pl[0]);
		}
	}
}
