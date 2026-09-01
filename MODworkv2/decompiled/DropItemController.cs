using System;
using Core.Settings;
using Entity.InteractableObjects.Item;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Level.StateData.LevelStates;
using Localization;
using UnityEngine;
using UnityEngine.UI;

public class DropItemController : MonoBehaviour
{
	public ItemLevelState RuntimeState;

	private static readonly int @int = Animator.StringToHash("Int");

	public CanvasGroup canvas;

	public Text text;

	public BoxCollider2D textCol;

	public RectTransform rect;

	public DropItem UI_dropSD;

	public SpriteRenderer[] itemOBJ;

	public Collider2D[] itemCol;

	public int index;

	public GameObject[] shadow;

	public GameObject bone;

	public Animator animator;

	public int ItemType;

	public WeaponClass weapon;

	public BaoshiClass baoshi;

	public UseItemClass useitem;

	[HideInInspector]
	public GameObject FXA;

	[HideInInspector]
	public GameObject FXB;

	[HideInInspector]
	public GameObject FXC;

	public float JStime;

	public bool LuoDi;

	public Vector2 dic;

	public float speed;

	public bool startDrop;

	public DisplayItemManager displayItemManager;

	private AudioManager _audioManager;

	private Transform _transform;

	private Transform _boneTransform;

	private Transform[] _itemTransforms;

	private GameObject[] _itemGameObjects;

	private Vector2 _dropMoveDirection;

	public IDropItemData currentItem;

	public bool CanAutoPick;

	public bool PlayerDrop;

	private float timeAuto;

	private float timePlayer;

	[SerializeField]
	private SpriteRenderer mapSprite;

	private void ResetState()
	{
		currentItem = null;
		ItemType = 0;
		index = 0;
		if (weapon != null)
		{
			weapon.Reset();
		}
		else
		{
			weapon = new WeaponClass();
		}
		if (baoshi != null)
		{
			baoshi.Reset();
		}
		else
		{
			baoshi = new BaoshiClass();
		}
		if (useitem != null)
		{
			useitem.Reset();
		}
		else
		{
			useitem = new UseItemClass();
		}
		startDrop = false;
		LuoDi = false;
		JStime = 0f;
		_dropMoveDirection = Vector2.zero;
		canvas.alpha = 0f;
		text.text = string.Empty;
		textCol.enabled = false;
		for (int i = 0; i < itemOBJ.Length; i++)
		{
			SetItemObjectActive(i, active: false);
		}
		for (int j = 0; j < shadow.Length; j++)
		{
			SetActiveIfNeeded(shadow[j], active: false);
		}
		for (int k = 0; k < itemCol.Length; k++)
		{
			if (itemCol[k] != null)
			{
				itemCol[k].enabled = false;
			}
		}
		SetActiveIfNeeded(FXA, active: false);
		SetActiveIfNeeded(FXB, active: false);
		SetActiveIfNeeded(FXC, active: false);
	}

	public void InitDrop(IDropItemData item, float high, bool playAnim = true)
	{
		SetItem(item);
		SetDropHigh(high);
		if (playAnim)
		{
			SetStart();
		}
		else
		{
			SetStart(hasAnimation: false);
		}
	}

	public void SetItem(IDropItemData item)
	{
		currentItem = item;
		ItemType = item.ItemType;
		RefreshUI(LOC.MM.CurrentLanguage);
	}

