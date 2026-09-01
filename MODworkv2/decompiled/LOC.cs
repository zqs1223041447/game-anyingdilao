using System;
using System.Collections.Generic;
using System.IO;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Localization;
using UnityEngine;

public class LOC : Singleton<LOC>
{
	private bool _inited;

	private readonly Dictionary<string, Dictionary<LanguageType, string>> _table = new Dictionary<string, Dictionary<LanguageType, string>>();

	private readonly HashSet<string> _loadedJsons = new HashSet<string>();

	public static LOC MM => Singleton<LOC>.Instance;

	public bool IsReady { get; private set; }

	public LanguageType CurrentLanguage { get; private set; }

	public event Action<LanguageType> OnLanguageChanged;

	private LOC()
	{
	}

	public void Init()
	{
		if (!_inited)
		{
			CurrentLanguage = (LanguageType)Singleton<SettingDataManager>.Instance.Game.language;
			_table.Clear();
			_loadedJsons.Clear();
			LoadLocalizationJson("Start_FY");
			LoadLocalizationJson("Main_FY");
			LoadLocalizationJson("MainDisplay_FY");
			LoadLocalizationJson("Note_FY");
			LoadLocalizationJson("Buff_FY");
			_inited = true;
			IsReady = true;
			this.OnLanguageChanged?.Invoke(CurrentLanguage);
		}
	}

	public void SetLanguageSetting(LanguageType lang)
	{
		if (CurrentLanguage != lang)
		{
			CurrentLanguage = lang;
			this.OnLanguageChanged?.Invoke(lang);
		}
	}

	public void LoadLocalizationJson(string fileName)
	{
		if (_loadedJsons.Contains(fileName))
		{
			LogUtil.Warn("LOC", "Localization json 已加载，跳过: " + fileName);
			return;
		}
		TextAsset textAsset = Singleton<ResManager>.Instance.Load<TextAsset>("res://Localization/" + fileName);
		if (!textAsset)
		{
			LogUtil.Error("LOC", "Json 文件加载失败: " + fileName);
			return;
		}
		_loadedJsons.Add(fileName);
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		foreach (KeyValuePair<string, Dictionary<LanguageType, string>> item in LocalizationJsonLoader.Load(textAsset.text, fileNameWithoutExtension))
		{
			if (!_table.TryGetValue(item.Key, out var value))
			{
				_table[item.Key] = new Dictionary<LanguageType, string>(item.Value);
				continue;
			}
			foreach (KeyValuePair<LanguageType, string> item2 in item.Value)
			{
				string value2 = item2.Value;
				if (!string.IsNullOrEmpty(value2) && !(value2 == "0"))
				{
					value[item2.Key] = value2;
				}
			}
		}
	}

	public string Get(string csvName, string key)
	{
		EnsureLocalizationLoaded(csvName);
		return Get(ComposeKey(csvName, key));
	}

	public string Get(string key)
	{
		return Get(key, CurrentLanguage);
	}

	public string Get(string key, LanguageType lang)
	{
		if (string.IsNullOrEmpty(key))
		{
			return string.Empty;
		}
		if (!_table.TryGetValue(key, out var value))
		{
			LogUtil.Warn("LOC", $"无法找到翻译键: {key} (Language={lang})");
			return key;
		}
		if (TryGetValidText(value, lang, out var value2))
		{
			return value2;
		}
		if (lang != 0 && TryGetValidText(value, LanguageType.English, out value2))
		{
			return value2;
		}
		return key;
	}

	private static bool TryGetValidText(Dictionary<LanguageType, string> langDict, LanguageType lang, out string value)
	{
		if (langDict.TryGetValue(lang, out value) && !string.IsNullOrEmpty(value))
		{
			string text = value.Trim();
			if (text != "0" && !string.Equals(text, "a", StringComparison.OrdinalIgnoreCase))
			{
				value = text;
				return true;
			}
		}
		value = null;
		return false;
	}

	private static string ComposeKey(string csvName, string key)
	{
		if (!string.IsNullOrEmpty(csvName))
		{
			return csvName + "." + key;
		}
		return key;
	}

	private void EnsureLocalizationLoaded(string csvName)
	{
		if (!string.IsNullOrEmpty(csvName) && !_loadedJsons.Contains(csvName))
		{
			LoadLocalizationJson(csvName);
		}
	}

	public string GetLevelFormat(string key, params object[] args)
	{
		return string.Format(GetLevel(key), args);
	}

	public string GetDialogFormat(string key, params object[] args)
	{
		return string.Format(GetDialog(key), args);
	}

	public string GetMainFormat(string key, params object[] args)
	{
		return string.Format(GetMain(key), args);
	}

	public string GetMain(string key)
	{
		EnsureLocalizationLoaded("Main_FY");
		return Get("Main_FY", key);
	}

	public string GetStart(string key)
	{
		EnsureLocalizationLoaded("Start_FY");
		return Get("Start_FY", key);
	}

	public string GetSkill(string key)
	{
		EnsureLocalizationLoaded("Skill_FY");
		return Get("Skill_FY", key);
	}

	public string GetItem(string key)
	{
		EnsureLocalizationLoaded("Item_FY");
		return Get("Item_FY", key);
	}

	public string GetSPC(string key)
	{
		EnsureLocalizationLoaded("SPC_FY");
		return Get("SPC_FY", key);
	}

	public string GetEnemy(string key)
	{
		EnsureLocalizationLoaded("Enemy_FY");
		return Get("Enemy_FY", key);
	}

	public string GetLevel(string key)
	{
		EnsureLocalizationLoaded("Level_FY");
		return Get("Level_FY", key);
	}

	public string GetDialog(string key)
	{
		EnsureLocalizationLoaded("Dialog_FY");
		return Get("Dialog_FY", key);
	}

	public string GetNote(string key)
	{
		EnsureLocalizationLoaded("Note_FY");
		return Get("Note_FY", key);
	}

	public string GetBuff(string key)
	{
		EnsureLocalizationLoaded("Buff_FY");
		return Get("Buff_FY", key);
	}

	public static string GetLanguageDisplayName(LanguageType lang)
	{
		return lang switch
		{
			LanguageType.English => "English", 
			LanguageType.ChineseS => "简体中文", 
			LanguageType.ChineseT => "繁體中文", 
			LanguageType.Japanese => "日本語", 
			LanguageType.Korean => "한국어", 
			LanguageType.French => "Français", 
			LanguageType.German => "Deutsch", 
			LanguageType.Russian => "Русский", 
			LanguageType.Polish => "Polski", 
			LanguageType.Italian => "Italiano", 
			LanguageType.Turkish => "Türkçe", 
			LanguageType.Czech => "Čeština", 
			LanguageType.Ukrainian => "Українська", 
			LanguageType.Dutch => "Nederlands", 
			LanguageType.Swedish => "Svenska", 
			LanguageType.Hungarian => "Magyar", 
			LanguageType.Greek => "Ελληνικά", 
			LanguageType.Thai => "ไทย", 
			LanguageType.SpanishSpain => "Español (España)", 
			LanguageType.SpanishLatinAmerica => "Español (Latinoamérica)", 
			LanguageType.PortugueseBrazil => "Português (Brasil)", 
			LanguageType.PortuguesePortugal => "Português (Portugal)", 
			LanguageType.Danish => "Dansk", 
			LanguageType.Finnish => "Suomi", 
			_ => "English", 
		};
	}
}
