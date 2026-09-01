using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using PoedbMod;
using UnityEngine;

public class SK_FlyA : MonoBehaviour
{
	public SpriteRenderer Arrow;

	public TrailRenderer[] trail;

	public float[] trTime;

	public GameObject[] par;

	[Header("=========")]
	public float LifeTime;

	public float DelDelay;

	public float speed;

	public bool NoFlySpeed;

	private float speedTMP;

	public float DotMulti;

	[HideInInspector]
	public bool hasMainDamage;

	[Header("=========")]
	public float ExpTimeMin;

	public float ExpTimeMax;

	public GameObject FX;

	public GameObject EXP;

	public bool NoLastFX;

	[HideInInspector]
	public bool hasFX;

	[HideInInspector]
	public bool hasEXP;

	[HideInInspector]
	public int ExpPos;

	[HideInInspector]
	public bool AngleEXP;

	[HideInInspector]
	public bool LastEXP;

	[Header("=========")]
	public GameObject SubA;

	[HideInInspector]
	public int SubApos;

	[HideInInspector]
	public bool AngleEXP_A;

	[HideInInspector]
	public bool LastEXP_A;

	public GameObject SubB;

	[HideInInspector]
	public int SubBpos;

	[HideInInspector]
	public bool AngleEXP_B;

	[HideInInspector]
	public bool LastEXP_B;

	[HideInInspector]
	public bool AllChuan;

	[HideInInspector]
	public Collider2D MainCOL;

	[HideInInspector]
	public Dicform dic;

	private float FXcd;

	private float EXPAcd;

	private float EXPBcd;

	private bool CanFX;

	private bool CanEXPA;

	private bool CanEXPB;

	private float ACTcd;

	private bool CanACT;

	private bool CanMove;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private float timeE;

	private float timeF;

	private float range;

	private bool startFollow;

	[HideInInspector]
	public bool Follow;

	public float lerpAngle;

	public List<Enemy> em = new List<Enemy>();

	public Collider2D[] hitEM = new Collider2D[4];

	[HideInInspector]
	public Transform target;

	[NonSerialized]
	public bool ReturnToPlayer;

	private bool returning;

	private int pierceLeft;

	private bool initialized;

	private static readonly Color CoreTint = new Color(1f, 0.33f, 0.3f, 1f);

	private static readonly Color MainTint = new Color(1f, 0.18f, 0.16f, 1f);

	private static readonly Color DeepTint = new Color(0.72f, 0.04f, 0.1f, 1f);

	[NonSerialized]
	private GameObject _fxIceTrail;

