using Core;
using Core.Teleport;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ACT_TP : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private const string TipKey = "Open Homeward Portal";

	private const float TipDelay = 0.2f;

	private const float OpenInterval = 1f;

	private const ControlAction ShortcutAction = ControlAction.TP;

	public Sprite Main;

	public Sprite Lit;

	public string OpenSound;

	private static ACT_TP instance;

	private Image image;

	private Button button;

	private GameUIManager gameUIManager;

	private float hoverTime;

	private float nextOpenTime;

	private bool hovering;

	private bool tipShown;

	private void Awake()
	{
		instance = this;
		image = GetComponent<Image>();
		button = GetComponent<Button>();
		gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		if ((bool)image && (bool)Main)
		{
			image.sprite = Main;
		}
		if ((bool)button)
		{
			button.onClick.AddListener(OpenHomePortal);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
		if ((bool)button)
		{
			button.onClick.RemoveListener(OpenHomePortal);
		}
	}

	private void OnEnable()
	{
		ResetHoverState();
	}

	private void OnDisable()
	{
		HideTip();
		ResetHoverState();
	}

	private void Update()
	{
		if (!hovering)
		{
			return;
		}
		if (ShouldSuppressShortcutTip())
		{
			HideTip();
			return;
		}
		if (!tipShown)
		{
			hoverTime += Time.deltaTime;
			if (hoverTime < 0.2f)
			{
				return;
			}
			tipShown = true;
		}
		if ((bool)gameUIManager)
		{
			gameUIManager.ShowTipWithShortcut(base.transform, "Open Homeward Portal", ControlAction.TP);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovering = true;
		tipShown = false;
		hoverTime = 0f;
		if ((bool)image && (bool)Lit)
		{
			image.sprite = Lit;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HideTip();
		ResetHoverState();
	}

	public static void OpenFromShortcut()
	{
		if (!instance)
		{
			instance = Object.FindObjectOfType<ACT_TP>();
		}
		if ((bool)instance)
		{
			instance.OpenHomePortal();
		}
	}

	private static bool ShouldSuppressShortcutTip()
	{
		if (SingletonMonoScope<GameUIManager>.HasInstance)
		{
			return SingletonMonoScope<GameUIManager>.Instance.Opened_IV;
		}
		return false;
	}

	private void OpenHomePortal()
	{
		if (Time.time < nextOpenTime || (SingletonMonoScope<PlayerManager>.HasInstance && !SingletonMonoScope<PlayerManager>.Instance.IsAlive))
		{
			return;
		}
		if (!InventoryManager.CheckScrollUseLimit(checkHomeScene: true))
		{
			GameManager.ShowTip(LOC.MM.GetLevel("portal_hint_no"), TipType.Fail);
		}
		else if (SingletonMonoScope<PortalManager>.HasInstance)
		{
			nextOpenTime = Time.time + 1f;
			SingletonMonoScope<PortalManager>.Instance.RequestOpenGoBackHomePortal();
			string text = OpenSound;
			if (string.IsNullOrWhiteSpace(text) && SingletonMonoGlobal<AudioManager>.HasInstance && (bool)SingletonMonoGlobal<AudioManager>.Instance.audioData)
			{
				text = SingletonMonoGlobal<AudioManager>.Instance.audioData.Quick_SK_Select;
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				RuntimeManager.PlayOneShot(text);
			}
		}
	}

	private void ResetHoverState()
	{
		hovering = false;
		tipShown = false;
		hoverTime = 0f;
		if ((bool)image && (bool)Main)
		{
			image.sprite = Main;
		}
	}

	private void HideTip()
	{
		if ((bool)gameUIManager)
		{
			gameUIManager.HideEmptyTip();
		}
	}
}
