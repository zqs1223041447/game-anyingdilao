using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SKillBT_Lie : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Text text;

	public int Type;

	public float Number;

	private void Awake()
	{
		Button component = GetComponent<Button>();
		if ((bool)component)
		{
			component.enabled = false;
		}
	}

	private void OnEnable()
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.RegisterDFLieBT(this);
		}
	}

	private void OnDisable()
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.UnregisterDFLieBT(this);
		}
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideDFTip();
		}
	}

	private void Start()
	{
		EnsureText();
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.RegisterDFLieBT(this);
		}
	}

	public void Refresh(float number)
	{
		Number = number;
		EnsureText();
		if ((bool)text)
		{
			text.text = Mathf.FloorToInt(Number).ToString();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.ShowDFLieTip(this, base.transform);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideDFTip();
		}
	}

	private void EnsureText()
	{
		if (!text)
		{
			if ((bool)base.transform.parent)
			{
				text = base.transform.parent.Find("Text")?.GetComponent<Text>();
			}
			if (!text)
			{
				text = GetComponentInChildren<Text>(includeInactive: true);
			}
		}
	}
}
