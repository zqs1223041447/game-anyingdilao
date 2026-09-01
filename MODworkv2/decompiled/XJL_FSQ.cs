using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XJL_FSQ : MonoBehaviour
{
	private float TimeA;

	private float targetRefreshTime;

	private const float TargetRefreshInterval = 0.8f;

	private const float RetargetCloserDistance = 0.5f;

	public Collider2D[] DPIT = new Collider2D[10];

	public List<DropItemController> DropOBJ = new List<DropItemController>();

	private PlayerManager PL;

	public GameObject[] OBJ;

	public int type;

	public List<XJL> List = new List<XJL>();

	private float appliedDropRate;

	private float appliedCritRate;

	private float appliedCritDamageAnti;

	private void Awake()
	{
		PL = SingletonMonoScope<PlayerManager>.Instance;
		TimeA = 0f;
		targetRefreshTime = 0f;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnDestroy()
	{
		ClearAllXJL();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		DropOBJ.Clear();
		if (!PL && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
		}
		for (int num = List.Count - 1; num >= 0; num--)
		{
			XJL xJL = List[num];
			if (!xJL)
			{
				List.RemoveAt(num);
			}
			else
			{
				xJL.father = this;
				xJL.transform.position = base.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
				xJL.tar = null;
			}
		}
		RebuildPlayerXJLEffects();
	}

	private void Update()
	{
		if (!PL && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
		}
		TimeA += Time.deltaTime;
		targetRefreshTime += Time.deltaTime;
		if (!(TimeA >= 0.2f))
		{
			return;
		}
		Vector3 playerPosition = GetPlayerPosition();
		int num = Physics2D.OverlapCircleNonAlloc(playerPosition, 9f, DPIT, LayerMask.GetMask("AutoPick"));
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				DropItemController component = DPIT[i].GetComponent<DropItemController>();
				if (CanAutoHandleDrop(component) && !DropOBJ.Contains(component))
				{
					DropOBJ.Add(component);
				}
				DPIT[i] = null;
			}
		}
		for (int num2 = List.Count - 1; num2 >= 0; num2--)
		{
			XJL xJL = List[num2];
			if (!xJL)
			{
				List.RemoveAt(num2);
			}
			else if (Vector2.Distance(playerPosition, xJL.transform.position) > 8f)
			{
				xJL.transform.position = new Vector3(playerPosition.x + Random.Range(-1f, 1f), playerPosition.y + Random.Range(-1f, 1f), 0f);
				xJL.tar = null;
			}
		}
		if (DropOBJ.Count > 0)
		{
			for (int num3 = DropOBJ.Count - 1; num3 >= 0; num3--)
			{
				DropItemController dropItemController = DropOBJ[num3];
				if (!CanAutoHandleDrop(dropItemController) || (double)Vector2.Distance(playerPosition, dropItemController.transform.position) > 9.5)
				{
					RemoveDropTargetAt(num3);
				}
			}
			SortDropTargetsByPlayerDistance();
			if (targetRefreshTime >= 0.8f)
			{
				RefreshXJLTargetsByPlayerDistance();
				targetRefreshTime = 0f;
			}
		}
		else
		{
			targetRefreshTime = 0f;
		}
		TimeA = 0f;
	}

	private static bool CanAutoHandleDrop(DropItemController item)
	{
		if ((bool)item && item.gameObject.activeInHierarchy && SingletonMonoScope<InventoryManager>.HasInstance)
		{
			return SingletonMonoScope<InventoryManager>.Instance.CanXJLAutoHandle(item);
		}
		return false;
	}

	public bool IsValidDropTarget(Transform target)
	{
		if (!target)
		{
			return false;
		}
		DropItemController component = target.GetComponent<DropItemController>();
		if ((bool)component && DropOBJ.Contains(component))
		{
			return CanAutoHandleDrop(component);
		}
		return false;
	}

	private Vector3 GetPlayerPosition()
	{
		if (!PL)
		{
			return base.transform.position;
		}
		return PL.transform.position;
	}

	private float GetPlayerDistance(DropItemController item)
	{
		if (!item)
		{
			return float.MaxValue;
		}
		return Vector2.Distance(GetPlayerPosition(), item.transform.position);
	}

	private float GetPlayerDistance(Transform target)
	{
		if (!target)
		{
			return float.MaxValue;
		}
		return Vector2.Distance(GetPlayerPosition(), target.position);
	}

	private void SortDropTargetsByPlayerDistance()
	{
		DropOBJ.Sort((DropItemController t1, DropItemController t2) => GetPlayerDistance(t1).CompareTo(GetPlayerDistance(t2)));
	}

	private void RefreshXJLTargetsByPlayerDistance()
	{
		if (DropOBJ.Count < 1)
		{
			return;
		}
		int num = 0;
		for (int num2 = List.Count - 1; num2 >= 0; num2--)
		{
			XJL xJL = List[num2];
			if (!xJL)
			{
				List.RemoveAt(num2);
			}
			else
			{
				DropItemController dropItemController = DropOBJ[num % DropOBJ.Count];
				num++;
				if ((bool)dropItemController && (!xJL.HasValidDropTarget() || GetPlayerDistance(dropItemController) + 0.5f < GetPlayerDistance(xJL.tar)))
				{
					xJL.SetDropTarget(dropItemController);
				}
			}
		}
	}

	private void RemoveDropTargetAt(int index)
	{
		DropItemController item = DropOBJ[index];
		ClearXJLTarget(item);
		DropOBJ.RemoveAt(index);
	}

	private void ClearXJLTarget(DropItemController item)
	{
		if (!item)
		{
			return;
		}
		Transform transform = item.transform;
		for (int num = List.Count - 1; num >= 0; num--)
		{
			XJL xJL = List[num];
			if (!xJL)
			{
				List.RemoveAt(num);
			}
			else if (xJL.tar == transform)
			{
				xJL.tar = null;
			}
		}
	}

	public void AddXJL(int type, int ID, int MainEL, float range1, float range2, float FStime1, float FStime2, float speed, float DMG)
	{
		RemoveXJL(ID);
		if (OBJ != null && type >= 0 && type < OBJ.Length && (bool)OBJ[type])
		{
			XJL component = LeanPool.Spawn(OBJ[type], new Vector3(base.transform.position.x + Random.Range(-0.3f, 0.3f), base.transform.position.y + Random.Range(-0.3f, 0.3f), 0f), Quaternion.identity).GetComponent<XJL>();
			if ((bool)component)
			{
				component.GlobleID = ID;
				component.XJL_type = type;
				component.MainEL = MainEL;
				component.damageType = SWS.DMtype(MainEL);
				component.PickRange = range1;
				component.ATRange = range2;
				component.PickJG = FStime1;
				component.UseSKTime = FStime2;
				component.Movespeed = speed;
				component.Number = DMG;
				component.father = this;
				component.tar = null;
				List.Add(component);
				RebuildPlayerXJLEffects();
			}
		}
	}

	public void RemoveXJL(int ID)
	{
		for (int num = List.Count - 1; num >= 0; num--)
		{
			XJL xJL = List[num];
			if (!xJL)
			{
				List.RemoveAt(num);
			}
			else if (xJL.GlobleID == ID)
			{
				xJL.ClearRuntimeEffects();
				List.RemoveAt(num);
				LeanPool.Despawn(xJL);
				RebuildPlayerXJLEffects();
				break;
			}
		}
	}

	public void ClearAllXJL()
	{
		for (int num = List.Count - 1; num >= 0; num--)
		{
			XJL xJL = List[num];
			if ((bool)xJL)
			{
				xJL.ClearRuntimeEffects();
				LeanPool.Despawn(xJL);
			}
		}
		List.Clear();
		RebuildPlayerXJLEffects();
	}

	private void RebuildPlayerXJLEffects()
	{
		if (!PL && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
		}
		if (!PL)
		{
			return;
		}
		PL.ItemDrop_Rate_buff_Tmp -= appliedDropRate;
		PL.BJrate_Tmp -= appliedCritRate;
		PL.XJL_BJD_Anti_Tmp -= appliedCritDamageAnti;
		appliedDropRate = 0f;
		appliedCritRate = 0f;
		appliedCritDamageAnti = 0f;
		for (int num = List.Count - 1; num >= 0; num--)
		{
			XJL xJL = List[num];
			if (!xJL)
			{
				List.RemoveAt(num);
			}
			else
			{
				switch (xJL.XJL_type)
				{
				case 0:
					appliedDropRate += xJL.Number;
					break;
				case 2:
					appliedCritRate += xJL.Number;
					break;
				case 6:
					appliedCritDamageAnti += xJL.Number;
					break;
				}
			}
		}
		PL.ItemDrop_Rate_buff_Tmp += appliedDropRate;
		PL.BJrate_Tmp += appliedCritRate;
		PL.XJL_BJD_Anti_Tmp += appliedCritDamageAnti;
		PL.XJL_Count = List.Count;
	}

	public void Pick(DropItemController item)
	{
		ClearXJLTarget(item);
		DropOBJ.Remove(item);
	}
}
