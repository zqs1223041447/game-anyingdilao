using System;
using System.Collections.Generic;
using System.Reflection;
using Core;
using Data.AutoGen.DataClass.Settings;
using Display;
using FMODUnity;
using FinkFramework.Odin.OdinSerializer;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs;
using Inputs.Cursors;
using Inputs.Gamepad;
using Interact;
using Localization;
using PostProcess;
using SaveSystem;
using UI.Managers;
using UI.Map;
using UnityEngine;

public class SettingDataManager : Singleton<SettingDataManager>
{
	public const int CursorSizeMinPercent = 50;

	public const int CursorSizeMaxPercent = 200;

	public const int CursorSizeDefaultPercent = 100;

	private bool _inited;

	public static float level_light;

	private GameSettingData _defaultGame;

	private InterfaceSettingData _defaultInterface;

	private AudioSettingData _defaultAudio;

	private List<ControlsSettingData> _defaultControls;

	private VideoSettingData _defaultVideo;

	private FilterData _defaultFilter;

	private const string SteamDeckVideoDefaultsAppliedKey = "SettingDataManager.SteamDeckVideoDefaultsApplied";

	public GameSettingData Game { get; private set; }

	public InterfaceSettingData Interface { get; private set; }

	public AudioSettingData Audio { get; private set; }

	public List<ControlsSettingData> Controls { get; private set; }

	public VideoSettingData Video { get; private set; }

	public FilterData Filter { get; private set; }

	public GameSettingData EditingGame { get; private set; }

	public InterfaceSettingData EditingInterface { get; private set; }

	public List<ControlsSettingData> EditingControls { get; private set; }

	public VideoSettingData EditingVideo { get; private set; }

	public FilterData EditingFilter { get; private set; }

	public bool IsEditing { get; private set; }

	public bool HasPendingChanges
	{
		get
		{
			if (!IsEditing)
			{
				return false;
			}
			if (!AreEqual(Game, EditingGame))
			{
				return true;
			}
			if (!AreEqual(Interface, EditingInterface))
			{
				return true;
			}
			if (!AreEqual(Controls, EditingControls))
			{
				return true;
			}
			if (!AreEqual(Video, EditingVideo))
			{
				return true;
			}
			if (!AreEqual(Filter, EditingFilter))
			{
				return true;
			}
			return false;
		}
	}

	private SettingDataManager()
	{
	}

	public GameSettingData GetGame()
	{
		return Game;
	}

	public InterfaceSettingData GetInterface()
	{
		return Interface;
	}

	public AudioSettingData GetAudio()
	{
		return Audio;
	}

	public List<ControlsSettingData> GetControls()
	{
		return Controls;
	}

	public VideoSettingData GetVideo()
	{
		return Video;
	}

	public FilterData GetFilter()
	{
		return Filter;
	}

	public GameSettingData GetEditingGame()
	{
		return EditingGame ?? Game;
	}

	public InterfaceSettingData GetEditingInterface()
	{
		return EditingInterface ?? Interface;
	}

	public List<ControlsSettingData> GetEditingControls()
	{
		return EditingControls ?? Controls;
	}

	public VideoSettingData GetEditingVideo()
	{
		return EditingVideo ?? Video;
	}

	public FilterData GetEditingFilter()
	{
		return EditingFilter ?? Filter;
	}

	public void BeginEdit()
	{
		if (!_inited)
		{
			Init();
		}
		if (!IsEditing)
		{
			EditingGame = DataUtil.DeepClone(Game);
			EditingInterface = DataUtil.DeepClone(Interface);
			EditingControls = DataUtil.DeepClone(Controls);
			EditingVideo = DataUtil.DeepClone(Video);
			EditingFilter = DataUtil.DeepClone(Filter);
			IsEditing = true;
		}
	}

	public void CancelEdit()
	{
		EditingGame = null;
		EditingInterface = null;
		EditingControls = null;
		EditingVideo = null;
		EditingFilter = null;
		IsEditing = false;
		SyncAutoAttackRuntime(Game.auto_attack);
		ApplyCursorScale(Interface.cursor);
	}

	public static GameSettingData CreateGameDefault()
	{
		GameSettingDataContainer gameSettingDataContainer = FilesUtil.LoadDefaultData<GameSettingDataContainer>();
		if (gameSettingDataContainer == null || gameSettingDataContainer.items.Count == 0)
		{
			LogUtil.Error("SettingDataManager", "Game 默认数据缺失，无法创建默认值");
			return CreateFallbackGameDefault();
		}
		return DataUtil.DeepClone(gameSettingDataContainer.items[0]);
	}

	private static GameSettingData CreateFallbackGameDefault()
	{
		return new GameSettingData
		{
			language = 1,
			auto_save = true,
			auto_save_time = 600,
			left_invert_x = false,
			left_invert_y = false,
			right_invert_x = false,
			right_invert_y = false,
			autoChangeUseToggle = true,
			pcPickupMode = PcPickupMode.Best,
			mouse_move = false,
			QZ_Move = true,
			auto_lock1 = true,
			auto_lock2 = false,
			auto_attack = false,
			Dis_Skill1 = 60,
			Dis_Skill2 = 60,
			Dis_Skill3 = 60,
			Dis_Skill4 = 60,
			Dis_Skill5 = 60,
			Dis_Skill6 = 60,
			Dis_Skill7 = 60,
			Dis_Skill8 = 60
		};
	}

