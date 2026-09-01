using Entity.Comp.CompanionAI;
using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UIItems;

public class CompItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private Text countText;

	private ACTListSkillBT skill;

	public void InitComp(Sprite sprite, int count, ACTListSkillBT skillData = null)
	{
		BindSkill(skillData);
		if ((bool)icon)
		{
			icon.sprite = sprite;
		}
		RefreshCount(count);
	}

	public void BindSkill(ACTListSkillBT skillData)
	{
		skill = skillData;
	}

	public void RefreshCount(int count)
	{
		if ((bool)countText)
		{
			countText.text = count.ToString();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ShowSkillTip();
	}

	public void ShowSkillTip()
	{
		if ((bool)skill && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.ShowCompItemSkillTip(skill, base.transform);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideSkillTip();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != 0 || !IsCtrlModifier() || !skill)
		{
			return;
		}
		CursorUIManager.ConsumeCtrlModifier();
		if (skill.DismissLowestHealthCompanion())
		{
			int num = skill.cpList?.Count ?? 0;
			RefreshCount(num);
			if (num <= 0 && SingletonMonoScope<GameUIManager>.HasInstance)
			{
				SingletonMonoScope<GameUIManager>.Instance.HideSkillTip();
			}
			if (SingletonMonoScope<CompanionManager>.HasInstance)
			{
				SingletonMonoScope<CompanionManager>.Instance.RefreshAfterCompItemDismiss();
			}
		}
	}

	private static bool IsCtrlModifier()
	{
		if (!Input.GetKey(KeyCode.LeftControl))
		{
			return Input.GetKey(KeyCode.RightControl);
		}
		return true;
	}
}
