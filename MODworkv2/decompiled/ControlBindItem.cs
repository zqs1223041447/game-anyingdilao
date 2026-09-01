using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlBindItem : MonoBehaviour
{
	[Header("UI")]
	public Text actionText;

	public TextMeshProUGUI keyText;

	public Text keyText_old;

	public Button bindButton;

	public Image keyImage;

	public ControlAction Action { get; private set; }

	public void SetConflict(bool conflict)
	{
		keyImage.gameObject.SetActive(conflict);
	}

	public void Bind(ControlAction controlAction, string actionLabel, string KeyRaw, Action<ControlAction> onClick)
	{
		Action = controlAction;
		if ((bool)actionText)
		{
			actionText.text = actionLabel;
		}
		SetKey(KeyRaw);
		if ((bool)bindButton)
		{
			bindButton.onClick.RemoveAllListeners();
			bindButton.onClick.AddListener(delegate
			{
				onClick?.Invoke(controlAction);
			});
		}
	}

	public void SetKey(string keyRaw)
	{
		if (string.IsNullOrEmpty(keyRaw))
		{
			ShowOldText(LOC.MM.GetStart("no_bind"));
			SetConflict(conflict: false);
			return;
		}
		if (KeyDisplayUtil.TryGetSpriteRichText(keyRaw, out var richText))
		{
			ShowTmpSprite(richText);
		}
		else
		{
			string plainText = KeyDisplayUtil.ToDisplayName(keyRaw);
			ShowOldText(plainText);
		}
		SetConflict(conflict: false);
	}

	public void SetActionLabel(string label)
	{
		if ((bool)actionText)
		{
			actionText.text = label;
		}
	}

	public void SetWaiting(string waitingText)
	{
		ShowOldText(waitingText);
		SetConflict(conflict: false);
	}

	public void SetInteractable(bool interactable)
	{
		if ((bool)bindButton)
		{
			bindButton.interactable = interactable;
		}
	}

	private void ShowTmpSprite(string richText)
	{
		if ((bool)keyText)
		{
			keyText.gameObject.SetActive(value: true);
			keyText.text = richText;
		}
		if ((bool)keyText_old)
		{
			keyText_old.gameObject.SetActive(value: false);
			keyText_old.text = string.Empty;
		}
	}

	private void ShowOldText(string plainText)
	{
		if ((bool)keyText)
		{
			keyText.gameObject.SetActive(value: false);
			keyText.text = string.Empty;
		}
		if ((bool)keyText_old)
		{
			keyText_old.gameObject.SetActive(value: true);
			keyText_old.text = plainText;
		}
	}
}
