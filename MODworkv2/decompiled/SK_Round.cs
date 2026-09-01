using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Round : MonoBehaviour
{
	public string SoundA;

	public Animator[] dao;

	public Transform[] trans;

	public ParticleSystem[] par;

	public GameObject FX;

	public GameObject ATFX;

	public GameObject spark;

	[Header("=========")]
	public int ATtype;

	public float angle;

	public float DelDelay;

	public float ATtime;

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

	private PlayerManager _playerManager;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		core = base.transform.Find("core");
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeO = 0f;
		canAT = false;
		this.wait(1E-05f, SetStart);
	}

	private void Update()
	{
		core.Rotate(new Vector3(0f, 0f, 1f), angle * Time.deltaTime);
		if (!canAT)
		{
			return;
		}
		if (sp.NoTime == 1)
		{
			timeA += Time.deltaTime;
			if (timeA > sp.BuffTime)
			{
				timeA = 0f;
				Stop();
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
		if (sp.SpecialType != 10)
		{
			return;
		}
		timeO += Time.deltaTime;
		if (timeO >= 0.15f)
		{
			if (sp.SpecialType == 10)
			{
				_playerManager.RefreshORB(sp, 0);
			}
			timeO = 0f;
		}
	}

	public void SetStart()
	{
		canAT = true;
		switch (ATtype)
		{
		case 0:
		{
			for (int k = 0; k < dao.Length; k++)
			{
				SK_RoundAT component3 = dao[k].GetComponent<SK_RoundAT>();
				component3.father = this;
				component3.CanAT = true;
				Dicform component4 = dao[k].gameObject.GetComponent<Dicform>();
				component4.sp = sp;
				component4.SetCount(sp.ZY);
				component4.SubType = 0;
				component4.Index = 0;
				dao[k].SetBool("Bool", value: false);
			}
			break;
		}
		case 1:
		{
			for (int l = 0; l < trans.Length; l++)
			{
				SK_RoundAT component5 = trans[l].GetComponent<SK_RoundAT>();
				component5.father = this;
				component5.CanAT = true;
				Dicform component6 = trans[l].gameObject.GetComponent<Dicform>();
				component6.sp = sp;
				component6.SubType = 0;
				component6.Index = 0;
			}
			break;
		}
		case 2:
		{
			for (int i = 0; i < par.Length; i++)
			{
				ParticleSystem.MainModule main = par[i].main;
				main.loop = true;
			}
			for (int j = 0; j < trans.Length; j++)
			{
				SK_RoundAT component = trans[j].GetComponent<SK_RoundAT>();
				component.father = this;
				component.CanAT = true;
				Dicform component2 = trans[j].gameObject.GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
			}
			break;
		}
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		switch (ATtype)
		{
		case 0:
			disSword();
			break;
		case 1:
			disThunder();
			break;
		case 2:
			disBall();
			break;
		}
	}

	public void disSword()
	{
		for (int i = 0; i < dao.Length; i++)
		{
			dao[i].GetComponent<SK_RoundAT>().CanAT = false;
			dao[i].SetBool("Bool", value: true);
		}
		canAT = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void disThunder()
	{
		canAT = false;
		for (int i = 0; i < trans.Length; i++)
		{
			trans[i].GetComponent<SK_RoundAT>().CanAT = false;
			LeanPool.Spawn(spark, trans[i].position, Quaternion.identity);
		}
		LeanPool.Despawn(this);
	}

	public void disBall()
	{
		for (int i = 0; i < trans.Length; i++)
		{
			trans[i].GetComponent<SK_RoundAT>().CanAT = false;
		}
		for (int j = 0; j < par.Length; j++)
		{
			ParticleSystem.MainModule main = par[j].main;
			main.loop = false;
		}
		canAT = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
