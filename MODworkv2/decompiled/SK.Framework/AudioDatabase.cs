using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SK.Framework;

[CreateAssetMenu(fileName = "New Audio Database", order = 215)]
public class AudioDatabase : ScriptableObject
{
	public string databaseName;

	public AudioMixerGroup outputAudioMixerGroup;

	public List<AudioData> datasets = new List<AudioData>(0);

	public AudioData this[int index] => datasets[index];

	public AudioData this[string dataName] => datasets.Find((AudioData m) => m.name == dataName);

	public AudioClip GetClip(string dataName)
	{
		return datasets.Find((AudioData m) => m.name == dataName)?.clip;
	}

	public void PlayAsBGM(string dataName)
	{
		Audio.BGM.Output = outputAudioMixerGroup;
		Audio.BGM.Play(GetClip(dataName));
	}

	public AudioHandler PlayAsSFX(string dataName)
	{
		AudioClip clip = GetClip(dataName);
		if (clip != null)
		{
			return Audio.SFX.Play(clip, outputAudioMixerGroup);
		}
		return null;
	}

	public AudioHandler PlayAsSFX(string dataName, Vector3 position)
	{
		AudioClip clip = GetClip(dataName);
		if (clip != null)
		{
			return Audio.SFX.Play(clip, position, outputAudioMixerGroup);
		}
		return null;
	}

	public AudioHandler PlayAsSFX(string dataName, Transform followTarget)
	{
		AudioClip clip = GetClip(dataName);
		if (clip != null)
		{
			return Audio.SFX.Play(clip, followTarget, outputAudioMixerGroup);
		}
		return null;
	}
}
