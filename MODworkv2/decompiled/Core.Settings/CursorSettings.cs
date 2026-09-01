using UnityEngine;

namespace Core.Settings;

[CreateAssetMenu(fileName = "光标系统设置", menuName = "全局项目设置/光标系统设置")]
public class CursorSettings : ScriptableObject
{
	public bool enableCursorManager;

	public bool enableCursorState;

	public float virualCursorScale = 0.73f;

	[Header("Hotspot")]
	public Vector2 smallHot = new Vector2(0.3f, 0.3f);

	public Vector2 mediumHot = new Vector2(0.2f, 0.2f);

	public Vector2 largeHot = new Vector2(0.05f, 0.2f);

	public Sprite uiCursorSmallSprite;

	public Sprite uiCursorMediumSprite;

	public Sprite uiCursorLargeSprite;

	public Sprite aimCursorSmallSprite;

	public Sprite aimCursorMediumSprite;

	public Sprite aimCursorLargeSprite;

	public Texture2D uiCursorSmall;

	public Texture2D uiCursorMedium;

	public Texture2D uiCursorLarge;

	public Texture2D aimCursorSmall;

	public Texture2D aimCursorMedium;

	public Texture2D aimCursorLarge;
}