	public static InterfaceSettingData CreateInterfaceDefault()
	{
		InterfaceSettingDataContainer interfaceSettingDataContainer = FilesUtil.LoadDefaultData<InterfaceSettingDataContainer>();
		if (interfaceSettingDataContainer == null || interfaceSettingDataContainer.items.Count == 0)
		{
			LogUtil.Error("SettingDataManager", "Interface 默认数据缺失，无法创建默认值");
			return new InterfaceSettingData
			{
				cursor = 100
			};
		}
		InterfaceSettingData interfaceSettingData = DataUtil.DeepClone(interfaceSettingDataContainer.items[0]);
		interfaceSettingData.cursor = NormalizeCursorSizePercent(interfaceSettingData.cursor);
		return interfaceSettingData;
	}

	public static AudioSettingData CreateSoundDefault()
	{
		AudioSettingDataContainer audioSettingDataContainer = FilesUtil.LoadDefaultData<AudioSettingDataContainer>();
		if (audioSettingDataContainer == null || audioSettingDataContainer.items.Count == 0)
		{
			LogUtil.Error("SettingDataManager", "Audio 默认数据缺失，无法创建默认值");
			return new AudioSettingData();
		}
		return DataUtil.DeepClone(audioSettingDataContainer.items[0]);
	}

	private static List<ControlsSettingData> CreateControlsDefault()
	{
		ControlsSettingDataContainer controlsSettingDataContainer = FilesUtil.LoadDefaultData<ControlsSettingDataContainer>();
		if (controlsSettingDataContainer == null || controlsSettingDataContainer.items.Count == 0)
		{
			LogUtil.Error("SettingDataManager", "Controls 默认数据缺失");
			return new List<ControlsSettingData>
			{
				new ControlsSettingData()
			};
		}
		return DataUtil.DeepClone(controlsSettingDataContainer.items);
	}

	private static VideoSettingData CreateVideoDefault()
	{
		VideoSettingDataContainer videoSettingDataContainer = FilesUtil.LoadDefaultData<VideoSettingDataContainer>();
		VideoSettingData videoSettingData;
		if (videoSettingDataContainer == null || videoSettingDataContainer.items.Count == 0)
		{
			LogUtil.Warn("SettingDataManager", "Video 默认数据缺失，使用空模板");
			videoSettingData = new VideoSettingData();
		}
		else
		{
			videoSettingData = DataUtil.DeepClone(videoSettingDataContainer.items[0]);
		}
		if (SteamManager.IsRunningOnSteamDeck())
		{
			ApplySteamDeckVideoDefaults(videoSettingData);
			return videoSettingData;
		}
		int num = 0;
		int num2 = 0;
		Resolution[] resolutions = Screen.resolutions;
		if (resolutions != null && resolutions.Length != 0)
		{
			int num3 = -1;
			for (int i = 0; i < resolutions.Length; i++)
			{
				int num4 = resolutions[i].width * resolutions[i].height;
				if (num4 > num3)
				{
					num3 = num4;
					num = resolutions[i].width;
					num2 = resolutions[i].height;
				}
			}
		}
		if (num <= 0 || num2 <= 0)
		{
			num = UnityEngine.Display.main.systemWidth;
			num2 = UnityEngine.Display.main.systemHeight;
		}
		if (num <= 0 || num2 <= 0)
		{
			Resolution currentResolution = Screen.currentResolution;
			num = currentResolution.width;
			num2 = currentResolution.height;
		}
		videoSettingData.resolution = DisplayMappingUtil.GetDefaultPresetByResolution(num, num2);
		if (!Enum.IsDefined(typeof(ScreenMode), videoSettingData.fullScreenMode))
		{
			videoSettingData.fullScreenMode = ScreenMode.FullScreenWindow;
		}
		videoSettingData.frame = ((videoSettingData.frame >= 0) ? videoSettingData.frame : 0);
		return videoSettingData;
	}

	private static bool ApplySteamDeckVideoDefaults(VideoSettingData data)
	{
		if (data == null)
		{
			return false;
		}
		bool result = false;
		if (data.resolution != ResolutionPreset.R1280x800)
		{
			data.resolution = ResolutionPreset.R1280x800;
			result = true;
		}
		if (data.fullScreenMode != ScreenMode.Windowed)
		{
			data.fullScreenMode = ScreenMode.Windowed;
			result = true;
		}
		if (data.frame < 0)
		{
			data.frame = 0;
			result = true;
		}
		return result;
	}

	private static bool TryApplySteamDeckVideoDefaultsOnce(VideoSettingData data)
	{
		if (data == null || !SteamManager.IsRunningOnSteamDeck())
		{
			return false;
		}
		if (PlayerPrefs.GetInt("SettingDataManager.SteamDeckVideoDefaultsApplied", 0) == 1)
		{
			return false;
		}
		bool num = ShouldReplaceLegacySteamDeckVideoSetting(data);
		PlayerPrefs.SetInt("SettingDataManager.SteamDeckVideoDefaultsApplied", 1);
		PlayerPrefs.Save();
		if (!num)
		{
			return false;
		}
		bool num2 = ApplySteamDeckVideoDefaults(data);
		if (num2)
		{
			LogUtil.Info("SettingDataManager", "Steam Deck 视频设置已迁移为 1280x800 Windowed");
		}
		return num2;
	}

	private static bool ShouldReplaceLegacySteamDeckVideoSetting(VideoSettingData data)
	{
		if (data == null)
		{
			return false;
		}
		if (data.fullScreenMode == ScreenMode.FullScreenWindow)
		{
			return true;
		}
		ResolutionInfo resolution = DisplayMappingUtil.GetResolution(data.resolution);
		if (resolution.width <= 1280)
		{
			return resolution.height > 800;
		}
		return true;
	}

	private static void MarkSteamDeckVideoDefaultsApplied()
	{
		if (SteamManager.IsRunningOnSteamDeck() && PlayerPrefs.GetInt("SettingDataManager.SteamDeckVideoDefaultsApplied", 0) != 1)
		{
			PlayerPrefs.SetInt("SettingDataManager.SteamDeckVideoDefaultsApplied", 1);
			PlayerPrefs.Save();
		}
	}

