using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Interact;
using Level.LevelStates;
using Level.StateData.LevelStates;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Chest : InteractableBase, IInteractableState
{
	[SerializeField]
	private SpriteRenderer mapSprite;

	private static readonly int liang = Shader.PropertyToID("_Liang");

	[HideInInspector]
	public CanvasGroup canvas;

	[HideInInspector]
	public Text text;

	[HideInInspector]
	public RectTransform rect;

	private GameObject Lit;

	private GameObject FX;

	private bool Opened;

	private GameObject off;

	private GameObject on;

	private SpriteRenderer render;

	private Transform point;

	public float high;

	public int Quality;

	public int Sound;

	private bool isSpawnedByPoint;

	private static Sprite opened => SettingsLoader.Instance.iconSettings.openedChest;

	private static Color openedChestColor => SettingsLoader.Instance.iconSettings.openedChestColor;

	private static Sprite newed => SettingsLoader.Instance.iconSettings.newChest;

	private static Color newChestColor => SettingsLoader.Instance.iconSettings.newChestColor;

	public string RuntimeId { get; private set; }

	public override InteractionType Type => InteractionType.Chest;

	private void ControlText()
	{
		if ((bool)text)
		{
			text.fontSize = 40;
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);
		}
		if ((bool)rect)
		{
			RectTransform component = rect.GetComponent<RectTransform>();
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 70f);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 13f);
		}
	}

	private void RefreshMapIcon()
	{
		if (!mapSprite)
		{
			mapSprite = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		Vector3 openedChestFinalScale = SettingsLoader.Instance.iconSettings.GetOpenedChestFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		Vector3 newChestFinalScale = SettingsLoader.Instance.iconSettings.GetNewChestFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		if ((bool)opened && (bool)newed)
		{
			mapSprite.sprite = (Opened ? opened : newed);
			mapSprite.color = (Opened ? openedChestColor : newChestColor);
			mapSprite.gameObject.transform.localScale = (opened ? openedChestFinalScale : newChestFinalScale);
		}
	}

	public void InitRuntimeId(string id)
	{
		isSpawnedByPoint = true;
		RuntimeId = id;
		Quality = 0;
		ApplyQuality();
		SingletonMonoScene<LevelInteractablesManager>.Instance.Register(this);
	}

	private void Awake()
	{
		off = base.transform.Find("main/of").gameObject;
		on = base.transform.Find("main/on").gameObject;
		render = base.transform.Find("main/of").gameObject.GetComponent<SpriteRenderer>();
		point = base.transform.Find("point").gameObject.transform;
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		text = base.transform.Find("Canvas/Text").GetComponent<Text>();
		rect = base.transform.Find("Canvas/Image").GetComponent<RectTransform>();
		ControlText();
		Lit = base.transform.Find("Lit").gameObject;
		FX = base.transform.Find("main/FX").gameObject;
		if (!isSpawnedByPoint)
		{
			RuntimeId = RuntimeIdUtil.GenerateByIndex(base.transform);
			SingletonMonoScene<LevelInteractablesManager>.Instance.Register(this);
			Quality = 1;
			ApplyQuality();
		}
		RefreshMapIcon();
	}

	private void OnEnable()
	{
		if (!Opened)
		{
			off.SetActive(value: true);
			on.SetActive(value: false);
		}
		canvas.alpha = 0f;
		ApplyQuality();
		RefreshMapIcon();
	}

	private void ApplyQuality()
	{
		if (Quality == 1)
		{
			Lit.SetActive(value: true);
			FX.SetActive(value: true);
		}
		else
		{
			Lit.SetActive(value: false);
			FX.SetActive(value: false);
		}
	}

	public override bool CanInteract()
	{
		if (Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.chestInteractDis)
		{
			return !Opened;
		}
		return false;
	}

	public override void Interact()
	{
		off.SetActive(value: false);
		on.SetActive(value: true);
		this.wait(0.03f, delegate
		{
			SingletonMonoScope<ItemManager>.Instance.ChestDrop(point, high, Quality);
		});
		Opened = true;
		canvas.alpha = 0f;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, Sound);
		if (Quality == 1)
		{
			Lit.SetActive(value: false);
			FX.SetActive(value: false);
		}
		RefreshMapIcon();
	}

	private void OnDestroy()
	{
		if (SingletonMonoScene<LevelInteractablesManager>.HasInstance)
		{
			SingletonMonoScene<LevelInteractablesManager>.Instance.Unregister(this);
		}
	}

	protected override void OnHover(bool isHovering)
	{
		if (!Opened)
		{
			if (isHovering)
			{
				render.material.SetFloat(liang, 0f);
				text.text = LOC.MM.GetMain("Chest");
				Canvas.ForceUpdateCanvases();
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

	public void FlushToState()
	{
		if (SingletonMonoScope<LevelManager>.HasInstance && !LevelManager.ShouldPersistLevelState(LevelManager.GetCurLevel()))
		{
			return;
		}
		LevelState currentLevelState = SingletonMonoGlobal<StateDataManager>.Instance.GetCurrentLevelState();
		if (currentLevelState != null)
		{
			if (!currentLevelState.Interactables.TryGetValue(RuntimeId, out var value))
			{
				value = new InteractableLevelState(InteractableType.Chest);
				currentLevelState.Interactables.Add(RuntimeId, value);
			}
			value.InteractableType = InteractableType.Chest;
			value.IsOpened = Opened;
		}
	}

	public void RestoreState()
	{
		if ((!SingletonMonoScope<LevelManager>.HasInstance || LevelManager.ShouldPersistLevelState(LevelManager.GetCurLevel())) && SingletonMonoGlobal<StateDataManager>.Instance.GetCurrentLevelState().Interactables.TryGetValue(RuntimeId, out var value) && value.IsOpened)
		{
			ApplyOpenedState();
		}
	}

	private void ApplyOpenedState()
	{
		Opened = true;
		off.SetActive(value: false);
		on.SetActive(value: true);
		canvas.alpha = 0f;
		if (Quality == 1)
		{
			Lit.SetActive(value: false);
			FX.SetActive(value: false);
		}
		RefreshMapIcon();
	}
}
