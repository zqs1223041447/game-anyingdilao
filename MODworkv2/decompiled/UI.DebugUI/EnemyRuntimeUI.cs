using Core.Settings;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Level.LevelStates;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.DebugUI;

public class EnemyRuntimeUI : SingletonMonoGlobal<EnemyRuntimeUI>
{
	private bool show;

	private EnemyPoint[] points;

	private Vector2 scrollPos;

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

	public void RefreshUI()
	{
		if (SettingsLoader.Instance.DebugMode)
		{
			points = Object.FindObjectsOfType<EnemyPoint>();
		}
	}

	private void Update()
	{
		if (SettingsLoader.Instance.DebugMode && !(SceneManager.GetActiveScene().name == "StartScene") && Input.GetKeyDown(KeyCode.F4))
		{
			show = !show;
			if (show)
			{
				points = Object.FindObjectsOfType<EnemyPoint>();
			}
		}
	}

	private void OnGUI()
	{
		if (!SettingsLoader.Instance.DebugMode || SceneManager.GetActiveScene().name == "StartScene" || !show || points == null)
		{
			return;
		}
		int num = Screen.width - 460 - 10;
		int num2 = (Screen.height - 620) / 2;
		GUI.Box(new Rect(num, num2, 460f, 620f), "敌人运行时数据面板");
		int num3 = num + 10;
		int num4 = num2 + 30;
		StateDataManager stateDataManager = SingletonMonoGlobal<StateDataManager>.Instance;
		GUI.Label(new Rect(num3, num4, 440f, 24f), $"Chapter: {(stateDataManager?.CurrentChapterState?.ChapterId).GetValueOrDefault(-1)}");
		num4 += 24;
		string curLevel = LevelManager.GetCurLevel();
		GUI.Label(new Rect(num3, num4, 440f, 24f), "Level: " + curLevel);
		num4 += 32;
		int num5 = 620 - (num4 - num2) - 60 - 10;
		Rect position = new Rect(num3, num4, 440f, num5);
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		EnemyPoint[] array = points;
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i])
			{
				num9++;
			}
		}
		Rect viewRect = new Rect(0f, 0f, 420f, num9 * 24 + 4);
		scrollPos = GUI.BeginScrollView(position, scrollPos, viewRect);
		int num10 = 0;
		array = points;
		EnemyPoint[] array2 = array;
		foreach (EnemyPoint enemyPoint in array2)
		{
			if ((bool)enemyPoint)
			{
				int activeCount = enemyPoint.ActiveCount;
				int totalCount = enemyPoint.TotalCount;
				int deadCount = enemyPoint.DeadCount;
				num6 += activeCount;
				num7 += deadCount;
				num8 += totalCount;
				GUI.Label(new Rect(0f, num10, viewRect.width, 24f), $"{enemyPoint.RuntimeId}    活跃:{activeCount} / 总:{totalCount} / 死亡:{deadCount}");
				num10 += 24;
			}
		}
		GUI.EndScrollView();
		int num11 = num2 + 620 - 60 - 8;
		GUI.Box(new Rect(num3, num11, 440f, 60f), GUIContent.none);
		GUI.Label(new Rect(num3 + 6, num11 + 6, 428f, 24f), $"TOTAL    活跃:{num6} / 总:{num8} / 死亡:{num7}");
		GUI.Label(new Rect(num3 + 6, num11 + 30, 428f, 24f), $"总计生成数: {EnemyPoint.TotalSpawned}    总计回收数: {EnemyPoint.TotalDespawned}    当前存活数: {EnemyPoint.TotalSpawned - EnemyPoint.TotalDespawned}");
	}
}
