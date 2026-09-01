using UnityEngine;

namespace Core.Teleport.PlayerSpawn;

public class LevelPlayerSpawnRequest
{
	public LevelPlayerSpawnReason Reason;

	public string reqFromLevelId;

	public TeleportType? FromTeleportType;

	public bool FromTeleportStation;

	public Vector3? BackFromHomePos;

	public Vector3? BackFromChallengePos;
}
