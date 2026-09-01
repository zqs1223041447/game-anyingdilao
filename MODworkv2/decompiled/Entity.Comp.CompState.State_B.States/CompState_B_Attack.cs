using Entity.Comp.CompanionAI;
using UnityEngine;

namespace Entity.Comp.CompState.State_B.States;

public class CompState_B_Attack : CompStateBase_B
{
	private float exitDelay;

	public override CompStateType Type => CompStateType.Attack;

	public CompState_B_Attack(CompB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		exitDelay = 0f;
		go.changeST(3);
		go.JStime = 0f;
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
			return;
		}
		exitDelay += Time.deltaTime;
		if (exitDelay >= 0.1f)
		{
			go.companion.NotifyActionFinished();
			Change(CompStateType.Idle);
		}
	}
}
