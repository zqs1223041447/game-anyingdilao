using UnityEngine;

namespace Level.StateData.ChapterStates;

public struct PortalData
{
	public string belongLevelId;

	public string targetLevelId;

	public Vector3 pos;

	public bool IsConsumed;

	public Vector3? returnPosInLevel;

	public int sceneQulity;
}
