using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class XpStat : MonoBehaviour
{
	private Image content;

	[SerializeField]
	private float lerpSpeed;

	private float currentFill;

	private float currentValue;

	private bool forceFullAtMaxPlayerLevel = true;

	public float MaxValue { get; set; }

	public bool IsFull => Mathf.Approximately(content.fillAmount, 1f);

	public float CurrentValue
	{
		get
		{
			return currentValue;
		}
		set
		{
			if (MaxValue <= 0f)
			{
				currentValue = 0f;
				currentFill = 0f;
				return;
			}
			if (value > MaxValue)
			{
				currentValue = MaxValue;
			}
			else if (value < 0f)
			{
				currentValue = 0f;
			}
			else
			{
				currentValue = value;
			}
			currentFill = currentValue / MaxValue;
		}
	}

	private void Awake()
	{
		content = GetComponent<Image>();
	}

	private void Update()
	{
		if (!forceFullAtMaxPlayerLevel || SingletonMonoScope<PlayerManager>.Instance.Level < 100)
		{
			HandleBar();
		}
		else
		{
			content.fillAmount = 0f;
		}
	}

	public void SetForceFullAtMaxPlayerLevel(bool value)
	{
		forceFullAtMaxPlayerLevel = value;
	}

	public void Initialize(float cValue, float maxValue)
	{
		MaxValue = Mathf.Max(1f, maxValue);
		CurrentValue = cValue;
		if ((bool)content)
		{
			content.fillAmount = currentFill;
		}
	}

	private void HandleBar()
	{
		if ((bool)content && !Mathf.Approximately(currentFill, content.fillAmount))
		{
			content.fillAmount = Mathf.MoveTowards(content.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
		}
	}

	public void Reset()
	{
		if ((bool)content)
		{
			content.fillAmount = 0f;
		}
	}
}
