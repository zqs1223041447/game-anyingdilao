using UnityEngine;

namespace Core.Settings;

[CreateAssetMenu(fileName = "图标系统设置", menuName = "全局项目设置/图标系统设置")]
public class IconSettings : ScriptableObject
{
	[Header("全局总缩放")]
	public Vector3 globalScale;

	[Header("地图联动补偿强度")]
	public float mapViewInfluence = 1f;

	public float mapScaleInfluence = 1f;

	[Header("玩家")]
	public Sprite player;

	public Color playerColor;

	public Vector3 playerScale;

	[Header("传送门")]
	public Sprite portal;

	public Color challengePortalColor;

	public Color mijingPortalColor;

	public Color homePortalColor;

	public Color backPortalColor;

	public Color bossPortalColor;

	public Vector3 portalScale;

	[Header("仓库")]
	public Sprite storge;

	public Color storgeColor;

	public Vector3 storgeScale;

	[Header("掉落物")]
	public Sprite dropItem;

	public Color dropItemColor;

	public Vector3 dropItemScale;

	[Header("NPC")]
	public Sprite npc;

	public Vector3 npcScale;

	[Header("传送阵")]
	public Sprite station;

	public Color stationUnlockColor;

	public Color stationLockColor;

	public Vector3 stationScale;

	[Header("神殿")]
	public Sprite temple;

	public Color templeUsedColor;

	public Color templeUnuseColor;

	public Vector3 templeScale;

	[Header("入口出口")]
	public Sprite levelPoint;

	public Vector3 levelPointScale;

	public Color EnterColor;

	public Color ExitColor;

	public Color OptionalEnterColor;

	[Header("敌人")]
	public Sprite enemy;

	public Vector3 enemyScale;

	public Color enemyColor;

	[Header("enemy")]
	public Sprite boss;

	public Vector3 bossScale;

	public Color bossColor;

	[Header("箱子/棺材")]
	public Sprite openedChest;

	public Color openedChestColor;

	public Vector3 openedChestScale;

	public Sprite newChest;

	public Color newChestColor;

	public Vector3 newChestScale;

	[Header("宝石加工台")]
	public Sprite bsTable;

	public Color bsTableColor;

	public Vector3 bsTableScale;

	private Vector3 ApplyGlobalScale(Vector3 localScale)
	{
		return new Vector3(localScale.x * globalScale.x, localScale.y * globalScale.y, localScale.z * globalScale.z);
	}

	public float GetMapCompensation(float mapViewRange, float mapScale)
	{
		float b = Mathf.Max(0.01f, mapViewRange);
		float num = Mathf.Max(0.01f, mapScale);
		float num2 = Mathf.Lerp(1f, b, mapViewInfluence);
		float num3 = Mathf.Lerp(1f, 1f / num, mapScaleInfluence);
		return Mathf.Clamp(num2 * num3, 0.5f, 2f);
	}

	public Vector3 GetFinalScale(Vector3 localScale, float mapViewRange, float mapScale)
	{
		Vector3 vector = ApplyGlobalScale(localScale);
		float mapCompensation = GetMapCompensation(mapViewRange, mapScale);
		return vector * mapCompensation;
	}

	public Vector3 GetPortalFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(portalScale, mapViewRange, mapScale);
	}

	public Vector3 GetStorgeFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(storgeScale, mapViewRange, mapScale);
	}

	public Vector3 GetDropItemFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(dropItemScale, mapViewRange, mapScale);
	}

	public Vector3 GetNpcFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(npcScale, mapViewRange, mapScale);
	}

	public Vector3 GetStationFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(stationScale, mapViewRange, mapScale);
	}

	public Vector3 GetTempleFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(templeScale, mapViewRange, mapScale);
	}

	public Vector3 GetLevelPointFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(levelPointScale, mapViewRange, mapScale);
	}

	public Vector3 GetEnemyFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(enemyScale, mapViewRange, mapScale);
	}

	public Vector3 GetBossFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(bossScale, mapViewRange, mapScale);
	}

	public Vector3 GetOpenedChestFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(openedChestScale, mapViewRange, mapScale);
	}

	public Vector3 GetNewChestFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(newChestScale, mapViewRange, mapScale);
	}

	public Vector3 GetBsTableFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(bsTableScale, mapViewRange, mapScale);
	}

	public Vector3 GetPlayerFinalScale(float mapViewRange, float mapScale)
	{
		return GetFinalScale(playerScale, mapViewRange, mapScale);
	}
}
