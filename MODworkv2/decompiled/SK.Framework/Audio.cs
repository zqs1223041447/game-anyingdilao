using UnityEngine;

namespace SK.Framework;

public class Audio : MonoBehaviour
{
	private static Audio instance;

	private BGMController bgm;

	private SFXController sfx;

	private AudioDatabaseController database;

	public static Audio Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("[SKFramework.Audio]").AddComponent<Audio>();
				instance.bgm = new BGMController();
				instance.sfx = new SFXController();
				instance.database = new AudioDatabaseController();
				Object.DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	public static BGMController BGM => Instance.bgm;

	public static SFXController SFX => Instance.sfx;

	public static AudioDatabaseController Database => Instance.database;

	private void Update()
	{
		sfx.Update();
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
