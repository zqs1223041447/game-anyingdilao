using System;
using Core;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Inputs;
using Mijing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class MijingPanel : GamepadSelectablePanel
{
	private static readonly Color EasyTextColor = new Color(64f / 85f, 61f / 85f, 0.5921569f, 1f);

	private static readonly Color MediumTextColor = new Color(0.78f, 0.68f, 1f, 1f);

	private static readonly Color HardTextColor = new Color(1f, 0.55f, 0.55f, 1f);

	private static readonly Color MasterTextColor = new Color(1f, 0.55f, 0.16f, 1f);

	private Button easyBtn;

	private Button mediumBtn;

	private Button hardBtn;

	private Button masterBtn;

	private Button floorLeftBtn;

	private Button floorRightBtn;

	private Button enterBtn;

	private Text difficultText;

	private Text introText;

	private Text floorText;

	private Text priceText;

	private DifficultType currentDifficultType;

	private int currentSelectedFloor = 1;

	protected override void Awake()
	{
		base.Awake();
		easyBtn = GetControl<Button>("EasyBtn");
		easyBtn.onClick.AddListener(OnClickEasy);
		mediumBtn = GetControl<Button>("MediumBtn");
		mediumBtn.onClick.AddListener(OnClickMedium);
		hardBtn = GetControl<Button>("HardBtn");
		hardBtn.onClick.AddListener(OnClickHard);
		masterBtn = GetControl<Button>("MasterBtn");
		masterBtn.onClick.AddListener(OnClickMaster);
		enterBtn = GetControl<Button>("EnterBtn");
		enterBtn.onClick.AddListener(OnClickEnter);
		floorLeftBtn = GetControl<Button>("FloorLeftBtn");
		floorLeftBtn.onClick.AddListener(OnFloorLeft);
		floorRightBtn = GetControl<Button>("FloorRightBtn");
		floorRightBtn.onClick.AddListener(OnFloorRight);
		difficultText = GetControl<Text>("DifficultText");
		introText = GetControl<Text>("IntroText");
		floorText = GetControl<Text>("FloorText");
		priceText = GetControl<Text>("PriceText");
		EnsureValidSelectedFloor();
		RefreshUI();
	}

	protected override void ClickBtn(string btnName)
	{
		if (btnName == "CloseBtn")
		{
			Time.timeScale = 1f;
			Singleton<UIManager>.Instance.HidePanel<MijingPanel>();
		}
	}

	private void Update()
	{
		HandleFloorShortcutInput();
	}

	private void OnClickEasy()
	{
		if (currentDifficultType != 0)
		{
			currentDifficultType = DifficultType.Easy;
			currentSelectedFloor = 1;
			if (SingletonMonoScope<MijingManager>.HasInstance)
			{
				SingletonMonoScope<MijingManager>.Instance.ApplyDifficulty(currentDifficultType);
			}
			RefreshUI();
		}
	}

	private void OnClickMedium()
	{
		if (currentDifficultType != DifficultType.Medium)
		{
			currentDifficultType = DifficultType.Medium;
			currentSelectedFloor = 1;
			if (SingletonMonoScope<MijingManager>.HasInstance)
			{
				SingletonMonoScope<MijingManager>.Instance.ApplyDifficulty(currentDifficultType);
			}
			RefreshUI();
		}
	}

	private void OnClickHard()
	{
		if (currentDifficultType != DifficultType.Hard)
		{
			currentDifficultType = DifficultType.Hard;
			currentSelectedFloor = 1;
			if (SingletonMonoScope<MijingManager>.HasInstance)
			{
				SingletonMonoScope<MijingManager>.Instance.ApplyDifficulty(currentDifficultType);
			}
			RefreshUI();
		}
	}

	private void OnClickMaster()
	{
		if (currentDifficultType != DifficultType.Master)
		{
			currentDifficultType = DifficultType.Master;
			currentSelectedFloor = 1;
			if (SingletonMonoScope<MijingManager>.HasInstance)
			{
				SingletonMonoScope<MijingManager>.Instance.ApplyDifficulty(currentDifficultType);
			}
			RefreshUI();
		}
	}

	private void OnClickEnter()
	{
		if (!SingletonMonoScope<MijingManager>.HasInstance)
		{
			return;
		}
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			long num = (long)Mathf.Round(SingletonMonoScope<MijingManager>.Instance.GetEnterPriceMultiplier(currentSelectedFloor));
			if (SingletonMonoScope<InventoryManager>.Instance.GlobalMoney < num)
			{
				GameManager.ShowTipLocalStartKey("money_not_enough", TipType.Fail);
				return;
			}
			SingletonMonoScope<InventoryManager>.Instance.RemoveMoney(num);
			GameManager.ShowTip(LOC.MM.GetMainFormat("shop_buy_success", num), TipType.Success);
		}
		SingletonMonoScope<MijingManager>.Instance.EnterMijing(currentSelectedFloor);
	}

	private static int GetMaxSelectableFloor()
	{
		if (!SaveManager.HasRuntime)
		{
			return int.MaxValue;
		}
		return SingletonMonoScope<MijingManager>.Instance.GetUnlockedFloorByCurrentDifficulty();
	}

	private void OnFloorLeft()
	{
		int floorStep = GetFloorStep();
		if (currentSelectedFloor - floorStep >= 1)
		{
			currentSelectedFloor -= floorStep;
			RefreshUI();
		}
	}

	private void OnFloorRight()
	{
		int floorStep = GetFloorStep();
		int maxSelectableFloor = GetMaxSelectableFloor();
		if (currentSelectedFloor + floorStep <= maxSelectableFloor)
		{
			currentSelectedFloor += floorStep;
			RefreshUI();
		}
	}

	private void HandleFloorShortcutInput()
	{
		if (InputBind.GetDown(ControlAction.PageL))
		{
			ChangeFloorByShortcut(left: true);
		}
		else if (InputBind.GetDown(ControlAction.PageR))
		{
			ChangeFloorByShortcut(left: false);
		}
	}

	private void ChangeFloorByShortcut(bool left)
	{
		if (left)
		{
			OnFloorLeft();
		}
		else
		{
			OnFloorRight();
		}
	}

	public void RefreshUI()
	{
		RefreshDifficultyText();
		RefreshFloorText();
		RefreshPriceText();
		RefreshIntroText();
		RefreshFloorArrowState();
	}

	private void RefreshDifficultyText()
	{
		switch (currentDifficultType)
		{
		case DifficultType.Easy:
			difficultText.text = LOC.MM.GetLevel("easy_mijing");
			difficultText.color = EasyTextColor;
			break;
		case DifficultType.Medium:
			difficultText.text = LOC.MM.GetLevel("medium_mijing");
			difficultText.color = MediumTextColor;
			break;
		case DifficultType.Hard:
			difficultText.text = LOC.MM.GetLevel("hard_mijing");
			difficultText.color = HardTextColor;
			break;
		case DifficultType.Master:
			difficultText.text = LOC.MM.GetLevel("master_mijing");
			difficultText.color = MasterTextColor;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void RefreshFloorText()
	{
		if ((bool)floorText)
		{
			floorText.text = LOC.MM.GetLevelFormat("mijing_floor", currentSelectedFloor);
		}
	}

	private void RefreshPriceText()
	{
		if ((bool)priceText)
		{
			if (!SingletonMonoScope<MijingManager>.HasInstance)
			{
				priceText.text = string.Empty;
				return;
			}
			long num = (long)Mathf.Round(SingletonMonoScope<MijingManager>.Instance.GetEnterPriceMultiplier(currentSelectedFloor));
			priceText.text = LOC.MM.GetLevelFormat("mijing_need_price", num);
		}
	}

	private void RefreshIntroText()
	{
		if ((bool)introText)
		{
			if (!SingletonMonoScope<MijingManager>.HasInstance)
			{
				introText.text = string.Empty;
				return;
			}
			MijingManager instance = SingletonMonoScope<MijingManager>.Instance;
			introText.text = LOC.MM.GetLevel("mijing_heal") + " " + FormatPercent(instance.GetEnemyHealthMultiplier(currentSelectedFloor)) + "\n" + LOC.MM.GetLevel("mijing_dmg") + " " + FormatPercent(instance.GetEnemyDamageMultiplier(currentSelectedFloor)) + "\n" + LOC.MM.GetLevel("mijing_anti") + " " + FormatPercentBonus(instance.GetEnemyDamageReductionMultiplier(currentSelectedFloor)) + "\n" + LOC.MM.GetLevel("mijing_chuan") + " " + FormatPercentBonus(instance.GetEnemyPenetrationMultiplier(currentSelectedFloor)) + "\n" + LOC.MM.GetLevel("mijing_drop") + " " + FormatPercent(instance.GetPlayerDropRateMultiplier(currentSelectedFloor)) + "\n" + LOC.MM.GetLevel("mijing_xp") + " " + FormatPercent(instance.GetEnemyXpMultiplier(currentSelectedFloor)) + "\n" + LOC.MM.GetLevel("mijing_rare_drop") + " " + FormatPercent(instance.GetRareItemDropRateMultiplier(currentSelectedFloor));
		}
	}

	private void RefreshFloorArrowState()
	{
		int maxSelectableFloor = GetMaxSelectableFloor();
		if ((bool)floorLeftBtn)
		{
			floorLeftBtn.interactable = currentSelectedFloor > 1;
		}
		if ((bool)floorRightBtn)
		{
			floorRightBtn.interactable = currentSelectedFloor < maxSelectableFloor;
		}
	}

	private static int GetFloorStep()
	{
		if (SingletonMonoScope<MijingManager>.HasInstance)
		{
			return Mathf.Max(1, SingletonMonoScope<MijingManager>.Instance.mijingSettings.intervalFloorNum);
		}
		return 1;
	}

	private void EnsureValidSelectedFloor()
	{
		int maxSelectableFloor = GetMaxSelectableFloor();
		if (currentSelectedFloor < 1)
		{
			currentSelectedFloor = 1;
		}
		else if (currentSelectedFloor > maxSelectableFloor)
		{
			currentSelectedFloor = maxSelectableFloor;
		}
	}

	private static string FormatPercent(float multiplier)
	{
		return $"{multiplier * 100f:0.##}%";
	}

	private static string FormatPercentBonus(float multiplier)
	{
		return $"{(multiplier - 1f) * 100f:0.##}%";
	}

	public override void OnShow()
	{
		if (SingletonMonoScope<MijingManager>.HasInstance)
		{
			currentDifficultType = SingletonMonoScope<MijingManager>.Instance.CurrentDifficulty;
		}
		RefreshUI();
		SetFirstSelected(easyBtn);
		if (SingletonMonoScope<InputManager>.HasInstance)
		{
			InputManager.AllActionToggle = false;
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
		Singleton<UIManager>.Instance.HidePanel<MijingPanel>();
		return true;
	}
}
