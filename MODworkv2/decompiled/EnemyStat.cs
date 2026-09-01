using UnityEngine;
using UnityEngine.UI;

public class EnemyStat : MonoBehaviour
{
	private Image content;

	private float maxValue;

	private float currentValue;

	private bool _initialized;

	public float MaxValue
	{
		get
		{
			return maxValue;
		}
		set
		{
			maxValue = value;
			ClampCurrentInternal();
			RefreshBar();
		}
	}

	public float CurrentValue
	{
		get
		{
			return currentValue;
		}
		set
		{
			currentValue = value;
			ClampCurrentInternal();
			RefreshBar();
		}
	}

	private void Awake()
	{
		content = GetComponent<Image>();
	}

	public void Initialize(float current, float max)
	{
		_initialized = true;
		MaxValue = max;
		CurrentValue = current;
		ClampCurrentInternal();
		RefreshBar();
	}

	public void SetCurrent(float value)
	{
		CurrentValue = value;
	}

	private void RefreshBar()
	{
		if (_initialized && (bool)content)
		{
			if (maxValue <= 0f)
			{
				content.fillAmount = 0f;
			}
			else
			{
				content.fillAmount = currentValue / maxValue;
			}
		}
	}

	private void ClampCurrentInternal()
	{
		if (maxValue <= 0f)
		{
			if (currentValue < 0f)
			{
				currentValue = 0f;
			}
		}
		else if (currentValue > maxValue)
		{
			currentValue = maxValue;
		}
		else if (currentValue < 0f)
		{
			currentValue = 0f;
		}
	}
}
