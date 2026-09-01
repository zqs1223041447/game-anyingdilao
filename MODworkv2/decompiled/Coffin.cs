using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Interact;
using Level.LevelStates;
using Level.StateData.LevelStates;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Coffin : InteractableBase, IInteractableState
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

	private bool Opened;

	private GameObject off;

	private GameObject on;

	private SpriteRenderer renderA;

	private SpriteRenderer renderB;

	private Transform point;

	[SerializeField]
	private float high;

	[SerializeField]
	private int Quality;

	[SerializeField]
	private int TaskItem;

	[SerializeField]
	private int Sound;

	private static Sprite opened => SettingsLoader.Instance.iconSettings.openedChest;

	private static Color openedChestColor => SettingsLoader.Instance.iconSettings.openedChestColor;

	private static Sprite newed => SettingsLoader.Instance.iconSettings.newChest;

	private static Color newChestColor => SettingsLoader.Instance.iconSettings.newChestColor;

	public override InteractionType Type => InteractionType.Chest;

	public string RuntimeId { get; private set; }

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

	private void Awake()
	{
		RuntimeId = RuntimeIdUtil.GenerateByIndex(base.transform);
		SingletonMonoScene<LevelInteractablesManager>.Instance.Register(this);
		off = base.transform.Find("main/of").gameObject;
		on = base.transform.Find("main/on").gameObject;
		renderA = base.transform.Find("main/of").gameObject.GetComponent<SpriteRenderer>();
		renderB = base.transform.Find("main/base").gameObject.GetComponent<SpriteRenderer>();
		point = base.transform.Find("point").gameObject.transform;
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		text = base.transform.Find("Canvas/Text").GetComponent<Text>();
		rect = base.transform.Find("Canvas/Image").GetComponent<RectTransform>();
		RefreshMapIcon();
	}

	private void Start()
	{
		if (!Opened)
		{
			off.SetActive(value: true);
			on.SetActive(value: false);
			Opened = false;
			canvas.alpha = 0f;
			RefreshMapIcon();
		}
	}

	public override bool CanInteract()
	{
		if (Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.coffinInteractDis)
		{
			return !Opened;
		}
		return false;
	}

	public override void Interact()
	{
		Opened = true;
		off.SetActive(value: false);
		on.SetActive(value: true);
		renderA.material.SetFloat(liang, 1f);
		renderB.material.SetFloat(liang, 1f);
		this.wait(0.05f, delegate
		{
			SingletonMonoScope<ItemManager>.Instance.ChestDrop(point, high, Quality);
		});
		canvas.alpha = 0f;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, Sound);
		RefreshMapIcon();
	}

	protected override void OnHover(bool isHovering)
	{
		if (!Opened)
		{
			if (isHovering)
			{
				renderA.material.SetFloat(liang, 0f);
				renderB.material.SetFloat(liang, 0f);
				text.text = LOC.MM.GetMain("Coffin");
				Canvas.ForceUpdateCanvases();
				text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth);
				float size = (text.preferredWidth * text.rectTransform.localScale.x + 15f) / rect.localScale.x;
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				canvas.alpha = 1f;
			}
			else
			{
				renderA.material.SetFloat(liang, 1f);
				renderB.material.SetFloat(liang, 1f);
				canvas.alpha = 0f;
			}
		}
	}

	public void FlushToState()
	{
		if (!SingletonMonoScope<LevelManager>.HasInstance || LevelManager.ShouldPersistLevelState(LevelManager.GetCurLevel()))
		{
			LevelState currentLevelState = SingletonMonoGlobal<StateDataManager>.Instance.GetCurrentLevelState();
			if (!currentLevelState.Interactables.TryGetValue(RuntimeId, out var value))
			{
				value = new InteractableLevelState(InteractableType.Coffin);
				currentLevelState.Interactables.Add(RuntimeId, value);
			}
			value.InteractableType = InteractableType.Coffin;
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
		renderA.material.SetFloat(liang, 1f);
		renderB.material.SetFloat(liang, 1f);
		canvas.alpha = 0f;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, Sound);
		RefreshMapIcon();
	}

	private void OnDestroy()
	{
		if (SingletonMonoScene<LevelInteractablesManager>.HasInstance)
		{
			SingletonMonoScene<LevelInteractablesManager>.Instance.Unregister(this);
		}
	}
}
