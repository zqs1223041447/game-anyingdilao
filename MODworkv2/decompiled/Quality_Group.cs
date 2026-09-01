using System;
using System.Collections.Generic;

[Serializable]
public class Quality_Group
{
	public List<Item_MB> Normal = new List<Item_MB>();

	public List<Item_MB> Magic = new List<Item_MB>();

	public List<Item_MB> Rare = new List<Item_MB>();

	public List<Item_MB> Exquisite = new List<Item_MB>();

	public List<Item_MB> Epic = new List<Item_MB>();

	public List<Item_MB> Legendary = new List<Item_MB>();

	public List<Item_MB> Mythical = new List<Item_MB>();
}
