using System;
using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using UnityEngine.UI;

public static class SkillTagSystem
{
	private static readonly string[] XiIndexNames = new string[12]
	{
		"Hell Messenger", "Storm Lord", "Arcanist", "Blade Master", "Holy Light", "Apocalypse", "Windwalker", "Doomsday Disciple", "High Elf", "Undead Emissary",
		"Void Sorcerer", "Corrupt Priest"
	};

	private static readonly string[] XiNameFallback = new string[12]
	{
		"地狱使者", "风暴领主", "奥术师", "剑圣", "圣光", "天启", "风之游侠", "末日信徒", "高阶精灵", "亡灵使者",
		"虚空咒师", "腐化祭司"
	};

	private static readonly HashSet<string> BoomerangWhitelist = new HashSet<string> { "Ice Crystal" };

	private const string ColorElement = "#6FD3FF";

	private const string ColorForm = "#FFC266";

	private const string SuffixSentinel = "<color=#6FD3FF>";

	private static readonly List<Func<ACT_skillSample, IEnumerable<string>>> _contributors = new List<Func<ACT_skillSample, IEnumerable<string>>>();

	public static void RegisterTagContributor(Func<ACT_skillSample, IEnumerable<string>> contributor)
	{
		try
		{
			if (contributor != null && !_contributors.Contains(contributor))
			{
				_contributors.Add(contributor);
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[SkillTagSystem] RegisterTagContributor 异常: " + ex);
		}
	}

	public static void ApplyToSkillTip(int xi, int type, string skillName, Text mainText)
	{
		try
		{
			if (!mainText)
			{
				return;
			}
			string text = mainText.text ?? string.Empty;
			int num = text.IndexOf("<color=#6FD3FF>", StringComparison.Ordinal);
			string text2 = ((num >= 0) ? text.Substring(0, num) : text).TrimEnd();
			string text3 = BuildTagLine(xi, type, skillName);
			if (string.IsNullOrEmpty(text3))
			{
				if (num >= 0)
				{
					mainText.text = text2;
				}
			}
			else
			{
				mainText.text = text2 + "\n" + text3;
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[SkillTagSystem] ApplyToSkillTip 异常: " + ex);
		}
	}

	private static string BuildTagLine(int xi, int type, string skillName)
	{
		try
		{
			string text = BuildElementPart(xi);
			string text2 = BuildFormPart(xi, type, skillName);
			if (text == null && text2 == null)
			{
				return null;
			}
			if (text == null)
			{
				return text2;
			}
			if (text2 == null)
			{
				return text;
			}
			return text + " " + text2;
		}
		catch (Exception ex)
		{
			LogUtil.Error("[SkillTagSystem] BuildTagLine 异常: " + ex);
			return null;
		}
	}

	private static string BuildElementPart(int xi)
	{
		try
		{
			if (xi < 0 || xi >= XiIndexNames.Length)
			{
				return null;
			}
			string text = null;
			try
			{
				string skill = LOC.MM.GetSkill(XiIndexNames[xi]);
				if (!string.IsNullOrEmpty(skill) && skill != XiIndexNames[xi])
				{
					text = skill;
				}
			}
			catch (Exception)
			{
			}
			if (text == null)
			{
				text = XiNameFallback[xi];
			}
			return "<color=#6FD3FF>◆" + text + "</color>";
		}
		catch (Exception ex2)
		{
			LogUtil.Error("[SkillTagSystem] BuildElementPart 异常: " + ex2);
			return null;
		}
	}

	private static string BuildFormPart(int xi, int type, string skillName)
	{
		try
		{
			List<string> list = new List<string>();
			try
			{
				switch (type)
				{
				case 0:
					CollectSampleFTags(xi, skillName, list);
					break;
				case 2:
					CollectCompFTags(xi, skillName, list);
					break;
				case 4:
					CollectDotFTags(xi, skillName, list);
					break;
				case 1:
				case 3:
					break;
				}
			}
			catch (Exception ex)
			{
				LogUtil.Error("[SkillTagSystem] 形态推导异常: " + ex);
			}
			try
			{
				ACT_skillSample aCT_skillSample = BuildSnapshot(xi, type, skillName);
				if (aCT_skillSample != null)
				{
					foreach (Func<ACT_skillSample, IEnumerable<string>> contributor in _contributors)
					{
						foreach (string item in contributor(aCT_skillSample))
						{
							AddTag(list, item);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				LogUtil.Error("[SkillTagSystem] 扩展贡献者异常: " + ex2);
			}
			if (list.Count == 0)
			{
				return null;
			}
			return "<color=#FFC266>◇" + string.Join("·", list) + "</color>";
		}
		catch (Exception ex3)
		{
			LogUtil.Error("[SkillTagSystem] BuildFormPart 异常: " + ex3);
			return null;
		}
	}

	private static void CollectSampleFTags(int xi, string skillName, List<string> tags)
	{
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (!instance || instance.XiData == null || xi < 0 || xi >= instance.XiData.Length || instance.XiData[xi] == null || !instance.XiData[xi].Sample_F.TryGetValue(skillName, out var value) || value == null)
		{
			return;
		}
		AddTag(tags, FSWord(value.FStype, xi / 3));
		if (value.AllChuan_F == 0)
		{
			AddTag(tags, "穿透");
		}
		if (value.colEXP == 0)
		{
			AddTag(tags, "命中爆炸");
		}
		if (value.LastEXP == 0)
		{
			AddTag(tags, "末段爆裂");
		}
		try
		{
			if (value.CountMulti > 1 || value.Count_F_Last > 1 || value.Count_S_Last > 1 || value.Count_ORB > 1)
			{
				AddTag(tags, "多弹");
			}
		}
		catch (Exception)
		{
		}
		if (value.Follow_F == 0)
		{
			AddTag(tags, "追踪");
		}
		try
		{
			if (value.MoveSpeedCut_Last > 0f)
			{
				AddTag(tags, "减速");
			}
		}
		catch (Exception)
		{
		}
		AddDotTagIfLinked(instance, xi, value.damageType, tags);
		if (skillName != null && BoomerangWhitelist.Contains(skillName))
		{
			AddTag(tags, "回旋");
		}
	}

	private static void CollectCompFTags(int xi, string skillName, List<string> tags)
	{
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if ((bool)instance && instance.XiData != null && xi >= 0 && xi < instance.XiData.Length && instance.XiData[xi] != null && instance.XiData[xi].Comp_F.TryGetValue(skillName, out var value) && value != null)
		{
			AddTag(tags, "召唤");
			if (value.Summon_count_Base > 1 || value.Count_A > 1 || value.Count_B > 1 || value.CountMulti_A > 1 || value.CountMulti_B > 1)
			{
				AddTag(tags, "多弹");
			}
			if (value.AllChuan_A == 0 || value.AllChuan_B == 0)
			{
				AddTag(tags, "穿透");
			}
			if (value.Follow_A == 0 || value.Follow_B == 0)
			{
				AddTag(tags, "追踪");
			}
			if (value.colEXP_A == 0 || value.colEXP_B == 0)
			{
				AddTag(tags, "命中爆炸");
			}
			AddDotTagIfLinked(instance, xi, value.damageType, tags);
		}
	}

	private static void CollectDotFTags(int xi, string skillName, List<string> tags)
	{
		TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
		if (!instance || instance.XiData == null || xi < 0 || xi >= instance.XiData.Length || instance.XiData[xi] == null || !instance.XiData[xi].Dot_F.TryGetValue(skillName, out var value) || value == null)
		{
			return;
		}
		AddTag(tags, DotWord(value.damageType));
		try
		{
			if (value.MVSpeedCut_Last > 0f || value.ATSpeedCut_Last > 0f)
			{
				AddTag(tags, "减速");
			}
		}
		catch (Exception)
		{
		}
	}

	private static void AddDotTagIfLinked(TalentManager tm, int xi, DamageType damageType, List<string> tags)
	{
		try
		{
			if (tm == null || tm.XiData == null || xi < 0 || xi >= tm.XiData.Length || tm.XiData[xi] == null)
			{
				return;
			}
			foreach (KeyValuePair<string, SkillData_Dot_Father> item in tm.XiData[xi].Dot_F)
			{
				if (item.Value != null && item.Value.Level_Base > 0 && item.Value.damageType == damageType)
				{
					AddTag(tags, DotWord(damageType));
					break;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private static string FSWord(int fsType, int weaponFamily)
	{
		switch (fsType)
		{
		case 3:
			if (weaponFamily == 0)
			{
				return "位移";
			}
			return "直射";
		case 0:
		case 1:
		case 2:
		case 4:
		case 5:
		case 6:
			return "直射";
		case 7:
		case 8:
		case 9:
			return "环绕";
		case 10:
			return "落点";
		default:
			return null;
		}
	}

	private static string DotWord(DamageType type)
	{
		return type switch
		{
			DamageType.fire => "灼烧", 
			DamageType.frozen => "冻伤", 
			DamageType.thunder => "感电", 
			DamageType.poison => "中毒", 
			DamageType.physics => "流血", 
			DamageType.shadow => "侵蚀", 
			_ => null, 
		};
	}

	private static ACT_skillSample BuildSnapshot(int xi, int type, string skillName)
	{
		try
		{
			if (type != 0)
			{
				return null;
			}
			TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
			if (!instance || instance.XiData == null || xi < 0 || xi >= instance.XiData.Length || instance.XiData[xi] == null)
			{
				return null;
			}
			if (!instance.XiData[xi].Sample_F.TryGetValue(skillName, out var value) || value == null)
			{
				return null;
			}
			return new ACT_skillSample
			{
				SkillName = skillName,
				FStype = value.FStype,
				damageType = value.damageType,
				AllChuan_F = value.AllChuan_F,
				colEXP = value.colEXP,
				LastEXP = value.LastEXP,
				Follow_F = value.Follow_F,
				CountMulti = value.CountMulti,
				Count_ORB = value.Count_ORB,
				MoveSpeedCut = value.MoveSpeedCut_Base
			};
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static void AddTag(List<string> tags, string tag)
	{
		if (!string.IsNullOrEmpty(tag) && !tags.Contains(tag))
		{
			tags.Add(tag);
		}
	}
}
