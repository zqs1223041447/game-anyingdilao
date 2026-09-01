using FinkFramework.Runtime.Singleton;

namespace Skill;

public class WeaponSpcManager : SingletonMonoScope<WeaponSpcManager>
{
	protected override void OnSingletonAwake()
	{
		base.OnSingletonAwake();
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}
}
