using Core.Settings;
using Dialog;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Inputs;
using Interact;
using UI.Panels;
using UnityEngine;
using UnityEngine.UI;

public class NPC : InteractableBase
{
	[SerializeField]
	private NpcType currentType;

	private static readonly int liang = Shader.PropertyToID("_Liang");

	[SerializeField]
	private MeshRenderer render;

	public CanvasGroup cav;

	public Text NameText;

	public RectTransform rect;

	public string NPCname;

	private PlayerManager PL;

	[SerializeField]
	private SpriteRenderer mapSprite;

	[Header("NPC感叹号图标提示（仅限队长）")]
	[SerializeField]
	private SpriteRenderer exclamationMark;

	public override InteractionType Type => InteractionType.NPC;

	public bool near => Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.npcInteractDis + 0.2f;

	private void Awake()
	{
		render = base.transform.Find("NPC/Spine").gameObject.GetComponent<MeshRenderer>();
		cav = base.transform.Find("Canvas").gameObject.GetComponent<CanvasGroup>();
		NameText = base.transform.Find("Canvas/Text").gameObject.GetComponent<Text>();
		rect = base.transform.Find("Canvas/Image").GetComponent<RectTransform>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		cav.alpha = 0f;
		if (!mapSprite)
		{
			mapSprite = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		if ((bool)SettingsLoader.Instance.iconSettings.npc)
		{
			mapSprite.gameObject.transform.localScale = Vector3.one * 0.5f;
			mapSprite.sprite = SettingsLoader.Instance.iconSettings.npc;
			mapSprite.transform.localScale = SettingsLoader.Instance.iconSettings.GetNpcFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		}
	}

	private void Update()
	{
		switch (currentType)
		{
		default:
			return;
		case NpcType.Shop:
			CheckCloseShopByDistance();
			break;
		case NpcType.Rebuild:
			CheckCloseWeaponByDistance();
			break;
		case NpcType.Dialog:
			CheckCloseDialogByDistance();
			break;
		}
		RefreshExclamationMark();
	}

	public override bool CanInteract()
	{
		return Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.npcInteractDis;
	}

	public override void Interact()
	{
		switch (currentType)
		{
		case NpcType.Shop:
			ShopToggle();
			break;
		case NpcType.Rebuild:
			WeaponToggle();
			break;
		case NpcType.Dialog:
			DialogToggle();
			break;
		}
	}

	private static void WeaponToggle()
	{
		if (SettingsLoader.Instance.WeaponToggle)
		{
			if (SingletonMonoScope<WeaponManager>.HasInstance && !SingletonMonoScope<WeaponManager>.Instance.Opened)
			{
				SingletonMonoScope<WeaponManager>.Instance.OpenWeapon();
				SingletonMonoScope<GameUIManager>.Instance.ShopChestCloseOther();
			}
			else
			{
				SingletonMonoScope<WeaponManager>.Instance.CloseWeapon();
			}
		}
	}

	private void CheckCloseWeaponByDistance()
	{
		if (SettingsLoader.Instance.WeaponToggle && SingletonMonoScope<WeaponManager>.HasInstance && SingletonMonoScope<WeaponManager>.Instance.Opened && (!near || !PL.IsAlive))
		{
			SingletonMonoScope<WeaponManager>.Instance.CloseWeapon();
		}
	}

	private void RefreshExclamationMark()
	{
		if ((bool)exclamationMark && SingletonMonoScope<DialogManager>.HasInstance)
		{
			bool active = SingletonMonoScope<DialogManager>.Instance.HasNewMainNpcDialog();
			exclamationMark.gameObject.SetActive(active);
		}
	}

	private static void DialogToggle()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			SingletonMonoScope<GameUIManager>.Instance.CloseAll();
		}
		if (SingletonMonoScope<DialogManager>.HasInstance)
		{
			string currentMainNpcDialogId = SingletonMonoScope<DialogManager>.Instance.GetCurrentMainNpcDialogId();
			if (!string.IsNullOrEmpty(currentMainNpcDialogId))
			{
				SingletonMonoScope<DialogManager>.Instance.OpenDialog(currentMainNpcDialogId);
			}
		}
	}

	private void CheckCloseDialogByDistance()
	{
		if (Singleton<UIManager>.Instance.IsPanelOpened<DialogPanel>() && (!near || !PL.IsAlive))
		{
			Singleton<UIManager>.Instance.HidePanel<DialogPanel>();
			InputManager.AllActionToggle = true;
		}
	}

	private static void ShopToggle()
	{
		if (SingletonMonoScope<ShopManager>.HasInstance && !SingletonMonoScope<ShopManager>.Instance.Opened)
		{
			SingletonMonoScope<ShopManager>.Instance.OpenShop();
			SingletonMonoScope<GameUIManager>.Instance.ShopChestCloseOther();
		}
		else
		{
			SingletonMonoScope<ShopManager>.Instance.CloseShop();
		}
	}

	private void CheckCloseShopByDistance()
	{
		if (SingletonMonoScope<ShopManager>.HasInstance && SingletonMonoScope<ShopManager>.Instance.Opened && (!near || !PL.IsAlive))
		{
			SingletonMonoScope<ShopManager>.Instance.CloseShop();
		}
	}

	protected override void OnHover(bool isHovering)
	{
		if ((bool)this && (bool)render)
		{
			if (isHovering)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetFloat(liang, 0f);
				render?.SetPropertyBlock(materialPropertyBlock);
				cav.alpha = 1f;
				NameText.text = LOC.MM.GetMain(NPCname);
				NameText.horizontalOverflow = HorizontalWrapMode.Overflow;
				float size = (NameText.preferredWidth * NameText.rectTransform.localScale.x + 15f) / rect.localScale.x;
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
			else
			{
				MaterialPropertyBlock materialPropertyBlock2 = new MaterialPropertyBlock();
				materialPropertyBlock2.SetFloat(liang, 1f);
				render?.SetPropertyBlock(materialPropertyBlock2);
				cav.alpha = 0f;
			}
		}
	}
}
