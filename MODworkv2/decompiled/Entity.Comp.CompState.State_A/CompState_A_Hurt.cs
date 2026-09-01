using Entity.Comp.CompanionAI;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Entity.Comp.CompState.State_A;

public class CompState_A_Hurt : CompStateBase_A
{
	private float timeJiTui;

	private float speedTmp;

	public override CompStateType Type => CompStateType.Hurt;

	public CompState_A_Hurt(CompA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		go.changeST(5);
		if ((bool)go.tar)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
		go.companion.IsAttack = false;
		go.companion.IsSkill = false;
		timeJiTui = 0f;
		speedTmp = 6f;
	}

	public override void OnExit()
	{
		go.companion.IsYun = false;
	}

	public override void OnUpdate()
	{
		if (go.companion.IsAlive)
		{
			if (go.companion.IsYun)
			{
				timeJiTui += Time.deltaTime;
				if (timeJiTui < 0.1f)
				{
					speedTmp = Mathf.Lerp(speedTmp, 0f, Time.deltaTime * 10f);
					go.companion.transform.Translate((go.companion.transform.position - SingletonMonoScope<PlayerManager>.Instance.transform.position).normalized * (speedTmp * Time.deltaTime));
				}
			}
		}
		else
		{
			Change(CompStateType.Die);
		}
	}
}
