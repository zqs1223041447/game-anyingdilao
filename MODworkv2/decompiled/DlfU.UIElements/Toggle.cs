using UnityEngine;
using UnityEngine.UIElements;

namespace DlfU.UIElements;

public class Toggle : UnityEngine.UIElements.Toggle
{
	public Toggle(string text, bool value, Justify contentJustify, EventCallback<ChangeEvent<bool>> callback)
	{
		base[0].StyleJustifyContent(contentJustify);
		base[0][0].StyleMargin(0f, 8f, 0f, 0f);
		base.text = text;
		this.value = value;
		this.RegisterValueChangedCallback(callback);
		this.StyleFont(value ? FontStyle.Italic : FontStyle.Normal);
		this.RegisterValueChangedCallback(delegate(ChangeEvent<bool> internalCallback)
		{
			this.StyleFont(internalCallback.newValue ? FontStyle.Italic : FontStyle.Normal);
		});
	}
}
