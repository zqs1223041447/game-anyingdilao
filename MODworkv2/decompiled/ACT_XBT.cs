using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ACT_XBT : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public UnityEvent leftClick;

	public UnityEvent rightClick;

	public int type;

	private ACTbar actbar;

	private Sprite spr;

	private void Awake()
	{
		actbar = SingletonMonoScope<ACTbar>.Instance;
	}

	private void Start()
	{
		leftClick.AddListener(ButtonLeftClick);
		rightClick.AddListener(ButtonRightClick);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		switch (eventData.button)
		{
		case PointerEventData.InputButton.Left:
			leftClick.Invoke();
			break;
		case PointerEventData.InputButton.Right:
			rightClick.Invoke();
			break;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}

	private void ButtonLeftClick()
	{
		switch (type)
		{
		case 0:
			actbar.ClearSK_Single();
			break;
		case 1:
			actbar.ClearUse_Single();
			break;
		}
	}

	private static void ButtonRightClick()
	{
	}
}
