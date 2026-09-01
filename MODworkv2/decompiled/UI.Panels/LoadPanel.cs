using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.UI.Base;
using Inputs.Cursors;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class LoadPanel : BasePanel, IPanelParam<AsyncOperation>
{
	[Header("Progress Setting")]
	[SerializeField]
	[Range(0f, 1f)]
	private float sceneLoadEndPercent = 0.4f;

	[SerializeField]
	[Range(0f, 0.3f)]
	private float sceneLoadRandomRange = 0.1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float sceneInitEndPercent = 0.8f;

	[SerializeField]
	[Range(0f, 0.3f)]
	private float sceneInitRandomRange = 0.1f;

	[SerializeField]
	private float finishSpeed = 2.5f;

	[SerializeField]
	private float finishHoldTime = 0.25f;

	private const float SmoothSpeed = 0.6f;

	private float sceneLoadEndTarget;

	private float sceneInitEndTarget;

	private AsyncOperation _op;

	private bool _running;

	private int _runId;

	private Image progressSlider;

	private Text progressText;

	private float displayProgress;

	protected override void Awake()
	{
		base.Awake();
		progressText = GetControl<Text>("loadingText");
		progressSlider = GetControl<Image>("loadingValue");
	}

	public void SetParam(AsyncOperation param)
	{
		_op = param;
	}

	public override void OnShow()
	{
		SetCursorHidden(hidden: true);
		_runId++;
		displayProgress = 0f;
		sceneLoadEndTarget = Random.Range(Mathf.Max(0f, sceneLoadEndPercent - sceneLoadRandomRange), Mathf.Min(1f, sceneLoadEndPercent + sceneLoadRandomRange));
		sceneInitEndTarget = Random.Range(Mathf.Max(sceneLoadEndTarget, sceneInitEndPercent - sceneInitRandomRange), Mathf.Min(1f, sceneInitEndPercent + sceneInitRandomRange));
		UpdateUI(0f);
		_running = true;
		RunProgress(_runId).Forget();
	}

	public override void OnHide()
	{
		_running = false;
		_op = null;
		SetCursorHidden(hidden: false);
	}

	private static void SetCursorHidden(bool hidden)
	{
		CursorManager.SetGlobalForceHidden(hidden);
	}

	private void LateUpdate()
	{
		if (_running && Cursor.visible)
		{
			CursorManager.EnforceGlobalForceHidden();
		}
	}

	private async UniTaskVoid RunProgress(int runId)
	{
		while (_running && runId == _runId && _op != null && !_op.isDone)
		{
			float target = Mathf.Clamp01(_op.progress / 0.9f) * sceneLoadEndTarget;
			displayProgress = Mathf.MoveTowards(displayProgress, target, Time.unscaledDeltaTime * 0.6f);
			UpdateUI(displayProgress);
			await UniTask.Yield();
		}
		if (_running && runId == _runId)
		{
			while (_running && displayProgress < sceneInitEndTarget)
			{
				displayProgress = Mathf.MoveTowards(displayProgress, sceneInitEndPercent, Time.unscaledDeltaTime * 0.6f);
				UpdateUI(displayProgress);
				await UniTask.Yield();
			}
		}
	}

	public async UniTask PlayFinishAndHold()
	{
		while (displayProgress < 1f)
		{
			displayProgress = Mathf.MoveTowards(displayProgress, 1f, Time.unscaledDeltaTime * finishSpeed);
			UpdateUI(displayProgress);
			await UniTask.Yield();
		}
		float hold = 0f;
		while (hold < finishHoldTime)
		{
			hold += Time.unscaledDeltaTime;
			UpdateUI(1f);
			await UniTask.Yield();
		}
	}

	private void UpdateUI(float value)
	{
		if ((bool)progressSlider)
		{
			progressSlider.fillAmount = value;
		}
		if ((bool)progressText)
		{
			progressText.text = $"{Mathf.RoundToInt(value * 100f)} %";
		}
	}
}
