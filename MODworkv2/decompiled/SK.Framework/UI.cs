using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SK.Framework;

public class UI : MonoBehaviour
{
	private static UI instance;

	private Dictionary<string, IUIView> viewDic;

	public static UI Instance
	{
		get
		{
			if (instance == null)
			{
				UI uI = Resources.Load<UI>("UI");
				if (null == uI)
				{
					Log.Error((object)"<color=red><b>[SKFramework.UI.Error]</b></color> 加载UI预制体失败");
				}
				else
				{
					instance = UnityEngine.Object.Instantiate(uI);
					instance.name = "[SKFramework.UI]";
					instance.viewDic = new Dictionary<string, IUIView>();
					string[] names = Enum.GetNames(typeof(ViewLevel));
					for (int num = names.Length - 1; num >= 0; num--)
					{
						GameObject obj = new GameObject(names[num]);
						obj.layer = LayerMask.NameToLayer("UI");
						obj.transform.SetParent(instance.transform, worldPositionStays: false);
						RectTransform rectTransform = obj.AddComponent<RectTransform>();
						rectTransform.sizeDelta = instance.GetComponent<CanvasScaler>().referenceResolution;
						rectTransform.anchorMin = Vector2.zero;
						rectTransform.anchorMax = Vector2.one;
						Vector2 vector = (rectTransform.offsetMax = Vector2.zero);
						Vector2 offsetMin = vector;
						rectTransform.offsetMin = offsetMin;
						rectTransform.SetAsFirstSibling();
					}
					UnityEngine.Object.DontDestroyOnLoad(instance);
				}
			}
			return instance;
		}
	}

	public static Canvas Canvas => Instance.GetComponent<Canvas>();

	public static Camera Camera => Instance.GetComponentInChildren<Camera>();

	public static Vector2 Resolution => Instance.GetComponent<CanvasScaler>().referenceResolution;

	public bool LoadView(string viewName, string viewResourcePath, ViewLevel level, out IUIView view, IViewData data = null, bool instant = false)
	{
		if (!viewDic.TryGetValue(viewName, out view))
		{
			GameObject gameObject = Resources.Load<GameObject>(viewResourcePath);
			if (null != gameObject)
			{
				Log.Info("<color=cyan><b>[SKFramework.UI.Info]</b></color> 加载视图[{0}]", viewName);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.transform.SetParent(base.transform.GetChild((int)level), worldPositionStays: false);
				gameObject2.name = viewName;
				view = gameObject2.GetComponent<IUIView>();
				view.Name = viewName;
				view.Init(data, instant);
				viewDic.Add(viewName, view);
				return true;
			}
			Log.Error("<color=red><b>[SKFramework.UI.Error]</b></color> 加载视图[{0}]失败 {1}", viewName, viewResourcePath);
		}
		return false;
	}

	public IUIView ShowView(string viewName, IViewData data = null, bool instant = false)
	{
		if (viewDic.TryGetValue(viewName, out var value))
		{
			Log.Info("<color=cyan><b>[SKFramework.UI.Info]</b></color> 显示视图[{0}]", viewName);
			value.Show(data, instant);
			return value;
		}
		Log.Error("<color=red><b>[SKFramework.UI.Error]</b></color> 显示视图[{0}]失败: 不存在", viewName);
		return null;
	}

	public IUIView HideView(string viewName, bool instant = false)
	{
		if (viewDic.TryGetValue(viewName, out var value))
		{
			Log.Info("<color=cyan><b>[SKFramework.UI.Info]</b></color> 隐藏视图[{0}]", viewName);
			value.Hide(instant);
			return value;
		}
		Log.Error("<color=red><b>[SKFramework.UI.Error]</b></color> 隐藏视图[{0}]失败: 不存在", viewName);
		return null;
	}

	public IUIView GetView(string viewName)
	{
		viewDic.TryGetValue(viewName, out var value);
		return value;
	}

	public bool UnloadView(string viewName, bool instant = false)
	{
		if (viewDic.TryGetValue(viewName, out var value))
		{
			Log.Info("<color=cyan><b>[SKFramework.UI.Info]</b></color> 卸载视图[{0}]", viewName);
			viewDic.Remove(viewName);
			value.Unload(instant);
			return true;
		}
		Log.Error("<color=red><b>[SKFramework.UI.Error]</b></color> 卸载视图[{0}]失败: 不存在", viewName);
		return false;
	}

	public void UnloadAll()
	{
		Log.Info((object)"<color=cyan><b>[SKFramework.UI.Info]</b></color> 卸载所有视图");
		List<IUIView> list = new List<IUIView>();
		foreach (KeyValuePair<string, IUIView> item in viewDic)
		{
			list.Add(item.Value);
		}
		int num;
		for (num = 0; num < list.Count; num++)
		{
			list[num].Unload(instant: true);
			list.RemoveAt(num);
			num--;
		}
		viewDic.Clear();
	}

	public void Remove(string viewName)
	{
		if (viewDic.ContainsKey(viewName))
		{
			viewDic.Remove(viewName);
		}
	}
}
