namespace SK.Framework;

public interface IObjectPool<T>
{
	int CurrentCacheCount { get; }

	int MaxCacheCount { get; set; }

	T Allocate();

	bool Recycle(T t);

	void Release();
}