	private void Awake()
	{
		_transform = base.transform;
		_boneTransform = bone.transform;
		_itemTransforms = new Transform[itemOBJ.Length];
		_itemGameObjects = new GameObject[itemOBJ.Length];
		canvas = _transform.Find("Canvas").GetComponent<CanvasGroup>();
		text = _transform.Find("Canvas/Text").GetComponent<Text>();
		textCol = _transform.Find("Canvas/Image").GetComponent<BoxCollider2D>();
		rect = _transform.Find("Canvas/Image").GetComponent<RectTransform>();
		UI_dropSD = _transform.Find("Canvas/Image").GetComponent<DropItem>();
		FXA = _transform.Find("Spine/SkeletonUtility-SkeletonRoot/root/FXA").gameObject;
		FXB = _transform.Find("Spine/SkeletonUtility-SkeletonRoot/root/FXB").gameObject;
		FXC = _transform.Find("Spine/SkeletonUtility-SkeletonRoot/root/FXC").gameObject;
		for (int i = 0; i < itemOBJ.Length; i++)
		{
			SpriteRenderer spriteRenderer = itemOBJ[i];
			_itemTransforms[i] = spriteRenderer.transform;
			_itemGameObjects[i] = spriteRenderer.gameObject;
			itemCol[i] = spriteRenderer.GetComponent<Collider2D>();
		}
		WeaponClass weaponClass = new WeaponClass();
		for (int j = 0; j < 6; j++)
		{
			WPSkill item = new WPSkill();
			weaponClass.WPSK.Add(item);
			WPAocao wPAocao = new WPAocao();
			wPAocao.HasAocao = false;
			weaponClass.Aocao.Add(wPAocao);
		}
		weapon = weaponClass;
		displayItemManager = SingletonMonoScope<DisplayItemManager>.Instance;
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
		if (!mapSprite)
		{
			mapSprite = _transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		if ((bool)SettingsLoader.Instance.iconSettings.dropItem)
		{
			mapSprite.gameObject.transform.localScale = Vector3.one * 0.2f;
			mapSprite.sprite = SettingsLoader.Instance.iconSettings.dropItem;
			mapSprite.color = SettingsLoader.Instance.iconSettings.dropItemColor;
			mapSprite.transform.localScale = SettingsLoader.Instance.iconSettings.GetDropItemFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
		}
	}

	private void Start()
	{
		RefreshUI(LOC.MM.CurrentLanguage);
	}

	private void OnEnable()
	{
		if (SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.Register(this);
		}
		LOC.MM.OnLanguageChanged += RefreshUI;
		startDrop = false;
		SetActiveIfNeeded(FXA, active: false);
		SetActiveIfNeeded(FXB, active: false);
		SetActiveIfNeeded(FXC, active: false);
		GameObject[] array = shadow;
		for (int i = 0; i < array.Length; i++)
		{
			SetActiveIfNeeded(array[i], active: false);
		}
		JStime = 0f;
		LuoDi = false;
		canvas.alpha = 0f;
		index = 0;
		dic = new Vector2(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-1f, 0.3f));
		_dropMoveDirection = dic.normalized;
		speed = UnityEngine.Random.Range(0.1f, 2f);
		timeAuto = 0f;
		timePlayer = 0f;
		CanAutoPick = false;
		DisplayItemManager obj = displayItemManager;
		obj.DropUIOn = (Action)Delegate.Combine(obj.DropUIOn, new Action(SetUI_On));
		DisplayItemManager obj2 = displayItemManager;
		obj2.DropUIOff = (Action)Delegate.Combine(obj2.DropUIOff, new Action(SetUI_Off));
	}

	private void OnDisable()
	{
		if (SingletonMonoScope<ItemManager>.HasInstance)
		{
			SingletonMonoScope<ItemManager>.Instance.Unregister(this);
		}
		LOC.MM.OnLanguageChanged -= RefreshUI;
		if ((bool)displayItemManager)
		{
			DisplayItemManager obj = displayItemManager;
			obj.DropUIOn = (Action)Delegate.Remove(obj.DropUIOn, new Action(SetUI_On));
			DisplayItemManager obj2 = displayItemManager;
			obj2.DropUIOff = (Action)Delegate.Remove(obj2.DropUIOff, new Action(SetUI_Off));
		}
		if (SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
			if ((bool)instance.xjl)
			{
				instance.xjl.Pick(this);
			}
		}
		ResetState();
	}

	public void PickUp(bool rightClick)
	{
		SingletonMonoScope<InventoryManager>.Instance.PickUp(this, rightClick);
	}

