using System.Collections;
using Core.Teleport.PlayerSpawn;
using Cysharp.Threading.Tasks;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI.Base;
using Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class DeadPanel : BasePanel
{
	public Text deadText;

	public Image bg;

	public CanvasGroup canvasGroup;

	[Header("面板渐入速度")]
	[SerializeField]
	private float panelFadeSpeed = 2.5f;

	[Header("变红速度")]
	[SerializeField]
	private float fadeSpeed = 0.3f;

	[Header("红色最高透明度")]
	[SerializeField]
	private float redAlpha = 0.4f;

	[Header("倒计时回城")]
	[SerializeField]
	private float time = 3f;

	private Coroutine _routine;

	protected override void Awake()
	{
		base.Awake();
		if (!deadText)
		{
			deadText = base.transform.Find("Content/LittleImage/deadText").GetComponent<Text>();
		}
		if (!bg)
		{
			bg = base.transform.Find("Content/Bg").GetComponent<Image>();
		}
		if (!canvasGroup)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		ResetVisual();
		_routine = StartCoroutine(PlayDeadSequence());
	}

	public override void OnHide()
	{
		base.OnHide();
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
	}

	private void ResetVisual()
	{
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.interactable = true;
		}
		if ((bool)bg)
		{
			Color color = bg.color;
			color.a = 0f;
			bg.color = color;
		}
		SetCountdownText(Mathf.CeilToInt(time));
	}

	private IEnumerator PlayDeadSequence()
	{
		if ((bool)canvasGroup)
		{
			float a = 0f;
			while (a < 1f)
			{
				a += Time.deltaTime * panelFadeSpeed;
				canvasGroup.alpha = Mathf.Clamp01(a);
				yield return null;
			}
			canvasGroup.alpha = 1f;
		}
		float targetAlpha = Mathf.Clamp01(redAlpha);
		float curAlpha = 0f;
		int remain = Mathf.CeilToInt(time);
		float nextTick = 1f;
		while (curAlpha < targetAlpha || remain > 0)
		{
			if (curAlpha < targetAlpha && (bool)bg)
			{
				curAlpha = Mathf.MoveTowards(curAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
				Color color = bg.color;
				color.a = curAlpha;
				bg.color = color;
			}
			if (remain > 0)
			{
				nextTick -= Time.deltaTime;
				if (nextTick <= 0f)
				{
					remain--;
					SetCountdownText(Mathf.Max(0, remain));
					nextTick += 1f;
				}
			}
			yield return null;
		}
		SingletonMonoGlobal<PlayerSpawnManager>.Instance.SetHomeRequest(new HomePlayerSpawnRequest
		{
			Reason = HomePlayerSpawnReason.HomeDefault
		});
		SceneLoadManager.LoadHomeScene(SceneTransitionMode.Fade).Forget();
	}

	private void SetCountdownText(int seconds)
	{
		if ((bool)deadText)
		{
			string text = $"<b><color=#FF6B6B>{seconds}s</color></b>";
			deadText.supportRichText = true;
			string main = LOC.MM.GetMain("dead_time");
			deadText.text = main + " " + text;
		}
	}
}
