public static class SetDataUtil
{
	public static Set_DT Clone(Set_DT source)
	{
		if (source == null)
		{
			return null;
		}
		Set_DT set_DT = new Set_DT
		{
			SetID = source.SetID,
			SetName = source.SetName,
			BuffName = source.BuffName,
			BuffType = source.BuffType,
			BuffTime = source.BuffTime,
			LayerMax = source.LayerMax,
			TP_Layer = source.TP_Layer,
			NumberL = source.NumberL,
			TP_Max = source.TP_Max,
			NumberM = source.NumberM
		};
		if (source.Lit == null)
		{
			return set_DT;
		}
		set_DT.Lit = new Set_DT_Lit[source.Lit.Length];
		for (int i = 0; i < source.Lit.Length; i++)
		{
			Set_DT_Lit set_DT_Lit = source.Lit[i];
			set_DT.Lit[i] = ((set_DT_Lit == null) ? null : new Set_DT_Lit
			{
				MainTP = set_DT_Lit.MainTP,
				SkillName = set_DT_Lit.SkillName,
				Index = set_DT_Lit.Index,
				GlobleID = set_DT_Lit.GlobleID,
				EL = set_DT_Lit.EL,
				Number = set_DT_Lit.Number,
				LinkSK = set_DT_Lit.LinkSK
			});
		}
		return set_DT;
	}
}
