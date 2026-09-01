using System;
using FinkFramework.Runtime.Singleton;

[Serializable]
public class PlayerDotData
{
	public int Every_Layer;

	public bool Crit_One;

	public int FJ;

	public int DMG_AddOne;

	public int All_LayerR;

	public bool Double_Layer;

	public bool Dot_Infect;

	public int Dot_Infect_Layer;

	public bool Dot_Infect_All;

	public bool YB;

	public bool YB_half;

	public int YB_Add;

	public int YB_MS;

	public int YS;

	public bool SL;

	public bool CM;

	public int MH;

	public bool ZZ;

	public int JY;

	public int Dead;

	public bool Dot_Crit;

	public int BoomDMGUp;

	public int LayerPRC;

	public int BE_CP;

	public int BF_DMG;

	public int DMG50;

	public int LowH_50;

	public int HighH_100;

	public int LowM_40;

	public int FrozenFoever;

	public int FrozenCut;

	public int Frozen30;

	public int FrozenHurtDMG;

	public bool FrozenForeverDot;

	public int Double_LayerLast
	{
		get
		{
			if (Double_Layer)
			{
				return 2;
			}
			return 1;
		}
	}

	public int LowH_50Last
	{
		get
		{
			if (SingletonMonoScope<PlayerManager>.Instance.HealStat.Cur < SingletonMonoScope<PlayerManager>.Instance.HealStat.Max * 0.5f)
			{
				return LowH_50;
			}
			return 0;
		}
	}

	public int HighH_100Last
	{
		get
		{
			if (SingletonMonoScope<PlayerManager>.Instance.HealStat.Cur + 1f > SingletonMonoScope<PlayerManager>.Instance.HealStat.Max)
			{
				return HighH_100;
			}
			return 0;
		}
	}

	public int LowM_40Last
	{
		get
		{
			if (SingletonMonoScope<PlayerManager>.Instance.ManaStat.Cur < SingletonMonoScope<PlayerManager>.Instance.ManaStat.Max * 0.4f)
			{
				return LowM_40;
			}
			return 0;
		}
	}

	public static PlayerDotData CreateDefault()
	{
		return new PlayerDotData();
	}
}
