using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_SelfPen : MonoBehaviour
{
	public ParticleSystem[] par;

	public Transform[] trans;

	public GameObject FX;

	public float DotMulti;

	public float size;

	public float ColTime;

	public float MoveSpeed;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager _playerManager;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
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
		if (!CanAT || !SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= 0.5f)
		{
			switch (sp.indexType)
			{
			case 0:
				if (_playerManager.IsAlive)
				{
					Fashe();
				}
				break;
			case 1:
				if (sp.cp.IsAlive)
				{
					Fashe();
				}
				break;
			case 2:
				if (sp.em.IsAlive)
				{
					Fashe();
				}
				break;
			}
			timeA = 0f;
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
		if (timeC >= 0.15f)
		{
			if (sp.SpecialType == 10)
			{
				_playerManager.RefreshORB(sp, 0);
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
		for (int i = 0; i < par.Length; i++)
		{
			ParticleSystem.MainModule main = par[i].main;
			main.loop = true;
		}
		CanAT = true;
	}

	public void Fashe()
	{
		Vector3 right = base.transform.right;
		float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
		EmptyCOL component = LeanPool.Spawn(_gameDataManager.SKPB.EmptyCol, trans[1].position, Quaternion.Euler(0f, 0f, num)).GetComponent<EmptyCOL>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = sp;
		component2.SetCount(sp.ZY);
		component2.SubType = 0;
		component2.Index = 0;
		component.size = size;
		component.Body = true;
		component.DotMulti = DotMulti;
		component.CanMV = true;
		component.MoveSpeed = MoveSpeed;
		component.lifeTime = ColTime;
		component.FX = FX;
		component.IsGround = false;
		EmptyCOL component3 = LeanPool.Spawn(_gameDataManager.SKPB.EmptyCol, trans[2].position, Quaternion.Euler(0f, 0f, num + 90f)).GetComponent<EmptyCOL>();
		Dicform component4 = component3.GetComponent<Dicform>();
		component4.sp = sp;
		component4.SetCount(sp.ZY);
		component4.SubType = 0;
		component2.Index = 0;
		component3.size = size;
		component3.Body = true;
		component3.DotMulti = DotMulti;
		component3.CanMV = true;
		component3.MoveSpeed = MoveSpeed;
		component3.lifeTime = ColTime;
		component3.FX = FX;
		component3.IsGround = false;
		EmptyCOL component5 = LeanPool.Spawn(_gameDataManager.SKPB.EmptyCol, trans[0].position, Quaternion.Euler(0f, 0f, num + 180f)).GetComponent<EmptyCOL>();
		Dicform component6 = component5.GetComponent<Dicform>();
		component6.sp = sp;
		component6.SetCount(sp.ZY);
		component6.SubType = 0;
		component2.Index = 0;
		component5.size = size;
		component5.Body = true;
		component5.DotMulti = DotMulti;
		component5.CanMV = true;
		component5.MoveSpeed = MoveSpeed;
		component5.lifeTime = ColTime;
		component5.FX = FX;
		component5.IsGround = false;
		EmptyCOL component7 = LeanPool.Spawn(_gameDataManager.SKPB.EmptyCol, trans[3].position, Quaternion.Euler(0f, 0f, 270f)).GetComponent<EmptyCOL>();
		Dicform component8 = component7.GetComponent<Dicform>();
		component8.sp = sp;
		component8.SetCount(sp.ZY);
		component8.SubType = 0;
		component2.Index = 0;
		component7.size = size;
		component7.Body = true;
		component7.DotMulti = DotMulti;
		component7.CanMV = true;
		component7.MoveSpeed = MoveSpeed;
		component7.lifeTime = ColTime;
		component7.FX = FX;
		component7.IsGround = false;
	}

	public void Stop()
	{
		CanAT = false;
		for (int i = 0; i < par.Length; i++)
		{
			ParticleSystem.MainModule main = par[i].main;
			main.loop = false;
		}
		this.wait(1.5f, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
