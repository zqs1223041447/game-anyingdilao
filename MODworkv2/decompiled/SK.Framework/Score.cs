namespace SK.Framework;

public class Score
{
	public static string Create(int id)
	{
		return ScoreMaster.Instance.Create(id);
	}

	public static string[] CreateGroup(string groupDescription, ValueMode valueMode, params int[] idArray)
	{
		return ScoreMaster.Instance.CreateGroup(groupDescription, valueMode, idArray);
	}

	public static bool Delete(string flag)
	{
		return ScoreMaster.Instance.Delete(flag);
	}

	public static bool DeleteGroup(string groupDescription)
	{
		return ScoreMaster.Instance.DeleteGroup(groupDescription);
	}

	public static bool DeleteGroupItem(string groupDescription, string flag)
	{
		return ScoreMaster.Instance.DeleteGroupItem(groupDescription, flag);
	}

	public static bool Obtain(string flag)
	{
		return ScoreMaster.Instance.Obtain(flag);
	}

	public static bool Obtain(string groupDescription, string flag)
	{
		return ScoreMaster.Instance.Obtain(groupDescription, flag);
	}

	public static bool Cancle(string flag)
	{
		return ScoreMaster.Instance.Cancle(flag);
	}

	public static bool Cancle(string groupDescription, string flag)
	{
		return ScoreMaster.Instance.Cancle(groupDescription, flag);
	}

	public static float GetGroupSum(string groupDescription)
	{
		return ScoreMaster.Instance.GetGroupSum(groupDescription);
	}

	public static float GetSum()
	{
		return ScoreMaster.Instance.GetSum();
	}
}
