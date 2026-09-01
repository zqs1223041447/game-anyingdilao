namespace SK.Framework;

public static class ReflectionExtension
{
	public static object GetFieldValue(this object self, string fieldName)
	{
		return self.GetType().GetField(fieldName)?.GetValue(self);
	}

	public static object GetPropertyValue(this object self, string propertyName, object[] index = null)
	{
		return self.GetType().GetProperty(propertyName)?.GetValue(self, index);
	}

	public static object ExecuteMethod(this object self, string methodName, params object[] args)
	{
		return self.GetType().GetMethod(methodName)?.Invoke(self, args);
	}
}
