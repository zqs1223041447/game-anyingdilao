using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class AudioManager : SingletonMonoGlobal<AudioManager>
{
	internal class AudioChannel
	{
		internal enum NextSelectionMode
		{
			Sequential,
			RandomAny,
			RandomExceptCurrent,
			RandomExceptFirst
		}

		[Header("BGM")]
		[Tooltip("距离曲目结束多少秒开始自然淡出")]
		public const float AutoFadeOutSeconds = 2f;

		[Tooltip("淡入淡出时长")]
		public float CrossFadeDuration = 2f;

		private EventInstance _current;

		private EventInstance _next;

		private string _pendingNext;

		private bool _autoFadeTriggered;

		private string[] _playlist;

		private int _index = -1;

		private NextSelectionMode _nextSelectionMode;

		private string _playlistSignature;

		private bool _loopForever;

		private bool _externallyPaused;

		public EventInstance Current => _current;

		public void Update()
		{
			if (!_externallyPaused)
			{
				UpdateAutoFade();
				CheckStoppedLoop();
				ForcePlaylistAdvance();
			}
		}

		public void SetExternallyPaused(bool paused)
		{
			_externallyPaused = paused;
			if (_current.isValid())
			{
				_current.setPaused(paused);
			}
			if (_next.isValid())
			{
				_next.setPaused(paused);
			}
		}

		public bool IsSamePlaylist(string[] playlist)
		{
			if (playlist == null || playlist.Length == 0)
			{
				return false;
			}
			string text = BuildSignature(playlist);
			return _playlistSignature == text;
		}

		public string[] GetPlaylistCopy()
		{
			if (_playlist == null || _playlist.Length == 0)
			{
				return null;
			}
			string[] array = new string[_playlist.Length];
			Array.Copy(_playlist, array, _playlist.Length);
			return array;
		}

		public string GetPlaylistSignature()
		{
			return _playlistSignature;
		}

		private void ForcePlaylistAdvance()
		{
			if (_playlist != null && _playlist.Length != 0 && !_autoFadeTriggered && _current.isValid())
			{
				_current.getPlaybackState(out var state);
				if (state == PLAYBACK_STATE.STOPPING || state == PLAYBACK_STATE.STOPPED)
				{
					_autoFadeTriggered = true;
					_pendingNext = GetNext();
				}
			}
		}

		private void CheckStoppedLoop()
		{
			if (!_loopForever || !_current.isValid() || (_playlist != null && _playlist.Length != 0))
			{
				return;
			}
			_current.getPlaybackState(out var state);
			if (state == PLAYBACK_STATE.STOPPED)
			{
				if (_playlist != null && _playlist.Length != 0)
				{
					_pendingNext = GetNext();
				}
				else
				{
					_current.start();
				}
			}
		}

		public void PlayPlaylist(string[] playlist, bool random)
		{
			PlayPlaylist(playlist, -1, random ? NextSelectionMode.RandomAny : NextSelectionMode.Sequential);
		}

		public void PlayPlaylist(string[] playlist, int startIndex, NextSelectionMode nextSelectionMode)
		{
			PlayPlaylist(playlist, startIndex, nextSelectionMode, randomFirstTrack: false);
		}

		public void PlayPlaylist(string[] playlist, int startIndex, NextSelectionMode nextSelectionMode, bool randomFirstTrack)
		{
			if (playlist == null || playlist.Length == 0)
			{
				return;
			}
			_loopForever = true;
			string text = BuildSignature(playlist);
			if (_current.isValid() && _playlistSignature == text && _nextSelectionMode == nextSelectionMode)
			{
				return;
			}
			_playlistSignature = text;
			_playlist = playlist;
			_nextSelectionMode = nextSelectionMode;
			_autoFadeTriggered = false;
			_index = -1;
			string text2;
			if (startIndex >= 0 && startIndex < _playlist.Length)
			{
				_index = startIndex;
				text2 = _playlist[_index];
			}
			else if (randomFirstTrack)
			{
				_index = UnityEngine.Random.Range(0, _playlist.Length);
				text2 = _playlist[_index];
			}
			else
			{
				text2 = GetNext();
			}
			if (_current.isValid())
			{
				_pendingNext = text2;
				return;
			}
			_current = RuntimeManager.CreateInstance(text2);
			if (_externallyPaused)
			{
				_current.setPaused(paused: true);
			}
			_current.start();
		}

		public void PlayOneShotReplace(string path, bool loopForever = true)
		{
			_loopForever = loopForever;
			Stop();
			_current = RuntimeManager.CreateInstance(path);
			if (_externallyPaused)
			{
				_current.setPaused(paused: true);
			}
			_current.start();
		}

		public void LockAutoFade(bool locked)
		{
			_autoFadeTriggered = locked;
		}

		private void UpdateAutoFade()
		{
			if (_current.isValid() && !_autoFadeTriggered && _playlist != null && _playlist.Length != 0)
			{
				_current.getTimelinePosition(out var position);
				_current.getDescription(out var description);
				description.getLength(out var length);
				if ((float)(length - position) / 1000f <= 2f)
				{
					_autoFadeTriggered = true;
					_pendingNext = GetNext();
				}
			}
		}

		public bool TryConsumePendingNext(out string next)
		{
			if (string.IsNullOrEmpty(_pendingNext))
			{
				next = null;
				return false;
			}
			next = _pendingNext;
			_pendingNext = null;
			return true;
		}

		public IEnumerator CrossFadeRoutine(string next)
		{
			StopAndRelease(ref _next);
			_next = RuntimeManager.CreateInstance(next);
			_next.setVolume(0f);
			if (_externallyPaused)
			{
				_next.setPaused(paused: true);
			}
			_next.start();
			float duration = Mathf.Max(0.01f, CrossFadeDuration);
			float time = 0f;
			while (time < duration)
			{
				while (_externallyPaused)
				{
					yield return null;
				}
				time += Time.unscaledDeltaTime;
				float num = Mathf.Clamp01(time / duration);
				if (_current.isValid())
				{
					_current.setVolume(1f - num);
				}
				_next.setVolume(num);
				yield return null;
			}
			StopAndRelease(ref _current);
			_current = _next;
			_next.clearHandle();
			_autoFadeTriggered = false;
		}

		private string GetNext()
		{
			if (_playlist == null || _playlist.Length == 0)
			{
				return null;
			}
			switch (_nextSelectionMode)
			{
			case NextSelectionMode.RandomAny:
				_index = UnityEngine.Random.Range(0, _playlist.Length);
				break;
			case NextSelectionMode.RandomExceptCurrent:
				_index = GetRandomIndexExcept(_index);
				break;
			case NextSelectionMode.RandomExceptFirst:
				_index = GetRandomIndexExcept(0);
				break;
			default:
				_index = (_index + 1) % _playlist.Length;
				break;
			}
			return _playlist[_index];
		}

		private int GetRandomIndexExcept(int excludedIndex)
		{
			if (_playlist == null || _playlist.Length <= 1)
			{
				return 0;
			}
			if (excludedIndex < 0 || excludedIndex >= _playlist.Length)
			{
				return UnityEngine.Random.Range(0, _playlist.Length);
			}
			int num = UnityEngine.Random.Range(0, _playlist.Length - 1);
			if (num >= excludedIndex)
			{
				num++;
			}
			return num;
		}

		private static string BuildSignature(string[] p)
		{
			return string.Join("|", p);
		}

		public void Stop()
		{
			StopAndRelease(ref _current);
			StopAndRelease(ref _next);
		}

		public void ResetState()
		{
			_playlist = null;
			_playlistSignature = null;
			_index = -1;
			_nextSelectionMode = NextSelectionMode.Sequential;
			_autoFadeTriggered = false;
			_loopForever = false;
			_pendingNext = null;
		}

		public void SoftReset()
		{
			_autoFadeTriggered = false;
			_pendingNext = null;
			_playlist = null;
			_playlistSignature = null;
			_index = -1;
			_nextSelectionMode = NextSelectionMode.Sequential;
			_loopForever = false;
		}
	}

	[Header("Data")]
	[HideInInspector]
	public AudioData audioData;

	[HideInInspector]
	public MusicData musicData;

	private AudioChannel _bgm;

	private AudioChannel _atmos;

	private Coroutine _bgmFadeCoroutine;

	private Coroutine _atmosFadeCoroutine;

	private bool _inited;

	private Coroutine _introToLoopCoroutine;

	private int _bgmRequestId;

	private bool _applicationPaused;

	private bool _applicationPauseMasterBusStateCaptured;

	private float _masterBusVolumeBeforeApplicationPause = 1f;

	public void Init()
	{
		if (!_inited)
		{
			audioData = Singleton<ResManager>.Instance.Load<AudioData>("res://Audio/AudioData");
			musicData = Singleton<ResManager>.Instance.Load<MusicData>("res://Audio/MusicData");
			_bgm = new AudioChannel();
			_atmos = new AudioChannel();
			_bgm.CrossFadeDuration = 2f;
			_atmos.CrossFadeDuration = 3.5f;
			_inited = true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Init();
	}

	private void Update()
	{
		_bgm.Update();
		_atmos.Update();
		HandleChannelFade(_bgm, ref _bgmFadeCoroutine);
		HandleChannelFade(_atmos, ref _atmosFadeCoroutine);
	}

	private void HandleChannelFade(AudioChannel channel, ref Coroutine routine)
	{
		if (channel.TryConsumePendingNext(out var next))
		{
			if (routine != null)
			{
				StopCoroutine(routine);
			}
			routine = StartCoroutine(channel.CrossFadeRoutine(next));
		}
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		_bgm?.Stop();
		_atmos?.Stop();
		base.OnDestroy();
	}

	private static void StopAndRelease(ref EventInstance inst)
	{
		if (inst.isValid())
		{
			inst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			inst.release();
			inst.clearHandle();
		}
	}

	public string[] GetCurrentBGMPlaylist()
	{
		return _bgm.GetPlaylistCopy();
	}

	public string GetCurrentBGMPlaylistSignature()
	{
		return _bgm.GetPlaylistSignature();
	}

	public bool IsCurrentBGMPlaylist(string[] playlist)
	{
		return _bgm.IsSamePlaylist(playlist);
	}

	public void PlayBGM(string[] playlist, bool random)
	{
		PrepareBGMRequest();
		_bgm.PlayPlaylist(playlist, random);
	}

	public void PlayHomeStartBGM(bool startFromFirstTrack)
	{
		PrepareBGMRequest();
		_bgm.PlayPlaylist(musicData.HomeStart, (!startFromFirstTrack) ? (-1) : 0, AudioChannel.NextSelectionMode.RandomExceptCurrent);
	}

	public void PlayHomeVictoryBGM()
	{
		PrepareBGMRequest();
		_bgm.PlayPlaylist(musicData.HomeVictory, -1, AudioChannel.NextSelectionMode.RandomExceptFirst, randomFirstTrack: true);
	}

	private void PrepareBGMRequest()
	{
		if (_introToLoopCoroutine != null)
		{
			StopCoroutine(_introToLoopCoroutine);
			_introToLoopCoroutine = null;
		}
		_bgmRequestId++;
		_bgm.LockAutoFade(locked: false);
	}

	public void ExitIntroLoopMode()
	{
		_bgm.LockAutoFade(locked: false);
		_bgm.SoftReset();
	}

	public void PlayIntroThenLoop(string introBGM, string loopBGM)
	{
		if (_introToLoopCoroutine != null)
		{
			StopCoroutine(_introToLoopCoroutine);
			_introToLoopCoroutine = null;
		}
		_bgmRequestId++;
		_bgm.Stop();
		_bgm.LockAutoFade(locked: true);
		_bgm.PlayOneShotReplace(introBGM, loopForever: false);
		int bgmRequestId = _bgmRequestId;
		_introToLoopCoroutine = StartCoroutine(IntroToLoop(loopBGM, bgmRequestId));
	}

	private IEnumerator IntroToLoop(string loopBGM, int reqId)
	{
		EventInstance current = _bgm.Current;
		if (current.isValid())
		{
			current.getDescription(out var description);
			description.getLength(out var length);
			yield return new WaitForSecondsRealtime((float)length / 1000f);
			if (reqId == _bgmRequestId)
			{
				_bgm.PlayOneShotReplace(loopBGM);
			}
		}
	}

	public void ResetBGM()
	{
		_bgm.Stop();
		_bgm.ResetState();
	}

	public void SoftResetBGM()
	{
		_bgm.SoftReset();
	}

	public void SetApplicationPaused(bool paused, bool force = false)
	{
		if (_inited && (force || _applicationPaused != paused))
		{
			_applicationPaused = paused;
			_bgm.SetExternallyPaused(paused);
			_atmos.SetExternallyPaused(paused);
			SetMasterBusVolumePaused(paused);
			SetBusPaused("bus:/Music", paused);
			SetBusPaused("bus:/Atmos", paused);
		}
	}

	private void SetMasterBusVolumePaused(bool paused)
	{
		try
		{
			Bus bus = RuntimeManager.GetBus("bus:/");
			if (paused)
			{
				if (!_applicationPauseMasterBusStateCaptured)
				{
					bus.getVolume(out _masterBusVolumeBeforeApplicationPause);
					_applicationPauseMasterBusStateCaptured = true;
				}
				bus.setVolume(0f);
			}
			else if (_applicationPauseMasterBusStateCaptured)
			{
				bus.setVolume(_masterBusVolumeBeforeApplicationPause);
				_applicationPauseMasterBusStateCaptured = false;
			}
		}
		catch
		{
		}
	}

	private static void SetBusPaused(string path, bool paused)
	{
		try
		{
			RuntimeManager.GetBus(path).setPaused(paused);
		}
		catch
		{
		}
	}

	public void PlayAtmos(string[] playlist, bool random)
	{
		_atmos.PlayPlaylist(playlist, random);
	}

	public bool IsCurrentAtomPlaylist(string[] playlist)
	{
		return _atmos.IsSamePlaylist(playlist);
	}

	public void StopAtmos()
	{
		if (_atmosFadeCoroutine != null)
		{
			StopCoroutine(_atmosFadeCoroutine);
			_atmosFadeCoroutine = null;
		}
		_atmos.Stop();
		_atmos.ResetState();
	}

	public void SceneSFX(Transform trans, string audioName)
	{
		RuntimeManager.PlayOneShot(audioName, trans.position);
	}

	public void SceneOpen(Transform trans, int a)
	{
		RuntimeManager.PlayOneShot(audioData.SoundChest[a], trans.position);
	}

	public void SceneBreakOBJ(Transform trans, int a)
	{
		RuntimeManager.PlayOneShot(audioData.SoundBreak[a], trans.position);
	}

	public void SceneEatTemple(Transform trans, int a)
	{
		RuntimeManager.PlayOneShot(audioData.SoundTemple[a], trans.position);
	}

	public void SceneStartUI(int a)
	{
		RuntimeManager.PlayOneShot(audioData.StartSceneUI[a]);
	}

	public void SoundString(Transform trans, string audioName)
	{
		RuntimeManager.PlayOneShot(audioName, trans.position);
	}

	public void PlaySO_Item(int itemType, string wpType, int index, int useType)
	{
		switch (itemType)
		{
		case 0:
			PlayWeaponPut(wpType, index);
			break;
		case 1:
			RuntimeManager.PlayOneShot(audioData.Baoshi.Put[index]);
			break;
		case 2:
			PlayUseItemPut(useType, index);
			break;
		}
	}

	public void PlaySO_UseItem(int index, int useType)
	{
		switch (useType)
		{
		case 2:
			RuntimeManager.PlayOneShot(audioData.Scoll.Use[index], base.transform.position);
			break;
		case 4:
			RuntimeManager.PlayOneShot(audioData.SPC.Use[index], base.transform.position);
			break;
		case 6:
			RuntimeManager.PlayOneShot(audioData.SPC.Use[index], base.transform.position);
			break;
		case 7:
			RuntimeManager.PlayOneShot(audioData.SPC.Use[index], base.transform.position);
			break;
		default:
			RuntimeManager.PlayOneShot(audioData.Potion.Use[index], base.transform.position);
			break;
		}
	}

	private void PlayWeaponPut(string type, int index)
	{
		if (type == null)
		{
			return;
		}
		switch (type.Length)
		{
		default:
			return;
		case 4:
			switch (type[1])
			{
			default:
				return;
			case 'e':
				if (type == "head")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Head.Put[index]);
				}
				return;
			case 'o':
				break;
			case 'a':
				if (type == "hand")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Hand.Put[index]);
				}
				return;
			}
			if (!(type == "body"))
			{
				if (!(type == "bone"))
				{
					return;
				}
				break;
			}
			RuntimeManager.PlayOneShot(audioData.WP_Armor.Put[index]);
			return;
		case 3:
			switch (type[0])
			{
			case 'l':
				if (type == "leg")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Shoes.Put[index]);
				}
				break;
			case 'b':
				if (type == "bow")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Bow.Put[index]);
				}
				break;
			}
			return;
		case 5:
			switch (type[1])
			{
			case 'w':
				if (type == "sword")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Sword.Put[index]);
				}
				return;
			case 't':
				break;
			case 'p':
				if (type == "spell")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Book.Put[index]);
				}
				return;
			case 'r':
				if (type == "arrow")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Arrow.Put[index]);
				}
				return;
			default:
				return;
			}
			if (!(type == "staff"))
			{
				return;
			}
			break;
		case 6:
			switch (type[0])
			{
			case 's':
				if (type == "shield")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Dun.Put[index]);
				}
				break;
			case 'c':
				if (type == "corpse")
				{
					RuntimeManager.PlayOneShot(audioData.WP_Offering.Put[index]);
				}
				break;
			case 'l':
				if (type == "little")
				{
					RuntimeManager.PlayOneShot(audioData.WP_ORB.Put[index]);
				}
				break;
			}
			return;
		}
		RuntimeManager.PlayOneShot(audioData.WP_Staff.Put[index]);
	}

	private void PlayUseItemPut(int useType, int index)
	{
		switch (useType)
		{
		case 2:
			RuntimeManager.PlayOneShot(audioData.Scoll.Put[index], base.transform.position);
			break;
		case 4:
			RuntimeManager.PlayOneShot(audioData.SPC.Put[index], base.transform.position);
			break;
		case 6:
			RuntimeManager.PlayOneShot(audioData.SPC.Put[index], base.transform.position);
			break;
		case 7:
			RuntimeManager.PlayOneShot(audioData.SPC.Put[index], base.transform.position);
			break;
		default:
			RuntimeManager.PlayOneShot(audioData.Potion.Put[index], base.transform.position);
			break;
		}
	}
}
