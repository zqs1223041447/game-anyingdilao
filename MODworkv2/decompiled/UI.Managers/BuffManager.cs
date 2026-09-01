using System;
using System.Collections.Generic;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using UI.UIItems;
using UnityEngine;

namespace UI.Managers;

public class BuffManager : SingletonMonoScope<BuffManager>
{
	[Header("神殿buff默认持续时间")]
	public int TempleTime = 180;

	[Header("图标配置文件")]
	public IconData BuffIconData;

	[Header("Buff容器")]
	public GameObject buffList;

	[Header("是否进行排序")]
	public bool isSort;

	[HideInInspector]
	public List<BuffPotionItem> PotionList = new List<BuffPotionItem>();

	[HideInInspector]
	public List<BuffTempleItem> TempleList = new List<BuffTempleItem>();

	private const string potionPrefabPath = "UI/Components/Buff/BuffPotionItem";

	private const string templePrefabPath = "UI/Components/Buff/BuffTempleItem";

	private GameObject potionPrefab;

	private GameObject templePrefab;

	private float perSecondTime;

	public PlayerManager PL;

	public float TempleTimeLast => Mathf.FloorToInt((float)TempleTime + (float)TempleTime * SingletonMonoScope<PlayerManager>.Instance.BuffT_Temple / 100f);

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		if (!potionPrefab)
		{
			potionPrefab = Singleton<ResManager>.Instance.Load<GameObject>("UI/Components/Buff/BuffPotionItem");
			templePrefab = Singleton<ResManager>.Instance.Load<GameObject>("UI/Components/Buff/BuffTempleItem");
		}
		if (!buffList && SingletonMonoScope<ACTbar>.HasInstance)
		{
			buffList = SingletonMonoScope<ACTbar>.Instance.buffList;
		}
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Update()
	{
		perSecondTime += Time.deltaTime;
		if (!(perSecondTime >= 1f))
		{
			return;
		}
		if (PotionList.Count > 0)
		{
			int num = PotionList.Count;
			for (int i = 0; i < num; i++)
			{
				PotionList[i].remainTime -= 1f;
				PotionList[i].RefreshTime();
				if (PotionList[i].remainTime == 0f)
				{
					BuffPotionItem buffPotionItem = PotionList[i];
					buffPotionItem.DelBuff();
					PotionList.Remove(buffPotionItem);
					LeanPool.Despawn(buffPotionItem);
					num--;
					i--;
				}
			}
		}
		if (TempleList.Count > 0)
		{
			int num2 = TempleList.Count;
			for (int j = 0; j < num2; j++)
			{
				TempleList[j].remainTime -= 1f;
				TempleList[j].RefreshTime();
				if (TempleList[j].remainTime == 0f)
				{
					BuffTempleItem buffTempleItem = TempleList[j];
					buffTempleItem.DelBuff();
					TempleList.Remove(buffTempleItem);
					LeanPool.Despawn(buffTempleItem);
					num2--;
					j--;
				}
			}
		}
		RefreshSlotOrder();
		perSecondTime = 0f;
	}

	public void AddPotionBuff(UseItemClass it)
	{
		BuffPotionItem samePotion = GetSamePotion(it);
		if ((bool)samePotion)
		{
			samePotion.remainTime = it.DurationLast;
			samePotion.maxTime = it.DurationLast;
			samePotion.Cover();
			samePotion.RefreshTime();
		}
		else
		{
			BuffPotionItem component = LeanPool.Spawn(potionPrefab, buffList.transform).GetComponent<BuffPotionItem>();
			component.IndexName = it.ItemName;
			component.remainTime = it.DurationLast;
			component.maxTime = it.DurationLast;
			component.UseType = it.UseType;
			component.Number = it.Number;
			component.damageType = it.damageType;
			component.Init(GetPotionIcon(it.UseType, it.damageType));
			component.Cover();
			PotionList.Add(component);
		}
		RefreshSlotOrder();
	}

	public BuffPotionItem GetSamePotion(UseItemClass it)
	{
		foreach (BuffPotionItem potion in PotionList)
		{
			if (potion.IndexName == it.ItemName)
			{
				return potion;
			}
		}
		return null;
	}

