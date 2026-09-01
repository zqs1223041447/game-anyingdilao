using System;
using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public static class MonoObjectPool
{
	public static T Allocate<T>() where T : MonoBehaviour, IPoolable
	{
		return MonoObjectPool<T>.Instance.Allocate();
	}

	public static bool Recycle<T>(T t) where T : MonoBehaviour, IPoolable
	{
		return MonoObjectPool<T>.Instance.Recycle(t);
	}

	public static void Release<T>() where T : MonoBehaviour, IPoolable
	{
		MonoObjectPool<T>.Instance.Release();
	}

	public static void SetMaxCacheCount<T>(int maxCacheCount) where T : MonoBehaviour, IPoolable
	{
		MonoObjectPool<T>.Instance.MaxCacheCount = maxCacheCount;
	}

	public static void CreateBy<T>(Func<T> createMethod) where T : MonoBehaviour, IPoolable
	{
		MonoObjectPool<T>.Instance.CreateBy(createMethod);
	}
}
public class MonoObjectPool<T> : IObjectPool<T> where T : MonoBehaviour, IPoolable
{
	private static MonoObjectPool<T> instance;

	private int maxCacheCount = 9;

	private readonly Stack<T> pool = new Stack<T>();

	private Func<T> createMethod;

	public static MonoObjectPool<T> Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Activator.CreateInstance<MonoObjectPool<T>>();
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
					UnityEngine.Object.Destroy(pool.Pop().gameObject);
				}
			}
			Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]最大缓存数量设置为[{1}]", typeof(MonoObjectPool<T>).Name, value);
		}
	}

	public T Allocate()
	{
		T val = ((pool.Count > 0) ? pool.Pop() : ((createMethod != null) ? createMethod() : new GameObject().AddComponent<T>()));
		val.hideFlags = HideFlags.HideInHierarchy;
		val.IsRecycled = false;
		Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]分配对象 当前池中数量[{1}]", typeof(MonoObjectPool<T>).Name, pool.Count);
		return val;
	}

	public bool Recycle(T t)
	{
		if (null == t || t.IsRecycled)
		{
			return false;
		}
		t.IsRecycled = true;
		t.OnRecycled();
		if (pool.Count < maxCacheCount)
		{
			pool.Push(t);
		}
		else
		{
			UnityEngine.Object.Destroy(t.gameObject);
		}
		Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]回收对象 当前池中数量[{1}]", typeof(MonoObjectPool<T>).Name, pool.Count);
		return true;
	}

	public void Release()
	{
		foreach (T item in pool)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		pool.Clear();
		instance = null;
		Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]被释放", typeof(MonoObjectPool<T>).Name);
	}

	public void CreateBy(Func<T> createMethod)
	{
		this.createMethod = createMethod;
		Log.Info("<color=cyan><b>[SKFramework.ObjectPool.Info]</b></color> 对象池[{0}]设置创建方法 {1}", typeof(MonoObjectPool<T>).Name, createMethod.Method);
	}
}
