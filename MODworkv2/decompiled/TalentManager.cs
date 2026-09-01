using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core;
using Data.SaveData;
using FMODUnity;
using FinkFramework.Runtime.Data;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Localization;
using PoedbMod;
using Scenes;
using UI.Panels;
using UI.Talent;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TalentManager : ScopedSingletonMono<TalentManager>
{
	private struct ShortcutChildSkill
	{
		public readonly int Type;

		public readonly string SkillName;

		public ShortcutChildSkill(int type, string skillName)
		{
			Type = type;
			SkillName = skillName;
		}
	}

	private struct TalentPageReference
	{
		public readonly int XiIndex;

		public readonly bool IsDF;

		public TalentPageReference(int xiIndex, bool isDF)
		{
			XiIndex = xiIndex;
			IsDF = isDF;
		}
	}

	public Dictionary<string, SkillSaveData> Runtime_All_Skill_Datas;

	public Dictionary<string, XiSaveData> Runtime_All_Xi_Datas;

	public TalentSaveData SaveData;

	[HideInInspector]
	public int P_Base;

	[HideInInspector]
	public int P_Used;

	[HideInInspector]
	public int P_Used_DF;

	public Text pointText;

	private const int DFTalentUnlockLevel = 100;

	private const string RemainingSkillPointsKey = "Remaining skill points";

	private const int SkillFWCharCount = 4;

	private const int SkillFWXiPerChar = 3;

	public readonly Dictionary<string, SKindex> SKI = new Dictionary<string, SKindex>();

	public SkillXiData[] XiData;

	public SkillXiBT[] XiBT;

	public CanvasGroup[] XiCAV;

	public SkillXiBT DFXiBT;

	public CanvasGroup DFXiCAV;

	public CanvasGroup canvasGroup;

	public IconData[] iconDT;

	public IconData[] iconDTB;

	public IconData SPCA;

	public IconData SPCB;

	[HideInInspector]
	[FormerlySerializedAs("HasLastSkill")]
	public bool HasBSSkill;

	[FormerlySerializedAs("LastSkillButton")]
	public SkillBT BSSkillButton;

	public TextAsset XiTA;

	public TextAsset[] skillTA;

	public SKFW_Group FW;

	public List<SkilDFData> DFData = new List<SkilDFData>();

	private readonly HashSet<SKillBT_DF> _dfSkillBTs = new HashSet<SKillBT_DF>();

	private readonly HashSet<SKillBT_Lie> _dfLieBTs = new HashSet<SKillBT_Lie>();

	public List<SkilChangeData> SKC_Data = new List<SkilChangeData>();

	public Dictionary<int, CompSkillChangeData> CPC_Data = new Dictionary<int, CompSkillChangeData>();

	private PlayerManager PL;

	private AudioManager _audioManager;

	private readonly HashSet<SkillBT> _skillBTs = new HashSet<SkillBT>();

	private bool _talentTablesLoaded;

	private static bool _poedbHasEnsured;

	public bool HasOpenedTalentPanel
	{
		get
		{
			if (SaveData != null)
			{
				return SaveData.HasOpenedTalentPanel;
			}
			return false;
		}
	}

	public bool HasAddedAnySkillPoint
	{
		get
		{
			if (SaveData != null)
			{
				return SaveData.HasAddedAnySkillPoint;
			}
			return false;
		}
	}

	public bool HasOpenedActSkillListAfterFirstSkillPoint
	{
		get
		{
			if (SaveData != null)
			{
				return SaveData.HasOpenedActSkillListAfterFirstSkillPoint;
			}
			return false;
		}
	}

	public int P_Have => P_Base - P_Used - P_Used_DF;

	public bool IsTalentDataReady { get; private set; }

	public void InitFromSaveData(TalentSaveData data)
	{
		SaveData = DataUtil.DeepClone(data);
		ApplySaveData(SaveData);
	}

	public void ApplySaveData(TalentSaveData data)
	{
		if (data == null)
		{
			data = TalentSaveData.CreateDefault();
		}
		P_Base = data.P_Base;
		P_Used = data.P_Used;
		P_Used_DF = data.P_Used_DF;
		SaveData = DataUtil.DeepClone(data);
		Runtime_All_Skill_Datas = CloneSkillDict(data.All_Skill_Datas);
		Runtime_All_Xi_Datas = CloneXiDict(data.All_Xi_Datas);
		RestoreDatas();
		RefreshActionBarGuide();
	}

	public TalentSaveData ExportSaveData()
	{
		FlushDatas();
		return new TalentSaveData
		{
			P_Base = P_Base,
			P_Used = P_Used,
			P_Used_DF = P_Used_DF,
			HasOpenedTalentPanel = HasOpenedTalentPanel,
			HasAddedAnySkillPoint = HasAddedAnySkillPoint,
			HasOpenedActSkillListAfterFirstSkillPoint = HasOpenedActSkillListAfterFirstSkillPoint,
			All_Skill_Datas = CloneSkillDict(Runtime_All_Skill_Datas),
			All_Xi_Datas = CloneXiDict(Runtime_All_Xi_Datas)
		};
	}

	public void MarkTalentPanelOpened()
	{
		if (SaveData != null && !SaveData.HasOpenedTalentPanel)
		{
			SaveData.HasOpenedTalentPanel = true;
			RefreshActionBarGuide();
		}
	}

	public void MarkSkillPointAdded()
	{
		if (SaveData != null && !SaveData.HasAddedAnySkillPoint)
		{
			SaveData.HasAddedAnySkillPoint = true;
			RefreshActionBarGuide();
		}
	}

	public void MarkActSkillListOpenedAfterFirstSkillPoint()
	{
		if (SaveData != null && SaveData.HasAddedAnySkillPoint && !SaveData.HasOpenedActSkillListAfterFirstSkillPoint)
		{
			SaveData.HasOpenedActSkillListAfterFirstSkillPoint = true;
			RefreshActionBarGuide();
		}
	}

	private static void RefreshActionBarGuide()
	{
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RefreshBeginnerGuide();
		}
	}

	private void FlushDatas()
	{
		Dictionary<string, SkillSaveData> dictionary = new Dictionary<string, SkillSaveData>();
		foreach (ISkillLevelData item in EnumerateAllSkills())
		{
			dictionary[item.IndexName] = new SkillSaveData
			{
				Level_Base = item.Level_Base,
				Level_WeaponOn = 0
			};
		}
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null)
			{
				dictionary[dFDatum.IndexName] = new SkillSaveData
				{
					Level_Base = dFDatum.Level_Base,
					SelectedIndex = dFDatum.SelectedIndex
				};
			}
		}
		Runtime_All_Skill_Datas = dictionary;
		Dictionary<string, XiSaveData> dictionary2 = new Dictionary<string, XiSaveData>();
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData != null)
			{
				dictionary2[skillXiData.IndexName] = new XiSaveData
				{
					Level_Base = skillXiData.Level_Base
				};
			}
		}
		Runtime_All_Xi_Datas = dictionary2;
	}

	private void RestoreDatas()
	{
		if (Runtime_All_Skill_Datas == null)
		{
			Runtime_All_Skill_Datas = new Dictionary<string, SkillSaveData>();
		}
		if (Runtime_All_Xi_Datas == null)
		{
			Runtime_All_Xi_Datas = new Dictionary<string, XiSaveData>();
		}
		foreach (ISkillLevelData item in EnumerateAllSkills())
		{
			if (Runtime_All_Skill_Datas.TryGetValue(item.IndexName, out var value) && value != null)
			{
				item.Level_Base = value.Level_Base;
				item.Level_WeaponOn = 0;
			}
		}
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null && Runtime_All_Skill_Datas.TryGetValue(dFDatum.IndexName, out var value2) && value2 != null)
			{
				dFDatum.Level_Base = value2.Level_Base;
				dFDatum.SelectedIndex = value2.SelectedIndex;
				dFDatum.EnsureSelectedForLeveledSkill();
			}
		}
		P_Used_DF = CalculateUsedDFPoints();
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData != null && Runtime_All_Xi_Datas.TryGetValue(skillXiData.IndexName, out var value3) && value3 != null)
			{
				skillXiData.Level_Base = value3.Level_Base;
			}
		}
	}

	private void RebuildActbarFromLevels()
	{
		for (int i = 0; i < XiData.Length; i++)
		{
			SkillXiData skillXiData = XiData[i];
			if (skillXiData == null)
			{
				continue;
			}
			foreach (SkillData_Sample_Father value4 in skillXiData.Sample_F.Values)
			{
				if (value4.Level_Base > 0)
				{
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(i, 0, value4);
				}
			}
			foreach (SkillData_Sample_Son value5 in skillXiData.Sample_S.Values)
			{
				if (value5.Level_Base > 0 && skillXiData.Sample_F.TryGetValue(value5.FatherSkill, out var value) && value != null)
				{
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(i, 1, value);
				}
			}
			foreach (SkillData_Comp_Father value6 in skillXiData.Comp_F.Values)
			{
				if (value6.Level_Base > 0)
				{
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(i, 2, value6);
				}
			}
			foreach (SkillData_Comp_Son value7 in skillXiData.Comp_S.Values)
			{
				if (value7.Level_Base > 0 && skillXiData.Comp_F.TryGetValue(value7.FatherSkill, out var value2) && value2 != null)
				{
					SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(i, 3, value2);
				}
			}
			foreach (SkillData_Dot_Father value8 in skillXiData.Dot_F.Values)
			{
				if (value8.Level_Base > 0)
				{
					SingletonMonoScope<ACTbar>.Instance.SetDot(value8);
				}
			}
			foreach (SkillData_Dot_Son value9 in skillXiData.Dot_S.Values)
			{
				if (value9.Level_Base > 0 && skillXiData.Dot_F.TryGetValue(value9.FatherSkill, out var value3) && value3 != null)
				{
					SingletonMonoScope<ACTbar>.Instance.SetDot(value3);
				}
			}
		}
	}

	public void RegisterSkillBT(SkillBT bt)
	{
		if (!bt)
		{
			return;
		}
		_skillBTs.Add(bt);
		if (IsTalentDataReady)
		{
			TryBindSkillBT(bt);
			if (bt.Xi >= 0 && bt.Xi < XiData.Length)
			{
				Refresh(bt.Xi);
				RecalcHasBSSkill();
			}
		}
	}

	public void UnregisterSkillBT(SkillBT bt)
	{
		if ((bool)bt)
		{
			_skillBTs.Remove(bt);
			ClearSkillBT(bt, bt.Xi, bt.SkillType, bt.IndexName);
		}
	}

	public void RegisterDFSkillBT(SKillBT_DF bt)
	{
		if ((bool)bt)
		{
			_dfSkillBTs.Add(bt);
			if (IsTalentDataReady)
			{
				BindDFSkillBT(bt);
				RefreshDF();
			}
		}
	}

	public void UnregisterDFSkillBT(SKillBT_DF bt)
	{
		if ((bool)bt)
		{
			_dfSkillBTs.Remove(bt);
			SkilDFData dFData = GetDFData(bt.Index);
			if (dFData != null && dFData.skillbt == bt)
			{
				dFData.skillbt = null;
			}
		}
	}

	public void RegisterDFLieBT(SKillBT_Lie bt)
	{
		if ((bool)bt)
		{
			_dfLieBTs.Add(bt);
			if (IsTalentDataReady)
			{
				RefreshDFLieBT(bt);
			}
		}
	}

	public void UnregisterDFLieBT(SKillBT_Lie bt)
	{
		if ((bool)bt)
		{
			_dfLieBTs.Remove(bt);
		}
	}

	public void RebindAllSkillBT()
	{
		if (!IsTalentDataReady)
		{
			return;
		}
		foreach (SkillBT skillBT in _skillBTs)
		{
			if ((bool)skillBT)
			{
				TryBindSkillBT(skillBT);
			}
		}
		foreach (SKillBT_DF dfSkillBT in _dfSkillBTs)
		{
			if ((bool)dfSkillBT)
			{
				BindDFSkillBT(dfSkillBT);
			}
		}
	}

	private void TryBindSkillBT(SkillBT bt)
	{
		int xi = bt.Xi;
		string indexName = bt.IndexName;
		if (xi < 0 || xi >= XiData.Length || XiData[xi] == null)
		{
			return;
		}
		switch (bt.SkillType)
		{
		case 0:
		{
			if (XiData[xi].Sample_F.TryGetValue(indexName, out var value6))
			{
				value6.skillbt = bt;
			}
			break;
		}
		case 1:
		{
			if (XiData[xi].Sample_S.TryGetValue(indexName, out var value2))
			{
				value2.skillbt = bt;
			}
			break;
		}
		case 2:
		{
			if (XiData[xi].Comp_F.TryGetValue(indexName, out var value4))
			{
				value4.skillbt = bt;
			}
			break;
		}
		case 3:
		{
			if (XiData[xi].Comp_S.TryGetValue(indexName, out var value7))
			{
				value7.skillbt = bt;
			}
			break;
		}
		case 4:
		{
			if (XiData[xi].Dot_F.TryGetValue(indexName, out var value5))
			{
				value5.skillbt = bt;
			}
			break;
		}
		case 5:
		{
			if (XiData[xi].Dot_S.TryGetValue(indexName, out var value3))
			{
				value3.skillbt = bt;
			}
			break;
		}
		case 6:
		{
			if (XiData[xi].Bei.TryGetValue(indexName, out var value))
			{
				value.skillbt = bt;
			}
			break;
		}
		}
	}

	public void ClearSkillBT(SkillBT bt, int xi, int type, string skillName)
	{
		if (xi < 0 || xi >= XiData.Length || XiData[xi] == null)
		{
			return;
		}
		switch (type)
		{
		case 0:
		{
			if (XiData[xi].Sample_F.TryGetValue(skillName, out var value2) && value2.skillbt == bt)
			{
				value2.skillbt = null;
			}
			break;
		}
		case 1:
		{
			if (XiData[xi].Sample_S.TryGetValue(skillName, out var value6) && value6.skillbt == bt)
			{
				value6.skillbt = null;
			}
			break;
		}
		case 2:
		{
			if (XiData[xi].Comp_F.TryGetValue(skillName, out var value3) && value3.skillbt == bt)
			{
				value3.skillbt = null;
			}
			break;
		}
		case 3:
		{
			if (XiData[xi].Comp_S.TryGetValue(skillName, out var value5) && value5.skillbt == bt)
			{
				value5.skillbt = null;
			}
			break;
		}
		case 4:
		{
			if (XiData[xi].Dot_F.TryGetValue(skillName, out var value7) && value7.skillbt == bt)
			{
				value7.skillbt = null;
			}
			break;
		}
		case 5:
		{
			if (XiData[xi].Dot_S.TryGetValue(skillName, out var value4) && value4.skillbt == bt)
			{
				value4.skillbt = null;
			}
			break;
		}
		case 6:
		{
			if (XiData[xi].Bei.TryGetValue(skillName, out var value) && value.skillbt == bt)
			{
				value.skillbt = null;
			}
			break;
		}
		}
	}

	public void SetSkillBT(SkillBT bt, int xi, int type, string skillName)
	{
		if (XiData == null || xi < 0 || xi >= XiData.Length || XiData[xi] == null)
		{
			LogUtil.Warn($"[TalentManager.SetSkillBT] Xi越界或未就绪 xi={xi} skill={skillName}");
			return;
		}
		if (string.IsNullOrEmpty(skillName))
		{
			LogUtil.Warn("[TalentManager.SetSkillBT] skillName空");
			return;
		}
		switch (type)
		{
		case 0:
		{
			if (!XiData[xi].Sample_F.TryGetValue(skillName, out var value16) || value16 == null)
			{
				LogUtil.Warn($"[TalentManager.SetSkillBT] 未找到 Sample_F xi={xi} skill={skillName},已防崩跳过");
				break;
			}
			value16.skillbt = bt;
			value16.skillbt.BS_Skill = value16.BS_Skill;
			value16.skillbt.LastSkill = value16.LastSkill;
			value16.skillbt.DashSkill = value16.DashSkill;
			value16.skillbt.TPSkill = value16.TPSkill;
			value16.skillbt.Refresh(value16.Level_Base, value16.Level_Max, value16.Level_WeaponOn);
			if (XiData[xi].Level_Base >= value16.UnLock_Point)
			{
				value16.skillbt.SkillTU.sprite = value16.icon;
				value16.skillbt.Unlock = true;
			}
			else
			{
				value16.skillbt.SkillTU.sprite = value16.iconB;
			}
			break;
		}
		case 1:
		{
			if (!XiData[xi].Sample_S.TryGetValue(skillName, out var value6) || value6 == null)
			{
				LogUtil.Warn($"[TalentManager.SetSkillBT] 未找到 Sample_S xi={xi} skill={skillName},已防崩跳过");
				break;
			}
			value6.skillbt = bt;
			value6.skillbt.Refresh(value6.Level_Base, value6.Level_Max, value6.Level_WeaponOn);
			if (XiData[xi].Level_Base >= value6.UnLock_Point)
			{
				value6.skillbt.SkillTU.sprite = value6.icon;
			}
			else
			{
				value6.skillbt.SkillTU.sprite = value6.iconB;
			}
			XiData[xi].Sample_F.TryGetValue(value6.FatherSkill, out var value7);
			if (value7.Level_Base <= 0)
			{
				break;
			}
			if (value6.FrontSkillType == 0)
			{
				XiData[xi].Sample_F.TryGetValue(value6.FrontSkill, out var value8);
				if (value8.Level_Base > 0)
				{
					value6.skillbt.Unlock = true;
				}
				else
				{
					value6.skillbt.Unlock = false;
				}
			}
			else if (value6.FrontSkillType == 1)
			{
				XiData[xi].Sample_S.TryGetValue(value6.FrontSkill, out var value9);
				if (value9.Level_Base > 0)
				{
					value6.skillbt.Unlock = true;
				}
				else
				{
					value6.skillbt.Unlock = false;
				}
			}
			break;
		}
		case 2:
		{
			if (!XiData[xi].Comp_F.TryGetValue(skillName, out var value11) || value11 == null)
			{
				LogUtil.Warn($"[TalentManager.SetSkillBT] 未找到 Comp_F xi={xi} skill={skillName},已防崩跳过");
				break;
			}
			value11.skillbt = bt;
			value11.skillbt.BS_Skill = value11.BS_Skill;
			value11.skillbt.LastSkill = value11.LastSkill;
			value11.skillbt.Refresh(value11.Level_Base, value11.Level_Max, value11.Level_WeaponOn);
			if (XiData[xi].Level_Base >= value11.UnLock_Point)
			{
				value11.skillbt.SkillTU.sprite = value11.icon;
				value11.skillbt.Unlock = true;
			}
			else
			{
				value11.skillbt.SkillTU.sprite = value11.iconB;
			}
			break;
		}
		case 3:
		{
			if (!XiData[xi].Comp_S.TryGetValue(skillName, out var value12) || value12 == null)
			{
				LogUtil.Warn($"[TalentManager.SetSkillBT] 未找到 Comp_S xi={xi} skill={skillName},已防崩跳过");
				break;
			}
			value12.skillbt = bt;
			value12.skillbt.Refresh(value12.Level_Base, value12.Level_Max, value12.Level_WeaponOn);
			if (XiData[xi].Level_Base >= value12.UnLock_Point)
			{
				value12.skillbt.SkillTU.sprite = value12.icon;
			}
			else
			{
				value12.skillbt.SkillTU.sprite = value12.iconB;
			}
			XiData[xi].Comp_F.TryGetValue(value12.FatherSkill, out var value13);
			if (value13.Level_Base <= 0)
			{
				break;
			}
			if (value12.FrontSkillType == 2)
			{
				XiData[xi].Comp_F.TryGetValue(value12.FrontSkill, out var value14);
				if (value14.Level_Base > 0)
				{
					value12.skillbt.Unlock = true;
				}
				else
				{
					value12.skillbt.Unlock = false;
				}
			}
			else if (value12.FrontSkillType == 3)
			{
				XiData[xi].Comp_S.TryGetValue(value12.FrontSkill, out var value15);
				if (value15.Level_Base > 0)
				{
					value12.skillbt.Unlock = true;
				}
				else
				{
					value12.skillbt.Unlock = false;
				}
			}
			break;
		}
		case 4:
		{
			if (!XiData[xi].Dot_F.TryGetValue(skillName, out var value10) || value10 == null)
			{
				LogUtil.Warn($"[TalentManager.SetSkillBT] 未找到 Dot_F xi={xi} skill={skillName},已防崩跳过");
				break;
			}
			value10.skillbt = bt;
			value10.skillbt.Refresh(value10.Level_Base, value10.Level_Max, value10.Level_WeaponOn);
			if (XiData[xi].Level_Base >= value10.UnLock_Point)
			{
				value10.skillbt.SkillTU.sprite = value10.icon;
				value10.skillbt.Unlock = true;
			}
			else
			{
				value10.skillbt.SkillTU.sprite = value10.iconB;
			}
			break;
		}
		case 5:
		{
			if (!XiData[xi].Dot_S.TryGetValue(skillName, out var value2) || value2 == null)
			{
				LogUtil.Warn($"[TalentManager.SetSkillBT] 未找到 Dot_S xi={xi} skill={skillName},已防崩跳过");
				break;
			}
			value2.skillbt = bt;
			value2.skillbt.Refresh(value2.Level_Base, value2.Level_Max, value2.Level_WeaponOn);
			if (XiData[xi].Level_Base >= value2.UnLock_Point)
			{
				value2.skillbt.SkillTU.sprite = value2.icon;
			}
			else
			{
				value2.skillbt.SkillTU.sprite = value2.iconB;
			}
			XiData[xi].Dot_F.TryGetValue(value2.FatherSkill, out var value3);
			if (value3.Level_Base <= 0)
			{
				break;
			}
			if (value2.FrontSkillType == 4)
			{
				XiData[xi].Dot_F.TryGetValue(value2.FrontSkill, out var value4);
				if (value4.Level_Base > 0)
				{
					value2.skillbt.Unlock = true;
				}
				else
				{
					value2.skillbt.Unlock = false;
				}
			}
			else if (value2.FrontSkillType == 5)
			{
				XiData[xi].Dot_S.TryGetValue(value2.FrontSkill, out var value5);
				if (value5.Level_Base > 0)
				{
					value2.skillbt.Unlock = true;
				}
				else
				{
					value2.skillbt.Unlock = false;
				}
			}
			break;
		}
		case 6:
		{
			if (!XiData[xi].Bei.TryGetValue(skillName, out var value) || value == null)
			{
				LogUtil.Warn($"[TalentManager.SetSkillBT] 未找到 Bei xi={xi} skill={skillName},已防崩跳过");
				break;
			}
			value.skillbt = bt;
			value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
			if (P_Used > 0)
			{
				if (XiData[xi].Level_Base >= value.UnLock_Point)
				{
					value.skillbt.SkillTU.sprite = value.icon;
					value.skillbt.Unlock = true;
				}
				else
				{
					value.skillbt.SkillTU.sprite = value.iconB;
					value.skillbt.Unlock = false;
				}
			}
			else
			{
				value.skillbt.SkillTU.sprite = value.iconB;
				value.skillbt.Unlock = false;
			}
			break;
		}
		}
	}

	private void BindDFSkillBT(SKillBT_DF bt)
	{
		SkilDFData dFData = GetDFData(bt.Index);
		if (dFData != null)
		{
			dFData.skillbt = bt;
			dFData.EnsureValidCurIndex();
			RefreshDFSkillBT(dFData);
		}
	}

	public SkilDFData GetDFData(int index)
	{
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null && dFDatum.Index == index)
			{
				return dFDatum;
			}
		}
		return null;
	}

	public bool DFHasMultipleChoices(int index)
	{
		return GetDFData(index)?.HasMultipleChoices ?? false;
	}

	public bool HasUsedDFTalentPoint()
	{
		return CalculateUsedDFPoints() > 0;
	}

	private bool IsDFTalentUnlockedByPlayerLevel()
	{
		if (PL != null)
		{
			return PL.Level >= 100;
		}
		return false;
	}

	private void ShowDFTalentLevelLockedTip()
	{
		GameManager.ShowTip(LOC.MM.GetMain("Unlock at level 100"), TipType.Fail);
	}

	public void SelectDFSkill(int index, int skillIndex)
	{
		if (!IsDFTalentUnlockedByPlayerLevel())
		{
			ShowDFTalentLevelLockedTip();
			return;
		}
		SkilDFData dFData = GetDFData(index);
		if (dFData == null || !dFData.IsValidSkillIndex(skillIndex))
		{
			return;
		}
		dFData.EnsureSelectedForLeveledSkill();
		if (dFData.HasSelectedSkill && dFData.CurIndex == skillIndex)
		{
			RefreshDF();
			RefreshDFSkillBT(dFData);
			if (SingletonMonoScope<GameUIManager>.HasInstance)
			{
				SingletonMonoScope<GameUIManager>.Instance.RefreshDFTip(index);
			}
			return;
		}
		SkilDFData_Lit currentLit = dFData.CurrentLit;
		if (dFData.Level_Base > 0)
		{
			ApplyDFSkillEffect(currentLit, add: false, dFData.Level_Base);
		}
		dFData.SelectLit(skillIndex);
		SkilDFData_Lit currentLit2 = dFData.CurrentLit;
		if (dFData.Level_Base > 0)
		{
			ApplyDFSkillEffect(currentLit2, add: true, dFData.Level_Base);
		}
		RefreshDF();
		RefreshDFSkillBT(dFData);
		FlushDatas();
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.RefreshDFTip(index);
		}
	}

	public void AddPointDF(int index)
	{
		if (!IsDFTalentUnlockedByPlayerLevel())
		{
			ShowDFTalentLevelLockedTip();
			return;
		}
		SkilDFData dFData = GetDFData(index);
		if (dFData == null)
		{
			return;
		}
		RefreshDF();
		if (dFData.Unlocked && dFData.HasSelectedSkill && dFData.Level_Base < dFData.Level_Max && P_Have > 0)
		{
			dFData.Level_Base++;
			ApplyDFSkillEffect(dFData, add: true, 1);
			P_Used_DF++;
			MarkSkillPointAdded();
			RefreshPointText();
			RefreshDF();
			SetXiBuff();
			FlushDatas();
			if (SingletonMonoScope<GameUIManager>.HasInstance)
			{
				SingletonMonoScope<GameUIManager>.Instance.RefreshDFTip(index);
			}
		}
	}

	private void ApplyDFSkillEffect(SkilDFData data, bool add, int level)
	{
		if (!(PL == null) && data != null && level > 0)
		{
			SkilDFData_Lit currentLit = data.CurrentLit;
			ApplyDFSkillEffect(currentLit, add, level);
		}
	}

	private void ApplyDFSkillEffect(SkilDFData_Lit lit, bool add, int level)
	{
		if (!(PL == null) && lit != null && level > 0 && SkilDFData.IsValidLit(lit))
		{
			PL.SetDFSkillBuff(add, lit.Type, lit.Number, level);
		}
	}

	private void ClearDFRuntimeTalentEffects()
	{
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null && dFDatum.Level_Base > 0)
			{
				dFDatum.EnsureSelectedForLeveledSkill();
				ApplyDFSkillEffect(dFDatum, add: false, dFDatum.Level_Base);
			}
		}
	}

	public void RefreshDF()
	{
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null)
			{
				dFDatum.EnsureSelectedForLeveledSkill();
				dFDatum.EnsureValidCurIndex();
				dFDatum.Unlocked = IsDFUnlocked(dFDatum);
				RefreshDFSkillBT(dFDatum);
			}
		}
		foreach (SKillBT_Lie dfLieBT in _dfLieBTs)
		{
			if ((bool)dfLieBT)
			{
				RefreshDFLieBT(dfLieBT);
			}
		}
	}

	public int GetDFLiePoint(int lie)
	{
		if (SkilDFData.IsNone(lie))
		{
			return 0;
		}
		int num = 0;
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null && dFDatum.Level_Base > 0 && (dFDatum.LieA == lie || dFDatum.LieB == lie || dFDatum.LieC == lie))
			{
				num += dFDatum.Level_Base;
			}
		}
		return num;
	}

	private void RefreshDFLieBT(SKillBT_Lie bt)
	{
		if ((bool)bt)
		{
			bt.Refresh(GetDFLiePoint(bt.Type));
		}
	}

	private bool IsDFUnlocked(SkilDFData data)
	{
		if (data == null)
		{
			return false;
		}
		if (!IsDFTalentUnlockedByPlayerLevel())
		{
			return false;
		}
		if (!IsDFLieRequirementMet(data))
		{
			return false;
		}
		return IsDFFatherRequirementMet(data);
	}

	private bool IsDFLieRequirementMet(SkilDFData data)
	{
		bool flag = false;
		if (!SkilDFData.IsNone(data.LieA))
		{
			flag = true;
			if (GetDFLiePoint(data.LieA) < data.Unlock_Point)
			{
				return false;
			}
		}
		if (!SkilDFData.IsNone(data.LieB))
		{
			flag = true;
			if (GetDFLiePoint(data.LieB) < data.Unlock_Point)
			{
				return false;
			}
		}
		if (!SkilDFData.IsNone(data.LieC))
		{
			flag = true;
			if (GetDFLiePoint(data.LieC) < data.Unlock_Point)
			{
				return false;
			}
		}
		if (!flag)
		{
			return data.Unlock_Point <= 0;
		}
		return true;
	}

	private bool IsDFFatherRequirementMet(SkilDFData data)
	{
		if (!SkilDFData.IsNone(data.FatherA) && !IsDFSkillLeveled(data.FatherA))
		{
			return false;
		}
		if (!SkilDFData.IsNone(data.FatherB) && !IsDFSkillLeveled(data.FatherB))
		{
			return false;
		}
		if (!SkilDFData.IsNone(data.FatherC) && !IsDFSkillLeveled(data.FatherC))
		{
			return false;
		}
		return true;
	}

	private bool IsDFSkillLeveled(int index)
	{
		SkilDFData dFData = GetDFData(index);
		if (dFData != null)
		{
			return dFData.Level_Base > 0;
		}
		return false;
	}

	private void RefreshDFSkillBT(SkilDFData data)
	{
		if (data != null && (bool)data.skillbt)
		{
			Sprite dFIcon = GetDFIcon(data, data.Unlocked && data.HasSelectedSkill);
			data.skillbt.Refresh(data.Level_Base, data.Level_Max, data.Unlocked, dFIcon);
		}
	}

	public Sprite GetDFIcon(SkilDFData data, bool unlocked)
	{
		if (data == null)
		{
			return null;
		}
		if (data.HasMultipleChoices && !data.HasSelectedSkill)
		{
			if (!SingletonMonoScope<GameUIManager>.HasInstance)
			{
				return null;
			}
			return SingletonMonoScope<GameUIManager>.Instance.IconADD;
		}
		SkilDFData_Lit currentLit = data.CurrentLit;
		if (currentLit == null)
		{
			return null;
		}
		return GetDFIconByIndex(currentLit.Icon, unlocked);
	}

	public Sprite GetDFIcon(int index, int skillIndex, bool unlocked)
	{
		SkilDFData dFData = GetDFData(index);
		if (dFData == null || !dFData.IsValidSkillIndex(skillIndex))
		{
			return null;
		}
		SkilDFData_Lit skilDFData_Lit = dFData.SK[skillIndex];
		return GetDFIconByIndex(skilDFData_Lit.Icon, unlocked);
	}

	private IconData GetDFIconData(bool unlocked)
	{
		IconData iconData = (unlocked ? SPCA : SPCB);
		if ((bool)iconData)
		{
			return iconData;
		}
		LogUtil.Error(unlocked ? "巅峰彩色图标库 SPCA 未配置" : "巅峰黑白图标库 SPCB 未配置");
		return null;
	}

	private Sprite GetDFIconByIndex(int iconIndex, bool unlocked)
	{
		Sprite spriteFromIconData = GetSpriteFromIconData(GetDFIconData(unlocked), iconIndex);
		if ((bool)spriteFromIconData)
		{
			return spriteFromIconData;
		}
		return GetSpriteFromIconData(GetDFIconData(!unlocked), iconIndex);
	}

	private Sprite GetSpriteFromIconData(IconData iconData, int iconIndex)
	{
		if (!iconData || iconData.icon == null || iconIndex < 0 || iconIndex >= iconData.icon.Length)
		{
			return null;
		}
		return iconData.icon[iconIndex];
	}

	public bool TryAddNormalSkillFromShortcut(int xi, int type, string skillName, bool includeChildren)
	{
		int num = AddNormalSkillToMax(xi, type, skillName);
		if (includeChildren)
		{
			foreach (ShortcutChildSkill item in GetShortcutChildSkillOrder(xi, type, skillName))
			{
				if (P_Have <= 0)
				{
					break;
				}
				num += AddNormalSkillToMax(xi, item.Type, item.SkillName);
			}
		}
		return num > 0;
	}

	private int AddNormalSkillToMax(int xi, int type, string skillName)
	{
		int num = 0;
		SkillData data;
		while (P_Have > 0 && CanAddNormalSkillPoint(xi, type, skillName, out data))
		{
			AddPoint(xi, type, skillName);
			RememberBSSkillButton(data);
			num++;
		}
		return num;
	}

	private bool CanAddNormalSkillPoint(int xi, int type, string skillName, out SkillData data)
	{
		data = GetNormalSkillData(xi, type, skillName);
		if (data == null || data.skillbt == null)
		{
			return false;
		}
		if (!data.skillbt.Unlock || data.Level_Base >= data.Level_Max || P_Have <= 0)
		{
			return false;
		}
		if (IsBlockedByOtherBSSkill(data))
		{
			return false;
		}
		return true;
	}

	private SkillData GetNormalSkillData(int xi, int type, string skillName)
	{
		if (XiData == null || xi < 0 || xi >= XiData.Length || XiData[xi] == null || string.IsNullOrEmpty(skillName))
		{
			return null;
		}
		SkillXiData skillXiData = XiData[xi];
		switch (type)
		{
		case 0:
		{
			if (!skillXiData.Sample_F.TryGetValue(skillName, out var value2))
			{
				return null;
			}
			return value2;
		}
		case 1:
		{
			if (!skillXiData.Sample_S.TryGetValue(skillName, out var value6))
			{
				return null;
			}
			return value6;
		}
		case 2:
		{
			if (!skillXiData.Comp_F.TryGetValue(skillName, out var value3))
			{
				return null;
			}
			return value3;
		}
		case 3:
		{
			if (!skillXiData.Comp_S.TryGetValue(skillName, out var value5))
			{
				return null;
			}
			return value5;
		}
		case 4:
		{
			if (!skillXiData.Dot_F.TryGetValue(skillName, out var value7))
			{
				return null;
			}
			return value7;
		}
		case 5:
		{
			if (!skillXiData.Dot_S.TryGetValue(skillName, out var value4))
			{
				return null;
			}
			return value4;
		}
		case 6:
		{
			if (!skillXiData.Bei.TryGetValue(skillName, out var value))
			{
				return null;
			}
			return value;
		}
		default:
			return null;
		}
	}

	private bool IsBlockedByOtherBSSkill(SkillData data)
	{
		if (!IsBSSkillData(data))
		{
			return false;
		}
		if (HasBSSkill)
		{
			return BSSkillButton != data.skillbt;
		}
		return false;
	}

	private static bool IsBSSkillData(SkillData data)
	{
		if (data is SkillData_Sample_Father skillData_Sample_Father)
		{
			return skillData_Sample_Father.BS_Skill;
		}
		if (data is SkillData_Comp_Father skillData_Comp_Father)
		{
			return skillData_Comp_Father.BS_Skill;
		}
		return false;
	}

	private void RememberBSSkillButton(SkillData data)
	{
		if (IsBSSkillData(data) && (bool)data.skillbt)
		{
			BSSkillButton = data.skillbt;
		}
	}

	private IEnumerable<ShortcutChildSkill> GetShortcutChildSkillOrder(int xi, int type, string skillName)
	{
		if (XiData == null || xi < 0 || xi >= XiData.Length || XiData[xi] == null)
		{
			yield break;
		}
		SkillXiData skillXiData = XiData[xi];
		switch (type)
		{
		case 0:
		{
			if (!skillXiData.Sample_F.TryGetValue(skillName, out var value2))
			{
				break;
			}
			foreach (string item in EnumerateChildNames(value2.SonA, value2.SonB, value2.SonC))
			{
				yield return new ShortcutChildSkill(1, item);
			}
			break;
		}
		case 2:
		{
			if (!skillXiData.Comp_F.TryGetValue(skillName, out var value3))
			{
				break;
			}
			foreach (string item2 in EnumerateChildNames(value3.SonA, value3.SonB, value3.SonC))
			{
				yield return new ShortcutChildSkill(3, item2);
			}
			break;
		}
		case 4:
		{
			if (!skillXiData.Dot_F.TryGetValue(skillName, out var value))
			{
				break;
			}
			foreach (string item3 in EnumerateChildNames(value.SonA, value.SonB, value.SonC, value.SonD))
			{
				yield return new ShortcutChildSkill(5, item3);
			}
			break;
		}
		}
	}

	private static IEnumerable<string> EnumerateChildNames(params string[] names)
	{
		if (names == null)
		{
			yield break;
		}
		for (int i = 0; i < names.Length; i++)
		{
			if (!string.IsNullOrEmpty(names[i]))
			{
				yield return names[i];
			}
		}
	}

	public void AddPoint(int xi, int type, string skillName)
	{
		RebindAllSkillBT();
		XiData[xi].Level_Base++;
		P_Used++;
		MarkSkillPointAdded();
		RefreshPointText();
		switch (type)
		{
		case 0:
			AddPointSampleFather(xi, skillName);
			break;
		case 1:
			AddPointSampleSon(xi, skillName);
			break;
		case 2:
			AddPointCompFather(xi, skillName);
			break;
		case 3:
			AddPointCompSon(xi, skillName);
			break;
		case 4:
			AddPointDotFather(xi, skillName);
			break;
		case 5:
			AddPointDotSon(xi, skillName);
			break;
		case 6:
			AddPointBei(xi, skillName);
			break;
		}
		Refresh(xi);
		SetXiBuff();
		SingletonMonoScope<GameUIManager>.Instance.RefreshSkilltip(xi, type, skillName);
	}

	public void LevelUP()
	{
		P_Base++;
		RefreshPointText();
	}

	private void AddPointSampleFather(int xi, string skillName)
	{
		XiData[xi].Sample_F.TryGetValue(skillName, out var value);
		value.Level_Base++;
		if (value.BS_Skill)
		{
			HasBSSkill = true;
		}
		SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(xi, 0, value);
		if ((bool)value.skillbt)
		{
			value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
		}
	}

	private void AddPointSampleSon(int xi, string skillName)
	{
		XiData[xi].Sample_S.TryGetValue(skillName, out var value);
		XiData[xi].Sample_F.TryGetValue(value.FatherSkill, out var value2);
		switch (value.FrontSkillType)
		{
		case 0:
			if (value2.Level_Base > 0)
			{
				value.Level_Base++;
				if ((bool)value.skillbt)
				{
					value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
				}
			}
			break;
		case 1:
		{
			XiData[xi].Sample_S.TryGetValue(value.FrontSkill, out var value3);
			if (value2.Level_Base > 0 && value3.Level_Base > 0)
			{
				value.Level_Base++;
				if ((bool)value.skillbt)
				{
					value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
				}
			}
			break;
		}
		}
		SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotSP(xi, 1, value2);
	}

	private void AddPointCompFather(int xi, string skillName)
	{
		XiData[xi].Comp_F.TryGetValue(skillName, out var value);
		value.Level_Base++;
		if (value.BS_Skill)
		{
			HasBSSkill = true;
		}
		SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(xi, 2, value);
		if ((bool)value.skillbt)
		{
			value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
		}
	}

	private void AddPointCompSon(int xi, string skillName)
	{
		XiData[xi].Comp_S.TryGetValue(skillName, out var value);
		XiData[xi].Comp_F.TryGetValue(value.FatherSkill, out var value2);
		switch (value.FrontSkillType)
		{
		case 2:
			if (value2.Level_Base > 0)
			{
				value.Level_Base++;
				if ((bool)value.skillbt)
				{
					value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
				}
			}
			break;
		case 3:
		{
			XiData[xi].Comp_S.TryGetValue(value.FrontSkill, out var value3);
			if (value2.Level_Base > 0 && value3.Level_Base > 0)
			{
				value.Level_Base++;
				if ((bool)value.skillbt)
				{
					value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
				}
			}
			break;
		}
		}
		SingletonMonoScope<ACTbar>.Instance.AddSkillListSlotCP(xi, 3, value2);
	}

	private void AddPointDotFather(int xi, string skillName)
	{
		XiData[xi].Dot_F.TryGetValue(skillName, out var value);
		value.Level_Base++;
		if ((bool)value.skillbt)
		{
			value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
		}
		SingletonMonoScope<ACTbar>.Instance.SetDot(value);
	}

	private void AddPointDotSon(int xi, string skillName)
	{
		XiData[xi].Dot_S.TryGetValue(skillName, out var value);
		XiData[xi].Dot_F.TryGetValue(value.FatherSkill, out var value2);
		switch (value.FrontSkillType)
		{
		case 4:
			if (value2.Level_Base > 0)
			{
				value.Level_Base++;
				if ((bool)value.skillbt)
				{
					value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
				}
			}
			break;
		case 5:
		{
			XiData[xi].Dot_S.TryGetValue(value.FrontSkill, out var value3);
			if (value2.Level_Base > 0 && value3.Level_Base > 0)
			{
				value.Level_Base++;
				if ((bool)value.skillbt)
				{
					value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
				}
			}
			break;
		}
		}
		SingletonMonoScope<ACTbar>.Instance.SetDot(value2);
	}

	private void AddPointBei(int xi, string skillName)
	{
		XiData[xi].Bei.TryGetValue(skillName, out var value);
		if (value.Level_Base < 1)
		{
			value.Level_Base++;
			if (value.Level_WeaponOn > 0)
			{
				PL.SetSkillBeiBuff(add: true, value.B_Type, value.B_Number, 1 + value.Level_WeaponOn);
			}
			else
			{
				PL.SetSkillBeiBuff(add: true, value.B_Type, value.B_Number, 1);
			}
		}
		else
		{
			value.Level_Base++;
			PL.SetSkillBeiBuff(add: true, value.B_Type, value.B_Number, 1);
		}
		if ((bool)value.skillbt)
		{
			value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
		}
	}

	private void LoadData_Xi(TextAsset csvFile)
	{
		int num = ((XiBT != null) ? XiBT.Length : 0);
		for (int i = 0; i < num; i++)
		{
			if ((bool)XiBT[i])
			{
				XiBT[i].Index = i;
			}
		}
		string[][] array = LoadTextFile(csvFile);
		if (array == null || XiData == null)
		{
			return;
		}
		int num2 = Mathf.Min(XiData.Length, array.Length - 1);
		for (int j = 1; j <= num2; j++)
		{
			if (XiData[j - 1] != null && !IsCsvRowEmpty(array[j]) && array[j].Length >= 6)
			{
				XiData[j - 1].IndexName = array[j][1];
				XiData[j - 1].Used = false;
				XiData[j - 1].Level_Base = int.Parse(array[j][3]);
				XiData[j - 1].damageType = GiveElement(int.Parse(array[j][4]));
				XiData[j - 1].Number = float.Parse(array[j][5], CultureInfo.InvariantCulture);
			}
		}
	}

	private void LoadData_SampleF(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SkillData_Sample_Father skillData_Sample_Father = new SkillData_Sample_Father();
			int num = 1;
			skillData_Sample_Father.IndexName = array[i][1];
			num++;
			skillData_Sample_Father.icon = iconDT[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Sample_Father.iconB = iconDTB[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			num++;
			skillData_Sample_Father.Price = SkillPrice.Price[int.Parse(array[i][num])];
			num++;
			skillData_Sample_Father.UnLock_Point = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Xi = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Level_Max = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Info = array[i][num];
			num++;
			skillData_Sample_Father.SonA = array[i][num];
			num++;
			skillData_Sample_Father.SonB = array[i][num];
			num++;
			skillData_Sample_Father.SonC = array[i][num];
			num++;
			skillData_Sample_Father.SampleSkill = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Sample_Father.BS_Skill = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Sample_Father.LastSkill = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Sample_Father.DashSkill = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Sample_Father.TPSkill = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Sample_Father.UseAni = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.FStype = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.LockType = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Father_type = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.OBJ = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.RTtypeOBJ = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.RTtypeFX = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Distance = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.ManaCost_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.CoolDown_Base = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.damageType = GiveElement(int.Parse(array[i][num]));
			skillData_Sample_Father.MainEL = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.ThroughType = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.AttackType = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Sample_Father.Damage_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Damage_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BJrate_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BJDamage_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.JYrate_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Through_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.FlySpeed_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.MoveSpeedCut_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.AttackSpeedCut_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.AntiCut_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_Damage_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_Damage_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_EL_Damage_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_EL_Damage_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_EL_Chuan_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_EL_Chuan_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_BJrate_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_BJrate_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_JYrate_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_JYrate_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_GeDang_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_GeDang_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_AttackSpeed_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_AttackSpeed_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_MoveSpeed_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_MoveSpeed_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_DamageAnti_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_DamageAnti_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_Health_Prc_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BF_Health_Prc_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_Damage_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_Damage_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_ATspeed_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_ATspeed_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_MVspeed_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_MVspeed_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_Health_Prc_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.C_Health_Prc_Level = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BSAT = array[i][num];
			num++;
			skillData_Sample_Father.BSAT_Count = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BSAT_Angle = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Is_BS = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.ChangeSkin = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.SkinIndex = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Reborn = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.NoTime = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.BuffTime_Base = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.DebuffTime = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Field_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.ORB_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.EXP_time = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.ZD_time_F = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.ZD_time_S = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Layer_SubA = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Layer_SubB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.ORB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.ZD_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.ZD_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.ZD_AB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.EXP_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.EXP_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.EXP_AB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Dic_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Dic_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.FX_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.FX_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Sound = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Count_ORB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Count_ATtarget_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.CF_Count = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Count_F_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Count_S_Base = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Count_AB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.CountMulti = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.CountEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TypeORB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.CF_Type = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Type_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Type_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Type_AB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TypeDIC_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TypeDIC_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TypeEXP_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TypeEXP_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TypeEXP_AB = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Size = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.High = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.JG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.AngleA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.AngleB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Range1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Range2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Range_AT = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.FStime1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.FStime2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Speed1 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Speed2 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Speed3 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Speed4 = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Sample_Father.Follow_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Follow_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.AllChuan_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.AllChuan_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Slow_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.Slow_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.RDSpeed_F = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.RDSpeed_S = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.HasFX = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.S_HasFX = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.A_HasFX = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.colEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.colEXP_A = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.S_colEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.A_colEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TimeEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.TimeEXP_A = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.LastEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.LastEXP_A = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.S_LastEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.A_LastEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.EXPpos = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.EXPpos_A = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.S_EXPpos = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.A_EXPpos = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.AngleEXP = int.Parse(array[i][num]);
			num++;
			skillData_Sample_Father.AngleEXP_A = int.Parse(array[i][num]);
			num++;
			XiData[int.Parse(array[i][5])].Sample_F.Add(skillData_Sample_Father.IndexName, skillData_Sample_Father);
			SKindex sKindex = new SKindex();
			sKindex.Xi = skillData_Sample_Father.Xi;
			sKindex.type = 0;
			SKI.Add(skillData_Sample_Father.IndexName, sKindex);
			AddSkillFW(skillData_Sample_Father, 0, int.Parse(array[i][2]), int.Parse(array[i][3]), int.Parse(array[i][26]));
		}
	}

	private void LoadData_SampleS(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SkillData_Sample_Son skillData_Sample_Son = new SkillData_Sample_Son();
			skillData_Sample_Son.IndexName = array[i][1];
			skillData_Sample_Son.icon = iconDT[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Sample_Son.iconB = iconDTB[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Sample_Son.Price = SkillPrice.Price[int.Parse(array[i][3])];
			skillData_Sample_Son.UnLock_Point = int.Parse(array[i][4]);
			skillData_Sample_Son.Xi = int.Parse(array[i][5]);
			skillData_Sample_Son.Level_Max = int.Parse(array[i][6]);
			skillData_Sample_Son.Info = array[i][7];
			skillData_Sample_Son.FrontSkill = array[i][8];
			skillData_Sample_Son.FrontSkillType = int.Parse(array[i][9]);
			skillData_Sample_Son.FatherSkill = array[i][10];
			skillData_Sample_Son.SonType = int.Parse(array[i][11]);
			skillData_Sample_Son.ManaCost = int.Parse(array[i][12]);
			skillData_Sample_Son.damageType = GiveElement(int.Parse(array[i][13]));
			skillData_Sample_Son.BaseA = int.Parse(array[i][14]);
			skillData_Sample_Son.LevelA = int.Parse(array[i][15]);
			skillData_Sample_Son.BaseB = int.Parse(array[i][16]);
			skillData_Sample_Son.LevelB = int.Parse(array[i][17]);
			skillData_Sample_Son.SubAttackTypeA = int.Parse(array[i][18]);
			skillData_Sample_Son.SubAttackTypeB = int.Parse(array[i][19]);
			skillData_Sample_Son.multiCount_Type = int.Parse(array[i][20]);
			skillData_Sample_Son.Count = int.Parse(array[i][21]);
			XiData[int.Parse(array[i][5])].Sample_S.Add(skillData_Sample_Son.IndexName, skillData_Sample_Son);
			SKindex sKindex = new SKindex();
			sKindex.Xi = skillData_Sample_Son.Xi;
			sKindex.type = 1;
			SKI.Add(skillData_Sample_Son.IndexName, sKindex);
			AddSkillFW(skillData_Sample_Son, 1, int.Parse(array[i][2]), int.Parse(array[i][3]), int.Parse(array[i][13]));
		}
	}

	private void LoadData_CompF(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SkillData_Comp_Father skillData_Comp_Father = new SkillData_Comp_Father();
			int num = 1;
			skillData_Comp_Father.IndexName = array[i][1];
			num++;
			skillData_Comp_Father.icon = iconDT[int.Parse(array[i][5])].icon[int.Parse(array[i][num])];
			skillData_Comp_Father.iconB = iconDTB[int.Parse(array[i][5])].icon[int.Parse(array[i][num])];
			num++;
			skillData_Comp_Father.Price = SkillPrice.Price[int.Parse(array[i][num])];
			num++;
			skillData_Comp_Father.UnLock_Point = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Xi = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Level_Max = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Info = array[i][num];
			num++;
			skillData_Comp_Father.SonA = array[i][num];
			num++;
			skillData_Comp_Father.SonB = array[i][num];
			num++;
			skillData_Comp_Father.SonC = array[i][num];
			num++;
			skillData_Comp_Father.SampleSkill = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Comp_Father.BS_Skill = GiveBool(int.Parse(array[i][num]));
			num++;
			skillData_Comp_Father.UseAni = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.obj = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Distance = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.ManaCost_Base = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.CoolDown_Base = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Damage_Base = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Damage_Level = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Health_Base = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Health_Level = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Health_Prc_Base = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Health_Prc_Level = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Summon_count_Base = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.damageType = GiveElement(int.Parse(array[i][num]));
			num++;
			skillData_Comp_Father.damageType_Change = GiveElement(int.Parse(array[i][num]));
			num++;
			skillData_Comp_Father.ChangeEL_SK = GiveElement(int.Parse(array[i][num]));
			num++;
			skillData_Comp_Father.ChangeEL_AR = GiveElement(int.Parse(array[i][num]));
			num++;
			skillData_Comp_Father.DotMultiA = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.DotMultiB = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.DisA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.DisB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.Idle_Time_Min = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.Idle_Time_Max = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.SO_IdleRate = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.SO_AttackRate = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.SO_SayRate = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.SO_HurtRate = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.SO_DieRate = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.SO_Idle = array[i][num];
			num++;
			skillData_Comp_Father.SO_Walk = array[i][num];
			num++;
			skillData_Comp_Father.SO_AttackA = array[i][num];
			num++;
			skillData_Comp_Father.SO_SayA = array[i][num];
			num++;
			skillData_Comp_Father.SO_AttackB = array[i][num];
			num++;
			skillData_Comp_Father.SO_SayB = array[i][num];
			num++;
			skillData_Comp_Father.SO_AttackC = array[i][num];
			num++;
			skillData_Comp_Father.SO_SayC = array[i][num];
			num++;
			skillData_Comp_Father.SO_Hurt = array[i][num];
			num++;
			skillData_Comp_Father.SO_Die = array[i][num];
			num++;
			skillData_Comp_Father.Type_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Type_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.TypeDIC_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.TypeDIC_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.JG_A = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.JG_B = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.AngleA = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.AngleB = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.FStimeA = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.FStimeB = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Comp_Father.Count_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Count_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Count_ATtarget_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Count_ATtarget_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.CountMulti_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.CountMulti_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Follow_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.Follow_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.AllChuan_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.AllChuan_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.RDSpeed_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.RDSpeed_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.HasFX_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.HasFX_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.colEXP_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.colEXP_B = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.EXPpos_A = int.Parse(array[i][num]);
			num++;
			skillData_Comp_Father.EXPpos_B = int.Parse(array[i][num]);
			XiData[int.Parse(array[i][5])].Comp_F.Add(skillData_Comp_Father.IndexName, skillData_Comp_Father);
			SKindex sKindex = new SKindex();
			sKindex.Xi = skillData_Comp_Father.Xi;
			sKindex.type = 2;
			SKI.Add(skillData_Comp_Father.IndexName, sKindex);
			AddSkillFW(skillData_Comp_Father, 2, int.Parse(array[i][2]), int.Parse(array[i][3]), DamageTypeToFWIndex(skillData_Comp_Father.damageType));
		}
	}

	private void LoadData_CompS(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SkillData_Comp_Son skillData_Comp_Son = new SkillData_Comp_Son();
			skillData_Comp_Son.IndexName = array[i][1];
			skillData_Comp_Son.icon = iconDT[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Comp_Son.iconB = iconDTB[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Comp_Son.Price = SkillPrice.Price[int.Parse(array[i][3])];
			skillData_Comp_Son.UnLock_Point = int.Parse(array[i][4]);
			skillData_Comp_Son.Xi = int.Parse(array[i][5]);
			skillData_Comp_Son.Level_Max = int.Parse(array[i][6]);
			skillData_Comp_Son.Info = array[i][7];
			skillData_Comp_Son.FrontSkill = array[i][8];
			skillData_Comp_Son.FrontSkillType = int.Parse(array[i][9]);
			skillData_Comp_Son.FatherSkill = array[i][10];
			skillData_Comp_Son.ManaCost = int.Parse(array[i][11]);
			skillData_Comp_Son.SonType = int.Parse(array[i][12]);
			skillData_Comp_Son.BaseA = int.Parse(array[i][13]);
			skillData_Comp_Son.LevelA = int.Parse(array[i][14]);
			skillData_Comp_Son.BaseB = int.Parse(array[i][15]);
			skillData_Comp_Son.LevelB = int.Parse(array[i][16]);
			skillData_Comp_Son.Summon_count = int.Parse(array[i][17]);
			skillData_Comp_Son.ChangeEL_SK = GiveElement(int.Parse(array[i][18]));
			skillData_Comp_Son.ChangeEL_AR = GiveElement(int.Parse(array[i][19]));
			XiData[int.Parse(array[i][5])].Comp_S.Add(skillData_Comp_Son.IndexName, skillData_Comp_Son);
			SKindex sKindex = new SKindex();
			sKindex.Xi = skillData_Comp_Son.Xi;
			sKindex.type = 3;
			SKI.Add(skillData_Comp_Son.IndexName, sKindex);
			AddSkillFW(skillData_Comp_Son, 3, int.Parse(array[i][2]), int.Parse(array[i][3]), DamageTypeToFWIndex(skillData_Comp_Son.ChangeEL_SK));
		}
	}

	private void LoadData_DotF(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SkillData_Dot_Father skillData_Dot_Father = new SkillData_Dot_Father();
			int num = 1;
			skillData_Dot_Father.IndexName = array[i][num];
			num++;
			skillData_Dot_Father.icon = iconDT[int.Parse(array[i][5])].icon[int.Parse(array[i][num])];
			skillData_Dot_Father.iconB = iconDTB[int.Parse(array[i][5])].icon[int.Parse(array[i][num])];
			num++;
			skillData_Dot_Father.Price = SkillPrice.Price[int.Parse(array[i][num])];
			num++;
			skillData_Dot_Father.UnLock_Point = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Xi = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Level_Max = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Info = array[i][num];
			num++;
			skillData_Dot_Father.SonA = array[i][num];
			num++;
			skillData_Dot_Father.SonB = array[i][num];
			num++;
			skillData_Dot_Father.SonC = array[i][num];
			num++;
			skillData_Dot_Father.SonD = array[i][num];
			num++;
			skillData_Dot_Father.damageType = GiveElement(int.Parse(array[i][num]));
			num++;
			skillData_Dot_Father.Layer_Base = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.DOTrate_Base = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.DOTrate_Level = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Damage_base = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Time_base = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.ATSpeedCut_Base = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.ATSpeedCut_Level = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.MVSpeedCut_Base = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.MVSpeedCut_Level = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.BoomDie_OBJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.BoomDie_Pos = int.Parse(array[i][num]);
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				skillData_Dot_Father.AttackType_BD = true;
			}
			else
			{
				skillData_Dot_Father.AttackType_BD = false;
			}
			num++;
			skillData_Dot_Father.Type_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.TypeDIC_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.TypeEXP_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Range_BD = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.SpeedMin_BD = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.SpeedMax_BD = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.Count_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.CountMulti_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.BuffTime_BD = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.ZD_time_BD = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.ZD_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.EXP_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Dic_BD = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.BoomJump_OBJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.BoomJump_Pos = int.Parse(array[i][num]);
			num++;
			if (int.Parse(array[i][num]) == 0)
			{
				skillData_Dot_Father.AttackType_BJ = true;
			}
			else
			{
				skillData_Dot_Father.AttackType_BJ = false;
			}
			num++;
			skillData_Dot_Father.Type_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.TypeDIC_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.TypeEXP_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Range_BJ = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.SpeedMin_BJ = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.SpeedMax_BJ = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.Count_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.CountMulti_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.BuffTime_BJ = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.ZD_time_BJ = float.Parse(array[i][num], CultureInfo.InvariantCulture);
			num++;
			skillData_Dot_Father.ZD_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.EXP_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.Dic_BJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.CutJump_OBJ = int.Parse(array[i][num]);
			num++;
			skillData_Dot_Father.CutJump_Pos = int.Parse(array[i][num]);
			XiData[int.Parse(array[i][5])].Dot_F.Add(skillData_Dot_Father.IndexName, skillData_Dot_Father);
			SKindex sKindex = new SKindex();
			sKindex.Xi = skillData_Dot_Father.Xi;
			sKindex.type = 4;
			SKI.Add(skillData_Dot_Father.IndexName, sKindex);
			AddSkillFW(skillData_Dot_Father, 4, int.Parse(array[i][2]), int.Parse(array[i][3]), DamageTypeToFWIndex(skillData_Dot_Father.damageType));
		}
	}

	private void LoadData_DotS(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SkillData_Dot_Son skillData_Dot_Son = new SkillData_Dot_Son();
			skillData_Dot_Son.IndexName = array[i][1];
			skillData_Dot_Son.icon = iconDT[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Dot_Son.iconB = iconDTB[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Dot_Son.Price = SkillPrice.Price[int.Parse(array[i][3])];
			skillData_Dot_Son.UnLock_Point = int.Parse(array[i][4]);
			skillData_Dot_Son.Xi = int.Parse(array[i][5]);
			skillData_Dot_Son.Level_Max = int.Parse(array[i][6]);
			skillData_Dot_Son.Info = array[i][7];
			skillData_Dot_Son.FrontSkill = array[i][8];
			skillData_Dot_Son.FrontSkillType = int.Parse(array[i][9]);
			skillData_Dot_Son.FatherSkill = array[i][10];
			skillData_Dot_Son.damageType = GiveElement(int.Parse(array[i][11]));
			skillData_Dot_Son.SonType = int.Parse(array[i][12]);
			skillData_Dot_Son.BaseA = int.Parse(array[i][13]);
			skillData_Dot_Son.LevelA = int.Parse(array[i][14]);
			skillData_Dot_Son.BaseB = float.Parse(array[i][15], CultureInfo.InvariantCulture);
			skillData_Dot_Son.LevelB = float.Parse(array[i][16], CultureInfo.InvariantCulture);
			skillData_Dot_Son.Layer = int.Parse(array[i][17]);
			XiData[int.Parse(array[i][5])].Dot_S.Add(skillData_Dot_Son.IndexName, skillData_Dot_Son);
			SKindex sKindex = new SKindex();
			sKindex.Xi = skillData_Dot_Son.Xi;
			sKindex.type = 5;
			SKI.Add(skillData_Dot_Son.IndexName, sKindex);
			AddSkillFW(skillData_Dot_Son, 5, int.Parse(array[i][2]), int.Parse(array[i][3]), DamageTypeToFWIndex(skillData_Dot_Son.damageType));
		}
	}

	private void LoadData_Bei(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		for (int i = 1; i < array.Length - 1; i++)
		{
			SkillData_Bei skillData_Bei = new SkillData_Bei();
			skillData_Bei.IndexName = array[i][1];
			skillData_Bei.icon = iconDT[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Bei.iconB = iconDTB[int.Parse(array[i][5])].icon[int.Parse(array[i][2])];
			skillData_Bei.Price = SkillPrice.Price[int.Parse(array[i][3])];
			skillData_Bei.UnLock_Point = int.Parse(array[i][4]);
			skillData_Bei.Xi = int.Parse(array[i][5]);
			skillData_Bei.Level_Max = int.Parse(array[i][6]);
			skillData_Bei.Info = array[i][7];
			skillData_Bei.damageType = GiveElement(int.Parse(array[i][8]));
			skillData_Bei.B_Type = int.Parse(array[i][9]);
			skillData_Bei.B_Number = float.Parse(array[i][10], CultureInfo.InvariantCulture);
			XiData[int.Parse(array[i][5])].Bei.Add(skillData_Bei.IndexName, skillData_Bei);
			SKindex sKindex = new SKindex();
			sKindex.Xi = skillData_Bei.Xi;
			sKindex.type = 6;
			SKI.Add(skillData_Bei.IndexName, sKindex);
		}
	}

	private void LoadData_DF(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		if (array == null)
		{
			return;
		}
		DFData.Clear();
		for (int i = 1; i < array.Length; i++)
		{
			if (IsCsvRowEmpty(array[i]))
			{
				continue;
			}
			if (array[i].Length < 26)
			{
				LogUtil.Error($"DF表第{i + 1}行列数不足，已跳过");
				continue;
			}
			int num = 1;
			SkilDFData skilDFData = new SkilDFData();
			skilDFData.Index = int.Parse(array[i][num]);
			num++;
			skilDFData.SK_Count = int.Parse(array[i][num]);
			num++;
			skilDFData.Unlock_Point = int.Parse(array[i][num]);
			num++;
			skilDFData.Level_Base = 0;
			skilDFData.Level_Max = int.Parse(array[i][num]);
			num++;
			skilDFData.LieA = int.Parse(array[i][num]);
			num++;
			skilDFData.LieB = int.Parse(array[i][num]);
			num++;
			skilDFData.LieC = int.Parse(array[i][num]);
			num++;
			skilDFData.FatherA = int.Parse(array[i][num]);
			num++;
			skilDFData.FatherB = int.Parse(array[i][num]);
			num++;
			skilDFData.FatherC = int.Parse(array[i][num]);
			num++;
			for (int j = 0; j < 3; j++)
			{
				SkilDFData_Lit skilDFData_Lit = new SkilDFData_Lit();
				skilDFData_Lit.IndexName = array[i][num];
				num++;
				skilDFData_Lit.Info = array[i][num];
				num++;
				skilDFData_Lit.Icon = int.Parse(array[i][num]);
				num++;
				skilDFData_Lit.Type = int.Parse(array[i][num]);
				num++;
				skilDFData_Lit.Number = int.Parse(array[i][num]);
				num++;
				skilDFData.SK.Add(skilDFData_Lit);
			}
			DFData.Add(skilDFData);
		}
	}

	private void LoadData_SKC(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		if (array == null)
		{
			return;
		}
		SKC_Data.Clear();
		for (int i = 1; i < array.Length; i++)
		{
			if (!IsCsvRowEmpty(array[i]))
			{
				if (array[i].Length < 18)
				{
					LogUtil.Error($"SKC表第{i + 1}行列数不足，已跳过");
					continue;
				}
				int num = 1;
				SkilChangeData skilChangeData = new SkilChangeData();
				skilChangeData.IndexName = array[i][num];
				num++;
				skilChangeData.GlobleID = int.Parse(array[i][num]);
				num++;
				skilChangeData.OBJ_Group = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_ZD_F = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_ZD_S = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_Dic_F = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_Type_F = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_Type_Dic_F = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_DMG = float.Parse(array[i][num], CultureInfo.InvariantCulture);
				num++;
				skilChangeData.FS_CT_F = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_CT_S = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_CT_AT = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_CT_Multi = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_Time1 = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_Time2 = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_Range1 = int.Parse(array[i][num]);
				num++;
				skilChangeData.FS_AngleA = int.Parse(array[i][num]);
				SKC_Data.Add(skilChangeData);
			}
		}
	}

	private void LoadData_CPC(TextAsset csvFile)
	{
		string[][] array = LoadTextFile(csvFile);
		if (array == null)
		{
			return;
		}
		CPC_Data.Clear();
		for (int i = 1; i < array.Length; i++)
		{
			if (!IsCsvRowEmpty(array[i]))
			{
				if (array[i].Length < 20)
				{
					LogUtil.Error($"CPC表第{i + 1}行列数不足，已跳过");
					continue;
				}
				int num = 1;
				CompSkillChangeData compSkillChangeData = new CompSkillChangeData();
				compSkillChangeData.IndexName = array[i][num];
				num++;
				compSkillChangeData.GlobleID = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.BStype = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_ZD = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_FStype = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_DMG = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_CT = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_CT_AT = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_CT_Multi = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_FStime = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.AT_Angle = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_ZD = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_FStype = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_DMG = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_CT = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_CT_AT = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_CT_Multi = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_FStime = int.Parse(array[i][num]);
				num++;
				compSkillChangeData.SK_Angle = int.Parse(array[i][num]);
				CPC_Data[compSkillChangeData.GlobleID] = compSkillChangeData;
			}
		}
	}

	private void RefreshAllSkillButtons()
	{
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData == null)
			{
				continue;
			}
			foreach (SkillData_Sample_Father value in skillXiData.Sample_F.Values)
			{
				if ((bool)value.skillbt)
				{
					value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
				}
			}
			foreach (SkillData_Sample_Son value2 in skillXiData.Sample_S.Values)
			{
				if ((bool)value2.skillbt)
				{
					value2.skillbt.Refresh(value2.Level_Base, value2.Level_Max, value2.Level_WeaponOn);
				}
			}
			foreach (SkillData_Comp_Father value3 in skillXiData.Comp_F.Values)
			{
				if ((bool)value3.skillbt)
				{
					value3.skillbt.Refresh(value3.Level_Base, value3.Level_Max, value3.Level_WeaponOn);
				}
			}
			foreach (SkillData_Comp_Son value4 in skillXiData.Comp_S.Values)
			{
				if ((bool)value4.skillbt)
				{
					value4.skillbt.Refresh(value4.Level_Base, value4.Level_Max, value4.Level_WeaponOn);
				}
			}
			foreach (SkillData_Dot_Father value5 in skillXiData.Dot_F.Values)
			{
				if ((bool)value5.skillbt)
				{
					value5.skillbt.Refresh(value5.Level_Base, value5.Level_Max, value5.Level_WeaponOn);
				}
			}
			foreach (SkillData_Dot_Son value6 in skillXiData.Dot_S.Values)
			{
				if ((bool)value6.skillbt)
				{
					value6.skillbt.Refresh(value6.Level_Base, value6.Level_Max, value6.Level_WeaponOn);
				}
			}
			foreach (SkillData_Bei value7 in skillXiData.Bei.Values)
			{
				if ((bool)value7.skillbt)
				{
					value7.skillbt.Refresh(value7.Level_Base, value7.Level_Max, value7.Level_WeaponOn);
				}
			}
		}
	}

	private void RefreshPassiveUnlockStateAllXi()
	{
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData == null)
			{
				continue;
			}
			foreach (KeyValuePair<string, SkillData_Bei> item in skillXiData.Bei)
			{
				SkillData_Bei value = item.Value;
				if ((bool)value?.skillbt)
				{
					if (P_Used > 0 && value.UnLock_Point <= skillXiData.Level_Base)
					{
						value.skillbt.SkillTU.sprite = value.icon;
						value.skillbt.Unlock = true;
					}
					else
					{
						value.skillbt.SkillTU.sprite = value.iconB;
						value.skillbt.Unlock = false;
					}
				}
			}
		}
	}

	private void RecalcHasBSSkill()
	{
		HasBSSkill = false;
		BSSkillButton = null;
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData == null)
			{
				continue;
			}
			foreach (SkillData_Sample_Father value in skillXiData.Sample_F.Values)
			{
				if (value.BS_Skill && value.Level_Base > 0)
				{
					HasBSSkill = true;
					BSSkillButton = value.skillbt;
					return;
				}
			}
			foreach (SkillData_Comp_Father value2 in skillXiData.Comp_F.Values)
			{
				if (value2.BS_Skill && value2.Level_Base > 0)
				{
					HasBSSkill = true;
					BSSkillButton = value2.skillbt;
					return;
				}
			}
		}
	}

	private IEnumerable<ISkillLevelData> EnumerateAllSkills()
	{
		SkillXiData[] xiData = XiData;
		SkillXiData[] array = xiData;
		foreach (SkillXiData xi in array)
		{
			if (xi == null)
			{
				continue;
			}
			foreach (SkillData_Sample_Father value in xi.Sample_F.Values)
			{
				yield return value;
			}
			foreach (SkillData_Sample_Son value2 in xi.Sample_S.Values)
			{
				yield return value2;
			}
			foreach (SkillData_Comp_Father value3 in xi.Comp_F.Values)
			{
				yield return value3;
			}
			foreach (SkillData_Comp_Son value4 in xi.Comp_S.Values)
			{
				yield return value4;
			}
			foreach (SkillData_Dot_Father value5 in xi.Dot_F.Values)
			{
				yield return value5;
			}
			foreach (SkillData_Dot_Son value6 in xi.Dot_S.Values)
			{
				yield return value6;
			}
			foreach (SkillData_Bei value7 in xi.Bei.Values)
			{
				yield return value7;
			}
		}
	}

	private int CalculateUsedDFPoints()
	{
		int num = 0;
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null)
			{
				num += dFDatum.Level_Base;
			}
		}
		return num;
	}

	private static Dictionary<string, SkillSaveData> CloneSkillDict(Dictionary<string, SkillSaveData> src)
	{
		Dictionary<string, SkillSaveData> dictionary = new Dictionary<string, SkillSaveData>();
		if (src == null)
		{
			return dictionary;
		}
		foreach (KeyValuePair<string, SkillSaveData> item in src)
		{
			SkillSaveData value = item.Value;
			dictionary[item.Key] = ((value == null) ? new SkillSaveData() : new SkillSaveData
			{
				Level_Base = value.Level_Base,
				Level_WeaponOn = 0,
				SelectedIndex = value.SelectedIndex
			});
		}
		return dictionary;
	}

	private static Dictionary<string, XiSaveData> CloneXiDict(Dictionary<string, XiSaveData> src)
	{
		Dictionary<string, XiSaveData> dictionary = new Dictionary<string, XiSaveData>();
		if (src == null)
		{
			return dictionary;
		}
		foreach (KeyValuePair<string, XiSaveData> item in src)
		{
			XiSaveData value = item.Value;
			dictionary[item.Key] = ((value == null) ? new XiSaveData() : new XiSaveData
			{
				Level_Base = value.Level_Base
			});
		}
		return dictionary;
	}

	public static bool GiveBool(int a)
	{
		return a switch
		{
			0 => true, 
			1 => false, 
			_ => false, 
		};
	}

	public static DamageType GiveElement(int a)
	{
		return a switch
		{
			0 => DamageType.fire, 
			1 => DamageType.frozen, 
			2 => DamageType.thunder, 
			3 => DamageType.poison, 
			4 => DamageType.physics, 
			5 => DamageType.shadow, 
			_ => DamageType.fire, 
		};
	}

	private static string[][] LoadTextFile(TextAsset textFile)
	{
		if (textFile != null)
		{
			string[] array = textFile.text.Split('\n');
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Split(',');
			}
			return array2.ToArray();
		}
		return null;
	}

	private static bool IsCsvRowEmpty(string[] row)
	{
		if (row == null)
		{
			return true;
		}
		for (int i = 0; i < row.Length; i++)
		{
			if (!string.IsNullOrWhiteSpace(row[i]))
			{
				return false;
			}
		}
		return true;
	}

	public void EnsureSkillFWLibrary()
	{
		if (!_talentTablesLoaded)
		{
			LoadTalentTables();
		}
		else if (FW == null || FW.Char == null || FW.Char.Length == 0 || FW.Char[0] == null)
		{
			InitSkillFWLibrary();
		}
	}

	public bool TryGetSkillFWPlayerType(string skillName, out int plType)
	{
		plType = -1;
		if (string.IsNullOrWhiteSpace(skillName))
		{
			return false;
		}
		EnsureSkillFWLibrary();
		if (FW?.Char == null)
		{
			return false;
		}
		for (int i = 0; i < FW.Char.Length; i++)
		{
			SKFW_Char sKFW_Char = FW.Char[i];
			if (sKFW_Char?.Xi == null)
			{
				continue;
			}
			for (int j = 0; j < sKFW_Char.Xi.Length; j++)
			{
				SKFW_Xi sKFW_Xi = sKFW_Char.Xi[j];
				if (sKFW_Xi?.FW == null)
				{
					continue;
				}
				for (int k = 0; k < sKFW_Xi.FW.Length; k++)
				{
					if (sKFW_Xi.FW[k] != null && sKFW_Xi.FW[k].SkillName == skillName)
					{
						plType = i;
						return true;
					}
				}
			}
		}
		return false;
	}

	private void InitSkillFWLibrary()
	{
		FW = new SKFW_Group
		{
			Char = new SKFW_Char[4]
		};
		for (int i = 0; i < FW.Char.Length; i++)
		{
			FW.Char[i] = new SKFW_Char
			{
				Xi = new SKFW_Xi[3]
			};
			for (int j = 0; j < FW.Char[i].Xi.Length; j++)
			{
				FW.Char[i].Xi[j] = new SKFW_Xi
				{
					FW = new SKFW[0]
				};
			}
		}
	}

	private void AddSkillFW(SkillData skill, int skillType, int skillIndex, int priceIndex, int el)
	{
		if (skill == null)
		{
			return;
		}
		InitSkillFWLibraryIfNeeded();
		int num = skill.Xi / 3;
		int num2 = skill.Xi % 3;
		if (num < 0 || num >= FW.Char.Length || num2 < 0 || num2 >= FW.Char[num].Xi.Length)
		{
			return;
		}
		SKFW_Xi sKFW_Xi = FW.Char[num].Xi[num2];
		if (sKFW_Xi.FW == null)
		{
			sKFW_Xi.FW = new SKFW[0];
		}
		for (int i = 0; i < sKFW_Xi.FW.Length; i++)
		{
			if (sKFW_Xi.FW[i] != null && sKFW_Xi.FW[i].SkillName == skill.IndexName)
			{
				return;
			}
		}
		Array.Resize(ref sKFW_Xi.FW, sKFW_Xi.FW.Length + 1);
		sKFW_Xi.FW[sKFW_Xi.FW.Length - 1] = CreateSkillFW(skill, skillType, skillIndex, priceIndex, el, num, num2);
	}

	private void InitSkillFWLibraryIfNeeded()
	{
		if (FW == null || FW.Char == null || FW.Char.Length != 4)
		{
			InitSkillFWLibrary();
		}
	}

	private static SKFW CreateSkillFW(SkillData skill, int skillType, int skillIndex, int priceIndex, int el, int charIndex, int xiIndex)
	{
		return new SKFW
		{
			PLtype = charIndex,
			Xi = xiIndex,
			Price = priceIndex,
			type = skillType,
			EL = el,
			index = skillIndex,
			SkillName = skill.IndexName
		};
	}

	private static int DamageTypeToFWIndex(DamageType type)
	{
		return type switch
		{
			DamageType.fire => 0, 
			DamageType.frozen => 1, 
			DamageType.thunder => 2, 
			DamageType.poison => 3, 
			DamageType.physics => 4, 
			DamageType.shadow => 5, 
			_ => 0, 
		};
	}

	private void SafeLoadTalentTable(string tableName, TextAsset table, Action<TextAsset> loader)
	{
		if (!table)
		{
			LogUtil.Error("天赋表未配置: " + tableName);
			return;
		}
		try
		{
			loader(table);
		}
		catch (Exception arg)
		{
			LogUtil.Error($"读取天赋表失败: {tableName}\n{arg}");
		}
	}

	private TextAsset GetSkillTable(int index)
	{
		if (skillTA == null || index < 0 || index >= skillTA.Length)
		{
			return null;
		}
		return skillTA[index];
	}

	public float GetManaSample(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0)
		{
			num += value2.ManaCost;
		}
		if (value3.Level_Base > 0)
		{
			num += value3.ManaCost;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0)
			{
				num += value4.ManaCost;
			}
		}
		return num;
	}

	public float GetManaComp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0)
		{
			num += value2.ManaCost;
		}
		if (value3.Level_Base > 0)
		{
			num += value3.ManaCost;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0)
			{
				num += value4.ManaCost;
			}
		}
		return num;
	}

	public int GetCount_father(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		int num = 0;
		if (value2.Level_Base > 0 && value2.SonType == 22 && value2.multiCount_Type == 0)
		{
			num += value2.Count_Last;
		}
		if (value3.Level_Base > 0 && value3.SonType == 22 && value3.multiCount_Type == 0)
		{
			num += value3.Count_Last;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 22 && value4.multiCount_Type == 0)
			{
				num += value4.Count_Last;
			}
		}
		return num;
	}

	public int GetCount_son(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		int num = 0;
		if (value2.Level_Base > 0 && value2.SonType == 22 && value2.multiCount_Type == 1)
		{
			num += value2.Count_Last;
		}
		if (value3.Level_Base > 0 && value3.SonType == 22 && value3.multiCount_Type == 1)
		{
			num += value3.Count_Last;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 22 && value4.multiCount_Type == 1)
			{
				num += value4.Count_Last;
			}
		}
		return num;
	}

	public int GetCount_AtTarget(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		int num = 0;
		if (value2.Level_Base > 0 && value2.SonType == 22 && value2.multiCount_Type == 2)
		{
			num += value2.Count_Last;
		}
		if (value3.Level_Base > 0 && value3.SonType == 22 && value3.multiCount_Type == 2)
		{
			num += value3.Count_Last;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 22 && value4.multiCount_Type == 2)
			{
				num += value4.Count_Last;
			}
		}
		return num;
	}

	public float GetCD_Sample(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 0)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 0)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 0)
			{
				num += value4.LastA;
			}
		}
		if (num > 80f)
		{
			return 80f;
		}
		return num;
	}

	public float GetCD_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 6)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 6)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 6)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBJrate(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 1)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 1)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 1)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetJYrate(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 2)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 2)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 2)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetThrough(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 3)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 3)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 3)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetMVspeedCut(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 4)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 4)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 4)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetATspeedCut(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 4)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 4)
		{
			num += value3.LastB;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 4)
			{
				num += value4.LastB;
			}
		}
		return num;
	}

	public float GetAntiCut(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 5)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 5)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 5)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBuffTime(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 6)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 6)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 6)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_Damage(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 7)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 7)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 7)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_EL_Damage(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 8)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 8)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 8)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_EL_Chuan(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 9)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 9)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 9)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_BJrate(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 10)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 10)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 10)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_JYrate(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 11)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 11)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 11)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_GeDang(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 12)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 12)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 12)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_AttackSpeed(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 13)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 13)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 13)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_MoveSpeed(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 14)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 14)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 14)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_DamageCut(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 15)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 15)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 15)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetBF_Health_Prc(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 16)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 16)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 16)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetCompDamage(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 17)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 17)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 17)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetCompAttackSpeed(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 18)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 18)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 18)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetCompMoveSpeed(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 19)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 19)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 19)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetCompHealth_Prc(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 20)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 20)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 20)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetCF_Rate(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 21)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 21)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 21)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public bool GetSubAttackTypeA(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		int num = 0;
		if (value2.Level_Base > 0 && value2.SonType == 23 && value2.SubAttackTypeA == 1)
		{
			num++;
		}
		if (value3.Level_Base > 0 && value3.SonType == 23 && value3.SubAttackTypeA == 1)
		{
			num++;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 23 && value4.SubAttackTypeA == 1)
			{
				num++;
			}
		}
		if (num > 0)
		{
			return false;
		}
		return true;
	}

	public bool GetSubAttackTypeB(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		int num = 0;
		if (value2.Level_Base > 0 && value2.SonType == 24 && value2.SubAttackTypeB == 1)
		{
			num++;
		}
		if (value3.Level_Base > 0 && value3.SonType == 24 && value3.SubAttackTypeB == 1)
		{
			num++;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 24 && value4.SubAttackTypeB == 1)
			{
				num++;
			}
		}
		if (num > 0)
		{
			return false;
		}
		return true;
	}

	public float GetSub_DamageA(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 23)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 23)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 23)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetSub_DamageB(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 24)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 24)
		{
			num += value3.LastB;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 24)
			{
				num += value4.LastB;
			}
		}
		return num;
	}

	public float GetBSAT_Damage(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 25)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 25)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 25)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float Get_BJDamage(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 30)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 30)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 30)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetFlySpeed(int xi, string a)
	{
		XiData[xi].Sample_F.TryGetValue(a, out var value);
		XiData[xi].Sample_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Sample_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 31)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 31)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Sample_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 31)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetAttackSpeed_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 0)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 0)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 0)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetGeDang_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 1)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 1)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 1)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public int GetSummon_count_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		int num = 0;
		if (value2.Level_Base > 0 && value2.SonType == 2)
		{
			num += value2.Summon_count * value2.Level_Base_Last;
		}
		if (value3.Level_Base > 0 && value3.SonType == 2)
		{
			num += value3.Summon_count * value3.Level_Base_Last;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 2)
			{
				num += value4.Summon_count * value4.Level_Base_Last;
			}
		}
		return num;
	}

	public float GetChange_AT_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 3)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 3)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 3)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetATSrate_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 4)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 4)
		{
			num += value3.LastA;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 4)
			{
				num += value4.LastA;
			}
		}
		return num;
	}

	public float GetATS_Damage_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 4)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 4)
		{
			num += value3.LastB;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 4)
			{
				num += value4.LastB;
			}
		}
		return num;
	}

	public float GetARS_Damage_Comp(int xi, string a)
	{
		XiData[xi].Comp_F.TryGetValue(a, out var value);
		XiData[xi].Comp_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Comp_S.TryGetValue(value.SonB, out var value3);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 5)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 5)
		{
			num += value3.LastB;
		}
		if (value.SonC != "0")
		{
			XiData[xi].Comp_S.TryGetValue(value.SonC, out var value4);
			if (value4.Level_Base > 0 && value4.SonType == 5)
			{
				num += value4.LastB;
			}
		}
		return num;
	}

	public float GetDamage_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 0)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 0)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 0)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 0)
		{
			num += value5.LastA;
		}
		return num;
	}

	public int GetLayer_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		int num = 0;
		if (value2.Level_Base > 0 && value2.SonType == 1)
		{
			num += value2.Layer * value2.Level_Base_Last;
		}
		if (value3.Level_Base > 0 && value3.SonType == 1)
		{
			num += value3.Layer * value3.Level_Base_Last;
		}
		if (value4.Level_Base > 0 && value4.SonType == 1)
		{
			num += value4.Layer * value4.Level_Base_Last;
		}
		if (value5.Level_Base > 0 && value5.SonType == 1)
		{
			num += value5.Layer * value5.Level_Base_Last;
		}
		return num;
	}

	public float GetTime_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 2)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 2)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 2)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 2)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetELAntiCut_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 3)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 3)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 3)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 3)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetYunCut_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 4)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 4)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 4)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 4)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetDamageLow_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 5)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 5)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 5)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 5)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetMSrate_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 6)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 6)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 6)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 6)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetMSnumber_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 6)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 6)
		{
			num += value3.LastB;
		}
		if (value4.Level_Base > 0 && value4.SonType == 6)
		{
			num += value4.LastB;
		}
		if (value5.Level_Base > 0 && value5.SonType == 6)
		{
			num += value5.LastB;
		}
		return num;
	}

	public float GetBoomDie_Rate_Dot(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 7)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 7)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 7)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 7)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetBoomDie_Damage(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 7)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 7)
		{
			num += value3.LastB;
		}
		if (value4.Level_Base > 0 && value4.SonType == 7)
		{
			num += value4.LastB;
		}
		if (value5.Level_Base > 0 && value5.SonType == 7)
		{
			num += value5.LastB;
		}
		return num;
	}

	public float GetBoomJump_Rate(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 8)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 8)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 8)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 8)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetBoomJump_Damage(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 8)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 8)
		{
			num += value3.LastB;
		}
		if (value4.Level_Base > 0 && value4.SonType == 8)
		{
			num += value4.LastB;
		}
		if (value5.Level_Base > 0 && value5.SonType == 8)
		{
			num += value5.LastB;
		}
		return num;
	}

	public float GetCutJump_Rate(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 9)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 9)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 9)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 9)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetCutJump_Damage(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 9)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 9)
		{
			num += value3.LastB;
		}
		if (value4.Level_Base > 0 && value4.SonType == 9)
		{
			num += value4.LastB;
		}
		if (value5.Level_Base > 0 && value5.SonType == 9)
		{
			num += value5.LastB;
		}
		return num;
	}

	public float GetFrozenJump_Rate(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 10)
		{
			num += value2.LastA;
		}
		if (value3.Level_Base > 0 && value3.SonType == 10)
		{
			num += value3.LastA;
		}
		if (value4.Level_Base > 0 && value4.SonType == 10)
		{
			num += value4.LastA;
		}
		if (value5.Level_Base > 0 && value5.SonType == 10)
		{
			num += value5.LastA;
		}
		return num;
	}

	public float GetFrozenJump_Time(int xi, string a)
	{
		XiData[xi].Dot_F.TryGetValue(a, out var value);
		XiData[xi].Dot_S.TryGetValue(value.SonA, out var value2);
		XiData[xi].Dot_S.TryGetValue(value.SonB, out var value3);
		XiData[xi].Dot_S.TryGetValue(value.SonC, out var value4);
		XiData[xi].Dot_S.TryGetValue(value.SonD, out var value5);
		float num = 0f;
		if (value2.Level_Base > 0 && value2.SonType == 10)
		{
			num += value2.LastB;
		}
		if (value3.Level_Base > 0 && value3.SonType == 10)
		{
			num += value3.LastB;
		}
		if (value4.Level_Base > 0 && value4.SonType == 10)
		{
			num += value4.LastB;
		}
		if (value5.Level_Base > 0 && value5.SonType == 10)
		{
			num += value5.LastB;
		}
		return num;
	}

	private void OnEnable()
	{
		LOC.MM.OnLanguageChanged += OnLanguageChanged;
		RefreshPointText();
	}

	private void OnDisable()
	{
		LOC.MM.OnLanguageChanged -= OnLanguageChanged;
	}

	private void OnLanguageChanged(LanguageType lang)
	{
		RefreshPointText();
	}

	private void RefreshPointText()
	{
		if ((bool)pointText)
		{
			pointText.text = string.Format("{0}：{1}", LOC.MM.GetMain("Remaining skill points"), P_Have);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
		LoadTalentTables();
	}

	private void LoadTalentTables()
	{
		if (!_talentTablesLoaded)
		{
			InitSkillFWLibrary();
			SafeLoadTalentTable("Xi", XiTA, LoadData_Xi);
			SafeLoadTalentTable("0 SampleF", GetSkillTable(0), LoadData_SampleF);
			SafeLoadTalentTable("1 SampleS", GetSkillTable(1), LoadData_SampleS);
			SafeLoadTalentTable("2 CompF", GetSkillTable(2), LoadData_CompF);
			SafeLoadTalentTable("3 CompS", GetSkillTable(3), LoadData_CompS);
			SafeLoadTalentTable("4 DotF", GetSkillTable(4), LoadData_DotF);
			SafeLoadTalentTable("5 DotS", GetSkillTable(5), LoadData_DotS);
			SafeLoadTalentTable("6 Bei", GetSkillTable(6), LoadData_Bei);
			if (skillTA != null && skillTA.Length > 7 && (bool)skillTA[7])
			{
				SafeLoadTalentTable("7 DF", skillTA[7], LoadData_DF);
			}
			if (skillTA != null && skillTA.Length > 8 && (bool)skillTA[8])
			{
				SafeLoadTalentTable("8 SKC", skillTA[8], LoadData_SKC);
			}
			if (skillTA != null && skillTA.Length > 9 && (bool)skillTA[9])
			{
				SafeLoadTalentTable("9 CP Change", skillTA[9], LoadData_CPC);
			}
			_talentTablesLoaded = true;
		}
	}

	public void EnsureTalentTablesLoaded()
	{
		LoadTalentTables();
	}

	private void Start()
	{
		try
		{
			PoedbSkillInjector.TryInjectData(this);
		}
		catch (Exception ex)
		{
			LogUtil.Error("[TalentManager.Start] PoedbSkillInjector 数据注入异常: " + ex);
		}
		try
		{
			if (IsTalentDataReady)
			{
				Refresh(6);
			}
		}
		catch
		{
		}
		this.wait(1E-05f, delegate
		{
			SetStart(0);
		});
	}

	public void SetStart(int a)
	{
		IsTalentDataReady = false;
		RefreshPointText();
		HasBSSkill = false;
		SetXiBuff();
		switch (PL.PLType)
		{
		case 0:
			XiCAV[0].alpha = 1f;
			XiCAV[0].blocksRaycasts = true;
			XiCAV[1].alpha = 0f;
			XiCAV[1].blocksRaycasts = false;
			XiCAV[2].alpha = 0f;
			XiCAV[2].blocksRaycasts = false;
			XiBT[0].SetOpen(1);
			XiBT[1].SetOpen(0);
			XiBT[2].SetOpen(0);
			XiData[0].Used = true;
			XiData[1].Used = true;
			XiData[2].Used = true;
			XiCAV[3].gameObject.SetActive(value: false);
			XiCAV[4].gameObject.SetActive(value: false);
			XiCAV[5].gameObject.SetActive(value: false);
			XiCAV[6].gameObject.SetActive(value: false);
			XiCAV[7].gameObject.SetActive(value: false);
			XiCAV[8].gameObject.SetActive(value: false);
			XiCAV[9].gameObject.SetActive(value: false);
			XiCAV[10].gameObject.SetActive(value: false);
			XiCAV[11].gameObject.SetActive(value: false);
			XiBT[3].gameObject.SetActive(value: false);
			XiBT[4].gameObject.SetActive(value: false);
			XiBT[5].gameObject.SetActive(value: false);
			XiBT[6].gameObject.SetActive(value: false);
			XiBT[7].gameObject.SetActive(value: false);
			XiBT[8].gameObject.SetActive(value: false);
			XiBT[9].gameObject.SetActive(value: false);
			XiBT[10].gameObject.SetActive(value: false);
			XiBT[11].gameObject.SetActive(value: false);
			break;
		case 1:
			XiCAV[3].alpha = 1f;
			XiCAV[3].blocksRaycasts = true;
			XiCAV[4].alpha = 0f;
			XiCAV[4].blocksRaycasts = false;
			XiCAV[5].alpha = 0f;
			XiCAV[5].blocksRaycasts = false;
			XiBT[3].SetOpen(1);
			XiBT[4].SetOpen(0);
			XiBT[5].SetOpen(0);
			XiData[3].Used = true;
			XiData[4].Used = true;
			XiData[5].Used = true;
			XiCAV[0].gameObject.SetActive(value: false);
			XiCAV[1].gameObject.SetActive(value: false);
			XiCAV[2].gameObject.SetActive(value: false);
			XiCAV[6].gameObject.SetActive(value: false);
			XiCAV[7].gameObject.SetActive(value: false);
			XiCAV[8].gameObject.SetActive(value: false);
			XiCAV[9].gameObject.SetActive(value: false);
			XiCAV[10].gameObject.SetActive(value: false);
			XiCAV[11].gameObject.SetActive(value: false);
			XiBT[0].gameObject.SetActive(value: false);
			XiBT[1].gameObject.SetActive(value: false);
			XiBT[2].gameObject.SetActive(value: false);
			XiBT[6].gameObject.SetActive(value: false);
			XiBT[7].gameObject.SetActive(value: false);
			XiBT[8].gameObject.SetActive(value: false);
			XiBT[9].gameObject.SetActive(value: false);
			XiBT[10].gameObject.SetActive(value: false);
			XiBT[11].gameObject.SetActive(value: false);
			break;
		case 2:
			XiCAV[6].alpha = 1f;
			XiCAV[6].blocksRaycasts = true;
			XiCAV[7].alpha = 0f;
			XiCAV[7].blocksRaycasts = false;
			XiCAV[8].alpha = 0f;
			XiCAV[8].blocksRaycasts = false;
			XiBT[6].SetOpen(1);
			XiBT[7].SetOpen(0);
			XiBT[8].SetOpen(0);
			XiData[6].Used = true;
			XiData[7].Used = true;
			XiData[8].Used = true;
			XiCAV[0].gameObject.SetActive(value: false);
			XiCAV[1].gameObject.SetActive(value: false);
			XiCAV[2].gameObject.SetActive(value: false);
			XiCAV[3].gameObject.SetActive(value: false);
			XiCAV[4].gameObject.SetActive(value: false);
			XiCAV[5].gameObject.SetActive(value: false);
			XiCAV[9].gameObject.SetActive(value: false);
			XiCAV[10].gameObject.SetActive(value: false);
			XiCAV[11].gameObject.SetActive(value: false);
			XiBT[0].gameObject.SetActive(value: false);
			XiBT[1].gameObject.SetActive(value: false);
			XiBT[2].gameObject.SetActive(value: false);
			XiBT[3].gameObject.SetActive(value: false);
			XiBT[4].gameObject.SetActive(value: false);
			XiBT[5].gameObject.SetActive(value: false);
			XiBT[9].gameObject.SetActive(value: false);
			XiBT[10].gameObject.SetActive(value: false);
			XiBT[11].gameObject.SetActive(value: false);
			break;
		case 3:
			XiCAV[9].alpha = 1f;
			XiCAV[9].blocksRaycasts = true;
			XiCAV[10].alpha = 0f;
			XiCAV[10].blocksRaycasts = false;
			XiCAV[11].alpha = 0f;
			XiCAV[11].blocksRaycasts = false;
			XiBT[9].SetOpen(1);
			XiBT[10].SetOpen(0);
			XiBT[11].SetOpen(0);
			XiData[9].Used = true;
			XiData[10].Used = true;
			XiData[11].Used = true;
			XiCAV[0].gameObject.SetActive(value: false);
			XiCAV[1].gameObject.SetActive(value: false);
			XiCAV[2].gameObject.SetActive(value: false);
			XiCAV[3].gameObject.SetActive(value: false);
			XiCAV[4].gameObject.SetActive(value: false);
			XiCAV[5].gameObject.SetActive(value: false);
			XiCAV[6].gameObject.SetActive(value: false);
			XiCAV[7].gameObject.SetActive(value: false);
			XiCAV[8].gameObject.SetActive(value: false);
			XiBT[0].gameObject.SetActive(value: false);
			XiBT[1].gameObject.SetActive(value: false);
			XiBT[2].gameObject.SetActive(value: false);
			XiBT[3].gameObject.SetActive(value: false);
			XiBT[4].gameObject.SetActive(value: false);
			XiBT[5].gameObject.SetActive(value: false);
			XiBT[6].gameObject.SetActive(value: false);
			XiBT[7].gameObject.SetActive(value: false);
			XiBT[8].gameObject.SetActive(value: false);
			break;
		}
		SetupCommonDFTreeUI();
		IsTalentDataReady = true;
		RebindAllSkillBT();
		for (int i = 0; i < XiData.Length; i++)
		{
			if (XiData[i] != null)
			{
				Refresh(i);
			}
		}
		RefreshAllSkillButtons();
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.ClearBeforeRebuildSkillList();
		}
		RebuildActbarFromLevels();
		RecalcHasBSSkill();
		RefreshPointText();
		RefreshDF();
	}

	private void SetupCommonDFTreeUI()
	{
		if ((bool)DFXiCAV && (bool)DFXiBT)
		{
			DFXiCAV.gameObject.SetActive(value: true);
			DFXiCAV.alpha = 0f;
			DFXiCAV.blocksRaycasts = false;
			DFXiBT.gameObject.SetActive(value: true);
			DFXiBT.SetOpen(0);
		}
	}

	private bool CanShowTalentPage(int index)
	{
		if (XiData != null && index >= 0 && index < XiData.Length && XiData[index] != null)
		{
			return XiData[index].Used;
		}
		return false;
	}

	private bool CanShowDFTalentPage()
	{
		if ((bool)DFXiCAV && (bool)DFXiBT)
		{
			return IsDFTalentUnlockedByPlayerLevel();
		}
		return false;
	}

	public void ChangePage(int index)
	{
		if (CanShowTalentPage(index))
		{
			ChangeTalentPage(index, isDF: false);
		}
	}

	public void ChangeDFPage()
	{
		if (!CanShowDFTalentPage())
		{
			ShowDFTalentLevelLockedTip();
		}
		else
		{
			ChangeTalentPage(-1, isDF: true);
		}
	}

	private void ChangeTalentPage(int xiIndex, bool isDF)
	{
		TryGetCurrentTalentPage(out var xiIndex2, out var isDF2);
		if (isDF2 == isDF && (isDF || xiIndex2 == xiIndex))
		{
			return;
		}
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideDFSkillListForPageChange();
		}
		RuntimeManager.PlayOneShot(_audioManager.audioData.Xi_Select);
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.CloseSkillListUI();
		}
		int num = Mathf.Min((XiCAV != null) ? XiCAV.Length : 0, (XiBT != null) ? XiBT.Length : 0);
		for (int i = 0; i < num; i++)
		{
			if ((bool)XiCAV[i] && (bool)XiBT[i] && XiCAV[i].gameObject.activeSelf)
			{
				bool flag = !isDF && i == xiIndex;
				XiCAV[i].alpha = (flag ? 1 : 0);
				XiCAV[i].blocksRaycasts = flag;
				XiBT[i].SetOpen(flag ? 1 : 0);
			}
		}
		if ((bool)DFXiCAV && (bool)DFXiBT)
		{
			DFXiCAV.gameObject.SetActive(value: true);
			DFXiCAV.alpha = (isDF ? 1 : 0);
			DFXiCAV.blocksRaycasts = isDF;
			DFXiBT.gameObject.SetActive(value: true);
			DFXiBT.SetOpen(isDF ? 1 : 0);
		}
	}

	public bool ChangePageByShortcut(bool left)
	{
		List<TalentPageReference> availableShortcutTalentPages = GetAvailableShortcutTalentPages();
		if (availableShortcutTalentPages.Count <= 0)
		{
			return false;
		}
		TryGetCurrentTalentPage(out var xiIndex, out var isDF);
		int num = -1;
		for (int i = 0; i < availableShortcutTalentPages.Count; i++)
		{
			if (availableShortcutTalentPages[i].IsDF == isDF && (availableShortcutTalentPages[i].IsDF || availableShortcutTalentPages[i].XiIndex == xiIndex))
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			num = 0;
		}
		int num2 = num + ((!left) ? 1 : (-1));
		if (num2 < 0 || num2 >= availableShortcutTalentPages.Count)
		{
			return false;
		}
		TalentPageReference talentPageReference = availableShortcutTalentPages[num2];
		if (talentPageReference.IsDF)
		{
			ChangeDFPage();
		}
		else
		{
			ChangePage(talentPageReference.XiIndex);
		}
		return true;
	}

	private List<TalentPageReference> GetAvailableShortcutTalentPages()
	{
		List<TalentPageReference> list = new List<TalentPageReference>();
		if (PL == null)
		{
			return list;
		}
		int num = ((XiData != null) ? XiData.Length : 0);
		int num2 = Mathf.Clamp(PL.PLType * 3, 0, num);
		for (int i = num2; i < num2 + 3 && i < num; i++)
		{
			if (CanShowTalentPage(i))
			{
				list.Add(new TalentPageReference(i, isDF: false));
			}
		}
		if (CanShowDFTalentPage())
		{
			list.Add(new TalentPageReference(-1, isDF: true));
		}
		return list;
	}

	private bool TryGetCurrentTalentPage(out int xiIndex, out bool isDF)
	{
		xiIndex = -1;
		isDF = false;
		if ((bool)DFXiCAV && DFXiCAV.gameObject.activeSelf && DFXiCAV.blocksRaycasts && DFXiCAV.alpha > 0.5f)
		{
			isDF = true;
			return true;
		}
		if (XiCAV == null)
		{
			return false;
		}
		for (int i = 0; i < XiCAV.Length; i++)
		{
			CanvasGroup canvasGroup = XiCAV[i];
			if ((bool)canvasGroup && canvasGroup.gameObject.activeSelf && canvasGroup.blocksRaycasts && canvasGroup.alpha > 0.5f)
			{
				xiIndex = i;
				return true;
			}
		}
		return false;
	}

	public void OpenClose()
	{
		if (!SingletonMonoScope<GameUIManager>.Instance.Opened_Talent)
		{
			MarkTalentPanelOpened();
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1f;
			SingletonMonoScope<GameUIManager>.Instance.Opened_Talent = true;
		}
		else
		{
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0f;
			SingletonMonoScope<GameUIManager>.Instance.Opened_Talent = false;
			SingletonMonoScope<GameUIManager>.Instance.HideDFSkillListForPageChange();
		}
	}

	public void SetXiBuff()
	{
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData != null && skillXiData.Used)
			{
				switch (skillXiData.damageType)
				{
				case DamageType.fire:
					PL.FireDamageXi = skillXiData.Number * (float)skillXiData.Level_Base;
					break;
				case DamageType.frozen:
					PL.FrozenDamageXi = skillXiData.Number * (float)skillXiData.Level_Base;
					break;
				case DamageType.thunder:
					PL.ThunderDamageXi = skillXiData.Number * (float)skillXiData.Level_Base;
					break;
				case DamageType.poison:
					PL.PoisonDamageXi = skillXiData.Number * (float)skillXiData.Level_Base;
					break;
				case DamageType.physics:
					PL.PhysicsDamageXi = skillXiData.Number * (float)skillXiData.Level_Base;
					break;
				case DamageType.shadow:
					PL.ShadowDamageXi = skillXiData.Number * (float)skillXiData.Level_Base;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
	}

	public void Refresh(int xi)
	{
		if (XiData[xi] == null)
		{
			LogUtil.Error($"XiData[{xi}] 为空");
			return;
		}
		SkillXiData skillXiData = XiData[xi];
		foreach (KeyValuePair<string, SkillData_Sample_Father> item in skillXiData.Sample_F)
		{
			SkillData_Sample_Father value = item.Value;
			if ((bool)value?.skillbt)
			{
				value.skillbt.SkillTU.sprite = value.iconB;
				value.skillbt.Unlock = false;
				value.skillbt.BS_Skill = value.BS_Skill;
				value.skillbt.LastSkill = value.LastSkill;
				value.skillbt.DashSkill = value.DashSkill;
				value.skillbt.TPSkill = value.TPSkill;
				value.skillbt.Refresh(value.Level_Base, value.Level_Max, value.Level_WeaponOn);
			}
		}
		foreach (KeyValuePair<string, SkillData_Sample_Son> sample_ in skillXiData.Sample_S)
		{
			SkillData_Sample_Son value2 = sample_.Value;
			if ((bool)value2?.skillbt)
			{
				value2.skillbt.SkillTU.sprite = value2.iconB;
				value2.skillbt.Unlock = false;
				value2.skillbt.Refresh(value2.Level_Base, value2.Level_Max, value2.Level_WeaponOn);
			}
		}
		foreach (KeyValuePair<string, SkillData_Comp_Father> item2 in skillXiData.Comp_F)
		{
			SkillData_Comp_Father value3 = item2.Value;
			if ((bool)value3?.skillbt)
			{
				value3.skillbt.SkillTU.sprite = value3.iconB;
				value3.skillbt.Unlock = false;
				value3.skillbt.BS_Skill = value3.BS_Skill;
				value3.skillbt.LastSkill = value3.LastSkill;
				value3.skillbt.Refresh(value3.Level_Base, value3.Level_Max, value3.Level_WeaponOn);
			}
		}
		foreach (KeyValuePair<string, SkillData_Comp_Son> comp_ in skillXiData.Comp_S)
		{
			SkillData_Comp_Son value4 = comp_.Value;
			if ((bool)value4?.skillbt)
			{
				value4.skillbt.SkillTU.sprite = value4.iconB;
				value4.skillbt.Unlock = false;
				value4.skillbt.Refresh(value4.Level_Base, value4.Level_Max, value4.Level_WeaponOn);
			}
		}
		foreach (KeyValuePair<string, SkillData_Dot_Father> item3 in skillXiData.Dot_F)
		{
			SkillData_Dot_Father value5 = item3.Value;
			if ((bool)value5?.skillbt)
			{
				value5.skillbt.SkillTU.sprite = value5.iconB;
				value5.skillbt.Unlock = false;
				value5.skillbt.Refresh(value5.Level_Base, value5.Level_Max, value5.Level_WeaponOn);
			}
		}
		foreach (KeyValuePair<string, SkillData_Dot_Son> dot_ in skillXiData.Dot_S)
		{
			SkillData_Dot_Son value6 = dot_.Value;
			if ((bool)value6?.skillbt)
			{
				value6.skillbt.SkillTU.sprite = value6.iconB;
				value6.skillbt.Unlock = false;
				value6.skillbt.Refresh(value6.Level_Base, value6.Level_Max, value6.Level_WeaponOn);
			}
		}
		foreach (KeyValuePair<string, SkillData_Bei> item4 in skillXiData.Bei)
		{
			SkillData_Bei value7 = item4.Value;
			if ((bool)value7?.skillbt)
			{
				value7.skillbt.SkillTU.sprite = value7.iconB;
				value7.skillbt.Unlock = false;
				value7.skillbt.Refresh(value7.Level_Base, value7.Level_Max, value7.Level_WeaponOn);
			}
		}
		foreach (KeyValuePair<string, SkillData_Sample_Father> item5 in skillXiData.Sample_F)
		{
			SkillData_Sample_Father value8 = item5.Value;
			if ((bool)value8?.skillbt && value8.UnLock_Point <= skillXiData.Level_Base)
			{
				value8.skillbt.SkillTU.sprite = value8.icon;
				value8.skillbt.Unlock = true;
			}
		}
		foreach (KeyValuePair<string, SkillData_Comp_Father> item6 in skillXiData.Comp_F)
		{
			SkillData_Comp_Father value9 = item6.Value;
			if ((bool)value9?.skillbt && value9.UnLock_Point <= skillXiData.Level_Base)
			{
				value9.skillbt.SkillTU.sprite = value9.icon;
				value9.skillbt.Unlock = true;
			}
		}
		foreach (KeyValuePair<string, SkillData_Dot_Father> item7 in skillXiData.Dot_F)
		{
			SkillData_Dot_Father value10 = item7.Value;
			if ((bool)value10?.skillbt && value10.UnLock_Point <= skillXiData.Level_Base)
			{
				value10.skillbt.SkillTU.sprite = value10.icon;
				value10.skillbt.Unlock = true;
			}
		}
		RefreshPassiveUnlockStateAllXi();
		foreach (KeyValuePair<string, SkillData_Sample_Son> sample_2 in skillXiData.Sample_S)
		{
			SkillData_Sample_Son value11 = sample_2.Value;
			if (!(value11?.skillbt) || value11.UnLock_Point > skillXiData.Level_Base || !skillXiData.Sample_F.TryGetValue(value11.FatherSkill, out var value12) || value12 == null || value12.Level_Base <= 0)
			{
				continue;
			}
			bool flag = false;
			switch (value11.FrontSkillType)
			{
			case 0:
			{
				if (skillXiData.Sample_F.TryGetValue(value11.FrontSkill, out var value14) && value14 != null && value14.Level_Base > 0)
				{
					flag = true;
				}
				break;
			}
			case 1:
			{
				if (skillXiData.Sample_S.TryGetValue(value11.FrontSkill, out var value13) && value13 != null && value13.Level_Base > 0)
				{
					flag = true;
				}
				break;
			}
			}
			if (flag)
			{
				value11.skillbt.Unlock = true;
				value11.skillbt.SkillTU.sprite = value11.icon;
			}
		}
		foreach (KeyValuePair<string, SkillData_Comp_Son> comp_2 in skillXiData.Comp_S)
		{
			SkillData_Comp_Son value15 = comp_2.Value;
			if (!(value15?.skillbt) || value15.UnLock_Point > skillXiData.Level_Base || !skillXiData.Comp_F.TryGetValue(value15.FatherSkill, out var value16) || value16 == null || value16.Level_Base <= 0)
			{
				continue;
			}
			bool flag2 = false;
			switch (value15.FrontSkillType)
			{
			case 2:
			{
				if (skillXiData.Comp_F.TryGetValue(value15.FrontSkill, out var value18) && value18 != null && value18.Level_Base > 0)
				{
					flag2 = true;
				}
				break;
			}
			case 3:
			{
				if (skillXiData.Comp_S.TryGetValue(value15.FrontSkill, out var value17) && value17 != null && value17.Level_Base > 0)
				{
					flag2 = true;
				}
				break;
			}
			}
			if (flag2)
			{
				value15.skillbt.Unlock = true;
				value15.skillbt.SkillTU.sprite = value15.icon;
			}
		}
		foreach (KeyValuePair<string, SkillData_Dot_Son> dot_2 in skillXiData.Dot_S)
		{
			SkillData_Dot_Son value19 = dot_2.Value;
			if (!(value19?.skillbt) || value19.UnLock_Point > skillXiData.Level_Base || !skillXiData.Dot_F.TryGetValue(value19.FatherSkill, out var value20) || value20 == null || value20.Level_Base <= 0)
			{
				continue;
			}
			bool flag3 = false;
			switch (value19.FrontSkillType)
			{
			case 4:
			{
				if (skillXiData.Dot_F.TryGetValue(value19.FrontSkill, out var value22) && value22 != null && value22.Level_Base > 0)
				{
					flag3 = true;
				}
				break;
			}
			case 5:
			{
				if (skillXiData.Dot_S.TryGetValue(value19.FrontSkill, out var value21) && value21 != null && value21.Level_Base > 0)
				{
					flag3 = true;
				}
				break;
			}
			}
			if (flag3)
			{
				value19.skillbt.Unlock = true;
				value19.skillbt.SkillTU.sprite = value19.icon;
			}
		}
	}

	public void Restore()
	{
		ClearAllRuntimeTalentEffects();
		ResetNormalTalentLevels();
		RebuildTalentDerivedStateAfterRestore();
		FlushDatas();
	}

	public void RestoreDF()
	{
		ClearDFRuntimeTalentEffects();
		ResetDFTalentLevels();
		RefreshDF();
		RefreshPointText();
		CloseResetTransientUI();
		Canvas.ForceUpdateCanvases();
		FlushDatas();
	}

	private void ClearAllRuntimeTalentEffects()
	{
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RestoreSkill();
			SingletonMonoScope<ACTbar>.Instance.ClearBeforeRebuildSkillList();
		}
		if (!(PL != null))
		{
			return;
		}
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData == null || !skillXiData.Used)
			{
				continue;
			}
			foreach (KeyValuePair<string, SkillData_Bei> item in skillXiData.Bei)
			{
				SkillData_Bei value = item.Value;
				if (value != null && value.Level_Base > 0)
				{
					int num = value.Level_Base + value.Level_WeaponOn;
					if (num > 0)
					{
						PL.SetSkillBeiBuff(add: false, value.B_Type, value.B_Number, num);
					}
				}
			}
		}
	}

	private void ResetNormalTalentLevels()
	{
		P_Used = 0;
		HasBSSkill = false;
		BSSkillButton = null;
		SkillXiData[] xiData = XiData;
		foreach (SkillXiData skillXiData in xiData)
		{
			if (skillXiData == null)
			{
				continue;
			}
			skillXiData.Level_Base = 0;
			foreach (KeyValuePair<string, SkillData_Sample_Father> item in skillXiData.Sample_F)
			{
				if (item.Value != null)
				{
					item.Value.Level_Base = 0;
				}
			}
			foreach (KeyValuePair<string, SkillData_Sample_Son> sample_ in skillXiData.Sample_S)
			{
				if (sample_.Value != null)
				{
					sample_.Value.Level_Base = 0;
				}
			}
			foreach (KeyValuePair<string, SkillData_Comp_Father> item2 in skillXiData.Comp_F)
			{
				if (item2.Value != null)
				{
					item2.Value.Level_Base = 0;
				}
			}
			foreach (KeyValuePair<string, SkillData_Comp_Son> comp_ in skillXiData.Comp_S)
			{
				if (comp_.Value != null)
				{
					comp_.Value.Level_Base = 0;
				}
			}
			foreach (KeyValuePair<string, SkillData_Dot_Father> item3 in skillXiData.Dot_F)
			{
				if (item3.Value != null)
				{
					item3.Value.Level_Base = 0;
				}
			}
			foreach (KeyValuePair<string, SkillData_Dot_Son> dot_ in skillXiData.Dot_S)
			{
				if (dot_.Value != null)
				{
					dot_.Value.Level_Base = 0;
				}
			}
			foreach (KeyValuePair<string, SkillData_Bei> item4 in skillXiData.Bei)
			{
				if (item4.Value != null)
				{
					item4.Value.Level_Base = 0;
				}
			}
		}
	}

	private void ResetDFTalentLevels()
	{
		P_Used_DF = 0;
		foreach (SkilDFData dFDatum in DFData)
		{
			if (dFDatum != null)
			{
				dFDatum.Level_Base = 0;
				dFDatum.SelectedIndex = 0;
				dFDatum.EnsureValidCurIndex();
			}
		}
	}

	private void RebuildTalentDerivedStateAfterRestore()
	{
		RecalcHasBSSkill();
		SetXiBuff();
		RebindAllSkillBT();
		for (int i = 0; i < XiData.Length; i++)
		{
			if (XiData[i] != null)
			{
				Refresh(i);
			}
		}
		RefreshAllSkillButtons();
		RebuildActbarFromLevels();
		RefreshPointText();
		RefreshDF();
		CloseResetTransientUI();
		Canvas.ForceUpdateCanvases();
	}

	private void CloseResetTransientUI()
	{
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.CloseSkillListUI();
		}
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideSkillTip();
			SingletonMonoScope<GameUIManager>.Instance.HideEmptyTip();
			SingletonMonoScope<GameUIManager>.Instance.HideDFSkillList();
		}
	}
}
