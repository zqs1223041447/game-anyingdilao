using System;

[Serializable]
public class PlayerRuntimeBuffModifier
{
	public float Damage;

	public float AttackSpeed;

	public float MoveSpeed;

	public float BJrate;

	public float BJDamage;

	public float DamageAnti;

	public float HealthPercent;

	public float DotDamage;

	public float DotTimeCut;

	public float CompanionDamage;

	public float CompanionAttackSpeed;

	public float GeDang;

	public float OrbDamage;

	public float TrapDamage;

	public float AllChuan;

	public readonly float[] ElementDamage = new float[6];

	public readonly float[] ElementChuan = new float[6];

	public readonly float[] ElementAnti = new float[6];

	public bool IsEmpty()
	{
		if (Damage != 0f || AttackSpeed != 0f || MoveSpeed != 0f || BJrate != 0f || BJDamage != 0f || DamageAnti != 0f || HealthPercent != 0f || DotDamage != 0f || DotTimeCut != 0f || CompanionDamage != 0f || CompanionAttackSpeed != 0f || GeDang != 0f || OrbDamage != 0f || TrapDamage != 0f || AllChuan != 0f)
		{
			return false;
		}
		for (int i = 0; i < 6; i++)
		{
			if (ElementDamage[i] != 0f || ElementChuan[i] != 0f || ElementAnti[i] != 0f)
			{
				return false;
			}
		}
		return true;
	}

	public PlayerRuntimeBuffModifier Clone()
	{
		PlayerRuntimeBuffModifier playerRuntimeBuffModifier = new PlayerRuntimeBuffModifier
		{
			Damage = Damage,
			AttackSpeed = AttackSpeed,
			MoveSpeed = MoveSpeed,
			BJrate = BJrate,
			BJDamage = BJDamage,
			DamageAnti = DamageAnti,
			HealthPercent = HealthPercent,
			DotDamage = DotDamage,
			DotTimeCut = DotTimeCut,
			CompanionDamage = CompanionDamage,
			CompanionAttackSpeed = CompanionAttackSpeed,
			GeDang = GeDang,
			OrbDamage = OrbDamage,
			TrapDamage = TrapDamage,
			AllChuan = AllChuan
		};
		for (int i = 0; i < 6; i++)
		{
			playerRuntimeBuffModifier.ElementDamage[i] = ElementDamage[i];
			playerRuntimeBuffModifier.ElementChuan[i] = ElementChuan[i];
			playerRuntimeBuffModifier.ElementAnti[i] = ElementAnti[i];
		}
		return playerRuntimeBuffModifier;
	}
}
