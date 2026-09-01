using System;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Level.LevelStates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Map;

public class MapManager : SingletonMonoScope<MapManager>
{
	[Header("地图根节点")]
	public GameObject mapRoot;

	[Header("各地图模式节点")]
	public GameObject minimap;

	public GameObject worldmapL;

	public GameObject worldmapC;

	public GameObject worldmapR;

	[Header("小地图UI组件")]
	public RawImage minimapRawImage;

	public CanvasGroup minimapCanvasGroup;

	[Header("世界地图UI组件")]
	public RawImage worldmapRawImageL;

	public CanvasGroup worldmapCanvasGroupL;

	public RawImage worldmapRawImageC;

	public CanvasGroup worldmapCanvasGroupC;

	public RawImage worldmapRawImageR;

	public CanvasGroup worldmapCanvasGroupR;

	[Header("地图摄像机")]
	public Camera minimapCam;

	public Camera worldmapCam;

	public LevelRoot currentLevelRoot;

	private const float WorldMapAlphaBonus = 1.5f;

	public MapDisplayMode CurrentMode { get; private set; }

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		BindReferences();
	}

	public void RegisterLevelRoot(LevelRoot root)
	{
		if ((bool)root)
		{
			currentLevelRoot = root;
			root.GetMapSprite();
			ApplyLevelMapSettings();
		}
	}

	public void UnregisterLevelRoot(LevelRoot root)
	{
		if ((bool)root && !(currentLevelRoot != root))
		{
			currentLevelRoot = null;
		}
	}

	private void ApplyLevelMapSettings()
	{
		if ((bool)currentLevelRoot)
		{
			currentLevelRoot.SetLevelMapAlpha(Singleton<SettingDataManager>.Instance.Interface.map_border_alpha);
		}
	}

	private void Start()
	{
		BindReferences();
		SetMapScale(Singleton<SettingDataManager>.Instance.Interface.map_scale);
		SetMapView(Singleton<SettingDataManager>.Instance.Interface.map_view_range);
		SetMapGlobalAlpha(Singleton<SettingDataManager>.Instance.Interface.map_global_alpha);
		SetMapBorderAlpha(Singleton<SettingDataManager>.Instance.Interface.map_border_alpha);
		SetMode(Singleton<SettingDataManager>.Instance.Interface.map_mode);
		SetEnable(Singleton<SettingDataManager>.Instance.Interface.map_toggle);
	}

	private void BindReferences()
	{
		if (!mapRoot && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			Transform transform = SingletonMonoScope<GameUIManager>.Instance.transform.Find("UICanvas/Map");
			if ((bool)transform)
			{
				mapRoot = transform.gameObject;
			}
		}
		if ((bool)mapRoot)
		{
			if (!minimap)
			{
				Transform transform2 = mapRoot.transform.Find("Minimap");
				if ((bool)transform2)
				{
					minimap = transform2.gameObject;
				}
			}
			if (!worldmapL)
			{
				Transform transform3 = mapRoot.transform.Find("WorldmapL");
				if ((bool)transform3)
				{
					worldmapL = transform3.gameObject;
				}
			}
			if (!worldmapC)
			{
				Transform transform4 = mapRoot.transform.Find("WorldmapC");
				if ((bool)transform4)
				{
					worldmapC = transform4.gameObject;
				}
			}
			if (!worldmapR)
			{
				Transform transform5 = mapRoot.transform.Find("WorldmapR");
				if ((bool)transform5)
				{
					worldmapR = transform5.gameObject;
				}
			}
		}
		if (!minimapRawImage && (bool)minimap)
		{
			minimapRawImage = minimap.GetComponentInChildren<RawImage>(includeInactive: true);
		}
		if (!worldmapRawImageL && (bool)worldmapL)
		{
			worldmapRawImageL = worldmapL.GetComponentInChildren<RawImage>(includeInactive: true);
		}
		if (!worldmapRawImageC && (bool)worldmapC)
		{
			worldmapRawImageC = worldmapC.GetComponentInChildren<RawImage>(includeInactive: true);
		}
		if (!worldmapRawImageR && (bool)worldmapR)
		{
			worldmapRawImageR = worldmapR.GetComponentInChildren<RawImage>(includeInactive: true);
		}
		if (!minimapCanvasGroup && (bool)minimap)
		{
			minimapCanvasGroup = minimap.GetComponent<CanvasGroup>();
		}
		if (!worldmapCanvasGroupL && (bool)worldmapL)
		{
			worldmapCanvasGroupL = worldmapL.GetComponent<CanvasGroup>();
		}
		if (!worldmapCanvasGroupC && (bool)worldmapC)
		{
			worldmapCanvasGroupC = worldmapC.GetComponent<CanvasGroup>();
		}
		if (!worldmapCanvasGroupR && (bool)worldmapR)
		{
			worldmapCanvasGroupR = worldmapR.GetComponent<CanvasGroup>();
		}
		if (!minimapCam && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			minimapCam = SingletonMonoScope<PlayerManager>.Instance.minimapCam;
		}
		if (!worldmapCam && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			worldmapCam = SingletonMonoScope<PlayerManager>.Instance.worldmapCam;
		}
	}

	public void SetEnable(bool enable)
	{
		if (!mapRoot)
		{
			return;
		}
		mapRoot.SetActive(enable);
		if (!enable)
		{
			if ((bool)minimapCam)
			{
				minimapCam.gameObject.SetActive(value: false);
			}
			if ((bool)worldmapCam)
			{
				worldmapCam.gameObject.SetActive(value: false);
			}
		}
		else
		{
			RefreshModeVisible();
		}
	}

	public void SetMode(MapDisplayMode mode)
	{
		CurrentMode = mode;
		if ((bool)mapRoot && mapRoot.activeSelf)
		{
			RefreshModeVisible();
		}
	}

	private void RefreshModeVisible()
	{
		HideAllModes();
		switch (CurrentMode)
		{
		case MapDisplayMode.Minimap:
			if ((bool)minimapCam && !minimapCam.gameObject.activeSelf)
			{
				minimapCam.gameObject.SetActive(value: true);
			}
			if ((bool)worldmapCam && worldmapCam.gameObject.activeSelf)
			{
				worldmapCam.gameObject.SetActive(value: false);
			}
			if ((bool)minimap)
			{
				minimap.SetActive(value: true);
			}
			break;
		case MapDisplayMode.WorldLeft:
			if ((bool)minimapCam && minimapCam.gameObject.activeSelf)
			{
				minimapCam.gameObject.SetActive(value: false);
			}
			if ((bool)worldmapCam && !worldmapCam.gameObject.activeSelf)
			{
				worldmapCam.gameObject.SetActive(value: true);
			}
			if ((bool)worldmapL)
			{
				worldmapL.SetActive(value: true);
			}
			break;
		case MapDisplayMode.WorldCenter:
			if ((bool)minimapCam && minimapCam.gameObject.activeSelf)
			{
				minimapCam.gameObject.SetActive(value: false);
			}
			if ((bool)worldmapCam && !worldmapCam.gameObject.activeSelf)
			{
				worldmapCam.gameObject.SetActive(value: true);
			}
			if ((bool)worldmapC)
			{
				worldmapC.SetActive(value: true);
			}
			break;
		case MapDisplayMode.WorldRight:
			if ((bool)minimapCam && minimapCam.gameObject.activeSelf)
			{
				minimapCam.gameObject.SetActive(value: false);
			}
			if ((bool)worldmapCam && !worldmapCam.gameObject.activeSelf)
			{
				worldmapCam.gameObject.SetActive(value: true);
			}
			if ((bool)worldmapR)
			{
				worldmapR.SetActive(value: true);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void HideAllModes()
	{
		if ((bool)minimap)
		{
			minimap.SetActive(value: false);
		}
		if ((bool)worldmapL)
		{
			worldmapL.SetActive(value: false);
		}
		if ((bool)worldmapC)
		{
			worldmapC.SetActive(value: false);
		}
		if ((bool)worldmapR)
		{
			worldmapR.SetActive(value: false);
		}
		if ((bool)minimapCam)
		{
			minimapCam.gameObject.SetActive(value: false);
		}
		if ((bool)worldmapCam)
		{
			worldmapCam.gameObject.SetActive(value: false);
		}
	}

	public void SetMapScale(float v)
	{
		Vector2 size = new Vector2(450f * v, 450f * v);
		Vector2 size2 = new Vector2(900f * v, 600f * v);
		SetMapNodeSize(minimap, size);
		SetMapNodeSize(worldmapL, size2);
		SetMapNodeSize(worldmapC, size2);
		SetMapNodeSize(worldmapR, size2);
	}

	private static void SetMapNodeSize(GameObject node, Vector2 size)
	{
		if ((bool)node)
		{
			RectTransform component = node.GetComponent<RectTransform>();
			if ((bool)component)
			{
				component.sizeDelta = size;
			}
		}
	}

	public void SetMapView(float v)
	{
		if ((bool)minimapCam)
		{
			float orthographicSize = 25f * v;
			minimapCam.orthographicSize = orthographicSize;
		}
		if ((bool)worldmapCam)
		{
			float orthographicSize2 = 35f * v;
			worldmapCam.orthographicSize = orthographicSize2;
		}
	}

	public void SetMapGlobalAlpha(float v)
	{
		float alpha = Mathf.Clamp01(v);
		float alpha2 = Mathf.Clamp01(v * 1.5f);
		if ((bool)minimapCanvasGroup)
		{
			minimapCanvasGroup.alpha = alpha;
		}
		if ((bool)worldmapCanvasGroupL)
		{
			worldmapCanvasGroupL.alpha = alpha2;
		}
		if ((bool)worldmapCanvasGroupC)
		{
			worldmapCanvasGroupC.alpha = alpha2;
		}
		if ((bool)worldmapCanvasGroupR)
		{
			worldmapCanvasGroupR.alpha = alpha2;
		}
	}

	public void SetMapBorderAlpha(float v)
	{
		if ((bool)currentLevelRoot)
		{
			currentLevelRoot.SetLevelMapAlpha(v);
		}
	}

	private void Update()
	{
		if (Time.timeScale != 0f && !GamepadUIActionManager.IsGameplayActionBlocked(ControlAction.MapMode) && InputBind.GetDown(ControlAction.MapMode))
		{
			Singleton<SettingDataManager>.Instance.ToggleMapModeImmediate();
		}
	}

	public static string GetLocalMapMode(MapDisplayMode mode)
	{
		return mode switch
		{
			MapDisplayMode.Minimap => LOC.MM.GetStart("minimap_mode"), 
			MapDisplayMode.WorldLeft => LOC.MM.GetStart("worldmap_l"), 
			MapDisplayMode.WorldCenter => LOC.MM.GetStart("worldmap_c"), 
			MapDisplayMode.WorldRight => LOC.MM.GetStart("worldmap_r"), 
			_ => throw new ArgumentOutOfRangeException("mode", mode, null), 
		};
	}
}
