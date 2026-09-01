using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Interact;
using Localization;
using UI.DebugUI;
using UI.Managers;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class BSTable : InteractableBase
{
	[SerializeField]
	private SpriteRenderer mapSprite;

	[Header("引用变量")]
	[SerializeField]
	private SpriteRenderer render;

	[SerializeField]
	private Light2D bsLight;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text hintText;

	[SerializeField]
	private Animator ani;

	[SerializeField]
	private LocalizedText localizedText;

	private static readonly int liang = Shader.PropertyToID("_Liang");

	private static readonly int b = Animator.StringToHash("Bool");

	public bool near => Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.npcInteractDis + 0.2f;

	public override InteractionType Type => InteractionType.openUI;

	private static float interactDistance => SettingsLoader.Instance.chestInteractDis;

	private void RefreshMapIcon()
	{
		if (!mapSprite)
		{
			mapSprite = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		if ((bool)mapSprite)
		{
			mapSprite.sprite = SettingsLoader.Instance.iconSettings.bsTable;
			mapSprite.color = SettingsLoader.Instance.iconSettings.bsTableColor;
			mapSprite.transform.localScale = SettingsLoader.Instance.iconSettings.GetBsTableFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		}
	}

	private void Awake()
	{
		if (!render)
		{
			render = base.transform.Find("main/base").gameObject.GetComponent<SpriteRenderer>();
		}
		if (!bsLight)
		{
			bsLight = base.transform.Find("light").gameObject.GetComponent<Light2D>();
		}
		if (!ani)
		{
			ani = base.transform.Find("Canvas/Image").gameObject.GetComponent<Animator>();
		}
		if (!hintText)
		{
			hintText = base.transform.Find("Canvas/Image/Text").gameObject.GetComponent<Text>();
		}
		if (!canvasGroup)
		{
			canvasGroup = base.transform.Find("Canvas ").gameObject.GetComponent<CanvasGroup>();
		}
		if ((bool)hintText && !hintText.TryGetComponent<LocalizedText>(out localizedText))
		{
			localizedText = hintText.gameObject.AddComponent<LocalizedText>();
		}
		if ((bool)bsLight)
		{
			bsLight.intensity = 0f;
		}
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = 0f;
		}
		RefreshMapIcon();
		if ((bool)localizedText)
		{
			localizedText.Set(LocalizationExcelList.Main_FY, "bs_table");
		}
	}

	private void Update()
	{
		if (!SingletonMonoScope<BaoshiManager>.HasInstance || SingletonMonoScope<BaoshiManager>.Instance.Opened)
		{
			CheckCloseBaoshiByDistance();
		}
	}

	private static void BaoshiToggle()
	{
		if (SettingsLoader.Instance.BaoshiToggle)
		{
			if (SingletonMonoScope<BaoshiManager>.HasInstance && !SingletonMonoScope<BaoshiManager>.Instance.Opened)
			{
				SingletonMonoScope<BaoshiManager>.Instance.OpenBaoshi();
				SingletonMonoScope<GameUIManager>.Instance.ShopChestCloseOther();
			}
			else
			{
				SingletonMonoScope<BaoshiManager>.Instance.CloseBaoshi();
			}
		}
	}

	private void CheckCloseBaoshiByDistance()
	{
		if (SettingsLoader.Instance.BaoshiToggle && SingletonMonoScope<BaoshiManager>.HasInstance && SingletonMonoScope<BaoshiManager>.Instance.Opened && SingletonMonoScope<PlayerManager>.HasInstance && (!near || !SingletonMonoScope<PlayerManager>.Instance.IsAlive))
		{
			SingletonMonoScope<BaoshiManager>.Instance.CloseBaoshi();
		}
	}

	public override void Interact()
	{
		if (!SettingsLoader.Instance.BaoshiToggle)
		{
			UILog.Warn("全局配置中未启用宝石加工台");
		}
		else
		{
			BaoshiToggle();
		}
	}

	public override bool CanInteract()
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return false;
		}
		Vector3 position = SingletonMonoScope<PlayerManager>.Instance.transform.position;
		return (base.transform.position - position).sqrMagnitude <= interactDistance * interactDistance;
	}

	protected override void OnHover(bool isHovering)
	{
		if (isHovering)
		{
			canvasGroup.alpha = 1f;
			render.material.SetFloat(liang, 0f);
			ani.SetBool(b, value: true);
			bsLight.intensity = 0.3f;
		}
		else
		{
			canvasGroup.alpha = 0f;
			render.material.SetFloat(liang, 1f);
			ani.SetBool(b, value: false);
			bsLight.intensity = 0f;
		}
	}
}
