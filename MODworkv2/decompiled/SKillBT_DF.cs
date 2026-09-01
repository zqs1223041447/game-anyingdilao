using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SKillBT_DF : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Text text;

	public Image Skill;

	[HideInInspector]
	public bool Unlock;

	public int Index;

	private int _level;

	private int _max;

	private TalentManager _talentManager;

	private GameUIManager _gameUIManager;

	private AudioManager _audioManager;

	private const int FillAddPointMax = 20;

	public bool Full
	{
		get
		{
			if (_max > 0)
			{
				return _level >= _max;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.RegisterDFSkillBT(this);
		}
	}

	private void OnDisable()
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.UnregisterDFSkillBT(this);
		}
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideDFTip();
			SingletonMonoScope<GameUIManager>.Instance.HideDFSkillList();
		}
	}

	private void Start()
	{
		EnsureRefs();
		if ((bool)_talentManager)
		{
			_talentManager.RegisterDFSkillBT(this);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		switch (eventData.button)
		{
		case PointerEventData.InputButton.Left:
			ButtonLeftClick();
			break;
		case PointerEventData.InputButton.Right:
			ButtonRightClick();
			break;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (EnsureRefs())
		{
			_gameUIManager.ShowDFTip(Index, base.transform);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance && !SingletonMonoScope<GameUIManager>.Instance.IsDFSkillListOpen)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideDFTip();
		}
	}

	public void Refresh(int level, int max, bool unlock, Sprite sprite)
	{
		_level = level;
		_max = max;
		Unlock = unlock;
		EnsureText();
		if ((bool)text)
		{
			text.text = level.ToString();
		}
		if (!Skill)
		{
			Skill = GetComponent<Image>();
		}
		if ((bool)Skill && (bool)sprite)
		{
			Skill.sprite = sprite;
		}
	}

	private void ButtonLeftClick()
	{
		if (!EnsureRefs())
		{
			return;
		}
		SkilDFData dFData = _talentManager.GetDFData(Index);
		if (dFData == null)
		{
			return;
		}
		if (_gameUIManager.IsDFSkillListOpen)
		{
			if (_gameUIManager.IsDFSkillListOpenFor(Index))
			{
				PlayDFSkillButtonSound();
				_gameUIManager.HideDFSkillList();
			}
			else if (dFData.HasMultipleChoices && !dFData.HasSelectedSkill)
			{
				PlayDFSkillButtonSound();
				_gameUIManager.ShowDFSkillList(this);
			}
			else
			{
				PlayDFSkillButtonSound();
				_gameUIManager.HideDFSkillList();
			}
		}
		else if (!dFData.HasSelectedSkill)
		{
			if (dFData.HasMultipleChoices)
			{
				_gameUIManager.ShowDFSkillList(this);
			}
		}
		else
		{
			AddPoint(GetShortcutAddPointLimit());
		}
	}

	private void ButtonRightClick()
	{
		if (!EnsureRefs() || !_talentManager.DFHasMultipleChoices(Index))
		{
			return;
		}
		if (_gameUIManager.IsDFSkillListOpenFor(Index))
		{
			PlayDFSkillButtonSound();
			_gameUIManager.HideDFSkillList();
			return;
		}
		if (_gameUIManager.IsDFSkillListOpen)
		{
			PlayDFSkillButtonSound();
		}
		_gameUIManager.ShowDFSkillList(this);
	}

	private void PlayDFSkillButtonSound()
	{
		if ((bool)_audioManager && (bool)_audioManager.audioData)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Add_Point_1);
		}
	}

	public void AddFillFromShortcut()
	{
		AddPoint(20);
	}

	public void AddFullFromShortcut()
	{
		AddPoint(int.MaxValue);
	}

	public void AddPoint(bool fill)
	{
		AddPoint((!fill) ? 1 : 20);
	}

	private void AddPoint(int maxAddCount)
	{
		if (!EnsureRefs())
		{
			return;
		}
		if (!Unlock || Full || _talentManager.P_Have <= 0)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Skill_NoPoint3);
			return;
		}
		RuntimeManager.PlayOneShot(_audioManager.audioData.Add_Point_2);
		int num = Mathf.Min(_talentManager.P_Have, _max - _level, Mathf.Max(1, maxAddCount));
		for (int i = 0; i < num; i++)
		{
			_talentManager.AddPointDF(Index);
		}
		if (Full)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.LitButton_6);
		}
	}

	private static bool IsFillPointInput()
	{
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			return true;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return GamepadInputManager.GetKey("Pad_X");
		}
		return false;
	}

	private static bool IsFullPointInput()
	{
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			return true;
		}
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			return GamepadInputManager.GetKey("Pad_Y");
		}
		return false;
	}

	private static int GetShortcutAddPointLimit()
	{
		if (IsFullPointInput())
		{
			return int.MaxValue;
		}
		if (!IsFillPointInput())
		{
			return 1;
		}
		return 20;
	}

	private bool EnsureRefs()
	{
		if (!_talentManager && SingletonMonoScope<TalentManager>.HasInstance)
		{
			_talentManager = SingletonMonoScope<TalentManager>.Instance;
		}
		if (!_gameUIManager && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			_gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		}
		if (!_audioManager && SingletonMonoGlobal<AudioManager>.HasInstance)
		{
			_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
		}
		if ((bool)_talentManager && (bool)_gameUIManager)
		{
			return _audioManager;
		}
		return false;
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
