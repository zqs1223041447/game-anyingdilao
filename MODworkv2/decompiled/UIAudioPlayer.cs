using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioPlayer : MonoBehaviour
{
	[Header("音频索引")]
	public int audioId;

	[Tooltip("滑动条数值变化超过该阈值才播放音效（防止疯狂触发）")]
	public float valueThreshold = 0.05f;

	private float _lastValue;

	private void Awake()
	{
		BindButton();
		BindToggle();
		BindSlider();
		BindCustomSlider();
	}

	private void BindButton()
	{
		Button component = GetComponent<Button>();
		if ((bool)component)
		{
			component.onClick.AddListener(delegate
			{
				SingletonMonoGlobal<AudioManager>.Instance.SceneStartUI(audioId);
			});
		}
	}

	private void BindToggle()
	{
		Toggle component = GetComponent<Toggle>();
		if ((bool)component)
		{
			component.onValueChanged.AddListener(delegate
			{
				SingletonMonoGlobal<AudioManager>.Instance.SceneStartUI(audioId);
			});
		}
	}

	private void BindSlider()
	{
		Slider component = GetComponent<Slider>();
		if (!component)
		{
			return;
		}
		_lastValue = component.value;
		component.onValueChanged.AddListener(delegate(float v)
		{
			if (Mathf.Abs(v - _lastValue) >= valueThreshold)
			{
				_lastValue = v;
				SingletonMonoGlobal<AudioManager>.Instance.SceneStartUI(audioId);
			}
		});
	}

	private void BindCustomSlider()
	{
		CustomScrollView component = GetComponent<CustomScrollView>();
		if (!component)
		{
			return;
		}
		_lastValue = component.value;
		component.onValueChanged.AddListener(delegate(float v)
		{
			if (Mathf.Abs(v - _lastValue) >= valueThreshold)
			{
				_lastValue = v;
				SingletonMonoGlobal<AudioManager>.Instance.SceneStartUI(audioId);
			}
		});
	}
}
