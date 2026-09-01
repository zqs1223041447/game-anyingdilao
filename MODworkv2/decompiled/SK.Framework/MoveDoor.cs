using System.Collections;
using UnityEngine;

namespace SK.Framework;

public class MoveDoor : SwitchableDoor
{
	[SerializeField]
	private Vector3 direction = Vector3.left;

	[SerializeField]
	private float magnitude = 1f;

	private Coroutine switchCoroutine;

	private void Start()
	{
		switch (state)
		{
		case SwitchState.Open:
			openValue = base.transform.position;
			closeValue = base.transform.position + direction.normalized * magnitude;
			break;
		case SwitchState.Close:
			openValue = base.transform.position + direction.normalized * magnitude;
			closeValue = base.transform.position;
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
		Vector3 beginPos = base.transform.position;
		while (Time.time - beginTime < duration)
		{
			float t = (Time.time - beginTime) / duration;
			base.transform.position = Vector3.Lerp(beginPos, openValue, t);
			yield return null;
		}
		base.transform.position = openValue;
		switchCoroutine = null;
	}

	private IEnumerator CloseCoroutine()
	{
		float beginTime = Time.time;
		Vector3 beginPos = base.transform.position;
		while (Time.time - beginTime < duration)
		{
			float t = (Time.time - beginTime) / duration;
			base.transform.position = Vector3.Lerp(beginPos, closeValue, t);
			yield return null;
		}
		base.transform.position = closeValue;
		switchCoroutine = null;
	}
}
