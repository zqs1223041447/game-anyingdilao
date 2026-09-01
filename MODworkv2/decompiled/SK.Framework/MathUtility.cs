namespace SK.Framework;

public static class MathUtility
{
	public static float Max(params float[] floatArray)
	{
		float num = floatArray[0];
		for (int i = 1; i < floatArray.Length; i++)
		{
			float num2 = floatArray[i];
			num = ((num > num2) ? num : num2);
		}
		return num;
	}

	public static int Max(params int[] intArray)
	{
		int num = intArray[0];
		for (int i = 1; i < intArray.Length; i++)
		{
			int num2 = intArray[i];
			num = ((num > num2) ? num : num2);
		}
		return num;
	}
}
