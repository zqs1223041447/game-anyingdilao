using System;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ACTListUseBT : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image icon;

	public string IndexName;

	public string useType;

	public int stackSize;

	public Text stackText;

	public UnityEvent leftClick;

	public UnityEvent rightClick;

	private ACTbar actbar;

	private GameUIManager _gameUIManager;

	private AudioManager _audioManager;

	private void Awake()
	{
		icon = GetComponent<Image>();
		stackText = base.transform.parent.transform.Find("stackSize").GetComponent<Text>();
		actbar = SingletonMonoScope<ACTbar>.Instance;
		_gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
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
		default:
			throw new ArgumentOutOfRangeException();
		case PointerEventData.InputButton.Middle:
			break;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SlotData slotData = SingletonMonoScope<InventoryManager>.Instance.ReturnSameUse(IndexName);
		if (slotData != null)
		{
			_gameUIManager.ShowACTUseTip(base.transform.position, slotData.useitem);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_gameUIManager.HideUseTip();
	}

	private void ButtonLeftClick()
	{
		actbar.SetUse(stackSize, useType, IndexName, icon.sprite);
		RuntimeManager.PlayOneShot(_audioManager.audioData.Quick_SK_Select);
	}

	private void ButtonRightClick()
	{
	}

	public void DEL()
	{
		actbar.actListUse.Remove(this);
		LeanPool.Despawn(base.transform.parent);
	}

	public void RefreshStack(int a)
	{
		stackSize = a;
		if (stackSize > 999)
		{
			stackText.text = "999+";
		}
		else
		{
			stackText.text = stackSize.ToString();
		}
	}
}
