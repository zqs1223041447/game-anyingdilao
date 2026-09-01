using System;
using System.Collections.Generic;
using System.Reflection;

namespace SK.Framework;

public static class ObjectPool
{
	public static T Allocate<T>() where T : IPoolable, new()
	{
		return ObjectPool<T>.Instance.Allocate();
	}

	public static bool Recycle<T>(T t) where T : IPoolable, new()
	{
		return ObjectPool<T>.Instance.Recycle(t);
	}

	public static void Release<T>() where T : IPoolable, new()
	{
		ObjectPool<T>.Instance.Release();
	}

	public static void SetMaxCacheCount<T>(int maxCacheCount) where T : IPoolable, new()
	{
		ObjectPool<T>.Instance.MaxCacheCount = maxCacheCount;
	}
}
public class ObjectPool<T> : IObjectPool<T> where T : IPoolable, new()
{
	private static ObjectPool<T> instance;

	private int maxCacheCount = 9;

	private readonly Stack<T> pool = new Stack<T>();

	public static ObjectPool<T> Instance
	{
		get
		{
			if (instance == null)
			{
				if (Array.FindIndex(typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public), (ConstructorInfo m) => m.GetParameters().Length == 0) == -1)
				{
					Log.Error("<color=red><b>[SKFramework.ObjectPool.Error]</b></color> [{0}]类型不具有无参构造函数", typeof(T).Name);
				}
				else
				{
					instance = Activator.CreateInstance<ObjectPool<T>>();
				}
			}
			return instance;
		}
	}

	public int CurrentCacheCount => pool.Count;

	public int MaxCacheCount
	{
		get
		{
			return maxCacheCount;
		}
		set
		{
			if (maxCacheCount == value)
			{
				return;
			}
			maxCacheCount = value;
			if (maxCacheCount > 0 && maxCacheCount < pool.Count)
			{
				for (int num = pool.Count - maxCacheCount; num > 0; num--)
				{
					pool.Pop();
				}
			}
			Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]最大缓存数量设置为[{1}]", typeof(ObjectPool<T>).Name, value);
		}
	}

	public T Allocate()
	{
		T result = ((pool.Count > 0) ? pool.Pop() : new T());
		result.IsRecycled = false;
		Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]分配对象 当前池中数量[{1}]", typeof(ObjectPool<T>).Name, pool.Count);
		return result;
	}

	public bool Recycle(T t)
	{
		if (t == null || t.IsRecycled)
		{
			return false;
		}
		t.IsRecycled = true;
		t.OnRecycled();
		if (pool.Count < maxCacheCount)
		{
			pool.Push(t);
		}
		Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]回收对象 当前池中数量[{1}]", typeof(ObjectPool<T>).Name, pool.Count);
		return true;
	}

	public void Release()
	{
		pool.Clear();
		instance = null;
		Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]被释放", typeof(ObjectPool<T>).Name);
	}
}
