using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FlyFollow : MonoBehaviour
{
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

	public float range;

	public float speed;

	[Header("=========")]
	public float starFollowTime;

	public float lerpAngle;

	[Header("=========")]
	public float ExpTimeMin;

	public float ExpTimeMax;

	public GameObject FX;

	public GameObject EXP;

	[HideInInspector]
	public bool hasFX;

	[HideInInspector]
	public bool colEXP;

	[HideInInspector]
	public int ExpPos;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Collider2D MainCOL;

	[HideInInspector]
	public bool hasMainDamage;

	private bool CanMV;

	private bool canFollow;

	private bool startFollow;

	private bool Following;

	private float FXcd;

	private float EXPcd;

	private bool CanFX;

	private bool CanEXP;

	private float ACTcd;

	private bool CanACT;

	private float starFollowTimeTmp;

	private bool canDAM;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeF;

	private float timeG;

	private float timeH;

	private float timeI;

	public List<Enemy> em = new List<Enemy>();

	[HideInInspector]
	public List<Companion> cp = new List<Companion>();

	[HideInInspector]
	public List<PlayerManager> pl = new List<PlayerManager>();

	public Collider2D[] hitEM = new Collider2D[6];

	[HideInInspector]
	public Transform target;

	private float speedTMP;

	private bool initialized;

	[NonSerialized]
	public bool ReturnToPlayer;

	private bool returning;

	private float returnClock;

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
		returnClock = 0f;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeF = 0f;
		timeG = 0f;
		timeH = 0f;
		timeI = 0f;
		starFollowTimeTmp = UnityEngine.Random.Range(starFollowTime * 0.6f, starFollowTime);
		FXcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		EXPcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		ACTcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		CanEXP = false;
		CanACT = false;
		MainCOL.enabled = false;
		canDAM = false;
		CanMV = false;
		startFollow = false;
		canFollow = false;
		Following = false;
		target = null;
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		initialized = false;
		speedTMP = 0f;
	}

	private void Update()
	{
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
			base.transform.position += vector.normalized * Mathf.Max(speedTMP, 2f) * Time.deltaTime;
			if (Vector3.Distance(target.position, base.transform.position) < 0.6f)
			{
				Stop();
			}
			return;
		}
		timeG += Time.deltaTime;
		if (timeG >= 0.2f)
		{
			int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("BodyCOLem"));
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					BodyCOL component = hitEM[i].GetComponent<BodyCOL>();
					if (component != null)
					{
						if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS && em.Count < 6)
						{
							em.Add(component.peo.em);
						}
						hitEM[i] = null;
					}
				}
			}
			Refresh();
			timeG = 0f;
		}
		timeB += Time.deltaTime;
		if (timeB > LifeTimeTmp)
		{
			timeB = 0f;
			Stop();
		}
		if (!startFollow)
		{
			timeA += Time.deltaTime;
			if (timeA > starFollowTimeTmp)
			{
				if (em.Count > 0)
				{
					target = em[0].yao.transform;
				}
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
		if (timeH >= EXPcd)
		{
			EXPcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanEXP = true;
			timeH = 0f;
		}
		timeI += Time.deltaTime;
		if (timeI >= ACTcd)
		{
			ACTcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanACT = true;
			timeI = 0f;
		}
		if (startFollow)
		{
			if (em.Count > 0)
			{
				if (Following)
				{
					if ((bool)target)
					{
						FollowMV();
					}
					else
					{
						SimpleMV();
					}
					return;
				}
				SimpleMV();
				timeC += Time.deltaTime;
				if (timeC >= 0.3f)
				{
					ACTcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
					Following = true;
					timeC = 0f;
				}
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
		base.transform.position += base.transform.right * (speedTMP * 0.6f * Time.deltaTime);
		base.transform.right = Vector3.Slerp(base.transform.right, target.position - base.transform.position, lerpAngle / Vector3.Distance(target.position, base.transform.position));
	}

	public void SimpleMV()
	{
		if (startFollow)
		{
			base.transform.Translate(Vector2.right * (speedTMP * 0.6f * Time.deltaTime));
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
			if (dic.sp.ZD_time_F == 0f)
			{
				LifeTimeTmp = LifeTime;
			}
			else
			{
				LifeTimeTmp = dic.sp.ZD_time_F;
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
			ExpPos = dic.sp.EXPpos;
		}
		else
		{
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
				ExpPos = dic.sp.S_EXPpos;
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
				ExpPos = dic.sp.AB_EXPpos;
				break;
			}
		}
		CanFX = true;
		CanEXP = true;
		CanACT = true;
		MainCOL.enabled = true;
		canDAM = true;
		CanMV = true;
		speedTMP = speed;
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
			StartReturn();
			if (returning)
			{
				return;
			}
		}
		if ((bool)FX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
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
		if ((bool)Arrow)
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
		if (canDAM && collision.CompareTag("BodyCOL") && dic.ZY)
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
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
					GameObject gameObject = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
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
					Dicform component2 = gameObject.GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SubType = dic.SubType;
					component2.Index = dic.Index + 1;
				}
				if (hasFX && FX != null && CanFX)
				{
					CanFX = false;
					LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.em.yao.transform);
				}
				Refresh();
				Following = false;
				if (em.Count > 0)
				{
					target = em[UnityEngine.Random.Range(0, em.Count)].yao.transform;
				}
			}
		}
		if (collision.CompareTag("ZoneSK"))
		{
			SK_StromLord component3 = collision.GetComponent<SK_StromLord>();
			if (dic.sp.ZY)
			{
				component3.BuffZD(dic);
			}
			else if (component3.sp.CutSpeedZone > 0 && !dic.CutSpeed)
			{
				speedTMP = speedTMP / 100f * (float)(100 - component3.sp.CutSpeedZone);
				dic.CutSpeed = true;
			}
		}
		if (collision.CompareTag("DoomBall"))
		{
			SK_Doom_Ball component4 = collision.GetComponent<SK_Doom_Ball>();
			if (dic.sp.ZY)
			{
				component4.SetHit(dic, base.transform.right);
			}
			else if (component4.father.sp.TypeDIC_F > 0 && UnityEngine.Random.Range(0, 101) < component4.father.sp.TypeDIC_F)
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

	public void Refresh()
	{
		for (int i = 0; i < em.Count; i++)
		{
			if (!em[i].IsAlive || em[i].IsYS || em[i].IsJump || Vector3.Distance(em[i].transform.position, base.transform.position) > range)
			{
				em.Remove(em[i]);
				i--;
			}
		}
		em.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
	}
}
