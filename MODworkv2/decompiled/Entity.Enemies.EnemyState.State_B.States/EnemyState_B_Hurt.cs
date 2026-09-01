using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Entity.Enemies.EnemyState.State_B.States;

public class EnemyState_B_Hurt : EnemyStateBase_B
{
	private float timeJiTui;

	private float speedTmp;

	public override EnemyStateType Type => EnemyStateType.Hurt;

	public EnemyState_B_Hurt(EnemyB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			go.changeST(4);
			if (!go.em.IS_Frozen && (bool)go.tar && (bool)go.tar2 && go.tar.transform != go.em.playerManager.transform)
			{
				go.tar.transform.position = go.tar2.transform.position;
			}
			go.em.IsAttack = false;
			timeJiTui = 0f;
			speedTmp = go.em.TuiSpeed;
		}
	}

	public override void OnExit()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.IsYun = false;
			timeJiTui = 0f;
		}
	}

	public override void OnUpdate()
	{
		if (!go || !go.em)
		{
			return;
		}
		if (!go.em.IsAlive)
		{
			Change(EnemyStateType.Die);
		}
		else if (go.em.IsYun)
		{
			timeJiTui += Time.deltaTime;
			if (timeJiTui < 0.1f)
			{
				speedTmp = Mathf.Lerp(speedTmp, 0f, Time.deltaTime * 10f);
				Vector3 normalized = (go.em.transform.position - SingletonMonoScope<PlayerManager>.Instance.transform.position).normalized;
				go.em.transform.Translate(normalized * (speedTmp * Time.deltaTime));
			}
		}
		else
		{
			Change(EnemyStateType.Idle);
		}
	}
}
