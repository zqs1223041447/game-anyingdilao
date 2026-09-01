using System;
using System.Collections;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Steamworks;
using UI.Panels;
using UnityEngine;

namespace Core;

public class ApplicationPauseManager : SingletonMonoGlobal<ApplicationPauseManager>
{
	[Flags]
	private enum AutoPauseReason
	{
		None = 0,
		FocusLost = 1,
		ApplicationPause = 2,
		SteamOverlay = 4,
		SteamDeckInactive = 8
	}

	private const uint SteamDeckInactiveSeconds = 1u;

	private AutoPauseReason _activeReasons;

	private bool _inited;

	private bool _steamOverlaySubscribed;

	private bool _steamDeckInactive;

	private bool _autoPauseApplied;

	private bool _restoreTimeScaleOnResume;

	private bool _resumePlayTimeOnResume;

	private float _timeScaleBeforeAutoPause = 1f;

	private Coroutine _audioReapplyRoutine;

	public void Init()
	{
		if (!_inited)
		{
			_inited = true;
			TrySubscribeSteamOverlay();
		}
	}

	private void Update()
	{
		if (!_inited)
		{
			return;
		}
		TrySubscribeSteamOverlay();
		PollSteamDeckActivity();
		if (HasActiveReason())
		{
			if (!Mathf.Approximately(Time.timeScale, 0f))
			{
				_timeScaleBeforeAutoPause = Time.timeScale;
				_restoreTimeScaleOnResume = true;
				Time.timeScale = 0f;
			}
			if (GameManager.gameInited && PlayTimeManager.IsRunning)
			{
				PlayTimeManager.StopCount();
				_resumePlayTimeOnResume = true;
			}
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (_inited)
		{
			SetReason(AutoPauseReason.FocusLost, !hasFocus);
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (_inited)
		{
			SetReason(AutoPauseReason.ApplicationPause, pauseStatus);
		}
	}

	protected override void OnDestroy()
	{
		UnsubscribeSteamOverlay();
		base.OnDestroy();
	}

	private void TrySubscribeSteamOverlay()
	{
		if (!_steamOverlaySubscribed && SteamManager.IsSteamReady())
		{
			SteamFriends.OnGameOverlayActivated += HandleSteamOverlayActivated;
			_steamOverlaySubscribed = true;
		}
	}

	private void UnsubscribeSteamOverlay()
	{
		if (_steamOverlaySubscribed)
		{
			SteamFriends.OnGameOverlayActivated -= HandleSteamOverlayActivated;
			_steamOverlaySubscribed = false;
		}
	}

	private void HandleSteamOverlayActivated(bool active)
	{
		SetReason(AutoPauseReason.SteamOverlay, active);
	}

	private void PollSteamDeckActivity()
	{
		uint seconds;
		if (!SteamManager.IsRunningOnSteamDeck())
		{
			if (_steamDeckInactive)
			{
				_steamDeckInactive = false;
				SetReason(AutoPauseReason.SteamDeckInactive, active: false);
			}
		}
		else if (SteamManager.TryGetSecondsSinceAppActive(out seconds))
		{
			bool flag = seconds >= 1;
			if (flag != _steamDeckInactive)
			{
				_steamDeckInactive = flag;
				SetReason(AutoPauseReason.SteamDeckInactive, flag);
			}
		}
	}

	private void SetReason(AutoPauseReason reason, bool active)
	{
		bool flag = HasActiveReason();
		if (active)
		{
			_activeReasons |= reason;
		}
		else
		{
			_activeReasons &= ~reason;
		}
		bool flag2 = HasActiveReason();
		if (!flag && flag2)
		{
			ApplyAutoPause();
		}
		else if (flag && !flag2)
		{
			ReleaseAutoPause();
		}
		else
		{
			ScheduleAudioReapply();
		}
	}

	private bool HasActiveReason()
	{
		return _activeReasons != AutoPauseReason.None;
	}

	private void ApplyAutoPause()
	{
		_autoPauseApplied = true;
		_timeScaleBeforeAutoPause = Time.timeScale;
		_restoreTimeScaleOnResume = !Mathf.Approximately(Time.timeScale, 0f);
		Time.timeScale = 0f;
		if (GameManager.gameInited && PlayTimeManager.IsRunning)
		{
			PlayTimeManager.StopCount();
			_resumePlayTimeOnResume = true;
		}
		else
		{
			_resumePlayTimeOnResume = false;
		}
		SetAudioPaused(paused: true, force: true);
		ScheduleAudioReapply();
	}

	private void ReleaseAutoPause()
	{
		if (_autoPauseApplied)
		{
			SetAudioPaused(paused: false, force: true);
			if (_restoreTimeScaleOnResume && Mathf.Approximately(Time.timeScale, 0f) && !IsKnownTimeScalePauseActive())
			{
				Time.timeScale = Mathf.Max(0.0001f, _timeScaleBeforeAutoPause);
			}
			if (_resumePlayTimeOnResume && GameManager.gameInited && !PlayTimeManager.IsRunning)
			{
				PlayTimeManager.StartCount();
			}
			_autoPauseApplied = false;
			_restoreTimeScaleOnResume = false;
			_resumePlayTimeOnResume = false;
			_timeScaleBeforeAutoPause = 1f;
			ScheduleAudioReapply();
		}
	}

	private void SetAudioPaused(bool paused, bool force)
	{
		if (SingletonMonoGlobal<AudioManager>.HasInstance)
		{
			SingletonMonoGlobal<AudioManager>.Instance.SetApplicationPaused(paused, force);
		}
	}

	private static bool IsKnownTimeScalePauseActive()
	{
		if (!GameManager.gameInited)
		{
			return false;
		}
		try
		{
			return Singleton<UIManager>.Instance.IsPanelOpened<PausePanel>() || Singleton<UIManager>.Instance.IsPanelOpened<SettingPanel>() || Singleton<UIManager>.Instance.IsPanelOpened<TeleportPanel>() || Singleton<UIManager>.Instance.IsPanelOpened<MijingPanel>();
		}
		catch
		{
			return false;
		}
	}

	private void ScheduleAudioReapply()
	{
		if (_inited && base.isActiveAndEnabled)
		{
			if (_audioReapplyRoutine != null)
			{
				StopCoroutine(_audioReapplyRoutine);
			}
			_audioReapplyRoutine = StartCoroutine(ReapplyAudioNextFrame());
		}
	}

	private IEnumerator ReapplyAudioNextFrame()
	{
		yield return null;
		SetAudioPaused(HasActiveReason(), force: true);
		_audioReapplyRoutine = null;
	}
}
