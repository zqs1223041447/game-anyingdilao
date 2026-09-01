using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{
	private static readonly int number = Shader.PropertyToID("number");

	private Image render;

	[SerializeField]
	private float lerpSpeed;

	private float currentFill;

	public float Cur;

	public float Max { get; set; }

	private void Start()
	{
		render = GetComponent<Image>();
	}

	private void Update()
	{
		currentFill = Cur / Max;
		if (Cur > Max)
		{
			Cur = Max;
		}
		if (Cur < 0f)
		{
			Cur = 0f;
		}
		HandleBar();
	}

	public void Initialize(float currentValue, float maxValue)
	{
		Max = maxValue;
		Cur = currentValue;
	}

	private void HandleBar()
	{
		if (!Mathf.Approximately(currentFill, render.material.GetFloat(number)))
		{
			render.material.SetFloat(number, Cur / Max);
		}
	}
}
