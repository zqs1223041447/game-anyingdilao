using System.Collections;
using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTipItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public TooltipItem tooltipItem;

	public string content;

	private bool alwaysShow;

	private void Awake()
	{
		alwaysShow = false;
	}

	private void Start()
	{
		if (!tooltipItem)
		{
			tooltipItem = Object.FindObjectOfType<TooltipItem>();
		}
		tooltipItem?.ShowAll();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private IEnumerator DelayRefreshDisplayState()
	{
		yield return null;
		if (!tooltipItem)
		{
			tooltipItem = Object.FindObjectOfType<TooltipItem>();
		}
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			HandleDisplayToolTip(SingletonMonoGlobal<CurrentInputManager>.Instance.CurrentDeviceType);
		}
	}

	private void HandleDisplayToolTip(InputDeviceType deviceType)
	{
		if ((bool)tooltipItem)
		{
			tooltipItem.ShowAll();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_ = (bool)tooltipItem;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_ = (bool)tooltipItem;
	}

	private void ShowByContent()
	{
		switch (content)
		{
		case "health":
		case "Health":
			tooltipItem.ShowHealth();
			break;
		case "mana":
		case "Mana":
			tooltipItem.ShowMana();
			break;
		}
	}
}
