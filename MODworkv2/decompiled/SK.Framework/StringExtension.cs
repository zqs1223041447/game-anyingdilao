using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SK.Framework;

public static class StringExtension
{
	public static int CharCount(this string self, char target)
	{
		char[] array = self.ToCharArray();
		string text = target.ToString().ToLower();
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (text == array[i].ToString().ToLower())
			{
				num++;
			}
		}
		return num;
	}

	public static T ToEnum<T>(this string self)
	{
		try
		{
			return (T)Enum.Parse(typeof(T), self);
		}
		catch
		{
			return default(T);
		}
	}

	public static T ToEnum<T>(this string self, bool ignoreCase)
	{
		try
		{
			return (T)Enum.Parse(typeof(T), self, ignoreCase);
		}
		catch
		{
			return default(T);
		}
	}

	public static string UppercaseFirst(this string self)
	{
		return char.ToUpper(self[0]) + self.Substring(1);
	}

	public static bool FileExists(this string self)
	{
		return File.Exists(self);
	}

	public static bool DeleteFile(this string self)
	{
		if (File.Exists(self))
		{
			File.Delete(self);
			return true;
		}
		return false;
	}

	public static bool DirectoryExists(this string self)
	{
		return Directory.Exists(self);
	}

	public static string CreateDirectory(this string self)
	{
		if (!Directory.Exists(self))
		{
			Directory.CreateDirectory(self);
		}
		return self;
	}

	public static bool DeleteDirectory(this string self)
	{
		if (Directory.Exists(self))
		{
			Directory.Delete(self);
			return true;
		}
		return false;
	}

	public static string PathCombine(this string self, string beCombined)
	{
		return Path.Combine(self, beCombined);
	}

	public static string ToBase64String(this string self)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(self));
	}

	public static bool IsContainChinese(this string self)
	{
		return Regex.IsMatch(self, "[\\u4e00-\\u9fa5]");
	}

	public static bool IsMatchHexadecimal(this string self)
	{
		return Regex.IsMatch(self, "/^#?([a-f0-9]{6}|[a-f0-9]{3})$/");
	}

	public static bool IsMatchURL(this string self)
	{
		return Regex.IsMatch(self, "/^(https?:\\/\\/)?([\\da-z\\.-]+)\\.([a-z\\.]{2,6})([\\/\\w \\.-]*)*\\/?$/");
	}

	public static bool IsMatchIPAddress(this string self)
	{
		return Regex.IsMatch(self, "/^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/");
	}

	public static bool IsMatchEmail(this string self)
	{
		return Regex.IsMatch(self, "^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$");
	}

	public static bool IsMatchMobilePhoneNumber(this string self)
	{
		return Regex.IsMatch(self, "^0{0,1}(13[4-9]|15[7-9]|15[0-2]|18[7-8])[0-9]{8}$");
	}
}
