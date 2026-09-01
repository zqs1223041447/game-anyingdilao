using System.Collections.Generic;
using Localization;
using Newtonsoft.Json;

public static class LocalizationJsonLoader
{
	public static Dictionary<string, Dictionary<LanguageType, string>> Load(string jsonText, string prefix)
	{
		Dictionary<string, Dictionary<string, string>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonText);
		Dictionary<string, Dictionary<LanguageType, string>> dictionary2 = new Dictionary<string, Dictionary<LanguageType, string>>();
		if (dictionary == null)
		{
			return dictionary2;
		}
		foreach (KeyValuePair<string, Dictionary<string, string>> item in dictionary)
		{
			string key = (string.IsNullOrEmpty(prefix) ? item.Key : (prefix + "." + item.Key));
			Dictionary<LanguageType, string> dictionary3 = new Dictionary<LanguageType, string>();
			foreach (KeyValuePair<string, string> item2 in item.Value)
			{
				if (LocalizationLanguageMap.TryGetLanguage(item2.Key, out var lang))
				{
					dictionary3[lang] = item2.Value;
				}
			}
			dictionary2[key] = dictionary3;
		}
		return dictionary2;
	}
}
