using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Data.SaveData;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Inputs;
using SaveSystem;
using UI.Panels;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : GamepadSelectablePanel
{
	private GameObject ChooseHeroGroup;

	private GameObject NewHeroGroup;

	private int startFlowVersion;

	private Button delete_btn;

	public GameObject deleteGroup;

	private Button deleteConfirm;

	private Button deleteCancel;

	private Button delete_global_btn;

	public GameObject deleteGlobalGroup;

	private Button deleteGlobalConfirm;

	private Button deleteGlobalCancel;

	private CustomLayoutGroup saveSlots;

	private SaveSlotItem currentSelected;

	private List<SaveSlotData> cachedSaveSlots;

	private bool saveSlotsCacheValid;

	private Button play_btn;

	private const float SaveSlotDoubleConfirmInterval = 0.35f;

	private SaveSlotItem lastConfirmCandidate;

	private float lastConfirmCandidateTime = -999f;

	private bool isStartingGame;

	private Button create_btn;

	private InputField nameInput;

	public Text typeTitle;

	public Text typeDescription;

	public GameObject typeSlot0;

	public GameObject typeSlot1;

	public GameObject typeSlot2;

	public GameObject typeSlot3;

	private PlayerTypeItem[] typeItems;

	private PlayerTypeItem currentSelectedType;

	private bool isCreatingSlot;

	protected override void Awake()
	{
		base.Awake();
		ChooseHeroGroup = base.transform.Find("StartBg/ChooseHeroGroup").gameObject;
		NewHeroGroup = base.transform.Find("StartBg/NewHeroGroup").gameObject;
		InitChooseGroup();
		InitCreateGroup();
	}

	public override void OnShow()
	{
		ShowStartFlowAsync().Forget();
	}

	private async UniTask ShowStartFlowAsync()
	{
		int version = ++startFlowVersion;
		await UniTask.Yield(PlayerLoopTiming.Update);
		if ((bool)this && base.gameObject.activeInHierarchy && version == startFlowVersion)
		{
			EnterStartFlow();
			RefreshStartFlowFocus();
		}
	}

	private void RefreshStartFlowFocus()
	{
		if (ChooseHeroGroup.activeSelf)
		{
			if ((bool)currentSelected)
			{
				Selectable component = currentSelected.GetComponent<Selectable>();
				if ((bool)component)
				{
					SetFirstSelected(component);
					return;
				}
			}
			SetFirstSelected(GetControl<Button>("ChooseHeroCreate"));
		}
		else if (NewHeroGroup.activeSelf)
		{
			SetFirstSelected(GetControl<Button>("CreateBtn"));
		}
	}

	protected override void ClickBtn(string btnName)
	{
		switch (btnName)
		{
		case "ChooseHeroBack":
			SetReturnSelected(GetControl<Button>("ChooseHeroBack"));
			Singleton<UIManager>.Instance.ShowExclusivePanel<MainPanel>();
			break;
		case "ChooseHeroCreate":
			NewHeroGroup.SetActive(value: true);
			SetFirstSelected(nameInput);
			SetReturnSelected(GetControl<Button>("ChooseHeroCreate"));
			ChooseHeroGroup.SetActive(value: false);
			RefreshUI();
			break;
		case "NewHeroBack":
		{
			List<SaveSlotData> list = LoadSaveSlotsForUI();
			if (list.Count == 0)
			{
				Singleton<UIManager>.Instance.ShowExclusivePanel<MainPanel>();
			}
			else
			{
				NewHeroGroup.SetActive(value: false);
				ChooseHeroGroup.SetActive(value: true);
				BuildSaveSlot(-1, list);
			}
			SetFirstSelected(GetControl<Button>("ChooseHeroCreate"));
			break;
		}
		}
	}

	public void RefreshUI()
	{
		if (ChooseHeroGroup.activeSelf)
		{
			RefreshChooseGroup();
		}
		if (NewHeroGroup.activeSelf)
		{
			RefreshCreateGroup();
		}
	}

	private void InitChooseGroup()
	{
		saveSlots = GetComponentInChildren<CustomLayoutGroup>();
		play_btn = GetControl<Button>("PlayGameBtn");
		delete_btn = GetControl<Button>("DeleteBtn");
		deleteConfirm = GetControl<Button>("DeleteConfirm");
		deleteCancel = GetControl<Button>("DeleteCancel");
		deleteConfirm.onClick.AddListener(delegate
		{
			if ((bool)deleteGroup && deleteGroup.activeSelf)
			{
				deleteGroup.SetActive(value: false);
				DeleteSelectedAndRefreshFocus();
			}
		});
		deleteCancel.onClick.AddListener(delegate
		{
			if ((bool)deleteGroup && deleteGroup.activeSelf)
			{
				CloseDeleteGroupAndRestoreFocus();
			}
		});
		delete_btn.onClick.AddListener(OnDeleteClicked);
		delete_btn.interactable = false;
		deleteGroup.SetActive(value: false);
		delete_global_btn = GetControl<Button>("DeleteGlobalBtn");
		deleteGlobalConfirm = GetControl<Button>("DeleteGlobalConfirm");
		deleteGlobalCancel = GetControl<Button>("DeleteGlobalCancel");
		deleteGlobalConfirm.onClick.AddListener(delegate
		{
			if ((bool)deleteGlobalGroup && deleteGlobalGroup.activeSelf)
			{
				deleteGlobalGroup.SetActive(value: false);
				DeleteGlobalAndRefreshFocus();
			}
		});
		deleteGlobalCancel.onClick.AddListener(delegate
		{
			if ((bool)deleteGlobalGroup && deleteGlobalGroup.activeSelf)
			{
				CloseDeleteGlobalGroupAndRestoreFocus();
			}
		});
		delete_global_btn.onClick.AddListener(OnDeleteGlobalClicked);
		deleteGlobalGroup.SetActive(value: false);
		play_btn.onClick.AddListener(OnPlayClicked);
		play_btn.interactable = false;
	}

	private void RefreshChooseGroup()
	{
		BuildSaveSlot(-1, LoadSaveSlotsForUI());
		deleteGroup.SetActive(value: false);
		deleteGlobalGroup.SetActive(value: false);
	}

	private List<SaveSlotData> LoadSaveSlotsForUI(bool forceRefresh = false)
	{
		if (!forceRefresh && saveSlotsCacheValid && cachedSaveSlots != null)
		{
			return cachedSaveSlots;
		}
		cachedSaveSlots = (from s in SaveManager.GetAllSaveSlotForUI()
			orderby s.SlotId
			select s).ToList();
		saveSlotsCacheValid = true;
		return cachedSaveSlots;
	}

	private void InvalidateSaveSlotCache()
	{
		saveSlotsCacheValid = false;
		cachedSaveSlots = null;
	}

	private void OnPlayClicked()
	{
		StartSelectedGame();
	}

	private void OnDeleteClicked()
	{
		if (!isStartingGame)
		{
			if (!currentSelected)
			{
				LogUtil.Error("未选择存档槽位 无法删除存档！");
			}
			else if ((bool)deleteGroup && !deleteGroup.activeSelf)
			{
				deleteGroup.SetActive(value: true);
				StartCoroutine(CoFocusDeleteConfirm());
			}
		}
	}

	private void OnDeleteGlobalClicked()
	{
		if (!isStartingGame && (bool)deleteGlobalGroup && !deleteGlobalGroup.activeSelf)
		{
			deleteGlobalGroup.SetActive(value: true);
			StartCoroutine(CoFocusDeleteGlobalConfirm());
		}
	}

	private IEnumerator CoFocusDeleteConfirm()
	{
		yield return null;
		SetFirstSelected(deleteConfirm);
	}

	private IEnumerator CoFocusDeleteGlobalConfirm()
	{
		yield return null;
		SetFirstSelected(deleteGlobalConfirm);
	}

	public void BuildSaveSlot(int forceSelectSlotId = -1, List<SaveSlotData> slots = null)
	{
		currentSelected = null;
		lastConfirmCandidate = null;
		lastConfirmCandidateTime = -999f;
		foreach (Transform item in saveSlots.transform)
		{
			Object.Destroy(item.gameObject);
		}
		GameObject original = Singleton<ResManager>.Instance.Load<GameObject>("res://UI/Components/Game/SaveSlot");
		int lastSlot = SaveManager.GetLastSlot();
		SaveSlotItem saveSlotItem = null;
		bool flag = false;
		slots = slots ?? LoadSaveSlotsForUI();
		foreach (SaveSlotData slot in slots)
		{
			SaveSlotItem component = Object.Instantiate(original, saveSlots.transform).GetComponent<SaveSlotItem>();
			component.SetData(slot);
			RegisterSlot(component);
			if (!saveSlotItem)
			{
				saveSlotItem = component;
			}
			if (forceSelectSlotId >= 0 && slot.SlotId == forceSelectSlotId)
			{
				SelectSlot(component);
				flag = true;
			}
			else if (!flag && slot.SlotId == lastSlot)
			{
				SelectSlot(component);
				flag = true;
			}
		}
		if (!flag && (bool)saveSlotItem)
		{
			SelectSlot(saveSlotItem);
		}
		if ((bool)currentSelected)
		{
			Selectable component2 = currentSelected.GetComponent<Selectable>();
			if ((bool)component2 && component2.IsInteractable() && component2.gameObject.activeInHierarchy)
			{
				SetFirstSelected(component2);
			}
		}
	}

	private void RegisterSlot(SaveSlotItem item)
	{
		item.OnClick = HandleSlotClicked;
		item.SetSelected(selected: false);
	}

	private void HandleSlotClicked(SaveSlotItem item)
	{
		if (!isStartingGame)
		{
			float unscaledTime = Time.unscaledTime;
			bool num = currentSelected == item && lastConfirmCandidate == item && unscaledTime - lastConfirmCandidateTime <= 0.35f;
			SelectSlot(item, moveToTop: false);
			lastConfirmCandidate = item;
			lastConfirmCandidateTime = unscaledTime;
			if (num)
			{
				StartSelectedGame();
			}
		}
	}

	private void StartSelectedGame()
	{
		if (!currentSelected)
		{
			LogUtil.Error("未选择存档槽位 无法开始游戏！");
			return;
		}
		lastConfirmCandidate = null;
		lastConfirmCandidateTime = -999f;
		StartSelectedGameAsync(currentSelected.SlotId).Forget();
	}

	private async UniTask StartSelectedGameAsync(int slotId)
	{
		if (isStartingGame)
		{
			return;
		}
		isStartingGame = true;
		SetChooseControlsInteractable(interactable: false);
		await UniTask.Yield(PlayerLoopTiming.Update);
		if (!this)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			isStartingGame = false;
			return;
		}
		await GameManager.StartGame(slotId);
		if ((bool)this && base.gameObject.activeInHierarchy)
		{
			isStartingGame = false;
			SetChooseControlsInteractable(interactable: true);
		}
	}

	private void SetChooseControlsInteractable(bool interactable)
	{
		if ((bool)play_btn)
		{
			play_btn.interactable = interactable && (bool)currentSelected;
		}
		if ((bool)delete_btn)
		{
			delete_btn.interactable = interactable && (bool)currentSelected;
		}
		if ((bool)delete_global_btn)
		{
			delete_global_btn.interactable = interactable;
		}
	}

	private void SelectSlot(SaveSlotItem item, bool moveToTop = true)
	{
		if (!(currentSelected == item))
		{
			if ((bool)currentSelected)
			{
				currentSelected.SetSelected(selected: false);
			}
			currentSelected = item;
			currentSelected.SetSelected(selected: true);
			if (moveToTop)
			{
				currentSelected.transform.SetSiblingIndex(0);
			}
			play_btn.interactable = true;
			delete_btn.interactable = true;
		}
	}

	private void EnterStartFlow()
	{
		List<SaveSlotData> list = LoadSaveSlotsForUI();
		if (list.Count == 0)
		{
			ChooseHeroGroup.SetActive(value: false);
			NewHeroGroup.SetActive(value: true);
			RefreshCreateGroup();
		}
		else
		{
			ChooseHeroGroup.SetActive(value: true);
			NewHeroGroup.SetActive(value: false);
			BuildSaveSlot(-1, list);
			deleteGroup.SetActive(value: false);
			deleteGlobalGroup.SetActive(value: false);
		}
	}

	public void InitCreateGroup()
	{
		create_btn = GetControl<Button>("CreateBtn");
		create_btn.onClick.AddListener(OnCreateBtnClicked);
		nameInput = GetControl<InputField>("NameInput");
		nameInput.onValueChanged.AddListener(delegate
		{
			RefreshCreateBtnState();
		});
		typeTitle = GetControl<Text>("typeName");
		typeDescription = GetControl<Text>("typeDescription");
		if (!typeSlot0 || !typeSlot1 || !typeSlot2 || !typeSlot3)
		{
			typeSlot0 = base.transform.Find("TypeSlot0").gameObject;
			typeSlot1 = base.transform.Find("TypeSlot1").gameObject;
			typeSlot2 = base.transform.Find("TypeSlot2").gameObject;
			typeSlot3 = base.transform.Find("TypeSlot3").gameObject;
		}
		typeItems = new PlayerTypeItem[4];
		typeItems[0] = typeSlot0.GetComponent<PlayerTypeItem>();
		typeItems[1] = typeSlot1.GetComponent<PlayerTypeItem>();
		typeItems[2] = typeSlot2.GetComponent<PlayerTypeItem>();
		typeItems[3] = typeSlot3.GetComponent<PlayerTypeItem>();
		for (int i = 0; i < typeItems.Length; i++)
		{
			typeItems[i].typeId = i;
			typeItems[i].OnClick = OnTypeSelected;
			typeItems[i].SetSelected(value: false);
		}
		ConfigureCreateGroupNavigation();
		currentSelectedType = null;
		create_btn.interactable = false;
	}

	private void ConfigureCreateGroupNavigation()
	{
		Button control = GetControl<Button>("NewHeroBack");
		Button button = (typeItems[0] ? typeItems[0].GetComponent<Button>() : null);
		Button button2 = (typeItems[1] ? typeItems[1].GetComponent<Button>() : null);
		Button button3 = (typeItems[2] ? typeItems[2].GetComponent<Button>() : null);
		Button button4 = (typeItems[3] ? typeItems[3].GetComponent<Button>() : null);
		SetExplicitNavigation(nameInput, control, create_btn, null, button);
		SetExplicitNavigation(create_btn, nameInput, control, null, button);
		SetExplicitNavigation(control, create_btn, nameInput, null, button);
		SetExplicitNavigation(button, null, null, nameInput, button2);
		SetExplicitNavigation(button2, null, null, button, button3);
		SetExplicitNavigation(button3, null, null, button2, button4);
		SetExplicitNavigation(button4, null, null, button3, null);
	}

	private static void SetExplicitNavigation(Selectable selectable, Selectable up, Selectable down, Selectable left, Selectable right)
	{
		if ((bool)selectable)
		{
			Navigation navigation = selectable.navigation;
			navigation.mode = Navigation.Mode.Explicit;
			navigation.selectOnUp = up;
			navigation.selectOnDown = down;
			navigation.selectOnLeft = left;
			navigation.selectOnRight = right;
			selectable.navigation = navigation;
		}
	}

	public void RefreshCreateGroup()
	{
		nameInput.text = string.Empty;
		currentSelectedType = null;
		PlayerTypeItem[] array = typeItems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetSelected(value: false);
		}
		PlayerTypeItem playerTypeItem = (currentSelectedType = typeItems[0]);
		currentSelectedType.SetSelected(value: true);
		typeTitle.text = LOC.MM.GetStart($"player_type{playerTypeItem.typeId}");
		typeDescription.text = LOC.MM.GetStart($"type_dec{playerTypeItem.typeId}");
		RefreshCreateBtnState();
	}

	private void OnTypeSelected(PlayerTypeItem item)
	{
		if (!(currentSelectedType == item))
		{
			if (currentSelectedType != null)
			{
				currentSelectedType.SetSelected(value: false);
			}
			currentSelectedType = item;
			currentSelectedType.SetSelected(value: true);
			typeTitle.text = LOC.MM.GetStart($"player_type{item.typeId}");
			typeDescription.text = LOC.MM.GetStart($"type_dec{item.typeId}");
			RefreshCreateBtnState();
		}
	}

	private void RefreshCreateBtnState()
	{
		bool flag = !string.IsNullOrWhiteSpace(nameInput.text);
		bool flag2 = currentSelectedType;
		create_btn.interactable = flag && flag2;
	}

	private void OnCreateBtnClicked()
	{
		if (isCreatingSlot || !currentSelectedType)
		{
			return;
		}
		string text = nameInput.text.Trim();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		isCreatingSlot = true;
		create_btn.interactable = false;
		try
		{
			if (!SaveManager.TryCreateNewSlot(text, currentSelectedType.typeId, out var newSlotId))
			{
				GameManager.ShowTip("创建存档失败，请重试", TipType.Fail);
				return;
			}
			SaveManager.SaveLastSlot(newSlotId);
			InvalidateSaveSlotCache();
			List<SaveSlotData> slots = LoadSaveSlotsForUI(forceRefresh: true);
			NewHeroGroup.SetActive(value: false);
			ChooseHeroGroup.SetActive(value: true);
			BuildSaveSlot(newSlotId, slots);
			deleteGroup.SetActive(value: false);
			deleteGlobalGroup.SetActive(value: false);
		}
		finally
		{
			isCreatingSlot = false;
			RefreshCreateBtnState();
		}
	}

	private void DeleteSelectedAndRefreshFocus()
	{
		if (!currentSelected)
		{
			return;
		}
		int slotId = currentSelected.SlotId;
		if (SaveManager.DeleteSaveSlot(slotId))
		{
			LogUtil.Success($"成功删除存档槽位: {slotId}");
		}
		else
		{
			LogUtil.Error($"删除存档槽位失败: {slotId}");
		}
		currentSelected = null;
		InvalidateSaveSlotCache();
		List<SaveSlotData> list = LoadSaveSlotsForUI(forceRefresh: true);
		if (list.Count == 0)
		{
			ChooseHeroGroup.SetActive(value: false);
			NewHeroGroup.SetActive(value: true);
			RefreshUI();
			SetFirstSelected(GetControl<Button>("NewHeroBack"));
			return;
		}
		BuildSaveSlot(-1, list);
		if ((bool)play_btn && play_btn.interactable)
		{
			SetFirstSelected(play_btn);
		}
		else if ((bool)delete_btn && delete_btn.interactable)
		{
			SetFirstSelected(delete_btn);
		}
	}

	private void CloseDeleteGroupAndRestoreFocus()
	{
		if ((bool)deleteGroup && deleteGroup.activeSelf)
		{
			deleteGroup.SetActive(value: false);
			if ((bool)delete_btn && delete_btn.gameObject.activeInHierarchy && delete_btn.interactable)
			{
				SetFirstSelected(delete_btn);
			}
		}
	}

	private static void DeleteGlobalAndRefreshFocus()
	{
		if (SaveManager.DeleteGlobalData())
		{
			GameManager.ShowTipLocalStartKey("delete_global_success", TipType.Success);
		}
		else
		{
			GameManager.ShowTipLocalStartKey("delete_global_fail", TipType.Fail);
		}
	}

	private void CloseDeleteGlobalGroupAndRestoreFocus()
	{
		if ((bool)deleteGlobalGroup && deleteGlobalGroup.activeSelf)
		{
			deleteGlobalGroup.SetActive(value: false);
			if ((bool)delete_global_btn && delete_global_btn.gameObject.activeInHierarchy && delete_global_btn.interactable)
			{
				SetFirstSelected(delete_global_btn);
			}
		}
	}

	public override bool OnCancel()
	{
		if ((bool)deleteGroup && deleteGroup.activeSelf)
		{
			CloseDeleteGroupAndRestoreFocus();
			return true;
		}
		if ((bool)deleteGlobalGroup && deleteGlobalGroup.activeSelf)
		{
			CloseDeleteGlobalGroupAndRestoreFocus();
			return true;
		}
		if ((bool)ChooseHeroGroup && ChooseHeroGroup.activeSelf)
		{
			Singleton<UIManager>.Instance.ShowExclusivePanel<MainPanel>();
			Selectable currentSelectable = GamepadUINavigationManager.GetCurrentSelectable();
			if ((bool)currentSelectable && currentSelectable.gameObject.TryGetComponent<SaveSlotItem>(out var _))
			{
				SetReturnSelected(GetControl<Button>("ChooseHeroBack"));
				return true;
			}
			SetReturnSelected(GamepadUINavigationManager.GetCurrentSelectable());
			return true;
		}
		if ((bool)NewHeroGroup && NewHeroGroup.activeSelf)
		{
			List<SaveSlotData> list = LoadSaveSlotsForUI();
			if (list.Count == 0)
			{
				Singleton<UIManager>.Instance.ShowExclusivePanel<MainPanel>();
			}
			else
			{
				NewHeroGroup.SetActive(value: false);
				ChooseHeroGroup.SetActive(value: true);
				BuildSaveSlot(-1, list);
			}
			SetReturnSelected(GamepadUINavigationManager.GetCurrentSelectable());
			SetFirstSelected(GetControl<Button>("ChooseHeroCreate"));
			return true;
		}
		return false;
	}
}
