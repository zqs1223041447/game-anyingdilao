using System;

namespace SK.Framework;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PackageAttribute : Attribute
{
	public string Name { get; private set; }

	public string Version { get; private set; }

	public string Path { get; private set; }

	public PackageAttribute(string name, string version, string path)
	{
		Name = name;
		Version = version;
		Path = path;
	}

	public override string ToString()
	{
		return Name + "-" + Version;
	}
}
