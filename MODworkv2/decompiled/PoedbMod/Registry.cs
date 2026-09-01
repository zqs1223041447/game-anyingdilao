using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PoedbMod;

public static class Registry
{
	private static readonly Dictionary<string, IModSkill> _skills = new Dictionary<string, IModSkill>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, IModAffix> _affixes = new Dictionary<string, IModAffix>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, IModTalent> _talents = new Dictionary<string, IModTalent>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, IModEquipment> _equipments = new Dictionary<string, IModEquipment>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, IModCrafting> _craftings = new Dictionary<string, IModCrafting>(StringComparer.OrdinalIgnoreCase);

	private static bool _tagHookInstalled;

	private static bool _initialized;

	public static int SkillCount => _skills.Count;

	public static int AffixCount => _affixes.Count;

	public static int TalentCount => _talents.Count;

	public static int EquipmentCount => _equipments.Count;

	public static int CraftingCount => _craftings.Count;

	public static bool IsInitialized => _initialized;

	public static void RegisterSkill(IModSkill skill)
	{
		try
		{
			if (skill != null && !string.IsNullOrEmpty(skill.Id))
			{
				_skills[skill.Id] = skill;
				if (PoedbModConfig.VerboseLog)
				{
					Debug.Log("[PoedbMod.Registry] 注册技能: " + skill.Id + " (" + skill.Name + ")");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.Registry] RegisterSkill 异常: " + ex);
		}
	}

	public static void RegisterAffix(IModAffix affix)
	{
		try
		{
			if (affix != null && !string.IsNullOrEmpty(affix.Id))
			{
				_affixes[affix.Id] = affix;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.Registry] RegisterAffix 异常: " + ex);
		}
	}

	public static void RegisterTalent(IModTalent talent)
	{
		try
		{
			if (talent != null && !string.IsNullOrEmpty(talent.Id))
			{
				_talents[talent.Id] = talent;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.Registry] RegisterTalent 异常: " + ex);
		}
	}

	public static void RegisterEquipment(IModEquipment eq)
	{
		try
		{
			if (eq == null || string.IsNullOrEmpty(eq.Id))
			{
				return;
			}
			_equipments[eq.Id] = eq;
			if (eq.ExplicitMods == null || eq.ExplicitMods.Count <= 0)
			{
				if (eq.FlavourText != null)
				{
				}
			}
			else
			{
				_ = eq.ExplicitMods[0];
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.Registry] RegisterEquipment 异常: " + ex);
		}
	}

	public static void RegisterCrafting(IModCrafting recipe)
	{
		try
		{
			if (recipe != null && !string.IsNullOrEmpty(recipe.Id))
			{
				_craftings[recipe.Id] = recipe;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.Registry] RegisterCrafting 异常: " + ex);
		}
	}

	public static IModSkill GetSkill(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		_skills.TryGetValue(id, out var value);
		return value;
	}

	public static IModAffix GetAffix(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		_affixes.TryGetValue(id, out var value);
		return value;
	}

	public static IModTalent GetTalent(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		_talents.TryGetValue(id, out var value);
		return value;
	}

	public static IModEquipment GetEquipment(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		_equipments.TryGetValue(id, out var value);
		return value;
	}

	public static IModCrafting GetCrafting(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		_craftings.TryGetValue(id, out var value);
		return value;
	}

	public static IReadOnlyCollection<IModSkill> AllSkills()
	{
		return _skills.Values.ToList().AsReadOnly();
	}

	public static IReadOnlyCollection<IModAffix> AllAffixes()
	{
		return _affixes.Values.ToList().AsReadOnly();
	}

	public static IReadOnlyCollection<IModTalent> AllTalents()
	{
		return _talents.Values.ToList().AsReadOnly();
	}

	public static IReadOnlyCollection<IModEquipment> AllEquipments()
	{
		return _equipments.Values.ToList().AsReadOnly();
	}

	public static IReadOnlyCollection<IModCrafting> AllCraftings()
	{
		return _craftings.Values.ToList().AsReadOnly();
	}

	public static void Initialize()
	{
		try
		{
			PoedbModConfig.TryLoadFromFile();
			DataLoader.Init();
			_skills.Clear();
			_affixes.Clear();
			_talents.Clear();
			_equipments.Clear();
			_craftings.Clear();
			foreach (ModSkill item in DataLoader.LoadCategory<ModSkill>("skills"))
			{
				RegisterSkill(item);
			}
			foreach (ModSkill item2 in DataLoader.LoadCategory<ModSkill>("support_gems"))
			{
				RegisterSkill(item2);
			}
			foreach (ModEquipment item3 in DataLoader.LoadCategory<ModEquipment>("equipment_effects"))
			{
				RegisterEquipment(item3);
				RegisterAffix(new ModAffix
				{
					Id = item3.Id,
					Name = item3.Name,
					Level = 0,
					PreSuf = "unique",
					Description = ((item3.ExplicitMods != null && item3.ExplicitMods.Count > 0) ? string.Join("; ", item3.ExplicitMods) : item3.FlavourText),
					Weight = string.Join(",", item3.Tags ?? new List<string>())
				});
			}
			foreach (ModCrafting item4 in DataLoader.LoadCategory<ModCrafting>("crafting"))
			{
				RegisterCrafting(item4);
			}
			foreach (ModAffix item5 in DataLoader.LoadCategory<ModAffix>("enemy_mods"))
			{
				RegisterAffix(item5);
			}
			foreach (ModAffix item6 in DataLoader.LoadCategory<ModAffix>("map_mods"))
			{
				RegisterAffix(item6);
			}
			foreach (ModTalent item7 in DataLoader.LoadCategory<ModTalent>("talent_tree"))
			{
				RegisterTalent(item7);
			}
			InstallSkillTagHook();
			_initialized = true;
			Debug.Log("[PoedbMod.Registry] 初始化完成: skills=" + _skills.Count + " equips=" + _equipments.Count + " crafts=" + _craftings.Count + " affixes=" + _affixes.Count + " talents=" + _talents.Count);
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.Registry] Initialize 异常: " + ex);
		}
	}

