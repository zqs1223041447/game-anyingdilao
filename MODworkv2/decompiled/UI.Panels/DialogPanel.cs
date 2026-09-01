using System;
using System.Collections.Generic;
using Dialog;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Inputs;
using Interact;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Panels;

public class DialogPanel : GamepadSelectablePanel
{
	[Header("UI")]
	[SerializeField]
	private Text dialogText;

	[SerializeField]
	private Button nextButton;

	[SerializeField]
	private Image arrow;

	private string currentDialogId;

	private readonly List<DialogLineData> currentLines = new List<DialogLineData>();

	private int currentLineIndex;

	private bool isCurrentLineFullyShown;

	public override void OnShow()
	{
		base.OnShow();
		SetFirstSelected(nextButton);
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = false;
		}
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			InteractionManager.AllInteractToggle = false;
		}
	}

	public override bool OnCancel()
	{
		Singleton<UIManager>.Instance.HidePanel<DialogPanel>();
		return true;
	}

	public override void OnHide()
	{
		base.OnHide();
		if ((bool)EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			SingletonMonoScope<InputManager>.Instance.PrepareGameplayInputUnlock(suppressMovement: false);
			InputManager.AllActionToggle = true;
		}
		GamepadUINavigationManager.BlockGamepadUIInput = false;
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			SingletonMonoScope<InteractionManager>.Instance.ClearAllHover();
			InteractionManager.BlockInteractUntilRelease(left: true, right: true, submit: true, cancel: true);
			InteractionManager.AllInteractToggle = true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if ((bool)nextButton)
		{
			nextButton.onClick.RemoveListener(OnClickNextButton);
			nextButton.onClick.AddListener(OnClickNextButton);
		}
	}

	protected void OnDestroy()
	{
		if ((bool)nextButton)
		{
			nextButton.onClick.RemoveListener(OnClickNextButton);
		}
	}

	public void InitDialog(string dialogId, IList<DialogLineData> lines)
	{
		currentDialogId = dialogId;
		currentLines.Clear();
		if (lines != null)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				DialogLineData dialogLineData = lines[i];
				if (dialogLineData != null)
				{
					currentLines.Add(dialogLineData);
				}
			}
		}
		currentLineIndex = 0;
		isCurrentLineFullyShown = false;
		if ((bool)dialogText)
		{
			dialogText.text = string.Empty;
		}
		RefreshArrow(show: false);
		if (string.IsNullOrEmpty(currentDialogId) || currentLines.Count == 0)
		{
			ClosePanelDirect();
		}
		else
		{
			ShowCurrentLine();
		}
	}

	private void OnClickNextButton()
	{
		if (currentLines.Count != 0 && isCurrentLineFullyShown)
		{
			TryInvokeCurrentLineEvent();
			GoNextLine();
		}
	}

	private void ShowCurrentLine()
	{
		if (currentLineIndex < 0 || currentLineIndex >= currentLines.Count)
		{
			if (SingletonMonoScope<DialogManager>.HasInstance)
			{
				SingletonMonoScope<DialogManager>.Instance.MarkCompleted(currentDialogId);
			}
			ClosePanelDirect();
			return;
		}
		string text = ResolveLineContent(currentLines[currentLineIndex]);
		if ((bool)dialogText)
		{
			dialogText.text = text;
		}
		isCurrentLineFullyShown = true;
		RefreshArrow(show: true);
	}

	private static string ResolveLineContent(DialogLineData line)
	{
		if (line == null)
		{
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(line.ContentKey))
		{
			if (line.FormatArgs != null && line.FormatArgs.Length != 0)
			{
				return LOC.MM.GetDialogFormat(line.ContentKey, line.FormatArgs);
			}
			return LOC.MM.GetDialog(line.ContentKey);
		}
		return line.Content ?? string.Empty;
	}

	private void GoNextLine()
	{
		currentLineIndex++;
		ShowCurrentLine();
	}

	private void TryInvokeCurrentLineEvent()
	{
		if (currentLineIndex < 0 || currentLineIndex >= currentLines.Count)
		{
			return;
		}
		DialogLineData dialogLineData = currentLines[currentLineIndex];
		if (dialogLineData == null || dialogLineData.OnLineFinished == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(dialogLineData.EventKey))
		{
			dialogLineData.OnLineFinished();
		}
		else if (dialogLineData.TriggerOnce)
		{
			if (!SingletonMonoScope<DialogManager>.Instance.HasTriggered(dialogLineData.EventKey))
			{
				try
				{
					dialogLineData.OnLineFinished();
					SingletonMonoScope<DialogManager>.Instance.MarkTriggered(dialogLineData.EventKey);
				}
				catch (Exception arg)
				{
					LogUtil.Error($"对话事件执行失败，EventKey = {dialogLineData.EventKey}\n{arg}");
				}
			}
		}
		else
		{
			dialogLineData.OnLineFinished();
		}
	}

	private void RefreshArrow(bool show)
	{
		if ((bool)arrow)
		{
			arrow.enabled = show;
		}
	}

	private static void ClosePanelDirect()
	{
		Singleton<UIManager>.Instance.HidePanel<DialogPanel>();
	}
}
