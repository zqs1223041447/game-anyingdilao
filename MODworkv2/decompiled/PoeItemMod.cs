using System;
using System.Collections.Generic;
using System.Reflection;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Localization;
using UnityEngine;

public static class PoeItemMod
{
	public const int FlaskGaleId = 91001;

	public const string FlaskGaleName = "PoeFlaskGale";

	public const string FlaskGaleUseType = "poe_flask_gale";

	public const int FlaskInsightId = 91002;

	public const string FlaskInsightName = "PoeFlaskInsight";

	public const string FlaskInsightUseType = "poe_flask_insight";

	public const int RingId = 91003;

	public const string RingItem = "PoeRingNova";

	public const int ChainId = 91004;

	public const string ChainItem = "PoeChainEcho";

	public const int JewelId = 91005;

	public const string JewelItem = "PoeJewelVolley";

	public const string JewelBStype = "projectile";

	public const int JewelSocketType = 26;

	public const int RingBonusProjectiles = 4;

	public const string SpecialModColor = "#00E5FF";

	private const int NoDropLevel = 999;

	private static readonly Dictionary<string, string[]> Descriptions = new Dictionary<string, string[]>
	{
		{
			"PoeFlaskGale",
			new string[2] { "Repeatable functional flask. On drink: +40% Movement Speed for 5s. Cooldown 4s. Not consumed.", "可重复饮用的功能药剂。饮用后移动速度 +40%，持续 5 秒；冷却 4 秒，不消耗瓶身。" }
		},
		{
			"PoeFlaskInsight",
			new string[2] { "Repeatable functional flask. On drink: +30% Critical Chance for 6s. Cooldown 6s. Not consumed.", "可重复饮用的功能药剂。饮用后暴击几率 +30%，持续 6 秒；冷却 6 秒，不消耗瓶身。" }
		},
		{
			"PoeRingNova",
			new string[2] { "Legendary Ring: skills fire 4 additional projectiles, and all your projectiles are evenly spread in a full 360-degree circle from the point of origin.", "传奇戒指：技能额外发射 4 枚投射物，你的全部投射物以完整 360° 圆环从发射点均匀射出。" }
		},
		{
			"PoeChainEcho",
			new string[2] { "Legendary Amulet: your projectiles gain +1 pierce; they return to you when pierce is exhausted, on a killing hit or at max range, hitting enemies on both the outbound and returning path.", "传奇项链：投射物获得 +1 次穿透；在穿透耗尽、命中消耗或到达最远距离后返回你身边，去程与返程均可命中敌人。" }
		},
		{
			"PoeJewelVolley",
			new string[2] { "Socketable Jewel: +1 to number of all projectiles. (Single-body projectiles gain 1 extra; barrage volleys gain +1 projectile count)", "镶嵌珠宝：所有投射物数量 +1。（单体投射物额外 +1 发；弹幕类技能弹数 +1）" }
		}
	};

	public static string LastCastInfo = "本局尚未出手";

	private static float _nextEquipDump;

	private static bool _locInjected;

	public static bool RingEquipped => IsEquipped("PoeRingNova");

	public static bool ReturnEquipped => IsEquipped("PoeChainEcho");

	public static bool HasJewelSocketed => ExtraProjectiles() > 0;

