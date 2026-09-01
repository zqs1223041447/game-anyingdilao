using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using UnityEngine;

public class SK_EXP_son : MonoBehaviour
{
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

	[Header("=========")]
	public float DelayA;

	public float DelayB;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	[HideInInspector]
	public Dicform dic;

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
		dic = GetComponent<Dicform>();
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
		if (!string.IsNullOrEmpty(SoundA))
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
		timeB += Time.deltaTime;
		if (timeB >= LifeTime)
		{
			timeB = 0f;
			LeanPool.Despawn(this);
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
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !dic || !dic.sp)
		{
			return;
		}
		if (!SingletonMonoScope<GameDataManager>.Instance.SKPB || !SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol)
		{
			LogUtil.Warn("SKPB.EmptyCol 为空，无法生成 EmptyCOL。");
			return;
		}
		GameObject gameObject = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity);
		if (!gameObject)
		{
			return;
		}
		EmptyCOL component = gameObject.GetComponent<EmptyCOL>();
		if (!component)
		{
			LogUtil.Warn("EmptyCol 预制体上未找到 EmptyCOL 组件。", gameObject);
			return;
		}
		Dicform component2 = gameObject.GetComponent<Dicform>();
		if (!component2)
		{
			LogUtil.Warn("EmptyCol 预制体上未找到 Dicform 组件。", gameObject);
			return;
		}
		component2.sp = dic.sp;
		component2.SetCount(dic.sp.ZY);
		component2.UPDamage = dic.UPDamage;
		component2.SubType = dic.SubType;
		component2.Index = dic.Index;
		component.size = size;
		component.Body = Body;
		component.DotMulti = DotMulti;
		component.lifeTime = 0.1f;
		component.IsGround = false;
	}

	public void FasheA()
	{
		if (!dic || !dic.sp || !SubA || dic.SubType != 0 || dic.sp.Layer_SubA != dic.Index || dic.sp.DamageA <= 0f)
		{
			return;
		}
		GameObject gameObject = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity);
		if (!gameObject)
		{
			return;
		}
		Dicform component = gameObject.GetComponent<Dicform>();
		if (!component)
		{
			LogUtil.Warn("SubA 预制体 " + SubA.name + " 上未找到 Dicform 组件。", SubA);
			return;
		}
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.UPDamage = dic.UPDamage;
		component.SubType = 1;
		component.Index = dic.Index + 1;
		if (!string.IsNullOrEmpty(SoundSubA))
		{
			RuntimeManager.PlayOneShot(SoundSubA, base.transform.position);
		}
	}

	public void FasheB()
	{
		if (!dic || !dic.sp || !SubB || dic.SubType != 0 || dic.sp.Layer_SubB != dic.Index || dic.sp.DamageB <= 0f)
		{
			return;
		}
		GameObject gameObject = LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity);
		if (!gameObject)
		{
			return;
		}
		Dicform component = gameObject.GetComponent<Dicform>();
		if (!component)
		{
			LogUtil.Warn("SubB 预制体 " + SubB.name + " 上未找到 Dicform 组件。", SubB);
			return;
		}
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.UPDamage = dic.UPDamage;
		component.SubType = 2;
		component.Index = dic.Index + 1;
		if (!string.IsNullOrEmpty(SoundSubB))
		{
			RuntimeManager.PlayOneShot(SoundSubB, base.transform.position);
		}
	}
}
