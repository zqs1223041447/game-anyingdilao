using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Chong : MonoBehaviour
{
	public string[] SoundA;

	public GameObject OBJ;

	public GameObject FX;

	public GameObject SubA;

	public GameObject SubB;

	public int SubApos;

	public int SubBpos;

	public int SubAType;

	public int SubBType;

	public ParticleSystem[] parLoop;

	public float DelDelay;

	public float DotMulti;

	private bool CanAT;

	[HideInInspector]
	public CircleCollider2D col;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		col = GetComponent<CircleCollider2D>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		CanAT = false;
		col.enabled = false;
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
		col.radius = sp.Size;
		col.enabled = true;
		CanAT = true;
		if (OBJ != null)
		{
			switch (sp.indexType)
			{
			case 0:
				LeanPool.Spawn(OBJ, PL.transform.position, Quaternion.identity, PL.transform);
				break;
			case 1:
				LeanPool.Spawn(OBJ, sp.cp.transform.position, Quaternion.identity, sp.cp.transform);
				break;
			case 2:
				LeanPool.Spawn(OBJ, sp.em.transform.position, Quaternion.identity, sp.em.transform);
				break;
			}
		}
		if (sp.Layer_SubA == 0 && SubAType == 0 && sp.DamageA > 0f && SubA != null)
		{
			switch (sp.indexType)
			{
			case 0:
				switch (SubApos)
				{
				case 0:
				{
					Dicform component6 = LeanPool.Spawn(SubA, PL.transform.position, Quaternion.identity, PL.transform).GetComponent<Dicform>();
					component6.sp = sp;
					component6.SetCount(sp.ZY);
					component6.SubType = 1;
					component6.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component5 = LeanPool.Spawn(SubA, PL.yao.transform.position, Quaternion.identity, PL.yao.transform).GetComponent<Dicform>();
					component5.sp = sp;
					component5.SubType = 1;
					component5.Index = 1;
					break;
				}
				}
				break;
			case 1:
				switch (SubApos)
				{
				case 0:
				{
					Dicform component4 = LeanPool.Spawn(SubA, sp.cp.transform.position, Quaternion.identity, sp.cp.transform).GetComponent<Dicform>();
					component4.sp = sp;
					component4.SetCount(sp.ZY);
					component4.SubType = 1;
					component4.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component3 = LeanPool.Spawn(SubA, sp.cp.yao.transform.position, Quaternion.identity, sp.cp.yao.transform).GetComponent<Dicform>();
					component3.sp = sp;
					component3.SetCount(sp.ZY);
					component3.SubType = 1;
					component3.Index = 1;
					break;
				}
				}
				break;
			case 2:
				switch (SubApos)
				{
				case 0:
				{
					Dicform component2 = LeanPool.Spawn(SubA, sp.em.transform.position, Quaternion.identity, sp.em.transform).GetComponent<Dicform>();
					component2.sp = sp;
					component2.SetCount(sp.ZY);
					component2.SubType = 1;
					component2.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component = LeanPool.Spawn(SubA, sp.em.transform.position, Quaternion.identity, sp.em.yao.transform).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 1;
					component.Index = 1;
					break;
				}
				}
				break;
			}
		}
		if (sp.Layer_SubB == 0 && SubBType == 0 && sp.DamageB > 0f && SubB != null)
		{
			switch (sp.indexType)
			{
			case 0:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component12 = LeanPool.Spawn(SubB, PL.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component12.sp = sp;
					component12.SetCount(sp.ZY);
					component12.SubType = 2;
					component12.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component11 = LeanPool.Spawn(SubB, PL.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component11.sp = sp;
					component11.SetCount(sp.ZY);
					component11.SubType = 2;
					component11.Index = 1;
					break;
				}
				}
				break;
			case 1:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component10 = LeanPool.Spawn(SubB, sp.cp.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component10.sp = sp;
					component10.SetCount(sp.ZY);
					component10.SubType = 2;
					component10.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component9 = LeanPool.Spawn(SubB, sp.cp.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component9.sp = sp;
					component9.SetCount(sp.ZY);
					component9.SubType = 2;
					component9.Index = 1;
					break;
				}
				}
				break;
			case 2:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component8 = LeanPool.Spawn(SubB, sp.em.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component8.sp = sp;
					component8.SetCount(sp.ZY);
					component8.SubType = 2;
					component8.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component7 = LeanPool.Spawn(SubB, sp.em.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component7.sp = sp;
					component7.SetCount(sp.ZY);
					component7.SubType = 2;
					component7.Index = 1;
					break;
				}
				}
				break;
			}
		}
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		if (SoundA[sp.Sound] != null)
		{
			RuntimeManager.PlayOneShot(SoundA[sp.Sound], base.transform.position);
		}
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		switch (sp.indexType)
		{
		case 0:
			if (!PL.IsChong)
			{
				Stop();
			}
			break;
		case 1:
			if (!sp.cp.IsChong)
			{
				Stop();
			}
			break;
		case 2:
			if (!sp.em.IsChong)
			{
				Stop();
			}
			break;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!CanAT || !collision.CompareTag("BodyCOL"))
		{
			return;
		}
		BodyCOL component = collision.GetComponent<BodyCOL>();
		if (sp.ZY)
		{
			if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
			{
				component.peo.EM_Set(sp, DotMulti, 0, Dot_Infect: false, 0, 0f);
			}
			return;
		}
		if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
		{
			component.peo.PL_Set(sp, 0);
		}
		if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
		{
			component.peo.CP_Set(sp, 0);
		}
	}

	public void Stop()
	{
		CanAT = false;
		if (FX != null)
		{
			switch (sp.indexType)
			{
			case 0:
				LeanPool.Spawn(FX, PL.yao.transform.position, Quaternion.identity);
				break;
			case 1:
				LeanPool.Spawn(FX, sp.cp.yao.transform.position, Quaternion.identity);
				break;
			case 2:
				LeanPool.Spawn(FX, sp.em.yao.transform.position, Quaternion.identity);
				break;
			}
		}
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		if (sp.Layer_SubA == 0 && SubAType == 1 && sp.DamageA > 0f && SubA != null)
		{
			switch (sp.indexType)
			{
			case 0:
				switch (SubApos)
				{
				case 0:
				{
					Dicform component6 = LeanPool.Spawn(SubA, PL.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component6.sp = sp;
					component6.SetCount(sp.ZY);
					component6.SubType = 1;
					component6.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component5 = LeanPool.Spawn(SubA, PL.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component5.sp = sp;
					component5.SetCount(sp.ZY);
					component5.SubType = 1;
					component5.Index = 1;
					break;
				}
				}
				break;
			case 1:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component4 = LeanPool.Spawn(SubA, sp.cp.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component4.sp = sp;
					component4.SetCount(sp.ZY);
					component4.SubType = 1;
					component4.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component3 = LeanPool.Spawn(SubA, sp.cp.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component3.sp = sp;
					component3.SetCount(sp.ZY);
					component3.SubType = 1;
					component3.Index = 1;
					break;
				}
				}
				break;
			case 2:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component2 = LeanPool.Spawn(SubA, sp.em.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component2.sp = sp;
					component2.SetCount(sp.ZY);
					component2.SubType = 1;
					component2.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component = LeanPool.Spawn(SubA, sp.em.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 1;
					component.Index = 1;
					break;
				}
				}
				break;
			}
		}
		if (sp.Layer_SubB == 0 && SubBType == 1 && sp.DamageB > 0f && SubB != null)
		{
			switch (sp.indexType)
			{
			case 0:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component12 = LeanPool.Spawn(SubB, PL.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component12.sp = sp;
					component12.SetCount(sp.ZY);
					component12.SubType = 2;
					component12.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component11 = LeanPool.Spawn(SubB, PL.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component11.sp = sp;
					component11.SetCount(sp.ZY);
					component11.SubType = 2;
					component11.Index = 1;
					break;
				}
				}
				break;
			case 1:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component10 = LeanPool.Spawn(SubB, sp.cp.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component10.sp = sp;
					component10.SetCount(sp.ZY);
					component10.SubType = 2;
					component10.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component9 = LeanPool.Spawn(SubB, sp.cp.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component9.sp = sp;
					component9.SetCount(sp.ZY);
					component9.SubType = 2;
					component9.Index = 1;
					break;
				}
				}
				break;
			case 2:
				switch (SubBpos)
				{
				case 0:
				{
					Dicform component8 = LeanPool.Spawn(SubB, sp.em.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component8.sp = sp;
					component8.SetCount(sp.ZY);
					component8.SubType = 2;
					component8.Index = 1;
					break;
				}
				case 1:
				{
					Dicform component7 = LeanPool.Spawn(SubB, sp.em.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component7.sp = sp;
					component7.SetCount(sp.ZY);
					component7.SubType = 2;
					component7.Index = 1;
					break;
				}
				}
				break;
			}
		}
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
