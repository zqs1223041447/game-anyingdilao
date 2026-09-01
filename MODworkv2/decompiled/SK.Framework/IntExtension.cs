using System;

namespace SK.Framework;

public static class IntExtension
{
	public static char ToLetter(this int self)
	{
		if (self < 1 || self > 26)
		{
			return '\0';
		}
		return Convert.ToChar(65 + self - 1);
	}

	public static int Fact(this int self)
	{
		if (self == 0)
		{
			return 1;
		}
		return self * (self - 1).Fact();
	}
}
