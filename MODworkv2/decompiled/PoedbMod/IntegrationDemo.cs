using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace PoedbMod;

public static class IntegrationDemo
{
	public static int RunAllChecks()
	{
		int num = 0;
		try
		{
			if (!Registry.IsInitialized)
			{
				Registry.Initialize();
			}
			Debug.Log("[PoedbMod.Demo] === 多机制集成演示开始 ===");
			try
			{
				IModEquipment modEquipment = Registry.ApplyEquipmentEffect("headhunter");
				if (modEquipment != null && modEquipment.ExplicitMods != null && modEquipment.ExplicitMods.Count > 0)
				{
					Debug.Log("[Demo] equipment_effects ✓ headhunter: " + string.Join("; ", modEquipment.ExplicitMods));
				}
				else
				{
					Debug.LogWarning("[Demo] equipment_effects ✗ headhunter 未找到");
					num++;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[Demo] equipment_effects 异常: " + ex.Message);
				num++;
			}
			string reason2;
			try
			{
				if (Registry.CanSupport("greater-multiple-projectiles-support", "tornado-shot", out var reason))
				{
					Debug.Log("[Demo] support_gems ✓ GMP 可支持 tornado-shot");
				}
				else
				{
					Debug.LogWarning("[Demo] support_gems ✗ 校验失败: " + reason);
					num++;
				}
				Debug.Log("[Demo] support_gems 限制示例: GMP→tornado-shot=" + Registry.CanSupport("greater-multiple-projectiles-support", "tornado-shot", out reason2));
			}
			catch (Exception ex2)
			{
				Debug.LogWarning("[Demo] support_gems 异常: " + ex2.Message);
				num++;
			}
			try
			{
				IModTalent talent = Registry.GetTalent("2001");
				if (talent != null && talent.IsJewelSocket)
				{
					Debug.Log("[Demo] talent_tree ✓ jewel_socket id=2001 radius=" + talent.JewelRadius);
				}
				else
				{
					Debug.LogWarning("[Demo] talent_tree ✗ jewel_socket 未找到");
					num++;
				}
				try
				{
					TalentManager talentManager = UnityEngine.Object.FindObjectOfType<TalentManager>();
					if (talentManager != null)
					{
						Registry.ApplyToTalentManager(talentManager);
					}
					else
					{
						Debug.Log("[Demo] talent_tree: TalentManager 未在场景中，跳过 Apply（不计失败）");
					}
				}
				catch
				{
				}
			}
			catch (Exception ex3)
			{
				Debug.LogWarning("[Demo] talent_tree 异常: " + ex3.Message);
				num++;
			}
			try
			{
				if (Registry.CanCraft("craft-plus1-socketed-gems", "Body Armour", out var reason3))
				{
					Debug.Log("[Demo] crafting ✓ +1 gems 可用于 Body Armour");
				}
				else
				{
					Debug.LogWarning("[Demo] crafting ✗ " + reason3);
					num++;
				}
				Debug.Log("[Demo] crafting 非法类别校验: " + Registry.CanCraft("craft-plus1-socketed-gems", "InvalidClass", out reason2) + "（期望 false）");
			}
			catch (Exception ex4)
			{
				Debug.LogWarning("[Demo] crafting 异常: " + ex4.Message);
				num++;
			}
			try
			{
				string text = Registry.ApplyEnemyMod("enemy-of-the-elder");
				if (!string.IsNullOrEmpty(text))
				{
					Debug.Log("[Demo] enemy_mods ✓ of the Elder: " + text);
				}
				else
				{
					Debug.LogWarning("[Demo] enemy_mods ✗ 未找到");
					num++;
				}
			}
			catch (Exception ex5)
			{
				Debug.LogWarning("[Demo] enemy_mods 异常: " + ex5.Message);
				num++;
			}
			try
			{
				string text2 = Registry.ApplyMapMod("map-of-antagonism");
				if (!string.IsNullOrEmpty(text2))
				{
					Debug.Log("[Demo] map_mods ✓ of Antagonism: " + text2);
				}
				else
				{
					Debug.LogWarning("[Demo] map_mods ✗ 未找到");
					num++;
				}
			}
			catch (Exception ex6)
			{
				Debug.LogWarning("[Demo] map_mods 异常: " + ex6.Message);
				num++;
			}
			try
			{
				IModSkill skill = Registry.GetSkill("tornado-shot");
				if (skill != null && skill.Tags != null && skill.Tags.Any((string t) => t == "Bow"))
				{
					Debug.Log("[Demo] skills ✓ tornado-shot tags=" + string.Join(",", skill.Tags) + " CountMulti=" + ((skill.ColumnOverrides != null && skill.ColumnOverrides.TryGetValue("CountMulti", out var value)) ? value : "?"));
				}
				else
				{
					Debug.LogWarning("[Demo] skills ✗ tornado-shot 未找到");
					num++;
				}
				string text3 = NLCommandProcessor.Process("参考POEDB增加龙卷射击技能");
				if (!string.IsNullOrEmpty(text3))
				{
					Debug.Log("[Demo] NLCommandProcessor ✓ pack=" + text3);
				}
				else
				{
					Debug.LogWarning("[Demo] NLCommandProcessor ✗ 生成失败");
				}
			}
			catch (Exception ex7)
			{
				Debug.LogWarning("[Demo] skills 异常: " + ex7.Message);
				num++;
			}
			Debug.Log("[PoedbMod.Demo] === 演示结束，失败数=" + num + " ===");
		}
		catch (Exception ex8)
		{
			Debug.LogError("[PoedbMod.Demo] RunAllChecks 异常: " + ex8);
			num++;
		}
		return num;
	}

	public static void Schedule(MonoBehaviour host, float delaySeconds = 2f)
	{
		if (host != null)
		{
			host.StartCoroutine(ScheduleCo(delaySeconds));
		}
	}

	private static IEnumerator ScheduleCo(float delay)
	{
		yield return new WaitForSeconds(delay);
		RunAllChecks();
	}
}
