using System.Collections;
using System.Collections.Generic;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs.Cursors;
using UI.UIItems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Entity.Comp.CompanionAI;

public class CompanionManager : SingletonMonoScope<CompanionManager>
{
	private const string prefabPath = "UI/Components/Comp/CompUI";

	private const int SummonSkillType = 1;

	[SerializeField]
	private Transform uiRoot;

	private GameObject companionPrefab;

	private bool _refreshQueued;

	private Dictionary<string, CompItem> compListUI;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		compListUI = new Dictionary<string, CompItem>();
	}

	protected override void Awake()
	{
		base.Awake();
		EnsureUIRoot();
	}

	private void Start()
	{
		if (!companionPrefab)
		{
			companionPrefab = Singleton<ResManager>.Instance.Load<GameObject>("UI/Components/Comp/CompUI");
		}
	}

	public void RequestRefreshNextFrame()
	{
		if (!_refreshQueued)
		{
			_refreshQueued = true;
			StartCoroutine(RefreshNextFrameCoroutine());
		}
	}

	private IEnumerator RefreshNextFrameCoroutine()
	{
		yield return null;
		_refreshQueued = false;
		if ((bool)this && (bool)base.gameObject)
		{
			RefreshAllCompUI();
		}
	}

	public void RefreshAllCompUI()
	{
		EnsureUIRoot();
		if (!SingletonMonoScope<ACTbar>.HasInstance)
		{
			ClearAllUI();
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (ACTListSkillBT item in SingletonMonoScope<ACTbar>.Instance.actListSkill)
		{
			if ((bool)item && item.DT != null && item.DT.type == 1 && !string.IsNullOrEmpty(item.IndexName))
			{
				hashSet.Add(item.IndexName);
				RefreshCompUI(item);
			}
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CompItem> item2 in compListUI)
		{
			if (!hashSet.Contains(item2.Key))
			{
				list.Add(item2.Key);
			}
		}
		foreach (string item3 in list)
		{
			RemoveItemUI(item3);
		}
	}

	public void RefreshAfterCompItemDismiss()
	{
		RefreshAllCompUI();
		StartCoroutine(RefreshHoveredCompItemTipNextFrame());
	}

	private IEnumerator RefreshHoveredCompItemTipNextFrame()
	{
		yield return null;
		CompItem hoveredCompItem = GetHoveredCompItem();
		if ((bool)hoveredCompItem)
		{
			hoveredCompItem.ShowSkillTip();
		}
		else if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.HideSkillTip();
		}
	}

	private static CompItem GetHoveredCompItem()
	{
		if (!EventSystem.current)
		{
			return null;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = (SingletonMonoScope<CursorInputManager>.HasInstance ? ((Vector2)SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition) : ((Vector2)Input.mousePosition))
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		foreach (RaycastResult item in list)
		{
			CompItem componentInParent = item.gameObject.GetComponentInParent<CompItem>();
			if ((bool)componentInParent)
			{
				return componentInParent;
			}
		}
		return null;
	}

	private void RefreshCompUI(ACTListSkillBT data)
	{
		if ((bool)data && !string.IsNullOrEmpty(data.IndexName))
		{
			int num = data.cpList?.Count ?? 0;
			CompItem value;
			if (num <= 0)
			{
				RemoveItemUI(data.IndexName);
			}
			else if (compListUI.TryGetValue(data.IndexName, out value) && (bool)value)
			{
				value.BindSkill(data);
				value.RefreshCount(num);
			}
			else
			{
				SpawnItemUI(data, num);
			}
		}
	}

	private void SpawnItemUI(ACTListSkillBT data, int count)
	{
		if (!data)
		{
			return;
		}
		if (!companionPrefab)
		{
			companionPrefab = Singleton<ResManager>.Instance.Load<GameObject>("UI/Components/Comp/CompUI");
			if (!companionPrefab)
			{
				LogUtil.Error("CompanionManager: 同伴UI预制体加载失败");
				return;
			}
		}
		Transform parent = (uiRoot ? uiRoot : base.transform);
		GameObject gameObject = Object.Instantiate(companionPrefab, parent);
		CompItem component = gameObject.GetComponent<CompItem>();
		if (!component)
		{
			LogUtil.Error("CompanionManager: 预制体上未找到 CompItem 组件");
			Object.Destroy(gameObject);
		}
		else
		{
			Sprite sprite = data.icon.sprite;
			component.InitComp(sprite, count, data);
			compListUI[data.IndexName] = component;
		}
	}

	public void RemoveItemUI(string indexName)
	{
		if (!string.IsNullOrEmpty(indexName) && compListUI.TryGetValue(indexName, out var value))
		{
			if ((bool)value)
			{
				Object.Destroy(value.gameObject);
			}
			compListUI.Remove(indexName);
		}
	}

	public void ClearAllUI()
	{
		foreach (KeyValuePair<string, CompItem> item in compListUI)
		{
			if ((bool)item.Value)
			{
				Object.Destroy(item.Value.gameObject);
			}
		}
		compListUI.Clear();
	}

	private void EnsureUIRoot()
	{
		if (!uiRoot && SingletonMonoScope<GameUIManager>.HasInstance && (bool)SingletonMonoScope<GameUIManager>.Instance.compListUI)
		{
			uiRoot = SingletonMonoScope<GameUIManager>.Instance.compListUI.transform;
		}
	}
}
