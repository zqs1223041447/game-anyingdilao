using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tips : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private const float TipDelay = 0.2f;

	private float JStime;

	public bool YanShi;

	public string str;

	public string secondaryStr;

	private void Awake()
	{
		YanShi = false;
	}

	private void OnEnable()
	{
		YanShi = false;
	}

	private void Update()
	{
		if (!YanShi)
		{
			return;
		}
		JStime += Time.deltaTime;
		if (JStime >= 0.2f)
		{
			ControlAction action;
			if (IsPcAutoSellTip())
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowTip(base.transform, str, GetSecondaryTipKey());
			}
			else if (IsGamepadAutoSellTip())
			{
				ShowGamepadAutoSellTip();
			}
			else if (TryGetShortcutAction(out action))
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowTipWithShortcut(base.transform, str, action, GetSecondaryTipKey());
			}
			else
			{
				SingletonMonoScope<GameUIManager>.Instance.ShowTip(base.transform, str, GetSecondaryTipKey());
			}
			YanShi = false;
			JStime = 0f;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		YanShi = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		YanShi = false;
		JStime = 0f;
		SingletonMonoScope<GameUIManager>.Instance.HideEmptyTip();
	}

	private string GetSecondaryTipKey()
	{
		if (!string.IsNullOrEmpty(secondaryStr))
		{
			return secondaryStr;
		}
		if (!string.IsNullOrEmpty(str) && str.StartsWith("AutoSell"))
		{
			if (!SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				return "AutoSellOnlyOtherClassesPC";
			}
			return "AutoSellOnlyOtherClassesGamepad";
		}
		return string.Empty;
	}

	private bool IsPcAutoSellTip()
	{
		if (!string.IsNullOrEmpty(str) && str.StartsWith("AutoSell"))
		{
			if (SingletonMonoGlobal<CurrentInputManager>.HasInstance)
			{
				return SingletonMonoGlobal<CurrentInputManager>.Instance.IsPcCurrent();
			}
			return true;
		}
		return false;
	}

	private bool IsGamepadAutoSellTip()
	{
		if (!string.IsNullOrEmpty(str) && str.StartsWith("AutoSell") && SingletonMonoGlobal<CurrentInputManager>.HasInstance)
		{
			return SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent();
		}
		return false;
	}

	private void ShowGamepadAutoSellTip()
	{
		string localizedAutoSellMainText = GetLocalizedAutoSellMainText();
		string gamepadAutoSellSecondaryText = GetGamepadAutoSellSecondaryText();
		SingletonMonoScope<GameUIManager>.Instance.ShowTipRawText(base.transform, localizedAutoSellMainText, gamepadAutoSellSecondaryText);
	}

	private string GetLocalizedAutoSellMainText()
	{
		string main = LOC.MM.GetMain(str);
		if (!string.IsNullOrEmpty(main))
		{
			return main;
		}
		return str;
	}

	private static string GetGamepadAutoSellSecondaryText()
	{
		string localKeyName = KeyDisplayUtil.GetLocalKeyName(ControlAction.SellAll);
		if (string.IsNullOrEmpty(localKeyName))
		{
			return LOC.MM.GetMain("Shortcut Key Not Set");
		}
		string text = KeyDisplayUtil.ToDisplayName("Pad_LStickPress");
		string main = LOC.MM.GetMain("AutoSellOnlyOtherClassesGamepad");
		main = StripGamepadShortcutPrefix(main);
		return localKeyName + " / " + text + "+" + localKeyName + "  " + main;
	}

	private static string StripGamepadShortcutPrefix(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		string[] array = new string[4] { "Pad_LS+Y", "Pad_LS + Y", "LS+Y", "LS + Y" };
		for (int i = 0; i < array.Length; i++)
		{
			if (text.StartsWith(array[i]))
			{
				return text.Substring(array[i].Length).Trim();
			}
		}
		if (text.StartsWith("Pad_LS+"))
		{
			int num = text.IndexOf("  ");
			if (num >= 0 && num + 2 < text.Length)
			{
				return text.Substring(num + 2).Trim();
			}
		}
		return text;
	}

	private bool TryGetShortcutAction(out ControlAction action)
	{
		action = ControlAction.Up;
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		if (str.StartsWith("AutoSell"))
		{
			action = ControlAction.SellAll;
			return true;
		}
		switch (str)
		{
		case "SortAll IV":
		case "SortAll CH":
			action = ControlAction.SortAll;
			return true;
		case "Sort IV":
		case "Sort CH":
			action = ControlAction.Sort;
			return true;
		default:
			return false;
		}
	}
}