	public static bool IsEquipped(string itemName)
	{
		try
		{
			int num = GlobalIdFor(itemName);
			if (!SingletonMonoScope<InventoryManager>.HasInstance)
			{
				return false;
			}
			CharButton[] charBT = SingletonMonoScope<InventoryManager>.Instance.CharBT;
			if (charBT == null)
			{
				return false;
			}
			for (int i = 0; i < charBT.Length; i++)
			{
				if (((UnityEngine.Object)(object)charBT[i] != null) && charBT[i].weapon != null && (charBT[i].weapon.ItemName == itemName || (num != 0 && charBT[i].weapon.GlobalID == num)))
				{
					return true;
				}
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	private static int GlobalIdFor(string itemName)
	{
		if (itemName == "PoeRingNova")
		{
			return 91003;
		}
		if (itemName == "PoeChainEcho")
		{
			return 91004;
		}
		return 0;
	}

	public static int ExtraProjectiles()
	{
		try
		{
			if (!SingletonMonoScope<PlayerManager>.HasInstance)
			{
				return 0;
			}
			return Mathf.Max(0, Mathf.RoundToInt(SingletonMonoScope<PlayerManager>.Instance.BS_ExtraProjectiles));
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public static bool IsRepeatableFlask(UseItemClass it)
	{
		if (it != null)
		{
			if (!(it.UseType == "poe_flask_gale"))
			{
				return it.UseType == "poe_flask_insight";
			}
			return true;
		}
		return false;
	}

	public static bool TryGetEquipDiagnostics(string itemName, int globalId, out string diag)
	{
		diag = null;
		bool flag = itemName == "PoeRingNova" || (globalId == 91003 && globalId != 0);
		bool flag2 = itemName == "PoeChainEcho" || (globalId == 91004 && globalId != 0);
		if (!flag && !flag2)
		{
			return false;
		}
		try
		{
			int num = 0;
			int num2 = 0;
			string text = "无此槽";
			string text2 = "无此槽";
			bool flag3 = false;
			bool flag4 = false;
			if (SingletonMonoScope<InventoryManager>.HasInstance)
			{
				CharButton[] charBT = SingletonMonoScope<InventoryManager>.Instance.CharBT;
				if (charBT != null)
				{
					num = charBT.Length;
					foreach (CharButton charButton in charBT)
					{
						if (((UnityEngine.Object)(object)charButton != null) && charButton.weapon != null)
						{
							num2++;
							if (charButton.charType == 7)
							{
								text = charButton.weapon.ItemName + "(gid=" + charButton.weapon.GlobalID + ")";
								flag3 = charButton.weapon.GlobalID == 91003 || charButton.weapon.ItemName == "PoeRingNova";
							}
							else if (charButton.charType == 6)
							{
								text2 = charButton.weapon.ItemName + "(gid=" + charButton.weapon.GlobalID + ")";
								flag4 = charButton.weapon.GlobalID == 91004 || charButton.weapon.ItemName == "PoeChainEcho";
							}
						}
					}
				}
			}
			string text3 = (((flag && flag3) || (flag2 && flag4)) ? "[MOD] ✓ 已穿戴生效" : ("[MOD] ⚠ 未穿戴（在背包中）——右键装备到" + (flag ? "戒指" : "项链") + "槽后生效"));
			diag = $"{text3} ｜ 装备槽: CharBT={num}(有货{num2}) ring={flag3} chain={flag4} 槽6={text2} 槽7={text}\n上次出手: {LastCastInfo}";
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool TryGetDescription(string itemName, out string text)
	{
		text = null;
		if (string.IsNullOrEmpty(itemName) || !Descriptions.TryGetValue(itemName, out var value))
		{
			return false;
		}
		bool flag = true;
		try
		{
			flag = LOC.MM == null || LOC.MM.CurrentLanguage != LanguageType.English;
		}
		catch (Exception)
		{
		}
		text = (flag ? value[1] : value[0]);
		return true;
	}

	public static void SpawnExtraProjectiles(Gun gun, SkillOBJ_DT_SP dt)
	{
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		string text = "?";
		try
		{
			text = (string.IsNullOrEmpty(dt.skillName) ? "?" : dt.skillName);
		}
		catch (Exception)
		{
		}
		try
		{
			if ((UnityEngine.Object)(object)gun == (UnityEngine.Object)null || (UnityEngine.Object)(object)dt == (UnityEngine.Object)null)
			{
				NoteCast("[" + text + "] gun/dt 为空，未处理");
				return;
			}
			bool ringEquipped = RingEquipped;
			int num = ExtraProjectiles();
			int num2 = (ringEquipped ? 4 : 0);
			LogUtil.Info("[PoeItemMod] cast: ring=" + ringEquipped + " jewelExtra=" + num + " ringBonus=" + num2, false);
			if (!ringEquipped)
			{
				DumpEquippedOnce();
			}
			if (num2 <= 0 && num <= 0)
			{
				NoteCast("[" + text + "] 无加成（星环/珠宝均未生效）");
				return;
			}
			int num3 = 0;
			try
			{
				num3 = Mathf.Max(1, dt.Count_F);
			}
			catch (Exception)
			{
				num3 = 1;
			}
			int num4 = num + num2;
			int num5 = num3 + num4;
			float num6 = BaseAimAngle(dt);
			LogUtil.Info("[PoeItemMod] cast spread: ring=" + ringEquipped + " nativeCountF=" + num3 + " bonus=" + num4 + " totalBullets=" + num5, false);
			Vector3 position = ((Component)dt).transform.position;
			Vector3 targetPos = dt.TargetPos;
			float num7 = Vector3.Distance(position, targetPos);
			if (ringEquipped)
			{
				int num8 = 0;
				if (num4 == 4 && num3 == 3)
				{
					float[] array = new float[4] { 51.4f, 102.9f, 257.1f, 308.6f };
					foreach (float num9 in array)
					{
						SkillOBJ_DT_SP skillOBJ_DT_SP = gun.CreatSP();
						if (!((UnityEngine.Object)(object)skillOBJ_DT_SP == (UnityEngine.Object)null))
						{
							num8++;
							float num10 = num6 + num9;
							((Component)skillOBJ_DT_SP).transform.position = position;
							((Component)skillOBJ_DT_SP).transform.rotation = Quaternion.Euler(0f, 0f, num10);
							skillOBJ_DT_SP.dic = new Vector2(Mathf.Cos(num10 * 3.14f / 180f), Mathf.Sin(num10 * 3.14f / 180f));
							skillOBJ_DT_SP.TargetPos = ((num7 > 0.1f) ? (position + new Vector3(skillOBJ_DT_SP.dic.x, skillOBJ_DT_SP.dic.y, 0f) * num7) : targetPos);
						}
					}
				}
				else
				{
					float num11 = 360f / (float)num5;
					int num12 = 0;
					int num13 = 0;
					while (num12 < num4 && num13 < 720)
					{
						num13++;
						float num14 = num6 + num11 * (float)num13;
						bool flag = false;
						for (int j = 0; j < num3; j++)
						{
							if (Mathf.Abs(Mathf.DeltaAngle(num6 + 360f * (float)j / (float)num3, num14)) < num11 * 0.49f)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							SkillOBJ_DT_SP skillOBJ_DT_SP2 = gun.CreatSP();
							if (!((UnityEngine.Object)(object)skillOBJ_DT_SP2 == (UnityEngine.Object)null) && SpawnsRingForSkill(skillOBJ_DT_SP2))
							{
								num12++;
								num8++;
								((Component)skillOBJ_DT_SP2).transform.position = position;
								((Component)skillOBJ_DT_SP2).transform.rotation = Quaternion.Euler(0f, 0f, num14);
								skillOBJ_DT_SP2.dic = new Vector2(Mathf.Cos(num14 * 3.14f / 180f), Mathf.Sin(num14 * 3.14f / 180f));
								skillOBJ_DT_SP2.TargetPos = ((num7 > 0.1f) ? (position + new Vector3(skillOBJ_DT_SP2.dic.x, skillOBJ_DT_SP2.dic.y, 0f) * num7) : targetPos);
							}
						}
					}
					while (num12 < num4)
					{
						SkillOBJ_DT_SP skillOBJ_DT_SP3 = gun.CreatSP();
						if ((UnityEngine.Object)(object)skillOBJ_DT_SP3 == (UnityEngine.Object)null)
						{
							break;
						}
						float num15 = num6 + 360f * (float)num12 / (float)num4;
						num12++;
						num8++;
						((Component)skillOBJ_DT_SP3).transform.position = position;
						((Component)skillOBJ_DT_SP3).transform.rotation = Quaternion.Euler(0f, 0f, num15);
						skillOBJ_DT_SP3.dic = new Vector2(Mathf.Cos(num15 * 3.14f / 180f), Mathf.Sin(num15 * 3.14f / 180f));
						skillOBJ_DT_SP3.TargetPos = ((num7 > 0.1f) ? (position + new Vector3(skillOBJ_DT_SP3.dic.x, skillOBJ_DT_SP3.dic.y, 0f) * num7) : targetPos);
					}
				}
				NoteCast("[" + text + "] ring=True 已生成 " + num8 + "/" + num4 + " 枚追加弹（360° 环状，总画幅 " + num5 + " 弹）");
				return;
			}
			int num16 = 0;
			for (int k = 0; k < num4; k++)
			{
				SkillOBJ_DT_SP skillOBJ_DT_SP4 = gun.CreatSP();
				if (!((UnityEngine.Object)(object)skillOBJ_DT_SP4 == (UnityEngine.Object)null))
				{
					num16++;
					((Component)skillOBJ_DT_SP4).transform.position = position;
					((Component)skillOBJ_DT_SP4).transform.rotation = ((Component)dt).transform.rotation;
					skillOBJ_DT_SP4.dic = dt.dic;
					skillOBJ_DT_SP4.TargetPos = targetPos;
				}
			}
			NoteCast("[" + text + "] ring=False 已生成 " + num16 + "/" + num4 + " 枚追加弹（同向补射）");
		}
		catch (Exception ex3)
		{
			NoteCast("[" + text + "] 异常: " + ex3.Message);
		}
	}

	private static void NoteCast(string msg)
	{
		LastCastInfo = msg;
		LogUtil.Info("[PoeItemMod] cast-note: " + msg, false);
	}

	private static float BaseAimAngle(SkillOBJ_DT_SP dt)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (dt.RTtypeOBJ == 1 && dt.dic.sqrMagnitude > 0.0001f)
			{
				return Mathf.Atan2(dt.dic.y, dt.dic.x) * 57.29578f;
			}
			return ((Component)dt).transform.eulerAngles.z;
		}
		catch (Exception)
		{
			return 0f;
		}
	}

	private static bool SpawnsRingForSkill(SkillOBJ_DT_SP sp)
	{
		try
		{
			if ((UnityEngine.Object)(object)sp == (UnityEngine.Object)null)
			{
				return false;
			}
			if (sp.FStype == 3)
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return true;
		}
	}

	private static void DumpEquippedOnce()
	{
		try
		{
			if (Time.time < _nextEquipDump)
			{
				return;
			}
			_nextEquipDump = Time.time + 5f;
			if (!SingletonMonoScope<InventoryManager>.HasInstance)
			{
				LogUtil.Info("[PoeItemMod] equip-dump: no InventoryManager instance", false);
				return;
			}
			CharButton[] charBT = SingletonMonoScope<InventoryManager>.Instance.CharBT;
			if (charBT == null)
			{
				LogUtil.Info("[PoeItemMod] equip-dump: CharBT is null", false);
				return;
			}
			for (int i = 0; i < charBT.Length; i++)
			{
				if (((UnityEngine.Object)(object)charBT[i] != null) && charBT[i].hasWeapon && charBT[i].weapon != null)
				{
					LogUtil.Info("[PoeItemMod] equip-dump slot" + i + " = " + charBT[i].weapon.ItemName + " (GlobalID=" + charBT[i].weapon.GlobalID + ")", false);
				}
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] equip-dump failed: " + ex.Message);
		}
	}

	public static void TryRegisterWeaponRows(ItemManager im)
	{
		try
		{
			if (!((UnityEngine.Object)(object)im == (UnityEngine.Object)null) && im.Weapon != null && im.Weapon.GP != null && (FindRow(im, 91003) == null || FindRow(im, 91004) == null))
			{
				int iconType = 0;
				int icon = 0;
				int iconType2 = 0;
				int icon2 = 0;
				int soundDrop = 0;
				int soundUse = 0;
				int soundDrop2 = 0;
				int soundUse2 = 0;
				Item_MB item_MB = FindAccessoryDonor(im, 7);
				Item_MB item_MB2 = FindAccessoryDonor(im, 6);
				if (item_MB != null)
				{
					iconType = item_MB.IconType;
					icon = item_MB.Icon;
					soundDrop = item_MB.SoundDrop;
					soundUse = item_MB.SoundUse;
				}
				if (item_MB2 != null)
				{
					iconType2 = item_MB2.IconType;
					icon2 = item_MB2.Icon;
					soundDrop2 = item_MB2.SoundDrop;
					soundUse2 = item_MB2.SoundUse;
				}
				if (FindRow(im, 91003) == null)
				{
					AddRow(im, BuildAccessoryRow(91003, "PoeRingNova", 7, iconType, icon, soundDrop, soundUse));
				}
				if (FindRow(im, 91004) == null)
				{
					AddRow(im, BuildAccessoryRow(91004, "PoeChainEcho", 6, iconType2, icon2, soundDrop2, soundUse2));
				}
				LogUtil.Info("[PoeItemMod] weapon rows registered (ring/chain)", false);
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] TryRegisterWeaponRows failed: " + ex.Message);
		}
	}

	private static Item_MB BuildAccessoryRow(int globalId, string name, int charType, int iconType, int icon, int soundDrop, int soundUse)
	{
		return new Item_MB
		{
			ItemName = name,
			GlobalID = globalId,
			ItemType = 0,
			DropLevelStart = 999,
			Quality = 5,
			SizeX = 1,
			SizeY = 1,
			MaxAocaoCount = 1,
			CurAocaoCount = 1,
			IconType = iconType,
			Icon = icon,
			SoundDrop = soundDrop,
			SoundUse = soundUse,
			RotateType = 0,
			PLtype = 0,
			WeaponType = "little",
			CharType = charType,
			Damage = 0f,
			Health = 0f,
			Mana = 0f,
			Element = 0f,
			SkillA = "0",
			SkillA_count = 0,
			SkillB = "0",
			SkillB_count = 0,
			SkillC = "0",
			SkillC_count = 0,
			SkillD = "0",
			SkillD_count = 0,
			SkillE = "0",
			SkillE_count = 0,
			SkillF = "0",
			SkillF_count = 0,
			Set_Index = 0,
			WP_SkillCount = 0
		};
	}

	private static void AddRow(ItemManager im, Item_MB mb)
	{
		if (mb.PLtype < 0 || mb.PLtype >= im.Weapon.GP.Length || mb.CharType < 0 || mb.CharType >= im.Weapon.GP[0].QL.Length)
		{
			return;
		}
		for (int i = 0; i < 4 && i < im.Weapon.GP.Length; i++)
		{
			Weapon_Group weapon_Group = im.Weapon.GP[i];
			if (weapon_Group != null && weapon_Group.QL != null && mb.CharType < weapon_Group.QL.Length && weapon_Group.QL[mb.CharType] != null)
			{
				weapon_Group.QL[mb.CharType].Legendary.Add(mb);
			}
		}
	}

	private static Item_MB FindRow(ItemManager im, int globalId)
	{
		if (im?.Weapon?.GP == null)
		{
			return null;
		}
		for (int i = 0; i < im.Weapon.GP.Length; i++)
		{
			Weapon_Group weapon_Group = im.Weapon.GP[i];
			if (weapon_Group?.QL == null)
			{
				continue;
			}
			for (int j = 0; j < weapon_Group.QL.Length; j++)
			{
				Quality_Group quality_Group = weapon_Group.QL[j];
				if (quality_Group != null)
				{
					Item_MB item_MB = FindIn(quality_Group.Legendary, globalId) ?? FindIn(quality_Group.Mythical, globalId) ?? FindIn(quality_Group.Epic, globalId) ?? FindIn(quality_Group.Exquisite, globalId) ?? FindIn(quality_Group.Rare, globalId) ?? FindIn(quality_Group.Magic, globalId) ?? FindIn(quality_Group.Normal, globalId);
					if (item_MB != null)
					{
						return item_MB;
					}
				}
			}
		}
		return null;
	}

	private static Item_MB FindIn(List<Item_MB> list, int globalId)
	{
		if (list == null)
		{
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && list[i].GlobalID == globalId)
			{
				return list[i];
			}
		}
		return null;
	}

	private static Item_MB FindAccessoryDonor(ItemManager im, int charType)
	{
		if (im?.Weapon?.GP == null)
		{
			return null;
		}
		for (int i = 0; i < im.Weapon.GP.Length; i++)
		{
			Weapon_Group weapon_Group = im.Weapon.GP[i];
			if (weapon_Group?.QL != null && charType < weapon_Group.QL.Length && weapon_Group.QL[charType] != null)
			{
				Quality_Group quality_Group = weapon_Group.QL[charType];
				Item_MB item_MB = FirstNonNull(quality_Group.Legendary) ?? FirstNonNull(quality_Group.Mythical) ?? FirstNonNull(quality_Group.Epic) ?? FirstNonNull(quality_Group.Exquisite) ?? FirstNonNull(quality_Group.Rare) ?? FirstNonNull(quality_Group.Magic) ?? FirstNonNull(quality_Group.Normal);
				if (item_MB != null)
				{
					return item_MB;
				}
			}
		}
		return null;
	}

	private static Item_MB FirstNonNull(List<Item_MB> list)
	{
		if (list == null)
		{
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				return list[i];
			}
		}
		return null;
	}

	public static void StageShopItems(ItemManager im)
	{
		try
		{
			if (!((UnityEngine.Object)(object)im == (UnityEngine.Object)null))
			{
				InjectLocFallback();
				ShopManager shopManager = (SingletonMonoScope<ShopManager>.HasInstance ? SingletonMonoScope<ShopManager>.Instance : null);
				if (!((UnityEngine.Object)(object)shopManager == (UnityEngine.Object)null))
				{
					StageEquip(im, shopManager, 91003);
					StageEquip(im, shopManager, 91004);
					StageFlask(im, shopManager, 91001);
					StageFlask(im, shopManager, 91002);
					StageJewel(im, shopManager);
					LogUtil.Info("[PoeItemMod] shop staged: ring/chain/flasks/jewel", false);
				}
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] StageShopItems failed: " + ex.Message);
		}
	}

	public static void VerifyShopStock(ShopManager shop)
	{
		try
		{
			if ((UnityEngine.Object)(object)shop == (UnityEngine.Object)null)
			{
				return;
			}
			ItemManager itemManager = (SingletonMonoScope<ItemManager>.HasInstance ? SingletonMonoScope<ItemManager>.Instance : null);
			if (!((UnityEngine.Object)(object)itemManager == (UnityEngine.Object)null))
			{
				bool flag = false;
				if (!IsStaged(shop, 91003))
				{
					LogUtil.Info("[PoeItemMod] restage ring", false);
					StageEquip(itemManager, shop, 91003);
					flag = true;
				}
				if (!IsStaged(shop, 91004))
				{
					LogUtil.Info("[PoeItemMod] restage chain", false);
					StageEquip(itemManager, shop, 91004);
					flag = true;
				}
				if (!IsStaged(shop, 91001))
				{
					LogUtil.Info("[PoeItemMod] restage flask gale", false);
					StageFlask(itemManager, shop, 91001);
					flag = true;
				}
				if (!IsStaged(shop, 91002))
				{
					LogUtil.Info("[PoeItemMod] restage flask insight", false);
					StageFlask(itemManager, shop, 91002);
					flag = true;
				}
				if (!IsStaged(shop, 91005))
				{
					LogUtil.Info("[PoeItemMod] restage jewel", false);
					StageJewel(itemManager, shop);
					flag = true;
				}
				if (flag)
				{
					LogUtil.Info("[PoeItemMod] shop stock verified/restaged", false);
				}
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] VerifyShopStock failed: " + ex.Message);
		}
	}

	private static bool IsStaged(ShopManager shop, int globalId)
	{
		try
		{
			List<SlotData> list = shop.MainPages[0]?.MainList;
			if (list == null)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				SlotData slotData = list[i];
				if (slotData == null)
				{
					continue;
				}
				switch (slotData.ItemType)
				{
				case 0:
					if (slotData.weapon != null && slotData.weapon.GlobalID == globalId)
					{
						return true;
					}
					break;
				case 1:
					if (slotData.baoshi != null && slotData.baoshi.GlobalID == globalId)
					{
						return true;
					}
					break;
				case 2:
					if (slotData.useitem != null && slotData.useitem.GlobalID == globalId)
					{
						return true;
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] IsStaged failed: " + ex.Message);
		}
		return false;
	}

	private static SlotData TakeBuySlot(ShopManager shop)
	{
		return shop.CheckEmptyBuy(new IntVector2(1, 1));
	}

	private static void StageEquip(ItemManager im, ShopManager shop, int globalId)
	{
		try
		{
			if (IsStaged(shop, globalId))
			{
				return;
			}
			Item_MB item_MB = FindRow(im, globalId);
			if (item_MB == null)
			{
				LogUtil.Error("[PoeItemMod] WP row missing for GlobalID=" + globalId);
				return;
			}
			SlotData slotData = TakeBuySlot(shop);
			if (slotData == null)
			{
				LogUtil.Error("[PoeItemMod] no empty buy slot for GlobalID=" + globalId);
				return;
			}
			slotData.ItemType = 0;
			int level = ((!SingletonMonoScope<PlayerManager>.HasInstance) ? 1 : Mathf.Max(1, Mathf.FloorToInt((float)SingletonMonoScope<PlayerManager>.Instance.Level)));
			im.SetWPdata(slotData.weapon, item_MB, level);
			slotData.weapon.Level = 1;
			slotData.weapon.Price = 0;
			shop.CreatWP(slotData);
			LogUtil.Info("[PoeItemMod] staged equip " + globalId + " size=" + slotData.weapon.Size.x + "x" + slotData.weapon.Size.y + " at " + slotData.GridPos.x + "," + slotData.GridPos.y, false);
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] StageEquip(" + globalId + ") failed: " + ex.Message);
		}
	}

	private static void StageFlask(ItemManager im, ShopManager shop, int globalId)
	{
		try
		{
			bool flag = globalId == 91001;
			UseItemClass useItemClass = FindFlaskDonor(im);
			if (useItemClass == null)
			{
				LogUtil.Error("[PoeItemMod] flask donor missing");
				return;
			}
			SlotData slotData = TakeBuySlot(shop);
			if (slotData == null)
			{
				LogUtil.Error("[PoeItemMod] no empty buy slot for flask " + globalId);
				return;
			}
			slotData.ItemType = 2;
			UseItemClass useitem = slotData.useitem;
			useitem.GlobalID = globalId;
			useitem.ItemType = useItemClass.ItemType;
			useitem.ItemName = (flag ? "PoeFlaskGale" : "PoeFlaskInsight");
			useitem.Price = 0;
			useitem.Quality = (flag ? 4 : 5);
			useitem.Size = useItemClass.Size;
			useitem.Icon = useItemClass.Icon;
			useitem.Level = 1;
			useitem.SoundDrop = useItemClass.SoundDrop;
			useitem.SoundUse = useItemClass.SoundUse;
			useitem.RotateType = useItemClass.RotateType;
			useitem.InfoType = 1;
			useitem.UseType = (flag ? "poe_flask_gale" : "poe_flask_insight");
			useitem.damageType = useItemClass.damageType;
			useitem.Number = (flag ? 40 : 30);
			useitem.CDTime = (flag ? 4f : 6f);
			useitem.Duration = (flag ? 5 : 6);
			useitem.MstackSize = 1;
			useitem.CstackSize = 1;
			useitem.DropSpriteSize = useItemClass.DropSpriteSize;
			shop.CreatUSE(slotData);
			LogUtil.Info("[PoeItemMod] staged flask " + globalId + " at " + slotData.GridPos.x + "," + slotData.GridPos.y, false);
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] StageFlask(" + globalId + ") failed: " + ex.Message);
		}
	}

	private static UseItemClass FindFlaskDonor(ItemManager im)
	{
		for (int i = 0; i < im.BuffPotion.Count; i++)
		{
			UseItemClass useItemClass = im.BuffPotion[i];
			if (useItemClass != null && useItemClass.UseType == "EL_Damage" && (UnityEngine.Object)(object)useItemClass.Icon != (UnityEngine.Object)null)
			{
				return useItemClass;
			}
		}
		if (im.BuffPotion.Count <= 0)
		{
			return null;
		}
		return im.BuffPotion[0];
	}

	private static void StageJewel(ItemManager im, ShopManager shop)
	{
		try
		{
			BaoshiClass baoshiClass = FindJewelDonor(im);
			if (baoshiClass == null)
			{
				LogUtil.Error("[PoeItemMod] jewel donor missing");
				return;
			}
			SlotData slotData = TakeBuySlot(shop);
			if (slotData == null)
			{
				LogUtil.Error("[PoeItemMod] no empty buy slot for jewel");
				return;
			}
			slotData.ItemType = 1;
			BaoshiClass baoshi = slotData.baoshi;
			baoshi.GlobalID = 91005;
			baoshi.ItemType = baoshiClass.ItemType;
			baoshi.ItemName = "PoeJewelVolley";
			baoshi.Price = 0;
			baoshi.Quality = 4;
			baoshi.Size = baoshiClass.Size;
			baoshi.Icon = baoshiClass.Icon;
			baoshi.Level = 1;
			baoshi.SoundDrop = baoshiClass.SoundDrop;
			baoshi.SoundUse = baoshiClass.SoundUse;
			baoshi.RotateType = baoshiClass.RotateType;
			baoshi.BStype = "projectile";
			baoshi.UseType = 0;
			baoshi.BS_Quality = 7;
			baoshi.Number = 1;
			baoshi.MstackSize = 999;
			baoshi.CstackSize = 1;
			baoshi.DropSpriteSize = baoshiClass.DropSpriteSize;
			shop.CreatBS(slotData);
			LogUtil.Info("[PoeItemMod] staged jewel at " + slotData.GridPos.x + "," + slotData.GridPos.y, false);
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] StageJewel failed: " + ex.Message);
		}
	}

	private static BaoshiClass FindJewelDonor(ItemManager im)
	{
		for (int i = 0; i < im.Baoshi.Count; i++)
		{
			BaoshiClass baoshiClass = im.Baoshi[i];
			if (baoshiClass != null && baoshiClass.UseType == 0 && (UnityEngine.Object)(object)baoshiClass.Icon != (UnityEngine.Object)null)
			{
				return baoshiClass;
			}
		}
		if (im.Baoshi.Count <= 0)
		{
			return null;
		}
		return im.Baoshi[0];
	}

	public static void InjectLocFallback()
	{
		if (_locInjected)
		{
			return;
		}
		try
		{
			LOC mM = LOC.MM;
			if (mM == null)
			{
				return;
			}
			FieldInfo field = typeof(LOC).GetField("_table", BindingFlags.Instance | BindingFlags.NonPublic);
			if (!(field == null) && field.GetValue(mM) is Dictionary<string, Dictionary<LanguageType, string>> dictionary)
			{
				if (dictionary.ContainsKey("Item_FY.PoeFlaskGale") && dictionary.ContainsKey("Item_FY.PoeRingNova"))
				{
					_locInjected = true;
					return;
				}
				Put(dictionary, "PoeFlaskGale", "Gale Flask", "疾风之瓶", "Repeatable functional flask. On drink: +40% Movement Speed for 5s. Cooldown 4s. Not consumed.", "可重复饮用的功能药剂。饮用后移动速度 +40%，持续 5 秒。冷却 4 秒，不消耗瓶身。");
				Put(dictionary, "PoeFlaskInsight", "Insight Flask", "洞悉之瓶", "Repeatable functional flask. On drink: +30% Critical Chance for 6s. Cooldown 6s. Not consumed.", "可重复饮用的功能药剂。饮用后暴击几率 +30%，持续 6 秒。冷却 6 秒，不消耗瓶身。");
				Put(dictionary, "PoeRingNova", "Ring of Nova", "星环之戒", "Legendary Ring: skills fire 4 additional projectiles, and all your projectiles are evenly spread in a full 360-degree circle.", "传奇戒指：技能额外发射 4 枚投射物，你的全部投射物以完整 360° 圆环从发射点均匀射出。");
				Put(dictionary, "PoeChainEcho", "Chain of Echo", "回响之链", "Legendary Amulet: projectiles gain +1 pierce and return at the end of flight, hitting on both the outbound and returning path.", "传奇项链：投射物获得 +1 次穿透；在穿透耗尽、命中消耗或到达最远距离后返回你身边，去程与返程均可命中敌人。");
				Put(dictionary, "PoeJewelVolley", "Jewel of Volley", "万箭之玉", "Socketable Jewel: +1 to number of all projectiles. (Single-body projectiles gain 1 extra; volley skills gain +1 projectile count)", "镶嵌珠宝：所有投射物数量 +1。（单体投射物额外 +1 发；弹幕类技能弹数 +1）");
				PutKey(dictionary, "Item_FY.PoeJewelVolley_Info", "All Projectiles +{0}", "所有投射物数量 +{0}");
				PutKey(dictionary, "PoeJewelVolley_Info", "All Projectiles +{0}", "所有投射物数量 +{0}");
				_locInjected = true;
				LogUtil.Info("[PoeItemMod] Item_FY fallback injected", false);
			}
		}
		catch (Exception ex)
		{
			LogUtil.Error("[PoeItemMod] InjectLocFallback failed: " + ex.Message);
		}
	}

	private static void Put(Dictionary<string, Dictionary<LanguageType, string>> table, string key, string en, string zh, string enInfo, string zhInfo)
	{
		PutKey(table, "Item_FY." + key, en, zh);
		PutKey(table, key, en, zh);
		PutKey(table, "Item_FY.info_" + key, enInfo, zhInfo);
		PutKey(table, "info_" + key, enInfo, zhInfo);
	}

	private static void PutKey(Dictionary<string, Dictionary<LanguageType, string>> table, string fullKey, string en, string zh)
	{
		if (!table.TryGetValue(fullKey, out var value))
		{
			value = (table[fullKey] = new Dictionary<LanguageType, string>());
		}
		value[LanguageType.English] = en;
		value[LanguageType.ChineseS] = zh;
		value[LanguageType.ChineseT] = zh;
	}
}
