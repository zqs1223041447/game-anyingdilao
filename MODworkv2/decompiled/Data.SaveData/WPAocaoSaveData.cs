using System;

namespace Data.SaveData;

[Serializable]
public class WPAocaoSaveData
{
	public bool HasAocao;

	public bool HasBaoshi;

	public string Name;

	public int Type;

	public int UseType;

	public int BS_Quality;

	public float Number;

	public static WPAocaoSaveData FromRuntime(WPAocao aocao)
	{
		if (aocao == null)
		{
			return null;
		}
		return new WPAocaoSaveData
		{
			HasAocao = aocao.HasAocao,
			HasBaoshi = aocao.HasBaoshi,
			Name = aocao.Name,
			Type = aocao.Type,
			UseType = aocao.UseType,
			BS_Quality = aocao.BS_Quality,
			Number = aocao.Number
		};
	}

	public void ApplyToRuntime(WPAocao aocao)
	{
		if (aocao != null)
		{
			aocao.HasAocao = HasAocao;
			aocao.HasBaoshi = HasBaoshi;
			aocao.Name = Name;
			aocao.Type = Type;
			aocao.UseType = UseType;
			aocao.BS_Quality = BS_Quality;
			aocao.Number = Number;
		}
	}
}
