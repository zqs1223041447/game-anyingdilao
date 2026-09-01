using System.Collections.Generic;
using Core.Teleport;
using Data.RuntimeData.Skills.CompSkill;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using Level.StateData.ChapterStates;
using Level.StateData.GlobalStates;
using Level.StateData.LevelStates;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level.LevelStates;

public class StateDataManager : SingletonMonoGlobal<StateDataManager>
{
	public PlayerManager player;

	private bool isInited;

	public GlobalState CurrentRuntimeState { get; private set; }

	public ChapterState CurrentChapterState { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Init();
	}

	public void Init(bool force = false)
	{
		if (force)
		{
			InitGlobalState();
			InitChapterState();
			if (!player && SingletonMonoScope<PlayerManager>.HasInstance)
			{
				player = SingletonMonoScope<PlayerManager>.Instance;
			}
			isInited = true;
		}
		if (!isInited)
		{
			InitGlobalState();
			InitChapterState();
			if (!player && SingletonMonoScope<PlayerManager>.HasInstance)
			{
				player = SingletonMonoScope<PlayerManager>.Instance;
			}
			isInited = true;
		}
	}

	public void InitGlobalState()
	{
		CurrentRuntimeState = new GlobalState();
	}

	public void SaveCompsState()
	{
		if (CurrentRuntimeState == null)
		{
			LogUtil.Info("StateDataManager", "SaveCompsState: CurrentRuntimeState为空, 跳过保存");
			return;
		}
		if (!SingletonMonoScope<ACTbar>.HasInstance)
		{
			LogUtil.Error("ActBar未发现");
		}
		CurrentRuntimeState.CompsDataList.Clear();
		foreach (ACTListSkillBT item in SingletonMonoScope<ACTbar>.Instance.actListSkill)
		{
			if (item.DT.type != 1 || item.cpList == null || item.cpList.Count == 0)
			{
				continue;
			}
			CompsGlobalState compsGlobalState = new CompsGlobalState
			{
				SkillIndexName = item.DT.IndexName
			};
			foreach (Companion cp in item.cpList)
			{
				compsGlobalState.CompStates.Add(new CompState
				{
					currentHp = cp.HealthStat.CurrentValue
				});
			}
			CurrentRuntimeState.CompsDataList.Add(compsGlobalState);
			item.ClearCpList();
		}
	}

