using System.Collections.Generic;
using FinkFramework.Runtime.Utils;
using Level.StateData.LevelStates;

namespace Level.StateData.ChapterStates;

public class ChapterState
{
	public int ChapterId;

	public readonly Dictionary<string, LevelState> LevelStates = new Dictionary<string, LevelState>();

	public readonly PortalChapterState PortalStates = new PortalChapterState();

	public LevelState GetOrCreateLevelState(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			LogUtil.Error("ChapterState.GetOrCreateLevelState 失败：levelId 为空。");
			return null;
		}
		if (!LevelStates.TryGetValue(levelId, out var value))
		{
			value = new LevelState
			{
				LevelId = levelId,
				EnemyPoints = new Dictionary<string, EnemyPointLevelState>(),
				ItemLevelStates = new List<ItemLevelState>(),
				Interactables = new Dictionary<string, InteractableLevelState>(),
				ChestPoints = new Dictionary<string, ChestPointLevelState>()
			};
			LevelStates.Add(levelId, value);
		}
		return value;
	}
}
