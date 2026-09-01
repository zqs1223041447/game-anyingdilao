using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace PoedbMod;

public static class NLCommandProcessor
{
	private static readonly Dictionary<string, string> ActionKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "技能", "skill" },
		{ "skill", "skill" },
		{ "词缀", "affix" },
		{ "mod", "affix" },
		{ "天赋", "talent" },
		{ "passive", "talent" },
		{ "制作", "crafting" },
		{ "craft", "crafting" },
		{ "辅助", "support" },
		{ "support", "support" },
		{ "地图", "map" },
		{ "map", "map" },
		{ "敌人", "enemy" },
		{ "monster", "enemy" },
		{ "装备", "equipment" },
		{ "equipment", "equipment" }
	};

	public static NLParseResult Parse(string text)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return new NLParseResult
				{
					Action = "skill",
					TargetId = null,
					Raw = text
				};
			}
			string text2 = text.ToLowerInvariant();
			string text3 = null;
			foreach (KeyValuePair<string, string> actionKeyword in ActionKeywords)
			{
				if (text2.Contains(actionKeyword.Key.ToLowerInvariant()))
				{
					text3 = actionKeyword.Value;
					break;
				}
			}
			if (text3 == null)
			{
				text3 = "skill";
			}
			string targetId = MatchKnownSkill(text);
			return new NLParseResult
			{
				Action = text3,
				TargetId = targetId,
				Raw = text
			};
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbMod.NL] Parse 异常: " + ex.Message);
			return new NLParseResult
			{
				Action = "skill",
				TargetId = null,
				Raw = text
			};
		}
	}

	private static string MatchKnownSkill(string text)
	{
		try
		{
			if (!Registry.IsInitialized)
			{
				Registry.Initialize();
			}
			string text2 = text.ToLowerInvariant();
			foreach (IModSkill item in Registry.AllSkills())
			{
				if (item != null)
				{
					if (!string.IsNullOrEmpty(item.Id) && text2.Contains(item.Id.ToLowerInvariant()))
					{
						return item.Id;
					}
					if (!string.IsNullOrEmpty(item.Name) && text2.Contains(item.Name.ToLowerInvariant()))
					{
						return item.Id;
					}
					if (!string.IsNullOrEmpty(item.NameZh) && text.Contains(item.NameZh))
					{
						return item.Id;
					}
				}
			}
			if (text.Contains("龙卷") || text2.Contains("tornado"))
			{
				return "tornado-shot";
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public static string Process(string command, string outputRoot = null)
	{
		try
		{
			string targetId = Parse(command).TargetId;
			if (string.IsNullOrEmpty(targetId))
			{
				Debug.LogWarning("[PoedbMod.NL] 未识别技能，尝试列出可用技能。");
				foreach (IModSkill item in Registry.AllSkills())
				{
					Debug.Log("[PoedbMod.NL] 可用技能: " + item.Id + " - " + item.NameZh + " (" + item.Name + ")");
				}
				return null;
			}
			IModSkill skill = Registry.GetSkill(targetId);
			if (skill == null)
			{
				Debug.LogError("[PoedbMod.NL] 未找到技能: " + targetId);
				return null;
			}
			return GeneratePack(skill, outputRoot);
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.NL] Process 异常: " + ex);
			return null;
		}
	}

	private static string GeneratePack(IModSkill skill, string outputRoot)
	{
		try
		{
			string text = Sanitize(skill.Name ?? skill.Id);
			string text2 = Path.Combine(outputRoot ?? ResolveDefaultPacksRoot(), text);
			Directory.CreateDirectory(text2);
			string path = Path.Combine(text2, "skill_definition.json");
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{ "schema_version", "1.0.0" },
				{ "pack_name", text },
				{ "source", "poedb.tw" },
				{
					"skill",
					new Dictionary<string, object>
					{
						{ "id", skill.Id },
						{ "name", skill.Name },
						{ "name_zh", skill.NameZh },
						{ "tags", skill.Tags },
						{ "description", skill.Description },
						{ "description_zh", skill.DescriptionZh }
					}
				}
			};
			if (skill.ColumnOverrides != null)
			{
				dictionary["column_overrides"] = skill.ColumnOverrides;
			}
			WriteAllTextBom(path, JsonConvert.SerializeObject(dictionary, Formatting.Indented));
			string path2 = Path.Combine(text2, "samplef_row.csv");
			string text3 = BuildSampleRow(skill);
			WriteAllTextBom(path2, text3 + "\n");
			string path3 = Path.Combine(text2, "localization.json");
			string value = "info_" + (skill.Name ?? skill.Id);
			if (skill.ColumnOverrides != null && skill.ColumnOverrides.TryGetValue("Info", out var value2))
			{
				value = value2;
			}
			Dictionary<string, object> value3 = new Dictionary<string, object>
			{
				{ "info_key", value },
				{
					"localizations",
					new Dictionary<string, string>
					{
						{
							"English",
							skill.Description ?? ""
						},
						{
							"ChineseS",
							skill.DescriptionZh ?? ""
						},
						{
							"ChineseT",
							skill.DescriptionZh ?? ""
						}
					}
				}
			};
			WriteAllTextBom(path3, JsonConvert.SerializeObject(value3, Formatting.Indented));
			string path4 = Path.Combine(text2, "pack.json");
			Dictionary<string, object> value4 = new Dictionary<string, object>
			{
				{ "pack_name", text },
				{
					"created_at",
					DateTime.UtcNow.ToString("o")
				},
				{
					"command",
					"参考POEDB增加" + (skill.NameZh ?? skill.Name) + "技能"
				},
				{ "skill_id", skill.Id },
				{
					"files",
					new Dictionary<string, string>
					{
						{
							"skill_definition.json",
							Sha256(path)
						},
						{
							"samplef_row.csv",
							Sha256(path2)
						},
						{
							"localization.json",
							Sha256(path3)
						}
					}
				},
				{ "deploy_notes", "1. 使用 SkillForge 导入 samplef_row.csv\n2. 导入 localization.json 的 info_key 到 resources.assets Skill_FY\n3. Tier 2 特效需在 SK_FlyA 中实现" }
			};
			WriteAllTextBom(path4, JsonConvert.SerializeObject(value4, Formatting.Indented));
			WriteAllTextBom(Path.Combine(text2, "README.md"), "# " + skill.NameZh + " (" + skill.Name + ")\n\n来源: poedb.tw\n标签: " + string.Join(", ", skill.Tags ?? new List<string>()) + "\n\n" + skill.DescriptionZh + "\n");
			Debug.Log("[PoedbMod.NL] Pack 已生成: " + text2);
			return text2;
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbMod.NL] GeneratePack 异常: " + ex);
			return null;
		}
	}

	private static string BuildSampleRow(IModSkill skill)
	{
		string[] array = new string[19]
		{
			"IndexName", "Info", "Xi", "Price", "UnLock_Point", "Level_Max", "UseAni", "FStype", "damageType", "Damage_Base",
			"Damage_Level", "ManaCost_Base", "CoolDown_Base", "CountMulti", "AllChuan_F", "Follow_F", "FlySpeed_Base", "Size", "AngleA"
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			{
				"IndexName",
				skill.Name ?? skill.Id
			},
			{
				"Info",
				"info_" + (skill.Name ?? skill.Id)
			},
			{ "Xi", "6" },
			{ "Price", "0" },
			{ "UnLock_Point", "0" },
			{ "Level_Max", "20" },
			{ "UseAni", "0" },
			{ "FStype", "7" },
			{ "damageType", "physics" },
			{ "Damage_Base", "100" },
			{ "Damage_Level", "3" },
			{ "ManaCost_Base", "8" },
			{ "CoolDown_Base", "1.2" },
			{ "CountMulti", "6" },
			{ "AllChuan_F", "0" },
			{ "Follow_F", "0" },
			{ "FlySpeed_Base", "10" },
			{ "Size", "1" },
			{ "AngleA", "0" }
		};
		if (skill.ColumnOverrides != null)
		{
			foreach (KeyValuePair<string, string> columnOverride in skill.ColumnOverrides)
			{
				dictionary[columnOverride.Key] = columnOverride.Value;
			}
		}
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string key in array2)
		{
			dictionary.TryGetValue(key, out var value);
			list.Add(value ?? "");
		}
		return string.Join(",", list);
	}

	private static string ResolveDefaultPacksRoot()
	{
		try
		{
			string dataPath = Application.dataPath;
			if (!string.IsNullOrEmpty(dataPath))
			{
				return Path.GetFullPath(Path.Combine(dataPath, "..", "builds", "packs"));
			}
		}
		catch
		{
		}
		return "builds/packs";
	}

	private static string Sanitize(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return "pack";
		}
		char[] array = name.ToLowerInvariant().ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (!char.IsLetterOrDigit(array[i]) && array[i] != '-' && array[i] != '_')
			{
				array[i] = '-';
			}
		}
		return new string(array).Trim('-');
	}

	private static void WriteAllTextBom(string path, string content)
	{
		File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
	}

	private static string Sha256(string path)
	{
		try
		{
			using SHA256 sHA = SHA256.Create();
			using FileStream inputStream = File.OpenRead(path);
			return BitConverter.ToString(sHA.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant();
		}
		catch
		{
			return "";
		}
	}
}