	public void RestoreCompState()
	{
		foreach (CompsGlobalState compsData in CurrentRuntimeState.CompsDataList)
		{
			if (SingletonMonoScope<ACTbar>.Instance.actListSkill.Count == 0)
			{
				LogUtil.Error("未读取到技能数据！");
			}
			foreach (ACTListSkillBT item in SingletonMonoScope<ACTbar>.Instance.actListSkill)
			{
				if (item.DT.IndexName != compsData.SkillIndexName)
				{
					continue;
				}
				ACT_skillComp comp = item.DT.comp;
				foreach (CompState compState in compsData.CompStates)
				{
					if (SingletonMonoScope<PlayerManager>.HasInstance)
					{
						if (!SingletonMonoScope<GameDataManager>.HasInstance)
						{
							return;
						}
						Vector2 vector = Random.insideUnitCircle * 0.2f;
						Vector3 position = SingletonMonoScope<PlayerManager>.Instance.transform.position + new Vector3(vector.x, vector.y, 0f);
						SK_FSQ_comp component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.CP_OBJ[comp.OBJ], position, Quaternion.identity).GetComponent<SK_FSQ_comp>();
						if (!SingletonMonoScope<Gun>.HasInstance)
						{
							return;
						}
						CompanionRuntimeData data = SingletonMonoScope<Gun>.Instance.SetCPData(new CompanionRuntimeData(), comp, compsData.SkillIndexName);
						component.Init(data, restore: true, compState.currentHp);
					}
				}
				break;
			}
		}
	}

	public void ClearGlobalState()
	{
		CurrentRuntimeState = null;
	}

	public void InitChapterState()
	{
		int chapterId = LevelManager.GetChapterId(SaveManager.RuntimeData.LastPlayLevelId);
		CurrentChapterState = new ChapterState
		{
			ChapterId = chapterId
		};
		LogUtil.Info($"创建运行时章节状态数据 最后一次游玩的章节Id为： {chapterId}，");
	}

	public void EnterChapter(int chapterId, string targetId)
	{
		if (!(targetId == "Home"))
		{
			if (!LevelManager.ShouldPersistLevelState(targetId))
			{
				LogUtil.Info("进入特殊临时关卡 " + targetId + "，不切章节，不清当前章节状态");
			}
			else if (chapterId <= 0)
			{
				LogUtil.Info($"EnterChapter: 目标关卡 {targetId} 的 chapterId 非法: {chapterId}");
			}
			else if (CurrentChapterState == null || CurrentChapterState.ChapterId != chapterId)
			{
				ExitChapter();
				CurrentChapterState = new ChapterState
				{
					ChapterId = chapterId
				};
				LogUtil.Info($"进入章节 {chapterId}，创建运行时章节状态数据");
			}
		}
	}

	public void ExitChapter()
	{
		if (CurrentChapterState != null)
		{
			LogUtil.Info($"退出章节 {CurrentChapterState.ChapterId} 或者退出游戏进程，清理其余章节运行时数据");
			ClearAllLevelStates();
			ClearAllPortalStates();
			CurrentChapterState = null;
			if (SingletonMonoScope<PortalManager>.HasInstance)
			{
				SingletonMonoScope<PortalManager>.Instance.ClearPortalData();
			}
		}
	}

	public void SetPortalState(PortalType type, PortalData data)
	{
		CurrentChapterState?.PortalStates.SetPortal(type, data);
	}

	public bool HasPortalState(PortalType type)
	{
		if (CurrentChapterState != null)
		{
			return CurrentChapterState.PortalStates.HasPortal(type);
		}
		return false;
	}

	public PortalData GetPortalState(PortalType type)
	{
		if (CurrentChapterState == null)
		{
			return default(PortalData);
		}
		return CurrentChapterState.PortalStates.GetPortal(type);
	}

	public void MarkPortalConsumed(PortalType type)
	{
		CurrentChapterState?.PortalStates.MarkConsumed(type);
	}

	public void RemovePortalState(PortalType type)
	{
		CurrentChapterState?.PortalStates.RemovePortal(type);
	}

	public void ClearAllPortalStates()
	{
		CurrentChapterState?.PortalStates.Clear();
	}

	public void ClearAllPortalStatesInMijing()
	{
		if (CurrentChapterState?.PortalStates != null && CurrentChapterState.ChapterId == -1)
		{
			CurrentChapterState.PortalStates.Clear();
		}
	}

	public LevelState GetCurrentLevelState()
	{
		string curLevel = LevelManager.GetCurLevel();
		if (string.IsNullOrEmpty(curLevel))
		{
			LogUtil.Error("GetCurrentLevelState 失败：CurLevel 为空。 Scene = " + SceneManager.GetActiveScene().name + "," + $" ChapterStateNull = {CurrentChapterState == null}");
			return null;
		}
		if (curLevel == "Home")
		{
			if (CurrentRuntimeState == null)
			{
				InitGlobalState();
			}
			if (CurrentRuntimeState.HomeLevelState == null)
			{
				CurrentRuntimeState.HomeLevelState = new LevelState
				{
					LevelId = curLevel
				};
			}
			if (CurrentRuntimeState.HomeLevelState.ItemLevelStates == null)
			{
				CurrentRuntimeState.HomeLevelState.ItemLevelStates = new List<ItemLevelState>();
			}
			if (CurrentRuntimeState.HomeLevelState.EnemyPoints == null)
			{
				CurrentRuntimeState.HomeLevelState.EnemyPoints = new Dictionary<string, EnemyPointLevelState>();
			}
			if (CurrentRuntimeState.HomeLevelState.Interactables == null)
			{
				CurrentRuntimeState.HomeLevelState.Interactables = new Dictionary<string, InteractableLevelState>();
			}
			if (CurrentRuntimeState.HomeLevelState.ChestPoints == null)
			{
				CurrentRuntimeState.HomeLevelState.ChestPoints = new Dictionary<string, ChestPointLevelState>();
			}
			return CurrentRuntimeState.HomeLevelState;
		}
		if (!LevelManager.ShouldPersistLevelState(curLevel))
		{
			return null;
		}
		if (CurrentChapterState == null)
		{
			LogUtil.Error("ChapterState 未初始化");
			return null;
		}
		return CurrentChapterState.GetOrCreateLevelState(curLevel);
	}

	public LevelState GetLevelStateByLevelId(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			return null;
		}
		if (levelId == "Home")
		{
			if (CurrentRuntimeState == null)
			{
				InitGlobalState();
			}
			if (CurrentRuntimeState.HomeLevelState == null)
			{
				CurrentRuntimeState.HomeLevelState = new LevelState
				{
					LevelId = levelId
				};
			}
			if (CurrentRuntimeState.HomeLevelState.ItemLevelStates == null)
			{
				CurrentRuntimeState.HomeLevelState.ItemLevelStates = new List<ItemLevelState>();
			}
			if (CurrentRuntimeState.HomeLevelState.EnemyPoints == null)
			{
				CurrentRuntimeState.HomeLevelState.EnemyPoints = new Dictionary<string, EnemyPointLevelState>();
			}
			if (CurrentRuntimeState.HomeLevelState.Interactables == null)
			{
				CurrentRuntimeState.HomeLevelState.Interactables = new Dictionary<string, InteractableLevelState>();
			}
			return CurrentRuntimeState.HomeLevelState;
		}
		if (!LevelManager.ShouldPersistLevelState(levelId))
		{
			return null;
		}
		return CurrentChapterState?.GetOrCreateLevelState(levelId);
	}

	public void ClearAllLevelStates()
	{
		if (CurrentChapterState?.LevelStates == null)
		{
			return;
		}
		foreach (KeyValuePair<string, LevelState> levelState in CurrentChapterState.LevelStates)
		{
			levelState.Value?.EnemyPoints?.Clear();
			levelState.Value?.ItemLevelStates?.Clear();
		}
		CurrentChapterState.LevelStates.Clear();
	}

	public void ClearAllState()
	{
		ClearGlobalState();
		ExitChapter();
	}
}
