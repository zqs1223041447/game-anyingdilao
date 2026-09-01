using UnityEngine;

namespace SK.Framework;

public abstract class SwitchableDoor : SwitchableObject
{
	[SerializeField]
	protected float duration = 0.5f;

	protected Vector3 openValue;

	protected Vector3 closeValue;
}
