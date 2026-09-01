using UnityEngine;

namespace Core.Settings;

[CreateAssetMenu(fileName = "宝石加工配置", menuName = "全局项目设置/宝石加工配置")]
public class BaoshiSettings : ScriptableObject
{
	[Header("单个高级宝石需要合成消耗的初级宝石数量")]
	public int needCount = 5;

	[Space(20f)]
	[Header("合成1级宝石需要的价格-标准品质")]
	public int createPrice1 = 100;

	[Header("合成2级宝石需要的价格-精致品质")]
	public int createPrice2 = 500;

	[Header("合成3级宝石需要的价格-卓越品质")]
	public int createPrice3 = 1000;

	[Header("合成4级宝石需要的价格-无瑕品质")]
	public int createPrice4 = 3000;

	[Header("合成5级宝石需要的价格-完美品质")]
	public int createPrice5 = 8000;

	[Header("合成6级宝石需要的价格-史诗品质")]
	public int createPrice6 = 20000;

	[Header("合成7级宝石需要的价格-传奇品质")]
	public int createPrice7 = 50000;

	[Space(20f)]
	[Header("拆卸0级宝石需要的价格-裂开品质")]
	public int splitPrice0 = 100;

	[Header("拆卸1级宝石需要的价格-标准品质")]
	public int splitPrice1 = 500;

	[Header("拆卸2级宝石需要的价格-精致品质")]
	public int splitPrice2 = 1000;

	[Header("拆卸3级宝石需要的价格-卓越品质")]
	public int splitPrice3 = 3000;

	[Header("拆卸4级宝石需要的价格-无瑕品质")]
	public int splitPrice4 = 8000;

	[Header("拆卸5级宝石需要的价格-完美品质")]
	public int splitPrice5 = 15000;

	[Header("拆卸6级宝石需要的价格-史诗品质")]
	public int splitPrice6 = 30000;

	[Header("拆卸7级宝石需要的价格-传奇品质")]
	public int splitPrice7 = 80000;
}
