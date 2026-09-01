using UnityEngine.UIElements;

namespace DlfU.UIElements;

public class VerticalLayout : VisualElement
{
	public VerticalLayout()
	{
		base.name = "VerticalLayout";
		this.StyleFlexDirection(FlexDirection.Column);
		this.StyleFlexGrow(1f);
	}
}
