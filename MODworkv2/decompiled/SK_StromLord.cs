using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_StromLord : MonoBehaviour
{
	public string SoundA;

	public GameObject OBJ;

	public GameObject ATprefab;

	public float FStime;

	public ParticleSystem[] parLoop;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private bool CanAT;

	private bool CanStop;

	private bool hasRegisteredATPrefab;

	private PlayerManager _playerManager;

	private ACTbar act;

	public float range;

	public Collider2D[] hit = new Collider2D[10];

	private GameDataManager _gameDataManager;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		act = SingletonMonoScope<ACTbar>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		CanAT = false;
		CanStop = false;
		hasRegisteredATPrefab = false;
		this.wait(1E-05f, SetStart);
	}

	private void OnDisable()
	{
		ReleaseATPrefab();
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= sp.BuffTime)
			{
				Stop();
				timeA = 0f;
			}
			if ((bool)OBJ)
			{
				timeB += Time.deltaTime;
				if (timeB >= FStime)
				{
					Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 0;
					component.Index = 0;
					component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
					if (Random.Range(0, 101) < 30)
					{
						RuntimeManager.PlayOneShot(SoundA, base.transform.position);
					}
					timeB = 0f;
				}
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
		if (CanStop)
		{
			timeC += Time.deltaTime;
			if (timeC >= 2f)
			{
				timeC = 0f;
				LeanPool.Despawn(this);
			}
		}
	}

	public void SetStart()
	{
		CanAT = true;
		if (sp.Is_BS == 0)
		{
			_playerManager.BSname = sp.skillName;
		}
		if (sp.Reborn > 0)
		{
			switch (sp.indexType)
			{
			case 0:
				_playerManager.HealStat.Cur += _playerManager.HealStat.Max * (float)sp.Reborn / 100f;
				break;
			case 1:
				sp.cp.HealthStat.CurrentValue += sp.cp.HealthStat.MaxValue * (float)sp.Reborn / 100f;
				break;
			case 2:
				sp.em.HealthStat.CurrentValue += sp.em.HealthStat.MaxValue * (float)sp.Reborn / 100f;
				break;
			}
		}
		if (sp.BSAT_Damage > 0f && (bool)ATprefab && sp.indexType == 0)
		{
			act.RegisterATPrefab(ATprefab);
			hasRegisteredATPrefab = true;
			SkillOBJ_DT_SP aTprefabSP = act.ATprefabSP;
			aTprefabSP.ZY = sp.ZY;
			aTprefabSP.Dot_Infect = false;
			aTprefabSP.Dot_Infect_Layer = 0;
			aTprefabSP.skillName = sp.skillName;
			aTprefabSP.damageType = sp.damageType;
			aTprefabSP.ThroughType = sp.ThroughType;
			aTprefabSP.Damage = sp.BSAT_Damage;
			aTprefabSP.BJDamage = sp.BJDamage;
			aTprefabSP.ATtarUP = sp.ATtarUP;
			aTprefabSP.MS_Dead = sp.MS_Dead;
			aTprefabSP.JYrate = sp.JYrate;
			aTprefabSP.Through = sp.Through;
			aTprefabSP.FlySpeed = sp.FlySpeed;
			aTprefabSP.Count_F = sp.Count_F;
			aTprefabSP.Count_ATtarget = sp.Count_ATtarget;
			aTprefabSP.CountMulti = sp.CountMulti;
			aTprefabSP.Type_F = sp.Type_F;
			aTprefabSP.TypeDIC_F = sp.TypeDIC_F;
			aTprefabSP.JG = sp.JG;
			aTprefabSP.AngleA = sp.AngleA;
			aTprefabSP.AngleB = sp.AngleB;
			aTprefabSP.Range1 = sp.Range1;
			aTprefabSP.Range2 = sp.Range2;
			aTprefabSP.FStime1 = sp.FStime1;
			aTprefabSP.FStime2 = sp.FStime2;
			aTprefabSP.Speed1 = sp.Speed1;
			aTprefabSP.Speed2 = sp.Speed2;
			aTprefabSP.Speed3 = sp.Speed3;
			aTprefabSP.Speed4 = sp.Speed4;
			aTprefabSP.Follow_F = sp.Follow_F;
			aTprefabSP.AllChuan_F = sp.AllChuan_F;
			aTprefabSP.Slow_F = sp.Slow_F;
			aTprefabSP.RDSpeed_F = sp.RDSpeed_F;
			aTprefabSP.HasFX = sp.HasFX;
			aTprefabSP.colEXP = sp.colEXP;
			aTprefabSP.colEXP = sp.colEXP;
			aTprefabSP.TimeEXP = sp.TimeEXP;
			aTprefabSP.colEXP = sp.colEXP;
			aTprefabSP.LastEXP = sp.LastEXP;
			aTprefabSP.EXPpos = sp.EXPpos;
			aTprefabSP.AngleEXP = sp.AngleEXP;
		}
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void BuffZD(Dicform dic)
	{
		if (sp.Dic_S > 0 && dic.UPDamage == 0f)
		{
			dic.UPDamage = sp.Dic_S;
		}
	}

	public void Stop()
	{
		CanAT = false;
		CanStop = true;
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		ReleaseATPrefab();
	}

	private void ReleaseATPrefab()
	{
		if (hasRegisteredATPrefab)
		{
			hasRegisteredATPrefab = false;
			if (act.UnregisterATPrefab())
			{
				_playerManager.BSname = null;
			}
		}
	}
}
