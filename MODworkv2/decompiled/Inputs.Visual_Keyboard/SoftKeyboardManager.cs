using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using UI.Panels;
using UnityEngine.UI;

namespace Inputs.Visual_Keyboard;

public class SoftKeyboardManager : Singleton<SoftKeyboardManager>
{
	private SoftKeyboardPanel currentPanel;

	public bool IsOpen
	{
		get
		{
			if ((bool)currentPanel)
			{
				return currentPanel.gameObject.activeSelf;
			}
			return false;
		}
	}

	private SoftKeyboardManager()
	{
	}

	public void Show(InputField inputField)
	{
		if ((bool)inputField)
		{
			if (!currentPanel)
			{
				currentPanel = Singleton<UIManager>.Instance.ShowPanel<SoftKeyboardPanel, InputField>(inputField);
			}
			else
			{
				Singleton<UIManager>.Instance.ShowPanel<SoftKeyboardPanel, InputField>(inputField);
			}
		}
	}

	public void Hide()
	{
		if (IsOpen)
		{
			Singleton<UIManager>.Instance.HidePanel<SoftKeyboardPanel>();
		}
	}
}
