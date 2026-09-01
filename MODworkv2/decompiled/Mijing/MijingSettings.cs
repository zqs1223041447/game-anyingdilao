using System;
using UI.Panels;
using UnityEngine;

namespace Mijing;

[CreateAssetMenu(fileName = "秘境数据配置", menuName = "全局项目设置/秘境数据配置")]
public class MijingSettings : ScriptableObject
{
	public MijingDifficultyFormulaConfig Easy;

	public MijingDifficultyFormulaConfig Medium;

	public MijingDifficultyFormulaConfig Hard;

	public MijingDifficultyFormulaConfig Master;

	[Header("进入下一层需要的分数")]
	public int needScore;

	[Header("怪物品质 0 获得分数")]
	public int EmScore0;

	[Header("怪物品质 1 获得分数")]
	public int EmScore1;

	[Header("怪物品质 2 获得分数")]
	public int EmScore2;

	[Header("怪物品质 3 获得分数")]
	public int EmScore3;

	[Header("怪物品质 4 获得分数")]
	public int EmScore4;

	[Header("怪物品质 5 获得分数")]
	public int EmScore5;

	[Header("间隔多少层保存进度")]
	public int intervalFloorNum = 5;

	public MijingDifficultyFormulaConfig GetDifficultyConfig(DifficultType difficulty)
	{
		return difficulty switch
		{
			DifficultType.Easy => Easy, 
			DifficultType.Medium => Medium, 
			DifficultType.Hard => Hard, 
			DifficultType.Master => Master, 
			_ => throw new ArgumentOutOfRangeException("difficulty", difficulty, null), 
		};
	}
}
