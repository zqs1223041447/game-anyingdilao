using Core;
using Core.Settings;
using Core.Teleport;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Mijing;
using SaveSystem;
using Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.DebugUI;

public class TestUI : SingletonMonoGlobal<TestUI>
{
	private PlayerManager PL;

	private bool visible;

	private GUIStyle labelStyle;

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

	private void Start()
	{
		if (!(SceneManager.GetActiveScene().name == "StartScene") && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
		}
	}

	private void Update()
	{
		if (!SettingsLoader.Instance.DebugMode)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			visible = !visible;
		}
		if (SceneManager.GetActiveScene().name == "StartScene")
		{
			return;
		}
		if (!PL)
		{
			if (SingletonMonoScope<PlayerManager>.HasInstance)
			{
				PL = SingletonMonoScope<PlayerManager>.Instance;
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			for (int i = 0; i < 3; i++)
			{
				SingletonMonoScope<ItemManager>.Instance.DropWeapon(PL.transform, 0.5f, PL.Level, 0, 20f);
			}
		}
		if (Input.GetKeyDown(KeyCode.Y))
		{
			for (int j = 0; j < 3; j++)
			{
				SingletonMonoScope<ItemManager>.Instance.DropBaoshi(PL.transform, 0.5f, Random.Range(70, 101));
			}
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			SingletonMonoScope<ItemManager>.Instance.DropBaoshi(PL.transform, 0.5f, Random.Range(100, 101));
			SingletonMonoScope<ItemManager>.Instance.DropAnyPotion(PL.transform, 0.5f, Random.Range(100, 101));
			SingletonMonoScope<ItemManager>.Instance.DropBuffPotion(PL.transform, 0.5f, PL.Level);
			SingletonMonoScope<ItemManager>.Instance.DropAnyPotion(PL.transform, 0.5f, Random.Range(0, 101));
			SingletonMonoScope<ItemManager>.Instance.DropPremPotion(PL.transform, 0.5f, Random.Range(100, 101));
			SingletonMonoScope<ItemManager>.Instance.DropSpcItem(PL.transform, 0.5f, "Void Treasure Bag");
			SingletonMonoScope<ItemManager>.Instance.DropSpcItem(PL.transform, 0.5f, "Public Chest Key");
		}
	}

