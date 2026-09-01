using Core.Settings;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Interact;
using Mijing;
using UI.DebugUI;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

namespace Core.Teleport;

public class MijingPortal : InteractableBase
{
	[SerializeField]
	private SpriteRenderer mapSprite;

	private int targetMijingId;

	[Header("音频事件Id")]
	[SerializeField]
	private string audio_event;

	[Header("引用变量")]
	[SerializeField]
	private Light2D portalLight;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text hintText;

	[SerializeField]
	private Animator animator;

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
			if (base.gameObject.name.Contains("Challenge"))
			{
				mapSprite.color = SettingsLoader.Instance.iconSettings.challengePortalColor;
			}
			else if (base.gameObject.name.Contains("Mijing"))
			{
				mapSprite.color = SettingsLoader.Instance.iconSettings.mijingPortalColor;
			}
			else if (base.gameObject.name.Contains("Home"))
			{
				mapSprite.color = SettingsLoader.Instance.iconSettings.homePortalColor;
			}
			else
			{
				mapSprite.color = SettingsLoader.Instance.iconSettings.challengePortalColor;
			}
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
		if ((bool)portalLight)
		{
			portalLight.intensity = 0f;
		}
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = 0f;
		}
		RefreshMapIcon();
		if (SingletonMonoScope<MijingManager>.HasInstance)
		{
			targetMijingId = SingletonMonoScope<MijingManager>.Instance.GetNextFloor(1);
		}
		RuntimeManager.PlayOneShot(audio_event, base.transform.position);
	}

	public void Init(int targetFloor)
	{
		targetMijingId = targetFloor;
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
		if (!SettingsLoader.Instance.MijingToggle)
		{
			UILog.Warn("全局配置中未启用秘境");
		}
		else if (SingletonMonoScope<MijingManager>.HasInstance)
		{
			SingletonMonoScope<MijingManager>.Instance.EnterMijing(targetMijingId);
		}
	}

	protected override void OnHover(bool isHovering)
	{
		hintText.text = LOC.MM.GetLevelFormat("mijing_portal_display", targetMijingId);
		if ((bool)portalLight)
		{
			portalLight.intensity = (isHovering ? 0.4f : 0f);
		}
		if ((bool)animator)
		{
			animator.SetBool(AnimHover, isHovering);
		}
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = (isHovering ? 1f : 0f);
		}
	}
}
