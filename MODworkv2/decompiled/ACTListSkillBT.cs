using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ACTListSkillBT : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image icon;

	public string IndexName;

	public int Xi;

	public int SkillType;

	public UnityEvent leftClick;

	public UnityEvent rightClick;

	public bool IsCD;

	public float CDTime;

	public float JStimeA;

	public bool EmptyBT;

	public ACT_skillData DT;

	public List<Companion> cpList;

	private ACTbar actBar;

	private GameUIManager _gameUIManager;

	private AudioManager _audioManager;

	public float Fill
	{
		get
		{
			if (CDTime >= 0f)
			{
				return (CDTime - JStimeA) / CDTime;
			}
			return 0f;
		}
	}

	private void Awake()
	{
		icon = GetComponent<Image>();
		IsCD = false;
		JStimeA = 0f;
		actBar = SingletonMonoScope<ACTbar>.Instance;
		_gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
	}

	private void Start()
	{
		leftClick.AddListener(ButtonLeftClick);
		rightClick.AddListener(ButtonRightClick);
	}

	private void OnEnable()
	{
		IsCD = false;
		JStimeA = 0f;
	}

	private void Update()
	{
		if (IsCD)
		{
			JStimeA += Time.deltaTime;
			if (JStimeA >= CDTime)
			{
				IsCD = false;
				JStimeA = 0f;
			}
		}
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
		case PointerEventData.InputButton.Middle:
			break;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_gameUIManager.ShowACTListSkillTip(Xi, SkillType, IndexName, base.transform, 1);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_gameUIManager.HideSkillTip();
	}

	private void ButtonLeftClick()
	{
		actBar.SetSkill(Xi, SkillType, this, icon.sprite);
		RuntimeManager.PlayOneShot(_audioManager.audioData.Add_Point_3);
	}

	private static void ButtonRightClick()
	{
	}

	public void ClearCpList()
	{
		if (cpList == null)
		{
			cpList = new List<Companion>();
		}
		else
		{
			if (cpList.Count <= 0)
			{
				return;
			}
			foreach (Companion item in cpList.ToList())
			{
				if ((bool)item)
				{
					item.SystemDelete();
				}
			}
			cpList.Clear();
		}
	}

	public bool DismissLowestHealthCompanion()
	{
		if (cpList == null || cpList.Count <= 0)
		{
			return false;
		}
		Companion companion = null;
		float num = float.MaxValue;
		for (int num2 = cpList.Count - 1; num2 >= 0; num2--)
		{
			Companion companion2 = cpList[num2];
			if (!companion2 || companion2.IsDead)
			{
				cpList.RemoveAt(num2);
			}
			else
			{
				float num3 = ((companion2.HealthStat != null) ? companion2.HealthStat.CurrentValue : float.MaxValue);
				if (!companion || num3 < num)
				{
					companion = companion2;
					num = num3;
				}
			}
		}
		if (!companion)
		{
			return false;
		}
		companion.SetDie(CompanionDeathMode.Dismiss);
		return true;
	}

	public void RefreshData()
	{
		foreach (Companion cp in cpList)
		{
			cp.RefreshData(DT);
		}
	}

	public void ResetCD()
	{
		IsCD = false;
		JStimeA = 0f;
	}
}
