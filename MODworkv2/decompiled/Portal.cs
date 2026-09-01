using System;
using Core.Settings;
using Core.Teleport;
using Cysharp.Threading.Tasks;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Interact;
using Level.StateData.ChapterStates;
using Localization;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class Portal : InteractableBase
{
	[Header("音效")]
	[SerializeField]
	private string spawnSound;

	[Header("消耗特效(可选)")]
	[SerializeField]
	private GameObject consumeFxPrefab;

	[Header("回到原关卡后=延迟(秒)开始播放消失特效")]
	[SerializeField]
	private float FxDelay = 0.5f;

	[Header("开始播放消失特效后延迟销毁(秒)")]
	[SerializeField]
	private float DestroyDelay = 0.5f;

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

	private static readonly int AnimHover = Animator.StringToHash("Bool");

	private bool interactEnabled;

	private float enableTimer;

	private const float EnableDelay = 0.1f;

	private static float interactDistance => SettingsLoader.Instance.portalInteractDis;

	public PortalType PortalType { get; private set; }

	public bool IsConsumed { get; private set; }

	public string TargetLevelId { get; private set; } = "";


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
		if ((bool)hintText && !hintText.TryGetComponent<LocalizedText>(out localizedText))
		{
			localizedText = hintText.gameObject.AddComponent<LocalizedText>();
		}
		ResetState();
		RefreshMapIcon();
	}

	private void OnEnable()
	{
		ResetState();
		if (!string.IsNullOrEmpty(spawnSound))
		{
			RuntimeManager.PlayOneShot(spawnSound, base.transform.position);
		}
	}

	private void Update()
	{
		HandleEnableTimer();
	}

	public void Init(PortalType type, PortalData data)
	{
		if (data.IsConsumed)
		{
			ConsumeLogic();
		}
		PortalType = type;
		TargetLevelId = data.targetLevelId;
		RefreshHintText();
	}

	public void SetInteractable(bool value)
	{
		interactEnabled = value;
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = (value ? 1f : 0.5f);
		}
		if (!value)
		{
			OnHover(isHovering: false);
		}
	}

	public void ConsumeLogic()
	{
		if (!IsConsumed)
		{
			IsConsumed = true;
			SetInteractable(value: false);
		}
	}

	public async UniTask PlayConsumeFxAndDestroyAsync()
	{
		if (FxDelay > 0f)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(FxDelay), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
		}
		if ((bool)consumeFxPrefab)
		{
			UnityEngine.Object.Instantiate(consumeFxPrefab, base.transform.position, Quaternion.identity);
		}
		if (DestroyDelay > 0f)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(DestroyDelay), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
		}
		if ((bool)this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void PlayConsumeFxAndDestroy()
	{
		if ((bool)consumeFxPrefab)
		{
			UnityEngine.Object.Instantiate(consumeFxPrefab, base.transform.position, Quaternion.identity);
		}
		UnityEngine.Object.Destroy(base.gameObject, DestroyDelay);
	}

	public override bool CanInteract()
	{
		if (!this)
		{
			return false;
		}
		if (!interactEnabled)
		{
			return false;
		}
		if (!interactEnabled)
		{
			return false;
		}
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return false;
		}
		Vector3 position = SingletonMonoScope<PlayerManager>.Instance.transform.position;
		return (base.transform.position - position).sqrMagnitude <= interactDistance * interactDistance;
	}

	public override void Interact()
	{
		if (!SingletonMonoScope<PortalManager>.HasInstance)
		{
			LogUtil.Error("Portal", "PortalManager 不存在，无法处理传送门交互");
		}
		else
		{
			SingletonMonoScope<PortalManager>.Instance.OnPortalInteracted(this);
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

	private void ResetState()
	{
		enableTimer = 0f;
		interactEnabled = false;
		if ((bool)portalLight)
		{
			portalLight.intensity = 0f;
		}
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = 0f;
		}
	}

	private void HandleEnableTimer()
	{
		if (interactEnabled)
		{
			return;
		}
		enableTimer += Time.deltaTime;
		if (enableTimer >= 0.1f)
		{
			interactEnabled = true;
			if ((bool)canvasGroup)
			{
				canvasGroup.alpha = 1f;
			}
		}
	}

	private void RefreshHintText()
	{
		if ((bool)localizedText)
		{
			switch (PortalType)
			{
			case PortalType.HomeGoLevel:
				localizedText.Set(LocalizationExcelList.Level_FY, LevelManager.GetLevelLocalKey(TargetLevelId));
				break;
			case PortalType.GoBackHome:
				localizedText.Set(LocalizationExcelList.Level_FY, "Home");
				break;
			case PortalType.Challenge:
				localizedText.Set(LocalizationExcelList.Level_FY, LevelManager.GetLevelLocalKey(TargetLevelId));
				break;
			}
		}
	}
}