	public static FilterData CreateFilterDefault()
	{
		FilterDataContainer filterDataContainer = FilesUtil.LoadDefaultData<FilterDataContainer>();
		if (filterDataContainer == null || filterDataContainer.items.Count == 0)
		{
			LogUtil.Error("SettingDataManager", "Filter default data missing, using fallback defaults.");
			return CreateFallbackFilterDefault();
		}
		FilterData filterData = DataUtil.DeepClone(filterDataContainer.items[0]);
		ApplyFilterDefaultFallbacks(filterData);
		return filterData;
	}

	private static FilterData CreateFallbackFilterDefault()
	{
		FilterData filterData = new FilterData();
		ApplyFilterDefaultFallbacks(filterData);
		return filterData;
	}

	private static void ApplyFilterDefaultFallbacks(FilterData data)
	{
		if (data != null)
		{
			if (data.Player_Auto_Pickup == 0)
			{
				data.Player_Auto_Pickup = 1;
			}
			if (data.Sprite_Auto_Pickup == 0)
			{
				data.Sprite_Auto_Pickup = 1;
			}
			if (data.Sprite_Automatically_Salvages == 0)
			{
				data.Sprite_Automatically_Salvages = 1;
			}
		}
	}

	private static bool TryLoadLocalFirst<TContainer, TItem>(out TItem item) where TContainer : class
	{
		item = default(TItem);
		if (!FilesUtil.HasLocalData<TContainer>())
		{
			return false;
		}
		if (!TryGetContainerItems(FilesUtil.LoadLocalData<TContainer>(), out List<TItem> items) || items == null || items.Count == 0 || items[0] == null)
		{
			LogUtil.Warn("SettingDataManager", typeof(TContainer).Name + " 本地设置读取失败，使用默认设置重建");
			return false;
		}
		item = DataUtil.DeepClone(items[0]);
		return true;
	}

	private static bool TryLoadLocalList<TContainer, TItem>(out List<TItem> items) where TContainer : class
	{
		items = null;
		if (!FilesUtil.HasLocalData<TContainer>())
		{
			return false;
		}
		if (!TryGetContainerItems(FilesUtil.LoadLocalData<TContainer>(), out items) || items == null || items.Count == 0)
		{
			LogUtil.Warn("SettingDataManager", typeof(TContainer).Name + " 本地设置读取失败，使用默认设置重建");
			return false;
		}
		items = DataUtil.DeepClone(items);
		return true;
	}

	private static bool TryGetContainerItems<TContainer, TItem>(TContainer container, out List<TItem> items) where TContainer : class
	{
		items = null;
		if (container == null)
		{
			return false;
		}
		FieldInfo field = typeof(TContainer).GetField("items");
		if (field == null)
		{
			return false;
		}
		items = field.GetValue(container) as List<TItem>;
		return items != null;
	}

	public void Init()
	{
		if (_inited)
		{
			return;
		}
		bool num = !FilesUtil.HasLocalData<GameSettingDataContainer>();
		GameSettingData item;
		bool flag = TryLoadLocalFirst<GameSettingDataContainer, GameSettingData>(out item);
		Game = (flag ? item : CreateGameDefault());
		if (num)
		{
			LanguageType defaultGameLanguage = LanguageDetectUtil.GetDefaultGameLanguage();
			Game.language = (int)defaultGameLanguage;
			LogUtil.Info($"首次启动，自动设置默认语言为: {defaultGameLanguage}");
		}
		if (!flag)
		{
			SaveGame();
		}
		InterfaceSettingData item2;
		bool num2 = TryLoadLocalFirst<InterfaceSettingDataContainer, InterfaceSettingData>(out item2);
		if (!num2)
		{
			Interface = CreateInterfaceDefault();
		}
		else
		{
			Interface = item2;
		}
		int num3 = NormalizeCursorSizePercent(Interface.cursor);
		bool flag2 = Interface.cursor != num3;
		Interface.cursor = num3;
		if (!num2 || flag2)
		{
			SaveInterface();
		}
		if (!TryLoadLocalFirst<AudioSettingDataContainer, AudioSettingData>(out var item3))
		{
			Audio = CreateSoundDefault();
			SaveAudio();
		}
		else
		{
			Audio = item3;
		}
		if (!TryLoadLocalList<ControlsSettingDataContainer, ControlsSettingData>(out var items))
		{
			Controls = CreateControlsDefault();
			SaveControls();
		}
		else
		{
			Controls = items;
		}
		bool num4 = NormalizeActbarControlBindings(Controls);
		if (!TryLoadLocalFirst<VideoSettingDataContainer, VideoSettingData>(out var item4))
		{
			Video = CreateVideoDefault();
			SaveVideo();
			MarkSteamDeckVideoDefaultsApplied();
		}
		else
		{
			Video = item4;
			if (TryApplySteamDeckVideoDefaultsOnce(Video))
			{
				SaveVideo();
			}
		}
		if (!TryLoadLocalFirst<FilterDataContainer, FilterData>(out var item5))
		{
			Filter = CreateFilterDefault();
			SaveFilter();
		}
		else
		{
			Filter = item5;
		}
		_defaultGame = CreateGameDefault();
		_defaultInterface = CreateInterfaceDefault();
		_defaultAudio = CreateSoundDefault();
		_defaultControls = CreateControlsDefault();
		NormalizeActbarControlBindings(_defaultControls);
		_defaultVideo = CreateVideoDefault();
		_defaultFilter = CreateFilterDefault();
		if (num4)
		{
			SaveControls();
		}
		ApplyRuntimeSettings();
		_inited = true;
	}