	private void OnGUI()
	{
		if (!SettingsLoader.Instance.DebugMode || !visible)
		{
			return;
		}
		if (labelStyle == null)
		{
			labelStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 22,
				alignment = TextAnchor.MiddleCenter
			};
		}
		float num = 150f;
		float num2 = 35f;
		float num3 = 15f;
		float x = (float)Screen.width - num - num3;
		float num4 = num3;
		float num5 = (float)Screen.height - num2 - num3;
		float num6 = (float)Screen.width - num - num3 - 100f;
		GUI.Label(new Rect(num6, num5, num + 40f, num2 + 5f), $"当前游玩时间: {PlayTimeManager.GetTotalSeconds()}", labelStyle);
		if (SingletonMonoScope<AutoSaveManager>.HasInstance)
		{
			AutoSaveManager autoSaveManager = SingletonMonoScope<AutoSaveManager>.Instance;
			string text = "自动存档: " + (autoSaveManager.IsEnabled ? "开启" : "关闭") + "\n" + $"已过去: {autoSaveManager.ElapsedTime:F1}s\n" + $"剩余: {autoSaveManager.RemainingTime:F1}s\n" + $"间隔: {autoSaveManager.Interval:F0}s";
			GUI.Label(new Rect(num6 - 220f, num5 - 90f, 260f, 150f), text, labelStyle);
		}
		if (SingletonMonoScope<LevelManager>.HasInstance)
		{
			GUI.Label(new Rect(x, num4, num, num2), "当前关卡ID: " + LevelManager.GetCurLevel(), labelStyle);
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "手动保存游戏"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			SaveManager.RequestSave();
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "切换到 Home 场景"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			if (SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetIsMijing())
			{
				SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
				{
					Reason = HomePlayerSpawnReason.BackFromMijing
				});
				SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade).Forget();
			}
			else if (SingletonMonoScope<LevelManager>.HasInstance && LevelManager.GetIsChallenge())
			{
				SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
				{
					Reason = HomePlayerSpawnReason.BackFromChallenge,
					BackFromChallengePos = ChallengeContext.FromWorldPos
				});
				SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade).Forget();
			}
			else
			{
				SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
				{
					Reason = HomePlayerSpawnReason.ReturnFromChapter,
					FromChapterId = LevelManager.GetChapterId(LevelManager.GetCurLevel())
				});
				SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade).Forget();
			}
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "切换到 第一章第1关"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromTeleport,
				FromTeleportType = TeleportType.Exit
			});
			SceneLoadManager.LoadLevelScene("01_01", SceneTransitionMode.Fade).Forget();
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "切换到 当前关卡的下一关"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene" || SceneManager.GetActiveScene().name == "HomeScene")
			{
				return;
			}
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromTeleport,
				FromTeleportType = TeleportType.Exit
			});
			SceneLoadManager.LoadLevelScene(LevelManager.GetNextLevelId(), SceneTransitionMode.Fade).Forget();
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "切换到 第一章第9关"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromTeleport,
				FromTeleportType = TeleportType.Exit
			});
			SceneLoadManager.LoadLevelScene("01_09", SceneTransitionMode.Fade).Forget();
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "切换到 第二章第10关"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromTeleport,
				FromTeleportType = TeleportType.Exit
			});
			SceneLoadManager.LoadLevelScene("02_10", SceneTransitionMode.Fade).Forget();
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "切换到 第七章第15关"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromTeleport,
				FromTeleportType = TeleportType.Exit
			});
			SceneLoadManager.LoadLevelScene("07_15", SceneTransitionMode.Fade).Forget();
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "切换到 第二章第12关"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromTeleport,
				FromTeleportType = TeleportType.Exit
			});
			SceneLoadManager.LoadLevelScene("02_12", SceneTransitionMode.Fade).Forget();
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "进入下一层秘境"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			if (SingletonMonoScope<MijingManager>.HasInstance && SaveManager.HasRuntime && SaveManager.RuntimeData.UnlockedMijing)
			{
				SingletonMonoScope<MijingManager>.Instance.EnterMijing(SingletonMonoScope<MijingManager>.Instance.GetNextFloor(1));
			}
		}
		num4 += num2 + 6f;
		if (GUI.Button(new Rect(x, num4, num, num2), "进入下50层秘境"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			if (SingletonMonoScope<MijingManager>.HasInstance && SaveManager.HasRuntime && SaveManager.RuntimeData.UnlockedMijing)
			{
				SingletonMonoScope<MijingManager>.Instance.EnterMijing(SingletonMonoScope<MijingManager>.Instance.GetNextFloor(50));
			}
		}
		num4 += num2 + 10f;
		if (GUI.Button(new Rect(x, num4, num, num2), "给玩家加3000元"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				SingletonMonoScope<InventoryManager>.Instance.AddMoney(3000L);
			}
		}
		num4 += num2 + 10f;
		if (GUI.Button(new Rect(x, num4, num, num2), "给玩家减3000元"))
		{
			if (SceneManager.GetActiveScene().name == "StartScene")
			{
				return;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				SingletonMonoScope<InventoryManager>.Instance.RemoveMoney(3000L);
			}
		}
		num4 += num2 + 10f;
		if (GUI.Button(new Rect(x, num4, num, num2), "仅删除全局存档数据") && SceneManager.GetActiveScene().name == "StartScene")
		{
			SaveManager.DeleteGlobalData();
		}
		num4 += num2 + 10f;
		if (GUI.Button(new Rect(x, num4, num, num2), "删除全部存档数据") && SceneManager.GetActiveScene().name == "StartScene")
		{
			SaveManager.DeleteAllSaveData();
		}
		if (SingletonMonoScope<AutoSaveManager>.HasInstance)
		{
			AutoSaveManager autoSaveManager2 = SingletonMonoScope<AutoSaveManager>.Instance;
			num4 += num2 + 10f;
			if (GUI.Button(new Rect(x, num4, num, num2), "重置自动存档计时"))
			{
				autoSaveManager2.ResetTimer();
			}
			num4 += num2 + 10f;
			if (GUI.Button(new Rect(x, num4, num, num2), "自动保存间隔设置为5秒"))
			{
				autoSaveManager2.SetInterval(5f);
			}
		}
	}
}
