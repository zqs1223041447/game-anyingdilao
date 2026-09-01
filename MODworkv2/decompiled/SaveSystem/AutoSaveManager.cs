using System.Collections;
using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SaveSystem;

public class AutoSaveManager : SingletonMonoScope<AutoSaveManager>
{
	[Header("自动存档设置")]
	[Tooltip("是否启用自动存档")]
	[SerializeField]
	private bool enableAutoSave = true;

	[Tooltip("自动存档时间间隔（秒）")]
	[SerializeField]
	private float autoSaveInterval = 300f;

	public Image icon;

	[Header("自动存档图标动画")]
	[SerializeField]
	private float fadeTime = 0.7f;

	[SerializeField]
	private int playCount = 3;

	private float timer;

	private Coroutine iconCoroutine;

	public bool IsEnabled => enableAutoSave;

	public float ElapsedTime => timer;

	public float Interval => autoSaveInterval;

	public float RemainingTime => Mathf.Max(0f, autoSaveInterval - timer);

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	private void Start()
	{
		if (!icon && SingletonMonoScope<GameUIManager>.HasInstance)
		{
			icon = SingletonMonoScope<GameUIManager>.Instance.transform.Find("UICanvas/AutoSaveIcon").GetComponent<Image>();
		}
		if ((bool)icon)
		{
			Color color = icon.color;
			color.a = 0f;
			icon.color = color;
			icon.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (enableAutoSave && SaveManager.HasRuntime && !(SceneManager.GetActiveScene().name == "StartScene"))
		{
			timer += Time.unscaledDeltaTime;
			if (timer >= autoSaveInterval)
			{
				timer = 0f;
				TrySaveWithIcon();
			}
		}
	}

	public void TrySaveWithIcon()
	{
		if (SaveManager.RequestSave() && (bool)icon)
		{
			PlaySaveIconEffect();
		}
	}

	public void SetEnable(bool enable)
	{
		enableAutoSave = enable;
	}

	public void SetInterval(float seconds)
	{
		autoSaveInterval = Mathf.Max(5f, seconds);
	}

	public void ResetTimer()
	{
		timer = 0f;
	}

	private void PlaySaveIconEffect()
	{
		if (iconCoroutine != null)
		{
			StopCoroutine(iconCoroutine);
		}
		iconCoroutine = StartCoroutine(PlayIconFade());
	}

	private IEnumerator PlayIconFade()
	{
		if (!icon)
		{
			yield break;
		}
		icon.gameObject.SetActive(value: true);
		Color c = icon.color;
		for (int i = 0; i < playCount; i++)
		{
			float t3 = 0f;
			while (t3 < fadeTime)
			{
				t3 += Time.unscaledDeltaTime;
				float a = Mathf.Lerp(0f, 1f, t3 / fadeTime);
				c.a = a;
				icon.color = c;
				yield return null;
			}
			t3 = 0f;
			while (t3 < fadeTime)
			{
				t3 += Time.unscaledDeltaTime;
				float a2 = Mathf.Lerp(1f, 0f, t3 / fadeTime);
				c.a = a2;
				icon.color = c;
				yield return null;
			}
		}
		c.a = 0f;
		icon.color = c;
		icon.gameObject.SetActive(value: false);
		iconCoroutine = null;
	}
}
