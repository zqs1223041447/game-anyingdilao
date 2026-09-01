using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using FinkFramework.Runtime.Utils;
using Inputs;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : GamepadSelectablePanel
{
	private GameObject btnGroup;

	private GameObject exitGroup;

	private GameObject aboutGroup;

	private GameObject updateGroup;

	public GameObject NT;

	public Text[] NoteText;

	public Text AboutText;

	private float timeA;

	protected override void Awake()
	{
		base.Awake();
		btnGroup = base.transform.Find("BtnGroup").gameObject;
		exitGroup = base.transform.Find("ExitGroup").gameObject;
		aboutGroup = base.transform.Find("AboutGroup").gameObject;
		updateGroup = base.transform.Find("UpdateGroup").gameObject;
		HideAllGroups();
		btnGroup.SetActive(value: true);
	}

	private void Start()
	{
		timeA = 0f;
		Refresh();
	}

	public void Refresh()
	{
		for (int i = 0; i < NoteText.Length; i++)
		{
			NoteText[i].text = " " + LOC.MM.GetNote(NoteText[i].name);
		}
		AboutText.text = " " + LOC.MM.GetNote("AboutGame");
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA >= 0.5f)
		{
			for (int i = 0; i < NoteText.Length; i++)
			{
				NoteText[i].text = " " + LOC.MM.GetNote(NoteText[i].name);
			}
			AboutText.text = " " + LOC.MM.GetNote("AboutGame");
			timeA = 0f;
		}
	}

	protected override void ClickBtn(string btnName)
	{
		if (btnName == null)
		{
			return;
		}
		switch (btnName.Length)
		{
		case 8:
			switch (btnName[0])
			{
			case 'S':
				if (btnName == "StartBtn")
				{
					SetReturnSelected(GetControl<Button>("StartBtn"));
					Singleton<UIManager>.Instance.ShowExclusivePanel<StartPanel>();
				}
				break;
			case 'A':
				if (btnName == "AboutBtn")
				{
					HideAllGroups();
					aboutGroup.SetActive(value: true);
					SetFirstSelected(GetControl<Button>("AboutBack"));
				}
				break;
			}
			break;
		case 11:
			switch (btnName[0])
			{
			case 'S':
				if (btnName == "SettingsBtn")
				{
					SetReturnSelected(GetControl<Button>("SettingsBtn"));
					Singleton<UIManager>.Instance.ShowExclusivePanel<SettingPanel>();
				}
				break;
			case 'E':
				if (btnName == "ExitConfirm")
				{
					LogUtil.Info("退出游戏");
					Application.Quit();
				}
				break;
			}
			break;
		case 9:
			switch (btnName[0])
			{
			case 'U':
				if (btnName == "UpdateBtn")
				{
					HideAllGroups();
					updateGroup.SetActive(value: true);
					SetFirstSelected(GetControl<Button>("UpdateBack"));
				}
				break;
			case 'A':
				if (btnName == "AboutBack")
				{
					HideAllGroups();
					btnGroup.SetActive(value: true);
					SetFirstSelected(GetControl<Button>("AboutBtn"));
				}
				break;
			}
			break;
		case 10:
			switch (btnName[0])
			{
			case 'U':
				if (btnName == "UpdateBack")
				{
					HideAllGroups();
					btnGroup.SetActive(value: true);
					SetFirstSelected(GetControl<Button>("UpdateBtn"));
				}
				break;
			case 'E':
				if (btnName == "ExitCancel")
				{
					HideAllGroups();
					btnGroup.SetActive(value: true);
					SetFirstSelected(GetControl<Button>("ExitBtn"));
				}
				break;
			}
			break;
		case 7:
			if (btnName == "ExitBtn")
			{
				HideAllGroups();
				exitGroup.SetActive(value: true);
				SetFirstSelected(GetControl<Button>("ExitCancel"));
			}
			break;
		}
	}

	public override void OnShow()
	{
		HideAllGroups();
		btnGroup.SetActive(value: true);
		SetFirstSelected(GetControl<Button>("StartBtn"));
	}

	private void HideAllGroups()
	{
		btnGroup.SetActive(value: false);
		exitGroup.SetActive(value: false);
		aboutGroup.SetActive(value: false);
		updateGroup.SetActive(value: false);
	}

	public override bool OnCancel()
	{
		if (aboutGroup.activeSelf)
		{
			HideAllGroups();
			btnGroup.SetActive(value: true);
			SetFirstSelected(GetControl<Button>("AboutBtn"));
			return true;
		}
		if (updateGroup.activeSelf)
		{
			HideAllGroups();
			btnGroup.SetActive(value: true);
			SetFirstSelected(GetControl<Button>("UpdateBtn"));
			return true;
		}
		if (exitGroup.activeSelf)
		{
			HideAllGroups();
			btnGroup.SetActive(value: true);
			SetFirstSelected(GetControl<Button>("ExitBtn"));
			return true;
		}
		return false;
	}
}
