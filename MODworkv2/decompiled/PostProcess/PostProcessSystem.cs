using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcess;

public class PostProcessSystem : MonoBehaviour
{
	private Volume volume;

	private Bloom bloom;

	private Light2D globalLight;

	public void Init()
	{
		volume = GetComponent<Volume>();
		globalLight = GetComponent<Light2D>();
		volume.profile.TryGet<Bloom>(out bloom);
	}

	public void SetBloomEnabled(bool enable)
	{
		if ((bool)bloom)
		{
			bloom.active = enable;
		}
	}

	public void SetGlobalLightIntensity(float intensity)
	{
		if ((bool)globalLight)
		{
			globalLight.intensity = Mathf.Max(0f, intensity);
		}
	}

	public void SetGlobalLightColor(Color color)
	{
		if ((bool)globalLight)
		{
			globalLight.color = color;
		}
	}
}
