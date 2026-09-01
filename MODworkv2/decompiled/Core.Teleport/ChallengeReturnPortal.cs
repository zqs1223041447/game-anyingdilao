using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Interact;
using Localization;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

namespace Core.Teleport;

public class ChallengeReturnPortal : InteractableBase
{
	[Header("设置该点位是否为入口")]
	[SerializeField]
	private bool isEnter;

	[SerializeField]
	private SpriteRenderer mapSprite;

	[Header("引用变量")]
	[SerializeField]
	private Light2D portalLight;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text hintText;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private LocalizedText localizedText;

	[SerializeField]
	private Transform enterPos;

	private static readonly int AnimHover = Animator.StringToHash("Bool");

	private static float interactDistance => SettingsLoader.Instance.portalInteractDis;

	public override InteractionType Type => InteractionType.Portal;

	private void RefreshMapIcon()
	{
		if (!mapSprite)
		{
			mapSprite = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		if ((bool)mapSprite)
		{
			mapSprite.sprite = SettingsLoader.Instance.iconSettings.portal;
			mapSprite.color = SettingsLoader.Instance.iconSettings.backPortalColor;
			mapSprite.transform.localScale = SettingsLoader.Instance.iconSettings.GetPortalFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		}
	}

	private void Awake()
	{
		if (!portalLight)
		{
			Transform transform = base.transform.Find("light");
			if ((bool)transform)
			{
				portalLight = transform.GetComponent<Light2D>();
			}
		}
		if (!animator)
		{
			Transform transform2 = base.transform.Find("Canvas/Image");
			if ((bool)transform2)
			{
				animator = transform2.GetComponent<Animator>();
			}
		}
		if (!canvasGroup)
		{
			Transform transform3 = base.transform.Find("Canvas");
			if ((bool)transform3)
			{
				canvasGroup = transform3.GetComponent<CanvasGroup>();
			}
		}
		if (!hintText)
		{
			Transform transform4 = base.transform.Find("Canvas/Image/Text");
			if ((bool)transform4)
			{
				hintText = transform4.GetComponent<Text>();
			}
		}
		if ((bool)hintText && !hintText.TryGetComponent<LocalizedText>(out localizedText))
		{
			localizedText = hintText.gameObject.AddComponent<LocalizedText>();
		}
		if (isEnter)
		{
			Vector3 pos = (enterPos ? enterPos.position : base.transform.position);
			SingletonMonoScope<TeleportManager>.Instance.RegisterChallengeEnter(pos, this);
		}
		SetupHintText();
		RefreshMapIcon();
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

	public override void Interact()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			SingletonMonoScope<TeleportManager>.Instance.BackFromChallenge();
		}
	}

	protected override void OnHover(bool isHovering)
	{
		if ((bool)portalLight)
		{
			portalLight.intensity = (isHovering ? 0.4f : 0f);
		}
		if ((bool)animator)
		{
			animator.SetBool(AnimHover, isHovering);
		}
	}

	private void SetupHintText()
	{
		if ((bool)localizedText)
		{
			localizedText.excel = LocalizationExcelList.Level_FY;
			localizedText.key = LevelManager.GetLevelLocalKey(ChallengeContext.FromLevelId);
		}
	}
}
