using System;
using UnityEngine;
using UnityEngine.UI;

namespace Entity.Misc;

public class CompStat : MonoBehaviour
{
	private Image content;

	private float currentFill;

	private float _lastValue;

	private bool _initialized;

	private bool _allowDeathCheck;

	public bool IsInitialized => _initialized;

	public float MaxValue { get; set; }

	public float CurrentValue { get; set; }

	public event Action OnInitialized;

	public event Action<float, float> OnValueChanged;

	public event Action OnZero;

	private void Awake()
	{
		content = GetComponent<Image>();
	}

	private void Update()
	{
		if (_initialized && !(MaxValue <= 0f))
		{
			UpdateUI();
		}
	}

	public void Initialize(float maxValue)
	{
		MaxValue = Mathf.Max(0f, maxValue);
		SetCurrentInternal(maxValue);
		_allowDeathCheck = true;
		if (!_initialized)
		{
			_initialized = true;
			this.OnInitialized?.Invoke();
		}
	}

	public void SetCurrent(float value)
	{
		SetCurrentInternal(value);
	}

	private void SetCurrentInternal(float value)
	{
		float currentValue = CurrentValue;
		CurrentValue = Mathf.Clamp(value, 0f, MaxValue);
		if (!Mathf.Approximately(currentValue, CurrentValue))
		{
			this.OnValueChanged?.Invoke(currentValue, CurrentValue);
		}
		if (_initialized && _allowDeathCheck && CurrentValue <= 0f && currentValue > 0f)
		{
			this.OnZero?.Invoke();
		}
		_lastValue = CurrentValue;
	}

	private void UpdateUI()
	{
		float num = CurrentValue / MaxValue;
		if (!Mathf.Approximately(content.fillAmount, num))
		{
			content.fillAmount = num;
		}
	}
}
