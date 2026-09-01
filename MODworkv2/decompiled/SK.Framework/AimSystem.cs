using UnityEngine;

namespace SK.Framework;

public class AimSystem : MonoBehaviour
{
	private static AimSystem instance;

	[SerializeField]
	private bool toggle = true;

	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private LayerMask aimLayer;

	[SerializeField]
	private float aimMaxDistance = 10f;

	[SerializeField]
	private AimMode aimMode;

	public static AimSystem Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<AimSystem>() ?? new GameObject("[SKFramework.Aim]").AddComponent<AimSystem>();
			}
			return instance;
		}
	}

	public bool Toggle
	{
		get
		{
			return toggle;
		}
		set
		{
			if (toggle != value)
			{
				toggle = value;
				if (CurrentAimableObject != null)
				{
					CurrentAimableObject.Exit();
					CurrentAimableObject = null;
				}
			}
		}
	}

	public IAimableObject CurrentAimableObject { get; private set; }

	private void Start()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main ?? Object.FindObjectOfType<Camera>();
		}
		aimMaxDistance = ((aimMode == AimMode.Mouse) ? float.MaxValue : aimMaxDistance);
	}

	private void Update()
	{
		if (!toggle)
		{
			return;
		}
		if (Physics.Raycast((aimMode == AimMode.Mouse) ? mainCamera.ScreenPointToRay(Input.mousePosition) : mainCamera.ViewportPointToRay(Vector2.one * 0.5f), out var hitInfo, aimMaxDistance, aimLayer))
		{
			IAimableObject component = hitInfo.collider.GetComponent<IAimableObject>();
			if (component != CurrentAimableObject)
			{
				CurrentAimableObject?.Exit();
				CurrentAimableObject = component;
				CurrentAimableObject?.Enter();
			}
		}
		else if (CurrentAimableObject != null)
		{
			CurrentAimableObject.Exit();
			CurrentAimableObject = null;
		}
		CurrentAimableObject?.Stay();
	}

	private void OnDestroy()
	{
		if (CurrentAimableObject != null)
		{
			CurrentAimableObject.Exit();
			CurrentAimableObject = null;
		}
		instance = null;
	}
}
