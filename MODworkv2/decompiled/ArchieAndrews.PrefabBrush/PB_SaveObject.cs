using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArchieAndrews.PrefabBrush;

[Serializable]
[CreateAssetMenu(fileName = "[NEW]PB_SaveFile", menuName = "PrefabBrush/Prefab Brush Save", order = 0)]
public class PB_SaveObject : ScriptableObject
{
	public List<GameObject> prefabList = new List<GameObject>();

	public List<PB_PrefabData> prefabData = new List<PB_PrefabData>();

	public PB_PaintType paintType;

	public float brushSize = 1f;

	public float minBrushSize = 0.1f;

	public float maxBrushSize = 30f;

	public float paintDeltaDistance = 0.4f;

	public float maxPaintDeltaDistance = 4f;

	public float minPaintDeltaDistance = 0.1f;

	public int prefabsPerStroke = 1;

	public int maxprefabsPerStroke = 30;

	public int minprefabsPerStroke = 1;

	public float spawnHeight = 10f;

	public bool addRigidbodyToPaintedPrefab = true;

	public float physicsIterations = 100f;

	public bool checkLayer;

	public bool checkTag;

	public bool checkSlope;

	public bool ignorePaintedPrefabs;

	public bool ignoreTriggers = true;

	public PB_Direction chainPivotAxis;

	public PB_Direction chainDirection;

	public int requiredTagMask;

	public int requiredLayerMask;

	public float minRequiredSlope;

	public float maxRequiredSlope;

	public Vector3 prefabOriginOffset;

	public Vector3 prefabRotationOffset;

	public PB_DragModType draggingAction;

	public PB_Direction rotationAxis;

	public float rotationSensitivity = 10f;

	public PB_ParentingStyle parentingStyle;

	public GameObject parent;

	public bool rotateToMatchSurface;

	public PB_Direction rotateSurfaceDirection;

	public bool randomizeRotation;

	public float minXRotation;

	public float maxXRotation;

	public float minYRotation;

	public float maxYRotation;

	public float minZRotation;

	public float maxZRotation;

	public PB_ScaleType scaleType;

	public PB_SaveApplicationType scaleApplicationType;

	public float minScale = 1f;

	public float maxScale = 1f;

	public float minXScale = 1f;

	public float maxXScale = 1f;

	public float minYScale = 1f;

	public float maxYScale = 1f;

	public float minZScale = 1f;

	public float maxZScale = 1f;

	public List<GameObject> parentList = new List<GameObject>();

	public float eraseBrushSize = 1f;

	public float minEraseBrushSize = 0.1f;

	public float maxEraseBrushSize = 20f;

	public PB_EraseTypes eraseType;

	public bool checkLayerForErase;

	public bool checkTagForErase;

	public bool checkSlopeForErase;

	public bool mustBeSelectedInBrush;

	public int requiredTagMaskForErase;

	public int requiredLayerMaskForErase;

	public float minRequiredSlopeForErase;

	public float maxRequiredSlopeForErase;

	public PB_EraseDetectionType eraseDetection;

	public KeyCode paintBrushHotKey = KeyCode.P;

	public bool paintBrushHoldKey = true;

	public KeyCode removeBrushHotKey = KeyCode.LeftControl;

	public bool removeBrushHoldKey = true;

	public KeyCode disableBrushHotKey = KeyCode.C;

	public bool disableBrushHoldKey;

	public bool hideToolsWarnings;

	public void AddPrefab(GameObject prefab)
	{
		PB_PrefabData pB_PrefabData = new PB_PrefabData();
		pB_PrefabData.prefab = prefab;
		pB_PrefabData.selected = true;
		prefabData.Add(pB_PrefabData);
	}

	public void UpgradeSave()
	{
		if (prefabList.Count != 0)
		{
			for (int i = 0; i < prefabList.Count; i++)
			{
				AddPrefab(prefabList[i]);
			}
			prefabList.Clear();
		}
	}

	public List<PB_PrefabData> GetActivePrefabs()
	{
		List<PB_PrefabData> list = new List<PB_PrefabData>();
		for (int i = 0; i < prefabData.Count; i++)
		{
			if (prefabData[i].selected)
			{
				list.Add(prefabData[i]);
			}
		}
		return list;
	}
}
