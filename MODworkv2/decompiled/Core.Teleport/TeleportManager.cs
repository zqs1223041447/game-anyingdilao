using System.Collections.Generic;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Scenes;
using UnityEngine;

namespace Core.Teleport;

public class TeleportManager : SingletonMonoScope<TeleportManager>
{
	private readonly List<LevelPoint> currentPoints = new List<LevelPoint>();

	private readonly List<TeleportStation> currentStations = new List<TeleportStation>();

	private Vector3? _challengeEnterPos;

	private readonly List<Vector3> _mijingEnterPositions = new List<Vector3>();

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	public void Register(LevelPoint point)
	{
		if (!currentPoints.Contains(point))
		{
			currentPoints.Add(point);
		}
	}

	public void Unregister(LevelPoint point)
	{
		currentPoints.Remove(point);
	}

	public void Register(TeleportStation station)
	{
		if (!currentStations.Contains(station))
		{
			currentStations.Add(station);
		}
	}

	public void Unregister(TeleportStation station)
	{
		currentStations.Remove(station);
	}

	public void Clear()
	{
		currentPoints.Clear();
		currentStations.Clear();
		ResetChallengeEnter();
		ResetMijingEnter();
	}

	public static TeleportType GetTargetType(TeleportType? from)
	{
		return from switch
		{
			TeleportType.Enter => TeleportType.Exit, 
			TeleportType.Exit => TeleportType.Enter, 
			TeleportType.Optional_Enter => TeleportType.Enter, 
			_ => TeleportType.Enter, 
		};
	}

	public Transform ResolveSpawnPoint(string fromId, TeleportType? fromType)
	{
		if (LevelManager.GetIsOptionalById(fromId))
		{
			TeleportType teleportType = TeleportType.Optional_Enter;
			foreach (LevelPoint currentPoint in currentPoints)
			{
				if (currentPoint.CurrentType == teleportType && currentPoint.targetLevelId == fromId)
				{
					return currentPoint.playerPos;
				}
			}
		}
		else
		{
			TeleportType targetType = GetTargetType(fromType);
			foreach (LevelPoint currentPoint2 in currentPoints)
			{
				if (currentPoint2.CurrentType == targetType)
				{
					return currentPoint2.playerPos;
				}
			}
		}
		return null;
	}

	public bool TryResolveStationSpawnPoint(out Vector3 pos)
	{
		int num = currentStations.Count - 1;
		while (num >= 0)
		{
			TeleportStation teleportStation = currentStations[num];
			if (!teleportStation)
			{
				currentStations.RemoveAt(num);
				num--;
				continue;
			}
			pos = teleportStation.transform.position;
			return true;
		}
		pos = default(Vector3);
		return false;
	}

	public void RegisterChallengeEnter(Vector3 pos, Object sender)
	{
		if (_challengeEnterPos.HasValue)
		{
			LogUtil.Error("TeleportManager", "检测到多个特殊关卡入口！来源对象：" + sender.name);
		}
		else
		{
			_challengeEnterPos = pos;
		}
	}

	public bool TryGetChallengeEnterPos(out Vector3 pos)
	{
		if (_challengeEnterPos.HasValue)
		{
			pos = _challengeEnterPos.Value;
			return true;
		}
		pos = default(Vector3);
		return false;
	}

	public void ResetChallengeEnter()
	{
		_challengeEnterPos = null;
	}

	public void RecordChallengeContext(string levelId, Vector3 worldPos)
	{
		ChallengeContext.FromLevelId = levelId;
		ChallengeContext.FromWorldPos = worldPos;
	}

	public void ResetChallengeContext()
	{
		ChallengeContext.FromLevelId = "";
		ChallengeContext.FromWorldPos = Vector3.zero;
	}

	public void BackFromChallenge()
	{
		if (ChallengeContext.FromLevelId == "")
		{
			LogUtil.Warn("TeleportManager", "未记录 ChallengeContext，返回失败");
			return;
		}
		if (ChallengeContext.FromLevelId == "Home")
		{
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
			{
				Reason = HomePlayerSpawnReason.BackFromChallenge,
				BackFromChallengePos = ChallengeContext.FromWorldPos
			});
			SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade).Forget();
		}
		else
		{
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.BackFromChallenge,
				BackFromChallengePos = ChallengeContext.FromWorldPos
			});
			SceneLoadManager.LoadLevelScene(ChallengeContext.FromLevelId, SceneTransitionMode.Fade).Forget();
		}
		ResetChallengeContext();
	}

	public void RegisterMijingEnter(Vector3 pos, Object sender)
	{
		_mijingEnterPositions.Add(pos);
	}

	public bool TryGetMijingEnterPos(out Vector3 pos)
	{
		if (_mijingEnterPositions.Count > 0)
		{
			pos = _mijingEnterPositions[Random.Range(0, _mijingEnterPositions.Count)];
			return true;
		}
		pos = default(Vector3);
		return false;
	}

	public void ResetMijingEnter()
	{
		_mijingEnterPositions.Clear();
	}
}
