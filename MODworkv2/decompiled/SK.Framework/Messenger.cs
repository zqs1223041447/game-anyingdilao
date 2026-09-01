using System;
using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public class Messenger : MonoBehaviour
{
	private static Messenger instance;

	private Dictionary<int, List<Delegate>> intSubjects;

	private Dictionary<string, List<Delegate>> stringSubjects;

	private Dictionary<int, List<IMessage>> intMessages;

	private Dictionary<string, List<IMessage>> stringMessages;

	public static Messenger Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("[SKFramework.Messenger]").AddComponent<Messenger>();
				instance.intSubjects = new Dictionary<int, List<Delegate>>();
				instance.stringSubjects = new Dictionary<string, List<Delegate>>();
				instance.intMessages = new Dictionary<int, List<IMessage>>();
				instance.stringMessages = new Dictionary<string, List<IMessage>>();
				UnityEngine.Object.DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	private void ADD(int subject, Delegate callback)
	{
		if (!intSubjects.ContainsKey(subject))
		{
			intSubjects.Add(subject, new List<Delegate>());
		}
		intSubjects[subject].Add(callback);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]订阅主题为[{1}]的消息 订阅事件[{2}]", callback.Target, subject, callback.Method);
	}

	private void ADD(string subject, Delegate callback)
	{
		if (!stringSubjects.ContainsKey(subject))
		{
			stringSubjects.Add(subject, new List<Delegate>());
		}
		stringSubjects[subject].Add(callback);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]订阅主题为[{1}]的消息 订阅事件[{2}]", callback.Target, subject, callback.Method);
	}

	private bool DEL(int subject, Delegate callback)
	{
		if (intSubjects.TryGetValue(subject, out var value))
		{
			value.Remove(callback);
			if (value.Count == 0)
			{
				intSubjects.Remove(subject);
			}
			Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]订阅主题为[{1}]的消息 订阅事件[{2}]", callback.Target, subject, callback.Method);
			return true;
		}
		return false;
	}

	private bool DEL(string subject, Delegate callback)
	{
		if (stringSubjects.TryGetValue(subject, out var value))
		{
			value.Remove(callback);
			if (value.Count == 0)
			{
				stringSubjects.Remove(subject);
			}
			Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]订阅主题为[{1}]的消息 订阅事件[{2}]", callback.Target, subject, callback.Method);
			return true;
		}
		return false;
	}

	public static void ADD(int subject, Action callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T>(int subject, Action<T> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2>(int subject, Action<T1, T2> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2, T3>(int subject, Action<T1, T2, T3> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2, T3, T4>(int subject, Action<T1, T2, T3, T4> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2, T3, T4, T5>(int subject, Action<T1, T2, T3, T4, T5> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD(string subject, Action callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T>(string subject, Action<T> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2>(string subject, Action<T1, T2> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2, T3>(string subject, Action<T1, T2, T3> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2, T3, T4>(string subject, Action<T1, T2, T3, T4> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static void ADD<T1, T2, T3, T4, T5>(string subject, Action<T1, T2, T3, T4, T5> callback)
	{
		Instance.ADD(subject, (Delegate)callback);
	}

	public static bool DEL(int subject, Action callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T>(int subject, Action<T> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2>(int subject, Action<T1, T2> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2, T3>(int subject, Action<T1, T2, T3> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2, T3, T4>(int subject, Action<T1, T2, T3, T4> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2, T3, T4, T5>(int subject, Action<T1, T2, T3, T4, T5> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL(string subject, Action callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T>(string subject, Action<T> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2>(string subject, Action<T1, T2> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2, T3>(string subject, Action<T1, T2, T3> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2, T3, T4>(string subject, Action<T1, T2, T3, T4> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static bool DEL<T1, T2, T3, T4, T5>(string subject, Action<T1, T2, T3, T4, T5> callback)
	{
		return Instance.DEL(subject, (Delegate)callback);
	}

	public static void Send(int subject)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布", subject);
		if (!Instance.intSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action)
			{
				(value[i] as Action)();
			}
		}
	}

	public static void Send<T>(int subject, T t)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数: {1}", subject, t);
		if (!Instance.intSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T>)
			{
				(value[i] as Action<T>)(t);
			}
		}
	}

	public static void Send<T1, T2>(int subject, T1 t1, T2 t2)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2}", subject, t1, t2);
		if (!Instance.intSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2>)
			{
				(value[i] as Action<T1, T2>)(t1, t2);
			}
		}
	}

	public static void Send<T1, T2, T3>(int subject, T1 t1, T2 t2, T3 t3)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2} 参数3:{3}", subject, t1, t2, t3);
		if (!Instance.intSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2, T3>)
			{
				(value[i] as Action<T1, T2, T3>)(t1, t2, t3);
			}
		}
	}

	public static void Send<T1, T2, T3, T4>(int subject, T1 t1, T2 t2, T3 t3, T4 t4)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4}", subject, t1, t2, t3, t4);
		if (!Instance.intSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2, T3, T4>)
			{
				(value[i] as Action<T1, T2, T3, T4>)(t1, t2, t3, t4);
			}
		}
	}

	public static void Send<T1, T2, T3, T4, T5>(int subject, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4} 参数5:{5}", subject, t1, t2, t3, t4, t5);
		if (!Instance.intSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2, T3, T4, T5>)
			{
				(value[i] as Action<T1, T2, T3, T4, T5>)(t1, t2, t3, t4, t5);
			}
		}
	}

	public static void Send(string subject)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布", subject);
		if (!Instance.stringSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action)
			{
				(value[i] as Action)();
			}
		}
	}

	public static void Send<T>(string subject, T t)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数:{1}", subject, t);
		if (!Instance.stringSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T>)
			{
				(value[i] as Action<T>)(t);
			}
		}
	}

	public static void Send<T1, T2>(string subject, T1 t1, T2 t2)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2}", subject, t1, t2);
		if (!Instance.stringSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2>)
			{
				(value[i] as Action<T1, T2>)(t1, t2);
			}
		}
	}

	public static void Send<T1, T2, T3>(string subject, T1 t1, T2 t2, T3 t3)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2} 参数3:{3}", subject, t1, t2, t3);
		if (!Instance.stringSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2, T3>)
			{
				(value[i] as Action<T1, T2, T3>)(t1, t2, t3);
			}
		}
	}

	public static void Send<T1, T2, T3, T4>(string subject, T1 t1, T2 t2, T3 t3, T4 t4)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4}", subject, t1, t2, t3, t4);
		if (!Instance.stringSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2, T3, T4>)
			{
				(value[i] as Action<T1, T2, T3, T4>)(t1, t2, t3, t4);
			}
		}
	}

	public static void Send<T1, T2, T3, T4, T5>(string subject, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 主题为[{0}]的消息发布 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4} 参数5:{5}", subject, t1, t2, t3, t4, t5);
		if (!Instance.stringSubjects.TryGetValue(subject, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Action<T1, T2, T3, T4, T5>)
			{
				(value[i] as Action<T1, T2, T3, T4, T5>)(t1, t2, t3, t4, t5);
			}
		}
	}

	public static void Pack<T>(int identifier, T t)
	{
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T> item = new Message<T>
		{
			content = t
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数:{1}", identifier, t);
	}

	public static void Pack<T1, T2>(int identifier, T1 t1, T2 t2)
	{
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2> item = new Message<T1, T2>
		{
			content1 = t1,
			content2 = t2
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2}", identifier, t1, t2);
	}

	public static void Pack<T1, T2, T3>(int identifier, T1 t1, T2 t2, T3 t3)
	{
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2, T3> item = new Message<T1, T2, T3>
		{
			content1 = t1,
			content2 = t2,
			content3 = t3
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2} 参数3:{3}", identifier, t1, t2, t3);
	}

	public static void Pack<T1, T2, T3, T4>(int identifier, T1 t1, T2 t2, T3 t3, T4 t4)
	{
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2, T3, T4> item = new Message<T1, T2, T3, T4>
		{
			content1 = t1,
			content2 = t2,
			content3 = t3,
			content4 = t4
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4}", identifier, t1, t2, t3, t4);
	}

	public static void Pack<T1, T2, T3, T4, T5>(int identifier, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
	{
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2, T3, T4, T5> item = new Message<T1, T2, T3, T4, T5>
		{
			content1 = t1,
			content2 = t2,
			content3 = t3,
			content4 = t4,
			content5 = t5
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4} 参数5:{5}", identifier, t1, t2, t3, t4, t5);
	}

	public static void Pack<T>(string identifier, T t)
	{
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T> item = new Message<T>
		{
			content = t
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数:{1}", identifier, t);
	}

	public static void Pack<T1, T2>(string identifier, T1 t1, T2 t2)
	{
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2> item = new Message<T1, T2>
		{
			content1 = t1,
			content2 = t2
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2}", identifier, t1, t2);
	}

	public static void Pack<T1, T2, T3>(string identifier, T1 t1, T2 t2, T3 t3)
	{
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2, T3> item = new Message<T1, T2, T3>
		{
			content1 = t1,
			content2 = t2,
			content3 = t3
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2} 参数3:{3}", identifier, t1, t2, t3);
	}

	public static void Pack<T1, T2, T3, T4>(string identifier, T1 t1, T2 t2, T3 t3, T4 t4)
	{
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2, T3, T4> item = new Message<T1, T2, T3, T4>
		{
			content1 = t1,
			content2 = t2,
			content3 = t3,
			content4 = t4
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4}", identifier, t1, t2, t3, t4);
	}

	public static void Pack<T1, T2, T3, T4, T5>(string identifier, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
	{
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.ContainsKey(identifier))
		{
			dictionary.Add(identifier, new List<IMessage>());
		}
		Message<T1, T2, T3, T4, T5> item = new Message<T1, T2, T3, T4, T5>
		{
			content1 = t1,
			content2 = t2,
			content3 = t3,
			content4 = t4,
			content5 = t5
		};
		dictionary[identifier].Add(item);
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> 标识符为[{0}]的消息打包 参数1:{1} 参数2:{2} 参数3:{3} 参数4:{4} 参数5:{5}", identifier, t1, t2, t3, t4, t5);
	}

	public static void Unpack<T>(int identifier, Action<T> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T>)
			{
				Message<T> message = value[i] as Message<T>;
				callback(message.content);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2>(int identifier, Action<T1, T2> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2>)
			{
				Message<T1, T2> message = value[i] as Message<T1, T2>;
				callback(message.content1, message.content2);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2, T3>(int identifier, Action<T1, T2, T3> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2, T3>)
			{
				Message<T1, T2, T3> message = value[i] as Message<T1, T2, T3>;
				callback(message.content1, message.content2, message.content3);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2, T3, T4>(int identifier, Action<T1, T2, T3, T4> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2, T3, T4>)
			{
				Message<T1, T2, T3, T4> message = value[i] as Message<T1, T2, T3, T4>;
				callback(message.content1, message.content2, message.content3, message.content4);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2, T3, T4, T5>(int identifier, Action<T1, T2, T3, T4, T5> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<int, List<IMessage>> dictionary = Instance.intMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2, T3, T4, T5>)
			{
				Message<T1, T2, T3, T4, T5> message = value[i] as Message<T1, T2, T3, T4, T5>;
				callback(message.content1, message.content2, message.content3, message.content4, message.content5);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T>(string identifier, Action<T> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T>)
			{
				Message<T> message = value[i] as Message<T>;
				callback(message.content);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2>(string identifier, Action<T1, T2> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2>)
			{
				Message<T1, T2> message = value[i] as Message<T1, T2>;
				callback(message.content1, message.content2);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2, T3>(string identifier, Action<T1, T2, T3> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2, T3>)
			{
				Message<T1, T2, T3> message = value[i] as Message<T1, T2, T3>;
				callback(message.content1, message.content2, message.content3);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2, T3, T4>(string identifier, Action<T1, T2, T3, T4> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2, T3, T4>)
			{
				Message<T1, T2, T3, T4> message = value[i] as Message<T1, T2, T3, T4>;
				callback(message.content1, message.content2, message.content3, message.content4);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}

	public static void Unpack<T1, T2, T3, T4, T5>(string identifier, Action<T1, T2, T3, T4, T5> callback)
	{
		Log.Info("<color=cyan><b>[SKFramework.Messenger.Info]</b></color> [{0}]拆包标识符为[{1}]的消息 拆包事件[{2}]", callback.Target, identifier, callback.Method);
		Dictionary<string, List<IMessage>> dictionary = Instance.stringMessages;
		if (!dictionary.TryGetValue(identifier, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i] is Message<T1, T2, T3, T4, T5>)
			{
				Message<T1, T2, T3, T4, T5> message = value[i] as Message<T1, T2, T3, T4, T5>;
				callback(message.content1, message.content2, message.content3, message.content4, message.content5);
				value.RemoveAt(i);
				i--;
			}
		}
		if (value.Count == 0)
		{
			dictionary.Remove(identifier);
		}
	}
}
