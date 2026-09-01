using System;
using Entity.InteractableObjects.Item;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
	private float slotSize;

	public Image icon;

	public IntVector2 itemSize;

	public int ItemType;

	[NonSerialized]
	public bool IsNewlyPicked;

	public WeaponClass weapon;

	public BaoshiClass baoshi;

	public UseItemClass useitem;

	public RectTransform rect;

	public GridLayoutGroup GridLayout;

	public Image[] aocao;

	public Image[] BS;

	public Text stackText;

	public int page;

	public IntVector2 saveSlot;

	private void Awake()
	{
		slotSize = SingletonMonoScope<InventoryManager>.Instance.slotSize;
		icon = GetComponent<Image>();
		rect = GetComponent<RectTransform>();
		if (!stackText)
		{
			stackText = base.transform.Find("Text").GetComponent<Text>();
		}
		SetStart();
	}

	public void SetStart()
	{
		WeaponClass weaponClass = new WeaponClass();
		for (int i = 0; i < 6; i++)
		{
			WPSkill item = new WPSkill();
			weaponClass.WPSK.Add(item);
			WPAocao wPAocao = new WPAocao();
			wPAocao.HasAocao = false;
			weaponClass.Aocao.Add(wPAocao);
		}
		weapon = weaponClass;
	}

	public void HideSocketDisplay()
	{
		for (int i = 0; i < 6; i++)
		{
			if (aocao != null && i < aocao.Length && (bool)aocao[i])
			{
				aocao[i].gameObject.SetActive(value: false);
				aocao[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			}
			if (BS != null && i < BS.Length && (bool)BS[i])
			{
				BS[i].gameObject.SetActive(value: false);
				BS[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			}
		}
	}

	public void SetItem(DropItemController item, bool handTake)
	{
		ItemType = item.ItemType;
		switch (ItemType)
		{
		case 0:
		{
			ItemCloneUtil.CopyWeaponTo(weapon, item.weapon);
			itemSize = weapon.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)weapon.Size.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)weapon.Size.y * slotSize);
			icon.sprite = weapon.Icon;
			for (int k = 0; k < 6; k++)
			{
				aocao[k].gameObject.SetActive(value: false);
				BS[k].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			}
			if (weapon.AocaoCount > weapon.Size.y)
			{
				GridLayout.constraintCount = 2;
			}
			else
			{
				GridLayout.constraintCount = 1;
			}
			if (weapon.AocaoCount > 0)
			{
				for (int l = 0; l < weapon.AocaoCount; l++)
				{
					aocao[l].gameObject.SetActive(value: true);
					aocao[l].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
				}
			}
			stackText.gameObject.SetActive(value: false);
			break;
		}
		case 1:
		{
			ItemCloneUtil.CopyBaoshiTo(baoshi, item.baoshi);
			itemSize = baoshi.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)baoshi.Size.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)baoshi.Size.y * slotSize);
			icon.sprite = baoshi.Icon;
			for (int j = 0; j < 6; j++)
			{
				aocao[j].gameObject.SetActive(value: false);
			}
			stackText.gameObject.SetActive(value: true);
			if (handTake)
			{
				stackText.text = baoshi.CstackSize.ToString();
			}
			else
			{
				RefreshStackIV(0);
			}
			break;
		}
		case 2:
		{
			ItemCloneUtil.CopyUseItemTo(useitem, item.useitem);
			itemSize = useitem.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)useitem.Size.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)useitem.Size.y * slotSize);
			icon.sprite = useitem.Icon;
			for (int i = 0; i < 6; i++)
			{
				aocao[i].gameObject.SetActive(value: false);
			}
			stackText.gameObject.SetActive(value: true);
			if (handTake)
			{
				stackText.text = useitem.CstackSize.ToString();
			}
			else
			{
				RefreshStackIV(1);
			}
			break;
		}
		}
	}

	public void SetWP(WeaponClass wp)
	{
		ItemCloneUtil.CopyWeaponTo(weapon, wp);
		itemSize = weapon.Size;
		ItemType = weapon.ItemType;
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)weapon.Size.x * slotSize);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)weapon.Size.y * slotSize);
		icon.sprite = weapon.Icon;
		HideSocketDisplay();
		if (weapon.AocaoCount > weapon.Size.y)
		{
			GridLayout.constraintCount = 2;
		}
		else
		{
			GridLayout.constraintCount = 1;
		}
		stackText.gameObject.SetActive(value: false);
	}

	public void SetBS(BaoshiClass bs, int index)
	{
		ItemCloneUtil.CopyBaoshiTo(baoshi, bs);
		itemSize = baoshi.Size;
		ItemType = baoshi.ItemType;
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)baoshi.Size.x * slotSize);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)baoshi.Size.y * slotSize);
		icon.sprite = baoshi.Icon;
		HideSocketDisplay();
		stackText.gameObject.SetActive(value: true);
		switch (index)
		{
		case 0:
			RefreshStackIV(0);
			break;
		case 1:
			RefreshStackChest(0);
			break;
		case 2:
			RefreshStackShop(0);
			break;
		case 3:
			RefreshStackHand(0);
			break;
		}
	}

	public void SetUse(UseItemClass use, int index)
	{
		ItemCloneUtil.CopyUseItemTo(useitem, use);
		itemSize = useitem.Size;
		ItemType = useitem.ItemType;
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)useitem.Size.x * slotSize);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)useitem.Size.y * slotSize);
		icon.sprite = useitem.Icon;
		HideSocketDisplay();
		stackText.gameObject.SetActive(value: true);
		switch (index)
		{
		case 0:
			RefreshStackIV(1);
			break;
		case 1:
			RefreshStackChest(1);
			break;
		case 2:
			RefreshStackShop(1);
			break;
		case 3:
			RefreshStackHand(1);
			break;
		}
	}

	public void RefreshStackIV(int a)
	{
		if (!stackText || !SingletonMonoScope<InventoryManager>.Instance || page < 0 || page >= SingletonMonoScope<InventoryManager>.Instance.Page.Count || saveSlot.x < 0 || saveSlot.y < 0)
		{
			return;
		}
		SlotData slotData = SingletonMonoScope<InventoryManager>.Instance.Page[page].DT[saveSlot.x, saveSlot.y];
		if (slotData == null)
		{
			return;
		}
		switch (a)
		{
		case 0:
			if (slotData.baoshi != null)
			{
				stackText.text = slotData.baoshi.CstackSize.ToString();
			}
			break;
		case 1:
			if (slotData.useitem != null)
			{
				stackText.text = slotData.useitem.CstackSize.ToString();
			}
			break;
		}
	}

	public void RefreshStackChest(int a)
	{
		if (!stackText || !SingletonMonoScope<WarehouseManager>.Instance || page < 0 || page >= SingletonMonoScope<WarehouseManager>.Instance.Page.Count || saveSlot.x < 0 || saveSlot.y < 0)
		{
			return;
		}
		SlotDataPage slotDataPage = SingletonMonoScope<WarehouseManager>.Instance.Page[page];
		if (slotDataPage?.DT == null || saveSlot.x >= slotDataPage.DT.GetLength(0) || saveSlot.y >= slotDataPage.DT.GetLength(1))
		{
			return;
		}
		SlotData slotData = slotDataPage.DT[saveSlot.x, saveSlot.y];
		if (slotData == null)
		{
			return;
		}
		switch (a)
		{
		case 0:
			if (slotData.baoshi != null)
			{
				stackText.text = slotData.baoshi.CstackSize.ToString();
			}
			break;
		case 1:
			if (slotData.useitem != null)
			{
				stackText.text = slotData.useitem.CstackSize.ToString();
			}
			break;
		}
	}

	public void RefreshStackShop(int a)
	{
		if (!stackText || !SingletonMonoScope<ShopManager>.Instance || page < 0 || page >= SingletonMonoScope<ShopManager>.Instance.Page.Count || saveSlot.x < 0 || saveSlot.y < 0)
		{
			return;
		}
		SlotDataPage slotDataPage = SingletonMonoScope<ShopManager>.Instance.Page[page];
		if (slotDataPage?.DT == null || saveSlot.x >= slotDataPage.DT.GetLength(0) || saveSlot.y >= slotDataPage.DT.GetLength(1))
		{
			return;
		}
		SlotData slotData = slotDataPage.DT[saveSlot.x, saveSlot.y];
		if (slotData == null)
		{
			return;
		}
		switch (a)
		{
		case 0:
			if (slotData.baoshi != null)
			{
				stackText.text = slotData.baoshi.CstackSize.ToString();
			}
			break;
		case 1:
			if (slotData.useitem != null)
			{
				stackText.text = slotData.useitem.CstackSize.ToString();
			}
			break;
		}
	}

	public void RefreshStackHand(int a)
	{
		switch (a)
		{
		case 0:
			stackText.text = Hand.Instance.baoshi.CstackSize.ToString();
			break;
		case 1:
			stackText.text = Hand.Instance.useitem.CstackSize.ToString();
			break;
		}
	}

	public void RefreshBS(SlotData dt)
	{
		if (dt != null && dt.weapon != null && (bool)dt.ItemOBJ && dt.ItemOBJ.aocao != null && dt.ItemOBJ.BS != null)
		{
			dt.ItemOBJ.RefreshSocketDisplay(dt.weapon, showEmptySockets: true);
		}
	}

	public void RefreshSocketDisplay(WeaponClass sourceWeapon, bool showEmptySockets)
	{
		HideSocketDisplay();
		if (sourceWeapon == null || sourceWeapon.Aocao == null || aocao == null || BS == null)
		{
			return;
		}
		int num = Mathf.Min(sourceWeapon.AocaoCount, sourceWeapon.Aocao.Count, aocao.Length, BS.Length);
		for (int i = 0; i < num; i++)
		{
			if ((bool)aocao[i] && (bool)BS[i])
			{
				aocao[i].gameObject.SetActive(value: true);
				if (sourceWeapon.Aocao[i].HasBaoshi)
				{
					aocao[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
					BS[i].sprite = sourceWeapon.Aocao[i].Icon;
					BS[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					BS[i].gameObject.SetActive(value: true);
				}
				else
				{
					aocao[i].color = (showEmptySockets ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 100) : new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0));
					BS[i].gameObject.SetActive(value: false);
					BS[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
				}
			}
		}
	}

	public void Split(int a, SlotData dt)
	{
		switch (a)
		{
		case 0:
			ItemCloneUtil.CopyBaoshiTo(baoshi, dt.baoshi);
			icon.sprite = baoshi.Icon;
			itemSize = baoshi.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)itemSize.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)itemSize.y * slotSize);
			break;
		case 1:
			ItemCloneUtil.CopyUseItemTo(useitem, dt.useitem);
			icon.sprite = useitem.Icon;
			itemSize = useitem.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)itemSize.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)itemSize.y * slotSize);
			break;
		}
	}

	public void NewPagePut(SlotData dt, int index)
	{
		ItemType = dt.ItemType;
		switch (ItemType)
		{
		case 0:
			ItemCloneUtil.CopyWeaponTo(weapon, dt.weapon);
			itemSize = weapon.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)weapon.Size.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)weapon.Size.y * slotSize);
			icon.sprite = weapon.Icon;
			HideSocketDisplay();
			if (weapon.AocaoCount > weapon.Size.y)
			{
				GridLayout.constraintCount = 2;
			}
			else
			{
				GridLayout.constraintCount = 1;
			}
			if (weapon.AocaoCount > 0)
			{
				for (int i = 0; i < weapon.AocaoCount; i++)
				{
					aocao[i].gameObject.SetActive(value: true);
					aocao[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
				}
			}
			stackText.gameObject.SetActive(value: false);
			break;
		case 1:
			ItemCloneUtil.CopyBaoshiTo(baoshi, dt.baoshi);
			itemSize = baoshi.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)baoshi.Size.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)baoshi.Size.y * slotSize);
			icon.sprite = baoshi.Icon;
			HideSocketDisplay();
			stackText.gameObject.SetActive(value: true);
			switch (index)
			{
			case 0:
				RefreshStackIV(0);
				break;
			case 1:
				RefreshStackChest(0);
				break;
			case 2:
				RefreshStackShop(0);
				break;
			}
			break;
		case 2:
			ItemCloneUtil.CopyUseItemTo(useitem, dt.useitem);
			itemSize = useitem.Size;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)useitem.Size.x * slotSize);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)useitem.Size.y * slotSize);
			icon.sprite = useitem.Icon;
			HideSocketDisplay();
			stackText.gameObject.SetActive(value: true);
			switch (index)
			{
			case 0:
				RefreshStackIV(1);
				break;
			case 1:
				RefreshStackChest(1);
				break;
			case 2:
				RefreshStackShop(1);
				break;
			}
			break;
		}
	}
}
