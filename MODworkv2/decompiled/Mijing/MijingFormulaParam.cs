using System;
using UnityEngine;

namespace Mijing;

[Serializable]
public struct MijingFormulaParam
{
	[Header("基础值")]
	[Min(0.01f)]
	public float BaseMultiplier;

	[Header("曲线倍率")]
	[Min(0.01f)]
	public float FloorGrowthMultiplier;

	[Header("数值上限(0为无上限)")]
	[Min(0f)]
	public float MaxMultiplier;

	public float Evaluate(int floor)
	{
		int num = Mathf.Max(1, floor);
		float num2 = BaseMultiplier * Mathf.Pow(FloorGrowthMultiplier, num - 1);
		if (MaxMultiplier > 0f)
		{
			num2 = Mathf.Min(num2, MaxMultiplier);
		}
		return num2;
	}

	public float EvaluateFromFirstFloorWithFallback(int floor, float defaultBaseMultiplier, float defaultFloorGrowthMultiplier)
	{
		float num = ((BaseMultiplier > 0f) ? BaseMultiplier : defaultBaseMultiplier);
		float f = ((FloorGrowthMultiplier > 0f) ? FloorGrowthMultiplier : defaultFloorGrowthMultiplier);
		int num2 = Mathf.Max(1, floor);
		float num3 = num * Mathf.Pow(f, num2 - 1);
		if (MaxMultiplier > 0f)
		{
			num3 = Mathf.Min(num3, MaxMultiplier);
		}
		return num3;
	}
}
