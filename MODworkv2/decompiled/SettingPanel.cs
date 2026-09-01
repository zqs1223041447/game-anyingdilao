using System;
using System.Collections.Generic;
using Core;
using Data.AutoGen.DataClass.Settings;
using Display;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Inputs;
using Interact;
using Localization;
using UI.Map;
using UI.Panels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : GamepadSelectablePanel
{
	private class ControlGroup
	{
		public ControlAction[] actions;

		public bool addSpacerAfter;
	}

	private class AudioItem
	{
		public Slider slider;

		public Text valueText;

		public Action<float> setEditing;

		public Func<float> getEditing;
	}

	private GameObject LeftMenu;

	private GameObject Game;

	private GameObject Interface;

	private GameObject Controls;

	private GameObject Video;

	private GameObject Audio;

	private GameObject Filter;

	private GameObject Bottom;

	private GameSettingData gameSettingData;

	private InterfaceSettingData interfaceSettingData;

	private AudioSettingData audioSettingData;

	private ControlsSettingData controlsSettingData;

	private FilterData filterSettingData;

	private Button GameBtn;

	private Button InterfaceBtn;

	private Button ControlsBtn;

	private Button VideoBtn;

	private Button AudioBtn;

	private Button FilterBtn;

	private Toggle mouseMoveToggle;

	private Toggle forceMoveToggle;

	private Toggle autoLockGamepadToggle;

	private Toggle autoLockPcToggle;

	private Toggle autoAttackToggle;

	private Toggle autoSaveToggle;

	private Slider autoSaveTimeSlider;

	private Text saveTimeText;

	private const int AutoSaveTimeMinSeconds = 180;

	private const int AutoSaveTimeMaxSeconds = 1800;

	private Toggle leftxToggle;

	private Toggle leftyToggle;

	private Toggle rightxToggle;

	private Toggle rightyToggle;

	private Toggle autoChangeUseToggle;

	private Button pickModeLeftBtn;

	private Button pickModeRightBtn;

	private readonly Slider[] gamepadSkillDistanceSliders = new Slider[8];

	private readonly Text[] gamepadSkillDistanceValueTexts = new Text[8];

	private readonly Text[] gamepadSkillDistanceLabelTexts = new Text[8];

	private Slider textSlider;

	private Toggle textToggle;

	private Toggle mapUIToggle;

	private Button mapLeftBtn;

	private Button mapRightBtn;

	private Slider mapScaleSlider;

	private Slider mapViewSlider;

	private Slider mapGlobalAlphaSlider;

	private Slider mapBorderAlphaSlider;

	private Slider cursorSlider;

	private Toggle itemToggle;

	private Toggle aimToggle;

	private Slider cursorSpeedSlider;

	private Toggle itemTipToggle;

	private static readonly ControlGroup[] ControlGroups = new ControlGroup[6]
	{
		new ControlGroup
		{
			actions = new ControlAction[4]
			{
				ControlAction.Up,
				ControlAction.Down,
				ControlAction.Left,
				ControlAction.Right
			},
			addSpacerAfter = true
		},
		new ControlGroup
		{
			actions = new ControlAction[8]
			{
				ControlAction.Skill1,
				ControlAction.Skill2,
				ControlAction.Skill3,
				ControlAction.Skill4,
				ControlAction.Skill5,
				ControlAction.Skill6,
				ControlAction.Skill7,
				ControlAction.Skill8
			},
			addSpacerAfter = true
		},
		new ControlGroup
		{
			actions = new ControlAction[5]
			{
				ControlAction.Item1,
				ControlAction.Item2,
				ControlAction.TP,
				ControlAction.PickUp,
				ControlAction.QuickUse
			},
			addSpacerAfter = true
		},
		new ControlGroup
		{
			actions = new ControlAction[5]
			{
				ControlAction.Mercenary,
				ControlAction.Talent,
				ControlAction.Bag,
				ControlAction.Stats,
				ControlAction.MapMode
			},
			addSpacerAfter = true
		},
		new ControlGroup
		{
			actions = new ControlAction[6]
			{
				ControlAction.Sell,
				ControlAction.SellAll,
				ControlAction.PageL,
				ControlAction.PageR,
				ControlAction.SortAll,
				ControlAction.Sort
			},
			addSpacerAfter = true
		},
		new ControlGroup
		{
			actions = new ControlAction[1] { ControlAction.AutoAT },
			addSpacerAfter = false
		}
	};

	private KeyCode? pendingKey;

	private int? pendingMouse;

	private string pendingGamepadKey;

	private Transform controlsContentRoot;

	private const string controlBindItemResPath = "UI/Components/Settings/ControlActionBlock";

	private const string controlBindNullResPath = "UI/Components/Settings/ControlActionNull";

	private readonly List<ControlBindItem> controlItems = new List<ControlBindItem>();

	private ControlAction? waitingAction;

	private string press_any_key_text;

	private bool waitReleaseBeforeGamepadListening;

	private bool suppressEscapeUntilRelease;

	private readonly List<AudioItem> audioItems = new List<AudioItem>();

	private Text resolutionText;

	private Button resolutionLeftBtn;

	private Button resolutionRightBtn;

	private Text frameText;

	private Button frameLeftBtn;

	private Button frameRightBtn;

	private Text screenText;

	private Button screenLeftBtn;

	private Button screenRightBtn;

	private Slider lightSlider;

	private Toggle vsyncToggle;

	private Toggle bloomToggle;

	private static readonly int[] FramePresets = new int[4] { 0, 30, 60, 120 };

	public static int LanguageCount => Enum.GetValues(typeof(LanguageType)).Length;

	public static int PickModeCount => Enum.GetValues(typeof(PcPickupMode)).Length;

	public static int MapModeCount => Enum.GetValues(typeof(MapDisplayMode)).Length;

	private static int ResolutionCount => Enum.GetValues(typeof(ResolutionPreset)).Length;

	private static int FrameCount => FramePresets.Length;

	private static int ScreenCount => Enum.GetValues(typeof(ScreenMode)).Length;

	public static int AutoPick1 => Enum.GetValues(typeof(QulityType)).Length;

	public static int AutoPick2 => Enum.GetValues(typeof(QulityType)).Length;

	public static int AutoPick3 => Enum.GetValues(typeof(QulityType)).Length;

	private void HideAllGroups()
	{
		Game.SetActive(value: false);
		Interface.SetActive(value: false);
		Controls.SetActive(value: false);
		Video.SetActive(value: false);
		Audio.SetActive(value: false);
		Interface.SetActive(value: false);
		Filter.SetActive(value: false);
	}

	public override bool OnCancel()
	{
		Selectable currentSelectable = GamepadUINavigationManager.GetCurrentSelectable();
		if (!currentSelectable)
		{
			BackLastLevel();
			return true;
		}
		switch (SceneManager.GetActiveScene().buildIndex)
		{
		case 0:
			if (currentSelectable == GameBtn || currentSelectable == InterfaceBtn || currentSelectable == ControlsBtn || currentSelectable == VideoBtn || currentSelectable == AudioBtn || currentSelectable == FilterBtn || currentSelectable == GetControl<Button>("BottomResetBtn") || currentSelectable == GetControl<Button>("BottomConfirmBtn") || currentSelectable == GetControl<Button>("BottomApplyBtn") || currentSelectable == GetControl<Button>("BackBtn"))
			{
				Singleton<UIManager>.Instance.ShowExclusivePanel<MainPanel>();
				return true;
			}
			break;
		case 1:
			if (currentSelectable == GameBtn || currentSelectable == InterfaceBtn || currentSelectable == ControlsBtn || currentSelectable == VideoBtn || currentSelectable == AudioBtn || currentSelectable == FilterBtn || currentSelectable == GetControl<Button>("BottomResetBtn") || currentSelectable == GetControl<Button>("BottomConfirmBtn") || currentSelectable == GetControl<Button>("BottomApplyBtn") || currentSelectable == GetControl<Button>("BackBtn"))
			{
				Singleton<UIManager>.Instance.ShowExclusivePanel<PausePanel>();
				Time.timeScale = 0f;
				return true;
			}
			break;
		case 2:
			if (currentSelectable == GameBtn || currentSelectable == InterfaceBtn || currentSelectable == ControlsBtn || currentSelectable == VideoBtn || currentSelectable == AudioBtn || currentSelectable == FilterBtn || currentSelectable == GetControl<Button>("BottomResetBtn") || currentSelectable == GetControl<Button>("BottomConfirmBtn") || currentSelectable == GetControl<Button>("BottomApplyBtn") || currentSelectable == GetControl<Button>("BackBtn"))
			{
				Singleton<UIManager>.Instance.ShowExclusivePanel<PausePanel>();
				Time.timeScale = 0f;
				return true;
			}
			break;
		}
		if (Game.activeSelf)
		{
			SetFirstSelected(InterfaceBtn);
			return true;
		}
		if (Interface.activeSelf)
		{
			SetFirstSelected(ControlsBtn);
			return true;
		}
		if (Controls.activeSelf)
		{
			SetFirstSelected(VideoBtn);
			return true;
		}
		if (Video.activeSelf)
		{
			SetFirstSelected(AudioBtn);
			return true;
		}
		if (Audio.activeSelf)
		{
			SetFirstSelected(VideoBtn);
			return true;
		}
		if (Filter.activeSelf)
		{
			SetFirstSelected(VideoBtn);
			return true;
		}
		return false;
	}

	private static void BackLastLevel()
	{
		switch (SceneManager.GetActiveScene().buildIndex)
		{
		case 0:
			Singleton<UIManager>.Instance.ShowExclusivePanel<MainPanel>();
			break;
		case 1:
			Singleton<UIManager>.Instance.ShowExclusivePanel<PausePanel>();
			Time.timeScale = 0f;
			break;
		case 2:
			Singleton<UIManager>.Instance.ShowExclusivePanel<PausePanel>();
			Time.timeScale = 0f;
			break;
		}
	}

	private void ResetToDefault(bool all)
	{
		if (!all)
		{
			if (Game.activeSelf)
			{
				Singleton<SettingDataManager>.Instance.ResetGame();
			}
			if (Interface.activeSelf)
			{
				Singleton<SettingDataManager>.Instance.ResetInterface();
			}
			if (Audio.activeSelf)
			{
				Singleton<SettingDataManager>.Instance.ResetAudioImmediate();
			}
			if (Controls.activeSelf)
			{
				Singleton<SettingDataManager>.Instance.ResetControls();
			}
			if (Video.activeSelf)
			{
				Singleton<SettingDataManager>.Instance.ResetVideo();
			}
			if (Filter.activeSelf)
			{
				Singleton<SettingDataManager>.Instance.ResetFilter();
			}
		}
		else
		{
			Singleton<SettingDataManager>.Instance.ResetAllToDefault();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		LeftMenu = base.transform.Find("SettingBg/LeftMenu").gameObject;
		Bottom = base.transform.Find("SettingBg/Bottom").gameObject;
		Game = base.transform.Find("SettingBg/Game").gameObject;
		Interface = base.transform.Find("SettingBg/Interface").gameObject;
		Controls = base.transform.Find("SettingBg/Controls").gameObject;
		Video = base.transform.Find("SettingBg/Video").gameObject;
		Audio = base.transform.Find("SettingBg/Audio").gameObject;
		Filter = base.transform.Find("SettingBg/Filter").gameObject;
		GameBtn = GetControl<Button>("GameBtn");
		ControlsBtn = GetControl<Button>("ControlsBtn");
		VideoBtn = GetControl<Button>("VideoBtn");
		AudioBtn = GetControl<Button>("AudioBtn");
		InterfaceBtn = GetControl<Button>("InterfaceBtn");
		FilterBtn = GetControl<Button>("FilterBtn");
		SettingDataManager mgr = Singleton<SettingDataManager>.Instance;
		GameBtn.onClick.AddListener(delegate
		{
			if (!mgr.IsEditing)
			{
				mgr.BeginEdit();
			}
			HideAllGroups();
			Game.SetActive(value: true);
			RefreshUI();
			RefreshTotalBtn();
			SetFirstSelected(GetControl<Button>("lanLeftBtn"));
		});
		ControlsBtn.onClick.AddListener(delegate
		{
			if (!mgr.IsEditing)
			{
				mgr.BeginEdit();
			}
			HideAllGroups();
			Controls.SetActive(value: true);
			BuildControlsUI();
			RefreshUI();
			RefreshTotalBtn();
			SetFirstSelected(GetFirstControlBindButton());
		});
		VideoBtn.onClick.AddListener(delegate
		{
			if (!mgr.IsEditing)
			{
				mgr.BeginEdit();
			}
			HideAllGroups();
			Video.SetActive(value: true);
			RefreshUI();
			RefreshTotalBtn();
			SetFirstSelected(GetControl<Button>("res_left_btn"));
		});
		AudioBtn.onClick.AddListener(delegate
		{
			if (!mgr.IsEditing)
			{
				mgr.BeginEdit();
			}
			HideAllGroups();
			Audio.SetActive(value: true);
			RefreshUI();
			RefreshTotalBtn();
			SetFirstSelected(GetControl<Slider>("setting_master_volume_slider"));
		});
		InterfaceBtn.onClick.AddListener(delegate
		{
			if (!mgr.IsEditing)
			{
				mgr.BeginEdit();
			}
			HideAllGroups();
			Interface.SetActive(value: true);
			RefreshUI();
			RefreshTotalBtn();
			SetFirstSelected(GetControl<Slider>("cursorSlider"));
		});
		FilterBtn.onClick.AddListener(delegate
		{
			if (!mgr.IsEditing)
			{
				mgr.BeginEdit();
			}
			HideAllGroups();
			Filter.SetActive(value: true);
			RefreshUI();
			RefreshTotalBtn();
			SetFirstSelected(GetControl<Button>("PLPickLeftBtn"));
		});
		HideAllGroups();
		LeftMenu.SetActive(value: true);
		Game.SetActive(value: true);
		Bottom.SetActive(value: true);
		InitGameSettings();
		InitInterfaceSettings();
		InitControlSettings();
		InitAudioSettings();
		InitVideoSettings();
		InitFilterSettings();
		RefreshTotalBtn();
		RefreshUI();
	}

	private void RefreshTotalBtn()
	{
		if (Game.activeSelf)
		{
			GameBtn.interactable = false;
			ControlsBtn.interactable = true;
			AudioBtn.interactable = true;
			VideoBtn.interactable = true;
			InterfaceBtn.interactable = true;
			FilterBtn.interactable = true;
		}
		if (Interface.activeSelf)
		{
			GameBtn.interactable = true;
			ControlsBtn.interactable = true;
			AudioBtn.interactable = true;
			VideoBtn.interactable = true;
			InterfaceBtn.interactable = false;
			FilterBtn.interactable = true;
		}
		if (Controls.activeSelf)
		{
			GameBtn.interactable = true;
			ControlsBtn.interactable = false;
			AudioBtn.interactable = true;
			VideoBtn.interactable = true;
			InterfaceBtn.interactable = true;
			FilterBtn.interactable = true;
		}
		if (Audio.activeSelf)
		{
			GameBtn.interactable = true;
			ControlsBtn.interactable = true;
			AudioBtn.interactable = false;
			VideoBtn.interactable = true;
			InterfaceBtn.interactable = true;
			FilterBtn.interactable = true;
		}
		if (Video.activeSelf)
		{
			GameBtn.interactable = true;
			ControlsBtn.interactable = true;
			AudioBtn.interactable = true;
			VideoBtn.interactable = false;
			InterfaceBtn.interactable = true;
			FilterBtn.interactable = true;
		}
		if (Filter.activeSelf)
		{
			GameBtn.interactable = true;
			ControlsBtn.interactable = true;
			AudioBtn.interactable = true;
			VideoBtn.interactable = true;
			InterfaceBtn.interactable = true;
			FilterBtn.interactable = false;
		}
	}

	public override void HideMe()
	{
		Singleton<SettingDataManager>.Instance.CancelEdit();
		RefreshUI();
	}

	public override void OnShow()
	{
		Singleton<SettingDataManager>.Instance.BeginEdit();
		HideAllGroups();
		LeftMenu.SetActive(value: true);
		Game.SetActive(value: true);
		Bottom.SetActive(value: true);
		RefreshTotalBtn();
		RefreshUI();
		SetFirstSelected(GetControl<Button>("lanLeftBtn"));
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = false;
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = true;
		}
	}

	public override void ShowMe()
	{
		base.ShowMe();
		RefreshUI();
	}

	public void RefreshUI()
	{
		if (Game.activeSelf)
		{
			RefreshGameUI();
		}
		if (Interface.activeSelf)
		{
			RefreshInterfaceUI();
		}
		if (Controls.activeSelf)
		{
			RefreshControlsUI();
		}
		if (Audio.activeSelf)
		{
			RefreshAudioUI();
		}
		if (Video.activeSelf)
		{
			RefreshVideoUI();
		}
		if (Filter.activeSelf)
		{
			RefreshFilterUI();
		}
		RefreshBottomButtons();
	}

	private void RefreshBottomButtons()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		GetControl<Button>("BottomApplyBtn").interactable = instance.HasPendingChanges;
	}

	protected override void ClickBtn(string btnName)
	{
		switch (btnName)
		{
		case "BackBtn":
			Singleton<SettingDataManager>.Instance.CancelEdit();
			RefreshUI();
			BackLastLevel();
			break;
		case "BottomConfirmBtn":
			Singleton<SettingDataManager>.Instance.Confirm();
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.KeyBindUI.RefreshUI();
			}
			RefreshUI();
			BackLastLevel();
			break;
		case "BottomApplyBtn":
			Singleton<SettingDataManager>.Instance.Apply();
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.KeyBindUI.RefreshUI();
			}
			RefreshUI();
			break;
		case "BottomResetBtn":
			ResetToDefault(all: false);
			RefreshUI();
			break;
		}
	}

	private void InitGameSettings()
	{
		GetControl<Button>("lanLeftBtn").onClick.AddListener(OnClickLanguageLeft);
		GetControl<Button>("lanRightBtn").onClick.AddListener(OnClickLanguageRight);
		pickModeLeftBtn = GetControl<Button>("pickLeftBtn");
		pickModeLeftBtn.onClick.AddListener(OnClickPickModeLeft);
		pickModeRightBtn = GetControl<Button>("pickRightBtn");
		pickModeRightBtn.onClick.AddListener(OnClickPickModeRight);
		mouseMoveToggle = GetControl<Toggle>("mouseMoveToggle");
		mouseMoveToggle.onValueChanged.AddListener(OnToggleMouseMove);
		forceMoveToggle = GetControl<Toggle>("QZMoveToggle");
		forceMoveToggle.onValueChanged.AddListener(OnToggleForceMove);
		autoLockGamepadToggle = GetControl<Toggle>("autoLockGamepadToggle");
		autoLockGamepadToggle.onValueChanged.AddListener(OnToggleAutoLockGamepad);
		autoLockPcToggle = GetControl<Toggle>("autoLockPcToggle");
		autoLockPcToggle.onValueChanged.AddListener(OnToggleAutoLockPc);
		autoAttackToggle = GetControl<Toggle>("autoAttackToggle");
		autoAttackToggle.onValueChanged.AddListener(OnToggleAutoAttack);
		autoSaveToggle = GetControl<Toggle>("autoSaveToggle");
		autoSaveToggle.onValueChanged.AddListener(OnToggleAutoSave);
		autoSaveTimeSlider = GetControl<Slider>("autoSaveTimeSlider");
		autoSaveTimeSlider.minValue = 180f;
		autoSaveTimeSlider.maxValue = 1800f;
		autoSaveTimeSlider.wholeNumbers = true;
		autoSaveTimeSlider.onValueChanged.AddListener(OnSliderAutoSaveTime);
		saveTimeText = FindAutoSaveTimeText();
		RefreshAutoSaveTimeText(Singleton<SettingDataManager>.Instance.GetGame().auto_save_time);
		leftxToggle = GetControl<Toggle>("leftxToggle");
		leftxToggle.onValueChanged.AddListener(OnToggleLeftX);
		leftyToggle = GetControl<Toggle>("leftyToggle");
		leftyToggle.onValueChanged.AddListener(OnToggleLeftY);
		rightxToggle = GetControl<Toggle>("rightxToggle");
		rightxToggle.onValueChanged.AddListener(OnToggleRightX);
		rightyToggle = GetControl<Toggle>("rightyToggle");
		rightyToggle.onValueChanged.AddListener(OnToggleRightY);
		autoChangeUseToggle = GetControl<Toggle>("autoChangeUseToggle");
		autoChangeUseToggle.onValueChanged.AddListener(OnToggleAutoChangeUse);
		InitGamepadSkillDistanceControls();
	}

	public void OnClickLanguageLeft()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		gameSettingData = instance.GetGame();
		int language = gameSettingData.language;
		language = (language - 1 + LanguageCount) % LanguageCount;
		instance.SetLanguageImmediate((LanguageType)language);
		RefreshUI();
	}

	public void OnClickLanguageRight()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		gameSettingData = instance.GetGame();
		int language = gameSettingData.language;
		language = (language + 1) % LanguageCount;
		instance.SetLanguageImmediate((LanguageType)language);
		RefreshUI();
	}

	public void OnClickPickModeLeft()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		gameSettingData = instance.GetEditingGame();
		int pcPickupMode = (int)gameSettingData.pcPickupMode;
		pcPickupMode = (pcPickupMode - 1 + PickModeCount) % PickModeCount;
		instance.SetPickModeEditing((PcPickupMode)pcPickupMode);
		RefreshUI();
	}

	public void OnClickPickModeRight()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		gameSettingData = instance.GetEditingGame();
		int pcPickupMode = (int)gameSettingData.pcPickupMode;
		pcPickupMode = (pcPickupMode + 1) % PickModeCount;
		instance.SetPickModeEditing((PcPickupMode)pcPickupMode);
		RefreshUI();
	}

	public void OnToggleMouseMove(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetMouseMoveToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleForceMove(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetForceMoveToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleAutoLock(bool isOn)
	{
		OnToggleAutoLockGamepad(isOn);
	}

	public void OnToggleAutoLockGamepad(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetAutoLockGamepadToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleAutoLockPc(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetAutoLockPcToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleAutoAttack(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetAutoAttackToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleAutoSave(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetAutoSaveToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnSliderAutoSaveTime(float v)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingGame() != null)
		{
			int num = Mathf.Clamp(Mathf.RoundToInt(v), 180, 1800);
			instance.SetAutoSaveTimeEditing(num);
			RefreshAutoSaveTimeText(num);
			RefreshGameUI();
			RefreshBottomButtons();
		}
	}

	public void OnToggleLeftX(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetLeftXToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleLeftY(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetLeftYToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleRightX(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetRightXToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleRightY(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetRightYToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleAutoChangeUse(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetAutoChangeUseToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	private void InitGamepadSkillDistanceControls()
	{
		for (int i = 0; i < gamepadSkillDistanceSliders.Length; i++)
		{
			int num = i + 1;
			Transform transform = FindChildRecursive(Game.transform, "Distance Skill" + num);
			if (!transform)
			{
				continue;
			}
			Slider componentInChildren = transform.GetComponentInChildren<Slider>(includeInactive: true);
			if ((bool)componentInChildren)
			{
				int captured = i;
				gamepadSkillDistanceSliders[i] = componentInChildren;
				componentInChildren.minValue = 10f;
				componentInChildren.maxValue = 100f;
				componentInChildren.wholeNumbers = true;
				componentInChildren.onValueChanged.AddListener(delegate(float v)
				{
					OnSliderGamepadSkillDistance(captured, v);
				});
			}
			Text[] componentsInChildren = transform.GetComponentsInChildren<Text>(includeInactive: true);
			foreach (Text text in componentsInChildren)
			{
				if (!text)
				{
					continue;
				}
				if (text.name == "saveTimeText")
				{
					gamepadSkillDistanceValueTexts[i] = text;
				}
				else if (!gamepadSkillDistanceLabelTexts[i] && text.name.StartsWith("Text", StringComparison.Ordinal))
				{
					gamepadSkillDistanceLabelTexts[i] = text;
					LocalizedText component = text.GetComponent<LocalizedText>();
					if ((bool)component)
					{
						component.Set(LocalizationExcelList.Start_FY, "setting_gamepad_skill" + num + "_distance");
					}
				}
			}
		}
	}

	private void OnSliderGamepadSkillDistance(int index, float value)
	{
		int percent = Mathf.Clamp(Mathf.RoundToInt(value), 10, 100);
		Singleton<SettingDataManager>.Instance.SetGamepadSkillDistanceEditing(index + 1, percent);
		RefreshGamepadSkillDistanceUI();
		RefreshBottomButtons();
	}

	private void RefreshGamepadSkillDistanceUI()
	{
		GameSettingData editingGame = Singleton<SettingDataManager>.Instance.GetEditingGame();
		for (int i = 0; i < gamepadSkillDistanceSliders.Length; i++)
		{
			int skillIndex = i + 1;
			int gamepadSkillDistancePercent = SettingDataManager.GetGamepadSkillDistancePercent(editingGame, skillIndex);
			if ((bool)gamepadSkillDistanceSliders[i])
			{
				gamepadSkillDistanceSliders[i].SetValueWithoutNotify(gamepadSkillDistancePercent);
			}
			if ((bool)gamepadSkillDistanceValueTexts[i])
			{
				gamepadSkillDistanceValueTexts[i].text = gamepadSkillDistancePercent + "%";
			}
			if ((bool)gamepadSkillDistanceLabelTexts[i])
			{
				LocalizedText component = gamepadSkillDistanceLabelTexts[i].GetComponent<LocalizedText>();
				if ((bool)component)
				{
					component.Set(LocalizationExcelList.Start_FY, "setting_gamepad_skill" + skillIndex + "_distance");
				}
				gamepadSkillDistanceLabelTexts[i].text = GetGamepadSkillDistanceLabel(skillIndex);
			}
		}
	}

	private string GetGamepadSkillDistanceLabel(int skillIndex)
	{
		string text = "setting_gamepad_skill" + skillIndex + "_distance";
		string start = LOC.MM.GetStart(text);
		if (string.IsNullOrEmpty(start) || start == text)
		{
			return "Gamepad Skill " + skillIndex + " Cast Distance";
		}
		return start;
	}

	private Text FindAutoSaveTimeText()
	{
		if ((bool)autoSaveTimeSlider && (bool)autoSaveTimeSlider.transform.parent)
		{
			Transform transform = autoSaveTimeSlider.transform.parent.Find("saveTimeText");
			if ((bool)transform)
			{
				Text component = transform.GetComponent<Text>();
				if ((bool)component)
				{
					return component;
				}
			}
			Text[] componentsInChildren = autoSaveTimeSlider.transform.parent.GetComponentsInChildren<Text>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if ((bool)componentsInChildren[i] && componentsInChildren[i].name == "saveTimeText")
				{
					return componentsInChildren[i];
				}
			}
		}
		return GetControl<Text>("saveTimeText");
	}

	private void RefreshAutoSaveTimeText(int seconds)
	{
		if (!saveTimeText)
		{
			saveTimeText = FindAutoSaveTimeText();
		}
		if ((bool)saveTimeText)
		{
			saveTimeText.text = FormatAutoSaveTime(seconds);
		}
	}

	private static string FormatAutoSaveTime(int seconds)
	{
		string text = LOC.MM.GetMain("S");
		if (string.IsNullOrEmpty(text) || text == "S")
		{
			text = "s";
		}
		return seconds + text;
	}

	private Transform FindChildRecursive(Transform root, string childName)
	{
		if (!root)
		{
			return null;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			Transform child = root.GetChild(i);
			if (child.name == childName)
			{
				return child;
			}
			Transform transform = FindChildRecursive(child, childName);
			if ((bool)transform)
			{
				return transform;
			}
		}
		return null;
	}

	private void RefreshGameUI()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		GameSettingData game = instance.GetGame();
		GameSettingData editingGame = instance.GetEditingGame();
		GetControl<Text>("LanguageText").text = LOC.GetLanguageDisplayName((LanguageType)game.language);
		int num = Mathf.Clamp(editingGame.auto_save_time, 180, 1800);
		RefreshAutoSaveTimeText(num);
		GetControl<Text>("PickUpModeText").text = PCPickupTargetManager.GetLocalMode(editingGame.pcPickupMode);
		autoSaveTimeSlider.SetValueWithoutNotify(num);
		mouseMoveToggle.SetIsOnWithoutNotify(editingGame.mouse_move);
		forceMoveToggle.SetIsOnWithoutNotify(editingGame.QZ_Move);
		if ((bool)autoLockGamepadToggle)
		{
			autoLockGamepadToggle.SetIsOnWithoutNotify(editingGame.auto_lock1);
		}
		if ((bool)autoLockPcToggle)
		{
			autoLockPcToggle.SetIsOnWithoutNotify(editingGame.auto_lock2);
		}
		autoAttackToggle.SetIsOnWithoutNotify(editingGame.auto_attack);
		autoSaveToggle.SetIsOnWithoutNotify(editingGame.auto_save);
		leftxToggle.SetIsOnWithoutNotify(editingGame.left_invert_x);
		leftyToggle.SetIsOnWithoutNotify(editingGame.left_invert_y);
		rightxToggle.SetIsOnWithoutNotify(editingGame.right_invert_x);
		rightyToggle.SetIsOnWithoutNotify(editingGame.right_invert_y);
		autoChangeUseToggle.SetIsOnWithoutNotify(editingGame.autoChangeUseToggle);
		RefreshGamepadSkillDistanceUI();
	}

	private void InitInterfaceSettings()
	{
		textSlider = GetControl<Slider>("damageScaleSlider");
		textSlider.onValueChanged.AddListener(OnSliderDamageScale);
		textToggle = GetControl<Toggle>("damageToggle");
		textToggle.onValueChanged.AddListener(OnToggleDamageNum);
		mapUIToggle = GetControl<Toggle>("mapToggle");
		mapUIToggle.onValueChanged.AddListener(OnToggleMap);
		mapScaleSlider = GetControl<Slider>("mapScaleSlider");
		mapScaleSlider.onValueChanged.AddListener(OnSliderMapScale);
		mapViewSlider = GetControl<Slider>("mapViewSlider");
		mapViewSlider.onValueChanged.AddListener(OnSliderMapView);
		cursorSlider = GetControl<Slider>("cursorSlider");
		cursorSlider.minValue = 50f;
		cursorSlider.maxValue = 200f;
		cursorSlider.wholeNumbers = true;
		cursorSlider.onValueChanged.AddListener(OnCursorChanged);
		itemToggle = GetControl<Toggle>("itemToggle");
		itemToggle.onValueChanged.AddListener(OnToggleItem);
		aimToggle = GetControl<Toggle>("aimToggle");
		aimToggle.onValueChanged.AddListener(OnToggleAim);
		cursorSpeedSlider = GetControl<Slider>("cursorSpeedSlider");
		cursorSpeedSlider.onValueChanged.AddListener(OnCursorSpeedChanged);
		mapGlobalAlphaSlider = GetControl<Slider>("mapGlobalAlphaSlider");
		mapGlobalAlphaSlider.onValueChanged.AddListener(OnMapGlobalAlphaChanged);
		mapBorderAlphaSlider = GetControl<Slider>("mapBorderAlphaSlider");
		mapBorderAlphaSlider.onValueChanged.AddListener(OnMapBorderAlphaChanged);
		mapLeftBtn = GetControl<Button>("mapLeftBtn");
		mapLeftBtn.onClick.AddListener(OnClickMapLeft);
		mapRightBtn = GetControl<Button>("mapRightBtn");
		mapRightBtn.onClick.AddListener(OnClickMapRight);
		itemTipToggle = GetControl<Toggle>("itemTipToggle");
		itemTipToggle.onValueChanged.AddListener(OnItemTip);
	}

	public void OnToggleDamageNum(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetDamageToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnClickMapLeft()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		interfaceSettingData = instance.GetEditingInterface();
		int map_mode = (int)interfaceSettingData.map_mode;
		map_mode = (map_mode - 1 + MapModeCount) % MapModeCount;
		instance.SetMapModeEditing((MapDisplayMode)map_mode);
		RefreshUI();
	}

	public void OnClickMapRight()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		interfaceSettingData = instance.GetEditingInterface();
		int map_mode = (int)interfaceSettingData.map_mode;
		map_mode = (map_mode + 1) % MapModeCount;
		instance.SetMapModeEditing((MapDisplayMode)map_mode);
		RefreshUI();
	}

	public void OnToggleMap(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetMapToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnToggleItem(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetItemToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnSliderDamageScale(float v)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingGame() != null)
		{
			instance.SetDamageScaleEditing(v);
			RefreshGameUI();
			RefreshBottomButtons();
		}
	}

	public void OnCursorSpeedChanged(float v)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingGame() != null)
		{
			instance.SetCursorSpeedScaleEditing(v);
			RefreshGameUI();
			RefreshBottomButtons();
		}
	}

	public void OnSliderMapScale(float v)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingGame() != null)
		{
			instance.SetMapScaleEditing(v);
			RefreshGameUI();
			RefreshBottomButtons();
		}
	}

	public void OnSliderMapView(float v)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingGame() != null)
		{
			instance.SetMapViewRangeEditing(v);
			RefreshGameUI();
			RefreshBottomButtons();
		}
	}

	public void OnMapGlobalAlphaChanged(float v)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingGame() != null)
		{
			instance.SetMapGlobalAlphaEditing(v);
			RefreshGameUI();
			RefreshBottomButtons();
		}
	}

	public void OnMapBorderAlphaChanged(float v)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingGame() != null)
		{
			instance.SetMapBorderAlphaEditing(v);
			RefreshGameUI();
			RefreshBottomButtons();
		}
	}

	private void OnCursorChanged(float value)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingInterface() != null)
		{
			instance.SetCursorEditing(value);
			RefreshBottomButtons();
		}
	}

	public void OnToggleAim(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetAimToggleEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	public void OnItemTip(bool isOn)
	{
		Singleton<SettingDataManager>.Instance.SetItemTipEditing(isOn);
		RefreshGameUI();
		RefreshBottomButtons();
	}

	private void RefreshInterfaceUI()
	{
		InterfaceSettingData editingInterface = Singleton<SettingDataManager>.Instance.GetEditingInterface();
		GetControl<Text>("MapModeText").text = MapManager.GetLocalMapMode(editingInterface.map_mode);
		textToggle.SetIsOnWithoutNotify(editingInterface.damage_text);
		textSlider.SetValueWithoutNotify(editingInterface.damage_scale);
		mapUIToggle.SetIsOnWithoutNotify(editingInterface.map_toggle);
		mapScaleSlider.SetValueWithoutNotify(editingInterface.map_scale);
		mapViewSlider.SetValueWithoutNotify(editingInterface.map_view_range);
		cursorSlider.SetValueWithoutNotify(editingInterface.cursor);
		itemToggle.SetIsOnWithoutNotify(editingInterface.display_item);
		aimToggle.SetIsOnWithoutNotify(editingInterface.aim_point);
		cursorSpeedSlider.SetValueWithoutNotify(editingInterface.cursor_speed);
		mapGlobalAlphaSlider.SetValueWithoutNotify(editingInterface.map_global_alpha);
		mapBorderAlphaSlider.SetValueWithoutNotify(editingInterface.map_border_alpha);
		itemTipToggle.SetIsOnWithoutNotify(editingInterface.item_tip);
	}

	private Button GetFirstControlBindButton()
	{
		foreach (ControlBindItem controlItem in controlItems)
		{
			if ((bool)controlItem && (bool)controlItem.bindButton && controlItem.bindButton.interactable)
			{
				return controlItem.bindButton;
			}
		}
		return null;
	}

	private Button GetBindButtonByAction(ControlAction action)
	{
		foreach (ControlBindItem controlItem in controlItems)
		{
			if ((bool)controlItem && controlItem.Action == action && (bool)controlItem.bindButton && controlItem.bindButton.interactable)
			{
				return controlItem.bindButton;
			}
		}
		return null;
	}

	private static ControlsSettingData GetCurrentEditingControlsData()
	{
		return Singleton<SettingDataManager>.Instance.GetCurrentEditingControl();
	}

	private void BuildControlsUI()
	{
		for (int num = controlsContentRoot.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(controlsContentRoot.GetChild(num).gameObject);
		}
		controlItems.Clear();
		ControlBindItem controlBindItem = Singleton<ResManager>.Instance.Load<ControlBindItem>("res://UI/Components/Settings/ControlActionBlock");
		if (!controlBindItem)
		{
			LogUtil.Error("未发现ControlBindItem prefab文件： UI/Components/Settings/ControlActionBlock");
			return;
		}
		press_any_key_text = LOC.MM.GetStart("press_any_key");
		ControlsSettingData currentEditingControlsData = GetCurrentEditingControlsData();
		if (currentEditingControlsData == null)
		{
			LogUtil.Error("当前设备编辑态 Controls 数据不存在！");
			return;
		}
		ControlGroup[] controlGroups = ControlGroups;
		foreach (ControlGroup controlGroup in controlGroups)
		{
			ControlAction[] actions = controlGroup.actions;
			for (int j = 0; j < actions.Length; j++)
			{
				ControlAction controlAction = actions[j];
				string bind = currentEditingControlsData.GetBind(controlAction);
				ControlBindItem controlBindItem2 = UnityEngine.Object.Instantiate(controlBindItem, controlsContentRoot);
				controlItems.Add(controlBindItem2);
				string start = LOC.MM.GetStart("control_" + controlAction.ToString().ToLower());
				bool interactable = CanRebindAction(SingletonMonoGlobal<CurrentInputManager>.HasInstance ? SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType : InputDeviceType.PC, controlAction);
				controlBindItem2.Bind(controlAction, start, bind ?? string.Empty, OnClickRebind);
				controlBindItem2.SetInteractable(interactable);
			}
			if (controlGroup.addSpacerAfter)
			{
				CreateSpacerBlock();
			}
		}
	}

	private void CreateSpacerBlock()
	{
		UnityEngine.Object.Instantiate(Singleton<ResManager>.Instance.Load<GameObject>("res://UI/Components/Settings/ControlActionNull"), controlsContentRoot);
	}

	private void InitControlSettings()
	{
		controlsContentRoot = GetControl<VerticalLayoutGroup>("ControlsContent").transform;
	}

	private void SetControlButtonsInteractable(bool enable)
	{
		GetControl<Button>("BackBtn").interactable = enable;
		GetControl<Button>("BottomConfirmBtn").interactable = enable;
		GetControl<Button>("BottomApplyBtn").interactable = enable;
		GetControl<Button>("BottomResetBtn").interactable = enable;
		foreach (ControlBindItem controlItem in controlItems)
		{
			if ((bool)controlItem)
			{
				controlItem.SetInteractable(enable);
			}
		}
	}

	public bool IsKeyConflict(ControlAction self, string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		ControlsSettingData currentEditingControlsData = GetCurrentEditingControlsData();
		if (currentEditingControlsData == null)
		{
			return false;
		}
		foreach (ControlAction value in Enum.GetValues(typeof(ControlAction)))
		{
			if (value != self)
			{
				string bind = currentEditingControlsData.GetBind(value);
				if (!string.IsNullOrEmpty(bind) && bind == key && !ShouldIgnoreKeyConflict(self, value))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool ShouldIgnoreKeyConflict(ControlAction self, ControlAction other)
	{
		if (IsContextualContainerAction(self) || IsContextualContainerAction(other))
		{
			return true;
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return false;
		}
		if (IsSkillAction(self) && IsSkillAction(other))
		{
			return true;
		}
		if (!IsContextualContainerAction(self))
		{
			return IsContextualContainerAction(other);
		}
		return true;
	}

	private static bool IsSkillAction(ControlAction action)
	{
		if ((uint)(action - 4) <= 6u || action == ControlAction.Skill8)
		{
			return true;
		}
		return false;
	}

	private static bool IsContextualContainerAction(ControlAction action)
	{
		if ((uint)(action - 23) <= 5u)
		{
			return true;
		}
		return false;
	}

	public void OnClickRebind(ControlAction action)
	{
		if (!CanRebindAction(SingletonMonoGlobal<CurrentInputManager>.HasInstance ? SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType : InputDeviceType.PC, action))
		{
			return;
		}
		waitingAction = action;
		pendingGamepadKey = null;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			waitReleaseBeforeGamepadListening = true;
			GamepadUINavigationManager.BlockGamepadUIInput = true;
		}
		SetControlBtnsInteractable(enable: false, action);
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			Button bindButtonByAction = GetBindButtonByAction(action);
			if ((bool)bindButtonByAction)
			{
				SetFirstSelected(bindButtonByAction);
			}
		}
		foreach (ControlBindItem controlItem in controlItems)
		{
			if ((bool)controlItem && controlItem.Action == action)
			{
				controlItem.SetWaiting(press_any_key_text);
				controlItem.SetConflict(conflict: false);
				break;
			}
		}
	}

	private void Update()
	{
		if (suppressEscapeUntilRelease)
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				suppressEscapeUntilRelease = false;
			}
			return;
		}
		if (waitingAction.HasValue)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GameManager.ShowTipLocalStartKey("cannot_bind", TipType.Fail);
				CancelRebind();
				suppressEscapeUntilRelease = true;
				return;
			}
		}
		else if (Input.GetKeyUp(KeyCode.Escape))
		{
			BackLastLevel();
			return;
		}
		if (!Controls.activeSelf || !waitingAction.HasValue)
		{
			return;
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			if (!pendingKey.HasValue)
			{
				foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
				{
					if (value != 0 && Input.GetKeyDown(value))
					{
						pendingKey = value;
						return;
					}
				}
			}
			else if (Input.GetKeyUp(pendingKey.Value))
			{
				ApplyRebind(pendingKey.Value.ToString());
				pendingKey = null;
				return;
			}
			if (!pendingMouse.HasValue)
			{
				for (int i = 0; i <= 2; i++)
				{
					if (Input.GetMouseButtonDown(i))
					{
						pendingMouse = i;
						break;
					}
				}
			}
			else if (Input.GetMouseButtonUp(pendingMouse.Value))
			{
				ApplyRebind("Mouse" + pendingMouse.Value);
				pendingMouse = null;
			}
			return;
		}
		if (waitReleaseBeforeGamepadListening)
		{
			if (GamepadInputManager.IsAnyPressed())
			{
				return;
			}
			waitReleaseBeforeGamepadListening = false;
		}
		if (string.IsNullOrEmpty(pendingGamepadKey))
		{
			if (GamepadInputManager.TryGetPressedKeyForRebind(out var rawKey))
			{
				bool num = !GamepadKeys.CanBind(rawKey);
				bool flag = (waitingAction.GetValueOrDefault() == ControlAction.Bag || waitingAction.GetValueOrDefault() == ControlAction.Talent) && (rawKey == "Pad_LStickPress" || rawKey == "Pad_RStickPress");
				if (num || flag)
				{
					GameManager.ShowTipLocalStartKey("cannot_bind", TipType.Fail);
					CancelRebind();
				}
				else
				{
					pendingGamepadKey = rawKey;
				}
			}
		}
		else if (GamepadInputManager.GetKeyUp(pendingGamepadKey))
		{
			ApplyRebind(pendingGamepadKey);
			pendingGamepadKey = null;
		}
	}

	private void RefreshControlsUI()
	{
		press_any_key_text = LOC.MM.GetStart("press_any_key");
		ControlsSettingData currentEditingControlsData = GetCurrentEditingControlsData();
		if (currentEditingControlsData == null)
		{
			return;
		}
		bool hasValue = waitingAction.HasValue;
		foreach (ControlBindItem controlItem in controlItems)
		{
			if ((bool)controlItem)
			{
				string start = LOC.MM.GetStart("control_" + controlItem.Action.ToString().ToLower());
				controlItem.SetActionLabel(start);
				string bind = currentEditingControlsData.GetBind(controlItem.Action);
				if (bind == null)
				{
					controlItem.SetKey(string.Empty);
					controlItem.SetConflict(conflict: false);
					continue;
				}
				if (hasValue && waitingAction.Value == controlItem.Action)
				{
					controlItem.SetWaiting(press_any_key_text);
					controlItem.SetConflict(conflict: false);
					continue;
				}
				controlItem.SetKey(bind);
				bool conflict = !hasValue && IsKeyConflict(controlItem.Action, bind);
				controlItem.SetConflict(conflict);
				bool interactable = CanRebindAction(SingletonMonoGlobal<CurrentInputManager>.HasInstance ? SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType : InputDeviceType.PC, controlItem.Action);
				controlItem.SetInteractable(interactable);
			}
		}
	}

	private void CancelRebind()
	{
		ControlAction? controlAction = waitingAction;
		pendingKey = null;
		pendingMouse = null;
		waitingAction = null;
		pendingGamepadKey = null;
		waitReleaseBeforeGamepadListening = false;
		GamepadUINavigationManager.BlockGamepadUIInput = false;
		SetControlButtonsInteractable(enable: true);
		RefreshUI();
		if (controlAction.HasValue && SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			Button bindButtonByAction = GetBindButtonByAction(controlAction.Value);
			if ((bool)bindButtonByAction)
			{
				SetFirstSelected(bindButtonByAction);
			}
		}
	}

	private void ApplyRebind(string value)
	{
		ControlsSettingData currentEditingControlsData = GetCurrentEditingControlsData();
		if (currentEditingControlsData == null || !waitingAction.HasValue)
		{
			return;
		}
		ControlAction value2 = waitingAction.Value;
		if (!currentEditingControlsData.SetBindKey(value2, value))
		{
			CancelRebind();
			return;
		}
		pendingKey = null;
		pendingMouse = null;
		pendingGamepadKey = null;
		waitReleaseBeforeGamepadListening = false;
		waitingAction = null;
		GamepadUINavigationManager.BlockGamepadUIInput = false;
		SetControlButtonsInteractable(enable: true);
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			SingletonMonoScope<InputManager>.Instance.MarkActionBindingCacheDirty();
		}
		Singleton<GamepadUIActionManager>.Instance.MarkActionSemanticCacheDirty();
		RefreshUI();
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			Button bindButtonByAction = GetBindButtonByAction(value2);
			if ((bool)bindButtonByAction)
			{
				SetFirstSelected(bindButtonByAction);
			}
		}
	}

	private void SetControlBtnsInteractable(bool enable, ControlAction? keepAction = null)
	{
		GetControl<Button>("BackBtn").interactable = enable;
		GetControl<Button>("BottomConfirmBtn").interactable = enable;
		GetControl<Button>("BottomApplyBtn").interactable = enable;
		GetControl<Button>("BottomResetBtn").interactable = enable;
		foreach (ControlBindItem controlItem in controlItems)
		{
			if ((bool)controlItem)
			{
				if (!enable && keepAction.HasValue && controlItem.Action == keepAction.Value)
				{
					controlItem.SetInteractable(interactable: true);
				}
				else
				{
					controlItem.SetInteractable(enable);
				}
			}
		}
	}

	private static bool IsLockedGamepadAction(ControlAction action)
	{
		if ((uint)action <= 3u || action == ControlAction.Stats || action == ControlAction.PickUp)
		{
			return true;
		}
		return false;
	}

	private static bool CanRebindAction(InputDeviceType deviceType, ControlAction action)
	{
		if ((deviceType == InputDeviceType.Gamepad || deviceType == InputDeviceType.Xbox || deviceType == InputDeviceType.PlayStation || deviceType == InputDeviceType.Switch) && IsLockedGamepadAction(action))
		{
			return false;
		}
		return true;
	}

	private void HandleCurrentInputDeviceChanged(InputDeviceType deviceType)
	{
		if (Controls.activeSelf)
		{
			if (waitingAction.HasValue)
			{
				CancelRebind();
			}
			else
			{
				RefreshControlsUI();
			}
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		CurrentInputManager.OnCurrentInputDeviceChanged += HandleCurrentInputDeviceChanged;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		CurrentInputManager.OnCurrentInputDeviceChanged -= HandleCurrentInputDeviceChanged;
	}

	private void InitAudioSettings()
	{
		SettingDataManager mgr = Singleton<SettingDataManager>.Instance;
		audioItems.Clear();
		audioItems.Add(CreateAudioItem("setting_master_volume_slider", "setting_master_volume_num", delegate(float v)
		{
			mgr.SetMasterImmediate(v);
		}, () => mgr.GetAudio().master_volome));
		audioItems.Add(CreateAudioItem("setting_ambient_volume_slider", "setting_ambient_volume_num", delegate(float v)
		{
			mgr.SetAmbientImmediate(v);
		}, () => mgr.GetAudio().ambient_volume));
		audioItems.Add(CreateAudioItem("setting_sfx_volume_slider", "setting_sfx_volume_num", delegate(float v)
		{
			mgr.SetSFXImmediate(v);
		}, () => mgr.GetAudio().sfx_volome));
		audioItems.Add(CreateAudioItem("setting_music_volume_slider", "setting_music_volume_num", delegate(float v)
		{
			mgr.SetMusicImmediate(v);
		}, () => mgr.GetAudio().music_volome));
		audioItems.Add(CreateAudioItem("setting_ui_volume_slider", "setting_ui_volume_num", delegate(float v)
		{
			mgr.SetUIImmediate(v);
		}, () => mgr.GetAudio().ui_volome));
	}

	private AudioItem CreateAudioItem(string sliderName, string valueTextName, Action<float> setter, Func<float> getter)
	{
		Slider control = GetControl<Slider>(sliderName);
		Text text = GetControl<Text>(valueTextName);
		AudioItem item = new AudioItem
		{
			slider = control,
			valueText = text,
			setEditing = setter,
			getEditing = getter
		};
		control.onValueChanged.AddListener(delegate(float v)
		{
			item.setEditing?.Invoke(v);
			text.text = Mathf.RoundToInt(v * 100f).ToString();
			RefreshBottomButtons();
		});
		return item;
	}

	private void RefreshAudioUI()
	{
		foreach (AudioItem audioItem in audioItems)
		{
			float num = audioItem.getEditing?.Invoke() ?? 0f;
			audioItem.slider.SetValueWithoutNotify(num);
			audioItem.valueText.text = Mathf.RoundToInt(num * 100f).ToString();
		}
	}

	private static int GetFrameIndex(int frame)
	{
		for (int i = 0; i < FramePresets.Length; i++)
		{
			if (FramePresets[i] == frame)
			{
				return i;
			}
		}
		return 0;
	}

	private void InitVideoSettings()
	{
		resolutionText = GetControl<Text>("resolution_text");
		resolutionLeftBtn = GetControl<Button>("res_left_btn");
		resolutionRightBtn = GetControl<Button>("res_right_btn");
		frameText = GetControl<Text>("frame_text");
		frameLeftBtn = GetControl<Button>("frame_left_btn");
		frameRightBtn = GetControl<Button>("frame_right_btn");
		screenText = GetControl<Text>("screen_text");
		screenLeftBtn = GetControl<Button>("screen_left_btn");
		screenRightBtn = GetControl<Button>("screen_right_btn");
		lightSlider = GetControl<Slider>("globalLightSlider");
		vsyncToggle = GetControl<Toggle>("vsync_toggle");
		bloomToggle = GetControl<Toggle>("bloom_toggle");
		resolutionLeftBtn.onClick.AddListener(OnClickResolutionLeft);
		resolutionRightBtn.onClick.AddListener(OnClickResolutionRight);
		frameLeftBtn.onClick.AddListener(OnClickFrameLeft);
		frameRightBtn.onClick.AddListener(OnClickFrameRight);
		screenLeftBtn.onClick.AddListener(OnClickScreenLeft);
		screenRightBtn.onClick.AddListener(OnClickScreenRight);
		lightSlider.onValueChanged.AddListener(OnLightChanged);
		vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
		bloomToggle.onValueChanged.AddListener(OnBloomChanged);
	}

	private void RefreshVideoUI()
	{
		VideoSettingData editingVideo = Singleton<SettingDataManager>.Instance.GetEditingVideo();
		if (editingVideo != null)
		{
			bool interactable = !editingVideo.vsync;
			frameLeftBtn.interactable = interactable;
			frameRightBtn.interactable = interactable;
			resolutionText.text = DisplayLabelUtil.Resolution(editingVideo.resolution);
			frameText.text = DisplayLabelUtil.Frame(editingVideo.frame);
			screenText.text = DisplayLabelUtil.GetScreenModeName(editingVideo.fullScreenMode);
			lightSlider.SetValueWithoutNotify(editingVideo.global_light);
			vsyncToggle.SetIsOnWithoutNotify(editingVideo.vsync);
			bloomToggle.SetIsOnWithoutNotify(editingVideo.bloom);
		}
	}

	private void OnClickResolutionLeft()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		VideoSettingData editingVideo = instance.GetEditingVideo();
		if (editingVideo != null)
		{
			int resolution = (int)editingVideo.resolution;
			resolution = (resolution - 1 + ResolutionCount) % ResolutionCount;
			instance.SetResolutionEditing((ResolutionPreset)resolution);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnClickResolutionRight()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		VideoSettingData editingVideo = instance.GetEditingVideo();
		if (editingVideo != null)
		{
			int resolution = (int)editingVideo.resolution;
			resolution = (resolution + 1) % ResolutionCount;
			instance.SetResolutionEditing((ResolutionPreset)resolution);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnClickFrameLeft()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		VideoSettingData editingVideo = instance.GetEditingVideo();
		if (editingVideo != null)
		{
			int frameIndex = GetFrameIndex(editingVideo.frame);
			frameIndex = (frameIndex - 1 + FrameCount) % FrameCount;
			instance.SetFrameEditing(FramePresets[frameIndex]);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnClickFrameRight()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		VideoSettingData editingVideo = instance.GetEditingVideo();
		if (editingVideo != null)
		{
			int frameIndex = GetFrameIndex(editingVideo.frame);
			frameIndex = (frameIndex + 1) % FrameCount;
			instance.SetFrameEditing(FramePresets[frameIndex]);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnClickScreenLeft()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		VideoSettingData editingVideo = instance.GetEditingVideo();
		if (editingVideo != null)
		{
			int fullScreenMode = (int)editingVideo.fullScreenMode;
			fullScreenMode = (fullScreenMode - 1 + ScreenCount) % ScreenCount;
			instance.SetScreenModeEditing((ScreenMode)fullScreenMode);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnClickScreenRight()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		VideoSettingData editingVideo = instance.GetEditingVideo();
		if (editingVideo != null)
		{
			int fullScreenMode = (int)editingVideo.fullScreenMode;
			fullScreenMode = (fullScreenMode + 1) % ScreenCount;
			instance.SetScreenModeEditing((ScreenMode)fullScreenMode);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnVSyncChanged(bool isOn)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingVideo() != null)
		{
			instance.SetVSyncEditing(isOn);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnLightChanged(float value)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingVideo() != null)
		{
			instance.SetGlobalLightEditing(value);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void OnBloomChanged(bool isOn)
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		if (instance.GetEditingVideo() != null)
		{
			instance.SetBloomEditing(isOn);
			RefreshVideoUI();
			RefreshBottomButtons();
		}
	}

	private void InitFilterSettings()
	{
		GetControl<Button>("PLPickLeftBtn").onClick.AddListener(OnClickAutoPickLeft1);
		GetControl<Button>("PLPickRightBtn").onClick.AddListener(OnClickAutoPickRight1);
		GetControl<Button>("SPPickLeftBtn").onClick.AddListener(OnClickAutoPickLeft2);
		GetControl<Button>("SPPickRightBtn").onClick.AddListener(OnClickAutoPickRight2);
		GetControl<Button>("SPFJLeftBtn").onClick.AddListener(OnClickAutoPickLeft3);
		GetControl<Button>("SPFJRightBtn").onClick.AddListener(OnClickAutoPickRight3);
	}

	public void OnClickAutoPickLeft1()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		filterSettingData = instance.GetFilter();
		int player_Auto_Pickup = filterSettingData.Player_Auto_Pickup;
		player_Auto_Pickup = (player_Auto_Pickup - 1 + AutoPick1) % AutoPick1;
		instance.SetFilterPL((QulityType)player_Auto_Pickup);
		RefreshUI();
	}

	public void OnClickAutoPickRight1()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		filterSettingData = instance.GetFilter();
		int player_Auto_Pickup = filterSettingData.Player_Auto_Pickup;
		player_Auto_Pickup = (player_Auto_Pickup + 1) % AutoPick1;
		instance.SetFilterPL((QulityType)player_Auto_Pickup);
		RefreshUI();
	}

	public void OnClickAutoPickLeft2()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		filterSettingData = instance.GetFilter();
		int sprite_Auto_Pickup = filterSettingData.Sprite_Auto_Pickup;
		sprite_Auto_Pickup = (sprite_Auto_Pickup - 1 + AutoPick2) % AutoPick2;
		instance.SetFilterXJL((QulityType)sprite_Auto_Pickup);
		RefreshUI();
	}

	public void OnClickAutoPickRight2()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		filterSettingData = instance.GetFilter();
		int sprite_Auto_Pickup = filterSettingData.Sprite_Auto_Pickup;
		sprite_Auto_Pickup = (sprite_Auto_Pickup + 1) % AutoPick2;
		instance.SetFilterXJL((QulityType)sprite_Auto_Pickup);
		RefreshUI();
	}

	public void OnClickAutoPickLeft3()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		filterSettingData = instance.GetFilter();
		int sprite_Automatically_Salvages = filterSettingData.Sprite_Automatically_Salvages;
		sprite_Automatically_Salvages = (sprite_Automatically_Salvages - 1 + AutoPick3) % AutoPick3;
		instance.SetFilterXJL_FJ((QulityType)sprite_Automatically_Salvages);
		RefreshUI();
	}

	public void OnClickAutoPickRight3()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		filterSettingData = instance.GetFilter();
		int sprite_Automatically_Salvages = filterSettingData.Sprite_Automatically_Salvages;
		sprite_Automatically_Salvages = (sprite_Automatically_Salvages + 1) % AutoPick3;
		instance.SetFilterXJL_FJ((QulityType)sprite_Automatically_Salvages);
		RefreshUI();
	}

	private void RefreshFilterUI()
	{
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		FilterData editingFilter = instance.GetEditingFilter();
		GetControl<Text>("PLPickText").text = LOC.MM.GetStart(FilterManager.GetPickDisplayName((QulityType)editingFilter.Player_Auto_Pickup));
		FilterData editingFilter2 = instance.GetEditingFilter();
		GetControl<Text>("SPPickText").text = LOC.MM.GetStart(FilterManager.GetPickDisplayName((QulityType)editingFilter2.Sprite_Auto_Pickup));
		FilterData editingFilter3 = instance.GetEditingFilter();
		GetControl<Text>("SPFJText").text = LOC.MM.GetStart(FilterManager.GetFJDisplayName((QulityType)editingFilter3.Sprite_Automatically_Salvages));
	}
}
