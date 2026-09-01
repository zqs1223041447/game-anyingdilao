using System.Collections;
using System.Collections.Generic;
using Entity.Enemies.EnemyAI;
using Entity.Enemies.EnemyState;
using Entity.Enemies.EnemyState.State_A;
using Entity.Enemies.EnemyState.State_A.States;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using Pathfinding;
using UnityEngine;

public class EnemyA : MonoBehaviour
{
	[HideInInspector]
	public AIDestinationSetter AIDS;

	[HideInInspector]
	public Transform ATpoint;

	[HideInInspector]
	public Enemy em;

	[HideInInspector]
	public float JStime;

	[HideInInspector]
	public float JStimeA;

	[HideInInspector]
	public SKprefab PB;

	private bool StartOK;

	public EnemyBrain brain;

	private EnemyStateMachine fsm;

	public bool atCD => JStime >= em.AttackSpeed_JG_Last - 0.0001f;

	private void SetupAI()
	{
		brain = new EnemyBrain(em, em.SpawnPos);
		Dictionary<EnemyStateType, IEnemyState> dictionary = new Dictionary<EnemyStateType, IEnemyState>
		{
			{
				EnemyStateType.Idle,
				new EnemyState_A_Idle(this)
			},
			{
				EnemyStateType.Walk,
				new EnemyState_A_Walk(this)
			},
			{
				EnemyStateType.Attack,
				new EnemyState_A_Attack(this)
			},
			{
				EnemyStateType.Return,
				new EnemyState_A_Return(this)
			},
			{
				EnemyStateType.Patrol,
				new EnemyState_A_Patrol(this)
			},
			{
				EnemyStateType.Die,
				new EnemyState_A_Die(this)
			}
		};
		fsm = new EnemyStateMachine(dictionary, () => (bool)em && em.IS_Frozen);
		foreach (KeyValuePair<EnemyStateType, IEnemyState> item in dictionary)
		{
			if (item.Value is EnemyStateBase_A enemyStateBase_A)
			{
				enemyStateBase_A.BindFSM(fsm);
			}
		}
		fsm.SetInitialState(EnemyStateType.Idle);
	}

	private void ApplyBrainResult()
	{
		if (!em.IsAlive)
		{
			RequestDie();
		}
		else
		{
			RequestState(brain.DesiredState);
		}
	}

	public void RequestState(EnemyStateType type)
	{
		fsm?.RequestState(type);
	}

	public void RequestDie()
	{
		RequestState(EnemyStateType.Die);
	}

	private void Awake()
	{
		em = GetComponent<Enemy>();
		ATpoint = base.transform.Find("main/Spirit");
		AIDS = GetComponent<AIDestinationSetter>();
		PB = SingletonMonoScope<GameDataManager>.Instance.SKPB;
		SetupAI();
	}

	public void ResetState()
	{
		brain.Reset(em.SpawnPos);
		fsm.Reset();
		fsm.SetInitialState(EnemyStateType.Idle);
	}

	private void OnEnable()
	{
		JStimeA = 0f;
		StartOK = false;
		if ((bool)em)
		{
			StartCoroutine(CoStartNextFrame());
			em.OnDirectDamaged += HandleDirectDamaged;
		}
	}

	private void OnDisable()
	{
		em.OnDirectDamaged -= HandleDirectDamaged;
	}

	private void HandleDirectDamaged()
	{
		brain?.OnHit();
	}

	private IEnumerator CoStartNextFrame()
	{
		yield return null;
		SetStart();
	}

	private void SetStart()
	{
		StartOK = true;
		JStime = em.AttackSpeed_JG_Last;
		ResetState();
	}

	private void Update()
	{
		if (!StartOK || !em)
		{
			return;
		}
		brain.Tick(Time.deltaTime);
		ApplyBrainResult();
		fsm.Tick();
		if (em.IsAlive)
		{
			JStimeA += Time.deltaTime;
			if (JStimeA >= 0.1f)
			{
				em.path.maxSpeed = em.MoveSpeed_Path * em.MoveSpeed_Last;
				JStimeA = 0f;
			}
			JStime += Time.deltaTime;
			if (JStime >= em.AttackSpeed_JG_Last)
			{
				JStime = em.AttackSpeed_JG_Last;
			}
		}
	}

