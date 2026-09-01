using System;
using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Inputs;
using Inputs.Cursors;
using Interact;
using PoedbMod;
using Scenes;
using UI.Managers;
using UI.Panels;
using UI.UIItems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUIManager : SingletonMonoScope<GameUIManager>
{
	public VirtualCursorItem virtualCursor;

	public GameObject compListUI;

	public GameObject itemTipList;

	public GameObject buffTip;

	public CanvasGroup coverAll;

	public GameObject char_cover;

	public GameObject splitBaoshiTip;

	public GameObject splitBaoshiIcon;

	public GameObject weaponSpcIcon;

	public GameObject weaponElmIcon;

	public GameObject weaponEnhIcon;

	[HideInInspector]
	public bool Opened_warehouse;

	[HideInInspector]
	public bool Opened_shop;

	[HideInInspector]
	public bool Opened_Character;

	[HideInInspector]
	public bool Opened_IV;

	[HideInInspector]
	public bool Opened_Talent;

	[HideInInspector]
	public bool Opened_weapon;

	[HideInInspector]
	public bool Opened_baoshi;

	public CanvasGroup[] BottomCAV;

	public SettingBT[] setBT;

	public Stat Health;

	public Stat Mana;

	public XpStat XP;

	public XpStat_DF DFXP;

	[HideInInspector]
	public CanvasGroup EmptyCAV;

	[HideInInspector]
	public Text Empty_mainA;

	[HideInInspector]
	public Text Empty_mainB;

	public CanvasGroup EnemyTipCAV;

	public CanvasGroup BossTipCAV;

	private PlayerManager PL;

	public GameObject Minimap;

	public PosDisplayPanel PosDisplay;

	public CanvasGroup LevelCav;

	public Text LevelText;

	[HideInInspector]
	public CanvasGroup WeaponCavA;

	[HideInInspector]
	public RectTransform WP_RectA;

	[HideInInspector]
	public Text WP_titleA;

	[HideInInspector]
	public Text WP_typeA;

	[HideInInspector]
	public Text WP_levelA;

	[HideInInspector]
	public Text WP_mainA;

	[HideInInspector]
	public Image WP_doubleIconA;

	[HideInInspector]
	public Text WP_dotA;

	[HideInInspector]
	public Text WP_skA;

	[HideInInspector]
	public Text WP_cpA;

	[HideInInspector]
	public Text WP_specialA;

	[HideInInspector]
	public Text WP_special2A;

	[HideInInspector]
	public Text WP_setNameA;

	[HideInInspector]
	public Text WP_setMainA;

	[HideInInspector]
	public Text WP_fwA;

	[HideInInspector]
	public GameObject WP_fwObjA;

	public GameObject[] skillOBJA;

	public Text[] skillTextA;

	[HideInInspector]
	public GameObject WP_lineA_A;

	[HideInInspector]
	public GameObject WP_lineA_B;

	[HideInInspector]
	public GameObject WP_lineA_C;

	[HideInInspector]
	public GameObject WP_lineA_D;

	[HideInInspector]
	public GameObject WP_lineA_E;

	[HideInInspector]
	public GameObject WP_lineA_F;

	[HideInInspector]
	public GameObject WP_lineA_G;

	[HideInInspector]
	public GameObject WP_lineA_H;

	[HideInInspector]
	public GameObject WP_lineA_I;

	public GameObject[] WP_baoshiOBJA;

	public Text[] WP_baoshiA;

	public Image[] pic_aocaoA;

	public Image[] pic_baoshiA;

	[HideInInspector]
	public Text WP_YJ;

	[HideInInspector]
	public Text WP_Set;

	[HideInInspector]
	public GameObject priceA;

	[HideInInspector]
	public Text WP_priceA;

	[HideInInspector]
	public CanvasGroup WeaponCavB;

	[HideInInspector]
	public RectTransform WP_RectB;

	[HideInInspector]
	public Text WP_titleB;

	[HideInInspector]
	public Text WP_typeB;

	[HideInInspector]
	public Text WP_levelB;

	[HideInInspector]
	public Text WP_mainB;

	[HideInInspector]
	public Image WP_doubleIconB;

	[HideInInspector]
	public Text WP_dotB;

	[HideInInspector]
	public Text WP_skB;

	[HideInInspector]
	public Text WP_cpB;

	[HideInInspector]
	public Text WP_specialB;

	[HideInInspector]
	public Text WP_special2B;

	[HideInInspector]
	public Text WP_setNameB;

	[HideInInspector]
	public Text WP_setMainB;

	[HideInInspector]
	public Text WP_fwB;

	[HideInInspector]
	public GameObject WP_fwObjB;

	public GameObject[] skillOBJB;

	public Text[] skillTextB;

	[HideInInspector]
	public GameObject WP_lineB_A;

	[HideInInspector]
	public GameObject WP_lineB_B;

	[HideInInspector]
	public GameObject WP_lineB_C;

	[HideInInspector]
	public GameObject WP_lineB_D;

	[HideInInspector]
	public GameObject WP_lineB_E;

	[HideInInspector]
	public GameObject WP_lineB_F;

	[HideInInspector]
	public GameObject WP_lineB_G;

	[HideInInspector]
	public GameObject WP_lineB_H;

	[HideInInspector]
	public GameObject WP_lineB_I;

	public GameObject[] WP_baoshiOBJB;

	public Text[] WP_baoshiB;

	public Image[] pic_aocaoB;

	public Image[] pic_baoshiB;

	[HideInInspector]
	public Text WP_priceB;

	[HideInInspector]
	public Text WP_equiped;

	[HideInInspector]
	public CanvasGroup SkillCAV;

	[HideInInspector]
	public RectTransform Skill_Rect;

	[HideInInspector]
	public Text Skill_title;

	[HideInInspector]
	public GameObject Skill_cost;

	[HideInInspector]
	public Text Skill_mana;

	[HideInInspector]
	public Text Skill_cd;

	[HideInInspector]
	public Text Skill_time;

	[HideInInspector]
	public Text Skill_main;

	[HideInInspector]
	public Text Skill_next;

	[HideInInspector]
	public Text Skill_type;

	[HideInInspector]
	public Text Skill_unlock;

	[HideInInspector]
	public GameObject Skill_lineA;

	[HideInInspector]
	public GameObject Skill_lineB;

	[HideInInspector]
	public GameObject Skill_lineC;

	[HideInInspector]
	public GameObject Skill_lineD;

	[HideInInspector]
	public CanvasGroup XiCAV;

	[HideInInspector]
	public Text Xi_title;

	[HideInInspector]
	public Text Xi_level;

	[HideInInspector]
	public Text Xi_element;

	[HideInInspector]
	public Text Xi_mainA;

	[HideInInspector]
	public Text Xi_mainB;

	[HideInInspector]
	public GameObject Xi_cost;

	[HideInInspector]
	public GameObject Xi_lineA;

	public CanvasGroup DFList_UI;

	public CanvasGroup DFTip_UI;

	public Sprite IconADD;

	private Transform _dfTipTarget;

	private int _dfTipIndex = -1;

	private bool _dfSkillListOpen;

	private int _dfSkillListTargetIndex = -1;

	private SKillBT_DF _dfSkillListTarget;

	private readonly List<RaycastResult> _dfSkillListRaycastResults = new List<RaycastResult>();

	[HideInInspector]
	public CanvasGroup DFCAV;

	[HideInInspector]
	public Text DF_title;

	[HideInInspector]
	public Text DF_level;

	[HideInInspector]
	public Text DF_element;

	[HideInInspector]
	public Text DF_mainA;

	[HideInInspector]
	public Text DF_mainB;

	[SerializeField]
	private CanvasGroup mijingGroup;

	[SerializeField]
	private Text floorText;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Image fillImg;

	private Text splitTipBaoshiText;

	private Text splitTipPriceText;

	private CanvasGroup splitCanvasGroup;

	[Header("宝石拆卸Tip跟随设置")]
	[SerializeField]
	private Vector2 splitTipOffset = new Vector2(-20f, 20f);

	[SerializeField]
	private float splitTipScreenPadding = 10f;

	private RectTransform splitTipRect;

	private bool isAnyPanelOpen;

	private bool wasAnyPanelOpenedLastFrame;

	[Header("Weapon Tip Position")]
	[SerializeField]
	private float tipSideOffsetX = 12f;

	[SerializeField]
	private float tipSideOffsetY;

	[SerializeField]
	private float compareTipSpacing = 10f;

	[SerializeField]
	private float screenPadding = 5f;

	public GlobalUiModalState CurrentModalState { get; private set; }

	public bool IsInModalState => CurrentModalState != GlobalUiModalState.None;

	public bool IsDFSkillListOpen => _dfSkillListOpen;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		SetUIStart();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		if (!Minimap)
		{
			Minimap = base.transform.Find("UICanvas/Minimap").gameObject;
		}
		if (!PosDisplay)
		{
			PosDisplay = base.transform.Find("UICanvas/posNameUI").gameObject.GetComponent<PosDisplayPanel>();
		}
		CanvasGroup[] bottomCAV = BottomCAV;
		for (int i = 0; i < bottomCAV.Length; i++)
		{
			bottomCAV[i].alpha = 0f;
		}
	}

	private void Start()
	{
		Opened_warehouse = false;
		Opened_shop = false;
		Opened_Character = false;
		Opened_IV = false;
		Opened_Talent = false;
		Opened_weapon = false;
		Opened_baoshi = false;
	}

	private void Update()
	{
		if (SingletonMonoGlobal<SceneFadeManager>.HasInstance && SingletonMonoGlobal<SceneFadeManager>.Instance.fadeCanvasGroup.alpha > 0.2f)
		{
			return;
		}
		HandleDFSkillListOutsideClick();
		skillBTlight();
		isAnyPanelOpen = false;
		CanvasGroup[] bottomCAV = BottomCAV;
		for (int i = 0; i < bottomCAV.Length; i++)
		{
			if (Mathf.Approximately(bottomCAV[i].alpha, 1f) || Singleton<UIManager>.Instance.IsPanelOpened<TeleportPanel>() || Singleton<UIManager>.Instance.IsPanelOpened<MijingPanel>() || Opened_shop)
			{
				isAnyPanelOpen = true;
				break;
			}
		}
		if (!TryHandleModalCancel())
		{
			if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				HandleGamepadInputBack();
			}
			else
			{
				HandlePcInputEsc();
			}
			if (CurrentModalState == GlobalUiModalState.BaoshiSplit)
			{
				UpdateSplitBaoshiTipPosition();
			}
			RefreshGameplayInputAfterPanelStateChanged();
		}
	}

	private void HandlePcInputEsc()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !GamepadUIActionManager.IsCancelConsumedThisFrame())
		{
			if (Singleton<UIManager>.Instance.IsPanelOpened<PausePanel>())
			{
				GamepadUIActionManager.ConsumeCancelForCurrentFrame();
				PausePanel.CloseAndResume();
			}
			else if (TryExitCurrentModalState())
			{
				GamepadUIActionManager.ConsumeCancelForCurrentFrame();
			}
			else if (isAnyPanelOpen)
			{
				GamepadUIActionManager.ConsumeCancelForCurrentFrame();
				CloseMainPanels();
			}
			else if (!Singleton<UIManager>.Instance.IsPanelOpened<SettingPanel>())
			{
				GamepadUIActionManager.ConsumeCancelForCurrentFrame();
				OpenClose_Pause();
			}
		}
	}

	private void HandleGamepadInputBack()
	{
		if (GamepadInputManager.GetMenuDown() && !TryExitCurrentModalState())
		{
			if (isAnyPanelOpen)
			{
				CloseMainPanels();
			}
			else if (!Singleton<UIManager>.Instance.IsPanelOpened<SettingPanel>())
			{
				OpenClose_Pause();
			}
		}
	}

	private bool TryHandleModalCancel()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			if (!GamepadInputManager.GetCancelDown())
			{
				return false;
			}
		}
		else if (!Input.GetMouseButtonDown(1))
		{
			return false;
		}
		return TryExitCurrentModalState();
	}

	private static Text FindOptionalText(Transform root, string path)
	{
		Transform transform = (root ? root.Find(path) : null);
		if (!transform)
		{
			return null;
		}
		return transform.GetComponent<Text>();
	}

	private static Text FindOptionalChildText(Transform root, string path)
	{
		Transform transform = (root ? root.Find(path) : null);
		if (!transform)
		{
			return null;
		}
		Transform transform2 = transform.Find("Text");
		Text text = (transform2 ? transform2.GetComponent<Text>() : null);
		if (!text)
		{
			return transform.GetComponentInChildren<Text>(includeInactive: true);
		}
		return text;
	}

	private static GameObject FindOptionalObject(Transform root, string path)
	{
		Transform transform = (root ? root.Find(path) : null);
		if (!transform)
		{
			return null;
		}
		return transform.gameObject;
	}

	private static Image FindOptionalImage(Transform root, string path)
	{
		Transform transform = (root ? root.Find(path) : null);
		if (!transform)
		{
			return null;
		}
		return transform.GetComponent<Image>();
	}

	public void SetUIStart()
	{
		WeaponCavA = base.transform.Find("UICanvas/WeaponTipA").GetComponent<CanvasGroup>();
		WP_RectA = base.transform.Find("UICanvas/WeaponTipA").GetComponent<RectTransform>();
		WP_titleA = WeaponCavA.transform.Find("title").GetComponent<Text>();
		WP_typeA = WeaponCavA.transform.Find("zhiye/type").GetComponent<Text>();
		WP_levelA = WeaponCavA.transform.Find("zhiye/level").GetComponent<Text>();
		WP_mainA = WeaponCavA.transform.Find("main").GetComponent<Text>();
		WP_doubleIconA = FindOptionalImage(WeaponCavA.transform, "main/Image");
		WP_dotA = FindOptionalText(WeaponCavA.transform, "dot");
		WP_skA = FindOptionalText(WeaponCavA.transform, "sk");
		WP_cpA = FindOptionalText(WeaponCavA.transform, "cp");
		WP_specialA = WeaponCavA.transform.Find("special").GetComponent<Text>();
		WP_special2A = FindOptionalText(WeaponCavA.transform, "special2");
		WP_setNameA = FindOptionalText(WeaponCavA.transform, "SetName");
		WP_setMainA = FindOptionalText(WeaponCavA.transform, "SetMain");
		WP_fwObjA = FindOptionalObject(WeaponCavA.transform, "FW");
		WP_fwA = FindOptionalChildText(WeaponCavA.transform, "FW");
		WP_lineA_A = WeaponCavA.transform.Find("lineA").gameObject;
		WP_lineA_B = WeaponCavA.transform.Find("lineB").gameObject;
		WP_lineA_C = WeaponCavA.transform.Find("lineC").gameObject;
		WP_lineA_D = WeaponCavA.transform.Find("lineD").gameObject;
		WP_lineA_E = WeaponCavA.transform.Find("lineE").gameObject;
		WP_lineA_F = WeaponCavA.transform.Find("lineF").gameObject;
		WP_lineA_G = FindOptionalObject(WeaponCavA.transform, "lineG");
		WP_lineA_H = FindOptionalObject(WeaponCavA.transform, "lineH");
		WP_lineA_I = FindOptionalObject(WeaponCavA.transform, "lineI");
		WP_YJ = WeaponCavA.transform.Find("YJ").GetComponent<Text>();
		WP_Set = WeaponCavA.transform.Find("YJ/Set").GetComponent<Text>();
		WP_priceA = WeaponCavA.transform.Find("price/price").GetComponent<Text>();
		priceA = WeaponCavA.transform.Find("price").gameObject;
		WeaponCavB = base.transform.Find("UICanvas/WeaponTipB").GetComponent<CanvasGroup>();
		WP_RectB = base.transform.Find("UICanvas/WeaponTipB").GetComponent<RectTransform>();
		WP_titleB = WeaponCavB.transform.Find("title").GetComponent<Text>();
		WP_typeB = WeaponCavB.transform.Find("zhiye/type").GetComponent<Text>();
		WP_levelB = WeaponCavB.transform.Find("zhiye/level").GetComponent<Text>();
		WP_mainB = WeaponCavB.transform.Find("main").GetComponent<Text>();
		WP_doubleIconB = FindOptionalImage(WeaponCavB.transform, "main/Image");
		WP_dotB = FindOptionalText(WeaponCavB.transform, "dot");
		WP_skB = FindOptionalText(WeaponCavB.transform, "sk");
		WP_cpB = FindOptionalText(WeaponCavB.transform, "cp");
		WP_specialB = WeaponCavB.transform.Find("special").GetComponent<Text>();
		WP_special2B = FindOptionalText(WeaponCavB.transform, "special2");
		WP_setNameB = FindOptionalText(WeaponCavB.transform, "SetName");
		WP_setMainB = FindOptionalText(WeaponCavB.transform, "SetMain");
		WP_fwObjB = FindOptionalObject(WeaponCavB.transform, "FW");
		WP_fwB = FindOptionalChildText(WeaponCavB.transform, "FW");
		WP_lineB_A = WeaponCavB.transform.Find("lineA").gameObject;
		WP_lineB_B = WeaponCavB.transform.Find("lineB").gameObject;
		WP_lineB_C = WeaponCavB.transform.Find("lineC").gameObject;
		WP_lineB_D = WeaponCavB.transform.Find("lineD").gameObject;
		WP_lineB_E = WeaponCavB.transform.Find("lineE").gameObject;
		WP_lineB_F = WeaponCavB.transform.Find("lineF").gameObject;
		WP_lineB_G = FindOptionalObject(WeaponCavB.transform, "lineG");
		WP_lineB_H = FindOptionalObject(WeaponCavB.transform, "lineH");
		WP_lineB_I = FindOptionalObject(WeaponCavB.transform, "lineI");
		WP_priceB = WeaponCavB.transform.Find("price/price").GetComponent<Text>();
		WP_equiped = WeaponCavB.transform.Find("equiped").GetComponent<Text>();
		SkillCAV = base.transform.Find("UICanvas/SkillTip").GetComponent<CanvasGroup>();
		Skill_Rect = base.transform.Find("UICanvas/SkillTip").GetComponent<RectTransform>();
		Skill_title = SkillCAV.transform.Find("title").GetComponent<Text>();
		Skill_cost = SkillCAV.transform.Find("cost").gameObject;
		Skill_mana = SkillCAV.transform.Find("cost/mana").GetComponent<Text>();
		Skill_cd = SkillCAV.transform.Find("cost/cd").GetComponent<Text>();
		Skill_time = SkillCAV.transform.Find("cost/time").GetComponent<Text>();
		Skill_main = SkillCAV.transform.Find("main").GetComponent<Text>();
		Skill_next = SkillCAV.transform.Find("next").GetComponent<Text>();
		Skill_type = SkillCAV.transform.Find("type").GetComponent<Text>();
		Skill_unlock = SkillCAV.transform.Find("unlock").GetComponent<Text>();
		Skill_lineA = SkillCAV.transform.Find("lineA").gameObject;
		Skill_lineB = SkillCAV.transform.Find("lineB").gameObject;
		Skill_lineC = SkillCAV.transform.Find("lineC").gameObject;
		Skill_lineD = SkillCAV.transform.Find("lineD").gameObject;
		XiCAV = base.transform.Find("UICanvas/XiTip").GetComponent<CanvasGroup>();
		Xi_title = XiCAV.transform.Find("title").GetComponent<Text>();
		Xi_cost = XiCAV.transform.Find("cost").gameObject;
		Xi_level = XiCAV.transform.Find("cost/level").GetComponent<Text>();
		Xi_element = XiCAV.transform.Find("cost/element").GetComponent<Text>();
		Xi_lineA = XiCAV.transform.Find("lineA")?.gameObject;
		Xi_mainA = XiCAV.transform.Find("mainA").GetComponent<Text>();
		Xi_mainB = XiCAV.transform.Find("mainB").GetComponent<Text>();
		DFCAV = (DFTip_UI ? DFTip_UI : base.transform.Find("UICanvas/DFXi_Tip").GetComponent<CanvasGroup>());
		DF_title = FindTextByName(DFCAV.transform, "title");
		DF_level = FindTextByName(DFCAV.transform, "level");
		DF_element = FindTextByName(DFCAV.transform, "element");
		DF_mainA = FindTextByName(DFCAV.transform, "mainA");
		DF_mainB = FindTextByName(DFCAV.transform, "mainB");
		SetCanvasGroupVisible(DFCAV, visible: false, interactive: false);
		if ((bool)DFList_UI)
		{
			SetCanvasGroupVisible(DFList_UI, visible: false, interactive: true);
		}
		EmptyCAV = base.transform.Find("UICanvas/SkillEmptyTip").GetComponent<CanvasGroup>();
		Empty_mainA = EmptyCAV.transform.Find("mainA").GetComponent<Text>();
		Empty_mainB = EmptyCAV.transform.Find("mainB").GetComponent<Text>();
		if (!mijingGroup)
		{
			mijingGroup = base.transform.Find("UICanvas/Mijing").GetComponent<CanvasGroup>();
		}
		if (!floorText && (bool)mijingGroup)
		{
			floorText = mijingGroup.transform.Find("FloorText").GetComponent<Text>();
		}
		if (!scoreText && (bool)mijingGroup)
		{
			scoreText = mijingGroup.transform.Find("ScoreText").GetComponent<Text>();
		}
		if (!fillImg && (bool)mijingGroup)
		{
			fillImg = mijingGroup.transform.Find("FillImage").GetComponent<Image>();
		}
		if (!compListUI)
		{
			compListUI = base.transform.Find("UICanvas/CompList").gameObject;
		}
		if (!buffTip)
		{
			buffTip = base.transform.Find("UICanvas/BuffTip").gameObject;
		}
		if (!itemTipList)
		{
			itemTipList = base.transform.Find("UICanvas/ItemTipList").gameObject;
		}
		if (!coverAll)
		{
			coverAll = base.transform.Find("UICanvas/CoverAll").GetComponent<CanvasGroup>();
		}
		if (!char_cover)
		{
			char_cover = base.transform.Find("UICanvas/Inventory/CharCover").gameObject;
		}
		if (!splitBaoshiTip)
		{
			splitBaoshiTip = base.transform.Find("UICanvas/SplitBaoshiTip").gameObject;
		}
		if (!splitBaoshiIcon)
		{
			splitBaoshiIcon = base.transform.Find("CursorCanvas/SplitBaoshiIcon").gameObject;
		}
		if (!weaponSpcIcon)
		{
			weaponSpcIcon = base.transform.Find("CursorCanvas/WeaponSpcIcon").gameObject;
		}
		if (!weaponElmIcon)
		{
			weaponElmIcon = base.transform.Find("CursorCanvas/WeaponElmIcon").gameObject;
		}
		if (!weaponEnhIcon)
		{
			weaponEnhIcon = base.transform.Find("CursorCanvas/WeaponEnhIcon").gameObject;
		}
		if ((bool)coverAll)
		{
			coverAll.blocksRaycasts = false;
		}
		if ((bool)coverAll)
		{
			coverAll.interactable = false;
		}
		if ((bool)char_cover)
		{
			char_cover.gameObject.SetActive(value: false);
		}
		if (!virtualCursor)
		{
			virtualCursor = base.transform.Find("CursorCanvas/Virual Cursor").GetComponent<VirtualCursorItem>();
		}
	}

	public void SetModalState(GlobalUiModalState state)
	{
		CurrentModalState = state;
	}

	public void ClearModalState()
	{
		CurrentModalState = GlobalUiModalState.None;
	}

	public bool TryExitCurrentModalState()
	{
		if (!IsInModalState)
		{
			return false;
		}
		switch (CurrentModalState)
		{
		case GlobalUiModalState.None:
			return false;
		case GlobalUiModalState.BaoshiSplit:
			if (SingletonMonoScope<BaoshiManager>.HasInstance)
			{
				SingletonMonoScope<BaoshiManager>.Instance.ExitSplitBaoshi();
			}
			return true;
		case GlobalUiModalState.WeaponSpc:
			if (SingletonMonoScope<WeaponManager>.HasInstance)
			{
				SingletonMonoScope<WeaponManager>.Instance.ExitSpc();
			}
			return true;
		case GlobalUiModalState.WeaponElm:
			if (SingletonMonoScope<WeaponManager>.HasInstance)
			{
				SingletonMonoScope<WeaponManager>.Instance.ExitElm();
			}
			return true;
		case GlobalUiModalState.WeaponEnh:
			if (SingletonMonoScope<WeaponManager>.HasInstance)
			{
				SingletonMonoScope<WeaponManager>.Instance.ExitEnh();
			}
			return true;
		default:
			ClearModalState();
			return false;
		}
	}

	public void SetCoverAll(float a)
	{
		if ((bool)coverAll)
		{
			coverAll.alpha = a;
			coverAll.blocksRaycasts = false;
			coverAll.interactable = false;
		}
		if (a > 0.01f)
		{
			char_cover.SetActive(value: true);
		}
		else
		{
			char_cover.SetActive(value: false);
		}
	}

	public void HideMijing()
	{
		mijingGroup.alpha = 0f;
	}

	public void RefreshMijing(int floor, int score, int maxScore)
	{
		mijingGroup.alpha = 1f;
		float num = Mathf.Clamp01((float)score / (float)Mathf.Max(1, maxScore));
		int num2 = Mathf.RoundToInt(num * 100f);
		fillImg.fillAmount = num;
		floorText.text = LOC.MM.GetLevelFormat("mijing_floor", floor);
		if (num2 >= 100)
		{
			scoreText.text = "100/100";
			fillImg.color = Color.green;
		}
		else
		{
			scoreText.text = $"{num2}/100";
			fillImg.color = Color.yellow;
		}
	}

	public void EnterBaoshiSplitMode()
	{
		SetModalState(GlobalUiModalState.BaoshiSplit);
		if ((bool)splitBaoshiIcon)
		{
			CursorIconItem component = splitBaoshiIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.ShowIcon();
			}
		}
		RefreshSplitBaoshiTip(isShow: true, null, 0L);
		SetCoverAll(0.5f);
	}

	public void ExitBaoshiSplitMode()
	{
		if ((bool)splitBaoshiIcon)
		{
			CursorIconItem component = splitBaoshiIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.HideIcon();
			}
		}
		RefreshSplitBaoshiTip(isShow: false, null, 0L);
		SetCoverAll(0f);
		ClearModalState();
	}

	public void RefreshSplitBaoshiTip(bool isShow, BaoshiClass baoshi = null, long price = 0L)
	{
		if (!splitBaoshiTip)
		{
			return;
		}
		if (!splitCanvasGroup)
		{
			splitCanvasGroup = splitBaoshiTip.GetComponent<CanvasGroup>();
		}
		if (!splitCanvasGroup)
		{
			return;
		}
		if (!splitTipRect)
		{
			splitTipRect = splitBaoshiTip.GetComponent<RectTransform>();
		}
		if (!splitTipBaoshiText)
		{
			Transform transform = splitBaoshiTip.transform.Find("baoshi/baoshi");
			if ((bool)transform)
			{
				splitTipBaoshiText = transform.GetComponent<Text>();
			}
		}
		if (!splitTipPriceText)
		{
			Transform transform2 = splitBaoshiTip.transform.Find("price");
			if ((bool)transform2)
			{
				splitTipPriceText = transform2.GetComponent<Text>();
			}
		}
		splitCanvasGroup.alpha = (isShow ? 1 : 0);
		if (!isShow)
		{
			splitCanvasGroup.interactable = false;
			splitCanvasGroup.blocksRaycasts = false;
			return;
		}
		if (baoshi == null)
		{
			if ((bool)splitTipBaoshiText)
			{
				string main = LOC.MM.GetMain("split_weapon");
				splitTipBaoshiText.text = "<color=#FFFFFF>" + main + "</color>";
			}
			if ((bool)splitTipPriceText)
			{
				string levelFormat = LOC.MM.GetLevelFormat("mijing_need_price", 0);
				splitTipPriceText.text = "<color=#FFFFFF>" + levelFormat + "</color>";
			}
		}
		else
		{
			if ((bool)splitTipBaoshiText)
			{
				string item = LOC.MM.GetItem(baoshi.ItemName);
				splitTipBaoshiText.text = "<color=#00FF00>" + item + "</color>";
			}
			if ((bool)splitTipPriceText)
			{
				string levelFormat2 = LOC.MM.GetLevelFormat("mijing_need_price", price);
				bool flag = SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<InventoryManager>.Instance.GlobalMoney >= price;
				splitTipPriceText.text = (flag ? ("<color=#00FF00>" + levelFormat2 + "</color>") : ("<color=#FF0000>" + levelFormat2 + "</color>"));
			}
		}
		splitCanvasGroup.interactable = false;
		splitCanvasGroup.blocksRaycasts = false;
	}

	public void ClearSplitBaoshiTip()
	{
		if (!splitBaoshiTip)
		{
			return;
		}
		if (!splitCanvasGroup)
		{
			splitCanvasGroup = splitBaoshiTip.GetComponent<CanvasGroup>();
		}
		if (!splitCanvasGroup)
		{
			return;
		}
		if (!splitTipRect)
		{
			splitTipRect = splitBaoshiTip.GetComponent<RectTransform>();
		}
		if (!splitTipBaoshiText)
		{
			Transform transform = splitBaoshiTip.transform.Find("baoshi/baoshi");
			if ((bool)transform)
			{
				splitTipBaoshiText = transform.GetComponent<Text>();
			}
		}
		if (!splitTipPriceText)
		{
			Transform transform2 = splitBaoshiTip.transform.Find("price");
			if ((bool)transform2)
			{
				splitTipPriceText = transform2.GetComponent<Text>();
			}
		}
		if ((bool)splitTipBaoshiText)
		{
			string main = LOC.MM.GetMain("split_weapon");
			splitTipBaoshiText.text = "<color=#FFFFFF>" + main + "</color>";
		}
		if ((bool)splitTipPriceText)
		{
			string levelFormat = LOC.MM.GetLevelFormat("mijing_need_price", 0);
			splitTipPriceText.text = "<color=#FFFFFF>" + levelFormat + "</color>";
		}
		splitCanvasGroup.interactable = false;
		splitCanvasGroup.blocksRaycasts = false;
	}

	private void UpdateSplitBaoshiTipPosition()
	{
		if (!splitBaoshiTip || !splitTipRect || !splitCanvasGroup || splitCanvasGroup.alpha <= 0.001f)
		{
			return;
		}
		RectTransform rectTransform = splitTipRect.parent as RectTransform;
		if (!rectTransform)
		{
			return;
		}
		Vector2 screenPoint;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			if (!SingletonMonoScope<CursorInputManager>.HasInstance)
			{
				return;
			}
			screenPoint = SingletonMonoScope<CursorInputManager>.Instance.VirtualScreenPosition;
		}
		else
		{
			if (!SingletonMonoScope<CursorInputManager>.HasInstance)
			{
				return;
			}
			screenPoint = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		}
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out var localPoint))
		{
			Vector2 anchoredPosition = localPoint + splitTipOffset;
			splitTipRect.anchoredPosition = anchoredPosition;
			ClampSplitTipInsideScreen();
		}
	}

	private void ClampSplitTipInsideScreen()
	{
		if ((bool)splitTipRect)
		{
			Vector3[] array = new Vector3[4];
			splitTipRect.GetWorldCorners(array);
			float x = array[0].x;
			float y = array[0].y;
			float x2 = array[2].x;
			float y2 = array[2].y;
			Vector3 zero = Vector3.zero;
			if (x < splitTipScreenPadding)
			{
				zero.x = splitTipScreenPadding - x;
			}
			else if (x2 > (float)Screen.width - splitTipScreenPadding)
			{
				zero.x = (float)Screen.width - splitTipScreenPadding - x2;
			}
			if (y < splitTipScreenPadding)
			{
				zero.y = splitTipScreenPadding - y;
			}
			else if (y2 > (float)Screen.height - splitTipScreenPadding)
			{
				zero.y = (float)Screen.height - splitTipScreenPadding - y2;
			}
			splitTipRect.position += zero;
		}
	}

	public void EnterWeaponSpcMode()
	{
		SetModalState(GlobalUiModalState.WeaponSpc);
		if ((bool)weaponSpcIcon)
		{
			CursorIconItem component = weaponSpcIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.ShowIcon();
			}
		}
		SetCoverAll(0.5f);
	}

	public void EnterWeaponElmMode()
	{
		SetModalState(GlobalUiModalState.WeaponElm);
		if ((bool)weaponElmIcon)
		{
			CursorIconItem component = weaponElmIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.ShowIcon();
			}
		}
		SetCoverAll(0.5f);
	}

	public void EnterWeaponEnhMode()
	{
		SetModalState(GlobalUiModalState.WeaponEnh);
		if ((bool)weaponEnhIcon)
		{
			CursorIconItem component = weaponEnhIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.ShowIcon();
			}
		}
		SetCoverAll(0.5f);
	}

	public void ExitWeaponSpcMode()
	{
		if ((bool)weaponSpcIcon)
		{
			CursorIconItem component = weaponSpcIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.HideIcon();
			}
		}
		SetCoverAll(0f);
		ClearModalState();
	}

	public void ExitWeaponElmMode()
	{
		if ((bool)weaponElmIcon)
		{
			CursorIconItem component = weaponElmIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.HideIcon();
			}
		}
		SetCoverAll(0f);
		ClearModalState();
	}

	public void ExitWeaponEnhMode()
	{
		if ((bool)weaponEnhIcon)
		{
			CursorIconItem component = weaponEnhIcon.GetComponent<CursorIconItem>();
			if ((bool)component)
			{
				component.HideIcon();
			}
		}
		SetCoverAll(0f);
		ClearModalState();
	}

	public void skillBTlight()
	{
		if (SingletonMonoScope<TalentManager>.Instance.P_Have > 0)
		{
			setBT[3].lightBT();
		}
		else
		{
			setBT[3].UNlightBT();
		}
	}

	public bool IsAnyPanelOpened()
	{
		if (Opened_warehouse)
		{
			return true;
		}
		if (Opened_shop)
		{
			return true;
		}
		if (Opened_Character)
		{
			return true;
		}
		if (Opened_IV)
		{
			return true;
		}
		if (Opened_Talent)
		{
			return true;
		}
		if (Opened_weapon)
		{
			return true;
		}
		if (Opened_baoshi)
		{
			return true;
		}
		if (Singleton<UIManager>.Instance.IsPanelOpened<TeleportPanel>())
		{
			return true;
		}
		if (Singleton<UIManager>.Instance.IsPanelOpened<MijingPanel>())
		{
			return true;
		}
		if (Singleton<UIManager>.Instance.IsPanelOpened<SettingPanel>())
		{
			return true;
		}
		if (Singleton<UIManager>.Instance.IsPanelOpened<PausePanel>())
		{
			return true;
		}
		if (Singleton<UIManager>.Instance.IsPanelOpened<DialogPanel>())
		{
			return true;
		}
		return false;
	}

	private void RefreshGameplayInputAfterPanelStateChanged()
	{
		bool flag = IsAnyPanelOpened();
		if (wasAnyPanelOpenedLastFrame && !flag)
		{
			RestoreGameplayInputAfterPanelClosed();
		}
		wasAnyPanelOpenedLastFrame = flag;
	}

	public void CloseMainPanels()
	{
		if ((bool)Storage.Instance && Storage.Instance.Opened)
		{
			Storage.Instance.CloseChestUI();
		}
		if (Opened_shop && SingletonMonoScope<ShopManager>.HasInstance)
		{
			SingletonMonoScope<ShopManager>.Instance.CloseShop();
		}
		if (Opened_weapon && SingletonMonoScope<WeaponManager>.HasInstance)
		{
			SingletonMonoScope<WeaponManager>.Instance.CloseWeapon();
		}
		if (Opened_baoshi && SingletonMonoScope<BaoshiManager>.HasInstance)
		{
			SingletonMonoScope<BaoshiManager>.Instance.CloseBaoshi();
		}
		Singleton<UIManager>.Instance.HideAllPanels();
		Opened_weapon = false;
		Opened_baoshi = false;
		Opened_Character = false;
		Opened_warehouse = false;
		Opened_IV = false;
		CloseTalentPanelState();
		Opened_shop = false;
		CanvasGroup[] bottomCAV = BottomCAV;
		foreach (CanvasGroup obj in bottomCAV)
		{
			obj.alpha = 0f;
			obj.blocksRaycasts = false;
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.CloseSkillListUI();
			SingletonMonoScope<ACTbar>.Instance.CloseUseListUI();
		}
		RestoreGameplayInputAfterPanelClosed();
	}

	private void RestoreGameplayInputIfNoPanelOpened()
	{
		if (!IsAnyPanelOpened())
		{
			RestoreGameplayInputAfterPanelClosed();
		}
	}

	private static void RestoreGameplayInputAfterPanelClosed()
	{
		if ((bool)EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			SingletonMonoScope<InputManager>.Instance.PrepareGameplayInputUnlock(suppressMovement: false);
		}
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			SingletonMonoScope<InteractionManager>.Instance.ClearAllHover();
			InteractionManager.ClearPendingReleaseBlocks();
		}
		GamepadUINavigationManager.BlockGamepadUIInput = false;
	}

	private void CloseTalentPanelState()
	{
		BottomCAV[3].alpha = 0f;
		BottomCAV[3].blocksRaycasts = false;
		Opened_Talent = false;
		HideDFSkillListForPageChange();
	}

	public void OpenClose_Pause()
	{
		if (!Singleton<UIManager>.Instance.IsPanelOpened<PausePanel>())
		{
			Singleton<UIManager>.Instance.ShowExclusivePanel<PausePanel>();
			Time.timeScale = 0f;
		}
		else
		{
			PausePanel.CloseAndResume();
		}
	}

	public void OpenClose_Character()
	{
		if (Opened_Character)
		{
			BottomCAV[1].alpha = 0f;
			BottomCAV[1].blocksRaycasts = false;
			Opened_Character = false;
			Character.Instance.RefreshUI();
		}
		else if (PL.IsAlive)
		{
			CloseCraftingAndShopIfOpened();
			BottomCAV[1].alpha = 1f;
			BottomCAV[1].blocksRaycasts = true;
			Opened_Character = true;
			Character.Instance.RefreshUI();
		}
	}

	public void OpenClose_IV()
	{
		if (Opened_IV)
		{
			BottomCAV[2].alpha = 0f;
			BottomCAV[2].blocksRaycasts = false;
			Opened_IV = false;
			RestoreGameplayInputIfNoPanelOpened();
			return;
		}
		if (PL.IsAlive)
		{
			BottomCAV[2].alpha = 1f;
			BottomCAV[2].blocksRaycasts = true;
			Opened_IV = true;
		}
		if (Opened_Talent)
		{
			CloseTalentPanelState();
		}
	}

	public void Toggle_IV(bool show)
	{
		if (show)
		{
			if (!Opened_IV)
			{
				if (PL.IsAlive)
				{
					BottomCAV[2].alpha = 1f;
					BottomCAV[2].blocksRaycasts = true;
					Opened_IV = true;
				}
				if (Opened_Talent)
				{
					CloseTalentPanelState();
				}
			}
		}
		else if (Opened_IV)
		{
			BottomCAV[2].alpha = 0f;
			BottomCAV[2].blocksRaycasts = false;
			Opened_IV = false;
			RestoreGameplayInputIfNoPanelOpened();
		}
	}

	public void OpenClose_Talent()
	{
		if (Opened_warehouse)
		{
			return;
		}
		if (Opened_Talent)
		{
			CloseTalentPanelState();
			RestoreGameplayInputIfNoPanelOpened();
		}
		else
		{
			if (!PL.IsAlive)
			{
				return;
			}
			CloseCraftingAndShopIfOpened();
			BottomCAV[3].alpha = 1f;
			BottomCAV[3].blocksRaycasts = true;
			Opened_Talent = true;
			try
			{
				if (SingletonMonoScope<TalentManager>.HasInstance)
				{
					PoedbSkillInjector.TryEnsureButtons(SingletonMonoScope<TalentManager>.Instance);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[GameUIManager.OpenClose_Talent] PoedbSkillInjector 异常: " + ex);
			}
			if (Opened_IV)
			{
				BottomCAV[2].alpha = 0f;
				BottomCAV[2].blocksRaycasts = false;
				Opened_IV = false;
			}
		}
	}

	private void CloseShopIfOpened()
	{
		if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened)
		{
			SingletonMonoScope<ShopManager>.Instance.CloseShop();
		}
	}

	private void CloseCraftingAndShopIfOpened()
	{
		CloseShopIfOpened();
		if (Opened_baoshi && SingletonMonoScope<BaoshiManager>.HasInstance)
		{
			SingletonMonoScope<BaoshiManager>.Instance.CloseBaoshi();
		}
		if (Opened_weapon && SingletonMonoScope<WeaponManager>.HasInstance)
		{
			SingletonMonoScope<WeaponManager>.Instance.CloseWeapon();
		}
	}

	public void OpenClose_Mercenary()
	{
	}

	public void ShopChestCloseOther()
	{
		BottomCAV[1].alpha = 0f;
		BottomCAV[1].blocksRaycasts = false;
		Opened_Character = false;
		CloseTalentPanelState();
	}

	public void CloseAll()
	{
		bool opened_warehouse = Opened_warehouse;
		if ((bool)Storage.Instance && Storage.Instance.Opened)
		{
			Storage.Instance.CloseChest();
		}
		if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened)
		{
			SingletonMonoScope<ShopManager>.Instance.CloseShop();
		}
		if (Opened_weapon && SingletonMonoScope<WeaponManager>.HasInstance)
		{
			SingletonMonoScope<WeaponManager>.Instance.CloseWeapon();
		}
		if (Opened_baoshi && SingletonMonoScope<BaoshiManager>.HasInstance)
		{
			SingletonMonoScope<BaoshiManager>.Instance.CloseBaoshi();
		}
		Singleton<UIManager>.Instance.HideAllPanels();
		CanvasGroup[] bottomCAV = BottomCAV;
		foreach (CanvasGroup canvasGroup in bottomCAV)
		{
			if ((bool)canvasGroup)
			{
				canvasGroup.alpha = 0f;
				canvasGroup.blocksRaycasts = false;
			}
		}
		Opened_warehouse = false;
		Opened_shop = false;
		Opened_Character = false;
		Opened_IV = false;
		CloseTalentPanelState();
		Opened_weapon = false;
		Opened_baoshi = false;
		if (SingletonMonoScope<InventoryManager>.HasInstance && (bool)SingletonMonoScope<InventoryManager>.Instance.cav)
		{
			SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 0f;
			SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = false;
		}
		if (SingletonMonoScope<WarehouseManager>.HasInstance && (bool)SingletonMonoScope<WarehouseManager>.Instance.cav)
		{
			SingletonMonoScope<WarehouseManager>.Instance.cav.alpha = 0f;
			SingletonMonoScope<WarehouseManager>.Instance.cav.blocksRaycasts = false;
		}
		if (SingletonMonoScope<InventoryManager>.HasInstance && (bool)Hand.Instance && Hand.Instance.isDragItem)
		{
			SingletonMonoScope<InventoryManager>.Instance.HandItemDrop();
			if (opened_warehouse && SingletonMonoScope<WarehouseManager>.HasInstance)
			{
				SingletonMonoScope<WarehouseManager>.Instance.HandItemDrop();
			}
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.CloseSkillListUI();
			SingletonMonoScope<ACTbar>.Instance.CloseUseListUI();
		}
		RestoreGameplayInputAfterPanelClosed();
	}

	private static void RefreshWeaponTipLayout(RectTransform rect)
	{
		if ((bool)rect)
		{
			Canvas.ForceUpdateCanvases();
			LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
		}
	}

	private void ClampTipToScreen(RectTransform rect)
	{
		if ((bool)rect && (bool)rect.GetComponentInParent<Canvas>())
		{
			Vector3[] array = new Vector3[4];
			rect.GetWorldCorners(array);
			float x = array[0].x;
			float y = array[0].y;
			float x2 = array[2].x;
			float y2 = array[2].y;
			float x3 = 0f;
			float y3 = 0f;
			if (x < screenPadding)
			{
				x3 = screenPadding - x;
			}
			else if (x2 > (float)Screen.width - screenPadding)
			{
				x3 = (float)Screen.width - screenPadding - x2;
			}
			if (y < screenPadding)
			{
				y3 = screenPadding - y;
			}
			else if (y2 > (float)Screen.height - screenPadding)
			{
				y3 = (float)Screen.height - screenPadding - y2;
			}
			rect.position += new Vector3(x3, y3, 0f);
		}
	}

	private float GetBestPivotY(RectTransform rect, Vector3 anchorPos)
	{
		if (!rect)
		{
			return 1f;
		}
		RefreshWeaponTipLayout(rect);
		float num = rect.rect.height * rect.lossyScale.y;
		float num2 = anchorPos.y - screenPadding;
		float num3 = (float)Screen.height - anchorPos.y - screenPadding;
		if (num2 >= num)
		{
			return 1f;
		}
		if (num3 > num2)
		{
			return 0f;
		}
		return 1f;
	}

	private static bool TryGetItemTopAnchor(SlotData slotData, SlotScript[,] slotGrid, bool preferRightSide, out Vector3 anchorPos)
	{
		anchorPos = Vector3.zero;
		if (slotData == null || slotGrid == null)
		{
			return false;
		}
		IntVector2 startPos = slotData.StartPos;
		IntVector2 itemSize = slotData.ItemSize;
		if (itemSize.x <= 0 || itemSize.y <= 0)
		{
			return false;
		}
		int x = startPos.x;
		int num = startPos.x + itemSize.x - 1;
		int y = startPos.y;
		if (x < 0 || y < 0)
		{
			return false;
		}
		if (num >= slotGrid.GetLength(0) || y >= slotGrid.GetLength(1))
		{
			return false;
		}
		RectTransform component = slotGrid[x, y].GetComponent<RectTransform>();
		RectTransform component2 = slotGrid[num, y].GetComponent<RectTransform>();
		if (!component || !component2)
		{
			return false;
		}
		Vector3[] array = new Vector3[4];
		Vector3[] array2 = new Vector3[4];
		component.GetWorldCorners(array);
		component2.GetWorldCorners(array2);
		anchorPos = (preferRightSide ? array2[2] : array[1]);
		return true;
	}

	private void LayoutSingleTip(RectTransform rect, Vector3 anchorPos, bool preferRightSide)
	{
		if ((bool)rect)
		{
			float bestPivotY = GetBestPivotY(rect, anchorPos);
			rect.pivot = (preferRightSide ? new Vector2(0f, bestPivotY) : new Vector2(1f, bestPivotY));
			float num = tipSideOffsetX;
			rect.position = new Vector3(anchorPos.x + (preferRightSide ? num : (0f - num)), anchorPos.y + tipSideOffsetY, anchorPos.z);
			ClampTipToScreen(rect);
		}
	}

	private void LayoutCompareTips(RectTransform rectA, RectTransform rectB, Vector3 anchorPos, bool preferRightSide)
	{
		if ((bool)rectA && (bool)rectB)
		{
			RefreshWeaponTipLayout(rectA);
			RefreshWeaponTipLayout(rectB);
			float num = rectA.rect.width * rectA.lossyScale.x;
			float a = rectA.rect.height * rectA.lossyScale.y;
			float b = rectB.rect.height * rectB.lossyScale.y;
			float num2 = Mathf.Max(a, b);
			float num3 = anchorPos.y - screenPadding;
			float num4 = (float)Screen.height - anchorPos.y - screenPadding;
			float y = ((num3 >= num2) ? 1f : ((!(num4 > num3)) ? 1f : 0f));
			if (preferRightSide)
			{
				rectA.pivot = new Vector2(0f, y);
				rectB.pivot = new Vector2(0f, y);
				rectA.position = new Vector3(anchorPos.x + tipSideOffsetX, anchorPos.y + tipSideOffsetY, anchorPos.z);
				rectB.position = new Vector3(rectA.position.x + num + compareTipSpacing, rectA.position.y, rectA.position.z);
			}
			else
			{
				rectA.pivot = new Vector2(1f, y);
				rectB.pivot = new Vector2(1f, y);
				rectA.position = new Vector3(anchorPos.x - tipSideOffsetX, anchorPos.y + tipSideOffsetY, anchorPos.z);
				rectB.position = new Vector3(rectA.position.x - num - compareTipSpacing, rectA.position.y, rectA.position.z);
			}
			ClampTipToScreen(rectA);
			ClampTipToScreen(rectB);
			RefreshWeaponTipLayout(rectA);
			num = rectA.rect.width * rectA.lossyScale.x;
			if (preferRightSide)
			{
				rectB.position = new Vector3(rectA.position.x + num + compareTipSpacing, rectA.position.y, rectA.position.z);
			}
			else
			{
				rectB.position = new Vector3(rectA.position.x - num - compareTipSpacing, rectA.position.y, rectA.position.z);
			}
			ClampTipToScreen(rectB);
			rectB.position = new Vector3(rectB.position.x, rectA.position.y, rectB.position.z);
			ClampTipToScreen(rectB);
		}
	}

	private void ShowWPTipAInternal(Vector3 trans, WeaponClass wp)
	{
		if (wp != null)
		{
			HideAllWeaponTips();
			Vector3 screenPosition = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			Vector2 pos = new Vector2(screenPosition.x / (float)Screen.width, screenPosition.y / (float)Screen.height);
			bool preferRightSide = pos.x < 0.5f;
			FillWeaponTipA(wp, pos);
			RefreshWeaponTipLayout(WP_RectA);
			LayoutSingleTip(WP_RectA, trans, preferRightSide);
			WeaponCavA.alpha = 1f;
		}
	}

	public void ShowWPTipB(Vector3 trans, WeaponClass wp)
	{
		if (wp != null)
		{
			HideAllWeaponTips();
			Vector3 screenPosition = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			bool flag = new Vector2(screenPosition.x / (float)Screen.width, screenPosition.y / (float)Screen.height).x < 0.5f;
			FillWeaponTipB(wp);
			RefreshWeaponTipLayout(WP_RectB);
			LayoutSingleTip(WP_RectB, trans, flag);
			float x = (flag ? 50f : (-50f));
			WP_RectB.position += new Vector3(x, 0f, 0f);
			ClampTipToScreen(WP_RectB);
			WeaponCavB.alpha = 1f;
		}
	}

	public void ShowWPTipA(WeaponClass wp, SlotData slotData, SlotScript[,] slotGrid)
	{
		if (wp != null && slotData != null && slotGrid != null)
		{
			bool preferRightSide = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition.x / (float)Screen.width < 0.5f;
			if (!TryGetItemTopAnchor(slotData, slotGrid, preferRightSide, out var anchorPos))
			{
				HideAllWeaponTips();
			}
			else
			{
				ShowWPTipAInternal(anchorPos, wp);
			}
		}
	}

	public void ShowCompareWeaponTips(WeaponClass targetWeapon, SlotData slotData, SlotScript[,] slotGrid)
	{
		if (targetWeapon == null || slotData == null || slotGrid == null)
		{
			return;
		}
		bool preferRightSide = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition.x / (float)Screen.width < 0.5f;
		if (!TryGetItemTopAnchor(slotData, slotGrid, preferRightSide, out var anchorPos))
		{
			HideAllWeaponTips();
			return;
		}
		CharButton charButton = SingletonMonoScope<InventoryManager>.Instance.ReturnCharBT(targetWeapon.CharType);
		if (!charButton || !charButton.hasWeapon || charButton.weapon == null)
		{
			ShowWPTipAInternal(anchorPos, targetWeapon);
		}
		else
		{
			ShowCompareWeaponTipsInternal(anchorPos, targetWeapon, charButton.weapon, preferRightSide);
		}
	}

	public void ShowCompareWeaponTips(WeaponClass wpA, WeaponClass wpB, SlotData slotData, SlotScript[,] slotGrid)
	{
		if (wpA != null && wpB != null && slotData != null && slotGrid != null)
		{
			bool preferRightSide = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition.x / (float)Screen.width < 0.5f;
			if (!TryGetItemTopAnchor(slotData, slotGrid, preferRightSide, out var anchorPos))
			{
				HideAllWeaponTips();
			}
			else
			{
				ShowCompareWeaponTipsInternal(anchorPos, wpA, wpB, preferRightSide);
			}
		}
	}

	private void ShowCompareWeaponTipsInternal(Vector3 anchorPos, WeaponClass wpA, WeaponClass wpB, bool preferRightSide)
	{
		if (wpA != null && wpB != null)
		{
			HideAllWeaponTips();
			Vector3 screenPosition = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			Vector2 pos = new Vector2(screenPosition.x / (float)Screen.width, screenPosition.y / (float)Screen.height);
			FillWeaponTipA(wpA, pos);
			FillWeaponTipB(wpB);
			RefreshWeaponTipLayout(WP_RectA);
			RefreshWeaponTipLayout(WP_RectB);
			LayoutCompareTips(WP_RectA, WP_RectB, anchorPos, preferRightSide);
			WeaponCavA.alpha = 1f;
			WeaponCavB.alpha = 1f;
		}
	}

	private void AppendGamepadXShortcut(ref string shortcutText, ref string actionText, Vector2 pos)
	{
		string gamepadXActionKey = GetGamepadXActionKey(pos);
		if (!string.IsNullOrEmpty(gamepadXActionKey))
		{
			shortcutText += "\nX";
			actionText = actionText + "\n" + LOC.MM.GetMain(gamepadXActionKey);
		}
	}

	private string GetGamepadXActionKey(Vector2 pos)
	{
		if (Opened_shop)
		{
			if (!(pos.x >= 0.5f))
			{
				return null;
			}
			return "QuickSell";
		}
		if (Opened_warehouse)
		{
			if (!(pos.x < 0.5f))
			{
				return "PutInChest";
			}
			return "PutInBackpack";
		}
		if (Opened_IV)
		{
			return "Drop";
		}
		return null;
	}

	private static void SetTipText(GameObject line, Text text, string value)
	{
		bool flag = !string.IsNullOrEmpty(value);
		if ((bool)line)
		{
			line.SetActive(flag);
		}
		if ((bool)text)
		{
			text.gameObject.SetActive(flag);
			text.text = (flag ? value : string.Empty);
		}
	}

	private static void SetTipLine(GameObject line, bool visible)
	{
		if ((bool)line)
		{
			line.SetActive(visible);
		}
	}

	private static bool HasWeaponSpecial(WeaponClass wp, int index, out string text)
	{
		text = string.Empty;
		if (wp == null)
		{
			return false;
		}
		if (!wp.TryGetSPCTemplate(index, out var _, out var mb) || mb == null || mb.SPCtype <= 0)
		{
			return false;
		}
		text = wp.GetSpecial(index);
		return true;
	}

	private static bool HasWeaponFw(WeaponClass wp, out string text)
	{
		text = string.Empty;
		if (wp == null || wp.FW_Base == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(wp.FW_Base.FWname) || string.IsNullOrEmpty(wp.FW_Base.type))
		{
			return false;
		}
		text = wp.GetFW_Base();
		return !string.IsNullOrEmpty(text);
	}

	private static bool HasSet(WeaponClass wp, out string setName, out string setMain)
	{
		setName = string.Empty;
		setMain = string.Empty;
		if (wp == null || wp.Set_Index <= 0)
		{
			return false;
		}
		setName = wp.GetSetName();
		setMain = wp.GetSet();
		if (string.IsNullOrEmpty(setName))
		{
			return !string.IsNullOrEmpty(setMain);
		}
		return true;
	}

	private void ResetWeaponTipAExtraBlocks()
	{
		SetTipText(WP_lineA_C, WP_specialA, null);
		SetTipText(WP_lineA_D, WP_special2A, null);
		SetTipText(WP_lineA_E, WP_setNameA, null);
		SetTipText(null, WP_setMainA, null);
		SetTipLine(WP_lineA_F, visible: false);
		SetTipText(WP_fwObjA, WP_fwA, null);
		SetTipText(WP_lineA_G, WP_skA, null);
		SetTipText(WP_lineA_H, WP_cpA, null);
		SetTipText(WP_lineA_I, null, null);
		SetTipText(null, WP_dotA, null);
		SetWeaponTipDoubleIcon(WP_doubleIconA, null);
	}

	private void ResetWeaponTipBExtraBlocks()
	{
		SetTipText(WP_lineB_C, WP_specialB, null);
		SetTipText(WP_lineB_D, WP_special2B, null);
		SetTipText(WP_lineB_E, WP_setNameB, null);
		SetTipText(null, WP_setMainB, null);
		SetTipLine(WP_lineB_F, visible: false);
		SetTipText(WP_fwObjB, WP_fwB, null);
		SetTipText(WP_lineB_G, WP_skB, null);
		SetTipText(WP_lineB_H, WP_cpB, null);
		SetTipText(WP_lineB_I, null, null);
		SetTipText(null, WP_dotB, null);
		SetWeaponTipDoubleIcon(WP_doubleIconB, null);
	}

	private static void SetWeaponTipDoubleIcon(Image image, WeaponClass wp)
	{
		if (!image)
		{
			return;
		}
		if (wp == null || !wp.HasBaseValueDouble)
		{
			image.gameObject.SetActive(value: false);
			return;
		}
		Sprite sprite = null;
		if (SingletonMonoScope<ItemManager>.HasInstance && SingletonMonoScope<ItemManager>.Instance.Double_Icon != null)
		{
			int baseValueDoubleIconIndex = wp.GetBaseValueDoubleIconIndex();
			if (baseValueDoubleIconIndex >= 0 && baseValueDoubleIconIndex < SingletonMonoScope<ItemManager>.Instance.Double_Icon.Length)
			{
				sprite = SingletonMonoScope<ItemManager>.Instance.Double_Icon[baseValueDoubleIconIndex];
			}
		}
		if (!sprite)
		{
			image.gameObject.SetActive(value: false);
			return;
		}
		image.sprite = sprite;
		image.color = Color.white;
		image.gameObject.SetActive(value: true);
	}

	private static string GetWeaponSkillColor(int xi)
	{
		switch (xi)
		{
		case 0:
		case 5:
		case 7:
			return "#FF0000";
		case 1:
			return "#53C5FF";
		case 2:
		case 4:
			return "#FFF242";
		case 3:
		case 6:
		case 9:
			return "#FFE6F6";
		case 8:
		case 11:
			return "#06FF00";
		case 10:
			return "#B300FF";
		default:
			return "#FFFFFF";
		}
	}

	private static string FormatWeaponSkillText(WPSkill skill, int xi)
	{
		if (skill == null || string.IsNullOrEmpty(skill.IndexName))
		{
			return string.Empty;
		}
		int num = skill.Number + skill.Number2;
		string text = $"<color={GetWeaponSkillColor(xi)}>{LOC.MM.GetSkill(skill.IndexName)} + {num}</color>";
		if (skill.Number2 > 0)
		{
			text += $" <color=#808080>+ {skill.Number2}</color>";
		}
		return text;
	}

	private void FillWeaponTipA(WeaponClass wp, Vector2 pos)
	{
		if (wp == null)
		{
			return;
		}
		WeaponCavA.alpha = 1f;
		ResetWeaponTipAExtraBlocks();
		WP_lineA_A.SetActive(value: true);
		WP_lineA_B.SetActive(value: true);
		WP_titleA.text = wp.GetTitle();
		switch (wp.PLtype)
		{
		case 0:
			if (PL.PLType == 0)
			{
				WP_typeA.text = "<color=#BAFDFF>" + LOC.MM.GetMain("MGC Item") + "</color>";
			}
			else
			{
				WP_typeA.text = "<color=#FF1F1F>" + LOC.MM.GetMain("MGC Item") + "</color>";
			}
			break;
		case 1:
			if (PL.PLType == 1)
			{
				WP_typeA.text = "<color=#BAFDFF>" + LOC.MM.GetMain("SQS Item") + "</color>";
			}
			else
			{
				WP_typeA.text = "<color=#FF1F1F>" + LOC.MM.GetMain("SQS Item") + "</color>";
			}
			break;
		case 2:
			if (PL.PLType == 2)
			{
				WP_typeA.text = "<color=#BAFDFF>" + LOC.MM.GetMain("ARC Item") + "</color>";
			}
			else
			{
				WP_typeA.text = "<color=#FF1F1F>" + LOC.MM.GetMain("ARC Item") + "</color>";
			}
			break;
		case 3:
			if (PL.PLType == 3)
			{
				WP_typeA.text = "<color=#BAFDFF>" + LOC.MM.GetMain("DEAD Item") + "</color>";
			}
			else
			{
				WP_typeA.text = "<color=#FF1F1F>" + LOC.MM.GetMain("DEAD Item") + "</color>";
			}
			break;
		case 1000:
			WP_typeA.text = null;
			break;
		}
		if (wp.Level > PL.Level)
		{
			WP_levelA.text = string.Format("<color=#FF1F1F>{0} : {1}</color>", LOC.MM.GetMain("Level"), wp.Level);
		}
		else
		{
			WP_levelA.text = string.Format("<color=#BAFDFF>{0} : {1}</color>", LOC.MM.GetMain("Level"), wp.Level);
		}
		WP_mainA.gameObject.SetActive(value: true);
		WP_mainA.text = wp.GetMain();
		SetWeaponTipDoubleIcon(WP_doubleIconA, wp);
		SetTipText(null, WP_dotA, wp.GetDot());
		SetTipText(null, WP_skA, wp.GetSK());
		SetTipText(null, WP_cpA, wp.GetCP());
		SetTipText(WP_lineA_C, WP_specialA, HasWeaponSpecial(wp, 0, out var text) ? text : null);
		SetTipText(WP_lineA_D, WP_special2A, HasWeaponSpecial(wp, 1, out var text2) ? text2 : null);
		if (HasSet(wp, out var setName, out var setMain))
		{
			WP_lineA_E.SetActive(value: true);
			SetTipText(null, WP_setNameA, setName);
			SetTipText(null, WP_setMainA, setMain);
		}
		else
		{
			WP_lineA_E.SetActive(value: false);
			SetTipText(null, WP_setNameA, null);
			SetTipText(null, WP_setMainA, null);
		}
		string text3;
		string value = (HasWeaponFw(wp, out text3) ? text3 : null);
		SetTipLine(WP_lineA_F, !string.IsNullOrEmpty(value));
		SetTipText(WP_fwObjA, WP_fwA, value);
		GameObject[] array = skillOBJA;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		if (wp.WP_SkillCount > 0)
		{
			SetTipLine(WP_lineA_G, visible: true);
			for (int j = 0; j < wp.WP_SkillCount; j++)
			{
				skillOBJA[j].gameObject.SetActive(value: true);
				string indexName = wp.WPSK[j].IndexName;
				if (!string.IsNullOrEmpty(indexName) && SingletonMonoScope<TalentManager>.Instance.SKI.TryGetValue(indexName, out var value2))
				{
					switch (value2.Xi)
					{
					case 0:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 1:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 2:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 3:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 4:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 5:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 6:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 7:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 8:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 9:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 10:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 11:
						skillTextA[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					}
				}
			}
		}
		else
		{
			SetTipLine(WP_lineA_G, visible: false);
			array = skillOBJA;
			for (int k = 0; k < array.Length; k++)
			{
				array[k].gameObject.SetActive(value: false);
			}
		}
		if (wp.AocaoCount > 0)
		{
			SetTipLine(WP_lineA_H, visible: true);
			array = WP_baoshiOBJA;
			for (int l = 0; l < array.Length; l++)
			{
				array[l].SetActive(value: false);
			}
			for (int m = 0; m < wp.AocaoCount; m++)
			{
				WP_baoshiOBJA[m].SetActive(value: true);
				if (wp.Aocao[m].HasBaoshi)
				{
					WP_baoshiA[m].text = wp.GetBaoshi(m);
					pic_aocaoA[m].color = new Color32(0, 0, 0, 0);
					pic_baoshiA[m].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					pic_baoshiA[m].sprite = wp.Aocao[m].Icon;
				}
				else
				{
					WP_baoshiA[m].text = LOC.MM.GetMain("Empty Slot");
					pic_aocaoA[m].color = new Color32(197, 197, 197, byte.MaxValue);
					pic_baoshiA[m].color = new Color32(0, 0, 0, 0);
				}
			}
		}
		else
		{
			SetTipLine(WP_lineA_H, visible: false);
			array = WP_baoshiOBJA;
			for (int n = 0; n < array.Length; n++)
			{
				array[n].SetActive(value: false);
			}
		}
		SetTipLine(WP_lineA_I, visible: true);
		string main;
		string main2;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			if (Opened_warehouse)
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("Equip");
				main = main + "\n" + LOC.MM.GetMain("Shift+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("PutInChest");
			}
			else if (Opened_shop && pos.x >= 0.5f)
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("Equip");
				main = main + "\n" + LOC.MM.GetMain("Shift+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("QuickSell");
			}
			else if (Opened_shop && pos.x < 0.5f)
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("QuickBuy");
				main = main + "\n" + LOC.MM.GetMain("LeftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("click_buy");
			}
			else
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("Equip");
				main = main + "\n" + LOC.MM.GetMain("Ctrl+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Drop");
			}
		}
		else if (Opened_warehouse)
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("Equip");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		else if (Opened_shop && pos.x >= 0.5f)
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("Equip");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		else if (Opened_shop && pos.x < 0.5f)
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("QuickBuy");
			main = main + "\n" + LOC.MM.GetMain("gamepad_confirm_click");
			main2 = main2 + "\n" + LOC.MM.GetMain("click_buy");
		}
		else
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("Equip");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		WP_YJ.gameObject.SetActive(value: true);
		WP_Set.gameObject.SetActive(value: true);
		priceA.SetActive(value: true);
		WP_YJ.text = main;
		WP_Set.text = main2;
		if (Opened_shop && pos.x < 0.5f)
		{
			WP_priceA.text = wp.ByPrice.ToString();
		}
		else
		{
			WP_priceA.text = wp.Price.ToString();
		}
	}

	private void FillWeaponTipB(WeaponClass wp)
	{
		WeaponCavB.alpha = 1f;
		ResetWeaponTipBExtraBlocks();
		WP_lineB_A.SetActive(value: true);
		WP_lineB_B.SetActive(value: true);
		WP_titleB.text = wp.GetTitle();
		switch (wp.PLtype)
		{
		case 0:
			WP_typeB.text = ((PL.PLType == 0) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("MGC Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("MGC Item") + "</color>"));
			break;
		case 1:
			WP_typeB.text = ((PL.PLType == 1) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("SQS Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("SQS Item") + "</color>"));
			break;
		case 2:
			WP_typeB.text = ((PL.PLType == 2) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("ARC Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("ARC Item") + "</color>"));
			break;
		case 3:
			WP_typeB.text = ((PL.PLType == 3) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("DEAD Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("DEAD Item") + "</color>"));
			break;
		case 1000:
			WP_typeB.text = null;
			break;
		}
		WP_levelB.text = ((wp.Level > PL.Level) ? string.Format("<color=#FF1F1F>{0} : {1}</color>", LOC.MM.GetMain("Level"), wp.Level) : string.Format("<color=#BAFDFF>{0} : {1}</color>", LOC.MM.GetMain("Level"), wp.Level));
		WP_mainB.gameObject.SetActive(value: true);
		WP_mainB.text = wp.GetMain();
		SetWeaponTipDoubleIcon(WP_doubleIconB, wp);
		SetTipText(null, WP_dotB, wp.GetDot());
		SetTipText(null, WP_skB, wp.GetSK());
		SetTipText(null, WP_cpB, wp.GetCP());
		SetTipText(WP_lineB_C, WP_specialB, HasWeaponSpecial(wp, 0, out var text) ? text : null);
		SetTipText(WP_lineB_D, WP_special2B, HasWeaponSpecial(wp, 1, out var text2) ? text2 : null);
		if (HasSet(wp, out var setName, out var setMain))
		{
			WP_lineB_E.SetActive(value: true);
			SetTipText(null, WP_setNameB, setName);
			SetTipText(null, WP_setMainB, setMain);
		}
		else
		{
			WP_lineB_E.SetActive(value: false);
			SetTipText(null, WP_setNameB, null);
			SetTipText(null, WP_setMainB, null);
		}
		string text3;
		string value = (HasWeaponFw(wp, out text3) ? text3 : null);
		SetTipLine(WP_lineB_F, !string.IsNullOrEmpty(value));
		SetTipText(WP_fwObjB, WP_fwB, value);
		GameObject[] array = skillOBJB;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		if (wp.WP_SkillCount > 0)
		{
			SetTipLine(WP_lineB_G, visible: true);
			for (int j = 0; j < wp.WP_SkillCount; j++)
			{
				skillOBJB[j].gameObject.SetActive(value: true);
				string indexName = wp.WPSK[j].IndexName;
				if (!string.IsNullOrEmpty(indexName) && SingletonMonoScope<TalentManager>.Instance.SKI.TryGetValue(indexName, out var value2))
				{
					switch (value2.Xi)
					{
					case 0:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 1:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 2:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 3:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 4:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 5:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 6:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 7:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 8:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 9:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 10:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					case 11:
						skillTextB[j].text = FormatWeaponSkillText(wp.WPSK[j], value2.Xi);
						break;
					}
				}
			}
		}
		else
		{
			SetTipLine(WP_lineB_G, visible: false);
		}
		if (wp.AocaoCount > 0)
		{
			SetTipLine(WP_lineB_H, visible: true);
			for (int k = 0; k < WP_baoshiOBJB.Length; k++)
			{
				WP_baoshiOBJB[k].SetActive(value: false);
			}
			for (int l = 0; l < wp.AocaoCount; l++)
			{
				WP_baoshiOBJB[l].SetActive(value: true);
				if (wp.Aocao[l].HasBaoshi)
				{
					WP_baoshiB[l].text = wp.GetBaoshi(l);
					pic_aocaoB[l].color = new Color32(0, 0, 0, 0);
					pic_baoshiB[l].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					pic_baoshiB[l].sprite = wp.Aocao[l].Icon;
				}
				else
				{
					WP_baoshiB[l].text = LOC.MM.GetMain("Empty Slot");
					pic_aocaoB[l].color = new Color32(197, 197, 197, byte.MaxValue);
					pic_baoshiB[l].color = new Color32(0, 0, 0, 0);
				}
			}
		}
		else
		{
			SetTipLine(WP_lineB_H, visible: false);
			array = WP_baoshiOBJB;
			for (int m = 0; m < array.Length; m++)
			{
				array[m].SetActive(value: false);
			}
		}
		SetTipLine(WP_lineB_I, visible: true);
		WP_priceB.text = wp.Price.ToString();
		WP_equiped.text = LOC.MM.GetMain("Equiped");
	}

	public void ShowBSTip(BaoshiClass bs, SlotData slotData, SlotScript[,] slotGrid)
	{
		if (bs != null)
		{
			HideAllWeaponTips();
			Vector3 screenPosition = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			Vector2 pos = new Vector2(screenPosition.x / (float)Screen.width, screenPosition.y / (float)Screen.height);
			bool preferRightSide = pos.x < 0.5f;
			FillGemTip(bs, pos);
			RefreshWeaponTipLayout(WP_RectA);
			if (!TryGetItemTopAnchor(slotData, slotGrid, preferRightSide, out var anchorPos))
			{
				HideAllWeaponTips();
				return;
			}
			LayoutSingleTip(WP_RectA, anchorPos, preferRightSide);
			WeaponCavA.alpha = 1f;
		}
	}

	public void ShowUseTip(UseItemClass item, SlotData slotData, SlotScript[,] slotGrid)
	{
		if (item != null)
		{
			HideAllWeaponTips();
			Vector3 screenPosition = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			Vector2 pos = new Vector2(screenPosition.x / (float)Screen.width, screenPosition.y / (float)Screen.height);
			bool preferRightSide = pos.x < 0.5f;
			FillUseItemTip(item, pos);
			RefreshWeaponTipLayout(WP_RectA);
			if (!TryGetItemTopAnchor(slotData, slotGrid, preferRightSide, out var anchorPos))
			{
				HideAllWeaponTips();
				return;
			}
			LayoutSingleTip(WP_RectA, anchorPos, preferRightSide);
			WeaponCavA.alpha = 1f;
		}
	}

	private string GetBaoshiTypeText(BaoshiClass bs)
	{
		if (bs == null)
		{
			return string.Empty;
		}
		switch (bs.UseType)
		{
		case 0:
		case 2:
			return LOC.MM.GetMain("Gem");
		case 1:
			return LOC.MM.GetMain("Essence");
		case 3:
		{
			if (SingletonMonoScope<TalentManager>.HasInstance && SingletonMonoScope<TalentManager>.Instance.TryGetSkillFWPlayerType(bs.SKname, out var plType))
			{
				string text = plType switch
				{
					0 => "Character_Class_MGC", 
					1 => "Character_Class_SQS", 
					2 => "Character_Class_ARC", 
					3 => "Character_Class_DEAD", 
					_ => string.Empty, 
				};
				if (!string.IsNullOrEmpty(text))
				{
					return LOC.MM.GetMainFormat("Skill Rune Format", LOC.MM.GetMain(text));
				}
			}
			return LOC.MM.GetMain("Rune");
		}
		case 4:
		case 5:
			return LOC.MM.GetMain("Rune");
		default:
			return LOC.MM.GetMain("Gem");
		}
	}

	private void FillGemTip(BaoshiClass bs, Vector2 pos)
	{
		WeaponCavA.alpha = 1f;
		ResetWeaponTipAExtraBlocks();
		WP_lineA_A.SetActive(value: true);
		WP_lineA_B.SetActive(value: true);
		WP_lineA_C.SetActive(value: false);
		WP_lineA_D.SetActive(value: false);
		WP_lineA_E.SetActive(value: false);
		SetTipLine(WP_lineA_I, visible: true);
		WP_specialA.gameObject.SetActive(value: false);
		GameObject[] array = skillOBJA;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		array = WP_baoshiOBJA;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: false);
		}
		WP_titleA.text = bs.GetTitle();
		WP_typeA.text = "<color=#BAFDFF>" + GetBaoshiTypeText(bs) + "</color>";
		WP_levelA.text = null;
		WP_mainA.text = bs.GetMain();
		string main;
		string main2;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			if (Opened_warehouse)
			{
				main = LOC.MM.GetMain("LiftClickDrag");
				main2 = LOC.MM.GetMain("LiftClickSocket");
				main = main + "\n" + LOC.MM.GetMain("Shift+RightClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Split");
				main = main + "\n" + LOC.MM.GetMain("Shift+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("PutInChest");
			}
			else if (Opened_shop && pos.x >= 0.5f)
			{
				main = LOC.MM.GetMain("LiftClickDrag");
				main2 = LOC.MM.GetMain("LiftClickSocket");
				main = main + "\n" + LOC.MM.GetMain("Shift+RightClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Split");
				main = main + "\n" + LOC.MM.GetMain("Shift+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("QuickSell");
			}
			else if (Opened_shop && pos.x < 0.5f)
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("QuickBuy");
				main = main + "\n" + LOC.MM.GetMain("LeftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("click_buy");
			}
			else
			{
				main = LOC.MM.GetMain("LiftClickDrag");
				main2 = LOC.MM.GetMain("LiftClickSocket");
				main = main + "\n" + LOC.MM.GetMain("Shift+RightClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Split");
				main = main + "\n" + LOC.MM.GetMain("Ctrl+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Drop");
			}
		}
		else if (Opened_warehouse)
		{
			main = LOC.MM.GetMain("AClickDrag");
			main2 = LOC.MM.GetMain("LiftClickSocket");
			main = main + "\nL3" + LOC.MM.GetMain("gamepad_back");
			main2 = main2 + "\n" + LOC.MM.GetMain("Split");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		else if (Opened_shop && pos.x >= 0.5f)
		{
			main = LOC.MM.GetMain("AClickDrag");
			main2 = LOC.MM.GetMain("LiftClickSocket");
			main = main + "\nL3" + LOC.MM.GetMain("gamepad_back");
			main2 = main2 + "\n" + LOC.MM.GetMain("Split");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		else if (Opened_shop && pos.x < 0.5f)
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("QuickBuy");
			main = main + "\n" + LOC.MM.GetMain("gamepad_confirm_click");
			main2 = main2 + "\n" + LOC.MM.GetMain("click_buy");
		}
		else
		{
			main = LOC.MM.GetMain("AClickDrag");
			main2 = LOC.MM.GetMain("LiftClickSocket");
			main = main + "\nL3" + LOC.MM.GetMain("gamepad_back");
			main2 = main2 + "\n" + LOC.MM.GetMain("Split");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		WP_YJ.gameObject.SetActive(value: true);
		WP_Set.gameObject.SetActive(value: true);
		priceA.SetActive(value: true);
		WP_YJ.text = main;
		WP_Set.text = main2;
		if (Opened_shop && pos.x < 0.5f)
		{
			WP_priceA.text = bs.ByPrice.ToString();
		}
		else
		{
			WP_priceA.text = bs.MaxPrice.ToString();
		}
	}

	private void FillUseItemTip(UseItemClass item, Vector2 pos)
	{
		if (item == null)
		{
			return;
		}
		WeaponCavA.alpha = 1f;
		ResetWeaponTipAExtraBlocks();
		WP_lineA_A.SetActive(value: true);
		WP_lineA_B.SetActive(value: true);
		WP_lineA_C.SetActive(value: false);
		WP_lineA_D.SetActive(value: false);
		WP_lineA_E.SetActive(value: false);
		SetTipLine(WP_lineA_I, visible: true);
		WP_specialA.gameObject.SetActive(value: false);
		GameObject[] array = skillOBJA;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		array = WP_baoshiOBJA;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: false);
		}
		WP_titleA.text = item.GetTitle();
		switch (item.InfoType)
		{
		case 0:
			WP_typeA.text = "<color=#BAFDFF>" + LOC.MM.GetMain("Restorative Potion") + "</color>";
			WP_levelA.text = string.Format("<color=#75FFAB>{0} : {1}{2}</color>", LOC.MM.GetMain("CDtime"), item.CDTime, LOC.MM.GetMain("S"));
			break;
		case 1:
			WP_typeA.text = "<color=#D5FFC5>" + LOC.MM.GetMain("Enhanced Potion") + "</color>";
			WP_levelA.text = string.Format("<color=#E675FF>{0} : {1}{2}</color>", LOC.MM.GetMain("Duration"), item.Duration, LOC.MM.GetMain("S"));
			break;
		case 2:
			WP_typeA.text = "<color=#D3EAFF>" + LOC.MM.GetMain("Scroll") + "</color>";
			WP_levelA.text = null;
			break;
		case 3:
			WP_typeA.text = "<color=#FFC284>" + LOC.MM.GetMain("Permanent Potion") + "</color>";
			WP_levelA.text = null;
			break;
		case 4:
			WP_typeA.text = "<color=#FFC0F3>" + LOC.MM.GetMain("Special Stone") + "</color>";
			WP_levelA.text = null;
			break;
		case 5:
			WP_typeA.text = "<color=#FFC0F3>" + LOC.MM.GetMain("Special Potion") + "</color>";
			WP_levelA.text = null;
			break;
		case 6:
			WP_typeA.text = "<color=#FFEE78>" + LOC.MM.GetMain("Special Item") + "</color>";
			WP_levelA.text = null;
			break;
		case 7:
			WP_typeA.text = "<color=#FFEE78>" + LOC.MM.GetMain("Special Item") + "</color>";
			WP_levelA.text = null;
			break;
		}
		WP_mainA.text = item.GetMain();
		string main;
		string main2;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			if (Opened_warehouse)
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("Use");
				main = main + "\n" + LOC.MM.GetMain("Shift+RightClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Split");
				main = main + "\n" + LOC.MM.GetMain("Shift+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("PutInChest");
			}
			else if (Opened_shop && pos.x >= 0.5f)
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("Use");
				main = main + "\n" + LOC.MM.GetMain("Shift+RightClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Split");
				main = main + "\n" + LOC.MM.GetMain("Shift+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("QuickSell");
			}
			else if (Opened_shop && pos.x < 0.5f)
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("QuickBuy");
				main = main + "\n" + LOC.MM.GetMain("LeftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("click_buy");
			}
			else
			{
				main = LOC.MM.GetMain("RightClick");
				main2 = LOC.MM.GetMain("Use");
				main = main + "\n" + LOC.MM.GetMain("Shift+RightClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Split");
				main = main + "\n" + LOC.MM.GetMain("Ctrl+LiftClick");
				main2 = main2 + "\n" + LOC.MM.GetMain("Drop");
			}
		}
		else if (Opened_warehouse)
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("Use");
			main = main + "\nL3" + LOC.MM.GetMain("gamepad_back");
			main2 = main2 + "\n" + LOC.MM.GetMain("Split");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		else if (Opened_shop && pos.x >= 0.5f)
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("Use");
			main = main + "\nL3" + LOC.MM.GetMain("gamepad_back");
			main2 = main2 + "\n" + LOC.MM.GetMain("Split");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		else if (Opened_shop && pos.x < 0.5f)
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("QuickBuy");
			main = main + "\n" + LOC.MM.GetMain("gamepad_confirm_click");
			main2 = main2 + "\n" + LOC.MM.GetMain("click_buy");
		}
		else
		{
			main = LOC.MM.GetMain("gamepad_back_click");
			main2 = LOC.MM.GetMain("Use");
			main = main + "\nL3" + LOC.MM.GetMain("gamepad_back");
			main2 = main2 + "\n" + LOC.MM.GetMain("Split");
			AppendGamepadXShortcut(ref main, ref main2, pos);
		}
		WP_YJ.gameObject.SetActive(value: true);
		WP_Set.gameObject.SetActive(value: true);
		priceA.SetActive(value: true);
		WP_YJ.text = main;
		WP_Set.text = main2;
		if (Opened_shop && pos.x < 0.5f)
		{
			WP_priceA.text = item.ByPrice.ToString();
		}
		else
		{
			WP_priceA.text = item.MaxPrice.ToString();
		}
	}

	public void HideAllWeaponTips()
	{
		WeaponCavA.alpha = 0f;
		WeaponCavB.alpha = 0f;
		SetWeaponTipDoubleIcon(WP_doubleIconA, null);
		SetWeaponTipDoubleIcon(WP_doubleIconB, null);
	}

	public void HideTooltipA()
	{
		WeaponCavA.alpha = 0f;
		SetWeaponTipDoubleIcon(WP_doubleIconA, null);
		if (WeaponCavB.alpha != 0f)
		{
			WeaponCavB.alpha = 0f;
			SetWeaponTipDoubleIcon(WP_doubleIconB, null);
		}
	}

	public void HideTooltipB()
	{
		WeaponCavB.alpha = 0f;
		SetWeaponTipDoubleIcon(WP_doubleIconB, null);
	}

	private void SetSkillTypeTip(bool isBSSkill, bool isLastSkill)
	{
		if (!isBSSkill && !isLastSkill)
		{
			Skill_type.gameObject.SetActive(value: false);
			return;
		}
		Skill_lineD.SetActive(value: true);
		Skill_type.gameObject.SetActive(value: true);
		string main = LOC.MM.GetMain("LastSkillTag");
		if (!isBSSkill)
		{
			Skill_type.text = main;
			return;
		}
		string main2 = LOC.MM.GetMain("LastSkill");
		string text = (main.StartsWith("（") ? string.Empty : " ");
		Skill_type.text = (isLastSkill ? (main2 + text + main) : main2);
	}

	public void ShowSkilltip(int xi, int type, string skillName, Transform trans)
	{
		SkillCAV.alpha = 1f;
		Vector3 screenPosition = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		Vector2 vector = new Vector2(screenPosition.x / (float)Screen.width, screenPosition.y / (float)Screen.height);
		if (vector.y < 0.2f)
		{
			Skill_Rect.pivot = new Vector2(1f, 0f);
			SkillCAV.transform.position = new Vector3(trans.position.x - 40f, trans.position.y - 40f, trans.position.z);
		}
		else if (vector.y > 0.2f && vector.y < 0.7f)
		{
			Skill_Rect.pivot = new Vector2(1f, 0.5f);
			SkillCAV.transform.position = new Vector3(trans.position.x - 40f, trans.position.y, trans.position.z);
		}
		else
		{
			Skill_Rect.pivot = new Vector2(1f, 1f);
			SkillCAV.transform.position = new Vector3(trans.position.x - 40f, trans.position.y + 40f, trans.position.z);
		}
		switch (type)
		{
		case 0:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: true);
			Skill_lineC.SetActive(value: false);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: true);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Sample_F.TryGetValue(skillName, out var value2) || value2 == null)
			{
				HideSkillTip();
				return;
			}
			string text2 = LOC.MM.GetSkill(value2.IndexName);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = skillName;
			}
			Skill_title.text = text2;
			if (value2.NoTime == 0)
			{
				Skill_mana.text = string.Format("{0} : {1:0.##}  {2}", LOC.MM.GetMain("ManaCast"), value2.ManaCost_Last, LOC.MM.GetMain("/S"));
			}
			else
			{
				Skill_mana.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("ManaCast"), value2.ManaCost_Last);
			}
			if (value2.SampleSkill)
			{
				Skill_cd.gameObject.SetActive(value: false);
			}
			else
			{
				Skill_cd.gameObject.SetActive(value: true);
				if (value2.NoTime == 0)
				{
					Skill_cd.text = LOC.MM.GetMain("AuraSkill") ?? "";
				}
				else
				{
					Skill_cd.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("CDtime"), value2.CoolDown_Last);
				}
			}
			if (value2.BuffTime_Last > 0f)
			{
				if (value2.NoTime == 0)
				{
					Skill_time.gameObject.SetActive(value: false);
				}
				else
				{
					Skill_time.gameObject.SetActive(value: true);
					Skill_time.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("Duration"), value2.BuffTime_Last);
				}
			}
			else
			{
				Skill_time.gameObject.SetActive(value: false);
			}
			Skill_main.text = value2.GetInfoA();
			if (value2.Level_Base > 0)
			{
				Skill_lineC.SetActive(value: true);
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value2.GetInfoB();
			}
			SetSkillTypeTip(value2.BS_Skill, value2.LastSkill);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value2.UnLock_Point)
			{
				Skill_lineC.SetActive(value: true);
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value2.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 1:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: false);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Sample_S.TryGetValue(skillName, out var value7) || value7 == null)
			{
				HideSkillTip();
				return;
			}
			string text7 = LOC.MM.GetSkill(value7.IndexName);
			if (string.IsNullOrEmpty(text7))
			{
				text7 = skillName;
			}
			Skill_title.text = text7;
			Skill_main.text = value7.GetInfoA();
			if (value7.Level_Base > 0)
			{
				Skill_lineC.SetActive(value: true);
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value7.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value7.UnLock_Point)
			{
				Skill_lineC.SetActive(value: true);
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value7.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 2:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: true);
			Skill_lineC.SetActive(value: false);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: true);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Comp_F.TryGetValue(skillName, out var value4) || value4 == null)
			{
				HideSkillTip();
				return;
			}
			string text4 = LOC.MM.GetSkill(value4.IndexName);
			if (string.IsNullOrEmpty(text4))
			{
				text4 = skillName;
			}
			Skill_title.text = text4;
			Skill_mana.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("ManaCast"), value4.ManaCost_Last);
			Skill_cd.gameObject.SetActive(value: true);
			Skill_cd.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("CDtime"), value4.CoolDown_Last);
			Skill_time.gameObject.SetActive(value: false);
			Skill_main.text = value4.GetInfoA();
			if (value4.Level_Base > 0)
			{
				Skill_lineC.SetActive(value: true);
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value4.GetInfoB();
			}
			SetSkillTypeTip(value4.BS_Skill, value4.LastSkill);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value4.UnLock_Point)
			{
				Skill_lineC.SetActive(value: true);
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value4.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 3:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: false);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Comp_S.TryGetValue(skillName, out var value3) || value3 == null)
			{
				HideSkillTip();
				return;
			}
			string text3 = LOC.MM.GetSkill(value3.IndexName);
			if (string.IsNullOrEmpty(text3))
			{
				text3 = skillName;
			}
			Skill_title.text = text3;
			Skill_main.text = value3.GetInfoA();
			if (value3.Level_Base > 0)
			{
				Skill_lineC.SetActive(value: true);
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value3.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value3.UnLock_Point)
			{
				Skill_lineC.SetActive(value: true);
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value3.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 4:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: false);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Dot_F.TryGetValue(skillName, out var value6) || value6 == null)
			{
				HideSkillTip();
				return;
			}
			string text6 = LOC.MM.GetSkill(value6.IndexName);
			if (string.IsNullOrEmpty(text6))
			{
				text6 = skillName;
			}
			Skill_title.text = text6;
			Skill_main.text = value6.GetInfoA();
			if (value6.Level_Base > 0)
			{
				Skill_lineC.SetActive(value: true);
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value6.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value6.UnLock_Point)
			{
				Skill_lineC.SetActive(value: true);
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value6.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 5:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: false);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Dot_S.TryGetValue(skillName, out var value5) || value5 == null)
			{
				HideSkillTip();
				return;
			}
			string text5 = LOC.MM.GetSkill(value5.IndexName);
			if (string.IsNullOrEmpty(text5))
			{
				text5 = skillName;
			}
			Skill_title.text = text5;
			Skill_main.text = value5.GetInfoA();
			if (value5.Level_Base > 0)
			{
				Skill_lineC.SetActive(value: true);
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value5.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value5.UnLock_Point)
			{
				Skill_lineC.SetActive(value: true);
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value5.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 6:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: false);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Bei.TryGetValue(skillName, out var value) || value == null)
			{
				HideSkillTip();
				return;
			}
			string text = LOC.MM.GetSkill(value.IndexName);
			if (string.IsNullOrEmpty(text))
			{
				text = skillName;
			}
			Skill_title.text = text;
			Skill_main.text = value.GetInfoA();
			if (value.Level_Base > 0)
			{
				Skill_lineC.SetActive(value: true);
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value.UnLock_Point)
			{
				Skill_lineC.SetActive(value: true);
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_lineC.SetActive(value: false);
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		}
		SkillTagSystem.ApplyToSkillTip(xi, type, skillName, Skill_main);
	}

	public void RefreshSkilltip(int xi, int type, string skillName)
	{
		switch (type)
		{
		case 0:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: true);
			Skill_lineC.SetActive(value: true);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: true);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Sample_F.TryGetValue(skillName, out var value2) || value2 == null)
			{
				HideSkillTip();
				return;
			}
			string text2 = LOC.MM.GetSkill(value2.IndexName);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = skillName;
			}
			Skill_title.text = text2;
			Skill_mana.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("ManaCast"), value2.ManaCost_Last);
			if (value2.SampleSkill)
			{
				Skill_cd.gameObject.SetActive(value: false);
			}
			else
			{
				Skill_cd.gameObject.SetActive(value: true);
				Skill_cd.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("CDtime"), value2.CoolDown_Last);
			}
			if (value2.BuffTime_Last > 0f)
			{
				Skill_time.gameObject.SetActive(value: true);
				Skill_time.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("Duration"), value2.BuffTime_Last);
			}
			else
			{
				Skill_time.gameObject.SetActive(value: false);
			}
			Skill_main.text = value2.GetInfoA();
			if (value2.Level_Base > 0)
			{
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value2.GetInfoB();
			}
			SetSkillTypeTip(value2.BS_Skill, value2.LastSkill);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value2.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value2.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 1:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: true);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Sample_S.TryGetValue(skillName, out var value7) || value7 == null)
			{
				HideSkillTip();
				return;
			}
			string text7 = LOC.MM.GetSkill(value7.IndexName);
			if (string.IsNullOrEmpty(text7))
			{
				text7 = skillName;
			}
			Skill_title.text = text7;
			Skill_main.text = value7.GetInfoA();
			if (value7.Level_Base > 0)
			{
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value7.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value7.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value7.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 2:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: true);
			Skill_lineC.SetActive(value: true);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: true);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Comp_F.TryGetValue(skillName, out var value4) || value4 == null)
			{
				HideSkillTip();
				return;
			}
			string text4 = LOC.MM.GetSkill(value4.IndexName);
			if (string.IsNullOrEmpty(text4))
			{
				text4 = skillName;
			}
			Skill_title.text = text4;
			Skill_mana.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("ManaCast"), value4.ManaCost_Last);
			Skill_cd.gameObject.SetActive(value: true);
			Skill_cd.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("CDtime"), value4.CoolDown_Last);
			Skill_time.gameObject.SetActive(value: false);
			Skill_main.text = value4.GetInfoA();
			if (value4.Level_Base > 0)
			{
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value4.GetInfoB();
			}
			SetSkillTypeTip(value4.BS_Skill, value4.LastSkill);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value4.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value4.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 3:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: true);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Comp_S.TryGetValue(skillName, out var value3) || value3 == null)
			{
				HideSkillTip();
				return;
			}
			string text3 = LOC.MM.GetSkill(value3.IndexName);
			if (string.IsNullOrEmpty(text3))
			{
				text3 = skillName;
			}
			Skill_title.text = text3;
			Skill_main.text = value3.GetInfoA();
			if (value3.Level_Base > 0)
			{
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value3.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value3.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value3.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 4:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: true);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Dot_F.TryGetValue(skillName, out var value6) || value6 == null)
			{
				HideSkillTip();
				return;
			}
			string text6 = LOC.MM.GetSkill(value6.IndexName);
			if (string.IsNullOrEmpty(text6))
			{
				text6 = skillName;
			}
			Skill_title.text = text6;
			Skill_main.text = value6.GetInfoA();
			if (value6.Level_Base > 0)
			{
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value6.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value6.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value6.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 5:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: true);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Dot_S.TryGetValue(skillName, out var value5) || value5 == null)
			{
				HideSkillTip();
				return;
			}
			string text5 = LOC.MM.GetSkill(value5.IndexName);
			if (string.IsNullOrEmpty(text5))
			{
				text5 = skillName;
			}
			Skill_title.text = text5;
			Skill_main.text = value5.GetInfoA();
			if (value5.Level_Base > 0)
			{
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value5.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value5.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value5.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		case 6:
		{
			Skill_lineA.SetActive(value: true);
			Skill_lineB.SetActive(value: false);
			Skill_lineC.SetActive(value: true);
			Skill_lineD.SetActive(value: false);
			Skill_next.gameObject.SetActive(value: false);
			Skill_cost.SetActive(value: false);
			if (!SingletonMonoScope<TalentManager>.Instance.XiData[xi].Bei.TryGetValue(skillName, out var value) || value == null)
			{
				HideSkillTip();
				return;
			}
			string text = LOC.MM.GetSkill(value.IndexName);
			if (string.IsNullOrEmpty(text))
			{
				text = skillName;
			}
			Skill_title.text = text;
			Skill_main.text = value.GetInfoA();
			if (value.Level_Base > 0)
			{
				Skill_next.gameObject.SetActive(value: true);
				Skill_next.text = value.GetInfoB();
			}
			Skill_type.gameObject.SetActive(value: false);
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		}
		SkillTagSystem.ApplyToSkillTip(xi, type, skillName, Skill_main);
	}

	public void HideSkillTip()
	{
		SkillCAV.alpha = 0f;
	}

	public void HideUseTip()
	{
		WeaponCavA.alpha = 0f;
	}

	public void ShowXiTip(int xi, Transform trans)
	{
		if (!SingletonMonoScope<TalentManager>.HasInstance || SingletonMonoScope<TalentManager>.Instance.XiData == null || xi < 0 || xi >= SingletonMonoScope<TalentManager>.Instance.XiData.Length || SingletonMonoScope<TalentManager>.Instance.XiData[xi] == null)
		{
			HideXiTip();
			return;
		}
		XiCAV.alpha = 1f;
		LayoutXiTipAtTargetRightBottom(trans);
		SetXiTipDetailVisible(visible: true);
		Xi_title.text = LOC.MM.GetSkill(SingletonMonoScope<TalentManager>.Instance.XiData[xi].IndexName);
		Xi_level.text = string.Format("{0} : {1}", LOC.MM.GetMain("Current Level"), SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base);
		string main = LOC.MM.GetMain("ElementType");
		main = main + " : <color=" + DamageColor.Colors[SingletonMonoScope<TalentManager>.Instance.XiData[xi].damageType] + ">" + LOC.MM.GetMain(SWS.El_Name(SingletonMonoScope<TalentManager>.Instance.XiData[xi].damageType)) + "</color> ";
		Xi_element.text = main;
		Xi_mainA.text = SingletonMonoScope<TalentManager>.Instance.XiData[xi].GetInfoA();
		Xi_mainB.text = SingletonMonoScope<TalentManager>.Instance.XiData[xi].GetInfoB();
	}

	public void ShowDFTalentTreeTip(Transform trans)
	{
		XiCAV.alpha = 1f;
		LayoutXiTipAtTargetRightBottom(trans);
		SetXiTipDetailVisible(visible: false);
		Xi_title.text = LOC.MM.GetMain("DF Talent Tree");
		if ((bool)Xi_mainB)
		{
			Xi_mainB.text = LOC.MM.GetMain("Unlock at level 100");
		}
	}

	private void SetXiTipDetailVisible(bool visible)
	{
		if ((bool)Xi_cost)
		{
			Xi_cost.SetActive(visible);
		}
		if ((bool)Xi_lineA)
		{
			Xi_lineA.SetActive(visible);
		}
		if ((bool)Xi_mainA)
		{
			Xi_mainA.gameObject.SetActive(visible);
		}
		if ((bool)Xi_mainB)
		{
			Xi_mainB.gameObject.SetActive(value: true);
			Transform transform = (Xi_cost ? Xi_cost.transform : null);
			if ((bool)Xi_mainB.transform.parent && Xi_mainB.transform.parent != transform)
			{
				Xi_mainB.transform.parent.gameObject.SetActive(value: true);
			}
		}
	}

	public void HideXiTip()
	{
		XiCAV.alpha = 0f;
	}

	private void LayoutXiTipAtTargetRightBottom(Transform target)
	{
		if ((bool)XiCAV && (bool)target)
		{
			RectTransform rectTransform = XiCAV.transform as RectTransform;
			if ((bool)rectTransform)
			{
				rectTransform.pivot = new Vector2(1f, 0f);
			}
			Vector3 vector = target.position;
			RectTransform rectTransform2 = target as RectTransform;
			if ((bool)rectTransform2)
			{
				Vector3[] array = new Vector3[4];
				rectTransform2.GetWorldCorners(array);
				vector = array[1];
			}
			XiCAV.transform.position = new Vector3(vector.x - 8f, vector.y + 8f, XiCAV.transform.position.z);
		}
	}

	public void ShowDFTip(int index, Transform trans)
	{
		if (_dfSkillListOpen || !SingletonMonoScope<TalentManager>.HasInstance)
		{
			return;
		}
		SkilDFData dFData = SingletonMonoScope<TalentManager>.Instance.GetDFData(index);
		if (dFData == null)
		{
			HideDFTip();
			return;
		}
		_dfTipIndex = index;
		_dfTipTarget = trans;
		SetCanvasGroupVisible(DFCAV, visible: true, interactive: false);
		LayoutDFTipAtTargetLeftCenter(trans);
		if ((bool)DF_title)
		{
			DF_title.text = dFData.GetTitle();
		}
		if ((bool)DF_level)
		{
			DF_level.text = string.Format("{0} : {1}/{2}", LOC.MM.GetMain("Current Level"), dFData.Level_Base, dFData.Level_Max);
		}
		if ((bool)DF_element)
		{
			DF_element.text = BuildDFChoiceSwitchText(dFData);
		}
		if ((bool)DF_mainA)
		{
			DF_mainA.text = dFData.GetInfoA();
		}
		if ((bool)DF_mainB)
		{
			DF_mainB.text = dFData.GetInfoBA();
		}
	}

	public void RefreshDFTip(int index)
	{
		if (_dfTipIndex == index && (bool)_dfTipTarget)
		{
			ShowDFTip(index, _dfTipTarget);
		}
	}

	public void ShowDFLieTip(SKillBT_Lie lie, Transform trans)
	{
		if (!_dfSkillListOpen && (bool)lie)
		{
			_dfTipIndex = -1;
			_dfTipTarget = trans;
			SetCanvasGroupVisible(DFCAV, visible: true, interactive: false);
			LayoutDFTipAtTargetLeftCenter(trans);
			if ((bool)DF_title)
			{
				DF_title.text = BuildDFLieStatText(lie.Type);
			}
			if ((bool)DF_level)
			{
				DF_level.text = string.Format("{0} : {1}", LOC.MM.GetMain("Current Level"), Mathf.FloorToInt(lie.Number));
			}
			if ((bool)DF_element)
			{
				DF_element.text = string.Empty;
			}
			if ((bool)DF_mainA)
			{
				DF_mainA.text = BuildDFLieInfoText(lie.Type);
			}
			if ((bool)DF_mainB)
			{
				DF_mainB.text = BuildDFLieCurrentBonusText(lie.Type, lie.Number);
			}
		}
	}

	public void HideDFTip()
	{
		_dfTipIndex = -1;
		_dfTipTarget = null;
		SetCanvasGroupVisible(DFCAV, visible: false, interactive: false);
	}

	public bool IsDFSkillListOpenFor(int index)
	{
		if (_dfSkillListOpen)
		{
			return _dfSkillListTargetIndex == index;
		}
		return false;
	}

	private void LayoutDFTipAtTargetLeftCenter(Transform target)
	{
		if ((bool)DFCAV && (bool)target)
		{
			RectTransform rectTransform = DFCAV.transform as RectTransform;
			Vector3 obj = (SingletonMonoScope<CursorInputManager>.HasInstance ? SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition : Input.mousePosition);
			bool flag = obj.y > (float)Screen.height * 0.5f;
			if ((bool)rectTransform)
			{
				rectTransform.pivot = (flag ? new Vector2(1f, 1f) : new Vector2(1f, 0f));
			}
			Vector3 vector = target.position;
			RectTransform rectTransform2 = target as RectTransform;
			if ((bool)rectTransform2)
			{
				Vector3[] array = new Vector3[4];
				rectTransform2.GetWorldCorners(array);
				vector = (array[0] + array[1]) * 0.5f;
			}
			DFCAV.transform.position = new Vector3(vector.x - 40f, vector.y, DFCAV.transform.position.z);
		}
	}

	public void ShowDFSkillListTip(SKillBT_DF target, int skillIndex, Transform trans)
	{
		if (!target || !SingletonMonoScope<TalentManager>.HasInstance)
		{
			return;
		}
		SkilDFData dFData = SingletonMonoScope<TalentManager>.Instance.GetDFData(target.Index);
		if (dFData == null || !dFData.IsValidSkillIndex(skillIndex))
		{
			HideDFTip();
			return;
		}
		SkilDFData_Lit skilDFData_Lit = dFData.SK[skillIndex];
		_dfTipIndex = -1;
		_dfTipTarget = trans;
		SetCanvasGroupVisible(DFCAV, visible: true, interactive: false);
		LayoutDFTipAtTargetLeftCenter(trans);
		if ((bool)DF_title)
		{
			DF_title.text = LOC.MM.GetSkill(skilDFData_Lit.IndexName);
		}
		if ((bool)DF_level)
		{
			DF_level.text = string.Format("{0} : {1}/{2}", LOC.MM.GetMain("Current Level"), dFData.Level_Base, dFData.Level_Max);
		}
		if ((bool)DF_element)
		{
			DF_element.text = string.Empty;
		}
		if ((bool)DF_mainA)
		{
			DF_mainA.text = LOC.MM.GetSkill(skilDFData_Lit.Info) + " : + " + SkilDFData.FormatLitNumber(skilDFData_Lit);
		}
		if ((bool)DF_mainB)
		{
			DF_mainB.text = LOC.MM.GetMain("Current bonus") + " : + " + SkilDFData.FormatLitNumber(skilDFData_Lit, dFData.Level_Base);
		}
	}

	public void ShowDFSkillList(SKillBT_DF target)
	{
		if (!target || !DFList_UI || !SingletonMonoScope<TalentManager>.HasInstance)
		{
			return;
		}
		SkilDFData dFData = SingletonMonoScope<TalentManager>.Instance.GetDFData(target.Index);
		if (dFData == null || !dFData.HasMultipleChoices)
		{
			HideDFSkillList();
			return;
		}
		bool dfSkillListOpen = _dfSkillListOpen;
		_dfSkillListOpen = true;
		_dfSkillListTargetIndex = target.Index;
		_dfSkillListTarget = target;
		HideDFTip();
		SetCanvasGroupVisible(DFList_UI, visible: true, interactive: true);
		LayoutDFSkillListAtTargetLeftCenter(target.transform);
		PlayDFSkillListOpenSound(dfSkillListOpen);
		int num = 0;
		SkillListBT_DF orCreateDFListButton = GetOrCreateDFListButton(0);
		if (!orCreateDFListButton)
		{
			HideDFSkillList();
			return;
		}
		for (int i = 0; i < dFData.SkillSlotCount; i++)
		{
			SkilDFData_Lit skilDFData_Lit = dFData.SK[i];
			if (dFData.IsValidSkillIndex(i))
			{
				SkillListBT_DF skillListBT_DF = ((num == 0) ? orCreateDFListButton : GetOrCreateDFListButton(num));
				if ((bool)skillListBT_DF)
				{
					Sprite dFIcon = SingletonMonoScope<TalentManager>.Instance.GetDFIcon(dFData.Index, i, dFData.Unlocked);
					skillListBT_DF.Setup(target, i, dFIcon, LOC.MM.GetSkill(skilDFData_Lit.IndexName));
					num++;
				}
			}
		}
		for (int j = num; j < DFList_UI.transform.childCount; j++)
		{
			SkillListBT_DF componentInChildren = DFList_UI.transform.GetChild(j).GetComponentInChildren<SkillListBT_DF>(includeInactive: true);
			if ((bool)componentInChildren)
			{
				componentInChildren.Hide();
			}
		}
		ResizeDFSkillList(num);
	}

	public void HideDFSkillList()
	{
		HideDFSkillList(restoreTip: true);
	}

	public void HideDFSkillListForPageChange()
	{
		HideDFSkillList(restoreTip: false);
	}

	private void HideDFSkillList(bool restoreTip)
	{
		SKillBT_DF dfSkillListTarget = _dfSkillListTarget;
		_dfSkillListOpen = false;
		_dfSkillListTargetIndex = -1;
		_dfSkillListTarget = null;
		SetCanvasGroupVisible(DFList_UI, visible: false, interactive: true);
		HideDFTip();
		if (restoreTip)
		{
			if ((bool)dfSkillListTarget)
			{
				ShowDFTip(dfSkillListTarget.Index, dfSkillListTarget.transform);
			}
			else
			{
				ShowDFTipUnderPointer();
			}
		}
	}

	private void PlayDFSkillListOpenSound(bool wasOpen)
	{
		if (!wasOpen && SingletonMonoGlobal<AudioManager>.HasInstance && (bool)SingletonMonoGlobal<AudioManager>.Instance.audioData)
		{
			RuntimeManager.PlayOneShot(SingletonMonoGlobal<AudioManager>.Instance.audioData.Add_Point_1);
		}
	}

	private void LayoutDFSkillListAtTargetLeftCenter(Transform target)
	{
		if ((bool)DFList_UI && (bool)target)
		{
			RectTransform rectTransform = DFList_UI.transform as RectTransform;
			if ((bool)rectTransform)
			{
				rectTransform.pivot = new Vector2(1f, 0.5f);
			}
			Vector3 vector = target.position;
			RectTransform rectTransform2 = target as RectTransform;
			if ((bool)rectTransform2)
			{
				Vector3[] array = new Vector3[4];
				rectTransform2.GetWorldCorners(array);
				vector = (array[0] + array[1] + array[2] + array[3]) * 0.25f;
			}
			DFList_UI.transform.position = new Vector3(vector.x - 60f, vector.y, DFList_UI.transform.position.z);
		}
	}

	private Text FindTextByName(Transform root, string objectName)
	{
		if (!root)
		{
			return null;
		}
		Text[] componentsInChildren = root.GetComponentsInChildren<Text>(includeInactive: true);
		foreach (Text text in componentsInChildren)
		{
			if (text.name == objectName)
			{
				return text;
			}
		}
		return null;
	}

	private void SetCanvasGroupVisible(CanvasGroup canvasGroup, bool visible, bool interactive)
	{
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = (visible ? 1 : 0);
			canvasGroup.interactable = visible && interactive;
			canvasGroup.blocksRaycasts = visible && interactive;
		}
	}

	private void HandleDFSkillListOutsideClick()
	{
		if (_dfSkillListOpen)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			bool mouseButtonDown2 = Input.GetMouseButtonDown(1);
			bool mouseButtonDown3 = Input.GetMouseButtonDown(2);
			if ((mouseButtonDown || mouseButtonDown2 || mouseButtonDown3) && (!(mouseButtonDown || mouseButtonDown2) || !IsPointerOverDFSkillListButton()) && (!(mouseButtonDown || mouseButtonDown2) || !IsPointerOverDFSkillButton()))
			{
				HideDFSkillList();
			}
		}
	}

	private bool IsPointerOverDFSkillListButton()
	{
		if (!EventSystem.current)
		{
			return false;
		}
		Vector2 position = (SingletonMonoScope<CursorInputManager>.HasInstance ? SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition : Input.mousePosition);
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = position
		};
		_dfSkillListRaycastResults.Clear();
		EventSystem.current.RaycastAll(eventData, _dfSkillListRaycastResults);
		foreach (RaycastResult dfSkillListRaycastResult in _dfSkillListRaycastResults)
		{
			if ((bool)dfSkillListRaycastResult.gameObject && (bool)dfSkillListRaycastResult.gameObject.GetComponentInParent<SkillListBT_DF>())
			{
				return true;
			}
		}
		return false;
	}

	private bool IsPointerOverDFSkillButton()
	{
		if (!EventSystem.current)
		{
			return false;
		}
		Vector2 position = (SingletonMonoScope<CursorInputManager>.HasInstance ? SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition : Input.mousePosition);
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = position
		};
		_dfSkillListRaycastResults.Clear();
		EventSystem.current.RaycastAll(eventData, _dfSkillListRaycastResults);
		foreach (RaycastResult dfSkillListRaycastResult in _dfSkillListRaycastResults)
		{
			if ((bool)(dfSkillListRaycastResult.gameObject ? dfSkillListRaycastResult.gameObject.GetComponentInParent<SKillBT_DF>() : null))
			{
				return true;
			}
		}
		return false;
	}

	private void ShowDFTipUnderPointer()
	{
		SKillBT_DF dFSkillButtonUnderPointer = GetDFSkillButtonUnderPointer();
		if ((bool)dFSkillButtonUnderPointer)
		{
			ShowDFTip(dFSkillButtonUnderPointer.Index, dFSkillButtonUnderPointer.transform);
		}
	}

	private SKillBT_DF GetDFSkillButtonUnderPointer()
	{
		if (!EventSystem.current)
		{
			return null;
		}
		Vector2 position = (SingletonMonoScope<CursorInputManager>.HasInstance ? SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition : Input.mousePosition);
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = position
		};
		_dfSkillListRaycastResults.Clear();
		EventSystem.current.RaycastAll(eventData, _dfSkillListRaycastResults);
		foreach (RaycastResult dfSkillListRaycastResult in _dfSkillListRaycastResults)
		{
			SKillBT_DF sKillBT_DF = (dfSkillListRaycastResult.gameObject ? dfSkillListRaycastResult.gameObject.GetComponentInParent<SKillBT_DF>() : null);
			if ((bool)sKillBT_DF)
			{
				return sKillBT_DF;
			}
		}
		return null;
	}

	private SkillListBT_DF GetOrCreateDFListButton(int index)
	{
		if (!DFList_UI || DFList_UI.transform.childCount == 0)
		{
			return null;
		}
		while (DFList_UI.transform.childCount <= index)
		{
			UnityEngine.Object.Instantiate(DFList_UI.transform.GetChild(0).gameObject, DFList_UI.transform);
		}
		Transform child = DFList_UI.transform.GetChild(index);
		SkillListBT_DF skillListBT_DF = child.GetComponentInChildren<SkillListBT_DF>(includeInactive: true);
		if (!skillListBT_DF)
		{
			Image image = null;
			Image[] componentsInChildren = child.GetComponentsInChildren<Image>(includeInactive: true);
			foreach (Image image2 in componentsInChildren)
			{
				if ((bool)image2 && image2.raycastTarget)
				{
					image = image2;
					break;
				}
			}
			skillListBT_DF = (image ? image.gameObject : child.gameObject).AddComponent<SkillListBT_DF>();
		}
		skillListBT_DF.SetDisplayRoot(child.gameObject);
		return skillListBT_DF;
	}

	private void ResizeDFSkillList(int count)
	{
		if (!DFList_UI)
		{
			return;
		}
		RectTransform component = DFList_UI.GetComponent<RectTransform>();
		if ((bool)component && DFList_UI.transform.childCount != 0)
		{
			RectTransform component2 = DFList_UI.transform.GetChild(0).GetComponent<RectTransform>();
			float num = (component2 ? component2.sizeDelta.x : 62f);
			float num2 = Mathf.Max(num, num * (float)count);
			HorizontalLayoutGroup component3 = DFList_UI.GetComponent<HorizontalLayoutGroup>();
			float num3 = 6f;
			if ((bool)component3)
			{
				component3.padding.left = 15;
				component3.padding.right = 15;
				component3.padding.top = 8;
				component3.padding.bottom = 8;
				num2 += (float)(component3.padding.left + component3.padding.right) + (float)Mathf.Max(0, count - 1) * component3.spacing;
				num3 = component3.padding.top + component3.padding.bottom;
			}
			component.sizeDelta = new Vector2(num2, (component2 ? component2.sizeDelta.y : component.sizeDelta.y) + num3);
		}
	}

	private string BuildDFChoiceSwitchText(SkilDFData data)
	{
		if (data == null || !data.HasMultipleChoices)
		{
			return string.Empty;
		}
		return LOC.MM.GetMain(data.HasSelectedSkill ? "Right click to switch skill" : "Left click to switch skill");
	}

	private string BuildDFLieInfoText(int lieType)
	{
		return LOC.MM.GetMain("Increases per skill point invested") + " " + FormatDFLieBonus(0.2f) + " " + BuildDFLieStatText(lieType);
	}

	private string BuildDFLieCurrentBonusText(int lieType, float level)
	{
		return LOC.MM.GetMain("Current bonus") + " : + " + FormatDFLieBonus(0.2f * level) + " " + BuildDFLieStatText(lieType);
	}

	private string BuildDFLieStatText(int lieType)
	{
		if (lieType >= 0 && lieType <= 5)
		{
			return LOC.MM.GetMain(SWS.El_DMG(lieType));
		}
		return lieType switch
		{
			6 => LOC.MM.GetMain("HealthMax"), 
			7 => LOC.MM.GetMain("ManaMax"), 
			_ => $"Lie {lieType}", 
		};
	}

	private string FormatDFLieBonus(float value)
	{
		return $"{value:0.#}%";
	}

	private string BuildDFUnlockText(SkilDFData data)
	{
		if (data == null)
		{
			return string.Empty;
		}
		string text = LOC.MM.GetMain("Unlock") + " : ";
		bool hasAny = false;
		AppendDFLieUnlock(ref text, ref hasAny, data.LieA, data.Unlock_Point);
		AppendDFLieUnlock(ref text, ref hasAny, data.LieB, data.Unlock_Point);
		AppendDFLieUnlock(ref text, ref hasAny, data.LieC, data.Unlock_Point);
		AppendDFFatherUnlock(ref text, ref hasAny, data.FatherA);
		AppendDFFatherUnlock(ref text, ref hasAny, data.FatherB);
		AppendDFFatherUnlock(ref text, ref hasAny, data.FatherC);
		if (!hasAny)
		{
			return string.Empty;
		}
		return text;
	}

	private void AppendDFLieUnlock(ref string text, ref bool hasAny, int lie, int point)
	{
		if (!SkilDFData.IsNone(lie) && SingletonMonoScope<TalentManager>.HasInstance)
		{
			if (hasAny)
			{
				text += " / ";
			}
			text += $"Lie {lie} {SingletonMonoScope<TalentManager>.Instance.GetDFLiePoint(lie)}/{point}";
			hasAny = true;
		}
	}

	private void AppendDFFatherUnlock(ref string text, ref bool hasAny, int father)
	{
		if (!SkilDFData.IsNone(father))
		{
			if (hasAny)
			{
				text += " / ";
			}
			text += $"Father {father}";
			hasAny = true;
		}
	}

	public void ShowEmptySkillTip(Transform trans)
	{
		EmptyCAV.alpha = 1f;
		EmptyCAV.transform.position = new Vector3(trans.position.x, trans.position.y + 40f, trans.position.z);
		Empty_mainA.gameObject.SetActive(value: true);
		Empty_mainA.text = LOC.MM.GetMain("Empty Quickskill");
		Empty_mainB.text = "<color=#FFE397>" + LOC.MM.GetMain("click set skill") + "</color>";
	}

	public void ShowEmptyUseTip(Transform trans)
	{
		EmptyCAV.alpha = 1f;
		EmptyCAV.transform.position = new Vector3(trans.position.x, trans.position.y + 40f, trans.position.z);
		Empty_mainA.gameObject.SetActive(value: true);
		Empty_mainA.text = LOC.MM.GetMain("Empty Quickskill");
		Empty_mainB.text = "<color=#FFE397>" + LOC.MM.GetMain("click set potion") + "</color>";
	}

	public void HideEmptyTip()
	{
		EmptyCAV.alpha = 0f;
	}

	public void ShowTip(Transform trans, string text)
	{
		ShowTip(trans, text, string.Empty, 40f);
	}

	public void ShowTip(Transform trans, string text, string secondaryText)
	{
		ShowTip(trans, text, secondaryText, 40f);
	}

	public void ShowTip(Transform trans, string text, string secondaryText, float yOffset)
	{
		EmptyCAV.alpha = 1f;
		EmptyCAV.transform.position = new Vector3(trans.position.x, trans.position.y + yOffset, trans.position.z);
		bool flag = !string.IsNullOrEmpty(secondaryText);
		Empty_mainA.gameObject.SetActive(flag);
		if (flag)
		{
			Empty_mainA.text = LOC.MM.GetMain(text);
			Empty_mainB.text = "<color=#FFE397>" + LOC.MM.GetMain(secondaryText) + "</color>";
		}
		else
		{
			Empty_mainB.text = "<color=#FFE397>" + LOC.MM.GetMain(text) + "</color>";
		}
	}

	public void ShowTipWithShortcut(Transform trans, string text, ControlAction shortcutAction)
	{
		ShowTipWithShortcut(trans, text, shortcutAction, string.Empty);
	}

	public void ShowTipWithShortcut(Transform trans, string text, ControlAction shortcutAction, string secondaryText)
	{
		EmptyCAV.alpha = 1f;
		EmptyCAV.transform.position = new Vector3(trans.position.x, trans.position.y + 50f, trans.position.z);
		Empty_mainA.gameObject.SetActive(value: true);
		string main = LOC.MM.GetMain(text);
		string shortcutTipText = GetShortcutTipText(shortcutAction);
		if (!string.IsNullOrEmpty(secondaryText))
		{
			Empty_mainA.text = main;
			Empty_mainB.text = "<color=#FFE397>" + shortcutTipText + "\n" + LOC.MM.GetMain(secondaryText) + "</color>";
		}
		else
		{
			Empty_mainA.text = main;
			Empty_mainB.text = "<color=#FFE397>" + shortcutTipText + "</color>";
		}
	}

	public void ShowTipWithShortcutText(Transform trans, string text, ControlAction shortcutAction, string secondaryText)
	{
		EmptyCAV.alpha = 1f;
		EmptyCAV.transform.position = new Vector3(trans.position.x, trans.position.y + 50f, trans.position.z);
		Empty_mainA.gameObject.SetActive(value: true);
		string main = LOC.MM.GetMain(text);
		string shortcutTipText = GetShortcutTipText(shortcutAction);
		Empty_mainA.text = main;
		Empty_mainB.text = (string.IsNullOrEmpty(secondaryText) ? ("<color=#FFE397>" + shortcutTipText + "</color>") : ("<color=#FFE397>" + shortcutTipText + "\n" + secondaryText + "</color>"));
	}

	public void ShowTipWithShortcutInline(Transform trans, string text, ControlAction shortcutAction, string secondaryText)
	{
		EmptyCAV.alpha = 1f;
		EmptyCAV.transform.position = new Vector3(trans.position.x, trans.position.y + 50f, trans.position.z);
		Empty_mainA.gameObject.SetActive(value: true);
		string main = LOC.MM.GetMain(text);
		string main2 = LOC.MM.GetMain(secondaryText);
		string shortcutTipText = GetShortcutTipText(shortcutAction);
		Empty_mainA.text = main;
		Empty_mainB.text = "<color=#FFE397>" + main2 + " " + shortcutTipText + "</color>";
	}

	public void ShowTipRawText(Transform trans, string mainText, string secondaryText)
	{
		EmptyCAV.alpha = 1f;
		EmptyCAV.transform.position = new Vector3(trans.position.x, trans.position.y + 50f, trans.position.z);
		bool flag = !string.IsNullOrEmpty(mainText);
		Empty_mainA.gameObject.SetActive(flag);
		if (flag)
		{
			Empty_mainA.text = mainText;
		}
		Empty_mainB.text = (string.IsNullOrEmpty(secondaryText) ? string.Empty : ("<color=#FFE397>" + secondaryText + "</color>"));
	}

	private static string GetGamepadShortcutDisplay(ControlAction action)
	{
		string text = KeyDisplayUtil.ToDisplayName(Singleton<SettingDataManager>.Instance.GetControl(InputDeviceType.Gamepad).GetBind(action));
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return string.Empty;
	}

	private static string GetShortcutTipText(ControlAction action)
	{
		string text = KeyDisplayUtil.ToDisplayName(InputBind.GetBindKeyName(action));
		if (!string.IsNullOrEmpty(text))
		{
			return LOC.MM.GetMainFormat("Shortcut Key", text);
		}
		return LOC.MM.GetMain("Shortcut Key Not Set");
	}

	public void ShowACTListSkillTip(int xi, int type, string skillName, Transform trans, int TipType)
	{
		SkillCAV.alpha = 1f;
		switch (TipType)
		{
		case 0:
			Skill_Rect.pivot = new Vector2(1f, 0f);
			SkillCAV.transform.position = new Vector3(trans.position.x - 45f, trans.position.y + 45f, trans.position.z);
			break;
		case 1:
		{
			Vector3 screenPosition = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			if (screenPosition.y < 0.2f)
			{
				Skill_Rect.pivot = new Vector2(1f, 0f);
				SkillCAV.transform.position = new Vector3(trans.position.x - 45f, trans.position.y - 40f, trans.position.z);
			}
			else if (screenPosition.y > 0.2f && screenPosition.y < 0.7f)
			{
				Skill_Rect.pivot = new Vector2(1f, 0.5f);
				SkillCAV.transform.position = new Vector3(trans.position.x - 45f, trans.position.y, trans.position.z);
			}
			else
			{
				Skill_Rect.pivot = new Vector2(1f, 1f);
				SkillCAV.transform.position = new Vector3(trans.position.x - 45f, trans.position.y + 40f, trans.position.z);
			}
			break;
		}
		}
		Skill_lineA.SetActive(value: true);
		Skill_lineB.SetActive(value: true);
		Skill_lineC.SetActive(value: false);
		Skill_lineD.SetActive(value: false);
		Skill_cost.SetActive(value: true);
		Skill_next.gameObject.SetActive(value: false);
		Skill_type.gameObject.SetActive(value: false);
		switch (type)
		{
		case 0:
		{
			SingletonMonoScope<TalentManager>.Instance.XiData[xi].Sample_F.TryGetValue(skillName, out var value2);
			Skill_title.text = LOC.MM.GetSkill(value2.IndexName);
			Skill_mana.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("ManaCast"), value2.ManaCost_Last);
			if (value2.SampleSkill)
			{
				Skill_cd.gameObject.SetActive(value: false);
			}
			else
			{
				Skill_cd.gameObject.SetActive(value: true);
				if (value2.CoolDown_Last - value2.CoolDown_Last * PL.CoolDown_Max / 100f >= 0f)
				{
					Skill_cd.text = string.Format("{0} : {1}", LOC.MM.GetMain("CDtime"), value2.CoolDown_Last - value2.CoolDown_Last * SingletonMonoScope<PlayerManager>.Instance.CoolDown_Max / 100f);
				}
				else
				{
					Skill_cd.text = "0";
				}
			}
			if (value2.BuffTime_Last > 0f)
			{
				Skill_time.gameObject.SetActive(value: true);
				Skill_time.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("Duration"), value2.BuffTime_Last);
			}
			else
			{
				Skill_time.gameObject.SetActive(value: false);
			}
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value2.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value2.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			Skill_main.text = value2.GetInfoA();
			break;
		}
		case 2:
		{
			SingletonMonoScope<TalentManager>.Instance.XiData[xi].Comp_F.TryGetValue(skillName, out var value);
			Skill_title.text = LOC.MM.GetSkill(value.IndexName);
			Skill_mana.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("ManaCast"), value.ManaCost_Last);
			Skill_cd.gameObject.SetActive(value: true);
			Skill_cd.text = string.Format("{0} : {1:0.##}", LOC.MM.GetMain("CDtime"), value.CoolDown_Last);
			Skill_time.gameObject.SetActive(value: false);
			Skill_main.text = value.GetInfoA();
			if (SingletonMonoScope<TalentManager>.Instance.XiData[xi].Level_Base < value.UnLock_Point)
			{
				Skill_unlock.gameObject.SetActive(value: true);
				Skill_unlock.text = string.Format("{0} {1} {2}", LOC.MM.GetMain("Need to invest"), value.UnLock_Point, LOC.MM.GetMain("skill points to unlock"));
			}
			else
			{
				Skill_unlock.gameObject.SetActive(value: false);
			}
			break;
		}
		}
	}

	public void ShowCompItemSkillTip(ACTListSkillBT skill, Transform trans)
	{
		if ((bool)skill)
		{
			ShowACTListSkillTip(skill.Xi, skill.SkillType, skill.IndexName, trans, 1);
			SetCompItemSkillTipPosition(trans);
			Skill_cost.SetActive(value: true);
			Skill_mana.gameObject.SetActive(value: true);
			Skill_mana.text = LOC.MM.GetMain("DismissOneCompanionShortcut");
			Skill_cd.gameObject.SetActive(value: false);
			Skill_time.gameObject.SetActive(value: false);
		}
	}

	private void SetCompItemSkillTipPosition(Transform trans)
	{
		Skill_Rect.pivot = new Vector2(0f, 1f);
		RectTransform rectTransform = trans as RectTransform;
		if (!rectTransform)
		{
			SkillCAV.transform.position = new Vector3(trans.position.x, trans.position.y - 8f, trans.position.z);
			return;
		}
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		SkillCAV.transform.position = array[0] + new Vector3(0f, -8f, 0f);
	}

	public void ShowACTUseTip(Vector3 trans, UseItemClass item)
	{
		WeaponCavA.alpha = 1f;
		ResetWeaponTipAExtraBlocks();
		WP_RectA.pivot = new Vector2(1f, 0f);
		WeaponCavA.transform.position = new Vector3(trans.x - 45f, trans.y + 45f, trans.z);
		WP_lineA_A.SetActive(value: true);
		WP_lineA_B.SetActive(value: true);
		WP_lineA_C.SetActive(value: false);
		WP_lineA_D.SetActive(value: false);
		WP_lineA_E.SetActive(value: false);
		SetTipLine(WP_lineA_I, visible: true);
		WP_specialA.gameObject.SetActive(value: false);
		GameObject[] array = skillOBJA;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		array = WP_baoshiOBJA;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: false);
		}
		WP_titleA.text = item.GetTitle();
		switch (item.InfoType)
		{
		case 0:
			WP_typeA.text = "<color=#BAFDFF>" + LOC.MM.GetMain("Restorative Potion") + "</color>";
			WP_levelA.text = string.Format("<color=#75FFAB>{0} : {1}{2}</color>", LOC.MM.GetMain("CDtime"), item.CDTime, LOC.MM.GetMain("S"));
			break;
		case 1:
			WP_typeA.text = "<color=#D5FFC5>" + LOC.MM.GetMain("Enhanced Potion") + "</color>";
			WP_levelA.text = string.Format("<color=#E675FF>{0} : {1}{2}</color>", LOC.MM.GetMain("Duration"), item.Duration, LOC.MM.GetMain("S"));
			break;
		case 2:
			WP_typeA.text = "<color=#D3EAFF>" + LOC.MM.GetMain("Scroll") + "</color>";
			WP_levelA.text = null;
			break;
		case 3:
			WP_typeA.text = "<color=#FFC284>" + LOC.MM.GetMain("Permanent Potion") + "</color>";
			WP_levelA.text = null;
			break;
		case 4:
			WP_typeA.text = "<color=#FFC0F3>" + LOC.MM.GetMain("Special Stone") + "</color>";
			WP_levelA.text = null;
			break;
		case 5:
			WP_typeA.text = "<color=#FFC0F3>" + LOC.MM.GetMain("Special Potion") + "</color>";
			WP_levelA.text = null;
			break;
		case 6:
			WP_typeA.text = "<color=#FFEE78>" + LOC.MM.GetMain("Special Item") + "</color>";
			WP_levelA.text = null;
			break;
		case 7:
			WP_typeA.text = "<color=#FFEE78>" + LOC.MM.GetMain("Special Item") + "</color>";
			WP_levelA.text = null;
			break;
		}
		WP_mainA.text = item.GetMain();
		SetTipLine(WP_lineA_I, visible: false);
		WP_YJ.gameObject.SetActive(value: false);
		WP_Set.gameObject.SetActive(value: false);
		priceA.SetActive(value: false);
	}

	public void ShowEnemyTip(int a)
	{
		switch (a)
		{
		case 0:
			EnemyTipCAV.alpha = 0f;
			break;
		case 1:
			EnemyTipCAV.alpha = 1f;
			break;
		}
	}

	public void ShowBossTip(int a)
	{
		switch (a)
		{
		case 0:
			BossTipCAV.alpha = 0f;
			break;
		case 1:
			BossTipCAV.alpha = 1f;
			break;
		}
	}
}
