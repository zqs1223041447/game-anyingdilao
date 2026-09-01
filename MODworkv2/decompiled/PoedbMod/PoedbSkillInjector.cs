using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FinkFramework.Runtime.Singleton;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PoedbMod;

public static class PoedbSkillInjector
{
	private const string TornadoName = "Tornado Shot";

	private const string TornadoInfoKey = "info_Tornado Shot";

	private const string CycloneName = "Cyclone";

	private const string CycloneInfoKey = "info_Cyclone";

	private static readonly int[] LandingXis = new int[4] { 0, 3, 6, 9 };

	private const float GridStepX = 155f;

	private const float GridStepY = 170f;

	private const float SlotClearance = 45f;

	private const float PanelMargin = 60f;

	private static readonly int[] SlotCandidatesX = new int[18]
	{
		1, 2, -1, -2, 0, 0, 1, 1, -1, -1,
		2, 2, 3, -3, 0, 0, 2, -2
	};

	private static readonly int[] SlotCandidatesY = new int[18]
	{
		0, 0, 0, 0, -1, 1, -1, 1, -1, 1,
		-1, 1, 0, 0, -2, 2, -2, -1
	};

	private static bool _locInjected;

	public static void TryInjectData(TalentManager tm)
	{
		try
		{
			if (tm == null || tm.XiData == null)
			{
				Debug.LogWarning("[PoedbSkillInjector] XiData 未就绪，本次跳过（面板打开时会自愈重试）");
				return;
			}
			InjectLocalizationFallback();
			for (int i = 0; i < LandingXis.Length; i++)
			{
				int num = LandingXis[i];
				if (num >= 0 && num < tm.XiData.Length && tm.XiData[num] != null && tm.XiData[num].Sample_F != null && tm.XiData[num].Sample_F.Count != 0)
				{
					InjectTornado(tm, num);
					InjectCyclone(tm, num);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbSkillInjector] TryInjectData 异常: " + ex);
		}
	}

	private static void InjectTornado(TalentManager tm, int xi)
	{
		Dictionary<string, SkillData_Sample_Father> sample_F = tm.XiData[xi].Sample_F;
		if (sample_F.ContainsKey("Tornado Shot"))
		{
			RegisterData(tm, xi, "Tornado Shot");
			return;
		}
		SkillData_Sample_Father skillData_Sample_Father = null;
		foreach (KeyValuePair<string, SkillData_Sample_Father> item in sample_F)
		{
			if (item.Value != null)
			{
				skillData_Sample_Father = item.Value;
				break;
			}
		}
		if (skillData_Sample_Father == null)
		{
			Debug.LogWarning("[PoedbSkillInjector] Xi=" + xi + " 无主动技能模板，跳过 Tornado Shot");
			return;
		}
		SkillData_Sample_Father skillData_Sample_Father2 = CloneFather(skillData_Sample_Father);
		skillData_Sample_Father2.IndexName = "Tornado Shot";
		skillData_Sample_Father2.Info = "info_Tornado Shot";
		skillData_Sample_Father2.Xi = xi;
		skillData_Sample_Father2.Price = 0;
		skillData_Sample_Father2.UnLock_Point = 0;
		skillData_Sample_Father2.Level_Max = 4;
		skillData_Sample_Father2.LastSkill = false;
		skillData_Sample_Father2.AllChuan_F = 0;
		skillData_Sample_Father2.ManaCost_Base = 8f;
		skillData_Sample_Father2.CoolDown_Base = 1.2f;
		skillData_Sample_Father2.Damage_Base = 100f;
		skillData_Sample_Father2.Damage_Level = 30f;
		sample_F["Tornado Shot"] = skillData_Sample_Father2;
		RegisterData(tm, xi, "Tornado Shot");
		Debug.Log("[PoedbSkillInjector] Xi=" + xi + " Tornado Shot 已追加（模板=" + skillData_Sample_Father.IndexName + "）");
	}

	private static void InjectCyclone(TalentManager tm, int xi)
	{
		Dictionary<string, SkillData_Sample_Father> sample_F = tm.XiData[xi].Sample_F;
		if (sample_F.ContainsKey("Cyclone"))
		{
			RegisterData(tm, xi, "Cyclone");
			return;
		}
		SkillData_Sample_Father skillData_Sample_Father = null;
		foreach (KeyValuePair<string, SkillData_Sample_Father> item in sample_F)
		{
			if (item.Value != null && (item.Value.FStype == 7 || item.Value.FStype == 8 || item.Value.FStype == 9))
			{
				skillData_Sample_Father = item.Value;
				break;
			}
		}
		if (skillData_Sample_Father == null)
		{
			Debug.LogWarning("[PoedbSkillInjector] Xi=" + xi + " 无环绕型（FStype 7/8/9）技能模板，跳过 Cyclone");
			return;
		}
		SkillData_Sample_Father skillData_Sample_Father2 = CloneFather(skillData_Sample_Father);
		skillData_Sample_Father2.IndexName = "Cyclone";
		skillData_Sample_Father2.Info = "info_Cyclone";
		skillData_Sample_Father2.Xi = xi;
		skillData_Sample_Father2.Price = 0;
		skillData_Sample_Father2.UnLock_Point = 0;
		skillData_Sample_Father2.Level_Max = 4;
		skillData_Sample_Father2.LastSkill = false;
		skillData_Sample_Father2.ManaCost_Base = 10f;
		skillData_Sample_Father2.CoolDown_Base = 5f;
		skillData_Sample_Father2.Damage_Base = 80f;
		skillData_Sample_Father2.Damage_Level = 20f;
		sample_F["Cyclone"] = skillData_Sample_Father2;
		RegisterData(tm, xi, "Cyclone");
		Debug.Log("[PoedbSkillInjector] Xi=" + xi + " Cyclone 已追加（模板=" + skillData_Sample_Father.IndexName + "）");
	}

	private static void RegisterData(TalentManager tm, int xi, string skillName)
	{
		try
		{
			tm.SKI[skillName] = new SKindex
			{
				Xi = xi,
				type = 0
			};
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbSkillInjector] SKI 注册异常 " + skillName + ": " + ex.Message);
		}
		try
		{
			SkillData_Sample_Father skillData_Sample_Father = tm.XiData[xi].Sample_F[skillName];
			if (skillData_Sample_Father != null)
			{
				AddToFW(tm, skillData_Sample_Father);
			}
		}
		catch (Exception ex2)
		{
			Debug.LogWarning("[PoedbSkillInjector] FW 注册异常 " + skillName + ": " + ex2.Message);
		}
	}

