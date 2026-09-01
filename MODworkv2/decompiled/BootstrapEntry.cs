using UnityEngine;

public class BootstrapEntry : MonoBehaviour
{
	private static bool _booted;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void CreateBootstrap()
	{
		if (!_booted)
		{
			GameObject obj = new GameObject("[Bootstrap]");
			Object.DontDestroyOnLoad(obj);
			obj.AddComponent<BootstrapEntry>();
			_booted = true;
		}
	}

	private void Awake()
	{
		if (!_booted)
		{
			_booted = true;
			GameBootstrap.Boot();
		}
	}
}
