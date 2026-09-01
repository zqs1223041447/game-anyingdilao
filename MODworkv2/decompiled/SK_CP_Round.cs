using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_CP_Round : MonoBehaviour
{
	public string SoundA;

	public Transform[] trans;

	public GameObject FX;

	[Header("=========")]
	public float angle;

	public float DotMulti;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public Transform core;

	private bool canAT;

	private float timeA;

	private float timeO;

	private PlayerManager PL;

	private Companion CP;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		core = base.transform.Find("core");
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeO = 0f;
		canAT = false;
		initialized = false;
	}

	private void Update()
	{
		if ((bool)core)
		{
			core.Rotate(new Vector3(0f, 0f, 1f), angle * Time.deltaTime);
		}
		if (!canAT)
		{
			return;
		}
		if (!CP || !CP.IsAlive)
		{
			Stop();
			return;
		}
		if (sp.NoTime == 1)
		{
			timeA += Time.deltaTime;
			if (timeA > sp.BuffTime)
			{
				timeA = 0f;
				Stop();
				return;
			}
			if ((bool)mg && mg.NeedStop)
			{
				Stop();
				return;
			}
		}
		else if ((bool)mg && mg.ORBStop)
		{
			Stop();
			return;
		}
		timeO += Time.deltaTime;
		if (timeO >= 0.15f)
		{
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshCompanionUniverseData(sp, CP);
			}
			timeO = 0f;
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
		if ((bool)sp)
		{
			return sp.cp;
		}
		return false;
	}

	public void SetStart()
	{
		CP = sp.cp;
		canAT = true;
		if ((bool)core)
		{
			core.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		}
		if (trans != null)
		{
			for (int i = 0; i < trans.Length; i++)
			{
				if ((bool)trans[i])
				{
					SK_CP_RoundAT component = trans[i].GetComponent<SK_CP_RoundAT>();
					if ((bool)component)
					{
						component.father = this;
						component.CanAT = true;
					}
					Dicform component2 = trans[i].GetComponent<Dicform>();
					if ((bool)component2)
					{
						component2.sp = sp;
						component2.SetCount(sp.ZY);
						component2.SubType = 0;
						component2.Index = 0;
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(SoundA))
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		canAT = false;
		if (!CP && (bool)sp)
		{
			CP = sp.cp;
		}
		if ((bool)CP)
		{
			CP.RemoveRound(this);
		}
		if (trans != null)
		{
			for (int i = 0; i < trans.Length; i++)
			{
				if ((bool)trans[i])
				{
					SK_CP_RoundAT component = trans[i].GetComponent<SK_CP_RoundAT>();
					if ((bool)component)
					{
						component.CanAT = false;
					}
				}
			}
		}
		LeanPool.Despawn(this);
	}
}