	public Sprite GetPotionIcon(string UseType, DamageType damageType)
	{
		switch (UseType)
		{
		case "EL_Damage":
			return damageType switch
			{
				DamageType.fire => BuffIconData.icon[0], 
				DamageType.thunder => BuffIconData.icon[1], 
				DamageType.poison => BuffIconData.icon[2], 
				DamageType.frozen => BuffIconData.icon[3], 
				DamageType.physics => BuffIconData.icon[4], 
				DamageType.shadow => BuffIconData.icon[5], 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		case "EL_Anti":
			return damageType switch
			{
				DamageType.fire => BuffIconData.icon[6], 
				DamageType.thunder => BuffIconData.icon[7], 
				DamageType.poison => BuffIconData.icon[8], 
				DamageType.frozen => BuffIconData.icon[9], 
				DamageType.physics => BuffIconData.icon[10], 
				DamageType.shadow => BuffIconData.icon[11], 
				_ => throw new ArgumentOutOfRangeException("damageType", damageType, null), 
			};
		case "xueshi":
			return BuffIconData.icon[12];
		case "xingyun":
			return BuffIconData.icon[13];
		case "zhaohuan":
			return BuffIconData.icon[14];
		case "poe_flask_gale":
			return BuffIconData.icon[13];
		case "poe_flask_insight":
			return BuffIconData.icon[4];
		default:
			LogUtil.Warn($"未找到对应药水图标！UseType:{UseType},  DamageType:{damageType}");
			return BuffIconData.icon[0];
		}
	}

	public void AddTempleBuff(int type)
	{
		BuffTempleItem buffTempleItem = HasSameTemple(type);
		if ((bool)buffTempleItem)
		{
			buffTempleItem.maxTime = TempleTimeLast;
			buffTempleItem.remainTime = TempleTimeLast;
			buffTempleItem.Cover();
			buffTempleItem.RefreshTime();
		}
		else
		{
			BuffTempleItem component = LeanPool.Spawn(templePrefab, buffList.transform).GetComponent<BuffTempleItem>();
			component.remainTime = TempleTimeLast;
			component.maxTime = TempleTimeLast;
			component.Type = type;
			component.Init(GetTempleIcon(type));
			component.Cover();
			TempleList.Add(component);
		}
		RefreshSlotOrder();
	}

	public BuffTempleItem HasSameTemple(int type)
	{
		foreach (BuffTempleItem temple in TempleList)
		{
			if (temple.Type == type)
			{
				return temple;
			}
		}
		return null;
	}

	public Sprite GetTempleIcon(int type)
	{
		switch (type)
		{
		case 0:
			return BuffIconData.icon[85];
		case 1:
			return BuffIconData.icon[86];
		case 2:
			return BuffIconData.icon[87];
		case 3:
			return BuffIconData.icon[88];
		case 4:
			return BuffIconData.icon[89];
		case 5:
			return BuffIconData.icon[90];
		case 6:
			return BuffIconData.icon[91];
		case 7:
			return BuffIconData.icon[92];
		case 8:
			return BuffIconData.icon[93];
		case 9:
			return BuffIconData.icon[94];
		case 10:
			return BuffIconData.icon[95];
		case 11:
			return BuffIconData.icon[96];
		case 12:
			return BuffIconData.icon[97];
		case 13:
			return BuffIconData.icon[98];
		case 14:
			return BuffIconData.icon[99];
		case 15:
			return BuffIconData.icon[100];
		case 16:
			return BuffIconData.icon[101];
		default:
			LogUtil.Warn($"未找到对应药水图标！Type:{type}");
			return BuffIconData.icon[0];
		}
	}

	public void ClearAll()
	{
		if (PotionList.Count > 0)
		{
			int num = PotionList.Count;
			int num2;
			for (num2 = 0; num2 < num; num2++)
			{
				BuffPotionItem buffPotionItem = PotionList[num2];
				buffPotionItem.DelBuff();
				PotionList.Remove(buffPotionItem);
				LeanPool.Despawn(buffPotionItem);
				num--;
				num2--;
			}
		}
		if (TempleList.Count > 0)
		{
			int num3 = TempleList.Count;
			int num4;
			for (num4 = 0; num4 < num3; num4++)
			{
				BuffTempleItem buffTempleItem = TempleList[num4];
				buffTempleItem.DelBuff();
				TempleList.Remove(buffTempleItem);
				LeanPool.Despawn(buffTempleItem);
				num3--;
				num4--;
			}
		}
		if (SingletonMonoScope<SimplePotionManager>.HasInstance)
		{
			SingletonMonoScope<SimplePotionManager>.Instance.ClearAllSimple();
		}
	}

	public void RefreshSlotOrder()
	{
		if (!isSort)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < PotionList.Count; i++)
		{
			if ((bool)PotionList[i])
			{
				PotionList[i].transform.SetSiblingIndex(num);
				num++;
			}
		}
		for (int j = 0; j < TempleList.Count; j++)
		{
			if ((bool)TempleList[j])
			{
				TempleList[j].transform.SetSiblingIndex(num);
				num++;
			}
		}
	}
}
