using FinkFramework.Runtime.Utils;
using Scenes;

namespace Obsoletes;

public class TestManager : ScopedSingletonMono<TestManager>
{
	protected override void Awake()
	{
		base.Awake();
		LogUtil.Info("测试执行Awake");
	}
}
