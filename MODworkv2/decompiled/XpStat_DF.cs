using UnityEngine;
using UnityEngine.UI;

public class XpStat_DF : MonoBehaviour
{
	private Image content;

	[SerializeField]
	private float lerpSpeed;

	private float currentFill;

	private float currentValue;

	public float MaxValue { get; set; }

	public bool IsFull
	{
		get
		{
			if ((bool)content)
			{
				return Mathf.Approximately(content.fillAmount, 1f);
			}
			return false;
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
			if (MaxValue <= 0f)
			{
				currentValue = 0f;
				currentFill = 0f;
			}
			else
			{
				currentValue = Mathf.Clamp(value, 0f, MaxValue);
				currentFill = currentValue / MaxValue;
			}
		}
	}

	private void Awake()
	{
		content = GetComponent<Image>();
	}

	private void Update()
	{
		HandleBar();
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