	[NonSerialized]
	private bool _fxBurstDone;

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
		em.Clear();
		startFollow = false;
		returning = false;
		pierceLeft = 0;
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		range = 4f;
		lerpAngle = 0.3f;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		timeE = 0f;
		timeF = 0f;
		CanMove = false;
		MainCOL.enabled = false;
		CanFX = false;
		CanEXPA = false;
		CanEXPB = false;
		CanACT = false;
		speedTMP = 0f;
		FXcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		EXPAcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		EXPBcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
		initialized = false;
		dic.CutSpeed = false;
	}

	public void FollowMV()
	{
		base.transform.position += base.transform.right * (speedTMP * Time.deltaTime);
		base.transform.right = Vector3.Slerp(base.transform.right, target.position - base.transform.position, lerpAngle / Vector3.Distance(target.position, base.transform.position));
	}

	public void SimpleMV()
	{
		base.transform.Translate(Vector2.right * (speedTMP * (1f + dic.sp.FlySpeed / 100f) * Time.deltaTime));
	}

	private void Update()
	{
		if (!CanMove)
		{
			return;
		}
		if (returning)
		{
			if (target == null)
			{
				Stop();
				return;
			}
			StraightReturnMV();
			if (Vector3.Distance(target.position, base.transform.position) < 0.6f)
			{
				Stop();
			}
			return;
		}
		if (dic.ChangeFL)
		{
			if (em.Count > 0)
			{
				target = em[0].yao.transform;
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
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			TimeStop();
		}
		timeB += Time.deltaTime;
		if (timeB >= FXcd)
		{
			FXcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanFX = true;
			timeB = 0f;
		}
		timeC += Time.deltaTime;
		if (timeC >= EXPAcd)
		{
			EXPAcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanEXPA = true;
			timeC = 0f;
		}
		timeD += Time.deltaTime;
		if (timeD >= EXPBcd)
		{
			EXPBcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanEXPB = true;
			timeD = 0f;
		}
		timeE += Time.deltaTime;
		if (timeE >= ACTcd)
		{
			ACTcd = UnityEngine.Random.Range(ExpTimeMin, ExpTimeMax);
			CanACT = true;
			timeE = 0f;
		}
		timeF += Time.deltaTime;
		if (!(timeF >= 0.15f))
		{
			return;
		}
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
		Refresh();
		timeF = 0f;
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
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: true);
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: true);
		}
		if (trail.Length != 0)
		{
			for (int j = 0; j < trail.Length; j++)
			{
				trail[j].emitting = true;
				trail[j].time = trTime[j];
			}
		}
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
			if (dic.sp.RDSpeed_F == 0)
			{
				speedTMP = UnityEngine.Random.Range(dic.sp.Speed1, dic.sp.Speed2);
			}
			else if (dic.sp.Speed1 == 0f)
			{
				if (NoFlySpeed)
				{
					speedTMP = speed;
				}
				else
				{
					speedTMP = speed * (1f + dic.sp.FlySpeed / 100f);
				}
			}
			else if (NoFlySpeed)
			{
				speedTMP = dic.sp.Speed1;
			}
			else
			{
				speedTMP = dic.sp.Speed1 * (1f + dic.sp.FlySpeed / 100f);
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
				hasEXP = true;
				hasMainDamage = false;
			}
			else
			{
				hasEXP = false;
				hasMainDamage = true;
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
			SubBpos = dic.sp.EXPpos_AB;
			if (dic.sp.AngleEXP_AB == 0)
			{
				AngleEXP_A = true;
				AngleEXP_B = true;
			}
			else
			{
				AngleEXP_A = false;
				AngleEXP_B = false;
			}
			if (dic.sp.LastEXP_AB == 0)
			{
				LastEXP_A = true;
				LastEXP_B = true;
			}
			else
			{
				LastEXP_A = false;
				LastEXP_B = false;
			}
		}
		else
		{
			if (dic.sp.RDSpeed_S == 0)
			{
				speedTMP = UnityEngine.Random.Range(dic.sp.Speed3, dic.sp.Speed4);
			}
			else if (dic.sp.Speed3 == 0f)
			{
				if (NoFlySpeed)
				{
					speedTMP = speed;
				}
				else
				{
					speedTMP = speed * (1f + dic.sp.FlySpeed / 100f);
				}
			}
			else if (NoFlySpeed)
			{
				speedTMP = dic.sp.Speed3;
			}
			else
			{
				speedTMP = dic.sp.Speed3 * (1f + dic.sp.FlySpeed / 100f);
			}
			if (dic.sp.AllChuan_S == 0)
			{
				AllChuan = true;
			}
			else
			{
				AllChuan = false;
			}
			switch (dic.SubType)
			{
			case 0:
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
					hasEXP = true;
					hasMainDamage = false;
				}
				else
				{
					hasEXP = false;
					hasMainDamage = true;
				}
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
				SubBpos = dic.sp.EXPpos_AB;
				if (dic.sp.AngleEXP_AB == 0)
				{
					AngleEXP_A = true;
					AngleEXP_B = true;
				}
				else
				{
					AngleEXP_A = false;
					AngleEXP_B = false;
				}
				if (dic.sp.LastEXP_AB == 0)
				{
					LastEXP_A = true;
					LastEXP_B = true;
				}
				else
				{
					LastEXP_A = false;
					LastEXP_B = false;
				}
				break;
			case 1:
			case 2:
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
					hasEXP = true;
					hasMainDamage = false;
				}
				else
				{
					hasEXP = false;
					hasMainDamage = true;
				}
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
		}
		else
		{
			CanFX = true;
		}
		CanEXPA = true;
		CanEXPB = true;
		CanACT = true;
		CanMove = true;
		MainCOL.enabled = true;
		try
		{
			ReturnToPlayer = dic != null && dic.sp != null && (dic.sp.skillName == "Ice Crystal" || Registry.IsBoomerangSkill(dic.sp.skillName) || PoeItemMod.ReturnEquipped);
			if (dic != null && dic.sp != null && dic.sp.skillName == "Ice Crystal")
			{
				ApplyShurikenStyle();
			}
		}
		catch
		{
			ReturnToPlayer = dic != null && dic.sp != null && (dic.sp.skillName == "Ice Crystal" || PoeItemMod.ReturnEquipped);
		}
		pierceLeft = (ReturnToPlayer ? 1 : 0);
	}

	private void ApplyShurikenStyle()
	{
		try
		{
			_fxBurstDone = false;
			if ((bool)Arrow)
			{
				Arrow.color = CoreTint;
			}
			if (trail != null)
			{
				for (int i = 0; i < trail.Length; i++)
				{
					TrailRenderer trailRenderer = trail[i];
					if (trailRenderer == null)
					{
						continue;
					}
					trailRenderer.startColor = CoreTint;
					trailRenderer.endColor = new Color(DeepTint.r, DeepTint.g, DeepTint.b, 0f);
					try
					{
						Gradient gradient = new Gradient();
						gradient.SetKeys(new GradientColorKey[3]
						{
							new GradientColorKey(CoreTint, 0f),
							new GradientColorKey(MainTint, 0.5f),
							new GradientColorKey(DeepTint, 1f)
						}, new GradientAlphaKey[3]
						{
							new GradientAlphaKey(0.95f, 0f),
							new GradientAlphaKey(0.55f, 0.5f),
							new GradientAlphaKey(0f, 1f)
						});
						trailRenderer.colorGradient = gradient;
					}
					catch
					{
					}
					if (trTime != null && i < trTime.Length)
					{
						float num = trTime[i] + 0.06f;
						if (num > 0.3f)
						{
							num = 0.3f;
						}
						trailRenderer.time = num;
					}
				}
			}
			AttachShardTrail();
		}
		catch
		{
		}
	}

	private void AttachShardTrail()
	{
		try
		{
			if (_fxIceTrail != null)
			{
				return;
			}
			Material material = FxSpriteFactory.MakeParticleMaterial(FxSpriteFactory.ShardTex, MainTint, FirstParticleMaterial());
			if (!(material == null))
			{
				GameObject gameObject = new GameObject("FxIceShardTrail");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				ParticleSystem particleSystem = gameObject.AddComponent<ParticleSystem>();
				ParticleSystem.MainModule main = particleSystem.main;
				main.loop = true;
				main.playOnAwake = true;
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				main.startLifetime = 0.35f;
				main.startSpeed = 0f;
				main.startSize = 0.16f;
				main.startRotation = new ParticleSystem.MinMaxCurve(0f, (float)Math.PI * 2f);
				main.maxParticles = 24;
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				emission.rateOverTime = 8f;
				try
				{
					ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
					colorOverLifetime.enabled = true;
					Gradient gradient = new Gradient();
					gradient.SetKeys(new GradientColorKey[2]
					{
						new GradientColorKey(CoreTint, 0f),
						new GradientColorKey(DeepTint, 1f)
					}, new GradientAlphaKey[2]
					{
						new GradientAlphaKey(0.9f, 0f),
						new GradientAlphaKey(0f, 1f)
					});
					colorOverLifetime.color = gradient;
				}
				catch
				{
				}
				ParticleSystemRenderer component = particleSystem.GetComponent<ParticleSystemRenderer>();
				if (component != null)
				{
					component.material = material;
					component.sortingOrder = 5;
				}
				particleSystem.Play();
				_fxIceTrail = gameObject;
			}
		}
		catch
		{
		}
	}

	private Material FirstParticleMaterial()
	{
		try
		{
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

	private void CleanupIceFx()
	{
		try
		{
			if (_fxIceTrail != null)
			{
				UnityEngine.Object.Destroy(_fxIceTrail);
				_fxIceTrail = null;
			}
		}
		catch
		{
		}
	}

	public void StraightReturnMV()
	{
		Vector3 vector = target.position - base.transform.position;
		vector.Normalize();
		base.transform.right = vector;
		base.transform.position += vector * speedTMP * (1f + dic.sp.FlySpeed / 100f) * Time.deltaTime;
	}

	private void StartReturn()
	{
		try
		{
			if (!_fxBurstDone && dic != null && dic.sp != null && dic.sp.skillName == "Ice Crystal")
			{
				_fxBurstDone = true;
				FxSpriteFactory.SpawnIceBurst(base.transform.position, CoreTint, MainTint, DeepTint, FirstParticleMaterial());
			}
			CleanupIceFx();
		}
		catch
		{
		}
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
			em.Clear();
			CanMove = true;
		}
	}

	private bool ChainPierceOrReturn()
	{
		if (!ReturnToPlayer || returning || dic == null || dic.sp == null || !dic.sp.ZY)
		{
			return false;
		}
		if (pierceLeft > 0)
		{
			pierceLeft--;
			return true;
		}
		StartReturn();
		return returning;
	}

	private void ReturnHit(Collider2D collision)
	{
		try
		{
			if (!collision.CompareTag("BodyCOL") || dic == null || dic.sp == null || !dic.sp.ZY)
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
		if (LastEXP)
		{
			Dicform component = ((!AngleEXP) ? LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity) : LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation)).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.UPDamage = dic.UPDamage;
			component.SubType = dic.SubType;
			component.Index = dic.Index + 1;
		}
		if (LastEXP_A && dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)SubA)
		{
			Dicform component2 = ((!AngleEXP_A) ? LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation)).GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SetCount(dic.sp.ZY);
			component2.SubType = 1;
			component2.Index = dic.Index + 1;
		}
		if (LastEXP_B && dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)SubB)
		{
			Dicform component3 = ((!AngleEXP_B) ? LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubB, base.transform.position, base.transform.rotation)).GetComponent<Dicform>();
			component3.sp = dic.sp;
			component3.SetCount(dic.sp.ZY);
			component3.SubType = 2;
			component3.Index = dic.Index + 1;
		}
		if ((bool)FX && !LastEXP && !NoLastFX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		CanMove = false;
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: false);
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: false);
		}
		if (trail.Length != 0)
		{
			for (int j = 0; j < trail.Length; j++)
			{
				trail[j].emitting = false;
			}
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void Stop()
	{
		CleanupIceFx();
		CanMove = false;
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: false);
			}
		}
		if ((bool)Arrow)
		{
			Arrow.gameObject.SetActive(value: false);
		}
		if (trail.Length != 0)
		{
			for (int j = 0; j < trail.Length; j++)
			{
				trail[j].emitting = false;
			}
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!CanMove)
		{
			return;
		}
		if (returning)
		{
			ReturnHit(collision);
			return;
		}
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
					if (dic.Index == 0 && CanACT)
					{
						SingletonMonoScope<ACTbar>.Instance.CreatACT_Hit(dic.sp.skillName, component.peo.em, base.transform.right);
						CanACT = false;
					}
					if (hasEXP && CanFX)
					{
						CanFX = false;
						GameObject gameObject;
						if (AngleEXP)
						{
							gameObject = LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation, component.peo.em.yao.transform);
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
						component2.dic = base.transform.right;
					}
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && CanEXPA && (bool)SubA)
					{
						CanEXPA = false;
						GameObject gameObject2 = ((!AngleEXP_A) ? LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation));
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
						Dicform component3 = gameObject2.GetComponent<Dicform>();
						component3.sp = dic.sp;
						component3.SetCount(dic.sp.ZY);
						component3.SubType = 1;
						component3.Index = dic.Index + 1;
						component3.dic = base.transform.right;
					}
					if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && CanEXPB && (bool)SubB)
					{
						CanEXPB = false;
						GameObject gameObject3 = ((!AngleEXP_B) ? LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubB, base.transform.position, base.transform.rotation));
						switch (SubBpos)
						{
						case 0:
							gameObject3.transform.position = component.peo.em.yao.transform.position;
							gameObject3.transform.SetParent(component.peo.em.yao.transform);
							break;
						case 1:
							gameObject3.transform.SetParent(component.peo.em.yao.transform);
							break;
						case 2:
							gameObject3.transform.position = component.peo.em.transform.position;
							break;
						}
						Dicform component4 = gameObject3.GetComponent<Dicform>();
						component4.sp = dic.sp;
						component4.SetCount(dic.sp.ZY);
						component4.SubType = 2;
						component4.Index = dic.Index + 1;
						component4.dic = base.transform.right;
					}
					if (hasFX && FX != null && CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.em.yao.transform);
					}
					if (!AllChuan)
					{
						if (ChainPierceOrReturn())
						{
							return;
						}
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
					if (hasEXP && CanFX)
					{
						CanFX = false;
						GameObject gameObject4;
						if (AngleEXP)
						{
							gameObject4 = LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation, component.peo.pl.yao.transform);
						}
						else
						{
							gameObject4 = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
							switch (ExpPos)
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
						component5.UPDamage = dic.UPDamage;
						component5.SubType = dic.SubType;
						component5.Index = dic.Index + 1;
						component5.dic = base.transform.right;
					}
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && CanEXPA && (bool)SubA)
					{
						CanEXPA = false;
						GameObject gameObject5 = ((!AngleEXP_A) ? LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation));
						switch (SubApos)
						{
						case 0:
							gameObject5.transform.position = component.peo.pl.yao.transform.position;
							gameObject5.transform.SetParent(component.peo.pl.yao.transform);
							break;
						case 1:
							gameObject5.transform.SetParent(component.peo.pl.yao.transform);
							break;
						case 2:
							gameObject5.transform.position = component.peo.pl.transform.position;
							break;
						}
						Dicform component6 = gameObject5.GetComponent<Dicform>();
						component6.sp = dic.sp;
						component6.SetCount(dic.sp.ZY);
						component6.SubType = 1;
						component6.Index = dic.Index + 1;
						component6.dic = base.transform.right;
					}
					if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && CanEXPB && (bool)SubB)
					{
						CanEXPB = false;
						GameObject gameObject6 = ((!AngleEXP_B) ? LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubB, base.transform.position, base.transform.rotation));
						switch (SubBpos)
						{
						case 0:
							gameObject6.transform.position = component.peo.pl.yao.transform.position;
							gameObject6.transform.SetParent(component.peo.pl.yao.transform);
							break;
						case 1:
							gameObject6.transform.SetParent(component.peo.pl.yao.transform);
							break;
						case 2:
							gameObject6.transform.position = component.peo.pl.transform.position;
							break;
						}
						Dicform component7 = gameObject6.GetComponent<Dicform>();
						component7.sp = dic.sp;
						component7.SetCount(dic.sp.ZY);
						component7.SubType = 2;
						component7.Index = dic.Index + 1;
						component7.dic = base.transform.right;
					}
					if (hasFX && (bool)FX && CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.pl.yao.transform);
					}
					if (!AllChuan)
					{
						if (ChainPierceOrReturn())
						{
							return;
						}
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
					if (hasEXP && CanFX)
					{
						CanFX = false;
						GameObject gameObject7;
						if (AngleEXP)
						{
							gameObject7 = LeanPool.Spawn(EXP, base.transform.position, base.transform.rotation, component.peo.cp.yao.transform);
						}
						else
						{
							gameObject7 = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
							switch (ExpPos)
							{
							case 0:
								gameObject7.transform.position = component.peo.cp.yao.transform.position;
								gameObject7.transform.SetParent(component.peo.cp.yao.transform);
								break;
							case 1:
								gameObject7.transform.SetParent(component.peo.cp.yao.transform);
								break;
							case 2:
								gameObject7.transform.position = component.peo.cp.transform.position;
								break;
							}
						}
						Dicform component8 = gameObject7.GetComponent<Dicform>();
						component8.sp = dic.sp;
						component8.SetCount(dic.sp.ZY);
						component8.UPDamage = dic.UPDamage;
						component8.SubType = dic.SubType;
						component8.Index = dic.Index + 1;
						component8.dic = base.transform.right;
					}
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && CanEXPA && (bool)SubA)
					{
						CanEXPA = false;
						GameObject gameObject8 = ((!AngleEXP_A) ? LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubA, base.transform.position, base.transform.rotation));
						switch (SubApos)
						{
						case 0:
							gameObject8.transform.position = component.peo.cp.yao.transform.position;
							gameObject8.transform.SetParent(component.peo.cp.yao.transform);
							break;
						case 1:
							gameObject8.transform.SetParent(component.peo.cp.yao.transform);
							break;
						case 2:
							gameObject8.transform.position = component.peo.cp.transform.position;
							break;
						}
						Dicform component9 = gameObject8.GetComponent<Dicform>();
						component9.sp = dic.sp;
						component9.SetCount(dic.sp.ZY);
						component9.SubType = 1;
						component9.Index = dic.Index + 1;
						component9.dic = base.transform.right;
					}
					if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && CanEXPB && (bool)SubB)
					{
						CanEXPB = false;
						GameObject gameObject9 = ((!AngleEXP_B) ? LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubB, base.transform.position, base.transform.rotation));
						switch (SubBpos)
						{
						case 0:
							gameObject9.transform.position = component.peo.cp.yao.transform.position;
							gameObject9.transform.SetParent(component.peo.cp.yao.transform);
							break;
						case 1:
							gameObject9.transform.SetParent(component.peo.cp.yao.transform);
							break;
						case 2:
							gameObject9.transform.position = component.peo.cp.transform.position;
							break;
						}
						Dicform component10 = gameObject9.GetComponent<Dicform>();
						component10.sp = dic.sp;
						component10.SetCount(dic.sp.ZY);
						component10.SubType = 2;
						component10.Index = dic.Index + 1;
						component10.dic = base.transform.right;
					}
					if (hasFX && FX != null && CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.cp.yao.transform);
					}
					if (!AllChuan)
					{
						if (ChainPierceOrReturn())
						{
							return;
						}
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
			SK_StromLord component11 = collision.GetComponent<SK_StromLord>();
			if (dic.sp.ZY)
			{
				component11.BuffZD(dic);
			}
			else if (component11.sp.CutSpeedZone > 0 && !dic.CutSpeed)
			{
				speedTMP = speedTMP / 100f * (float)(100 - component11.sp.CutSpeedZone);
				dic.CutSpeed = true;
			}
		}
		if (collision.CompareTag("DoomBall"))
		{
			SK_Doom_Ball component12 = collision.GetComponent<SK_Doom_Ball>();
			if (dic.sp.ZY)
			{
				component12.SetHit(dic, base.transform.right);
			}
			else if (component12.father.sp.TypeDIC_F > 0 && UnityEngine.Random.Range(0, 101) < component12.father.sp.TypeDIC_F)
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
			TimeStop();
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
