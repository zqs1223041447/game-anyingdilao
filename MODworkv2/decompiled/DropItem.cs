using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Interact;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItem : InteractableBase
{
	public SpriteRenderer render;

	public DropItemController parent;

	public bool IS_UI_Shader;

	public override InteractionType Type => InteractionType.Item;

	public override int Priority => GetDropItemPriority();

	private void Awake()
	{
		if (!IS_UI_Shader)
		{
			render = GetComponent<SpriteRenderer>();
			parent = GetComponentInParent<DropItemController>();
		}
		else
		{
			parent = base.transform.parent.GetComponentInParent<DropItemController>();
		}
	}

	private int GetDropItemPriority()
	{
		if (!parent)
		{
			return 0;
		}
		int num = 300000;
		switch (parent.ItemType)
		{
		case 0:
			num = 300000;
			break;
		case 1:
			num = 100000;
			break;
		case 2:
			num = 200000;
			break;
		}
		int itemQualityScore = GetItemQualityScore();
		return num + itemQualityScore;
	}

	private int GetItemQualityScore()
	{
		if (!parent)
		{
			return 0;
		}
		switch (parent.ItemType)
		{
		case 0:
			if (parent.weapon == null)
			{
				return 0;
			}
			return GetWeaponScore(parent.weapon);
		case 1:
			if (parent.baoshi == null)
			{
				return 0;
			}
			return GetBaoshiScore(parent.baoshi);
		case 2:
			if (parent.useitem == null)
			{
				return 0;
			}
			return GetUseItemScore(parent.useitem);
		default:
			return 0;
		}
	}

	private static int GetWeaponScore(WeaponClass weapon)
	{
		return (weapon.Quality + 1) * 1000;
	}

	private static int GetBaoshiScore(BaoshiClass baoshi)
	{
		return (baoshi.Quality + 1) * 1000;
	}

	private static int GetUseItemScore(UseItemClass useitem)
	{
		return (useitem.Quality + 1) * 1000;
	}

	public override void Interact()
	{
	}

	public override void OnLeftClick()
	{
		if (CanInteract() && !InputManager.IsPointerOverUIForCurrentCursorMode() && parent.LuoDi)
		{
			parent.PickUp(rightClick: false);
		}
	}

	public override void OnRightClick()
	{
		if (CanInteract() && !EventSystem.current.IsPointerOverGameObject() && SingletonMonoScope<GameUIManager>.Instance.Opened_IV && parent.LuoDi)
		{
			parent.PickUp(rightClick: true);
		}
	}

	public override bool CanInteract()
	{
		return Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.itemInteractDis;
	}

	protected override void OnHover(bool isHovering)
	{
		if (isHovering)
		{
			if ((bool)render)
			{
				render.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}
		else if (IS_UI_Shader)
		{
			if (parent.displayItemManager.DropItemUI_IsOpened && (bool)render)
			{
				render.color = new Color32(195, 195, 195, byte.MaxValue);
			}
		}
		else if (!parent.displayItemManager.DropItemUI_IsOpened && (bool)render)
		{
			render.color = new Color32(195, 195, 195, byte.MaxValue);
		}
	}
}
