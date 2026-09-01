using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkilDFData
{
	public const int NoneValue = 10000;

	public bool Unlocked;

	public int Index;

	public int SK_Count;

	public int Unlock_Point;

	public int Level_Base;

	public int Level_Max;

	public int LieA;

	public int LieB;

	public int LieC;

	public int FatherA;

	public int FatherB;

	public int FatherC;

	public int CurIndex;

	public List<SkilDFData_Lit> SK = new List<SkilDFData_Lit>();

	public SKillBT_DF skillbt;

	private int _selectedLitIndex = -1;

	public string IndexName => $"DF_{Index}";

	public int SelectedIndex
	{
		get
		{
			if (!HasMultipleChoices)
			{
				return 0;
			}
			return _selectedLitIndex + 1;
		}
		set
		{
			_selectedLitIndex = ((value <= 0) ? (-1) : (value - 1));
		}
	}

	public bool HasSelectedSkill
	{
		get
		{
			if (HasMultipleChoices)
			{
				return _selectedLitIndex >= 0;
			}
			return true;
		}
	}

	public int SkillSlotCount
	{
		get
		{
			if (SK == null)
			{
				return 0;
			}
			if (SK_Count > 0 && !IsNone(SK_Count))
			{
				return Mathf.Min(SK_Count, SK.Count);
			}
			return SK.Count;
		}
	}

	public int ValidSkillCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < SkillSlotCount; i++)
			{
				if (IsValidLit(SK[i]))
				{
					num++;
				}
			}
			return num;
		}
	}

	public bool HasMultipleChoices => ValidSkillCount > 1;

	public SkilDFData_Lit CurrentLit
	{
		get
		{
			EnsureValidCurIndex();
			if (CurIndex < 0 || CurIndex >= SK.Count)
			{
				return null;
			}
			return SK[CurIndex];
		}
	}

	public void SelectLit(int skillIndex)
	{
		CurIndex = skillIndex;
		if (HasMultipleChoices)
		{
			_selectedLitIndex = skillIndex;
		}
	}

	public void EnsureSelectedForLeveledSkill()
	{
		EnsureValidCurIndex();
		if (Level_Base <= 0 || HasSelectedSkill)
		{
			return;
		}
		if (IsValidSkillIndex(CurIndex))
		{
			SelectLit(CurIndex);
			return;
		}
		for (int i = 0; i < SkillSlotCount; i++)
		{
			if (IsValidSkillIndex(i))
			{
				SelectLit(i);
				break;
			}
		}
	}

	public bool IsValidSkillIndex(int index)
	{
		if (index >= 0 && index < SkillSlotCount)
		{
			return IsValidLit(SK[index]);
		}
		return false;
	}

	public void EnsureValidCurIndex()
	{
		if (HasMultipleChoices)
		{
			if (_selectedLitIndex < 0)
			{
				CurIndex = 0;
				return;
			}
			if (!IsValidSkillIndex(_selectedLitIndex))
			{
				_selectedLitIndex = -1;
			}
			if (_selectedLitIndex < 0)
			{
				CurIndex = 0;
				return;
			}
			CurIndex = _selectedLitIndex;
		}
		if (IsValidSkillIndex(CurIndex))
		{
			return;
		}
		for (int i = 0; i < SkillSlotCount; i++)
		{
			if (IsValidLit(SK[i]))
			{
				CurIndex = i;
				if (HasMultipleChoices)
				{
					_selectedLitIndex = i;
				}
				return;
			}
		}
		CurIndex = 0;
		_selectedLitIndex = -1;
	}

	public static bool IsNone(int value)
	{
		return value == 10000;
	}

	public static bool IsValidLit(SkilDFData_Lit lit)
	{
		if (lit != null && !string.IsNullOrEmpty(lit.IndexName) && lit.IndexName != 10000.ToString() && !IsNone(lit.Icon) && !IsNone(lit.Type))
		{
			return !IsNone(lit.Number);
		}
		return false;
	}

	public string GetTitle()
	{
		SkilDFData_Lit currentLit = CurrentLit;
		if (currentLit != null)
		{
			return LOC.MM.GetSkill(currentLit.IndexName) + " ";
		}
		return string.Empty;
	}

	public string GetInfoA()
	{
		SkilDFData_Lit currentLit = CurrentLit;
		if (currentLit == null)
		{
			return string.Empty;
		}
		return LOC.MM.GetSkill(currentLit.Info) + " : + " + FormatLitNumber(currentLit);
	}

	public string GetInfoBA()
	{
		SkilDFData_Lit currentLit = CurrentLit;
		if (currentLit == null)
		{
			return string.Empty;
		}
		return LOC.MM.GetMain("Current bonus") + " : + " + FormatLitNumber(currentLit, Level_Base);
	}

	public static string FormatLitNumber(SkilDFData_Lit lit, int level = 1)
	{
		if (lit == null)
		{
			return string.Empty;
		}
		int num = lit.Number * level;
		if (lit.Type <= 2 || lit.Type == 43 || lit.Type == 55)
		{
			return num.ToString();
		}
		return $"{num}%";
	}
}