	public void changeST(int type)
	{
		switch (type)
		{
		case 0:
			AIDS.target = em.MVTarget;
			em.path.canMove = false;
			break;
		case 1:
			AIDS.target = em.MVTarget;
			em.path.canMove = true;
			break;
		case 2:
			AIDS.target = em.MVTarget;
			em.path.canMove = false;
			break;
		case 3:
			AIDS.target = null;
			em.path.canMove = false;
			break;
		case 4:
			em.path.canMove = true;
			if ((bool)AIDS)
			{
				AIDS.target = em.BrainMovePoint;
			}
			break;
		}
	}

	public void PrepareAttackEnter()
	{
		if ((bool)em)
		{
			PickAttackSkillIndex();
			UseAT();
			em.AT_Idle_Cur = Random.Range(em.AT_Idle_Min / em.AttackSpeed_Last, em.AT_Idle_Max / em.AttackSpeed_Last);
			JStime = 0f;
		}
	}

	private void PickAttackSkillIndex()
	{
		int num = Random.Range(0, 101);
		int dotSilencedSkillRate = em.GetDotSilencedSkillRate(em.SK_Rate);
		if (num < dotSilencedSkillRate)
		{
			if (Random.Range(0, 101) < em.SK_Rate_ELSS && em.SK_ELSS.ATmod != 2)
			{
				em.SK_Cur_Index = 4;
				return;
			}
			if (em.SK_Rate_CompFS > 0)
			{
				if (em.SK_Rate_Comp > 0)
				{
					if (em.SK_Rate_FS > 0)
					{
						if (Random.Range(0, 101) < 30)
						{
							int num2 = Random.Range(0, 101);
							em.SK_Cur_Index = ((num2 >= em.SK_Rate_FS) ? 1 : 3);
						}
						else
						{
							int num3 = Random.Range(0, 101);
							em.SK_Cur_Index = ((num3 >= em.SK_Rate_Comp) ? 1 : 2);
						}
						return;
					}
					if (em.SK_Rate_FS == 0)
					{
						int num4 = Random.Range(0, 101);
						em.SK_Cur_Index = ((num4 >= em.SK_Rate_Comp) ? 1 : 2);
						return;
					}
				}
				if (em.SK_Rate_Comp == 0 && em.SK_Rate_FS > 0)
				{
					int num5 = Random.Range(0, 101);
					em.SK_Cur_Index = ((num5 >= em.SK_Rate_FS) ? 1 : 3);
					return;
				}
			}
			em.SK_Cur_Index = 1;
		}
		else
		{
			em.SK_Cur_Index = 0;
		}
	}

	public void UseAT()
	{
		switch (em.SK_Cur_Index)
		{
		case 0:
			SetAT_Data(em.SK_AT);
			break;
		case 1:
			SetAT_Data(em.SK_A);
			break;
		case 2:
			SetCP_Data(em.SK_Comp);
			break;
		case 3:
			SetFS_Data(em.SK_FS);
			break;
		case 4:
			SetAT_Data(em.SK_ELSS);
			break;
		}
	}

