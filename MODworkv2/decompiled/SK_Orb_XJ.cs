using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Orb_XJ : MonoBehaviour
{
	public string[] SoundA;

	public GameObject[] ORB;

	[HideInInspector]
	public GameObject Buff;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager PL;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.ORB_time)
		{
			timeA = 0f;
			if (PL.IsAlive)
			{
				Fashe();
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
			if (mg.NeedStop)
			{
				Stop();
			}
		}
		else if (mg.ORBStop)
		{
			Stop();
		}
		timeC += Time.deltaTime;
		if (timeC >= 0.3f)
		{
			if (sp.SpecialType == 10)
			{
				PL.RefreshORB(sp, 0);
			}
			timeC = 0f;
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
		CanAT = true;
		Buff = LeanPool.Spawn(ORB[sp.MainEL], base.transform.position, Quaternion.identity, base.transform);
		RuntimeManager.PlayOneShot(SoundA[sp.MainEL], base.transform.position);
	}

	public void Fashe()
	{
		Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[sp.ORB].OBJ[sp.MainEL], new Vector3(PL.transform.position.x + Random.Range(-0.2f, 0.2f), PL.transform.position.y + Random.Range(-0.2f, 0.2f), 0f), Quaternion.identity).GetComponent<Dicform>();
		component.sp = sp;
		component.SetCount(sp.ZY);
		component.SubType = 0;
		component.Index = 0;
	}

	public void Stop()
	{
		CanAT = false;
		if ((bool)Buff)
		{
			LeanPool.Despawn(Buff);
		}
		LeanPool.Despawn(this);
	}
}
