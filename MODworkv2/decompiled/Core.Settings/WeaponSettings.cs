using UnityEngine;

namespace Core.Settings;

[CreateAssetMenu(fileName = "武器重铸配置", menuName = "全局项目设置/武器重铸配置")]
public class WeaponSettings : ScriptableObject
{
	[Header("UI设置")]
	[Tooltip("在物品名称显示 + X 时的字体颜色")]
	public Color textColor = Color.yellow;

	[Header("重铸设置")]
	[Tooltip("单个装备最多可进行多少次重铸")]
	public int Reb_CountMax = 1000;

	[Tooltip("重铸价格的基础值")]
	public float Reb_Price_Base = 300f;

	[Tooltip("重铸次数带来的价格成长倍率")]
	public float Reb_PriceUP_Count = 1.07f;

	[Tooltip("装备等级带来的价格成长倍率")]
	public float Reb_PriceUP_Level = 1.065f;

	[Header("强化设置")]
	[Tooltip("不同品质  单个装备最多可进行多少次强化")]
	public int maxZQ0 = 5;

	public int maxZQ1 = 5;

	public int maxZQ2 = 10;

	public int maxZQ3 = 10;

	public int maxZQ4 = 15;

	public int maxZQ5 = 15;

	public int maxZQ6 = 20;

	[Tooltip("强化价格的基础值")]
	public float ZQ_Price_Base = 300f;

	[Tooltip("强化次数带来的价格成长倍率")]
	public float ZQ_Price_Count = 1.08f;

	[Tooltip("装备等级带来的价格成长倍率")]
	public float ZQ_Price_Level = 1.065f;

	[Tooltip("每次强化提升的数值百分比-最小值")]
	public float ZQ_Min = 0.01f;

	[Tooltip("每次强化提升的数值百分比-最大值")]
	public float ZQ_Max = 0.025f;
}
