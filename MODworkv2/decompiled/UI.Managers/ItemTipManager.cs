using System.Collections.Generic;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Lean.Pool;
using UI.UIItems;
using UnityEngine;

namespace UI.Managers;

public class ItemTipManager : SingletonMonoScope<ItemTipManager>
{
	[Header("预制体")]
	public GameObject tipPrefab;

	private const string prefabPath = "UI/Components/Game/ItemTip";

	[Header("布局参数")]
	[SerializeField]
	private int maxVisibleCount = 8;

	[SerializeField]
	private float startY;

	[SerializeField]
	private float itemHeight = 60f;

	[SerializeField]
	private float spacing = 10f;

	[Header("UI根物体")]
	[SerializeField]
	private GameObject uiroot;

	private readonly List<ItemTipItem> activeTips = new List<ItemTipItem>();

	public bool enableItemTip { get; set; }

	public void SetItemTipEnable(bool enable)
	{
		enableItemTip = enable;
	}

	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		if (!tipPrefab)
		{
			tipPrefab = Singleton<ResManager>.Instance.Load<GameObject>("UI/Components/Game/ItemTip");
		}
		if (!uiroot && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			uiroot = SingletonMonoScope<GameUIManager>.Instance.itemTipList;
		}
	}

	private void Start()
	{
		if (Singleton<SettingDataManager>.Instance.Game != null)
		{
			SetItemTipEnable(Singleton<SettingDataManager>.Instance.Interface.item_tip);
		}
	}

	public void AddItemTip(string itemName, int count, Sprite icon = null)
	{
		if (!enableItemTip)
		{
			return;
		}
		if (!tipPrefab)
		{
			LogUtil.Error("ItemTipManager: tipPrefab 为空");
			return;
		}
		if (GetAliveTipCount() >= maxVisibleCount)
		{
			ItemTipItem oldestAliveTip = GetOldestAliveTip();
			if ((bool)oldestAliveTip)
			{
				oldestAliveTip.ForceExit();
			}
		}
		GameObject gameObject = LeanPool.Spawn(tipPrefab, uiroot.transform);
		ItemTipItem component = gameObject.GetComponent<ItemTipItem>();
		if (!component)
		{
			LogUtil.Error("ItemTip 预制体上缺少 ItemTipItem 脚本");
			LeanPool.Despawn(gameObject);
			return;
		}
		activeTips.Add(component);
		component.Init(this, itemName, count, icon);
		RefreshAllTipPosition();
		Vector2 targetPosition = GetTargetPosition(GetAliveIndex(component));
		component.PlayEnter(targetPosition);
	}

	public void NotifyTipExitComplete(ItemTipItem item)
	{
		if ((bool)item)
		{
			activeTips.Remove(item);
			if ((bool)item.gameObject)
			{
				LeanPool.Despawn(item.gameObject);
			}
			RefreshAllTipPosition();
		}
	}

	private void RefreshAllTipPosition()
	{
		if (!enableItemTip)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < activeTips.Count; i++)
		{
			ItemTipItem itemTipItem = activeTips[i];
			if ((bool)itemTipItem && !itemTipItem.IsExiting)
			{
				Vector2 targetPosition = GetTargetPosition(num);
				itemTipItem.SetTargetPosition(targetPosition);
				num++;
			}
		}
	}

	private Vector2 GetTargetPosition(int index)
	{
		float y = startY - (float)index * (itemHeight + spacing);
		return new Vector2(0f, y);
	}

	private int GetAliveTipCount()
	{
		int num = 0;
		for (int i = 0; i < activeTips.Count; i++)
		{
			if ((bool)activeTips[i] && !activeTips[i].IsExiting)
			{
				num++;
			}
		}
		return num;
	}

	private ItemTipItem GetOldestAliveTip()
	{
		for (int i = 0; i < activeTips.Count; i++)
		{
			if ((bool)activeTips[i] && !activeTips[i].IsExiting)
			{
				return activeTips[i];
			}
		}
		return null;
	}

	private int GetAliveIndex(ItemTipItem target)
	{
		int num = 0;
		for (int i = 0; i < activeTips.Count; i++)
		{
			ItemTipItem itemTipItem = activeTips[i];
			if ((bool)itemTipItem && !itemTipItem.IsExiting)
			{
				if (itemTipItem == target)
				{
					return num;
				}
				num++;
			}
		}
		return num;
	}
}
