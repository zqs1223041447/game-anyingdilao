using System;
using Data.SaveData;
using FinkFramework.Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SaveSystem;

public class SaveSlotItem : MonoBehaviour
{
	public Text nameText;

	public Text playerTypeText;

	public Text levelText;

	public Text versionText;

	public Text playedSecondsText;

	public Image playerIcon;

	public Image SelectIcon;

	public Sprite type0;

	public Sprite type1;

	public Sprite type2;

	public Sprite type3;

	public Sprite selected_type0;

	public Sprite selected_type1;

	public Sprite selected_type2;

	public Sprite selected_type3;

	private int currentPlayerType;

	public Action<SaveSlotItem> OnClick;

	private Button button;

	public int SlotId { get; private set; }

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	private void OnEnable()
	{
		if (button != null)
		{
			button.onClick.AddListener(HandleClick);
		}
	}

	private void OnDisable()
	{
		if (button != null)
		{
			button.onClick.RemoveListener(HandleClick);
		}
	}

	public void SetData(SaveSlotData data)
	{
		SlotId = data.SlotId;
		currentPlayerType = data.PlayerType;
		nameText.text = data.playerName;
		levelText.text = ((data.level == 100) ? $"{data.level} + {Mathf.Max(1, data.dfLevel)}" : data.level.ToString());
		versionText.text = data.GameVersion;
		playedSecondsText.text = TextsUtil.SecondToHMS2((int)data.PlayTimeSeconds);
		switch (currentPlayerType)
		{
		case 0:
			playerIcon.sprite = type0;
			playerTypeText.text = LOC.MM.GetStart("player_type0");
			break;
		case 1:
			playerIcon.sprite = type1;
			playerTypeText.text = LOC.MM.GetStart("player_type1");
			break;
		case 2:
			playerIcon.sprite = type2;
			playerTypeText.text = LOC.MM.GetStart("player_type2");
			break;
		case 3:
			playerIcon.sprite = type3;
			playerTypeText.text = LOC.MM.GetStart("player_type3");
			break;
		}
	}

	public void SetSelected(bool selected)
	{
		SelectIcon.gameObject.SetActive(selected);
		switch (currentPlayerType)
		{
		case 0:
			playerIcon.sprite = (selected ? selected_type0 : type0);
			break;
		case 1:
			playerIcon.sprite = (selected ? selected_type1 : type1);
			break;
		case 2:
			playerIcon.sprite = (selected ? selected_type2 : type2);
			break;
		case 3:
			playerIcon.sprite = (selected ? selected_type3 : type3);
			break;
		}
	}

	private void HandleClick()
	{
		OnClick?.Invoke(this);
	}
}
