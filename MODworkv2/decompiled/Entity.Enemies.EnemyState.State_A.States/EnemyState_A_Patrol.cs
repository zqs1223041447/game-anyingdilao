using UnityEngine;

namespace Entity.Enemies.EnemyState.State_A.States;

public class EnemyState_A_Patrol : EnemyStateBase_A
{
	private const float PatrolRadius = 1.5f;

	private float patrolDuration;

	private float timer;

	public override EnemyStateType Type => EnemyStateType.Patrol;

	public EnemyState_A_Patrol(EnemyA go)
		: base(go)
	{
	}

	public override void OnEnter()
	{
		if ((bool)go && (bool)go.em)
		{
			timer = 0f;
			patrolDuration = Random.Range(1f, 2f);
			go.em.CanSO_Idle = true;
			Vector2 vector = Random.insideUnitCircle * 1.5f;
			Vector3 vector2 = new Vector3(go.em.SpawnPos.x + vector.x, go.em.SpawnPos.y + vector.y, go.em.SpawnPos.z);
			Vector3 position = vector2;
			if ((bool)AstarPath.active)
			{
				position = AstarPath.active.GetNearest(vector2).position;
			}
			go.em.BrainMovePoint.position = position;
			go.changeST(4);
		}
	}

	public override void OnExit()
	{
		if ((bool)go && (bool)go.em)
		{
			timer = 0f;
			go.em.CanSO_Idle = false;
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
			return;
		}
		if (go.em.hasTarget && (bool)go.em.playerManager && go.em.playerManager.IsAlive)
		{
			Change(EnemyStateType.Walk);
			return;
		}
		timer += Time.deltaTime;
		if (!go.em.BrainMovePoint)
		{
			if (timer >= patrolDuration)
			{
				Change(EnemyStateType.Idle);
			}
		}
		else if (Vector3.Distance(go.transform.position, go.em.BrainMovePoint.transform.position) <= 0.25f || timer >= patrolDuration)
		{
			Change(EnemyStateType.Idle);
		}
	}
}
