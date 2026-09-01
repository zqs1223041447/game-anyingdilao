using System.Collections.Generic;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UI.UIItems;
using UnityEngine;

public class SimplePotionManager : SingletonMonoScope<SimplePotionManager>
{
	public GameObject simplePotion;

	[HideInInspector]
	public List<BuffSimpleItem> SimpleList = new List<BuffSimpleItem>();

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		if (!simplePotion)
		{
			simplePotion = Singleton<ResManager>.Instance.Load<GameObject>("UI/Components/Buff/BuffSimpleItem");
		}
	}

	public bool HasSameDrink(UseItemClass it)
	{
		foreach (BuffSimpleItem simple in SimpleList)
		{
			if (simple.UseType == it.UseType)
			{
				return true;
			}
		}
		return false;
	}

	public void AddSimpleDrink(UseItemClass it)
	{
		BuffSimpleItem component = LeanPool.Spawn(simplePotion, base.transform).GetComponent<BuffSimpleItem>();
		component.CDTime = it.CDTime;
		component.UseType = it.UseType;
		SimpleList.Add(component);
		for (int i = 0; i < SingletonMonoScope<ACTbar>.Instance.useBT.Length; i++)
		{
			if (SingletonMonoScope<ACTbar>.Instance.useBT[i].Type == component.UseType)
			{
				SingletonMonoScope<ACTbar>.Instance.useBT[i].slot = component;
				SingletonMonoScope<ACTbar>.Instance.useBT[i].IsCD = true;
			}
		}
	}

	public void ClearAllSimple()
	{
		if (SimpleList.Count > 0)
		{
			int num = SimpleList.Count;
			int num2;
			for (num2 = 0; num2 < num; num2++)
			{
				BuffSimpleItem buffSimpleItem = SimpleList[num2];
				buffSimpleItem.Del();
				LeanPool.Despawn(buffSimpleItem);
				num--;
				num2--;
			}
		}
	}
}