	public void SetAT_Data(EM_Skill_SP dt)
	{
		Transform transform = ((dt.TypeTar != 0) ? em.MVTarget : em.ATTarget);
		SkillOBJ_DT_SP component;
		Transform aTpoint;
		switch (dt.FStype)
		{
		case 0:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = ATpoint;
			break;
		case 1:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = ATpoint;
			break;
		case 2:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = ATpoint;
			break;
		case 3:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = ATpoint;
			break;
		case 4:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = em.transform;
			break;
		case 5:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = em.yao.transform;
			break;
		case 6:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.transform.position, Quaternion.identity, em.transform).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = em.transform;
			break;
		case 7:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = em.yao.transform;
			break;
		case 8:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.headUp.transform.position, Quaternion.identity, em.headUp.transform).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = em.yao.transform;
			break;
		case 9:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.MVTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = base.transform;
			break;
		case 10:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], em.ATTarget.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = base.transform;
			break;
		default:
			component = LeanPool.Spawn(PB.Skill[dt.OBJ].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			aTpoint = ATpoint;
			break;
		}
		if (dt.RTtypeOBJ == 0)
		{
			Vector3 vector = transform.position - aTpoint.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			component.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
		component.indexType = 2;
		component.em = em;
		component.ZY = false;
		component.Dot_Infect = false;
		component.Dot_Infect_Layer = 0;
		component.TargetPos = transform.position;
		component.skillName = dt.IndexName;
		component.dic = transform.position - aTpoint.position;
		component.RTtypeOBJ = dt.RTtypeOBJ;
		component.RTtypeFX = dt.RTtypeFX;
		component.Distance = dt.Distance;
		component.GlobalID = 100000;
		component.damageType = em.MainELType;
		component.MainEL = em.MainElement;
		component.ThroughType = dt.ThroughType;
		component.AttackType = dt.AttackType;
		component.AttackTypeA = dt.AttackTypeA;
		component.AttackTypeB = dt.AttackTypeB;
		component.Damage = dt.Damage / 100f * em.Damage_Last;
		component.DamageA = dt.DamageA / 100f * em.Damage_Last;
		component.DamageB = dt.DamageB / 100f * em.Damage_Last;
		component.BJrate = em.BJRate;
		component.Through = em.Through;
		component.FlySpeed = em.FlySpeed;
		component.Chuan = em.Chuan;
		component.MoveSpeedCut = dt.SpeedCut;
		component.AttackSpeedCut = dt.SpeedCut;
		component.BF_EL_Chuan = 0f;
		component.BF_BJrate = 0f;
		component.BF_GeDang = 0f;
		component.BF_DamageAnti = dt.BF_DamageAnti;
		component.C_Damage = dt.C_Damage;
		component.C_ATspeed = dt.CompAttackSpeed;
		component.C_MVspeed = 0f;
		component.C_Health_Prc = 0f;
		component.BF_Through = 0f;
		component.CF_Rate = em.CF_Rate;
		component.ChangeSkin = 1;
		component.SkinIndex = 0;
		component.Reborn = dt.Reborn;
		component.DotRate = dt.DotRate;
		component.DotDamage = dt.DotDamage / 100f * em.Damage_Last;
		component.NoTime = 1;
		component.BuffTime = dt.BuffTime;
		component.DebuffTime = dt.DebuffTime;
		component.Field_time = 0f;
		component.ORB_time = dt.ORB_time;
		component.EXP_time = dt.EXP_time;
		component.ZD_time_F = 0f;
		component.ZD_time_S = 0f;
		component.ORB = dt.ORB;
		component.ZD_F = dt.ZD_F;
		component.ZD_S = dt.ZD_S;
		component.ZD_AB = dt.ZD_AB;
		component.EXP_F = dt.EXP_F;
		component.EXP_S = dt.EXP_S;
		component.EXP_AB = dt.EXP_AB;
		component.Dic_F = dt.Dic_F;
		component.Dic_S = dt.Dic_S;
		component.FX_F = 0;
		component.FX_S = 0;
		component.Sound = dt.Sound;
		component.Count_ORB = dt.Count_ORB;
		component.Count_ATtarget = dt.Count_ATtarget;
		component.CF_Count = dt.CF_Count;
		component.Count_F = dt.Count_F;
		component.Count_S = dt.Count_S;
		component.Count_AB = dt.Count_AB;
		component.CountMulti = dt.CountMulti;
		component.CountEXP = dt.CountEXP;
		component.TypeORB = dt.TypeORB;
		component.CF_Type = dt.CF_Type;
		component.Type_F = dt.Type_F;
		component.Type_S = dt.Type_S;
		component.Type_AB = dt.Type_AB;
		component.TypeDIC_F = dt.TypeDIC_F;
		component.TypeDIC_S = dt.TypeDIC_S;
		component.TypeEXP_F = dt.TypeEXP_F;
		component.TypeEXP_S = dt.TypeEXP_S;
		component.TypeEXP_AB = dt.TypeEXP_AB;
		component.Size = dt.Size;
		component.High = dt.High;
		component.JG = dt.JG;
		component.AngleA = dt.AngleA;
		component.AngleB = dt.AngleB;
		component.Range1 = dt.Range1;
		component.Range2 = dt.Range2;
		component.Range_AT = dt.Range_AT;
		component.FStime1 = dt.FStime1;
		component.FStime2 = dt.FStime2;
		component.Speed1 = dt.Speed1;
		component.Speed2 = dt.Speed2;
		component.Speed3 = dt.Speed3;
		component.Speed4 = dt.Speed4;
		component.Follow_F = dt.Follow_F;
		component.Follow_S = dt.Follow_S;
		component.AllChuan_F = dt.AllChuan_F;
		component.AllChuan_S = dt.AllChuan_S;
		component.Slow_F = 1;
		component.Slow_S = 1;
		component.RDSpeed_F = dt.RDSpeed_F;
		component.RDSpeed_S = dt.RDSpeed_S;
		component.HasFX = dt.HasFX;
		component.S_HasFX = dt.S_HasFX;
		component.AB_HasFX = dt.AB_HasFX;
		component.colEXP = dt.colEXP;
		component.colEXP_A = dt.colEXP_AB;
		component.S_colEXP = dt.S_colEXP;
		component.AB_colEXP = dt.AB_colEXP;
		component.TimeEXP = dt.TimeEXP;
		component.TimeEXP_AB = dt.TimeEXP_AB;
		component.LastEXP = 1;
		component.LastEXP_AB = 1;
		component.S_LastEXP = 1;
		component.AB_LastEXP = 1;
		component.EXPpos = dt.EXPpos;
		component.EXPpos_AB = dt.EXPpos_AB;
		component.S_EXPpos = dt.S_EXPpos;
		component.AB_EXPpos = dt.AB_EXPpos;
		component.AngleEXP = dt.AngleEXP;
		component.AngleEXP_AB = dt.AngleEXP_AB;
	}

	public void SetCP_Data(EM_Skill_CP dt)
	{
		int num = em.Comp_Count - em.cpList.Count;
		int num2 = ((num < em.Comp_EveryCount) ? num : em.Comp_EveryCount);
		float num3 = ((num2 < 2) ? 1f : ((num2 >= 4) ? 2f : 1.5f));
		int eL = Random.Range(0, 6);
		for (int i = 0; i < num2; i++)
		{
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			SK_FSQ_compEM component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.CompMB, new Vector3(base.transform.position.x + Random.Range(0f - num3, num3), base.transform.position.y + Random.Range(0f - num3, num3), base.transform.position.z), Quaternion.identity).GetComponent<SK_FSQ_compEM>();
			component.UseType = 0;
			component.FX = em.CP_FX;
			component.offset = GetMB(em.GlobalID).CompOffset;
			component.skCP = em.SK_Comp;
			component.EL = eL;
			component.em = em;
		}
		if (dt.CPFX > 0)
		{
			LeanPool.Spawn(PB.ATFX[dt.CPFX].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity);
		}
	}

	public void SetFS_Data(EM_Skill_FS dt)
	{
		float num = ((em.FS_EveryCount < 4) ? 1.2f : ((em.FS_EveryCount <= 3 || em.FS_EveryCount >= 8) ? 2.5f : 1.8f));
		if (em.FS_Count - em.fsList.Count < em.FS_EveryCount)
		{
			int num2 = em.FS_EveryCount - (em.FS_Count - em.fsList.Count);
			int num3;
			for (num3 = 0; num3 < num2; num3++)
			{
				Enemy enemy = em.fsList[num3];
				em.fsList.Remove(enemy);
				num3--;
				enemy.HealthStat.CurrentValue = 0f;
			}
		}
		for (int i = 0; i < em.FS_EveryCount; i++)
		{
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			SK_FSQ_compEM component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.CompMB, new Vector3(base.transform.position.x + Random.Range(0f - num, num), base.transform.position.y + Random.Range(0f - num, num), base.transform.position.z), Quaternion.identity).GetComponent<SK_FSQ_compEM>();
			component.UseType = 1;
			component.FX = em.CP_FX;
			component.offset = GetMB(em.GlobalID).CompOffset;
			component.EL = em.MainElement;
			component.em = em;
		}
		if (dt.CPFX > 0)
		{
			LeanPool.Spawn(PB.ATFX[dt.CPFX].OBJ[em.MainElement], ATpoint.transform.position, Quaternion.identity);
		}
	}

	public static EnemyMB GetMB(int id)
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return null;
		}
		foreach (EnemyMB item in SingletonMonoScope<GameDataManager>.Instance.EMMB)
		{
			if (item.GlobalID == id)
			{
				return item;
			}
		}
		return null;
	}
}
