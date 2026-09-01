using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using UI.Panels;

namespace Scenes;

public static class SceneLoading
{
	private static bool loadingClosed;

	public static async UniTask CloseLoadingUIOnce()
	{
		if (!loadingClosed)
		{
			loadingClosed = true;
			LoadPanel loadPanel = await Singleton<UIManager>.Instance.GetPanelAsync<LoadPanel>();
			if ((bool)loadPanel)
			{
				await loadPanel.PlayFinishAndHold();
			}
			Singleton<UIManager>.Instance.HidePanelsInLayer(E_MainLayer.Middle);
		}
	}

	public static void Reset()
	{
		loadingClosed = false;
	}
}
