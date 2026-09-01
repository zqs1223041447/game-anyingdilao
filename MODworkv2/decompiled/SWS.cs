using FinkFramework.Runtime.Utils;
using UI.DebugUI;
using UnityEngine;

public static class SWS
{
	public static void SS()
	{
		LogUtil.Info("log");
	}

	public static void SS(int a)
	{
		Debug.Log(a);
	}

	public static void SS(float a)
	{
		Debug.Log(a);
	}

	public static void SS(string msg)
	{
		LogUtil.Info(msg);
	}

	public static void SSS()
	{
		UILog.L("log");
	}

	public static void SSS(string msg)
	{
		UILog.L(msg);
	}

	public static bool GetBool(int A)
	{
		if (A == 0)
		{
			return true;
		}
		return false;
	}

	public static float DistanceRandom(float dis)
	{
		if (dis <= 1.05f || (dis > 1.05f && dis <= 2.1f))
		{
			return Random.Range(0.5f, 1f);
		}
		return Random.Range(0.8f, 1f);
	}

	public static DamageType DMtype(int A)
	{
		return A switch
		{
			0 => DamageType.fire, 
			1 => DamageType.frozen, 
			2 => DamageType.thunder, 
			3 => DamageType.poison, 
			4 => DamageType.physics, 
			5 => DamageType.shadow, 
			_ => DamageType.fire, 
		};
	}

	public static string El_Name(DamageType type)
	{
		return type switch
		{
			DamageType.fire => "fire", 
			DamageType.frozen => "frozen", 
			DamageType.thunder => "thunder", 
			DamageType.poison => "poison", 
			DamageType.physics => "physics", 
			DamageType.shadow => "shadow", 
			_ => string.Empty, 
		};
	}

	public static string El_Name(int A)
	{
		return A switch
		{
			0 => "fire", 
			1 => "frozen", 
			2 => "thunder", 
			3 => "poison", 
			4 => "physics", 
			5 => "shadow", 
			_ => string.Empty, 
		};
	}

	public static string El_DMG(int A)
	{
		return A switch
		{
			0 => "fire damage", 
			1 => "frozen damage", 
			2 => "thunder damage", 
			3 => "poison damage", 
			4 => "physics damage", 
			5 => "shadow damage", 
			_ => string.Empty, 
		};
	}

	public static string El_DMG(DamageType type)
	{
		return type switch
		{
			DamageType.fire => "fire damage", 
			DamageType.frozen => "frozen damage", 
			DamageType.thunder => "thunder damage", 
			DamageType.poison => "poison damage", 
			DamageType.physics => "physics damage", 
			DamageType.shadow => "shadow damage", 
			_ => string.Empty, 
		};
	}

	public static string El_Chuan(int A)
	{
		return A switch
		{
			0 => "fire chuan", 
			1 => "frozen chuan", 
			2 => "thunder chuan", 
			3 => "poison chuan", 
			4 => "physics chuan", 
			5 => "shadow chuan", 
			_ => string.Empty, 
		};
	}

	public static string El_Chuan(DamageType type)
	{
		return type switch
		{
			DamageType.fire => "fire chuan", 
			DamageType.frozen => "frozen chuan", 
			DamageType.thunder => "thunder chuan", 
			DamageType.poison => "poison chuan", 
			DamageType.physics => "physics chuan", 
			DamageType.shadow => "shadow chuan", 
			_ => string.Empty, 
		};
	}

	public static string El_Anti(int A)
	{
		return A switch
		{
			0 => "fire Anti", 
			1 => "frozen Anti", 
			2 => "thunder Anti", 
			3 => "poison Anti", 
			4 => "physics Anti", 
			5 => "shadow Anti", 
			_ => string.Empty, 
		};
	}

	public static string El_Anti(DamageType type)
	{
		return type switch
		{
			DamageType.fire => "fire Anti", 
			DamageType.frozen => "frozen Anti", 
			DamageType.thunder => "thunder Anti", 
			DamageType.poison => "poison Anti", 
			DamageType.physics => "physics Anti", 
			DamageType.shadow => "shadow Anti", 
			_ => string.Empty, 
		};
	}

	public static string Dot_R(DamageType type)
	{
		return type switch
		{
			DamageType.fire => "dianran Rate", 
			DamageType.frozen => "shuangdong Rate", 
			DamageType.thunder => "daodian Rate", 
			DamageType.poison => "zhongdu Rate", 
			DamageType.physics => "liuxue Rate", 
			DamageType.shadow => "kuwei Rate", 
			_ => string.Empty, 
		};
	}

	public static string Dot_DMG(DamageType type)
	{
		return type switch
		{
			DamageType.fire => "dianran damage", 
			DamageType.frozen => "shuangdong damage", 
			DamageType.thunder => "daodian damage", 
			DamageType.poison => "zhongdu damage", 
			DamageType.physics => "liuxue damage", 
			DamageType.shadow => "kuwei damage", 
			_ => string.Empty, 
		};
	}

	public static string SPC_name(string name, int A)
	{
		return A switch
		{
			0 => name + "0", 
			1 => name + "1", 
			2 => name + "2", 
			3 => name + "3", 
			4 => name + "4", 
			5 => name + "5", 
			_ => string.Empty, 
		};
	}

	public static float GetAT_Idle_Min(int type)
	{
		return type switch
		{
			0 => 0f, 
			1 => 0.5f, 
			2 => 1f, 
			3 => 1.5f, 
			4 => 2f, 
			5 => 3f, 
			_ => 0.5f, 
		};
	}

	public static float GetAT_Idle_Max(int type)
	{
		return type switch
		{
			0 => 1f, 
			1 => 1.5f, 
			2 => 2f, 
			3 => 3f, 
			4 => 4f, 
			5 => 6f, 
			_ => 0.5f, 
		};
	}
}