	public void AutoPickUp()
	{
		SingletonMonoScope<InventoryManager>.Instance.AutoPickUp(this);
	}

	public void SetDropHigh(float high)
	{
		animator.SetLayerWeight(1, high);
	}

	public void SetStart(bool hasAnimation = true)
	{
		startDrop = true;
		_dropMoveDirection = dic.normalized;
		if (hasAnimation)
		{
			switch (ItemType)
			{
			case 0:
				animator.SetInteger(@int, weapon.RotateType);
				break;
			case 1:
				animator.SetInteger(@int, 0);
				break;
			case 2:
				animator.SetInteger(@int, 0);
				break;
			}
		}
		else
		{
			switch (ItemType)
			{
			case 0:
				animator.SetInteger(@int, weapon.RotateType + 6);
				break;
			case 1:
				animator.SetInteger(@int, 6);
				break;
			case 2:
				animator.SetInteger(@int, 6);
				break;
			}
			dic = Vector2.zero;
			_dropMoveDirection = Vector2.zero;
			speed = 0f;
		}
		GameObject[] array = shadow;
		for (int i = 0; i < array.Length; i++)
		{
			SetActiveIfNeeded(array[i], active: false);
		}
		for (int j = 0; j < itemOBJ.Length; j++)
		{
			SetItemObjectActive(j, active: false);
		}
		switch (ItemType)
		{
		case 0:
			switch (weapon.WeaponType)
			{
			case "head":
				SetItemObjectActive(0, active: true);
				itemOBJ[0].sprite = weapon.Icon;
				index = 0;
				break;
			case "body":
				SetItemObjectActive(1, active: true);
				itemOBJ[1].sprite = weapon.Icon;
				index = 1;
				break;
			case "hand":
				SetItemObjectActive(2, active: true);
				itemOBJ[2].sprite = weapon.Icon;
				index = 2;
				break;
			case "leg":
				SetItemObjectActive(3, active: true);
				itemOBJ[3].sprite = weapon.Icon;
				index = 3;
				break;
			case "sword":
				if (weapon.Size.y == 4 && weapon.Size.x == 2)
				{
					SetItemObjectActive(4, active: true);
					itemOBJ[4].sprite = weapon.Icon;
					index = 4;
				}
				else if (weapon.Size.y == 4 && weapon.Size.x == 1)
				{
					SetItemObjectActive(5, active: true);
					itemOBJ[5].sprite = weapon.Icon;
					index = 5;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 1)
				{
					SetItemObjectActive(6, active: true);
					itemOBJ[6].sprite = weapon.Icon;
					index = 6;
				}
				break;
			case "staff":
				if (weapon.Size.y == 4 && weapon.Size.x == 2)
				{
					SetItemObjectActive(10, active: true);
					itemOBJ[10].sprite = weapon.Icon;
					index = 10;
				}
				else if (weapon.Size.y == 4 && weapon.Size.x == 1)
				{
					SetItemObjectActive(11, active: true);
					itemOBJ[11].sprite = weapon.Icon;
					index = 11;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 2)
				{
					SetItemObjectActive(12, active: true);
					itemOBJ[12].sprite = weapon.Icon;
					index = 12;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 1)
				{
					SetItemObjectActive(13, active: true);
					itemOBJ[13].sprite = weapon.Icon;
					index = 13;
				}
				break;
			case "bow":
				if (weapon.Size.y == 4 && weapon.Size.x == 2)
				{
					SetItemObjectActive(14, active: true);
					itemOBJ[14].sprite = weapon.Icon;
					index = 14;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 2)
				{
					SetItemObjectActive(15, active: true);
					itemOBJ[15].sprite = weapon.Icon;
					index = 15;
				}
				else if (weapon.Size.y == 4 && weapon.Size.x == 1)
				{
					SetItemObjectActive(16, active: true);
					itemOBJ[16].sprite = weapon.Icon;
					index = 16;
				}
				break;
			case "arrow":
				if (weapon.Size.y == 4 && weapon.Size.x == 2)
				{
					SetItemObjectActive(17, active: true);
					itemOBJ[17].sprite = weapon.Icon;
					index = 17;
				}
				else if (weapon.Size.y == 4 && weapon.Size.x == 1)
				{
					SetItemObjectActive(18, active: true);
					itemOBJ[18].sprite = weapon.Icon;
					index = 18;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 1)
				{
					SetItemObjectActive(19, active: true);
					itemOBJ[19].sprite = weapon.Icon;
					index = 19;
				}
				break;
			case "bone":
				if (weapon.Size.y == 4 && weapon.Size.x == 2)
				{
					SetItemObjectActive(20, active: true);
					itemOBJ[20].sprite = weapon.Icon;
					index = 20;
				}
				else if (weapon.Size.y == 4 && weapon.Size.x == 1)
				{
					SetItemObjectActive(21, active: true);
					itemOBJ[21].sprite = weapon.Icon;
					index = 21;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 2)
				{
					SetItemObjectActive(22, active: true);
					itemOBJ[22].sprite = weapon.Icon;
					index = 22;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 1)
				{
					SetItemObjectActive(23, active: true);
					itemOBJ[23].sprite = weapon.Icon;
					index = 23;
				}
				break;
			case "shield":
				if (weapon.Size.y == 4 && weapon.Size.x == 2)
				{
					SetItemObjectActive(7, active: true);
					itemOBJ[7].sprite = weapon.Icon;
					index = 7;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 2)
				{
					SetItemObjectActive(8, active: true);
					itemOBJ[8].sprite = weapon.Icon;
					index = 8;
				}
				else if (weapon.Size.y == 2 && weapon.Size.x == 2)
				{
					SetItemObjectActive(9, active: true);
					itemOBJ[9].sprite = weapon.Icon;
					index = 9;
				}
				break;
			case "spell":
				if (weapon.Size.y == 2 && weapon.Size.x == 2)
				{
					SetItemObjectActive(24, active: true);
					itemOBJ[24].sprite = weapon.Icon;
					index = 24;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 2)
				{
					SetItemObjectActive(25, active: true);
					itemOBJ[25].sprite = weapon.Icon;
					index = 25;
				}
				break;
			case "corpse":
				if (weapon.Size.y == 4 && weapon.Size.x == 2)
				{
					SetItemObjectActive(26, active: true);
					itemOBJ[26].sprite = weapon.Icon;
					index = 26;
				}
				else if (weapon.Size.y == 3 && weapon.Size.x == 2)
				{
					SetItemObjectActive(27, active: true);
					itemOBJ[27].sprite = weapon.Icon;
					index = 27;
				}
				else if (weapon.Size.y == 2 && weapon.Size.x == 2)
				{
					SetItemObjectActive(28, active: true);
					itemOBJ[28].sprite = weapon.Icon;
					index = 28;
				}
				break;
			case "little":
				SetItemObjectActive(29, active: true);
				itemOBJ[29].sprite = weapon.Icon;
				index = 29;
				break;
			}
			break;
		case 1:
			switch (baoshi.DropSpriteSize)
			{
			case 0:
				SetItemObjectActive(29, active: true);
				itemOBJ[29].sprite = baoshi.Icon;
				index = 29;
				break;
			case 1:
				SetItemObjectActive(30, active: true);
				itemOBJ[30].sprite = baoshi.Icon;
				index = 30;
				break;
			}
			break;
		case 2:
			switch (useitem.DropSpriteSize)
			{
			case 0:
				SetItemObjectActive(29, active: true);
				itemOBJ[29].sprite = useitem.Icon;
				index = 29;
				break;
			case 1:
				SetItemObjectActive(30, active: true);
				itemOBJ[30].sprite = useitem.Icon;
				index = 30;
				break;
			}
			break;
		}
		switch (ItemType)
		{
		case 0:
			switch (weapon.Quality)
			{
			case 4:
				SetActiveIfNeeded(FXC, active: true);
				break;
			case 5:
				SetActiveIfNeeded(FXB, active: true);
				break;
			case 6:
				SetActiveIfNeeded(FXA, active: true);
				break;
			}
			break;
		case 1:
			switch (baoshi.Quality)
			{
			case 4:
				SetActiveIfNeeded(FXC, active: true);
				break;
			case 5:
				SetActiveIfNeeded(FXB, active: true);
				break;
			case 6:
				SetActiveIfNeeded(FXA, active: true);
				break;
			}
			break;
		case 2:
			switch (useitem.Quality)
			{
			case 4:
				SetActiveIfNeeded(FXC, active: true);
				break;
			case 5:
				SetActiveIfNeeded(FXB, active: true);
				break;
			case 6:
				SetActiveIfNeeded(FXA, active: true);
				break;
			}
			break;
		}
		UI_dropSD.render = itemOBJ[index];
		if (!hasAnimation)
		{
			FinishLanding(hasAnim: false);
		}
		else
		{
			_audioManager.SoundString(_transform, "event:/UI/Scene/Drop_Item");
		}
	}

