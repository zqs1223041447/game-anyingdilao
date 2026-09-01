using UnityEngine;

namespace Core.Teleport.PlayerSpawn;

public class HomePlayerSpawnRequest
{
	public HomePlayerSpawnReason Reason;

	public int FromChapterId;

	public Vector3? BackFromChallengePos;

	public bool PlayHomeStartFromFirst;

	public bool PlayHomeVictoryMusic;
}
