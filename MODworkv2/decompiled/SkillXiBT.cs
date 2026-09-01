using FinkFramework.Runtime.Singleton;
using UI.CustomHandler;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillXiBT : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Sprite selected;

	public Sprite unselected;

	public int Index;

	public UnityEvent leftClick;

	private UIButtonState buttonState;

	private bool IsDFTalentButton
	{
		get
		{
			if (SingletonMonoScope<TalentManager>.HasInstance)
			{
				return SingletonMonoScope<TalentManager>.Instance.DFXiBT == this;
			}
			return false;
		}
	}

	private void Awake()
	{
		buttonState = GetComponent<UIButtonState>();
		GetComponent<Image>();
	}

	private void Start()
	{
		leftClick.AddListener(ButtonLeftClick);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			leftClick.Invoke();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (IsDFTalentButton)
		{
			SingletonMonoScope<GameUIManager>.Instance.ShowDFTalentTreeTip(base.transform);
		}
		else
		{
			SingletonMonoScope<GameUIManager>.Instance.ShowXiTip(Index, base.transform);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SingletonMonoScope<GameUIManager>.Instance.HideXiTip();
	}

	public void SetOpen(int a)
	{
		switch (a)
		{
		case 0:
			buttonState.SetBaseSprite(unselected);
			break;
		case 1:
			buttonState.SetBaseSprite(selected);
			break;
		}
	}

	private void ButtonLeftClick()
	{
		if (IsDFTalentButton)
		{
			SingletonMonoScope<TalentManager>.Instance.ChangeDFPage();
		}
		else
		{
			SingletonMonoScope<TalentManager>.Instance.ChangePage(Index);
		}
	}
}
