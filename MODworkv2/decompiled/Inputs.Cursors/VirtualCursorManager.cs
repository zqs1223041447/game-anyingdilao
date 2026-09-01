using System;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inputs.Cursors;

public class VirtualCursorManager : SingletonMonoGlobal<VirtualCursorManager>
{
	[Header("场景规则")]
	[SerializeField]
	private string homeSceneName = "HomeScene";

	[SerializeField]
	private string levelSceneName = "LevelScene";

	private bool lastShouldUseVirtualCursor;

	public static bool HasActiveUI
	{
		get
		{
			if (SingletonMonoScope<GameUIManager>.HasInstance)
			{
				if (!SingletonMonoScope<GameUIManager>.Instance.Opened_IV && !SingletonMonoScope<GameUIManager>.Instance.Opened_shop && !SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse && !SingletonMonoScope<GameUIManager>.Instance.Opened_Talent && !SingletonMonoScope<GameUIManager>.Instance.Opened_weapon)
				{
					return SingletonMonoScope<GameUIManager>.Instance.Opened_baoshi;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsSceneAllowed
	{
		get
		{
			string text = SceneManager.GetActiveScene().name;
			if (!(text == homeSceneName))
			{
				return text == levelSceneName;
			}
			return true;
		}
	}

	public bool ShouldUseVirtualCursor
	{
		get
		{
			if (!IsSceneAllowed)
			{
				return false;
			}
			if (!HasActiveUI)
			{
				return false;
			}
			if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				return false;
			}
			return true;
		}
	}

	public static event Action<bool> OnVirtualCursorStateChanged;

	public void Init()
	{
		lastShouldUseVirtualCursor = ShouldUseVirtualCursor;
		LogUtil.Info("虚拟全局光标管理器初始化完成");
	}

	private void Update()
	{
		bool flag = (GamepadUINavigationManager.BlockGamepadUIInput = ShouldUseVirtualCursor);
		if (flag != lastShouldUseVirtualCursor)
		{
			lastShouldUseVirtualCursor = flag;
			VirtualCursorManager.OnVirtualCursorStateChanged?.Invoke(flag);
		}
	}
}
