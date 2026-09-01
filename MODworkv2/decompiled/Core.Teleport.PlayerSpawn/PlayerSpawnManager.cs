using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Core.Teleport.PlayerSpawn;

public class PlayerSpawnManager : SingletonMonoGlobal<PlayerSpawnManager>
{
	private HomePlayerSpawnRequest _homeRequest;

	private LevelPlayerSpawnRequest _levelRequest;

	public void SetHomeRequest(HomePlayerSpawnRequest req)
	{
		_homeRequest = req;
	}

	public HomePlayerSpawnRequest PeekHomeRequest()
	{
		return _homeRequest;
	}

	public void SetLevelRequest(LevelPlayerSpawnRequest req)
	{
		req.reqFromLevelId = LevelManager.GetCurLevel();
		_levelRequest = req;
	}

	public void ClearAllRequest()
	{
		_homeRequest = null;
		_levelRequest = null;
	}

	public Vector3 ResolveForHome()
	{
		if (_homeRequest == null)
		{
			LogUtil.Info("PlayerSpawnManager", "SpawnRequest 为空，使用默认主城出生点");
			return GetHomeDefaultSpawn();
		}
		HomePlayerSpawnRequest homeRequest = _homeRequest;
		switch (homeRequest.Reason)
		{
		case HomePlayerSpawnReason.ReturnFromHomePortal:
			return GameManager.GetPortalPos();
		case HomePlayerSpawnReason.ReturnFromChapter:
			return SingletonMonoScene<HomeSceneManager>.Instance.FindStationByChapter(homeRequest.FromChapterId);
		case HomePlayerSpawnReason.HomeDefault:
			return GetHomeDefaultSpawn();
		case HomePlayerSpawnReason.BackFromChallenge:
			if (homeRequest.BackFromChallengePos.HasValue)
			{
				return homeRequest.BackFromChallengePos.Value;
			}
			return GameManager.GetPortalPos();
		case HomePlayerSpawnReason.BackFromMijing:
			return GameManager.GetMijingPos();
		default:
			return Vector3.zero;
		}
	}

	public Vector3 ResolveForLevel()
	{
		if (_levelRequest == null)
		{
			LogUtil.Warn("PlayerSpawnManager", "SpawnRequest 为空，使用关卡初始出生点");
			return GetLevelDefaultSpawn();
		}
		LevelPlayerSpawnRequest levelRequest = _levelRequest;
		switch (levelRequest.Reason)
		{
		case LevelPlayerSpawnReason.EnterFromTeleport:
		{
			if (levelRequest.FromTeleportStation && !LevelManager.GetIsCurChapterFirst() && SingletonMonoScope<TeleportManager>.Instance.TryResolveStationSpawnPoint(out var pos2))
			{
				return pos2;
			}
			Transform transform = SingletonMonoScope<TeleportManager>.Instance.ResolveSpawnPoint(levelRequest.reqFromLevelId, levelRequest.FromTeleportType);
			if ((bool)transform)
			{
				return transform.position;
			}
			return GetLevelDefaultSpawn();
		}
		case LevelPlayerSpawnReason.BackFromHome:
			if (levelRequest.BackFromHomePos.HasValue)
			{
				return levelRequest.BackFromHomePos.Value;
			}
			return GetLevelDefaultSpawn();
		case LevelPlayerSpawnReason.EnterFromChallenge:
		{
			if (SingletonMonoScope<TeleportManager>.Instance.TryGetChallengeEnterPos(out var pos3))
			{
				SingletonMonoScope<TeleportManager>.Instance.ResetChallengeEnter();
				return pos3;
			}
			LogUtil.Warn("PlayerSpawnManager", "未注册 Challenge 入口，使用默认出生点");
			return GetLevelDefaultSpawn();
		}
		case LevelPlayerSpawnReason.BackFromChallenge:
			if (levelRequest.BackFromChallengePos.HasValue)
			{
				return levelRequest.BackFromChallengePos.Value;
			}
			return GetLevelDefaultSpawn();
		case LevelPlayerSpawnReason.EnterFromMijing:
		{
			if (SingletonMonoScope<TeleportManager>.Instance.TryGetMijingEnterPos(out var pos))
			{
				SingletonMonoScope<TeleportManager>.Instance.ResetMijingEnter();
				return pos;
			}
			LogUtil.Error("秘境入口未找到 或者 秘境入口未注册完毕");
			LogUtil.Warn("PlayerSpawnManager", "未注册 Mijing 入口，使用默认出生点");
			return GetLevelDefaultSpawn();
		}
		default:
			return GetLevelDefaultSpawn();
		}
	}

	private static Vector3 GetHomeDefaultSpawn()
	{
		return GameManager.GetStartPos();
	}

	private static Vector3 GetLevelDefaultSpawn()
	{
		Transform transform = SingletonMonoScope<TeleportManager>.Instance.ResolveSpawnPoint("01_01", TeleportType.Exit);
		if ((bool)transform)
		{
			return transform.position;
		}
		return Vector3.zero;
	}
}