	private void FinishLanding(bool hasAnim = true)
	{
		JStime = 0f;
		LuoDi = true;
		if ((bool)displayItemManager && displayItemManager.DropItemUI_IsOpened)
		{
			canvas.alpha = 1f;
			textCol.enabled = true;
			if (itemCol != null && index >= 0 && index < itemCol.Length && itemCol[index] != null)
			{
				itemCol[index].enabled = false;
			}
		}
		else
		{
			canvas.alpha = 0f;
			textCol.enabled = false;
			if (itemCol != null && index >= 0 && index < itemCol.Length && itemCol[index] != null)
			{
				itemCol[index].enabled = true;
			}
		}
		if (!hasAnim)
		{
			return;
		}
		switch (ItemType)
		{
		case 0:
			switch (weapon.Quality)
			{
			case 4:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/A");
				break;
			case 5:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/B");
				break;
			case 6:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/C");
				break;
			}
			break;
		case 1:
			switch (baoshi.Quality)
			{
			case 4:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/A");
				break;
			case 5:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/B");
				break;
			case 6:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/C");
				break;
			}
			break;
		case 2:
			switch (useitem.Quality)
			{
			case 4:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/A");
				break;
			case 5:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/B");
				break;
			case 6:
				_audioManager.SoundString(_transform, "event:/UI/Scene/DropLegened/C");
				break;
			}
			break;
		}
	}

