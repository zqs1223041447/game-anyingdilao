using Localization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	[Header("Localization")]
	public LocalizationExcelList excel;

	public string key;

	private Text _text;

	private void Awake()
	{
		_text = GetComponent<Text>();
		Refresh();
	}

	private void OnEnable()
	{
		LOC.MM.OnLanguageChanged += OnLanguageChanged;
		Refresh();
	}

	private void OnDisable()
	{
		LOC.MM.OnLanguageChanged -= OnLanguageChanged;
	}

	private void OnLanguageChanged(LanguageType lang)
	{
		Refresh();
	}

	private void Refresh()
	{
		if (!(_text == null) && !string.IsNullOrEmpty(key))
		{
			string csvName = excel.ToString();
			_text.text = LOC.MM.Get(csvName, key);
		}
	}

	public void Set(LocalizationExcelList e, string k)
	{
		excel = e;
		key = k;
		Refresh();
	}
}
