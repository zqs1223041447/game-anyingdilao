namespace Data.RuntimeData;

public static class GlobalRuntimeData
{
	public static long PendingDeathLostMoney;

	public static bool HasPendingDeathLostMoney;

	public static void SetDeathLostMoney(long money)
	{
		PendingDeathLostMoney = money;
		HasPendingDeathLostMoney = true;
	}

	public static void ClearDeathLostMoney()
	{
		PendingDeathLostMoney = 0L;
		HasPendingDeathLostMoney = false;
	}
}
