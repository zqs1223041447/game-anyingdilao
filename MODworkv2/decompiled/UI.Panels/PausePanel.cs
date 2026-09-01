using Core;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Inputs;
using Interact;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class PausePanel : GamepadSelectablePanel
{
	protected override void ClickBtn(string btnName)
	{
		base.ClickBtn(btnName);
		switch (btnName)
		{
		case "BackBtn":
			CloseAndResume();
			break;
		case "SettingBtn":
			Singleton<UIManager>.Instance.ShowExclusivePanel<SettingPanel>();
			break;
		case "MenuBtn":
			SaveAndBackToMenu().Forget();
			break;
		case "ExitBtn":
			SaveAndQuit().Forget();
			break;
		}
	}

	private async UniTaskVoid SaveAndBackToMenu()
	{
		bool flag = !SaveManager.HasRuntime;
		if (!flag)
		{
			flag = await SaveManager.SaveAndExitAndWaitIfNeeded();
		}
		if (flag)
		{
			await GameManager.BackToMenu();
		}
		else
		{
			LogUtil.Error("保存并返回主菜单失败，已留在当前游戏。");
		}
	}

	private async UniTaskVoid SaveAndQuit()
	{
		bool flag = SaveManager.HasRuntime;
		if (flag)
		{
			flag = !(await SaveManager.SaveAndExitAndWaitIfNeeded());
		}
		if (flag)
		{
			LogUtil.Error("保存并退出失败，已取消退出。");
		}
		else
		{
			Application.Quit();
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		SetFirstSelected(GetControl<Button>("BackBtn"));
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = false;
		}
	}

	public override bool OnCancel()
	{
		CloseAndResume();
		return true;
	}

	public static void CloseAndResume()
	{
		InteractionManager.BlockInteractUntilRelease(left: true, right: true, submit: true, cancel: true);
		Time.timeScale = 1f;
		Singleton<UIManager>.Instance.HidePanel<PausePanel>();
	}

	public override void OnHide()
	{
		base.OnHide();
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = true;
		}
	}
}
