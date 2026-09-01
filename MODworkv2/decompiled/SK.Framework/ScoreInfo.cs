using System;
using UnityEngine;

namespace SK.Framework;

[Serializable]
public class ScoreInfo
{
	[ScoreID]
	public int id;

	[TextArea]
	public string description;

	public float value;
}
