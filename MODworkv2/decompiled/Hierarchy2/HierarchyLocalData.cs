using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hierarchy2;

[ExecuteInEditMode]
public class HierarchyLocalData : MonoBehaviour
{
	public static readonly Dictionary<Scene, HierarchyLocalData> instances = new Dictionary<Scene, HierarchyLocalData>();

	public Dictionary<GameObject, CustomRowItem> dCustomRowItems = new Dictionary<GameObject, CustomRowItem>();

	public List<CustomRowItem> lCustomRowItems = new List<CustomRowItem>();

	private void OnEnable()
	{
		if (!instances.ContainsKey(base.gameObject.scene))
		{
			instances.Add(base.gameObject.scene, this);
		}
		if (!base.gameObject.CompareTag("EditorOnly"))
		{
			base.gameObject.tag = "EditorOnly";
		}
		base.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInBuild;
		ClearNullRef();
		ConvertToDic();
	}

	private void OnDestroy()
	{
		instances.Remove(base.gameObject.scene);
	}

	public static bool GetInstance(Scene scene, out HierarchyLocalData hierarchyLocalData)
	{
		return instances.TryGetValue(scene, out hierarchyLocalData);
	}

	public CustomRowItem CreateCustomRowItemFor(GameObject go)
	{
		CustomRowItem customRowItem = new CustomRowItem(go);
		lCustomRowItems.Add(customRowItem);
		ClearNullRef();
		ConvertToDic();
		return customRowItem;
	}

	public void RemoveCustomRowItemOf(GameObject go)
	{
		lCustomRowItems.RemoveAll((CustomRowItem item) => item.gameObject == go);
		dCustomRowItems.Remove(go);
		ClearNullRef();
		ConvertToDic();
	}

	public bool TryGetCustomRowData(GameObject go, out CustomRowItem customRowItem)
	{
		return dCustomRowItems.TryGetValue(go, out customRowItem);
	}

	private void ConvertToDic()
	{
		dCustomRowItems = lCustomRowItems.ToDictionary((CustomRowItem item) => item.gameObject);
	}

	private void ClearNullRef()
	{
		lCustomRowItems.RemoveAll((CustomRowItem item) => item.gameObject == null);
	}
}
