using UnityEngine;

namespace Entity.Enemies.EnemyState.State_A.States;

public class EnemyState_A_Return : EnemyStateBase_A
{
	private Vector3 returnPos;

	public override EnemyStateType Type => EnemyStateType.Return;

	public EnemyState_A_Return(EnemyA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			go.changeST(1);
			go.em.CanSO_Idle = true;
			if ((bool)go.em.emitter)
			{
				go.em.emitter.Play();
			}
			Vector2 vector = Random.insideUnitCircle * 1f;
			Vector3 position = new Vector3(go.em.SpawnPos.x + vector.x, go.em.SpawnPos.y + vector.y, go.em.SpawnPos.z);
			if ((bool)AstarPath.active)
			{
				returnPos = AstarPath.active.GetNearest(position).position;
			}
			else
			{
				returnPos = position;
			}
			if ((bool)go.em.BrainMovePoint)
			{
				go.em.BrainMovePoint.position = returnPos;
			}
			else
			{
				Change(EnemyStateType.Idle);
			}
		}
	}

	public override void OnExit()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.CanSO_Idle = false;
			if ((bool)go.em.emitter)
			{
				go.em.emitter.Stop();
			}
		}
	}

	public override void OnUpdate()
	{
		if ((bool)go && (bool)go.em)
		{
			if (!go.em.IsAlive)
			{
				Change(EnemyStateType.Die);
			}
			else if (go.em.hasTarget)
			{
				Change(EnemyStateType.Walk);
			}
			else if (Vector3.Distance(go.transform.position, returnPos) <= 0.3f)
			{
				Change(EnemyStateType.Idle);
			}
		}
	}
}
