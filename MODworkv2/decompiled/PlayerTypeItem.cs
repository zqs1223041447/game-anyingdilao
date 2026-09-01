using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTypeItem : MonoBehaviour
{
	public int typeId;

	public GameObject selected;

	public GameObject unselected;

	public Action<PlayerTypeItem> OnClick;

	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	private void OnEnable()
	{
		if ((bool)button)
		{
			button.onClick.AddListener(HandleClick);
		}
	}

	private void OnDisable()
	{
		if ((bool)button)
		{
			button.onClick.RemoveListener(HandleClick);
		}
	}

	private void HandleClick()
	{
		OnClick?.Invoke(this);
	}

	public void SetSelected(bool value)
	{
		if ((bool)selected)
		{
			selected.SetActive(value);
		}
		if ((bool)unselected)
		{
			unselected.SetActive(!value);
		}
	}
}
