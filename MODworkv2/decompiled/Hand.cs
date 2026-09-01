using Entity.InteractableObjects.Item;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Inputs.Cursors;
using Lean.Pool;
using UnityEngine;

public class Hand : MonoBehaviour
{
	private static Hand instance;

	public ItemScript ItemOBJ;

	public IntVector2 itemSize;

	public bool isDragItem;

	public bool IsNewlyPickedItem;

	public int itemType;

	public WeaponClass weapon;

	public BaoshiClass baoshi;

	public UseItemClass useitem;

	public int Mpos;

	private AudioManager _audioManager;

	public static Hand Instance
	{
		get
		{
			if (!instance)
			{
				instance = Object.FindObjectOfType<Hand>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
	}

	private void Start()
	{
		for (int i = 0; i < 6; i++)
		{
			WPSkill item = new WPSkill();
			weapon.WPSK.Add(item);
			WPAocao wPAocao = new WPAocao();
			wPAocao.HasAocao = false;
			weapon.Aocao.Add(wPAocao);
		}
		isDragItem = false;
		IsNewlyPickedItem = false;
	}

	private void Update()
	{
		if (isDragItem)
		{
			ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		}
	}

	public void HideBaoshiDisplay()
	{
		for (int i = 0; i < 6; i++)
		{
			ItemOBJ.aocao[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			ItemOBJ.BS[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
		}
	}

	public void PickUPItem(DropItemController it, ItemScript A)
	{
		RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		ItemOBJ = A;
		A.IsNewlyPicked = true;
		IsNewlyPickedItem = true;
		itemType = it.ItemType;
		switch (it.ItemType)
		{
		case 0:
			ItemCloneUtil.CopyWeaponTo(weapon, it.weapon);
			itemSize = it.weapon.Size;
			break;
		case 1:
			ItemCloneUtil.CopyBaoshiTo(baoshi, it.baoshi);
			itemSize = it.baoshi.Size;
			break;
		case 2:
			ItemCloneUtil.CopyUseItemTo(useitem, it.useitem);
			itemSize = it.useitem.Size;
			break;
		}
		isDragItem = true;
		A.transform.SetParent(base.transform);
		A.GetComponent<RectTransform>().localScale = Vector3.one;
		ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
	}

	public void TakeItem(SlotData dt)
	{
		RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		ItemOBJ = dt.ItemOBJ;
		IsNewlyPickedItem = dt.ItemData != null && dt.ItemData.IsNewlyPicked;
		if ((bool)ItemOBJ)
		{
			ItemOBJ.IsNewlyPicked = IsNewlyPickedItem;
		}
		itemType = dt.ItemType;
		switch (dt.ItemType)
		{
		case 0:
			ItemCloneUtil.CopyWeaponTo(weapon, dt.weapon);
			itemSize = dt.weapon.Size;
			HideBaoshiDisplay();
			break;
		case 1:
			ItemCloneUtil.CopyBaoshiTo(baoshi, dt.baoshi);
			itemSize = dt.baoshi.Size;
			ItemOBJ.RefreshStackHand(0);
			break;
		case 2:
			ItemCloneUtil.CopyUseItemTo(useitem, dt.useitem);
			itemSize = dt.useitem.Size;
			ItemOBJ.RefreshStackHand(1);
			break;
		}
		ItemOBJ.transform.SetParent(base.transform);
		ItemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
		ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
		ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		isDragItem = true;
	}

	public void BuyItemSurplus(SlotData dt, ItemScript A)
	{
		if (dt == null || !A || (bool)ItemOBJ)
		{
			if ((bool)A)
			{
				LeanPool.Despawn(A);
			}
			return;
		}
		RuntimeManager.PlayOneShot(_audioManager.audioData.Pick_Item);
		ItemOBJ = A;
		A.IsNewlyPicked = false;
		IsNewlyPickedItem = false;
		itemType = dt.ItemType;
		switch (dt.ItemType)
		{
		case 0:
			ItemCloneUtil.CopyWeaponTo(weapon, dt.weapon);
			itemSize = dt.weapon.Size;
			HideBaoshiDisplay();
			break;
		case 1:
			ItemCloneUtil.CopyBaoshiTo(baoshi, dt.baoshi);
			itemSize = dt.baoshi.Size;
			A.SetBS(baoshi, 3);
			break;
		case 2:
			ItemCloneUtil.CopyUseItemTo(useitem, dt.useitem);
			itemSize = dt.useitem.Size;
			A.SetUse(useitem, 3);
			break;
		}
		ItemOBJ.transform.SetParent(base.transform);
		ItemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
		ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
		ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		isDragItem = true;
	}

	public void TakeOne(SlotData dt)
	{
		IsNewlyPickedItem = dt.ItemData != null && dt.ItemData.IsNewlyPicked;
		if ((bool)ItemOBJ)
		{
			ItemOBJ.IsNewlyPicked = IsNewlyPickedItem;
		}
		itemType = dt.ItemType;
		switch (dt.ItemType)
		{
		case 1:
			ItemCloneUtil.CopyBaoshiTo(baoshi, dt.baoshi);
			HideBaoshiDisplay();
			baoshi.CstackSize = 1;
			ItemOBJ.Split(0, dt);
			itemSize = dt.baoshi.Size;
			break;
		case 2:
			ItemCloneUtil.CopyUseItemTo(useitem, dt.useitem);
			HideBaoshiDisplay();
			useitem.CstackSize = 1;
			ItemOBJ.Split(1, dt);
			itemSize = dt.useitem.Size;
			break;
		}
		ItemOBJ.transform.SetParent(base.transform);
		ItemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
		ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
		ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
		isDragItem = true;
	}

	public void TakeWP(WeaponClass wp, ItemScript its)
	{
		if (wp != null && (bool)its)
		{
			ItemOBJ = its;
			IsNewlyPickedItem = its.IsNewlyPicked;
			itemType = 0;
			ItemCloneUtil.CopyWeaponTo(weapon, wp);
			itemSize = wp.Size;
			if ((bool)ItemOBJ)
			{
				ItemOBJ.HideSocketDisplay();
			}
			ItemOBJ.transform.SetParent(base.transform);
			ItemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
			ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
			ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			isDragItem = true;
		}
	}

	public void TakeBS(BaoshiClass BS, ItemScript its)
	{
		if (BS != null && (bool)its)
		{
			ItemOBJ = its;
			IsNewlyPickedItem = its.IsNewlyPicked;
			itemType = 1;
			baoshi = BS;
			itemSize = BS.Size;
			HideBaoshiDisplay();
			ItemOBJ.transform.SetParent(base.transform);
			ItemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
			ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
			ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			isDragItem = true;
		}
	}

	public void TakeUSE(UseItemClass USE, ItemScript its)
	{
		if (USE != null && (bool)its)
		{
			ItemOBJ = its;
			IsNewlyPickedItem = its.IsNewlyPicked;
			itemType = 2;
			useitem = USE;
			itemSize = USE.Size;
			HideBaoshiDisplay();
			ItemOBJ.transform.SetParent(base.transform);
			ItemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
			ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
			ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			isDragItem = true;
		}
	}

	public void DELItem()
	{
		itemSize = IntVector2.Zero;
		isDragItem = false;
		itemType = -1;
		ItemScript itemOBJ = ItemOBJ;
		ItemOBJ = null;
		IsNewlyPickedItem = false;
		weapon = new WeaponClass();
		baoshi = new BaoshiClass();
		useitem = new UseItemClass();
		if ((bool)itemOBJ)
		{
			itemOBJ.IsNewlyPicked = false;
			LeanPool.Despawn(itemOBJ);
		}
	}

	public void Equip()
	{
		itemSize = IntVector2.Zero;
	}

	public void Dequip(ItemScript it, WeaponClass wp)
	{
		if ((bool)it && wp != null)
		{
			ItemOBJ = it;
			IsNewlyPickedItem = it.IsNewlyPicked;
			itemType = it.ItemType;
			ItemCloneUtil.CopyWeaponTo(weapon, wp);
			itemSize = wp.Size;
			if ((bool)ItemOBJ)
			{
				ItemOBJ.HideSocketDisplay();
			}
			ItemOBJ.transform.SetParent(base.transform);
			ItemOBJ.GetComponent<RectTransform>().localScale = Vector3.one;
			ItemOBJ.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
			ItemOBJ.transform.position = SingletonMonoScope<CursorInputManager>.Instance.ScreenPosition;
			isDragItem = true;
		}
	}

	public void ClearItem()
	{
		ItemOBJ = null;
		itemSize = IntVector2.Zero;
		isDragItem = false;
		IsNewlyPickedItem = false;
	}
}
