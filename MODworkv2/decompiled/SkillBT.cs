using System;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillBT : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Text text;

	public Image SkillTU;

	[HideInInspector]
	public bool Unlock;

	public string IndexName;

	public int Xi;

	public int SkillType;

	[HideInInspector]
	public bool BS_Skill;

	[HideInInspector]
	public bool LastSkill;

	[HideInInspector]
	public bool DashSkill;

	[HideInInspector]
	public bool TPSkill;

	private int Level_Base;

	private int Level_Max;

	public UnityEvent leftClick;

	public UnityEvent rightClick;

	private TalentManager _talentManager;

	private ACTbar actbar;

	private GameUIManager _gameUIManager;

	private AudioManager _audioManager;

	public bool Full => Level_Base == Level_Max;

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
		_gameUIManager.ShowSkilltip(Xi, SkillType, IndexName, base.transform);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_gameUIManager.HideSkillTip();
	}

	private void OnEnable()
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.RegisterSkillBT(this);
		}
	}

	private void OnDisable()
	{
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.UnregisterSkillBT(this);
		}
	}

	private void Start()
	{
		_talentManager = SingletonMonoScope<TalentManager>.Instance;
		actbar = SingletonMonoScope<ACTbar>.Instance;
		_gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
		_talentManager.SetSkillBT(this, Xi, SkillType, IndexName);
		leftClick.AddListener(ButtonLeftClick);
		rightClick.AddListener(ButtonRightClick);
		if (SingletonMonoScope<TalentManager>.HasInstance)
		{
			SingletonMonoScope<TalentManager>.Instance.RegisterSkillBT(this);
		}
	}

	private void EnsureText()
	{
		if (!text)
		{
			text = ((!base.transform.parent) ? null : base.transform.parent.Find("Text")?.GetComponent<Text>());
			if (!text)
			{
				LogUtil.Error("[SkillBT] 未找到 Text 子节点: " + base.name);
			}
		}
	}

	public void Refresh(int level, int max, int weaponOn)
	{
		if (!this)
		{
			return;
		}
		Level_Base = level;
		Level_Max = max;
		EnsureText();
		if (!text)
		{
			return;
		}
		if (weaponOn > 0)
		{
			if (level > 0)
			{
				text.text = $"<color=#FFE43B>{level + weaponOn}/{max + weaponOn}</color>";
			}
			else
			{
				text.text = $"<color=#FFE43B>{level}/{max + weaponOn}</color>";
			}
		}
		else
		{
			text.text = $"<color=#ffffffff>{level}/{max}</color>";
		}
	}

	private void RememberBSSkillIfNeeded()
	{
		if (BS_Skill)
		{
			_talentManager.BSSkillButton = this;
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

	private static bool IsAddCurrentAndChildrenInput()
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

	public void AddFillFromShortcut()
	{
		ButtonShortcutAdd(includeChildren: false);
	}

	public void AddCurrentAndChildrenFromShortcut()
	{
		ButtonShortcutAdd(includeChildren: true);
	}

	private void ButtonShortcutAdd(bool includeChildren)
	{
		actbar.CloseSkillListUI();
		RuntimeManager.PlayOneShot(_talentManager.TryAddNormalSkillFromShortcut(Xi, SkillType, IndexName, includeChildren) ? _audioManager.audioData.Add_Point_2 : _audioManager.audioData.Skill_NoPoint3);
		if (Full)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.LitButton_6);
		}
		actbar.RefreshCD();
	}

	private void ButtonLeftClick()
	{
		actbar.CloseSkillListUI();
		if (IsAddCurrentAndChildrenInput())
		{
			ButtonShortcutAdd(includeChildren: true);
			return;
		}
		if (IsFillPointInput())
		{
			ButtonShortcutAdd(includeChildren: false);
			return;
		}
		if (IsFillPointInput())
		{
			if (_talentManager.P_Have > 0 && Unlock && !Full)
			{
				RuntimeManager.PlayOneShot(_audioManager.audioData.Add_Point_2);
				switch (SkillType)
				{
				case 0:
					if (_talentManager.HasBSSkill)
					{
						if (!BS_Skill)
						{
							if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
							{
								int num10 = Level_Max - Level_Base;
								for (int num11 = 0; num11 < num10; num11++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
							else
							{
								int p_Have6 = _talentManager.P_Have;
								for (int num12 = 0; num12 < p_Have6; num12++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
						}
						else
						{
							if (!BS_Skill || !(_talentManager.BSSkillButton == this))
							{
								break;
							}
							if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
							{
								int num13 = Level_Max - Level_Base;
								for (int num14 = 0; num14 < num13; num14++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
							else
							{
								int p_Have7 = _talentManager.P_Have;
								for (int num15 = 0; num15 < p_Have7; num15++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
						}
						break;
					}
					if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
					{
						int num16 = Level_Max - Level_Base;
						for (int num17 = 0; num17 < num16; num17++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					else
					{
						int p_Have8 = _talentManager.P_Have;
						for (int num18 = 0; num18 < p_Have8; num18++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					RememberBSSkillIfNeeded();
					break;
				case 1:
					if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
					{
						int num2 = Level_Max - Level_Base;
						for (int k = 0; k < num2; k++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					else
					{
						int p_Have2 = _talentManager.P_Have;
						for (int l = 0; l < p_Have2; l++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					break;
				case 2:
					if (_talentManager.HasBSSkill)
					{
						if (!BS_Skill)
						{
							if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
							{
								int num3 = Level_Max - Level_Base;
								for (int m = 0; m < num3; m++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
							else
							{
								int p_Have3 = _talentManager.P_Have;
								for (int n = 0; n < p_Have3; n++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
						}
						else
						{
							if (!(_talentManager.BSSkillButton == this))
							{
								break;
							}
							if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
							{
								int num4 = Level_Max - Level_Base;
								for (int num5 = 0; num5 < num4; num5++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
							else
							{
								int p_Have4 = _talentManager.P_Have;
								for (int num6 = 0; num6 < p_Have4; num6++)
								{
									_talentManager.AddPoint(Xi, SkillType, IndexName);
								}
							}
						}
						break;
					}
					if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
					{
						int num7 = Level_Max - Level_Base;
						for (int num8 = 0; num8 < num7; num8++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					else
					{
						int p_Have5 = _talentManager.P_Have;
						for (int num9 = 0; num9 < p_Have5; num9++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					RememberBSSkillIfNeeded();
					break;
				case 3:
					if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
					{
						int num25 = Level_Max - Level_Base;
						for (int num26 = 0; num26 < num25; num26++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					else
					{
						int p_Have11 = _talentManager.P_Have;
						for (int num27 = 0; num27 < p_Have11; num27++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					break;
				case 4:
					if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
					{
						int num19 = Level_Max - Level_Base;
						for (int num20 = 0; num20 < num19; num20++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					else
					{
						int p_Have9 = _talentManager.P_Have;
						for (int num21 = 0; num21 < p_Have9; num21++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					break;
				case 5:
					if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
					{
						int num22 = Level_Max - Level_Base;
						for (int num23 = 0; num23 < num22; num23++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					else
					{
						int p_Have10 = _talentManager.P_Have;
						for (int num24 = 0; num24 < p_Have10; num24++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					break;
				case 6:
					if (_talentManager.P_Have - (Level_Max - Level_Base) > 0)
					{
						int num = Level_Max - Level_Base;
						for (int i = 0; i < num; i++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					else
					{
						int p_Have = _talentManager.P_Have;
						for (int j = 0; j < p_Have; j++)
						{
							_talentManager.AddPoint(Xi, SkillType, IndexName);
						}
					}
					break;
				}
				if (Full)
				{
					RuntimeManager.PlayOneShot(_audioManager.audioData.LitButton_6);
				}
			}
			else
			{
				RuntimeManager.PlayOneShot(_audioManager.audioData.Skill_NoPoint3);
			}
		}
		else if (_talentManager.P_Have > 0 && Unlock && !Full)
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Add_Point_2);
			switch (SkillType)
			{
			case 0:
				if (_talentManager.HasBSSkill)
				{
					if (!BS_Skill)
					{
						_talentManager.AddPoint(Xi, SkillType, IndexName);
					}
					else if (_talentManager.BSSkillButton == this)
					{
						_talentManager.AddPoint(Xi, SkillType, IndexName);
					}
				}
				else
				{
					_talentManager.AddPoint(Xi, SkillType, IndexName);
					RememberBSSkillIfNeeded();
				}
				break;
			case 1:
				_talentManager.AddPoint(Xi, SkillType, IndexName);
				break;
			case 2:
				if (_talentManager.HasBSSkill)
				{
					if (!BS_Skill)
					{
						_talentManager.AddPoint(Xi, SkillType, IndexName);
					}
					else if (BS_Skill && _talentManager.BSSkillButton == this)
					{
						_talentManager.AddPoint(Xi, SkillType, IndexName);
					}
				}
				else
				{
					_talentManager.AddPoint(Xi, SkillType, IndexName);
					RememberBSSkillIfNeeded();
				}
				break;
			case 3:
				_talentManager.AddPoint(Xi, SkillType, IndexName);
				break;
			case 4:
				_talentManager.AddPoint(Xi, SkillType, IndexName);
				break;
			case 5:
				_talentManager.AddPoint(Xi, SkillType, IndexName);
				break;
			case 6:
				_talentManager.AddPoint(Xi, SkillType, IndexName);
				break;
			}
			if (Full)
			{
				RuntimeManager.PlayOneShot(_audioManager.audioData.LitButton_6);
			}
		}
		else
		{
			RuntimeManager.PlayOneShot(_audioManager.audioData.Skill_NoPoint3);
		}
		actbar.RefreshCD();
	}

	private void ButtonRightClick()
	{
	}
}
