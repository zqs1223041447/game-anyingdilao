using UnityEngine.UIElements;

namespace DlfU.UIElements;

public class HorizontalLayout : VisualElement
{
	public HorizontalLayout()
	{
		base.name = "HorizontalLayout";
		this.StyleFlexDirection(FlexDirection.Row);
		this.StyleFlexGrow(1f);
	}
}
