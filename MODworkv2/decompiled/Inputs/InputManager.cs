using System;
using System.Collections.Generic;
using Data.AutoGen.DataClass.Settings;
using Entity.Character.Player;
using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using Interact;
using UI.Managers;
using UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inputs;

public class InputManager : SingletonMonoScope<InputManager>
{
	private struct ActionBindingCache
	{
		public bool UsesUiOccupiedKey;
	}

	private PlayerManager _playerManager;

	private ACTbar actBar;

	public static bool AllActionToggle = true;

	private bool mouseInteractionLocked;

	private bool gamepadInteractionLocked;

	private bool isPcCurrent;

	private bool pointerOverUI;

	private bool anySkillDownThisFrame;

	private bool anyItemDownThisFrame;

	private bool pointerConflictUiBlockedThisFrame;

	private bool pointerConflictInteractableBlockedThisFrame;

	private bool skill1Down;

	private bool skill2Down;

	private bool skill3Down;

	private bool skill4Down;

	private bool skill5Down;

	private bool skill6Down;

	private bool skill7Down;

	private bool skill8Down;

	private bool skill1Hold;

	private bool skill2Hold;

	private bool skill3Hold;

	private bool skill4Hold;

	private bool skill5Hold;

	private bool skill6Hold;

	private bool skill7Hold;

	private bool submitDown;

	private bool cancelDown;

	private bool mouseLeftDown;

	private bool mouseRightDown;

	private bool item1Down;

	private bool item2Down;

	private bool townPortalDown;

	private float quickUseAllBuffCooldownUntil;

	private int sellAllShortcutQuality = 1;

	private int contextualShortcutHandledFrame = -1;

	private static readonly ControlAction[] GameplayActionsToSuppressOnUnlock = new ControlAction[20]
	{
		ControlAction.Up,
		ControlAction.Down,
		ControlAction.Left,
		ControlAction.Right,
		ControlAction.Skill1,
		ControlAction.Skill2,
		ControlAction.Skill3,
		ControlAction.Skill4,
		ControlAction.Skill5,
		ControlAction.Skill6,
		ControlAction.Skill7,
		ControlAction.Skill8,
		ControlAction.Item1,
		ControlAction.Item2,
		ControlAction.PickUp,
		ControlAction.TP,
		ControlAction.MapMode,
		ControlAction.QuickUse,
		ControlAction.Mercenary,
		ControlAction.AutoAT
	};

	private static readonly ControlAction[] GameplayButtonActionsToSuppressOnUnlock = new ControlAction[16]
	{
		ControlAction.Skill1,
		ControlAction.Skill2,
		ControlAction.Skill3,
		ControlAction.Skill4,
		ControlAction.Skill5,
		ControlAction.Skill6,
		ControlAction.Skill7,
		ControlAction.Skill8,
		ControlAction.Item1,
		ControlAction.Item2,
		ControlAction.PickUp,
		ControlAction.TP,
		ControlAction.MapMode,
		ControlAction.QuickUse,
		ControlAction.Mercenary,
		ControlAction.AutoAT
	};

	private static readonly ControlAction[] ContextualContainerShortcutActions = new ControlAction[6]
	{
		ControlAction.Sell,
		ControlAction.SellAll,
		ControlAction.PageL,
		ControlAction.PageR,
		ControlAction.SortAll,
		ControlAction.Sort
	};

	private float pickupHoldNextTime;

	private bool pickupHoldTriggered;

	private readonly Dictionary<ControlAction, ActionBindingCache> _actionBindingCache = new Dictionary<ControlAction, ActionBindingCache>();

	private InputDeviceType _bindingCacheDeviceType;