	private void SaveAll()
	{
		SaveGame();
		SaveInterface();
		SaveAudio();
		SaveControls();
		SaveVideo();
		SaveFilter();
	}

	public void ResetAllToDefault()
	{
		if (IsEditing)
		{
			int language = EditingGame.language;
			EditingGame = DataUtil.DeepClone(_defaultGame);
			EditingGame.language = language;
			EditingInterface = DataUtil.DeepClone(_defaultInterface);
			EditingControls = DataUtil.DeepClone(_defaultControls);
			EditingVideo = DataUtil.DeepClone(_defaultVideo);
			EditingFilter = DataUtil.DeepClone(_defaultFilter);
			SyncAutoAttackRuntime(EditingGame.auto_attack);
		}
	}

	public void Apply()
	{
		if (IsEditing)
		{
			Game = DataUtil.DeepClone(EditingGame);
			Interface = DataUtil.DeepClone(EditingInterface);
			Controls = DataUtil.DeepClone(EditingControls);
			Video = DataUtil.DeepClone(EditingVideo);
			Filter = DataUtil.DeepClone(EditingFilter);
			InputBind.ClearCache();
			ApplyGameSettings();
			ApplyInterfaceSettings();
			ApplyVideoSettings();
			SaveAll();
		}
	}

	private void ApplyRuntimeSettings()
	{
		ApplyGameSettings();
		ApplyInterfaceSettings();
		ApplyVideoSettings();
		ApplyAudioSettings();
		ApplyFilterSettings();
	}

	public void Confirm()
	{
		Apply();
		CancelEdit();
	}

	public void ResetGame()
	{
		if (IsEditing)
		{
			int language = EditingGame.language;
			EditingGame = DataUtil.DeepClone(_defaultGame);
			EditingGame.language = language;
			SyncAutoAttackRuntime(EditingGame.auto_attack);
		}
	}

	public void SaveGame()
	{
		GameSettingDataContainer gameSettingDataContainer = new GameSettingDataContainer();
		gameSettingDataContainer.items.Clear();
		gameSettingDataContainer.items.Add(Game);
		FilesUtil.SaveLocalData(gameSettingDataContainer);
	}

