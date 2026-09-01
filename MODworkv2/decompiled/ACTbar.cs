using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.SaveData;
using Entity.Character.Player;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs.Gamepad;
using Lean.Pool;
using Localization;
using Scenes;
using UI.UIItems;
using UnityEngine;
using UnityEngine.UI;

public class ACTbar : ScopedSingletonMono<ACTbar>
{
	[HideInInspector]
	public ActbarSaveData SaveData;

	private ActbarSaveData pendingSaveData;

	private Coroutine restoreCoroutine;

	public ACT_skillBT[] skillBT;

	[HideInInspector]
	public int OpendSkillBT;

	[HideInInspector]
	public bool OpendSkillList;

	public ACT_UseBT[] useBT;

	[HideInInspector]
	public int OpendUseBT;

	[HideInInspector]
	public bool OpendUseList;

	[HideInInspector]
	public int UseListCount;

	[HideInInspector]
	public ACT_useData[] useDT;

	[HideInInspector]
	public ACT_DOT[] DOT;

	public readonly Dictionary<int, ACT_SPC> SK = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> HIT = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> DIE = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> HURT = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> GD = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> CPSK = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> CPHURT = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> CPDIE = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> CPUNIVERSE = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, SkillOBJ_DT_SP> ORB = new Dictionary<int, SkillOBJ_DT_SP>();

	[HideInInspector]
	public GameObject ATprefab;

	[HideInInspector]
	public int AT_Layer;

	public SkillOBJ_DT_SP ATprefabSP;

	public CanvasGroup ACTListSkillPanel;

	public GameObject ActListSkillContent;

	public GameObject actSkillSlot;

	public CanvasGroup ACTListUsePanel;

	public GameObject ACTListUseContent;

	public GameObject actUseSlot;

	[HideInInspector]
	public List<ACTListSkillBT> actListSkill = new List<ACTListSkillBT>();

	[HideInInspector]
	public List<ACTListUseBT> actListUse = new List<ACTListUseBT>();

	public ACT_XBT SkillXBT;

	public ACT_XBT UseXBT;

	[HideInInspector]
	public List<SK_BuffA> SkillBuffList = new List<SK_BuffA>();

	private float timeA;

	private float autoUseTimer;

	private float gdUseTimer;

	private float actHurtTimer;

	private float actGdTimer;

	private float actCpHurtTimer;

	private bool releasingLinkedSkills;

	private readonly HashSet<string> linkedSkillNames = new HashSet<string>();

	private readonly Collider2D[] autoUseEnemies = new Collider2D[12];

	private readonly List<int> autoAttackNoCooldownSkillIndexes = new List<int>(8);

	public readonly Dictionary<int, ACT_SPC> CPLINKSK = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> CPSAMESK = new Dictionary<int, ACT_SPC>();

	public readonly Dictionary<int, ACT_SPC> CPTRISK = new Dictionary<int, ACT_SPC>();

	private GameDataManager _gameDataManager;

	private PlayerManager PL;

	public KeyBindUI KeyBindUI;

	private ACT_Auto autoAttackButton;

	public GameObject buffList;

	private Transform talentGuideRoot;

	private Transform actSkillListGuideRoot;

	private Text talentGuideText;

	private Text actSkillListGuideText;

	private const string TalentGuideTextKey = "Guide_ObtainSkillPoint";

	private const string ActSkillListGuideTextKey = "Guide_AssignSkill";

	private const int DotElementCount = 6;

	public bool AutoReplaceUseBinding { get; private set; }

	public void BeginRestoreFromSaveData(ActbarSaveData data)
	{
		pendingSaveData = data;
		if (restoreCoroutine != null)
		{
			StopCoroutine(restoreCoroutine);
		}
		restoreCoroutine = StartCoroutine(CoBeginRestoreFromSaveData());
	}

	private IEnumerator CoBeginRestoreFromSaveData()
	{
		yield return null;
		yield return new WaitUntil(() => SingletonMonoScope<TalentManager>.HasInstance && SingletonMonoScope<TalentManager>.Instance.IsTalentDataReady && SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<InventoryManager>.Instance.IsInventoryDataReady);
		if (pendingSaveData != null)
		{
			InitFromSaveData(pendingSaveData);
			pendingSaveData = null;
		}
		restoreCoroutine = null;
	}

	public void InitFromSaveData(ActbarSaveData data)
	{
		SaveData = DataUtil.DeepClone(data);
		ApplySaveData(SaveData);
	}

	public ActbarSaveData ExportSaveData()
	{
		ActbarSaveData actbarSaveData = new ActbarSaveData
		{
			SkillSlots = new List<ActbarSkillSlotSaveData>(),
			UseSlots = new List<ActbarUseSlotSaveData>()
		};
		ACT_skillBT[] array = skillBT;
		foreach (ACT_skillBT aCT_skillBT in array)
		{
			actbarSaveData.SkillSlots.Add(new ActbarSkillSlotSaveData
			{
				Opened = aCT_skillBT.Opened,
				IndexName = aCT_skillBT.IndexName,
				Xi = aCT_skillBT.Xi,
				SkillType = aCT_skillBT.SkillType
			});
		}
		ACT_UseBT[] array2 = useBT;
		foreach (ACT_UseBT aCT_UseBT in array2)
		{
			actbarSaveData.UseSlots.Add(new ActbarUseSlotSaveData
			{
				Opend = aCT_UseBT.Opend,
				IndexName = aCT_UseBT.IndexName,
				Type = aCT_UseBT.Type
			});
		}
		return actbarSaveData;
	}

	public void ApplySaveData(ActbarSaveData data)
	{
		if (data == null)
		{
			data = ActbarSaveData.CreateDefault();
		}
		RestoreSaveData(data);
	}

	public void RestoreSaveData(ActbarSaveData data)
	{
		SetSkillSart();
		SetUseSart();
		if (data != null)
		{
			RestoreSkillSlots(data.SkillSlots);
			RebuildUseListFromInventory();
			RestoreUseSlots(data.UseSlots);
			RefreshUseBindingStack();
			TryAutoUpgradeUseBindings();
		}
	}

	private void RestoreSkillSlots(List<ActbarSkillSlotSaveData> slots)
	{
		if (slots == null)
		{
			return;
		}
		slots = MigrateSkillSlotsForEightSkillBar(slots);
		int num = Mathf.Min(skillBT.Length, slots.Count);
		for (int i = 0; i < num; i++)
		{
			ActbarSkillSlotSaveData actbarSkillSlotSaveData = slots[i];
			if (actbarSkillSlotSaveData == null || !actbarSkillSlotSaveData.Opened || string.IsNullOrEmpty(actbarSkillSlotSaveData.IndexName))
			{
				continue;
			}
			ACTListSkillBT aCTListSkillBT = null;
			foreach (ACTListSkillBT item in actListSkill)
			{
				if ((bool)item && item.IndexName == actbarSkillSlotSaveData.IndexName)
				{
					aCTListSkillBT = item;
					break;
				}
			}
			if ((bool)aCTListSkillBT)
			{
				skillBT[i].Opened = true;
				skillBT[i].Xi = actbarSkillSlotSaveData.Xi;
				skillBT[i].SkillType = actbarSkillSlotSaveData.SkillType;
				skillBT[i].IndexName = actbarSkillSlotSaveData.IndexName;
				skillBT[i].actL = aCTListSkillBT;
				skillBT[i].image.sprite = aCTListSkillBT.icon.sprite;
				skillBT[i].imageCD.fillAmount = 0f;
			}
		}
	}

	private List<ActbarSkillSlotSaveData> MigrateSkillSlotsForEightSkillBar(List<ActbarSkillSlotSaveData> slots)
	{
		if (skillBT == null || skillBT.Length < 8 || slots.Count != 7)
		{
			return slots;
		}
		List<ActbarSkillSlotSaveData> list = new List<ActbarSkillSlotSaveData>(slots);
		list.Insert(4, null);
		return list;
	}

	private void RestoreUseSlots(List<ActbarUseSlotSaveData> slots)
	{
		if (slots == null)
		{
			return;
		}
		int num = Mathf.Min(useBT.Length, slots.Count);
		for (int i = 0; i < num; i++)
		{
			ActbarUseSlotSaveData actbarUseSlotSaveData = slots[i];
			if (actbarUseSlotSaveData == null || !actbarUseSlotSaveData.Opend || string.IsNullOrEmpty(actbarUseSlotSaveData.IndexName))
			{
				continue;
			}
			SlotData slotData = SingletonMonoScope<InventoryManager>.Instance.ReturnSameUse(actbarUseSlotSaveData.IndexName);
			if (slotData?.useitem != null)
			{
				ACT_UseBT aCT_UseBT = useBT[i];
				aCT_UseBT.Opend = true;
				aCT_UseBT.IndexName = actbarUseSlotSaveData.IndexName;
				aCT_UseBT.Type = actbarUseSlotSaveData.Type;
				aCT_UseBT.image.sprite = slotData.useitem.Icon;
				aCT_UseBT.stackText.gameObject.SetActive(value: true);
				int useItemTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(actbarUseSlotSaveData.IndexName);
				aCT_UseBT.RefreshStack(useItemTotalCountInInv);
				BuffSimpleItem buffSimpleItem = ReturnSimplePotionItem(aCT_UseBT);
				if ((bool)buffSimpleItem)
				{
					aCT_UseBT.IsCD = true;
					aCT_UseBT.slot = buffSimpleItem;
				}
				else
				{
					aCT_UseBT.IsCD = false;
					aCT_UseBT.slot = null;
					aCT_UseBT.imageCD.fillAmount = 0f;
				}
			}
		}
	}

	public void RegisterATPrefab(GameObject prefab)
	{
		if ((bool)prefab)
		{
			ATprefab = prefab;
			AT_Layer = Mathf.Max(0, AT_Layer) + 1;
		}
	}

	public bool UnregisterATPrefab()
	{
		if (AT_Layer > 0)
		{
			AT_Layer--;
		}
		if (AT_Layer > 0)
		{
			return false;
		}
		AT_Layer = 0;
		ATprefab = null;
		return true;
	}

	public void ResetATPrefabState()
	{
		AT_Layer = 0;
		ATprefab = null;
	}

	protected override void Awake()
	{
		base.Awake();
		if (!ACTListSkillPanel)
		{
			ACTListSkillPanel = base.transform.parent.transform.Find("ACTListSkill").GetComponent<CanvasGroup>();
		}
		if (!ACTListUsePanel)
		{
			ACTListUsePanel = base.transform.parent.transform.Find("ACTListUse").GetComponent<CanvasGroup>();
		}
		if (!ActListSkillContent && (bool)ACTListSkillPanel)
		{
			ActListSkillContent = ACTListSkillPanel.transform.Find("Content").gameObject;
		}
		if (!ACTListUseContent && (bool)ACTListUsePanel)
		{
			ACTListUseContent = ACTListUsePanel.transform.Find("Content").gameObject;
		}
		if (!buffList)
		{
			buffList = base.transform.Find("BuffList").gameObject;
		}
		ATprefabSP = GetComponent<SkillOBJ_DT_SP>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		PL = SingletonMonoScope<PlayerManager>.Instance;
		if (!KeyBindUI)
		{
			KeyBindUI = base.transform.Find("ActionBar").GetComponent<KeyBindUI>();
		}
		autoAttackButton = GetComponentInChildren<ACT_Auto>(includeInactive: true);
		EnsureBeginnerGuideUI();
		EnsureDotRuntimeData();
	}

	public bool ToggleAutoAttackFromShortcut()
	{
		if (!autoAttackButton)
		{
			autoAttackButton = GetComponentInChildren<ACT_Auto>(includeInactive: true);
		}
		if (!autoAttackButton)
		{
			return false;
		}
		autoAttackButton.ToggleAutoAttackFromShortcut();
		return true;
	}

	private void OnEnable()
	{
		LOC.MM.OnLanguageChanged += OnLanguageChanged;
	}

	private void OnDisable()
	{
		LOC.MM.OnLanguageChanged -= OnLanguageChanged;
	}

	private void Start()
	{
		SetAutoReplaceUseBinding(Singleton<SettingDataManager>.Instance.GetGame().autoChangeUseToggle);
		timeA = 0f;
		AT_Layer = 0;
		SetStart();
		SetDotSart();
		SetSkillSart();
		SetUseSart();
		RebuildDotRuntimeDataFromTalent();
		RefreshBeginnerGuide();
	}

	private void OnLanguageChanged(LanguageType lang)
	{
		RefreshBeginnerGuideText();
	}

	private void EnsureBeginnerGuideUI()
	{
		if (!talentGuideRoot)
		{
			talentGuideRoot = base.transform.Find("Setting (2)/BT (8)/SanJiao");
		}
		if (!actSkillListGuideRoot)
		{
			actSkillListGuideRoot = base.transform.Find("ActionBar/LBT/SanJiao");
		}
		if (!talentGuideText && (bool)talentGuideRoot)
		{
			talentGuideText = talentGuideRoot.GetComponentInChildren<Text>(includeInactive: true);
		}
		if (!actSkillListGuideText && (bool)actSkillListGuideRoot)
		{
			actSkillListGuideText = actSkillListGuideRoot.GetComponentInChildren<Text>(includeInactive: true);
		}
	}

	public void RefreshBeginnerGuide()
	{
		EnsureBeginnerGuideUI();
		RefreshBeginnerGuideText();
		bool active = SingletonMonoScope<TalentManager>.HasInstance && !SingletonMonoScope<TalentManager>.Instance.HasOpenedTalentPanel;
		bool active2 = SingletonMonoScope<TalentManager>.HasInstance && SingletonMonoScope<TalentManager>.Instance.HasAddedAnySkillPoint && !SingletonMonoScope<TalentManager>.Instance.HasOpenedActSkillListAfterFirstSkillPoint;
		if ((bool)talentGuideRoot)
		{
			talentGuideRoot.gameObject.SetActive(active);
		}
		if ((bool)actSkillListGuideRoot)
		{
			actSkillListGuideRoot.gameObject.SetActive(active2);
		}
	}

