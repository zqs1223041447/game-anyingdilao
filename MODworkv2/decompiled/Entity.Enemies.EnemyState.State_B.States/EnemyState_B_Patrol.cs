using UnityEngine;

namespace Entity.Enemies.EnemyState.State_B.States;

public class EnemyState_B_Patrol : EnemyStateBase_B
{
	public override EnemyStateType Type => EnemyStateType.Patrol;

	public EnemyState_B_Patrol(EnemyB go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.CanSO_Idle = true;
			Vector2 vector = Random.insideUnitCircle * 1.5f;
			Vector3 vector2 = new Vector3(go.em.SpawnPos.x + vector.x, go.em.SpawnPos.y + vector.y, go.em.SpawnPos.z);
			Vector3 position = vector2;
			if ((bool)AstarPath.active)
			{
				position = AstarPath.active.GetNearest(vector2).position;
			}
			go.em.BrainMovePoint.position = position;
			go.changeST(5);
		}
	}

	public override void OnExit()
	{
		if ((bool)go && (bool)go.em)
		{
			go.em.CanSO_Idle = false;
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
			else if (go.em.IsYun)
			{
				Change(EnemyStateType.Hurt);
			}
			else if (go.em.hasTarget && (bool)go.em.playerManager && go.em.playerManager.IsAlive)
			{
				Change(EnemyStateType.Walk);
			}
			else if ((bool)go.em.BrainMovePoint && Vector3.Distance(go.transform.position, go.em.BrainMovePoint.position) <= 0.3f)
			{
				Change(EnemyStateType.Idle);
			}
		}
	}
}
