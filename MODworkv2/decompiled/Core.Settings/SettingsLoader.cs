using FinkFramework.Runtime.Utils;
using UnityEngine;

namespace Core.Settings;

public static class SettingsLoader
{
	private static GlobalSettings _instance;

	public static GlobalSettings Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Resources.Load<GlobalSettings>("Settings/GlobalSettings");
				if (!_instance)
				{
					LogUtil.Error("GlobalSettings 未找到！");
				}
			}
			return _instance;
		}
	}
}
