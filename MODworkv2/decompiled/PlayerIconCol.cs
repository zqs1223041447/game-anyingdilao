using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;

public class PlayerIconCol : MonoBehaviour
{
	[Header("默认朝向（本地空间）")]
	[Tooltip("图标美术在 0° 时面向的方向")]
	public Vector2 defaultForward = Vector2.right;

	[Header("手柄死区")]
	public float deadZone = 0.3f;

	private void Awake()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		component.sprite = SettingsLoader.Instance.iconSettings.player;
		component.color = SettingsLoader.Instance.iconSettings.playerColor;
		component.transform.localScale = SettingsLoader.Instance.iconSettings.GetPlayerFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
	}

	private void Update()
	{
		Vector2 vector = Vector2.zero;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			float num = GamepadInputManager.GetLeftStickXRaw();
			float num2 = GamepadInputManager.GetLeftStickYRaw();
			if (Mathf.Abs(num) < deadZone)
			{
				num = 0f;
			}
			if (Mathf.Abs(num2) < deadZone)
			{
				num2 = 0f;
			}
			Vector2 vector2 = new Vector2(num, num2);
			if (vector2.sqrMagnitude > 0.001f)
			{
				vector = vector2;
			}
		}
		else
		{
			if (InputBind.Get(ControlAction.Up))
			{
				vector.y += 1f;
			}
			if (InputBind.Get(ControlAction.Down))
			{
				vector.y -= 1f;
			}
			if (InputBind.Get(ControlAction.Left))
			{
				vector.x -= 1f;
			}
			if (InputBind.Get(ControlAction.Right))
			{
				vector.x += 1f;
			}
		}
		if (!(vector.sqrMagnitude < 0.001f))
		{
			vector.Normalize();
			float num3 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			float num4 = Mathf.Atan2(defaultForward.y, defaultForward.x) * 57.29578f;
			float z = num3 - num4;
			base.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
	}
}
