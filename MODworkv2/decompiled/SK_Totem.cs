using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Totem : MonoBehaviour
{
	public string SoundA;

	public ParticleSystem[] parLoop;

	public float size;

	public float FaSheTime;

	[HideInInspector]
	public GameObject qiu;

	[HideInInspector]
	public Dicform dic;

	private float JStimeA;

	private float JStimeB;

	private bool CanAT;

	private bool initialized;

	private void Awake()
	{
		qiu = base.transform.Find("main/qiu").gameObject;
		dic = GetComponent<Dicform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		JStimeA = 0f;
		JStimeB = 0f;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (CanAT)
		{
			JStimeA += Time.deltaTime;
			if (JStimeA >= FaSheTime)
			{
				Fashe();
				JStimeA = 0f;
			}
			JStimeB += Time.deltaTime;
			if (JStimeB >= dic.sp.BuffTime + dic.sp.BuffTime * (float)SingletonMonoScope<PlayerManager>.Instance.TuT_Time / 100f)
			{
				JStimeB = 0f;
				Stop();
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
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		CanAT = true;
		qiu.SetActive(value: true);
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		EmptyCOL_BF component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol_BF, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL_BF>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = dic.sp;
		component2.SetCount(dic.sp.ZY);
		component.size = size;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Fashe()
	{
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			EmptyCOL_BF component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol_BF, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL_BF>();
			Dicform component2 = component.GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SetCount(dic.sp.ZY);
			component.size = size;
		}
	}

	public void Stop()
	{
		CanAT = false;
		qiu.SetActive(value: false);
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		this.wait(1.5f, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
