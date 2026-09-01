using System.Collections.Generic;
using Core;
using Core.Settings;
using Core.Teleport;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Interact;
using Level.LevelStates;
using UI.Panels;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportStation : InteractableBase, ILevelLockable
{
	public CanvasGroup canvas;

	public Text text;

	public RectTransform rect;

	public SpriteRenderer render;

	public Light2D lit;

	public LevelRoot root;

	public List<GameObject> fires = new List<GameObject>();

	public SpriteRenderer mapIcon;

	[HideInInspector]
	public float size;

	[Header("本传送站的章节Id (仅用于主城)")]
	public int ChapterId;

	private int ChapterIdInLevel;

	private bool isInRange;

	private static readonly int liang = Shader.PropertyToID("_Liang");

	private bool canUseInHome;

	private bool isHomeScene;

	private bool isLocked;

	public override InteractionType Type => InteractionType.openUI;

	private static float EnterRange => SettingsLoader.Instance.teleportInteractDis + 0.2f;

	private static float LeaveRange => SettingsLoader.Instance.teleportInteractDis + 0.4f;

	private void ControlText()
	{
		if ((bool)text)
		{
			text.fontSize = 45;
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);
		}
		if ((bool)rect)
		{
			RectTransform component = rect.GetComponent<RectTransform>();
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 120f);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 18f);
		}
	}

	private void Awake()
	{
		if (SceneManager.GetActiveScene().name == "HomeScene")
		{
			isHomeScene = true;
		}
		else
		{
			isHomeScene = false;
		}
		if (!mapIcon)
		{
			mapIcon = base.transform.Find("Map").GetComponent<SpriteRenderer>();
		}
		render = GetComponent<SpriteRenderer>();
		canvas = base.transform.Find("Canvas").GetComponent<CanvasGroup>();
		text = base.transform.Find("Canvas/Text").GetComponent<Text>();
		rect = base.transform.Find("Canvas/Image").GetComponent<RectTransform>();
		lit = GetComponent<Light2D>();
		if (fires.Count == 0)
		{
			fires.Add(base.transform.Find("A/main/fire").gameObject);
			fires.Add(base.transform.Find("B/main/fire").gameObject);
			fires.Add(base.transform.Find("C/main/fire").gameObject);
			fires.Add(base.transform.Find("D/main/fire").gameObject);
		}
		if (!isHomeScene && SingletonMonoScope<LevelManager>.HasInstance)
		{
			ChapterIdInLevel = LevelManager.GetCurChapterId();
		}
		if (!root)
		{
			root = GetComponentInParent<LevelRoot>();
		}
		lit.pointLightInnerRadius = 0.8f * size;
		lit.pointLightOuterRadius = 1.5f * size;
		lit.intensity = 0.3f;
		canvas.alpha = 0f;
		if (SingletonMonoScene<HomeSceneManager>.HasInstance)
		{
			SingletonMonoScene<HomeSceneManager>.Instance.RegisterStation(this);
		}
		if (!isHomeScene && SingletonMonoScope<TeleportManager>.HasInstance)
		{
			SingletonMonoScope<TeleportManager>.Instance.Register(this);
		}
		if ((bool)root)
		{
			root.RegisterLockable(this);
		}
		if ((bool)SettingsLoader.Instance.iconSettings.station)
		{
			mapIcon.gameObject.transform.localScale = Vector3.one * 0.6f;
			mapIcon.sprite = SettingsLoader.Instance.iconSettings.station;
			mapIcon.transform.localScale = SettingsLoader.Instance.iconSettings.GetStationFinalScale(Singleton<SettingDataManager>.Instance.GetInterface().map_view_range, Singleton<SettingDataManager>.Instance.GetInterface().map_scale);
			mapIcon.color = SettingsLoader.Instance.iconSettings.stationUnlockColor;
		}
		ControlText();
		ControlInteractInHome();
	}

	private void ShowVisual()
	{
		mapIcon.color = SettingsLoader.Instance.iconSettings.stationUnlockColor;
		if ((bool)lit)
		{
			lit.pointLightInnerRadius = 0.8f * size;
			lit.pointLightOuterRadius = 1.5f * size;
			lit.intensity = 0.3f;
		}
		if (fires.Count <= 0)
		{
			return;
		}
		foreach (GameObject fire in fires)
		{
			fire.SetActive(value: true);
		}
	}

	private void HideVisual()
	{
		mapIcon.color = SettingsLoader.Instance.iconSettings.stationLockColor;
		if ((bool)lit)
		{
			lit.enabled = false;
		}
		if (fires.Count <= 0)
		{
			return;
		}
		foreach (GameObject fire in fires)
		{
			fire.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (SingletonMonoScene<HomeSceneManager>.HasInstance)
		{
			SingletonMonoScene<HomeSceneManager>.Instance.NotifyStationReady();
		}
	}

	private void OnDestroy()
	{
		if (SingletonMonoScene<HomeSceneManager>.HasInstance)
		{
			SingletonMonoScene<HomeSceneManager>.Instance.Unregister(this);
		}
		if (SingletonMonoScope<TeleportManager>.HasInstance)
		{
			SingletonMonoScope<TeleportManager>.Instance.Unregister(this);
		}
	}

	public override bool CanInteract()
	{
		if (SingletonMonoScope<PlayerManager>.HasInstance && !isLocked)
		{
			return Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < SettingsLoader.Instance.teleportInteractDis;
		}
		return false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<PlayerManager>.HasInstance)
		{
			return;
		}
		float sqrMagnitude = (base.transform.position - SingletonMonoScope<PlayerManager>.Instance.transform.position).sqrMagnitude;
		if (isInRange)
		{
			if (sqrMagnitude > LeaveRange * LeaveRange)
			{
				isInRange = false;
				Singleton<UIManager>.Instance.HideAllPanels();
			}
		}
		else if (sqrMagnitude <= EnterRange * EnterRange)
		{
			isInRange = true;
		}
	}

	public override void Interact()
	{
		lit.pointLightInnerRadius = 1f * size;
		lit.pointLightOuterRadius = 2f * size;
		lit.intensity = 0.5f;
		if (isHomeScene)
		{
			if (canUseInHome)
			{
				Singleton<UIManager>.Instance.ShowExclusivePanel<TeleportPanel, int>(ChapterId);
				Time.timeScale = 0f;
			}
			else
			{
				GameManager.ShowTip(LOC.MM.GetLevel("not_unlock"), TipType.Fail);
			}
		}
		else
		{
			Singleton<UIManager>.Instance.ShowExclusivePanel<TeleportPanel, int>(ChapterIdInLevel);
			Time.timeScale = 0f;
		}
	}

	protected override void OnHover(bool isHovering)
	{
		if (isLocked)
		{
			return;
		}
		if (isHovering)
		{
			render.material.SetFloat(liang, 0f);
			if (isHomeScene)
			{
				text.text = LOC.MM.GetLevel("Chapter" + ChapterId);
				text.horizontalOverflow = HorizontalWrapMode.Overflow;
				float num = (text.preferredWidth * text.rectTransform.localScale.x + 15f) / rect.localScale.x;
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
			}
			else
			{
				text.text = LOC.MM.GetLevel("teleport_ui_title");
				text.horizontalOverflow = HorizontalWrapMode.Overflow;
				float num2 = (text.preferredWidth * text.rectTransform.localScale.x + 15f) / rect.localScale.x;
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num2);
			}
			canvas.alpha = 1f;
		}
		else
		{
			render.material.SetFloat(liang, 1f);
			canvas.alpha = 0f;
		}
	}

	public void SetLocked(bool locked)
	{
		if (isHomeScene)
		{
			isLocked = false;
			return;
		}
		isLocked = locked;
		if (locked)
		{
			HideVisual();
		}
		else
		{
			ShowVisual();
		}
	}

	public void ControlInteractInHome()
	{
		if (isHomeScene)
		{
			if (SaveManager.RuntimeData.UnlockedChapterIds.Contains(ChapterId))
			{
				canUseInHome = true;
				ShowVisual();
			}
			else
			{
				canUseInHome = false;
				HideVisual();
			}
		}
	}
}
