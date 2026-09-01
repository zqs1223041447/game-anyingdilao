using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class FadePanel : MonoBehaviour
{
	[Header("引用")]
	public Image Icon;

	public Image fadeImage;

	public Text Text;

	public CanvasGroup CanvasGroup;

	[Header("旋转设置")]
	[SerializeField]
	private float rotateSpeed = 180f;

	[Header("呼吸缩放设置")]
	[SerializeField]
	private float scaleAmplitude = 0.12f;

	[SerializeField]
	private float scaleFrequency = 2f;

	[Header("Tip 刷新设置")]
	[SerializeField]
	private float tipRefreshInterval = 5f;

	[SerializeField]
	private int tipCount = 3;

	private float _scaleTimer;

	private bool _isShowing;

	private CancellationTokenSource _playCts;

	private void Update()
	{
		if (!Icon || !fadeImage || !CanvasGroup || !Text)
		{
			return;
		}
		if (CanvasGroup.alpha > 0.01f)
		{
			if (!_isShowing)
			{
				_isShowing = true;
				StartPlayLoop();
			}
		}
		else if (_isShowing)
		{
			_isShowing = false;
			StopPlayLoop();
		}
	}

	private void OnDestroy()
	{
		StopPlayLoop();
	}

	private void StartPlayLoop()
	{
		StopPlayLoop();
		_playCts = new CancellationTokenSource();
		RefreshTips();
		PlayAnimLoop(_playCts.Token).Forget();
		PlayTipLoop(_playCts.Token).Forget();
	}

	private void StopPlayLoop()
	{
		if (_playCts != null)
		{
			_playCts.Cancel();
			_playCts.Dispose();
			_playCts = null;
		}
		if ((bool)Icon)
		{
			Icon.transform.localRotation = Quaternion.identity;
			Icon.transform.localScale = Vector3.one;
		}
		_scaleTimer = 0f;
	}

	private async UniTaskVoid PlayAnimLoop(CancellationToken token)
	{
		while (!token.IsCancellationRequested && (bool)Icon)
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			Icon.transform.Rotate(0f, 0f, (0f - rotateSpeed) * unscaledDeltaTime);
			_scaleTimer += unscaledDeltaTime * scaleFrequency;
			float num = Mathf.Sin(_scaleTimer) * scaleAmplitude;
			float num2 = 1f + num;
			Icon.transform.localScale = new Vector3(num2, num2, 1f);
			await UniTask.Yield(PlayerLoopTiming.Update, token);
		}
	}

	private async UniTaskVoid PlayTipLoop(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			await UniTask.Delay(Mathf.Max(1, Mathf.RoundToInt(tipRefreshInterval * 1000f)), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, token);
			if (!token.IsCancellationRequested)
			{
				RefreshTips();
				continue;
			}
			break;
		}
	}

	private void RefreshTips()
	{
		if ((bool)Text && tipCount > 0)
		{
			int num = Random.Range(1, tipCount + 1);
			Text.text = LOC.MM.GetStart("tip" + num);
		}
	}
}
