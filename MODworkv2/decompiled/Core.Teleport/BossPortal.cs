using Core.Settings;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Interact;
using Localization;
using Scenes;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

namespace Core.Teleport;

public class BossPortal : InteractableBase
{
	[Header("音效")]
	[SerializeField]
	private string spawnSound;

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

	private bool isInited;

	private BossPortalType bossPortalType = BossPortalType.GoHome;

	private bool interactEnabled;

	private bool isConsumed;

	private float enableTimer;

	private const float EnableDelay = 0.1f;

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
			mapSprite.color = SettingsLoader.Instance.iconSettings.bossPortalColor;
			mapSprite.transform.localScale = SettingsLoader.Instance.iconSettings.GetPortalFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		}
	}

	public void Init(BossPortalType type)
	{
		if (!isInited)
		{
			bossPortalType = type;
		}
		enableTimer = 0f;
		interactEnabled = false;
		isConsumed = false;
		SetupHintText();
		isInited = true;
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
		RefreshMapIcon();
	}

	private void Update()
	{
		HandleEnableTimer();
	}

	public override bool CanInteract()
	{
		if (!base.gameObject.activeSelf)
		{
			return false;
		}
		if (isConsumed)
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
		if (isConsumed)
		{
			return;
		}
		if (!isInited)
		{
			LogUtil.Error("boss返回传送门初始化未完成，禁止交互！");
		}
		else if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			ConsumeLogic();
			switch (bossPortalType)
			{
			case BossPortalType.GoLevel:
				SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
				{
					Reason = LevelPlayerSpawnReason.EnterFromTeleport,
					FromTeleportType = TeleportType.Exit
				});
				SceneLoadManager.LoadLevelScene(LevelManager.GetNextLevelId(), SceneTransitionMode.Fade).Forget();
				break;
			case BossPortalType.GoHome:
				SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
				{
					Reason = HomePlayerSpawnReason.ReturnFromChapter,
					FromChapterId = LevelManager.GetChapterId(LevelManager.GetCurLevel()),
					PlayHomeVictoryMusic = LevelManager.GetIsCurChapterFinal()
				});
				SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade).Forget();
				break;
			}
		}
	}

	private void SetInteractable(bool value)
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

	private void ConsumeLogic()
	{
		if (!isConsumed)
		{
			isConsumed = true;
			SetInteractable(value: false);
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

	private void HandleEnableTimer()
	{
		if (isConsumed || interactEnabled)
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

	private void SetupHintText()
	{
		if ((bool)localizedText)
		{
			switch (bossPortalType)
			{
			case BossPortalType.GoHome:
				localizedText.Set(LocalizationExcelList.Level_FY, "Home");
				break;
			case BossPortalType.GoLevel:
				localizedText.Set(LocalizationExcelList.Level_FY, LevelManager.GetLevelLocalKey(LevelManager.GetNextLevelId()));
				break;
			}
		}
	}
}