	private void RefreshBeginnerGuideText()
	{
		EnsureBeginnerGuideUI();
		if (LOC.MM.IsReady)
		{
			if ((bool)talentGuideText)
			{
				talentGuideText.text = LOC.MM.GetMain("Guide_ObtainSkillPoint");
			}
			if ((bool)actSkillListGuideText)
			{
				actSkillListGuideText.text = LOC.MM.GetMain("Guide_AssignSkill");
			}
		}
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA >= 0.15f)
		{
			foreach (ACTListSkillBT item in actListSkill)
			{
				if (item.SkillType == 2)
				{
					item.RefreshData();
				}
			}
			timeA = 0f;
		}
		autoUseTimer += Time.deltaTime;
		if (autoUseTimer >= 0.1f)
		{
			TryAutoUseSkills();
			autoUseTimer = 0f;
		}
	}

	public void SetStart()
	{
		OpendSkillList = false;
		OpendUseList = false;
		for (int i = 0; i < skillBT.Length; i++)
		{
			skillBT[i].index = i;
		}
		for (int j = 0; j < useBT.Length; j++)
		{
			useBT[j].index = j;
		}
		UseListCount = 0;
	}

	public void SetAutoReplaceUseBinding(bool enable)
	{
		if (AutoReplaceUseBinding != enable)
		{
			AutoReplaceUseBinding = enable;
			PruneInvalidUseList();
			RefreshUseBindingStack();
			TryAutoUpgradeUseBindings();
		}
	}

	public bool GetAutoReplaceUseBinding()
	{
		return AutoReplaceUseBinding;
	}

	private void KeepUseBindingButSetZero(int index)
	{
		if (IsValidUseSlotIndex(index))
		{
			ACT_UseBT aCT_UseBT = useBT[index];
			if ((bool)aCT_UseBT && aCT_UseBT.Opend && !string.IsNullOrEmpty(aCT_UseBT.IndexName))
			{
				aCT_UseBT.stackSize = 0;
				aCT_UseBT.RefreshStack(0);
				aCT_UseBT.IsCD = false;
				aCT_UseBT.slot = null;
				aCT_UseBT.imageCD.fillAmount = 0f;
			}
		}
	}

	public void ShowACTListUse(int index, Transform trans)
	{
		if (!OpendUseList)
		{
			ACTListUsePanel.alpha = 1f;
			ACTListUsePanel.blocksRaycasts = true;
			OpendUseList = true;
			OpendUseBT = index;
			ACTListUsePanel.transform.position = new Vector3(trans.position.x, ACTListUsePanel.transform.position.y, trans.position.z);
		}
		else if (OpendUseBT == index)
		{
			ACTListUsePanel.alpha = 0f;
			ACTListUsePanel.blocksRaycasts = false;
			OpendUseList = false;
		}
		else
		{
			ACTListUsePanel.alpha = 1f;
			ACTListUsePanel.blocksRaycasts = true;
			OpendUseList = true;
			OpendUseBT = index;
			ACTListUsePanel.transform.position = new Vector3(trans.position.x, ACTListUsePanel.transform.position.y, trans.position.z);
		}
	}

	public void CloseUseListUI()
	{
		ACTListUsePanel.alpha = 0f;
		ACTListUsePanel.blocksRaycasts = false;
		OpendUseList = false;
	}

	public void ClearUseByIndex(int index)
	{
		if (IsValidUseSlotIndex(index))
		{
			ACT_UseBT aCT_UseBT = useBT[index];
			aCT_UseBT.Opend = false;
			aCT_UseBT.Type = null;
			aCT_UseBT.stackSize = 0;
			aCT_UseBT.IndexName = null;
			aCT_UseBT.IsCD = false;
			aCT_UseBT.slot = null;
			aCT_UseBT.image.sprite = aCT_UseBT.backGround;
			aCT_UseBT.imageCD.fillAmount = 0f;
			if ((bool)aCT_UseBT.stackText)
			{
				aCT_UseBT.stackText.text = string.Empty;
				aCT_UseBT.stackText.gameObject.SetActive(value: false);
			}
		}
	}

	public void RefreshUseBindingStack()
	{
		PruneInvalidUseList();
		for (int i = 0; i < useBT.Length; i++)
		{
			ACT_UseBT aCT_UseBT = useBT[i];
			if (!aCT_UseBT)
			{
				continue;
			}
			if (!aCT_UseBT.Opend || string.IsNullOrEmpty(aCT_UseBT.IndexName))
			{
				ClearUseByIndex(i);
				continue;
			}
			int useItemTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(aCT_UseBT.IndexName);
			if (useItemTotalCountInInv > 0)
			{
				aCT_UseBT.RefreshStack(useItemTotalCountInInv);
				BuffSimpleItem buffSimpleItem = ReturnSimplePotionItem(aCT_UseBT);
				if ((bool)buffSimpleItem)
				{
					aCT_UseBT.IsCD = true;
					aCT_UseBT.slot = buffSimpleItem;
				}
				else
				{
					aCT_UseBT.IsCD = false;
					aCT_UseBT.slot = null;
					aCT_UseBT.imageCD.fillAmount = 0f;
				}
			}
			else if (AutoReplaceUseBinding)
			{
				ExchangeUseFromActbar(i);
			}
			else
			{
				KeepUseBindingButSetZero(i);
			}
		}
	}

	public void ClearUse_Single()
	{
		useBT[OpendUseBT].Opend = false;
		useBT[OpendUseBT].Type = null;
		useBT[OpendUseBT].stackSize = 0;
		useBT[OpendUseBT].stackText.gameObject.SetActive(value: false);
		useBT[OpendUseBT].image.sprite = useBT[OpendUseBT].backGround;
		useBT[OpendUseBT].IndexName = null;
		useBT[OpendUseBT].imageCD.fillAmount = 0f;
		CloseUseListUI();
	}

	public static BuffSimpleItem ReturnSimplePotionItem(ACT_UseBT bt)
	{
		if (!bt)
		{
			return null;
		}
		if (!SingletonMonoScope<SimplePotionManager>.HasInstance)
		{
			return null;
		}
		if (SingletonMonoScope<SimplePotionManager>.Instance.SimpleList == null)
		{
			LogUtil.Error("ReturnSimplePotionItem 失败：SimpleList 为 null");
			return null;
		}
		foreach (BuffSimpleItem simple in SingletonMonoScope<SimplePotionManager>.Instance.SimpleList)
		{
			if (!simple)
			{
				LogUtil.Warn("ReturnSimplePotionItem：SimpleList 中存在 null 元素，已跳过");
			}
			else if (simple.UseType == bt.Type)
			{
				return simple;
			}
		}
		return null;
	}

	public void AddUseListSlot(UseItemClass use)
	{
		if (use == null || !CanJoinActbarUseList(use) || !CheckListUse(use.ItemName))
		{
			return;
		}
		int useItemTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(use.ItemName);
		if (useItemTotalCountInInv <= 0)
		{
			return;
		}
		GameObject obj = LeanPool.Spawn(actUseSlot, ACTListUseContent.transform);
		obj.transform.SetSiblingIndex(UseXBT.transform.GetSiblingIndex());
		ACTListUseBT componentInChildren = obj.GetComponentInChildren<ACTListUseBT>();
		componentInChildren.useType = use.UseType;
		componentInChildren.IndexName = use.ItemName;
		componentInChildren.icon.sprite = use.Icon;
		componentInChildren.stackSize = useItemTotalCountInInv;
		componentInChildren.RefreshStack(useItemTotalCountInInv);
		actListUse.Add(componentInChildren);
		UseListCount++;
		ACT_UseBT[] array = useBT;
		foreach (ACT_UseBT aCT_UseBT in array)
		{
			if (aCT_UseBT.IndexName == use.ItemName)
			{
				aCT_UseBT.stackSize = useItemTotalCountInInv;
				aCT_UseBT.RefreshStack(useItemTotalCountInInv);
			}
		}
	}

	public bool CheckListUse(string actName)
	{
		int num = 0;
		foreach (ACTListUseBT item in actListUse)
		{
			if (item.IndexName == actName)
			{
				num++;
			}
		}
		return num <= 0;
	}

	public void ClearListUse()
	{
		foreach (ACTListUseBT item in actListUse)
		{
			LeanPool.Despawn(item.transform.parent);
			UseListCount--;
		}
		actListUse.Clear();
		CloseUseListUI();
	}

	public void RebuildUseListFromInventory()
	{
		ClearListUse();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (MainSlotPage mainPage in SingletonMonoScope<InventoryManager>.Instance.MainPages)
		{
			foreach (SlotData main in mainPage.MainList)
			{
				if (main != null && main.isOC && main.useitem != null)
				{
					UseItemClass useitem = main.useitem;
					if (CanJoinActbarUseList(useitem) && hashSet.Add(useitem.ItemName))
					{
						AddUseListSlot(useitem);
					}
				}
			}
		}
	}

	public void RefreshUseListOne(UseItemClass use)
	{
		if (use == null || !CanJoinActbarUseList(use))
		{
			return;
		}
		int useItemTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(use.ItemName);
		ACTListUseBT sameUse = GetSameUse(use);
		if (useItemTotalCountInInv <= 0)
		{
			if ((bool)sameUse)
			{
				actListUse.Remove(sameUse);
				UseListCount--;
				LeanPool.Despawn(sameUse.transform.parent);
			}
			ACT_UseBT[] array = useBT;
			foreach (ACT_UseBT aCT_UseBT in array)
			{
				if (aCT_UseBT.IndexName == use.ItemName)
				{
					aCT_UseBT.stackSize = 0;
					aCT_UseBT.RefreshStack(0);
				}
			}
			return;
		}
		if ((bool)sameUse)
		{
			sameUse.stackSize = useItemTotalCountInInv;
			sameUse.RefreshStack(useItemTotalCountInInv);
			ACT_UseBT[] array = useBT;
			foreach (ACT_UseBT aCT_UseBT2 in array)
			{
				if (aCT_UseBT2.IndexName == use.ItemName)
				{
					aCT_UseBT2.stackSize = useItemTotalCountInInv;
					aCT_UseBT2.RefreshStack(useItemTotalCountInInv);
				}
			}
		}
		else
		{
			AddUseListSlot(use);
		}
		TryAutoUpgradeUseBindings();
	}

	public ACTListUseBT GetSameUse(UseItemClass use)
	{
		foreach (ACTListUseBT item in actListUse)
		{
			if (item.IndexName == use.ItemName)
			{
				return item;
			}
		}
		return null;
	}

	private static bool CanJoinActbarUseList(UseItemClass use)
	{
		if (use == null)
		{
			return false;
		}
		if (use.InfoType != 0)
		{
			return use.InfoType == 2;
		}
		return true;
	}

	public void SetUseSart()
	{
		ACT_useData[] array = useDT;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Opend = false;
		}
		ACT_UseBT[] array2 = useBT;
		foreach (ACT_UseBT aCT_UseBT in array2)
		{
			aCT_UseBT.Opend = false;
			aCT_UseBT.Type = null;
			aCT_UseBT.IndexName = null;
			aCT_UseBT.stackSize = 0;
			aCT_UseBT.IsCD = false;
			aCT_UseBT.slot = null;
			aCT_UseBT.imageCD.fillAmount = 0f;
			aCT_UseBT.image.sprite = aCT_UseBT.backGround;
			if ((bool)aCT_UseBT.stackText)
			{
				aCT_UseBT.stackText.gameObject.SetActive(value: false);
				aCT_UseBT.stackText.text = string.Empty;
			}
		}
	}

	private bool IsValidUseSlotIndex(int index)
	{
		if (index >= 0)
		{
			return index < useBT.Length;
		}
		return false;
	}

	public void SetUse(int stack, string type, string btnName, Sprite icon)
	{
		SetUseByIndex(OpendUseBT, stack, type, btnName, icon);
		CloseUseListUI();
	}

	public void SetUseByIndex(int index, int stack, string type, string btnName, Sprite icon)
	{
		if (IsValidUseSlotIndex(index))
		{
			ACT_UseBT aCT_UseBT = useBT[index];
			aCT_UseBT.Opend = true;
			aCT_UseBT.Type = type;
			aCT_UseBT.RefreshStack(stack);
			aCT_UseBT.IndexName = btnName;
			aCT_UseBT.image.sprite = icon;
			aCT_UseBT.stackText.gameObject.SetActive(value: true);
			BuffSimpleItem buffSimpleItem = ReturnSimplePotionItem(aCT_UseBT);
			if ((bool)buffSimpleItem)
			{
				aCT_UseBT.IsCD = true;
				aCT_UseBT.slot = buffSimpleItem;
			}
			else
			{
				aCT_UseBT.IsCD = false;
				aCT_UseBT.slot = null;
				aCT_UseBT.imageCD.fillAmount = 0f;
			}
		}
	}

	public void ExchangeUseFromActbar(int index)
	{
		if (!IsValidUseSlotIndex(index))
		{
			return;
		}
		string indexName = useBT[index].IndexName;
		string useItemUseType = GetUseItemUseType(indexName);
		if (string.IsNullOrEmpty(indexName) || string.IsNullOrEmpty(useItemUseType))
		{
			ClearUseByIndex(index);
			return;
		}
		ACTListUseBT bestReplacementUse = GetBestReplacementUse(useItemUseType, indexName);
		if (!bestReplacementUse)
		{
			ClearUseByIndex(index);
			return;
		}
		int useItemTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(bestReplacementUse.IndexName);
		if (useItemTotalCountInInv <= 0)
		{
			ClearUseByIndex(index);
		}
		else
		{
			SetUseByIndex(index, useItemTotalCountInInv, bestReplacementUse.useType, bestReplacementUse.IndexName, bestReplacementUse.icon.sprite);
		}
	}

	public void TryAutoUpgradeUseBindings()
	{
		if (!AutoReplaceUseBinding || !SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return;
		}
		PruneInvalidUseList();
		for (int i = 0; i < useBT.Length; i++)
		{
			ACT_UseBT aCT_UseBT = useBT[i];
			if (!aCT_UseBT || !aCT_UseBT.Opend || string.IsNullOrEmpty(aCT_UseBT.IndexName))
			{
				continue;
			}
			string useItemUseType = GetUseItemUseType(aCT_UseBT.IndexName);
			if (!IsAutoReplacePotionUseType(useItemUseType))
			{
				continue;
			}
			ACTListUseBT bestReplacementUse = GetBestReplacementUse(useItemUseType, null);
			if ((bool)bestReplacementUse && !string.IsNullOrEmpty(bestReplacementUse.IndexName) && !(bestReplacementUse.IndexName == aCT_UseBT.IndexName))
			{
				int useItemTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(bestReplacementUse.IndexName);
				if (useItemTotalCountInInv > 0)
				{
					SetUseByIndex(i, useItemTotalCountInInv, bestReplacementUse.useType, bestReplacementUse.IndexName, bestReplacementUse.icon.sprite);
				}
			}
		}
	}

	private ACTListUseBT GetBestReplacementUse(string targetUseType, string excludeIndexName)
	{
		if (actListUse == null || actListUse.Count == 0)
		{
			return null;
		}
		if (!SingletonMonoScope<ItemManager>.HasInstance || !SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return null;
		}
		if (string.IsNullOrEmpty(targetUseType))
		{
			return null;
		}
		ACTListUseBT result = null;
		UseItemClass useItemClass = null;
		foreach (ACTListUseBT item in actListUse)
		{
			if ((bool)item && !string.IsNullOrEmpty(item.IndexName) && (string.IsNullOrEmpty(excludeIndexName) || !(item.IndexName == excludeIndexName)) && SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(item.IndexName) > 0)
			{
				UseItemClass useItemData = GetUseItemData(item.IndexName);
				if (CanReplacePotionUseType(targetUseType, useItemData) && (useItemClass == null || IsBetterPotion(useItemData, useItemClass)))
				{
					result = item;
					useItemClass = useItemData;
				}
			}
		}
		return result;
	}

	private static bool CanReplacePotionUseType(string targetUseType, UseItemClass candidate)
	{
		if (candidate == null || string.IsNullOrEmpty(targetUseType))
		{
			return false;
		}
		if (!IsAutoReplacePotionUseType(targetUseType))
		{
			return candidate.UseType == targetUseType;
		}
		if (!IsAutoReplacePotionUseType(candidate.UseType))
		{
			return false;
		}
		return candidate.UseType == targetUseType;
	}

	private static bool IsAutoReplacePotionUseType(string useType)
	{
		if (!(useType == "health") && !(useType == "mana"))
		{
			return useType == "huoli";
		}
		return true;
	}

	private static bool IsBetterPotion(UseItemClass candidate, UseItemClass current)
	{
		if (candidate == null)
		{
			return false;
		}
		if (current == null)
		{
			return true;
		}
		bool flag = candidate.UseType == "huoli";
		bool flag2 = current.UseType == "huoli";
		if (flag != flag2)
		{
			return flag;
		}
		int potionRestoreRank = GetPotionRestoreRank(candidate);
		int potionRestoreRank2 = GetPotionRestoreRank(current);
		if (potionRestoreRank != potionRestoreRank2)
		{
			return potionRestoreRank > potionRestoreRank2;
		}
		if (potionRestoreRank >= 100 && potionRestoreRank2 >= 100 && !Mathf.Approximately(candidate.CDTime, current.CDTime))
		{
			return candidate.CDTime < current.CDTime;
		}
		return candidate.Level > current.Level;
	}

	private static int GetPotionRestoreRank(UseItemClass use)
	{
		return use?.Number ?? int.MinValue;
	}

	private static string GetUseItemUseType(string indexName)
	{
		if (!SingletonMonoScope<ItemManager>.HasInstance || string.IsNullOrEmpty(indexName))
		{
			return string.Empty;
		}
		foreach (UseItemClass item in SingletonMonoScope<ItemManager>.Instance.Potion)
		{
			if (indexName == item.ItemName)
			{
				return item.UseType;
			}
		}
		if (SingletonMonoScope<ItemManager>.Instance.Scroll.TryGetValue(indexName, out var value) && value != null)
		{
			return value.UseType;
		}
		return string.Empty;
	}

	private static UseItemClass GetUseItemData(string indexName)
	{
		if (!SingletonMonoScope<ItemManager>.HasInstance || string.IsNullOrEmpty(indexName))
		{
			return null;
		}
		foreach (UseItemClass item in SingletonMonoScope<ItemManager>.Instance.Potion)
		{
			if (indexName == item.ItemName)
			{
				return item;
			}
		}
		if (SingletonMonoScope<ItemManager>.Instance.Scroll.TryGetValue(indexName, out var value))
		{
			return value;
		}
		return null;
	}

	public void PruneInvalidUseList()
	{
		if (actListUse == null || actListUse.Count == 0)
		{
			return;
		}
		for (int num = actListUse.Count - 1; num >= 0; num--)
		{
			ACTListUseBT aCTListUseBT = actListUse[num];
			if (!aCTListUseBT || string.IsNullOrEmpty(aCTListUseBT.IndexName))
			{
				if ((bool)aCTListUseBT)
				{
					LeanPool.Despawn(aCTListUseBT.transform.parent);
				}
				actListUse.RemoveAt(num);
				UseListCount = Mathf.Max(0, UseListCount - 1);
			}
			else
			{
				int useItemTotalCountInInv = SingletonMonoScope<InventoryManager>.Instance.GetUseItemTotalCountInInv(aCTListUseBT.IndexName);
				if (useItemTotalCountInInv <= 0)
				{
					LeanPool.Despawn(aCTListUseBT.transform.parent);
					actListUse.RemoveAt(num);
					UseListCount = Mathf.Max(0, UseListCount - 1);
				}
				else
				{
					aCTListUseBT.RefreshStack(useItemTotalCountInInv);
				}
			}
		}
	}

	public bool HasSameSkillFX(string skillName)
	{
		foreach (SK_BuffA skillBuff in SkillBuffList)
		{
			if (skillBuff.sp.skillName == skillName)
			{
				return true;
			}
		}
		return false;
	}

	public void ClearSkillListArea()
	{
		if (actListSkill == null)
		{
			actListSkill = new List<ACTListSkillBT>();
		}
		if ((bool)ActListSkillContent)
		{
			Transform transform = ActListSkillContent.transform;
			for (int num = transform.childCount - 1; num >= 0; num--)
			{
				Transform child = transform.GetChild(num);
				if ((bool)child && (!SkillXBT || !(child == SkillXBT.transform)))
				{
					ACTListSkillBT componentInChildren = child.GetComponentInChildren<ACTListSkillBT>(includeInactive: true);
					if ((bool)componentInChildren)
					{
						componentInChildren.ClearCpList();
					}
					LeanPool.Despawn(child.gameObject);
				}
			}
		}
		else
		{
			for (int num2 = actListSkill.Count - 1; num2 >= 0; num2--)
			{
				ACTListSkillBT aCTListSkillBT = actListSkill[num2];
				if ((bool)aCTListSkillBT)
				{
					aCTListSkillBT.ClearCpList();
					LeanPool.Despawn((aCTListSkillBT.transform.parent ? aCTListSkillBT.transform.parent : aCTListSkillBT.transform).gameObject);
				}
			}
		}
		actListSkill.Clear();
		if ((bool)SkillXBT)
		{
			SkillXBT.transform.SetAsLastSibling();
		}
	}

	public void ClearSkillHotbarBindings()
	{
		if (skillBT == null)
		{
			return;
		}
		for (int i = 0; i < skillBT.Length; i++)
		{
			ACT_skillBT aCT_skillBT = skillBT[i];
			if ((bool)aCT_skillBT)
			{
				aCT_skillBT.Opened = false;
				aCT_skillBT.IsCD = false;
				aCT_skillBT.Xi = 0;
				aCT_skillBT.SkillType = 0;
				aCT_skillBT.IndexName = string.Empty;
				aCT_skillBT.actL = null;
				aCT_skillBT.ClearRuntimeState();
			}
		}
	}

	public void ClearBeforeRebuildSkillList()
	{
		ClearSkillHotbarBindings();
		ClearSkillListArea();
	}

	public void ShowACTListSkill(int index, Transform trans)
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.MarkActSkillListOpenedAfterFirstSkillPoint();
		}
		if (!OpendSkillList)
		{
			ACTListSkillPanel.alpha = 1f;
			ACTListSkillPanel.blocksRaycasts = true;
			OpendSkillList = true;
			OpendSkillBT = index;
			ACTListSkillPanel.transform.position = new Vector3(trans.position.x, ACTListSkillPanel.transform.position.y, trans.position.z);
		}
		else if (OpendSkillBT == index)
		{
			ACTListSkillPanel.alpha = 0f;
			ACTListSkillPanel.blocksRaycasts = false;
			OpendSkillList = false;
		}
		else
		{
			ACTListSkillPanel.alpha = 1f;
			ACTListSkillPanel.blocksRaycasts = true;
			OpendSkillList = true;
			OpendSkillBT = index;
			ACTListSkillPanel.transform.position = new Vector3(trans.position.x, ACTListSkillPanel.transform.position.y, trans.position.z);
		}
	}

	public void CloseSkillListUI()
	{
		ACTListSkillPanel.alpha = 0f;
		ACTListSkillPanel.blocksRaycasts = false;
		OpendSkillList = false;
	}

	public void AddSkillListSlotSP(int xi, int type, SkillData_Sample_Father data)
	{
		if (data != null && !string.IsNullOrEmpty(data.IndexName) && data.Level_Base > 0)
		{
			ACTListSkillBT aCTListSkillBT = CheckListSkill(data.IndexName);
			if ((bool)aCTListSkillBT)
			{
				SetSkill_Sample(aCTListSkillBT.DT, data);
				aCTListSkillBT.CDTime = data.CoolDown_Last - data.CoolDown_Last * PL.CoolDown_Max / 100f;
				return;
			}
			GameObject obj = LeanPool.Spawn(actSkillSlot, ActListSkillContent.transform);
			obj.transform.SetSiblingIndex(SkillXBT.transform.GetSiblingIndex());
			ACTListSkillBT componentInChildren = obj.GetComponentInChildren<ACTListSkillBT>();
			componentInChildren.ClearCpList();
			componentInChildren.IsCD = false;
			componentInChildren.JStimeA = 0f;
			componentInChildren.EmptyBT = false;
			componentInChildren.Xi = xi;
			componentInChildren.SkillType = type;
			componentInChildren.IndexName = data.IndexName;
			componentInChildren.icon.sprite = data.icon;
			SetSkill_Sample(componentInChildren.DT, data);
			actListSkill.Add(componentInChildren);
			componentInChildren.CDTime = data.CoolDown_Last - data.CoolDown_Last * PL.CoolDown_Max / 100f;
		}
	}

	public void AddSkillListSlotCP(int xi, int type, SkillData_Comp_Father data)
	{
		if (data != null && !string.IsNullOrEmpty(data.IndexName) && data.Level_Base > 0)
		{
			ACTListSkillBT aCTListSkillBT = CheckListSkill(data.IndexName);
			if ((bool)aCTListSkillBT)
			{
				SetSkill_Comp(aCTListSkillBT.DT, data);
				aCTListSkillBT.CDTime = data.CoolDown_Last - data.CoolDown_Last * PL.CoolDown_Max / 100f;
				return;
			}
			GameObject obj = LeanPool.Spawn(actSkillSlot, ActListSkillContent.transform);
			obj.transform.SetSiblingIndex(SkillXBT.transform.GetSiblingIndex());
			ACTListSkillBT componentInChildren = obj.GetComponentInChildren<ACTListSkillBT>();
			componentInChildren.ClearCpList();
			componentInChildren.IsCD = false;
			componentInChildren.JStimeA = 0f;
			componentInChildren.EmptyBT = false;
			componentInChildren.Xi = xi;
			componentInChildren.SkillType = type;
			componentInChildren.IndexName = data.IndexName;
			componentInChildren.icon.sprite = data.icon;
			SetSkill_Comp(componentInChildren.DT, data);
			actListSkill.Add(componentInChildren);
			componentInChildren.CDTime = data.CoolDown_Last - data.CoolDown_Last * PL.CoolDown_Max / 100f;
		}
	}

	public ACTListSkillBT CheckListSkill(string itemName)
	{
		if (string.IsNullOrEmpty(itemName) || actListSkill == null)
		{
			return null;
		}
		return actListSkill.FirstOrDefault((ACTListSkillBT item) => (bool)item && item.IndexName == itemName);
	}

	public void ClearListSkill()
	{
		int num;
		for (num = 0; num < actListSkill.Count; num++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[num];
			actListSkill[num].ClearCpList();
			actListSkill.Remove(aCTListSkillBT);
			LeanPool.Despawn(aCTListSkillBT.transform.parent);
			num--;
		}
		CloseSkillListUI();
	}

	public void SetSkill_Sample(ACT_skillData dt, SkillData_Sample_Father data)
	{
		dt.Opend = true;
		dt.type = 0;
		dt.SampleSkill = data.SampleSkill;
		dt.IndexName = data.IndexName;
		dt.UseAni = data.UseAni;
		dt.ManaCost = data.ManaCost_Last;
		dt.simple.FStype = data.FStype;
		dt.simple.LockType = data.LockType;
		dt.simple.OBJ = data.OBJ;
		dt.simple.OBJ_Group = data.OBJ_Group;
		dt.simple.RTtypeOBJ = data.RTtypeOBJ;
		dt.simple.RTtypeFX = data.RTtypeFX;
		dt.simple.Distance = data.Distance;
		dt.simple.CoolDown = data.CoolDown_Last - data.CoolDown_Last * PL.CoolDown_Max / 100f;
		dt.simple.damageType = data.damageType;
		dt.simple.MainEL = data.MainEL;
		dt.simple.ThroughType = data.ThroughType;
		dt.simple.AttackType = data.AttackType;
		dt.simple.AttackTypeA = data.SubAttackTypeA;
		dt.simple.AttackTypeB = data.SubAttackTypeB;
		dt.simple.Damage = data.Damage_Max;
		dt.simple.DamageA = data.Sub_DamageA;
		dt.simple.DamageB = data.Sub_DamageB;
		dt.simple.BJrate = data.BJrate_Last;
		dt.simple.BJDamage = data.BJDamage_Last;
		dt.simple.JYrate = data.JYrate_Last;
		dt.simple.Through = data.Through_Last;
		dt.simple.FlySpeed = data.FlySpeed_Last;
		dt.simple.MoveSpeedCut = data.MoveSpeedCut_Last;
		dt.simple.AttackSpeedCut = data.AttackSpeedCut_Last;
		dt.simple.AntiCut = data.AntiCut_Last;
		dt.simple.BF_Damage = data.BF_Damage_Last;
		dt.simple.BF_EL_Damage = data.BF_EL_Damage_Last;
		dt.simple.BF_EL_Chuan = data.BF_EL_Chuan_Last;
		dt.simple.BF_BJrate = data.BF_BJrate_Last;
		dt.simple.BF_JYrate = data.BF_JYrate_Last;
		dt.simple.BF_GeDang = data.BF_GeDang_Last;
		dt.simple.BF_AttackSpeed = data.BF_AttackSpeed_Last;
		dt.simple.BF_MoveSpeed = data.BF_MoveSpeed_Last;
		dt.simple.BF_DamageAnti = data.BF_DamageAnti_Last;
		dt.simple.BF_Health_Prc = data.BF_Health_Prc_Last;
		dt.simple.C_Damage = data.C_Damage_Last;
		dt.simple.C_ATspeed = data.C_ATspeed_Last;
		dt.simple.C_MVspeed = data.C_MVspeed_Last;
		dt.simple.C_Health_Prc = data.C_Health_Prc_Last;
		dt.simple.CF_Rate = data.CF_Rate;
		dt.simple.CF_Type = data.CF_Type;
		dt.simple.CF_Count = data.CF_Count_Last;
		dt.simple.Layer_SubA = data.Layer_SubA;
		dt.simple.Layer_SubB = data.Layer_SubB;
		dt.simple.BSAT = data.BSAT;
		dt.simple.BSAT_Count = data.BSAT_Count;
		dt.simple.BSAT_Angle = data.BSAT_Angle;
		dt.simple.Is_BS = data.Is_BS;
		dt.simple.ChangeSkin = data.ChangeSkin;
		dt.simple.SkinIndex = data.SkinIndex;
		dt.simple.Reborn = data.Reborn;
		dt.simple.BSAT_Damage = data.BSAT_Damage;
		SetSampleRuntimeSpecialData(dt.simple, data);
		dt.simple.TypeORB = data.TypeORB;
		dt.simple.Type_F = data.Type_F_Last;
		dt.simple.Type_S = data.Type_S;
		dt.simple.Type_AB = data.Type_AB;
		dt.simple.TypeDIC_F = data.TypeDIC_F_Last;
		dt.simple.TypeDIC_S = data.TypeDIC_S;
		dt.simple.TypeEXP_F = data.TypeEXP_F;
		dt.simple.TypeEXP_S = data.TypeEXP_S;
		dt.simple.TypeEXP_AB = data.TypeEXP_AB;
		dt.simple.Size = data.Size;
		dt.simple.High = data.High;
		dt.simple.JG = data.JG;
		dt.simple.AngleA = data.AngleA_Last;
		dt.simple.AngleB = data.AngleB;
		dt.simple.Range1 = data.Range1_Last;
		dt.simple.Range2 = data.Range2;
		dt.simple.Range_AT = data.Range_AT;
		dt.simple.FStime1 = data.FStime1_Last;
		dt.simple.FStime2 = data.FStime2_Last;
		dt.simple.Speed1 = data.Speed1;
		dt.simple.Speed2 = data.Speed2;
		dt.simple.Speed3 = data.Speed3;
		dt.simple.Speed4 = data.Speed4;
		dt.simple.Count_ORB = data.Count_ORB;
		dt.simple.Count_ATtarget = data.Count_ATtarget_Last;
		dt.simple.ATtar_DMG = data.ATtar_DMG;
		dt.simple.Count_F = data.Count_F_Last + PoeItemMod.ExtraProjectiles();
		dt.simple.Count_S = data.Count_S_Last;
		dt.simple.Count_AB = data.Count_AB_Last;
		dt.simple.CountMulti = data.CountMulti_Last;
		dt.simple.CountEXP = data.CountEXP;
		dt.simple.NoTime = data.NoTime;
		dt.simple.BuffTime = data.BuffTime_Last;
		dt.simple.DebuffTime = data.DebuffTime;
		dt.simple.Field_time = data.Field_time;
		dt.simple.ORB_time = data.ORB_time;
		dt.simple.EXP_time = data.EXP_time;
		dt.simple.ZD_time_F = data.ZD_time_F;
		dt.simple.ZD_time_S = data.ZD_time_S;
		dt.simple.ORB = data.ORB;
		dt.simple.ZD_F = data.ZD_F_Last;
		dt.simple.ZD_S = data.ZD_S_Last;
		dt.simple.ZD_AB = data.ZD_AB;
		dt.simple.EXP_F = data.EXP_F;
		dt.simple.EXP_S = data.EXP_S;
		dt.simple.EXP_AB = data.EXP_AB;
		dt.simple.Dic_F = data.Dic_F_Last;
		dt.simple.Dic_S = data.Dic_S;
		dt.simple.FX_F = data.FX_F;
		dt.simple.FX_S = data.FX_S;
		dt.simple.Sound = data.Sound;
		dt.simple.Follow_F = data.Follow_F;
		dt.simple.Follow_S = data.Follow_S;
		dt.simple.AllChuan_F = data.AllChuan_F;
		dt.simple.AllChuan_S = data.AllChuan_S;
		dt.simple.Slow_F = data.Slow_F;
		dt.simple.Slow_S = data.Slow_S;
		dt.simple.RDSpeed_F = data.RDSpeed_F;
		dt.simple.RDSpeed_S = data.RDSpeed_S;
		dt.simple.HasFX = data.HasFX;
		dt.simple.S_HasFX = data.S_HasFX;
		dt.simple.A_HasFX = data.A_HasFX;
		dt.simple.colEXP = data.colEXP;
		dt.simple.colEXP_A = data.colEXP_A;
		dt.simple.S_colEXP = data.S_colEXP;
		dt.simple.A_colEXP = data.A_colEXP;
		dt.simple.TimeEXP = data.TimeEXP;
		dt.simple.TimeEXP_A = data.TimeEXP_A;
		dt.simple.LastEXP = data.LastEXP;
		dt.simple.LastEXP_A = data.LastEXP_A;
		dt.simple.S_LastEXP = data.S_LastEXP;
		dt.simple.A_LastEXP = data.A_LastEXP;
		dt.simple.EXPpos = data.EXPpos;
		dt.simple.EXPpos_A = data.EXPpos_A;
		dt.simple.S_EXPpos = data.S_EXPpos;
		dt.simple.A_EXPpos = data.A_EXPpos;
		dt.simple.AngleEXP = data.AngleEXP;
		dt.simple.AngleEXP_A = data.AngleEXP_A;
	}

	public void SetSkill_Comp(ACT_skillData dt, SkillData_Comp_Father data)
	{
		dt.Opend = true;
		dt.type = 1;
		dt.SampleSkill = data.SampleSkill;
		dt.IndexName = data.IndexName;
		dt.UseAni = data.UseAni;
		dt.ManaCost = data.ManaCost_Last;
		dt.comp.Distance = data.Distance;
		dt.comp.OBJ = data.obj;
		dt.comp.CoolDown = data.CoolDown_Last - data.CoolDown_Last * PL.CoolDown_Max / 100f;
		dt.comp.Damage = data.Damage_Max;
		dt.comp.Health = data.Health_Last;
		dt.comp.Health_Prc = data.Health_Prc_Last;
		dt.comp.AttackSpeed = data.AttackSpeed_Last;
		dt.comp.GeDang = data.GeDang_Last;
		dt.comp.Summon_count = data.Summon_count_Last;
		dt.comp.AutoSummonOnReborn = data.AutoSummonOnReborn;
		dt.comp.BStype = data.BStype;
		dt.comp.AT_ZD = ((data.AT_ZD == 100000) ? data.BStype : data.AT_ZD);
		dt.comp.SK_ZD = ((data.SK_ZD == 100000) ? data.BStype : data.SK_ZD);
		dt.comp.AT_DMG = data.AT_DMG;
		dt.comp.SK_DMG = data.SK_DMG;
		dt.comp.damageType = data.damageType;
		dt.comp.damageType_Change = data.damageType_Change;
		dt.comp.Change_AT = data.Change_AT;
		dt.comp.ATSrate = data.ATSrate;
		dt.comp.ChangeEL_SK = data.ChangeEL_SK;
		dt.comp.ATS_Damage = data.ATS_Damage;
		dt.comp.ChangeEL_AR = data.ChangeEL_AR;
		dt.comp.ARS_Damage = data.ARS_Damage;
		dt.comp.DotMultiA = data.DotMultiA;
		dt.comp.DotMultiB = data.DotMultiB;
		dt.comp.GD_R_Heal = data.GD_R_Heal;
		dt.comp.BloodDie = data.BloodDie;
		dt.comp.TGYJ = data.TGYJ;
		dt.comp.AT_DotLayer = data.AT_DotLayer;
		dt.comp.BJ_NoDot = data.BJ_NoDot;
		dt.comp.WS_All = data.WS_All;
		dt.comp.Field_Range = data.Field_Range;
		dt.comp.Kill_R_Heal = data.Kill_R_Heal;
		dt.comp.Hurt_FT = data.Hurt_FT;
		dt.comp.EveryDMG = data.EveryDMG;
		dt.comp.EveryChuan = data.EveryChuan;
		dt.comp.EveryATS = data.EveryATS;
		dt.comp.EveryMVS = data.EveryMVS;
		dt.comp.EveryHeal = data.EveryHeal;
		dt.comp.EveryMana = data.EveryMana;
		dt.comp.EveryCD = data.EveryCD;
		dt.comp.EveryBJR = data.EveryBJR;
		dt.comp.EveryBJD = data.EveryBJD;
		dt.comp.EveryGD = data.EveryGD;
		dt.comp.EveryDMG_Anti = data.EveryDMG_Anti;
		dt.comp.EveryDotTimeCut = data.EveryDotTimeCut;
		dt.comp.EveryAllChuan = data.EveryAllChuan;
		dt.comp.EveryAllAnti = data.EveryAllAnti;
		dt.comp.EveryDrop = data.EveryDrop;
		dt.comp.EveryXJ_DMG = data.EveryXJ_DMG;
		dt.comp.EveryORB_DMG = data.EveryORB_DMG;
		dt.comp.EveryDot_DMG = data.EveryDot_DMG;
		dt.comp.DisA = data.DisA;
		dt.comp.DisB = data.DisB;
		dt.comp.Idle_Time_Min = data.Idle_Time_Min;
		dt.comp.Idle_Time_Max = data.Idle_Time_Max;
		dt.comp.SO_IdleRate = data.SO_IdleRate;
		dt.comp.SO_AttackRate = data.SO_AttackRate;
		dt.comp.SO_SayRate = data.SO_SayRate;
		dt.comp.SO_HurtRate = data.SO_HurtRate;
		dt.comp.SO_DieRate = data.SO_DieRate;
		dt.comp.SO_Idle = data.SO_Idle;
		dt.comp.SO_Walk = data.SO_Walk;
		dt.comp.SO_AttackA = data.SO_AttackA;
		dt.comp.SO_SayA = data.SO_SayA;
		dt.comp.SO_AttackB = data.SO_AttackB;
		dt.comp.SO_SayB = data.SO_SayB;
		dt.comp.SO_AttackC = data.SO_AttackC;
		dt.comp.SO_SayC = data.SO_SayC;
		dt.comp.SO_Hurt = data.SO_Hurt;
		dt.comp.SO_Die = data.SO_Die;
		dt.comp.Type_A = ((data.AT_FStype == 100000) ? data.Type_A : data.AT_FStype);
		dt.comp.Type_B = ((data.SK_FStype == 100000) ? data.Type_B : data.SK_FStype);
		dt.comp.TypeDIC_A = data.TypeDIC_A;
		dt.comp.TypeDIC_B = data.TypeDIC_B;
		dt.comp.JG_A = data.JG_A;
		dt.comp.JG_B = data.JG_B;
		dt.comp.AngleA = data.AngleA_Change_Last;
		dt.comp.AngleB = data.AngleB_Change_Last;
		dt.comp.FStimeA = data.FStimeA_Change_Last;
		dt.comp.FStimeB = data.FStimeB_Change_Last;
		dt.comp.Count_A = data.Count_A_Change_Last;
		dt.comp.Count_B = data.Count_B_Change_Last;
		dt.comp.AT_Double = data.AT_Double;
		dt.comp.Count_ATtarget_A = data.Count_ATtarget_A_Change_Last;
		dt.comp.Count_ATtarget_B = data.Count_ATtarget_B_Change_Last;
		dt.comp.CountMulti_A = data.CountMulti_A_Change_Last;
		dt.comp.CountMulti_B = data.CountMulti_B_Change_Last;
		dt.comp.Follow_A = data.Follow_A;
		dt.comp.Follow_B = data.Follow_B;
		dt.comp.AllChuan_A = data.AllChuan_A;
		dt.comp.AllChuan_B = data.AllChuan_B;
		dt.comp.RDSpeed_A = data.RDSpeed_A;
		dt.comp.RDSpeed_B = data.RDSpeed_B;
		dt.comp.HasFX_A = data.HasFX_A;
		dt.comp.HasFX_B = data.HasFX_B;
		dt.comp.colEXP_A = data.colEXP_A;
		dt.comp.colEXP_B = data.colEXP_B;
		dt.comp.EXPpos_A = data.EXPpos_A;
		dt.comp.EXPpos_B = data.EXPpos_B;
	}

	public void RefreshCD()
	{
		this.wait(0.0001f, RefreshStep);
	}

	public void RefreshStep()
	{
		if (actListSkill.Count <= 0)
		{
			return;
		}
		foreach (ACTListSkillBT item in actListSkill)
		{
			switch (item.SkillType)
			{
			case 0:
			{
				SingletonMonoScope<TalentManager>.Instance.XiData[item.Xi].Sample_F.TryGetValue(item.IndexName, out var value2);
				if (value2 != null)
				{
					SetSkill_Sample(item.DT, value2);
					item.CDTime = item.DT.simple.CoolDown;
				}
				break;
			}
			case 2:
			{
				SingletonMonoScope<TalentManager>.Instance.XiData[item.Xi].Comp_F.TryGetValue(item.IndexName, out var value);
				if (value != null)
				{
					SetSkill_Comp(item.DT, value);
					item.CDTime = item.DT.comp.CoolDown;
				}
				break;
			}
			}
		}
	}

	private static void SetSampleRuntimeSpecialData(ACT_skillSample simple, SkillData_Sample_Father data)
	{
		if (simple != null && data != null)
		{
			simple.BSAT_DMG = data.BSAT_DMG;
			simple.LockType = data.LockType;
			simple.AutoUse = data.AutoUse;
			simple.Refresh = data.Refresh;
			simple.CompUP_DMG = data.CompUP_DMG;
			simple.ATtarUP = data.ATtarUP;
			simple.MS_Dead = data.MS_Dead;
			simple.GD_Use = data.GD_Use;
			simple.JCskill = data.JCskill;
			simple.LinkSK = data.LinkSK;
			simple.LinkAll = data.LinkAll;
			simple.EveryLink = data.EveryLink;
			simple.LastSkill = data.LastSkill;
			simple.DashSkill = data.DashSkill;
			simple.TPSkill = data.TPSkill;
			simple.UseDMG = data.UseDMG;
			simple.UseATS = data.UseATS;
			simple.UseMVS = data.UseMVS;
			if (simple.UseDMG_EL == null || simple.UseDMG_EL.Length < 6)
			{
				simple.UseDMG_EL = new int[6];
			}
			if (simple.UseChuan == null || simple.UseChuan.Length < 6)
			{
				simple.UseChuan = new int[6];
			}
			simple.UseDMG_EL[0] = data.UseDMG_EL0;
			simple.UseDMG_EL[1] = data.UseDMG_EL1;
			simple.UseDMG_EL[2] = data.UseDMG_EL2;
			simple.UseDMG_EL[3] = data.UseDMG_EL3;
			simple.UseDMG_EL[4] = data.UseDMG_EL4;
			simple.UseDMG_EL[5] = data.UseDMG_EL5;
			simple.UseChuan[0] = data.UseChuan0;
			simple.UseChuan[1] = data.UseChuan1;
			simple.UseChuan[2] = data.UseChuan2;
			simple.UseChuan[3] = data.UseChuan3;
			simple.UseChuan[4] = data.UseChuan4;
			simple.UseChuan[5] = data.UseChuan5;
			simple.UseCP_DMG = data.UseCP_DMG;
			simple.UseCP_ATS = data.UseCP_ATS;
			simple.Has_DMG = data.Has_DMG;
			simple.Has_ATS = data.Has_ATS;
			simple.Has_MVS = data.Has_MVS;
			simple.Has_BJR = data.Has_BJR;
			simple.Has_BJD = data.Has_BJD;
			simple.Has_DotTimeCut = data.Has_DotTimeCut;
			simple.Has_DMG_Cut = data.Has_DMG_Cut;
			simple.Has_GD = data.Has_GD;
			simple.Has_ORB_DMG = data.Has_ORB_DMG;
			simple.Has_XJ_DMG = data.Has_XJ_DMG;
			simple.Has_Dot_DMG = data.Has_Dot_DMG;
			simple.Has_CP_DMG = data.Has_CP_DMG;
			simple.WD = data.WD;
			simple.Crit_Time = data.Crit_Time;
			simple.Crit_CD = data.Crit_CD;
			simple.Over_Prc = data.Over_Prc;
			simple.CutSpeedZone = data.CutSpeedZone;
			simple.Count_ATtarget = data.Count_ATtarget_Last;
			simple.Count_F = data.Count_F_Last + PoeItemMod.ExtraProjectiles();
			simple.Count_S = data.Count_S_Last;
			simple.Count_AB = data.Count_AB_Last;
			simple.CountMulti = data.CountMulti_Last;
		}
	}

	public void SetSkill(int xi, int type, ACTListSkillBT dt, Sprite icon)
	{
		skillBT[OpendSkillBT].Opened = true;
		skillBT[OpendSkillBT].Xi = xi;
		skillBT[OpendSkillBT].SkillType = type;
		skillBT[OpendSkillBT].image.sprite = icon;
		skillBT[OpendSkillBT].actL = dt;
		skillBT[OpendSkillBT].IndexName = dt.DT.IndexName;
		CloseSkillListUI();
	}

	public void ClearSK_Single()
	{
		skillBT[OpendSkillBT].Opened = false;
		skillBT[OpendSkillBT].Xi = 0;
		skillBT[OpendSkillBT].SkillType = 0;
		skillBT[OpendSkillBT].image.sprite = skillBT[OpendSkillBT].backGround;
		skillBT[OpendSkillBT].actL = null;
		skillBT[OpendSkillBT].IndexName = null;
		skillBT[OpendSkillBT].ClearRuntimeState();
		CloseSkillListUI();
	}

	public void SetSkillSart()
	{
		ACT_skillBT[] array = skillBT;
		foreach (ACT_skillBT aCT_skillBT in array)
		{
			aCT_skillBT.actL = null;
			aCT_skillBT.Opened = false;
			aCT_skillBT.image.sprite = aCT_skillBT.backGround;
			aCT_skillBT.ClearRuntimeState();
		}
	}

	public bool TryReleaseSkillDirect(ACTListSkillBT skill, bool useCooldown, bool spendMana)
	{
		if (!skill || skill.DT == null || !PL || !PL.IsAlive || !SingletonMonoScope<Gun>.HasInstance)
		{
			return false;
		}
		if (useCooldown && skill.IsCD)
		{
			return false;
		}
		if (spendMana && PL.ManaStat.Cur < skill.DT.ManaCost)
		{
			return false;
		}
		if (useCooldown)
		{
			skill.IsCD = true;
		}
		if (spendMana)
		{
			ApplySkillDirectCost(skill.DT);
		}
		PL.BuffRuntime?.OnSkillUsed(skill);
		SingletonMonoScope<Gun>.Instance.CastDirect(skill);
		return true;
	}

	public void TryRefreshSkillCooldown(ACTListSkillBT skill)
	{
		if ((bool)skill && skill.IsCD && skill.DT != null && skill.DT.type == 0)
		{
			int refresh = skill.DT.simple.Refresh;
			if (refresh > 0 && UnityEngine.Random.value < (float)refresh * 0.01f)
			{
				skill.ResetCD();
			}
		}
	}

	public float GetJCSkillDamage(ACTListSkillBT skill)
	{
		if (!skill || skill.DT == null || skill.DT.type != 0 || skill.DT.simple == null)
		{
			return 0f;
		}
		string jCskill = skill.DT.simple.JCskill;
		if (string.IsNullOrWhiteSpace(jCskill) || actListSkill == null || actListSkill.Count == 0)
		{
			return 0f;
		}
		jCskill = jCskill.Trim();
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if ((bool)aCTListSkillBT && !(aCTListSkillBT == skill) && aCTListSkillBT.DT != null && aCTListSkillBT.DT.type == 0 && aCTListSkillBT.DT.simple != null && (aCTListSkillBT.IndexName == jCskill || aCTListSkillBT.DT.IndexName == jCskill))
			{
				return aCTListSkillBT.DT.simple.Damage;
			}
		}
		return 0f;
	}

	public void TryReleaseLinkedSkills(ACTListSkillBT source)
	{
		if (!source || source.DT == null || source.DT.type != 0 || source.DT.simple == null || actListSkill == null || actListSkill.Count == 0)
		{
			return;
		}
		bool flag = !releasingLinkedSkills;
		if (flag)
		{
			linkedSkillNames.Clear();
			releasingLinkedSkills = true;
		}
		try
		{
			MarkLinkedSkill(source);
			ReleaseLinkSK(source);
			ReleaseLinkAll(source);
			ReleaseEveryLink(source);
		}
		finally
		{
			if (flag)
			{
				linkedSkillNames.Clear();
				releasingLinkedSkills = false;
			}
		}
	}

	private void ReleaseLinkSK(ACTListSkillBT source)
	{
		string[] linkSK = source.DT.simple.LinkSK;
		if (linkSK != null && linkSK.Length != 0)
		{
			for (int i = 0; i < linkSK.Length; i++)
			{
				ACTListSkillBT target = FindLinkableSkillByName(linkSK[i]);
				ReleaseLinkedSkill(target, source, useCooldown: false, spendMana: false);
			}
		}
	}

	private void ReleaseLinkAll(ACTListSkillBT source)
	{
		if (!source.DT.simple.LinkAll)
		{
			return;
		}
		List<ACTListSkillBT> list = new List<ACTListSkillBT>();
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if (CanTryLinkAllSkill(aCTListSkillBT, source))
			{
				list.Add(aCTListSkillBT);
			}
		}
		list.Sort((ACTListSkillBT a, ACTListSkillBT b) => b.DT.ManaCost.CompareTo(a.DT.ManaCost));
		while (list.Count > 0)
		{
			int highestManaAffordableLinkAllIndex = GetHighestManaAffordableLinkAllIndex(list, source);
			if (highestManaAffordableLinkAllIndex >= 0)
			{
				ACTListSkillBT target = list[highestManaAffordableLinkAllIndex];
				list.RemoveAt(highestManaAffordableLinkAllIndex);
				ReleaseLinkedSkill(target, source, useCooldown: true, spendMana: true);
				continue;
			}
			break;
		}
	}

	private void ReleaseEveryLink(ACTListSkillBT source)
	{
		if (source.DT.SampleSkill)
		{
			return;
		}
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if ((bool)aCTListSkillBT && !(aCTListSkillBT == source) && aCTListSkillBT.DT != null && aCTListSkillBT.DT.type == 0 && aCTListSkillBT.DT.simple != null && aCTListSkillBT.DT.simple.EveryLink)
			{
				ReleaseLinkedSkill(aCTListSkillBT, source, useCooldown: false, spendMana: false);
			}
		}
	}

	private bool ReleaseLinkedSkill(ACTListSkillBT target, ACTListSkillBT source, bool useCooldown, bool spendMana)
	{
		if (!IsLinkableSkillTarget(target, source))
		{
			return false;
		}
		if (IsLinkedSkillMarked(target))
		{
			return false;
		}
		MarkLinkedSkill(target);
		return TryReleaseSkillDirect(target, useCooldown, spendMana);
	}

	private int GetHighestManaAffordableLinkAllIndex(List<ACTListSkillBT> candidates, ACTListSkillBT source)
	{
		for (int i = 0; i < candidates.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = candidates[i];
			if (!CanTryLinkAllSkill(aCTListSkillBT, source))
			{
				candidates.RemoveAt(i);
				i--;
			}
			else if (PL.ManaStat.Cur >= aCTListSkillBT.DT.ManaCost)
			{
				return i;
			}
		}
		return -1;
	}

	private bool CanTryLinkAllSkill(ACTListSkillBT target, ACTListSkillBT source)
	{
		if ((bool)target && target != source && target.Xi == source.Xi && !target.IsCD && target.DT != null && target.DT.type == 0 && target.DT.simple != null)
		{
			return !IsLinkedSkillMarked(target);
		}
		return false;
	}

	public ACTListSkillBT FindSampleSkillByName(string skillName)
	{
		if (string.IsNullOrWhiteSpace(skillName))
		{
			return null;
		}
		string text = skillName.Trim();
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if ((bool)aCTListSkillBT && aCTListSkillBT.DT != null && aCTListSkillBT.DT.type == 0 && aCTListSkillBT.DT.simple != null && (aCTListSkillBT.IndexName == text || aCTListSkillBT.DT.IndexName == text))
			{
				return aCTListSkillBT;
			}
		}
		return null;
	}

	private void MarkLinkedSkill(ACTListSkillBT skill)
	{
		string linkedSkillKey = GetLinkedSkillKey(skill);
		if (!string.IsNullOrEmpty(linkedSkillKey))
		{
			linkedSkillNames.Add(linkedSkillKey);
		}
	}

	private bool IsLinkedSkillMarked(ACTListSkillBT skill)
	{
		string linkedSkillKey = GetLinkedSkillKey(skill);
		if (!string.IsNullOrEmpty(linkedSkillKey))
		{
			return linkedSkillNames.Contains(linkedSkillKey);
		}
		return false;
	}

	private static string GetLinkedSkillKey(ACTListSkillBT skill)
	{
		if (!skill)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(skill.IndexName))
		{
			return skill.IndexName;
		}
		if (skill.DT == null)
		{
			return null;
		}
		return skill.DT.IndexName;
	}

	private ACTListSkillBT FindLinkableSkillByName(string skillName)
	{
		if (string.IsNullOrWhiteSpace(skillName))
		{
			return null;
		}
		string text = skillName.Trim();
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if (IsLinkableSkillTarget(aCTListSkillBT, null) && (aCTListSkillBT.IndexName == text || aCTListSkillBT.DT.IndexName == text))
			{
				return aCTListSkillBT;
			}
		}
		return null;
	}

	private static bool IsLinkableSkillTarget(ACTListSkillBT target, ACTListSkillBT source)
	{
		if (!target || target == source || target.DT == null)
		{
			return false;
		}
		if (target.DT.type != 0 || target.DT.simple == null)
		{
			if (target.DT.type == 1)
			{
				return target.DT.comp != null;
			}
			return false;
		}
		return true;
	}

	private void ApplySkillDirectCost(ACT_skillData dt)
	{
		PL.ApplySkillCastCost(dt);
	}

	private void TryAutoUseSkills()
	{
		if (!PL || !PL.IsAlive)
		{
			return;
		}
		bool flag = HasEnemyNearPlayer(5f);
		if (PL.AutoAttackEnabled && flag)
		{
			TryAutoAttackFromActbar();
		}
		if (!PL.IsBattle || actListSkill == null || actListSkill.Count == 0)
		{
			return;
		}
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if (!aCTListSkillBT || aCTListSkillBT.DT == null || aCTListSkillBT.DT.SampleSkill)
			{
				continue;
			}
			if (aCTListSkillBT.DT.type == 0)
			{
				ACT_skillSample simple = aCTListSkillBT.DT.simple;
				if (!flag || simple == null || !simple.AutoUse)
				{
					continue;
				}
			}
			else
			{
				if (aCTListSkillBT.DT.type != 1)
				{
					continue;
				}
				ACT_skillComp comp = aCTListSkillBT.DT.comp;
				if (comp == null || !comp.AutoSummonOnReborn || comp.Summon_count <= 0 || GetAliveCompCount(aCTListSkillBT) >= comp.Summon_count)
				{
					continue;
				}
			}
			TryReleaseSkillDirect(aCTListSkillBT, useCooldown: true, spendMana: true);
		}
	}

	private void TryAutoAttackFromActbar()
	{
		if (skillBT == null || PL.IsSkill || PL.IsAttack)
		{
			return;
		}
		autoAttackNoCooldownSkillIndexes.Clear();
		int num = Mathf.Min(skillBT.Length, 8);
		for (int i = 0; i < num; i++)
		{
			ACTListSkillBT autoAttackSkill = GetAutoAttackSkill(i);
			if (!autoAttackSkill)
			{
				continue;
			}
			if (HasCooldown(autoAttackSkill))
			{
				if (TryReleaseSkillWithAnimation(i))
				{
					return;
				}
			}
			else if (autoAttackSkill.DT.SampleSkill)
			{
				autoAttackNoCooldownSkillIndexes.Add(i);
			}
		}
		for (int j = 0; j < autoAttackNoCooldownSkillIndexes.Count && !TryReleaseSkillWithAnimation(autoAttackNoCooldownSkillIndexes[j]); j++)
		{
		}
	}

	private ACTListSkillBT GetAutoAttackSkill(int index)
	{
		if (skillBT == null || index < 0 || index >= skillBT.Length)
		{
			return null;
		}
		ACT_skillBT aCT_skillBT = skillBT[index];
		if (!aCT_skillBT || !aCT_skillBT.Opened || !aCT_skillBT.actL)
		{
			return null;
		}
		ACTListSkillBT actL = aCT_skillBT.actL;
		if (!actL || actL.DT == null || actL.DT.type != 0 || actL.DT.simple == null)
		{
			return null;
		}
		ACT_skillSample simple = actL.DT.simple;
		if (simple.DashSkill || simple.TPSkill)
		{
			return null;
		}
		if (actL.IsCD || PL.ManaStat.Cur < actL.DT.ManaCost)
		{
			return null;
		}
		return actL;
	}

	private bool TryReleaseSkillWithAnimation(int index)
	{
		if (!SingletonMonoScope<PlayerActionManager>.HasInstance || index < 0 || skillBT == null || index >= skillBT.Length)
		{
			return false;
		}
		ACT_skillBT aCT_skillBT = skillBT[index];
		if (!aCT_skillBT || !aCT_skillBT.Opened || !aCT_skillBT.actL || aCT_skillBT.actL.DT == null || aCT_skillBT.actL.IsCD)
		{
			return false;
		}
		if (PL.ManaStat.Cur < aCT_skillBT.actL.DT.ManaCost || PL.IsSkill || PL.IsAttack)
		{
			return false;
		}
		SingletonMonoScope<PlayerActionManager>.Instance.UseSkill(index);
		return true;
	}

	private static bool HasCooldown(ACTListSkillBT skill)
	{
		if (!skill || skill.DT == null)
		{
			return false;
		}
		if (!(skill.CDTime > 0.0001f))
		{
			if (skill.DT.simple != null)
			{
				return skill.DT.simple.CoolDown > 0.0001f;
			}
			return false;
		}
		return true;
	}

	private bool HasEnemyNearPlayer(float range)
	{
		int num = Physics2D.OverlapCircleNonAlloc(PL.transform.position, range, autoUseEnemies, LayerMask.GetMask("BodyCOLem"));
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = autoUseEnemies[i];
			autoUseEnemies[i] = null;
			if ((bool)collider2D)
			{
				BodyCOL component = collider2D.GetComponent<BodyCOL>();
				if ((bool)component && component.peo != null && component.peo.CharacterType == 2 && component.peo.em != null && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void TryGDUseSkills()
	{
		if (!PL || !PL.IsAlive || actListSkill == null || actListSkill.Count == 0 || !SingletonMonoScope<Gun>.HasInstance || Time.time < gdUseTimer)
		{
			return;
		}
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if ((bool)aCTListSkillBT && aCTListSkillBT.DT != null && aCTListSkillBT.DT.type == 0 && aCTListSkillBT.DT.simple != null)
			{
				int gD_Use = aCTListSkillBT.DT.simple.GD_Use;
				if (gD_Use > 0 && !(UnityEngine.Random.value >= (float)gD_Use * 0.01f) && TryReleaseSkillDirect(aCTListSkillBT, useCooldown: false, spendMana: false))
				{
					gdUseTimer = Time.time + 0.2f;
					break;
				}
			}
		}
	}

	public void AddWP_SPC(SPC_MB spc, int MainEL, int id, float SPC_PRC)
	{
		AddWP_SPC(spc, MainEL, id, 0, -1, SPC_PRC);
	}

	public void AddWP_SPC(SPC_MB spc, int MainEL, int id, int spcSlotIndex, int itemCharType, float SPC_PRC)
	{
		if (spc == null)
		{
			return;
		}
		int sPCSourceKey = GetSPCSourceKey(spc.SPCindex, spcSlotIndex, itemCharType);
		if (spc.SPCtype == 10)
		{
			SetORB(spc, MainEL, sPCSourceKey, 0, SPC_PRC, spcSlotIndex, itemCharType);
			return;
		}
		if (spc.SPCtype == 11)
		{
			if (!PL.xjl)
			{
				PL.xjl = PL.GetComponent<XJL_FSQ>();
			}
			if ((bool)PL.xjl)
			{
				PL.xjl.AddXJL(spc.OBJ, sPCSourceKey, MainEL, spc.Range1, spc.Range2, spc.FStime1, spc.FStime2, spc.Speed1, spc.Damage);
			}
			return;
		}
		ACT_SPC aCT_SPC = new ACT_SPC();
		aCT_SPC.GlobalID = sPCSourceKey;
		aCT_SPC.SPCindex = spc.SPCindex;
		aCT_SPC.SPCSlotIndex = spcSlotIndex;
		aCT_SPC.SPCItemCharType = itemCharType;
		aCT_SPC.SpecialType = spc.SPCtype;
		aCT_SPC.Name = spc.SPCname;
		aCT_SPC.FStype = spc.FStype;
		aCT_SPC.LockType = spc.LockType;
		aCT_SPC.info = spc.info;
		aCT_SPC.Price = spc.Price;
		aCT_SPC.skillName = spc.SkillName;
		aCT_SPC.ZQName = spc.ZQName;
		aCT_SPC.RTtypeOBJ = spc.RTtypeOBJ;
		aCT_SPC.Distance = spc.Distance;
		aCT_SPC.SPC_PRC = SPC_PRC;
		aCT_SPC.Rate = spc.Rate;
		aCT_SPC.Damage = spc.Damage;
		aCT_SPC.damageType = SWS.DMtype(MainEL);
		aCT_SPC.MainEL = MainEL;
		aCT_SPC.ThroughType = spc.ThroughType;
		aCT_SPC.AttackType = spc.AttackType;
		aCT_SPC.AttackTypeA = spc.AttackTypeA;
		aCT_SPC.AttackTypeB = spc.AttackTypeB;
		aCT_SPC.DamageA = spc.DamageA;
		aCT_SPC.DamageB = spc.DamageB;
		aCT_SPC.NoTime = spc.NoTime;
		aCT_SPC.BuffTime = spc.BuffTime;
		aCT_SPC.DebuffTime = spc.DebuffTime;
		aCT_SPC.Field_time = spc.Field_time;
		aCT_SPC.ORB_time = spc.ORB_time;
		aCT_SPC.EXP_time = spc.EXP_time;
		aCT_SPC.ZD_time_F = spc.ZD_time_F;
		aCT_SPC.ZD_time_S = spc.ZD_time_S;
		aCT_SPC.OBJ = spc.OBJ;
		aCT_SPC.Layer_SubA = spc.Layer_SubA;
		aCT_SPC.Layer_SubB = spc.Layer_SubB;
		aCT_SPC.ORB = spc.ORB;
		aCT_SPC.ZD_F = spc.ZD_F;
		aCT_SPC.ZD_S = spc.ZD_S;
		aCT_SPC.ZD_AB = spc.ZD_AB;
		aCT_SPC.EXP_F = spc.EXP_F;
		aCT_SPC.EXP_S = spc.EXP_S;
		aCT_SPC.EXP_AB = spc.EXP_AB;
		aCT_SPC.Dic_F = spc.Dic_F;
		aCT_SPC.Dic_S = spc.Dic_S;
		aCT_SPC.FX_F = spc.FX_F;
		aCT_SPC.FX_S = spc.FX_S;
		aCT_SPC.Sound = spc.Sound;
		aCT_SPC.Count_ORB = spc.Count_ORB;
		aCT_SPC.Count_ATtarget = spc.Count_ATtarget;
		aCT_SPC.Count_F = spc.Count_F;
		aCT_SPC.Count_S = spc.Count_S;
		aCT_SPC.Count_A = spc.Count_AB;
		aCT_SPC.CountMulti = spc.CountMulti;
		aCT_SPC.CountEXP = spc.CountEXP;
		aCT_SPC.TypeORB = spc.TypeORB;
		aCT_SPC.Type_F = spc.Type_F;
		aCT_SPC.Type_S = spc.Type_S;
		aCT_SPC.Type_AB = spc.Type_AB;
		aCT_SPC.TypeDIC_F = spc.TypeDIC_F;
		aCT_SPC.TypeDIC_S = spc.TypeDIC_S;
		aCT_SPC.TypeEXP_F = spc.TypeEXP_F;
		aCT_SPC.TypeEXP_S = spc.TypeEXP_S;
		aCT_SPC.TypeEXP_AB = spc.TypeEXP_AB;
		aCT_SPC.Size = spc.Size;
		aCT_SPC.High = spc.High;
		aCT_SPC.JG = spc.JG;
		aCT_SPC.AngleA = spc.AngleA;
		aCT_SPC.AngleB = spc.AngleB;
		aCT_SPC.Range1 = spc.Range1;
		aCT_SPC.Range2 = spc.Range2;
		aCT_SPC.Range_AT = spc.Range_AT;
		aCT_SPC.FStime1 = spc.FStime1;
		aCT_SPC.FStime2 = spc.FStime2;
		aCT_SPC.Speed1 = spc.Speed1;
		aCT_SPC.Speed2 = spc.Speed2;
		aCT_SPC.Speed3 = spc.Speed3;
		aCT_SPC.Speed4 = spc.Speed4;
		aCT_SPC.Follow_F = spc.Follow_F;
		aCT_SPC.Follow_S = spc.Follow_S;
		aCT_SPC.AllChuan_F = spc.AllChuan_F;
		aCT_SPC.AllChuan_S = spc.AllChuan_S;
		aCT_SPC.Slow_F = spc.Slow_F;
		aCT_SPC.Slow_S = spc.Slow_S;
		aCT_SPC.RDSpeed_F = spc.RDSpeed_F;
		aCT_SPC.RDSpeed_S = spc.RDSpeed_S;
		aCT_SPC.HasFX = spc.HasFX;
		aCT_SPC.S_HasFX = spc.S_HasFX;
		aCT_SPC.A_HasFX = spc.AB_HasFX;
		aCT_SPC.colEXP = spc.colEXP;
		aCT_SPC.colEXP_A = spc.colEXP_A;
		aCT_SPC.S_colEXP = spc.S_colEXP;
		aCT_SPC.A_colEXP = spc.AB_colEXP;
		aCT_SPC.TimeEXP = spc.TimeEXP;
		aCT_SPC.TimeEXP_A = spc.TimeEXP_AB;
		aCT_SPC.LastEXP = spc.LastEXP;
		aCT_SPC.LastEXP_A = spc.LastEXP_AB;
		aCT_SPC.S_LastEXP = spc.S_LastEXP;
		aCT_SPC.A_LastEXP = spc.AB_LastEXP;
		aCT_SPC.EXPpos = spc.EXPpos;
		aCT_SPC.EXPpos_A = spc.EXPpos_AB;
		aCT_SPC.S_EXPpos = spc.S_EXPpos;
		aCT_SPC.A_EXPpos = spc.AB_EXPpos;
		aCT_SPC.AngleEXP = spc.AngleEXP;
		aCT_SPC.AngleEXP_A = spc.AngleEXP_AB;
		switch (spc.SPCtype)
		{
		case 1:
			SK[sPCSourceKey] = aCT_SPC;
			break;
		case 2:
			HIT[sPCSourceKey] = aCT_SPC;
			break;
		case 3:
			DIE[sPCSourceKey] = aCT_SPC;
			break;
		case 4:
			HURT[sPCSourceKey] = aCT_SPC;
			break;
		case 5:
			GD[sPCSourceKey] = aCT_SPC;
			break;
		case 20:
			CPSK[sPCSourceKey] = aCT_SPC;
			break;
		case 21:
			CPHURT[sPCSourceKey] = aCT_SPC;
			break;
		case 22:
			CPDIE[sPCSourceKey] = aCT_SPC;
			break;
		case 23:
			aCT_SPC.GlobalID = sPCSourceKey;
			CPUNIVERSE[aCT_SPC.GlobalID] = aCT_SPC;
			ApplyCompanionUniverseSPC(aCT_SPC);
			break;
		case 30:
			CPLINKSK[sPCSourceKey] = aCT_SPC;
			break;
		case 31:
			CPSAMESK[sPCSourceKey] = aCT_SPC;
			break;
		case 32:
			CPTRISK[sPCSourceKey] = aCT_SPC;
			break;
		}
	}

	public void DelWP_SPC(SPC_MB spc, int id)
	{
		DelWP_SPC(spc, id, 0, -1);
	}

	public void DelWP_SPC(SPC_MB spc, int id, int spcSlotIndex, int itemCharType)
	{
		if (spc == null)
		{
			return;
		}
		int sPCSourceKey = GetSPCSourceKey(spc.SPCindex, spcSlotIndex, itemCharType);
		switch (spc.SPCtype)
		{
		case 1:
			SK.Remove(sPCSourceKey);
			break;
		case 2:
			HIT.Remove(sPCSourceKey);
			break;
		case 3:
			DIE.Remove(sPCSourceKey);
			break;
		case 4:
			HURT.Remove(sPCSourceKey);
			break;
		case 5:
			GD.Remove(sPCSourceKey);
			break;
		case 10:
			SetORB(spc, 0, sPCSourceKey, 1, 1f, spcSlotIndex, itemCharType);
			break;
		case 11:
			if ((bool)PL.xjl)
			{
				PL.xjl.RemoveXJL(sPCSourceKey);
			}
			break;
		case 20:
			CPSK.Remove(sPCSourceKey);
			break;
		case 21:
			CPHURT.Remove(sPCSourceKey);
			break;
		case 22:
			CPDIE.Remove(sPCSourceKey);
			break;
		case 23:
			DelCompanionUniverseSPC(sPCSourceKey);
			break;
		case 30:
			CPLINKSK.Remove(sPCSourceKey);
			break;
		case 31:
			CPSAMESK.Remove(sPCSourceKey);
			break;
		case 32:
			CPTRISK.Remove(sPCSourceKey);
			break;
		case 0:
		case 6:
		case 7:
		case 8:
		case 9:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
			break;
		}
	}

	private int GetSPCSourceKey(int spcIndex, int spcSlotIndex, int itemCharType)
	{
		return ((((0x1A5D ^ spcIndex) * 397) ^ spcSlotIndex) * 397) ^ itemCharType;
	}

	public void SetORB(SPC_MB spc, int MainEL, int id, int A, float SPC_PRC = 1f, int spcSlotIndex = -1, int itemCharType = -1)
	{
		switch (A)
		{
		case 0:
		{
			if (ORB.ContainsKey(id))
			{
				SetORB(spc, MainEL, id, 1, SPC_PRC, spcSlotIndex, itemCharType);
			}
			SkillOBJ_DT_SP skillOBJ_DT_SP = spc.FStype switch
			{
				0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[spc.OBJ].OBJ[MainEL], PL.transform.position, Quaternion.identity, PL.transform).GetComponent<SkillOBJ_DT_SP>(), 
				1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[spc.OBJ].OBJ[MainEL], PL.yao.transform.position, Quaternion.identity, PL.transform).GetComponent<SkillOBJ_DT_SP>(), 
				2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[spc.OBJ].OBJ[MainEL], PL.body.transform.position, Quaternion.identity, PL.transform).GetComponent<SkillOBJ_DT_SP>(), 
				3 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[spc.OBJ].OBJ[MainEL], PL.headUp.transform.position, Quaternion.identity, PL.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
				_ => null, 
			};
			if ((bool)skillOBJ_DT_SP)
			{
				SetORBdata(skillOBJ_DT_SP, spc, MainEL, id, SPC_PRC, spcSlotIndex, itemCharType);
				SK_BuffA component2 = skillOBJ_DT_SP.gameObject.GetComponent<SK_BuffA>();
				if ((bool)component2)
				{
					component2.IsORB = true;
				}
				ORB[id] = skillOBJ_DT_SP;
			}
			break;
		}
		case 1:
		{
			ORB.TryGetValue(id, out var value);
			if ((bool)value)
			{
				SK_BuffA component = value.gameObject.GetComponent<SK_BuffA>();
				if ((bool)component)
				{
					component.StopORB();
				}
				else
				{
					LeanPool.Despawn(value);
				}
			}
			ORB.Remove(id);
			break;
		}
		}
	}

	private void ApplyCompanionUniverseSPC(ACT_SPC value)
	{
		if (value == null)
		{
			return;
		}
		foreach (Companion companionUniverseTarget in GetCompanionUniverseTargets())
		{
			if (IsCompanionUniverseTarget(value, companionUniverseTarget))
			{
				SetCompanionUniverseSPC(value, companionUniverseTarget);
			}
		}
	}

	private HashSet<Companion> GetCompanionUniverseTargets()
	{
		HashSet<Companion> hashSet = new HashSet<Companion>();
		if (actListSkill != null)
		{
			for (int i = 0; i < actListSkill.Count; i++)
			{
				ACTListSkillBT aCTListSkillBT = actListSkill[i];
				if (!aCTListSkillBT || aCTListSkillBT.cpList == null)
				{
					continue;
				}
				for (int j = 0; j < aCTListSkillBT.cpList.Count; j++)
				{
					Companion companion = aCTListSkillBT.cpList[j];
					if (IsPlayerCompanion(companion))
					{
						hashSet.Add(companion);
					}
				}
			}
		}
		Companion[] array = UnityEngine.Object.FindObjectsOfType<Companion>();
		foreach (Companion companion2 in array)
		{
			if (IsPlayerCompanion(companion2))
			{
				hashSet.Add(companion2);
			}
		}
		return hashSet;
	}

	private bool IsPlayerCompanion(Companion comp)
	{
		if (!comp)
		{
			return false;
		}
		if (!comp.sp || !(comp.sp.pl == PL))
		{
			return comp.PL == PL;
		}
		return true;
	}

	private void ApplyCompanionUniverseSPC(Companion comp)
	{
		if (!comp || CPUNIVERSE == null || CPUNIVERSE.Count == 0)
		{
			return;
		}
		foreach (ACT_SPC value in CPUNIVERSE.Values)
		{
			if (IsCompanionUniverseTarget(value, comp))
			{
				SetCompanionUniverseSPC(value, comp);
			}
		}
	}

	private bool IsCompanionUniverseTarget(ACT_SPC value, Companion comp)
	{
		if (value == null || !comp || string.IsNullOrEmpty(value.skillName))
		{
			return false;
		}
		if (value.skillName == "0")
		{
			return true;
		}
		if (comp.Name == value.skillName)
		{
			return true;
		}
		if ((bool)comp.sp)
		{
			return comp.sp.skillName == value.skillName;
		}
		return false;
	}

	private void SetCompanionUniverseSPC(ACT_SPC value, Companion comp)
	{
		if (!comp)
		{
			return;
		}
		comp.ClearPermanentSkill(value.GlobalID);
		GameObject skillPrefab = GetSkillPrefab(value.OBJ, value.MainEL);
		if (!skillPrefab)
		{
			return;
		}
		bool flag = skillPrefab.GetComponent<SK_CP_Universe>();
		bool flag2 = skillPrefab.GetComponent<SK_CP_Forever>();
		bool flag3 = skillPrefab.GetComponent<SK_CP_Round>();
		if (!flag && !flag2 && !flag3)
		{
			return;
		}
		Transform companionPermanentParent = GetCompanionPermanentParent(value, comp);
		Vector3 companionPermanentPosition = GetCompanionPermanentPosition(value, comp);
		GameObject gameObject = LeanPool.Spawn(skillPrefab, companionPermanentPosition, Quaternion.identity, companionPermanentParent);
		SkillOBJ_DT_SP component = gameObject.GetComponent<SkillOBJ_DT_SP>();
		SK_CP_Universe component2 = gameObject.GetComponent<SK_CP_Universe>();
		SK_CP_Forever component3 = gameObject.GetComponent<SK_CP_Forever>();
		SK_CP_Round component4 = gameObject.GetComponent<SK_CP_Round>();
		if (!component || (flag && !component2) || (flag2 && !component3) || (flag3 && !component4))
		{
			LeanPool.Despawn(gameObject);
			return;
		}
		SetCompanionUniverseData(component, value, comp);
		Dicform component5 = gameObject.GetComponent<Dicform>();
		if ((bool)component5)
		{
			component5.sp = component;
		}
		SK_BuffA component6 = gameObject.GetComponent<SK_BuffA>();
		if ((bool)component6)
		{
			component6.IsORB = true;
		}
		if ((bool)component2)
		{
			component2.sp = component;
			comp.AddUniverse(component2);
		}
		if ((bool)component3)
		{
			component3.sp = component;
			comp.AddForever(component3);
		}
		if ((bool)component4)
		{
			component4.sp = component;
			comp.AddRound(component4);
		}
	}

	private Transform GetCompanionPermanentParent(ACT_SPC value, Companion comp)
	{
		if (value.FStype == 3)
		{
			if (!comp.yao)
			{
				return comp.transform;
			}
			return comp.yao.transform;
		}
		return comp.transform;
	}

	private Vector3 GetCompanionPermanentPosition(ACT_SPC value, Companion comp)
	{
		switch (value.FStype)
		{
		case 0:
			return comp.transform.position;
		case 1:
			if (!comp.yao)
			{
				return comp.transform.position;
			}
			return comp.yao.transform.position;
		case 2:
			if (!comp.body)
			{
				return comp.transform.position;
			}
			return comp.body.transform.position;
		case 3:
			if (!comp.headUp)
			{
				if (!comp.yao)
				{
					return comp.transform.position;
				}
				return comp.yao.transform.position;
			}
			return comp.headUp.transform.position;
		default:
			return comp.transform.position;
		}
	}

	private GameObject GetSkillPrefab(int obj, int mainEl)
	{
		if (!_gameDataManager || _gameDataManager.SKPB == null || _gameDataManager.SKPB.Skill == null)
		{
			return null;
		}
		if (obj < 0 || obj >= _gameDataManager.SKPB.Skill.Length)
		{
			return null;
		}
		if (_gameDataManager.SKPB.Skill[obj] == null || _gameDataManager.SKPB.Skill[obj].OBJ == null)
		{
			return null;
		}
		if (mainEl < 0 || mainEl >= _gameDataManager.SKPB.Skill[obj].OBJ.Length)
		{
			return null;
		}
		return _gameDataManager.SKPB.Skill[obj].OBJ[mainEl];
	}

	private void SetCompanionUniverseData(SkillOBJ_DT_SP sp, ACT_SPC value, Companion comp)
	{
		Vector3 companionSpcTargetPosition = GetCompanionSpcTargetPosition(comp, comp.ATTarget);
		SetSPCdata(sp, value, companionSpcTargetPosition);
		sp.indexType = 1;
		sp.pl = SingletonMonoScope<PlayerManager>.Instance;
		sp.cp = comp;
		sp.ZY = true;
		sp.Dot_Infect = false;
		sp.Dot_Infect_Layer = 0;
		sp.TargetPos = companionSpcTargetPosition;
		sp.skillName = value.Name;
		sp.ZQName = value.ZQName;
		sp.GlobalID = value.GlobalID;
		sp.SPCindex = value.SPCindex;
		sp.SPCSlotIndex = value.SPCSlotIndex;
		sp.SPCItemCharType = value.SPCItemCharType;
		sp.SpecialType = value.SpecialType;
		RefreshCompanionUniverseData(sp, comp);
	}

	public void RefreshCompanionUniverseData(SkillOBJ_DT_SP sp, Companion comp)
	{
		if ((bool)sp && (bool)comp && (bool)PL)
		{
			float num = PL.ORB_Damage_Last + PL.Orb_Universe_DMG_Last;
			sp.Damage = PL.GiveDamage(sp.damageType) * comp.Damage_Last / 100f * sp.SPC_Damage / 100f * (1f + num / 100f);
			sp.DamageA = PL.GiveDamage(sp.damageType) * comp.Damage_Last / 100f * sp.SPC_DamageA / 100f * (1f + num / 100f);
			sp.DamageB = PL.GiveDamage(sp.damageType) * comp.Damage_Last / 100f * sp.SPC_DamageB / 100f * (1f + num / 100f);
			sp.JYrate = PL.JYrate_Last;
			sp.BJrate = (comp.BJ_NoDot ? 100f : PL.BJrate_Last);
			sp.BJDamage = PL.BJDamage_Last;
			sp.Through = PL.ThroughRate;
			sp.Chuan = PL.GiveChuan(sp.damageType);
			sp.FlySpeed = comp.FlySpeed;
		}
	}

	private void DelCompanionUniverseSPC(int id)
	{
		CPUNIVERSE.TryGetValue(id, out var value);
		CPUNIVERSE.Remove(id);
		if (value == null)
		{
			return;
		}
		foreach (Companion companionUniverseTarget in GetCompanionUniverseTargets())
		{
			if (IsCompanionUniverseTarget(value, companionUniverseTarget))
			{
				companionUniverseTarget.ClearPermanentSkill(id);
			}
		}
	}

	public void CreatACT_SK(string a, Transform trans, float z)
	{
		CreatACT_SK(a, trans, z, SingletonMonoScope<Gun>.HasInstance ? Gun.MousePos : AimProvider.GetAimWorldPos());
	}

	public void CreatACT_SK(string a, Transform trans, float z, Vector3 castTargetPos)
	{
		CreatACT_CompSameAndTri(a);
		foreach (ACT_SPC value in SK.Values)
		{
			if (!(value.skillName == a) || !((float)UnityEngine.Random.Range(0, 101) < value.RateLast))
			{
				continue;
			}
			Vector3 playerSpcTargetPosition = GetPlayerSpcTargetPosition(value, castTargetPos);
			SkillOBJ_DT_SP skillOBJ_DT_SP;
			switch (value.FStype)
			{
			case 0:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 1:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 2:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.transform.position, Quaternion.identity, PL.transform).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 3:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.yao.transform.position, Quaternion.identity, PL.transform).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 4:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.yao.transform.position, Quaternion.identity, PL.yao.transform).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 10:
			{
				Vector3 vector = playerSpcTargetPosition;
				Vector3 vector2 = playerSpcTargetPosition - PL.transform.position;
				float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
				float num2 = ((value.Distance > 0f) ? value.Distance : 6f);
				float x = PL.transform.position.x + num2 * Mathf.Cos(num * 3.14f / 180f);
				float y = PL.transform.position.y + num2 * Mathf.Sin(num * 3.14f / 180f);
				Vector3 vector3 = new Vector3(x, y, PL.transform.position.z);
				RaycastHit2D raycastHit2D = Physics2D.Raycast(PL.transform.position, vector - PL.transform.position, Vector2.Distance(PL.transform.position, vector), LayerMask.GetMask("block"));
				Vector3 vector4;
				if (!raycastHit2D.collider)
				{
					vector4 = ((!(Vector3.Distance(vector, PL.transform.position) > Vector3.Distance(vector3, PL.transform.position))) ? vector : vector3);
				}
				else if (!raycastHit2D.collider.CompareTag("blockWALL"))
				{
					vector4 = ((!(Vector3.Distance(vector, PL.transform.position) > Vector3.Distance(vector3, PL.transform.position))) ? vector : vector3);
				}
				else
				{
					Vector3 vector5 = new Vector3(raycastHit2D.point.x, raycastHit2D.point.y, 0f);
					vector4 = ((!(Vector3.Distance(vector, PL.transform.position) > Vector3.Distance(vector3, PL.transform.position))) ? ((!(Vector3.Distance(vector, PL.transform.position) > Vector3.Distance(vector5, PL.transform.position))) ? vector : vector5) : ((!(Vector3.Distance(vector3, PL.transform.position) > Vector3.Distance(vector5, PL.transform.position))) ? vector3 : vector5));
				}
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], new Vector3(vector4.x, vector4.y, 0f), Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				break;
			}
			default:
				skillOBJ_DT_SP = null;
				break;
			}
			if (value.RTtypeOBJ == 0)
			{
				Vector3 vector6 = playerSpcTargetPosition - PL.transform.position;
				float z2 = Mathf.Atan2(vector6.y, vector6.x) * 57.29578f;
				if (value.FStype == 10)
				{
					skillOBJ_DT_SP.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z2);
				}
				else
				{
					skillOBJ_DT_SP.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z);
				}
			}
			SetSPCdata(skillOBJ_DT_SP, value, playerSpcTargetPosition);
		}
	}

	private Vector3 GetPlayerSpcTargetPosition(ACT_SPC value, Vector3 fallbackTargetPos)
	{
		if ((bool)PL && PL.IsAutoLockActive())
		{
			if (value.LockType == 1 && PL.TryGetAutoLockFootPosition(out var position))
			{
				return position;
			}
			if (PL.TryGetAutoLockYaoPosition(out var position2))
			{
				return position2;
			}
		}
		fallbackTargetPos.z = 0f;
		return fallbackTargetPos;
	}

	public void CreatACT_CPSK(string a, Companion comp, Transform trans, float z)
	{
		if (!comp)
		{
			return;
		}
		CreatACT_CPLinkSK(a, comp);
		foreach (ACT_SPC value in CPSK.Values)
		{
			if (value.skillName != a || (float)UnityEngine.Random.Range(0, 101) >= value.RateLast)
			{
				continue;
			}
			SkillOBJ_DT_SP skillOBJ_DT_SP;
			switch (value.FStype)
			{
			case 0:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 1:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 2:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.transform.position, Quaternion.identity, comp.transform).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 3:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.yao.transform.position, Quaternion.identity, comp.transform).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 4:
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.yao.transform.position, Quaternion.identity, comp.yao.transform).GetComponent<SkillOBJ_DT_SP>();
				break;
			case 10:
			{
				Vector3 companionSpcTargetPosition = GetCompanionSpcTargetPosition(comp, trans);
				Vector3 companionSpcLimitedTarget = GetCompanionSpcLimitedTarget(comp, companionSpcTargetPosition, value.Distance);
				skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], companionSpcLimitedTarget, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
				break;
			}
			default:
				skillOBJ_DT_SP = null;
				break;
			}
			if (!skillOBJ_DT_SP)
			{
				continue;
			}
			if (value.RTtypeOBJ == 0)
			{
				if (value.FStype == 10)
				{
					Vector3 vector = GetCompanionSpcTargetPosition(comp, trans) - comp.transform.position;
					float z2 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
					skillOBJ_DT_SP.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z2);
				}
				else
				{
					skillOBJ_DT_SP.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z);
				}
			}
			SetSPCdata(skillOBJ_DT_SP, value);
			skillOBJ_DT_SP.cp = comp;
			skillOBJ_DT_SP.TargetPos = GetCompanionSpcTargetPosition(comp, trans);
		}
	}

	private void CreatACT_CPLinkSK(string a, Companion comp)
	{
		if (!comp || CPLINKSK.Count == 0)
		{
			return;
		}
		foreach (ACT_SPC value in CPLINKSK.Values)
		{
			if (IsCompanionSPCTarget(value, comp, a) && RollSPCRate(value))
			{
				ACTListSkillBT skill = FindSampleSkillByName(value.ZQName);
				FirePlayerSkillFromCompanion(skill, comp, useAttackPointForWeaponFStype: true);
			}
		}
	}

	private void CreatACT_CompSameAndTri(string playerSkillName)
	{
		if (string.IsNullOrEmpty(playerSkillName) || (CPSAMESK.Count == 0 && CPTRISK.Count == 0))
		{
			return;
		}
		ACTListSkillBT aCTListSkillBT = null;
		foreach (Companion activeCompanion in GetActiveCompanions())
		{
			foreach (ACT_SPC value in CPSAMESK.Values)
			{
				if (IsCompanionSPCTarget(value, activeCompanion, null) && IsSkillNameMatch(value.ZQName, playerSkillName) && RollSPCRate(value))
				{
					if (!aCTListSkillBT)
					{
						aCTListSkillBT = FindSampleSkillByName(playerSkillName);
					}
					FirePlayerSkillFromCompanion(aCTListSkillBT, activeCompanion, useAttackPointForWeaponFStype: false);
				}
			}
			foreach (ACT_SPC value2 in CPTRISK.Values)
			{
				if (IsCompanionSPCTarget(value2, activeCompanion, null) && IsSkillNameMatch(value2.ZQName, playerSkillName) && RollSPCRate(value2))
				{
					FireCompanionOwnSkill(activeCompanion);
				}
			}
		}
	}

	private IEnumerable<Companion> GetActiveCompanions()
	{
		if (actListSkill == null)
		{
			yield break;
		}
		HashSet<Companion> visited = new HashSet<Companion>();
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT bt = actListSkill[i];
			if (!bt || bt.cpList == null)
			{
				continue;
			}
			for (int j = 0; j < bt.cpList.Count; j++)
			{
				Companion companion = bt.cpList[j];
				if (IsActiveCompanion(companion) && visited.Add(companion))
				{
					yield return companion;
				}
			}
		}
	}

	private bool IsCompanionSPCTarget(ACT_SPC value, Companion comp, string triggerName)
	{
		if (value == null || !comp || string.IsNullOrWhiteSpace(value.skillName))
		{
			return false;
		}
		string text = value.skillName.Trim();
		if (text == "0")
		{
			return true;
		}
		if (!string.IsNullOrEmpty(triggerName) && triggerName == text)
		{
			return true;
		}
		if (comp.Name == text)
		{
			return true;
		}
		if ((bool)comp.sp)
		{
			return comp.sp.skillName == text;
		}
		return false;
	}

	private bool IsSkillNameMatch(string ruleName, string skillName)
	{
		if (string.IsNullOrWhiteSpace(ruleName) || string.IsNullOrEmpty(skillName))
		{
			return false;
		}
		string text = ruleName.Trim();
		if (!(text == "0"))
		{
			return text == skillName;
		}
		return true;
	}

	private bool RollSPCRate(ACT_SPC value)
	{
		if (value == null)
		{
			return false;
		}
		return (float)UnityEngine.Random.Range(0, 101) < value.RateLast;
	}

	private void FirePlayerSkillFromCompanion(ACTListSkillBT skill, Companion comp, bool useAttackPointForWeaponFStype)
	{
		if (!skill || skill.DT == null || skill.DT.type != 0 || skill.DT.simple == null || !comp)
		{
			return;
		}
		ACT_skillSample simple = skill.DT.simple;
		if (simple.FStype == 3)
		{
			return;
		}
		GameObject playerSkillPrefab = GetPlayerSkillPrefab(simple);
		if (!playerSkillPrefab)
		{
			return;
		}
		Vector3 companionPlayerSkillTargetPosition = GetCompanionPlayerSkillTargetPosition(comp);
		Vector3 companionPlayerSkillAimPosition = GetCompanionPlayerSkillAimPosition(comp, simple.LockType);
		ResolveCompanionPlayerSkillSpawn(simple, comp, companionPlayerSkillTargetPosition, useAttackPointForWeaponFStype, out var spawnPos, out var parent, out var angleOrigin);
		SkillOBJ_DT_SP component = (parent ? LeanPool.Spawn(playerSkillPrefab, spawnPos, Quaternion.identity, parent) : LeanPool.Spawn(playerSkillPrefab, spawnPos, Quaternion.identity)).GetComponent<SkillOBJ_DT_SP>();
		if ((bool)component)
		{
			SetCompanionPlayerSkillData(component, skill, comp, companionPlayerSkillTargetPosition);
			Vector3 vector = companionPlayerSkillAimPosition - angleOrigin;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			switch (simple.RTtypeOBJ)
			{
			case 0:
				component.transform.rotation = Quaternion.Euler(0f, 0f, z);
				break;
			case 1:
				component.dic = new Vector2(vector.x, vector.y);
				break;
			}
		}
	}

	private GameObject GetPlayerSkillPrefab(ACT_skillSample data)
	{
		if (!_gameDataManager || _gameDataManager.SKPB == null || _gameDataManager.SKPB.SK_Group == null || data == null)
		{
			return null;
		}
		if (data.OBJ_Group < 0 || data.OBJ_Group >= _gameDataManager.SKPB.SK_Group.Length)
		{
			return null;
		}
		SKprefab_OBJ sKprefab_OBJ = _gameDataManager.SKPB.SK_Group[data.OBJ_Group];
		if (sKprefab_OBJ == null || sKprefab_OBJ.OBJ == null || data.OBJ < 0 || data.OBJ >= sKprefab_OBJ.OBJ.Length)
		{
			return null;
		}
		return sKprefab_OBJ.OBJ[data.OBJ];
	}

	private void ResolveCompanionPlayerSkillSpawn(ACT_skillSample data, Companion comp, Vector3 targetPos, bool useAttackPointForWeaponFStype, out Vector3 spawnPos, out Transform parent, out Vector3 angleOrigin)
	{
		parent = null;
		Transform companionAttackPoint = GetCompanionAttackPoint(comp);
		Transform companionYao = GetCompanionYao(comp);
		Transform transform = (comp ? comp.transform : null);
		Transform companionHeadUp = GetCompanionHeadUp(comp);
		switch (data.FStype)
		{
		case 0:
		case 1:
		case 2:
		{
			Transform transform2 = (useAttackPointForWeaponFStype ? companionAttackPoint : companionYao);
			spawnPos = transform2.position;
			switch (data.FStype)
			{
			case 1:
				angleOrigin = companionYao.position;
				break;
			case 2:
				angleOrigin = companionHeadUp.position;
				break;
			default:
				angleOrigin = transform2.position;
				break;
			}
			break;
		}
		case 4:
			spawnPos = transform.position;
			angleOrigin = transform.position;
			break;
		case 5:
			spawnPos = companionYao.position;
			angleOrigin = companionYao.position;
			break;
		case 6:
			spawnPos = companionHeadUp.position;
			angleOrigin = companionHeadUp.position;
			break;
		case 7:
			parent = transform;
			spawnPos = transform.position;
			angleOrigin = transform.position;
			break;
		case 8:
			parent = companionYao;
			spawnPos = companionYao.position;
			angleOrigin = companionYao.position;
			break;
		case 9:
			parent = companionHeadUp;
			spawnPos = companionHeadUp.position;
			angleOrigin = companionHeadUp.position;
			break;
		case 10:
			spawnPos = targetPos;
			angleOrigin = comp.transform.position;
			break;
		default:
			spawnPos = companionAttackPoint.position;
			angleOrigin = companionAttackPoint.position;
			break;
		}
		spawnPos.z = 0f;
		angleOrigin.z = 0f;
	}

	private Vector3 GetCompanionPlayerSkillTargetPosition(Companion comp)
	{
		if ((bool)comp && (bool)comp.MVTarget)
		{
			return comp.MVTarget.position;
		}
		if ((bool)comp && (bool)comp.ATTarget)
		{
			return comp.ATTarget.position;
		}
		if (!comp)
		{
			return Vector3.zero;
		}
		return comp.transform.position;
	}

	private Vector3 GetCompanionPlayerSkillAimPosition(Companion comp, int lockType)
	{
		if ((bool)comp && lockType == 1 && (bool)comp.MVTarget)
		{
			return comp.MVTarget.position;
		}
		if ((bool)comp && (bool)comp.ATTarget)
		{
			return comp.ATTarget.position;
		}
		if ((bool)comp && (bool)comp.MVTarget)
		{
			return comp.MVTarget.position;
		}
		if (!comp)
		{
			return Vector3.zero;
		}
		return comp.transform.position;
	}

	private Transform GetCompanionAttackPoint(Companion comp)
	{
		if (!comp)
		{
			return null;
		}
		Transform transform = comp.transform.Find("main/Spine/AT");
		if (!transform)
		{
			transform = comp.transform.Find("main/Spirit");
		}
		if (!transform)
		{
			transform = GetCompanionYao(comp);
		}
		if (!transform)
		{
			return comp.transform;
		}
		return transform;
	}

	private Transform GetCompanionYao(Companion comp)
	{
		if ((bool)comp && (bool)comp.yao)
		{
			return comp.yao.transform;
		}
		if (!comp)
		{
			return null;
		}
		return comp.transform;
	}

	private Transform GetCompanionHeadUp(Companion comp)
	{
		if ((bool)comp && (bool)comp.headUp)
		{
			return comp.headUp.transform;
		}
		return GetCompanionYao(comp);
	}

	private Transform GetCompanionBody(Companion comp)
	{
		if ((bool)comp && (bool)comp.body)
		{
			return comp.body.transform;
		}
		return GetCompanionYao(comp);
	}

	private void SetCompanionPlayerSkillData(SkillOBJ_DT_SP sp, ACTListSkillBT skill, Companion comp, Vector3 targetPos)
	{
		ACT_skillSample simple = skill.DT.simple;
		simple.EnsureRuntimeBuffDefaults();
		sp.FStype = simple.FStype;
		sp.LockType = simple.LockType;
		sp.indexType = 1;
		sp.pl = PL;
		sp.cp = comp;
		sp.ZY = true;
		sp.TargetPos = targetPos;
		sp.skillName = skill.IndexName;
		sp.ZQName = null;
		sp.RTtypeOBJ = simple.RTtypeOBJ;
		sp.RTtypeFX = simple.RTtypeFX;
		sp.Distance = simple.Distance;
		sp.GlobalID = 100000;
		sp.SpecialType = 0;
		sp.damageType = simple.damageType;
		sp.MainEL = simple.MainEL;
		sp.ThroughType = simple.ThroughType;
		sp.AttackType = simple.AttackType;
		sp.AttackTypeA = simple.AttackTypeA;
		sp.AttackTypeB = simple.AttackTypeB;
		float jCSkillDamage = GetJCSkillDamage(skill);
		float num = jCSkillDamage * 0.3f;
		sp.Damage = (simple.Damage + jCSkillDamage) / 100f * PL.GiveDamage(sp.damageType);
		sp.DamageA = (simple.DamageA + num) / 100f * PL.GiveDamage(sp.damageType);
		sp.DamageB = (simple.DamageB + num) / 100f * PL.GiveDamage(sp.damageType);
		sp.SPC_Damage = simple.Damage;
		sp.SPC_DamageA = simple.DamageA;
		sp.SPC_DamageB = simple.DamageB;
		sp.BJrate = simple.BJrate + PL.BJrate_Last;
		sp.BJDamage = simple.BJDamage + PL.BJDamage_Last;
		sp.JYrate = simple.JYrate + PL.JYrate_Last;
		sp.Through = simple.Through + PL.ThroughRate;
		sp.FlySpeed = simple.FlySpeed + PL.FlySpeed;
		sp.MoveSpeedCut = simple.MoveSpeedCut;
		sp.AttackSpeedCut = simple.AttackSpeedCut;
		sp.AntiCut = simple.AntiCut;
		sp.BF_Damage = simple.BF_Damage;
		sp.BF_EL_Damage = simple.BF_EL_Damage;
		sp.BF_EL_Chuan = simple.BF_EL_Chuan;
		sp.BF_BJrate = simple.BF_BJrate;
		sp.BF_JYrate = simple.BF_JYrate;
		sp.BF_GeDang = simple.BF_GeDang;
		sp.BF_AttackSpeed = simple.BF_AttackSpeed;
		sp.BF_MoveSpeed = simple.BF_MoveSpeed;
		sp.BF_DamageAnti = simple.BF_DamageAnti;
		sp.BF_Health_Prc = simple.BF_Health_Prc;
		sp.C_Damage = simple.C_Damage;
		sp.C_ATspeed = simple.C_ATspeed;
		sp.C_MVspeed = simple.C_MVspeed;
		sp.C_Health_Prc = simple.C_Health_Prc;
		sp.CF_Rate = simple.CF_Rate;
		sp.CF_Type = simple.CF_Type;
		sp.CF_Count = simple.CF_Count;
		sp.Layer_SubA = simple.Layer_SubA;
		sp.Layer_SubB = simple.Layer_SubB;
		sp.BSAT = simple.BSAT;
		sp.BSAT_Count = simple.BSAT_Count;
		sp.BSAT_Angle = simple.BSAT_Angle;
		sp.Is_BS = simple.Is_BS;
		sp.ChangeSkin = simple.ChangeSkin;
		sp.SkinIndex = simple.SkinIndex;
		sp.Reborn = simple.Reborn;
		sp.BSAT_Damage = simple.BSAT_Damage;
		sp.BSAT_DMG = simple.BSAT_DMG;
		sp.AutoUse = simple.AutoUse;
		sp.Refresh = simple.Refresh;
		sp.CompUP_DMG = simple.CompUP_DMG;
		sp.ATtarUP = simple.ATtarUP;
		sp.MS_Dead = simple.MS_Dead;
		sp.GD_Use = simple.GD_Use;
		sp.JCskill = simple.JCskill;
		sp.LinkSK = simple.LinkSK;
		sp.LinkAll = simple.LinkAll;
		sp.EveryLink = simple.EveryLink;
		sp.LastSkill = simple.LastSkill;
		sp.DashSkill = simple.DashSkill;
		sp.TPSkill = simple.TPSkill;
		sp.UseDMG = simple.UseDMG;
		sp.UseATS = simple.UseATS;
		sp.UseMVS = simple.UseMVS;
		CopySix(simple.UseDMG_EL, sp.UseDMG_EL);
		CopySix(simple.UseChuan, sp.UseChuan);
		sp.UseCP_DMG = simple.UseCP_DMG;
		sp.UseCP_ATS = simple.UseCP_ATS;
		sp.Has_DMG = simple.Has_DMG;
		sp.Has_ATS = simple.Has_ATS;
		sp.Has_MVS = simple.Has_MVS;
		sp.Has_BJR = simple.Has_BJR;
		sp.Has_BJD = simple.Has_BJD;
		sp.Has_DotTimeCut = simple.Has_DotTimeCut;
		sp.Has_DMG_Cut = simple.Has_DMG_Cut;
		sp.Has_GD = simple.Has_GD;
		sp.Has_ORB_DMG = simple.Has_ORB_DMG;
		sp.Has_XJ_DMG = simple.Has_XJ_DMG;
		sp.Has_Dot_DMG = simple.Has_Dot_DMG;
		sp.Has_CP_DMG = simple.Has_CP_DMG;
		sp.WD = simple.WD;
		sp.Crit_Time = simple.Crit_Time;
		sp.Crit_CD = simple.Crit_CD;
		sp.Over_Prc = simple.Over_Prc;
		sp.CutSpeedZone = simple.CutSpeedZone;
		sp.TypeORB = simple.TypeORB;
		sp.Type_F = simple.Type_F;
		sp.Type_S = simple.Type_S;
		sp.Type_AB = simple.Type_AB;
		sp.TypeDIC_F = simple.TypeDIC_F;
		sp.TypeDIC_S = simple.TypeDIC_S;
		sp.TypeEXP_F = simple.TypeEXP_F;
		sp.TypeEXP_S = simple.TypeEXP_S;
		sp.TypeEXP_AB = simple.TypeEXP_AB;
		sp.Size = simple.Size;
		sp.High = simple.High;
		sp.JG = simple.JG;
		sp.AngleA = simple.AngleA;
		sp.AngleB = simple.AngleB;
		sp.Range1 = simple.Range1;
		sp.Range2 = simple.Range2;
		sp.Range_AT = simple.Range_AT;
		sp.FStime1 = simple.FStime1;
		sp.FStime2 = simple.FStime2;
		sp.Speed1 = simple.Speed1;
		sp.Speed2 = simple.Speed2;
		sp.Speed3 = simple.Speed3;
		sp.Speed4 = simple.Speed4;
		sp.Count_ORB = simple.Count_ORB;
		sp.Count_ATtarget = simple.Count_ATtarget;
		sp.ATtar_DMG = simple.ATtar_DMG;
		sp.Count_F = simple.Count_F;
		sp.Count_S = simple.Count_S;
		sp.Count_AB = simple.Count_AB;
		sp.CountMulti = simple.CountMulti;
		sp.CountEXP = simple.CountEXP;
		sp.NoTime = simple.NoTime;
		sp.BuffTime = simple.BuffTime;
		sp.DebuffTime = simple.DebuffTime;
		sp.Field_time = simple.Field_time;
		sp.ORB_time = simple.ORB_time;
		sp.EXP_time = simple.EXP_time;
		sp.ZD_time_F = simple.ZD_time_F;
		sp.ZD_time_S = simple.ZD_time_S;
		sp.ORB = simple.ORB;
		sp.ZD_F = simple.ZD_F;
		sp.ZD_S = simple.ZD_S;
		sp.ZD_AB = simple.ZD_AB;
		sp.EXP_F = simple.EXP_F;
		sp.EXP_S = simple.EXP_S;
		sp.EXP_AB = simple.EXP_AB;
		sp.Dic_F = simple.Dic_F;
		sp.Dic_S = simple.Dic_S;
		sp.FX_F = simple.FX_F;
		sp.FX_S = simple.FX_S;
		sp.Sound = simple.Sound;
		sp.Follow_F = simple.Follow_F;
		sp.Follow_S = simple.Follow_S;
		sp.AllChuan_F = simple.AllChuan_F;
		sp.AllChuan_S = simple.AllChuan_S;
		sp.Slow_F = simple.Slow_F;
		sp.Slow_S = simple.Slow_S;
		sp.RDSpeed_F = simple.RDSpeed_F;
		sp.RDSpeed_S = simple.RDSpeed_S;
		sp.HasFX = simple.HasFX;
		sp.S_HasFX = simple.S_HasFX;
		sp.AB_HasFX = simple.A_HasFX;
		sp.colEXP = simple.colEXP;
		sp.colEXP_A = simple.colEXP_A;
		sp.S_colEXP = simple.S_colEXP;
		sp.AB_colEXP = simple.A_colEXP;
		sp.TimeEXP = simple.TimeEXP;
		sp.TimeEXP_AB = simple.TimeEXP_A;
		sp.LastEXP = simple.LastEXP;
		sp.LastEXP_AB = simple.LastEXP_A;
		sp.S_LastEXP = simple.S_LastEXP;
		sp.AB_LastEXP = simple.A_LastEXP;
		sp.EXPpos = simple.EXPpos;
		sp.EXPpos_AB = simple.EXPpos_A;
		sp.S_EXPpos = simple.S_EXPpos;
		sp.AB_EXPpos = simple.A_EXPpos;
		sp.AngleEXP = simple.AngleEXP;
		sp.AngleEXP_AB = simple.AngleEXP_A;
		sp.Dot_Infect = false;
		sp.Dot_Infect_Layer = 0;
	}

	private void CopySix(int[] source, int[] target)
	{
		if (target != null && target.Length >= 6)
		{
			for (int i = 0; i < 6; i++)
			{
				target[i] = ((source != null && source.Length > i) ? source[i] : 0);
			}
		}
	}

	private void FireCompanionOwnSkill(Companion comp)
	{
		if (!comp || comp.OBJ_SKA == null || comp.SK_ZD < 0 || comp.SK_ZD >= comp.OBJ_SKA.Length)
		{
			return;
		}
		GameObject gameObject = comp.OBJ_SKA[comp.SK_ZD];
		if (!gameObject)
		{
			return;
		}
		Vector3 companionSpcTargetPosition = GetCompanionSpcTargetPosition(comp, null);
		Transform companionYao = GetCompanionYao(comp);
		Vector3 vector = companionSpcTargetPosition - companionYao.position;
		float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		SkillOBJ_DT_SP component;
		switch (comp.SKApos)
		{
		case 1:
			component = LeanPool.Spawn(gameObject, comp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			break;
		case 3:
		{
			Vector3 position = companionSpcTargetPosition;
			if ((bool)comp.MVTarget)
			{
				Enemy component2 = comp.MVTarget.GetComponent<Enemy>();
				if ((bool)component2)
				{
					position = component2.transform.position;
				}
			}
			component = LeanPool.Spawn(gameObject, position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			break;
		}
		case 4:
			component = LeanPool.Spawn(gameObject, companionSpcTargetPosition, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
			break;
		default:
			component = LeanPool.Spawn(gameObject, companionYao.position, Quaternion.Euler(0f, 0f, z)).GetComponent<SkillOBJ_DT_SP>();
			break;
		}
		if ((bool)component)
		{
			SetCompanionOwnSkillData(component, comp, companionSpcTargetPosition);
		}
	}

	private void SetCompanionOwnSkillData(SkillOBJ_DT_SP sp, Companion comp, Vector3 targetPos)
	{
		sp.indexType = 1;
		sp.pl = PL;
		sp.cp = comp;
		sp.ZY = true;
		sp.Dot_Infect = false;
		sp.Dot_Infect_Layer = 0;
		sp.skillName = comp.Name;
		sp.AttackType = true;
		sp.damageType = comp.ChangeEL_SK;
		sp.Chuan = PL.GiveChuan(comp.ChangeEL_SK);
		sp.Damage = comp.Damage_Last * comp.ATS_Damage / comp.Damage_Base / 100f * PL.GiveDamage(comp.ChangeEL_SK) * comp.SkillDamageMultiplier;
		sp.Type_F = comp.Type_B;
		sp.Type_S = comp.Type_B;
		sp.TypeDIC_F = comp.TypeDIC_B;
		sp.TypeDIC_S = comp.TypeDIC_B;
		sp.JG = comp.JG_B;
		sp.AngleA = comp.AngleB;
		sp.AngleB = comp.AngleB;
		sp.FStime1 = comp.FStimeB;
		sp.FStime2 = comp.FStimeB;
		sp.Count_F = comp.Count_B;
		sp.Count_S = comp.Count_B;
		sp.Count_ATtarget = comp.Count_ATtarget_B;
		sp.CountMulti = comp.CountMulti_B;
		sp.Follow_F = comp.Follow_B;
		sp.Follow_S = comp.Follow_B;
		sp.AllChuan_F = comp.AllChuan_B;
		sp.RDSpeed_F = comp.RDSpeed_B;
		sp.HasFX = comp.HasFX_B;
		sp.colEXP = comp.colEXP_B;
		sp.EXPpos = comp.EXPpos_B;
		sp.BJrate = (comp.BJ_NoDot ? 100f : PL.BJrate_Last);
		sp.JYrate = PL.JYrate_Last;
		sp.BJDamage = PL.BJDamage_Last;
		sp.FlySpeed = comp.FlySpeed;
		sp.AT_DotLayer = comp.AT_DotLayer;
		sp.BJ_NoDot = comp.BJ_NoDot;
		sp.WS_All = comp.WS_All;
		sp.Field_Range = comp.Field_Range;
		sp.TargetPos = targetPos;
		sp.Distance = comp.Range_EM;
		sp.Layer_SubA = 0;
		sp.Layer_SubB = 0;
		sp.NoTime = 1;
		sp.BuffTime = 1f;
		sp.Slow_F = 1;
		sp.Slow_S = 1;
		sp.AB_HasFX = 1;
		sp.colEXP_A = 1;
		sp.AB_colEXP = 1;
		sp.TimeEXP = 1;
		sp.TimeEXP_AB = 1;
		sp.LastEXP = 1;
		sp.LastEXP_AB = 1;
		sp.S_LastEXP = 1;
		sp.AB_LastEXP = 1;
		sp.AngleEXP = 1;
	}

	public void CreatACT_CPHurt(Companion comp)
	{
		if (!comp || CPHURT.Count == 0 || Time.time < actCpHurtTimer)
		{
			return;
		}
		actCpHurtTimer = Time.time + 0.2f;
		foreach (ACT_SPC value in CPHURT.Values)
		{
			if (!((float)UnityEngine.Random.Range(0, 101) >= value.RateLast))
			{
				SkillOBJ_DT_SP skillOBJ_DT_SP = value.FStype switch
				{
					0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.body.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					3 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.headUp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					_ => null, 
				};
				if ((bool)skillOBJ_DT_SP)
				{
					SetSPCdata(skillOBJ_DT_SP, value);
					skillOBJ_DT_SP.cp = comp;
					skillOBJ_DT_SP.TargetPos = GetCompanionSpcTargetPosition(comp, null);
				}
			}
		}
	}

	public void CreatACT_CPDie(Companion comp)
	{
		if (!comp || CPDIE.Count == 0)
		{
			return;
		}
		foreach (ACT_SPC value in CPDIE.Values)
		{
			if (!((float)UnityEngine.Random.Range(0, 101) >= value.RateLast))
			{
				SkillOBJ_DT_SP skillOBJ_DT_SP = value.FStype switch
				{
					0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.yao.transform.position, Quaternion.identity, comp.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
					_ => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], comp.yao.transform.position, Quaternion.identity, comp.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
				};
				if ((bool)skillOBJ_DT_SP)
				{
					SetSPCdata(skillOBJ_DT_SP, value);
					skillOBJ_DT_SP.cp = comp;
					skillOBJ_DT_SP.TargetPos = GetCompanionSpcTargetPosition(comp, null);
				}
			}
		}
	}

	private Vector3 GetCompanionSpcTargetPosition(Companion comp, Transform fallback)
	{
		if ((bool)comp && (bool)comp.ATTarget)
		{
			return comp.ATTarget.position;
		}
		if ((bool)comp && (bool)comp.MVTarget)
		{
			return comp.MVTarget.position;
		}
		if ((bool)fallback)
		{
			return fallback.position;
		}
		if (!comp)
		{
			return Vector3.zero;
		}
		return comp.transform.position;
	}

	private Vector3 GetCompanionSpcLimitedTarget(Companion comp, Vector3 targetPos, float distance)
	{
		Vector3 position = comp.transform.position;
		float num = ((distance > 0f) ? distance : 6f);
		Vector3 vector = targetPos - position;
		if (vector.sqrMagnitude <= 0.0001f)
		{
			return new Vector3(position.x, position.y, 0f);
		}
		float num2 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		Vector3 vector2 = new Vector3(position.x + num * Mathf.Cos(num2 * ((float)Math.PI / 180f)), position.y + num * Mathf.Sin(num2 * ((float)Math.PI / 180f)), position.z);
		Vector3 vector3 = ((Vector3.Distance(targetPos, position) > Vector3.Distance(vector2, position)) ? vector2 : targetPos);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(position, targetPos - position, Vector2.Distance(position, targetPos), LayerMask.GetMask("block"));
		if ((bool)raycastHit2D.collider && raycastHit2D.collider.CompareTag("blockWALL"))
		{
			Vector3 vector4 = new Vector3(raycastHit2D.point.x, raycastHit2D.point.y, 0f);
			vector3 = ((Vector3.Distance(vector3, position) > Vector3.Distance(vector4, position)) ? vector4 : vector3);
		}
		return new Vector3(vector3.x, vector3.y, 0f);
	}

	public void CreatACT_Hit(string a, Enemy em, Vector3 angle)
	{
		foreach (ACT_SPC value in HIT.Values)
		{
			if (value.skillName == a && (float)UnityEngine.Random.Range(0, 101) < value.RateLast)
			{
				SkillOBJ_DT_SP skillOBJ_DT_SP;
				switch (value.FStype)
				{
				case 0:
					skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
					break;
				case 1:
					skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
					break;
				case 2:
					Debug.Log(value.OBJ);
					Debug.Log(value.MainEL);
					skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>();
					Debug.Log(15);
					break;
				case 3:
					skillOBJ_DT_SP = LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.headUp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>();
					break;
				default:
					skillOBJ_DT_SP = null;
					break;
				}
				if (value.RTtypeOBJ == 0)
				{
					float z = Mathf.Atan2(angle.y, angle.x) * 57.29578f;
					skillOBJ_DT_SP.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z);
				}
				SetSPCdata(skillOBJ_DT_SP, value);
			}
		}
	}

	public void CreatACT_Die(Enemy em)
	{
		foreach (ACT_SPC value in DIE.Values)
		{
			if ((float)UnityEngine.Random.Range(0, 101) < value.RateLast)
			{
				SetSPCdata(value.FStype switch
				{
					0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
					_ => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
				}, value);
			}
		}
	}

	public void CreatACT_Hurt(int multi)
	{
		if (HURT.Count == 0 || Time.time < actHurtTimer)
		{
			return;
		}
		actHurtTimer = Time.time + 0.2f;
		foreach (ACT_SPC value in HURT.Values)
		{
			if ((float)UnityEngine.Random.Range(0, 101) + PL.H_hurtR + PL.M_hurtR + (float)multi < value.RateLast)
			{
				SetSPCdata(value.FStype switch
				{
					0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.body.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					3 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.headUp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					_ => null, 
				}, value);
			}
		}
	}

	public void CreatACT_GD()
	{
		if (GD.Count == 0 || Time.time < actGdTimer)
		{
			return;
		}
		actGdTimer = Time.time + 0.2f;
		foreach (ACT_SPC value in GD.Values)
		{
			if ((float)UnityEngine.Random.Range(0, 101) < value.RateLast)
			{
				SetSPCdata(value.FStype switch
				{
					0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.body.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					3 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[value.OBJ].OBJ[value.MainEL], PL.headUp.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
					_ => null, 
				}, value);
			}
		}
	}

	public void SetSPCdata(SkillOBJ_DT_SP sp, ACT_SPC value)
	{
		SetSPCdata(sp, value, AimProvider.GetAimWorldPos());
	}

	public void SetSPCdata(SkillOBJ_DT_SP sp, ACT_SPC value, Vector3 targetPos)
	{
		sp.indexType = 0;
		sp.pl = SingletonMonoScope<PlayerManager>.Instance;
		sp.ZY = true;
		sp.Dot_Infect = false;
		sp.Dot_Infect_Layer = 0;
		targetPos.z = 0f;
		sp.TargetPos = targetPos;
		sp.skillName = value.Name;
		sp.ZQName = value.ZQName;
		sp.LockType = value.LockType;
		sp.RTtypeOBJ = value.RTtypeOBJ;
		sp.Distance = value.Distance;
		sp.GlobalID = value.GlobalID;
		sp.SPCindex = value.SPCindex;
		sp.SPCSlotIndex = value.SPCSlotIndex;
		sp.SPCItemCharType = value.SPCItemCharType;
		sp.SpecialType = value.SpecialType;
		sp.damageType = value.damageType;
		sp.MainEL = value.MainEL;
		sp.ThroughType = value.ThroughType;
		sp.AttackType = value.AttackType;
		sp.AttackTypeA = value.AttackTypeA;
		sp.AttackTypeB = value.AttackTypeB;
		sp.Damage = value.DamageLast * value.SPC_PRC * PL.GiveDamage(sp.damageType) / 100f;
		sp.DamageA = value.DamageA * value.SPC_PRC * PL.GiveDamage(sp.damageType) / 100f;
		sp.DamageB = value.DamageB * value.SPC_PRC * PL.GiveDamage(sp.damageType) / 100f;
		sp.SPC_Damage = value.DamageLast * value.SPC_PRC;
		sp.SPC_DamageA = value.DamageA * value.SPC_PRC;
		sp.SPC_DamageB = value.DamageB * value.SPC_PRC;
		sp.BJrate = PL.BJrate_Last;
		sp.BJDamage = PL.BJDamage_Last;
		sp.JYrate = PL.JYrate_Last;
		sp.Through = PL.ThroughRate;
		sp.FlySpeed = PL.FlySpeed;
		sp.MoveSpeedCut = 0f;
		sp.AttackSpeedCut = 0f;
		sp.AntiCut = 0f;
		sp.BF_Damage = 0f;
		sp.BF_EL_Damage = 0f;
		sp.BF_EL_Chuan = 0f;
		sp.BF_BJrate = 0f;
		sp.BF_JYrate = 0f;
		sp.BF_GeDang = 0f;
		sp.BF_AttackSpeed = 0f;
		sp.BF_MoveSpeed = 0f;
		sp.BF_DamageAnti = 0f;
		sp.BF_Health_Prc = 0f;
		sp.C_Damage = 0f;
		sp.C_ATspeed = 0f;
		sp.C_MVspeed = 0f;
		sp.C_Health_Prc = 0f;
		sp.CF_Rate = 0f;
		sp.BSAT = null;
		sp.Is_BS = 1;
		sp.ChangeSkin = 1;
		sp.SkinIndex = 0;
		sp.Reborn = 0;
		sp.NoTime = value.NoTime;
		sp.BuffTime = value.BuffTime;
		sp.DebuffTime = value.DebuffTime;
		sp.Field_time = value.Field_time;
		sp.ORB_time = value.ORB_time;
		sp.EXP_time = value.EXP_time;
		sp.ZD_time_F = value.ZD_time_F;
		sp.ZD_time_S = value.ZD_time_S;
		sp.Layer_SubA = value.Layer_SubA;
		sp.Layer_SubB = value.Layer_SubB;
		sp.ORB = value.ORB;
		sp.ZD_F = value.ZD_F;
		sp.ZD_S = value.ZD_S;
		sp.ZD_AB = value.ZD_AB;
		sp.EXP_F = value.EXP_F;
		sp.EXP_S = value.EXP_S;
		sp.EXP_AB = value.EXP_AB;
		sp.Dic_F = value.Dic_F;
		sp.Dic_S = value.Dic_S;
		sp.FX_F = value.FX_F;
		sp.FX_S = value.FX_S;
		sp.Sound = value.Sound;
		sp.Count_ORB = value.Count_ORB;
		sp.Count_ATtarget = value.Count_ATtarget;
		sp.ATtar_DMG = 0;
		sp.CF_Count = 0;
		sp.Count_F = value.Count_F;
		sp.Count_S = value.Count_S;
		sp.Count_AB = value.Count_A;
		sp.CountMulti = value.CountMulti;
		sp.CountEXP = value.CountEXP;
		sp.TypeORB = value.TypeORB;
		sp.CF_Type = 0;
		sp.Type_F = value.Type_F;
		sp.Type_S = value.Type_S;
		sp.Type_AB = value.Type_AB;
		sp.TypeDIC_F = value.TypeDIC_F;
		sp.TypeDIC_S = value.TypeDIC_S;
		sp.TypeEXP_F = value.TypeEXP_F;
		sp.TypeEXP_S = value.TypeEXP_S;
		sp.TypeEXP_AB = value.TypeEXP_AB;
		sp.Size = value.Size;
		sp.High = value.High;
		sp.JG = value.JG;
		sp.AngleA = value.AngleA;
		sp.AngleB = value.AngleB;
		sp.Range1 = value.Range1;
		sp.Range2 = value.Range2;
		sp.Range_AT = value.Range_AT;
		sp.FStime1 = value.FStime1;
		sp.FStime2 = value.FStime2;
		sp.Speed1 = value.Speed1;
		sp.Speed2 = value.Speed2;
		sp.Speed3 = value.Speed3;
		sp.Speed4 = value.Speed4;
		sp.Follow_F = value.Follow_F;
		sp.Follow_S = value.Follow_S;
		sp.AllChuan_F = value.AllChuan_F;
		sp.AllChuan_S = value.AllChuan_S;
		sp.Slow_F = value.Slow_F;
		sp.Slow_S = value.Slow_S;
		sp.RDSpeed_F = value.RDSpeed_F;
		sp.RDSpeed_S = value.RDSpeed_S;
		sp.HasFX = value.HasFX;
		sp.S_HasFX = value.S_HasFX;
		sp.AB_HasFX = value.A_HasFX;
		sp.colEXP = value.colEXP;
		sp.colEXP_A = value.colEXP_A;
		sp.S_colEXP = value.S_colEXP;
		sp.AB_colEXP = value.A_colEXP;
		sp.TimeEXP = value.TimeEXP;
		sp.TimeEXP_AB = value.TimeEXP_A;
		sp.LastEXP = value.LastEXP;
		sp.LastEXP_AB = value.LastEXP_A;
		sp.S_LastEXP = value.S_LastEXP;
		sp.AB_LastEXP = value.A_LastEXP;
		sp.EXPpos = value.EXPpos;
		sp.EXPpos_AB = value.EXPpos_A;
		sp.S_EXPpos = value.S_EXPpos;
		sp.AB_EXPpos = value.A_EXPpos;
		sp.AngleEXP = value.AngleEXP;
		sp.AngleEXP_AB = value.AngleEXP_A;
	}

	public void SetORBdata(SkillOBJ_DT_SP sp, SPC_MB value, int MainEL, int id, float SPC_PRC = 1f, int spcSlotIndex = -1, int itemCharType = -1)
	{
		sp.indexType = 0;
		sp.pl = SingletonMonoScope<PlayerManager>.Instance;
		sp.ZY = true;
		sp.Dot_Infect = false;
		sp.Dot_Infect_Layer = 0;
		sp.TargetPos = AimProvider.GetAimWorldPos();
		sp.skillName = value.SPCname;
		sp.ZQName = value.ZQName;
		sp.LockType = value.LockType;
		sp.RTtypeOBJ = value.RTtypeOBJ;
		sp.Distance = value.Distance;
		sp.GlobalID = id;
		sp.SPCindex = value.SPCindex;
		sp.SPCSlotIndex = spcSlotIndex;
		sp.SPCItemCharType = itemCharType;
		sp.SpecialType = value.SPCtype;
		sp.damageType = SWS.DMtype(MainEL);
		sp.MainEL = MainEL;
		sp.ThroughType = value.ThroughType;
		sp.AttackType = value.AttackType;
		sp.AttackTypeA = value.AttackTypeA;
		sp.AttackTypeB = value.AttackTypeB;
		sp.Damage = value.DamageLast * SPC_PRC / 100f * PL.GiveDamage(sp.damageType);
		sp.DamageA = value.DamageALast * SPC_PRC / 100f * PL.GiveDamage(sp.damageType);
		sp.DamageB = value.DamageBLast * SPC_PRC / 100f * PL.GiveDamage(sp.damageType);
		sp.SPC_Damage = value.DamageLast * SPC_PRC;
		sp.SPC_DamageA = value.DamageALast * SPC_PRC;
		sp.SPC_DamageB = value.DamageBLast * SPC_PRC;
		sp.BJrate = PL.BJrate_Last;
		sp.BJDamage = PL.BJDamage_Last;
		sp.JYrate = PL.JYrate_Last;
		sp.Through = PL.ThroughRate;
		sp.FlySpeed = PL.FlySpeed;
		sp.MoveSpeedCut = 0f;
		sp.AttackSpeedCut = 0f;
		sp.AntiCut = 0f;
		sp.BF_Damage = 0f;
		sp.BF_EL_Damage = 0f;
		sp.BF_EL_Chuan = 0f;
		sp.BF_BJrate = 0f;
		sp.BF_JYrate = 0f;
		sp.BF_GeDang = 0f;
		sp.BF_AttackSpeed = 0f;
		sp.BF_MoveSpeed = 0f;
		sp.BF_DamageAnti = 0f;
		sp.BF_Health_Prc = 0f;
		sp.C_Damage = 0f;
		sp.C_ATspeed = 0f;
		sp.C_MVspeed = 0f;
		sp.C_Health_Prc = 0f;
		sp.CF_Rate = 0f;
		sp.BSAT = null;
		sp.Is_BS = 1;
		sp.ChangeSkin = 1;
		sp.SkinIndex = 0;
		sp.Reborn = 0;
		sp.NoTime = value.NoTime;
		sp.BuffTime = value.BuffTime;
		sp.DebuffTime = value.DebuffTime;
		sp.Field_time = value.Field_time;
		sp.ORB_time = value.ORB_time;
		sp.EXP_time = value.EXP_time;
		sp.ZD_time_F = value.ZD_time_F;
		sp.ZD_time_S = value.ZD_time_S;
		sp.Layer_SubA = value.Layer_SubA;
		sp.Layer_SubB = value.Layer_SubB;
		sp.ORB = value.ORB;
		sp.ZD_F = value.ZD_F;
		sp.ZD_S = value.ZD_S;
		sp.ZD_AB = value.ZD_AB;
		sp.EXP_F = value.EXP_F;
		sp.EXP_S = value.EXP_S;
		sp.EXP_AB = value.EXP_AB;
		sp.Dic_F = value.Dic_F;
		sp.Dic_S = value.Dic_S;
		sp.FX_F = value.FX_F;
		sp.FX_S = value.FX_S;
		sp.Sound = value.Sound;
		sp.Count_ORB = value.Count_ORB;
		sp.Count_ATtarget = value.Count_ATtarget;
		sp.ATtar_DMG = 0;
		sp.CF_Count = 0;
		sp.Count_F = value.Count_F;
		sp.Count_S = value.Count_S;
		sp.Count_AB = value.Count_AB;
		sp.CountMulti = value.CountMulti;
		sp.CountEXP = value.CountEXP;
		sp.TypeORB = value.TypeORB;
		sp.CF_Type = 0;
		sp.Type_F = value.Type_F;
		sp.Type_S = value.Type_S;
		sp.Type_AB = value.Type_AB;
		sp.TypeDIC_F = value.TypeDIC_F;
		sp.TypeDIC_S = value.TypeDIC_S;
		sp.TypeEXP_F = value.TypeEXP_F;
		sp.TypeEXP_S = value.TypeEXP_S;
		sp.TypeEXP_AB = value.TypeEXP_AB;
		sp.Size = value.Size;
		sp.High = value.High;
		sp.JG = value.JG;
		sp.AngleA = value.AngleA;
		sp.AngleB = value.AngleB;
		sp.Range1 = value.Range1;
		sp.Range2 = value.Range2;
		sp.Range_AT = value.Range_AT;
		sp.FStime1 = value.FStime1;
		sp.FStime2 = value.FStime2;
		sp.Speed1 = value.Speed1;
		sp.Speed2 = value.Speed2;
		sp.Speed3 = value.Speed3;
		sp.Speed4 = value.Speed4;
		sp.Follow_F = value.Follow_F;
		sp.Follow_S = value.Follow_S;
		sp.AllChuan_F = value.AllChuan_F;
		sp.AllChuan_S = value.AllChuan_S;
		sp.Slow_F = value.Slow_F;
		sp.Slow_S = value.Slow_S;
		sp.RDSpeed_F = value.RDSpeed_F;
		sp.RDSpeed_S = value.RDSpeed_S;
		sp.HasFX = value.HasFX;
		sp.S_HasFX = value.S_HasFX;
		sp.AB_HasFX = value.AB_HasFX;
		sp.colEXP = value.colEXP;
		sp.colEXP_A = value.colEXP_A;
		sp.S_colEXP = value.S_colEXP;
		sp.AB_colEXP = value.AB_colEXP;
		sp.TimeEXP = value.TimeEXP;
		sp.TimeEXP_AB = value.TimeEXP_AB;
		sp.LastEXP = value.LastEXP;
		sp.LastEXP_AB = value.LastEXP_AB;
		sp.S_LastEXP = value.S_LastEXP;
		sp.AB_LastEXP = value.AB_LastEXP;
		sp.EXPpos = value.EXPpos;
		sp.EXPpos_AB = value.EXPpos_AB;
		sp.S_EXPpos = value.S_EXPpos;
		sp.AB_EXPpos = value.AB_EXPpos;
		sp.AngleEXP = value.AngleEXP;
		sp.AngleEXP_AB = value.AngleEXP_AB;
	}

	private static DamageType GetDotDamageType(int index)
	{
		return index switch
		{
			0 => DamageType.fire, 
			1 => DamageType.frozen, 
			2 => DamageType.thunder, 
			3 => DamageType.poison, 
			4 => DamageType.physics, 
			5 => DamageType.shadow, 
			_ => DamageType.fire, 
		};
	}

	private void EnsureDotRuntimeData()
	{
		if (DOT == null || DOT.Length != 6)
		{
			ACT_DOT[] dOT = DOT;
			DOT = new ACT_DOT[6];
			if (dOT != null)
			{
				int num = Mathf.Min(dOT.Length, 6);
				for (int i = 0; i < num; i++)
				{
					DOT[i] = dOT[i];
				}
			}
		}
		for (int j = 0; j < 6; j++)
		{
			if (DOT[j] == null)
			{
				DOT[j] = ACT_DOT.CreateDefault(GetDotDamageType(j));
			}
			DOT[j].damageType = GetDotDamageType(j);
		}
	}

	public void SetDot(SkillData_Dot_Father dt)
	{
		EnsureDotRuntimeData();
		if (dt.Level_Base <= 0)
		{
			return;
		}
		ACT_DOT[] dOT = DOT;
		foreach (ACT_DOT aCT_DOT in dOT)
		{
			if (aCT_DOT != null && aCT_DOT.damageType == dt.damageType)
			{
				aCT_DOT.Opened = true;
				aCT_DOT.Damage = dt.Damage_Max;
				aCT_DOT.Layer_Max = dt.Layer_Max;
				aCT_DOT.DOTrate = dt.DOTrate_Max;
				aCT_DOT.lifeTime = dt.Time_Max;
				aCT_DOT.ATSpeedCut = dt.ATSpeedCut_Last;
				aCT_DOT.MVSpeedCut = dt.MVSpeedCut_Last;
				aCT_DOT.ELAntiCut = dt.ELAntiCut;
				aCT_DOT.YunCut = dt.YunCut;
				aCT_DOT.DamageLow = dt.DamageLow;
				aCT_DOT.MSnumber = dt.MSnumber;
				aCT_DOT.MSrate = dt.MSrate;
				aCT_DOT.BoomDie_Rate = dt.BoomDie_Rate;
				aCT_DOT.BoomDie_Damage = dt.BoomDie_Damage;
				aCT_DOT.BoomDie_OBJ = dt.BoomDie_OBJ;
				aCT_DOT.BoomDie_Pos = dt.BoomDie_Pos;
				aCT_DOT.AttackType_BD = dt.AttackType_BD;
				aCT_DOT.Type_BD = dt.Type_BD;
				aCT_DOT.TypeDIC_BD = dt.TypeDIC_BD;
				aCT_DOT.TypeEXP_BD = dt.TypeEXP_BD;
				aCT_DOT.Range_BD = dt.Range_BD;
				aCT_DOT.SpeedMin_BD = dt.SpeedMin_BD;
				aCT_DOT.SpeedMax_BD = dt.SpeedMax_BD;
				aCT_DOT.Count_BD = dt.Count_BD;
				aCT_DOT.CountMulti_BD = dt.CountMulti_BD;
				aCT_DOT.BuffTime_BD = dt.BuffTime_BD;
				aCT_DOT.ZD_time_BD = dt.ZD_time_BD;
				aCT_DOT.ZD_BD = dt.ZD_BD;
				aCT_DOT.EXP_BD = dt.EXP_BD;
				aCT_DOT.Dic_BD = dt.Dic_BD;
				aCT_DOT.BoomJump_Rate = dt.BoomJump_Rate;
				aCT_DOT.BoomJump_Damage = dt.BoomJump_Damage;
				aCT_DOT.BoomJump_OBJ = dt.BoomJump_OBJ;
				aCT_DOT.BoomJump_Pos = dt.BoomJump_Pos;
				aCT_DOT.AttackType_BJ = dt.AttackType_BJ;
				aCT_DOT.Type_BJ = dt.Type_BJ;
				aCT_DOT.TypeDIC_BJ = dt.TypeDIC_BJ;
				aCT_DOT.TypeEXP_BJ = dt.TypeEXP_BJ;
				aCT_DOT.Range_BJ = dt.Range_BJ;
				aCT_DOT.SpeedMin_BJ = dt.SpeedMin_BJ;
				aCT_DOT.SpeedMax_BJ = dt.SpeedMax_BJ;
				aCT_DOT.Count_BJ = dt.Count_BJ;
				aCT_DOT.CountMulti_BJ = dt.CountMulti_BJ;
				aCT_DOT.BuffTime_BJ = dt.BuffTime_BJ;
				aCT_DOT.ZD_time_BJ = dt.ZD_time_BJ;
				aCT_DOT.ZD_BJ = dt.ZD_BJ;
				aCT_DOT.EXP_BJ = dt.EXP_BJ;
				aCT_DOT.Dic_BJ = dt.Dic_BJ;
				aCT_DOT.CutJump_Rate = dt.CutJump_Rate;
				aCT_DOT.CutJump_Damage = dt.CutJump_Damage;
				aCT_DOT.CutJump_OBJ = dt.CutJump_OBJ;
				aCT_DOT.CutJump_Pos = dt.CutJump_Pos;
				aCT_DOT.FrozenJump_Rate = dt.FrozenJump_Rate;
				aCT_DOT.FrozenJump_Time = dt.FrozenJump_Time;
			}
		}
	}

	public ACT_DOT GiveDot(DamageType type)
	{
		EnsureDotRuntimeData();
		ACT_DOT[] dOT = DOT;
		foreach (ACT_DOT aCT_DOT in dOT)
		{
			if (aCT_DOT != null && aCT_DOT.damageType == type)
			{
				return aCT_DOT;
			}
		}
		return DOT[Mathf.Clamp(SingletonMonoScope<PlayerManager>.HasInstance ? SingletonMonoScope<PlayerManager>.Instance.GiveInt(type) : 0, 0, 5)];
	}

	public void SetDotSart()
	{
		EnsureDotRuntimeData();
		ClearDotRuntimeData();
	}

	public void RebuildDotRuntimeDataFromTalent()
	{
		if (!SingletonMonoScope<TalentManager>.HasInstance || SingletonMonoScope<TalentManager>.Instance.XiData == null)
		{
			return;
		}
		for (int i = 0; i < SingletonMonoScope<TalentManager>.Instance.XiData.Length; i++)
		{
			SkillXiData skillXiData = SingletonMonoScope<TalentManager>.Instance.XiData[i];
			if (skillXiData == null)
			{
				continue;
			}
			if (skillXiData.Dot_F != null)
			{
				foreach (SkillData_Dot_Father value2 in skillXiData.Dot_F.Values)
				{
					if (value2 != null && value2.Level_Base > 0)
					{
						SetDot(value2);
					}
				}
			}
			if (skillXiData.Dot_S == null || skillXiData.Dot_F == null)
			{
				continue;
			}
			foreach (SkillData_Dot_Son value3 in skillXiData.Dot_S.Values)
			{
				if (value3 != null && value3.Level_Base > 0 && skillXiData.Dot_F.TryGetValue(value3.FatherSkill, out var value) && value != null)
				{
					SetDot(value);
				}
			}
		}
	}

	public static GameObject TakeDotFX(int a, Transform trans, int size)
	{
		return LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.DotFX[size].OBJ[a], trans.position, Quaternion.identity, trans);
	}

	public DOTobj TakeDotFX(Transform trans, DamageType type, int size)
	{
		switch (type)
		{
		case DamageType.fire:
		{
			GameObject obj6 = LeanPool.Spawn(_gameDataManager.SKPB.DotFX[size].OBJ[0], trans.position, Quaternion.identity, trans);
			obj6.GetComponent<DOTobj>().damageType = type;
			return obj6.GetComponent<DOTobj>();
		}
		case DamageType.frozen:
		{
			GameObject obj5 = LeanPool.Spawn(_gameDataManager.SKPB.DotFX[size].OBJ[1], trans.position, Quaternion.identity, trans);
			obj5.GetComponent<DOTobj>().damageType = type;
			return obj5.GetComponent<DOTobj>();
		}
		case DamageType.thunder:
		{
			GameObject obj4 = LeanPool.Spawn(_gameDataManager.SKPB.DotFX[size].OBJ[2], trans.position, Quaternion.identity, trans);
			obj4.GetComponent<DOTobj>().damageType = type;
			return obj4.GetComponent<DOTobj>();
		}
		case DamageType.poison:
		{
			GameObject obj3 = LeanPool.Spawn(_gameDataManager.SKPB.DotFX[size].OBJ[3], trans.position, Quaternion.identity, trans);
			obj3.GetComponent<DOTobj>().damageType = type;
			return obj3.GetComponent<DOTobj>();
		}
		case DamageType.physics:
		{
			GameObject obj2 = LeanPool.Spawn(_gameDataManager.SKPB.DotFX[size].OBJ[4], trans.position, Quaternion.identity, trans);
			obj2.GetComponent<DOTobj>().damageType = type;
			return obj2.GetComponent<DOTobj>();
		}
		case DamageType.shadow:
		{
			GameObject obj = LeanPool.Spawn(_gameDataManager.SKPB.DotFX[size].OBJ[5], trans.position, Quaternion.identity, trans);
			obj.GetComponent<DOTobj>().damageType = type;
			return obj.GetComponent<DOTobj>();
		}
		default:
			return null;
		}
	}

	public void TakeBoomDie(int i, Enemy em, int layer)
	{
		SkillOBJ_DT_SP skillOBJ_DT_SP = DOT[i].BoomDie_Pos switch
		{
			0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomDie_OBJ].OBJ[i], em.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomDie_OBJ].OBJ[i], em.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomDie_OBJ].OBJ[i], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
			_ => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomDie_OBJ].OBJ[i], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
		};
		skillOBJ_DT_SP.indexType = 0;
		skillOBJ_DT_SP.pl = PL;
		skillOBJ_DT_SP.ZY = true;
		skillOBJ_DT_SP.RTtypeOBJ = 1;
		skillOBJ_DT_SP.Distance = 0f;
		skillOBJ_DT_SP.GlobalID = 100000;
		skillOBJ_DT_SP.skillName = null;
		skillOBJ_DT_SP.ZQName = null;
		skillOBJ_DT_SP.Damage = DOT[i].BoomDie_Damage / 100f * PL.GiveDamage(DOT[i].damageType) * (1f + (float)PL.DOT[i].BoomDMGUp / 100f);
		skillOBJ_DT_SP.DamageA = 0f;
		skillOBJ_DT_SP.DamageB = 0f;
		skillOBJ_DT_SP.damageType = DOT[i].damageType;
		skillOBJ_DT_SP.MainEL = i;
		skillOBJ_DT_SP.Dot_Infect = PL.DOT[i].Dot_Infect;
		if (PL.DOT[i].Dot_Infect_All)
		{
			skillOBJ_DT_SP.Dot_Infect_Layer = layer;
		}
		else if (PL.DOT[i].Dot_Infect_Layer > 1)
		{
			skillOBJ_DT_SP.Dot_Infect_Layer = PL.DOT[i].Dot_Infect_Layer;
		}
		else
		{
			skillOBJ_DT_SP.Dot_Infect_Layer = 1;
		}
		skillOBJ_DT_SP.ThroughType = 0;
		skillOBJ_DT_SP.AttackType = DOT[i].AttackType_BD;
		skillOBJ_DT_SP.BJrate = PL.BJrate_Last;
		skillOBJ_DT_SP.BJDamage = PL.BJDamage_Last;
		skillOBJ_DT_SP.JYrate = PL.JYrate_Last;
		skillOBJ_DT_SP.Through = PL.ThroughRate;
		skillOBJ_DT_SP.FlySpeed = PL.FlySpeed;
		skillOBJ_DT_SP.MoveSpeedCut = 0f;
		skillOBJ_DT_SP.AttackSpeedCut = 0f;
		skillOBJ_DT_SP.AntiCut = 0f;
		skillOBJ_DT_SP.BuffTime = 0f;
		skillOBJ_DT_SP.BF_Damage = 0f;
		skillOBJ_DT_SP.BF_EL_Damage = 0f;
		skillOBJ_DT_SP.BF_EL_Chuan = 0f;
		skillOBJ_DT_SP.BF_BJrate = 0f;
		skillOBJ_DT_SP.BF_JYrate = 0f;
		skillOBJ_DT_SP.BF_GeDang = 0f;
		skillOBJ_DT_SP.BF_AttackSpeed = 0f;
		skillOBJ_DT_SP.BF_MoveSpeed = 0f;
		skillOBJ_DT_SP.BF_DamageAnti = 0f;
		skillOBJ_DT_SP.BF_Health_Prc = 0f;
		skillOBJ_DT_SP.C_Damage = 0f;
		skillOBJ_DT_SP.C_ATspeed = 0f;
		skillOBJ_DT_SP.C_MVspeed = 0f;
		skillOBJ_DT_SP.C_Health_Prc = 0f;
		skillOBJ_DT_SP.CF_Rate = 0f;
		skillOBJ_DT_SP.BSAT = null;
		skillOBJ_DT_SP.Is_BS = 0;
		skillOBJ_DT_SP.ChangeSkin = 1;
		skillOBJ_DT_SP.SkinIndex = 0;
		skillOBJ_DT_SP.Reborn = 0;
		skillOBJ_DT_SP.Type_F = DOT[i].Type_BD;
		skillOBJ_DT_SP.TypeDIC_F = DOT[i].TypeDIC_BD;
		skillOBJ_DT_SP.TypeEXP_F = DOT[i].TypeEXP_BD;
		skillOBJ_DT_SP.Size = 0f;
		skillOBJ_DT_SP.High = 0f;
		skillOBJ_DT_SP.JG = 0.1f;
		skillOBJ_DT_SP.Range1 = DOT[i].Range_BD;
		skillOBJ_DT_SP.Speed1 = DOT[i].SpeedMin_BD;
		skillOBJ_DT_SP.Speed2 = DOT[i].SpeedMax_BD;
		skillOBJ_DT_SP.Count_F = DOT[i].Count_BD;
		skillOBJ_DT_SP.CountMulti = DOT[i].CountMulti_BD;
		skillOBJ_DT_SP.CountEXP = 1;
		skillOBJ_DT_SP.NoTime = 1;
		skillOBJ_DT_SP.BuffTime = DOT[i].BuffTime_BD;
		skillOBJ_DT_SP.DebuffTime = 0f;
		skillOBJ_DT_SP.Field_time = 0f;
		skillOBJ_DT_SP.ZD_time_F = DOT[i].ZD_time_BD;
		skillOBJ_DT_SP.ZD_F = DOT[i].ZD_BD;
		skillOBJ_DT_SP.EXP_F = DOT[i].EXP_BD;
		skillOBJ_DT_SP.Dic_F = DOT[i].Dic_BD;
		skillOBJ_DT_SP.FX_F = 1;
		skillOBJ_DT_SP.Follow_F = 1;
		skillOBJ_DT_SP.AllChuan_F = 0;
		skillOBJ_DT_SP.Slow_F = 1;
		skillOBJ_DT_SP.RDSpeed_F = 1;
		skillOBJ_DT_SP.HasFX = 0;
		skillOBJ_DT_SP.colEXP = 1;
		skillOBJ_DT_SP.colEXP_A = 1;
		skillOBJ_DT_SP.TimeEXP = 1;
		skillOBJ_DT_SP.LastEXP = 1;
		skillOBJ_DT_SP.LastEXP_AB = 1;
	}

	public void TakeBoomJump(int i, Enemy em)
	{
		object obj = DOT[i].BoomJump_Pos switch
		{
			0 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomJump_OBJ].OBJ[i], em.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			1 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomJump_OBJ].OBJ[i], em.yao.transform.position, Quaternion.identity).GetComponent<SkillOBJ_DT_SP>(), 
			2 => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomJump_OBJ].OBJ[i], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
			_ => LeanPool.Spawn(_gameDataManager.SKPB.Skill[DOT[i].BoomJump_OBJ].OBJ[i], em.yao.transform.position, Quaternion.identity, em.yao.transform).GetComponent<SkillOBJ_DT_SP>(), 
		};
		((SkillOBJ_DT_SP)obj).indexType = 0;
		((SkillOBJ_DT_SP)obj).pl = PL;
		((SkillOBJ_DT_SP)obj).ZY = true;
		((SkillOBJ_DT_SP)obj).Dot_Infect = false;
		((SkillOBJ_DT_SP)obj).Dot_Infect_Layer = 0;
		((SkillOBJ_DT_SP)obj).RTtypeOBJ = 1;
		((SkillOBJ_DT_SP)obj).Distance = 0f;
		((SkillOBJ_DT_SP)obj).GlobalID = 100000;
		((SkillOBJ_DT_SP)obj).skillName = null;
		((SkillOBJ_DT_SP)obj).ZQName = null;
		((SkillOBJ_DT_SP)obj).Damage = DOT[i].BoomJump_Damage / 100f * PL.GiveDamage(DOT[i].damageType) * (1f + (float)PL.DOT[i].BoomDMGUp / 100f);
		((SkillOBJ_DT_SP)obj).DamageA = 0f;
		((SkillOBJ_DT_SP)obj).DamageB = 0f;
		((SkillOBJ_DT_SP)obj).damageType = DOT[i].damageType;
		((SkillOBJ_DT_SP)obj).MainEL = i;
		((SkillOBJ_DT_SP)obj).ThroughType = 0;
		((SkillOBJ_DT_SP)obj).AttackType = DOT[i].AttackType_BJ;
		((SkillOBJ_DT_SP)obj).BJrate = PL.BJrate_Last;
		((SkillOBJ_DT_SP)obj).BJDamage = PL.BJDamage_Last;
		((SkillOBJ_DT_SP)obj).JYrate = PL.JYrate_Last;
		((SkillOBJ_DT_SP)obj).Through = PL.ThroughRate;
		((SkillOBJ_DT_SP)obj).FlySpeed = PL.FlySpeed;
		((SkillOBJ_DT_SP)obj).MoveSpeedCut = 0f;
		((SkillOBJ_DT_SP)obj).AttackSpeedCut = 0f;
		((SkillOBJ_DT_SP)obj).AntiCut = 0f;
		((SkillOBJ_DT_SP)obj).BuffTime = 0f;
		((SkillOBJ_DT_SP)obj).BF_Damage = 0f;
		((SkillOBJ_DT_SP)obj).BF_EL_Damage = 0f;
		((SkillOBJ_DT_SP)obj).BF_EL_Chuan = 0f;
		((SkillOBJ_DT_SP)obj).BF_BJrate = 0f;
		((SkillOBJ_DT_SP)obj).BF_JYrate = 0f;
		((SkillOBJ_DT_SP)obj).BF_GeDang = 0f;
		((SkillOBJ_DT_SP)obj).BF_AttackSpeed = 0f;
		((SkillOBJ_DT_SP)obj).BF_MoveSpeed = 0f;
		((SkillOBJ_DT_SP)obj).BF_DamageAnti = 0f;
		((SkillOBJ_DT_SP)obj).BF_Health_Prc = 0f;
		((SkillOBJ_DT_SP)obj).C_Damage = 0f;
		((SkillOBJ_DT_SP)obj).C_ATspeed = 0f;
		((SkillOBJ_DT_SP)obj).C_MVspeed = 0f;
		((SkillOBJ_DT_SP)obj).C_Health_Prc = 0f;
		((SkillOBJ_DT_SP)obj).CF_Rate = 0f;
		((SkillOBJ_DT_SP)obj).BSAT = null;
		((SkillOBJ_DT_SP)obj).Is_BS = 0;
		((SkillOBJ_DT_SP)obj).ChangeSkin = 1;
		((SkillOBJ_DT_SP)obj).SkinIndex = 0;
		((SkillOBJ_DT_SP)obj).Reborn = 0;
		((SkillOBJ_DT_SP)obj).Type_F = DOT[i].Type_BJ;
		((SkillOBJ_DT_SP)obj).TypeDIC_F = DOT[i].TypeDIC_BJ;
		((SkillOBJ_DT_SP)obj).TypeEXP_F = DOT[i].TypeEXP_BJ;
		((SkillOBJ_DT_SP)obj).Size = 0f;
		((SkillOBJ_DT_SP)obj).High = 0f;
		((SkillOBJ_DT_SP)obj).JG = 0.1f;
		((SkillOBJ_DT_SP)obj).Range1 = DOT[i].Range_BJ;
		((SkillOBJ_DT_SP)obj).Speed1 = DOT[i].SpeedMin_BJ;
		((SkillOBJ_DT_SP)obj).Speed2 = DOT[i].SpeedMax_BJ;
		((SkillOBJ_DT_SP)obj).Count_F = DOT[i].Count_BJ;
		((SkillOBJ_DT_SP)obj).CountMulti = DOT[i].CountMulti_BJ;
		((SkillOBJ_DT_SP)obj).CountEXP = 1;
		((SkillOBJ_DT_SP)obj).NoTime = 1;
		((SkillOBJ_DT_SP)obj).BuffTime = DOT[i].BuffTime_BJ;
		((SkillOBJ_DT_SP)obj).DebuffTime = 0f;
		((SkillOBJ_DT_SP)obj).Field_time = 0f;
		((SkillOBJ_DT_SP)obj).ZD_time_F = DOT[i].ZD_time_BJ;
		((SkillOBJ_DT_SP)obj).ZD_F = DOT[i].ZD_BJ;
		((SkillOBJ_DT_SP)obj).EXP_F = DOT[i].EXP_BJ;
		((SkillOBJ_DT_SP)obj).Dic_F = DOT[i].Dic_BJ;
		((SkillOBJ_DT_SP)obj).FX_F = 1;
		((SkillOBJ_DT_SP)obj).Follow_F = 1;
		((SkillOBJ_DT_SP)obj).AllChuan_F = 0;
		((SkillOBJ_DT_SP)obj).Slow_F = 1;
		((SkillOBJ_DT_SP)obj).RDSpeed_F = 1;
		((SkillOBJ_DT_SP)obj).HasFX = 0;
		((SkillOBJ_DT_SP)obj).colEXP = 1;
		((SkillOBJ_DT_SP)obj).colEXP_A = 1;
		((SkillOBJ_DT_SP)obj).TimeEXP = 1;
		((SkillOBJ_DT_SP)obj).LastEXP = 1;
		((SkillOBJ_DT_SP)obj).LastEXP_AB = 1;
	}

	public void TakeCutJump(int a, Enemy em)
	{
		switch (DOT[a].CutJump_Pos)
		{
		case 0:
			LeanPool.Spawn(_gameDataManager.SKPB.CutJump[DOT[a].CutJump_OBJ], em.transform.position, Quaternion.identity);
			break;
		case 1:
			LeanPool.Spawn(_gameDataManager.SKPB.CutJump[DOT[a].CutJump_OBJ], em.yao.transform.position, Quaternion.identity);
			break;
		case 2:
			LeanPool.Spawn(_gameDataManager.SKPB.CutJump[DOT[a].CutJump_OBJ], em.yao.transform.position, Quaternion.identity, em.yao.transform);
			break;
		}
	}

	public void TakeFrozen(Transform trans, int size)
	{
		switch (size)
		{
		case 0:
			LeanPool.Spawn(_gameDataManager.SKPB.FrozenFX[0], trans.position, Quaternion.identity, trans);
			break;
		case 1:
			LeanPool.Spawn(_gameDataManager.SKPB.FrozenFX[1], trans.position, Quaternion.identity, trans);
			break;
		case 2:
			LeanPool.Spawn(_gameDataManager.SKPB.FrozenFX[2], trans.position, Quaternion.identity, trans);
			break;
		}
	}

	public void CompCountPlus(string index, Companion comp)
	{
		if (!comp)
		{
			LogUtil.Error("在计数已召唤同伴数量时获取的同伴为空");
			return;
		}
		ApplyCompanionUniverseSPC(comp);
		ACTListSkillBT aCTListSkillBT = CheckListSkill(index);
		if ((bool)aCTListSkillBT)
		{
			if (aCTListSkillBT.cpList == null)
			{
				aCTListSkillBT.cpList = new List<Companion>();
			}
			if (aCTListSkillBT.DT?.comp == null)
			{
				LogUtil.Error("在计数已召唤同伴数量时技能 " + index + " 没有召唤配置");
				return;
			}
			ValidateCompCount(index);
			aCTListSkillBT.cpList.Add(comp);
			ValidateCompCount(index);
		}
	}

	public void ValidateCompCount(string index)
	{
		ACTListSkillBT aCTListSkillBT = CheckListSkill(index);
		if (!aCTListSkillBT || aCTListSkillBT.cpList == null)
		{
			return;
		}
		if (aCTListSkillBT.DT?.comp == null)
		{
			LogUtil.Error("在校验同伴数量时技能 " + index + " 没有召唤配置");
			return;
		}
		int summon_count = aCTListSkillBT.DT.comp.Summon_count;
		for (int num = aCTListSkillBT.cpList.Count - 1; num >= 0; num--)
		{
			Companion companion = aCTListSkillBT.cpList[num];
			if (!companion || companion.IsDead)
			{
				aCTListSkillBT.cpList.RemoveAt(num);
			}
		}
		while (aCTListSkillBT.cpList.Count > summon_count)
		{
			Companion companion2 = aCTListSkillBT.cpList[0];
			aCTListSkillBT.cpList.RemoveAt(0);
			if ((bool)companion2 && !companion2.IsDead)
			{
				companion2.SystemDelete();
			}
		}
	}

	public void RemoveFromCompList(string index, Companion comp)
	{
		CheckListSkill(index).cpList.Remove(comp);
	}

	public void RestoreSkill()
	{
		ClearDotRuntimeData();
		ResetATPrefabState();
		if (SkillBuffList != null)
		{
			for (int num = SkillBuffList.Count - 1; num >= 0; num--)
			{
				SK_BuffA sK_BuffA = SkillBuffList[num];
				if ((bool)sK_BuffA)
				{
					sK_BuffA.StopBuff();
				}
			}
			SkillBuffList.Clear();
		}
		if ((bool)PL && (bool)PL.BuffMG)
		{
			PL.BuffMG.ClearSkillBuff();
		}
		PL?.BuffRuntime?.ClearAllRuntimeBuffs();
		SetSkillSart();
		ClearListSkill();
	}

	public void ClearDotRuntimeData()
	{
		EnsureDotRuntimeData();
		for (int i = 0; i < DOT.Length; i++)
		{
			ACT_DOT aCT_DOT = DOT[i];
			if (aCT_DOT != null)
			{
				aCT_DOT.Opened = false;
				aCT_DOT.Layer_Max = 0;
				aCT_DOT.DOTrate = 0f;
				aCT_DOT.Damage = 0f;
				aCT_DOT.lifeTime = 0f;
				aCT_DOT.ATSpeedCut = 0f;
				aCT_DOT.MVSpeedCut = 0f;
				aCT_DOT.ELAntiCut = 0f;
				aCT_DOT.YunCut = 0f;
				aCT_DOT.DamageLow = 0f;
				aCT_DOT.MSnumber = 0f;
				aCT_DOT.MSrate = 0f;
				aCT_DOT.BoomDie_Rate = 0f;
				aCT_DOT.BoomDie_Damage = 0f;
				aCT_DOT.BoomDie_OBJ = 0;
				aCT_DOT.BoomDie_Pos = 0;
				aCT_DOT.AttackType_BD = false;
				aCT_DOT.Type_BD = 0;
				aCT_DOT.TypeDIC_BD = 0;
				aCT_DOT.TypeEXP_BD = 0;
				aCT_DOT.Range_BD = 0f;
				aCT_DOT.SpeedMin_BD = 0f;
				aCT_DOT.SpeedMax_BD = 0f;
				aCT_DOT.Count_BD = 0;
				aCT_DOT.CountMulti_BD = 0;
				aCT_DOT.BuffTime_BD = 0f;
				aCT_DOT.ZD_time_BD = 0f;
				aCT_DOT.ZD_BD = 0;
				aCT_DOT.EXP_BD = 0;
				aCT_DOT.Dic_BD = 0;
				aCT_DOT.BoomJump_Rate = 0f;
				aCT_DOT.BoomJump_Damage = 0f;
				aCT_DOT.BoomJump_OBJ = 0;
				aCT_DOT.BoomJump_Pos = 0;
				aCT_DOT.AttackType_BJ = false;
				aCT_DOT.Type_BJ = 0;
				aCT_DOT.TypeDIC_BJ = 0;
				aCT_DOT.TypeEXP_BJ = 0;
				aCT_DOT.Range_BJ = 0f;
				aCT_DOT.SpeedMin_BJ = 0f;
				aCT_DOT.SpeedMax_BJ = 0f;
				aCT_DOT.Count_BJ = 0;
				aCT_DOT.CountMulti_BJ = 0;
				aCT_DOT.BuffTime_BJ = 0f;
				aCT_DOT.ZD_time_BJ = 0f;
				aCT_DOT.ZD_BJ = 0;
				aCT_DOT.EXP_BJ = 0;
				aCT_DOT.Dic_BJ = 0;
				aCT_DOT.CutJump_Rate = 0f;
				aCT_DOT.CutJump_Damage = 0f;
				aCT_DOT.CutJump_OBJ = 0;
				aCT_DOT.CutJump_Pos = 0;
				aCT_DOT.FrozenJump_Rate = 0f;
				aCT_DOT.FrozenJump_Time = 0f;
			}
		}
		if (DOT.Length != 0 && DOT[0] != null)
		{
			DOT[0].damageType = DamageType.fire;
		}
		if (DOT.Length > 1 && DOT[1] != null)
		{
			DOT[1].damageType = DamageType.frozen;
		}
		if (DOT.Length > 2 && DOT[2] != null)
		{
			DOT[2].damageType = DamageType.thunder;
		}
		if (DOT.Length > 3 && DOT[3] != null)
		{
			DOT[3].damageType = DamageType.poison;
		}
		if (DOT.Length > 4 && DOT[4] != null)
		{
			DOT[4].damageType = DamageType.physics;
		}
		if (DOT.Length > 5 && DOT[5] != null)
		{
			DOT[5].damageType = DamageType.shadow;
		}
	}

	public int GetCP_CT()
	{
		return GetAliveCompTotalCount();
	}

	public int GetCPClass_CT()
	{
		if (actListSkill == null)
		{
			return 0;
		}
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if (!aCTListSkillBT || aCTListSkillBT.DT == null || aCTListSkillBT.DT.type != 1 || aCTListSkillBT.cpList == null)
			{
				continue;
			}
			for (int j = 0; j < aCTListSkillBT.cpList.Count; j++)
			{
				Companion companion = aCTListSkillBT.cpList[j];
				if (IsActiveCompanion(companion))
				{
					string text = ((!string.IsNullOrEmpty(companion.Name)) ? companion.Name : aCTListSkillBT.IndexName);
					if (!string.IsNullOrEmpty(text))
					{
						hashSet.Add(text);
					}
				}
			}
		}
		return hashSet.Count;
	}

	public int GetEveryCompDMG()
	{
		int num = 0;
		int aliveCompTotalCount = GetAliveCompTotalCount();
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if ((bool)aCTListSkillBT && aCTListSkillBT.DT != null)
			{
				if (aCTListSkillBT.DT.type == 1 && aCTListSkillBT.DT.comp.EveryDMG != 0)
				{
					num += aCTListSkillBT.DT.comp.EveryDMG * GetAliveCompCount(aCTListSkillBT);
				}
				else if (aCTListSkillBT.DT.type == 0 && aCTListSkillBT.DT.simple.CompUP_DMG != 0)
				{
					num += aCTListSkillBT.DT.simple.CompUP_DMG * aliveCompTotalCount;
				}
			}
		}
		return num;
	}

	public int GetEveryCompChuan()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryChuan + skill.EveryAllChuan);
	}

	public int GetEveryCompATS()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryATS);
	}

	public int GetEveryCompMVS()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryMVS);
	}

	public int GetEveryCompHeal()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryHeal);
	}

	public int GetEveryCompMana()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryMana);
	}

	public int GetEveryCompCD()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryCD);
	}

	public int GetEveryCompBJR()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryBJR);
	}

	public int GetEveryCompBJD()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryBJD);
	}

	public int GetEveryCompGD()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryGD);
	}

	public int GetEveryCompDMG_Anti()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryDMG_Anti);
	}

	public int GetEveryCompDotTimeCut()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryDotTimeCut);
	}

	public int GetEveryCompAllAnti()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryAllAnti);
	}

	public int GetEveryCompDrop()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryDrop);
	}

	public int GetEveryCompXJ_DMG()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryXJ_DMG);
	}

	public int GetEveryCompORB_DMG()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryORB_DMG);
	}

	public int GetEveryCompDot_DMG()
	{
		return GetEveryCompStat((ACT_skillComp skill) => skill.EveryDot_DMG);
	}

	private int GetEveryCompStat(Func<ACT_skillComp, int> selector)
	{
		int num = 0;
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if ((bool)aCTListSkillBT && aCTListSkillBT.DT != null && aCTListSkillBT.DT.type == 1)
			{
				int num2 = selector(aCTListSkillBT.DT.comp);
				if (num2 != 0)
				{
					num += num2 * GetAliveCompCount(aCTListSkillBT);
				}
			}
		}
		return num;
	}

	private static int GetAliveCompCount(ACTListSkillBT skill)
	{
		if (!skill || skill.cpList == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < skill.cpList.Count; i++)
		{
			if (IsActiveCompanion(skill.cpList[i]))
			{
				num++;
			}
		}
		return num;
	}

	private int GetAliveCompTotalCount()
	{
		if (actListSkill == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = actListSkill[i];
			if ((bool)aCTListSkillBT && aCTListSkillBT.DT != null && aCTListSkillBT.DT.type == 1)
			{
				num += GetAliveCompCount(aCTListSkillBT);
			}
		}
		return num;
	}

	private static bool IsActiveCompanion(Companion comp)
	{
		if ((bool)comp && comp.IsAlive)
		{
			return comp.gameObject.activeInHierarchy;
		}
		return false;
	}

	public void CP_DMGsplit(float damage, DamageType type, Enemy em)
	{
		int num = 0;
		for (int i = 0; i < actListSkill.Count; i++)
		{
			if (actListSkill[i].DT.type == 1)
			{
				for (int j = 0; j < actListSkill[i].cpList.Count; j++)
				{
					num++;
				}
			}
		}
		for (int k = 0; k < actListSkill.Count; k++)
		{
			if (actListSkill[k].DT.type == 1)
			{
				for (int l = 0; l < actListSkill[k].cpList.Count; l++)
				{
					actListSkill[k].cpList[k].TakeDamage(damage / (float)num, 0f, 0f, 0f, 0f, type, em);
				}
			}
		}
	}
}
