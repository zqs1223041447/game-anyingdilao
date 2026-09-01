using UnityEngine;

namespace Entity.Enemies.EnemyState.State_B.States;

public class EnemyState_B_Return : EnemyStateBase_B
{
	private Vector3 returnPos;

	public override EnemyStateType Type => EnemyStateType.Return;

	public EnemyState_B_Return(EnemyB go)
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
			Vector3 vector = (go.em.OwnerPoint ? go.em.OwnerPoint.transform.position : go.em.SpawnPos);
			Vector2 vector2 = Random.insideUnitCircle * 1f;
			Vector3 position = new Vector3(vector.x + vector2.x, vector.y + vector2.y, vector.z);
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
