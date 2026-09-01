using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class XJL : MonoBehaviour
{
	public int GlobleID;

	public int XJL_type;

	[HideInInspector]
	public int MainEL;

	[HideInInspector]
	public DamageType damageType;

	[HideInInspector]
	public float PickRange;

	[HideInInspector]
	public float ATRange;

	[HideInInspector]
	public float PickJG;

	[HideInInspector]
	public float UseSKTime;

	[HideInInspector]
	public float Movespeed;

	[HideInInspector]
	public float Number;

	private float timeA;

	private float timeB;

	private float timeC;

	public Collider2D[] DPIT = new Collider2D[2];

	private readonly List<Enemy> enemyBuffer = new List<Enemy>(8);

	private readonly List<Companion> auraCompanions = new List<Companion>(8);

	private PlayerManager PL;

	public XJL_FSQ father;

	private XJL_Stat st;

	public bool CanMV;

	public Transform tar;

	private bool initialized;

	public float UseSKTime_Last => Mathf.Max(0.1f, UseSKTime - UseSKTime * PL.XJL_UseSKTime / 100f);

	private void Awake()
	{
		PL = SingletonMonoScope<PlayerManager>.Instance;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		ChangeState(new XJL_idle());
		CanMV = false;
		initialized = false;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
	}

	private void OnDisable()
	{
		ClearRuntimeEffects();
	}

	private void Update()
	{
		if (!CanMV || !PL || !father)
		{
			return;
		}
		RefreshDropTarget();
		st.Update();
		timeB += Time.deltaTime;
		if (timeB >= UseSKTime_Last)
		{
			switch (XJL_type)
			{
			case 3:
				HealPlayerOrCompanion();
				break;
			case 5:
				CurseNearbyEnemy();
				break;
			}
			timeB = 0f;
		}
		timeC += Time.deltaTime;
		if (timeC >= 1f)
		{
			switch (XJL_type)
			{
			case 1:
				RefreshCompanionAura();
				break;
			case 4:
				RestorePlayerMana();
				break;
			}
			timeC = 0f;
		}
		timeA += Time.deltaTime;
		if (timeA >= PickJG)
		{
			int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, PickRange, DPIT, LayerMask.GetMask("AutoPick"));
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					DropItemController component = DPIT[i].GetComponent<DropItemController>();
					if ((bool)component && component.CanAutoPick && SingletonMonoScope<InventoryManager>.HasInstance)
					{
						SingletonMonoScope<InventoryManager>.Instance.AutoPickUpByXJL(component);
					}
					DPIT[i] = null;
				}
			}
			RefreshDropTarget();
			timeA = 0f;
		}
		RefreshDropTarget();
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
		if (father != null)
		{
			return PL != null;
		}
		return false;
	}

	public void SetStart()
	{
		CanMV = true;
	}

	public bool HasValidDropTarget()
	{
		if ((bool)father)
		{
			return father.IsValidDropTarget(tar);
		}
		return false;
	}

	public void SetDropTarget(DropItemController target)
	{
		tar = (target ? target.transform : null);
	}

	private void RefreshDropTarget()
	{
		if (!father || father.DropOBJ.Count < 1)
		{
			tar = null;
		}
		else
		{
			if (father.IsValidDropTarget(tar))
			{
				return;
			}
			int num = Mathf.Min(3, father.DropOBJ.Count);
			int num2 = Random.Range(0, num);
			for (int i = 0; i < num; i++)
			{
				DropItemController dropItemController = father.DropOBJ[(num2 + i) % num];
				if ((bool)dropItemController && father.IsValidDropTarget(dropItemController.transform))
				{
					tar = dropItemController.transform;
					return;
				}
			}
			tar = null;
		}
	}

	public void ClearRuntimeEffects()
	{
		ClearCompanionAura();
	}

	private void HealPlayerOrCompanion()
	{
		float num = ((Number > 0f) ? Number : 10f);
		float num2 = num * 2f;
		if ((bool)PL.HealStat && PL.HealStat.Cur < PL.HealStat.Max)
		{
			PL.HealStat.Cur += PL.HealStat.Max * num / 100f;
			return;
		}
		Companion lowestHealthCompanionInRange = GetLowestHealthCompanionInRange();
		if ((bool)lowestHealthCompanionInRange && lowestHealthCompanionInRange.HealthStat != null && lowestHealthCompanionInRange.HealthStat.CurrentValue < lowestHealthCompanionInRange.HealthStat.MaxValue)
		{
			lowestHealthCompanionInRange.HealthStat.SetCurrent(lowestHealthCompanionInRange.HealthStat.CurrentValue + lowestHealthCompanionInRange.HealthStat.MaxValue * num2 / 100f);
		}
	}

	private void RestorePlayerMana()
	{
		float num = ((Number > 0f) ? Number : 1f);
		if ((bool)PL.ManaStat && PL.ManaStat.Cur < PL.ManaStat.Max)
		{
			PL.ManaStat.Cur += PL.ManaStat.Max * num / 100f;
		}
	}

	private void CurseNearbyEnemy()
	{
		if (!PL)
		{
			return;
		}
		PL.CollectEnemiesInRange(ATRange, enemyBuffer, onlyNormalEnemy: false);
		if (enemyBuffer.Count > 0)
		{
			Enemy enemy = enemyBuffer[Random.Range(0, enemyBuffer.Count)];
			if ((bool)enemy && !(enemy.peo == null) && !(enemy.peo.BuffEM == null))
			{
				Buff_Enemy buff_Enemy = new Buff_Enemy();
				buff_Enemy.type = 0;
				buff_Enemy.BuffTime = 3f;
				buff_Enemy.damageType = damageType;
				buff_Enemy.HurtDamageAdd = ((Number > 0f) ? Number : 10f);
				enemy.peo.BuffEM.AddBuff(buff_Enemy);
			}
		}
	}

	private void RefreshCompanionAura()
	{
		for (int num = auraCompanions.Count - 1; num >= 0; num--)
		{
			Companion comp = auraCompanions[num];
			if (!IsValidAuraTarget(comp))
			{
				RemoveCompanionAuraAt(num);
			}
		}
		if (!SingletonMonoScope<ACTbar>.HasInstance || SingletonMonoScope<ACTbar>.Instance.actListSkill == null)
		{
			return;
		}
		for (int i = 0; i < SingletonMonoScope<ACTbar>.Instance.actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = SingletonMonoScope<ACTbar>.Instance.actListSkill[i];
			if (!aCTListSkillBT || aCTListSkillBT.cpList == null)
			{
				continue;
			}
			for (int j = 0; j < aCTListSkillBT.cpList.Count; j++)
			{
				Companion companion = aCTListSkillBT.cpList[j];
				if (IsValidAuraTarget(companion) && !auraCompanions.Contains(companion))
				{
					companion.Damage_Bei += Number;
					auraCompanions.Add(companion);
				}
			}
		}
	}

	private bool IsValidAuraTarget(Companion comp)
	{
		if ((bool)comp && !comp.IsDead && comp.IsReady)
		{
			return Vector2.Distance(base.transform.position, comp.transform.position) <= ATRange;
		}
		return false;
	}

	private void ClearCompanionAura()
	{
		for (int num = auraCompanions.Count - 1; num >= 0; num--)
		{
			RemoveCompanionAuraAt(num);
		}
	}

	private void RemoveCompanionAuraAt(int index)
	{
		Companion companion = auraCompanions[index];
		if ((bool)companion)
		{
			companion.Damage_Bei -= Number;
		}
		auraCompanions.RemoveAt(index);
	}

	private Companion GetLowestHealthCompanionInRange()
	{
		Companion result = null;
		float num = 1f;
		if (!SingletonMonoScope<ACTbar>.HasInstance || SingletonMonoScope<ACTbar>.Instance.actListSkill == null)
		{
			return null;
		}
		for (int i = 0; i < SingletonMonoScope<ACTbar>.Instance.actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = SingletonMonoScope<ACTbar>.Instance.actListSkill[i];
			if (!aCTListSkillBT || aCTListSkillBT.cpList == null)
			{
				continue;
			}
			for (int j = 0; j < aCTListSkillBT.cpList.Count; j++)
			{
				Companion companion = aCTListSkillBT.cpList[j];
				if (IsValidAuraTarget(companion) && !(companion.HealthStat == null) && !(companion.HealthStat.MaxValue <= 0f))
				{
					float num2 = companion.HealthStat.CurrentValue / companion.HealthStat.MaxValue;
					if (num2 < num)
					{
						num = num2;
						result = companion;
					}
				}
			}
		}
		return result;
	}

	public void ChangeState(XJL_Stat Astat)
	{
		st?.Exit();
		st = Astat;
		st.Enter(this);
	}
}