	private static void InstallSkillTagHook()
	{
		try
		{
			if (!_tagHookInstalled)
			{
				SkillTagSystem.RegisterTagContributor(CollectPoedbTags);
				_tagHookInstalled = true;
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbMod.Registry] InstallSkillTagHook 异常: " + ex.Message);
		}
	}

	private static IEnumerable<string> CollectPoedbTags(ACT_skillSample sample)
	{
		List<string> list = new List<string>();
		try
		{
			if (sample == null)
			{
				return list;
			}
			string text = sample.SkillName ?? string.Empty;
			if (text == "Cyclone")
			{
				list.Add("Area·环绕·持续");
				if (_skills.TryGetValue("cyclone", out var value) && value != null && value.Tags != null && value.Tags.Count > 0)
				{
					list.Add("POEDB: " + string.Join(", ", value.Tags));
				}
				return list;
			}
			if (text == "Tornado Shot")
			{
				list.Add("Projectile·穿透·散射");
				if (_skills.TryGetValue("tornado-shot", out var value2) && value2 != null && value2.Tags != null && value2.Tags.Count > 0)
				{
					list.Add("POEDB: " + string.Join(", ", value2.Tags));
				}
				return list;
			}
			if (_skills.Count == 0)
			{
				return list;
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbMod.Registry] CollectPoedbTags 异常: " + ex.Message);
		}
		return list;
	}

