using Localization;
using UnityEngine;
using UnityEngine.UI;

internal class LocalizationTextBinder : MonoBehaviour
{
	private Text _text;

	private string _csv;

	private string _key;

	public void Init(Text text, string csv, string key)
	{
		_text = text;
		_csv = csv;
		_key = key;
		Refresh();
	}

	private void OnEnable()
	{
		LOC.MM.OnLanguageChanged += OnLanguageChanged;
	}

	private void OnDisable()
	{
		if (LOC.MM != null)
		{
			LOC.MM.OnLanguageChanged -= OnLanguageChanged;
		}
	}

	private void OnLanguageChanged(LanguageType lang)
	{
		Refresh();
	}

	private void Refresh()
	{
		if (!(_text == null))
		{
			_text.text = LOC.MM.Get(_csv, _key);
		}
	}
}
