using System;
using System.Text;
using Core.Settings;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs;
using Interact;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace UI.DebugUI;

public class PerformanceUI : SingletonMonoGlobal<PerformanceUI>
{
	private bool visible;

	private float deltaTime;

	private float uptime;

	private readonly FrameTiming[] frameTimings = new FrameTiming[1];

	private float cpuFrameMs;

	private float gpuFrameMs;

	private GUIStyle labelStyle;

	private readonly StringBuilder sb = new StringBuilder(512);

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

	private void Update()
	{
		if (!SettingsLoader.Instance.DebugMode)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F6))
		{
			visible = !visible;
		}
		if (visible)
		{
			deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
			uptime += Time.unscaledDeltaTime;
			FrameTimingManager.CaptureFrameTimings();
			if (FrameTimingManager.GetLatestTimings(1u, frameTimings) != 0)
			{
				FrameTiming frameTiming = frameTimings[0];
				cpuFrameMs = (float)frameTiming.cpuFrameTime;
				gpuFrameMs = (float)frameTiming.gpuFrameTime;
			}
		}
	}

	private void OnGUI()
	{
		if (SettingsLoader.Instance.DebugMode && visible)
		{
			if (labelStyle == null)
			{
				labelStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 26,
					alignment = TextAnchor.UpperLeft
				};
			}
			sb.Clear();
			DrawPerformance();
			DrawFrameTiming();
			DrawRendering();
			DrawMemory();
			DrawScene();
			DrawTime();
			DrawInput();
			GUI.Label(new Rect(10f, 10f, 950f, Screen.height), sb.ToString(), labelStyle);
		}
	}

	private void DrawPerformance()
	{
		float num = ((deltaTime > 0f) ? (1f / deltaTime) : 0f);
		float num2 = deltaTime * 1000f;
		sb.AppendLine("== 性能 Performance ==");
		sb.AppendLine($"帧率 FPS            : {num:F1}");
		sb.AppendLine($"帧耗时 FrameTime    : {num2:F2} ms");
		sb.AppendLine();
	}

	private void DrawFrameTiming()
	{
		sb.AppendLine("== 帧耗时 Frame Timing ==");
		sb.AppendLine($"CPU Frame Time : {cpuFrameMs:F2} ms");
		sb.AppendLine($"GPU Frame Time : {gpuFrameMs:F2} ms");
		if (gpuFrameMs > cpuFrameMs + 1f)
		{
			sb.AppendLine("瓶颈 Bottleneck : GPU");
		}
		else if (cpuFrameMs > gpuFrameMs + 1f)
		{
			sb.AppendLine("瓶颈 Bottleneck : CPU");
		}
		else
		{
			sb.AppendLine("瓶颈 Bottleneck : Balanced");
		}
		sb.AppendLine();
	}

	private void DrawRendering()
	{
	}

	private void DrawMemory()
	{
		long monoUsedSizeLong = Profiler.GetMonoUsedSizeLong();
		long totalAllocatedMemoryLong = Profiler.GetTotalAllocatedMemoryLong();
		long totalReservedMemoryLong = Profiler.GetTotalReservedMemoryLong();
		sb.AppendLine("== 内存 Memory ==");
		sb.AppendLine("Mono 使用   : " + FormatMB(monoUsedSizeLong));
		sb.AppendLine("总分配     : " + FormatMB(totalAllocatedMemoryLong));
		sb.AppendLine("已保留     : " + FormatMB(totalReservedMemoryLong));
		sb.AppendLine();
	}

	private void DrawScene()
	{
		Scene activeScene = SceneManager.GetActiveScene();
		sb.AppendLine("== 场景 Scene ==");
		sb.AppendLine("名称 Name : " + activeScene.name);
		sb.AppendLine($"索引 Index: {activeScene.buildIndex}");
		sb.AppendLine();
	}

	private void DrawTime()
	{
		sb.AppendLine("== 时间 Time ==");
		sb.AppendLine($"TimeScale : {Time.timeScale:F2}");
		sb.AppendLine("运行时长 Uptime : " + FormatTime(uptime));
		sb.AppendLine();
	}

	private void DrawInput()
	{
		sb.AppendLine("== 输入 Input ==");
		sb.AppendLine($"鼠标锁定 LockState : {Cursor.lockState}");
		sb.AppendLine($"鼠标可见 Visible   : {Cursor.visible}");
		sb.AppendLine($"任意按键 AnyKey   : {Input.anyKey}");
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			sb.AppendLine($"交互系统 是否解锁   : {InteractionManager.AllInteractToggle}");
		}
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			sb.AppendLine($"输入系统 是否解锁   : {InputManager.AllActionToggle}");
		}
		sb.AppendLine();
	}

	private static string FormatMB(long bytes)
	{
		return $"{(float)bytes / 1048576f:F1} MB";
	}

	private static string FormatTime(float seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
	}
}
