using System;
using System.Collections.Generic;
using Localization;

public static class LocalizationLanguageMap
{
	public static readonly Dictionary<string, LanguageType> ColumnToLanguage = new Dictionary<string, LanguageType>(StringComparer.Ordinal)
	{
		{
			"English",
			LanguageType.English
		},
		{
			"ChineseS",
			LanguageType.ChineseS
		},
		{
			"Russian",
			LanguageType.Russian
		},
		{
			"German",
			LanguageType.German
		},
		{
			"French",
			LanguageType.French
		},
		{
			"PortugueseBrazil",
			LanguageType.PortugueseBrazil
		},
		{
			"Polish",
			LanguageType.Polish
		},
		{
			"Korean",
			LanguageType.Korean
		},
		{
			"ChineseT",
			LanguageType.ChineseT
		},
		{
			"Spanish",
			LanguageType.SpanishSpain
		},
		{
			"SpanishSpain",
			LanguageType.SpanishSpain
		},
		{
			"Turkish",
			LanguageType.Turkish
		},
		{
			"Czech",
			LanguageType.Czech
		},
		{
			"Swedish",
			LanguageType.Swedish
		},
		{
			"Italian",
			LanguageType.Italian
		},
		{
			"SpanishLatinAmerica",
			LanguageType.SpanishLatinAmerica
		},
		{
			"Dutch",
			LanguageType.Dutch
		},
		{
			"Ukrainian",
			LanguageType.Ukrainian
		},
		{
			"Thai",
			LanguageType.Thai
		},
		{
			"Hungarian",
			LanguageType.Hungarian
		},
		{
			"PortuguesePortugal",
			LanguageType.PortuguesePortugal
		},
		{
			"Danish",
			LanguageType.Danish
		},
		{
			"Japanese",
			LanguageType.Japanese
		},
		{
			"Greek",
			LanguageType.Greek
		},
		{
			"Finnish",
			LanguageType.Finnish
		}
	};

	public const LanguageType FallbackLanguage = LanguageType.English;

	public static bool TryGetLanguage(string header, out LanguageType lang)
	{
		return ColumnToLanguage.TryGetValue(header, out lang);
	}
}
