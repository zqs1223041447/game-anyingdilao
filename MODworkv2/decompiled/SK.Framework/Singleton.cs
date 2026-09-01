using System;

namespace SK.Framework;

public static class Singleton<T> where T : class, ISingleton
{
	private static T instance;

	private static readonly object _lock = new object();

	public static T Instance
	{
		get
		{
			lock (_lock)
			{
				if (instance == null)
				{
					instance = Activator.CreateInstance<T>();
					instance.OnInit();
					Log.Info("<color=cyan><b>[SKFramework.Singleton.Info]</b></color> 单例[{0}]初始化完成", typeof(Singleton<T>).Name);
				}
			}
			return instance;
		}
	}

	public static void Dispose()
	{
		instance = null;
		Log.Info("<color=cyan><b>[SKFramework.Singleton.Info]</b></color> 单例[{0}]被释放", typeof(Singleton<T>).Name);
	}
}
