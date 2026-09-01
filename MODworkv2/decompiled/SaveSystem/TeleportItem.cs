using Core.Teleport;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SaveSystem;

public class TeleportItem : MonoBehaviour
{
	public Text levelName;

	public GameObject hereIcon;

	public Button btn;

	public void Init(string levelId, bool here)
	{
		SetLevelName(levelId);
		SetHere(here);
		btn.interactable = !here;
		btn.onClick.RemoveAllListeners();
		if (!btn.interactable)
		{
			return;
		}
		btn.onClick.AddListener(delegate
		{
			string text = SceneManager.GetActiveScene().name;
			if (levelId == "Home")
			{
				if (text == "LevelScene")
				{
					string curLevel = LevelManager.GetCurLevel();
					SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
					{
						Reason = HomePlayerSpawnReason.ReturnFromChapter,
						FromChapterId = LevelManager.GetChapterId(curLevel)
					});
				}
			}
			else
			{
				SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
				{
					Reason = LevelPlayerSpawnReason.EnterFromTeleport,
					FromTeleportType = TeleportType.Exit,
					FromTeleportStation = true
				});
			}
			RunTeleportNextFrame(levelId).Forget();
		});
	}

	private static async UniTaskVoid RunTeleportNextFrame(string levelId)
	{
		await UniTask.NextFrame();
		if (levelId == "Home")
		{
			SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade).Forget();
		}
		else
		{
			await SceneLoadManager.LoadLevelScene(levelId, SceneTransitionMode.Fade);
		}
	}

	public void SetHere(bool b)
	{
		if ((bool)hereIcon)
		{
			hereIcon.SetActive(b);
		}
	}

	public void SetLevelName(string id)
	{
		if ((bool)levelName && SingletonMonoScope<LevelManager>.HasInstance)
		{
			levelName.text = LOC.MM.GetLevel(LevelManager.GetLevelLocalKey(id));
		}
	}
}
