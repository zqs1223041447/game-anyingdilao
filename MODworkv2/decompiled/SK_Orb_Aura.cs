using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Orb_Aura : MonoBehaviour
{
	public GameObject[] FX;

	public float size;

	public float FaSheTimeBF;

	private float timeA;

	private float timeB;

	private bool CanAT;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanAT = false;
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
		for (int i = 0; i < FX.Length; i++)
		{
			FX[i].SetActive(value: false);
		}
		FX[sp.TypeORB].SetActive(value: true);
		switch (sp.TypeORB)
		{
		case 0:
			sp.C_Damage = sp.ORB;
			sp.C_ATspeed = 0f;
			sp.C_MVspeed = 0f;
			sp.C_Health_Prc = 0f;
			break;
		case 1:
			sp.C_ATspeed = sp.ORB;
			sp.C_Damage = 0f;
			sp.C_MVspeed = 0f;
			sp.C_Health_Prc = 0f;
			break;
		case 2:
			sp.C_MVspeed = sp.ORB;
			sp.C_Damage = 0f;
			sp.C_ATspeed = 0f;
			sp.C_Health_Prc = 0f;
			break;
		case 3:
			sp.C_Health_Prc = sp.ORB;
			sp.C_Damage = 0f;
			sp.C_ATspeed = 0f;
			sp.C_MVspeed = 0f;
			break;
		}
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= FaSheTimeBF)
		{
			timeA = 0f;
			if (PL.IsAlive)
			{
				FasheBF();
			}
		}
		if (sp.NoTime == 1)
		{
			timeB += Time.deltaTime;
			if (timeB >= sp.BuffTime)
			{
				timeB = 0f;
				Stop();
			}
		}
		else if ((bool)mg && mg.ORBStop)
		{
			Stop();
		}
		if ((bool)mg && mg.NeedStop)
		{
			Stop();
		}
	}

	public void FasheBF()
	{
		EmptyCOL_BF component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol_BF, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL_BF>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = sp;
		component2.SetCount(sp.ZY);
		component.size = size;
	}

	public void Stop()
	{
		LeanPool.Despawn(this);
	}
}