	private void Update()
	{
		if (!startDrop)
		{
			return;
		}
		_itemTransforms[index].SetPositionAndRotation(_boneTransform.position, _boneTransform.rotation);
		if (!LuoDi)
		{
			float deltaTime = Time.deltaTime;
			_transform.Translate(_dropMoveDirection * (speed * deltaTime));
			JStime += deltaTime;
			if (!(JStime >= 0.425f))
			{
				return;
			}
			Vector3 position = _transform.position;
			switch (ItemType)
			{
			case 0:
				switch (weapon.WeaponType)
				{
				case "head":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Head.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[0], active: true);
					break;
				case "leg":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Shoes.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[0], active: true);
					break;
				case "body":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Armor.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[1], active: true);
					break;
				case "hand":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Hand.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[2], active: true);
					break;
				case "sword":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Sword.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[3], active: true);
					break;
				case "staff":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Staff.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[3], active: true);
					break;
				case "bone":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Staff.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[3], active: true);
					break;
				case "shield":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Dun.DP[weapon.SoundDrop], position);
					switch (weapon.Size.y)
					{
					case 2:
						SetActiveIfNeeded(shadow[4], active: true);
						break;
					case 3:
					case 4:
						SetActiveIfNeeded(shadow[1], active: true);
						break;
					}
					break;
				case "spell":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Book.DP[weapon.SoundDrop], position);
					if ((uint)(weapon.Size.y - 2) <= 1u)
					{
						SetActiveIfNeeded(shadow[2], active: true);
					}
					break;
				case "corpse":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Offering.DP[weapon.SoundDrop], position);
					switch (weapon.Size.y)
					{
					case 2:
						SetActiveIfNeeded(shadow[4], active: true);
						break;
					case 3:
					case 4:
						SetActiveIfNeeded(shadow[1], active: true);
						break;
					}
					break;
				case "bow":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Bow.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[0], active: true);
					break;
				case "arrow":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_Arrow.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[5], active: true);
					break;
				case "little":
					RuntimeManager.PlayOneShot(_audioManager.audioData.WP_ORB.DP[weapon.SoundDrop], position);
					SetActiveIfNeeded(shadow[6], active: true);
					break;
				}
				break;
			case 1:
				RuntimeManager.PlayOneShot(_audioManager.audioData.Baoshi.DP[baoshi.SoundDrop], position);
				switch (baoshi.DropSpriteSize)
				{
				case 0:
					SetActiveIfNeeded(shadow[6], active: true);
					break;
				case 1:
					SetActiveIfNeeded(shadow[7], active: true);
					break;
				}
				break;
			case 2:
				switch (useitem.InfoType)
				{
				case 0:
					RuntimeManager.PlayOneShot(_audioManager.audioData.Potion.DP[useitem.SoundDrop], position);
					break;
				case 1:
					RuntimeManager.PlayOneShot(_audioManager.audioData.Potion.DP[useitem.SoundDrop], position);
					break;
				case 2:
					RuntimeManager.PlayOneShot(_audioManager.audioData.Scoll.DP[useitem.SoundDrop], position);
					break;
				case 3:
					RuntimeManager.PlayOneShot(_audioManager.audioData.Potion.DP[useitem.SoundDrop], position);
					break;
				case 4:
					RuntimeManager.PlayOneShot(_audioManager.audioData.SPC.DP[useitem.SoundDrop], position);
					break;
				case 5:
					RuntimeManager.PlayOneShot(_audioManager.audioData.Potion.DP[useitem.SoundDrop], position);
					break;
				case 6:
					RuntimeManager.PlayOneShot(_audioManager.audioData.SPC.DP[useitem.SoundDrop], position);
					break;
				case 7:
					RuntimeManager.PlayOneShot(_audioManager.audioData.SPC.DP[useitem.SoundDrop], position);
					break;
				}
				switch (useitem.DropSpriteSize)
				{
				case 0:
					SetActiveIfNeeded(shadow[6], active: true);
					break;
				case 1:
					SetActiveIfNeeded(shadow[7], active: true);
					break;
				}
				break;
			}
			FinishLanding();
		}
		else if (PlayerDrop)
		{
			timePlayer += Time.deltaTime;
			if (timePlayer >= 6000f)
			{
				CanAutoPick = true;
			}
		}
		else
		{
			timeAuto += Time.deltaTime;
			if (timeAuto >= 0.3f)
			{
				CanAutoPick = true;
			}
		}
	}

	private void RefreshUI(LanguageType lang)
	{
		if (currentItem == null)
		{
			text.text = string.Empty;
			return;
		}
		text.text = currentItem.GetTitle(display: false);
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		float num = (text.preferredWidth * text.rectTransform.localScale.x + 15f) / rect.localScale.x;
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
		textCol.size = new Vector2(num, 12f);
	}

	private void SetUI_On()
	{
		canvas.alpha = 1f;
		textCol.enabled = true;
		itemCol[index].enabled = false;
	}

	private void SetUI_Off()
	{
		canvas.alpha = 0f;
		textCol.enabled = false;
		itemCol[index].enabled = true;
	}

	private void SetItemObjectActive(int itemIndex, bool active)
	{
		SetActiveIfNeeded((_itemGameObjects != null) ? _itemGameObjects[itemIndex] : itemOBJ[itemIndex].gameObject, active);
	}

	private static void SetActiveIfNeeded(GameObject obj, bool active)
	{
		if (obj.activeSelf != active)
		{
			obj.SetActive(active);
		}
	}
}
