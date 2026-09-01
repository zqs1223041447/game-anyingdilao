using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class SK_BuffA : MonoBehaviour
{
	private float timeA;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private bool CanBuff;

	[HideInInspector]
	public bool NeedStop;

	[HideInInspector]
	public bool ORBStop;

	[HideInInspector]
	public bool IsORB;

	private bool endedNaturally;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
	}

	private void OnEnable()
	{
		NeedStop = false;
		ORBStop = false;
		timeA = 0f;
		endedNaturally = false;
		CanBuff = false;
		initialized = false;
	}

	private void OnDisable()
	{
		if ((bool)sp && sp.indexType == 0 && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			SingletonMonoScope<PlayerManager>.Instance.BuffRuntime?.UnregisterSkillInstance(sp, endedNaturally);
		}
	}

	private void Update()
	{
		if (!CanBuff)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.BuffTime - 0.05f)
		{
			timeA = 0f;
			endedNaturally = true;
			CanBuff = false;
			switch (sp.indexType)
			{
			case 0:
				SingletonMonoScope<ACTbar>.Instance.SkillBuffList.Remove(this);
				break;
			case 1:
				sp.cp.SkillBuffList.Remove(this);
				break;
			case 2:
				sp.em.SkillBuffList.Remove(this);
				break;
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
		if (IsORB)
		{
			return;
		}
		CanBuff = true;
		switch (sp.indexType)
		{
		case 0:
			if (sp.BuffTime > 0f)
			{
				Buff_PL buff_PL = new Buff_PL();
				buff_PL.type = 1;
				buff_PL.BuffTime = sp.BuffTime;
				buff_PL.IsSkillBuff = true;
				buff_PL.IndexName = sp.skillName;
				buff_PL.damageType = sp.damageType;
				buff_PL.DotDamage = 0f;
				buff_PL.DotChuan = 0f;
				if (SingletonMonoScope<PlayerManager>.Instance.peo.BuffPL.HasSameBuff(sp.skillName))
				{
					buff_PL.Damage = sp.BF_Damage / 2f;
					buff_PL.EL_Damage = sp.BF_EL_Damage / 2f;
					buff_PL.Chuan = sp.BF_EL_Chuan / 2f;
					buff_PL.BJrate = sp.BF_BJrate / 2f;
					buff_PL.JYrate = sp.BF_JYrate / 2f;
					buff_PL.GeDang = sp.BF_GeDang / 2f;
					buff_PL.AttackSpeed = sp.BF_AttackSpeed / 2f;
					buff_PL.MoveSpeed = sp.BF_MoveSpeed / 2f;
					buff_PL.DamageAnti = sp.BF_DamageAnti / 2f;
					buff_PL.Health_Prc = sp.BF_Health_Prc / 2f;
				}
				else
				{
					buff_PL.Damage = sp.BF_Damage;
					buff_PL.EL_Damage = sp.BF_EL_Damage;
					buff_PL.Chuan = sp.BF_EL_Chuan;
					buff_PL.BJrate = sp.BF_BJrate;
					buff_PL.JYrate = sp.BF_JYrate;
					buff_PL.GeDang = sp.BF_GeDang;
					buff_PL.AttackSpeed = sp.BF_AttackSpeed;
					buff_PL.MoveSpeed = sp.BF_MoveSpeed;
					buff_PL.DamageAnti = sp.BF_DamageAnti;
					buff_PL.Health_Prc = sp.BF_Health_Prc;
				}
				SingletonMonoScope<PlayerManager>.Instance.BuffMG.AddBuff(buff_PL);
				SingletonMonoScope<ACTbar>.Instance.SkillBuffList.Add(this);
			}
			break;
		case 1:
			if (sp.BuffTime > 0f)
			{
				Buff_CP buff_CP = new Buff_CP();
				buff_CP.type = 1;
				buff_CP.BuffTime = sp.BuffTime;
				buff_CP.damageType = sp.damageType;
				buff_CP.Damage = sp.C_Damage;
				buff_CP.AttackSpeed = sp.C_ATspeed;
				buff_CP.MoveSpeed = sp.C_MVspeed;
				buff_CP.Health_Prc = sp.C_Health_Prc;
				sp.cp.BuffMG.AddBuff(buff_CP);
			}
			sp.cp.SkillBuffList.Add(this);
			break;
		case 2:
			if (sp.BuffTime > 0f)
			{
				Buff_Enemy buff_Enemy = new Buff_Enemy();
				buff_Enemy.type = 1;
				buff_Enemy.BuffTime = sp.BuffTime;
				buff_Enemy.damageType = sp.damageType;
				buff_Enemy.Damage = sp.C_Damage;
				buff_Enemy.Chuan = sp.BF_EL_Chuan;
				buff_Enemy.Through = sp.BF_Through;
				buff_Enemy.BJrate = sp.BF_BJrate;
				buff_Enemy.GeDang = sp.BF_GeDang;
				buff_Enemy.AttackSpeed = sp.C_ATspeed;
				buff_Enemy.MoveSpeed = sp.C_MVspeed;
				buff_Enemy.DamageAnti = sp.BF_DamageAnti;
				buff_Enemy.Health_Prc = sp.C_Health_Prc;
				sp.em.BuffMG.AddBuff(buff_Enemy);
				sp.em.SkillBuffList.Add(this);
			}
			break;
		}
	}

	public void StopBuff()
	{
		NeedStop = true;
	}

	public void StopORB()
	{
		IsORB = false;
		ORBStop = true;
	}
}
