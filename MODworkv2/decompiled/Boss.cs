using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Level.LevelStates;
using UnityEngine;

public class Boss : MonoBehaviour
{
	[HideInInspector]
	public Enemy em;

	public List<EM_Skill_SP> AT;

	public List<EM_Skill_SP> SK;

	public EM_Skill_CP SKC;

	[HideInInspector]
	public int SK_Cur_IndexA;

	[HideInInspector]
	public int SK_Cur_IndexB;

	[HideInInspector]
	public List<string> SO_Idle = new List<string>();

	[HideInInspector]
	public string SO_AttackA;

	[HideInInspector]
	public string SO_SayA;

	[HideInInspector]
	public string SO_AttackB;

	[HideInInspector]
	public string SO_SayB;

	[HideInInspector]
	public string SO_AttackC;

	[HideInInspector]
	public string SO_SayC;

	[HideInInspector]
	public string SO_AttackD;

	[HideInInspector]
	public string SO_SayD;

	[HideInInspector]
	public string SO_AttackE;

	[HideInInspector]
	public string SO_SayE;

	[HideInInspector]
	public string SO_ChongStart;

	[HideInInspector]
	public string SO_ChongEnd;

	[HideInInspector]
	public string SO_Jump;

	[HideInInspector]
	public string SO_Land;

	[HideInInspector]
	public string SO_SPC1;

	[HideInInspector]
	public string SO_SPC2;

	[HideInInspector]
	public string SO_SPC3;

	private bool registered;

	public bool canAttack => em.path.remainingDistance < Range_AT;

	public bool AttackLost => Vector2.Distance(em.transform.position, em.MVTarget.transform.position) > Range_AT + Range_AT_Hurt;

	public float Range_AT_Hurt
	{
		get
		{
			switch (SK_Cur_IndexA)
			{
			case 0:
				if (AT.Count > 0)
				{
					return AT[SK_Cur_IndexB].Range_Hurt;
				}
				break;
			case 1:
				if (SK.Count > 0)
				{
					return SK[SK_Cur_IndexB].Range_Hurt;
				}
				break;
			}
			return 0.5f;
		}
	}

	public float Range_AT
	{
		get
		{
			switch (SK_Cur_IndexA)
			{
			case 0:
				if (AT.Count > 0)
				{
					return AT[SK_Cur_IndexB].Distance;
				}
				break;
			case 1:
				if (SK.Count > 0)
				{
					return SK[SK_Cur_IndexB].Distance;
				}
				break;
			case 2:
				if (SKC != null)
				{
					return em.Range_Cur;
				}
				break;
			}
			return 1f;
		}
	}

	private void OnEnable()
	{
		TryRegister();
	}

	private void OnDisable()
	{
		TryUnregister();
	}

	private void TryRegister()
	{
		if (!registered && SingletonMonoScene<BossLevelManager>.HasInstance && (!SingletonMonoScope<LevelManager>.HasInstance || LevelManager.GetIsBoss()))
		{
			SingletonMonoScene<BossLevelManager>.Instance.RegisterBoss(this);
			registered = true;
		}
	}

	private void TryUnregister()
	{
		if (registered && SingletonMonoScene<BossLevelManager>.HasInstance)
		{
			SingletonMonoScene<BossLevelManager>.Instance.UnregisterBoss(this);
			registered = false;
		}
	}

	public void BossDie()
	{
		TryUnregister();
	}
}
