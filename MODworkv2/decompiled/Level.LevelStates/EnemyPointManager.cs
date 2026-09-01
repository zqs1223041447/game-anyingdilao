using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Level.LevelStates;

public class EnemyPointManager : SingletonMonoScene<EnemyPointManager>
{
	private readonly HashSet<EnemyPoint> points = new HashSet<EnemyPoint>();

	protected override void Awake()
	{
		base.Awake();
		points.Clear();
	}

	public void Register(EnemyPoint p)
	{
		if ((bool)p)
		{
			points.Add(p);
		}
	}

	public void Unregister(EnemyPoint p)
	{
		if ((bool)p)
		{
			points.Remove(p);
		}
	}

	public void FlushAll()
	{
		foreach (EnemyPoint point in points)
		{
			if ((bool)point)
			{
				point.FlushActiveEnemiesToState();
			}
		}
	}

	public void PrewarmForTeleport(Vector3 targetPos, float unloadGraceTime)
	{
		foreach (EnemyPoint point in points)
		{
			if ((bool)point)
			{
				point.PrewarmForTeleport(targetPos, unloadGraceTime);
			}
		}
	}

	public void ClearAllRefs()
	{
		points.Clear();
	}
}
