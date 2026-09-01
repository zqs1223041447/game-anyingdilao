using FMODUnity;
using UnityEngine;

namespace Entity.Enemies.EnemyState.State_A.States;

public class EnemyState_A_Die : EnemyStateBase_A
{
	public override EnemyStateType Type => EnemyStateType.Die;

	public EnemyState_A_Die(EnemyA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.CanSO_Idle = false;
			if ((bool)go.em.emitter)
			{
				go.em.emitter.Stop();
			}
			go.em.canvas.alpha = 0f;
			if (Random.Range(0, 101) < go.em.SO_DieRate)
			{
				RuntimeManager.PlayOneShot(go.em.SO_Die, go.em.yao.transform.position);
			}
			for (int i = 0; i < go.em.Spirit.Length; i++)
			{
				go.em.Spirit[i].gameObject.SetActive(value: false);
			}
			go.em.OnDie();
			go.changeST(3);
		}
	}

	public override void OnExit()
	{
	}

	public override void OnUpdate()
	{
	}
}
