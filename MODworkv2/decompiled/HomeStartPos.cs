using Core;
using UnityEngine;

public class HomeStartPos : MonoBehaviour
{
	public enum HomeStartPosType
	{
		PlayerPos,
		PortalPos,
		MijingPos
	}

	public HomeStartPosType currentType;

	private void Awake()
	{
		switch (currentType)
		{
		case HomeStartPosType.PlayerPos:
			GameManager.SetPlayerStartPos(base.transform.position);
			break;
		case HomeStartPosType.PortalPos:
			GameManager.SetPortalStartPos(base.transform.position);
			break;
		case HomeStartPosType.MijingPos:
			GameManager.SetMijingPos(base.transform.position);
			break;
		}
	}
}
