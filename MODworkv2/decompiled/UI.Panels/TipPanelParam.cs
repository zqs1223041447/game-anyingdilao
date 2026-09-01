using UnityEngine;

namespace UI.Panels;

public class TipPanelParam
{
	public readonly string Content;

	public readonly TipType Type;

	public readonly float StayTime;

	public readonly bool UseCustomTextColor;

	public readonly Color TextColor;

	public TipPanelParam(string content, TipType type = TipType.Normal, float stayTime = -1f, bool useCustomTextColor = false, Color textColor = default(Color))
	{
		Content = content;
		Type = type;
		StayTime = stayTime;
		UseCustomTextColor = useCustomTextColor;
		TextColor = textColor;
	}
}
