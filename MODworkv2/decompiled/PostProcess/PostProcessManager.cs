using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace PostProcess;

public class PostProcessManager : SingletonMonoGlobal<PostProcessManager>
{
	private PostProcessSystem view;

	public void Init()
	{
		if (!(view != null))
		{
			GameObject gameObject = Object.Instantiate(Singleton<ResManager>.Instance.Load<GameObject>("res://PostProcess/PostProcessSystem"));
			gameObject.name = "[PostProcessSystem]";
			Object.DontDestroyOnLoad(gameObject);
			view = gameObject.GetComponent<PostProcessSystem>();
			view.Init();
		}
	}

	public void SetBloomEnabled(bool enable)
	{
		view?.SetBloomEnabled(enable);
	}

	public void SetGlobalLightIntensity(float intensity)
	{
		view?.SetGlobalLightIntensity(intensity);
	}

	public void SetGlobalLightColor(Color color)
	{
		view?.SetGlobalLightColor(color);
	}
}
