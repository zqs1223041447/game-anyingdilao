using Display;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CustomHandler;

public class CanvasController : MonoBehaviour
{
	[Range(0f, 1f)]
	public float normalMatch;

	[Range(0f, 1f)]
	public float ultraWideMatch = 1f;

	private const float UltraWideThreshold = 2.2f;

	private CanvasScaler scaler;

	private float ratio;

	private void Awake()
	{
		scaler = GetComponent<CanvasScaler>();
		ApplyMatch();
	}

	private void ApplyMatch()
	{
		ratio = (float)Screen.width / (float)Screen.height;
		scaler.matchWidthOrHeight = ((ratio >= 2.2f) ? ultraWideMatch : normalMatch);
	}

	private void OnEnable()
	{
		Singleton<DisplayManager>.Instance.OnDisplayChanged += HandleDisplayChanged;
	}

	private void OnDisable()
	{
		Singleton<DisplayManager>.Instance.OnDisplayChanged -= HandleDisplayChanged;
	}

	private void HandleDisplayChanged(ResolutionInfo info, FullScreenMode mode)
	{
		ApplyMatch();
	}
}
