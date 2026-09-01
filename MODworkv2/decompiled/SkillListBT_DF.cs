using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillListBT_DF : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, ISelectHandler
{
	public Image icon;

	public Text text;

	private GameObject _displayRoot;

	private SKillBT_DF _target;

	private int _skillIndex;

	private GameObject DisplayRoot
	{
		get
		{
			if (!_displayRoot)
			{
				return base.gameObject;
			}
			return _displayRoot;
		}
	}

	public void SetDisplayRoot(GameObject displayRoot)
	{
		_displayRoot = displayRoot;
	}

	public void Setup(SKillBT_DF target, int skillIndex, Sprite sprite, string title)
	{
		_target = target;
		_skillIndex = skillIndex;
		EnsureRefs();
		if ((bool)icon)
		{
			icon.sprite = sprite;
			icon.enabled = sprite != null;
			icon.raycastTarget = true;
		}
		if ((bool)text)
		{
			text.text = title;
			text.raycastTarget = false;
		}
		DisplayRoot.SetActive(value: true);
	}

	public void Hide()
	{
		_target = null;
		DisplayRoot.SetActive(value: false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			SelectSkill();
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		SelectSkill();
	}

	public void OnSelect(BaseEventData eventData)
	{
		ShowTip();
	}

	private void SelectSkill()
	{
		if ((bool)_target && SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.SelectDFSkill(_target.Index, _skillIndex);
			if (SingletonMonoGlobal<AudioManager>.HasInstance && (bool)SingletonMonoGlobal<AudioManager>.Instance.audioData)
			{
				RuntimeManager.PlayOneShot(SingletonMonoGlobal<AudioManager>.Instance.audioData.Add_Point_3);
			}
			if (SingletonMonoScope<GameUIManager>.HasInstance)
			{
				SingletonMonoScope<GameUIManager>.Instance.HideDFSkillList();
			}
			FocusTargetForGamepad();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ShowTip();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideDFTip();
		}
	}

	private void ShowTip()
	{
		if ((bool)_target && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.ShowDFSkillListTip(_target, _skillIndex, base.transform);
		}
	}

	private void FocusTargetForGamepad()
	{
		if (!_target || !SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return;
		}
		Selectable component = _target.GetComponent<Selectable>();
		if (GamepadUINavigationManager.IsSelectableValidForGamepad(component))
		{
			if ((bool)EventSystem.current)
			{
				EventSystem.current.SetSelectedGameObject(null);
				EventSystem.current.SetSelectedGameObject(component.gameObject);
			}
			GamepadUINavigationManager.RequestForceFocus(component);
		}
	}

	private void EnsureRefs()
	{
		if (!icon)
		{
			icon = FindIconImage();
		}
		if (!text)
		{
			text = GetComponentInChildren<Text>(includeInactive: true);
		}
	}

	private Image FindIconImage()
	{
		Image component = GetComponent<Image>();
		if ((bool)component && component.raycastTarget)
		{
			return component;
		}
		Image[] componentsInChildren = GetComponentsInChildren<Image>(includeInactive: true);
		Image[] array = componentsInChildren;
		foreach (Image image in array)
		{
			if ((bool)image && image != component && image.raycastTarget)
			{
				return image;
			}
		}
		if ((bool)component)
		{
			return component;
		}
		if (componentsInChildren.Length == 0)
		{
			return null;
		}
		return componentsInChildren[0];
	}
}
