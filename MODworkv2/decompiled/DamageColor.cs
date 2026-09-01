using System.Collections.Generic;

public static class DamageColor
{
	public static Dictionary<DamageType, string> Colors { get; } = new Dictionary<DamageType, string>
	{
		{
			DamageType.fire,
			"#FF0000"
		},
		{
			DamageType.frozen,
			"#43B7FF"
		},
		{
			DamageType.thunder,
			"#FFF242"
		},
		{
			DamageType.poison,
			"#62FF2F"
		},
		{
			DamageType.physics,
			"#EEC4F1"
		},
		{
			DamageType.shadow,
			"#AC53FF"
		}
	};

}
