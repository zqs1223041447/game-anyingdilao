using System;
using FinkFramework.Runtime.Singleton;

public class DisplayItemManager : SingletonMonoScope<DisplayItemManager>
{
	public bool DropItemUI_IsOpened;

	public Action DropUIOn;

	public Action DropUIOff;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
	}

	public void ChangeItemUI_On()
	{
		DropUIOn?.Invoke();
	}

	public void ChangeItemUI_Off()
	{
		DropUIOff?.Invoke();
	}
}
