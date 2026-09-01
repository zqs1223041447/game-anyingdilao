using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_BuffA : MonoBehaviour
{
	public string SoundA;

	public GameObject OBJ;

	public GameObject EXP;

	public GameObject FX;

	public GameObject ATprefab;

	public GameObject Heal;

	public GameObject SubA;

	public GameObject SubB;

	public ParticleSystem[] parLoop;

	public bool UseDICtime;

	public float LifeTime;

	public float DelDelay;

	public bool HasBodyFX;

	public bool CanEXP;

	public float FaSheTime;

	public float DotMulti;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	private bool CanStop;

	private bool hasRegisteredATPrefab;

	private PlayerManager _playerManager;

	private ACTbar act;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
		act = SingletonMonoScope<ACTbar>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
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
			if (timeA >= LifeTime)
			{
				Stop();
				timeA = 0f;
			}
			if (CanEXP && EXP != null)
			{
				timeB += Time.deltaTime;
				if (timeB >= FaSheTime)
				{
					Dicform component = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 0;
					component.Index = 0;
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
			if (timeC >= DelDelay)
			{
				timeC = 0f;
				LeanPool.Despawn(this);
			}
		}
	}

	public void SetStart()
	{
		CanAT = true;
		if (OBJ != null)
		{
			LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity);
		}
		if (UseDICtime)
		{
			LifeTime = sp.BuffTime;
		}
		if (sp.Is_BS == 0)
		{
			_playerManager.BSname = sp.skillName;
		}
		if (sp.ChangeSkin == 0)
		{
			switch (sp.indexType)
			{
			case 0:
				_playerManager.sqs.ChangeSkin(sp.SkinIndex);
				break;
			case 1:
				sp.cp.SetSkin(sp.SkinIndex);
				break;
			case 2:
				sp.em.SetSkin(sp.SkinIndex);
				break;
			}
		}
		if (sp.Reborn > 0)
		{
			switch (sp.indexType)
			{
			case 0:
				_playerManager.HealStat.Cur += _playerManager.HealStat.Max * (float)sp.Reborn / 100f;
				if (Heal != null)
				{
					LeanPool.Spawn(Heal, _playerManager.transform.position, Quaternion.identity);
				}
				break;
			case 1:
				sp.cp.HealthStat.CurrentValue += sp.cp.HealthStat.MaxValue * (float)sp.Reborn / 100f;
				if ((bool)Heal)
				{
					LeanPool.Spawn(Heal, sp.cp.transform.position, Quaternion.identity);
				}
				break;
			case 2:
				if (sp.em.peo.DotEM.GerDotSL())
				{
					sp.em.HealthStat.CurrentValue -= sp.em.HealthStat.MaxValue * (float)sp.Reborn / 100f;
				}
				else
				{
					sp.em.HealthStat.CurrentValue += sp.em.HealthStat.MaxValue * (float)sp.Reborn / 100f;
				}
				if ((bool)Heal)
				{
					LeanPool.Spawn(Heal, sp.em.transform.position, Quaternion.identity);
				}
				break;
			}
		}
		if (HasBodyFX && sp.indexType == 0)
		{
			_playerManager.sqs.FXon();
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
			ParticleSystem[] array = parLoop;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				main.loop = true;
			}
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		CanAT = false;
		CanStop = true;
		if ((bool)FX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array = parLoop;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				main.loop = false;
			}
		}
		if (sp.ChangeSkin == 0 && !act.HasSameSkillFX(sp.skillName))
		{
			_playerManager.sqs.ChangeSkin(0);
		}
		ReleaseATPrefab();
		if (HasBodyFX && !act.HasSameSkillFX(sp.skillName))
		{
			_playerManager.sqs.FXoff();
		}
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
