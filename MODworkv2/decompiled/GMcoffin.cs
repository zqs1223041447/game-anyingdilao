using Core.Settings;
using FinkFramework.Runtime.Singleton;
using Interact;
using Level.LevelStates;
using Level.StateData.LevelStates;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class GMcoffin : InteractableBase, IInteractableState
{
	[SerializeField]
	private SpriteRenderer mapSprite;

	private static readonly int open = Animator.StringToHash("open");

	private static readonly int liang = Shader.PropertyToID("_Liang");

	[HideInInspector]
	public CanvasGroup canvas;

	[HideInInspector]
	public Text text;

	[HideInInspector]
	public RectTransform rect;

	private bool Opened;

	private SkeletonMecanim mecanim;

	private Animator ani;

	private int doorskin;

	private SpriteRenderer renderA;

	private SpriteRenderer renderB;

	private MeshRenderer renderC;

	private Transform point;

	public float high;

	public int sound;

	private static Sprite opened => SettingsLoader.Instance.iconSettings.openedChest;

	private static Color openedChestColor => SettingsLoader.Instance.iconSettings.openedChestColor;

	private static Sprite newed => SettingsLoader.Instance.iconSettings.newChest;

	private static Color newChestColor => SettingsLoader.Instance.iconSettings.newChestColor;

	public string RuntimeId { get; private set; }

	public override InteractionType Type => InteractionType.Chest;

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
		Opened = false;
		mecanim = base.transform.Find("main/Spine").gameObject.GetComponent<SkeletonMecanim>();
		ani = base.transform.Find("main/Spine").gameObject.GetComponent<Animator>();
		renderA = base.transform.Find("main/base").gameObject.GetComponent<SpriteRenderer>();
		renderB = base.transform.Find("main/Q").gameObject.GetComponent<SpriteRenderer>();
		renderC = base.transform.Find("main/Spine").gameObject.GetComponent<MeshRenderer>();
		point = base.transform.Find("point").gameObject.transform;
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		text = base.transform.Find("Canvas/Text").GetComponent<Text>();
		rect = base.transform.Find("Canvas/Image").GetComponent<RectTransform>();
		doorskin = Random.Range(0, 2);
		Skeleton skeleton = mecanim.Skeleton;
		SkeletonData data = skeleton.Data;
		Skin skin = new Skin("custom");
		if (doorskin == 0)
		{
			skin.AddSkin(data.FindSkin("bbbb"));
			skeleton.SetSkin(skin);
			skeleton.SetSlotsToSetupPose();
		}
		else
		{
			skin.AddSkin(data.FindSkin("cccc"));
			skeleton.SetSkin(skin);
			skeleton.SetSlotsToSetupPose();
		}
		ani.SetBool(open, value: false);
		canvas.alpha = 0f;
		RefreshMapIcon();
	}

	public override bool CanInteract()
	{
		if (Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.l_coffinInteractDis)
		{
			return !Opened;
		}
		return false;
	}

	public override void Interact()
	{
		ani.SetBool(open, value: true);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetFloat(liang, 1f);
		renderC.SetPropertyBlock(materialPropertyBlock);
		renderA.material.SetFloat(liang, 1f);
		renderB.material.SetFloat(liang, 1f);
		this.wait(0.4f, delegate
		{
			SingletonMonoScope<ItemManager>.Instance.ChestDrop(point, high, 1);
		});
		Opened = true;
		canvas.alpha = 0f;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, 8);
		RefreshMapIcon();
	}

	protected override void OnHover(bool isHovering)
	{
		if (!Opened)
		{
			if (isHovering)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetFloat(liang, 0f);
				renderC.SetPropertyBlock(materialPropertyBlock);
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
				MaterialPropertyBlock materialPropertyBlock2 = new MaterialPropertyBlock();
				materialPropertyBlock2.SetFloat(liang, 1f);
				renderC.SetPropertyBlock(materialPropertyBlock2);
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
				value = new InteractableLevelState(InteractableType.GMCoffin);
				currentLevelState.Interactables.Add(RuntimeId, value);
			}
			value.InteractableType = InteractableType.GMCoffin;
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
		ani.SetBool(open, value: true);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetFloat(liang, 1f);
		renderC.SetPropertyBlock(materialPropertyBlock);
		renderA.material.SetFloat(liang, 1f);
		renderB.material.SetFloat(liang, 1f);
		Opened = true;
		canvas.alpha = 0f;
		SingletonMonoGlobal<AudioManager>.Instance.SceneOpen(base.transform, 8);
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
