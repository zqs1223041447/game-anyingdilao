using System.Collections.Generic;
using Core.Settings;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace UI.DebugUI;

public class DebugLogUI : SingletonMonoGlobal<DebugLogUI>
{
	private class LogItem
	{
		public string text;

		public float startTime;

		public float duration;

		public Color color;
	}

	[Header("快捷键设置")]
	[SerializeField]
	private KeyCode toggleKey = KeyCode.F7;

	public static bool Enabled = true;

	private static bool drawEnabled = true;

	private static float drawDisableAtTime = -1f;

	private static readonly List<LogItem> logs = new List<LogItem>();

	private GUIStyle style;

	private const float maxWidth = 800f;

	private bool isInited;

	public void Init()
	{
		if (SettingsLoader.Instance.DebugMode)
		{
			if (!isInited)
			{
				LogUtil.Info("初始化" + base.name);
			}
			isInited = true;
		}
	}

	public static void Log(string text, Color color, float duration = 1.5f)
	{
		if (Enabled)
		{
			logs.Add(new LogItem
			{
				text = text,
				color = color,
				startTime = Time.unscaledTime,
				duration = Mathf.Max(0.1f, duration)
			});
		}
	}

	private static void ForceLog(string text, Color color, float duration)
	{
		logs.Add(new LogItem
		{
			text = text,
			color = color,
			startTime = Time.unscaledTime,
			duration = Mathf.Max(0.1f, duration)
		});
	}

	private void Update()
	{
		if (!SettingsLoader.Instance.DebugMode)
		{
			return;
		}
		if (Input.GetKeyDown(toggleKey))
		{
			if (Enabled)
			{
				ForceLog("DebugLogUI 已关闭", Color.gray, 1.5f);
				Enabled = false;
				drawEnabled = true;
				drawDisableAtTime = Time.unscaledTime + 1.5f;
			}
			else
			{
				Enabled = true;
				ForceLog("DebugLogUI 已开启", Color.green, 1.5f);
				drawEnabled = true;
				drawDisableAtTime = -1f;
			}
		}
		if (!Enabled && drawDisableAtTime > 0f && Time.unscaledTime >= drawDisableAtTime)
		{
			drawEnabled = false;
			drawDisableAtTime = -1f;
		}
		float unscaledTime = Time.unscaledTime;
		for (int num = logs.Count - 1; num >= 0; num--)
		{
			if (unscaledTime - logs[num].startTime >= logs[num].duration)
			{
				logs.RemoveAt(num);
			}
		}
	}

	private void OnGUI()
	{
		if (!SettingsLoader.Instance.DebugMode || !drawEnabled || logs.Count == 0)
		{
			return;
		}
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label)
			{
				fontSize = 26,
				wordWrap = true,
				alignment = TextAnchor.UpperCenter
			};
		}
		if (logs.Count == 0)
		{
			return;
		}
		float x = ((float)Screen.width - 800f) * 0.5f;
		float num = 20f;
		float unscaledTime = Time.unscaledTime;
		foreach (LogItem log in logs)
		{
			float num2 = Mathf.Clamp01((unscaledTime - log.startTime) / log.duration);
			float num3 = 1f;
			if (num2 > 0.7f)
			{
				num3 = Mathf.Lerp(1f, 0f, (num2 - 0.7f) / 0.3f);
			}
			float num4 = 0f;
			if (num2 > 0.7f)
			{
				num4 = Mathf.Lerp(0f, -18f, (num2 - 0.7f) / 0.3f);
			}
			Color color = log.color;
			color.a *= num3;
			GUI.color = color;
			float num5 = style.CalcHeight(new GUIContent(log.text), 800f);
			GUI.Label(new Rect(x, num + num4, 800f, num5), log.text, style);
			num += num5 + 8f;
		}
		GUI.color = Color.white;
	}
}
