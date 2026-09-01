using Entity.Comp.CompanionAI;
using UnityEngine;

namespace Entity.Comp.CompState.State_A;

public class CompState_A_Attack : CompStateBase_A
{
	private float jsTime;

	public override CompStateType Type => CompStateType.Attack;

	public CompState_A_Attack(CompA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.companion.ChangeIntent(Companion.CompanionIntentState.Combat);
		go.changeST(3);
		jsTime = 0f;
	}

	public override void OnExit()
	{
		go.companion.IsAttack = false;
		go.companion.IsSkill = false;
	}

	public override void OnUpdate()
	{
		if (!go.companion.IsAlive)
		{
			Change(CompStateType.Die);
		}
		else if (go.companion.IsYun)
		{
			Change(CompStateType.Hurt);
		}
		else if (go.companion.TargetList.Count > 0)
		{
			jsTime += Time.deltaTime;
			if (jsTime > 0.1f && (bool)go.tar)
			{
				go.tar.transform.position = go.companion.ATTarget.transform.position;
			}
		}
	}
}
