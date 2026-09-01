using System.Collections;
using UnityEngine;

namespace SK.Framework;

public class RotateDoor : SwitchableDoor
{
	[SerializeField]
	private float angle = 90f;

	private Coroutine switchCoroutine;

	private void Start()
	{
		switch (state)
		{
		case SwitchState.Open:
			openValue = base.transform.forward + base.transform.position;
			closeValue = Quaternion.AngleAxis(angle, base.transform.up) * base.transform.forward + base.transform.position;
			break;
		case SwitchState.Close:
			openValue = Quaternion.AngleAxis(angle, base.transform.up) * base.transform.forward + base.transform.position;
			closeValue = base.transform.forward + base.transform.position;
			break;
		}
	}

	public override void Open()
	{
		if (state != 0)
		{
			state = SwitchState.Open;
			if (switchCoroutine != null)
			{
				StopCoroutine(switchCoroutine);
			}
			switchCoroutine = StartCoroutine(OpenCoroutine());
		}
	}

	public override void Close()
	{
		if (state != SwitchState.Close)
		{
			state = SwitchState.Close;
			if (switchCoroutine != null)
			{
				StopCoroutine(switchCoroutine);
			}
			switchCoroutine = StartCoroutine(CloseCoroutine());
		}
	}

	private IEnumerator OpenCoroutine()
	{
		float beginTime = Time.time;
		Quaternion beginRot = base.transform.rotation;
		Quaternion targetRot = Quaternion.LookRotation(openValue - base.transform.position, base.transform.up);
		while (Time.time - beginTime < duration)
		{
			float t = (Time.time - beginTime) / duration;
			base.transform.rotation = Quaternion.Lerp(beginRot, targetRot, t);
			yield return null;
		}
		base.transform.rotation = targetRot;
		switchCoroutine = null;
	}

	private IEnumerator CloseCoroutine()
	{
		float beginTime = Time.time;
		Quaternion beginRot = base.transform.rotation;
		Quaternion targetRot = Quaternion.LookRotation(closeValue - base.transform.position, base.transform.up);
		while (Time.time - beginTime < duration)
		{
			float t = (Time.time - beginTime) / duration;
			base.transform.rotation = Quaternion.Lerp(beginRot, targetRot, t);
			yield return null;
		}
		base.transform.rotation = targetRot;
		switchCoroutine = null;
	}
}
