using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_EXP_Father : MonoBehaviour
{
	public GameObject FX;

	public string SoundA;

	public string SoundB;

	public float SoDelay;

	public string SoundSubA;

	public string SoundSubB;

	public float size;

	public float LifeTime;

	public float DelayTime;

	public float DotMulti;

	public bool Body;

	public bool LowDamage;

	public int MaxATCount;

	[Header("=========")]
	public GameObject SubA;

	public float DelayA;

	public GameObject SubB;

	public float DelayB;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private float timeE;

	private bool SoundOK;

	private bool CanAT;

	private bool CanATA;

	private bool CanATB;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		timeE = 0f;
		SoundOK = false;
		CanAT = false;
		CanATA = false;
		CanATB = false;
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
		CanATA = true;
		CanATB = true;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= LifeTime)
		{
			timeB = 0f;
			LeanPool.Despawn(this);
		}
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= DelayTime)
			{
				Fashe();
				CanAT = false;
				timeA = 0f;
			}
		}
		if (CanATA)
		{
			timeC += Time.deltaTime;
			if (timeC >= DelayA)
			{
				FasheA();
				CanATA = false;
				timeC = 0f;
			}
		}
		if (CanATB)
		{
			timeD += Time.deltaTime;
			if (timeD >= DelayB)
			{
				FasheB();
				CanATB = false;
				timeD = 0f;
			}
		}
		if (SoundOK)
		{
			return;
		}
		timeE += Time.deltaTime;
		if (timeE >= SoDelay)
		{
			if (SoundB != null)
			{
				RuntimeManager.PlayOneShot(SoundB, base.transform.position);
			}
			timeE = 0f;
			SoundOK = true;
		}
	}

	public void Fashe()
	{
		EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = sp;
		component2.SetCount(sp.ZY);
		component2.SubType = 0;
		component2.Index = 0;
		component.size = size;
		component.Body = Body;
		component.DotMulti = DotMulti;
		component.lifeTime = 0.1f;
		component.FX = FX;
		component.IsGround = false;
	}

	public void FasheA()
	{
		if (sp.Layer_SubA == 0 && sp.DamageA > 0f && (bool)SubA)
		{
			Dicform component = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component.sp = sp;
			component.SetCount(sp.ZY);
			component.SubType = 1;
			component.Index = 1;
			if (SoundSubA != null)
			{
				RuntimeManager.PlayOneShot(SoundSubA, base.transform.position);
			}
		}
	}

	public void FasheB()
	{
		if (sp.Layer_SubB == 0 && sp.DamageB > 0f && (bool)SubB)
		{
			Vector3 right = base.transform.right;
			float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
			Dicform component = LeanPool.Spawn(SubB, base.transform.position, Quaternion.Euler(0f, 0f, num + (float)Random.Range(10, -10))).GetComponent<Dicform>();
			component.sp = sp;
			component.SetCount(sp.ZY);
			component.SubType = 2;
			component.Index = 1;
			if (SoundSubB != null)
			{
				RuntimeManager.PlayOneShot(SoundSubB, base.transform.position);
			}
		}
	}
}
