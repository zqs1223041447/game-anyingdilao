using System.Collections;
using Data.RuntimeData.Skills.CompSkill;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_comp : MonoBehaviour
{
	public GameObject fx;

	public float offset;

	public GameObject obj;

	public float TimeDelay;

	[HideInInspector]
	public bool HasCreat;

	[HideInInspector]
	public GameObject go;

	private PlayerManager PL;

	private Coroutine _routine;

	private bool _running;

	private void Awake()
	{
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		HasCreat = false;
		go = null;
		_running = false;
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
	}

	private void OnDisable()
	{
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_running = false;
	}

	public void Init(CompanionRuntimeData data, bool restore = false, float hp = 0f)
	{
		if (data == null)
		{
			LogUtil.Error("传入的数据源为空");
			return;
		}
		if (!restore)
		{
			StartNormalOnce(data);
			return;
		}
		CreateCompInstant(data, hp);
		this.wait(0.01f, delegate
		{
			LeanPool.Despawn(base.gameObject);
		});
	}

	private void StartNormalOnce(CompanionRuntimeData data)
	{
		if (!_running)
		{
			_running = true;
			if (_routine != null)
			{
				StopCoroutine(_routine);
				_routine = null;
			}
			_routine = StartCoroutine(RunNormal(data));
		}
	}

	private IEnumerator RunNormal(CompanionRuntimeData data)
	{
		if ((bool)fx)
		{
			go = LeanPool.Spawn(fx, new Vector3(base.transform.position.x, base.transform.position.y + offset, base.transform.position.z), Quaternion.identity, base.transform);
		}
		if (TimeDelay > 0f)
		{
			yield return new WaitForSeconds(TimeDelay);
		}
		CreateCompInstant(data);
		yield return new WaitForSeconds(1f);
		if ((bool)go)
		{
			LeanPool.Despawn(go);
			go = null;
		}
		LeanPool.Despawn(base.gameObject);
		_routine = null;
		_running = false;
	}

	private void CreateCompInstant(CompanionRuntimeData data, float hp = 0f)
	{
		if (HasCreat)
		{
			return;
		}
		HasCreat = true;
		if (data == null)
		{
			LogUtil.Error("FSQ 同伴发射器", "comp_data 为空，生成被终止");
			return;
		}
		if (!obj)
		{
			LogUtil.Error("FSQ 同伴发射器", "obj 为空，生成被终止");
			return;
		}
		GameObject gameObject = LeanPool.Spawn(obj, base.transform.position, Quaternion.identity);
		Companion companion = (gameObject ? gameObject.GetComponent<Companion>() : null);
		if (!companion)
		{
			LogUtil.Error("FSQ 同伴发射器", "生成的对象没有 Companion 组件");
			return;
		}
		SetCompData(companion, data, hp);
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.CompCountPlus(data.skillName, companion);
		}
	}

	public void SetCompData(Companion comp, CompanionRuntimeData dt_cp, float hp = 0f)
	{
		comp.Name = dt_cp.skillName;
		comp.BStype = dt_cp.BStype;
		comp.GetComponent<FxControl_CPA>()?.ApplyColorData();
		comp.AT_ZD = dt_cp.AT_ZD;
		comp.SK_ZD = dt_cp.SK_ZD;
		comp.AT_DMG = dt_cp.AT_DMG;
		comp.SK_DMG = dt_cp.SK_DMG;
		comp.AttackSpeed_Bei = dt_cp.AttackSpeed;
		comp.GeDang_Base = dt_cp.GeDang;
		comp.Damage_Base = dt_cp.Damage;
		comp.Health_Prc_Base = dt_cp.Health_Prc;
		comp.damageType = dt_cp.damageType;
		comp.damageType_Change = dt_cp.damageType_Change;
		comp.Change_AT = dt_cp.Change_AT;
		comp.ATSrate = dt_cp.ATSrate;
		comp.ChangeEL_SK = dt_cp.ChangeEL_SK;
		comp.ATS_Damage = dt_cp.ATS_Damage;
		comp.ChangeEL_AR = dt_cp.ChangeEL_AR;
		comp.ARS_Damage = dt_cp.ARS_Damage;
		comp.DotMultiA = dt_cp.DotMultiA;
		comp.DotMultiB = dt_cp.DotMultiB;
		comp.GD_R_Heal = dt_cp.GD_R_Heal;
		comp.BloodDie = dt_cp.BloodDie;
		comp.TGYJ = dt_cp.TGYJ;
		comp.Kill_R_Heal = dt_cp.Kill_R_Heal;
		comp.Hurt_FT = dt_cp.Hurt_FT;
		comp.AT_DotLayer = dt_cp.AT_DotLayer;
		comp.BJ_NoDot = dt_cp.BJ_NoDot;
		comp.WS_All = dt_cp.WS_All;
		comp.Field_Range = dt_cp.Field_Range;
		comp.MaxForceFollowDistance = dt_cp.DisA;
		comp.MaxTeleportDistance = dt_cp.DisB;
		comp.Idle_Time_Min = dt_cp.Idle_Time_Min;
		comp.Idle_Time_Max = dt_cp.Idle_Time_Max;
		comp.SO_IdleRate = dt_cp.SO_IdleRate;
		comp.SO_AttackRate = dt_cp.SO_AttackRate;
		comp.SO_SayRate = dt_cp.SO_SayRate;
		comp.SO_HurtRate = dt_cp.SO_HurtRate;
		comp.SO_DieRate = dt_cp.SO_DieRate;
		comp.SO_Idle = dt_cp.SO_Idle;
		comp.SO_Walk = dt_cp.SO_Walk;
		comp.SO_AttackA = dt_cp.SO_AttackA;
		comp.SO_SayA = dt_cp.SO_SayA;
		comp.SO_AttackB = dt_cp.SO_AttackB;
		comp.SO_SayB = dt_cp.SO_SayB;
		comp.SO_AttackC = dt_cp.SO_AttackC;
		comp.SO_SayC = dt_cp.SO_SayC;
		comp.SO_Hurt = dt_cp.SO_Hurt;
		comp.SO_Die = dt_cp.SO_Die;
		comp.Type_A = dt_cp.Type_A;
		comp.Type_B = dt_cp.Type_B;
		comp.TypeDIC_A = dt_cp.TypeDIC_A;
		comp.TypeDIC_B = dt_cp.TypeDIC_B;
		comp.JG_A = dt_cp.JG_A;
		comp.JG_B = dt_cp.JG_B;
		comp.AngleA = dt_cp.AngleA;
		comp.AngleB = dt_cp.AngleB;
		comp.FStimeA = dt_cp.FStimeA;
		comp.FStimeB = dt_cp.FStimeB;
		comp.Count_A = dt_cp.Count_A;
		comp.Count_B = dt_cp.Count_B;
		comp.AT_Double = dt_cp.AT_Double;
		comp.Count_ATtarget_A = dt_cp.Count_ATtarget_A;
		comp.Count_ATtarget_B = dt_cp.Count_ATtarget_B;
		comp.CountMulti_A = dt_cp.CountMulti_A;
		comp.CountMulti_B = dt_cp.CountMulti_B;
		comp.Follow_A = dt_cp.Follow_A;
		comp.Follow_B = dt_cp.Follow_B;
		comp.AllChuan_A = dt_cp.AllChuan_A;
		comp.AllChuan_B = dt_cp.AllChuan_B;
		comp.RDSpeed_A = dt_cp.RDSpeed_A;
		comp.RDSpeed_B = dt_cp.RDSpeed_B;
		comp.HasFX_A = dt_cp.HasFX_A;
		comp.HasFX_B = dt_cp.HasFX_B;
		comp.colEXP_A = dt_cp.colEXP_A;
		comp.colEXP_B = dt_cp.colEXP_B;
		comp.EXPpos_A = dt_cp.EXPpos_A;
		comp.EXPpos_B = dt_cp.EXPpos_B;
		comp.HealthStat.Initialize((dt_cp.Health + PL.Damage_Last * 3f + PL.HealStat.Max * 3f) * (1f + PL.C_Health_Last / 100f));
		float current = ((hp > 0f) ? hp : comp.HealthStat.MaxValue);
		comp.HealthStat.SetCurrent(current);
		comp.sp = comp.gameObject.GetComponent<SkillOBJ_DT_SP>();
		comp.sp.indexType = 1;
		comp.sp.cp = comp;
		comp.sp.pl = PL;
		comp.sp.ZY = true;
		comp.sp.Dot_Infect = false;
		comp.sp.Dot_Infect_Layer = 0;
		comp.sp.skillName = dt_cp.skillName;
		comp.sp.Damage = comp.Damage_Last / 100f * PL.GiveDamage(dt_cp.damageType) * (float)comp.AT_DMG / 100f;
		comp.sp.damageType = dt_cp.damageType;
		comp.sp.AttackType = true;
		comp.sp.AttackTypeA = true;
		comp.sp.DamageA = comp.Damage_Last * dt_cp.ATS_Damage / dt_cp.Damage / 100f * PL.GiveDamage(dt_cp.ChangeEL_SK) * comp.SkillDamageMultiplier;
		comp.sp.AttackTypeB = false;
		comp.sp.DamageB = comp.Damage_Last * dt_cp.ARS_Damage / dt_cp.Damage / 100f * PL.GiveDamage(dt_cp.ChangeEL_AR) * comp.SkillDamageMultiplier;
		comp.sp.BJrate = (comp.BJ_NoDot ? 100f : PL.BJrate_Last);
		comp.sp.JYrate = PL.JYrate_Last;
		comp.sp.Chuan = PL.GiveChuan(dt_cp.ChangeEL_AR);
		comp.sp.BJDamage = PL.BJDamage_Last;
		comp.sp.FlySpeed = comp.FlySpeed;
		comp.sp.AT_DotLayer = comp.AT_DotLayer;
		comp.sp.BJ_NoDot = comp.BJ_NoDot;
		comp.sp.WS_All = comp.WS_All;
		comp.sp.Field_Range = comp.Field_Range;
	}
}
