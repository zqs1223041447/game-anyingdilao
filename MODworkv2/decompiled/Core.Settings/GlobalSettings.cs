using Mijing;
using UnityEngine;

namespace Core.Settings;

public class GlobalSettings : ScriptableObject
{
	public CursorSettings cursorSettings;

	public IconSettings iconSettings;

	public MijingSettings mijingSettings;

	public BaoshiSettings baoshiSettings;

	public WeaponSettings weaponSettings;

	public bool SteamToggle;

	public bool DebugMode;

	public bool saveIsJson;

	public LayerMask interactLayers;

	public bool fmodToggle;

	public bool canDead = true;

	public float enemyUnloadDis = 16f;

	public float enemyTriggerDis = 9f;

	public float itemInteractDis = 1.4f;

	public float chestInteractDis = 2f;

	public float coffinInteractDis = 2f;

	public float templeInteractDis = 2f;

	public float l_coffinInteractDis = 2f;

	public float portalInteractDis = 2f;

	public float teleportInteractDis = 1.8f;

	public float npcInteractDis = 2f;

	public bool MijingToggle;

	public bool BaoshiToggle;

	public bool WeaponToggle;
}
