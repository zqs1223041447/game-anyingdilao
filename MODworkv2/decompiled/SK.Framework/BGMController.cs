using UnityEngine;
using UnityEngine.Audio;

namespace SK.Framework;

public class BGMController
{
	private readonly AudioSource source;

	private bool isPaused;

	public float Volume
	{
		get
		{
			return source.volume;
		}
		set
		{
			if (source.volume != value)
			{
				source.volume = value;
				Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 背景音乐音量调整为 {0}", source.volume);
			}
		}
	}

	public int Priority
	{
		get
		{
			return source.priority;
		}
		set
		{
			if (source.priority != value)
			{
				source.priority = value;
				Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 背景音乐优先级调整为 {0}", source.priority);
			}
		}
	}

	public float Pitch
	{
		get
		{
			return source.pitch;
		}
		set
		{
			if (source.pitch != value)
			{
				source.pitch = value;
				Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 背景音乐音高调整为 {0}", source.pitch);
			}
		}
	}

	public bool IsMuted
	{
		get
		{
			return source.mute;
		}
		set
		{
			if (source.mute != value)
			{
				source.mute = value;
				Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 背景音乐{0}静音", value ? "设置" : "取消");
			}
		}
	}

	public bool IsPaused
	{
		get
		{
			return isPaused;
		}
		set
		{
			if (isPaused != value)
			{
				isPaused = value;
				if (isPaused)
				{
					source.Pause();
					Log.Info((object)"<color=cyan><b>[SKFramework.Audio.Info]</b></color> 暂停背景音乐");
				}
				else
				{
					source.UnPause();
					Log.Info((object)"<color=cyan><b>[SKFramework.Audio.Info]</b></color> 恢复背景音乐");
				}
			}
		}
	}

	public bool IsPlaying => source.isPlaying;

	public bool IsLoop
	{
		get
		{
			return source.loop;
		}
		set
		{
			if (source.loop != value)
			{
				source.loop = value;
				Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> {0}背景音乐循环", source.loop ? "设置" : "取消");
			}
		}
	}

	public float Progress
	{
		get
		{
			if (!source.isPlaying)
			{
				return 0f;
			}
			return source.time / source.clip.length;
		}
	}

	public float Time => source.time;

	public AudioClip Clip => source.clip;

	public AudioMixerGroup Output
	{
		get
		{
			return source.outputAudioMixerGroup;
		}
		set
		{
			source.outputAudioMixerGroup = value;
		}
	}

	public BGMController()
	{
		source = new GameObject("[BGM]").AddComponent<AudioSource>();
		source.loop = true;
		source.transform.SetParent(Audio.Instance.transform);
	}

	public void Play(AudioClip bgm)
	{
		if (source.isPlaying)
		{
			source.Stop();
		}
		source.clip = bgm;
		source.Play();
		isPaused = false;
		Log.Info("<color=cyan><b>[SKFramework.Audio.Info]</b></color> 播放背景音乐 {0}", (bgm != null) ? bgm.name : "--");
	}

	public void Stop()
	{
		source.Stop();
		Log.Info((object)"<color=cyan><b>[SKFramework.Audio.Info]</b></color> 终止背景音乐");
	}
}
