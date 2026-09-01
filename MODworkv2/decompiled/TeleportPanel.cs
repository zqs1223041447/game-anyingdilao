using System.Collections.Generic;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.UI.Base;
using FinkFramework.Runtime.Utils;
using Inputs;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportPanel : GamepadSelectablePanel, IPanelParam<int>
{
	public GameObject levelGroup;

	[Header("章节Id")]
	public int ChapterId;

	private GameObject teleportItemPrefab;

	private CanvasGroup _canvasGroup;

	protected override void Awake()
	{
		base.Awake();
		InitPanel();
		_canvasGroup = GetComponent<CanvasGroup>();
		if (!_canvasGroup)
		{
			_canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
	}

	public void SetInteractable(bool interactable)
	{
		if ((bool)_canvasGroup)
		{
			_canvasGroup.interactable = interactable;
			_canvasGroup.blocksRaycasts = interactable;
		}
	}

	protected override void ClickBtn(string btnName)
	{
		if (btnName == "CloseBtn")
		{
			Time.timeScale = 1f;
			Singleton<UIManager>.Instance.HidePanel<TeleportPanel>();
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = true;
		}
		Time.timeScale = 1f;
	}

	public override bool OnCancel()
	{
		Time.timeScale = 1f;
		Singleton<UIManager>.Instance.HidePanel<TeleportPanel>();
		return true;
	}

	private void InitPanel()
	{
		if (!levelGroup)
		{
			levelGroup = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
		}
	}

	public override void OnShow()
	{
		RefreshUI();
		Button firstValidTeleportButton = GetFirstValidTeleportButton();
		SetFirstSelected(firstValidTeleportButton);
		if ((bool)firstValidTeleportButton)
		{
			GamepadUINavigationManager.RequestForceFocus(firstValidTeleportButton);
		}
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = false;
		}
	}

	private Button GetFirstValidTeleportButton()
	{
		Button[] componentsInChildren;
		if ((bool)levelGroup)
		{
			componentsInChildren = levelGroup.GetComponentsInChildren<Button>(includeInactive: true);
			foreach (Button button in componentsInChildren)
			{
				if (GamepadUINavigationManager.IsSelectableValidForGamepad(button))
				{
					return button;
				}
			}
		}
		componentsInChildren = GetComponentsInChildren<Button>(includeInactive: true);
		foreach (Button button2 in componentsInChildren)
		{
			if (GamepadUINavigationManager.IsSelectableValidForGamepad(button2))
			{
				return button2;
			}
		}
		return null;
	}

	private void RefreshUI()
	{
		BuildLevelItems();
	}

	private void BuildLevelItems()
	{
		if (!levelGroup || !SingletonMonoScope<LevelManager>.HasInstance)
		{
			return;
		}
		for (int num = levelGroup.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(levelGroup.transform.GetChild(num).gameObject);
		}
		if (!teleportItemPrefab)
		{
			teleportItemPrefab = Singleton<ResManager>.Instance.Load<GameObject>("res://UI/Components/Game/TeleportItem");
		}
		if (!teleportItemPrefab)
		{
			LogUtil.Error("TeleportItem 预制体加载失败：res://UI/Components/Game/TeleportItem");
			return;
		}
		string text = ((SceneManager.GetActiveScene().name == "HomeScene") ? "Home" : LevelManager.GetCurLevel());
		HashSet<string> hashSet = null;
		if (SaveManager.HasRuntime && SaveManager.RuntimeData.UnlockedLevelIds != null)
		{
			hashSet = SaveManager.RuntimeData.UnlockedLevelIds;
		}
		Object.Instantiate(teleportItemPrefab, levelGroup.transform, worldPositionStays: false).GetComponent<TeleportItem>().Init("Home", text == "Home");
		if (hashSet == null || hashSet.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>(hashSet);
		list.Sort();
		foreach (string item in list)
		{
			if (!(item == "Home") && LevelManager.GetChapterId(item) == ChapterId)
			{
				Object.Instantiate(teleportItemPrefab, levelGroup.transform, worldPositionStays: false).GetComponent<TeleportItem>().Init(item, item == text);
			}
		}
	}

	public void SetParam(int param)
	{
		ChapterId = param;
	}
}
