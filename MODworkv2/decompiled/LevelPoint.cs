using System;
using Core.Settings;
using Core.Teleport;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using Data.AutoGen.DataClass.Level;
using FinkFramework.Runtime.Singleton;
using Interact;
using Level.LevelStates;
using Localization;
using Scenes;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class LevelPoint : InteractableBase, ILevelLockable
{
	public CanvasGroup canvas;

	public LevelRoot root;

	public Light2D lit;

	public Text tex;

	public Animator ani;

	public LocalizedText localText;

	public Transform playerPos;

	public SpriteRenderer icon;

	public Image bg;

	[Header("本关卡传送点类型")]
	public TeleportType CurrentType;

	[Header("当类型为支线入口时，表示进入排序后的第几个支线（从 0 开始）")]
	public int optionalOrder;

	[HideInInspector]
	public string targetLevelId = "";

	private static readonly int b = Animator.StringToHash("Bool");

	private bool isInteracted;

	private bool CanTeleport = true;

	private bool isLocked;

	public override InteractionType Type => InteractionType.Portal;

	private static float interactDistance => SettingsLoader.Instance.portalInteractDis;

	private void Awake()
	{
		InitReferences();
		ResolveTargetLevel();
		RefreshTargetDisplay();
		if ((bool)root)
		{
			root.RegisterLockable(this);
			root.RegisterExpectedPoint();
		}
		if (!bg)
		{
			bg = base.transform.Find("Canvas/Image").GetComponent<Image>();
		}
		if ((bool)tex)
		{
			tex.fontSize = 40;
			tex.GetComponent<RectTransform>().localScale = Vector3.one * 0.6f;
			tex.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80f);
			tex.horizontalOverflow = HorizontalWrapMode.Overflow;
		}
		if ((bool)bg)
		{
			bg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 370f);
			bg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70f);
		}
	}

	private void InitReferences()
	{
		if (!canvas)
		{
			canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		}
		if (!lit)
		{
			lit = base.transform.Find("light").GetComponent<Light2D>();
		}
		if (!ani)
		{
			ani = base.transform.Find("Canvas/Image").GetComponent<Animator>();
		}
		if (!tex)
		{
			tex = base.transform.Find("Canvas/Image/Text").GetComponent<Text>();
		}
		if (!root)
		{
			root = GetComponentInParent<LevelRoot>();
		}
		if (!playerPos)
		{
			playerPos = base.transform.Find("playerPos");
		}
		if (!icon)
		{
			icon = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		lit.intensity = 0f;
	}

	private void ResolveTargetLevel()
	{
		CanTeleport = true;
		targetLevelId = null;
		if (!SingletonMonoScope<LevelManager>.HasInstance)
		{
			return;
		}
		switch (CurrentType)
		{
		case TeleportType.Enter:
			if (LevelManager.GetIsCurChapterFirst())
			{
				CanTeleport = false;
				targetLevelId = "Home";
			}
			else if (LevelManager.GetIsOptional())
			{
				targetLevelId = LevelManager.GetOptionalParentMainId(LevelManager.GetCurLevel());
			}
			else
			{
				targetLevelId = LevelManager.GetPrevLevelId();
			}
			break;
		case TeleportType.Exit:
			if (LevelManager.GetIsOptional())
			{
				targetLevelId = LevelManager.GetOptionalParentMainId(LevelManager.GetCurLevel());
			}
			else if (LevelManager.GetIsCurChapterFinal())
			{
				targetLevelId = "Home";
			}
			else
			{
				targetLevelId = LevelManager.GetNextLevelId();
			}
			break;
		case TeleportType.Optional_Enter:
			targetLevelId = LevelManager.GetOptionalChildLevelIdByOrder(LevelManager.GetCurLevel(), optionalOrder);
			if (string.IsNullOrEmpty(targetLevelId))
			{
				CanTeleport = false;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!CanTeleport && (bool)canvas)
		{
			canvas.alpha = 0f;
		}
	}

	private void RefreshTargetDisplay()
	{
		if (!CanTeleport || string.IsNullOrEmpty(targetLevelId))
		{
			return;
		}
		LevelData levelData = LevelManager.GetLevelData(targetLevelId);
		if (levelData != null)
		{
			if (!tex.TryGetComponent<LocalizedText>(out localText))
			{
				localText = tex.gameObject.AddComponent<LocalizedText>();
			}
			localText.Set(LocalizationExcelList.Level_FY, levelData.LocalName);
		}
	}

	private void Start()
	{
		if (!CanTeleport)
		{
			if ((bool)icon)
			{
				icon.gameObject.SetActive(value: false);
			}
			return;
		}
		switch (CurrentType)
		{
		case TeleportType.Enter:
			icon.color = SettingsLoader.Instance.iconSettings.EnterColor;
			break;
		case TeleportType.Exit:
			icon.color = SettingsLoader.Instance.iconSettings.ExitColor;
			break;
		case TeleportType.Optional_Enter:
			icon.color = SettingsLoader.Instance.iconSettings.OptionalEnterColor;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if ((bool)SettingsLoader.Instance.iconSettings.levelPoint)
		{
			icon.transform.localScale = SettingsLoader.Instance.iconSettings.GetLevelPointFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
			icon.sprite = SettingsLoader.Instance.iconSettings.levelPoint;
		}
	}

	private void OnEnable()
	{
		if (SingletonMonoScope<TeleportManager>.HasInstance)
		{
			SingletonMonoScope<TeleportManager>.Instance.Register(this);
		}
		if ((bool)root)
		{
			root.NotifyPointReady();
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (SingletonMonoScope<TeleportManager>.HasInstance)
		{
			SingletonMonoScope<TeleportManager>.Instance.Unregister(this);
		}
	}

	public override bool CanInteract()
	{
		if (!CanTeleport)
		{
			return false;
		}
		if (isInteracted)
		{
			return false;
		}
		if (isLocked)
		{
			return false;
		}
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			Vector3 position = SingletonMonoScope<PlayerManager>.Instance.transform.position;
			return (base.transform.position - position).sqrMagnitude <= interactDistance * interactDistance;
		}
		return false;
	}

	public override void Interact()
	{
		if (CanTeleport && !string.IsNullOrEmpty(targetLevelId))
		{
			SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetLevelRequest(new LevelPlayerSpawnRequest
			{
				Reason = LevelPlayerSpawnReason.EnterFromTeleport,
				FromTeleportType = CurrentType
			});
			isInteracted = true;
			SceneLoadManager.LoadLevelScene(targetLevelId, SceneTransitionMode.Fade).Forget();
		}
	}

	protected override void OnHover(bool isHovering)
	{
		if (CanTeleport && !isLocked)
		{
			if (isHovering)
			{
				lit.intensity = 0.7f;
				ani.SetBool(b, value: true);
			}
			else
			{
				lit.intensity = 0f;
				ani.SetBool(b, value: false);
			}
		}
	}

	public void SetLocked(bool locked)
	{
		isLocked = locked;
		if (locked)
		{
			canvas.alpha = 0f;
		}
		else if (CanTeleport)
		{
			canvas.alpha = 1f;
		}
	}
}
