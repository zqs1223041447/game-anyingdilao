using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightEXP : MonoBehaviour
{
	[HideInInspector]
	public Light2D lit;

	public AnimationCurve curve;

	[HideInInspector]
	public bool UseSkillTime;

	[HideInInspector]
	public bool LightDown;

	private float JStimeA;

	private float JStimeB;

	private void Awake()
	{
		lit = GetComponent<Light2D>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		lit.enabled = true;
		JStimeA = 0f;
		JStimeB = 0f;
		LightDown = false;
		lit.intensity = curve.Evaluate(JStimeA);
	}

	private void Update()
	{
		if (UseSkillTime)
		{
			if (LightDown)
			{
				JStimeA += Time.deltaTime;
				JStimeB += Time.deltaTime;
				if (JStimeB > 0.1f)
				{
					JStimeB = 0f;
					lit.intensity = curve.Evaluate(JStimeA);
				}
			}
		}
		else
		{
			JStimeA += Time.deltaTime;
			JStimeB += Time.deltaTime;
			if (JStimeB > 0.1f)
			{
				JStimeB = 0f;
				lit.intensity = curve.Evaluate(JStimeA);
			}
		}
	}
}
