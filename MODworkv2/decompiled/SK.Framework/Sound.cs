using System;
using UnityEngine;

namespace SK.Framework;

[Serializable]
public class Sound
{
	public SoundSource source;

	public AudioClip audioClip;

	public string databaseName;

	public string audioDataName;

	public AudioClip GetAudioClip()
	{
		switch (source)
		{
		case SoundSource.AudioClip:
			return audioClip;
		case SoundSource.Datebase:
		{
			AudioDatabase database = Audio.Database.Get(databaseName);
			if (database == null)
			{
				Audio.Database.Load(databaseName, out database);
			}
			return database.GetClip(audioDataName);
		}
		default:
			return null;
		}
	}

	public void Play()
	{
		switch (source)
		{
		case SoundSource.AudioClip:
			Audio.SFX.Play(audioClip);
			break;
		case SoundSource.Datebase:
		{
			AudioDatabase database = Audio.Database.Get(databaseName);
			if (database == null)
			{
				Audio.Database.Load(databaseName, out database);
			}
			if (database != null)
			{
				AudioData audioData = database[audioDataName];
				if (audioData != null)
				{
					Audio.SFX.Play(audioData.clip, database.outputAudioMixerGroup);
				}
			}
			break;
		}
		}
	}
}
