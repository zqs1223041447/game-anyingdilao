using Steamworks;
using UnityEngine;

namespace Localization;

public static class LanguageDetectUtil
{
	public static LanguageType GetDefaultGameLanguage()
	{
		if (SteamClient.IsValid)
		{
			string gameLanguage = SteamApps.GameLanguage;
			if (!string.IsNullOrEmpty(gameLanguage) && TryMapSteamLanguage(gameLanguage, out var language))
			{
				return language;
			}
			string steamUILanguage = SteamUtils.SteamUILanguage;
			if (!string.IsNullOrEmpty(steamUILanguage) && TryMapSteamLanguage(steamUILanguage, out var language2))
			{
				return language2;
			}
		}
		return MapSystemLanguage(Application.systemLanguage);
	}

	private static bool TryMapSteamLanguage(string steamLang, out LanguageType language)
	{
		switch (steamLang.ToLower())
		{
		case "english":
			language = LanguageType.English;
			return true;
		case "schinese":
			language = LanguageType.ChineseS;
			return true;
		case "russian":
			language = LanguageType.Russian;
			return true;
		case "german":
			language = LanguageType.German;
			return true;
		case "french":
			language = LanguageType.French;
			return true;
		case "brazilian":
			language = LanguageType.PortugueseBrazil;
			return true;
		case "polish":
			language = LanguageType.Polish;
			return true;
		case "koreana":
			language = LanguageType.Korean;
			return true;
		case "tchinese":
			language = LanguageType.ChineseT;
			return true;
		case "spanish":
			language = LanguageType.SpanishSpain;
			return true;
		case "turkish":
			language = LanguageType.Turkish;
			return true;
		case "czech":
			language = LanguageType.Czech;
			return true;
		case "swedish":
			language = LanguageType.Swedish;
			return true;
		case "italian":
			language = LanguageType.Italian;
			return true;
		case "latam":
			language = LanguageType.SpanishLatinAmerica;
			return true;
		case "dutch":
			language = LanguageType.Dutch;
			return true;
		case "ukrainian":
			language = LanguageType.Ukrainian;
			return true;
		case "thai":
			language = LanguageType.Thai;
			return true;
		case "hungarian":
			language = LanguageType.Hungarian;
			return true;
		case "portuguese":
			language = LanguageType.PortuguesePortugal;
			return true;
		case "danish":
			language = LanguageType.Danish;
			return true;
		case "japanese":
			language = LanguageType.Japanese;
			return true;
		case "greek":
			language = LanguageType.Greek;
			return true;
		case "finnish":
			language = LanguageType.Finnish;
			return true;
		default:
			language = LanguageType.English;
			return false;
		}
	}

	private static LanguageType MapSystemLanguage(SystemLanguage lang)
	{
		return lang switch
		{
			SystemLanguage.English => LanguageType.English, 
			SystemLanguage.ChineseSimplified => LanguageType.ChineseS, 
			SystemLanguage.ChineseTraditional => LanguageType.ChineseT, 
			SystemLanguage.Russian => LanguageType.Russian, 
			SystemLanguage.German => LanguageType.German, 
			SystemLanguage.French => LanguageType.French, 
			SystemLanguage.Portuguese => LanguageType.PortuguesePortugal, 
			SystemLanguage.Polish => LanguageType.Polish, 
			SystemLanguage.Korean => LanguageType.Korean, 
			SystemLanguage.Spanish => LanguageType.SpanishSpain, 
			SystemLanguage.Turkish => LanguageType.Turkish, 
			SystemLanguage.Czech => LanguageType.Czech, 
			SystemLanguage.Swedish => LanguageType.Swedish, 
			SystemLanguage.Italian => LanguageType.Italian, 
			SystemLanguage.Dutch => LanguageType.Dutch, 
			SystemLanguage.Ukrainian => LanguageType.Ukrainian, 
			SystemLanguage.Thai => LanguageType.Thai, 
			SystemLanguage.Hungarian => LanguageType.Hungarian, 
			SystemLanguage.Danish => LanguageType.Danish, 
			SystemLanguage.Japanese => LanguageType.Japanese, 
			SystemLanguage.Greek => LanguageType.Greek, 
			SystemLanguage.Finnish => LanguageType.Finnish, 
			_ => LanguageType.English, 
		};
	}
}
