using System;

namespace Mijing;

[Serializable]
public struct MijingDifficultyFormulaConfig
{
	public MijingFormulaParam EnemyHealth;

	public MijingFormulaParam EnemyDamage;

	public MijingFormulaParam PlayerDropRate;

	public MijingFormulaParam EnemyDamageReduction;

	public MijingFormulaParam EnemyPenetration;

	public MijingFormulaParam EnterPrice;

	public MijingFormulaParam EnemyXp;

	public MijingFormulaParam RareItemDropRate;

	public MijingFormulaParam WP_DMG;

	public MijingFormulaParam WP_PRC;

	public MijingFormulaParam SPC_DMG;
}
