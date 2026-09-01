using UnityEngine;
using UnityEngine.UIElements;

namespace DlfU.UIElements;

public class LabelImage : VisualElement
{
	public Image imageElement;

	public Label labelElement;

	public string Label
	{
		get
		{
			return labelElement.text;
		}
		set
		{
			labelElement.text = value;
		}
	}

	public LabelImage(string label, Texture2D image)
	{
		imageElement = new Image();
		imageElement.image = image;
		imageElement.StyleSize(16f, 16f);
		Add(imageElement);
		labelElement = new Label();
		labelElement.text = label;
		Add(labelElement);
	}
}
