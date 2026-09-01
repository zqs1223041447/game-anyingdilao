using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Interact;
using Lean.Pool;
using Level.LevelStates;
using Level.StateData.LevelStates;
using UI.Managers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Temple : InteractableBase, IInteractableState
{
	[SerializeField]
	private SpriteRenderer mapSprite;

	private static readonly int liang = Shader.PropertyToID("_Liang");

	public CanvasGroup canvas;

	public Text text;

	public RectTransform rect;

	public GameObject lit;

	public SpriteRenderer render;

	public GameObject point;

	private bool Opened;

	private int A;

	public int type;

	private static Sprite sp => SettingsLoader.Instance.iconSettings.temple;

	private static Color openedColor => SettingsLoader.Instance.iconSettings.templeUsedColor;

	private static Color newColor => SettingsLoader.Instance.iconSettings.templeUnuseColor;

	public string RuntimeId { get; private set; }

	public override InteractionType Type => InteractionType.Temple;

	public override int Priority => 0;

	private void RefreshMapIcon()
	{
		if (!mapSprite)
		{
			mapSprite = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		if ((bool)sp)
		{
			mapSprite.sprite = sp;
			mapSprite.color = (Opened ? openedColor : newColor);
			mapSprite.gameObject.transform.localScale = SettingsLoader.Instance.iconSettings.GetTempleFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		}
	}

	private void Awake()
	{
		RuntimeId = RuntimeIdUtil.GenerateByIndex(base.transform);
		SingletonMonoScene<LevelInteractablesManager>.Instance.Register(this);
		lit = base.transform.Find("main/light").gameObject;
		render = base.transform.Find("main/base").gameObject.GetComponent<SpriteRenderer>();
		point = base.transform.Find("main/point").gameObject;
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		text = base.transform.Find("Canvas/Text").GetComponent<Text>();
		rect = base.transform.Find("Canvas/Image").GetComponent<RectTransform>();
		RefreshMapIcon();
	}

	private void Start()
	{
		if (Opened)
		{
			return;
		}
		A = Random.Range(0, 101);
		if (A > 0)
		{
			lit.SetActive(value: true);
			type = Random.Range(0, 17);
			if (!SingletonMonoScope<GameDataManager>.HasInstance)
			{
				return;
			}
			LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.WDPB.TempleFX[type], point.transform.position, base.transform.rotation, point.transform);
			canvas.alpha = 0f;
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		RefreshMapIcon();
	}

	public override void OnHoverEnter()
	{
		if (!Opened)
		{
			render.material.SetFloat(liang, 0f);
			string main = LOC.MM.GetMain("Temple" + type);
			text.text = main;
			Canvas.ForceUpdateCanvases();
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			float size = (text.preferredWidth * text.rectTransform.localScale.x + 15f) / rect.localScale.x;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			canvas.alpha = 1f;
		}
	}

	public override void OnHoverExit()
	{
		if (!Opened)
		{
			render.material.SetFloat(liang, 1f);
			canvas.alpha = 0f;
		}
	}

	public override void Interact()
	{
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.WDPB.TempleSpark, point.transform.position, point.transform.rotation, point.transform);
			LeanPool.Despawn(point.transform.GetChild(0).gameObject);
			lit.SetActive(value: false);
			render.material.SetFloat(liang, 1f);
			canvas.alpha = 0f;
			Opened = true;
			SingletonMonoGlobal<AudioManager>.Instance.SceneEatTemple(base.transform, type);
			if (SingletonMonoScope<BuffManager>.HasInstance)
			{
				SingletonMonoScope<BuffManager>.Instance.AddTempleBuff(type);
			}
			SingletonMonoScope<ACTbar>.Instance.RefreshCD();
			RefreshMapIcon();
			if (Random.Range(0, 100) < SingletonMonoScope<PlayerManager>.Instance.Temple_BS)
			{
				SingletonMonoScope<ItemManager>.Instance.DropBaoshi(base.transform, 1.5f, SingletonMonoScope<PlayerManager>.Instance.Level);
			}
		}
	}

	public override bool CanInteract()
	{
		if (Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.templeInteractDis)
		{
			return !Opened;
		}
		return false;
	}

	public void FlushToState()
	{
		if (!SingletonMonoScope<LevelManager>.HasInstance || LevelManager.ShouldPersistLevelState(LevelManager.GetCurLevel()))
		{
			LevelState currentLevelState = SingletonMonoGlobal<StateDataManager>.Instance.GetCurrentLevelState();
			if (!currentLevelState.Interactables.TryGetValue(RuntimeId, out var value))
			{
				value = new InteractableLevelState(InteractableType.Temple);
				currentLevelState.Interactables.Add(RuntimeId, value);
			}
			value.InteractableType = InteractableType.Temple;
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
		if (point.transform.childCount > 0)
		{
			LeanPool.Despawn(point.transform.GetChild(0).gameObject);
		}
		lit.SetActive(value: false);
		render.material.SetFloat(liang, 1f);
		canvas.alpha = 0f;
		Opened = true;
		RefreshMapIcon();
		SingletonMonoScope<ACTbar>.Instance.RefreshCD();
	}

	private void OnDestroy()
	{
		if (SingletonMonoScene<LevelInteractablesManager>.HasInstance)
		{
			SingletonMonoScene<LevelInteractablesManager>.Instance.Unregister(this);
		}
	}
}
