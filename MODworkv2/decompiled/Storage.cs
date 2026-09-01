using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Interact;
using UnityEngine;
using UnityEngine.UI;

public class Storage : InteractableBase
{
	private static readonly int liang = Shader.PropertyToID("_Liang");

	private static Storage instance;

	public CanvasGroup canvas;

	public Text text;

	public RectTransform rect;

	public bool Opened;

	public GameObject off;

	public GameObject on;

	public SpriteRenderer render;

	private PlayerManager PL;

	[SerializeField]
	private SpriteRenderer mapSprite;

	public override InteractionType Type => InteractionType.Chest;

	public static Storage Instance
	{
		get
		{
			if (!instance)
			{
				instance = Object.FindObjectOfType<Storage>();
			}
			return instance;
		}
	}

	public bool near => Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.chestInteractDis + 0.2f;

	private void Awake()
	{
		off = base.transform.Find("main/of").gameObject;
		on = base.transform.Find("main/on").gameObject;
		render = base.transform.Find("main/of").gameObject.GetComponent<SpriteRenderer>();
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		text = base.transform.Find("Canvas/Text").GetComponent<Text>();
		rect = base.transform.Find("Canvas/Image").GetComponent<RectTransform>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		if (!mapSprite)
		{
			mapSprite = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		if ((bool)SettingsLoader.Instance.iconSettings.storge && (bool)mapSprite)
		{
			mapSprite.gameObject.transform.localScale = Vector3.one * 0.4f;
			mapSprite.sprite = SettingsLoader.Instance.iconSettings.storge;
			mapSprite.color = SettingsLoader.Instance.iconSettings.storgeColor;
			mapSprite.transform.localScale = SettingsLoader.Instance.iconSettings.GetStorgeFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		}
	}

	private void Start()
	{
		off.SetActive(value: true);
		on.SetActive(value: false);
		Opened = false;
		canvas.alpha = 0f;
	}

	private void Update()
	{
		if (Opened && (!near || !PL.IsAlive))
		{
			CloseChest();
		}
	}

	public void OpenChest()
	{
		off.SetActive(value: false);
		on.SetActive(value: true);
		Opened = true;
		SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse = true;
		SingletonMonoScope<WarehouseManager>.Instance.cav.alpha = 1f;
		SingletonMonoScope<WarehouseManager>.Instance.cav.blocksRaycasts = true;
		SingletonMonoScope<GameUIManager>.Instance.Opened_IV = true;
		SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 1f;
		SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = true;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, 1);
	}

	public void CloseChest()
	{
		off.SetActive(value: true);
		on.SetActive(value: false);
		Opened = false;
		SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse = false;
		SingletonMonoScope<WarehouseManager>.Instance.cav.alpha = 0f;
		SingletonMonoScope<WarehouseManager>.Instance.cav.blocksRaycasts = false;
		SingletonMonoScope<GameUIManager>.Instance.Opened_IV = false;
		SingletonMonoScope<InventoryManager>.Instance.cav.alpha = 0f;
		SingletonMonoScope<InventoryManager>.Instance.cav.blocksRaycasts = false;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, 0);
	}

	public void CloseChestUI()
	{
		off.SetActive(value: true);
		on.SetActive(value: false);
		Opened = false;
		SingletonMonoScope<GameUIManager>.Instance.Opened_warehouse = false;
		SingletonMonoScope<WarehouseManager>.Instance.cav.alpha = 0f;
		SingletonMonoScope<WarehouseManager>.Instance.cav.blocksRaycasts = false;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, 0);
	}

	public override bool CanInteract()
	{
		return Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.chestInteractDis;
	}

	public override void Interact()
	{
		if (Opened)
		{
			CloseChest();
			return;
		}
		OpenChest();
		SingletonMonoScope<GameUIManager>.Instance.ShopChestCloseOther();
	}

	protected override void OnHover(bool isHovering)
	{
		if (isHovering)
		{
			render.material.SetFloat(liang, 0f);
			text.text = LOC.MM.GetMain("ChestCamp");
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			float size = (text.preferredWidth * text.rectTransform.localScale.x + 15f) / rect.localScale.x;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			canvas.alpha = 1f;
		}
		else
		{
			render.material.SetFloat(liang, 1f);
			canvas.alpha = 0f;
		}
	}
}
