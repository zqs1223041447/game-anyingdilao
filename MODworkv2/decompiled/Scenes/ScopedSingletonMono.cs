using FinkFramework.Runtime.Singleton;
using UnityEngine;

namespace Scenes;

public class ScopedSingletonMono<T> : SingletonMonoScope<T> where T : MonoBehaviour
{
	private const ProcessScope scope = ProcessScope.Game;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.RegisterToScope(this, ProcessScope.Game);
	}

	protected override void OnDestroy()
	{
		if (SingletonMonoGlobal<SessionManager>.HasInstance)
		{
			SingletonMonoGlobal<SessionManager>.Instance.UnregisterFromScope(this, ProcessScope.Game);
		}
	}
}