	public void ApplyGameSettings()
	{
		if (SingletonMonoScope<AutoSaveManager>.HasInstance)
		{
			SingletonMonoScope<AutoSaveManager>.Instance.SetEnable(Game.auto_save);
			SingletonMonoScope<AutoSaveManager>.Instance.SetInterval(Game.auto_save_time);
		}
		GamepadInputManager.SetInvertLeftX(Game.left_invert_x);
		GamepadInputManager.SetInvertLeftY(Game.left_invert_y);
		GamepadInputManager.SetInvertRightX(Game.right_invert_x);
		GamepadInputManager.SetInvertRightY(Game.right_invert_y);
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.SetAutoReplaceUseBinding(Game.autoChangeUseToggle);
		}
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			SyncAutoAttackRuntime(Game.auto_attack);
			SingletonMonoScope<PlayerManager>.Instance.RefreshMouseMoveRuntime();
		}
	}

	public void SetLanguageImmediate(LanguageType lang)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingGame != null)
		{
			if (EditingGame.language == (int)lang)
			{
				return;
			}
			EditingGame.language = (int)lang;
		}
		if (Game.language != (int)lang)
		{
			Game.language = (int)lang;
			LOC.MM.SetLanguageSetting(lang);
		}
		SaveGame();
	}

	public void SetPickModeEditing(PcPickupMode mode)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.pcPickupMode = mode;
		}
	}

	public void SetAutoSaveToggleEditing(bool b)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.auto_save = b;
		}
	}

	public void SetMouseMoveToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.mouse_move = toggle;
		}
	}

	public void SetForceMoveToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.QZ_Move = toggle;
		}
	}

	public void SetAutoLockToggleEditing(bool toggle)
	{
		SetAutoLockGamepadToggleEditing(toggle);
	}

	public void SetAutoLockGamepadToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.auto_lock1 = toggle;
		}
	}

	public void SetAutoLockPcToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.auto_lock2 = toggle;
		}
	}

	public bool IsAutoLockActiveForCurrentInput()
	{
		if (!_inited)
		{
			Init();
		}
		return IsAutoLockActiveForCurrentInput(Game);
	}

	public static bool IsAutoLockActiveForCurrentInput(GameSettingData game)
	{
		if (game == null)
		{
			return false;
		}
		if (!ShouldUseGamepadAutoLock())
		{
			return game.auto_lock2;
		}
		return game.auto_lock1;
	}

	private static bool ShouldUseGamepadAutoLock()
	{
		if (SteamManager.IsRunningOnSteamDeck())
		{
			return true;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent();
		}
		return false;
	}

	public void SetAutoAttackToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.auto_attack = toggle;
			SyncAutoAttackRuntime(toggle);
		}
	}

	private static void SyncAutoAttackRuntime(bool enabled)
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			SingletonMonoScope<PlayerManager>.Instance.AutoAttackEnabled = enabled;
		}
	}

	public void SetAutoSaveTimeEditing(int timeMulti)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			timeMulti = Mathf.Clamp(timeMulti, 180, 1800);
			if (!Mathf.Approximately(EditingGame.auto_save_time, timeMulti))
			{
				EditingGame.auto_save_time = timeMulti;
			}
		}
	}

	public void SetLeftXToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.left_invert_x = toggle;
		}
	}

	public void SetLeftYToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.left_invert_y = toggle;
		}
	}

	public void SetRightXToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.right_invert_x = toggle;
		}
	}

	public void SetRightYToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.right_invert_y = toggle;
		}
	}

	public void SetAutoChangeUseToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingGame.autoChangeUseToggle = toggle;
		}
	}

	public void SetGamepadSkillDistanceEditing(int skillIndex, int percent)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			percent = Mathf.Clamp(percent, 10, 100);
			SetGamepadSkillDistance(EditingGame, skillIndex, percent);
		}
	}

	public int GetGamepadSkillDistancePercent(int skillIndex)
	{
		if (!_inited)
		{
			Init();
		}
		return GetGamepadSkillDistancePercent(Game, skillIndex);
	}

	public static int GetGamepadSkillDistancePercent(GameSettingData data, int skillIndex)
	{
		if (data == null)
		{
			return 50;
		}
		int num = skillIndex switch
		{
			1 => data.Dis_Skill1, 
			2 => data.Dis_Skill2, 
			3 => data.Dis_Skill3, 
			4 => data.Dis_Skill4, 
			5 => data.Dis_Skill5, 
			6 => data.Dis_Skill6, 
			7 => data.Dis_Skill7, 
			8 => data.Dis_Skill8, 
			_ => 50, 
		};
		return Mathf.Clamp((num <= 0) ? 50 : num, 10, 100);
	}

	private static void SetGamepadSkillDistance(GameSettingData data, int skillIndex, int percent)
	{
		if (data != null)
		{
			switch (skillIndex)
			{
			case 1:
				data.Dis_Skill1 = percent;
				break;
			case 2:
				data.Dis_Skill2 = percent;
				break;
			case 3:
				data.Dis_Skill3 = percent;
				break;
			case 4:
				data.Dis_Skill4 = percent;
				break;
			case 5:
				data.Dis_Skill5 = percent;
				break;
			case 6:
				data.Dis_Skill6 = percent;
				break;
			case 7:
				data.Dis_Skill7 = percent;
				break;
			case 8:
				data.Dis_Skill8 = percent;
				break;
			}
		}
	}

	public void ResetInterface()
	{
		if (IsEditing)
		{
			EditingInterface = DataUtil.DeepClone(_defaultInterface);
		}
	}

	public void SaveInterface()
	{
		InterfaceSettingDataContainer interfaceSettingDataContainer = new InterfaceSettingDataContainer();
		interfaceSettingDataContainer.items.Clear();
		interfaceSettingDataContainer.items.Add(Interface);
		FilesUtil.SaveLocalData(interfaceSettingDataContainer);
	}

	public void ApplyInterfaceSettings()
	{
		if (SingletonMonoScope<DamgeTextManager>.HasInstance)
		{
			SingletonMonoScope<DamgeTextManager>.Instance.SetSCTScale(Interface.damage_scale);
			SingletonMonoScope<DamgeTextManager>.Instance.SetSCTToggle(Interface.damage_text);
		}
		if (SingletonMonoScope<MapManager>.HasInstance)
		{
			SingletonMonoScope<MapManager>.Instance.SetEnable(Interface.map_toggle);
			SingletonMonoScope<MapManager>.Instance.SetMode(Interface.map_mode);
			SingletonMonoScope<MapManager>.Instance.SetMapScale(Interface.map_scale);
			SingletonMonoScope<MapManager>.Instance.SetMapView(Interface.map_view_range);
			SingletonMonoScope<MapManager>.Instance.SetMapGlobalAlpha(Interface.map_global_alpha);
			SingletonMonoScope<MapManager>.Instance.SetMapBorderAlpha(Interface.map_border_alpha);
		}
		if (SingletonMonoScope<DisplayItemManager>.HasInstance)
		{
			if (!Interface.display_item)
			{
				SingletonMonoScope<DisplayItemManager>.Instance.DropItemUI_IsOpened = false;
				SingletonMonoScope<DisplayItemManager>.Instance.ChangeItemUI_Off();
			}
			else
			{
				SingletonMonoScope<DisplayItemManager>.Instance.DropItemUI_IsOpened = true;
				SingletonMonoScope<DisplayItemManager>.Instance.ChangeItemUI_On();
			}
		}
		ApplyCursorScale(Interface.cursor);
		if (SingletonMonoScope<GamepadAimManager>.HasInstance)
		{
			GamepadAimManager.SetAimPointImage(Interface.aim_point);
		}
		if (SingletonMonoScope<CursorInputManager>.HasInstance)
		{
			CursorInputManager.SetMoveSpeed(Interface.cursor_speed);
		}
		if (SingletonMonoScope<ItemTipManager>.HasInstance)
		{
			SingletonMonoScope<ItemTipManager>.Instance.SetItemTipEnable(Interface.item_tip);
		}
	}

	public void SetDamageToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingInterface.damage_text != toggle)
		{
			EditingInterface.damage_text = toggle;
		}
	}

	public void SetCursorSpeedScaleEditing(float scale)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && !Mathf.Approximately(EditingInterface.damage_scale, scale))
		{
			EditingInterface.cursor_speed = scale;
		}
	}

	public void SetAimToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingInterface.aim_point != toggle)
		{
			EditingInterface.aim_point = toggle;
		}
	}

	public void SetItemTipEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingInterface.item_tip != toggle)
		{
			EditingInterface.item_tip = toggle;
		}
	}

	public void SetDamageScaleEditing(float scale)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			scale = Mathf.Clamp(scale, 1f, 3f);
			if (!Mathf.Approximately(EditingInterface.damage_scale, scale))
			{
				EditingInterface.damage_scale = scale;
			}
		}
	}

	public void ToggleMapModeImmediate()
	{
		if (!_inited)
		{
			Init();
		}
		if (!Interface.map_toggle)
		{
			Interface.map_toggle = true;
			Interface.map_mode = MapDisplayMode.Minimap;
		}
		else
		{
			switch (Interface.map_mode)
			{
			case MapDisplayMode.Minimap:
				Interface.map_mode = MapDisplayMode.WorldLeft;
				break;
			case MapDisplayMode.WorldLeft:
				Interface.map_mode = MapDisplayMode.WorldCenter;
				break;
			case MapDisplayMode.WorldCenter:
				Interface.map_mode = MapDisplayMode.WorldRight;
				break;
			case MapDisplayMode.WorldRight:
				Interface.map_toggle = false;
				break;
			default:
				Interface.map_mode = MapDisplayMode.Minimap;
				break;
			}
		}
		if (SingletonMonoScope<MapManager>.HasInstance)
		{
			SingletonMonoScope<MapManager>.Instance.SetMode(Interface.map_mode);
			SingletonMonoScope<MapManager>.Instance.SetEnable(Interface.map_toggle);
		}
		SaveInterface();
	}

	public void SetItemToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingInterface.display_item = toggle;
		}
	}

	public void SetMapModeEditing(MapDisplayMode mode)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingInterface.map_mode = mode;
		}
	}

	public void SetMapToggleEditing(bool toggle)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing)
		{
			EditingInterface.map_toggle = toggle;
		}
	}

	public void SetMapScaleEditing(float scale)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && !Mathf.Approximately(EditingInterface.map_scale, scale))
		{
			EditingInterface.map_scale = scale;
		}
	}

	public void SetMapViewRangeEditing(float scale)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && !Mathf.Approximately(EditingInterface.map_view_range, scale))
		{
			EditingInterface.map_view_range = scale;
		}
	}

	public void SetMapGlobalAlphaEditing(float scale)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && !Mathf.Approximately(EditingInterface.map_global_alpha, scale))
		{
			EditingInterface.map_global_alpha = scale;
		}
	}

	public void SetMapBorderAlphaEditing(float scale)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && !Mathf.Approximately(EditingInterface.map_border_alpha, scale))
		{
			EditingInterface.map_border_alpha = scale;
		}
	}

	public void SetCursorEditing(float value)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingInterface != null)
		{
			int num = NormalizeCursorSizePercent(Mathf.RoundToInt(value));
			if (EditingInterface.cursor != num)
			{
				EditingInterface.cursor = num;
				ApplyCursorScale(num);
			}
		}
	}

	private static int NormalizeCursorSizePercent(int value)
	{
		return value switch
		{
			0 => 50, 
			1 => 75, 
			2 => 100, 
			_ => Mathf.Clamp(value, 50, 200), 
		};
	}

	private static void ApplyCursorScale(int cursorSizePercent)
	{
		if (SingletonMonoGlobal<CursorManager>.HasInstance)
		{
			int num = NormalizeCursorSizePercent(cursorSizePercent);
			SingletonMonoGlobal<CursorManager>.Instance.SetScale((float)num / 100f);
		}
	}

	private void ApplyAudioSettings()
	{
		RuntimeManager.GetVCA("vca:/Atmos").setVolume(Audio.ambient_volume);
		RuntimeManager.GetVCA("vca:/Master").setVolume(Audio.master_volome);
		RuntimeManager.GetVCA("vca:/Music").setVolume(Audio.music_volome);
		RuntimeManager.GetVCA("vca:/SFX").setVolume(Audio.sfx_volome);
		RuntimeManager.GetVCA("vca:/UI").setVolume(Audio.ui_volome);
	}

	public void ResetAudioImmediate()
	{
		if (!_inited)
		{
			Init();
		}
		Audio = DataUtil.DeepClone(_defaultAudio);
		ApplyAudioSettings();
		SaveAudio();
	}

	public void SaveAudio()
	{
		AudioSettingDataContainer audioSettingDataContainer = new AudioSettingDataContainer();
		audioSettingDataContainer.items.Clear();
		audioSettingDataContainer.items.Add(Audio);
		FilesUtil.SaveLocalData(audioSettingDataContainer);
	}

	public void SetMusicImmediate(float v)
	{
		if (!_inited)
		{
			Init();
		}
		if (!Mathf.Approximately(Audio.music_volome, v))
		{
			Audio.music_volome = v;
			RuntimeManager.GetVCA("vca:/Music").setVolume(v);
			SaveAudio();
		}
	}

	public void SetMasterImmediate(float v)
	{
		if (!_inited)
		{
			Init();
		}
		if (!Mathf.Approximately(Audio.master_volome, v))
		{
			Audio.master_volome = v;
			RuntimeManager.GetVCA("vca:/Master").setVolume(v);
			SaveAudio();
		}
	}

	public void SetSFXImmediate(float v)
	{
		if (!_inited)
		{
			Init();
		}
		if (!Mathf.Approximately(Audio.sfx_volome, v))
		{
			Audio.sfx_volome = v;
			RuntimeManager.GetVCA("vca:/SFX").setVolume(v);
			SaveAudio();
		}
	}

	public void SetUIImmediate(float v)
	{
		if (!_inited)
		{
			Init();
		}
		if (!Mathf.Approximately(Audio.ui_volome, v))
		{
			Audio.ui_volome = v;
			RuntimeManager.GetVCA("vca:/UI").setVolume(v);
			SaveAudio();
		}
	}

	public void SetAmbientImmediate(float v)
	{
		if (!_inited)
		{
			Init();
		}
		if (!Mathf.Approximately(Audio.ambient_volume, v))
		{
			Audio.ambient_volume = v;
			RuntimeManager.GetVCA("vca:/Atmos").setVolume(v);
			SaveAudio();
		}
	}

	public void ResetControls()
	{
		if (IsEditing)
		{
			EditingControls = DataUtil.DeepClone(_defaultControls);
			NormalizeActbarControlBindings(EditingControls);
		}
	}

	public void SaveControls()
	{
		FilesUtil.SaveLocalData(new ControlsSettingDataContainer
		{
			items = DataUtil.DeepClone(Controls)
		});
	}

	private static ControlsSettingData FindControlsByDevice(List<ControlsSettingData> list, InputDeviceType deviceType)
	{
		if (list == null || list.Count == 0)
		{
			return null;
		}
		foreach (ControlsSettingData item in list)
		{
			if (item != null && item.device == deviceType)
			{
				return item;
			}
		}
		if (deviceType == InputDeviceType.Xbox || deviceType == InputDeviceType.PlayStation || deviceType == InputDeviceType.Switch || deviceType == InputDeviceType.Gamepad)
		{
			foreach (ControlsSettingData item2 in list)
			{
				if (item2 != null && item2.device == InputDeviceType.Gamepad)
				{
					return item2;
				}
			}
		}
		foreach (ControlsSettingData item3 in list)
		{
			if (item3 != null && item3.device == InputDeviceType.PC)
			{
				return item3;
			}
		}
		return list[0];
	}

	public ControlsSettingData GetControl(InputDeviceType deviceType)
	{
		return FindControlsByDevice(Controls, deviceType);
	}

	public ControlsSettingData GetEditingControl(InputDeviceType deviceType)
	{
		return FindControlsByDevice(EditingControls ?? Controls, deviceType);
	}

	public ControlsSettingData GetCurrentControl()
	{
		InputDeviceType deviceType = (SingletonMonoGlobal<CurrentInputManager>.HasInstance ? SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType : InputDeviceType.PC);
		return GetControl(deviceType);
	}

	public ControlsSettingData GetCurrentEditingControl()
	{
		InputDeviceType deviceType = (SingletonMonoGlobal<CurrentInputManager>.HasInstance ? SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType : InputDeviceType.PC);
		return GetEditingControl(deviceType);
	}

	private static bool NormalizeActbarControlBindings(List<ControlsSettingData> controls)
	{
		if (controls == null)
		{
			return false;
		}
		bool changed = false;
		foreach (ControlsSettingData control in controls)
		{
			if (control != null)
			{
				if (control.device == InputDeviceType.PC)
				{
					SetBindIfMissing(ref control.Skill5, "Alpha3");
					SetBindIfMissing(ref control.Skill6, "Alpha4");
					SetBindIfMissing(ref control.Skill7, "Alpha5");
					SetBindIfMissing(ref control.Skill8, "Space");
					SetBindIfMissing(ref control.TP, "B");
					SetBindIfMissing(ref control.QuickUse, "C");
					SetBindIfMissing(ref control.Mercenary, "Y");
					SetBindIfMissing(ref control.PageL, "Mouse_WheelUp");
					SetBindIfMissing(ref control.PageR, "Mouse_WheelDown");
					SetBindIfMissing(ref control.AutoAT, "Mouse2");
					continue;
				}
				SetGamepadSkillBind(ref control.Skill1, "Pad_RB");
				SetGamepadSkillBind(ref control.Skill2, "Pad_RB");
				SetGamepadSkillBind(ref control.Skill3, "Pad_RT");
				SetGamepadSkillBind(ref control.Skill4, "Pad_RT");
				SetGamepadSkillBind(ref control.Skill5, "Pad_LB");
				SetGamepadSkillBind(ref control.Skill6, "Pad_LB");
				SetGamepadSkillBind(ref control.Skill7, "Pad_LT");
				SetGamepadSkillBind(ref control.Skill8, "Pad_LT");
				SetBindIfMissing(ref control.Item1, "Pad_X");
				SetBindIfMissing(ref control.Item2, "Pad_Y");
				SetBindIfMissing(ref control.TP, "Pad_B");
				SetBindIfMissing(ref control.PickUp, "Pad_A");
				SetBindIfMissing(ref control.QuickUse, "Pad_RStickPress");
				SetBindIfMissing(ref control.Mercenary, "Pad_DPadUp");
				SetBindIfMissing(ref control.Talent, "Pad_DPadRight");
				SetBindIfMissing(ref control.Bag, "Pad_DPadDown");
				SetBindIfMissing(ref control.Stats, "Pad_DPadLeft");
				SetBindIfMissing(ref control.MapMode, "Pad_Back");
				SetBindIfMissing(ref control.Sell, "Pad_X");
				SetBindIfMissing(ref control.SellAll, "Pad_Y");
				SetBindIfMissing(ref control.PageL, "Pad_LB");
				SetBindIfMissing(ref control.PageR, "Pad_RB");
				SetBindIfMissing(ref control.SortAll, "Pad_LT");
				SetBindIfMissing(ref control.Sort, "Pad_RT");
				SetBindIfMissing(ref control.AutoAT, "Pad_LStickPress");
			}
		}
		return changed;
		void SetBindIfMissing(ref string bind, string expected)
		{
			if (string.IsNullOrWhiteSpace(bind))
			{
				bind = expected;
				changed = true;
			}
		}
		void SetGamepadSkillBind(ref string bind, string expected)
		{
			if (string.IsNullOrWhiteSpace(bind) || !GamepadKeys.CanBind(KeyNameUtil.NormalizeKeyName(bind)))
			{
				bind = expected;
				changed = true;
			}
		}
	}

	public void ResetVideo()
	{
		if (IsEditing)
		{
			EditingVideo = DataUtil.DeepClone(_defaultVideo);
		}
	}

	public void SaveVideo()
	{
		VideoSettingDataContainer videoSettingDataContainer = new VideoSettingDataContainer();
		videoSettingDataContainer.items.Clear();
		videoSettingDataContainer.items.Add(Video);
		FilesUtil.SaveLocalData(videoSettingDataContainer);
	}

	public void ApplyVideoSettings()
	{
		VideoSettingData video = Video;
		if (video != null)
		{
			Singleton<DisplayManager>.Instance.Apply(video);
			if (video.vsync)
			{
				QualitySettings.vSyncCount = 1;
				Application.targetFrameRate = -1;
			}
			else
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = ((video.frame <= 0) ? (-1) : video.frame);
			}
			if (SingletonMonoGlobal<PostProcessManager>.HasInstance)
			{
				SingletonMonoGlobal<PostProcessManager>.Instance.SetGlobalLightIntensity(video.global_light + level_light);
				SingletonMonoGlobal<PostProcessManager>.Instance.SetBloomEnabled(video.bloom);
			}
		}
	}

	public void SetResolutionEditing(ResolutionPreset preset)
	{
		if (IsEditing && EditingVideo.resolution != preset)
		{
			EditingVideo.resolution = preset;
		}
	}

	public void SetFrameEditing(int frame)
	{
		if (IsEditing && EditingVideo.frame != frame)
		{
			EditingVideo.frame = frame;
		}
	}

	public void SetScreenModeEditing(ScreenMode mode)
	{
		if (IsEditing && EditingVideo != null && EditingVideo.fullScreenMode != mode)
		{
			EditingVideo.fullScreenMode = mode;
		}
	}

	public void SetVSyncEditing(bool enable)
	{
		if (IsEditing && EditingVideo != null && EditingVideo.vsync != enable)
		{
			EditingVideo.vsync = enable;
		}
	}

	public void SetGlobalLightEditing(float value)
	{
		if (IsEditing && EditingVideo != null && !Mathf.Approximately(EditingVideo.global_light, value))
		{
			EditingVideo.global_light = value;
		}
	}

	public void SetBloomEditing(bool enable)
	{
		if (IsEditing && EditingVideo != null && EditingVideo.bloom != enable)
		{
			EditingVideo.bloom = enable;
		}
	}

	public void ResetFilter()
	{
		if (IsEditing)
		{
			int player_Auto_Pickup = EditingFilter.Player_Auto_Pickup;
			EditingFilter = DataUtil.DeepClone(_defaultFilter);
			EditingFilter.Player_Auto_Pickup = player_Auto_Pickup;
			int sprite_Auto_Pickup = EditingFilter.Sprite_Auto_Pickup;
			EditingFilter = DataUtil.DeepClone(_defaultFilter);
			EditingFilter.Sprite_Auto_Pickup = sprite_Auto_Pickup;
			int sprite_Automatically_Salvages = EditingFilter.Sprite_Automatically_Salvages;
			EditingFilter = DataUtil.DeepClone(_defaultFilter);
			EditingFilter.Sprite_Automatically_Salvages = sprite_Automatically_Salvages;
		}
	}

	public void SaveFilter()
	{
		FilterDataContainer filterDataContainer = new FilterDataContainer();
		filterDataContainer.items.Clear();
		filterDataContainer.items.Add(Filter);
		FilesUtil.SaveLocalData(filterDataContainer);
	}

	public void ApplyFilterSettings()
	{
		SingletonMonoGlobal<FilterManager>.Instance.SetFilterPL((QulityType)Filter.Player_Auto_Pickup);
		SingletonMonoGlobal<FilterManager>.Instance.SetFilterXJL((QulityType)Filter.Sprite_Auto_Pickup);
		SingletonMonoGlobal<FilterManager>.Instance.SetFilterXJL_FJ((QulityType)Filter.Sprite_Automatically_Salvages);
	}

	public void SetFilterPL(QulityType qulity)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingFilter != null)
		{
			if (EditingFilter.Player_Auto_Pickup == (int)qulity)
			{
				return;
			}
			EditingFilter.Player_Auto_Pickup = (int)qulity;
		}
		if (Filter.Player_Auto_Pickup != (int)qulity)
		{
			Filter.Player_Auto_Pickup = (int)qulity;
			SingletonMonoGlobal<FilterManager>.Instance.SetFilterPL(qulity);
		}
		SaveFilter();
	}

	public void SetFilterXJL(QulityType qulity)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingFilter != null)
		{
			if (EditingFilter.Sprite_Auto_Pickup == (int)qulity)
			{
				return;
			}
			EditingFilter.Sprite_Auto_Pickup = (int)qulity;
		}
		if (Filter.Sprite_Auto_Pickup != (int)qulity)
		{
			Filter.Sprite_Auto_Pickup = (int)qulity;
			SingletonMonoGlobal<FilterManager>.Instance.SetFilterXJL(qulity);
		}
		SaveFilter();
	}

	public void SetFilterXJL_FJ(QulityType qulity)
	{
		if (!_inited)
		{
			Init();
		}
		if (IsEditing && EditingFilter != null)
		{
			if (EditingFilter.Sprite_Automatically_Salvages == (int)qulity)
			{
				return;
			}
			EditingFilter.Sprite_Automatically_Salvages = (int)qulity;
		}
		if (Filter.Sprite_Automatically_Salvages != (int)qulity)
		{
			Filter.Sprite_Automatically_Salvages = (int)qulity;
			SingletonMonoGlobal<FilterManager>.Instance.SetFilterXJL_FJ(qulity);
		}
		SaveFilter();
	}

	private static bool AreEqual<T>(T a, T b)
	{
		if ((object)a == (object)b)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		try
		{
			byte[] array = SerializationUtility.SerializeValue(a, DataFormat.Binary);
			byte[] array2 = SerializationUtility.SerializeValue(b, DataFormat.Binary);
			if (array.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
	}
}
