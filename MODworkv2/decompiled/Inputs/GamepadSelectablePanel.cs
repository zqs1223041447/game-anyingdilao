using System.Collections;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inputs;

public class GamepadSelectablePanel : BasePanel
{
	protected bool useGamepadUI;

	protected Selectable firstSelected;

	protected Selectable returnSelected;

	public virtual bool OnCancel()
	{
		return false;
	}

	protected virtual void OnEnable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged += HandleInputDeviceChanged;
		RefreshByCurrentDevice();
	}

	protected virtual void OnDisable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged -= HandleInputDeviceChanged;
		ClearSelectionIfInsidePanel();
	}

	public Selectable GetFirstSelected()
	{
		return firstSelected;
	}

	public Selectable GetReturnSelected()
	{
		return returnSelected;
	}

	protected void SetFirstSelected(Selectable target, bool selectNow = true)
	{
		firstSelected = target;
		if (selectNow && useGamepadUI)
		{
			SelectCurrentTarget();
		}
	}

	public void SetReturnSelected(Selectable target)
	{
		returnSelected = target;
	}

	protected Selectable GetDefaultSelected()
	{
		if (!returnSelected)
		{
			return firstSelected;
		}
		return returnSelected;
	}

	protected void ConsumeReturnSelected()
	{
		returnSelected = null;
	}

	protected void RefreshByCurrentDevice()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			useGamepadUI = SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent();
			if (useGamepadUI)
			{
				SelectCurrentTarget();
			}
			else
			{
				ClearSelection();
			}
		}
	}

	protected void HandleInputDeviceChanged(InputDeviceType deviceType)
	{
		useGamepadUI = CurrentInputManager.IsGamepad(deviceType);
		if (useGamepadUI)
		{
			SelectCurrentTarget();
		}
		else
		{
			ClearSelection();
		}
	}

	protected void SelectCurrentTarget()
	{
		if (base.isActiveAndEnabled)
		{
			Selectable defaultSelected = GetDefaultSelected();
			if ((bool)EventSystem.current && (bool)defaultSelected)
			{
				StartCoroutine(SelectNextFrame(defaultSelected));
			}
		}
	}

	private IEnumerator SelectNextFrame(Selectable target)
	{
		yield return null;
		if (base.isActiveAndEnabled && (bool)EventSystem.current && (bool)target && target.gameObject.activeInHierarchy && GamepadUINavigationManager.IsSelectableValidForGamepad(target))
		{
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(target.gameObject);
			ConsumeReturnSelected();
		}
	}

	protected void SetSelectedNull()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void ClearSelectionIfInsidePanel()
	{
		if ((bool)EventSystem.current)
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if ((bool)currentSelectedGameObject && (currentSelectedGameObject.transform == base.transform || currentSelectedGameObject.transform.IsChildOf(base.transform)))
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
		}
	}

	protected static void ClearSelection()
	{
		if ((bool)EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
