using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class People : MonoBehaviour
{
	public PlayerManager pl;

	public Companion cp;

	public Enemy em;

	public int CharacterType;

	public bool ZY;

	public DOT_MG DotEM;

	public BuffMG_EM BuffEM;

	public BuffMG_PL BuffPL;

	public BuffMG_CP BuffCP;

	private void Awake()
	{
		if (base.transform.parent.gameObject.transform.TryGetComponent<PlayerManager>(out var component))
		{
			component.peo = this;
			pl = component;
			CharacterType = 0;
			ZY = true;
			if (TryGetComponent<BuffMG_PL>(out var component2))
			{
				BuffPL = component2;
			}
		}
		if (base.transform.parent.gameObject.transform.TryGetComponent<Companion>(out var component3))
		{
			component3.peo = this;
			cp = component3;
			CharacterType = 1;
			ZY = true;
			if (TryGetComponent<BuffMG_CP>(out var component4))
			{
				BuffCP = component4;
			}
		}
		if (base.transform.parent.gameObject.transform.TryGetComponent<Enemy>(out var component5))
		{
			component5.peo = this;
			em = component5;
			CharacterType = 2;
			ZY = false;
			if (TryGetComponent<DOT_MG>(out var component6))
			{
				DotEM = component6;
			}
			if (TryGetComponent<BuffMG_EM>(out var component7))
			{
				BuffEM = component7;
			}
		}
	}

	public void PL_Set(SkillOBJ_DT_SP sp, int SubType)
	{
		bool flag = IsCursedEnemyAttack(sp);
		float bJrate = (flag ? 0f : sp.BJrate);
		float bJDamage = (flag ? 0f : sp.BJDamage);
		switch (SubType)
		{
		case 0:
			if (sp.AttackType)
			{
				pl.TakeDamage(sp.Damage, sp.Chuan, bJrate, bJDamage, sp.damageType, sp.em);
			}
			else
			{
				pl.TakeDamage(sp.Damage / 2f, sp.Chuan, bJrate, bJDamage, sp.damageType, sp.em);
			}
			break;
		case 1:
			if (sp.AttackTypeA)
			{
				pl.TakeDamage(sp.DamageA, sp.Chuan, bJrate, bJDamage, sp.damageType, sp.em);
			}
			else
			{
				pl.TakeDamage(sp.DamageA / 2f, sp.Chuan, bJrate, bJDamage, sp.damageType, sp.em);
			}
			break;
		case 2:
			if (sp.AttackTypeB)
			{
				pl.TakeDamage(sp.DamageB, sp.Chuan, bJrate, bJDamage, sp.damageType, sp.em);
			}
			else
			{
				pl.TakeDamage(sp.DamageB / 2f, sp.Chuan, bJrate, bJDamage, sp.damageType, sp.em);
			}
			break;
		case 3:
			pl.TakeDamage(sp.Damage / 2f, sp.Chuan, bJrate, bJDamage, sp.damageType, sp.em);
			break;
		}
		if (!flag && SubType == 0 && (sp.DebuffTime > 0f || sp.AttackSpeedCut > 0f || sp.MoveSpeedCut > 0f || sp.DotDamage > 0f) && sp.DotRate > 0f && (float)Random.Range(1, 100) < sp.DotRate)
		{
			Buff_PL buff_PL = new Buff_PL();
			buff_PL.type = 0;
			buff_PL.damageType = sp.damageType;
			buff_PL.BuffTime = sp.DebuffTime;
			buff_PL.IsSkillBuff = false;
			buff_PL.De_ATSpeedCut = sp.AttackSpeedCut;
			buff_PL.De_MVSpeedCut = sp.MoveSpeedCut;
			buff_PL.DotChuan = sp.Chuan;
			buff_PL.DotDamage = sp.DotDamage;
			BuffPL.AddBuff(buff_PL);
		}
	}

	public void CP_Set(SkillOBJ_DT_SP sp, int SubType)
	{
		bool flag = IsCursedEnemyAttack(sp);
		float bJrate = (flag ? 0f : sp.BJrate);
		float bJDamage = (flag ? 0f : sp.BJDamage);
		switch (SubType)
		{
		case 0:
			if (sp.AttackType)
			{
				cp.TakeDamage(sp.Damage, sp.Chuan, bJrate, bJDamage, sp.JYrate, sp.damageType, sp.em);
			}
			else
			{
				cp.TakeDamage(sp.Damage / 2f, sp.Chuan, bJrate, bJDamage, sp.JYrate, sp.damageType, sp.em);
			}
			break;
		case 1:
			if (sp.AttackTypeA)
			{
				cp.TakeDamage(sp.DamageA, sp.Chuan, bJrate, bJDamage, sp.JYrate, sp.damageType, sp.em);
			}
			else
			{
				cp.TakeDamage(sp.DamageA / 2f, sp.Chuan, bJrate, bJDamage, sp.JYrate, sp.damageType, sp.em);
			}
			break;
		case 2:
			if (sp.AttackTypeB)
			{
				cp.TakeDamage(sp.DamageB, sp.Chuan, bJrate, bJDamage, sp.JYrate, sp.damageType, sp.em);
			}
			else
			{
				cp.TakeDamage(sp.DamageB / 2f, sp.Chuan, bJrate, bJDamage, sp.JYrate, sp.damageType, sp.em);
			}
			break;
		case 3:
			cp.TakeDamage(sp.Damage / 2f, sp.Chuan, bJrate, bJDamage, sp.JYrate, sp.damageType, sp.em);
			break;
		}
		if (!flag && !SingletonMonoScope<PlayerManager>.Instance.CPNoBad && SubType == 0 && (sp.DebuffTime > 0f || sp.AttackSpeedCut > 0f || sp.MoveSpeedCut > 0f || sp.DotDamage > 0f) && sp.DotRate > 0f && (float)Random.Range(1, 100) < sp.DotRate)
		{
			Buff_CP buff_CP = new Buff_CP();
			buff_CP.type = 0;
			buff_CP.damageType = sp.damageType;
			buff_CP.BuffTime = sp.DebuffTime;
			buff_CP.De_ATSpeedCut = sp.AttackSpeedCut;
			buff_CP.De_MVSpeedCut = sp.MoveSpeedCut;
			buff_CP.DotChuan = sp.Chuan;
			buff_CP.DotDamage = sp.DotDamage;
			BuffCP.AddBuff(buff_CP);
		}
	}

	private bool IsCursedEnemyAttack(SkillOBJ_DT_SP sp)
	{
		if (sp != null && sp.indexType == 2)
		{
			if (!sp.DotZZ)
			{
				if (sp.em != null && sp.em.peo != null && sp.em.peo.DotEM != null)
				{
					return sp.em.peo.DotEM.GerDotZZ();
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public void EM_Set(SkillOBJ_DT_SP sp, float DotMulti, int SubType, bool Dot_Infect, int Dot_Infect_Layer, float UPDamage)
	{
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		instance.EnsurePlayerDotData();
		bool flag = BlockCompanionBadEffect(sp);
		int companionExtraDotLayer = GetCompanionExtraDotLayer(sp);
		ACT_DOT aCT_DOT = SingletonMonoScope<ACTbar>.Instance.GiveDot(sp.damageType);
		if (aCT_DOT != null)
		{
			float mSrate = (flag ? 0f : aCT_DOT.MSrate);
			float mSnumber = (flag ? 0f : aCT_DOT.MSnumber);
			switch (SubType)
			{
			case 0:
				if (sp.AttackType)
				{
					em.TakeDamage(sp.Damage + sp.Damage * UPDamage / 100f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, mSrate, mSnumber, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				else
				{
					em.TakeDamage((sp.Damage + sp.Damage * UPDamage / 100f) / 2f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, mSrate, mSnumber, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				break;
			case 1:
				if (sp.AttackTypeA)
				{
					em.TakeDamage(sp.DamageA + sp.DamageA * UPDamage / 100f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, mSrate, mSnumber, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				else
				{
					em.TakeDamage((sp.DamageA + sp.DamageA * UPDamage / 100f) / 2f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, mSrate, mSnumber, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				break;
			case 2:
				if (sp.AttackTypeB)
				{
					em.TakeDamage(sp.DamageB + sp.DamageB * UPDamage / 100f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, mSrate, mSnumber, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				else
				{
					em.TakeDamage((sp.DamageB + sp.DamageB * UPDamage / 100f) / 2f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, mSrate, mSnumber, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				break;
			case 3:
				em.TakeDamage((sp.Damage + sp.DamageB * UPDamage / 100f) / 4f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, mSrate, mSnumber, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				break;
			}
			if (!flag && instance.NoDot_BJD == 0 && SubType != 3)
			{
				if (Dot_Infect)
				{
					DotEM.AddDot(sp.damageType, aCT_DOT, Dot_Infect_Layer + companionExtraDotLayer);
				}
				else if ((float)Random.Range(1, 101) < DotMulti && (float)Random.Range(1, 101) < aCT_DOT.DOTrate)
				{
					if (instance.DOT[instance.GiveInt(sp.damageType)].All_LayerR > 0)
					{
						if (Random.Range(1, 101) < instance.DOT[instance.GiveInt(sp.damageType)].All_LayerR)
						{
							DotEM.AddDot(sp.damageType, aCT_DOT, aCT_DOT.Layer_Max);
						}
						else
						{
							DotEM.AddDot(sp.damageType, aCT_DOT, 1 + instance.DOT[instance.GiveInt(sp.damageType)].Every_Layer + companionExtraDotLayer);
						}
					}
					else
					{
						DotEM.AddDot(sp.damageType, aCT_DOT, 1 + instance.DOT[instance.GiveInt(sp.damageType)].Every_Layer + companionExtraDotLayer);
					}
				}
			}
		}
		else
		{
			switch (SubType)
			{
			case 0:
				if (sp.AttackType)
				{
					em.TakeDamage(sp.Damage, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, 0f, 0f, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				else
				{
					em.TakeDamage(sp.Damage / 2f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, 0f, 0f, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				break;
			case 1:
				if (sp.AttackTypeA)
				{
					em.TakeDamage(sp.DamageA, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, 0f, 0f, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				else
				{
					em.TakeDamage(sp.DamageA / 2f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, 0f, 0f, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				break;
			case 2:
				if (sp.AttackTypeB)
				{
					em.TakeDamage(sp.DamageB, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, 0f, 0f, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				else
				{
					em.TakeDamage(sp.DamageB / 2f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, 0f, 0f, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				}
				break;
			case 3:
				em.TakeDamage(sp.Damage / 4f, instance.GiveChuan(sp.damageType), sp.BJrate, sp.BJDamage, 0f, 0f, sp.JYrate, sp.damageType, sp.indexType, sp.pl, sp.cp, sp);
				break;
			}
		}
		TrySkillMSDead(sp, flag);
		if (flag)
		{
			return;
		}
		if (SubType != 0)
		{
			_ = SubType - 1;
			_ = 1;
		}
		else if (sp.DebuffTime > 0f || sp.AttackSpeedCut > 0f || sp.MoveSpeedCut > 0f || sp.AntiCut > 0f)
		{
			if (sp.AttackType)
			{
				Buff_Enemy buff_Enemy = new Buff_Enemy();
				buff_Enemy.type = 0;
				buff_Enemy.damageType = sp.damageType;
				buff_Enemy.BuffTime = sp.DebuffTime;
				buff_Enemy.ATSpeedCut = sp.AttackSpeedCut;
				buff_Enemy.MVSpeedCut = sp.MoveSpeedCut;
				buff_Enemy.AntiCut = sp.AntiCut;
				BuffEM.AddBuff(buff_Enemy);
			}
			else if (Random.Range(0, 101) < 30)
			{
				Buff_Enemy buff_Enemy2 = new Buff_Enemy();
				buff_Enemy2.type = 0;
				buff_Enemy2.damageType = sp.damageType;
				buff_Enemy2.BuffTime = sp.DebuffTime;
				buff_Enemy2.ATSpeedCut = sp.AttackSpeedCut;
				buff_Enemy2.MVSpeedCut = sp.MoveSpeedCut;
				buff_Enemy2.AntiCut = sp.AntiCut / 2f;
				BuffEM.AddBuff(buff_Enemy2);
			}
		}
	}

	private static bool BlockCompanionBadEffect(SkillOBJ_DT_SP sp)
	{
		if (sp != null && sp.indexType == 1)
		{
			return sp.BJ_NoDot;
		}
		return false;
	}

	private void TrySkillMSDead(SkillOBJ_DT_SP sp, bool blockBadEffect)
	{
		if (!blockBadEffect && !(sp == null) && sp.MS_Dead > 0 && !(em == null) && !em.IsDpsTarget && em.Quality < 2 && em.IsAlive && (!(sp.skillName == "FireBall") || sp.TryClaimMSDeadTarget(em)))
		{
			float num = Mathf.Clamp01((float)sp.MS_Dead / 100f);
			if (Random.value < num)
			{
				em.HealthStat.SetCurrent(0f);
			}
		}
	}

	private static int GetCompanionExtraDotLayer(SkillOBJ_DT_SP sp)
	{
		if (sp == null || sp.indexType != 1)
		{
			return 0;
		}
		return Mathf.Max(0, sp.AT_DotLayer);
	}
}