	public static bool IsBoomerangSkill(string skillName)
	{
		try
		{
			if (string.IsNullOrEmpty(skillName))
			{
				return false;
			}
			IModSkill modSkill = GetSkill(skillName) ?? _skills.Values.FirstOrDefault((IModSkill x) => string.Equals(x.Name, skillName, StringComparison.OrdinalIgnoreCase));
			if (modSkill != null && modSkill.Tags != null)
			{
				foreach (string tag in modSkill.Tags)
				{
					if (string.Equals(tag, "Returning", StringComparison.OrdinalIgnoreCase) || string.Equals(tag, "Boomerang", StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public static bool CanSupport(string supportId, string targetSkillId, out string reason)
	{
		reason = null;
		try
		{
			IModSkill skill = GetSkill(supportId);
			IModSkill skill2 = GetSkill(targetSkillId);
			if (skill == null)
			{
				reason = "未找到辅助宝石: " + supportId;
				return false;
			}
			if (skill2 == null)
			{
				reason = "未找到目标技能: " + targetSkillId;
				return false;
			}
			if (skill.SupportedTags != null && skill.SupportedTags.Count > 0)
			{
				bool flag = false;
				foreach (string st in skill.SupportedTags)
				{
					if (skill2.Tags != null && skill2.Tags.Any((string t) => string.Equals(t, st, StringComparison.OrdinalIgnoreCase)))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					reason = skill.Name + " 不支持 " + skill2.Name + "（需要 " + string.Join("/", skill.SupportedTags) + "）";
					return false;
				}
			}
			if (skill.Restrictions != null)
			{
				foreach (string restriction in skill.Restrictions)
				{
					if (!string.IsNullOrEmpty(restriction) && restriction.IndexOf("projectile", StringComparison.OrdinalIgnoreCase) >= 0 && (skill2.Tags == null || !skill2.Tags.Any((string t) => string.Equals(t, "Projectile", StringComparison.OrdinalIgnoreCase))))
					{
						reason = restriction;
						return false;
					}
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbMod.Registry] CanSupport 异常: " + ex.Message);
			return true;
		}
	}

	public static bool CanCraft(string recipeId, string itemClass, out string reason)
	{
		reason = null;
		try
		{
			IModCrafting crafting = GetCrafting(recipeId);
			if (crafting == null)
			{
				reason = "未找到配方: " + recipeId;
				return false;
			}
			if (crafting.ItemClasses == null || crafting.ItemClasses.Count == 0)
			{
				return true;
			}
			foreach (string itemClass2 in crafting.ItemClasses)
			{
				if (string.Equals(itemClass2, itemClass, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			reason = crafting.Mod + " 不适用于 " + itemClass;
			return false;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbMod.Registry] CanCraft 异常: " + ex.Message);
			return true;
		}
	}

	public static void ApplyToTalentManager(TalentManager manager)
	{
		try
		{
			if (manager == null)
			{
				return;
			}
			Debug.Log("[PoedbMod.Registry] ApplyToTalentManager: 已注册 talents=" + _talents.Count + " | jewel_sockets=" + _talents.Values.Count((IModTalent t) => t.IsJewelSocket));
			foreach (IModTalent value in _talents.Values)
			{
				if (value.IsJewelSocket)
				{
					Debug.Log("[PoedbMod.Talent] 珠宝插槽: " + value.Name + " id=" + value.Id + " radius=" + value.JewelRadius);
				}
				else if (PoedbModConfig.VerboseLog)
				{
					Debug.Log("[PoedbMod.Talent] 节点: " + value.Name + " (" + value.Type + ") stats=" + string.Join("; ", value.Stats ?? new List<string>()));
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbMod.Registry] ApplyToTalentManager 异常: " + ex.Message);
		}
	}

	public static bool CanUseSkill(string skillId, out string reason)
	{
		reason = null;
		try
		{
			IModSkill skill = GetSkill(skillId);
			if (skill == null)
			{
				return true;
			}
			if (skill.Restrictions != null)
			{
				_ = skill.Restrictions.Count;
				_ = 0;
			}
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbMod.Registry] CanUseSkill 异常: " + ex.Message);
			return true;
		}
	}

	public static string ApplyEnemyMod(string modId)
	{
		try
		{
			return GetAffix(modId)?.Description;
		}
		catch
		{
			return null;
		}
	}

	public static string ApplyMapMod(string modId)
	{
		try
		{
			return GetAffix(modId)?.Description;
		}
		catch
		{
			return null;
		}
	}

	public static IModEquipment ApplyEquipmentEffect(string equipId)
	{
		try
		{
			return GetEquipment(equipId);
		}
		catch
		{
			return null;
		}
	}
}