	private bool _bindingCacheBuilt;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
		actBar = SingletonMonoScope<ACTbar>.Instance;
	}

	private void Update()
	{
		if (!AllActionToggle || Time.timeScale == 0f)
		{
			return;
		}
		if (!SingletonMonoScope<ShopManager>.HasInstance || !SingletonMonoScope<ShopManager>.Instance.Opened)
		{
			ResetSellAllQualityShortcut();
		}
		if (HandleContextualContainerShortcuts() || HandleQuickUseShortcut() || HandleMercenaryShortcut() || HandleAutoAttackShortcut())
		{
			return;
		}
		if (!GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.Talent) && InputBind.GetDown(ControlAction.Talent))
		{
			SingletonMonoScope<GameUIManager>.Instance.OpenClose_Talent();
		}
		if (!GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.Stats) && InputBind.GetDown(ControlAction.Stats))
		{
			SingletonMonoScope<GameUIManager>.Instance.OpenClose_Character();
		}
		if (!GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.Bag) && InputBind.GetDown(ControlAction.Bag) && !CloseCraftingOrReforgeUiFromBagShortcut())
		{
			if (SingletonMonoScope<ShopManager>.Instance.Opened || SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse)
			{
				SingletonMonoScope<InventoryManager>.Instance.CloseUI();
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.OpenClose_IV();
			}
		}
	}

	private static bool CloseCraftingOrReforgeUiFromBagShortcut()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return false;
		}
		bool result = false;
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_baoshi && SingletonMonoScope<BaoshiManager>.HasInstance)
		{
			SingletonMonoScope<BaoshiManager>.Instance.CloseBaoshi();
			result = true;
		}
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_weapon && SingletonMonoScope<WeaponManager>.HasInstance)
		{
			SingletonMonoScope<WeaponManager>.Instance.CloseWeapon();
			result = true;
		}
		return result;
	}

	private void LateUpdate()
	{
		HandleInteractionLockRelease();
		if (AllActionToggle && Time.timeScale != 0f && contextualShortcutHandledFrame != Time.frameCount)
		{
			CacheFrameContext();
			if (!HandleWorldInteraction() && !HandlePcPickupInput() && !IsInteractionLocked() && _playerManager.IsAlive)
			{
				HandleSkillInput(ControlAction.Skill1, skill1Down);
				HandleSkillInput(ControlAction.Skill2, skill2Down);
				HandleSkillInput(ControlAction.Skill3, skill3Down);
				HandleSkillInput(ControlAction.Skill4, skill4Down);
				HandleSkillInput(ControlAction.Skill5, skill5Down);
				HandleSkillInput(ControlAction.Skill6, skill6Down);
				HandleSkillInput(ControlAction.Skill7, skill7Down);
				HandleSkillInput(ControlAction.Skill8, skill8Down);
				HandleUseInput(ControlAction.Item1, item1Down);
				HandleUseInput(ControlAction.Item2, item2Down);
				HandleTownPortalInput(townPortalDown);
			}
		}
	}

	private void CacheFrameContext()
	{
		isPcCurrent = SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent();
		if (isPcCurrent)
		{
			mouseLeftDown = Input.GetMouseButtonDown(0);
			mouseRightDown = Input.GetMouseButtonDown(1);
			submitDown = false;
			cancelDown = false;
			pointerOverUI = (bool)EventSystem.current && EventSystem.current.IsPointerOverGameObject();
		}
		else
		{
			mouseLeftDown = false;
			mouseRightDown = false;
			submitDown = GamepadInputManager.GetSubmitDown();
			cancelDown = GamepadInputManager.GetCancelDown();
			pointerOverUI = SingletonMonoGlobal<CursorUIManager>.HasInstance && SingletonMonoGlobal<CursorUIManager>.Instance.IsPointerOverUI();
		}
		skill1Down = InputBind.GetDown(ControlAction.Skill1);
		skill2Down = InputBind.GetDown(ControlAction.Skill2);
		skill3Down = InputBind.GetDown(ControlAction.Skill3);
		skill4Down = InputBind.GetDown(ControlAction.Skill4);
		skill5Down = InputBind.GetDown(ControlAction.Skill5);
		skill6Down = InputBind.GetDown(ControlAction.Skill6);
		skill7Down = InputBind.GetDown(ControlAction.Skill7);
		skill8Down = InputBind.GetDown(ControlAction.Skill8);
		item1Down = InputBind.GetDown(ControlAction.Item1);
		item2Down = InputBind.GetDown(ControlAction.Item2);
		townPortalDown = InputBind.GetDown(ControlAction.TP);
		anySkillDownThisFrame = skill1Down || skill2Down || skill3Down || skill4Down || skill5Down || skill6Down || skill7Down || skill8Down;
		anyItemDownThisFrame = item1Down || item2Down;
		CachePointerConflictContext();
	}

	public void ResetSellAllQualityShortcut()
	{
		sellAllShortcutQuality = 1;
	}

	private bool HandleQuickUseShortcut()
	{
		if (GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.QuickUse))
		{
			return false;
		}
		if (!InputBind.GetDown(ControlAction.QuickUse))
		{
			return false;
		}
		if (Time.time < quickUseAllBuffCooldownUntil)
		{
			return true;
		}
		quickUseAllBuffCooldownUntil = Time.time + 2f;
		if (SingletonMonoScope<InventoryManager>.HasInstance)
		{
			SingletonMonoScope<InventoryManager>.Instance.UseAllDurationBuffPotionsFromShortcut();
		}
		InputBind.SuppressHeldUntilRelease(ControlAction.QuickUse);
		return true;
	}

	private bool HandleMercenaryShortcut()
	{
		if (GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.Mercenary))
		{
			return false;
		}
		if (!InputBind.GetDown(ControlAction.Mercenary))
		{
			return false;
		}
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.OpenClose_Mercenary();
		}
		InputBind.SuppressHeldUntilRelease(ControlAction.Mercenary);
		return true;
	}

	private bool HandleAutoAttackShortcut()
	{
		if (GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.AutoAT))
		{
			return false;
		}
		if (!InputBind.GetDown(ControlAction.AutoAT))
		{
			return false;
		}
		if (!actBar || !actBar.ToggleAutoAttackFromShortcut())
		{
			return false;
		}
		InputBind.SuppressHeldUntilRelease(ControlAction.AutoAT);
		return true;
	}

	private bool HandleContextualContainerShortcuts()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return false;
		}
		if (HandleContextualTalentShortcuts())
		{
			return true;
		}
		if (!IsContainerContextOpen())
		{
			return false;
		}
		bool flag = false;
		if (InputBind.GetDown(ControlAction.PageL))
		{
			HandleContextualPageShortcut(turnLeft: true);
			flag = true;
		}
		else if (InputBind.GetDown(ControlAction.PageR))
		{
			HandleContextualPageShortcut(turnLeft: false);
			flag = true;
		}
		else
		{
			if (!SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				return false;
			}
			if (Input.GetKeyDown(KeyCode.JoystickButton2) && HandleGamepadXContainerShortcut())
			{
				flag = true;
			}
			else if (InputBind.GetDown(ControlAction.Sell))
			{
				if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened && SingletonMonoScope<InventoryManager>.HasInstance)
				{
					SingletonMonoScope<InventoryManager>.Instance.QuickSell();
				}
				flag = true;
			}
			else if (InputBind.GetDown(ControlAction.SellAll))
			{
				if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened && SingletonMonoScope<InventoryManager>.HasInstance)
				{
					if (IsGamepadSellOnlyOtherClassesShortcut())
					{
						SingletonMonoScope<InventoryManager>.Instance.SellAutoOtherClassWeaponsAllQualities();
					}
					else
					{
						SingletonMonoScope<InventoryManager>.Instance.SellAuto(sellAllShortcutQuality);
						sellAllShortcutQuality = Mathf.Min(sellAllShortcutQuality + 1, 6);
					}
				}
				flag = true;
			}
			else if (InputBind.GetDown(ControlAction.SortAll))
			{
				HandleContextualSortShortcut(allPages: true);
				flag = true;
			}
			else if (InputBind.GetDown(ControlAction.Sort))
			{
				HandleContextualSortShortcut(allPages: false);
				flag = true;
			}
		}
		if (!flag)
		{
			return false;
		}
		contextualShortcutHandledFrame = Time.frameCount;
		if (SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			InputBind.SuppressHeldUntilRelease(GameplayActionsToSuppressOnUnlock);
		}
		InputBind.SuppressHeldUntilRelease(ContextualContainerShortcutActions);
		return true;
	}

	private static bool IsGamepadSellOnlyOtherClassesShortcut()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return Input.GetKey(KeyCode.JoystickButton8);
		}
		return false;
	}

	private static bool HandleContextualTalentShortcuts()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance || !SingletonMonoScope<GameUIManager>.Instance.Opened_Talent || !SingletonMonoScope<TalentManager>.HasInstance)
		{
			return false;
		}
		if (InputBind.GetDown(ControlAction.PageL))
		{
			SingletonMonoScope<TalentManager>.Instance.ChangePageByShortcut(left: true);
			InputBind.SuppressHeldUntilRelease(ContextualContainerShortcutActions);
			return true;
		}
		if (InputBind.GetDown(ControlAction.PageR))
		{
			SingletonMonoScope<TalentManager>.Instance.ChangePageByShortcut(left: false);
			InputBind.SuppressHeldUntilRelease(ContextualContainerShortcutActions);
			return true;
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return false;
		}
		if (Input.GetKeyDown(KeyCode.JoystickButton2) && TryHandleTalentSkillShortcut(includeChildren: false))
		{
			return true;
		}
		if (Input.GetKeyDown(KeyCode.JoystickButton3) && TryHandleTalentSkillShortcut(includeChildren: true))
		{
			return true;
		}
		return false;
	}

	private static bool TryHandleTalentSkillShortcut(bool includeChildren)
	{
		if (TryGetUiComponentUnderCursor<SKillBT_DF>(out var component))
		{
			if (includeChildren)
			{
				component.AddFullFromShortcut();
			}
			else
			{
				component.AddFillFromShortcut();
			}
			return true;
		}
		if (TryGetUiComponentUnderCursor<SkillBT>(out var component2))
		{
			if (includeChildren)
			{
				component2.AddCurrentAndChildrenFromShortcut();
			}
			else
			{
				component2.AddFillFromShortcut();
			}
			return true;
		}
		return false;
	}

	private static bool TryGetUiComponentUnderCursor<T>(out T component) where T : Component
	{
		component = null;
		if (!SingletonMonoScope<CursorInputManager>.HasInstance || !EventSystem.current)
		{
			return false;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i].gameObject;
			if ((bool)gameObject && gameObject.activeInHierarchy)
			{
				component = gameObject.GetComponentInParent<T>();
				if ((bool)component)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool HandleGamepadXContainerShortcut()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return false;
		}
		bool flag = IsCursorOnLeftHalfOfScreen();
		if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened)
		{
			if (!flag && SingletonMonoScope<InventoryManager>.HasInstance)
			{
				return SingletonMonoScope<InventoryManager>.Instance.TryGamepadQuickSellUnderCursor();
			}
			return false;
		}
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse)
		{
			if (flag)
			{
				if (SingletonMonoScope<WarehouseManager>.HasInstance)
				{
					return SingletonMonoScope<WarehouseManager>.Instance.TryGamepadSendToInventoryUnderCursor();
				}
				return false;
			}
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				return SingletonMonoScope<InventoryManager>.Instance.TryGamepadSendToWarehouseUnderCursor();
			}
			return false;
		}
		if (SingletonMonoScope<GameUIManager>.Instance.Opened_IV)
		{
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				return SingletonMonoScope<InventoryManager>.Instance.TryGamepadDropUnderCursor();
			}
			return false;
		}
		return false;
	}

	private static bool IsContainerContextOpen()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return false;
		}
		if (!SingletonMonoScope<GameUIManager>.Instance.Opened_IV && !SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse)
		{
			return SingletonMonoScope<GameUIManager>.Instance.Opened_shop;
		}
		return true;
	}

	private static void HandleContextualPageShortcut(bool turnLeft)
	{
		bool flag = IsCursorOnLeftHalfOfScreen();
		if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse && flag)
		{
			if (SingletonMonoScope<WarehouseManager>.HasInstance)
			{
				SingletonMonoScope<WarehouseManager>.Instance.ChangePage(turnLeft);
				SingletonMonoScope<WarehouseManager>.Instance.RefreshPointerSlotStateAndTip();
			}
		}
		else if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened && flag)
		{
			SingletonMonoScope<ShopManager>.Instance.ChangePage(turnLeft);
			SingletonMonoScope<ShopManager>.Instance.RefreshPointerSlotStateAndTip();
		}
		else if (SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<GameUIManager>.HasInstance && (SingletonMonoScope<GameUIManager>.Instance.Opened_IV || SingletonMonoScope<GameUIManager>.Instance.Opened_shop))
		{
			SingletonMonoScope<InventoryManager>.Instance.ChangePage(turnLeft);
			SingletonMonoScope<InventoryManager>.Instance.RefreshPointerSlotStateAndTip();
		}
	}

	private static void HandleContextualSortShortcut(bool allPages)
	{
		if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened && IsCursorOnLeftHalfOfScreen())
		{
			SingletonMonoScope<ShopManager>.Instance.RefreshShop();
			SingletonMonoScope<ShopManager>.Instance.RefreshPointerSlotStateAndTip();
		}
		else if (SingletonMonoScope<GameUIManager>.HasInstance && SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse && IsCursorOnLeftHalfOfScreen())
		{
			if (SingletonMonoScope<WarehouseManager>.HasInstance)
			{
				if (allPages)
				{
					SingletonMonoScope<WarehouseManager>.Instance.SortAll();
				}
				else
				{
					SingletonMonoScope<WarehouseManager>.Instance.SortCur();
				}
				SingletonMonoScope<WarehouseManager>.Instance.RefreshPointerSlotStateAndTip();
			}
		}
		else if (SingletonMonoScope<InventoryManager>.HasInstance && SingletonMonoScope<GameUIManager>.HasInstance && (SingletonMonoScope<GameUIManager>.Instance.Opened_IV || SingletonMonoScope<GameUIManager>.Instance.Opened_shop))
		{
			if (allPages)
			{
				SingletonMonoScope<InventoryManager>.Instance.SortAll();
			}
			else
			{
				SingletonMonoScope<InventoryManager>.Instance.SortCur();
			}
			SingletonMonoScope<InventoryManager>.Instance.RefreshPointerSlotStateAndTip();
		}
	}

	private static bool IsCursorOnLeftHalfOfScreen()
	{
		Vector3 obj = (SingletonMonoScope<CursorInputManager>.HasInstance ? SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition : Input.mousePosition);
		return obj.x < (float)Screen.width * 0.5f;
	}

	private bool HandlePcPickupInput()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return false;
		}
		PcPickupMode pcPickupMode = Singleton<SettingDataManager>.Instance.GetGame().pcPickupMode;
		if (pcPickupMode == PcPickupMode.Off || GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.PickUp))
		{
			pickupHoldTriggered = false;
			return false;
		}
		bool down = InputBind.GetDown(ControlAction.PickUp);
		bool flag = InputBind.Get(ControlAction.PickUp);
		if (InputBind.GetUp(ControlAction.PickUp))
		{
			pickupHoldTriggered = false;
			return false;
		}
		if (down)
		{
			pickupHoldTriggered = true;
			pickupHoldNextTime = Time.time + 0.12f;
			if (SingletonMonoScope<PCPickupTargetManager>.HasInstance && SingletonMonoScope<PCPickupTargetManager>.Instance.TryPickup(pcPickupMode))
			{
				return true;
			}
			return false;
		}
		if (flag && pickupHoldTriggered && Time.time >= pickupHoldNextTime)
		{
			pickupHoldNextTime = Time.time + 0.08f;
			if (SingletonMonoScope<PCPickupTargetManager>.HasInstance && SingletonMonoScope<PCPickupTargetManager>.Instance.TryPickup(pcPickupMode))
			{
				return true;
			}
		}
		return false;
	}

	private void CachePointerConflictContext()
	{
		pointerConflictUiBlockedThisFrame = false;
		pointerConflictInteractableBlockedThisFrame = false;
		if (isPcCurrent)
		{
			if (SingletonMonoScope<CursorInputManager>.HasInstance)
			{
				pointerConflictUiBlockedThisFrame = CursorManager.IsScreenPositionOverUI(SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition);
			}
			if (SingletonMonoScope<InteractionManager>.HasInstance)
			{
				IInteractable currentTarget = SingletonMonoScope<InteractionManager>.Instance.CurrentTarget;
				if (currentTarget != null && InteractionManager.CanInteractNow() && currentTarget.CanInteract())
				{
					pointerConflictInteractableBlockedThisFrame = true;
				}
			}
		}
		else if (SingletonMonoScope<GameUIManager>.Instance.IsAnyPanelOpened() && CursorInputManager.IsUsingVirtualMouse && SingletonMonoGlobal<CursorUIManager>.HasInstance && SingletonMonoGlobal<CursorUIManager>.Instance.IsPointerOverUI())
		{
			pointerConflictUiBlockedThisFrame = true;
		}
	}

	private bool HandleWorldInteraction()
	{
		if (!SingletonMonoScope<InteractionManager>.HasInstance)
		{
			return false;
		}
		if (InteractionManager.IsCursorMode)
		{
			return HandleCursorModeWorldInteraction();
		}
		return HandleKeyModeWorldInteraction();
	}

	private bool HandleCursorModeWorldInteraction()
	{
		if (IsPointerOverUIForCurrentCursorMode())
		{
			return false;
		}
		HandleActionListDisplay();
		if (IsCursorModeLocked())
		{
			return false;
		}
		if (IsCursorLeftDown() && (HandleDragThrowInCurrentMode() || HandleInteract(isRightClick: false)))
		{
			SetCursorModeLocked(locked: true);
			return true;
		}
		if (IsCursorRightDown() && !ShouldReserveCancelForTownPortal() && HandleInteract(isRightClick: true))
		{
			SetCursorModeLocked(locked: true);
			return true;
		}
		return false;
	}

	private bool HandleKeyModeWorldInteraction()
	{
		HandleActionListDisplay();
		if (gamepadInteractionLocked)
		{
			return false;
		}
		bool num = GamepadUIActionManager.IsGameplaySubmitBlocked();
		bool flag = GamepadUIActionManager.IsGameplayCancelBlocked();
		if (num)
		{
			submitDown = false;
		}
		if (flag)
		{
			cancelDown = false;
		}
		if (submitDown && (HandleDragThrowInCurrentMode() || HandleInteract(isRightClick: false)))
		{
			gamepadInteractionLocked = true;
			return true;
		}
		if (cancelDown && !ShouldReserveCancelForTownPortal() && HandleInteract(isRightClick: true))
		{
			gamepadInteractionLocked = true;
			return true;
		}
		return false;
	}

	private bool ShouldReserveCancelForTownPortal()
	{
		if (!isPcCurrent && townPortalDown)
		{
			return !IsTownPortalShortcutBlockedByUi();
		}
		return false;
	}

	private void HandleInteractionLockRelease()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
			{
				mouseInteractionLocked = false;
			}
		}
		else if (GamepadInputManager.GetSubmitUp() || GamepadInputManager.GetCancelUp())
		{
			gamepadInteractionLocked = false;
		}
	}

	private bool IsInteractionLocked()
	{
		if (!SingletonMonoScope<InteractionManager>.HasInstance)
		{
			return false;
		}
		if (InteractionManager.IsCursorMode)
		{
			return IsCursorModeLocked();
		}
		return gamepadInteractionLocked;
	}

	private bool IsCursorModeLocked()
	{
		if (!SingletonMonoScope<InteractionManager>.HasInstance)
		{
			return false;
		}
		if (!InteractionManager.IsCursorMode)
		{
			return false;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return mouseInteractionLocked;
		}
		return gamepadInteractionLocked;
	}

	private void SetCursorModeLocked(bool locked)
	{
		if (SingletonMonoScope<InteractionManager>.HasInstance && InteractionManager.IsCursorMode)
		{
			if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
			{
				mouseInteractionLocked = locked;
			}
			else
			{
				gamepadInteractionLocked = locked;
			}
		}
	}

	public static bool IsPointerOverUIForCurrentCursorMode()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return false;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			if ((bool)EventSystem.current)
			{
				return EventSystem.current.IsPointerOverGameObject();
			}
			return false;
		}
		if (SingletonMonoGlobal<CursorUIManager>.HasInstance)
		{
			return SingletonMonoGlobal<CursorUIManager>.Instance.IsPointerOverUI();
		}
		return false;
	}

	private static bool IsCursorLeftDown()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return false;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return Input.GetMouseButtonDown(0);
		}
		return GamepadInputManager.GetSubmitDown();
	}

	private static bool IsCursorRightDown()
	{
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return false;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent())
		{
			return Input.GetMouseButtonDown(1);
		}
		return GamepadInputManager.GetCancelDown();
	}

	private static bool HandleDragThrowInCurrentMode()
	{
		if (!Hand.Instance.isDragItem)
		{
			return false;
		}
		switch (Hand.Instance.itemType)
		{
		case 0:
			SingletonMonoScope<ItemManager>.Instance.ThrowWP(Hand.Instance.weapon);
			Hand.Instance.DELItem();
			break;
		case 1:
			SingletonMonoScope<ItemManager>.Instance.ThrowBS(Hand.Instance.baoshi);
			Hand.Instance.DELItem();
			break;
		case 2:
			SingletonMonoScope<ItemManager>.Instance.ThrowUSE(Hand.Instance.useitem);
			Hand.Instance.DELItem();
			break;
		}
		return true;
	}

	public void ClearAllInteractionLocks()
	{
		mouseInteractionLocked = false;
		gamepadInteractionLocked = false;
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			InteractionManager.ClearPendingReleaseBlocks();
		}
	}

	private static bool HandleInteract(bool isRightClick)
	{
		if (!SingletonMonoScope<InteractionManager>.HasInstance)
		{
			return false;
		}
		IInteractable currentTarget = SingletonMonoScope<InteractionManager>.Instance.CurrentTarget;
		if (currentTarget == null)
		{
			return false;
		}
		if (!InteractionManager.CanInteractNow())
		{
			return false;
		}
		if (!currentTarget.CanInteract())
		{
			return false;
		}
		if (isRightClick)
		{
			currentTarget.OnRightClick();
		}
		else
		{
			currentTarget.OnLeftClick();
		}
		return true;
	}

	private void HandleSkillInput(ControlAction action, bool actionDown)
	{
		if ((isPcCurrent || !SingletonMonoScope<GameUIManager>.HasInstance || !SingletonMonoScope<GameUIManager>.Instance.IsAnyPanelOpened()) && !GamepadUIActionManager.IsGameplayActionBlocked(action) && !ShouldBlockActionBecausePointerConflict(action))
		{
			if (actionDown)
			{
				SingletonMonoScope<PlayerActionManager>.Instance.TryUseSkillDown(action);
			}
			else if (InputBind.Get(action))
			{
				SingletonMonoScope<PlayerActionManager>.Instance.TryUseSkillHold(action);
			}
		}
	}

	private void HandleUseInput(ControlAction action, bool actionDown)
	{
		if ((!SingletonMonoScope<ShopManager>.HasInstance || !SingletonMonoScope<ShopManager>.Instance.Opened) && !GamepadUIActionManager.IsGameplayActionBlocked(action) && !ShouldBlockActionBecausePointerConflict(action) && actionDown)
		{
			switch (action)
			{
			case ControlAction.Item1:
				SingletonMonoScope<PlayerActionManager>.Instance.TryUseItem(0);
				break;
			case ControlAction.Item2:
				SingletonMonoScope<PlayerActionManager>.Instance.TryUseItem(1);
				break;
			}
		}
	}

	private void HandleTownPortalInput(bool actionDown)
	{
		if (!IsTownPortalShortcutBlockedByUi() && !GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.TP) && !ShouldBlockActionBecausePointerConflict(ControlAction.TP) && actionDown)
		{
			ACT_TP.OpenFromShortcut();
		}
	}

	private static bool IsTownPortalShortcutBlockedByUi()
	{
		if (!SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return false;
		}
		if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return false;
		}
		return SingletonMonoScope<GameUIManager>.Instance.IsAnyPanelOpened();
	}

	private bool ShouldBlockActionBecausePointerConflict(ControlAction action)
	{
		EnsureActionBindingCache();
		if (!_actionBindingCache.TryGetValue(action, out var value) || !value.UsesUiOccupiedKey)
		{
			return false;
		}
		if (isPcCurrent)
		{
			if (pointerConflictUiBlockedThisFrame)
			{
				return true;
			}
			if (pointerConflictInteractableBlockedThisFrame)
			{
				return true;
			}
		}
		else if (pointerConflictUiBlockedThisFrame)
		{
			return true;
		}
		return false;
	}

	public void HandleActionListDisplay()
	{
		if (!_playerManager.IsAlive || pointerOverUI)
		{
			return;
		}
		int num;
		if (!isPcCurrent)
		{
			if (submitDown)
			{
				num = 1;
				goto IL_004c;
			}
			num = (cancelDown ? 1 : 0);
		}
		else
		{
			if (mouseLeftDown)
			{
				num = 1;
				goto IL_004c;
			}
			num = (mouseRightDown ? 1 : 0);
		}
		if (num != 0 || anySkillDownThisFrame)
		{
			goto IL_004c;
		}
		goto IL_0057;
		IL_0057:
		if (num != 0 || anyItemDownThisFrame)
		{
			actBar.CloseUseListUI();
		}
		return;
		IL_004c:
		actBar.CloseSkillListUI();
		goto IL_0057;
	}

	public void MarkActionBindingCacheDirty()
	{
		_bindingCacheBuilt = false;
	}

	private void RebuildActionBindingCache()
	{
		_actionBindingCache.Clear();
		_bindingCacheBuilt = false;
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			InputDeviceType currentDeviceType = SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType;
			ControlsSettingData control = Singleton<SettingDataManager>.Instance.GetControl(currentDeviceType);
			if (control != null)
			{
				_bindingCacheDeviceType = currentDeviceType;
				CacheActionBinding(control, ControlAction.Skill1);
				CacheActionBinding(control, ControlAction.Skill2);
				CacheActionBinding(control, ControlAction.Skill3);
				CacheActionBinding(control, ControlAction.Skill4);
				CacheActionBinding(control, ControlAction.Skill5);
				CacheActionBinding(control, ControlAction.Skill6);
				CacheActionBinding(control, ControlAction.Skill7);
				CacheActionBinding(control, ControlAction.Skill8);
				CacheActionBinding(control, ControlAction.Item1);
				CacheActionBinding(control, ControlAction.Item2);
				CacheActionBinding(control, ControlAction.QuickUse);
				CacheActionBinding(control, ControlAction.Mercenary);
				CacheActionBinding(control, ControlAction.Talent);
				CacheActionBinding(control, ControlAction.Stats);
				CacheActionBinding(control, ControlAction.Bag);
				CacheActionBinding(control, ControlAction.TP);
				CacheActionBinding(control, ControlAction.Sell);
				CacheActionBinding(control, ControlAction.SellAll);
				CacheActionBinding(control, ControlAction.PageL);
				CacheActionBinding(control, ControlAction.PageR);
				CacheActionBinding(control, ControlAction.SortAll);
				CacheActionBinding(control, ControlAction.Sort);
				CacheActionBinding(control, ControlAction.AutoAT);
				_bindingCacheBuilt = true;
			}
		}
	}

	private void CacheActionBinding(ControlsSettingData controls, ControlAction action)
	{
		string bindKey = controls.GetBindKey(action);
		if (string.IsNullOrWhiteSpace(bindKey))
		{
			_actionBindingCache[action] = default(ActionBindingCache);
			return;
		}
		bindKey = KeyNameUtil.NormalizeKeyName(bindKey);
		bool usesUiOccupiedKey = string.Equals(bindKey, "Mouse0", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "Mouse1", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "LeftControl", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "RightControl", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "LeftShift", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "RightShift", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "Pad_LStickPress", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "Pad_RStickPress", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "Pad_DPadUp", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "Pad_DPadLeft", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "Pad_DPadRight", StringComparison.OrdinalIgnoreCase) || string.Equals(bindKey, "Pad_DPadDown", StringComparison.OrdinalIgnoreCase);
		_actionBindingCache[action] = new ActionBindingCache
		{
			UsesUiOccupiedKey = usesUiOccupiedKey
		};
	}

	private void EnsureActionBindingCache()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && (!_bindingCacheBuilt || _bindingCacheDeviceType != SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType))
		{
			RebuildActionBindingCache();
		}
	}

	public void ForceClearInteractionLocks()
	{
		mouseInteractionLocked = false;
		gamepadInteractionLocked = false;
		if (SingletonMonoScope<InteractionManager>.HasInstance)
		{
			InteractionManager.ClearPendingReleaseBlocks();
		}
	}

	public void PrepareGameplayInputUnlock(bool suppressMovement = true)
	{
		ForceClearInteractionLocks();
		InputBind.SuppressHeldUntilRelease(suppressMovement ? GameplayActionsToSuppressOnUnlock : GameplayButtonActionsToSuppressOnUnlock);
		if (suppressMovement)
		{
			GamepadInputManager.SuppressLeftStickUntilNeutral();
		}
	}
}