	private static SkillData_Sample_Father CloneFather(SkillData_Sample_Father src)
	{
		SkillData_Sample_Father skillData_Sample_Father = new SkillData_Sample_Father();
		CopyAllFields(typeof(SkillData), src, skillData_Sample_Father);
		CopyAllFields(typeof(SkillData_Sample_Father), src, skillData_Sample_Father);
		skillData_Sample_Father.IndexName = src.IndexName;
		skillData_Sample_Father.Level_Base = 0;
		skillData_Sample_Father.Level_WeaponOn = 0;
		skillData_Sample_Father.skillbt = null;
		return skillData_Sample_Father;
	}

	private static void CopyAllFields(Type type, SkillData_Sample_Father src, SkillData_Sample_Father dst)
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			try
			{
				fieldInfo.SetValue(dst, fieldInfo.GetValue(src));
			}
			catch
			{
			}
		}
	}

	private static void AddToFW(TalentManager tm, SkillData_Sample_Father skill)
	{
		tm.EnsureSkillFWLibrary();
		if (tm.FW == null || tm.FW.Char == null)
		{
			return;
		}
		int num = skill.Xi / 3;
		int num2 = skill.Xi % 3;
		if (num < 0 || num >= tm.FW.Char.Length || num2 < 0 || num2 >= tm.FW.Char[num].Xi.Length)
		{
			return;
		}
		SKFW_Xi sKFW_Xi = tm.FW.Char[num].Xi[num2];
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
		sKFW_Xi.FW[sKFW_Xi.FW.Length - 1] = new SKFW
		{
			PLtype = num,
			Xi = num2,
			Price = 0,
			type = 0,
			EL = (int)skill.damageType,
			index = 0,
			SkillName = skill.IndexName
		};
	}

	public static void TryEnsureButtons(TalentManager tm)
	{
		try
		{
			if (tm == null)
			{
				return;
			}
			TryInjectData(tm);
			if (tm.XiData == null)
			{
				return;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < LandingXis.Length; i++)
			{
				int num = LandingXis[i];
				if (num >= 0 && num < tm.XiData.Length && tm.XiData[num] != null && tm.XiData[num].Sample_F != null)
				{
					List<Vector2> occupied = new List<Vector2>();
					if (!EnsureButton(tm, num, "Tornado Shot", occupied))
					{
						list.Add("Xi" + num + ":Tornado Shot");
					}
					if (!EnsureButton(tm, num, "Cyclone", occupied))
					{
						list.Add("Xi" + num + ":Cyclone");
					}
				}
			}
			if (list.Count == 0)
			{
				Debug.Log("[PoedbSkillInjector] 节点按钮保障完成 8/8 就绪");
			}
			else
			{
				Debug.LogWarning("[PoedbSkillInjector] 节点按钮缺失（下次打开面板重试）: " + string.Join(" | ", list.ToArray()));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PoedbSkillInjector] TryEnsureButtons 异常: " + ex);
		}
	}

	private static bool EnsureButton(TalentManager tm, int xi, string skillName, List<Vector2> occupied)
	{
		try
		{
			if (FindButton(tm, xi, skillName) != null)
			{
				return true;
			}
			if (!tm.XiData[xi].Sample_F.ContainsKey(skillName))
			{
				return false;
			}
			SkillBT skillBT = FindTemplateButton(tm, xi, skillName);
			if (skillBT == null)
			{
				Debug.LogWarning("[PoedbSkillInjector] 按钮克隆待重试 Xi=" + xi + " " + skillName + "（本页暂无模板格，面板未就绪）");
				return false;
			}
			_ = FindFirstFatherButton(tm, xi) == null;
			Transform parent = skillBT.transform.parent;
			GameObject gameObject = ((parent != null && parent.Find("Text") != null) ? parent.gameObject : skillBT.gameObject);
			Transform parent2 = gameObject.transform.parent;
			if (parent2 == null)
			{
				return false;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, parent2, worldPositionStays: false);
			gameObject2.name = "SkillBT_" + skillName + "_Xi" + xi;
			SkillBT skillBT2 = gameObject2.GetComponent<SkillBT>();
			if (skillBT2 == null)
			{
				skillBT2 = gameObject2.GetComponentInChildren<SkillBT>();
			}
			if (skillBT2 == null)
			{
				UnityEngine.Object.Destroy(gameObject2);
				Debug.LogWarning("[PoedbSkillInjector] 克隆体丢失 SkillBT，已销毁 Xi=" + xi + " " + skillName);
				return false;
			}
			RewireChildRefs(gameObject, gameObject2, skillBT2, skillBT);
			skillBT2.IndexName = skillName;
			skillBT2.Xi = xi;
			skillBT2.SkillType = 0;
			TryApplyLocalIcon(skillBT2, skillName);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			if (component != null && component2 != null)
			{
				if (TryFindFreeSlot(tm, xi, component, parent2 as RectTransform, occupied, out var slot))
				{
					component2.anchoredPosition = slot;
				}
				else
				{
					component2.anchoredPosition = component.anchoredPosition + new Vector2(0f, -170f);
				}
				occupied.Add(component2.anchoredPosition);
			}
			if (!gameObject2.activeSelf)
			{
				gameObject2.SetActive(value: true);
			}
			Debug.Log("[PoedbSkillInjector] 节点按钮已克隆 Xi=" + xi + " " + skillName + " pos=" + ((component2 != null) ? component2.anchoredPosition.ToString() : "n/a"));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbSkillInjector] EnsureButton 异常 Xi=" + xi + " " + skillName + ": " + ex.Message);
			return false;
		}
	}

	private static bool TryFindFreeSlot(TalentManager tm, int xi, RectTransform anchorRt, RectTransform parentRt, List<Vector2> occupied, out Vector2 slot)
	{
		slot = anchorRt.anchoredPosition;
		List<Vector2> list = new List<Vector2>();
		foreach (SkillBT item in EnumerateSkillBTs(tm))
		{
			if ((bool)item && item.Xi == xi && item.SkillType == 0)
			{
				RectTransform component = item.gameObject.GetComponent<RectTransform>();
				if (component != null)
				{
					list.Add(component.anchoredPosition);
				}
			}
		}
		if (occupied != null)
		{
			list.AddRange(occupied);
		}
		Vector2 anchoredPosition = anchorRt.anchoredPosition;
		float num = ((parentRt != null && parentRt.rect.width >= 10f) ? (parentRt.rect.width * 0.5f) : 480f);
		float num2 = ((parentRt != null && parentRt.rect.height >= 10f) ? (parentRt.rect.height * 0.5f) : 700f);
		float num3 = 2025f;
		for (int i = 0; i < SlotCandidatesX.Length; i++)
		{
			Vector2 vector = anchoredPosition + new Vector2((float)SlotCandidatesX[i] * 155f, (float)SlotCandidatesY[i] * 170f);
			if (Mathf.Abs(vector.x) > num - 60f || Mathf.Abs(vector.y) > num2 - 60f)
			{
				continue;
			}
			bool flag = true;
			foreach (Vector2 item2 in list)
			{
				if ((item2 - vector).sqrMagnitude < num3)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				slot = vector;
				return true;
			}
		}
		return false;
	}

	private static void RewireChildRefs(GameObject srcGo, GameObject cloneGo, SkillBT cloneBt, SkillBT srcBt)
	{
		Transform transform = null;
		if (srcBt.text != null)
		{
			string text = RelativePathUnder(srcBt.text.transform, srcGo.transform);
			transform = ((text != null) ? cloneGo.transform.Find(text) : null);
		}
		if (transform == null)
		{
			transform = cloneGo.transform.Find("Text");
		}
		if (transform != null)
		{
			cloneBt.text = transform.GetComponent<Text>();
		}
		if (srcBt.SkillTU != null)
		{
			string text2 = RelativePathUnder(srcBt.SkillTU.transform, srcGo.transform);
			Transform transform2 = ((text2 != null) ? cloneGo.transform.Find(text2) : null);
			if (transform2 != null)
			{
				cloneBt.SkillTU = transform2.GetComponent<Image>();
			}
		}
	}

	private static SkillBT FindButton(TalentManager tm, int xi, string skillName)
	{
		foreach (SkillBT item in EnumerateSkillBTs(tm))
		{
			if ((bool)item && item.Xi == xi && item.SkillType == 0 && item.IndexName == skillName)
			{
				return item;
			}
		}
		return null;
	}

	private static SkillBT FindTemplateButton(TalentManager tm, int xi, string skillName)
	{
		foreach (SkillBT item in EnumerateSkillBTs(tm))
		{
			if ((bool)item && item.Xi == xi && item.SkillType == 0 && item.IndexName != skillName && item.IndexName != "Tornado Shot" && item.IndexName != "Cyclone")
			{
				return item;
			}
		}
		return null;
	}

	private static IEnumerable<SkillBT> EnumerateSkillBTs(TalentManager tm)
	{
		if (tm != null)
		{
			SkillBT[] componentsInChildren = tm.GetComponentsInChildren<SkillBT>(includeInactive: true);
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				SkillBT[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					yield return array[i];
				}
				yield break;
			}
		}
		SkillBT[] array2 = UnityEngine.Object.FindObjectsOfType<SkillBT>();
		if (array2 != null)
		{
			SkillBT[] array = array2;
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[i];
			}
		}
		foreach (SkillBT item in EnumerateAllSkillBTsSafe())
		{
			yield return item;
		}
	}

	private static IEnumerable<SkillBT> EnumerateAllSkillBTsSafe()
	{
		SkillBT[] array = null;
		try
		{
			array = Resources.FindObjectsOfTypeAll<SkillBT>();
		}
		catch
		{
		}
		if (array == null)
		{
			yield break;
		}
		SkillBT[] array2 = array;
		foreach (SkillBT skillBT in array2)
		{
			if ((bool)skillBT && skillBT.gameObject != null && skillBT.gameObject.scene.IsValid())
			{
				yield return skillBT;
			}
		}
	}

	private static SkillBT FindFirstFatherButton(TalentManager tm, int xi)
	{
		foreach (KeyValuePair<string, SkillData_Sample_Father> item in tm.XiData[xi].Sample_F)
		{
			if (item.Value != null && !(item.Key == "Tornado Shot") && !(item.Key == "Cyclone"))
			{
				SkillBT skillBT = FindButton(tm, xi, item.Key);
				if (skillBT != null)
				{
					return skillBT;
				}
			}
		}
		return null;
	}

	private static string RelativePathUnder(Transform node, Transform ancestor)
	{
		if (node == null || ancestor == null)
		{
			return null;
		}
		List<string> list = new List<string>();
		Transform transform = node;
		while (transform != null && transform != ancestor)
		{
			list.Insert(0, transform.name);
			transform = transform.parent;
		}
		if (transform != ancestor)
		{
			return null;
		}
		return string.Join("/", list.ToArray());
	}

	private static void TryApplyLocalIcon(SkillBT bt, string skillName)
	{
		try
		{
			if (bt == null || bt.SkillTU == null)
			{
				return;
			}
			Sprite sprite = TryLoadPoedbSprite(skillName);
			if (sprite == null)
			{
				return;
			}
			bt.SkillTU.sprite = sprite;
			TalentManager instance = SingletonMonoScope<TalentManager>.Instance;
			if (instance != null && instance.XiData != null && bt.Xi >= 0 && bt.Xi < instance.XiData.Length && instance.XiData[bt.Xi] != null && instance.XiData[bt.Xi].Sample_F.TryGetValue(skillName, out var value) && value != null)
			{
				value.icon = sprite;
				if (value.iconB == null)
				{
					value.iconB = sprite;
				}
			}
			Debug.Log("[PoedbSkillInjector] 图标已替换(本地文件) " + skillName);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbSkillInjector] TryApplyLocalIcon 异常: " + ex.Message);
		}
	}

	private static Sprite TryLoadPoedbSprite(string skillName)
	{
		string text = ((skillName == "Tornado Shot") ? "Tornado_Shot.png" : "Cyclone.png");
		string path = "poedb/icons/" + Path.GetFileNameWithoutExtension(text);
		try
		{
			Sprite sprite = Resources.Load<Sprite>(path);
			if (sprite != null)
			{
				return sprite;
			}
		}
		catch
		{
		}
		string path2 = "data/poedb/icons/" + text;
		string path3 = Path.Combine(Directory.GetCurrentDirectory(), path2);
		if (!File.Exists(path3))
		{
			return null;
		}
		try
		{
			byte[] data = File.ReadAllBytes(path3);
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
			if (texture2D.LoadImage(data))
			{
				texture2D.filterMode = FilterMode.Bilinear;
				return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
			}
		}
		catch
		{
		}
		return null;
	}

	private static void InjectLocalizationFallback()
	{
		if (_locInjected)
		{
			return;
		}
		try
		{
			LOC mM = LOC.MM;
			if (mM != null)
			{
				FieldInfo field = typeof(LOC).GetField("_table", BindingFlags.Instance | BindingFlags.NonPublic);
				if (!(field == null) && field.GetValue(mM) is Dictionary<string, Dictionary<LanguageType, string>> table)
				{
					Put(table, "Tornado Shot", "Tornado Shot", "龙卷射击");
					Put(table, "info_Tornado Shot", "Fire a piercing arrow that flies straight and pierces through every enemy in its path. (Damage 100% +30%/Lv, Mana 8, Cooldown 1.2s)", "发射一支穿透箭矢，笔直飞行并贯穿路径上的所有敌人。（伤害 100% +30%/级，蓝耗 8，冷却 1.2 秒）");
					Put(table, "Cyclone", "Cyclone", "旋风斩");
					Put(table, "info_Cyclone", "Summon a whirlwind that orbits around you and continuously damages nearby enemies. (Damage 80% +20%/Lv, Mana 10, Cooldown 5s)", "召唤环绕自身旋转的旋风，持续对周围敌人造成伤害。（伤害 80% +20%/级，蓝耗 10，冷却 5 秒）");
					_locInjected = true;
					Debug.Log("[PoedbSkillInjector] 本地化 fallback 已注入 Tornado Shot / Cyclone");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[PoedbSkillInjector] InjectLocalizationFallback 异常: " + ex.Message);
		}
	}

	private static void Put(Dictionary<string, Dictionary<LanguageType, string>> table, string key, string en, string zh)
	{
		string key2 = "Skill_FY." + key;
		if (!table.TryGetValue(key2, out var value))
		{
			value = (table[key2] = new Dictionary<LanguageType, string>());
		}
		value[LanguageType.English] = en;
		value[LanguageType.ChineseS] = zh;
		value[LanguageType.ChineseT] = zh;
		if (!table.TryGetValue(key, out var value2))
		{
			value2 = (table[key] = new Dictionary<LanguageType, string>());
		}
		value2[LanguageType.English] = en;
		value2[LanguageType.ChineseS] = zh;
		value2[LanguageType.ChineseT] = zh;
	}
}
