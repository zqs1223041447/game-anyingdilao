namespace SK.Framework;

public interface IPoolable
{
	bool IsRecycled { get; set; }

	void OnRecycled();
}
