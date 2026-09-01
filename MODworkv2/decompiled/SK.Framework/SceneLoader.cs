using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SK.Framework;

public class SceneLoader : MonoBehaviour
{
	private GetSceneMode getSceneMode;

	private LoadSceneMode loadSceneMode;

	private string sceneName;

	private int sceneBuildIndex;

	private float sceneActivationDelay = 3f;

	private Action onBegan;

	private Action<float> onLoading;

	private Action onCompleted;

	public float Progress { get; private set; }

	private IEnumerator LoadCoroutine()
	{
		yield return null;
		onBegan?.Invoke();
		yield return null;
		AsyncOperation asyncOperation = null;
		switch (getSceneMode)
		{
		case GetSceneMode.Name:
			asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
			break;
		case GetSceneMode.BuildIndex:
			asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, loadSceneMode);
			break;
		}
		asyncOperation.allowSceneActivation = false;
		while (asyncOperation.progress < 0.9f)
		{
			Progress = Mathf.Clamp01(asyncOperation.progress / 0.9f) * 0.2f;
			onLoading?.Invoke(Progress);
			Log.Info("<color=cyan><b>[SKFramework.Scene.Info]</b></color> 场景加载进度[{0}]", Progress);
			yield return null;
		}
		float delayBeginTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - delayBeginTime < sceneActivationDelay)
		{
			float value = (Time.realtimeSinceStartup - delayBeginTime) / sceneActivationDelay;
			Progress = Mathf.Clamp01(value) * 0.8f + 0.2f;
			onLoading?.Invoke(Progress);
			Log.Info("<color=cyan><b>[SKFramework.Scene.Info]</b></color> 场景加载进度[{0}]", Progress);
			yield return null;
		}
		asyncOperation.allowSceneActivation = true;
		while (!asyncOperation.isDone)
		{
			yield return null;
		}
		Log.Info((object)"<color=cyan><b>[SKFramework.Scene.Info]</b></color> 场景加载完成");
		onCompleted?.Invoke();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public SceneLoader LoadAsync()
	{
		StartCoroutine(LoadCoroutine());
		return this;
	}

	public SceneLoader LoadAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		this.sceneName = sceneName;
		getSceneMode = GetSceneMode.Name;
		this.loadSceneMode = loadSceneMode;
		StartCoroutine(LoadCoroutine());
		return this;
	}

	public SceneLoader LoadAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		this.sceneBuildIndex = sceneBuildIndex;
		getSceneMode = GetSceneMode.BuildIndex;
		this.loadSceneMode = loadSceneMode;
		StartCoroutine(LoadCoroutine());
		return this;
	}

	public SceneLoader SetSceneActivationDelay(float sceneActivationDelay)
	{
		this.sceneActivationDelay = sceneActivationDelay;
		return this;
	}

	public SceneLoader OnBegan(Action onBegan)
	{
		this.onBegan = onBegan;
		return this;
	}

	public SceneLoader OnLoading(Action<float> onLoading)
	{
		this.onLoading = onLoading;
		return this;
	}

	public SceneLoader OnCompleted(Action onCompleted)
	{
		this.onCompleted = onCompleted;
		return this;
	}

	public static SceneLoader LoadAsync(string sceneName, float sceneActivationDelay = 3f, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		SceneLoader sceneLoader = new GameObject("[SceneLoader." + sceneName + "]").AddComponent<SceneLoader>();
		UnityEngine.Object.DontDestroyOnLoad(sceneLoader);
		sceneLoader.SetSceneActivationDelay(sceneActivationDelay);
		sceneLoader.LoadAsync(sceneName, loadSceneMode);
		return sceneLoader;
	}

	public static SceneLoader LoadAsync(int sceneBuildIndex, float sceneActivationDelay = 3f, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		SceneLoader sceneLoader = new GameObject($"[SceneLoader.{sceneBuildIndex}]").AddComponent<SceneLoader>();
		UnityEngine.Object.DontDestroyOnLoad(sceneLoader);
		sceneLoader.SetSceneActivationDelay(sceneActivationDelay);
		sceneLoader.LoadAsync(sceneBuildIndex, loadSceneMode);
		return sceneLoader;
	}
}
