using System.Collections.Generic;
using Data.SaveData;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Mijing;
using UI.Panels;
using UnityEngine;

namespace Dialog;

public class DialogManager : SingletonMonoScope<DialogManager>
{
	private readonly HashSet<string> triggeredEvents = new HashSet<string>();

	private readonly HashSet<string> completedDialogs = new HashSet<string>();

	private readonly Dictionary<string, DialogData> dialogConfigs = new Dictionary<string, DialogData>();

	private readonly List<DialogCandidateData> mainNpcDialogCandidates = new List<DialogCandidateData>();

	private readonly List<string> mainNpcIdleLineKeys = new List<string>();

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		RegisterAllDialogs();
	}

	private void RegisterAllDialogs()
	{
		dialogConfigs.Clear();
		mainNpcDialogCandidates.Clear();
		RegisterTutorialDialog();
		RegisterMijingDialog();
		RegisterIdleDialog();
		RegisterMainNpcDialogCandidates();
	}

	public void RegisterDialog(DialogData dialogData)
	{
		if (dialogData == null || string.IsNullOrEmpty(dialogData.DialogId))
		{
			LogUtil.Warn("DialogManager.RegisterDialog 失败：dialogData 或 DialogId 为空。");
			return;
		}
		if (dialogData.Lines == null)
		{
			dialogData.Lines = new List<DialogLineData>();
		}
		dialogConfigs[dialogData.DialogId] = dialogData;
	}

	public DialogData GetDialogData(string dialogId)
	{
		if (string.IsNullOrEmpty(dialogId))
		{
			return null;
		}
		dialogConfigs.TryGetValue(dialogId, out var value);
		return value;
	}

	public void OpenDialog(string dialogId)
	{
		if (dialogId == "idle_dialog")
		{
			OpenRandomIdleDialog(dialogId);
			return;
		}
		DialogData dialogData = GetDialogData(dialogId);
		if (dialogData == null)
		{
			LogUtil.Warn("DialogManager.OpenDialog 失败：未找到对话配置，dialogId = " + dialogId);
			return;
		}
		if (dialogData.Lines == null || dialogData.Lines.Count == 0)
		{
			LogUtil.Warn("DialogManager.OpenDialog 失败：dialogId = " + dialogId + " 的对话内容为空。");
			return;
		}
		List<DialogLineData> list = new List<DialogLineData>(dialogData.Lines.Count);
		for (int i = 0; i < dialogData.Lines.Count; i++)
		{
			DialogLineData dialogLineData = dialogData.Lines[i];
			if (dialogLineData != null)
			{
				list.Add(dialogLineData);
			}
		}
		if (list.Count == 0)
		{
			LogUtil.Warn("DialogManager.OpenDialog 失败：dialogId = " + dialogId + " 的有效对话内容为空。");
		}
		else
		{
			Singleton<UIManager>.Instance.ShowPanel<DialogPanel>().InitDialog(dialogId, list);
		}
	}

	private void OpenRandomIdleDialog(string dialogId)
	{
		if (mainNpcIdleLineKeys.Count == 0)
		{
			LogUtil.Warn("DialogManager.OpenRandomIdleDialog 失败：idle 对话池为空。");
			return;
		}
		int index = Random.Range(0, mainNpcIdleLineKeys.Count);
		string contentKey = mainNpcIdleLineKeys[index];
		List<DialogLineData> lines = new List<DialogLineData>
		{
			new DialogLineData
			{
				ContentKey = contentKey
			}
		};
		Singleton<UIManager>.Instance.ShowPanel<DialogPanel>().InitDialog(dialogId, lines);
	}

	public bool HasTriggered(string eventKey)
	{
		if (string.IsNullOrEmpty(eventKey))
		{
			return false;
		}
		return triggeredEvents.Contains(eventKey);
	}

	public void MarkTriggered(string eventKey)
	{
		if (!string.IsNullOrEmpty(eventKey))
		{
			triggeredEvents.Add(eventKey);
		}
	}

	public bool HasCompleted(string dialogId)
	{
		if (string.IsNullOrEmpty(dialogId))
		{
			return false;
		}
		return completedDialogs.Contains(dialogId);
	}

	public void MarkCompleted(string dialogId)
	{
		if (!string.IsNullOrEmpty(dialogId))
		{
			completedDialogs.Add(dialogId);
		}
	}

	public DialogSaveData ExportSaveData()
	{
		DialogSaveData dialogSaveData = DialogSaveData.CreateDefault();
		dialogSaveData.TriggeredEvents.AddRange(triggeredEvents);
		dialogSaveData.CompletedDialogs.AddRange(completedDialogs);
		return dialogSaveData;
	}

	public void InitFromSaveData(DialogSaveData data)
	{
		DialogSaveData data2 = DataUtil.DeepClone(data);
		ApplySaveData(data2);
	}

	public void ApplySaveData(DialogSaveData data)
	{
		if (data == null)
		{
			data = DialogSaveData.CreateDefault();
		}
		triggeredEvents.Clear();
		completedDialogs.Clear();
		if (data.TriggeredEvents != null)
		{
			for (int i = 0; i < data.TriggeredEvents.Count; i++)
			{
				string text = data.TriggeredEvents[i];
				if (!string.IsNullOrEmpty(text))
				{
					triggeredEvents.Add(text);
				}
			}
		}
		if (data.CompletedDialogs == null)
		{
			return;
		}
		for (int j = 0; j < data.CompletedDialogs.Count; j++)
		{
			string text2 = data.CompletedDialogs[j];
			if (!string.IsNullOrEmpty(text2))
			{
				completedDialogs.Add(text2);
			}
		}
	}

	private void RegisterMainNpcDialogCandidates()
	{
		mainNpcDialogCandidates.Add(new DialogCandidateData
		{
			DialogId = "mijing_dialog",
			Priority = 200,
			IsNewContent = true,
			RemoveAfterComplete = true,
			Conditions = new List<DialogConditionData>
			{
				new DialogConditionData
				{
					Type = DialogConditionType.EventTriggered,
					Param = "mijing_unlocked"
				}
			}
		});
		mainNpcDialogCandidates.Add(new DialogCandidateData
		{
			DialogId = "tutorial_dialog",
			Priority = 100,
			IsNewContent = true,
			RemoveAfterComplete = true,
			Conditions = new List<DialogConditionData>
			{
				new DialogConditionData
				{
					Type = DialogConditionType.DialogNotCompleted,
					Param = "tutorial_dialog"
				}
			}
		});
		mainNpcDialogCandidates.Add(new DialogCandidateData
		{
			DialogId = "idle_dialog",
			Priority = 0,
			IsNewContent = false,
			RemoveAfterComplete = false,
			Conditions = new List<DialogConditionData>()
		});
	}

	private bool IsConditionMet(DialogConditionData condition)
	{
		if (condition == null)
		{
			return false;
		}
		return condition.Type switch
		{
			DialogConditionType.None => true, 
			DialogConditionType.EventTriggered => HasTriggered(condition.Param), 
			DialogConditionType.EventNotTriggered => !HasTriggered(condition.Param), 
			DialogConditionType.DialogCompleted => HasCompleted(condition.Param), 
			DialogConditionType.DialogNotCompleted => !HasCompleted(condition.Param), 
			_ => false, 
		};
	}

	private bool AreConditionsMet(List<DialogConditionData> conditions)
	{
		if (conditions == null || conditions.Count == 0)
		{
			return true;
		}
		for (int i = 0; i < conditions.Count; i++)
		{
			if (!IsConditionMet(conditions[i]))
			{
				return false;
			}
		}
		return true;
	}

	public string GetCurrentMainNpcDialogId()
	{
		DialogCandidateData dialogCandidateData = null;
		for (int i = 0; i < mainNpcDialogCandidates.Count; i++)
		{
			DialogCandidateData dialogCandidateData2 = mainNpcDialogCandidates[i];
			if (dialogCandidateData2 != null && !string.IsNullOrEmpty(dialogCandidateData2.DialogId) && (!dialogCandidateData2.RemoveAfterComplete || !HasCompleted(dialogCandidateData2.DialogId)) && AreConditionsMet(dialogCandidateData2.Conditions) && (dialogCandidateData == null || dialogCandidateData2.Priority > dialogCandidateData.Priority))
			{
				dialogCandidateData = dialogCandidateData2;
			}
		}
		return dialogCandidateData?.DialogId;
	}

	public bool HasNewMainNpcDialog()
	{
		for (int i = 0; i < mainNpcDialogCandidates.Count; i++)
		{
			DialogCandidateData dialogCandidateData = mainNpcDialogCandidates[i];
			if (dialogCandidateData != null && dialogCandidateData.IsNewContent && (!dialogCandidateData.RemoveAfterComplete || !HasCompleted(dialogCandidateData.DialogId)) && AreConditionsMet(dialogCandidateData.Conditions))
			{
				return true;
			}
		}
		return false;
	}

	private void RegisterTutorialDialog()
	{
		string text = "Adventurer";
		if (SaveManager.HasRuntime && SaveManager.RuntimeData.PlayerData != null)
		{
			text = SaveManager.RuntimeData.PlayerData.PlayerName;
		}
		string localKeyName = KeyDisplayUtil.GetLocalKeyName(ControlAction.Talent);
		string localKeyName2 = KeyDisplayUtil.GetLocalKeyName(ControlAction.Bag);
		string localKeyName3 = KeyDisplayUtil.GetLocalKeyName(ControlAction.Stats);
		DialogData dialogData = new DialogData();
		dialogData.DialogId = "tutorial_dialog";
		dialogData.Lines = new List<DialogLineData>
		{
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_0",
				FormatArgs = new object[1] { text }
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_1"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_2"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_3"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_4"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_5",
				FormatArgs = new object[1] { localKeyName }
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_6",
				FormatArgs = new object[1] { localKeyName2 }
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_7",
				FormatArgs = new object[1] { localKeyName3 }
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_8",
				EventKey = "tutorial_dialog_reward",
				TriggerOnce = true,
				OnLineFinished = GiveTutorialRewardStatic
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_9"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_10"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_11"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_12"
			},
			new DialogLineData
			{
				ContentKey = "tutorial_dialog_13"
			}
		};
		DialogData dialogData2 = dialogData;
		RegisterDialog(dialogData2);
	}

	private void RegisterMijingDialog()
	{
		DialogData dialogData = new DialogData();
		dialogData.DialogId = "mijing_dialog";
		dialogData.Lines = new List<DialogLineData>
		{
			new DialogLineData
			{
				ContentKey = "mijing_dialog_0"
			},
			new DialogLineData
			{
				ContentKey = "mijing_dialog_1"
			},
			new DialogLineData
			{
				ContentKey = "mijing_dialog_2"
			},
			new DialogLineData
			{
				ContentKey = "mijing_dialog_3"
			},
			new DialogLineData
			{
				ContentKey = "mijing_dialog_4"
			},
			new DialogLineData
			{
				ContentKey = "mijing_dialog_5"
			},
			new DialogLineData
			{
				ContentKey = "mijing_dialog_6",
				FormatArgs = new object[1] { SingletonMonoScope<MijingManager>.Instance.mijingSettings.intervalFloorNum }
			},
			new DialogLineData
			{
				ContentKey = "mijing_dialog_7"
			}
		};
		DialogData dialogData2 = dialogData;
		RegisterDialog(dialogData2);
	}

	private void RegisterIdleDialog()
	{
		DialogData dialogData = new DialogData
		{
			DialogId = "idle_dialog"
		};
		RegisterDialog(dialogData);
		mainNpcIdleLineKeys.Clear();
		mainNpcIdleLineKeys.Add("idle_dialog_0");
		mainNpcIdleLineKeys.Add("idle_dialog_1");
		mainNpcIdleLineKeys.Add("idle_dialog_2");
		mainNpcIdleLineKeys.Add("idle_dialog_3");
	}

	private static void GiveTutorialRewardStatic()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance && SingletonMonoScope<ItemManager>.HasInstance)
		{
			Transform transform = SingletonMonoScope<PlayerManager>.Instance.transform;
			SingletonMonoScope<ItemManager>.Instance.DropPotionById(transform, 0.5f, 3, 60000);
			SingletonMonoScope<ItemManager>.Instance.DropPotionById(transform, 0.5f, 3, 60009);
			SingletonMonoScope<ItemManager>.Instance.DropSpcPotion(transform.transform, 0.5f, int.MaxValue, "Forgetfulness Potion");
		}
	}
}
