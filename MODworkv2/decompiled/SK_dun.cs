using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_dun : MonoBehaviour
{
	public string SoundA;

	public GameObject spark;

	public GameObject ATprefab;

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private bool CanAT;

	private float timeA;

	private bool hasRegisteredATPrefab;

	private PlayerManager PL;

	private ACTbar act;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		act = SingletonMonoScope<ACTbar>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		CanAT = false;
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
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
	}

	public void SetStart()
	{
		CanAT = true;
		if (sp.Is_BS == 0)
		{
			PL.BSname = sp.skillName;
		}
		if (sp.BSAT_Damage > 0f && (bool)ATprefab)
		{
			if (sp.indexType == 0)
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
				aTprefabSP.Crit_Time = sp.Crit_Time;
				aTprefabSP.Crit_CD = sp.Crit_CD;
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
			if (sp.indexType == 0)
			{
				PL.PrefabCount(86, add: true);
			}
		}
		if (sp.Reborn > 0 && sp.indexType == 0)
		{
			PL.HealStat.Cur += PL.HealStat.Max * (float)sp.Reborn / 100f;
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		CanAT = false;
		ReleaseATPrefab();
		if (sp.indexType == 0)
		{
			PL.PrefabCount(86, add: false);
		}
		if (spark != null)
		{
			LeanPool.Spawn(spark, base.transform.parent.position, Quaternion.identity, base.transform.parent.transform);
		}
		this.wait(1E-06f, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void ReleaseATPrefab()
	{
		if (hasRegisteredATPrefab)
		{
			hasRegisteredATPrefab = false;
			if (act.UnregisterATPrefab())
			{
				PL.BSname = null;
			}
		}
	}
}
