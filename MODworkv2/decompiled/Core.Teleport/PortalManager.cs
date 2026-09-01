using System.Collections.Generic;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Level.LevelStates;
using Level.StateData.ChapterStates;
using Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Teleport;

public class PortalManager : SingletonMonoScope<PortalManager>
{
	private PlayerManager player;

	private readonly Dictionary<PortalType, Portal> _instances = new Dictionary<PortalType, Portal>();

	private static readonly Dictionary<PortalType, string> PortalPrefabPath = new Dictionary<PortalType, string>
	{
		{
			PortalType.HomeGoLevel,
			"World/Build/HomePortal"
		},
		{
			PortalType.GoBackHome,
			"World/Build/HomePortal"
		},
		{
			PortalType.Challenge,
			"World/Build/ChallengePortal"
		}
	};

	private bool _isGoBackHomeBusy;

	private bool _isPortalBusy;

	private const float MinOffset = 0.3f;

	private const float MaxOffset = 0.8f;

	private const int TryCount = 8;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			player = SingletonMonoScope<PlayerManager>.Instance;
		}
		HomeSceneManager.OnHomeSceneRestored += OnHomeRestored;
		LevelSceneManager.OnLevelSceneRestored += OnLevelRestored;
	}

	protected override void OnDestroy()
	{
		HomeSceneManager.OnHomeSceneRestored -= OnHomeRestored;
		LevelSceneManager.OnLevelSceneRestored -= OnLevelRestored;
	}

	public void OnPortalInteracted(Portal portal)
	{
		if ((bool)portal)
		{
			switch (portal.PortalType)
			{
			case PortalType.GoBackHome:
				HandleGoBackHomePortal().Forget();
				break;
			case PortalType.HomeGoLevel:
				HandleHomeGoLevelPortal().Forget();
				break;
			case PortalType.Challenge:
				HandleChallengePortal().Forget();
				break;
			default:
				LogUtil.Warn("PortalManager", $"未处理的 PortalType: {portal.PortalType}");
				break;
			}
		}
	}

	public void RestorePortals()
	{
		ClearPortalData();
		if (SceneManager.GetActiveScene().name == "HomeScene")
		{
			TrySpawnPortalFromState(PortalType.HomeGoLevel);
		}
		else
		{
			TrySpawnPortalFromState(PortalType.GoBackHome);
		}
		TrySpawnPortalFromState(PortalType.Challenge);
	}

	public void RequestOpenGoBackHomePortal()
	{
		if (EnsurePlayer())
		{
			Vector3 vector = GetPortalSpawnPos(player.transform.position);
			RequestOpenPortalInternal(PortalType.GoBackHome, vector);
			CreateHomeGoLevelPortal(LevelManager.GetCurLevel(), vector);
		}
	}

	public void RequestOpenChallengePortal(int sceneQulity = 0)
	{
		if (EnsurePlayer())
		{
			Vector3 spawnPos = GetPortalSpawnPos(player.transform.position);
			RequestOpenPortalInternal(PortalType.Challenge, spawnPos, sceneQulity);
		}
	}

	public void ClearPortalData()
	{
		foreach (KeyValuePair<PortalType, Portal> instance in _instances)
		{
			if ((bool)instance.Value)
			{
				Object.Destroy(instance.Value.gameObject);
			}
		}
		_instances.Clear();
	}

	private void OnHomeRestored()
	{
		foreach (KeyValuePair<PortalType, Portal> instance in _instances)
		{
			PortalType key = instance.Key;
			Portal value = instance.Value;
			if ((bool)value && value.IsConsumed)
			{
				SingletonMonoGlobal<StateDataManager>.Instance.RemovePortalState(key);
				value.PlayConsumeFxAndDestroyAsync().Forget();
			}
		}
	}

	private void OnLevelRestored()
	{
		foreach (KeyValuePair<PortalType, Portal> instance in _instances)
		{
			PortalType key = instance.Key;
			Portal value = instance.Value;
			if ((bool)value && value.IsConsumed)
			{
				SingletonMonoGlobal<StateDataManager>.Instance.RemovePortalState(key);
				value.PlayConsumeFxAndDestroyAsync().Forget();
			}
		}
	}

	private async UniTask HandleGoBackHomePortal()
	{
		StateDataManager instance = SingletonMonoGlobal<StateDataManager>.Instance;
		if (!instance.HasPortalState(PortalType.GoBackHome) || _isGoBackHomeBusy)
		{
			return;
		}
		_isGoBackHomeBusy = true;
		try
		{
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
			{
				Reason = HomePlayerSpawnReason.ReturnFromHomePortal
			});
			instance.MarkPortalConsumed(PortalType.GoBackHome);
			ConsumePortalLogic(PortalType.GoBackHome);
			await SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade);
		}
		finally
		{
			_isGoBackHomeBusy = false;
		}
	}

	private async UniTask HandleHomeGoLevelPortal()
	{
		StateDataManager instance = SingletonMonoGlobal<StateDataManager>.Instance;
		if (instance.HasPortalState(PortalType.HomeGoLevel))
		{
			PortalData portalState = instance.GetPortalState(PortalType.HomeGoLevel);
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.BackFromHome,
				BackFromHomePos = portalState.returnPosInLevel
			});
			instance.MarkPortalConsumed(PortalType.HomeGoLevel);
			ConsumePortalLogic(PortalType.HomeGoLevel);
			instance.RemovePortalState(PortalType.HomeGoLevel);
			instance.MarkPortalConsumed(PortalType.GoBackHome);
			ConsumePortalLogic(PortalType.GoBackHome);
			await SceneLoadManager.LoadLevelScene(portalState.targetLevelId, SceneTransitionMode.Fade);
		}
	}

	private async UniTask HandleChallengePortal()
	{
		StateDataManager instance = SingletonMonoGlobal<StateDataManager>.Instance;
		if (instance.HasPortalState(PortalType.Challenge))
		{
			PortalData portalState = instance.GetPortalState(PortalType.Challenge);
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromChallenge
			});
			SingletonMonoScope<TeleportManager>.Instance.RecordChallengeContext(portalState.belongLevelId, portalState.pos);
			string curLevel = LevelManager.GetCurLevel();
			instance.MarkPortalConsumed(PortalType.Challenge);
			ConsumePortalLogic(PortalType.Challenge);
			if (curLevel != portalState.targetLevelId)
			{
				await SceneLoadManager.LoadLevelScene(portalState.targetLevelId, SceneTransitionMode.Fade, portalState.sceneQulity);
			}
			else
			{
				LogUtil.Warn("PortalManager", "已在目标关卡，传送门失效");
			}
		}
	}

	private void RemoveChapterPortalState(PortalType type)
	{
		SingletonMonoGlobal<StateDataManager>.Instance.CurrentChapterState?.PortalStates.RemovePortal(type);
	}

	private void CreateHomeGoLevelPortal(string targetLevelId, Vector3 returnPosInLevel)
	{
		SingletonMonoGlobal<StateDataManager>.Instance.SetPortalState(PortalType.HomeGoLevel, new PortalData
		{
			belongLevelId = "Home",
			targetLevelId = targetLevelId,
			pos = GameManager.GetPortalPos(),
			returnPosInLevel = returnPosInLevel,
			IsConsumed = false
		});
	}

	private bool EnsurePlayer()
	{
		if (!player && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			player = SingletonMonoScope<PlayerManager>.Instance;
		}
		return player;
	}

	private static GameObject LoadPortalPrefab(PortalType type)
	{
		if (!PortalPrefabPath.TryGetValue(type, out var value))
		{
			LogUtil.Error("PortalManager", $"未配置 PortalType={type} 的 Prefab 路径");
			return null;
		}
		return Singleton<ResManager>.Instance.Load<GameObject>(value);
	}

	private void TrySpawnPortalFromState(PortalType type)
	{
		if (SingletonMonoGlobal<StateDataManager>.Instance.HasPortalState(type))
		{
			PortalData portalState = SingletonMonoGlobal<StateDataManager>.Instance.GetPortalState(type);
			string curLevel = LevelManager.GetCurLevel();
			if (!(portalState.belongLevelId != curLevel))
			{
				GameObject prefab = LoadPortalPrefab(type);
				SpawnPortalInstance(type, prefab, portalState.pos);
			}
		}
	}

	private void RequestOpenPortalInternal(PortalType type, Vector3 spawnPos, int sceneQulity = 0)
	{
		if (_isPortalBusy || !SingletonMonoScope<LevelManager>.HasInstance)
		{
			return;
		}
		_isPortalBusy = true;
		try
		{
			if (SingletonMonoGlobal<StateDataManager>.Instance.HasPortalState(type))
			{
				RemoveInstanceWithFade(type);
			}
			RemoveChapterPortalState(type);
			switch (type)
			{
			default:
				return;
			case PortalType.HomeGoLevel:
				LogUtil.Error("PortalManager", "HomeGoLevel 不能通过 RequestOpenPortalInternal 创建，只能由 GoBackHome 流程生成");
				return;
			case PortalType.GoBackHome:
				SingletonMonoGlobal<StateDataManager>.Instance.SetPortalState(type, new PortalData
				{
					belongLevelId = LevelManager.GetCurLevel(),
					targetLevelId = "Home",
					pos = spawnPos,
					IsConsumed = false
				});
				break;
			case PortalType.Challenge:
			{
				List<string> allChallengeLevelIds = LevelManager.GetAllChallengeLevelIds();
				if (allChallengeLevelIds == null || allChallengeLevelIds.Count == 0)
				{
					LogUtil.Error("PortalManager", "ChallengeLevelIds 为空");
					return;
				}
				SingletonMonoGlobal<StateDataManager>.Instance.SetPortalState(type, new PortalData
				{
					belongLevelId = LevelManager.GetCurLevel(),
					targetLevelId = allChallengeLevelIds[Random.Range(0, allChallengeLevelIds.Count)],
					pos = spawnPos,
					sceneQulity = Mathf.Max(0, sceneQulity),
					IsConsumed = false
				});
				break;
			}
			}
			GameObject prefab = LoadPortalPrefab(type);
			SpawnPortalInstance(type, prefab, spawnPos);
		}
		finally
		{
			_isPortalBusy = false;
		}
	}

	private void ConsumePortalLogic(PortalType type)
	{
		if (_instances.TryGetValue(type, out var value) && (bool)value)
		{
			value.ConsumeLogic();
		}
	}

	private void RemoveInstanceWithFade(PortalType type)
	{
		if (_instances.TryGetValue(type, out var value) && (bool)value)
		{
			value.ConsumeLogic();
			value.PlayConsumeFxAndDestroy();
		}
		SingletonMonoGlobal<StateDataManager>.Instance.RemovePortalState(type);
		_instances.Remove(type);
	}

	private void RemoveInstanceImmediate(PortalType type)
	{
		if (_instances.TryGetValue(type, out var value) && (bool)value)
		{
			Object.Destroy(value.gameObject);
		}
		_instances.Remove(type);
	}

	private void SpawnPortalInstance(PortalType type, GameObject prefab, Vector3 pos)
	{
		if ((bool)prefab)
		{
			RemoveInstanceImmediate(type);
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.transform.position = pos;
			Portal component = gameObject.GetComponent<Portal>();
			if (!component)
			{
				Object.Destroy(gameObject);
				return;
			}
			PortalData portalState = SingletonMonoGlobal<StateDataManager>.Instance.GetPortalState(type);
			component.Init(type, portalState);
			_instances[type] = component;
		}
	}

	public static Vector2 GetPortalSpawnPos(Vector2 playerPos)
	{
		for (int i = 0; i < 8; i++)
		{
			Vector2 normalized = Random.insideUnitCircle.normalized;
			float num = Random.Range(0.3f, 0.8f);
			Vector2 vector = playerPos + normalized * num;
			int mask = LayerMask.GetMask("block");
			if (!Physics2D.OverlapCircle(vector, 0.4f, mask))
			{
				return vector;
			}
		}
		Vector2 normalized2 = Random.insideUnitCircle.normalized;
		return playerPos + normalized2 * 0.3f;
	}
}
