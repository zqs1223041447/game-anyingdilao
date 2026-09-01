using System.Collections;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Level.LevelStates;
using Level.StateData.LevelStates;
using UnityEngine;
using Utils;

public class ChestPoint : MonoBehaviour
{
	private string RuntimeId;

	public bool Shu;

	private LevelManager LV;

	private bool initialized;

	private ChestPointLevelState pointState;

	private void Awake()
	{
		LV = SingletonMonoScope<LevelManager>.Instance;
		RuntimeId = "ChestPoint_Level" + LevelManager.GetCurLevel() + "_" + RuntimeIdUtil.GenerateByIndex(base.transform);
	}

	private void OnEnable()
	{
		StartCoroutine(InitNextFrame());
	}

	private IEnumerator InitNextFrame()
	{
		yield return null;
		InitChestPoint();
	}

	private void InitChestPoint()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		LevelState currentLevelState = SingletonMonoGlobal<StateDataManager>.Instance.GetCurrentLevelState();
		bool flag = LevelManager.ShouldPersistLevelState(LevelManager.GetCurLevel());
		if (currentLevelState != null && flag)
		{
			if (currentLevelState.ChestPoints == null)
			{
				currentLevelState.ChestPoints = new Dictionary<string, ChestPointLevelState>();
			}
			if (!currentLevelState.ChestPoints.TryGetValue(RuntimeId, out pointState))
			{
				pointState = new ChestPointLevelState();
				pointState.RuntimeId = RuntimeId;
				currentLevelState.ChestPoints.Add(RuntimeId, pointState);
				SpawnRandom(pointState);
			}
			else
			{
				RestoreSpawn(pointState);
			}
		}
		else
		{
			pointState = new ChestPointLevelState();
			pointState.RuntimeId = RuntimeId;
			SpawnRandom(pointState);
		}
	}

	private void SpawnRandom(ChestPointLevelState levelState)
	{
		List<ChestSpawnInfo> collection = LV.CreateChest(RuntimeId, base.transform, Shu);
		levelState.Chests.AddRange(collection);
	}

	private void RestoreSpawn(ChestPointLevelState levelState)
	{
		foreach (ChestSpawnInfo chest2 in levelState.Chests)
		{
			Chest chest = LV.SetChest(chest2.ChestIndex, chest2.Position);
			chest.InitRuntimeId(chest2.RuntimeId);
			chest.RestoreState();
		}
	}
}
