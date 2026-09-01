using System;

[Serializable]
public class Buff_PL_Layer
{
	public string BuffName;

	public int BuffType;

	public float BuffTime;

	public int LayerMax;

	public int LayerCur;

	public DamageType damageType;

	public int Type_Layer;

	public float Number_Layer;

	public int Type_Max;

	public float Number_Max;

	public Buff_PL_Layer()
	{
		InitDefault();
	}

	public void InitDefault()
	{
		BuffName = string.Empty;
		BuffType = 0;
		Type_Layer = 0;
		Type_Max = 0;
		BuffTime = 0f;
		LayerMax = 0;
		LayerCur = 0;
		damageType = DamageType.fire;
		Number_Layer = 0f;
		Number_Max = 0f;
	}

	public void Normalize()
	{
		if (BuffName == null)
		{
			BuffName = string.Empty;
		}
		if (BuffTime < 0f)
		{
			BuffTime = 0f;
		}
		if (LayerMax < 0)
		{
			LayerMax = 0;
		}
		if (LayerCur < 0)
		{
			LayerCur = 0;
		}
		if (LayerMax > 0 && LayerCur > LayerMax)
		{
			LayerCur = LayerMax;
		}
	}
}
