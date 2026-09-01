namespace SK.Framework;

public interface IMonoSingleton : ISingleton
{
	bool IsDontDestroyOnLoad { get; }
}
