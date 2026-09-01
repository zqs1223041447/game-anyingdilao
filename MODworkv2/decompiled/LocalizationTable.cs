using System.Collections.Generic;
using Localization;

public class LocalizationTable
{
	private readonly Dictionary<string, Dictionary<LanguageType, string>> _table = new Dictionary<string, Dictionary<LanguageType, string>>();

	public void Add(string key, Dictionary<LanguageType, string> values)
	{
		_table[key] = values;
	}

	public bool TryGet(string key, out Dictionary<LanguageType, string> values)
	{
		return _table.TryGetValue(key, out values);
	}

	public void Clear()
	{
		_table.Clear();
	}
}
