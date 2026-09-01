using System;
using System.Collections.Generic;

namespace SK.Framework;

public static class DictionaryExtension
{
	public static Dictionary<K, V> Copy<K, V>(this Dictionary<K, V> self)
	{
		Dictionary<K, V> dictionary = new Dictionary<K, V>(self.Count);
		using Dictionary<K, V>.Enumerator enumerator = self.GetEnumerator();
		while (enumerator.MoveNext())
		{
			dictionary.Add(enumerator.Current.Key, enumerator.Current.Value);
		}
		return dictionary;
	}

	public static Dictionary<K, V> ForEach<K, V>(this Dictionary<K, V> self, Action<K, V> action)
	{
		using Dictionary<K, V>.Enumerator enumerator = self.GetEnumerator();
		while (enumerator.MoveNext())
		{
			action(enumerator.Current.Key, enumerator.Current.Value);
		}
		return self;
	}

	public static Dictionary<K, V> AddRange<K, V>(this Dictionary<K, V> self, Dictionary<K, V> target, bool isOverride = false)
	{
		foreach (KeyValuePair<K, V> item in target)
		{
			if (self.ContainsKey(item.Key) && isOverride)
			{
				self[item.Key] = item.Value;
			}
			else
			{
				self.Add(item.Key, item.Value);
			}
		}
		return self;
	}

	public static List<V> Value2List<K, V>(this Dictionary<K, V> self)
	{
		List<V> list = new List<V>(self.Count);
		foreach (KeyValuePair<K, V> item in self)
		{
			list.Add(item.Value);
		}
		return list;
	}

	public static V[] Value2Array<K, V>(this Dictionary<K, V> self)
	{
		V[] array = new V[self.Count];
		int num = -1;
		foreach (KeyValuePair<K, V> item in self)
		{
			array[++num] = item.Value;
		}
		return array;
	}

	public static bool TryAdd<K, V>(this Dictionary<K, V> self, K k, V v)
	{
		if (!self.ContainsKey(k))
		{
			self.Add(k, v);
			return true;
		}
		return false;
	}
}
